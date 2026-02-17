# Vice vs Virtue Service

## Overview
Vice vs Virtue (VvV) is a guild-based PvP system where guilds choose to fight for either Vice or Virtue. Battles occur in contested cities, and participants earn silver points to purchase exclusive rewards.

## Era
- **Expansion:** Time of Legends+ (`Core.TOL`)
- **Enabled Check:** `Config.Get("VvV.Enabled", true)`

## Files
| File/Folder | Description |
|-------------|-------------|
| `ViceVsVirtueSystem.cs` | Core VvV system and points |
| `VvVBattle.cs` | Battle management |
| `VvVRewards.cs` | Reward definitions |
| `CityInfo.cs` | City battle data |
| `GuildStats.cs` | Guild statistics tracking |
| `Gumps/` | VvV interface gumps |
| `Items/VvVAltar.cs` | Battle altar |
| `Items/VvVSigil.cs` | Capture objective |
| `Items/Rewards/` | VvV reward items |
| `Items/Rewards/Banners/` | Virtue/Vice banners |
| `Items/Rewards/Tiles/` | Decorative tiles |
| `Mobiles/SilverTrader.cs` | Reward vendor |
| `Mobiles/VvVPriest.cs` | VvV priest NPC |

## Functionality
Guild-based faction PvP with city battles and exclusive rewards.

### Teams
| Team | Hue | Philosophy |
|------|-----|------------|
| Virtue | 2124 | Fight for good |
| Vice | 2118 | Fight for evil |

### Battle Cities
- Britain
- Jhelom
- Minoc
- Moonglow
- Ocllo
- Skara Brae
- Trinsic
- Yew

### Point System
- Based on `PointsSystem` framework
- Maximum 10,000 points
- Silver currency for rewards
- Guild and individual stats

## How it Works for Players

### Joining VvV
1. Guild leader signs up guild
2. Choose Vice or Virtue alignment
3. Receive starting silver (default: 2000)
4. Participate in battles

### Battle Mechanics
1. Battles occur in contested cities
2. Capture sigils at altars
3. Defend captured objectives
4. Earn points for kills and objectives
5. Winning team gets bonus rewards

### Earning Silver
- Killing enemy participants
- Capturing objectives
- Winning battles
- Bonus for top performers

### Spending Silver
1. Visit Silver Trader NPC
2. Browse reward categories
3. Purchase with silver points
4. Receive reward items

## Configuration
```csharp
// ViceVsVirtueSystem.cs
public static bool Enabled = Config.Get("VvV.Enabled", true);
public static int StartSilver = Config.Get("VvV.StartSilver", 2000);
public static bool EnhancedRules = Config.Get("VvV.EnhancedRules", false);

public static int VirtueHue = 2124;
public static int ViceHue = 2118;

public override double MaxPoints { get { return 10000; } }
```

## GM Commands
```
[VvV                  - VvV management
[add SilverTrader     - Add reward vendor
[add VvVPriest        - Add VvV priest
[add VvVAltar         - Add battle altar
```

## Rewards
### Equipment
| Item | Description |
|------|-------------|
| VvV Arms | Special weapon styles |
| VvV Chests | Armor pieces |
| VvV Hats | Headgear |
| VvV Robe | Clothing |
| VvV Epaulettes | Shoulder items |

### Consumables
| Item | Description |
|------|-------------|
| VvV Potions | Combat potions |
| VvV Wands | Magic wands |
| Mana Spike | Mana restoration |
| Essence of Courage | Combat buff |

### Decorations
| Item | Description |
|------|-------------|
| Virtue Banners | Honor, Justice, etc. |
| Vice Banners | Covetous, Deceit, etc. |
| Dungeon Tiles | Decorative floor tiles |

### Special Items
| Item | Description |
|------|-------------|
| VvV Steeds | Special mounts |
| Cannon Turret | Defensive placement |
| VvV Trap Kit | Trap crafting |
| Morph Earrings | Appearance change |
| Forged Royal Pardon | Murder count reduction |
| VvV Hair Dye | Special hair colors |

## Battle System
```csharp
// VvVBattle.cs
public class VvVBattle
{
    public ViceVsVirtueSystem System { get; }
    public VvVCity City { get; }
    public DateTime StartTime { get; }
    public bool Active { get; }

    // Battle state management
    // Objective tracking
    // Score calculation
}
```

## Guild Statistics
```csharp
// GuildStats.cs
public class VvVGuildStats
{
    public int Kills { get; set; }
    public int Deaths { get; set; }
    public int Assists { get; set; }
    public int Score { get; set; }
    public int BattlesWon { get; set; }
}
```

## Gumps
| Gump | Purpose |
|------|---------|
| BattleStatsGump | Current battle statistics |
| BattleWarningGump | Battle start notification |
| ConfirmSignupGump | Guild signup confirmation |
| ExemptCityGump | City exemption settings |
| GuildLeaderboardGump | Top guilds ranking |
| LeaderboardGump | Individual rankings |
| RewardGump | Reward purchase interface |
| VvVBattleStatusGump | Battle status display |

## Player Entry
```csharp
// VvVPlayerEntry - extends PointsEntry
public class VvVPlayerEntry : PointsEntry
{
    public bool Active { get; set; }
    public VvVType Team { get; set; }
    public int Kills { get; set; }
    public int Deaths { get; set; }
    public int Assists { get; set; }
}
```

## City Exemption
Guilds can exempt certain cities from VvV battles:
```csharp
public List<VvVCity> ExemptCities { get; set; }
```

## FAQ

**Q: How do I join VvV?**
A: Your guild leader must sign up the guild.

**Q: Can I switch teams?**
A: Yes, with cooldown periods.

**Q: Where do battles happen?**
A: In one of the 8 contested cities.

**Q: How do I earn silver?**
A: Kill enemies, capture objectives, win battles.

**Q: What happens when I die?**
A: Stat loss may apply; respawn and continue.

**Q: Can my city be exempted?**
A: Guilds can exempt cities from their battles.

**Q: Are rewards tradeable?**
A: Varies by item type.

**Q: What's the Enhanced Rules option?**
A: Optional stricter PvP rules.

## Death Handling
```csharp
public void HandlePlayerDeath(PlayerMobile victim)
{
    // Award points to killers
    // Track statistics
    // Apply stat loss if applicable
}
```

## Related Systems
- Factions (`../Factions/`) - Legacy PvP system
- Guild System
- Points Systems (`../PointsSystems/`)
- City Loyalty (`../City Loyalty System/`)
- PVP Arena (`../PVP Arena System/`)
