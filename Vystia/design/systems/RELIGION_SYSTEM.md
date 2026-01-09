# Vystia Religion System Design

## Overview

The Religion System provides 6 regional religions with piety progression, passive bonuses, and devotion powers. Players gain piety through prayer, tithing, and pilgrimages.

## Implementation Status: COMPLETE

**Location:** `ServUO/Scripts/Custom/VystiaClasses/Religion/`
**Files:** 2 (662 lines total)

---

## The Six Religions

| Religion | Region | Hue | Theme |
|----------|--------|-----|-------|
| Frostfather Cult | Frosthold | 1150 (Ice Blue) | Eternal Winter, Frozen Ancestors |
| Emberheart Order | Emberlands | 1358 (Fire Red) | Eternal Flame, Purifying Fire |
| Greenward Circle | Verdantpeak | 2010 (Forest Green) | Nature's Balance, Living World |
| Crystalline Ascendancy | Crystal Barrens | 1154 (Crystal Blue) | Cosmic Knowledge, Arcane Truth |
| Voidwalker Path | ShadowVoid | 1109 (Dark Purple) | The Void, Shadow Magic |
| Forge Pact | Ironclad | 2305 (Metallic) | Machine Spirit, Steam Power |

---

## Piety System

### Tiers
| Tier | Piety Required | Unlocks |
|------|----------------|---------|
| None | 0-49 | - |
| Initiate | 50-199 | First passive bonus |
| Devoted | 200-499 | First devotion power |
| Faithful | 500-899 | Second devotion power |
| Exalted | 900-1000 | Third devotion power |

### Maximum Piety: 1000

---

## Passive Bonuses

### Frostfather Cult
- Initiate: Cold Resistance +5%
- Devoted: Cold damage +3%

### Emberheart Order
- Initiate: Fire Resistance +5%
- Devoted: Fire damage +3%

### Greenward Circle
- Initiate: Poison Resistance +5%
- Devoted: Healing +5%

### Crystalline Ascendancy
- Initiate: Energy Resistance +5%
- Devoted: Mana Regen +2

### Voidwalker Path
- Initiate: Physical Resistance +3%
- Devoted: Stealth +5

### Forge Pact
- Initiate: Armor +5
- Devoted: Crafting +5

---

## Devotion Powers

Each religion grants 3 devotion powers at higher tiers:

### Frostfather Cult
1. **Frost Shield** (200 piety) - Temporary cold damage reduction
2. **Winter's Embrace** (500 piety) - Cold aura effect
3. **Avatar of Ice** (900 piety) - Transform into ice avatar

### Emberheart Order
1. **Flame Shield** (200) - Fire damage shield
2. **Phoenix Blessing** (500) - Fire resurrection effect
3. **Avatar of Fire** (900) - Fire transformation

### Greenward Circle
1. **Nature's Grace** (200) - Healing over time
2. **Regeneration** (500) - Enhanced healing
3. **Avatar of Nature** (900) - Nature transformation

### Crystalline Ascendancy
1. **Arcane Insight** (200) - Spell cost reduction
2. **Mana Shield** (500) - Damage absorption
3. **Avatar of Crystal** (900) - Crystal transformation

### Voidwalker Path
1. **Shadow Cloak** (200) - Stealth enhancement
2. **Void Step** (500) - Short-range teleport
3. **Avatar of Void** (900) - Void transformation

### Forge Pact
1. **Iron Skin** (200) - Physical resistance boost
2. **Mechanical Blessing** (500) - Construct enhancement
3. **Avatar of Steel** (900) - Mechanical transformation

---

## Piety Actions

| Action | Piety Gained | Cooldown | Notes |
|--------|--------------|----------|-------|
| Daily Prayer | +10 | 24 hours | At any shrine |
| Tithing | +1 per 100g | 24h cap (30 piety) | Donate gold |
| Pilgrimage | +75 | Per shrine | Visit holy sites |
| Blessed Crafting | +25 | Per item | Craft blessed items |

---

## Conversion Rules

- Players can only follow one religion at a time
- Converting to a new religion resets piety to 0
- Apostasy warning is displayed before conversion

---

## GM Commands

| Command | Access | Description |
|---------|--------|-------------|
| `[Religion` | Player | Show current status |
| `[Convert <1-6>` | Player | Convert to religion |
| `[Pray` | Player | Daily prayer |
| `[Tithe <gold>` | Player | Donate gold |
| `[SetPiety <amount>` | GM | Set exact piety |

---

## Storage

Uses dictionary-based attachment pattern:
```csharp
Dictionary<PlayerMobile, PietyData> s_PietyData
```

Each `PietyData` contains:
- Religion (enum)
- Piety (int, 0-1000)
- LastPrayer (DateTime)
- LastTithe (DateTime)

---

*Last Updated: 2026-01-03*
