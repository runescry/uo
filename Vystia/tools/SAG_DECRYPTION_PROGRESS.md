# UO Sagas .sag File Decryption Attempt

## Summary

**Date:** January 2026
**Goal:** Decrypt .sag files from UO Sagas to extract game assets for use with standard UO tools (CentrED#, UO Fiddler)
**Result:** ❌ Unsuccessful - Sagas has professional-grade encryption protection

---

## What We Know About .sag Files

### Encryption Scheme
- **Algorithm:** AES-256-CTR
- **IV Location:** First 16 bytes of each .sag file
- **Implementation:** Hardware AES-NI (not software)
- **Key Storage:** Obfuscated/runtime-derived (not plaintext)

### Known Values (from tiledata.sag)
| Field | Value |
|-------|-------|
| IV | `ce0842d2405328b7f5a190af9e2fe0d2` |
| First encrypted block | `f2e063244b71732e5e4183e83965aed9` |
| Expected plaintext | `00000000000000000000554e55534544` ("UNUSED" at offset 10) |
| Calculated keystream | `f2e063244b71732e5e41d6a66c36eb9d` |

### Strings Found in ClassicUO.exe
- `UOFileSag` - SAG file handler class
- `DecryptAES256_CTR` - Decryption function name
- `MatchKey`, `SelectKey` - Key selection logic
- `ReadSAG`, `LoadSAGIndex` - File loading functions

---

## Methods Attempted

### 1. Frida BCrypt API Hooking ❌
**Approach:** Hook Windows BCrypt functions to capture AES key when generated

**Tools:**
- frida-tools (pip install)
- Custom JavaScript hooks for BCryptGenerateSymmetricKey

**Result:** No BCrypt calls detected - Sagas uses custom AES implementation, not Windows API

**Files Created:**
- `D:\UO\Vystia\tools\frida_hook_aes.py`
- `D:\UO\Vystia\tools\frida_trace_bcrypt.py`
- `D:\UO\Vystia\tools\frida_spawn_capture.py`
- `D:\UO\__handlers__\bcrypt.dll\BCryptGenerateSymmetricKey.js`

---

### 2. Memory Scanning ❌
**Approach:** Scan process memory for:
- Plaintext AES key
- Decrypted tiledata (pattern: zeros + "UNUSED")
- AES S-box (implementation marker)
- Encrypted .sag data blocks

**Tools:**
- pymem (Python memory access)
- Custom scanning scripts

**Results:**
- No plaintext key found
- No AES S-box in memory (confirms AES-NI hardware usage)
- "UNUSED" string found but in NVIDIA driver, not tiledata
- Encrypted blocks not found contiguously (streaming decryption)

**Files Created:**
- `D:\UO\Vystia\tools\dump_decrypted_from_memory.py`
- `D:\UO\Vystia\tools\find_tiledata_in_memory.py`
- `D:\UO\Vystia\tools\find_sag_in_memory.py`
- `D:\UO\Vystia\tools\search_keystream.py`
- `D:\UO\Vystia\tools\dump_all_memory_regions.py`
- `D:\UO\Vystia\tools\analyze_dumps.py`

---

### 3. Key Derivation Bruteforce ❌
**Approach:** Try common key derivation methods using known strings

**Tested:**
- SHA256 of known strings (UOFileSag, DecryptAES256_CTR, etc.)
- MD5 combinations
- Direct string-to-key conversions
- XOR combinations

**Result:** 111 derived keys tested, none produced valid decryption

**Files Created:**
- `D:\UO\Vystia\tools\bruteforce_key.py`
- `D:\UO\Vystia\tools\test_derived_keys.py`

---

### 4. API Monitor ❌
**Approach:** Monitor Windows API calls during .sag file loading

**Tools:**
- API Monitor x64 (D:\Tools\APIMonitor\)

**Result:**
- CRYPT32.dll calls visible but grayed out
- No BCrypt calls captured
- Confirms custom encryption implementation

---

### 5. DynamoRIO Instrumentation ❌
**Approach:** Use binary instrumentation to intercept AES-NI instructions and dump XMM registers

**Tools:**
- DynamoRIO 10.0.0 (D:\Tools\DynamoRIO\)
- Pre-built opcodes.dll sample
- Custom aes_key_extractor.cpp (not compiled - needs VS Build Tools)

**Results:**
- Launch mode: Game runs but doesn't display window
- Attach mode: Game crashes immediately
- Anti-instrumentation protection detected

**Files Created:**
- `D:\UO\Vystia\tools\dynamorio_aes\CMakeLists.txt`
- `D:\UO\Vystia\tools\dynamorio_aes\aes_key_extractor.cpp`
- `D:\UO\Vystia\tools\dynamorio_key_extraction.md`

---

### 6. x64dbg Debugging ❌
**Approach:** Attach debugger, find AES instructions, set breakpoints, capture XMM registers

**Tools:**
- x64dbg (D:\Tools\x64dbg\)

**Result:** Anti-debug protection - game detects debugger and crashes

---

## Technical Analysis

### Why Standard Methods Failed

1. **No BCrypt API**
   - Sagas implements custom AES, bypassing Windows crypto APIs
   - Cannot hook standard functions to capture key

2. **Hardware AES-NI**
   - Uses CPU's built-in AES instructions (AESENC, AESDEC)
   - No software S-box in memory to identify
   - Key loaded directly to XMM registers, never in RAM as plaintext

3. **Streaming Decryption**
   - Files not decrypted all at once
   - Small chunks decrypted on-demand
   - No full decrypted file in memory to dump

4. **Anti-Debug/Anti-Instrumentation**
   - Detects debugger attachment (x64dbg)
   - Detects binary instrumentation (DynamoRIO)
   - Crashes or refuses to run when detected

5. **NativeAOT Compilation**
   - ClassicUO compiled as native binary, not standard .NET
   - No IL to decompile with dnSpy/ILSpy
   - Harder to analyze than managed code

---

## File Structure Analysis

### Encrypted (.sag) - Cannot use with standard tools:
- map0.sag - map5.sag (maps)
- art.sag, artidx.sag (artwork)
- tiledata.sag (tile definitions)
- anim.sag - anim5.sag (animations)
- gumpart.sag, gumpidx.sag (UI elements)
- multi.sag (multi-tile structures)
- fonts.sag, light.sag, etc.

### Unencrypted - Can use normally:
- hues.mul (standard EA file)
- *.idx files (index files)
- *.def files (text configuration)
- Cliloc.enu (localization)
- *.ogg music files

---

## Remaining Options

### 1. Community Resources
- Ask UO Sagas Discord/community if anyone has decryption tools
- Check if other private server communities have solutions

### 2. Contact Developers
- Request export tools for content creators
- Ask about asset licensing for derivative projects

### 3. Deep Static Analysis (Advanced)
- Spend significant time in Ghidra/IDA
- Trace key derivation through native code
- Requires advanced RE skills and time investment

### 4. Use Standard UO Files
- Continue projects with EA's original .mul files
- These work perfectly with all existing tools
- The Vystia project already uses this approach

---

## Conclusion

The UO Sagas team implemented professional-grade asset protection:
- Custom AES-256-CTR implementation
- Hardware AES-NI acceleration
- Anti-debug and anti-instrumentation measures
- No plaintext key storage

Without insider knowledge, significant reverse engineering expertise, or community assistance, the .sag files remain encrypted. The recommended path forward is to use standard UO .mul files which are fully compatible with all editing tools.

---

## Files Created During Investigation

```
D:\UO\Vystia\tools\
├── frida_hook_aes.py
├── frida_trace_bcrypt.py
├── frida_spawn_capture.py
├── frida_aesni_hook.js
├── dump_decrypted_from_memory.py
├── find_tiledata_in_memory.py
├── find_sag_in_memory.py
├── search_keystream.py
├── dump_all_memory_regions.py
├── analyze_dumps.py
├── bruteforce_key.py
├── test_derived_keys.py
├── check_hues_mul.py
├── dynamorio_extract_key.cpp
├── dynamorio_key_extraction.md
├── SAG_DECRYPTION_PROGRESS.md (this file)
├── memory_dumps/
│   └── [30 memory dump files]
└── dynamorio_aes/
    ├── CMakeLists.txt
    └── aes_key_extractor.cpp

D:\UO\__handlers__\bcrypt.dll\
└── BCryptGenerateSymmetricKey.js

D:\Tools\
├── APIMonitor/
├── DynamoRIO/DynamoRIO-Windows-10.0.0/
└── x64dbg/
```
