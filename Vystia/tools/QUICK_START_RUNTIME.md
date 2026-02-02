# Quick Start: Extract Key/IV with Frida

## Prerequisites

✅ Frida is already installed on your system!

## Steps

### 1. Start the Extraction Script

```bash
python Vystia/tools/extract_key_with_frida.py
```

### 2. Launch Sagas ClassicUO

- The script will automatically attach when ClassicUO.exe starts
- Or if ClassicUO is already running, it will attach immediately

### 3. Load a .sag File

- In ClassicUO, do something that loads a .sag file:
  - Load a map (accesses map*.sag)
  - View a multi (accesses multi.sag)
  - Load artwork (accesses art.sag)

### 4. Check Results

- Keys/IVs will be printed to console
- Results saved to `extracted_sag_keys.txt`

### 5. Use the Key/IV

Once extracted:

```bash
python decrypt_sag.py "D:\Ultima Online - Sagas\UOData\multi.sag" \
    --key "<extracted-key>" \
    --iv "<extracted-iv>" \
    --output "multi.mul"
```

## Alternative: Simple Memory Dump

If Frida doesn't work, try the simpler approach:

```bash
# Start ClassicUO first, then:
python Vystia/tools/simple_memory_dump.py
```

This scans process memory for potential keys (may have false positives).

## Troubleshooting

**"ClassicUO.exe not found"**
- Make sure Sagas ClassicUO is running
- Check the exact process name in Task Manager

**"No keys extracted"**
- Make sure you actually load a .sag file in ClassicUO
- Try different file types (map, multi, art)
- The decryption might use different APIs than we're hooking

**"Access denied"**
- Run as Administrator
- Check Windows Defender/antivirus settings

## Next Steps After Extraction

1. Test decryption on a small file first
2. Validate the decrypted file
3. Batch decrypt all files
4. Convert to .mul format
5. Use with UO Fiddler/CentrED
