# Spawner Baseline Configuration Documentation

## Overview

This document describes the baseline configuration for Spawner attributes used in world creation, recreation, and deletion scripts. 

**Important**: The baseline can be set in two ways:
1. **Code Defaults**: Based on default values from the Spawner constructor and InitSpawner method (initial state)
2. **World State**: Calculated by scanning existing spawners in the current world (recommended for accuracy)

By default, the baseline uses code defaults. You can update it to reflect your actual world state using the `UpdateSpawnerBaseline` command.

## Purpose

The baseline configuration ensures consistency across all world generation scripts (CreateWorld, RecreateWorld, DeleteWorld) by providing documented default values for all spawner attributes.

## Baseline Attributes

All baseline values are defined in `SpawnerBaseline.cs` and can be accessed via `SpawnerBaseline.Defaults`. The following attributes are currently supported:

### Timing Attributes

- **MinDelay** (TimeSpan)
  - Default: `TimeSpan.FromMinutes(5)`
  - Description: Minimum time between spawns
  - Decoration Parameter: `MinDelay=<TimeSpan>`

- **MaxDelay** (TimeSpan)
  - Default: `TimeSpan.FromMinutes(10)`
  - Description: Maximum time between spawns
  - Decoration Parameter: `MaxDelay=<TimeSpan>`

- **NextSpawn** (TimeSpan)
  - Default: `TimeSpan.Zero`
  - Description: Time until next spawn (immediate when created via decoration)
  - Decoration Parameter: `NextSpawn=<TimeSpan>`

### Spawn Control Attributes

- **MaxCount** (int)
  - Default: `1`
  - Description: Maximum number of creatures to spawn
  - Decoration Parameter: `Count=<int>`
  - Maps to: `Spawner.MaxCount`

- **SpawnObjects** (List<SpawnObject>)
  - Default: Empty list (must be populated)
  - Description: List of creatures/items to spawn
  - Decoration Parameter: `Spawn=<creature name>`
  - Note: Multiple Spawn parameters can be specified

### Range Attributes

- **HomeRange** (int)
  - Default: `5`
  - Description: Home range for spawned creatures
  - Decoration Parameter: `HomeRange=<int>`

- **SpawnRange** (int)
  - Default: `4`
  - Description: Range within which spawns can occur
  - Note: Set via constructor, not decoration parameter

- **WalkingRange** (int)
  - Default: `-1`
  - Description: Walking range for spawned creatures (-1 uses HomeRange instead)
  - Note: Set in InitSpawner, not decoration parameter

### Behavior Attributes

- **Running** (bool)
  - Default: `true`
  - Description: Whether spawner is active
  - Decoration Parameter: `Running=<bool>`

- **Group** (bool)
  - Default: `false`
  - Description: Group spawn mode (true = spawn all at once, false = spawn individually)
  - Decoration Parameter: `Group=<bool>`

- **Team** (int)
  - Default: `0`
  - Description: Team number for spawned creatures
  - Decoration Parameter: `Team=<int>`

- **GuardImmune** (bool)
  - Default: `false`
  - Description: Whether spawned creatures are immune to guards
  - Note: Not currently set via decoration files, but available

## Usage

### Updating Baseline from World State (Recommended)

To set the baseline based on actual spawners in your world:

```
[UpdateSpawnerBaseline [mode|median|mean] [apply]
```

Examples:
- `UpdateSpawnerBaseline` - Scan and show results using mode (most common value)
- `UpdateSpawnerBaseline mode apply` - Scan using mode and update defaults
- `UpdateSpawnerBaseline median apply` - Scan using median and update defaults
- `UpdateSpawnerBaseline mean apply` - Scan using mean/average and update defaults

**Calculation Modes:**
- **Mode** (default): Uses the most common value found across all spawners
- **Median**: Uses the middle value when sorted
- **Mean**: Uses the average value

### Applying Baseline to a Spawner

```csharp
Spawner spawner = new Spawner();
SpawnerBaseline.ApplyBaseline(spawner);
```

Or via CreateWorld helper:

```csharp
CreateWorld.ApplySpawnerBaseline(spawner);
```

### Getting Baseline Summary

```csharp
string summary = SpawnerBaseline.GetBaselineSummary();
// or
string summary = CreateWorld.GetSpawnerBaselineSummary();
```

### Scanning World State Programmatically

```csharp
// Scan world and get results without applying
var result = SpawnerBaseline.CalculateFromWorldState(SpawnerBaseline.CalculationMode.Mode);
Console.WriteLine(result.ToString());

// Apply the calculated values to defaults
SpawnerBaseline.ApplyWorldStateBaseline(result);
```

### Customizing Baseline Values

To manually change the baseline defaults, modify the properties in `SpawnerBaseline.Defaults`:

```csharp
SpawnerBaseline.Defaults.MinDelay = TimeSpan.FromMinutes(10);
SpawnerBaseline.Defaults.MaxCount = 5;
```

## Decoration File Format

When creating spawners via decoration files (`.cfg`), use the following format:

```
Server.Mobiles.Spawner 0x1f13 (Spawn=Orc;MinDelay=00:05:00;MaxDelay=00:10:00;Count=3;Team=0;HomeRange=5;Running=true;Group=false)
X Y Z
```

### Parameter Format

- Parameters are separated by semicolons (`;`)
- Format: `ParameterName=Value`
- Boolean values: `true` or `false`
- TimeSpan values: `HH:MM:SS` format (e.g., `00:05:00` for 5 minutes)
- Integer values: Numeric (e.g., `5`, `10`)

## Files Modified

1. **SpawnerBaseline.cs** (NEW)
   - Defines baseline configuration class with all documented attributes
   - Provides `ApplyBaseline()` method for applying defaults
   - Provides `GetBaselineSummary()` for documentation

2. **Decorate.cs** (UPDATED)
   - Added comprehensive documentation comments for each spawner attribute
   - References SpawnerBaseline.cs for baseline documentation

3. **CreateWorld.cs** (UPDATED)
   - Added reference to SpawnerBaseline.cs
   - Added helper methods: `ApplySpawnerBaseline()` and `GetSpawnerBaselineSummary()`
   - Added class-level documentation

## Related Files

- `ServUO/Scripts/Services/Spawner/Spawner.cs` - Spawner implementation
- `ServUO/Scripts/Commands/Decorate.cs` - Decoration file parsing
- `ServUO/Scripts/Commands/CreateWorld.cs` - World generation commands

## Notes

- **Initial State**: The baseline values start as code defaults from the Spawner constructor
- **World State**: Use `UpdateSpawnerBaseline` command to calculate baseline from actual spawners in your world
- **Override**: Values can be overridden in decoration files using the appropriate parameters
- **Consistency**: The baseline ensures consistency when spawners are created programmatically
- **Precision**: All timing values use `TimeSpan` for precision
- **NextSpawn**: The `NextSpawn` is set to `TimeSpan.Zero` by default to allow immediate spawning when created via decoration
- **Recommended Workflow**: 
  1. Run `UpdateSpawnerBaseline mode apply` to set baseline from your current world
  2. Use this baseline for all future world generation scripts
  3. Re-run the command if you make significant changes to spawner configurations

