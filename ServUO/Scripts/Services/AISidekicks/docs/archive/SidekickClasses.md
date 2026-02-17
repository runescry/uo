# Sidekick Classes Reference

All sidekick archetypes follow **T2A (The Second Age) rules**:
- **Stat Cap**: 225 total (max 100 per stat)
- **Skill Cap**: 700 total (max 100.0 per skill)

## Stat Effects (BaseCreature)

| Stat | Effect |
|------|--------|
| **STR** | HP (1:1 ratio) |
| **DEX** | Stamina (1:1 ratio), affects swing speed |
| **INT** | Mana (1:1 ratio) |

---

## Melee Archetypes

### Warrior (Lumberjack Build)
*The classic T2A damage dealer - axes + Lumberjacking for maximum damage.*

| Stat | Value | Purpose |
|------|-------|---------|
| STR | 100 | Max HP (100) |
| DEX | 100 | Max swing speed |
| INT | 25 | Light magic utility |

| Skill | Value | Purpose |
|-------|-------|---------|
| Lumberjacking | 100 | +damage bonus with axes |
| Tactics | 100 | +damage bonus |
| Anatomy | 100 | +damage bonus + bandage heal |
| Swords | 100 | Axe weapon skill |
| Healing | 100 | Bandage healing |
| Magic Resist | 100 | Magic defense |
| Magery | 75 | Emergency utility spells |
| Meditation | 25 | Mana regeneration |

**Total Skills**: 700 | **Equipment**: Double Axe (Vanq), Studded Armor

---

### Paladin
*Holy warrior with Chivalry abilities.*

| Stat | Value | Purpose |
|------|-------|---------|
| STR | 100 | Max HP (100) |
| DEX | 100 | Max swing speed |
| INT | 25 | Chivalry mana |

| Skill | Value | Purpose |
|-------|-------|---------|
| Swords | 100 | Primary weapon |
| Tactics | 100 | +damage bonus |
| Anatomy | 100 | +damage + bandage bonus |
| Parry | 100 | Shield blocking |
| Magic Resist | 100 | Magic defense |
| Healing | 100 | Bandage healing |
| Chivalry | 100 | Paladin abilities |

**Total Skills**: 700 | **Equipment**: Sword + Shield, Plate Armor

---

### Thief
*Dex-focused assassin with stealth abilities.*

| Stat | Value | Purpose |
|------|-------|---------|
| STR | 75 | HP (75) - backstab damage |
| DEX | 100 | Max stealth/speed |
| INT | 50 | Utility |

| Skill | Value | Purpose |
|-------|-------|---------|
| Fencing | 100 | Dagger weapon |
| Tactics | 100 | +damage bonus |
| Anatomy | 100 | +damage + bandage bonus |
| Hiding | 100 | Go invisible |
| Stealth | 100 | Move while hidden |
| Magic Resist | 100 | Magic defense |
| Healing | 100 | Bandage healing |

**Total Skills**: 700 | **Equipment**: Dagger, Leather Armor

---

## Mage Archetypes

### Mage (Pure Caster)
*Classic T2A mage - maximum spell power.*

| Stat | Value | Purpose |
|------|-------|---------|
| STR | 80 | HP pool (80) |
| DEX | 45 | Casting speed |
| INT | 100 | Max mana (100) |

| Skill | Value | Purpose |
|-------|-------|---------|
| Magery | 100 | Spell casting |
| Eval Int | 100 | Spell damage |
| Meditation | 100 | Mana regeneration |
| Magic Resist | 100 | Magic defense |
| Wrestling | 100 | Melee defense |
| Anatomy | 100 | Bandage heal bonus |
| Healing | 100 | Bandage healing |

**Total Skills**: 700 | **Equipment**: Staff, Robes

---

### Healer (Support Mage) - IMPLEMENTED
*Pure support class - keeps owner, sidekicks, and pets alive. Does NOT engage in melee combat.*

| Stat | Value | Purpose |
|------|-------|---------|
| STR | 80 | HP pool (80) |
| DEX | 45 | Casting speed |
| INT | 100 | Max mana (100) |

| Skill | Value | Purpose |
|-------|-------|---------|
| Magery | 100 | Heal/Cure spells |
| Veterinary | 100 | Bandage pets/creatures |
| Healing | 100 | Bandage players |
| Anatomy | 100 | +player heal bonus |
| Magic Resist | 100 | Magic defense |
| Meditation | 100 | Mana regeneration |
| Animal Lore | 100 | +pet heal bonus |

**Total Skills**: 700 | **Equipment**: Staff, Robes, 150 Bandages, 70 Potions

**Self-Healing Priority:**
1. Bandage loop - always active if below 100% and not poisoned
2. Greater Heal Potion at 75% health (instant)
3. Greater Heal spell at 50% health
4. At CRITICAL (25%): Teleport/Run escape + Greater Heal

**Heal Target Priority:** Self (critical) → Owner → Other Sidekicks → Pets

---

### Necromancer - IMPLEMENTED
*Dark magic specialist using necromancy spells for debuffs and DoT damage.*

| Stat | Value | Purpose |
|------|-------|---------|
| STR | 80 | HP pool (80) |
| DEX | 45 | Casting speed |
| INT | 100 | Max mana (100) |

| Skill | Value | Purpose |
|-------|-------|---------|
| Necromancy | 100 | Death magic |
| Spirit Speak | 100 | Spirit communication |
| Magery | 100 | Utility heals |
| Eval Int | 100 | Spell damage |
| Meditation | 100 | Mana regeneration |
| Magic Resist | 100 | Magic defense |
| Wrestling | 100 | Melee defense |

**Total Skills**: 700 | **Equipment**: Dark Robes, Necro Spellbook, Necro Reagents (50 each)

**Spell Priority:**
1. Evil Omen → Corpse Skin (debuffs)
2. Strangle (DoT)
3. Poison Strike (AoE)
4. Pain Spike (direct damage spam)

---

### Druid (Nature Mage)
*Animal affinity and nature magic.*

| Stat | Value | Purpose |
|------|-------|---------|
| STR | 80 | HP pool (80) |
| DEX | 45 | Casting speed |
| INT | 100 | Max mana (100) |

| Skill | Value | Purpose |
|-------|-------|---------|
| Magery | 100 | Spell casting |
| Eval Int | 100 | Spell damage |
| Meditation | 100 | Mana regeneration |
| Animal Lore | 100 | Beast knowledge |
| Veterinary | 100 | Pet healing |
| Magic Resist | 100 | Magic defense |
| Wrestling | 100 | Melee defense |

**Total Skills**: 700 | **Equipment**: Staff, Leather Armor

---

## Ranged Archetypes

### Archer
*Ranged damage with stealth ambush.*

| Stat | Value | Purpose |
|------|-------|---------|
| STR | 80 | Bow damage |
| DEX | 100 | Max archery speed |
| INT | 45 | Minimal |

| Skill | Value | Purpose |
|-------|-------|---------|
| Archery | 100 | Bow/Crossbow |
| Tactics | 100 | +damage bonus |
| Anatomy | 100 | +damage + bandage bonus |
| Healing | 100 | Bandage healing |
| Magic Resist | 100 | Magic defense |
| Hiding | 100 | Stealth ambush |
| Tracking | 100 | Find targets |

**Total Skills**: 700 | **Equipment**: Heavy Crossbow, Studded Armor

---

### Ranger
*Archer with animal knowledge.*

| Stat | Value | Purpose |
|------|-------|---------|
| STR | 80 | Bow damage |
| DEX | 100 | Max archery speed |
| INT | 45 | Minimal |

| Skill | Value | Purpose |
|-------|-------|---------|
| Archery | 100 | Ranged weapon |
| Tactics | 100 | +damage bonus |
| Anatomy | 100 | +damage + bandage bonus |
| Healing | 100 | Bandage healing |
| Magic Resist | 100 | Magic defense |
| Tracking | 100 | Find targets |
| Animal Lore | 100 | Beast knowledge |

**Total Skills**: 700 | **Equipment**: Bow, Studded Armor

---

## Hybrid Archetypes

### Battlemage
*Melee warrior with magic backup.*

| Stat | Value | Purpose |
|------|-------|---------|
| STR | 80 | HP (80), melee damage |
| DEX | 70 | Balanced swing speed |
| INT | 75 | Spell mana |

| Skill | Value | Purpose |
|-------|-------|---------|
| Swords | 100 | Melee weapon |
| Tactics | 100 | +damage bonus |
| Anatomy | 100 | +damage + bandage bonus |
| Magery | 100 | Spell casting |
| Eval Int | 100 | Spell damage |
| Magic Resist | 100 | Magic defense |
| Meditation | 100 | Mana regeneration |

**Total Skills**: 700 | **Equipment**: Sword + Shield, Studded Armor

---

### Cleric
*Holy melee with healing magic.*

| Stat | Value | Purpose |
|------|-------|---------|
| STR | 80 | HP (80), mace damage |
| DEX | 70 | Balanced swing speed |
| INT | 75 | Heal mana |

| Skill | Value | Purpose |
|-------|-------|---------|
| Macing | 100 | Classic cleric mace |
| Tactics | 100 | +damage bonus |
| Magery | 100 | Heal spells |
| Healing | 100 | Bandage healing |
| Anatomy | 100 | +heal bonus |
| Magic Resist | 100 | Magic defense |
| Parry | 100 | Shield blocking |

**Total Skills**: 700 | **Equipment**: Mace + Shield, Plate Armor

---

### Tamer
*Animal tamer with magic support.*

| Stat | Value | Purpose |
|------|-------|---------|
| STR | 80 | HP (80) - survive while taming |
| DEX | 45 | Casting speed |
| INT | 100 | Max mana for taming |

| Skill | Value | Purpose |
|-------|-------|---------|
| Animal Taming | 100 | Tame creatures |
| Animal Lore | 100 | Beast knowledge |
| Veterinary | 100 | Pet healing |
| Magery | 100 | Spell casting |
| Meditation | 100 | Mana regeneration |
| Magic Resist | 100 | Magic defense |
| Wrestling | 100 | Melee defense |

**Total Skills**: 700 | **Equipment**: Staff, Leather Armor

---

## Summary Table

| Archetype | STR | DEX | INT | Total | Combat Style | Optimal Range |
|-----------|-----|-----|-----|-------|--------------|---------------|
| **Warrior** | 100 | 100 | 25 | 225 | Melee | 1 |
| **Paladin** | 100 | 100 | 25 | 225 | Melee | 1 |
| **Thief** | 75 | 100 | 50 | 225 | Melee | 1 |
| **Mage** | 80 | 45 | 100 | 225 | Mage | 10 |
| **Healer** | 80 | 45 | 100 | 225 | Mage | 8 |
| **Necromancer** | 80 | 45 | 100 | 225 | Mage | 10 |
| **Druid** | 80 | 45 | 100 | 225 | Hybrid | 8 |
| **Archer** | 80 | 100 | 45 | 225 | Archer | 5 |
| **Ranger** | 80 | 100 | 45 | 225 | Archer | 5 |
| **Battlemage** | 80 | 70 | 75 | 225 | Hybrid | 5 |
| **Cleric** | 80 | 70 | 75 | 225 | Hybrid | 5 |
| **Tamer** | 80 | 45 | 100 | 225 | Hybrid | 3 |

---

## Skill Breakdown

### Damage Skills
- **Tactics**: +damage to physical attacks
- **Anatomy**: +damage to physical attacks + bandage heal bonus
- **Lumberjacking**: +damage with axes (stacks with above)
- **Eval Int**: +spell damage

### Defense Skills
- **Magic Resist**: Reduces spell damage and resist debuffs
- **Parry**: Block attacks with shield (requires shield equipped)
- **Wrestling**: Melee defense when unarmed or spell casting

### Utility Skills
- **Healing**: Bandage healing (affected by Anatomy)
- **Meditation**: Passive mana regeneration
- **Hiding/Stealth**: Invisibility and stealth movement

---

## Combat AI Behaviors

Each archetype has its own dedicated combat AI with specialized behaviors:

### WarriorCombatAI (Warrior, Tamer*)
- **Behavior**: Stay in melee range, tank damage
- **Healing**: Bandages on damage, Greater Heal Potions when low
- **Curing**: Greater Cure Potions, Cure spell (if Magery 40+)
- **Priority**: Survive and keep hitting

### MageCombatAI (Mage, Battlemage, Druid)
- **Behavior**: Kite at 10-16 tiles, combo spells
- **Combos**: Explosion + Energy Bolt, Poison combos
- **Healing**: Greater Heal spell, bandages, potions
- **Curing**: Cure spell, Greater Cure Potions
- **Priority**: Maintain distance, burst damage

### NecromancerCombatAI (Necromancer) - IMPLEMENTED
- **Behavior**: Maintain 5-10 tile range, debuff then damage
- **Spell Priority**:
  1. Evil Omen (next resist fails)
  2. Corpse Skin (reduce resists)
  3. Strangle (DoT + mana drain)
  4. Poison Strike (AoE poison)
  5. Pain Spike (direct damage spam)
  6. Wither (AoE when multiple enemies)
- **Healing**: Bandages, Greater Heal potion/spell
- **Curing**: Cure spell, Greater Cure Potions
- **Priority**: Debuff → DoT → Burst damage

### ArcherCombatAI (Archer, Ranger)
- **Behavior**: Maintain 4-10 tile range, kite if too close
- **Healing**: Bandages, Greater Heal Potions
- **Curing**: Greater Cure Potions
- **Priority**: Keep distance, steady damage

### ThiefCombatAI (Thief)
- **Behavior**: Hide, approach stealthed, backstab, escape when low
- **Healing**: Bandages, Greater Heal Potions
- **Escape**: Hide at 30% health, run away
- **Priority**: Stealth damage, escape when in danger

### PaladinCombatAI (Paladin)
- **Behavior**: Stay in melee, use Chivalry buffs
- **Abilities**: Close Wounds (heal), Consecrate Weapon, Divine Fury, Cleanse by Fire
- **Healing**: Close Wounds > Bandages > Potions
- **Priority**: Buffed melee combat

### HealerCombatAI (Healer, Cleric) - IMPLEMENTED
- **Behavior**: Pure support class - stays near heal targets within bandage range (2 tiles)
- **Does NOT engage in melee combat** - Combatant is not set
- **Heal Target Priority**: Self (critical) → Owner → Other Sidekicks → Pets
- **Self-Healing Loop**:
  1. Bandage loop - always active if below 100% and not poisoned
  2. Greater Heal Potion at 75% health (instant, no cast time)
  3. Greater Heal spell at 50% health
  4. At CRITICAL (25%): Teleport/Run escape + Greater Heal while escaping
- **Pet Healing**: Veterinary + Animal Lore for bandaging pets/creatures
- **Curing**: Cure potion > Cure spell on self and allies
- **NO offensive spells** - pure support
- **Priority**: Keep everyone alive, starting with self when critically low

### TamerCombatAI (Tamer)
- **Behavior**: Command pets to fight, stay at safe distance (10 tiles)
- **Pet Commands**: 
  - "All Kill" - Send pets to attack enemy
  - "All Follow Me" - Recall pets after combat
  - "All Guard Me" - Defensive stance when idle
- **Pet Healing**: 
  - Greater Heal spell when pet < 25% health
  - Cure spell when pet is poisoned
  - Bandages (Veterinary) when pet < 70% health
- **Self-Defense**: Retreat, command pets to protect, use potions
- **Priority**: Keep pets alive and fighting, stay safe

---

## AI Summary Table

| Archetype | Combat AI | Range | Healing | Kiting |
|-----------|-----------|-------|---------|--------|
| Warrior | WarriorCombatAI | 1 | Bandage+Potion | No |
| Paladin | PaladinCombatAI | 1 | Chivalry+Bandage | No |
| Thief | ThiefCombatAI | 1 | Bandage+Potion | Escape |
| Mage | MageCombatAI | 10 | Spell+Bandage | Yes |
| **Necromancer** | **NecromancerCombatAI** | **5-10** | **Spell+Bandage** | **Yes** |
| Battlemage | MageCombatAI | 5 | Spell+Bandage | Yes |
| Druid | MageCombatAI | 8 | Spell+Bandage | Yes |
| Archer | ArcherCombatAI | 5 | Bandage+Potion | Yes |
| Ranger | ArcherCombatAI | 5 | Bandage+Potion | Yes |
| Healer | HealerCombatAI | 8 | Spell+Bandage | Yes |
| Cleric | HealerCombatAI | 5 | Spell+Bandage | Yes |
| Tamer | TamerCombatAI | 10 | Vet+Spell | Yes |

---

*Last updated: Added NecromancerCombatAI with necromancy spells (Pain Spike, Poison Strike, Strangle, Evil Omen, Corpse Skin, Wither)*

