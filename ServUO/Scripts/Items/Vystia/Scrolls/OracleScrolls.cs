using System;
using Server;
using Server.Items;

namespace Server.Items.VystiaScrolls
{
    /// <summary>
    /// Spell scrolls for Oracle magic
    /// </summary>

    public class OracleSpell1_1Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell1_1Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell1_1Scroll(int amount) : base(1160, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell1_1Scroll(Serial serial) : base(serial)
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

    public class OracleSpell1_2Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell1_2Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell1_2Scroll(int amount) : base(1161, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell1_2Scroll(Serial serial) : base(serial)
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

    public class OracleSpell1_3Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell1_3Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell1_3Scroll(int amount) : base(1162, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell1_3Scroll(Serial serial) : base(serial)
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

    public class OracleSpell1_4Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell1_4Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell1_4Scroll(int amount) : base(1163, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell1_4Scroll(Serial serial) : base(serial)
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

    public class OracleSpell2_1Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell2_1Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell2_1Scroll(int amount) : base(1164, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell2_1Scroll(Serial serial) : base(serial)
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

    public class OracleSpell2_2Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell2_2Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell2_2Scroll(int amount) : base(1165, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell2_2Scroll(Serial serial) : base(serial)
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

    public class OracleSpell2_3Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell2_3Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell2_3Scroll(int amount) : base(1166, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell2_3Scroll(Serial serial) : base(serial)
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

    public class OracleSpell2_4Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell2_4Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell2_4Scroll(int amount) : base(1167, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell2_4Scroll(Serial serial) : base(serial)
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

    public class OracleSpell3_1Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell3_1Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell3_1Scroll(int amount) : base(1168, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell3_1Scroll(Serial serial) : base(serial)
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

    public class OracleSpell3_2Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell3_2Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell3_2Scroll(int amount) : base(1169, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell3_2Scroll(Serial serial) : base(serial)
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

    public class OracleSpell3_3Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell3_3Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell3_3Scroll(int amount) : base(1170, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell3_3Scroll(Serial serial) : base(serial)
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

    public class OracleSpell3_4Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell3_4Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell3_4Scroll(int amount) : base(1171, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell3_4Scroll(Serial serial) : base(serial)
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

    public class OracleSpell4_1Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell4_1Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell4_1Scroll(int amount) : base(1172, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell4_1Scroll(Serial serial) : base(serial)
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

    public class OracleSpell4_2Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell4_2Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell4_2Scroll(int amount) : base(1173, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell4_2Scroll(Serial serial) : base(serial)
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

    public class OracleSpell4_3Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell4_3Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell4_3Scroll(int amount) : base(1174, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell4_3Scroll(Serial serial) : base(serial)
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

    public class OracleSpell4_4Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell4_4Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell4_4Scroll(int amount) : base(1175, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell4_4Scroll(Serial serial) : base(serial)
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

    public class OracleSpell5_1Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell5_1Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell5_1Scroll(int amount) : base(1176, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell5_1Scroll(Serial serial) : base(serial)
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

    public class OracleSpell5_2Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell5_2Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell5_2Scroll(int amount) : base(1177, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell5_2Scroll(Serial serial) : base(serial)
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

    public class OracleSpell5_3Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell5_3Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell5_3Scroll(int amount) : base(1178, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell5_3Scroll(Serial serial) : base(serial)
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

    public class OracleSpell5_4Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell5_4Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell5_4Scroll(int amount) : base(1179, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell5_4Scroll(Serial serial) : base(serial)
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

    public class OracleSpell6_1Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell6_1Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell6_1Scroll(int amount) : base(1180, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell6_1Scroll(Serial serial) : base(serial)
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

    public class OracleSpell6_2Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell6_2Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell6_2Scroll(int amount) : base(1181, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell6_2Scroll(Serial serial) : base(serial)
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

    public class OracleSpell6_3Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell6_3Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell6_3Scroll(int amount) : base(1182, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell6_3Scroll(Serial serial) : base(serial)
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

    public class OracleSpell6_4Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell6_4Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell6_4Scroll(int amount) : base(1183, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell6_4Scroll(Serial serial) : base(serial)
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

    public class OracleSpell7_1Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell7_1Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell7_1Scroll(int amount) : base(1184, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell7_1Scroll(Serial serial) : base(serial)
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

    public class OracleSpell7_2Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell7_2Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell7_2Scroll(int amount) : base(1185, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell7_2Scroll(Serial serial) : base(serial)
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

    public class OracleSpell7_3Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell7_3Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell7_3Scroll(int amount) : base(1186, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell7_3Scroll(Serial serial) : base(serial)
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

    public class OracleSpell7_4Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell7_4Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell7_4Scroll(int amount) : base(1187, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell7_4Scroll(Serial serial) : base(serial)
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

    public class OracleSpell8_1Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell8_1Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell8_1Scroll(int amount) : base(1188, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell8_1Scroll(Serial serial) : base(serial)
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

    public class OracleSpell8_2Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell8_2Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell8_2Scroll(int amount) : base(1189, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell8_2Scroll(Serial serial) : base(serial)
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

    public class OracleSpell8_3Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell8_3Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell8_3Scroll(int amount) : base(1190, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell8_3Scroll(Serial serial) : base(serial)
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

    public class OracleSpell8_4Scroll : SpellScroll
    {
        [Constructable]
        public OracleSpell8_4Scroll() : this(1)
        {
        }

        [Constructable]
        public OracleSpell8_4Scroll(int amount) : base(1191, 0x1F2D, amount)
        {
            Hue = 0x482;
        }

        public OracleSpell8_4Scroll(Serial serial) : base(serial)
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
