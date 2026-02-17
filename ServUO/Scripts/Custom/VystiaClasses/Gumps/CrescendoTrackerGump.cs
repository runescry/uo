#if VYSTIA_CRESCENDO
using System;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Custom.VystiaClasses.Systems;

namespace Server.Custom.VystiaClasses.Gumps
{
    public class CrescendoTrackerGump : Gump
    {
        private readonly PlayerMobile m_Player;
        private readonly int m_CrescendoCurrent;
        private readonly int m_CrescendoMax;
        private readonly int m_ConcentrationCurrent;
        private readonly int m_ConcentrationMax;

        private const int GumpWidth = 360;
        private const int GumpHeight = 86;
        private const int BarWidth = 327;
        private const int BarHeight = 15;
        private const int BarX = 16;
        private const int ConcY = 12;
        private const int CresY = 40;

        private static readonly Dictionary<Mobile, bool> s_EnabledPlayers = new Dictionary<Mobile, bool>();
        private static readonly Dictionary<Mobile, Timer> s_RefreshTimers = new Dictionary<Mobile, Timer>();

        public static void Initialize()
        {
            CommandSystem.Register("Crescendo", AccessLevel.Player, new CommandEventHandler(Crescendo_OnCommand));
            CommandSystem.Register("CR", AccessLevel.Player, new CommandEventHandler(Crescendo_OnCommand));

            EventSink.Logout += OnLogout;
        }

        private static void OnLogout(LogoutEventArgs e)
        {
            if (e.Mobile != null)
                s_EnabledPlayers.Remove(e.Mobile);
        }

        [Usage("Crescendo")]
        [Description("Toggle the Crescendo tracker display.")]
        private static void Crescendo_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile pm)
            {
                Toggle(pm);
            }
        }

        public static void Toggle(PlayerMobile pm)
        {
            if (IsEnabled(pm))
            {
                Disable(pm);
                pm.SendMessage(0x35, "Crescendo tracker disabled.");
            }
            else
            {
                Enable(pm);
                pm.SendMessage(0x35, "Crescendo tracker enabled. Use [CR to toggle.");
            }
        }

        public static bool IsEnabled(Mobile m)
        {
            return s_EnabledPlayers.TryGetValue(m, out bool enabled) && enabled;
        }

        public static void Enable(PlayerMobile pm)
        {
            s_EnabledPlayers[pm] = true;
            StartTimer(pm);
            Refresh(pm);
        }

        public static void Disable(PlayerMobile pm)
        {
            s_EnabledPlayers[pm] = false;
            StopTimer(pm);
            pm.CloseGump(typeof(CrescendoTrackerGump));
        }

        public static void Refresh(PlayerMobile pm)
        {
            if (pm == null || pm.NetState == null || !IsEnabled(pm))
                return;

            var resource = VystiaResourceManager.GetResource(pm, ResourceType.Crescendo) as CrescendoResource;
            if (resource == null)
                return;

            int concentrationCurrent = pm.Concentration;
            int concentrationMax = pm.MaxConcentration;

            pm.CloseGump(typeof(CrescendoTrackerGump));
            pm.SendGump(new CrescendoTrackerGump(pm, resource.Current, resource.Maximum, concentrationCurrent, concentrationMax));
        }

        private static void StartTimer(PlayerMobile pm)
        {
            StopTimer(pm);
            var t = new RefreshTimer(pm);
            s_RefreshTimers[pm] = t;
            t.Start();
        }

        private static void StopTimer(PlayerMobile pm)
        {
            if (pm != null && s_RefreshTimers.TryGetValue(pm, out var t))
            {
                t.Stop();
                s_RefreshTimers.Remove(pm);
            }
        }

        private class RefreshTimer : Timer
        {
            private readonly PlayerMobile _pm;

            public RefreshTimer(PlayerMobile pm) : base(TimeSpan.Zero, TimeSpan.FromSeconds(1.0))
            {
                _pm = pm;
                Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                if (_pm == null || _pm.Deleted || _pm.NetState == null || !IsEnabled(_pm))
                {
                    StopTimer(_pm);
                    return;
                }

                Refresh(_pm);
            }
        }

        public CrescendoTrackerGump(PlayerMobile player, int crescendoCurrent, int crescendoMax, int concentrationCurrent, int concentrationMax)
            : base(10, 155)
        {
            m_Player = player;
            m_CrescendoCurrent = crescendoCurrent;
            m_CrescendoMax = crescendoMax;
            m_ConcentrationCurrent = concentrationCurrent;
            m_ConcentrationMax = concentrationMax;

            Closable = false;
            Disposable = false;
            Dragable = true;
            Resizable = false;

            BuildGump();
        }

        private void BuildGump()
        {
            DrawBar(BarX, ConcY, m_ConcentrationCurrent, m_ConcentrationMax, 0x0029);
            DrawBar(BarX, CresY, m_CrescendoCurrent, m_CrescendoMax, 0x0028);

            AddHtml(BarX, ConcY - 2, BarWidth, 14,
                $"<BASEFONT COLOR=#FFFFFF><CENTER>{m_ConcentrationCurrent}/{m_ConcentrationMax}</CENTER></BASEFONT>",
                false, false);

            AddHtml(BarX, CresY - 2, BarWidth, 14,
                $"<BASEFONT COLOR=#FFFFFF><CENTER>{m_CrescendoCurrent}/{m_CrescendoMax}</CENTER></BASEFONT>",
                false, false);
        }

        private void DrawBar(int x, int y, int current, int max, int fillGumpId)
        {
            int fillWidth = max > 0 ? (int)((float)current / max * BarWidth) : 0;
            if (fillWidth > 0 && fillWidth < 2)
                fillWidth = 2;

            if (fillWidth > 0)
            {
                AddImageTiled(x, y, fillWidth, BarHeight, fillGumpId);
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
        }
    }
}
#endif
