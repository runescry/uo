# Peerless Service

## Overview
The Peerless system manages high-tier boss encounters that require special keys to access, offering the best loot in the game including unique artifacts.

## Era
- **Expansion:** Mondain's Legacy+ (`Core.ML`)

## Files
| File | Description |
|------|-------------|
| `BasePeerless.cs` | Base peerless boss class |
| `PeerlessAltar.cs` | Altar for key activation |
| `PeerlessKey.cs` | Access key items |
| `MasterKey.cs` | Combined multi-key item |
| `PeerlessTeleporter.cs` | Teleporter to boss area |
| `ConfirmGumps.cs` | Confirmation dialogs |

## Functionality
Peerless are the most powerful bosses in UO, requiring group coordination and specific keys to access.

### Peerless Bosses
| Boss | Location | Expansion |
|------|----------|-----------|
| Dread Horn | Twisted Weald | ML |
| Lady Melisande | Blighted Grove | ML |
| Chief Paroxysmus | Palace of Paroxysmus | ML |
| Shimmering Effusion | Prism of Light | ML |
| Monstrous Interred Grizzle | The Citadel | ML |
| Travesty | Labyrinth | ML |
| Medusa | Stygian Abyss | SA |
| Stygian Dragon | Stygian Abyss | SA |

### Key System
Each peerless requires multiple keys:
- Keys drop from dungeon monsters
- Multiple keys needed per attempt
- Keys are consumed on use
- Master Keys combine multiple

## How it Works for Players

### Obtaining Keys
1. Enter the peerless dungeon
2. Kill monsters for key drops
3. Collect all required keys
4. Combine or use at altar

### Activating the Encounter
1. Travel to peerless altar
2. Use required keys on altar
3. Portal opens to boss area
4. Time limit to enter

### Fighting the Peerless
1. Enter boss arena
2. Fight powerful boss
3. Handle special mechanics
4. Defeat for loot

### Loot Distribution
- Based on damage dealt
- Top contributors get artifacts
- Gold distributed to all
- Corpse has additional loot

## Configuration
```csharp
// BasePeerless.cs
public abstract class BasePeerless : BaseCreature
{
    public override bool AutoDispel { get; }
    public virtual double ChangeCombatant { get; }
}

// PeerlessAltar.cs
public class PeerlessAltar : Item
{
    public Type[] RequiredKeys { get; }
    public int KeysRequired { get; }
}
```

## GM Commands
```
[add [PeerlessName]
[add PeerlessAltar
[add PeerlessKey
```

## Key Requirements (Examples)
| Boss | Keys Needed |
|------|-------------|
| Dread Horn | 3 Corruption keys |
| Chief Paroxysmus | 4 Paroxysmus keys |
| Medusa | Varies by party |

## Peerless Mechanics
Each boss has unique mechanics:
- **Dread Horn** - Area attacks, summons
- **Medusa** - Petrification, gaze
- **Stygian Dragon** - Breath, flight

## Loot Tables
| Loot Type | Drop Source |
|-----------|-------------|
| Artifacts | Boss corpse |
| Gold | Gold shower |
| Ingredients | Boss corpse |
| Recipes | Boss corpse |

### Notable Artifacts
- Peerless-specific artifacts
- Replica artifacts
- Ingredients for crafting
- Rare recipes

## FAQ

**Q: How many keys do I need?**
A: Varies by peerless, typically 3-6.

**Q: Can I solo a peerless?**
A: Very difficult, designed for groups.

**Q: How long does the portal stay open?**
A: Limited time, usually a few minutes.

**Q: What if I die inside?**
A: Can be resurrected; instance may timeout.

**Q: How is loot distributed?**
A: Top damage dealers get artifacts, everyone gets something.

**Q: Can I farm keys?**
A: Yes, dungeon monsters drop keys regularly.

## Boss Arena
Each peerless has dedicated arena:
- Teleport in via portal
- Sealed during fight
- Exit after completion
- Arena resets

## Related Systems
- Instanced Peerless (`../InstancedPeerless/`) - Private versions
- Champion System (`../ChampionSystem/`) - Similar boss concept
- Loot Generation (`../LootGeneration/`) - Artifact drops
