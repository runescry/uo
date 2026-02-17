# Malas Service

## Overview
The Malas service manages Malas-specific content including the Sphynx encounter and other unique features of the Malas facet.

## Era
- **Expansion:** Age of Shadows+ (`Core.AOS`)
- **Facet:** Malas

## Files
| File | Description |
|------|-------------|
| `Sphynx/` | Sphynx encounter and riddles |

## Functionality
Manages unique encounters and mechanics specific to the Malas facet.

### The Sphynx
A riddle-based encounter where players must answer correctly or face consequences.

## Sphynx Encounter

### Location
Found in the Malas desert region.

### Mechanics
1. Approach the Sphynx
2. Sphynx poses a riddle
3. Answer correctly = reward
4. Answer incorrectly = consequence

### Riddle System
- Multiple riddles in rotation
- Various difficulty levels
- Timed response window
- One attempt per encounter

## How it Works for Players

### Finding the Sphynx
1. Travel to Malas
2. Explore the desert region
3. Locate the Sphynx statue/NPC

### Attempting the Riddle
1. Speak with the Sphynx
2. Receive a riddle
3. Type your answer
4. Await judgment

### Outcomes
**Correct Answer:**
- Reward item
- Passage granted
- Possible buff

**Incorrect Answer:**
- Damage or death
- Teleportation
- Temporary curse

## Configuration
```csharp
// Sphynx configuration
public class Sphynx : BaseCreature
{
    public string[] Riddles { get; set; }
    public string[] Answers { get; set; }
    public TimeSpan AnswerTime { get; set; }
}
```

## GM Commands
```
[add Sphynx
[SphynxReset
```

## Example Riddles
| Riddle | Answer |
|--------|--------|
| "What has keys but no locks?" | Piano |
| "What has hands but cannot clap?" | Clock |
| (Various mythology-themed riddles) | (Answers) |

## FAQ

**Q: Where is the Sphynx?**
A: In the Malas desert, specific coordinates vary.

**Q: How often can I attempt riddles?**
A: Typically once per Sphynx cooldown.

**Q: What happens if I don't answer?**
A: Treated as incorrect answer.

**Q: Are answers case-sensitive?**
A: Usually not, but check server configuration.

**Q: What rewards are available?**
A: Gold, items, possibly rare artifacts.

## Malas Features
The Malas facet includes:
- Luna (housing/shopping city)
- Umbra (dark city)
- Doom dungeon
- Desert regions
- Champion spawns

## Related Systems
- Doom (`../Doom/`) - Malas dungeon
- Champion System (`../ChampionSystem/`) - Malas spawns
