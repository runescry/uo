"""
Find decryption-related code in Sagas ClassicUO.exe
Focus on AES, file reading, and .sag handling
"""
import os
import re

def find_decryption_code():
    exe_path = r"D:\Ultima Online - Sagas\ClassicUO\ClassicUO.exe"
    
    with open(exe_path, 'rb') as f:
        data = f.read()
    
    print("="*60)
    print("Searching for decryption-related code patterns...")
    print("="*60)
    
    # Key patterns to find
    patterns = {
        'AES': [
            b'DecryptAES256_CTR',
            b'AES256',
            b'AES',
            b'Rijndael',
        ],
        'File Reading': [
            b'GetUOFilePath',
            b'UOFile',
            b'FileStream',
            b'ReadByte',
            b'ReadBytes',
        ],
        'SAG Files': [
            b'.sag',
            b'.SAG',
        ],
        'Decryption': [
            b'Decrypt',
            b'decrypt',
            b'CopyDecryptedData',
        ],
    }
    
    results = {}
    
    for category, pattern_list in patterns.items():
        print(f"\n{category}:")
        results[category] = []
        
        for pattern in pattern_list:
            matches = list(re.finditer(re.escape(pattern), data, re.IGNORECASE))
            if matches:
                print(f"  Found '{pattern.decode('latin1', errors='ignore')}': {len(matches)} occurrences")
                
                for match in matches[:5]:  # Show first 5
                    start = max(0, match.start() - 100)
                    end = min(len(data), match.end() + 100)
                    context = data[start:end]
                    
                    # Try to extract readable strings from context
                    readable_parts = []
                    current = b''
                    for byte in context:
                        if 32 <= byte < 127:
                            current += bytes([byte])
                        else:
                            if len(current) >= 4:
                                try:
                                    readable_parts.append(current.decode('ascii'))
                                except:
                                    pass
                            current = b''
                    
                    if readable_parts:
                        print(f"    Offset {hex(match.start())}:")
                        for part in readable_parts[:10]:  # Show first 10 readable parts
                            if any(kw in part.lower() for kw in ['file', 'read', 'decrypt', 'encrypt', 'sag', 'mul', 'stream']):
                                print(f"      - {part}")
                    
                    results[category].append({
                        'pattern': pattern,
                        'offset': match.start(),
                        'context': context[:200]  # First 200 bytes
                    })
    
    # Look for specific "DecryptAES256_CTR" context
    print("\n" + "="*60)
    print("Analyzing DecryptAES256_CTR context...")
    print("="*60)
    
    aes_pattern = b'DecryptAES256_CTR'
    aes_matches = list(re.finditer(re.escape(aes_pattern), data, re.IGNORECASE))
    
    if aes_matches:
        print(f"Found {len(aes_matches)} occurrences of DecryptAES256_CTR")
        
        for i, match in enumerate(aes_matches[:3]):  # Analyze first 3
            print(f"\nOccurrence {i+1} at offset {hex(match.start())}:")
            
            # Get larger context
            start = max(0, match.start() - 500)
            end = min(len(data), match.end() + 500)
            context = data[start:end]
            
            # Extract all readable strings from this region
            strings_in_context = []
            current = b''
            for byte in context:
                if 32 <= byte < 127:
                    current += bytes([byte])
                else:
                    if len(current) >= 3:
                        try:
                            s = current.decode('ascii')
                            if any(kw in s.lower() for kw in ['file', 'read', 'decrypt', 'encrypt', 'sag', 'mul', 'stream', 'aes', 'key', 'iv']):
                                strings_in_context.append(s)
                        except:
                            pass
                    current = b''
            
            print(f"  Relevant strings in context ({len(strings_in_context)} found):")
            for s in strings_in_context[:20]:
                print(f"    {s}")
            
            # Look for hex patterns that might be keys/IVs
            print(f"\n  Hex dump of context (first 200 bytes):")
            hex_dump = ' '.join(f'{b:02x}' for b in context[:200])
            print(f"    {hex_dump}")
    
    # Try to find file extension replacement logic
    print("\n" + "="*60)
    print("Looking for file extension handling...")
    print("="*60)
    
    # Look for patterns like ".mul" followed by ".sag" or vice versa
    mul_positions = [m.start() for m in re.finditer(b'\.mul', data, re.IGNORECASE)]
    sag_positions = [m.start() for m in re.finditer(b'\.sag', data, re.IGNORECASE)]
    
    print(f"Found {len(mul_positions)} '.mul' references")
    print(f"Found {len(sag_positions)} '.sag' references")
    
    # Check if .sag appears near .mul (might indicate extension replacement)
    for sag_pos in sag_positions[:10]:
        for mul_pos in mul_positions:
            if abs(sag_pos - mul_pos) < 50:  # Within 50 bytes
                print(f"\nFound .sag and .mul close together:")
                print(f"  .sag at {hex(sag_pos)}, .mul at {hex(mul_pos)}")
                context_start = max(0, min(sag_pos, mul_pos) - 50)
                context_end = max(sag_pos, mul_pos) + 10
                context = data[context_start:context_end]
                try:
                    readable = ''.join(c if 32 <= c < 127 else '.' for c in context)
                    print(f"  Context: ...{readable}...")
                except:
                    pass
                break

if __name__ == "__main__":
    find_decryption_code()
