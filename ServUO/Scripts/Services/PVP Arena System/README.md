# PVP Arena System Service

## Overview
The PVP Arena System provides organized player-versus-player combat in designated arenas with rankings, matchmaking, and rewards.

## Era
- **Expansion:** Time of Legends+ (`Core.TOL`)

## Files
| File | Description |
|------|-------------|
| `PVPArenaSystem.cs` | Core arena system |
| `PVPArena.cs` | Individual arena definition |
| `ArenaDuel.cs` | Duel management |
| `ArenaTeam.cs` | Team-based matches |
| `ArenaGate.cs` | Arena entry portal |
| `ArenaExitBanner.cs` | Arena exit |
| `ArenaManager.cs` | Arena management |
| `ArenaStone.cs` | Arena access stone |
| `ArenaStats.cs` | Player statistics |
| `Definitions.cs` | Arena type definitions |
| `Gumps.cs` | Arena UI |
| `Region.cs` | Arena region rules |

## Functionality
Provides structured PvP with rules, rankings, and no permanent penalties.

### Arena Features
- No stat/skill loss on death
- Equipment protection
- Ranking system
- Matchmaking
- Team and solo modes

### Match Types
| Type | Description |
|------|-------------|
| Duel | 1v1 combat |
| Team | Team vs team |
| Tournament | Bracket elimination |
| Practice | No ranking impact |

## How it Works for Players

### Joining Arena
1. Find Arena Stone or Gate
2. Queue for match type
3. Wait for matchmaking
4. Enter when matched

### During Match
1. Fight opponent(s)
2. Use normal combat skills
3. No item loss on death
4. Match ends on victory

### After Match
1. Rankings updated
2. Stats recorded
3. Rewards distributed
4. Return to normal world

## Configuration
```csharp
// PVPArenaSystem.cs
public class PVPArenaSystem
{
    public static bool Enabled = true;
    public static TimeSpan MatchDuration = TimeSpan.FromMinutes(10);
    public static int MinLevel = 0;
}

// ArenaStats.cs
public class ArenaStats
{
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Rating { get; set; }
}
```

## GM Commands
```
[ArenaAdmin            - Arena management
[add ArenaStone
[add ArenaGate
```

## Arena Regions
Arenas have special rules:
- No looting corpses
- Instant resurrection
- Combat logging prevention
- Time limits enforced

## Ranking System
| Rank | Rating Range |
|------|-------------|
| Novice | 0-999 |
| Apprentice | 1000-1499 |
| Journeyman | 1500-1999 |
| Expert | 2000-2499 |
| Master | 2500-2999 |
| Grandmaster | 3000+ |

### Rating Changes
- Win: +15-25 rating
- Loss: -10-20 rating
- Adjusted by opponent rating

## FAQ

**Q: Do I lose items if I die?**
A: No, equipment is protected in arenas.

**Q: Is there skill loss?**
A: No stat or skill loss in arenas.

**Q: How do rankings work?**
A: ELO-style rating system based on wins/losses.

**Q: Can I practice without affecting rank?**
A: Yes, use practice mode.

**Q: Are there rewards?**
A: Ranking milestones may unlock rewards.

**Q: Can I decline a match?**
A: Yes, with potential queue penalty.

## Match Flow
```
1. Queue for match
2. Matchmaking finds opponent
3. Both players notified
4. Teleport to arena
5. Countdown begins
6. Combat active
7. Victory determined
8. Teleport out
9. Stats updated
```

## Team Matches
- Form team before queuing
- Matched against similar team
- Coordination rewarded
- Team rankings tracked

## Related Systems
- Vice vs Virtue (`../ViceVsVirtue/`) - Open world PvP
- Factions (`../Factions/`) - Legacy PvP
- Party (`../Party/`) - Team formation
