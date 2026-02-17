using System;
using Server;
using Server.Items;

namespace Server.Items.VystiaScrolls
{
    /// <summary>
    /// Spell scrolls for Witch magic
    /// </summary>

    public class WitchSpell1_1Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell1_1Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell1_1Scroll(int amount) : base(1064, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell1_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell1_2Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell1_2Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell1_2Scroll(int amount) : base(1065, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell1_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell1_3Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell1_3Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell1_3Scroll(int amount) : base(1066, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell1_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell1_4Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell1_4Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell1_4Scroll(int amount) : base(1067, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell1_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell2_1Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell2_1Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell2_1Scroll(int amount) : base(1068, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell2_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell2_2Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell2_2Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell2_2Scroll(int amount) : base(1069, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell2_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell2_3Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell2_3Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell2_3Scroll(int amount) : base(1070, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell2_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell2_4Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell2_4Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell2_4Scroll(int amount) : base(1071, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell2_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell3_1Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell3_1Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell3_1Scroll(int amount) : base(1072, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell3_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell3_2Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell3_2Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell3_2Scroll(int amount) : base(1073, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell3_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell3_3Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell3_3Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell3_3Scroll(int amount) : base(1074, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell3_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell3_4Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell3_4Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell3_4Scroll(int amount) : base(1075, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell3_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell4_1Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell4_1Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell4_1Scroll(int amount) : base(1076, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell4_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell4_2Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell4_2Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell4_2Scroll(int amount) : base(1077, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell4_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell4_3Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell4_3Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell4_3Scroll(int amount) : base(1078, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell4_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell4_4Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell4_4Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell4_4Scroll(int amount) : base(1079, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell4_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell5_1Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell5_1Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell5_1Scroll(int amount) : base(1080, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell5_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell5_2Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell5_2Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell5_2Scroll(int amount) : base(1081, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell5_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell5_3Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell5_3Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell5_3Scroll(int amount) : base(1082, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell5_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell5_4Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell5_4Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell5_4Scroll(int amount) : base(1083, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell5_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell6_1Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell6_1Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell6_1Scroll(int amount) : base(1084, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell6_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell6_2Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell6_2Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell6_2Scroll(int amount) : base(1085, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell6_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell6_3Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell6_3Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell6_3Scroll(int amount) : base(1086, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell6_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell6_4Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell6_4Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell6_4Scroll(int amount) : base(1087, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell6_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell7_1Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell7_1Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell7_1Scroll(int amount) : base(1088, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell7_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell7_2Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell7_2Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell7_2Scroll(int amount) : base(1089, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell7_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell7_3Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell7_3Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell7_3Scroll(int amount) : base(1090, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell7_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell7_4Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell7_4Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell7_4Scroll(int amount) : base(1091, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell7_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell8_1Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell8_1Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell8_1Scroll(int amount) : base(1092, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell8_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell8_2Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell8_2Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell8_2Scroll(int amount) : base(1093, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell8_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell8_3Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell8_3Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell8_3Scroll(int amount) : base(1094, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell8_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitchSpell8_4Scroll : SpellScroll
    {
        [Constructable]
        public WitchSpell8_4Scroll() : this(1)
        {
        }

        [Constructable]
        public WitchSpell8_4Scroll(int amount) : base(1095, 0x1F2D, amount)
        {
            Hue = 0x81D;
        }

        public WitchSpell8_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
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
