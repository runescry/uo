# 18th Anniversary Service

## Overview
The 18th Anniversary service provides commemorative items and gift distribution for Ultima Online's 18th anniversary celebration.

## Era
- **Expansion:** All (Event-based)
- **Availability:** Anniversary period only

## Files
| File | Description |
|------|-------------|
| `18th AnniversaryBag.cs` | Special container for anniversary items |
| `AnniversaryCard.cs` | Commemorative anniversary card |
| `AnniversaryPlate.cs` | Decorative plate item |
| `AnniversaryVaseShort.cs` | Short decorative vase |
| `AnniversaryVaseTall.cs` | Tall decorative vase |
| `EnchantedTimepiece.cs` | Special clock item |
| `Giver.cs` | NPC/system that distributes gifts |

## Functionality
This service handles the distribution of special anniversary items to players during the 18th anniversary event period. Items are typically collectible decorations that can be placed in player housing.

### Items Included
- **Anniversary Bag** - Container holding all anniversary gifts
- **Anniversary Card** - Commemorative card item
- **Anniversary Plates** - Decorative wall/floor plates
- **Anniversary Vases** - Decorative vases (short and tall variants)
- **Enchanted Timepiece** - Functional clock with special appearance

## How Players Obtain Items
1. Log in during the anniversary event period
2. The Giver system automatically distributes anniversary bags to eligible characters
3. Items are placed in the player's backpack or bank if backpack is full

## Configuration
The gift distribution is typically controlled by:
- Event start/end dates in the Giver class
- One gift per account restriction

## GM Commands
No specific GM commands - gifts are distributed automatically during the event period.

## FAQ

**Q: Can I get anniversary items outside the event period?**
A: No, these items are only distributed during the specific anniversary celebration window.

**Q: Are the items tradeable?**
A: Yes, anniversary items can typically be traded between players.

**Q: Can I place these items in my house?**
A: Yes, all anniversary decorations are designed for house placement.

**Q: What if I miss the event?**
A: You'll need to trade with other players or wait for potential future availability.

## Related Systems
- Gift Giving Service (`../GiftGiving/`)
- Veteran Rewards (`../VeteranRewards/`)
