# Clean Up Britannia Service

## Overview
Clean Up Britannia (CUB) is a points-based recycling system where players turn in unwanted items for points that can be exchanged for exclusive rewards.

## Era
- **Expansion:** High Seas+ (`Core.HS`)
- **Points System:** Uses PointsSystem framework

## Files
| File | Description |
|------|-------------|
| `CleanUpBritanniaRewards.cs` | Reward definitions and point values |
| `Gumps.cs` | UI for point exchange |
| `PointExchange.cs` | Turn-in and point calculation logic |
| `Creatures/` | CUB-related creature spawns |
| `Items/` | Reward items |

## Functionality
Players can dispose of nearly any item in the game for Clean Up Britannia points, then exchange those points for exclusive rewards.

### Point Values
Items have different point values based on:
- Item type
- Item quality
- Item properties
- Rarity

### Reward Categories
- Decorative items
- Functional tools
- Rare hue items
- Exclusive artifacts

## How it Works for Players

### Turning In Items
1. Find a Clean Up Britannia collection box
2. Drop unwanted items into the box
3. Receive points based on item value
4. Points accumulate on your account

### Checking Points
- Use the CUB gump to view current points
- Points shown in character property sheet
- Speak to CUB NPCs for balance

### Claiming Rewards
1. Open the CUB rewards gump
2. Browse available rewards
3. Select reward (must have enough points)
4. Receive reward item

## Point Values (Examples)
| Item Type | Points |
|-----------|--------|
| Basic weapon | 1-10 |
| Magic weapon | 10-100+ |
| Artifact | 100-1000+ |
| Rare item | Variable |

## Configuration
```csharp
// CleanUpBritanniaRewards.cs
public static int GetPoints(Item item)
{
    // Returns point value based on item properties
}

// PointExchange.cs
public static void Exchange(Mobile from, Item item)
{
    // Process item turn-in
}
```

## GM Commands
```
[CUBPoints [player] [amount]  - Set player points
[add CleanUpBritanniaBox      - Place collection box
```

## Rewards List (Examples)
| Reward | Point Cost | Description |
|--------|------------|-------------|
| Decorative item | 10,000 | Housing decoration |
| Tool upgrade | 25,000 | Enhanced crafting tool |
| Rare hue dye | 50,000 | Special color dye |
| Exclusive artifact | 100,000+ | Unique equipment |

## FAQ

**Q: What items give the most points?**
A: High-end magic items, artifacts, and rare items give the most points.

**Q: Can I turn in anything?**
A: Most items can be turned in, but some quest/special items may be excluded.

**Q: Where are collection boxes?**
A: Typically in major city banks and public buildings.

**Q: Do points expire?**
A: No, points remain on your account indefinitely.

**Q: Can I transfer points?**
A: No, points are character/account bound.

**Q: What's the best strategy?**
A: Save up points for high-value exclusive rewards rather than spending on low-tier items.

## Collection Box Locations
- Britain Bank
- Luna Bank
- Zento Bank
- Other major city banks

## Point Calculation
Points are calculated based on:
```csharp
int points = BaseValue(item);
points += PropertyBonus(item);
points += QualityBonus(item);
points += RarityBonus(item);
return points;
```

## Related Systems
- Points Systems (`../PointsSystems/`)
- Community Collections (`../CommunityCollections/`)
- Vendor System (for item value reference)
