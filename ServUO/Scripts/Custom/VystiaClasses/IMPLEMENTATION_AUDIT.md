# Vystia Systems Implementation Audit

**Date:** 2025-01-XX  
**Status:** Comprehensive audit of missing implementations  
**Purpose:** Identify all systems, features, and mechanics from the design document that are not yet implemented

---

## Executive Summary

This audit compares the Vystia Complete Design Document against the current implementation to identify gaps, missing features, and incomplete systems.

**Overall Status:**
- ✅ **Core Systems:** Religion, Faction, Crafting foundations are implemented
- ⚠️ **Quest Integration:** Quest system exists but not fully integrated with religion/faction
- ❌ **Missing Features:** Several key features from design document are not implemented
- ❌ **Incomplete Systems:** Some systems are partially implemented

---

## 1. RELIGION SYSTEM

### ✅ Implemented

1. **Piety System**
   - ✅ Piety tracking (0-1000)
   - ✅ Piety tiers (None, Initiate, Adherent, Devoted, Faithful, Exalted)
   - ✅ Daily prayer (+10 piety)
   - ✅ Tithe system (+1 piety per 100g, max 30/day)

2. **Passive Bonuses**
   - ✅ All 6 religions have passive bonuses at Initiate (50) and Devoted (200)
   - ✅ Resistance bonuses (Cold, Fire, Poison, Energy, Physical)
   - ✅ Skill bonuses (Crafting, Stealth)
   - ✅ Mana regen bonus
   - ✅ Healing bonus
   - ✅ Damage bonuses (Cold, Fire)
   - ✅ Armor bonus

3. **Devotion Powers**
   - ✅ All 18 powers defined (3 per religion)
   - ✅ Power activation system (`[DevotionPower` command)
   - ✅ Cooldown tracking
   - ✅ Power effects implemented
   - ✅ Power recharge at shrine (Devoted tier)

4. **Blessed Items**
   - ✅ Item blessing system
   - ✅ Success rates by piety tier
   - ✅ Critical blessing chances
   - ✅ Religion-specific bonuses
   - ✅ Opposed religion restrictions
   - ✅ Blessing refresh at shrine (Adherent tier)

5. **Shrines**
   - ✅ Shrine items (`VystiaShrine`)
   - ✅ Shrine menu system
   - ✅ All shrine functions (Pray, Tithe, Refresh, Recharge, Bless, Resurrect)
   - ✅ Test items (Shrine Stones)

6. **Religion PvP**
   - ✅ Damage bonuses vs opposed religion
   - ✅ Damage reduction (Champion tier)
   - ✅ Healing effectiveness reduction

7. **Class-Religion Synergies**
   - ✅ All major synergies implemented
   - ✅ Resource bonuses (decay, regen, max)
   - ✅ Skill bonuses
   - ✅ Damage bonuses
   - ✅ Duration bonuses

### ❌ Missing / Incomplete

1. **Piety Gain Methods**
   - ❌ **Pilgrimage System** - Design doc specifies +75 piety for visiting shrines, but no pilgrimage tracking/cooldown system
   - ❌ **Quest Rewards** - No integration with quest system to award piety
   - ❌ **Blessed Item Crafting** - Design doc mentions +25 piety for crafting blessed items, but no crafting integration

2. **Piety Loss System**
   - ❌ **Piety Decay** - Design doc mentions piety loss for certain actions (killing aligned NPCs, desecrating shrines), but no decay system implemented
   - ❌ **Religion Change Penalties** - Design doc specifies piety retention on religion change, but no change system exists

3. **Religion Conversion**
   - ❌ **Conversion NPCs** - No high priests or conversion NPCs to convert players to religions
   - ❌ **Conversion UI** - No gump or system for players to convert
   - ❌ **Conversion Requirements** - Design doc may specify requirements, but no system exists

4. **Portable Shrines**
   - ❌ **Portable Shrine Items** - Design doc lists 6 portable shrine items (one per religion), none implemented
   - ❌ **Portable Shrine Crafting** - No recipes for crafting portable shrines

5. **Major Temples**
   - ❌ **Temple Locations** - Design doc specifies 6 major temples, no temple items/structures
   - ❌ **Temple Special Functions** - No special temple functions (legendary blessing, moonlight pool, etc.)

6. **Divine Blessings (Champion Tier)**
   - ❌ **Ancient Being Blessings** - Design doc specifies 6 divine blessings from ancient beings, none implemented
   - ❌ **Ancient Being NPCs** - No ancient being NPCs exist

7. **Devotion Power TODOs**
   - ⚠️ **Endurance of Winter** - "Cannot die" flag not implemented (TODO in code)
   - ⚠️ **Nature's Sanctuary** - Zone-based healing bonus not implemented (TODO in code)
   - ⚠️ **Celestial Alignment** - Spell count tracking and mana cost reduction not fully implemented (TODO in code)
   - ⚠️ **Abyssal Call** - Water elemental summon not implemented (TODO in code)

---

## 2. FACTION SYSTEM

### ✅ Implemented

1. **Reputation System**
   - ✅ Reputation tracking (-3000 to 21000)
   - ✅ Reputation tiers (Hostile, Unfriendly, Neutral, Friendly, Honored, Revered, Exalted)
   - ✅ Tier thresholds match design document

2. **Vendor Discounts**
   - ✅ Discount system implemented
   - ✅ Discount percentages match design (5%, 8%, 12%, 15%)
   - ✅ Faction vendor base class
   - ✅ Discount applies to buy/sell

3. **Reputation Rewards**
   - ✅ Quest reward constants defined
   - ✅ Boss kill rewards defined
   - ✅ Donation rewards defined
   - ✅ Enemy kill rewards defined

4. **Test Items**
   - ✅ Faction Stones for testing
   - ✅ Faction vendor stones

### ❌ Missing / Incomplete

1. **Reputation Gain Integration**
   - ❌ **Quest Integration** - `ReputationRewards.AwardQuestReputation()` exists but quest system doesn't call it
   - ❌ **Boss Kill Integration** - `ReputationRewards.AwardBossReputation()` exists but boss death handlers don't call it
   - ❌ **Donation System** - `ReputationRewards.AwardDonationReputation()` exists but no donation NPC/UI
   - ❌ **PvP Kill Integration** - `EnemyKillReward` constant exists but no PvP kill handler

2. **Faction Tokens**
   - ❌ **Token Currency** - Design doc mentions "Faction tokens for currency", but no token items exist
   - ❌ **Token Vendors** - No special vendors that accept tokens
   - ❌ **Token Rewards** - No system to award tokens

3. **Faction-Specific Benefits**
   - ❌ **Recipe Access** - Design doc mentions "Regional recipes" and "Legendary recipe access" at certain tiers, but no tier-gated recipes
   - ❌ **Title System** - Design doc mentions "Title" at Exalted tier, but no title system
   - ❌ **Unique Items** - Design doc mentions "unique items" at Exalted tier, but no unique item vendors

4. **Faction Vendors**
   - ⚠️ **Vendor Stock** - Faction vendors exist but may not have faction-specific stock
   - ⚠️ **Vendor Spawning** - Vendor stones exist but no permanent vendor placement system

5. **Faction Reputation Tiers (Design Mismatch)**
   - ⚠️ **Tier Names** - Design doc uses "Allied" (1,501-4,500) but implementation uses "Honored" (6,000+)
   - ⚠️ **Tier Thresholds** - Implementation thresholds don't match design doc exactly:
     - Design: Friendly (1-1,500), Allied (1,501-4,500), Honored (4,501-9,000), Revered (9,001-15,000), Exalted (15,000+)
     - Implementation: Friendly (3,000+), Honored (6,000+), Revered (12,000+), Exalted (15,000+)

---

## 3. CRAFTING SYSTEM

### ✅ Implemented

1. **Crafting Disciplines**
   - ✅ Engineering (Artificer) - `DefEngineering.cs`
   - ✅ Transmutation (Alchemist) - `DefTransmutation.cs`
   - ✅ Blacksmithy recipes - `VystiaCraftingRecipes.cs`

2. **Regional Materials**
   - ✅ All 8 ingot types exist
   - ✅ All 8 ore types exist
   - ✅ Ore smelting system

3. **Regional Recipes**
   - ✅ Frosthold weapons and armor
   - ✅ Emberlands weapons and armor
   - ✅ Crystal Barrens weapons
   - ✅ Ironclad weapons and armor
   - ✅ ShadowVoid weapons and armor
   - ✅ Regional shields

4. **Crafting Bonuses**
   - ✅ Religion crafting skill bonuses (Cogsmith Creed)
   - ✅ Religion exceptional chance bonus (Forge Blessing)
   - ✅ Engineering craft success bonus

5. **Test Items**
   - ✅ Crafting station stones
   - ✅ All crafting tools
   - ✅ All materials in test kit

### ❌ Missing / Incomplete

1. **Crafting Disciplines (Missing 7 of 10)**
   - ❌ **Runecrafting** (Enchanter) - No crafting system
   - ❌ **Inscription** (Oracle) - No crafting system
   - ❌ **Leathercraft** (Ranger) - No crafting system
   - ❌ **Woodshaping** (Druid) - No crafting system
   - ❌ **Clothcraft** (Bard) - No crafting system
   - ❌ **Necrocraft** (Necromancer) - No crafting system
   - ❌ **Jewelcraft** (Sorcerer) - No crafting system

2. **Transmutation Recipes**
   - ❌ **Secondary Resource Potions** - Design doc lists 14 potion recipes (Fury Draught, Chi Elixir, etc.), none implemented
   - ❌ **Potion Effects** - No potions that modify secondary resources

3. **Engineering Constructs**
   - ❌ **Construct Recipes** - Design doc lists 5 construct recipes (Clockwork Spider, Repair Drone, etc.), none implemented
   - ❌ **Construct Items** - No construct item classes exist
   - ❌ **Construct Summoning** - No system to summon constructs

4. **Legendary Weapons**
   - ❌ **Legendary Recipes** - Design doc lists 4 legendary weapons (The Eternal Winter, Phoenix Ascension, etc.), none implemented
   - ❌ **Boss Materials** - No boss drop materials exist (Heart of Winter, Lava Pearl, etc.)

5. **Crafted Item Quality**
   - ⚠️ **Quality Tiers** - Design doc specifies Quality (1.4×), Exceptional (1.8×), Masterwork (2.5×), Legendary (5.0×)
   - ⚠️ **Masterwork System** - No masterwork crafting system
   - ⚠️ **Legendary System** - No legendary crafting system with boss materials

6. **Regional Materials (Missing)**
   - ❌ **Wood Types** - Design doc lists Frostwood, Emberwood, Shadowwood, Livingwood, but no wood items exist
   - ❌ **Leather Types** - Design doc lists Polar, Volcanic, Swamp, Desert, Shadow leather, but no leather items exist
   - ❌ **Special Materials** - Design doc lists Eternal Ice, Everburning Coal, Cursed Pearl, Treant Heart, Prismatic Shard, Steam Core, Lich Dust, Deep Pearl, Storm Feather, Time Sand - most don't exist

7. **Crafting Stations**
   - ⚠️ **Special Stations** - Design doc mentions "Alchemist's Lab", "Steam Forge", "Runic Altar", "Scriptorium", "Living Workshop", "Ossuary", "Jeweler's Bench" - only basic stations exist

---

## 4. QUEST SYSTEM

### ✅ Implemented

1. **Quest Infrastructure**
   - ✅ Quest system exists (`VystiaQuestSystem.cs`)
   - ✅ Quest NPCs exist (`QuestNPC.cs`)
   - ✅ Quest waypoints exist
   - ✅ Dynamic quest generation (LLM-based)

2. **Quest Types**
   - ✅ Kill quests
   - ✅ Delivery quests
   - ✅ Escort quests
   - ✅ Dynamic quests

### ❌ Missing / Incomplete

1. **Quest-Religion Integration**
   - ❌ **Piety Rewards** - No quests award piety
   - ❌ **Religion-Specific Quests** - No quests tied to specific religions
   - ❌ **Pilgrimage Quests** - No quests that require visiting shrines

2. **Quest-Faction Integration**
   - ❌ **Reputation Rewards** - Quest system doesn't call `ReputationRewards.AwardQuestReputation()`
   - ❌ **Faction-Specific Quests** - No quests tied to specific factions
   - ❌ **Faction Quest Givers** - No faction-aligned quest NPCs

3. **Quest Tiers**
   - ❌ **Tier System** - Design doc specifies 4 quest tiers (Initiation, Apprentice, Journeyman, Master) with gold rewards, but no tier system
   - ❌ **Daily Quests** - Design doc mentions "Daily Quests (200-800g)", but no daily quest system
   - ❌ **Faction Quests** - Design doc mentions "Faction Quests (300-2,500g, +Reputation)", but no faction quest system

4. **Quest Rewards**
   - ⚠️ **Gold Rewards** - Quest system may award gold, but amounts may not match design doc tiers
   - ❌ **Material Rewards** - Design doc doesn't specify material rewards, but quests could award crafting materials

---

## 5. ECONOMY SYSTEM

### ✅ Implemented

1. **Gold Sources**
   - ✅ Creature loot (via standard ServUO)
   - ✅ Quest rewards (via quest system)

2. **Gold Sinks**
   - ✅ Repair costs (via `VystiaRepairService`)
   - ✅ Service costs (via standard ServUO)

### ❌ Missing / Incomplete

1. **Service Costs**
   - ❌ **Service NPCs** - Design doc lists specific costs (Cure Poison 25g, Cure Disease 50g, etc.), but no custom service NPCs
   - ❌ **Moongate Costs** - Design doc specifies regional (100g) and cross-continental (250g) moongate costs, but no custom moongate system
   - ❌ **Ship Passage** - Design doc specifies 200-500g ship passage, but no custom ship system

2. **Training Costs**
   - ❌ **Vystia Skill Training** - Design doc specifies training costs for Vystia skills (0-30: 500g, 30-50: 2,000g, etc.), but no training system
   - ❌ **Trainer NPCs** - No class trainers that charge for training

3. **Housing Costs**
   - ❌ **Custom Housing** - Design doc specifies housing costs (Small 50,000g, etc.), but no custom housing system

4. **Religious Gold Sinks**
   - ✅ Tithe system exists
   - ✅ Blessing costs exist (5% item value)
   - ❌ **Portable Shrine Costs** - Design doc specifies ~1,500g materials, but no portable shrines exist

5. **Economy Balance**
   - ⚠️ **Gold Flow Tracking** - No system to track gold flow per hour
   - ⚠️ **Inflation Controls** - No explicit inflation control systems

---

## 6. CREATURE AND BOSS SYSTEM

### ✅ Implemented

1. **Creature Infrastructure**
   - ✅ Pet system exists
   - ✅ Creature scaling exists

### ❌ Missing / Incomplete

1. **Regional Bosses (10 Total)**
   - ❌ **All 10 Bosses** - Design doc lists 10 regional bosses, none implemented:
     - Frost Father (Frosthold)
     - Volcano Wyrm (Emberlands)
     - Sphinx of Surya (Desert)
     - Coven Matriarch (Shadowfen)
     - Ancient Treant (Verdantpeak)
     - Crystal Drake Alpha (Crystal Barrens)
     - Forge Master (Ironclad)
     - Griffin Lord (Skyreach)
     - Ancient Kraken (Underwater)
     - Timeworn Lich (ShadowVoid)

2. **Boss Mechanics**
   - ❌ **Boss Phases** - Design doc specifies 3-5 phases per boss, no phase system
   - ❌ **Boss Enrage** - Design doc specifies enrage timers (12-20 min), no enrage system
   - ❌ **Boss Loot** - Design doc specifies gold and material rewards, no boss loot system

3. **Ancient Beings (12 Total)**
   - ❌ **All 12 Ancient Beings** - Design doc lists 12 ancient beings, none implemented:
     - Frosthelm the Eternal Winter
     - Emberflame the Ashen Tyrant
     - Verdantheart the Forest Guardian
     - Crystalwing the Prismatic Oracle
     - Abyssus the Depth King
     - Elder Oakbark
     - Sphynx of Emberlands
     - Frost Father's Avatar
     - Great Machinist's Construct
     - Lunara's Dryad Herald
     - The Crystal Sphinx
     - Ironbark the War-Ancient

4. **Ancient Being Functions**
   - ❌ **Quest Givers** - 5 ancient beings are quest givers, but no quest integration
   - ❌ **Recipe Teachers** - 3 ancient beings teach recipes, but no recipe teaching system
   - ❌ **Divine Blessings** - 3 ancient beings grant divine blessings (Champion tier), but no blessing system

5. **Divine Blessings**
   - ❌ **All 6 Blessings** - Design doc lists 6 divine blessings, none implemented:
     - Machinist's Perfection (+15% exceptional craft, 24hr)
     - Lunar Radiance (+15% healing, 24hr)
     - Solar Clarity (immune to illusions/blind, 24hr)
     - Abyssal Favor (water breathing, +35% swim, 24hr)
     - Starlight (+15% spell power, 24hr)
     - Frost Father's Endurance (+50 HP, immune to slow, 24hr)

6. **Creature Distribution**
   - ❌ **138 Creatures** - Design doc specifies 138 creatures across regions, but no creature implementation audit exists
   - ❌ **Regional Creatures** - No verification that regional creatures exist

---

## 7. ZONE CONTROL SYSTEM

### ❌ Missing / Incomplete

1. **Zone Types**
   - ❌ **Sanctuary Zones** - Design doc specifies green zones (no PvP), but no zone system
   - ❌ **Contested Zones** - Design doc specifies yellow zones (flagging-based PvP), but no zone system
   - ❌ **Lawless Zones** - Design doc specifies red zones (open PvP), but no zone system
   - ❌ **Extreme Zones** - Design doc specifies black zones (open PvP, skill loss), but no zone system

2. **Zone Features**
   - ❌ **Zone Colors** - No visual zone indication system
   - ❌ **Zone Rules** - No system to enforce zone-specific rules (PvP, loot, death penalty)
   - ❌ **Zone Services** - No system to restrict services by zone type

---

## 8. CAMPING/HIKING SYSTEM

### ❌ Missing / Incomplete

1. **Camping System**
   - ❌ **Campfire Creation** - Design doc specifies camping skill 0-29 for light campfire, but no camping system
   - ❌ **Safe Camp** - Design doc specifies skill 90-99 for safe camp (no spawns 8 tiles), but no system
   - ❌ **Outpost Creation** - Design doc specifies skill 100 for outpost creation, but no system

2. **Hiking System**
   - ❌ **Hike Ability** - Design doc specifies hiking to discovered locations, but no hiking system
   - ❌ **Hike Restrictions** - No system to enforce hiking restrictions (damage interrupt, zone restrictions, etc.)
   - ❌ **Location Discovery** - No system to track discovered locations for hiking

3. **Wilderness Bonuses**
   - ❌ **Wilderness Damage** - Design doc specifies +5% to +15% wilderness damage bonuses, but no system

---

## 9. SKILL SYSTEMS

### ✅ Implemented

1. **Custom Skills**
   - ✅ All 26 custom skills defined (IDs 58-83)
   - ✅ Skills integrated into class system

2. **Skill Bonuses**
   - ✅ Religion skill bonuses
   - ✅ Class-religion synergy skill bonuses

### ❌ Missing / Incomplete

1. **Skill Training**
   - ❌ **Training Costs** - Design doc specifies training costs for Vystia skills, but no training system
   - ❌ **Trainer NPCs** - No class trainers that provide training
   - ❌ **Training Restrictions** - Design doc specifies tier requirements (Regional trainer, Faction trainer, Master trainer), but no system

2. **Skill Gain Modifiers**
   - ⚠️ **Power Hour** - Design doc specifies ×2.0 gain for first hour daily, but no tracking system
   - ⚠️ **Training Dummy** - Design doc specifies ×0.5 gain capped at 50, but no training dummy system

---

## 10. ITEM SYSTEMS

### ✅ Implemented

1. **Regional Equipment**
   - ✅ Regional weapons exist
   - ✅ Regional armor exists
   - ✅ Regional shields exist

2. **Blessed Items**
   - ✅ Blessed item system exists
   - ✅ Blessed item effects implemented

### ❌ Missing / Incomplete

1. **Class Ability Items**
   - ❌ **RageTotem** - Referenced in `MISSING_ITEMS_TODO.md`, not implemented
   - ❌ **ConstructControlDevice** - Referenced in `MISSING_ITEMS_TODO.md`, not implemented
   - ❌ **ShapeshiftTotem** - Referenced in `MISSING_ITEMS_TODO.md`, not implemented
   - ❌ **HolySymbol** - Referenced in `MISSING_ITEMS_TODO.md`, not implemented
   - ❌ **ArtificerBlueprints** - Referenced in `MISSING_ITEMS_TODO.md`, not implemented

2. **Spellbooks**
   - ⚠️ **Spellbook Items** - Design doc mentions spellbooks, but need to verify all 12 exist

3. **Reagents**
   - ⚠️ **Reagent Items** - Design doc specifies 104 reagents (96 school-specific + 8 universal), but need to verify all exist

4. **Portable Shrines**
   - ❌ **All 6 Portable Shrines** - Design doc lists 6 portable shrine items, none implemented

5. **Legendary Items**
   - ❌ **Legendary Weapons** - Design doc lists 4 legendary weapons, none implemented
   - ❌ **Legendary Materials** - No boss drop materials for legendary crafting

---

## 11. NPC SYSTEMS

### ❌ Missing / Incomplete

1. **High Priests**
   - ❌ **6 High Priests** - Design doc mentions high priests for each religion, but no NPCs exist
   - ❌ **Conversion System** - No NPCs to convert players to religions

2. **Class Trainers**
   - ❌ **25 Class Trainers** - Design doc mentions class trainers, but no trainer NPCs exist
   - ❌ **Training System** - No training system for Vystia skills

3. **Faction Vendors**
   - ⚠️ **Vendor Stock** - Faction vendors exist but may not have faction-specific stock
   - ⚠️ **Vendor Placement** - No system to place permanent faction vendors

4. **Quest NPCs**
   - ✅ Quest NPCs exist
   - ⚠️ **Faction Quest NPCs** - No faction-aligned quest NPCs

5. **Service NPCs**
   - ❌ **Custom Service NPCs** - Design doc specifies service costs, but no custom service NPCs

---

## 12. REGIONAL SYSTEMS

### ❌ Missing / Incomplete

1. **Regional Materials**
   - ❌ **Wood Types** - Frostwood, Emberwood, Shadowwood, Livingwood don't exist
   - ❌ **Leather Types** - Polar, Volcanic, Swamp, Desert, Shadow leather don't exist
   - ❌ **Special Materials** - Most special materials don't exist (Eternal Ice, Everburning Coal, etc.)

2. **Regional Structures**
   - ❌ **Major Temples** - 6 major temples don't exist
   - ❌ **Regional Forges** - Design doc mentions "Regional Forges" but no special forge items

3. **Regional Services**
   - ❌ **Regional Moongates** - No custom moongate system with costs
   - ❌ **Regional Ships** - No custom ship system

---

## 13. PVP SYSTEMS

### ✅ Implemented

1. **Religion PvP**
   - ✅ Damage bonuses vs opposed religion
   - ✅ Damage reduction
   - ✅ Healing effectiveness reduction

### ❌ Missing / Incomplete

1. **Faction PvP**
   - ❌ **Enemy Kill Rewards** - `EnemyKillReward` constant exists but no PvP kill handler
   - ❌ **Faction PvP Flags** - No system to flag players for faction PvP

2. **Zone-Based PvP**
   - ❌ **Zone PvP Rules** - No zone system to enforce PvP rules

---

## 14. SUMMARY BY PRIORITY

### 🔴 Critical Missing (Core Gameplay)

1. **Quest Integration**
   - Quest → Piety rewards
   - Quest → Reputation rewards
   - Faction-specific quests
   - Religion-specific quests

2. **Boss System**
   - All 10 regional bosses
   - Boss loot system
   - Boss → Reputation rewards

3. **Ancient Beings**
   - All 12 ancient beings
   - Divine blessings system
   - Recipe teaching system

4. **Crafting Disciplines**
   - 7 missing crafting systems (Runecrafting, Inscription, Leathercraft, Woodshaping, Clothcraft, Necrocraft, Jewelcraft)
   - Transmutation potion recipes
   - Engineering construct recipes
   - Legendary weapon recipes

### 🟡 Important Missing (Enhanced Gameplay)

5. **Piety Gain Methods**
   - Pilgrimage system
   - Quest piety rewards
   - Blessed item crafting piety

6. **Faction Integration**
   - Donation system
   - PvP kill rewards
   - Faction tokens
   - Tier-gated recipes

7. **Religion Conversion**
   - High priest NPCs
   - Conversion system
   - Conversion UI

8. **Portable Shrines**
   - All 6 portable shrine items
   - Portable shrine crafting

### 🟢 Nice to Have (Polish)

9. **Zone Control System**
   - Zone types and rules
   - Zone visual indicators

10. **Camping/Hiking System**
    - Camping mechanics
    - Hiking system
    - Location discovery

11. **Training System**
    - Vystia skill training
    - Trainer NPCs
    - Training costs

12. **Service NPCs**
    - Custom service NPCs
    - Service cost system

---

## 15. IMPLEMENTATION NOTES

### Design Document Mismatches

1. **Faction Tier Names**
   - Design: Friendly, Allied, Honored, Revered, Exalted
   - Implementation: Friendly, Honored, Revered, Exalted (missing "Allied")
   - **Action:** Either add "Allied" tier or update design doc

2. **Faction Tier Thresholds**
   - Design: Friendly (1-1,500), Allied (1,501-4,500), Honored (4,501-9,000), Revered (9,001-15,000), Exalted (15,000+)
   - Implementation: Friendly (3,000+), Honored (6,000+), Revered (12,000+), Exalted (15,000+)
   - **Action:** Align thresholds with design doc or update design doc

3. **Religion Names**
   - Design: Frosthelm Faith, Cogsmith Creed, Lunara's Covenant, Surya's Sandscript, Oceana's Covenant, Celestis Arcanum
   - Implementation: Frosthelm Faith, Cogsmith Creed, Lunara's Covenant, Surya's Sandscript, Oceana's Covenant, Celestis Arcanum
   - **Action:** Already documented, implementation uses different names

### Code TODOs

1. **VystiaDevotionPowers.cs**
   - Endurance of Winter: "Cannot die" flag
   - Nature's Sanctuary: Zone-based healing bonus
   - Celestial Alignment: Spell count tracking
   - Abyssal Call: Water elemental summon

2. **VystiaSkillIntegration.cs**
   - Resource maximum bonuses (methods exist but not hooked)
   - Resource generation bonuses (methods exist but not hooked)
   - Cost reduction bonuses (methods exist but not hooked)

---

## 16. TESTING GAPS

### Missing Test Items

1. **Boss Spawners** - No test items to spawn bosses
2. **Ancient Being Spawners** - No test items to spawn ancient beings
3. **Quest Test Items** - No test items to create/manage quests
4. **Training Test Items** - No test items to test training system
5. **Zone Test Items** - No test items to create zones

---

## 17. DOCUMENTATION GAPS

### Missing Documentation

1. **Boss System Documentation** - No documentation for boss mechanics
2. **Ancient Being Documentation** - No documentation for ancient beings
3. **Quest System Documentation** - Limited documentation for quest system
4. **Crafting System Documentation** - Missing documentation for 7 crafting disciplines
5. **Training System Documentation** - No documentation for training system

---

## 18. RECOMMENDATIONS

### Immediate Priorities

1. **Quest Integration** - Connect quest system to religion/faction rewards
2. **Boss System** - Implement at least 2-3 bosses as proof of concept
3. **Crafting Disciplines** - Implement 2-3 missing crafting systems
4. **Piety Gain Methods** - Add pilgrimage and quest piety rewards

### Medium-Term Priorities

5. **Ancient Beings** - Implement all 12 ancient beings
6. **Faction Integration** - Complete faction reward integration
7. **Religion Conversion** - Add conversion system and NPCs
8. **Portable Shrines** - Implement all 6 portable shrine items

### Long-Term Priorities

9. **Zone Control System** - Implement zone types and rules
10. **Camping/Hiking System** - Implement camping and hiking mechanics
11. **Training System** - Implement Vystia skill training
12. **Service NPCs** - Implement custom service NPCs

---

**Last Updated:** 2025-01-XX  
**Next Review:** After implementing critical missing features

