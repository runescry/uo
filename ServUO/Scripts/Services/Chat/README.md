# Chat Service

## Overview
The Chat system provides global and custom chat channel functionality, allowing players to communicate across the game world.

## Era
- **Expansion:** All
- **Enabled Check:** Always enabled (core feature)

## Files
| File | Description |
|------|-------------|
| `ChatSystem.cs` | Core chat logic and channel management |
| `Channel.cs` | Individual channel definitions |
| `ChatActionHandler.cs` | Base action handler class |
| `ChatActionHandlers.cs` | Specific action implementations |
| `ChatCommand.cs` | Chat command parsing |
| `ChatUser.cs` | User state and preferences |
| `Logging.cs` | Chat message logging |
| `Packets.cs` | Network packet definitions |

## Functionality
The chat system enables server-wide and custom channel communication beyond local speech.

### Channel Types
- **Global Chat** - Server-wide communication
- **Custom Channels** - Player-created topic channels
- **Guild Chat** - Guild member communication
- **Party Chat** - Party member communication

### Chat Features
- Create/join/leave channels
- Channel moderation
- Ignore lists
- Chat logging

## How it Works for Players

### Accessing Chat
1. Open the chat window (default: varies by client)
2. Select a channel or create new
3. Type messages to broadcast

### Channel Commands
| Command | Function |
|---------|----------|
| `/join [channel]` | Join a channel |
| `/leave [channel]` | Leave a channel |
| `/create [channel]` | Create new channel |
| `/kick [user]` | Kick user (moderator) |
| `/ban [user]` | Ban user (moderator) |

### Channel Moderation
- Channel creators become moderators
- Moderators can kick/ban users
- Password protection available
- Voice control for ordered discussions

## Configuration
```csharp
// ChatSystem.cs
public static void Initialize()
{
    // Register chat packet handlers
    // Configure default channels
}

// Logging configuration
public static void Log(string channel, string username, string message)
```

## GM Commands
```
[chat broadcast [message]  - Server broadcast
[chat create [name]        - Create channel
[chat delete [name]        - Delete channel
```

## Chat Packet Structure
```csharp
// Incoming chat packet
public sealed class ChatMessagePacket : Packet
{
    // Channel, sender, message data
}

// Outgoing chat packet
public sealed class ChatJoinPacket : Packet
{
    // User join notification
}
```

## FAQ

**Q: How do I open the chat window?**
A: Client-dependent. Check your client's keybindings or menu.

**Q: Can I be in multiple channels?**
A: Yes, you can join multiple channels simultaneously.

**Q: Are chat messages logged?**
A: Server-side logging is configurable by administrators.

**Q: How do I ignore someone in chat?**
A: Use the ignore command or client-side ignore function.

**Q: Can I create private channels?**
A: Yes, create a channel and set a password for privacy.

## Action Handlers
The system supports these actions:
- **ChangeChannelPassword** - Set/change channel password
- **AddIgnore** - Add user to ignore list
- **RemoveIgnore** - Remove from ignore list
- **EmptyChannel** - Clear all users from channel

## Logging System
Chat logging captures:
- Channel name
- Username
- Timestamp
- Message content
- Action type (message, join, leave, etc.)

## Related Systems
- Party System (`../Party/`)
- Guild System
- Help System (`../Help/`)
