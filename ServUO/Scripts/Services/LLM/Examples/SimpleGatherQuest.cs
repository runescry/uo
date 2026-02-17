using System;
using Server;
using Server.Engines.Quests;
using Server.Items;
using Server.Mobiles;

namespace Server.Services.LLM.Examples
{
    /// <summary>
    /// Simple gather quest - collect 5 logs and return them
    /// This is an example of how to create an LLM-driven quest
    /// </summary>
    public class SimpleGatherQuest : QuestSystem
    {
        private static Type[] m_TypeReferenceTable = new Type[]
        {
            typeof(GatherLogsObjective),
            typeof(ReturnLogsObjective)
        };

        public SimpleGatherQuest(PlayerMobile from) : base(from)
        {
        }

        // Serialization constructor
        public SimpleGatherQuest()
        {
        }

        public override Type[] TypeReferenceTable => m_TypeReferenceTable;

        public override object Name => "The Woodcutter's Request";

        public override object OfferMessage =>
            "Greetings, traveler! I find myself in need of lumber for a project.\n\n" +
            "Would you be willing to gather some logs for me? I need about 5 fresh logs from the forest.\n\n" +
            "Return them to me and I shall reward you for your efforts!";

        public override int Picture => 0x1BDD; // Log graphic

        public override TimeSpan RestartDelay => TimeSpan.FromMinutes(30);

        public override bool IsTutorial => false;

        public override void Accept()
        {
            base.Accept();
            AddObjective(new GatherLogsObjective());
        }
    }

    /// <summary>
    /// Objective: Gather 5 logs
    /// </summary>
    public class GatherLogsObjective : QuestObjective
    {
        public GatherLogsObjective()
        {
        }

        public override object Message =>
            "Gather 5 logs from trees in the forest. You can find logs by chopping trees with an axe.";

        public override int MaxProgress => 5;

        public override void CheckProgress()
        {
            PlayerMobile player = System?.From as PlayerMobile;
            if (player == null)
                return;

            // Count logs in player's backpack
            int logCount = 0;
            Container backpack = player.Backpack;

            if (backpack != null)
            {
                logCount = backpack.GetAmount(typeof(Log));
            }

            // Update progress
            CurProgress = Math.Min(logCount, MaxProgress);

            if (CurProgress >= MaxProgress && !Completed)
            {
                Complete();
            }
        }

        public override void OnComplete()
        {
            System.AddObjective(new ReturnLogsObjective());
        }
    }

    /// <summary>
    /// Objective: Return logs to the quest giver
    /// </summary>
    public class ReturnLogsObjective : QuestObjective
    {
        public ReturnLogsObjective()
        {
        }

        public override object Message =>
            "You have gathered enough logs! Return them to the quest giver.";

        public override void OnComplete()
        {
            PlayerMobile player = System?.From as PlayerMobile;
            if (player == null)
                return;

            // Remove 5 logs from player
            Container backpack = player.Backpack;
            if (backpack != null)
            {
                backpack.ConsumeTotal(typeof(Log), 5);
            }

            // Give reward
            GiveReward();

            // Complete the quest
            System.Complete();
        }

        private void GiveReward()
        {
            PlayerMobile player = System?.From as PlayerMobile;
            if (player == null)
                return;

            // Reward: 500 gold and a basic axe
            player.AddToBackpack(new Gold(500));
            player.AddToBackpack(new Axe());

            player.SendMessage(0x23, "You have been rewarded with 500 gold and an axe!");
        }
    }
}
