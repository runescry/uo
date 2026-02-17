using System;
using Server;
using Server.Items;

namespace Server.Items.VystiaScrolls
{
    /// <summary>
    /// Spell scrolls for Enchanter magic
    /// </summary>

    public class EnchanterSpell1_1Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell1_1Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell1_1Scroll(int amount) : base(1320, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell1_1Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell1_2Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell1_2Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell1_2Scroll(int amount) : base(1321, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell1_2Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell1_3Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell1_3Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell1_3Scroll(int amount) : base(1322, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell1_3Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell1_4Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell1_4Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell1_4Scroll(int amount) : base(1323, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell1_4Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell2_1Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell2_1Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell2_1Scroll(int amount) : base(1324, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell2_1Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell2_2Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell2_2Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell2_2Scroll(int amount) : base(1325, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell2_2Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell2_3Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell2_3Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell2_3Scroll(int amount) : base(1326, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell2_3Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell2_4Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell2_4Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell2_4Scroll(int amount) : base(1327, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell2_4Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell3_1Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell3_1Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell3_1Scroll(int amount) : base(1328, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell3_1Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell3_2Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell3_2Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell3_2Scroll(int amount) : base(1329, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell3_2Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell3_3Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell3_3Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell3_3Scroll(int amount) : base(1330, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell3_3Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell3_4Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell3_4Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell3_4Scroll(int amount) : base(1331, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell3_4Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell4_1Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell4_1Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell4_1Scroll(int amount) : base(1332, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell4_1Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell4_2Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell4_2Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell4_2Scroll(int amount) : base(1333, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell4_2Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell4_3Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell4_3Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell4_3Scroll(int amount) : base(1334, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell4_3Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell4_4Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell4_4Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell4_4Scroll(int amount) : base(1335, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell4_4Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell5_1Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell5_1Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell5_1Scroll(int amount) : base(1336, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell5_1Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell5_2Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell5_2Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell5_2Scroll(int amount) : base(1337, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell5_2Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell5_3Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell5_3Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell5_3Scroll(int amount) : base(1338, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell5_3Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell5_4Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell5_4Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell5_4Scroll(int amount) : base(1339, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell5_4Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell6_1Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell6_1Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell6_1Scroll(int amount) : base(1340, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell6_1Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell6_2Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell6_2Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell6_2Scroll(int amount) : base(1341, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell6_2Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell6_3Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell6_3Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell6_3Scroll(int amount) : base(1342, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell6_3Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell6_4Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell6_4Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell6_4Scroll(int amount) : base(1343, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell6_4Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell7_1Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell7_1Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell7_1Scroll(int amount) : base(1344, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell7_1Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell7_2Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell7_2Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell7_2Scroll(int amount) : base(1345, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell7_2Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell7_3Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell7_3Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell7_3Scroll(int amount) : base(1346, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell7_3Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell7_4Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell7_4Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell7_4Scroll(int amount) : base(1347, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell7_4Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell8_1Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell8_1Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell8_1Scroll(int amount) : base(1348, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell8_1Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell8_2Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell8_2Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell8_2Scroll(int amount) : base(1349, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell8_2Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell8_3Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell8_3Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell8_3Scroll(int amount) : base(1350, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell8_3Scroll(Serial serial) : base(serial)
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

    public class EnchanterSpell8_4Scroll : SpellScroll
    {
        [Constructable]
        public EnchanterSpell8_4Scroll() : this(1)
        {
        }

        [Constructable]
        public EnchanterSpell8_4Scroll(int amount) : base(1351, 0x1F2D, amount)
        {
            Hue = 0x8FD;
        }

        public EnchanterSpell8_4Scroll(Serial serial) : base(serial)
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
