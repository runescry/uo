"""
Find UOFileSag class and related decryption code
"""
import os
import re

def find_uofilesag():
    exe_path = r"D:\Ultima Online - Sagas\ClassicUO\ClassicUO.exe"
    
    with open(exe_path, 'rb') as f:
        data = f.read()
    
    print("="*60)
    print("Searching for UOFileSag references...")
    print("="*60)
    
    # Find all occurrences of UOFileSag
    uofilesag_pattern = b'UOFileSag'
    matches = list(re.finditer(re.escape(uofilesag_pattern), data, re.IGNORECASE))
    
    print(f"Found {len(matches)} occurrences of 'UOFileSag'\n")
    
    for i, match in enumerate(matches[:5]):  # Analyze first 5
        print(f"Occurrence {i+1} at offset {hex(match.start())}:")
        
        # Get large context
        start = max(0, match.start() - 500)
        end = min(len(data), match.end() + 500)
        context = data[start:end]
        
        # Extract readable strings
        strings = []
        current = b''
        for byte in context:
            if 32 <= byte < 127:
                current += bytes([byte])
            else:
                if len(current) >= 3:
                    try:
                        s = current.decode('ascii')
                        strings.append(s)
                    except:
                        pass
                current = b''
        
        # Filter relevant strings
        relevant = [s for s in strings if any(kw in s.lower() for kw in 
            ['file', 'read', 'decrypt', 'encrypt', 'sag', 'mul', 'stream', 'aes', 'key', 'iv', 'uofile', 'class', 'method'])]
        
        print(f"  Relevant strings ({len(relevant)} found):")
        for s in relevant[:30]:
            print(f"    {s}")
        
        # Show hex context
        print(f"\n  Hex context (200 bytes around match):")
        hex_start = max(0, match.start() - 100)
        hex_end = min(len(data), match.end() + 100)
        hex_context = data[hex_start:hex_end]
        hex_str = ' '.join(f'{b:02x}' for b in hex_context[:200])
        print(f"    {hex_str}")
        print()
    
    # Look for patterns that might indicate file extension replacement
    print("="*60)
    print("Looking for file extension replacement logic...")
    print("="*60)
    
    # Search for patterns like "mul" near "sag" or file path manipulation
    file_related_patterns = [
        (b'\.mul', b'\.sag'),
        (b'mul', b'sag'),
        (b'Replace', b'sag'),
        (b'Substring', b'sag'),
    ]
    
    for pattern1, pattern2 in file_related_patterns:
        matches1 = list(re.finditer(pattern1, data, re.IGNORECASE))
        matches2 = list(re.finditer(pattern2, data, re.IGNORECASE))
        
        # Check if they appear close together
        for m1 in matches1[:20]:
            for m2 in matches2[:20]:
                if abs(m1.start() - m2.start()) < 100:
                    print(f"\nFound {pattern1} and {pattern2} close together:")
                    print(f"  {pattern1.decode('latin1', errors='ignore')} at {hex(m1.start())}")
                    print(f"  {pattern2.decode('latin1', errors='ignore')} at {hex(m2.start())}")
                    
                    context_start = max(0, min(m1.start(), m2.start()) - 50)
                    context_end = max(m1.end(), m2.end()) + 50
                    context = data[context_start:context_end]
                    
                    # Extract strings from this region
                    strings_here = []
                    current = b''
                    for byte in context:
                        if 32 <= byte < 127:
                            current += bytes([byte])
                        else:
                            if len(current) >= 3:
                                try:
                                    strings_here.append(current.decode('ascii'))
                                except:
                                    pass
                            current = b''
                    
                    if strings_here:
                        print(f"  Strings in region:")
                        for s in strings_here[:10]:
                            print(f"    {s}")
                    break
    
    # Look for encryption key/IV patterns
    print("\n" + "="*60)
    print("Looking for encryption key/IV patterns...")
    print("="*60)
    
    key_patterns = [
        b'Key',
        b'IV',
        b'InitializationVector',
        b'SecretKey',
        b'EncryptionKey',
    ]
    
    for pattern in key_patterns:
        matches = list(re.finditer(pattern, data, re.IGNORECASE))
        if matches:
            print(f"\nFound '{pattern.decode('latin1', errors='ignore')}': {len(matches)} occurrences")
            
            # Check context around first few matches
            for match in matches[:3]:
                start = max(0, match.start() - 200)
                end = min(len(data), match.end() + 200)
                context = data[start:end]
                
                # Look for hex patterns that might be keys (32 bytes for AES-256, 16 bytes for IV)
                # Check for sequences of hex-like bytes
                hex_like = []
                valid_bytes = set([0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f])
                for i in range(len(context) - 32):
                    chunk = context[i:i+32]
                    # Check if chunk looks like a key (mostly printable or common hex values)
                    if all((32 <= b < 127) or (b in valid_bytes) for b in chunk):
                        hex_like.append((i + start, chunk))
                
                if hex_like:
                    print(f"  At offset {hex(match.start())}, found {len(hex_like)} potential key/IV candidates:")
                    for offset, chunk in hex_like[:3]:
                        hex_str = ' '.join(f'{b:02x}' for b in chunk)
                        print(f"    Offset {hex(offset)}: {hex_str}")

if __name__ == "__main__":
    find_uofilesag()
