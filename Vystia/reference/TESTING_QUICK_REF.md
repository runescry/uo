# Vystia Testing Quick Reference

*Command-only reference for GM testing. See TESTING_GUIDE.md for full details.*

---

## Environment Setup
| Command | Purpose |
|---------|---------|
| `[VTKG` | Open VTK Gump interface |
| `[vtk` | Spawn all 12 test tables at Green Acres |
| `[vtkclean` | Clean up test items |
| `[Go 5445 1153 0` | Teleport to Green Acres |
| `[svs 100` | Set all Vystia skills to 100 |
| `[skillcap 7200` | Set skill cap to 7200 |

---

## Class System (25 Classes)
| Command | Purpose |
|---------|---------|
| `[SetClassV2 <ClassName>` | Assign class |
| `[ClassInfoV2` | View current class details |
| `[ListClassesV2` | List all 25 classes |

**Classes:** Barbarian, Beastmaster, IceMage, Sorcerer, Ranger, Illusionist, Witch, Warlock, Necromancer, Druid, Alchemist, Wizard, Oracle, Artificer, Fighter, Monk, Templar, Summoner, BountyHunter, Knight, Shaman, Cleric, Paladin, Bard (Songweaving), Enchanter

---

## Magic System (11 Spellbooks + Songweaving)
| Command | Purpose | Spell IDs |
|---------|---------|-----------|
| `[sb ice` | Ice Magic spellbook | 1000-1031 |
| `[sb nature` | Nature Magic spellbook | 1032-1063 |
| `[sb hex` | Hex Magic spellbook | 1064-1095 |
| `[sb elemental` | Elemental Magic spellbook | 1096-1127 |
| `[sb dark` | Dark Magic spellbook | 1128-1159 |
| `[sb divination` | Divination spellbook | 1160-1191 |
| `[sb necro` | Necromancy spellbook | 1192-1223 |
| `[sb summoning` | Summoning spellbook | 1224-1255 |
| `[sb shamanic` | Shamanic spellbook | 1256-1287 |
| `[sb songweaving` | Songweaving songbook | songs list |
| `[song` | Songweaving song command | provocation/peacemaking/discordance/requiem/mending/courage/swiftness/light/fortune |
| `[sb enchanting` | Enchanting spellbook | 1320-1351 |
| `[sb illusion` | Illusion spellbook | 1352-1383 |

---

## Combat System
### Secondary Resources
| Command | Purpose |
|---------|---------|
| `[GetResources` | View all 15 resource values |
| `[SetResource <Type> <Value>` | Set resource value |
| `[ResetResources` | Reset all resources to 0 |

**Resources:** Fury, Chi, Combo, Energy, Focus, Essence, Mana, Nature, Shadow, Soul, Spirit, Rage, Heat, Chill, Crescendo

### Buffs & Debuffs
| Command | Purpose |
|---------|---------|
| `[ApplyBuff <Type> <Duration> <Power>` | Apply buff |
| `[ListBuffs` | View active buffs |
| `[RemoveBuff <Type>` | Remove specific buff |

### Crowd Control
| Command | Purpose |
|---------|---------|
| `[ApplyCC <Type> <Duration>` | Apply CC effect |
| `[CheckDR <Type>` | Check DR level |

**CC Types:** Stun, Root, Silence, Fear, Sleep, Slow, etc.

### Stances
| Command | Purpose |
|---------|---------|
| `[SetStance <Name>` | Activate stance |
| `[StanceInfo` | View current stance |
| `[ListStances` | List all 28 stances |

### Combat Dummies
| Command | Purpose |
|---------|---------|
| `[CD Passive` | Passive (damage tracking) |
| `[CD Melee` | Melee retaliation |
| `[CD Caster` | Spell casting |
| `[CD Hybrid` | Both melee and spells |
| `[CD Hybrid <Faction>` | With faction affiliation |
| `[CD Hybrid None <Religion>` | With religion affiliation |

---

## Creatures & Bosses
| Command | Purpose |
|---------|---------|
| `[spawnvystia` | Open spawn gump (3 pages) |
| `[BossStones` / `[BS` | Get all 10 boss stones |
| `[AncientStones` / `[AS` | Get all 12 ancient stones |
| `[kill` | Kill targeted creature |

---

## Economy Systems
### Factions
| Command | Purpose |
|---------|---------|
| `[Factions` | View all 7 faction reps |
| `[SetReputation <Faction> <Value>` | Set faction rep |
| `[FactionStones` | Get faction test stones |

**Factions:** Frostguard, FlameLegion, Greenward, ArcaneConclave, Technoguild, Sandwalkers, Voidborn

### Religion
| Command | Purpose |
|---------|---------|
| `[Religion` | View current religion/piety |
| `[SetReligion <Religion>` | Join religion |
| `[SetPiety <Value>` | Set piety level |
| `[ShrineStones` | Get shrine spawn stones |

**Religions:** FrosthelmFaith, SuryasSandscript, LunarasCovenant, CelestisArcanum, OceanasCovenant, CogsmithCreed

### Zones
| Command | Purpose |
|---------|---------|
| `[ZoneInfo` / `[ZI` | View current zone |
| `[SetZone <Type>` / `[SZ` | Set zone type |
| `[ZoneList` / `[ZL` | List zone assignments |

**Zone Types:** Sanctuary, Contested, Lawless, Extreme

### Housing
| Command | Purpose |
|---------|---------|
| `[HouseCosts` / `[HC` | View house prices |
| `[TaxInfo` / `[TI` | View property tax status |
| `[PayTax` / `[PT` | Pay taxes early |

---

## Pet System
| Command | Purpose |
|---------|---------|
| `[SummonPet <Type>` | Summon pet |
| `[PetInfo` | View pet stats |
| `[DismissPets` | Dismiss all pets |

**Pet Types:** FireElemental, IceElemental, EarthElemental, StormElemental, WaterSpirit, SkeletonWarrior, Zombie, Wraith, BoneKnight, DeathKnight, ClockworkSpider, ClockworkScout, SteamGolem, GearBeast

---

## Quest System
| Command | Purpose |
|---------|---------|
| `[QuestEditor` / `[QE` | Open quest editor |
| `[QuestWizard` | Create quest wizard |
| `[addquestNPC` / `[aqn` | Spawn quest NPC |
| `[save` | Save world (quest persistence) |

---

## Vendors & Equipment
| Command | Purpose |
|---------|---------|
| `[spawnvystia` | Page 3 = Vendors |
| `[spawntrainer <Class>` / `[str` | Spawn class trainer |
| `[spawnalltrainers` / `[sat` | Spawn all 25 trainers |

---

## AI Sidekicks
| Command | Purpose |
|---------|---------|
| `[SpawnSidekick Mage` | Spawn mage companion |
| `[SpawnSidekick Warrior` | Spawn warrior companion |
| `[st` | Spawn tamer companion |
| `[ol` | Spawn Arctic Ogre Lord (test target) |

---

## Admin Tools
| Command | Purpose |
|---------|---------|
| `[VA` | Open VystiaAdmin panel |
| `[AbilityEditor` | Open ability editor |

---

## Quick Test Sequence

```
1. [VTKG                    # Open test kit
2. Click "Spawn ALL Tables" # Create test area
3. Click "Go to Green Acres"# Teleport
4. [svs 100                 # Max skills
5. [SetClassV2 Barbarian    # Test class
6. [sb ice                  # Get spellbook
7. [CD Passive              # Spawn target
8. Cast spells              # Test magic
9. [Factions                # Check rep
10. [ShrineStones           # Get shrines
11. [spawnvystia            # Test spawns
12. [VA                     # Admin panel
```

---

*Last Updated: 2026-01-06*

