# SQLite DLL Requirements

## Why NuGet Packages?

SQLite is a **native C library**, not a pure .NET library. To use it from .NET, we need:

1. **System.Data.SQLite.dll** - The .NET wrapper (managed code)
2. **SQLite.Interop.dll** - The native interop layer (unmanaged code, x64/x86)

These come from the NuGet package `System.Data.SQLite` (which depends on `stub.system.data.sqlite.core.netframework`).

## The Problem

When you install the NuGet package, it:
- ✅ Adds the reference to your project
- ✅ Includes the DLLs in the package
- ❌ **Does NOT automatically copy them to the ServUO root directory**

ServUO.exe runs from the root directory, so it needs the DLLs there.

## The Solution

The `build-and-copy.ps1` script now automatically copies:
- `System.Data.SQLite.dll` → ServUO root
- `SQLite.Interop.dll` (x64) → ServUO root

This happens every time you build, so you don't need to manually copy them.

## Alternative: Pure Managed SQLite

If you want to avoid native DLLs entirely, you could use:
- **Microsoft.Data.Sqlite** - But this requires .NET Core/.NET 5+, not .NET Framework 4.8
- **SQLitePCL.raw** - Pure managed, but more complex API

For .NET Framework 4.8, `System.Data.SQLite` is the standard, recommended approach.

## Verification

After building, check that these files exist in `ServUO/`:
- ✅ `System.Data.SQLite.dll` (~1-2 MB)
- ✅ `SQLite.Interop.dll` (~1 MB, x64 version)

If they're missing, the server will crash with:
```
System.DllNotFoundException: Unable to load DLL 'SQLite.Interop.dll'
```

## Summary

- **Yes, we're using SQLite** - Simple, file-based database
- **But SQLite needs native DLLs** - This is how SQLite works on Windows
- **The build script copies them automatically** - You don't need to think about it
- **This is normal** - All SQLite .NET implementations require this

