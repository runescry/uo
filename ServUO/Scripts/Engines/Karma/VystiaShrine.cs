using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Engines.Karma;
using Server.Custom.VystiaClasses.Religion;

namespace Server.Items
{
    /// <summary>
    /// Vystia Shrine - Replaces standard shrine with Vystia religion functionality
    /// Maintains shrine infrastructure while using Vystia religion names and content
    /// </summary>
    public class VystiaShrine : Item
    {
        private VystiaReligion m_Religion;
        private VystiaReligionDefinition m_Definition;

        [Constructable]
        public VystiaShrine() : this(VystiaReligion.FrosthelmFaith)
        {
        }

        [Constructable]
        public VystiaShrine(VystiaReligion religion)
        {
            m_Religion = religion;
            m_Definition = VystiaReligionSystem.GetReligionDefinition(religion);

            if (m_Definition != null)
            {
                Hue = m_Definition.Color;
                Name = $"{m_Definition.Name} Shrine";
            }
            else
            {
                Hue = 1153;
                Name = "Vystia Shrine";
            }

            Movable = false;
            Light = LightType.Circle300;
        }

        public VystiaShrine(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage("That is too far away.");
                return;
            }

            from.SendGump(new VystiaShrineGump(from, m_Religion, m_Definition));
        }

        public override void OnSingleClick(Mobile from)
        {
            if (m_Definition != null)
            {
                from.SendMessage($"This is a shrine to {m_Definition.Name}.");
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Definition != null)
            {
                list.Add("Religion", m_Definition.Name);
                list.Add("Deity", m_Definition.Deity);
                list.Add("Holy Site", m_Definition.HolySite);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.Write((int)m_Religion);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Religion = (VystiaReligion)reader.ReadInt();
            m_Definition = VystiaReligionSystem.GetReligionDefinition(m_Religion);
        }
    }

    /// <summary>
    /// Vystia Shrine Gump
    /// </summary>
    public class VystiaShrineGump : BaseGump
    {
        private readonly Mobile m_From;
        private readonly VystiaReligion m_Religion;
        private readonly VystiaReligionDefinition m_Definition;

        public VystiaShrineGump(Mobile from, VystiaReligion religion, VystiaReligionDefinition definition) 
            : base(50, 50)
        {
            m_From = from;
            m_Religion = religion;
            m_Definition = definition;

            Closable = true;
            Disposable = false;
        }

        public override void OnResponse(NetworkNetState sender, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 0: // Pray
                    Pray();
                    break;
                case 1: // Tithe
                    Tithe();
                    break;
                case 2: // Pilgrimage
                    Pilgrimage();
                    break;
                case 3: // Join Religion
                    JoinReligion();
                    break;
                case 4: // View Status
                    ViewStatus();
                    break;
                case 5: // View Benefits
                    ViewBenefits();
                    break;
                case 6: // Close
                    break;
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            // Handle response
        }

        private void Pray()
        {
            if (m_From is PlayerMobile player)
            {
                VystiaReligionSystem.PerformDailyPrayer(player);
            }
        }

        private void Tithe()
        {
            if (m_From is PlayerMobile player)
            {
                player.SendMessage("How much gold would you like to tithe?");
                player.BeginTarget(new TitheTarget(player), m_Definition);
            }
        }

        private void Pilgrimage()
        {
            if (m_From is PlayerMobile player)
            {
                VystiaReligionSystem.PerformPilgrimage(player);
            }
        }

        private void JoinReligion()
        {
            if (m_From is PlayerMobile player)
            {
                VystiaReligionSystem.SetPlayerReligion(player, m_Religion);
            }
        }

        private void ViewStatus()
        {
            if (m_From is PlayerMobile player)
            {
                var religion = VystiaReligionSystem.GetPlayerReligion(player);
                var tier = VystiaReligionSystem.GetPietyTier(player);
                var playerData = VystiaReligionSystem.GetPlayerData(player);

                player.SendMessage($"Current Religion: {religion}");
                player.SendMessage($"Piety Tier: {tier}");
                player.SendMessage($"Piety: {playerData.Piety}");
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
    /// Tithe Target for gold donation
    /// </summary>
    public class TitheTarget : Target
    {
        private readonly VystiaReligionDefinition m_Definition;

        public TitheTarget(PlayerMobile from, VystiaReligionDefinition definition) : base(-1, false, TargetFlags.None)
        {
            m_Definition = definition;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (from is PlayerMobile player)
            {
                if (targeted is Gold gold)
                {
                    var amount = gold.Amount;
                    if (amount > 0)
                    {
                        if (VystiaReligionSystem.PerformTithe(player, amount))
                        {
                            gold.Delete();
                        }
                    }
                    else
                    {
                        player.SendMessage("That gold has no value.");
                    }
                }
                else
                {
                    player.SendMessage("You must target gold to tithe.");
                }
            }
        }
    }

    /// <summary>
    /// Vystia Shrine Gump (Enhanced)
    /// </summary>
    public class VystiaShrineGump : BaseGump
    {
        private readonly Mobile m_From;
        private readonly VystiaReligion m_Religion;
        private readonly VystiaReligionDefinition m_Definition;

        public VystiaShrineGump(Mobile from, VystiaReligion religion, VystiaReligionDefinition definition) 
            : base(100, 100)
        {
            m_From = from;
            m_Religion = religion;
            m_Definition = definition;

            Closable = true;
            Disposable = false;

            AddPage(0);

            AddBackground(0, 0, 400, 350, 9380);
            AddAlphaRegion(10, 10, 380, 330);

            AddHtml(10, 10, 380, 20, $"<CENTER><BASEFONT COLOR=#FFFFFF><BIG>{m_Definition?.Name ?? "Vystia Shrine"}</BIG></BASEFONT></CENTER>", false, false);
            
            AddHtml(10, 40, 380, 60, $"<BASEFONT COLOR=#FFFFFF>{m_Definition?.Description ?? "A sacred place of worship and devotion."}</BASEFONT>", false, false);

            AddHtml(10, 110, 380, 20, $"<BASEFONT COLOR=#FFFFFF><CENTER>Deity: {m_Definition?.Deity ?? "Unknown"}</CENTER></BASEFONT>", false, false);

            AddButton(20, 140, 4005, 4013, 1, GumpButtonType.Reply, 0);
            AddHtml(55, 140, 300, 20, "<BASEFONT COLOR=#FFFFFF>Pray</BASEFONT>", false, false);

            AddButton(20, 170, 4005, 4013, 2, GumpButtonType.Reply, 0);
            AddHtml(55, 170, 300, 20, "<BASEFONT COLOR=#FFFFFF>Tithe Gold</BASEFONT>", false, false);

            AddButton(20, 200, 4005, 4013, 3, GumpButtonType.Reply, 0);
            AddHtml(55, 200, 300, 20, "<BASEFONT COLOR=#FFFFFF>Pilgrimage</BASEFONT>", false, false);

            AddButton(20, 230, 4005, 4013, 4, GumpButtonType.Reply, 0);
            AddHtml(55, 230, 300, 20, "<BASEFONT COLOR=#FFFFFF>Join Religion</BASEFONT>", false, false);

            AddButton(20, 260, 4005, 4013, 5, GumpButtonType.Reply, 0);
            AddHtml(55, 260, 300, 20, "<BASEFONT COLOR=#FFFFFF>View Status</BASEFONT>", false, false);

            AddButton(20, 290, 4005, 4013, 6, GumpButtonType.Reply, 0);
            AddHtml(55, 290, 300, 20, "<BASEFONT COLOR=#FFFFFF>View Benefits</BASEFONT>", false, false);

            AddHtml(10, 320, 380, 20, $"<BASEFONT COLOR=#FFFFFF><CENTER>Holy Site: {m_Definition?.HolySite ?? "Unknown"}</CENTER></BASEFONT>", false, false);
        }
    }

    /// <summary>
    /// Portable Vystia Shrine
    /// </summary>
    public class PortableVystiaShrine : VystiaShrine
    {
        [Constructable]
        public PortableVystiaShrine() : base()
        {
            Movable = true;
            LootType = LootType.Blessed;
        }

        public PortableVystiaShrine(VystiaReligion religion) : base(religion)
        {
            Movable = true;
            LootType = LootType.Blessed;
        }

        public PortableVystiaShrine(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.IsPlayer())
            {
                base.OnDoubleClick(from);
            }
            else
            {
                from.SendMessage("Only players can use this shrine.");
            }
        }
    }
}
