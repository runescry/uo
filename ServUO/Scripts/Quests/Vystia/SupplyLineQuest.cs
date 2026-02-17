using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class SupplyLineQuest : BaseQuest
    {
        public SupplyLineQuest()
            : base()
        {
            AddObjective(new DeliverObjective(typeof(IronIngot), "iron ingot", 10, typeof(IronhavenGuardCaptain), "Captain Steelhart"));

            AddReward(new BaseReward(1000)); // 1000 gold
        }

        public override object Title => "Supply Line";

        public override object Description =>
            "The Ironclad military needs supplies urgently. I need you to deliver 10 Iron Ingots to Captain Steelhart at the northern outpost. Our supply lines have been disrupted by raiders, and the garrison is running low on equipment repairs.";

        public override object Refuse =>
            "Very well. The supplies can wait, I suppose.";

        public override object Uncomplete =>
            "I still need 10 iron ingot. Please deliver them to Captain Steelhart.";

        public override object Complete =>
            "Excellent! The supplies have arrived safely. Thank you for your help.";

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
