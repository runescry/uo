# Faction Leader NPC Role and Personality Fix

**Date:** 2025-12-08
**Status:** ✅ COMPLETE - Build Successful
**Priority:** CRITICAL - Fixes LLM personality system

---

## Problem Summary

### Issue 1: Faction Leaders Identified as Merchants
**Symptoms:**
- Emperor Garrick responding as a "shrewd merchant"
- Using archaic English ("Prithee", "thee", "Verily")
- Saying they're in "Britannia" instead of "Vystia"
- Talking about "humble establishments" and "fine goods"

**Root Cause:**
The `InferRoleFromVendor` method in `NPCLocationDatabase.cs` had no detection for faction leaders, so it defaulted all unrecognized BaseVendor NPCs to `NPCRole.Merchant` (line 147).

### Issue 2: Faction Leaders Speaking in Third Person
**Symptoms:**
- "Co-founder of Ironclad Alliance who signed pact..." (lore description as dialogue)
- Not speaking AS the character, but ABOUT the character
- Responses too long and getting cut off

**Root Cause:**
The scripted keyword responses in faction leader NPC files were using lore descriptions verbatim instead of first-person dialogue.

---

## Complete Solution

### Fix 1: Added Faction Leader Roles to NPCRole Enum ✅

**File:** `ServUO/Scripts/Services/LLM/Data/NPCKnowledgeSystem.cs`
**Lines:** 37-43

**Added Roles:**
```csharp
// Vystia faction leaders
FactionLeader,   // General faction leader role
Emperor,         // Emperor Garrick Steelarm
Chieftain,       // Chieftain Bjorn Frostbeard
Elder,           // Elder Seraphina Leafwhisper
Sultan,          // Sultan Azir al-Rashid
Archmage         // Archmage Pyrus Ashborn
```

### Fix 2: Updated Role Inference Detection ✅

**File:** `ServUO/Scripts/Services/LLM/Data/NPCLocationDatabase.cs`
**Lines:** 123-171

**Added Detection Logic:**
```csharp
// Vystia faction leaders - check first for specific roles
if (vendorTypeName.Contains("emperor") || vendorName.Contains("emperor") || vendorTitle.Contains("emperor"))
    return NPCKnowledgeSystem.NPCRole.Emperor;

if (vendorTypeName.Contains("chieftain") || vendorName.Contains("chieftain") || vendorTitle.Contains("chieftain"))
    return NPCKnowledgeSystem.NPCRole.Chieftain;

if (vendorTypeName.Contains("elder") || (vendorName.Contains("elder") && (vendorTitle.Contains("leader") || vendorTitle.Contains("council"))))
    return NPCKnowledgeSystem.NPCRole.Elder;

if (vendorTypeName.Contains("sultan") || vendorName.Contains("sultan") || vendorTitle.Contains("sultan"))
    return NPCKnowledgeSystem.NPCRole.Sultan;

if (vendorTypeName.Contains("archmage") || vendorName.Contains("archmage") || vendorTitle.Contains("archmage"))
    return NPCKnowledgeSystem.NPCRole.Archmage;

// General faction leader check
if (vendorTitle.Contains("leader") || vendorTitle.Contains("lord") || vendorTitle.Contains("lady"))
    return NPCKnowledgeSystem.NPCRole.FactionLeader;
```

**Detection Method:**
- Checks NPC class name (e.g., "EmperorGarrickSteelarm" → Emperor)
- Checks NPC name (e.g., "Emperor Garrick Steelarm" → Emperor)
- Checks NPC title (e.g., "Emperor of the Ironclad Empire" → Emperor)

### Fix 3: Added Faction Leader Knowledge Domain ✅

**File:** `ServUO/Scripts/Services/LLM/Data/KnowledgeDomain.cs`
**Lines:** 117-125 (switch cases), 347-369 (domain initialization)

**Added Switch Cases:**
```csharp
// Vystia faction leaders - all use same domain (political/historical experts)
case NPCKnowledgeSystem.NPCRole.FactionLeader:
case NPCKnowledgeSystem.NPCRole.Emperor:
case NPCKnowledgeSystem.NPCRole.Chieftain:
case NPCKnowledgeSystem.NPCRole.Elder:
case NPCKnowledgeSystem.NPCRole.Sultan:
case NPCKnowledgeSystem.NPCRole.Archmage:
    InitializeFactionLeaderDomain();
    break;
```

**Knowledge Domain Expertise Levels:**

**EXPERT:**
- History (they make history)
- Geography (their territories)
- Law (they enforce/create it)
- Trade (economic policy)

**PROFICIENT:**
- Combat (military strategy)
- Magic (educated in magic)
- Monsters (threats to realm)
- Dungeons (realm knowledge)

**AWARE:**
- Religion (cultural knowledge)
- Healing (general knowledge)
- Crafting (economic oversight)
- Nature (environmental awareness)

**Result:** Faction leaders have comprehensive knowledge appropriate for rulers, not just merchant-level knowledge.

### Fix 4: Fixed Scripted Dialogue ✅

**Files:**
- `ServUO/Scripts/Mobiles/Vystia/NPCs/FactionLeaders/EmperorGarrickSteelarm.cs`
- `ServUO/Scripts/Mobiles/Vystia/NPCs/FactionLeaders/ChieftainBjornFrostbeard.cs`
- `ServUO/Scripts/Mobiles/Vystia/NPCs/FactionLeaders/ElderSeraphinaLeafwhisper.cs`
- `ServUO/Scripts/Mobiles/Vystia/NPCs/FactionLeaders/SultanAziralRashid.cs`
- `ServUO/Scripts/Mobiles/Vystia/NPCs/FactionLeaders/ArchmagePyrusAshborn.cs`

**Changes:**
- All dialogue rewritten to first person
- Regal/authoritative tone matching personality
- Responses under 100 characters (no cutoff)
- Added third keyword for regional/thematic topics

**Example (Emperor Garrick):**

BEFORE:
```csharp
Say("Co-founder of Ironclad Alliance who signed pact with Warlord Flamefist during siege of Ironhold. Believes technology and magic together will bring prosperity.");
```

AFTER:
```csharp
Say("The Ironclad Alliance unites technology and magic. Together, we shall bring prosperity to all Vystia.");
```

### Fix 5: Fixed LLM System Prompt ✅

**File:** `ServUO/Scripts/Services/LLM/Services/LLMService.cs`
**Line:** 184

**Changed From:**
```csharp
systemMessage.Append($"You are {npcName}, an NPC in the world of Ultima Online. {npcPersonality} Keep your responses brief (1-2 sentences) and in character. You are speaking directly to {playerName}.");
```

**Changed To:**
```csharp
systemMessage.Append($"You are {npcName}, an NPC in Vystia, a custom world. {npcPersonality} Keep your responses brief (1-2 sentences) and in character. Speak in first person as yourself, not in third person. You are speaking directly to {playerName}.");
```

**Key Changes:**
- "world of Ultima Online" → "Vystia, a custom world"
- Added explicit instruction: "Speak in first person as yourself, not in third person"

---

## Build Status

**Build:** ✅ SUCCESS
**Errors:** 0
**Warnings:** 15 (unreachable code in vendor NPCs - harmless)

**Build Command:**
```bash
cd C:\DevEnv\GIT\UO\ServUO
dotnet build
```

**All Changed Files Compiled Successfully:**
- `NPCKnowledgeSystem.cs` (added 6 faction leader roles)
- `NPCLocationDatabase.cs` (added detection logic)
- `KnowledgeDomain.cs` (added faction leader domain)
- `LLMService.cs` (fixed system prompt)
- All 5 faction leader NPC files (fixed dialogue)

---

## Testing Guide

### Before Testing: Restart Server

**CRITICAL:** The server must be stopped and restarted to load the new role system and LLM prompt changes.

```bash
# Stop server (Ctrl+C)
# Start server
cd C:\DevEnv\GIT\UO\ServUO
dotnet run
```

### Test 1: Verify Role Detection

Spawn a faction leader and check the server console:

```
[add EmperorGarrickSteelarm
```

**Expected Log Output:**
```
[NPCKnowledge] Building knowledge base for Emperor at Wind
[LLMConversationHelper] Emperor Garrick Steelarm (Emperor) knowledge base loaded: X entries
```

**BEFORE FIX:**
```
[NPCKnowledge] Building knowledge base for Merchant at Wind
[LLMConversationHelper] Emperor Garrick Steelarm (Merchant) knowledge base loaded: X entries
```

### Test 2: Verify Scripted Dialogue

Say keywords to test first-person responses:

**Test Commands:**
```
Say: "hello"
Expected: "Hail, {YourName}. I am Emperor Garrick Steelarm. What brings you before the Iron Throne?"

Say: "faction"
Expected: "The Ironclad Alliance unites technology and magic. Together, we shall bring prosperity to all Vystia."

Say: "empire"
Expected: "My empire leads Vystia into a new age. Steam and steel shall forge our destiny."
```

**What to Check:**
- ✅ First person ("I am", "my empire")
- ✅ Regal tone
- ✅ Short responses (no cutoff)
- ✅ Says "Vystia" not "Britannia"

### Test 3: Verify LLM Integration

Say something that triggers LLM (not a keyword):

```
Say: "Who are you?"
Say: "Tell me about yourself"
Say: "What do you think about the war?"
```

**Expected Response Characteristics:**
- ✅ Identifies as Emperor (not merchant)
- ✅ Says "Vystia" not "Britannia"
- ✅ First person ("I am Emperor Garrick Steelarm...")
- ✅ No archaic English ("Prithee", "thee", "dost thou")
- ✅ No merchant dialogue ("humble establishment", "fine goods", "best prices")
- ✅ Brief (1-2 sentences per system prompt)
- ✅ Appropriate personality (visionary leader, strategic, pragmatic)

**GOOD Response Example:**
```
"I am Emperor Garrick Steelarm, ruler of the Ironclad Empire. My alliance with the archmages brings prosperity through technology and magic united in Vystia."
```

**BAD Response Example (should NOT happen anymore):**
```
"Prithee, I am Emperor Garrick Steelarm, a shrewd merchant traversing the lands of Britannia. What fine goods dost thou seek?"
```

### Test All 5 Faction Leaders

| NPC | Command | Expected Role | Expected Personality |
|-----|---------|---------------|---------------------|
| Emperor Garrick | `[add EmperorGarrickSteelarm` | Emperor | Visionary leader, strategic, pragmatic |
| Chieftain Bjorn | `[add ChieftainBjornFrostbeard` | Chieftain | Warrior chieftain, gruff, honorable |
| Elder Seraphina | `[add ElderSeraphinaLeafwhisper` | Elder | Ancient elf, wise, protective of nature |
| Sultan Azir | `[add SultanAziralRashid` | Sultan | Shrewd merchant prince, diplomatic |
| Archmage Pyrus | `[add ArchmagePyrusAshborn` | Archmage | Powerful fire mage, ambitious |

---

## Technical Details

### How the Role System Works

1. **NPC Spawns** → Server creates NPC instance
2. **Role Inference** → `NPCLocationDatabase.InferRoleFromVendor()` checks:
   - Class name (`EmperorGarrickSteelarm` → contains "emperor")
   - NPC name (`"Emperor Garrick Steelarm"` → contains "emperor")
   - NPC title (`"Emperor of the Ironclad Empire"` → contains "emperor")
3. **Role Assigned** → Returns `NPCRole.Emperor`
4. **Knowledge Domain Loaded** → `InitializeFactionLeaderDomain()` sets expertise levels
5. **Conversation Starts** → LLM system uses:
   - Emperor role (not Merchant)
   - Faction leader knowledge domain (comprehensive, not limited)
   - Vystia-specific system prompt
   - NPC personality from lore

### Knowledge Domain Impact

**Before Fix (Merchant Role):**
- Limited knowledge (trade-focused)
- No historical expertise
- No political knowledge
- Generic merchant personality

**After Fix (Emperor Role):**
- EXPERT in history, geography, law, trade
- PROFICIENT in combat, magic, monsters, dungeons
- AWARE in religion, healing, crafting, nature
- Appropriate leader personality

### System Prompt Impact

**Before Fix:**
```
"You are Emperor Garrick Steelarm, an NPC in the world of Ultima Online..."
```
→ LLM thinks: "I'm in Ultima Online/Britannia"

**After Fix:**
```
"You are Emperor Garrick Steelarm, an NPC in Vystia, a custom world. Speak in first person as yourself, not in third person..."
```
→ LLM thinks: "I'm in Vystia, I should speak as Emperor in first person"

---

## Files Changed

### Core LLM System Files (3 files)
1. `ServUO/Scripts/Services/LLM/Data/NPCKnowledgeSystem.cs`
   - Added 6 faction leader roles to NPCRole enum (lines 37-43)

2. `ServUO/Scripts/Services/LLM/Data/NPCLocationDatabase.cs`
   - Added faction leader detection to InferRoleFromVendor (lines 129-148)
   - Now checks class name, NPC name, and title for role keywords

3. `ServUO/Scripts/Services/LLM/Data/KnowledgeDomain.cs`
   - Added switch cases for faction leader roles (lines 117-125)
   - Created InitializeFactionLeaderDomain method (lines 347-369)
   - Set comprehensive knowledge expertise for leaders

### LLM Service File (1 file)
4. `ServUO/Scripts/Services/LLM/Services/LLMService.cs`
   - Fixed system prompt to say "Vystia" instead of "Ultima Online" (line 184)
   - Added explicit first-person instruction

### Faction Leader NPC Files (5 files)
5. `ServUO/Scripts/Mobiles/Vystia/NPCs/FactionLeaders/EmperorGarrickSteelarm.cs`
6. `ServUO/Scripts/Mobiles/Vystia/NPCs/FactionLeaders/ChieftainBjornFrostbeard.cs`
7. `ServUO/Scripts/Mobiles/Vystia/NPCs/FactionLeaders/ElderSeraphinaLeafwhisper.cs`
8. `ServUO/Scripts/Mobiles/Vystia/NPCs/FactionLeaders/SultanAziralRashid.cs`
9. `ServUO/Scripts/Mobiles/Vystia/NPCs/FactionLeaders/ArchmagePyrusAshborn.cs`
   - All rewrote dialogue to first person, regal tone, under 100 chars

**Total Files Changed:** 9
**Lines Added:** ~150
**Lines Modified:** ~50

---

## Next Steps

1. **Restart Server** - REQUIRED to load new role system
2. **Test All 5 Faction Leaders** - Verify role detection and personality
3. **Verify LLM Responses** - Ensure no more "Britannia" or merchant dialogue
4. **Document Results** - Update project status with test results
5. **Expand to Other NPCs** - Apply same role detection to quest givers, vendors, etc.

---

## Additional Notes

### Why This Fix Was Critical

The LLM personality system is a core feature of the Vystia shard. Faction leaders are the most important NPCs in the world - they represent major political powers and drive the narrative. Having them speak as generic merchants completely broke immersion and made the LLM system look broken.

This fix ensures:
- Faction leaders have appropriate knowledge and personality
- The custom Vystia world is properly identified to the LLM
- All NPCs can be properly categorized by role for future expansion

### Future Improvements

1. **Add More Specific Roles:**
   - QuestMaster, GuildLeader, Artisan, Diplomat, General, Admiral, etc.

2. **Role-Specific Personalities:**
   - Each role could have personality templates in npc_domain.json
   - Emperors default to "commanding, strategic"
   - Merchants default to "friendly, profit-oriented"
   - Etc.

3. **Dynamic Knowledge Updates:**
   - Faction leaders could learn about recent events
   - Knowledge could expand based on player interactions

4. **Faction-Specific Knowledge:**
   - Emperor Garrick knows more about Ironclad territories
   - Chieftain Bjorn knows more about Frosthold
   - Etc.

---

*This document summarizes all fixes applied on 2025-12-08 to resolve the faction leader personality and role detection issues.*
