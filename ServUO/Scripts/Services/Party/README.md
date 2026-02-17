# Party Service

## Overview
The Party system allows players to form groups for cooperative gameplay, sharing experience, communication, and coordination features.

## Era
- **Expansion:** All
- **Availability:** Always enabled

## Files
| File | Description |
|------|-------------|
| `Party.cs` | Core party logic |
| `PartyCommands.cs` | Party chat and commands |
| `PartyMemberInfo.cs` | Member data tracking |
| `AddPartyTarget.cs` | Target for adding members |
| `RemoveFromParty.cs` | Removal logic |
| `RemovePartyTarget.cs` | Target for removing members |
| `DeclineTimer.cs` | Invitation timeout |
| `Packets.cs` | Network packet definitions |

## Functionality
Parties enable group play with shared features and communication.

### Party Features
- Group communication (party chat)
- Shared looting options
- Stat bar sharing
- Damage contribution tracking
- Coordinate travel

### Party Limits
- Maximum members: 10
- Minimum members: 2
- Cross-facet parties allowed

## How it Works for Players

### Forming a Party
1. Target another player
2. Use party invite command/action
3. Target accepts invitation
4. Party formed

### Party Commands
| Command | Function |
|---------|----------|
| `/add` | Invite player |
| `/rem` | Remove player |
| `/quit` | Leave party |
| `/party` | Party chat |

### Communication
- Party chat visible to all members
- Chat prefix identifies party messages
- Works across distances

### Looting Options
Party leader can set:
- Free loot for all
- Leader distributes
- Round-robin

## Configuration
```csharp
// Party.cs
public class Party
{
    public static int Capacity = 10;
    public Mobile Leader { get; }
    public List<Mobile> Members { get; }
}

// PartyCommands.cs
public static void HandleCommand(Mobile from, string[] args)
```

## GM Commands
```
[party [player]         - Force add to party
[partyinfo [player]     - View party info
```

## Party Benefits
| Benefit | Description |
|---------|-------------|
| Communication | Private chat channel |
| Stat Bars | See member health |
| Coordination | Group targeting |
| Loot | Shared loot options |
| Credit | Damage credit sharing |

## Invitation Process
```
1. Player A invites Player B
2. Invitation sent, timer starts
3. Player B accepts/declines
4. Accept: Party formed/joined
   Decline: No party
   Timeout: Auto-decline
```

## FAQ

**Q: How do I invite someone?**
A: Use `/add` command or client party interface.

**Q: What's the party size limit?**
A: 10 members maximum.

**Q: Can I be in multiple parties?**
A: No, one party at a time.

**Q: How do I leave a party?**
A: Use `/quit` command.

**Q: Does party work across maps?**
A: Yes, party communication works across facets.

**Q: Who becomes leader if leader leaves?**
A: Next in list becomes leader automatically.

## Party Chat
- Prefix party messages with `/party` or designated key
- All members see message
- Works at any distance
- Logs may capture party chat

## Network Packets
Party uses specific packets for:
- Invitation
- Acceptance/decline
- Member updates
- Health bar sharing
- Chat messages

## Related Systems
- Chat (`../Chat/`) - General communication
- Guilds - Larger organization
- Champion System (`../ChampionSystem/`) - Party damage credit
