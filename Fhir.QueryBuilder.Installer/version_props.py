"""
讀寫儲存庫根目錄 Fhir.QueryBuilder.Version.props（FHIRQueryBuilderVersion）。
供 build_installer.py 與獨立遞增版號使用。
"""

from __future__ import annotations

import re
import sys
from pathlib import Path

PROPS_FILENAME = "Fhir.QueryBuilder.Version.props"
TAG_PATTERN = re.compile(
    r"(<FHIRQueryBuilderVersion\s*>)\s*([^<]*?)\s*(</FHIRQueryBuilderVersion\s*>)",
    re.DOTALL,
)
SEMVER_PATTERN = re.compile(r"^(\d+)\.(\d+)\.(\d+)$")


def props_path(repo_root: Path) -> Path:
    return (repo_root / PROPS_FILENAME).resolve()


def read_version(repo_root: Path) -> str:
    path = props_path(repo_root)
    if not path.is_file():
        raise FileNotFoundError(path)
    text = path.read_text(encoding="utf-8")
    m = TAG_PATTERN.search(text)
    if not m:
        raise ValueError(f"找不到 <FHIRQueryBuilderVersion>：{path}")
    ver = (m.group(2) or "").strip()
    if not ver:
        raise ValueError(f"FHIRQueryBuilderVersion 為空：{path}")
    return ver


def write_version(repo_root: Path, version: str) -> None:
    path = props_path(repo_root)
    text = path.read_text(encoding="utf-8")

    def repl(match: re.Match[str]) -> str:
        return f"{match.group(1)}{version}{match.group(3)}"

    new_text, n = TAG_PATTERN.subn(repl, text, count=1)
    if n != 1:
        raise ValueError(f"無法寫入 FHIRQueryBuilderVersion：{path}")
    path.write_text(new_text, encoding="utf-8")


def bump_patch(version: str) -> str:
    m = SEMVER_PATTERN.match(version.strip())
    if not m:
        raise ValueError(f"版號須為 Major.Minor.Patch 三段數字，目前為：{version!r}")
    major, minor, patch = (int(m.group(i)) for i in range(1, 4))
    return f"{major}.{minor}.{patch + 1}"


def main(argv: list[str]) -> None:
    """獨立執行：python version_props.py bump | show"""
    repo_root = Path(__file__).resolve().parent.parent
    cmd = argv[1] if len(argv) > 1 else "show"
    if cmd == "show":
        print(read_version(repo_root))
        return
    if cmd == "bump":
        v = read_version(repo_root)
        nv = bump_patch(v)
        write_version(repo_root, nv)
        print(nv)
        return
    print("用法: version_props.py [show|bump]", file=sys.stderr)
    raise SystemExit(2)


if __name__ == "__main__":
    main(sys.argv)
