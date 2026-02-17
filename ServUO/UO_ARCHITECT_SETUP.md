# UO Architect Remote Server Setup Guide

## Installation Complete ✅

The UO Architect Remote Server has been installed in your ServUO server.

## Files Installed

1. **Scripts**: `Scripts/Custom/OrbRemoteServer/` - All UO Architect server scripts
2. **DLLs** (in ServUO root directory):
   - `OrbServerSDK.dll`
   - `UOArchitectInterface.dll`
   - `System.Runtime.Remoting.dll` (already existed)

## How It Works

The UO Architect Remote Server uses a **static constructor** pattern, which means:
- The `OrbServer` class automatically starts when ServUO loads the scripts
- The server listens on **port 2594** (TCP)
- No manual initialization is required - it starts automatically!

## Connecting from UO Architect

1. **Start your ServUO server** - The OrbServer will automatically start listening on port 2594
2. **Open UO Architect** (the client application)
3. **Configure connection**:
   - **Server**: `127.0.0.1` (or your server IP)
   - **Port**: `2594`
   - **Username**: Your ServUO account username
   - **Password**: Your ServUO account password
4. **Access Level Required**: GameMaster or higher

## Verification

When ServUO starts, you should see:
```
UO Architect Server initialized and listening on port 2594
```

If you see this message, the server is ready to accept connections.

## Troubleshooting

### Port Already in Use
If port 2594 is already in use, edit `Scripts/Custom/OrbRemoteServer/OrbServer.cs`:
```csharp
private static readonly int SERVER_PORT = 2594; // Change this to another port
```

### Firewall Issues
- Windows Firewall may block port 2594
- Add an exception for ServUO.exe or port 2594

### Connection Refused
- Ensure ServUO is running
- Check that the DLLs are in the ServUO root directory
- Verify your account has GameMaster or higher access level

### Script Errors
- Check the ServUO console for compilation errors
- Ensure all DLLs are present in the root directory
- Verify the script files are in `Scripts/Custom/OrbRemoteServer/`

## Features

UO Architect Remote Server enables:
- **Build Design**: Place buildings/structures from UO Architect
- **Extract Items**: Extract items from the game world
- **Select Items**: Select items in the game world
- **Delete Items**: Delete items remotely
- **Move Items**: Move items remotely
- **Get Location**: Get player location

## Security Notes

- Only accounts with **GameMaster** or higher access can connect
- The connection uses ServUO's account authentication
- All actions are logged in the ServUO console

## Next Steps

1. Start ServUO
2. Connect from UO Architect client
3. Begin building!
