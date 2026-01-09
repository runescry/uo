# Autonomous Sidekick Archetypes - Specification

**Purpose:** Define the main archetypes for autonomous sidekick NPCs, including combat styles, personalities, behaviors, and capabilities.

**Date:** 2025-01-XX

---

## Overview

Each archetype represents a distinct combat role and personality combination. **Sidekicks are true companions that start as new characters and grow with the player** - they gain skills and stats over time, can die and be resurrected, and develop alongside their master.

### Key Characteristics

- **Starting Stats:** Use the same starting stats as player character archetypes (e.g., Mage starts with STR=25, DEX=20, INT=45)
- **Starting Skills:** Begin with 30.0 in 3-4 primary skills (total ~110-120 skill points)
- **Growth:** Gain skills and stats through use, just like players
- **Death & Resurrection:** Can die, become ghosts, and be resurrected like players
- **Persistence:** Stats and skills persist across sessions
- **Companion Bond:** True companion that grows and dies with you

Archetypes determine:
- **Combat Style:** How the sidekick fights (melee, ranged, magic, hybrid)
- **Starting Skills & Stats:** Initial attributes matching player archetypes
- **Equipment:** Preferred weapons and armor
- **Personality:** LLM personality type and speech pattern
- **Autonomous Behaviors:** How they act when not in combat
- **Combat Tactics:** Specific combat behaviors and strategies

---

## Core Combat Archetypes

### 1. Warrior (Melee Tank)

**Combat Role:** Front-line fighter, damage dealer, tank

**Starting Stats (New Character):**
- **STR:** 45 (matches player Warrior archetype)
- **DEX:** 35 (matches player Warrior archetype)
- **INT:** 10 (matches player Warrior archetype)
- **Total:** 90 (will grow to 225 cap over time)

**Starting Skills (New Character):**
- **Anatomy:** 30.0
- **Healing:** 30.0
- **Swordsmanship:** 30.0
- **Tactics:** 30.0
- **Total:** 120.0 skill points (will grow to 720 cap over time)

**Growth Path:**
- Skills and stats gain through use, just like players
- Will develop toward typical warrior build:
  - STR: 100-125 (high)
  - DEX: 70-90 (moderate)
  - INT: 55-65 (low-moderate)
  - Skills: Focus on Swordsmanship, Tactics, Anatomy, Healing, Parrying

**Combat Style:**
- **AI Type:** Enhanced MeleeAI
- **Range:** Melee (1 tile)
- **Positioning:** Front-line, engages enemies directly
- **Tactics:**
  - Charges into combat
  - Tanks damage for owner
  - Uses weapon abilities aggressively
  - Intercepts enemies attacking owner
  - Maintains close range

**Equipment:**
- **Weapons:** Swords, maces, axes (two-handed preferred)
- **Armor:** Plate/chainmail (high AR)
- **Shield:** Yes (for tanking)

**Personality:**
- **Type:** `PersonalityType.Warrior`
- **Speech Pattern:** `Archaic` or `Formal`
- **Traits:** Brave, direct, honor-focused, protective
- **Combat Commentary:**
  - "I'll hold the line!"
  - "Stand behind me!"
  - "For honor and glory!"
  - "That was a good strike!"

**Autonomous Behaviors:**
- Guards owner when idle
- Scans for threats
- Maintains defensive position
- Offers combat advice ("We should be careful here")

**Special Abilities:**
- Weapon abilities (Armor Ignore, Bleed Attack, etc.)
- Can use bandages on self/owner
- High survivability

---

### 2. Mage (Spell Caster)

**Combat Role:** Ranged damage, crowd control, support

**Starting Stats (New Character):**
- **STR:** 25 (matches player Magician archetype)
- **DEX:** 20 (matches player Magician archetype)
- **INT:** 45 (matches player Magician archetype)
- **Total:** 90 (will grow to 225 cap over time)

**Starting Skills (New Character):**
- **EvalInt:** 30.0
- **Wrestling:** 30.0
- **Magery:** 30.0
- **Meditation:** 30.0
- **Total:** 120.0 skill points (will grow to 720 cap over time)

**Growth Path:**
- Skills and stats gain through use, just like players
- Will develop toward typical mage build:
  - STR: 25-50 (low)
  - DEX: 50-70 (moderate)
  - INT: 105-150 (very high, can use enhancements to reach 150)
  - Skills: Focus on Magery, EvalInt, Meditation, Resisting Spells

**Combat Style:**
- **AI Type:** Enhanced MageAI
- **Range:** 8-12 tiles (spell range)
- **Positioning:** Back-line, maintains distance
- **Tactics:**
  - Kites enemies (runs while casting)
  - Prioritizes crowd control (Paralyze, Poison)
  - Uses area damage (Explosion, Flame Strike)
  - Teleports when threatened
  - Manages mana carefully

**Equipment:**
- **Weapons:** Spellbook, staff (backup)
- **Armor:** Robes (low AR, high MR)
- **Reagents:** Carries full reagent pack

**Personality:**
- **Type:** `PersonalityType.Mage`
- **Speech Pattern:** `Formal` or `Archaic`
- **Traits:** Intellectual, analytical, strategic, cautious
- **Combat Commentary:**
  - "The arcane forces are mine to command!"
  - "Stand back, I'll handle this with magic!"
  - "My mana is running low..."
  - "That spell was perfectly executed!"

**Autonomous Behaviors:**
- Meditates when mana low
- Studies spellbook when idle
- Offers magical advice
- Identifies magical threats

**Special Abilities:**
- Full spell repertoire (all circles)
- Spell combos (Paralyze → Explosion → Poison)
- Can heal with Greater Heal
- Can cure poison
- Teleportation for escape

---

### 3. Archer (Ranged DPS)

**Combat Role:** Ranged damage, kiting, support fire

**Primary Stats (Total = 225):**
- **STR:** 50-70 (moderate)
- **DEX:** 100-125 (very high, can use enhancements to reach 150)
- **INT:** 75-55 (moderate-low, adjusts to keep total at 225)
- **Total:** 225 ✅

**Primary Skills:**
- Archery: 100-120
- Tactics: 100-120
- Anatomy: 80-100
- Healing: 60-80
- Hiding: 40-60 (optional)
- Stealth: 40-60 (optional)

**Combat Style:**
- **AI Type:** Enhanced ArcherAI
- **Range:** 2-8 tiles (weapon range)
- **Positioning:** Mid-range, maintains optimal distance
- **Tactics:**
  - Kites enemies (runs while shooting)
  - Maintains weapon range
  - Focuses fire on single target
  - Switches targets if current dies
  - Uses special shots (Armor Pierce, etc.)

**Equipment:**
- **Weapons:** Bow, crossbow, composite bow
- **Armor:** Studded/leather (medium AR, high DEX)
- **Ammunition:** Carries arrows/bolts

**Personality:**
- **Type:** `PersonalityType.Ranger`
- **Speech Pattern:** `Casual` or `Archaic`
- **Traits:** Independent, precise, patient, observant
- **Combat Commentary:**
  - "I've got a clear shot!"
  - "Target acquired!"
  - "That arrow found its mark!"
  - "I'll cover you from here!"

**Autonomous Behaviors:**
- Scans for distant threats
- Maintains overwatch position
- Tracks movement patterns
- Offers tactical observations

**Special Abilities:**
- Special shots (Armor Pierce, Concussion Blow)
- Can use bandages
- High accuracy
- Fast attack speed

---

### 4. Healer (Support) - IMPLEMENTED

**Combat Role:** Pure support - heals owner, other sidekicks, and pets

**Primary Stats (Total = 225):**
- **STR:** 25-50 (low)
- **DEX:** 50-70 (moderate)
- **INT:** 105-150 (high, can use enhancements to reach 150)
- **Total:** 180-270 (with enhancements, base ~180-200) ✅

**Primary Skills:**
- Magery: 100-120 (for healing spells)
- Eval Int: 80-100
- Healing: 100-120
- Anatomy: 100-120
- Meditation: 60-80

**Combat Style:**
- **AI Type:** HealerCombatAI (Support Class)
- **Range:** 1-2 tiles from heal target (bandage range)
- **Positioning:** Near heal targets, stays out of melee when possible
- **Tactics:**
  - Does NOT engage in melee combat
  - Heal Priority: Self (critical) → Owner → Other Sidekicks → Pets
  - Bandage loop: Always keep bandages running if below 100%
  - Heal potion at 75% health (instant)
  - Greater Heal spell at 50% health
  - Escape (teleport/run) at 25% health while healing

**Equipment:**
- **Weapons:** Spellbook, staff (backup only)
- **Armor:** Robes or light armor
- **Supplies:** 150 bandages, 70 potions (heal/cure), full reagent pack

**Personality:**
- **Type:** `PersonalityType.Healer`
- **Speech Pattern:** `Casual` or `Formal`
- **Traits:** Compassionate, nurturing, protective, calm
- **Combat Commentary:**
  - "Let me heal those wounds!"
  - "You're safe now, I've got you!"
  - "Stay strong, I'm here to help!"
  - "Your health is my priority!"

**Autonomous Behaviors:**
- Monitors owner/sidekicks/pets health constantly
- Bandages proactively (always looping below 100%)
- Moves to heal targets within bandage range
- Cures poison on self and allies
- Flees when critically low health

**Self-Healing Priority (when taking damage):**
1. Bandage loop - always try if below 100% and not poisoned
2. Heal potion at 75% or below (instant, no casting time)
3. Greater Heal spell at 50% or below
4. At CRITICAL (25%): Escape (teleport if possible, run if not) + Greater Heal

**Special Abilities:**
- Greater Heal on self and allies
- Cure spells and potions
- Veterinary bandaging for pets
- Teleport escape when critical
- Does NOT cast offensive spells

**Implementation Notes:**
- Support class: `Combatant` is NOT set (prevents moving toward enemy)
- Uses `ControlTarget` as enemy reference
- Bandage range: 2 tiles (matches Bandage.Range)
- Stuck detection with `MoveToFreeTile` fallback

---

### 5. Paladin (Tank/Support Hybrid)

**Combat Role:** Tank with healing, protection, virtue-focused

**Primary Stats (Total = 225):**
- **STR:** 90-110 (high)
- **DEX:** 60-80 (moderate)
- **INT:** 55-75 (moderate)
- **Total:** 205-265 (exceeds 225) ⚠️ **ADJUSTED BELOW**

**Adjusted Stats (Total = 225):**
- **STR:** 80-100 (high)
- **DEX:** 60-80 (moderate)
- **INT:** 65-85 (moderate)
- **Total:** 205-265 (with enhancements, base ~205-220) ✅

**Primary Skills:**
- Swordsmanship: 100-120
- Tactics: 100-120
- Chivalry: 100-120
- Healing: 80-100
- Anatomy: 80-100
- Parrying: 80-100

**Combat Style:**
- **AI Type:** Enhanced MeleeAI with Chivalry
- **Range:** Melee (1 tile)
- **Positioning:** Front-line, protects owner
- **Tactics:**
  - Tanks like Warrior
  - Uses Chivalry abilities (Consecrate Weapon, Divine Fury)
  - Heals owner when needed
  - Protects owner from harm
  - Focuses on virtue and honor

**Equipment:**
- **Weapons:** Swords (virtue weapons preferred)
- **Armor:** Plate/chainmail (high AR)
- **Shield:** Yes

**Personality:**
- **Type:** `PersonalityType.Paladin`
- **Speech Pattern:** `Formal` or `Archaic`
- **Traits:** Righteous, honorable, protective, virtuous
- **Combat Commentary:**
  - "By the Virtues, I will protect you!"
  - "Justice will be served!"
  - "Stand with honor!"
  - "The Virtues guide my blade!"

**Autonomous Behaviors:**
- Upholds virtues
- Protects the weak
- Offers moral guidance
- Maintains high standards

**Special Abilities:**
- Chivalry abilities (Consecrate Weapon, Divine Fury, Enemy of One)
- Can heal with spells
- High survivability
- Virtue-based bonuses

---

### 6. Ranger (Scout/Hybrid)

**Combat Role:** Versatile fighter, scout, tracker

**Primary Stats (Total = 225):**
- **STR:** 50-70 (moderate)
- **DEX:** 90-110 (high, can use enhancements)
- **INT:** 65-85 (moderate)
- **Total:** 205-265 (with enhancements, base ~205-220) ✅

**Primary Skills:**
- Archery: 100-120
- Tactics: 80-100
- Tracking: 80-100
- Healing: 60-80
- Anatomy: 60-80
- Hiding: 60-80
- Stealth: 60-80

**Combat Style:**
- **AI Type:** Enhanced ArcherAI with melee capability
- **Range:** 2-8 tiles (prefers ranged)
- **Positioning:** Flexible, adapts to situation
- **Tactics:**
  - Prefers ranged combat
  - Can melee if needed
  - Uses stealth for positioning
  - Tracks enemies
  - Versatile combat style

**Equipment:**
- **Weapons:** Bow (primary), sword (backup)
- **Armor:** Studded/leather (medium AR)
- **Ammunition:** Arrows

**Personality:**
- **Type:** `PersonalityType.Ranger`
- **Speech Pattern:** `Casual` or `Archaic`
- **Traits:** Independent, resourceful, observant, adaptable
- **Combat Commentary:**
  - "I've tracked our enemy!"
  - "The wilderness is my ally!"
  - "I'll strike from the shadows!"
  - "Nature provides all we need!"

**Autonomous Behaviors:**
- Tracks enemies
- Scans for threats
- Uses stealth for reconnaissance
- Offers wilderness knowledge

**Special Abilities:**
- Tracking
- Stealth/Hiding
- Can use bandages
- Versatile combat

---

### 7. Thief (Rogue/Assassin)

**Combat Role:** High damage, stealth, utility

**Primary Stats (Total = 225):**
- **STR:** 25-50 (low)
- **DEX:** 100-125 (very high, can use enhancements to reach 150)
- **INT:** 75-100 (moderate-high)
- **Total:** 200-275 (with enhancements, base ~200-220) ✅

**Primary Skills:**
- Fencing: 100-120
- Tactics: 80-100
- Hiding: 100-120
- Stealth: 100-120
- Snooping: 60-80
- Lockpicking: 60-80
- Stealing: 60-80 (optional)

**Combat Style:**
- **AI Type:** Enhanced MeleeAI with stealth
- **Range:** Melee (1 tile)
- **Positioning:** Flanking, backstabbing
- **Tactics:**
  - Uses stealth to position
  - Attacks from behind (backstab)
  - High damage, low survivability
  - Disengages when threatened
  - Uses poison

**Equipment:**
- **Weapons:** Daggers, rapiers (fast, high damage)
- **Armor:** Leather (low AR, high DEX)
- **Tools:** Lockpicks, poison

**Personality:**
- **Type:** `PersonalityType.Thief`
- **Speech Pattern:** `Casual` or `Cryptic`
- **Traits:** Cunning, secretive, opportunistic, independent
- **Combat Commentary:**
  - "They never saw me coming!"
  - "Strike from the shadows!"
  - "A clean kill!"
  - "I'll make this quick!"

**Autonomous Behaviors:**
- Uses stealth for reconnaissance
- Identifies valuable targets
- Offers tactical information
- Scans for opportunities

**Special Abilities:**
- Stealth attacks (backstab)
- Poison application
- Lockpicking
- High critical hit chance
- Fast attack speed

---

### 8. Tamer (Pet Commander) - IMPLEMENTED

**Combat Role:** Support class - commands pets to fight, heals pets

**Primary Stats (Total = 225):**
- **STR:** 50-70 (moderate)
- **DEX:** 50-70 (moderate)
- **INT:** 85-125 (high, for healing spells)
- **Total:** 185-265 (with enhancements, base ~200-220) ✅

**Primary Skills:**
- Animal Taming: 100-120
- Animal Lore: 100-120
- Veterinary: 100-120
- Magery: 80-100 (for Greater Heal)
- Meditation: 60-80

**Combat Style:**
- **AI Type:** TamerCombatAI (Support Class)
- **Range:** 1-2 tiles from pet (bandage range)
- **Positioning:** Adjacent to pet, opposite side from enemy
- **Tactics:**
  - Commands pets to "ALL KILL" enemies
  - Stays near pet to bandage (veterinary healing)
  - Casts Greater Heal on pet when < 50% health
  - Does NOT engage in melee combat
  - Post-combat: heals pets to full health

**Equipment:**
- **Weapons:** Staff (backup only)
- **Armor:** Leather/studded (medium AR)
- **Supplies:** 500 bandages, full reagent pack

**Personality:**
- **Type:** `PersonalityType.Tamer`
- **Speech Pattern:** `Casual` or `Formal`
- **Traits:** Nurturing, protective of pets, patient, nature-focused
- **Combat Commentary:**
  - "Go get them, my friend!"
  - "I'll keep you healed!"
  - "Stay strong, I'm right here!"
  - "Together we are unstoppable!"

**Autonomous Behaviors:**
- Commands pets to attack enemies
- Monitors pet health constantly
- Bandages pets proactively
- Stays out of melee combat
- Heals pets to full after combat

**Special Abilities:**
- Pet command system (ALL KILL, FOLLOW, GUARD)
- Veterinary bandaging (10-second cooldown)
- Greater Heal on pets (not self)
- Post-combat recovery mode

**Implementation Notes:**
- Support class: `Combatant` is NOT set (prevents moving toward enemy)
- Uses `ControlTarget` as enemy reference
- `PendingHealTarget` property for spell targeting
- `m_ReachedPet` hysteresis for stable positioning

---

### 9. Necromancer (Dark Mage) - IMPLEMENTED

**Combat Role:** Damage over time, debuffs, dark magic specialist

**Primary Stats:**
- **STR:** 80 (HP pool)
- **DEX:** 45 (casting speed)
- **INT:** 100 (max mana pool)

**Primary Skills:**
- Necromancy: 100
- Spirit Speak: 100
- Magery: 100 (for utility heals)
- Eval Int: 100
- Meditation: 100
- Magic Resist: 100
- Wrestling: 100

**Combat Style:**
- **AI Type:** NecromancerCombatAI (custom)
- **Range:** 5-10 tiles (spell range)
- **Positioning:** Back-line, maintains safe distance
- **Tactics:**
  - Applies debuffs first (Evil Omen → Corpse Skin)
  - DoT with Strangle
  - AoE damage with Poison Strike
  - Spam Pain Spike for direct damage
  - Uses Wither when multiple enemies nearby
  - Retreats and heals when low health

**Equipment:**
- **Weapons:** Necromancer Spellbook (all necro spells), Standard Spellbook
- **Armor:** Dark Robes
- **Reagents:** Standard reagents (50 each) + Necro reagents (50 each: Bat Wing, Grave Dust, Daemon Blood, Nox Crystal, Pig Iron)

**Spell Priority:**
1. **Debuffs**: Evil Omen (next resist check fails) → Corpse Skin (reduces resists)
2. **DoT**: Strangle (mana drain + damage over time)
3. **AoE**: Poison Strike (poison damage burst)
4. **Direct**: Pain Spike (instant damage, spammable)
5. **Multi-target**: Wither (AoE when 2+ enemies)

**Self-Healing:**
- Bandages when not poisoned
- Greater Heal Potion at 50% health
- Greater Heal spell when low
- Cure Potion/Spell when poisoned
- Retreat when enemy too close

**Personality:**
- **Type:** `PersonalityType.Mage` (with dark twist)
- **Speech Pattern:** `Cryptic` or `Archaic`
- **Traits:** Dark, mysterious, power-focused, pragmatic

**Special Abilities:**
- Pain Spike (direct damage, 5 mana, 20 skill)
- Poison Strike (AoE poison, 17 mana, 50 skill)
- Strangle (DoT + mana drain, 29 mana, 65 skill)
- Evil Omen (next resist fails, 11 mana, 20 skill)
- Corpse Skin (reduce resists, 11 mana, 20 skill)
- Wither (AoE cold damage, 23 mana, 60 skill)

---

## Hybrid Archetypes

### 9. Battlemage (Warrior + Mage)

**Combat Role:** Melee fighter with spell support

**Primary Stats (Total = 225):**
- **STR:** 70-90 (moderate-high)
- **DEX:** 60-80 (moderate)
- **INT:** 75-95 (high)
- **Total:** 205-265 (with enhancements, base ~205-220) ✅

**Primary Skills:**
- Swordsmanship: 100-120
- Magery: 80-100
- Tactics: 100-120
- Eval Int: 80-100
- Healing: 60-80

**Combat Style:**
- **AI Type:** Hybrid (MeleeAI + MageAI)
- **Range:** 1-10 tiles (melee + spells)
- **Positioning:** Mid-range, flexible
- **Tactics:**
  - Melees when close
  - Casts spells when far
  - Buffs self (Bless, Protection)
  - Versatile combat

**Personality:**
- **Type:** `PersonalityType.Warrior` or `PersonalityType.Mage`
- **Speech Pattern:** `Formal` or `Archaic`
- **Traits:** Strategic, versatile, intelligent, combat-focused

---

### 10. Cleric (Healer + Warrior)

**Combat Role:** Support fighter, healer with melee capability

**Primary Stats (Total = 225):**
- **STR:** 50-70 (moderate)
- **DEX:** 50-70 (moderate)
- **INT:** 105-150 (high, can use enhancements to reach 150)
- **Total:** 205-290 (with enhancements, base ~205-220) ✅

**Primary Skills:**
- Swordsmanship: 80-100
- Magery: 80-100
- Healing: 100-120
- Anatomy: 100-120
- Tactics: 80-100

**Combat Style:**
- **AI Type:** Hybrid (MeleeAI + HealerAI)
- **Range:** 1-10 tiles
- **Positioning:** Mid-range, near owner
- **Tactics:**
  - Heals owner primarily
  - Can melee if needed
  - Buffs owner
  - Support-focused

**Personality:**
- **Type:** `PersonalityType.Healer`
- **Speech Pattern:** `Formal` or `Casual`
- **Traits:** Compassionate, protective, supportive

---

## Archetype Selection Matrix

| Archetype | Combat Role | Primary Stat | Range | Complexity | Status |
|-----------|-------------|--------------|-------|------------|--------|
| Warrior | Tank/DPS | STR | Melee | Low | ✅ Implemented |
| Mage | Ranged DPS | INT | 8-12 | High | ✅ Implemented |
| Archer | Ranged DPS | DEX | 2-8 | Medium | Planned |
| **Healer** | **Support** | **INT** | **1-2** | **Medium** | **✅ Implemented** |
| Paladin | Tank/Support | STR/INT | Melee | Medium | Planned |
| Ranger | Scout/DPS | DEX | 2-8 | Medium | Planned |
| Thief | DPS/Utility | DEX | Melee | High | Planned |
| **Tamer** | **Pet Support** | **INT** | **1-2** | **Medium** | **✅ Implemented** |
| **Necromancer** | **DPS/Debuff** | **INT** | **5-10** | **High** | **✅ Implemented** |
| Battlemage | Hybrid | STR/INT | 1-10 | Very High | Planned |
| Cleric | Support | INT | 1-10 | Medium | Planned |

---

## Personality Integration

### Personality Traits by Archetype

**Warrior:**
- Direct, brave, honor-focused
- Protective of owner
- Dislikes cowardice
- Values strength

**Mage:**
- Intellectual, analytical
- Strategic thinker
- Cautious
- Knowledge-focused

**Archer:**
- Independent, precise
- Patient, observant
- Tactical
- Nature-focused

**Healer:**
- Compassionate, nurturing
- Protective, caring
- Calm under pressure
- Life-focused

**Paladin:**
- Righteous, honorable
- Virtue-focused
- Protective
- Justice-oriented

**Ranger:**
- Independent, resourceful
- Adaptable, observant
- Wilderness-focused
- Self-reliant

**Thief:**
- Cunning, secretive
- Opportunistic
- Independent
- Shadow-focused

**Necromancer:**
- Dark, mysterious
- Power-focused
- Pragmatic
- Death-focused

---

## Autonomous Behavior Patterns

### By Situation

**Idle (No Combat):**
- **Warrior:** Guards owner, scans for threats
- **Mage:** Meditates, studies spellbook
- **Archer:** Maintains overwatch, tracks movement
- **Healer:** Monitors owner's health
- **Paladin:** Maintains virtue, protects
- **Ranger:** Scouts area, tracks
- **Thief:** Uses stealth, identifies opportunities
- **Necromancer:** Maintains minions, studies

**Combat:**
- **Warrior:** Charges, tanks, protects
- **Mage:** Kites, casts, manages mana
- **Archer:** Maintains range, focuses fire
- **Healer:** Heals, buffs, stays safe
- **Paladin:** Tanks, heals, protects
- **Ranger:** Adapts, versatile combat
- **Thief:** Flanks, backstabs, disengages
- **Necromancer:** Debuffs, summons, damages

**Owner Low Health:**
- **Warrior:** Intercepts enemies, protects
- **Mage:** Teleports owner away, heals
- **Archer:** Focuses fire on threats
- **Healer:** Prioritizes healing
- **Paladin:** Protects, heals, defends
- **Ranger:** Distracts enemies
- **Thief:** Eliminates threats quickly
- **Necromancer:** Debuffs enemies, supports

---

## Implementation Notes

### Starting Stats & Skills

**Sidekicks start as new characters** using the same starting stats and skills as player character archetypes:

| Archetype | Starting Stats | Starting Skills |
|-----------|---------------|-----------------|
| Warrior | STR=45, DEX=35, INT=10 (total=90) | Anatomy=30, Healing=30, Swords=30, Tactics=30 (total=120) |
| Mage | STR=25, DEX=20, INT=45 (total=90) | EvalInt=30, Wrestling=30, Magery=30, Meditation=30 (total=120) |
| Necromancer | STR=25, DEX=20, INT=45 (total=90) | Necromancy=30, SpiritSpeak=30, Swords=30, Meditation=20 (total=110) |
| Paladin | STR=45, DEX=20, INT=25 (total=90) | Chivalry=30, Swords=30, Focus=30, Tactics=30 (total=120) |

**Growth:**
- Skills and stats gain through use, just like players
- Use `SkillCheck.cs` system for skill gains
- Use `TryStatGain()` for stat gains
- Controlled creatures use `PetChanceToGainStats` (5% default)

### Skill & Stat Caps

**IMPORTANT:** Verify your server's `TotalSkillCap` in `Config/PlayerCaps.cfg`
- **Standard UO:** 720 total skill points (sum of all skills)
- **This Server:** Check `TotalSkillCap` value (may be configured differently)
- **Individual Skill Cap:** 100.0 (can be raised to 120.0 with power scrolls)
- **Total Stat Cap:** 225 (STR + DEX + INT)
- **Individual Stat Cap:** 125 base, 150 max with enhancements

**Note:** Skills use fixed-point format (multiply by 10), so `TotalSkillCap=1000` = 100.0 skill points. If your server uses a different cap, adjust accordingly.

### Death & Resurrection

**Sidekicks can die and be resurrected like players:**
- Override `OnDeath()` to prevent deletion (set `Player = true` or handle specially)
- Use `Resurrect()` method to bring back from death
- Can become ghosts when dead (like players)
- Death penalties apply (stat/skill loss if configured)
- Can be resurrected by owner or other players

**Implementation:**
```csharp
public override void OnDeath(Container c)
{
    // Don't delete - become ghost like player
    if (!m_Player)
        m_Player = true; // Mark as player-like for resurrection
    
    base.OnDeath(c);
}

public override bool CheckResurrect()
{
    // Allow resurrection if owner is nearby or by owner's command
    return ControlMaster != null && ControlMaster.InRange(this, 12);
}
```

### Equipment Generation
- Spawn with appropriate starting equipment for archetype (matching player character creation)
- Equipment should match skill focus
- Can upgrade equipment over time as sidekick grows

### Stat & Skill Initialization
Use `CharacterCreation` methods for starting stats and skills:
- `InitStats(int str, int dex, int intel)` - Sets starting stats
- `SetSkill(SkillName name, double val)` - Sets starting skills

**Example (Warrior):**
```csharp
// Starting stats (matches player Warrior)
InitStats(45, 35, 10);

// Starting skills (matches player Warrior)
SetSkill(SkillName.Anatomy, 30.0);
SetSkill(SkillName.Healing, 30.0);
SetSkill(SkillName.Swordsmanship, 30.0);
SetSkill(SkillName.Tactics, 30.0);
```

**Growth:**
- Skills and stats will gain automatically through `SkillCheck.cs` system
- No manual intervention needed - sidekick grows naturally through use

### AI Selection
- Each archetype uses appropriate base AI
- Enhanced with custom logic for player-like behavior
- LLM integration for personality and commentary

### Balance Considerations
- Sidekicks should be helpful but not overpowered
- Damage output balanced to player level
- Survivability appropriate to role
- Mana/stamina management important

---

## Example Spawn Commands

```
[SpawnSidekick Warrior Melee
[SpawnSidekick Mage Spell
[SpawnSidekick Archer Ranged
[SpawnSidekick Healer Support
[SpawnSidekick Paladin Hybrid
[SpawnSidekick Ranger Scout
[SpawnSidekick Thief Stealth
[SpawnSidekick Necromancer Dark
```

---

## Future Enhancements

### Advanced Archetypes
- **Samurai:** Honor-focused warrior with special abilities
- **Ninja:** Stealth assassin with special techniques
- **Bard:** Support with songs and buffs
- **Druid:** Nature mage with animal companions
- **Monk:** Unarmed fighter with special techniques

### Customization
- Allow players to customize archetype
- Mix and match skills
- Custom personality traits
- Equipment preferences

### Progression
- Sidekicks gain experience
- Skill improvements over time
- Equipment upgrades
- Relationship development

---

## Summary

These archetypes provide distinct gameplay experiences:
- **Tanks:** Warrior, Paladin (protect owner)
- **DPS:** Mage, Archer, Thief, Necromancer (deal damage)
- **Support:** Healer, Cleric (heal and buff)
- **Hybrids:** Battlemage, Cleric, Ranger (versatile)

Each archetype has unique:
- Combat style and tactics
- Personality and speech
- Autonomous behaviors
- Special abilities

This variety allows players to choose sidekicks that complement their playstyle and provide engaging, dynamic companions.

