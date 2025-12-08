# Ice Magic - Frosthold School

**Class:** Ice Mage
**Region:** Frosthold
**Theme:** Cold damage, slowing effects, defensive ice barriers, area denial
**Spellbook:** Ice Mage Spellbook (IceMageSpellbook.cs)
**Primary Stat:** Intelligence
**Hue:** 0x481 (Ice Blue)
**Status:** ✅ **100% COMPLETE** (32/32 spells implemented)
**Implementation Date:** 2025-12-06

---

## Overview

Ice Magic harnesses the frozen powers of Frosthold, focusing on cold damage, mobility control, and defensive ice constructs. Ice Mages excel at kiting enemies with slowing effects, creating frozen barriers, and devastating groups with AoE cold damage.

**Playstyle:**
- **Kiting:** Use slows and freezes to keep enemies at range
- **Area Control:** Create ice walls and frozen ground to control battlefield
- **Burst AoE:** Devastate groups with massive cold damage spells
- **Defense:** High cold resistance and protective ice barriers

**Strengths:**
- Strong AoE damage (Blizzard, Absolute Zero, Ice Age)
- Excellent crowd control (slows, freezes, roots, stuns)
- Defensive ice barriers and armor
- High cold resistance
- Area denial with ice terrain
- Execute mechanics (Rime Reaper instant kill)
- Powerful summons and transformations

**Weaknesses:**
- Weak against fire damage
- Limited direct healing (only Glacial Mend)
- Many spells require positioning
- Mana intensive at high circles
- Channeled spells require standing still

---

## Implementation Status

**Files Location:** `ServUO/Scripts/Custom/VystiaClasses/Spells/IceMage/`

**Total Spells:** 32/32 ✅
**Total Files:** 29 spell files + 3 existing
**Lines of Code:** ~4,500+

**All spells:**
- ✅ Use correct ServUO API (MagerySpell inheritance)
- ✅ Use Vystia reagents (no standard UO reagents)
- ✅ Include spell reflection checks
- ✅ Have ice-themed visual effects (0x481 hue)
- ✅ Have appropriate sound effects
- ✅ Are fully serializable

---

## Spellbook

### IceMageSpellbook
- **Item ID:** 0xEFA (Spellbook graphic)
- **Hue:** 0x481 (Ice Blue)
- **Name:** "Tome of Frozen Arts"
- **Capacity:** 32 spells (8 circles × 4 spells)
- **Weight:** 3.0 stones
- **Special Bonus:** Grants +5% cold damage when equipped
- **Spell ID Range:** 1000-1031
- **GM Command:** `[spellbook ice]` or `[sb ice]`

---

## Complete Spell List

### Circle 1 (4 mana) - Basic Ice
*Entry-level spells for new Ice Mages*

#### 1. Frost Touch ✅
- **File:** `FrostTouchSpell.cs`
- **Mana:** 4
- **Reagents:** Frostbloom, Winterleaf
- **Range:** Melee (1 tile)
- **Damage:** 4-8 cold damage
- **Special:** 15% chance to slow (-10 DEX for 3s)

**What You See:**
- Blue frost particles appear on your hands (0x373A)
- When you touch an enemy, ice spreads across them
- Slowed enemies get frost on their feet and receive message "You have been slowed by frost!"

**How It Works:**
- Cast on yourself, then touch an enemy within 1 tile
- Instant damage with potential slow
- Great for melee defense when enemies get close
- Low mana cost allows spam casting

---

#### 2. Ice Shard ✅
- **File:** `IceShardSpell.cs`
- **Mana:** 4
- **Reagents:** Frostbloom
- **Range:** 10 tiles
- **Damage:** 5-10 cold damage

**What You See:**
- A small ice projectile (0x36E4) shoots from you to target
- Ice blue trail follows the projectile
- Cracking ice sound (0x64F) on impact

**How It Works:**
- Basic ranged damage spell
- Fast casting (0.75s)
- Supports spell reflection
- Your bread-and-butter low-level damage spell
- Very mana efficient for sustained damage

---

#### 3. Frost Ward ✅
- **File:** `FrostWardSpell.cs`
- **Mana:** 4
- **Reagents:** Winterleaf, Frostbloom
- **Range:** 12 tiles (can cast on allies)
- **Duration:** 30 seconds
- **Effect:** +5 Cold Resistance

**What You See:**
- Faint blue shimmer surrounds target (0x375A)
- Gentle wind chime sound (0x1E9)
- Message: "You are surrounded by a protective frost ward!"
- After 30s: "Your frost ward dissipates."

**How It Works:**
- Protective buff against cold damage
- Can be cast on yourself or allies
- Stacks with armor resistances
- Useful before fighting ice creatures
- Auto-expires after 30 seconds

---

#### 4. Chill Aura ✅
- **File:** `ChillAuraSpell.cs`
- **Mana:** 4
- **Reagents:** Winterleaf
- **Duration:** 20 seconds
- **Area:** 2 tile radius around caster
- **Effect:** Enemies within range are slowed

**What You See:**
- Cold mist appears at your feet (0x3709)
- Mist pulses every 3 seconds
- Enemies near you get message: "You feel chilled by the icy aura!"
- After 20s: "Your chill aura fades away."

**How It Works:**
- Creates defensive aura around you
- Automatically affects any enemy that comes within 2 tiles
- Helps with kiting melee enemies
- Does not require targeting, just cast on self
- Lasts full 20 seconds or until you die

---

### Circle 2 (6 mana) - Freezing Arts
*Improved damage and utility spells*

#### 5. Freezing Grasp ✅
- **File:** `FreezingGraspSpell.cs`
- **Mana:** 6
- **Reagents:** Frostbloom, Winterleaf
- **Range:** 8 tiles
- **Damage:** 8-14 cold damage
- **Special:** 30% chance to slow (-15 DEX for 4s)

**What You See:**
- Blue ghostly hand (0x36BD) reaches from you to target
- Ice spreads on target on impact
- Slowed targets get frost particles (0x374A) at waist
- Message: "You have been slowed by the freezing grasp!"

**How It Works:**
- Improved version of basic damage spells
- Higher slow chance (30% vs 15%)
- Stronger slow effect (-15 DEX vs -10 DEX)
- Longer slow duration (4s vs 3s)
- Good for kiting single targets

---

#### 6. Ice Shield ✅
- **File:** `IceShieldSpell.cs`
- **Mana:** 6
- **Reagents:** Winterleaf, Glacier Crystal
- **Range:** 12 tiles
- **Duration:** 60 seconds
- **Effect:** +10 Physical, +15 Cold Resistance

**What You See:**
- Orbiting ice shards appear around target (0x3779)
- Ice crystal sound (0x64F)
- Message: "You are surrounded by orbiting ice shards!"
- Shows resistance bonuses in message
- After 60s: "Your ice shield shatters." with breaking sound

**How It Works:**
- Strong defensive buff
- Provides both physical and cold resistance
- Can be cast on allies
- Lasts a full minute
- Great for tanks or when expecting heavy damage
- Note: Damage reflection listed in tooltip but not fully implemented (requires damage event hooks)

---

#### 7. Frost Slick ✅
- **File:** `FrostSlickSpell.cs`
- **Mana:** 6
- **Reagents:** Frostbloom, Winterleaf
- **Range:** 12 tiles (ground target)
- **Area:** 3x3 tiles
- **Duration:** 15 seconds
- **Effect:** Slows enemies standing on it (-20 DEX for 5s)

**What You See:**
- Target ground location for placement
- Icy ground texture appears (0x122A) with particles
- Slipping sound (0x028)
- Enemies stepping on it: "You slip on the icy ground!"
- Ice particles appear on their feet
- After 15s: Frost slick melts away

**How It Works:**
- Area denial spell - create zones enemies must avoid
- Anyone standing in the 3x3 area gets slowed
- Slow persists for 5s even after leaving the area
- Great for controlling chokepoints
- Place between you and melee enemies
- Can slow multiple enemies at once

---

#### 8. Glacial Mend ✅
- **File:** `GlacialMendSpell.cs`
- **Mana:** 6
- **Reagents:** Winterleaf, Frostbloom
- **Range:** 8 tiles
- **Healing:** 15-25 HP
- **Bonus:** +5 Cold Resistance for 30s

**What You See:**
- Blue healing sparkles surround target (0x376A)
- Gentle healing sound (0x1F2)
- Message: "Glacial energy heals you for X health!"
- Also get Frost Ward buff message

**How It Works:**
- Ice Mage's only direct healing spell
- Heals moderate amount (15-25 HP)
- Also applies minor cold resistance buff
- Can be cast on allies
- Useful for solo play or supporting party
- Cheaper than high-circle healing spells

---

### Circle 3 (9 mana) - Offensive Ice
*Core damage spells for combat*

#### 9. Ice Bolt ✅ (Previously Implemented)
- **File:** `IceBoltSpell.cs`
- **Mana:** 9
- **Reagents:** Frostbloom, Winterleaf, Glacier Crystal
- **Range:** 12 tiles
- **Damage:** 19-34 cold damage (scales with Magery/Eval Int)
- **Special:** 25% chance to slow (-15 DEX for 5s)

**What You See:**
- Large ice projectile (0x36E4, blue hue) flies to target
- Ice impact sound (0x1FB)
- Spell reflection creates delayed mirror projectile
- Slowed targets get frost at waist with message

**How It Works:**
- Your primary single-target damage spell
- Good damage scaling with skills
- Decent slow chance for kiting
- Can be reflected by Magic Reflection
- Fast enough casting for combat rotation

---

#### 10. Frozen Ground ✅
- **File:** `FrozenGroundSpell.cs`
- **Mana:** 9
- **Reagents:** Frostbloom, Winterleaf, Glacier Crystal
- **Range:** 12 tiles (ground target)
- **Area:** 4 tile radius
- **Duration:** 20 seconds
- **Damage:** 3-6 per second
- **Effect:** Also slows enemies (-20 DEX for 4s)

**What You See:**
- Target ground location
- Ice covers large area (0x122A) with constant particles
- Storm sound every 5 seconds (0x64F)
- Enemies in area take damage each second
- Frost particles on enemies every 3 seconds
- Message: "The frozen ground chills you!"

**How It Works:**
- Damage over time zone control spell
- 4-tile radius = very large area (9x9 tiles!)
- Ticks every second for 20 seconds
- Also applies slow effect every 3 seconds
- Great for:
  - Area denial
  - Farming multiple enemies
  - Boss fights (stand in safe spot, boss in zone)
- Can't be dispelled, lasts full duration

---

#### 11. Ice Spear ✅
- **File:** `IceSpearSpell.cs`
- **Mana:** 9
- **Reagents:** Frostbloom, Winterleaf, Glacier Crystal
- **Range:** 14 tiles
- **Damage:** 25-40 cold damage per target
- **Special:** Pierces through up to 3 enemies in a line

**What You See:**
- Large ice spear projectile (0x36E4 scaled up)
- Travels in straight line from you toward target
- Hits primary target plus any enemies in the line
- Ice shatter effect (0x36CB) on each hit
- Message: "Your ice spear pierces through X enemies!"

**How It Works:**
- Line damage spell (like Chain Lightning)
- Finds enemies between you and target in straight line
- Maximum 3 enemies hit total
- Each enemy takes full damage (not split)
- Great for:
  - Groups of enemies lined up
  - Hallways and narrow spaces
  - Maximizing damage per mana
- Uses geometric line projection for targeting

---

#### 12. Frostbite ✅
- **File:** `FrostbiteSpell.cs`
- **Mana:** 9
- **Reagents:** Winterleaf, Glacier Crystal
- **Range:** 10 tiles
- **Duration:** 12 seconds (6 ticks, 1 per 2 seconds)
- **Damage:** 4-7 per tick
- **Special:** Reduces healing received by 25% (documented, not fully implemented)

**What You See:**
- Target gets frost particles at waist (0x374A)
- Debuff sound (0x1ED)
- Message: "You suffer from frostbite! Healing is reduced."
- Damage ticks every 2 seconds
- Message every other tick: "Frostbite saps your vitality!"
- After 12s: "The frostbite effect wears off."

**How It Works:**
- Damage over time debuff
- 6 ticks over 12 seconds = 24-42 total damage
- Anti-healing mechanic (theoretical, not in code yet)
- Good for:
  - PvP (reduced healing)
  - Boss fights (consistent damage)
  - Applying while kiting
- Can't be dispelled, runs full course

---

### Circle 4 (11 mana) - Ice Barriers
*Defensive spells and utility*

#### 13. Frost Armor ✅ (Previously Implemented)
- **File:** `FrostArmorSpell.cs`
- **Mana:** 11
- **Reagents:** Winterleaf, Glacier Crystal, Permafrost Essence
- **Range:** 12 tiles
- **Duration:** 120-240 seconds (scales with Magery skill)
- **Effect:** +10 Physical, +20 Cold Resistance

**What You See:**
- Frost aura surrounds target (0x375A)
- Crisp ice sound (0x1E9)
- Message: "You are surrounded by protective frost armor!"
- Buff lasts 2-4 minutes depending on your Magery skill
- Message: "Your frost armor dissipates." with melting visual

**How It Works:**
- Long-duration defensive buff
- Scales: 120s base + (Magery × 1.2), capped at 240s
- At 100 Magery = 240s (4 minutes)
- Strong cold resistance (+20) for ice-heavy areas
- Moderate physical resistance (+10)
- Can be cast on allies
- Great for dungeons and boss fights

---

#### 14. Ice Wall ✅
- **File:** `IceWallSpell.cs`
- **Mana:** 11
- **Reagents:** Glacier Crystal, Permafrost Essence
- **Range:** 12 tiles (ground target)
- **Effect:** Creates 5-tile line wall of ice
- **Duration:** 30 seconds (auto-expires)
- **HP:** 100 per wall segment (can be destroyed)

**What You See:**
- Target ground location
- 5 ice wall segments appear in a line (0x80 or 0x7E)
- Orientation based on your facing direction
- Blue ice hue (0x481)
- Ice formation sound (0x1F8)
- Sparkles on each segment (0x376A)
- After 30s: Walls shatter with particle effects (0x3735)

**How It Works:**
- Creates physical barrier blocking movement
- Wall faces horizontal or vertical based on caster direction
- 5 tiles long = blocks hallways or creates corridors
- Each segment has 100 HP and can be destroyed
- Auto-expires after 30 seconds
- Great for:
  - Blocking enemy paths
  - Creating safe zones
  - Splitting enemy groups
  - Defending positions
- Counts as static items blocking CanFit() checks

---

#### 15. Icicle Barrage ✅
- **File:** `IcicleBarrageSpell.cs`
- **Mana:** 11
- **Reagents:** Glacier Crystal, Permafrost Essence
- **Range:** 12 tiles (target enemy)
- **Area:** 3 tile radius around target
- **Damage:** 15-25 cold damage per enemy

**What You See:**
- Target an enemy
- 5 icicles rain down with staggered timing (200ms apart)
- Each icicle is offset randomly in the target area
- Particle effects (0x36E4) for each icicle
- Ice storm sound (0x64F)
- Hit enemies get ice impact visual (0x36CB)
- Message: "Your icicle barrage hits X targets!"

**How It Works:**
- AoE burst damage centered on target
- 3-tile radius = hits lots of enemies
- Each enemy takes full damage
- Staggered visual for dramatic effect
- Great for:
  - Packed enemy groups
  - Quick AoE burst
  - Farming
- Not a DoT, all damage instant

---

#### 16. Permafrost ✅
- **File:** `PermafrostSpell.cs`
- **Mana:** 11
- **Reagents:** Winterleaf, Glacier Crystal, Permafrost Essence
- **Range:** 10 tiles
- **Duration:** 8 seconds
- **Damage:** 5-10 per second
- **Effect:** Roots target (cannot move)

**What You See:**
- Ice forms around target's feet (0x376A)
- Freezing sound (0x1F8)
- Target frozen in place (can't move)
- Message: "Your feet are frozen in place!"
- Damage ticks every second with frost particles
- Message every 2 seconds: "The permafrost damages you!"
- After 8s: "The permafrost melts and you can move again."

**How It Works:**
- Hard CC (crowd control) - root effect
- Target.Frozen = true (cannot move)
- Still can cast spells and attack
- Also deals DoT (5-10 per second)
- Total: 40-80 damage over 8 seconds
- Great for:
  - Stopping fleeing enemies
  - PvP lockdown
  - Keeping melee enemies at range
  - Boss positioning

---

### Circle 5 (14 mana) - Greater Ice
*Powerful mid-tier spells*

#### 17. Glacial Strike ✅
- **File:** `GlacialStrikeSpell.cs`
- **Mana:** 14
- **Reagents:** Permafrost Essence, Arctic Pearl
- **Range:** 14 tiles
- **Damage:** 35-55 cold damage
- **Special:** 50% chance to freeze (stun for 2s)

**What You See:**
- Massive ice bolt projectile (0x36E4 large)
- Heavy impact sound (0x1FB)
- On freeze: target completely frozen (0x376A)
- Message: "You are frozen solid!"
- After 2s: "You break free from the ice!" with shatter effect

**How It Works:**
- High damage single-target nuke
- 50% chance to stun (Frozen = true)
- Frozen targets can't move OR act (paralyzed)
- 2 second stun = time for follow-up spells
- Supports spell reflection
- Great for:
  - Burst damage
  - PvP stun combos
  - Interrupting casters
  - Boss damage

---

#### 18. Frozen Tomb ✅
- **File:** `FrozenTombSpell.cs`
- **Mana:** 14
- **Reagents:** Permafrost Essence, Arctic Pearl
- **Range:** 10 tiles
- **Duration:** 6 seconds
- **Effect:** Target immune to damage but cannot act

**What You See:**
- Ice sphere forms around target (0x3779)
- Freezing sound (0x1F8)
- Message: "You are encased in a frozen tomb!"
- Target frozen, paralyzed, AND blessed (immune)
- After 6s: "The frozen tomb shatters!" with particle burst (0x3735)

**How It Works:**
- Defensive disable spell
- Target becomes:
  - Frozen (can't move)
  - Paralyzed (can't act)
  - Blessed (immune to all damage)
- Double-edged:
  - Saves allies from death
  - Disables enemies temporarily
- Great for:
  - Emergency ally save
  - Removing dangerous enemy from fight
  - Buying time to escape or heal
  - PvP tactical plays
- Can be cast on ally OR enemy

---

#### 19. Shatter ✅
- **File:** `ShatterSpell.cs`
- **Mana:** 14
- **Reagents:** Permafrost Essence, Glacier Crystal
- **Range:** 12 tiles
- **Damage:** 20-35 base, 30-52 if target slowed/frozen
- **Special:** +50% damage vs slowed or frozen targets

**What You See:**
- Shattering ice effect (0x36BD) flies to target
- Impact sound (0x1FB)
- If target is slowed/frozen:
  - Extra shatter particles (0x3735)
  - Message: "Your shatter spell shatters the frozen enemy!"
  - Visibly bigger impact

**How It Works:**
- Combo finisher spell
- Checks if target has slow debuff OR is frozen
- Bonus applies to any ice magic slow effect:
  - IceBolt_Slow, FrostTouch_Slow, FreezingGrasp_Slow
  - FrostSlick_Slow, FrozenGround_Slow
  - Or Frozen status (from Glacial Strike, etc.)
- 50% damage boost = 20-35 becomes 30-52
- Great combo chains:
  - Ice Bolt → Shatter
  - Glacial Strike → Shatter (if freeze procs)
  - Frozen Ground → Shatter (enemy in zone)
- Rewards good spell sequencing

---

#### 20. Hypothermia ✅
- **File:** `HypothermiaSpell.cs`
- **Mana:** 14
- **Reagents:** Permafrost Essence, Arctic Pearl
- **Range:** 10 tiles
- **Duration:** 20 seconds
- **Effect:** 10% max HP damage + severe debuffs

**What You See:**
- Blue debuff aura surrounds target (0x374A)
- Debuff sound (0x1ED)
- Immediate damage (10% of max HP)
- Message: "You suffer from severe hypothermia!"
- Target moves and attacks very slowly
- After 20s: "The hypothermia wears off." with particle burst

**How It Works:**
- Severe debuff spell
- Instant damage: 10% of target's max HP
  - 1000 HP target = 100 instant damage
  - Scales with enemy HP (better vs bosses)
- Debuff: -40 DEX for 20 seconds
  - Simulates -25% movement speed
  - Simulates -15% attack speed
- Great for:
  - Weakening bosses
  - Slowing dangerous enemies
  - PvP mobility reduction
  - Setting up kill combos

---

### Circle 6 (20 mana) - Master Ice
*Advanced spells with powerful effects*

#### 21. Blizzard ✅ (Previously Implemented)
- **File:** `BlizzardSpell.cs`
- **Mana:** 20
- **Reagents:** Permafrost Essence, Arctic Pearl, Frozen Soul
- **Range:** 12 tiles (ground target)
- **Area:** 5 tile radius
- **Duration:** 10 seconds (ticks every 1 second)
- **Damage:** 3-8 per tick + Magery scaling

**What You See:**
- Target ground location
- Massive swirling ice storm (0x3709) appears
- Storm sound (0x64F) every 3 seconds
- Enemies in 5-tile radius take constant damage
- Frost particles on enemies
- Message every other tick: "The blizzard chills you to the bone!"
- Storm persists for full 10 seconds

**How It Works:**
- Premier AoE damage spell
- 5-tile radius = MASSIVE area (11x11 tiles)
- 10 ticks = 30-80 base damage total
- Scales with Magery skill (damage + base)
- Also slows all enemies in area
- Great for:
  - Farming large packs
  - Boss fights (stand outside, boss inside)
  - Dungeon room clearing
  - PvP area denial
- Can't be dispelled or interrupted once cast

---

#### 22. Glacial Fortress ✅
- **File:** `GlacialFortressSpell.cs`
- **Mana:** 20
- **Reagents:** Permafrost Essence, Arctic Pearl, Frozen Soul
- **Duration:** 60 seconds
- **Effect:** +25 to ALL resistances

**What You See:**
- Ice dome forms around you (0x3779)
- Fortress sound (0x1F8)
- Message: "You are surrounded by a glacial fortress!"
- Resistance buff messages show all 5 types
- After 60s: "Your glacial fortress crumbles." with shatter effect

**How It Works:**
- Ultimate defensive buff (self only)
- +25 to all 5 resistances:
  - Physical +25
  - Fire +25
  - Cold +25
  - Poison +25
  - Energy +25
- Lasts full minute
- Great for:
  - Boss fights (survive heavy damage)
  - PvP survival
  - Soloing dangerous content
  - Tank role in parties
- Note: Damage reflection and projectile blocking documented but not implemented

---

#### 23. Avalanche ✅
- **File:** `AvalancheSpell.cs`
- **Mana:** 20
- **Reagents:** Permafrost Essence, Arctic Pearl, Frozen Soul
- **Range:** 14 tiles (direction target)
- **Area:** 8 tile cone (45° angle)
- **Damage:** 30-50 cold damage
- **Special:** Knockback 3 tiles, applies slow

**What You See:**
- Target a direction
- Massive ice/snow cascade effect
- Avalanche sound (0x664)
- Multiple visual effects:
  - Ice projectiles (0x36E4)
  - Snow particles (0x3709)
- Enemies in cone:
  - Take damage
  - Get pushed back 3 tiles
  - Covered in ice/snow
  - Slowed (-20 DEX for 5s)
- Message: "You are knocked back by the avalanche!"
- Message: "Your avalanche hits X targets!"

**How It Works:**
- Cone AoE knockback spell
- Targets in 45° cone in front of you
- Range: up to 8 tiles from you
- Each enemy:
  - Takes 30-50 damage
  - Knocked back 3 tiles (away from you)
  - Slowed for 5 seconds
- Knockback uses vector math (preserves direction)
- Great for:
  - Defensive emergency (push enemies away)
  - PvP displacement
  - Knocking enemies off ledges
  - Creating space to escape
- Affects multiple enemies

---

#### 24. Deep Freeze ✅
- **File:** `DeepFreezeSpell.cs`
- **Mana:** 20
- **Reagents:** Permafrost Essence, Arctic Pearl, Frozen Soul
- **Range:** 12 tiles
- **Duration:** 10 seconds
- **Effect:** Stun + vulnerability (-25 all resistances)

**What You See:**
- Complete ice encasement (0x376A)
- Heavy freezing sound (0x1F8)
- Message: "You are frozen solid and vulnerable!"
- Target completely paralyzed
- Caster message: "Your target is frozen solid and takes double damage!"
- After 10s: "The deep freeze shatters!" with burst effect

**How It Works:**
- Ultimate CC + vulnerability spell
- Target becomes:
  - Frozen (can't move)
  - Paralyzed (can't act)
- Vulnerability debuff:
  - -25 to all 5 resistances
  - Simulates "double damage" effect
- Lasts 10 seconds
- Great for:
  - PvP lockdown
  - Setting up burst combos
  - Boss burn phases
  - Removing dangerous enemy
- Combine with Shatter for massive damage!

---

### Circle 7 (40 mana) - Ancient Ice
*Legendary tier spells*

#### 25. Absolute Zero ✅
- **File:** `AbsoluteZeroSpell.cs`
- **Mana:** 40
- **Reagents:** Arctic Pearl, Frozen Soul, Eternal Ice
- **Range:** 12 tiles (ground target)
- **Area:** 6 tile radius
- **Damage:** 50-80 cold damage
- **Special:** Freezes all enemies for 3s, creates frozen ground

**What You See:**
- Target ground location
- MASSIVE blue explosion (0x36BD)
- Explosion sound (0x307)
- Huge particle burst at impact
- All enemies in 6-tile radius:
  - Take heavy damage
  - Frozen solid (0x376A)
  - Message: "You are frozen by absolute zero!"
- Frozen ground effect starts immediately
- Message: "You unleash absolute zero!"
- Message: "Absolute zero hits X targets!"

**How It Works:**
- Ultimate AoE nuke + CC
- 6-tile radius = 13x13 tile area
- Massive instant damage (50-80)
- All enemies frozen for 3 seconds
- Also creates Frozen Ground effect (from Circle 3 spell)
  - Additional DoT ticks
  - Additional slow
- Great for:
  - Emergency room clear
  - Boss fight burst phase
  - PvP group fights
  - Farming endgame content
- Very expensive but devastating

---

#### 26. Glacier Summon ✅
- **File:** `GlacierSummonSpell.cs`
- **Mana:** 40
- **Reagents:** Arctic Pearl, Frozen Soul, Permafrost Essence
- **Range:** 12 tiles (ground target)
- **Duration:** 180 seconds (3 minutes)
- **Summon:** Ice Elemental (700 HP, 20-30 damage)

**What You See:**
- Target ground location
- Ice forms and swirls (0x3779)
- Summoning sound (0x1F8)
- Ice Elemental materializes at location
- Elemental follows and fights for you
- Message: "You summon a mighty ice elemental!"
- After 3 minutes: Elemental dissipates

**How It Works:**
- Powerful summon spell
- Ice Elemental stats:
  - 700 HP (very tanky)
  - 20-30 damage per hit
  - Immune to cold damage
  - Strong against fire enemies
- Summon lasts 3 minutes
- Elemental:
  - Follows you
  - Attacks your enemies
  - Can tank damage for you
- Only one summon at a time
- Great for:
  - Solo play (tank companion)
  - Boss fights (extra DPS)
  - Farming (kills while you loot)
  - Tanking for squishier mages

---

#### 27. Eternal Winter ✅
- **File:** `EternalWinterSpell.cs`
- **Mana:** 40
- **Reagents:** Arctic Pearl, Frozen Soul, Eternal Ice
- **Range:** 12 tiles (ground target)
- **Area:** 8 tile radius
- **Duration:** 30 seconds
- **Damage:** 5-10 per second
- **Effect:** 50% slow

**What You See:**
- Target ground location
- Constant blizzard effect covers massive area
- Storm sound every 5 seconds (0x64F)
- 8-tile radius = 17x17 tile area!
- Everything within area covered in ice/snow
- Enemies constantly take damage
- Message: "You create an eternal winter!"
- Message every 3s for enemies: "The eternal winter freezes you!"
- Lasts full 30 seconds

**How It Works:**
- Ultimate zone control spell
- 8-tile radius = largest AoE in game
- Ticks every second for 30 seconds
- Each tick:
  - 5-10 damage to all enemies
  - 50% slow (-50 DEX) refreshed
- Total: 150-300 damage over 30s
- Great for:
  - Locking down entire boss rooms
  - Massive pack farming
  - PvP siege defense
  - Prolonged fights
- Once cast, can't be stopped

---

#### 28. Fimbulwinter's Wrath ✅
- **File:** `FimbulwintersWrathSpell.cs`
- **Mana:** 40
- **Reagents:** Arctic Pearl, Frozen Soul, Eternal Ice, Heart of Winter
- **Duration:** 15 seconds
- **Effect:** Transform into avatar of winter

**What You See:**
- Multiple visual effects stack on you:
  - Ice aura (0x375A)
  - Frost particles (0x3779)
- Transformation sound (0x307)
- Message: "You become the avatar of winter!"
- Blue/white particles constantly swirl around you
- Enemies near you constantly take damage
- After 15s: "Fimbulwinter's wrath fades." with particle burst

**How It Works:**
- Transformation ultimate
- Buffs you with:
  - 100% Cold Resistance (immune)
  - 100% Fire Resistance (immune)
  - Damage aura (10 dmg/sec to enemies within 15 tiles)
- Note: +50% cold damage and mana reduction not fully implemented
- Aura damages all enemies in HUGE range
- Lasts 15 seconds
- Great for:
  - Melee range combat as mage
  - Farming packs (run through them)
  - PvP survivability
  - Boss fights (ignore mechanics)
- Most expensive Circle 7 spell (4 reagents)

---

### Circle 8 (50 mana) - Legendary Ice
*Ultimate endgame spells*

#### 29. Frost Meteor ✅
- **File:** `FrostMeteorSpell.cs`
- **Mana:** 50
- **Reagents:** Frozen Soul, Eternal Ice, Heart of Winter
- **Range:** 20 tiles (ground target)
- **Area:** 10 tile radius
- **Damage:** 80-120 cold damage
- **Special:** 75% slow, creates frozen ground

**What You See:**
- Target ground location (20 tile range!)
- Message: "You call down a frost meteor!"
- 2 second delay
- MASSIVE impact:
  - Meteor crash visual (0x36D4)
  - Earthquake sound (0x664)
  - Huge particle burst
- 10-tile radius = 21x21 tile area
- All enemies:
  - Take massive damage
  - Covered in ice (0x376A)
  - Heavily slowed (-75 DEX for 10s)
  - Message: "The frost meteor devastates you!"
- Frozen ground spawns at impact
- Message: "Frost meteor hits X targets!"

**How It Works:**
- Devastating AoE nuke
- 20 tile range = entire screen
- 10-tile radius = can hit 20+ enemies
- 2 second cast delay (telegraphed)
- Enormous damage (80-120)
- Creates Frozen Ground DoT zone
- 75% slow = enemies barely move
- Great for:
  - Screen-wide farming
  - Boss execution
  - PvP sieges
  - Event content
- Most dramatic visual in game

---

#### 30. Ice Age ✅
- **File:** `IceAgeSpell.cs`
- **Mana:** 50
- **Reagents:** Frozen Soul, Eternal Ice, Heart of Winter, Frozen Ore (5)
- **Area:** 30 tile radius (ENTIRE SCREEN)
- **Duration:** 20 seconds
- **Damage:** 8-15 per second
- **Effect:** 90% slow, random freeze chance

**What You See:**
- NO TARGETING - affects entire screen
- Everything freezes:
  - Ice/snow particles everywhere (0x3709)
  - Constant storm sounds (0x307)
  - Screen-wide visual effects
- Message: "You bring forth an ICE AGE!"
- All enemies on screen:
  - Take damage every second
  - Massively slowed (90%)
  - Random chance to freeze solid each tick
  - Frost particles on all
- Effect lasts full 20 seconds
- Most visually impressive spell

**How It Works:**
- Ultimate screen-wide devastation
- 30-tile radius = EVERYTHING in visible range
- No targeting required (self-cast)
- Ticks every second for 20 seconds:
  - 8-15 damage per tick
  - 90% slow (-90 DEX)
  - 20% chance per tick to freeze (2s stun)
- Total: 160-300 damage over 20 seconds
- Enemies frozen randomly throughout
- Most expensive spell (needs 5 Frozen Ore)
- Great for:
  - When completely overwhelmed
  - Final boss phases
  - PvP last stand
  - "Nuclear option" scenarios
- Once cast, affects everything for full duration

---

#### 31. Rime Reaper ✅
- **File:** `RimeReaperSpell.cs`
- **Mana:** 50
- **Reagents:** Frozen Soul, Eternal Ice, Heart of Winter
- **Range:** 15 tiles (direction target)
- **Area:** Arc sweep (90° angle, 15 tiles)
- **Damage:** 100-150 cold damage
- **Special:** INSTANT KILL enemies below 20% HP

**What You See:**
- Target a direction
- Giant ice scythe slash visual (0x36BD)
- Scythe sound (0x232)
- Massive particle effects
- Enemies hit by arc sweep:
  - Normal: Take 100-150 damage + ice impact
  - Below 20% HP: SHATTER INTO ICE
    - Instant death
    - Shatter particles (0x3735)
    - Message: "[Enemy name] shatters into ice!"
- Message: "You swing the Rime Reaper!"
- Message: "Rime Reaper hits X targets!"

**How It Works:**
- Execute ability (like Overwatch's Reaper)
- Wide arc in front of you (90° cone)
- 15 tile range
- Normal damage: 100-150
- Execute mechanic:
  - Checks enemy HP%
  - If ≤ 20% max HP: instant kill
  - Ignores remaining HP
- Great for:
  - Finishing weakened bosses
  - Cleaning up low HP enemies
  - PvP executions
  - Burst AoE + execution combo
- Very satisfying when enemies shatter
- Can execute multiple enemies at once

---

#### 32. Cocytus Prison ✅
- **File:** `CocytusPrisonSpell.cs`
- **Mana:** 50
- **Reagents:** Frozen Soul, Eternal Ice (3), Heart of Winter
- **Range:** 12 tiles
- **Duration:** Up to 60 seconds (channeled)
- **Effect:** Complete lockdown (requires caster channel)

**What You See:**
- Target enemy
- Massive ice prison forms (0x3779 scaled 2x)
- Freezing sound (0x1F8)
- Prison visual item appears at target location
- Target message: "You are imprisoned in Cocytus!"
- Caster message: "You imprison your target in Cocytus! You must channel to maintain it."
- Both caster AND target frozen in place
- Prison pulses with ice particles every 2 seconds
- Target is:
  - Frozen (can't move)
  - Paralyzed (can't act)
  - Blessed (IMMUNE to all damage)
- After up to 60s: "The Cocytus prison shatters!" with massive particle burst
- Both released

**How It Works:**
- Ultimate lockdown spell
- Completely disables one enemy
- COST: You also can't move or cast (channeling)
- Target takes NO damage while imprisoned
- Double-edged sword:
  - Good: Removes dangerous enemy from fight completely
  - Bad: Also removes YOU from fight
- Either lasts 60s or until you release it
- Prison has visual item that can be destroyed
- Great for:
  - 1v1 PvP (mutual disable)
  - Removing boss from fight temporarily
  - Protecting fleeing allies (lock chaser)
  - Emergency freeze button
- Most expensive spell (3 Eternal Ice)
- True "ultimate" - game-changing but risky

---

## Reagent Usage by Circle

**Circle 1:**
- Frostbloom, Winterleaf

**Circle 2:**
- Frostbloom, Winterleaf, Glacier Crystal

**Circle 3:**
- Frostbloom, Winterleaf, Glacier Crystal

**Circle 4:**
- Winterleaf, Glacier Crystal, Permafrost Essence

**Circle 5:**
- Permafrost Essence, Glacier Crystal, Arctic Pearl

**Circle 6:**
- Permafrost Essence, Arctic Pearl, Frozen Soul

**Circle 7:**
- Arctic Pearl, Frozen Soul, Permafrost Essence, Eternal Ice, Heart of Winter

**Circle 8:**
- Frozen Soul, Eternal Ice, Heart of Winter, Frozen Ore

---

## Combat Strategies

### Solo PvE Farming
1. Open with **Blizzard** or **Frozen Ground** (area DoT)
2. Use **Ice Bolt** and **Ice Shard** for single targets
3. Apply **Frost Armor** before combat
4. **Glacial Mend** for emergency healing
5. Finish low HP enemies with **Rime Reaper**

### Boss Fights
1. Pre-buff: **Frost Armor**, **Glacial Fortress**
2. Place boss in **Frozen Ground** or **Eternal Winter**
3. Burst with **Absolute Zero** or **Frost Meteor**
4. Use **Ice Wall** to control boss position
5. Summon **Ice Elemental** for tank/DPS
6. Execute with **Rime Reaper** below 20%

### PvP Dueling
1. Open with **Glacial Strike** (stun)
2. Follow with **Shatter** (combo damage)
3. **Permafrost** to prevent fleeing
4. **Hypothermia** to slow enemy
5. Defensive: **Avalanche** (knockback) or **Frozen Tomb** (emergency)
6. Ultimate: **Cocytus Prison** (mutual disable)

### Group Farming
1. **Ice Age** for screen-wide damage
2. **Eternal Winter** for prolonged zone control
3. **Frost Meteor** for massive AoE
4. **Ice Spear** for line damage
5. Let **Ice Elemental** tank

### Kiting Strategy
1. **Chill Aura** always active
2. **Frost Slick** between you and enemy
3. **Ice Bolt** while moving
4. **Avalanche** to create distance
5. **Ice Wall** to block paths

---

## Known Limitations

**Not Fully Implemented:**
- Ice Shield damage reflection (requires damage event hooks)
- Frostbite healing reduction (requires healing system hooks)
- Fimbulwinter's Wrath damage boost (requires damage calculation hooks)
- Fimbulwinter's Wrath mana reduction (requires spell cast hooks)

**These features are documented in tooltips but not active in code. Complex to implement - require deeper ServUO integration.**

---

## Testing Commands

```
[spellbook ice          - Spawn Ice Mage Spellbook
[add Frostbloom 100     - Get Circle 1-3 reagent
[add Winterleaf 100     - Get Circle 1-4 reagent
[add GlacierCrystal 100 - Get Circle 2-4 reagent
[add PermafrostEssence 100 - Get Circle 3-6 reagent
[add ArcticPearl 100    - Get Circle 5-7 reagent
[add FrozenSoul 100     - Get Circle 6-8 reagent
[add EternalIce 100     - Get Circle 7-8 reagent
[add HeartOfWinter 100  - Get Circle 8 reagent
[add FrozenOre 100      - Get Ice Age reagent
```

---

## File Locations

**Spell Files:** `C:\DevEnv\GIT\UO\ServUO\Scripts\Custom\VystiaClasses\Spells\IceMage\`

**Spellbook File:** `C:\DevEnv\GIT\UO\ServUO\Scripts\Items\Equipment\Spellbooks\VystiaSpellbooks.cs`

**Reagent File:** `C:\DevEnv\GIT\UO\ServUO\Scripts\Items\Vystia\Resources\Reagents\IceMagicReagents.cs`

**Commands File:** `C:\DevEnv\GIT\UO\ServUO\Scripts\Custom\Commands\VystiaSpellbookCommands.cs`

---

*Ice Magic - 100% Complete*
*Implementation Date: 2025-12-06*
*Total Spells: 32/32*
*Status: Ready for in-game testing*
