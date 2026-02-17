using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class HerbGatheringQuest : BaseQuest
    {
        public HerbGatheringQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(Ginseng), "ginseng", 20));

            AddReward(new BaseReward(500)); // 500 gold
        }

        public override object Title => "Sacred Herb Gathering";

        public override object Description =>
            "The Tree Council needs rare herbs for a healing ritual. I need you to gather 20 Ginseng from the deepest parts of Verdantpeak Forest. Be careful - ancient guardians protect those groves, and they don't take kindly to outsiders.";

        public override object Refuse =>
            "I see. Perhaps another time then.";

        public override object Uncomplete =>
            "I still need 20 ginseng. Keep searching!";

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
