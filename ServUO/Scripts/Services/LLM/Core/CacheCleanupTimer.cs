using System;
using Server;

namespace Server.Services.LLM
{
    /// <summary>
    /// Timer to periodically clean up expired cache entries
    /// </summary>
    public class CacheCleanupTimer : Timer
    {
        public CacheCleanupTimer() : base(TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(30))
        {
            Priority = TimerPriority.FiveSeconds;
        }

        protected override void OnTick()
        {
            LLMResponseCache.CleanupExpired();
        }

        public static void Initialize()
        {
            new CacheCleanupTimer().Start();
        }
    }
}

