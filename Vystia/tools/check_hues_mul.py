"""
Check if hues.mul is standard unencrypted format.
"""
sagas_hues = r"D:\Ultima Online - Sagas\UOData\hues.mul"
standard_hues = r"C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\hues.mul"

def analyze_hues(path, name):
    print(f"\n{name}:")
    try:
        with open(path, 'rb') as f:
            data = f.read(256)
        print(f"  Size check: {len(data)} bytes read")
        print(f"  First 32 bytes: {data[:32].hex()}")

        # Standard hues.mul structure:
        # Each hue group: 4 bytes header + 32 hues * 88 bytes
        # First 4 bytes might be header or first color

        # Check if it looks like color data (16-bit values)
        colors = []
        for i in range(0, 64, 2):
            color = int.from_bytes(data[i:i+2], 'little')
            colors.append(color)
        print(f"  First 32 color values: {colors}")

        # Check for potential encryption (high entropy = encrypted)
        unique_bytes = len(set(data[:256]))
        print(f"  Unique bytes in first 256: {unique_bytes}/256")
        if unique_bytes > 200:
            print(f"  *** Likely ENCRYPTED (high entropy)")
        else:
            print(f"  *** Likely UNENCRYPTED (normal entropy)")

    except FileNotFoundError:
        print(f"  File not found")

analyze_hues(sagas_hues, "Sagas hues.mul")
analyze_hues(standard_hues, "Standard hues.mul")
