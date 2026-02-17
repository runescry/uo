using System;
using System.Collections.Generic;
using System.IO;

using Server.Commands;
using Server.Items;
using Server.Network;

namespace Server.Misc
{
    public static class SeasonCycle
    {
        // Configuration - Based on UO Time
        // 5 real seconds = 1 UO minute
        // 120 real minutes (2 hours) = 1 UO day
        // 7 UO years = 1 season (~14 real hours per season)
        // Full year cycle (4 seasons) = ~56 real hours
        private const int UOYearsPerSeason = 7;
        private const int UODaysPerYear = 365;
        private const int UODaysPerSeason = UOYearsPerSeason * UODaysPerYear; // 2555 UO days per season

        // UO time constants (from Clock class)
        private const double SecondsPerUOMinute = 5.0;
        private const double SecondsPerUODay = SecondsPerUOMinute * 60 * 24; // 7200 real seconds = 2 hours per UO day
        private const double SecondsPerSeason = UODaysPerSeason * SecondsPerUODay; // ~5.1 million seconds (~14 hours)

        // Testing mode: Set to true to make seasons change every 2 real minutes instead of ~14 hours
        private static readonly bool TestingMode = false;
        private static readonly double TestingSecondsPerSeason = 120.0; // 2 minutes for testing

        private static readonly bool EnableSeasonCycle = true;
        
        // Track season start time (real time when in testing mode, UO time otherwise)
        private static DateTime _seasonStartRealTime = DateTime.UtcNow;

        // Maps that should cycle seasons (by map index)
        private static readonly HashSet<int> CyclingMaps = new HashSet<int>
        {
            1,  // Trammel
            2,  // Ilshenar
            3,  // Malas
            4,  // Tokuno
            5,  // TerMur
            9,  // Vystia (custom map)
        };

        // Season names for display
        private static readonly string[] SeasonNames = { "Spring", "Summer", "Fall", "Winter" };

        // Track season by UO total minutes at season start
        private static int _currentSeason = 0;
        private static long _seasonStartUOMinutes = 0;

        // File to persist season state across restarts
        private static readonly string SavePath = Path.Combine(Core.BaseDirectory, "Saves", "SeasonCycle.bin");

        // WorldStart for UO time calculation (same as Clock class)
        private static readonly DateTime WorldStart = new DateTime(1997, 9, 1);

        public static int CurrentSeason => _currentSeason;
        public static string CurrentSeasonName => SeasonNames[_currentSeason];

        /// <summary>
        /// Gets the current total UO minutes since WorldStart
        /// </summary>
        private static long GetTotalUOMinutes()
        {
            var timeSpan = DateTime.UtcNow - WorldStart;
            return (long)(timeSpan.TotalSeconds / SecondsPerUOMinute);
        }

        /// <summary>
        /// Gets UO minutes until next season change
        /// </summary>
        public static long UOMinutesUntilNextSeason
        {
            get
            {
                if (TestingMode)
                {
                    // In testing mode, convert real time to UO minutes for display
                    TimeSpan realTimeRemaining = TimeUntilNextSeason;
                    return (long)(realTimeRemaining.TotalSeconds / SecondsPerUOMinute);
                }
                
                long currentMinutes = GetTotalUOMinutes();
                long minutesPerSeason = (long)(UODaysPerSeason * 24 * 60); // UO minutes per season
                long seasonEndMinutes = _seasonStartUOMinutes + minutesPerSeason;
                return Math.Max(0, seasonEndMinutes - currentMinutes);
            }
        }

        /// <summary>
        /// Gets real time until next season change
        /// </summary>
        public static TimeSpan TimeUntilNextSeason
        {
            get
            {
                if (TestingMode)
                {
                    // In testing mode, use real time directly
                    TimeSpan elapsed = DateTime.UtcNow - _seasonStartRealTime;
                    TimeSpan remaining = TimeSpan.FromSeconds(TestingSecondsPerSeason) - elapsed;
                    return remaining.TotalSeconds > 0 ? remaining : TimeSpan.Zero;
                }
                
                double realSecondsRemaining = UOMinutesUntilNextSeason * SecondsPerUOMinute;
                return TimeSpan.FromSeconds(realSecondsRemaining);
            }
        }

        public static void Initialize()
        {
            try
            {
                if (!EnableSeasonCycle)
                {
                    Utility.PushColor(ConsoleColor.Yellow);
                    Console.WriteLine("[SeasonCycle] Season cycle is disabled.");
                    Utility.PopColor();
                    return;
                }

                // Load saved season state
                Load();

                // Check if we need to advance the season
                CheckSeasonChange();

                // Start the timer to check for season changes
                // In testing mode: check every 10 seconds, otherwise every 5 real minutes (1 UO hour)
                new SeasonCycleTimer().Start();

                // Register commands
                CommandSystem.Register("Season", AccessLevel.Player, Season_OnCommand);
                CommandSystem.Register("SetSeason", AccessLevel.GameMaster, SetSeason_OnCommand);
                CommandSystem.Register("SeasonInfo", AccessLevel.GameMaster, SeasonInfo_OnCommand);

                // Hook into world save
                EventSink.WorldSave += OnWorldSave;

                TimeSpan remaining = TimeUntilNextSeason;
                Utility.PushColor(ConsoleColor.Cyan);
                Console.WriteLine($"[SeasonCycle] Initialized - Current season: {CurrentSeasonName}");
                if (TestingMode)
                {
                    Console.WriteLine($"[SeasonCycle] TESTING MODE: Seasons change every {TestingSecondsPerSeason} seconds");
                    Console.WriteLine($"[SeasonCycle] Next season change in: {remaining.TotalSeconds:F0} seconds");
                }
                else
                {
                    Console.WriteLine($"[SeasonCycle] Next season change in: {remaining.Hours}h {remaining.Minutes}m (real time)");
                }
                Utility.PopColor();
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[SeasonCycle] Error during initialization: {ex}");
                Utility.PopColor();
            }
        }

        private static void OnWorldSave(WorldSaveEventArgs e)
        {
            Save();
        }

        [Usage("Season")]
        [Description("Shows the current season and time until next season change.")]
        private static void Season_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            TimeSpan remaining = TimeUntilNextSeason;

            // Get current UO date for display
            long totalMinutes = GetTotalUOMinutes();
            int uoDay = (int)((totalMinutes / 60 / 24) % 365) + 1;
            int uoYear = (int)(totalMinutes / 60 / 24 / 365);

            from.SendMessage(0x35, $"Current Season: {CurrentSeasonName}");
            from.SendMessage($"UO Date: Day {uoDay} of Year {uoYear}");

            if (remaining.TotalSeconds > 0)
            {
                if (remaining.TotalHours >= 1)
                    from.SendMessage($"Next season in: {(int)remaining.TotalHours}h {remaining.Minutes}m (real time)");
                else
                    from.SendMessage($"Next season in: {remaining.Minutes}m {remaining.Seconds}s (real time)");
            }
        }

        [Usage("SetSeason <spring|summer|fall|winter|0-3>")]
        [Description("Sets the current season for all cycling maps.")]
        private static void SetSeason_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Length < 1)
            {
                from.SendMessage("Usage: [SetSeason <spring|summer|fall|winter|0-3>");
                from.SendMessage($"Current season: {CurrentSeasonName}");
                return;
            }

            string arg = e.GetString(0).ToLower();
            int newSeason = -1;

            switch (arg)
            {
                case "spring":
                case "0":
                    newSeason = 0;
                    break;
                case "summer":
                case "1":
                    newSeason = 1;
                    break;
                case "fall":
                case "autumn":
                case "2":
                    newSeason = 2;
                    break;
                case "winter":
                case "3":
                    newSeason = 3;
                    break;
                default:
                    from.SendMessage("Invalid season. Use: spring, summer, fall, winter (or 0-3)");
                    return;
            }

            SetSeason(newSeason, true);
            from.SendMessage(0x35, $"Season changed to: {CurrentSeasonName}");
            from.SendMessage("All players on cycling maps will see the change.");
        }

        [Usage("SeasonInfo")]
        [Description("Shows detailed season cycle information.")]
        private static void SeasonInfo_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            long totalMinutes = GetTotalUOMinutes();
            int uoDay = (int)((totalMinutes / 60 / 24) % 365) + 1;
            int uoYear = (int)(totalMinutes / 60 / 24 / 365);
            int uoHour = (int)((totalMinutes / 60) % 24);
            int uoMinute = (int)(totalMinutes % 60);

            TimeSpan remaining = TimeUntilNextSeason;

            from.SendMessage(0x35, "=== Season Cycle Info ===");
            from.SendMessage($"Current Season: {CurrentSeasonName} ({_currentSeason})");
            from.SendMessage($"UO Time: {uoHour:D2}:{uoMinute:D2}, Day {uoDay}, Year {uoYear}");
            from.SendMessage($"Season Duration: {UOYearsPerSeason} UO years ({UODaysPerSeason} UO days)");
            from.SendMessage($"Real Time per Season: ~{SecondsPerSeason / 3600:F1} hours");

            if (remaining.TotalSeconds > 0)
                from.SendMessage($"Next Season In: {(int)remaining.TotalHours}h {remaining.Minutes}m {remaining.Seconds}s (real time)");
            else
                from.SendMessage("Season change pending...");

            from.SendMessage($"Cycling Maps: {string.Join(", ", CyclingMaps)}");
        }

        public static void SetSeason(int season, bool resetTimer = false)
        {
            if (season < 0 || season > 3)
                return;

            _currentSeason = season;

            if (resetTimer)
            {
                // Reset the season start time to now
                if (TestingMode)
                {
                    _seasonStartRealTime = DateTime.UtcNow;
                }
                else
                {
                    _seasonStartUOMinutes = GetTotalUOMinutes();
                }
            }

            ApplySeasonToMaps();
            Save();

            // Announce to all players
            foreach (NetState ns in NetState.Instances)
            {
                if (ns?.Mobile != null)
                {
                    ns.Mobile.SendMessage(0x35, $"The season has changed to {CurrentSeasonName}!");
                }
            }
        }

        private static void ApplySeasonToMaps()
        {
            foreach (int mapIndex in CyclingMaps)
            {
                Map map = Map.Maps[mapIndex];
                if (map != null)
                {
                    // Update the map's season
                    map.Season = _currentSeason;

                    // Send season update to all players on this map
                    foreach (NetState ns in NetState.Instances)
                    {
                        if (ns?.Mobile != null && ns.Mobile.Map == map)
                        {
                            ns.Send(SeasonChange.Instantiate(_currentSeason, true));
                            ns.Mobile.CheckLightLevels(true);
                        }
                    }
                }
            }

            Utility.PushColor(ConsoleColor.Green);
            Console.WriteLine($"[SeasonCycle] Applied {CurrentSeasonName} to {CyclingMaps.Count} maps");
            Utility.PopColor();
        }

        private static void CheckSeasonChange()
        {
            if (TestingMode)
            {
                // In testing mode, use real time
                TimeSpan elapsed = DateTime.UtcNow - _seasonStartRealTime;
                
                while (elapsed.TotalSeconds >= TestingSecondsPerSeason)
                {
                    // Advance to next season
                    int nextSeason = (_currentSeason + 1) % 4;

                    Utility.PushColor(ConsoleColor.Cyan);
                    Console.WriteLine($"[SeasonCycle] Season changing from {CurrentSeasonName} to {SeasonNames[nextSeason]}");
                    Utility.PopColor();

                    _currentSeason = nextSeason;
                    _seasonStartRealTime = DateTime.UtcNow; // Reset to now
                    elapsed = TimeSpan.Zero; // Reset elapsed for next check

                    ApplySeasonToMaps();

                    // Announce to all players
                    foreach (NetState ns in NetState.Instances)
                    {
                        if (ns?.Mobile != null)
                        {
                            ns.Mobile.SendMessage(0x35, $"The season has changed to {CurrentSeasonName}!");
                        }
                    }
                }
            }
            else
            {
                // Normal mode: use UO time
                long currentMinutes = GetTotalUOMinutes();
                long minutesPerSeason = (long)(UODaysPerSeason * 24 * 60);

                // Check if current time has passed the season end
                while (currentMinutes >= _seasonStartUOMinutes + minutesPerSeason)
                {
                    // Advance to next season
                    int nextSeason = (_currentSeason + 1) % 4;

                    Utility.PushColor(ConsoleColor.Cyan);
                    Console.WriteLine($"[SeasonCycle] Season changing from {CurrentSeasonName} to {SeasonNames[nextSeason]}");
                    Utility.PopColor();

                    _currentSeason = nextSeason;
                    _seasonStartUOMinutes += minutesPerSeason;

                    ApplySeasonToMaps();

                    // Announce to all players
                    foreach (NetState ns in NetState.Instances)
                    {
                        if (ns?.Mobile != null)
                        {
                            ns.Mobile.SendMessage(0x35, $"The season has changed to {CurrentSeasonName}!");
                        }
                    }
                }
            }

            Save();
        }

        private static void Save()
        {
            try
            {
                string dir = Path.GetDirectoryName(SavePath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                using (FileStream fs = new FileStream(SavePath, FileMode.Create, FileAccess.Write))
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    if (TestingMode)
                    {
                        writer.Write(3); // version 3 - testing mode (real time based)
                        writer.Write(_currentSeason);
                        writer.Write(_seasonStartRealTime.ToBinary());
                    }
                    else
                    {
                        writer.Write(2); // version 2 - UO time based
                        writer.Write(_currentSeason);
                        writer.Write(_seasonStartUOMinutes);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SeasonCycle] Error saving: {ex.Message}");
            }
        }

        private static void Load()
        {
            try
            {
                if (!File.Exists(SavePath))
                {
                    // First run - calculate initial season based on UO time
                    InitializeFromUOTime();
                    return;
                }

                using (FileStream fs = new FileStream(SavePath, FileMode.Open, FileAccess.Read))
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    int version = reader.ReadInt32();

                    if (version >= 3)
                    {
                        // Version 3: Testing mode (real time based)
                        _currentSeason = reader.ReadInt32();
                        long timeBinary = reader.ReadInt64();
                        _seasonStartRealTime = DateTime.FromBinary(timeBinary);
                    }
                    else if (version >= 2)
                    {
                        // Version 2: UO time based
                        _currentSeason = reader.ReadInt32();
                        _seasonStartUOMinutes = reader.ReadInt64();
                        
                        // If we're now in testing mode but loaded old data, reset timer
                        if (TestingMode)
                        {
                            _seasonStartRealTime = DateTime.UtcNow;
                        }
                    }
                    else
                    {
                        // Version 1: Old real-time based - migrate
                        _currentSeason = reader.ReadInt32();
                        // Ignore old DateTime, recalculate
                        InitializeFromUOTime();
                        return;
                    }

                    // Validate
                    if (_currentSeason < 0 || _currentSeason > 3)
                        _currentSeason = 0;
                }

                // Apply the loaded season to maps
                ApplySeasonToMaps();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SeasonCycle] Error loading: {ex.Message}");
                InitializeFromUOTime();
            }
        }

        private static void InitializeFromUOTime()
        {
            if (TestingMode)
            {
                // In testing mode, just start from Spring (0) with current time
                _currentSeason = 0;
                _seasonStartRealTime = DateTime.UtcNow;
                
                Utility.PushColor(ConsoleColor.Cyan);
                Console.WriteLine($"[SeasonCycle] Testing mode initialized - Season: {CurrentSeasonName} (changes every {TestingSecondsPerSeason} seconds)");
                Utility.PopColor();
            }
            else
            {
                // Calculate current season based on total UO time elapsed
                long totalMinutes = GetTotalUOMinutes();
                long minutesPerSeason = (long)(UODaysPerSeason * 24 * 60);
                long minutesPerYear = minutesPerSeason * 4; // 4 seasons per year

                // Figure out where we are in the yearly cycle
                long minutesIntoYear = totalMinutes % minutesPerYear;
                _currentSeason = (int)(minutesIntoYear / minutesPerSeason);

                // Calculate when this season started
                long seasonStartInYear = _currentSeason * minutesPerSeason;
                long yearsElapsed = totalMinutes / minutesPerYear;
                _seasonStartUOMinutes = (yearsElapsed * minutesPerYear) + seasonStartInYear;

                Utility.PushColor(ConsoleColor.Cyan);
                Console.WriteLine($"[SeasonCycle] Initialized from UO time - Season: {CurrentSeasonName}");
                Utility.PopColor();
            }

            if (_currentSeason < 0 || _currentSeason > 3)
                _currentSeason = 0;

            // Apply the season to maps
            ApplySeasonToMaps();
        }

        private class SeasonCycleTimer : Timer
        {
            public SeasonCycleTimer()
                : base(
                    TestingMode ? TimeSpan.FromSeconds(10) : TimeSpan.FromMinutes(5), // Check every 10 seconds in testing mode, 5 minutes otherwise
                    TestingMode ? TimeSpan.FromSeconds(10) : TimeSpan.FromMinutes(5))
            {
                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                CheckSeasonChange();
            }
        }
    }
}
