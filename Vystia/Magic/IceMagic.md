# Ice Magic - Frosthold School

**Class:** Ice Mage
**Region:** Frosthold
**Theme:** Cold damage, slowing effects, defensive ice barriers, area denial
**Spellbook:** Ice Mage Spellbook (IceMageSpellbook.cs)
**Primary Stat:** Intelligence
**Hue:** 0x481 (Ice Blue)

---

## Overview

Ice Magic harnesses the frozen powers of Frosthold, focusing on cold damage, mobility control, and defensive ice constructs. Ice Mages excel at kiting enemies with slowing effects, creating frozen barriers, and devastating groups with AoE cold damage.

**Strengths:**
- Strong AoE damage
- Excellent crowd control (slows, freezes, roots)
- Defensive ice barriers and armor
- High cold resistance
- Area denial with ice terrain

**Weaknesses:**
- Weak against fire damage
- Limited direct healing
- Many spells require positioning
- Lower single-target burst than other mages

---

## Spellbook Design

### IceMageSpellbook
- **Item ID:** 0xEFA (Spellbook graphic)
- **Hue:** 0x481 (Ice Blue)
- **Name:** "Tome of Frozen Arts"
- **Capacity:** 32 spells
- **Weight:** 3.0 stones
- **Special:** Grants +5% cold damage when equipped

---

## Spell List (32 Spells)

### Circle 1 (4 mana) - Basic Ice
*Mana: 4 | Cast Time: 0.5-1.0s*

#### 1. Frost Touch
- **Mana:** 4
- **Reagents:** Ginseng, Nightshade
- **Cast Time:** 0.5s
- **Range:** Melee (1 tile)
- **Effect:** Deals 4-8 cold damage on touch, 15% chance to slow (-10 DEX for 3s)
- **Visual:** Blue frost particles on hands (0x373A)
- **Sound:** 0x1FB
- **Purpose:** Basic melee defense spell

#### 2. Ice Shard
- **Mana:** 4
- **Reagents:** MandrakeRoot
- **Cast Time:** 0.75s
- **Range:** 10 tiles
- **Effect:** Deals 5-10 cold damage
- **Visual:** Small ice projectile (0x36E4)
- **Sound:** 0x64F
- **Purpose:** Basic ranged damage

#### 3. Frost Ward
- **Mana:** 4
- **Reagents:** Garlic, SpidersSilk
- **Cast Time:** 1.0s
- **Duration:** 30 seconds
- **Effect:** +5 Cold Resistance
- **Visual:** Faint blue shimmer (0x375A)
- **Sound:** 0x1E9
- **Purpose:** Basic cold protection

#### 4. Chill Aura
- **Mana:** 4
- **Reagents:** Nightshade
- **Cast Time:** 0.75s
- **Duration:** 20 seconds
- **Area:** 2 tile radius around caster
- **Effect:** Enemies within 2 tiles have -5% attack speed
- **Visual:** Cold mist at feet (0x3709)
- **Sound:** 0x1F1
- **Purpose:** Melee deterrent

---

### Circle 2 (6 mana) - Freezing Arts
*Mana: 6 | Cast Time: 1.0-1.5s*

#### 5. Freezing Grasp
- **Mana:** 6
- **Reagents:** Ginseng, SulfurousAsh
- **Cast Time:** 1.0s
- **Range:** 8 tiles
- **Effect:** Deals 8-14 cold damage, 30% chance to slow (-15 DEX for 4s)
- **Visual:** Blue hand reaching out (0x36BD)
- **Sound:** 0x1FB
- **Purpose:** Improved slow effect

#### 6. Ice Shield
- **Mana:** 6
- **Reagents:** Garlic, MandrakeRoot
- **Cast Time:** 1.25s
- **Duration:** 60 seconds
- **Effect:** +10 Physical Resistance, +15 Cold Resistance, reflects 5 damage on melee hits
- **Visual:** Orbiting ice shards (0x3779)
- **Sound:** 0x64F
- **Purpose:** Defensive buff

#### 7. Frost Slick
- **Mana:** 6
- **Reagents:** Bloodmoss, Nightshade
- **Cast Time:** 1.5s
- **Range:** 12 tiles (ground target)
- **Area:** 3x3 tiles
- **Duration:** 15 seconds
- **Effect:** Enemies walking through slip and are slowed (-20 DEX for 5s)
- **Visual:** Icy ground texture (0x122A)
- **Sound:** 0x028
- **Purpose:** Area denial and positioning

#### 8. Glacial Mend
- **Mana:** 6
- **Reagents:** Ginseng, Garlic
- **Cast Time:** 1.5s
- **Range:** 8 tiles
- **Effect:** Heals 15-25 HP, grants +5 Cold Resistance for 30s
- **Visual:** Blue healing sparkles (0x376A)
- **Sound:** 0x1F2
- **Purpose:** Minor healing with cold theme

---

### Circle 3 (9 mana) - Offensive Ice
*Mana: 9 | Cast Time: 1.5-2.0s*

#### 9. Ice Bolt ✓ (IMPLEMENTED)
- **Mana:** 9
- **Reagents:** MandrakeRoot, SulfurousAsh
- **Cast Time:** 1.5s
- **Range:** 12 tiles
- **Effect:** Deals 19-34 cold damage (scales with Magery/EvalInt), 25% chance to slow (-15 DEX for 5s)
- **Visual:** Ice projectile (0x36E4, hue 0x481)
- **Sound:** 0x1FB
- **Purpose:** Core damage spell
- **File:** IceBoltSpell.cs

#### 10. Frozen Ground
- **Mana:** 9
- **Reagents:** Bloodmoss, SulfurousAsh, Nightshade
- **Cast Time:** 2.0s
- **Range:** 12 tiles (ground target)
- **Area:** 4 tile radius
- **Duration:** 20 seconds
- **Effect:** Enemies on frozen ground take 3-6 damage/second and are slowed (-20 DEX)
- **Visual:** Ice covering ground (0x122A + particles)
- **Sound:** 0x64F
- **Purpose:** Area damage over time

#### 11. Ice Spear
- **Mana:** 9
- **Reagents:** MandrakeRoot, SulfurousAsh, BlackPearl
- **Cast Time:** 1.75s
- **Range:** 14 tiles
- **Effect:** Deals 25-40 cold damage, pierces targets (hits up to 3 enemies in line)
- **Visual:** Large ice spear projectile (0x36E4 scaled)
- **Sound:** 0x1FB
- **Purpose:** Multi-target damage

#### 12. Frostbite
- **Mana:** 9
- **Reagents:** Nightshade, Bloodmoss
- **Cast Time:** 1.5s
- **Range:** 10 tiles
- **Duration:** 12 seconds
- **Effect:** DoT dealing 4-7 damage/tick (6 ticks), reduces healing received by 25%
- **Visual:** Blue damage numbers, frost on target (0x374A)
- **Sound:** 0x1ED
- **Purpose:** Anti-healing debuff

---

### Circle 4 (11 mana) - Ice Barriers
*Mana: 11 | Cast Time: 2.0-2.5s*

#### 13. Frost Armor ✓ (IMPLEMENTED)
- **Mana:** 11
- **Reagents:** Garlic, Ginseng, MandrakeRoot
- **Cast Time:** 2.0s
- **Range:** 12 tiles
- **Duration:** 120-240 seconds (scales with Magery)
- **Effect:** +10 Physical Resistance, +20 Cold Resistance
- **Visual:** Frost aura (0x375A)
- **Sound:** 0x1E9
- **Purpose:** Defensive buff
- **File:** FrostArmorSpell.cs

#### 14. Ice Wall
- **Mana:** 11
- **Reagents:** Bloodmoss, MandrakeRoot, SpidersSilk
- **Cast Time:** 2.5s
- **Range:** 12 tiles
- **Effect:** Creates a 5-tile long wall of ice (blocks movement, 100 HP, lasts 30s)
- **Visual:** Transparent ice wall (0x80 + ice graphic)
- **Sound:** 0x1F8
- **Purpose:** Terrain blocking

#### 15. Icicle Barrage
- **Mana:** 11
- **Reagents:** MandrakeRoot, SulfurousAsh, BlackPearl
- **Cast Time:** 2.0s
- **Range:** 12 tiles
- **Area:** 3 tile radius around target
- **Effect:** Deals 15-25 cold damage to all enemies in area
- **Visual:** Multiple icicles rain down (0x36E4 x5)
- **Sound:** 0x64F
- **Purpose:** AoE burst damage

#### 16. Permafrost
- **Mana:** 11
- **Reagents:** Garlic, Ginseng, MandrakeRoot, Nightshade
- **Cast Time:** 2.25s
- **Range:** 10 tiles
- **Duration:** 8 seconds
- **Effect:** Target is rooted (cannot move), takes 5-10 damage/second
- **Visual:** Ice encasing feet (0x376A)
- **Sound:** 0x1F8
- **Purpose:** Hard CC (root)

---

### Circle 5 (14 mana) - Greater Ice
*Mana: 14 | Cast Time: 2.5-3.0s*

#### 17. Glacial Strike
- **Mana:** 14
- **Reagents:** MandrakeRoot, SulfurousAsh, BlackPearl, Bloodmoss
- **Cast Time:** 2.5s
- **Range:** 14 tiles
- **Effect:** Deals 35-55 cold damage, 50% chance to freeze (stun for 2s)
- **Visual:** Massive ice bolt (0x36E4 large)
- **Sound:** 0x1FB
- **Purpose:** High damage with stun

#### 18. Frozen Tomb
- **Mana:** 14
- **Reagents:** Garlic, Ginseng, MandrakeRoot, SpidersSilk
- **Cast Time:** 2.75s
- **Range:** 10 tiles
- **Duration:** 6 seconds
- **Effect:** Encases target in ice (immune to damage, cannot move/act). Breaks on damage.
- **Visual:** Ice sphere around target (0x3779)
- **Sound:** 0x1F8
- **Purpose:** Defensive disable (save ally or freeze enemy)

#### 19. Shatter
- **Mana:** 14
- **Reagents:** MandrakeRoot, SulfurousAsh, BlackPearl
- **Cast Time:** 2.0s
- **Range:** 12 tiles
- **Effect:** Deals 20-35 damage, BONUS: +50% damage if target is slowed/frozen
- **Visual:** Shattering ice explosion (0x36BD)
- **Sound:** 0x1FB
- **Purpose:** Combo finisher

#### 20. Hypothermia
- **Mana:** 14
- **Reagents:** Nightshade, Bloodmoss, MandrakeRoot
- **Cast Time:** 2.5s
- **Range:** 10 tiles
- **Duration:** 20 seconds
- **Effect:** Target loses 10% max HP, -25% movement speed, -15% attack speed
- **Visual:** Blue debuff aura (0x374A)
- **Sound:** 0x1ED
- **Purpose:** Severe debuff

---

### Circle 6 (20 mana) - Master Ice
*Mana: 20 | Cast Time: 3.0-3.5s*

#### 21. Blizzard ✓ (IMPLEMENTED)
- **Mana:** 20
- **Reagents:** MandrakeRoot, SulfurousAsh, Bloodmoss, SpidersSilk
- **Cast Time:** 3.0s
- **Range:** 12 tiles (ground target)
- **Area:** 5 tile radius
- **Duration:** 10 seconds (1 tick/second)
- **Effect:** Deals 3-8 damage/tick (scales with Magery), slows enemies
- **Visual:** Swirling ice storm (0x3709)
- **Sound:** 0x64F
- **Purpose:** Area DoT damage
- **File:** BlizzardSpell.cs

#### 22. Glacial Fortress
- **Mana:** 20
- **Reagents:** Garlic, Ginseng, MandrakeRoot, SpidersSilk, Bloodmoss
- **Cast Time:** 3.5s
- **Duration:** 60 seconds
- **Effect:** Creates ice fortress around caster (5 tile radius): +25 All Resistances, reflects 15 damage on melee hits, blocks projectiles
- **Visual:** Ice dome (0x3779 scaled)
- **Sound:** 0x1F8
- **Purpose:** Ultimate defense

#### 23. Avalanche
- **Mana:** 20
- **Reagents:** MandrakeRoot, SulfurousAsh, BlackPearl, Bloodmoss
- **Cast Time:** 3.0s
- **Range:** 14 tiles (direction)
- **Area:** 8 tile cone
- **Effect:** Deals 30-50 cold damage to all enemies in cone, knocks back 3 tiles, slows
- **Visual:** Ice and snow cascade (0x36E4 + 0x3709)
- **Sound:** 0x664
- **Purpose:** Massive AoE knockback

#### 24. Deep Freeze
- **Mana:** 20
- **Reagents:** MandrakeRoot, Nightshade, Bloodmoss, BlackPearl
- **Cast Time:** 3.25s
- **Range:** 12 tiles
- **Duration:** 10 seconds
- **Effect:** Target is frozen solid (stun), takes double damage from all sources
- **Visual:** Complete ice encasement (0x376A)
- **Sound:** 0x1F8
- **Purpose:** Hard CC with vulnerability

---

### Circle 7 (40 mana) - Ancient Ice
*Mana: 40 | Cast Time: 3.5-4.0s*

#### 25. Absolute Zero
- **Mana:** 40
- **Reagents:** MandrakeRoot, SulfurousAsh, BlackPearl, Bloodmoss, SpidersSilk
- **Cast Time:** 4.0s
- **Range:** 12 tiles
- **Area:** 6 tile radius
- **Effect:** Deals 50-80 cold damage to all enemies, freezes all for 3 seconds, creates frozen ground
- **Visual:** Massive blue explosion (0x36BD)
- **Sound:** 0x307
- **Purpose:** Ultimate AoE nuke

#### 26. Glacier Summon
- **Mana:** 40
- **Reagents:** MandrakeRoot, Bloodmoss, SpidersSilk, BlackPearl
- **Cast Time:** 3.5s
- **Duration:** 180 seconds
- **Effect:** Summons an Ice Elemental (700 HP, strong cold attacks, immune to cold)
- **Visual:** Ice forming into elemental (0x3779)
- **Sound:** 0x1F8
- **Purpose:** Powerful summon

#### 27. Eternal Winter
- **Mana:** 40
- **Reagents:** MandrakeRoot, Nightshade, Bloodmoss, BlackPearl, SpidersSilk, Garlic
- **Cast Time:** 3.75s
- **Range:** 12 tiles (ground target)
- **Area:** 8 tile radius
- **Duration:** 30 seconds
- **Effect:** Zone becomes frozen wasteland: 5-10 damage/tick to enemies, slows 50%, visibility reduced
- **Visual:** Constant blizzard effect (0x3709 + fog)
- **Sound:** 0x64F (looping)
- **Purpose:** Zone control ultimate

#### 28. Fimbulwinter's Wrath
- **Mana:** 40
- **Reagents:** All 8 reagents
- **Cast Time:** 4.0s
- **Range:** Self (15 tile radius)
- **Duration:** 15 seconds
- **Effect:** Caster becomes avatar of winter: +50% cold damage, immunity to cold/fire, all spells cost 50% less mana, aura deals 10 damage/second to enemies
- **Visual:** Ice aura with snowflakes (0x375A + 0x3779)
- **Sound:** 0x307
- **Purpose:** Transformation ultimate

---

### Circle 8 (50 mana) - Legendary Ice
*Mana: 50 | Cast Time: 4.0-5.0s*

#### 29. Frost Meteor
- **Mana:** 50
- **Reagents:** All 8 reagents
- **Cast Time:** 5.0s
- **Range:** 20 tiles (ground target)
- **Area:** 10 tile radius
- **Effect:** Calls down massive ice meteor dealing 80-120 damage, creates frozen terrain (20s), slows all enemies 75%
- **Visual:** Falling ice meteor (0x36D4)
- **Sound:** 0x664
- **Purpose:** Devastating AoE nuke

#### 30. Ice Age
- **Mana:** 50
- **Reagents:** All 8 reagents + FrozenOre (5)
- **Cast Time:** 4.5s
- **Area:** Entire screen (30 tile radius)
- **Duration:** 20 seconds
- **Effect:** Everything freezes: All enemies slowed 90%, take 8-15 damage/tick, chance to freeze solid each tick
- **Visual:** Everything covered in ice and snow
- **Sound:** 0x307
- **Purpose:** Screen-wide devastation

#### 31. Rime Reaper
- **Mana:** 50
- **Reagents:** All 8 reagents
- **Cast Time:** 4.0s
- **Range:** 15 tiles
- **Effect:** Massive ice scythe sweeps in arc, dealing 100-150 damage, instant kill if target below 20% HP (shatters into ice)
- **Visual:** Giant ice scythe slash (0x13F9 + ice particles)
- **Sound:** 0x232
- **Purpose:** Execute ability

#### 32. Cocytus Prison
- **Mana:** 50
- **Reagents:** All 8 reagents + EternalIce (3)
- **Cast Time:** 4.5s
- **Range:** 12 tiles
- **Duration:** Until broken (max 60s)
- **Effect:** Target is imprisoned in unbreakable ice (500 HP to break), completely disabled. Caster channels to maintain (can't cast other spells).
- **Visual:** Massive ice prison (0x3779 scaled 2x)
- **Sound:** 0x1F8
- **Purpose:** Ultimate single-target lockdown

---

## Spell Progression

**Early Game (Circles 1-3):** Basic damage, slows, and protection
**Mid Game (Circles 4-5):** AoE damage, hard CC, combo potential
**Late Game (Circles 6-7):** Zone control, massive AoE, summons
**End Game (Circle 8):** Devastating ultimates, executes, screen-wide effects

---

## Reagent Usage Summary

**Primary Reagents:**
- MandrakeRoot (26 spells) - Primary offensive reagent
- SulfurousAsh (17 spells) - Damage amplifier
- Bloodmoss (16 spells) - DoT and debuffs
- Nightshade (11 spells) - CC and slows
- BlackPearl (13 spells) - High-tier damage
- Garlic (10 spells) - Defensive/buff spells
- Ginseng (8 spells) - Healing/protection
- SpidersSilk (13 spells) - Barriers and control

**Special Resources:**
- FrozenOre - 1 spell (Ice Age)
- EternalIce - 1 spell (Cocytus Prison)

---

## Mana Cost Distribution

- **Circle 1:** 4 mana × 4 spells = 16 total
- **Circle 2:** 6 mana × 4 spells = 24 total
- **Circle 3:** 9 mana × 4 spells = 36 total
- **Circle 4:** 11 mana × 4 spells = 44 total
- **Circle 5:** 14 mana × 4 spells = 56 total
- **Circle 6:** 20 mana × 4 spells = 80 total
- **Circle 7:** 40 mana × 4 spells = 160 total
- **Circle 8:** 50 mana × 4 spells = 200 total

---

## Implementation Notes

### Already Implemented (3 spells):
✓ Ice Bolt (Circle 3)
✓ Frost Armor (Circle 4)
✓ Blizzard (Circle 6)

### Implementation Priority:
1. **High Priority:** Circles 1-2 (starter spells)
2. **Medium Priority:** Circles 3-5 (core gameplay)
3. **Low Priority:** Circles 6-7 (advanced)
4. **Future:** Circle 8 (legendary spells)

### ServUO API Patterns:
- All spells inherit from `MagerySpell`
- Namespace: `Server.Spells.VystiaSpells.IceMage`
- Use `GetNewAosDamage()` for damage calculations
- Use `SpellHelper.Damage()` for applying damage
- Cold damage: `SpellHelper.Damage(spell, target, damage, 0, 0, 100, 0, 0)`
- Slows: Use `StatMod` for DEX reduction
- Buffs: Use `ResistanceMod` (2 args: type, offset)

---

## ServUO Implementation Notes

### ✅ Standard Mechanics (Built-in ServUO)

**Direct Damage Spells:**
- Use `GetNewAosDamage(base, dice, sides, target)` for scaling damage
- Apply via `SpellHelper.Damage(spell, target, damage, phys%, fire%, cold%, pois%, nrgy%)`
- Example: Ice Bolt, Ice Shard, Glacial Strike

**Slow/Root Effects:**
- **Slow:** Apply via `StatMod(StatType.Dex, "ModName", -amount, duration)`
- **Root:** Set `target.Frozen = true` with Timer to unfreeze
- Remove via `target.RemoveStatMod("ModName")` or `target.Frozen = false`
- Example: All slow spells, Permafrost (root)

**Resistance Buffs:**
- Create: `ResistanceMod(ResistanceType.Cold, +amount)`
- Apply: `target.AddResistanceMod(mod)`
- Remove: `target.RemoveResistanceMod(mod)` - must pass same object reference
- Store mod reference in Timer lambda for delayed removal
- Example: Frost Ward, Frost Armor, Ice Shield

**DoT (Damage over Time):**
- Use `Timer.DelayCall()` with TimeSpan intervals
- Create timer that repeats and deals damage each tick
- Store timer reference to allow early cancellation
- Example: Frozen Ground, Blizzard, Frostbite

**AoE Spells:**
- Use `map.GetMobilesInRange(location, radius)` to get targets
- Filter with `eable.Free()` after iteration
- Check `SpellHelper.ValidIndirectTarget()` for valid targets
- Apply effects to each valid target
- Example: All AoE spells

### 🔧 Custom Mechanics (Require Implementation)

**Ice Walls / Barriers:**
- **Ice Wall Spell:** Needs custom `IceWallItem` class inheriting from `Item`
- Set `Movable = false`, blocking properties
- Place as static items at target locations
- HP-based destruction: Override `OnDamage()` to track HP, delete when broken
- Timer-based expiration: `Timer.DelayCall(duration, () => wall.Delete())`
- Blocks movement via `CanFit()` checks
- Example: Ice Wall (Circle 4)

```csharp
public class IceWallItem : Item
{
    private int m_HP;

    public IceWallItem() : base(0x80) // Wall graphic
    {
        Movable = false;
        m_HP = 100;
    }

    public override void OnDamage(int amount, Mobile from)
    {
        m_HP -= amount;
        if (m_HP <= 0)
            Delete();
    }
}
```

**Frozen Ground / Terrain Effects:**
- **Method 1:** Place invisible items at ground locations
  - Create `FrozenGroundItem : Item` with timer
  - Override `OnMoveOver(Mobile m)` to apply effects
  - Delete after duration expires

- **Method 2:** Track affected tiles in dictionary
  - Store `Dictionary<Point3D, Timer>` of active frozen tiles
  - Check in `Mobile.OnMovement` event (requires hook)
  - More complex but no item spam

**Combo Mechanics (Shatter bonus vs slowed):**
- Check target for active `StatMod` with slow name
- Or check custom bool flag: `target.GetTempBool("IsSlowed")`
- Apply damage multiplier if condition met
- Example: Shatter deals +50% if target slowed

```csharp
// In Shatter spell
bool isSlowed = target.GetStatMod("IceBolt_Slow") != null;
if (isSlowed)
    damage *= 1.5;
```

**Freeze/Stun Effects:**
- **Method 1:** Use `target.Frozen = true` (built-in)
  - Prevents movement but allows casting
  - Timer to set `Frozen = false` after duration

- **Method 2:** Use `target.Paralyzed = true` (built-in)
  - Full stun (no movement or actions)
  - More powerful but harder to balance

**Ice Fortresses / Domes:**
- Create custom `IceFortress` buff tracking system
- Store active fortress in player property: `m_ActiveFortress`
- Apply resistance buffs when fortress active
- Reflect damage in `Mobile.OnDamagedBy()` hook
- Visual: Persistent `FixedEffect()` at player location

**Ice Transformation (Avatar spells):**
- Use `BodyMod` or `BodyValue` to change appearance
- Apply multiple stat/resistance buffs
- Set flag: `m_IceAvatar = true` to track state
- Remove all buffs when transformation ends
- Example: Fimbulwinter's Wrath, Ice Age transformation

### ⚠️ Advanced Mechanics (Complex Implementation)

**Projectile Blocking (Glacial Fortress):**
- Requires hooking into projectile system
- **Difficult:** ServUO doesn't have easy projectile interception
- **Alternative:** Increase ranged defense/dodge instead
- Or apply damage reduction vs. ranged attacks

**Knockback Effects:**
- Calculate direction: `from.GetDirectionTo(target)`
- Move target: `target.Location = GetKnockbackLocation()`
- Validate: Check `map.CanFit()` before moving
- Play movement animation
- Example: Avalanche knockback

**Conditional Spell Interactions:**
- Track debuffs via custom properties or dictionary
- Check conditions before applying effects
- Example: Lava Burst checks for Flame Shock (Elemental Magic)
- Cross-spell combos require state tracking

### 📝 Implementation Checklist per Spell

1. **Basic Damage Spell:**
   - ✅ Inherit from `MagerySpell`
   - ✅ Define `SpellInfo` with reagents
   - ✅ Override `Circle` property
   - ✅ Implement `OnCast()` with targeting
   - ✅ Calculate damage with `GetNewAosDamage()`
   - ✅ Apply with `SpellHelper.Damage()`

2. **Buff/Debuff Spell:**
   - ✅ Create `ResistanceMod` or `StatMod`
   - ✅ Apply to target
   - ✅ Store mod reference in variable
   - ✅ Use `Timer.DelayCall()` for expiration
   - ✅ Remove mod in timer callback
   - ✅ Check for null/deleted target before removing

3. **AoE Spell:**
   - ✅ Get ground target via `Target` class
   - ✅ Use `map.GetMobilesInRange()`
   - ✅ Filter valid targets
   - ✅ Apply effect to each target
   - ✅ Call `eable.Free()` after iteration
   - ✅ Play visual effects at location

4. **DoT Spell:**
   - ✅ Apply initial effect
   - ✅ Create repeating timer (not DelayCall)
   - ✅ Track tick count
   - ✅ Deal damage each tick
   - ✅ Stop timer after max ticks
   - ✅ Store timer reference for early cancellation

### 🎯 Priority Implementation Order

**Phase 1 - Basic Systems:**
- Direct damage spells (Ice Bolt already done ✅)
- Simple buffs/debuffs (Frost Armor already done ✅)
- Slows via StatMod
- Basic DoTs

**Phase 2 - Intermediate:**
- AoE damage (Blizzard already done ✅)
- Frozen ground terrain effects
- Roots/freezes
- Ice walls

**Phase 3 - Advanced:**
- Combo mechanics (Shatter)
- Transformations (avatar forms)
- Complex interactions
- Projectile effects

---

## Balance Considerations

**PvE:**
- Excellent for kiting ranged enemies
- Strong AoE for multiple targets
- Defensive options for survival
- Good at controlling space

**PvP:**
- High skill cap (requires positioning)
- Strong against melee classes
- Vulnerable during long casts
- Countered by fire mages and high mobility

**Synergies:**
- Combos: Slow → Shatter for bonus damage
- Defensive: Frost Armor → Ice Shield → Glacial Fortress
- Offensive: Frozen Ground → Blizzard → Absolute Zero
- Control: Permafrost → Deep Freeze → Cocytus Prison

---

**Last Updated:** 2025-12-05
**Status:** Design Complete - 3/32 Implemented
**Next Steps:** Implement Circle 1-2 spells for early game viability
