using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Skills;
using Server.Commands;
using Server.Custom.VystiaClasses.Classes;

namespace Server.Items
{
    /// <summary>
    /// Vystia Class Stone - Replaces skill advancement with Vystia class functionality
    /// Maintains skill infrastructure while using Vystia class names and content
    /// </summary>
    public class VystiaClassStone : Item
    {
        private VystiaClass m_Class;
        private VystiaClassDefinition m_Definition;

        [Constructable]
        public VystiaClassStone() : this(VystiaClass.IceMage)
        {
        }

        [Constructable]
        public VystiaClassStone(VystiaClass vystiaClass)
        {
            m_Class = vystiaClass;
            m_Definition = VystiaClassSystem.GetClassDefinition(vystiaClass);

            if (m_Definition != null)
            {
                Hue = m_Definition.Color;
                Name = $"{m_Definition.Name} Class Stone";
            }
            else
            {
                Hue = 1153;
                Name = "Vystia Class Stone";
            }

            Movable = false;
            Light = LightType.Circle300;
        }

        public VystiaClassStone(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage("That is too far away.");
                return;
            }

            from.SendGump(new VystiaClassStoneGump(from, m_Class, m_Definition));
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Definition != null)
            {
                list.Add("Class", m_Definition.Name);
                list.Add("Primary Skill", m_Definition.PrimarySkill.ToString());
                list.Add("Secondary Skills", string.Join(", ", m_Definition.SecondarySkills));
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.Write((int)m_Class);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Class = (VystiaClass)reader.ReadInt();
            m_Definition = VystiaClassSystem.GetClassDefinition(m_Class);
        }
    }

    /// <summary>
    /// Vystia Class Stone Gump
    /// </summary>
    public class VystiaClassStoneGump : BaseGump
    {
        private readonly Mobile m_From;
        private readonly VystiaClass m_Class;
        private readonly VystiaClassDefinition m_Definition;

        public VystiaClassStoneGump(Mobile from, VystiaClass vystiaClass, VystiaClassDefinition definition) 
            : base(50, 50)
        {
            m_From = from;
            m_Class = vystiaClass;
            m_Definition = definition;

            Closable = true;
            Disposable = false;
        }

        public override void OnResponse(NetworkNetState sender, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 0: // Join Class
                    JoinClass();
                    break;
                case 1: // Leave Class
                    LeaveClass();
                    break;
                case 2: // View Status
                    ViewStatus();
                    break;
                case 3: // View Abilities
                    ViewAbilities();
                    break;
                case 4: // View Progress
                    ViewProgress();
                    break;
                case 5: // Close
                    break;
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            // Handle response
        }

        private void JoinClass()
        {
            if (m_From is PlayerMobile player)
            {
                VystiaClassSystem.SetPlayerClass(player, m_Class);
            }
        }

        private void LeaveClass()
        {
            if (m_From is PlayerMobile player)
            {
                VystiaClassSystem.SetPlayerClass(player, VystiaClass.None);
            }
        }

        private void ViewStatus()
        {
            if (m_From is PlayerMobile player)
            {
                var playerData = VystiaClassSystem.GetPlayerData(player);
                var classDef = VystiaClassSystem.GetClassDefinition(m_Class);

                player.SendMessage($"=== {classDef?.Name ?? "Vystia Class"} STATUS ===");
                player.SendMessage($"Current Class: {playerData.Class}");
                player.SendMessage($"Level: {playerData.Level}");
                player.SendMessage($"Experience: {playerData.Experience}");
                player.SendMessage($"Next Level: {VystiaClassSystem.GetExperienceForNextLevel(player)}");
                player.SendMessage($"Last Level Up: {playerData.LastLevelUp:yyyy-MM-dd HH:mm:ss}");
            }
        }

        private void ViewAbilities()
        {
            if (m_From is PlayerMobile player)
            {
                var playerData = VystiaClassSystem.GetPlayerData(player);
                var classDef = VystiaClassSystem.GetClassDefinition(m_Class);

                player.SendMessage($"=== {classDef?.Name ?? "Vystia Class"} ABILITIES ===");
                player.SendMessage($"Unlocked Abilities: {playerData.UnlockedAbilities.Count}");
                
                if (playerData.UnlockedAbilities.Count > 0)
                {
                    player.SendMessage("Abilities:");
                    foreach (var ability in playerData.UnlockedAbilities)
                    {
                        player.SendMessage($"  • {ability}");
                    }
                }
            }
        }

        private void ViewProgress()
        {
            if (m_From is PlayerMobile player)
            {
                var playerData = VystiaClassSystem.GetPlayerData(player);
                var classDef = VystiaClassSystem.GetClassDefinition(m_Class);

                player.SendMessage($"=== {classDef?.Name ?? "Vystia Class"} PROGRESS ===");
                player.SendMessage($"Primary Skill: {classDef?.PrimarySkill}");
                player.SendMessage($"Skill Bonuses: {classDef?.SkillBonuses?.Length ?? 0}");
                
                if (classDef?.SkillBonuses != null)
                {
                    player.SendMessage("Skill Bonuses:");
                    foreach (var bonus in classDef.SkillBonuses)
                    {
                        player.SendMessage($"  • {bonus.Skill}: +{bonus.Bonus:P1}");
                    }
                }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            // Handle double click
        }
    }

    /// <summary>
    /// Vystia Class Stone Gump (Enhanced)
    /// </summary>
    public class VystiaClassStoneGump : BaseGump
    {
        private readonly Mobile m_From;
        private readonly VystiaClass m_Class;
        private readonly VystiaClassDefinition m_Definition;

        public VystiaClassStoneGump(Mobile from, VystiaClass vystiaClass, VystiaClassDefinition definition) 
            : base(100, 100)
        {
            m_From = from;
            m_Class = vystiaClass;
            m_Definition = definition;

            Closable = true;
            Disposable = false;

            AddPage(0);

            AddBackground(0, 0, 400, 350, 9380);
            AddAlphaRegion(10, 10, 380, 330);

            AddHtml(10, 10, 380, 20, $"<CENTER><BASEFONT COLOR=#FFFFFF><BIG>{m_Definition?.Name ?? "Vystia Class"}</BIG></BASEFONT></CENTER>", false, false);
            
            AddHtml(10, 40, 380, 60, $"<BASEFONT COLOR=#FFFFFF>{m_Definition?.Description ?? "A path of mastery and specialization."}</BASEFONT>", false, false);

            AddHtml(10, 110, 380, 20, $"<BASEFONT COLOR=#FFFFFF><CENTER>Primary Skill: {m_Definition?.PrimarySkill}</CENTER></BASEFONT>", false, false);

            AddButton(20, 140, 4005, 4013, 1, GumpButtonType.Reply, 0);
            AddHtml(55, 140, 300, 20, "<BASEFONT COLOR=#FFFFFF>Join Class</BASEFONT>", false, false);

            AddButton(20, 170, 4005, 4013, 2, GumpButtonType.Reply, 0);
            AddHtml(55, 170, 300, 20, "<BASEFONT COLOR=#FFFFFF>Leave Class</BASEFONT>", false, false);

            AddButton(20, 200, 4005, 4013, 3, GumpButtonType.Reply, 0);
            AddHtml(55, 200, 300, 20, "<BASEFONT COLOR=#FFFFFF>View Status</BASEFONT>", false, false);

            AddButton(20, 230, 4005, 4013, 4, GumpButtonType.Reply, 0);
            AddHtml(55, 230, 300, 20, "<BASEFONT COLOR=#FFFFFF>View Abilities</BASEFONT>", false, false);

            AddButton(20, 260, 4005, 4013, 5, GumpButtonType.Reply, 0);
            AddHtml(55, 260, 300, 20, "<BASEFONT COLOR=#FFFFFF>View Progress</BASEFONT>", false, false);

            AddHtml(10, 320, 380, 20, $"<BASEFONT COLOR=#FFFFFF><CENTER>Secondary Skills: {string.Join(", ", m_Definition?.SecondarySkills ?? new string[0])}</CENTER></BASEFONT>", false, false);
        }
    }

    /// <summary>
    /// Portable Vystia Class Stone
    /// </summary>
    public class PortableVystiaClassStone : VystiaClassStone
    {
        [Constructable]
        public PortableVystiaClassStone() : base()
        {
            Movable = true;
            LootType = LootType.Blessed;
        }

        public PortableVystiaClassStone(VystiaClass vystiaClass) : base(vystiaClass)
        {
            Movable = true;
            LootType = LootType.Blessed;
        }

        public PortableVystiaClassStone(Serial serial) : base(serial)
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
                from.SendMessage("Only players can use this class stone.");
            }
        }
    }
}
