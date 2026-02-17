using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    /// <summary>
    /// Ironhaven Guard Captain - Guard in Ironhaven Gates
    /// Personality: Vigilant, disciplined
    /// </summary>
    public class IronhavenGuardCaptain : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;

        [Constructable]
        public IronhavenGuardCaptain() : base("Ironhaven Guard Captain")
        {
            // Appearance
            Body = 0x190;
            Hue = 2213;

            // Equipment and appearance setup
            SetupAppearance();
        }

        private void SetupAppearance()
        {
            // Guard appearance
            AddItem(new PlateChest());
            AddItem(new PlateLegs());
            AddItem(new PlateArms());
            AddItem(new PlateGloves());
            AddItem(new Longsword());
        }

        public override void InitSBInfo()
        {
            // Initialize vendor buy/sell lists
            // m_SBInfos.Add(new SBVystiaGuard());
        }

        public IronhavenGuardCaptain(Serial serial) : base(serial)
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
