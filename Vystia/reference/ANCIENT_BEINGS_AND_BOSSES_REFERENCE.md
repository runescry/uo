# Vystia Ancient Beings and Bosses Reference

**Generated:** 2025-12-09
**Purpose:** Complete reference for all ancient talking creatures and boss creatures with body IDs

---

## ANCIENT CREATURES (Talking NPCs with LLM Integration)

**Total: 12 Ancient Beings**

### Dragons (5)

| # | Name | Class Name | Body ID (Hex) | Body ID (Dec) | Type | Location |
|---|------|------------|---------------|---------------|------|----------|
| 1 | Frosthelm the Eternal Winter | `FrosthelmEternalWinter` | 0xC | 12 | White Dragon | Frozen Peak, Frosthold |
| 2 | Emberflame the Ashen Tyrant | `EmberflameAshenTyrant` | 0x3B | 59 | Red Dragon | Volcano caldera, Emberlands |
| 3 | Verdantheart the Forest Guardian | `VerdantheartForestGuardian` | 0x3C | 60 | Green Dragon | Hidden grove, Verdantpeak |
| 4 | Crystalwing the Prismatic Oracle | `CrystalwingPrismaticOracle` | 0x3B | 59 | Crystal Dragon | Crystal Barrens cave |
| 5 | Abyssus the Depth King | `AbyssusDepthKing` | 0x96 | 150 | Sea Serpent | Forgotten Depths trench |

### Treants (2)

| # | Name | Class Name | Body ID (Hex) | Body ID (Dec) | Type | Location |
|---|------|------------|---------------|---------------|------|----------|
| 6 | Elder Oakbark | `ElderOakbark` | 0x2F | 47 | Treant | The Grand Oak roots, Verdantpeak |
| 7 | Ironbark the War-Ancient | `IronbarkWarAncient` | 0x2F | 47 | War Treant | Verdantpeak border |

### Sphinxes (2)

| # | Name | Class Name | Body ID (Hex) | Body ID (Dec) | Type | Location |
|---|------|------------|---------------|---------------|------|----------|
| 8 | Sphynx of Emberlands | `SphynxOfEmberlands` | 0x5F | 95 | Sphinx | Volcanic wastes, Emberlands |
| 9 | The Crystal Sphinx | `TheCrystalSphinx` | 0x5F | 95 | Sphinx | Crystal Barrens cave |

### Divine Avatars / Spirits (3)

| # | Name | Class Name | Body ID (Hex) | Body ID (Dec) | Type | Location |
|---|------|------------|---------------|---------------|------|----------|
| 10 | The Frost Father's Avatar | `FrostFatherAvatar` | 0xD | 13 | Air Elemental | Aurora manifestation, Frosthold |
| 11 | The Great Machinist's Construct | `GreatMachinistConstruct` | 0x2F5 | 757 | Iron Golem | Heart of Great Forge, Ironclad |
| 12 | Lunara's Dryad Herald | `LunaraDryadHerald` | 0x191 | 401 | Female Ethereal | Grove of Spirits, Verdantpeak |

---

## BOSS CREATURES (Combat Encounters)

**Total: 10 Regional Bosses**

| # | Name | Class Name | Body ID | Type | Hue | Location | Title |
|---|------|------------|---------|------|-----|----------|-------|
| 1 | Frost Father | `FrostFather` | 76 | Titan | 1152 (ice blue) | Frozen Halls, Frosthold | Guardian of the Frozen Halls |
| 2 | Volcano Wyrm | `VolcanoWyrm` | 46 | Dragon | 1358 (fire orange) | Volcano, Emberlands | the Molten Terror |
| 3 | Sphinx of Surya | `SphinxOfSurya` | 788 | Sphinx | 1719 (sandstone) | Ancient ruins, Whispering Sands | Guardian of Ancient Secrets |
| 4 | Coven Matriarch | `CovenMatriarch` | 259 | Swamp Hag | 2073 (murky green) | Deep swamp, Shadowfen | Mistress of the Mire |
| 5 | Ancient Treant | `AncientTreant` | 301 | Treant | 2010 (forest green) | Sacred grove, Verdantpeak | Guardian of the Sacred Grove |
| 6 | Crystal Drake Alpha | `CrystalDrakeAlpha` | 52 | Drake | 1154 (crystal) | Crystal caves, Crystal Barrens | Master of Prismatic Energies |
| 7 | Forge Master | `ForgeMaster` | 752 | Golem | 2305 (metallic) | Deep foundry, Ironclad | The Eternal Smith |
| 8 | Griffin Lord | `GriffinLord` | 5 | Griffin | 1281 (storm blue) | Sky peaks, Skyreach | Lord of the Skies |
| 9 | Ancient Kraken | `AncientKraken` | 77 | Kraken | 1365 (deep blue) | Ocean depths, Underwater | Terror of the Abyss |
| 10 | Timeworn Lich | `TimewornLich` | 24 | Lich | 1109 (void black) | Void realm, ShadowVoid | Master of the Void |

---

## SPAWN COMMANDS

### Ancient Creatures (LLM-Enabled NPCs)
```
[add FrosthelmEternalWinter
[add EmberflameAshenTyrant
[add VerdantheartForestGuardian
[add CrystalwingPrismaticOracle
[add AbyssusDepthKing
[add ElderOakbark
[add IronbarkWarAncient
[add SphynxOfEmberlands
[add TheCrystalSphinx
[add FrostFatherAvatar
[add GreatMachinistConstruct
[add LunaraDryadHerald
```

### Boss Creatures (Combat Encounters)
```
[add FrostFather
[add VolcanoWyrm
[add SphinxOfSurya
[add CovenMatriarch
[add AncientTreant
[add CrystalDrakeAlpha
[add ForgeMaster
[add GriffinLord
[add AncientKraken
[add TimewornLich
```

### Special Commands
```
[SpawnAncientLine          - Spawn all 12 ancient creatures in a line, frozen
[sal                       - Short alias for SpawnAncientLine
[SpawnAncientLine 3        - Spawn with 3-tile spacing (default: 2)
[DiagnoseNPC              - Check if NPC has LLM integration
```

---

## BODY ID REFERENCE

### Common UO Body IDs Used

| Body ID (Dec) | Body ID (Hex) | Creature Type | Usage |
|---------------|---------------|---------------|--------|
| 12 | 0xC | Dragon | White Dragon (ancient) |
| 13 | 0xD | Air Elemental | Spirit/Avatar |
| 24 | 0x18 | Lich | Undead boss |
| 46 | 0x2E | Dragon | Fire dragon |
| 47 | 0x2F | Treant | Tree creature |
| 52 | 0x34 | Drake | Lesser dragon |
| 59 | 0x3B | Dragon | Red dragon |
| 60 | 0x3C | Dragon | Green dragon |
| 76 | 0x4C | Titan | Giant humanoid |
| 77 | 0x4D | Kraken | Sea monster |
| 95 | 0x5F | Sphinx | Riddle creature |
| 150 | 0x96 | Sea Serpent | Sea dragon |
| 259 | 0x103 | Hag | Swamp witch |
| 301 | 0x12D | Treant | Ancient treant |
| 401 | 0x191 | Female Human | Ethereal spirit |
| 752 | 0x2F0 | Golem | Mechanical construct |
| 757 | 0x2F5 | Iron Golem | Divine construct |
| 788 | 0x314 | Sphinx | Desert sphinx |

---

## NOTES

### Ancient Creatures vs Bosses

**Ancient Creatures (Talking NPCs):**
- ✅ Full LLM integration for dynamic conversation
- ✅ Quest givers and lore keepers
- ✅ Non-aggressive (FightMode.Aggressor)
- ✅ Very high karma (good aligned)
- ✅ Can be frozen for display purposes
- ✅ Use `[DiagnoseNPC]` to verify LLM integration

**Boss Creatures (Combat):**
- ⚔️ High-level combat encounters
- ⚔️ Phase-based mechanics
- ⚔️ Summon minions during fight
- ⚔️ Drop legendary artifacts
- ⚔️ Aggressive (FightMode.Closest)
- ⚔️ Designed for group combat

### Important Differences

1. **Sphinx of Surya (Boss)** vs **Sphynx of Emberlands (Ancient)**
   - Boss: Combat encounter, Body 788, aggressive
   - Ancient: LLM NPC, Body 95 (0x5F), conversational

2. **Frost Father (Boss)** vs **Frost Father's Avatar (Ancient)**
   - Boss: Combat titan, Body 76
   - Ancient: Divine spirit, Body 13 (0xD), ethereal

3. **Ancient Treant (Boss)** vs **Elder Oakbark/Ironbark (Ancients)**
   - Boss: Combat encounter, Body 301
   - Ancients: LLM NPCs, Body 47 (0x2F), conversational

---

## LOCATION GUIDE

### By Region

**Frosthold:**
- Frosthelm the Eternal Winter (Ancient)
- Frost Father (Boss)
- Frost Father's Avatar (Ancient)

**Emberlands:**
- Emberflame the Ashen Tyrant (Ancient)
- Sphynx of Emberlands (Ancient)
- Volcano Wyrm (Boss)

**Verdantpeak:**
- Elder Oakbark (Ancient)
- Ironbark the War-Ancient (Ancient)
- Verdantheart the Forest Guardian (Ancient)
- Lunara's Dryad Herald (Ancient)
- Ancient Treant (Boss)

**Whispering Sands / Desert:**
- Sphinx of Surya (Boss)

**Crystal Barrens:**
- Crystalwing the Prismatic Oracle (Ancient)
- The Crystal Sphinx (Ancient)
- Crystal Drake Alpha (Boss)

**Ironclad Empire:**
- The Great Machinist's Construct (Ancient)
- Forge Master (Boss)

**Shadowfen:**
- Coven Matriarch (Boss)

**Skyreach:**
- Griffin Lord (Boss)

**Underwater / Forgotten Depths:**
- Abyssus the Depth King (Ancient)
- Ancient Kraken (Boss)

**ShadowVoid:**
- Timeworn Lich (Boss)

---

**Total Unique Creatures:** 22 (12 Ancient + 10 Bosses)
**All creatures have full LLM integration capability**
**All creatures serialize properly for world persistence**

*Last Updated: 2025-12-09*
