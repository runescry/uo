# AI Sidekicks System

Autonomous NPC companions that fight alongside players using intelligent combat AI.

## Status: OPERATIONAL (v2.4.0)

**Last Updated:** December 2, 2025

## Quick Start

```
[SpawnMage       - Spawn Mage sidekick
[st              - Spawn Tamer sidekick
[SpawnSidekick   - Spawn any archetype
[ol              - Spawn Arctic Ogre Lord (test target)
```

## Implemented Archetypes

| Archetype | Combat AI | Description |
|-----------|-----------|-------------|
| **Mage** | `MageCombatAI` | Kiting mage with spell combos, 40-tile leash, circular kiting at boundary |
| **Warrior** | `WarriorCombatAI` | Melee tank with self-healing |
| **Tamer** | `TamerCombatAI` | Pet commander, heals pets with bandages/spells |
| **Healer** | `HealerCombatAI` | Pure support, heals owner and allies |
| **Necromancer** | `NecromancerCombatAI` | Dark magic with necromancy spells |
| **Archer** | `ArcherCombatAI` | Ranged combat |
| **Paladin** | `PaladinCombatAI` | Holy warrior with chivalry |

## Documentation

All documentation is in the `docs/` folder:

| File | Description |
|------|-------------|
| `docs/STATUS.md` | Current feature status and changelog |
| `docs/ARCHETYPES.md` | Complete archetype specifications |
| `docs/MAGE_COMBAT.md` | Mage AI detailed documentation |
| `docs/COMPANION_SYSTEM.md` | How sidekicks work as companions |
| `docs/ARCHITECTURE.md` | Technical architecture |
| `docs/TESTING_GUIDE.md` | Testing procedures |

### Simulation System

The `Simulation/` folder contains a Python-based combat simulator for AI parameter optimization:
- `Simulation/README.md` - Simulation overview
- `Simulation/docs/` - Detailed simulation documentation

## Key Features

### Mage Combat AI (v2.4.0)
- **Leash System**: 40-tile boundary from combat start point
- **Circular Kiting**: Strafes perpendicular when at leash boundary
- **Chase Mode**: Aggressive 3-5 tile spell release for finishing kills
- **Spell Combos**: Curse -> Explosion -> Poison -> Energy Bolt
- **Predictive Positioning**: Accounts for enemy movement during cast time

### Support Classes
- **Tamer**: Commands pets, heals with bandages and Greater Heal
- **Healer**: Pure support, bandage loop, heal potions at 75% HP

### Core System
- Full pet command compatibility (follow, kill, stay, guard)
- Persistence across server restarts
- Death/resurrection support (IsBonded)
- Starting equipment (spellbooks, bandages, potions, armor)

## Directory Structure

```
AISidekicks/
├── README.md              # This file
├── AI/                    # Combat AI implementations
│   ├── SidekickAI.cs      # Base AI controller
│   ├── MageCombatAI.cs    # Mage combat logic
│   ├── TamerCombatAI.cs   # Tamer support logic
│   └── ...                # Other archetype AIs
├── Archetypes/            # Sidekick class definitions
├── docs/                  # Documentation
│   ├── STATUS.md          # Current status
│   ├── ARCHETYPES.md      # Archetype specs
│   └── archive/           # Historical docs
├── Simulation/            # Python combat simulator
│   ├── README.md
│   ├── full_combat_simulator.py
│   └── docs/
└── Data/                  # Data files
```

## Related Systems

- **LLM Integration**: `Scripts/Services/LLM/` - Natural conversation
- **Base AI**: `Scripts/Mobiles/AI/` - ServUO AI system
