# XmlSpawner Service

## Overview
XmlSpawner is a powerful spawning system that extends beyond basic monster spawning to include conditional spawning, scripted behaviors, quests, attachments, and dynamic content creation through XML configuration.

## Era
- **Expansion:** All (Third-party tool)
- **Version:** 4.00

## Files
| File/Folder | Description |
|-------------|-------------|
| `XmlSpawner Core/XmlSpawner2.cs` | Main spawner implementation |
| `XmlSpawner Core/BaseXmlSpawner.cs` | Base spawner class |
| `XmlSpawner Core/XmlSpawnerGumps.cs` | Spawner management UI |
| `XmlSpawner Core/XmlSpawnerSkillCheck.cs` | Skill trigger support |
| `XmlSpawner Core/XmlTextEntryBook.cs` | Text entry support |
| `XmlSpawner Core/ItemFlags.cs` | Item flag management |
| `XmlSpawner Core/PacketHandlerOverrides.cs` | Network packet handling |
| `XmlSpawner Core/XmlAttach/` | Attachment system |
| `XmlSpawner Core/XmlAttachments/` | Built-in attachments |
| `XmlSpawner Core/XmlItems/` | Spawner-related items |
| `XmlSpawner Core/XmlMobiles/` | Spawner-related mobiles |
| `XmlSpawner Core/XmlPropsGumps/` | Property editing gumps |
| `XmlSpawner Core/XmlQuest/` | Quest system |
| `XmlSpawner Core/XmlUtils/` | Utility commands |
| `XMLSpawner Support/` | Support items and tools |

## Functionality
XmlSpawner provides advanced spawning with scripting, triggers, and dynamic behaviors.

### Core Features
- Conditional spawning
- Trigger-based activation
- Scripted behaviors
- XML import/export
- Quest integration
- Attachment system
- Region-based spawning

### Spawn Position Types
| Type | Description |
|------|-------------|
| Random | Random within range |
| RowFill | Fill in rows |
| ColFill | Fill in columns |
| Perimeter | Around edges |
| Player | Near triggering player |
| Waypoint | At waypoint locations |
| RelXY | Relative coordinates |
| DeltaLocation | Offset from spawner |
| Location | Exact coordinates |
| Wet | Water tiles only |
| Tiles | Specific tile types |
| NoTiles | Exclude tile types |
| ItemID | Near specific items |
| NoItemID | Away from items |

## How it Works for Players

### Interacting with Spawners
1. Spawners may have triggers
2. Player actions activate spawns
3. Quests may use spawner triggers
4. Dynamic content appears based on conditions

### Quest Integration
1. XmlQuest items track progress
2. Kill objectives from spawners
3. Collection objectives
4. Trigger-based quest stages

## Configuration
```csharp
// XmlSpawner2.cs
public static void Configure()
{
    m_XmlPoints = Config.Get("XmlSpawner2.Points", false);
    m_XmlFactions = Config.Get("XmlSpawner2.Factions", false);
    m_XmlSockets = Config.Get("XmlSpawner2.Sockets", false);
}

// Time of Day modes
public enum TODModeType { Realtime, Gametime }

// Version
public const string Version = "4.00";
public const byte MaxLoops = 10; // Prevent stack overflow
```

## GM Commands
```
[XmlSpawnerAdd        - Add new spawner
[XmlEdit              - Edit spawner properties
[XmlFind              - Find spawners
[XmlAdd               - Quick add objects
[WhatIsIt             - Identify objects
[WriteMulti           - Export multi objects
```

## Attachment System
Attachments add dynamic behaviors to items and mobiles.

### Built-in Attachments
| Attachment | Description |
|------------|-------------|
| XmlData | Store custom data |
| XmlDialog | NPC dialogue |
| XmlFire | Fire effects |
| XmlFreeze | Freeze effects |
| XmlHue | Dynamic hue changes |
| XmlPoison | Poison effects |
| XmlSkill | Skill requirements |
| XmlSound | Sound effects |
| XmlStr/Dex/Int | Stat modifiers |
| XmlTitle | Custom titles |
| XmlValue | Value storage |
| XmlAnimate | Animation control |
| XmlMorph | Shape changing |
| XmlMessage | Messages |
| XmlFollow | Following behavior |
| XmlIsEnemy | Enemy flagging |
| XmlWeaponAbility | Weapon abilities |
| XmlLifeDrain | Life drain effects |
| XmlManaDrain | Mana drain effects |
| XmlStamDrain | Stamina drain |
| XmlLightning | Lightning effects |
| XmlDeathAction | On-death triggers |
| XmlMinionStrike | Minion attacks |
| XmlAddFame | Fame rewards |
| XmlAddKarma | Karma rewards |
| XmlAddVirtue | Virtue rewards |
| XmlAddTithing | Tithing points |
| XmlRestrictEquip | Equipment restrictions |
| XmlSaveItem | Persistence |
| XmlEnemyMastery | Enemy bonuses |
| XmlDate | Date tracking |
| XmlLocalVariable | Local variables |
| XmlMagicWord | Magic activation |
| XmlNextSpawn | Spawn linking |
| XmlUse | Use triggers |
| XmlAosAttributes | AOS item attributes |

## Quest System
### Quest Components
| Component | Description |
|-----------|-------------|
| XmlQuest | Quest definition |
| XmlQuestAttachment | Player quest state |
| XmlQuestBook | Quest journal |
| XmlQuestHolder | Quest container |
| XmlQuestToken | Quest progress token |
| XmlQuestNPC | Quest-giving NPC |
| QuestHolder | Quest item container |
| QuestNote | Quest information |

### Quest Gumps
| Gump | Purpose |
|------|---------|
| QuestLogGump | Quest journal |
| QuestRewardGump | Reward selection |
| XmlPlayerQuestGump | Quest interaction |
| XmlQuestBookGump | Quest book interface |

### Quest Rewards
- QuestLeadersBoard - Leaderboard display
- QuestRewardStone - Reward dispensing
- XmlQuestPoints - Point tracking
- XmlQuestPointsRewards - Point rewards

## Spawner Items
| Item | Description |
|------|-------------|
| QuestHolder | Hold quest items |
| QuestNote | Quest information |
| SimpleMap | Basic map item |
| SimpleNote | Text note |
| SimpleSwitches | Toggle switches |
| SimpleTileTrap | Tile-based trap |
| SingleUseSwitch | One-time switch |
| TimedSwitches | Timed activation |
| Tokens | Quest tokens |
| XmlSpawnerAddon | Addon support |
| XmlQuestMaker | Quest creation tool |

## Spawner Mobiles
| Mobile | Description |
|--------|-------------|
| TalkingBaseCreature | Dialogue-capable NPC |
| XmlQuestNPC | Quest-giving NPC |

## Property Gumps
For editing spawner and attachment properties:
- XmlPropsGump
- XmlSetGump
- XmlSetCustomEnumGump
- XmlSetListOptionGump
- XmlSetObjectGump
- XmlSetPoint2DGump
- XmlSetPoint3DGump
- XmlSetTimeSpanGump

## Support Items
| Item | Description |
|------|-------------|
| BaseArtifact | Artifact base class |
| SimpleArtifact | Simple artifact implementation |
| SpawnerExporter | Export spawner data |
| StaffCloak | Staff identification |

## FAQ

**Q: What's the difference from regular spawners?**
A: XmlSpawner adds triggers, conditions, scripting, and quests.

**Q: How do I create a spawner?**
A: Use [XmlSpawnerAdd command or properties gump.

**Q: Can spawners trigger on player actions?**
A: Yes, through skill checks, proximity, and speech.

**Q: How do attachments work?**
A: Attachments add behaviors to any item or mobile.

**Q: Can I export spawner configurations?**
A: Yes, to XML format for backup/sharing.

**Q: How do quests integrate?**
A: XmlQuest system ties into spawner triggers.

**Q: What's the MaxLoops limit?**
A: 10 recursive calls to prevent stack overflow.

## Scripting
XmlSpawner supports basic scripting in spawn definitions:
- Conditional spawning
- Property setting
- Variable use
- Trigger chains
- Timer-based events

## Time of Day
```csharp
public enum TODModeType
{
    Realtime,  // Real-world time
    Gametime   // In-game time
}
```

## Related Systems
- Spawner (`../Spawner/`) - Basic spawner
- Quests (`../Quests/`)
- Loot Generation (`../LootGeneration/`)
- Champion System (`../ChampionSystem/`)
