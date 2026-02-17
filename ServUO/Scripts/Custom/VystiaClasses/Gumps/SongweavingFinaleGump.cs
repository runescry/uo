#if VYSTIA_SONGWEAVING
using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Systems;

namespace Server.Custom.VystiaClasses.Gumps
{
    public class SongweavingFinaleGump : Gump
    {
        private readonly PlayerMobile m_Player;
        private readonly List<FinaleDefinition> m_Finales;

        public SongweavingFinaleGump(PlayerMobile player) : base(70, 70)
        {
            m_Player = player;
            m_Finales = SongweavingFinaleSystem.GetAll();

            Closable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);
            AddBackground(0, 0, 430, 320, 0x13BE);
            AddAlphaRegion(12, 12, 406, 296);

            AddLabel(20, 18, 1153, "Songweaving Finales");
            AddLabel(20, 36, 0x3B2, "Spend Crescendo on powerful finales");
            AddLabel(250, 18, 0x3B2, GetCrescendoLabel());

            int y = 65;
            int buttonId = 1;
            foreach (FinaleDefinition finale in m_Finales)
            {
                bool canSpend = SongweavingSystem.CanSpendCrescendo(m_Player, finale.Cost);
                int hue = canSpend ? 0x481 : 0x21;

                AddButton(20, y, 0x837, 0x838, buttonId, GumpButtonType.Reply, 0);
                AddLabel(50, y, hue, finale.Name);
                AddLabel(200, y, 0x3B2, finale.Summary);
                AddLabel(365, y, hue, finale.Cost.ToString());

                y += 24;
                buttonId++;
            }
        }

        private string GetCrescendoLabel()
        {
            if (m_Player == null)
                return "Crescendo: 0/0";

            CrescendoResource resource = VystiaResourceManager.GetResource(m_Player, ResourceType.Crescendo) as CrescendoResource;
            int current = resource?.Current ?? 0;
            int max = resource?.Maximum ?? 0;
            return $"Crescendo: {current}/{max}";
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            if (m_Player == null || m_Player.Deleted)
                return;

            int index = info.ButtonID - 1;
            if (index < 0 || index >= m_Finales.Count)
                return;

            FinaleDefinition finale = m_Finales[index];
            SongweavingFinaleSystem.BeginFinale(m_Player, finale);
        }
    }
}
#endif
