#if VYSTIA_CRESCENDO
using System;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Custom.VystiaClasses.Gumps
{
    public class BardBarLayoutTestGump : Gump
    {
        private const int FrameGumpId = 0x0803;
        private const int BlueBarGumpId = 0x0806;
        private const int RedBarGumpId = 0x0805;

        private const int Width = 140;
        private const int Height = 54;
        private const int BarWidth = 109;
        private const int BarHeight = 5;
        private const int BarX = 34;
        private const int ConcY = 12;
        private const int CresY = 25;

        public static void Initialize()
        {
            CommandSystem.Register("BardBarTest", AccessLevel.GameMaster, BardBarTest_OnCommand);
        }

        private static void BardBarTest_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile pm)
            {
                pm.CloseGump(typeof(BardBarLayoutTestGump));
                pm.SendGump(new BardBarLayoutTestGump());
            }
        }

        public BardBarLayoutTestGump() : base(10, 155)
        {
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            Build();
        }

        private void Build()
        {
            AddImage(0, 0, FrameGumpId);

            // Debug bounds (bright)
            AddAlphaRegion(0, 0, Width, Height);
            AddImageTiled(0, 0, Width, 1, 9274);
            AddImageTiled(0, Height - 1, Width, 1, 9274);
            AddImageTiled(0, 0, 1, Height, 9274);
            AddImageTiled(Width - 1, 0, 1, Height, 9274);

            // Bar rails (full width)
            AddImageTiled(BarX, ConcY, BarWidth, BarHeight, 2624);
            AddImageTiled(BarX, CresY, BarWidth, BarHeight, 2624);

            // Full fill samples
            AddImageTiled(BarX, ConcY, BarWidth, BarHeight, BlueBarGumpId);
            AddImageTiled(BarX, CresY, BarWidth, BarHeight, RedBarGumpId);

            // Crosshair markers
            AddImageTiled(BarX - 2, ConcY - 2, 2, 2, 9274);
            AddImageTiled(BarX - 2, CresY - 2, 2, 2, 9274);

            AddHtml(4, Height - 14, Width - 8, 12, "<BASEFONT COLOR=#FFFFFF>Layout Test</BASEFONT>", false, false);
        }
    }
}
#endif
