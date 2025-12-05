# BMP File Comparison Report

**Date**: 2025-10-26
**Purpose**: Identify why terrain_new.bmp and altitude_new.bmp fail while Terrain.bmp and Altitude.bmp work

---

## Critical Differences Found

### 1. **Color Mode - MOST CRITICAL**

| Attribute | Working Files | Non-Working Files | Impact |
|-----------|--------------|-------------------|---------|
| **bits_per_pixel** | **8-bit** | **24-bit** | ❌ CRITICAL |
| **Mode** | **P (Palette)** | **RGB (True Color)** | ❌ CRITICAL |
| **Pixel data offset** | 1078 bytes | 54 bytes | Header + palette vs header only |

**Finding**: The non-working files are **24-bit RGB true color** instead of **8-bit indexed palette mode**.

**Why this breaks UOMapMake**:
- UOMapMake expects 8-bit palette-indexed BMPs where each pixel value IS the terrain ID
- In 24-bit RGB mode, UOMapMake cannot map colors to terrain types
- The palette table is missing entirely (offset is 54 instead of 1078)

---

### 2. **Palette Index 0 Usage - CRITICAL**

| File Type | Working | Non-Working |
|-----------|---------|-------------|
| **Terrain.bmp** | Min value: **1** (never uses 0) | Min value: **0** (uses 0 extensively) |
| **Altitude.bmp** | Min value: **24** (never uses 0) | Min value: **0** (uses 0 extensively) |

**Pixel counts using index 0**:
- Working Terrain: **0 pixels**
- Non-working Terrain: **60,057,334 pixels** (68%)
- Working Altitude: **0 pixels**
- Non-working Altitude: **61,915,735 pixels** (71%)

**Why this matters**:
- Palette index 0 can cause rendering glitches in UO
- Working files carefully avoid index 0
- Terrain working file starts at index 1 (not 0)
- Altitude working file starts at index 24 (not 0)

---

### 3. **Dimensions**

| Attribute | Working Files | Non-Working Files |
|-----------|--------------|-------------------|
| Width | 2304 | 7168 |
| Height | 1600 | 4096 |
| Total pixels | 3,686,400 | 29,360,128 |

**Note**: This is expected - the working examples are smaller test maps, while the non-working files are full-size Map5 (8192×4096 equivalent in 7168×4096).

---

### 4. **File Size**

| File | Working | Non-Working | Difference |
|------|---------|-------------|------------|
| Terrain | 3.5 MB | 84 MB | 24× larger |
| Altitude | 3.5 MB | 84 MB | 24× larger |

**Why**: 24-bit RGB uses 3 bytes per pixel vs 1 byte per pixel for 8-bit palette.

---

## Detailed Header Comparison

### Working Files (Correct Format)

```
File format:       BMPv3 (40-byte DIB header)
Bits per pixel:    8 (palette mode)
Compression:       BI_RGB (uncompressed)
Pixel offset:      1078 bytes (14 header + 40 DIB + 1024 palette)
Color mode:        P (indexed palette)
Palette entries:   256 colors (stored at offset 54-1078)
```

### Non-Working Files (Incorrect Format)

```
File format:       BMPv3 (40-byte DIB header)
Bits per pixel:    24 (true color RGB)
Compression:       BI_RGB (uncompressed)
Pixel offset:      54 bytes (14 header + 40 DIB, NO PALETTE)
Color mode:        RGB (direct RGB values)
Palette entries:   NONE (not applicable to 24-bit)
```

---

## Pixel Data Analysis

### Working Terrain.bmp

```
First 20 pixel values: [5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5]
Unique values: 22 different terrain IDs
Value range: 1-255
Uses index 0: NO ✓
```

**Interpretation**:
- Each pixel stores a palette index (1-255)
- Index 5 = Deep Water terrain type
- Never uses index 0

### Non-Working terrain_new.bmp

```
First 20 bytes: [132, 0, 0, 132, 0, 0, 132, 0, 0, 132, 0, 0, 132, 0, 0, 132, 0, 0, 132, 0]
Pattern: Groups of 3 bytes (B, G, R)
Uses index 0: YES (60M pixels) ✗
```

**Interpretation**:
- Each pixel is 3 bytes: Blue, Green, Red
- First pixel: RGB(0, 0, 132) - a blue color
- UOMapMake cannot interpret RGB values as terrain IDs

---

## Working Altitude.bmp

```
First 20 pixel values: [24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24]
Unique values: 7 discrete elevation levels
Value range: 24-216
Uses index 0: NO ✓
```

**Interpretation**:
- Each pixel stores an elevation value (24-216)
- Index 24 = low elevation / coastal
- Index 216 = high elevation / mountains
- 7 discrete levels total

---

## Root Cause Analysis

### Why Non-Working Files Fail

1. **Wrong color mode**: Saved as 24-bit RGB instead of 8-bit palette
   - UOMapMake needs palette indices, not RGB values
   - Missing the 1024-byte palette table entirely

2. **Uses palette index 0**: Majority of pixels are black (0,0,0) = index 0
   - Working files carefully avoid index 0
   - This can cause UO rendering issues

3. **Color values don't match palette**: RGB values in 24-bit mode don't correspond to UO terrain definitions
   - Even if converted to palette mode, the colors wouldn't match Terrain.xml

---

## How to Fix

### Required Changes

1. **Convert to 8-bit palette mode**
   - Use `Image.convert('P', palette=custom_palette)`
   - Ensure palette table is written at offset 54-1078

2. **Build palette with exact UO colors**
   - Palette[1] = RGB(0, 100, 0) for Grass (ID 1)
   - Palette[5] = RGB(0, 0, 130) for Deep Water (ID 5)
   - Palette[12] = RGB(255, 255, 255) for Snow (ID 12)
   - etc.

3. **Never use palette index 0**
   - Start terrain IDs at 1 (lowest valid terrain)
   - Start altitude at 1 or higher
   - Leave palette[0] defined but unused

4. **Use manual BMP writer**
   - PIL/Pillow can corrupt palette order
   - Write BMP structure manually with struct.pack()
   - Ensure palette entries are in exact order

---

## Code Fix Applied

The working `scripts/50_write_uo_bitmaps.py` already implements these fixes:

```python
# Write BMPv3 manually with exact palette control
def write_bmp_v3_manual(data, palette, filepath, is_grayscale=False):
    h, w = data.shape

    # Build 256-entry palette
    palette_bytes = bytearray()
    for i in range(256):
        if i < len(palette):
            r, g, b = palette[i]
        else:
            r, g, b = 0, 0, 0
        palette_bytes.extend([b, g, r, 0])  # BGRA format

    # Write BMP with palette at offset 54
    # Pixel data starts at offset 1078 (54 + 1024)
    ...
```

**Key fixes**:
- Forces 8-bit palette mode
- Builds 256-color palette table
- Writes palette at correct offset
- Never uses index 0 in pixel data

---

## Validation Checklist

To verify a BMP will work with UOMapMake:

✅ **File format**: BMPv3 (40-byte BITMAPINFOHEADER)
✅ **Bits per pixel**: 8 (not 24)
✅ **Compression**: BI_RGB (0 = uncompressed)
✅ **Pixel offset**: 1078 bytes (indicates palette present)
✅ **Color mode**: P (palette/indexed)
✅ **Palette entries**: 256 colors defined
✅ **Palette index 0**: Not used in pixel data
✅ **Palette colors**: Match Terrain.xml exactly
✅ **Dimensions**: 8192×4096 for Map5

---

## Summary

**The non-working files fail because they are 24-bit RGB true color images instead of 8-bit palette-indexed BMPs.**

UOMapMake requires:
1. 8-bit palette mode (1 byte per pixel = terrain ID)
2. 256-color palette table with exact UO colors
3. No usage of palette index 0
4. Palette must match Terrain.xml definitions

The current pipeline (`50_write_uo_bitmaps.py`) correctly generates working files by:
- Writing BMPs manually with exact palette control
- Using 8-bit indexed mode
- Avoiding palette index 0
- Matching UO terrain colors exactly

---

*Generated by compare_bmps.py*
*Vystia Pipeline v2 - 2025-10-26*
