# Vystia Ability System

Data-driven, automation-ready ability framework for 832+ abilities across 26 classes.

## Quick Start

### Register an Ability

```csharp
// Create and register an ability
var iceBlast = new AbilityDefinition()
    .WithId(10005)
    .WithName("Ice Blast")
    .InSchool(AbilitySchool.Ice)
    .InCircle(2)
    .WithManaCost(8)
    .Targeting(AbilityTargetType.SingleTarget, 12)
    .WithDamage(18, 28, VystiaDamageType.Cold)
    .WithStack(StackType.Chill, 2, 12)
    .WithCooldown(5);

AbilityRegistry.RegisterAbility(iceBlast);
```

### Use Factory Methods

```csharp
// Simple damage spell
var fireBolt = AbilityDefinition.CreateDamageSpell(
    id: 10100,
    name: "Fire Bolt",
    school: AbilitySchool.Elemental,
    circle: 1,
    minDmg: 15,
    maxDmg: 25,
    dmgType: VystiaDamageType.Fire,
    manaCost: 5);

AbilityRegistry.RegisterAbility(fireBolt);

// Healer spell
var fastHeal = AbilityDefinition.CreateHealSpell(
    id: 40001,
    name: "Fast Heal",
    school: AbilitySchool.Cleric,
    circle: 2,
    minHeal: 30,
    maxHeal: 50,
    manaCost: 15);

AbilityRegistry.RegisterAbility(fastHeal);

// Melee strike
var backstab = AbilityDefinition.CreateMartialStrike(
    id: 20100,
    name: "Backstab",
    school: AbilitySchool.Rogue,
    minDmg: 40,
    maxDmg: 65,
    staminaCost: 25);

AbilityRegistry.RegisterAbility(backstab);
```

### Execute an Ability

```csharp
// Get the ability definition
AbilityDefinition ability = AbilityRegistry.GetAbility(10001);

// Execute on a target
AbilityExecutionResult result = AbilityExecutor.Execute(caster, ability, target);

if (result.Success)
{
    caster.SendMessage($"Dealt {result.TotalDamage} damage (crit: {result.WasCrit})");
}
else
{
    caster.SendMessage($"Failed: {result.FailureReason}");
}
```

## Ability Structure

### Identity
- **Id:** Unique integer (e.g., 10001)
- **Name:** Display name (e.g., "Ice Bolt")
- **Description:** Flavor text
- **School:** Magic school or martial discipline
- **Circle:** Spell circle (1-8) or ability tier

### Costs
- **ManaCost:** Mana required
- **StaminaCost:** Stamina required
- **HealthCost:** Life tap cost
- **SecondaryCosts:** Resource costs (Soul Shards, Chi, etc.)
- **ReagentCost:** Generic reagent count

### Targeting
- **TargetType:** Self, SingleTarget, SingleFriendly, AoE, Cone, Line, ChainTarget, GroundTarget, Passive
- **Range:** Distance in tiles
- **AoERadius:** Radius for area abilities
- **MaxTargets:** Max targets affected
- **RequiresLineOfSight:** Must see target

### Timing
- **CastTime:** Channel duration
- **Cooldown:** Seconds before reuse
- **SharedCooldownGroup:** Group with other abilities
- **GlobalCooldownOverride:** Custom GCD

### Requirements
- **RequiredStance:** Stance requirement
- **MinResourceStacks:** Minimum stack count
- **RequiresStealth:** Must be hidden
- **RequiresBehindTarget:** Backstab requirement
- **RequiredWeapon:** Sword, Axe, Staff, etc.
- **ComboFromAbilities:** Enables combo chain

### Effects
- **Effects[]:** List of AbilityEffect

### Resource Generation
- **ResourceGeneration:** Dictionary of resource gains (Fury, Faith, etc.)

### Modifiers
- **CanCrit:** Can critically hit
- **CritChanceBonus:** Additional crit chance
- **CritMultiplierBonus:** Crit damage modifier
- **IgnoresArmor:** Bypasses physical resist
- **IgnoresResistance:** Bypasses all resists
- **ArmorPenetration:** Reduces target armor

### Flags
- **IsChanneled:** Requires channel time
- **IsInstant:** No cast time
- **IsPassive:** Always active
- **BreaksStealth:** Reveals hidden caster
- **CanBeInterrupted:** Can be disrupted
- **ConsumesStacks:** Uses up stacks
- **IsFinisher:** Consumes all stacks for bonus

## Effect Types

### Damage
- **DirectDamage:** Single hit
- **DamageOverTime:** Damage per tick
- **PercentDamage:** % of target's max HP

### Healing
- **DirectHeal:** Single heal
- **HealOverTime:** Heal per tick
- **PercentHeal:** % of target's missing HP
- **Absorb:** Damage shield

### Buff/Debuff
- **ApplyBuff:** Positive effect
- **ApplyDebuff:** Negative effect
- **RemoveBuff:** Dispel positives
- **RemoveDebuff:** Cleanse negatives
- **DispelMagic:** Remove all magic

### Resource
- **RestoreMana:** Restore mana
- **RestoreStamina:** Restore stamina
- **DrainMana:** Steal mana
- **DrainStamina:** Steal stamina
- **GenerateResource:** Create secondary resource
- **ConsumeResource:** Spend secondary resource

### Stack/Mark
- **ApplyStack:** Add stacks (Chill, Combo, etc.)
- **ConsumeStack:** Remove stacks (finisher)
- **CheckStack:** Conditional based on stacks

### Summon
- **Summon:** Spawn creature
- **Dismiss:** Remove summon
- **PetCommand:** Command pet

### Control
- **ApplyCC:** Crowd control (Stun, Root, etc.)
- **RemoveCC:** Dispel CC

### Movement
- **Teleport:** Move caster/target
- **Knockback:** Push away
- **Pull:** Pull toward
- **Leap:** Jump distance

### Utility
- **BreakStealth:** Reveal hidden
- **EnterStealth:** Hide
- **Interrupt:** Cancel spellcast
- **Taunt:** Force target to attack
- **ThreatModify:** Increase/decrease aggro

### Special
- **ConditionalEffect:** If X then apply effect
- **RepeatEffect:** Execute multiple times
- **DelayedEffect:** Trigger after delay
- **TriggerAbility:** Cast another ability

## Conditions

Conditional effects check conditions and apply bonuses:

```csharp
effect.Condition = "TargetFrozen";
effect.ConditionalMultiplier = 2.0;  // 200% damage if frozen
```

**Built-in Conditions:**
- `TargetFrozen` - Target has Freeze CC
- `TargetBelow20Percent` - Target < 20% HP
- `TargetBelow35Percent` - Target < 35% HP
- `TargetFullHealth` - Target at max HP
- `TargetStunned` - Target has Stun CC
- `TargetBleeding` - Target has Bleed buff
- `CasterFullResource` - Caster max secondary resource
- `CasterHidden` - Caster is stealthed

## On-Hit Effects

Triggered when damage connects:

- `ApplyChill` - Add chill stack
- `ApplyBleed` - Apply bleed DoT
- `ApplyBurn` - Apply burn DoT
- `ApplyCorruption` - Apply corruption DoT
- `ApplyPoison` - Apply poison DoT
- `ApplySlow` - Slow target
- `ApplyWeaken` - Reduce STR
- `LifeSteal` - Heal caster (% of damage)
- `ManaSteal` - Drain mana (% of damage)
- `StaminaDrain` - Drain stamina
- `Knockback` - Push target away
- `Stun` - Stun target

## Resource Generation

Abilities can generate secondary resources:

```csharp
ability.GeneratesResource(ResourceType.Fury, 10);
ability.GeneratesResource(ResourceType.SoulShards, 1);
```

Resources are also auto-generated during damage:
- **Soul Shards** (Warlock): 25% chance on crit
- **Fury** (Barbarian): 1 per 10 damage
- **Chi** (Monk): 30% chance on hit
- **Combo Points** (Rogue): 1 per hit (per-target)
- **Pursuit** (Bounty Hunter): On marked targets

## GM Commands

```
[TestAbility <id>            - Execute ability on target
[ListAbilities [school]      - List registered abilities
```

## Example: Complete Ability

```csharp
// Rogue Eviscerate - Consumes all combo points for bonus damage
var eviscerate = new AbilityDefinition()
    .WithId(20002)
    .WithName("Eviscerate")
    .InSchool(AbilitySchool.Rogue)
    .WithStaminaCost(25)
    .Targeting(AbilityTargetType.SingleTarget, 2)
    .AsInstant()
    .WithCooldown(5)
    .AsFinisher();

// Base damage
eviscerate.Effects.Add(new AbilityEffect
{
    Type = AbilityEffectType.DirectDamage,
    MinValue = 30,
    MaxValue = 50,
    DamageType = VystiaDamageType.Physical
});

// Consumes combo points
eviscerate.Effects.Add(new AbilityEffect
{
    Type = AbilityEffectType.ConsumeStack,
    StackType = StackType.Combo
});

AbilityRegistry.RegisterAbility(eviscerate);
```

## Python Automation

Abilities are designed to be generated by Python scripts:

```python
# generate_abilities.py
def parse_ability(row):
    """Parse ability from CSV/markdown table"""
    return AbilityDefinition(
        id=row['id'],
        name=row['name'],
        school=row['school'],
        circle=row['circle'],
        mana_cost=row['mana'],
        effects=[...]
    )

def generate_all_abilities():
    """Generate C# code for all abilities"""
    for school in SCHOOLS:
        for ability_data in parse_school(school):
            csharp_code = generate_csharp(ability_data)
            write_file(f"Abilities_{school}.cs", csharp_code)
```

## See Also

- `AbilityDefinition.cs` - Data structure and factories
- `AbilityExecutor.cs` - Execution engine and registry
- `SYSTEMS_V2.md` - Complete system documentation
- `../design/vystia_complete_class_design.md` - 832 ability designs

