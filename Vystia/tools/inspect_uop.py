"""
Quick UOP file inspector to determine actual entry size
"""
import struct

uop_path = r"C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\artLegacyMUL.uop"

with open(uop_path, 'rb') as f:
    # Read header
    header = f.read(28)
    print(f"Header bytes: {len(header)}")
    print(f"Header hex: {header.hex()}")

    magic, version, signature, next_block, block_size, file_count = struct.unpack('<IIIQII', header)

    print(f"\nMagic: 0x{magic:X} (should be 0x50594D)")
    print(f"Version: {version}")
    print(f"Signature: 0x{signature:X}")
    print(f"Next block: {next_block}")
    print(f"Block size: {block_size}")
    print(f"File count: {file_count}")

    # Go to first block
    f.seek(next_block)
    block_header = f.read(12)
    block_file_count, block_next = struct.unpack('<IQ', block_header)

    print(f"\nFirst block:")
    print(f"File count in block: {block_file_count}")
    print(f"Next block: {block_next}")

    # Try different entry sizes
    print(f"\nTrying to read first entry...")
    entry_pos = f.tell()

    # Try 34 bytes
    f.seek(entry_pos)
    entry34 = f.read(34)
    print(f"\n34 bytes: {entry34.hex()}")

    # Try 32 bytes
    f.seek(entry_pos)
    entry32 = f.read(32)
    print(f"\n32 bytes: {entry32.hex()}")

    # Try parsing as different formats
    print(f"\nTrying format '<QIIIQHH' (32 bytes):")
    f.seek(entry_pos)
    data32 = f.read(32)
    try:
        vals = struct.unpack('<QIIIQHH', data32)
        print(f"Success: offset={vals[0]}, header={vals[1]}, compressed={vals[2]}, decompressed={vals[3]}, hash={vals[4]:016X}, crc={vals[5]:04X}, compression={vals[6]}")
    except Exception as e:
        print(f"Failed: {e}")

    print(f"\nTrying format '<QIIIQIH' (34 bytes - with 4-byte CRC):")
    f.seek(entry_pos)
    data34 = f.read(34)
    try:
        vals = struct.unpack('<QIIIQIH', data34)
        print(f"Success: offset={vals[0]}, header={vals[1]}, compressed={vals[2]}, decompressed={vals[3]}, hash={vals[4]:016X}, crc={vals[5]:08X}, compression={vals[6]}")
    except Exception as e:
        print(f"Failed: {e}")
