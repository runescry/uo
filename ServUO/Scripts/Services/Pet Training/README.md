# Pet Training Service

## Overview
The Pet Training system allows players to customize and enhance their tamed creatures with new abilities, stats, and special attacks.

## Era
- **Expansion:** Time of Legends+ (`Core.TOL`)

## Files
| File | Description |
|------|-------------|
| `PetTrainingHelper.cs` | Core training logic |
| `AbilityProfile.cs` | Pet ability management |
| `PlanningProfile.cs` | Training plan storage |
| `TrainingProfile.cs` | Active training state |
| `TrainingDefinition.cs` | Training rules and limits |
| `TrainingPoint.cs` | Training point management |
| `SpecialAbility.cs` | Special ability definitions |
| `AreaEffects.cs` | Area attack abilities |
| `EthologistTitleDeed.cs` | Title for pet trainers |
| `PetTrainingGate.cs` | Training area access |
| `Gumps.cs` | Training UI |

## Functionality
Allows deep customization of tamed pets through training points spent on stats, resistances, and abilities.

### Training Points
- Earned through pet use
- Spent on improvements
- Limited total points
- Unlock special abilities

### Trainable Aspects
| Aspect | Description |
|--------|-------------|
| Stats | Strength, Dexterity, Intelligence |
| Resistances | Fire, Cold, Poison, Energy, Physical |
| Skills | Combat and utility skills |
| Abilities | Special attacks and effects |

### Special Abilities
Powerful abilities unlocked through training:
- **Melee Abilities** - Enhanced attacks
- **Magic Abilities** - Spell-like effects
- **Area Effects** - AoE damage
- **Utility** - Support abilities

## How it Works for Players

### Starting Training
1. Tame a creature
2. Bond with the pet
3. Access training gump
4. View available upgrades

### Earning Training Points
1. Use pet in combat
2. Pet earns experience
3. Convert to training points
4. Points unlock options

### Spending Points
1. Open training gump
2. Select improvement category
3. Choose specific upgrade
4. Confirm point expenditure

### Planning Builds
1. Use planning profile
2. Map out desired build
3. Save plan for reference
4. Execute over time

## Configuration
```csharp
// PetTrainingHelper.cs
public static class PetTrainingHelper
{
    public static int GetMaxPoints(BaseCreature bc)
    public static bool CanTrain(BaseCreature bc, TrainingDefinition def)
}

// TrainingDefinition.cs
public class TrainingDefinition
{
    public int PointCost { get; set; }
    public int MaxLevel { get; set; }
}
```

## GM Commands
```
[PetTrain [pet]           - Force training gump
[SetPetPoints [amount]    - Set training points
```

## Special Abilities List
| Ability | Type | Effect |
|---------|------|--------|
| Raging Breath | Area | Fire damage AoE |
| Lightning Strike | Single | Lightning damage |
| Frenzied Whirlwind | Area | Physical AoE |
| Venomous Bite | Single | Poison application |
| (Many more) | Various | Various effects |

## Training Costs
| Upgrade Type | Point Cost |
|--------------|------------|
| Minor Stat | 50 |
| Major Stat | 100 |
| Resistance | 75 |
| Ability Tier 1 | 150 |
| Ability Tier 2 | 300 |
| Ability Tier 3 | 500 |

## FAQ

**Q: What pets can be trained?**
A: Most tamed and bonded creatures.

**Q: How do I earn training points?**
A: Use your pet in combat; it earns experience.

**Q: Can I reset training?**
A: Generally no, so plan carefully.

**Q: What's the maximum training points?**
A: Varies by creature type and stats.

**Q: Can I train any ability?**
A: Limited by creature type and available points.

**Q: Is there a training skill?**
A: Animal Taming and Lore affect training options.

## Pet Categories
| Category | Training Potential |
|----------|-------------------|
| Weak | Low points cap |
| Medium | Moderate cap |
| Strong | High cap |
| Greater | Very high cap |

## Ethologist Title
- Earned through extensive pet training
- Shows mastery of system
- Cosmetic achievement

## Related Systems
- Taming System
- Veterinary Skill
- Stable System
