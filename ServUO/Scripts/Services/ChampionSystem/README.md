# Champion System Service

## Overview
The Champion System provides large-scale PvM encounters where players fight through waves of creatures to summon and defeat powerful Champion bosses, earning Power Scrolls and valuable loot.

## Era
- **Expansion:** Age of Shadows+ (`Core.AOS`)
- **Enabled Check:** `Config.Get("Champions.Enabled", true)`

## Files
| File | Description |
|------|-------------|
| `ChampionSystem.cs` | Core system logic, rotation, and scheduling |
| `ChampionSpawn.cs` | Individual spawn controller |
| `ChampionSpawnType.cs` | Champion type definitions |
| `ChampionAltar.cs` | Altar where champions are summoned |
| `ChampionPlatform.cs` | Physical platform structure |
| `ChampionSkull.cs` | Skull drops from champions |
| `ChampionSkullBrazier.cs` | Brazier to place skulls |
| `ChampionSkullPlatform.cs` | Platform for skull braziers |
| `ChampionSkullType.cs` | Skull type enumeration |
| `GoldShower.cs` | Gold rain effect on champion death |
| `HarrowerGate.cs` | Portal to Harrower encounter |
| `RestartTimer.cs` | Spawn restart timing logic |
| `SliceTimer.cs` | Combat slice timing |
| `StarRoomGate.cs` | Teleporter to star room |
| `ChampionSpawnController/` | Advanced spawn control |

## Functionality
Champion Spawns are world events where players gather to fight escalating waves of monsters, ultimately summoning a Champion boss.

### Spawn Mechanics
1. **Level 1-4** - Kill creatures to advance the spawn
2. **Red Candles** - Visual progress indicator on altar
3. **Champion Appears** - After level 4 completion
4. **Loot Distribution** - Based on damage contribution

### Champion Types
| Champion | Location | Skull Color |
|----------|----------|-------------|
| Barracoon the Piper | Despise | Blue |
| Mephitis | Terathan Keep | Green |
| Neira the Necromancer | Deceit | Purple |
| Rikktor | Destard | Orange |
| Semidar | Fire Dungeon | Red |
| Lord Oaks | Twisted Weald | Brown |
| Serado the Awakened | Isamu-Jima | Gold |
| Ilhenir the Stained | Bedlam | Turquoise |
| Twaulo of the Glade | Prism of Light | White |
| Primeval Lich | Abyss | Crimson |

## How it Works for Players

### Finding a Spawn
1. Travel to Felucca facet (required for spawns)
2. Locate an active champion altar
3. Red skull icons indicate spawn progress

### Fighting the Spawn
1. Kill monsters in the spawn area
2. Progress shown by candles on altar
3. Stay in area to contribute
4. Higher-level monsters spawn as progress increases

### Level Progression
| Level | Creature Power | Candle Count |
|-------|---------------|--------------|
| 1 | Low | 4 candles |
| 2 | Medium | 8 candles |
| 3 | High | 12 candles |
| 4 | Very High | 16 candles |
| Boss | Champion | All lit |

### Claiming Rewards
- **Damage-based** - Top damage dealers get scrolls
- **Gold Shower** - 50 piles of 4000-5500 gold
- **Power Scrolls** - +5, +10, +15, +20, +25 skill caps
- **Stat Scrolls** - Increase stat caps
- **Champion Skull** - Used for Harrower summoning

## Configuration
```csharp
// ChampionSystem.cs
public static bool Enabled = Config.Get("Champions.Enabled", true);
public static TimeSpan RotateDelay = Config.Get("Champions.RotateDelay", TimeSpan.FromDays(1));
public static bool StatScrollsEnabled = Config.Get("Champions.StatScrolls", true);
public static int GoldRange = Config.Get("Champions.GoldRange", 200);
```

### Config File Options
```
Champions.Enabled = true
Champions.RotateDelay = 1.00:00:00
Champions.StatScrolls = true
Champions.GoldRange = 200
```

## GM Commands
```
[ChampionInfo          - View all spawn status
[ChampionSpawnEdit     - Edit spawn properties
[add ChampionSpawn     - Place new spawn
[add ChampionAltar     - Place altar
```

## Harrower Encounter
Collecting all champion skulls enables the Harrower summoning:
1. Collect skulls from each champion type
2. Place skulls on ChampionSkullBraziers
3. Complete set opens HarrowerGate
4. Defeat the Harrower for ultimate rewards

## FAQ

**Q: Why can't I find any champion spawns?**
A: Spawns only occur in Felucca. Check for active altar locations.

**Q: How are Power Scrolls distributed?**
A: Top damage dealers receive scrolls. Being in a party shares credit.

**Q: Can I solo a champion spawn?**
A: Possible with strong characters, but designed for groups.

**Q: How often do spawns rotate?**
A: Default is every 24 hours (`Champions.RotateDelay`).

**Q: What happens if I die during the spawn?**
A: You can return and continue contributing to damage totals.

**Q: Do scrolls drop for everyone?**
A: No, limited number of scrolls distributed to top contributors.

## Spawn Rotation
The system rotates active spawns periodically:
- Not all spawns active simultaneously
- Rotation creates variety
- GM can force rotation with commands

## Loot Tables
| Reward | Drop Rate | Notes |
|--------|-----------|-------|
| Gold Shower | 100% | 50 piles around altar |
| Power Scrolls | Variable | Based on contribution |
| Stat Scrolls | Variable | If enabled |
| Champion Skull | 100% | One per champion |
| Artifacts | Rare | Champion-specific |

## Related Systems
- Factions (`../Factions/`) - Faction players can claim spawns
- Points Systems (`../PointsSystems/`)
- Harrower Encounter
