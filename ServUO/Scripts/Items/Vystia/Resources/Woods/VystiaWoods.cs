using System;

namespace Server.Items
{
    /// <summary>
    /// Base class for all Vystia regional wood logs.
    /// These are standalone items harvested from regional trees.
    /// </summary>
    public abstract class BaseVystiaLog : Item
    {
        private string m_Region;
        private double m_LumberjackingRequired;
        private string m_Property;

        [CommandProperty(AccessLevel.GameMaster)]
        public string Region { get { return m_Region; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public double LumberjackingRequired { get { return m_LumberjackingRequired; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Property { get { return m_Property; } }

        public BaseVystiaLog(int hue, string region, double lumberjacking, string property)
            : this(1, hue, region, lumberjacking, property)
        {
        }

        public BaseVystiaLog(int amount, int hue, string region, double lumberjacking, string property)
            : base(0x1BDD) // Log graphic
        {
            Stackable = true;
            Amount = amount;
            Hue = hue;
            m_Region = region;
            m_LumberjackingRequired = lumberjacking;
            m_Property = property;
        }

        public BaseVystiaLog(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.Write(m_Region);
            writer.Write(m_LumberjackingRequired);
            writer.Write(m_Property);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Region = reader.ReadString();
            m_LumberjackingRequired = reader.ReadDouble();
            m_Property = reader.ReadString();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: {0}", m_Region);
            list.Add("Lumberjacking: {0:F1}", m_LumberjackingRequired);
            list.Add(m_Property);
        }
    }

    /// <summary>
    /// Base class for all Vystia regional boards.
    /// These are processed from regional logs.
    /// </summary>
    public abstract class BaseVystiaBoard : Item
    {
        private string m_Region;
        private string m_Property;

        [CommandProperty(AccessLevel.GameMaster)]
        public string Region { get { return m_Region; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Property { get { return m_Property; } }

        public BaseVystiaBoard(int hue, string region, string property)
            : this(1, hue, region, property)
        {
        }

        public BaseVystiaBoard(int amount, int hue, string region, string property)
            : base(0x1BD7) // Board graphic
        {
            Stackable = true;
            Amount = amount;
            Hue = hue;
            m_Region = region;
            m_Property = property;
        }

        public BaseVystiaBoard(Serial serial)
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

    #region Frosthold - Frostwillow Wood

    /// <summary>
    /// Frostwillow Log from Frosthold
    /// Cold Resist +5 when crafted into equipment
    /// </summary>
    public class FrostwillowLog : BaseVystiaLog
    {
        [Constructable]
        public FrostwillowLog() : this(1) { }

        [Constructable]
        public FrostwillowLog(int amount) : base(amount, 1152, "Frosthold", 85.0, "Cold Resist +5") { }

        public FrostwillowLog(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "frostwillow log"; } }

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
    /// Frostwillow Board - processed from Frostwillow Logs
    /// </summary>
    [Flipable(0x1BD7, 0x1BDA)]
    public class FrostwillowBoard : BaseVystiaBoard
    {
        [Constructable]
        public FrostwillowBoard() : this(1) { }

        [Constructable]
        public FrostwillowBoard(int amount) : base(amount, 1152, "Frosthold", "Cold Resist +5") { }

        public FrostwillowBoard(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "frostwillow board"; } }

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

    #region Emberlands - Flamewood

    /// <summary>
    /// Flamewood Log from Emberlands
    /// Fire Resist +5 when crafted into equipment
    /// </summary>
    public class FlamewoodLog : BaseVystiaLog
    {
        [Constructable]
        public FlamewoodLog() : this(1) { }

        [Constructable]
        public FlamewoodLog(int amount) : base(amount, 1358, "Emberlands", 90.0, "Fire Resist +5") { }

        public FlamewoodLog(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "flamewood log"; } }

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
    /// Flamewood Board - processed from Flamewood Logs
    /// </summary>
    [Flipable(0x1BD7, 0x1BDA)]
    public class FlamewoodBoard : BaseVystiaBoard
    {
        [Constructable]
        public FlamewoodBoard() : this(1) { }

        [Constructable]
        public FlamewoodBoard(int amount) : base(amount, 1358, "Emberlands", "Fire Resist +5") { }

        public FlamewoodBoard(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "flamewood board"; } }

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

    #region Crystal Barrens - Crystalwood

    /// <summary>
    /// Crystalwood Log from Crystal Barrens
    /// Spell Channeling when crafted into equipment
    /// </summary>
    public class CrystalwoodLog : BaseVystiaLog
    {
        [Constructable]
        public CrystalwoodLog() : this(1) { }

        [Constructable]
        public CrystalwoodLog(int amount) : base(amount, 1154, "Crystal Barrens", 95.0, "Spell Channeling") { }

        public CrystalwoodLog(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "crystalwood log"; } }

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
    /// Crystalwood Board - processed from Crystalwood Logs
    /// </summary>
    [Flipable(0x1BD7, 0x1BDA)]
    public class CrystalwoodBoard : BaseVystiaBoard
    {
        [Constructable]
        public CrystalwoodBoard() : this(1) { }

        [Constructable]
        public CrystalwoodBoard(int amount) : base(amount, 1154, "Crystal Barrens", "Spell Channeling") { }

        public CrystalwoodBoard(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "crystalwood board"; } }

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

    #region Shadowfen - Shadowwood

    /// <summary>
    /// Shadowwood Log from Shadowfen
    /// Stealth +10 when crafted into equipment
    /// </summary>
    public class ShadowwoodLog : BaseVystiaLog
    {
        [Constructable]
        public ShadowwoodLog() : this(1) { }

        [Constructable]
        public ShadowwoodLog(int amount) : base(amount, 2212, "Shadowfen", 80.0, "Stealth +10") { }

        public ShadowwoodLog(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "shadowwood log"; } }

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
    /// Shadowwood Board - processed from Shadowwood Logs
    /// </summary>
    [Flipable(0x1BD7, 0x1BDA)]
    public class ShadowwoodBoard : BaseVystiaBoard
    {
        [Constructable]
        public ShadowwoodBoard() : this(1) { }

        [Constructable]
        public ShadowwoodBoard(int amount) : base(amount, 2212, "Shadowfen", "Stealth +10") { }

        public ShadowwoodBoard(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "shadowwood board"; } }

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

    #region Verdantpeak - Livingwood

    /// <summary>
    /// Livingwood Log from Verdantpeak
    /// Self-Repair when crafted into equipment
    /// </summary>
    public class LivingwoodLog : BaseVystiaLog
    {
        [Constructable]
        public LivingwoodLog() : this(1) { }

        [Constructable]
        public LivingwoodLog(int amount) : base(amount, 2010, "Verdantpeak", 70.0, "Self-Repair") { }

        public LivingwoodLog(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "livingwood log"; } }

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
    /// Livingwood Board - processed from Livingwood Logs
    /// </summary>
    [Flipable(0x1BD7, 0x1BDA)]
    public class LivingwoodBoard : BaseVystiaBoard
    {
        [Constructable]
        public LivingwoodBoard() : this(1) { }

        [Constructable]
        public LivingwoodBoard(int amount) : base(amount, 2010, "Verdantpeak", "Self-Repair") { }

        public LivingwoodBoard(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "livingwood board"; } }

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

    #region Ironclad - Ironwood

    /// <summary>
    /// Ironwood Log from Ironclad Empire
    /// Physical Resist +10 when crafted into equipment
    /// </summary>
    public class IronwoodLog : BaseVystiaLog
    {
        [Constructable]
        public IronwoodLog() : this(1) { }

        [Constructable]
        public IronwoodLog(int amount) : base(amount, 2401, "Ironclad Empire", 85.0, "Physical Resist +10") { }

        public IronwoodLog(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "ironwood log"; } }

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
    /// Ironwood Board - processed from Ironwood Logs
    /// </summary>
    [Flipable(0x1BD7, 0x1BDA)]
    public class IronwoodBoard : BaseVystiaBoard
    {
        [Constructable]
        public IronwoodBoard() : this(1) { }

        [Constructable]
        public IronwoodBoard(int amount) : base(amount, 2401, "Ironclad Empire", "Physical Resist +10") { }

        public IronwoodBoard(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "ironwood board"; } }

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

    #region Desert - Petrified Wood

    /// <summary>
    /// Petrified Wood Log from Desert regions
    /// Durability +50% when crafted into equipment
    /// </summary>
    public class PetrifiedWoodLog : BaseVystiaLog
    {
        [Constructable]
        public PetrifiedWoodLog() : this(1) { }

        [Constructable]
        public PetrifiedWoodLog(int amount) : base(amount, 2305, "Desert", 75.0, "Durability +50%") { }

        public PetrifiedWoodLog(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "petrified wood log"; } }

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
    /// Petrified Wood Board - processed from Petrified Wood Logs
    /// </summary>
    [Flipable(0x1BD7, 0x1BDA)]
    public class PetrifiedWoodBoard : BaseVystiaBoard
    {
        [Constructable]
        public PetrifiedWoodBoard() : this(1) { }

        [Constructable]
        public PetrifiedWoodBoard(int amount) : base(amount, 2305, "Desert", "Durability +50%") { }

        public PetrifiedWoodBoard(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "petrified wood board"; } }

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
