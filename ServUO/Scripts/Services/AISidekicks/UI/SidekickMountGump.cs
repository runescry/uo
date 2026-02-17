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
    /// Gump for managing sidekick mounts and pets
    /// </summary>
    public class SidekickMountGump : Gump
    {
        private const int GumpWidth = 500;
        private const int GumpHeight = 400;

        private AutonomousSidekick m_Sidekick;
        private PlayerMobile m_Player;

        public SidekickMountGump(PlayerMobile player, AutonomousSidekick sidekick) : base(50, 50)
        {
            m_Player = player;
            m_Sidekick = sidekick;

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
            AddHtml(20, 20, GumpWidth - 40, 30, $"<center><basefont color=#FFFFFF size=6><b>{m_Sidekick.Name}'s Mounts & Pets</b></basefont></center>", false, false);

            int y = 60;
            int x = 30;

            // Current mount status
            if (m_Sidekick.CurrentMount != null && !m_Sidekick.CurrentMount.Deleted)
            {
                AddHtml(x, y, GumpWidth - 60, 25, $"<basefont color=#FFFF00 size=4><b>Currently Mounted:</b> {m_Sidekick.CurrentMount.Name}</basefont>", false, false);
                y += 30;
                AddButton(x, y, 4017, 4019, 200, GumpButtonType.Reply, 0); // Dismount button
                AddHtml(x + 35, y + 3, 200, 25, "<basefont color=#FFFFFF size=3>Dismount</basefont>", false, false);
                y += 40;
            }
            else
            {
                AddHtml(x, y, GumpWidth - 60, 25, "<basefont color=#CCCCCC size=4><b>Not Currently Mounted</b></basefont>", false, false);
                y += 40;
            }

            // Owned pets list
            AddHtml(x, y, GumpWidth - 60, 25, "<basefont color=#FFFFFF size=4><b>Owned Pets:</b></basefont>", false, false);
            y += 30;

            List<BaseCreature> ownedPets = m_Sidekick.OwnedPets.Where(p => p != null && !p.Deleted).ToList();

            if (ownedPets.Count == 0)
            {
                AddHtml(x, y, GumpWidth - 60, 25, "<basefont color=#CCCCCC size=3>No pets owned. Transfer pets to sidekick to manage them.</basefont>", false, false);
                y += 30;
            }
            else
            {
                int buttonId = 100;
                foreach (BaseCreature pet in ownedPets)
                {
                    bool isMounted = (pet == m_Sidekick.CurrentMount);
                    string status = isMounted ? " (Mounted)" : "";

                    AddHtml(x, y, 200, 20, $"<basefont color=#FFFFFF size=3>{pet.Name}{status}</basefont>", false, false);

                    if (!isMounted)
                    {
                        AddButton(x + 200, y, 4005, 4007, buttonId, GumpButtonType.Reply, 0); // Mount button
                        AddHtml(x + 235, y + 3, 80, 20, "<basefont color=#00FF00 size=3>Mount</basefont>", false, false);
                    }

                    AddButton(x + 320, y, 4017, 4019, buttonId + 1000, GumpButtonType.Reply, 0); // Release button
                    AddHtml(x + 355, y + 3, 80, 20, "<basefont color=#FF0000 size=3>Release</basefont>", false, false);

                    y += 25;
                    buttonId++;
                }
            }

            y += 10;
            AddHtml(x, y, GumpWidth - 60, 60, "<basefont color=#CCCCCC size=3><b>Instructions:</b><br>To give a pet to your sidekick, use the pet transfer command on the pet.<br>Only pets owned by the sidekick can be mounted.</basefont>", false, false);

            // Close button
            int buttonY = GumpHeight - 50;
            AddButton(GumpWidth / 2 - 60, buttonY, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddHtml(GumpWidth / 2 - 30, buttonY + 3, 120, 25, "<center><basefont color=#FF0000 size=4><b>Close</b></basefont></center>", false, false);
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

            if (info.ButtonID == 200)
            {
                // Dismount
                if (m_Sidekick.CurrentMount != null)
                {
                    m_Sidekick.SetMount(null);
                    from.SendMessage($"{m_Sidekick.Name} has dismounted.");
                    // Refresh gump
                    from.SendGump(new SidekickMountGump(m_Player, m_Sidekick));
                }
                return;
            }

            // Mount button (100-199)
            if (info.ButtonID >= 100 && info.ButtonID < 200)
            {
                int index = info.ButtonID - 100;
                List<BaseCreature> ownedPets = m_Sidekick.OwnedPets.Where(p => p != null && !p.Deleted).ToList();

                if (index >= 0 && index < ownedPets.Count)
                {
                    BaseCreature pet = ownedPets[index];
                    m_Sidekick.SetMount(pet);
                    from.SendMessage($"{m_Sidekick.Name} will mount {pet.Name}.");
                    // Refresh gump
                    from.SendGump(new SidekickMountGump(m_Player, m_Sidekick));
                }
                return;
            }

            // Release button (1100-1199)
            if (info.ButtonID >= 1100 && info.ButtonID < 1200)
            {
                int index = info.ButtonID - 1100;
                List<BaseCreature> ownedPets = m_Sidekick.OwnedPets.Where(p => p != null && !p.Deleted).ToList();

                if (index >= 0 && index < ownedPets.Count)
                {
                    BaseCreature pet = ownedPets[index];
                    m_Sidekick.RemovePet(pet);
                    from.SendMessage($"You released {pet.Name} from {m_Sidekick.Name}'s control.");
                    // Refresh gump
                    from.SendGump(new SidekickMountGump(m_Player, m_Sidekick));
                }
                return;
            }
        }
    }
}

