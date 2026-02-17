// Vystia Pilgrimage System
// Track shrine visits for pilgrimage bonuses

using System;
using System.Collections.Generic;
using System.IO;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.VystiaClasses.Religion
{
    #region Pilgrimage System

    /// <summary>
    /// Manages pilgrimage tracking for players
    /// Visiting all shrines of a religion grants bonus piety
    /// </summary>
    public static class VystiaPilgrimageSystem
    {
        // Shrine locations by religion
        private static readonly Dictionary<VystiaReligion, List<ShrineLocation>> s_ShrineLocations
            = new Dictionary<VystiaReligion, List<ShrineLocation>>();

        // Player pilgrimage progress
        private static readonly Dictionary<PlayerMobile, PilgrimageProgress> s_Progress
            = new Dictionary<PlayerMobile, PilgrimageProgress>();

        // Cooldown between pilgrimages (1 week)
        public static readonly TimeSpan PilgrimageCooldown = TimeSpan.FromDays(7);

        // Piety rewards
        public const int ShrineVisitPiety = 25;       // Per shrine visited
        public const int PilgrimageCompletePiety = 100; // Bonus for completing all shrines

        private static readonly string SavePath = Path.Combine(Core.BaseDirectory, "Saves/Vystia/Pilgrimages.bin");

        public static void Initialize()
        {
            EventSink.WorldSave += OnWorldSave;
            EventSink.WorldLoad += OnWorldLoad;

            CommandSystem.Register("PilgrimageStatus", AccessLevel.Player, PilgrimageStatus_OnCommand);
            CommandSystem.Register("PS", AccessLevel.Player, PilgrimageStatus_OnCommand);
            CommandSystem.Register("RegisterShrine", AccessLevel.GameMaster, RegisterShrine_OnCommand);
            CommandSystem.Register("ListShrines", AccessLevel.GameMaster, ListShrines_OnCommand);
            CommandSystem.Register("ResetPilgrimage", AccessLevel.GameMaster, ResetPilgrimage_OnCommand);

            InitializeDefaultShrines();

            Console.WriteLine("[Vystia] Pilgrimage System initialized.");
        }

        /// <summary>
        /// Initialize default shrine locations (can be overridden via commands)
        /// </summary>
        private static void InitializeDefaultShrines()
        {
            // These are placeholder locations - GMs should register actual shrine locations
            // Format: Religion -> List of shrine locations

            // Each religion has multiple shrines that must be visited
            foreach (VystiaReligion religion in Enum.GetValues(typeof(VystiaReligion)))
            {
                if (religion == VystiaReligion.None)
                    continue;

                if (!s_ShrineLocations.ContainsKey(religion))
                    s_ShrineLocations[religion] = new List<ShrineLocation>();
            }
        }

        #region Shrine Registration

        /// <summary>
        /// Register a shrine location for a religion
        /// </summary>
        public static void RegisterShrine(VystiaReligion religion, string name, Point3D location, Map map)
        {
            if (religion == VystiaReligion.None)
                return;

            if (!s_ShrineLocations.TryGetValue(religion, out var shrines))
            {
                shrines = new List<ShrineLocation>();
                s_ShrineLocations[religion] = shrines;
            }

            // Check if shrine already exists at this location
            foreach (var existing in shrines)
            {
                if (existing.Location == location && existing.Map == map)
                    return; // Already registered
            }

            shrines.Add(new ShrineLocation(name, location, map));
        }

        /// <summary>
        /// Get all shrine locations for a religion
        /// </summary>
        public static List<ShrineLocation> GetShrines(VystiaReligion religion)
        {
            if (s_ShrineLocations.TryGetValue(religion, out var shrines))
                return shrines;
            return new List<ShrineLocation>();
        }

        #endregion

        #region Pilgrimage Progress

        /// <summary>
        /// Get or create pilgrimage progress for a player
        /// </summary>
        public static PilgrimageProgress GetProgress(PlayerMobile pm)
        {
            if (pm == null)
                return null;

            if (!s_Progress.TryGetValue(pm, out var progress))
            {
                progress = new PilgrimageProgress();
                s_Progress[pm] = progress;
            }

            return progress;
        }

        /// <summary>
        /// Called when a player visits a shrine
        /// </summary>
        public static void OnShrineVisit(PlayerMobile pm, VystiaReligion religion, Point3D shrineLocation, Map shrineMap)
        {
            if (pm == null || religion == VystiaReligion.None)
                return;

            var progress = GetProgress(pm);
            var pietyData = VystiaPiety.GetPiety(pm);

            // Check if player follows this religion
            if (pietyData == null || pietyData.Religion != religion)
            {
                pm.SendMessage(0x22, "You do not follow this faith. This shrine holds no meaning for you.");
                return;
            }

            // Check pilgrimage cooldown
            if (!progress.CanStartPilgrimage(religion))
            {
                var remaining = progress.GetCooldownRemaining(religion);
                pm.SendMessage(0x22, "You have completed a pilgrimage recently. Return in {0:F1} days.", remaining.TotalDays);
                return;
            }

            // Find matching shrine
            var shrines = GetShrines(religion);
            ShrineLocation matchedShrine = null;

            foreach (var shrine in shrines)
            {
                if (shrine.Map == shrineMap && Utility.InRange(shrine.Location, shrineLocation, 5))
                {
                    matchedShrine = shrine;
                    break;
                }
            }

            if (matchedShrine == null)
            {
                // Not a registered pilgrimage shrine
                return;
            }

            // Check if already visited this shrine in current pilgrimage
            if (progress.HasVisited(religion, matchedShrine.Location))
            {
                pm.SendMessage(0x35, "You have already visited {0} on this pilgrimage.", matchedShrine.Name);
                return;
            }

            // Mark shrine as visited
            progress.MarkVisited(religion, matchedShrine.Location);

            // Award piety for visiting
            VystiaPiety.AddPiety(pm, ShrineVisitPiety, "pilgrimage shrine visit");

            pm.PlaySound(0x214);
            pm.FixedParticles(0x376A, 9, 32, 5005, 0, 0, EffectLayer.Waist);
            pm.SendMessage(0x35, "You have visited {0}. (+{1} piety)", matchedShrine.Name, ShrineVisitPiety);

            // Check if pilgrimage is complete
            int visited = progress.GetVisitedCount(religion);
            int total = shrines.Count;

            if (visited >= total && total > 0)
            {
                // Pilgrimage complete!
                CompletePilgrimage(pm, religion, progress);
            }
            else
            {
                pm.SendMessage(0x35, "Pilgrimage progress: {0}/{1} shrines visited.", visited, total);
            }
        }

        /// <summary>
        /// Complete a pilgrimage and award bonus
        /// </summary>
        private static void CompletePilgrimage(PlayerMobile pm, VystiaReligion religion, PilgrimageProgress progress)
        {
            // Award completion bonus
            VystiaPiety.AddPiety(pm, PilgrimageCompletePiety, "pilgrimage completion");

            // Mark pilgrimage complete (starts cooldown)
            progress.CompletePilgrimage(religion);

            // Effects
            pm.PlaySound(0x1F2);
            pm.FixedParticles(0x376A, 9, 32, 5030, 0x35, 0, EffectLayer.Waist);

            var religionInfo = ReligionData.GetInfo(religion);
            pm.SendMessage(0x35, "You have completed a sacred pilgrimage to all shrines of {0}!", religionInfo?.Name ?? religion.ToString());
            pm.SendMessage(0x35, "Bonus piety awarded: +{0}", PilgrimageCompletePiety);
        }

        /// <summary>
        /// Reset a player's pilgrimage progress (GM command)
        /// </summary>
        public static void ResetProgress(PlayerMobile pm, VystiaReligion religion)
        {
            var progress = GetProgress(pm);
            progress?.ResetPilgrimage(religion);
        }

        #endregion

        #region Commands

        [Usage("PilgrimageStatus")]
        [Description("Shows your current pilgrimage progress.")]
        private static void PilgrimageStatus_OnCommand(CommandEventArgs e)
        {
            if (!(e.Mobile is PlayerMobile pm))
                return;

            var pietyData = VystiaPiety.GetPiety(pm);
            if (pietyData == null || pietyData.Religion == VystiaReligion.None)
            {
                pm.SendMessage("You do not follow any religion.");
                return;
            }

            var religion = pietyData.Religion;
            var progress = GetProgress(pm);
            var shrines = GetShrines(religion);
            var religionInfo = ReligionData.GetInfo(religion);

            pm.SendMessage(0x35, "=== Pilgrimage Status for {0} ===", religionInfo?.Name ?? religion.ToString());

            if (shrines.Count == 0)
            {
                pm.SendMessage("No shrines have been registered for this religion.");
                return;
            }

            if (!progress.CanStartPilgrimage(religion))
            {
                var remaining = progress.GetCooldownRemaining(religion);
                pm.SendMessage(0x22, "Pilgrimage on cooldown. {0:F1} days remaining.", remaining.TotalDays);
            }

            int visited = progress.GetVisitedCount(religion);
            pm.SendMessage("Progress: {0}/{1} shrines visited", visited, shrines.Count);

            foreach (var shrine in shrines)
            {
                bool isVisited = progress.HasVisited(religion, shrine.Location);
                string status = isVisited ? "[Visited]" : "[Not Visited]";
                pm.SendMessage(isVisited ? 0x35 : 0x22, "  {0} - {1}", shrine.Name, status);
            }
        }

        [Usage("RegisterShrine <religion> <name>")]
        [Description("Registers a shrine at your current location for pilgrimage.")]
        private static void RegisterShrine_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Arguments.Length < 2)
            {
                from.SendMessage("Usage: [RegisterShrine <religion> <name>");
                from.SendMessage("Religions: FrosthelmFaith, SuryasSandscript, LunarasCovenant, CelestisArcanum, OceanasCovenant, CogsmithCreed");
                return;
            }

            string religionArg = e.Arguments[0].ToLower();
            string name = string.Join(" ", e.Arguments, 1, e.Arguments.Length - 1);

            VystiaReligion religion = VystiaReligion.None;

            switch (religionArg)
            {
                case "frostfathercult":
                case "frostfather":
                    religion = VystiaReligion.FrosthelmFaith;
                    break;
                case "emberheartorder":
                case "emberheart":
                    religion = VystiaReligion.SuryasSandscript;
                    break;
                case "greenwardcircle":
                case "greenward":
                    religion = VystiaReligion.LunarasCovenant;
                    break;
                case "crystallineascendancy":
                case "crystalline":
                    religion = VystiaReligion.CelestisArcanum;
                    break;
                case "voidwalkerpath":
                case "voidwalker":
                    religion = VystiaReligion.OceanasCovenant;
                    break;
                case "forgepact":
                case "forge":
                    religion = VystiaReligion.CogsmithCreed;
                    break;
            }

            if (religion == VystiaReligion.None)
            {
                from.SendMessage("Unknown religion: {0}", religionArg);
                return;
            }

            RegisterShrine(religion, name, from.Location, from.Map);
            from.SendMessage("Shrine '{0}' registered for {1} at ({2}, {3}, {4}).", name, religion, from.X, from.Y, from.Z);
        }

        [Usage("ListShrines [religion]")]
        [Description("Lists all registered pilgrimage shrines.")]
        private static void ListShrines_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            foreach (var kvp in s_ShrineLocations)
            {
                if (kvp.Value.Count == 0)
                    continue;

                from.SendMessage(0x35, "=== {0} ===", kvp.Key);
                foreach (var shrine in kvp.Value)
                {
                    from.SendMessage("  {0} - ({1}, {2}, {3}) [{4}]",
                        shrine.Name, shrine.Location.X, shrine.Location.Y, shrine.Location.Z, shrine.Map);
                }
            }
        }

        [Usage("ResetPilgrimage <player>")]
        [Description("Resets a player's pilgrimage progress and cooldown.")]
        private static void ResetPilgrimage_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Arguments.Length < 1)
            {
                from.SendMessage("Usage: [ResetPilgrimage <player>");
                return;
            }

            string playerName = e.Arguments[0];
            PlayerMobile target = null;

            foreach (Mobile m in World.Mobiles.Values)
            {
                if (m is PlayerMobile pm && pm.Name.ToLower() == playerName.ToLower())
                {
                    target = pm;
                    break;
                }
            }

            if (target == null)
            {
                from.SendMessage("Player not found: {0}", playerName);
                return;
            }

            var pietyData = VystiaPiety.GetPiety(target);
            if (pietyData?.Religion != VystiaReligion.None)
            {
                ResetProgress(target, pietyData.Religion);
                from.SendMessage("Reset pilgrimage progress for {0}.", target.Name);
            }
            else
            {
                from.SendMessage("{0} does not follow any religion.", target.Name);
            }
        }

        #endregion

        #region Save/Load

        private static void OnWorldSave(WorldSaveEventArgs e)
        {
            try
            {
                string dir = Path.GetDirectoryName(SavePath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                BinaryFileWriter writer = new BinaryFileWriter(SavePath, true);

                // Save shrine locations
                writer.Write(s_ShrineLocations.Count);
                foreach (var kvp in s_ShrineLocations)
                {
                    writer.Write((int)kvp.Key);
                    writer.Write(kvp.Value.Count);
                    foreach (var shrine in kvp.Value)
                    {
                        writer.Write(shrine.Name);
                        writer.Write(shrine.Location);
                        writer.Write(shrine.Map);
                    }
                }

                // Save player progress
                writer.Write(s_Progress.Count);
                foreach (var kvp in s_Progress)
                {
                    writer.Write(kvp.Key);
                    kvp.Value.Serialize(writer);
                }

                writer.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Vystia Pilgrimage] Error saving: {0}", ex.Message);
            }
        }

        private static void OnWorldLoad()
        {
            if (!File.Exists(SavePath))
                return;

            try
            {
                using (FileStream fs = new FileStream(SavePath, FileMode.Open))
                using (BinaryReader br = new BinaryReader(fs))
                {
                    GenericReader reader = new BinaryFileReader(br);

                    // Load shrine locations
                    int shrineCount = reader.ReadInt();
                    for (int i = 0; i < shrineCount; i++)
                    {
                        VystiaReligion religion = (VystiaReligion)reader.ReadInt();
                        int locationCount = reader.ReadInt();

                        if (!s_ShrineLocations.ContainsKey(religion))
                            s_ShrineLocations[religion] = new List<ShrineLocation>();

                        for (int j = 0; j < locationCount; j++)
                        {
                            string name = reader.ReadString();
                            Point3D location = reader.ReadPoint3D();
                            Map map = reader.ReadMap();
                            s_ShrineLocations[religion].Add(new ShrineLocation(name, location, map));
                        }
                    }

                    // Load player progress
                    int playerCount = reader.ReadInt();
                    for (int i = 0; i < playerCount; i++)
                    {
                        Mobile m = reader.ReadMobile();
                        var progress = new PilgrimageProgress();
                        progress.Deserialize(reader);

                        if (m is PlayerMobile pm)
                            s_Progress[pm] = progress;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Vystia Pilgrimage] Error loading: {0}", ex.Message);
            }
        }

        #endregion
    }

    #endregion

    #region Data Classes

    /// <summary>
    /// Represents a shrine location for pilgrimage
    /// </summary>
    public class ShrineLocation
    {
        public string Name { get; }
        public Point3D Location { get; }
        public Map Map { get; }

        public ShrineLocation(string name, Point3D location, Map map)
        {
            Name = name;
            Location = location;
            Map = map;
        }
    }

    /// <summary>
    /// Tracks pilgrimage progress for a player
    /// </summary>
    public class PilgrimageProgress
    {
        // Visited shrines by religion
        private readonly Dictionary<VystiaReligion, HashSet<Point3D>> m_VisitedShrines
            = new Dictionary<VystiaReligion, HashSet<Point3D>>();

        // Last pilgrimage completion by religion
        private readonly Dictionary<VystiaReligion, DateTime> m_LastCompletion
            = new Dictionary<VystiaReligion, DateTime>();

        public bool CanStartPilgrimage(VystiaReligion religion)
        {
            if (!m_LastCompletion.TryGetValue(religion, out var lastCompletion))
                return true;

            return DateTime.UtcNow - lastCompletion >= VystiaPilgrimageSystem.PilgrimageCooldown;
        }

        public TimeSpan GetCooldownRemaining(VystiaReligion religion)
        {
            if (!m_LastCompletion.TryGetValue(religion, out var lastCompletion))
                return TimeSpan.Zero;

            var elapsed = DateTime.UtcNow - lastCompletion;
            var remaining = VystiaPilgrimageSystem.PilgrimageCooldown - elapsed;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }

        public bool HasVisited(VystiaReligion religion, Point3D location)
        {
            if (!m_VisitedShrines.TryGetValue(religion, out var visited))
                return false;
            return visited.Contains(location);
        }

        public void MarkVisited(VystiaReligion religion, Point3D location)
        {
            if (!m_VisitedShrines.TryGetValue(religion, out var visited))
            {
                visited = new HashSet<Point3D>();
                m_VisitedShrines[religion] = visited;
            }
            visited.Add(location);
        }

        public int GetVisitedCount(VystiaReligion religion)
        {
            if (!m_VisitedShrines.TryGetValue(religion, out var visited))
                return 0;
            return visited.Count;
        }

        public void CompletePilgrimage(VystiaReligion religion)
        {
            m_LastCompletion[religion] = DateTime.UtcNow;
            // Clear visited shrines for next pilgrimage
            if (m_VisitedShrines.ContainsKey(religion))
                m_VisitedShrines[religion].Clear();
        }

        public void ResetPilgrimage(VystiaReligion religion)
        {
            m_LastCompletion.Remove(religion);
            if (m_VisitedShrines.ContainsKey(religion))
                m_VisitedShrines[religion].Clear();
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0); // version

            writer.Write(m_VisitedShrines.Count);
            foreach (var kvp in m_VisitedShrines)
            {
                writer.Write((int)kvp.Key);
                writer.Write(kvp.Value.Count);
                foreach (var loc in kvp.Value)
                    writer.Write(loc);
            }

            writer.Write(m_LastCompletion.Count);
            foreach (var kvp in m_LastCompletion)
            {
                writer.Write((int)kvp.Key);
                writer.Write(kvp.Value);
            }
        }

        public void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();

            int visitedCount = reader.ReadInt();
            for (int i = 0; i < visitedCount; i++)
            {
                VystiaReligion religion = (VystiaReligion)reader.ReadInt();
                int locCount = reader.ReadInt();
                var locs = new HashSet<Point3D>();
                for (int j = 0; j < locCount; j++)
                    locs.Add(reader.ReadPoint3D());
                m_VisitedShrines[religion] = locs;
            }

            int completionCount = reader.ReadInt();
            for (int i = 0; i < completionCount; i++)
            {
                VystiaReligion religion = (VystiaReligion)reader.ReadInt();
                DateTime time = reader.ReadDateTime();
                m_LastCompletion[religion] = time;
            }
        }
    }

    #endregion
}

