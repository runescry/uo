# Factions Service

## Overview
The Faction system is a PvP system where players join one of four competing factions to fight for control of Britannian cities through sigil capture and territorial warfare.

## Era
- **Expansion:** Age of Shadows - Stygian Abyss
- **Status:** Disabled when Vice vs Virtue is enabled
- **Enabled Check:** `!ViceVsVirtueSystem.Enabled`
- **Facet:** Felucca only

## Files
| File | Description |
|------|-------------|
| `Core/` | Core faction logic, player states, persistence |
| `Definitions/` | Faction and town definitions |
| `Gumps/` | Faction UI gumps |
| `Instances/` | Faction instance implementations |
| `Items/` | Faction-specific items |
| `Mobiles/` | Faction NPCs and guards |

## The Four Factions
| Faction | Description | Stronghold |
|---------|-------------|------------|
| Minax | Followers of the sorceress Minax | Dungeon near Wrong |
| Council of Mages | Magical council | Magincia |
| True Britannians | Loyal to Lord British | Britain |
| Shadowlords | Forces of darkness | Yew |

## Functionality
Factions compete for control of towns through sigil capture, earning silver and special abilities.

### Sigil System
1. Each town has a sigil
2. Factions steal sigils from town monoliths
3. Return sigils to stronghold
4. Corrupt/purify sigils to claim towns
5. Town ownership grants benefits

### Faction Silver
- Currency earned through:
  - Killing enemy faction members
  - Killing faction-aligned creatures
  - Town ownership taxes
- Spent on:
  - Faction gear
  - Faction guards
  - Stronghold upgrades

### Skill Loss
- Faction members suffer skill loss on death to other faction members
- Loss duration: 20 minutes (5 minutes in SA+)
- Affects all skills proportionally

## How it Works for Players

### Joining a Faction
1. Find the faction stone at a stronghold
2. Use the stone to join (or have guild leader join)
3. Receive faction robe
4. 3-day waiting period to leave

### Faction Combat
1. Attack enemy faction members freely in Felucca
2. Earn kill points for victories
3. Lose kill points for defeats
4. Kill points affect rank

### Rank System
| Rank | Requirements |
|------|--------------|
| Follower | 0+ kill points |
| Rank 2-9 | Increasing kill points |
| Commander | Elected or appointed |

### Town Control
1. Steal enemy town's sigil
2. Return to your stronghold
3. Place on stronghold monolith
4. Wait for corruption/purification
5. Town becomes controlled

## Configuration
```csharp
// Core/Faction.cs Settings class
public static bool NewCoMLocation = Config.Get("Factions.NewCoMLocation", true);
public static bool Enabled = !ViceVsVirtueSystem.Enabled;

// Skill loss settings
public const double SkillLossFactor = 1.0 / 3;
public static TimeSpan SkillLossPeriod = Core.SA ? TimeSpan.FromMinutes(5) : TimeSpan.FromMinutes(20);
```

## GM Commands
```
[FactionElection      - Manage faction elections
[FactionCommander     - Set faction commander
[FactionKick          - Remove player from faction
[FactionBan           - Ban account from factions
[FactionUnban         - Remove faction ban
[FactionItemReset     - Reset faction items
[FactionReset         - Full faction reset
[FactionTownReset     - Reset town control
```

## Faction Items
| Item | Function |
|------|----------|
| Faction Robe | Identifies faction membership |
| Faction Traps | Defensive stronghold traps |
| Faction Horse | Special faction mount |
| Faction Gear | Combat equipment |

## Town Benefits
Controlling a town provides:
- Tax income
- Sheriff appointment
- Finance minister appointment
- Town guard control
- NPC vendor taxes

## FAQ

**Q: Why are Factions disabled?**
A: Factions are automatically disabled when Vice vs Virtue is enabled. They're mutually exclusive systems.

**Q: Can I be in a faction and VvV?**
A: No, you must choose one or the other.

**Q: What happens to my faction gear if I leave?**
A: Faction-imbued items lose their bonuses when you leave.

**Q: How do elections work?**
A: Faction members vote for commander candidates during election periods.

**Q: Can guilds join factions?**
A: Yes, guild leaders can join their entire guild to a faction.

**Q: What's the sigil corruption time?**
A: Varies based on corruption/purification state and faction activity.

## Sigil Mechanics
```
Corruption Grace Period: After stealing sigil
Corruption Period: Time to corrupt sigil
Purification Period: Time to purify corrupted sigil
Return Period: Time before stolen sigil returns home
```

## Related Systems
- Vice vs Virtue (`../ViceVsVirtue/`) - Modern replacement
- Ethics (`../Ethics/`) - Alignment system
- Champion System (`../ChampionSystem/`) - Faction-affected spawns
