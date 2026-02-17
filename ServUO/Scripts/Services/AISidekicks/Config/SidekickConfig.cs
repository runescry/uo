using System;
using Server;

namespace Server.Services.AISidekicks
{
    /// <summary>
    /// Configuration for the AI Sidekicks system
    /// Uses standard Config.Get pattern matching other services
    /// </summary>
    public static class SidekickConfig
    {
        #region System Settings

        private static bool m_Enabled = true;
        private static int m_MaxPerPlayer = 1;

        public static bool Enabled => m_Enabled;
        public static int MaxPerPlayer => m_MaxPerPlayer;

        #endregion

        #region Behavior Settings

        private static int m_DecisionInterval = 5;
        private static bool m_AutonomousBehavior = true;
        private static int m_FollowDistance = 3;
        private static int m_TeleportThreshold = 12;

        public static int DecisionInterval => m_DecisionInterval;
        public static bool AutonomousBehavior => m_AutonomousBehavior;
        public static int FollowDistance => m_FollowDistance;
        public static int TeleportThreshold => m_TeleportThreshold;

        #endregion

        #region Movement Settings

        private static double m_WalkSpeed = 0.2;
        private static double m_RunSpeed = 0.4;
        private static double m_PassiveSpeed = 0.2;

        public static double WalkSpeed => m_WalkSpeed;
        public static double RunSpeed => m_RunSpeed;
        public static double PassiveSpeed => m_PassiveSpeed;

        #endregion

        #region Combat Settings

        private static bool m_PlayerLikeCombat = true;
        private static int m_CombatRange = 1;
        private static int m_ArcherRange = 8;
        private static int m_MageRange = 10;

        public static bool PlayerLikeCombat => m_PlayerLikeCombat;
        public static int CombatRange => m_CombatRange;
        public static int ArcherRange => m_ArcherRange;
        public static int MageRange => m_MageRange;

        #endregion

        #region LLM Settings

        private static bool m_CombatCommentary = true;
        private static int m_CombatCommentaryInterval = 10;
        private static int m_HearingRange = 10;

        public static bool CombatCommentary => m_CombatCommentary;
        public static int CombatCommentaryInterval => m_CombatCommentaryInterval;
        public static int HearingRange => m_HearingRange;

        #endregion

        #region Equipment Settings

        private static bool m_AutoEquipStartingGear = true;
        private static bool m_AllowOwnerEquip = true;

        public static bool AutoEquipStartingGear => m_AutoEquipStartingGear;
        public static bool AllowOwnerEquip => m_AllowOwnerEquip;

        #endregion

        /// <summary>
        /// Load configuration from Sidekick.cfg using Config.Get pattern
        /// </summary>
        public static void Configure()
        {
            // System settings
            m_Enabled = Config.Get("Sidekick.Enabled", true);
            m_MaxPerPlayer = Config.Get("Sidekick.MaxPerPlayer", 1);

            // Behavior settings
            m_DecisionInterval = Config.Get("Sidekick.DecisionInterval", 5);
            m_AutonomousBehavior = Config.Get("Sidekick.AutonomousBehavior", true);
            m_FollowDistance = Config.Get("Sidekick.FollowDistance", 3);
            m_TeleportThreshold = Config.Get("Sidekick.TeleportThreshold", 12);

            // Movement settings
            m_WalkSpeed = Config.Get("Sidekick.WalkSpeed", 0.2);
            m_RunSpeed = Config.Get("Sidekick.RunSpeed", 0.4);
            m_PassiveSpeed = Config.Get("Sidekick.PassiveSpeed", 0.2);

            // Combat settings
            m_PlayerLikeCombat = Config.Get("Sidekick.PlayerLikeCombat", true);
            m_CombatRange = Config.Get("Sidekick.CombatRange", 1);
            m_ArcherRange = Config.Get("Sidekick.ArcherRange", 8);
            m_MageRange = Config.Get("Sidekick.MageRange", 10);

            // LLM settings
            m_CombatCommentary = Config.Get("Sidekick.CombatCommentary", true);
            m_CombatCommentaryInterval = Config.Get("Sidekick.CombatCommentaryInterval", 10);
            m_HearingRange = Config.Get("Sidekick.HearingRange", 10);

            // Equipment settings
            m_AutoEquipStartingGear = Config.Get("Sidekick.AutoEquipStartingGear", true);
            m_AllowOwnerEquip = Config.Get("Sidekick.AllowOwnerEquip", true);

            Console.WriteLine("[SidekickConfig] Configuration loaded:");
            Console.WriteLine($"[SidekickConfig]   Enabled: {m_Enabled}");
            Console.WriteLine($"[SidekickConfig]   MaxPerPlayer: {m_MaxPerPlayer}");
            Console.WriteLine($"[SidekickConfig]   DecisionInterval: {m_DecisionInterval}s");
            Console.WriteLine($"[SidekickConfig]   FollowDistance: {m_FollowDistance}");
            Console.WriteLine($"[SidekickConfig]   WalkSpeed: {m_WalkSpeed}, RunSpeed: {m_RunSpeed}");
        }
    }
}

