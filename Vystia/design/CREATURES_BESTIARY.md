# COMPLETE VYSTIA BESTIARY - ALL EXPANSIONS
## Using Full UO Art Library (T2A through Time of Legends)

---

## IMPORTANT: Thematic Creature Guidelines

This bestiary contains ONLY creatures that fit the Vystia world theme. **Do NOT spawn generic UO monsters** that are not listed here. The following common UO creatures are **EXCLUDED** from Vystia:

### Excluded Creatures (Non-Thematic)
| Creature | Reason for Exclusion |
|----------|---------------------|
| Orc, Orc Lord, etc. | No orcs in Vystia lore - use Goblins instead |
| Ettin | Doesn't fit any regional theme |
| Ratman, Ratman Archer | Use Kobold instead for small reptilians |
| Mongbat | Generic - use Ember Bat or regional bats |
| Headless One | No undead of this type in lore |
| Corpser (generic) | Use Thorn Beast instead |
| Giant Serpent (generic) | Use regional serpents (Boreal, Shadow, Sky) |
| Ogre, Ogre Lord | Use regional trolls or giants instead |
| Lizardman (generic) | Use Salamander or Kobold variants |
| Gazer | Use Beholder instead |
| Daemon (generic red) | Use Shadow Fiend instead |
| Balron | Not in Vystia lore |
| Bone Knight, Bone Mage | Use regional undead (Drowned Dead, etc.) |
| Imp (generic) | Use Steam Mephit instead |
| Terathan | Not in Vystia lore |
| Ophidian | Not in Vystia lore |
| Meer | Not in Vystia lore |
| Juka | Not in Vystia lore |
| Solen | Not in Vystia lore |

### Reskinning Guideline
When populating a region, use the creatures listed for that region with appropriate hues. Do not mix creatures across regions unless there's a lore reason (e.g., invasion, corruption).

---

## SERVUO COMPATIBILITY REFERENCE

All creatures in this bestiary are **fully compatible** with ServUO's current systems.

### Special Ability Mapping (SpecialAbility.cs)

| Bestiary Ability | ServUO Implementation |
|------------------|----------------------|
| Fire breath | `DragonBreath` with Fire damage type |
| Cold breath | `DragonBreath` with Cold damage type |
| Poison breath | `DragonBreath` with Poison damage type |
| Fire aura | `IAuraCreature` interface + `AngryFire` or `Inferno` |
| Cold aura | `IAuraCreature` interface (see ColdDrake, FrostDragon) |
| Life drain | `StealLife`, `LifeLeech`, or `LifeDrain` |
| Mana drain | `ManaDrain` ability |
| Poison attack | `VenomousBite`, `PoisonSpit`, `BloodDisease` |
| Bleed attack | `ViciousBite` |
| Stun attack | `ColossalBlow`, `TailSwipe` |
| Fear effect | `TrueFear` |
| Web attack | `Webbing` |
| Regeneration | `Heal` ability on timer |
| Summon minions | Custom spawn logic in OnThink() |
| Pack tactics | AI coordination in OnThink() |

### AI Types for Vystia Creatures

| Creature Type | Recommended AIType |
|---------------|-------------------|
| Melee fighters (giants, trolls, wolves) | `AIType.AI_Melee` |
| Magic users (liches, witches, mages) | `AIType.AI_Mage` |
| Necromancers (dark mages, liches) | `AIType.AI_NecroMage` |
| Animals (wolves, bears, spiders) | `AIType.AI_Animal` or `AI_Predator` |
| Archers (harpies with ranged) | `AIType.AI_Archer` |
| Healers (dryads, sprites) | `AIType.AI_Healer` |
| Berserkers (owlbears, trolls) | `AIType.AI_Berserk` |

### Poison Levels

| Bestiary Reference | ServUO Poison |
|-------------------|---------------|
| Minor poison | `Poison.Lesser` |
| Poison | `Poison.Regular` |
| Greater poison | `Poison.Greater` |
| Deadly poison | `Poison.Deadly` |
| Lethal poison | `Poison.Lethal` |

### Loot Packs (All Verified)

All loot packs in this bestiary exist in ServUO:
- `LootPack.Poor`, `LootPack.Meager`, `LootPack.Average`
- `LootPack.Rich`, `LootPack.FilthyRich`, `LootPack.UltraRich`
- `LootPack.Gems`, `LootPack.MedScrolls`, `LootPack.HighScrolls`
- `LootPack.SuperBoss` (for major bosses)

---

## EXPANSION BODY IDS REFERENCE

This bestiary uses bodies from ALL expansions for maximum visual variety:

| Expansion | Notable Bodies |
|-----------|----------------|
| T2A | 1-200 (classics: dragons, liches, elementals) |
| Third Dawn | 301-320 (golems, reapers, void creatures) |
| LBR | 700-750 (goblins, gargoyles, slashers) |
| ML | 750-800 (bog things, plague beasts, void) |
| SA | 800-850 (stygian dragon, primeval lich) |
| HS | 850-900 (krakens, sea creatures) |
| ToL | 1000+ (tigers, dinosaurs) |

---

# FROSTHOLD & WINTERGUARD CREATURES

### FROST GIANT
- **Body ID:** 76 (Titan)
- **Hue:** 1152 (Ice blue)
- **BaseSoundID:** 609
- **Stats:** STR 400-500, DEX 60-80, INT 40-60
- **HP:** 800-1000
- **Damage:** 25-35
- **Virtual Armor:** 48
- **Special:** Cold damage 25%, Unprovokable
- **Loot:** LootPack.FilthyRich, LootPack.Gems (8)
- **Taming:** Not tameable

### WINTER WOLF
- **Body ID:** 277 (White Wolf - newer art)
- **Hue:** 1150 (Snow white)
- **BaseSoundID:** 0xE5
- **Stats:** STR 120-150, DEX 80-100, INT 35-50
- **HP:** 85-100
- **Damage:** 14-18
- **Virtual Armor:** 32
- **Special:** Pack tactics, Cold damage 25%
- **Loot:** LootPack.Average
- **Taming:** Yes - MinTame 85.1, ControlSlots 1

### ICE TROLL
- **Body ID:** 53 (Troll)
- **Hue:** 1153 (Frost white)
- **BaseSoundID:** 461
- **Stats:** STR 200-250, DEX 45-65, INT 45-70
- **HP:** 150-200
- **Damage:** 14-20
- **Virtual Armor:** 40
- **Special:** Regeneration, Cold damage 25%
- **Loot:** LootPack.Average
- **Taming:** Not tameable

### FROST WRAITH
- **Body ID:** 26 (Wraith)
- **Hue:** 1152 (Ice blue)
- **BaseSoundID:** 0x482
- **Stats:** STR 100-120, DEX 75-95, INT 150-200
- **HP:** 80-100
- **Damage:** 11-13
- **Virtual Armor:** 28
- **Special:** Mana drain, Cold damage 50%
- **Loot:** LootPack.Meager
- **Taming:** Not tameable

### FROZEN HORROR
- **Body ID:** 312 (Abysmal Horror - SA art)
- **Hue:** 1151 (Pale ice)
- **BaseSoundID:** 0x48D
- **Stats:** STR 250-300, DEX 50-70, INT 50-70
- **HP:** 200-250
- **Damage:** 18-22
- **Virtual Armor:** 40
- **Special:** Cold aura, Freeze attack
- **Loot:** LootPack.Rich, LootPack.MedScrolls
- **Taming:** Not tameable

### FROST DRAGON
- **Body ID:** 12 (Dragon)
- **Hue:** 1152 (Ice blue)
- **BaseSoundID:** 362
- **Stats:** STR 796-825, DEX 86-105, INT 436-475
- **HP:** 478-495
- **Damage:** 16-22
- **Virtual Armor:** 60
- **Special:** Ice breath, Flight, Magic user
- **Loot:** LootPack.FilthyRich (2), LootPack.Gems (8)
- **Taming:** Yes - MinTame 104.7, ControlSlots 4

### ICE SPRITE
- **Body ID:** 58 (Wisp)
- **Hue:** 1150 (Ice white)
- **BaseSoundID:** 466
- **Stats:** STR 50-80, DEX 150-200, INT 100-150
- **HP:** 50-75
- **Damage:** 5-10
- **Virtual Armor:** 25
- **Special:** Magic user, Invisibility
- **Loot:** LootPack.Meager
- **Taming:** Not tameable

### ICE GOLEM
- **Body ID:** 752 (Golem - Third Dawn)
- **Hue:** 1152 (Ice blue)
- **BaseSoundID:** 541
- **Stats:** STR 251-350, DEX 76-100, INT 101-150
- **HP:** 250-350
- **Damage:** 18-25
- **Virtual Armor:** 50
- **Special:** Cold immunity, Slow aura
- **Loot:** LootPack.Rich
- **Taming:** Not tameable

### GLACIAL BEAR
- **Body ID:** 211 (Polar Bear)
- **Hue:** 1153 (Frost white)
- **BaseSoundID:** 0xA3
- **Stats:** STR 200-250, DEX 60-80, INT 20-40
- **HP:** 180-220
- **Damage:** 16-22
- **Virtual Armor:** 35
- **Special:** Cold damage, Maul attack
- **Loot:** LootPack.Average
- **Taming:** Yes - MinTame 75.0, ControlSlots 2

---

# EMBERLANDS CREATURES

### LAVA HOUND
- **Body ID:** 98 (Hell Hound)
- **Hue:** 1161 (Lava red)
- **BaseSoundID:** 0xE5
- **Stats:** STR 102-150, DEX 81-105, INT 36-60
- **HP:** 100-150
- **Damage:** 15-20
- **Virtual Armor:** 30
- **Special:** Fire breath (50 damage)
- **Loot:** LootPack.Average
- **Taming:** Yes - MinTame 85.5, ControlSlots 1

### MAGMA TROLL
- **Body ID:** 53 (Troll)
- **Hue:** 1358 (Molten orange)
- **BaseSoundID:** 461
- **Stats:** STR 250-300, DEX 50-70, INT 50-70
- **HP:** 200-250
- **Damage:** 18-24
- **Virtual Armor:** 35
- **Special:** Fire aura, Regeneration
- **Loot:** LootPack.Average
- **Taming:** Not tameable

### PHOENIX
- **Body ID:** 5 (Eagle)
- **Hue:** 1358 (Fire orange)
- **BaseSoundID:** 0x8F
- **Stats:** STR 504-700, DEX 202-300, INT 504-700
- **HP:** 340-383
- **Damage:** 15-25
- **Virtual Armor:** 60
- **Special:** Fire breath (75 damage), Resurrection on death
- **Loot:** LootPack.FilthyRich, LootPack.Gems (5)
- **Taming:** Not tameable

### VOLCANO WYRM
- **Body ID:** 46 (Ancient Wyrm)
- **Hue:** 1359 (Deep red)
- **BaseSoundID:** 362
- **Stats:** STR 900-1000, DEX 100-120, INT 500-600
- **HP:** 600-700
- **Damage:** 20-30
- **Virtual Armor:** 70
- **Special:** Massive fire breath (90 damage), Lava pools
- **Loot:** LootPack.FilthyRich (2), LootPack.Gems (8)
- **Taming:** Not tameable

### EMBER BAT
- **Body ID:** 317 (Giant Bat - Third Dawn)
- **Hue:** 1160 (Ember red)
- **BaseSoundID:** 0x270
- **Stats:** STR 30-50, DEX 90-120, INT 10-20
- **HP:** 30-40
- **Damage:** 5-8
- **Virtual Armor:** 15
- **Special:** Swarm tactics, Fire bite
- **Loot:** LootPack.Poor
- **Taming:** Yes - MinTame 60.0, ControlSlots 1

### ASH GOLEM
- **Body ID:** 752 (Golem - Third Dawn)
- **Hue:** 1109 (Ash grey)
- **BaseSoundID:** 541
- **Stats:** STR 200-250, DEX 40-60, INT 40-60
- **HP:** 180-220
- **Damage:** 15-20
- **Virtual Armor:** 35
- **Special:** Fire immunity, Ash cloud
- **Loot:** LootPack.Average, LootPack.Gems (3)
- **Taming:** Not tameable

### FLAME SPRITE
- **Body ID:** 58 (Wisp)
- **Hue:** 1360 (Bright flame)
- **BaseSoundID:** 466
- **Stats:** STR 60-90, DEX 120-150, INT 80-100
- **HP:** 40-60
- **Damage:** 5-8
- **Virtual Armor:** 20
- **Special:** Fire magic, Ignite
- **Loot:** LootPack.Meager
- **Taming:** Not tameable

### LAVA ELEMENTAL
- **Body ID:** 720 (Lava Elemental - SA)
- **Hue:** 0 (Default)
- **BaseSoundID:** 268
- **Stats:** STR 250-300, DEX 80-100, INT 100-150
- **HP:** 200-280
- **Damage:** 15-22
- **Virtual Armor:** 40
- **Special:** Fire damage 100%, Lava pool on death
- **Loot:** LootPack.Rich, LootPack.Gems (5)
- **Taming:** Not tameable

### FIRE ANT
- **Body ID:** 738 (Fire Ant - SA)
- **Hue:** 0 (Default)
- **BaseSoundID:** 0x5A
- **Stats:** STR 100-150, DEX 80-100, INT 20-40
- **HP:** 80-120
- **Damage:** 10-15
- **Virtual Armor:** 25
- **Special:** Fire spit, Swarm tactics
- **Loot:** LootPack.Meager
- **Taming:** Yes - MinTame 65.0, ControlSlots 1

---

# WHISPERING SANDS CREATURES

### SAND ELEMENTAL
- **Body ID:** 14 (Earth Elemental)
- **Hue:** 2305 (Sandy tan)
- **BaseSoundID:** 268
- **Stats:** STR 126-155, DEX 66-85, INT 71-92
- **HP:** 150-200
- **Damage:** 12-18
- **Virtual Armor:** 34
- **Special:** Sandstorm, Blind
- **Loot:** LootPack.Average, LootPack.Gems (3)
- **Taming:** Not tameable

### DESERT MUMMY
- **Body ID:** 154 (Mummy)
- **Hue:** 2309 (Dusty brown)
- **BaseSoundID:** 471
- **Stats:** STR 200-250, DEX 40-60, INT 40-60
- **HP:** 200-250
- **Damage:** 15-20
- **Virtual Armor:** 35
- **Special:** Curse, Disease
- **Loot:** LootPack.Rich, LootPack.MedScrolls
- **Taming:** Not tameable

### GIANT SCORPION
- **Body ID:** 48 (Scorpion)
- **Hue:** 2307 (Desert brown)
- **BaseSoundID:** 397
- **Stats:** STR 73-115, DEX 76-95, INT 16-30
- **HP:** 100-150
- **Damage:** 10-15
- **Virtual Armor:** 28
- **Special:** Greater poison
- **Loot:** LootPack.Meager
- **Taming:** Yes - MinTame 80.0, ControlSlots 1

### SPHINX
- **Body ID:** 788 (Stone Harpy - Third Dawn)
- **Hue:** 2306 (Sandstone)
- **BaseSoundID:** 402
- **Stats:** STR 300-400, DEX 100-150, INT 200-300
- **HP:** 350-400
- **Damage:** 18-25
- **Virtual Armor:** 45
- **Special:** Riddles (gump), Magic resistance
- **Loot:** LootPack.FilthyRich, LootPack.MedScrolls (2)
- **Taming:** Not tameable

### DESERT HARPY
- **Body ID:** 30 (Harpy)
- **Hue:** 2308 (Sun-bleached)
- **BaseSoundID:** 402
- **Stats:** STR 100-150, DEX 120-160, INT 80-100
- **HP:** 80-120
- **Damage:** 10-15
- **Virtual Armor:** 25
- **Special:** Flight, Screech
- **Loot:** LootPack.Meager (2)
- **Taming:** Not tameable

### ANKHEG
- **Body ID:** 787 (Ant Lion - LBR)
- **Hue:** 2304 (Sand yellow)
- **BaseSoundID:** 0x5A
- **Stats:** STR 200-250, DEX 60-80, INT 20-40
- **HP:** 150-200
- **Damage:** 15-20
- **Virtual Armor:** 35
- **Special:** Burrow, Acid spit
- **Loot:** LootPack.Average
- **Taming:** Not tameable

### SAND VORTEX
- **Body ID:** 790 (Sand Vortex - LBR)
- **Hue:** 0 (Default)
- **BaseSoundID:** 0x10C
- **Stats:** STR 150-200, DEX 150-200, INT 50-80
- **HP:** 120-180
- **Damage:** 12-18
- **Virtual Armor:** 30
- **Special:** Sandstorm AoE, Blind
- **Loot:** LootPack.Average
- **Taming:** Not tameable

### SCARAB BEETLE
- **Body ID:** 714 (Iron Beetle - SA base)
- **Hue:** 2305 (Sand)
- **BaseSoundID:** 0x21D
- **Stats:** STR 150-200, DEX 60-80, INT 20-40
- **HP:** 100-150
- **Damage:** 12-18
- **Virtual Armor:** 40
- **Special:** Hardened shell, Acid spray
- **Loot:** LootPack.Average
- **Taming:** Yes - MinTame 75.0, ControlSlots 2

---

# SHADOWFEN CREATURES

### SWAMP TROLL
- **Body ID:** 53 (Troll)
- **Hue:** 2212 (Swamp green)
- **BaseSoundID:** 461
- **Stats:** STR 180-220, DEX 50-70, INT 50-70
- **HP:** 180-220
- **Damage:** 16-22
- **Virtual Armor:** 35
- **Special:** Poison attack, Regeneration
- **Loot:** LootPack.Average
- **Taming:** Not tameable

### BOG WITCH
- **Body ID:** 87 (Evil Mage Female)
- **Hue:** 2213 (Bog brown)
- **BaseSoundID:** 0x4B0
- **Stats:** STR 80-100, DEX 80-100, INT 200-250
- **HP:** 150-200
- **Damage:** 10-15
- **Virtual Armor:** 25
- **Special:** Poison spells, Curse
- **Loot:** LootPack.Rich, LootPack.MedScrolls (2)
- **Taming:** Not tameable

### MISTWALKER
- **Body ID:** 740 (Dream Wraith - SA)
- **Hue:** 2214 (Fog grey)
- **BaseSoundID:** 0x482
- **Stats:** STR 100-130, DEX 100-120, INT 100-150
- **HP:** 100-130
- **Damage:** 12-16
- **Virtual Armor:** 28
- **Special:** Ethereal, Confusion
- **Loot:** LootPack.Meager
- **Taming:** Not tameable

### BOG BEAST
- **Body ID:** 780 (Bog Thing - LBR)
- **Hue:** 0 (Default green)
- **BaseSoundID:** 0x165
- **Stats:** STR 200-250, DEX 50-70, INT 30-50
- **HP:** 180-220
- **Damage:** 18-24
- **Virtual Armor:** 35
- **Special:** Swamp fever, Stench, Spawns Boglings
- **Loot:** LootPack.Average
- **Taming:** Not tameable

### SHADOW WOLF
- **Body ID:** 739 (Leather Wolf - SA)
- **Hue:** 1109 (Shadow black)
- **BaseSoundID:** 0xE5
- **Stats:** STR 100-150, DEX 90-120, INT 50-75
- **HP:** 75-100
- **Damage:** 11-17
- **Virtual Armor:** 30
- **Special:** Stealth, Pack tactics
- **Loot:** LootPack.Average
- **Taming:** Yes - MinTame 83.1, ControlSlots 1

### BOGLING
- **Body ID:** 779 (Bogling - LBR)
- **Hue:** 0 (Default)
- **BaseSoundID:** 0x165
- **Stats:** STR 50-80, DEX 80-100, INT 20-40
- **HP:** 40-60
- **Damage:** 5-10
- **Virtual Armor:** 15
- **Special:** Poison, Swarm
- **Loot:** LootPack.Poor
- **Taming:** Not tameable

### PLAGUE BEAST
- **Body ID:** 775 (Plague Beast - LBR)
- **Hue:** 2212 (Swamp)
- **BaseSoundID:** 0x165
- **Stats:** STR 200-280, DEX 40-60, INT 40-60
- **HP:** 200-280
- **Damage:** 15-22
- **Virtual Armor:** 40
- **Special:** Disease cloud, Poison immunity
- **Loot:** LootPack.Rich
- **Taming:** Not tameable

### QUAGMIRE
- **Body ID:** 789 (Quagmire - LBR)
- **Hue:** 0 (Default)
- **BaseSoundID:** 0x165
- **Stats:** STR 150-200, DEX 40-60, INT 60-80
- **HP:** 150-200
- **Damage:** 12-18
- **Virtual Armor:** 30
- **Special:** Root trap, Poison cloud
- **Loot:** LootPack.Average
- **Taming:** Not tameable

---

# VERDANTPEAK CREATURES

### TREANT
- **Body ID:** 301 (Reaper - Third Dawn - better tree look)
- **Hue:** 2010 (Forest green)
- **BaseSoundID:** 442
- **Stats:** STR 300-400, DEX 30-50, INT 100-150
- **HP:** 300-400
- **Damage:** 20-25
- **Virtual Armor:** 45
- **Special:** Poison immunity, Root attack
- **Loot:** LootPack.Rich, LootPack.Gems (5)
- **Taming:** Not tameable

### DIRE WOLF
- **Body ID:** 23 (Dire Wolf)
- **Hue:** 2011 (Dark forest)
- **BaseSoundID:** 0xE5
- **Stats:** STR 120-150, DEX 80-100, INT 40-60
- **HP:** 120-150
- **Damage:** 15-20
- **Virtual Armor:** 32
- **Special:** Pack leader, Howl buff
- **Loot:** LootPack.Average
- **Taming:** Yes - MinTame 90.0, ControlSlots 1

### FOREST SPRITE
- **Body ID:** 128 (Pixie)
- **Hue:** 2012 (Leaf green)
- **BaseSoundID:** 0x467
- **Stats:** STR 40-60, DEX 120-150, INT 100-150
- **HP:** 40-60
- **Damage:** 4-7
- **Virtual Armor:** 20
- **Special:** Nature magic, Healing aura
- **Loot:** LootPack.Meager
- **Taming:** Not tameable

### GIANT SPIDER
- **Body ID:** 20 (Giant Spider)
- **Hue:** 2010 (Forest green)
- **BaseSoundID:** 0x388
- **Stats:** STR 100-150, DEX 80-100, INT 30-50
- **HP:** 80-120
- **Damage:** 10-15
- **Virtual Armor:** 25
- **Special:** Web, Poison
- **Loot:** LootPack.Poor
- **Taming:** Yes - MinTame 70.0, ControlSlots 1

### SHADOW DRYAD
- **Body ID:** 266 (Dryad - SA)
- **Hue:** 1109 (Dark)
- **BaseSoundID:** 0x467
- **Stats:** STR 80-100, DEX 100-120, INT 150-200
- **HP:** 100-150
- **Damage:** 8-12
- **Virtual Armor:** 25
- **Special:** Nature curse, Thorn volley
- **Loot:** LootPack.Rich, LootPack.MedScrolls (2)
- **Taming:** Not tameable

### CORRUPTED TREANT
- **Body ID:** 301 (Reaper - Third Dawn)
- **Hue:** 1109 (Dead wood)
- **BaseSoundID:** 442
- **Stats:** STR 250-350, DEX 30-50, INT 80-120
- **HP:** 250-350
- **Damage:** 18-25
- **Virtual Armor:** 40
- **Special:** Disease, Rot touch
- **Loot:** LootPack.Rich, LootPack.Gems (3)
- **Taming:** Not tameable

### THORN BEAST
- **Body ID:** 8 (Corpser)
- **Hue:** 2010 (Forest green)
- **BaseSoundID:** 684
- **Stats:** STR 150-200, DEX 20-40, INT 20-40
- **HP:** 150-200
- **Damage:** 15-20
- **Virtual Armor:** 35
- **Special:** Thorn volley, Root trap
- **Loot:** LootPack.Average
- **Taming:** Not tameable

### WOLF SPIDER
- **Body ID:** 736 (Wolf Spider - SA)
- **Hue:** 2010 (Forest)
- **BaseSoundID:** 0x388
- **Stats:** STR 150-200, DEX 100-130, INT 30-50
- **HP:** 100-150
- **Damage:** 12-18
- **Virtual Armor:** 30
- **Special:** Leaping attack, Deadly poison
- **Loot:** LootPack.Average
- **Taming:** Yes - MinTame 82.0, ControlSlots 2

### FAIRY DRAGON
- **Body ID:** 718 (Fairy Dragon - SA)
- **Hue:** 2012 (Forest green)
- **BaseSoundID:** 0x8F
- **Stats:** STR 200-280, DEX 150-200, INT 200-280
- **HP:** 150-220
- **Damage:** 12-18
- **Virtual Armor:** 35
- **Special:** Nature magic, Polymorph, Flight
- **Loot:** LootPack.Rich, LootPack.Gems (5)
- **Taming:** Yes - MinTame 98.0, ControlSlots 2

---

# CRYSTAL BARRENS CREATURES

### CRYSTAL DRAKE
- **Body ID:** 60 (Drake)
- **Hue:** 1154 (Crystal blue)
- **BaseSoundID:** 362
- **Stats:** STR 400-500, DEX 100-150, INT 200-300
- **HP:** 250-300
- **Damage:** 15-22
- **Virtual Armor:** 40
- **Special:** Energy damage, Reflection
- **Loot:** LootPack.FilthyRich, LootPack.Gems (8)
- **Taming:** Not tameable

### GEMSTONE GOLEM
- **Body ID:** 752 (Golem - Third Dawn)
- **Hue:** 1155 (Prismatic)
- **BaseSoundID:** 541
- **Stats:** STR 200-250, DEX 40-60, INT 100-150
- **HP:** 200-250
- **Damage:** 18-25
- **Virtual Armor:** 50
- **Special:** Spell reflection, Hardened
- **Loot:** LootPack.Rich, LootPack.Gems (10)
- **Taming:** Not tameable

### LIGHT ELEMENTAL
- **Body ID:** 165 (Wisp variant)
- **Hue:** 1156 (Bright white)
- **BaseSoundID:** 466
- **Stats:** STR 100-150, DEX 150-200, INT 150-200
- **HP:** 120-150
- **Damage:** 10-15
- **Virtual Armor:** 30
- **Special:** Energy burst, Blind
- **Loot:** LootPack.Meager
- **Taming:** Not tameable

### CRYSTAL SPIDER
- **Body ID:** 737 (Trapdoor Spider - SA)
- **Hue:** 1154 (Crystal blue)
- **BaseSoundID:** 0x388
- **Stats:** STR 150-200, DEX 100-150, INT 50-75
- **HP:** 100-150
- **Damage:** 10-15
- **Virtual Armor:** 40
- **Special:** Energy poison, Crystal web
- **Loot:** LootPack.Average
- **Taming:** Not tameable

### PRISMATIC WYRM
- **Body ID:** 46 (Ancient Wyrm)
- **Hue:** 1155 (Prismatic)
- **BaseSoundID:** 362
- **Stats:** STR 700-850, DEX 100-130, INT 400-500
- **HP:** 500-650
- **Damage:** 18-26
- **Virtual Armor:** 65
- **Special:** All elemental damage, Prismatic breath
- **Loot:** LootPack.FilthyRich (2), LootPack.Gems (12)
- **Taming:** Not tameable

---

# DEEPFORGE CREATURES

### CAVE TROLL
- **Body ID:** 53 (Troll)
- **Hue:** 1109 (Stone grey)
- **BaseSoundID:** 461
- **Stats:** STR 200-250, DEX 50-70, INT 40-60
- **HP:** 200-250
- **Damage:** 16-22
- **Virtual Armor:** 40
- **Special:** Stone skin, Darkvision
- **Loot:** LootPack.Average
- **Taming:** Not tameable

### STONE GIANT
- **Body ID:** 76 (Titan)
- **Hue:** 1110 (Granite)
- **BaseSoundID:** 609
- **Stats:** STR 400-500, DEX 40-60, INT 60-80
- **HP:** 400-500
- **Damage:** 22-30
- **Virtual Armor:** 50
- **Special:** Boulder throw, Earthquake
- **Loot:** LootPack.FilthyRich, LootPack.Gems (8)
- **Taming:** Not tameable

### FIRE ELEMENTAL
- **Body ID:** 15 (Fire Elemental)
- **Hue:** 0 (Default)
- **BaseSoundID:** 838
- **Stats:** STR 126-155, DEX 166-185, INT 171-192
- **HP:** 100-150
- **Damage:** 10-15
- **Virtual Armor:** 30
- **Special:** Fire damage 100%, Burn
- **Loot:** LootPack.Average, LootPack.Gems (3)
- **Taming:** Not tameable

### SALAMANDER
- **Body ID:** 35 (Lizardman)
- **Hue:** 1161 (Fire red)
- **BaseSoundID:** 417
- **Stats:** STR 150-200, DEX 80-100, INT 50-70
- **HP:** 120-150
- **Damage:** 12-18
- **Virtual Armor:** 30
- **Special:** Fire immunity, Heat aura
- **Loot:** LootPack.Average
- **Taming:** Not tameable

### MAGMA GOLEM
- **Body ID:** 752 (Golem - Third Dawn)
- **Hue:** 1358 (Molten)
- **BaseSoundID:** 541
- **Stats:** STR 300-400, DEX 50-70, INT 50-75
- **HP:** 280-380
- **Damage:** 18-26
- **Virtual Armor:** 50
- **Special:** Fire immunity, Lava trail
- **Loot:** LootPack.Rich, LootPack.Gems (5)
- **Taming:** Not tameable

### CLOCKWORK SCORPION
- **Body ID:** 717 (Clockwork Scorpion - SA)
- **Hue:** 0 (Default)
- **BaseSoundID:** 0x5A
- **Stats:** STR 150-200, DEX 100-130, INT 50-75
- **HP:** 120-180
- **Damage:** 12-18
- **Virtual Armor:** 35
- **Special:** Mechanical, Poison sting
- **Loot:** LootPack.Average
- **Taming:** Not tameable

---

# IRONCLAD EMPIRE CREATURES

### IRON GOLEM
- **Body ID:** 752 (Golem - Third Dawn)
- **Hue:** 2406 (Iron grey)
- **BaseSoundID:** 541
- **Stats:** STR 251-350, DEX 76-100, INT 101-150
- **HP:** 250-350
- **Damage:** 18-25
- **Virtual Armor:** 55
- **Special:** Bleed immune, Poison immune, Rust vulnerability
- **Loot:** LootPack.Rich
- **Taming:** Not tameable

### CLOCKWORK BEAST
- **Body ID:** 768 (Juggernaut - LBR)
- **Hue:** 2407 (Copper)
- **BaseSoundID:** 541
- **Stats:** STR 150-200, DEX 100-120, INT 50-70
- **HP:** 150-200
- **Damage:** 14-20
- **Virtual Armor:** 40
- **Special:** Self-repair, Steam attack
- **Loot:** LootPack.Average
- **Taming:** Not tameable

### STEAMWORK GOLEM
- **Body ID:** 829 (Rising Colossus - SA)
- **Hue:** 2401 (Bronze)
- **BaseSoundID:** 541
- **Stats:** STR 300-400, DEX 40-60, INT 50-75
- **HP:** 280-380
- **Damage:** 20-28
- **Virtual Armor:** 55
- **Special:** Steam cloud, Repair protocol
- **Loot:** LootPack.Rich
- **Taming:** Not tameable

### MECHANICAL SPIDER
- **Body ID:** 717 (Clockwork Scorpion base - SA)
- **Hue:** 2406 (Iron grey)
- **BaseSoundID:** 0x388
- **Stats:** STR 100-150, DEX 120-150, INT 30-50
- **HP:** 100-150
- **Damage:** 10-15
- **Virtual Armor:** 35
- **Special:** Metal web, Oil spray
- **Loot:** LootPack.Meager
- **Taming:** Not tameable

### STEAM ELEMENTAL
- **Body ID:** 13 (Air Elemental)
- **Hue:** 2401 (Bronze steam)
- **BaseSoundID:** 655
- **Stats:** STR 150-200, DEX 100-150, INT 80-120
- **HP:** 150-200
- **Damage:** 12-18
- **Virtual Armor:** 30
- **Special:** Steam cloud, Burn, Obscure vision
- **Loot:** LootPack.Average, LootPack.Gems (3)
- **Taming:** Not tameable

### JUGGERNAUT
- **Body ID:** 768 (Juggernaut - LBR)
- **Hue:** 0 (Default)
- **BaseSoundID:** 541
- **Stats:** STR 300-400, DEX 60-80, INT 80-120
- **HP:** 280-380
- **Damage:** 18-26
- **Virtual Armor:** 60
- **Special:** Unstoppable charge, Stun
- **Loot:** LootPack.FilthyRich
- **Taming:** Not tameable

---

# SKY/MOUNTAIN CREATURES

### GRIFFIN
- **Body ID:** 5 (Eagle)
- **Hue:** 0 (Natural)
- **BaseSoundID:** 0x8F
- **Stats:** STR 200-250, DEX 120-150, INT 80-100
- **HP:** 150-200
- **Damage:** 12-18
- **Virtual Armor:** 30
- **Special:** Flight, Dive attack
- **Loot:** LootPack.Rich
- **Taming:** Yes - MinTame 95.0, ControlSlots 2

### HARPY
- **Body ID:** 30 (Harpy)
- **Hue:** 0 (Natural)
- **BaseSoundID:** 402
- **Stats:** STR 100-150, DEX 100-130, INT 60-80
- **HP:** 80-120
- **Damage:** 10-15
- **Virtual Armor:** 25
- **Special:** Flight, Screech
- **Loot:** LootPack.Meager (2)
- **Taming:** Not tameable

### STONE HARPY
- **Body ID:** 788 (Stone Harpy - Third Dawn)
- **Hue:** 0 (Default stone)
- **BaseSoundID:** 402
- **Stats:** STR 150-200, DEX 80-100, INT 60-80
- **HP:** 120-160
- **Damage:** 12-18
- **Virtual Armor:** 40
- **Special:** Stone form, Flight
- **Loot:** LootPack.Average
- **Taming:** Not tameable

### AIR ELEMENTAL
- **Body ID:** 13 (Air Elemental)
- **Hue:** 0 (Natural)
- **BaseSoundID:** 655
- **Stats:** STR 126-155, DEX 166-185, INT 101-132
- **HP:** 100-150
- **Damage:** 10-15
- **Virtual Armor:** 28
- **Special:** Lightning, Whirlwind
- **Loot:** LootPack.Average, LootPack.Gems (3)
- **Taming:** Not tameable

### GARGOYLE
- **Body ID:** 4 (Gargoyle)
- **Hue:** 0 (Natural)
- **BaseSoundID:** 372
- **Stats:** STR 146-175, DEX 76-95, INT 81-105
- **HP:** 100-150
- **Damage:** 10-15
- **Virtual Armor:** 30
- **Special:** Stone form, Flight
- **Loot:** LootPack.Average
- **Taming:** Not tameable

### STYGIAN GARGOYLE
- **Body ID:** 730 (Raptor/Gargoyle - SA)
- **Hue:** 1109 (Dark)
- **BaseSoundID:** 372
- **Stats:** STR 250-320, DEX 100-130, INT 150-200
- **HP:** 200-280
- **Damage:** 15-22
- **Virtual Armor:** 45
- **Special:** Dark magic, Flight, Stone form
- **Loot:** LootPack.Rich
- **Taming:** Not tameable

### WIND DANCER
- **Body ID:** 30 (Harpy)
- **Hue:** 1001 (Light grey)
- **BaseSoundID:** 402
- **Stats:** STR 100-150, DEX 150-200, INT 100-150
- **HP:** 80-120
- **Damage:** 8-12
- **Virtual Armor:** 30
- **Special:** Wind magic, Flight
- **Loot:** LootPack.Meager (2)
- **Taming:** Not tameable

### STORM CROW
- **Body ID:** 6 (Bird)
- **Hue:** 1109 (Storm grey)
- **BaseSoundID:** 0x2EE
- **Stats:** STR 50-75, DEX 100-125, INT 30-50
- **HP:** 30-45
- **Damage:** 5-7
- **Virtual Armor:** 15
- **Special:** Lightning peck, Flight
- **Loot:** LootPack.Poor
- **Taming:** Yes - MinTame 35.1, ControlSlots 1

### THUNDER BIRD
- **Body ID:** 5 (Eagle)
- **Hue:** 1109 (Storm grey)
- **BaseSoundID:** 0x8F
- **Stats:** STR 200-250, DEX 100-150, INT 100-150
- **HP:** 150-200
- **Damage:** 15-20
- **Virtual Armor:** 35
- **Special:** Lightning strike, Storm call
- **Loot:** LootPack.Rich, LootPack.Gems (3)
- **Taming:** Not tameable

### STORM ELEMENTAL
- **Body ID:** 13 (Air Elemental)
- **Hue:** 1109 (Dark grey)
- **BaseSoundID:** 655
- **Stats:** STR 150-200, DEX 150-200, INT 100-150
- **HP:** 150-200
- **Damage:** 12-18
- **Virtual Armor:** 35
- **Special:** Lightning bolt, Thunder AoE
- **Loot:** LootPack.Average, LootPack.Gems (5)
- **Taming:** Not tameable

### LIGHTNING GOLEM
- **Body ID:** 752 (Golem - Third Dawn)
- **Hue:** 1001 (Electric blue)
- **BaseSoundID:** 541
- **Stats:** STR 200-250, DEX 100-120, INT 80-100
- **HP:** 200-250
- **Damage:** 15-22
- **Virtual Armor:** 40
- **Special:** Lightning field, Chain lightning
- **Loot:** LootPack.Average, LootPack.Gems (5)
- **Taming:** Not tameable

---

# UNDERWATER CREATURES

### SEA SERPENT
- **Body ID:** 150 (Sea Serpent)
- **Hue:** 0 (Natural)
- **BaseSoundID:** 447
- **Stats:** STR 168-225, DEX 58-85, INT 53-95
- **HP:** 110-127
- **Damage:** 10-14
- **Virtual Armor:** 30
- **Special:** Aquatic, Poison
- **Loot:** LootPack.Average
- **Taming:** Not tameable

### DEEP SEA SERPENT
- **Body ID:** 150 (Sea Serpent)
- **Hue:** 1367 (Deep blue)
- **BaseSoundID:** 447
- **Stats:** STR 200-250, DEX 60-80, INT 60-80
- **HP:** 150-200
- **Damage:** 12-18
- **Virtual Armor:** 35
- **Special:** Aquatic, Greater poison
- **Loot:** LootPack.Average
- **Taming:** Not tameable

### KRAKEN
- **Body ID:** 77 (Kraken)
- **Hue:** 1367 (Deep sea)
- **BaseSoundID:** 353
- **Stats:** STR 500-600, DEX 60-80, INT 100-150
- **HP:** 500-600
- **Damage:** 20-30
- **Virtual Armor:** 50
- **Special:** Tentacle attack, Ink cloud, Grapple
- **Loot:** LootPack.FilthyRich (2), LootPack.Gems (8)
- **Taming:** Not tameable

### WATER ELEMENTAL
- **Body ID:** 16 (Water Elemental)
- **Hue:** 0 (Natural)
- **BaseSoundID:** 278
- **Stats:** STR 126-155, DEX 66-85, INT 101-125
- **HP:** 100-150
- **Damage:** 10-15
- **Virtual Armor:** 30
- **Special:** Water attack, Drown
- **Loot:** LootPack.Average, LootPack.Gems (3)
- **Taming:** Not tameable

### DEEP DWELLER
- **Body ID:** 77 (Kraken)
- **Hue:** 1367 (Deep sea blue)
- **BaseSoundID:** 353
- **Stats:** STR 200-250, DEX 60-80, INT 40-60
- **HP:** 150-200
- **Damage:** 12-18
- **Virtual Armor:** 35
- **Special:** Aquatic, Tentacles
- **Loot:** LootPack.Average
- **Taming:** Not tameable

### DROWNED DEAD
- **Body ID:** 154 (Mummy)
- **Hue:** 1367 (Sea blue)
- **BaseSoundID:** 471
- **Stats:** STR 150-200, DEX 40-60, INT 40-60
- **HP:** 150-200
- **Damage:** 12-18
- **Virtual Armor:** 30
- **Special:** Drowning touch, Undead
- **Loot:** LootPack.Rich, LootPack.MedScrolls
- **Taming:** Not tameable

### LEVIATHAN
- **Body ID:** 77 (Kraken)
- **Hue:** 1109 (Dark)
- **BaseSoundID:** 353
- **Stats:** STR 700-850, DEX 70-90, INT 150-200
- **HP:** 600-800
- **Damage:** 22-32
- **Virtual Armor:** 65
- **Special:** Massive AoE, Tidal wave, Grapple
- **Loot:** LootPack.FilthyRich (2), LootPack.Gems (10)
- **Taming:** Not tameable

---

# SHADOW/VOID CREATURES

### SHADOW WISP
- **Body ID:** 165 (Wisp variant)
- **Hue:** 1109 (Shadow black)
- **BaseSoundID:** 466
- **Stats:** STR 50-80, DEX 150-200, INT 100-150
- **HP:** 50-80
- **Damage:** 5-10
- **Virtual Armor:** 25
- **Special:** Ethereal, Shadow magic
- **Loot:** LootPack.Meager
- **Taming:** Not tameable

### SHADE
- **Body ID:** 26 (Wraith)
- **Hue:** 1109 (Pure black)
- **BaseSoundID:** 0x482
- **Stats:** STR 100-120, DEX 100-120, INT 100-120
- **HP:** 80-100
- **Damage:** 10-14
- **Virtual Armor:** 28
- **Special:** Life drain, Invisible
- **Loot:** LootPack.Meager
- **Taming:** Not tameable

### SHADOW KNIGHT
- **Body ID:** 311 (Shadow Knight - Third Dawn)
- **Hue:** 0 (Default shadow)
- **BaseSoundID:** 451
- **Stats:** STR 200-250, DEX 80-100, INT 50-70
- **HP:** 200-250
- **Damage:** 15-20
- **Virtual Armor:** 45
- **Special:** Shadow strike, Fear aura
- **Loot:** LootPack.Rich, LootPack.MedScrolls
- **Taming:** Not tameable

### ANCIENT LICH
- **Body ID:** 78 (Ancient Lich)
- **Hue:** 1109 (Shadow)
- **BaseSoundID:** 0x3E9
- **Stats:** STR 400-500, DEX 100-120, INT 500-600
- **HP:** 400-500
- **Damage:** 15-20
- **Virtual Armor:** 50
- **Special:** Master necromancer, Death magic, Summon undead
- **Loot:** LootPack.FilthyRich, LootPack.MedScrolls (3)
- **Taming:** Not tameable

### SHADOW FIEND
- **Body ID:** 9 (Daemon)
- **Hue:** 1109 (Pure black)
- **BaseSoundID:** 357
- **Stats:** STR 300-400, DEX 100-120, INT 200-250
- **HP:** 300-400
- **Damage:** 18-25
- **Virtual Armor:** 45
- **Special:** Shadow flame, Darkness aura
- **Loot:** LootPack.Rich, LootPack.MedScrolls (2)
- **Taming:** Not tameable

### VOID WALKER
- **Body ID:** 740 (Dream Wraith - SA)
- **Hue:** 1109 (Void black)
- **BaseSoundID:** 0x482
- **Stats:** STR 150-200, DEX 100-150, INT 150-200
- **HP:** 150-200
- **Damage:** 12-18
- **Virtual Armor:** 35
- **Special:** Phase shift, Void touch, Teleport
- **Loot:** LootPack.Average
- **Taming:** Not tameable

### DARK MAGE
- **Body ID:** 124 (Evil Mage)
- **Hue:** 1109 (Black robes)
- **BaseSoundID:** 0x45A
- **Stats:** STR 80-100, DEX 80-100, INT 300-400
- **HP:** 150-200
- **Damage:** 8-12
- **Virtual Armor:** 25
- **Special:** Dark magic, Curse master
- **Loot:** LootPack.Rich, LootPack.MedScrolls (2)
- **Taming:** Not tameable

### PRIMEVAL LICH
- **Body ID:** 830 (Primeval Lich - SA)
- **Hue:** 0 (Default)
- **BaseSoundID:** 0x3E9
- **Stats:** STR 600-750, DEX 120-150, INT 700-850
- **HP:** 600-800
- **Damage:** 20-28
- **Virtual Armor:** 60
- **Special:** Ancient necromancy, Mass summon, Death aura
- **Loot:** LootPack.FilthyRich (2), LootPack.Gems (10)
- **Taming:** Not tameable

### SLASHER OF VEILS
- **Body ID:** 741 (Slasher - SA)
- **Hue:** 1109 (Void)
- **BaseSoundID:** 0x482
- **Stats:** STR 400-500, DEX 150-200, INT 200-300
- **HP:** 350-450
- **Damage:** 18-26
- **Virtual Armor:** 55
- **Special:** Reality tear, Phase attack
- **Loot:** LootPack.FilthyRich, LootPack.Gems (8)
- **Taming:** Not tameable

---

# DUNGEON BOSSES

### FROST FATHER (Frozen Halls Boss)
- **Body ID:** 76 (Titan)
- **Hue:** 1152 (Ice blue)
- **BaseSoundID:** 609
- **Stats:** STR 600-700, DEX 60-80, INT 100-150
- **HP:** 1200-1500
- **Damage:** 35-45
- **Virtual Armor:** 70
- **Special:** Frost shield phases, Summons Frost Wraiths, Cone freeze
- **Loot:** LootPack.FilthyRich (3), LootPack.Gems (12)
- **Taming:** Not tameable

### DEEPFORGE BEHEMOTH (Deepforge Mines Boss)
- **Body ID:** 829 (Rising Colossus - SA)
- **Hue:** 1177 (Dark metallic)
- **BaseSoundID:** 541
- **Stats:** STR 700-800, DEX 50-70, INT 80-120
- **HP:** 1500-1800
- **Damage:** 40-50
- **Virtual Armor:** 80
- **Special:** Magma burst AoE, Forge empowerment, Stun stomp
- **Loot:** LootPack.FilthyRich (3), LootPack.Gems (12)
- **Taming:** Not tameable

### SPHINX OF SURYA (Pyramid Boss)
- **Body ID:** 788 (Stone Harpy - Third Dawn)
- **Hue:** 2306 (Sandstone)
- **BaseSoundID:** 402
- **Stats:** STR 500-600, DEX 120-150, INT 300-400
- **HP:** 800-1000
- **Damage:** 25-35
- **Virtual Armor:** 60
- **Special:** Riddle gump, Wrong answers spawn adds, Sandstorm blind
- **Loot:** LootPack.FilthyRich (3), LootPack.Gems (12)
- **Taming:** Not tameable

### VOLCANO WYRM (Emberdeep Caldera Boss)
- **Body ID:** 826 (Stygian Dragon - SA)
- **Hue:** 1359 (Deep red)
- **BaseSoundID:** 362
- **Stats:** STR 800-900, DEX 80-100, INT 200-300
- **HP:** 1000-1200
- **Damage:** 35-45
- **Virtual Armor:** 80
- **Special:** Lava pools, Fire breath, Periodic eruption
- **Loot:** LootPack.FilthyRich (3), LootPack.Gems (12)
- **Taming:** Not tameable

### COVEN MATRIARCH (Shadowfen Crypts Boss)
- **Body ID:** 87 (Evil Mage Female)
- **Hue:** 2213 (Bog brown)
- **BaseSoundID:** 0x4B0
- **Stats:** STR 200-250, DEX 100-120, INT 400-500
- **HP:** 600-800
- **Damage:** 20-30
- **Virtual Armor:** 50
- **Special:** Curse aura, Fear pulses, Mirror images
- **Loot:** LootPack.FilthyRich (3), LootPack.Gems (12)
- **Taming:** Not tameable

### ANCIENT TREANT (Verdant Depths Boss)
- **Body ID:** 301 (Reaper - Third Dawn)
- **Hue:** 2010 (Forest green)
- **BaseSoundID:** 442
- **Stats:** STR 500-600, DEX 40-60, INT 200-300
- **HP:** 1000-1200
- **Damage:** 30-40
- **Virtual Armor:** 70
- **Special:** Root snare, Barkskin damage reduction, Call of the wild adds
- **Loot:** LootPack.FilthyRich (3), LootPack.Gems (12)
- **Taming:** Not tameable

### CRYSTAL DRAKE ALPHA (Crystal Caverns Boss)
- **Body ID:** 60 (Drake)
- **Hue:** 1154 (Crystal blue)
- **BaseSoundID:** 362
- **Stats:** STR 600-700, DEX 120-150, INT 300-400
- **HP:** 800-1000
- **Damage:** 30-40
- **Virtual Armor:** 70
- **Special:** Prismatic beam, Shard storms, Reflect magic window
- **Loot:** LootPack.FilthyRich (3), LootPack.Gems (12)
- **Taming:** Not tameable

### GRIFFIN LORD (Skyreach Summit Boss)
- **Body ID:** 5 (Eagle)
- **Hue:** 0 (Natural)
- **BaseSoundID:** 0x8F
- **Stats:** STR 400-500, DEX 150-200, INT 150-200
- **HP:** 700-900
- **Damage:** 25-35
- **Virtual Armor:** 60
- **Special:** Wind walls, Knockback gusts, Aerial dive burst
- **Loot:** LootPack.FilthyRich (3), LootPack.Gems (12)
- **Taming:** Not tameable

### ANCIENT KRAKEN (Abyssal Trench Boss)
- **Body ID:** 77 (Kraken)
- **Hue:** 1367 (Deep sea)
- **BaseSoundID:** 353
- **Stats:** STR 800-900, DEX 80-100, INT 200-300
- **HP:** 1200-1500
- **Damage:** 40-50
- **Virtual Armor:** 80
- **Special:** Tentacle grabs, Ink cloud blind, Pressure crush, Summon Deep Dwellers
- **Loot:** LootPack.FilthyRich (3), LootPack.Gems (12)
- **Taming:** Not tameable

### TIMEWORN LICH (Twilight Labyrinth Boss)
- **Body ID:** 830 (Primeval Lich - SA)
- **Hue:** 1109 (Shadow)
- **BaseSoundID:** 0x3E9
- **Stats:** STR 600-700, DEX 120-150, INT 600-700
- **HP:** 1000-1200
- **Damage:** 25-35
- **Virtual Armor:** 65
- **Special:** Time slow field, Shadow duplicate, Life drain channel
- **Loot:** LootPack.FilthyRich (3), LootPack.Gems (12)
- **Taming:** Not tameable

---

# LORE RECONCILIATION - MISSING CREATURES

The following creatures were mentioned in VYSTIA_WORLD_LORE.md regional bestiaries and have been added:

## FROSTHOLD ADDITIONS

### ICE ELEMENTAL
- **Body ID:** 161 (Snow Elemental - T2A)
- **Hue:** 1150 (Ice white)
- **BaseSoundID:** 268
- **Stats:** STR 150-200, DEX 80-100, INT 100-150
- **HP:** 150-200
- **Damage:** 12-18
- **Virtual Armor:** 35
- **Special:** Cold damage 100%, Cold aura
- **Loot:** LootPack.Average, LootPack.Gems (3)
- **Taming:** Not tameable

### GLACIAL STALKER
- **Body ID:** 227 (Cougar/Panther)
- **Hue:** 1150 (Ice white)
- **BaseSoundID:** 0x462
- **Stats:** STR 120-160, DEX 150-200, INT 40-60
- **HP:** 100-140
- **Damage:** 12-18
- **Virtual Armor:** 30
- **Special:** Stealth, Ambush attack, Cold damage
- **Loot:** LootPack.Average
- **Taming:** Yes - MinTame 88.0, ControlSlots 2

### BOREAL SERPENT
- **Body ID:** 89 (Ice Serpent)
- **Hue:** 1152 (Ice blue)
- **BaseSoundID:** 219
- **Stats:** STR 100-150, DEX 90-120, INT 30-50
- **HP:** 80-120
- **Damage:** 10-15
- **Virtual Armor:** 25
- **Special:** Cold damage, Constrict
- **Loot:** LootPack.Average
- **Taming:** Yes - MinTame 70.0, ControlSlots 1

### FROST SALAMANDER
- **Body ID:** 35 (Lizardman)
- **Hue:** 1152 (Ice blue)
- **BaseSoundID:** 417
- **Stats:** STR 150-200, DEX 80-100, INT 50-70
- **HP:** 120-150
- **Damage:** 12-18
- **Virtual Armor:** 30
- **Special:** Cold immunity, Frost aura
- **Loot:** LootPack.Average
- **Taming:** Not tameable

---

## VERDANTPEAK ADDITIONS

### DRYAD
- **Body ID:** 266 (Dryad - SA)
- **Hue:** 2012 (Leaf green)
- **BaseSoundID:** 0x467
- **Stats:** STR 80-100, DEX 100-120, INT 150-200
- **HP:** 100-150
- **Damage:** 8-12
- **Virtual Armor:** 25
- **Special:** Nature magic, Charm, Tree meld
- **Loot:** LootPack.Rich, LootPack.MedScrolls (2)
- **Taming:** Not tameable

### OWLBEAR
- **Body ID:** 212 (Grizzly Bear)
- **Hue:** 2011 (Dark forest)
- **BaseSoundID:** 0xA3
- **Stats:** STR 200-280, DEX 60-80, INT 30-50
- **HP:** 180-250
- **Damage:** 18-25
- **Virtual Armor:** 35
- **Special:** Berserker rage, Maul attack
- **Loot:** LootPack.Average
- **Taming:** Yes - MinTame 91.0, ControlSlots 2

### FOREST HAG
- **Body ID:** 87 (Evil Mage Female)
- **Hue:** 2010 (Forest green)
- **BaseSoundID:** 0x4B0
- **Stats:** STR 80-100, DEX 80-100, INT 180-220
- **HP:** 120-160
- **Damage:** 8-12
- **Virtual Armor:** 25
- **Special:** Curse, Illusion, Polymorph
- **Loot:** LootPack.Rich, LootPack.MedScrolls (2)
- **Taming:** Not tameable

### FOREST GOBLIN
- **Body ID:** 723 (Goblin - SA)
- **Hue:** 2010 (Forest green)
- **BaseSoundID:** 0x600
- **Stats:** STR 50-80, DEX 80-120, INT 30-50
- **HP:** 40-60
- **Damage:** 5-10
- **Virtual Armor:** 15
- **Special:** Trap setting, Ambush, Pack tactics
- **Loot:** LootPack.Meager
- **Taming:** Not tameable

### DARK UNICORN
- **Body ID:** 123 (Unicorn)
- **Hue:** 1109 (Shadow black)
- **BaseSoundID:** 0xA8
- **Stats:** STR 300-400, DEX 150-200, INT 200-250
- **HP:** 250-350
- **Damage:** 18-25
- **Virtual Armor:** 45
- **Special:** Shadow magic, Charge attack, Dispel magic
- **Loot:** LootPack.Rich, LootPack.Gems (5)
- **Taming:** Not tameable

### GREEN DRAGON
- **Body ID:** 12 (Dragon)
- **Hue:** 2010 (Forest green)
- **BaseSoundID:** 362
- **Stats:** STR 750-800, DEX 80-100, INT 400-450
- **HP:** 450-500
- **Damage:** 16-22
- **Virtual Armor:** 58
- **Special:** Poison breath, Nature magic, Flight
- **Loot:** LootPack.FilthyRich (2), LootPack.Gems (8)
- **Taming:** Not tameable

---

## IRONCLAD EMPIRE ADDITIONS

### STEAM MEPHIT
- **Body ID:** 10 (Imp)
- **Hue:** 2401 (Bronze steam)
- **BaseSoundID:** 422
- **Stats:** STR 60-80, DEX 100-130, INT 80-100
- **HP:** 40-60
- **Damage:** 5-8
- **Virtual Armor:** 20
- **Special:** Steam jet, Scalding touch, Flight
- **Loot:** LootPack.Meager
- **Taming:** Not tameable

### CHIMERA
- **Body ID:** 46 (Ancient Wyrm - modified for multi-head look)
- **Hue:** 2406 (Ironclad grey)
- **BaseSoundID:** 362
- **Stats:** STR 400-500, DEX 100-130, INT 100-150
- **HP:** 350-450
- **Damage:** 20-28
- **Virtual Armor:** 50
- **Special:** Fire breath, Poison bite, Charge attack
- **Loot:** LootPack.FilthyRich, LootPack.Gems (5)
- **Taming:** Not tameable

### WYVERN
- **Body ID:** 62 (Wyvern)
- **Hue:** 2407 (Copper tint)
- **BaseSoundID:** 362
- **Stats:** STR 200-250, DEX 120-150, INT 50-70
- **HP:** 180-220
- **Damage:** 15-20
- **Virtual Armor:** 35
- **Special:** Poison sting, Flight, Dive attack
- **Loot:** LootPack.Average
- **Taming:** Yes - MinTame 92.0, ControlSlots 2

### MIMIC
- **Body ID:** 751 (Mimic - Chest form)
- **Hue:** 0 (Default)
- **BaseSoundID:** 0x3E9
- **Stats:** STR 150-200, DEX 100-130, INT 80-120
- **HP:** 150-200
- **Damage:** 15-22
- **Virtual Armor:** 40
- **Special:** Surprise attack, Adhesive, Shapechanger
- **Loot:** LootPack.FilthyRich
- **Taming:** Not tameable

---

## DEEPFORGE ADDITIONS

### RUST MONSTER
- **Body ID:** 244 (Giant Beetle)
- **Hue:** 1109 (Rust brown)
- **BaseSoundID:** 0x21D
- **Stats:** STR 100-150, DEX 80-100, INT 20-40
- **HP:** 80-120
- **Damage:** 8-12
- **Virtual Armor:** 25
- **Special:** Metal corrosion (destroys metal armor/weapons over time)
- **Loot:** LootPack.Meager
- **Taming:** Not tameable

### GELATINOUS CUBE
- **Body ID:** 51 (Slime)
- **Hue:** 1364 (Translucent cyan)
- **BaseSoundID:** 456
- **Stats:** STR 150-200, DEX 10-20, INT 10-20
- **HP:** 200-250
- **Damage:** 10-15
- **Virtual Armor:** 40
- **Special:** Engulf, Acid dissolve, Physical resistance
- **Loot:** LootPack.Rich (random engulfed items)
- **Taming:** Not tameable

### MIND FLAYER
- **Body ID:** 318 (Watcher - Third Dawn - tentacled)
- **Hue:** 1367 (Deep purple)
- **BaseSoundID:** 0x482
- **Stats:** STR 150-200, DEX 80-100, INT 300-400
- **HP:** 200-280
- **Damage:** 12-18
- **Virtual Armor:** 35
- **Special:** Mind blast, Dominate, Extract brain
- **Loot:** LootPack.FilthyRich, LootPack.MedScrolls (3)
- **Taming:** Not tameable

### BEHOLDER
- **Body ID:** 318 (Watcher - Third Dawn)
- **Hue:** 1175 (Eye purple)
- **BaseSoundID:** 0x482
- **Stats:** STR 200-250, DEX 60-80, INT 350-450
- **HP:** 300-400
- **Damage:** 15-20
- **Virtual Armor:** 40
- **Special:** Antimagic cone, Multiple eye rays (random effects)
- **Loot:** LootPack.FilthyRich, LootPack.MedScrolls (3)
- **Taming:** Not tameable

### ACIDIC OOZE
- **Body ID:** 51 (Slime)
- **Hue:** 63 (Acid green)
- **BaseSoundID:** 456
- **Stats:** STR 120-160, DEX 20-40, INT 10-20
- **HP:** 150-200
- **Damage:** 10-15
- **Virtual Armor:** 30
- **Special:** Acid damage, Split on hit
- **Loot:** LootPack.Meager
- **Taming:** Not tameable

### KOBOLD
- **Body ID:** 253 (Ratman)
- **Hue:** 2406 (Scale brown)
- **BaseSoundID:** 437
- **Stats:** STR 40-60, DEX 60-90, INT 30-50
- **HP:** 30-50
- **Damage:** 4-8
- **Virtual Armor:** 15
- **Special:** Trap master, Pack tactics, Flee at low HP
- **Loot:** LootPack.Poor
- **Taming:** Not tameable

---

## WHISPERING SANDS ADDITIONS

### LAMIA
- **Body ID:** 732 (Medusa - SA)
- **Hue:** 2305 (Sandy tan)
- **BaseSoundID:** 0x4B0
- **Stats:** STR 150-200, DEX 100-120, INT 200-250
- **HP:** 180-240
- **Damage:** 12-18
- **Virtual Armor:** 35
- **Special:** Charm, Drain wisdom, Illusion
- **Loot:** LootPack.Rich, LootPack.MedScrolls (2)
- **Taming:** Not tameable

### BLUE DRAGON (Sand Dragon)
- **Body ID:** 12 (Dragon)
- **Hue:** 1367 (Desert blue)
- **BaseSoundID:** 362
- **Stats:** STR 750-800, DEX 80-100, INT 400-450
- **HP:** 450-500
- **Damage:** 16-22
- **Virtual Armor:** 58
- **Special:** Lightning breath, Sandstorm, Burrow
- **Loot:** LootPack.FilthyRich (2), LootPack.Gems (8)
- **Taming:** Not tameable

### ROC
- **Body ID:** 5 (Eagle)
- **Hue:** 2307 (Desert brown)
- **BaseSoundID:** 0x8F
- **Stats:** STR 400-500, DEX 100-130, INT 60-80
- **HP:** 300-400
- **Damage:** 18-26
- **Virtual Armor:** 40
- **Special:** Massive size, Carry off attack, Flight
- **Loot:** LootPack.Rich, LootPack.Gems (5)
- **Taming:** Not tameable

### GENIE (Djinn)
- **Body ID:** 13 (Air Elemental)
- **Hue:** 2306 (Gold/tan)
- **BaseSoundID:** 655
- **Stats:** STR 250-350, DEX 150-200, INT 300-400
- **HP:** 250-350
- **Damage:** 15-22
- **Virtual Armor:** 45
- **Special:** Wish granting (quest), Polymorph, Teleport
- **Loot:** LootPack.FilthyRich, LootPack.Gems (8)
- **Taming:** Not tameable

---

## SHADOWFEN ADDITIONS

### SHADOW SERPENT
- **Body ID:** 89 (Ice Serpent)
- **Hue:** 1109 (Shadow black)
- **BaseSoundID:** 219
- **Stats:** STR 100-150, DEX 100-130, INT 50-70
- **HP:** 80-120
- **Damage:** 10-15
- **Virtual Armor:** 25
- **Special:** Poison, Stealth, Shadow strike
- **Loot:** LootPack.Average
- **Taming:** Yes - MinTame 75.0, ControlSlots 1

### MARSH SPIRIT
- **Body ID:** 740 (Dream Wraith - SA)
- **Hue:** 2212 (Swamp green)
- **BaseSoundID:** 0x482
- **Stats:** STR 80-100, DEX 100-120, INT 100-150
- **HP:** 80-100
- **Damage:** 8-12
- **Virtual Armor:** 25
- **Special:** Confusion, Drown, Ethereal
- **Loot:** LootPack.Meager
- **Taming:** Not tameable

### SWAMP GHOUL
- **Body ID:** 153 (Ghoul)
- **Hue:** 2212 (Swamp green)
- **BaseSoundID:** 471
- **Stats:** STR 100-140, DEX 60-80, INT 30-50
- **HP:** 100-140
- **Damage:** 10-15
- **Virtual Armor:** 25
- **Special:** Paralysis touch, Disease, Undead
- **Loot:** LootPack.Meager
- **Taming:** Not tameable

### MUD GOLEM
- **Body ID:** 752 (Golem - Third Dawn)
- **Hue:** 2212 (Mud brown)
- **BaseSoundID:** 541
- **Stats:** STR 180-220, DEX 30-50, INT 30-50
- **HP:** 180-220
- **Damage:** 14-20
- **Virtual Armor:** 35
- **Special:** Engulf, Slow, Poison immunity
- **Loot:** LootPack.Average
- **Taming:** Not tameable

### FEN DRAKE
- **Body ID:** 60 (Drake)
- **Hue:** 2212 (Swamp green)
- **BaseSoundID:** 362
- **Stats:** STR 250-300, DEX 80-100, INT 80-120
- **HP:** 200-260
- **Damage:** 14-20
- **Virtual Armor:** 40
- **Special:** Poison breath, Swamp camouflage, Flight
- **Loot:** LootPack.Rich, LootPack.Gems (3)
- **Taming:** Not tameable

### BOGHOUND
- **Body ID:** 99 (Hell Hound)
- **Hue:** 2212 (Swamp green)
- **BaseSoundID:** 0xE5
- **Stats:** STR 80-120, DEX 80-100, INT 20-40
- **HP:** 60-90
- **Damage:** 10-15
- **Virtual Armor:** 25
- **Special:** Pack tactics, Tracking, Poison bite
- **Loot:** LootPack.Meager
- **Taming:** Yes - MinTame 72.0, ControlSlots 1

---

## SKYREACH MOUNTAINS ADDITIONS (from Lore line 1836)

### THUNDERBIRD
- **Body ID:** 5 (Eagle)
- **Hue:** 1109 (Storm grey)
- **BaseSoundID:** 0x8F
- **Stats:** STR 200-250, DEX 120-160, INT 100-150
- **HP:** 180-240
- **Damage:** 15-22
- **Virtual Armor:** 35
- **Special:** Lightning strike, Storm call, Flight
- **Loot:** LootPack.Rich, LootPack.Gems (5)
- **Taming:** Not tameable

### CLOUD GIANT
- **Body ID:** 76 (Titan)
- **Hue:** 1001 (Cloud white)
- **BaseSoundID:** 609
- **Stats:** STR 350-450, DEX 70-90, INT 100-150
- **HP:** 400-500
- **Damage:** 22-30
- **Virtual Armor:** 50
- **Special:** Lightning bolt, Wind gust, Boulder throw
- **Loot:** LootPack.FilthyRich, LootPack.Gems (8)
- **Taming:** Not tameable

### HIGHPEAK YETI
- **Body ID:** 212 (Grizzly Bear)
- **Hue:** 1150 (Snow white)
- **BaseSoundID:** 0xA3
- **Stats:** STR 180-240, DEX 50-70, INT 20-40
- **HP:** 160-220
- **Damage:** 16-22
- **Virtual Armor:** 35
- **Special:** Cold damage, Maul attack, Camouflage
- **Loot:** LootPack.Average
- **Taming:** Not tameable

### SKY SERPENT
- **Body ID:** 150 (Sea Serpent)
- **Hue:** 1001 (Cloud white)
- **BaseSoundID:** 447
- **Stats:** STR 200-250, DEX 100-130, INT 60-80
- **HP:** 180-240
- **Damage:** 14-20
- **Virtual Armor:** 35
- **Special:** Flight, Lightning damage, Constrict
- **Loot:** LootPack.Rich
- **Taming:** Not tameable

### WIND ELEMENTAL
- **Body ID:** 13 (Air Elemental)
- **Hue:** 1001 (Pure white)
- **BaseSoundID:** 655
- **Stats:** STR 140-180, DEX 180-220, INT 100-140
- **HP:** 120-160
- **Damage:** 10-15
- **Virtual Armor:** 30
- **Special:** Whirlwind, Knockback, Air shield
- **Loot:** LootPack.Average, LootPack.Gems (3)
- **Taming:** Not tameable

---

## SUNKEN ISLES ADDITIONS (from Lore line 1860)

### MERFOLK WARRIOR
- **Body ID:** 773 (Merfolk - LBR)
- **Hue:** 0 (Natural)
- **BaseSoundID:** 0x3E9
- **Stats:** STR 100-150, DEX 100-130, INT 50-70
- **HP:** 100-140
- **Damage:** 12-18
- **Virtual Armor:** 30
- **Special:** Aquatic, Trident attack, Call dolphins
- **Loot:** LootPack.Average
- **Taming:** Not tameable

### REEF GUARDIAN
- **Body ID:** 16 (Water Elemental)
- **Hue:** 1363 (Coral)
- **BaseSoundID:** 278
- **Stats:** STR 150-200, DEX 80-100, INT 80-120
- **HP:** 150-200
- **Damage:** 12-18
- **Virtual Armor:** 40
- **Special:** Coral armor, Aquatic, Thorn damage
- **Loot:** LootPack.Average, LootPack.Gems (3)
- **Taming:** Not tameable

### SIREN
- **Body ID:** 30 (Harpy)
- **Hue:** 1367 (Sea blue)
- **BaseSoundID:** 402
- **Stats:** STR 80-100, DEX 100-130, INT 150-200
- **HP:** 100-140
- **Damage:** 8-12
- **Virtual Armor:** 25
- **Special:** Song of charm, Lure, Flight
- **Loot:** LootPack.Rich, LootPack.MedScrolls (2)
- **Taming:** Not tameable

### CORAL GOLEM
- **Body ID:** 752 (Golem - Third Dawn)
- **Hue:** 1363 (Coral pink)
- **BaseSoundID:** 541
- **Stats:** STR 200-250, DEX 40-60, INT 40-60
- **HP:** 200-250
- **Damage:** 15-22
- **Virtual Armor:** 45
- **Special:** Thorn damage, Aquatic, Coral regrowth
- **Loot:** LootPack.Rich, LootPack.Gems (5)
- **Taming:** Not tameable

### SEA WITCH
- **Body ID:** 87 (Evil Mage Female)
- **Hue:** 1367 (Sea blue)
- **BaseSoundID:** 0x4B0
- **Stats:** STR 80-100, DEX 80-100, INT 200-280
- **HP:** 150-200
- **Damage:** 10-15
- **Virtual Armor:** 30
- **Special:** Water magic, Summon sea creatures, Curse
- **Loot:** LootPack.Rich, LootPack.MedScrolls (2)
- **Taming:** Not tameable

---

## ETERNAL TWILIGHT ADDITIONS (from Lore line 1883)

### DUSK STALKER
- **Body ID:** 739 (Leather Wolf - SA)
- **Hue:** 1175 (Twilight purple)
- **BaseSoundID:** 0xE5
- **Stats:** STR 120-160, DEX 120-150, INT 40-60
- **HP:** 100-140
- **Damage:** 12-18
- **Virtual Armor:** 30
- **Special:** Shadow step, Pack tactics, Twilight sight
- **Loot:** LootPack.Average
- **Taming:** Yes - MinTame 85.0, ControlSlots 2

### NIGHTMARE
- **Body ID:** 114 (Nightmare)
- **Hue:** 0 (Default)
- **BaseSoundID:** 0xA8
- **Stats:** STR 496-525, DEX 86-105, INT 86-125
- **HP:** 298-315
- **Damage:** 16-22
- **Virtual Armor:** 50
- **Special:** Fire breath (20 damage), Fear, Flight (ethereal)
- **Loot:** LootPack.FilthyRich, LootPack.Gems (5)
- **Taming:** Yes - MinTame 95.3, ControlSlots 2

### TIME WRAITH
- **Body ID:** 740 (Dream Wraith - SA)
- **Hue:** 1175 (Purple temporal)
- **BaseSoundID:** 0x482
- **Stats:** STR 150-200, DEX 120-150, INT 200-280
- **HP:** 150-200
- **Damage:** 12-18
- **Virtual Armor:** 35
- **Special:** Time slow, Age touch, Phase
- **Loot:** LootPack.Rich
- **Taming:** Not tameable

### STAR BEAST
- **Body ID:** 741 (Slasher - SA)
- **Hue:** 1156 (Starlight white)
- **BaseSoundID:** 0x482
- **Stats:** STR 250-350, DEX 150-200, INT 150-200
- **HP:** 250-350
- **Damage:** 18-25
- **Virtual Armor:** 45
- **Special:** Energy damage, Star fire, Teleport
- **Loot:** LootPack.FilthyRich, LootPack.Gems (8)
- **Taming:** Not tameable

### EVENING DRAGON
- **Body ID:** 12 (Dragon)
- **Hue:** 1175 (Twilight purple)
- **BaseSoundID:** 362
- **Stats:** STR 750-800, DEX 80-100, INT 400-450
- **HP:** 450-500
- **Damage:** 16-22
- **Virtual Armor:** 58
- **Special:** Shadow breath, Time manipulation, Flight
- **Loot:** LootPack.FilthyRich (2), LootPack.Gems (8)
- **Taming:** Not tameable

### TWILIGHT SHADE
- **Body ID:** 26 (Wraith)
- **Hue:** 1175 (Twilight purple)
- **BaseSoundID:** 0x482
- **Stats:** STR 100-130, DEX 100-130, INT 120-160
- **HP:** 100-130
- **Damage:** 10-14
- **Virtual Armor:** 30
- **Special:** Life drain, Phase, Twilight aura
- **Loot:** LootPack.Meager
- **Taming:** Not tameable

---

# EXPANSION BODY ID REFERENCE

## Key Expansion Bodies Used

| Body ID | Name | Expansion | Used For |
|---------|------|-----------|----------|
| 10 | Imp | T2A | Steam Mephit |
| 51 | Slime | T2A | Gelatinous Cube, Acidic Ooze |
| 62 | Wyvern | T2A | Wyvern |
| 77 | Kraken | T2A | Kraken, Deep Dweller, Leviathan |
| 89 | Ice Serpent | T2A | Boreal Serpent, Shadow Serpent |
| 114 | Nightmare | T2A | Nightmare |
| 123 | Unicorn | T2A | Dark Unicorn |
| 128 | Pixie | T2A | Forest Sprite |
| 153 | Ghoul | T2A | Swamp Ghoul |
| 161 | Snow Elemental | T2A | Ice Elemental |
| 165 | Wisp Variant | LBR | Light Elemental, Shadow Wisp |
| 211 | Polar Bear | T2A | Glacial Bear |
| 212 | Grizzly Bear | T2A | Owlbear, Highpeak Yeti |
| 227 | Cougar/Panther | T2A | Glacial Stalker |
| 244 | Giant Beetle | SA | Rust Monster |
| 253 | Ratman | T2A | Kobold |
| 266 | Dryad | SA | Dryad, Shadow Dryad |
| 277 | White Wolf | LBR | Winter Wolf |
| 301 | Reaper | Third Dawn | Treant, Corrupted Treant, Ancient Treant |
| 311 | Shadow Knight | Third Dawn | Shadow Knight |
| 312 | Abysmal Horror | SA | Frozen Horror |
| 317 | Giant Bat | Third Dawn | Ember Bat |
| 318 | Watcher | Third Dawn | Mind Flayer, Beholder |
| 714 | Iron Beetle | SA | Scarab Beetle |
| 717 | Clockwork Scorpion | SA | Clockwork Scorpion, Mechanical Spider |
| 718 | Fairy Dragon | SA | Fairy Dragon |
| 720 | Lava Elemental | SA | Lava Elemental |
| 723 | Goblin | SA | Forest Goblin |
| 730 | Raptor/Gargoyle | SA | Stygian Gargoyle |
| 732 | Medusa | SA | Lamia |
| 736 | Wolf Spider | SA | Wolf Spider |
| 737 | Trapdoor Spider | SA | Crystal Spider |
| 738 | Fire Ant | SA | Fire Ant |
| 739 | Leather Wolf | SA | Shadow Wolf, Dusk Stalker |
| 740 | Dream Wraith | SA | Mistwalker, Void Walker, Time Wraith, Marsh Spirit |
| 741 | Slasher | SA | Slasher of Veils, Star Beast |
| 751 | Mimic | SA | Mimic |
| 752 | Golem | Third Dawn | All Golems (Ice, Ash, Gemstone, Iron, Magma, Steam, Lightning, Mud, Coral) |
| 768 | Juggernaut | LBR | Clockwork Beast, Juggernaut |
| 773 | Merfolk | LBR | Merfolk Warrior |
| 775 | Plague Beast | LBR | Plague Beast |
| 779 | Bogling | LBR | Bogling |
| 780 | Bog Thing | LBR | Bog Beast |
| 787 | Ant Lion | LBR | Ankheg |
| 788 | Stone Harpy | Third Dawn | Sphinx, Stone Harpy |
| 789 | Quagmire | LBR | Quagmire |
| 790 | Sand Vortex | LBR | Sand Vortex |
| 826 | Stygian Dragon | SA | Volcano Wyrm |
| 829 | Rising Colossus | SA | Steamwork Golem, Deepforge Behemoth |
| 830 | Primeval Lich | SA | Primeval Lich, Timeworn Lich |

---

# SUMMARY

- **Total Unique Creatures:** 150+
- **Dungeon Bosses:** 10
- **Tameable Creatures:** 25+
- **Body IDs Used:** T2A through SA expansions
- **No restrictions on expansion art**

## Regional Distribution
| Region | Creature Count |
|--------|----------------|
| Frosthold/Winterguard | 13 |
| Emberlands | 9 |
| Whispering Sands | 12 |
| Shadowfen | 14 |
| Verdantpeak | 15 |
| Crystal Barrens | 5 |
| Deepforge | 12 |
| Ironclad Empire | 10 |
| Skyreach Mountains | 16 |
| Underwater/Sunken Isles | 12 |
| Shadow/Void | 10 |
| Eternal Twilight | 6 |

## Lore Reconciliation Complete
All creatures mentioned in VYSTIA_WORLD_LORE.md regional bestiaries have been added with appropriate body IDs and thematic hues.
