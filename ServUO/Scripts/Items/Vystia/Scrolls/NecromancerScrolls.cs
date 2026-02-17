using System;
using Server;
using Server.Items;

namespace Server.Items.VystiaScrolls
{
    /// <summary>
    /// Spell scrolls for Necromancer magic
    /// </summary>

    public class NecromancerSpell1_1Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell1_1Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell1_1Scroll(int amount) : base(1192, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell1_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell1_2Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell1_2Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell1_2Scroll(int amount) : base(1193, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell1_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell1_3Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell1_3Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell1_3Scroll(int amount) : base(1194, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell1_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell1_4Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell1_4Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell1_4Scroll(int amount) : base(1195, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell1_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell2_1Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell2_1Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell2_1Scroll(int amount) : base(1196, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell2_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell2_2Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell2_2Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell2_2Scroll(int amount) : base(1197, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell2_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell2_3Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell2_3Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell2_3Scroll(int amount) : base(1198, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell2_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell2_4Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell2_4Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell2_4Scroll(int amount) : base(1199, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell2_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell3_1Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell3_1Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell3_1Scroll(int amount) : base(1200, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell3_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell3_2Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell3_2Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell3_2Scroll(int amount) : base(1201, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell3_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell3_3Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell3_3Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell3_3Scroll(int amount) : base(1202, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell3_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell3_4Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell3_4Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell3_4Scroll(int amount) : base(1203, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell3_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell4_1Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell4_1Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell4_1Scroll(int amount) : base(1204, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell4_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell4_2Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell4_2Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell4_2Scroll(int amount) : base(1205, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell4_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell4_3Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell4_3Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell4_3Scroll(int amount) : base(1206, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell4_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell4_4Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell4_4Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell4_4Scroll(int amount) : base(1207, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell4_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell5_1Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell5_1Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell5_1Scroll(int amount) : base(1208, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell5_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell5_2Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell5_2Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell5_2Scroll(int amount) : base(1209, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell5_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell5_3Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell5_3Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell5_3Scroll(int amount) : base(1210, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell5_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell5_4Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell5_4Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell5_4Scroll(int amount) : base(1211, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell5_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell6_1Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell6_1Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell6_1Scroll(int amount) : base(1212, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell6_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell6_2Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell6_2Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell6_2Scroll(int amount) : base(1213, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell6_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell6_3Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell6_3Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell6_3Scroll(int amount) : base(1214, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell6_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell6_4Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell6_4Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell6_4Scroll(int amount) : base(1215, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell6_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell7_1Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell7_1Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell7_1Scroll(int amount) : base(1216, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell7_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell7_2Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell7_2Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell7_2Scroll(int amount) : base(1217, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell7_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell7_3Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell7_3Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell7_3Scroll(int amount) : base(1218, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell7_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell7_4Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell7_4Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell7_4Scroll(int amount) : base(1219, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell7_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell8_1Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell8_1Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell8_1Scroll(int amount) : base(1220, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell8_1Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell8_2Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell8_2Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell8_2Scroll(int amount) : base(1221, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell8_2Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell8_3Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell8_3Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell8_3Scroll(int amount) : base(1222, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell8_3Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecromancerSpell8_4Scroll : SpellScroll
    {
        [Constructable]
        public NecromancerSpell8_4Scroll() : this(1)
        {
        }

        [Constructable]
        public NecromancerSpell8_4Scroll(int amount) : base(1223, 0x1F2D, amount)
        {
            Hue = 0x455;
        }

        public NecromancerSpell8_4Scroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
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
