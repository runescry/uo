# 22nd Anniversary Service

## Overview
The 22nd Anniversary service provides commemorative items and gift distribution for Ultima Online's 22nd anniversary celebration.

## Era
- **Expansion:** All (Event-based)
- **Availability:** Anniversary period only

## Files
| File | Description |
|------|-------------|
| `Anniversary22GiftToken.cs` | Token redeemable for anniversary rewards |
| `Giver.cs` | NPC/system that distributes gift tokens |
| `Items/` | Subfolder containing anniversary item definitions |

## Functionality
This service handles the distribution of 22nd anniversary gift tokens and items. Players receive tokens that can be exchanged for various commemorative rewards.

### Token System
Unlike earlier anniversaries that gave items directly, the 22nd anniversary uses a token-based system:
- Players receive Anniversary Gift Tokens
- Tokens can be redeemed for choice of rewards
- Allows player preference in reward selection

## How Players Obtain Items
1. Log in during the 22nd anniversary event period
2. Receive Anniversary22GiftToken automatically
3. Double-click the token to open reward selection
4. Choose desired anniversary item(s)

## Configuration
Gift distribution controlled by:
- Event date validation in Giver class
- Per-account distribution limits
- Token redemption options

## GM Commands
No specific GM commands - token distribution is automatic during the event.

## FAQ

**Q: How many tokens do I receive?**
A: Typically one token per account during the event period.

**Q: Can I save my token for later?**
A: Yes, tokens can be held and redeemed at any time after receipt.

**Q: What rewards are available?**
A: Check the Items subfolder for available anniversary rewards, or double-click your token in-game.

**Q: Can tokens be traded?**
A: This depends on the token's configuration - check the item properties.

## Related Systems
- Gift Giving Service (`../GiftGiving/`)
- 18th Anniversary (`../18th Anniversary/`)
- Veteran Rewards (`../VeteranRewards/`)
