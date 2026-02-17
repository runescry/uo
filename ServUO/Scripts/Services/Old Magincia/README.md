# Old Magincia Service

## Overview
The Old Magincia service contains content related to the original Magincia city before its destruction, including the decorated/rebuilt versions.

## Era
- **Expansion:** All (Pre-destruction content)
- **Location:** Magincia island

## Files
| File | Description |
|------|-------------|
| `Magincia/` | Original Magincia city content |
| `Decorated Magincia/` | Festive/decorated version |

## Functionality
Manages the historical Magincia content before the city was destroyed and rebuilt as New Magincia.

### Historical Context
- Original Magincia was a wealthy city
- Destroyed by daemon invasion
- Rubble and ruins remained
- Eventually rebuilt as New Magincia

### Decorated Version
Special decorated version for events:
- Holiday decorations
- Festival content
- Seasonal changes

## Content

### Original Magincia
- Pre-destruction buildings
- Original NPCs
- Historical items
- City layout

### Decorated Magincia
- Holiday themes
- Special decorations
- Event-specific content

## How it Works for Players

### Visiting Old Magincia
1. Travel to Magincia island
2. View ruins/original city
3. Interact with historical content

### Decorated Events
1. During special events
2. City becomes decorated
3. Special NPCs/items available

## Configuration
```csharp
// Decoration activation
public static bool DecoratedMagincia = false;

// Toggle decorated version
public static void ToggleDecorations(bool enabled)
```

## GM Commands
```
[DecorateMagincia       - Toggle decorations
```

## Historical NPCs
| NPC | Role |
|-----|------|
| Merchants | Trade goods |
| Bankers | Banking services |
| Quest NPCs | Local quests |

## FAQ

**Q: What happened to old Magincia?**
A: Destroyed by daemon invasion in game lore.

**Q: Can I visit the ruins?**
A: Yes, the island still exists in-game.

**Q: What's the difference from New Magincia?**
A: Old = destroyed/ruins, New = rebuilt city with different systems.

**Q: When is decorated Magincia available?**
A: During special holiday events (server-configured).

**Q: Is there content unique to Old Magincia?**
A: Some historical items and NPCs may be unique.

## Related Systems
- New Magincia (`../New Magincia/`) - Rebuilt city
- Gift Giving (`../GiftGiving/`) - Holiday events
- Seasonal Events (`../Seasonal Events/`)
