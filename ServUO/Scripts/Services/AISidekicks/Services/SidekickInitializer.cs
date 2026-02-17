using System;
using Server;
using Server.Commands;
using Server.Services.AISidekicks;

namespace Server.Services.AISidekicks
{
    /// <summary>
    /// Initializes the AISidekicks service on server startup
    /// </summary>
    public class SidekickInitializer
    {
        private static bool s_Initialized = false;

        public static void Initialize()
        {
            if (s_Initialized)
                return;

            s_Initialized = true;

            Console.WriteLine("");
            Console.WriteLine("========================================");
            Console.WriteLine("  AI Sidekicks System Initialization");
            Console.WriteLine("========================================");
            Console.WriteLine("");

            // Phase 1: Commands
            Console.WriteLine("[AISidekicks] Phase 1: Commands");
            Console.WriteLine("----------------------------------------");
            SpawnSidekickCommand.Initialize();
            Console.WriteLine("[AISidekicks] Commands registered");
            Console.WriteLine("");

            // Phase 2: Configuration
            Console.WriteLine("[AISidekicks] Phase 2: Configuration");
            Console.WriteLine("----------------------------------------");
            LoadConfiguration();
            Console.WriteLine("");

            // Summary
            Console.WriteLine("========================================");
            Console.WriteLine("  AI Sidekicks System Initialized");
            Console.WriteLine("========================================");
            Console.WriteLine("");
        }

        /// <summary>
        /// Load configuration from Sidekick.cfg
        /// </summary>
        private static void LoadConfiguration()
        {
            try
            {
                SidekickConfig.Configure();
                Console.WriteLine("[AISidekicks] Configuration loaded successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AISidekicks] Error loading configuration: {ex.Message}");
            }
        }
    }
}

