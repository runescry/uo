# Runtime Analysis Tools - Ready to Use

## Status: ✅ All Tools Ready

Both Frida and psutil are installed and ready. You can start extracting the key/IV immediately.

## Quick Start (Recommended)

### Option 1: Frida Hook (Best Choice)

**File**: `extract_key_with_frida.py`

**Usage:**
```bash
# Run the script (it will wait for ClassicUO to start)
python Vystia/tools/extract_key_with_frida.py

# Then launch Sagas ClassicUO and load a .sag file
# The key/IV will be automatically extracted
```

**What it does:**
- Hooks into Windows CryptoAPI (BCryptDecrypt, CryptDecrypt)
- Hooks into DecryptAES256_CTR function (if found)
- Monitors file I/O for .sag files
- Automatically saves extracted keys/IVs to `extracted_sag_keys.txt`

**Advantages:**
- Most precise method
- Captures key/IV at the moment of decryption
- Automatic extraction

### Option 2: Memory Dump (Fallback)

**File**: `simple_memory_dump.py`

**Usage:**
```bash
# 1. Start Sagas ClassicUO first
# 2. Load a .sag file in ClassicUO
# 3. Then run:
python Vystia/tools/simple_memory_dump.py
```

**What it does:**
- Scans entire process memory
- Looks for 32-byte sequences that could be keys
- Looks for 16-byte sequences that could be IVs
- Saves candidates to `extracted_keys.txt`

**Advantages:**
- Simple, no special setup
- Works even if API hooking fails
- May find keys in memory buffers

**Disadvantages:**
- May have false positives
- Less precise than hooking

## Step-by-Step: Extract Key/IV

### Using Frida (Recommended)

1. **Open terminal in `d:\UO`**

2. **Run the extraction script:**
   ```bash
   python Vystia/tools/extract_key_with_frida.py
   ```

3. **Launch Sagas ClassicUO:**
   - Navigate to `D:\Ultima Online - Sagas\ClassicUO\`
   - Run `ClassicUO.exe`
   - The Frida script will automatically attach

4. **Trigger .sag file loading:**
   - Connect to server (loads map files)
   - Or use any feature that accesses .sag files
   - Watch the console for key/IV extraction

5. **Check results:**
   - Keys/IVs printed to console
   - Saved to `extracted_sag_keys.txt` in current directory

6. **Test decryption:**
   ```bash
   python Vystia/tools/decrypt_sag.py "D:\Ultima Online - Sagas\UOData\multi.sag" \
       --key "<extracted-key-from-file>" \
       --iv "<extracted-iv-from-file>"
   ```

### Using Memory Dump (Alternative)

1. **Start Sagas ClassicUO** (let it fully load)

2. **Load a .sag file** (connect to server, view map, etc.)

3. **Run memory dump:**
   ```bash
   python Vystia/tools/simple_memory_dump.py
   ```

4. **Review results:**
   - Check `extracted_keys.txt`
   - Test promising candidates with `decrypt_sag.py`

## Expected Output

### Frida Output:
```
[+] Found ClassicUO module: ClassicUO.exe
[+] Hooked BCryptDecrypt
[+] Hooked DecryptAES256_CTR
[+] Reading .sag file: D:\...\multi.sag

[KEY FOUND] a4ba3ccb76938ff1c186fabd56834580bfcc61c9e3ffa2541214e42712321f59
[IV FOUND]  22d47ed7244149740000000000000000
```

### Memory Dump Output:
```
Found process ClassicUO.exe (PID: 12345)
Scanning process memory for encryption keys...
Found 15 potential keys in region 0x400000
...
Results saved to extracted_keys.txt
```

## Troubleshooting

### Frida Issues

**"Process not found"**
- Make sure ClassicUO.exe is running
- Check exact process name: `tasklist | findstr ClassicUO`

**"No keys extracted"**
- Make sure you actually load a .sag file
- Try connecting to server (loads map files)
- The decryption might use custom implementation, not Windows CryptoAPI
- Try the memory dump approach instead

**"Access denied"**
- Run PowerShell/terminal as Administrator
- Check Windows Defender exclusions

### Memory Dump Issues

**"Process not found"**
- Start ClassicUO first, then run the script

**"Too many false positives"**
- This is expected - test candidates manually
- Look for keys with entropy 4.0-7.0
- Test with `decrypt_sag.py` and validate results

## Next Steps After Extraction

1. **Test on small file:**
   ```bash
   python decrypt_sag.py multi.sag --key <key> --iv <iv> --entry 0 --idx multi.idx
   ```

2. **Validate:**
   ```bash
   python validate_decrypted.py multi.mul --type multi --idx multi.idx
   ```

3. **If valid, decrypt all files:**
   ```bash
   # Decrypt
   python decrypt_sag.py map0.sag --key <key> --iv <iv>
   
   # Convert
   python sag_to_mul.py map0.sag.decrypted
   ```

4. **Use with UO tools:**
   - Point UO Fiddler to decrypted .mul files
   - Update CentrED config to use decrypted files

## Files Created

- `extracted_sag_keys.txt` - Frida extraction results
- `extracted_keys.txt` - Memory dump results

## Support Tools

- `decrypt_sag.py` - Decrypt once key/IV found
- `sag_to_mul.py` - Convert to .mul format
- `validate_decrypted.py` - Verify decrypted files
- `QUICK_START_RUNTIME.md` - This file
