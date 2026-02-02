# Can UO Fiddler and CentrED Read .SAG Files?

## Short Answer: **No, not directly**

Both UO Fiddler and CentrED are hardcoded to read `.mul` files and cannot read `.sag` files without modification. Additionally, even if we could point them at `.sag` files, the files are **encrypted**, so the tools wouldn't be able to parse the data.

## Detailed Analysis

### UO Fiddler

**File Loading System:**
- Uses a `Files.MulPath` dictionary that maps filenames to file paths
- Has a **hardcoded list** of expected filenames (all with `.mul` or `.uop` extensions)
- File paths are configurable, but **filenames are hardcoded** in `Ultima/Files.cs`

**Example from code:**
```csharp
private readonly static string[] _uoFiles = {
    "anim.mul",
    "art.mul",
    "map0.mul",
    "multi.mul",
    // ... etc, all hardcoded to .mul
};
```

**What this means:**
- You can configure UO Fiddler to look in `D:\Ultima Online - Sagas\UOData\`
- But it will still look for `map0.mul`, `multi.mul`, etc.
- It won't find `map0.sag`, `multi.sag` because those names aren't in its expected file list

### CentrED

**File Loading System:**
- Uses XML configuration files (`Cedserver.xml`) that specify **exact file paths**
- Example from config:
  ```xml
  <Map>D:\UO\UO Adventures\Client\Data Files\map0.mul</Map>
  <Statics>D:\UO\UO Adventures\Client\Data Files\statics0.mul</Statics>
  ```

**What this means:**
- You could manually edit the config to point to `.sag` files
- But CentrED's file readers are designed for `.mul` format
- Even if you point it at `.sag` files, it would fail because:
  1. The files are encrypted
  2. The file reading code expects `.mul` format structure

## Workaround Options

### Option 1: Decrypt First, Then Convert (Recommended)

If we can reverse engineer the Sagas ClassicUO client to find the decryption method:

1. **Decrypt .sag files** → Convert to standard `.mul` format
2. **Use UO Fiddler/CentrED** on the decrypted `.mul` files
3. **Re-encrypt** if you need to put them back

**Tools needed:**
- Custom Python script to decrypt `.sag` → `.mul`
- Standard UO Fiddler/CentrED for editing
- Custom script to re-encrypt `.mul` → `.sag`

### Option 2: Modify the Tools

**For UO Fiddler:**
- Modify `Ultima/Files.cs` to also look for `.sag` extensions
- Add decryption layer in file reading code
- **Complexity:** High (requires C# development)

**For CentrED:**
- Modify file readers to handle `.sag` format
- Add decryption before reading
- **Complexity:** High (requires C# development)

### Option 3: Use Sagas Client as Intermediary

1. **Use Sagas ClassicUO client** to load `.sag` files (it can read them)
2. **Extract data from memory** while client is running
3. **Save as `.mul` format**
4. **Use UO Fiddler/CentrED** on extracted `.mul` files

**Tools needed:**
- Memory dump tool (e.g., Process Hacker, Cheat Engine)
- Custom script to parse memory dumps
- Standard UO tools for editing

## Recommended Approach

**Best path forward:**

1. **Reverse engineer Sagas ClassicUO.exe** to find decryption method
   - Use ILSpy or dnSpy to decompile
   - Look for `.sag` file reading code
   - Extract decryption algorithm

2. **Create decryption tool** (Python script)
   - Implement decryption based on findings
   - Convert `.sag` → `.mul`

3. **Use standard UO tools** (UO Fiddler/CentrED)
   - Work with decrypted `.mul` files
   - Make your edits

4. **Re-encrypt if needed**
   - Convert edited `.mul` → `.sag`
   - Use with Sagas client

## Current Status

- ✅ **Analysis complete**: Files are encrypted, tools can't read them
- ⏳ **Next step**: Reverse engineer Sagas ClassicUO.exe for decryption method
- ❌ **Not yet possible**: Direct use of UO Fiddler/CentrED on `.sag` files

## Conclusion

**UO Fiddler and CentrED cannot read `.sag` files** because:
1. They're hardcoded for `.mul` extensions
2. The `.sag` files are encrypted
3. The tools don't have decryption capabilities

**To use these tools, you'll need to:**
- First decrypt the `.sag` files (by reverse engineering the Sagas client)
- Convert them to `.mul` format
- Then use UO Fiddler/CentrED normally
