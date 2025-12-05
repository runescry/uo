# ServUO File Locations and Workflow Guide

## 🎯 **CRITICAL: Where to Edit Files**

ServUO has a **confusing file structure** with multiple copies of the same files. This document clarifies which files actually get loaded by the server.

---

## 📂 **File Hierarchy (Priority Order)**

### **1. Runtime Scripts (HIGHEST PRIORITY - EDIT THESE!)**
```
Location: C:\DevEnv\GIT\UO\ServUO\Server\bin\Release\Scripts\*.cs
Priority: ⭐⭐⭐ CRITICAL - These files are loaded FIRST by ServUO
```

**Why:** ServUO uses **runtime compilation**. Files in this directory are compiled and loaded at runtime, OVERRIDING the compiled Scripts.dll.

**Files to edit:**
- `VystiaWorldGenerator.cs` - Main world generation file

### **2. Source Scripts (Compiled into Scripts.dll)**
```
Location: C:\DevEnv\GIT\UO\ServUO\Scripts\**\*.cs
Priority: ⭐⭐ MEDIUM - Only used if no runtime file exists
```

**Why:** These files are compiled into `Scripts.dll` during build, but runtime scripts take precedence.

**Current structure:**
- `Scripts/Custom/VystiaWorldGenerator.cs` - Source file (gets compiled but ignored at runtime)

### **3. Vystia Project Source (Development)**
```
Location: C:\DevEnv\GIT\UO\Vystia\src\*.cs
Priority: ⭐ LOW - Reference only, NOT used by ServUO
```

**Why:** This is your development/source control location. Changes here do NOT affect ServUO unless copied to the correct locations.

---

## 🔄 **Correct Workflow for Editing**

### **Option A: Edit Runtime Files Directly (RECOMMENDED)**

1. **Edit the runtime file:**
   ```
   C:\DevEnv\GIT\UO\ServUO\Server\bin\Release\Scripts\VystiaWorldGenerator.cs
   ```

2. **Restart ServUO server:**
   - ServUO will automatically recompile the runtime script
   - Changes take effect immediately

3. **Backup to source control:**
   ```bash
   cp /c/DevEnv/GIT/UO/ServUO/Server/bin/Release/Scripts/VystiaWorldGenerator.cs \
      /c/DevEnv/GIT/UO\Vystia/src/VystiaWorldGenerator.cs
   ```

### **Option B: Edit Source and Deploy**

1. **Edit the source file:**
   ```
   C:\DevEnv\GIT\UO\Vystia\src\VystiaWorldGenerator.cs
   ```

2. **Copy to BOTH locations:**
   ```bash
   # Copy to runtime location (required!)
   cp /c/DevEnv/GIT/UO/Vystia/src/VystiaWorldGenerator.cs \
      /c/DevEnv/GIT/UO/ServUO/Server/bin/Release/Scripts/VystiaWorldGenerator.cs

   # Copy to Scripts location (optional, for compilation)
   cp /c/DevEnv/GIT/UO/Vystia/src/VystiaWorldGenerator.cs \
      /c/DevEnv/GIT/UO/ServUO/Scripts/Custom/VystiaWorldGenerator.cs
   ```

3. **Restart ServUO server**

---

## 🚫 **Common Mistakes (Why Things Go in Circles)**

### ❌ **Mistake 1: Editing Only Source Files**
```
Editing: C:\DevEnv\GIT\UO\Vystia\src\VystiaWorldGenerator.cs
Result: Changes NOT visible in game
Why: Source files are not loaded by ServUO
```

### ❌ **Mistake 2: Editing Only Scripts Project**
```
Editing: C:\DevEnv\GIT\UO\ServUO\Scripts\Custom\VystiaWorldGenerator.cs
Building: dotnet build Scripts/Scripts.csproj
Result: Changes NOT visible in game
Why: Runtime scripts override compiled Scripts.dll
```

### ❌ **Mistake 3: Forgetting to Restart Server**
```
Editing: Correct file
Result: Changes NOT visible in game
Why: Server must restart to recompile runtime scripts
```

### ✅ **Correct Approach**
```
Editing: C:\DevEnv\GIT\UO\ServUO\Server\bin\Release\Scripts\VystiaWorldGenerator.cs
Action: Restart ServUO server
Result: Changes visible in game immediately
```

---

## 🔍 **How to Verify Which File is Loaded**

### **Method 1: Check Server Console**
When ServUO starts, it shows:
```
Core: Compiling scripts...
Scripts -> C:\DevEnv\GIT\UO\ServUO\Scripts\bin\Release\Scripts.dll
```

But then it ALSO compiles runtime scripts from:
```
C:\DevEnv\GIT\UO\ServUO\Server\bin\Release\Scripts\*.cs
```

### **Method 2: Add Debug Messages**
Add this to the top of `Initialize()`:
```csharp
public static void Initialize()
{
    Console.WriteLine("VystiaWorldGenerator loaded from RUNTIME SCRIPTS");
    // ... rest of code
}
```

Check the server console on startup to see which version is loaded.

### **Method 3: Check File Timestamps**
```bash
ls -la /c/DevEnv/GIT/UO/ServUO/Server/bin/Release/Scripts/VystiaWorldGenerator.cs
ls -la /c/DevEnv/GIT/UO/ServUO/Scripts/Custom/VystiaWorldGenerator.cs
ls -la /c/DevEnv/GIT/UO/Vystia/src/VystiaWorldGenerator.cs
```

The file with the most recent timestamp should be your latest edit.

---

## 📋 **File Synchronization Checklist**

When making changes to VystiaWorldGenerator.cs:

- [ ] Edit the file at: `Server/bin/Release/Scripts/VystiaWorldGenerator.cs`
- [ ] Restart ServUO server
- [ ] Test commands in-game
- [ ] If working, copy to source: `Vystia/src/VystiaWorldGenerator.cs`
- [ ] (Optional) Copy to Scripts: `Scripts/Custom/VystiaWorldGenerator.cs`
- [ ] Commit changes to source control

---

## 🛠️ **CORRECT Build and Deploy Process**

### **THE REAL PROBLEM (DISCOVERED OCT 21, 2025)**

ServUO loads `Scripts.dll` from **THE ROOT DIRECTORY**: `C:\DevEnv\GIT\UO\ServUO\Scripts.dll`

The build project is configured to output there (`OutputPath=..\`), but sometimes the file doesn't get copied automatically.

### **CORRECT Workflow:**

1. **Edit the file:**
   ```
   C:\DevEnv\GIT\UO\ServUO\Scripts\Commands\VystiaWorldGenerator.cs
   ```

2. **Stop ServUO completely** (file is locked while running)

3. **Build the project:**
   ```bash
   cd /c/DevEnv/GIT/UO/ServUO && dotnet build Scripts/Scripts.csproj -c Release
   ```

4. **Manually copy DLL to root** (if build didn't auto-copy):
   ```bash
   cp /c/DevEnv/GIT/UO/ServUO/Scripts/bin/Release/Scripts.dll \
      /c/DevEnv/GIT/UO/ServUO/Scripts.dll
   ```

5. **Start ServUO** - it will load the new Scripts.dll

6. **Verify in console** - look for:
   ```
   *** VystiaWorldGenerator Initialize() called ***
   *** VystiaWorldGenerator registered 4 commands successfully ***
   ```

### **Quick Copy Commands**

**From Scripts/Commands to Source Control:**
```bash
cp /c/DevEnv/GIT/UO/ServUO/Scripts/Commands/VystiaWorldGenerator.cs \
   /c/DevEnv/GIT/UO/Vystia/src/VystiaWorldGenerator.cs
```

**From Source Control to Scripts/Commands:**
```bash
cp /c/DevEnv/GIT/UO/Vystia/src/VystiaWorldGenerator.cs \
   /c/DevEnv/GIT/UO/ServUO/Scripts/Commands/VystiaWorldGenerator.cs
```

---

## 🎓 **Lessons Learned**

1. **ServUO loads runtime Scripts.dll LAST** - Runtime .cs files in `Server/bin/Release/Scripts/` take precedence
2. **Building Scripts.csproj is NOT enough** - Must also copy to runtime location
3. **Always edit the runtime file directly** - Or copy changes there before testing
4. **Server restart is REQUIRED** - Runtime compilation happens at startup
5. **Keep source files in sync** - Use the copy commands above

---

## 🔗 **Related Files**

- `VYSTIA_TROUBLESHOOTING_LOG.md` - History of issues and solutions
- `VYSTIA_COMMANDS_GUIDE.md` - Available commands
- `VYSTIA_GO_COMMANDS.md` - Teleport commands

---

**Last Updated:** 2025-10-21
**Status:** Active reference document
