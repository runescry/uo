using System;
using Server;

namespace Server.Items
{
    #region Shadowforged Helm

    public class ShadowforgedHelm : PlateHelm
    {
        [Constructable]
        public ShadowforgedHelm() : base()
        {
            Name = "Shadowforged Helm";
            Hue = 1109; // Shadow black

            Attributes.BonusDex = 3;
            Attributes.NightSight = 1;

            PoisonBonus = 8;
            PhysicalBonus = 3;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Shadowfen");
            list.Add("Crafted from Bog Iron Ingots");
        }

        public ShadowforgedHelm(Serial serial) : base(serial) { }

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

    #region Shadowforged Gorget

    public class ShadowforgedGorget : PlateGorget
    {
        [Constructable]
        public ShadowforgedGorget() : base()
        {
            Name = "Shadowforged Gorget";
            Hue = 1109;

            Attributes.BonusDex = 2;

            PoisonBonus = 6;
            PhysicalBonus = 2;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Shadowfen");
            list.Add("Crafted from Bog Iron Ingots");
        }

        public ShadowforgedGorget(Serial serial) : base(serial) { }

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

    #region Shadowforged Chest

    public class ShadowforgedChest : PlateChest
    {
        [Constructable]
        public ShadowforgedChest() : base()
        {
            Name = "Shadowforged Breastplate";
            Hue = 1109;

            Attributes.BonusDex = 5;
            Attributes.NightSight = 1;
            Attributes.ReflectPhysical = 10;

            PoisonBonus = 12;
            PhysicalBonus = 5;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Shadowfen");
            list.Add("Crafted from Bog Iron Ingots");
        }

        public ShadowforgedChest(Serial serial) : base(serial) { }

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

    #region Shadowforged Arms

    public class ShadowforgedArms : PlateArms
    {
        [Constructable]
        public ShadowforgedArms() : base()
        {
            Name = "Shadowforged Arms";
            Hue = 1109;

            Attributes.BonusDex = 3;
            Attributes.DefendChance = 5;

            PoisonBonus = 8;
            PhysicalBonus = 3;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Shadowfen");
            list.Add("Crafted from Bog Iron Ingots");
        }

        public ShadowforgedArms(Serial serial) : base(serial) { }

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

    #region Shadowforged Gloves

    public class ShadowforgedGloves : PlateGloves
    {
        [Constructable]
        public ShadowforgedGloves() : base()
        {
            Name = "Shadowforged Gauntlets";
            Hue = 1109;

            Attributes.BonusDex = 2;
            Attributes.AttackChance = 5;

            PoisonBonus = 6;
            PhysicalBonus = 2;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Shadowfen");
            list.Add("Crafted from Bog Iron Ingots");
        }

        public ShadowforgedGloves(Serial serial) : base(serial) { }

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

    #region Shadowforged Legs

    public class ShadowforgedLegs : PlateLegs
    {
        [Constructable]
        public ShadowforgedLegs() : base()
        {
            Name = "Shadowforged Leggings";
            Hue = 1109;

            Attributes.BonusDex = 4;
            Attributes.DefendChance = 5;

            PoisonBonus = 10;
            PhysicalBonus = 4;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Shadowfen");
            list.Add("Crafted from Bog Iron Ingots");
        }

        public ShadowforgedLegs(Serial serial) : base(serial) { }

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
