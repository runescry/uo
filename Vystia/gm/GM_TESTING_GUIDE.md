# Vystia GM Testing Guide

This guide covers how to use the **VystiaAdminGump** (`[VA`) to test all classes, abilities, spells, equipment, and consumables.

---

## Quick Start

1. Log in as GM/Admin
2. Type `[VA` or `[VystiaAdmin` to open the Vystia Admin Gump
3. Navigate tabs to test different systems

**Songweaving Smoke Test:** See `Vystia/TESTING_GUIDE.md` → **TEST 1.3: Songweaving (Bard) Smoke Test** for the fast Bard verification flow.

---

## VystiaAdminGump Overview

The admin gump has **6 tabs**:

| Tab | Purpose |
|-----|---------|
| **Classes** | Assign classes (Beginner/Maxed) |
| **Resources** | Secondary resources, reagent packs |
| **Stances** | Stance system testing |
| **Skills** | Set/reset Vystia skills |
| **Items** | Spawn equipment, spellbooks, consumables |
| **Tools** | GM tools and utilities |

### Items Tab Sub-Pages
| Sub-Page | Contents |
|----------|----------|
| Spellbooks | 12 spellbooks + Songbook of Weaving (custom range) |
| Reagents | Reagent packs by school |
| Weapons | Regional + legendary weapons |
| Armor | Regional + legendary armor |
| Focus Items | Class-specific ability items |
| Consumables | Potions and consumables |

---

## Tab 1: Classes

### Class Assignment Options

Each class has two buttons:
- **B (Beginner)** - Assigns class with default starting stats/equipment
- **M (Maxed)** - Assigns class with maxed skills + full loadout

### What "Maxed" Gives You

When you click "M" for any class, you receive:

1. **Class Assignment** - Full class with all abilities unlocked
2. **Skills Set to 100** - Class skill maxed
3. **Class Spellbook** (if magic class) - Pre-filled with all 32 spells
4. **Legendary Weapon** - Based on class region:
   - Frosthold classes → The Eternal Winter (ice axe)
   - Emberlands classes → Phoenix Ascension (fire katana)
   - Ironclad classes → The Cogmaster (steam hammer)
   - Crystal/Energy classes → Prismatic Edge (all-element sword)
   - ShadowVoid classes → Voidcaller (shadow staff)
5. **Legendary Armor Set** - Based on class role:
   - Tank/Healer → Glacial Aegis (plate set)
   - RangedDPS/CasterDPS → Shadow Shroud (leather set)
   - MeleeDPS/Support/Hybrid → Steamwork Exosuit (plate set)
6. **Regional Potions** - Based on class region (10 of each type)
7. **Alchemist Special** - Gets ALL potions (143 total - they're the potion master!)

### Testing Checklist: Classes

| Test | Steps | Expected Result |
|------|-------|-----------------|
| Assign Beginner | Click "B" next to any class | Class assigned, basic stats |
| Assign Maxed | Click "M" next to any class | Class + gear + potions |
| Magic Class Maxed | Click "M" on Ice Mage | Gets Ice spellbook + gear |
| Martial Class Maxed | Click "M" on Barbarian | Gets legendary axe + gear |
| Alchemist Maxed | Click "M" on Alchemist | Gets 143 potions! |

---

## Items Tab

### Sub-Pages

The Items tab has **6 sub-pages**:

| Sub-Page | Contents |
|----------|----------|
| **Spellbooks** | 12 spellbooks + Songbook of Weaving (custom range) |
| **Reagents** | Reagent packs by magic school |
| **Weapons** | All 40 regional weapons + 5 legendary |
| **Armor** | All 59 regional armor pieces + shields |
| **Focus Items** | All 16 special ability items |
| **Consumables** | All 8 regional potions + batch actions |

### Spawning Weapons

**Regional Weapons (40):**
- Click weapon name to spawn 1
- Organized by type: Swords, Axes, Maces, Polearms, Ranged

**Legendary Weapons (5):**
- The Eternal Winter (Frosthold)
- Phoenix Ascension (Emberlands)
- The Cogmaster (Ironclad)
- Prismatic Edge (Crystal)
- Voidcaller (ShadowVoid)

### Spawning Armor

**By Type:**
- Plate Armor (24 pieces)
- Chain Armor (9 pieces)
- Ring Armor (8 pieces)
- Leather Armor (18 pieces)

**Legendary Sets (19 pieces):**
- Glacial Aegis Set (6 plate pieces)
- Steamwork Exosuit Set (6 plate pieces)
- Shadow Shroud Set (6 leather pieces)
- Molten Core Chest (1 unique)

### Spawning Spellbooks

| Spellbook | Class | Spells |
|-----------|-------|--------|
| Ice Mage Spellbook | Ice Mage | 32 ice spells |
| Druid Spellbook | Druid | 32 nature spells |
| Witch Spellbook | Witch | 32 hex spells |
| Sorcerer Spellbook | Sorcerer | 32 elemental spells |
| Warlock Spellbook | Warlock | 32 dark spells |
| Oracle Spellbook | Oracle | 32 divination spells |
| Necromancer Spellbook | Necromancer | 32 necromancy spells |
| Summoner Spellbook | Summoner | 32 summoning spells |
| Shaman Spellbook | Shaman | 32 shamanic spells |
| Songbook of Weaving | Bard | Songweaving spellbook (drag/hotkey) + commands |
| Enchanter Spellbook | Enchanter | 32 enchanting spells |
| Illusionist Spellbook | Illusionist | 32 illusion spells |

### Spawning Consumables

**Resistance Potions:**
| Button | Potion | Effect |
|--------|--------|--------|
| Cold Resist | Cold Resistance Potion | +25 Cold Resist, 10 min |
| Heat Resist | Heat Resistance Potion | +25 Fire Resist, 10 min |
| Poison Resist | Poison Resistance Potion | +25 Poison Resist, 10 min |
| Energy Resist | Energy Resistance Potion | +25 Energy Resist, 10 min |

**Enhancement Potions:**
| Button | Potion | Effect |
|--------|--------|--------|
| Nature's Blessing | Nature's Blessing Potion | +5 HP Regen, 15 min |
| Crystal Clarity | Crystal Clarity Potion | +15 INT, See Hidden, 20 min |
| Desert Swiftness | Desert Swiftness Potion | +10 DEX, 10 min |
| Ironclad Fortitude | Ironclad Fortitude Potion | +10 STR, 10 min |

**Batch Actions:**
- "All Resistance" - Spawns 10 of each resistance potion
- "All Enhancement" - Spawns 10 of each enhancement potion
- "All Potions" - Spawns 10 of everything

### Testing Checklist: Items

| Test | Steps | Expected Result |
|------|-------|-----------------|
| Spawn weapon | Click weapon name | Weapon in backpack |
| Spawn legendary | Click legendary name | Artifact-quality weapon |
| Spawn armor set | Click each piece | Full set in backpack |
| Spawn spellbook | Click spellbook name | Spellbook with 32 spells |
| Use spellbook | Double-click spellbook | Opens spell gump |
| Spawn potions | Click potion button | Potions in backpack |
| Use potion | Double-click potion | Effect applied, message shown |
| Batch potions | Click "All Potions" | 80 potions total |

---

## Tab 3: Skills

### Skill Overview

Vystia adds **26 custom skills** (IDs 58-83):

**Magic Skills (58-69):**
| ID | Skill | Class |
|----|-------|-------|
| 58 | Cryomancy | Ice Mage |
| 59 | Demonology | Warlock |
| 60 | Necromancy Arts | Necromancer |
| 61 | Druidism | Druid |
| 62 | Elementalism | Sorcerer |
| 63 | Songweaving | Bard |
| 64 | Hexcraft | Witch |
| 65 | Divination | Oracle |
| 66 | Conjuration | Summoner |
| 67 | Spirit Calling | Shaman |
| 68 | Runeweaving | Enchanter |
| 69 | Illusion Magic | Illusionist |

**Martial Skills (70-83):**
| ID | Skill | Class |
|----|-------|-------|
| 70 | Berserking | Barbarian |
| 71 | Subterfuge | Rogue |
| 72 | Martial Arts | Monk |
| 73 | Chivalric Arts | Knight |
| 74 | Holy Devotion | Paladin |
| 75 | Marksmanship | Ranger |
| 76 | Combat Mastery | Fighter |
| 77 | Zealotry | Templar |
| 78 | Manhunting | Bounty Hunter |
| 79 | Beast Bonding | Beastmaster |
| 80 | Engineering | Artificer |
| 81 | Transmutation | Alchemist |
| 82 | Divine Grace | Cleric |
| 83 | Arcane Studies | Wizard |

### Skill Actions

| Button | Action |
|--------|--------|
| "100" next to skill | Set that skill to 100 |
| "0" next to skill | Set that skill to 0 |
| "Class Skill to 100" | Max the currently assigned class skill |
| "Reset All to 0" | Reset all 26 Vystia skills to 0 |

### Console Commands

Additional skill commands:
```
[rvs              - Reset all Vystia skills to 0
[svs <value>      - Set all Vystia skills to value (e.g., [svs 100)
[skillcap [VAlue] - View/set total skill cap (default: 84000 for Vystia)
[skillinfo        - Display Vystia skill info
```

### Testing Checklist: Skills

| Test | Steps | Expected Result |
|------|-------|-----------------|
| Set skill to 100 | Click "100" next to Cryomancy | Skill shows 100.0 |
| Reset skill | Click "0" next to Cryomancy | Skill shows 0.0 |
| Reset all | Click "Reset All to 0" | All 26 skills at 0.0 |
| Verify in skills gump | Use `[skills` command | Skills visible in native gump |

---

## Spawning Test Creatures

Use `[spawnvystia` command to spawn Vystia creatures for testing:

```
[spawnvystia              - Opens creature spawn gump
[spawnvystia 10           - Spawn with 10-tile radius selection
```

**By Region:**
- Frosthold creatures (ice/cold themed)
- Emberlands creatures (fire themed)
- Shadowfen creatures (poison/swamp)
- Verdantpeak, Crystal Barrens, Ironclad, Skyreach, Underwater, ShadowVoid

**Boss Creatures:**
- Regional bosses for testing high-end abilities

### Testing Checklist: Creatures

| Test | Steps | Expected Result |
|------|-------|-----------------|
| Spawn weak mob | `[spawnvystia` → select tier 1 | Easy kill for testing |
| Spawn boss | `[spawnvystia` → select boss | High HP target for combos |
| Test AoE | Spawn multiple mobs | Multiple targets in range |

---

## Complete Class Testing Procedure

### Step-by-Step: Testing a Magic Class

**Example: Ice Mage**

1. **Setup**
   ```
   [VA → Classes tab → Click "M" next to Ice Mage
   ```
   - You now have: Ice Mage class, Ice Spellbook, Legendary gear, Potions

2. **Verify Spellbook**
   - Open backpack
   - Double-click Ice Mage Spellbook
   - Verify all 32 spells present (8 circles × 4 spells)

3. **Verify Skills**
   ```
   [VA → Skills tab
   ```
   - Cryomancy should show 100.0

4. **Spawn Test Target**
   ```
   [VA → Creatures tab → Spawn a mid-tier creature
   ```

5. **Test Spell Casting**
   - Open spellbook
   - Cast Circle 1 spell: Frost Touch
   - Cast Circle 3 spell: Ice Bolt
   - Cast Circle 6 spell: Blizzard
   - Cast Circle 8 spell: Absolute Zero

6. **Test Combo**
   ```
   Frost Touch → Freezing Grasp → Ice Bolt × 3 (build 5 Chill)
   → Target should be FROZEN
   → Shatter (bonus damage)
   → Blizzard (AoE cleanup)
   ```

7. **Verify Results**
   - Spells cast successfully
   - Damage numbers appear
   - Chill stacks apply
   - Freeze effect triggers at 5 stacks

---

### Step-by-Step: Testing a Martial Class

**Example: Barbarian**

1. **Setup**
   ```
   [VA → Classes tab → Click "M" next to Barbarian
   ```
   - You now have: Barbarian class, The Eternal Winter, Legendary armor, Potions

2. **Verify Equipment**
   - Open backpack
   - Equip The Eternal Winter (2H axe)
   - Equip Glacial Aegis armor set

3. **Verify Skills**
   ```
   [VA → Skills tab
   ```
   - Berserking should show 100.0

4. **Spawn Test Target**
   ```
   [VA → Creatures tab → Spawn a boss creature
   ```

5. **Test Abilities**
   - Use ability hotkeys or ability gump
   - Circle 1: Reckless Strike, Frost Fury
   - Circle 2: Whirlwind, Berserker Rage
   - Circle 3: Avalanche Strike, Frenzy
   - Circle 4: Wrath of the North, Primal Avatar

6. **Test Combo**
   ```
   Build Fury in combat (auto-generates)
   → Berserker Rage (+50% damage)
   → Reckless Strike (big hit)
   → Whirlwind (AoE)
   → Avalanche Strike (ice AoE)
   → When low HP: Frenzy (+50% speed)
   → Rampage (5 hits)
   → Wrath of the North (ultimate)
   ```

7. **Verify Results**
   - Fury generates in combat
   - Rage mode activates (visual effect)
   - Abilities deal damage
   - Transforms work

---

## Testing All 26 Classes

### Magic Classes Quick Test

| Class | Maxed Setup | Key Spell to Test | Combo End |
|-------|------------|-------------------|-----------|
| Ice Mage | M button | Ice Bolt (C3) | Shatter on Frozen |
| Druid | M button | Wild Shape (C5) | Tree of Life |
| Witch | M button | Bog Curse (C2) | Death Bloom |
| Sorcerer | M button | Magma Bolt (C3) | Elemental Embodiment |
| Warlock | M button | Eldritch Blast (C2) | Summon Patron (3 shards) |
| Oracle | M button | Psychic Bolt (C5) | Crystal Communion |
| Necromancer | M button | Raise Undead (C3) | Army of the Dead |
| Summoner | M button | Summon Elemental (C3) | Mass Summon |
| Shaman | M button | Elemental Totem (C4) | Ancestral Guidance |
| Bard | M button | Inspiring Song (C2) | Symphony of War (10 Crescendo) |
| Enchanter | M button | Enchant Weapon (C3) | Runic Trap |
| Illusionist | M button | Phantom Duplicate (C3) | Reality Shatter |

### Martial Classes Quick Test

| Class | Maxed Setup | Key Ability to Test | Combo End |
|-------|------------|---------------------|-----------|
| Barbarian | M button | Berserker Rage | Primal Avatar |
| Beastmaster | M button | Call Pet | Alpha Predator |
| Fighter | M button | Power Strike | Avatar of War |
| Monk | M button | Flurry of Blows | Touch of Death |
| Templar | M button | Judgment Strike | Iron Justicar |
| Ranger | M button | Aimed Shot | Kill Shot (execute) |
| Knight | M button | Challenge | Azure Tide |
| Paladin | M button | Crusader Strike | Wrath of Righteous |
| Rogue | M button | Sinister Strike | Death from Above |
| Bounty Hunter | M button | Mark Target | Execution Contract |
| Artificer | M button | Deploy Turret | Mech Suit |
| Alchemist | M button | Fire Bomb | Philosopher's Bomb |
| Cleric | M button | Heal | Divine Intervention |
| Wizard | M button | Arcane Bolt | Meteor |

---

## Complete Magic School Testing Reference (2025-12-30)

**Total:** 352 spells across 11 magic schools (32 per school) + Songweaving songs/finales (15)
**Spell IDs:** 1000-1383

### Universal Spell Properties

| Circle | Mana | Skill Req | Fizzle Range | Typical Damage |
|--------|------|-----------|--------------|----------------|
| 1 | 4 | 0 | - | 4-12 |
| 2 | 6-8 | 10 | 0-10 | 8-18 |
| 3 | 9-11 | 20 | 0-20 | 12-22 |
| 4 | 11-14 | 30 | 0-30 | 15-28 |
| 5 | 14-17 | 45 | 0-45 | 20-35 |
| 6 | 20-24 | 60 | 0-60 | 24-40 |
| 7 | 40 | 75 | 0-75 | 45-80 |
| 8 | 50 | 90 | 0-90 | 55-100+ |

---

### 1. ICE MAGIC (Ice Mage)
**Skill:** Cryomancy (ID 58) | **Spell IDs:** 1000-1031 | **Spellbook:** IceMageSpellbook

#### Reagents
| Reagent | Command | Circles |
|---------|---------|---------|
| Frostbloom | `[add Frostbloom 50` | 1-3 |
| Winterleaf | `[add Winterleaf 50` | 1-4 |
| GlacierCrystal | `[add GlacierCrystal 50` | 3-5 |
| PermafrostEssence | `[add PermafrostEssence 30` | 5-6 |
| ArcticPearl | `[add ArcticPearl 20` | 6-8 |
| FrozenSoul | `[add FrozenSoul 20` | 6-8 |
| FrostEssence | `[add FrostEssence 20` | 7-8 |
| HeartOfWinter | `[add HeartOfWinter 10` | 8 |

#### Complete Spell Table
| Circle | Spell | Damage | Target | Radius | Effects | Reagents |
|--------|-------|--------|--------|--------|---------|----------|
| 1 | Frost Touch | 4-8 | Single | - | 10% slow | Frostbloom, Winterleaf |
| 1 | Ice Shard | 5-10 | Single | - | Piercing | Frostbloom, Winterleaf |
| 1 | Frost Ward | - | Self | - | +10 Cold Resist 60s | Frostbloom |
| 1 | Avalanche | 6-12 | AoE | 3 | Knockback | Frostbloom, Winterleaf |
| 2 | Freezing Grasp | 8-14 | Single | - | -10 DEX 5s | Winterleaf, GlacierCrystal |
| 2 | Ice Shield | - | Self | - | Absorb 30 dmg | Winterleaf |
| 2 | Frost Slick | - | Ground | 4 | Slow zone | Frostbloom, Winterleaf |
| 2 | Glacial Mend | Heal 15-25 | Single | - | Cold HoT | Winterleaf |
| 3 | **Ice Bolt** | 19 base | Single | - | **25% slow (-15 DEX 5s)** | Frostbloom, Winterleaf, GlacierCrystal |
| 3 | Frostbite | 12-18 | Single | - | DoT 3-5/tick 8s | GlacierCrystal |
| 3 | Frozen Ground | - | Ground | 5 | Slow zone 15s | GlacierCrystal |
| 3 | Ice Spear | 15-22 | Single | - | Armor pierce | GlacierCrystal |
| 4 | Frost Armor | - | Self | - | +25 AR 120s | PermafrostEssence |
| 4 | Ice Wall | - | Ground | - | Creates barrier | GlacierCrystal |
| 4 | Icicle Barrage | 18-28 | Single | - | 5 hits 4-6 each | PermafrostEssence |
| 4 | Permafrost | 15-25 | AoE | 4 | Permanent slow | PermafrostEssence |
| 5 | Glacial Strike | 25-35 | Single | - | 40% freeze 2s | PermafrostEssence, ArcticPearl |
| 5 | Frozen Tomb | - | Single | - | 5s stasis | ArcticPearl |
| 5 | **Shatter** | 30-50 | Single | - | **+50% vs frozen** | PermafrostEssence |
| 5 | Hypothermia | 20-30 | Single | - | -20 all stats 10s | PermafrostEssence |
| 6 | **Blizzard** | 3-8/tick | Ground | 5 | **DoT zone 10s** | PermafrostEssence, ArcticPearl, FrozenSoul |
| 6 | Glacial Fortress | - | Self | - | Barrier + reflect | ArcticPearl, FrozenSoul |
| 6 | Deep Freeze | 25-40 | Single | - | 100% freeze 4s | ArcticPearl, FrozenSoul |
| 6 | Chill Aura | 5-10/tick | Caster | 3 | Aura DoT 20s | FrozenSoul |
| 7 | **Absolute Zero** | 50-80 | Ground | 6 | **Freeze 3s + ground** | ArcticPearl, FrozenSoul, FrostEssence |
| 7 | Glacier Summon | - | Ground | - | Ice Elemental | FrozenSoul, FrostEssence |
| 7 | Eternal Winter | - | Ground | 8 | Permanent blizzard | FrostEssence, HeartOfWinter |
| 7 | **Fimbulwinter's Wrath** | 80-120 | Caster | 10 | **Ultimate +50% freeze** | ArcticPearl, FrozenSoul, FrostEssence, HeartOfWinter |
| 8 | Frost Meteor | 70-100 | Ground | 6 | Massive impact | HeartOfWinter |
| 8 | Ice Age | 40-60/tick | Ground | 10 | Map-wide slow | HeartOfWinter |
| 8 | Rime Reaper | 80-120 | Single | - | Execute <20% HP | HeartOfWinter |
| 8 | Cocytus Prison | - | Ground | 8 | Mass freeze 6s | HeartOfWinter |

#### Finale List (Current Implementation)
- Sharp Note (5)
- Mesmerise (5)
- Cacophony (10)
- Fortissimo (15)
- Soothing Chorus (15)
- Symphony of Destruction (20)

#### Testing Checklist
- [ ] Ice Bolt deals ~19 damage with 25% slow chance (-15 DEX)
- [ ] Blizzard creates 5-tile DoT zone for 10 seconds
- [ ] Absolute Zero freezes targets for 3 seconds in 6-tile radius
- [ ] Fimbulwinter's Wrath hits 80+ damage in 10 tile radius with 50% freeze
- [ ] Shatter deals +50% bonus damage to frozen targets

---

### 2. NATURE MAGIC (Druid)
**Skill:** Druidism (ID 61) | **Spell IDs:** 1032-1063 | **Spellbook:** DruidSpellbook

#### Reagents
| Reagent | Command | Circles |
|---------|---------|---------|
| WildMoss | `[add WildMoss 50` | 1-3 |
| Moonpetal | `[add Moonpetal 50` | 1-4 |
| LivingBark | `[add LivingBark 30` | 3-5 |
| ElderwoodSeed | `[add ElderwoodSeed 30` | 4-6 |
| PrimalVine | `[add PrimalVine 20` | 5-7 |
| AncientSap | `[add AncientSap 20` | 6-8 |
| WorldTreeLeaf | `[add WorldTreeLeaf 10` | 7-8 |
| HeartOfTheForest | `[add HeartOfTheForest 10` | 8 |

#### Complete Spell Table
| Circle | Spell | Damage/Heal | Target | Radius | Effects |
|--------|-------|-------------|--------|--------|---------|
| 1 | Nature's Touch | Heal 8-15 | Single | - | Minor heal |
| 1 | **Thorn Dart** | 4-10 | Single | - | 100% poison dmg |
| 1 | Barkskin | - | Self | - | +15 AR 60s |
| 1 | Detect Life | - | Self | 15 | Reveals hidden |
| 2 | Entangle | - | Single | - | Root 4s |
| 2 | Wild Growth | - | Ground | 3 | Healing zone |
| 2 | Nature's Blessing | - | Single | - | +10 all stats 60s |
| 2 | Poison Spores | 8-14 | AoE | 3 | Poison DoT |
| 3 | Rejuvenation | Heal 20-35 | Single | - | HoT 5/tick 10s |
| 3 | Spore Cloud | 12-20 | AoE | 4 | Poison + confuse |
| 3 | Strangling Vines | 10-18 | Single | - | Root + DoT |
| 3 | Thorn Volley | 15-25 | AoE | 4 | Multi-target |
| 4 | Greater Regen | HoT 8/tick | Single | - | 20s duration |
| 4 | Nature's Wrath | 20-30 | AoE | 5 | Physical dmg |
| 4 | Bear Form | - | Self | - | Transform +30 STR |
| 4 | Plague | - | AoE | 5 | Spreads poison |
| 5 | **Earthquake** | 20-35 | Ground | 5 | **2s stun + destroy ice** |
| 5 | Force of Nature | 25-40 | Single | - | Summons treant |
| 5 | Healing Grove | HoT 10/tick | Ground | 5 | Zone heal 15s |
| 5 | Wolf Form | - | Self | - | Transform +20 DEX |
| 6 | Ancient Treant | - | - | - | Powerful summon |
| 6 | Living Fortress | - | Self | - | +50 AR immobile |
| 6 | Primordial Restore | Heal 50-80 | AoE | 5 | Mass heal |
| 6 | Toxic Bloom | 25-40 | AoE | 5 | Poison explosion |
| 7 | Avatar of Forest | - | Self | - | Ultimate transform |
| 7 | World Tree Embrace | Heal 80-120 | AoE | 8 | Ultimate heal |
| 7 | Hydra Form | - | Self | - | Legendary transform |
| 7 | Thorn Apocalypse | 45-70 | AoE | 8 | Mass thorns |
| 8 | Ultimate spells | 60-100+ | Various | - | Apocalyptic effects |

---

### 3. HEX MAGIC (Witch)
**Skill:** Hexcraft (ID 64) | **Spell IDs:** 1064-1095 | **Spellbook:** WitchSpellbook

#### Reagents
| Reagent | Command | Circles |
|---------|---------|---------|
| ToadsEye | `[add ToadsEye 50` | 1-4 |
| SwampLotus | `[add SwampLotus 50` | 1-4 |
| HagsHair | `[add HagsHair 30` | 3-6 |
| CursedPearl | `[add CursedPearl 30` | 4-7 |
| WitchBlood | `[add WitchBlood 20` | 5-7 |
| DarkMoonflower | `[add DarkMoonflower 20` | 6-8 |
| SoulOfTheDamned | `[add SoulOfTheDamned 10` | 7-8 |
| WitchQueensTear | `[add WitchQueensTear 10` | 8 |

#### Complete Spell Table
| Circle | Spell | Effect | Target | Duration | Special |
|--------|-------|--------|--------|----------|---------|
| 1 | Weak Curse | -5 stat | Single | 30s | Random stat |
| 1 | Evil Eye | -10 hit | Single | 20s | Accuracy debuff |
| 1 | Hex Bolt | 5-10 | Single | - | Poison damage |
| 1 | Minor Drain | 5-8 drain | Single | - | Heals caster |
| 2 | Wither | -15 STR | Single | 30s | STR debuff |
| 2 | Jinx | Bad luck | Single | 60s | +10% fizzle |
| 2 | Curse Frailty | -20 AR | Single | 30s | Defense down |
| 2 | Life Tap | 10-15 drain | Single | - | Heals caster |
| 3 | Enfeeble | -20 all | Single | 20s | Major debuff |
| 3 | Hex of Pain | 8-15+DoT | Single | 10s | 3-5/tick |
| 3 | Soul Leech | 15-25 drain | Single | - | Heals caster |
| 3 | Crippling Curse | -30 DEX | Single | 30s | Movement slow |
| 4 | Blight | 15-25 | AoE | 4 | Poison spreads |
| 4 | **Hex of Agony** | 3-5/tick | Single | 8-12s | **ANTI-HEAL** |
| 4 | Curse Weakness | -25 STR/DEX | Single | 45s | Dual debuff |
| 4 | Drain Life | 20-30 drain | Single | - | Heals 100% |
| 5 | Torment | 25-40+DoT | Single | 15s | 5-8/tick |
| 5 | Hex of Doom | -30 all | Single | 30s | Major debuff |
| 5 | Soul Siphon | 30-45 drain | Single | - | Mana+HP drain |
| 5 | Greater Curse | -40 random | Single | 60s | Massive debuff |
| 6 | Plague | 20-30 | AoE | 6 | Contagious |
| 6 | **Hex Storm** | 20-30 | Ground | 5 | **Random hex** |
| 6 | Life Drain | 40-60 drain | Single | - | Full heal |
| 6 | Mass Hex | -20 all | AoE | 5 | Group debuff |
| 7 | Damnation | 50-80+curse | Single | 60s | Ultimate curse |
| 7 | Witch's Bane | -50 all | Single | 30s | Devastating |
| 7 | Soul Harvest | 60-100 drain | AoE | 5 | Mass drain |
| 7 | Fatal Curse | Execute | Single | - | Kills <15% HP |
| 8 | Apocalyptic Hex | 80-120 | AoE | 8 | Ultimate |
| 8 | Unspeakable Hex | Varies | Single | Perm | Legendary |

#### HEX OF AGONY - Critical Testing
**CONVERTS ALL HEALING TO DAMAGE!**

| Property | Value |
|----------|-------|
| Circle | 4 |
| Mana | 11 |
| Duration | 8-12s (8 + Hexcraft/25) |
| DoT | 3-5 poison/second |
| Visual | Yellow healthbar |
| Anti-Heal | 100% healing → poison damage |

**Test Procedure:**
```
1. Cast on creature → Yellow healthbar appears
2. Watch DoT ticks (3-5 damage/second)
3. Cast healing spell on target
4. RESULT: Healing deals POISON DAMAGE instead
5. Wait 8-12s → Yellow healthbar disappears
```

#### HEX STORM - Random Effects
| Roll | Hex | Effect | Duration |
|------|-----|--------|----------|
| 0 | Weakness | -15 STR | 6+(Hexcraft/20)s |
| 1 | Lethargy | -20 DEX | 6+(Hexcraft/20)s |
| 2 | Confusion | -20 INT | 6+(Hexcraft/20)s |
| 3 | Venom | Greater Poison | Until cured |

---

### 4. ELEMENTAL MAGIC (Sorcerer)
**Skill:** Elementalism (ID 62) | **Spell IDs:** 1096-1127 | **Spellbook:** SorcererSpellbook

#### Reagents
| Reagent | Command | Circles |
|---------|---------|---------|
| EmberDust | `[add EmberDust 50` | 1-3 |
| VolcanicAsh | `[add VolcanicAsh 50` | 2-4 |
| Moltenite | `[add Moltenite 30` | 3-5 |
| PrimordialEmber | `[add PrimordialEmber 30` | 4-6 |
| DragonHeart | `[add DragonHeart 20` | 5-7 |
| PhoenixFeather | `[add PhoenixFeather 20` | 6-8 |
| LavaCore | `[add LavaCore 10` | 7-8 |
| HeartOfTheVolcano | `[add HeartOfTheVolcano 10` | 8 |

#### Complete Spell Table
| Circle | Spell | Damage | Target | Radius | Element |
|--------|-------|--------|--------|--------|---------|
| 1 | Flame Bolt | 5-12 | Single | - | 100% Fire |
| 1 | Ignite | 3-6+DoT | Single | - | Fire 2/tick |
| 1 | Heat Shield | - | Self | - | +15 Fire Resist |
| 1 | Smoke Screen | - | Ground | 3 | Obscures vision |
| 2 | Fireball | 10-18 | Single | - | 100% Fire |
| 2 | Ember Burst | 8-14 | AoE | 3 | Fire splash |
| 2 | Combustion | 12-20 | Single | - | Ignites target |
| 2 | Flame Ward | - | Self | - | Fire absorb |
| 3 | Flame Pillar | 15-25 | Ground | 2 | DoT zone |
| 3 | Incinerate | 18-28 | Single | - | Burns armor |
| 3 | Magma Armor | - | Self | - | +30 AR + aura |
| 3 | Volcanic Rock | 20-30 | Single | - | Phys+Fire |
| 4 | Inferno | 22-35 | AoE | 4 | Fire explosion |
| 4 | Lava Flow | 15-25/tick | Ground | 5 | DoT zone 10s |
| 4 | Ring of Fire | - | Caster | 3 | Protection |
| 4 | Flame Tempest | 25-40 | AoE | 5 | Swirling fire |
| 5 | Meteor Strike | 35-50 | Ground | 4 | Impact+splash |
| 5 | Pyroclasm | 30-45 | AoE | 5 | Eruption |
| 5 | Phoenix Shield | - | Self | - | Rebirth on death |
| 5 | Solar Flare | 40-60 | AoE | 6 | Blind + damage |
| 6 | Volcanic Eruption | 40-60 | Ground | 6 | Massive AoE |
| 6 | **Hellfire Nova** | 25-35 | Caster | 5 | **Caster-centered AoE** |
| 6 | Cataclysm | 45-70 | Ground | 7 | Destruction |
| 6 | Avatar of Flame | - | Self | - | Fire transform |
| 7 | Primordial Inferno | 60-90 | Ground | 8 | Ultimate fire |
| 7 | Fire Lord | - | - | - | Powerful summon |
| 7 | Molten Titan | - | Self | - | Ultimate transform |
| 7 | Pyroclastic Flow | 50-80 | Line | 10 | Line of fire |
| 8 | Elemental Apocalypse | 80-120 | Ground | 10 | Ultimate |

#### HELLFIRE NOVA - Testing
| Property | Value |
|----------|-------|
| Circle | 6 |
| Mana | 24 |
| Target | Caster-centered (no targeting) |
| Radius | 5 tiles |
| Damage | 25-35 + (Elementalism × 0.15) |
| Type | 100% Fire |
| Visual | Dual fire explosion |
| Reagents | DragonHeart (1), PrimordialEmber (1) |

---

### 5. DARK MAGIC (Warlock)
**Skill:** Demonology (ID 59) | **Spell IDs:** 1128-1159 | **Spellbook:** WarlockSpellbook

#### Reagents
| Reagent | Command | Circles |
|---------|---------|---------|
| VoidCrystal | `[add VoidCrystal 50` | 1-3 |
| VoidWeed | `[add VoidWeed 50` | 1-4 |
| ShadowPetal | `[add ShadowPetal 30` | 2-5 |
| VoidSilk | `[add VoidSilk 30` | 4-6 |
| DemonHeart | `[add DemonHeart 20` | 5-8 |
| ShadowEssence | `[add ShadowEssence 20` | 6-8 |
| VoidCore | `[add VoidCore 10` | 7-8 |
| DemonPrinceTear | `[add DemonPrinceTear 10` | 8 |

#### Complete Spell Table
| Circle | Spell | Damage | Target | Radius | Effects |
|--------|-------|--------|--------|--------|---------|
| 1 | Shadow Bolt | 5-12 | Single | - | Energy damage |
| 1 | Dark Pact | - | Self | - | +15% dmg -10 HP |
| 1 | Corruption | 3-5/tick | Single | 10s | Poison DoT |
| 1 | Fear | - | Single | - | Fear 3s |
| 2 | **Chaos Bolt** | 12-18 | Single | - | **Random element** |
| 2 | Demonic Armor | - | Self | - | +20 AR dark aura |
| 2 | Life Leech | 10-18 | Single | - | Heals caster |
| 2 | Nightmare | - | Single | - | Sleep + DoT |
| 3 | **Shadow Nova** | 12-18 | Caster | 4 | **Caster-centered** |
| 3 | Summon Imp | - | - | - | Familiar |
| 3 | Soul Drain | 15-25 | Single | - | Mana+HP |
| 3 | Darkness | - | Ground | 5 | Vision block |
| 4 | Shadow Storm | 20-32 | AoE | 4 | Energy AoE |
| 4 | Demon Skin | - | Self | - | +40 AR |
| 4 | Rain of Fire | 18-28/tick | Ground | 5 | DoT zone |
| 4 | Terror | - | AoE | 5 | Mass fear 4s |
| 5 | Soul Shatter | 30-45 | Single | - | Mana burn |
| 5 | Summon Voidwalker | - | - | - | Tank summon |
| 5 | Shadowflame | 25-40 | AoE | 4 | Shadow+Fire |
| 5 | Chaotic Void | 30-50 | Single | - | Random effects |
| 6 | Dark Ritual | - | Self | - | +50% dmg 30s |
| 6 | **Chaos Storm** | 28-38 | Ground | 5 | **Random element/target** |
| 6 | Demon Form | - | Self | - | Transform |
| 6 | Mass Fear | - | AoE | 6 | Fear all 5s |
| 7 | Soul Harvest | 50-80 | AoE | 5 | Mass drain |
| 7 | Summon Doomguard | - | - | - | Elite summon |
| 7 | **Apocalyptic Chaos** | 45-65 | Ground | 6 | **2s stun -15 all stats** |
| 7 | Void Zone | 40-60/tick | Ground | 6 | Death zone |
| 8 | Warlock Apocalypse | 80-120 | AoE | 10 | Ultimate |
| 8 | Demonic Ascension | - | Self | - | Ultimate transform |
| 8 | Demon Prince | - | - | - | Legendary summon |

#### CHAOS DAMAGE TYPES
| Roll | Element | Color |
|------|---------|-------|
| 0 | Physical | Grey (0x455) |
| 1 | Fire | Red (0x21) |
| 2 | Cold | Blue (0x481) |
| 3 | Poison | Green (0x1A) |
| 4 | Energy | Purple (0x496) |

#### APOCALYPTIC CHAOS - Testing
| Property | Value |
|----------|-------|
| Circle | 7 |
| Mana | 40 |
| Radius | 6 tiles |
| Damage | 45-65 + (Demonology × 0.2) |
| Stun | 2 seconds (Paralyzed) |
| Debuff | -15 STR, -15 DEX, -15 INT |
| Duration | 8 + (Demonology/12) seconds |
| Reagents | DemonHeart (2), ShadowEssence (1) |

---

### 6. DIVINATION MAGIC (Oracle)
**Skill:** Divination (ID 65) | **Spell IDs:** 1160-1191 | **Spellbook:** OracleSpellbook

#### Reagents
| Reagent | Command |
|---------|---------|
| CrystalDust | `[add CrystalDust 50` |
| PropheticLeaf | `[add PropheticLeaf 50` |
| TimeSand | `[add TimeSand 30` |
| SeeingStone | `[add SeeingStone 30` |
| FateCrystal | `[add FateCrystal 20` |
| EternalGlass | `[add EternalGlass 20` |
| ChronoShard | `[add ChronoShard 10` |
| OraclesTear | `[add OraclesTear 10` |

#### Key Spells
| Circle | Spell | Effect | Notes |
|--------|-------|--------|-------|
| 2 | Prismatic Bolt | 10-18 dmg | Random element |
| 4 | Time Warp | +50% speed 10s | Haste |
| 6 | **Prismatic Storm** | 22-32 dmg | **AoE 5 tiles random element** |
| 6 | Time Stop | Freeze 3s | AoE 5 |
| 7 | Chrono Lord | Transform | Time master |
| 8 | Timeless State | Immune 5s | Invulnerability |

#### PRISMATIC STORM - Testing
| Property | Value |
|----------|-------|
| Circle | 6 |
| Mana | 24 |
| Target | Ground-targeted AoE |
| Radius | 5 tiles |
| Damage | 22-32 + (Divination × 0.12) |
| Type | Random element per target |
| Visual | Multi-colored particles |
| Reagents | PropheticLeaf (1), SeeingStone (1) |

---

### 7. NECROMANCY (Necromancer)
**Skill:** NecromancyArts (ID 60) | **Spell IDs:** 1192-1223 | **Spellbook:** NecromancerSpellbook

#### Reagents
| Reagent | Command |
|---------|---------|
| GraveMoss | `[add GraveMoss 50` |
| BoneDust | `[add BoneDust 50` |
| CorpseAsh | `[add CorpseAsh 30` |
| SoulFragment | `[add SoulFragment 30` |
| DeathEssence | `[add DeathEssence 20` |
| LichDust | `[add LichDust 20` |
| PhylacteryChip | `[add PhylacteryChip 10` |
| DeathKnightSoul | `[add DeathKnightSoul 10` |

#### Key Spells
| Circle | Spell | Damage | Target | Effects |
|--------|-------|--------|--------|---------|
| 1 | **Death Bolt** | 5-10 | Single | Energy damage |
| 1 | Animate Bone | - | Corpse | Raise skeleton |
| 3 | Death Grip | 15-25 | Single | Pull + damage |
| 5 | Mass Raise | - | AoE | Raise all corpses |
| 6 | Corpse Explosion | 35-55 | Corpse | 5 tile radius |
| 6 | Army of Dead | - | Ground | Mass summon |
| 7 | Lich Form | - | Self | Undead transform |
| 8 | Undead Dragon | - | - | Ultimate summon |

---

### 8. SUMMONING MAGIC (Summoner)
**Skill:** Conjuration (ID 66) | **Spell IDs:** 1224-1255 | **Spellbook:** SummonerSpellbook

#### Reagents
| Reagent | Command | Circles |
|---------|---------|---------|
| PlanarDust | `[add PlanarDust 50` | 1-3 |
| EtherShard | `[add EtherShard 50` | 1-4 |
| SummoningCircle | `[add SummoningCircle 30` | 3-5 |
| SpiritChain | `[add SpiritChain 30` | 4-6 |
| BeastEssence | `[add BeastEssence 20` | 5-7 |
| SummoningSalt | `[add SummoningSalt 20` | 6-8 |
| PlanarGem | `[add PlanarGem 10` | 7-8 |
| DimensionalKey | `[add DimensionalKey 10` | 8 |

#### Complete Spell Table
| Circle | Spell | Type | Target | Duration | Effects |
|--------|-------|------|--------|----------|---------|
| 1 | **Summon Rabbit** | Summon | Ground | 10 min | 2 follower slots |
| 1 | Summon Wisp | Summon | Ground | 10 min | Light source |
| 1 | Mend Summon | Heal | Summon | - | Heal 15-25 HP |
| 1 | Arcane Bolt | 10-18 dmg | Single | - | Energy damage |
| 2 | Summon Wolf | Summon | Ground | 10 min | Combat pet |
| 2 | Summon Bear | Summon | Ground | 10 min | Tank pet |
| 2 | Unsummon | Dismiss | Summon | - | Remove summon |
| 2 | Summon Fire Sprite | Summon | Ground | 10 min | Fire damage |
| 3 | Summon Air Elemental | Summon | Ground | 15 min | Flying |
| 3 | Summon Earth Elemental | Summon | Ground | 15 min | High HP |
| 3 | Empower Summon | Buff | Summon | 60s | +25% damage |
| 3 | Greater Heal Summon | Heal | Summon | - | Heal 40-60 HP |
| 4 | Summon Storm Elemental | Summon | Ground | 15 min | Lightning |
| 4 | Summon Void Elemental | Summon | Ground | 15 min | Drains |
| 4 | Summon Frenzy | Buff | All Summons | 30s | +50% attack speed |
| 4 | Symbiotic Link | Link | Summon | 120s | Share HP |
| 5 | Summon Drake | Summon | Ground | 20 min | Dragon-type |
| 5 | Summon Elemental Lord | Summon | Ground | 20 min | Powerful |
| 5 | Mass Heal Summons | Heal | All Summons | - | Heal 30-50 each |
| 5 | Summon Shield | Buff | Summons | 30s | +30 AR |
| 6 | Summon Phoenix | Summon | Ground | 20 min | Rebirth on death |
| 6 | Summon Hydra | Summon | Ground | 20 min | Multi-attack |
| 6 | Mass Empower | Buff | All Summons | 60s | +30% all stats |
| 6 | Sacrifice Summon | Damage | AoE | - | Explodes summon |
| 7 | Summon Titan | Summon | Ground | 30 min | Colossal |
| 7 | Summon Greater Dragon | Summon | Ground | 30 min | Ultimate dragon |
| 7 | Army of Beasts | Summon | Ground | 15 min | 3 creatures |
| 7 | Bind Beast | Control | Creature | 5 min | Mind control |
| 8 | **Summoner's Apocalypse** | 80-140 dmg | Single | - | **100% physical** |
| 8 | Avatar of Summoning | Transform | Self | 120s | Ultimate form |
| 8 | Swarm of Creatures | Summon | Ground | 10 min | 5 small creatures |
| 8 | Planar Convergence | Summon | Ground | 60s | All elementals |

#### Finale List (Current Implementation)
- Sharp Note (5)
- Mesmerise (5)
- Cacophony (10)
- Fortissimo (15)
- Soothing Chorus (15)
- Symphony of Destruction (20)

#### Testing Checklist
- [ ] Summon Rabbit creates rabbit that follows caster (10 min)
- [ ] Summoner's Apocalypse deals 80-140 physical damage
- [ ] Empower Summon buffs pet damage by +25%
- [ ] Multiple summons respect follower slot limits

---

### 9. SHAMANIC MAGIC (Shaman)
**Skill:** SpiritCalling (ID 67) | **Spell IDs:** 1256-1287 | **Spellbook:** ShamanSpellbook

#### Reagents
| Reagent | Command | Circles |
|---------|---------|---------|
| StormEssence | `[add StormEssence 50` | 1-3 |
| SpiritFeather | `[add SpiritFeather 50` | 1-4 |
| TotemCarving | `[add TotemCarving 30` | 3-5 |
| AncestorBone | `[add AncestorBone 30` | 4-6 |
| WindEssence | `[add WindEssence 20` | 5-7 |
| EarthSpirit | `[add EarthSpirit 20` | 6-8 |
| SkyTotem | `[add SkyTotem 10` | 7-8 |
| WorldSpiritShard | `[add WorldSpiritShard 10` | 8 |

#### Complete Spell Table
| Circle | Spell | Damage/Effect | Target | Radius | Duration |
|--------|-------|---------------|--------|--------|----------|
| 1 | Lightning Bolt | 8-15 | Single | - | Instant |
| 1 | Earth Shield | +15 AR | Self | - | 60s |
| 1 | Healing Stream | HoT 5/tick | Single | - | 15s |
| 1 | Spirit Strike | 6-12 | Single | - | Energy dmg |
| 2 | Chain Lightning | 10-18 | Chain (3) | - | Jumps 3 targets |
| 2 | Flame Shock | 8-14+DoT | Single | - | Fire 3/tick |
| 2 | Strength Totem | +10 STR | Ground | 8 | 60s |
| 2 | Mana Spring Totem | +5 Mana/tick | Ground | 8 | 60s |
| 3 | Summon Spirit Wolf | Summon | Ground | - | 10 min |
| 3 | Purification | Cure | Single | - | Remove poison |
| 3 | Ghost Wolf Form | Transform | Self | - | +40% speed |
| 3 | Fire Totem | 5-10/tick | Ground | 5 | 30s |
| 4 | **Flame Shock** | 20-25 | Single | - | **+Conjuration×0.5 energy** |
| 4 | Lava Burst | 22-32 | Single | - | Fire damage |
| 4 | Chain Heal | Heal 20-30 | Chain (4) | - | Jumps 4 allies |
| 4 | Thunderstorm Totem | 8-15/tick | Ground | 6 | 30s |
| 5 | Earth Elemental | Summon | Ground | - | 15 min tank |
| 5 | Elemental Fury | +30% dmg | Self | - | 30s buff |
| 5 | Spirit Link Totem | Share HP | Ground | 8 | 30s |
| 5 | Totem of Wrath | +15% dmg aura | Ground | 8 | 60s |
| 6 | Lightning Storm | 25-40 | Ground | 5 | DoT zone |
| 6 | Greater Earth Elemental | Summon | Ground | - | 20 min |
| 6 | Mega Chain Lightning | 20-35 | Chain (6) | - | Jumps 6 targets |
| 6 | Maelstrom | 30-45 | Ground | 6 | Vortex zone |
| 7 | **Ascendance** | -7 INT debuff | Single | - | **8-20s duration** |
| 7 | Ancestral Spirit | Resurrect | Corpse | - | Brings back ally |
| 7 | Four Totems | 4 totems | Ground | - | All totem buffs |
| 7 | Spirit of the Wild | Transform | Self | - | +30 all stats |
| 8 | Apocalyptic Chain Lightning | 50-80 | Chain (10) | - | Jumps 10 targets |
| 8 | Shaman Lord | Transform | Self | - | Ultimate form |
| 8 | Ancestor's Blessing | All buffs | Party | 8 | 120s |
| 8 | Totem Army | 8 totems | Ground | - | All types |

#### Finale List (Current Implementation)
- Sharp Note (5)
- Mesmerise (5)
- Cacophony (10)
- Fortissimo (15)
- Soothing Chorus (15)
- Symphony of Destruction (20)

#### Testing Checklist
- [ ] Chain Lightning jumps 3 targets correctly
- [ ] Flame Shock deals 20-25 + skill bonus energy damage
- [ ] Ascendance applies -7 INT debuff for 8-20 seconds
- [ ] Totems spawn at ground location and pulse effects
- [ ] Ghost Wolf Form increases movement speed

---

### 10. SONGWEAVING (Bard)
**Skill:** Songweaving (ID 63) | **Songbook:** Songbook of Weaving (spellbook) | **Command:** `[song <name>]` | **Hotbar:** Songweaving Hotbar | **Finales:** `[finale <name>]` / `[finales]` (Crescendo tracker)

**Full Bard test plan:** See `Vystia/gm/BARD_TESTING_GUIDE.md`.

#### Reagents (Songweaving Bag)
Songweaving songs do **not** consume reagents, but the school reagents are available for vendors/crafting/tests:
| Reagent | Command |
|---------|---------|
| SongPetal | `[add SongPetal 50` |
| EchoDust | `[add EchoDust 50` |
| VoiceCrystal | `[add VoiceCrystal 50` |
| MuseEssence | `[add MuseEssence 50` |
| HarmonyGem | `[add HarmonyGem 50` |
| EternalNote | `[add EternalNote 50` |
| GoldenString | `[add GoldenString 50` |
| DragonScale | `[add DragonScale 50` |

#### Song List (Current Implementation)
| Song | Effect |
|------|--------|
| Song of Provocation | Force one creature to attack another |
| Lullaby | Pacify a target for a short duration |
| Discordant Note | Weaken target defenses |
| Dirge of Weakness | Sonic harm over time |
| Song of Healing | Party heal over time |
| Song of Courage | Party stat boost |
| Song of Swiftness | Party speed boost |
| Song of Illumination | Night sight |
| Inspire Accuracy | Luck bonus |

#### Finale List (Current Implementation)
- Sharp Note (5)
- Mesmerise (5)
- Cacophony (10)
- Fortissimo (15)
- Soothing Chorus (15)
- Symphony of Destruction (20)

#### Testing Checklist
- [ ] Each song triggers its expected effect and message
- [ ] Songweaving hotbar displays cooldowns and blocks rapid re-cast
- [ ] Crescendo increases on successful songs
- [ ] Fortune applies luck bonus while active

---

### 11. ENCHANTING MAGIC (Enchanter)
**Skill:** Runeweaving (ID 68) | **Spell IDs:** 1320-1351 | **Spellbook:** EnchanterSpellbook

#### Reagents
| Reagent | Command | Circles |
|---------|---------|---------|
| ArcaneDust | `[add ArcaneDust 50` | 1-3 |
| EssenceOfMagic | `[add EssenceOfMagic 50` | 1-4 |
| RunicPowder | `[add RunicPowder 30` | 3-5 |
| GlyphStone | `[add GlyphStone 30` | 4-6 |
| EnchantedGem | `[add EnchantedGem 20` | 5-7 |
| MasterRune | `[add MasterRune 20` | 6-8 |
| AncientGlyph | `[add AncientGlyph 10` | 7-8 |
| TitanRune | `[add TitanRune 10` | 8 |

#### Complete Spell Table
| Circle | Spell | Effect | Target | Duration | Special |
|--------|-------|--------|--------|----------|---------|
| 1 | **Magic Weapon** | -1 INT debuff | Single | 8-20s | Basic curse |
| 1 | Detect Magic | Reveal | Area | Instant | Shows enchants |
| 1 | Sharpen | +5 damage | Weapon | 300s | Weapon buff |
| 1 | Arcane Shield | Absorb 20 | Self | 60s | Magic absorb |
| 2 | Enchant Arrows | +3 damage | Quiver | 100 shots | Ranged buff |
| 2 | Flaming Weapon | +Fire dmg | Weapon | 300s | Fire enchant |
| 2 | Fortify Armor | +10 AR | Armor | 300s | Armor buff |
| 2 | Disenchant | Remove | Item | - | Strip enchants |
| 3 | Lightning Weapon | +Energy | Weapon | 300s | Energy enchant |
| 3 | Elemental Barrier | +15 resists | Self | 120s | All resists |
| 3 | Rune of Protection | Ward | Ground | 300s | Damage block |
| 3 | Spell Reflection | Reflect 50% | Self | 30s | Return spells |
| 4 | Vampiric Weapon | Life steal | Weapon | 300s | 15% drain |
| 4 | Greater Disenchant | Mass remove | AoE | - | Strip area |
| 4 | Runic Empowerment | +20% spell | Self | 120s | Spell damage |
| 4 | Rune of Healing | HoT zone | Ground | 60s | 5/tick heal |
| 5 | Holy Weapon | +Holy dmg | Weapon | 300s | vs Undead |
| 5 | Aegis of Warding | +30 AR | Party | 60s | Group buff |
| 5 | Enchant Party Weapons | +10 dmg | Party | 300s | Group enchant |
| 5 | Rune of Power | +25% dmg | Ground | 30s | Damage zone |
| 6 | Legendary Weapon | +All dmg | Weapon | 600s | Ultimate enchant |
| 6 | Prismatic Barrier | +25 all | Self | 60s | All resists |
| 6 | Mass Disenchant | Strip all | AoE | 10 | Area dispel |
| 6 | Invulnerability | Immune | Self | 5s | Brief immunity |
| 7 | Godly Weapon | +50 damage | Weapon | 300s | Massive buff |
| 7 | Archmage's Blessing | +All spells | Self | 120s | Ultimate buff |
| 7 | Enchant Army | +15 dmg | All allies | 300s | Mass enchant |
| 7 | Rune of Resurrection | Auto-revive | Ground | 600s | Death ward |
| 8 | Artifact Empowerment | +100% | Item | 60s | Ultimate power |
| 8 | **Rune of Apocalypse** | 150-250 dmg | Single | - | **100% energy** |
| 8 | Invincible Armor | Immune | Self | 10s | Cannot die |
| 8 | Mass Enchant Weapons | +30 dmg | All allies | 300s | Ultimate mass |

#### Finale List (Current Implementation)
- Sharp Note (5)
- Mesmerise (5)
- Cacophony (10)
- Fortissimo (15)
- Soothing Chorus (15)
- Symphony of Destruction (20)

#### Testing Checklist
- [ ] Magic Weapon applies -1 INT debuff for 8-20s
- [ ] Rune of Apocalypse deals 150-250 energy damage
- [ ] Weapon enchants show visual effect (hue change)
- [ ] Enchants persist through death/logout
- [ ] Spell Reflection returns 50% of incoming spells

---

### 12. ILLUSION MAGIC (Illusionist)
**Skill:** IllusionMagic (ID 69) | **Spell IDs:** 1352-1383 | **Spellbook:** IllusionistSpellbook

#### Reagents
| Reagent | Command | Circles |
|---------|---------|---------|
| MirrorDust | `[add MirrorDust 50` | 1-3 |
| PhantomSilk | `[add PhantomSilk 50` | 1-4 |
| ShadowVeil | `[add ShadowVeil 30` | 3-5 |
| DreamEssence | `[add DreamEssence 30` | 4-6 |
| PhantomPetal | `[add PhantomPetal 20` | 5-7 |
| MindCrystal | `[add MindCrystal 20` | 6-8 |
| RealityShard | `[add RealityShard 10` | 7-8 |
| ChaosPrism | `[add ChaosPrism 10` | 8 |

#### Complete Spell Table
| Circle | Spell | Effect | Target | Duration | Special |
|--------|-------|--------|--------|----------|---------|
| 1 | **Minor Illusion** | -1 INT debuff | Single | 8-20s | Basic curse |
| 1 | Blur | +10% evade | Self | 60s | Dodge buff |
| 1 | Detect Thoughts | Reveal | Single | - | Shows stats |
| 1 | Phantom Bolt | 5-10 dmg | Single | - | Energy |
| 2 | Invisibility | Hidden | Self | 60s | Standard invis |
| 2 | Confuse | Random move | Single | 8s | Movement chaos |
| 2 | Mirror Image | Clone | Self | 30s | 1 decoy |
| 2 | Mind Spike | 10-18 dmg | Single | - | INT damage |
| 3 | Illusory Double | Clone | Self | 60s | 2 decoys |
| 3 | Charm Beast | Control | Creature | 60s | Animal charm |
| 3 | Greater Invisibility | Hidden + act | Self | 30s | Attack while invis |
| 3 | Mind Blast | 15-25 dmg | Single | - | Stun 1s |
| 4 | Illusory Terrain | Obscure | Ground | 120s | Hide area |
| 4 | Mass Confusion | Chaos | AoE | 5 | 10s confuse |
| 4 | Phase Shift | Teleport | Self | - | Short blink |
| 4 | Psychic Scream | 20-30 | AoE | 4 | Fear 3s |
| 5 | Dominate Mind | Control | Single | 30s | Mind control |
| 5 | True Invisibility | Perfect hide | Self | 120s | Cannot reveal |
| 5 | Mind Shatter | 30-45 dmg | Single | - | Mana burn |
| 5 | Phantasmal Killer | 35-50 dmg | Single | - | Fear + damage |
| 6 | Perfect Invisibility | Party hide | Party | 60s | Group invis |
| 6 | Mass Charm | Control | AoE | 5 | 20s charm |
| 6 | Psychic Storm | 25-40 | AoE | 6 | Energy storm |
| 6 | Mind Control | Full control | Single | 60s | Player control |
| 7 | Legion of Mirrors | 5 clones | Self | 60s | Major decoys |
| 7 | Reality Warp | Change | Area | 30s | Terrain shift |
| 7 | Illusory Army | 10 illusions | Ground | 60s | Fake army |
| 7 | Phantasmal Dragon | Summon | Ground | 120s | Fake dragon |
| 8 | **Apocalyptic Nightmare** | -8 INT debuff | Single | 8-20s | **Ultimate curse** |
| 8 | Perfect Illusion | Undetectable | Self | 300s | Perfect disguise |
| 8 | Illusory Apocalypse | Fake doom | AoE | 10 | Terror effect |
| 8 | Master of Puppets | Control all | AoE | 6 | Mass control |

#### Finale List (Current Implementation)
- Sharp Note (5)
- Mesmerise (5)
- Cacophony (10)
- Fortissimo (15)
- Soothing Chorus (15)
- Symphony of Destruction (20)

#### Testing Checklist
- [ ] Minor Illusion applies -1 INT debuff for 8-20s
- [ ] Apocalyptic Nightmare applies -8 INT debuff (ultimate)
- [ ] Invisibility hides character properly
- [ ] Mirror Images draw enemy attention
- [ ] Mind control allows targeting of controlled creature

---

### Quick Reagent Commands (All 12 Schools)

```bash
# ICE MAGIC
[add Frostbloom 50 && [add Winterleaf 50 && [add GlacierCrystal 50 && [add PermafrostEssence 30 && [add ArcticPearl 20 && [add FrozenSoul 20 && [add FrostEssence 20 && [add HeartOfWinter 10

# NATURE MAGIC
[add WildMoss 50 && [add Moonpetal 50 && [add LivingBark 30 && [add ElderwoodSeed 30 && [add PrimalVine 20 && [add AncientSap 20 && [add WorldTreeLeaf 10 && [add HeartOfTheForest 10

# HEX MAGIC
[add ToadsEye 50 && [add SwampLotus 50 && [add HagsHair 30 && [add CursedPearl 30 && [add WitchBlood 20 && [add DarkMoonflower 20 && [add SoulOfTheDamned 10 && [add WitchQueensTear 10

# ELEMENTAL MAGIC
[add EmberDust 50 && [add VolcanicAsh 50 && [add Moltenite 30 && [add PrimordialEmber 30 && [add DragonHeart 20 && [add PhoenixFeather 20 && [add LavaCore 10 && [add HeartOfTheVolcano 10

# DARK MAGIC
[add VoidCrystal 50 && [add VoidWeed 50 && [add ShadowPetal 30 && [add VoidSilk 30 && [add DemonHeart 20 && [add ShadowEssence 20 && [add VoidCore 10 && [add DemonPrinceTear 10

# DIVINATION
[add CrystalDust 50 && [add PropheticLeaf 50 && [add TimeSand 30 && [add SeeingStone 30 && [add FateCrystal 20 && [add EternalGlass 20 && [add ChronoShard 10 && [add OraclesTear 10

# NECROMANCY
[add GraveMoss 50 && [add BoneDust 50 && [add CorpseAsh 30 && [add SoulFragment 30 && [add DeathEssence 20 && [add LichDust 20 && [add PhylacteryChip 10 && [add DeathKnightSoul 10

# SUMMONING MAGIC
[add PlanarDust 50 && [add EtherShard 50 && [add SummoningCircle 30 && [add SpiritChain 30 && [add BeastEssence 20 && [add SummoningSalt 20 && [add PlanarGem 10 && [add DimensionalKey 10

# SHAMANIC MAGIC
[add StormEssence 50 && [add SpiritFeather 50 && [add TotemCarving 30 && [add AncestorBone 30 && [add WindEssence 20 && [add EarthSpirit 20 && [add SkyTotem 10 && [add WorldSpiritShard 10

# SONGWEAVING
[add SongPetal 50 && [add EchoDust 50 && [add VoiceCrystal 50 && [add MuseEssence 50 && [add HarmonyGem 50 && [add EternalNote 50 && [add GoldenString 50 && [add DragonScale 50

# ENCHANTING MAGIC
[add ArcaneDust 50 && [add EssenceOfMagic 50 && [add RunicPowder 30 && [add GlyphStone 30 && [add EnchantedGem 20 && [add MasterRune 20 && [add AncientGlyph 10 && [add TitanRune 10

# ILLUSION MAGIC
[add MirrorDust 50 && [add PhantomSilk 50 && [add ShadowVeil 30 && [add DreamEssence 30 && [add PhantomPetal 20 && [add MindCrystal 20 && [add RealityShard 10 && [add ChaosPrism 10

# ALL REAGENTS (spawn vendor)
[add VystiaReagentVendor
```

---

### Master Testing Checklist

**Per School (12 spellbooks + Songweaving):**
- [ ] Spellbook opens with 32 spells (Songweaving shows 15 song/finale icons)
- [ ] Circle 1-2 cast at 0 skill
- [ ] Circle 7-8 require 75+ skill
- [ ] Damage scales with school skill
- [ ] AoE hits correct radius
- [ ] DoT ticks correctly
- [ ] Debuffs apply and expire
- [ ] Summons follow commands
- [ ] Transforms grant bonuses
- [ ] Ultimate spells devastate

**Balanced Spells (Priority Testing):**
- [ ] Chaos Bolt: Random element each cast
- [ ] Shadow Nova: 4-tile caster AoE
- [ ] Chaos Storm: 5-tile ground AoE
- [ ] Apocalyptic Chaos: 2s stun + all stat debuff
- [ ] Hex of Agony: Anti-heal converts to damage
- [ ] Hex Storm: Random hex debuffs
- [ ] Prismatic Storm: Random element per target
- [ ] Hellfire Nova: 5-tile caster AoE fire

---

## Testing Consumables

### Potion Effect Verification

1. **Setup**
   ```
   [VA → Items tab → Consumables sub-page
   → Click "All Potions"
   ```

2. **Test Resistance Potions**
   | Potion | Test | Verify |
   |--------|------|--------|
   | Cold Resistance | Double-click | Message: "+25 Cold Resist for 10 min" |
   | Heat Resistance | Double-click | Message: "+25 Fire Resist for 10 min" |
   | Poison Resistance | Double-click | Message: "+25 Poison Resist for 10 min" |
   | Energy Resistance | Double-click | Message: "+25 Energy Resist for 10 min" |

3. **Test Enhancement Potions**
   | Potion | Test | Verify |
   |--------|------|--------|
   | Nature's Blessing | Double-click | HP regen buff icon/message |
   | Crystal Clarity | Double-click | +15 INT in stats |
   | Desert Swiftness | Double-click | +10 DEX in stats |
   | Ironclad Fortitude | Double-click | +10 STR in stats |

4. **Test Cooldowns**
   - Use same potion twice rapidly
   - Should see: "You must wait X seconds"

5. **Test Duration**
   - Use potion, wait for duration
   - Should see: "Effect has worn off"

---

## Testing Equipment

### Legendary Weapon Stats

| Weapon | Base Damage | Bonus | Special |
|--------|-------------|-------|---------|
| The Eternal Winter | 18-22 | +25 Cold | Freeze on hit |
| Phoenix Ascension | 14-18 | +20 Fire | Fire trail |
| The Cogmaster | 16-20 | +15 Energy | EMP effect |
| Prismatic Edge | 12-16 | +10 All | Multi-element |
| Voidcaller | 10-14 | +30 Shadow | Life steal |

### Legendary Armor Set Bonuses

**Glacial Aegis (Tank Plate):**
- Per piece: +3 STR, +10% Defend, +5% Reflect
- 6 pieces = +18 STR, +60% Defend, +30% Reflect

**Steamwork Exosuit (DPS Plate):**
- Per piece: +5 STR, +10% Weapon Speed
- 6 pieces = +30 STR, +60% Weapon Speed

**Shadow Shroud (Rogue Leather):**
- Per piece: +5 DEX, Stealth bonus
- 6 pieces = +30 DEX, major stealth bonus

### Equipment Test Procedure

1. Spawn equipment from Items tab
2. Equip all pieces
3. Open paperdoll/status
4. Verify stat bonuses applied
5. Test in combat vs spawned creature

---

## Testing Crafting Systems

Vystia features two custom crafting systems that use Vystia skills instead of standard UO skills.

### Testing Transmutation (Alchemist)

**Skill Used:** Transmutation (ID 81)
**Tool:** Alchemist's Kit

**Setup:**
```
[VA → Classes tab → Click "M" next to Alchemist
```
This gives you the Alchemist's Kit in your backpack.

**Test Procedure:**

1. **Open Crafting Menu**
   - Double-click Alchemist's Kit in backpack
   - Should see: "You open your alchemist's kit..."
   - Transmutation Menu gump should open

2. **Verify Categories**
   | Category | Contains |
   |----------|----------|
   | Healing Potions | Lesser/Normal/Greater Nature's Healing |
   | Cure Potions | Lesser/Normal/Greater Antivenom |
   | Refresh Potions | Stamina Tonic, Greater Stamina Tonic |
   | Enhancement Potions | Agility/Strength Elixirs, Night Vision |
   | Poison Potions | Lesser/Normal/Greater/Deadly Venom |
   | Explosive Potions | Lesser/Normal/Greater Explosive Flask |
   | Special Items | Smoke Bomb |

3. **Acquire Reagents**
   ```
   [VA → Resources tab → Give Nature Reagents
   [VA → Resources tab → Give Hex Reagents
   ```
   Or spawn from vendors: `[spawnvystia → Vendors → Druid Vendor / Witch Vendor`

4. **Craft Test Items**
   - Select "Healing Potions" category
   - Click "Lesser Nature's Healing"
   - If you have Wild Moss + Bottle, item is crafted
   - Check skill gain messages

5. **Verify Skill Requirement**
   ```
   [VA → Skills → Set Transmutation to 0
   ```
   - Try crafting Greater Nature's Healing (requires 55.0 skill)
   - Should fail: "You don't have enough skill"

### Testing Engineering (Artificer)

**Skill Used:** Engineering (ID 80)
**Tool:** Engineering Tool Kit

**Setup:**
```
[VA → Classes tab → Click "M" next to Artificer
```
Or spawn tool: `[add EngineeringToolKit`

**Test Procedure:**

1. **Open Crafting Menu**
   - Double-click Engineering Tool Kit
   - Should see: "You open your engineering tool kit..."
   - Engineering Menu gump should open

2. **Verify Categories**
   | Category | Contains |
   |----------|----------|
   | Basic Components | Clockwork Gear, Spring, Steam Core |
   | Gadgets | Smoke Grenade, Small/Medium/Large Explosive |
   | Tools | Engineering Tool Kit |
   | Clockwork Items | Construct Control Device |
   | Clockwork Equipment | Sword, Shield, Full Plate Set |

3. **Acquire Resources**
   ```
   [add ClockworkIngot 50
   [add ClockworkGear 20
   [add ClockworkSpring 10
   [add SteamCore 5
   ```

4. **Craft Test Items**
   - Select "Basic Components" category
   - Click "Clockwork Gear" (low skill requirement)
   - Verify gear appears in backpack
   - Try crafting Clockwork Sword (requires 65.0 skill)

5. **Verify Tool Uses**
   - Check Engineering Tool Kit properties
   - Shows "uses remaining: X"
   - Craft several items
   - Verify uses decrease

---

## Testing Class Special Items

Each class has a unique special item. Test these thoroughly.

### Pet & Summon Items

| Item | Class | Test Procedure |
|------|-------|----------------|
| **Beast Whistle** | Beastmaster | 1. Set Beast Bonding to various levels (30, 60, 90)<br>2. Double-click whistle<br>3. Verify pet summoned matches skill tier<br>4. Verify pet follows you<br>5. Wait 10 min, verify pet despawns |
| **Summoning Circle** | Summoner | 1. Summon a creature (e.g., Energy Vortex)<br>2. Double-click Summoning Circle<br>3. Verify creature gets +25% HP<br>4. Check cooldown (120s) |
| **Construct Control Device** | Artificer | 1. Double-click device<br>2. Verify Clockwork Scout spawns<br>3. Verify it follows and fights |

### Combat & Buff Items

| Item | Class | Test Procedure |
|------|-------|----------------|
| **Rage Totem** | Barbarian | 1. Double-click totem<br>2. Verify +20 STR buff<br>3. Check buff duration (30s)<br>4. Verify charges decrease |
| **Holy Symbol** | Cleric | 1. Take some damage first<br>2. Double-click symbol<br>3. Verify AoE heal effect<br>4. Check cooldown |
| **Templar's Cross** | Templar | 1. Double-click cross<br>2. Verify +10 all resists in status<br>3. Wait 30s, verify buff expires<br>4. Check 90s cooldown |
| **Knight's Banner** | Knight | 1. Form a party<br>2. Have party members within 8 tiles<br>3. Double-click banner<br>4. Verify all get +5 STR/DEX |
| **Enchanting Crystal** | Enchanter | 1. Equip a weapon<br>2. Double-click crystal<br>3. Verify weapon hue changes (1154)<br>4. Check weapon properties (+10 damage)<br>5. Wait 60s, verify buff expires |

### Utility Items

| Item | Class | Test Procedure |
|------|-------|----------------|
| **Crystal Orb** | Oracle | 1. Spawn some creatures nearby<br>2. Double-click orb<br>3. Verify HP/stats revealed for up to 5 creatures<br>4. Check 30s cooldown |
| **Monk's Prayer Beads** | Monk | 1. Reduce Mana/Stam to low<br>2. Double-click beads<br>3. Verify Mana/Stam restored<br>4. Amount scales with Martial Arts skill |
| **Spirit Totem** | Shaman | 1. Take some damage<br>2. Double-click totem<br>3. Verify instant heal<br>4. Verify +5 HP regen ticks (6 times over 30s)<br>5. Amount scales with Spirit Calling skill |
| **Magic Lute** | Bard | 1. Form a party<br>2. Have party members within 6 tiles<br>3. Double-click lute<br>4. Verify +10 All Skills for 45s<br>5. Check 120s cooldown |

**Songweaving Core Songs (Phase B):**
- Provocation (2-target)
- Peacemaking
- Discordance
- Requiem
- Mending (party HoT)
- Courage (party stats)
- Swiftness (party dex)
- Light (Night Sight)
- Fortune (Luck bonus)

| **Bounty Ledger** | Bounty Hunter | 1. Double-click ledger<br>2. Target a creature<br>3. Verify creature is marked (visual effect)<br>4. Check ledger properties shows marked target<br>5. Mark lasts 5 minutes |
| **Shapeshift Totem** | Druid | 1. Double-click totem<br>2. Verify form selection gump opens<br>3. Select Bear form<br>4. Verify body changes<br>5. Select Human to revert |

### Testing Commands

Spawn any class special item directly:
```
[add RageTotem
[add BeastWhistle
[add AlchemistKit
[add CrystalOrb
[add MonkBeads
[add TemplarCross
[add SummoningCircle
[add BountyLedger
[add KnightBanner
[add SpiritTotem
[add MagicLute
[add EnchantingCrystal
[add ConstructControlDevice
[add ShapeshiftTotem
[add HolySymbol
[add EngineeringToolKit
```

---

## Troubleshooting

### Common Issues

| Issue | Cause | Solution |
|-------|-------|----------|
| Spells not casting | Skill too low | `[VA → Skills → set to 100` |
| "Cannot use this" | Wrong class | Assign correct class first |
| No damage | Target immune | Try different damage type |
| Ability missing | Not registered | Check ability file compiled |
| Potion on cooldown | Used recently | Wait for cooldown |

### Debug Commands

```
[skills           - Open native skill gump (GM)
[props            - View object properties
[set str 100      - Set strength to 100
[set hits 500     - Set max HP to 500
[invul            - Toggle invulnerability
```

### Checking Ability Registration

```
// In server console, on startup:
[Vystia] Registering generated abilities...
[Vystia] Registered abilities from 11 magic schools + Songweaving (IDs 1384-1398)
[Vystia] Registering martial class abilities...
[Vystia] Registered abilities from 14 martial classes
```

If these messages don't appear, check:
- `GeneratedAbilityInitializer.cs` compiles
- `MartialAbilityInitializer.cs` compiles
- Both are called on server start

---

## Summary: Complete Test Workflow

### Full System Test (30 min)

1. **Classes (10 min)**
   - Test 2 magic classes (Beginner + Maxed)
   - Test 2 martial classes (Beginner + Maxed)
   - Verify gear/potions given

2. **Spells (10 min)**
   - Open spellbook
   - Cast 1 spell from each circle (8 spells)
   - Verify damage, effects, messages

3. **Abilities (5 min)**
   - Test 4 abilities (one per circle)
   - Verify damage, cooldowns, resources

4. **Consumables (3 min)**
   - Use 1 of each potion type
   - Verify effects and messages

5. **Equipment (2 min)**
   - Equip legendary set
   - Verify stat bonuses

---

## Appendix: All Button IDs

### Classes Tab
- Beginner buttons: 1100-1125
- Maxed buttons: 2500-2525

### Items Tab
- Weapons: 1300-1399
- Armor: 1400-1499
- Shields: 1500-1549
- Spellbooks: 1550-1599
- Class Items: 1600-1699
- Consumables: 1900-1922

### Skills Tab
- Set to 100: 2000-2099
- Set to 0: 2100-2199
- Class Skill to 100: 2400
- Reset All: 2401

---

---

## New Economy Systems (2026-01-02)

### Training Costs
Players must now pay gold to train skills from class trainers.

**Location:** `VystiaClassTrainers.cs`

| Skill Range | Cost | Tier Name |
|-------------|------|-----------|
| 0-20 | 500g | Novice |
| 20-40 | 2,000g | Apprentice |
| 40-60 | 5,000g | Journeyman |
| 60-80 | 10,000g | Expert |
| 80-100 | 25,000g | Master |

**Testing:**
```
[add BarbarianTrainer    - Spawn a trainer
```
- Click trainer to open skill training menu
- Verify costs shown for each tier
- Test with insufficient gold (should fail)
- Test with sufficient gold (should deduct and train)

### Repair Costs
Blacksmiths now charge for equipment repairs.

**Location:** `VystiaRepairService.cs`

| Material Tier | Cost per Durability | Min Fee |
|---------------|---------------------|---------|
| Standard | 2g | 50g |
| Regional Tier 1 | 35g | 50g |
| Regional Tier 2 | 50g | 50g |
| Legendary | 75g | 50g |
| Artifact | 100g | 50g |

**Testing:**
```
[add Blacksmith    - Spawn blacksmith vendor
```
- Right-click blacksmith → "Repair Equipment"
- Damage a weapon first, then test repair
- Verify cost calculation

### Service Fees
NPC services now cost gold.

**Location:** `VystiaServiceFees.cs`

| Service | Cost | Notes |
|---------|------|-------|
| Healer Resurrection | 50g | Free for young players |
| Moongate (Local) | 100g | Same region |
| Moongate (Cross-region) | 150g | Different region |
| Moongate (Cross-facet) | 250g | Different map |
| Pet Stabling (Daily) | 30g | Per day |
| Pet Stabling (Weekly) | 100g | Per week |

**Testing:**
```
[add Healer    - Spawn healer
[kill          - Kill yourself
```
- Walk near healer as ghost
- Should see resurrection gump with 50g cost
- Test with/without gold

---

## Religion System (2026-01-02)

### Overview
6 religions with piety tracking (0-1000).

**Location:** `VystiaReligionSystem.cs`

### Religions
| ID | Religion | Region | Passive Bonus |
|----|----------|--------|---------------|
| 1 | Frosthelm Faith | Frosthold | +5% Cold Resist |
| 2 | Surya's Sandscript | Emberlands | +5% Fire Resist |
| 3 | Lunara's Covenant | Verdantpeak | +5% Poison Resist, +5% Healing |
| 4 | Celestis Arcanum | Crystal Barrens | +5% Energy Resist, +2 Mana Regen |
| 5 | Oceana's Covenant | ShadowVoid | +3% Physical Resist, +5 Stealth |
| 6 | Cogsmith Creed | Ironclad | +5 Armor, +5 Crafting |

### Piety Tiers
| Tier | Piety Required | Unlocks |
|------|----------------|---------|
| None | 0-49 | - |
| Initiate | 50-199 | First passive bonus |
| Devoted | 200-499 | First devotion power |
| Faithful | 500-899 | Second devotion power |
| Exalted | 900-1000 | Third devotion power |

### Religion Commands
```
[Religion             - Show current religion status
[SetReligion <1-6>    - Convert to religion (GM)
[SetPiety <amount>    - Set exact piety (GM)
[AddPiety <amount>    - Add/remove piety (GM)
[Pray                 - Daily prayer (+10 piety)
[Tithe <gold>         - Donate gold (+1 piety per 100g)
```

### Testing Religion System
1. **Convert to Religion:**
   ```
   [SetReligion 1    - Join Frosthelm Faith
   [Religion         - Verify status shows religion
   ```

2. **Gain Piety:**
   ```
   [Pray             - Should gain +10 piety (once per day)
   [Tithe 1000       - Donate 1000g for +10 piety
   [AddPiety 100     - GM: Add 100 piety
   ```

3. **Check Tier Progression:**
   ```
   [SetPiety 50      - Should show "Initiate" tier
   [SetPiety 200     - Should show "Devoted" tier
   [SetPiety 900     - Should show "Exalted" tier
   ```

---

## Faction Reputation System (2026-01-02)

### Overview
7 factions with reputation tiers from Hostile (-3000) to Exalted (15000+).

**Location:** `VystiaFactionSystem.cs`

### Factions
| ID | Faction | Region | Enemy Faction |
|----|---------|--------|---------------|
| 1 | Frostguard | Frosthold | Flame Legion |
| 2 | Flame Legion | Emberlands | Frostguard |
| 3 | Greenward | Verdantpeak | Voidborn |
| 4 | Arcane Conclave | Crystal Barrens | Technoguild |
| 5 | Technoguild | Ironclad | Arcane Conclave |
| 6 | Sandwalkers | Desert | None |
| 7 | Voidborn | ShadowVoid | Greenward |

### Reputation Tiers
| Tier | Reputation | Vendor Discount |
|------|------------|-----------------|
| Hostile | -3000 to -1001 | 0% |
| Unfriendly | -1000 to -1 | 0% |
| Neutral | 0 to 2999 | 0% |
| Friendly | 3000 to 5999 | 5% |
| Honored | 6000 to 11999 | 8% |
| Revered | 12000 to 14999 | 12% |
| Exalted | 15000+ | 15% |

### Reputation Commands
```
[Factions                    - Show all faction standings
[Faction <1-7>               - Show detailed faction info
[SetReputation <1-7> <amount> - Set reputation (GM)
[AddReputation <1-7> <amount> - Add/remove reputation (GM)
[DonateFaction <1-7> <gold>   - Donate for reputation
```

### Testing Faction System
1. **Check All Standings:**
   ```
   [Factions    - Lists all 7 factions with current reputation
   ```

2. **View Faction Details:**
   ```
   [Faction 1   - Show Frostguard details and progress
   ```

3. **Gain Reputation:**
   ```
   [DonateFaction 1 5000    - Donate 5000g for +250 rep
   [AddReputation 1 1000    - GM: Add 1000 rep
   ```

4. **Test Enemy Faction Penalty:**
   ```
   [SetReputation 1 0       - Reset Frostguard to 0
   [SetReputation 2 0       - Reset Flame Legion to 0
   [AddReputation 1 1000    - Add 1000 to Frostguard
   [Factions                - Flame Legion should show -500
   ```
   (Gaining rep with a faction loses half as much with enemy faction)

---

## Class Balance Testing (2026-01-02)

### Differentiated Starting Stats
Classes now have unique starting stats instead of all 100/100/100.

**Testing:**
```
[VA → Classes tab → Click "B" (Beginner)
```
Then check stats with `[props` or status gump.

### Sample Starting Stats
| Class | STR | DEX | INT | Role |
|-------|-----|-----|-----|------|
| Barbarian | 45 | 25 | 10 | Melee DPS |
| Ice Mage | 15 | 20 | 45 | Caster |
| Knight | 42 | 23 | 15 | Tank |
| Monk | 30 | 35 | 15 | DEX Melee |
| Druid | 20 | 20 | 40 | Hybrid Healer |
| Ranger | 25 | 45 | 10 | Ranged DPS |
| Artificer | 27 | 28 | 25 | Balanced Crafter |

---

## Ability Cost Scaling (2026-01-02)

### Overview
Abilities now automatically scale mana/stamina costs, cooldowns, and cast times by circle.

**Location:** `AbilityCostScaling.cs`

### Spell Costs (Auto-scaled)
| Circle | Mana Cost | Cooldown | Cast Time |
|--------|-----------|----------|-----------|
| 1 | 5 | 0.5s | Instant |
| 2 | 9 | 1.5s | 0.5s |
| 3 | 14 | 3.0s | 1.0s |
| 4 | 20 | 5.0s | 1.5s |
| 5 | 28 | 8.0s | 2.0s |
| 6 | 39 | 12.5s | 2.25s |
| 7 | 52 | 20.0s | 2.5s |
| 8 | 68 | 45.0s | 3.0s |

### Martial Costs (Stamina by Tier)
| Tier | Stamina Cost | Cooldown |
|------|--------------|----------|
| 1 | 10 | 1.5s |
| 2 | 18 | 3.0s |
| 3 | 30 | 4.5s |
| 4 | 48 | 6.0s |

**Testing:**
- Cast spells from different circles
- Verify mana cost matches table
- Verify cooldown prevents re-cast
- Higher circles should have longer cast times

---

## Pet System (2026-01-02)

### Overview
4 classes have unique pet systems: Summoner, Necromancer, Beastmaster, Artificer.

**Location:** `VystiaPetSystem.cs`, `SummonerPets.cs`, `NecromancerPets.cs`, `ArtificerPets.cs`

### Pet Types by Class
| Class | Pets | Control Slots |
|-------|------|---------------|
| **Summoner** | Water/Fire/Earth/Air Elemental | 2-3 |
| **Necromancer** | Skeleton Warrior/Mage, Zombie, Wraith, Bone Knight | 1-3 |
| **Beastmaster** | Wolf, Bear, Boar, Snow Leopard, Ice Wyrm | 2-4 |
| **Artificer** | Clockwork Scout, Steam Golem, Gear Construct, Automaton Guard | 2-4 |

### Pet Commands
```
[SummonPet <type>       # Summon specific pet
[DismissPets            # Dismiss all active pets
[PetInfo                # Show pet stats
[PetHeal                # GM: Heal all pets
[SetPetLevel <1-5>      # GM: Set pet power level
```

### Testing Pet System
1. **Assign Class & Summon Pet:**
   ```
   [SetClassV2 Summoner
   [SummonPet WaterElemental
   ```
   - Verify pet spawns at player's feet
   - Verify pet follows player

2. **Test Pet Combat:**
   ```
   [ol                  # Spawn Ogre Lord
   ```
   - Order pet to attack
   - Verify pet deals damage
   - Verify pet takes damage

3. **Test Pet Limits:**
   - Try summoning more pets than control slots allow
   - Should get "You cannot control any more pets" message

4. **Dismiss and Respawn:**
   ```
   [DismissPets
   [SummonPet FireElemental
   ```
   - Verify old pet despawns
   - Verify new pet spawns correctly

---

## Housing System (2026-01-02)

### Overview
Houses have Vystia-specific purchase prices and weekly property taxes.

**Location:** `VystiaHousingCosts.cs`, `VystiaPropertyTax.cs`

### Housing Costs
| Size | Dimensions | Purchase Price | Weekly Tax |
|------|------------|----------------|------------|
| Small | 7×7 | 50,000g | 500g |
| Medium | 11×11 | 150,000g | 1,500g |
| Large | 15×15 | 400,000g | 4,000g |
| Keep | 18×18 | 1,000,000g | 10,000g |
| Castle | 31×31 | 3,000,000g | 30,000g |

### Housing Commands
```
[HouseCosts            # Display all house prices
[HouseTax              # Show current tax due
[PayTax                # Pay weekly property tax
[SetHouseOwner <name>  # GM: Transfer ownership
[DeleteHouse           # GM: Remove house
```

### Testing Housing System
1. **View Costs:**
   ```
   [HouseCosts
   ```
   - Verify table matches prices above

2. **Purchase House (if deed available):**
   - Check gold deducted matches price
   - Verify house placed

3. **Tax Collection:**
   ```
   [HouseTax           # Check tax amount due
   [PayTax             # Pay the tax
   ```
   - Tax due should be 1% of purchase price per week

---

## Zone Control System (2026-01-02)

### Overview
4 zone types with different PvP rules, death penalties, and loot rules.

**Location:** `VystiaZoneSystem.cs`

### Zone Types
| Zone | Color | PvP | Death Penalty | Loot Drop | XP/Gold Bonus |
|------|-------|-----|---------------|-----------|---------------|
| **Sanctuary** | Green | No | None | 0% | 0.75x |
| **Contested** | Yellow | Consent | 5% skills | 10% | 1.0x |
| **Lawless** | Red | Always | 10% skills | 25% | 1.25x |
| **Extreme** | Black | Always | 15% skills | 50% | 1.5x |

### Zone Commands
```
[ZoneInfo              # Show current zone rules
[SetZone <type>        # GM: Set region zone type
[ListZones             # List all defined zones
[ZoneStats             # Show zone kill/death stats
```

### Testing Zone System
1. **Check Current Zone:**
   ```
   [ZoneInfo
   ```
   - Should display zone type and rules
   - Colors: Green/Yellow/Red/Black

2. **PvP Rules:**
   - In Sanctuary: PvP attacks should be blocked
   - In Contested: Need consent (duel system)
   - In Lawless/Extreme: Full open PvP

3. **Death Penalty Test:**
   ```
   [SetZone Lawless
   [kill                # Kill self
   ```
   - Check skills dropped by 10%
   - Check loot dropped (25% of inventory)

4. **XP/Gold Bonus:**
   - Kill creatures in different zones
   - Compare gold drops (Extreme should be 1.5x)

---

*Use this guide to systematically test all Vystia systems before player launch!*

**Last Updated:** 2026-01-02

