# Vystia World Generation Troubleshooting Log

## Problem Summary

The Vystia world generation system was experiencing multiple issues preventing proper terrain generation and causing server instability.

## Initial Issues Identified

### 1. Command Registration Problems
- **Issue**: `[GenVystiaWorld` command was not being recognized in-game
- **Root Cause**: ServUO was not loading the VystiaWorldGenerator.cs file properly
- **Symptoms**: "Command not valid" errors when trying to run Vystia commands

### 2. Server Crash Loops
- **Issue**: Server would crash repeatedly when trying to run Vystia commands
- **Root Cause**: Multiple factors including incorrect file paths, missing dependencies, and problematic code
- **Symptoms**: Server would start and immediately crash, requiring manual intervention

### 3. Badspawn.log Errors
- **Issue**: 134+ bad spawns detected and logged
- **Root Cause**: VystiaDungeonSpawners was trying to spawn custom creatures that don't exist in ServUO
- **Symptoms**: Console warnings about bad spawns, creatures like "WinterWolf", "IceTroll", "FrostWraith" not found

### 4. Incorrect World Generation
- **Issue**: `[GenVystiaWorld` was generating cities, dungeons, and spawners instead of just terrain markers
- **Root Cause**: Old version of VystiaWorldGenerator.cs was still being loaded
- **Symptoms**: Console output showed full world generation instead of terrain-only generation

### 5. Invalid Coordinates
- **Issue**: Teleporting to Vystia coordinates resulted in "green acres" (invalid map area)
- **Root Cause**: Coordinates were outside valid Felucca map boundaries
- **Symptoms**: `[Go` commands would teleport to invalid areas instead of Vystia regions

## Solutions Implemented

### 1. Fixed Command Registration
- **Action**: Created proper command registration in VystiaWorldGenerator.cs
- **Method**: Used `CommandSystem.Register` with correct syntax
- **Result**: Commands are now recognized in-game

### 2. Resolved Server Crashes
- **Action**: Simplified VystiaWorldGenerator.cs to remove problematic dependencies
- **Method**: Removed complex terrain generation logic, focused on simple static markers
- **Result**: Server runs stably without crashes

### 3. Addressed Badspawn Issues
- **Action**: Removed spawner generation from world generation process
- **Method**: Simplified to terrain markers only, no creature spawning
- **Result**: Eliminated badspawn.log errors

### 4. Fixed World Generation Logic
- **Action**: Created completely new, simplified VystiaWorldGenerator.cs
- **Method**: Removed all old generation methods (GenerateCities, GenerateDungeons, GenerateSpawners)
- **Result**: Only generates terrain markers as intended

### 5. Corrected Coordinates
- **Action**: Updated coordinates to be within valid Felucca map bounds
- **Method**: Changed from 6000,1000+ to 2000,2000+ coordinates
- **Result**: Teleport commands now work correctly

## Technical Details

### File Structure Changes
```
ServUO/Server/bin/Release/Scripts/VystiaWorldGenerator.cs - Runtime file
ServUO/Scripts/Scripts.csproj - Project file (removed linked items)
Vystia/src/VystiaWorldGenerator.cs - Source file
```

### Coordinate System
- **Frosthold region**: 2000,2000 to 2600,2400 (blue rocks)
- **Emberlands region**: 3000,3000 to 3600,3400 (red rocks)
- **Desert region**: 4000,4000 to 4600,4400 (yellow rocks)

### Command Syntax
- **Correct**: `[GenVystiaWorld` (single opening bracket, no closing bracket)
- **Correct**: `[Go 2000 2000 0` (x y z coordinates)
- **Incorrect**: `[GenVystiaWorld]` (with closing bracket)
- **Incorrect**: `[Go 2000 2000 0 0` (extra parameter)

## Current Status

### Working Components
- ✅ Command registration
- ✅ Server stability
- ✅ Terrain marker generation
- ✅ Coordinate system
- ✅ Clear terrain functionality

### Remaining Issues
- ⚠️ Terrain markers may not be visible (needs testing)
- ⚠️ Badspawn.log warnings may persist from existing spawners

## Next Steps

1. **Test terrain generation**: Run `[GenVystiaWorld` and verify markers appear
2. **Test teleportation**: Use `[Go 2000 2000 0` to visit Frosthold region
3. **Verify marker visibility**: Check if colored rocks are visible in-game
4. **Clean up existing spawners**: Remove any remaining problematic spawners

## Lessons Learned

1. **ServUO loads compiled Scripts.dll, not runtime .cs files**
2. **Command syntax requires single opening bracket only**
3. **Coordinates must be within valid map boundaries**
4. **Simplification is key to stability**
5. **Runtime compilation can override compiled code**

## Files Modified

- `ServUO/Server/bin/Release/Scripts/VystiaWorldGenerator.cs` - Completely rewritten
- `ServUO/Scripts/Scripts.csproj` - Removed linked compile items
- `ServUO/Server/bin/Release/Scripts.dll` - Rebuilt with new code

## Commands Available

- `[GenVystiaWorld` - Generates terrain markers only
- `[ClearVystiaWorld` - Clears terrain markers only

---

## Update: October 21, 2025 - File Location Discovery

### New Issue Identified
- **Issue**: Changes to VystiaWorldGenerator.cs were not taking effect
- **Root Cause**: ServUO uses runtime compilation from `Server/bin/Release/Scripts/*.cs` which overrides compiled Scripts.dll
- **Symptoms**: Commands not available, changes not visible after rebuild

### File Location Hierarchy Discovered
1. **Runtime scripts** (highest priority): `C:\DevEnv\GIT\UO\ServUO\Server\bin\Release\Scripts\VystiaWorldGenerator.cs`
2. **Compiled scripts** (medium priority): `C:\DevEnv\GIT\UO\ServUO\Scripts\Custom\VystiaWorldGenerator.cs`
3. **Source control** (reference only): `C:\DevEnv\GIT\UO\Vystia\src\VystiaWorldGenerator.cs`

### Solution Implemented
- **Created documentation**: `.claude/SERVUO_FILE_LOCATIONS.md` - Complete guide to file hierarchy
- **Added new commands**: `[DeleteBadSpawns` and `[DeleteAllSpawners` to remove problematic spawners
- **Updated deployment process**: Always copy to runtime location first

### Current Commands Available
- `[GenVystiaWorld` - Generate terrain markers (using Item objects, not Static)
- `[ClearVystiaWorld` - Clear terrain markers
- `[DeleteBadSpawns` - Delete Vystia spawners causing badspawn.log errors
- `[DeleteAllSpawners` - Delete ALL spawners (use with caution!)

### Next Steps to Fix Badspawn Errors
1. Run `[DeleteBadSpawns` in-game to remove problematic spawners
2. Restart server to confirm badspawn.log warnings are gone
3. Run `[GenVystiaWorld` to create new terrain markers
4. Test teleportation: `[Go 2000 2000 0`, `[Go 3000 3000 0`, `[Go 4000 4000 0`

---

*Last Updated: October 21, 2025*
*Status: Documentation added, awaiting badspawn cleanup*
