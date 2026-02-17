# Seasonal Events Service

## Overview
The Seasonal Events system manages time-limited events that activate during specific periods, offering unique content, rewards, and gameplay experiences.

## Era
- **Expansion:** Various
- **Command:** `[SeasonSystemGump`

## Files
| File | Description |
|------|-------------|
| `SeasonalEventSystem.cs` | Core event management |
| `SeasonalEventGump.cs` | Admin configuration UI |
| `ForsakenFoes/` | Forsaken Foes event |
| `KrampusEncounter/` | Krampus holiday event |
| `RisingTide/` | Rising Tide pirate event |
| `TreasuresOfDoom/` | Doom treasure event |
| `TreasuresOfKhaldun/` | Khaldun treasure event |
| `TreasuresOfKotlCity/` | Kotl City event |
| `TreasuresOfSorceresDungeon/` | Sorcerer's dungeon event |
| `TreasuresOfTokuno/` | Tokuno artifact event |

## Events

### Treasures of Tokuno
**Status:** Manual activation
**Features:**
- Special artifact drops
- Turn-in system for rewards
- Multiple eras/phases
- Unique Tokuno items

### Virtue Artifacts
**Status:** Always Active
**Features:**
- Virtue-themed items
- Shrine-based acquisition
- Permanent content

### Treasures of Kotl City
**Status:** Seasonal (October)
**Duration:** 60 days
**Features:**
- Kotl City exploration
- Unique monsters
- Special artifacts

### Sorcerer's Dungeon
**Status:** Seasonal (October)
**Duration:** 60 days
**Features:**
- Halloween theme
- Magic-focused content
- Special drops

### Treasures of Doom
**Status:** Seasonal (October)
**Duration:** 60 days
**Features:**
- Enhanced Doom content
- Special creatures
- Unique rewards

### Treasures of Khaldun
**Status:** Seasonal (October)
**Duration:** 60 days
**Features:**
- Khaldun enhancements
- Ghost theme
- Special items

### Krampus Encounter
**Status:** Seasonal (December)
**Duration:** 60 days
**Features:**
- Holiday event
- Krampus boss
- Winter rewards

### Rising Tide
**Status:** Active
**Features:**
- Pirate/sea content
- Ship encounters
- Naval rewards

### Fellowship
**Status:** Inactive (January)
**Duration:** 60 days
**Features:**
- Fellowship quests
- Special NPCs
- Unique rewards

## Configuration
```csharp
// SeasonalEventSystem.cs
public static void LoadEntries()
{
    Entries.Add(new SeasonalEventEntry(
        EventType.TreasuresOfTokuno,
        "Treasures of Tokuno",
        EventStatus.Inactive));
    // ... more entries
}

// SeasonalEventEntry.cs
public enum EventStatus
{
    Inactive,  // Off
    Active,    // Always on
    Seasonal   // Date-based
}
```

## GM Commands
```
[SeasonSystemGump      - Configure events
```

## Event Status Types
| Status | Behavior |
|--------|----------|
| Inactive | Event disabled |
| Active | Always enabled |
| Seasonal | Enabled during configured dates |

## How it Works for Players

### During Active Events
1. Special content spawns
2. Unique drops available
3. Event NPCs appear
4. Limited-time rewards

### Event Participation
1. Check event status
2. Travel to event location
3. Complete event objectives
4. Claim rewards before end

## FAQ

**Q: How do I know what events are active?**
A: Town criers announce, or check with event NPCs.

**Q: Are event rewards limited?**
A: Usually time-limited, some may return.

**Q: Can admins force activate events?**
A: Yes, using SeasonSystemGump.

**Q: When do seasonal events start?**
A: Based on configured month/day/duration.

**Q: Are event items tradeable?**
A: Usually yes, unless specifically bound.

## Event Schedule (Default)
| Event | Month | Duration |
|-------|-------|----------|
| Kotl City | October | 60 days |
| Sorcerer's | October | 60 days |
| Doom | October | 60 days |
| Khaldun | October | 60 days |
| Krampus | December | 60 days |
| Fellowship | January | 60 days |

## Related Systems
- Gift Giving (`../GiftGiving/`)
- Points Systems (`../PointsSystems/`)
- Town Cryer (`../Town Cryer/`)
