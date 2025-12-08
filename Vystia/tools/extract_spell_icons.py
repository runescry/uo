"""
UO Spell Icon Extractor
Extracts spell icons from UO artLegacyMUL.uop file and saves as PNG images
"""

import struct
import os
import zlib
import hashlib
from PIL import Image

class UOPFile:
    """Parser for UO .uop (Mythic Package) files"""

    def __init__(self, filepath):
        self.filepath = filepath
        self.entries = {}
        self._parse()

    def _hash_filename(self, filename):
        """Hash filename to match UOP index"""
        filename = filename.lower()
        hash_val = hashlib.md5(filename.encode('utf-8')).digest()
        return struct.unpack('<Q', hash_val[:8])[0]

    def _parse(self):
        """Parse UOP file structure"""
        with open(self.filepath, 'rb') as f:
            # Read header (28 bytes)
            header = f.read(28)
            if len(header) < 28:
                raise ValueError("Invalid UOP header")

            # UOP header: magic(4) + version(4) + signature(4) + next_block(8) + block_size(4) + file_count(4) = 28 bytes
            magic, version, signature, next_block, block_size, file_count = struct.unpack('<IIIQII', header)

            if magic != 0x50594D:  # 'MYP' signature
                raise ValueError("Not a valid UOP file")

            # Read all blocks
            while next_block != 0:
                f.seek(next_block)
                block_header = f.read(12)
                file_count, next_block = struct.unpack('<IQ', block_header)

                # Read file entries in this block
                for i in range(file_count):
                    entry_data = f.read(34)
                    if len(entry_data) < 34:
                        break

                    # UOP entry: offset(8) + header_size(4) + compressed(4) + decompressed(4) + hash(8) + crc(4) + compression(2) = 34 bytes
                    file_offset, header_size, compressed_size, decompressed_size, file_hash, crc, compression = struct.unpack('<QIIIIQIH', entry_data)

                    if file_offset == 0:
                        continue

                    self.entries[file_hash] = {
                        'offset': file_offset + header_size,
                        'compressed_size': compressed_size,
                        'decompressed_size': decompressed_size,
                        'compression': compression
                    }

    def get_entry(self, art_id):
        """Get data for specific art ID"""
        # Art files are stored as "build/artlegacymul/######.tga"
        filename = f"build/artlegacymul/{art_id:06d}.tga"
        file_hash = self._hash_filename(filename)

        if file_hash not in self.entries:
            return None

        entry = self.entries[file_hash]

        with open(self.filepath, 'rb') as f:
            f.seek(entry['offset'])
            data = f.read(entry['compressed_size'])

            # Decompress if needed
            if entry['compression'] == 1:  # ZLib compression
                data = zlib.decompress(data)

            return data


class UOArtExtractor:
    def __init__(self, uo_path):
        self.uo_path = uo_path
        self.uop_path = os.path.join(uo_path, "artLegacyMUL.uop")

        if not os.path.exists(self.uop_path):
            raise FileNotFoundError(f"artLegacyMUL.uop not found at: {self.uop_path}")

        print(f"Loading UOP file: {self.uop_path}")
        self.uop = UOPFile(self.uop_path)
        print(f"Loaded {len(self.uop.entries)} entries from UOP file")

    def extract_art(self, art_id):
        """Extract art data and convert to PIL Image"""
        data = self.uop.get_entry(art_id)

        if not data:
            print(f"Art ID {art_id} not found in UOP file")
            return None

        try:
            # Parse TGA header (18 bytes minimum)
            if len(data) < 18:
                print(f"Art ID {art_id}: Invalid TGA data (too short)")
                return None

            id_length = data[0]
            color_map_type = data[1]
            image_type = data[2]

            # Read dimensions (little-endian)
            width = struct.unpack('<H', data[12:14])[0]
            height = struct.unpack('<H', data[14:16])[0]
            bpp = data[16]

            print(f"Art ID {art_id}: {width}x{height} pixels, {bpp}-bit TGA")

            # Skip header and any ID field
            offset = 18 + id_length

            # Handle color map if present
            if color_map_type == 1:
                color_map_start = struct.unpack('<H', data[3:5])[0]
                color_map_length = struct.unpack('<H', data[5:7])[0]
                color_map_depth = data[7]
                color_map_size = color_map_length * (color_map_depth // 8)
                offset += color_map_size

            # Create image
            img = Image.new('RGBA', (width, height), (0, 0, 0, 0))
            pixels = img.load()

            # Read pixel data based on format
            if image_type == 2:  # Uncompressed RGB
                if bpp == 16:
                    # 16-bit color (5-5-5-1 or 5-6-5)
                    for y in range(height - 1, -1, -1):  # TGA is bottom-up
                        for x in range(width):
                            if offset + 2 > len(data):
                                break

                            color16 = struct.unpack('<H', data[offset:offset+2])[0]
                            offset += 2

                            # Convert 16-bit A1R5G5B5 to RGBA
                            a = 255 if (color16 & 0x8000) else 0
                            r = ((color16 >> 10) & 0x1F) * 8
                            g = ((color16 >> 5) & 0x1F) * 8
                            b = (color16 & 0x1F) * 8

                            pixels[x, y] = (r, g, b, a)

                elif bpp == 32:
                    # 32-bit BGRA
                    for y in range(height - 1, -1, -1):
                        for x in range(width):
                            if offset + 4 > len(data):
                                break

                            b, g, r, a = data[offset:offset+4]
                            offset += 4
                            pixels[x, y] = (r, g, b, a)

            elif image_type == 10:  # RLE compressed RGB
                x, y = 0, height - 1

                while y >= 0 and offset < len(data):
                    packet_header = data[offset]
                    offset += 1

                    if packet_header & 0x80:  # RLE packet
                        count = (packet_header & 0x7F) + 1

                        if bpp == 16:
                            color16 = struct.unpack('<H', data[offset:offset+2])[0]
                            offset += 2
                            a = 255 if (color16 & 0x8000) else 0
                            r = ((color16 >> 10) & 0x1F) * 8
                            g = ((color16 >> 5) & 0x1F) * 8
                            b = (color16 & 0x1F) * 8
                            color = (r, g, b, a)
                        elif bpp == 32:
                            b, g, r, a = data[offset:offset+4]
                            offset += 4
                            color = (r, g, b, a)

                        for i in range(count):
                            if x < width and y >= 0:
                                pixels[x, y] = color
                                x += 1
                                if x >= width:
                                    x = 0
                                    y -= 1

                    else:  # Raw packet
                        count = (packet_header & 0x7F) + 1

                        for i in range(count):
                            if bpp == 16:
                                color16 = struct.unpack('<H', data[offset:offset+2])[0]
                                offset += 2
                                a = 255 if (color16 & 0x8000) else 0
                                r = ((color16 >> 10) & 0x1F) * 8
                                g = ((color16 >> 5) & 0x1F) * 8
                                b = (color16 & 0x1F) * 8
                                color = (r, g, b, a)
                            elif bpp == 32:
                                b, g, r, a = data[offset:offset+4]
                                offset += 4
                                color = (r, g, b, a)

                            if x < width and y >= 0:
                                pixels[x, y] = color
                                x += 1
                                if x >= width:
                                    x = 0
                                    y -= 1

            return img

        except Exception as e:
            print(f"Art ID {art_id}: Error parsing TGA: {e}")
            return None

    def extract_spell_icons(self, start_id=2240, end_id=2303, output_dir="spell_icons"):
        """Extract all spell icons and save as PNG"""
        os.makedirs(output_dir, exist_ok=True)

        extracted = 0
        failed = 0

        for art_id in range(start_id, end_id + 1):
            try:
                img = self.extract_art(art_id)
                if img:
                    output_path = os.path.join(output_dir, f"spell_icon_{art_id}.png")
                    img.save(output_path)
                    print(f"[OK] Saved: {output_path}")
                    extracted += 1
                else:
                    print(f"[FAIL] Could not extract art ID {art_id}")
                    failed += 1
            except Exception as e:
                print(f"[ERROR] Art ID {art_id}: {e}")
                failed += 1

        print(f"\n=== Extraction Complete ===")
        print(f"Extracted: {extracted}")
        print(f"Failed: {failed}")
        print(f"Output directory: {output_dir}")


if __name__ == "__main__":
    # UO client path
    uo_path = r"C:\Program Files (x86)\Electronic Arts\Ultima Online Classic"

    try:
        # Create extractor
        extractor = UOArtExtractor(uo_path)

        # Start with sample extraction (2240-2245)
        print("\n=== SAMPLE EXTRACTION (2240-2245) ===\n")
        extractor.extract_spell_icons(start_id=2240, end_id=2245, output_dir="spell_icons_sample")

        print("\n" + "="*50)
        print("Check the 'spell_icons_sample' folder to verify the icons.")
        print("If they look correct, uncomment the line below to extract all 64 icons.")
        print("="*50)

        # Uncomment this to extract all spell icons (2240-2303)
        # print("\n=== FULL EXTRACTION (2240-2303) ===\n")
        # extractor.extract_spell_icons(start_id=2240, end_id=2303, output_dir="spell_icons_full")

    except Exception as e:
        print(f"Fatal error: {e}")
        import traceback
        traceback.print_exc()
