"""
Search for the AES keystream or key schedule in memory.
We know:
- IV: ce0842d2405328b7f5a190af9e2fe0d2
- Encrypted: f2e063244b71732e5e4183e83965aed9
- Plaintext: 00000000000000000000554e55534544
- Keystream: ciphertext XOR plaintext
"""
import sys
try:
    import pymem
except:
    print("pip install pymem")
    sys.exit(1)

def xor_bytes(a, b):
    return bytes(x ^ y for x, y in zip(a, b))

def main():
    print("=" * 60)
    print("Searching for AES keystream in memory")
    print("=" * 60)

    # Known values
    iv = bytes.fromhex('ce0842d2405328b7f5a190af9e2fe0d2')
    encrypted = bytes.fromhex('f2e063244b71732e5e4183e83965aed9')
    plaintext = bytes.fromhex('00000000000000000000554e55534544')

    # Calculate keystream
    keystream = xor_bytes(encrypted, plaintext)

    print(f"IV:        {iv.hex()}")
    print(f"Encrypted: {encrypted.hex()}")
    print(f"Plaintext: {plaintext.hex()}")
    print(f"Keystream: {keystream.hex()}")

    # Patterns to search for
    patterns = [
        ("Keystream", keystream),
        ("IV", iv),
        ("First 8 bytes of keystream", keystream[:8]),
    ]

    try:
        pm = pymem.Pymem("ClassicUO.exe")
    except:
        print("\nClassicUO.exe not found!")
        return

    print(f"\nAttached to PID: {pm.process_id}")

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

    for pattern_name, pattern in patterns:
        print(f"\nSearching for {pattern_name}: {pattern.hex()[:32]}...")

        mbi = MEMORY_BASIC_INFORMATION()
        address = 0
        found = 0

        while address < 0x7FFFFFFFFFFF and found < 3:
            result = kernel32.VirtualQueryEx(
                pm.process_handle,
                ctypes.c_ulonglong(address),
                ctypes.byref(mbi),
                ctypes.sizeof(mbi)
            )

            if result == 0:
                break

            if mbi.State == 0x1000 and mbi.Protect in [0x02, 0x04, 0x20, 0x40]:
                if 1000 < mbi.RegionSize < 100000000:
                    try:
                        data = pm.read_bytes(mbi.BaseAddress, min(mbi.RegionSize, 20000000))
                        idx = 0
                        while idx < len(data) - len(pattern):
                            idx = data.find(pattern, idx)
                            if idx == -1:
                                break
                            found_addr = mbi.BaseAddress + idx
                            print(f"  FOUND at {hex(found_addr)}")

                            # Dump context
                            start = max(0, idx - 32)
                            context = data[start:idx + len(pattern) + 32]
                            print(f"    32 bytes before: {context[:32].hex()}")
                            print(f"    Pattern:         {context[32:32+len(pattern)].hex()}")
                            print(f"    32 bytes after:  {context[32+len(pattern):].hex()}")

                            found += 1
                            idx += 1
                            if found >= 3:
                                break
                    except:
                        pass

            address = mbi.BaseAddress + mbi.RegionSize
            if address == 0:
                break

        if found == 0:
            print(f"  Not found")

    # Also search for AES S-box (always present in AES implementations)
    print("\n" + "=" * 60)
    print("Searching for AES S-box (implementation marker)...")
    sbox_start = bytes([0x63, 0x7c, 0x77, 0x7b, 0xf2, 0x6b, 0x6f, 0xc5])

    mbi = MEMORY_BASIC_INFORMATION()
    address = 0
    sbox_found = []

    while address < 0x7FFFFFFFFFFF:
        result = kernel32.VirtualQueryEx(
            pm.process_handle,
            ctypes.c_ulonglong(address),
            ctypes.byref(mbi),
            ctypes.sizeof(mbi)
        )

        if result == 0:
            break

        if mbi.State == 0x1000:
            try:
                data = pm.read_bytes(mbi.BaseAddress, min(mbi.RegionSize, 20000000))
                idx = data.find(sbox_start)
                if idx != -1:
                    sbox_found.append(mbi.BaseAddress + idx)
                    if len(sbox_found) >= 5:
                        break
            except:
                pass

        address = mbi.BaseAddress + mbi.RegionSize
        if address == 0:
            break

    if sbox_found:
        print(f"Found AES S-box at {len(sbox_found)} locations:")
        for addr in sbox_found[:5]:
            print(f"  {hex(addr)}")
        print("This confirms AES implementation is loaded in memory.")
    else:
        print("AES S-box not found - unusual for an AES implementation")

if __name__ == "__main__":
    main()
