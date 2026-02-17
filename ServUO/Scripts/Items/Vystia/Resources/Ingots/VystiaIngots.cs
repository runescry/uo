using System;

namespace Server.Items
{
    /// <summary>
    /// Base class for all Vystia regional ingots.
    /// These are standalone items smelted from regional ores.
    /// </summary>
    public abstract class BaseVystiaIngot : Item
    {
        private string m_Region;
        private string m_Property;

        [CommandProperty(AccessLevel.GameMaster)]
        public string Region { get { return m_Region; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Property { get { return m_Property; } }

        public BaseVystiaIngot(int hue, string region, string property)
            : this(1, hue, region, property)
        {
        }

        public BaseVystiaIngot(int amount, int hue, string region, string property)
            : base(0x1BF2) // Ingot graphic
        {
            Stackable = true;
            Amount = amount;
            Hue = hue;
            m_Region = region;
            m_Property = property;
        }

        public BaseVystiaIngot(Serial serial)
            : base(serial)
        {
        }

        public override double DefaultWeight { get { return 0.1; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.Write(m_Region);
            writer.Write(m_Property);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Region = reader.ReadString();
            m_Property = reader.ReadString();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: {0}", m_Region);
            list.Add(m_Property);
        }
    }

    /// <summary>
    /// Frostforged Ingot - smelted from Frozen Ore
    /// Cold damage +5 when crafted into equipment
    /// </summary>
    [Flipable(0x1BF2, 0x1BEF)]
    public class FrostforgedIngot : BaseVystiaIngot
    {
        [Constructable]
        public FrostforgedIngot() : this(1) { }

        [Constructable]
        public FrostforgedIngot(int amount) : base(amount, 1152, "Frosthold", "Cold Damage +5") { }

        public FrostforgedIngot(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "frostforged ingot"; } }

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

    /// <summary>
    /// Emberforged Ingot - smelted from Molten Ore
    /// Fire damage +5 when crafted into equipment
    /// </summary>
    [Flipable(0x1BF2, 0x1BEF)]
    public class EmberforgedIngot : BaseVystiaIngot
    {
        [Constructable]
        public EmberforgedIngot() : this(1) { }

        [Constructable]
        public EmberforgedIngot(int amount) : base(amount, 1358, "Emberlands", "Fire Damage +5") { }

        public EmberforgedIngot(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "emberforged ingot"; } }

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

    /// <summary>
    /// Crystalline Ingot - smelted from Crystal Ore
    /// Energy resist +5 when crafted into equipment
    /// </summary>
    [Flipable(0x1BF2, 0x1BEF)]
    public class CrystallineIngot : BaseVystiaIngot
    {
        [Constructable]
        public CrystallineIngot() : this(1) { }

        [Constructable]
        public CrystallineIngot(int amount) : base(amount, 1154, "Crystal Barrens", "Energy Resist +5") { }

        public CrystallineIngot(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "crystalline ingot"; } }

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

    /// <summary>
    /// Clockwork Ingot - smelted from Steamwork Ore
    /// Durability +25% when crafted into equipment
    /// </summary>
    [Flipable(0x1BF2, 0x1BEF)]
    public class ClockworkIngot : BaseVystiaIngot
    {
        [Constructable]
        public ClockworkIngot() : this(1) { }

        [Constructable]
        public ClockworkIngot(int amount) : base(amount, 2401, "Ironclad Empire", "Durability +25%") { }

        public ClockworkIngot(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "clockwork ingot"; } }

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

    /// <summary>
    /// Shadowforged Ingot - smelted from Bog Iron Ore
    /// Self-repair 1 when crafted into equipment
    /// </summary>
    [Flipable(0x1BF2, 0x1BEF)]
    public class ShadowforgedIngot : BaseVystiaIngot
    {
        [Constructable]
        public ShadowforgedIngot() : this(1) { }

        [Constructable]
        public ShadowforgedIngot(int amount) : base(amount, 2212, "Shadowfen", "Self-Repair 1") { }

        public ShadowforgedIngot(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "shadowforged ingot"; } }

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

    /// <summary>
    /// Sunforged Ingot - smelted from Sandstone Ore
    /// Weight -30% when crafted into equipment
    /// </summary>
    [Flipable(0x1BF2, 0x1BEF)]
    public class SunforgedIngot : BaseVystiaIngot
    {
        [Constructable]
        public SunforgedIngot() : this(1) { }

        [Constructable]
        public SunforgedIngot(int amount) : base(amount, 2305, "Desert", "Weight -30%") { }

        public SunforgedIngot(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "sunforged ingot"; } }

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

    /// <summary>
    /// Voidforged Ingot - smelted from Obsidian Ore
    /// Mage Armor when crafted into equipment
    /// </summary>
    [Flipable(0x1BF2, 0x1BEF)]
    public class VoidforgedIngot : BaseVystiaIngot
    {
        [Constructable]
        public VoidforgedIngot() : this(1) { }

        [Constructable]
        public VoidforgedIngot(int amount) : base(amount, 1109, "Obsidian Wastes", "Mage Armor") { }

        public VoidforgedIngot(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "voidforged ingot"; } }

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

    /// <summary>
    /// Natureforged Ingot - smelted from Living Ore
    /// HP Regen +1 when crafted into equipment
    /// </summary>
    [Flipable(0x1BF2, 0x1BEF)]
    public class NatureforgedIngot : BaseVystiaIngot
    {
        [Constructable]
        public NatureforgedIngot() : this(1) { }

        [Constructable]
        public NatureforgedIngot(int amount) : base(amount, 2010, "Verdantpeak", "HP Regen +1") { }

        public NatureforgedIngot(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "natureforged ingot"; } }

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
