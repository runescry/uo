"""
Enhanced binary analysis to find hardcoded encryption keys and IVs
"""
import os
import struct
import re

def find_hardcoded_keys_ivs():
    """Search for hardcoded 32-byte keys and 16-byte IVs in the binary"""
    exe_path = r"D:\Ultima Online - Sagas\ClassicUO\ClassicUO.exe"
    
    if not os.path.exists(exe_path):
        print(f"Executable not found: {exe_path}")
        return
    
    with open(exe_path, 'rb') as f:
        data = f.read()
    
    print("="*60)
    print("Searching for hardcoded encryption keys (32 bytes)...")
    print("="*60)
    
    # Look for 32-byte sequences that could be keys
    # Keys are unlikely to be all zeros, all 0xFF, or highly repetitive
    potential_keys = []
    
    for i in range(len(data) - 32):
        key_candidate = data[i:i+32]
        
        # Skip if too repetitive
        if len(set(key_candidate)) < 4:
            continue
        
        # Skip if all zeros or all 0xFF
        if key_candidate == b'\x00' * 32 or key_candidate == b'\xFF' * 32:
            continue
        
        # Check entropy (keys should have reasonable entropy)
        byte_counts = [0] * 256
        for byte in key_candidate:
            byte_counts[byte] += 1
        
        entropy = 0
        import math
        for count in byte_counts:
            if count > 0:
                p = count / 32
                entropy += -p * math.log2(p)
        
        # Keys should have entropy > 3.0 (not too structured)
        if 3.0 < entropy < 8.0:
            # Check if near encryption-related strings
            context_start = max(0, i - 200)
            context_end = min(len(data), i + 32 + 200)
            context = data[context_start:context_end]
            
            # Look for encryption-related strings nearby
            crypto_strings = [b'decrypt', b'encrypt', b'AES', b'CTR', b'UOFileSag', b'Key', b'IV']
            has_crypto_context = any(s in context for s in crypto_strings)
            
            if has_crypto_context:
                potential_keys.append((i, key_candidate, entropy, context))
    
    print(f"Found {len(potential_keys)} potential keys near crypto-related code")
    
    for idx, (offset, key, entropy, context) in enumerate(potential_keys[:20]):  # Show first 20
        print(f"\nKey candidate {idx+1} at offset {hex(offset)}:")
        print(f"  Key (hex): {key.hex()}")
        print(f"  Entropy: {entropy:.4f}")
        
        # Extract readable strings from context
        strings_in_context = []
        current = b''
        for byte in context:
            if 32 <= byte < 127:
                current += bytes([byte])
            else:
                if len(current) >= 4:
                    try:
                        s = current.decode('ascii')
                        if any(kw in s.lower() for kw in ['file', 'decrypt', 'encrypt', 'aes', 'key', 'iv', 'sag']):
                            strings_in_context.append(s)
                    except:
                        pass
                current = b''
        
        if strings_in_context:
            print(f"  Nearby strings:")
            for s in strings_in_context[:5]:
                print(f"    {s}")
    
    print("\n" + "="*60)
    print("Searching for hardcoded IVs (16 bytes)...")
    print("="*60)
    
    # Look for 16-byte sequences that could be IVs
    potential_ivs = []
    
    for i in range(len(data) - 16):
        iv_candidate = data[i:i+16]
        
        # Skip if too repetitive
        if len(set(iv_candidate)) < 2:
            continue
        
        # Check if near encryption-related strings or keys
        context_start = max(0, i - 200)
        context_end = min(len(data), i + 16 + 200)
        context = data[context_start:context_end]
        
        # Look for encryption-related strings nearby
        crypto_strings = [b'decrypt', b'encrypt', b'AES', b'CTR', b'UOFileSag', b'Key', b'IV', b'Counter']
        has_crypto_context = any(s in context for s in crypto_strings)
        
        # Also check if near a potential key
        near_key = False
        for key_offset, _, _, _ in potential_keys[:10]:
            if abs(i - key_offset) < 100:
                near_key = True
                break
        
        if has_crypto_context or near_key:
            potential_ivs.append((i, iv_candidate, context))
    
    print(f"Found {len(potential_ivs)} potential IVs near crypto-related code")
    
    for idx, (offset, iv, context) in enumerate(potential_ivs[:20]):  # Show first 20
        print(f"\nIV candidate {idx+1} at offset {hex(offset)}:")
        print(f"  IV (hex): {iv.hex()}")
    
    # Look for key derivation patterns
    print("\n" + "="*60)
    print("Searching for key derivation patterns...")
    print("="*60)
    
    # Look for common hash function names
    hash_functions = [b'MD5', b'SHA256', b'SHA1', b'Hash', b'DeriveKey', b'PBKDF2']
    for hash_func in hash_functions:
        matches = list(re.finditer(re.escape(hash_func), data, re.IGNORECASE))
        if matches:
            print(f"\nFound '{hash_func.decode('latin1', errors='ignore')}': {len(matches)} occurrences")
            for match in matches[:5]:
                start = max(0, match.start() - 100)
                end = min(len(data), match.end() + 100)
                context = data[start:end]
                
                # Extract strings
                strings_here = []
                current = b''
                for byte in context:
                    if 32 <= byte < 127:
                        current += bytes([byte])
                    else:
                        if len(current) >= 4:
                            try:
                                strings_here.append(current.decode('ascii'))
                            except:
                                pass
                        current = b''
                
                if strings_here:
                    print(f"  At offset {hex(match.start())}:")
                    for s in strings_here[:3]:
                        if any(kw in s.lower() for kw in ['key', 'file', 'sag', 'mul', 'derive']):
                            print(f"    {s}")

if __name__ == "__main__":
    find_hardcoded_keys_ivs()
