"""
Test derived keys against .sag files to find the correct decryption key.
"""
import os
import hashlib
import struct
from Crypto.Cipher import AES
from Crypto.Util import Counter

def decrypt_with_key(data, key, iv=None):
    """Decrypt data with AES-256-CTR."""
    if iv is None:
        iv = b'\x00' * 16
    counter = Counter.new(128, initial_value=int.from_bytes(iv, 'big'))
    cipher = AES.new(key, AES.MODE_CTR, counter=counter)
    return cipher.decrypt(data)

def check_tiledata(decrypted):
    """Check if decrypted data looks like tiledata.mul."""
    # Tiledata.mul starts with land tile data
    # Each land tile entry is 26 bytes (old format) or 30 bytes (new format)
    # First 4 bytes of each entry are flags

    if len(decrypted) < 1000:
        return False, "Too short"

    # Check for reasonable flag values (should be small numbers)
    flags = struct.unpack('<I', decrypted[0:4])[0]
    if flags > 0xFFFF:  # Flags shouldn't be huge
        return False, f"Bad flags: {hex(flags)}"

    # Check multiple entries for consistency
    valid_entries = 0
    for i in range(0, min(len(decrypted), 1000), 26):
        if i + 4 > len(decrypted):
            break
        flags = struct.unpack('<I', decrypted[i:i+4])[0]
        if flags < 0x10000:  # Reasonable flag value
            valid_entries += 1

    if valid_entries > 10:
        return True, f"Valid entries: {valid_entries}"
    return False, f"Only {valid_entries} valid entries"

def check_map(decrypted):
    """Check if decrypted data looks like map*.mul."""
    # Map data consists of 196-byte blocks (64 cells * 3 bytes + 4 byte header)
    # Each cell is: tile_id (2 bytes) + z (1 byte signed)

    if len(decrypted) < 196:
        return False, "Too short"

    # Check first block header (should be reasonable values)
    # Header is typically 0x0000 or small value
    header = struct.unpack('<I', decrypted[0:4])[0]

    # Check some tile IDs (should be < 0x4000 typically)
    valid_tiles = 0
    for i in range(4, min(196, len(decrypted)), 3):
        if i + 2 > len(decrypted):
            break
        tile_id = struct.unpack('<H', decrypted[i:i+2])[0]
        if tile_id < 0x4000:  # Valid tile range
            valid_tiles += 1

    if valid_tiles > 30:
        return True, f"Valid tiles: {valid_tiles}"
    return False, f"Only {valid_tiles} valid tiles"

def check_art(decrypted):
    """Check if decrypted data could be art.mul."""
    # Art entries typically start with dimensions or are raw pixel data
    if len(decrypted) < 100:
        return False, "Too short"

    # Check for reasonable small values at start
    val1 = struct.unpack('<H', decrypted[0:2])[0]
    val2 = struct.unpack('<H', decrypted[2:4])[0]

    if val1 < 1024 and val2 < 1024:
        return True, f"Reasonable dimensions: {val1}x{val2}"
    return False, f"Bad values: {val1}, {val2}"

def generate_keys():
    """Generate candidate keys to test."""
    keys = []

    # Known strings for key derivation
    seeds = [
        b"UOFileSag",
        b"DecryptAES256_CTR",
        b"OpenAesAlgorithm",
        b"ClassicUO",
        b"Sagas",
        b"UO Sagas",
        b"AES256_CTR",
        b"sagas",
        b"SAGAS",
        b"uofilesag",
        b"UOFILESAG",
    ]

    for seed in seeds:
        # SHA-256
        keys.append((f"SHA256({seed.decode()})", hashlib.sha256(seed).digest()))

        # MD5 doubled
        md5 = hashlib.md5(seed).digest()
        keys.append((f"MD5x2({seed.decode()})", md5 + md5))

        # SHA-256 of lowercase
        keys.append((f"SHA256({seed.decode().lower()})", hashlib.sha256(seed.lower()).digest()))

    return keys

def main():
    sag_files = {
        'tiledata': r"D:\Ultima Online - Sagas\UOData\tiledata.sag",
        'map0': r"D:\Ultima Online - Sagas\UOData\map0.sag",
        'hues': r"D:\Ultima Online - Sagas\UOData\hues.sag",
    }

    # Read first 4KB of each file
    file_data = {}
    for name, path in sag_files.items():
        if os.path.exists(path):
            with open(path, 'rb') as f:
                file_data[name] = f.read(4096)
            print(f"Loaded {name}: {len(file_data[name])} bytes")

    keys = generate_keys()
    print(f"\nTesting {len(keys)} derived keys...")
    print("=" * 70)

    # Test each key
    for key_name, key in keys:
        # Try with zero IV
        for file_name, encrypted in file_data.items():
            decrypted = decrypt_with_key(encrypted, key)

            # Check based on file type
            if file_name == 'tiledata':
                valid, msg = check_tiledata(decrypted)
            elif file_name.startswith('map'):
                valid, msg = check_map(decrypted)
            else:
                valid, msg = check_art(decrypted)

            if valid:
                print(f"\n*** POTENTIAL MATCH ***")
                print(f"Key: {key_name}")
                print(f"Hex: {key.hex()}")
                print(f"File: {file_name}")
                print(f"Result: {msg}")
                print(f"First 32 bytes decrypted: {decrypted[:32].hex()}")

    print("\n" + "=" * 70)
    print("Testing complete.")
    print("\nIf no matches found, the key may be:")
    print("1. Derived differently (PBKDF2, custom algorithm)")
    print("2. Loaded from a separate file")
    print("3. Obfuscated in the binary")
    print("4. Generated from hardware/system info")

if __name__ == "__main__":
    main()
