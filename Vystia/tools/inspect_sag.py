"""
Analyze .sag file format to understand structure
"""
import struct
import os
import zlib

def analyze_sag_file(filepath):
    """Analyze a .sag file to determine its format"""
    print(f"\n{'='*60}")
    print(f"Analyzing: {os.path.basename(filepath)}")
    print(f"{'='*60}")
    
    file_size = os.path.getsize(filepath)
    print(f"File size: {file_size:,} bytes")
    
    with open(filepath, 'rb') as f:
        # Read first 128 bytes to check for magic numbers
        header = f.read(128)
        print(f"\nFirst 128 bytes (hex):")
        print(header.hex())
        
        # Check for common magic numbers
        print(f"\nChecking for known formats:")
        
        # Check for UOP format (MYP\x00)
        if header[:4] == b'MYP\x00':
            print("  ✓ UOP format detected!")
            return analyze_uop_format(f, filepath)
        
        # Check for ZIP format
        if header[:2] == b'PK':
            print("  ✓ ZIP format detected!")
            return analyze_zip_format(f, filepath)
        
        # Check for GZIP format
        if header[:2] == b'\x1f\x8b':
            print("  ✓ GZIP format detected!")
            return analyze_gzip_format(f, filepath)
        
        # Check if it looks encrypted (high entropy)
        entropy = calculate_entropy(header)
        print(f"  Entropy: {entropy:.4f} (high = encrypted/compressed, low = structured)")
        
        # Try to find patterns
        print(f"\nLooking for patterns:")
        
        # Check for repeated patterns (might indicate block structure)
        check_repeated_patterns(header)
        
        # Try reading as if it's a simple container
        f.seek(0)
        try_simple_structures(f, filepath)
        
        # Check if it might be XOR encrypted
        check_xor_encryption(header)
        
        return None

def calculate_entropy(data):
    """Calculate Shannon entropy"""
    if not data:
        return 0
    import math
    entropy = 0
    for x in range(256):
        p_x = float(data.count(bytes([x]))) / len(data)
        if p_x > 0:
            entropy += - p_x * math.log2(p_x)
    return entropy

def check_repeated_patterns(data):
    """Check for repeated byte patterns that might indicate structure"""
    # Look for 4-byte, 8-byte patterns
    for pattern_len in [4, 8, 12, 16]:
        patterns = {}
        for i in range(0, len(data) - pattern_len, pattern_len):
            pattern = data[i:i+pattern_len]
            if pattern in patterns:
                patterns[pattern] += 1
            else:
                patterns[pattern] = 1
        
        # Find most common pattern
        if patterns:
            most_common = max(patterns.items(), key=lambda x: x[1])
            if most_common[1] > 2:
                print(f"  Found repeated {pattern_len}-byte pattern: {most_common[0].hex()} (appears {most_common[1]} times)")

def check_xor_encryption(data):
    """Check if data might be XOR encrypted with a simple key"""
    # Try common XOR keys
    common_keys = [0x00, 0xFF, 0xAA, 0x55, 0x5A, 0xA5]
    
    for key in common_keys:
        decrypted = bytes([b ^ key for b in data[:32]])
        # Check if decrypted looks more structured
        if decrypted[:4] == b'MYP\x00' or decrypted[:2] == b'PK':
            print(f"  Possible XOR encryption with key 0x{key:02X}!")
            print(f"  Decrypted start: {decrypted[:32].hex()}")

def try_simple_structures(f, filepath):
    """Try to interpret as simple container formats"""
    f.seek(0)
    
    # Try reading as simple header + data
    try:
        # Try 4-byte magic + 4-byte version + offsets
        magic = struct.unpack('<I', f.read(4))[0]
        version = struct.unpack('<I', f.read(4))[0]
        print(f"\nAs simple header:")
        print(f"  First 4 bytes as uint32: 0x{magic:08X}")
        print(f"  Next 4 bytes as uint32: 0x{version:08X}")
        
        # Try reading as offset table
        f.seek(0)
        first_offset = struct.unpack('<Q', f.read(8))[0]
        if first_offset < os.path.getsize(filepath) and first_offset > 0:
            print(f"  First 8 bytes as offset: {first_offset:,} (valid if < file size)")
    except:
        pass

def analyze_uop_format(f, filepath):
    """Analyze as UOP format"""
    f.seek(0)
    magic = f.read(4)
    version, sig = struct.unpack('<II', f.read(8))
    next_block = struct.unpack('<Q', f.read(8))[0]
    block_size, file_count = struct.unpack('<II', f.read(8))
    
    print(f"  Version: {version}")
    print(f"  File count: {file_count}")
    print(f"  First block at: {next_block}")
    return "UOP"

def analyze_zip_format(f, filepath):
    """Analyze as ZIP format"""
    return "ZIP"

def analyze_gzip_format(f, filepath):
    """Analyze as GZIP format"""
    return "GZIP"

if __name__ == "__main__":
    base_path = r"D:\Ultima Online - Sagas\UOData"
    
    # Analyze a few different .sag files
    test_files = [
        "map0.sag",
        "art.sag", 
        "tiledata.sag",
        "multi.sag"
    ]
    
    for filename in test_files:
        filepath = os.path.join(base_path, filename)
        if os.path.exists(filepath):
            analyze_sag_file(filepath)
        else:
            print(f"File not found: {filepath}")
