#!/usr/bin/env python3
"""
產出可交付物：
- Full：完整發佈（web.config + wwwroot），適 IIS／Azure App Service 等。
- Static：僅 wwwroot 內容，適 CDN、S3、GitHub Pages、nginx 純靜態（伺服端不需 .NET）。
終端使用者皆只需瀏覽器；不需安裝 .NET Runtime。
"""

from __future__ import annotations

import argparse
import shutil
import subprocess
import sys
import time
from datetime import datetime
from pathlib import Path


def _stamp_index_asset_query(full_dir_wwwroot: Path, version: str) -> None:
    """與 build_installer 相同：將 index.html 內 ?v=dev 換成正式版號。"""
    index = full_dir_wwwroot / "index.html"
    if not index.is_file():
        return
    text = index.read_text(encoding="utf-8")
    if "?v=dev" not in text:
        return
    index.write_text(text.replace("?v=dev", f"?v={version}"), encoding="utf-8")


def _read_query_builder_version(repo_root: Path) -> str | None:
    script = repo_root / "Fhir.QueryBuilder.Installer" / "version_props.py"
    if not script.is_file():
        return None
    r = subprocess.run(
        [sys.executable, str(script), "show"],
        capture_output=True,
        text=True,
        encoding="utf-8",
    )
    if r.returncode != 0:
        print(r.stderr or r.stdout, file=sys.stderr)
        return None
    return r.stdout.strip() or None


def _zip_directory_flat(src_dir: Path, zip_file: Path) -> None:
    """將目錄內容壓成 zip，檔案位於壓縮檔根目錄（等同 tar -C src .）。"""
    src_dir = src_dir.resolve()
    zip_file = zip_file.resolve()
    if zip_file.exists():
        zip_file.unlink()
    # 略候以避免防毒／索引程式鎖檔
    time.sleep(0.4)
    archive_base = zip_file.parent / zip_file.stem
    shutil.make_archive(str(archive_base), "zip", root_dir=str(src_dir))


def main() -> None:
    parser = argparse.ArgumentParser(description="發佈 Blazor WASM 可交付物（full / static / zip）")
    parser.add_argument(
        "-o",
        "--output-root",
        default="",
        metavar="DIR",
        help="輸出根目錄；省略則使用儲存庫 artifacts/Fhir.QueryBuilder.App.Blazor",
    )
    args = parser.parse_args()

    script_dir = Path(__file__).resolve().parent
    project_file = script_dir / "Fhir.QueryBuilder.App.Blazor.csproj"

    if not project_file.is_file():
        print(f"找不到專案：{project_file}", file=sys.stderr)
        raise SystemExit(1)

    output_root = Path(args.output_root) if args.output_root.strip() else None
    if output_root is None:
        output_root = script_dir.parent / "artifacts" / "Fhir.QueryBuilder.App.Blazor"

    stamp = datetime.now().strftime("%Y%m%d-%H%M%S")
    session_dir = output_root / stamp
    full_dir = session_dir / "full"
    static_dir = session_dir / "static"

    session_dir.mkdir(parents=True, exist_ok=True)

    print(f"Publishing Release -> {full_dir}")
    r = subprocess.run(
        [
            "dotnet",
            "publish",
            str(project_file),
            "-c",
            "Release",
            "-o",
            str(full_dir),
        ]
    )
    if r.returncode != 0:
        print(f"dotnet publish 失敗 (exit {r.returncode})", file=sys.stderr)
        raise SystemExit(r.returncode)

    wwwroot = full_dir / "wwwroot"
    if not wwwroot.is_dir():
        print(f"發佈輸出缺少 wwwroot：{wwwroot}", file=sys.stderr)
        raise SystemExit(1)

    repo_root = script_dir.parent
    qb_ver = _read_query_builder_version(repo_root)
    if qb_ver:
        _stamp_index_asset_query(wwwroot, qb_ver)

    print(f"Copying static site -> {static_dir}")
    if static_dir.exists():
        shutil.rmtree(static_dir)
    shutil.copytree(wwwroot, static_dir)

    zip_full = session_dir / "Fhir.QueryBuilder.App.Blazor-IIS.zip"
    zip_static = session_dir / "Fhir.QueryBuilder.App.Blazor-Static.zip"

    print("Zipping...")
    _zip_directory_flat(full_dir, zip_full)
    _zip_directory_flat(static_dir, zip_static)

    print()
    print("完成。")
    print(f"  Full（IIS 等）: {full_dir}")
    print(f"  Static（純靜態）: {static_dir}")
    print(f"  ZIP: {zip_full}")
    print(f"  ZIP: {zip_static}")


if __name__ == "__main__":
    main()
