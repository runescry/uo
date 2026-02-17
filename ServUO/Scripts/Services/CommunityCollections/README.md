# Community Collections Service

## Overview
The Community Collections system allows players to donate items to various collections (Museum, Royal Zoo) in exchange for points and rewards, including special titles.

## Era
- **Expansion:** Mondain's Legacy+ (`Core.ML`)
- **Museum:** Time of Legends+ (`Core.TOL`)

## Files
| File | Description |
|------|-------------|
| `CollectionsSystem.cs` | Core collection management |
| `BaseCollectionItem.cs` | Base class for collection items |
| `BaseCollectionMobile.cs` | Base class for collection NPCs |
| `CollectionDecayTimer.cs` | Timer for collection progress decay |
| `CollectionItem.cs` | Individual donation item definition |
| `CommunityCollectionGump.cs` | Main donation UI |
| `ConfirmRewardGump.cs` | Reward confirmation dialog |
| `ConfirmTransferPetGump.cs` | Pet donation confirmation |
| `ICollection.cs` | Collection interface |
| `MuseumDonationBox.cs` | Museum donation container |
| `RoyalZooDonationBox.cs` | Zoo donation container |
| `SelectTitleGump.cs` | Title selection UI |

## Functionality
Community Collections provide server-wide donation goals. As players donate, the collection progresses toward completion, unlocking rewards for all participants.

### Collection Types
| Collection | Location | Focus |
|------------|----------|-------|
| Britain Library | Britain | Books, scrolls |
| Royal Zoo | Moonglow | Creatures, pets |
| Museum | Britain (TOL) | Artifacts, relics |
| Vesper Museum | Vesper | Historical items |

### Donation System
- Each collection accepts specific item types
- Donations give personal points
- Community progress tracked separately
- Rewards unlock at thresholds

## How it Works for Players

### Finding Collections
1. Travel to collection location (Britain Library, Royal Zoo, etc.)
2. Find the Collection Representative NPC
3. Speak with them to view donation options

### Making Donations
1. Open the Community Collection gump
2. View accepted donation types
3. Drop acceptable items on NPC or donation box
4. Receive points based on item value

### Claiming Rewards
1. Accumulate enough personal points
2. Open rewards gump from Collection NPC
3. Select desired reward
4. Points deducted, reward given

### Earning Titles
Special titles unlock at point thresholds:
- Contributor
- Philanthropist
- Benefactor
- Patron
- (Collection-specific titles)

## Configuration
```csharp
// CollectionsSystem.cs
public static void Initialize()
{
    // Register collections
    // Set up decay timers
}

// Decay settings
public static TimeSpan DecayPeriod = TimeSpan.FromDays(1);
public static double DecayPercent = 0.01; // 1% decay
```

## GM Commands
```
[add MuseumDonationBox
[add RoyalZooDonationBox
[CollectionAdmin      - Admin gump for collections
```

## Collection Progress
Collections track:
- **Personal Points** - Your donation total
- **Community Points** - All donations combined
- **Tier Progress** - Community milestone toward rewards

### Tier System
| Tier | Community Points | Reward Unlocked |
|------|-----------------|-----------------|
| 1 | 10,000 | Basic reward |
| 2 | 50,000 | Intermediate reward |
| 3 | 100,000 | Advanced reward |
| 4 | 500,000 | Rare reward |
| 5 | 1,000,000 | Ultimate reward |

## FAQ

**Q: What items can I donate?**
A: Each collection accepts different items. Check the collection gump for accepted types.

**Q: Do my points decay?**
A: Personal points don't decay. Community progress may decay slowly over time.

**Q: Can I donate pets?**
A: Yes, the Royal Zoo accepts creature donations.

**Q: How do I get titles?**
A: Accumulate enough points and use the title selection gump.

**Q: Where is the Museum?**
A: Britain (TOL expansion required).

**Q: Can I see community progress?**
A: Yes, the collection gump shows overall community progress.

## Pet Donation (Royal Zoo)
1. Stable the pet you wish to donate
2. Visit the Royal Zoo
3. Use the donation gump
4. Confirm pet transfer
5. Receive points based on pet value

## Donation Values
Values based on:
- Item rarity
- Item condition
- Collection demand
- Community needs

## Related Systems
- Clean Up Britannia (`../CleanUpBritannia/`)
- Points Systems (`../PointsSystems/`)
- Library Collection Quest
