# .SAG File Format Analysis

## Summary

The `.sag` files in `D:\Ultima Online - Sagas\UOData\` appear to be **encrypted or obfuscated versions** of standard Ultima Online `.mul` files. They are used by the "Ultima Online - Sagas" client/server setup.

## Findings

### File Structure
- **High entropy** (~6.5/8.0) indicates encryption or compression
- Files are **NOT** in standard formats:
  - Not UOP format (doesn't start with `MYP\x00`)
  - Not ZIP/GZIP compressed
  - Not standard .mul format (data doesn't parse correctly)
  
### Index Files
- Standard `.idx` files exist and work normally (e.g., `multi.idx`, `anim.idx`)
- Index files use standard UO format: `offset(4) + length(4) + extra(4) = 12 bytes per entry`
- Index entries point to offsets in the `.sag` files

### Test Results
When attempting to read `.sag` files directly:
- **multi.sag**: Data at indexed offsets doesn't parse as valid multi component data
- **map0.sag**: Data doesn't parse as valid map tile data
- Values are clearly encrypted/obfuscated

### Attempted Decryption Methods
The following methods were tried without success:
- ZLib decompression (whole file and with header skip)
- GZip decompression
- Single-byte XOR decryption (common keys: 0x00, 0xFF, 0xAA, 0x55, etc.)
- Multi-byte XOR decryption (common patterns)

## Files Found

### Data Files (.sag)
- `map0.sag` through `map5.sag` (map data)
- `facet00.sag` through `facet05.sag` (facet data)
- `art.sag`, `artidx.sag` (artwork)
- `tiledata.sag` (tile definitions)
- `multi.sag` (multi structures)
- `statics0.sag` through `statics5.sag` (static items)
- `staidx0.sag` through `staidx5.sag` (static indexes)
- `gumpart.sag`, `gumpidx.sag` (gump graphics)
- `fonts.sag`, `unifont*.sag` (fonts)
- `anim*.sag` (animations)
- `sound.sag`, `soundidx.sag` (sounds)
- `hues.sag`, `light.sag`, `lightidx.sag`, `radarcol.sag` (colors/lighting)
- `texmaps.sag`, `texidx.sag` (textures)
- `skills.sag` (skills data)

### Index Files (.idx)
- `multi.idx`
- `anim.idx` through `anim5.idx`
- `Skills.idx`

## Possible Solutions

### 1. Reverse Engineer the Client
- The ClassicUO client in the Sagas directory might handle decryption
- Look for decryption code in:
  - `ClassicUO\ClassicUO.exe`
  - Client DLLs
  - Configuration files

### 2. Check for Decryption Tools
- Look for tools in the Sagas directory that might convert .sag to .mul
- Check launcher/updater executables for conversion utilities

### 3. Contact Sagas Developers
- If this is a custom shard/client, the developers may have tools or documentation

### 4. Memory Dump Analysis
- If the client decrypts files in memory, a memory dump while the client is running might reveal the decrypted data
- Use tools like Process Monitor to see file access patterns

### 5. Network Protocol Analysis
- If data is decrypted on-the-fly, network traffic analysis might reveal patterns

## Tools Created

1. **`inspect_sag.py`**: Analyzes .sag file headers and structure
2. **`read_sag.py`**: Attempts various decryption/decompression methods
3. **`test_sag_direct_read.py`**: Tests if files can be read directly as .mul files

## ClassicUO Client Analysis

### Findings
- **Sagas Client Location**: `D:\Ultima Online - Sagas\ClassicUO\ClassicUO.exe` (18.8 MB compiled executable)
- **Settings File**: Points to `..\\UOData` directory (which contains the .sag files)
- **Workspace ClassicUO**: Standard open-source version that only handles `.mul` and `.uop` files
- **No .sag Support in Source**: The workspace ClassicUO source code does not contain any `.sag` file handling

### Implications
The Sagas ClassicUO client is likely a **modified/custom build** that:
1. Either handles `.sag` file decryption internally
2. Or has been modified to look for `.sag` extensions instead of `.mul`
3. Or uses a plugin/extension system to handle `.sag` files

## Next Steps

### Immediate Actions
1. **Test if client can read .sag files**: Run the Sagas ClassicUO client and verify it successfully loads data from `.sag` files
2. **Memory dump analysis**: Use Process Monitor or similar tools to see what files the client actually opens
3. **Binary analysis**: Use tools like ILSpy or dnSpy to decompile the Sagas ClassicUO.exe and look for `.sag` handling code
4. **Check for plugins**: Look for DLL files in the Sagas directory that might handle file decryption

### Reverse Engineering Approach
1. **Decompile Sagas ClassicUO.exe** using:
   - ILSpy (for .NET decompilation)
   - dnSpy (for .NET debugging/decompilation)
   - IDA Pro or Ghidra (for deeper binary analysis)
2. **Search decompiled code** for:
   - `.sag` string references
   - Decryption/encryption functions
   - File reading code that differs from standard ClassicUO
3. **Hook file I/O**: Use API monitoring tools to intercept file reads and see if decryption happens

### Alternative Approaches
1. **Contact Sagas developers** for documentation or tools
2. **Check Sagas forums/documentation** for file format information
3. **Try renaming .sag to .mul** to see if they're just renamed files (unlikely given encryption evidence)

## Conclusion

The `.sag` files are encrypted/obfuscated and cannot be read with standard UO file reading methods. The Sagas ClassicUO client appears to be a modified build that handles these files, but the source code is not available in the workspace. 

**To read .sag files, we need to:**
- Reverse engineer the Sagas ClassicUO.exe binary to find the decryption method
- Or obtain the decryption code/tools from the Sagas developers
- Or use runtime analysis (memory dumps, API hooks) to capture decrypted data
