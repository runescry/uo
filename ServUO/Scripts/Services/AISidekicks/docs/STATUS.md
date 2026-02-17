# AI Sidekicks - Current Status

**Last Updated:** December 2, 2025
**Version:** 2.4.0 (Leash & Chase System)

## Executive Summary

The AI Sidekicks system is **fully functional** and provides autonomous companion NPCs that can follow commands, engage in combat, cast spells, and adapt to different combat scenarios.

### Latest Updates (v2.4.0)
- **Leash System**: Mage stays within 40 tiles of combat start point
- **Circular Kiting**: At leash boundary, strafes perpendicular instead of retreating
- **Chase Mode**: Aggressive 3-5 tile spell release when enemy below 35% HP
- **Simulation-Optimized Parameters**: MIN_SAFE_RANGE=16, MAX_CAST_RANGE=24

### Previous Major Updates
- **Mage Combat**: Professional UO PvP tactics with spell combos and predictive kiting
- **Tamer Support**: Commands pets, heals with bandages and Greater Heal
- **Healer Support**: Pure support class with heal priority system
- **Necromancer**: Dark magic with necromancy spells

## ✅ Completed Features

### Core System
- **Base Architecture**: `BaseSidekick` inherits from `BaseCreature`, enabling full pet command compatibility
- **Custom AI**: `SidekickAI` extends `BaseAI` with specialized combat and movement logic
- **Speech Commands**: Full support for pet commands (follow, kill, stay, guard, etc.)
- **Archetype System**: 10 distinct sidekick types with unique stats, skills, and equipment
- **Persistence**: Sidekicks save/load correctly and maintain state across server restarts

### Combat System
- **Melee Combat**: Standard melee sidekicks engage in close combat with proper movement
- **NEW: Advanced Mage Combat** (`MageCombatAI`):
  - **Proper Spell Ranges**: 3-11 tiles (corrected from 20-25)
  - **Spell Combos**: Explosion→Energy Bolt burst damage, Energy Bolt spam finisher
  - **Priority System**: Critical→Defensive→Interrupt→Offensive→Positioning
  - **Mana Management**: Retreat→Meditate→Invisibility when low mana
  - **Bandage Healing**: Uses bandages first (mana-efficient), spells as backup
  - **Spell Interruption**: Magic Arrow spam to disrupt enemy casting
  - **Movement**: Running only (no teleport spam)
- **Tamer Support Class** (`TamerCombatAI`):
  - **Pet Commands**: Automatically commands bonded pets to "ALL KILL" enemies
  - **Veterinary Healing**: Bandages pets every 10 seconds during combat
  - **Greater Heal on Pets**: Casts Greater Heal on pets (not self) when pet < 50% health
  - **Support Class Behavior**: Does NOT engage in melee, stays near pet to heal
  - **Post-Combat Recovery**: Heals pets to full health after combat ends
  - **Smart Positioning**: Stays adjacent to pet, follows if pet moves away
- **NEW: Healer Support Class** (`HealerCombatAI`):
  - **Pure Support**: Does NOT engage in melee combat, no offensive spells
  - **Heal Priority**: Self (critical) → Owner → Other Sidekicks → Pets
  - **Bandage Loop**: Always active if below 100% health and not poisoned
  - **Heal Potion**: Uses Greater Heal Potion at 75% health (instant)
  - **Greater Heal**: Casts on self at 50% health
  - **Escape Logic**: At 25% health - Teleport (if possible) or Run + Greater Heal while escaping
  - **Cure Poison**: Uses cure potions and spells on self and allies
- **Healing**: Automatic healing when health drops below 60% (bandages + spells)
- **Meditation**: Mana regeneration while retreating or invisible

### Movement & Kiting
- **Running**: Sidekicks properly run when retreating or advancing
- **Wall Collision**: Handles obstacles by trying perpendicular directions and using `PathFollower`
- **Stuck Detection**: Detects when sidekick can't retreat further and adapts casting range
- **Adaptive Range**: Falls back to closer casting range (8-10 tiles) when stuck in tight spaces

### Equipment & Spawning
- **Starting Equipment**: Each archetype spawns with appropriate gear:
  - Full spellbooks (all spells unlocked)
  - 500 bandages
  - Bag with 250 of each reagent
  - Full suit of Horned runic leather armor
  - Vanquishing weapons for each class
  - 20 of each potion type (excluding agility, strength, refresh, poison, explosion, invisibility)
- **Horse Mount**: All sidekicks spawn with a horse and start mounted
- **Stats**: All sidekicks start with Str/Int/Dex = 100
- **Skills**: All archetype skills start at 125.0

### Pet Commands
- **Follow**: Sidekick follows owner at appropriate distance
- **Kill/Attack**: Targets and engages enemies
- **Stay**: Stops movement and holds position
- **Guard**: Defends owner and attacks aggressors
- **Release**: Releases sidekick from control
- **Transfer**: Transfers ownership to another player

## 🔧 Recent Fixes & Improvements

### MAJOR: Leash & Chase System (v2.4.0 - Latest)
- **Leash System**: Combat anchor set at combat start, mage stays within 40 tiles
- **Circular Kiting**: At leash boundary (35+ tiles), strafes perpendicular to enemy
- **Direction Switching**: If blocked, reverses circular direction (CW/CCW)
- **Chase Mode Constants**: `CHASE_SPELL_RELEASE_MIN=3`, `CHASE_SPELL_RELEASE_MAX=5`
- **Aggressive Finishing**: When enemy < 35% HP, uses point-blank spell release
- **No Retreat in Chase**: Moves closer instead of retreating during execute phase
- **Simulation Parameters**: Applied optimal values (MIN_SAFE_RANGE=16, MAX_CAST_RANGE=24)

### Necromancer Combat AI (v2.3.0)
- **NEW: NecromancerCombatAI Class**: Dedicated AI for necromancer archetype
- **Necromancy Spells**: Uses actual necromancy spells instead of standard magery
- **Spell Priority**: Evil Omen → Corpse Skin (debuffs) → Strangle (DoT) → Poison Strike (AoE) → Pain Spike (spam)
- **Wither AoE**: Casts Wither when multiple enemies nearby
- **Necro Reagents**: Backpack includes 50 each of Bat Wing, Grave Dust, Daemon Blood, Nox Crystal, Pig Iron
- **Necromancer Spellbook**: Full necromancer spellbook in addition to standard spellbook

### Healer Support Class (v2.2.0)
- **NEW: HealerCombatAI Class**: Complete support class AI for healers
- **Pure Support**: Does NOT engage in melee combat, no offensive spells
- **Bandage Loop**: Always active if below 100% health and not poisoned
- **Heal Potion at 75%**: Uses Greater Heal Potion (instant, no cast time)
- **Greater Heal at 50%**: Casts on self when below 50% health
- **Escape at 25%**: Teleport (if mana/skill available) or Run + Greater Heal while escaping
- **Heal Target Priority**: Self (critical) → Owner → Other Sidekicks → Pets

### Tamer Support Class (v2.1.0)
- **TamerCombatAI Class**: Complete support class AI for pet tamers
- **Support Class Combatant Handling**: Tamer/Healer sidekicks no longer have `Combatant` set (prevents base AI from moving toward enemy)
- **Pending Heal Target System**: Beneficial spells target pets instead of self
- **Post-Combat Recovery**: `DoOrderFollow()` triggers pet healing until full health
- **Smart Hysteresis**: Stays near pet, follows only when bandage is ready and pet moved away
- **Spell Target Processing**: Added to `DoOrderFollow()` for recovery healing spells

### MAJOR: Advanced Mage Combat System (v2.0.0)
- **NEW: MageCombatAI Class**: Complete rewrite of mage combat logic
- **Correct Spell Ranges**: Uses actual UO spell ranges (3-11 tiles) instead of 20-25
- **Spell Combo System**:
  - Explosion→Energy Bolt (simultaneous burst damage)
  - Energy Bolt→Energy Bolt (low health finisher)
  - Magic Arrow (interrupt enemy casting)
- **Priority-Based Decision Making**:
  1. CRITICAL: Very low mana/health → Go invisible or emergency heal
  2. DEFENSIVE: Low health/mana/poisoned → Bandage/heal/cure/retreat
  3. INTERRUPT: Enemy casting → Magic Arrow spam
  4. OFFENSIVE: Good position → Execute spell combos
  5. POSITIONING: Maintain 3-11 tile range
- **Intelligent Mana Management**:
  - Retreat when mana < 20
  - Meditate while retreating
  - Go invisible at safe distance (5+ tiles)
  - Meditate while invisible
- **Bandage Healing System**:
  - Uses bandages first (saves mana)
  - Falls back to Greater Heal if needed
  - 10-second bandage cooldown
- **No Teleport Spam**: Uses running for all movement
- **Invisibility Strategy**: Only at safe distance with low mana

### Movement Improvements
- **Running Mode**: Fixed `MoveTo()` and `DoMoveImpl()` to properly apply `Direction.Running` flag
- **Wall Collision**: Enhanced `RunFrom()` to try perpendicular directions and use `PathFollower` when blocked
- **Pathfinding**: Integrated `PathFollower` for complex obstacle navigation

### Combat Refinements
- **Spell Breaking Prevention**: Added checks to prevent movement during spell casting
- **Spell Target Processing**: Added `ProcessSpellTarget()` to correctly invoke spell effects
- **Mana Management**: Increased mage Int to 300 for larger mana pool
- **Healing Priority**: Healing spells take priority when health drops below 60%

### System Stability
- **Crash Protection**: Added try-catch blocks around spell processing and meditation
- **AI Timer**: Ensured AI timer is properly initialized and running
- **State Management**: Fixed `ForcedAI` caching to prevent AI state loss
- **Command Responsiveness**: Fixed issues where sidekicks wouldn't respond to commands after combat

## 📊 Technical Details

### Architecture
- **Inheritance**: `BaseSidekick` → `BaseCreature` → `Mobile`
- **AI System**: `SidekickAI` → `BaseAI`
- **Combat AI**: `PlayerLikeCombatAI` for special abilities
- **Mage AI**: `MageAI` integration for spell casting

### Key Properties
- `Controlled = true`: Sidekick is under player control
- `IsBonded = true`: Prevents deletion on death
- `FightMode.Closest`: Proactive defense
- `RangeFight`: Set based on combat style (1 for melee, 8 for archer, 10 for mage)
- `FreezeOnCast = true`: Mages don't move while casting

### Spell Casting Flow
1. **Cast Phase**: At 20-25 tiles, initiate spell cast
2. **Casting State**: Freeze in place while `IsCasting = true`
3. **Sequencing State**: Move to 8-10 tiles when `State = Sequencing`
4. **Release**: Spell releases at optimal range
5. **Retreat**: Move back to 20-25 tiles for next cast

### Stuck Detection Algorithm
1. Track consecutive retreat failures
2. Monitor distance changes after retreat attempts
3. If distance doesn't increase after 3 attempts → **Stuck Mode**
4. In Stuck Mode: Allow casting at current distance (minimum 8 tiles)
5. Reset counter when distance increases or spell is cast

## 🎮 In-Game Behavior

### Normal Combat (Open Space)
- Mage retreats to 20-25 tiles
- Casts spell
- Moves to 8-10 tiles to release
- Retreats back to 20-25 tiles
- Repeats cycle

### Tight Spaces (Tunnels, Dungeons)
- Attempts to retreat to 20-25 tiles
- Detects stuck state after 3 failed attempts
- Switches to "Stuck Mode"
- Casts at available distance (8-10 tiles minimum)
- Continues combat effectively despite space constraints

### Melee Combat
- Engages at melee range (1 tile)
- Follows target movement
- Uses appropriate weapons for archetype

## 📝 Known Limitations

1. **Limited Spell Combos**: Currently only 2 main combos (Explosion→EB, EB→EB)
2. **No Buff Management**: Doesn't cast Protection/Magic Reflection
3. **No Poison Combo**: Doesn't use Explosion→Poison→Energy Bolt combo yet
4. **Pathfinding**: `PathFollower` may fail in very complex mazes
5. **Single Target**: No multi-target spell tactics (Meteor Swarm/Chain Lightning)

## 🔮 Future Enhancements

### Planned Improvements
1. **Poison Combo**: Explosion→Poison→Energy Bolt for sustained pressure
2. **Buff Management**: Cast Protection to prevent fizzle from damage
3. **Magic Reflection**: Use against enemy mages
4. **Field Spells**: Paralyze Field / Energy Field for tactical control
5. **Group Combat**: Coordination when multiple sidekicks are present
6. **AoE Spells**: Meteor Swarm / Chain Lightning for multiple enemies

### Potential Features
- **Dispel**: Remove enemy buffs
- **Smart Spell Selection**: Choose spells based on enemy resistances
- **Debuff Combos**: Curse→Weaken→Explosion→Energy Bolt
- **Combat Commentary**: LLM-powered combat banter (future integration)
- **Adaptive Tactics**: Different strategies for different enemy types

## 🐛 Debugging & Diagnostics

### Logging
All key actions are logged to server console with color coding:
- **Green**: Successful actions (spell casting, movement)
- **Yellow**: Warnings (retreat attempts, stuck detection)
- **Cyan**: Information (distance checks, spell selection)
- **Red**: Errors (crashes, failures)
- **Magenta**: Combat state (mage combat entry)

### Key Log Messages
- `[SidekickAI.DoActionCombat]` - Combat decisions
- `[SidekickAI.RunFrom]` - Retreat attempts
- `[SidekickAI.DoOrderAttack]` - Attack command processing
- `[AutonomousSidekick.SetupOwnerRelationship]` - Initialization

## 📚 Related Documentation

- **`MAGE_COMBAT_SYSTEM.md`**: **NEW** - Detailed mage combat AI documentation
- **`SIDEKICK_ARCHETYPES.md`**: Complete archetype specifications
- **`ARCHITECTURE_ANALYSIS.md`**: Technical architecture details
- **`TESTING_GUIDE.md`**: Testing procedures and scenarios
- **`README.md`**: General overview and documentation index

## ✅ Testing Status

### Tested Scenarios
- ✅ Spawning with correct equipment
- ✅ Following owner
- ✅ Attacking targets
- ✅ Mage spell casting in open space
- ✅ Mage spell casting in tight spaces
- ✅ Wall collision handling
- ✅ Running mode
- ✅ Healing when low on health
- ✅ Meditation when low on mana
- ✅ Command responsiveness after combat
- ✅ Death and resurrection (via `IsBonded`)

### Pending Tests
- ⏳ Multiple sidekicks in combat
- ⏳ Very long combat sessions
- ⏳ Complex dungeon navigation
- ⏳ Teleport/Recall usage when stuck

## 🎯 Current Priority

**Status**: Version 2.0.0 - Advanced Mage Combat System Deployed. Focus is on:
1. **Testing**: In-game testing of new mage combat system
2. **Validation**: Verify spell combos work correctly (Explosion→Energy Bolt timing)
3. **Tuning**: Adjust mana thresholds and bandage vs spell heal balance
4. **Observation**: Monitor invisibility + meditation behavior
5. **Iteration**: Add poison combo and buff management based on test results

## 📞 Support

For issues or questions:
1. Check server logs for diagnostic messages
2. Review this status document
3. Check related documentation files
4. Review code comments in `SidekickAI.cs`

---

**System Status**: ✅ **OPERATIONAL** (v2.3.0)
**Last Major Update**: Necromancer Combat AI - Dedicated AI with necromancy spells and reagents (December 1, 2025)

## 🚀 Upgrade Notes (v1.0.0 → v2.0.0)

### What Changed
- **OLD**: Mages used simple kiting (retreat, cast, retreat) at 20-25 tiles
- **NEW**: Mages use UO PvP tactics with spell combos at 3-11 tiles

### Migration
- Existing sidekicks will automatically use new combat system on next load
- No data migration required
- Old `MageAI` code remains as fallback (legacy)

### Testing Checklist
- [ ] Spawn mage sidekick
- [ ] Verify spell combo (Explosion→Energy Bolt)
- [ ] Verify bandage healing when health < 60%
- [ ] Verify retreat→meditate→invisibility when mana < 20
- [ ] Verify Magic Arrow interrupt when enemy casts
- [ ] Verify proper range maintenance (3-11 tiles)
- [ ] Verify no teleport spam (running only)

