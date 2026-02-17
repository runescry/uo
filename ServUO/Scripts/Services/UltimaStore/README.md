# Ultima Store Service

## Overview
The Ultima Store provides an in-game marketplace where players can purchase items using Sovereigns, a premium or earned currency.

## Era
- **Expansion:** High Seas+ (`Core.HS`)

## Files
| File | Description |
|------|-------------|
| `UltimaStore.cs` | Core store implementation |
| `UltimaStoreGump.cs` | Store interface UI |
| `StoreEntry.cs` | Individual item entries |
| `PlayerProfile.cs` | Player currency tracking |
| `SystemConfig.cs` | Store configuration |
| `ReadMe.txt` | Original documentation |

## Functionality
An in-game store system for purchasing cosmetic and convenience items.

### Currency
- **Sovereigns** - Store currency
- Earned or purchased (server-dependent)
- Account-bound
- Tracked per account

### Item Categories
- Cosmetic items
- Mounts
- House decorations
- Convenience items
- Character services

## How it Works for Players

### Accessing the Store
1. Use store command or button
2. Store interface opens
3. Browse categories
4. View item details

### Making Purchases
1. Select desired item
2. Confirm you have enough currency
3. Click purchase
4. Item delivered to backpack

### Checking Balance
- Shown in store interface
- Account profile
- Balance updates in real-time

## Configuration
```csharp
// SystemConfig.cs
public static class UltimaStoreConfig
{
    public static bool Enabled = true;
    public static string CurrencyName = "Sovereigns";
}

// StoreEntry.cs
public class StoreEntry
{
    public Type ItemType { get; set; }
    public int Cost { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
}
```

## GM Commands
```
[UltimaStore           - Open store admin
[SetSovereigns [amt]   - Set player currency
[add [StoreItem]       - Add store items manually
```

## Store Categories
| Category | Items |
|----------|-------|
| Mounts | Ethereal mounts |
| Pets | Companion creatures |
| Decor | House decorations |
| Services | Character services |
| Cosmetic | Appearance items |

## Player Profile
Tracks per account:
- Sovereign balance
- Purchase history
- Pending items
- Account status

## FAQ

**Q: How do I get Sovereigns?**
A: Server-dependent - purchase, events, or achievements.

**Q: Are purchases account-wide?**
A: Delivery is per-character, currency is per-account.

**Q: Can I gift items?**
A: Depends on item - some are tradeable.

**Q: What if my pack is full?**
A: Item may go to bank or be held for later.

**Q: Are store items better than crafted?**
A: Generally cosmetic/convenience, not power advantages.

**Q: Can I refund purchases?**
A: Policy varies by server administration.

## Item Delivery
Purchase process:
1. Currency deducted
2. Item created
3. Added to backpack
4. If full, alternate delivery

## Store Items (Examples)
| Item | Cost | Category |
|------|------|----------|
| Ethereal Mount | 500 | Mounts |
| Decoration | 100 | Decor |
| Character Transfer | 1000 | Services |

## Related Systems
- Veteran Rewards (`../VeteranRewards/`)
- Currency systems
- Player accounts
