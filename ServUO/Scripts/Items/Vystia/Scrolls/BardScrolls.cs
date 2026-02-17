using System;
using Server;
using Server.Items;

namespace Server.Items.VystiaScrolls
{
    /// <summary>
    /// Spell scrolls for Bard magic
    /// </summary>

    public class BardSpell1_1Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell1_1Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell1_1Scroll(int amount) : base(1288, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell1_1Scroll(Serial serial) : base(serial)
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

    public class BardSpell1_2Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell1_2Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell1_2Scroll(int amount) : base(1289, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell1_2Scroll(Serial serial) : base(serial)
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

    public class BardSpell1_3Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell1_3Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell1_3Scroll(int amount) : base(1290, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell1_3Scroll(Serial serial) : base(serial)
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

    public class BardSpell1_4Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell1_4Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell1_4Scroll(int amount) : base(1291, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell1_4Scroll(Serial serial) : base(serial)
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

    public class BardSpell2_1Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell2_1Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell2_1Scroll(int amount) : base(1292, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell2_1Scroll(Serial serial) : base(serial)
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

    public class BardSpell2_2Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell2_2Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell2_2Scroll(int amount) : base(1293, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell2_2Scroll(Serial serial) : base(serial)
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

    public class BardSpell2_3Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell2_3Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell2_3Scroll(int amount) : base(1294, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell2_3Scroll(Serial serial) : base(serial)
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

    public class BardSpell2_4Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell2_4Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell2_4Scroll(int amount) : base(1295, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell2_4Scroll(Serial serial) : base(serial)
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

    public class BardSpell3_1Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell3_1Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell3_1Scroll(int amount) : base(1296, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell3_1Scroll(Serial serial) : base(serial)
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

    public class BardSpell3_2Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell3_2Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell3_2Scroll(int amount) : base(1297, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell3_2Scroll(Serial serial) : base(serial)
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

    public class BardSpell3_3Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell3_3Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell3_3Scroll(int amount) : base(1298, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell3_3Scroll(Serial serial) : base(serial)
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

    public class BardSpell3_4Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell3_4Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell3_4Scroll(int amount) : base(1299, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell3_4Scroll(Serial serial) : base(serial)
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

    public class BardSpell4_1Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell4_1Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell4_1Scroll(int amount) : base(1300, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell4_1Scroll(Serial serial) : base(serial)
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

    public class BardSpell4_2Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell4_2Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell4_2Scroll(int amount) : base(1301, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell4_2Scroll(Serial serial) : base(serial)
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

    public class BardSpell4_3Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell4_3Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell4_3Scroll(int amount) : base(1302, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell4_3Scroll(Serial serial) : base(serial)
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

    public class BardSpell4_4Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell4_4Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell4_4Scroll(int amount) : base(1303, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell4_4Scroll(Serial serial) : base(serial)
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

    public class BardSpell5_1Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell5_1Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell5_1Scroll(int amount) : base(1304, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell5_1Scroll(Serial serial) : base(serial)
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

    public class BardSpell5_2Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell5_2Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell5_2Scroll(int amount) : base(1305, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell5_2Scroll(Serial serial) : base(serial)
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

    public class BardSpell5_3Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell5_3Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell5_3Scroll(int amount) : base(1306, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell5_3Scroll(Serial serial) : base(serial)
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

    public class BardSpell5_4Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell5_4Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell5_4Scroll(int amount) : base(1307, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell5_4Scroll(Serial serial) : base(serial)
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

    public class BardSpell6_1Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell6_1Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell6_1Scroll(int amount) : base(1308, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell6_1Scroll(Serial serial) : base(serial)
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

    public class BardSpell6_2Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell6_2Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell6_2Scroll(int amount) : base(1309, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell6_2Scroll(Serial serial) : base(serial)
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

    public class BardSpell6_3Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell6_3Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell6_3Scroll(int amount) : base(1310, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell6_3Scroll(Serial serial) : base(serial)
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

    public class BardSpell6_4Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell6_4Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell6_4Scroll(int amount) : base(1311, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell6_4Scroll(Serial serial) : base(serial)
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

    public class BardSpell7_1Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell7_1Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell7_1Scroll(int amount) : base(1312, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell7_1Scroll(Serial serial) : base(serial)
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

    public class BardSpell7_2Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell7_2Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell7_2Scroll(int amount) : base(1313, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell7_2Scroll(Serial serial) : base(serial)
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

    public class BardSpell7_3Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell7_3Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell7_3Scroll(int amount) : base(1314, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell7_3Scroll(Serial serial) : base(serial)
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

    public class BardSpell7_4Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell7_4Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell7_4Scroll(int amount) : base(1315, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell7_4Scroll(Serial serial) : base(serial)
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

    public class BardSpell8_1Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell8_1Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell8_1Scroll(int amount) : base(1316, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell8_1Scroll(Serial serial) : base(serial)
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

    public class BardSpell8_2Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell8_2Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell8_2Scroll(int amount) : base(1317, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell8_2Scroll(Serial serial) : base(serial)
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

    public class BardSpell8_3Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell8_3Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell8_3Scroll(int amount) : base(1318, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell8_3Scroll(Serial serial) : base(serial)
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

    public class BardSpell8_4Scroll : SpellScroll
    {
        [Constructable]
        public BardSpell8_4Scroll() : this(1)
        {
        }

        [Constructable]
        public BardSpell8_4Scroll(int amount) : base(1319, 0x1F2D, amount)
        {
            Hue = 0x8A5;
        }

        public BardSpell8_4Scroll(Serial serial) : base(serial)
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
