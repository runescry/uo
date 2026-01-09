# Vystia Complete Class Design
## 26 Unique Classes for ServUO
### 12 Magic Schools + 14 Martial Classes

---

# Part One: Design Philosophy

Each class must have:
1. **Clear Fantasy** — One sentence that explains the class
2. **Core Mechanic** — Unique resource or system that defines gameplay
3. **Role Identity** — What they bring to a group
4. **Internal Synergy** — Abilities that combo within the class
5. **Weakness** — Something they're bad at

---

# Part Two: Complete Class Matrix

## Magic Classes

| Class | School | Fantasy | Core Mechanic | Role | Strength | Weakness |
|-------|--------|---------|---------------|------|----------|----------|
| **Sorcerer** | Elementalist | Master of four elements | Stance dancing | Flex | Adapts to any situation | Jack of all trades |
| **Warlock** | Warlock | Dark bargainer | Soul Shards + Demon | DoT DPS | Sustained damage, self-healing | Squishy, slow ramp-up |
| **Necromancer** | Necromancer | Undead army commander | Corpse economy | Pet Swarm | Overwhelming numbers | Useless without corpses |
| **Witch** | Curse Witch | Hex specialist | Voodoo + Curse stacking | Debuffer | Shuts down enemies | Zero burst damage |
| **Enchanter** | Enchanter | Mind controller | CC chains + Haste | Controller | Best CC in game | Zero damage |
| **Oracle** | Chronomancer | Time manipulator | Rewind + Time Wells | Support/Survive | Undo mistakes | Complex, low damage |
| **Illusionist** | Mesmer | Illusionist trickster | Clones + Shatter | Burst/Chaos | Burst damage, confusion | Clones die easily |
| **Ice Mage** | Ice Mage | Frozen executioner | Chill → Freeze → Shatter | Control DPS | Lock down + execute | Weak vs fire resist |
| **Shaman** | Shaman | Spirit channeler | Totems + Spirits | Burst/Utility | Setup into big burst | Totems can be killed |
| **Summoner** | Summoner | Beast master | Creature slots + Bonds | Pet Tank/DPS | Creature variety | Weak alone |
| **Druid** | Druid | Shapeshifter | Form switching | Hybrid | Does everything okay | Forms have cooldowns |
| **Bard** | Bard | Battle musician | Songs + Crescendo | Support | Best party buffs | Must channel, vulnerable |

## Martial Classes

| Class | System | Fantasy | Core Mechanic | Role | Strength | Weakness |
|-------|--------|---------|---------------|------|----------|----------|
| **Paladin** | Chivalry | Holy warrior | Tithing + Virtues | Tank/Support | Sustain, party buffs, undead killer | Low burst, mana hungry |
| **Templar** | Judgment | Zealous inquisitor | Zeal stacks + Judgment | Burst DPS | Execute damage, anti-magic | No sustain, all-in |
| **Knight** | Valor | Stalwart defender | Shield mechanics + Rallying | Tank | Best damage mitigation | Low mobility, low damage |
| **Fighter** | Combat Mastery | Weapon master | Stance dancing + Weapon specs | Melee DPS | Versatile, consistent | No magic, no utility |
| **Barbarian** | Rage | Rage-fueled berserker | Fury buildup + Rage transform | Burst Melee | Huge burst, unstoppable | Vulnerable after rage |
| **Monk** | Ki | Disciplined martial artist | Chi + Combo points | Combo DPS | Sustained damage, mobility | Squishy, complex |
| **Rogue** | Shadow Arts | Shadow operative | Stealth + Combo points | Burst/Utility | Ambush damage, utility | Weak in sustained fights |
| **Ranger** | Wilderness | Wilderness hunter | Focus + Terrain mastery | Ranged/Hybrid | Kiting, tracking, traps | Weak in melee |
| **Bounty Hunter** | Pursuit | Relentless tracker | Marks + Pursuit stacks | Hunter/Debuff | Single target shutdown | Weak vs groups |
| **Beastmaster** | Pack Tactics | Pack alpha | Bond + Pack coordination | Pet DPS | Multiple pets, synergy | Weak alone |
| **Artificer** | Engineering | Combat engineer | Steam + Charges | Gadget DPS | Turrets, AoE, zones | Setup time, immobile |
| **Alchemist** | Alchemy | Battle chemist | Reagents + Mutagens | Burst/Utility | Consumable burst, buffs | Resource dependent |
| **Cleric** | Divine Magic | Divine healer | Faith + Prayer | Healer/Support | Best heals, resurrection | Zero damage |
| **Wizard** | Arcana | Arcane generalist | Spell Power + Metamagic | Flex Caster | Spell variety, research | Master of none |

---

# Part Three: Magic Schools (12 Schools)


---

# 1. ELEMENTALIST (Sorcerer)

### *"Master of the four primal elements"*

**Inspired By:** GW2 Elementalist, FFXIV Black Mage
**Role:** Flexible DPS/Support/Tank depending on stance
**Core Mechanic:** Stance switching with combo effects

## Stance System

```csharp
public ElementalStance ActiveStance { get; set; }
// Switching stances triggers combo from previous stance
// 2 second cooldown between switches
```

| Stance | Passive | Role | Color |
|--------|---------|------|-------|
| 🔥 Fire | +20% damage dealt | Burst DPS | Red |
| 💧 Water | +5 HP/tick regen | Healer | Blue |
| 🪨 Earth | +25 all resists | Tank | Brown |
| 💨 Air | +30% movement | Mobility | White |

## Stance Combos (On Switch)

| From → To | Combo Effect |
|-----------|--------------|
| Fire → Water | **Steam Blast**: 5 tile blind AoE |
| Fire → Earth | **Molten Armor**: +30 fire resist, thorns damage |
| Fire → Air | **Firestorm**: Fire tornado follows you 10s |
| Water → Fire | **Flash Boil**: Instant 25-40 damage AoE |
| Water → Earth | **Mudslide**: 6 tile -60% slow zone |
| Water → Air | **Mist Form**: Invisible + invulnerable 2s |
| Earth → Fire | **Eruption**: Lava geyser at your location |
| Earth → Water | **Geyser**: Knockup + heal AoE |
| Earth → Air | **Sandstorm**: 8 tile blind + damage AoE |
| Air → Fire | **Lightning Storm**: Chain lightning AoE |
| Air → Water | **Blizzard**: Cold AoE + slow |
| Air → Earth | **Dust Devil**: Pulling vortex AoE |

## Spells (32 total — 8 per stance)

### 🔥 Fire Stance

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Fireball | 6-12 fire damage |
| 2 | Ignite | 5-8 fire/tick for 12s, spreads on death |
| 3 | Fire Breath | Cone 6 tiles, 18-28 fire |
| 4 | Meteor | 28-42 fire + burning ground item 8s |
| 5 | Flame Wave | Line 10 tiles, 32-48 fire |
| 6 | Pyroclasm | 8 tile AoE, 40-60 fire, all ignited |
| 7 | Phoenix Fire | 55-80 fire + if kills, reset cooldowns |
| 8 | Avatar of Flame | Transform: Immune fire, +75% fire dmg, 45s |

### 💧 Water Stance

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Water Bolt | 5-10 cold, 20% slow 4s |
| 2 | Healing Stream | Target heals 8-14 HP |
| 3 | Cleansing Rain | 5 tile AoE, cure poison + heal 10-16 |
| 4 | Geyser | 22-35 cold + knockback 4 tiles |
| 5 | Healing Tide | 6 tile AoE, 8-12 HP/tick for 10s |
| 6 | Waterspout | Pull all enemies to center + 30-45 cold |
| 7 | Tsunami | 12 tile wave, 48-72 cold, knockback 6 |
| 8 | Avatar of Water | Transform: 2x healing, immune cold, 45s |

### 🪨 Earth Stance

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Stone Shard | 5-10 physical |
| 2 | Earth Armor | +20 physical resist 60s |
| 3 | Quickite | Root 5s |
| 4 | Rock Wall | Spawn 6 stone items blocking movement, 30s |
| 5 | Earthquake | 6 tile AoE, 28-42 phys, stun 2s |
| 6 | Stone Skin | -50% all damage taken for 15s |
| 7 | Meteor Shower | 10 tile AoE, 50-75 phys, slow terrain |
| 8 | Avatar of Earth | Transform: +75 resists, taunt aura, 45s |

### 💨 Air Stance

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Shock | 5-10 energy |
| 2 | Tailwind | +40% move speed 20s |
| 3 | Lightning Bolt | 16-26 energy |
| 4 | Blink | Teleport 10 tiles |
| 5 | Chain Lightning | 28-42 energy, chains to 4 targets |
| 6 | Cyclone | 6 tile AoE, -50% speed, pulls in |
| 7 | Thunderstorm | 10 tile AoE, 45-68 energy, random stuns |
| 8 | Avatar of Air | Transform: Fly, +75% speed, 50% dodge, 45s |

---

# 2. WARLOCK

### *"Power at any price"*

**Inspired By:** WoW Warlock, Diablo Necromancer
**Role:** Sustained DoT damage + Demon pet
**Core Mechanic:** Soul Shards + Single powerful demon

## Soul Shard System

```csharp
public int SoulShards { get; set; } // Max 5
// Generate: Kill enemy while Drain Soul active
// Generate: Shadow Bolt crits
// Spend: Empower spells, instant summons
```

## Demon Hierarchy (One Active)

| Demon | Circle | Stats | Role |
|-------|:------:|-------|------|
| Imp | 2 | 150 HP, 8-12 fire | Ranged DPS |
| Voidwalker | 4 | 500 HP, 10-16 shadow | Tank (Taunt) |
| Succubus | 5 | 250 HP, 14-22 shadow | CC (Seduce) |
| Felhunter | 5 | 350 HP, 16-24 phys | Anti-mage (Silence, Devour Magic) |
| Felguard | 7 | 700 HP, 28-40 phys | Melee DPS (Cleave) |
| Infernal | 8 | 1000 HP, 35-50 fire | AoE (Stun on summon, fire aura) |

## Spells (32 total)

### Affliction (DoTs)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Corruption | 4-7 shadow/tick 18s, spreads on death |
| 2 | Curse of Agony | 2→4→6→8→10 ramping shadow/tick 24s |
| 2 | Curse of Weakness | -20% damage dealt 30s |
| 3 | Curse of Tongues | +75% cast time 15s |
| 3 | Siphon Life | 5-8 shadow/tick, heals you 100%, 21s |
| 5 | Unstable Affliction | 8-12 shadow/tick 15s, damages dispeller |
| 7 | Seed of Corruption | DoT, explodes 35-50 AoE when target dies |
| 8 | Doom | Target dies in 60s unless healed to 100% |

### Destruction (Burst)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Shadow Bolt | 6-12 shadow, crit = Soul Shard |
| 3 | Immolate | 15-25 fire + 6-10/tick 15s |
| 4 | Shadowburn | 30-45 shadow, costs 1 Shard, instant |
| 5 | Conflagrate | Consume Immolate for 40-60 instant fire |
| 5 | Rain of Fire | 8 tile AoE, 10-16 fire/tick 8s |
| 6 | Soul Fire | 55-80 fire, 3s cast, costs 1 Shard |
| 7 | Chaos Bolt | 50-75 shadow+fire, ignores resist |
| 8 | Cataclysm | 12 tile AoE, 70-100 fire, Immolate all |

### Demonology (Pets + Transform)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 2 | Summon Imp | Summon ranged demon |
| 3 | Drain Life | Channel: 10-16 shadow/tick, heal 100% |
| 3 | Drain Soul | Channel: 8-14/tick, Shard on kill |
| 4 | Summon Voidwalker | Summon tank demon |
| 4 | Health Funnel | Channel: Your HP → Demon HP |
| 5 | Summon Succubus | Summon CC demon |
| 5 | Summon Felhunter | Summon anti-mage demon |
| 6 | Demonic Empowerment | Demon +50% stats 30s |
| 7 | Summon Felguard | Summon melee demon |
| 7 | Demonic Sacrifice | Kill demon → buff (varies by demon) |
| 8 | Summon Infernal | Meteor summon, AoE stun |
| 8 | Metamorphosis | Transform: Demon form, new abilities, 30s |

### Sacrifice Buffs (From Demonic Sacrifice)

| Demon Sacrificed | Buff Gained (30 min) |
|------------------|---------------------|
| Imp | +15% fire damage |
| Voidwalker | +10% max HP |
| Succubus | +10% shadow damage |
| Felhunter | +20 mana/tick |
| Felguard | +15% all damage |


---

# 3. NECROMANCER

### *"Death is only the beginning"*

**Inspired By:** Diablo 2 Necromancer, GW2 Necromancer
**Role:** Undead army commander
**Core Mechanic:** Corpse economy + Life Force

## Corpse System

```csharp
// Corpses are invisible items at death locations
// Corpses last 60 seconds
// Many spells consume corpses
public int NearbyCorpses => GetCorpsesInRange(12);
```

## Life Force (Secondary Resource)

```csharp
public int LifeForce { get; set; } // Max 100
// Generate: Nearby enemy deaths (+10 per death)
// Generate: Life drain spells
// Spend: Death Shroud, powerful abilities
```

## Undead Minions (Slot System: 5 max)

| Minion | Circle | Slots | Stats | Special |
|--------|:------:|:-----:|-------|---------|
| Skeleton | 1 | 1 | 100 HP, 8-12 phys | Basic melee |
| Zombie | 2 | 1 | 150 HP, 6-10 phys | Slow, infects |
| Skeletal Mage | 3 | 2 | 80 HP, 12-18 shadow | Ranged |
| Bone Golem | 4 | 2 | 400 HP, 15-22 phys | Taunt, tough |
| Wraith | 5 | 2 | 120 HP, 18-28 shadow | Flies, life drain |
| Revenant | 6 | 3 | 300 HP, 22-35 phys | Uses weapon skills |
| Bone Dragon | 7 | 4 | 800 HP, 35-50 phys | Breath attack, flight |
| Lich | 8 | 5 | 500 HP, 40-60 shadow | Casts spells, aura |

## Spells (32 total)

### Summoning

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Raise Skeleton | Summon from corpse, 1 slot |
| 2 | Raise Zombie | Summon from corpse, 1 slot, infects |
| 3 | Raise Skeletal Mage | Summon from corpse, 2 slots |
| 4 | Raise Bone Golem | Summon (no corpse), 2 slots |
| 5 | Raise Wraith | Summon from corpse, 2 slots |
| 6 | Raise Revenant | Summon from corpse, 3 slots |
| 7 | Raise Bone Dragon | Summon (3 corpses), 4 slots |
| 8 | Raise Lich | Summon from powerful corpse, 5 slots |

### Corpse Skills

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Corpse Explosion | Detonate corpse: 20-35 AoE phys |
| 2 | Consume Corpse | Eat corpse: heal 25-40 HP |
| 3 | Bone Wall | Create from corpses: 6 tile wall |
| 4 | Corpse Lance | Launch bones from corpse: 30-48 phys |
| 5 | Mass Corpse Explosion | Detonate ALL corpses in range |
| 6 | Desecrate | Create 3 corpses at target location |
| 7 | Army of the Dead | Raise ALL corpses as skeletons (temp) |

### Death Magic

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Death Bolt | 5-10 shadow |
| 2 | Weaken | -15 STR, -15% damage 30s |
| 3 | Life Tap | Curse: attacks heal attacker 25% |
| 4 | Bone Spear | 25-40 phys, pierces |
| 5 | Decrepify | -50% speed, -50% damage 15s |
| 6 | Bone Spirit | Homing 45-65 shadow |
| 7 | Death Wave | Cone 10 tiles, 50-75 shadow, heal per hit |
| 8 | Death Shroud | Transform: Use Life Force as HP, new skills, ends when depleted |

### Death Shroud Skills (While Transformed)

| Skill | Life Force Cost | Effect |
|-------|:---------------:|--------|
| Life Blast | 10 | 35-50 shadow |
| Dark Path | 15 | Teleport + damage trail |
| Doom | 25 | AoE fear 5s |
| Consume | 0 | Eat nearby corpse for +20 Life Force |

---

# 4. CURSE WITCH (Witch)

### *"Your pain is my pleasure"*

**Inspired By:** DAoC Bonedancer, WoW Affliction
**Role:** Pure debuffer, anti-healer
**Core Mechanic:** Curse stacking + Voodoo Doll

## Curse Stacking

```csharp
// Multiple curses stack on target
// Some spells scale with curse count
public int GetCurseCount(Mobile target);
```

## Voodoo Doll

```csharp
public class VoodooDoll : Item
{
    public Mobile LinkedTarget { get; set; }
    public DateTime LinkExpires { get; set; } // 60s duration
    // Doll spells bypass range/LoS to target
}
```

## Spells (32 total)

### Basic Curses (Circle 1-3)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Hex of Pain | 3-5 shadow/tick 18s |
| 1 | Hex of Weakness | -15 STR 30s |
| 1 | Hex of Clumsiness | -15 DEX 30s |
| 2 | Hex of Stupidity | -15 INT 30s |
| 2 | Hex of Sloth | -30% attack speed 20s |
| 2 | Hex of Lethargy | -30% movement 20s |
| 3 | Hex of Vulnerability | -20 all resists 25s |
| 3 | Hex of Exhaustion | -50% stamina regen 30s |

### Anti-Healing (Circle 3-6)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 3 | Mortal Wound | -50% healing received 15s |
| 4 | Festering Wound | Heals deal damage instead 8s |
| 4 | Life Siphon | Steal 25-40 HP from target |
| 5 | Soul Rot | -75% healing, 6-10 shadow/tick 12s |
| 5 | Mana Rot | -75% mana regen, burn 5 mana/tick 12s |
| 6 | Death Mark | Target takes 2x damage from all sources 8s |

### Voodoo (Circle 4-7)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 4 | Create Voodoo Doll | Link to target 60s |
| 4 | Stick Pins | (Doll) 20-35 shadow, ignores armor |
| 5 | Twist Limbs | (Doll) -50% speed, -50% attack 10s |
| 5 | Burn Doll | (Doll) 8-14 fire/tick 12s |
| 6 | Break Doll | (Doll) 50-75 damage, stun 3s, doll destroyed |
| 7 | Soul Bind | (Doll) Damage you take splits to target |

### Scaling Curses (Circle 5-8)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 5 | Curse Amplification | Your curses deal 2x effect 15s |
| 5 | Contagion | Your curses spread to nearby enemies |
| 6 | Agony | 5 damage per curse on target, per tick |
| 6 | Mass Hex | All enemies 8 tiles: -15 all stats 20s |
| 7 | Doom Spiral | Target takes +10% damage per curse on them |
| 7 | Soul Shackle | Cannot heal above 50% HP 20s |
| 8 | Finger of Death | If target <20% HP and 3+ curses: instant kill |
| 8 | Witch Queen | Transform: Curses instant cast, -50% cost, 2x duration, 45s |

---

# 5. ENCHANTER

### *"The battlefield is my instrument"*

**Inspired By:** EQ Enchanter, DAoC Sorcerer
**Role:** Pure CC + Party support
**Core Mechanic:** CC chains + Haste buffs

## Design: ZERO Direct Damage

The Enchanter deals no damage. Power comes from:
- Best CC duration and variety
- Best party buffs (Haste)
- Mana manipulation
- Mind control

## CC Diminishing Returns

```csharp
// Track CC applications per target
// 1st: 100% duration
// 2nd: 50% duration
// 3rd: 25% duration
// 4th+: Immune 15s
public int GetCCStacks(Mobile target, CCType type);
```

## Spells (32 total)

### Single Target CC (Circle 1-5)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Daze | Stun 2s |
| 2 | Mesmerize | Sleep 12s (breaks on damage) |
| 2 | Root | Cannot move 6s |
| 3 | Blind | Miss 75% of attacks 8s |
| 3 | Slow | -50% move/attack speed 15s |
| 4 | Greater Mesmerize | Sleep 18s, 50% break resist |
| 4 | Charm Monster | Control creature 45s |
| 5 | Paralyze | Cannot act 6s |

### AoE CC (Circle 5-8)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 5 | Mass Daze | 8 tile AoE stun 2s |
| 5 | AoE Mesmerize | 8 tile AoE sleep 8s |
| 6 | Gravity Well | 8 tile AoE root 4s + pull to center |
| 6 | Mass Slow | 10 tile AoE -50% speed 12s |
| 7 | Mass Charm | Control up to 4 creatures 30s |
| 7 | Chaos Storm | 10 tile AoE confusion 12s |
| 8 | Time Stop | 12 tile AoE freeze 4s (affects you too) |
| 8 | Dominate | Control target player 6s (PvP) |

### Buffs (Circle 1-6)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Minor Haste | +15% speed 60s |
| 2 | Clarity | +10 INT 5 min |
| 2 | Alacrity | +10 DEX 5 min |
| 3 | Haste | +30% attack/cast speed 60s |
| 3 | Mana Shield | Damage drains mana instead of HP 30s |
| 4 | Rune Shield | Absorb 150 damage 5 min |
| 5 | Group Haste | Party +30% speed 60s |
| 6 | Celerity | +60% all speeds 20s |

### Mana Manipulation (Circle 3-8)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 3 | Mana Drain | Steal 25-40 mana |
| 4 | Mana Gift | Transfer 60 of your mana to ally |
| 5 | Mana Burn | Damage = 75% of target's mana, drain all |
| 6 | Group Clarity | Party +15 mana/tick 30s |
| 7 | Mind Blank | Target immune to CC 15s |
| 8 | Archmage | Transform: Spells cost 50% mana, CC +50% duration, 45s |


---

# 6. CHRONOMANCER (Oracle)

### *"Time is a river I can swim upstream"*

**Inspired By:** GW2 Chronomancer, FFXIV Astrologian
**Role:** Time support, survivability, utility
**Core Mechanic:** Time Wells + Rewind

## Time Echo System

```csharp
public class TimeEcho
{
    public Point3D Location { get; set; }
    public int HP { get; set; }
    public int Mana { get; set; }
    public int Stam { get; set; }
    public List<BuffInfo> Buffs { get; set; }
}
// Store echo every 5 seconds, keep last 3 (15s history)
public Queue<TimeEcho> TimeEchoes { get; set; }
```

## Time Well System

```csharp
public class TimeWell : Item
{
    public TimeWellType Type { get; set; }
    public int Radius { get; set; }
    public TimeSpan Duration { get; set; }
    // Wells affect all in radius each tick
}
```

| Well Type | Effect in Radius |
|-----------|------------------|
| Haste Well | +40% speed |
| Slow Well | -40% speed (enemies) |
| Healing Well | +5 HP/tick |
| Gravity Well | Pulls to center |

## Spells (32 total)

### Haste (Circle 1-4)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Quicken | +20% speed 20s |
| 2 | Alacrity | +30% attack/cast speed 30s |
| 2 | Blink | Instant teleport 8 tiles |
| 3 | Time Skip | Next spell is instant cast |
| 3 | Haste | +50% all speeds 15s |
| 4 | Haste Well | Place well: +40% speed in 5 tiles, 20s |
| 4 | Group Quicken | Party +30% speed 30s |

### Slow (Circle 2-5)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 2 | Temporal Slow | -40% speed 12s |
| 3 | Time Freeze | Stun 3s |
| 4 | Slow Well | Place well: enemies -40% speed, 20s |
| 4 | Entropy | -50% speed, -10 HP/tick 12s |
| 5 | Mass Slow | All enemies 10 tiles: -40% speed 10s |
| 5 | Gravity Well | Place well: pulls enemies to center |

### Rewind (Circle 4-8)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 4 | Recall | Teleport to your location from 10s ago |
| 5 | Temporal Shield | Remember HP; 4s later restore to remembered |
| 5 | Rewind Self | Return to full state from 10s ago |
| 6 | Rewind Ally | Return ally to state from 10s ago |
| 6 | Borrowed Time | If you die in 10s, auto-rewind |
| 7 | Mass Rewind | All allies return to state from 5s ago |
| 7 | Chrono Shift | Swap HP% and positions with target |
| 8 | Déjà Vu | Reset all ability cooldowns |

### Foresight (Circle 1-6)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Foresight | +30% dodge 10s |
| 2 | Prescience | +20 all resists 30s |
| 3 | Fate's Favor | Next attack against you auto-misses |
| 4 | Premonition | Party: next attack misses each |
| 5 | Inevitable | Your attacks cannot miss 15s |
| 6 | Time Dilation | Your buffs last 2x duration 30s |

### Ultimate (Circle 7-8)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 7 | Time Stop Field | 12 tile AoE: enemies frozen 5s |
| 7 | Age | Target: -2 all stats/tick for 15s |
| 8 | Paradox | Duplicate yourself for 15s (clone mirrors actions) |
| 8 | Chrono Lord | Transform: Immune CC, rewind costs 0, wells 2x effect, 45s |

---

# 7. MESMER (Illusionist)

### *"Is it real? Does it matter?"*

**Inspired By:** GW2 Mesmer, EQ Enchanter
**Role:** Burst damage through clones, confusion
**Core Mechanic:** Clone army + Shatter

## Clone System

```csharp
public class MesmerClone : BaseCreature
{
    public override int Body => m_Original.Body;
    public override int Hue => m_Original.Hue;
    public override string Name => m_Original.Name;
    public int CloneHP { get; set; } = 1; // Dies instantly
    // AI: Attack caster's target with weak attacks
}
public List<MesmerClone> ActiveClones { get; set; } // Max 3
```

## Shatter System

```csharp
// Destroy all clones for AoE effects at each clone location
public void Shatter(ShatterType type)
{
    int count = ActiveClones.Count;
    foreach (var clone in ActiveClones)
    {
        DoShatterEffect(clone.Location, type, count);
        clone.Kill(); // Plays death effect
    }
}
```

| Shatter | Effect Per Clone Location |
|---------|--------------------------|
| Mind Wrack | 18-28 psychic damage AoE |
| Cry of Pain | 4s confusion AoE |
| Diversion | 5s daze AoE |
| Distortion | Caster invuln 1.5s per clone |

## Spells (32 total)

### Clone Creation (Circle 1-5)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Mirror Image | Summon 1 clone at your location |
| 2 | Decoy | Summon clone + you invisible 4s |
| 3 | Echo | Summon 2 clones |
| 4 | Phantasm | Summon 1 durable clone (150 HP, real damage) |
| 5 | Legion | Summon 3 clones |
| 5 | Illusionary Warden | Summon clone that reflects projectiles |

### Shatter (Circle 3-7)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 3 | Mind Wrack | Shatter: 18-28 damage per clone |
| 4 | Cry of Pain | Shatter: 4s confusion per clone |
| 5 | Diversion | Shatter: 5s daze per clone |
| 6 | Distortion | Shatter: 1.5s invuln per clone |
| 7 | Mind Explosion | Shatter: 35-50 + stun 2s per clone |

### Misdirection (Circle 2-6)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 2 | Blur | 30% dodge 15s |
| 3 | Swap | Trade places with a clone |
| 3 | Beguile | Target attacks clone instead 10s |
| 4 | Veil | You + clones invisible 6s |
| 5 | Confuse | Target attacks allies 8s |
| 6 | Mirror Match | Target sees allies as enemies 10s |

### Psychic Damage (Circle 1-8)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Mind Spike | 5-10 psychic |
| 2 | Psychic Scream | Cone, 10-16 psychic + fear 3s |
| 3 | Mind Stab | 18-28 psychic, interrupts |
| 4 | Phantasmal Pain | 25-38 psychic + 6-10/tick 10s |
| 5 | Mind Blast | 6 tile AoE, 30-45 psychic |
| 6 | Nightmare | Sleep + 12-18 psychic/tick while asleep |
| 7 | Psychic Storm | 10 tile AoE, 48-72 psychic, confuse 5s |
| 8 | Mastermind | Transform: Max 6 clones, shatters 2x, clones 100 HP, 45s |

### Stealth (Circle 3-5)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 3 | Invisibility | Invisible 60s or until action |
| 4 | Portal Pair | Create entrance/exit portals, 30s |
| 5 | Mass Invis | Party invisible 15s |

---

# 8. ICE MAGE

### *"Winter's cold embrace"*

**Inspired By:** WoW Frost Mage
**Role:** Control + Execute DPS
**Core Mechanic:** Chill → Freeze → Shatter combo

## Chill Stacking

```csharp
public int GetChillStacks(Mobile target); // Max 5
// 1-2 stacks: -15% speed
// 3-4 stacks: -30% speed
// 5 stacks: FROZEN (stunned 4s, clears stacks)
// Frozen targets take +50% cold damage
```

## Shatter Mechanic

```csharp
// Attacking frozen target = Shatter
// +50% damage, breaks freeze early, AoE cold nova
```

## Spells (32 total)

### Chill Builders (Circle 1-4)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Frostbolt | 6-11 cold + 1 Chill stack |
| 1 | Frost Armor | +15 cold resist, attackers get 1 Chill, 60s |
| 2 | Ice Shard | 10-16 cold + 1 Chill |
| 2 | Chilling Wind | Cone 6 tiles, 8-14 cold + 1 Chill each |
| 3 | Frost Nova | AoE 5 tiles, 12-20 cold + 2 Chill each |
| 3 | Frostfire Bolt | 15-24 cold+fire + 1 Chill |
| 4 | Blizzard | 8 tile AoE, 6-10/tick, +1 Chill/tick, 8s |
| 4 | Ice Lance | 20-32 cold + 2 Chill (instant cast) |

### Freeze/Shatter (Circle 4-7)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 4 | Flash Freeze | Instant 5 Chill = Frozen |
| 5 | Deep Freeze | Target Frozen 5s (no Chill needed) |
| 5 | Shatter | If Frozen: 40-60 cold + AoE nova, consumes Freeze |
| 6 | Glacial Spike | 50-75 cold, if 5 Chill: Frozen + Shatter |
| 6 | Ice Tomb | Target Frozen, immune to damage 6s |
| 7 | Comet Storm | Rain 5 comets, each 25-40 cold + 1 Chill |

### Utility/Defense (Circle 2-6)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 2 | Frost Slick | 4x4 tile ice, -50% speed zone 15s |
| 3 | Ice Barrier | Absorb 80 damage 60s |
| 4 | Ice Wall | 8 tile ice wall blocking movement 20s |
| 4 | Cold Snap | Reset all Frost cooldowns |
| 5 | Ice Block | You immune + Frozen 8s (can cancel early) |
| 6 | Frozen Orb | Slow-moving orb, Frostbolts at nearby enemies |

### Execute/Ultimate (Circle 6-8)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 6 | Rime Reaper | 45-68 cold, +50% if target <25% HP |
| 7 | Glacial Tomb | 8 tile AoE, all Frozen 4s |
| 7 | Absolute Zero | 55-85 cold, target Frozen, -50 cold resist 15s |
| 8 | Ice Age | 15 tile AoE, 60-90 cold, perma-slow terrain, all Frozen |
| 8 | Avatar of Frost | Transform: Spells +2 Chill, Frozen lasts 2x, immune cold, 45s |

d | All summons attack target |
| 1 | Defend Command | All summons guard you |
| 2 | Frenzy | Summons +30% attack speed 15s |
| 3 | Bond | Bond with target summon |
| 4 | Share Pain | Damage split between you and summons 20s |
| 5 | Empower | Summon +50% damage 20s |

### Utility

| Cir | Spell | Effect |
|:---:|-------|--------|
| 2 | Mend Pet | Heal summon 50-80 HP |
| 3 | Dismiss | Unsummon creature |
| 4 | Rebond | Change bonded creature |
| 5 | Mass Mend | Heal all summons 30-50 HP |
| 6 | Sacrifice | Kill summon for 50% of its HP as heal to you |

### Ultimate

| Cir | Spell | Effect |
|:---:|-------|--------|
| 7 | Pack Leader | All summons +25% stats 60s |
| 7 | Call of the Wild | Summon 3 random creatures (3s each) |
| 8 | Avatar of Summoning | Transform: Unlimited slots, summons -50% damage taken, 45s |

---

# 11. DRUID

### *"I am one with nature"*

**Inspired By:** WoW Druid, D&D Druid
**Role:** Hybrid (form dependent)
**Core Mechanic:** Shapeshifting forms

## Form System

```csharp
public DruidForm ActiveForm { get; set; }
// Each form: Different body, abilities, role
// Human form: Full spellcasting
// Beast forms: Locked spells, new abilities
```

| Form | Body | Role | Restrictions |
|------|------|------|--------------|
| Human | Normal | Caster | Full spells |
| Bear | 0xD5 | Tank | Melee only, +armor |
| Cat | 0xD6 | Melee DPS | Stealth, bleeds |
| Moonkin | 0x3C | Caster DPS | Nature spells only |
| Tree | 0x723 | Healer | Heal spells only |
| Travel | 0xDC | Mobility | No combat |

## Form Abilities

### Bear Form (Tank)

| Ability | Effect |
|---------|--------|
| Maul | 25-40 physical |
| Swipe | 5 tile AoE 15-25 phys |
| Growl | Taunt single target |
| Frenzied Regen | +30 HP/tick 10s |
| Survival Instincts | -50% damage taken 12s |

### Cat Form (Melee DPS)

| Ability | Effect |
|---------|--------|
| Claw | 15-25 phys + combo point |
| Rake | 10-16 phys + bleed 8s + combo |
| Rip | Bleed (5 damage/tick × combo points) 12s |
| Ferocious Bite | 20 damage × combo points |
| Prowl | Stealth |
| Pounce | From stealth: stun 3s + combo |

### Moonkin Form (Caster DPS)

| Ability | Effect |
|---------|--------|
| Wrath | 15-25 nature |
| Starfire | 28-42 arcane (slow cast) |
| Moonfire | 12-18 arcane + 5-8/tick DoT |
| Starfall | AoE 10 tiles, 8-14 arcane/tick 10s |
| Solar Beam | Silence zone 6s |

### Tree Form (Healer)

| Ability | Effect |
|---------|--------|
| Lifebloom | HoT 6-10/tick, blooms for 25-40 at end |
| Rejuvenation | HoT 8-12/tick 12s |
| Swiftmend | Consume HoT for instant 40-60 heal |
| Wild Growth | AoE HoT 5 targets 6-10/tick 8s |
| Tranquility | Channel: AoE heal 15-25/tick 8s |

## Spells (32 total)

### Forms (Circle 1-6)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Bear Form | Tank mode |
| 2 | Cat Form | DPS mode |
| 3 | Travel Form | +100% speed, no combat |
| 4 | Moonkin Form | Caster DPS mode |
| 5 | Tree Form | Healer mode |
| 6 | Flight Form | Flying travel (outdoors) |

### Nature Spells (Human/Moonkin)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Wrath | 10-16 nature |
| 2 | Moonfire | 12-18 arcane + 5-8/tick 12s |
| 3 | Entangling Roots | Root 8s, 3-5 nature/tick |
| 3 | Starfire | 25-38 arcane |
| 4 | Nature's Grasp | Next melee attacker rooted 5s |
| 5 | Starfall | 10 tile AoE, 8-14/tick 10s |
| 6 | Hurricane | 10 tile AoE, 6-10/tick, -50% speed, 10s |
| 7 | Solar Beam | 8 tile line silence zone 6s |

### Healing (Human/Tree)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Rejuvenation | HoT 8-12/tick 12s |
| 2 | Healing Touch | 30-48 heal (slow cast) |
| 3 | Lifebloom | HoT 6-10/tick, blooms 25-40 |
| 4 | Swiftmend | Consume HoT for instant 40-60 |
| 5 | Wild Growth | AoE HoT 5 targets |
| 6 | Nature's Cure | Remove poison, curse, disease |
| 7 | Tranquility | Channel: AoE heal 15-25/tick |
| 8 | Rebirth | Resurrect with 50% HP (combat res) |

### Ultimate

| Cir | Spell | Effect |
|:---:|-------|--------|
| 8 | Incarnation | Enhance current form 30s: Bear +100% armor, Cat +100% crit, Moonkin +50% damage, Tree +50% healing |

---

# 12. BARD

### *"My song shapes the battlefield"*

**Inspired By:** FFXIV Bard, EQ Bard, DAoC Minstrel
**Role:** Party support + Enemy debuffs
**Core Mechanic:** Songs (channeled auras) + Crescendo

## Song System

```csharp
public class BardSong
{
    public bool IsActive { get; set; }
    public Timer TickTimer { get; set; }
    public int ManaCostPerTick { get; set; }
    public int Radius { get; set; } = 10;
}
// Only ONE song active at a time
// Songs tick every 3s with effect
// Moving is allowed, casting other spells ends song
```

## Crescendo System

```csharp
// Certain finishers scale with song duration
// Longer song = bigger finish
public int SongTicks { get; set; } // Tracks ticks since song started
// Crescendo damage = base + (SongTicks × 5)
```

## Spells (32 total)

### Buff Songs (Circle 1-5)

| Cir | Spell | Effect While Singing |
|:---:|-------|---------------------|
| 1 | Song of Speed | Party +20% speed |
| 2 | Song of Might | Party +15% damage |
| 2 | Song of Shielding | Party +15 all resists |
| 3 | Song of Haste | Party +25% attack speed |
| 3 | Song of Regeneration | Party +5 HP/tick |
| 4 | Song of Mana | Party +8 mana/tick |
| 5 | Song of War | Party +25% damage, +20 armor |
| 5 | Song of the Aegis | Party -20% damage taken |

### Debuff Songs (Circle 2-6)

| Cir | Spell | Effect on Enemies |
|:---:|-------|-------------------|
| 2 | Dirge of Weakness | -15% damage dealt |
| 3 | Dirge of Slowness | -25% speed |
| 3 | Dirge of Pain | 4-7 sonic/tick |
| 4 | Dirge of Confusion | 20% chance/tick: attack ally |
| 5 | Dirge of Terror | 25% chance/tick: flee |
| 6 | Dirge of Doom | -25% all stats |

### Instant Melodies (Circle 1-6)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Dissonant Note | 5-10 sonic damage |
| 2 | Piercing Shriek | 12-18 sonic + interrupt |
| 3 | Sonic Blast | Cone, 15-25 sonic + deaf 6s |
| 4 | Shatter Note | 25-40 sonic, +50% vs shielded |
| 5 | Thunder Clap | 8 tile AoE, 30-45 sonic, stun 2s |
| 6 | Wail of Doom | 10 tile AoE, 40-55 sonic, fear 4s |

### Crescendo Finishers (Circle 4-8)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 4 | Crescendo Strike | 15 + (5 × SongTicks) sonic damage |
| 5 | Crescendo Blast | AoE 6 tiles, 12 + (4 × SongTicks) sonic |
| 6 | Crescendo Heal | Party heal 10 + (5 × SongTicks) HP |
| 7 | Crescendo Finale | AoE 10 tiles, 20 + (6 × SongTicks) sonic + stun |
| 8 | Magnum Opus | All party buffs maxed, all enemies debuffed, ends song |

### Ultimate

| Cir | Spell | Effect |
|:---:|-------|--------|
| 7 | Dual Song | Sing 2 songs simultaneously 30s |
| 7 | Anthem of Heroes | Party immune to fear/charm 20s |
| 8 | Maestro Ascension | Transform: Songs cost 0 mana, 2x radius, can cast during song, 45s |


---

# Part Four: Martial Classes (14 Classes)

---

# 1. PALADIN

### *"My faith is my shield, my sword is justice"*

**Inspired By:** UO Chivalry, D&D Paladin, WoW Paladin
**Role:** Holy tank/support hybrid
**Core Mechanic:** Tithing + Virtue system

## Tithing System

```csharp
public int TithingPoints { get; set; } // Max 100,000
// Generate: Tithe gold at shrines (1 gold = 1 point)
// Spend: All abilities cost tithing points
// Encourages gold sink, religious roleplay
```

## Virtue System

```csharp
public Dictionary<Virtue, int> VirtueStacks { get; set; } // Max 3 each

// Virtues build through actions:
// Valor: Damage dealt to evil creatures
// Compassion: Healing allies
// Justice: Killing murderers/criminals
// Sacrifice: Taking damage for allies

// At 3 stacks, can consume for powerful effect
```

| Virtue | Builds From | Consume Effect |
|--------|-------------|----------------|
| Valor | Damage to undead/demons | Next attack +100% damage |
| Compassion | Healing others | Instant full heal to ally |
| Justice | Killing reds/criminals | 10s immunity to all damage |
| Sacrifice | Damage taken for others | Party +50% damage 15s |

## Spells (32 total)

### Holy Strikes (Circle 1-4)

| Cir | Spell | Tithe | Effect |
|:---:|-------|:-----:|--------|
| 1 | Crusader Strike | 50 | 12-18 holy damage |
| 1 | Judgment of Light | 75 | 10-15 holy + heals you 50% |
| 2 | Holy Smite | 100 | 18-28 holy, +50% vs undead |
| 2 | Seal of Righteousness | 50 | Next 5 attacks +8 holy damage |
| 3 | Divine Strike | 150 | 25-38 holy + 3s stun vs evil |
| 3 | Hammer of Justice | 200 | 20-30 holy + 4s stun |
| 4 | Exorcism | 300 | 35-52 holy, instant kill demons <25% HP |
| 4 | Blade of Light | 250 | 30-45 holy, pierces armor |

### Defensive (Circle 2-6)

| Cir | Spell | Tithe | Effect |
|:---:|-------|:-----:|--------|
| 2 | Divine Protection | 150 | -50% damage taken 10s |
| 3 | Holy Shield | 200 | +30 all resists, +20% block 30s |
| 3 | Cleanse | 100 | Remove poison, curse, bleed from target |
| 4 | Blessing of Protection | 300 | Target immune to physical 8s |
| 5 | Divine Shield | 500 | You immune to all damage 6s, can't attack |
| 5 | Lay on Hands | 1000 | Instant full heal, 30 min cooldown |
| 6 | Guardian Angel | 800 | If you die in 30s, resurrect at 50% HP |

### Support (Circle 1-5)

| Cir | Spell | Tithe | Effect |
|:---:|-------|:-----:|--------|
| 1 | Blessing of Might | 100 | Target +15% damage 5 min |
| 1 | Blessing of Wisdom | 100 | Target +10 mana/tick 5 min |
| 2 | Blessing of Kings | 200 | Target +10% all stats 5 min |
| 3 | Holy Light | 150 | Heal 30-48 HP |
| 4 | Flash of Light | 100 | Instant heal 15-25 HP |
| 4 | Consecration | 300 | 6 tile AoE, 8-14 holy/tick, heals allies 5-8/tick, 12s |
| 5 | Aura of Devotion | 400 | Party +20 all resists 60s |

### Ultimate (Circle 6-8)

| Cir | Spell | Tithe | Effect |
|:---:|-------|:-----:|--------|
| 6 | Turn Undead | 500 | All undead 10 tiles: fear 8s or instant kill <20% HP |
| 6 | Holy Wrath | 600 | 8 tile AoE, 40-60 holy, +100% vs undead/demons |
| 7 | Avenging Wrath | 800 | Transform: +50% damage, +50% healing, wings, 20s |
| 7 | Divine Intervention | 1500 | Target ally: immune all damage 10s, massive heal |
| 8 | Avatar of Light | 2000 | Transform: Immune to evil damage, abilities cost 0, radiant aura damages undead, 30s |
| 8 | Resurrection | 2500 | Resurrect dead player with 75% HP/mana |

---

# 2. TEMPLAR

### *"Judgment comes for the unworthy"*

**Inspired By:** Diablo Crusader, Warhammer Witch Hunter
**Role:** Holy burst DPS, anti-caster
**Core Mechanic:** Zeal stacks + Judgment execution

## Zeal System

```csharp
public int ZealStacks { get; set; } // Max 10
// Generate: +1 per hit landed, +2 on crit, +3 on kill
// Decay: -1 every 5s out of combat
// Spend: Empower finishers
```

## Judgment Mechanic

```csharp
// Mark target for Judgment
// While marked: you deal +15% damage to them
// Judgment finisher: Damage scales with target's missing HP
// Execute threshold: Instant kill <15% HP if 10 Zeal
```

## Spells (32 total)

### Zeal Builders (Circle 1-3)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Zealous Strike | 10-15 holy + 1 Zeal |
| 1 | Punish | 12-18 phys + 1 Zeal, +2 if target is caster |
| 2 | Smite | 15-24 holy + 1 Zeal, interrupts |
| 2 | Fervent Blade | 3 rapid strikes, 6-10 each, +1 Zeal per hit |
| 3 | Righteous Fury | Next 5 attacks +1 Zeal each, +20% damage |
| 3 | Burning Brand | 18-28 fire + 5-8/tick 10s + 1 Zeal/tick |

### Judgment (Circle 3-6)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 3 | Mark for Judgment | Mark target 30s, +15% damage from you |
| 4 | Judgment Strike | 25-38 holy, +5% per Zeal stack |
| 5 | Execution Sentence | 40-60 holy, if <25% HP: +100% damage |
| 5 | Final Judgment | Consume all Zeal: 10 + (8 × Zeal) holy damage |
| 6 | Verdict | If marked target <15% HP and 10 Zeal: instant kill |
| 6 | Mass Judgment | Mark all enemies 8 tiles, 20-30 holy each |

### Anti-Magic (Circle 2-5)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 2 | Rebuke | 12-18 holy, silence 3s |
| 3 | Purge Magic | Dispel 2 buffs from target |
| 3 | Spell Breaker | 20-32 holy, +100% damage vs mana shields |
| 4 | Inquisitor's Brand | Target: spells cost +50% mana 15s |
| 4 | Censure | 8 tile AoE silence zone 5s |
| 5 | Null Field | 6 tile AoE: no magic can be cast 8s |

### Aggression (Circle 4-7)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 4 | Charge | Rush 10 tiles to target, 20-30 phys, stun 2s, +2 Zeal |
| 5 | Frenzy | +50% attack speed, +2 Zeal per hit, 12s |
| 5 | Wrathful Smash | 35-52 holy + knockback 4 tiles |
| 6 | Rampage | Each kill extends Frenzy by 3s |
| 7 | Unstoppable Zeal | Immune to CC, +30% speed, 10s |

### Ultimate (Circle 7-8)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 7 | Inquisition | All enemies 12 tiles: marked, -25% damage dealt 20s |
| 7 | Fanatic's Rage | Zeal doesn't decay, +2 per hit, 30s |
| 8 | Wrath of the Righteous | Transform: Attacks generate 3 Zeal, Judgment abilities +100% damage, immune silence, 30s |
| 8 | Final Reckoning | All marked targets: take 60-90 holy + Verdict check |

---

# 3. KNIGHT

### *"None shall pass while I stand"*

**Inspired By:** WoW Protection Warrior, FFXIV Paladin tank
**Role:** Pure tank, party protector
**Core Mechanic:** Shield mechanics + Rally system

## Shield System

```csharp
public int BlockValue { get; set; } // From shield + skills
public int BlockChance { get; set; } // Base 25% with shield

// Successful blocks:
// - Negate damage up to BlockValue
// - Generate 1 Fortitude
// - Trigger block-reactive abilities
```

## Fortitude System

```csharp
public int Fortitude { get; set; } // Max 10
// Generate: Blocking attacks, taking damage, taunting
// Spend: Defensive cooldowns, party protection
// Decay: -1 every 10s out of combat
```

## Rally System

```csharp
// Rally abilities buff nearby allies
// Rally range: 8 tiles
// Rallied allies get Knight's passive: +10 armor
```

## Spells (32 total)

### Shield Attacks (Circle 1-4)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Shield Bash | 8-14 phys + 2s daze |
| 1 | Shield Slam | 12-18 phys, +50% if you blocked last 3s |
| 2 | Punishing Strike | 15-22 phys + 1 Fortitude |
| 2 | Shield Charge | Rush 6 tiles, 18-28 phys, stun 2s |
| 3 | Concussive Blow | 20-30 phys + 4s stun, costs 2 Fortitude |
| 3 | Riposte | After block: instant 25-38 phys |
| 4 | Shield Crush | Cone 4 tiles, 22-35 phys, -20% damage dealt 10s |
| 4 | Battering Ram | 30-45 phys, knockback 5 tiles |

### Tanking (Circle 1-5)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Taunt | Force target to attack you 6s |
| 2 | Defensive Stance | -20% damage dealt, +30% block, +20 armor |
| 2 | Shield Wall | +50 block value, +25% block chance 15s |
| 3 | Mass Taunt | All enemies 6 tiles: attack you 6s |
| 3 | Iron Skin | -30% all damage taken 12s, costs 3 Fortitude |
| 4 | Last Stand | +50% max HP for 15s, costs 5 Fortitude |
| 5 | Fortress | -75% damage taken, cannot move, 8s |
| 5 | Unbreakable | Cannot drop below 1 HP for 6s, costs 5 Fortitude |

### Party Protection (Circle 3-7)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 3 | Intercept | Rush to ally, take next hit for them |
| 4 | Guardian's Oath | Target ally: 50% of damage they take hits you instead, 20s |
| 4 | Rally | Party 8 tiles: +15 armor, +10% HP 30s |
| 5 | Shield Ally | Target ally: your block value shields them 15s |
| 5 | Inspiring Presence | Party 8 tiles: +10% damage dealt 30s |
| 6 | Phalanx | Party 6 tiles: share your block chance 15s |
| 6 | Hold the Line | 8 tile AoE: enemies cannot pass through 10s |
| 7 | Rallying Cry | Party 10 tiles: heal 20-35 HP, +20 all resists 20s |

### Ultimate (Circle 7-8)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 7 | Champion's Bulwark | Block chance 100%, block value +100 20s |
| 7 | Undying Loyalty | If ally would die 8 tiles: they survive at 1 HP, damage transfers to you |
| 8 | Avatar of the Shield | Transform: Cannot die, all damage to party redirects to you, 15s |
| 8 | Bastion | 12 tile AoE: all allies -50% damage taken, you +200 armor, 20s |


---

# 4. FIGHTER

### *"Every weapon is an extension of my will"*

**Inspired By:** D&D Fighter, UO Weapon Skills
**Role:** Pure melee DPS, weapon specialist
**Core Mechanic:** Weapon Mastery + Combat Stances

## Weapon Mastery System

```csharp
public WeaponType MasteredWeapon { get; set; }
// Mastery: +25% damage, unique ability unlocked per weapon type
// Can switch mastery at trainer (not in combat)
```

| Weapon Type | Mastery Bonus | Unique Ability |
|-------------|---------------|----------------|
| Sword | +10% crit chance | Vorpal Strike (execute) |
| Axe | +15% damage | Cleave (hits 3 targets) |
| Mace | +20% armor pierce | Skull Crack (stun) |
| Polearm | +2 tile range | Impale (root + bleed) |
| Fencing | +15% parry | Riposte (counter-attack) |

## Combat Stance System

```csharp
public CombatStance ActiveStance { get; set; }
// Switch: 1s cooldown, instant
// Each stance modifies base attacks
```

| Stance | Passive | Role |
|--------|---------|------|
| Aggressive | +30% damage, -15% defense | Burst |
| Defensive | +30% parry, -15% damage | Survival |
| Balanced | +15% damage, +15% parry | Default |
| Berserker | +50% attack speed, +20% damage taken | All-in |

## Spells (32 total)

### Core Strikes (Circle 1-3)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Strike | 12-18 phys |
| 1 | Quick Strike | 8-12 phys, instant, low cooldown |
| 2 | Power Attack | 22-32 phys, slow |
| 2 | Double Strike | Two hits, 10-15 each |
| 3 | Rending Strike | 18-28 phys + bleed 5-8/tick 12s |
| 3 | Precise Strike | 20-30 phys, +30% crit chance |

### Weapon Specials (Circle 3-6)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 3 | Cleave | Hit all enemies 2 tiles, 15-25 phys |
| 4 | Whirlwind | 360° attack, 6 tiles, 20-32 phys |
| 4 | Mortal Strike | 28-42 phys, -50% healing 10s |
| 5 | Execute | 35-52 phys, +100% damage if <25% HP |
| 5 | Disarm | 20-30 phys, target cannot use weapon abilities 6s |
| 6 | Decapitate | 45-68 phys, instant kill if <10% HP |

### Mobility (Circle 2-5)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 2 | Charge | Rush 8 tiles, +25% damage on next attack |
| 3 | Leap | Jump 6 tiles, AoE 15-25 phys on landing |
| 4 | Hamstring | 18-28 phys, -50% speed 8s |
| 4 | Pursuit | Rush to target, immune to roots/slows during |
| 5 | Blade Storm | Spin while moving, 10-16 phys/tick to adjacent 6s |

### Combat Mastery (Circle 4-7)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 4 | Battle Cry | +20% damage, +20% attack speed 15s |
| 5 | Second Wind | Heal 25% max HP, remove slows/roots |
| 5 | Overpower | After parry: instant 30-45 phys, guaranteed crit |
| 6 | Flurry | 5 rapid attacks, 12-18 each |
| 6 | Colossus Smash | 40-60 phys, target -30% armor 15s |
| 7 | Recklessness | +100% crit chance, +50% crit damage, +30% damage taken, 12s |

### Ultimate (Circle 7-8)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 7 | Bladestorm | Channel: Spin in place, 25-38 phys/tick to all 5 tiles, immune CC, 6s |
| 7 | Warmaster | All weapon abilities -50% cooldown 30s |
| 8 | Avatar of War | Transform: +75% damage, +50% attack speed, abilities cost 0 stamina, 30s |
| 8 | Deathblow | 80-120 phys, if kills: reset all cooldowns, refund all stamina |

---

# 5. BARBARIAN

### *"My rage knows no bounds"*

**Inspired By:** Diablo Barbarian, WoW Fury Warrior
**Role:** Melee burst, berserker
**Core Mechanic:** Fury buildup + Rage transformation

## Fury System

```csharp
public int Fury { get; set; } // Max 100
// Generate: +5 per hit dealt, +10 per hit taken, +20 on kill
// Decay: -5 per second out of combat
// At 100: Can activate Rage
```

## Rage Transformation

```csharp
public bool IsEnraged { get; set; }
// Duration: Until Fury depletes (drains 5/sec)
// While Enraged:
// - +50% damage dealt
// - +25% attack speed
// - Immune to CC
// - Cannot use non-Rage abilities
// - New abilities unlocked
```

## Spells (32 total)

### Fury Builders (Circle 1-3)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Bash | 10-16 phys + 5 Fury |
| 1 | Frenzy | 8-12 phys + 5 Fury, +5% attack speed stacking 5x |
| 2 | Cleave | 14-22 phys to 3 targets + 5 Fury per hit |
| 2 | Wild Strike | 16-26 phys + 8 Fury, random bonus effect |
| 3 | Rend | 12-18 phys + bleed 6-10/tick 12s + 5 Fury |
| 3 | Bloodthirst | 20-30 phys + heal 50% damage dealt + 10 Fury |

### Fury Spenders (Circle 3-6)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 3 | Rampage | 25-38 phys, costs 20 Fury |
| 4 | Execute | 30-45 phys, +5% damage per 1% target missing HP, costs 30 Fury |
| 4 | Slam | 6 tile AoE, 22-35 phys, costs 25 Fury |
| 5 | Furious Charge | Rush 12 tiles, 35-52 phys, stun 2s, costs 25 Fury |
| 5 | Earthquake | 8 tile AoE, 40-60 phys, stun 3s, costs 40 Fury |
| 6 | Wrath of the Berserker | Instantly gain 50 Fury |

### Survival (Circle 2-5)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 2 | War Cry | +20% armor, +10 Fury, 20s |
| 3 | Ignore Pain | -50% damage taken 6s |
| 4 | Bloodbath | Heal 3% max HP per enemy hit for 10s |
| 4 | Threatening Shout | Enemies 8 tiles: -25% damage dealt 12s |
| 5 | Undying Rage | If you would die: survive at 1 HP, +100 Fury |

### Rage Abilities (Only usable while Enraged)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 4 | Savage Blow | 35-52 phys, drains 10 Fury |
| 5 | Annihilate | 45-68 phys, drains 15 Fury |
| 6 | Carnage | 8 tile AoE, 38-58 phys, drains 20 Fury |
| 7 | Primal Scream | 10 tile AoE fear 6s, drains 25 Fury |
| 7 | Titan Grip | Dual wield any weapons, +100% damage 15s |

### Ultimate (Circle 6-8)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 6 | Rage | Enter Rage transformation (requires 100 Fury) |
| 7 | Eternal Rage | Fury drains 50% slower during Rage |
| 8 | Avatar of Carnage | Transform: Fury doesn't drain, all abilities usable, +100% damage, attacks cleave, 30s |
| 8 | Cataclysm | 15 tile AoE, 70-100 phys, all enemies knocked down 4s, drains all Fury |

---

# 6. MONK

### *"Body and spirit in perfect harmony"*

**Inspired By:** WoW Monk, D&D Monk, Diablo Monk
**Role:** Combo-based melee DPS, mobile
**Core Mechanic:** Chi + Combo system

## Chi System

```csharp
public int Chi { get; set; } // Max 5
// Generate: Specific builders give +1 Chi
// Spend: Finishers consume Chi for increased effect
// No decay, persists until spent
```

## Combo System

```csharp
public Queue<ComboMove> ComboQueue { get; set; } // Last 3 moves
// Certain sequences = bonus effects
// Example: Jab → Jab → Tiger Palm = Combo: Double damage
```

| Combo Sequence | Bonus Effect |
|----------------|--------------|
| Jab → Jab → Tiger Palm | +100% Tiger Palm damage |
| Tiger Palm → Blackout Kick → Rising Sun Kick | AoE version |
| Any 3 different moves | +1 Chi |
| Same move 3x | -50% cost on 3rd |

## Spells (32 total)

### Chi Builders (Circle 1-3)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Jab | 8-12 phys + 1 Chi |
| 1 | Tiger Palm | 12-18 phys + 1 Chi, ignores 30% armor |
| 2 | Blackout Kick | 15-24 phys + 1 Chi |
| 2 | Rising Sun Kick | 18-28 phys + 1 Chi, -10% damage dealt debuff |
| 3 | Spinning Crane Kick | AoE 4 tiles, 10-16 phys + 1 Chi per target hit |
| 3 | Chi Wave | 20-32 phys, bounces to 3 targets, +1 Chi |

### Chi Spenders (Circle 3-6)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 3 | Fists of Fury | Channel: 5 rapid punches, 8-14 each, costs 3 Chi |
| 4 | Strike of the Windlord | Cone 5 tiles, 28-42 phys, costs 2 Chi |
| 4 | Touch of Death | If target <15% HP: instant kill, costs 5 Chi |
| 5 | Whirling Dragon Punch | 6 tile AoE, 35-52 phys, costs 3 Chi |
| 5 | Chi Burst | Line 10 tiles, 30-45 phys, heals allies, costs 2 Chi |
| 6 | Quaking Palm | Stun 5s, costs 4 Chi |

### Mobility (Circle 2-5)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 2 | Roll | Dash 6 tiles, breaks roots |
| 2 | Flying Serpent Kick | Leap 10 tiles to target, 15-25 phys |
| 3 | Transcendence | Mark your location |
| 3 | Transcendence: Transfer | Return to marked location |
| 4 | Tiger's Lust | +70% speed, immune slows 6s |
| 5 | Storm, Earth, Fire | Split into 3 spirits, each copy attacks |

### Inner Focus (Circle 2-6)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 2 | Expel Harm | Heal 20-30 HP + 1 Chi |
| 3 | Fortifying Brew | +20% max HP, -20% damage taken 15s |
| 4 | Dampen Harm | Next 3 hits -50% damage |
| 4 | Diffuse Magic | -60% magic damage, reflect DoTs, 6s |
| 5 | Touch of Karma | Redirect 100% damage taken to attacker 6s |
| 6 | Serenity | All abilities cost 0 Chi, cooldowns reset, 10s |

### Ultimate (Circle 7-8)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 7 | Way of the Crane | Attacks heal nearby ally for 50% damage dealt 15s |
| 7 | Invoke Xuen | Summon White Tiger spirit (600 HP, 25-40 phys) 30s |
| 8 | Avatar of the Fist | Transform: Chi max 10, abilities generate 2x Chi, +50% damage, 30s |
| 8 | Seven Sided Strike | Hit target 7 times instantly, 18-28 each, invulnerable during |


---

# 7. ROGUE

### *"You never see me coming"*

**Inspired By:** WoW Rogue, UO Stealth, Classic Thief
**Role:** Stealth burst, utility
**Core Mechanic:** Stealth + Combo Points

## Stealth System

```csharp
public bool IsStealthed { get; set; }
// Enter: 2s channel, breaks on damage
// Exit: Attacking, taking damage, certain actions
// Bonus: Attacks from stealth deal +50% damage, enable openers
```

## Combo Point System

```csharp
public int ComboPoints { get; set; } // Max 5, per target
// Generate: Most attacks give +1
// Spend: Finishers scale with points
// Reset: On target death, on finisher
```

## Spells (32 total)

### Openers (Require Stealth)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Cheap Shot | Stun 4s + 2 Combo |
| 2 | Garrote | Silence 3s + bleed 6-10/tick 12s + 1 Combo |
| 3 | Ambush | 35-52 phys + 2 Combo |
| 4 | Shadowstrike | Teleport behind + 40-60 phys + 2 Combo |
| 5 | Sap | Incapacitate 30s (breaks on damage), no combat |

### Combo Builders (Circle 1-4)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Sinister Strike | 10-16 phys + 1 Combo |
| 1 | Backstab | 18-28 phys from behind + 1 Combo |
| 2 | Hemorrhage | 12-18 phys + bleed + 1 Combo |
| 2 | Gouge | 10-15 phys + incapacitate 4s + 1 Combo |
| 3 | Mutilate | 20-30 phys + 2 Combo (requires daggers) |
| 4 | Fan of Knives | AoE 6 tiles, 12-18 phys + 1 Combo per target |

### Finishers (Circle 2-6)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 2 | Eviscerate | 8 + (12 × Combo) phys |
| 3 | Rupture | Bleed (3 × Combo)/tick for (6 + 3×Combo)s |
| 3 | Kidney Shot | Stun (1 × Combo)s |
| 4 | Slice and Dice | +10% attack speed per Combo 20s |
| 5 | Envenom | 15 + (15 × Combo) poison + -20% healing |
| 6 | Death from Above | Leap + 20 + (18 × Combo) phys AoE |

### Utility (Circle 2-6)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 2 | Stealth | Enter stealth (2s channel) |
| 2 | Sprint | +70% speed 8s |
| 3 | Evasion | +75% dodge 10s |
| 3 | Kick | Interrupt + 4s silence |
| 4 | Vanish | Instant stealth, breaks roots/snares |
| 4 | Blind | Target blinded 10s (breaks on damage) |
| 5 | Cloak of Shadows | Immune magic 5s, purge DoTs |
| 6 | Smoke Bomb | 6 tile AoE: enemies cannot target into/out of 8s |

### Ultimate (Circle 7-8)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 7 | Shadow Dance | Use stealth abilities without stealth 8s |
| 7 | Vendetta | +30% damage to target, can see through stealth/invis 20s |
| 8 | Avatar of Shadows | Transform: Permanent stealth, attacks don't break it, +100% opener damage, 20s |
| 8 | Deathmark | Combo finishers deal 2x damage, kills reset all cooldowns 15s |

---

# 8. RANGER

### *"The wilderness is my domain"*

**Inspired By:** D&D Ranger, GW2 Ranger, UO Archer
**Role:** Ranged DPS, tracking, traps
**Core Mechanic:** Focus + Terrain Mastery

## Focus System

```csharp
public int Focus { get; set; } // Max 100
// Generate: Standing still (+10/sec), successful hits (+5)
// Spend: Empowered shots, traps
// Decay: -5/sec while moving
```

## Terrain Mastery

```csharp
public TerrainType MasteredTerrain { get; set; }
// While in mastered terrain: +20% damage, +20% speed, stealth bonus
// Terrains: Forest, Desert, Snow, Swamp, Mountains
```

## Spells (32 total)

### Archery (Circle 1-4)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Steady Shot | 10-16 phys, +5 Focus |
| 1 | Arcane Shot | 12-18 arcane |
| 2 | Aimed Shot | 20-32 phys, 2s cast, costs 20 Focus |
| 2 | Multi-Shot | Hit 3 targets, 10-16 each |
| 3 | Rapid Fire | 5 shots, 8-12 each |
| 3 | Piercing Shot | 25-38 phys, ignores armor |
| 4 | Kill Shot | 35-52 phys, +100% if <25% HP |
| 4 | Barrage | 10 random shots in area, 12-18 each |

### Traps (Circle 2-6)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 2 | Freezing Trap | First enemy: frozen 6s |
| 2 | Explosive Trap | 6 tile AoE, 20-32 fire |
| 3 | Tar Trap | 6 tile zone, -60% speed 15s |
| 3 | Snake Trap | Releases 3 snakes on trigger |
| 4 | Ice Trap | 8 tile zone, frozen ground 20s |
| 5 | Immolation Trap | 6 tile AoE burn, 10-16 fire/tick 10s |
| 6 | Binding Shot | Tether target to location 10s, stun if break |

### Tracking & Survival (Circle 1-5)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Track | Reveal all enemies/creatures on minimap 30s |
| 2 | Hunter's Mark | Target takes +15% damage from you, visible through stealth |
| 3 | Camouflage | Stealth while stationary in mastered terrain |
| 3 | Disengage | Leap back 8 tiles, immune snares during |
| 4 | Aspect of the Cheetah | +50% speed, ends if hit |
| 4 | Mend Pet | Heal companion 50-80 HP |
| 5 | Feign Death | Appear dead, drop threat, 30s or until move |

### Animal Companion (Circle 3-7)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 3 | Call Companion | Summon wolf/cat/bear (300 HP, 15-25 phys) |
| 4 | Kill Command | Companion attacks for 30-45 phys |
| 5 | Bestial Wrath | Companion +100% damage, immune CC 10s |
| 6 | Pack Alpha | Companion +50% stats, you +25% stats near companion |
| 7 | Spirit Bond | Share HP pool with companion |

### Ultimate (Circle 7-8)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 7 | Volley | 10 tile AoE, 15 arrows, 12-18 each, costs 50 Focus |
| 7 | True Sight | See all stealthed/invisible, +50% crit 20s |
| 8 | Avatar of the Hunt | Transform: Focus doesn't decay, all shots pierce, +75% damage, companion +100% stats, 30s |
| 8 | Black Arrow | 60-90 phys, if kills: raise target as undead ally 60s |

---

# 9. BOUNTY HUNTER

### *"No one escapes my pursuit"*

**Inspired By:** Mandalorian, The Witcher, Darkest Dungeon Bounty Hunter
**Role:** Single-target specialist, anti-player
**Core Mechanic:** Marks + Pursuit stacks

## Mark System

```csharp
public Mobile MarkedTarget { get; set; }
// Mark: Lasts until target dies or you mark another
// While marked: +25% damage to target, track through world
// Only one mark active
```

## Pursuit System

```csharp
public int PursuitStacks { get; set; } // Max 10, on marked target only
// Generate: +1 per hit on marked target, +2 on crit
// Bonus: Each stack = +3% damage to marked target
// Spend: Execution abilities
```

## Spells (32 total)

### Marking & Tracking (Circle 1-4)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Mark Quarry | Mark target, +25% damage from you |
| 1 | Track Quarry | See marked target's location on map |
| 2 | Study Weakness | +2 Pursuit, learn target's resists |
| 2 | Relentless | Cannot be slowed while moving toward mark |
| 3 | Hunter's Insight | See marked target's HP/mana/cooldowns |
| 3 | Nowhere to Hide | Mark visible through stealth/invis |
| 4 | Predator's Sense | Teleport to mark if within 20 tiles |

### Pursuit Builders (Circle 1-4)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Tracking Strike | 10-16 phys + 1 Pursuit |
| 2 | Crippling Shot | 15-24 phys + -30% speed + 1 Pursuit |
| 2 | Hamstring | 12-18 phys + root 3s + 1 Pursuit |
| 3 | Grappling Hook | Pull target 6 tiles + 1 Pursuit |
| 3 | Bola | 18-28 phys + root 4s + 2 Pursuit |
| 4 | Flashbang | Blind 6s + 2 Pursuit |

### Pursuit Spenders (Circle 4-7)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 4 | Exploit Weakness | 20 + (6 × Pursuit) phys, costs 5 Pursuit |
| 5 | Collect Bounty | 30 + (10 × Pursuit) phys, costs 10 Pursuit |
| 5 | Finish the Job | If <20% HP: instant kill, costs 10 Pursuit |
| 6 | No Escape | Mark cannot use movement abilities 10s, costs 5 Pursuit |
| 6 | Inevitable End | +5% damage per Pursuit stack 15s |
| 7 | Dead or Alive | 50 + (8 × Pursuit) phys, heal 50% damage dealt |

### Utility (Circle 2-6)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 2 | Caltrops | 6 tile zone, -40% speed + 5-8 bleed |
| 3 | Net | Root target 5s |
| 3 | Smoke Bomb | Blind all 6 tiles 4s, you stealth |
| 4 | Assassinate | From stealth: 35-52 phys + 3 Pursuit |
| 5 | Preparation | Reset all cooldowns, +50 stamina |
| 6 | Contract | Marked target: -25% damage dealt, -25% healing |

### Ultimate (Circle 7-8)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 7 | Nowhere to Run | Mark cannot leave 15 tile radius of you 20s |
| 7 | Professional | Pursuit generation doubled, mark switching instant 30s |
| 8 | Avatar of the Hunt | Transform: Mark takes 2x damage from all sources, you +50% damage, teleport to mark free, 30s |
| 8 | Claim Bounty | If marked target dies: gain 1000 gold + full heal + reset all cooldowns |


---

# 10. BEASTMASTER

### *"My pack fights as one"*

**Inspired By:** WoW Hunter BM, D&D Beastmaster, Pokemon
**Role:** Multi-pet commander
**Core Mechanic:** Pack Bonds + Coordination

## Pack System

```csharp
public List<TamedBeast> Pack { get; set; } // Max 3 active
public TamedBeast AlphaBond { get; set; } // One bonded beast
// Pack members are permanent tames, not temporary summons
// Alpha: +50% stats, shares your resists, telepathic command
```

## Beast Types

| Beast | HP | Damage | Special |
|-------|:--:|--------|---------|
| Wolf | 200 | 12-18 | Pack Tactics (+10% per wolf) |
| Bear | 400 | 15-22 | Taunt, high armor |
| Cat | 180 | 18-28 | Prowl, high crit |
| Boar | 250 | 14-20 | Charge stun |
| Spider | 150 | 10-16 | Web, poison |
| Raptor | 220 | 20-30 | Bleed, high damage |
| Gorilla | 350 | 16-24 | Intimidate, pummel |
| Serpent | 120 | 8-14 | Deadly poison |

## Spells (32 total)

### Taming (Circle 1-4)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Tame Beast | Tame creature (10s channel) |
| 2 | Stable | Send beast to stable (max 10 stabled) |
| 2 | Call from Stable | Summon stabled beast |
| 3 | Alpha Bond | Bond with beast (+50% stats, telepathy) |
| 3 | Release | Release beast to wild |
| 4 | Advanced Taming | Tame rare/exotic beasts |

### Commands (Circle 1-5)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Attack | All pack attacks target |
| 1 | Defend | All pack guards you |
| 2 | Frenzy | Pack +30% attack speed 12s |
| 2 | Kill Command | Alpha deals 30-45 phys |
| 3 | Coordinated Attack | All pack attacks same time (+10% damage each) |
| 3 | Pack Howl | Pack +20% damage, fear nearby enemies 3s |
| 4 | Bestial Fury | Alpha +100% damage, immune CC 10s |
| 5 | Stampede | All 3 pack charge target, 25-38 each, stun 2s |

### Pack Bonuses (Circle 3-6)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 3 | Pack Tactics | Pack members +10% damage per pack member |
| 4 | Shared Senses | See through Alpha's eyes, +tracking |
| 4 | Blood Bond | Damage to Alpha splits to you |
| 5 | Alpha Presence | You +25% stats while near Alpha |
| 5 | Pack Resilience | Pack -25% damage taken |
| 6 | Spirit Link | Pack shares HP pool |

### Utility (Circle 2-6)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 2 | Mend Pack | Heal all pack 25-40 HP |
| 3 | Play Dead | Beast appears dead, drop threat |
| 3 | Fetch | Alpha retrieves items/loot |
| 4 | Aspect of the Beast | You gain Alpha's special ability 30s |
| 5 | Primal Rage | All pack +50% crit 15s |
| 6 | Mass Mend | Heal all pack 50-75 HP |

### Ultimate (Circle 7-8)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 7 | Apex Predator | Alpha: +100% stats, abilities cooldown reset 20s |
| 7 | Call of the Wild | Summon 3 temporary beasts 30s |
| 8 | Avatar of the Pack | Transform: Become beast form, pack +100% stats, unlimited pack size, 30s |
| 8 | Primal Bond | All pack: immune death 10s, if would die: survive at 1 HP |

---

# 11. ARTIFICER

### *"Technology conquers all"*

**Inspired By:** WoW Engineer, Guild Wars 2 Engineer
**Role:** Gadget DPS, zone control
**Core Mechanic:** Steam + Charges

## Steam System

```csharp
public int Steam { get; set; } // Max 100
// Generate: +10/sec while near boiler, +5 on gadget hit
// Spend: Power abilities, maintain turrets
// Decay: -5/sec if no boiler nearby
```

## Charge System

```csharp
public int Charges { get; set; } // Max 10
// Generate: Crafted between fights (reagents)
// Spend: Bombs, grenades, consumables
// No regen in combat
```

## Spells (32 total)

### Turrets (Circle 2-6)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 2 | Gun Turret | 150 HP, 10-16 phys every 2s |
| 3 | Flame Turret | 120 HP, cone 6-10 fire/tick |
| 3 | Net Turret | 100 HP, roots first enemy 4s |
| 4 | Rocket Turret | 180 HP, 25-38 fire every 4s |
| 5 | Healing Turret | 150 HP, heals allies 8-12/tick |
| 6 | Siege Turret | 300 HP, 40-60 phys every 3s, 12 range |

### Bombs & Grenades (Circle 1-5)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Frag Grenade | 6 tile AoE, 12-18 phys, costs 1 Charge |
| 2 | Fire Bomb | 6 tile AoE, 15-24 fire + burning 5-8/tick, costs 1 Charge |
| 2 | Flash Grenade | Blind 6 tiles 5s, costs 1 Charge |
| 3 | Concussion Bomb | 6 tile AoE, 18-28 phys + stun 2s, costs 2 Charges |
| 4 | Cluster Bomb | 3 explosions, 15-25 each, costs 2 Charges |
| 5 | Siege Bomb | 10 tile AoE, 40-60 phys, costs 3 Charges |

### Gadgets (Circle 1-5)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Deploy Boiler | Place boiler (generates Steam in 8 tiles) |
| 2 | Personal Shield | Absorb 100 damage, costs 20 Steam |
| 3 | Rocket Boots | +100% speed, fly over obstacles 6s, costs 30 Steam |
| 3 | Grapple | Pull yourself to location 10 tiles |
| 4 | Overcharge | All turrets +100% damage 10s, costs 40 Steam |
| 5 | Mech Suit | Summon rideable mech (500 HP, 25-40 phys) 30s |

### Mechanical Pets (Circle 3-6)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 3 | Deploy Spider Drone | 100 HP, poisons, scouts |
| 4 | Deploy Attack Drone | 150 HP, 15-24 phys ranged |
| 5 | Deploy Repair Drone | 120 HP, heals you 10-16/tick |
| 6 | Deploy Siege Golem | 400 HP, 30-45 phys, knockback |

### Ultimate (Circle 7-8)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 7 | Turret Overload | All turrets: explode for 50-75 AoE, resummon instantly |
| 7 | Unlimited Charges | Charges don't deplete 20s |
| 8 | Avatar of the Machine | Transform: +50% gadget damage, turrets +100% HP, Steam +200% generation, 30s |
| 8 | Orbital Strike | Call down laser, 15 tile line, 80-120 energy, costs all Steam |

---

# 12. ALCHEMIST

### *"The right formula changes everything"*

**Inspired By:** Pathfinder Alchemist, Witcher potions
**Role:** Buff/burst through consumables, mutations
**Core Mechanic:** Reagents + Mutagens

## Reagent System

```csharp
public Dictionary<Reagent, int> Reagents { get; set; }
// Gather: From world, vendors, loot
// Combine: Create potions, bombs, mutagens
// Carry limit: 50 total reagents
```

## Mutagen System

```csharp
public Mutagen ActiveMutagen { get; set; } // Only one active
// Duration: 5 minutes
// Powerful buffs with drawbacks
// Crafted from rare reagents
```

| Mutagen | Buff | Drawback |
|---------|------|----------|
| Predator | +30% damage, +30% attack speed | -20% max HP |
| Warden | +50 all resists, +25% max HP | -20% damage |
| Shadow | Permanent stealth detection, +30% crit | -30% healing received |
| Catalyst | +50% potion effects, -50% cooldowns | Potions damage you 10% HP |
| Berserker | +50% damage when <50% HP | Cannot be healed above 50% |

## Spells (32 total)

### Combat Potions (Circle 1-4)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Healing Potion | Heal 30-48 HP |
| 1 | Mana Potion | Restore 40-60 mana |
| 2 | Haste Potion | +40% speed 30s |
| 2 | Strength Potion | +20 STR 60s |
| 3 | Resist Potion | +30 all resists 60s |
| 3 | Invisibility Potion | Invisible 30s |
| 4 | Giant's Potion | +50% max HP, +50% damage 30s |
| 4 | Stoneskin Potion | -40% physical damage 30s |

### Bombs (Circle 2-5)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 2 | Fire Bomb | 6 tile AoE, 18-28 fire |
| 2 | Frost Bomb | 6 tile AoE, 15-24 cold + slow |
| 3 | Acid Bomb | 6 tile AoE, 20-32 poison + -20 armor |
| 3 | Shock Bomb | 6 tile AoE, 22-35 energy + stun 2s |
| 4 | Void Bomb | 8 tile AoE, 30-45 shadow + silence 4s |
| 5 | Inferno Bomb | 10 tile AoE, 40-60 fire + burning ground |

### Mutagens (Circle 4-7)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 4 | Predator Mutagen | +30% damage, -20% HP, 5 min |
| 4 | Warden Mutagen | +50 resists, -20% damage, 5 min |
| 5 | Shadow Mutagen | Stealth detect, +30% crit, -30% healing, 5 min |
| 5 | Catalyst Mutagen | +50% potion effect, self-damage, 5 min |
| 6 | Berserker Mutagen | +50% damage <50% HP, heal cap, 5 min |
| 7 | Perfected Mutagen | +25% all stats, no drawback, 5 min |

### Transmutation (Circle 3-6)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 3 | Transmute Metal | Convert metal type (10 ingots) |
| 4 | Philosopher's Stone | Convert 100 iron to gold |
| 4 | Homunculus | Create small servant (100 HP, fetch/scout) |
| 5 | Transmute Flesh | Heal 50-80 HP, costs reagents |
| 6 | Clone Potion | Create copy of self for 30s |

### Ultimate (Circle 7-8)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 7 | Mass Potion | All party gains your current potion effect |
| 7 | Toxin Master | All bombs +100% effect, +50% radius 30s |
| 8 | Avatar of Alchemy | Transform: Potions instant, no cooldown, double effect, mutagen has no drawback, 30s |
| 8 | Elixir of Immortality | Cannot die 15s, heal to full when expires |


---

# 13. CLERIC

### *"The divine light flows through me"*

**Inspired By:** D&D Cleric, EQ Cleric, WoW Priest
**Role:** Primary healer, resurrection, buffs
**Core Mechanic:** Faith + Prayer

## Faith System

```csharp
public int Faith { get; set; } // Max 100
// Generate: +5 per heal cast, +10 on critical heal, +20 on resurrection
// Spend: Powerful miracles
// Decay: None, persists
```

## Prayer System

```csharp
// Prayers are long-duration buffs (30 min)
// Only one prayer active per target
// Recasting refreshes duration
```

## Spells (32 total)

### Direct Heals (Circle 1-4)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Minor Heal | Heal 15-24 HP |
| 2 | Heal | Heal 30-48 HP |
| 2 | Flash Heal | Instant heal 20-32 HP |
| 3 | Greater Heal | Heal 50-80 HP (slow cast) |
| 3 | Binding Heal | Heal target 30-45 HP, heal self 20-30 HP |
| 4 | Critical Heal | Heal 70-100 HP, costs 20 Faith |

### Heal Over Time (Circle 2-5)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 2 | Renew | HoT 8-12/tick 15s |
| 3 | Greater Renew | HoT 12-18/tick 18s |
| 4 | Prayer of Mending | Heal 25-40, jumps to injured ally on damage |
| 5 | Divine Hymn | Channel: AoE HoT 15-25/tick 8s |

### Group Heals (Circle 4-7)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 4 | Circle of Healing | Party heal 20-32 HP |
| 5 | Prayer of Healing | Party heal 35-55 HP (slow cast) |
| 6 | Mass Heal | Party heal 50-80 HP |
| 7 | Divine Providence | Party heal 80-120 HP, costs 50 Faith |

### Prayers/Buffs (Circle 1-5)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Prayer of Fortitude | +20% max HP 30 min |
| 2 | Prayer of Spirit | +10 mana/tick 30 min |
| 2 | Prayer of Protection | +20 all resists 30 min |
| 3 | Prayer of Strength | +15% damage 30 min |
| 3 | Prayer of Wisdom | +15 INT 30 min |
| 4 | Divine Spirit | +25% all stats 10 min, costs 20 Faith |
| 5 | Power Word: Shield | Absorb 150 damage 30s |

### Utility (Circle 2-6)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 2 | Cure Poison | Remove poison |
| 2 | Cure Disease | Remove disease |
| 3 | Remove Curse | Remove curse |
| 3 | Dispel Magic | Remove magic debuff |
| 4 | Purify | Remove all negative effects |
| 5 | Turn Undead | Undead 10 tiles: fear 10s |
| 6 | Holy Ground | 8 tile AoE: allies +30% healing received, undead damaged 15s |

### Ultimate (Circle 6-8)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 6 | Guardian Spirit | Target: if die in 10s, resurrect at 50% HP |
| 7 | Resurrection | Resurrect with 50% HP/mana (out of combat) |
| 7 | Mass Resurrection | Resurrect all dead in 20 tiles |
| 8 | Avatar of the Divine | Transform: Heals +100%, cost 0, instant cast, 20s |
| 8 | Miracle | Full heal all party, remove all debuffs, costs 100 Faith |

---

# 14. WIZARD

### *"Knowledge is the greatest power"*

**Inspired By:** D&D Wizard, UO Magery (generalist)
**Role:** Flexible caster, spell variety
**Core Mechanic:** Spell Power + Metamagic

## Spell Power System

```csharp
public int SpellPower { get; set; }
// From INT, skills, equipment
// Increases all spell damage/healing by %
// Base: 100 + (INT - 50)
```

## Metamagic System

```csharp
public MetamagicType ActiveMetamagic { get; set; }
// Modify next spell cast
// Only one metamagic per spell
// Costs extra mana
```

| Metamagic | Effect | Mana Cost |
|-----------|--------|:---------:|
| Empower | +50% damage/healing | +50% |
| Quicken | Instant cast | +100% |
| Extend | Double duration | +25% |
| Widen | +50% AoE radius | +50% |
| Silent | No incantation (stealth cast) | +25% |

## Spells (32 total)

### Damage (Circle 1-5)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Magic Missile | 6-12 arcane (never misses) |
| 2 | Fireball | 6 tile AoE, 18-28 fire |
| 2 | Lightning Bolt | Line 10 tiles, 20-32 energy |
| 3 | Ice Storm | 8 tile AoE, 22-35 cold + slow |
| 3 | Chain Lightning | 25-38 energy, chains to 4 |
| 4 | Disintegrate | 40-60 arcane (single target) |
| 4 | Meteor Swarm | 8 tile AoE, 35-52 fire |
| 5 | Finger of Death | 50-75 shadow, +50% vs <25% HP |

### Control (Circle 2-5)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 2 | Web | 6 tile AoE root 6s |
| 2 | Grease | 6 tile AoE slow zone 15s |
| 3 | Hold Person | Paralyze 6s |
| 3 | Slow | -50% speed 15s |
| 4 | Wall of Force | 8 tile wall, blocks movement/spells 20s |
| 4 | Confusion | Random actions 10s |
| 5 | Dominate | Control creature 30s |
| 5 | Maze | Banish target 10s |

### Utility (Circle 1-5)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 1 | Light | Illuminate area 30 min |
| 1 | Detect Magic | See magic auras 60s |
| 2 | Invisibility | Invisible 60s |
| 2 | Knock | Open locked door/chest |
| 3 | Fly | Flight 5 min |
| 3 | Dispel Magic | Remove magic effects |
| 4 | Teleport | Teleport to marked location |
| 5 | Scrying | See distant location/person 60s |

### Protection (Circle 2-6)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 2 | Shield | +25% defense, immune Magic Missile 60s |
| 3 | Protection from Elements | +30 elemental resists 5 min |
| 4 | Stoneskin | Absorb 100 physical damage |
| 4 | Globe of Invulnerability | Immune circle 1-3 spells 10s |
| 5 | Spell Turning | Reflect next spell back |
| 6 | Prismatic Shield | +50 all resists, 20% spell reflect 30s |

### Ultimate (Circle 6-8)

| Cir | Spell | Effect |
|:---:|-------|--------|
| 6 | Time Stop | Freeze all enemies 15 tiles 3s |
| 7 | Power Word: Kill | If target <100 HP: instant death |
| 7 | Wish | Any circle 1-5 spell, instant, free |
| 8 | Avatar of the Arcane | Transform: Metamagic free, all spells -50% cost, +50% Spell Power, 30s |
| 8 | Cataclysm | 15 tile AoE, 80-120 arcane + fire + cold + energy |


---

# Part Five: Implementation Reference

---

## Resource Comparison

| Class | Primary Resource | Secondary | Generation |
|-------|------------------|-----------|------------|
| Sorcerer | Mana | Stance | Passive |
| Warlock | Mana | Soul Shards | Kills, crits |
| Necromancer | Mana | Life Force + Corpses | Deaths, drains |
| Witch | Mana | Curse stacks + Voodoo | Casting curses |
| Enchanter | Mana | CC DR tracking | Passive |
| Oracle | Mana | Time Echoes | Automatic |
| Illusionist | Mana | Clones | Summoning |
| Ice Mage | Mana | Chill stacks | Cold damage |
| Shaman | Mana | Totems | Placement |
| Summoner | Mana | Creature slots + Bond | Summoning |
| Druid | Mana | Forms | Shapeshifting |
| Bard | Mana | Song ticks | Channeling |
| Paladin | Mana | Tithing + Virtues | Tithe gold, actions |
| Templar | Mana | Zeal | Combat hits |
| Knight | Stamina | Fortitude | Blocking, taking damage |
| Fighter | Stamina | Stance | Passive |
| Barbarian | Fury | Rage (transform) | Hits dealt/received |
| Monk | Stamina | Chi | Builder abilities |
| Rogue | Stamina | Combo Points | Builder abilities |
| Ranger | Stamina | Focus | Standing still |
| Bounty Hunter | Stamina | Pursuit | Hits on mark |
| Beastmaster | Stamina | Pack Bond | Passive |
| Artificer | Steam | Charges | Boiler, crafting |
| Alchemist | Reagents | Mutagen | Gathering, crafting |
| Cleric | Mana | Faith | Healing |
| Wizard | Mana | Spell Power | Passive (INT) |

---

## ServUO Native Features Used

| Mechanic | Implementation |
|----------|----------------|
| Stances/Forms | BuffInfo swap + BodyMod |
| Soul Shards | int on PlayerMobile |
| Corpses | Item at death location |
| Life Force | int secondary resource |
| Curses | Stacking BuffInfo |
| Voodoo Doll | Item with Mobile ref |
| CC DR | Dictionary per target |
| Time Echo | Queue of state objects |
| Wells/Totems | Item with Timer |
| Clones | BaseCreature with Body copy |
| Shatter | Delete creatures, AoE effect |
| Chill Stacks | int per target |
| Bonds | Mobile reference |
| Songs | Channeled buff aura |
| Crescendo | Counter during channel |
| Tithing | int on PlayerMobile (UO native) |
| Virtues | Dictionary on PlayerMobile |
| Zeal/Fury/Chi | int with decay timer |
| Combo Points | int per target |
| Stealth | Native hiding system |
| Focus | int with regen/decay |
| Marks | Mobile reference |
| Pursuit | int on marked target |
| Pack | List of BaseCreature |
| Steam/Charges | int resources |
| Reagents | Item container |
| Mutagens | BuffInfo with drawback |
| Faith | int on PlayerMobile |
| Spell Power | Calculated stat |
| Metamagic | Enum modifier |

---

## Balance Guidelines

### Damage Per Mana (DPM) Targets

| Role | Target DPM | Notes |
|------|:----------:|-------|
| Burst DPS | 2.0-2.5 | High DPM, long cooldowns |
| Sustained DPS | 1.5-2.0 | Moderate DPM, spammable |
| Support | 0.5-1.0 | Low damage, high utility |
| Tank | 1.0-1.5 | Moderate, self-healing |
| Healer | N/A | Healing per mana instead |

### CC Duration Caps

| CC Type | Max Duration | Notes |
|---------|:------------:|-------|
| Stun | 4s | Hard CC |
| Freeze | 5s | Ice exclusive |
| Sleep | 12s | Breaks on damage |
| Root | 8s | Can still attack |
| Silence | 6s | Can still move/attack |
| Fear | 6s | Runs away |
| Charm | 30s | Creature only |

### Transform Duration

All ultimate transforms: **30-45 seconds**, significant power boost, long cooldown (5+ minutes).

---

## Skill Level → Circle Access

| Skill Level | Circles Available |
|-------------|-------------------|
| 0-29 | Circle 1 |
| 30-39 | Circle 2 |
| 40-49 | Circle 3 |
| 50-59 | Circle 4 |
| 60-69 | Circle 5 |
| 70-79 | Circle 6 |
| 80-89 | Circle 7 |
| 90+ | Circle 8 |
| 100+ | Mastery abilities |
| 110-120 | Enhanced masteries |

---

## Class-to-Skill Mapping

| Class | Primary Skill | Support Skills |
|-------|---------------|----------------|
| Sorcerer | Elemancy | Eval Int, Meditation |
| Warlock | Demonology | Spirit Speak, Eval Int |
| Necromancer | Necromancy | Spirit Speak, Anatomy |
| Witch | Hexcraft | Poisoning, Spirit Speak |
| Enchanter | Enchanting | Eval Int, Meditation |
| Oracle | Chronomancy | Meditation, Focus |
| Illusionist | Illusion | Stealth, Hiding |
| Ice Mage | Cryomancy | Eval Int, Meditation |
| Shaman | Shamanism | Spirit Speak, Animal Lore |
| Summoner | Summoning | Animal Taming, Animal Lore |
| Druid | Shapeshifting | Animal Lore, Veterinary |
| Bard | Musicianship | Provocation, Peacemaking |
| Paladin | Chivalry | Tactics, Healing |
| Templar | Judgment | Tactics, Resist Spells |
| Knight | Valor | Parrying, Tactics |
| Fighter | Combat Mastery | Tactics, Anatomy |
| Barbarian | Rage | Tactics, Anatomy |
| Monk | Ki | Tactics, Focus |
| Rogue | Shadow Arts | Stealth, Hiding |
| Ranger | Wilderness | Archery, Tracking |
| Bounty Hunter | Pursuit | Tracking, Forensics |
| Beastmaster | Pack Tactics | Animal Taming, Veterinary |
| Artificer | Engineering | Tinkering, Mining |
| Alchemist | Alchemy | Cooking, Taste ID |
| Cleric | Divine Magic | Meditation, Healing |
| Wizard | Arcana | Eval Int, Inscription |

---

## Total Ability Count

- **12 Magic Schools × 32 abilities = 384 abilities**
- **14 Martial Classes × 32 abilities = 448 abilities**
- **Grand Total: 832 unique abilities**

---

## Lore Integration

Each class ties to Vystia's world:

| Class | Faction/Location |
|-------|------------------|
| Artificer | Technoguild Artisans |
| Barbarian | Frosthold Berserkers |
| Bard | Steam Harmonic Virtuosos |
| Cleric | Priests of the Elemental Balance |
| Druid | Guardians of Verdantpeak |
| Fighter | Ironclad Legionnaires |
| Ice Mage | Frostweavers of Winterguard |
| Monk | Gearwheel Monastery |
| Paladin | Knights of the Forge Pact |
| Ranger | Dune Striders of The Whispering Sands |
| Rogue | Shadow Guild Operatives |
| Sorcerer | Elemental Savants of The Emberlands |
| Warlock | Covenant of the Hidden Eye |
| Wizard | Scholars of the Arcane Academy |
| Alchemist | Brewmasters of the Verdant Isles |
| Necromancer | Shadowbinders of Eternal Twilight |
| Shaman | Spiritcallers of The Wilderlands |
| Witch | Covens of Shadowfen |
| Enchanter | Sigilmasters of the Mystic Canyons |
| Illusionist | Mirage Weavers of The Whispering Sands |
| Templar | Justicars of the Ironclad Empire |
| Beastmaster | Pack Leaders of Frosthold |
| Summoner | Arcane Summoners of the Forgotten Depths |
| Bounty Hunter | Crypt Trackers of the Sunken Isles |
| Knight | Guardians of the Glimmering Archipelago |
| Oracle | Seers of the Crystal Barrens |

---

**End of Complete Class Design Document**

**Total: 26 Classes | 832 Abilities | Ready for Implementation**

---

# Part Six: PvP and PvE Combo Documentation

---

## Internal Class Combos (Solo Play)

Each class has built-in ability synergies. These are the core rotations and combos players should master.

---

### SORCERER (Elementalist)

**PvE Rotation:**
1. Fire Stance → Ignite (DoT) → Fireball spam
2. Switch to Earth → Eruption combo triggers → Earth Armor for survival
3. Low HP? Switch to Water → Flash Boil combo → Healing Tide

**PvP Burst Combo:**
1. Air Stance → Chain Lightning
2. Switch to Fire → Lightning Storm combo triggers
3. Pyroclasm for AoE finish
4. If focused: Fire → Water = Steam Blast (blind) → escape

**Key Synergies:**
| Combo | Effect | Use Case |
|-------|--------|----------|
| Fire → Water | Steam Blast blind | Escape/setup |
| Water → Air | Mist Form invuln | Survival |
| Air → Fire | Lightning Storm | Burst AoE |
| Earth → Water | Geyser knockup + heal | Peel/sustain |

---

### WARLOCK

**PvE Rotation:**
1. Summon Voidwalker (tank)
2. Corruption → Curse of Agony → Siphon Life (full DoT stack)
3. Immolate → Shadow Bolt spam (fish for Soul Shards)
4. Conflagrate when Immolate ticking
5. Drain Life if HP drops

**PvP Burst Combo:**
1. Unstable Affliction + Corruption + Curse of Agony (pressure)
2. Immolate → Conflagrate (burst)
3. Shadowburn (execute) with Soul Shard
4. Seed of Corruption on melee trains

**Key Synergies:**
| Combo | Effect | Use Case |
|-------|--------|----------|
| Immolate → Conflagrate | 40-60 instant burst | Execute |
| Drain Soul on kill | Generate Soul Shard | Resource |
| DoT stack → Seed | AoE explosion on death | Cleave |
| Voidwalker + Drain Life | Unkillable 1v1 | Duels |

---

### NECROMANCER

**PvE Rotation:**
1. Pull with Death Bolt
2. Enemy dies → Raise Skeleton
3. Build army: 5 Skeletons OR 2 Skeletal Mages + Bone Golem
4. Corpse Explosion on corpse piles for AoE
5. Death Shroud for boss burn phases

**PvP Burst Combo:**
1. Desecrate (create corpses)
2. Corpse Lance × 3 (massive burst)
3. Mass Corpse Explosion if corpses available
4. Death Shroud → Life Blast spam if focused

**Key Synergies:**
| Combo | Effect | Use Case |
|-------|--------|----------|
| Kill → Raise Skeleton | Free army | Sustained fights |
| Desecrate → Corpse Lance | Burst without kills | Opener |
| Army of Dead → Mass Explosion | Massive AoE | Group PvP |
| Death Shroud → Consume | Extend transform | Survival |

---

### WITCH (Curse Witch)

**PvE Rotation:**
1. Hex of Vulnerability (-resists)
2. Hex of Pain + Hex of Weakness
3. Stack 5+ curses
4. Agony (damage per curse)
5. Doom Spiral (+10% damage per curse)

**PvP Shutdown Combo:**
1. Create Voodoo Doll (link target)
2. Mortal Wound (-50% healing)
3. Soul Rot (-75% healing + DoT)
4. Stick Pins through doll (bypasses LoS)
5. Finger of Death execute at <20% HP with 3+ curses

**Key Synergies:**
| Combo | Effect | Use Case |
|-------|--------|----------|
| 5+ curses → Agony | 25+ damage/tick | Sustained pressure |
| Mortal Wound → Soul Rot | -125% healing (capped) | Anti-healer |
| Voodoo Doll → Stick Pins | LoS-ignoring damage | Chase/hide |
| Curse stack → Finger of Death | Execute threshold | Kill confirm |

---

### ENCHANTER

**PvE Rotation:**
1. Haste party
2. Mesmerize dangerous add
3. Charm Monster (use enemy as pet)
4. Mana Gift to healer
5. Group Clarity for sustain

**PvP Control Combo:**
1. Mesmerize healer
2. Mass Slow on melee
3. Mana Burn on caster
4. Paralyze kill target
5. Mind Blank ally if CC'd

**Key Synergies:**
| Combo | Effect | Use Case |
|-------|--------|----------|
| Mesmerize → team bursts other | Split pressure | Focus fire |
| Mana Burn → Silence | Caster shutdown | Anti-mage |
| Group Haste → melee train | +30% team DPS | Burst window |
| Charm Monster → tank add | Free pet | PvE cheese |

---

### ORACLE (Chronomancer)

**PvE Rotation:**
1. Haste Well on melee
2. Group Quicken
3. Slow Well on enemies
4. Temporal Shield on tank before big hit
5. Rewind Ally if mistake

**PvP Survival Combo:**
1. Borrowed Time (death insurance)
2. Get bursted → auto-rewind
3. Time Stop Field (freeze enemies)
4. Chrono Shift (swap HP% with enemy)
5. Déjà Vu (reset cooldowns) → repeat

**Key Synergies:**
| Combo | Effect | Use Case |
|-------|--------|----------|
| Temporal Shield → big hit | Negate damage | Tank support |
| Borrowed Time → Rewind | Can't die for 10s | Survival |
| Time Stop → team burst | 5s free damage | Coordinated kill |
| Chrono Shift low HP | Steal enemy HP% | Clutch reversal |

---

### ILLUSIONIST (Mesmer)

**PvE Rotation:**
1. Mirror Image × 3
2. Phantasm (durable clone)
3. Mind Wrack shatter (burst)
4. Rebuild clones
5. Repeat

**PvP Burst Combo:**
1. Legion (3 clones)
2. Veil (all invisible)
3. Position clones around target
4. Mind Explosion shatter (105-150 + 6s stun)
5. Phantasmal Pain follow-up

**Key Synergies:**
| Combo | Effect | Use Case |
|-------|--------|----------|
| 3 clones → Mind Wrack | 54-84 AoE burst | Damage |
| 3 clones → Distortion | 4.5s invulnerability | Survival |
| Decoy → Ambush | Invisible reposition | Escape |
| Swap → Shatter | Reposition + burst | Juke |

---

### ICE MAGE

**PvE Rotation:**
1. Frost Armor (passive chill)
2. Frostbolt spam (build chill)
3. At 5 stacks → Shatter
4. Blizzard on packs
5. Comet Storm for burst

**PvP Burst Combo:**
1. Frost Nova (2 chill + damage)
2. Ice Lance × 2 (4 more chill = frozen)
3. Deep Freeze (extend freeze)
4. Glacial Spike (50-75 + shatter)
5. Rime Reaper execute

**Key Synergies:**
| Combo | Effect | Use Case |
|-------|--------|----------|
| 5 chill → Frozen → Shatter | +50% damage burst | Kill combo |
| Ice Block → Cold Snap | Reset block, repeat | Survival |
| Blizzard + Frost Nova | Perma-slow + freeze | AoE control |
| Absolute Zero → Shatter | -50 cold resist + burst | Tank buster |

---

### SHAMAN

**PvE Rotation:**
1. Drop Stoneskin + Healing Stream + Windfury + Searing totems
2. Flame Shock → Lava Burst (auto-crit)
3. Chain Lightning on packs
4. Feral Spirits for burst
5. Spirit Link Totem for dangerous phases

**PvP Burst Combo:**
1. Grounding Totem (eat first spell)
2. Earthbind Totem (slow)
3. Flame Shock → Lava Burst
4. Feral Spirits
5. Thunderstorm (knockback + damage)

**Key Synergies:**
| Combo | Effect | Use Case |
|-------|--------|----------|
| Flame Shock → Lava Burst | Guaranteed crit | Burst |
| 4 totems + wolves | Maximum pressure | All-in |
| Tremor Totem | Break fear/charm | Anti-CC |
| Spirit Link → AoE damage | Spread damage | Survival |

---

### SUMMONER

**PvE Rotation:**
1. Summon Drake (3 slots) + Bear (2 slots) = 5 slots
2. Bond with Drake (+25% stats)
3. Attack Command
4. Empower on Drake for burst
5. Mass Mend to sustain

**PvP Swarm Combo:**
1. 5× Wolf (pack tactics = +50% each)
2. Bond with one wolf
3. Frenzy (+30% attack speed)
4. Stampede on target
5. Sacrifice low HP summon for heal

**Key Synergies:**
| Combo | Effect | Use Case |
|-------|--------|----------|
| 5 wolves + Pack Tactics | +50% damage each | Swarm |
| Bond + Empower | +75% stats on one | Focus fire |
| Phoenix + death | Auto-rebirth | Sustain |
| Share Pain | Split damage 6 ways | Survival |

---

### DRUID

**PvE Tank Rotation (Bear):**
1. Bear Form
2. Growl (taunt)
3. Swipe (AoE threat)
4. Frenzied Regen when damaged
5. Survival Instincts for big hits

**PvE DPS Rotation (Cat):**
1. Cat Form → Prowl
2. Pounce (opener + stun)
3. Rake (bleed + combo)
4. Claw to 5 combo points
5. Rip (finisher bleed)
6. Ferocious Bite execute

**PvP Flag Carry:**
1. Travel Form (speed)
2. If caught: Bear Form → Survival Instincts
3. Frenzied Regen
4. Bash stun → Travel Form escape

**Key Synergies:**
| Combo | Effect | Use Case |
|-------|--------|----------|
| Prowl → Pounce → Rake → Rip | Full opener | Burst |
| Bear → Cat → Bear | Form dance survival | Duels |
| Tree → Swiftmend | Instant big heal | Emergency |
| Incarnation + any form | 30s power spike | Burn phase |

---

### BARD

**PvE Rotation:**
1. Song of War (+25% damage party)
2. Maintain for 10+ ticks
3. Crescendo Heal if needed
4. Switch to Song of Regeneration for sustain
5. Magnum Opus for phase transitions

**PvP Support Combo:**
1. Song of the Aegis (-20% damage taken)
2. Build Crescendo ticks
3. Thunder Clap (interrupt enemy casts)
4. Crescendo Finale (AoE stun)
5. Dual Song for double buff

**Key Synergies:**
| Combo | Effect | Use Case |
|-------|--------|----------|
| Long song → Crescendo Finale | Big stun + damage | Setup |
| Dirge of Terror + team burst | Enemies flee | Peel |
| Dual Song | Two buffs at once | Ultimate value |
| Anthem of Heroes | Fear immunity | Anti-Warlock |

---

### PALADIN

**PvE Tank Rotation:**
1. Consecration (AoE threat + heal)
2. Seal of Righteousness
3. Crusader Strike spam
4. Holy Shield for mitigation
5. Lay on Hands emergency

**PvP Support Combo:**
1. Blessing of Kings on team
2. Aura of Devotion
3. Hammer of Justice (stun target)
4. Holy Wrath (burst)
5. Divine Shield if focused → Lay on Hands

**Key Synergies:**
| Combo | Effect | Use Case |
|-------|--------|----------|
| Consecration + melee | Damage + heal | Sustain |
| Hammer of Justice → team burst | 4s stun setup | Kill window |
| Divine Shield → Lay on Hands | Unkillable heal | Reset |
| Virtue stacks → consume | Big burst/heal | Clutch |

---

### TEMPLAR

**PvE Rotation:**
1. Mark for Judgment
2. Zealous Strike × 3 (build Zeal)
3. Burning Brand (DoT + Zeal/tick)
4. Judgment Strike at 10 Zeal
5. Verdict execute

**PvP Burst Combo:**
1. Charge (gap close + 2 Zeal)
2. Mark for Judgment
3. Frenzy (+50% attack speed)
4. Build to 10 Zeal fast
5. Final Judgment (80+ damage)
6. Verdict execute (<15% HP)

**Key Synergies:**
| Combo | Effect | Use Case |
|-------|--------|----------|
| Mark → all damage +15% | Sustained bonus | Focus fire |
| 10 Zeal → Verdict | Execute threshold | Kill confirm |
| Null Field → melee train | No magic zone | Anti-caster |
| Frenzy → Rampage | Extended window | Cleanup |

---

### KNIGHT

**PvE Tank Rotation:**
1. Defensive Stance
2. Taunt → Shield Wall
3. Shield Slam (Fortitude gen)
4. Riposte after blocks
5. Last Stand for emergencies

**PvP Peel Combo:**
1. Intercept to ally
2. Guardian's Oath (split damage)
3. Mass Taunt
4. Shield Charge (stun)
5. Hold the Line (block path)

**Key Synergies:**
| Combo | Effect | Use Case |
|-------|--------|----------|
| Block → Riposte | Free counter-attack | Damage |
| Intercept → Guardian's Oath | Save ally | Peel |
| Phalanx + party | Shared block chance | Group survival |
| Avatar of Shield | Unkillable redirect | Ultimate peel |

---

### FIGHTER

**PvE Rotation:**
1. Aggressive Stance
2. Charge → Rending Strike (bleed)
3. Mortal Strike
4. Execute at <25%
5. Battle Cry for burst windows

**PvP Burst Combo:**
1. Berserker Stance
2. Charge
3. Colossus Smash (-30% armor)
4. Recklessness (+100% crit)
5. Flurry (5 hits, all crit)
6. Execute

**Key Synergies:**
| Combo | Effect | Use Case |
|-------|--------|----------|
| Colossus Smash → burst | Armor shred | Tank buster |
| Recklessness → Flurry | 5 guaranteed crits | Burst |
| Hamstring → Pursuit | Stick to target | Chase |
| Overpower after parry | Free crit | Counter |

---

### BARBARIAN

**PvE Rotation:**
1. Bash/Frenzy (build Fury)
2. Rend (bleed)
3. At 100 Fury → Rage
4. Savage Blow spam
5. Carnage for AoE

**PvP Burst Combo:**
1. War Cry (+armor, +Fury)
2. Furious Charge (gap close)
3. Get hit (generates Fury)
4. At 100 → Rage
5. Annihilate → Carnage
6. Primal Scream (AoE fear)

**Key Synergies:**
| Combo | Effect | Use Case |
|-------|--------|----------|
| Bloodthirst spam | Sustain + Fury | Solo |
| Rage → Titan Grip | Dual 2H weapons | Burst |
| Undying Rage → Rage | Can't die + transform | Clutch |
| Earthquake → Carnage | AoE stun + damage | Group PvP |

---

### MONK

**PvE Rotation:**
1. Tiger Palm (Chi + armor ignore)
2. Blackout Kick (Chi)
3. Rising Sun Kick (Chi + debuff)
4. At 5 Chi → Fists of Fury
5. Touch of Death execute

**PvP Burst Combo:**
1. Flying Serpent Kick (gap close)
2. Tiger Palm → Blackout Kick → Rising Sun Kick (combo bonus)
3. Fists of Fury (channel)
4. Touch of Karma (reflect damage)
5. Touch of Death execute

**Key Synergies:**
| Combo | Effect | Use Case |
|-------|--------|----------|
| 3-move combo → bonus | Various effects | Optimization |
| Serenity → spam | Free abilities | Burst window |
| Touch of Karma | Reflect 100% damage | Anti-burst |
| Roll → Transcendence | Extreme mobility | Escape |

---

### ROGUE

**PvE Rotation:**
1. Stealth → Ambush (opener)
2. Hemorrhage (bleed + combo)
3. At 5 combo → Rupture
4. Sinister Strike to rebuild
5. Eviscerate finisher

**PvP Burst Combo:**
1. Sap (out of combat CC)
2. Stealth → Cheap Shot (4s stun)
3. Garrote (silence + bleed)
4. Mutilate × 2 (4 combo)
5. Kidney Shot (4s stun chain)
6. Eviscerate

**Key Synergies:**
| Combo | Effect | Use Case |
|-------|--------|----------|
| Cheap Shot → Kidney Shot | 8s stun chain | Kill window |
| Vanish → restealth | Reset fight | Survival |
| Shadow Dance | Openers without stealth | Burst |
| Cloak of Shadows | Magic immunity | Anti-caster |

---

### RANGER

**PvE Rotation:**
1. Hunter's Mark
2. Stand still (build Focus)
3. Aimed Shot (big hit)
4. Rapid Fire
5. Kill Shot execute
6. Traps on patrol paths

**PvP Kiting Combo:**
1. Freezing Trap (behind you)
2. Hunter's Mark
3. Aimed Shot
4. If melee closes: Disengage
5. Enemy hits trap → frozen
6. Kill Shot execute

**Key Synergies:**
| Combo | Effect | Use Case |
|-------|--------|----------|
| Stand still → Focus → Aimed | High burst | Opener |
| Disengage → trap → kite | Peel + CC | Survival |
| Bestial Wrath + Kill Command | Pet burst | Execute |
| Feign Death → Stealth | Reset fight | Escape |

---

### BOUNTY HUNTER

**PvE Rotation:**
1. Mark Quarry
2. Tracking Strike (build Pursuit)
3. Bola (root + 2 Pursuit)
4. At 10 Pursuit → Collect Bounty
5. Finish the Job execute

**PvP Assassination Combo:**
1. Track Quarry (find target)
2. Smoke Bomb → Stealth
3. Assassinate (3 Pursuit)
4. Grappling Hook (pull + Pursuit)
5. Flashbang (blind + 2 Pursuit)
6. Nowhere to Run (can't escape)
7. Inevitable End (+50% damage)
8. Finish the Job

**Key Synergies:**
| Combo | Effect | Use Case |
|-------|--------|----------|
| Mark → all abilities +25% | Sustained bonus | Focus |
| 10 Pursuit → Finish the Job | Instant kill <20% | Execute |
| Nowhere to Run → burst | Target trapped | Kill confirm |
| Claim Bounty | 1000g + full reset | Reward |

---

### BEASTMASTER

**PvE Rotation:**
1. Alpha Bond best pet
2. Call full pack (3 beasts)
3. Attack Command
4. Kill Command on cooldown
5. Bestial Fury for burst
6. Mend Pack for sustain

**PvP Swarm Combo:**
1. 3× Cat (high damage + prowl)
2. Alpha Bond one cat
3. Pack Howl (fear enemies)
4. Stampede (all charge)
5. Primal Rage (+50% crit)
6. Apex Predator (Alpha +100%)

**Key Synergies:**
| Combo | Effect | Use Case |
|-------|--------|----------|
| Pack Tactics + 3 pets | +30% each | Sustained |
| Stampede → Bestial Fury | Burst + stun | All-in |
| Spirit Link | Shared HP pool | Survival |
| Aspect of Beast | Gain pet ability | Utility |

---

### ARTIFICER

**PvE Rotation:**
1. Deploy Boiler (Steam gen)
2. Gun Turret + Flame Turret
3. Frag Grenade spam
4. Overcharge turrets
5. Siege Golem for bosses

**PvP Zone Control Combo:**
1. Deploy Boiler
2. Net Turret + Rocket Turret
3. Personal Shield
4. Concussion Bomb (stun)
5. Cluster Bomb follow-up
6. If rushed: Rocket Boots escape

**Key Synergies:**
| Combo | Effect | Use Case |
|-------|--------|----------|
| Boiler + turrets | Sustained zone | Control |
| Overcharge → Turret Overload | Burst AoE | All-in |
| Mech Suit | Mobile damage | Chase |
| Orbital Strike | 80-120 line nuke | Finisher |

---

### ALCHEMIST

**PvE Rotation:**
1. Predator Mutagen (+30% damage)
2. Strength Potion
3. Fire Bomb → Acid Bomb
4. Healing Potion as needed
5. Giant's Potion for bosses

**PvP Burst Combo:**
1. Catalyst Mutagen (+50% potion effect)
2. Giant's Potion (+75% HP/damage with mutagen)
3. Shock Bomb (stun)
4. Inferno Bomb
5. Void Bomb (silence)
6. Elixir of Immortality if dying

**Key Synergies:**
| Combo | Effect | Use Case |
|-------|--------|----------|
| Catalyst + any potion | +50% effect | Amplify |
| Berserker Mutagen low HP | +50% damage | Clutch |
| Toxin Master + bombs | Double bomb damage | Burst |
| Clone Potion | 2v1 | Duel |

---

### CLERIC

**PvE Rotation:**
1. Prayer of Fortitude on tank
2. Prayer of Spirit on casters
3. Renew on tank (HoT)
4. Heal as needed
5. Circle of Healing for AoE
6. Guardian Spirit before big hits

**PvP Support Combo:**
1. Power Word: Shield on focus target
2. Prayer of Mending (bouncing heal)
3. Flash Heal spam
4. Purify to remove CC
5. Guardian Spirit if ally dying
6. Mass Resurrection after wipe

**Key Synergies:**
| Combo | Effect | Use Case |
|-------|--------|----------|
| Shield + Renew | Absorb + HoT | Preemptive |
| Guardian Spirit → death | Auto-resurrect | Save |
| Holy Ground + melee | +30% healing zone | Sustain |
| Miracle | Full heal + cleanse all | Emergency |

---

### WIZARD

**PvE Rotation:**
1. Fireball (AoE)
2. Chain Lightning (cleave)
3. Ice Storm (slow + damage)
4. Disintegrate (single target)
5. Metamagic: Empower on big spells

**PvP Burst Combo:**
1. Metamagic: Quicken
2. Time Stop (instant)
3. Wall of Force (trap)
4. Metamagic: Empower
5. Meteor Swarm (+50% damage)
6. Power Word: Kill execute

**Key Synergies:**
| Combo | Effect | Use Case |
|-------|--------|----------|
| Quicken → Time Stop | Instant freeze | Setup |
| Empower → Meteor | +50% AoE | Burst |
| Web → Ice Storm | Root + slow + damage | Control |
| Wish | Free circle 1-5 spell | Flexibility |

---

## Group PvE Synergies (Party Combos)

### Tank + Healer Core

| Tank | Healer | Synergy |
|------|--------|---------|
| Knight | Cleric | Guardian's Oath + Prayer of Mending = unkillable |
| Paladin | Druid (Tree) | Consecration heal + HoTs = double sustain |
| Knight | Bard | Rally + Song of Aegis = -35% damage taken |

### DPS + Support Combos

| DPS | Support | Synergy |
|-----|---------|---------|
| Barbarian | Enchanter | Haste + Rage = instant 100 Fury |
| Ice Mage | Chronomancer | Time Stop + Ice Age = perma-frozen |
| Necromancer | Bard | Song of Might + Army = +15% to all undead |
| Rogue | Enchanter | Group Haste + Shadow Dance = insane burst |

### AoE Wombo Combos

| Setup | Follow-up | Effect |
|-------|-----------|--------|
| Enchanter Gravity Well | Ice Mage Ice Age | Pull + freeze all |
| Knight Hold the Line | Wizard Meteor Swarm | Trapped + nuked |
| Bard Crescendo Finale | Barbarian Cataclysm | Stunned + AoE |
| Shaman Thunderstorm | Summoner Stampede | Knocked + trampled |

---

## Group PvP Synergies (Arena/War)

### 2v2 Compositions

| Comp | Strategy |
|------|----------|
| Rogue + Ice Mage | Sap one, burst other. Kidney Shot → Shatter combo |
| Warlock + Paladin | DoT pressure + sustain. Unkillable attrition |
| Templar + Enchanter | Mesmerize → Mark → Verdict execute |
| Barbarian + Cleric | Rage through damage, Cleric keeps alive |

### 3v3 Compositions

| Comp | Strategy |
|------|----------|
| Knight + Cleric + Rogue | Peel for Cleric, Rogue assassinates |
| Warlock + Witch + Druid | Triple DoT + anti-heal = pressure comp |
| Ice Mage + Enchanter + Templar | CC chain → execute |
| Bard + Barbarian + Monk | Song of War + double melee train |

### Large Scale (War/Siege)

| Role | Best Classes | Job |
|------|--------------|-----|
| Frontline | Knight, Paladin, Barbarian | Absorb damage, zone |
| Backline DPS | Ice Mage, Wizard, Ranger | AoE damage |
| Support | Bard, Cleric, Enchanter | Buffs, heals, CC |
| Assassins | Rogue, Bounty Hunter, Templar | Pick off healers |
| Siege | Artificer, Necromancer, Summoner | Turrets, pets, attrition |

### Target Priority

1. **Healers** (Cleric, Druid Tree, Paladin) - Kill first
2. **Controllers** (Enchanter, Bard, Chronomancer) - High impact CC
3. **Glass Cannons** (Ice Mage, Wizard, Rogue) - High damage, low HP
4. **Tanks** (Knight, Paladin) - Kill last, ignore if possible

---

## Counter Matchups

### Class Counters

| Class | Countered By | Reason |
|-------|--------------|--------|
| Necromancer | Paladin | Turn Undead destroys army |
| Warlock | Templar | Null Field shuts down caster |
| Ice Mage | Sorcerer (Fire) | Fire resist, stance counters |
| Rogue | Bounty Hunter | Nowhere to Hide reveals stealth |
| Enchanter | Monk | Diffuse Magic + mobility |
| Bard | Rogue | Kick interrupts songs |
| Summoner | Ice Mage | Blizzard kills all pets |
| Knight | Witch | Death Mark doubles all damage |
| Barbarian | Enchanter | CC prevents Rage |
| Cleric | Witch | Anti-healing completely shuts down |

### Hard Counters

| Situation | Counter |
|-----------|---------|
| Enemy stacking resists | Templar Verdict (execute ignores) |
| Enemy pet class | Ice Mage Blizzard (AoE pet clear) |
| Enemy healer | Witch Soul Rot (-75% healing) |
| Enemy melee train | Artificer turrets + Net Turret |
| Enemy caster stack | Templar Null Field (no magic zone) |
| Enemy stealth | Bounty Hunter Nowhere to Hide |

---

## Diminishing Returns Reference

CC stacking follows diminishing returns:

| Application | Duration |
|-------------|----------|
| 1st CC | 100% duration |
| 2nd CC (same type) | 50% duration |
| 3rd CC (same type) | 25% duration |
| 4th+ CC | Immune 15s |

**CC Categories (share DR):**
- Stun: Cheap Shot, Hammer of Justice, Concussive Blow, etc.
- Root: Entangling Roots, Freezing Trap, Web, etc.
- Silence: Garrote, Rebuke, Censure, etc.
- Fear: Psychic Scream, Primal Scream, Pack Howl, etc.
- Incapacitate: Mesmerize, Blind, Sap (separate from stun)

**Do NOT share DR:**
- Different CC types (stun → root → silence = full duration each)
- Slows (never DR, always full effect)

---

**End of PvP/PvE Combo Documentation**
