using System;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Custom.VystiaClasses;
using Server.Custom.VystiaClasses.Systems;

namespace Server.Custom.VystiaClasses.Gumps
{
    /// <summary>
    /// HUD-style gump displaying the player's primary secondary resource (Fury, Chi, etc.)
    /// Compact bar design that updates in real-time during combat.
    /// </summary>
    public class ResourceDisplayGump : Gump
    {
        private readonly PlayerMobile m_Player;
        private readonly ResourceType m_ResourceType;
        private readonly int m_Current;
        private readonly int m_Maximum;

        // Gump dimensions
        private const int GumpWidth = 200;
        private const int GumpHeight = 50;
        private const int BarWidth = 180;
        private const int BarHeight = 16;

        // Static tracking for who has the gump enabled
        private static readonly Dictionary<Mobile, bool> s_EnabledPlayers = new Dictionary<Mobile, bool>();

        public static void Initialize()
        {
            CommandSystem.Register("ResourceBar", AccessLevel.Player, new CommandEventHandler(ResourceBar_OnCommand));
            CommandSystem.Register("RB", AccessLevel.Player, new CommandEventHandler(ResourceBar_OnCommand));
            CommandSystem.Register("ToggleResourceBar", AccessLevel.Player, new CommandEventHandler(ResourceBar_OnCommand));

            EventSink.Logout += OnLogout;
        }

        private static void OnLogout(LogoutEventArgs e)
        {
            if (e.Mobile != null)
                s_EnabledPlayers.Remove(e.Mobile);
        }

        [Usage("ResourceBar")]
        [Description("Toggle the secondary resource bar display.")]
        private static void ResourceBar_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile pm)
            {
                ToggleResourceBar(pm);
            }
        }

        public static void ToggleResourceBar(PlayerMobile pm)
        {
            if (IsEnabled(pm))
            {
                Disable(pm);
                pm.SendMessage(0x35, "Resource bar disabled.");
            }
            else
            {
                Enable(pm);
                pm.SendMessage(0x35, "Resource bar enabled. Use [RB to toggle.");
            }
        }

        public static bool IsEnabled(Mobile m)
        {
            return s_EnabledPlayers.TryGetValue(m, out bool enabled) && enabled;
        }

        public static void Enable(PlayerMobile pm)
        {
            s_EnabledPlayers[pm] = true;
            Refresh(pm);
        }

        public static void Disable(PlayerMobile pm)
        {
            s_EnabledPlayers[pm] = false;
            pm.CloseGump(typeof(ResourceDisplayGump));
        }

        /// <summary>
        /// Refresh the resource bar for a player (called by VystiaResourceManager)
        /// </summary>
        public static void Refresh(PlayerMobile pm)
        {
            if (pm == null || pm.NetState == null || !IsEnabled(pm))
                return;

            pm.CloseGump(typeof(ResourceDisplayGump));

            // Get player's class and primary resource
            var classType = VystiaClassManager.Instance.GetClassType(pm);
            if (classType == PlayerClassTypeV2.None)
                return;

            var manager = VystiaResourceManager.GetManager(pm);
            if (manager == null)
                return;

            // Get the primary resource for this class
            var resourceType = GetPrimaryResourceForClass(classType);
            var resource = manager.GetResource(resourceType);

            if (resource != null)
            {
                pm.SendGump(new ResourceDisplayGump(pm, resourceType, resource.Current, resource.Maximum));
            }
        }

        /// <summary>
        /// Get the primary resource type for a class
        /// </summary>
        private static ResourceType GetPrimaryResourceForClass(PlayerClassTypeV2 classType)
        {
            switch (classType)
            {
                // Fury classes
                case PlayerClassTypeV2.Barbarian:
                case PlayerClassTypeV2.Knight:
                    return ResourceType.Fury;

                // Chi classes
                case PlayerClassTypeV2.Monk:
                    return ResourceType.Chi;

                // Combo classes
                case PlayerClassTypeV2.Fighter:
                case PlayerClassTypeV2.BountyHunter:
                    return ResourceType.ComboPoints;

                // Focus classes
                case PlayerClassTypeV2.Ranger:
                    return ResourceType.Focus;

                // Nature classes
                case PlayerClassTypeV2.Druid:
                case PlayerClassTypeV2.Beastmaster:
                    return ResourceType.LifeForce;

                // Soul classes
                case PlayerClassTypeV2.Warlock:
                case PlayerClassTypeV2.Necromancer:
                    return ResourceType.SoulShards;

                // Spirit classes
                case PlayerClassTypeV2.Shaman:
                case PlayerClassTypeV2.Cleric:
                case PlayerClassTypeV2.Paladin:
                case PlayerClassTypeV2.Templar:
                case PlayerClassTypeV2.Oracle:
                    return ResourceType.Faith;

                // Essence classes
                case PlayerClassTypeV2.Wizard:
                case PlayerClassTypeV2.Alchemist:
                case PlayerClassTypeV2.Summoner:
                case PlayerClassTypeV2.Enchanter:
                    return ResourceType.Charges;

                // Shadow classes
                case PlayerClassTypeV2.Witch:
                case PlayerClassTypeV2.Illusionist:
                    return ResourceType.SoulShards;

                // Energy classes
                case PlayerClassTypeV2.Artificer:
                    return ResourceType.Steam;

                // Chill classes
                case PlayerClassTypeV2.IceMage:
                    return ResourceType.ChillStacks;

                // Heat classes
                case PlayerClassTypeV2.Sorcerer:
                    return ResourceType.Charges;

                // Harmony classes
                case PlayerClassTypeV2.Bard:
                    return ResourceType.Crescendo;

                default:
                    return ResourceType.Fury;
            }
        }

        public ResourceDisplayGump(PlayerMobile player, ResourceType resourceType, int current, int max)
            : base(10, 200) // Position on left side of screen, below paperdoll area
        {
            m_Player = player;
            m_ResourceType = resourceType;
            m_Current = current;
            m_Maximum = max;

            Closable = false;
            Disposable = false;
            Dragable = true;
            Resizable = false;

            BuildGump();
        }

        private void BuildGump()
        {
            // Background
            AddBackground(0, 0, GumpWidth, GumpHeight, 9270);

            // Resource name and class indicator
            string resourceName = GetResourceDisplayName(m_ResourceType);
            int nameColor = GetResourceColor(m_ResourceType);

            AddHtml(10, 5, GumpWidth - 20, 20,
                $"<BASEFONT COLOR=#{nameColor:X6}><CENTER>{resourceName}</CENTER></BASEFONT>",
                false, false);

            // Bar background (dark)
            AddImageTiled(10, 28, BarWidth, BarHeight, 2624);

            // Bar fill (colored based on resource type)
            int fillWidth = m_Maximum > 0 ? (int)((float)m_Current / m_Maximum * BarWidth) : 0;
            if (fillWidth > 0)
            {
                int barHue = GetResourceBarHue(m_ResourceType);
                AddImageTiled(10, 28, fillWidth, BarHeight, 2624);
                AddAlphaRegion(10, 28, fillWidth, BarHeight);

                // Colored overlay
                AddBackground(10, 28, fillWidth, BarHeight, GetBarBackground(m_ResourceType));
            }

            // Numeric display
            AddHtml(10, 28, BarWidth, BarHeight,
                $"<BASEFONT COLOR=#FFFFFF><CENTER>{m_Current} / {m_Maximum}</CENTER></BASEFONT>",
                false, false);
        }

        private string GetResourceDisplayName(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Fury: return "FURY";
                case ResourceType.Chi: return "CHI";
                case ResourceType.ComboPoints: return "COMBO";
                case ResourceType.Focus: return "FOCUS";
                case ResourceType.LifeForce: return "NATURE";
                case ResourceType.SoulShards: return "SOUL";
                case ResourceType.Faith: return "FAITH";
                case ResourceType.Charges: return "ESSENCE";
                case ResourceType.Steam: return "STEAM";
                case ResourceType.ChillStacks: return "CHILL";
                case ResourceType.Crescendo: return "HARMONY";
                case ResourceType.Zeal: return "ZEAL";
                case ResourceType.Fortitude: return "FORTITUDE";
                case ResourceType.Pursuit: return "PURSUIT";
                case ResourceType.Virtues: return "VIRTUES";
                default: return type.ToString().ToUpper();
            }
        }

        private int GetResourceColor(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Fury: return 0xFF4444;      // Red
                case ResourceType.Chi: return 0x44FF44;       // Green
                case ResourceType.ComboPoints: return 0xFFFF44; // Yellow
                case ResourceType.Focus: return 0x44FFFF;     // Cyan
                case ResourceType.LifeForce: return 0x88FF88; // Light Green
                case ResourceType.SoulShards: return 0x9944FF; // Purple
                case ResourceType.Faith: return 0xFFFFAA;     // Light Yellow
                case ResourceType.Charges: return 0x4488FF;   // Blue
                case ResourceType.Steam: return 0xAAAAAA;     // Grey
                case ResourceType.ChillStacks: return 0x88CCFF; // Ice Blue
                case ResourceType.Crescendo: return 0xFFAA44; // Orange
                case ResourceType.Zeal: return 0xFF8844;      // Orange-Red
                case ResourceType.Fortitude: return 0x8888AA; // Steel
                case ResourceType.Pursuit: return 0x88AA88;   // Muted Green
                case ResourceType.Virtues: return 0xFFFFFF;   // White
                default: return 0xFFFFFF;
            }
        }

        private int GetResourceBarHue(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Fury: return 0x21;         // Red
                case ResourceType.Chi: return 0x59;          // Green
                case ResourceType.ComboPoints: return 0x35;  // Yellow
                case ResourceType.Focus: return 0x5A;        // Cyan
                case ResourceType.LifeForce: return 0x59;    // Green
                case ResourceType.SoulShards: return 0x10;   // Purple
                case ResourceType.Faith: return 0x35;        // Gold
                case ResourceType.Charges: return 0x5;       // Blue
                case ResourceType.Steam: return 0x3B2;       // Grey
                case ResourceType.ChillStacks: return 0x480; // Ice Blue
                case ResourceType.Crescendo: return 0x501;   // Orange
                default: return 0x35;
            }
        }

        private int GetBarBackground(ResourceType type)
        {
            // Use different gump backgrounds for visual variety
            switch (type)
            {
                case ResourceType.Fury:
                    return 9154; // Red-tinted
                case ResourceType.Chi:
                    return 9154; // Green
                case ResourceType.SoulShards:
                    return 9154; // Purple
                case ResourceType.ChillStacks:
                    return 9154; // Blue
                default:
                    return 9154; // Default
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            // Gump is non-interactive, just closes silently
        }
    }
}
