# Veteran Rewards Service

## Overview
The Veteran Rewards system grants special items and bonuses to players based on their account age. Long-time players receive access to exclusive rewards as a thank-you for their loyalty.

## Era
- **Expansion:** All (Enhanced in later expansions)
- **Enabled Check:** `Config.Get("VetRewards.Enabled", true)`

## Files
| File | Description |
|------|-------------|
| `RewardSystem.cs` | Core reward calculation and access |
| `RewardCategory.cs` | Reward categorization |
| `RewardEntry.cs` | Individual reward definitions |
| `RewardList.cs` | Reward tier lists |
| `RewardChoiceGump.cs` | Reward selection UI |
| `RewardConfirmGump.cs` | Purchase confirmation |
| `RewardNoticeGump.cs` | New reward notification |
| `RewardOptionGump.cs` | Reward options UI |
| `RewardDemolitionGump.cs` | House demolition rewards |
| `StatRewardGump.cs` | Stat bonus rewards |
| `DaviesLocker.cs` | Treasure map storage reward |
| `Character Statue Maker/` | Character statue creation |
| `CrystalPortals/` | Portal reward items |

## Functionality
Rewards are granted based on account age in 30-day intervals.

### Reward Tiers
| Year | Level | Cumulative Days |
|------|-------|-----------------|
| 1 | 1-12 | 30-360 |
| 2 | 13-24 | 390-720 |
| 3 | 25-36 | 750-1080 |
| 4 | 37-48 | 1110-1440 |
| 5+ | 49+ | 1470+ |

### Reward Types
- Ethereal mounts
- House decorations
- Character statues
- Crystal portals
- Special clothing
- Skill cap bonuses
- Storage containers
- Cosmetic items

## How it Works for Players

### Checking Rewards
1. Account age calculated automatically
2. New rewards unlock each month
3. Notification when rewards available
4. Choose from available options

### Claiming Rewards
1. Access reward interface
2. Browse available rewards by tier
3. Select desired reward
4. Confirm selection
5. Item delivered to backpack

### Skill Cap Bonus
1. Unlocks at certain tiers
2. Increases maximum skill cap
3. Applied automatically
4. Cumulative with each tier

## Configuration
```csharp
// RewardSystem.cs
public static bool Enabled = Config.Get("VetRewards.Enabled", true);
public static bool SkillCapRewards = Config.Get("VetRewards.SkillCapRewards", true);
public static int SkillCap = Config.Get("PlayerCaps.TotalSkillCap", 7000);
public static int SkillCapBonus = Config.Get("VetRewards.SkillCapBonus", 200);
public static int SkillCapBonusLevels = Config.Get("VetRewards.SkillCapBonusLevels", 4);
public static TimeSpan RewardInterval = Config.Get("VetRewards.RewardInterval", TimeSpan.FromDays(30.0d));
public static int StartingLevel = Config.Get("VetRewards.StartingLevel", 0);
```

## GM Commands
```
[VetReward            - Open reward menu
[SetRewardLevel [n]   - Set player reward level
[add [RewardItem]     - Add specific reward items
```

## Reward Categories
| Category | Examples |
|----------|----------|
| Mounts | Ethereal mounts (horse, llama, ostard) |
| Housing | Decorative items, banners |
| Statues | Character statue maker |
| Portals | Crystal portals for teleportation |
| Storage | Davies' Locker for treasure maps |
| Clothing | Robes, cloaks, special dyes |
| Tools | Special crafting tools |

## Character Statues
Special reward allowing players to create statues of their characters:
```csharp
// CharacterStatueMaker.cs
public class CharacterStatueMaker : Item
{
    // Creates statue based on character appearance
    // Placeable in houses
    // Customizable pose
}
```

## Crystal Portals
Teleportation reward items:
```csharp
// CrystalPortal.cs
public class CrystalPortal : Item
{
    // Links two locations
    // House placement only
    // Charges required
}

// CorruptedCrystalPortal.cs
public class CorruptedCrystalPortal : CrystalPortal
{
    // Enhanced version
    // Additional features
}
```

## Skill Cap System
```csharp
// Skill cap increases per tier
public static float SkillCapBonusIncrement = SkillCapBonus / SkillCapBonusLevels;
// Default: 200 / 4 = 50 points per tier level

// Total cap calculation
int bonusLevels = Math.Min(rewardLevel, SkillCapBonusLevels);
int totalCap = SkillCap + (int)(bonusLevels * SkillCapBonusIncrement);
```

## Access Checking
```csharp
// Check if player has access to reward
public static bool HasAccess(Mobile mob, RewardEntry entry)
{
    if (Core.Expansion < entry.RequiredExpansion)
        return false;

    TimeSpan ts;
    return HasAccess(mob, entry.List, out ts);
}

// Time-based access
TimeSpan totalTime = (DateTime.UtcNow - acct.Created) +
    TimeSpan.FromDays(RewardInterval.TotalDays * StartingLevel);
```

## FAQ

**Q: How do I check my reward level?**
A: Open the veteran rewards menu from your paperdoll.

**Q: Can I choose any reward?**
A: Only rewards at or below your current level.

**Q: Are rewards per-account or per-character?**
A: Account-wide unlocking, claimed per character.

**Q: What's the skill cap bonus?**
A: Up to 200 additional skill points at max tier.

**Q: Can I get past rewards?**
A: Yes, all unlocked tiers remain available.

**Q: Do rewards transfer between shards?**
A: Account age transfers, items do not.

**Q: What's the starting level configuration?**
A: Servers can grant starting levels to all accounts.

## Reward Interface
The `IRewardItem` interface marks veteran reward items:
```csharp
public interface IRewardItem
{
    bool IsRewardItem { get; set; }
}
```

## Related Systems
- Account System
- Housing System
- Ultima Store (`../UltimaStore/`)
- Points Systems (`../PointsSystems/`)
