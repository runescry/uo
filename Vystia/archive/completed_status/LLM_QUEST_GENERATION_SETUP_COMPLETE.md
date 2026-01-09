# LLM Dynamic Quest Generation - Setup Complete! ✅

**Date:** 2025-12-12
**Status:** Ready for testing

---

## What Was Created

### 1. **POI Registry** ✅
**File:** `D:\UO\Data\Vystia\poi_registry.json`
**Contains:** 20 Point-of-Interest locations across all Vystia regions

**POIs Created:**
- **Ironclad Empire** (4): Britain Center, Britain Bank, Great Forge, general area
- **Frosthold** (2): Minoc, Eternal Glacier
- **Verdantpeak** (2): Yew Forest, Sacred Grove
- **Emberlands** (1): Volcanic Region
- **Desert of Surya** (1): Ancient Ruins
- **Shadowfen** (1): Swamp Depths
- **Crystal Barrens** (2): Moonglow, Crystal Fields
- **Skyreach Peaks** (2): Vesper, Floating Isle
- **Maritime Sovereignty** (2): Jhelom, Grand Harbor
- **Underwater** (1): Deepwater Trench
- **ShadowVoid** (1): Planar Rift
- **New Haven** (1): Training Grounds (starter area)

Each POI includes:
- `poiId`: Stable identifier (e.g., "IRONCLAD_BRITAIN_CENTER")
- `poiName`: Display name (e.g., "Ironclad Empire - Britain Commons")
- `map`: UO map name (Felucca/Trammel)
- `x`, `y`, `z`: Exact coordinates
- `radius`: Detection radius for waypoints
- `tags`: Searchable keywords (region, theme, features)

---

### 2. **NPC Template Registry** ✅
**File:** `D:\UO\Data\Vystia\npc_templates.json`
**Contains:** 42 NPC templates ready for quest use

**NPC Categories:**

#### **Faction Leaders (5)**
- Emperor Garrick Steelarm (Ironclad)
- Archmage Pyrus Ashborn (Emberlands)
- Sultan Azir al-Rashid (Desert)
- Chieftain Bjorn Frostbeard (Frosthold)
- Elder Seraphina Leafwhisper (Verdantpeak)

#### **Ancient Beings (9)**
- Abyssus the Depth King (Sea Dragon)
- Elder Oakbark (Ancient Treant)
- The Great Machinist's Construct (Divine Clockwork)
- Ironbark the War-Ancient (War Treant)
- Crystalwing the Prismatic Oracle (Crystal Dragon)
- Emberflame the Ashen Tyrant (Fire Dragon)
- The Frost Father's Avatar (Divine Ice Spirit)
- Lunara's Dryad Herald (Divine Nature Spirit)
- The Crystal Sphinx (Riddle Master)

#### **Regional Bosses (10)**
- Frost Father (Frosthold)
- Forge Master (Ironclad)
- Coven Matriarch (Shadowfen)
- Volcano Wyrm (Emberlands)
- Timeworn Lich (ShadowVoid)
- Ancient Treant (Verdantpeak)
- Crystal Drake Alpha (Crystal Barrens)
- Sphinx of Surya (Desert)
- Ancient Kraken (Underwater)
- Griffin Lord (Skyreach)

#### **Class Trainers (8 samples)**
- Barbarian Battlemaster
- Ice Mage Adept
- Druid Elder
- Witchcraft Practitioner
- Sorcerer Master
- Artificer Master
- Oracle Seer
- Necromancer Lord

#### **Quest Givers (2)**
- Quartermaster Grimwald
- Sage Theron

#### **Generic Template (1)**
- Generic Quest NPC (fallback)

Each template includes:
- `templateId`: Stable identifier
- `displayName`: Full display name
- `mobileTypeName`: C# class name to spawn
- `name`, `title`: NPC naming
- `defaultPersonality`: LLM personality type
- `defaultSpeechPattern`: Speech style
- `knowledgeTags`: Knowledge domains for context

---

### 3. **[GenLLMQuest] Command** ✅
**File:** `D:\UO\ServUO\Scripts\Custom\Commands\GenLLMQuestCommand.cs`

**Usage:**
```
[GenLLMQuest [poiId]
```

**Examples:**
```
[GenLLMQuest IRONCLAD_BRITAIN_CENTER
[GenLLMQuest FROSTHOLD_MINOC
[GenLLMQuest VERDANTPEAK_YEW
[GenLLMQuest CRYSTAL_BARRENS_MOONGLOW
```

**What It Does:**
1. Generates a demo quest plan JSON (simulates LLM output)
2. Validates the plan against POI registry and safety rules
3. Compiles the plan into a `DynamicQuest` with waypoints
4. Spawns quest NPCs and boss creatures at POI locations
5. Creates a per-player/party instance with automatic cleanup
6. Shows quest offer gump to player

**Demo Quest:**
- **Title:** "The Ironclad Supplies Crisis"
- **Quest Giver:** Quartermaster Grimwald (spawned at POI)
- **Objective:** Defeat the Forge Master boss
- **Completion:** Return to Quartermaster Grimwald
- **Expiry:** 120 minutes (auto-cleanup)

---

## How to Test (Step-by-Step)

### **Step 1: Build the Server**
```bash
cd D:\UO\ServUO
dotnet build
```

Expected: 0 errors, 0 warnings

### **Step 2: Start ServUO**
```bash
cd D:\UO\ServUO
dotnet run
```

### **Step 3: Connect with Client**
- Use UO client or ClassicUO
- Login as GM account

### **Step 4: Test POI Registry Loading**
In-game console, you should see:
```
VystiaPoiRegistry: Loaded 20 POIs from Data/Vystia/poi_registry.json
```

If not loaded, the command will auto-load on first use.

### **Step 5: Generate Your First Quest**
```
[GenLLMQuest IRONCLAD_BRITAIN_CENTER
```

**Expected Behavior:**
1. ✅ Console: "Loaded 20 POIs"
2. ✅ Quest offer gump appears
3. ✅ Quest details shown:
   - Title: "The Ironclad Supplies Crisis"
   - Description: Quest lore
   - Waypoints: 3 steps
   - Expiry: 120 minutes

### **Step 6: Accept the Quest**
Click "Accept" in the gump.

**Expected:**
- Quest added to your quest log (`[questlog` or `[ql`)
- Quartermaster Grimwald spawns at the POI location
- Objective: Talk to Quartermaster Grimwald

### **Step 7: Complete Waypoint 1 - Talk to NPC**
Walk to the spawned NPC and say "hello" or "quest".

**Expected:**
- NPC responds with LLM-aware dialogue
- Quest updates: "Defeat the Forge Master"
- Forge Master boss spawns nearby

### **Step 8: Complete Waypoint 2 - Defeat Boss**
Kill the Forge Master.

**Expected:**
- Boss dies
- Quest updates: "Return to Quartermaster Grimwald"
- Loot drops from boss

### **Step 9: Complete Waypoint 3 - Return**
Talk to Quartermaster Grimwald again.

**Expected:**
- Quest completes
- Rewards granted (if configured)
- Quest removed from log
- NPCs despawn (cleanup)

### **Step 10: Test Auto-Cleanup**
Wait 120 minutes (or change `expiresMinutes` in demo plan to 1 minute).

**Expected:**
- After expiry, all spawned NPCs/bosses delete automatically
- Quest unregisters from system
- Instance attachment removed

---

## Testing Checklist

- [ ] POI registry loads (20 POIs)
- [ ] NPC template registry loads (42 templates)
- [ ] [GenLLMQuest command works
- [ ] Quest offer gump appears
- [ ] Quest can be accepted
- [ ] Quest NPC spawns at correct POI location
- [ ] Boss spawns when waypoint activates
- [ ] Boss death triggers next waypoint
- [ ] Quest completion works
- [ ] Auto-cleanup removes spawned entities after expiry
- [ ] Quest log integration works

---

## Available POIs for Testing

### **Starter-Friendly:**
```
[GenLLMQuest NEWHAVEN_TRAINING        # Safe, Trammel
[GenLLMQuest IRONCLAD_BRITAIN_CENTER  # Central hub
[GenLLMQuest VERDANTPEAK_YEW          # Forest area
```

### **Mid-Level:**
```
[GenLLMQuest FROSTHOLD_MINOC          # Frozen mountains
[GenLLMQuest CRYSTAL_BARRENS_MOONGLOW # Magic island
[GenLLMQuest SKYREACH_VESPER          # Wind peaks
```

### **Advanced:**
```
[GenLLMQuest EMBERLANDS_VOLCANO       # Fire/lava danger
[GenLLMQuest SHADOWFEN_SWAMP          # Poison swamp
[GenLLMQuest UNDERWATER_DEPTHS        # Ocean depths
[GenLLMQuest SHADOWVOID_DUNGEON       # Planar void
```

---

## Architecture Overview

### **Pipeline Flow:**
1. **Input:** Quest plan JSON (from LLM or demo)
2. **Validator:** Checks POI existence, NPC templates, safety rules
3. **Compiler:** Converts plan → `DynamicQuest` + `QuestWaypoint`s
4. **Spawner:** Creates quest NPCs and bosses at POIs
5. **Instance Tracker:** Per-player/party attachment with expiry
6. **Cleanup:** Automatic deletion on expiry/abandonment

### **Data Files:**
- `Data/Vystia/poi_registry.json` - 20 POIs
- `Data/Vystia/npc_templates.json` - 42 NPC templates
- `Data/VystiaQuests.xml` - Persistent quests (excludes ephemeral)

### **Code Files:**
```
ServUO/Scripts/Custom/VystiaClasses/Quests/Generation/
├── DynamicQuestPlanModels.cs          # JSON contract
├── VystiaPoiRegistry.cs               # POI loader
├── QuestPlanValidator.cs              # Hard validation
├── QuestPlanCompilerAndSpawner.cs     # Plan → Quest + spawn
├── GeneratedQuestInstanceAttachment.cs # Per-player instance
├── GeneratedQuestCleanup.cs           # Auto-cleanup timer
└── LLMQuestGenerationService.cs       # Orchestrator

ServUO/Scripts/Custom/Commands/
└── GenLLMQuestCommand.cs              # [GenLLMQuest] command
```

---

## Next Steps

### **Immediate (MVP Phase 1):**
- [x] POI registry created
- [x] NPC template registry created
- [x] Demo command created
- [ ] **Test the full quest flow** (Steps 1-10 above)
- [ ] Debug any issues found during testing

### **Future (Phase 2 - Real LLM Integration):**
- [ ] Implement OpenAI planner (or local LLM)
- [ ] Create "Chronicler NPC" as production entry point
- [ ] Add quest board UI for players
- [ ] Add quest difficulty scaling
- [ ] Implement multi-quest arcs (questlines)

### **Future (Phase 3 - World Events):**
- [ ] Scheduled regional events
- [ ] Faction-based quest generation
- [ ] Shared event seeds for server-wide participation

---

## Troubleshooting

### **"POI registry not found"**
**Fix:** Ensure `D:\UO\Data\Vystia\poi_registry.json` exists.

### **"NPC template not found"**
**Fix:** Check `npc_templates.json` contains the `templateId` referenced in quest plan.

### **"Failed to spawn NPC"**
**Fix:** Verify `mobileTypeName` in NPC template matches actual C# class name.

### **Quest doesn't expire**
**Fix:** Check `expiresMinutes` in quest plan JSON. Default demo: 120 minutes.

### **Boss doesn't spawn**
**Fix:** Verify boss type name in allowlist (`QuestPlanValidator.cs`).

---

## Allowlisted Boss Types (For Safety)

The following boss types can be spawned via quest generation:

```csharp
"FrostFather"
"ForgeMaster"
"CovenMatriarch"
"VolcanoWyrm"
"TimewornLich"
"AncientTreant"
"CrystalDrakeAlpha"
"SphinxOfSurya"
"AncientKraken"
"GriffinLord"
```

To add more, edit `QuestPlanValidator.AllowedBossTypes` in `QuestPlanValidator.cs`.

---

## Custom Quest Plan Format

If you want to test custom quest plans, create a JSON file matching this schema:

```json
{
  "title": "Your Quest Title",
  "description": "Quest description...",
  "tier": 1,
  "expiresMinutes": 120,
  "waypoints": [
    {
      "type": "Origin",
      "condition": "TalkToNPC",
      "poiId": "IRONCLAD_BRITAIN_CENTER",
      "npcTemplateId": "QUEST_QUARTERMASTER",
      "npcDialogueContext": "Hello adventurer!",
      "playerLocationHint": "Seek the quartermaster..."
    },
    {
      "type": "Waypoint",
      "condition": "DefeatBoss",
      "poiId": "IRONCLAD_BRITAIN_CENTER",
      "bossTypeName": "ForgeMaster",
      "npcDialogueContext": "Defeat the boss!",
      "playerLocationHint": "Journey to the forge..."
    },
    {
      "type": "NPCCompletion",
      "condition": "TalkToNPC",
      "poiId": "IRONCLAD_BRITAIN_CENTER",
      "npcTemplateId": "QUEST_QUARTERMASTER",
      "npcDialogueContext": "Well done!",
      "playerLocationHint": "Return to the quartermaster..."
    }
  ]
}
```

Then modify the command to load your JSON instead of the demo.

---

## Safety Features Active

✅ **POI Allowlist** - Only registered POIs can be used
✅ **Boss Allowlist** - Only approved boss types can spawn
✅ **Hard Bounds** - Max waypoints, spawn counts, reward caps
✅ **Content Filters** - Rejects profanity, exploits, player-kill objectives
✅ **Auto-Cleanup** - Prevents world bloat from abandoned quests
✅ **Ephemeral Flag** - Generated quests excluded from `VystiaQuests.xml`

---

## Summary

**What You Have:**
- 20 POIs across all Vystia regions (Ironclad → Britain, Frosthold → Minoc, etc.)
- 42 NPC templates (faction leaders, ancient beings, bosses, trainers, quest givers)
- Complete quest generation pipeline (plan → validate → compile → spawn → cleanup)
- [GenLLMQuest] command for instant testing
- Demo quest ready to run

**What to Do:**
1. Build ServUO (`dotnet build`)
2. Run server (`dotnet run`)
3. Login as GM
4. Run `[GenLLMQuest IRONCLAD_BRITAIN_CENTER`
5. Accept quest → Talk to NPC → Kill boss → Return to NPC → Complete!

**Status:** ✅ **READY FOR TESTING!**

Next: Wire real LLM planner when you're ready for production AI-generated quests.

---

*LLM Quest Generation System - Setup Complete*
*Created: 2025-12-12*
