using System;

namespace Server.Items
{
    /// <summary>
    /// Base class for all Vystia regional hides.
    /// These drop from regional creatures and can be processed into leather.
    /// </summary>
    public abstract class BaseVystiaHide : Item
    {
        private string m_Region;
        private string m_SourceCreature;
        private string m_Property;

        [CommandProperty(AccessLevel.GameMaster)]
        public string Region { get { return m_Region; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public string SourceCreature { get { return m_SourceCreature; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Property { get { return m_Property; } }

        public BaseVystiaHide(int hue, string region, string source, string property)
            : this(1, hue, region, source, property)
        {
        }

        public BaseVystiaHide(int amount, int hue, string region, string source, string property)
            : base(0x1079) // Hide graphic
        {
            Stackable = true;
            Amount = amount;
            Hue = hue;
            m_Region = region;
            m_SourceCreature = source;
            m_Property = property;
        }

        public BaseVystiaHide(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.Write(m_Region);
            writer.Write(m_SourceCreature);
            writer.Write(m_Property);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Region = reader.ReadString();
            m_SourceCreature = reader.ReadString();
            m_Property = reader.ReadString();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: {0}", m_Region);
            list.Add("Source: {0}", m_SourceCreature);
            list.Add(m_Property);
        }
    }

    /// <summary>
    /// Base class for all Vystia regional leathers.
    /// These are processed from regional hides.
    /// </summary>
    public abstract class BaseVystiaLeather : Item
    {
        private string m_Region;
        private string m_Property;

        [CommandProperty(AccessLevel.GameMaster)]
        public string Region { get { return m_Region; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Property { get { return m_Property; } }

        public BaseVystiaLeather(int hue, string region, string property)
            : this(1, hue, region, property)
        {
        }

        public BaseVystiaLeather(int amount, int hue, string region, string property)
            : base(0x1081) // Leather graphic
        {
            Stackable = true;
            Amount = amount;
            Hue = hue;
            m_Region = region;
            m_Property = property;
        }

        public BaseVystiaLeather(Serial serial)
            : base(serial)
        {
        }

        public override double DefaultWeight { get { return 1.0; } }

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

    #region Frosthold - Frost Leather (Winter Wolf)

    /// <summary>
    /// Frost Hide from Frosthold - drops from Winter Wolves and similar creatures
    /// </summary>
    public class FrostHide : BaseVystiaHide
    {
        [Constructable]
        public FrostHide() : this(1) { }

        [Constructable]
        public FrostHide(int amount) : base(amount, 1152, "Frosthold", "Winter Wolf", "Cold Resist +10") { }

        public FrostHide(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "frost hide"; } }

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
    /// Frost Leather - processed from Frost Hides
    /// Cold Resist +10 when crafted into equipment
    /// </summary>
    public class FrostLeather : BaseVystiaLeather
    {
        [Constructable]
        public FrostLeather() : this(1) { }

        [Constructable]
        public FrostLeather(int amount) : base(amount, 1152, "Frosthold", "Cold Resist +10") { }

        public FrostLeather(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "frost leather"; } }

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

    #endregion

    #region Emberlands - Fire Leather (Lava Hound)

    /// <summary>
    /// Fire Hide from Emberlands - drops from Lava Hounds and similar creatures
    /// </summary>
    public class FireHide : BaseVystiaHide
    {
        [Constructable]
        public FireHide() : this(1) { }

        [Constructable]
        public FireHide(int amount) : base(amount, 1358, "Emberlands", "Lava Hound", "Fire Resist +10") { }

        public FireHide(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "fire hide"; } }

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
    /// Fire Leather - processed from Fire Hides
    /// Fire Resist +10 when crafted into equipment
    /// </summary>
    public class FireLeather : BaseVystiaLeather
    {
        [Constructable]
        public FireLeather() : this(1) { }

        [Constructable]
        public FireLeather(int amount) : base(amount, 1358, "Emberlands", "Fire Resist +10") { }

        public FireLeather(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "fire leather"; } }

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

    #endregion

    #region Shadowfen - Shadow Leather (Shadow Wolf)

    /// <summary>
    /// Shadow Hide from Shadowfen - drops from Shadow Wolves and similar creatures
    /// </summary>
    public class ShadowHide : BaseVystiaHide
    {
        [Constructable]
        public ShadowHide() : this(1) { }

        [Constructable]
        public ShadowHide(int amount) : base(amount, 1109, "Shadowfen", "Shadow Wolf", "Stealth +15") { }

        public ShadowHide(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "shadow hide"; } }

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
    /// Shadow Leather - processed from Shadow Hides
    /// Stealth +15 when crafted into equipment
    /// </summary>
    public class ShadowLeather : BaseVystiaLeather
    {
        [Constructable]
        public ShadowLeather() : this(1) { }

        [Constructable]
        public ShadowLeather(int amount) : base(amount, 1109, "Shadowfen", "Stealth +15") { }

        public ShadowLeather(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "shadow leather"; } }

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

    #endregion
}
