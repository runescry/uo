"""
Test decryption by comparing with known UO data patterns
This script attempts to find the key/IV by testing against known data structures
"""
import os
import struct
from Crypto.Cipher import AES
from Crypto.Util import Counter

def test_with_known_multi_pattern(sag_path, idx_path, entry_id, expected_tile_id=None):
    """
    Test decryption by checking if decrypted data matches known multi patterns
    
    Known patterns:
    - Multi entries are 12 bytes: tile_id(2) + x(2) + y(2) + z(2) + flags(4)
    - Valid tile_ids: 0x0001 to 0x3FFF
    - Valid coordinates: -100 to 100
    - Valid z: -128 to 127
    """
    # Read encrypted data
    with open(idx_path, 'rb') as idx:
        idx.seek(entry_id * 12)
        offset, length, extra = struct.unpack('<III', idx.read(12))
    
    with open(sag_path, 'rb') as f:
        f.seek(offset)
        encrypted_data = f.read(min(length, 1024))
    
    # Known multi patterns - common multi tile IDs
    common_tile_ids = [
        0x0001, 0x0002, 0x0003, 0x0064, 0x0065,  # Common floor/wall tiles
        0x00A8, 0x00A9, 0x00AA, 0x00AB,  # Doors
        0x01DB, 0x01DC, 0x01DD, 0x01DE,  # Stairs
    ]
    
    # Try decrypting with various keys and check if result matches known patterns
    # This is a brute-force approach that would take too long
    # Instead, we'll look for patterns in the encrypted data itself
    
    print(f"Analyzing encrypted data for entry {entry_id}...")
    print(f"Encrypted data (first 32 bytes): {encrypted_data[:32].hex()}")
    
    # Look for repeating patterns that might indicate encryption mode
    # CTR mode should produce different ciphertext for same plaintext with different counters
    # But if IV is predictable, we might see patterns
    
    return None, None

def analyze_encryption_patterns(sag_path):
    """Analyze encryption patterns across the file"""
    file_size = os.path.getsize(sag_path)
    
    # Read multiple chunks from different parts of the file
    chunk_size = 1024
    chunks = []
    
    with open(sag_path, 'rb') as f:
        # Read from beginning, middle, and end
        positions = [0, file_size // 2, max(0, file_size - chunk_size)]
        
        for pos in positions:
            f.seek(pos)
            chunk = f.read(chunk_size)
            if len(chunk) == chunk_size:
                chunks.append((pos, chunk))
    
    print(f"Analyzing {len(chunks)} chunks from {os.path.basename(sag_path)}...")
    
    # Look for patterns
    # In CTR mode, if the same plaintext appears at different positions,
    # the ciphertext should be different (unless counter wraps)
    
    # Check for repeating patterns
    for i, (pos1, chunk1) in enumerate(chunks):
        for j, (pos2, chunk2) in enumerate(chunks[i+1:], i+1):
            # Look for identical blocks (might indicate same plaintext + same counter)
            for block_size in [16, 32, 64]:
                for offset1 in range(0, len(chunk1) - block_size, block_size):
                    block1 = chunk1[offset1:offset1+block_size]
                    for offset2 in range(0, len(chunk2) - block_size, block_size):
                        block2 = chunk2[offset2:offset2+block_size]
                        if block1 == block2:
                            print(f"Found identical {block_size}-byte block:")
                            print(f"  Position 1: {pos1 + offset1}")
                            print(f"  Position 2: {pos2 + offset2}")
                            print(f"  Block: {block1.hex()}")
    
    return None

if __name__ == "__main__":
    sag_path = r"D:\Ultima Online - Sagas\UOData\multi.sag"
    idx_path = r"D:\Ultima Online - Sagas\UOData\multi.idx"
    
    if os.path.exists(sag_path):
        analyze_encryption_patterns(sag_path)
        
        # Test with known multi entry
        if os.path.exists(idx_path):
            test_with_known_multi_pattern(sag_path, idx_path, 0)
