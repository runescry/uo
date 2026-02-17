using System;
using Server;
using Server.Items;

namespace Server.Items.VystiaScrolls
{
    /// <summary>
    /// Spell scrolls for Druid magic
    /// </summary>

    public class DruidSpell1_1Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell1_1Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell1_1Scroll(int amount) : base(1032, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell1_1Scroll(Serial serial) : base(serial)
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

    public class DruidSpell1_2Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell1_2Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell1_2Scroll(int amount) : base(1033, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell1_2Scroll(Serial serial) : base(serial)
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

    public class DruidSpell1_3Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell1_3Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell1_3Scroll(int amount) : base(1034, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell1_3Scroll(Serial serial) : base(serial)
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

    public class DruidSpell1_4Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell1_4Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell1_4Scroll(int amount) : base(1035, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell1_4Scroll(Serial serial) : base(serial)
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

    public class DruidSpell2_1Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell2_1Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell2_1Scroll(int amount) : base(1036, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell2_1Scroll(Serial serial) : base(serial)
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

    public class DruidSpell2_2Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell2_2Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell2_2Scroll(int amount) : base(1037, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell2_2Scroll(Serial serial) : base(serial)
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

    public class DruidSpell2_3Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell2_3Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell2_3Scroll(int amount) : base(1038, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell2_3Scroll(Serial serial) : base(serial)
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

    public class DruidSpell2_4Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell2_4Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell2_4Scroll(int amount) : base(1039, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell2_4Scroll(Serial serial) : base(serial)
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

    public class DruidSpell3_1Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell3_1Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell3_1Scroll(int amount) : base(1040, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell3_1Scroll(Serial serial) : base(serial)
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

    public class DruidSpell3_2Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell3_2Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell3_2Scroll(int amount) : base(1041, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell3_2Scroll(Serial serial) : base(serial)
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

    public class DruidSpell3_3Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell3_3Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell3_3Scroll(int amount) : base(1042, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell3_3Scroll(Serial serial) : base(serial)
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

    public class DruidSpell3_4Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell3_4Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell3_4Scroll(int amount) : base(1043, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell3_4Scroll(Serial serial) : base(serial)
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

    public class DruidSpell4_1Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell4_1Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell4_1Scroll(int amount) : base(1044, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell4_1Scroll(Serial serial) : base(serial)
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

    public class DruidSpell4_2Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell4_2Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell4_2Scroll(int amount) : base(1045, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell4_2Scroll(Serial serial) : base(serial)
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

    public class DruidSpell4_3Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell4_3Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell4_3Scroll(int amount) : base(1046, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell4_3Scroll(Serial serial) : base(serial)
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

    public class DruidSpell4_4Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell4_4Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell4_4Scroll(int amount) : base(1047, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell4_4Scroll(Serial serial) : base(serial)
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

    public class DruidSpell5_1Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell5_1Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell5_1Scroll(int amount) : base(1048, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell5_1Scroll(Serial serial) : base(serial)
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

    public class DruidSpell5_2Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell5_2Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell5_2Scroll(int amount) : base(1049, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell5_2Scroll(Serial serial) : base(serial)
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

    public class DruidSpell5_3Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell5_3Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell5_3Scroll(int amount) : base(1050, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell5_3Scroll(Serial serial) : base(serial)
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

    public class DruidSpell5_4Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell5_4Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell5_4Scroll(int amount) : base(1051, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell5_4Scroll(Serial serial) : base(serial)
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

    public class DruidSpell6_1Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell6_1Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell6_1Scroll(int amount) : base(1052, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell6_1Scroll(Serial serial) : base(serial)
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

    public class DruidSpell6_2Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell6_2Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell6_2Scroll(int amount) : base(1053, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell6_2Scroll(Serial serial) : base(serial)
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

    public class DruidSpell6_3Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell6_3Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell6_3Scroll(int amount) : base(1054, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell6_3Scroll(Serial serial) : base(serial)
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

    public class DruidSpell6_4Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell6_4Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell6_4Scroll(int amount) : base(1055, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell6_4Scroll(Serial serial) : base(serial)
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

    public class DruidSpell7_1Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell7_1Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell7_1Scroll(int amount) : base(1056, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell7_1Scroll(Serial serial) : base(serial)
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

    public class DruidSpell7_2Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell7_2Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell7_2Scroll(int amount) : base(1057, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell7_2Scroll(Serial serial) : base(serial)
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

    public class DruidSpell7_3Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell7_3Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell7_3Scroll(int amount) : base(1058, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell7_3Scroll(Serial serial) : base(serial)
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

    public class DruidSpell7_4Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell7_4Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell7_4Scroll(int amount) : base(1059, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell7_4Scroll(Serial serial) : base(serial)
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

    public class DruidSpell8_1Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell8_1Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell8_1Scroll(int amount) : base(1060, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell8_1Scroll(Serial serial) : base(serial)
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

    public class DruidSpell8_2Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell8_2Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell8_2Scroll(int amount) : base(1061, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell8_2Scroll(Serial serial) : base(serial)
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

    public class DruidSpell8_3Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell8_3Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell8_3Scroll(int amount) : base(1062, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell8_3Scroll(Serial serial) : base(serial)
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

    public class DruidSpell8_4Scroll : SpellScroll
    {
        [Constructable]
        public DruidSpell8_4Scroll() : this(1)
        {
        }

        [Constructable]
        public DruidSpell8_4Scroll(int amount) : base(1063, 0x1F2D, amount)
        {
            Hue = 0x7D6;
        }

        public DruidSpell8_4Scroll(Serial serial) : base(serial)
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
