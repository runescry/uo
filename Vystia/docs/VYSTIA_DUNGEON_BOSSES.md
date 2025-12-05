# Vystia Dungeon Bosses Guide

## Overview
Boss index, mechanics, key gating, and reward tables for all Vystia dungeons.

**Implementation Status: ALL 10 BOSSES COMPLETE**

---

## Frozen Halls (Frosthold) - IMPLEMENTED
- **Boss:** Frost Father
- **File:** `Scripts/Mobiles/Vystia/Bosses/FrostFather.cs`
- **Phase 1 (100-66%):** Ice breath, cold aura damage (IAuraCreature)
- **Phase 2 (66-33%):** Summons Frost Wraiths, blizzard AoE
- **Phase 3 (33-0%):** Cone freeze attack, increased damage
- **Key:** Frost Seal (drops from Frost Wraiths)
- **Legendary Drop:** The Eternal Winter (Halberd) - 2% chance
- **Resources:** Frozen Artifacts, FrostforgedIngot, EternalIce

## Emberdeep Caldera (Emberlands) - IMPLEMENTED
- **Boss:** Volcano Wyrm
- **File:** `Scripts/Mobiles/Vystia/Bosses/VolcanoWyrm.cs`
- **Phase 1 (100-66%):** Fire breath, magma pool creation
- **Phase 2 (66-33%):** Summons Fire Elementals, lava zones
- **Phase 3 (33-0%):** Eruption AoE (60-90 damage), fire aura
- **Key:** Caldera Keystone (from Fire Elementals)
- **Legendary Drop:** Phoenix Ascension (Katana) - 2% chance
- **Resources:** EmberforgedIngot, EverburningCoal, LavaPearl

## Pyramid of Surya (Desert) - IMPLEMENTED
- **Boss:** Sphinx of Surya
- **File:** `Scripts/Mobiles/Vystia/Bosses/SphinxOfSurya.cs`
- **Phase 1 (100-66%):** Riddles (debuff on "failure"), sandstorms
- **Phase 2 (66-33%):** Solar beam attacks, summons Sand Elementals
- **Phase 3 (33-0%):** Time warp (slow all), blinding light AoE
- **Key:** Sphinx Sigil (from Desert Mummies)
- **Legendary Drop:** TimeDust - 15% chance
- **Resources:** SunforgedIngot, DesertRose, SandstoneOre

## Shadowfen Crypts (Swamp) - IMPLEMENTED
- **Boss:** Coven Matriarch
- **File:** `Scripts/Mobiles/Vystia/Bosses/CovenMatriarch.cs`
- **Phase 1 (100-66%):** Poison attacks, curse spells (stat drain)
- **Phase 2 (66-33%):** Summons Bog Things, poison clouds (DoT)
- **Phase 3 (33-0%):** Mass curse, life drain (heals self)
- **Key:** Witchbane Charm (from Bog Witches)
- **Legendary Drop:** ShadowSilk - 15% chance
- **Resources:** ShadowforgedIngot, SwampLotus, VoidDust

## Verdant Depths (Forest) - IMPLEMENTED
- **Boss:** Ancient Treant
- **File:** `Scripts/Mobiles/Vystia/Bosses/AncientTreant.cs`
- **Phase 1 (100-66%):** Root attacks (paralyze), vine entangle (slow)
- **Phase 2 (66-33%):** Summons Reapers, nature's wrath AoE
- **Phase 3 (33-0%):** Devastating slam, passive regeneration
- **Key:** Heartwood Rune (from Forest Spirits)
- **Legendary Drop:** TreantHeart - 15% chance
- **Resources:** VerdantforgedIngot, LivingWood, MoonPetal

## Crystal Caverns (Crystal Barrens) - IMPLEMENTED
- **Boss:** Crystal Drake Alpha
- **File:** `Scripts/Mobiles/Vystia/Bosses/CrystalDrakeAlpha.cs`
- **Phase 1 (100-66%):** Crystal shard attacks, reflective shield (35% reflect)
- **Phase 2 (66-33%):** Summons Crystal Elementals, prismatic beams (mana drain)
- **Phase 3 (33-0%):** Crystal shatter AoE, bleeding DoT
- **Key:** Prism Key (from Gemstone Golems)
- **Legendary Drop:** PrismCore - 15% chance
- **Resources:** CrystalforgedIngot, ResonatingCrystal, StarMoss

## Skyreach Summit (Mountain) - IMPLEMENTED
- **Boss:** Griffin Lord
- **File:** `Scripts/Mobiles/Vystia/Bosses/GriffinLord.cs`
- **Phase 1 (100-66%):** Dive attacks (teleport + damage), wind gusts
- **Phase 2 (66-33%):** Summons Air Elementals, lightning strikes (chain in P3)
- **Phase 3 (33-0%):** Tornado AoE (50-75 damage), disorientation debuffs
- **Key:** Sky Sigil (from Harpies)
- **Legendary Drop:** StormFeather - 15% chance
- **Resources:** SkyforgedIngot, SkyIronOre, CloudMoss

## Abyssal Trench (Underwater) - IMPLEMENTED
- **Boss:** Ancient Kraken
- **File:** `Scripts/Mobiles/Vystia/Bosses/AncientKraken.cs`
- **Phase 1 (100-66%):** Tentacle slam (grapple chance), ink cloud (blind)
- **Phase 2 (66-33%):** Summons Sea Serpents, whirlpool (pull + DoT)
- **Phase 3 (33-0%):** Crushing grasp (freeze + DoT), tidal wave transition
- **Key:** Abyssal Pearl (from Deep Sea Serpents)
- **Legendary Drop:** KrakenInk - 15% chance
- **Resources:** AbyssalforgedIngot, DeepCoralOre, AbyssalKelp

## Ironclad Foundry (Ironclad Empire) - IMPLEMENTED
- **Boss:** Forge Master
- **File:** `Scripts/Mobiles/Vystia/Bosses/ForgeMaster.cs`
- **Phase 1 (100-66%):** Hammer strikes (ground shake), molten metal splash
- **Phase 2 (66-33%):** Summons Clockwork Golems, steam vents (blind + DoT)
- **Phase 3 (33-0%):** Mechanical overdrive (temp stat boost), forge explosion
- **Key:** Forge Key (from Clockwork Beasts)
- **Legendary Drop:** The Cogmaster (War Hammer) - 2% chance
- **Resources:** SteelforgedIngot, RefinedIronOre, MechanicalGear, GearCore

## Twilight Labyrinth (Shadow Void) - IMPLEMENTED
- **Boss:** Timeworn Lich
- **File:** `Scripts/Mobiles/Vystia/Bosses/TimewornLich.cs`
- **Phase 1 (100-66%):** Necromantic bolts, soul drain (heals self + mana drain)
- **Phase 2 (66-33%):** Summons Spectres, void storms (random stat drain)
- **Phase 3 (33-0%):** Time stop (4s freeze all), mass soul harvest (60-90 damage)
- **Key:** Twilight Sigil (from Shades)
- **Legendary Drop:** Voidcaller (Quarter Staff) - 2% chance
- **Resources:** VoidforgedIngot, ObsidianShard, VoidEssence, NightshadeEssence

---

## Boss Statistics Summary

| Boss | HP Range | Damage | Fame | Special |
|------|----------|--------|------|---------|
| Frost Father | 1200-1500 | 25-35 | 25000 | IAuraCreature, Dragon Breath |
| Volcano Wyrm | 1500-1800 | 40-50 | 28000 | IAuraCreature, Dragon Breath |
| Sphinx of Surya | 1400-1700 | 30-40 | 26000 | Riddle Mechanic |
| Coven Matriarch | 1300-1600 | 25-35 | 25000 | Life Drain Healing |
| Ancient Treant | 1600-1900 | 35-45 | 27000 | Passive Regeneration |
| Crystal Drake Alpha | 1400-1700 | 30-40 | 26000 | Reflective Shield |
| Griffin Lord | 1350-1650 | 30-42 | 25000 | Dive Teleport |
| Ancient Kraken | 1700-2000 | 35-48 | 28000 | CanSwim, Grapple |
| Forge Master | 1800-2100 | 40-52 | 30000 | BleedImmune, Overdrive |
| Timeworn Lich | 1500-1800 | 28-38 | 32000 | BleedImmune, Time Stop |

---

## Reward Tiers
- **Dungeon Boss:** FilthyRich x2 + Regional Loot Pack (Rich) + Unique drops
- **Legendary Weapons:** 2% drop chance from designated bosses
- **Rare Resources:** 10-15% chance for region-specific rare materials
- **Equipment:** 5% chance for regional armor/weapons

---

## GM Spawn Commands
```
[add FrostFather           - Spawn Frost Father
[add VolcanoWyrm           - Spawn Volcano Wyrm
[add SphinxOfSurya         - Spawn Sphinx of Surya
[add CovenMatriarch        - Spawn Coven Matriarch
[add AncientTreant         - Spawn Ancient Treant
[add CrystalDrakeAlpha     - Spawn Crystal Drake Alpha
[add GriffinLord           - Spawn Griffin Lord
[add AncientKraken         - Spawn Ancient Kraken
[add ForgeMaster           - Spawn Forge Master
[add TimewornLich          - Spawn Timeworn Lich
```

---

## Technical Notes
- **All bosses implement:** 3-phase mechanics with HP thresholds (66%, 33%)
- **Phase transitions:** Visual effects, sound, overhead messages, stat changes
- **Summons:** Phase 2+ summons 2-3 minions; Phase 3 summons 3-4
- **Serialization:** All bosses save/load phase state
- **Keys:** Simple item checks at boss doors; drop rates 10-20%
- **Spawners:** Ensure respawn timers align with boss windows
