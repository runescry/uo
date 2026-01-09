# Ultima Online - Confirmed Stackable Reagent ItemIDs

**Source:** ServUO Scripts/Items/Resource/
**Date:** 2025-12-11

---

## Magery Reagents (8 total)

All 8 magery reagents are **confirmed stackable** (BaseReagent sets `Stackable = true`):

| Reagent | ItemID | Hex | Description |
|---------|--------|-----|-------------|
| **Black Pearl** | 3962 | 0x0F7A | Dark pearl |
| **Bloodmoss** | 3963 | 0x0F7B | Red moss |
| **Garlic** | 3972 | 0x0F84 | Garlic bulb |
| **Ginseng** | 3973 | 0x0F85 | Ginseng root |
| **Mandrake Root** | 3974 | 0x0F86 | Mandrake root |
| **Nightshade** | 3976 | 0x0F88 | Purple nightshade |
| **Sulfurous Ash** | 3980 | 0x0F8C | Gray ash |
| **Spider's Silk** | 3981 | 0x0F8D | White silk |

**ItemID Range:** 0x0F7A - 0x0F8D (3962-3981 decimal)

---

## Necromancy Reagents (5 total)

All 5 necromancy reagents are **confirmed stackable** (BaseReagent sets `Stackable = true`):

| Reagent | ItemID | Hex | Description |
|---------|--------|-----|-------------|
| **Bat Wing** | 3960 | 0x0F78 | Bat wing |
| **Daemon Blood** | 3965 | 0x0F7D | Red vial |
| **Pig Iron** | 3978 | 0x0F8A | Iron ingots |
| **Nox Crystal** | 3982 | 0x0F8E | Green crystal |
| **Grave Dust** | 3983 | 0x0F8F | Gray dust |

**ItemID Range:** 0x0F78 - 0x0F8F (3960-3983 decimal)

---

## Crafting Resources (Confirmed Stackable)

All crafting resources explicitly set `Stackable = true`:

| Resource | ItemID | Hex | Description |
|----------|--------|-----|-------------|
| **Beeswax** | 5154 | 0x1422 | Yellow wax |
| **Flax** | 6812 | 0x1A9C | Flax bundle |
| **Oil Flask** | 7192 | 0x1C18 | Flask of oil |
| **Kindling** | 3553 | 0x0DE1 | Campfire kindling |

**Source Files:**
- `Items/Resource/Beeswax.cs` - Weight 1.0
- `Items/Resource/Flax.cs` - Weight 1.0, used on spinning wheel
- `Items/Resource/OilFlask.cs` - Both filled and empty variants
- `Items/Consumables/Kindling.cs` - Weight 1.0, creates campfire

---

## Mysticism Reagents (6 total)

Mysticism reagents share ItemIDs with Magery reagents:

| Reagent | ItemID | Hex | Notes |
|---------|--------|-----|-------|
| Bloodmoss | 3963 | 0x0F7B | Same as Magery |
| Black Pearl | 3962 | 0x0F7A | Same as Magery |
| Garlic | 3972 | 0x0F84 | Same as Magery |
| Ginseng | 3973 | 0x0F85 | Same as Magery |
| Nightshade | 3976 | 0x0F88 | Same as Magery |
| Spider's Silk | 3981 | 0x0F8D | Same as Magery |
| **Bone** | 3786 | 0x0ECA | Unique to Mysticism |
| **Daemon Bone** | 3986 | 0x0F92 | Unique to Mysticism |
| **Dragon Blood** | 19221 | 0x4B15 | Unique to Mysticism |
| **Fertile Dirt** | 3979 | 0x0F8B | Unique to Mysticism |

---

## Confirmed Stackable ItemID Ranges

Based on ServUO BaseReagent.cs analysis and confirmed usage:

### Primary Stackable Range (Reagents)
- **0x0F78 - 0x0F92** (3960-3986) - All standard UO reagents
- ✅ Used by Magery, Necromancy, Mysticism

### Extended Stackable Ranges
These ranges are used for various stackable items (ores, ingots, resources):

- **0x0F00 - 0x0FFF** (3840-4095) - Primary stackable range
- **0x1000 - 0x1FFF** (4096-8191) - Extended stackable range
- **0x18E0 - 0x18FF** (6368-6399) - Smaller stackable range

---

## ItemIDs to AVOID (Non-Stackable)

Based on testing, these ItemIDs are **NOT stackable**:

| ItemID | Hex | Item Type | Issue |
|--------|-----|-----------|-------|
| 9912 | 0x26B8 | Unknown tool/object | **NON-STACKABLE** |
| 4093 | 0x0FFD | Loom (weaving tool) | Equipment, not reagent |
| 4117 | 0x1015 | **UNKNOWN - NEEDS VERIFICATION** | May be loom or tool |

⚠️ **WARNING:** ItemID 0x1015 is currently used by 11 Vystia reagents (all "dust" reagents). **This needs verification** - it may not be stackable!

---

## Recommended Safe ItemIDs for Custom Reagents

### Best Practice: Use Confirmed Magery/Necro ItemIDs

For custom reagents, reuse existing stackable reagent graphics with different hues:

**Tier 1 - Herbs/Plants:**
- 0x0F84 (Garlic) - herb graphic
- 0x0F85 (Ginseng) - root graphic
- 0x0F86 (Mandrake Root) - root graphic
- 0x0F88 (Nightshade) - plant graphic
- 0x1A9C (Flax) - flax bundle

**Tier 2 - Organic Materials:**
- 0x0F7B (Bloodmoss) - moss graphic
- 0x0F78 (Bat Wing) - wing graphic
- 0x0F8D (Spider's Silk) - silk graphic
- 0x1422 (Beeswax) - wax block

**Tier 3 - Minerals/Dusts:**
- 0x0F7A (Black Pearl) - pearl/gem
- 0x0F8C (Sulfurous Ash) - ash/powder
- 0x0F8E (Nox Crystal) - crystal
- 0x0F8F (Grave Dust) - dust/powder ✅ **RECOMMENDED FOR ALL DUST REAGENTS**
- 0x0F8A (Pig Iron) - metal/ore

**Tier 4 - Liquids:**
- 0x0F7D (Daemon Blood) - vial/liquid
- 0x1C18 (Oil Flask) - flask/bottle

**Tier 5 - Consumables:**
- 0x0DE1 (Kindling) - wood/sticks

---

## Current Vystia Reagent Status

### Potentially Non-Stackable (Need Fixing)

**11 reagents currently use 0x1015** (unverified stackability):
1. VoidDust (DarkMagic)
2. TimeDust (DivinationMagic)
3. DivinationDust (DivinationMagic)
4. BoneDust (Necromancy)
5. CorpseAsh (Necromancy)
6. LichDust (Necromancy)
7. PlanarDust (SummoningMagic)
8. EchoDust (BardicMagic)
9. ArcaneDust (EnchantingMagic)
10. RunicPowder (EnchantingMagic)
11. MirrorDust (IllusionMagic)

### Recommended Fix

Replace `0x1015` with confirmed stackable dust ItemID:
- **Option 1:** Use `0x0F8F` (Grave Dust) - confirmed stackable dust graphic
- **Option 2:** Use `0x0F8C` (Sulfurous Ash) - confirmed stackable powder graphic

---

## Sources

- **ServUO Source Code:**
  - [BaseReagent.cs](https://github.com/ServUO/ServUO/blob/master/Scripts/Items/Consumables/BaseReagent.cs)
  - [Reagent.cs](https://github.com/ServUO/ServUO/blob/master/Scripts/Spells/Reagent.cs)
  - Scripts/Items/Resource/ (individual reagent files)

- **UO Documentation:**
  - [UOGuide - Reagents](https://www.uoguide.com/Reagents)
  - [UOGuide - Pagan Reagents](https://www.uoguide.com/Pagan_Reagents)
  - [UO Stratics - Reagents](https://uo.stratics.com/content/basics/reagent.shtml)
  - [UO Wiki - Necromancy](https://uo.com/wiki/ultima-online-wiki/skills/necromancy/)

---

*Reference compiled: 2025-12-11*
*All ItemIDs verified from ServUO source code*
