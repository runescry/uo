using System;
using Server;

namespace Server.Items
{
    #region Frostforged Helm

    public class FrostforgedHelm : PlateHelm
    {
        [Constructable]
        public FrostforgedHelm() : base()
        {
            Name = "Frostforged Helm";
            Hue = 1152; // Ice blue

            Attributes.BonusInt = 3;
            Attributes.RegenMana = 1;

            ColdBonus = 8;
            PhysicalBonus = 3;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Frosthold");
            list.Add("Crafted from Frozen Ingots");
        }

        public FrostforgedHelm(Serial serial) : base(serial) { }

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

    #region Frostforged Gorget

    public class FrostforgedGorget : PlateGorget
    {
        [Constructable]
        public FrostforgedGorget() : base()
        {
            Name = "Frostforged Gorget";
            Hue = 1152;

            Attributes.BonusInt = 2;
            Attributes.RegenMana = 1;

            ColdBonus = 6;
            PhysicalBonus = 2;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Frosthold");
            list.Add("Crafted from Frozen Ingots");
        }

        public FrostforgedGorget(Serial serial) : base(serial) { }

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

    #region Frostforged Chest

    public class FrostforgedChest : PlateChest
    {
        [Constructable]
        public FrostforgedChest() : base()
        {
            Name = "Frostforged Breastplate";
            Hue = 1152;

            Attributes.BonusInt = 5;
            Attributes.RegenMana = 2;
            Attributes.LowerManaCost = 5;

            ColdBonus = 12;
            PhysicalBonus = 5;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Frosthold");
            list.Add("Crafted from Frozen Ingots");
        }

        public FrostforgedChest(Serial serial) : base(serial) { }

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

    #region Frostforged Arms

    public class FrostforgedArms : PlateArms
    {
        [Constructable]
        public FrostforgedArms() : base()
        {
            Name = "Frostforged Arms";
            Hue = 1152;

            Attributes.BonusInt = 3;
            Attributes.RegenMana = 1;

            ColdBonus = 8;
            PhysicalBonus = 3;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Frosthold");
            list.Add("Crafted from Frozen Ingots");
        }

        public FrostforgedArms(Serial serial) : base(serial) { }

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

    #region Frostforged Gloves

    public class FrostforgedGloves : PlateGloves
    {
        [Constructable]
        public FrostforgedGloves() : base()
        {
            Name = "Frostforged Gauntlets";
            Hue = 1152;

            Attributes.BonusInt = 2;
            Attributes.CastSpeed = 1;

            ColdBonus = 6;
            PhysicalBonus = 2;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Frosthold");
            list.Add("Crafted from Frozen Ingots");
        }

        public FrostforgedGloves(Serial serial) : base(serial) { }

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

    #region Frostforged Legs

    public class FrostforgedLegs : PlateLegs
    {
        [Constructable]
        public FrostforgedLegs() : base()
        {
            Name = "Frostforged Leggings";
            Hue = 1152;

            Attributes.BonusInt = 4;
            Attributes.RegenMana = 1;

            ColdBonus = 10;
            PhysicalBonus = 4;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Frosthold");
            list.Add("Crafted from Frozen Ingots");
        }

        public FrostforgedLegs(Serial serial) : base(serial) { }

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
