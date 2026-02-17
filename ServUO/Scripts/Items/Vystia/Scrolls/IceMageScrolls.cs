using System;
using Server;
using Server.Items;

namespace Server.Items.VystiaScrolls
{
    /// <summary>
    /// Spell scrolls for IceMage magic
    /// </summary>

    public class IceMageSpell1_1Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell1_1Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell1_1Scroll(int amount) : base(1000, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell1_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell1_2Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell1_2Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell1_2Scroll(int amount) : base(1001, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell1_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell1_3Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell1_3Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell1_3Scroll(int amount) : base(1002, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell1_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell1_4Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell1_4Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell1_4Scroll(int amount) : base(1003, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell1_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell2_1Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell2_1Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell2_1Scroll(int amount) : base(1004, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell2_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell2_2Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell2_2Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell2_2Scroll(int amount) : base(1005, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell2_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell2_3Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell2_3Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell2_3Scroll(int amount) : base(1006, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell2_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell2_4Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell2_4Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell2_4Scroll(int amount) : base(1007, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell2_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell3_1Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell3_1Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell3_1Scroll(int amount) : base(1008, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell3_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell3_2Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell3_2Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell3_2Scroll(int amount) : base(1009, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell3_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell3_3Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell3_3Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell3_3Scroll(int amount) : base(1010, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell3_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell3_4Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell3_4Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell3_4Scroll(int amount) : base(1011, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell3_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell4_1Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell4_1Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell4_1Scroll(int amount) : base(1012, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell4_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell4_2Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell4_2Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell4_2Scroll(int amount) : base(1013, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell4_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell4_3Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell4_3Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell4_3Scroll(int amount) : base(1014, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell4_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell4_4Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell4_4Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell4_4Scroll(int amount) : base(1015, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell4_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell5_1Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell5_1Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell5_1Scroll(int amount) : base(1016, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell5_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell5_2Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell5_2Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell5_2Scroll(int amount) : base(1017, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell5_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell5_3Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell5_3Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell5_3Scroll(int amount) : base(1018, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell5_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell5_4Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell5_4Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell5_4Scroll(int amount) : base(1019, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell5_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell6_1Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell6_1Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell6_1Scroll(int amount) : base(1020, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell6_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell6_2Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell6_2Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell6_2Scroll(int amount) : base(1021, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell6_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell6_3Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell6_3Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell6_3Scroll(int amount) : base(1022, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell6_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell6_4Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell6_4Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell6_4Scroll(int amount) : base(1023, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell6_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell7_1Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell7_1Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell7_1Scroll(int amount) : base(1024, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell7_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell7_2Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell7_2Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell7_2Scroll(int amount) : base(1025, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell7_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell7_3Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell7_3Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell7_3Scroll(int amount) : base(1026, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell7_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell7_4Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell7_4Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell7_4Scroll(int amount) : base(1027, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell7_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell8_1Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell8_1Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell8_1Scroll(int amount) : base(1028, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell8_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell8_2Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell8_2Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell8_2Scroll(int amount) : base(1029, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell8_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell8_3Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell8_3Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell8_3Scroll(int amount) : base(1030, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell8_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class IceMageSpell8_4Scroll : SpellScroll
    {
        [Constructable]
        public IceMageSpell8_4Scroll() : this(1)
        {
        }

        [Constructable]
        public IceMageSpell8_4Scroll(int amount) : base(1031, 0x1F2D, amount)
        {
            Hue = 0x481;
        }

        public IceMageSpell8_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

}
