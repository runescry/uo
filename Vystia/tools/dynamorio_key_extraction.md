# DynamoRIO AES Key Extraction Guide

## Prerequisites

### 1. Install Visual Studio Build Tools (Required for custom client)
Download from: https://visualstudio.microsoft.com/downloads/#build-tools-for-visual-studio-2022

During installation, select:
- "Desktop development with C++" workload
- Windows 10/11 SDK

### 2. Install CMake
Download from: https://cmake.org/download/
Choose: "Windows x64 Installer"

### 3. DynamoRIO (Already installed)
Location: `D:\Tools\DynamoRIO\DynamoRIO-Windows-10.0.0\`

---

## Quick Test First (No Compilation Needed!)

Before building a custom client, test if DynamoRIO works with ClassicUO using the pre-built `opcodes.dll`:

```cmd
D:\Tools\DynamoRIO\DynamoRIO-Windows-10.0.0\bin64\drrun.exe -c D:\Tools\DynamoRIO\DynamoRIO-Windows-10.0.0\samples\bin64\opcodes.dll -- "D:\Ultima Online - Sagas\ClassicUO\ClassicUO.exe"
```

This runs the pre-built `opcodes.dll` which counts all instruction types.
Look for `aesenc`, `aesdec` in the output to confirm AES-NI is being used.

---

## Build the AES Key Extractor (After installing VS Build Tools)

### Step 1: Open Developer Command Prompt
Search for "Developer Command Prompt for VS 2022" in Start menu.

### Step 2: Navigate to build directory
```cmd
cd D:\UO\Vystia\tools\dynamorio_aes
mkdir build
cd build
```

### Step 3: Configure with CMake
```cmd
cmake -G "Visual Studio 17 2022" -A x64 -DDynamoRIO_DIR=D:\Tools\DynamoRIO\DynamoRIO-Windows-10.0.0\cmake ..
```

### Step 4: Build
```cmd
cmake --build . --config Release
```

This creates: `Release\aes_key_extractor.dll`

---

## Run Against ClassicUO

### Launch with DynamoRIO
```cmd
D:\Tools\DynamoRIO\DynamoRIO-Windows-10.0.0\bin64\drrun.exe -c D:\UO\Vystia\tools\dynamorio_aes\build\Release\aes_key_extractor.dll -- "D:\Ultima Online - Sagas\ClassicUO\ClassicUO.exe"
```

### Output
The tool creates `aes_keys.log` in the current directory with:
- Every AES-NI instruction executed
- XMM0-XMM15 register contents at each AES instruction
- Potential 256-bit keys (XMM pairs)

---

## Expected Results

If successful, `aes_keys.log` will contain entries like:

```
========== AES #1 ==========
PC: 0x7ff612345678
Opcode: AESDEC
XMM00: ce0842d2405328b7f5a190af9e2fe0d2  <- This might be the IV
XMM01: xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx  <- Potential key bytes
XMM02: xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
...

Potential 256-bit keys:
XMM0+XMM1: xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
```

The 32-byte AES-256 key will appear in one of these XMM register combinations.

---

## Troubleshooting

### "Application refused to run under DynamoRIO"
Some apps detect instrumentation. Try:
```cmd
drrun.exe -no_hw_randomization -c ... -- app.exe
```

### "Access Denied"
Run as Administrator

### Build fails
Make sure you opened "Developer Command Prompt for VS 2022", not regular CMD.

---

## Files Created

- `D:\UO\Vystia\tools\dynamorio_aes\CMakeLists.txt` - Build configuration
- `D:\UO\Vystia\tools\dynamorio_aes\aes_key_extractor.cpp` - Source code
- `D:\UO\Vystia\tools\dynamorio_extract_key.cpp` - Backup of source

---

## Alternative: x64dbg Method

If DynamoRIO doesn't work (crashes game), try x64dbg:

### Step 1: Open x64dbg as Administrator
Location: `D:\Tools\x64dbg\release\x64\x64dbg.exe`

### Step 2: Attach to ClassicUO
- File -> Attach
- Find ClassicUO.exe in the list
- Click Attach

### Step 3: Search for AES-NI Instructions
In the command bar at bottom, type:
```
findasm aesdec
```
or
```
findasm aesenc
```

### Step 4: Set Breakpoint
- Right-click on the found instruction
- Select "Breakpoint" -> "Toggle"

### Step 5: When Breakpoint Hits
- Look at the XMM registers panel (usually on the right)
- XMM0-XMM15 will show the key/data values
- The AES-256 key is 32 bytes, likely in XMM0+XMM1 or similar

### Step 6: Log XMM Values
In command bar:
```
log "XMM0={XMM0} XMM1={XMM1}"
```

### Note on Anti-Debug
If game detects debugger and crashes:
1. Try ScyllaHide plugin (anti-anti-debug)
2. Or use "Hide Debugger" option in x64dbg settings
