using System;
using Server;

namespace Server.Items
{
    #region Voidforged Helm

    public class VoidforgedHelm : PlateHelm
    {
        [Constructable]
        public VoidforgedHelm() : base()
        {
            Name = "Voidforged Helm";
            Hue = 1109; // Shadow black

            Attributes.BonusInt = 3;
            Attributes.BonusMana = 5;
            Attributes.LowerManaCost = 3;

            EnergyBonus = 8;
            PhysicalBonus = 3;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Obsidian Wastes");
            list.Add("Crafted from Obsidian Ingots");
        }

        public VoidforgedHelm(Serial serial) : base(serial) { }

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

    #region Voidforged Gorget

    public class VoidforgedGorget : PlateGorget
    {
        [Constructable]
        public VoidforgedGorget() : base()
        {
            Name = "Voidforged Gorget";
            Hue = 1109;

            Attributes.BonusInt = 2;
            Attributes.BonusMana = 3;
            Attributes.LowerManaCost = 2;

            EnergyBonus = 6;
            PhysicalBonus = 2;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Obsidian Wastes");
            list.Add("Crafted from Obsidian Ingots");
        }

        public VoidforgedGorget(Serial serial) : base(serial) { }

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

    #region Voidforged Chest

    public class VoidforgedChest : PlateChest
    {
        [Constructable]
        public VoidforgedChest() : base()
        {
            Name = "Voidforged Breastplate";
            Hue = 1109;

            Attributes.BonusInt = 5;
            Attributes.BonusMana = 10;
            Attributes.LowerManaCost = 8;
            Attributes.SpellDamage = 8;

            EnergyBonus = 12;
            PhysicalBonus = 5;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Obsidian Wastes");
            list.Add("Crafted from Obsidian Ingots");
        }

        public VoidforgedChest(Serial serial) : base(serial) { }

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

    #region Voidforged Arms

    public class VoidforgedArms : PlateArms
    {
        [Constructable]
        public VoidforgedArms() : base()
        {
            Name = "Voidforged Arms";
            Hue = 1109;

            Attributes.BonusInt = 3;
            Attributes.BonusMana = 5;
            Attributes.LowerManaCost = 3;

            EnergyBonus = 8;
            PhysicalBonus = 3;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Obsidian Wastes");
            list.Add("Crafted from Obsidian Ingots");
        }

        public VoidforgedArms(Serial serial) : base(serial) { }

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

    #region Voidforged Gloves

    public class VoidforgedGloves : PlateGloves
    {
        [Constructable]
        public VoidforgedGloves() : base()
        {
            Name = "Voidforged Gauntlets";
            Hue = 1109;

            Attributes.BonusInt = 2;
            Attributes.BonusMana = 3;
            Attributes.CastSpeed = 1;

            EnergyBonus = 6;
            PhysicalBonus = 2;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Obsidian Wastes");
            list.Add("Crafted from Obsidian Ingots");
        }

        public VoidforgedGloves(Serial serial) : base(serial) { }

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

    #region Voidforged Legs

    public class VoidforgedLegs : PlateLegs
    {
        [Constructable]
        public VoidforgedLegs() : base()
        {
            Name = "Voidforged Leggings";
            Hue = 1109;

            Attributes.BonusInt = 4;
            Attributes.BonusMana = 8;
            Attributes.LowerManaCost = 5;

            EnergyBonus = 10;
            PhysicalBonus = 4;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Obsidian Wastes");
            list.Add("Crafted from Obsidian Ingots");
        }

        public VoidforgedLegs(Serial serial) : base(serial) { }

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
