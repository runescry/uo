using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Services.AISidekicks;

namespace Server.Services.AISidekicks.UI
{
    /// <summary>
    /// Confirmation gump for deleting a sidekick
    /// </summary>
    public class SidekickDeleteConfirmGump : Gump
    {
        private PlayerMobile m_Player;
        private AutonomousSidekick m_Sidekick;

        public SidekickDeleteConfirmGump(PlayerMobile player, AutonomousSidekick sidekick) : base(50, 50)
        {
            m_Player = player;
            m_Sidekick = sidekick;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);

            // Background
            AddBackground(0, 0, 400, 200, 9200);
            AddImageTiled(10, 10, 380, 180, 2624);
            AddAlphaRegion(10, 10, 380, 180);

            // Title
            AddHtml(20, 20, 360, 30, "<center><basefont color=#FF0000 size=6><b>Delete Sidekick?</b></basefont></center>", false, false);

            // Warning message
            string sidekickName = sidekick != null && !sidekick.Deleted ? sidekick.Name : "Unknown";
            AddHtml(20, 60, 360, 60, 
                $"<center><basefont color=#FFFFFF size=4>Are you sure you want to delete <b>{sidekickName}</b>?<br>" +
                "This action cannot be undone!</basefont></center>", false, false);

            // Yes button
            AddButton(100, 140, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtml(135, 143, 100, 25, "<center><basefont color=#FF0000 size=4><b>Yes, Delete</b></basefont></center>", false, false);

            // No button
            AddButton(220, 140, 4005, 4007, 0, GumpButtonType.Reply, 0);
            AddHtml(255, 143, 100, 25, "<center><basefont color=#00FF00 size=4><b>Cancel</b></basefont></center>", false, false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (from == null || from != m_Player)
                return;

            if (info.ButtonID == 0)
            {
                // Cancel - reopen main menu
                from.SendGump(new SidekickMainMenuGump(m_Player));
                return;
            }

            if (info.ButtonID == 1)
            {
                // Confirm delete
                if (m_Sidekick != null && !m_Sidekick.Deleted)
                {
                    string sidekickName = m_Sidekick.Name;
                    
                    // Release any pets/mounts first
                    if (m_Sidekick.OwnedPets != null)
                    {
                        foreach (var pet in m_Sidekick.OwnedPets.ToArray())
                        {
                            if (pet != null && !pet.Deleted)
                            {
                                pet.ControlMaster = null;
                                pet.Controlled = false;
                            }
                        }
                    }

                    // Dismount if mounted
                    if (m_Sidekick.Mount != null)
                    {
                        m_Sidekick.Mount.Rider = null;
                    }

                    // Delete the sidekick
                    m_Sidekick.Delete();
                    
                    from.SendMessage($"You have deleted {sidekickName}.");
                    Console.WriteLine($"[SidekickDeleteConfirmGump] {from.Name} deleted sidekick {sidekickName}");
                }
                else
                {
                    from.SendMessage("That sidekick no longer exists.");
                }

                // Reopen main menu
                from.SendGump(new SidekickMainMenuGump(m_Player));
            }
        }
    }
}

