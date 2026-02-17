using System;
using Server;
using Server.Items;

namespace Server.Items.VystiaScrolls
{
    /// <summary>
    /// Spell scrolls for Summoner magic
    /// </summary>

    public class SummonerSpell1_1Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell1_1Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell1_1Scroll(int amount) : base(1224, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell1_1Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell1_2Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell1_2Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell1_2Scroll(int amount) : base(1225, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell1_2Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell1_3Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell1_3Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell1_3Scroll(int amount) : base(1226, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell1_3Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell1_4Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell1_4Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell1_4Scroll(int amount) : base(1227, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell1_4Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell2_1Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell2_1Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell2_1Scroll(int amount) : base(1228, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell2_1Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell2_2Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell2_2Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell2_2Scroll(int amount) : base(1229, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell2_2Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell2_3Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell2_3Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell2_3Scroll(int amount) : base(1230, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell2_3Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell2_4Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell2_4Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell2_4Scroll(int amount) : base(1231, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell2_4Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell3_1Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell3_1Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell3_1Scroll(int amount) : base(1232, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell3_1Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell3_2Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell3_2Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell3_2Scroll(int amount) : base(1233, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell3_2Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell3_3Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell3_3Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell3_3Scroll(int amount) : base(1234, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell3_3Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell3_4Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell3_4Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell3_4Scroll(int amount) : base(1235, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell3_4Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell4_1Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell4_1Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell4_1Scroll(int amount) : base(1236, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell4_1Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell4_2Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell4_2Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell4_2Scroll(int amount) : base(1237, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell4_2Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell4_3Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell4_3Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell4_3Scroll(int amount) : base(1238, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell4_3Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell4_4Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell4_4Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell4_4Scroll(int amount) : base(1239, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell4_4Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell5_1Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell5_1Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell5_1Scroll(int amount) : base(1240, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell5_1Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell5_2Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell5_2Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell5_2Scroll(int amount) : base(1241, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell5_2Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell5_3Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell5_3Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell5_3Scroll(int amount) : base(1242, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell5_3Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell5_4Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell5_4Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell5_4Scroll(int amount) : base(1243, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell5_4Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell6_1Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell6_1Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell6_1Scroll(int amount) : base(1244, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell6_1Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell6_2Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell6_2Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell6_2Scroll(int amount) : base(1245, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell6_2Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell6_3Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell6_3Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell6_3Scroll(int amount) : base(1246, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell6_3Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell6_4Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell6_4Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell6_4Scroll(int amount) : base(1247, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell6_4Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell7_1Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell7_1Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell7_1Scroll(int amount) : base(1248, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell7_1Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell7_2Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell7_2Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell7_2Scroll(int amount) : base(1249, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell7_2Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell7_3Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell7_3Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell7_3Scroll(int amount) : base(1250, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell7_3Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell7_4Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell7_4Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell7_4Scroll(int amount) : base(1251, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell7_4Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell8_1Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell8_1Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell8_1Scroll(int amount) : base(1252, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell8_1Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell8_2Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell8_2Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell8_2Scroll(int amount) : base(1253, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell8_2Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell8_3Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell8_3Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell8_3Scroll(int amount) : base(1254, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell8_3Scroll(Serial serial) : base(serial)
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

    public class SummonerSpell8_4Scroll : SpellScroll
    {
        [Constructable]
        public SummonerSpell8_4Scroll() : this(1)
        {
        }

        [Constructable]
        public SummonerSpell8_4Scroll(int amount) : base(1255, 0x1F2D, amount)
        {
            Hue = 0x555;
        }

        public SummonerSpell8_4Scroll(Serial serial) : base(serial)
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
