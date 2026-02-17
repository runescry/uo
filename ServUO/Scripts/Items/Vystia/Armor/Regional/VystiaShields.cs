using System;
using Server;

namespace Server.Items
{
    #region Frostforged Shield

    public class FrostforgedShield : HeaterShield
    {
        [Constructable]
        public FrostforgedShield() : base()
        {
            Name = "Frostforged Shield";
            Hue = 1152; // Ice blue

            Attributes.BonusInt = 3;
            Attributes.RegenMana = 2;
            Attributes.SpellChanneling = 1;

            ColdBonus = 10;
            PhysicalBonus = 5;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Frosthold");
            list.Add("Crafted from Frozen Ingots");
        }

        public FrostforgedShield(Serial serial) : base(serial) { }

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

    #endregion

    #region Emberforged Shield

    public class EmberforgedShield : HeaterShield
    {
        [Constructable]
        public EmberforgedShield() : base()
        {
            Name = "Emberforged Shield";
            Hue = 1358; // Fire orange

            Attributes.BonusStr = 3;
            Attributes.RegenHits = 2;
            Attributes.ReflectPhysical = 10;

            FireBonus = 10;
            PhysicalBonus = 5;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Emberlands");
            list.Add("Crafted from Molten Ingots");
        }

        public EmberforgedShield(Serial serial) : base(serial) { }

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

    #endregion

    #region Crystalline Shield

    public class CrystallineShield : HeaterShield
    {
        [Constructable]
        public CrystallineShield() : base()
        {
            Name = "Crystalline Shield";
            Hue = 1154; // Crystal blue

            Attributes.BonusInt = 3;
            Attributes.BonusMana = 8;
            Attributes.SpellChanneling = 1;
            Attributes.CastRecovery = 1;

            EnergyBonus = 10;
            PhysicalBonus = 5;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Crystal Barrens");
            list.Add("Crafted from Crystal Ingots");
        }

        public CrystallineShield(Serial serial) : base(serial) { }

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

    #endregion

    #region Shadowforged Shield

    public class ShadowforgedShield : HeaterShield
    {
        [Constructable]
        public ShadowforgedShield() : base()
        {
            Name = "Shadowforged Shield";
            Hue = 1109; // Shadow black

            Attributes.BonusDex = 3;
            Attributes.DefendChance = 10;
            Attributes.NightSight = 1;

            PoisonBonus = 10;
            PhysicalBonus = 5;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Shadowfen");
            list.Add("Crafted from Bog Iron Ingots");
        }

        public ShadowforgedShield(Serial serial) : base(serial) { }

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

    #endregion

    #region Natureforged Shield

    public class NatureforgedShield : HeaterShield
    {
        [Constructable]
        public NatureforgedShield() : base()
        {
            Name = "Natureforged Shield";
            Hue = 2010; // Forest green

            Attributes.BonusHits = 8;
            Attributes.RegenHits = 3;
            ArmorAttributes.SelfRepair = 5;

            PhysicalBonus = 12;
            PoisonBonus = 3;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Verdantpeak");
            list.Add("Crafted from Living Ingots");
        }

        public NatureforgedShield(Serial serial) : base(serial) { }

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

    #endregion

    #region Sunforged Shield

    public class SunforgedShield : HeaterShield
    {
        [Constructable]
        public SunforgedShield() : base()
        {
            Name = "Sunforged Shield";
            Hue = 2305; // Desert gold
            Weight = 6.0; // Lighter

            Attributes.BonusDex = 3;
            Attributes.BonusStam = 8;
            Attributes.RegenStam = 2;

            FireBonus = 10;
            PhysicalBonus = 5;
        }

        public override int AosStrReq { get { return 70; } } // Lower requirement

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Desert");
            list.Add("Crafted from Sandstone Ingots");
        }

        public SunforgedShield(Serial serial) : base(serial) { }

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

    #endregion

    #region Clockwork Shield

    public class ClockworkShield : HeaterShield
    {
        [Constructable]
        public ClockworkShield() : base()
        {
            Name = "Clockwork Shield";
            Hue = 2401; // Clockwork bronze

            Attributes.BonusStr = 2;
            Attributes.BonusDex = 2;
            Attributes.DefendChance = 10;
            ArmorAttributes.SelfRepair = 5;
            ArmorAttributes.DurabilityBonus = 100;

            EnergyBonus = 8;
            PhysicalBonus = 7;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Ironclad Empire");
            list.Add("Crafted from Steamwork Ingots");
        }

        public ClockworkShield(Serial serial) : base(serial) { }

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

    #endregion

    #region Voidforged Shield

    public class VoidforgedShield : HeaterShield
    {
        [Constructable]
        public VoidforgedShield() : base()
        {
            Name = "Voidforged Shield";
            Hue = 1109; // Shadow black

            Attributes.BonusInt = 3;
            Attributes.BonusMana = 8;
            Attributes.LowerManaCost = 5;
            Attributes.SpellChanneling = 1;

            EnergyBonus = 10;
            PhysicalBonus = 5;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Obsidian Wastes");
            list.Add("Crafted from Obsidian Ingots");
        }

        public VoidforgedShield(Serial serial) : base(serial) { }

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

    #endregion
}
