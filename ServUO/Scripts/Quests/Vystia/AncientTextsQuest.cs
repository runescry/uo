using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class AncientTextsQuest : BaseQuest
    {
        public AncientTextsQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(BlankScroll), "ancient scroll", 5));

            AddReward(new BaseReward(750)); // 750 gold
        }

        public override object Title => "Ancient Texts";

        public override object Description =>
            "I'm researching the ancient history of Verdantpeak and I need scrolls from the old library ruins. The library was abandoned centuries ago and is now overrun with creatures. Please retrieve 5 Ancient Scrolls for my research. They should be found on the Guardians protecting the ruins.";

        public override object Refuse =>
            "I see. Perhaps another time then.";

        public override object Uncomplete =>
            "I still need 5 ancient scroll. Keep searching!";

        public override object Complete =>
            "Perfect! You've gathered everything I needed. Here's your reward.";

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
