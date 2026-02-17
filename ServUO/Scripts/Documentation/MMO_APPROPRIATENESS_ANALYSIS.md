# MMO Appropriateness Analysis - ServUO Services

This document identifies services and features that are inappropriate, problematic, or need attention for a serious, engaging MMO production environment.

**Analysis Date:** 2025-01-XX

---

## CRITICAL ISSUES - Must Disable/Remove for Production

### 1. TestCenter Service
**Location:** `ServUO/Scripts/Services/TestCenter.cs`

**Severity:** CRITICAL - Game Breaking

**Issues:**
- Allows players to set stats (STR/DEX/INT) to any value via speech commands (`set str 125`)
- Allows players to set any skill to any value (`set [skillname] [value]`)
- Fills player banks with unlimited test items including:
  - Bank checks worth 500,000+ gold
  - All artifacts in the game
  - Power scrolls (skill cap increases)
  - Stat scrolls
  - Unlimited crafting materials
  - All ethereal mounts
  - Complete spellbooks
  - Unlimited reagents

**Impact:**
- Completely destroys game economy
- Eliminates all progression and achievement
- Makes the game meaningless - players can instantly max everything
- Creates massive inflation if enabled even briefly

**Recommendation:**
- **DISABLE BY DEFAULT** (currently default: false, which is correct)
- Consider removing entirely or moving to admin-only commands
- If kept, restrict to specific test accounts only
- Never enable on production servers

**Current Status:** Default disabled, but still dangerous if accidentally enabled

---

### 2. Hardcoded Halloween Dates (2012)
**Location:** `ServUO/Scripts/Services/HolidaySettings.cs`

**Severity:** HIGH - Breaks Immersion

**Issues:**
- Halloween dates hardcoded to October 24 - November 15, **2012**
- Events will never trigger again (dates are in the past)
- Multiple services depend on these dates:
  - `TrickOrTreat`
  - `PlayerZombies` (HalloweenHauntings)
  - `PumpkinPatch`

**Impact:**
- Seasonal events are permanently broken
- Players cannot experience Halloween content
- Code appears abandoned/outdated

**Recommendation:**
- Update dates to current year or make them relative (e.g., always Oct 24 - Nov 15 of current year)
- Consider making dates configurable via config file
- Test all Halloween services after date fix

---

## HIGH PRIORITY ISSUES - Review and Adjust

### 3. Veteran Rewards - Skill Cap Bonuses
**Location:** `ServUO/Scripts/Services/VeteranRewards/RewardSystem.cs`

**Severity:** HIGH - Pay-to-Win / Unfair Advantage

**Issues:**
- Gives skill cap bonuses (default: +200 per level, up to 4 levels = +800 total)
- Based on account age, not gameplay achievement
- Creates permanent advantage for older accounts
- Default enabled with `SkillCapRewards = true`

**Impact:**
- New players can never catch up to veteran players in skill caps
- Creates two-tier player base
- Rewards account age over skill/achievement
- May violate fair play principles

**Recommendation:**
- Consider disabling skill cap bonuses (`VetRewards.SkillCapRewards=false`)
- If kept, make bonuses cosmetic only (titles, mounts, decorations)
- Consider making bonuses achievable through gameplay instead
- Document clearly if this is intentional design choice

**Configuration:**
```
VetRewards.SkillCapRewards=false  # Disable skill cap bonuses
```

---

### 4. Ultima Store - Premium Currency
**Location:** `ServUO/Scripts/Services/UltimaStore/`

**Severity:** MEDIUM-HIGH - Potential Pay-to-Win

**Issues:**
- Implements premium currency store (Sovereigns)
- Default enabled (`Store.Enabled=true`)
- Can sell powerful items, mounts, character customization
- Cost multiplier can be adjusted (default: 1.0)

**Impact:**
- If not carefully managed, can become pay-to-win
- May create advantage for paying players
- Needs clear policy on what can be purchased

**Recommendation:**
- Review all store items for game balance
- Ensure no gameplay advantages can be purchased
- Consider cosmetic-only items
- Document store policy clearly
- Monitor for pay-to-win concerns

**Configuration:**
```
Store.Enabled=true  # Currently enabled - review items carefully
Store.CostMultiplier=1.0  # Adjust pricing if needed
```

---

### 5. DailyRares - Non-Decaying Items
**Location:** `ServUO/Scripts/Services/DailyRares.cs`

**Severity:** MEDIUM - World Clutter

**Issues:**
- Spawns items with `LastMoved` set far in future (prevents decay)
- Items never disappear naturally
- Default enabled (`DailyRares.Enabled=true`)
- Spawns on multiple maps (Felucca, Trammel, Ilshenar, Malas)

**Impact:**
- Items accumulate in world over time
- Can cause world clutter if not managed
- May impact server performance with many items

**Recommendation:**
- Consider making items decay normally
- Or implement periodic cleanup
- Or reduce spawn frequency
- Monitor item counts over time

**Configuration:**
```
DailyRares.Enabled=true  # Review if causing clutter
```

---

## MEDIUM PRIORITY ISSUES - Consider Adjustments

### 6. Vendor Searching - Reduced Player Interaction
**Location:** `ServUO/Scripts/Services/Vendor Searching/`

**Severity:** MEDIUM - Gameplay Impact

**Issues:**
- Allows searching all player vendors from one interface
- Can exclude Felucca (PvP map)
- Always enabled (no config flag)

**Impact:**
- Reduces need to explore and visit different towns
- May reduce player-to-player interaction
- Makes finding items too easy
- Could hurt vendor economy if too powerful

**Recommendation:**
- Consider adding search limitations (cooldown, cost, range)
- Or make it a premium feature
- Or require players to visit vendors first to "discover" them
- Balance convenience vs. exploration

---

### 7. PreventInaccess - Staff Only
**Location:** `ServUO/Scripts/Services/PreventInaccess.cs`

**Severity:** LOW - Staff Utility

**Issues:**
- Only affects staff (non-player access levels)
- Hardcoded to `Enabled = true`
- Moves staff to safe locations on crash

**Impact:**
- No player impact
- Useful for staff operations
- No changes needed

**Recommendation:**
- Keep as-is (staff utility only)

---

### 8. LLM Service - Cost and Performance
**Location:** `ServUO/Scripts/Services/LLM/`

**Severity:** MEDIUM - Operational Concerns

**Issues:**
- Requires OpenAI API (paid) or local Ollama setup
- API calls cost money (OpenAI) or server resources (Ollama)
- Default enabled but requires configuration
- Response caching helps but may still be expensive at scale

**Impact:**
- Can be expensive to run with many players
- May have performance issues with high NPC count
- Requires ongoing maintenance and monitoring

**Recommendation:**
- Monitor API costs closely
- Set up usage limits/alerts
- Consider disabling for low-population servers
- Use local Ollama for cost control
- Implement aggressive caching
- Consider rate limiting per player

**Configuration:**
```
LLMConversationPlugin.Enabled=true  # Review costs before enabling
```

---

## DESIGN CONCERNS - Review Gameplay Balance

### 9. Paragon System - Artifact Drops
**Location:** `ServUO/Scripts/Services/Paragon.cs`

**Severity:** LOW-MEDIUM - Balance Concern

**Issues:**
- Paragons can drop artifacts (rare items)
- Artifact chance based on fame and player luck
- Only on specific maps (default: Ilshenar)

**Impact:**
- May create artifact inflation if paragons spawn frequently
- Could devalue rare items

**Recommendation:**
- Monitor artifact drop rates
- Adjust `ChestChance` and artifact drop rates if needed
- Consider limiting paragon spawn frequency

**Configuration:**
```
Paragon.ChestChance=0.10  # 10% chance - review if too high
```

---

### 10. Champion System - Power Scroll Rewards
**Location:** `ServUO/Scripts/Services/ChampionSystem/`

**Severity:** LOW-MEDIUM - Balance Concern

**Issues:**
- Awards power scrolls (skill cap increases) on champion defeat
- Default: 6 power scrolls per champion
- Default: 16 stat scrolls per champion
- Scroll chance: 0.1% (very low, but still exists)

**Impact:**
- Power scrolls are very valuable
- May create skill cap inflation over time
- Could devalue skill progression

**Recommendation:**
- Monitor power scroll distribution
- Consider reducing scroll amounts if inflation occurs
- Ensure champions are appropriately difficult

**Configuration:**
```
Champions.PowerScrolls=6  # Review if causing inflation
Champions.StatScrolls=16   # Review if causing inflation
Champions.ScrollChance=0.1 # Very low chance - probably fine
```

---

## SUMMARY OF RECOMMENDATIONS

### Immediate Actions (Before Production):
1. ✅ **Verify TestCenter is disabled** - Check `TestCenter.Enabled=false` in config
2. ✅ **Fix Halloween dates** - Update `HolidaySettings.cs` to use current year or relative dates
3. ⚠️ **Review Veteran Rewards** - Consider disabling skill cap bonuses
4. ⚠️ **Review Ultima Store items** - Ensure no pay-to-win items
5. ⚠️ **Review Vendor Searching** - Consider adding limitations

### Configuration Checklist:
```
# Critical - Must be false
TestCenter.Enabled=false

# Review these settings
VetRewards.SkillCapRewards=false  # Consider disabling
Store.Enabled=true                 # Review items carefully
DailyRares.Enabled=true            # Monitor for clutter
LLMConversationPlugin.Enabled=true # Monitor costs
```

### Code Changes Needed:
1. **HolidaySettings.cs** - Update dates from 2012 to current year or relative
2. **TestCenter.cs** - Consider adding access level check (admin only)
3. **VendorSearch.cs** - Consider adding usage limitations

### Monitoring Required:
- TestCenter usage (should be zero)
- Veteran reward skill cap distribution
- Store purchases and balance impact
- LLM API costs and performance
- DailyRares item accumulation
- Power scroll distribution from champions

---

## Services That Are Appropriate

These services are well-designed for production MMO use:

✅ **Chat System** - Standard MMO feature
✅ **Bulk Order System** - Encourages crafting engagement
✅ **Quest System** - Core gameplay mechanic
✅ **Plant System** - Engaging resource gathering
✅ **City Loyalty System** - Adds depth and player investment
✅ **Virtues System** - Classic UO feature, encourages roleplay
✅ **ViceVsVirtue** - PvP content with clear rules
✅ **Champion System** - Group content (review rewards only)
✅ **Craft System** - Essential gameplay
✅ **Astronomy System** - Optional exploration content
✅ **DisguisePersistence** - Quality of life feature
✅ **ItemFixes** - Necessary bug fixes
✅ **PreventInaccess** - Staff utility only

---

## Conclusion

The main concerns for a serious MMO are:

1. **TestCenter** - Must never be enabled (currently safe, default disabled)
2. **Halloween dates** - Broken and need fixing
3. **Veteran skill cap bonuses** - Creates unfair advantage
4. **Ultima Store** - Needs careful item curation
5. **Vendor Searching** - May reduce exploration (design choice)

Most other services are appropriate for production use, though some may need balance tuning based on your server's specific goals and player base.

