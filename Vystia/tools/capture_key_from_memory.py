"""
Capture the AES key from ClassicUO memory while it's running.

Instructions:
1. Start the Sagas ClassicUO client
2. Wait for it to load (login screen)
3. Run this script
4. It will scan the process memory for the decryption key

Requires: pip install pymem
"""
import sys

try:
    import pymem
    import pymem.process
except ImportError:
    print("Please install pymem: pip install pymem")
    sys.exit(1)

from Crypto.Cipher import AES
from Crypto.Util import Counter

# Known plaintext and ciphertext from tiledata
ORIGINAL_PLAINTEXT = bytes.fromhex('00000000000000000000554e55534544')
SAG_IV = bytes.fromhex('ce0842d2405328b7f5a190af9e2fe0d2')
SAG_ENCRYPTED = bytes.fromhex('f2e063244b71732e5e4183e83965aed9')

# Required keystream
KEYSTREAM_NEEDED = bytes(a ^ b for a, b in zip(ORIGINAL_PLAINTEXT, SAG_ENCRYPTED))
IV_VALUE = int.from_bytes(SAG_IV, 'big')

def test_key(key_bytes):
    """Test if a 32-byte sequence is the correct key."""
    try:
        counter = Counter.new(128, initial_value=IV_VALUE)
        cipher = AES.new(key_bytes, AES.MODE_CTR, counter=counter)
        test_stream = cipher.encrypt(b'\x00' * 16)
        return test_stream == KEYSTREAM_NEEDED
    except:
        return False

def scan_process_memory(process_name="ClassicUO.exe"):
    """Scan process memory for the AES key."""
    print(f"Looking for process: {process_name}")

    try:
        pm = pymem.Pymem(process_name)
    except pymem.exception.ProcessNotFound:
        print(f"Process {process_name} not found!")
        print("Make sure ClassicUO is running.")
        return None

    print(f"Found process PID: {pm.process_id}")
    print(f"Scanning memory for AES key...")
    print()

    # Get all memory regions
    regions = list(pm.list_modules())
    print(f"Found {len(regions)} modules")

    # Scan each region
    for module in regions:
        try:
            base = module.lpBaseOfDll
            size = module.SizeOfImage
            print(f"Scanning {module.name}: {hex(base)} - {hex(base + size)}")

            # Read memory in chunks
            chunk_size = 1024 * 1024  # 1MB
            for offset in range(0, size, chunk_size):
                try:
                    read_size = min(chunk_size, size - offset)
                    data = pm.read_bytes(base + offset, read_size)

                    # Search for key in this chunk
                    for i in range(len(data) - 32):
                        candidate = data[i:i+32]

                        # Quick filter
                        if candidate.count(0) > 8:
                            continue

                        if test_key(candidate):
                            addr = base + offset + i
                            print()
                            print("=" * 60)
                            print(f"*** FOUND KEY at address {hex(addr)} ***")
                            print(f"Key (hex): {candidate.hex()}")
                            print("=" * 60)
                            return candidate

                except Exception as e:
                    pass

        except Exception as e:
            print(f"  Error scanning {module.name}: {e}")

    print()
    print("Key not found in loaded modules.")
    print("Trying heap memory...")

    # Try scanning all readable memory
    try:
        import ctypes
        from ctypes import wintypes

        # Memory protection constants
        PAGE_READWRITE = 0x04
        PAGE_READONLY = 0x02
        PAGE_EXECUTE_READ = 0x20
        PAGE_EXECUTE_READWRITE = 0x40

        READABLE = [PAGE_READWRITE, PAGE_READONLY, PAGE_EXECUTE_READ, PAGE_EXECUTE_READWRITE]

        kernel32 = ctypes.windll.kernel32

        class MEMORY_BASIC_INFORMATION(ctypes.Structure):
            _fields_ = [
                ("BaseAddress", ctypes.c_void_p),
                ("AllocationBase", ctypes.c_void_p),
                ("AllocationProtect", wintypes.DWORD),
                ("RegionSize", ctypes.c_size_t),
                ("State", wintypes.DWORD),
                ("Protect", wintypes.DWORD),
                ("Type", wintypes.DWORD),
            ]

        mbi = MEMORY_BASIC_INFORMATION()
        address = 0
        regions_scanned = 0

        while True:
            result = kernel32.VirtualQueryEx(
                pm.process_handle,
                ctypes.c_void_p(address),
                ctypes.byref(mbi),
                ctypes.sizeof(mbi)
            )

            if result == 0:
                break

            # Check if readable and committed
            if mbi.State == 0x1000 and mbi.Protect in READABLE:
                try:
                    data = pm.read_bytes(mbi.BaseAddress, min(mbi.RegionSize, 10 * 1024 * 1024))
                    regions_scanned += 1

                    if regions_scanned % 100 == 0:
                        print(f"  Scanned {regions_scanned} memory regions...")

                    for i in range(len(data) - 32):
                        candidate = data[i:i+32]
                        if candidate.count(0) > 8:
                            continue
                        if test_key(candidate):
                            print()
                            print("=" * 60)
                            print(f"*** FOUND KEY at address {hex(mbi.BaseAddress + i)} ***")
                            print(f"Key (hex): {candidate.hex()}")
                            print("=" * 60)
                            return candidate

                except:
                    pass

            address = mbi.BaseAddress + mbi.RegionSize
            if address > 0x7FFFFFFFFFFF:
                break

        print(f"Scanned {regions_scanned} memory regions total")

    except Exception as e:
        print(f"Error scanning heap: {e}")

    return None

def main():
    print("AES Key Capture Tool for ClassicUO")
    print("=" * 40)
    print()
    print("Required keystream:", KEYSTREAM_NEEDED.hex())
    print()

    key = scan_process_memory()

    if key:
        print()
        print("SUCCESS! Key found.")
        print()
        print("To use this key:")
        print(f'python decrypt_sag.py "D:\\Ultima Online - Sagas\\UOData\\tiledata.sag" --key {key.hex()} --output tiledata.mul')
    else:
        print()
        print("Key not found. Make sure:")
        print("1. ClassicUO.exe is running")
        print("2. The game has loaded (at least to login screen)")
        print("3. You're running this script as Administrator")

if __name__ == "__main__":
    main()
