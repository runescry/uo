# VYSTIA RESOURCES SYSTEM
## Complete Mining, Lumberjacking, and Reagent System

**Last Updated:** December 2024

---

## IMPLEMENTATION STATUS

| Category | Designed | Implemented | Status |
|----------|----------|-------------|--------|
| Ores | 8 types | 0 | ❌ Not Started |
| Ingots | 8 types | 0 | ❌ Not Started |
| Woods | 7 types | 0 | ❌ Not Started |
| Boards | 7 types | 0 | ❌ Not Started |
| Leathers | 3 types | 0 | ❌ Not Started |
| Reagents | 15 types | 0 | ❌ Not Started |
| Special Resources | 8 types | 4 | 🟡 50% |

**Implemented Items:**
- ✅ `FrozenArtifact.cs` - Frosthold crafting material
- ✅ `FrostSeal.cs` - Frosthold dungeon key
- ✅ `HeartwoodCoreFragment.cs` - Artifact fragment

**Next Phase:** Phase 1 - Core Resources (see `docs/VYSTIA_IMPLEMENTATION_PLAN.md`)

---

**Legacy Reference:** `src/VystiaResources.cs` - Original planning file

---

## MINING RESOURCES

### Ore Types (Regional)
| Ore Type | Region | Hue | Mining | Properties |
|----------|--------|-----|---------|------------|
| Iron Ore | Everywhere | 0 | 0.0 | Standard |
| Frozen Ore | Frosthold | 1152 | 85.0 | Cold damage +5 |
| Molten Ore | Emberlands | 1358 | 90.0 | Fire damage +5 |
| Crystal Ore | Crystal Barrens | 1154 | 95.0 | Energy resist +5 |
| Steamwork Ore | Ironclad | 2401 | 80.0 | Durability +25% |
| Bog Iron Ore | Shadowfen | 2212 | 75.0 | Self-repair 1 |
| Sandstone Ore | Desert | 2305 | 70.0 | Weight -30% |
| Obsidian Ore | Obsidian | 1109 | 100.0 | Mage Armor |
| Living Ore | Verdantpeak | 2010 | 65.0 | HP Regen +1 |

### Gems (Mining Byproducts)
- Diamonds (Crystal, 1150) - 100gp
- Rubies (Ember, 1161) - 75gp
- Sapphires (Frost, 1152) - 75gp
- Emeralds (Forest, 2010) - 50gp
- Amethysts (Shadow, 1109) - 50gp
- Topaz (Desert, 2305) - 40gp
- Onyx (Obsidian, 1109) - 60gp
- Opals (Crystal, 1154) - 80gp

## LUMBERJACKING RESOURCES

### Wood Types
| Wood Type | Region | Hue | Skill | Properties |
|-----------|--------|-----|-------|------------|
| Regular Wood | Everywhere | 0 | 0.0 | Standard |
| Frostwood | Frosthold | 1152 | 85.0 | Cold resist +5 |
| Flamewood | Emberlands | 1358 | 90.0 | Fire resist +5 |
| Petrified Wood | Desert | 2305 | 75.0 | Durability +50% |
| Shadowwood | Shadowfen | 2212 | 80.0 | Stealth +10 |
| Living Wood | Verdantpeak | 2010 | 70.0 | Self-repair |
| Crystal Wood | Crystal | 1154 | 95.0 | Spell channeling |
| Ironwood | Ironclad | 2401 | 85.0 | Physical resist +10 |

## NEW VYSTIA REAGENTS (15)

### Elemental Reagents
1. **Frost Essence** (Hue 1152)
   - Source: Ice elementals, frozen waterfalls
   - Uses: Ice spells, cold enchantments

2. **Ember Bloom** (Hue 1358)
   - Source: Volcanic vents
   - Uses: Fire spells, heat protection

3. **Storm Crystal** (Hue 1001)
   - Source: Lightning strikes
   - Uses: Lightning spells, energy magic

4. **Void Dust** (Hue 1109)
   - Source: Shadow creatures
   - Uses: Shadow magic, invisibility

### Nature Reagents
5. **Living Bark** (Hue 2010)
   - Source: Treants, ancient trees
   - Uses: Nature spells, healing

6. **Swamp Lotus** (Hue 2212)
   - Source: Bog waters
   - Uses: Poison spells, disease cure

7. **Desert Rose** (Hue 2305)
   - Source: Oasis gardens
   - Uses: Illusion magic, mirages

8. **Crystal Pollen** (Hue 1154)
   - Source: Crystal flowers
   - Uses: Energy spells, mana restoration

### Mechanical Reagents
9. **Clockwork Gear** (Hue 2401)
   - Source: Clockwork creatures
   - Uses: Mechanical spells, golem creation

10. **Steam Vapor** (Hue 1001)
    - Source: Steam vents
    - Uses: Fog spells, concealment

### Exotic Reagents
11. **Dragon Scale Powder**
    - Source: Ground dragon scales
    - Uses: High-level magic, resistance

12. **Phoenix Feather** (Hue 1358)
    - Source: Phoenix nests
    - Uses: Resurrection, fire immunity

13. **Kraken Ink** (Hue 1109)
    - Source: Kraken
    - Uses: Water magic, darkness

14. **Time Dust** (Hue 1154)
    - Source: Time anomalies
    - Uses: Haste/slow spells

15. **Ley Line Essence** (Hue 1156)
    - Source: Ley line nodes
    - Uses: Teleportation, gate travel

## STANDARD UO REAGENTS (8)
1. Black Pearl - Ocean
2. Blood Moss - Swamps
3. Garlic - Farms
4. Ginseng - Forests
5. Mandrake Root - Graveyards
6. Nightshade - Dark forests
7. Spider's Silk - Spider lairs
8. Sulfurous Ash - Volcanic

## FISHING RESOURCES

### Fish Types
- Common Fish - All waters (Food)
- Ice Fish - Frozen waters (Cold resist)
- Lava Fish - Lava pools (Fire resist)
- Crystal Fish - Crystal pools (Mana regen)
- Shadow Fish - Dark water (Stealth)
- Golden Fish - Oasis (Luck)
- Deep Fish - Ocean (Water breathing)

### Special Fishing Items
- Message in Bottle - Treasure maps
- Sea Serpent Scale - Armor crafting
- Pearls - Reagent/jewelry
- Coral - Decoration
- Sunken Treasure - Gold/items
- Kraken Tentacle - Weapon crafting

## LEATHER/HIDE TYPES

| Hide Type | Source | Hue | Properties |
|-----------|--------|-----|------------|
| Regular Leather | Animals | 0 | Standard |
| Spined Leather | Lizards | 2220 | Luck +40 |
| Horned Leather | Unicorns | 2117 | AR bonus |
| Barbed Leather | Dragons | 2129 | AR + resists |
| Frost Leather | Winter Wolves | 1152 | Cold resist |
| Fire Leather | Lava Hounds | 1358 | Fire resist |
| Shadow Leather | Shadow Wolves | 1109 | Stealth |

## CREATURE RESOURCES

### Bones & Body Parts
- Dragon Scales - Elite armor
- Troll Teeth - Weapons
- Giant Bones - Furniture
- Demon Horn - Dark magic
- Phoenix Feather - Resurrection
- Unicorn Horn - Cure poison
- Griffin Feather - Flight items
- Kraken Beak - Ship weapons

## SPECIAL REGIONAL RESOURCES

### Frosthold Exclusive
- Eternal Ice - Never melts
- Frozen Essence - Ice magic
- Winter Wolf Pelt - Cold protection
- Ice Crystal - Magic focus

### Emberlands Exclusive
- Ever-burning Coal - Perpetual fuel
- Lava Pearl - Fire jewelry
- Phoenix Ash - Resurrection
- Molten Core - Forge enhancement

### Crystal Barrens Exclusive
- Prismatic Shard - All-element magic
- Ley Crystal - Teleportation
- Memory Stone - Skill storage
- Focus Crystal - Meditation

### Shadowfen Exclusive
- Bog Iron - Self-repairing
- Swamp Gas - Explosive
- Witch Hair - Curses
- Shadow Silk - Stealth armor

### Ironclad Empire Exclusive
- Clockwork Parts - Gears, springs
- Steam Core - Power source
- Brass Tubing - Crafting
- Coal Oil - Fuel

### Desert Exclusive
- Glass Sand - Glass crafting
- Cactus Juice - Stamina
- Scorpion Venom - Poison
- Mirage Dust - Illusions

### Verdantpeak Exclusive
- Living Seed - Grows into items
- Ancient Amber - Preservation
- Treant Heart - Nature power
- Moonwell Water - Blessed

## RARE LEGENDARY MATERIALS

1. **Starfall Metal** - Meteor impacts, +50 all resists
2. **World Tree Wood** - One location, Blessed
3. **Primordial Essence** - Ancient bosses, Ultimate spells
4. **Void Crystal** - Void rifts, Dimensional magic
5. **Divine Tear** - Avatar drop, True resurrection

## GATHERING TOOLS

### Mining Tools
- Iron Pickaxe - 50 uses, Standard
- Frostforged Pickaxe - 75 uses, +5 mining
- Clockwork Pickaxe - 150 uses, Auto-smelt
- Crystal Pickaxe - 100 uses, Gem bonus

### Lumberjacking Tools
- Iron Hatchet - 50 uses, Standard
- Living Hatchet - 100 uses, Regrows trees
- Flame Axe - 75 uses, Auto-process

### Fishing Tools
- Fishing Pole - 50 uses, Standard
- Deep Sea Rod - 100 uses, +10 fishing
- Crystal Rod - 75 uses, Magic fish

## RESOURCE RESPAWN RATES
- Iron ore: 10 minutes
- Colored ore: 15 minutes
- Rare ore: 30 minutes
- Regular trees: 10 minutes
- Special wood: 20 minutes
- Common reagents: 30 minutes
- Rare reagents: 2 hours

## TRADE VALUES (per unit)
| Type | Common | Uncommon | Rare | Legendary |
|------|--------|----------|------|-----------|
| Ore/Ingot | 5gp | 15gp | 50gp | 500gp |
| Wood/Board | 3gp | 10gp | 30gp | 300gp |
| Leather | 4gp | 12gp | 40gp | 400gp |
| Reagent | 2gp | 8gp | 25gp | 250gp |
| Gem | 10gp | 50gp | 100gp | 1000gp |

## SUMMARY
- 9 Ore Types + Iron
- 8 Wood Types + Regular
- 15 New Reagents + 8 Standard UO
- 9 Gem Types
- 7 Fish Types + Common
- 7 Leather Types + Regular
- 20+ Special Regional Resources
- 5 Legendary Materials
