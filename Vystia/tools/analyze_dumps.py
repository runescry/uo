"""
Analyze memory dumps for tiledata patterns and UO strings.
"""
import os
import glob

DUMPS_DIR = r"D:\UO\Vystia\tools\memory_dumps"

def analyze_file(filepath):
    """Analyze a single dump file."""
    with open(filepath, 'rb') as f:
        data = f.read()

    filename = os.path.basename(filepath)
    print(f"\n{'='*60}")
    print(f"File: {filename}")
    print(f"Size: {len(data):,} bytes ({len(data)//1024}KB)")

    # Search for UO-related strings
    uo_strings = [
        b'tiledata', b'TileData', b'.mul', b'.sag', b'.uop',
        b'UNUSED', b'gump', b'Gump', b'skill', b'Skill',
        b'anim', b'Anim', b'sound', b'Sound', b'map',
        b'land', b'Land', b'static', b'Static',
        b'UOFileSag', b'DecryptAES', b'AES256',
        b'tile', b'Tile', b'art', b'Art',
    ]

    found_strings = []
    for s in uo_strings:
        idx = data.find(s)
        if idx != -1:
            # Get context
            start = max(0, idx - 10)
            end = min(len(data), idx + len(s) + 30)
            context = data[start:end]
            found_strings.append((s.decode(errors='ignore'), idx, context))

    if found_strings:
        print("\nUO-related strings found:")
        for name, offset, context in found_strings[:10]:
            # Try to make context readable
            readable = ''.join(chr(b) if 32 <= b < 127 else '.' for b in context)
            print(f"  '{name}' at offset {offset}: ...{readable}...")

    # Check for ASCII names (tile names in tiledata are ASCII strings)
    ascii_regions = []
    i = 0
    while i < len(data) - 20:
        # Look for sequences of printable ASCII followed by nulls
        seq = data[i:i+20]
        printable = sum(1 for b in seq if 32 <= b < 127)
        nulls = seq.count(0)

        if printable > 10 and nulls > 3:
            try:
                text = seq.decode('ascii', errors='ignore').strip('\x00').strip()
                if len(text) > 4 and text.isalpha():
                    ascii_regions.append((i, text))
            except:
                pass
        i += 16  # Skip ahead

    if ascii_regions:
        print(f"\nASCII text regions found ({len(ascii_regions)} total):")
        for offset, text in ascii_regions[:20]:
            print(f"  Offset {offset}: '{text}'")

    # Look for tiledata structure patterns
    # Each land tile: 4 bytes flags, 2 bytes texture, 20 bytes name
    # Each static tile: 4 bytes flags, 1 byte weight, 1 byte quality, etc.

    # Try to detect repeating structure
    if len(data) > 1000:
        # Look for patterns at common tiledata offsets
        for struct_size in [26, 30, 37, 41]:  # Various tiledata entry sizes
            matches = 0
            for offset in range(0, min(len(data)-struct_size*10, 10000), struct_size):
                entry = data[offset:offset+struct_size]
                # Check if it looks like a tile entry
                # - First 4 bytes are flags (usually small)
                # - Has some ASCII in name field
                flags = int.from_bytes(entry[:4], 'little')
                if flags < 0x100000:  # Reasonable flags
                    name_area = entry[6:26] if struct_size >= 26 else entry[6:min(struct_size, 26)]
                    printable = sum(1 for b in name_area if 32 <= b < 127 or b == 0)
                    if printable > len(name_area) // 2:
                        matches += 1
            if matches > 50:
                print(f"\nPossible tiledata structure detected!")
                print(f"  Entry size: {struct_size} bytes")
                print(f"  Pattern matches: {matches}")

    # Check first 256 bytes hex dump
    print("\nFirst 128 bytes (hex):")
    for i in range(0, min(128, len(data)), 16):
        hex_part = ' '.join(f'{b:02x}' for b in data[i:i+16])
        ascii_part = ''.join(chr(b) if 32 <= b < 127 else '.' for b in data[i:i+16])
        print(f"  {i:04x}: {hex_part}  {ascii_part}")

def main():
    print("Analyzing memory dumps for UO data patterns")

    # Analyze dumps with potential UO content
    priority_files = [
        'region_074_0x7ffca44df000_IMAGE.bin',  # Has UO strings
    ]

    for pf in priority_files:
        path = os.path.join(DUMPS_DIR, pf)
        if os.path.exists(path):
            analyze_file(path)

    # Check the 3.2MB region that might be tiledata-sized
    # It wasn't dumped because it didn't have UNUSED, but let's analyze the closest one
    print("\n" + "="*60)
    print("Searching all dumps for any tiledata-like content...")

    for filepath in glob.glob(os.path.join(DUMPS_DIR, '*.bin')):
        with open(filepath, 'rb') as f:
            data = f.read()

        # Check for land tile patterns
        # Standard tiledata has specific structure
        if b'UNUSED' in data or b'grass' in data.lower() or b'dirt' in data.lower():
            print(f"\n*** {os.path.basename(filepath)} may contain tiledata! ***")
            idx = data.find(b'UNUSED') if b'UNUSED' in data else data.lower().find(b'grass')
            print(f"  Pattern at offset: {idx}")
            print(f"  Context: {data[max(0,idx-20):idx+40]}")

if __name__ == "__main__":
    main()
