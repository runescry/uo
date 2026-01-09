# Vystia Classes Reference

**Total:** 25 playable classes
**Status:** All implemented and functional
**Location:** `ServUO/Scripts/Custom/VystiaClasses/Classes/`

---

## Class Overview Table

| # | Class | Region | Role | STR | DEX | INT | Primary Skill | Spellbook |
|---|-------|--------|------|-----|-----|-----|---------------|-----------|
| 1 | Barbarian | Frosthold | Melee DPS | 45 | 20 | 15 | Berserking (70) | - |
| 2 | Beastmaster | Frosthold | Pet/Ranged | 25 | 40 | 15 | BeastBonding (79) | - |
| 3 | Ice Mage | Frosthold | Caster DPS | 15 | 20 | 45 | Cryomancy (58) | IceMageSpellbook |
| 4 | Sorcerer | Emberlands | Caster DPS | 15 | 20 | 45 | Elementalism (62) | SorcererSpellbook |
| 5 | Ranger | Desert | Ranged DPS | 25 | 45 | 10 | Marksmanship (75) | - |
| 6 | Illusionist | Desert | Caster CC | 15 | 23 | 42 | IllusionMagic (69) | IllusionistSpellbook |
| 7 | Witch | Shadowfen | Debuffer | 15 | 20 | 45 | Hexcraft (64) | WitchSpellbook |
| 8 | Warlock | ShadowVoid | Caster DPS | 18 | 17 | 45 | Demonology (59) | WarlockSpellbook |
| 9 | Druid | Verdantpeak | Healer/Hybrid | 20 | 20 | 40 | Druidism (61) | DruidSpellbook |
| 10 | Alchemist | Verdantpeak | Support | 20 | 30 | 30 | Transmutation (81) | - |
| 11 | Oracle | Crystal Barrens | Utility | 15 | 22 | 43 | Divination (65) | OracleSpellbook |
| 12 | Artificer | Ironclad | Pet/Ranged | 25 | 30 | 25 | Engineering (80) | - |
| 13 | Fighter | Ironclad | Tank | 40 | 25 | 15 | CombatMastery (76) | - |
| 14 | Monk | Ironclad | Melee/Hybrid | 30 | 35 | 15 | MartialArts (72) | - |
| 15 | Templar | Ironclad | Tank/DPS | 40 | 20 | 20 | Zealotry (77) | - |
| 16 | Necromancer | ShadowVoid | Caster/Pet | 18 | 17 | 45 | NecromancyArts (60) | NecromancerSpellbook |
| 17 | Summoner | Underwater | Pet/Caster | 15 | 20 | 45 | Conjuration (66) | SummonerSpellbook |
| 18 | Bounty Hunter | Multi-Regional | Ranged/Melee | 30 | 35 | 15 | Manhunting (78) | - |
| 19 | Knight | Multi-Regional | Tank/Melee | 42 | 23 | 15 | ChivalricArts (73) | - |
| 20 | Shaman | Multi-Regional | Healer/Hybrid | 20 | 20 | 40 | SpiritCalling (67) | ShamanSpellbook |
| 21 | Wizard | Crystal Barrens | Utility | 10 | 20 | 50 | ArcaneStudies (83) | - |
| 22 | Cleric | Multi-Regional | Healer | 20 | 20 | 40 | DivineGrace (82) | - |
| 23 | Paladin | Multi-Regional | Tank/Healer | 35 | 20 | 25 | HolyDevotion (74) | - |
| 24 | Bard | Multi-Regional | Support/CC | 15 | 35 | 30 | BardicLore (63) | BardSpellbook |
| 25 | Enchanter | Multi-Regional | Utility/Buff | 15 | 25 | 40 | Runeweaving (68) | EnchanterSpellbook |

---

## Classes by Region

### Frosthold (3)
| Class | Role | Special Item |
|-------|------|--------------|
| Barbarian | Melee DPS | RageTotem |
| Beastmaster | Pet/Ranged | BeastWhistle |
| Ice Mage | Caster DPS | IceMageSpellbook |

### Emberlands (1)
| Class | Role | Special Item |
|-------|------|--------------|
| Sorcerer | Caster DPS | SorcererSpellbook |

### Desert of Surya (2)
| Class | Role | Special Item |
|-------|------|--------------|
| Ranger | Ranged DPS | - |
| Illusionist | Caster CC | IllusionistSpellbook |

### Shadowfen (1)
| Class | Role | Special Item |
|-------|------|--------------|
| Witch | Debuffer | WitchSpellbook |

### ShadowVoid (2)
| Class | Role | Special Item |
|-------|------|--------------|
| Warlock | Caster DPS | WarlockSpellbook |
| Necromancer | Caster/Pet | NecromancerSpellbook |

### Verdantpeak (2)
| Class | Role | Special Item |
|-------|------|--------------|
| Druid | Healer/Hybrid | DruidSpellbook, ShapeshiftTotem |
| Alchemist | Support | AlchemistKit |

### Crystal Barrens (2)
| Class | Role | Special Item |
|-------|------|--------------|
| Oracle | Utility | OracleSpellbook, CrystalOrb |
| Wizard | Utility | - |

### Ironclad Wastes (4)
| Class | Role | Special Item |
|-------|------|--------------|
| Artificer | Pet/Ranged | ConstructControlDevice, ArtificerBlueprints |
| Fighter | Tank | - |
| Monk | Melee/Hybrid | MonkBeads |
| Templar | Tank/DPS | TemplarCross |

### Underwater (1)
| Class | Role | Special Item |
|-------|------|--------------|
| Summoner | Pet/Caster | SummonerSpellbook, SummoningCircle |

### Multi-Regional (7)
| Class | Role | Special Item |
|-------|------|--------------|
| Bounty Hunter | Ranged/Melee | BountyLedger |
| Knight | Tank/Melee | KnightBanner |
| Shaman | Healer/Hybrid | ShamanSpellbook, SpiritTotem |
| Cleric | Healer | HolySymbol |
| Paladin | Tank/Healer | - |
| Bard | Support/CC | BardSpellbook, MagicLute |
| Enchanter | Utility/Buff | EnchanterSpellbook, EnchantingCrystal |

---

## Classes by Role

### Tanks (4)
Fighter, Knight, Paladin, Templar

### Melee DPS (3)
Barbarian, Monk, (Templar hybrid)

### Ranged DPS (3)
Ranger, Bounty Hunter, (Beastmaster hybrid)

### Caster DPS (5)
Ice Mage, Sorcerer, Warlock, Necromancer, (Wizard hybrid)

### Healers (4)
Cleric, Druid, Shaman, (Paladin hybrid)

### Support/Utility (4)
Alchemist, Bard, Enchanter, Oracle

### Pet Classes (4)
Beastmaster, Artificer, Necromancer, Summoner

### Crowd Control (2)
Illusionist, Witch

---

## Special Class Items

| Item | Class | Function | Location |
|------|-------|----------|----------|
| RageTotem | Barbarian | +20 STR buff for 30s | AbilityItems/ |
| BeastWhistle | Beastmaster | Summon animal companion | ClassSpecialItems.cs |
| IceMageSpellbook | Ice Mage | 32 ice spells | VystiaSpellbooks.cs |
| SorcererSpellbook | Sorcerer | 32 elemental spells | VystiaSpellbooks.cs |
| IllusionistSpellbook | Illusionist | 32 illusion spells | VystiaSpellbooks.cs |
| WitchSpellbook | Witch | 32 hex spells | VystiaSpellbooks.cs |
| WarlockSpellbook | Warlock | 32 dark spells | VystiaSpellbooks.cs |
| DruidSpellbook | Druid | 32 nature spells | VystiaSpellbooks.cs |
| ShapeshiftTotem | Druid | 5 animal forms | AbilityItems/ |
| AlchemistKit | Alchemist | Portable alchemy station | ClassSpecialItems.cs |
| OracleSpellbook | Oracle | 32 divination spells | VystiaSpellbooks.cs |
| CrystalOrb | Oracle | Divination focus | ClassSpecialItems.cs |
| ConstructControlDevice | Artificer | Summon clockwork scout | AbilityItems/ |
| ArtificerBlueprints | Artificer | Reference book | AbilityItems/ |
| MonkBeads | Monk | Meditation aid | ClassSpecialItems.cs |
| TemplarCross | Templar | Divine symbol | ClassSpecialItems.cs |
| NecromancerSpellbook | Necromancer | 32 necromancy spells | VystiaSpellbooks.cs |
| SummonerSpellbook | Summoner | 32 summoning spells | VystiaSpellbooks.cs |
| SummoningCircle | Summoner | Summon focus | ClassSpecialItems.cs |
| BountyLedger | Bounty Hunter | Bounty tracker | ClassSpecialItems.cs |
| KnightBanner | Knight | Honor symbol | ClassSpecialItems.cs |
| ShamanSpellbook | Shaman | 32 shamanic spells | VystiaSpellbooks.cs |
| SpiritTotem | Shaman | Spirit focus | ClassSpecialItems.cs |
| HolySymbol | Cleric | AoE healing burst | AbilityItems/ |
| BardSpellbook | Bard | 32 bardic spells | VystiaSpellbooks.cs |
| MagicLute | Bard | Song focus | ClassSpecialItems.cs |
| EnchanterSpellbook | Enchanter | 32 enchanting spells | VystiaSpellbooks.cs |
| EnchantingCrystal | Enchanter | Enchantment focus | ClassSpecialItems.cs |

---

## File Locations

| Category | Path |
|----------|------|
| Class Definitions | `ServUO/Scripts/Custom/VystiaClasses/Classes/` |
| Special Items | `ServUO/Scripts/Custom/VystiaClasses/Items/AbilityItems/` |
| Consolidated Items | `ServUO/Scripts/Custom/VystiaClasses/Items/ClassSpecialItems.cs` |
| Spellbooks | `ServUO/Scripts/Items/Equipment/Spellbooks/VystiaSpellbooks.cs` |

---

*Last Updated: 2025-12-11*
*Source of Truth: This file. Do not duplicate class tables elsewhere.*
