# ServUO Build Sequence - CRITICAL REFERENCE

## 🚨 **THE PROBLEM WE DISCOVERED**

After hours of troubleshooting on Oct 21, 2025, we discovered that ServUO has a **specific build and deployment sequence** that MUST be followed, or your code changes will not be loaded.

---

## 📋 **REQUIRED BUILD SEQUENCE**

### **Step 1: Stop ServUO**
```bash
# CRITICAL: Server must be stopped or Scripts.dll is locked
# Kill the ServUO process completely
```

**Why:** The `Scripts.dll` file is locked while ServUO is running, preventing updates.

---

### **Step 2: Edit Your Code**
```
File Location: D:\UO\ServUO\Scripts\Commands\VystiaWorldGenerator.cs
```

**Important Notes:**
- Edit files in `Scripts/Commands/` directory (NOT in Custom, NOT in src)
- Use namespace `Server.Commands` (NOT `Server.Systems.Vystia`)
- Class must have `public static void Initialize()` method
- Commands are registered with `CommandSystem.Register()`

---

### **Step 3: Build the Scripts Project**
```bash
cd /c/DevEnv/GIT/UO/ServUO
dotnet build Scripts/Scripts.csproj -c Release
```

**What this does:**
- Compiles all .cs files in the Scripts directory
- Outputs to: `Scripts/bin/Release/Scripts.dll`
- **Should** copy to root: `ServUO/Scripts.dll` (but sometimes doesn't)

**Expected Output:**
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

---

### **Step 4: MANUALLY Copy Scripts.dll to Root**

**🚨 CRITICAL: Use PowerShell Copy-Item - cmd /c copy DOES NOT WORK!**

```powershell
# THE ONLY COMMAND THAT WORKS:
powershell.exe -Command "Copy-Item -Path 'D:\UO\ServUO\Scripts\bin\Release\Scripts.dll' -Destination 'D:\UO\ServUO\Scripts.dll' -Force"
```

**DO NOT USE THESE - THEY FAIL SILENTLY:**
```bash
# ❌ DOES NOT WORK:
cp /c/DevEnv/GIT/UO/ServUO/Scripts/bin/Release/Scripts.dll /c/DevEnv/GIT/UO/ServUO/Scripts.dll

# ❌ DOES NOT WORK:
cmd /c copy "D:\UO\ServUO\Scripts\bin\Release\Scripts.dll" "D:\UO\ServUO\Scripts.dll"

# ❌ DOES NOT WORK:
copy /Y D:\UO\ServUO\Scripts\bin\Release\Scripts.dll D:\UO\ServUO\Scripts.dll
```

**Why this is needed:**
- ServUO loads `Scripts.dll` from the **root directory**: `D:\UO\ServUO\Scripts.dll`
- The build project is configured with `OutputPath=..\` which should copy there
- But the copy NEVER happens automatically in our setup
- If you skip this step, ServUO loads the OLD DLL and your changes are ignored
- **cmd.exe copy commands fail silently on Windows** - use PowerShell Copy-Item!

**Verify the copy worked:**
```powershell
powershell.exe -Command "Get-Item 'D:\UO\ServUO\Scripts.dll' | Select-Object Name, LastWriteTime, Length"
# Check timestamp - should match build time (within last few seconds)
# Check file size - should be different if you added new code
```

---

### **Step 5: Start ServUO**
```bash
# Start ServUO server
```

**During startup, watch for:**
```
Core: Compiling scripts...
MSBuild version 17.3.2+561848881 for .NET
  Scripts -> D:\UO\ServUO\Scripts\bin\Release\Scripts.dll

Build succeeded.
```

**Then look for your Initialize messages:**
```
*** VystiaWorldGenerator Initialize() called ***
*** VystiaWorldGenerator registered 4 commands successfully ***
```

**If you DON'T see these messages:**
- Your code has a runtime error (silently caught)
- The Scripts.dll wasn't copied to the root directory
- Go back to Step 1 and try again

---

### **Step 6: Test In-Game**
```
[DeleteBadSpawns
[GenVystiaWorld
[Go 2000 2000 0
```

**Expected Results:**
- Commands should be recognized (no "not a valid command" error)
- Commands should execute and show feedback messages
- Terrain markers should appear in the game world

---

## 🔄 **QUICK REFERENCE WORKFLOW**

```powershell
# 1. Stop ServUO (close the server window)

# 2. Edit your code
# D:\UO\ServUO\Scripts\Commands\VystiaWorldGenerator.cs

# 3. Build (server must be stopped!)
dotnet build "D:\UO\ServUO\Scripts\Scripts.csproj" -c Release

# 4. Copy DLL to root (CRITICAL! Use PowerShell Copy-Item!)
powershell.exe -Command "Copy-Item -Path 'D:\UO\ServUO\Scripts\bin\Release\Scripts.dll' -Destination 'D:\UO\ServUO\Scripts.dll' -Force"

# 5. Verify the copy worked
powershell.exe -Command "Get-Item 'D:\UO\ServUO\Scripts.dll' | Select-Object Name, LastWriteTime, Length"

# 6. Start ServUO and verify console shows:
# *** VystiaWorldGenerator Initialize() called ***
# *** VystiaMapWiper Initialize() called ***

# 7. Test in-game
# [GenVystiaWorld
# [WipeFelucca
```

---

## ⚠️ **COMMON MISTAKES**

### ❌ **Mistake #1: Building While Server is Running**
```bash
# Server is running...
dotnet build Scripts/Scripts.csproj -c Release
# Build succeeds BUT Scripts.dll is locked and doesn't update!
```

**Result:** Changes are not loaded. You waste time wondering why your code isn't working.

**Fix:** Always stop the server first!

---

### ❌ **Mistake #2: Not Copying Scripts.dll to Root**
```bash
dotnet build Scripts/Scripts.csproj -c Release
# Outputs to: Scripts/bin/Release/Scripts.dll
# But ServUO loads from: Scripts.dll (root)
# Forgot to copy!
```

**Result:** ServUO loads the old DLL. Your changes are ignored.

**Fix:** Always manually copy after building!

---

### ❌ **Mistake #3: Editing Wrong File Location**
```bash
# Editing: Vystia/src/VystiaWorldGenerator.cs
# But ServUO compiles from: ServUO/Scripts/Commands/VystiaWorldGenerator.cs
```

**Result:** Changes are in the wrong place and never get compiled.

**Fix:** Edit files in `ServUO/Scripts/Commands/` directly!

---

### ❌ **Mistake #4: Wrong Namespace**
```csharp
namespace Server.Systems.Vystia  // WRONG!
{
    public class VystiaWorldGenerator
    {
        public static void Initialize() { ... }
    }
}
```

**Result:** Initialize() is never called. Class isn't discovered by ServUO.

**Fix:** Use `namespace Server.Commands`

---

### ❌ **Mistake #5: Not Checking Console Output**
```
# Server starts but no debug messages appear
# Assume it's working and test in-game
# Commands don't work
```

**Result:** You don't realize Initialize() was never called.

**Fix:** ALWAYS check for the debug messages in console:
```
*** VystiaWorldGenerator Initialize() called ***
```

---

## 🎯 **VERIFICATION CHECKLIST**

After building, verify each step:

- [ ] ServUO is completely stopped
- [ ] `dotnet build` shows "Build succeeded"
- [ ] `Scripts/bin/Release/Scripts.dll` timestamp is current
- [ ] **`Scripts.dll` (root) timestamp is current** ← CRITICAL!
- [ ] Server console shows Initialize() messages on startup
- [ ] Commands work in-game without "not valid" errors

---

## 📁 **FILE LOCATIONS REFERENCE**

### **Source File (for version control)**
```
D:\UO\Vystia\src\VystiaWorldGenerator.cs
```
**Purpose:** Git repository, backups, documentation

---

### **Active Development File (where you actually edit)**
```
D:\UO\ServUO\Scripts\Commands\VystiaWorldGenerator.cs
```
**Purpose:** This is the file that gets compiled into Scripts.dll

---

### **Build Output**
```
D:\UO\ServUO\Scripts\bin\Release\Scripts.dll
```
**Purpose:** Where the build process outputs the compiled DLL

---

### **Runtime File (what ServUO actually loads!)**
```
D:\UO\ServUO\Scripts.dll
```
**Purpose:** This is the DLL that ServUO loads at startup
**CRITICAL:** This is the file that MUST be updated or changes won't work!

---

## 🔧 **DEBUGGING TIPS**

### **If Initialize() Doesn't Get Called:**

1. **Check DLL timestamps:**
   ```bash
   ls -lh /c/DevEnv/GIT/UO/ServUO/Scripts.dll
   ls -lh /c/DevEnv/GIT/UO/ServUO/Scripts/bin/Release/Scripts.dll
   ```
   Both should have current timestamps!

2. **Verify class is in DLL:**
   ```bash
   powershell.exe -ExecutionPolicy Bypass -File TestScriptsDLL.ps1
   ```
   Should show:
   ```
   FOUND: VystiaWorldGenerator class
     - Initialize() method found!
   ```

3. **Add file-based logging:**
   ```csharp
   public static void Initialize()
   {
       File.WriteAllText("C:\\INIT_CALLED.txt", "Called at " + DateTime.Now);
       // rest of code...
   }
   ```
   If file appears, Initialize() ran. If not, it didn't run.

4. **Check for runtime exceptions:**
   - Remove all code except Console.WriteLine
   - If that works, add code back piece by piece
   - Find which line causes the silent exception

---

## 📝 **BACKUP AND SYNC COMMANDS**

### **Backup from ServUO to Source Control:**
```powershell
powershell.exe -Command "Copy-Item -Path 'D:\UO\ServUO\Scripts\Commands\VystiaWorldGenerator.cs' -Destination 'D:\UO\Vystia\src\VystiaWorldGenerator.cs' -Force"
powershell.exe -Command "Copy-Item -Path 'D:\UO\ServUO\Scripts\Commands\VystiaMapWiper.cs' -Destination 'D:\UO\Vystia\src\VystiaMapWiper.cs' -Force"
```

### **Restore from Source Control to ServUO:**
```powershell
powershell.exe -Command "Copy-Item -Path 'D:\UO\Vystia\src\VystiaWorldGenerator.cs' -Destination 'D:\UO\ServUO\Scripts\Commands\VystiaWorldGenerator.cs' -Force"
powershell.exe -Command "Copy-Item -Path 'D:\UO\Vystia\src\VystiaMapWiper.cs' -Destination 'D:\UO\ServUO\Scripts\Commands\VystiaMapWiper.cs' -Force"
```

### **After copying, remember to rebuild and deploy:**
```powershell
dotnet build "D:\UO\ServUO\Scripts\Scripts.csproj" -c Release
powershell.exe -Command "Copy-Item -Path 'D:\UO\ServUO\Scripts\bin\Release\Scripts.dll' -Destination 'D:\UO\ServUO\Scripts.dll' -Force"
```

---

## 🎓 **LESSONS LEARNED**

1. **ServUO compiles at runtime** but loads from a pre-built DLL
2. **The root Scripts.dll is what matters** - not the build output
3. **Building while server runs = wasted effort** - file is locked
4. **No console messages = Initialize() didn't run** - don't assume it worked
5. **Namespace matters** - use `Server.Commands` not custom namespaces
6. **Manual DLL copy is required** - the build doesn't always auto-copy

---

**Last Updated:** October 21, 2025
**Status:** WORKING - All commands functional, terrain markers visible
**Critical Discovery:** Must manually copy Scripts.dll to root directory
