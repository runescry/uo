# Gift Giving Service

## Overview
The Gift Giving system manages automatic distribution of holiday gifts and special event items to players during designated periods.

## Era
- **Expansion:** All
- **Activation:** Seasonal/Event-based

## Files
| File | Description |
|------|-------------|
| `GiftGiving.cs` | Core gift distribution logic |
| `HolidayGiftBox2018.cs` | 2018 holiday gift box definition |
| `Winter2004.cs` | Winter 2004 gift content |

## Functionality
Automatically distributes themed gifts to players during holidays and special events.

### Gift Events
| Event | Typical Period | Gift Type |
|-------|---------------|-----------|
| Christmas | December | Holiday gifts |
| Anniversary | September | Anniversary items |
| Special Events | Variable | Event-specific |

### Distribution Methods
- **Login-based** - Gift given on first login during event
- **Time-based** - Gift after certain playtime during event
- **One per account** - Prevents duplicate gifts

## How it Works for Players

### Receiving Gifts
1. Log in during gift event period
2. System checks gift eligibility
3. Gift automatically placed in backpack
4. If backpack full, placed in bank

### Gift Contents
Gifts typically contain:
- Decorative items
- Consumable items
- Commemorative objects
- Rare collectibles

## Configuration
```csharp
// GiftGiving.cs
public abstract class GiftGiver
{
    public abstract DateTime Start { get; }
    public abstract DateTime Finish { get; }
    public abstract void GiveGift(Mobile mob);
}

// Example implementation
public class HolidayGiftGiver : GiftGiver
{
    public override DateTime Start => new DateTime(2024, 12, 1);
    public override DateTime Finish => new DateTime(2025, 1, 5);

    public override void GiveGift(Mobile mob)
    {
        // Create and give gift
    }
}
```

## GM Commands
```
[GiveGift [player]      - Manually give current gift
[add HolidayGiftBox2018 - Add specific gift box
```

## Creating Custom Gift Events

### Step 1: Create Gift Class
```csharp
public class MyCustomGift : GiftGiver
{
    public override DateTime Start => /* start date */;
    public override DateTime Finish => /* end date */;

    public override void GiveGift(Mobile mob)
    {
        Item gift = new MyGiftItem();
        mob.AddToBackpack(gift);
    }
}
```

### Step 2: Register Gift Giver
Add to the gift giver list in `GiftGiving.cs`.

### Step 3: Create Gift Items
Define the items that will be given.

## FAQ

**Q: I didn't get my gift, what happened?**
A: Check if you logged in during the event period. Contact staff if issue persists.

**Q: Can I get multiple gifts?**
A: Usually one per account per event.

**Q: What if my backpack was full?**
A: Gift should go to your bank box.

**Q: Are gifts tradeable?**
A: Most gifts are tradeable unless marked blessed/insured.

**Q: Can I get past event gifts?**
A: Generally no, unless staff enables them.

**Q: When is the next gift event?**
A: Check server announcements for upcoming events.

## Gift Tracking
The system tracks:
- Account gift receipt
- Character gift receipt
- Event participation
- Gift serial numbers

## Winter 2004 Example
Classic gift event structure:
- Snow globe decorations
- Winter-themed items
- One per account
- December distribution

## Related Systems
- Seasonal Events (`../Seasonal Events/`)
- Anniversary Events (`../18th Anniversary/`, `../22nd Anniversary/`)
- Holiday Settings (`../HolidaySettings.cs`)
