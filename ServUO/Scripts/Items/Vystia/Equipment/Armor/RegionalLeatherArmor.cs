using System;
using Server;

namespace Server.Items.Vystia
{
    // ============================================================================
    // FROST LEATHER
    // Winter wolf hide armor
    // ============================================================================

    public class FrostLeatherCap : LeatherCap
    {
        [Constructable]
        public FrostLeatherCap()
        {
            Name = "Frost Leather Cap";
            Hue = 1152;

            // Winter wolf hide armor
            Attributes.BonusDex = 5;
            ColdBonus = 6;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("FROSTHOLD Leather Armor");
        }

        public FrostLeatherCap(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class FrostLeatherGorget : LeatherGorget
    {
        [Constructable]
        public FrostLeatherGorget()
        {
            Name = "Frost Leather Gorget";
            Hue = 1152;

            // Winter wolf hide armor
            Attributes.BonusDex = 5;
            ColdBonus = 4;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("FROSTHOLD Leather Armor");
        }

        public FrostLeatherGorget(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class FrostLeatherTunic : LeatherChest
    {
        [Constructable]
        public FrostLeatherTunic()
        {
            Name = "Frost Leather Tunic";
            Hue = 1152;

            // Winter wolf hide armor
            Attributes.BonusDex = 5;
            ColdBonus = 10;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("FROSTHOLD Leather Armor");
        }

        public FrostLeatherTunic(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class FrostLeatherSleeves : LeatherArms
    {
        [Constructable]
        public FrostLeatherSleeves()
        {
            Name = "Frost Leather Sleeves";
            Hue = 1152;

            // Winter wolf hide armor
            Attributes.BonusDex = 5;
            ColdBonus = 6;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("FROSTHOLD Leather Armor");
        }

        public FrostLeatherSleeves(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class FrostLeatherGloves : LeatherGloves
    {
        [Constructable]
        public FrostLeatherGloves()
        {
            Name = "Frost Leather Gloves";
            Hue = 1152;

            // Winter wolf hide armor
            Attributes.BonusDex = 5;
            ColdBonus = 4;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("FROSTHOLD Leather Armor");
        }

        public FrostLeatherGloves(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class FrostLeatherLegs : LeatherLegs
    {
        [Constructable]
        public FrostLeatherLegs()
        {
            Name = "Frost Leather Legs";
            Hue = 1152;

            // Winter wolf hide armor
            Attributes.BonusDex = 5;
            ColdBonus = 8;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("FROSTHOLD Leather Armor");
        }

        public FrostLeatherLegs(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    // ============================================================================
    // FIRE LEATHER
    // Lava hound hide armor
    // ============================================================================

    public class FireLeatherCap : LeatherCap
    {
        [Constructable]
        public FireLeatherCap()
        {
            Name = "Fire Leather Cap";
            Hue = 1358;

            // Lava hound hide armor
            Attributes.RegenStam = 3;
            FireBonus = 6;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("EMBERLANDS Leather Armor");
        }

        public FireLeatherCap(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class FireLeatherGorget : LeatherGorget
    {
        [Constructable]
        public FireLeatherGorget()
        {
            Name = "Fire Leather Gorget";
            Hue = 1358;

            // Lava hound hide armor
            Attributes.RegenStam = 3;
            FireBonus = 4;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("EMBERLANDS Leather Armor");
        }

        public FireLeatherGorget(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class FireLeatherTunic : LeatherChest
    {
        [Constructable]
        public FireLeatherTunic()
        {
            Name = "Fire Leather Tunic";
            Hue = 1358;

            // Lava hound hide armor
            Attributes.RegenStam = 3;
            FireBonus = 10;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("EMBERLANDS Leather Armor");
        }

        public FireLeatherTunic(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class FireLeatherSleeves : LeatherArms
    {
        [Constructable]
        public FireLeatherSleeves()
        {
            Name = "Fire Leather Sleeves";
            Hue = 1358;

            // Lava hound hide armor
            Attributes.RegenStam = 3;
            FireBonus = 6;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("EMBERLANDS Leather Armor");
        }

        public FireLeatherSleeves(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class FireLeatherGloves : LeatherGloves
    {
        [Constructable]
        public FireLeatherGloves()
        {
            Name = "Fire Leather Gloves";
            Hue = 1358;

            // Lava hound hide armor
            Attributes.RegenStam = 3;
            FireBonus = 4;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("EMBERLANDS Leather Armor");
        }

        public FireLeatherGloves(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class FireLeatherLegs : LeatherLegs
    {
        [Constructable]
        public FireLeatherLegs()
        {
            Name = "Fire Leather Legs";
            Hue = 1358;

            // Lava hound hide armor
            Attributes.RegenStam = 3;
            FireBonus = 8;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("EMBERLANDS Leather Armor");
        }

        public FireLeatherLegs(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    // ============================================================================
    // SHADOW LEATHER
    // Shadow wolf hide armor
    // ============================================================================

    public class ShadowLeatherCap : LeatherCap
    {
        [Constructable]
        public ShadowLeatherCap()
        {
            Name = "Shadow Leather Cap";
            Hue = 1109;

            // Shadow wolf hide armor
            Attributes.NightSight = 1;
            Attributes.BonusDex = 3;
            SkillBonuses.SetValues(0, SkillName.Stealth, 3.0);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("SHADOWFEN Leather Armor");
        }

        public ShadowLeatherCap(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class ShadowLeatherGorget : LeatherGorget
    {
        [Constructable]
        public ShadowLeatherGorget()
        {
            Name = "Shadow Leather Gorget";
            Hue = 1109;

            // Shadow wolf hide armor
            Attributes.NightSight = 1;
            Attributes.BonusDex = 3;
            SkillBonuses.SetValues(0, SkillName.Stealth, 2.0);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("SHADOWFEN Leather Armor");
        }

        public ShadowLeatherGorget(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class ShadowLeatherTunic : LeatherChest
    {
        [Constructable]
        public ShadowLeatherTunic()
        {
            Name = "Shadow Leather Tunic";
            Hue = 1109;

            // Shadow wolf hide armor
            Attributes.NightSight = 1;
            Attributes.BonusDex = 3;
            SkillBonuses.SetValues(0, SkillName.Stealth, 5.0);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("SHADOWFEN Leather Armor");
        }

        public ShadowLeatherTunic(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class ShadowLeatherSleeves : LeatherArms
    {
        [Constructable]
        public ShadowLeatherSleeves()
        {
            Name = "Shadow Leather Sleeves";
            Hue = 1109;

            // Shadow wolf hide armor
            Attributes.NightSight = 1;
            Attributes.BonusDex = 3;
            SkillBonuses.SetValues(0, SkillName.Stealth, 3.0);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("SHADOWFEN Leather Armor");
        }

        public ShadowLeatherSleeves(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class ShadowLeatherGloves : LeatherGloves
    {
        [Constructable]
        public ShadowLeatherGloves()
        {
            Name = "Shadow Leather Gloves";
            Hue = 1109;

            // Shadow wolf hide armor
            Attributes.NightSight = 1;
            Attributes.BonusDex = 3;
            SkillBonuses.SetValues(0, SkillName.Stealth, 3.0);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("SHADOWFEN Leather Armor");
        }

        public ShadowLeatherGloves(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class ShadowLeatherLegs : LeatherLegs
    {
        [Constructable]
        public ShadowLeatherLegs()
        {
            Name = "Shadow Leather Legs";
            Hue = 1109;

            // Shadow wolf hide armor
            Attributes.NightSight = 1;
            Attributes.BonusDex = 3;
            SkillBonuses.SetValues(0, SkillName.Stealth, 4.0);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("SHADOWFEN Leather Armor");
        }

        public ShadowLeatherLegs(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

}
