using System;
using Server;

namespace Server.Items.Vystia
{
    // ============================================================================
    // CLASS-SPECIFIC LEGENDARY ARMOR SETS
    //
    // Each of the 26 Vystia classes has its own unique legendary armor set.
    // Sets are themed to match class abilities and playstyle.
    //
    // Total: 26 classes x 6 pieces = 156 items
    // ============================================================================

    // ============================================================================
    // ICE MAGE LEGENDARY SET
    // Base: Leather
    // Hue: 1152
    // Bonuses: +5 BonusInt, +8 SpellDamage, +3 RegenMana
    // ============================================================================

    public class IceMageLegendaryCap : LeatherCap
    {
        [Constructable]
        public IceMageLegendaryCap()
        {
            Name = "Ice Mage Legendary Cap";
            Hue = 1152;

            // Legendary Ice Mage stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 8;
            Attributes.RegenMana = 3;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Crystallized from eternal ice");
            list.Add("Set: Ice Mage Legendary (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public IceMageLegendaryCap(Serial serial) : base(serial) { }

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

    public class IceMageLegendaryGorget : LeatherGorget
    {
        [Constructable]
        public IceMageLegendaryGorget()
        {
            Name = "Ice Mage Legendary Gorget";
            Hue = 1152;

            // Legendary Ice Mage stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 8;
            Attributes.RegenMana = 3;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Crystallized from eternal ice");
            list.Add("Set: Ice Mage Legendary (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public IceMageLegendaryGorget(Serial serial) : base(serial) { }

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

    public class IceMageLegendaryTunic : LeatherChest
    {
        [Constructable]
        public IceMageLegendaryTunic()
        {
            Name = "Ice Mage Legendary Tunic";
            Hue = 1152;

            // Legendary Ice Mage stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 8;
            Attributes.RegenMana = 3;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Crystallized from eternal ice");
            list.Add("Set: Ice Mage Legendary (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public IceMageLegendaryTunic(Serial serial) : base(serial) { }

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

    public class IceMageLegendarySleeves : LeatherArms
    {
        [Constructable]
        public IceMageLegendarySleeves()
        {
            Name = "Ice Mage Legendary Sleeves";
            Hue = 1152;

            // Legendary Ice Mage stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 8;
            Attributes.RegenMana = 3;

            BaseArmorRating = 49;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Crystallized from eternal ice");
            list.Add("Set: Ice Mage Legendary (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public IceMageLegendarySleeves(Serial serial) : base(serial) { }

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

    public class IceMageLegendaryGloves : LeatherGloves
    {
        [Constructable]
        public IceMageLegendaryGloves()
        {
            Name = "Ice Mage Legendary Gloves";
            Hue = 1152;

            // Legendary Ice Mage stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 8;
            Attributes.RegenMana = 3;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Crystallized from eternal ice");
            list.Add("Set: Ice Mage Legendary (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public IceMageLegendaryGloves(Serial serial) : base(serial) { }

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

    public class IceMageLegendaryLeggings : LeatherLegs
    {
        [Constructable]
        public IceMageLegendaryLeggings()
        {
            Name = "Ice Mage Legendary Leggings";
            Hue = 1152;

            // Legendary Ice Mage stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 8;
            Attributes.RegenMana = 3;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Crystallized from eternal ice");
            list.Add("Set: Ice Mage Legendary (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public IceMageLegendaryLeggings(Serial serial) : base(serial) { }

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
    // DRUID LEGENDARY SET
    // Base: Leather
    // Hue: 2010
    // Bonuses: +4 BonusInt, +2 BonusDex, +3 RegenHits
    // ============================================================================

    public class DruidLegendaryCap : LeatherCap
    {
        [Constructable]
        public DruidLegendaryCap()
        {
            Name = "Druid Legendary Cap";
            Hue = 2010;

            // Legendary Druid stats
            SkillBonuses.SetValues(0, SkillName.AnimalLore, 5.0);

            Attributes.BonusInt = 4;
            Attributes.BonusDex = 2;
            Attributes.RegenHits = 3;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Grown from the Living Forest");
            list.Add("Set: Druid Legendary (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public DruidLegendaryCap(Serial serial) : base(serial) { }

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

    public class DruidLegendaryGorget : LeatherGorget
    {
        [Constructable]
        public DruidLegendaryGorget()
        {
            Name = "Druid Legendary Gorget";
            Hue = 2010;

            // Legendary Druid stats
            SkillBonuses.SetValues(0, SkillName.AnimalLore, 5.0);

            Attributes.BonusInt = 4;
            Attributes.BonusDex = 2;
            Attributes.RegenHits = 3;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Grown from the Living Forest");
            list.Add("Set: Druid Legendary (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public DruidLegendaryGorget(Serial serial) : base(serial) { }

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

    public class DruidLegendaryTunic : LeatherChest
    {
        [Constructable]
        public DruidLegendaryTunic()
        {
            Name = "Druid Legendary Tunic";
            Hue = 2010;

            // Legendary Druid stats
            SkillBonuses.SetValues(0, SkillName.AnimalLore, 5.0);

            Attributes.BonusInt = 4;
            Attributes.BonusDex = 2;
            Attributes.RegenHits = 3;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Grown from the Living Forest");
            list.Add("Set: Druid Legendary (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public DruidLegendaryTunic(Serial serial) : base(serial) { }

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

    public class DruidLegendarySleeves : LeatherArms
    {
        [Constructable]
        public DruidLegendarySleeves()
        {
            Name = "Druid Legendary Sleeves";
            Hue = 2010;

            // Legendary Druid stats
            SkillBonuses.SetValues(0, SkillName.AnimalLore, 5.0);

            Attributes.BonusInt = 4;
            Attributes.BonusDex = 2;
            Attributes.RegenHits = 3;

            BaseArmorRating = 49;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Grown from the Living Forest");
            list.Add("Set: Druid Legendary (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public DruidLegendarySleeves(Serial serial) : base(serial) { }

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

    public class DruidLegendaryGloves : LeatherGloves
    {
        [Constructable]
        public DruidLegendaryGloves()
        {
            Name = "Druid Legendary Gloves";
            Hue = 2010;

            // Legendary Druid stats
            SkillBonuses.SetValues(0, SkillName.AnimalLore, 5.0);

            Attributes.BonusInt = 4;
            Attributes.BonusDex = 2;
            Attributes.RegenHits = 3;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Grown from the Living Forest");
            list.Add("Set: Druid Legendary (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public DruidLegendaryGloves(Serial serial) : base(serial) { }

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

    public class DruidLegendaryLeggings : LeatherLegs
    {
        [Constructable]
        public DruidLegendaryLeggings()
        {
            Name = "Druid Legendary Leggings";
            Hue = 2010;

            // Legendary Druid stats
            SkillBonuses.SetValues(0, SkillName.AnimalLore, 5.0);

            Attributes.BonusInt = 4;
            Attributes.BonusDex = 2;
            Attributes.RegenHits = 3;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Grown from the Living Forest");
            list.Add("Set: Druid Legendary (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public DruidLegendaryLeggings(Serial serial) : base(serial) { }

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
    // WITCH LEGENDARY SET
    // Base: Leather
    // Hue: 1109
    // Bonuses: +5 BonusInt, +6 SpellDamage, +2 RegenMana
    // ============================================================================

    public class WitchLegendaryCap : LeatherCap
    {
        [Constructable]
        public WitchLegendaryCap()
        {
            Name = "Witch Legendary Cap";
            Hue = 1109;

            // Legendary Witch stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 6;
            Attributes.RegenMana = 2;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Woven with dark hexcraft");
            list.Add("Set: Witch Legendary (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public WitchLegendaryCap(Serial serial) : base(serial) { }

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

    public class WitchLegendaryGorget : LeatherGorget
    {
        [Constructable]
        public WitchLegendaryGorget()
        {
            Name = "Witch Legendary Gorget";
            Hue = 1109;

            // Legendary Witch stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 6;
            Attributes.RegenMana = 2;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Woven with dark hexcraft");
            list.Add("Set: Witch Legendary (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public WitchLegendaryGorget(Serial serial) : base(serial) { }

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

    public class WitchLegendaryTunic : LeatherChest
    {
        [Constructable]
        public WitchLegendaryTunic()
        {
            Name = "Witch Legendary Tunic";
            Hue = 1109;

            // Legendary Witch stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 6;
            Attributes.RegenMana = 2;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Woven with dark hexcraft");
            list.Add("Set: Witch Legendary (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public WitchLegendaryTunic(Serial serial) : base(serial) { }

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

    public class WitchLegendarySleeves : LeatherArms
    {
        [Constructable]
        public WitchLegendarySleeves()
        {
            Name = "Witch Legendary Sleeves";
            Hue = 1109;

            // Legendary Witch stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 6;
            Attributes.RegenMana = 2;

            BaseArmorRating = 49;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Woven with dark hexcraft");
            list.Add("Set: Witch Legendary (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public WitchLegendarySleeves(Serial serial) : base(serial) { }

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

    public class WitchLegendaryGloves : LeatherGloves
    {
        [Constructable]
        public WitchLegendaryGloves()
        {
            Name = "Witch Legendary Gloves";
            Hue = 1109;

            // Legendary Witch stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 6;
            Attributes.RegenMana = 2;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Woven with dark hexcraft");
            list.Add("Set: Witch Legendary (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public WitchLegendaryGloves(Serial serial) : base(serial) { }

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

    public class WitchLegendaryLeggings : LeatherLegs
    {
        [Constructable]
        public WitchLegendaryLeggings()
        {
            Name = "Witch Legendary Leggings";
            Hue = 1109;

            // Legendary Witch stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 6;
            Attributes.RegenMana = 2;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Woven with dark hexcraft");
            list.Add("Set: Witch Legendary (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public WitchLegendaryLeggings(Serial serial) : base(serial) { }

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
    // SORCERER LEGENDARY SET
    // Base: Leather
    // Hue: 1161
    // Bonuses: +5 BonusInt, +10 SpellDamage, +2 RegenMana
    // ============================================================================

    public class SorcererLegendaryCap : LeatherCap
    {
        [Constructable]
        public SorcererLegendaryCap()
        {
            Name = "Sorcerer Legendary Cap";
            Hue = 1161;

            // Legendary Sorcerer stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 10;
            Attributes.RegenMana = 2;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Forged in volcanic flames");
            list.Add("Set: Sorcerer Legendary (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public SorcererLegendaryCap(Serial serial) : base(serial) { }

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

    public class SorcererLegendaryGorget : LeatherGorget
    {
        [Constructable]
        public SorcererLegendaryGorget()
        {
            Name = "Sorcerer Legendary Gorget";
            Hue = 1161;

            // Legendary Sorcerer stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 10;
            Attributes.RegenMana = 2;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Forged in volcanic flames");
            list.Add("Set: Sorcerer Legendary (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public SorcererLegendaryGorget(Serial serial) : base(serial) { }

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

    public class SorcererLegendaryTunic : LeatherChest
    {
        [Constructable]
        public SorcererLegendaryTunic()
        {
            Name = "Sorcerer Legendary Tunic";
            Hue = 1161;

            // Legendary Sorcerer stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 10;
            Attributes.RegenMana = 2;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Forged in volcanic flames");
            list.Add("Set: Sorcerer Legendary (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public SorcererLegendaryTunic(Serial serial) : base(serial) { }

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

    public class SorcererLegendarySleeves : LeatherArms
    {
        [Constructable]
        public SorcererLegendarySleeves()
        {
            Name = "Sorcerer Legendary Sleeves";
            Hue = 1161;

            // Legendary Sorcerer stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 10;
            Attributes.RegenMana = 2;

            BaseArmorRating = 49;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Forged in volcanic flames");
            list.Add("Set: Sorcerer Legendary (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public SorcererLegendarySleeves(Serial serial) : base(serial) { }

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

    public class SorcererLegendaryGloves : LeatherGloves
    {
        [Constructable]
        public SorcererLegendaryGloves()
        {
            Name = "Sorcerer Legendary Gloves";
            Hue = 1161;

            // Legendary Sorcerer stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 10;
            Attributes.RegenMana = 2;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Forged in volcanic flames");
            list.Add("Set: Sorcerer Legendary (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public SorcererLegendaryGloves(Serial serial) : base(serial) { }

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

    public class SorcererLegendaryLeggings : LeatherLegs
    {
        [Constructable]
        public SorcererLegendaryLeggings()
        {
            Name = "Sorcerer Legendary Leggings";
            Hue = 1161;

            // Legendary Sorcerer stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 10;
            Attributes.RegenMana = 2;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Forged in volcanic flames");
            list.Add("Set: Sorcerer Legendary (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public SorcererLegendaryLeggings(Serial serial) : base(serial) { }

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
    // WARLOCK LEGENDARY SET
    // Base: Leather
    // Hue: 1175
    // Bonuses: +5 BonusInt, +8 SpellDamage, +3 RegenMana
    // ============================================================================

    public class WarlockLegendaryCap : LeatherCap
    {
        [Constructable]
        public WarlockLegendaryCap()
        {
            Name = "Warlock Legendary Cap";
            Hue = 1175;

            // Legendary Warlock stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 8;
            Attributes.RegenMana = 3;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Infused with void essence");
            list.Add("Set: Warlock Legendary (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public WarlockLegendaryCap(Serial serial) : base(serial) { }

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

    public class WarlockLegendaryGorget : LeatherGorget
    {
        [Constructable]
        public WarlockLegendaryGorget()
        {
            Name = "Warlock Legendary Gorget";
            Hue = 1175;

            // Legendary Warlock stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 8;
            Attributes.RegenMana = 3;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Infused with void essence");
            list.Add("Set: Warlock Legendary (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public WarlockLegendaryGorget(Serial serial) : base(serial) { }

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

    public class WarlockLegendaryTunic : LeatherChest
    {
        [Constructable]
        public WarlockLegendaryTunic()
        {
            Name = "Warlock Legendary Tunic";
            Hue = 1175;

            // Legendary Warlock stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 8;
            Attributes.RegenMana = 3;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Infused with void essence");
            list.Add("Set: Warlock Legendary (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public WarlockLegendaryTunic(Serial serial) : base(serial) { }

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

    public class WarlockLegendarySleeves : LeatherArms
    {
        [Constructable]
        public WarlockLegendarySleeves()
        {
            Name = "Warlock Legendary Sleeves";
            Hue = 1175;

            // Legendary Warlock stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 8;
            Attributes.RegenMana = 3;

            BaseArmorRating = 49;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Infused with void essence");
            list.Add("Set: Warlock Legendary (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public WarlockLegendarySleeves(Serial serial) : base(serial) { }

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

    public class WarlockLegendaryGloves : LeatherGloves
    {
        [Constructable]
        public WarlockLegendaryGloves()
        {
            Name = "Warlock Legendary Gloves";
            Hue = 1175;

            // Legendary Warlock stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 8;
            Attributes.RegenMana = 3;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Infused with void essence");
            list.Add("Set: Warlock Legendary (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public WarlockLegendaryGloves(Serial serial) : base(serial) { }

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

    public class WarlockLegendaryLeggings : LeatherLegs
    {
        [Constructable]
        public WarlockLegendaryLeggings()
        {
            Name = "Warlock Legendary Leggings";
            Hue = 1175;

            // Legendary Warlock stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 8;
            Attributes.RegenMana = 3;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Infused with void essence");
            list.Add("Set: Warlock Legendary (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public WarlockLegendaryLeggings(Serial serial) : base(serial) { }

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
    // ORACLE LEGENDARY SET
    // Base: Leather
    // Hue: 1154
    // Bonuses: +6 BonusInt, +8 LowerManaCost, +40 Luck
    // ============================================================================

    public class OracleLegendaryCap : LeatherCap
    {
        [Constructable]
        public OracleLegendaryCap()
        {
            Name = "Oracle Legendary Cap";
            Hue = 1154;

            // Legendary Oracle stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 6;
            Attributes.LowerManaCost = 8;
            Attributes.Luck = 40;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Shimmering with foresight");
            list.Add("Set: Oracle Legendary (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public OracleLegendaryCap(Serial serial) : base(serial) { }

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

    public class OracleLegendaryGorget : LeatherGorget
    {
        [Constructable]
        public OracleLegendaryGorget()
        {
            Name = "Oracle Legendary Gorget";
            Hue = 1154;

            // Legendary Oracle stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 6;
            Attributes.LowerManaCost = 8;
            Attributes.Luck = 40;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Shimmering with foresight");
            list.Add("Set: Oracle Legendary (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public OracleLegendaryGorget(Serial serial) : base(serial) { }

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

    public class OracleLegendaryTunic : LeatherChest
    {
        [Constructable]
        public OracleLegendaryTunic()
        {
            Name = "Oracle Legendary Tunic";
            Hue = 1154;

            // Legendary Oracle stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 6;
            Attributes.LowerManaCost = 8;
            Attributes.Luck = 40;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Shimmering with foresight");
            list.Add("Set: Oracle Legendary (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public OracleLegendaryTunic(Serial serial) : base(serial) { }

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

    public class OracleLegendarySleeves : LeatherArms
    {
        [Constructable]
        public OracleLegendarySleeves()
        {
            Name = "Oracle Legendary Sleeves";
            Hue = 1154;

            // Legendary Oracle stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 6;
            Attributes.LowerManaCost = 8;
            Attributes.Luck = 40;

            BaseArmorRating = 49;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Shimmering with foresight");
            list.Add("Set: Oracle Legendary (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public OracleLegendarySleeves(Serial serial) : base(serial) { }

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

    public class OracleLegendaryGloves : LeatherGloves
    {
        [Constructable]
        public OracleLegendaryGloves()
        {
            Name = "Oracle Legendary Gloves";
            Hue = 1154;

            // Legendary Oracle stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 6;
            Attributes.LowerManaCost = 8;
            Attributes.Luck = 40;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Shimmering with foresight");
            list.Add("Set: Oracle Legendary (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public OracleLegendaryGloves(Serial serial) : base(serial) { }

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

    public class OracleLegendaryLeggings : LeatherLegs
    {
        [Constructable]
        public OracleLegendaryLeggings()
        {
            Name = "Oracle Legendary Leggings";
            Hue = 1154;

            // Legendary Oracle stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 6;
            Attributes.LowerManaCost = 8;
            Attributes.Luck = 40;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Shimmering with foresight");
            list.Add("Set: Oracle Legendary (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public OracleLegendaryLeggings(Serial serial) : base(serial) { }

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
    // NECROMANCER LEGENDARY SET
    // Base: Leather
    // Hue: 1109
    // Bonuses: +5 BonusInt, +7 SpellDamage, +2 RegenMana
    // ============================================================================

    public class NecroLegendaryCap : LeatherCap
    {
        [Constructable]
        public NecroLegendaryCap()
        {
            Name = "Necromancer Legendary Cap";
            Hue = 1109;

            // Legendary Necromancer stats
            SkillBonuses.SetValues(0, SkillName.Necromancy, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 7;
            Attributes.RegenMana = 2;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Bound with death magic");
            list.Add("Set: Necromancer Legendary (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public NecroLegendaryCap(Serial serial) : base(serial) { }

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

    public class NecroLegendaryGorget : LeatherGorget
    {
        [Constructable]
        public NecroLegendaryGorget()
        {
            Name = "Necromancer Legendary Gorget";
            Hue = 1109;

            // Legendary Necromancer stats
            SkillBonuses.SetValues(0, SkillName.Necromancy, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 7;
            Attributes.RegenMana = 2;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Bound with death magic");
            list.Add("Set: Necromancer Legendary (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public NecroLegendaryGorget(Serial serial) : base(serial) { }

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

    public class NecroLegendaryTunic : LeatherChest
    {
        [Constructable]
        public NecroLegendaryTunic()
        {
            Name = "Necromancer Legendary Tunic";
            Hue = 1109;

            // Legendary Necromancer stats
            SkillBonuses.SetValues(0, SkillName.Necromancy, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 7;
            Attributes.RegenMana = 2;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Bound with death magic");
            list.Add("Set: Necromancer Legendary (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public NecroLegendaryTunic(Serial serial) : base(serial) { }

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

    public class NecroLegendarySleeves : LeatherArms
    {
        [Constructable]
        public NecroLegendarySleeves()
        {
            Name = "Necromancer Legendary Sleeves";
            Hue = 1109;

            // Legendary Necromancer stats
            SkillBonuses.SetValues(0, SkillName.Necromancy, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 7;
            Attributes.RegenMana = 2;

            BaseArmorRating = 49;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Bound with death magic");
            list.Add("Set: Necromancer Legendary (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public NecroLegendarySleeves(Serial serial) : base(serial) { }

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

    public class NecroLegendaryGloves : LeatherGloves
    {
        [Constructable]
        public NecroLegendaryGloves()
        {
            Name = "Necromancer Legendary Gloves";
            Hue = 1109;

            // Legendary Necromancer stats
            SkillBonuses.SetValues(0, SkillName.Necromancy, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 7;
            Attributes.RegenMana = 2;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Bound with death magic");
            list.Add("Set: Necromancer Legendary (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public NecroLegendaryGloves(Serial serial) : base(serial) { }

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

    public class NecroLegendaryLeggings : LeatherLegs
    {
        [Constructable]
        public NecroLegendaryLeggings()
        {
            Name = "Necromancer Legendary Leggings";
            Hue = 1109;

            // Legendary Necromancer stats
            SkillBonuses.SetValues(0, SkillName.Necromancy, 5.0);

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 7;
            Attributes.RegenMana = 2;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Bound with death magic");
            list.Add("Set: Necromancer Legendary (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public NecroLegendaryLeggings(Serial serial) : base(serial) { }

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
    // SUMMONER LEGENDARY SET
    // Base: Leather
    // Hue: 1266
    // Bonuses: +5 BonusInt, +15 BonusMana, +3 RegenMana
    // ============================================================================

    public class SummonerLegendaryCap : LeatherCap
    {
        [Constructable]
        public SummonerLegendaryCap()
        {
            Name = "Summoner Legendary Cap";
            Hue = 1266;

            // Legendary Summoner stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.BonusMana = 15;
            Attributes.RegenMana = 3;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by the Deep Ones");
            list.Add("Set: Summoner Legendary (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public SummonerLegendaryCap(Serial serial) : base(serial) { }

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

    public class SummonerLegendaryGorget : LeatherGorget
    {
        [Constructable]
        public SummonerLegendaryGorget()
        {
            Name = "Summoner Legendary Gorget";
            Hue = 1266;

            // Legendary Summoner stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.BonusMana = 15;
            Attributes.RegenMana = 3;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by the Deep Ones");
            list.Add("Set: Summoner Legendary (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public SummonerLegendaryGorget(Serial serial) : base(serial) { }

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

    public class SummonerLegendaryTunic : LeatherChest
    {
        [Constructable]
        public SummonerLegendaryTunic()
        {
            Name = "Summoner Legendary Tunic";
            Hue = 1266;

            // Legendary Summoner stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.BonusMana = 15;
            Attributes.RegenMana = 3;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by the Deep Ones");
            list.Add("Set: Summoner Legendary (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public SummonerLegendaryTunic(Serial serial) : base(serial) { }

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

    public class SummonerLegendarySleeves : LeatherArms
    {
        [Constructable]
        public SummonerLegendarySleeves()
        {
            Name = "Summoner Legendary Sleeves";
            Hue = 1266;

            // Legendary Summoner stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.BonusMana = 15;
            Attributes.RegenMana = 3;

            BaseArmorRating = 49;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by the Deep Ones");
            list.Add("Set: Summoner Legendary (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public SummonerLegendarySleeves(Serial serial) : base(serial) { }

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

    public class SummonerLegendaryGloves : LeatherGloves
    {
        [Constructable]
        public SummonerLegendaryGloves()
        {
            Name = "Summoner Legendary Gloves";
            Hue = 1266;

            // Legendary Summoner stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.BonusMana = 15;
            Attributes.RegenMana = 3;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by the Deep Ones");
            list.Add("Set: Summoner Legendary (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public SummonerLegendaryGloves(Serial serial) : base(serial) { }

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

    public class SummonerLegendaryLeggings : LeatherLegs
    {
        [Constructable]
        public SummonerLegendaryLeggings()
        {
            Name = "Summoner Legendary Leggings";
            Hue = 1266;

            // Legendary Summoner stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.BonusMana = 15;
            Attributes.RegenMana = 3;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by the Deep Ones");
            list.Add("Set: Summoner Legendary (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public SummonerLegendaryLeggings(Serial serial) : base(serial) { }

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
    // SHAMAN LEGENDARY SET
    // Base: Leather
    // Hue: 2212
    // Bonuses: +4 BonusInt, +2 BonusStr, +2 RegenHits
    // ============================================================================

    public class ShamanLegendaryCap : LeatherCap
    {
        [Constructable]
        public ShamanLegendaryCap()
        {
            Name = "Shaman Legendary Cap";
            Hue = 2212;

            // Legendary Shaman stats
            SkillBonuses.SetValues(0, SkillName.SpiritSpeak, 5.0);

            Attributes.BonusInt = 4;
            Attributes.BonusStr = 2;
            Attributes.RegenHits = 2;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by ancestor spirits");
            list.Add("Set: Shaman Legendary (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ShamanLegendaryCap(Serial serial) : base(serial) { }

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

    public class ShamanLegendaryGorget : LeatherGorget
    {
        [Constructable]
        public ShamanLegendaryGorget()
        {
            Name = "Shaman Legendary Gorget";
            Hue = 2212;

            // Legendary Shaman stats
            SkillBonuses.SetValues(0, SkillName.SpiritSpeak, 5.0);

            Attributes.BonusInt = 4;
            Attributes.BonusStr = 2;
            Attributes.RegenHits = 2;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by ancestor spirits");
            list.Add("Set: Shaman Legendary (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ShamanLegendaryGorget(Serial serial) : base(serial) { }

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

    public class ShamanLegendaryTunic : LeatherChest
    {
        [Constructable]
        public ShamanLegendaryTunic()
        {
            Name = "Shaman Legendary Tunic";
            Hue = 2212;

            // Legendary Shaman stats
            SkillBonuses.SetValues(0, SkillName.SpiritSpeak, 5.0);

            Attributes.BonusInt = 4;
            Attributes.BonusStr = 2;
            Attributes.RegenHits = 2;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by ancestor spirits");
            list.Add("Set: Shaman Legendary (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ShamanLegendaryTunic(Serial serial) : base(serial) { }

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

    public class ShamanLegendarySleeves : LeatherArms
    {
        [Constructable]
        public ShamanLegendarySleeves()
        {
            Name = "Shaman Legendary Sleeves";
            Hue = 2212;

            // Legendary Shaman stats
            SkillBonuses.SetValues(0, SkillName.SpiritSpeak, 5.0);

            Attributes.BonusInt = 4;
            Attributes.BonusStr = 2;
            Attributes.RegenHits = 2;

            BaseArmorRating = 49;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by ancestor spirits");
            list.Add("Set: Shaman Legendary (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ShamanLegendarySleeves(Serial serial) : base(serial) { }

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

    public class ShamanLegendaryGloves : LeatherGloves
    {
        [Constructable]
        public ShamanLegendaryGloves()
        {
            Name = "Shaman Legendary Gloves";
            Hue = 2212;

            // Legendary Shaman stats
            SkillBonuses.SetValues(0, SkillName.SpiritSpeak, 5.0);

            Attributes.BonusInt = 4;
            Attributes.BonusStr = 2;
            Attributes.RegenHits = 2;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by ancestor spirits");
            list.Add("Set: Shaman Legendary (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ShamanLegendaryGloves(Serial serial) : base(serial) { }

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

    public class ShamanLegendaryLeggings : LeatherLegs
    {
        [Constructable]
        public ShamanLegendaryLeggings()
        {
            Name = "Shaman Legendary Leggings";
            Hue = 2212;

            // Legendary Shaman stats
            SkillBonuses.SetValues(0, SkillName.SpiritSpeak, 5.0);

            Attributes.BonusInt = 4;
            Attributes.BonusStr = 2;
            Attributes.RegenHits = 2;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by ancestor spirits");
            list.Add("Set: Shaman Legendary (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ShamanLegendaryLeggings(Serial serial) : base(serial) { }

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
    // BARD LEGENDARY SET
    // Base: Leather
    // Hue: 1281
    // Bonuses: +3 BonusDex, +3 BonusInt, +50 Luck
    // ============================================================================

    public class BardLegendaryCap : LeatherCap
    {
        [Constructable]
        public BardLegendaryCap()
        {
            Name = "Bard Legendary Cap";
            Hue = 1281;

            // Legendary Bard stats
            SkillBonuses.SetValues(0, SkillName.Musicianship, 5.0);

            Attributes.BonusDex = 3;
            Attributes.BonusInt = 3;
            Attributes.Luck = 50;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Resonant with harmonic magic");
            list.Add("Set: Bard Legendary (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public BardLegendaryCap(Serial serial) : base(serial) { }

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

    public class BardLegendaryGorget : LeatherGorget
    {
        [Constructable]
        public BardLegendaryGorget()
        {
            Name = "Bard Legendary Gorget";
            Hue = 1281;

            // Legendary Bard stats
            SkillBonuses.SetValues(0, SkillName.Musicianship, 5.0);

            Attributes.BonusDex = 3;
            Attributes.BonusInt = 3;
            Attributes.Luck = 50;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Resonant with harmonic magic");
            list.Add("Set: Bard Legendary (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public BardLegendaryGorget(Serial serial) : base(serial) { }

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

    public class BardLegendaryTunic : LeatherChest
    {
        [Constructable]
        public BardLegendaryTunic()
        {
            Name = "Bard Legendary Tunic";
            Hue = 1281;

            // Legendary Bard stats
            SkillBonuses.SetValues(0, SkillName.Musicianship, 5.0);

            Attributes.BonusDex = 3;
            Attributes.BonusInt = 3;
            Attributes.Luck = 50;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Resonant with harmonic magic");
            list.Add("Set: Bard Legendary (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public BardLegendaryTunic(Serial serial) : base(serial) { }

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

    public class BardLegendarySleeves : LeatherArms
    {
        [Constructable]
        public BardLegendarySleeves()
        {
            Name = "Bard Legendary Sleeves";
            Hue = 1281;

            // Legendary Bard stats
            SkillBonuses.SetValues(0, SkillName.Musicianship, 5.0);

            Attributes.BonusDex = 3;
            Attributes.BonusInt = 3;
            Attributes.Luck = 50;

            BaseArmorRating = 49;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Resonant with harmonic magic");
            list.Add("Set: Bard Legendary (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public BardLegendarySleeves(Serial serial) : base(serial) { }

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

    public class BardLegendaryGloves : LeatherGloves
    {
        [Constructable]
        public BardLegendaryGloves()
        {
            Name = "Bard Legendary Gloves";
            Hue = 1281;

            // Legendary Bard stats
            SkillBonuses.SetValues(0, SkillName.Musicianship, 5.0);

            Attributes.BonusDex = 3;
            Attributes.BonusInt = 3;
            Attributes.Luck = 50;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Resonant with harmonic magic");
            list.Add("Set: Bard Legendary (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public BardLegendaryGloves(Serial serial) : base(serial) { }

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

    public class BardLegendaryLeggings : LeatherLegs
    {
        [Constructable]
        public BardLegendaryLeggings()
        {
            Name = "Bard Legendary Leggings";
            Hue = 1281;

            // Legendary Bard stats
            SkillBonuses.SetValues(0, SkillName.Musicianship, 5.0);

            Attributes.BonusDex = 3;
            Attributes.BonusInt = 3;
            Attributes.Luck = 50;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Resonant with harmonic magic");
            list.Add("Set: Bard Legendary (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public BardLegendaryLeggings(Serial serial) : base(serial) { }

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
    // ENCHANTER LEGENDARY SET
    // Base: Leather
    // Hue: 1155
    // Bonuses: +5 BonusInt, +10 LowerManaCost, +15 EnhancePotions
    // ============================================================================

    public class EnchanterLegendaryCap : LeatherCap
    {
        [Constructable]
        public EnchanterLegendaryCap()
        {
            Name = "Enchanter Legendary Cap";
            Hue = 1155;

            // Legendary Enchanter stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.LowerManaCost = 10;
            Attributes.EnhancePotions = 15;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Infused with runic power");
            list.Add("Set: Enchanter Legendary (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public EnchanterLegendaryCap(Serial serial) : base(serial) { }

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

    public class EnchanterLegendaryGorget : LeatherGorget
    {
        [Constructable]
        public EnchanterLegendaryGorget()
        {
            Name = "Enchanter Legendary Gorget";
            Hue = 1155;

            // Legendary Enchanter stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.LowerManaCost = 10;
            Attributes.EnhancePotions = 15;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Infused with runic power");
            list.Add("Set: Enchanter Legendary (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public EnchanterLegendaryGorget(Serial serial) : base(serial) { }

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

    public class EnchanterLegendaryTunic : LeatherChest
    {
        [Constructable]
        public EnchanterLegendaryTunic()
        {
            Name = "Enchanter Legendary Tunic";
            Hue = 1155;

            // Legendary Enchanter stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.LowerManaCost = 10;
            Attributes.EnhancePotions = 15;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Infused with runic power");
            list.Add("Set: Enchanter Legendary (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public EnchanterLegendaryTunic(Serial serial) : base(serial) { }

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

    public class EnchanterLegendarySleeves : LeatherArms
    {
        [Constructable]
        public EnchanterLegendarySleeves()
        {
            Name = "Enchanter Legendary Sleeves";
            Hue = 1155;

            // Legendary Enchanter stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.LowerManaCost = 10;
            Attributes.EnhancePotions = 15;

            BaseArmorRating = 49;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Infused with runic power");
            list.Add("Set: Enchanter Legendary (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public EnchanterLegendarySleeves(Serial serial) : base(serial) { }

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

    public class EnchanterLegendaryGloves : LeatherGloves
    {
        [Constructable]
        public EnchanterLegendaryGloves()
        {
            Name = "Enchanter Legendary Gloves";
            Hue = 1155;

            // Legendary Enchanter stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.LowerManaCost = 10;
            Attributes.EnhancePotions = 15;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Infused with runic power");
            list.Add("Set: Enchanter Legendary (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public EnchanterLegendaryGloves(Serial serial) : base(serial) { }

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

    public class EnchanterLegendaryLeggings : LeatherLegs
    {
        [Constructable]
        public EnchanterLegendaryLeggings()
        {
            Name = "Enchanter Legendary Leggings";
            Hue = 1155;

            // Legendary Enchanter stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 5;
            Attributes.LowerManaCost = 10;
            Attributes.EnhancePotions = 15;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Infused with runic power");
            list.Add("Set: Enchanter Legendary (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public EnchanterLegendaryLeggings(Serial serial) : base(serial) { }

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
    // ILLUSIONIST LEGENDARY SET
    // Base: Leather
    // Hue: 1153
    // Bonuses: +4 BonusInt, +3 BonusDex, +10 DefendChance
    // ============================================================================

    public class IllusionistLegendaryCap : LeatherCap
    {
        [Constructable]
        public IllusionistLegendaryCap()
        {
            Name = "Illusionist Legendary Cap";
            Hue = 1153;

            // Legendary Illusionist stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 4;
            Attributes.BonusDex = 3;
            Attributes.DefendChance = 10;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Shifting like desert mirages");
            list.Add("Set: Illusionist Legendary (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public IllusionistLegendaryCap(Serial serial) : base(serial) { }

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

    public class IllusionistLegendaryGorget : LeatherGorget
    {
        [Constructable]
        public IllusionistLegendaryGorget()
        {
            Name = "Illusionist Legendary Gorget";
            Hue = 1153;

            // Legendary Illusionist stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 4;
            Attributes.BonusDex = 3;
            Attributes.DefendChance = 10;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Shifting like desert mirages");
            list.Add("Set: Illusionist Legendary (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public IllusionistLegendaryGorget(Serial serial) : base(serial) { }

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

    public class IllusionistLegendaryTunic : LeatherChest
    {
        [Constructable]
        public IllusionistLegendaryTunic()
        {
            Name = "Illusionist Legendary Tunic";
            Hue = 1153;

            // Legendary Illusionist stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 4;
            Attributes.BonusDex = 3;
            Attributes.DefendChance = 10;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Shifting like desert mirages");
            list.Add("Set: Illusionist Legendary (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public IllusionistLegendaryTunic(Serial serial) : base(serial) { }

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

    public class IllusionistLegendarySleeves : LeatherArms
    {
        [Constructable]
        public IllusionistLegendarySleeves()
        {
            Name = "Illusionist Legendary Sleeves";
            Hue = 1153;

            // Legendary Illusionist stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 4;
            Attributes.BonusDex = 3;
            Attributes.DefendChance = 10;

            BaseArmorRating = 49;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Shifting like desert mirages");
            list.Add("Set: Illusionist Legendary (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public IllusionistLegendarySleeves(Serial serial) : base(serial) { }

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

    public class IllusionistLegendaryGloves : LeatherGloves
    {
        [Constructable]
        public IllusionistLegendaryGloves()
        {
            Name = "Illusionist Legendary Gloves";
            Hue = 1153;

            // Legendary Illusionist stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 4;
            Attributes.BonusDex = 3;
            Attributes.DefendChance = 10;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Shifting like desert mirages");
            list.Add("Set: Illusionist Legendary (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public IllusionistLegendaryGloves(Serial serial) : base(serial) { }

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

    public class IllusionistLegendaryLeggings : LeatherLegs
    {
        [Constructable]
        public IllusionistLegendaryLeggings()
        {
            Name = "Illusionist Legendary Leggings";
            Hue = 1153;

            // Legendary Illusionist stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 4;
            Attributes.BonusDex = 3;
            Attributes.DefendChance = 10;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Shifting like desert mirages");
            list.Add("Set: Illusionist Legendary (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public IllusionistLegendaryLeggings(Serial serial) : base(serial) { }

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
    // BARBARIAN LEGENDARY SET
    // Base: Plate
    // Hue: 1150
    // Bonuses: +6 BonusStr, +10 AttackChance, +3 RegenStam
    // ============================================================================

    public class BarbarianLegendaryHelm : PlateHelm
    {
        [Constructable]
        public BarbarianLegendaryHelm()
        {
            Name = "Barbarian Legendary Helm";
            Hue = 1150;

            // Legendary Barbarian stats
            SkillBonuses.SetValues(0, SkillName.Swords, 5.0);

            Attributes.BonusStr = 6;
            Attributes.AttackChance = 10;
            Attributes.RegenStam = 3;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Forged in Frosthold's fury");
            list.Add("Set: Barbarian Legendary (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public BarbarianLegendaryHelm(Serial serial) : base(serial) { }

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

    public class BarbarianLegendaryGorget : PlateGorget
    {
        [Constructable]
        public BarbarianLegendaryGorget()
        {
            Name = "Barbarian Legendary Gorget";
            Hue = 1150;

            // Legendary Barbarian stats
            SkillBonuses.SetValues(0, SkillName.Swords, 5.0);

            Attributes.BonusStr = 6;
            Attributes.AttackChance = 10;
            Attributes.RegenStam = 3;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Forged in Frosthold's fury");
            list.Add("Set: Barbarian Legendary (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public BarbarianLegendaryGorget(Serial serial) : base(serial) { }

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

    public class BarbarianLegendaryChest : PlateChest
    {
        [Constructable]
        public BarbarianLegendaryChest()
        {
            Name = "Barbarian Legendary Chest";
            Hue = 1150;

            // Legendary Barbarian stats
            SkillBonuses.SetValues(0, SkillName.Swords, 5.0);

            Attributes.BonusStr = 6;
            Attributes.AttackChance = 10;
            Attributes.RegenStam = 3;

            BaseArmorRating = 54;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Forged in Frosthold's fury");
            list.Add("Set: Barbarian Legendary (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public BarbarianLegendaryChest(Serial serial) : base(serial) { }

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

    public class BarbarianLegendaryArms : PlateArms
    {
        [Constructable]
        public BarbarianLegendaryArms()
        {
            Name = "Barbarian Legendary Arms";
            Hue = 1150;

            // Legendary Barbarian stats
            SkillBonuses.SetValues(0, SkillName.Swords, 5.0);

            Attributes.BonusStr = 6;
            Attributes.AttackChance = 10;
            Attributes.RegenStam = 3;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Forged in Frosthold's fury");
            list.Add("Set: Barbarian Legendary (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public BarbarianLegendaryArms(Serial serial) : base(serial) { }

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

    public class BarbarianLegendaryGloves : PlateGloves
    {
        [Constructable]
        public BarbarianLegendaryGloves()
        {
            Name = "Barbarian Legendary Gloves";
            Hue = 1150;

            // Legendary Barbarian stats
            SkillBonuses.SetValues(0, SkillName.Swords, 5.0);

            Attributes.BonusStr = 6;
            Attributes.AttackChance = 10;
            Attributes.RegenStam = 3;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Forged in Frosthold's fury");
            list.Add("Set: Barbarian Legendary (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public BarbarianLegendaryGloves(Serial serial) : base(serial) { }

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

    public class BarbarianLegendaryLegs : PlateLegs
    {
        [Constructable]
        public BarbarianLegendaryLegs()
        {
            Name = "Barbarian Legendary Legs";
            Hue = 1150;

            // Legendary Barbarian stats
            SkillBonuses.SetValues(0, SkillName.Swords, 5.0);

            Attributes.BonusStr = 6;
            Attributes.AttackChance = 10;
            Attributes.RegenStam = 3;

            BaseArmorRating = 53;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Forged in Frosthold's fury");
            list.Add("Set: Barbarian Legendary (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public BarbarianLegendaryLegs(Serial serial) : base(serial) { }

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
    // BEASTMASTER LEGENDARY SET
    // Base: Leather
    // Hue: 2301
    // Bonuses: +3 BonusStr, +3 BonusDex, +2 RegenHits
    // ============================================================================

    public class BeastmasterLegendaryCap : LeatherCap
    {
        [Constructable]
        public BeastmasterLegendaryCap()
        {
            Name = "Beastmaster Legendary Cap";
            Hue = 2301;

            // Legendary Beastmaster stats
            SkillBonuses.SetValues(0, SkillName.AnimalTaming, 5.0);

            Attributes.BonusStr = 3;
            Attributes.BonusDex = 3;
            Attributes.RegenHits = 2;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by wild spirits");
            list.Add("Set: Beastmaster Legendary (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public BeastmasterLegendaryCap(Serial serial) : base(serial) { }

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

    public class BeastmasterLegendaryGorget : LeatherGorget
    {
        [Constructable]
        public BeastmasterLegendaryGorget()
        {
            Name = "Beastmaster Legendary Gorget";
            Hue = 2301;

            // Legendary Beastmaster stats
            SkillBonuses.SetValues(0, SkillName.AnimalTaming, 5.0);

            Attributes.BonusStr = 3;
            Attributes.BonusDex = 3;
            Attributes.RegenHits = 2;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by wild spirits");
            list.Add("Set: Beastmaster Legendary (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public BeastmasterLegendaryGorget(Serial serial) : base(serial) { }

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

    public class BeastmasterLegendaryTunic : LeatherChest
    {
        [Constructable]
        public BeastmasterLegendaryTunic()
        {
            Name = "Beastmaster Legendary Tunic";
            Hue = 2301;

            // Legendary Beastmaster stats
            SkillBonuses.SetValues(0, SkillName.AnimalTaming, 5.0);

            Attributes.BonusStr = 3;
            Attributes.BonusDex = 3;
            Attributes.RegenHits = 2;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by wild spirits");
            list.Add("Set: Beastmaster Legendary (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public BeastmasterLegendaryTunic(Serial serial) : base(serial) { }

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

    public class BeastmasterLegendarySleeves : LeatherArms
    {
        [Constructable]
        public BeastmasterLegendarySleeves()
        {
            Name = "Beastmaster Legendary Sleeves";
            Hue = 2301;

            // Legendary Beastmaster stats
            SkillBonuses.SetValues(0, SkillName.AnimalTaming, 5.0);

            Attributes.BonusStr = 3;
            Attributes.BonusDex = 3;
            Attributes.RegenHits = 2;

            BaseArmorRating = 49;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by wild spirits");
            list.Add("Set: Beastmaster Legendary (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public BeastmasterLegendarySleeves(Serial serial) : base(serial) { }

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

    public class BeastmasterLegendaryGloves : LeatherGloves
    {
        [Constructable]
        public BeastmasterLegendaryGloves()
        {
            Name = "Beastmaster Legendary Gloves";
            Hue = 2301;

            // Legendary Beastmaster stats
            SkillBonuses.SetValues(0, SkillName.AnimalTaming, 5.0);

            Attributes.BonusStr = 3;
            Attributes.BonusDex = 3;
            Attributes.RegenHits = 2;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by wild spirits");
            list.Add("Set: Beastmaster Legendary (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public BeastmasterLegendaryGloves(Serial serial) : base(serial) { }

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

    public class BeastmasterLegendaryLeggings : LeatherLegs
    {
        [Constructable]
        public BeastmasterLegendaryLeggings()
        {
            Name = "Beastmaster Legendary Leggings";
            Hue = 2301;

            // Legendary Beastmaster stats
            SkillBonuses.SetValues(0, SkillName.AnimalTaming, 5.0);

            Attributes.BonusStr = 3;
            Attributes.BonusDex = 3;
            Attributes.RegenHits = 2;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by wild spirits");
            list.Add("Set: Beastmaster Legendary (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public BeastmasterLegendaryLeggings(Serial serial) : base(serial) { }

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
    // FIGHTER LEGENDARY SET
    // Base: Plate
    // Hue: 2401
    // Bonuses: +5 BonusStr, +2 BonusDex, +8 AttackChance
    // ============================================================================

    public class FighterLegendaryHelm : PlateHelm
    {
        [Constructable]
        public FighterLegendaryHelm()
        {
            Name = "Fighter Legendary Helm";
            Hue = 2401;

            // Legendary Fighter stats
            SkillBonuses.SetValues(0, SkillName.Tactics, 5.0);

            Attributes.BonusStr = 5;
            Attributes.BonusDex = 2;
            Attributes.AttackChance = 8;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Tempered in endless combat");
            list.Add("Set: Fighter Legendary (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public FighterLegendaryHelm(Serial serial) : base(serial) { }

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

    public class FighterLegendaryGorget : PlateGorget
    {
        [Constructable]
        public FighterLegendaryGorget()
        {
            Name = "Fighter Legendary Gorget";
            Hue = 2401;

            // Legendary Fighter stats
            SkillBonuses.SetValues(0, SkillName.Tactics, 5.0);

            Attributes.BonusStr = 5;
            Attributes.BonusDex = 2;
            Attributes.AttackChance = 8;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Tempered in endless combat");
            list.Add("Set: Fighter Legendary (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public FighterLegendaryGorget(Serial serial) : base(serial) { }

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

    public class FighterLegendaryChest : PlateChest
    {
        [Constructable]
        public FighterLegendaryChest()
        {
            Name = "Fighter Legendary Chest";
            Hue = 2401;

            // Legendary Fighter stats
            SkillBonuses.SetValues(0, SkillName.Tactics, 5.0);

            Attributes.BonusStr = 5;
            Attributes.BonusDex = 2;
            Attributes.AttackChance = 8;

            BaseArmorRating = 54;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Tempered in endless combat");
            list.Add("Set: Fighter Legendary (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public FighterLegendaryChest(Serial serial) : base(serial) { }

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

    public class FighterLegendaryArms : PlateArms
    {
        [Constructable]
        public FighterLegendaryArms()
        {
            Name = "Fighter Legendary Arms";
            Hue = 2401;

            // Legendary Fighter stats
            SkillBonuses.SetValues(0, SkillName.Tactics, 5.0);

            Attributes.BonusStr = 5;
            Attributes.BonusDex = 2;
            Attributes.AttackChance = 8;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Tempered in endless combat");
            list.Add("Set: Fighter Legendary (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public FighterLegendaryArms(Serial serial) : base(serial) { }

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

    public class FighterLegendaryGloves : PlateGloves
    {
        [Constructable]
        public FighterLegendaryGloves()
        {
            Name = "Fighter Legendary Gloves";
            Hue = 2401;

            // Legendary Fighter stats
            SkillBonuses.SetValues(0, SkillName.Tactics, 5.0);

            Attributes.BonusStr = 5;
            Attributes.BonusDex = 2;
            Attributes.AttackChance = 8;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Tempered in endless combat");
            list.Add("Set: Fighter Legendary (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public FighterLegendaryGloves(Serial serial) : base(serial) { }

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

    public class FighterLegendaryLegs : PlateLegs
    {
        [Constructable]
        public FighterLegendaryLegs()
        {
            Name = "Fighter Legendary Legs";
            Hue = 2401;

            // Legendary Fighter stats
            SkillBonuses.SetValues(0, SkillName.Tactics, 5.0);

            Attributes.BonusStr = 5;
            Attributes.BonusDex = 2;
            Attributes.AttackChance = 8;

            BaseArmorRating = 53;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Tempered in endless combat");
            list.Add("Set: Fighter Legendary (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public FighterLegendaryLegs(Serial serial) : base(serial) { }

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
    // MONK LEGENDARY SET
    // Base: Leather
    // Hue: 2213
    // Bonuses: +5 BonusDex, +2 BonusStr, +12 DefendChance
    // ============================================================================

    public class MonkLegendaryCap : LeatherCap
    {
        [Constructable]
        public MonkLegendaryCap()
        {
            Name = "Monk Legendary Cap";
            Hue = 2213;

            // Legendary Monk stats
            SkillBonuses.SetValues(0, SkillName.Wrestling, 5.0);

            Attributes.BonusDex = 5;
            Attributes.BonusStr = 2;
            Attributes.DefendChance = 12;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Woven with inner chi");
            list.Add("Set: Monk Legendary (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public MonkLegendaryCap(Serial serial) : base(serial) { }

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

    public class MonkLegendaryGorget : LeatherGorget
    {
        [Constructable]
        public MonkLegendaryGorget()
        {
            Name = "Monk Legendary Gorget";
            Hue = 2213;

            // Legendary Monk stats
            SkillBonuses.SetValues(0, SkillName.Wrestling, 5.0);

            Attributes.BonusDex = 5;
            Attributes.BonusStr = 2;
            Attributes.DefendChance = 12;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Woven with inner chi");
            list.Add("Set: Monk Legendary (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public MonkLegendaryGorget(Serial serial) : base(serial) { }

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

    public class MonkLegendaryTunic : LeatherChest
    {
        [Constructable]
        public MonkLegendaryTunic()
        {
            Name = "Monk Legendary Tunic";
            Hue = 2213;

            // Legendary Monk stats
            SkillBonuses.SetValues(0, SkillName.Wrestling, 5.0);

            Attributes.BonusDex = 5;
            Attributes.BonusStr = 2;
            Attributes.DefendChance = 12;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Woven with inner chi");
            list.Add("Set: Monk Legendary (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public MonkLegendaryTunic(Serial serial) : base(serial) { }

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

    public class MonkLegendarySleeves : LeatherArms
    {
        [Constructable]
        public MonkLegendarySleeves()
        {
            Name = "Monk Legendary Sleeves";
            Hue = 2213;

            // Legendary Monk stats
            SkillBonuses.SetValues(0, SkillName.Wrestling, 5.0);

            Attributes.BonusDex = 5;
            Attributes.BonusStr = 2;
            Attributes.DefendChance = 12;

            BaseArmorRating = 49;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Woven with inner chi");
            list.Add("Set: Monk Legendary (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public MonkLegendarySleeves(Serial serial) : base(serial) { }

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

    public class MonkLegendaryGloves : LeatherGloves
    {
        [Constructable]
        public MonkLegendaryGloves()
        {
            Name = "Monk Legendary Gloves";
            Hue = 2213;

            // Legendary Monk stats
            SkillBonuses.SetValues(0, SkillName.Wrestling, 5.0);

            Attributes.BonusDex = 5;
            Attributes.BonusStr = 2;
            Attributes.DefendChance = 12;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Woven with inner chi");
            list.Add("Set: Monk Legendary (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public MonkLegendaryGloves(Serial serial) : base(serial) { }

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

    public class MonkLegendaryLeggings : LeatherLegs
    {
        [Constructable]
        public MonkLegendaryLeggings()
        {
            Name = "Monk Legendary Leggings";
            Hue = 2213;

            // Legendary Monk stats
            SkillBonuses.SetValues(0, SkillName.Wrestling, 5.0);

            Attributes.BonusDex = 5;
            Attributes.BonusStr = 2;
            Attributes.DefendChance = 12;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Woven with inner chi");
            list.Add("Set: Monk Legendary (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public MonkLegendaryLeggings(Serial serial) : base(serial) { }

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
    // TEMPLAR LEGENDARY SET
    // Base: Plate
    // Hue: 1153
    // Bonuses: +4 BonusStr, +2 BonusInt, +3 RegenHits
    // ============================================================================

    public class TemplarLegendaryHelm : PlateHelm
    {
        [Constructable]
        public TemplarLegendaryHelm()
        {
            Name = "Templar Legendary Helm";
            Hue = 1153;

            // Legendary Templar stats
            SkillBonuses.SetValues(0, SkillName.Chivalry, 5.0);

            Attributes.BonusStr = 4;
            Attributes.BonusInt = 2;
            Attributes.RegenHits = 3;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by divine light");
            list.Add("Set: Templar Legendary (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public TemplarLegendaryHelm(Serial serial) : base(serial) { }

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

    public class TemplarLegendaryGorget : PlateGorget
    {
        [Constructable]
        public TemplarLegendaryGorget()
        {
            Name = "Templar Legendary Gorget";
            Hue = 1153;

            // Legendary Templar stats
            SkillBonuses.SetValues(0, SkillName.Chivalry, 5.0);

            Attributes.BonusStr = 4;
            Attributes.BonusInt = 2;
            Attributes.RegenHits = 3;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by divine light");
            list.Add("Set: Templar Legendary (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public TemplarLegendaryGorget(Serial serial) : base(serial) { }

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

    public class TemplarLegendaryChest : PlateChest
    {
        [Constructable]
        public TemplarLegendaryChest()
        {
            Name = "Templar Legendary Chest";
            Hue = 1153;

            // Legendary Templar stats
            SkillBonuses.SetValues(0, SkillName.Chivalry, 5.0);

            Attributes.BonusStr = 4;
            Attributes.BonusInt = 2;
            Attributes.RegenHits = 3;

            BaseArmorRating = 54;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by divine light");
            list.Add("Set: Templar Legendary (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public TemplarLegendaryChest(Serial serial) : base(serial) { }

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

    public class TemplarLegendaryArms : PlateArms
    {
        [Constructable]
        public TemplarLegendaryArms()
        {
            Name = "Templar Legendary Arms";
            Hue = 1153;

            // Legendary Templar stats
            SkillBonuses.SetValues(0, SkillName.Chivalry, 5.0);

            Attributes.BonusStr = 4;
            Attributes.BonusInt = 2;
            Attributes.RegenHits = 3;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by divine light");
            list.Add("Set: Templar Legendary (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public TemplarLegendaryArms(Serial serial) : base(serial) { }

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

    public class TemplarLegendaryGloves : PlateGloves
    {
        [Constructable]
        public TemplarLegendaryGloves()
        {
            Name = "Templar Legendary Gloves";
            Hue = 1153;

            // Legendary Templar stats
            SkillBonuses.SetValues(0, SkillName.Chivalry, 5.0);

            Attributes.BonusStr = 4;
            Attributes.BonusInt = 2;
            Attributes.RegenHits = 3;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by divine light");
            list.Add("Set: Templar Legendary (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public TemplarLegendaryGloves(Serial serial) : base(serial) { }

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

    public class TemplarLegendaryLegs : PlateLegs
    {
        [Constructable]
        public TemplarLegendaryLegs()
        {
            Name = "Templar Legendary Legs";
            Hue = 1153;

            // Legendary Templar stats
            SkillBonuses.SetValues(0, SkillName.Chivalry, 5.0);

            Attributes.BonusStr = 4;
            Attributes.BonusInt = 2;
            Attributes.RegenHits = 3;

            BaseArmorRating = 53;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed by divine light");
            list.Add("Set: Templar Legendary (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public TemplarLegendaryLegs(Serial serial) : base(serial) { }

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
    // RANGER LEGENDARY SET
    // Base: Leather
    // Hue: 2305
    // Bonuses: +6 BonusDex, +10 AttackChance, +5 WeaponSpeed
    // ============================================================================

    public class RangerLegendaryCap : LeatherCap
    {
        [Constructable]
        public RangerLegendaryCap()
        {
            Name = "Ranger Legendary Cap";
            Hue = 2305;

            // Legendary Ranger stats
            SkillBonuses.SetValues(0, SkillName.Archery, 5.0);

            Attributes.BonusDex = 6;
            Attributes.AttackChance = 10;
            Attributes.WeaponSpeed = 5;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Crafted from desert winds");
            list.Add("Set: Ranger Legendary (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public RangerLegendaryCap(Serial serial) : base(serial) { }

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

    public class RangerLegendaryGorget : LeatherGorget
    {
        [Constructable]
        public RangerLegendaryGorget()
        {
            Name = "Ranger Legendary Gorget";
            Hue = 2305;

            // Legendary Ranger stats
            SkillBonuses.SetValues(0, SkillName.Archery, 5.0);

            Attributes.BonusDex = 6;
            Attributes.AttackChance = 10;
            Attributes.WeaponSpeed = 5;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Crafted from desert winds");
            list.Add("Set: Ranger Legendary (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public RangerLegendaryGorget(Serial serial) : base(serial) { }

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

    public class RangerLegendaryTunic : LeatherChest
    {
        [Constructable]
        public RangerLegendaryTunic()
        {
            Name = "Ranger Legendary Tunic";
            Hue = 2305;

            // Legendary Ranger stats
            SkillBonuses.SetValues(0, SkillName.Archery, 5.0);

            Attributes.BonusDex = 6;
            Attributes.AttackChance = 10;
            Attributes.WeaponSpeed = 5;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Crafted from desert winds");
            list.Add("Set: Ranger Legendary (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public RangerLegendaryTunic(Serial serial) : base(serial) { }

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

    public class RangerLegendarySleeves : LeatherArms
    {
        [Constructable]
        public RangerLegendarySleeves()
        {
            Name = "Ranger Legendary Sleeves";
            Hue = 2305;

            // Legendary Ranger stats
            SkillBonuses.SetValues(0, SkillName.Archery, 5.0);

            Attributes.BonusDex = 6;
            Attributes.AttackChance = 10;
            Attributes.WeaponSpeed = 5;

            BaseArmorRating = 49;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Crafted from desert winds");
            list.Add("Set: Ranger Legendary (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public RangerLegendarySleeves(Serial serial) : base(serial) { }

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

    public class RangerLegendaryGloves : LeatherGloves
    {
        [Constructable]
        public RangerLegendaryGloves()
        {
            Name = "Ranger Legendary Gloves";
            Hue = 2305;

            // Legendary Ranger stats
            SkillBonuses.SetValues(0, SkillName.Archery, 5.0);

            Attributes.BonusDex = 6;
            Attributes.AttackChance = 10;
            Attributes.WeaponSpeed = 5;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Crafted from desert winds");
            list.Add("Set: Ranger Legendary (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public RangerLegendaryGloves(Serial serial) : base(serial) { }

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

    public class RangerLegendaryLeggings : LeatherLegs
    {
        [Constructable]
        public RangerLegendaryLeggings()
        {
            Name = "Ranger Legendary Leggings";
            Hue = 2305;

            // Legendary Ranger stats
            SkillBonuses.SetValues(0, SkillName.Archery, 5.0);

            Attributes.BonusDex = 6;
            Attributes.AttackChance = 10;
            Attributes.WeaponSpeed = 5;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Crafted from desert winds");
            list.Add("Set: Ranger Legendary (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public RangerLegendaryLeggings(Serial serial) : base(serial) { }

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
    // KNIGHT LEGENDARY SET
    // Base: Plate
    // Hue: 2406
    // Bonuses: +4 BonusStr, +15 DefendChance, +5 ReflectPhysical
    // ============================================================================

    public class KnightLegendaryHelm : PlateHelm
    {
        [Constructable]
        public KnightLegendaryHelm()
        {
            Name = "Knight Legendary Helm";
            Hue = 2406;

            // Legendary Knight stats
            SkillBonuses.SetValues(0, SkillName.Parry, 5.0);

            Attributes.BonusStr = 4;
            Attributes.DefendChance = 15;
            Attributes.ReflectPhysical = 5;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Forged with honor");
            list.Add("Set: Knight Legendary (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public KnightLegendaryHelm(Serial serial) : base(serial) { }

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

    public class KnightLegendaryGorget : PlateGorget
    {
        [Constructable]
        public KnightLegendaryGorget()
        {
            Name = "Knight Legendary Gorget";
            Hue = 2406;

            // Legendary Knight stats
            SkillBonuses.SetValues(0, SkillName.Parry, 5.0);

            Attributes.BonusStr = 4;
            Attributes.DefendChance = 15;
            Attributes.ReflectPhysical = 5;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Forged with honor");
            list.Add("Set: Knight Legendary (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public KnightLegendaryGorget(Serial serial) : base(serial) { }

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

    public class KnightLegendaryChest : PlateChest
    {
        [Constructable]
        public KnightLegendaryChest()
        {
            Name = "Knight Legendary Chest";
            Hue = 2406;

            // Legendary Knight stats
            SkillBonuses.SetValues(0, SkillName.Parry, 5.0);

            Attributes.BonusStr = 4;
            Attributes.DefendChance = 15;
            Attributes.ReflectPhysical = 5;

            BaseArmorRating = 54;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Forged with honor");
            list.Add("Set: Knight Legendary (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public KnightLegendaryChest(Serial serial) : base(serial) { }

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

    public class KnightLegendaryArms : PlateArms
    {
        [Constructable]
        public KnightLegendaryArms()
        {
            Name = "Knight Legendary Arms";
            Hue = 2406;

            // Legendary Knight stats
            SkillBonuses.SetValues(0, SkillName.Parry, 5.0);

            Attributes.BonusStr = 4;
            Attributes.DefendChance = 15;
            Attributes.ReflectPhysical = 5;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Forged with honor");
            list.Add("Set: Knight Legendary (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public KnightLegendaryArms(Serial serial) : base(serial) { }

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

    public class KnightLegendaryGloves : PlateGloves
    {
        [Constructable]
        public KnightLegendaryGloves()
        {
            Name = "Knight Legendary Gloves";
            Hue = 2406;

            // Legendary Knight stats
            SkillBonuses.SetValues(0, SkillName.Parry, 5.0);

            Attributes.BonusStr = 4;
            Attributes.DefendChance = 15;
            Attributes.ReflectPhysical = 5;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Forged with honor");
            list.Add("Set: Knight Legendary (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public KnightLegendaryGloves(Serial serial) : base(serial) { }

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

    public class KnightLegendaryLegs : PlateLegs
    {
        [Constructable]
        public KnightLegendaryLegs()
        {
            Name = "Knight Legendary Legs";
            Hue = 2406;

            // Legendary Knight stats
            SkillBonuses.SetValues(0, SkillName.Parry, 5.0);

            Attributes.BonusStr = 4;
            Attributes.DefendChance = 15;
            Attributes.ReflectPhysical = 5;

            BaseArmorRating = 53;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Forged with honor");
            list.Add("Set: Knight Legendary (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public KnightLegendaryLegs(Serial serial) : base(serial) { }

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
    // PALADIN LEGENDARY SET
    // Base: Plate
    // Hue: 1153
    // Bonuses: +3 BonusStr, +3 BonusInt, +4 RegenHits
    // ============================================================================

    public class PaladinLegendaryHelm : PlateHelm
    {
        [Constructable]
        public PaladinLegendaryHelm()
        {
            Name = "Paladin Legendary Helm";
            Hue = 1153;

            // Legendary Paladin stats
            SkillBonuses.SetValues(0, SkillName.Chivalry, 5.0);

            Attributes.BonusStr = 3;
            Attributes.BonusInt = 3;
            Attributes.RegenHits = 4;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Sanctified by virtue");
            list.Add("Set: Paladin Legendary (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public PaladinLegendaryHelm(Serial serial) : base(serial) { }

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

    public class PaladinLegendaryGorget : PlateGorget
    {
        [Constructable]
        public PaladinLegendaryGorget()
        {
            Name = "Paladin Legendary Gorget";
            Hue = 1153;

            // Legendary Paladin stats
            SkillBonuses.SetValues(0, SkillName.Chivalry, 5.0);

            Attributes.BonusStr = 3;
            Attributes.BonusInt = 3;
            Attributes.RegenHits = 4;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Sanctified by virtue");
            list.Add("Set: Paladin Legendary (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public PaladinLegendaryGorget(Serial serial) : base(serial) { }

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

    public class PaladinLegendaryChest : PlateChest
    {
        [Constructable]
        public PaladinLegendaryChest()
        {
            Name = "Paladin Legendary Chest";
            Hue = 1153;

            // Legendary Paladin stats
            SkillBonuses.SetValues(0, SkillName.Chivalry, 5.0);

            Attributes.BonusStr = 3;
            Attributes.BonusInt = 3;
            Attributes.RegenHits = 4;

            BaseArmorRating = 54;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Sanctified by virtue");
            list.Add("Set: Paladin Legendary (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public PaladinLegendaryChest(Serial serial) : base(serial) { }

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

    public class PaladinLegendaryArms : PlateArms
    {
        [Constructable]
        public PaladinLegendaryArms()
        {
            Name = "Paladin Legendary Arms";
            Hue = 1153;

            // Legendary Paladin stats
            SkillBonuses.SetValues(0, SkillName.Chivalry, 5.0);

            Attributes.BonusStr = 3;
            Attributes.BonusInt = 3;
            Attributes.RegenHits = 4;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Sanctified by virtue");
            list.Add("Set: Paladin Legendary (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public PaladinLegendaryArms(Serial serial) : base(serial) { }

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

    public class PaladinLegendaryGloves : PlateGloves
    {
        [Constructable]
        public PaladinLegendaryGloves()
        {
            Name = "Paladin Legendary Gloves";
            Hue = 1153;

            // Legendary Paladin stats
            SkillBonuses.SetValues(0, SkillName.Chivalry, 5.0);

            Attributes.BonusStr = 3;
            Attributes.BonusInt = 3;
            Attributes.RegenHits = 4;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Sanctified by virtue");
            list.Add("Set: Paladin Legendary (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public PaladinLegendaryGloves(Serial serial) : base(serial) { }

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

    public class PaladinLegendaryLegs : PlateLegs
    {
        [Constructable]
        public PaladinLegendaryLegs()
        {
            Name = "Paladin Legendary Legs";
            Hue = 1153;

            // Legendary Paladin stats
            SkillBonuses.SetValues(0, SkillName.Chivalry, 5.0);

            Attributes.BonusStr = 3;
            Attributes.BonusInt = 3;
            Attributes.RegenHits = 4;

            BaseArmorRating = 53;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Sanctified by virtue");
            list.Add("Set: Paladin Legendary (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public PaladinLegendaryLegs(Serial serial) : base(serial) { }

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
    // ROGUE LEGENDARY SET
    // Base: Leather
    // Hue: 1109
    // Bonuses: +6 BonusDex, +8 AttackChance, +8 WeaponSpeed
    // ============================================================================

    public class RogueLegendaryCap : LeatherCap
    {
        [Constructable]
        public RogueLegendaryCap()
        {
            Name = "Rogue Legendary Cap";
            Hue = 1109;

            // Legendary Rogue stats
            SkillBonuses.SetValues(0, SkillName.Stealth, 5.0);

            Attributes.BonusDex = 6;
            Attributes.AttackChance = 8;
            Attributes.WeaponSpeed = 8;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Sewn with shadow threads");
            list.Add("Set: Rogue Legendary (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public RogueLegendaryCap(Serial serial) : base(serial) { }

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

    public class RogueLegendaryGorget : LeatherGorget
    {
        [Constructable]
        public RogueLegendaryGorget()
        {
            Name = "Rogue Legendary Gorget";
            Hue = 1109;

            // Legendary Rogue stats
            SkillBonuses.SetValues(0, SkillName.Stealth, 5.0);

            Attributes.BonusDex = 6;
            Attributes.AttackChance = 8;
            Attributes.WeaponSpeed = 8;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Sewn with shadow threads");
            list.Add("Set: Rogue Legendary (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public RogueLegendaryGorget(Serial serial) : base(serial) { }

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

    public class RogueLegendaryTunic : LeatherChest
    {
        [Constructable]
        public RogueLegendaryTunic()
        {
            Name = "Rogue Legendary Tunic";
            Hue = 1109;

            // Legendary Rogue stats
            SkillBonuses.SetValues(0, SkillName.Stealth, 5.0);

            Attributes.BonusDex = 6;
            Attributes.AttackChance = 8;
            Attributes.WeaponSpeed = 8;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Sewn with shadow threads");
            list.Add("Set: Rogue Legendary (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public RogueLegendaryTunic(Serial serial) : base(serial) { }

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

    public class RogueLegendarySleeves : LeatherArms
    {
        [Constructable]
        public RogueLegendarySleeves()
        {
            Name = "Rogue Legendary Sleeves";
            Hue = 1109;

            // Legendary Rogue stats
            SkillBonuses.SetValues(0, SkillName.Stealth, 5.0);

            Attributes.BonusDex = 6;
            Attributes.AttackChance = 8;
            Attributes.WeaponSpeed = 8;

            BaseArmorRating = 49;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Sewn with shadow threads");
            list.Add("Set: Rogue Legendary (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public RogueLegendarySleeves(Serial serial) : base(serial) { }

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

    public class RogueLegendaryGloves : LeatherGloves
    {
        [Constructable]
        public RogueLegendaryGloves()
        {
            Name = "Rogue Legendary Gloves";
            Hue = 1109;

            // Legendary Rogue stats
            SkillBonuses.SetValues(0, SkillName.Stealth, 5.0);

            Attributes.BonusDex = 6;
            Attributes.AttackChance = 8;
            Attributes.WeaponSpeed = 8;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Sewn with shadow threads");
            list.Add("Set: Rogue Legendary (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public RogueLegendaryGloves(Serial serial) : base(serial) { }

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

    public class RogueLegendaryLeggings : LeatherLegs
    {
        [Constructable]
        public RogueLegendaryLeggings()
        {
            Name = "Rogue Legendary Leggings";
            Hue = 1109;

            // Legendary Rogue stats
            SkillBonuses.SetValues(0, SkillName.Stealth, 5.0);

            Attributes.BonusDex = 6;
            Attributes.AttackChance = 8;
            Attributes.WeaponSpeed = 8;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Sewn with shadow threads");
            list.Add("Set: Rogue Legendary (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public RogueLegendaryLeggings(Serial serial) : base(serial) { }

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
    // BOUNTY HUNTER LEGENDARY SET
    // Base: Leather
    // Hue: 2118
    // Bonuses: +4 BonusDex, +2 BonusStr, +10 AttackChance
    // ============================================================================

    public class BountyHunterLegendaryCap : LeatherCap
    {
        [Constructable]
        public BountyHunterLegendaryCap()
        {
            Name = "Bounty Hunter Legendary Cap";
            Hue = 2118;

            // Legendary Bounty Hunter stats
            SkillBonuses.SetValues(0, SkillName.Tracking, 5.0);

            Attributes.BonusDex = 4;
            Attributes.BonusStr = 2;
            Attributes.AttackChance = 10;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Marked with pursuit runes");
            list.Add("Set: Bounty Hunter Legendary (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public BountyHunterLegendaryCap(Serial serial) : base(serial) { }

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

    public class BountyHunterLegendaryGorget : LeatherGorget
    {
        [Constructable]
        public BountyHunterLegendaryGorget()
        {
            Name = "Bounty Hunter Legendary Gorget";
            Hue = 2118;

            // Legendary Bounty Hunter stats
            SkillBonuses.SetValues(0, SkillName.Tracking, 5.0);

            Attributes.BonusDex = 4;
            Attributes.BonusStr = 2;
            Attributes.AttackChance = 10;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Marked with pursuit runes");
            list.Add("Set: Bounty Hunter Legendary (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public BountyHunterLegendaryGorget(Serial serial) : base(serial) { }

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

    public class BountyHunterLegendaryTunic : LeatherChest
    {
        [Constructable]
        public BountyHunterLegendaryTunic()
        {
            Name = "Bounty Hunter Legendary Tunic";
            Hue = 2118;

            // Legendary Bounty Hunter stats
            SkillBonuses.SetValues(0, SkillName.Tracking, 5.0);

            Attributes.BonusDex = 4;
            Attributes.BonusStr = 2;
            Attributes.AttackChance = 10;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Marked with pursuit runes");
            list.Add("Set: Bounty Hunter Legendary (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public BountyHunterLegendaryTunic(Serial serial) : base(serial) { }

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

    public class BountyHunterLegendarySleeves : LeatherArms
    {
        [Constructable]
        public BountyHunterLegendarySleeves()
        {
            Name = "Bounty Hunter Legendary Sleeves";
            Hue = 2118;

            // Legendary Bounty Hunter stats
            SkillBonuses.SetValues(0, SkillName.Tracking, 5.0);

            Attributes.BonusDex = 4;
            Attributes.BonusStr = 2;
            Attributes.AttackChance = 10;

            BaseArmorRating = 49;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Marked with pursuit runes");
            list.Add("Set: Bounty Hunter Legendary (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public BountyHunterLegendarySleeves(Serial serial) : base(serial) { }

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

    public class BountyHunterLegendaryGloves : LeatherGloves
    {
        [Constructable]
        public BountyHunterLegendaryGloves()
        {
            Name = "Bounty Hunter Legendary Gloves";
            Hue = 2118;

            // Legendary Bounty Hunter stats
            SkillBonuses.SetValues(0, SkillName.Tracking, 5.0);

            Attributes.BonusDex = 4;
            Attributes.BonusStr = 2;
            Attributes.AttackChance = 10;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Marked with pursuit runes");
            list.Add("Set: Bounty Hunter Legendary (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public BountyHunterLegendaryGloves(Serial serial) : base(serial) { }

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

    public class BountyHunterLegendaryLeggings : LeatherLegs
    {
        [Constructable]
        public BountyHunterLegendaryLeggings()
        {
            Name = "Bounty Hunter Legendary Leggings";
            Hue = 2118;

            // Legendary Bounty Hunter stats
            SkillBonuses.SetValues(0, SkillName.Tracking, 5.0);

            Attributes.BonusDex = 4;
            Attributes.BonusStr = 2;
            Attributes.AttackChance = 10;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Marked with pursuit runes");
            list.Add("Set: Bounty Hunter Legendary (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public BountyHunterLegendaryLeggings(Serial serial) : base(serial) { }

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
    // ARTIFICER LEGENDARY SET
    // Base: Plate
    // Hue: 2401
    // Bonuses: +4 BonusStr, +2 BonusInt, +8 WeaponSpeed
    // ============================================================================

    public class ArtificerLegendaryHelm : PlateHelm
    {
        [Constructable]
        public ArtificerLegendaryHelm()
        {
            Name = "Artificer Legendary Helm";
            Hue = 2401;

            // Legendary Artificer stats
            SkillBonuses.SetValues(0, SkillName.Tinkering, 5.0);

            Attributes.BonusStr = 4;
            Attributes.BonusInt = 2;
            Attributes.WeaponSpeed = 8;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Powered by steam cores");
            list.Add("Set: Artificer Legendary (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ArtificerLegendaryHelm(Serial serial) : base(serial) { }

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

    public class ArtificerLegendaryGorget : PlateGorget
    {
        [Constructable]
        public ArtificerLegendaryGorget()
        {
            Name = "Artificer Legendary Gorget";
            Hue = 2401;

            // Legendary Artificer stats
            SkillBonuses.SetValues(0, SkillName.Tinkering, 5.0);

            Attributes.BonusStr = 4;
            Attributes.BonusInt = 2;
            Attributes.WeaponSpeed = 8;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Powered by steam cores");
            list.Add("Set: Artificer Legendary (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ArtificerLegendaryGorget(Serial serial) : base(serial) { }

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

    public class ArtificerLegendaryChest : PlateChest
    {
        [Constructable]
        public ArtificerLegendaryChest()
        {
            Name = "Artificer Legendary Chest";
            Hue = 2401;

            // Legendary Artificer stats
            SkillBonuses.SetValues(0, SkillName.Tinkering, 5.0);

            Attributes.BonusStr = 4;
            Attributes.BonusInt = 2;
            Attributes.WeaponSpeed = 8;

            BaseArmorRating = 54;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Powered by steam cores");
            list.Add("Set: Artificer Legendary (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ArtificerLegendaryChest(Serial serial) : base(serial) { }

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

    public class ArtificerLegendaryArms : PlateArms
    {
        [Constructable]
        public ArtificerLegendaryArms()
        {
            Name = "Artificer Legendary Arms";
            Hue = 2401;

            // Legendary Artificer stats
            SkillBonuses.SetValues(0, SkillName.Tinkering, 5.0);

            Attributes.BonusStr = 4;
            Attributes.BonusInt = 2;
            Attributes.WeaponSpeed = 8;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Powered by steam cores");
            list.Add("Set: Artificer Legendary (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ArtificerLegendaryArms(Serial serial) : base(serial) { }

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

    public class ArtificerLegendaryGloves : PlateGloves
    {
        [Constructable]
        public ArtificerLegendaryGloves()
        {
            Name = "Artificer Legendary Gloves";
            Hue = 2401;

            // Legendary Artificer stats
            SkillBonuses.SetValues(0, SkillName.Tinkering, 5.0);

            Attributes.BonusStr = 4;
            Attributes.BonusInt = 2;
            Attributes.WeaponSpeed = 8;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Powered by steam cores");
            list.Add("Set: Artificer Legendary (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ArtificerLegendaryGloves(Serial serial) : base(serial) { }

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

    public class ArtificerLegendaryLegs : PlateLegs
    {
        [Constructable]
        public ArtificerLegendaryLegs()
        {
            Name = "Artificer Legendary Legs";
            Hue = 2401;

            // Legendary Artificer stats
            SkillBonuses.SetValues(0, SkillName.Tinkering, 5.0);

            Attributes.BonusStr = 4;
            Attributes.BonusInt = 2;
            Attributes.WeaponSpeed = 8;

            BaseArmorRating = 53;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Powered by steam cores");
            list.Add("Set: Artificer Legendary (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ArtificerLegendaryLegs(Serial serial) : base(serial) { }

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
    // ALCHEMIST LEGENDARY SET
    // Base: Leather
    // Hue: 2010
    // Bonuses: +4 BonusInt, +2 BonusDex, +25 EnhancePotions
    // ============================================================================

    public class AlchemistLegendaryCap : LeatherCap
    {
        [Constructable]
        public AlchemistLegendaryCap()
        {
            Name = "Alchemist Legendary Cap";
            Hue = 2010;

            // Legendary Alchemist stats
            SkillBonuses.SetValues(0, SkillName.Alchemy, 5.0);

            Attributes.BonusInt = 4;
            Attributes.BonusDex = 2;
            Attributes.EnhancePotions = 25;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Infused with reagent essence");
            list.Add("Set: Alchemist Legendary (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public AlchemistLegendaryCap(Serial serial) : base(serial) { }

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

    public class AlchemistLegendaryGorget : LeatherGorget
    {
        [Constructable]
        public AlchemistLegendaryGorget()
        {
            Name = "Alchemist Legendary Gorget";
            Hue = 2010;

            // Legendary Alchemist stats
            SkillBonuses.SetValues(0, SkillName.Alchemy, 5.0);

            Attributes.BonusInt = 4;
            Attributes.BonusDex = 2;
            Attributes.EnhancePotions = 25;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Infused with reagent essence");
            list.Add("Set: Alchemist Legendary (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public AlchemistLegendaryGorget(Serial serial) : base(serial) { }

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

    public class AlchemistLegendaryTunic : LeatherChest
    {
        [Constructable]
        public AlchemistLegendaryTunic()
        {
            Name = "Alchemist Legendary Tunic";
            Hue = 2010;

            // Legendary Alchemist stats
            SkillBonuses.SetValues(0, SkillName.Alchemy, 5.0);

            Attributes.BonusInt = 4;
            Attributes.BonusDex = 2;
            Attributes.EnhancePotions = 25;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Infused with reagent essence");
            list.Add("Set: Alchemist Legendary (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public AlchemistLegendaryTunic(Serial serial) : base(serial) { }

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

    public class AlchemistLegendarySleeves : LeatherArms
    {
        [Constructable]
        public AlchemistLegendarySleeves()
        {
            Name = "Alchemist Legendary Sleeves";
            Hue = 2010;

            // Legendary Alchemist stats
            SkillBonuses.SetValues(0, SkillName.Alchemy, 5.0);

            Attributes.BonusInt = 4;
            Attributes.BonusDex = 2;
            Attributes.EnhancePotions = 25;

            BaseArmorRating = 49;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Infused with reagent essence");
            list.Add("Set: Alchemist Legendary (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public AlchemistLegendarySleeves(Serial serial) : base(serial) { }

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

    public class AlchemistLegendaryGloves : LeatherGloves
    {
        [Constructable]
        public AlchemistLegendaryGloves()
        {
            Name = "Alchemist Legendary Gloves";
            Hue = 2010;

            // Legendary Alchemist stats
            SkillBonuses.SetValues(0, SkillName.Alchemy, 5.0);

            Attributes.BonusInt = 4;
            Attributes.BonusDex = 2;
            Attributes.EnhancePotions = 25;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Infused with reagent essence");
            list.Add("Set: Alchemist Legendary (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public AlchemistLegendaryGloves(Serial serial) : base(serial) { }

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

    public class AlchemistLegendaryLeggings : LeatherLegs
    {
        [Constructable]
        public AlchemistLegendaryLeggings()
        {
            Name = "Alchemist Legendary Leggings";
            Hue = 2010;

            // Legendary Alchemist stats
            SkillBonuses.SetValues(0, SkillName.Alchemy, 5.0);

            Attributes.BonusInt = 4;
            Attributes.BonusDex = 2;
            Attributes.EnhancePotions = 25;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Infused with reagent essence");
            list.Add("Set: Alchemist Legendary (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public AlchemistLegendaryLeggings(Serial serial) : base(serial) { }

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
    // CLERIC LEGENDARY SET
    // Base: Chain
    // Hue: 1153
    // Bonuses: +4 BonusInt, +4 RegenHits, +2 RegenMana
    // ============================================================================

    public class ClericLegendaryCoif : ChainCoif
    {
        [Constructable]
        public ClericLegendaryCoif()
        {
            Name = "Cleric Legendary Coif";
            Hue = 1153;

            // Legendary Cleric stats
            SkillBonuses.SetValues(0, SkillName.Healing, 5.0);

            Attributes.BonusInt = 4;
            Attributes.RegenHits = 4;
            Attributes.RegenMana = 2;

            BaseArmorRating = 49;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed with divine grace");
            list.Add("Set: Cleric Legendary (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ClericLegendaryCoif(Serial serial) : base(serial) { }

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

    public class ClericLegendaryGorget : PlateGorget
    {
        [Constructable]
        public ClericLegendaryGorget()
        {
            Name = "Cleric Legendary Gorget";
            Hue = 1153;

            // Legendary Cleric stats
            SkillBonuses.SetValues(0, SkillName.Healing, 5.0);

            Attributes.BonusInt = 4;
            Attributes.RegenHits = 4;
            Attributes.RegenMana = 2;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed with divine grace");
            list.Add("Set: Cleric Legendary (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ClericLegendaryGorget(Serial serial) : base(serial) { }

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

    public class ClericLegendaryTunic : ChainChest
    {
        [Constructable]
        public ClericLegendaryTunic()
        {
            Name = "Cleric Legendary Tunic";
            Hue = 1153;

            // Legendary Cleric stats
            SkillBonuses.SetValues(0, SkillName.Healing, 5.0);

            Attributes.BonusInt = 4;
            Attributes.RegenHits = 4;
            Attributes.RegenMana = 2;

            BaseArmorRating = 53;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed with divine grace");
            list.Add("Set: Cleric Legendary (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ClericLegendaryTunic(Serial serial) : base(serial) { }

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

    public class ClericLegendarySleeves : PlateArms
    {
        [Constructable]
        public ClericLegendarySleeves()
        {
            Name = "Cleric Legendary Sleeves";
            Hue = 1153;

            // Legendary Cleric stats
            SkillBonuses.SetValues(0, SkillName.Healing, 5.0);

            Attributes.BonusInt = 4;
            Attributes.RegenHits = 4;
            Attributes.RegenMana = 2;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed with divine grace");
            list.Add("Set: Cleric Legendary (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ClericLegendarySleeves(Serial serial) : base(serial) { }

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

    public class ClericLegendaryGloves : PlateGloves
    {
        [Constructable]
        public ClericLegendaryGloves()
        {
            Name = "Cleric Legendary Gloves";
            Hue = 1153;

            // Legendary Cleric stats
            SkillBonuses.SetValues(0, SkillName.Healing, 5.0);

            Attributes.BonusInt = 4;
            Attributes.RegenHits = 4;
            Attributes.RegenMana = 2;

            BaseArmorRating = 49;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed with divine grace");
            list.Add("Set: Cleric Legendary (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ClericLegendaryGloves(Serial serial) : base(serial) { }

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

    public class ClericLegendaryLeggings : ChainLegs
    {
        [Constructable]
        public ClericLegendaryLeggings()
        {
            Name = "Cleric Legendary Leggings";
            Hue = 1153;

            // Legendary Cleric stats
            SkillBonuses.SetValues(0, SkillName.Healing, 5.0);

            Attributes.BonusInt = 4;
            Attributes.RegenHits = 4;
            Attributes.RegenMana = 2;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Blessed with divine grace");
            list.Add("Set: Cleric Legendary (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ClericLegendaryLeggings(Serial serial) : base(serial) { }

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
    // WIZARD LEGENDARY SET
    // Base: Leather
    // Hue: 1154
    // Bonuses: +6 BonusInt, +8 SpellDamage, +5 LowerManaCost
    // ============================================================================

    public class WizardLegendaryCap : LeatherCap
    {
        [Constructable]
        public WizardLegendaryCap()
        {
            Name = "Wizard Legendary Cap";
            Hue = 1154;

            // Legendary Wizard stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 6;
            Attributes.SpellDamage = 8;
            Attributes.LowerManaCost = 5;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Woven with arcane threads");
            list.Add("Set: Wizard Legendary (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public WizardLegendaryCap(Serial serial) : base(serial) { }

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

    public class WizardLegendaryGorget : LeatherGorget
    {
        [Constructable]
        public WizardLegendaryGorget()
        {
            Name = "Wizard Legendary Gorget";
            Hue = 1154;

            // Legendary Wizard stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 6;
            Attributes.SpellDamage = 8;
            Attributes.LowerManaCost = 5;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Woven with arcane threads");
            list.Add("Set: Wizard Legendary (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public WizardLegendaryGorget(Serial serial) : base(serial) { }

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

    public class WizardLegendaryTunic : LeatherChest
    {
        [Constructable]
        public WizardLegendaryTunic()
        {
            Name = "Wizard Legendary Tunic";
            Hue = 1154;

            // Legendary Wizard stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 6;
            Attributes.SpellDamage = 8;
            Attributes.LowerManaCost = 5;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Woven with arcane threads");
            list.Add("Set: Wizard Legendary (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public WizardLegendaryTunic(Serial serial) : base(serial) { }

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

    public class WizardLegendarySleeves : LeatherArms
    {
        [Constructable]
        public WizardLegendarySleeves()
        {
            Name = "Wizard Legendary Sleeves";
            Hue = 1154;

            // Legendary Wizard stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 6;
            Attributes.SpellDamage = 8;
            Attributes.LowerManaCost = 5;

            BaseArmorRating = 49;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Woven with arcane threads");
            list.Add("Set: Wizard Legendary (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public WizardLegendarySleeves(Serial serial) : base(serial) { }

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

    public class WizardLegendaryGloves : LeatherGloves
    {
        [Constructable]
        public WizardLegendaryGloves()
        {
            Name = "Wizard Legendary Gloves";
            Hue = 1154;

            // Legendary Wizard stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 6;
            Attributes.SpellDamage = 8;
            Attributes.LowerManaCost = 5;

            BaseArmorRating = 48;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Woven with arcane threads");
            list.Add("Set: Wizard Legendary (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public WizardLegendaryGloves(Serial serial) : base(serial) { }

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

    public class WizardLegendaryLeggings : LeatherLegs
    {
        [Constructable]
        public WizardLegendaryLeggings()
        {
            Name = "Wizard Legendary Leggings";
            Hue = 1154;

            // Legendary Wizard stats
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            Attributes.BonusInt = 6;
            Attributes.SpellDamage = 8;
            Attributes.LowerManaCost = 5;

            BaseArmorRating = 51;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Woven with arcane threads");
            list.Add("Set: Wizard Legendary (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public WizardLegendaryLeggings(Serial serial) : base(serial) { }

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
