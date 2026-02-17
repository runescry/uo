# Expansions Service

## Overview
The Expansions service manages expansion-specific content, features, and dungeons that were added in major game expansions like Mondain's Legacy, High Seas, and Time of Legends.

## Era
- **Coverage:** Mondain's Legacy through Time of Legends
- **Core Checks:** `Core.ML`, `Core.HS`, `Core.SA`, `Core.TOL`

## Files
| File | Description |
|------|-------------|
| `MondainsLegacy.cs` | ML dungeon and content management |
| `TimeOfLegends.cs` | TOL content and features |
| `High Seas/` | High Seas expansion content |
| `Time Of Legends/` | TOL-specific implementations |

## Mondain's Legacy (`MondainsLegacy.cs`)

### Managed Dungeons
| Dungeon | Toggle Property |
|---------|-----------------|
| Palace of Paroxysmus | `Parox` |
| Twisted Weald | `TwistedWeald` |
| Blighted Grove | `BlightedGrove` |
| Bedlam | `Bedlam` |
| Prism of Light | `PrismOfLight` |
| The Citadel | `Citadel` |
| Painted Caves | `PaintedCaves` |
| Labyrinth | `Labyrinth` |
| Sanctuary | `Sanctuary` |
| Stygian Dragon Lair | `StygianDragonLair` |
| Medusa's Lair | `MedusasLair` |

### ML Features
- Elven race and items
- Peerless boss encounters
- New quest system
- Special artifacts
- Heartwood elven city

### Configuration
```csharp
// MondainsLegacy.cs
public static bool Parox { get; set; }
public static bool TwistedWeald { get; set; }
public static bool BlightedGrove { get; set; }
// ... etc

public static void Configure()
{
    // Load settings from config
}
```

### GM Commands
```
[DecorateML         - Generate ML decoration
[SettingsML         - Toggle dungeon settings
```

## Time of Legends (`TimeOfLegends.cs`)

### TOL Content
- Eodon jungle continent
- Shadowguard encounters
- Myrmidex faction system
- Dragon Turtle champion
- New creature types

### Managed Features
| Feature | Description |
|---------|-------------|
| Eodon | Jungle continent access |
| Shadowguard | Multi-wing dungeon |
| Myrmidex | Ant-like creature faction |
| Dragon Turtle | Champion spawn |

### Recipe System
TOL adds recipes that drop from specific creatures:
```csharp
public static Type[] RecipeDrops = new Type[]
{
    // Creature types that drop recipes
};

public static void CheckRecipeDrop(Mobile killed)
{
    // Random recipe drop on kill
}
```

## High Seas (`High Seas/`)

### HS Features
- Ship combat system
- Sea creatures and monsters
- Fishing enhancements
- Pirate encounters
- Underwater content

### Ship Types
| Ship | Size | Cannons |
|------|------|---------|
| Small Ship | Small | 2 |
| Medium Ship | Medium | 4 |
| Large Ship | Large | 6 |
| Galleon | Huge | 8 |

## How it Works for Players

### Accessing ML Content
1. Ensure ML expansion is enabled
2. Travel to ML locations (Heartwood, dungeons)
3. Meet skill/quest requirements
4. Enter expansion dungeons

### Accessing TOL Content
1. Ensure TOL expansion is enabled
2. Use moongate to Eodon
3. Explore new continent
4. Participate in Myrmidex conflict

### Accessing HS Content
1. Ensure HS expansion is enabled
2. Obtain a ship
3. Sail the seas
4. Engage in naval combat

## Configuration
```csharp
// Global expansion checks
if (Core.ML) { /* ML content enabled */ }
if (Core.SA) { /* SA content enabled */ }
if (Core.HS) { /* HS content enabled */ }
if (Core.TOL) { /* TOL content enabled */ }
```

## Expansion Timeline
| Expansion | Year | Key Features |
|-----------|------|--------------|
| Mondain's Legacy | 2005 | Elves, Peerless |
| Stygian Abyss | 2009 | Gargoyles, SA dungeon |
| High Seas | 2010 | Ships, naval combat |
| Time of Legends | 2015 | Eodon, Pet Training |
| Endless Journey | 2018 | F2P model |

## FAQ

**Q: How do I enable an expansion?**
A: Set the Core.Expansion value in server configuration.

**Q: Can I enable partial expansion content?**
A: Yes, use SettingsML and similar commands to toggle specific features.

**Q: Do I need client files for expansions?**
A: Yes, players need appropriate client version for expansion content.

**Q: Can expansions be disabled mid-shard?**
A: Possible but may cause issues with existing expansion items/characters.

**Q: What expansion should I run?**
A: Depends on your shard's theme and player preferences.

## Related Systems
- Peerless (`../Peerless/`) - ML boss encounters
- Pet Training (`../Pet Training/`) - TOL system
- Seasonal Events (`../Seasonal Events/`) - Expansion events
- All dungeon systems
