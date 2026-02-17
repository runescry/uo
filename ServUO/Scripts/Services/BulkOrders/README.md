# Bulk Order Deeds (BOD) Service

## Overview
The Bulk Order Deed system allows players to complete crafting contracts for NPCs in exchange for special rewards. BODs are a major endgame crafting activity.

## Era
- **Expansion:** Age of Shadows+ (`Core.AOS`)
- **New System:** Time of Legends (`Core.TOL`) - Points banking
- **Enabled Check:** `BulkOrderSystem.NewSystemEnabled = Core.TOL`

## Files
| File | Description |
|------|-------------|
| `BulkOrderSystem.cs` | Core BOD logic and reward calculations |
| `BulkMaterialType.cs` | Material type definitions |
| `Books/` | BOD book containers |
| `Items/` | BOD-related items |
| `LargeBODs/` | Large bulk order definitions |
| `SmallBODs/` | Small bulk order definitions |
| `Rewards/` | Reward item definitions |

## Functionality
Players obtain bulk orders from crafting NPCs, create the requested items, and turn them in for rewards. The system supports multiple crafting disciplines.

### Supported Crafts
| Craft | NPC Type |
|-------|----------|
| Blacksmithing | Blacksmith |
| Tailoring | Tailor |
| Tinkering | Tinker |
| Carpentry | Carpenter |
| Fletching | Bowyer |
| Alchemy | Alchemist |
| Inscription | Scribe |
| Cooking | Cook |

### BOD Types
- **Small BOD** - Single item type request (e.g., 20 Iron Daggers)
- **Large BOD** - Collection of Small BODs (e.g., complete armor set)

### Quality Modifiers
- **Exceptional** - Items must be crafted exceptional quality
- **Material** - Specific material required (e.g., Valorite, Barbed)
- **Quantity** - Number of items needed (10, 15, 20)

## How it Works for Players

### Getting a BOD
1. Visit the appropriate crafting NPC
2. Say "bulk order" or use the context menu
3. Receive a BOD based on your skill level
4. Cooldown timer starts for next BOD request

### Completing Small BODs
1. Craft the required items matching specifications
2. Drop items onto the BOD deed
3. BOD shows completion progress
4. Turn in completed BOD to the NPC

### Completing Large BODs
1. Complete all required Small BODs
2. Combine Small BODs into the Large BOD
3. Turn in completed Large BOD for enhanced rewards

### Claiming Rewards
1. Turn in completed BOD to NPC
2. View reward menu (TOL+) or receive random reward (Pre-TOL)
3. Select desired reward or bank points

## Points System (TOL+)
```csharp
public enum PointsMode
{
    Enabled,    // Always bank points
    Disabled,   // Always take items
    Automatic   // Auto-select based on reward value
}
```

### Points Banking
- Points accumulate across turn-ins
- Spend points on any available reward
- Higher-tier rewards cost more points

## Configuration
```csharp
// BulkOrderSystem.cs
public static bool NewSystemEnabled = Core.TOL;

// Configurable values
public static bool CanGetBulkOrder(Mobile m, BODType type)
public static bool ComputeGold(Type type, int quantity, out int gold)
```

## GM Commands
```
[add SmallBOD
[add LargeBOD
[add BODBook
```

## Reward Tiers
Rewards scale based on:
- BOD difficulty (material + exceptional + quantity)
- Large vs Small BOD
- Era-specific rewards

### Example Rewards
| Tier | Blacksmith Rewards |
|------|-------------------|
| Low | Gloves of Mining, Prospector's Tool |
| Mid | Colored Anvil, Runic Hammer |
| High | Ancient Hammer, Power Scrolls |

## FAQ

**Q: How often can I get a BOD?**
A: Every 6 hours by default. Higher skill = potentially better BODs.

**Q: Can I buy/sell BODs?**
A: Yes, BODs are tradeable items.

**Q: What's the best way to complete BODs?**
A: Organize with BOD Books, focus on one material type at a time.

**Q: Do I need the exact item?**
A: Yes - material, quality, and quantity must match exactly.

**Q: Can I exchange unwanted BODs?**
A: Yes (TOL+), speak to the NPC about exchanging BODs for a small fee.

**Q: What affects BOD quality received?**
A: Your crafting skill level and random chance.

## Large BOD Combinations
Large BODs require specific Small BOD combinations:
- **Ringmail Set** - All ringmail armor pieces
- **Chainmail Set** - All chainmail armor pieces
- **Platemail Set** - All platemail armor pieces
- **Weapon Sets** - Grouped weapon types

## Related Systems
- Craft System (`../Craft/`)
- Points Systems (`../PointsSystems/`)
- Vendor System (for selling BODs)
