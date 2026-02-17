#if VYSTIA_SONGWEAVING
using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Songweaving;

namespace Server.Custom.VystiaClasses.Gumps
{
    public class SongweavingHotbarGump : Gump
    {
        private readonly PlayerMobile m_Player;
        private readonly List<SongEntry> m_Songs;

        public SongweavingHotbarGump(PlayerMobile player) : base(200, 200)
        {
            m_Player = player;
            m_Songs = SongweavingRegistry.GetAll();

            Closable = true;
            Dragable = true;
            Resizable = false;

            int width = 36 + (m_Songs.Count * 40);
            int height = 90;

            AddBackground(0, 0, width, height, 0x13BE);
            AddAlphaRegion(4, 4, width - 8, height - 8);
            AddLabel(10, 8, 0x3B2, GetCrescendoLabel());
            AddLabel(10, 22, 0x3B2, GetConcentrationLabel());

            int x = 10;
            int buttonId = 1;
            TimeSpan cooldown = Server.Custom.VystiaClasses.Systems.SongweavingSystem.GetCooldownRemaining(m_Player);
            int cooldownSeconds = (int)Math.Ceiling(cooldown.TotalSeconds);
            foreach (SongEntry entry in m_Songs)
            {
                AddButton(x, 42, 4005, 4007, buttonId, GumpButtonType.Reply, 0);
                AddItem(x + 2, 40, entry.IconItemId, entry.IconHue);
                if (cooldownSeconds > 0)
                    AddLabel(x + 6, 62, 0x21, cooldownSeconds.ToString());
                buttonId++;
                x += 40;
            }
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

        private string GetConcentrationLabel()
        {
            if (m_Player == null)
                return "Concentration: 0/0";

            return $"Concentration: {m_Player.Concentration}/{m_Player.MaxConcentration}";
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            if (m_Player == null || m_Player.Deleted)
                return;

            int index = info.ButtonID - 1;
            if (index < 0 || index >= m_Songs.Count)
                return;

            SongEntry entry = m_Songs[index];
            TimeSpan cooldown = Server.Custom.VystiaClasses.Systems.SongweavingSystem.GetCooldownRemaining(m_Player);
            if (cooldown > TimeSpan.Zero)
            {
                m_Player.SendMessage("You must wait before performing another song.");
            }
            else
            {
                entry.Song.Begin(m_Player);
            }

            m_Player.CloseGump(typeof(SongweavingHotbarGump));
            m_Player.SendGump(new SongweavingHotbarGump(m_Player));
        }
    }
}
#endif
