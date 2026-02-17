#if VYSTIA_SONGWEAVING
using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Songweaving;

namespace Server.Custom.VystiaClasses.Gumps
{
    public class SongweavingSongbookGump : Gump
    {
        private readonly PlayerMobile m_Player;
        private readonly List<SongEntry> m_Songs;

        public SongweavingSongbookGump(PlayerMobile player) : base(50, 50)
        {
            m_Player = player;
            m_Songs = SongweavingRegistry.GetAll();

            Closable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);
            AddBackground(0, 0, 430, 380, 0x13BE);
            AddAlphaRegion(12, 12, 406, 356);

            AddLabel(20, 18, 1153, "Songbook of Weaving");
            AddLabel(20, 36, 0x3B2, "Select a song to perform");
            AddLabel(250, 18, 0x3B2, GetConcentrationLabel());
            AddLabel(250, 36, 0x3B2, GetCrescendoLabel());
            AddButton(330, 18, 4011, 4012, 1000, GumpButtonType.Reply, 0);
            AddLabel(355, 18, 0x3B2, "Hotbar");
            AddButton(330, 36, 4011, 4012, 1001, GumpButtonType.Reply, 0);
            AddLabel(355, 36, 0x3B2, "Finales");

            int y = 65;
            int buttonId = 1;
            TimeSpan cooldown = Server.Custom.VystiaClasses.Systems.SongweavingSystem.GetCooldownRemaining(m_Player);
            int cooldownSeconds = (int)Math.Ceiling(cooldown.TotalSeconds);
            foreach (SongEntry entry in m_Songs)
            {
                AddButton(20, y, 0x837, 0x838, buttonId, GumpButtonType.Reply, 0);
                AddItem(50, y - 2, entry.IconItemId, entry.IconHue);
                AddLabel(75, y, 0x481, entry.DisplayName);
                AddLabel(220, y, 0x3B2, entry.Summary);
                if (cooldownSeconds > 0)
                    AddLabel(370, y, 0x21, cooldownSeconds.ToString());

                y += 24;
                buttonId++;
            }
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            if (m_Player == null || m_Player.Deleted)
                return;

            if (info.ButtonID == 1000)
            {
                m_Player.CloseGump(typeof(SongweavingHotbarGump));
                m_Player.SendGump(new SongweavingHotbarGump(m_Player));
                return;
            }

            if (info.ButtonID == 1001)
            {
                m_Player.CloseGump(typeof(Server.Custom.VystiaClasses.Gumps.SongweavingFinaleGump));
                m_Player.SendGump(new Server.Custom.VystiaClasses.Gumps.SongweavingFinaleGump(m_Player));
                return;
            }

            int index = info.ButtonID - 1;
            if (index < 0 || index >= m_Songs.Count)
                return;

            SongEntry entry = m_Songs[index];
            TimeSpan cooldown = Server.Custom.VystiaClasses.Systems.SongweavingSystem.GetCooldownRemaining(m_Player);
            if (cooldown > TimeSpan.Zero)
            {
                m_Player.SendMessage("You must wait before performing another song.");
                m_Player.SendGump(new SongweavingSongbookGump(m_Player));
                return;
            }

            entry.Song.Begin(m_Player);
        }

        private string GetConcentrationLabel()
        {
            if (m_Player == null)
                return "Concentration: 0/0";

            return $"Concentration: {m_Player.Concentration}/{m_Player.MaxConcentration}";
        }

        private string GetCrescendoLabel()
        {
            if (m_Player == null)
                return "Crescendo: 0/0";

            var resource = Server.Custom.VystiaClasses.Systems.VystiaResourceManager.GetResource(
                m_Player,
                Server.Custom.VystiaClasses.Systems.ResourceType.Crescendo) as Server.Custom.VystiaClasses.Systems.CrescendoResource;

            int current = resource?.Current ?? 0;
            int max = resource?.Maximum ?? 0;
            return $"Crescendo: {current}/{max}";
        }
    }
}
#endif
