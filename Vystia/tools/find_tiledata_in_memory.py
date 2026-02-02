"""
Search for exact tiledata.mul bytes in ClassicUO memory.
"""
import sys
import os

try:
    import pymem
except ImportError:
    print("pip install pymem")
    sys.exit(1)

ORIGINAL_TILEDATA = r"C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\tiledata.mul"

def main():
    print("=" * 60)
    print("Searching for tiledata.mul in ClassicUO memory")
    print("=" * 60)

    # Read original tiledata
    if not os.path.exists(ORIGINAL_TILEDATA):
        print(f"Original tiledata not found: {ORIGINAL_TILEDATA}")
        return

    with open(ORIGINAL_TILEDATA, 'rb') as f:
        original = f.read(1000)

    print(f"Original tiledata first 32 bytes: {original[:32].hex()}")

    # Use a unique pattern from the middle (not just zeros)
    # Find a non-zero section
    search_pattern = None
    for i in range(0, 500, 16):
        chunk = original[i:i+16]
        if chunk.count(0) < 8 and len(set(chunk)) > 4:
            search_pattern = chunk
            print(f"Using pattern from offset {i}: {chunk.hex()}")
            break

    if not search_pattern:
        # Use bytes 100-116 as fallback
        search_pattern = original[100:116]
        print(f"Fallback pattern: {search_pattern.hex()}")

    # Attach to ClassicUO
    try:
        pm = pymem.Pymem("ClassicUO.exe")
    except:
        print("ClassicUO.exe not found!")
        return

    print(f"\nAttached to PID: {pm.process_id}")
    print(f"Searching for pattern: {search_pattern.hex()}")

    # Search heap
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

    mbi = MEMORY_BASIC_INFORMATION()
    address = 0
    found = []

    print("\nSearching memory regions...")

    while address < 0x7FFFFFFFFFFF:
        result = kernel32.VirtualQueryEx(
            pm.process_handle,
            ctypes.c_ulonglong(address),
            ctypes.byref(mbi),
            ctypes.sizeof(mbi)
        )

        if result == 0:
            break

        if mbi.State == 0x1000 and mbi.Protect in [0x02, 0x04, 0x20, 0x40]:
            if 10000 < mbi.RegionSize < 500000000:
                try:
                    data = pm.read_bytes(mbi.BaseAddress, min(mbi.RegionSize, 50000000))
                    idx = data.find(search_pattern)
                    if idx != -1:
                        found_addr = mbi.BaseAddress + idx
                        found.append((found_addr, idx, mbi.RegionSize))
                        print(f"\n*** FOUND at {hex(found_addr)} ***")

                        # Verify by checking more bytes
                        try:
                            # Read what should be tiledata start
                            potential_start = found_addr - idx  # Should be start of tiledata
                            first_bytes = pm.read_bytes(mbi.BaseAddress, 64)
                            print(f"Region start bytes: {first_bytes[:32].hex()}")
                            print(f"Original start:     {original[:32].hex()}")

                            if first_bytes[:32] == original[:32]:
                                print("\n*** EXACT MATCH! This is decrypted tiledata! ***")
                                print(f"Decrypted tiledata at: {hex(mbi.BaseAddress)}")

                                # Dump first 1KB
                                dump = pm.read_bytes(mbi.BaseAddress, 1024)
                                with open("decrypted_tiledata_sample.bin", "wb") as f:
                                    f.write(dump)
                                print("Saved first 1KB to decrypted_tiledata_sample.bin")

                        except Exception as e:
                            print(f"Verify error: {e}")

                except Exception as e:
                    pass

        address = mbi.BaseAddress + mbi.RegionSize
        if address == 0:
            break

    print(f"\nFound {len(found)} potential matches")

    if not found:
        print("\nTiledata pattern not found.")
        print("The game might use a different tiledata version.")
        print("Or files are decrypted on-demand, not preloaded.")

if __name__ == "__main__":
    main()
