# Quick Build Guide

## TL;DR - Use the Script! 🚀

Instead of manually building and copying every time, use the automated script:

```powershell
.\build-and-copy.ps1
```

That's it! The script will:
1. ✅ Check if ServUO is running (stops you if it is)
2. ✅ Build the Scripts project
3. ✅ Copy Scripts.dll to root automatically

## Why You Need to Copy Every Time

**Yes, you need to do this every time you make code changes.**

Here's why:
- ServUO loads `Scripts.dll` from the **root directory** (`ServUO/Scripts.dll`)
- The build outputs to `Scripts/bin/Release/Scripts.dll`
- Even though the project has `OutputPath=..\`, the copy doesn't always happen automatically
- If you skip the copy, ServUO loads the **old DLL** and your changes are ignored

## Manual Method (If You Prefer)

If you don't want to use the script:

```powershell
# 1. Stop ServUO (if running)

# 2. Build
cd ServUO
dotnet build Scripts/Scripts.csproj -c Release

# 3. Copy (CRITICAL - use PowerShell Copy-Item!)
Copy-Item -Path "Scripts\bin\Release\Scripts.dll" -Destination "Scripts.dll" -Force

# 4. Start ServUO
```

## Using the Script

### Basic Usage
```powershell
cd ServUO
.\build-and-copy.ps1
```

### Debug Build
```powershell
.\build-and-copy.ps1 -Configuration Debug
```

### What It Does
1. **Checks if ServUO is running** - Prevents build failures from locked files
2. **Builds the project** - Compiles all your code changes
3. **Copies DLL to root** - Ensures ServUO loads the new version
4. **Shows file info** - Verifies the copy worked

## Troubleshooting

### "ServUO is running" Error
**Fix:** Stop the ServUO server first. The DLL is locked while running.

### "Build failed" Error
**Fix:** Check for compilation errors in your code. The script will show them.

### "Copy failed" Error
**Fix:** Make sure you have write permissions to the ServUO root directory.

### Changes Not Appearing
**Check:**
1. Did the build succeed? (Look for "Build succeeded")
2. Did the copy succeed? (Check the file info output)
3. Did you restart ServUO? (Old DLL is loaded in memory)
4. Check console for initialization messages

## Pro Tips

1. **Use the script** - It's faster and prevents mistakes
2. **Always stop ServUO first** - Prevents file lock issues
3. **Check console output** - Look for your `Initialize()` messages
4. **Verify file timestamp** - The copied DLL should have a recent timestamp

## Workflow

```powershell
# 1. Make code changes
# (Edit files in ServUO/Scripts/...)

# 2. Stop ServUO (if running)

# 3. Run build script
.\build-and-copy.ps1

# 4. Start ServUO

# 5. Test changes
```

That's it! The script automates steps 2-3, so you just need to:
- Edit code
- Run script
- Start server
- Test

