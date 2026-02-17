using System;
using Server;

namespace Server.Items.Vystia
{
    // ============================================================================
    // CRYSTAL CHAIN
    // Energy-infused chain mail
    // ============================================================================

    public class CrystalChainCoif : ChainCoif
    {
        [Constructable]
        public CrystalChainCoif()
        {
            Name = "Crystal Chain Coif";
            Hue = 1154;

            // Energy-infused chain mail
            Attributes.BonusInt = 3;
            Attributes.LowerManaCost = 5;
            EnergyBonus = 8;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("CRYSTAL Chain Armor");
        }

        public CrystalChainCoif(Serial serial) : base(serial) { }

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

    public class CrystalChainTunic : ChainChest
    {
        [Constructable]
        public CrystalChainTunic()
        {
            Name = "Crystal Chain Tunic";
            Hue = 1154;

            // Energy-infused chain mail
            Attributes.BonusInt = 3;
            Attributes.LowerManaCost = 5;
            EnergyBonus = 12;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("CRYSTAL Chain Armor");
        }

        public CrystalChainTunic(Serial serial) : base(serial) { }

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

    public class CrystalChainLegs : ChainLegs
    {
        [Constructable]
        public CrystalChainLegs()
        {
            Name = "Crystal Chain Legs";
            Hue = 1154;

            // Energy-infused chain mail
            Attributes.BonusInt = 3;
            Attributes.LowerManaCost = 5;
            EnergyBonus = 10;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("CRYSTAL Chain Armor");
        }

        public CrystalChainLegs(Serial serial) : base(serial) { }

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
    // SHADOW CHAIN
    // Stealth-enhancing dark chain
    // ============================================================================

    public class ShadowChainCoif : ChainCoif
    {
        [Constructable]
        public ShadowChainCoif()
        {
            Name = "Shadow Chain Coif";
            Hue = 1109;

            // Stealth-enhancing dark chain
            Attributes.NightSight = 1;
            Attributes.RegenStam = 2;
            SkillBonuses.SetValues(0, SkillName.Stealth, 5.0);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("SHADOW Chain Armor");
        }

        public ShadowChainCoif(Serial serial) : base(serial) { }

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

    public class ShadowChainTunic : ChainChest
    {
        [Constructable]
        public ShadowChainTunic()
        {
            Name = "Shadow Chain Tunic";
            Hue = 1109;

            // Stealth-enhancing dark chain
            Attributes.NightSight = 1;
            Attributes.RegenStam = 2;
            SkillBonuses.SetValues(0, SkillName.Stealth, 10.0);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("SHADOW Chain Armor");
        }

        public ShadowChainTunic(Serial serial) : base(serial) { }

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

    public class ShadowChainLegs : ChainLegs
    {
        [Constructable]
        public ShadowChainLegs()
        {
            Name = "Shadow Chain Legs";
            Hue = 1109;

            // Stealth-enhancing dark chain
            Attributes.NightSight = 1;
            Attributes.RegenStam = 2;
            SkillBonuses.SetValues(0, SkillName.Stealth, 5.0);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("SHADOW Chain Armor");
        }

        public ShadowChainLegs(Serial serial) : base(serial) { }

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
    // DESERT CHAIN
    // Lightweight sand-forged chain
    // ============================================================================

    public class DesertChainCoif : ChainCoif
    {
        [Constructable]
        public DesertChainCoif()
        {
            Name = "Desert Chain Coif";
            Hue = 2305;

            // Lightweight sand-forged chain
            Attributes.BonusDex = 3;
            Weight = Weight * 0.7; // 30% lighter
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("DESERT Chain Armor");
        }

        public DesertChainCoif(Serial serial) : base(serial) { }

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

    public class DesertChainTunic : ChainChest
    {
        [Constructable]
        public DesertChainTunic()
        {
            Name = "Desert Chain Tunic";
            Hue = 2305;

            // Lightweight sand-forged chain
            Attributes.BonusDex = 3;
            Weight = Weight * 0.7; // 30% lighter
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("DESERT Chain Armor");
        }

        public DesertChainTunic(Serial serial) : base(serial) { }

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

    public class DesertChainLegs : ChainLegs
    {
        [Constructable]
        public DesertChainLegs()
        {
            Name = "Desert Chain Legs";
            Hue = 2305;

            // Lightweight sand-forged chain
            Attributes.BonusDex = 3;
            Weight = Weight * 0.7; // 30% lighter
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("DESERT Chain Armor");
        }

        public DesertChainLegs(Serial serial) : base(serial) { }

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
