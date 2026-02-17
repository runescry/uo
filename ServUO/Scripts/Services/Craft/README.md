# Craft Service

## Overview
The Craft system provides the framework for all player crafting activities, including Blacksmithing, Tailoring, Carpentry, and more. It handles recipes, resource consumption, success chances, and exceptional item creation.

## Era
- **Expansion:** All (with era-specific additions)
- **Skills:** 11 crafting disciplines

## Files
| File | Description |
|------|-------------|
| `DefAlchemy.cs` | Alchemy recipes and potions |
| `DefBlacksmithy.cs` | Metalwork - weapons and armor |
| `DefBowFletching.cs` | Bows and arrows |
| `DefCarpentry.cs` | Wooden items and furniture |
| `DefCartography.cs` | Maps and navigation |
| `DefCooking.cs` | Food preparation |
| `DefGlassblowing.cs` | Glass items |
| `DefInscription.cs` | Scrolls and spellbooks |
| `DefMasonry.cs` | Stone items |
| `DefTailoring.cs` | Cloth and leather items |
| `DefTinkering.cs` | Tools and mechanical items |
| `Core/` | Core crafting framework classes |

## Crafting Disciplines

### Alchemy (`DefAlchemy.cs`)
**Skill:** Alchemy
**Era Additions:**
- SE: Enhanced healing potions
- SA: New potion types
- TOL: Additional recipes

**Products:** Potions, poisons, enhancement items

### Blacksmithing (`DefBlacksmithy.cs`)
**Skill:** Blacksmithy
**Era Additions:**
- AOS: Runic tools, enhanced metals
- SE: Samurai weapons
- ML: Elven weapons
- SA: Gargoyle items, alterations

**Products:** Weapons, armor, shields

### Bowcrafting/Fletching (`DefBowFletching.cs`)
**Skill:** Fletching
**Era Additions:**
- ML: Elven bows
- SA: New bow types

**Products:** Bows, crossbows, arrows

### Carpentry (`DefCarpentry.cs`)
**Skill:** Carpentry
**Era Additions:**
- AOS: Furniture
- ML: Elven furniture
- SA: New items

**Products:** Furniture, wooden items, instruments

### Cartography (`DefCartography.cs`)
**Skill:** Cartography
**Era Additions:**
- HS: Sea charts

**Products:** Maps, treasure maps

### Cooking (`DefCooking.cs`)
**Skill:** Cooking
**Era Additions:** Each expansion adds recipes

**Products:** Food items, beverages

### Glassblowing (`DefGlassblowing.cs`)
**Skill:** Alchemy (secondary)
**Era:** SE+

**Products:** Glass items, empty bottles

### Inscription (`DefInscription.cs`)
**Skill:** Inscription
**Era Additions:**
- ML: New scrolls
- TOL: Additional items

**Products:** Scrolls, spellbooks, runebooks

### Masonry (`DefMasonry.cs`)
**Skill:** Mining (secondary)
**Era:** SA+

**Products:** Stone items, gargoyle equipment

### Tailoring (`DefTailoring.cs`)
**Skill:** Tailoring
**Era Additions:**
- SE: Ninja gear
- ML: Elven clothing
- SA: Gargoyle attire

**Products:** Clothing, leather armor, cloth items

### Tinkering (`DefTinkering.cs`)
**Skill:** Tinkering
**Era Additions:** Continuous through all eras

**Products:** Tools, traps, jewelry, mechanical items

## How it Works for Players

### Basic Crafting
1. Have required skill level
2. Have required resources in pack
3. Use the appropriate tool
4. Select item from craft menu
5. Success/failure determined by skill

### Success Factors
- **Skill Level** - Primary factor
- **Tool Quality** - Runic tools add bonuses
- **Resource Type** - Special materials affect results
- **Exceptional Chance** - Based on skill

### Material Types
| Material Category | Examples |
|-------------------|----------|
| Metals | Iron, Dull Copper, Shadow, Copper, Bronze, Gold, Agapite, Verite, Valorite |
| Leather | Regular, Spined, Horned, Barbed |
| Wood | Regular, Oak, Ash, Yew, Heartwood, Bloodwood, Frostwood |
| Cloth | Regular, Special cloth types |

### Runic Tools
Special tools that add random magical properties:
- **Dull Copper** - 1-2 properties
- **Shadow** - 2-3 properties
- **Copper** - 2-3 properties
- **Bronze** - 3-4 properties
- **Gold** - 3-4 properties
- **Agapite** - 4-5 properties
- **Verite** - 4-5 properties
- **Valorite** - 5-6 properties

## Configuration
```csharp
// In Core/ - CraftSystem.cs
public abstract class CraftSystem
{
    public abstract double DefaultChanceAtMin { get; }
    public abstract SkillName MainSkill { get; }
    public virtual int GumpTitleNumber { get; }
}

// Success calculation
public double GetSuccessChance(Mobile from, CraftItem item)
```

## GM Commands
```
[add RunicHammer
[add RunicSewingKit
[SetSkill [skill] [value]  - For testing
```

## Crafting Gump Features
- Category navigation
- Recipe browsing
- Resource requirements display
- Make/Make Last/Make Number options
- Repair option (Blacksmith/Tailor)
- Enhance option (SA+)
- Mark item option (Carpentry)

## FAQ

**Q: How do I get exceptional items?**
A: High skill level increases exceptional chance. Minimum skill varies by item.

**Q: What affects success chance?**
A: Skill level, item difficulty, and any exceptional attempt penalty.

**Q: How do I use runic tools?**
A: Craft items normally with the runic tool - properties added automatically.

**Q: Can I repair items?**
A: Blacksmiths and Tailors can repair their respective item types.

**Q: How do I learn recipes?**
A: Some are known automatically by skill; others require recipe scrolls.

**Q: What is "marking" for Carpentry?**
A: Marks the crafter's name on the item as the maker.

## Exceptional Items
Exceptional quality provides:
- +35% durability
- Better base stats
- Maker's mark option
- Higher resale value

## Bulk Order System
See `../BulkOrders/` for the Bulk Order Deed system that rewards crafters for completing order contracts.

## Related Systems
- Bulk Orders (`../BulkOrders/`)
- Loot Generation (`../LootGeneration/`)
- Harvest (`../Harvest/`) - Resource gathering
- Armor Refinement (`../Armor Refinement/`)
