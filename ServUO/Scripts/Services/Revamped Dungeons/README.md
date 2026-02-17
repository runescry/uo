# Revamped Dungeons Service

## Overview
The Revamped Dungeons service contains enhanced versions of classic dungeons with new encounters, mechanics, and rewards.

## Era
- **Expansion:** Various (SA, TOL)

## Files
| File | Description |
|------|-------------|
| `BlackthornDungeon/` | Blackthorn's dungeon (TOL) |
| `Covetous Void Spawn/` | Covetous void encounter (SA) |
| `DespiseRevamped/` | Despise dungeon update (SA) |
| `Shame Revamped/` | Shame dungeon update (SA) |
| `TheExodusEncounter/` | Exodus boss encounter (SA) |
| `WrongDungeon/` | Wrong dungeon update (TOL) |

## Dungeon Details

### Blackthorn Dungeon
**Era:** Time of Legends
**Location:** Trammel/Felucca
**Features:**
- Agent training system
- Unique creatures
- Blackthorn artifacts
- Point system rewards

### Covetous Void Spawn
**Era:** Stygian Abyss
**Features:**
- Void creature invasion
- Wave-based encounters
- Special loot tables
- Group content

### Despise Revamped
**Era:** Stygian Abyss
**Features:**
- Alignment system (good/evil)
- Wisp companions
- Crystal currency
- Faction-based gameplay

### Shame Revamped
**Era:** Stygian Abyss
**Features:**
- Crystal system
- Unique boss encounters
- Resource gathering
- Shame artifacts

### Exodus Encounter
**Era:** Stygian Abyss
**Features:**
- Exodus boss fight
- Key collection mechanic
- Unique artifacts
- Group coordination required

### Wrong Dungeon
**Era:** Time of Legends
**Features:**
- Prison theme
- Unique creatures
- Special loot
- Quest integration

## How it Works for Players

### Accessing Dungeons
1. Travel to dungeon entrance
2. Meet entry requirements
3. Navigate dungeon
4. Complete objectives

### Dungeon-Specific Mechanics
Each dungeon has unique systems:
- Point accumulation
- Key collection
- Alignment choices
- Boss encounters

## Configuration
Each dungeon has separate configuration:
```csharp
// Example: DespiseRevamped
public static bool Enabled = Core.SA;
public static TimeSpan RespawnDelay = TimeSpan.FromMinutes(30);
```

## GM Commands
```
[DespiseGen           - Generate Despise
[ShameGen             - Generate Shame
[ExodusGen            - Generate Exodus
[BlackthornGen        - Generate Blackthorn
```

## Rewards by Dungeon

### Despise Rewards
| Reward | Cost |
|--------|------|
| Wisp upgrades | Crystals |
| Artifacts | Points |
| Decorations | Crystals |

### Shame Rewards
| Reward | Cost |
|--------|------|
| Artifacts | Crystals |
| Resources | Crystals |
| Titles | Points |

### Blackthorn Rewards
| Reward | Cost |
|--------|------|
| Agent gear | Points |
| Recipes | Points |
| Artifacts | Points |

## FAQ

**Q: What expansion do I need?**
A: SA for most, TOL for Blackthorn/Wrong.

**Q: Are revamped dungeons harder?**
A: Generally yes, with better rewards.

**Q: Do I need a group?**
A: Some content requires groups.

**Q: How do I earn dungeon currency?**
A: Kill monsters, complete objectives.

**Q: Where do I spend rewards?**
A: Dungeon-specific NPCs.

## Related Systems
- Points Systems (`../PointsSystems/`)
- Champion System (`../ChampionSystem/`)
- Seasonal Events (`../Seasonal Events/`)
