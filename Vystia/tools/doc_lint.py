#!/usr/bin/env python
"""Doc lint: flags missing Last Updated, duplicate claims, and complete assertions without code pointers."""

from __future__ import annotations

import argparse
import re
from collections import defaultdict
from pathlib import Path

KEYWORDS = (
    "complete",
    "fully implemented",
    "100%",
    "done",
    "finished",
    "ready",
)

LAST_UPDATED_RE = re.compile(r"(?i)last updated\s*:")
CODE_PTR_RE = re.compile(r"`[^`]*(ServUO/|\\.cs)[^`]*`|ServUO/|\\.cs\\b")

SKIP_DIRS = {".git", "archive", "tools", "bin", "obj"}


def iter_md_files(root: Path):
    for path in root.rglob("*.md"):
        if any(part in SKIP_DIRS for part in path.parts):
            continue
        yield path


def normalize_line(line: str) -> str:
    return re.sub(r"\s+", " ", line.strip().lower())


def has_keyword(line: str) -> bool:
    lower = line.lower()
    return any(k in lower for k in KEYWORDS)


def find_code_pointer(line: str) -> bool:
    return bool(CODE_PTR_RE.search(line))


def lint_file(path: Path):
    try:
        text = path.read_text(encoding="utf-8", errors="ignore")
    except Exception:
        return None

    lines = text.splitlines()
    has_last_updated = any(LAST_UPDATED_RE.search(line) for line in lines)

    missing_code_pointer = []
    for idx, line in enumerate(lines):
        if not has_keyword(line):
            continue
        if find_code_pointer(line):
            continue
        prev_line = lines[idx - 1] if idx > 0 else ""
        next_line = lines[idx + 1] if idx + 1 < len(lines) else ""
        if find_code_pointer(prev_line) or find_code_pointer(next_line):
            continue
        missing_code_pointer.append((idx + 1, line.strip()))

    keyword_lines = [(idx + 1, line) for idx, line in enumerate(lines) if has_keyword(line)]

    return {
        "has_last_updated": has_last_updated,
        "missing_code_pointer": missing_code_pointer,
        "keyword_lines": keyword_lines,
    }


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--root", default="Vystia", help="Root folder to scan")
    parser.add_argument("--output", default="", help="Write report to file")
    args = parser.parse_args()

    root = Path(args.root)
    if not root.exists():
        raise SystemExit(f"Root not found: {root}")

    missing_last_updated = []
    missing_code_pointer = []
    duplicate_map = defaultdict(list)

    for path in iter_md_files(root):
        result = lint_file(path)
        if not result:
            continue
        if not result["has_last_updated"]:
            missing_last_updated.append(path)
        for line_no, line in result["missing_code_pointer"]:
            missing_code_pointer.append((path, line_no, line))
        for line_no, line in result["keyword_lines"]:
            norm = normalize_line(line)
            if norm:
                duplicate_map[norm].append((path, line_no, line.strip()))

    duplicates = {k: v for k, v in duplicate_map.items() if len(v) > 1}

    out = []
    out.append("DOC LINT REPORT")
    out.append("=")
    out.append("")

    out.append("Missing Last Updated:")
    if missing_last_updated:
        for path in sorted(missing_last_updated):
            out.append(f"- {path}")
    else:
        out.append("- None")
    out.append("")

    out.append("Complete/ready claims without code pointer:")
    if missing_code_pointer:
        for path, line_no, line in missing_code_pointer:
            out.append(f"- {path}:{line_no}: {line}")
    else:
        out.append("- None")
    out.append("")

    out.append("Duplicate claims (same line appears in multiple files):")
    if duplicates:
        for norm, entries in sorted(duplicates.items(), key=lambda kv: (-len(kv[1]), kv[0])):
            out.append(f"- '{norm}' ({len(entries)} files)")
            for path, line_no, line in entries:
                out.append(f"  - {path}:{line_no}: {line}")
    else:
        out.append("- None")

    report = "\n".join(out)
    if args.output:
        Path(args.output).write_text(report, encoding="utf-8")
    else:
        print(report)


if __name__ == "__main__":
    main()
