"""
Attempt to read .sag files - these appear to be encrypted/compressed versions of .mul files
"""
import struct
import os
import zlib
import gzip

class SAGReader:
    """Reader for .sag files - attempts various decryption/decompression methods"""
    
    def __init__(self, sag_path):
        self.sag_path = sag_path
        self.file_size = os.path.getsize(sag_path)
        self.data = None
        
    def read_raw(self):
        """Read raw file data"""
        with open(self.sag_path, 'rb') as f:
            return f.read()
    
    def try_decompress_zlib(self, data=None):
        """Try decompressing with zlib"""
        if data is None:
            data = self.read_raw()
        
        try:
            # Try decompressing the whole file
            decompressed = zlib.decompress(data)
            print(f"  [OK] ZLib decompression successful: {len(data)} -> {len(decompressed)} bytes")
            return decompressed
        except:
            try:
                # Try skipping header (maybe first N bytes are not compressed)
                for skip in [16, 32, 64, 128]:
                    try:
                        decompressed = zlib.decompress(data[skip:])
                        print(f"  [OK] ZLib decompression successful (skipped {skip} bytes): {len(data)} -> {len(decompressed)} bytes")
                        return decompressed
                    except:
                        continue
            except:
                pass
        
        return None
    
    def try_decompress_gzip(self, data=None):
        """Try decompressing with gzip"""
        if data is None:
            data = self.read_raw()
        
        try:
            decompressed = gzip.decompress(data)
            print(f"  [OK] GZip decompression successful: {len(data)} -> {len(decompressed)} bytes")
            return decompressed
        except:
            return None
    
    def try_xor_decrypt(self, data=None, key=None):
        """Try XOR decryption"""
        if data is None:
            data = self.read_raw()
        
        if key is None:
            # Try common single-byte keys
            keys = [0x00, 0xFF, 0xAA, 0x55, 0x5A, 0xA5, 0x96, 0x69]
        else:
            keys = [key]
        
        for k in keys:
            decrypted = bytes([b ^ k for b in data[:128]])
            
            # Check if decrypted looks like known formats
            if decrypted[:4] == b'MYP\x00':  # UOP
                print(f"  [OK] XOR decryption successful with key 0x{k:02X} (UOP format detected)")
                return bytes([b ^ k for b in data])
            if decrypted[:2] == b'PK':  # ZIP
                print(f"  [OK] XOR decryption successful with key 0x{k:02X} (ZIP format detected)")
                return bytes([b ^ k for b in data])
            if decrypted[:2] == b'\x1f\x8b':  # GZIP
                print(f"  [OK] XOR decryption successful with key 0x{k:02X} (GZIP format detected)")
                return bytes([b ^ k for b in data])
        
        return None
    
    def try_multi_byte_xor(self, data=None):
        """Try multi-byte XOR keys"""
        if data is None:
            data = self.read_raw()
        
        # Try common multi-byte patterns
        keys = [
            b'\x00' * 4,
            b'\xFF' * 4,
            b'UO\x00\x00',
            b'SAG\x00',
            b'MUL\x00',
        ]
        
        for key in keys:
            if len(key) == 0:
                continue
            decrypted = bytes([data[i] ^ key[i % len(key)] for i in range(min(128, len(data)))])
            
            if decrypted[:4] == b'MYP\x00' or decrypted[:2] == b'PK' or decrypted[:2] == b'\x1f\x8b':
                print(f"  [OK] Multi-byte XOR decryption successful with key {key.hex()}")
                return bytes([data[i] ^ key[i % len(key)] for i in range(len(data))])
        
        return None
    
    def analyze_structure(self):
        """Analyze file structure"""
        print(f"\nAnalyzing: {os.path.basename(self.sag_path)}")
        print(f"File size: {self.file_size:,} bytes")
        
        data = self.read_raw()
        
        # Try various methods
        print("\nTrying decompression methods:")
        result = self.try_decompress_zlib(data)
        if result:
            return result
        
        result = self.try_decompress_gzip(data)
        if result:
            return result
        
        print("\nTrying decryption methods:")
        result = self.try_xor_decrypt(data)
        if result:
            # Try decompressing the decrypted data
            decompressed = self.try_decompress_zlib(result)
            if decompressed:
                return decompressed
            return result
        
        result = self.try_multi_byte_xor(data)
        if result:
            decompressed = self.try_decompress_zlib(result)
            if decompressed:
                return decompressed
            return result
        
        print("\n  X Could not decrypt/decompress with standard methods")
        print("  File may use custom encryption or compression")
        
        return None
    
    def read_with_index(self, idx_path, entry_id):
        """Read a specific entry using an index file (like multi.idx)"""
        if not os.path.exists(idx_path):
            print(f"Index file not found: {idx_path}")
            return None
        
        # Read index entry (standard UO format: offset(4) + length(4) + extra(4) = 12 bytes)
        with open(idx_path, 'rb') as idx:
            idx.seek(entry_id * 12)
            entry_data = idx.read(12)
            
            if len(entry_data) < 12:
                return None
            
            offset, length, extra = struct.unpack('<III', entry_data)
            
            if offset == 0xFFFFFFFF or length == 0:
                return None
            
            print(f"Index entry {entry_id}: offset={offset}, length={length}, extra={extra}")
        
        # Try to read from .sag file
        # First, try to decrypt/decompress the whole file
        decrypted = self.analyze_structure()
        
        if decrypted:
            if offset < len(decrypted):
                return decrypted[offset:offset+length]
        else:
            # If we can't decrypt, try reading raw (might work if encryption is per-block)
            with open(self.sag_path, 'rb') as f:
                f.seek(offset)
                return f.read(length)
        
        return None


def test_sag_reader():
    """Test reading various .sag files"""
    base_path = r"D:\Ultima Online - Sagas\UOData"
    
    test_files = [
        ("multi.sag", "multi.idx", 0),  # Try reading first multi
        ("tiledata.sag", None, None),
        ("art.sag", None, None),
    ]
    
    for sag_file, idx_file, entry_id in test_files:
        sag_path = os.path.join(base_path, sag_file)
        if not os.path.exists(sag_path):
            continue
        
        reader = SAGReader(sag_path)
        
        if idx_file and entry_id is not None:
            idx_path = os.path.join(base_path, idx_file)
            print(f"\n{'='*60}")
            print(f"Testing: {sag_file} with index {idx_file}, entry {entry_id}")
            print(f"{'='*60}")
            data = reader.read_with_index(idx_path, entry_id)
            if data:
                print(f"Successfully read {len(data)} bytes")
                print(f"First 32 bytes: {data[:32].hex()}")
        else:
            print(f"\n{'='*60}")
            print(f"Testing: {sag_file}")
            print(f"{'='*60}")
            result = reader.analyze_structure()
            if result:
                print(f"Successfully processed: {len(result)} bytes")
                print(f"First 32 bytes: {result[:32].hex()}")


if __name__ == "__main__":
    test_sag_reader()
