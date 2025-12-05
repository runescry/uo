"""
Find the AnimID for hair items in UO tiledata.mul
Based on ClassicUO's TileDataLoader.cs parsing
"""
import struct
from pathlib import Path

UO_CLIENT_PATH = r"C:\Program Files (x86)\Electronic Arts\Ultima Online Classic"

def read_tiledata_item(tiledata_path, item_id):
    """Read item data from tiledata.mul - matching ClassicUO's format"""
    with open(tiledata_path, 'rb') as f:
        f.seek(0, 2)
        file_size = f.tell()
        f.seek(0)

        # Determine if old or new format (CV_7090+)
        # Land: 512 groups * (4 header + 32 * land_entry_size)
        # Old land entry: 4 flags + 2 textId + 20 name = 26 bytes
        # New land entry: 8 flags + 2 textId + 20 name = 30 bytes (but ClassicUO uses same struct)

        # Actually ClassicUO checks FileManager.Version < ClientVersion.CV_7090
        # For old: flags = 4 bytes, for new: flags = 8 bytes

        # Let's detect by file size
        # Old format: land = 512 * (4 + 32*26) = 512 * 836 = 428032
        #            statics = groups * (4 + 32*37)
        # New format: land = 512 * (4 + 32*30) = 512 * 964 = 493568
        #            statics = groups * (4 + 32*41)

        # Calculate land section size for both formats
        land_old = 512 * (4 + 32 * 26)  # 428032
        land_new = 512 * (4 + 32 * 30)  # 493568

        # Static entry sizes
        static_old = 37  # 4 flags + rest
        static_new = 41  # 8 flags + rest

        # Test which format by checking if positions make sense
        # Try new format first (more common)
        is_old = False

        # Land takes 512 groups
        land_size = land_new  # Try new format
        static_entry_size = static_new

        # Calculate item offset
        # Items are in groups of 32, each group has 4-byte header
        item_group = item_id // 32
        item_in_group = item_id % 32

        item_offset = land_size + item_group * (4 + 32 * static_entry_size) + 4 + item_in_group * static_entry_size

        if item_offset + static_entry_size > file_size:
            print(f"Offset {item_offset} exceeds file size {file_size}, trying old format...")
            land_size = land_old
            static_entry_size = static_old
            item_offset = land_size + item_group * (4 + 32 * static_entry_size) + 4 + item_in_group * static_entry_size
            is_old = True

        print(f"Item {item_id} (0x{item_id:04X}) at offset {item_offset}, format={'old' if is_old else 'new'}")

        f.seek(item_offset)
        data = f.read(static_entry_size)

        if len(data) < static_entry_size:
            print("Not enough data!")
            return None

        # Parse based on ClassicUO format:
        # flags: 4/8 bytes (uint/ulong)
        # weight: 1 byte
        # layer: 1 byte
        # count: 4 bytes (int)
        # animId: 2 bytes (ushort)
        # hue: 2 bytes
        # lightIndex: 2 bytes
        # height: 1 byte
        # name: 20 bytes

        if is_old:
            flags = struct.unpack('<I', data[0:4])[0]
            weight = data[4]
            layer = data[5]
            count = struct.unpack('<i', data[6:10])[0]
            anim_id = struct.unpack('<H', data[10:12])[0]
            hue = struct.unpack('<H', data[12:14])[0]
            light_index = struct.unpack('<H', data[14:16])[0]
            height = data[16]
            name = data[17:37].decode('latin-1').rstrip('\x00')
        else:
            flags = struct.unpack('<Q', data[0:8])[0]
            weight = data[8]
            layer = data[9]
            count = struct.unpack('<i', data[10:14])[0]
            anim_id = struct.unpack('<H', data[14:16])[0]
            hue = struct.unpack('<H', data[16:18])[0]
            light_index = struct.unpack('<H', data[18:20])[0]
            height = data[20]
            name = data[21:41].decode('latin-1').rstrip('\x00')

        return {
            'flags': flags,
            'weight': weight,
            'layer': layer,
            'count': count,
            'anim_id': anim_id,
            'hue': hue,
            'light_index': light_index,
            'height': height,
            'name': name
        }

def main():
    tiledata_path = Path(UO_CLIENT_PATH) / "tiledata.mul"

    if not tiledata_path.exists():
        print(f"tiledata.mul not found at {tiledata_path}")
        return

    # Hair item IDs to check
    hair_items = {
        0x203B: "Short Hair",
        0x203C: "Long Hair",
        0x203D: "Ponytail",
        0x2044: "Mohawk",
        0x2045: "Pageboy",
        0x2046: "Buns",
        0x2047: "Afro",
        0x2048: "Receding",
        0x2049: "Two Pig Tails",
        0x204A: "Topknot",
    }

    print("=" * 60)
    print("Hair AnimID Finder")
    print("=" * 60)

    for item_id, name in hair_items.items():
        print(f"\n--- {name} (0x{item_id:04X}) ---")
        item_data = read_tiledata_item(tiledata_path, item_id)
        if item_data:
            print(f"  Name in file: '{item_data['name']}'")
            print(f"  AnimID: {item_data['anim_id']} (0x{item_data['anim_id']:04X})")
            print(f"  Layer: {item_data['layer']}")
            print(f"  Flags: 0x{item_data['flags']:016X}")

if __name__ == "__main__":
    main()
