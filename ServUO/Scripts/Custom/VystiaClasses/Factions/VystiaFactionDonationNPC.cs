using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Prompts;

namespace Server.Custom.VystiaClasses.Factions
{
    /// <summary>
    /// NPC that accepts gold donations for faction reputation
    /// </summary>
    public class VystiaFactionDonationNPC : BaseVendor
    {
        private VystiaFaction m_Faction;
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public VystiaFaction Faction
        {
            get { return m_Faction; }
            set { m_Faction = value; InvalidateProperties(); }
        }

        public override bool IsInvulnerable => true;
        public override bool CanTeach => false;

        public override void InitSBInfo()
        {
            // No vendor inventory - this NPC only accepts donations
        }

        [Constructable]
        public VystiaFactionDonationNPC(VystiaFaction faction) : base("the Faction Donation Officer")
        {
            m_Faction = faction;
            var info = FactionData.GetInfo(faction);
            if (info != null)
            {
                Name = $"{info.Name} Donation Officer";
                Title = $"of the {info.Name}";
            }
        }

        public VystiaFactionDonationNPC(Serial serial) : base(serial) { }

        public override void OnDoubleClick(Mobile from)
        {
            if (from is PlayerMobile pm)
            {
                if (!from.InRange(Location, 3))
                {
                    from.SendMessage(0x22, "You are too far away.");
                    return;
                }

                pm.SendGump(new FactionDonationGump(pm, this));
            }
            else
            {
                base.OnDoubleClick(from);
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Faction\t{0}", m_Faction);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
            writer.Write((int)m_Faction);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Faction = (VystiaFaction)reader.ReadInt();
        }

        private class FactionDonationGump : Gump
        {
            private PlayerMobile m_Player;
            private VystiaFactionDonationNPC m_NPC;

            public FactionDonationGump(PlayerMobile player, VystiaFactionDonationNPC npc) : base(50, 50)
            {
                m_Player = player;
                m_NPC = npc;

                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                AddPage(0);
                AddBackground(0, 0, 350, 300, 9200);
                AddAlphaRegion(10, 10, 330, 280);

                var info = FactionData.GetInfo(m_NPC.Faction);
                AddHtml(20, 20, 310, 20, $"<basefont color=#FFFFFF><center>{(info?.Name ?? m_NPC.Faction.ToString())} Donations</center></basefont>", false, false);

                int currentRep = VystiaReputation.GetFactionReputation(m_Player, m_NPC.Faction);
                ReputationTier currentTier = FactionData.GetTier(currentRep);

                AddHtml(20, 50, 310, 20, $"<basefont color=#FFFFFF>Current Reputation: {currentRep} ({currentTier})</basefont>", false, false);
                AddHtml(20, 75, 310, 40, $"<basefont color=#FFFFFF>Donate gold to increase your reputation with {(info?.Name ?? m_NPC.Faction.ToString())}.</basefont>", false, false);
                AddHtml(20, 115, 310, 20, $"<basefont color=#FFFFFF>Reward: {ReputationRewards.DonationPer1000Gold} reputation per 1,000 gold</basefont>", false, false);

                AddButton(20, 150, 4005, 4007, 1, GumpButtonType.Reply, 0);
                AddHtml(60, 150, 200, 20, "<basefont color=#FFFFFF>Donate 1,000 gold</basefont>", false, false);

                AddButton(20, 180, 4005, 4007, 2, GumpButtonType.Reply, 0);
                AddHtml(60, 180, 200, 20, "<basefont color=#FFFFFF>Donate 5,000 gold</basefont>", false, false);

                AddButton(20, 210, 4005, 4007, 3, GumpButtonType.Reply, 0);
                AddHtml(60, 210, 200, 20, "<basefont color=#FFFFFF>Donate 10,000 gold</basefont>", false, false);

                AddButton(20, 240, 4005, 4007, 4, GumpButtonType.Reply, 0);
                AddHtml(60, 240, 200, 20, "<basefont color=#FFFFFF>Donate Custom Amount</basefont>", false, false);
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (m_Player == null || m_NPC == null)
                    return;

                if (info.ButtonID == 0)
                    return;

                int goldAmount = 0;
                switch (info.ButtonID)
                {
                    case 1: goldAmount = 1000; break;
                    case 2: goldAmount = 5000; break;
                    case 3: goldAmount = 10000; break;
                    case 4:
                        m_Player.SendMessage(0x35, "How much gold would you like to donate? (Minimum 1,000 gold)");
                        m_Player.Prompt = new DonationPrompt(m_Player, m_NPC);
                        return;
                }

                if (goldAmount > 0)
                {
                    ProcessDonation(m_Player, m_NPC, goldAmount);
                }
            }
        }

        private class DonationPrompt : Prompt
        {
            private PlayerMobile m_Player;
            private VystiaFactionDonationNPC m_NPC;

            public DonationPrompt(PlayerMobile player, VystiaFactionDonationNPC npc)
            {
                m_Player = player;
                m_NPC = npc;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (!(from is PlayerMobile pm))
                    return;

                if (!int.TryParse(text, out int goldAmount) || goldAmount < 1000)
                {
                    pm.SendMessage(0x22, "Please enter a valid amount of at least 1,000 gold.");
                    return;
                }

                ProcessDonation(pm, m_NPC, goldAmount);
            }

            public override void OnCancel(Mobile from)
            {
                if (from is PlayerMobile pm)
                    pm.SendMessage("Donation cancelled.");
            }
        }

        private static void ProcessDonation(PlayerMobile pm, VystiaFactionDonationNPC npc, int goldAmount)
        {
            if (pm == null || npc == null)
                return;

            if (goldAmount < 1000)
            {
                pm.SendMessage(0x22, "Minimum donation is 1,000 gold.");
                return;
            }

            // AwardDonationReputation handles gold withdrawal and reputation calculation
            int oldRep = VystiaReputation.GetFactionReputation(pm, npc.Faction);
            ReputationRewards.AwardDonationReputation(pm, npc.Faction, goldAmount);
            int newRep = VystiaReputation.GetFactionReputation(pm, npc.Faction);
            int reputationGained = newRep - oldRep;

            if (reputationGained > 0)
            {
                pm.SendMessage(0x35, "You have donated {0} gold to {1} and gained {2} reputation!", 
                    goldAmount, FactionData.GetInfo(npc.Faction)?.Name ?? npc.Faction.ToString(), reputationGained);
                pm.PlaySound(0x2E6); // Coin sound
            }
        }
    }
}
