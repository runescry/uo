using System;

namespace Server.Items
{
    /// <summary>
    /// Base class for all Vystia special regional resources.
    /// These are rare materials with unique properties.
    /// </summary>
    public abstract class BaseVystiaSpecialResource : Item
    {
        private string m_Region;
        private string m_Property;

        [CommandProperty(AccessLevel.GameMaster)]
        public string Region { get { return m_Region; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Property { get { return m_Property; } }

        public BaseVystiaSpecialResource(int itemID, int hue, string region, string property)
            : this(1, itemID, hue, region, property)
        {
        }

        public BaseVystiaSpecialResource(int amount, int itemID, int hue, string region, string property)
            : base(itemID)
        {
            Stackable = true;
            Amount = amount;
            Hue = hue;
            m_Region = region;
            m_Property = property;
        }

        public BaseVystiaSpecialResource(Serial serial)
            : base(serial)
        {
        }

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

    #region Eternal Ice - Frosthold

    /// <summary>
    /// Eternal Ice - magical ice from Frosthold that never melts
    /// </summary>
    public class EternalIce : BaseVystiaSpecialResource
    {
        [Constructable]
        public EternalIce() : this(1) { }

        [Constructable]
        public EternalIce(int amount) : base(amount, 0x1F1C, 1152, "Frosthold", "Never melts") { }

        public EternalIce(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "eternal ice"; } }

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

    #region Ice Crystal - Frosthold

    /// <summary>
    /// Ice Crystal - crystallized magical ice used as a magic focus
    /// </summary>
    public class IceCrystal : BaseVystiaSpecialResource
    {
        [Constructable]
        public IceCrystal() : this(1) { }

        [Constructable]
        public IceCrystal(int amount) : base(amount, 0x1F19, 1152, "Frosthold", "Magic focus") { }

        public IceCrystal(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "ice crystal"; } }

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

    #region Everburning Coal - Emberlands

    /// <summary>
    /// Everburning Coal - magical coal from Emberlands that burns perpetually
    /// </summary>
    public class EverburningCoal : BaseVystiaSpecialResource
    {
        [Constructable]
        public EverburningCoal() : this(1) { }

        [Constructable]
        public EverburningCoal(int amount) : base(amount, 0x19B9, 1358, "Emberlands", "Perpetual fuel") { }

        public EverburningCoal(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "everburning coal"; } }

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

    #region Lava Pearl - Emberlands

    /// <summary>
    /// Lava Pearl - rare pearl formed in lava, used in fire jewelry
    /// </summary>
    public class LavaPearl : BaseVystiaSpecialResource
    {
        [Constructable]
        public LavaPearl() : this(1) { }

        [Constructable]
        public LavaPearl(int amount) : base(amount, 0x3196, 1358, "Emberlands", "Fire jewelry crafting") { }

        public LavaPearl(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "lava pearl"; } }

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

    #region Prismatic Shard - Crystal Barrens

    /// <summary>
    /// Prismatic Shard - multi-colored crystal shard for all-element magic
    /// </summary>
    public class PrismaticShard : BaseVystiaSpecialResource
    {
        [Constructable]
        public PrismaticShard() : this(1) { }

        [Constructable]
        public PrismaticShard(int amount) : base(amount, 0x1F19, 0, "Crystal Barrens", "All-element magic") { }

        public PrismaticShard(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "prismatic shard"; } }

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

    #region Ley Crystal - Crystal Barrens

    /// <summary>
    /// Ley Crystal - crystal attuned to ley lines, used in teleportation
    /// </summary>
    public class LeyCrystal : BaseVystiaSpecialResource
    {
        [Constructable]
        public LeyCrystal() : this(1) { }

        [Constructable]
        public LeyCrystal(int amount) : base(amount, 0x1F19, 1156, "Crystal Barrens", "Teleportation") { }

        public LeyCrystal(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "ley crystal"; } }

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

    #region Shadow Silk - Shadowfen

    /// <summary>
    /// Shadow Silk - magical silk from shadow spiders, used in stealth armor
    /// </summary>
    public class ShadowSilk : BaseVystiaSpecialResource
    {
        [Constructable]
        public ShadowSilk() : this(1) { }

        [Constructable]
        public ShadowSilk(int amount) : base(amount, 0x1766, 1109, "Shadowfen", "Stealth armor crafting") { }

        public ShadowSilk(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "shadow silk"; } }

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

    #region Treant Heart - Verdantpeak

    /// <summary>
    /// Treant Heart - rare drop from Ancient Treants, contains nature power
    /// </summary>
    public class TreantHeart : BaseVystiaSpecialResource
    {
        [Constructable]
        public TreantHeart() : this(1) { }

        [Constructable]
        public TreantHeart(int amount) : base(amount, 0x1CED, 2010, "Verdantpeak", "Nature power") { }

        public TreantHeart(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "treant heart"; } }

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
