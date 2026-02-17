# Armor Refinement Service

## Overview
The Armor Refinement system allows players to enhance armor pieces with special refinement components, adding powerful bonuses to equipment.

## Era
- **Expansion:** Stygian Abyss+ (`Core.SA`)
- **Requirement:** SA client or higher

## Files
| File | Description |
|------|-------------|
| `ArmorRefiner.cs` | Core refinement logic and NPC |
| `Gumps.cs` | UI for refinement process |
| `Items.cs` | Refinement component definitions |

## Functionality
Armor Refinement provides an alternative to imbuing for enhancing armor. Refinement components are found as loot and can be applied to armor to add specific bonuses.

### Refinement Types
Refinements typically modify armor in these categories:
- **Defense Chance Increase (DCI)**
- **Hit Chance Increase (HCI)**
- **Damage modifiers**
- **Resistances**

### Refinement Components
Components drop from:
- High-end monster loot
- Treasure chests
- Boss encounters
- Quest rewards

## How it Works for Players

### Obtaining Refinements
1. Kill monsters in SA+ content areas
2. Loot refinement components from corpses/chests
3. Components have varying quality levels

### Applying Refinements
1. Find an Armor Refiner NPC or use the refinement gump
2. Select the armor piece to refine
3. Select the refinement component to apply
4. Confirm the refinement

### Requirements
- Armor must be compatible with refinement type
- Some refinements require specific armor materials
- Cannot stack multiple refinements of same type

## Configuration
```csharp
// In Items.cs - Refinement drop chances
public static bool Roll(Container c, int rolls, double chance)

// Vendor validation
public static bool CheckForVendor(Mobile from, RefinementItem item)
```

## GM Commands
No specific GM commands. Use `[add` to spawn refinement items for testing:
```
[add RefinementItem
```

## FAQ

**Q: Can I remove a refinement from armor?**
A: Generally no - refinements are permanent once applied.

**Q: Do refinements work with imbued armor?**
A: Yes, refinements stack with imbued properties up to item caps.

**Q: Where do I find the Armor Refiner NPC?**
A: Typically in SA expansion areas like the Stygian Abyss.

**Q: Can refinements fail?**
A: The application process has success/failure based on skill and component quality.

**Q: What's the best armor for refinement?**
A: High-end crafted or artifact armor benefits most from refinement.

## Related Systems
- Loot Generation (`../LootGeneration/`)
- Craft System (`../Craft/`)
- Imbuing (`../LootGeneration/Imbuing/`)
