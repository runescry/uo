using System;
using Server;
using Server.Items;

namespace Server.Items.VystiaScrolls
{
    /// <summary>
    /// Spell scrolls for Warlock magic
    /// </summary>

    public class WarlockSpell1_1Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell1_1Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell1_1Scroll(int amount) : base(1128, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell1_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell1_2Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell1_2Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell1_2Scroll(int amount) : base(1129, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell1_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell1_3Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell1_3Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell1_3Scroll(int amount) : base(1130, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell1_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell1_4Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell1_4Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell1_4Scroll(int amount) : base(1131, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell1_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell2_1Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell2_1Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell2_1Scroll(int amount) : base(1132, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell2_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell2_2Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell2_2Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell2_2Scroll(int amount) : base(1133, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell2_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell2_3Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell2_3Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell2_3Scroll(int amount) : base(1134, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell2_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell2_4Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell2_4Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell2_4Scroll(int amount) : base(1135, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell2_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell3_1Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell3_1Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell3_1Scroll(int amount) : base(1136, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell3_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell3_2Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell3_2Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell3_2Scroll(int amount) : base(1137, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell3_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell3_3Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell3_3Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell3_3Scroll(int amount) : base(1138, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell3_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell3_4Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell3_4Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell3_4Scroll(int amount) : base(1139, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell3_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell4_1Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell4_1Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell4_1Scroll(int amount) : base(1140, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell4_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell4_2Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell4_2Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell4_2Scroll(int amount) : base(1141, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell4_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell4_3Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell4_3Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell4_3Scroll(int amount) : base(1142, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell4_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell4_4Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell4_4Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell4_4Scroll(int amount) : base(1143, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell4_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell5_1Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell5_1Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell5_1Scroll(int amount) : base(1144, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell5_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell5_2Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell5_2Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell5_2Scroll(int amount) : base(1145, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell5_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell5_3Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell5_3Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell5_3Scroll(int amount) : base(1146, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell5_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell5_4Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell5_4Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell5_4Scroll(int amount) : base(1147, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell5_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell6_1Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell6_1Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell6_1Scroll(int amount) : base(1148, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell6_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell6_2Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell6_2Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell6_2Scroll(int amount) : base(1149, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell6_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell6_3Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell6_3Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell6_3Scroll(int amount) : base(1150, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell6_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell6_4Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell6_4Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell6_4Scroll(int amount) : base(1151, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell6_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell7_1Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell7_1Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell7_1Scroll(int amount) : base(1152, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell7_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell7_2Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell7_2Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell7_2Scroll(int amount) : base(1153, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell7_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell7_3Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell7_3Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell7_3Scroll(int amount) : base(1154, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell7_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell7_4Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell7_4Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell7_4Scroll(int amount) : base(1155, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell7_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell8_1Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell8_1Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell8_1Scroll(int amount) : base(1156, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell8_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell8_2Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell8_2Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell8_2Scroll(int amount) : base(1157, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell8_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell8_3Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell8_3Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell8_3Scroll(int amount) : base(1158, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell8_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WarlockSpell8_4Scroll : SpellScroll
    {
        [Constructable]
        public WarlockSpell8_4Scroll() : this(1)
        {
        }

        [Constructable]
        public WarlockSpell8_4Scroll(int amount) : base(1159, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public WarlockSpell8_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
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
