using System;
using Server;

namespace Server.Items
{
    #region Crystalline Helm

    public class CrystallineHelm : PlateHelm
    {
        [Constructable]
        public CrystallineHelm() : base()
        {
            Name = "Crystalline Helm";
            Hue = 1154; // Crystal blue

            Attributes.BonusInt = 3;
            Attributes.BonusMana = 5;

            EnergyBonus = 8;
            PhysicalBonus = 3;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Crystal Barrens");
            list.Add("Crafted from Crystal Ingots");
        }

        public CrystallineHelm(Serial serial) : base(serial) { }

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

    #region Crystalline Gorget

    public class CrystallineGorget : PlateGorget
    {
        [Constructable]
        public CrystallineGorget() : base()
        {
            Name = "Crystalline Gorget";
            Hue = 1154;

            Attributes.BonusInt = 2;
            Attributes.BonusMana = 3;

            EnergyBonus = 6;
            PhysicalBonus = 2;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Crystal Barrens");
            list.Add("Crafted from Crystal Ingots");
        }

        public CrystallineGorget(Serial serial) : base(serial) { }

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

    #region Crystalline Chest

    public class CrystallineChest : PlateChest
    {
        [Constructable]
        public CrystallineChest() : base()
        {
            Name = "Crystalline Breastplate";
            Hue = 1154;

            Attributes.BonusInt = 5;
            Attributes.BonusMana = 8;
            Attributes.SpellDamage = 10;

            EnergyBonus = 12;
            PhysicalBonus = 5;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Crystal Barrens");
            list.Add("Crafted from Crystal Ingots");
        }

        public CrystallineChest(Serial serial) : base(serial) { }

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

    #region Crystalline Arms

    public class CrystallineArms : PlateArms
    {
        [Constructable]
        public CrystallineArms() : base()
        {
            Name = "Crystalline Arms";
            Hue = 1154;

            Attributes.BonusInt = 3;
            Attributes.BonusMana = 5;

            EnergyBonus = 8;
            PhysicalBonus = 3;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Crystal Barrens");
            list.Add("Crafted from Crystal Ingots");
        }

        public CrystallineArms(Serial serial) : base(serial) { }

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

    #region Crystalline Gloves

    public class CrystallineGloves : PlateGloves
    {
        [Constructable]
        public CrystallineGloves() : base()
        {
            Name = "Crystalline Gauntlets";
            Hue = 1154;

            Attributes.BonusInt = 2;
            Attributes.CastRecovery = 1;

            EnergyBonus = 6;
            PhysicalBonus = 2;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Crystal Barrens");
            list.Add("Crafted from Crystal Ingots");
        }

        public CrystallineGloves(Serial serial) : base(serial) { }

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

    #region Crystalline Legs

    public class CrystallineLegs : PlateLegs
    {
        [Constructable]
        public CrystallineLegs() : base()
        {
            Name = "Crystalline Leggings";
            Hue = 1154;

            Attributes.BonusInt = 4;
            Attributes.BonusMana = 6;

            EnergyBonus = 10;
            PhysicalBonus = 4;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Crystal Barrens");
            list.Add("Crafted from Crystal Ingots");
        }

        public CrystallineLegs(Serial serial) : base(serial) { }

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
