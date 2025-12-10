# VYSTIA EQUIPMENT SYSTEM
## Complete Weapons, Armor, and Shields with Regional Variants

**Last Updated:** 2025-12-08

---

## IMPLEMENTATION STATUS

| Category | Designed | Implemented | Status | Generated |
|----------|----------|-------------|--------|-----------|
| **Class Starting Equipment** | 25 sets | 25 sets | ✅ 100% | Manual |
| **Class Special Items** | 16 | 16 | ✅ 100% | Manual |
| **Regional Weapons** | 40+ | 40 | ✅ 100% | ✅ Python Script |
| **Legendary Weapons** | 5 | 5 | ✅ 100% | ✅ Python Script |
| **Regional Plate Armor** | 24 | 24 | ✅ 100% | ✅ Python Script |
| **Regional Chain Armor** | 9 | 9 | ✅ 100% | ✅ Python Script |
| **Regional Ring Armor** | 8 | 8 | ✅ 100% | ✅ Python Script |
| **Regional Leather Armor** | 18 | 18 | ✅ 100% | ✅ Python Script |
| **Regional Shields** | 8 | 8 | ✅ 100% | ✅ Python Script |
| **Legendary Armor Sets** | 18+ | 19 | ✅ 100% | ✅ Python Script |

**Total Equipment Items:** 171 / 171 (100%)

---

## GENERATED EQUIPMENT (2025-12-08)

**Generation Method:** Python automation scripts (`generate_all_equipment.py`, `generate_armor_shields.py`)
**Build Status:** ✅ 0 errors, 0 warnings
**Files Created:** 5 new C# files in `ServUO/Scripts/Items/Vystia/Equipment/`

### Regional Weapons (40 items) ✅
**File:** `Equipment/Weapons/RegionalWeapons.cs`

- **Swords (17):** Frosthold (4), Emberlands (4), Crystal (3), Ironclad (3), Shadow (3)
- **Axes (8):** Frosthold (3), Emberlands (3), Ironclad (2)
- **Maces (7):** Frosthold (3), Emberlands (2), Ironclad (2)
- **Polearms (4):** Frosthold (2), Emberlands (2)
- **Ranged (4):** Frosthold (2), Verdantpeak (2)

**Features:** Regional hues, +20% damage, element damage (60/40 split), full serialization

### Legendary Weapons (4 new items) ✅
**File:** `Equipment/Weapons/LegendaryWeapons.cs`

- **Phoenix Ascension** (Katana) - 100% Fire, Hit Fireball 40%
- **The Cogmaster** (WarHammer) - 50/50 Energy/Physical, Hit Lightning 30%
- **Prismatic Edge** (Longsword) - 20% each damage type
- **Voidcaller** (QuarterStaff) - Spell Channeling, Mage Weapon -10

**Plus Existing:**
- **The Eternal Winter** (Halberd) - 100% Cold, Hit Cold Area 50%

### Regional Plate Armor (24 items) ✅
**File:** `Equipment/Armor/RegionalPlateArmor.cs`

**4 Complete Sets (6 pieces each):**
- **Frostforged Plate** (Hue 1152) - +5% Defend per piece
- **Emberforged Plate** (Hue 1358) - +5% Defend per piece
- **Clockwork Plate** (Hue 2401) - +2 STR, +1 Stam Regen per piece
- **Voidforged Plate** (Hue 1109) - Spell Channeling, -1 Cast Speed

### Regional Shields (8 items) ✅
**File:** `Equipment/Shields/RegionalShields.cs`

- Ice Wall, Flame Guard, Prism Shield, Clockwork Shield
- Bog Shield, Sand Shield, Void Shield, Living Shield

**Features:** +5-15% Defend, special properties (Spell Channeling, Reflect Physical, HP Regen)

### Legendary Armor (1 item) ✅
**File:** `Equipment/Armor/LegendaryArmor.cs`

- **Molten Core** (PlateChest) - +10 STR, +3 HP Regen, +15 Parry skill, AR 60

### Regional Chain Armor (9 items) ✅
**File:** `Equipment/Armor/RegionalChainArmor.cs`

**3 Complete Sets (3 pieces each):**
- **Crystal Chain** (Hue 1154) - +3 INT, -5% Mana Cost per piece
- **Shadow Chain** (Hue 1109) - Night Sight, +2 Stam Regen per piece
- **Desert Chain** (Hue 2305) - +3 DEX, 30% lighter weight per piece

### Regional Ring Armor (8 items) ✅
**File:** `Equipment/Armor/RegionalRingArmor.cs`

**2 Complete Sets (4 pieces each):**
- **Living Ring** (Hue 2010) - +2 HP Regen, +50 Luck per piece
- **Steam Ring** (Hue 2401) - +3 STR, +5% Weapon Speed per piece

### Regional Leather Armor (18 items) ✅
**File:** `Equipment/Armor/RegionalLeatherArmor.cs`

**3 Complete Sets (6 pieces each):**
- **Frost Leather** (Hue 1152) - +5 DEX per piece
- **Fire Leather** (Hue 1358) - +3 Stam Regen per piece
- **Shadow Leather** (Hue 1109) - Night Sight, +3 DEX, Stealth bonus per piece

### Legendary Armor Sets (18 items) ✅
**File:** `Equipment/Armor/LegendaryArmorSets.cs`

**3 Complete Legendary Sets:**
- **Glacial Aegis** (6-piece Plate) - +3 STR, +10% Defend, +5% Reflect, +5 Parry per piece
- **Steamwork Exosuit** (6-piece Plate) - +5 STR, -2 DEX, +10% Weapon Speed, +5 Blacksmith per piece
- **Shadow Shroud** (6-piece Leather) - +5 DEX, Night Sight, +2 Stam Regen, +5 Stealth/Hiding per piece

---

## PREVIOUSLY IMPLEMENTED

### Class Starting Equipment (25 sets) ✅
All 25 character classes have complete starting equipment sets including weapons, armor, and reagents. Each class receives region-appropriate equipment based on their home region and role.

### Class Special Items (16) ✅
All 16 class-specific special ability items fully implemented:
- RageTotem (Barbarian), BeastWhistle (Beastmaster), AlchemistKit (Alchemist)
- CrystalOrb (Oracle), MonkBeads (Monk), TemplarCross (Templar)
- SummoningCircle (Summoner), BountyLedger (Bounty Hunter), KnightBanner (Knight)
- SpiritTotem (Shaman), MagicLute (Bard), EnchantingCrystal (Enchanter)
- ConstructControlDevice (Artificer), ClockworkScout (Artificer), ShapeshiftTotem (Druid), HolySymbol (Cleric)

---

## NEXT PHASE

**High Priority:**
- In-game testing of all generated equipment (131 items)
- Add GM spawn commands for equipment testing
- Add resistance overrides to armor/shields (BaseXxxResistance properties)
- Implement missing skill bonuses (Stealth, Hiding)

**Medium Priority:**
- Add OnHit handlers for special shield effects (counter-attacks, poison)
- Implement set bonus detection system (wearing all pieces)
- Create equipment upgrade/enhancement system
- Add equipment to creature loot tables

**Low Priority:**
- Crafting system integration
- Equipment dye/customization system
- Equipment repair mechanics
- Visual effects for legendary sets

---

**Legacy Reference:** `src/VystiaEquipment.cs` - Original planning file

---

# WEAPONS BY CATEGORY

## SWORDS

### Base Types (Standard UO Graphics)
| Weapon | Graphic ID | Speed | Damage | Ingots |
|--------|------------|-------|---------|---------|
| Broadsword | 0xF5E | 30 | 14-18 | 10 |
| Longsword | 0xF61 | 35 | 15-16 | 12 |
| Viking Sword | 0x13B9 | 30 | 15-17 | 14 |
| Cutlass | 0x1441 | 40 | 11-13 | 8 |
| Katana | 0x13FE | 25 | 11-15 | 8 |
| Scimitar | 0x13B5 | 35 | 13-15 | 10 |
| Kryss | 0x1401 | 20 | 10-12 | 8 |

### Regional Sword Variants

#### FROSTHOLD SWORDS (Hue 1152)
- **Icicle Blade** (Longsword) - 12 Frostforged ingots
- **Winter's Edge** (Viking Sword) - 14 Frostforged ingots
- **Frostbite** (Cutlass) - 8 Frostforged ingots
- **Glacier Shard** (Kryss) - 8 Frostforged ingots

#### EMBERLANDS SWORDS (Hue 1358)
- **Flame Tongue** (Katana) - 8 Emberforged ingots
- **Magma Blade** (Scimitar) - 10 Emberforged ingots
- **Phoenix Wing** (Broadsword) - 10 Emberforged ingots
- **Lava Edge** (Longsword) - 12 Emberforged ingots

#### CRYSTAL SWORDS (Hue 1154)
- **Crystal Shard** (Kryss) - 8 Crystalline ingots
- **Prism Blade** (Longsword) - 12 Crystalline ingots
- **Refraction Edge** (Katana) - 8 Crystalline ingots

#### IRONCLAD SWORDS (Hue 2401)
- **Clockwork Sword** (Broadsword) - 10 Clockwork ingots + 5 gears
- **Gear Blade** (Viking Sword) - 14 Clockwork ingots + 3 gears
- **Steam Saber** (Cutlass) - 8 Clockwork ingots + 2 springs

#### SHADOW SWORDS (Hue 1109)
- **Shadow Fang** (Kryss) - 8 Shadowforged ingots
- **Void Edge** (Katana) - 8 Shadowforged ingots
- **Dark Blade** (Scimitar) - 10 Shadowforged ingots

## AXES

### Base Types
| Weapon | Graphic ID | Speed | Damage | Ingots |
|--------|------------|-------|---------|---------|
| Hatchet | 0xF43 | 40 | 13-15 | 4 |
| Battle Axe | 0xF47 | 30 | 15-17 | 14 |
| Double Axe | 0xF4B | 35 | 15-17 | 12 |
| Executioner's Axe | 0xF45 | 35 | 15-17 | 14 |
| Two Handed Axe | 0x1443 | 30 | 16-18 | 16 |
| War Axe | 0x13B0 | 35 | 14-16 | 12 |
| Pickaxe | 0xE86 | 35 | 13-15 | 8 |

### Regional Axe Variants

#### FROSTHOLD AXES (Hue 1152)
- **Frozen Cleaver** (Battle Axe) - 14 Frostforged ingots
- **Ice Shard Axe** (Double Axe) - 12 Frostforged ingots
- **Glacial Hatchet** (Hatchet) - 4 Frostforged ingots

#### EMBERLANDS AXES (Hue 1358)
- **Molten Axe** (Two Handed Axe) - 16 Emberforged ingots
- **Flame Cleaver** (War Axe) - 12 Emberforged ingots
- **Lava Pick** (Pickaxe) - 8 Emberforged ingots

#### IRONCLAD AXES (Hue 2401)
- **Gear Axe** (Executioner's Axe) - 14 Clockwork ingots + 5 gears
- **Steam Cleaver** (Battle Axe) - 14 Clockwork ingots + 3 springs

## MACES & HAMMERS

### Base Types
| Weapon | Graphic ID | Speed | Damage | Ingots |
|--------|------------|-------|---------|---------|
| Club | 0x13B4 | 40 | 11-13 | 10 |
| Mace | 0xF5C | 35 | 12-14 | 6 |
| Maul | 0x143B | 30 | 14-16 | 10 |
| War Mace | 0x1407 | 30 | 16-17 | 14 |
| War Hammer | 0x1439 | 25 | 17-18 | 16 |
| Hammer Pick | 0x143D | 30 | 15-17 | 16 |
| Quarter Staff | 0xE89 | 25 | 11-14 | 6 |

### Regional Mace Variants

#### FROSTHOLD MACES (Hue 1152)
- **Glacial Hammer** (War Hammer) - 16 Frostforged ingots
- **Frost Maul** (Maul) - 10 Frostforged ingots
- **Ice Club** (Club) - 10 Frostforged ingots

#### EMBERLANDS MACES (Hue 1358)
- **Molten Mace** (War Mace) - 14 Emberforged ingots
- **Magma Hammer** (War Hammer) - 16 Emberforged ingots

#### IRONCLAD MACES (Hue 2401)
- **Piston Mace** (War Mace) - 14 Clockwork ingots + 4 springs
- **Steam Hammer** (War Hammer) - 16 Clockwork ingots + 6 gears

## POLEARMS

### Base Types
| Weapon | Graphic ID | Speed | Damage | Ingots |
|--------|------------|-------|---------|---------|
| Spear | 0xF62 | 40 | 13-15 | 12 |
| Pike | 0x26BE | 35 | 14-16 | 16 |
| Short Spear | 0x1403 | 45 | 10-13 | 6 |
| Halberd | 0x143E | 25 | 18-19 | 20 |
| Bardiche | 0xF4D | 30 | 17-18 | 18 |

### Regional Polearm Variants

#### FROSTHOLD POLEARMS (Hue 1152)
- **Ice Lance** (Pike) - 16 Frostforged ingots
- **Frozen Halberd** (Halberd) - 20 Frostforged ingots

#### EMBERLANDS POLEARMS (Hue 1358)
- **Lava Spear** (Spear) - 12 Emberforged ingots
- **Volcanic Pike** (Pike) - 16 Emberforged ingots

## RANGED WEAPONS

### Base Types
| Weapon | Graphic ID | Speed | Damage | Materials |
|--------|------------|-------|---------|-----------|
| Bow | 0x13B2 | 20 | 16-18 | 7 boards |
| Crossbow | 0xF50 | 18 | 18-20 | 10 boards + 1 ingot |
| Heavy Crossbow | 0x13FD | 10 | 19-20 | 12 boards + 2 ingots |

### Regional Ranged Variants

#### FROSTHOLD RANGED (Hue 1152)
- **Frost Bow** - 7 Frostwood boards, Cold damage
- **Ice Crossbow** - 10 Frostwood + 1 Frostforged ingot

#### VERDANTPEAK RANGED (Hue 2010)
- **Living Bow** - 7 Living wood boards, HP regen
- **Nature's Crossbow** - 10 Living wood + 1 Natureforged ingot

---

# ARMOR SETS

## PLATE ARMOR

### Base Plate Set (Standard UO)
| Piece | Graphic ID | AR | Ingots |
|-------|------------|-----|---------|
| Plate Helm | 0x1412 | 5 | 15 |
| Plate Gorget | 0x1413 | 5 | 10 |
| Platemail Chest | 0x1415 | 5 | 25 |
| Plate Arms | 0x1410 | 5 | 18 |
| Plate Gloves | 0x1414 | 5 | 12 |
| Plate Legs | 0x1411 | 5 | 20 |

### FROSTFORGED PLATE (Hue 1152)
- **Full Set Bonus:** Cold immunity, +15 Cold Resist
- **Properties:** -5 Fire Resist per piece
- **Total Ingots:** 100 Frostforged

### EMBERFORGED PLATE (Hue 1358)
- **Full Set Bonus:** Fire immunity, +15 Fire Resist
- **Properties:** -5 Cold Resist per piece
- **Total Ingots:** 100 Emberforged

### CLOCKWORK PLATE (Hue 2401)
- **Full Set Bonus:** +10 STR, +10 Stamina Regen
- **Properties:** Self-Repair 2
- **Total Ingots:** 100 Clockwork + 20 gears

### VOIDFORGED PLATE (Hue 1109)
- **Full Set Bonus:** Mage Armor, +10 Magic Resist
- **Properties:** Spell Channeling
- **Total Ingots:** 100 Voidforged

## CHAIN ARMOR

### Base Chain Set
| Piece | Graphic ID | AR | Ingots |
|-------|------------|-----|---------|
| Chain Coif | 0x13BB | 4 | 10 |
| Chainmail Tunic | 0x13BF | 4 | 20 |
| Chain Legs | 0x13BE | 4 | 18 |

### Regional Chain Variants
- **Crystal Chain** (Hue 1154) - Energy resistance
- **Shadow Chain** (Hue 1109) - Stealth bonus
- **Desert Chain** (Hue 2305) - Lightweight

## RING ARMOR

### Base Ring Set
| Piece | Graphic ID | AR | Ingots |
|-------|------------|-----|---------|
| Ring Helm | 0x140B | 3 | 8 |
| Ring Tunic | 0x13EC | 3 | 18 |
| Ring Sleeves | 0x13EE | 3 | 14 |
| Ring Gloves | 0x13EB | 3 | 10 |
| Ring Legs | 0x13F0 | 3 | 16 |

### Regional Ring Variants
- **Living Ring** (Hue 2010) - HP Regeneration
- **Steam Ring** (Hue 2401) - Mechanical bonuses

## LEATHER ARMOR

### Base Leather Set
| Piece | Graphic ID | AR | Leather |
|-------|------------|-----|---------|
| Leather Cap | 0x1DB9 | 2 | 4 |
| Leather Gorget | 0x13C7 | 2 | 3 |
| Leather Tunic | 0x13CC | 2 | 12 |
| Leather Sleeves | 0x13CD | 2 | 8 |
| Leather Gloves | 0x13C6 | 2 | 3 |
| Leather Legs | 0x13CB | 2 | 10 |

### Regional Leather Variants
- **Frost Leather** (Hue 1152) - From Winter Wolves
- **Fire Leather** (Hue 1358) - From Lava Hounds
- **Shadow Leather** (Hue 1109) - From Shadow Wolves

---

# SHIELDS

## Base Shield Types
| Shield | Graphic ID | AR | Ingots/Boards |
|--------|------------|-----|---------------|
| Buckler | 0x1B73 | 1 | 10 ingots |
| Metal Shield | 0x1B7B | 2 | 14 ingots |
| Bronze Shield | 0x1B72 | 3 | 12 ingots |
| Metal Kite Shield | 0x1B74 | 4 | 16 ingots |
| Heater Shield | 0x1B76 | 5 | 18 ingots |
| Wooden Shield | 0x1B7A | 1 | 8 boards |
| Wooden Kite Shield | 0x1B78 | 2 | 12 boards |
| Order Shield | 0x1BC4 | 6 | 20 ingots |
| Chaos Shield | 0x1BC3 | 6 | 20 ingots |

## Regional Shield Variants

### FROSTHOLD SHIELDS (Hue 1152)
- **Ice Wall** (Heater Shield) - 18 Frostforged ingots
  - Cold Resist +10, Reflect Physical 5%

### EMBERLANDS SHIELDS (Hue 1358)
- **Flame Guard** (Metal Kite Shield) - 16 Emberforged ingots
  - Fire Resist +10, Attackers take 2 fire damage

### CRYSTAL SHIELDS (Hue 1154)
- **Prism Shield** (Metal Shield) - 14 Crystalline ingots
  - Energy Resist +15, Spell Reflect 10%

### IRONCLAD SHIELDS (Hue 2401)
- **Clockwork Shield** (Order Shield) - 20 Clockwork ingots + 5 gears
  - Self-Repair 3, +10 Parry skill

### SHADOWFEN SHIELDS (Hue 2212)
- **Bog Shield** (Wooden Shield) - 8 Shadowwood boards
  - Poison Resist +15, Poison attackers

### DESERT SHIELDS (Hue 2305)
- **Sand Shield** (Buckler) - 10 Sunforged ingots
  - +10 DEX, Weight -50%

### OBSIDIAN SHIELDS (Hue 1109)
- **Void Shield** (Chaos Shield) - 22 Voidforged ingots
  - Spell Channeling, FC -1

### VERDANTPEAK SHIELDS (Hue 2010)
- **Living Shield** (Wooden Kite) - 12 Living wood boards
  - HP Regen +2, Heals 1 HP/min

---

# SPECIAL ARMOR PIECES

## HELMETS

### Special Regional Helms
- **Frost Crown** (Plate Helm, Hue 1152) - Boss drop
- **Phoenix Crest** (Chain Coif, Hue 1358) - Boss drop
- **Crystal Circlet** (Circlet 0x2B6F, Hue 1154) - Crafted
- **Steam Goggles** (Orc Helm 0x1F0B, Hue 2401) - Crafted
- **Shadow Hood** (Hood 0x1540, Hue 1109) - Crafted

## GLOVES

### Special Regional Gloves
- **Frost Gauntlets** - Freeze touch attack
- **Ember Gloves** - Fire touch attack
- **Crystal Gloves** - Energy absorption
- **Clockwork Gloves** - +10 Tinkering

## BOOTS

### Base Boot Types
| Boot | Graphic ID | AR | Leather |
|------|------------|-----|---------|
| Boots | 0x170B | 2 | 8 |
| Thigh Boots | 0x1711 | 2 | 10 |
| Sandals | 0x170D | 0 | 4 |

### Regional Boot Variants
- **Frost Boots** (Hue 1152) - Walk on ice/water
- **Fire Boots** (Hue 1358) - Walk on lava
- **Wind Boots** (Hue 1001) - +5 DEX
- **Shadow Boots** (Hue 1109) - Stealth +20

---

# CRAFTING REQUIREMENTS

## BLACKSMITHING

### Skill Requirements by Material
| Material | Min Skill | Success at |
|----------|-----------|------------|
| Iron | 0.0 | 50.0 |
| Sunforged | 65.0 | 115.0 |
| Natureforged | 70.0 | 120.0 |
| Shadowforged | 75.0 | 125.0 |
| Steamwork | 80.0 | 130.0 |
| Frostforged | 85.0 | 135.0 |
| Emberforged | 90.0 | 140.0 |
| Crystalline | 95.0 | 145.0 |
| Voidforged | 100.0 | 150.0 |

### Special Crafting Requirements

#### Frostforged Items
- Must be within 5 tiles of snow/ice
- Requires Frozen Essence (1 per 10 ingots)
- 10% chance to shatter on failure

#### Emberforged Items
- Must be near forge or lava
- Requires Sulfurous Ash (1 per 10 ingots)
- Crafter takes 5 fire damage per attempt

#### Clockwork Items
- Requires GM Tinkering + GM Blacksmith
- Needs gears/springs components
- Can add mechanical mods

## TAILORING

### Leather Armor Requirements
| Material | Min Skill | Success at |
|----------|-----------|------------|
| Regular Leather | 0.0 | 50.0 |
| Spined Leather | 65.0 | 115.0 |
| Horned Leather | 80.0 | 130.0 |
| Barbed Leather | 99.0 | 149.0 |
| Frost Leather | 85.0 | 135.0 |
| Fire Leather | 90.0 | 140.0 |
| Shadow Leather | 95.0 | 145.0 |

---

# ARTIFACT WEAPONS & ARMOR

## LEGENDARY WEAPONS

### The Eternal Winter (Halberd)
- **Graphic:** 0x143E
- **Hue:** 1152 (Ice blue)
- **Damage:** 25-35
- **Specials:** Hit Cold Area 50%, Cold Damage 100%
- **Drop:** Frost Father (Frozen Halls boss)

### Phoenix Ascension (Katana)
- **Graphic:** 0x13FE
- **Hue:** 1358 (Fire)
- **Damage:** 20-30
- **Specials:** Hit Fireball 40%, Fire Damage 100%
- **Drop:** Volcano Wyrm

### The Cogmaster (War Hammer)
- **Graphic:** 0x1439
- **Hue:** 2401 (Bronze)
- **Damage:** 22-28
- **Specials:** Hit Lightning 30%, Self-Repair 5
- **Drop:** Forge Master

### Prismatic Edge (Longsword)
- **Graphic:** 0xF61
- **Hue:** 1154 (Crystal)
- **Damage:** 18-25
- **Specials:** All damage types 20% each
- **Drop:** Crystal Dragon

### Voidcaller (Quarter Staff)
- **Graphic:** 0xE89
- **Hue:** 1109 (Black)
- **Specials:** Spell Channeling, Mage Weapon -10
- **Drop:** Shadow Lich

## LEGENDARY ARMOR

### Glacial Aegis (Full Plate Set)
- **Hue:** 1152
- **Total AR:** 35
- **Bonuses:** Cold immunity, Reflect damage 15%
- **Drop:** Frost Father (set pieces)

### Molten Core (Platemail Chest)
- **Graphic:** 0x1415
- **Hue:** 1358
- **AR:** 8
- **Bonuses:** Fire Resist +30, STR +10
- **Drop:** Magma Lord

### Steamwork Exosuit (Full Set)
- **Hue:** 2401
- **Bonuses:** STR +20, DEX -10, Self-Repair 5
- **Drop:** Clockwork Titan (set pieces)

### Shadow Shroud (Leather Set)
- **Hue:** 1109
- **Bonuses:** Stealth +30, Hiding +30
- **Drop:** Shadow Master (set pieces)

---

# SET BONUSES

## Complete Set Bonuses (Wearing all 6 pieces)

### Elemental Sets
- **Frostforged:** Immunity to freeze, Cold damage aura
- **Emberforged:** Immunity to burn, Fire damage aura
- **Crystalline:** 25% spell reflect, Mana regen +5
- **Voidforged:** Phase shift ability, Magic resist +20

### Regional Sets
- **Clockwork:** Mechanical pet summoning, No stamina loss
- **Living:** Nature's blessing (constant healing), Animal friend
- **Shadow:** True invisibility 1/day, Poison immunity
- **Desert:** Never thirst, Mirage double 1/day

---

# ARMOR DYEING

## Dyeable Materials
- Leather armor - All hues
- Cloth armor - All hues
- Metal armor - Limited hues

## Special Dyes
- **Frost Dye** (Hue 1152) - From ice creatures
- **Fire Dye** (Hue 1358) - From fire creatures
- **Shadow Dye** (Hue 1109) - From shadow creatures
- **Crystal Dye** (Hue 1154) - From crystal creatures

---

# REPAIR & MAINTENANCE

## Repair Requirements
| Material | Skill Needed | Success Rate |
|----------|-------------|--------------|
| Iron | 50.0 | 90% |
| Regional Metals | Material skill -10 | 75% |
| Artifact items | GM + Material | 50% |

## Durability
- **Iron:** 50 uses
- **Regional metals:** 60-100 uses
- **Clockwork:** Self-repair
- **Living:** Regenerates in forests
- **Crystal:** Brittle (30 uses)

---

# SUMMARY

## Total Armory Content
- **70+ Base Weapons** (7 sword types, 7 axes, 7 maces, 5 polearms, 3 ranged)
- **200+ Regional Weapon Variants** (each region has variants)
- **6 Complete Armor Sets** (Plate, Chain, Ring, Leather, Studded, Bone)
- **48+ Regional Armor Variants** (8 regions × 6 pieces)
- **9 Shield Types** with 8 regional variants each
- **15+ Artifact Weapons**
- **10+ Artifact Armor Sets**
- **Special crafting requirements** per region
- **Set bonuses** for complete suits

All items use standard UO graphics with regional hue variations, making them 100% compatible with any UO client while providing unique regional identity and progression paths.
