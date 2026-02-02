# .SAG File Decryption Guide

## Overview

This guide explains how to decrypt `.sag` files from Ultima Online Sagas and convert them to `.mul` format for use with UO Fiddler and CentrED.

## Prerequisites

- Python 3.11+
- `pycryptodome` library (install with `pip install pycryptodome`)
- Access to `.sag` files in `D:\Ultima Online - Sagas\UOData\`
- Encryption key and IV (32-byte key, 16-byte IV)

## Current Status

**Encryption Method**: AES-256-CTR  
**File Handler**: `UOFileSag` class in Sagas ClassicUO.exe  
**Key/IV Status**: Not yet found - requires further reverse engineering

## Tools Available

### 1. `decrypt_sag.py` - Main Decryption Tool

Decrypts `.sag` files using AES-256-CTR.

**Usage:**
```bash
python decrypt_sag.py <sag_file> --key <32-byte-hex-key> --iv <16-byte-hex-iv> [options]
```

**Examples:**
```bash
# Decrypt entire file
python decrypt_sag.py "D:\Ultima Online - Sagas\UOData\multi.sag" \
    --key "a4ba3ccb76938ff1c186fabd56834580bfcc61c9e3ffa2541214e42712321f59" \
    --iv "22d47ed7244149740000000000000000" \
    --output "multi.mul"

# Decrypt specific entry using index
python decrypt_sag.py "D:\Ultima Online - Sagas\UOData\multi.sag" \
    --key "a4ba3ccb76938ff1c186fabd56834580bfcc61c9e3ffa2541214e42712321f59" \
    --iv "22d47ed7244149740000000000000000" \
    --idx "D:\Ultima Online - Sagas\UOData\multi.idx" \
    --entry 0 \
    --output "multi_entry_0.bin"
```

**Options:**
- `--key`: 32-byte key as hex string (64 hex characters) or path to key file
- `--iv`: 16-byte IV as hex string (32 hex characters) or path to IV file
- `--idx`: Path to .idx file (required for `--entry`)
- `--entry`: Entry ID to decrypt (requires `--idx`)
- `--output`: Output file path (default: .sag -> .mul)
- `--chunk-size`: Chunk size for large files (default: 1MB)

### 2. `sag_to_mul.py` - Conversion Tool

Converts decrypted `.sag` files to `.mul` format and handles `.idx` files.

**Usage:**
```bash
# Single file
python sag_to_mul.py <decrypted_file> [--output <output.mul>]

# Batch conversion
python sag_to_mul.py <directory> --batch [--output <output_dir>]
```

**Examples:**
```bash
# Convert single file
python sag_to_mul.py "multi.sag.decrypted" --output "multi.mul"

# Batch convert all .sag files in directory
python sag_to_mul.py "D:\Ultima Online - Sagas\UOData" --batch --pattern "*.sag"
```

**Options:**
- `--output`: Output file or directory
- `--batch`: Enable batch mode for directories
- `--pattern`: File pattern for batch mode (default: `*.sag`)
- `--no-idx`: Don't copy .idx files

### 3. `validate_decrypted.py` - Validation Tool

Validates decrypted files to ensure they match expected `.mul` format.

**Usage:**
```bash
python validate_decrypted.py <file> [--type <multi|map|tiledata>] [--idx <index_file>]
```

**Examples:**
```bash
# Validate multi file
python validate_decrypted.py "multi.mul" --type multi --idx "multi.idx"

# Validate map file
python validate_decrypted.py "map0.mul" --type map

# Auto-detect type
python validate_decrypted.py "tiledata.mul"
```

**Options:**
- `--type`: File type (`multi`, `map`, `tiledata`, or auto-detect)
- `--idx`: Path to .idx file (for multi files)

### 4. `try_aes256_ctr_decrypt.py` - Pattern Testing

Tests various key/IV patterns to find the correct decryption parameters.

**Usage:**
```bash
python try_aes256_ctr_decrypt.py
```

This script automatically tests common patterns on `multi.sag`, `tiledata.sag`, and `map0.sag`.

### 5. Analysis Tools

- `find_hardcoded_keys.py`: Searches binary for hardcoded keys/IVs
- `check_config_for_keys.py`: Checks config files, registry, and environment variables
- `find_uofilesag.py`: Analyzes UOFileSag class references

## Workflow

### Step 1: Find the Key/IV

The encryption key and IV must be discovered through reverse engineering. Options:

1. **Pattern Testing**: Run `try_aes256_ctr_decrypt.py` to test common patterns
2. **Binary Analysis**: Use `find_hardcoded_keys.py` to search the executable
3. **Config Analysis**: Use `check_config_for_keys.py` to check config files
4. **Runtime Analysis**: Capture decrypted data from memory while Sagas client is running
5. **Decompile**: Use ILSpy/dnSpy to decompile Sagas ClassicUO.exe

### Step 2: Decrypt Files

Once you have the key/IV:

```bash
# Decrypt a file
python decrypt_sag.py "multi.sag" \
    --key "<your-32-byte-hex-key>" \
    --iv "<your-16-byte-hex-iv>" \
    --output "multi.mul"
```

### Step 3: Convert to .mul Format

If the decrypted file needs conversion:

```bash
python sag_to_mul.py "multi.sag.decrypted" --output "multi.mul"
```

### Step 4: Validate

Validate the decrypted file:

```bash
python validate_decrypted.py "multi.mul" --type multi --idx "multi.idx"
```

### Step 5: Use with UO Tools

**UO Fiddler:**
1. Open UO Fiddler
2. Go to Settings → Path Settings
3. Set the root path to directory containing decrypted `.mul` files
4. UO Fiddler will now read the decrypted files

**CentrED:**
1. Edit `Cedserver.xml` configuration file
2. Update file paths to point to decrypted `.mul` files:
   ```xml
   <Map>D:\path\to\decrypted\map0.mul</Map>
   <Statics>D:\path\to\decrypted\statics0.mul</Statics>
   <StaIdx>D:\path\to\decrypted\staidx0.mul</StaIdx>
   ```
3. Restart CentrED server

## File Types

### Supported File Types

- `map*.sag` → `map*.mul` (map terrain data)
- `multi.sag` → `multi.mul` (multi structures)
- `art.sag` → `art.mul` (artwork)
- `tiledata.sag` → `tiledata.mul` (tile definitions)
- `statics*.sag` → `statics*.mul` (static items)
- `gumpart.sag` → `gumpart.mul` (gump graphics)
- Other `.sag` variants

### Index Files

`.idx` files are **not encrypted** and work with standard UO tools. They just need to be copied alongside the decrypted `.mul` files.

## Troubleshooting

### "Key must be 32 bytes"
- Ensure key is exactly 64 hex characters (32 bytes)
- Remove spaces, dashes, or other separators

### "IV must be 16 bytes"
- Ensure IV is exactly 32 hex characters (16 bytes)
- Remove spaces, dashes, or other separators

### "File validation failed"
- Key/IV may be incorrect
- File may be corrupted
- Try validating with different file type

### "Too many invalid entries"
- Key/IV is likely incorrect
- Try different key/IV combinations
- Verify you're using the correct key/IV for the file type

## Next Steps

1. **Find the Key/IV**: Continue reverse engineering to discover the encryption parameters
2. **Test Decryption**: Once key/IV is found, test on small files first
3. **Batch Process**: Use batch conversion for all files
4. **Validate**: Validate all decrypted files before use
5. **Backup**: Always keep original `.sag` files as backup

## References

- `SAG_FILE_ANALYSIS.md` - Initial file format analysis
- `SAG_TOOLS_COMPATIBILITY.md` - UO Fiddler/CentrED compatibility info
- `SAG_REVERSE_ENGINEERING_FINDINGS.md` - Detailed reverse engineering findings
- `SAG_DECRYPTION_PROGRESS.md` - Current progress and status
