# VYSTIA NPC GENERATION - PHASE 1 COMPLETE

**Generated:** 2025-12-08
**Updated:** 2025-12-08 (Build errors fixed)
**Status:** ✅ All 13 NPCs building successfully, ready for in-game testing

## What Was Created

### NPC Generator Tool
**Location:** `D:\UO\Vystia\tools\generate_npcs.py`

**Features:**
- Template-based C# NPC generation
- Proper ServUO inheritance and serialization
- LLM integration placeholders
- Keyword-based dialogue system
- Quest giver support

### Generated NPCs (13 total)

**Output Directory:** `D:\UO\ServUO\Scripts\Mobiles\Vystia\NPCs\`

#### 1. Faction Leaders (5 NPCs)
Location: `NPCs/FactionLeaders/`

| NPC | Faction | Location | Personality |
|-----|---------|----------|-------------|
| **Emperor Garrick Steelarm** | Ironclad Alliance | Imperial Palace, Ironhaven | Visionary leader, strategic genius |
| **Chieftain Bjorn Frostbeard** | Polar Alliance | Frost Palace, Frostholm | Legendary warrior, honorable |
| **Elder Seraphina Leafwhisper** | Sylvan Concord | Heart Tree, Verdantheart | Ancient elf, wise, protective |
| **Sultan Azir al-Rashid** | League of Sands | Palace of Sun and Sand, Sunspire | Shrewd merchant, neutral diplomat |
| **Archmage Pyrus Ashborn** | Ironclad Alliance | Magma Citadel, Emberforge | Powerful fire sorcerer, ambitious |

#### 2. Talking Creatures (3 NPCs)
Location: `NPCs/TalkingCreatures/`

| Creature | Type | Age | Location | Personality |
|----------|------|-----|----------|-------------|
| **Frosthelm the Eternal Winter** | White Ancient Dragon | 3000+ years | Frozen Peak, Frosthold | Ancient, wise, protective |
| **Elder Oakbark** | Ancient Treant | 2000+ years | Deep Verdantpeak Forest | Patient, guardian |
| **Sphinx of Surya** | Desert Sphinx | 5000+ years | Ancient ruins, Whispering Sands | Riddler, knowledge keeper |

#### 3. Essential Vendors (3 NPCs)
Location: `NPCs/Vendors/`

| NPC | Type | Location | Services |
|-----|------|----------|----------|
| **Ironhaven Banker** | Banker | Ironhaven | Banking services |
| **Frostholm Healer** | Healer | Frostholm | Healing, cures, resurrections |
| **Ironhaven Guard Captain** | Guard | Ironhaven Gates | Security, law enforcement |

#### 4. Quest Givers (2 NPCs)
Location: `NPCs/QuestGivers/`

| NPC | Quest | Location | Personality |
|-----|-------|----------|-------------|
| **Quartermaster Grimwald** | Supply Line | Ironhaven Barracks | Gruff military veteran |
| **Sage Theron** | Ancient Texts | Verdantheart Library | Scholarly, curious |

#### 5. Spawn Commands
Location: `NPCs/VystiaNPCCommands.cs`

**GM Commands:**
```
[SpawnVystiaLeader <name>
  - garrick, bjorn, seraphina, azir, pyrus

[SpawnVystiaCreature <name>
  - frosthelm, oakbark, sphinx
```

## NPC Features

### All NPCs Include:
- ✅ Proper ServUO base class inheritance (BaseVendor, BaseCreature, MondainQuester)
- ✅ Full serialization/deserialization
- ✅ Regional hue themes matching world regions
- ✅ Appropriate equipment and appearance
- ✅ Stats appropriate to their role
- ✅ LLM integration placeholders

### Faction Leaders Have:
- High stats befitting their status (500-700 HP, 300-500 Mana)
- Regal appearance with faction-colored clothing
- Keyword-based dialogue responses
- Full lore context for LLM integration
- CanTeach = true (can teach skills)

### Talking Creatures Have:
- Very high stats (5000-7000 HP, ancient power level)
- Custom body IDs for unique appearance
- Keyword dialogue system
- OnSpeech event handling
- Karma = 25000 (ancient guardians, not hostile)
- CanSpeakTo and CanTeach enabled

### Vendors Have:
- BaseVendor inheritance
- SBInfo lists for buy/sell
- Appropriate equipment by vendor type
- Professional appearance

### Quest Givers Have:
- MondainQuester inheritance
- Quest array setup
- Quest-specific dialogue
- Appropriate appearance

## Build Fixes Applied

### Issues Fixed (2025-12-08)

1. **Missing `using System.Collections.Generic;` directive**
   - Added to faction leader template
   - Required for `List<SBInfo>` in vendor NPCs

2. **Invalid `CanSpeakTo(Mobile)` override**
   - Removed from talking creature template
   - Method doesn't exist as overridable in BaseCreature
   - Used `CanTeach = true` and `PlayerRangeSensitive = false` instead

3. **Class name typo: FroslmEternalWinter → FrosthelmEternalWinter**
   - Fixed in `generate_npcs.py` line 196
   - Changed `.replace("the", "")` to `.replace(" the ", " ")`
   - Now only removes " the " as a complete word, not "the" within words
   - Fixed both class name and filename generation

**Build Result:** ✅ 0 errors, 0 warnings

## Testing the NPCs

### Step 1: Build ServUO
```bash
cd D:\UO\ServUO
dotnet build
```

### Step 2: Start Server
```bash
dotnet run
```

### Step 3: Test Spawning

**Faction Leaders:**
```
[add EmperorGarrickSteelarm
[add ChieftainBjornFrostbeard
[add ElderSeraphinaLeafwhisper
[add SultanAziralRashid
[add ArchmagePyrusAshborn
```

**Talking Creatures:**
```
[add FrosthelmEternalWinter
[add ElderOakbark
[add SphinxofSurya
```

**Vendors:**
```
[add IronhavenBanker
[add FrostholmHealer
[add IronhavenGuardCaptain
```

**Using Spawn Commands:**
```
[SpawnVystiaLeader garrick
[SpawnVystiaCreature frosthelm
```

### Step 4: Test Dialogue

Talk to NPCs:
- Say "greetings" or "hello" → Should respond
- Say "faction" → Faction leaders should explain their faction
- Say "lore" → Talking creatures should share ancient knowledge
- Say "quest" → Quest givers should mention tasks

## Next Steps - Expansion

### Phase 2: More NPCs to Generate

From `VYSTIA_NPC_DESIGN.md`, we still need:

#### 1. City NPCs (200+ NPCs)
For each of 10 capital cities:
- 2 Bankers (one at main bank, one at branch)
- 3 Healers (temple, clinic, field hospital)
- 5 Guards (gate, patrol, captain, city watch, dungeon)
- 8 Trade NPCs (Blacksmith, Armorer, Tailor, Alchemist, Carpenter, Bowyer, Provisioner, Jeweler)
- 4 Class Trainers per region
- 3 Quest Givers
- 2 Innkeepers/Barkeepers

**Per city:** ~25-30 NPCs
**Total:** 250-300 NPCs

#### 2. More Talking Creatures
- 5 Ancient Dragons (Fire, Forest, Crystal, Void, Sand)
- 1 More Ancient Treant (Ironbark)
- 2 More Divine Avatars (Frost Father's Herald, Great Machinist's Construct)
- Underwater talking creatures

**Total:** ~10 more talking creatures

#### 3. Regional Trainers
For each of 25 classes, trainers in appropriate cities:
- Ice Mage Trainer (Frostholm)
- Druid Trainer (Verdantheart)
- Sorcerer Trainer (Emberforge)
- etc.

**Total:** 25 class trainers (can have multiples)

#### 4. Specialty NPCs
- Stable masters (10 cities)
- Ship captains (coastal cities)
- Dungeon guides
- Lore keepers
- Faction recruiters

**Total:** 50+ specialty NPCs

### Phase 3: LLM Integration

Once basic NPCs are working, integrate with LLM system:

1. **Replace keyword responses with LLM queries**
   - Pass player question to LLM
   - Include NPC personality and lore context
   - Use NPCKnowledgeSystem for role-based filtering

2. **Add memory system**
   - NPCs remember previous conversations
   - Track reputation with factions
   - Quest state awareness

3. **Dynamic dialogue generation**
   - Context-aware responses based on:
     - Time of day
     - Player's faction standing
     - Recent events in the world
     - Nearby NPCs and their relationships

### Phase 4: Quest Systems

Implement actual quest classes:
- Supply Line Quest (Quartermaster Grimwald)
- Ancient Texts Quest (Sage Theron)
- Dragon quests for talking dragons
- Sphinx riddle challenges
- Faction reputation quests

## Generator Script Expansion

To add more NPCs, modify `generate_npcs.py`:

### Add More Categories

```python
# City Guards
city_guards = [
    {
        "name": "Verdantheart Gate Guard",
        "body": "0x190",
        "hue": 2010,
        "location": "Verdantheart Main Gate",
        # ...
    },
    # ... more guards
]
```

### Add More Talking Creatures

```python
talking_creatures.append({
    "name": "Emberflame the Phoenix Lord",
    "type": "Phoenix Ancient",
    "body": "0xD",  # Phoenix body
    "hue": 1358,
    "location": "Volcanic Peak, Emberlands",
    "personality": "Fiery, passionate, reborn many times",
    "age": "Immortal (reborn 100+ times)",
    "lore": "Legendary phoenix who has died and been reborn countless times. Guardian of the eternal flame."
})
```

### Batch Generation

Create a loop to generate NPCs for all 10 cities:

```python
cities = [
    {"name": "Ironhaven", "hue": 2213, "region": "Ironclad"},
    {"name": "Frostholm", "hue": 1152, "region": "Frosthold"},
    # ... all 10 cities
]

for city in cities:
    # Generate banker
    generate_vendor_template(
        f"{city['name']} Banker",
        "0x190",
        city["hue"],
        "Banker",
        ["banking"],
        city["name"],
        "Professional banker"
    )

    # Generate healer
    # Generate guards
    # Generate merchants
    # etc.
```

## Performance Considerations

### Current NPCs: 13
- Minimal performance impact
- Can all be active simultaneously

### Target NPCs: 300-400
- Spawning strategy needed
- Not all NPCs active simultaneously
- City-based spawning (only spawn NPCs in cities where players are)
- Despawn inactive city NPCs after no players for 10 minutes

### LLM Query Performance
- Cache common responses
- Limit simultaneous LLM queries
- Queue system for dialogue generation
- Fallback to keyword responses during high load

## Summary

**Phase 1 Complete:**
- ✅ 13 NPCs generated and ready to test
- ✅ NPC generator tool created
- ✅ Spawn commands implemented
- ✅ All NPCs have proper serialization
- ✅ LLM integration placeholders ready

**Next Actions:**
1. Build ServUO and test the 13 NPCs
2. Verify dialogue responses work
3. Expand generator for city NPCs (Phase 2)
4. Implement LLM integration (Phase 3)
5. Create quest systems (Phase 4)

**Total Progress:**
- NPCs Created: 13/400+ (3%)
- Categories: 4/10 (Faction Leaders, Talking Creatures, Vendors, Quest Givers)
- Ready for: Testing and iteration
