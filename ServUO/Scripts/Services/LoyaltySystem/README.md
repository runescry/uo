# Loyalty System Service

## Overview
The Loyalty System tracks player loyalty ratings with various factions and entities, unlocking content and abilities at higher loyalty levels.

## Era
- **Expansion:** Stygian Abyss+ (`Core.SA`)

## Files
| File | Description |
|------|-------------|
| `LoyaltyRating.cs` | Loyalty level definitions and calculations |
| `LoyaltyRatingGump.cs` | Loyalty display UI |

## Functionality
Tracks and displays loyalty to various game factions, with higher loyalty granting access to special content.

### Loyalty Factions
Different entities track loyalty separately:
- Queen Zhah (Gargoyle Queen)
- Void Pool Entities
- Other expansion-specific factions

### Loyalty Levels
Loyalty increases through faction-related activities:
- Completing quests
- Killing faction enemies
- Donating resources
- Participating in events

## How it Works for Players

### Checking Loyalty
1. Open character status
2. View loyalty ratings section
3. Or speak with faction NPCs

### Increasing Loyalty
1. Perform faction-friendly actions
2. Complete faction quests
3. Kill faction enemies
4. Make donations

### Loyalty Benefits
Higher loyalty unlocks:
- Special NPCs/vendors
- Quest access
- Unique items
- Area access

## Configuration
```csharp
// LoyaltyRating.cs
public enum LoyaltyRating
{
    Disfavored = -2,
    Disliked = -1,
    Neutral = 0,
    Respected = 1,
    Admired = 2,
    Adored = 3,
    Revered = 4
}

public static int GetLoyaltyLevel(Mobile from)
{
    // Calculate based on actions
}
```

## GM Commands
```
[SetLoyalty [faction] [level]  - Set loyalty level
[CheckLoyalty [player]         - View loyalty
```

## Loyalty Actions
| Action | Loyalty Change |
|--------|----------------|
| Kill faction enemy | +1 to +5 |
| Complete quest | +5 to +20 |
| Donate resources | +1 to +10 |
| Hostile action | -10 to -50 |

## Loyalty Thresholds
| Level | Points Required |
|-------|----------------|
| Disfavored | -1000+ |
| Disliked | -500 to -999 |
| Neutral | -499 to 499 |
| Respected | 500 to 1999 |
| Admired | 2000 to 4999 |
| Adored | 5000 to 9999 |
| Revered | 10000+ |

## FAQ

**Q: How do I check my loyalty?**
A: Character status window or faction NPC dialog.

**Q: Can loyalty decrease?**
A: Yes, hostile actions reduce loyalty.

**Q: What does loyalty unlock?**
A: Varies by faction - special vendors, quests, items, areas.

**Q: Is loyalty account-wide?**
A: Usually per-character.

**Q: How fast does loyalty increase?**
A: Depends on activities performed.

**Q: What if I'm hostile to a faction?**
A: Some content may be locked until loyalty improves.

## Faction Examples

### Queen Zhah
- Location: Royal Palace, Ter Mur
- Activities: Void Pool, Abyss quests
- Rewards: Gargoyle-specific items

### Other Factions
Various expansion-specific loyalty tracking.

## Related Systems
- City Loyalty System (`../City Loyalty System/`)
- Points Systems (`../PointsSystems/`)
- Quests (`../Quests/`)
