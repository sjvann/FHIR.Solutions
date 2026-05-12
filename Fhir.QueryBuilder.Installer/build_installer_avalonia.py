#!/usr/bin/env python3
"""
產出 FHIR Query Builder — Avalonia 桌面版之 Windows 離線安裝程式（Inno Setup 6）。
1) dotnet publish win-x64 自包含（無需客戶端另行安裝 .NET Runtime）
2) 呼叫 ISCC 編譯 FHIRQueryBuilder-Avalonia.iss

版號與 Blazor 安裝程式共用 Fhir.QueryBuilder.Version.props（FHIRQueryBuilderVersion）。
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
    parser = argparse.ArgumentParser(
        description="建置 FHIR Query Builder Avalonia 桌面版 Windows 安裝程式",
    )
    parser.add_argument(
        "-c",
        "--configuration",
        default="Release",
        help="組態（預設 Release）",
    )
    parser.add_argument(
        "--no-bump",
        action="store_true",
        help="不自動遞增 Fhir.QueryBuilder.Version.props 的 patch",
    )
    args = parser.parse_args()

    installer_dir = Path(__file__).resolve().parent
    repo_root = installer_dir.parent

    staging_app = repo_root / "artifacts" / "FHIRQueryBuilderAvaloniaInstallerStaging" / "app"
    avalonia_project = repo_root / "Fhir.QueryBuilder.App.Avalonia" / "Fhir.QueryBuilder.App.Avalonia.csproj"
    iss_file = installer_dir / "FHIRQueryBuilder-Avalonia.iss"

    for p in (avalonia_project, iss_file):
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

    print(f"Publishing Avalonia (win-x64 self-contained) -> {staging_app}")
    if staging_app.is_dir():
        shutil.rmtree(staging_app)
    staging_app.mkdir(parents=True, exist_ok=True)

    _run(
        [
            "dotnet",
            "publish",
            str(avalonia_project),
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

    exe_path = staging_app / "Fhir.QueryBuilder.App.Avalonia.exe"
    if not exe_path.is_file():
        print(f"找不到主程式：{exe_path}", file=sys.stderr)
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
        artifacts_dir.glob("FHIRQueryBuilder-Avalonia-Setup-*.exe"),
        key=lambda p: p.stat().st_mtime,
        reverse=True,
    )
    if setups:
        print()
        print("安裝程式已產生：")
        print(f"  {setups[0]}")


if __name__ == "__main__":
    main()
