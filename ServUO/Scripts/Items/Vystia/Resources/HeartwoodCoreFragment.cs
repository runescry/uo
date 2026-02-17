using System;
using Server;

namespace Server.Items
{
    /// <summary>
    /// Heartwood Core Fragment - Ultra-rare artifact component
    /// Source: Frost Father (1%), Ancient Treant (0.1%)
    /// Uses: Combine 5 fragments to create the Heartwood Core legendary artifact
    /// </summary>
    public class HeartwoodCoreFragment : Item
    {
        [Constructable]
        public HeartwoodCoreFragment() : base(0x136C) // Rock graphic
        {
            Name = "heartwood core fragment";
            Hue = 2010; // Forest green
            Weight = 0.0;
            LootType = LootType.Blessed; // Cannot be stolen/looted
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060658, "Artifact Fragment\tLegendary"); // ~1_val~: ~2_val~
            list.Add(1042971, "5"); // Charges: ~1_val~ (repurposed as "Needed: 5")
            list.Add("Combine 5 fragments to create the Heartwood Core");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack
                return;
            }

            // Count fragments in pack
            int count = 0;
            foreach (Item item in from.Backpack.Items)
            {
                if (item is HeartwoodCoreFragment)
                    count++;
            }

            from.SendMessage("You have {0} of 5 Heartwood Core Fragments needed.", count);

            if (count >= 5)
            {
                from.SendMessage("You have enough fragments! Use a Heartwood Forge to combine them.");
            }
        }

        public HeartwoodCoreFragment(Serial serial) : base(serial)
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
