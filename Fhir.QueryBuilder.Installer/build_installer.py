#!/usr/bin/env python3
"""
產出 Windows 安裝程式（需已安裝 Inno Setup 6，ISCC.exe）。
1) 發佈 Blazor WASM → wwwroot
2) 發佈本機 Kestrel 啟動器（win-x64 自包含）
3) 合併至 staging 並呼叫 ISCC 編譯 FHIRQueryBuilder.iss
"""

from __future__ import annotations

import argparse
import os
import shutil
import subprocess
import sys
from pathlib import Path

from version_props import bump_patch, read_version, write_version


def _run(cmd: list[str], *, cwd: Path | None = None) -> None:
    r = subprocess.run(cmd, cwd=cwd)
    if r.returncode != 0:
        raise SystemExit(f"指令失敗 (exit {r.returncode}): {' '.join(cmd)}")


def _stamp_wwwroot_asset_versions(wwwroot: Path, version: str) -> None:
    """將 index.html 內 ?v=dev 換成當次發佈版號，避免 favicon／manifest 快取舊資源。"""
    index = wwwroot / "index.html"
    if not index.is_file():
        return
    text = index.read_text(encoding="utf-8")
    needle = "?v=dev"
    replacement = f"?v={version}"
    if needle not in text:
        print(
            f"注意：{index} 未包含 {needle!r}，略過資源版號替換。",
            file=sys.stderr,
        )
        return
    index.write_text(text.replace(needle, replacement), encoding="utf-8")


def _find_iscc() -> Path | None:
    candidates = [
        Path(os.environ.get("ProgramFiles(x86)", "")) / "Inno Setup 6" / "ISCC.exe",
        Path(os.environ.get("ProgramFiles", "")) / "Inno Setup 6" / "ISCC.exe",
    ]
    for p in candidates:
        if p.is_file():
            return p
    return None


def main() -> None:
    parser = argparse.ArgumentParser(description="建置 FHIR Query Builder Windows 安裝程式")
    parser.add_argument(
        "-c",
        "--configuration",
        default="Release",
        help="組態（預設 Release）",
    )
    parser.add_argument(
        "--no-bump",
        action="store_true",
        help="不自動遞增 Fhir.QueryBuilder.Version.props 的 patch（重建同一版號時使用）",
    )
    args = parser.parse_args()

    installer_dir = Path(__file__).resolve().parent
    repo_root = installer_dir.parent

    staging_app = repo_root / "artifacts" / "FHIRQueryBuilderInstallerStaging" / "app"
    blazor_publish = repo_root / "artifacts" / "FHIRQueryBuilderInstallerStaging" / "blazor-publish"

    blazor_project = repo_root / "Fhir.QueryBuilder.App.Blazor" / "Fhir.QueryBuilder.App.Blazor.csproj"
    host_project = repo_root / "Fhir.QueryBuilder.App.LocalHost" / "Fhir.QueryBuilder.App.LocalHost.csproj"
    iss_file = installer_dir / "FHIRQueryBuilder.iss"

    for p in (blazor_project, host_project, iss_file):
        if not p.is_file():
            print(f"找不到必要檔案：{p}", file=sys.stderr)
            raise SystemExit(1)

    if not args.no_bump:
        cur = read_version(repo_root)
        nxt = bump_patch(cur)
        write_version(repo_root, nxt)
        print(f"版號已遞增：{cur} → {nxt}（Fhir.QueryBuilder.Version.props）")
    release_version = read_version(repo_root)
    print(f"本次建置版號：{release_version}")

    print(f"Staging Blazor publish -> {blazor_publish}")
    if blazor_publish.is_dir():
        shutil.rmtree(blazor_publish)
    blazor_publish.mkdir(parents=True, exist_ok=True)

    _run(
        [
            "dotnet",
            "publish",
            str(blazor_project),
            "-c",
            args.configuration,
            "-o",
            str(blazor_publish),
        ]
    )

    wwwroot_src = blazor_publish / "wwwroot"
    if not wwwroot_src.is_dir():
        print(f"Blazor 發佈輸出缺少 wwwroot：{wwwroot_src}", file=sys.stderr)
        raise SystemExit(1)

    _stamp_wwwroot_asset_versions(wwwroot_src, release_version)

    print(f"Staging LocalHost (win-x64 self-contained) -> {staging_app}")
    if staging_app.is_dir():
        shutil.rmtree(staging_app)
    staging_app.mkdir(parents=True, exist_ok=True)

    _run(
        [
            "dotnet",
            "publish",
            str(host_project),
            "-c",
            args.configuration,
            "-r",
            "win-x64",
            "--self-contained",
            "true",
            "-o",
            str(staging_app),
            "-p:PublishReadyToRun=true",
        ]
    )

    print("Copying wwwroot...")
    wwwroot_dest = staging_app / "wwwroot"
    if wwwroot_dest.exists():
        shutil.rmtree(wwwroot_dest)
    shutil.copytree(wwwroot_src, wwwroot_dest)

    exe_path = staging_app / "FHIRQueryBuilder.exe"
    if not exe_path.is_file():
        print(f"找不到啟動程式：{exe_path}", file=sys.stderr)
        raise SystemExit(1)

    iscc = _find_iscc()
    if iscc is None:
        print()
        print("已準備好安裝檔內容於：")
        print(f"  {staging_app}")
        print()
        print("未找到 Inno Setup 6（ISCC.exe）。請安裝後再執行：")
        print("  https://jrsoftware.org/isdl.php")
        print()
        print("手動編譯安裝程式：")
        print(f'  "{iss_file}"')
        raise SystemExit(0)

    print(f"Compiling installer with: {iscc}")
    _run(
        [
            str(iscc),
            f"/DMyAppVersion={release_version}",
            str(iss_file),
        ],
        cwd=installer_dir,
    )

    artifacts_dir = repo_root / "artifacts"
    setups = sorted(
        artifacts_dir.glob("FHIRQueryBuilder-Setup-*.exe"),
        key=lambda p: p.stat().st_mtime,
        reverse=True,
    )
    if setups:
        print()
        print("安裝程式已產生：")
        print(f"  {setups[0]}")


if __name__ == "__main__":
    main()
