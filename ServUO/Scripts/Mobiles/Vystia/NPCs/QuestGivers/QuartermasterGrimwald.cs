using System;
using Server.Items;
using Server.Engines.Quests;

namespace Server.Mobiles
{
    /// <summary>
    /// Quartermaster Grimwald - Quest Giver in Ironhaven Barracks
    /// Personality: Gruff military veteran
    /// Quest: SupplyLine
    /// </summary>
    public class QuartermasterGrimwald : MondainQuester
    {
        public override Type[] Quests => new Type[]
        {
            typeof(SupplyLineQuest)
        };

        [Constructable]
        public QuartermasterGrimwald() : base("Quartermaster Grimwald", "Quest Giver")
        {
            // Appearance
            Body = 0x190;
            Hue = 2213;

            SetupAppearance();
        }

        private void SetupAppearance()
        {
            // Quest giver appearance
            AddItem(new Robe(2213));
            AddItem(new Boots());
            AddItem(new QuarterStaff());
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            // LLM Integration Context
            // Lore Context: Veteran quartermaster who oversees supply lines for the Ironclad military.
        }

        public QuartermasterGrimwald(Serial serial) : base(serial)
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
