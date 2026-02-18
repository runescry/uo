using System;

namespace Server.Mobiles
{
    /// <summary>
    /// Initializes Chronicler spawning system on server startup
    /// This ensures Chronicler NPCs are automatically spawned in major cities
    /// </summary>
    public static class ChroniclerInitializer
    {
        /// <summary>
        /// Called during server initialization to set up Chronicler spawning
        /// </summary>
        public static void Initialize()
        {
            ChroniclerSpawner.Initialize();
        }
    }
}
