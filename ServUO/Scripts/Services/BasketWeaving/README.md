# Basket Weaving Service

## Overview
The Basket Weaving system allows players to craft decorative baskets and use natural plant-based dyes for coloring items.

## Era
- **Expansion:** Stygian Abyss+ (`Core.SA`)
- **Skill Required:** None (but crafting skill helps)

## Files
| File | Description |
|------|-------------|
| `BasketWeavingBook.cs` | Recipe book for basket patterns |
| `Clippers.cs` | Tool for harvesting plant materials |
| `ColorFixative.cs` | Fixes dye colors to items |
| `MiscSAResources.cs` | SA-specific crafting resources |
| `NaturalDye.cs` | Dye items made from plants |
| `PlantPigment.cs` | Raw pigment extracted from plants |
| `PlantPigmentHue.cs` | Color definitions and mixing logic |
| `SilverSapling.cs` | Special plant resource |
| `Baskets/` | Subfolder with basket item definitions |

## Functionality
Basket Weaving provides an alternative crafting avenue focused on decorative items and natural dyes from the plant system.

### Crafting Chain
1. **Harvest** - Use Clippers on plants to get materials
2. **Process** - Create Plant Pigments from harvested materials
3. **Mix** - Combine pigments to create Natural Dyes
4. **Apply** - Use dyes on compatible items

### Color System
The pigment system includes:
- **Primary Colors** - Basic pigment hues
- **Bright Colors** - Intensified variants
- **Mixed Colors** - Combined pigment results
- **Color Fixative** - Permanence for applied dyes

## How it Works for Players

### Getting Started
1. Obtain Clippers (crafted by Tinkers)
2. Grow plants using the Plant system
3. Harvest plant materials with Clippers
4. Learn recipes from Basket Weaving Book

### Creating Dyes
1. Use Clippers on mature plants
2. Combine harvested materials to make Plant Pigment
3. Process pigments into Natural Dyes
4. Apply Color Fixative if needed

### Crafting Baskets
1. Obtain basket weaving materials
2. Use the Basket Weaving Book for recipes
3. Craft decorative basket items
4. Dye baskets with natural dyes

## Configuration
```csharp
// PlantPigmentHue.cs - Color mixing
public static bool IsMixable(PlantPigmentHue hue)
public static bool IsBright(PlantPigmentHue hue)
public static bool IsPrimary(PlantPigmentHue hue)
```

## GM Commands
```
[add Clippers
[add BasketWeavingBook
[add PlantPigment
[add NaturalDye
[add ColorFixative
```

## FAQ

**Q: What skill affects basket weaving?**
A: Success is generally not skill-based, but some items may benefit from crafting skills.

**Q: Can I dye any item with natural dyes?**
A: Only compatible items - primarily cloth, leather, and basket items.

**Q: How do I get different colors?**
A: Mix different plant pigments together. Primary colors combine to create new hues.

**Q: Where do I get the materials?**
A: Grow plants using the Plant system and harvest with Clippers.

**Q: Are baskets functional or just decorative?**
A: Most baskets are decorative containers for housing.

## Color Mixing Reference
| Pigment 1 | Pigment 2 | Result |
|-----------|-----------|--------|
| Red | Blue | Purple |
| Red | Yellow | Orange |
| Blue | Yellow | Green |
| (See PlantPigmentHue.cs for full list) |

## Related Systems
- Plant System (`../Plants/`)
- Craft System (`../Craft/`)
- Housing Decoration
