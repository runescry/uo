# Vystia Housing System Design

## Overview

The Vystia Housing System implements custom purchase prices and weekly property taxes for player housing. This serves as a significant gold sink and economy balance mechanism.

## Implementation Status: COMPLETE

**Location:** `ServUO/Scripts/Custom/VystiaClasses/Housing/`
**Files:** 2 (1,134 lines total)

---

## Housing Tiers

| Size | Dimensions | Purchase Price | Weekly Tax |
|------|------------|----------------|------------|
| Small | 7x7 (49 tiles) | 50,000g | 500g |
| Medium | 11x11 (121 tiles) | 150,000g | 1,500g |
| Large | 15x15 (225 tiles) | 400,000g | 4,000g |
| Keep | 18x18 (324 tiles) | 1,000,000g | 10,000g |
| Castle | 31x31 (961 tiles) | 3,000,000g | 30,000g |

---

## Size Detection

The system automatically detects house size by:
1. **Type Name** - Checks for "Castle", "Keep", "Tower" in class name
2. **Component Dimensions** - Uses MultiComponentList width/height
3. **Plot Area** - Width x Height for custom foundations

---

## Features

### Price Override System
- `VystiaHousePriceOverride.Enabled` - Toggle system on/off
- Applies Vystia pricing at placement time
- Works with all house types

### Tax Collection
- Automatic weekly tax deduction
- Failure to pay results in house decay
- Grace period before demolition

---

## GM Commands

| Command | Access | Description |
|---------|--------|-------------|
| `[HouseCosts` / `[HC` | Player | Display all housing costs |
| `[SetHousePrice <amount>` / `[SHP` | GM | Override house price |
| `[HouseInfo` / `[HI` | GM | Show house details |

---

## Integration Points

### House Placement
```csharp
int price = VystiaHousingCosts.GetHousePurchasePrice(house);
int tax = VystiaHousingCosts.GetHouseWeeklyTax(house);
```

### Affordability Check
```csharp
bool canAfford = VystiaHousingCosts.CanAfford(player, VystiaHouseSize.Large);
```

### Charge Player
```csharp
bool success = VystiaHousingCosts.ChargeForHouse(player, VystiaHouseSize.Medium);
```

---

## Economy Impact

**Monthly Gold Sink Estimate (per house):**
- Small: 2,000g/month in taxes
- Medium: 6,000g/month
- Large: 16,000g/month
- Keep: 40,000g/month
- Castle: 120,000g/month

---

*Last Updated: 2026-01-03*
