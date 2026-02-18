using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Engines.VvV;
using Server.Custom.VystiaClasses.Factions;

namespace Server.Items
{
    /// <summary>
    /// Vystia Faction Stone - Replaces VvV Stone with Vystia faction functionality
    /// Maintains VvV infrastructure while using Vystia faction names and content
    /// </summary>
    public class VystiaFactionStone : Item
    {
        private VystiaFaction m_Faction;
        private VystiaFactionDefinition m_Definition;

        [Constructable]
        public VystiaFactionStone() : this(VystiaFaction.Frostguard)
        {
        }

        [Constructable]
        public VystiaFactionStone(VystiaFaction faction)
        {
            m_Faction = faction;
            m_Definition = VystiaFactionSystem.GetFactionDefinition(faction);

            if (m_Definition != null)
            {
                Hue = m_Definition.Color;
                Name = $"{m_Definition.Name} Faction Stone";
            }
            else
            {
                Hue = 1153;
                Name = "Vystia Faction Stone";
            }

            Movable = false;
            Light = LightType.Circle300;
        }

        public VystiaFactionStone(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage("That is too far away.");
                return;
            }

            from.SendGump(new VystiaFactionStoneGump(from, m_Faction, m_Definition));
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Definition != null)
            {
                list.Add("Faction", m_Definition.Name);
                list.Add("Stronghold", m_Definition.Stronghold);
                list.Add("Leader", m_Definition.Leader);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.Write((int)m_Faction);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Faction = (VystiaFaction)reader.ReadInt();
            m_Definition = VystiaFactionSystem.GetFactionDefinition(m_Faction);
        }
    }

    /// <summary>
    /// Vystia Faction Stone Gump
    /// </summary>
    public class VystiaFactionStoneGump : BaseGump
    {
        private readonly Mobile m_From;
        private readonly VystiaFaction m_Faction;
        private readonly VystiaFactionDefinition m_Definition;

        public VystiaFactionStoneGump(Mobile from, VystiaFaction faction, VystiaFactionDefinition definition) 
            : base(50, 50)
        {
            m_From = from;
            m_Faction = faction;
            m_Definition = definition;

            Closable = true;
            Disposable = false;
        }

        public override void OnResponse(NetworkNetState sender, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 0: // Join Faction
                    JoinFaction();
                    break;
                case 1: // Leave Faction
                    LeaveFaction();
                    break;
                case 2: // View Status
                    ViewStatus();
                    break;
                case 3: // View Benefits
                    ViewBenefits();
                    break;
                case 4: // Close
                    break;
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            // Handle response
        }

        private void JoinFaction()
        {
            if (m_From is PlayerMobile player)
            {
                VystiaFactionSystem.SetPlayerFaction(player, m_Faction);
            }
        }

        private void LeaveFaction()
        {
            if (m_From is PlayerMobile player)
            {
                VystiaFactionSystem.SetPlayerFaction(player, VystiaFaction.None);
            }
        }

        private void ViewStatus()
        {
            if (m_From is PlayerMobile player)
            {
                var faction = VystiaFactionSystem.GetPlayerFaction(player);
                var rank = VystiaFactionSystem.GetFactionRank(player);
                var playerData = VystiaFactionSystem.GetPlayerData(player);

                player.SendMessage($"Current Faction: {faction}");
                player.SendMessage($"Rank: {rank}");
                player.SendMessage($"Silver: {playerData.Silver}");
            }
        }

        private void ViewBenefits()
        {
            if (m_Definition != null)
            {
                m_From.SendMessage($"{m_Definition.Name} Benefits:");
                m_From.SendMessage(m_Definition.Benefits);
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            // Handle double click
        }
    }

    /// <summary>
    /// Vystia Faction Stone Gump (Enhanced)
    /// </summary>
    public class VystiaFactionStoneGump : BaseGump
    {
        private readonly Mobile m_From;
        private readonly VystiaFaction m_Faction;
        private readonly VystiaFactionDefinition m_Definition;

        public VystiaFactionStoneGump(Mobile from, VystiaFaction faction, VystiaFactionDefinition definition) 
            : base(100, 100)
        {
            m_From = from;
            m_Faction = faction;
            m_Definition = definition;

            Closable = true;
            Disposable = false;

            AddPage(0);

            AddBackground(0, 0, 400, 300, 9380);
            AddAlphaRegion(10, 10, 380, 280);

            AddHtml(10, 10, 380, 20, $"<CENTER><BASEFONT COLOR=#FFFFFF><BIG>{m_Definition?.Name ?? "Vystia Faction"}</BIG></BASEFONT></CENTER>", false, false);
            
            AddHtml(10, 40, 380, 60, $"<BASEFONT COLOR=#FFFFFF>{m_Definition?.Description ?? "Join a Vystia faction to gain special benefits and rewards."}</BASEFONT>", false, false);

            AddButton(20, 110, 4005, 4013, 1, GumpButtonType.Reply, 0);
            AddHtml(55, 110, 300, 20, "<BASEFONT COLOR=#FFFFFF>Join Faction</BASEFONT>", false, false);

            AddButton(20, 140, 4005, 4013, 2, GumpButtonType.Reply, 0);
            AddHtml(55, 140, 300, 20, "<BASEFONT COLOR=#FFFFFF>Leave Faction</BASEFONT>", false, false);

            AddButton(20, 170, 4005, 4013, 3, GumpButtonType.Reply, 0);
            AddHtml(55, 170, 300, 20, "<BASEFONT COLOR=#FFFFFF>View Status</BASEFONT>", false, false);

            AddButton(20, 200, 4005, 4013, 4, GumpButtonType.Reply, 0);
            AddHtml(55, 200, 300, 20, "<BASEFONT COLOR=#FFFFFF>View Benefits</BASEFONT>", false, false);

            AddHtml(10, 240, 380, 40, $"<BASEFONT COLOR=#FFFFFF><CENTER>Stronghold: {m_Definition?.Stronghold ?? "Unknown"}</CENTER></BASEFONT>", false, false);
        }
    }
}
