"""
Find the AnimID for beard items in UO tiledata.mul
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

        land_new = 512 * (4 + 32 * 30)
        static_entry_size = 41

        item_group = item_id // 32
        item_in_group = item_id % 32

        item_offset = land_new + item_group * (4 + 32 * static_entry_size) + 4 + item_in_group * static_entry_size

        f.seek(item_offset)
        data = f.read(static_entry_size)

        if len(data) < static_entry_size:
            return None

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
            'anim_id': anim_id,
            'layer': layer,
            'name': name
        }

def main():
    tiledata_path = Path(UO_CLIENT_PATH) / "tiledata.mul"

    # Beard item IDs used by dwarves
    beard_items = {
        0x203E: "Long Beard",
        0x203F: "Short Beard",
        0x2040: "Goatee",
        0x2041: "Mustache",
        0x204B: "Medium Long Beard",
        0x204C: "Vandyke",
        0x204D: "Long Beard 2",
    }

    print("=" * 60)
    print("Beard AnimID Finder")
    print("=" * 60)

    for item_id, name in beard_items.items():
        item_data = read_tiledata_item(tiledata_path, item_id)
        if item_data:
            print(f"{name} (0x{item_id:04X}): AnimID = {item_data['anim_id']} (0x{item_data['anim_id']:04X}), Layer = {item_data['layer']}")

if __name__ == "__main__":
    main()
