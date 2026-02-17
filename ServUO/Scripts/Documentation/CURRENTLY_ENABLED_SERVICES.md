# Currently Enabled Services - ServUO Server

**Analysis Date:** 2025-01-XX  
**Expansion Level:** EJ (Endless Journey) - This enables AOS, SE, ML, SA, HS, TOL, EJ features

---

## ✅ ENABLED SERVICES (From Config Files)

### Core Game Systems
- **Chat System** - `Config/Chat.cfg`: `Enabled=True`
- **Champion System** - `Config/Champions.cfg`: `Enabled=true`
- **City Loyalty System** - `Config/CityLoyalty.cfg`: `Enabled=True`
- **ViceVsVirtue** - `Config/VvV.cfg`: `Enabled=True`
- **Veteran Rewards** - `Config/VetRewards.cfg`: `Enabled=True`
  - ⚠️ **Note:** `SkillCapRewards=False` (skill cap bonuses disabled - GOOD)
- **Ultima Store** - `Config/Store.cfg`: `Enabled=True`
- **Treasure Maps** - `Config/TreasureMaps.cfg`: `Enabled=True`
- **Daily Rares** - `Config/DailyRares.cfg`: `Enabled=True`
- **Honesty Virtue** - `Config/Honesty.cfg`: `Enabled=True`

### LLM Service
- **LLM Memory System** - `Config/LLMMemory.cfg`: `LLMMemory.MemorySystem.Enabled=true`
- **LLM Service** - `Config/LLM.cfg`: API key configured (OpenAI)
- **LLM Conversation Plugin** - Default: `Enabled=true` (hardcoded)

### Server Systems
- **AutoSave** - `Config/AutoSave.cfg`: `Enabled=True`

---

## ❌ DISABLED SERVICES (From Config Files)

- **TestCenter** - `Config/TestCenter.cfg`: `Enabled=False` ✅ **SAFE**
- **AutoRestart** - `Config/AutoRestart.cfg`: `Enabled=False`

---

## ✅ ENABLED BY DEFAULT (No Config File, Always Active)

### Always Enabled Services
- **Assistants** - Default: `false` (disabled by default, no config file found)
- **DisguisePersistence** - Always enabled (no config flag)
- **ItemFixes** - Always enabled (core fixes)
- **PreventInaccess** - Always enabled (staff utility)
- **Vendor Searching** - Always enabled (no config flag)
- **Quest System** - Always enabled (base framework)
- **Plant System** - Always enabled (core gameplay)
- **Craft System** - Always enabled (core gameplay)
- **Basket Weaving** - Always enabled
- **Armor Refinement** - Always enabled
- **Points System** - Always enabled (base framework)
- **Virtues System** - Always enabled (core gameplay, individual virtues configurable)

---

## ✅ ENABLED BY EXPANSION (Current: EJ)

Since `CurrentExpansion=EJ`, the following expansion-based services are **ENABLED**:

### Age of Shadows (AOS) Features
- **Paragon System** - Enabled (requires `Core.AOS`)

### Time of Legends (TOL) Features
- **Bulk Order System (New)** - Enabled (`NewSystemEnabled = Core.TOL`)

### Endless Journey (EJ) Features
- **Astronomy System** - Enabled (`Enabled = Core.EJ`)
- **Treasure Maps (New System)** - Enabled (`NewSystem = Core.EJ`)

---

## ⚠️ CONDITIONAL / DATE-BASED SERVICES

### Halloween Events (Currently INACTIVE)
- **TrickOrTreat** - Date-based: Oct 24 - Nov 15, **2012** ❌ **BROKEN** (dates in past)
- **PlayerZombies** - Date-based: Oct 24 - Nov 15, **2012** ❌ **BROKEN** (dates in past)
- **PumpkinPatch** - Date-based: Oct 24 - Nov 15, **2012** ❌ **BROKEN** (dates in past)

**Status:** These services will **NEVER** activate because dates are hardcoded to 2012.

---

## 📊 SUMMARY BY CATEGORY

### ✅ Production-Ready & Enabled
1. Chat System
2. Champion System
3. City Loyalty System
4. ViceVsVirtue
5. Veteran Rewards (skill cap bonuses disabled - good)
6. Ultima Store
7. Treasure Maps
8. Daily Rares
9. LLM Service (with OpenAI API configured)
10. Quest System
11. Plant System
12. Craft System
13. Bulk Order System (TOL)
14. Astronomy System (EJ)
15. Paragon System (AOS)
16. Virtues System
17. Vendor Searching
18. Points System

### ❌ Disabled (Good)
1. TestCenter ✅ **CRITICAL - Must stay disabled**

### ⚠️ Broken (Need Fixing)
1. Halloween Events (TrickOrTreat, PlayerZombies, PumpkinPatch) - Dates hardcoded to 2012

### ⚠️ Review Recommended
1. **Ultima Store** - Enabled, review items for pay-to-win concerns
2. **Daily Rares** - Enabled, monitor for world clutter
3. **LLM Service** - Enabled with OpenAI, monitor API costs
4. **Vendor Searching** - Always enabled, consider adding limitations

---

## 🔍 DETAILED CONFIGURATION STATUS

### TestCenter
```
Config/TestCenter.cfg: Enabled=False
Status: ✅ SAFE - Correctly disabled
```

### Chat System
```
Config/Chat.cfg: Enabled=True
AllowCreateChannels=True
Status: ✅ Active
```

### Champion System
```
Config/Champions.cfg: Enabled=true
PowerScrolls=6
StatScrolls=16
Status: ✅ Active
```

### City Loyalty
```
Config/CityLoyalty.cfg: Enabled=True
Status: ✅ Active
```

### ViceVsVirtue
```
Config/VvV.cfg: Enabled=True
EnhancedRules=False
Status: ✅ Active
```

### Veteran Rewards
```
Config/VetRewards.cfg: Enabled=True
SkillCapRewards=False  ← GOOD! Skill cap bonuses disabled
Status: ✅ Active (but skill cap bonuses correctly disabled)
```

### Ultima Store
```
Config/Store.cfg: Enabled=True
CostMultiplier=1.0
CurrencyImpl=Sovereigns
Status: ✅ Active - Review items carefully
```

### Daily Rares
```
Config/DailyRares.cfg: Enabled=True
Status: ✅ Active - Monitor for clutter
```

### LLM Service
```
Config/LLM.cfg: API key configured (OpenAI)
Config/LLMMemory.cfg: LLMMemory.MemorySystem.Enabled=true
Status: ✅ Active - Monitor API costs
```

### Treasure Maps
```
Config/TreasureMaps.cfg: Enabled=True
Status: ✅ Active
```

### Honesty Virtue
```
Config/Honesty.cfg: Enabled=True
TrammelGeneration=True
Status: ✅ Active
```

---

## 🎯 KEY FINDINGS

### ✅ Good News
1. **TestCenter is disabled** - Critical safety measure in place
2. **Veteran skill cap bonuses disabled** - Fair play maintained
3. Most core systems are active and functional

### ⚠️ Concerns
1. **Halloween events broken** - Need to fix dates in `HolidaySettings.cs`
2. **Ultima Store enabled** - Review items for balance
3. **LLM API costs** - Monitor usage (OpenAI API key configured)
4. **Daily Rares** - May accumulate over time

### 📝 Recommended Actions
1. Fix Halloween dates in `HolidaySettings.cs` (change 2012 to current year or relative dates)
2. Review Ultima Store items for pay-to-win concerns
3. Set up API usage monitoring for LLM service
4. Monitor DailyRares item accumulation
5. Consider adding limitations to Vendor Searching

---

## 🔧 HOW TO CHECK CURRENT STATUS

To verify what's actually running on your server:

1. **Check config files** in `Config/` directory
2. **Check server console** during startup for initialization messages
3. **Use admin commands** to test services (e.g., `[Chat`, `[Store`, etc.)
4. **Check expansion level** in `Config/Expansion.cfg` (currently: EJ)

---

**Note:** This analysis is based on config files and code defaults. Some services may have additional runtime checks or conditions that affect their actual activation status.

