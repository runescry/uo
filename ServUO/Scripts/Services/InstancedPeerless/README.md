# Instanced Peerless Service

## Overview
The Instanced Peerless system provides private boss encounters where groups can fight peerless bosses without competition from other players.

## Era
- **Expansion:** Stygian Abyss+ (`Core.SA`)
- **Bosses:** Medusa, Stygian Dragon

## Files
| File | Description |
|------|-------------|
| `PeerlessInstance.cs` | Instance management and lifecycle |
| `PeerlessKeyBrazier.cs` | Key activation brazier |
| `PeerlessPlatform.cs` | Entry platform structure |
| `InstanceEnterGate.cs` | Portal into instance |
| `InstanceExitGate.cs` | Portal out of instance |
| `InstanceRegion.cs` | Instance area region definition |
| `ConfirmJoinInstanceGump.cs` | Join confirmation dialog |
| `ConfirmExitInstanceGump.cs` | Exit confirmation dialog |
| `RejoinInstanceGump.cs` | Rejoin active instance dialog |
| `Medusa/` | Medusa boss specifics |
| `StygianDragon/` | Stygian Dragon boss specifics |

## Functionality
Unlike regular peerless which are in shared world space, instanced peerless create private copies of the boss area for each group.

### Instance System
1. Group collects required keys
2. Activate keys at brazier
3. Private instance created
4. Group enters via gate
5. Fight boss privately
6. Instance destroyed after completion

### Benefits
- No competition for boss
- No kill stealing
- Group-paced encounters
- Guaranteed loot distribution

## How it Works for Players

### Preparation
1. Form a group/party
2. Collect peerless keys (from dungeon creatures)
3. Travel to peerless entrance

### Starting the Instance
1. Use keys on the brazier
2. Instance begins creation
3. Enter gate within time limit
4. Gate closes once encounter starts

### During the Fight
1. Battle the peerless boss
2. Instance is private to your group
3. Death allows rejoining
4. Defeat boss for loot

### After Completion
1. Collect loot from boss
2. Exit via exit gate
3. Instance destroys itself
4. Cooldown before next attempt

## Medusa Encounter

### Keys Required
Collected from Medusa's Lair creatures

### Mechanics
- Petrification gaze attack
- Spawns minion creatures
- Stone statues come alive

### Rewards
- Medusa's Blood (resource)
- Tangle (weapon)
- Medusa Floor Tile
- Various artifacts

## Stygian Dragon Encounter

### Keys Required
Collected from Abyss creatures

### Mechanics
- Breath attack
- Tail sweep
- Spawns drakes

### Rewards
- Dragon's Blood (resource)
- Dragon artifact set pieces
- Various artifacts

## Configuration
```csharp
// PeerlessInstance.cs
public class PeerlessInstance
{
    public TimeSpan InstanceDuration { get; set; }
    public TimeSpan GracePeriod { get; set; }
    public int MaxPlayers { get; set; }
}

// InstanceRegion.cs
public class InstanceRegion : BaseRegion
{
    // Defines instance boundaries
}
```

## GM Commands
```
[add PeerlessKeyBrazier
[add InstanceEnterGate
[InstanceAdmin            - Manage active instances
```

## Instance Lifecycle
```
1. Keys Used → Instance Creating (30 sec)
2. Instance Ready → Gate Opens (2 min to enter)
3. Combat Active → Fight Boss
4. Boss Defeated → Loot Period (10 min)
5. Exit/Timeout → Instance Destroyed
```

## FAQ

**Q: How many keys do I need?**
A: Varies by encounter, typically one per party member.

**Q: Can I rejoin if I die?**
A: Yes, use the rejoin option at the entrance.

**Q: What happens if everyone dies?**
A: Instance continues until timeout or you rejoin and complete.

**Q: Is loot shared?**
A: Loot goes to top damage dealers, similar to regular peerless.

**Q: How long does the instance last?**
A: Typically 1-2 hours maximum.

**Q: Can others join my instance?**
A: Only party/group members who were present at creation.

## Instance States
| State | Description |
|-------|-------------|
| Creating | Instance being generated |
| Ready | Gate open, awaiting players |
| Active | Combat in progress |
| Completed | Boss defeated, looting |
| Expired | Instance being destroyed |

## Related Systems
- Peerless (`../Peerless/`) - Regular peerless encounters
- Party (`../Party/`) - Group formation
- Underworld (`../Underworld/`) - SA dungeon content
