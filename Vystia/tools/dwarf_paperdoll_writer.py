"""
Dwarf Paperdoll Writer for Ultima Online
Copies human paperdoll gumps to dwarf body gump IDs.

Copies:
- Gump 50400 (human male paperdoll) -> Gump 50987 (dwarf male paperdoll)
- Gump 60401 (human female paperdoll) -> Gump 60988 (dwarf female paperdoll)

Supports both MUL and UOP gump formats.

Usage:
    python dwarf_paperdoll_writer.py
"""

import os
import struct
import shutil
import hashlib
import zlib
from pathlib import Path
from datetime import datetime

# Configuration
UO_CLIENT_PATH = r"C:\Program Files (x86)\Electronic Arts\Ultima Online Classic"
BACKUP_PATH = r"C:\DevEnv\GIT\UO\Vystia\tools\backups"
OUTPUT_PATH = r"C:\DevEnv\GIT\UO\Vystia\tools\patched_client"

# Gump ID mappings: source -> target
GUMP_MAPPINGS = {
    50400: 50987,  # Human male -> Dwarf male paperdoll
    60401: 60988,  # Human female -> Dwarf female paperdoll
}


def backup_files(client_path, backup_path):
    """Create timestamped backups"""
    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    backup_dir = Path(backup_path) / f"gump_{timestamp}"
    backup_dir.mkdir(parents=True, exist_ok=True)

    for filename in ["gumpart.mul", "gumpidx.mul", "gumpartLegacyMUL.uop"]:
        src = Path(client_path) / filename
        if src.exists():
            dst = backup_dir / filename
            print(f"Backing up {src} -> {dst}")
            shutil.copy2(src, dst)

    print(f"Backups created in: {backup_dir}")
    return backup_dir


class UOPReader:
    """Reader for UOP (Ultima Online Package) files"""

    def __init__(self, uop_path):
        self.uop_path = uop_path
        self.entries = {}  # gump_id -> (offset, compressed_length, decompressed_length, is_compressed)
        self._hash_to_id = {}  # Pre-built hash->id lookup
        self._build_hash_table()
        self._parse_uop()

    def _build_hash_table(self):
        """Pre-build hash table for all possible gump IDs"""
        print("Building gump hash lookup table...")
        for gump_id in range(70000):  # Cover all paperdoll range
            h = self._compute_hash(gump_id)
            self._hash_to_id[h] = gump_id
        print(f"Built hash table with {len(self._hash_to_id)} entries")

    def _parse_uop(self):
        """Parse UOP file structure"""
        with open(self.uop_path, 'rb') as f:
            # Read header
            magic = f.read(4)
            if magic != b'MYP\x00':
                raise ValueError(f"Invalid UOP magic: {magic}")

            version, misc = struct.unpack('<II', f.read(8))
            first_table_offset = struct.unpack('<Q', f.read(8))[0]
            block_size, file_count = struct.unpack('<II', f.read(8))

            print(f"UOP: version={version}, files={file_count}, first_table={first_table_offset}")

            # Read file tables
            table_offset = first_table_offset
            while table_offset != 0:
                f.seek(table_offset)
                entry_count = struct.unpack('<I', f.read(4))[0]
                next_table = struct.unpack('<Q', f.read(8))[0]

                for i in range(entry_count):
                    data_offset = struct.unpack('<Q', f.read(8))[0]
                    header_length = struct.unpack('<I', f.read(4))[0]
                    compressed_length = struct.unpack('<I', f.read(4))[0]
                    decompressed_length = struct.unpack('<I', f.read(4))[0]
                    file_hash = struct.unpack('<Q', f.read(8))[0]
                    adler32 = struct.unpack('<I', f.read(4))[0]
                    is_compressed = struct.unpack('<H', f.read(2))[0]

                    if data_offset == 0:
                        continue

                    # Decode gump ID from hash
                    gump_id = self._hash_to_id.get(file_hash)
                    if gump_id is not None:
                        self.entries[gump_id] = (
                            data_offset + header_length,
                            compressed_length,
                            decompressed_length,
                            is_compressed
                        )
                    else:
                        # Store unmatched hashes for debugging
                        if not hasattr(self, '_unmatched'):
                            self._unmatched = []
                        if len(self._unmatched) < 10:
                            self._unmatched.append(file_hash)

                table_offset = next_table

        print(f"Parsed {len(self.entries)} gump entries from UOP")
        if hasattr(self, '_unmatched') and self._unmatched:
            print(f"Sample unmatched hashes from UOP: {[hex(h) for h in self._unmatched]}")
        # Debug: show some found entries
        found_ids = sorted(self.entries.keys())[:20]
        print(f"Sample gump IDs found: {found_ids}")

    def _compute_hash(self, gump_id):
        """Compute UOP hash for a gump ID"""
        filename = f"build/gumpartlegacymul/{gump_id:08d}.tga"
        return self._create_hash(filename)

    def _create_hash(self, s):
        """UO UOP hash function - exact port from ClassicUO"""
        length = len(s)
        ebx = edi = esi = (length + 0xDEADBEEF) & 0xFFFFFFFF
        eax = 0

        i = 0
        while i + 12 < length:
            edi = (edi + ((ord(s[i+7]) << 24) | (ord(s[i+6]) << 16) | (ord(s[i+5]) << 8) | ord(s[i+4]))) & 0xFFFFFFFF
            esi = (esi + ((ord(s[i+11]) << 24) | (ord(s[i+10]) << 16) | (ord(s[i+9]) << 8) | ord(s[i+8]))) & 0xFFFFFFFF
            edx = (((ord(s[i+3]) << 24) | (ord(s[i+2]) << 16) | (ord(s[i+1]) << 8) | ord(s[i])) - esi) & 0xFFFFFFFF
            edx = ((edx + ebx) ^ ((esi >> 28) | (esi << 4))) & 0xFFFFFFFF
            esi = (esi + edi) & 0xFFFFFFFF
            edi = ((edi - edx) ^ ((edx >> 26) | (edx << 6))) & 0xFFFFFFFF
            edx = (edx + esi) & 0xFFFFFFFF
            esi = ((esi - edi) ^ ((edi >> 24) | (edi << 8))) & 0xFFFFFFFF
            edi = (edi + edx) & 0xFFFFFFFF
            ebx = ((edx - esi) ^ ((esi >> 16) | (esi << 16))) & 0xFFFFFFFF
            esi = (esi + edi) & 0xFFFFFFFF
            edi = ((edi - ebx) ^ ((ebx >> 13) | (ebx << 19))) & 0xFFFFFFFF
            ebx = (ebx + esi) & 0xFFFFFFFF
            esi = ((esi - edi) ^ ((edi >> 28) | (edi << 4))) & 0xFFFFFFFF
            edi = (edi + ebx) & 0xFFFFFFFF
            i += 12

        # Handle remaining bytes with fallthrough logic
        remaining = length - i
        if remaining >= 12:
            esi = (esi + (ord(s[i+11]) << 24)) & 0xFFFFFFFF
        if remaining >= 11:
            esi = (esi + (ord(s[i+10]) << 16)) & 0xFFFFFFFF
        if remaining >= 10:
            esi = (esi + (ord(s[i+9]) << 8)) & 0xFFFFFFFF
        if remaining >= 9:
            esi = (esi + ord(s[i+8])) & 0xFFFFFFFF
        if remaining >= 8:
            edi = (edi + (ord(s[i+7]) << 24)) & 0xFFFFFFFF
        if remaining >= 7:
            edi = (edi + (ord(s[i+6]) << 16)) & 0xFFFFFFFF
        if remaining >= 6:
            edi = (edi + (ord(s[i+5]) << 8)) & 0xFFFFFFFF
        if remaining >= 5:
            edi = (edi + ord(s[i+4])) & 0xFFFFFFFF
        if remaining >= 4:
            ebx = (ebx + (ord(s[i+3]) << 24)) & 0xFFFFFFFF
        if remaining >= 3:
            ebx = (ebx + (ord(s[i+2]) << 16)) & 0xFFFFFFFF
        if remaining >= 2:
            ebx = (ebx + (ord(s[i+1]) << 8)) & 0xFFFFFFFF
        if remaining >= 1:
            ebx = (ebx + ord(s[i])) & 0xFFFFFFFF

        # Final mixing
        esi = ((esi ^ edi) - ((edi >> 18) | (edi << 14))) & 0xFFFFFFFF
        eax = ((esi ^ ebx) - ((esi >> 21) | (esi << 11))) & 0xFFFFFFFF
        edi = ((edi ^ eax) - ((eax >> 7) | (eax << 25))) & 0xFFFFFFFF
        esi = ((esi ^ edi) - ((edi >> 16) | (edi << 16))) & 0xFFFFFFFF
        edx = ((esi ^ eax) - ((esi >> 28) | (esi << 4))) & 0xFFFFFFFF
        edi = ((edi ^ edx) - ((edx >> 18) | (edx << 14))) & 0xFFFFFFFF
        eax = ((esi ^ edi) - ((edi >> 8) | (edi << 24))) & 0xFFFFFFFF

        return ((esi << 32) | eax) & 0xFFFFFFFFFFFFFFFF

    def read_gump(self, gump_id):
        """Read gump data by ID"""
        if gump_id not in self.entries:
            return None, None

        offset, comp_len, decomp_len, is_compressed = self.entries[gump_id]

        with open(self.uop_path, 'rb') as f:
            f.seek(offset)
            data = f.read(comp_len)

            if is_compressed:
                data = zlib.decompress(data)

            return data, decomp_len

        return None, None


def read_gump_entry_mul(idx_path, gump_id):
    """Read gump index entry from MUL format"""
    with open(idx_path, 'rb') as f:
        f.seek(gump_id * 12)
        data = f.read(12)
        if len(data) < 12:
            return None, None, None
        offset, length, extra = struct.unpack('<III', data)
        if offset == 0xFFFFFFFF or length == 0:
            return None, None, None
        return offset, length, extra


def copy_gumps_mul(client_path, output_path, mappings):
    """Copy gumps using MUL format"""
    idx_path = Path(client_path) / "gumpidx.mul"
    mul_path = Path(client_path) / "gumpart.mul"
    out_idx_path = Path(output_path) / "gumpidx.mul"
    out_mul_path = Path(output_path) / "gumpart.mul"

    if not idx_path.exists() or not mul_path.exists():
        return False

    if not out_idx_path.exists():
        shutil.copy2(idx_path, out_idx_path)
    if not out_mul_path.exists():
        shutil.copy2(mul_path, out_mul_path)

    idx_file = open(out_idx_path, 'r+b')
    mul_file = open(out_mul_path, 'r+b')

    try:
        for source_id, target_id in mappings.items():
            print(f"\n--- Copying gump {source_id} -> {target_id} ---")

            offset, length, extra = read_gump_entry_mul(idx_path, source_id)
            if offset is None:
                print(f"WARNING: Source gump {source_id} not found")
                continue

            width = (extra >> 16) & 0xFFFF
            height = extra & 0xFFFF
            print(f"Source: offset={offset}, length={length}, size={width}x{height}")

            with open(mul_path, 'rb') as f:
                f.seek(offset)
                gump_data = f.read(length)

            target_offset, target_length, _ = read_gump_entry_mul(out_idx_path, target_id)

            if target_offset is not None and target_length >= length:
                mul_file.seek(target_offset)
                mul_file.write(gump_data)
                if target_length > length:
                    mul_file.write(b'\x00' * (target_length - length))
                idx_file.seek(target_id * 12)
                idx_file.write(struct.pack('<III', target_offset, length, extra))
            else:
                mul_file.seek(0, 2)
                new_offset = mul_file.tell()
                mul_file.write(gump_data)

                idx_file.seek(0, 2)
                idx_size = idx_file.tell()
                required = (target_id + 1) * 12
                if idx_size < required:
                    idx_file.seek(idx_size)
                    while idx_file.tell() < required:
                        idx_file.write(struct.pack('<III', 0xFFFFFFFF, 0, 0))

                idx_file.seek(target_id * 12)
                idx_file.write(struct.pack('<III', new_offset, length, extra))

            print(f"Done: gump {source_id} -> {target_id}")

        return True
    finally:
        idx_file.close()
        mul_file.close()


def copy_gumps_uop(client_path, output_path, mappings):
    """Copy gumps using UOP format - creates MUL files as output"""
    uop_path = Path(client_path) / "gumpartLegacyMUL.uop"

    if not uop_path.exists():
        return False

    print(f"Reading UOP file: {uop_path}")
    reader = UOPReader(str(uop_path))

    out_idx_path = Path(output_path) / "gumpidx.mul"
    out_mul_path = Path(output_path) / "gumpart.mul"

    # If output MUL files don't exist, we need to convert from UOP first
    # For simplicity, we'll just create entries for our target gumps

    # Check if source gumps exist
    for source_id, target_id in mappings.items():
        if source_id not in reader.entries:
            print(f"WARNING: Source gump {source_id} not found in UOP")
            # Try to find it by computing hash directly
            test_hash = reader._compute_hash(source_id)
            print(f"  Expected hash for {source_id}: {test_hash:#018x}")

    # For UOP, we need a different approach
    # ClassicUO can read both MUL and UOP, but prefers MUL if present
    # So we create MUL files with just the gumps we need

    print("\nCreating MUL output files...")

    # Initialize or open output files
    if out_idx_path.exists():
        idx_file = open(out_idx_path, 'r+b')
        mul_file = open(out_mul_path, 'r+b')
    else:
        # Create new empty files
        idx_file = open(out_idx_path, 'wb')
        mul_file = open(out_mul_path, 'wb')
        # Fill idx with empty entries up to max gump ID we need
        max_id = max(max(mappings.keys()), max(mappings.values())) + 1
        for i in range(max_id):
            idx_file.write(struct.pack('<III', 0xFFFFFFFF, 0, 0))
        idx_file.close()
        mul_file.close()
        idx_file = open(out_idx_path, 'r+b')
        mul_file = open(out_mul_path, 'r+b')

    try:
        for source_id, target_id in mappings.items():
            print(f"\n--- Copying gump {source_id} -> {target_id} ---")

            gump_data, data_len = reader.read_gump(source_id)
            if gump_data is None:
                print(f"WARNING: Could not read source gump {source_id}")
                continue

            print(f"Read {len(gump_data)} bytes for gump {source_id}")

            # Append to mul file
            mul_file.seek(0, 2)
            new_offset = mul_file.tell()
            mul_file.write(gump_data)

            # Update idx - need to figure out width/height
            # For now, use placeholder (ClassicUO will read from data)
            extra = 0  # Will be parsed from gump data by client

            # Ensure idx is large enough
            idx_file.seek(0, 2)
            idx_size = idx_file.tell()
            required = (target_id + 1) * 12
            if idx_size < required:
                idx_file.seek(idx_size)
                while idx_file.tell() < required:
                    idx_file.write(struct.pack('<III', 0xFFFFFFFF, 0, 0))

            idx_file.seek(target_id * 12)
            idx_file.write(struct.pack('<III', new_offset, len(gump_data), extra))

            print(f"Done: gump {source_id} -> {target_id} at offset {new_offset}")

        return True
    finally:
        idx_file.close()
        mul_file.close()


def main():
    print("=" * 60)
    print("Dwarf Paperdoll Writer")
    print("=" * 60)

    if not os.path.exists(UO_CLIENT_PATH):
        print(f"ERROR: UO client not found at {UO_CLIENT_PATH}")
        return

    Path(OUTPUT_PATH).mkdir(parents=True, exist_ok=True)

    print("\n--- Creating Backups ---")
    backup_dir = backup_files(UO_CLIENT_PATH, BACKUP_PATH)

    print("\n--- Copying Paperdoll Gumps ---")

    # Check which format to use
    mul_exists = (Path(UO_CLIENT_PATH) / "gumpidx.mul").exists()
    uop_exists = (Path(UO_CLIENT_PATH) / "gumpartLegacyMUL.uop").exists()

    success = False
    if mul_exists:
        print("Using MUL format")
        success = copy_gumps_mul(UO_CLIENT_PATH, OUTPUT_PATH, GUMP_MAPPINGS)
    elif uop_exists:
        print("Using UOP format")
        success = copy_gumps_uop(UO_CLIENT_PATH, OUTPUT_PATH, GUMP_MAPPINGS)
    else:
        print("ERROR: No gump files found!")

    if success:
        print("\n" + "=" * 60)
        print("DONE!")
        print(f"Patched files: {OUTPUT_PATH}")
        print("\nCopy to client:")
        print(f'  copy "{OUTPUT_PATH}\\gumpart.mul" "{UO_CLIENT_PATH}"')
        print(f'  copy "{OUTPUT_PATH}\\gumpidx.mul" "{UO_CLIENT_PATH}"')
        print("=" * 60)


if __name__ == "__main__":
    main()
