# Remote Admin Service

## Overview
The Remote Admin service provides network-based server administration capabilities, allowing authorized administrators to manage the server remotely.

## Era
- **Expansion:** All
- **Availability:** Administrative feature

## Files
| File | Description |
|------|-------------|
| `Network.cs` | Network connection handling |
| `PacketHandlers.cs` | Admin packet processing |
| `Packets.cs` | Admin protocol packets |
| `RemoteAdminLogging.cs` | Action logging |

## Functionality
Enables remote server management through a secure network protocol.

### Features
- Remote server control
- Account management
- World management
- Player administration
- Log viewing

### Security
- Authentication required
- Encrypted connections
- Action logging
- Access level restrictions

## How it Works

### Connection
1. Admin connects via admin client
2. Authentication challenge sent
3. Credentials verified
4. Session established

### Commands
Remote admin supports:
- Server status queries
- Player kicks/bans
- Account modifications
- World saves
- Server shutdown

## Configuration
```csharp
// Network.cs
public static class RemoteAdmin
{
    public static bool Enabled = false;
    public static int Port = 2594;
    public static string[] AllowedIPs = { };
}
```

## Admin Levels
| Level | Permissions |
|-------|-------------|
| Basic | View status |
| Elevated | Player management |
| Full | All operations |

## Protocol
Admin protocol uses custom packets:
- Authentication packets
- Command packets
- Response packets
- Status packets

## Logging
All actions logged:
- Login attempts
- Commands executed
- Changes made
- Errors encountered

## FAQ

**Q: Is remote admin secure?**
A: Yes, with proper configuration and IP restrictions.

**Q: What port does it use?**
A: Default 2594, configurable.

**Q: Can I restrict by IP?**
A: Yes, whitelist allowed IPs.

**Q: Are actions logged?**
A: Yes, comprehensive logging available.

**Q: What client is needed?**
A: Compatible admin client application.

## Security Best Practices
1. Use strong passwords
2. Restrict allowed IPs
3. Monitor logs regularly
4. Use encrypted connections
5. Limit admin accounts

## Available Operations
| Operation | Description |
|-----------|-------------|
| Status | Server status |
| Players | Online player list |
| Kick | Remove player |
| Ban | Ban account |
| Save | World save |
| Shutdown | Server shutdown |

## Related Systems
- Account System
- Logging System
- Server Console
- Help System (`../Help/`)
