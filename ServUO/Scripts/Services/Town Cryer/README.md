# Town Cryer Service

## Overview
The Town Cryer system manages news announcements, city information, and event notifications through Town Crier NPCs.

## Era
- **Expansion:** Time of Legends+ (`Core.TOL`)

## Files
| File | Description |
|------|-------------|
| `TownCryerSystem.cs` | Core announcement system |
| `TownCrierGreetingEntry.cs` | Greeting messages |
| `TownCryerCityEntry.cs` | City-specific news |
| `TownCryerGuildEntry.cs` | Guild announcements |
| `TownCryerModeratorEntry.cs` | Moderated content |
| `TownCryerNewsEntry.cs` | News entries |
| `Gumps/` | Town Cryer UI |
| `Quests/` | Cryer-related quests |

## Functionality
Town Criers broadcast important information to players through spoken announcements and interactive dialogs.

### Announcement Types
| Type | Content |
|------|---------|
| Greetings | Welcome messages |
| City News | Local city information |
| Guild News | Guild announcements |
| Events | Special event notices |
| General News | Server-wide information |

### Features
- Automatic announcements
- Player-submitted news (moderated)
- Guild integration
- Event notification

## How it Works for Players

### Hearing Announcements
1. Approach Town Crier NPC
2. Crier speaks announcement
3. Click Crier for more details
4. Browse news categories

### Submitting News
1. Speak with Town Crier
2. Select "Submit News"
3. Enter announcement
4. Await moderation

### Guild Announcements
1. Guild leader contacts Crier
2. Submits guild news
3. News added to rotation
4. Members see announcements

## Configuration
```csharp
// TownCryerSystem.cs
public class TownCryerSystem
{
    public static List<TownCryerNewsEntry> NewsEntries { get; }
    public static TimeSpan AnnouncementInterval { get; set; }
}

// Entry types
public class TownCryerNewsEntry
{
    public string Title { get; set; }
    public string Body { get; set; }
    public DateTime Expires { get; set; }
}
```

## GM Commands
```
[TownCrier              - Manage announcements
[add TownCrier
```

## Entry Types

### Greeting Entry
Welcomes players to the city:
- Random greeting selection
- City-specific messages
- Time-based variants

### City Entry
City-specific information:
- Governor messages
- Trade deals
- City events
- Local news

### Guild Entry
Guild announcements:
- Recruitment
- Events
- Achievements
- Messages from leadership

### Moderator Entry
Admin-approved content:
- Server announcements
- Important notices
- Event information
- Policy updates

## FAQ

**Q: How do I become a Town Crier?**
A: Town Criers are NPCs, not player roles.

**Q: Can I submit news?**
A: Yes, through the Town Crier interface.

**Q: Is submitted news reviewed?**
A: Yes, moderation required.

**Q: How long do announcements last?**
A: Until expiration date or removal.

**Q: Can guilds post freely?**
A: Guild news has separate queue.

## News Categories
Players can browse:
- Local city news
- Guild announcements
- Server events
- General information

## Announcement Rotation
Town Criers rotate through:
1. Greeting message
2. City news
3. Guild news
4. Event notices
5. Repeat cycle

## Related Systems
- City Loyalty System (`../City Loyalty System/`)
- Guild System
- Seasonal Events (`../Seasonal Events/`)
