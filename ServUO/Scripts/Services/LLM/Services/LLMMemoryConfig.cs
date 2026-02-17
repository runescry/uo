using System;
using Server;

namespace Server.Services.LLM
{
    /// <summary>
    /// Configuration for the LLM Memory System
    /// </summary>
    public static class LLMMemoryConfig
    {
        private static bool m_Enabled = true;
        private static string m_DatabaseProvider = "SQLite";
        private static string m_DatabaseConnectionString = "";
        private static bool m_DatabasePooling = true;
        private static int m_DatabaseMinPoolSize = 5;
        private static int m_DatabaseMaxPoolSize = 20;
        private static int m_DatabaseCommandTimeout = 30;

        private static bool m_CacheEnabled = true;
        private static string m_CacheProvider = "Redis";
        private static string m_CacheConnectionString = "";
        private static int m_CacheDefaultTTL = 600;
        private static int m_CacheMemoryTTL = 600;
        private static int m_CacheRelationshipTTL = 900;
        private static int m_CacheConversationTTL = 1800;

        private static int m_MaxMemoriesPerNPC = 50;
        private static int m_MemoryImportanceThreshold = 3;
        private static bool m_AutoCleanupEnabled = true;
        private static int m_CleanupIntervalHours = 24;

        private static bool m_RelationshipDecayEnabled = true;
        private static int m_RelationshipDecayDays = 7;
        private static int m_RelationshipDecayAmount = 1;

        private static int m_FlushIntervalSeconds = 10;
        private static int m_DatabaseBatchSize = 100;

        public static void Configure()
        {
            // Load configuration file manually (matching LocalLLMService pattern)
            string configPath = System.IO.Path.Combine(Core.BaseDirectory.Directory, "Config", "LLMMemory.cfg");
            
            if (System.IO.File.Exists(configPath))
            {
                try
                {
                    string[] lines = System.IO.File.ReadAllLines(configPath);
                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                            continue;

                        int equalsIndex = line.IndexOf('=');
                        if (equalsIndex < 0)
                            continue;

                        string key = line.Substring(0, equalsIndex).Trim();
                        string value = line.Substring(equalsIndex + 1).Trim();

                        // Parse configuration values
                        if (key == "LLMMemory.MemorySystem.Enabled")
                            bool.TryParse(value, out m_Enabled);
                        else if (key == "LLMMemory.MemoryDatabase.Provider")
                            m_DatabaseProvider = value;
                        else if (key == "LLMMemory.MemoryDatabase.ConnectionString")
                            m_DatabaseConnectionString = value;
                        else if (key == "LLMMemory.MemoryDatabase.Pooling")
                            bool.TryParse(value, out m_DatabasePooling);
                        else if (key == "LLMMemory.MemoryDatabase.MinPoolSize")
                            int.TryParse(value, out m_DatabaseMinPoolSize);
                        else if (key == "LLMMemory.MemoryDatabase.MaxPoolSize")
                            int.TryParse(value, out m_DatabaseMaxPoolSize);
                        else if (key == "LLMMemory.MemoryDatabase.CommandTimeout")
                            int.TryParse(value, out m_DatabaseCommandTimeout);
                        else if (key == "LLMMemory.MemoryCache.Enabled")
                            bool.TryParse(value, out m_CacheEnabled);
                        else if (key == "LLMMemory.MemoryCache.Provider")
                            m_CacheProvider = value;
                        else if (key == "LLMMemory.MemoryCache.ConnectionString")
                            m_CacheConnectionString = value;
                        else if (key == "LLMMemory.MemoryCache.DefaultTTL")
                            int.TryParse(value, out m_CacheDefaultTTL);
                        else if (key == "LLMMemory.MemoryCache.MemoryTTL")
                            int.TryParse(value, out m_CacheMemoryTTL);
                        else if (key == "LLMMemory.MemoryCache.RelationshipTTL")
                            int.TryParse(value, out m_CacheRelationshipTTL);
                        else if (key == "LLMMemory.MemoryCache.ConversationTTL")
                            int.TryParse(value, out m_CacheConversationTTL);
                        else if (key == "LLMMemory.MemorySystem.MaxMemoriesPerNPC")
                            int.TryParse(value, out m_MaxMemoriesPerNPC);
                        else if (key == "LLMMemory.MemorySystem.MemoryImportanceThreshold")
                            int.TryParse(value, out m_MemoryImportanceThreshold);
                        else if (key == "LLMMemory.MemorySystem.AutoCleanupEnabled")
                            bool.TryParse(value, out m_AutoCleanupEnabled);
                        else if (key == "LLMMemory.MemorySystem.CleanupIntervalHours")
                            int.TryParse(value, out m_CleanupIntervalHours);
                        else if (key == "LLMMemory.MemorySystem.RelationshipDecayEnabled")
                            bool.TryParse(value, out m_RelationshipDecayEnabled);
                        else if (key == "LLMMemory.MemorySystem.RelationshipDecayDays")
                            int.TryParse(value, out m_RelationshipDecayDays);
                        else if (key == "LLMMemory.MemorySystem.RelationshipDecayAmount")
                            int.TryParse(value, out m_RelationshipDecayAmount);
                        else if (key == "LLMMemory.MemoryDatabase.FlushIntervalSeconds")
                            int.TryParse(value, out m_FlushIntervalSeconds);
                        else if (key == "LLMMemory.MemoryDatabase.BatchSize")
                            int.TryParse(value, out m_DatabaseBatchSize);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LLMMemory] Error loading config: {ex.Message}");
                }
            }

            // Console.WriteLine("[LLMMemory] Configuration loaded:");
            // Console.WriteLine($"[LLMMemory]   Enabled: {m_Enabled}");
            // Console.WriteLine($"[LLMMemory]   Database: {m_DatabaseProvider}");
        }

        // Properties
        public static bool Enabled => m_Enabled;
        public static string DatabaseProvider => m_DatabaseProvider;
        public static string DatabaseConnectionString => m_DatabaseConnectionString;
        public static bool DatabasePooling => m_DatabasePooling;
        public static int DatabaseMinPoolSize => m_DatabaseMinPoolSize;
        public static int DatabaseMaxPoolSize => m_DatabaseMaxPoolSize;
        public static int DatabaseCommandTimeout => m_DatabaseCommandTimeout;

        public static bool CacheEnabled => m_CacheEnabled;
        public static string CacheProvider => m_CacheProvider;
        public static string CacheConnectionString => m_CacheConnectionString;
        public static int CacheDefaultTTL => m_CacheDefaultTTL;
        public static int CacheMemoryTTL => m_CacheMemoryTTL;
        public static int CacheRelationshipTTL => m_CacheRelationshipTTL;
        public static int CacheConversationTTL => m_CacheConversationTTL;

        public static int MaxMemoriesPerNPC => m_MaxMemoriesPerNPC;
        public static int MemoryImportanceThreshold => m_MemoryImportanceThreshold;
        public static bool AutoCleanupEnabled => m_AutoCleanupEnabled;
        public static int CleanupIntervalHours => m_CleanupIntervalHours;

        public static bool RelationshipDecayEnabled => m_RelationshipDecayEnabled;
        public static int RelationshipDecayDays => m_RelationshipDecayDays;
        public static int RelationshipDecayAmount => m_RelationshipDecayAmount;

        public static int FlushIntervalSeconds => m_FlushIntervalSeconds;
        public static int DatabaseBatchSize => m_DatabaseBatchSize;
    }
}

