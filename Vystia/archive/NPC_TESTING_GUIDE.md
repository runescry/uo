# NPC and LLM System Testing Guide

**Created:** 2025-12-08
**Purpose:** Test faction leader dialogue fixes and LLM system prompt corrections

---

## What Was Fixed

### 1. Faction Leader Dialogue (5 NPCs)
**Problem:** Leaders were speaking in third person from lore descriptions
**Fix:** Rewrote all dialogue to be first-person, regal, and concise

**Files Changed:**
- `ServUO/Scripts/Mobiles/Vystia/NPCs/FactionLeaders/EmperorGarrickSteelarm.cs`
- `ServUO/Scripts/Mobiles/Vystia/NPCs/FactionLeaders/ChieftainBjornFrostbeard.cs`
- `ServUO/Scripts/Mobiles/Vystia/NPCs/FactionLeaders/ElderSeraphinaLeafwhisper.cs`
- `ServUO/Scripts/Mobiles/Vystia/NPCs/FactionLeaders/SultanAziralRashid.cs`
- `ServUO/Scripts/Mobiles/Vystia/NPCs/FactionLeaders/ArchmagePyrusAshborn.cs`

### 2. LLM System Prompt Fix
**Problem:** NPCs thought they were in "Britannia" and used wrong character roles
**Fix:** Changed system prompt to say "Vystia, a custom world" instead of "Ultima Online"

**File Changed:**
- `ServUO/Scripts/Services/LLM/Services/LLMService.cs` (line 184)

**Key Changes:**
- "world of Ultima Online" → "Vystia, a custom world"
- Added: "Speak in first person as yourself, not in third person"

---

## Testing Steps

### Step 1: Stop and Rebuild Server
```bash
# Stop the running ServUO server (Ctrl+C in server console)
# Then rebuild:
cd C:\DevEnv\GIT\UO\ServUO
dotnet build
```

Expected result: 0 errors (warnings about unreachable code in vendors are harmless)

### Step 2: Start Server
```bash
# Start the server
cd C:\DevEnv\GIT\UO\ServUO
dotnet run
# Or double-click ServUO.exe
```

### Step 3: Test Faction Leaders

#### Emperor Garrick Steelarm (Ironclad Alliance)
```
[add EmperorGarrickSteelarm
```

**Test Dialogue:**
1. Say "hello" → Should respond: "Hail, {YourName}. I am Emperor Garrick Steelarm. What brings you before the Iron Throne?"
2. Say "faction" → Should respond: "The Ironclad Alliance unites technology and magic. Together, we shall bring prosperity to all Vystia."
3. Say "empire" → Should respond: "My empire leads Vystia into a new age. Steam and steel shall forge our destiny."

**What to Check:**
- ✅ Speaks in first person ("I am", "my empire")
- ✅ Regal, authoritative tone
- ✅ Short responses (no cutoff)
- ✅ Says "Vystia" not "Britannia"

#### Chieftain Bjorn Frostbeard (Polar Alliance)
```
[add ChieftainBjornFrostbeard
```

**Test Dialogue:**
1. Say "hello" → "Well met, {YourName}. I am Chieftain Bjorn Frostbeard. Speak your purpose."
2. Say "alliance" → "The Polar Alliance stands strong. My warriors guard the frozen north with honor and steel."
3. Say "frosthold" → "Frosthold breeds the mightiest warriors in Vystia. We fear neither frost giant nor dragon."

**What to Check:**
- ✅ Gruff warrior tone
- ✅ Honorable and direct
- ✅ First person

#### Elder Seraphina Leafwhisper (Sylvan Concord)
```
[add ElderSeraphinaLeafwhisper
```

**Test Dialogue:**
1. Say "hello" → "Blessings, child. I am Elder Seraphina Leafwhisper. The forest welcomes you."
2. Say "concord" → "The Sylvan Concord protects the sacred groves. Nature shall not bend to industry's greed."
3. Say "forest" → "For five centuries I have guided these lands. The Heart Tree shows me all that transpires."

**What to Check:**
- ✅ Ancient, wise tone
- ✅ Protective of nature
- ✅ Speaks as leader (500 years old)

#### Sultan Azir al-Rashid (League of Sands)
```
[add SultanAziralRashid
```

**Test Dialogue:**
1. Say "hello" → "Peace be upon you, {YourName}. I am Sultan Azir al-Rashid. How may I serve your interests?"
2. Say "league" → "The League of Sands prospers through trade, not war. Gold flows where swords cannot reach."
3. Say "trade" → "My caravans connect all of Vystia. Neutrality brings wealth that conflict destroys."

**What to Check:**
- ✅ Diplomatic, shrewd tone
- ✅ Merchant prince personality
- ✅ Neutral stance

#### Archmage Pyrus Ashborn (Ironclad Alliance - Fire Mage)
```
[add ArchmagePyrusAshborn
```

**Test Dialogue:**
1. Say "hello" → "You stand before Archmage Pyrus Ashborn, {YourName}. State your business."
2. Say "alliance" → "I stand with Emperor Garrick in the Ironclad Alliance. Fire and steel shall reshape Vystia."
3. Say "fire" → "My forges burn eternal, crafting weapons of legendary flame. None rival Emberlands' mastery."

**What to Check:**
- ✅ Powerful, ambitious tone
- ✅ Confident in abilities
- ✅ Speaks as mage leader

---

### Step 4: Test LLM Integration (Advanced)

**Prerequisites:**
- LLM service must be running (Python server or API configured)
- NPCs need to have LLM integration enabled

**Test with Emperor Garrick:**
```
[add EmperorGarrickSteelarm
```

Say something that triggers LLM (not a keyword):
- "What do you think about the war?"
- "Tell me about your empire"
- "Who are you?"

**What to Check:**
- ✅ Response says "Vystia" NOT "Britannia"
- ✅ Speaks as Emperor (not a merchant or generic NPC)
- ✅ First person ("I am Emperor Garrick")
- ✅ NO archaic English ("Prithee", "thee", "Verily")
- ✅ Brief (1-2 sentences)
- ✅ Uses Vystia lore (SimpleLoreSystem RAG should inject context)

**Example of GOOD response:**
> "I am Emperor Garrick Steelarm, ruler of the Ironclad Empire. My alliance with the archmages brings prosperity through technology and magic united."

**Example of BAD response (should NOT happen anymore):**
> "Prithee, I am Emperor Garrick Steelarm, a shrewd merchant traversing the lands of Britannia..."

---

## Expected Results

### Scripted Dialogue (Keywords)
All 5 faction leaders should:
- Speak in first person
- Use regal/authoritative tone matching personality
- Give short responses (under 100 chars, no cutoff)
- Mention "Vystia" or their faction/region
- Sound like leaders, not generic NPCs

### LLM Dialogue (Free-form questions)
NPCs with LLM integration should:
- Identify world as "Vystia" not "Britannia" or "Ultima Online"
- Speak as their character (Emperor, not merchant)
- Use first person
- Avoid archaic English unless appropriate for character
- Give brief responses (1-2 sentences per system prompt)
- Use Vystia lore from SimpleLoreSystem

---

## Troubleshooting

### Issue: NPCs still say "Britannia"
**Cause:** Server not restarted after LLMService.cs fix
**Fix:** Stop server, rebuild, restart

### Issue: Dialogue is third person
**Cause:** Old Scripts.dll still loaded
**Fix:** Stop server, delete Scripts.dll, rebuild, restart

### Issue: Responses getting cut off
**Cause:** Responses too long
**Fix:** Check NPC .cs files - all responses should be under 100 chars

### Issue: LLM not responding
**Cause:** LLM service not running or not configured
**Fix:** Check LLM service status, verify configuration

### Issue: Wrong personality in LLM responses
**Cause:** NPC lore entry has wrong personality description
**Fix:** Check `ServUO/Data/Lore/npc_domain.json` for correct personality field

---

## Build Status

**Last Build:** 2025-12-08
**Errors:** 0 (file lock errors due to running server - harmless)
**Warnings:** 15 unreachable code warnings in vendor NPCs (harmless)

**Files Compiled Successfully:**
- All 5 faction leader NPCs
- LLMService.cs with corrected system prompt
- All quest and vendor NPCs

---

## Additional Commands

### Quest Testing
```
[add QuartermasterGrimwald    # Quest giver (SupplyLineQuest)
[add CaptainValdrikSteelhart  # Quest target
```

### Vendor Testing
```
[add IronhavenBanker         # Banking services
[add FrostholmHealer         # Healing services
[add IronhavenGuardCaptain   # Guard captain
```

### Spawn All NPCs
```
[spawnvystia                 # Opens gump, navigate to NPCs or Vendors page
```

---

## Next Steps After Testing

1. **If tests pass:** Mark testing tasks as complete, move to NPC expansion (400+ NPCs)
2. **If issues found:** Document specific problems, fix, rebuild, retest
3. **Quest testing:** Test SupplyLineQuest delivery mechanics
4. **Vendor testing:** Test vendor inventories and LLM dialogue

---

*This guide documents the faction leader dialogue fixes and LLM system prompt corrections made on 2025-12-08.*
