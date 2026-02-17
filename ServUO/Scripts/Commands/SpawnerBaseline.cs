using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server;

namespace Server.Commands
{
    /// <summary>
    /// Baseline configuration for Spawner attributes used in world creation/recreation/deletion scripts.
    /// These values can be based on code defaults OR calculated from current world state.
    /// </summary>
    public static class SpawnerBaseline
    {
        /// <summary>
        /// Default spawner configuration based on current decoration file attributes.
        /// These are the baseline values used when creating spawners through world generation scripts.
        /// </summary>
        public static class Defaults
        {
            /// <summary>
            /// Default minimum delay between spawns. Set via MinDelay parameter in decoration files.
            /// Default: 5 minutes (from Spawner constructor)
            /// </summary>
            public static TimeSpan MinDelay { get; set; } = TimeSpan.FromMinutes(5);

            /// <summary>
            /// Default maximum delay between spawns. Set via MaxDelay parameter in decoration files.
            /// Default: 10 minutes (from Spawner constructor)
            /// </summary>
            public static TimeSpan MaxDelay { get; set; } = TimeSpan.FromMinutes(10);

            /// <summary>
            /// Default next spawn time. Set via NextSpawn parameter in decoration files.
            /// Default: TimeSpan.Zero (immediate spawn when created via decoration)
            /// </summary>
            public static TimeSpan NextSpawn { get; set; } = TimeSpan.Zero;

            /// <summary>
            /// Default maximum count of spawned creatures. Set via Count parameter in decoration files.
            /// Maps to Spawner.MaxCount property.
            /// Default: 1 (from Spawner constructor)
            /// </summary>
            public static int MaxCount { get; set; } = 1;

            /// <summary>
            /// Default team number. Set via Team parameter in decoration files.
            /// Default: 0 (no team)
            /// </summary>
            public static int Team { get; set; } = 0;

            /// <summary>
            /// Default home range. Set via HomeRange parameter in decoration files.
            /// Default: 5 (from InitSpawner method)
            /// </summary>
            public static int HomeRange { get; set; } = 5;

            /// <summary>
            /// Default spawn range. Used in Spawner constructor.
            /// Default: 4 (from Spawner constructor)
            /// </summary>
            public static int SpawnRange { get; set; } = 4;

            /// <summary>
            /// Default walking range. Set in InitSpawner method.
            /// Default: -1 (uses HomeRange instead)
            /// </summary>
            public static int WalkingRange { get; set; } = -1;

            /// <summary>
            /// Default running state. Set via Running parameter in decoration files.
            /// Default: true (from InitSpawner method)
            /// </summary>
            public static bool Running { get; set; } = true;

            /// <summary>
            /// Default group spawn mode. Set via Group parameter in decoration files.
            /// When true, spawns all creatures at once; when false, spawns individually.
            /// Default: false (from InitSpawner method)
            /// </summary>
            public static bool Group { get; set; } = false;

            /// <summary>
            /// Default guard immune flag. Not currently set via decoration files, but available.
            /// Default: false
            /// </summary>
            public static bool GuardImmune { get; set; } = false;
        }

        /// <summary>
        /// Applies baseline configuration to a spawner instance.
        /// This ensures all spawners created through world generation scripts use consistent baseline values.
        /// </summary>
        /// <param name="spawner">The spawner to configure</param>
        public static void ApplyBaseline(Spawner spawner)
        {
            if (spawner == null)
                return;

            spawner.MinDelay = Defaults.MinDelay;
            spawner.MaxDelay = Defaults.MaxDelay;
            spawner.NextSpawn = Defaults.NextSpawn;
            spawner.MaxCount = Defaults.MaxCount;
            spawner.Team = Defaults.Team;
            spawner.HomeRange = Defaults.HomeRange;
            spawner.SpawnRange = Defaults.SpawnRange;
            spawner.WalkingRange = Defaults.WalkingRange;
            spawner.Running = Defaults.Running;
            spawner.Group = Defaults.Group;
            spawner.GuardImmune = Defaults.GuardImmune;
        }

        /// <summary>
        /// Gets a summary of all baseline attributes for documentation purposes.
        /// </summary>
        public static string GetBaselineSummary()
        {
            return string.Format(
                "Spawner Baseline Configuration:\n" +
                "  MinDelay: {0}\n" +
                "  MaxDelay: {1}\n" +
                "  NextSpawn: {2}\n" +
                "  MaxCount: {3}\n" +
                "  Team: {4}\n" +
                "  HomeRange: {5}\n" +
                "  SpawnRange: {6}\n" +
                "  WalkingRange: {7}\n" +
                "  Running: {8}\n" +
                "  Group: {9}\n" +
                "  GuardImmune: {10}",
                Defaults.MinDelay,
                Defaults.MaxDelay,
                Defaults.NextSpawn,
                Defaults.MaxCount,
                Defaults.Team,
                Defaults.HomeRange,
                Defaults.SpawnRange,
                Defaults.WalkingRange,
                Defaults.Running,
                Defaults.Group,
                Defaults.GuardImmune
            );
        }

        /// <summary>
        /// Scans the current world state to calculate baseline values from existing spawners.
        /// This provides actual values from spawners currently in the world rather than code defaults.
        /// </summary>
        /// <param name="useMode">Determines how to calculate baseline: Mode (most common), Median, or Mean</param>
        /// <returns>Statistics about the scan including count of spawners analyzed</returns>
        public static ScanResult CalculateFromWorldState(CalculationMode useMode = CalculationMode.Mode)
        {
            var spawners = new List<Spawner>();
            var stats = new ScanResult();

            // Collect all Spawner instances from the world
            foreach (Item item in World.Items.Values)
            {
                if (item is Spawner spawner && !spawner.Deleted)
                {
                    spawners.Add(spawner);
                }
            }

            stats.TotalSpawners = spawners.Count;

            if (spawners.Count == 0)
            {
                return stats;
            }

            // Calculate baseline values based on mode
            switch (useMode)
            {
                case CalculationMode.Mode:
                    CalculateModeBaseline(spawners, stats);
                    break;
                case CalculationMode.Median:
                    CalculateMedianBaseline(spawners, stats);
                    break;
                case CalculationMode.Mean:
                    CalculateMeanBaseline(spawners, stats);
                    break;
            }

            return stats;
        }

        /// <summary>
        /// Applies baseline values calculated from world state to the Defaults.
        /// </summary>
        /// <param name="result">Scan result containing calculated baseline values</param>
        public static void ApplyWorldStateBaseline(ScanResult result)
        {
            if (result == null || !result.IsValid)
                return;

            Defaults.MinDelay = result.MinDelay;
            Defaults.MaxDelay = result.MaxDelay;
            Defaults.MaxCount = result.MaxCount;
            Defaults.Team = result.Team;
            Defaults.HomeRange = result.HomeRange;
            Defaults.SpawnRange = result.SpawnRange;
            Defaults.WalkingRange = result.WalkingRange;
            Defaults.Running = result.Running;
            Defaults.Group = result.Group;
            Defaults.GuardImmune = result.GuardImmune;
        }

        private static void CalculateModeBaseline(List<Spawner> spawners, ScanResult stats)
        {
            // MinDelay - find most common value
            var minDelays = spawners.Select(s => s.MinDelay).ToList();
            stats.MinDelay = GetMode(minDelays);

            // MaxDelay - find most common value
            var maxDelays = spawners.Select(s => s.MaxDelay).ToList();
            stats.MaxDelay = GetMode(maxDelays);

            // MaxCount - find most common value
            var maxCounts = spawners.Select(s => s.MaxCount).ToList();
            stats.MaxCount = GetMode(maxCounts);

            // Team - find most common value
            var teams = spawners.Select(s => s.Team).ToList();
            stats.Team = GetMode(teams);

            // HomeRange - find most common value
            var homeRanges = spawners.Select(s => s.HomeRange).ToList();
            stats.HomeRange = GetMode(homeRanges);

            // SpawnRange - find most common value
            var spawnRanges = spawners.Select(s => s.SpawnRange).ToList();
            stats.SpawnRange = GetMode(spawnRanges);

            // WalkingRange - find most common value
            var walkingRanges = spawners.Select(s => s.WalkingRange).ToList();
            stats.WalkingRange = GetMode(walkingRanges);

            // Running - find most common value (boolean)
            var running = spawners.Select(s => s.Running).ToList();
            stats.Running = GetMode(running);

            // Group - find most common value (boolean)
            var groups = spawners.Select(s => s.Group).ToList();
            stats.Group = GetMode(groups);

            // GuardImmune - find most common value (boolean)
            var guardImmune = spawners.Select(s => s.GuardImmune).ToList();
            stats.GuardImmune = GetMode(guardImmune);
        }

        private static void CalculateMedianBaseline(List<Spawner> spawners, ScanResult stats)
        {
            var sorted = spawners.OrderBy(s => s.MinDelay.TotalMinutes).ToList();
            stats.MinDelay = sorted[sorted.Count / 2].MinDelay;

            sorted = spawners.OrderBy(s => s.MaxDelay.TotalMinutes).ToList();
            stats.MaxDelay = sorted[sorted.Count / 2].MaxDelay;

            sorted = spawners.OrderBy(s => s.MaxCount).ToList();
            stats.MaxCount = sorted[sorted.Count / 2].MaxCount;

            sorted = spawners.OrderBy(s => s.Team).ToList();
            stats.Team = sorted[sorted.Count / 2].Team;

            sorted = spawners.OrderBy(s => s.HomeRange).ToList();
            stats.HomeRange = sorted[sorted.Count / 2].HomeRange;

            sorted = spawners.OrderBy(s => s.SpawnRange).ToList();
            stats.SpawnRange = sorted[sorted.Count / 2].SpawnRange;

            sorted = spawners.OrderBy(s => s.WalkingRange).ToList();
            stats.WalkingRange = sorted[sorted.Count / 2].WalkingRange;

            // For booleans, use mode
            var running = spawners.Select(s => s.Running).ToList();
            stats.Running = GetMode(running);

            var groups = spawners.Select(s => s.Group).ToList();
            stats.Group = GetMode(groups);

            var guardImmune = spawners.Select(s => s.GuardImmune).ToList();
            stats.GuardImmune = GetMode(guardImmune);
        }

        private static void CalculateMeanBaseline(List<Spawner> spawners, ScanResult stats)
        {
            stats.MinDelay = TimeSpan.FromMinutes(spawners.Average(s => s.MinDelay.TotalMinutes));
            stats.MaxDelay = TimeSpan.FromMinutes(spawners.Average(s => s.MaxDelay.TotalMinutes));
            stats.MaxCount = (int)Math.Round(spawners.Average(s => s.MaxCount));
            stats.Team = (int)Math.Round(spawners.Average(s => s.Team));
            stats.HomeRange = (int)Math.Round(spawners.Average(s => s.HomeRange));
            stats.SpawnRange = (int)Math.Round(spawners.Average(s => s.SpawnRange));
            stats.WalkingRange = (int)Math.Round(spawners.Average(s => s.WalkingRange));

            // For booleans, use mode
            var running = spawners.Select(s => s.Running).ToList();
            stats.Running = GetMode(running);

            var groups = spawners.Select(s => s.Group).ToList();
            stats.Group = GetMode(groups);

            var guardImmune = spawners.Select(s => s.GuardImmune).ToList();
            stats.GuardImmune = GetMode(guardImmune);
        }

        private static T GetMode<T>(List<T> values) where T : IComparable
        {
            if (values.Count == 0)
                return default(T);

            var groups = values.GroupBy(v => v).OrderByDescending(g => g.Count());
            return groups.First().Key;
        }

        /// <summary>
        /// Calculation mode for determining baseline from world state.
        /// </summary>
        public enum CalculationMode
        {
            /// <summary>Use the most common value (mode)</summary>
            Mode,
            /// <summary>Use the median value</summary>
            Median,
            /// <summary>Use the mean/average value</summary>
            Mean
        }

        /// <summary>
        /// Generates C# code that can be pasted into the Defaults class to update code defaults.
        /// </summary>
        /// <param name="result">Scan result containing calculated baseline values</param>
        /// <returns>C# code string with property assignments</returns>
        public static string GenerateCodeFromResult(ScanResult result)
        {
            if (result == null || !result.IsValid)
                return "// Invalid scan result";

            return string.Format(
                "            // Generated from world state scan ({0} spawners, {1})\n" +
                "            // Paste these values into SpawnerBaseline.Defaults class\n\n" +
                "            public static TimeSpan MinDelay {{ get; set; }} = TimeSpan.FromMinutes({2});\n" +
                "            public static TimeSpan MaxDelay {{ get; set; }} = TimeSpan.FromMinutes({3});\n" +
                "            public static TimeSpan NextSpawn {{ get; set; }} = TimeSpan.Zero;\n" +
                "            public static int MaxCount {{ get; set; }} = {4};\n" +
                "            public static int Team {{ get; set; }} = {5};\n" +
                "            public static int HomeRange {{ get; set; }} = {6};\n" +
                "            public static int SpawnRange {{ get; set; }} = {7};\n" +
                "            public static int WalkingRange {{ get; set; }} = {8};\n" +
                "            public static bool Running {{ get; set; }} = {9};\n" +
                "            public static bool Group {{ get; set; }} = {10};\n" +
                "            public static bool GuardImmune {{ get; set; }} = {11};",
                result.TotalSpawners,
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                result.MinDelay.TotalMinutes,
                result.MaxDelay.TotalMinutes,
                result.MaxCount,
                result.Team,
                result.HomeRange,
                result.SpawnRange,
                result.WalkingRange,
                result.Running.ToString().ToLower(),
                result.Group.ToString().ToLower(),
                result.GuardImmune.ToString().ToLower()
            );
        }

        /// <summary>
        /// Result of scanning world state for baseline values.
        /// </summary>
        public class ScanResult
        {
            public int TotalSpawners { get; set; }
            public TimeSpan MinDelay { get; set; }
            public TimeSpan MaxDelay { get; set; }
            public int MaxCount { get; set; }
            public int Team { get; set; }
            public int HomeRange { get; set; }
            public int SpawnRange { get; set; }
            public int WalkingRange { get; set; }
            public bool Running { get; set; }
            public bool Group { get; set; }
            public bool GuardImmune { get; set; }

            public bool IsValid => TotalSpawners > 0;

            public override string ToString()
            {
                return string.Format(
                    "World State Baseline (from {0} spawners):\n" +
                    "  MinDelay: {1}\n" +
                    "  MaxDelay: {2}\n" +
                    "  MaxCount: {3}\n" +
                    "  Team: {4}\n" +
                    "  HomeRange: {5}\n" +
                    "  SpawnRange: {6}\n" +
                    "  WalkingRange: {7}\n" +
                    "  Running: {8}\n" +
                    "  Group: {9}\n" +
                    "  GuardImmune: {10}",
                    TotalSpawners,
                    MinDelay,
                    MaxDelay,
                    MaxCount,
                    Team,
                    HomeRange,
                    SpawnRange,
                    WalkingRange,
                    Running,
                    Group,
                    GuardImmune
                );
            }
        }
    }
}

