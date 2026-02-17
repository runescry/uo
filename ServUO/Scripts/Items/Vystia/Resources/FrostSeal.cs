using System;
using Server;

namespace Server.Items
{
    /// <summary>
    /// Frost Seal - Key item for Frozen Halls dungeon
    /// Source: Drops from Frost Wraiths (10-20% drop rate)
    /// Uses: Required to access Frost Father boss chamber
    /// </summary>
    public class FrostSeal : Item
    {
        [Constructable]
        public FrostSeal() : base(0x2258) // Crystal/sigil graphic
        {
            Name = "Frost Seal";
            Hue = 1152; // Ice blue
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060658, "Dungeon Key\tFrozen Halls"); // ~1_val~: ~2_val~
            list.Add("Grants access to the Frost Father's chamber");
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage(0x480, "This seal pulses with frozen energy. It will grant you access to the Frost Father's chamber in the Frozen Halls.");
        }

        public FrostSeal(Serial serial) : base(serial)
        {
        }

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
