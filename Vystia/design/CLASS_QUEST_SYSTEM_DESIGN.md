# Vystia Class Quest System Design

**Status:** Design Phase
**Date:** 2025-12-11

---

## Overview

The Class Quest System provides structured progression for all 26 Vystia classes. Each class has unique quests that:
- Teach class mechanics through gameplay
- Reward class-specific items and abilities
- Provide lore and world-building
- Gate certain abilities behind quest completion

**Quest Systems Note:** This design targets **Vystia Dynamic Quests** (VystiaQuestSystem/DynamicQuest).  
The **Mondain/BaseQuest** system is separate and is used for classic quest chains.

---

## Quest Structure

### Tier 1: Initiation (Level 1-30)
**Purpose:** Introduce class basics

| Quest | Description | Reward |
|-------|-------------|--------|
| The First Step | Speak to class trainer, learn basics | Class Focus Item |
| Gathering Materials | Collect regional reagents/materials | 25 reagents |
| Trial of [Skill] | Use class skill 10 times successfully | +5 skill |

### Tier 2: Apprentice (Level 30-60)
**Purpose:** Develop class identity

| Quest | Description | Reward |
|-------|-------------|--------|
| The [Class] Way | Complete class-specific challenge | Tier 2 ability |
| Regional Bond | Travel to class homeland | Regional buff |
| Master's Test | Pass trainer's skill check | Stance unlock |

### Tier 3: Journeyman (Level 60-90)
**Purpose:** Advanced class techniques

| Quest | Description | Reward |
|-------|-------------|--------|
| Ancient Knowledge | Find lost [Class] artifact | Legendary item |
| The [Class] Brotherhood | Join class guild/faction | Faction reputation |
| Champion's Trial | Defeat class-specific boss | Title |

### Tier 4: Master (Level 90+)
**Purpose:** Class mastery and prestige

| Quest | Description | Reward |
|-------|-------------|--------|
| The Final Trial | Complete class capstone quest | Ultimate ability |
| Legacy of the [Class] | Teach another player | Mentor achievement |
| Legendary Destiny | Epic multi-part quest chain | Legendary equipment set |

---

## Class-Specific Quest Examples

### Ice Mage (Frosthold)

**Tier 1: The Frozen Path**
1. "Whispers of Winter" - Visit the Ice Archon in Frosthold
2. "Gathering Frost" - Collect 25 Frostbloom, 25 Glacier Crystal
3. "Chill of Knowledge" - Cast 10 ice spells successfully

**Tier 2: Mastering the Cold**
4. "The Frozen Heart" - Find the Heart of Winter artifact
5. "Frosthold's Blessing" - Meditate at the Glacier Shrine
6. "Winter's Embrace" - Unlock the Glacial Stance

**Tier 3: Ice Archon's Trials**
7. "The Blizzard's Core" - Collect core from Ice Elemental boss
8. "Frost Brotherhood" - Gain Honored with Frosthold faction
9. "Champion of Winter" - Defeat the Frost Father

**Tier 4: Legend of Ice**
10. "The Eternal Winter" - Complete the legendary ice weapon quest
11. "Frost Mentor" - Help a new Ice Mage complete their initiation
12. "Frozen Destiny" - Obtain the Glacial Aegis armor set

### Barbarian (Frosthold)

**Tier 1: Path of Rage**
1. "The Blood Call" - Speak to the Battlemaster
2. "Trophy Hunt" - Collect 10 beast hides from Frosthold creatures
3. "First Blood" - Defeat 10 enemies while enraged

**Tier 2: Way of the Berserker**
4. "Ancestral Weapon" - Recover a legendary axe from ruins
5. "Northern Bond" - Prove strength at the Tribal Grounds
6. "Rage Mastery" - Unlock the Berserker Stance

**Tier 3: Champion's Path**
7. "The Great Hunt" - Slay the Glacial Bear Alpha
8. "Tribal Recognition" - Become Champion of the Northern Tribes
9. "Battle Legend" - Defeat the Frost Giant Chieftain

**Tier 4: Legendary Warrior**
10. "Fury Incarnate" - Complete the rage mastery ritual
11. "Warrior's Legacy" - Train an apprentice in combat
12. "Eternal Warrior" - Obtain the legendary warrior armor

### Druid (Verdantpeak)

**Tier 1: Nature's Call**
1. "Grove Awakening" - Find the Forestkeeper in Verdantpeak
2. "Living Harvest" - Collect 25 Moonpetal, 25 Primal Vine
3. "First Transformation" - Shapeshift 10 times

**Tier 2: Forest Guardian**
4. "The Ancient Grove" - Visit the Elder Treant
5. "Verdant Blessing" - Restore a corrupted forest area
6. "Form Mastery" - Unlock all basic shapeshifting forms

**Tier 3: Nature's Champion**
7. "Heart of the Forest" - Obtain Treant Heart from Ancient Treant boss
8. "Druid Circle" - Join the Verdantpeak Druid Circle
9. "Guardian Title" - Defeat the Corruption Elemental

**Tier 4: Archdruid**
10. "Nature's Avatar" - Complete the archdruid transformation ritual
11. "Grove Mentor" - Guide a new druid through their training
12. "Living Legend" - Obtain the legendary nature armor set

---

## Quest Mechanics

### Quest Triggers
- **NPC Dialog:** Talk to trainers or quest givers
- **Item Collection:** Gather reagents, materials, or artifacts
- **Combat:** Kill specific creatures or bosses
- **Skill Use:** Use class abilities a certain number of times
- **Location:** Visit specific areas or shrines
- **Faction:** Achieve reputation levels with class factions

### Quest Rewards

| Type | Examples |
|------|----------|
| Items | Class focus items, reagents, equipment |
| Skills | Skill points, ability unlocks |
| Stances | New stance unlocks |
| Titles | "Ice Mage Initiate", "Frost Champion", etc. |
| Reputation | Faction standing increases |
| Abilities | Special class abilities gated by quests |

### Quest Tracking
- Journal system to track active quests
- Progress indicators for multi-part objectives
- Map markers for quest locations
- Quest log with completed quests history

---

## Implementation Priority

### Phase 1: Core Framework
1. Quest journal system
2. Quest giver NPCs (trainers already created)
3. Basic quest tracking and completion
4. Reward delivery system

### Phase 2: Tier 1 Quests (All Classes)
1. Implement initiation quest for each class (25 quests)
2. Basic material gathering quests
3. Skill usage tracking quests

### Phase 3: Tier 2-4 Quests
1. Class-specific advanced quests
2. Boss encounters for class quests
3. Faction system integration
4. Title and achievement system

---

## Technical Requirements

### Files to Create
- `Scripts/Custom/VystiaClasses/Quests/VystiaQuestSystem.cs` - Core framework
- `Scripts/Custom/VystiaClasses/Quests/ClassQuests/` - Per-class quest definitions
- `Scripts/Custom/VystiaClasses/Quests/VystiaQuestGump.cs` - Quest journal UI
- `Scripts/Custom/VystiaClasses/Quests/VystiaQuestLog.cs` - Player quest tracking

### Data Storage
- PlayerMobile extension for quest progress
- Quest completion flags
- Faction reputation values

### Integration Points
- Trainers serve as quest givers
- Class selection triggers first quest
- Skill gains trigger quest progress
- Combat events trigger quest completion

---

## Regional Quest Hubs

| Region | Classes | Hub Location |
|--------|---------|--------------|
| Frosthold | Barbarian, Beastmaster, Ice Mage | Frost Citadel |
| Emberlands | Sorcerer | Flame Temple |
| Desert | Ranger, Illusionist | Oasis Haven |
| Shadowfen | Witch | Bog Sanctuary |
| ShadowVoid | Warlock, Necromancer | Void Sanctum |
| Verdantpeak | Druid, Alchemist | Elder Grove |
| Crystal Barrens | Oracle, Wizard | Crystal Spire |
| Ironclad | Artificer, Fighter, Monk, Templar | Forge Hall |
| Underwater | Summoner | Deepwater Temple |
| Multi-Regional | Knight, Cleric, Paladin, Bard, Enchanter, Shaman, Bounty Hunter | Capital City |

---

## Future Considerations

### Prestige Classes
- Tier 4 completion unlocks prestige specialization
- Advanced class variants with unique abilities
- Requires completion of legendary quest line

### Class Cooperation
- Multi-class group quests
- Class synergy bonuses for completing together
- Cross-class achievement system

### Seasonal Events
- Holiday-themed class quests
- Limited-time rewards
- Recurring annual quest chains

---

*Design Document: 2025-12-11*
*Status: Ready for Implementation*
