# VYSTIA CREATURES IMPLEMENTATION GUIDE

Complete creature implementation guide for the Vystia world system using **ALL UO expansion body IDs**.

**Last Updated:** December 2024

---

## IMPLEMENTATION STATUS

| Category | Designed | Implemented | Status |
|----------|----------|-------------|--------|
| Bosses | 10 | 1 | 🟡 10% |
| Frosthold Creatures | 13 | 0 | ❌ Not Started |
| Emberlands Creatures | 9 | 0 | ❌ Not Started |
| Other Regions | 118 | 0 | ❌ Not Started |
| **Total** | **150** | **1** | **<1%** |

**Implemented Creatures:**
- ✅ `FrostFather.cs` - Frosthold boss (3-phase mechanics, cold aura, summons)

**Next Phase:** Phase 4 - Regional Creatures (see `docs/VYSTIA_IMPLEMENTATION_PLAN.md`)

---

## Overview

The Vystia creature system includes **150+ unique creatures** across all regions, using body IDs from T2A through Stygian Abyss for maximum visual variety.

**Master Reference:** See `docs/VYSTIA_CREATURES_BESTIARY_FULL.md` for complete creature database.

**IMPORTANT:** Only spawn creatures listed in the bestiary. Generic UO monsters (Orcs, Ettins, Ratmen, etc.) are NOT thematically appropriate for Vystia.

---

## File Structure

### Creature Implementation Files (To Be Created)
```
ServUO/Scripts/Mobiles/Vystia/
├── Frosthold/
│   ├── FrostGiant.cs
│   ├── WinterWolf.cs
│   ├── IceTroll.cs
│   ├── FrostWraith.cs
│   ├── FrozenHorror.cs
│   ├── FrostDragon.cs
│   ├── IceSprite.cs
│   ├── IceGolem.cs
│   ├── GlacialBear.cs
│   ├── IceElemental.cs
│   ├── GlacialStalker.cs
│   ├── BorealSerpent.cs
│   └── FrostSalamander.cs
├── Emberlands/
│   ├── LavaHound.cs
│   ├── MagmaTroll.cs
│   ├── Phoenix.cs
│   ├── VolcanoWyrm.cs
│   ├── EmberBat.cs
│   ├── AshGolem.cs
│   ├── FlameSprite.cs
│   ├── LavaElemental.cs
│   └── FireAnt.cs
├── WhisperingSands/
│   ├── SandElemental.cs
│   ├── DesertMummy.cs
│   ├── GiantScorpion.cs
│   ├── Sphinx.cs
│   ├── DesertHarpy.cs
│   ├── Ankheg.cs
│   ├── SandVortex.cs
│   ├── ScarabBeetle.cs
│   ├── Lamia.cs
│   ├── BlueDragon.cs
│   ├── Roc.cs
│   └── Genie.cs
├── Shadowfen/
│   ├── SwampTroll.cs
│   ├── BogWitch.cs
│   ├── Mistwalker.cs
│   ├── BogBeast.cs
│   ├── ShadowWolf.cs
│   ├── Bogling.cs
│   ├── PlagueBeast.cs
│   ├── Quagmire.cs
│   ├── ShadowSerpent.cs
│   ├── MarshSpirit.cs
│   ├── SwampGhoul.cs
│   ├── MudGolem.cs
│   ├── FenDrake.cs
│   └── Boghound.cs
├── Verdantpeak/
│   ├── Treant.cs
│   ├── DireWolf.cs
│   ├── ForestSprite.cs
│   ├── GiantSpider.cs
│   ├── ShadowDryad.cs
│   ├── CorruptedTreant.cs
│   ├── ThornBeast.cs
│   ├── WolfSpider.cs
│   ├── FairyDragon.cs
│   ├── Dryad.cs
│   ├── Owlbear.cs
│   ├── ForestHag.cs
│   ├── ForestGoblin.cs
│   ├── DarkUnicorn.cs
│   └── GreenDragon.cs
├── CrystalBarrens/
│   ├── CrystalDrake.cs
│   ├── GemstoneGolem.cs
│   ├── LightElemental.cs
│   ├── CrystalSpider.cs
│   └── PrismaticWyrm.cs
├── Deepforge/
│   ├── CaveTroll.cs
│   ├── StoneGiant.cs
│   ├── Salamander.cs
│   ├── FireElemental.cs
│   ├── MagmaGolem.cs
│   ├── ClockworkScorpion.cs
│   ├── RustMonster.cs
│   ├── GelatinousCube.cs
│   ├── MindFlayer.cs
│   ├── Beholder.cs
│   ├── AcidicOoze.cs
│   └── Kobold.cs
├── IroncladEmpire/
│   ├── IronGolem.cs
│   ├── ClockworkBeast.cs
│   ├── SteamworkGolem.cs
│   ├── MechanicalSpider.cs
│   ├── SteamElemental.cs
│   ├── Juggernaut.cs
│   ├── SteamMephit.cs
│   ├── Chimera.cs
│   ├── Wyvern.cs
│   └── Mimic.cs
├── SkyMountain/
│   ├── Griffin.cs
│   ├── Harpy.cs
│   ├── StoneHarpy.cs
│   ├── AirElemental.cs
│   ├── Gargoyle.cs
│   ├── StygianGargoyle.cs
│   ├── WindDancer.cs
│   ├── StormCrow.cs
│   ├── ThunderBird.cs
│   ├── StormElemental.cs
│   ├── LightningGolem.cs
│   ├── CloudGiant.cs
│   ├── HighpeakYeti.cs
│   ├── SkySerpent.cs
│   └── WindElemental.cs
├── Underwater/
│   ├── SeaSerpent.cs
│   ├── DeepSeaSerpent.cs
│   ├── Kraken.cs
│   ├── WaterElemental.cs
│   ├── DeepDweller.cs
│   ├── DrownedDead.cs
│   ├── Leviathan.cs
│   ├── MerfolkWarrior.cs
│   ├── ReefGuardian.cs
│   ├── Siren.cs
│   ├── CoralGolem.cs
│   └── SeaWitch.cs
├── ShadowVoid/
│   ├── ShadowWisp.cs
│   ├── Shade.cs
│   ├── ShadowKnight.cs
│   ├── AncientLich.cs
│   ├── ShadowFiend.cs
│   ├── VoidWalker.cs
│   ├── DarkMage.cs
│   ├── PrimevalLich.cs
│   └── SlasherOfVeils.cs
├── EternalTwilight/
│   ├── DuskStalker.cs
│   ├── Nightmare.cs
│   ├── TimeWraith.cs
│   ├── StarBeast.cs
│   ├── EveningDragon.cs
│   └── TwilightShade.cs
└── Bosses/
    ├── FrostFather.cs
    ├── DeepforgeBehemoth.cs
    ├── SphinxOfSurya.cs
    ├── VolcanoWyrmBoss.cs
    ├── CovenMatriarch.cs
    ├── AncientTreant.cs
    ├── CrystalDrakeAlpha.cs
    ├── GriffinLord.cs
    ├── AncientKraken.cs
    └── TimewornLich.cs
```

---

## Key Expansion Body IDs

This system uses body IDs from ALL expansions:

| Expansion | Body Range | Key Bodies |
|-----------|------------|------------|
| T2A | 1-200 | Dragons (12), Elementals (13-16), Wolves (23), Lich (24), Titan (76) |
| Third Dawn | 301-320 | Reaper (301), Shadow Knight (311), Giant Bat (317), Watcher (318), Golem (752) |
| LBR | 700-790 | Juggernaut (768), Merfolk (773), Bog Things (779-780), Ant Lion (787), Stone Harpy (788) |
| SA | 800-850 | Lava Elemental (720), Wolf Spider (736), Dream Wraith (740), Slasher (741), Stygian Dragon (826), Primeval Lich (830) |

**See bestiary for complete body ID reference table.**

---

## Hue Palettes by Region

| Region | Hue Range | Colors |
|--------|-----------|--------|
| Frosthold | 1150-1153 | Ice blue, snow white, pale ice, frost white |
| Emberlands | 1358-1361 | Fire orange, molten orange, deep red, ember red |
| Whispering Sands | 2304-2309 | Sand yellow, sandy tan, sandstone, desert brown |
| Shadowfen | 2212-2214 | Swamp green, bog brown, fog grey |
| Verdantpeak | 2010-2012 | Forest green, dark forest, leaf green |
| Crystal Barrens | 1154-1156 | Crystal blue, prismatic, bright white |
| Deepforge | 1109-1110 | Stone grey, granite |
| Ironclad Empire | 2401-2407 | Bronze, iron grey, copper |
| Shadow/Void | 1109 | Shadow black (consistent) |
| Underwater | 1363-1367 | Coral, sea blue, deep sea |
| Eternal Twilight | 1175 | Twilight purple |
| Skyreach | 1001, 1109 | Cloud white, storm grey |

---

## Loot Tables (Standard LootPack)

| Creature Tier | Loot Pack | Examples |
|---------------|-----------|----------|
| Weak (HP < 50) | `LootPack.Poor` or `LootPack.Meager` | Bats, sprites, goblins |
| Average (HP 50-150) | `LootPack.Average` | Wolves, trolls, elementals |
| Strong (HP 150-350) | `LootPack.Rich` | Giants, drakes, treants |
| Elite (HP 350-600) | `LootPack.FilthyRich` | Dragons, wyrms, liches |
| Bosses (HP 600+) | `LootPack.FilthyRich` x3, `LootPack.Gems` x12 | Dungeon bosses |

**Scrolls:** Add `LootPack.MedScrolls` for magic-using creatures.

---

## Stat Templates

### Boss Creatures (HP 800-1500)
```csharp
SetStr(600, 800);
SetDex(80, 150);
SetInt(200, 400);
SetHits(1000, 1500);
SetDamage(25, 45);
VirtualArmor = 70;

AddLoot(LootPack.FilthyRich, 3);
AddLoot(LootPack.Gems, 12);
```

### Giant Creatures (HP 400-800)
```csharp
SetStr(400, 500);
SetDex(60, 80);
SetInt(40, 80);
SetHits(600, 900);
SetDamage(20, 30);
VirtualArmor = 50;

AddLoot(LootPack.FilthyRich);
AddLoot(LootPack.Gems, 8);
```

### Strong Creatures (HP 150-400)
```csharp
SetStr(200, 300);
SetDex(60, 100);
SetInt(50, 100);
SetHits(200, 350);
SetDamage(12, 22);
VirtualArmor = 40;

AddLoot(LootPack.Rich);
AddLoot(LootPack.MedScrolls);  // If magic user
```

### Medium Creatures (HP 80-200)
```csharp
SetStr(100, 180);
SetDex(70, 100);
SetInt(30, 60);
SetHits(100, 180);
SetDamage(10, 18);
VirtualArmor = 30;

AddLoot(LootPack.Average);
```

### Small Creatures (HP 30-100)
```csharp
SetStr(50, 100);
SetDex(80, 120);
SetInt(20, 50);
SetHits(50, 100);
SetDamage(5, 12);
VirtualArmor = 25;

AddLoot(LootPack.Meager);
```

### Weak Creatures (HP < 50)
```csharp
SetStr(30, 60);
SetDex(80, 130);
SetInt(10, 30);
SetHits(30, 50);
SetDamage(5, 10);
VirtualArmor = 15;

AddLoot(LootPack.Poor);
```

---

## Example Implementations

### Frost Giant (Using Titan Body 76)
```csharp
using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a frost giant corpse")]
    public class FrostGiant : BaseCreature
    {
        [Constructable]
        public FrostGiant() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a frost giant";
            Body = 76;          // Titan
            Hue = 1152;         // Ice blue
            BaseSoundID = 609;

            SetStr(400, 500);
            SetDex(60, 80);
            SetInt(40, 60);

            SetHits(800, 1000);
            SetDamage(25, 35);

            SetDamageType(ResistanceType.Physical, 75);
            SetDamageType(ResistanceType.Cold, 25);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 15, 25);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 80.0, 100.0);
            SetSkill(SkillName.Tactics, 90.0, 100.0);
            SetSkill(SkillName.Wrestling, 90.0, 100.0);

            Fame = 12000;
            Karma = -12000;
            VirtualArmor = 48;

            Tamable = false;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.Gems, 8);
        }

        public override bool Unprovokable => true;
        public override Poison PoisonImmune => Poison.Regular;

        public FrostGiant(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
```

### Treant (Using Reaper Body 301 - Third Dawn)
```csharp
using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a treant corpse")]
    public class Treant : BaseCreature
    {
        [Constructable]
        public Treant() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a treant";
            Body = 301;         // Reaper (Third Dawn) - tree-like appearance
            Hue = 2010;         // Forest green
            BaseSoundID = 442;

            SetStr(300, 400);
            SetDex(30, 50);
            SetInt(100, 150);

            SetHits(300, 400);
            SetDamage(20, 25);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 10, 20);  // Vulnerable to fire
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 100);   // Immune to poison
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 70.0, 90.0);
            SetSkill(SkillName.Tactics, 80.0, 100.0);
            SetSkill(SkillName.Wrestling, 80.0, 100.0);

            Fame = 8000;
            Karma = 8000;  // Good creature
            VirtualArmor = 45;

            Tamable = false;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Gems, 5);
        }

        public override Poison PoisonImmune => Poison.Lethal;

        public Treant(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
```

### Mind Flayer (Using Watcher Body 318 - Third Dawn)
```csharp
using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a mind flayer corpse")]
    public class MindFlayer : BaseCreature
    {
        [Constructable]
        public MindFlayer() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a mind flayer";
            Body = 318;         // Watcher (Third Dawn) - tentacled appearance
            Hue = 1367;         // Deep purple
            BaseSoundID = 0x482;

            SetStr(150, 200);
            SetDex(80, 100);
            SetInt(300, 400);

            SetHits(200, 280);
            SetDamage(12, 18);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.EvalInt, 100.0, 120.0);
            SetSkill(SkillName.Magery, 100.0, 120.0);
            SetSkill(SkillName.MagicResist, 90.0, 110.0);
            SetSkill(SkillName.Meditation, 80.0, 100.0);
            SetSkill(SkillName.Tactics, 70.0, 90.0);
            SetSkill(SkillName.Wrestling, 60.0, 80.0);

            Fame = 10000;
            Karma = -10000;
            VirtualArmor = 35;

            Tamable = false;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.MedScrolls, 3);
        }

        // TODO: Add Mind Blast special attack

        public MindFlayer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
```

---

## Creature Statistics Summary

### By Region

| Region | Creatures | Tameable |
|--------|-----------|----------|
| Frosthold/Winterguard | 13 | 3 (Winter Wolf, Glacial Stalker, Boreal Serpent) |
| Emberlands | 9 | 2 (Lava Hound, Fire Ant) |
| Whispering Sands | 12 | 2 (Giant Scorpion, Scarab Beetle) |
| Shadowfen | 14 | 3 (Shadow Wolf, Shadow Serpent, Boghound) |
| Verdantpeak | 15 | 4 (Dire Wolf, Giant Spider, Owlbear, Wolf Spider) |
| Crystal Barrens | 5 | 0 |
| Deepforge | 12 | 0 |
| Ironclad Empire | 10 | 1 (Wyvern) |
| Skyreach Mountains | 16 | 2 (Griffin, Storm Crow) |
| Underwater/Sunken Isles | 12 | 0 |
| Shadow/Void | 10 | 0 |
| Eternal Twilight | 6 | 2 (Dusk Stalker, Nightmare) |
| **Bosses** | 10 | 0 |
| **Total** | **150+** | **25+** |

---

## Implementation Roadmap

### Phase 1: Create Creature Files
- [ ] Create folder structure under `Scripts/Mobiles/Vystia/`
- [ ] Implement Frosthold creatures (13)
- [ ] Implement Emberlands creatures (9)
- [ ] Implement Whispering Sands creatures (12)
- [ ] Implement Shadowfen creatures (14)
- [ ] Implement Verdantpeak creatures (15)
- [ ] Implement remaining regional creatures
- [ ] Implement dungeon bosses (10)

### Phase 2: Spawner System
- [ ] Create `VystiaDungeonSpawners.cs`
- [ ] Implement `[GenVystiaSpawners]` command
- [ ] Configure spawn locations for each dungeon
- [ ] Test spawn rates and difficulty balance

### Phase 3: Special Abilities
- [ ] Cold/Fire/Poison damage auras
- [ ] Boss phase mechanics
- [ ] Pack tactics for wolves
- [ ] Riddle system for Sphinx
- [ ] Mind blast for Mind Flayer
- [ ] Eye rays for Beholder
- [ ] Rust attack for Rust Monster

---

## ServUO Special Ability Implementation

### Using IAuraCreature for Elemental Auras

```csharp
using Server.Mobiles;

namespace Server.Mobiles
{
    public class FrostGiant : BaseCreature, IAuraCreature
    {
        public virtual void AuraEffect(Mobile m)
        {
            // Cold aura damages nearby enemies
            m.SendLocalizedMessage(1008111, false, Name); // Frozen by aura
            AOS.Damage(m, this, Utility.RandomMinMax(5, 10), 0, 0, 100, 0, 0);
            m.PlaySound(0x1FB); // Frost sound
        }

        // ... rest of constructor
    }
}
```

### Using DragonBreath for Breath Attacks

```csharp
// In creature constructor, enable breath
HasBreath = true;
BreathComputeDamage = BreathDamageCalc; // Custom damage calc

// Override breath properties
public override int BreathPhysicalDamage => 0;
public override int BreathFireDamage => 0;
public override int BreathColdDamage => 100;  // Frost breath
public override int BreathPoisonDamage => 0;
public override int BreathEnergyDamage => 0;
public override int BreathEffectHue => 1152;  // Ice blue
public override int BreathEffectSound => 0x64F; // Ice sound
```

### Boss Phase Mechanics

```csharp
private int m_Phase = 1;
private DateTime m_NextPhaseCheck;

public override void OnThink()
{
    base.OnThink();

    if (DateTime.UtcNow >= m_NextPhaseCheck)
    {
        m_NextPhaseCheck = DateTime.UtcNow + TimeSpan.FromSeconds(5);

        // Phase transitions based on HP
        if (m_Phase == 1 && Hits < HitsMax * 0.66)
        {
            m_Phase = 2;
            Say("*roars with fury*");
            // Summon adds
            SpawnMinions(2);
        }
        else if (m_Phase == 2 && Hits < HitsMax * 0.33)
        {
            m_Phase = 3;
            Say("*unleashes full power*");
            // Enrage
            SetDamage(DamageMin + 5, DamageMax + 10);
        }
    }
}

private void SpawnMinions(int count)
{
    for (int i = 0; i < count; i++)
    {
        BaseCreature minion = new FrostWraith();
        minion.MoveToWorld(Location, Map);
        minion.Combatant = Combatant;
    }
}
```

### Pack Tactics Implementation

```csharp
public override void OnThink()
{
    base.OnThink();

    // Check for nearby pack members
    int packBonus = 0;
    foreach (Mobile m in GetMobilesInRange(5))
    {
        if (m is WinterWolf && m != this && !m.Deleted)
            packBonus++;
    }

    // Apply pack damage bonus (max +3)
    packBonus = Math.Min(packBonus, 3);
    if (packBonus > 0 && Combatant != null)
    {
        DamageMin = BaseDamageMin + packBonus;
        DamageMax = BaseDamageMax + packBonus;
    }
}
```

### Using SpecialAbility System

```csharp
using Server.Items;

// In creature constructor
SetSpecialAbility(SpecialAbility.VenomousBite);  // Poison attack
SetSpecialAbility(SpecialAbility.LifeLeech);     // Life drain
SetSpecialAbility(SpecialAbility.TrueFear);      // Fear effect
```

### Riddle System for Sphinx

```csharp
public override void OnDoubleClick(Mobile from)
{
    if (from.InRange(Location, 3))
    {
        from.SendGump(new SphinxRiddleGump(this, from));
    }
}

// In SphinxRiddleGump.cs
public class SphinxRiddleGump : Gump
{
    private Sphinx m_Sphinx;
    private Mobile m_From;
    private static string[] Riddles = new string[]
    {
        "What has keys but no locks?",
        "What can travel around the world while staying in a corner?",
        "The more you take, the more you leave behind. What am I?"
    };
    private static string[] Answers = new string[]
    {
        "piano", "stamp", "footsteps"
    };

    // Implement gump with text entry and validation
}
```

---

## ServUO Existing Creatures as Templates

Use these existing ServUO creatures as reference implementations:

| Vystia Creature | ServUO Template | Location |
|-----------------|-----------------|----------|
| Frost Dragon | `FrostDragon.cs` | Scripts/Mobiles/Normal/ |
| Phoenix | `Phoenix.cs` | Scripts/Mobiles/Normal/ |
| Ice Elemental | `IceElemental.cs` | Scripts/Mobiles/Normal/ |
| Kraken | `Kraken.cs` | Scripts/Mobiles/Normal/ |
| Ancient Lich | `AncientLich.cs` | Scripts/Mobiles/Normal/ |
| Nightmare | `Nightmare.cs` | Scripts/Mobiles/Normal/ |
| Golem types | `Golem.cs` | Scripts/Mobiles/Normal/ |
| Bog Thing | `BogThing.cs` | Scripts/Mobiles/Normal/ |
| Wolf Spider | `WolfSpider.cs` | Scripts/Mobiles/Normal/ |
| Dream Wraith | `DreamWraith.cs` | Scripts/Mobiles/Normal/ |

---

## References

- **Full Bestiary:** `docs/VYSTIA_CREATURES_BESTIARY_FULL.md`
- **World Lore:** `docs/VYSTIA_WORLD_LORE.md`
- **Dungeon Bosses:** `docs/VYSTIA_DUNGEON_BOSSES.md`
- **ServUO SpecialAbility:** `Scripts/Services/Pet Training/SpecialAbility.cs`
- **ServUO BaseCreature:** `Scripts/Mobiles/Normal/BaseCreature.cs`
- **ServUO LootPack:** `Scripts/Misc/LootPack.cs`
