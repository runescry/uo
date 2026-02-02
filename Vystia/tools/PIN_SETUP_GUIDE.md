# Intel Pin Setup and Usage Guide

## Overview

Intel Pin is a dynamic binary instrumentation framework that can intercept and analyze program execution. We'll use it to extract the AES-256-CTR key/IV from ClassicUO.exe.

## Download Intel Pin

### Option 1: Direct Download (May Require Login)

1. Visit: https://software.intel.com/content/www/us/en/develop/articles/pin-a-binary-instrumentation-tool-downloads.html
2. Download: `pin-3.28-98754-gc2c769c81-msvc-windows.zip` (or latest version)
3. Save to: `D:\Tools\IntelPin\pin-3.28-windows.zip`

### Option 2: Alternative Sources

- Check Intel's developer forums
- Look for community mirrors
- Use browser download (may work better than PowerShell)

### Option 3: Use Setup Script

```powershell
# Run the setup script (it will check for Pin and guide you)
powershell -ExecutionPolicy Bypass -File Vystia\tools\setup_pin.ps1
```

## Installation

1. **Extract Pin:**
   ```powershell
   Expand-Archive -Path "D:\Tools\IntelPin\pin-3.28-windows.zip" -DestinationPath "D:\Tools\IntelPin" -Force
   ```

2. **Verify Installation:**
   ```powershell
   D:\Tools\IntelPin\pin-3.28-*\pin.exe --version
   ```

## Compile the Pintool

### Using Visual Studio

1. **Open Visual Studio Developer Command Prompt:**
   - Start Menu → Visual Studio → Developer Command Prompt

2. **Navigate to tool directory:**
   ```cmd
   cd D:\Tools\IntelPin\pin-3.28-*\source\tools
   mkdir pin_extract_key
   cd pin_extract_key
   ```

3. **Copy source files:**
   ```cmd
   copy D:\UO\Vystia\tools\pin_extract_key.cpp .
   copy D:\UO\Vystia\tools\pin_makefile Makefile
   ```

4. **Compile:**
   ```cmd
   nmake
   ```

   Or manually:
   ```cmd
   cl /LD /MD /EHsc /O2 /I"D:\Tools\IntelPin\pin-3.28-*\source\include\pin" pin_extract_key.cpp /link /LIBPATH:"D:\Tools\IntelPin\pin-3.28-*\ia32\lib" pin.lib /OUT:pin_extract_key.dll
   ```

## Run the Pintool

```cmd
D:\Tools\IntelPin\pin-3.28-*\pin.exe -t source\tools\pin_extract_key\pin_extract_key.dll -- "D:\Ultima Online - Sagas\ClassicUO\ClassicUO.exe"
```

The tool will:
- Launch ClassicUO.exe under Pin's control
- Monitor for AES decryption
- Extract key/IV when decryption occurs
- Save results to `pin_key_extraction.log`

## Expected Output

```
Intel Pin Key/IV Extractor Started
===================================
Image loaded: ClassicUO.exe at 0x400000
*** Found ClassicUO module ***
Looking for DecryptAES256_CTR at: 0xa7c494
[POTENTIAL_KEY] Address: 0x12345678, Data: a4ba3ccb76938ff1...
[POTENTIAL_IV]  Address: 0x12345690, Data: 22d47ed724414974...
```

## Troubleshooting

### "Pin not found"
- Check Pin installation path
- Update paths in Makefile/compile command

### "Cannot attach to process"
- Run as Administrator
- Disable anti-virus temporarily
- Check if process has anti-debugging

### "No keys extracted"
- Make sure ClassicUO actually loads .sag files
- Check log file for any errors
- The decryption function address may need adjustment

### Compilation Errors
- Ensure Visual Studio C++ tools are installed
- Check Pin include/library paths
- May need to adjust for x64 vs x86 architecture

## Manual Address Adjustment

If the automatic detection doesn't work, you may need to:

1. **Find the actual runtime address:**
   - Use x64dbg or similar to find `DecryptAES256_CTR`
   - Note the actual address

2. **Update the Pintool:**
   - Edit `pin_extract_key.cpp`
   - Change `decrypt_offset = 0xa3c494` to match actual offset
   - Recompile

## Alternative: Simpler Pintool

If the full tool is complex, create a simpler version that just logs all 32-byte and 16-byte memory reads near function calls.

## Files

- `pin_extract_key.cpp` - Main Pintool source
- `pin_makefile` - Makefile for compilation
- `setup_pin.ps1` - Automated setup script
- `PIN_SETUP_GUIDE.md` - This file
