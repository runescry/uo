using System;
using Server;

namespace Server.Items
{
    #region Natureforged Helm

    public class NatureforgedHelm : PlateHelm
    {
        [Constructable]
        public NatureforgedHelm() : base()
        {
            Name = "Natureforged Helm";
            Hue = 2010; // Forest green

            Attributes.BonusHits = 5;
            Attributes.RegenHits = 2;

            PhysicalBonus = 8;
            PoisonBonus = 3;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Verdantpeak");
            list.Add("Crafted from Living Ingots");
        }

        public NatureforgedHelm(Serial serial) : base(serial) { }

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

    #region Natureforged Gorget

    public class NatureforgedGorget : PlateGorget
    {
        [Constructable]
        public NatureforgedGorget() : base()
        {
            Name = "Natureforged Gorget";
            Hue = 2010;

            Attributes.BonusHits = 3;
            Attributes.RegenHits = 1;

            PhysicalBonus = 6;
            PoisonBonus = 2;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Verdantpeak");
            list.Add("Crafted from Living Ingots");
        }

        public NatureforgedGorget(Serial serial) : base(serial) { }

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

    #region Natureforged Chest

    public class NatureforgedChest : PlateChest
    {
        [Constructable]
        public NatureforgedChest() : base()
        {
            Name = "Natureforged Breastplate";
            Hue = 2010;

            Attributes.BonusHits = 8;
            Attributes.RegenHits = 3;
            ArmorAttributes.SelfRepair = 3;

            PhysicalBonus = 12;
            PoisonBonus = 5;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Verdantpeak");
            list.Add("Crafted from Living Ingots");
        }

        public NatureforgedChest(Serial serial) : base(serial) { }

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

    #region Natureforged Arms

    public class NatureforgedArms : PlateArms
    {
        [Constructable]
        public NatureforgedArms() : base()
        {
            Name = "Natureforged Arms";
            Hue = 2010;

            Attributes.BonusHits = 5;
            Attributes.RegenHits = 2;

            PhysicalBonus = 8;
            PoisonBonus = 3;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Verdantpeak");
            list.Add("Crafted from Living Ingots");
        }

        public NatureforgedArms(Serial serial) : base(serial) { }

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

    #region Natureforged Gloves

    public class NatureforgedGloves : PlateGloves
    {
        [Constructable]
        public NatureforgedGloves() : base()
        {
            Name = "Natureforged Gauntlets";
            Hue = 2010;

            Attributes.BonusHits = 3;
            ArmorAttributes.SelfRepair = 2;

            PhysicalBonus = 6;
            PoisonBonus = 2;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Verdantpeak");
            list.Add("Crafted from Living Ingots");
        }

        public NatureforgedGloves(Serial serial) : base(serial) { }

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

    #region Natureforged Legs

    public class NatureforgedLegs : PlateLegs
    {
        [Constructable]
        public NatureforgedLegs() : base()
        {
            Name = "Natureforged Leggings";
            Hue = 2010;

            Attributes.BonusHits = 6;
            Attributes.RegenHits = 2;

            PhysicalBonus = 10;
            PoisonBonus = 4;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Verdantpeak");
            list.Add("Crafted from Living Ingots");
        }

        public NatureforgedLegs(Serial serial) : base(serial) { }

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
