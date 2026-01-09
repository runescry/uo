# Vystia Vendors Reference

**Total Vendors:** 15
**Location:** `ServUO/Scripts/Mobiles/Vystia/Vendors/`

---

## Vendor Summary

| # | Vendor | Type | Items Sold |
|---|--------|------|------------|
| 1-12 | Magic School Vendors | Spell supplies | 8 reagents, 32 scrolls, 1 spellbook each |
| 13 | VystiaReagentVendor | All reagents | 82 reagents (all schools) |
| 14 | VystiaResourceVendor | Crafting materials | Ores, ingots, components, potions |
| 15 | VystiaClassItemVendor | Class items | 26 focus items, 17 potions |

---

## Magic School Vendors (12)

Each vendor sells supplies for one magic school:
- **1 Spellbook** (empty, 150 gold)
- **8 Reagents** (5 gold each)
- **32 Spell Scrolls** (10-45 gold by circle)

| Vendor | School | Class | Hue | Spellbook |
|--------|--------|-------|-----|-----------|
| IceMageVendor | Ice Magic | Ice Mage | 0x481 | IceMageSpellbook |
| DruidVendor | Nature Magic | Druid | 0x7D6 | DruidSpellbook |
| WitchVendor | Hex Magic | Witch | 0x81D | WitchSpellbook |
| SorcererVendor | Elemental | Sorcerer | 0x54E | SorcererSpellbook |
| WarlockVendor | Dark Magic | Warlock | 0x455 | WarlockSpellbook |
| OracleVendor | Divination | Oracle | 0x482 | OracleSpellbook |
| NecromancerVendor | Necromancy | Necromancer | 0x455 | NecromancerSpellbook |
| SummonerVendor | Summoning | Summoner | 0x555 | SummonerSpellbook |
| ShamanVendor | Shamanic | Shaman | 0x501 | ShamanSpellbook |
| BardVendor | Bardic | Bard | 0x8A5 | BardSpellbook |
| EnchanterVendor | Enchanting | Enchanter | 0x8FD | EnchanterSpellbook |
| IllusionistVendor | Illusion | Illusionist | 0x47E | IllusionistSpellbook |

### Scroll Prices by Circle

| Circle | Price |
|--------|-------|
| 1-2 | 10-15 gold |
| 3-4 | 20-25 gold |
| 5-6 | 30-35 gold |
| 7-8 | 40-45 gold |

---

## VystiaReagentVendor

**Sells:** All 82 Vystia magic reagents across 12 schools
**File:** `VystiaReagentVendor.cs`

### Reagent Prices by Tier

| Tier | Price | Circle |
|------|-------|--------|
| Basic | 5 gold | 1-2 |
| Common | 12-14 gold | 3-4 |
| Uncommon | 35-40 gold | 5-6 |
| Rare | 60-80 gold | 7-8 |

### Reagents by School

| School | Count | Examples |
|--------|-------|----------|
| Ice Magic | 8 | Frostbloom, Glacier Crystal, Heart of Winter |
| Nature Magic | 8 | Wild Moss, Moonpetal, Ancient Root |
| Hex Magic | 8 | Bog Moss, Viper Fang, Cursed Pearl |
| Elemental | 6 | Ash Petal, Lava Glass, Primordial Ember |
| Dark Magic | 6 | Shadow Moss, Void Weed, Void Crystal |
| Divination | 6 | Divination Dust, Time Sand, Fate Crystal |
| Necromancy | 6 | Grave Moss, Bone Dust, Lich Dust |
| Summoning | 5 | (underwater-themed) |
| Shamanic | 6 | Thunder Moss, Spirit Feather, Primal Thunder |
| Bardic | 8 | Song Petal, Echo Dust, Eternal Note |
| Enchanting | 7 | Arcane Dust, Rune Fragment, Essence of Magic |
| Illusion | 8 | Shadow Petal, Mirror Dust, Chaos Prism |

---

## VystiaResourceVendor

**Sells:** Crafting materials, ores, ingots, components
**File:** `VystiaResourceVendor.cs`

### Regional Ores (8 types)

| Ore | Region | Price |
|-----|--------|-------|
| Frozen Ore | Frosthold | 15 |
| Molten Ore | Emberlands | 18 |
| Crystal Ore | Crystal Barrens | 20 |
| Steamwork Ore | Ironclad | 22 |
| Bog Iron Ore | Shadowfen | 15 |
| Sandstone Ore | Desert | 17 |
| Obsidian Ore | ShadowVoid | 25 |
| Living Ore | Verdantpeak | 20 |

### Regional Ingots (8 types)

| Ingot | Region | Price |
|-------|--------|-------|
| Frostforged Ingot | Frosthold | 25 |
| Emberforged Ingot | Emberlands | 30 |
| Crystalline Ingot | Crystal Barrens | 35 |
| Clockwork Ingot | Ironclad | 40 |
| Shadowforged Ingot | Shadowfen | 25 |
| Sunforged Ingot | Desert | 28 |
| Voidforged Ingot | ShadowVoid | 45 |
| Natureforged Ingot | Verdantpeak | 35 |

### Mechanical Components (3 types)

| Component | Price | Use |
|-----------|-------|-----|
| Clockwork Gear | 10 | Engineering crafting |
| Clockwork Spring | 8 | Engineering crafting |
| Steam Core | 50 | Advanced crafting |

### Special Resources

| Resource | Price | Source |
|----------|-------|--------|
| Eternal Ice | 100 | Frosthold |
| Everburning Coal | 100 | Emberlands |
| Prismatic Shard | 150 | Crystal Barrens |
| Treant Heart | 200 | Verdantpeak |
| Living Bark | 75 | Verdantpeak |
| Swamp Lotus | 50 | Shadowfen |

### Regional Potions (8 types)

| Potion | Price | Effect |
|--------|-------|--------|
| Cold Resistance | 150 | +Cold resist |
| Heat Resistance | 150 | +Fire resist |
| Poison Resistance | 150 | +Poison resist |
| Energy Resistance | 150 | +Energy resist |
| Nature's Blessing | 200 | Verdantpeak buff |
| Crystal Clarity | 250 | +INT buff |
| Desert Swiftness | 175 | +DEX buff |
| Ironclad Fortitude | 175 | +STR buff |

---

## VystiaClassItemVendor

**Sells:** Class focus items, resource potions, combat potions
**File:** `VystiaClassItemVendor.cs`

### Class Focus Items (26 items)

| Class | Item | Price |
|-------|------|-------|
| Barbarian | Fury Idol | 800 |
| Fighter | War Banner | 750 |
| Knight | Combat Manual | 900 |
| Monk | Chi Beads | 800 |
| Paladin | Virtuous Relic | 1000 |
| Ranger | Hunter's Mark Totem | 850 |
| Rogue | Shadow Veil | 900 |
| Templar | Zealous Icon | 950 |
| Bounty Hunter | Tracking Stone | 850 |
| Beastmaster | Beast Bond | 900 |
| Ice Mage | Frost Crystal | 1000 |
| Sorcerer | Elemental Orb | 1200 |
| Warlock | Soul Gem | 1500 |
| Necromancer | Death's Hourglass | 1500 |
| Druid | Primal Totem | 1000 |
| Wizard | Arcane Conduit | 1100 |
| Oracle | Seer's Crystal | 1200 |
| Witch | Hex Doll | 1100 |
| Illusionist | Mirror Shard | 1000 |
| Summoner | Summoner's Sigil | 1300 |
| Shaman | Spirit Feather | 1000 |
| Cleric | Sacred Censer | 1000 |
| Bard | Dragon Lute | 1200 |
| Enchanter | Rune Stone | 1100 |
| Alchemist | Philosopher's Stone | 2000 |
| Artificer | Steam Core | 1500 |

### Resource Potions (11 items)

| Potion | Class | Price | Effect |
|--------|-------|-------|--------|
| Soul Essence Vial | Warlock | 150 | Restore soul shards |
| Fury Tonic | Barbarian | 75 | Restore fury |
| Chi Tea | Monk | 100 | Restore chi |
| Focus Elixir | Ranger | 75 | Restore focus |
| Fortitude Draught | Knight | 100 | Restore fortitude |
| Faith Incense | Cleric | 80 | Restore faith |
| Crescendo Crystal | Bard | 120 | Restore crescendo |
| Life Force Vial | Necromancer | 100 | Restore life force |
| Zeal Elixir | Templar | 100 | Restore zeal |
| Steam Canister | Artificer | 90 | Restore steam |
| Pursuit Tracker | Bounty Hunter | 110 | Restore pursuit |

### Combat Potions (6 items)

| Potion | Price | Effect | Duration |
|--------|-------|--------|----------|
| Burst Potion | 200 | +100% damage | Temporary |
| Haste Potion | 250 | +50% speed | Temporary |
| Resist Potion | 150 | +30 all resists | Temporary |
| Cleanse Potion | 175 | Remove debuffs | Instant |
| Second Wind | 300 | 50% HP/Mana/Stam | Instant |
| Invisibility Potion | 200 | Invisibility | 10 seconds |

---

## Spawn Commands

| Command | Description |
|---------|-------------|
| `[spawnvystia` | Opens spawn gump with Vendors page |
| `[add IceMageVendor` | Spawn specific vendor |
| `[add VystiaReagentVendor` | Spawn reagent vendor |
| `[add VystiaResourceVendor` | Spawn resource vendor |
| `[add VystiaClassItemVendor` | Spawn class item vendor |

---

*Last Updated: 2026-01-01*
