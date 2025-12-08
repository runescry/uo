# Vystia Map 9 Setup Guide

## Overview

Map 9 (Vystia) requires map files in **TWO locations**:
1. **Server**: Loads from `C:\Vystia_Map` (already configured)
2. **Client**: Needs files in the UO client's data directory

## Required Files

- `map9.mul` - Terrain data
- `staidx9.mul` - Static items index
- `statics9.mul` - Static items data

## Setup Steps

### Step 1: Verify Server Configuration

The server is already configured to load map files from:
```
C:\Vystia_Map\map9.mul
C:\Vystia_Map\staidx9.mul
C:\Vystia_Map\statics9.mul
```

### Step 2: Copy Files to Client Directory

**Option A: Use the Server Command (Recommended)**
```
[CopyVystiaToClient
```
This will copy files to the first configured DataDirectory.

Or specify a path:
```
[CopyVystiaToClient "C:\Program Files (x86)\Electronic Arts\Ultima Online Classic"
```

**Option B: Manual Copy**

Copy these files from `C:\Vystia_Map\` to your UO client directory:
- `map9.mul`
- `staidx9.mul`  
- `statics9.mul`

Typical client directory locations:
- `C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\`
- `C:\Program Files\Electronic Arts\Ultima Online Classic\`
- Or wherever your UO client is installed

### Step 3: Restart Client

**IMPORTANT**: You must restart your UO client after copying the files for them to be recognized.

### Step 4: Verify Map is Working

Use these commands in-game:
```
[TestMap Vystia          - Check if map is registered
[VystiaDiagnostics       - Full diagnostic information
[FacetGump               - Should show Vystia in the list
[Go Vystia               - Teleport to Vystia
```

## Troubleshooting

### Map Not Appearing in World Map Gump

The **client's world map gump** (Alt+M) is client-side and may not show Map 9 by default. This is a client limitation. However:
- The server-side `[FacetGump` command WILL show Vystia
- The `[Go Vystia` command WILL work
- Players CAN teleport to and play on Map 9

### Map Files Not Found

1. Check that files exist in `C:\Vystia_Map\`
2. Check that files exist in client directory
3. Run `[VystiaDiagnostics` to see file paths
4. Verify file permissions (files should be readable)

### Client Can't See Map

1. Ensure files are in the correct client directory
2. Restart the client completely
3. Check client logs for map loading errors
4. Some clients may need map9 files in a specific subdirectory

## Notes

- The server loads map files from `C:\Vystia_Map` (custom location)
- The client loads map files from its own installation directory
- Both need the same map files for proper functionality
- The client's world map gump may not show Map 9 (client limitation)
- Server-side commands and gumps will work correctly

