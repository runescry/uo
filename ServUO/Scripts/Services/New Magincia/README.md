# New Magincia Service

## Overview
The New Magincia service manages the rebuilt city of Magincia including the housing lottery, bazaar marketplace, plant growing, and distillation systems.

## Era
- **Expansion:** High Seas+ (`Core.HS`)
- **Location:** New Magincia island

## Files
| File | Description |
|------|-------------|
| `Command.cs` | Admin commands for New Magincia |
| `MaginciaPlants.cs` | Special plant growing system |
| `ReadMe.txt` | Original documentation |
| `Distillation/` | Alcohol distillation crafting |
| `Housing Lotto/` | Housing lottery system |
| `Magincia Bazaar/` | Player vendor marketplace |

## Sub-Systems

### Housing Lottery (`Housing Lotto/`)
Players can win house plots through lottery:
- Purchase lottery tickets
- Random drawing for plots
- Multiple plot sizes available
- Recurring lottery cycles

### Magincia Bazaar (`Magincia Bazaar/`)
Player-run marketplace system:
- Rent bazaar stalls
- Set up vendors
- Prime location foot traffic
- Time-limited rentals

### Distillation (`Distillation/`)
Craft alcoholic beverages:
- Use distillation equipment
- Create wines and spirits
- Special drink effects
- Decorative bottles

### Magincia Plants (`MaginciaPlants.cs`)
Special plant growing unique to Magincia:
- Region-specific plants
- Special growth conditions
- Unique harvestables

## How it Works for Players

### Housing Lottery
1. Find lottery stone in New Magincia
2. View available plots
3. Purchase lottery ticket
4. Wait for drawing
5. If winner, claim plot

### Using the Bazaar
1. Travel to Magincia Bazaar
2. Find available stall
3. Pay rental fee
4. Place vendor in stall
5. Stock and price items

### Distillation
1. Obtain distillation equipment
2. Gather ingredients
3. Use equipment to create drinks
4. Age drinks for better quality
5. Sell or consume

## Configuration
```csharp
// Command.cs
[Usage("MaginciaLottery")]
[Description("Manage Magincia housing lottery")]
public static void MaginciaLottery_OnCommand(CommandEventArgs e)

// Lottery settings
public static TimeSpan LotteryDuration = TimeSpan.FromDays(7);
public static int TicketPrice = 10000;
```

## GM Commands
```
[MaginciaLottery       - Manage lottery
[MaginciaBazaar        - Manage bazaar
[add MaginciaLottoStone
```

## Housing Lottery Details
| Plot Size | Ticket Price | Odds |
|-----------|-------------|------|
| Small | Lower | Higher |
| Medium | Medium | Medium |
| Large | Higher | Lower |

### Lottery Cycle
1. Plots become available
2. Ticket sales open
3. Sales period (typically 1 week)
4. Drawing occurs
5. Winners notified
6. Claim period
7. Unclaimed plots re-lottery

## Bazaar Details
| Stall Size | Rent (weekly) | Capacity |
|------------|---------------|----------|
| Small | Lower | Limited |
| Medium | Medium | Moderate |
| Large | Higher | Maximum |

### Stall Features
- Prime shopping location
- Easy player access
- Vendor protection
- Advertising visibility

## FAQ

**Q: How do I enter the lottery?**
A: Find the lottery stone, select a plot, purchase ticket.

**Q: Can I buy multiple tickets?**
A: Typically one per account per plot.

**Q: What if I don't claim my plot?**
A: Plot goes back to lottery after claim period.

**Q: How much is bazaar rent?**
A: Varies by stall size and duration.

**Q: Can I brew any drink?**
A: Limited to available recipes and ingredients.

**Q: Is New Magincia housing special?**
A: Prime location with unique city benefits.

## Distillation Recipes
| Drink | Ingredients | Effect |
|-------|-------------|--------|
| Wine | Grapes | Social buff |
| Spirits | Various | Stronger effect |
| Special | Rare ingredients | Unique effects |

## Related Systems
- Plants (`../Plants/`) - General plant system
- Housing System
- Vendor System
- Old Magincia (`../Old Magincia/`) - Pre-destruction content
