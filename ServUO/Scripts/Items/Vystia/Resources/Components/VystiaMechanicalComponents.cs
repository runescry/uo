using System;

namespace Server.Items
{
    /// <summary>
    /// Base class for all Vystia mechanical components.
    /// These drop from clockwork creatures in Ironclad and are used in clockwork crafting.
    /// </summary>
    public abstract class BaseVystiaComponent : Item
    {
        private string m_Source;
        private string m_Uses;

        [CommandProperty(AccessLevel.GameMaster)]
        public string Source { get { return m_Source; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Uses { get { return m_Uses; } }

        public BaseVystiaComponent(int itemID, int hue, string source, string uses)
            : this(1, itemID, hue, source, uses)
        {
        }

        public BaseVystiaComponent(int amount, int itemID, int hue, string source, string uses)
            : base(itemID)
        {
            Stackable = true;
            Amount = amount;
            Hue = hue;
            m_Source = source;
            m_Uses = uses;
        }

        public BaseVystiaComponent(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.Write(m_Source);
            writer.Write(m_Uses);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Source = reader.ReadString();
            m_Uses = reader.ReadString();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Source: {0}", m_Source);
            list.Add(m_Uses);
        }
    }

    #region Clockwork Gear - Ironclad

    /// <summary>
    /// Clockwork Gear - drops from clockwork creatures in Ironclad
    /// Used in clockwork crafting
    /// </summary>
    public class ClockworkGear : BaseVystiaComponent
    {
        [Constructable]
        public ClockworkGear() : this(1) { }

        [Constructable]
        public ClockworkGear(int amount) : base(amount, 0x1053, 2401, "Clockwork Creatures", "Clockwork crafting") { }

        public ClockworkGear(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "clockwork gear"; } }

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

    #region Clockwork Spring - Ironclad

    /// <summary>
    /// Clockwork Spring - drops from clockwork creatures in Ironclad
    /// Used in clockwork crafting
    /// </summary>
    public class ClockworkSpring : BaseVystiaComponent
    {
        [Constructable]
        public ClockworkSpring() : this(1) { }

        [Constructable]
        public ClockworkSpring(int amount) : base(amount, 0x105D, 2401, "Clockwork Creatures", "Clockwork crafting") { }

        public ClockworkSpring(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "clockwork spring"; } }

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

    #region Steam Core - Ironclad

    /// <summary>
    /// Steam Core - rare drop from steam vents and powerful clockwork creatures
    /// Used as power source for advanced clockwork crafting
    /// </summary>
    public class SteamCore : BaseVystiaComponent
    {
        [Constructable]
        public SteamCore() : this(1) { }

        [Constructable]
        public SteamCore(int amount) : base(amount, 0x0F8E, 2401, "Steam Vents", "Power source for clockwork") { }

        public SteamCore(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "steam core"; } }

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
