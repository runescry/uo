using System;
using Server;
using Server.Commands;

namespace Server.Services.LLM
{
    /// <summary>
    /// Commands to manage LLM response cache
    /// </summary>
    public class CacheCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("CacheStats", AccessLevel.GameMaster, CacheStats_OnCommand);
            CommandSystem.Register("ClearCache", AccessLevel.Administrator, ClearCache_OnCommand);
        }

        [Usage("CacheStats")]
        [Description("Display LLM response cache statistics")]
        private static void CacheStats_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            string stats = LLMResponseCache.GetCacheStats();
            string[] lines = stats.Split('\n');
            foreach (string line in lines)
            {
                from.SendMessage(line);
            }
        }

        [Usage("ClearCache")]
        [Description("Clear the LLM response cache")]
        private static void ClearCache_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            LLMResponseCache.ClearCache();
            from.SendMessage("LLM response cache cleared.");
        }
    }
}

