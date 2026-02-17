using System;
using Server;

namespace Server.Items.Vystia
{
    // ============================================================================
    // FROSTFORGED PLATE
    // Set Bonus: Cold immunity, +15 Cold Resist
    // ============================================================================

    public class FrostforgedPlateHelm : PlateHelm
    {
        [Constructable]
        public FrostforgedPlateHelm()
        {
            Name = "Frostforged Plate Helm";
            Hue = 1152;

            // Regional armor stats
            Attributes.DefendChance = 5;
            ColdBonus = 8;
            PhysicalBonus = 3;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("FROSTHOLD Crafted");
            list.Add("Part of Frostforged Plate Set");
        }

        public FrostforgedPlateHelm(Serial serial) : base(serial) { }

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

    public class FrostforgedPlateGorget : PlateGorget
    {
        [Constructable]
        public FrostforgedPlateGorget()
        {
            Name = "Frostforged Plate Gorget";
            Hue = 1152;

            // Regional armor stats
            Attributes.DefendChance = 5;
            ColdBonus = 6;
            PhysicalBonus = 2;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("FROSTHOLD Crafted");
            list.Add("Part of Frostforged Plate Set");
        }

        public FrostforgedPlateGorget(Serial serial) : base(serial) { }

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

    public class FrostforgedPlateChest : PlateChest
    {
        [Constructable]
        public FrostforgedPlateChest()
        {
            Name = "Frostforged Plate Chest";
            Hue = 1152;

            // Regional armor stats
            Attributes.DefendChance = 5;
            ColdBonus = 12;
            PhysicalBonus = 5;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("FROSTHOLD Crafted");
            list.Add("Part of Frostforged Plate Set");
        }

        public FrostforgedPlateChest(Serial serial) : base(serial) { }

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

    public class FrostforgedPlateArms : PlateArms
    {
        [Constructable]
        public FrostforgedPlateArms()
        {
            Name = "Frostforged Plate Arms";
            Hue = 1152;

            // Regional armor stats
            Attributes.DefendChance = 5;
            ColdBonus = 8;
            PhysicalBonus = 3;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("FROSTHOLD Crafted");
            list.Add("Part of Frostforged Plate Set");
        }

        public FrostforgedPlateArms(Serial serial) : base(serial) { }

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

    public class FrostforgedPlateGloves : PlateGloves
    {
        [Constructable]
        public FrostforgedPlateGloves()
        {
            Name = "Frostforged Plate Gloves";
            Hue = 1152;

            // Regional armor stats
            Attributes.DefendChance = 5;
            ColdBonus = 6;
            PhysicalBonus = 2;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("FROSTHOLD Crafted");
            list.Add("Part of Frostforged Plate Set");
        }

        public FrostforgedPlateGloves(Serial serial) : base(serial) { }

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

    public class FrostforgedPlateLegs : PlateLegs
    {
        [Constructable]
        public FrostforgedPlateLegs()
        {
            Name = "Frostforged Plate Legs";
            Hue = 1152;

            // Regional armor stats
            Attributes.DefendChance = 5;
            ColdBonus = 10;
            PhysicalBonus = 4;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("FROSTHOLD Crafted");
            list.Add("Part of Frostforged Plate Set");
        }

        public FrostforgedPlateLegs(Serial serial) : base(serial) { }

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
    // EMBERFORGED PLATE
    // Set Bonus: Fire immunity, +15 Fire Resist
    // ============================================================================

    public class EmberforgedPlateHelm : PlateHelm
    {
        [Constructable]
        public EmberforgedPlateHelm()
        {
            Name = "Emberforged Plate Helm";
            Hue = 1358;

            // Regional armor stats
            Attributes.DefendChance = 5;
            FireBonus = 8;
            PhysicalBonus = 3;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("EMBERLANDS Crafted");
            list.Add("Part of Emberforged Plate Set");
        }

        public EmberforgedPlateHelm(Serial serial) : base(serial) { }

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

    public class EmberforgedPlateGorget : PlateGorget
    {
        [Constructable]
        public EmberforgedPlateGorget()
        {
            Name = "Emberforged Plate Gorget";
            Hue = 1358;

            // Regional armor stats
            Attributes.DefendChance = 5;
            FireBonus = 6;
            PhysicalBonus = 2;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("EMBERLANDS Crafted");
            list.Add("Part of Emberforged Plate Set");
        }

        public EmberforgedPlateGorget(Serial serial) : base(serial) { }

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

    public class EmberforgedPlateChest : PlateChest
    {
        [Constructable]
        public EmberforgedPlateChest()
        {
            Name = "Emberforged Plate Chest";
            Hue = 1358;

            // Regional armor stats
            Attributes.DefendChance = 5;
            FireBonus = 12;
            PhysicalBonus = 5;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("EMBERLANDS Crafted");
            list.Add("Part of Emberforged Plate Set");
        }

        public EmberforgedPlateChest(Serial serial) : base(serial) { }

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

    public class EmberforgedPlateArms : PlateArms
    {
        [Constructable]
        public EmberforgedPlateArms()
        {
            Name = "Emberforged Plate Arms";
            Hue = 1358;

            // Regional armor stats
            Attributes.DefendChance = 5;
            FireBonus = 8;
            PhysicalBonus = 3;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("EMBERLANDS Crafted");
            list.Add("Part of Emberforged Plate Set");
        }

        public EmberforgedPlateArms(Serial serial) : base(serial) { }

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

    public class EmberforgedPlateGloves : PlateGloves
    {
        [Constructable]
        public EmberforgedPlateGloves()
        {
            Name = "Emberforged Plate Gloves";
            Hue = 1358;

            // Regional armor stats
            Attributes.DefendChance = 5;
            FireBonus = 6;
            PhysicalBonus = 2;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("EMBERLANDS Crafted");
            list.Add("Part of Emberforged Plate Set");
        }

        public EmberforgedPlateGloves(Serial serial) : base(serial) { }

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

    public class EmberforgedPlateLegs : PlateLegs
    {
        [Constructable]
        public EmberforgedPlateLegs()
        {
            Name = "Emberforged Plate Legs";
            Hue = 1358;

            // Regional armor stats
            Attributes.DefendChance = 5;
            FireBonus = 10;
            PhysicalBonus = 4;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("EMBERLANDS Crafted");
            list.Add("Part of Emberforged Plate Set");
        }

        public EmberforgedPlateLegs(Serial serial) : base(serial) { }

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
    // CLOCKWORK PLATE
    // Set Bonus: +10 STR, +10 Stamina Regen
    // ============================================================================

    public class ClockworkPlateHelm : PlateHelm
    {
        [Constructable]
        public ClockworkPlateHelm()
        {
            Name = "Clockwork Plate Helm";
            Hue = 2401;

            // Regional armor stats
            Attributes.BonusStr = 2;
            Attributes.RegenStam = 1;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("IRONCLAD Crafted");
            list.Add("Part of Clockwork Plate Set");
        }

        public ClockworkPlateHelm(Serial serial) : base(serial) { }

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

    public class ClockworkPlateGorget : PlateGorget
    {
        [Constructable]
        public ClockworkPlateGorget()
        {
            Name = "Clockwork Plate Gorget";
            Hue = 2401;

            // Regional armor stats
            Attributes.BonusStr = 2;
            Attributes.RegenStam = 1;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("IRONCLAD Crafted");
            list.Add("Part of Clockwork Plate Set");
        }

        public ClockworkPlateGorget(Serial serial) : base(serial) { }

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

    public class ClockworkPlateChest : PlateChest
    {
        [Constructable]
        public ClockworkPlateChest()
        {
            Name = "Clockwork Plate Chest";
            Hue = 2401;

            // Regional armor stats
            Attributes.BonusStr = 2;
            Attributes.RegenStam = 1;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("IRONCLAD Crafted");
            list.Add("Part of Clockwork Plate Set");
        }

        public ClockworkPlateChest(Serial serial) : base(serial) { }

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

    public class ClockworkPlateArms : PlateArms
    {
        [Constructable]
        public ClockworkPlateArms()
        {
            Name = "Clockwork Plate Arms";
            Hue = 2401;

            // Regional armor stats
            Attributes.BonusStr = 2;
            Attributes.RegenStam = 1;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("IRONCLAD Crafted");
            list.Add("Part of Clockwork Plate Set");
        }

        public ClockworkPlateArms(Serial serial) : base(serial) { }

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

    public class ClockworkPlateGloves : PlateGloves
    {
        [Constructable]
        public ClockworkPlateGloves()
        {
            Name = "Clockwork Plate Gloves";
            Hue = 2401;

            // Regional armor stats
            Attributes.BonusStr = 2;
            Attributes.RegenStam = 1;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("IRONCLAD Crafted");
            list.Add("Part of Clockwork Plate Set");
        }

        public ClockworkPlateGloves(Serial serial) : base(serial) { }

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

    public class ClockworkPlateLegs : PlateLegs
    {
        [Constructable]
        public ClockworkPlateLegs()
        {
            Name = "Clockwork Plate Legs";
            Hue = 2401;

            // Regional armor stats
            Attributes.BonusStr = 2;
            Attributes.RegenStam = 1;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("IRONCLAD Crafted");
            list.Add("Part of Clockwork Plate Set");
        }

        public ClockworkPlateLegs(Serial serial) : base(serial) { }

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
    // VOIDFORGED PLATE
    // Set Bonus: Mage Armor, +10 Magic Resist
    // ============================================================================

    public class VoidforgedPlateHelm : PlateHelm
    {
        [Constructable]
        public VoidforgedPlateHelm()
        {
            Name = "Voidforged Plate Helm";
            Hue = 1109;

            // Regional armor stats
            Attributes.SpellChanneling = 1;
            Attributes.CastSpeed = -1;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("SHADOWVOID Crafted");
            list.Add("Part of Voidforged Plate Set");
        }

        public VoidforgedPlateHelm(Serial serial) : base(serial) { }

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

    public class VoidforgedPlateGorget : PlateGorget
    {
        [Constructable]
        public VoidforgedPlateGorget()
        {
            Name = "Voidforged Plate Gorget";
            Hue = 1109;

            // Regional armor stats
            Attributes.SpellChanneling = 1;
            Attributes.CastSpeed = -1;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("SHADOWVOID Crafted");
            list.Add("Part of Voidforged Plate Set");
        }

        public VoidforgedPlateGorget(Serial serial) : base(serial) { }

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

    public class VoidforgedPlateChest : PlateChest
    {
        [Constructable]
        public VoidforgedPlateChest()
        {
            Name = "Voidforged Plate Chest";
            Hue = 1109;

            // Regional armor stats
            Attributes.SpellChanneling = 1;
            Attributes.CastSpeed = -1;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("SHADOWVOID Crafted");
            list.Add("Part of Voidforged Plate Set");
        }

        public VoidforgedPlateChest(Serial serial) : base(serial) { }

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

    public class VoidforgedPlateArms : PlateArms
    {
        [Constructable]
        public VoidforgedPlateArms()
        {
            Name = "Voidforged Plate Arms";
            Hue = 1109;

            // Regional armor stats
            Attributes.SpellChanneling = 1;
            Attributes.CastSpeed = -1;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("SHADOWVOID Crafted");
            list.Add("Part of Voidforged Plate Set");
        }

        public VoidforgedPlateArms(Serial serial) : base(serial) { }

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

    public class VoidforgedPlateGloves : PlateGloves
    {
        [Constructable]
        public VoidforgedPlateGloves()
        {
            Name = "Voidforged Plate Gloves";
            Hue = 1109;

            // Regional armor stats
            Attributes.SpellChanneling = 1;
            Attributes.CastSpeed = -1;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("SHADOWVOID Crafted");
            list.Add("Part of Voidforged Plate Set");
        }

        public VoidforgedPlateGloves(Serial serial) : base(serial) { }

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

    public class VoidforgedPlateLegs : PlateLegs
    {
        [Constructable]
        public VoidforgedPlateLegs()
        {
            Name = "Voidforged Plate Legs";
            Hue = 1109;

            // Regional armor stats
            Attributes.SpellChanneling = 1;
            Attributes.CastSpeed = -1;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("SHADOWVOID Crafted");
            list.Add("Part of Voidforged Plate Set");
        }

        public VoidforgedPlateLegs(Serial serial) : base(serial) { }

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
