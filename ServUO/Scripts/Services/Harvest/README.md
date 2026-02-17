# Harvest Service

## Overview
The Harvest system manages all resource gathering activities including Mining, Lumberjacking, and Fishing - the three primary methods for obtaining raw materials.

## Era
- **Expansion:** All (with era-specific enhancements)
- **Skills:** Mining, Lumberjacking, Fishing

## Files
| File | Description |
|------|-------------|
| `HarvestSystem.cs` | Core harvest framework |
| `Mining.cs` | Mining skill implementation |
| `Lumberjacking.cs` | Lumberjacking skill implementation |
| `Fishing.cs` | Fishing skill implementation |
| `Core/` | Harvest system core classes |

## Functionality
Players use tools to gather resources from the world, with success rates based on skill level and resource rarity.

### Mining
**Skill:** Mining
**Tool:** Pickaxe, Shovel
**Resources:** Ore, Gems, Granite

#### Ore Types
| Ore | Skill Required | Color |
|-----|---------------|-------|
| Iron | 0 | Default |
| Dull Copper | 65 | Copper |
| Shadow Iron | 70 | Dark |
| Copper | 75 | Orange |
| Bronze | 80 | Brown |
| Gold | 85 | Gold |
| Agapite | 90 | Red |
| Verite | 95 | Green |
| Valorite | 99 | Blue |

### Lumberjacking
**Skill:** Lumberjacking
**Tool:** Hatchet, Axe
**Resources:** Wood, Special Wood

#### Wood Types
| Wood | Skill Required | Era |
|------|---------------|-----|
| Regular | 0 | All |
| Oak | 65 | ML+ |
| Ash | 80 | ML+ |
| Yew | 95 | ML+ |
| Heartwood | 100 | ML+ |
| Bloodwood | 100 | ML+ |
| Frostwood | 100 | ML+ |

### Fishing
**Skill:** Fishing
**Tool:** Fishing Pole
**Resources:** Fish, Special Items

#### Fishing Yields
| Item | Skill Required |
|------|---------------|
| Fish | 0+ |
| Boots/Shoes | 0+ (junk) |
| Message in Bottle | 75+ |
| Treasure Maps | 90+ |
| Sea Serpents | 80+ |
| Special Fish | Various |

## How it Works for Players

### Mining
1. Equip pickaxe or shovel
2. Target mountain/cave tile
3. Receive ore based on skill and vein
4. Smelt ore into ingots at forge

### Lumberjacking
1. Equip hatchet or axe
2. Target tree
3. Receive logs based on skill
4. Convert logs to boards at sawmill

### Fishing
1. Equip fishing pole
2. Target water tile
3. Wait for bite
4. Receive fish or special items

## Configuration
```csharp
// HarvestSystem.cs
public class HarvestDefinition
{
    public int BankWidth { get; set; }
    public int BankHeight { get; set; }
    public TimeSpan MinRespawn { get; set; }
    public TimeSpan MaxRespawn { get; set; }
}

// Mining.cs
public static HarvestDefinition OreAndStone = new HarvestDefinition
{
    BankWidth = 8,
    BankHeight = 8,
    MinRespawn = TimeSpan.FromMinutes(10),
    MaxRespawn = TimeSpan.FromMinutes(20)
};
```

## GM Commands
```
[SetSkill Mining 100
[SetSkill Lumberjacking 100
[SetSkill Fishing 100
[add Pickaxe
[add Hatchet
[add FishingPole
```

## Resource Banks
Resources spawn in "banks" - invisible areas that regenerate over time:
- Each location has a finite resource amount
- Resources respawn after depletion
- Bank size affects total resources available

## Skill Bonuses

### Mining Bonuses
- **Gargoyle Race:** +5 to mining
- **Prospector's Tool:** Shows ore types
- **Sturdy Pickaxe:** Extra uses

### Lumberjacking Bonuses
- **Elf Race:** +5 to lumberjacking
- **Sturdy Hatchet:** Extra uses

### Fishing Bonuses
- **Fishing Ship:** Access deep water
- **Fabled Fishing Net:** Special catches
- **Bait:** Increased success

## FAQ

**Q: Why am I only getting iron ore?**
A: Mining skill affects ore type. Need 65+ for colored ores.

**Q: How do I find special wood?**
A: Special wood spawns randomly in ML+ areas. Need 65+ lumberjacking.

**Q: Can I fish from a boat?**
A: Yes, boats allow access to deep water fishing.

**Q: How long until resources respawn?**
A: Typically 10-20 minutes, varies by resource type.

**Q: What's the best mining spot?**
A: Large mountains with continuous tiles provide more resources.

**Q: Can I gain skill while harvesting?**
A: Yes, harvesting provides skill gains up to your cap.

## Special Items from Fishing
| Item | Description |
|------|-------------|
| Message in Bottle | Contains SOS coordinates |
| Treasure Map | Leads to buried treasure |
| Big Fish | Trophy fish for display |
| Special Nets | Used for special fishing |

## Related Systems
- Craft (`../Craft/`) - Uses harvested resources
- Treasure Maps (`../TreasureMaps/`) - Found while fishing
- High Seas (`../Expansions/High Seas/`) - Ship fishing
