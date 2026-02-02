"""
Attempt brute force on small keyspace
This tests if the key/IV might be derived from a small set of values
"""
import os
import struct
import itertools
from Crypto.Cipher import AES
from Crypto.Util import Counter

def calculate_entropy(data):
    """Calculate Shannon entropy"""
    if not data:
        return 0
    import math
    entropy = 0
    byte_counts = [0] * 256
    for byte in data:
        byte_counts[byte] += 1
    
    for count in byte_counts:
        if count > 0:
            p = count / len(data)
            entropy += -p * math.log2(p)
    
    return entropy

def try_small_keyspace(sag_path, idx_path=None, entry_id=0):
    """
    Try brute force on a small keyspace
    Tests if key/IV might be simple values or derived from small sets
    """
    # Read encrypted data
    if idx_path and os.path.exists(idx_path):
        with open(idx_path, 'rb') as idx:
            idx.seek(entry_id * 12)
            offset, length, extra = struct.unpack('<III', idx.read(12))
        
        with open(sag_path, 'rb') as f:
            f.seek(offset)
            encrypted_data = f.read(min(length, 256))  # Small chunk for testing
    else:
        with open(sag_path, 'rb') as f:
            encrypted_data = f.read(256)
    
    print(f"Testing small keyspace on {len(encrypted_data)} bytes...")
    
    # Try very simple keys (all same byte value)
    print("\nTesting simple keys (all same byte)...")
    for byte_val in [0x00, 0xFF, 0xAA, 0x55, 0x5A, 0xA5]:
        key = bytes([byte_val] * 32)
        for iv_byte in [0x00, 0xFF, 0xAA, 0x55]:
            iv = bytes([iv_byte] * 16)
            
            try:
                counter = Counter.new(128, initial_value=int.from_bytes(iv, 'big'))
                cipher = AES.new(key, AES.MODE_CTR, counter=counter)
                decrypted = cipher.decrypt(encrypted_data)
                
                entropy = calculate_entropy(decrypted)
                
                # Check if it looks like valid data
                if len(decrypted) >= 12:
                    tile_id = struct.unpack('<H', decrypted[:2])[0]
                    x = struct.unpack('<h', decrypted[2:4])[0]
                    y = struct.unpack('<h', decrypted[4:6])[0]
                    z = struct.unpack('<h', decrypted[6:8])[0]
                    
                    if (0 < tile_id < 0x4000 and 
                        -100 < x < 100 and 
                        -100 < y < 100 and 
                        -128 <= z <= 127 and
                        entropy < 6.0):  # Lower entropy threshold
                        print(f"\n[POTENTIAL MATCH]")
                        print(f"  Key: {key.hex()}")
                        print(f"  IV: {iv.hex()}")
                        print(f"  Entropy: {entropy:.4f}")
                        print(f"  Tile ID: {tile_id}, x={x}, y={y}, z={z}")
                        return key, iv
            except:
                continue
    
    # Try keys based on file offset
    print("\nTesting offset-based keys/IVs...")
    if idx_path:
        with open(idx_path, 'rb') as idx:
            idx.seek(entry_id * 12)
            offset, length, extra = struct.unpack('<III', idx.read(12))
        
        # Key from offset (various formats)
        offset_key_variants = [
            struct.pack('<QQQQ', offset, offset, offset, offset)[:32],
            struct.pack('>QQQQ', offset, offset, offset, offset)[:32],
            (offset.to_bytes(8, 'little') * 4)[:32],
            (offset.to_bytes(8, 'big') * 4)[:32],
        ]
        
        # IV from offset
        offset_iv_variants = [
            struct.pack('<Q', offset) + b'\x00' * 8,
            struct.pack('>Q', offset) + b'\x00' * 8,
            offset.to_bytes(8, 'little') + b'\x00' * 8,
            offset.to_bytes(8, 'big') + b'\x00' * 8,
        ]
        
        for key in offset_key_variants:
            for iv in offset_iv_variants:
                try:
                    counter = Counter.new(128, initial_value=int.from_bytes(iv, 'big'))
                    cipher = AES.new(key, AES.MODE_CTR, counter=counter)
                    decrypted = cipher.decrypt(encrypted_data)
                    
                    entropy = calculate_entropy(decrypted)
                    
                    if len(decrypted) >= 12:
                        tile_id = struct.unpack('<H', decrypted[:2])[0]
                        x = struct.unpack('<h', decrypted[2:4])[0]
                        y = struct.unpack('<h', decrypted[4:6])[0]
                        z = struct.unpack('<h', decrypted[6:8])[0]
                        
                        if (0 < tile_id < 0x4000 and 
                            -100 < x < 100 and 
                            -100 < y < 100 and 
                            -128 <= z <= 127 and
                            entropy < 6.0):
                            print(f"\n[POTENTIAL MATCH - Offset-based]")
                            print(f"  Key: {key.hex()}")
                            print(f"  IV: {iv.hex()}")
                            print(f"  Entropy: {entropy:.4f}")
                            return key, iv
                except:
                    continue
    
    print("\nNo matches found in small keyspace.")
    return None, None

if __name__ == "__main__":
    sag_path = r"D:\Ultima Online - Sagas\UOData\multi.sag"
    idx_path = r"D:\Ultima Online - Sagas\UOData\multi.idx"
    
    if os.path.exists(sag_path):
        key, iv = try_small_keyspace(sag_path, idx_path, 0)
        if key and iv:
            print(f"\nFound key/IV:")
            print(f"Key: {key.hex()}")
            print(f"IV: {iv.hex()}")
