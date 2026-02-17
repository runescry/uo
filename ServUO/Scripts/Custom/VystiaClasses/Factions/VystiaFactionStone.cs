using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Commands;
using Server.Gumps;
using Server.Network;
using Server.Custom.VystiaClasses.Factions;

namespace Server.Custom.VystiaClasses.Factions
{
    /// <summary>
    /// Test item: Faction Stone for each faction
    /// Allows testing faction reputation and vendor interactions
    /// </summary>
    public class VystiaFactionStone : Item
    {
        private VystiaFaction m_Faction;

        [CommandProperty(AccessLevel.GameMaster)]
        public VystiaFaction Faction
        {
            get { return m_Faction; }
            set
            {
                m_Faction = value;
                InvalidateProperties();
            }
        }

        [Constructable]
        public VystiaFactionStone(VystiaFaction faction)
            : base(0x1F13) // Gem graphic
        {
            m_Faction = faction;
            Name = GetStoneName(faction);
            Hue = FactionData.GetInfo(faction)?.Hue ?? 0;
            Weight = 1.0;
            Movable = true;
        }

        public VystiaFactionStone(Serial serial)
            : base(serial)
        {
        }

        private string GetStoneName(VystiaFaction faction)
        {
            switch (faction)
            {
                case VystiaFaction.Frostguard:
                    return "Frostguard Faction Stone";
                case VystiaFaction.FlameLegion:
                    return "Flame Legion Faction Stone";
                case VystiaFaction.Greenward:
                    return "Greenward Faction Stone";
                case VystiaFaction.ArcaneConclave:
                    return "Arcane Conclave Faction Stone";
                case VystiaFaction.Technoguild:
                    return "Technoguild Faction Stone";
                case VystiaFaction.Sandwalkers:
                    return "Sandwalkers Faction Stone";
                case VystiaFaction.Voidborn:
                    return "Voidborn Faction Stone";
                default:
                    return "Faction Stone";
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from == null || !(from is PlayerMobile))
                return;

            PlayerMobile pm = from as PlayerMobile;

            // Show faction info and options
            ShowFactionMenu(pm);
        }

        private void ShowFactionMenu(PlayerMobile pm)
        {
            var factionInfo = FactionData.GetInfo(m_Faction);
            if (factionInfo == null)
                return;

            var reputation = VystiaReputation.GetFactionReputation(pm, m_Faction);
            var tier = FactionData.GetTier(reputation);

            pm.SendGump(new FactionStoneGump(pm, this, factionInfo, reputation, tier));
        }

        /// <summary>
        /// Add reputation to the player for this faction
        /// </summary>
        public void AddReputation(PlayerMobile pm, int amount)
        {
            if (pm == null)
                return;

            int oldRep = VystiaReputation.GetFactionReputation(pm, m_Faction);
            VystiaReputation.AddReputation(pm, m_Faction, amount, "faction stone test");
            int newRep = VystiaReputation.GetFactionReputation(pm, m_Faction);

            pm.SendMessage(0x35, "Your reputation with {0} has changed from {1} to {2} ({3:+0;-0}).",
                FactionData.GetInfo(m_Faction)?.Name ?? "Unknown",
                oldRep,
                newRep,
                newRep - oldRep);
        }

        /// <summary>
        /// Set reputation to a specific tier
        /// </summary>
        public void SetReputationTier(PlayerMobile pm, ReputationTier tier)
        {
            if (pm == null)
                return;

            int targetRep = GetReputationForTier(tier);
            int currentRep = VystiaReputation.GetFactionReputation(pm, m_Faction);
            int difference = targetRep - currentRep;

            if (difference != 0)
            {
                VystiaReputation.AddReputation(pm, m_Faction, difference, "faction stone tier set");
                pm.SendMessage(0x35, "Your reputation with {0} has been set to {1} ({2}).",
                    FactionData.GetInfo(m_Faction)?.Name ?? "Unknown",
                    targetRep,
                    tier);
            }
            else
            {
                pm.SendMessage(0x35, "Your reputation with {0} is already at {1}.",
                    FactionData.GetInfo(m_Faction)?.Name ?? "Unknown",
                    tier);
            }
        }

        private int GetReputationForTier(ReputationTier tier)
        {
            switch (tier)
            {
                case ReputationTier.Hostile:
                    return -2000; // Middle of hostile range
                case ReputationTier.Unfriendly:
                    return -500; // Middle of unfriendly range
                case ReputationTier.Neutral:
                    return 1500; // Middle of neutral range
                case ReputationTier.Friendly:
                    return 750; // Middle of friendly range (1-1500)
                case ReputationTier.Allied:
                    return 3000; // Middle of allied range (1501-4500)
                case ReputationTier.Honored:
                    return 6750; // Middle of honored range (4501-9000)
                case ReputationTier.Exalted:
                    return 12000; // Middle of exalted range (9001-15000)
                default:
                    return 0;
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Faction\t{0}", FactionData.GetInfo(m_Faction)?.Name ?? "Unknown");
            list.Add(1060659, "Usage\tDouble-click to manage faction");
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

        #region Faction Stone Gump

        private class FactionStoneGump : Gump
        {
            private PlayerMobile m_Player;
            private VystiaFactionStone m_Stone;
            private FactionInfo m_FactionInfo;
            private int m_Reputation;
            private ReputationTier m_Tier;

            public FactionStoneGump(PlayerMobile player, VystiaFactionStone stone, FactionInfo factionInfo, int reputation, ReputationTier tier)
                : base(50, 50)
            {
                m_Player = player;
                m_Stone = stone;
                m_FactionInfo = factionInfo;
                m_Reputation = reputation;
                m_Tier = tier;

                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                AddPage(0);
                AddBackground(0, 0, 400, 500, 9200);
                AddLabel(20, 20, 0, m_FactionInfo.Name);
                AddLabel(20, 45, 0, m_FactionInfo.Description);

                // Current reputation info
                AddLabel(20, 80, 0, "Current Reputation:");
                AddLabel(200, 80, GetTierColor(tier), string.Format("{0} ({1})", reputation, tier));

                // Reputation tier buttons
                AddLabel(20, 110, 0, "Set Reputation Tier:");
                int y = 135;
                foreach (ReputationTier repTier in Enum.GetValues(typeof(ReputationTier)))
                {
                    AddButton(20, y, 4005, 4007, 100 + (int)repTier, GumpButtonType.Reply, 0);
                    AddLabel(60, y, GetTierColor(repTier), repTier.ToString());
                    y += 25;
                }

                // Quick reputation adjustments
                AddLabel(20, y + 10, 0, "Quick Adjustments:");
                y += 35;
                AddButton(20, y, 4005, 4007, 200, GumpButtonType.Reply, 0);
                AddLabel(60, y, 0, "+100 Reputation");
                y += 25;
                AddButton(20, y, 4005, 4007, 201, GumpButtonType.Reply, 0);
                AddLabel(60, y, 0, "+500 Reputation");
                y += 25;
                AddButton(20, y, 4005, 4007, 202, GumpButtonType.Reply, 0);
                AddLabel(60, y, 0, "+1000 Reputation");
                y += 25;
                AddButton(20, y, 4005, 4007, 203, GumpButtonType.Reply, 0);
                AddLabel(60, y, 0, "-100 Reputation");
                y += 25;
                AddButton(20, y, 4005, 4007, 204, GumpButtonType.Reply, 0);
                AddLabel(60, y, 0, "-500 Reputation");
            }

            private int GetTierColor(ReputationTier tier)
            {
                switch (tier)
                {
                    case ReputationTier.Hostile:
                        return 0x22; // Red
                    case ReputationTier.Unfriendly:
                        return 0x3B2; // Orange
                    case ReputationTier.Neutral:
                        return 0x3B2; // Yellow
                    case ReputationTier.Friendly:
                        return 0x59; // Light green
                    case ReputationTier.Allied:
                        return 0x3F; // Green
                    case ReputationTier.Honored:
                        return 0x3; // Bright green
                    case ReputationTier.Exalted:
                        return 0x35; // Gold
                    default:
                        return 0;
                }
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (m_Player == null || m_Stone == null)
                    return;

                int buttonID = info.ButtonID;

                // Tier buttons (100-106)
                if (buttonID >= 100 && buttonID <= 106)
                {
                    ReputationTier tier = (ReputationTier)(buttonID - 100);
                    m_Stone.SetReputationTier(m_Player, tier);
                    m_Player.SendGump(new FactionStoneGump(m_Player, m_Stone, m_FactionInfo, 
                        VystiaReputation.GetFactionReputation(m_Player, m_Stone.Faction),
                        FactionData.GetTier(VystiaReputation.GetFactionReputation(m_Player, m_Stone.Faction))));
                }
                // Quick adjustment buttons (200-204)
                else if (buttonID >= 200 && buttonID <= 204)
                {
                    int amount = 0;
                    switch (buttonID)
                    {
                        case 200: amount = 100; break;
                        case 201: amount = 500; break;
                        case 202: amount = 1000; break;
                        case 203: amount = -100; break;
                        case 204: amount = -500; break;
                    }
                    m_Stone.AddReputation(m_Player, amount);
                    m_Player.SendGump(new FactionStoneGump(m_Player, m_Stone, m_FactionInfo,
                        VystiaReputation.GetFactionReputation(m_Player, m_Stone.Faction),
                        FactionData.GetTier(VystiaReputation.GetFactionReputation(m_Player, m_Stone.Faction))));
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Command to spawn all faction stones for testing
    /// </summary>
    public class FactionStoneCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("FactionStones", AccessLevel.GameMaster, FactionStones_OnCommand);
            CommandSystem.Register("FS", AccessLevel.GameMaster, FactionStones_OnCommand); // Short alias
        }

        [Usage("FactionStones")]
        [Description("Spawns all faction stones in your backpack")]
        private static void FactionStones_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            if (from == null || from.Backpack == null)
                return;

            // Create faction stones for all 7 factions
            VystiaFactionStone[] stones = new VystiaFactionStone[]
            {
                new VystiaFactionStone(VystiaFaction.Frostguard),
                new VystiaFactionStone(VystiaFaction.FlameLegion),
                new VystiaFactionStone(VystiaFaction.Greenward),
                new VystiaFactionStone(VystiaFaction.ArcaneConclave),
                new VystiaFactionStone(VystiaFaction.Technoguild),
                new VystiaFactionStone(VystiaFaction.Sandwalkers),
                new VystiaFactionStone(VystiaFaction.Voidborn)
            };

            foreach (var stone in stones)
            {
                from.Backpack.DropItem(stone);
            }

            from.SendMessage(0x35, "You have received {0} faction stones!", stones.Length);
        }
    }
}
