# Advanced Mage Combat AI System

**Created:** November 24, 2025
**Version:** 2.0.0

## Overview

The new `MageCombatAI` system implements professional UO PvP mage tactics for sidekick mages. This system replaces the old kiting-based approach with a strategic, priority-based combat AI that uses proper spell ranges, combos, and defensive tactics.

## Key Improvements

### 1. Correct Spell Ranges
The old system used 20-25 tile ranges, which exceeded actual spell ranges (10-12 tiles). The new system uses:
- **MIN_SAFE_RANGE**: 3 tiles (out of melee)
- **OPTIMAL_CAST_RANGE**: 8 tiles (mid-range for all spells)
- **MAX_CAST_RANGE**: 11 tiles (maximum spell range)

### 2. Spell Combo System
Implements burst damage combos to prevent enemy healing:

#### Standard Burst Combo (Explosion → Energy Bolt)
- Pre-cast Explosion (3-second delay)
- Follow immediately with Energy Bolt
- Both spells hit simultaneously for maximum burst damage
- **Mana Cost**: 40 (20 + 20)

#### Finisher Combo (Energy Bolt → Energy Bolt)
- Used when enemy health is low (<30%)
- Quick, repeatable damage
- **Mana Cost**: 40 (20 + 20)

#### Interrupt Combo (Magic Arrow spam)
- Fast-cast Magic Arrow to interrupt enemy spell casting
- Minimal cooldown (0.75 seconds)
- **Mana Cost**: 4 per cast

### 3. Priority-Based Decision Making

The AI evaluates combat priority in this order:

1. **CRITICAL**: Very low mana or critical health
   - Go invisible (at safe distance) to meditate
   - Emergency heal if critical health
   - Retreat from melee range

2. **DEFENSIVE**: Poisoned, low health, or low mana
   - Cure poison immediately
   - Cast Greater Heal when health < 60%
   - Retreat to safe distance and meditate if low mana
   - Only go invisible when at safe distance (5+ tiles)

3. **INTERRUPT**: Enemy is casting
   - Cast Magic Arrow to disrupt enemy spells
   - Prioritize interrupting heals

4. **OFFENSIVE**: Good position with resources
   - Execute spell combos
   - Maintain optimal range (3-11 tiles)

5. **POSITIONING**: Adjust range
   - Retreat if too close (<3 tiles)
   - Advance if too far (>11 tiles)
   - Maintain optimal range (8 tiles)

## Combat Flow

### Health & Mana Thresholds
- **CRITICAL_HEALTH**: 40% - Emergency actions
- **LOW_HEALTH**: 60% - Start healing
- **LOW_MANA**: 20 - Retreat and meditate
- **CRITICAL_MANA**: 10 - Go invisible to meditate

### Defensive Strategy

#### Low Mana Management
1. If mana < 20 and distance >= 5 tiles:
   - Cast Invisibility
   - Meditate while hidden

2. If mana < 20 but NOT at safe distance:
   - Retreat while running
   - Meditate while retreating
   - Once at safe distance (5+ tiles), go invisible

#### Poison Handling
- Immediately cast Cure when poisoned (if mana >= 6)
- Prevents bandage healing disruption

#### Health Management
- Cast Greater Heal when health < 60%
- Emergency heal when health < 40%
- Maintain safe distance while healing

### Offensive Strategy

#### Positioning
- Maintain 3-11 tile range at all times
- Optimal range: 8 tiles (middle ground)
- Run from enemy if too close (<3 tiles)
- Move toward enemy if too far (>11 tiles)

#### Spell Combos
1. **High Health Target**: Explosion → Energy Bolt
2. **Low Health Target** (<30%): Energy Bolt → Energy Bolt
3. **Enemy Casting**: Magic Arrow (interrupt)

### Movement
- **No Teleport**: Uses running only (Direction.Running flag)
- **Invisibility**: Only at safe distance with low mana
- **Retreat**: Run away from enemy when too close
- **Advance**: Move toward enemy when too far

## Technical Details

### Spell Timing
- **Explosion**: 3-second delay before damage, cast first
- **Energy Bolt**: Instant damage, cast immediately after Explosion
- **Magic Arrow**: Fast cast, 0.75s cooldown
- **Greater Heal**: 2.5s cast time
- **Cure**: 2.0s cast time
- **Invisibility**: Breaks on movement/action, used for meditation only

### Combo Tracking
The AI tracks:
- Current combo state (None, ExplosionEnergyBolt, FinisherCombo, Interrupt)
- Last spell cast times
- Next allowed cast time (global cooldown)
- Combo target (ensures combo completes on same target)

### State Management
```csharp
private CombatCombo m_CurrentCombo = CombatCombo.None;
private IDamageable m_ComboTarget = null;
private bool m_ExplosionActive = false;
private bool m_WaitingForEnergyBolt = false;
```

## Integration with SidekickAI

The `MageCombatAI` is initialized in `SidekickAI.InitializeMageAI()` and invoked from `SidekickAI.DoActionCombat()`:

```csharp
if (m_MageCombatAI != null && m_Sidekick.CombatStyle == CombatStyle.Mage)
{
    bool combatHandled = m_MageCombatAI.DoCombat(mobile);

    // Always face target
    if (!DirectionLocked)
        m_Sidekick.Direction = m_Sidekick.GetDirectionTo(mobile);

    return combatHandled;
}
```

## Spell Requirements

### Mana Costs
- **Explosion**: 20 mana
- **Energy Bolt**: 20 mana
- **Magic Arrow**: 4 mana
- **Greater Heal**: 11 mana
- **Cure**: 6 mana
- **Invisibility**: 9 mana

### Required Skills
- **Magery**: 125.0 (set by archetype)
- **Evaluating Intelligence**: 125.0 (spell damage)
- **Meditation**: 125.0 (mana regeneration)
- **Resisting Spells**: 125.0 (resist enemy spells)

## Advantages Over Old System

### Old System (SidekickAI Mage Combat)
- Used incorrect ranges (20-25 tiles, spells max at 12)
- No spell combos (random spell selection)
- Simple kiting (retreat → cast → retreat)
- Stuck detection for tight spaces
- No mana management strategy

### New System (MageCombatAI)
- ✅ Correct spell ranges (3-11 tiles)
- ✅ Strategic spell combos (burst damage)
- ✅ Priority-based decision making
- ✅ Proper mana management (invisibility + meditation)
- ✅ Spell interruption mechanics
- ✅ Defensive priority system
- ✅ No teleport spam (uses running)
- ✅ Intelligent positioning

## Testing Recommendations

### Test Scenarios
1. **Burst Combo**: Verify Explosion → Energy Bolt timing
2. **Low Mana**: Verify retreat → meditate → invisibility sequence
3. **Poison**: Verify immediate cure response
4. **Low Health**: Verify Greater Heal at 60% health
5. **Melee Range**: Verify retreat behavior
6. **Interrupt**: Verify Magic Arrow on enemy casting
7. **Finisher**: Verify Energy Bolt spam on low health targets

### Expected Behavior
- Mage maintains 3-11 tile range
- Uses spell combos for burst damage
- Retreats when mana is low
- Goes invisible only at safe distance
- Meditates to regenerate mana
- Interrupts enemy spell casting
- Heals at 60% health
- Cures poison immediately

## Future Enhancements

### Potential Additions
1. **Poison Combo**: Explosion → Poison → Energy Bolt
2. **Protection Buff**: Cast Protection to prevent fizzle
3. **Field Spells**: Use Paralyze Field / Energy Field tactically
4. **Dispel**: Remove enemy buffs
5. **Area Spells**: Meteor Swarm / Chain Lightning for multiple enemies
6. **Smart Spell Selection**: Choose spells based on enemy resistances

## Known Limitations

1. **No Teleport**: Uses running only (by design)
2. **Basic Combo Set**: Limited to 2 main combos
3. **No Buff Management**: Doesn't cast Protection/Magic Reflection
4. **No Debuff Removal**: Doesn't counter Curse/Weaken
5. **Single Target**: No multi-target tactics

## Conclusion

The new `MageCombatAI` system provides a professional, UO PvP-style mage combat experience for sidekicks. It uses correct spell ranges, strategic combos, priority-based decision making, and intelligent mana management. This system should provide a challenging and realistic mage opponent that players will recognize from PvP combat.

---

**For issues or questions, see:**
- `SidekickAI.cs` (integration point)
- `MageCombatAI.cs` (combat system)
- `CURRENT_STATUS.md` (overall system status)
