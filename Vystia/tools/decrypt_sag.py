"""
Comprehensive .sag file decryption tool
Supports decrypting entire files or specific entries using AES-256-CTR
"""
import os
import sys
import struct
import argparse
from Crypto.Cipher import AES
from Crypto.Util import Counter

def decrypt_data(encrypted_data, key, iv):
    """
    Decrypt data using AES-256-CTR
    
    Args:
        encrypted_data: Bytes to decrypt
        key: 32-byte encryption key
        iv: 16-byte initialization vector
    
    Returns:
        Decrypted data
    """
    if len(key) != 32:
        raise ValueError(f"Key must be 32 bytes, got {len(key)} bytes")
    if len(iv) != 16:
        raise ValueError(f"IV must be 16 bytes, got {len(iv)} bytes")
    
    # Create counter for CTR mode
    counter = Counter.new(128, initial_value=int.from_bytes(iv, 'big'))
    
    # Create cipher and decrypt
    cipher = AES.new(key, AES.MODE_CTR, counter=counter)
    decrypted = cipher.decrypt(encrypted_data)
    
    return decrypted

def decrypt_entry(sag_path, idx_path, entry_id, key, iv, output_path=None):
    """
    Decrypt a specific entry from a .sag file using its index
    
    Args:
        sag_path: Path to .sag file
        idx_path: Path to .idx file
        entry_id: Entry ID to decrypt
        key: 32-byte encryption key
        iv: 16-byte initialization vector
        output_path: Optional path to save decrypted data
    
    Returns:
        Decrypted data
    """
    if not os.path.exists(sag_path):
        raise FileNotFoundError(f".sag file not found: {sag_path}")
    if not os.path.exists(idx_path):
        raise FileNotFoundError(f".idx file not found: {idx_path}")
    
    # Read index entry
    with open(idx_path, 'rb') as idx:
        idx.seek(entry_id * 12)
        entry_data = idx.read(12)
        
        if len(entry_data) < 12:
            raise ValueError(f"Invalid entry ID: {entry_id}")
        
        offset, length, extra = struct.unpack('<III', entry_data)
        
        if offset == 0xFFFFFFFF or length == 0:
            raise ValueError(f"Entry {entry_id} is empty or invalid")
    
    # Read encrypted data
    with open(sag_path, 'rb') as f:
        f.seek(offset)
        encrypted_data = f.read(length)
    
    if len(encrypted_data) != length:
        raise ValueError(f"Could not read full entry: expected {length} bytes, got {len(encrypted_data)}")
    
    # Decrypt
    decrypted = decrypt_data(encrypted_data, key, iv)
    
    # Save if output path specified
    if output_path:
        os.makedirs(os.path.dirname(output_path) or '.', exist_ok=True)
        with open(output_path, 'wb') as f:
            f.write(decrypted)
        print(f"Decrypted entry {entry_id} saved to: {output_path}")
    
    return decrypted

def decrypt_file(sag_path, key, iv, output_path=None, chunk_size=1024*1024):
    """
    Decrypt an entire .sag file
    
    Args:
        sag_path: Path to .sag file
        key: 32-byte encryption key
        iv: 16-byte initialization vector
        output_path: Path to save decrypted file (default: .sag -> .mul)
        chunk_size: Size of chunks to process (default: 1MB)
    
    Returns:
        Path to decrypted file
    """
    if not os.path.exists(sag_path):
        raise FileNotFoundError(f".sag file not found: {sag_path}")
    
    # Determine output path
    if not output_path:
        output_path = sag_path.replace('.sag', '.mul')
        if output_path == sag_path:
            output_path = sag_path + '.decrypted'
    
    file_size = os.path.getsize(sag_path)
    
    print(f"Decrypting {os.path.basename(sag_path)} ({file_size:,} bytes)...")
    print(f"Output: {output_path}")
    
    # Decrypt in chunks
    with open(sag_path, 'rb') as infile, open(output_path, 'wb') as outfile:
        bytes_processed = 0
        
        while True:
            chunk = infile.read(chunk_size)
            if not chunk:
                break
            
            # Decrypt chunk
            decrypted_chunk = decrypt_data(chunk, key, iv)
            outfile.write(decrypted_chunk)
            
            bytes_processed += len(chunk)
            if file_size > 0:
                percent = (bytes_processed / file_size) * 100
                print(f"Progress: {percent:.1f}% ({bytes_processed:,}/{file_size:,} bytes)", end='\r')
    
    print(f"\nDecryption complete: {output_path}")
    return output_path

def parse_key_iv(key_str, iv_str):
    """
    Parse key and IV from hex strings or files
    
    Args:
        key_str: Hex string or path to file containing key
        iv_str: Hex string or path to file containing IV
    
    Returns:
        Tuple of (key_bytes, iv_bytes)
    """
    # Parse key
    if os.path.exists(key_str):
        with open(key_str, 'rb') as f:
            key = f.read(32)
    else:
        try:
            key = bytes.fromhex(key_str.replace(' ', '').replace('-', ''))
        except ValueError:
            raise ValueError(f"Invalid key format: {key_str}")
    
    if len(key) != 32:
        raise ValueError(f"Key must be 32 bytes (64 hex chars), got {len(key)} bytes")
    
    # Parse IV
    if os.path.exists(iv_str):
        with open(iv_str, 'rb') as f:
            iv = f.read(16)
    else:
        try:
            iv = bytes.fromhex(iv_str.replace(' ', '').replace('-', ''))
        except ValueError:
            raise ValueError(f"Invalid IV format: {iv_str}")
    
    if len(iv) != 16:
        raise ValueError(f"IV must be 16 bytes (32 hex chars), got {len(iv)} bytes")
    
    return key, iv

def main():
    parser = argparse.ArgumentParser(description='Decrypt .sag files using AES-256-CTR')
    parser.add_argument('sag_file', help='Path to .sag file')
    parser.add_argument('--key', required=True, help='32-byte key as hex string or path to key file')
    parser.add_argument('--iv', required=True, help='16-byte IV as hex string or path to IV file')
    parser.add_argument('--idx', help='Path to .idx file (for entry decryption)')
    parser.add_argument('--entry', type=int, help='Entry ID to decrypt (requires --idx)')
    parser.add_argument('--output', help='Output file path (default: .sag -> .mul)')
    parser.add_argument('--chunk-size', type=int, default=1024*1024, help='Chunk size for file decryption (default: 1MB)')
    
    args = parser.parse_args()
    
    try:
        # Parse key and IV
        key, iv = parse_key_iv(args.key, args.iv)
        
        print(f"Key: {key.hex()}")
        print(f"IV:  {iv.hex()}")
        print()
        
        # Decrypt entry or entire file
        if args.entry is not None:
            if not args.idx:
                parser.error("--idx is required when using --entry")
            
            decrypted = decrypt_entry(args.sag_file, args.idx, args.entry, key, iv, args.output)
            print(f"Decrypted {len(decrypted)} bytes")
        else:
            output_path = decrypt_file(args.sag_file, key, iv, args.output, args.chunk_size)
            print(f"Successfully decrypted to: {output_path}")
    
    except Exception as e:
        print(f"Error: {e}", file=sys.stderr)
        sys.exit(1)

if __name__ == "__main__":
    main()
