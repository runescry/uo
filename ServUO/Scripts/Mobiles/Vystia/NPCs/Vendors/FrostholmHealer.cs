using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    /// <summary>
    /// Frostholm Healer - Healer in Frostholm
    /// Personality: Compassionate, caring
    /// </summary>
    public class FrostholmHealer : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;

        [Constructable]
        public FrostholmHealer() : base("Frostholm Healer")
        {
            // Appearance
            Body = 0x191;
            Hue = 1152;

            // Equipment and appearance setup
            SetupAppearance();
        }

        private void SetupAppearance()
        {
            // Healer appearance
            AddItem(new Robe(0x47E)); // Blue robe
            AddItem(new Sandals());
        }

        public override void InitSBInfo()
        {
            // Initialize vendor buy/sell lists
            // m_SBInfos.Add(new SBVystiaHealer());
        }

        public FrostholmHealer(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
