# Points Systems Service

## Overview
The Points Systems service provides a unified framework for tracking various point-based currencies used across different game systems.

## Era
- **Expansion:** Various (system-dependent)

## Files
| File | Description |
|------|-------------|
| `PointsSystem.cs` | Core points framework |
| `PlayerMobileProps.cs` | Player point properties |
| `BlackthornData.cs` | Blackthorn dungeon points |
| `CasinoData.cs` | Casino gambling points |
| `CleanUpBritanniaData.cs` | CUB recycling points |
| `DespiseCrystals.cs` | Despise dungeon crystals |
| `FellowshipData.cs` | Fellowship event points |
| `GauntletPoints.cs` | Doom gauntlet points |
| `QueensLoyalty.cs` | Queen Zhah loyalty points |
| `ShameCrystals.cs` | Shame dungeon crystals |
| `VirtueArtifactsSystem.cs` | Virtue artifact points |
| `VoidPool.cs` | Void pool points |

## Functionality
Unified point tracking for multiple game systems, each with their own earning methods and rewards.

### Point Types
| System | Currency | Era |
|--------|----------|-----|
| Blackthorn | Agent Points | TOL |
| Casino | Casino Chips | ML |
| Clean Up Britannia | CUB Points | HS |
| Despise | Wisp Crystals | SA |
| Fellowship | Fellowship Points | EJ |
| Gauntlet | Gauntlet Points | AOS |
| Queens Loyalty | Loyalty Points | SA |
| Shame | Crystals | SA |
| Virtue Artifacts | Virtue Points | EJ |
| Void Pool | Void Points | SA |

## How it Works for Players

### Earning Points
Each system has unique earning methods:
- Combat activities
- Turn-in donations
- Quest completion
- Event participation

### Spending Points
1. Find appropriate vendor/NPC
2. View available rewards
3. Exchange points for items
4. Points deducted

### Checking Balance
- Character property sheet
- Point-specific NPCs
- System gumps

## Configuration
```csharp
// PointsSystem.cs
public abstract class PointsSystem
{
    public abstract PointsType Loyalty { get; }
    public abstract TextDefinition Name { get; }
    public abstract bool AutoAdd { get; }
    public abstract double MaxPoints { get; }
}

// Individual system implementation
public class CleanUpBritanniaData : PointsSystem
{
    public override PointsType Loyalty => PointsType.CleanUpBritannia;
    public override double MaxPoints => double.MaxValue;
}
```

## GM Commands
```
[SetPoints [system] [amount]
[GetPoints [player] [system]
```

## Point Systems Detail

### Blackthorn Points
- Earned: Blackthorn dungeon activities
- Spent: Special equipment, recipes

### Casino Points
- Earned: Gambling wins
- Spent: Casino rewards

### Clean Up Britannia
- Earned: Item turn-ins
- Spent: Exclusive decorations, items

### Despise Crystals
- Earned: Despise dungeon kills
- Spent: Wisp companion upgrades

### Gauntlet Points
- Earned: Doom gauntlet completion
- Spent: Doom rewards

### Queens Loyalty
- Earned: Serving Queen Zhah
- Spent: Gargoyle rewards

### Shame Crystals
- Earned: Shame dungeon activities
- Spent: Shame rewards

### Virtue Artifacts
- Earned: Virtue shrine activities
- Spent: Virtue artifact pieces

### Void Pool
- Earned: Void pool event
- Spent: Void pool rewards

## FAQ

**Q: Are points shared between characters?**
A: Usually per-character, check specific system.

**Q: Do points expire?**
A: Generally no, unless system-specific.

**Q: Can I transfer points?**
A: No, points are character/account bound.

**Q: How do I see all my points?**
A: Character status window or system NPCs.

**Q: What system has best rewards?**
A: Subjective - depends on your needs.

## Point Exchange NPCs
Each system has specific NPCs:
- CUB Turn-in boxes
- Casino dealers
- Blackthorn agents
- Dungeon-specific NPCs

## Related Systems
- Clean Up Britannia (`../CleanUpBritannia/`)
- Loyalty System (`../LoyaltySystem/`)
- Seasonal Events (`../Seasonal Events/`)
