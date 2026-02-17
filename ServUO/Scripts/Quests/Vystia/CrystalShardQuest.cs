using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class CrystalShardQuest : BaseQuest
    {
        public CrystalShardQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(EnergyVortex), "crystal shard", 15));

            AddReward(new BaseReward(1800)); // 1800 gold
        }

        public override object Title => "Crystal Shard Collection";

        public override object Description =>
            "The energy crystals of the Crystal Barrens hold immense magical power. I'm conducting experiments and need 15 pristine Crystal Shards. You'll likely find them on the Crystal Elementals that inhabit the region. Handle them carefully - they're quite volatile!";

        public override object Refuse =>
            "I see. Perhaps another time then.";

        public override object Uncomplete =>
            "I still need 15 crystal shard. Keep searching!";

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
