using System;
using Server;

namespace Server.Items.Vystia
{
    // ============================================================================
    // REGIONAL SHIELDS
    // ============================================================================

    public class IceWall : HeaterShield
    {
        [Constructable]
        public IceWall()
        {
            Name = "Ice Wall";
            Hue = 1152;

            // Regional shield stats
            Attributes.DefendChance = 10;
            Attributes.ReflectPhysical = 5;
            ColdBonus = 10;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("FROSTHOLD Crafted");
        }

        public IceWall(Serial serial) : base(serial) { }

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

    public class FlameGuard : MetalKiteShield
    {
        [Constructable]
        public FlameGuard()
        {
            Name = "Flame Guard";
            Hue = 1358;

            // Regional shield stats
            Attributes.DefendChance = 10;
            FireBonus = 10;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("EMBERLANDS Crafted");
        }

        public FlameGuard(Serial serial) : base(serial) { }

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

    public class PrismShield : MetalShield
    {
        [Constructable]
        public PrismShield()
        {
            Name = "Prism Shield";
            Hue = 1154;

            // Regional shield stats
            Attributes.SpellChanneling = 1;
            Attributes.DefendChance = 10;
            EnergyBonus = 10;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("CRYSTAL Crafted");
        }

        public PrismShield(Serial serial) : base(serial) { }

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

    public class ClockworkShield : OrderShield
    {
        [Constructable]
        public ClockworkShield()
        {
            Name = "Clockwork Shield";
            Hue = 2401;

            // Regional shield stats
            Attributes.BonusDex = 5;
            Attributes.DefendChance = 15;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("IRONCLAD Crafted");
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

    public class BogShield : WoodenShield
    {
        [Constructable]
        public BogShield()
        {
            Name = "Bog Shield";
            Hue = 2212;

            // Regional shield stats
            Attributes.DefendChance = 10;
            PoisonBonus = 10;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("SHADOWFEN Crafted");
        }

        public BogShield(Serial serial) : base(serial) { }

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

    public class SandShield : Buckler
    {
        [Constructable]
        public SandShield()
        {
            Name = "Sand Shield";
            Hue = 2305;

            // Regional shield stats
            Attributes.BonusDex = 10;
            Weight = 2.0;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("DESERT Crafted");
        }

        public SandShield(Serial serial) : base(serial) { }

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

    public class VoidShield : ChaosShield
    {
        [Constructable]
        public VoidShield()
        {
            Name = "Void Shield";
            Hue = 1109;

            // Regional shield stats
            Attributes.SpellChanneling = 1;
            Attributes.CastSpeed = -1;
            Attributes.DefendChance = 10;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("OBSIDIAN Crafted");
        }

        public VoidShield(Serial serial) : base(serial) { }

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

    public class LivingShield : WoodenKiteShield
    {
        [Constructable]
        public LivingShield()
        {
            Name = "Living Shield";
            Hue = 2010;

            // Regional shield stats
            Attributes.RegenHits = 2;
            Attributes.DefendChance = 5;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("VERDANTPEAK Crafted");
        }

        public LivingShield(Serial serial) : base(serial) { }

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
