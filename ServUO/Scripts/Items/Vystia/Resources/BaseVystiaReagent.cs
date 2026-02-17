using System;
using Server;
using Server.Items;

namespace Server.Items
{
    /// <summary>
    /// Base class for all Vystia magic reagents
    /// </summary>
    public abstract class BaseVystiaReagent : Item
    {
        private string m_FoundLocation;
        private string m_Usage;

        public virtual string FoundLocation { get { return m_FoundLocation; } set { m_FoundLocation = value; } }
        public virtual string Usage { get { return m_Usage; } set { m_Usage = value; } }

        public BaseVystiaReagent(int amount, int itemID, int hue, string location, string usage) : base(itemID)
        {
            Stackable = true;
            Amount = amount;
            Hue = hue;
            m_FoundLocation = location;
            m_Usage = usage;
        }

        public BaseVystiaReagent(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write(m_FoundLocation);
            writer.Write(m_Usage);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_FoundLocation = reader.ReadString();
            m_Usage = reader.ReadString();
        }
    }
}
