# UO Architect Client - Quick Start Guide

## Prerequisites

✅ **ServUO server is running** and you see:
```
UO Architect Server for RunUO 2.0 is listening on port 2594
UO Architect Server initialized and listening on port 2594
```

✅ **You have a GameMaster account** on your ServUO server

## Step 1: Launch UO Architect Client

1. Navigate to: `D:\UO\UO Architect\UO Architect v2.7\Client\`
2. Run: **`UO Architect.exe`**

## Step 2: Connect to Your Server

When UO Architect opens, you'll need to configure the connection:

### Connection Settings:
- **Server IP**: `127.0.0.1` (if running locally) or your server's IP address
- **Port**: `2594`
- **Username**: Your ServUO account username (must be GameMaster or higher)
- **Password**: Your ServUO account password

### How to Connect:
1. Look for a **"Connect"** or **"Server Settings"** button/menu in UO Architect
2. Enter the connection details above
3. Click **Connect**

**Note**: If you don't see connection options immediately, check:
- **File → Server Settings** or **File → Connect**
- Look for a toolbar button with a server/network icon
- Check the menu bar for "Connection" or "Server" options

## Step 3: Verify Connection

When connected successfully, you should see:
- A status message indicating you're connected
- In ServUO console: `{your_username} connected to the Orb Script Server`

## Step 4: Using UO Architect

### Option A: Load a Pre-built Design

1. **File → Open Design** (or similar menu option)
2. Browse to: `D:\UO\UO Architect\Buildings and Structures\` or `D:\UO\UO Architect\Library\`
3. Select a `.uoa` file (e.g., a castle, house, arena)
4. Click **Build** or **Place** button
5. In your UO client, you'll see a targeting cursor
6. Target the ground where you want the structure placed
7. The structure will be built at that location!

### Option B: Create Your Own Design

1. Use the **toolbox** in UO Architect to add items:
   - Walls, floors, doors
   - Decorations, furniture
   - Stairs, roofs
2. Arrange items to create your structure
3. **File → Save Design** to save as `.uoa` file
4. Use **Build** to place it in-game

### Option C: Extract Items from Game

1. Use UO Architect's **Extract** tool
2. Select items in the game world
3. Save the extracted design as a `.uoa` file
4. Modify and place it elsewhere

## Available Pre-built Designs

You have hundreds of designs available:

- **`Buildings and Structures/`** - Organized by type (Castles, Houses, Dungeons, etc.)
- **`Library/`** - Organized categories:
  - Arenas, Banks, Bridges, Craft Shops
  - Docks and Ships, Dungeons, Farms
  - Guilds, Houses, Inns, Towers
  - And many more!

## Troubleshooting

### "Connection Refused" or "Cannot Connect"
- ✅ Ensure ServUO is running
- ✅ Check that you see "listening on port 2594" in ServUO console
- ✅ Verify your account has GameMaster access level
- ✅ Check Windows Firewall isn't blocking port 2594
- ✅ Try `127.0.0.1` instead of `localhost`

### "Not Authorized" Error
- Your account needs **GameMaster** or higher access level
- Check your account access level in ServUO: `[Admin` command or check `Accounts.cfg`

### Items Don't Appear When Building
- Check ServUO console for errors
- Verify the area isn't blocked
- Some item IDs in old designs may not exist in your server version

### Client Won't Start
- Ensure you have all DLLs in the Client folder:
  - `OrbServerSDK.dll`
  - `UOArchitectInterface.dll`
  - `Ultima.dll`
- Try running as Administrator

## Quick Reference

| Action | Location |
|--------|----------|
| **Client Executable** | `D:\UO\UO Architect\UO Architect v2.7\Client\UO Architect.exe` |
| **Server Port** | `2594` |
| **Pre-built Designs** | `D:\UO\UO Architect\Buildings and Structures\` or `Library\` |
| **Required Access** | GameMaster or higher |

## Next Steps

1. ✅ Connect to your server
2. ✅ Try loading a small design (like a simple house)
3. ✅ Place it in a test area
4. ✅ Explore the pre-built designs library
5. ✅ Start building your custom world!

Happy building! 🏰
