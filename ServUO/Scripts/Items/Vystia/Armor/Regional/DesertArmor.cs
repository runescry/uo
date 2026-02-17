using System;
using Server;

namespace Server.Items
{
    #region Sunforged Helm

    public class SunforgedHelm : PlateHelm
    {
        [Constructable]
        public SunforgedHelm() : base()
        {
            Name = "Sunforged Helm";
            Hue = 2305; // Desert gold
            Weight = 4.0; // Lighter

            Attributes.BonusDex = 2;
            Attributes.BonusStam = 5;

            FireBonus = 8;
            PhysicalBonus = 3;
        }

        public override int AosStrReq { get { return 60; } } // Lower requirement

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Desert");
            list.Add("Crafted from Sandstone Ingots");
        }

        public SunforgedHelm(Serial serial) : base(serial) { }

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

    #region Sunforged Gorget

    public class SunforgedGorget : PlateGorget
    {
        [Constructable]
        public SunforgedGorget() : base()
        {
            Name = "Sunforged Gorget";
            Hue = 2305;
            Weight = 1.5;

            Attributes.BonusDex = 2;
            Attributes.BonusStam = 3;

            FireBonus = 6;
            PhysicalBonus = 2;
        }

        public override int AosStrReq { get { return 35; } }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Desert");
            list.Add("Crafted from Sandstone Ingots");
        }

        public SunforgedGorget(Serial serial) : base(serial) { }

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

    #region Sunforged Chest

    public class SunforgedChest : PlateChest
    {
        [Constructable]
        public SunforgedChest() : base()
        {
            Name = "Sunforged Breastplate";
            Hue = 2305;
            Weight = 7.0;

            Attributes.BonusDex = 3;
            Attributes.BonusStam = 10;
            Attributes.RegenStam = 3;

            FireBonus = 12;
            PhysicalBonus = 5;
        }

        public override int AosStrReq { get { return 70; } }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Desert");
            list.Add("Crafted from Sandstone Ingots");
        }

        public SunforgedChest(Serial serial) : base(serial) { }

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

    #region Sunforged Arms

    public class SunforgedArms : PlateArms
    {
        [Constructable]
        public SunforgedArms() : base()
        {
            Name = "Sunforged Arms";
            Hue = 2305;
            Weight = 4.0;

            Attributes.BonusDex = 2;
            Attributes.BonusStam = 5;

            FireBonus = 8;
            PhysicalBonus = 3;
        }

        public override int AosStrReq { get { return 60; } }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Desert");
            list.Add("Crafted from Sandstone Ingots");
        }

        public SunforgedArms(Serial serial) : base(serial) { }

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

    #region Sunforged Gloves

    public class SunforgedGloves : PlateGloves
    {
        [Constructable]
        public SunforgedGloves() : base()
        {
            Name = "Sunforged Gauntlets";
            Hue = 2305;
            Weight = 1.5;

            Attributes.BonusDex = 2;
            Attributes.WeaponSpeed = 5;

            FireBonus = 6;
            PhysicalBonus = 2;
        }

        public override int AosStrReq { get { return 50; } }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Desert");
            list.Add("Crafted from Sandstone Ingots");
        }

        public SunforgedGloves(Serial serial) : base(serial) { }

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

    #region Sunforged Legs

    public class SunforgedLegs : PlateLegs
    {
        [Constructable]
        public SunforgedLegs() : base()
        {
            Name = "Sunforged Leggings";
            Hue = 2305;
            Weight = 5.0;

            Attributes.BonusDex = 3;
            Attributes.BonusStam = 8;

            FireBonus = 10;
            PhysicalBonus = 4;
        }

        public override int AosStrReq { get { return 65; } }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Desert");
            list.Add("Crafted from Sandstone Ingots");
        }

        public SunforgedLegs(Serial serial) : base(serial) { }

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
