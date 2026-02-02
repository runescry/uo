"""
Try to decrypt .sag files using AES-256-CTR with various key/IV patterns
Enhanced with comprehensive pattern testing
"""
import os
import struct
import hashlib
import json
import winreg
from Crypto.Cipher import AES
from Crypto.Util import Counter

def get_registry_keys():
    """Try to get keys from Windows registry"""
    keys = []
    try:
        # Check HKCU
        try:
            with winreg.OpenKey(winreg.HKEY_CURRENT_USER, r"Software\UOSagas") as key:
                for i in range(winreg.QueryInfoKey(key)[1]):
                    name, value, _ = winreg.EnumValue(key, i)
                    if isinstance(value, (str, bytes)):
                        keys.append(value.encode('utf-8') if isinstance(value, str) else value)
        except:
            pass
        
        # Check HKLM
        try:
            with winreg.OpenKey(winreg.HKEY_LOCAL_MACHINE, r"Software\UOSagas") as key:
                for i in range(winreg.QueryInfoKey(key)[1]):
                    name, value, _ = winreg.EnumValue(key, i)
                    if isinstance(value, (str, bytes)):
                        keys.append(value.encode('utf-8') if isinstance(value, str) else value)
        except:
            pass
    except:
        pass
    return keys

def get_env_vars():
    """Get environment variables that might contain keys"""
    env_vars = []
    relevant_vars = ['UOSAGAS_KEY', 'SAGAS_KEY', 'UO_KEY', 'ENCRYPTION_KEY']
    for var in relevant_vars:
        value = os.environ.get(var)
        if value:
            env_vars.append(value.encode('utf-8'))
    return env_vars

def get_config_keys(config_path):
    """Extract potential keys from config files"""
    keys = []
    if os.path.exists(config_path):
        try:
            with open(config_path, 'r') as f:
                config = json.load(f)
                # Look for any string values that might be keys
                for key, value in config.items():
                    if isinstance(value, str) and len(value) >= 16:
                        keys.append(value.encode('utf-8'))
        except:
            pass
    return keys

def try_decrypt_with_patterns(sag_path, idx_path=None, entry_id=0):
    """Try various key/IV patterns to decrypt .sag files"""
    
    print(f"Attempting to decrypt: {os.path.basename(sag_path)}")
    
    # Read encrypted data
    offset = 0
    if idx_path and entry_id is not None and os.path.exists(idx_path):
        # Read specific entry using index
        with open(idx_path, 'rb') as idx:
            idx.seek(entry_id * 12)
            entry_data = idx.read(12)
            if len(entry_data) == 12:
                offset, length, extra = struct.unpack('<III', entry_data)
                if offset == 0xFFFFFFFF or length == 0:
                    offset = 0
                    length = 1024
            else:
                length = 1024
    else:
        length = 1024
    
    with open(sag_path, 'rb') as f:
        f.seek(offset)
        encrypted_data = f.read(min(length, 1024))  # Read first 1KB for testing
    
    print(f"Encrypted data size: {len(encrypted_data)} bytes")
    print(f"First 32 bytes (hex): {encrypted_data[:32].hex()}")
    
    # Build comprehensive key patterns
    filename = os.path.basename(sag_path).encode('ascii', errors='ignore')
    filepath = sag_path.encode('ascii', errors='ignore')
    base_path = os.path.dirname(sag_path).encode('ascii', errors='ignore')
    
    key_patterns = []
    
    # Basic patterns
    key_patterns.extend([
        b'\x00' * 32,  # Zero key
        b'\xFF' * 32,  # All 0xFF
        b'\xAA' * 32,  # All 0xAA
        b'\x55' * 32,  # All 0x55
    ])
    
    # UO/Sagas related strings
    uo_strings = [
        b'Ultima Online Sagas',
        b'Sagas',
        b'UOSagas',
        b'Ultima Online',
        b'ClassicUO',
        b'login.uosagas.com',
    ]
    for s in uo_strings:
        key_patterns.append(s.ljust(32, b'\x00'))
        key_patterns.append(s.ljust(32, b'\xFF'))
    
    # Filename-based
    if len(filename) > 0:
        key_patterns.append(filename[:32].ljust(32, b'\x00'))
        key_patterns.append(filename[:32].ljust(32, b'\xFF'))
        # Filename hash (MD5)
        md5_hash = hashlib.md5(filename).digest()
        key_patterns.append(md5_hash + md5_hash)  # 32 bytes
        # Filename hash (SHA256)
        key_patterns.append(hashlib.sha256(filename).digest())
        # Filename hash (simple)
        filename_hash = hash(filename) % (2**256)
        key_patterns.append(filename_hash.to_bytes(32, 'little'))
        key_patterns.append(filename_hash.to_bytes(32, 'big'))
    
    # Path-based
    if len(filepath) > 0:
        key_patterns.append(hashlib.sha256(filepath).digest())
        path_hash = hash(filepath) % (2**256)
        key_patterns.append(path_hash.to_bytes(32, 'little'))
    
    # Registry keys
    registry_keys = get_registry_keys()
    for reg_key in registry_keys:
        if len(reg_key) >= 32:
            key_patterns.append(reg_key[:32])
        elif len(reg_key) > 0:
            key_patterns.append(hashlib.sha256(reg_key).digest())
    
    # Environment variables
    env_keys = get_env_vars()
    for env_key in env_keys:
        if len(env_key) >= 32:
            key_patterns.append(env_key[:32])
        elif len(env_key) > 0:
            key_patterns.append(hashlib.sha256(env_key).digest())
    
    # Config file keys
    config_path = r"D:\Ultima Online - Sagas\ClassicUO\settings.json"
    config_keys = get_config_keys(config_path)
    for cfg_key in config_keys:
        if len(cfg_key) >= 32:
            key_patterns.append(cfg_key[:32])
        elif len(cfg_key) > 0:
            key_patterns.append(hashlib.sha256(cfg_key).digest())
    
    # Remove duplicates while preserving order
    seen = set()
    unique_keys = []
    for key in key_patterns:
        key_bytes = bytes(key[:32].ljust(32, b'\x00'))
        if key_bytes not in seen:
            seen.add(key_bytes)
            unique_keys.append(key_bytes)
    
    key_patterns = unique_keys
    
    # Build comprehensive IV patterns
    iv_patterns = []
    
    # Basic patterns
    iv_patterns.extend([
        b'\x00' * 16,  # Zero IV
        b'\xFF' * 16,  # All 0xFF
        b'\xAA' * 16,  # All 0xAA
        b'\x55' * 16,  # All 0x55
    ])
    
    # File offset-based
    iv_patterns.append(struct.pack('<Q', offset) + b'\x00' * 8)
    iv_patterns.append(struct.pack('>Q', offset) + b'\x00' * 8)
    iv_patterns.append(struct.pack('<Q', 0) + b'\x00' * 8)
    
    # Filename-based
    if len(filename) > 0:
        # Filename hash (MD5, first 16 bytes)
        iv_patterns.append(hashlib.md5(filename).digest()[:16])
        # Filename hash (simple)
        filename_iv_hash = hash(filename) % (2**128)
        iv_patterns.append(filename_iv_hash.to_bytes(16, 'little'))
        iv_patterns.append(filename_iv_hash.to_bytes(16, 'big'))
    
    # Entry ID based (for indexed files)
    if idx_path:
        iv_patterns.append(struct.pack('<Q', entry_id) + b'\x00' * 8)
        iv_patterns.append(struct.pack('>Q', entry_id) + b'\x00' * 8)
    
    # Remove duplicate IVs
    seen_iv = set()
    unique_ivs = []
    for iv in iv_patterns:
        iv_bytes = bytes(iv[:16].ljust(16, b'\x00'))
        if iv_bytes not in seen_iv:
            seen_iv.add(iv_bytes)
            unique_ivs.append(iv_bytes)
    
    iv_patterns = unique_ivs
    
    print(f"\nTesting {len(key_patterns)} key patterns and {len(iv_patterns)} IV patterns...")
    print(f"Total combinations: {len(key_patterns) * len(iv_patterns)}")
    
    print("\n" + "="*60)
    print("Trying key/IV combinations...")
    print("="*60)
    
    best_match = None
    best_entropy = 8.0
    best_valid = False
    
    for key_idx, key in enumerate(key_patterns):
        if key_idx % 10 == 0:
            print(f"Testing key pattern {key_idx}/{len(key_patterns)}...", end='\r')
        
        for iv_idx, iv in enumerate(iv_patterns):
            try:
                # Create counter
                counter = Counter.new(128, initial_value=int.from_bytes(iv, 'big'))
                
                # Decrypt
                cipher = AES.new(key, AES.MODE_CTR, counter=counter)
                decrypted = cipher.decrypt(encrypted_data)
                
                # Check entropy
                entropy = calculate_entropy(decrypted)
                
                # Check for valid UO data structures
                looks_valid = False
                validation_details = []
                
                # Try parsing as multi data (12 bytes per entry)
                if len(decrypted) >= 12:
                    tile_id = struct.unpack('<H', decrypted[:2])[0]
                    x = struct.unpack('<h', decrypted[2:4])[0]
                    y = struct.unpack('<h', decrypted[4:6])[0]
                    z = struct.unpack('<h', decrypted[6:8])[0]
                    flags = struct.unpack('<I', decrypted[8:12])[0]
                    
                    if (0 < tile_id < 0x4000 and 
                        -100 < x < 100 and 
                        -100 < y < 100 and 
                        -128 <= z <= 127):
                        looks_valid = True
                        validation_details.append(f"Valid multi: tile={tile_id}, x={x}, y={y}, z={z}")
                
                # Try parsing as map data (3 bytes per tile)
                if not looks_valid and len(decrypted) >= 3:
                    tile_id = struct.unpack('<H', decrypted[:2])[0]
                    z = struct.unpack('b', decrypted[2:3])[0]
                    
                    if tile_id < 0x4000 and -128 <= z <= 127:
                        looks_valid = True
                        validation_details.append(f"Valid map tile: tile={tile_id}, z={z}")
                
                # Try parsing as tiledata (1188 bytes per 32 entries)
                if not looks_valid and len(decrypted) >= 4:
                    # Tiledata has flags (4 or 8 bytes) followed by name (20 bytes)
                    # Check if first bytes look like flags
                    flags_test = struct.unpack('<I', decrypted[:4])[0]
                    if flags_test < 0xFFFFFFFF:  # Reasonable flag value
                        # Check if following bytes are ASCII (name field)
                        name_bytes = decrypted[4:24] if len(decrypted) >= 24 else decrypted[4:]
                        if all(32 <= b < 127 or b == 0 for b in name_bytes[:20]):
                            looks_valid = True
                            validation_details.append("Valid tiledata structure")
                
                # Track best match
                if looks_valid or entropy < best_entropy:
                    if looks_valid or (not best_valid and entropy < best_entropy):
                        best_match = (key, iv, decrypted, entropy, looks_valid, validation_details)
                        best_entropy = entropy
                        best_valid = looks_valid
                
                # Print potential matches
                if looks_valid or entropy < 5.0:
                    print(f"\n[POTENTIAL MATCH] Key pattern {key_idx}, IV pattern {iv_idx}")
                    print(f"  Key (hex): {key.hex()}")
                    print(f"  IV (hex): {iv.hex()}")
                    print(f"  Entropy: {entropy:.4f}")
                    if validation_details:
                        for detail in validation_details:
                            print(f"  {detail}")
                    print(f"  First 32 bytes decrypted: {decrypted[:32].hex()}")
                    
                    if looks_valid:
                        print(f"  ✓ Data looks valid!")
                        return key, iv, decrypted
                        
            except Exception as e:
                continue
    
    print()  # New line after progress
    
    if best_match:
        key, iv, decrypted, entropy, looks_valid, details = best_match
        print(f"\n[BEST MATCH] Entropy: {entropy:.4f}, Valid: {looks_valid}")
        print(f"  Key (hex): {key.hex()}")
        print(f"  IV (hex): {iv.hex()}")
        if details:
            for detail in details:
                print(f"  {detail}")
        return key, iv, decrypted
    
    print("\nNo valid decryption found with tested patterns.")
    return None, None, None

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

if __name__ == "__main__":
    # Test with multiple files
    test_files = [
        (r"D:\Ultima Online - Sagas\UOData\multi.sag", r"D:\Ultima Online - Sagas\UOData\multi.idx", 0),
        (r"D:\Ultima Online - Sagas\UOData\tiledata.sag", None, None),
        (r"D:\Ultima Online - Sagas\UOData\map0.sag", None, None),
    ]
    
    found_key = None
    found_iv = None
    
    for sag_path, idx_path, entry_id in test_files:
        if os.path.exists(sag_path):
            print("\n" + "="*60)
            print(f"Testing: {os.path.basename(sag_path)}")
            print("="*60)
            
            key, iv, decrypted = try_decrypt_with_patterns(sag_path, idx_path, entry_id)
            
            if key and iv:
                found_key = key
                found_iv = iv
                print("\n" + "="*60)
                print("SUCCESS! Found working key/IV combination")
                print("="*60)
                print(f"Key: {key.hex()}")
                print(f"IV: {iv.hex()}")
                break
    
    if not found_key:
        print("\n" + "="*60)
        print("Could not find working key/IV with tested patterns.")
        print("="*60)
        print("Key/IV might be:")
        print("  - Hardcoded in the binary (need deeper analysis)")
        print("  - Derived from a config file")
        print("  - Derived from registry/environment variables")
        print("  - Derived using a complex algorithm")
        print("  - Per-file unique (different key/IV for each file)")
