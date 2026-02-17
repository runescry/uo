using System;
using Server;
using Server.Items;

namespace Server.Items.VystiaScrolls
{
    /// <summary>
    /// Spell scrolls for Sorcerer magic
    /// </summary>

    public class SorcererSpell1_1Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell1_1Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell1_1Scroll(int amount) : base(1096, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell1_1Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell1_2Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell1_2Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell1_2Scroll(int amount) : base(1097, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell1_2Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell1_3Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell1_3Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell1_3Scroll(int amount) : base(1098, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell1_3Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell1_4Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell1_4Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell1_4Scroll(int amount) : base(1099, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell1_4Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell2_1Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell2_1Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell2_1Scroll(int amount) : base(1100, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell2_1Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell2_2Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell2_2Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell2_2Scroll(int amount) : base(1101, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell2_2Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell2_3Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell2_3Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell2_3Scroll(int amount) : base(1102, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell2_3Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell2_4Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell2_4Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell2_4Scroll(int amount) : base(1103, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell2_4Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell3_1Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell3_1Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell3_1Scroll(int amount) : base(1104, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell3_1Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell3_2Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell3_2Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell3_2Scroll(int amount) : base(1105, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell3_2Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell3_3Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell3_3Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell3_3Scroll(int amount) : base(1106, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell3_3Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell3_4Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell3_4Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell3_4Scroll(int amount) : base(1107, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell3_4Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell4_1Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell4_1Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell4_1Scroll(int amount) : base(1108, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell4_1Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell4_2Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell4_2Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell4_2Scroll(int amount) : base(1109, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell4_2Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell4_3Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell4_3Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell4_3Scroll(int amount) : base(1110, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell4_3Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell4_4Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell4_4Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell4_4Scroll(int amount) : base(1111, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell4_4Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell5_1Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell5_1Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell5_1Scroll(int amount) : base(1112, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell5_1Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell5_2Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell5_2Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell5_2Scroll(int amount) : base(1113, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell5_2Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell5_3Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell5_3Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell5_3Scroll(int amount) : base(1114, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell5_3Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell5_4Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell5_4Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell5_4Scroll(int amount) : base(1115, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell5_4Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell6_1Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell6_1Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell6_1Scroll(int amount) : base(1116, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell6_1Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell6_2Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell6_2Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell6_2Scroll(int amount) : base(1117, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell6_2Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell6_3Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell6_3Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell6_3Scroll(int amount) : base(1118, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell6_3Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell6_4Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell6_4Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell6_4Scroll(int amount) : base(1119, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell6_4Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell7_1Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell7_1Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell7_1Scroll(int amount) : base(1120, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell7_1Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell7_2Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell7_2Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell7_2Scroll(int amount) : base(1121, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell7_2Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell7_3Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell7_3Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell7_3Scroll(int amount) : base(1122, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell7_3Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell7_4Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell7_4Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell7_4Scroll(int amount) : base(1123, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell7_4Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell8_1Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell8_1Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell8_1Scroll(int amount) : base(1124, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell8_1Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell8_2Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell8_2Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell8_2Scroll(int amount) : base(1125, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell8_2Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell8_3Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell8_3Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell8_3Scroll(int amount) : base(1126, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell8_3Scroll(Serial serial) : base(serial)
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

    public class SorcererSpell8_4Scroll : SpellScroll
    {
        [Constructable]
        public SorcererSpell8_4Scroll() : this(1)
        {
        }

        [Constructable]
        public SorcererSpell8_4Scroll(int amount) : base(1127, 0x1F2D, amount)
        {
            Hue = 0x54E;
        }

        public SorcererSpell8_4Scroll(Serial serial) : base(serial)
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
