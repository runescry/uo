using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Services.AISidekicks;

namespace Server.Services.AISidekicks.UI
{
    /// <summary>
    /// Main menu gump for sidekick management - provides access to all sidekick features
    /// </summary>
    public class SidekickMainMenuGump : Gump
    {
        private const int GumpWidth = 500;
        private const int GumpHeight = 500;

        private PlayerMobile m_Player;

        public SidekickMainMenuGump(PlayerMobile player) : base(50, 50)
        {
            m_Player = player;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            BuildGump();
        }

        private void BuildGump()
        {
            AddPage(0);

            // Background
            AddBackground(0, 0, GumpWidth, GumpHeight, 9200);
            AddImageTiled(10, 10, GumpWidth - 20, GumpHeight - 20, 2624);
            AddAlphaRegion(10, 10, GumpWidth - 20, GumpHeight - 20);

            // Title
            AddHtml(20, 20, GumpWidth - 40, 40, "<center><basefont color=#FFFFFF size=7><b>Sidekick Management</b></basefont></center>", false, false);

            int y = 70;
            int x = 30;

            // Get all sidekicks owned by player
            List<AutonomousSidekick> sidekicks = GetPlayerSidekicks();

            if (sidekicks.Count == 0)
            {
                AddHtml(x, y, GumpWidth - 60, 30, "<center><basefont color=#FFFF00 size=4><b>You don't have any sidekicks yet.</b></basefont></center>", false, false);
                y += 50;

                // Create new sidekick button
                AddButton(GumpWidth / 2 - 100, y, 4005, 4007, 1, GumpButtonType.Reply, 0);
                AddHtml(GumpWidth / 2 - 70, y + 3, 200, 25, "<center><basefont color=#00FF00 size=4><b>Create New Sidekick</b></basefont></center>", false, false);
            }
            else
            {
                // Sidekick list header
                AddHtml(x, y, GumpWidth - 60, 25, "<basefont color=#FFFFFF size=5><b>Your Sidekicks:</b></basefont>", false, false);
                y += 35;

                // Display each sidekick with management options
                int buttonId = 10;
                foreach (AutonomousSidekick sidekick in sidekicks)
                {
                    if (sidekick == null || sidekick.Deleted)
                        continue;

                    // Sidekick name and status
                    string status = sidekick.Alive ? "Alive" : "Dead";
                    string mountStatus = sidekick.CurrentMount != null && !sidekick.CurrentMount.Deleted ? " (Mounted)" : "";
                    AddHtml(x, y, 300, 25, $"<basefont color=#FFFFFF size=4><b>{sidekick.Name}</b> - {sidekick.ArchetypeType} ({status}){mountStatus}</basefont>", false, false);
                    y += 30;

                    // Management buttons for this sidekick
                    int buttonX = x;
                    int buttonY = y;

                    // Equipment button
                    AddButton(buttonX, buttonY, 4005, 4007, buttonId, GumpButtonType.Reply, 0);
                    AddHtml(buttonX + 35, buttonY + 3, 120, 20, "<basefont color=#FFFFFF size=3>Equipment</basefont>", false, false);
                    buttonX += 140;

                    // Mount button
                    AddButton(buttonX, buttonY, 4005, 4007, buttonId + 100, GumpButtonType.Reply, 0);
                    AddHtml(buttonX + 35, buttonY + 3, 120, 20, "<basefont color=#FFFFFF size=3>Mount</basefont>", false, false);
                    buttonX += 140;

                    // Mounts/Pets button
                    AddButton(buttonX, buttonY, 4005, 4007, buttonId + 200, GumpButtonType.Reply, 0);
                    AddHtml(buttonX + 35, buttonY + 3, 120, 20, "<basefont color=#FFFFFF size=3>Mounts/Pets</basefont>", false, false);
                    buttonX += 140;

                    // Properties button (for GM/admin)
                    if (m_Player.AccessLevel >= AccessLevel.GameMaster)
                    {
                        AddButton(buttonX, buttonY, 4005, 4007, buttonId + 300, GumpButtonType.Reply, 0);
                        AddHtml(buttonX + 35, buttonY + 3, 120, 20, "<basefont color=#FFFF00 size=3>Properties</basefont>", false, false);
                        buttonX += 140;
                    }

                    // Delete button
                    AddButton(buttonX, buttonY, 4005, 4007, buttonId + 400, GumpButtonType.Reply, 0);
                    AddHtml(buttonX + 35, buttonY + 3, 120, 20, "<basefont color=#FF0000 size=3>Delete</basefont>", false, false);

                    y += 35;
                    buttonId++;
                }

                y += 20;

                // Create new sidekick button
                AddButton(GumpWidth / 2 - 100, y, 4005, 4007, 1, GumpButtonType.Reply, 0);
                AddHtml(GumpWidth / 2 - 70, y + 3, 200, 25, "<center><basefont color=#00FF00 size=4><b>Create New Sidekick</b></basefont></center>", false, false);
                y += 40;
            }

            // Instructions
            y += 10;
            AddHtml(x, y, GumpWidth - 60, 80, "<basefont color=#CCCCCC size=3><b>Instructions:</b><br>" +
                "• Click 'Equipment' to manage sidekick's gear and inventory<br>" +
                "• Click 'Mounts/Pets' to manage sidekick's pets and mounts<br>" +
                "• Use pet commands like '[name] follow' or '[name] attack [target]' to control sidekicks<br>" +
                "• Sidekicks automatically attack creatures that attack you or them</basefont>", false, false);

            // Close button
            int closeY = GumpHeight - 50;
            AddButton(GumpWidth / 2 - 60, closeY, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddHtml(GumpWidth / 2 - 30, closeY + 3, 120, 25, "<center><basefont color=#FF0000 size=4><b>Close</b></basefont></center>", false, false);
        }

        private List<AutonomousSidekick> GetPlayerSidekicks()
        {
            List<AutonomousSidekick> sidekicks = new List<AutonomousSidekick>();

            foreach (Mobile m in World.Mobiles.Values)
            {
                if (m is AutonomousSidekick sidekick && 
                    sidekick.Owner == m_Player && 
                    !sidekick.Deleted)
                {
                    sidekicks.Add(sidekick);
                }
            }

            return sidekicks;
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (from == null || from != m_Player)
                return;

            if (info.ButtonID == 0)
            {
                // Close
                return;
            }

            if (info.ButtonID == 1)
            {
                // Create new sidekick
                from.SendGump(new AdvancedSidekickCreationGump(m_Player));
                return;
            }

            // Get sidekick list
            List<AutonomousSidekick> sidekicks = GetPlayerSidekicks();

            // Equipment button (10-109)
            if (info.ButtonID >= 10 && info.ButtonID < 110)
            {
                int index = info.ButtonID - 10;
                if (index >= 0 && index < sidekicks.Count)
                {
                    AutonomousSidekick sidekick = sidekicks[index];
                    from.SendGump(new SidekickEquipmentGump(m_Player, sidekick));
                }
                return;
            }

            // Mount button (110-209) - targeting
            if (info.ButtonID >= 110 && info.ButtonID < 210)
            {
                int index = info.ButtonID - 110;
                Console.WriteLine($"[SidekickMainMenuGump] Mount button clicked - ButtonID: {info.ButtonID}, Index: {index}, Sidekicks count: {sidekicks.Count}");
                if (index >= 0 && index < sidekicks.Count)
                {
                    AutonomousSidekick sidekick = sidekicks[index];
                    Console.WriteLine($"[SidekickMainMenuGump] Setting mount target - Sidekick: {sidekick?.Name}, Player: {from?.Name}");
                    if (sidekick != null && !sidekick.Deleted && from is PlayerMobile player)
                    {
                        // Close the gump first, then set the target
                        // This ensures the target cursor appears properly
                        from.CloseGump(typeof(SidekickMainMenuGump));
                        
                        // Small delay to ensure gump is fully closed before setting target
                        Timer.DelayCall(TimeSpan.FromMilliseconds(50), () =>
                        {
                            if (from != null && !from.Deleted && sidekick != null && !sidekick.Deleted)
                            {
                                from.SendMessage("Target a mountable creature for your sidekick.");
                                from.Target = new SidekickMountTarget(player, sidekick);
                                Console.WriteLine($"[SidekickMainMenuGump] Target set - Target type: {from.Target?.GetType().Name}, TargetID: {from.Target?.TargetID}");
                            }
                        });
                    }
                    else
                    {
                        Console.WriteLine($"[SidekickMainMenuGump] ERROR - Sidekick null/deleted or from is not PlayerMobile");
                        from.SendMessage("Unable to set mount target. Please try again.");
                    }
                }
                else
                {
                    Console.WriteLine($"[SidekickMainMenuGump] ERROR - Invalid index: {index}");
                    from.SendMessage("Invalid sidekick selection.");
                }
                return;
            }

            // Mounts/Pets button (210-309)
            if (info.ButtonID >= 210 && info.ButtonID < 310)
            {
                int index = info.ButtonID - 210;
                if (index >= 0 && index < sidekicks.Count)
                {
                    AutonomousSidekick sidekick = sidekicks[index];
                    from.SendGump(new SidekickMountGump(m_Player, sidekick));
                }
                return;
            }

            // Properties button (310-409) - GM only
            if (info.ButtonID >= 310 && info.ButtonID < 410)
            {
                int index = info.ButtonID - 310;
                if (index >= 0 && index < sidekicks.Count && m_Player.AccessLevel >= AccessLevel.GameMaster)
                {
                    AutonomousSidekick sidekick = sidekicks[index];
                    from.SendGump(new Server.Gumps.PropertiesGump(from, sidekick));
                }
                return;
            }

            // Delete button (410-509) - with confirmation
            if (info.ButtonID >= 410 && info.ButtonID < 510)
            {
                int index = info.ButtonID - 410;
                if (index >= 0 && index < sidekicks.Count)
                {
                    AutonomousSidekick sidekick = sidekicks[index];
                    if (sidekick != null && !sidekick.Deleted)
                    {
                        // Show confirmation gump
                        from.SendGump(new SidekickDeleteConfirmGump(m_Player, sidekick));
                    }
                }
                return;
            }
        }
    }
}

