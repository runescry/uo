using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    /// <summary>
    /// Ironhaven Banker - Banker in Ironhaven
    /// Personality: Professional, efficient
    /// </summary>
    public class IronhavenBanker : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;

        [Constructable]
        public IronhavenBanker() : base("Ironhaven Banker")
        {
            // Appearance
            Body = 0x190;
            Hue = 2213;

            // Equipment and appearance setup
            SetupAppearance();
        }

        private void SetupAppearance()
        {
            // Banker appearance
            AddItem(new FancyShirt(Hue));
            AddItem(new LongPants(Hue));
            AddItem(new Boots());
        }

        public override void InitSBInfo()
        {
            // Initialize vendor buy/sell lists
            // m_SBInfos.Add(new SBVystiaBanker());
        }

        public IronhavenBanker(Serial serial) : base(serial)
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
