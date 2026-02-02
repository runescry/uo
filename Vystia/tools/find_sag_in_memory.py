"""
Search for .sag file data in memory and look for decrypted version nearby.
"""
import sys
import os

try:
    import pymem
except:
    print("pip install pymem")
    sys.exit(1)

SAG_FILE = r"D:\Ultima Online - Sagas\UOData\tiledata.sag"

def main():
    print("=" * 60)
    print("Searching for .sag file contents in memory")
    print("=" * 60)

    # Read SAG file
    with open(SAG_FILE, 'rb') as f:
        sag_data = f.read(4096)

    print(f"SAG file loaded: {len(sag_data)} bytes")

    # Different patterns to search for
    patterns = [
        ("First 32 bytes (IV + encrypted)", sag_data[:32]),
        ("Bytes 16-48 (encrypted start)", sag_data[16:48]),
        ("Bytes 100-132", sag_data[100:132]),
    ]

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

    for name, pattern in patterns:
        print(f"\nSearching for {name}...")
        print(f"  Pattern: {pattern[:16].hex()}...")

        mbi = MEMORY_BASIC_INFORMATION()
        address = 0
        found = 0

        while address < 0x7FFFFFFFFFFF and found < 5:
            result = kernel32.VirtualQueryEx(
                pm.process_handle,
                ctypes.c_ulonglong(address),
                ctypes.byref(mbi),
                ctypes.sizeof(mbi)
            )

            if result == 0:
                break

            if mbi.State == 0x1000 and mbi.Protect in [0x02, 0x04, 0x20, 0x40]:
                if 1000 < mbi.RegionSize < 500000000:
                    try:
                        data = pm.read_bytes(mbi.BaseAddress, min(mbi.RegionSize, 50000000))
                        idx = data.find(pattern)
                        if idx != -1:
                            found_addr = mbi.BaseAddress + idx
                            print(f"\n  *** FOUND at {hex(found_addr)} ***")
                            found += 1

                            # This could be the file mapped in memory
                            # Look for decrypted data nearby (within a few MB)
                            print(f"  Region base: {hex(mbi.BaseAddress)}, size: {mbi.RegionSize}")

                            # Check if there's zero-heavy data nearby (decrypted tiledata starts with zeros)
                            for offset in [0, 0x100000, 0x200000, -0x100000]:
                                check_addr = idx + offset
                                if 0 <= check_addr < len(data) - 100:
                                    sample = data[check_addr:check_addr+32]
                                    zeros = sample.count(0)
                                    if zeros > 20:  # Lots of zeros like tiledata start
                                        print(f"  Potential decrypted data at offset {hex(offset)}: {sample.hex()}")

                    except:
                        pass

            address = mbi.BaseAddress + mbi.RegionSize
            if address == 0:
                break

        if found == 0:
            print("  Not found")

    # Let's also scan for any region that starts with many zeros followed by "UNUSED" pattern
    print("\n" + "=" * 60)
    print("Searching for decrypted tiledata pattern (zeros + UNUSED)...")

    # Pattern: lots of zeros then 0x554e555345 ("UNUSE")
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

        # Look for large regions that might hold file data
        if mbi.State == 0x1000 and mbi.RegionSize > 1000000:
            try:
                # Just read first 64 bytes to check
                first_bytes = pm.read_bytes(mbi.BaseAddress, 64)

                # Check if starts with zeros and has UNUSED around byte 10-12
                if first_bytes[:10].count(0) >= 8:
                    unused_pos = first_bytes.find(b'UNUSED')
                    if unused_pos != -1 and unused_pos < 20:
                        print(f"\n*** POTENTIAL TILEDATA at {hex(mbi.BaseAddress)} ***")
                        print(f"  First 64 bytes: {first_bytes.hex()}")
                        print(f"  Readable: {first_bytes}")

                        # Dump more
                        more = pm.read_bytes(mbi.BaseAddress, 512)
                        with open("potential_tiledata.bin", "wb") as f:
                            f.write(more)
                        print("  Saved 512 bytes to potential_tiledata.bin")

            except:
                pass

        address = mbi.BaseAddress + mbi.RegionSize
        if address == 0:
            break

    print("\nDone.")

if __name__ == "__main__":
    main()
