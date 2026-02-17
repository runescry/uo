# Ethics Service

## Overview
The Ethics system provides a Hero vs Evil alignment framework, allowing players to choose moral paths that grant unique abilities and affect gameplay interactions.

## Era
- **Expansion:** Age of Shadows+ (`Core.AOS`)
- **Requires:** Factions system enabled
- **Facet:** Felucca only

## Files
| File | Description |
|------|-------------|
| `Core/` | Core ethics system logic |
| `Definitions/` | Ethics definitions and constants |
| `Evil/` | Evil path abilities and items |
| `Hero/` | Hero path abilities and items |

## Functionality
Players can align themselves as Heroes or Villains, gaining access to alignment-specific powers and abilities.

### Alignment Paths
| Path | Description | Alignment |
|------|-------------|-----------|
| Hero | Protector of the innocent | Good |
| Evil | Servant of darkness | Evil |

### Power System
- **Life Force** - Resource for abilities
- **Power** - Accumulated through kills
- **History** - Total power ever earned
- **Sphere** - Alignment strength

## How it Works for Players

### Joining an Ethic
1. Be a member of a faction
2. Find the appropriate shrine/altar
3. Dedicate yourself to Hero or Evil
4. Receive starting power

### Earning Power
**Heroes:**
- Kill evil players
- Kill evil creatures
- Protect innocent NPCs
- Complete heroic acts

**Evil:**
- Kill hero players
- Kill good creatures
- Sacrifice innocents
- Complete dark rituals

### Using Abilities
1. Accumulate sufficient power
2. Use alignment-specific items/abilities
3. Power consumed on use
4. Some abilities have cooldowns

## Hero Abilities
| Ability | Power Cost | Effect |
|---------|------------|--------|
| Holy Shield | Variable | Protective barrier |
| Summon Familiar | Variable | Holy creature companion |
| Holy Sword | Variable | Blessed weapon |
| Cleanse | Variable | Remove evil effects |

## Evil Abilities
| Ability | Power Cost | Effect |
|---------|------------|--------|
| Unholy Shield | Variable | Dark protection |
| Summon Familiar | Variable | Evil creature companion |
| Blight | Variable | Curse enemies |
| Corrupt | Variable | Dark weapon enchant |

## Configuration
```csharp
// In Core/
public class Ethics
{
    public static bool Enabled = true;
    public static int PowerCap = 100;
    public static TimeSpan PowerDecay = TimeSpan.FromHours(1);
}

// Power calculations
public static int GetPowerGain(Mobile killer, Mobile victim)
```

## GM Commands
```
[Ethics             - Ethics admin menu
[SetEthic [hero/evil] - Set player's ethic
[EthicPower [amount]  - Set ethic power
```

## Power Mechanics
- Maximum power: 100
- Power decays over time if unused
- Killing higher-power enemies gives more
- History tracks lifetime power earned

### Power Transfer on Kill
```
Victor gains: (Victim's Power / 5)
Victim loses: (Power / 2)
```

## FAQ

**Q: Do I need to be in a faction?**
A: Yes, ethics requires faction membership.

**Q: Can I change my ethic?**
A: Possible but with significant penalties and waiting periods.

**Q: Does ethic affect PvE?**
A: Yes, certain creatures are aligned and affect power.

**Q: What happens when I die?**
A: You lose a portion of your power to your killer.

**Q: Is there neutral option?**
A: No, you must choose Hero or Evil once you join.

**Q: How long does power last?**
A: Power decays slowly over time if not used.

## Alignment Interactions
| Attacker | Target | Result |
|----------|--------|--------|
| Hero | Evil | Power gain for hero |
| Evil | Hero | Power gain for evil |
| Hero | Hero | No benefit, criminal flag |
| Evil | Evil | No benefit |

## Ethic Items
Special items only usable by aligned players:
- **Hero Items** - Holy/blessed equipment
- **Evil Items** - Cursed/dark equipment
- **Neutral Artifacts** - Usable by either

## Related Systems
- Factions (`../Factions/`) - Required for ethics
- Vice vs Virtue (`../ViceVsVirtue/`) - Modern PvP alternative
- Virtues (`../Virtues/`) - Virtue system
