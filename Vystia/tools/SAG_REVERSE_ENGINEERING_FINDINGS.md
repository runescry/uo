# .SAG File Reverse Engineering Findings

## Key Discoveries

### 1. File Handler Class: `UOFileSag`
- **Location**: Found in Sagas ClassicUO.exe at offset `0x9bfba4`
- **Context**: Appears alongside `UOFile`, `UOFileIndex`, `UOFileLoader`
- **Implication**: There's a dedicated class for handling `.sag` files, likely a subclass or variant of `UOFile`

### 2. Encryption Method: AES-256-CTR
- **Method**: `DecryptAES256_CTR` found at offset `0xa3c494`
- **Related strings**: 
  - `DecryptAES256_CTR`
  - `DecryptOld`
  - `Decrypt`
  - `AES256_CTR`
- **Implication**: Files are encrypted using AES-256 in CTR (Counter) mode

### 3. File Structure
- Standard `.idx` files work normally (point to offsets in `.sag` files)
- `.sag` files contain encrypted data at those offsets
- File reading follows standard UO file structure, but data is encrypted

## What We Know

1. **Encryption Algorithm**: AES-256-CTR
2. **File Handler**: `UOFileSag` class exists
3. **File Format**: Encrypted version of standard `.mul` format
4. **Index Files**: Standard `.idx` format (not encrypted)

## What We Need to Find

1. **Encryption Key**: 32 bytes for AES-256
2. **Initialization Vector (IV)**: 16 bytes for AES-CTR
3. **Key Derivation**: How the key/IV are generated (might be hardcoded, derived from filename, or from a config file)
4. **File Extension Handling**: How `.mul` → `.sag` replacement works

## Next Steps

### Option 1: Runtime Analysis
- Run Sagas ClassicUO client
- Use Process Monitor to see file access
- Use memory dump tools to capture decrypted data in memory
- Hook file I/O APIs to intercept decryption

### Option 2: Deeper Binary Analysis
- Use ILSpy/dnSpy if we can get it working
- Search for hardcoded keys/IVs in the binary
- Look for key derivation functions
- Find where `UOFileSag` is instantiated

### Option 3: Pattern Analysis
- Try common key derivation methods:
  - Hardcoded key
  - Key derived from filename
  - Key from config file
  - Key from registry/environment
- Try common IV patterns:
  - Zero IV
  - File offset-based IV
  - Filename hash as IV

## Implementation Notes

AES-256-CTR decryption requires:
```python
from Crypto.Cipher import AES
from Crypto.Util import Counter

# Key: 32 bytes
# IV: 16 bytes (used as initial counter value)
# Mode: CTR (Counter mode)

cipher = AES.new(key, AES.MODE_CTR, counter=Counter.new(128, initial_value=int.from_bytes(iv, 'big')))
decrypted = cipher.decrypt(encrypted_data)
```

## Current Status

- ✅ Identified encryption method (AES-256-CTR)
- ✅ Found file handler class (UOFileSag)
- ⏳ Need to find key/IV
- ⏳ Need to understand key derivation
- ❌ Cannot yet decrypt files
