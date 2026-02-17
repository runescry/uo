using System;
using Server;
using Server.Items;

namespace Server.Items.VystiaScrolls
{
    /// <summary>
    /// Spell scrolls for Shaman magic
    /// </summary>

    public class ShamanSpell1_1Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell1_1Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell1_1Scroll(int amount) : base(1256, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell1_1Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell1_2Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell1_2Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell1_2Scroll(int amount) : base(1257, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell1_2Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell1_3Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell1_3Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell1_3Scroll(int amount) : base(1258, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell1_3Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell1_4Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell1_4Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell1_4Scroll(int amount) : base(1259, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell1_4Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell2_1Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell2_1Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell2_1Scroll(int amount) : base(1260, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell2_1Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell2_2Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell2_2Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell2_2Scroll(int amount) : base(1261, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell2_2Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell2_3Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell2_3Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell2_3Scroll(int amount) : base(1262, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell2_3Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell2_4Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell2_4Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell2_4Scroll(int amount) : base(1263, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell2_4Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell3_1Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell3_1Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell3_1Scroll(int amount) : base(1264, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell3_1Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell3_2Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell3_2Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell3_2Scroll(int amount) : base(1265, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell3_2Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell3_3Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell3_3Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell3_3Scroll(int amount) : base(1266, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell3_3Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell3_4Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell3_4Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell3_4Scroll(int amount) : base(1267, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell3_4Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell4_1Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell4_1Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell4_1Scroll(int amount) : base(1268, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell4_1Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell4_2Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell4_2Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell4_2Scroll(int amount) : base(1269, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell4_2Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell4_3Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell4_3Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell4_3Scroll(int amount) : base(1270, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell4_3Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell4_4Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell4_4Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell4_4Scroll(int amount) : base(1271, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell4_4Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell5_1Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell5_1Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell5_1Scroll(int amount) : base(1272, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell5_1Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell5_2Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell5_2Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell5_2Scroll(int amount) : base(1273, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell5_2Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell5_3Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell5_3Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell5_3Scroll(int amount) : base(1274, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell5_3Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell5_4Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell5_4Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell5_4Scroll(int amount) : base(1275, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell5_4Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell6_1Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell6_1Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell6_1Scroll(int amount) : base(1276, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell6_1Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell6_2Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell6_2Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell6_2Scroll(int amount) : base(1277, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell6_2Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell6_3Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell6_3Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell6_3Scroll(int amount) : base(1278, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell6_3Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell6_4Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell6_4Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell6_4Scroll(int amount) : base(1279, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell6_4Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell7_1Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell7_1Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell7_1Scroll(int amount) : base(1280, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell7_1Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell7_2Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell7_2Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell7_2Scroll(int amount) : base(1281, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell7_2Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell7_3Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell7_3Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell7_3Scroll(int amount) : base(1282, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell7_3Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell7_4Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell7_4Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell7_4Scroll(int amount) : base(1283, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell7_4Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell8_1Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell8_1Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell8_1Scroll(int amount) : base(1284, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell8_1Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell8_2Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell8_2Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell8_2Scroll(int amount) : base(1285, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell8_2Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell8_3Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell8_3Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell8_3Scroll(int amount) : base(1286, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell8_3Scroll(Serial serial) : base(serial)
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

    public class ShamanSpell8_4Scroll : SpellScroll
    {
        [Constructable]
        public ShamanSpell8_4Scroll() : this(1)
        {
        }

        [Constructable]
        public ShamanSpell8_4Scroll(int amount) : base(1287, 0x1F2D, amount)
        {
            Hue = 0x501;
        }

        public ShamanSpell8_4Scroll(Serial serial) : base(serial)
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
