# NPC and LLM System Testing Guide

**Created:** 2025-12-08  
**Last Updated:** 2025-01-02
**Purpose:** Test faction leader dialogue, LLM integration, service fees, and faction reputation systems

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

**Files Changed:**
- `ServUO/Scripts/Services/LLM/Services/LLMService.cs` (lines 181, 204)
- `ServUO/Scripts/Services/LLM/Data/NPCPersonalities.cs` (GetContextualInfo method)

**Key Changes:**
- "world of Ultima Online" → "Vystia, a custom world" (in system prompt)
- Added: "Speak in first person as yourself, not in third person"
- **NEW (2025-12-13):** Location context now replaces "Britannia" with "Vystia" for Vystia NPCs

---

## Testing Steps

### Step 1: Stop and Rebuild Server
```bash
# Stop the running ServUO server (Ctrl+C in server console)
# Then rebuild:
cd D:\UO\ServUO
dotnet build
```

Expected result: 0 errors (warnings about unreachable code in vendors are harmless)

### Step 2: Start Server
```bash
# Start the server
cd D:\UO\ServUO
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
**Cause:** Server not restarted after fixes, or NPC is not detected as Vystia NPC
**Fix:** 
1. Stop server, rebuild, restart
2. Verify NPC is in a Vystia namespace (e.g., `Server.Mobiles.Vystia`)
3. The location context fix in NPCPersonalities.cs should automatically replace "Britannia" with "Vystia" for Vystia NPCs

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

**Last Build:** 2025-12-13
**Errors:** 0 (file lock errors due to running server - harmless)
**Warnings:** 15 unreachable code warnings in vendor NPCs (harmless)

**Files Compiled Successfully:**
- All 5 faction leader NPCs
- LLMService.cs with corrected system prompt (lines 181, 204)
- NPCPersonalities.cs with location context fix (replaces "Britannia" → "Vystia" for Vystia NPCs)
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

## Service Fee Systems (NEW - 2025-01-02)

### Resurrection Service Fees

**Cost:** 50 gold per resurrection

**Testing Steps:**
1. Die or use `[Kill self` to kill your character
2. Find a healer NPC (e.g., `[add FrostholmHealer`)
3. Attempt resurrection service
4. Verify 50g is deducted from bank/backpack
5. Verify message shows "Resurrection: 50 gold"

**What to Check:**
- ✅ Gold is properly deducted
- ✅ Resurrection works after payment
- ✅ No resurrection if insufficient gold
- ✅ Proper message displayed

### Moongate Travel Fees

**Costs:**
| Destination Type | Cost |
|-----------------|------|
| Same Region | 100g |
| Adjacent Region | 150g |
| Cross-Continent | 250g |

**Testing Steps:**
1. Use a moongate in any Vystia region
2. Select a destination
3. Verify correct gold amount deducted
4. Verify travel occurs after payment

**What to Check:**
- ✅ Correct fee based on distance
- ✅ Gold deducted before travel
- ✅ Travel blocked if insufficient gold
- ✅ Fee message displayed

---

## Faction Reputation System (NEW - 2025-01-02)

### Overview

7 factions with reputation tiers from Hostile (-3000) to Exalted (15000+)

**Factions:**
1. Frostguard (Frosthold) - Enemy: Flame Legion
2. Flame Legion (Emberlands) - Enemy: Frostguard
3. Greenward (Verdantpeak) - Enemy: Voidborn
4. Arcane Conclave (Crystal Barrens) - Enemy: Technoguild
5. Technoguild (Ironclad) - Enemy: Arcane Conclave
6. Sandwalkers (Desert) - No enemy
7. Voidborn (ShadowVoid) - Enemy: Greenward

### Reputation Tiers

| Tier | Rep Range | Vendor Discount |
|------|-----------|-----------------|
| Hostile | < -1000 | 0% |
| Unfriendly | -1000 to 0 | 0% |
| Neutral | 0 to 3000 | 0% |
| Friendly | 3000 to 6000 | 5% |
| Honored | 6000 to 12000 | 8% |
| Revered | 12000 to 15000 | 12% |
| Exalted | 15000+ | 15% |

### Testing Commands

```
[Factions             - Show all faction standings
[Faction <1-7>        - Detailed info on one faction
[SetReputation <1-7> <amount>  - Set exact reputation (GM)
[AddReputation <1-7> <amount>  - Add/remove reputation (GM)
[DonateFaction <1-7> <gold>    - Donate gold for reputation
```

### Testing Procedure

**Test 1: View Standings**
```
[Factions
```
Expected: See all 7 factions with current reputation and tier

**Test 2: Set Reputation**
```
[SetReputation 1 5000
[Factions
```
Expected: Frostguard shows 5000 (Friendly)

**Test 3: Vendor Discount**
```
[SetReputation 1 15000
```
Then buy from a Frostguard faction vendor
Expected: 15% discount applied

**Test 4: Enemy Faction Penalty**
```
[SetReputation 1 0
[AddReputation 1 1000
[Factions
```
Expected: Frostguard +1000, Flame Legion -500 (enemy penalty)

**Test 5: Donations**
```
[DonateFaction 1 5000
```
Expected: +250 reputation (50 per 1000g), gold deducted from bank

---

## Religion/Piety System (NEW - 2025-01-02)

### Overview

6 religions with piety tracking (0-1000)

**Religions:**
1. Frosthelm Faith (Frosthold)
2. Surya's Sandscript (Emberlands)
3. Lunara's Covenant (Verdantpeak)
4. Celestis Arcanum (Crystal Barrens)
5. Oceana's Covenant (ShadowVoid)
6. Cogsmith Creed (Ironclad)

### Piety Tiers

| Tier | Piety | Unlocks |
|------|-------|---------|
| None | 0-49 | Nothing |
| Initiate | 50-199 | 1st passive bonus |
| Devoted | 200-499 | 2nd passive + 1st devotion power |
| Faithful | 500-899 | 2nd devotion power |
| Exalted | 900-1000 | 3rd devotion power |

### Testing Commands

```
[Religion              - Show current religion status
[SetReligion <1-6>     - Convert to religion (GM)
[SetPiety <amount>     - Set exact piety (GM)
[AddPiety <amount>     - Add/remove piety (GM)
[Pray                  - Daily prayer (+10 piety)
[Tithe <gold>          - Donate gold (+1 per 100g)
```

### Testing Procedure

**Test 1: Convert to Religion**
```
[SetReligion 1
[Religion
```
Expected: Shows "Frosthelm Faith" membership

**Test 2: Gain Piety**
```
[Pray
```
Expected: +10 piety, message shown

**Test 3: Tithe Gold**
```
[Tithe 1000
```
Expected: +10 piety (1 per 100g), 1000g deducted

**Test 4: Tier Progression**
```
[SetPiety 200
[Religion
```
Expected: Shows "Devoted" tier with unlocked bonuses

---

## Pet System Testing (NEW - 2026-01-02)

### Overview

4 pet classes with unique pet types:
- **Summoner:** Elemental pets
- **Necromancer:** Undead minions
- **Beastmaster:** Tamed animals
- **Artificer:** Mechanical constructs

### Pet Commands

```
[SummonPet <type>    - Summon a pet by type
[SP <type>           - Shortcut
[DismissPets         - Dismiss all active pets
[DP                  - Shortcut
[PetInfo             - Show current pet information
[PI                  - Shortcut
[PetList <class>     - List available pets for a class
[PL <class>          - Shortcut
```

### Testing Procedure

**Test 1: Summon Pet**
```
[SetClassV2 Summoner
[SummonPet WaterElemental
```
Expected: Water Elemental appears and follows you

**Test 2: Pet Limit**
```
[SummonPet WaterElemental
[SummonPet FireElemental
[SummonPet EarthElemental
[SummonPet AirElemental
[SummonPet WaterElemental   # Should fail - at limit
```
Expected: 4th pet summons, 5th pet fails with "pet limit reached"

**Test 3: Dismiss Pets**
```
[DismissPets
```
Expected: All pets vanish, confirmation message

**Test 4: Pet Info**
```
[SummonPet WaterElemental
[PetInfo
```
Expected: Shows pet stats, health, owner info

**What to Check:**
- ✅ Pets scale with owner's skills
- ✅ Pet limits enforced per class
- ✅ Pets follow and attack on command
- ✅ Pets persist across sessions (serialization)
- ✅ Wrong class cannot summon class-specific pets

---

## Housing System Testing (NEW - 2026-01-02)

### Overview

5 house sizes with purchase prices and weekly taxes

| Size | Dimensions | Purchase | Weekly Tax |
|------|------------|----------|------------|
| Small | 7×7 | 50,000g | 500g |
| Medium | 11×11 | 150,000g | 1,500g |
| Large | 15×15 | 400,000g | 4,000g |
| Keep | 18×18 | 1,000,000g | 10,000g |
| Castle | 31×31 | 3,000,000g | 30,000g |

### Housing Commands

```
[HouseCosts           - Display all housing costs
[HC                   - Shortcut
[TaxInfo              - Show your tax status
[TI                   - Shortcut
[PayTax               - Pay outstanding taxes
[PT                   - Shortcut
[SetHousePrice <amt>  - Set price of targeted house (GM)
[SHP <amt>            - Shortcut
[HouseInfo            - Display house info (GM)
[HI                   - Shortcut
[TaxExempt            - Toggle tax exemption (GM)
[TE                   - Shortcut
[ForceTaxCollection   - Force tax collection cycle (GM)
[FTC                  - Shortcut
[TaxStatus            - Server-wide tax stats (GM)
[TS                   - Shortcut
```

### Testing Procedure

**Test 1: View Costs**
```
[HouseCosts
```
Expected: See all 5 sizes with purchase prices and weekly taxes

**Test 2: House Purchase**
1. Use house placement tool
2. Place a small house (7x7)
3. Verify 50,000g is charged (not default UO price)

**Test 3: Tax Info**
```
[TaxInfo
```
Expected: Shows current tax debt, due date, grace period

**Test 4: Force Tax Collection**
```
[ForceTaxCollection
```
Expected: Triggers immediate tax collection cycle
- Houses with funds: Tax deducted from bank
- Houses without funds: Grace period starts/continues

**Test 5: Tax Exemption**
1. Target your house
2. Use `[TaxExempt`
3. Use `[HouseInfo` to verify exemption

**What to Check:**
- ✅ Correct Vystia prices (not default UO)
- ✅ Taxes auto-deduct weekly from bank
- ✅ 7-day grace period before condemnation
- ✅ Tax exemption toggle works
- ✅ Multiple houses tracked separately

---

## Zone Control Testing (NEW - 2026-01-02)

### Overview

4 zone types with different PvP and loot rules

| Zone | PvP | Death Penalty | Loot Drop | XP/Gold |
|------|-----|---------------|-----------|---------|
| Sanctuary (Green) | No | None | 0% | 0.75x |
| Contested (Yellow) | Consent | 5% skill loss | 10% | 1.0x |
| Lawless (Red) | Always | 10% skill loss | 25% | 1.25x |
| Extreme (Black) | Always | 15% skill loss | 50% | 1.5x |

### Zone Commands

```
[ZoneInfo            - Show current zone type and rules
[ZI                  - Shortcut
[TogglePvP           - Toggle PvP consent (Contested only)
[SetZone <type>      - Set zone type (GM)
[SZ <type>           - Shortcut
[ZoneList            - List all zones (GM)
[ZL                  - Shortcut
[CreateZone <name> <type>  - Create zone (GM)
[DeleteZone <name>   - Delete zone (GM)
```

### Testing Procedure

**Test 1: View Zone Info**
```
[ZoneInfo
```
Expected: Shows current zone type, PvP rules, bonuses

**Test 2: PvP Consent in Contested**
1. Enter a Contested (yellow) zone
2. Use `[TogglePvP`
3. Attack another player with PvP toggled
Expected: PvP damage occurs
4. Attack a player without PvP toggled
Expected: Attack blocked

**Test 3: Create Zone**
```
[CreateZone TestZone Lawless
[ZoneInfo
```
Expected: New Lawless zone created at location

**Test 4: Zone Bonuses**
1. Kill a creature in Sanctuary zone
2. Note gold drop
3. Kill same creature type in Extreme zone
4. Compare gold drops
Expected: Extreme zone gives ~2x gold (1.5x multiplier)

**Test 5: Death Penalty**
1. Enter Lawless zone
2. Die to a creature
3. Check skills after resurrection
Expected: 10% skill loss (each skill reduced by 10%)

**What to Check:**
- ✅ Correct zone type displayed
- ✅ PvP consent works in Contested
- ✅ Always-on PvP in Lawless/Extreme
- ✅ Death penalties applied correctly
- ✅ XP/gold multipliers working
- ✅ Guards only respond in Sanctuary

---

*This guide documents the faction leader dialogue fixes, LLM system prompt corrections, service fees, faction reputation, religion systems, pet system, housing system, and zone control. Last updated 2026-01-02.*

