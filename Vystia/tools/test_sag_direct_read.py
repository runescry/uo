"""
Test if .sag files can be read directly as if they were .mul files
"""
import struct
import os

def test_multi_sag():
    """Test reading multi.sag as if it were multi.mul"""
    sag_path = r"D:\Ultima Online - Sagas\UOData\multi.sag"
    idx_path = r"D:\Ultima Online - Sagas\UOData\multi.idx"
    
    # Read index entry 0
    with open(idx_path, 'rb') as idx:
        idx.seek(0)
        offset, length, extra = struct.unpack('<III', idx.read(12))
        print(f"Index entry 0: offset={offset}, length={length}, extra={extra}")
    
    # Try reading from .sag file at that offset
    with open(sag_path, 'rb') as f:
        f.seek(offset)
        data = f.read(min(length, 608))  # Read first entry
        
        print(f"\nRead {len(data)} bytes from offset {offset}")
        print(f"First 64 bytes (hex): {data[:64].hex()}")
        
        # Try parsing as multi data (12 bytes per entry: tile_id(2) + x(2) + y(2) + z(2) + flags(4))
        print("\nTrying to parse as multi component entries:")
        entry_count = length // 12
        print(f"Expected entries: {entry_count}")
        
        for i in range(min(5, entry_count)):  # Show first 5 entries
            entry_data = data[i*12:(i+1)*12]
            if len(entry_data) == 12:
                tile_id, x, y, z, flags = struct.unpack('<HhhhI', entry_data)
                print(f"  Entry {i}: tile_id=0x{tile_id:04X} ({tile_id}), x={x}, y={y}, z={z}, flags=0x{flags:08X}")
                
                # Check if values look reasonable
                if 0 < tile_id < 0x4000 and -100 < x < 100 and -100 < y < 100 and -100 < z < 100:
                    print(f"    -> Looks like valid multi data!")
                else:
                    print(f"    -> Values look suspicious (might be encrypted)")

def test_map_sag():
    """Test reading map0.sag as if it were map0.mul"""
    sag_path = r"D:\Ultima Online - Sagas\UOData\map0.sag"
    
    # Map files are structured as blocks of 8x8 tiles
    # Each tile is 3 bytes: tile_id(2) + z(1)
    # Each block is 8*8*3 = 192 bytes
    
    print(f"\n{'='*60}")
    print("Testing map0.sag")
    print(f"{'='*60}")
    
    with open(sag_path, 'rb') as f:
        # Read first block (192 bytes)
        block_data = f.read(192)
        print(f"First block (192 bytes): {block_data.hex()[:96]}...")
        
        # Try parsing as map tiles
        print("\nTrying to parse as map tiles (3 bytes each):")
        for i in range(min(8, 64)):  # Show first 8 tiles
            tile_data = block_data[i*3:(i+1)*3]
            if len(tile_data) == 3:
                tile_id = struct.unpack('<H', tile_data[:2])[0]
                z = struct.unpack('b', tile_data[2:3])[0]  # signed byte
                print(f"  Tile {i}: tile_id=0x{tile_id:04X} ({tile_id}), z={z}")
                
                # Check if values look reasonable
                if tile_id < 0x4000 and -128 <= z <= 127:
                    print(f"    -> Looks like valid map data!")
                else:
                    print(f"    -> Values look suspicious")

if __name__ == "__main__":
    test_multi_sag()
    test_map_sag()
