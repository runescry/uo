# Vystia System Test Guide

**Purpose:** Comprehensive in-game testing checklist for all Vystia systems
**Last Updated:** 2026-01-03
**Estimated Time:** 2-3 hours for full test

---

## Pre-Test Setup

### 1. Start Server
```bash
cd D:\UO\ServUO
dotnet run
```

### 2. Create Test Character
- Login with GM account
- Use `[admin` to access admin panel if needed

### 3. Prepare Test Area
```
[go Green Acres          # Or any open test area
[clearvystia 50          # Clear any existing Vystia creatures
```

---

## Test 1: Class System (25 Classes)

### 1.1 Class Assignment
```
[ListClassesV2                    # Verify all 25 classes listed
[SetClassV2 IceMage               # Assign Ice Mage class
[ClassInfoV2                      # Verify class info displays
```

**Verify:**
- [ ] All 25 classes appear in list
- [ ] Class assigns without error
- [ ] Stats change to class defaults (Ice Mage: 15/20/45)
- [ ] Primary skill assigned (Cryomancy for Ice Mage)

### 1.2 Class Removal
```
[RemoveClassV2                    # Remove class
[ClassInfoV2                      # Should show "No class"
```

### 1.3 Test All Classes (Quick)
Test at least one class per category:
- [ ] **Magic:** `[SetClassV2 IceMage` - verify INT-based stats
- [ ] **Martial:** `[SetClassV2 Fighter` - verify STR-based stats
- [ ] **Hybrid:** `[SetClassV2 Paladin` - verify mixed stats
- [ ] **Pet:** `[SetClassV2 Summoner` - verify pet slot info

---

## Test 2: Skill System (26 Custom Skills)

### 2.1 Skill Commands
```
[skillinfo                        # Show skill breakdown
[svs 50                           # Set all Vystia skills to 50
[skillinfo                        # Verify skills changed
[rvs                              # Reset all Vystia skills to 0
```

### 2.2 Skill Cap
```
[skillcap                         # Show current cap
[skillcap 84000                   # Set recommended cap
[skillcap                         # Verify change
```

**Verify:**
- [ ] 26 Vystia skills listed (IDs 58-83)
- [ ] Skills set/reset correctly
- [ ] Skill cap changes work

---

## Test 3: Magic System (12 Spellbooks + Songbook)

### 3.1 Spellbooks
```
[sb ice                           # Give Ice Mage spellbook
[sb druid                         # Give Druid spellbook
[sb necro                         # Give Necromancer spellbook
```

**Verify for each spellbook:**
- [ ] Spellbook appears in backpack
- [ ] Double-click opens spell list
- [ ] 32 spells visible (8 circles × 4 spells)
- [ ] Spell icons display correctly

### 3.2 Spell Casting
```
[SetClassV2 IceMage
[svs 100                          # Max skills for testing
[sb ice
```

**Test each circle (Ice Mage example):**
- [ ] Circle 1: Frost Bolt (target creature, verify damage)
- [ ] Circle 2: Ice Armor (self buff, verify buff icon)
- [ ] Circle 3: Frost Nova (AoE, verify multiple targets hit)
- [ ] Circle 4: Glacial Spike (verify higher damage)
- [ ] Circle 5-8: Test at least one spell each

### 3.3 Reagent Consumption
```
[VystiaReagents                   # Spawn reagent vendor
```
- Buy reagents for school
- Cast spell
- [ ] Verify reagents consumed from pack

### 3.4 Test All Schools (12 spellbooks + Songweaving)
| School | Command | Quick Test Spell |
|--------|---------|------------------|
| Ice Magic | `[sb ice` | Frost Bolt |
| Nature | `[sb druid` | Entangle |
| Hex | `[sb witch` | Curse |
| Elemental | `[sb sorcerer` | Fireball |
| Dark | `[sb warlock` | Shadow Bolt |
| Divination | `[sb oracle` | Foresight |
| Necromancy | `[sb necro` | Raise Dead |
| Summoning | `[sb summoner` | Summon Elemental |
| Shamanic | `[sb shaman` | Spirit Link |
| Songweaving | `[sb songweaving` | Provocation |
| Enchanting | `[sb enchanter` | Enchant Weapon |
| Illusion | `[sb illusionist` | Mirror Image |

---

## Test 4: Martial Abilities (224 Abilities)

### 4.1 Ability Testing
```
[SetClassV2 Fighter
[svs 100
[TestAbility 2000                 # Fighter: Power Strike
[TestAbility 2001                 # Fighter: Cleave
```

### 4.2 Test By Class
```
[ListAbilities Fighter            # List Fighter abilities
[TD                               # Spawn test dummy
```

**Test key abilities per class:**
- [ ] Fighter (2000-2015): Power Strike, Shield Wall
- [ ] Barbarian (2016-2031): Reckless Strike, Berserker Rage
- [ ] Paladin (2096-2111): Crusader Strike, Divine Storm
- [ ] Cleric (2192-2207): Heal, Divine Smite

---

## Test 5: Secondary Resources (15 Types)

### 5.1 Resource Display
```
[SetClassV2 Barbarian
[GetResources                     # Show all resources
```

### 5.2 Resource Generation
```
[TD                               # Spawn test dummy
# Attack dummy to generate Fury
[GetResources                     # Verify Fury increased
```

### 5.3 Resource Setting
```
[SetResource Fury 100             # Set Fury to max
[GetResources                     # Verify
[ResetResources                   # Reset all to 0
```

**Test by class type:**
- [ ] Barbarian → Fury
- [ ] Monk → Chi
- [ ] Warlock → Soul Shards
- [ ] Ice Mage → Chill Stacks

---

## Test 6: Buff/Debuff System

### 6.1 Apply Buffs
```
[ApplyBuff StrengthBoost 60 10    # +10 STR for 60 seconds
[ListBuffs                        # Verify buff active
```

### 6.2 Buff Types
- [ ] `[ApplyBuff DamageShield 30 20` - Absorb shield
- [ ] `[ApplyBuff SpeedBoost 30 10` - Movement speed
- [ ] `[ApplyBuff HealOverTime 30 5` - Regen effect

### 6.3 Remove Buffs
```
[RemoveBuff StrengthBoost         # Remove specific
[ClearBuffs                       # Remove all
[ListBuffs                        # Should be empty
```

---

## Test 7: Crowd Control System

### 7.1 Apply CC
```
[TD                               # Spawn dummy
[ApplyCC Stun 5                   # 5 second stun
[ListCC                           # Verify CC active
```

### 7.2 Diminishing Returns
```
[ApplyCC Stun 5                   # First: 5 seconds
[ApplyCC Stun 5                   # Second: 2.5 seconds (50%)
[ApplyCC Stun 5                   # Third: 1.25 seconds (25%)
[ApplyCC Stun 5                   # Fourth: Immune
[CheckDR Stun                     # Verify DR level
```

### 7.3 Reset DR
```
[ResetDR                          # Reset all DR
[CheckDR Stun                     # Should be level 0
```

---

## Test 8: Stance System (28 Stances)

### 8.1 Stance Commands
```
[SetClassV2 Fighter
[ListStances                      # List available stances
[SetStance Defensive              # Activate stance
[StanceInfo                       # Verify active
```

### 8.2 Stance Effects
- [ ] Defensive stance: Verify damage reduction
- [ ] Aggressive stance: Verify damage increase
- [ ] Verify stance switch cooldown

### 8.3 Clear Stance
```
[RemoveStance                     # Deactivate
[StanceInfo                       # Should show none
```

---

## Test 9: Economy System

### 9.1 Repair Service
```
[SpawnBlacksmith                  # Spawn repair NPC
[RepairCost                       # Display cost formula
```

**Test repair:**
- Equip damaged weapon
- Use blacksmith
- [ ] Verify gold deducted
- [ ] Verify item repaired

### 9.2 Service Fees
```
[SpawnHealer                      # Spawn healer NPC
[ServiceFees                      # Display all fees
```

**Test services:**
- [ ] Resurrection: Kill character, use healer (50g)
- [ ] Full Heal: Damage self, use healer (25g)
- [ ] Cure Poison: Poison self, use healer (15g)

### 9.3 Stablemaster
```
[SpawnStablemaster                # Spawn stable NPC
```
- [ ] Stable a pet (30g/week)
- [ ] Retrieve pet

---

## Test 10: Religion System

### 10.1 Religion Commands
```
[Religion                         # Show current religion/piety
[SetReligion Solarius             # Set to Solarius
[Religion                         # Verify change
```

### 10.2 Piety Gains
```
[Pray                             # +10 piety (once per day)
[Religion                         # Verify piety increased
[Tithe 1000                       # Donate 1000g (+10 piety)
[Religion                         # Verify piety increased
```

### 10.3 Piety Tiers
```
[SetPiety 50                      # Faithful tier
[Religion                         # Verify tier name
[SetPiety 200                     # Devoted tier
[Religion                         # Verify passive bonus active
[SetPiety 500                     # Blessed tier
[SetPiety 900                     # Exalted tier
```

### 10.4 All Religions
```
[SetReligion Solarius             # Sun god
[SetReligion Lunara               # Moon goddess
[SetReligion Terrath              # Earth god
[SetReligion Aquos                # Water god
[SetReligion Sylvanis             # Nature god
[SetReligion Mortis               # Death god
```

### 10.5 Shrines
```
[SpawnShrine Solarius             # Spawn Solarius shrine
```
- [ ] Interact with shrine
- [ ] Verify pilgrimage bonus (+75 piety)

---

## Test 11: Pet System

### 11.1 Summoner Pets
```
[SetClassV2 Summoner
[SummonPet WaterElemental         # Summon elemental
[PetInfo                          # Show pet info
[DismissPets                      # Dismiss all pets
```

### 11.2 All Pet Classes
```
# Summoner
[SetClassV2 Summoner
[PetList Summoner                 # List available pets
[SummonPet WaterElemental

# Necromancer
[SetClassV2 Necromancer
[PetList Necromancer
[SummonPet SkeletonWarrior

# Beastmaster
[SetClassV2 Beastmaster
[PetList Beastmaster
[SummonPet Wolf

# Artificer
[SetClassV2 Artificer
[PetList Artificer
[SummonPet ClockworkScout
```

**Verify:**
- [ ] Pet spawns at player location
- [ ] Pet follows player
- [ ] Pet attacks targets
- [ ] Pet limit enforced (can't summon more than allowed)

---

## Test 12: Faction System

### 12.1 Faction Display
```
[Factions                         # Show all faction standings
```

### 12.2 Reputation Changes
```
[SetReputation Frosthold 1000     # Set to Friendly
[Factions                         # Verify change
[AddReputation Frosthold 500      # Add more
[Factions                         # Verify total
```

### 12.3 Faction Tiers
```
[SetReputation Ironclad 0         # Neutral
[SetReputation Ironclad 3000      # Honored
[SetReputation Ironclad 8000      # Revered
[SetReputation Ironclad 15000     # Exalted
```

### 12.4 Faction Vendor
```
[SpawnFactionVendor Ironclad      # Spawn vendor
```
- [ ] Verify prices based on reputation
- [ ] Hostile: +25% prices
- [ ] Exalted: -15% prices

### 12.5 Donation
```
[DonateFaction Frosthold 1000     # Donate 1000g
[Factions                         # Verify +50 rep (1 per 20g)
```

---

## Test 13: Housing System

### 13.1 Housing Costs
```
[HouseCosts                       # Display all prices
```

**Verify prices:**
- [ ] Small (7×7): 50,000g / 500g weekly
- [ ] Medium (11×11): 150,000g / 1,500g weekly
- [ ] Large (15×15): 400,000g / 4,000g weekly
- [ ] Keep (18×18): 1,000,000g / 10,000g weekly
- [ ] Castle (31×31): 3,000,000g / 30,000g weekly

### 13.2 Tax System
```
[TaxInfo                          # Show player's tax status
[TaxStatus                        # Show server-wide stats
```

### 13.3 House Management (if house exists)
```
[HouseInfo                        # Target house for info
[TaxExempt                        # Toggle tax exemption
[PayTax                           # Pay outstanding taxes
```

---

## Test 14: Zone Control

### 14.1 Zone Info
```
[ZoneInfo                         # Show current zone type
[ZoneList                         # List all zones
```

### 14.2 Zone Types
```
[SetZone Sanctuary                # Safe zone
[ZoneInfo                         # Verify no PvP
[SetZone Contested                # Consent PvP
[SetZone Lawless                  # Open PvP
[SetZone Extreme                  # Full loot PvP
```

### 14.3 Zone Effects
**Sanctuary (Green):**
- [ ] PvP disabled
- [ ] No death penalty
- [ ] 0.75x XP/gold

**Contested (Yellow):**
- [ ] PvP requires consent
- [ ] 5% skill loss on death
- [ ] 1.0x XP/gold

**Lawless (Red):**
- [ ] Always-on PvP
- [ ] 10% skill loss
- [ ] 25% loot drop

**Extreme (Black):**
- [ ] Always-on PvP
- [ ] 15% skill loss
- [ ] 50% loot drop
- [ ] 1.5x XP/gold

---

## Test 15: Quest System

**Note:** This test covers **Vystia Dynamic Quests** (QuestNPC/Chronicler).  
The **Mondain/BaseQuest** system is separate and uses classic quest givers.

### 15.1 Quest Editor
```
[QE                               # Open quest editor
```

**Test quest creation:**
- [ ] Create new quest
- [ ] Add title and description
- [ ] Add objectives (kill, collect, talk)
- [ ] Add rewards (gold, items, skills)
- [ ] Save quest

### 15.2 Quest NPC Wizard
```
[aqn                              # Open quest NPC wizard
```

- [ ] Select quest
- [ ] Select waypoint
- [ ] Configure NPC
- [ ] Spawn NPC

### 15.3 Quest Flow
```
[FindQuestNPC                     # Find active quest NPCs
```

- [ ] Accept quest from NPC
- [ ] Complete objectives
- [ ] Return to NPC
- [ ] Receive rewards

### 15.4 Quest Cleanup
```
[ClearQuests                      # Clear all quests
```

---

## Test 16: Trainers & Vendors

### 16.1 Class Trainers
```
[sat                              # Spawn all 25 trainers
```

**Verify each trainer:**
- [ ] Correct name and title
- [ ] Regional hue appropriate
- [ ] Class selection dialogue works

### 16.2 Magic Vendors
```
[spawnvystia                      # Open spawn gump
# Navigate to Vendors page
# Spawn all vendors
```

**Verify vendors sell:**
- [ ] School-specific reagents
- [ ] 32 spell scrolls
- [ ] Empty spellbook

### 16.3 Resource Vendor
```
[VystiaResources                  # Spawn resource vendor
```
- [ ] Sells all ore types
- [ ] Sells all ingot types
- [ ] Prices correct

---

## Test 17: Creatures

### 17.1 Spawn Creatures
```
[spawnvystia                      # Open spawn gump
```

**Test one creature per region:**
- [ ] Frosthold: FrostGiant
- [ ] Emberlands: LavaHound
- [ ] Desert: SandElemental
- [ ] Shadowfen: BogBeast
- [ ] Verdantpeak: ForestGuardian
- [ ] Crystal Barrens: CrystalGolem
- [ ] Ironclad: SteamGolem
- [ ] Skyreach: StormRoc
- [ ] Underwater: DeepwaterShark
- [ ] ShadowVoid: VoidStalker

### 17.2 Boss Creatures
```
[spawnvystia                      # Navigate to Bosses
```
- [ ] FrostFather spawns
- [ ] Boss has correct stats
- [ ] Regional drops work

### 17.3 Cleanup
```
[clearvystia 50                   # Clear test creatures
```

---

## Test 18: Equipment

### 18.1 Regional Weapons
```
[add FrostBlade                   # Frosthold weapon
[add FlameTongue                  # Emberlands weapon
```
- [ ] Correct stats
- [ ] Regional hue
- [ ] Damage type appropriate

### 18.2 Legendary Weapons
```
[add TheEternalWinter             # Legendary ice weapon
[add PhoenixAscension             # Legendary fire weapon
```
- [ ] Artifact-level stats
- [ ] Special abilities

### 18.3 Armor Sets
```
[add FrostforgedPlateChest
[add EmberforgedPlateChest
```
- [ ] Set bonuses work
- [ ] Regional resist appropriate

---

## Test 19: Admin Gump

### 19.1 VystiaAdmin
```
[va                               # Open admin gump
```

**Verify all tabs work:**
- [ ] Class management
- [ ] Resource management
- [ ] Buff management
- [ ] Ability testing
- [ ] Equipment spawning

---

## Test Summary Checklist

### Core Systems
- [ ] Class assignment/removal
- [ ] 26 custom skills
- [ ] 352 spells across 11 schools + 15 Songweaving (IDs 1384-1398)
- [ ] 224 martial abilities
- [ ] 15 secondary resources
- [ ] Buff/debuff system
- [ ] 15 CC types with DR
- [ ] 28 stances

### Gameplay Systems
- [ ] Economy (repair, services, training)
- [ ] Religion (6 religions, piety)
- [ ] Pets (4 classes)
- [ ] Factions (7 factions)
- [ ] Housing (5 sizes, taxes)
- [ ] Zones (4 types)
- [ ] Quests (editor, NPCs)

### Content
- [ ] 25 class trainers
- [ ] 15 vendors
- [ ] 138 creatures
- [ ] 172 equipment items

---

## Known Issues Log

Record any issues found during testing:

| Issue | System | Severity | Status |
|-------|--------|----------|--------|
| | | | |
| | | | |
| | | | |

---

## Test Results

**Tester:** _______________
**Date:** _______________
**Build:** 0 errors, 0 warnings

### Summary
- Tests Passed: ___/19
- Critical Issues: ___
- Minor Issues: ___

### Notes
_________________________________
_________________________________
_________________________________

---

*This guide covers all major Vystia systems. Complete testing validates production readiness.*
