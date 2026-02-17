# Vystia Class System v2.0 - Core Systems Documentation

**Status:** ✅ **Sprints 1-4 Complete**
**Build Status:** 0 errors, 0 warnings
**Last Updated:** 2025-12-10

---

## Overview

The Vystia Class System v2.0 is a data-driven, automation-ready framework for implementing 832 unique abilities across 26 character classes (12 magic, 14 martial).

**Design Principle:** Systems First, Automation Second, Content Last
- Build foundational systems that enable mass-generation of abilities via Python
- All abilities are data structures that can be instantiated and registered at runtime
- GM commands provide immediate testing and iteration

---

## Implemented Core Systems (8 files, ~4,500 LOC)

### 1. Secondary Resource Framework ✅
**File:** `Systems/SecondaryResource.cs` (~1,185 lines)

**15 Resource Types Implemented:**

| Resource | Class | Behavior | Max |
|----------|-------|----------|-----|
| **SoulShards** | Warlock | Generate on kill/crit | 3 |
| **LifeForce** | Necromancer | Generate from death nearby | 100 |
| **ChillStacks** | Ice Mage | Per-target, 5 = Frozen | 5 |
| **Crescendo** | Bard | Tick while channeling | 100 |
| **Fury** | Barbarian | Decay out of combat | 100 |
| **Chi** | Monk | No decay, meditative | 5 |
| **ComboPoints** | Rogue | Per-target, finisher reset | 5 |
| **Focus** | Ranger | Regen stationary, decay moving | 100 |
| **Zeal** | Templar | Decay out of combat | 10 |
| **Fortitude** | Knight | Generate on block | 10 |
| **Pursuit** | Bounty Hunter | Marked target only | 10 |
| **Steam** | Artificer | Regen near machinery | 100 |
| **Charges** | Artificer | Crafted only, no regen | 10 |
| **Faith** | Cleric | Generate on heals | 100 |
| **Virtues** | Paladin | 4 virtue stacks (Valor, Compassion, Justice, Sacrifice) | 10 |

**Architecture:**
```
ISecondaryResource (interface)
├── SecondaryResourceBase (abstract)
│   ├── Standard resources (Fury, Chi, Zeal, etc.)
│   └── PerTargetResourceBase (ComboPoints, ChillStacks, Pursuit)
└── SecondaryResourceFactory (creates instances by type)
```

**Features:**
- Automatic decay/regen per tick
- Combat state tracking (in-combat, last-combat-time)
- Per-target stack tracking for ComboPoints, ChillStacks, Pursuit
- Full serialization/deserialization
- Factory pattern for instantiation

---

### 2. Resource Manager ✅
**File:** `Systems/VystiaResourceManager.cs` (~350 lines)

**Attachment:** PlayerMobile extension
**Scope:** Player → All Resources + Manager

**Features:**
- Singleton manager per PlayerMobile
- All 15 resources per player
- Tick-based updates (decay/regen)
- Combat hook system (OnDamageDealt, OnDamageT, OnBlock, OnKill, OnHeal)
- GM Commands: `[SetResource`, `[GetResources`, etc.

**Combat Hooks:**
```csharp
manager.OnDamageDealt(damage);  // Generate Fury, etc.
manager.OnDamageT(damage);      // Generate Fury
manager.OnBlock();              // Generate Fortitude
manager.OnKill(victim);         // Generate SoulShards
manager.OnHeal(amount);         // Generate Faith
```

---

### 3. Target Tracker ✅
**File:** `Systems/TargetTracker.cs` (~700 lines)

**Purpose:** Track per-target state from attacker's perspective

**Per-Target Tracked:**
- **Stacks:** Chill, Curse, Combo, Pursuit, Bleed, Poison, Burn, Corruption, Weaken, Vulnerability, Expose, Mark
- **Marks:** BountyMark, HuntersMark, DeathMark, SoulMark, Hex, Divine
- **Combat State:** LastHitTime, TotalDamageDealt, IsInCombat
- **DoT State:** Damage per tick, tick interval

**Architecture:**
```
TargetState
├── Stacks[12] with stack thresholds (Chill 5 = Frozen)
├── Marks (one per target)
├── Combat tracking
└── DoT processing

VystiaTargetTracker (singleton)
├── GetState(attacker, target) → TargetState
├── AddStacks(attacker, target, type, count, duration)
├── ConsumeStacks(attacker, target, type)
├── ApplyMark(attacker, target, type, duration, bonusDamage)
└── ProcessDoTs() - automatic tick processing
```

**Stack Thresholds:**
- **Chill 5** → Frozen (root, can't move)
- **Combo 5** → Max stacks (finisher ready)
- **Pursuit 10** → Max stacks

**Automatic Events:**
- Chill 5 → Triggers frozen (CC effect)
- Combo 5 → Notifies attacker

---

### 4. Buff/Debuff System ✅
**File:** `Systems/VystiaBuffSystem.cs` (~850 lines)

**Purpose:** Stackable buffs, DoTs, HoTs, transforms

**Buff Categories:**

| Category | Examples | Behavior |
|----------|----------|----------|
| **Stat Buffs** | StrengthBuff, DexBuff, IntBuff, AllStatsBuff | +X stat modifier |
| **Stat Debuffs** | StrengthDebuff, etc. | -X stat modifier |
| **Damage Types** | DamageIncrease, Vulnerability | % modifier |
| **Resistance** | PhysicalResist, FireResist, AllResist | Resist modifier |
| **Speed** | HasteBuff, SlowDebuff, CastSpeedBuff | Speed multiplier |
| **Regen** | HitPointRegen, ManaRegen, StaminaRegen | Per-tick value |
| **DoT** | Bleed, Burn, Poison, Corruption, SoulDrain | Damage per tick |
| **HoT** | Rejuvenation, LifeBloom, Tranquility | Heal per tick |
| **Shield** | DamageAbsorb, ManaShield, ReflectShield | Absorption/reflection |
| **Transform** | BearForm, CatForm, WolfForm, TreeForm, Rage | Body change + stat bonus |
| **Control** | Stealth, Invisible, Silenced, Pacified | Special effect |
| **Utility** | WaterBreathing, NightVision, LifeTap | Special mechanic |
| **Class-Specific** | SongOfCourage (Bard), Blessing (Paladin), ShadowCloak (Warlock) | Unique per class |

**Stack Behaviors:**
- **Refresh:** Resets duration, same strength
- **Stack:** Adds stacks, each has effect
- **Replace:** Stronger replaces weaker
- **Extend:** Adds to duration
- **Ignore:** Does nothing if present

**Buff Instance:**
```
VystiaBuffInstance
├── Definition (VystiaBuffDefinition)
├── Target, Caster
├── Stacks, Duration
├── StatMods (applied/removed on apply/remove)
├── ResistMods (applied/removed on apply/remove)
├── DoT/HoT (damage/heal per tick)
├── Shield (absorption remaining)
├── Reflect (reflection %)
├── Transform (body/hue)
└── OnTick() - DoT/HoT processing
```

**GM Commands:**
```
[ApplyBuff <type> [duration] [value]
[RemoveBuff <type>
[ListBuffs
[ClearBuffs [all|beneficial|harmful]
```

**Registered Buffs:** 25+ default definitions (extensible)

---

### 5. Damage System ✅
**File:** `Systems/VystiaDamageSystem.cs` (~650 lines)

**Purpose:** Standardized damage calculation pipeline

**Damage Types:** Physical, Fire, Cold, Poison, Energy, Shadow, Holy, Arcane, Nature, Bleed

**Damage Pipeline:**
1. Roll base damage (min-max)
2. Roll crit (crit chance, crit multiplier)
3. Apply damage multiplier (buffs, stances, abilities)
4. Apply flat bonus
5. Apply source buffs (from VystiaBuffSystem)
6. Calculate effective resists (with penetration)
7. Apply resistances (armor, resists)
8. Apply target debuffs (shields, absorbs, vulnerability)
9. Ensure minimum 1 damage
10. Process on-hit effects
11. Generate resources
12. Record hit in TargetTracker

**DamageContext:**
```
DamageContext
├── Source, Target
├── MinDamage, MaxDamage
├── Damage type distribution (phys%, fire%, cold%, pois%, energy%)
├── CanCrit, CritChance, CritMultiplier
├── DamageMultiplier, FlatDamageBonus
├── ArmorPenetration, ResistancePenetration
├── Flags: CanBeBlocked, CanBeDodged, IgnoresArmor, IsSpell, IsAbility, IsAutoAttack
├── OnHitEffects[] (ApplyChill, ApplyBleed, LifeSteal, Knockback, etc.)
└── Results: RawDamage, FinalDamage, WasCrit, etc.
```

**On-Hit Effects:**
- ApplyChill, ApplyBleed, ApplyBurn, ApplyCorruption, ApplyPoison, ApplySlow, ApplyWeaken
- LifeSteal, ManaSteal, StaminaDrain, Knockback, Stun

**Resource Generation on Damage:**
- **Soul Shards** (Warlock): 25% chance on crit
- **Fury** (Barbarian): 1 per 10 damage
- **Chi** (Monk): 30% chance on auto-attack
- **Combo Points** (Rogue): Per-target via TargetTracker
- **Pursuit** (Bounty Hunter): If target marked

**GM Commands:**
```
[TestDamage <min> <max> [type]     - Test damage calculation
[TestHeal <min> <max>              - Test healing calculation
[TestCrit <critChance>             - Test with X% crit
```

---

### 6. Crowd Control System ✅
**File:** `Systems/CrowdControlSystem.cs` (~700 lines)

**Purpose:** CC types with diminishing returns

**CC Types:** Stun, Freeze, Root, Silence, Fear, Sleep, Charm, Knockback, Knockdown, Slow, Blind, Disarm, Pacify, Confuse, Polymorph

**DR Categories:**
| Category | Types | Reset | Immunity |
|----------|-------|-------|----------|
| **Stun** | Stun, Knockdown | 15s | 15s at DR3 |
| **Incapacitate** | Fear, Sleep, Charm, Confuse | 15s | 15s at DR3 |
| **Root** | Root, Freeze | 15s | 15s at DR3 |
| **Silence** | Silence, Pacify, Disarm | 15s | 15s at DR3 |
| **None** | Slow, Blind, Knockback, Polymorph | N/A | No DR |

**Diminishing Returns:**
- **1st CC:** 100% duration
- **2nd CC (within 15s):** 50% duration
- **3rd CC (within 15s):** 25% duration
- **4th CC+:** Immunity for 15s (immune reset after 15s of no CCs)

**CCEntry:**
```
CCEntry
├── Type, Target, Source
├── AppliedAt, ExpiresAt
├── OriginalDuration
├── DRLevel (0-3)
└── RemainingDuration
```

**DRHistory (per category):**
```
DRHistory
├── Category
├── Level (current DR level)
├── LastApplied (time of last CC)
├── IsImmune → bool
└── IncrementDR() → updates level and immunity
```

**Special CC Mechanics:**
- **Sleep:** Breaks on damage
- **Charm:** Breaks on damage > 10
- **Fear:** Creatures flee via `BeginFlee()`
- **Freeze:** Can't move, can cast/attack
- **Root:** Can't move, can cast/attack
- **Knockback:** Instant, applies push

**GM Commands:**
```
[ApplyCC <type> [duration]  - Apply CC with DR
[RemoveCC <type>            - Remove specific CC
[ListCC                     - List active CCs
[CheckDR                    - Show DR levels
[ResetDR                    - Reset DR levels
```

---

### 7. Ability Definition ✅
**File:** `Abilities/AbilityDefinition.cs` (~550 lines)

**Purpose:** Data-driven ability structure for automation

**Ability Properties:**

```
AbilityDefinition
│
├── Identity
│   ├── Id (int)
│   ├── Name, Description
│   ├── School (Magic/Martial)
│   └── Circle (1-8)
│
├── Costs
│   ├── ManaCost, StaminaCost, HealthCost
│   ├── SecondaryCosts (Dictionary<ResourceType, int>)
│   └── ReagentCost
│
├── Targeting
│   ├── TargetType (Self, SingleTarget, AoE, Cone, Line, ChainTarget, etc.)
│   ├── Range, AoERadius, MaxTargets
│   └── RequiresLineOfSight
│
├── Timing
│   ├── CastTime, Cooldown
│   ├── SharedCooldownGroup
│   └── GlobalCooldownOverride
│
├── Requirements
│   ├── RequiredStance, MinResourceStacks
│   ├── RequiresStealth, RequiresBehindTarget
│   ├── RequiredWeapon, MinLevel, MinSkill
│   └── ComboFromAbilities[]
│
├── Effects
│   └── Effects[] (AbilityEffect)
│
├── Resource Generation
│   └── ResourceGeneration (Dictionary<ResourceType, int>)
│
├── Modifiers
│   ├── CanCrit, CritChanceBonus, CritMultiplierBonus
│   ├── IgnoresArmor, IgnoresResistance
│   └── ArmorPenetration
│
├── Visual/Sound
│   ├── CastAnimation, CastSound
│   ├── ImpactEffectId, ImpactSound
│   └── ProjectileId, ProjectileHue
│
└── Flags
    ├── IsChanneled, IsInstant, IsPassive
    ├── BreaksStealth, CanBeInterrupted
    └── ConsumesStacks, IsFinisher
```

**AbilityEffect:**
```
AbilityEffect
├── Type (DirectDamage, DamageOverTime, DirectHeal, ApplyBuff, ApplyCC, ApplyStack, etc.)
├── Values (MinValue, MaxValue, Duration, Stacks)
├── Damage/Buff Type
├── Condition (if X then Y, "TargetFrozen", "TargetBelow20Percent")
├── ConditionalMultiplier
├── ChainTargets, ChainFalloff
├── Delay, TriggeredAbilityId
└── Visual (EffectId, EffectHue, SoundId)
```

**Builder Pattern:**
```csharp
var ability = new AbilityDefinition()
    .WithId(10001)
    .WithName("Ice Bolt")
    .InSchool(AbilitySchool.Ice)
    .InCircle(1)
    .WithManaCost(4)
    .Targeting(AbilityTargetType.SingleTarget)
    .WithDamage(12, 18, VystiaDamageType.Cold)
    .WithStack(StackType.Chill, 1, 10)
    .WithCooldown(0)
    .WithImpactEffect(0x36D4, 0x1E5, 1153);
```

**Static Factories:**
```csharp
// Simple damage spell
AbilityDefinition.CreateDamageSpell(id, name, school, circle, minDmg, maxDmg, type, mana)

// Simple heal
AbilityDefinition.CreateHealSpell(id, name, school, circle, minHeal, maxHeal, mana)

// DoT
AbilityDefinition.CreateDoTSpell(id, name, school, circle, dpsPerTick, duration, type, mana)

// AoE
AbilityDefinition.CreateAoESpell(id, name, school, circle, minDmg, maxDmg, type, radius, mana)

// Buff
AbilityDefinition.CreateBuffSpell(id, name, school, circle, buffType, value, duration, mana)

// Martial strike
AbilityDefinition.CreateMartialStrike(id, name, school, minDmg, maxDmg, stamina)

// Finisher
AbilityDefinition.CreateFinisher(id, name, school, baseDmg, dmgPerStack, stamina)
```

---

### 8. Ability Executor ✅
**File:** `Abilities/AbilityExecutor.cs` (~900 lines)

**Purpose:** Execute abilities from data-driven definitions

**Execution Pipeline:**
1. **Validate** requirements (mana, stamina, resources, weapon, stealth, etc.)
2. **Pay** costs (mana, stamina, resources)
3. **Resolve** targets (self, single, AoE, cone, line, chain, etc.)
4. **Play** cast effects (animation, sound)
5. **Apply** effects to each target (damage, heal, buff, CC, stacks, etc.)
6. **Generate** resources
7. **Trigger** cooldown
8. **Break** stealth if needed

**AbilityExecutionResult:**
```
AbilityExecutionResult
├── Success (bool)
├── FailureReason (string)
├── TotalDamage, TotalHealing
├── TargetsHit, WasCrit
└── ResourcesGenerated/Spent (Dictionary)
```

**Target Resolution:**
- **Self:** Just caster
- **SingleTarget/Friendly:** One target
- **PointBlankAoE:** Radius around caster
- **TargetAoE:** Radius around target/ground location
- **Cone:** Cone in front of caster (direction + angle)
- **Line:** Line from caster to target
- **ChainTarget:** Bounces with falloff

**Effect Application:**
- Checks conditions (TargetFrozen, TargetBelow20%, etc.)
- Applies conditional multiplier if condition met
- Calculates damage/heal via VystiaDamageCalculator/VystiaHealingCalculator
- Applies buff/debuff via VystiaBuffManager
- Applies CC via CrowdControlManager
- Applies stacks via VystiaTargetTracker
- Handles special effects (knockback, pull, teleport, stealth, etc.)

**Cooldown Management:**
- Individual ability cooldown
- Shared cooldown groups
- Query: IsOnCooldown(), GetRemainingCooldown()

**AbilityRegistry:**
```
AbilityRegistry (singleton)
├── RegisterAbility(definition)
├── GetAbility(id) → AbilityDefinition
├── GetAbilitiesBySchool(school) → List
└── AllAbilities → IEnumerable
```

**Registered Test Abilities:**
```
10001 - Ice Bolt (damage + chill)
10002 - Frostfire Bolt (damage + DoT)
10003 - Blizzard (AoE + slow)
10004 - Frost Armor (buff)
20001 - Sinister Strike (combo builder)
20002 - Eviscerate (finisher)
20003 - Heroic Strike (basic strike)
20004 - Execute (conditional bonus)
```

**GM Commands:**
```
[TestAbility <id>           - Execute ability on target
[ListAbilities [school]     - List registered abilities
```

---

## Integration Summary

### Data Flow
```
AbilityDefinition (data)
    ↓
AbilityExecutor.Execute()
    ├→ Validate (cost, requirement checks)
    ├→ PayCosts (mana, stamina, resources)
    ├→ ResolveTargets (single, AoE, cone, etc.)
    ├→ ApplyEffects (for each target)
    │   ├→ DamageContext → VystiaDamageCalculator
    │   │   └→ VystiaBuffManager (source/target buffs)
    │   │   └→ VystiaTargetTracker (record hit)
    │   │   └→ VystiaDamageSystem (on-hit effects)
    │   ├→ HealContext → VystiaHealingCalculator
    │   ├→ VystiaBuffManager.ApplyBuff()
    │   ├→ CrowdControlManager.ApplyCC()
    │   └→ VystiaTargetTracker.AddStacks()
    ├→ GenerateResources (via VystiaResourceManager)
    └→ TriggerCooldown (ability + shared groups)
```

### System Interactions
```
VystiaResourceManager
├→ SecondaryResource (15 types)
├→ VystiaTargetTracker (per-target stacks)
├→ VystiaDamageSystem (OnKill, OnCrit, OnBlock hooks)
└→ Combat tracking (last-hit, is-in-combat)

VystiaTargetTracker
├→ StackType (12 types)
├→ MarkType (6 types)
├→ Automatic DoT processing
└→ Per-target state persistence

VystiaBuffSystem
├→ 25+ buff types registered
├→ Stackable with stack behaviors
├→ DoT/HoT per-tick processing
├→ Transform (body change + stats)
└→ Shield (absorption + reflection)

VystiaDamageSystem
├→ Full damage pipeline (9 steps)
├→ Resource generation on damage
├→ On-hit effects (12 types)
└→ Integrated with buff/CC/stack systems

CrowdControlManager
├→ 15 CC types
├→ Diminishing returns (4 categories)
├→ Automatic DR tracking
└→ Immunity mechanics

AbilityExecutor
├→ 100+ validation checks
├→ Target resolution (8 types)
├→ Effect application (20+ effect types)
└→ Cooldown management (individual + shared)
```

---

## Next Steps (Sprints 5-11)

### Sprint 5: Stance System
- StanceSystem.cs - Stance definitions + switching
- StanceComboSystem - Combo triggers between stances
- Druid forms, Sorcerer elements, Barbarian Rage

### Sprint 6: Class Framework
- PlayerClassV2.cs - Enhanced class base
- Core 12 class implementations
- Class selection gump
- Passive hook system

### Sprint 7: GM Ability Editor
- AbilityEditorGump - In-game ability creator
- AbilityTestGump - Testing with diagnostics
- Training dummy with HP display
- Combat log viewer

### Sprint 8: Python Automation
- generate_ability_data.py - Parse design doc → structured data
- generate_spell_classes.py - Generate C# spell classes
- Generate Core 12 classes (384 abilities)
- Validation and compilation

### Sprints 9-11: Content Generation
- Generate remaining 14 classes (448 abilities)
- Item ecosystem update
- Balance testing and adjustments
- Documentation and polish

---

## File Locations

```
ServUO/Scripts/Custom/VystiaClasses/
├── Systems/
│   ├── SecondaryResource.cs       ✅ Complete
│   ├── VystiaResourceManager.cs   ✅ Complete
│   ├── TargetTracker.cs           ✅ Complete
│   ├── VystiaBuffSystem.cs        ✅ Complete
│   ├── VystiaDamageSystem.cs      ✅ Complete
│   ├── CrowdControlSystem.cs      ✅ Complete
│   ├── StanceSystem.cs            ⏳ Pending
│   ├── SetBonusSystem.cs          ⏳ Pending
│   └── [other systems]            ⏳ Pending
├── Abilities/
│   ├── AbilityDefinition.cs       ✅ Complete
│   ├── AbilityExecutor.cs         ✅ Complete
│   ├── AbilityEffect.cs           (included in Definition)
│   └── AbilityRegistry.cs         (included in Executor)
└── [other components]             ⏳ Pending
```

---

## GM Testing Commands

### Resource Management
```
[SetResource <type> <amount>
[GetResources
[ResetResources
[TestResource <type>
```

### Target Tracking
```
[GetTargetStacks
[SetTargetStacks <type> <amount>
[ClearTargetStacks
[TestMark <type>
```

### Buff System
```
[ApplyBuff <type> [duration] [value]
[RemoveBuff <type>
[ListBuffs
[ClearBuffs [all|beneficial|harmful]
```

### Damage/Healing
```
[TestDamage <min> <max> [type]
[TestHeal <min> <max>
[TestCrit <critChance>
```

### Crowd Control
```
[ApplyCC <type> [duration]
[RemoveCC <type>
[ListCC
[CheckDR
[ResetDR
```

### Abilities
```
[TestAbility <id>
[ListAbilities [school]
```

---

## Build & Compilation

**Status:** ✅ 0 errors, 0 warnings

```bash
cd D:\UO\ServUO
dotnet build
```

**All core systems compile successfully and are ready for class implementation.**

