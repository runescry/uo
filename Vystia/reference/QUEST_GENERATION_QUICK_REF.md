# Quest Generation Quick Reference

**Command:** `[GenLLMQuest [poiId]`

**Note:** This is for **Vystia Dynamic Quests** (QuestNPC/Chronicler).  
The **Mondain/BaseQuest** system uses classic quest givers (MondainQuester/LLMQuester).

---

## POI Quick List (20 Total)

### Starter Areas
| POI ID | Region | Location | Map |
|--------|--------|----------|-----|
| `NEWHAVEN_TRAINING` | New Haven | Training Grounds | Trammel |
| `IRONCLAD_BRITAIN_CENTER` | Ironclad | Britain Commons | Felucca |
| `IRONCLAD_BRITAIN_BANK` | Ironclad | Britain Bank | Felucca |

### Regional Hubs
| POI ID | Region | Location | Map |
|--------|--------|----------|-----|
| `FROSTHOLD_MINOC` | Frosthold | Minoc | Felucca |
| `VERDANTPEAK_YEW` | Verdantpeak | Yew Forest | Felucca |
| `CRYSTAL_BARRENS_MOONGLOW` | Crystal Barrens | Moonglow | Felucca |
| `LEAGUE_SANDS_TRINSIC` | Desert | Trinsic | Felucca |
| `SKYREACH_VESPER` | Skyreach | Vesper | Felucca |
| `MARITIME_JHELOM` | Maritime | Jhelom | Felucca |

### Special Locations
| POI ID | Region | Location | Map |
|--------|--------|----------|-----|
| `IRONCLAD_FORGE` | Ironclad | The Great Forge | Felucca |
| `VERDANTPEAK_GROVE` | Verdantpeak | Sacred Grove | Felucca |
| `CRYSTAL_BARRENS_FIELDS` | Crystal Barrens | Crystal Fields | Felucca |
| `FROSTHOLD_GLACIER` | Frosthold | Eternal Glacier | Felucca |
| `DESERT_RUINS` | Desert | Ancient Ruins | Felucca |
| `SKYREACH_FLOATING_ISLE` | Skyreach | Floating Isle | Felucca |
| `MARITIME_HARBOR` | Maritime | Grand Harbor | Felucca |

### Dangerous Areas
| POI ID | Region | Location | Map |
|--------|--------|----------|-----|
| `EMBERLANDS_VOLCANO` | Emberlands | Volcanic Region | Felucca |
| `SHADOWFEN_SWAMP` | Shadowfen | Swamp Depths | Felucca |
| `UNDERWATER_DEPTHS` | Underwater | Deepwater Trench | Felucca |
| `SHADOWVOID_DUNGEON` | ShadowVoid | Planar Rift | Felucca |

---

## NPC Template Quick List (42 Total)

### Faction Leaders (5)
| Template ID | NPC Name | Faction | Personality |
|-------------|----------|---------|-------------|
| `FACTION_LEADER_IRONCLAD` | Emperor Garrick Steelarm | Ironclad Alliance | Authoritative |
| `FACTION_LEADER_EMBERLANDS` | Archmage Pyrus Ashborn | Ironclad Alliance | Sage |
| `FACTION_LEADER_DESERT` | Sultan Azir al-Rashid | League of Sands | Sultan |
| `FACTION_LEADER_FROSTHOLD` | Chieftain Bjorn Frostbeard | Polar Alliance | Chieftain |
| `FACTION_LEADER_VERDANTPEAK` | Elder Seraphina Leafwhisper | Sylvan Concord | Elder |

### Ancient Beings (9)
| Template ID | NPC Name | Type | Region |
|-------------|----------|------|--------|
| `ANCIENT_DEPTH_KING` | Abyssus the Depth King | Sea Dragon | Underwater |
| `ANCIENT_TREANT_ELDER` | Elder Oakbark | Treant | Verdantpeak |
| `ANCIENT_MACHINIST_CONSTRUCT` | The Great Machinist's Construct | Construct | Ironclad |
| `ANCIENT_WAR_TREANT` | Ironbark the War-Ancient | War Treant | Verdantpeak |
| `ANCIENT_CRYSTAL_DRAGON` | Crystalwing the Prismatic Oracle | Crystal Dragon | Crystal Barrens |
| `ANCIENT_FIRE_DRAGON` | Emberflame the Ashen Tyrant | Fire Dragon | Emberlands |
| `ANCIENT_FROST_AVATAR` | The Frost Father's Avatar | Divine Ice Spirit | Frosthold |
| `ANCIENT_NATURE_HERALD` | Lunara's Dryad Herald | Divine Nature Spirit | Verdantpeak |
| `ANCIENT_CRYSTAL_SPHINX` | The Crystal Sphinx | Sphinx | Crystal Barrens |

### Regional Bosses (10)
| Template ID | Boss Name | Boss Type | Region |
|-------------|-----------|-----------|--------|
| `BOSS_FROST_FATHER` | The Frost Father | FrostFather | Frosthold |
| `BOSS_FORGE_MASTER` | The Forge Master | ForgeMaster | Ironclad |
| `BOSS_COVEN_MATRIARCH` | The Coven Matriarch | CovenMatriarch | Shadowfen |
| `BOSS_VOLCANO_WYRM` | The Volcano Wyrm | VolcanoWyrm | Emberlands |
| `BOSS_TIMEWORN_LICH` | The Timeworn Lich | TimewornLich | ShadowVoid |
| `BOSS_ANCIENT_TREANT` | The Ancient Treant | AncientTreant | Verdantpeak |
| `BOSS_CRYSTAL_DRAKE` | Crystal Drake Alpha | CrystalDrakeAlpha | Crystal Barrens |
| `BOSS_SPHINX_SURYA` | The Sphinx of Surya | SphinxOfSurya | Desert |
| `BOSS_ANCIENT_KRAKEN` | The Ancient Kraken | AncientKraken | Underwater |
| `BOSS_GRIFFIN_LORD` | The Griffin Lord | GriffinLord | Skyreach |

### Class Trainers (Sample - 8 shown of 25)
| Template ID | Trainer Name | Class | Region |
|-------------|--------------|-------|--------|
| `TRAINER_BARBARIAN` | Battlemaster | Barbarian | Frosthold |
| `TRAINER_ICE_MAGE` | Frostweaver | Ice Mage | Frosthold |
| `TRAINER_DRUID` | Elder Grove | Druid | Verdantpeak |
| `TRAINER_WITCH` | Hexweaver | Witch | Shadowfen |
| `TRAINER_SORCERER` | Flameweaver | Sorcerer | Emberlands |
| `TRAINER_ARTIFICER` | Gearsmith | Artificer | Ironclad |
| `TRAINER_ORACLE` | Crystalseer | Oracle | Crystal Barrens |
| `TRAINER_NECROMANCER` | Shadowbinder | Necromancer | ShadowVoid |

### Quest Givers (Mondain/BaseQuest, 2)
| Template ID | NPC Name | Quest Type |
|-------------|----------|------------|
| `QUEST_QUARTERMASTER` | Quartermaster Grimwald | Supplies/Military |
| `QUEST_SAGE_THERON` | Sage Theron | Knowledge/Ancient Texts |

### Generic (1)
| Template ID | Usage |
|-------------|-------|
| `GENERIC_QUEST_NPC` | Fallback for auto-generated NPCs |

---

## Quick Test Commands

### Test Each Region:
```
[GenLLMQuest IRONCLAD_BRITAIN_CENTER     # Ironclad Empire
[GenLLMQuest FROSTHOLD_MINOC             # Frosthold
[GenLLMQuest VERDANTPEAK_YEW             # Verdantpeak
[GenLLMQuest EMBERLANDS_VOLCANO          # Emberlands
[GenLLMQuest LEAGUE_SANDS_TRINSIC        # Desert
[GenLLMQuest SHADOWFEN_SWAMP             # Shadowfen
[GenLLMQuest CRYSTAL_BARRENS_MOONGLOW    # Crystal Barrens
[GenLLMQuest SKYREACH_VESPER             # Skyreach
[GenLLMQuest MARITIME_JHELOM             # Maritime
[GenLLMQuest UNDERWATER_DEPTHS           # Underwater
[GenLLMQuest SHADOWVOID_DUNGEON          # ShadowVoid
```

### Test Special Locations:
```
[GenLLMQuest IRONCLAD_FORGE              # Great Forge
[GenLLMQuest VERDANTPEAK_GROVE           # Sacred Grove
[GenLLMQuest FROSTHOLD_GLACIER           # Eternal Glacier
[GenLLMQuest DESERT_RUINS                # Ancient Ruins
[GenLLMQuest SKYREACH_FLOATING_ISLE      # Floating Isle
```

---

## Waypoint Condition Types

| Condition | Usage | Notes |
|-----------|-------|-------|
| `TalkToNPC` | Quest NPC dialogue | Auto-completes on speech |
| `ReachLocation` | Travel to POI | Radius-based detection |
| `DefeatBoss` | Kill boss creature | Requires boss spawn |
| `CollectItems` | Gather items | Future implementation |
| `RecruitSidekick` | Recruit companion | Future implementation |
| `Custom` | Script-defined logic | For special mechanics |

---

## Boss Types (Allowlisted)

```
FrostFather         - Frosthold legendary boss
ForgeMaster         - Ironclad legendary boss
CovenMatriarch      - Shadowfen legendary boss
VolcanoWyrm         - Emberlands legendary boss
TimewornLich        - ShadowVoid legendary boss
AncientTreant       - Verdantpeak legendary boss
CrystalDrakeAlpha   - Crystal Barrens legendary boss
SphinxOfSurya       - Desert legendary boss
AncientKraken       - Underwater legendary boss
GriffinLord         - Skyreach legendary boss
```

---

## Quest Management Commands

### Find Quest NPCs

| Command | Description | Example |
|---------|-------------|---------|
| `[FindQuestNPC` / `[FQNPC` | Find quest NPCs for active quests | `[FQNPC` |
| `[FindQuestNPC respawn` | Respawn missing quest NPCs | `[FQNPC respawn` |

**Usage:**
- Shows all NPCs for your active quests and their locations
- Displays distance to each NPC
- Use `respawn` parameter to recreate missing NPCs at waypoint locations

### Clear Quests

| Command | Description | Example |
|---------|-------------|---------|
| `[ClearQuests` | Clear all your own quests | `[ClearQuests` |
| `[ClearQuests <playerName>` | Clear all quests for a player | `[ClearQuests Runescry` |

**What it clears:**
- All active Vystia quests
- All completed Vystia quests
- All LLM-generated ephemeral quest instances

### Quest Editor

| Command | Description |
|---------|-------------|
| `[QuestEditor` / `[QE` | Open quest editor gump |
| `[addquestNPC` / `[aqn` | Open quest NPC spawn wizard |

---

*Quest Generation Quick Reference*
*Last Updated: 2025-12-12*
