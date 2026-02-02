"""
Analyze Sagas ClassicUO.exe to find .sag file handling and decryption
"""
import os
import re

def analyze_exe():
    exe_path = r"D:\Ultima Online - Sagas\ClassicUO\ClassicUO.exe"
    
    if not os.path.exists(exe_path):
        print(f"Executable not found: {exe_path}")
        return
    
    file_size = os.path.getsize(exe_path)
    print(f"Analyzing: {exe_path}")
    print(f"File size: {file_size:,} bytes")
    
    with open(exe_path, 'rb') as f:
        data = f.read()
    
    # Check if it's a .NET assembly
    print("\n" + "="*60)
    print("Checking file type...")
    print("="*60)
    
    is_dotnet = b'\x42\x53\x4A\x42' in data[:1024*1024]  # .NET signature
    is_pe = data[:2] == b'MZ'
    
    print(f"Is PE executable: {is_pe}")
    print(f"Is .NET assembly: {is_dotnet}")
    
    # Search for .sag references
    print("\n" + "="*60)
    print("Searching for .sag file references...")
    print("="*60)
    
    sag_patterns = [
        b'.sag',
        b'.SAG',
        b'sag',
        b'SAG',
        b'\.sag',
    ]
    
    for pattern in sag_patterns:
        matches = []
        start = 0
        while True:
            pos = data.find(pattern, start)
            if pos == -1:
                break
            matches.append(pos)
            start = pos + 1
        
        if matches:
            print(f"\nFound {len(matches)} occurrences of '{pattern.decode('latin1', errors='ignore')}':")
            for i, offset in enumerate(matches[:10]):  # Show first 10
                context_start = max(0, offset - 40)
                context_end = min(len(data), offset + 40)
                context = data[context_start:context_end]
                
                # Try to extract readable strings
                try:
                    readable = ''.join(c if 32 <= c < 127 else '.' for c in context)
                    print(f"  Offset {hex(offset)}: ...{readable}...")
                except:
                    print(f"  Offset {hex(offset)}: (binary data)")
    
    # Search for encryption/decryption related strings
    print("\n" + "="*60)
    print("Searching for encryption/decryption keywords...")
    print("="*60)
    
    crypto_keywords = [
        b'decrypt',
        b'encrypt',
        b'Decrypt',
        b'Encrypt',
        b'cipher',
        b'Cipher',
        b'XOR',
        b'xor',
        b'AES',
        b'DES',
        b'Rijndael',
        b'CryptoStream',
        b'ICryptoTransform',
    ]
    
    for keyword in crypto_keywords:
        count = data.count(keyword)
        if count > 0:
            print(f"Found '{keyword.decode('latin1', errors='ignore')}': {count} occurrences")
            # Find first occurrence with context
            pos = data.find(keyword)
            if pos != -1:
                context_start = max(0, pos - 60)
                context_end = min(len(data), pos + 60)
                context = data[context_start:context_end]
                try:
                    readable = ''.join(c if 32 <= c < 127 else '.' for c in context)
                    print(f"  First at {hex(pos)}: ...{readable}...")
                except:
                    pass
    
    # Search for file reading patterns
    print("\n" + "="*60)
    print("Searching for file I/O patterns...")
    print("="*60)
    
    file_patterns = [
        b'FileStream',
        b'File.Open',
        b'File.Read',
        b'GetUOFilePath',
        b'UOFile',
        b'ReadByte',
        b'ReadBytes',
    ]
    
    for pattern in file_patterns:
        count = data.count(pattern)
        if count > 0:
            print(f"Found '{pattern.decode('latin1', errors='ignore')}': {count} occurrences")
    
    # Try to extract readable strings that might be relevant
    print("\n" + "="*60)
    print("Extracting potentially relevant strings...")
    print("="*60)
    
    # Look for strings that contain file-related or crypto-related terms
    strings = extract_strings(data, min_length=4)
    
    relevant_strings = []
    keywords = ['sag', 'mul', 'decrypt', 'encrypt', 'cipher', 'file', 'read', 'stream']
    
    for s in strings:
        s_lower = s.lower()
        if any(kw in s_lower for kw in keywords):
            relevant_strings.append(s)
    
    print(f"\nFound {len(relevant_strings)} potentially relevant strings:")
    for s in relevant_strings[:50]:  # Show first 50
        if len(s) < 200:  # Skip very long strings
            print(f"  {s}")

def extract_strings(data, min_length=4):
    """Extract readable strings from binary data"""
    strings = []
    current_string = b''
    
    for byte in data:
        if 32 <= byte < 127:  # Printable ASCII
            current_string += bytes([byte])
        else:
            if len(current_string) >= min_length:
                try:
                    strings.append(current_string.decode('ascii'))
                except:
                    pass
            current_string = b''
    
    # Add last string if exists
    if len(current_string) >= min_length:
        try:
            strings.append(current_string.decode('ascii'))
        except:
            pass
    
    return strings

if __name__ == "__main__":
    analyze_exe()
