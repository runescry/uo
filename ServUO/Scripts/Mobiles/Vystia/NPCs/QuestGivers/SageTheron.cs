using System;
using Server.Items;
using Server.Engines.Quests;

namespace Server.Mobiles
{
    /// <summary>
    /// Sage Theron - Quest Giver in Verdantheart Library
    /// Personality: Scholarly, curious
    /// Quest: AncientTexts
    /// </summary>
    public class SageTheron : MondainQuester
    {
        public override Type[] Quests => new Type[]
        {
            typeof(AncientTextsQuest)
        };

        [Constructable]
        public SageTheron() : base("Sage Theron", "Quest Giver")
        {
            // Appearance
            Body = 0x190;
            Hue = 2010;

            SetupAppearance();
        }

        private void SetupAppearance()
        {
            // Quest giver appearance
            AddItem(new Robe(2010));
            AddItem(new Boots());
            AddItem(new QuarterStaff());
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            // LLM Integration Context
            // Lore Context: Elven sage who studies ancient texts and seeks lost knowledge of the old world.
        }

        public SageTheron(Serial serial) : base(serial)
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
