# Runtime Analysis Guide for .SAG File Decryption

## Overview

If static analysis doesn't reveal the encryption key/IV, runtime analysis can capture decrypted data directly from memory while the Sagas ClassicUO client is running.

## Method 1: Memory Dump Analysis

### Using Process Hacker

1. **Start Sagas ClassicUO client**
   - Launch `D:\Ultima Online - Sagas\ClassicUO\ClassicUO.exe`
   - Let it load and access `.sag` files

2. **Create Memory Dump**
   - Open Process Hacker
   - Find `ClassicUO.exe` process
   - Right-click → Create dump file → Full dump
   - Save to a location with enough space

3. **Analyze Dump**
   - Use tools like `strings` or hex editors to search for:
     - Known UO data patterns (multi components, map tiles)
     - Decrypted file headers
     - Key/IV values near decrypted data

### Using Cheat Engine

1. **Attach to Process**
   - Open Cheat Engine
   - File → Open Process → Select `ClassicUO.exe`

2. **Search for Known Patterns**
   - If you know what decrypted data should look like, search for it
   - Example: Search for known multi component values
   - Find where decrypted data is stored in memory

3. **Extract Decrypted Data**
   - Once found, dump the memory region
   - Compare with encrypted `.sag` file to find key/IV

## Method 2: API Hooking

### Using API Monitor

1. **Monitor File I/O**
   - Launch API Monitor
   - Add `ClassicUO.exe` to monitored processes
   - Filter for file operations (ReadFile, CreateFile, etc.)

2. **Capture Decryption**
   - Watch for `.sag` file access
   - Monitor crypto API calls (CryptDecrypt, etc.)
   - Capture parameters including keys/IVs

### Using Frida (Advanced)

Create a Frida script to hook decryption functions:

```javascript
// Hook AES decryption
var aes = Module.findExportByName(null, "AES_decrypt");
if (aes) {
    Interceptor.attach(aes, {
        onEnter: function(args) {
            console.log("AES_decrypt called");
            console.log("Key:", hexdump(args[0], {length: 32}));
            console.log("IV:", hexdump(args[1], {length: 16}));
        }
    });
}
```

## Method 3: Process Monitor

1. **Monitor File Access**
   - Run Process Monitor (ProcMon)
   - Filter for `ClassicUO.exe`
   - Watch for `.sag` file reads

2. **Identify Decryption Points**
   - Note when files are accessed
   - Check stack traces to find decryption code
   - Look for timing patterns (decryption happens before use)

## Method 4: .NET Decompilation

### Using ILSpy

1. **Decompile Executable**
   - Open ILSpy
   - File → Open → Select `ClassicUO.exe`
   - Navigate to `UOFileSag` class

2. **Find Decryption Code**
   - Search for `DecryptAES256_CTR`
   - Find key/IV initialization
   - Extract key derivation logic

### Using dnSpy

1. **Debug and Decompile**
   - Open dnSpy
   - File → Open → Select `ClassicUO.exe`
   - Set breakpoints in `UOFileSag` class
   - Step through decryption code
   - Inspect key/IV values at runtime

## Method 5: Python Memory Analysis

Create a script to analyze memory dumps:

```python
import mmap
import struct

def find_decrypted_data(dump_path, known_pattern):
    """Search memory dump for known decrypted data patterns"""
    with open(dump_path, 'rb') as f:
        with mmap.mmap(f.fileno(), 0, access=mmap.ACCESS_READ) as mm:
            # Search for pattern
            offset = mm.find(known_pattern)
            if offset != -1:
                # Extract surrounding data
                context = mm[max(0, offset-1000):offset+1000]
                return context
    return None
```

## Recommended Approach

1. **Start with ILSpy/dnSpy** - Easiest if executable is .NET
2. **Try Process Monitor** - See when/how files are accessed
3. **Use Memory Dump** - If decompilation doesn't work
4. **API Hooking** - Most advanced but most reliable

## Expected Results

Once key/IV is found:
- Use `decrypt_sag.py` to decrypt files
- Use `sag_to_mul.py` to convert to `.mul` format
- Use `validate_decrypted.py` to verify
- Use with UO Fiddler/CentrED

## Tools Needed

- Process Hacker: https://processhacker.sourceforge.io/
- Cheat Engine: https://cheatengine.org/
- API Monitor: http://www.rohitab.com/apimonitor
- ILSpy: https://github.com/icsharpcode/ILSpy
- dnSpy: https://github.com/dnSpy/dnSpy
- Process Monitor: https://docs.microsoft.com/sysinternals/downloads/procmon
