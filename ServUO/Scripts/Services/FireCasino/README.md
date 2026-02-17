# Fire Casino Service

## Overview
The Fire Casino (Fortune's Fire) is a gambling establishment where players can play dice games and other casino activities for gold.

## Era
- **Expansion:** Mondain's Legacy+ (`Core.ML`)
- **Location:** Fire Island or designated casino area

## Files
| File | Description |
|------|-------------|
| `CasinoGumps.cs` | Casino game UI gumps |
| `DiceGames.cs` | Dice game logic and rules |
| `FortuesFireGrog.cs` | Special casino drink item |
| `Generate.cs` | Casino area generation |
| `Mobiles.cs` | Casino NPC definitions |
| `TentBrownAddon.cs` | Brown casino tent decoration |
| `TentWhiteAddon.cs` | White casino tent decoration |

## Functionality
Players visit the casino to gamble gold on various games of chance, primarily dice-based games.

### Games Available
- **Dice Games** - Roll dice against the house
- **High/Low** - Bet on dice outcome
- **Specific Number** - Bet on exact roll

### Casino Currency
- Standard gold coins
- No special casino chips required
- Direct gold wagering

## How it Works for Players

### Getting There
1. Travel to Fire Island (ML content)
2. Find the Fortune's Fire casino
3. Speak with casino NPCs

### Playing Dice Games
1. Approach a dice game dealer
2. Open the gambling gump
3. Place your bet (gold amount)
4. Choose your wager type
5. Roll the dice
6. Win or lose based on outcome

### Winning/Losing
- Wins pay out based on odds
- Losses taken from your gold
- House always has edge

## Game Rules

### Hi-Lo Dice
1. Bet on High (8+) or Low (6-)
2. 7 is a push (return bet)
3. Pays 1:1 on wins

### Specific Number
1. Bet on exact dice total
2. Higher payouts for rarer totals
3. Example: Rolling exact 2 or 12 pays more

## Configuration
```csharp
// DiceGames.cs
public class DiceGame
{
    public int MinBet { get; set; }
    public int MaxBet { get; set; }
    public double HouseEdge { get; set; }
}

// Generate.cs
public static void Generate()
{
    // Spawn casino NPCs and items
}
```

## GM Commands
```
[add FireCasinoDealer
[add TentBrownAddon
[add TentWhiteAddon
[FireCasinoGen          - Generate casino
```

## Casino NPCs
| NPC | Function |
|-----|----------|
| Dice Dealer | Runs dice games |
| Bartender | Serves Fortune's Fire Grog |
| Guard | Casino security |

## Fortune's Fire Grog
Special drink available at the casino:
- Unique visual effect
- Temporary buff/debuff
- Cosmetic fun item

## FAQ

**Q: Where is the Fire Casino?**
A: Fire Island, accessible via ship or moongate.

**Q: What's the minimum bet?**
A: Varies by game, typically 100-1000 gold.

**Q: Is there a maximum bet?**
A: Yes, to limit house exposure.

**Q: Can I cheat?**
A: No, dice rolls are server-controlled and random.

**Q: What are the odds?**
A: House has a small edge, varies by game type.

**Q: Is gambling with real items possible?**
A: This system uses gold only.

## Payout Table (Example)
| Bet Type | Payout |
|----------|--------|
| Hi/Lo | 1:1 |
| Exact 7 | 4:1 |
| Exact 2/12 | 30:1 |
| Exact 3/11 | 15:1 |

## Related Systems
- Points Systems (`../PointsSystems/CasinoData.cs`)
- Gold economy
