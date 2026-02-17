# City Loyalty System Service

## Overview
The City Loyalty System allows players to pledge allegiance to Britannian cities, earn titles, participate in elections for Governor, and benefit from city-wide trade deals.

## Era
- **Expansion:** Time of Legends+ (`Core.TOL`)
- **Enabled Check:** `Config.Get("CityLoyalty.Enabled", true)`
- **Map:** Trammel (Siege: Felucca)

## Files
| File | Description |
|------|-------------|
| `CityLoyaltySystem.cs` | Core system and points tracking |
| `CityLoyaltyEntry.cs` | Individual player loyalty data |
| `CityDefinition.cs` | City boundaries and properties |
| `Election.cs` | Governor election system |
| `Gumps.cs` | UI for loyalty and elections |
| `Setup.cs` | Initial city setup and NPCs |
| `Items/` | City-related items |
| `Mobiles/` | City NPCs (Herald, Minister, etc.) |
| `Trading/` | Trade deal system |

## Cities
| City | Description |
|------|-------------|
| Britain | Capital city, central location |
| Moonglow | Mage city, Verity Isle |
| Jhelom | Warrior city, islands |
| Yew | Forest city, druids |
| Minoc | Mining town |
| Trinsic | Paladin city |
| Skara Brae | Nature/ranger focus |
| Vesper | Trade hub |
| New Magincia | Rebuilt city |

## Functionality
Players can become citizens of cities, earn loyalty through activities, and participate in the political system.

### Loyalty System
- **Love Points** - Earned through city activities
- **Hate Points** - From enemy city actions
- **Neutrality** - Can maintain neutral status

### Loyalty Ratings
From lowest to highest:
```
Abhorred → Reviled → Despised → Loathed → Detested →
Disliked → Disfavored → Unknown → Doubted → Distrusted →
Disgraced → Denigrated → Commended → Esteemed → Respected →
Honored → Admired → Adored → Lauded → Exalted →
Revered → Venerated
```

### City Titles (Earned)
| Title | Requirement |
|-------|-------------|
| Citizen | Join city |
| Knight | Loyalty level |
| Baronet | Higher loyalty |
| Baron | Higher loyalty |
| Viscount | Higher loyalty |
| Earl | Higher loyalty |
| Marquis | Higher loyalty |
| Duke | Highest loyalty |

## How it Works for Players

### Joining a City
1. Find the City Herald in your chosen city
2. Speak to them about citizenship
3. Wait the joining period (default: 7 days after leaving another city)
4. Receive citizen title

### Earning Loyalty
- Complete trade runs using Trade Minister
- Donate gold to city treasury
- Participate in city events
- Kill enemies of the city

### Governor Elections
1. Must be a citizen to vote
2. Announce candidacy to City Stone
3. Campaign period occurs
4. Citizens vote at ballot boxes
5. Winner becomes Governor

### Governor Powers
- Set city title
- Post city announcements
- Negotiate trade deals
- Access city treasury

## Trade Deals
Governors can arrange trade deals with guilds:
| Guild | Benefit |
|-------|---------|
| Guild of Arcane Arts | Magic bonuses |
| Society of Clothiers | Tailoring bonuses |
| Bardic Collegium | Bard bonuses |
| Order of Engineers | Tinkering bonuses |
| Guild of Healers | Healing bonuses |
| Maritime Guild | Fishing/sailing bonuses |
| Merchants Association | Vendor bonuses |
| Mining Cooperative | Mining bonuses |
| League of Rangers | Combat bonuses |
| Guild of Assassins | Stealth bonuses |
| Warriors Guild | Melee bonuses |

### Trade Deal Mechanics
- Cost: 2,000,000 gold (configurable)
- Duration: 7 days (configurable)
- Cooldown: 7 days between deals
- One active deal per city

## Configuration
```csharp
// CityLoyaltySystem.cs
public static readonly bool Enabled = Config.Get("CityLoyalty.Enabled", true);
public static readonly int CitizenJoinWait = Config.Get("CityLoyalty.JoinWait", 7);
public static readonly int BannerCost = Config.Get("CityLoyalty.BannerCost", 250000);
public static readonly int TradeDealCost = Config.Get("CityLoyalty.TradeDealCost", 2000000);
public static readonly int TradeDealCostPeriod = Config.Get("CityLoyalty.TradeDealPeriod", 7);
public static readonly int TradeDealCooldown = Config.Get("CityLoyalty.TradeDealCooldown", 7);
public static readonly int MaxBallotBoxes = Config.Get("CityLoyalty.MaxBallotBoxes", 10);
```

## GM Commands
```
[CityLoyalty          - City loyalty admin gump
[SetGovernor          - Set city governor
[RemoveGovernor       - Remove governor
```

## City NPCs
| NPC | Function |
|-----|----------|
| City Herald | Citizenship management, announcements |
| Guard Captain | City defense information |
| Trade Minister | Trade runs and deals |
| City Stone | Elections and governance |

## FAQ

**Q: How long to become a citizen?**
A: 7 days after leaving a previous city (configurable).

**Q: Can I be citizen of multiple cities?**
A: No, only one city at a time.

**Q: How do I become Governor?**
A: Win an election. Must be a citizen in good standing.

**Q: What happens if no one runs for Governor?**
A: The position remains vacant until next election.

**Q: How do trade deals benefit me?**
A: All citizens receive the guild bonus while the deal is active.

**Q: How do I check my loyalty?**
A: Speak to the City Herald or check your character status.

## Election Schedule
- Elections occur periodically (configurable)
- Candidacy period for announcements
- Voting period for citizen votes
- Results announced, new Governor installed

## Related Systems
- Points Systems (`../PointsSystems/`)
- Town Cryer (`../Town Cryer/`)
- Trading System (`Trading/` subfolder)
