using System;
using Server;

namespace Server.Items
{
    #region Clockwork Helm

    public class ClockworkHelm : PlateHelm
    {
        [Constructable]
        public ClockworkHelm() : base()
        {
            Name = "Clockwork Helm";
            Hue = 2401; // Clockwork bronze

            Attributes.BonusStr = 2;
            Attributes.BonusDex = 2;
            ArmorAttributes.SelfRepair = 3;
            ArmorAttributes.DurabilityBonus = 50;

            EnergyBonus = 6;
            PhysicalBonus = 5;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Ironclad Empire");
            list.Add("Crafted from Steamwork Ingots");
        }

        public ClockworkHelm(Serial serial) : base(serial) { }

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

    #region Clockwork Gorget

    public class ClockworkGorget : PlateGorget
    {
        [Constructable]
        public ClockworkGorget() : base()
        {
            Name = "Clockwork Gorget";
            Hue = 2401;

            Attributes.BonusStr = 1;
            Attributes.BonusDex = 1;
            ArmorAttributes.SelfRepair = 3;
            ArmorAttributes.DurabilityBonus = 50;

            EnergyBonus = 5;
            PhysicalBonus = 3;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Ironclad Empire");
            list.Add("Crafted from Steamwork Ingots");
        }

        public ClockworkGorget(Serial serial) : base(serial) { }

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

    #region Clockwork Chest

    public class ClockworkChest : PlateChest
    {
        [Constructable]
        public ClockworkChest() : base()
        {
            Name = "Clockwork Breastplate";
            Hue = 2401;

            Attributes.BonusStr = 3;
            Attributes.BonusDex = 3;
            Attributes.DefendChance = 10;
            ArmorAttributes.SelfRepair = 5;
            ArmorAttributes.DurabilityBonus = 100;

            EnergyBonus = 10;
            PhysicalBonus = 7;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Ironclad Empire");
            list.Add("Crafted from Steamwork Ingots");
        }

        public ClockworkChest(Serial serial) : base(serial) { }

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

    #region Clockwork Arms

    public class ClockworkArms : PlateArms
    {
        [Constructable]
        public ClockworkArms() : base()
        {
            Name = "Clockwork Arms";
            Hue = 2401;

            Attributes.BonusStr = 2;
            Attributes.BonusDex = 2;
            ArmorAttributes.SelfRepair = 3;
            ArmorAttributes.DurabilityBonus = 50;

            EnergyBonus = 6;
            PhysicalBonus = 5;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Ironclad Empire");
            list.Add("Crafted from Steamwork Ingots");
        }

        public ClockworkArms(Serial serial) : base(serial) { }

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

    #region Clockwork Gloves

    public class ClockworkGloves : PlateGloves
    {
        [Constructable]
        public ClockworkGloves() : base()
        {
            Name = "Clockwork Gauntlets";
            Hue = 2401;

            Attributes.BonusStr = 1;
            Attributes.BonusDex = 2;
            Attributes.WeaponSpeed = 5;
            ArmorAttributes.SelfRepair = 3;
            ArmorAttributes.DurabilityBonus = 50;

            EnergyBonus = 5;
            PhysicalBonus = 3;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Ironclad Empire");
            list.Add("Crafted from Steamwork Ingots");
        }

        public ClockworkGloves(Serial serial) : base(serial) { }

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

    #region Clockwork Legs

    public class ClockworkLegs : PlateLegs
    {
        [Constructable]
        public ClockworkLegs() : base()
        {
            Name = "Clockwork Leggings";
            Hue = 2401;

            Attributes.BonusStr = 2;
            Attributes.BonusDex = 3;
            ArmorAttributes.SelfRepair = 4;
            ArmorAttributes.DurabilityBonus = 75;

            EnergyBonus = 8;
            PhysicalBonus = 6;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Ironclad Empire");
            list.Add("Crafted from Steamwork Ingots");
        }

        public ClockworkLegs(Serial serial) : base(serial) { }

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
