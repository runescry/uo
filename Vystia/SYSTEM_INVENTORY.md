# VYSTIA COMPLETE SYSTEM INVENTORY
**Generated: 2026-01-01**

---

## SECTION 1: CHARACTER CLASS SYSTEM

### 1.1 Character Classes
**Path:** `ServUO/Scripts/Custom/VystiaClasses/Classes/`
**Total:** 21 class files

| Class | Region | Status |
|-------|--------|--------|
| Barbarian | Frosthold | Complete |
| Beastmaster | Frosthold | Complete |
| Ice Mage | Frosthold | Complete |
| Sorcerer | Emberlands | Complete |
| Ranger | Desert | Complete |
| Illusionist | Desert | Complete |
| Witch | Shadowfen | Complete |
| Warlock | ShadowVoid | Complete |
| Necromancer | ShadowVoid | Complete |
| Druid | Verdantpeak | Complete |
| Alchemist | Verdantpeak | Complete |
| Wizard | Crystal Barrens | Complete |
| Oracle | Crystal Barrens | Complete |
| Artificer | Ironclad | Complete |
| Fighter | Ironclad | Complete |
| Monk | Ironclad | Complete |
| Templar | Ironclad | Complete |
| Summoner | Underwater | Complete |
| Bounty Hunter | Multi-Regional | Complete |
| Knight | Multi-Regional | Complete |
| Shaman | Multi-Regional | Complete |
| Cleric | Multi-Regional | Complete |
| Paladin | Multi-Regional | Complete |
| Bard | Multi-Regional | Complete |
| Enchanter | Multi-Regional | Complete |

### 1.2 Class System v2.0 Core Files
**Path:** `ServUO/Scripts/Custom/VystiaClasses/`

| File | Description | Status |
|------|-------------|--------|
| Systems/SecondaryResource.cs | Secondary Resources (15 types) | OK |
| Systems/VystiaResourceManager.cs | Resource Manager | OK |
| Systems/TargetTracker.cs | Target Tracker | OK |
| Systems/VystiaBuffSystem.cs | Buff/Debuff System | OK |
| Systems/VystiaDamageSystem.cs | Damage Pipeline | OK |
| Systems/CrowdControlSystem.cs | Crowd Control (15 CC types) | OK |
| Abilities/AbilityDefinition.cs | Ability Definition | OK |
| Abilities/AbilityExecutor.cs | Ability Executor | OK |
| Systems/StanceSystem.cs | Stance System (28 stances) | OK |
| Classes/PlayerClassV2.cs | Class Framework | OK |
| Core/PlayerClass.cs | Base Class System | OK |
| Core/VystiaClassApplicator.cs | Class Applicator | OK |

### 1.3 Generated Abilities
**Path:** `ServUO/Scripts/Custom/VystiaClasses/Abilities/Generated/`

| Category | Files | Abilities |
|----------|-------|-----------|
| Magic Ability Files | 13 | ~256 |
| Martial Ability Files | 15 | ~256 |
| **TOTAL** | **28** | **~512** |

### 1.4 Class Trainers
**Path:** `ServUO/Scripts/Mobiles/Vystia/Trainers/`
**Files:** 1+ (VystiaClassTrainers.cs contains all 25 trainers)

---

## SECTION 2: MAGIC SYSTEM

### 2.1 Magic Spells
**Path:** `ServUO/Scripts/Custom/VystiaClasses/Spells/`
**Schools:** 12 | **Total Spells:** 384

| School | Skill | Skill ID | Spells | Spell IDs |
|--------|-------|----------|--------|-----------|
| Ice Magic | Cryomancy | 58 | 32 | 1000-1031 |
| Nature Magic | Druidism | 59 | 32 | 1032-1063 |
| Hex Magic | Hexcraft | 60 | 32 | 1064-1095 |
| Elemental Magic | Elementalism | 61 | 32 | 1096-1127 |
| Dark Magic | Demonology | 62 | 32 | 1128-1159 |
| Divination | Divination | 63 | 32 | 1160-1191 |
| Necromancy | NecromancyArts | 64 | 32 | 1192-1223 |
| Summoning | Conjuration | 65 | 32 | 1224-1255 |
| Shamanic | SpiritCalling | 66 | 32 | 1256-1287 |
| Bardic | BardicLore | 67 | 32 | 1288-1319 |
| Enchanting | Runeweaving | 68 | 32 | 1320-1351 |
| Illusion | IllusionMagic | 69 | 32 | 1352-1383 |

### 2.2 Magic Reagents
**Path:** `ServUO/Scripts/Items/Vystia/Resources/Reagents/`
**Files:** 13 | **Total Reagents:** 104 (8 per school)

| School | Reagent File | Reagents |
|--------|--------------|----------|
| Ice | IceMagicReagents.cs | 8 |
| Nature | NatureMagicReagents.cs | 8 |
| Hex | HexMagicReagents.cs | 8 |
| Elemental | ElementalMagicReagents.cs | 8 |
| Dark | DarkMagicReagents.cs | 8 |
| Divination | DivinationMagicReagents.cs | 8 |
| Necromancy | NecromancyReagents.cs | 8 |
| Summoning | SummoningMagicReagents.cs | 8 |
| Shamanic | ShamanicMagicReagents.cs | 8 |
| Bardic | BardicMagicReagents.cs | 8 |
| Enchanting | EnchantingMagicReagents.cs | 8 |
| Illusion | IllusionMagicReagents.cs | 8 |

### 2.3 Spell Scrolls
**Path:** `ServUO/Scripts/Items/Vystia/Scrolls/`
**Files:** 12 | **Total Scrolls:** 384 (32 per school)

### 2.4 Spellbooks
**Path:** `ServUO/Scripts/Items/Equipment/Spellbooks/VystiaSpellbooks.cs`
**Total:** 12 spellbooks

| Spellbook | Class |
|-----------|-------|
| IceMageSpellbook | Ice Mage |
| DruidSpellbook | Druid |
| WitchSpellbook | Witch |
| SorcererSpellbook | Sorcerer |
| WarlockSpellbook | Warlock |
| OracleSpellbook | Oracle |
| VystiaNecromancerSpellbook | Necromancer |
| SummonerSpellbook | Summoner |
| ShamanSpellbook | Shaman |
| BardSpellbook | Bard |
| EnchanterSpellbook | Enchanter |
| IllusionistSpellbook | Illusionist |

### 2.5 Custom Skills
**Skill IDs:** 58-83 (26 total)

| Category | IDs | Count |
|----------|-----|-------|
| Magic Skills | 58-69 | 12 |
| Martial Skills | 70-83 | 14 |

---

## SECTION 3: CREATURES & NPCs

### 3.1 Creatures by Region
**Path:** `ServUO/Scripts/Mobiles/Vystia/`
**Total:** 138 creatures

| Region | Count | Theme |
|--------|-------|-------|
| Bosses | 10 | Regional champions |
| Frosthold | 12 | Ice/cold |
| Emberlands | 8 | Fire/lava |
| Desert | 11 | Sand/heat |
| Shadowfen | 13 | Swamp/poison |
| Verdantpeak | 13 | Forest/nature |
| Crystal Barrens | 4 | Crystal/energy |
| Ironclad | 9 | Mechanical/steam |
| Skyreach | 15 | Wind/lightning |
| Underwater | 12 | Aquatic |
| ShadowVoid | 9 | Void/darkness |
| Misc | 15 | Generic |
| NPCs | 1 | Story NPCs |
| Vendors | 4 | Shops |
| Trainers | 1 | Class trainers |

### 3.2 Vendors
**Path:** `ServUO/Scripts/Mobiles/Vystia/Vendors/`
**Files:** 4 | **Total Vendors:** 14+

| Vendor Type | Count |
|-------------|-------|
| Magic School Vendors | 12 |
| VystiaReagentVendor | 1 |
| VystiaResourceVendor | 1 |

### 3.3 Custom Races
| Race | Body IDs | Status |
|------|----------|--------|
| Dwarf | 987 (M), 988 (F) | Complete |

---

## SECTION 4: AI SYSTEMS

### 4.1 AI Sidekicks
**Path:** `ServUO/Scripts/Services/AISidekicks/`
**Total C# Files:** 27 | **Python Simulation:** 14 files

#### Core Files
| File | Description |
|------|-------------|
| Core/BaseSidekick.cs | Base sidekick class |
| Core/AutonomousSidekick.cs | Autonomous behavior |
| Core/SidekickSpeechHandler.cs | Speech handling |

#### Combat AI Archetypes (8 types)
| AI | File |
|----|------|
| Warrior | AI/WarriorCombatAI.cs |
| Mage | AI/MageCombatAI.cs |
| Healer | AI/HealerCombatAI.cs |
| Tamer | AI/TamerCombatAI.cs |
| Archer | AI/ArcherCombatAI.cs |
| Paladin | AI/PaladinCombatAI.cs |
| Thief | AI/ThiefCombatAI.cs |
| Necromancer | AI/NecromancerCombatAI.cs |

#### Other Systems
| Category | Files |
|----------|-------|
| UI Gumps | 5 |
| Config | 1 |
| Data | 3 |
| Commands | 1 |
| Services | 1 |

### 4.2 LLM NPC System
**Path:** `ServUO/Scripts/Services/LLM/`
**Total Files:** 46

#### Core
| File | Description |
|------|-------------|
| Core/LLMNpc.cs | Base LLM NPC |
| Core/LLMQuester.cs | Quest-giving NPCs |
| Core/ConversationContext.cs | Conversation state |
| Core/LLMInitializer.cs | System initialization |
| Core/ExampleLLMNpcs.cs | Example implementations |

#### Services
| File | Description |
|------|-------------|
| Services/LLMService.cs | Main LLM service |
| Services/LocalLLMService.cs | Local model support |
| Services/UnifiedLLMService.cs | Unified interface |
| Services/LLMMemoryService.cs | Memory persistence |
| Services/SQLiteMemoryDatabase.cs | SQLite storage |
| Services/LLMResponseCache.cs | Response caching |
| Services/LLMErrorHandler.cs | Error handling |

#### Data Systems
| File | Description |
|------|-------------|
| Data/NPCPersonalities.cs | Personality types |
| Data/NPCPersonalityDatabase.cs | Personality storage |
| Data/NPCKnowledgeSystem.cs | Knowledge base |
| Data/NPCLocationDatabase.cs | Location awareness |
| Data/VectorLoreSystem.cs | Vector embeddings |
| Data/EmbeddingService.cs | Embedding generation |
| Data/SimpleLoreSystem.cs | Simple lore lookup |
| Data/KnowledgeDomain.cs | Knowledge domains |

#### Interface
| File | Description |
|------|-------------|
| ILLMConversational.cs | Conversation interface |
| LLMConversationPlugin.cs | Plugin system |
| LLMConversationHelper.cs | Helper utilities |

#### Commands
| Command | Description |
|---------|-------------|
| SpawnLLMQuester | Spawn quest NPC (Mondain/BaseQuest) |
| SpawnPersonalityNPC | Spawn personality NPC |
| SpawnNPCGroups | Spawn NPC groups |
| LLMMenuCommand | LLM menu |
| CacheCommands | Cache management |
| MemoryTestCommand | Memory testing |
| KnowledgeTestCommand | Knowledge testing |
| LocationTestCommand | Location testing |
| ClearMemoryCommand | Clear memory |

---

## SECTION 5: QUEST SYSTEM

### 5.1 Quest Components
| Component | Status |
|-----------|--------|
| Quest Editor Gump (Dynamic) | Complete |
| QuestNPC (Dynamic) | Complete |
| DynamicQuest/Waypoints (Dynamic) | Complete |
| LLMQuester (Mondain/BaseQuest) | Complete |

---

## SECTION 6: ITEMS & EQUIPMENT

### 6.1 Equipment
**Path:** `ServUO/Scripts/Items/Vystia/Equipment/`

| Category | Files | Items |
|----------|-------|-------|
| Weapons | 2 | ~45 |
| Armor | 8 | ~59 |
| Shields | 1 | ~8 |
| **TOTAL** | **11** | **~112** |

### 6.2 Class Items
**Path:** `ServUO/Scripts/Custom/VystiaClasses/Items/`
**Total Files:** 9

| File | Description | Status |
|------|-------------|--------|
| ClassFocusItems.cs | Focus Items (25) | OK |
| ResourceConsumables.cs | Consumables | OK |
| AbilityItems/RageTotem.cs | Rage Totem | OK |
| AbilityItems/ClassSpecialItems.cs | Special Items | OK |

### 6.3 Resources
**Path:** `ServUO/Scripts/Items/Vystia/Resources/`

| Category | Files |
|----------|-------|
| Components | 1 |
| Ingots | 1 |
| Leathers | 1 |
| Ores | 1 |
| Reagents | 13 |
| Special | 1 |
| Woods | 1 |

---

## SECTION 7: CRAFTING SYSTEMS

**Path:** `ServUO/Scripts/Custom/VystiaClasses/Crafting/`

| System | File | Status |
|--------|------|--------|
| Alchemist Transmutation | DefTransmutation.cs | Complete |
| Artificer Engineering | DefEngineering.cs | Complete |

---

## SECTION 8: UI SYSTEMS

### 8.1 Gumps
**Path:** `ServUO/Scripts/Custom/VystiaClasses/Gumps/`
**Total:** 11 files

| Gump | Command | Status |
|------|---------|--------|
| VystiaAdminGump.cs | [VA] | Complete |
| VystiaQuestEditorGump.cs | [QE] | Complete |
| SpawnVystiaGump.cs | [spawnvystia] | Complete |

---

## SECTION 9: DOCUMENTATION

### 9.1 Reference Documentation
**Path:** `Vystia/reference/`

| Document | Description | Status |
|----------|-------------|--------|
| CLASSES.md | All 25 classes | OK |
| SPELLS.md | All 384 spells | OK |
| SKILLS.md | All 26 skills | OK |
| COMMANDS.md | GM commands | OK |

### 9.2 Guide Documentation
**Path:** `Vystia/gm/`

| Document | Description | Status |
|----------|-------------|--------|
| GM_TESTING_GUIDE.md | Testing procedures | OK |

### 9.3 AI Context Files
| File | Description | Status |
|------|-------------|--------|
| /CLAUDE.md | Master context | OK |
| /ServUO/CLAUDE.md | ServUO context | OK |
| VystiaClasses/CLAUDE.md | Class system context | OK |

---

## SECTION 10: TOOLS & UTILITIES

### 10.1 Python Tools
**Path:** `Vystia/tools/`
**Total:** 106 Python scripts

Key tools:
- inventory_vystia.py - System inventory
- dwarf_sprite_creator.py - Dwarf race sprites
- map_data_extractor.py - Map utilities
- generate_*.py - Code generation scripts

### 10.2 GM Commands
**Path:** `ServUO/Scripts/Custom/VystiaClasses/Commands/`
**Files:** 4

---

## COMPLETE SUMMARY

| System | Count | Status |
|--------|-------|--------|
| Character Classes | 21 | Complete |
| Magic Schools | 12 | Complete |
| Total Spells | 384 | Complete |
| Creatures | 138 | Complete |
| Reagent Types | 104 | Complete |
| Spell Scrolls | 384 | Complete |
| Spellbooks | 12 | Complete |
| Vendors | 14+ | Complete |
| Custom Skills | 26 | Complete |
| AI Sidekicks | 27 C# + 14 Py | Complete |
| LLM NPC System | 46 files | Complete |
| Gumps | 11 | Complete |
| Abilities | ~512 | Complete |
| Class Trainers | 25 | Complete |
| Crafting Systems | 2 | Complete |
| Custom Races | 1 (Dwarf) | Complete |
| Python Tools | 106 | Complete |

---

## OVERALL STATUS: ~96% Complete

All core systems are production-ready:
- Character class system v2.0
- Complete magic system (384 spells)
- AI Sidekicks (8 combat archetypes)
- LLM NPC system (46 files)
- 138 custom creatures
- Full equipment system

---

*Last Updated: 2026-01-01*
*Generated by inventory_vystia.py*
