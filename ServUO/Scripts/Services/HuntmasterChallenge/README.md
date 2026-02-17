# Huntmaster Challenge Service

## Overview
The Huntmaster Challenge is a competitive hunting system where players compete to kill the most impressive creatures and earn trophies, rankings, and rewards.

## Era
- **Expansion:** High Seas+ (`Core.HS`)

## Files
| File | Description |
|------|-------------|
| `HuntingSystem.cs` | Core hunting system logic |
| `HuntMaster.cs` | Huntmaster NPC |
| `HuntMasterRewardGump.cs` | Reward selection UI |
| `HuntingPermit.cs` | Required permit to participate |
| `HuntTrophy.cs` | Basic trophy item |
| `ComplexHuntTrophy.cs` | Advanced trophy display |
| `DisplayTrophy.cs` | Wall-mounted trophy display |
| `BestKillBoard.cs` | Leaderboard display |
| `KillEntry.cs` | Individual kill record |
| `HuntingTrophyInfo.cs` | Trophy information definitions |
| `ResourceSatchels.cs` | Hunting resource containers |
| `RewardItems.cs` | Reward item definitions |

## Functionality
Players participate in hunting challenges, recording their best kills and competing for top rankings on leaderboards.

### Hunting Permits
- Required to participate
- Purchase from Huntmaster NPC
- Time-limited validity
- Tracks kills while active

### Trophy System
When killing eligible creatures:
- Kill rating calculated
- Trophy created if impressive
- Trophies display on walls
- Best kills tracked on leaderboards

### Kill Rating Factors
- Creature type
- Creature stats (HP, damage dealt)
- Special attributes
- Random variation

## How it Works for Players

### Getting Started
1. Find a Huntmaster NPC
2. Purchase a Hunting Permit
3. Equip/activate permit
4. Hunt eligible creatures
5. Return trophies to Huntmaster

### Making Kills Count
1. Have active permit
2. Kill eligible creature
3. Receive trophy item
4. Trophy rates your kill
5. Submit for ranking

### Earning Rewards
1. Accumulate hunting points
2. Speak with Huntmaster
3. View reward menu
4. Exchange points for rewards

## Configuration
```csharp
// HuntingSystem.cs
public static class HuntingSystem
{
    public static TimeSpan PermitDuration = TimeSpan.FromDays(7);
    public static int PointsPerKill = 10;
}

// Eligible creatures list
public static Type[] HuntableCreatures = new Type[]
{
    typeof(GreatHart),
    typeof(Boar),
    typeof(BlackBear),
    // ... etc
};
```

## GM Commands
```
[add Huntmaster
[add HuntingPermit
[add BestKillBoard
[HuntingReset        - Reset hunting data
```

## Eligible Creatures
| Creature | Points | Trophy Type |
|----------|--------|-------------|
| Great Hart | Low | Antler mount |
| Black Bear | Medium | Bear head |
| Great Wyrm | High | Dragon head |
| (Various) | Variable | Type-specific |

## Trophy Types
| Trophy | Description |
|--------|-------------|
| Basic Trophy | Simple wall mount |
| Complex Trophy | Detailed display |
| Display Trophy | Premium presentation |

## Leaderboards
The BestKillBoard tracks:
- Player name
- Creature killed
- Kill rating
- Date of kill

### Ranking Categories
- Best overall kill
- Best per creature type
- Most kills total
- Weekly/monthly leaders

## FAQ

**Q: Where do I find the Huntmaster?**
A: Huntmasters are located in various towns and hunting lodges.

**Q: How long does a permit last?**
A: Default is 7 days from activation.

**Q: Do I need the permit equipped?**
A: The permit must be in your backpack while hunting.

**Q: What makes a good kill rating?**
A: Creature difficulty, stats, and random factors all contribute.

**Q: Can I display multiple trophies?**
A: Yes, trophies are furniture items for your house.

**Q: How do leaderboards reset?**
A: Depends on server configuration - may be weekly, monthly, or persistent.

## Rewards
| Reward | Point Cost |
|--------|------------|
| Hunting Cape | 1,000 |
| Huntmaster Title | 5,000 |
| Special Mount | 10,000 |
| Rare Trophy | 25,000 |

## Resource Satchels
Special containers that:
- Auto-collect hunting resources
- Organize trophy materials
- Increase inventory efficiency

## Related Systems
- Points Systems (`../PointsSystems/`)
- Clean Up Britannia (`../CleanUpBritannia/`) - Similar point system
- Community Collections (`../CommunityCollections/`)
