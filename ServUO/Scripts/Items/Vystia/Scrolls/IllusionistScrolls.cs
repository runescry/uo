using System;
using Server;
using Server.Items;

namespace Server.Items.VystiaScrolls
{
    /// <summary>
    /// Spell scrolls for Illusionist magic
    /// </summary>

    public class IllusionistSpell1_1Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell1_1Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell1_1Scroll(int amount) : base(1352, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell1_1Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell1_2Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell1_2Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell1_2Scroll(int amount) : base(1353, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell1_2Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell1_3Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell1_3Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell1_3Scroll(int amount) : base(1354, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell1_3Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell1_4Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell1_4Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell1_4Scroll(int amount) : base(1355, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell1_4Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell2_1Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell2_1Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell2_1Scroll(int amount) : base(1356, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell2_1Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell2_2Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell2_2Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell2_2Scroll(int amount) : base(1357, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell2_2Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell2_3Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell2_3Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell2_3Scroll(int amount) : base(1358, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell2_3Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell2_4Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell2_4Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell2_4Scroll(int amount) : base(1359, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell2_4Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell3_1Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell3_1Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell3_1Scroll(int amount) : base(1360, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell3_1Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell3_2Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell3_2Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell3_2Scroll(int amount) : base(1361, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell3_2Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell3_3Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell3_3Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell3_3Scroll(int amount) : base(1362, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell3_3Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell3_4Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell3_4Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell3_4Scroll(int amount) : base(1363, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell3_4Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell4_1Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell4_1Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell4_1Scroll(int amount) : base(1364, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell4_1Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell4_2Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell4_2Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell4_2Scroll(int amount) : base(1365, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell4_2Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell4_3Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell4_3Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell4_3Scroll(int amount) : base(1366, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell4_3Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell4_4Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell4_4Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell4_4Scroll(int amount) : base(1367, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell4_4Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell5_1Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell5_1Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell5_1Scroll(int amount) : base(1368, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell5_1Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell5_2Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell5_2Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell5_2Scroll(int amount) : base(1369, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell5_2Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell5_3Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell5_3Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell5_3Scroll(int amount) : base(1370, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell5_3Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell5_4Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell5_4Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell5_4Scroll(int amount) : base(1371, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell5_4Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell6_1Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell6_1Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell6_1Scroll(int amount) : base(1372, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell6_1Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell6_2Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell6_2Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell6_2Scroll(int amount) : base(1373, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell6_2Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell6_3Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell6_3Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell6_3Scroll(int amount) : base(1374, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell6_3Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell6_4Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell6_4Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell6_4Scroll(int amount) : base(1375, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell6_4Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell7_1Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell7_1Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell7_1Scroll(int amount) : base(1376, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell7_1Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell7_2Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell7_2Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell7_2Scroll(int amount) : base(1377, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell7_2Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell7_3Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell7_3Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell7_3Scroll(int amount) : base(1378, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell7_3Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell7_4Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell7_4Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell7_4Scroll(int amount) : base(1379, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell7_4Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell8_1Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell8_1Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell8_1Scroll(int amount) : base(1380, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell8_1Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell8_2Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell8_2Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell8_2Scroll(int amount) : base(1381, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell8_2Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell8_3Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell8_3Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell8_3Scroll(int amount) : base(1382, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell8_3Scroll(Serial serial) : base(serial)
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

    public class IllusionistSpell8_4Scroll : SpellScroll
    {
        [Constructable]
        public IllusionistSpell8_4Scroll() : this(1)
        {
        }

        [Constructable]
        public IllusionistSpell8_4Scroll(int amount) : base(1383, 0x1F2D, amount)
        {
            Hue = 0x47E;
        }

        public IllusionistSpell8_4Scroll(Serial serial) : base(serial)
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
