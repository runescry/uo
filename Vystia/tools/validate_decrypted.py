"""
Validate decrypted .sag files to ensure they match expected .mul format
"""
import os
import sys
import struct
import argparse

def validate_multi_file(mul_path, idx_path=None):
    """Validate a multi.mul file"""
    errors = []
    warnings = []
    
    if not os.path.exists(mul_path):
        return False, ["File not found"], []
    
    if idx_path and os.path.exists(idx_path):
        # Validate using index
        with open(idx_path, 'rb') as idx:
            idx_size = os.path.getsize(idx_path)
            entry_count = idx_size // 12
            
            print(f"Multi file: {os.path.basename(mul_path)}")
            print(f"Index entries: {entry_count}")
            
            valid_entries = 0
            invalid_entries = 0
            
            for i in range(min(entry_count, 100)):  # Check first 100 entries
                idx.seek(i * 12)
                offset, length, extra = struct.unpack('<III', idx.read(12))
                
                if offset == 0xFFFFFFFF or length == 0:
                    continue
                
                if offset + length > os.path.getsize(mul_path):
                    errors.append(f"Entry {i}: offset+length exceeds file size")
                    invalid_entries += 1
                    continue
                
                # Read and validate entry
                with open(mul_path, 'rb') as mul:
                    mul.seek(offset)
                    entry_data = mul.read(min(length, 12))
                    
                    if len(entry_data) >= 12:
                        tile_id = struct.unpack('<H', entry_data[:2])[0]
                        x = struct.unpack('<h', entry_data[2:4])[0]
                        y = struct.unpack('<h', entry_data[4:6])[0]
                        z = struct.unpack('<h', entry_data[6:8])[0]
                        
                        # Validate ranges
                        if not (0 < tile_id < 0x4000):
                            warnings.append(f"Entry {i}: suspicious tile_id {tile_id}")
                        if not (-100 < x < 100):
                            warnings.append(f"Entry {i}: suspicious x coordinate {x}")
                        if not (-100 < y < 100):
                            warnings.append(f"Entry {i}: suspicious y coordinate {y}")
                        if not (-128 <= z <= 127):
                            warnings.append(f"Entry {i}: suspicious z coordinate {z}")
                        
                        if (0 < tile_id < 0x4000 and 
                            -100 < x < 100 and 
                            -100 < y < 100 and 
                            -128 <= z <= 127):
                            valid_entries += 1
                        else:
                            invalid_entries += 1
            
            print(f"Valid entries: {valid_entries}")
            print(f"Invalid entries: {invalid_entries}")
            
            if valid_entries > 0 and invalid_entries == 0:
                return True, errors, warnings
            elif valid_entries > invalid_entries:
                return True, errors, warnings  # Mostly valid
            else:
                return False, errors + ["Too many invalid entries"], warnings
    else:
        # Basic file validation
        file_size = os.path.getsize(mul_path)
        if file_size == 0:
            return False, ["File is empty"], []
        
        # Check if file has reasonable size
        if file_size < 12:
            return False, ["File too small to contain valid multi data"], []
        
        return True, errors, warnings

def validate_map_file(mul_path):
    """Validate a map*.mul file"""
    errors = []
    warnings = []
    
    if not os.path.exists(mul_path):
        return False, ["File not found"], []
    
    file_size = os.path.getsize(mul_path)
    
    print(f"Map file: {os.path.basename(mul_path)}")
    print(f"File size: {file_size:,} bytes")
    
    # Map files should be multiples of 3 bytes (each tile is 3 bytes)
    if file_size % 3 != 0:
        warnings.append(f"File size ({file_size}) is not a multiple of 3")
    
    # Check first few tiles
    valid_tiles = 0
    invalid_tiles = 0
    
    with open(mul_path, 'rb') as f:
        for i in range(min(100, file_size // 3)):  # Check first 100 tiles
            tile_data = f.read(3)
            if len(tile_data) < 3:
                break
            
            tile_id = struct.unpack('<H', tile_data[:2])[0]
            z = struct.unpack('b', tile_data[2:3])[0]
            
            if tile_id < 0x4000 and -128 <= z <= 127:
                valid_tiles += 1
            else:
                invalid_tiles += 1
    
    print(f"Valid tiles: {valid_tiles}")
    print(f"Invalid tiles: {invalid_tiles}")
    
    if valid_tiles > 0 and invalid_tiles == 0:
        return True, errors, warnings
    elif valid_tiles > invalid_tiles:
        return True, errors, warnings
    else:
        return False, errors + ["Too many invalid tiles"], warnings

def validate_tiledata_file(mul_path):
    """Validate a tiledata.mul file"""
    errors = []
    warnings = []
    
    if not os.path.exists(mul_path):
        return False, ["File not found"], []
    
    file_size = os.path.getsize(mul_path)
    
    print(f"Tiledata file: {os.path.basename(mul_path)}")
    print(f"File size: {file_size:,} bytes")
    
    # Tiledata has known structure
    # Each land tile entry is 26 bytes (old) or 30 bytes (new)
    # Each static tile entry is 37 bytes (old) or 41 bytes (new)
    
    if file_size < 26:
        return False, ["File too small"], []
    
    # Basic validation - check if file has reasonable structure
    with open(mul_path, 'rb') as f:
        # Read first entry
        first_entry = f.read(26)
        if len(first_entry) < 26:
            return False, ["File too small for tiledata entry"], []
        
        # Check flags field (first 4 bytes)
        flags = struct.unpack('<I', first_entry[:4])[0]
        if flags > 0xFFFFFFFF:
            warnings.append("Suspicious flags value")
    
    return True, errors, warnings

def validate_file(mul_path, file_type=None, idx_path=None):
    """
    Validate a decrypted file
    
    Args:
        mul_path: Path to decrypted .mul file
        file_type: Type of file ('multi', 'map', 'tiledata', or None for auto-detect)
        idx_path: Path to .idx file (for multi files)
    
    Returns:
        Tuple of (is_valid, errors, warnings)
    """
    filename = os.path.basename(mul_path).lower()
    
    # Auto-detect file type
    if not file_type:
        if 'multi' in filename:
            file_type = 'multi'
        elif 'map' in filename:
            file_type = 'map'
        elif 'tiledata' in filename:
            file_type = 'tiledata'
        else:
            file_type = 'generic'
    
    print("="*60)
    print(f"Validating: {os.path.basename(mul_path)}")
    print(f"Type: {file_type}")
    print("="*60)
    
    if file_type == 'multi':
        return validate_multi_file(mul_path, idx_path)
    elif file_type == 'map':
        return validate_map_file(mul_path)
    elif file_type == 'tiledata':
        return validate_tiledata_file(mul_path)
    else:
        # Generic validation
        if not os.path.exists(mul_path):
            return False, ["File not found"], []
        file_size = os.path.getsize(mul_path)
        if file_size == 0:
            return False, ["File is empty"], []
        return True, [], []

def main():
    parser = argparse.ArgumentParser(description='Validate decrypted .sag/.mul files')
    parser.add_argument('file', help='Path to decrypted file to validate')
    parser.add_argument('--type', choices=['multi', 'map', 'tiledata'], help='File type (auto-detected if not specified)')
    parser.add_argument('--idx', help='Path to .idx file (for multi files)')
    
    args = parser.parse_args()
    
    try:
        is_valid, errors, warnings = validate_file(args.file, args.type, args.idx)
        
        print()
        if warnings:
            print("Warnings:")
            for warning in warnings:
                print(f"  - {warning}")
        
        if errors:
            print("Errors:")
            for error in errors:
                print(f"  - {error}")
        
        if is_valid and not errors:
            print("✓ File appears to be valid!")
            sys.exit(0)
        elif is_valid:
            print("⚠ File has warnings but may be usable")
            sys.exit(0)
        else:
            print("✗ File validation failed")
            sys.exit(1)
    
    except Exception as e:
        print(f"Error: {e}", file=sys.stderr)
        sys.exit(1)

if __name__ == "__main__":
    main()
