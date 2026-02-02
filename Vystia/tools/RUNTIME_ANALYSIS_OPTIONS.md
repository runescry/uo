# Runtime Analysis Options for Key/IV Extraction

## Overview

Since static analysis didn't reveal the encryption key/IV, we need to extract them at runtime while the Sagas ClassicUO client is running.

## Option 1: Frida (Easiest - Recommended)

**Pros:**
- Easy to use, Python-based
- Good documentation
- Cross-platform
- Active community

**Cons:**
- Requires installation
- May be detected by anti-debugging

**Setup:**
```bash
pip install frida frida-tools
```

**Usage:**
```bash
python use_frida_for_key_extraction.py
```

Then launch Sagas ClassicUO and load a .sag file. The script will hook into decryption functions and extract key/IV.

## Option 2: Simple Memory Dump (Simplest)

**Pros:**
- No special tools needed
- Uses standard Windows APIs
- Easy to understand

**Cons:**
- Less precise (searches entire memory)
- May find false positives
- Requires psutil library

**Setup:**
```bash
pip install psutil
```

**Usage:**
```bash
# Start Sagas ClassicUO first, then:
python simple_memory_dump.py
```

This scans the process memory for potential 32-byte keys and 16-byte IVs.

## Option 3: DynamoRIO (Most Powerful)

**Pros:**
- Very powerful instrumentation
- Can hook any instruction
- Industry standard

**Cons:**
- More complex setup
- Requires compilation
- Larger download

**Setup:**
1. Download from: https://github.com/DynamoRIO/dynamorio/releases
2. Extract to `D:\Tools\DynamoRIO\`
3. Compile client (see `dynamorio_extract_key.cpp`)

**Usage:**
```bash
D:\Tools\DynamoRIO\bin32\drrun.exe -client dynamorio_extract_key.dll -- ClassicUO.exe
```

## Option 4: ILSpy/dnSpy (Best for .NET)

**Pros:**
- Can decompile .NET code
- See actual source code
- Set breakpoints and inspect variables

**Cons:**
- Only works if executable is .NET (may be AOT compiled)
- Requires manual analysis

**Setup:**
1. Download dnSpy: https://github.com/dnSpy/dnSpy/releases
2. Open `ClassicUO.exe` in dnSpy
3. Navigate to `UOFileSag` class
4. Set breakpoint in decryption method
5. Run and inspect key/IV values

## Option 5: Cheat Engine (User-Friendly)

**Pros:**
- GUI-based, easy to use
- Good for memory scanning
- Can find patterns

**Cons:**
- Less precise than API hooking
- Manual process

**Usage:**
1. Open Cheat Engine
2. Attach to ClassicUO.exe
3. Search for known patterns (if you have sample decrypted data)
4. Find memory addresses containing key/IV

## Recommended Approach

**Start with Option 1 (Frida)** - it's the easiest and most effective for this use case.

If Frida doesn't work:
- Try Option 2 (Simple Memory Dump) for a quick scan
- Use Option 4 (dnSpy) if the executable is .NET and can be decompiled
- Fall back to Option 3 (DynamoRIO) for maximum control

## Expected Output

Once key/IV is extracted, you'll get:
- 32-byte key (64 hex characters)
- 16-byte IV (32 hex characters)

Then use:
```bash
python decrypt_sag.py file.sag --key <extracted-key> --iv <extracted-iv>
```

## Troubleshooting

**"Process not found"**
- Make sure ClassicUO.exe is running
- Check process name (might be different)

**"Access denied"**
- Run as Administrator
- Check if anti-virus is blocking

**"No keys found"**
- Key/IV might be derived dynamically
- Try hooking key derivation functions
- Check if decryption happens in a DLL
