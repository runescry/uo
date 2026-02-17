using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class FrostWolfHuntQuest : BaseQuest
    {
        public FrostWolfHuntQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(DireWolf), "DireWolf", 8));

            AddReward(new BaseReward(1500)); // 1500 gold
        }

        public override object Title => "Frost Wolf Hunt";

        public override object Description =>
            "The frost wolves have been attacking our supply caravans traveling between Frostholm and the southern settlements. Chieftain Bjorn has authorized a bounty on these beasts. Slay 8 Frost Wolves and I'll pay you well for your service to Frosthold.";

        public override object Refuse =>
            "I understand. These creatures are dangerous. Return if you change your mind.";

        public override object Uncomplete =>
            "You haven't completed your task yet. Return when you've slain all 8 DireWolfs.";

        public override object Complete =>
            "Well done! You've proven your strength. Here is your reward.";

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
