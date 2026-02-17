using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class FireElementalThreatQuest : BaseQuest
    {
        public FireElementalThreatQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(FireElemental), "FireElemental", 10));

            AddReward(new BaseReward(2000)); // 2000 gold
        }

        public override object Title => "Fire Elemental Threat";

        public override object Description =>
            "Fire elementals have been spawning near the volcanic vents and threatening our miners. Archmage Pyrus requests that you eliminate 10 Fire Elementals to secure the mining operations. The forges of Emberforge depend on those ores!";

        public override object Refuse =>
            "I understand. These creatures are dangerous. Return if you change your mind.";

        public override object Uncomplete =>
            "You haven't completed your task yet. Return when you've slain all 10 FireElementals.";

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
