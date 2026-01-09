# Vystia Faction System Design

## Overview

The Faction System provides 7 regional factions with reputation progression, vendor discounts, and faction-specific content. Each faction has allied and enemy factions.

## Implementation Status: COMPLETE

**Location:** `ServUO/Scripts/Custom/VystiaClasses/Factions/`
**Files:** 3 (1,384 lines total)

---

## The Seven Factions

| ID | Faction | Region | Enemy |
|----|---------|--------|-------|
| 1 | Frostguard | Frosthold | Flame Legion |
| 2 | Flame Legion | Emberlands | Frostguard |
| 3 | Greenward | Verdantpeak | Voidborn |
| 4 | Arcane Conclave | Crystal Barrens | None |
| 5 | Technoguild | Ironclad | None |
| 6 | Sandwalkers | Desert | None |
| 7 | Voidborn | ShadowVoid | Greenward |

---

## Reputation Tiers

| Tier | Threshold | Vendor Discount | Effects |
|------|-----------|-----------------|---------|
| Hostile | <-1000 | None | Attacked on sight |
| Unfriendly | -1000 to 0 | None | Limited services |
| Neutral | 0 to 3000 | None | Basic services |
| Friendly | 3000 to 6000 | 5% | Quest access |
| Honored | 6000 to 12000 | 8% | Advanced quests |
| Revered | 12000 to 15000 | 12% | Special items |
| Exalted | 15000+ | 15% | All content |

---

## Reputation Gains

| Action | Reputation | Notes |
|--------|------------|-------|
| Kill enemy creature | +10 to +50 | Based on difficulty |
| Complete faction quest | +100 to +500 | Based on quest tier |
| Gold donation | +50 per 1,000g | Via [DonateFaction] |
| Faction crafting | +25 per item | Regional recipes |
| Escort missions | +75 | Per escort |

---

## Reputation Losses

| Action | Reputation | Notes |
|--------|------------|-------|
| Kill faction NPC | -100 to -500 | Severity based |
| Kill faction creature | -25 to -100 | Protected beasts |
| Help enemy faction | -50 | Observed actions |
| Fail faction quest | -50 | Quest abandonment |

---

## Enemy Faction Mechanics

When gaining reputation with one faction, you may lose with its enemy:
- Frostguard <-> Flame Legion
- Greenward <-> Voidborn

**Transfer Rate:** 25% of gain becomes loss with enemy

---

## Vendor Discounts

Faction vendors apply discount based on reputation:
```csharp
int discount = FactionData.GetVendorDiscount(tier);
int finalPrice = originalPrice * (100 - discount) / 100;
```

---

## GM Commands

| Command | Access | Description |
|---------|--------|-------------|
| `[Factions` | Player | Show all standings |
| `[Faction <1-7>` | Player | Detailed faction info |
| `[SetReputation <faction> <amount>` | GM | Set exact rep |
| `[AddReputation <faction> <amount>` | GM | Modify rep |
| `[DonateFaction <faction> <gold>` | Player | Donate gold |

---

## Faction Vendors

**VystiaFactionVendor** class provides:
- Dynamic pricing based on reputation
- Faction-specific inventory
- Reputation requirement checks
- Tier-locked items

---

## Data Storage

Uses player attachment pattern:
```csharp
Dictionary<VystiaFaction, int> FactionReputations
```

Methods:
- `VystiaReputation.GetFactionReputation(pm, faction)`
- `VystiaReputation.AddReputation(pm, faction, amount, source)`
- `VystiaReputation.GetFactionTier(pm, faction)`

---

## Integration with Other Systems

### Zone System
- Faction towns are Sanctuary zones
- Faction wilderness varies by region

### Quest System
- Faction quests unlock at Friendly
- Advanced quests at Honored/Revered

### Crafting System
- Regional recipes require faction standing

---

*Last Updated: 2026-01-03*
