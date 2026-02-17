# Treasure Maps Service

## Overview
The Treasure Maps system allows players to find, decode, and dig up buried treasure chests containing gold and items.

## Era
- **Expansion:** All (HS: Enhanced)

## Files
| File | Description |
|------|-------------|
| `TreasureMap.cs` | Core treasure map item |
| `TreasureMapChest.cs` | Buried chest container |
| `TreasureMapInfo.cs` | Map level and location data |
| `TreasureMapProtection.cs` | Chest guardian system |
| `BagOfGems.cs` | Gem reward container |

## Functionality
Players decode treasure maps, travel to locations, dig up chests, and defeat guardians for valuable loot.

### Map Levels
| Level | Difficulty | Loot Quality |
|-------|------------|--------------|
| 1 (Plainly Drawn) | Easy | Basic |
| 2 (Expertly Drawn) | Medium | Moderate |
| 3 (Adeptly Drawn) | Hard | Good |
| 4 (Cleverly Drawn) | Very Hard | Great |
| 5 (Deviously Drawn) | Expert | Excellent |
| 6 (Ingeniously Drawn) | Master | Best |

### Map Types (HS+)
- Standard treasure maps
- Stash maps (High Seas)
- Supply maps (High Seas)

## How it Works for Players

### Obtaining Maps
1. Kill monsters (random drop)
2. Fish from water
3. Trade with players
4. Quest rewards

### Decoding Maps
1. Use Cartography skill on map
2. Skill check based on map level
3. Success reveals location
4. Failure may damage map

### Finding Location
1. View decoded map
2. Match terrain to map image
3. Travel to location
4. Center on exact spot

### Digging Up Treasure
1. Use shovel at location
2. Multiple dig attempts may be needed
3. Chest emerges from ground
4. Guardians spawn

### Defeating Guardians
1. Monsters spawn to protect chest
2. Defeat all guardians
3. Chest unlocks
4. Claim treasure

## Configuration
```csharp
// TreasureMapInfo.cs
public class TreasureMapInfo
{
    public int Level { get; set; }
    public double MinSkill { get; set; }
    public Type[] GuardianTypes { get; set; }
    public int ChestGold { get; set; }
}

// TreasureMap.cs
public class TreasureMap : MapItem
{
    public int Level { get; set; }
    public bool Completed { get; set; }
    public Point2D ChestLocation { get; set; }
}
```

## GM Commands
```
[add TreasureMap [level]
[add TreasureMapChest
```

## Skill Requirements
| Level | Cartography | Lockpicking |
|-------|-------------|-------------|
| 1 | 0 | 0 |
| 2 | 41 | 41 |
| 3 | 51 | 51 |
| 4 | 61 | 61 |
| 5 | 71 | 71 |
| 6 | 81 | 81 |

## Chest Contents
| Content | Level Scaling |
|---------|---------------|
| Gold | Increases with level |
| Gems | More at higher levels |
| Magic Items | Better at higher levels |
| Scrolls | Higher circle scrolls |
| Reagents | Larger quantities |

## Guardian Types
| Level | Guardians |
|-------|-----------|
| 1-2 | Skeletons, zombies |
| 3-4 | Liches, mummies |
| 5-6 | Dragons, balrons |

## FAQ

**Q: Where do maps drop?**
A: Most monsters, fishing, and some quests.

**Q: Can maps fail to decode?**
A: Yes, and multiple failures may destroy the map.

**Q: How do I find the location?**
A: Match the map image to the game terrain.

**Q: What if I can't find it?**
A: Look for landmarks, try different angles.

**Q: Can I dig without decoding?**
A: No, must decode first.

**Q: What's the best map level?**
A: Higher is better loot but harder guardians.

## High Seas Additions
- Stash maps (smaller, easier)
- Supply maps (ship supplies)
- Sea creature drops
- Underwater treasures

## Map Decay
- Undecoded maps don't decay
- Decoded maps may have time limits
- Completed maps can be discarded

## Related Systems
- Cartography skill
- Lockpicking skill
- Fishing (`../Harvest/Fishing.cs`)
- Loot Generation (`../LootGeneration/`)
