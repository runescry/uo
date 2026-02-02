"""
Dump decrypted UO data from ClassicUO memory using pymem.
"""
import sys
try:
    import pymem
    import pymem.process
except ImportError:
    print("Install pymem: pip install pymem")
    sys.exit(1)

def main():
    print("=" * 60)
    print("Searching for decrypted UO data in ClassicUO memory")
    print("=" * 60)

    try:
        pm = pymem.Pymem("ClassicUO.exe")
    except:
        print("ClassicUO.exe not found!")
        return

    print(f"Attached to PID: {pm.process_id}")

    # Search for "UNUSED" which appears in tiledata.mul
    search_pattern = b"UNUSED"

    print(f"\nSearching for '{search_pattern.decode()}' pattern...")

    found_locations = []

    # Use pymem's pattern search
    try:
        # Get process modules
        modules = list(pm.list_modules())
        print(f"Found {len(modules)} modules")

        # Search in each module
        for module in modules:
            try:
                base = module.lpBaseOfDll
                size = module.SizeOfImage

                if size > 100000000:  # Skip very large regions
                    continue

                data = pm.read_bytes(base, min(size, 50000000))

                # Search for pattern
                idx = 0
                while True:
                    idx = data.find(search_pattern, idx)
                    if idx == -1:
                        break
                    found_locations.append((module.name, base + idx))
                    idx += 1
                    if len(found_locations) > 10:
                        break

            except Exception as e:
                pass

        if found_locations:
            print(f"\nFound {len(found_locations)} matches!")
            for name, addr in found_locations[:5]:
                print(f"  {name}: {hex(addr)}")

                # Dump context
                try:
                    context = pm.read_bytes(addr - 50, 150)
                    print(f"    Context: ...{context[40:60].hex()}[UNUSED]{context[56:80].hex()}...")
                except:
                    pass
        else:
            print("Pattern not found in modules.")

    except Exception as e:
        print(f"Module search error: {e}")

    # Also try searching heap
    print("\nSearching heap memory...")

    try:
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
        heap_found = []
        regions_checked = 0

        while address < 0x7FFFFFFFFFFF:
            result = kernel32.VirtualQueryEx(
                pm.process_handle,
                ctypes.c_ulonglong(address),
                ctypes.byref(mbi),
                ctypes.sizeof(mbi)
            )

            if result == 0:
                break

            # Check committed, readable memory
            if mbi.State == 0x1000 and mbi.Protect in [0x02, 0x04, 0x20, 0x40]:
                if mbi.RegionSize > 100000 and mbi.RegionSize < 100000000:
                    regions_checked += 1
                    try:
                        data = pm.read_bytes(mbi.BaseAddress, min(mbi.RegionSize, 10000000))
                        idx = data.find(search_pattern)
                        if idx != -1:
                            heap_found.append(mbi.BaseAddress + idx)
                            print(f"  Found at heap {hex(mbi.BaseAddress + idx)}")

                            # Dump some context
                            start = max(0, idx - 100)
                            context = data[start:idx+100]
                            print(f"    First 50 bytes before: {context[:50].hex()}")

                            if len(heap_found) >= 5:
                                break
                    except:
                        pass

            address = mbi.BaseAddress + mbi.RegionSize
            if address == 0:
                break

        print(f"\nChecked {regions_checked} heap regions")
        print(f"Found {len(heap_found)} matches in heap")

        if heap_found:
            print("\n*** Decrypted tiledata IS in memory! ***")
            print("The game has successfully decrypted the .sag files.")

    except Exception as e:
        print(f"Heap search error: {e}")
        import traceback
        traceback.print_exc()

if __name__ == "__main__":
    main()
