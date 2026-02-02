"""
Search for AES-256 keys in the ClassicUO binary by looking for high-entropy byte sequences.
"""
import os
import math
import hashlib
from collections import Counter

def calculate_entropy(data):
    """Calculate Shannon entropy of byte sequence."""
    if not data:
        return 0
    counter = Counter(data)
    length = len(data)
    entropy = 0
    for count in counter.values():
        if count > 0:
            p = count / length
            entropy -= p * math.log2(p)
    return entropy

def find_high_entropy_sequences(data, seq_len=32, min_entropy=4.0):
    """Find sequences with high entropy (likely keys)."""
    candidates = []
    for i in range(len(data) - seq_len):
        seq = data[i:i+seq_len]
        # Skip if contains too many zeros or repeated bytes
        if seq.count(0) > seq_len // 4:
            continue
        if max(Counter(seq).values()) > seq_len // 2:
            continue
        entropy = calculate_entropy(seq)
        if entropy >= min_entropy:
            candidates.append((i, seq, entropy))
    return candidates

def derive_keys_from_strings(data):
    """Try deriving keys from known strings using common methods."""
    print("\n" + "=" * 70)
    print("TRYING KEY DERIVATION FROM KNOWN STRINGS")
    print("=" * 70)

    # Known strings that might be used for key derivation
    potential_seeds = [
        b"UOFileSag",
        b"DecryptAES256_CTR",
        b"OpenAesAlgorithm",
        b"ClassicUO",
        b"Sagas",
        b"UO Sagas",
        b"AES256_CTR",
    ]

    derived_keys = []
    for seed in potential_seeds:
        # SHA-256 hash
        sha256_key = hashlib.sha256(seed).digest()
        derived_keys.append((f"SHA256({seed.decode()})", sha256_key))

        # MD5 doubled
        md5_hash = hashlib.md5(seed).digest()
        md5_doubled = md5_hash + md5_hash
        derived_keys.append((f"MD5x2({seed.decode()})", md5_doubled))

    print("\nDerived keys to test:")
    for name, key in derived_keys:
        print(f"  {name}: {key.hex()}")

    return derived_keys

def search_near_crypto_strings(data):
    """Search for key-like data near known crypto-related strings."""
    print("\n" + "=" * 70)
    print("SEARCHING NEAR CRYPTO STRINGS FOR KEY PATTERNS")
    print("=" * 70)

    markers = [
        b"DecryptAES256_CTR",
        b"UOFileSag",
        b"AES256_CTR",
        b"OpenAesAlgorithm",
    ]

    for marker in markers:
        idx = data.find(marker)
        if idx == -1:
            continue

        print(f"\n'{marker.decode()}' at offset {hex(idx)}")

        # Search in 2KB window around the string
        start = max(0, idx - 1024)
        end = min(len(data), idx + 1024)
        region = data[start:end]

        # Look for high entropy 32-byte sequences
        for i in range(len(region) - 32):
            seq = region[i:i+32]
            entropy = calculate_entropy(seq)

            # High entropy and not mostly ASCII
            non_ascii = sum(1 for b in seq if b < 32 or b > 126)
            if entropy > 5.0 and non_ascii > 16:
                abs_offset = start + i
                print(f"  High entropy sequence at {hex(abs_offset)}:")
                print(f"    Entropy: {entropy:.2f}")
                print(f"    Hex: {seq.hex()}")
                print(f"    ---")

def find_aligned_32byte_sequences(data):
    """Look for 32-byte sequences at aligned addresses (common for keys)."""
    print("\n" + "=" * 70)
    print("SEARCHING FOR ALIGNED 32-BYTE KEY CANDIDATES")
    print("=" * 70)

    candidates = []

    # Search in .rdata-like sections (high addresses in PE)
    # Typically keys are in read-only data sections
    search_start = 0x100000  # Skip code section usually

    for i in range(search_start, len(data) - 32, 16):  # 16-byte aligned
        seq = data[i:i+32]

        # Must have variety
        unique = len(set(seq))
        if unique < 20:
            continue

        # Not too many zeros
        if seq.count(0) > 4:
            continue

        # Not mostly ASCII (keys are usually binary)
        ascii_count = sum(1 for b in seq if 32 <= b <= 126)
        if ascii_count > 16:
            continue

        entropy = calculate_entropy(seq)
        if entropy > 5.5:
            candidates.append((i, seq, entropy))

    # Show top candidates by entropy
    candidates.sort(key=lambda x: x[2], reverse=True)

    print(f"\nTop 20 candidates (by entropy):")
    for offset, seq, entropy in candidates[:20]:
        print(f"  {hex(offset)}: entropy={entropy:.2f} hex={seq.hex()[:32]}...")

def main():
    exe_path = r"D:\Ultima Online - Sagas\ClassicUO\ClassicUO.exe"

    print(f"Analyzing: {exe_path}")
    print(f"File size: {os.path.getsize(exe_path):,} bytes")

    with open(exe_path, 'rb') as f:
        data = f.read()

    # Method 1: Search near crypto strings
    search_near_crypto_strings(data)

    # Method 2: Try key derivation from known strings
    derived = derive_keys_from_strings(data)

    # Method 3: Find aligned high-entropy sequences
    find_aligned_32byte_sequences(data)

    # Summary
    print("\n" + "=" * 70)
    print("NEXT STEPS")
    print("=" * 70)
    print("""
1. Try the derived keys with decrypt_sag.py
2. Try the high-entropy candidates as keys
3. If none work, the key may be obfuscated or runtime-generated

Example test command:
python decrypt_sag.py "D:\\Ultima Online - Sagas\\UOData\\tiledata.sag" --key <hex_key> --iv 00000000000000000000000000000000 --output test.mul
    """)

if __name__ == "__main__":
    main()
