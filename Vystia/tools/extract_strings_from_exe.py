"""
Extract readable strings from Sagas ClassicUO.exe and search for encryption keys
"""
import os
import re

def extract_strings_from_binary(data, min_length=4):
    """Extract all printable ASCII strings from binary data."""
    strings = []
    current = b''
    current_offset = 0

    for i, byte in enumerate(data):
        if 32 <= byte < 127:
            if not current:
                current_offset = i
            current += bytes([byte])
        else:
            if len(current) >= min_length:
                strings.append((current.decode('ascii'), current_offset, len(current)))
            current = b''
    return strings

def find_context(data, search_term, context_bytes=100):
    """Find context around a search term in binary."""
    results = []
    search_bytes = search_term.encode('ascii') if isinstance(search_term, str) else search_term

    for match in re.finditer(re.escape(search_bytes), data):
        start = max(0, match.start() - context_bytes)
        end = min(len(data), match.end() + context_bytes)
        context = data[start:end]
        readable = ''.join(chr(c) if 32 <= c < 127 else '.' for c in context)
        results.append((match.start(), readable))
    return results

def find_potential_keys(data):
    """Search for byte patterns that could be encryption keys."""
    print("\n" + "=" * 70)
    print("SEARCHING FOR POTENTIAL ENCRYPTION KEYS")
    print("=" * 70)

    # Look for 32-byte sequences that could be AES-256 keys
    # Keys often appear near known strings
    key_markers = [b'DecryptAES256', b'AES256_CTR', b'CopyDecryptedData', b'UOFileSag']

    for marker in key_markers:
        for match in re.finditer(re.escape(marker), data):
            offset = match.start()
            print(f"\nFound '{marker.decode()}' at offset {hex(offset)}")

            # Look in surrounding 500 bytes for potential key material
            search_start = max(0, offset - 500)
            search_end = min(len(data), offset + 500)
            region = data[search_start:search_end]

            # Show hex dump of interesting regions
            print(f"  Region [{hex(search_start)} - {hex(search_end)}]:")

            # Look for sequences of non-zero bytes that could be keys
            # Keys are typically 16, 24, or 32 bytes
            for key_len in [16, 32]:
                for i in range(len(region) - key_len):
                    candidate = region[i:i+key_len]
                    # Skip if mostly zeros or mostly same byte
                    if candidate.count(0) < key_len // 4:
                        unique_bytes = len(set(candidate))
                        if unique_bytes > key_len // 2:  # Has variety
                            # Check if it looks like a key (not just ASCII text)
                            non_printable = sum(1 for b in candidate if b < 32 or b > 126)
                            if non_printable > key_len // 4:
                                abs_offset = search_start + i
                                print(f"  Potential {key_len}-byte key at {hex(abs_offset)}:")
                                print(f"    Hex: {candidate.hex()}")
                                break  # Just show first candidate per marker

def main():
    exe_path = r"D:\Ultima Online - Sagas\ClassicUO\ClassicUO.exe"

    print(f"Analyzing: {exe_path}")
    print(f"File size: {os.path.getsize(exe_path):,} bytes")
    print("=" * 70)

    with open(exe_path, 'rb') as f:
        data = f.read()

    # Find specific encryption-related strings and their context
    print("\n" + "=" * 70)
    print("ENCRYPTION-RELATED STRINGS WITH CONTEXT")
    print("=" * 70)

    crypto_terms = ['DecryptAES256_CTR', 'UOFileSag', 'CopyDecryptedData', '.sag']

    for term in crypto_terms:
        results = find_context(data, term, context_bytes=80)
        if results:
            print(f"\n'{term}' found {len(results)} times:")
            for offset, context in results[:3]:  # Show first 3
                print(f"  Offset {hex(offset)}:")
                # Break context into lines
                for i in range(0, len(context), 60):
                    print(f"    {context[i:i+60]}")

    # Search for potential keys
    find_potential_keys(data)

    # Look for hardcoded byte arrays (common pattern for keys)
    print("\n" + "=" * 70)
    print("SEARCHING FOR HARDCODED BYTE ARRAY PATTERNS")
    print("=" * 70)

    # Look for 0x pattern (like 0xa4, 0xba, 0x3c...) - C# byte array initialization
    hex_array_pattern = rb'0x[0-9a-fA-F]{2}(?:\s*,\s*0x[0-9a-fA-F]{2}){15,}'
    matches = re.findall(hex_array_pattern, data)
    if matches:
        print(f"\nFound {len(matches)} hex array patterns")
        for m in matches[:5]:
            print(f"  {m.decode('ascii', errors='ignore')[:100]}...")
    else:
        print("\nNo hex array patterns found")

    # Summary
    print("\n" + "=" * 70)
    print("SUMMARY")
    print("=" * 70)
    print("""
Next steps for dnSpy:
1. In dnSpy, switch to the 'PE' tab and look at the .rdata section
2. Search for byte sequences near DecryptAES256_CTR
3. Try runtime debugging - set breakpoint on file read and inspect memory
4. The key might be derived at runtime rather than hardcoded
    """)

if __name__ == "__main__":
    main()
