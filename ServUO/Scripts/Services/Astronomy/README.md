# Astronomy Service

## Overview
The Astronomy system allows players to observe celestial objects through telescopes, discover constellations, and earn the Astronomer title.

## Era
- **Expansion:** Endless Journey (`Core.EJ`)
- **Enabled Check:** `AstronomySystem.Enabled = Core.EJ`

## Files
| File | Description |
|------|-------------|
| `AstronomySystem.cs` | Core astronomy logic and constellation management |
| `AstronomerTitleDeed.cs` | Deed for claiming Astronomer title |
| `BrassOrrery.cs` | Decorative astronomy instrument |
| `ConstellationInfo.cs` | Constellation data definitions |
| `ConstellationLedger.cs` | Player's constellation discovery log |
| `PrimerOnBritannianAstronomy.cs` | Educational book item |
| `StarChart.cs` | Craftable star chart item |
| `Telescope.cs` | Main telescope item for observation |
| `Tent.cs` | Astronomy observation tent |
| `Willebrord.cs` | Astronomy NPC quest giver |
| `21st Anniversary Gifts/` | Anniversary-related astronomy items |

## Functionality
Players can engage in stargazing activities to discover constellations, learn about Britannian astronomy, and earn titles.

### Core Features
- **Telescope Observation** - Use telescopes to view the night sky
- **Constellation Discovery** - Find and record constellations
- **Star Charts** - Craft and collect star charts
- **Astronomer Title** - Earn title through discoveries

### Constellation System
- Multiple constellations available for discovery
- Constellations have specific viewing conditions
- Discovery progress tracked in Constellation Ledger
- Maximum constellations configurable: `MaxConstellations`

## How it Works for Players

### Getting Started
1. Obtain a Telescope (crafted or purchased)
2. Get a Constellation Ledger to track discoveries
3. Find a good observation location
4. Use telescope at night to observe

### Making Discoveries
1. Set up telescope in a clear area
2. Wait for nighttime
3. Use the telescope to scan the sky
4. Record discoveries in your ledger
5. Complete discoveries to earn titles

### Items Available
- **Telescope** - Required for observation
- **Constellation Ledger** - Tracks your discoveries
- **Star Chart** - Record specific constellation positions
- **Brass Orrery** - Decorative mechanical model
- **Primer on Britannian Astronomy** - Educational reading

## Configuration
```csharp
// In AstronomySystem.cs
public static bool Enabled = Core.EJ;
public static int MaxConstellations = [configurable];

// Constellation loading
public static bool CheckNameExists(string name)
```

## GM Commands
```
[add Telescope
[add ConstellationLedger
[add BrassOrrery
[add AstronomerTitleDeed
```

## FAQ

**Q: When can I observe constellations?**
A: Only at night in-game. Weather and location may also affect visibility.

**Q: How many constellations are there?**
A: The system supports multiple constellations - check your ledger for the complete list.

**Q: Where do I get a telescope?**
A: Telescopes can be crafted by Tinkers or purchased from certain NPCs.

**Q: What does the Astronomer title give me?**
A: It's a cosmetic title displayed on your character.

**Q: Can I observe from my house?**
A: Yes, if you have roof access or an observatory setup.

## Related Systems
- Tinkering Craft (`../Craft/DefTinkering.cs`)
- 21st Anniversary Content (subfolder)
- Title System
