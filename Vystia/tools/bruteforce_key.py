"""
Try to find the AES key by testing various derivation methods.
We know:
- AES-256-CTR mode
- First 16 bytes of .sag file might be IV
- We have original tiledata.mul for comparison
"""
import hashlib
import os
from Crypto.Cipher import AES
from Crypto.Util import Counter

# Paths
SAG_FILE = r"D:\Ultima Online - Sagas\UOData\tiledata.sag"
ORIGINAL_FILE = r"C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\tiledata.mul"

def try_decrypt(key, iv, encrypted_data, expected_start):
    """Try to decrypt with given key and IV."""
    try:
        iv_int = int.from_bytes(iv, 'big')
        ctr = Counter.new(128, initial_value=iv_int)
        cipher = AES.new(key, AES.MODE_CTR, counter=ctr)
        decrypted = cipher.decrypt(encrypted_data[:64])

        # Check if first bytes match expected
        if decrypted[:16] == expected_start[:16]:
            return True, decrypted
        return False, None
    except:
        return False, None

def derive_keys():
    """Generate candidate keys from various methods."""
    keys = []

    # Strings found in the binary
    seeds = [
        b"UOFileSag",
        b"DecryptAES256_CTR",
        b"AES256_CTR",
        b"ClassicUO",
        b"Sagas",
        b"UO Sagas",
        b"sagas",
        b"SAGAS",
        b"MatchKey",
        b"SelectKey",
        b"OpenAesAlgorithm",
        b"uosagas",
        b"UOSAGAS",
        b"login.uosagas.com",
        b"uosagas.com",
        b"tiledata",
        b"tiledata.sag",
        b".sag",
    ]

    for seed in seeds:
        # SHA-256
        keys.append((f"SHA256({seed.decode(errors='ignore')})", hashlib.sha256(seed).digest()))

        # SHA-256 of lowercase
        keys.append((f"SHA256(lower:{seed.decode(errors='ignore')})", hashlib.sha256(seed.lower()).digest()))

        # SHA-256 of uppercase
        keys.append((f"SHA256(upper:{seed.decode(errors='ignore')})", hashlib.sha256(seed.upper()).digest()))

        # MD5 doubled
        md5 = hashlib.md5(seed).digest()
        keys.append((f"MD5x2({seed.decode(errors='ignore')})", md5 + md5))

        # SHA-512 first 32 bytes
        keys.append((f"SHA512[:32]({seed.decode(errors='ignore')})", hashlib.sha512(seed).digest()[:32]))

        # Padded to 32 bytes
        if len(seed) <= 32:
            padded = seed + b'\x00' * (32 - len(seed))
            keys.append((f"Padded({seed.decode(errors='ignore')})", padded))

    # Combinations
    combos = [
        b"UOFileSag" + b"AES256_CTR",
        b"ClassicUO" + b"Sagas",
        b"Sagas" + b"UOFileSag",
    ]
    for combo in combos:
        keys.append((f"SHA256({combo.decode(errors='ignore')})", hashlib.sha256(combo).digest()))

    return keys

def main():
    print("=" * 60)
    print("AES Key Brute Force for .sag files")
    print("=" * 60)

    # Read files
    if not os.path.exists(SAG_FILE):
        print(f"ERROR: {SAG_FILE} not found")
        return

    with open(SAG_FILE, 'rb') as f:
        sag_data = f.read(4096)

    print(f"SAG file: {len(sag_data)} bytes loaded")
    print(f"First 32 bytes: {sag_data[:32].hex()}")

    # The IV might be first 16 bytes
    potential_iv = sag_data[:16]
    print(f"Potential IV: {potential_iv.hex()}")

    # Encrypted data starts after IV (or maybe at offset 0)
    encrypted_options = [
        ("IV=first16, data=after", potential_iv, sag_data[16:]),
        ("IV=zeros, data=start", b'\x00'*16, sag_data),
        ("IV=first16, data=start", potential_iv, sag_data),
    ]

    # Get expected plaintext
    if os.path.exists(ORIGINAL_FILE):
        with open(ORIGINAL_FILE, 'rb') as f:
            original = f.read(4096)
        print(f"\nOriginal tiledata.mul first 32 bytes: {original[:32].hex()}")
    else:
        print(f"\nOriginal file not found at {ORIGINAL_FILE}")
        # Use known tiledata structure - starts with land tile flags
        original = b'\x00' * 64  # First tiles often have zero flags

    # Generate keys
    keys = derive_keys()
    print(f"\nTesting {len(keys)} derived keys...")
    print("-" * 60)

    # Try each combination
    for key_name, key in keys:
        for enc_name, iv, encrypted in encrypted_options:
            success, decrypted = try_decrypt(key, iv, encrypted, original)
            if success:
                print(f"\n*** FOUND KEY! ***")
                print(f"Method: {key_name}")
                print(f"Format: {enc_name}")
                print(f"Key: {key.hex()}")
                print(f"IV: {iv.hex()}")
                print(f"Decrypted start: {decrypted[:32].hex()}")
                return

    print("\nNo key found with simple derivation methods.")
    print("\nThe key might be:")
    print("1. Derived using PBKDF2 or similar")
    print("2. XOR'd or otherwise obfuscated")
    print("3. Loaded from network/server")
    print("4. Generated from hardware info")

if __name__ == "__main__":
    main()
