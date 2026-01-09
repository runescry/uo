# Vystia Economy System Design

## Overview

The Economy System provides gold sinks through NPC services, repairs, and fees. This balances the economy by removing gold from circulation.

## Implementation Status: COMPLETE

**Location:** `ServUO/Scripts/Custom/VystiaClasses/Economy/`
**Files:** 2 (1,096 lines total)

---

## Service Fees

### Resurrection Service

| Fame Level | Fame Range | Cost |
|------------|------------|------|
| 0 | 0-1,249 | 50g |
| 1 | 1,250-2,499 | 60g |
| 2 | 2,500-4,999 | 70g |
| 3 | 5,000-9,999 | 80g |
| 4 | 10,000-14,999 | 90g |
| 5 | 15,000+ | 100g |

**Formula:** `BaseResurrectionCost + (FameLevel * CostPerFameLevel)`
- BaseResurrectionCost = 50g
- CostPerFameLevel = 10g

### Travel Fees (Moongate)

| Distance | Cost |
|----------|------|
| Short (<500 tiles) | 100g |
| Medium (500-1,500 tiles) | 175g |
| Long (>1,500 tiles) | 250g |

**Recall Insurance:** 25g (protects against fizzle)

### Stabling Fees

| Duration | Cost per Pet |
|----------|--------------|
| Daily | 30g |
| Weekly | 150g (2 days free) |

---

## NPCs

### VystiaHealer
**Responds to speech:**
- "resurrect", "rez", "revive" - Offer resurrection
- "heal" - Free healing (to full HP)
- "accept" - Confirm resurrection

**Features:**
- Shows resurrection gump with cost
- Checks backpack and bank gold
- 50% HP restoration on resurrect
- Visual/sound effects

### VystiaMoongateAttendant
**Responds to speech:**
- "travel", "gate", "teleport" - Show travel info
- "cost", "price", "fee" - Show fee structure

---

## Repair Service

**VystiaRepairService.cs** handles:
- Durability-based repair costs
- Regional material bonuses
- Blacksmith integration
- GM override options

---

## Gold Sink Summary

| Service | Typical Cost | Frequency |
|---------|--------------|-----------|
| Resurrection | 50-100g | Per death |
| Travel | 100-250g | Per trip |
| Stabling | 30g/day | Daily |
| House Tax | 500-30,000g | Weekly |
| Repairs | Variable | Per item |

---

## Integration

### Check Player Gold
```csharp
int total = Banker.GetBalance(pm) + GetBackpackGold(pm);
```

### Deduct Gold
```csharp
bool success = DeductGold(pm, cost);
// Tries backpack first, then bank
```

### Service Fee Check
```csharp
int cost = VystiaServiceFees.CalculateResurrectionCost(player);
bool canPay = VystiaServiceFees.DoResurrection(player, healer);
```

---

## Economy Balance Goals

1. **Gold Sink Rate** - Remove 5-10% of player wealth monthly
2. **Service Accessibility** - Keep basic services affordable
3. **Wealth Scaling** - Higher fame = higher costs
4. **Risk/Reward** - Dangerous zones have higher costs

---

*Last Updated: 2026-01-03*
