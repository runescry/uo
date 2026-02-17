# Virtues Service

## Overview
The Virtues system implements the eight Virtues of Britannia, allowing players to gain virtue points through noble actions and advance through virtue paths (Seeker, Follower, Knight).

## Era
- **Expansion:** All (Core UO feature)

## Files
| File | Description |
|------|-------------|
| `VirtueHelper.cs` | Core virtue calculations and utilities |
| `VirtueGump.cs` | Main virtue interface |
| `VirtueInfoGump.cs` | Virtue information display |
| `VirtueStatusGump.cs` | Current virtue status |
| `Compassion.cs` | Compassion virtue implementation |
| `Honesty.cs` | Honesty virtue implementation |
| `Honor.cs` | Honor virtue implementation |
| `Humility.cs` | Humility virtue implementation |
| `Justice.cs` | Justice virtue implementation |
| `Sacrifice.cs` | Sacrifice virtue implementation |
| `Spirituality.cs` | Spirituality virtue implementation |
| `Valor.cs` | Valor virtue implementation |

## Functionality
Players perform virtuous actions to gain points in the eight virtues of Britannia.

### The Eight Virtues
| Virtue | Max Points | Gained By |
|--------|------------|-----------|
| Compassion | 21000 | Escorting NPCs |
| Honesty | 21000 | Returning lost items |
| Honor | 20000 | Honorable combat |
| Humility | 21000 | Shrine meditation |
| Justice | 21000 | Protecting innocents |
| Sacrifice | 22000 | Self-sacrifice actions |
| Spirituality | 21000 | Shrine meditation |
| Valor | 21000 | Defeating foes |

### Virtue Levels
| Level | Title | Points Required |
|-------|-------|-----------------|
| 0 | None | 0-3999 |
| 1 | Seeker | 4000+ |
| 2 | Follower | 10000+ |
| 3 | Knight | Max (20000-22000) |

## How it Works for Players

### Gaining Virtue
1. Perform virtuous actions
2. Receive virtue point notifications
3. Check status via virtue gump
4. Progress through paths

### Virtue Actions
#### Compassion
- Escort NPC travelers to destinations
- Help those in need

#### Honesty
- Return lost items to owners
- Truthful dealings

#### Honor
- Engage in honorable combat
- Respect opponents

#### Humility
- Meditate at shrines
- Humble actions

#### Justice
- Protect innocent NPCs
- Punish murderers

#### Sacrifice
- Give up valuable items
- Self-sacrifice for others

#### Spirituality
- Meditate at shrines
- Spiritual practices

#### Valor
- Defeat challenging monsters
- Brave combat

### Checking Virtue
1. Open paperdoll
2. Click virtue symbol
3. View current standings
4. See path progress

## Configuration
```csharp
// VirtueHelper.cs
public enum VirtueLevel
{
    None,
    Seeker,
    Follower,
    Knight
}

public enum VirtueName
{
    Humility,
    Sacrifice,
    Compassion,
    Spirituality,
    Valor,
    Honor,
    Justice,
    Honesty
}

// Maximum points per virtue
public static int GetMaxAmount(VirtueName virtue)
{
    if (virtue == VirtueName.Honor)
        return 20000;
    if (virtue == VirtueName.Sacrifice)
        return 22000;
    return 21000;
}
```

## Virtue Shield Bonus
Wielding a Virtue Shield provides 1.5x bonus to virtue gains:
```csharp
if (from.FindItemOnLayer(Layer.TwoHanded) is VirtueShield)
    amount = amount + (int)(amount * 1.5);
```

## GM Commands
```
[SetVirtue [virtue] [amount]  - Set virtue points
[CheckVirtue                  - View player virtues
```

## Virtue Calculations
### Level Calculation
```csharp
public static VirtueLevel GetLevel(Mobile from, VirtueName virtue)
{
    var v = from.Virtues.GetValue((int)virtue);
    int vl;
    var vmax = GetMaxAmount(virtue);

    if (v < 4000)
        vl = 0;
    else if (v >= vmax)
        vl = 3;
    else
        vl = (v + 10000) / 10000;

    return (VirtueLevel)vl;
}
```

### Awarding Virtue
```csharp
public static bool Award(Mobile from, VirtueName virtue, int amount, ref bool gainedPath)
{
    var current = from.Virtues.GetValue((int)virtue);
    var maxAmount = GetMaxAmount(virtue);

    if (current >= maxAmount)
        return false;

    // Apply Virtue Shield bonus
    if (from.FindItemOnLayer(Layer.TwoHanded) is VirtueShield)
        amount = amount + (int)(amount * 1.5);

    // Cap at maximum
    if ((current + amount) >= maxAmount)
        amount = maxAmount - current;

    var oldLevel = GetLevel(from, virtue);
    from.Virtues.SetValue((int)virtue, current + amount);
    var newLevel = GetLevel(from, virtue);

    gainedPath = (newLevel != oldLevel);

    if (gainedPath)
        EventSink.InvokeVirtueLevelChange(...);

    return true;
}
```

### Losing Virtue
```csharp
public static bool Atrophy(Mobile from, VirtueName virtue, int amount)
{
    var current = from.Virtues.GetValue((int)virtue);

    if ((current - amount) < 0)
        amount = current;

    from.Virtues.SetValue((int)virtue, current - amount);
    return true;
}
```

## Virtue Events
Level changes trigger events for other systems:
```csharp
EventSink.InvokeVirtueLevelChange(new VirtueLevelChangeEventArgs(
    from,
    (int)oldLevel,
    (int)newLevel,
    (int)virtue
));
```

## FAQ

**Q: How do I check my virtues?**
A: Click the virtue symbol on your paperdoll.

**Q: What's the highest virtue level?**
A: Knight of the virtue (level 3).

**Q: Do virtues decay over time?**
A: Yes, through atrophy if not maintained.

**Q: What's the Virtue Shield?**
A: A special item that boosts virtue gains by 1.5x.

**Q: How do I gain Compassion?**
A: Escort NPC travelers to their destinations.

**Q: How do I gain Valor?**
A: Defeat challenging monsters in combat.

**Q: What benefits do virtues provide?**
A: Special abilities and title/recognition.

**Q: Can I lose virtue levels?**
A: Yes, through atrophy or negative actions.

## Shrine System
Players can meditate at shrines to gain virtue points:
- Each shrine corresponds to a virtue
- Meditation requires time
- Points awarded based on shrine

## Related Systems
- Vice vs Virtue (`../ViceVsVirtue/`)
- Ethics (`../Ethics/`)
- Honor System
- Quest System (`../Quests/`)
