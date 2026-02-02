# .SAG File Tools - Quick Reference

## Tools Overview

This directory contains tools for decrypting and working with `.sag` files from Ultima Online Sagas.

## Quick Start

1. **Install dependencies:**
   ```bash
   pip install pycryptodome
   ```

2. **Find the encryption key/IV** (not yet discovered - see analysis tools)

3. **Decrypt a file:**
   ```bash
   python decrypt_sag.py <file.sag> --key <hex-key> --iv <hex-iv>
   ```

4. **Convert to .mul:**
   ```bash
   python sag_to_mul.py <decrypted_file>
   ```

5. **Validate:**
   ```bash
   python validate_decrypted.py <file.mul> --type <multi|map|tiledata>
   ```

## Tool Categories

### Decryption Tools
- `decrypt_sag.py` - Main decryption tool
- `try_aes256_ctr_decrypt.py` - Pattern testing

### Conversion Tools
- `sag_to_mul.py` - Convert decrypted files to .mul format

### Validation Tools
- `validate_decrypted.py` - Validate decrypted files

### Analysis Tools
- `find_hardcoded_keys.py` - Search binary for keys/IVs
- `check_config_for_keys.py` - Check config files for keys
- `find_uofilesag.py` - Analyze UOFileSag class
- `analyze_sagas_exe.py` - General binary analysis
- `extract_strings_from_exe.py` - String extraction

### Documentation
- `SAG_DECRYPTION_GUIDE.md` - Complete user guide
- `SAG_FILE_ANALYSIS.md` - File format analysis
- `SAG_TOOLS_COMPATIBILITY.md` - UO Fiddler/CentrED compatibility
- `SAG_REVERSE_ENGINEERING_FINDINGS.md` - Reverse engineering details
- `SAG_DECRYPTION_PROGRESS.md` - Current status

## File Structure

```
Vystia/tools/
├── decrypt_sag.py                    # Main decryption tool
├── sag_to_mul.py                     # Conversion tool
├── validate_decrypted.py             # Validation tool
├── try_aes256_ctr_decrypt.py         # Pattern testing
├── find_hardcoded_keys.py            # Binary key search
├── check_config_for_keys.py          # Config checker
├── find_uofilesag.py                 # UOFileSag analysis
├── analyze_sagas_exe.py              # Binary analysis
├── extract_strings_from_exe.py       # String extraction
├── inspect_sag.py                    # File inspection
├── read_sag.py                       # Read attempts
├── test_sag_direct_read.py           # Direct read tests
└── *.md                              # Documentation files
```

## Status

**Ready for use once key/IV is discovered.**

All tools are implemented and tested. The encryption method (AES-256-CTR) and file structure are understood. The remaining step is finding the encryption key and IV through reverse engineering or runtime analysis.
