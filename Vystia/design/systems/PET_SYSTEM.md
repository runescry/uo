# Vystia Pet System Design

## Overview

The Vystia Pet System provides class-specific companions for Beastmaster, Summoner, Necromancer, and Artificer classes. Each class has unique pet types with different mechanics.

## Implementation Status: COMPLETE

**Location:** `ServUO/Scripts/Custom/VystiaClasses/Pets/`
**Files:** 5 (2,984 lines total)

---

## Pet Types by Class

### Beastmaster
- **Type:** TamedBeast
- **Mechanic:** Permanent, bondable companions
- **Scaling Skill:** Animal Taming

### Summoner
- **Types:** SummonedElemental, SummonedCreature
- **Mechanic:** Timed summons (5-20 minutes based on tier)
- **Scaling Skill:** Magery

### Necromancer
- **Types:** UndeadMinion, UndeadServant
- **Mechanic:** Timed undead (5-20 minutes based on tier)
- **Scaling Skill:** Necromancy

### Artificer
- **Types:** MechanicalConstruct, ClockworkServant
- **Mechanic:** Timed constructs (5-20 minutes based on tier)
- **Scaling Skill:** Tinkering

---

## Pet Tiers

| Tier | Control Slots | Base HP | Base Damage | Duration |
|------|---------------|---------|-------------|----------|
| Lesser | 1 | 50 | 5-10 | 5 min |
| Standard | 2 | 100 | 10-18 | 8 min |
| Greater | 3 | 175 | 15-25 | 12 min |
| Superior | 4 | 275 | 20-32 | 15 min |
| Legendary | 5 | 400 | 28-42 | 20 min |

---

## Skill Scaling

Stats scale from 0.8x (0 skill) to 1.5x (120 skill):
```
scalar = 0.8 + (skillValue / 200.0)
```

---

## Available Summons

### Fire Elemental
- Tier: Standard+
- Type: SummonedElemental
- Abilities: Fire damage, burning aura

### Ice Elemental
- Tier: Standard+
- Type: SummonedElemental
- Abilities: Cold damage, slow effect

### Skeleton
- Tier: Lesser+
- Type: UndeadMinion
- Abilities: Basic melee

### Zombie
- Tier: Lesser+
- Type: UndeadMinion
- Abilities: Basic melee, higher HP

### Clockwork
- Tier: Standard+
- Type: MechanicalConstruct
- Abilities: Mechanical attacks

---

## GM Commands

| Command | Access | Description |
|---------|--------|-------------|
| `[SummonPet <type> [tier]` | GM | Summon test pet |
| `[DismissPets [type]` | GM | Dismiss pets |
| `[PetInfo` | GM | Show pet status |

**Pet Types for [SummonPet:**
- `elemental` / `fire` - Fire Elemental
- `ice` / `water` - Ice Elemental
- `undead` / `skeleton` - Skeleton
- `zombie` - Zombie
- `construct` / `clockwork` - Clockwork

---

## Technical Notes

- All summoned pets are bard-immune
- Summoned pets cannot be tamed
- Pets don't drop loot
- Full serialization for save/load

---

*Last Updated: 2026-01-03*
