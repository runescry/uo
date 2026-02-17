using System;
using Server;

namespace Server.Items
{
    #region Emberforged Helm

    public class EmberforgedHelm : PlateHelm
    {
        [Constructable]
        public EmberforgedHelm() : base()
        {
            Name = "Emberforged Helm";
            Hue = 1358; // Fire orange

            Attributes.BonusStr = 3;
            Attributes.RegenHits = 1;

            FireBonus = 8;
            PhysicalBonus = 3;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Emberlands");
            list.Add("Crafted from Molten Ingots");
        }

        public EmberforgedHelm(Serial serial) : base(serial) { }

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

    #region Emberforged Gorget

    public class EmberforgedGorget : PlateGorget
    {
        [Constructable]
        public EmberforgedGorget() : base()
        {
            Name = "Emberforged Gorget";
            Hue = 1358;

            Attributes.BonusStr = 2;
            Attributes.RegenHits = 1;

            FireBonus = 6;
            PhysicalBonus = 2;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Emberlands");
            list.Add("Crafted from Molten Ingots");
        }

        public EmberforgedGorget(Serial serial) : base(serial) { }

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

    #region Emberforged Chest

    public class EmberforgedChest : PlateChest
    {
        [Constructable]
        public EmberforgedChest() : base()
        {
            Name = "Emberforged Breastplate";
            Hue = 1358;

            Attributes.BonusStr = 5;
            Attributes.RegenHits = 2;
            Attributes.WeaponDamage = 10;

            FireBonus = 12;
            PhysicalBonus = 5;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Emberlands");
            list.Add("Crafted from Molten Ingots");
        }

        public EmberforgedChest(Serial serial) : base(serial) { }

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

    #region Emberforged Arms

    public class EmberforgedArms : PlateArms
    {
        [Constructable]
        public EmberforgedArms() : base()
        {
            Name = "Emberforged Arms";
            Hue = 1358;

            Attributes.BonusStr = 3;
            Attributes.RegenHits = 1;

            FireBonus = 8;
            PhysicalBonus = 3;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Emberlands");
            list.Add("Crafted from Molten Ingots");
        }

        public EmberforgedArms(Serial serial) : base(serial) { }

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

    #region Emberforged Gloves

    public class EmberforgedGloves : PlateGloves
    {
        [Constructable]
        public EmberforgedGloves() : base()
        {
            Name = "Emberforged Gauntlets";
            Hue = 1358;

            Attributes.BonusStr = 2;
            Attributes.AttackChance = 5;

            FireBonus = 6;
            PhysicalBonus = 2;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Emberlands");
            list.Add("Crafted from Molten Ingots");
        }

        public EmberforgedGloves(Serial serial) : base(serial) { }

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

    #region Emberforged Legs

    public class EmberforgedLegs : PlateLegs
    {
        [Constructable]
        public EmberforgedLegs() : base()
        {
            Name = "Emberforged Leggings";
            Hue = 1358;

            Attributes.BonusStr = 4;
            Attributes.RegenHits = 1;

            FireBonus = 10;
            PhysicalBonus = 4;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Emberlands");
            list.Add("Crafted from Molten Ingots");
        }

        public EmberforgedLegs(Serial serial) : base(serial) { }

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
