"""
Dump all large memory regions from ClassicUO to files for offline analysis.
Looking for any region that might contain decrypted game data.
"""
import sys
import os

try:
    import pymem
except ImportError:
    print("pip install pymem")
    sys.exit(1)

OUTPUT_DIR = r"D:\UO\Vystia\tools\memory_dumps"

def main():
    print("=" * 60)
    print("Dumping all large memory regions from ClassicUO")
    print("=" * 60)

    os.makedirs(OUTPUT_DIR, exist_ok=True)

    try:
        pm = pymem.Pymem("ClassicUO.exe")
    except:
        print("ClassicUO.exe not found!")
        return

    print(f"Attached to PID: {pm.process_id}")

    import ctypes
    from ctypes import wintypes

    kernel32 = ctypes.windll.kernel32

    class MEMORY_BASIC_INFORMATION(ctypes.Structure):
        _fields_ = [
            ("BaseAddress", ctypes.c_ulonglong),
            ("AllocationBase", ctypes.c_ulonglong),
            ("AllocationProtect", wintypes.DWORD),
            ("RegionSize", ctypes.c_ulonglong),
            ("State", wintypes.DWORD),
            ("Protect", wintypes.DWORD),
            ("Type", wintypes.DWORD),
        ]

    # tiledata.mul is about 3MB, so look for regions >= 1MB
    MIN_SIZE = 1 * 1024 * 1024  # 1MB
    MAX_SIZE = 50 * 1024 * 1024  # 50MB

    mbi = MEMORY_BASIC_INFORMATION()
    address = 0
    regions = []

    print("\nScanning memory regions...")

    while address < 0x7FFFFFFFFFFF:
        result = kernel32.VirtualQueryEx(
            pm.process_handle,
            ctypes.c_ulonglong(address),
            ctypes.byref(mbi),
            ctypes.sizeof(mbi)
        )

        if result == 0:
            break

        # Committed, readable memory
        if mbi.State == 0x1000 and mbi.Protect in [0x02, 0x04, 0x20, 0x40]:
            if MIN_SIZE <= mbi.RegionSize <= MAX_SIZE:
                regions.append((mbi.BaseAddress, mbi.RegionSize, mbi.Type))

        address = mbi.BaseAddress + mbi.RegionSize
        if address == 0:
            break

    print(f"Found {len(regions)} regions between 1MB-50MB")

    # Dump each region
    for i, (base, size, mtype) in enumerate(regions):
        type_name = "PRIVATE" if mtype == 0x20000 else "MAPPED" if mtype == 0x40000 else "IMAGE"
        print(f"\nRegion {i+1}: {hex(base)} - {size // 1024}KB ({type_name})")

        try:
            data = pm.read_bytes(base, min(size, MAX_SIZE))

            # Quick analysis
            zeros = data[:1024].count(0)

            # Check for tiledata signatures
            has_unused = b'UNUSED' in data[:10000]
            has_flags = data[:4] == b'\x00\x00\x00\x00'  # tiledata starts with 0 flags

            # Check for any text patterns
            has_uo_strings = any(s in data[:10000] for s in [b'tiledata', b'TileData', b'.mul', b'.sag'])

            print(f"  First 32 bytes: {data[:32].hex()}")
            print(f"  Zeros in first 1KB: {zeros}")
            print(f"  Has 'UNUSED': {has_unused}")
            print(f"  Has UO strings: {has_uo_strings}")

            # If looks interesting, dump it
            if has_unused or has_uo_strings or zeros > 500:
                filename = os.path.join(OUTPUT_DIR, f"region_{i:03d}_{hex(base)}_{type_name}.bin")
                with open(filename, 'wb') as f:
                    f.write(data)
                print(f"  *** DUMPED to {filename}")

                # Show more details
                if has_unused:
                    idx = data.find(b'UNUSED')
                    print(f"  'UNUSED' at offset: {idx}")
                    print(f"  Context: {data[max(0,idx-20):idx+30]}")

        except Exception as e:
            print(f"  Error reading: {e}")

    # Also look for any region containing tiledata-like structure
    print("\n" + "=" * 60)
    print("Searching for tiledata structure patterns...")

    # In tiledata.mul, each land tile entry is 26 bytes (old format) or 30 bytes (new)
    # First group has 32 tiles, then groups of 32 tiles
    # Each group might start with a header

    mbi = MEMORY_BASIC_INFORMATION()
    address = 0

    while address < 0x7FFFFFFFFFFF:
        result = kernel32.VirtualQueryEx(
            pm.process_handle,
            ctypes.c_ulonglong(address),
            ctypes.byref(mbi),
            ctypes.sizeof(mbi)
        )

        if result == 0:
            break

        if mbi.State == 0x1000 and mbi.RegionSize > 100000:
            try:
                # Just read first 1KB to check structure
                data = pm.read_bytes(mbi.BaseAddress, 1024)

                # Look for repeating patterns typical of tiledata
                # Land tiles have flags (4 bytes), textureID (2 bytes), name (20 bytes)
                # So there's a pattern every 26 or 30 bytes

                # Simple heuristic: lots of small values (flags, IDs) with occasional ASCII
                small_vals = sum(1 for b in data if b < 32)
                ascii_vals = sum(1 for b in data if 32 <= b < 128)

                if ascii_vals > 100 and small_vals > 500:
                    print(f"\nPotential tiledata at {hex(mbi.BaseAddress)}")
                    print(f"  Size: {mbi.RegionSize // 1024}KB")
                    print(f"  ASCII chars: {ascii_vals}, Small vals: {small_vals}")
                    print(f"  First 64: {data[:64].hex()}")

                    # Check for tile name pattern
                    for offset in [4, 6, 26, 30]:  # Common tiledata offsets
                        name_area = data[offset:offset+20]
                        printable = sum(1 for c in name_area if 32 <= c < 128)
                        if printable > 10:
                            try:
                                name = name_area.decode('ascii', errors='ignore').strip('\x00')
                                if name:
                                    print(f"  Possible name at +{offset}: '{name}'")
                            except:
                                pass

            except:
                pass

        address = mbi.BaseAddress + mbi.RegionSize
        if address == 0:
            break

    print("\nDone. Check", OUTPUT_DIR, "for dumps.")

if __name__ == "__main__":
    main()
