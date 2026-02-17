using System;
using Server;

namespace Server.Items
{
    /// <summary>
    /// Frozen Artifact - Rare crafting material from Frosthold
    /// Source: Frost Father, Ancient Ice Dragons, Polar Elementals
    /// Uses: Crafting Ice Dragon Scale Armor, Frost Magic items
    /// </summary>
    public class FrozenArtifact : Item
    {
        [Constructable]
        public FrozenArtifact() : this(1)
        {
        }

        [Constructable]
        public FrozenArtifact(int amount) : base(0x1F19) // Crystal ball graphic
        {
            Name = "frozen artifact";
            Hue = 1152; // Ice blue
            Stackable = true;
            Amount = amount;
            Weight = 2.0;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060658, "Crafting Material\tFrosthold"); // ~1_val~: ~2_val~
            list.Add("Used for crafting Ice Dragon Scale Armor");
        }

        public FrozenArtifact(Serial serial) : base(serial)
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
