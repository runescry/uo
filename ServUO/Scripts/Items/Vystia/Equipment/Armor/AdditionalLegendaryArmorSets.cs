using System;
using Server;

namespace Server.Items.Vystia
{
    // ============================================================================
    // ADDITIONAL LEGENDARY ARMOR SETS (Role-Based)
    //
    // Complete Role Coverage:
    // - Tank: Glacial Aegis (existing)
    // - MeleeDPS: Steamwork Exosuit (existing)
    // - Stealth/Rogue: Shadow Shroud (existing)
    // - Healer: Celestial Raiment (NEW)
    // - RangedDPS: Stormrider Garb (NEW)
    // - CasterDPS: Arcanist Regalia (NEW)
    // - Support: Harmonist Vestments (NEW)
    // - Hybrid: Uses appropriate set based on primary role
    // ============================================================================

    // ============================================================================
    // CELESTIAL RAIMENT
    // Role: Healer
    // Hue: 1153
    // Bonuses: +4 BonusInt, +4 RegenHits, +3 RegenMana
    // ============================================================================

    public class CelestialRaimentCoif : ChainCoif
    {
        [Constructable]
        public CelestialRaimentCoif()
        {
            Name = "Celestial Raiment Coif";
            Hue = 1153;

            // Legendary stats for Healer role
            SkillBonuses.SetValues(0, SkillName.Healing, 5.0);

            Attributes.BonusInt = 4;
            Attributes.RegenHits = 4;
            Attributes.RegenMana = 3;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by the Divine Light");
            list.Add("Set: Celestial Raiment (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public CelestialRaimentCoif(Serial serial) : base(serial) { }

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

    public class CelestialRaimentGorget : PlateGorget
    {
        [Constructable]
        public CelestialRaimentGorget()
        {
            Name = "Celestial Raiment Gorget";
            Hue = 1153;

            // Legendary stats for Healer role
            SkillBonuses.SetValues(0, SkillName.Healing, 5.0);

            Attributes.BonusInt = 4;
            Attributes.RegenHits = 4;
            Attributes.RegenMana = 3;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by the Divine Light");
            list.Add("Set: Celestial Raiment (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public CelestialRaimentGorget(Serial serial) : base(serial) { }

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

    public class CelestialRaimentTunic : ChainChest
    {
        [Constructable]
        public CelestialRaimentTunic()
        {
            Name = "Celestial Raiment Tunic";
            Hue = 1153;

            // Legendary stats for Healer role
            SkillBonuses.SetValues(0, SkillName.Healing, 5.0);

            Attributes.BonusInt = 4;
            Attributes.RegenHits = 4;
            Attributes.RegenMana = 3;

            BaseArmorRating = 54;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by the Divine Light");
            list.Add("Set: Celestial Raiment (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public CelestialRaimentTunic(Serial serial) : base(serial) { }

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

    public class CelestialRaimentSleeves : PlateArms
    {
        [Constructable]
        public CelestialRaimentSleeves()
        {
            Name = "Celestial Raiment Sleeves";
            Hue = 1153;

            // Legendary stats for Healer role
            SkillBonuses.SetValues(0, SkillName.Healing, 5.0);

            Attributes.BonusInt = 4;
            Attributes.RegenHits = 4;
            Attributes.RegenMana = 3;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by the Divine Light");
            list.Add("Set: Celestial Raiment (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public CelestialRaimentSleeves(Serial serial) : base(serial) { }

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

    public class CelestialRaimentGloves : PlateGloves
    {
        [Constructable]
        public CelestialRaimentGloves()
        {
            Name = "Celestial Raiment Gloves";
            Hue = 1153;

            // Legendary stats for Healer role
            SkillBonuses.SetValues(0, SkillName.Healing, 5.0);

            Attributes.BonusInt = 4;
            Attributes.RegenHits = 4;
            Attributes.RegenMana = 3;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by the Divine Light");
            list.Add("Set: Celestial Raiment (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public CelestialRaimentGloves(Serial serial) : base(serial) { }

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

    public class CelestialRaimentLeggings : ChainLegs
    {
        [Constructable]
        public CelestialRaimentLeggings()
        {
            Name = "Celestial Raiment Leggings";
            Hue = 1153;

            // Legendary stats for Healer role
            SkillBonuses.SetValues(0, SkillName.Healing, 5.0);

            Attributes.BonusInt = 4;
            Attributes.RegenHits = 4;
            Attributes.RegenMana = 3;

            BaseArmorRating = 53;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by the Divine Light");
            list.Add("Set: Celestial Raiment (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public CelestialRaimentLeggings(Serial serial) : base(serial) { }

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

    // ============================================================================
    // STORMRIDER GARB
    // Role: RangedDPS
    // Hue: 1165
    // Bonuses: +5 BonusDex, +10 AttackChance, +5 WeaponSpeed
    // ============================================================================

    public class StormriderGarbCap : LeatherCap
    {
        [Constructable]
        public StormriderGarbCap()
        {
            Name = "Stormrider Garb Cap";
            Hue = 1165;

            // Legendary stats for RangedDPS role
            SkillBonuses.SetValues(0, SkillName.Archery, 5.0);

            Attributes.BonusDex = 5;
            Attributes.AttackChance = 10;
            Attributes.WeaponSpeed = 5;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Woven from windswept clouds");
            list.Add("Set: Stormrider Garb (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public StormriderGarbCap(Serial serial) : base(serial) { }

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

    public class StormriderGarbGorget : LeatherGorget
    {
        [Constructable]
        public StormriderGarbGorget()
        {
            Name = "Stormrider Garb Gorget";
            Hue = 1165;

            // Legendary stats for RangedDPS role
            SkillBonuses.SetValues(0, SkillName.Archery, 5.0);

            Attributes.BonusDex = 5;
            Attributes.AttackChance = 10;
            Attributes.WeaponSpeed = 5;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Woven from windswept clouds");
            list.Add("Set: Stormrider Garb (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public StormriderGarbGorget(Serial serial) : base(serial) { }

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

    public class StormriderGarbTunic : LeatherChest
    {
        [Constructable]
        public StormriderGarbTunic()
        {
            Name = "Stormrider Garb Tunic";
            Hue = 1165;

            // Legendary stats for RangedDPS role
            SkillBonuses.SetValues(0, SkillName.Archery, 5.0);

            Attributes.BonusDex = 5;
            Attributes.AttackChance = 10;
            Attributes.WeaponSpeed = 5;

            BaseArmorRating = 54;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Woven from windswept clouds");
            list.Add("Set: Stormrider Garb (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public StormriderGarbTunic(Serial serial) : base(serial) { }

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

    public class StormriderGarbSleeves : LeatherArms
    {
        [Constructable]
        public StormriderGarbSleeves()
        {
            Name = "Stormrider Garb Sleeves";
            Hue = 1165;

            // Legendary stats for RangedDPS role
            SkillBonuses.SetValues(0, SkillName.Archery, 5.0);

            Attributes.BonusDex = 5;
            Attributes.AttackChance = 10;
            Attributes.WeaponSpeed = 5;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Woven from windswept clouds");
            list.Add("Set: Stormrider Garb (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public StormriderGarbSleeves(Serial serial) : base(serial) { }

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

    public class StormriderGarbGloves : LeatherGloves
    {
        [Constructable]
        public StormriderGarbGloves()
        {
            Name = "Stormrider Garb Gloves";
            Hue = 1165;

            // Legendary stats for RangedDPS role
            SkillBonuses.SetValues(0, SkillName.Archery, 5.0);

            Attributes.BonusDex = 5;
            Attributes.AttackChance = 10;
            Attributes.WeaponSpeed = 5;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Woven from windswept clouds");
            list.Add("Set: Stormrider Garb (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public StormriderGarbGloves(Serial serial) : base(serial) { }

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

    public class StormriderGarbLeggings : LeatherLegs
    {
        [Constructable]
        public StormriderGarbLeggings()
        {
            Name = "Stormrider Garb Leggings";
            Hue = 1165;

            // Legendary stats for RangedDPS role
            SkillBonuses.SetValues(0, SkillName.Archery, 5.0);

            Attributes.BonusDex = 5;
            Attributes.AttackChance = 10;
            Attributes.WeaponSpeed = 5;

            BaseArmorRating = 53;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Woven from windswept clouds");
            list.Add("Set: Stormrider Garb (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public StormriderGarbLeggings(Serial serial) : base(serial) { }

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

    // ============================================================================
    // ARCANIST REGALIA
    // Role: CasterDPS
    // Hue: 1154
    // Bonuses: +5 BonusInt, +10 SpellDamage, +3 RegenMana
    // ============================================================================

    public class ArcanistRegaliaCap : LeatherCap
    {
        [Constructable]
        public ArcanistRegaliaCap()
        {
            Name = "Arcanist Regalia Cap";
            Hue = 1154;

            // Legendary stats for CasterDPS role
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 10;
            Attributes.RegenMana = 3;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Infused with pure arcane essence");
            list.Add("Set: Arcanist Regalia (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ArcanistRegaliaCap(Serial serial) : base(serial) { }

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

    public class ArcanistRegaliaGorget : LeatherGorget
    {
        [Constructable]
        public ArcanistRegaliaGorget()
        {
            Name = "Arcanist Regalia Gorget";
            Hue = 1154;

            // Legendary stats for CasterDPS role
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 10;
            Attributes.RegenMana = 3;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Infused with pure arcane essence");
            list.Add("Set: Arcanist Regalia (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ArcanistRegaliaGorget(Serial serial) : base(serial) { }

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

    public class ArcanistRegaliaTunic : LeatherChest
    {
        [Constructable]
        public ArcanistRegaliaTunic()
        {
            Name = "Arcanist Regalia Tunic";
            Hue = 1154;

            // Legendary stats for CasterDPS role
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 10;
            Attributes.RegenMana = 3;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Infused with pure arcane essence");
            list.Add("Set: Arcanist Regalia (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ArcanistRegaliaTunic(Serial serial) : base(serial) { }

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

    public class ArcanistRegaliaSleeves : LeatherArms
    {
        [Constructable]
        public ArcanistRegaliaSleeves()
        {
            Name = "Arcanist Regalia Sleeves";
            Hue = 1154;

            // Legendary stats for CasterDPS role
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 10;
            Attributes.RegenMana = 3;

            BaseArmorRating = 49;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Infused with pure arcane essence");
            list.Add("Set: Arcanist Regalia (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ArcanistRegaliaSleeves(Serial serial) : base(serial) { }

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

    public class ArcanistRegaliaGloves : LeatherGloves
    {
        [Constructable]
        public ArcanistRegaliaGloves()
        {
            Name = "Arcanist Regalia Gloves";
            Hue = 1154;

            // Legendary stats for CasterDPS role
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 10;
            Attributes.RegenMana = 3;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Infused with pure arcane essence");
            list.Add("Set: Arcanist Regalia (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ArcanistRegaliaGloves(Serial serial) : base(serial) { }

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

    public class ArcanistRegaliaLeggings : LeatherLegs
    {
        [Constructable]
        public ArcanistRegaliaLeggings()
        {
            Name = "Arcanist Regalia Leggings";
            Hue = 1154;

            // Legendary stats for CasterDPS role
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 10;
            Attributes.RegenMana = 3;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Infused with pure arcane essence");
            list.Add("Set: Arcanist Regalia (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ArcanistRegaliaLeggings(Serial serial) : base(serial) { }

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

    // ============================================================================
    // HARMONIST VESTMENTS
    // Role: Support
    // Hue: 2010
    // Bonuses: +2 BonusStr, +2 BonusDex, +2 BonusInt, +50 Luck
    // ============================================================================

    public class HarmonistVestmentsCap : LeatherCap
    {
        [Constructable]
        public HarmonistVestmentsCap()
        {
            Name = "Harmonist Vestments Cap";
            Hue = 2010;

            // Legendary stats for Support role
            SkillBonuses.SetValues(0, SkillName.Musicianship, 5.0);

            Attributes.BonusStr = 2;
            Attributes.BonusDex = 2;
            Attributes.BonusInt = 2;
            Attributes.Luck = 50;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Crafted to enhance all who stand near");
            list.Add("Set: Harmonist Vestments (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public HarmonistVestmentsCap(Serial serial) : base(serial) { }

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

    public class HarmonistVestmentsGorget : LeatherGorget
    {
        [Constructable]
        public HarmonistVestmentsGorget()
        {
            Name = "Harmonist Vestments Gorget";
            Hue = 2010;

            // Legendary stats for Support role
            SkillBonuses.SetValues(0, SkillName.Musicianship, 5.0);

            Attributes.BonusStr = 2;
            Attributes.BonusDex = 2;
            Attributes.BonusInt = 2;
            Attributes.Luck = 50;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Crafted to enhance all who stand near");
            list.Add("Set: Harmonist Vestments (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public HarmonistVestmentsGorget(Serial serial) : base(serial) { }

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

    public class HarmonistVestmentsTunic : LeatherChest
    {
        [Constructable]
        public HarmonistVestmentsTunic()
        {
            Name = "Harmonist Vestments Tunic";
            Hue = 2010;

            // Legendary stats for Support role
            SkillBonuses.SetValues(0, SkillName.Musicianship, 5.0);

            Attributes.BonusStr = 2;
            Attributes.BonusDex = 2;
            Attributes.BonusInt = 2;
            Attributes.Luck = 50;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Crafted to enhance all who stand near");
            list.Add("Set: Harmonist Vestments (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public HarmonistVestmentsTunic(Serial serial) : base(serial) { }

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

    public class HarmonistVestmentsSleeves : LeatherArms
    {
        [Constructable]
        public HarmonistVestmentsSleeves()
        {
            Name = "Harmonist Vestments Sleeves";
            Hue = 2010;

            // Legendary stats for Support role
            SkillBonuses.SetValues(0, SkillName.Musicianship, 5.0);

            Attributes.BonusStr = 2;
            Attributes.BonusDex = 2;
            Attributes.BonusInt = 2;
            Attributes.Luck = 50;

            BaseArmorRating = 49;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Crafted to enhance all who stand near");
            list.Add("Set: Harmonist Vestments (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public HarmonistVestmentsSleeves(Serial serial) : base(serial) { }

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

    public class HarmonistVestmentsGloves : LeatherGloves
    {
        [Constructable]
        public HarmonistVestmentsGloves()
        {
            Name = "Harmonist Vestments Gloves";
            Hue = 2010;

            // Legendary stats for Support role
            SkillBonuses.SetValues(0, SkillName.Musicianship, 5.0);

            Attributes.BonusStr = 2;
            Attributes.BonusDex = 2;
            Attributes.BonusInt = 2;
            Attributes.Luck = 50;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Crafted to enhance all who stand near");
            list.Add("Set: Harmonist Vestments (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public HarmonistVestmentsGloves(Serial serial) : base(serial) { }

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

    public class HarmonistVestmentsLeggings : LeatherLegs
    {
        [Constructable]
        public HarmonistVestmentsLeggings()
        {
            Name = "Harmonist Vestments Leggings";
            Hue = 2010;

            // Legendary stats for Support role
            SkillBonuses.SetValues(0, SkillName.Musicianship, 5.0);

            Attributes.BonusStr = 2;
            Attributes.BonusDex = 2;
            Attributes.BonusInt = 2;
            Attributes.Luck = 50;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Crafted to enhance all who stand near");
            list.Add("Set: Harmonist Vestments (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public HarmonistVestmentsLeggings(Serial serial) : base(serial) { }

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
