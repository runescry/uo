# Vystia Spells Reference

**Total:** 384 spells (32 per school × 12 schools)
**Status:** All implemented and functional
**Spell ID Range:** 1000-1383

---

## Quick Reference

| School | Class | Spell IDs | Spellbook |
|--------|-------|-----------|-----------|
| Ice Magic | Ice Mage | 1000-1031 | IceMageSpellbook |
| Nature Magic | Druid | 1032-1063 | DruidSpellbook |
| Hex Magic | Witch | 1064-1095 | WitchSpellbook |
| Elemental Magic | Sorcerer | 1096-1127 | SorcererSpellbook |
| Dark Magic | Warlock | 1128-1159 | WarlockSpellbook |
| Divination Magic | Oracle | 1160-1191 | OracleSpellbook |
| Necromancy | Necromancer | 1192-1223 | NecromancerSpellbook |
| Summoning Magic | Summoner | 1224-1255 | SummonerSpellbook |
| Shamanic Magic | Shaman | 1256-1287 | ShamanSpellbook |
| Bardic Magic | Bard | 1288-1319 | BardSpellbook |
| Enchanting Magic | Enchanter | 1320-1351 | EnchanterSpellbook |
| Illusion Magic | Illusionist | 1352-1383 | IllusionistSpellbook |

---

## Spell ID Calculation

```
Spell ID = BaseOffset + (Circle - 1) * 4 + SpellIndex

Where:
- BaseOffset = School's starting ID (see table above)
- Circle = 1-8
- SpellIndex = 0-3 (4 spells per circle)
```

**Example:** Druid Circle 3, Spell 2
- BaseOffset = 1032
- Circle = 3
- SpellIndex = 1 (0-indexed)
- Spell ID = 1032 + (3-1)*4 + 1 = 1032 + 8 + 1 = 1041

---

## Spells by School

### Ice Magic (IDs 1000-1031)
**Class:** Ice Mage | **Skill:** Cryomancy | **Theme:** Cold, frost, freezing

| Circle | ID Range | Spells |
|--------|----------|--------|
| 1 | 1000-1003 | Frost Touch, Ice Shard, Frost Ward, Avalanche |
| 2 | 1004-1007 | Freezing Grasp, Ice Shield, Frost Slick, Glacial Mend |
| 3 | 1008-1011 | Ice Bolt, Frostbite, Frozen Ground, Ice Spear |
| 4 | 1012-1015 | Frost Armor, Ice Wall, Icicle Barrage, Permafrost |
| 5 | 1016-1019 | Glacial Strike, Frozen Tomb, Shatter, Hypothermia |
| 6 | 1020-1023 | Blizzard, Glacial Fortress, Deep Freeze, Chill Aura |
| 7 | 1024-1027 | Absolute Zero, Glacier Summon, Eternal Winter, Fimbulwinter's Wrath |
| 8 | 1028-1031 | Frost Meteor, Ice Age, Rime Reaper, Cocytus Prison |

### Nature Magic (IDs 1032-1063)
**Class:** Druid | **Skill:** Druidism | **Theme:** Plants, animals, healing

| Circle | ID Range | Spells |
|--------|----------|--------|
| 1 | 1032-1035 | Nature's Touch, Thorn Dart, Bark Skin, Verdant Growth |
| 2 | 1036-1039 | Entangling Roots, Wild Growth, Nature's Blessing, Poison Spray |
| 3 | 1040-1043 | Call Lightning, Rejuvenation, Briar Patch, Fungal Bloom |
| 4 | 1044-1047 | Summon Treant, Nature's Wrath, Regeneration, Vine Whip |
| 5 | 1048-1051 | Hurricane, Living Armor, Mass Heal, Strangling Vines |
| 6 | 1052-1055 | Earthquake, Nature's Avatar, Photosynthesis, Spore Cloud |
| 7 | 1056-1059 | Wrath of Nature, Ancient Guardian, Life Bloom, Petrify |
| 8 | 1060-1063 | Cataclysm, World Tree's Blessing, Genesis, Nature's Apocalypse |

### Hex Magic (IDs 1064-1095)
**Class:** Witch | **Skill:** Hexcraft | **Theme:** Curses, debuffs, life drain

| Circle | ID Range | Spells |
|--------|----------|--------|
| 1 | 1064-1067 | Weak Curse, Evil Eye, Hex Bolt, Minor Drain |
| 2 | 1068-1071 | Wither, Jinx, Curse of Frailty, Life Tap |
| 3 | 1072-1075 | Enfeeble, Hex of Pain, Soul Leech, Crippling Curse |
| 4 | 1076-1079 | Blight, Voodoo, Curse of Weakness, Drain Life |
| 5 | 1080-1083 | Torment, Hex of Doom, Soul Siphon, Greater Curse |
| 6 | 1084-1087 | Plague, Mass Hex, Life Drain, Curse of Death |
| 7 | 1088-1091 | Damnation, Witch's Bane, Soul Harvest, Fatal Curse |
| 8 | 1092-1095 | Apocalyptic Hex, Death Curse, Soul Destruction, Eternal Torment |

### Elemental Magic (IDs 1096-1127)
**Class:** Sorcerer | **Skill:** Elementalism | **Theme:** Fire, earth, air, water

| Circle | ID Range | Spells |
|--------|----------|--------|
| 1 | 1096-1099 | Flame Dart, Stone Skin, Gust, Water Bolt |
| 2 | 1100-1103 | Fireball, Earth Shield, Wind Slash, Tidal Wave |
| 3 | 1104-1107 | Flame Wave, Tremor, Cyclone, Flood |
| 4 | 1108-1111 | Inferno, Stone Wall, Tornado, Maelstrom |
| 5 | 1112-1115 | Meteor, Earthquake, Hurricane, Tsunami |
| 6 | 1116-1119 | Firestorm, Lava Burst, Tempest, Deluge |
| 7 | 1120-1123 | Phoenix Fire, Volcanic Eruption, Storm of Ages, Elemental Fury |
| 8 | 1124-1127 | Ragnarok, Worldbreaker, Primordial Storm, Elemental Apocalypse |

### Dark Magic (IDs 1128-1159)
**Class:** Warlock | **Skill:** Demonology | **Theme:** Demons, shadow, chaos

| Circle | ID Range | Spells |
|--------|----------|--------|
| 1 | 1128-1131 | Shadow Bolt, Dark Pact, Imp Familiar, Corruption |
| 2 | 1132-1135 | Shadow Word, Demonic Armor, Hellfire, Fear |
| 3 | 1136-1139 | Soul Drain, Summon Imp, Chaos Bolt, Darkness |
| 4 | 1140-1143 | Shadow Storm, Demon Skin, Rain of Fire, Terror |
| 5 | 1144-1147 | Soul Shatter, Summon Voidwalker, Chaos Nova, Shadowflame |
| 6 | 1148-1151 | Dark Ritual, Demon Form, Hellstorm, Mass Fear |
| 7 | 1152-1155 | Soul Harvest, Summon Doomguard, Chaos Storm, Void Zone |
| 8 | 1156-1159 | Apocalypse, Demonic Ascension, Chaotic Rift, Eternal Darkness |

### Divination Magic (IDs 1160-1191)
**Class:** Oracle | **Skill:** Divination | **Theme:** Foresight, time, fate

| Circle | ID Range | Spells |
|--------|----------|--------|
| 1 | 1160-1163 | Minor Insight, Crystal Gaze, Time Skip, Fate's Touch |
| 2 | 1164-1167 | Foresight, Scrying, Temporal Shift, Fortune |
| 3 | 1168-1171 | Precognition, Crystal Shield, Haste, Destiny Bond |
| 4 | 1172-1175 | Future Sight, Mirror Image, Time Warp, Fate Weave |
| 5 | 1176-1179 | Prophecy, Crystal Storm, Slow Time, Destiny's Call |
| 6 | 1180-1183 | True Sight, Prismatic Barrier, Time Stop, Fate's Judgment |
| 7 | 1184-1187 | Oracle's Vision, Crystal Apocalypse, Temporal Mastery, Weave Destiny |
| 8 | 1188-1191 | Omniscience, Prismatic Annihilation, Time Rewind, Fate's End |

### Necromancy (IDs 1192-1223)
**Class:** Necromancer | **Skill:** NecromancyArts | **Theme:** Undead, death, decay

| Circle | ID Range | Spells |
|--------|----------|--------|
| 1 | 1192-1195 | Raise Skeleton, Death Touch, Bone Armor, Decay |
| 2 | 1196-1199 | Summon Zombie, Life Drain, Corpse Shield, Blight |
| 3 | 1200-1203 | Animate Dead, Death Coil, Bone Wall, Pestilence |
| 4 | 1204-1207 | Summon Wraith, Soul Rend, Necrotic Armor, Plague |
| 5 | 1208-1211 | Army of Dead, Death Grip, Bone Storm, Epidemic |
| 6 | 1212-1215 | Summon Lich, Soul Harvest, Corpse Explosion, Mass Decay |
| 7 | 1216-1219 | Undead Legion, Death Wave, Necrotic Storm, Pandemic |
| 8 | 1220-1223 | Summon Death Knight, Armageddon, Apocalypse, Death's Embrace |

### Summoning Magic (IDs 1224-1255)
**Class:** Summoner | **Skill:** Conjuration | **Theme:** Creature summoning, bonds

| Circle | ID Range | Spells |
|--------|----------|--------|
| 1 | 1224-1227 | Summon Familiar, Conjure Weapon, Spirit Bond, Minor Summon |
| 2 | 1228-1231 | Summon Beast, Conjure Armor, Strengthen Bond, Lesser Summon |
| 3 | 1232-1235 | Summon Elemental, Conjure Shield, Soul Link, Summon Aid |
| 4 | 1236-1239 | Summon Guardian, Conjure Barrier, Life Bond, Greater Summon |
| 5 | 1240-1243 | Summon Champion, Conjure Fortress, Bond of Power, Major Summon |
| 6 | 1244-1247 | Summon Avatar, Conjure Storm, Eternal Bond, Superior Summon |
| 7 | 1248-1251 | Summon Titan, Conjure Apocalypse, Bond of Souls, Legendary Summon |
| 8 | 1252-1255 | Summon God, Conjure Reality, Perfect Bond, Ultimate Summon |

### Shamanic Magic (IDs 1256-1287)
**Class:** Shaman | **Skill:** SpiritCalling | **Theme:** Totems, spirits, elements

| Circle | ID Range | Spells |
|--------|----------|--------|
| 1 | 1256-1259 | Healing Totem, Spirit Bolt, Earth Shock, Ancestral Guidance |
| 2 | 1260-1263 | Fire Totem, Ghost Wolf, Flame Shock, Spirit Link |
| 3 | 1264-1267 | Lightning Totem, Spirit Walk, Chain Lightning, Ancestor's Blessing |
| 4 | 1268-1271 | Mana Totem, Spirit Strike, Lava Burst, Totemic Recall |
| 5 | 1272-1275 | Storm Totem, Spirit Form, Elemental Blast, Ancestral Spirit |
| 6 | 1276-1279 | Earth Totem, Spirit Surge, Stormstrike, Greater Totem |
| 7 | 1280-1283 | Elemental Totem, Spirit Mastery, Elemental Fury, Ancestral Wrath |
| 8 | 1284-1287 | World Totem, Spirit Apocalypse, Elemental Apocalypse, Ancestor's Vengeance |

### Bardic Magic (IDs 1288-1319)
**Class:** Bard | **Skill:** BardicLore | **Theme:** Songs, sonic, inspiration

| Circle | ID Range | Spells |
|--------|----------|--------|
| 1 | 1288-1291 | Inspiring Note, Sonic Dart, Soothing Melody, Discord |
| 2 | 1292-1295 | Battle Hymn, Sound Burst, Healing Song, Cacophony |
| 3 | 1296-1299 | War Chant, Sonic Wave, Rejuvenating Tune, Dissonance |
| 4 | 1300-1303 | Epic Ballad, Thunder Clap, Restoration Song, Shatter |
| 5 | 1304-1307 | Heroic Anthem, Sonic Boom, Mass Healing Song, Screech |
| 6 | 1308-1311 | Victory March, Thunderstorm, Song of Life, Wail |
| 7 | 1312-1315 | Legend's Call, Sonic Apocalypse, Divine Chorus, Banshee Cry |
| 8 | 1316-1319 | World Song, Sound of Doom, Resurrection Hymn, Silence of Death |

### Enchanting Magic (IDs 1320-1351)
**Class:** Enchanter | **Skill:** Runeweaving | **Theme:** Buffs, runes, enhancement

| Circle | ID Range | Spells |
|--------|----------|--------|
| 1 | 1320-1323 | Minor Enchant, Rune of Protection, Empower, Weaken Armor |
| 2 | 1324-1327 | Enchant Weapon, Rune of Strength, Greater Empower, Shatter Armor |
| 3 | 1328-1331 | Superior Enchant, Rune of Speed, Mass Empower, Armor Break |
| 4 | 1332-1335 | Major Enchant, Rune of Fortitude, Aura of Power, Disenchant |
| 5 | 1336-1339 | Greater Enchant, Rune of Might, Mass Aura, Nullify |
| 6 | 1340-1343 | Epic Enchant, Rune of Champions, Divine Empowerment, Dispel Magic |
| 7 | 1344-1347 | Legendary Enchant, Rune of Legends, Godly Empowerment, Spell Breaker |
| 8 | 1348-1351 | Mythic Enchant, Rune of Gods, Ascension, Absolute Nullification |

### Illusion Magic (IDs 1352-1383)
**Class:** Illusionist | **Skill:** IllusionMagic | **Theme:** Illusions, mind, deception

| Circle | ID Range | Spells |
|--------|----------|--------|
| 1 | 1352-1355 | Minor Illusion, Daze, Blur, Distract |
| 2 | 1356-1359 | Phantom Image, Confuse, Invisibility, Misdirect |
| 3 | 1360-1363 | Major Illusion, Mesmerize, Greater Invisibility, Decoy |
| 4 | 1364-1367 | Illusory Double, Charm, Mass Invisibility, Phantom Army |
| 5 | 1368-1371 | Grand Illusion, Dominate, Perfect Invisibility, Mirage |
| 6 | 1372-1375 | Phantasmal Killer, Mass Charm, Cloak of Shadows, Nightmare |
| 7 | 1376-1379 | Reality Warp, Mind Control, Void Cloak, Dream Walk |
| 8 | 1380-1383 | True Illusion, Absolute Domination, Nonexistence, Eternal Dream |

---

## Spell Circles - Difficulty & Skill Requirements

| Circle | Skill Required | Fizzle Range | Gain Range |
|--------|----------------|--------------|------------|
| 1 | 0 | - | 0-30 |
| 2 | 10 | 0-10 | 0-40 |
| 3 | 20 | 0-20 | 10-50 |
| 4 | 30 | 0-30 | 20-60 |
| 5 | 45 | 0-45 | 35-75 |
| 6 | 60 | 0-60 | 50-90 |
| 7 | 75 | 0-75 | 65-100 |
| 8 | 90 | 0-90 | 80-120 |

**Note:** Skill requirements are automatically determined by the spell's Circle property in `VystiaSpellBase.cs`. These values match standard UO Magery progression.

---

## File Locations

| Category | Path |
|----------|------|
| Spell Definitions | `ServUO/Scripts/Custom/VystiaClasses/Spells/` |
| Spell Initializer | `ServUO/Scripts/Custom/VystiaClasses/Spells/VystiaSpellInitializer.cs` |
| Spell Base Class | `ServUO/Scripts/Custom/VystiaClasses/Spells/VystiaSpellBase.cs` |
| Spellbooks | `ServUO/Scripts/Items/Equipment/Spellbooks/VystiaSpellbooks.cs` |
| Spell Scrolls | `ServUO/Scripts/Items/Vystia/Scrolls/` |

---

*Last Updated: 2025-12-11*
*Source of Truth: This file. Do not duplicate spell tables elsewhere.*
