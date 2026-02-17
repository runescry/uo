# Plants Service

## Overview
The Plants system allows players to grow decorative and resource-producing plants in their homes, featuring cross-pollination, plant care, and seed collection.

## Era
- **Expansion:** Age of Shadows+ (`Core.AOS`)

## Files
| File | Description |
|------|-------------|
| `PlantSystem.cs` | Core plant growth logic |
| `PlantItem.cs` | Plant container item |
| `PlantType.cs` | Plant type definitions |
| `PlantHue.cs` | Plant color definitions |
| `PlantBowl.cs` | Growing container |
| `PlantResources.cs` | Harvestable resources |
| `MainPlantGump.cs` | Main plant care UI |
| `ReproductionGump.cs` | Seed/resource collection UI |
| `PollinateTarget.cs` | Cross-pollination targeting |
| `PlantPourTarget.cs` | Watering targeting |
| `DisplayHelpTopic.cs` | Help system |
| `EmptyTheBowlGump.cs` | Bowl cleanup UI |
| `FertileDirtGump.cs` | Dirt quality UI |
| `SetToDecorativeGump.cs` | Decoration mode UI |
| `PottedCoffeePlant.cs` | Special coffee plant |
| `Raised Garden Beds/` | Advanced garden system |

## Functionality
A comprehensive plant growing system with breeding, care, and harvesting.

### Plant Lifecycle
1. **Seed** - Plant a seed in dirt
2. **Seedling** - Early growth stage
3. **Sapling** - Mid growth stage
4. **Adult** - Full grown, can reproduce
5. **Decorative** - Optional final state

### Plant Care
| Need | Method | Effect |
|------|--------|--------|
| Water | Pour water | Growth progress |
| Nutrients | Add potions | Health/growth |
| Pest Control | Add potions | Prevent damage |

### Plant Types
- Decorative flowers
- Resource plants
- Rare/special plants
- Color variants

## How it Works for Players

### Getting Started
1. Obtain seeds (loot, purchase, trade)
2. Get a plant bowl and fertile dirt
3. Plant seed in bowl
4. Place in home

### Daily Care
1. Check plant status gump
2. Water if needed
3. Add nutrients if needed
4. Treat for pests/fungus
5. Wait for growth

### Pollination
1. Have two adult plants
2. Use pollinate action
3. Target one plant, then other
4. Hybrid seeds possible

### Harvesting
1. Adult plant produces resources
2. Use reproduction gump
3. Collect seeds or resources
4. Plant new seeds or sell

## Configuration
```csharp
// PlantSystem.cs
public class PlantSystem
{
    public TimeSpan GrowthCheck = TimeSpan.FromHours(23);
    public int MaxWater = 4;
    public int MaxPoison = 8;
}

// PlantType.cs
public enum PlantType
{
    CampionFlowers,
    Poppies,
    Snowdrops,
    Bulrushes,
    // ... many more
}
```

## GM Commands
```
[add PlantBowl
[add PlantItem [type]
[add FertileDirt
```

## Plant Types (Examples)
| Type | Color Options | Resources |
|------|---------------|-----------|
| Campion | Multiple | Seeds, petals |
| Poppies | Multiple | Seeds, resources |
| Fern | Green | Decorative |
| Tribarrel Cactus | Limited | Special resources |

## Plant Colors
Base colors can be bred for:
- Standard colors (red, blue, yellow)
- Rare colors (black, white, fire)
- Hybrid colors (through breeding)

## Plant Needs
| Status | Cure |
|--------|------|
| Needs Water | Pour water |
| Needs Nutrients | Greater Heal potion |
| Fungus | Greater Cure potion |
| Insects | Greater Poison potion |

## FAQ

**Q: Where do I get seeds?**
A: Monster loot, player vendors, breeding.

**Q: How often do I need to tend plants?**
A: Check daily for water/nutrients.

**Q: Can plants die?**
A: Yes, from neglect or severe problems.

**Q: How do I get rare colors?**
A: Cross-pollinate specific color combinations.

**Q: What are plants good for?**
A: Decoration, resources, basketweaving materials.

**Q: Can I move a plant?**
A: Yes, in its bowl/container.

## Color Breeding
| Parent 1 | Parent 2 | Offspring |
|----------|----------|-----------|
| Red | Blue | Purple |
| Blue | Yellow | Green |
| Red | Yellow | Orange |
| (Complex chart in game) |

## Raised Garden Beds
Advanced growing system:
- Multiple plants per bed
- Automated watering options
- Increased capacity
- House addon

## Related Systems
- Basket Weaving (`../BasketWeaving/`) - Uses plant resources
- Housing System - Plant placement
- Harvest (`../Harvest/`) - Related gathering
