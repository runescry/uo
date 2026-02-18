using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Items;
using Server.Skills;
using Server.Gumps;
using Server.Network;
using Server.Commands;
using Server.Custom.VystiaClasses.Classes;

namespace Server.Skills
{
    /// <summary>
    /// Vystia Class System - Replaces Skill System with Vystia classes
    /// Maintains skill infrastructure while using Vystia class names and content
    /// </summary>
    public class VystiaClassSystem
    {
        private static readonly Dictionary<VystiaClass, VystiaClassDefinition> s_ClassDefinitions;
        private static readonly Dictionary<Mobile, VystiaClassPlayerData> s_PlayerData;
        private static readonly object s_Lock = new object();
        private static bool s_Initialized = false;

        static VystiaClassSystem()
        {
            s_ClassDefinitions = new Dictionary<VystiaClass, VystiaClassDefinition>();
            s_PlayerData = new Dictionary<Mobile, VystiaClassPlayerData>();
            InitializeClasses();
        }

        /// <summary>
        /// Initialize the Vystia Class System
        /// </summary>
        public static void Initialize()
        {
            if (s_Initialized)
                return;

            s_Initialized = true;

            // Register commands
            CommandSystem.Register("VystiaClass", AccessLevel.Player, VystiaClass_OnCommand);
            CommandSystem.Register("VC", AccessLevel.Player, VystiaClass_OnCommand);

            Console.WriteLine("[VystiaClassSystem] Initialized Vystia Class System");
            Console.WriteLine($"[VystiaClassSystem] Registered {s_ClassDefinitions.Count} Vystia classes");
        }

        /// <summary>
        /// Get class definition by class type
        /// </summary>
        public static VystiaClassDefinition GetClassDefinition(VystiaClass vystiaClass)
        {
            return s_ClassDefinitions.GetValueOrDefault(vystiaClass);
        }

        /// <summary>
        /// Get player class data
        /// </summary>
        public static VystiaClassPlayerData GetPlayerData(Mobile mobile)
        {
            if (mobile == null)
                return null;

            lock (s_Lock)
            {
                if (!s_PlayerData.ContainsKey(mobile))
                {
                    s_PlayerData[mobile] = new VystiaClassPlayerData(mobile);
                }

                return s_PlayerData[mobile];
            }
        }

        /// <summary>
        /// Get player's current class
        /// </summary>
        public static VystiaClass GetPlayerClass(Mobile mobile)
        {
            var playerData = GetPlayerData(mobile);
            return playerData?.Class ?? VystiaClass.None;
        }

        /// <summary>
        /// Set player's class
        /// </summary>
        public static bool SetPlayerClass(Mobile mobile, VystiaClass vystiaClass)
        {
            if (mobile == null || vystiaClass == VystiaClass.None)
                return false;

            lock (s_Lock)
            {
                var playerData = GetPlayerData(mobile);
                var oldClass = playerData.Class;

                if (oldClass != VystiaClass.None)
                {
                    // Leaving current class
                    mobile.SendMessage($"You have left the {oldClass} class!");
                    RemoveClassAbilities(mobile, oldClass);
                }

                playerData.Class = vystiaClass;
                playerData.Level = 1;
                playerData.Experience = 0;
                playerData.LastLevelUp = DateTime.MinValue;

                mobile.SendMessage($"You have joined the {vystiaClass} class!");
                mobile.SendMessage($"You are now level 1 with 0 experience!");

                // Add class abilities
                AddClassAbilities(mobile, vystiaClass);

                return true;
            }
        }

        /// <summary>
        /// Add experience to player's class
        /// </summary>
        public static bool AddExperience(Mobile from, int experience)
        {
            if (from == null || experience <= 0)
                return false;

            lock (s_Lock)
            {
                var playerData = GetPlayerData(from);
                if (playerData.Class == VystiaClass.None)
                {
                    from.SendMessage("You must join a class first!");
                    return false;
                }

                var oldLevel = playerData.Level;
                playerData.Experience += experience;

                // Check for level up
                var newLevel = CalculateLevel(playerData.Experience);
                if (newLevel > oldLevel)
                {
                    playerData.Level = newLevel;
                    playerData.LastLevelUp = DateTime.UtcNow;
                    
                    from.SendMessage($"Congratulations! You have reached level {newLevel} in {playerData.Class}!");
                    
                    // Grant level up rewards
                    GrantLevelUpRewards(from, playerData, newLevel);
                    
                    // Check for new abilities
                    UnlockNewAbilities(from, playerData, newLevel);
                }

                from.SendMessage($"You have gained {experience} experience in {playerData.Class}!");

                return true;
            }
        }

        /// <summary>
        /// Calculate level from experience
        /// </summary>
        private static int CalculateLevel(int experience)
        {
            // Experience formula: Level^2 * 1000
            return (int)Math.Sqrt(experience / 1000.0);
        }

        /// <summary>
        /// Get experience needed for next level
        /// </summary>
        public static int GetExperienceForNextLevel(Mobile mobile)
        {
            var playerData = GetPlayerData(mobile);
            if (playerData.Class == VystiaClass.None)
                return 0;

            var nextLevel = playerData.Level + 1;
            return nextLevel * nextLevel * 1000 - playerData.Experience;
        }

        /// <summary>
        /// Get class skill bonuses
        /// </summary>
        public static Dictionary<SkillName, double> GetClassSkillBonuses(Mobile mobile)
        {
            var bonuses = new Dictionary<SkillName, double>();
            
            var playerData = GetPlayerData(mobile);
            if (playerData.Class == VystiaClass.None)
                return bonuses;

            var classDef = GetClassDefinition(playerData.Class);
            if (classDef?.SkillBonuses != null)
            {
                foreach (var bonus in classDef.SkillBonuses)
                {
                    bonuses[bonus.Skill] = bonus.Bonus;
                }
            }

            return bonuses;
        }

        /// <summary>
        /// Check if player has class ability
        /// </summary>
        public static bool HasClassAbility(Mobile mobile, string abilityName)
        {
            var playerData = GetPlayerData(mobile);
            if (playerData.Class == VystiaClass.None)
                return false;

            return playerData.UnlockedAbilities.Contains(abilityName);
        }

        /// <summary>
        /// Handle class command
        /// </summary>
        private static void VystiaClass_OnCommand(CommandEventArgs e)
        {
            var from = e.Mobile as PlayerMobile;
            if (from == null)
                return;

            if (e.Length == 0)
            {
                ShowClassInfo(from);
                return;
            }

            switch (e.GetString(0).ToLower())
            {
                case "join":
                    JoinClass(from, e);
                    break;
                case "leave":
                    LeaveClass(from);
                    break;
                case "level":
                    ShowClassLevel(from);
                    break;
                case "abilities":
                    ShowClassAbilities(from);
                    break;
                case "info":
                    ShowClassInfo(from);
                    break;
                default:
                    ShowClassInfo(from);
                    break;
            }
        }

        /// <summary>
        /// Show class information
        /// </summary>
        private static void ShowClassInfo(Mobile from)
        {
            from.SendMessage("=== VYSTIA CLASS SYSTEM ===");
            from.SendMessage("Commands:");
            from.SendMessage("  [VystiaClass join <class>] - Join a class");
            from.SendMessage("  [VystiaClass leave] - Leave current class");
            from.SendMessage("  [VystiaClass level] - Show your class level");
            from.SendMessage("  [VystiaClass abilities] - Show class abilities");
            from.SendMessage("  [VystiaClass info] - Show this help");
            from.SendMessage("");
            from.SendMessage("Available Classes:");
            
            foreach (var vystiaClass in Enum.GetValues(typeof(VystiaClass)))
            {
                if (vystiaClass != VystiaClass.None)
                {
                    var def = GetClassDefinition(vystiaClass);
                    from.SendMessage($"  {vystiaClass} - {def?.Description}");
                }
            }
        }

        /// <summary>
        /// Join a class
        /// </summary>
        private static void JoinClass(Mobile from, CommandEventArgs e)
        {
            if (e.Length < 2)
            {
                from.SendMessage("Usage: [VystiaClass join <class>");
                return;
            }

            if (!Enum.TryParse<VystiaClass>(e.GetString(1), true, out var vystiaClass) || vystiaClass == VystiaClass.None)
            {
                from.SendMessage("Invalid class. Available classes:");
                foreach (var c in Enum.GetValues(typeof(VystiaClass)))
                {
                    if (c != VystiaClass.None)
                        from.SendMessage($"  {c}");
                }
                return;
            }

            SetPlayerClass(from, vystiaClass);
        }

        /// <summary>
        /// Leave current class
        /// </summary>
        private static void LeaveClass(Mobile from)
        {
            SetPlayerClass(from, VystiaClass.None);
        }

        /// <summary>
        /// Show class level
        /// </summary>
        private static void ShowClassLevel(Mobile from)
        {
            var playerData = GetPlayerData(from);
            if (playerData.Class == VystiaClass.None)
            {
                from.SendMessage("You are not in a class!");
                return;
            }

            from.SendMessage("=== CLASS STATUS ===");
            from.SendMessage($"Class: {playerData.Class}");
            from.SendMessage($"Level: {playerData.Level}");
            from.SendMessage($"Experience: {playerData.Experience}");
            from.SendMessage($"Next Level: {GetExperienceForNextLevel(from)}");
            from.SendMessage($"Last Level Up: {playerData.LastLevelUp:yyyy-MM-dd HH:mm:ss}");
        }

        /// </// </summary>
        /// Show class abilities
        /// </summary>
        private static void ShowClassAbilities(Mobile from)
        {
            var playerData = GetPlayerData(from);
            if (playerData.Class == VystiaClass.None)
            {
                from.SendMessage("You are not in a class!");
                return;
            }

            from.SendMessage($"=== {playerData.Class} ABILITIES ===");
            from.SendMessage($"Unlocked Abilities: {playerData.UnlockedAbilities.Count}");
            
            if (playerData.UnlockedAbilities.Count > 0)
            {
                from.SendMessage("Abilities:");
                foreach (var ability in playerData.UnlockedAbilities)
                {
                    from.SendMessage($"  • {ability}");
                }
            }
        }

        /// <summary>
        /// Add class abilities
        /// </summary>
        private static void AddClassAbilities(Mobile mobile, VystiaClass vystiaClass)
        {
            var classDef = GetClassDefinition(vystiaClass);
            if (classDef?.StartingAbilities != null)
            {
                foreach (var ability in classDef.StartingAbilities)
                {
                    if (!string.IsNullOrEmpty(ability))
                    {
                        var playerData = GetPlayerData(mobile);
                        playerData.UnlockedAbilities.Add(ability);
                        mobile.SendMessage($"You have unlocked the {ability} ability!");
                    }
                }
            }
        }

        /// <summary>
        /// Remove class abilities
        /// </summary>
        private static void RemoveClassAbilities(Mobile mobile, VystiaClass vystiaClass)
        {
            var playerData = GetPlayerData(mobile);
            playerData.UnlockedAbilities.Clear();
        }

        /// <summary>
        /// Grant level up rewards
        /// </summary>
        private static void GrantLevelUpRewards(Mobile mobile, VystiaClassPlayerData playerData, int newLevel)
        {
            // Grant level up rewards based on class
            switch (playerData.Class)
            {
                case VystiaClass.IceMage:
                    // Ice mage specific rewards
                    break;
                case VystiaClass.Necromancer:
                    // Necromancer specific rewards
                    break;
                case VystiaClass.Shaman:
                    // Shaman specific rewards
                    break;
                // Add other classes...
            }
        }

        /// <summary>
        /// Unlock new abilities
        /// </summary>
        private static void UnlockNewAbilities(Mobile mobile, VystiaClassPlayerData playerData, int newLevel)
        {
            var classDef = GetClassDefinition(playerData.Class);
            if (classDef?.LevelAbilities != null)
            {
                foreach (var levelAbility in classDef.LevelAbilities)
                {
                    if (levelAbility.Level == newLevel && !string.IsNullOrEmpty(levelAbility.Ability))
                    {
                        var playerDataLocked = GetPlayerData(mobile);
                        playerDataLocked.UnlockedAbilities.Add(levelAbility.Ability);
                        mobile.SendMessage($"You have unlocked the {levelAbility.Ability} ability at level {newLevel}!");
                    }
                }
            }
        }

        /// <summary>
        /// Initialize class definitions
        /// </summary>
        private static void InitializeClasses()
        {
            s_ClassDefinitions[VystiaClass.IceMage] = new VystiaClassDefinition
            {
                Class = VystiaClass.IceMage,
                Name = "Ice Mage",
                Description = "Master of ice and cold magic",
                Color = 1153, // Ice blue
                PrimarySkill = SkillName.Magery,
                SecondarySkills = new[] { SkillName.EvalIntelligence, SkillName.Meditation },
                StartingAbilities = new[] { "Ice Bolt", "Frost Shield", "Cold Resistance" },
                SkillBonuses = new[]
                {
                    new SkillBonus { Skill = SkillName.Magery, Bonus = 0.2 },
                    new SkillBonus { Skill = SkillName.EvalIntelligence, Bonus = 0.1 },
                    new SkillBonus { Skill = SkillName.ResistingCold, Bonus = 0.3 }
                },
                LevelAbilities = new[]
                {
                    new LevelAbility { Level = 5, Ability = "Ice Storm" },
                    new LevelAbility { Level = 10, Ability = "Blizzard" },
                    new LevelAbility { Level = 15, Ability = "Absolute Zero" }
                }
            };

            s_ClassDefinitions[VystiaClass.Necromancer] = new VystiaClassDefinition
            {
                Class = VystiaClass.Necromancer,
                Name = "Necromancer",
                Description = "Master of death and undeath magic",
                Color = 1175, // Dark purple
                PrimarySkill = SkillName.Necromancy,
                SecondarySkills = new[] { SkillName.SpiritSpeak, SkillName.MagicResist },
                StartingAbilities = new[] { "Animate Dead", "Vampiric Touch", "Undead Command" },
                SkillBonuses = new[]
                {
                    new SkillBonus { Skill = SkillName.Necromancy, Bonus = 0.2 },
                    new SkillBonus { Skill = SkillName.SpiritSpeak, Bonus = 0.1 },
                    new SkillBonus { Skill = SkillName.MagicResist, Bonus = 0.2 }
                },
                LevelAbilities = new[]
                {
                    new LevelAbility { Level = 5, Ability = "Summon Lich" },
                    new LevelAbility { Level = 10, Ability = "Necromancy Mastery" },
                    new LevelAbility { Level = 15, Ability = "Death Touch" }
                }
            };

            s_ClassDefinitions[VystiaClass.Shaman] = new VystiaClassDefinition
            {
                Class = VystiaClass.Shaman,
                Name = "Shaman",
                Description = "Master of nature and spirit magic",
                Color = 1273, // Forest green
                PrimarySkill = SkillName.SpiritSpeak,
                SecondarySkills = new[] { SkillName.AnimalTaming, SkillName.AnimalLore },
                StartingAbilities = new[] { "Spirit Walk", "Animal Companion", "Nature's Blessing" },
                SkillBonuses = new[]
                {
                    new SkillBonus { Skill = SkillName.SpiritSpeak, Bonus = 0.2 },
                    new SkillBonus { Skill = SkillName.AnimalTaming, Bonus = 0.1 },
                    new SkillBonus { Skill = SkillName.AnimalLore, Bonus = 0.1 }
                },
                LevelAbilities = new[]
                {
                    new LevelAbility { Level = 5, Ability = "Spirit Shield" },
                    new LevelAbility { Level = 10, Ability = "Animal Mastery" },
                    new LevelAbility { Level = 15, Ability = "Nature's Wrath" }
                }
            };

            s_ClassDefinitions[VystiaClass.Templar] = new VystiaClassDefinition
            {
                Class = VystiaClass.Templar,
                Name = "Templar",
                Description = "Holy warrior with divine protection",
                Color = 1152, // Holy gold
                PrimarySkill = SkillName.Chivalry,
                SecondarySkills = new[] { SkillName.Parrying, SkillName.MagicResist },
                StartingAbilities = new[] { "Divine Shield", "Holy Light", "Righteous Fury" },
                SkillBonuses = new[]
                {
                    new SkillBonus { Skill = SkillName.Chivalry, Bonus = 0.2 },
                    new Bonus { Skill = SkillName.Parrying, Bonus = 0.1 },
                    new SkillBonus { Skill = SkillName.MagicResist, Bonus = 0.2 }
                },
                LevelAbilities = new[]
                {
                    new LevelAbility { Level = 5, Ability = "Divine Intervention" },
                    new LevelAbility { Level = 10, Ability = "Holy Wrath" },
                    new LevelAbility { Level = 15, Ability = "Divine Protection" }
                }
            };

            // Add more classes...
        }

        /// <summary>
        /// Get all class definitions
        /// </summary>
        public static Dictionary<VystiaClass, VystiaClassDefinition> GetAllClasses()
        {
            return new Dictionary<VystiaClass, VystiaClassDefinition>(s_ClassDefinitions);
        }

        /// <summary>
        /// Get system statistics
        /// </summary>
        public static VystiaClassStatistics GetStatistics()
        {
            lock (s_Lock)
            {
                var stats = new VystiaClassStatistics
                {
                    TotalPlayers = s_PlayerData.Count,
                    ClassDistribution = new Dictionary<VystiaClass, int>(),
                    LevelDistribution = new Dictionary<int, int>(),
                    AverageLevel = 0,
                    LastUpdate = DateTime.UtcNow
                };

                // Count class distribution
                foreach (var playerData in s_PlayerData.Values)
                {
                    if (stats.ClassDistribution.ContainsKey(playerData.Class))
                        stats.ClassDistribution[playerData.Class]++;
                    else
                        stats.ClassDistribution[playerData.Class] = 1;

                    var level = playerData.Level;
                    if (stats.LevelDistribution.ContainsKey(level))
                        stats.LevelDistribution[level]++;
                    else
                        stats.LevelDistribution[level] = 1;
                }

                // Calculate average level
                if (stats.TotalPlayers > 0)
                {
                    stats.AverageLevel = s_PlayerData.Values.Average(pd => pd.Level);
                }

                return stats;
            }
        }
    }

    /// <summary>
    /// Vystia class definition
    /// </summary>
    public class VystiaClassDefinition
    {
        public VystiaClass Class { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Color { get; set; }
        public SkillName PrimarySkill { get; set; }
        public SkillName[] SecondarySkills { get; set; }
        public string[] StartingAbilities { get; set; }
        public SkillBonus[] SkillBonuses { get; set; }
        public LevelAbility[] LevelAbilities { get; set; }

        public VystiaClassDefinition()
        {
            SecondarySkills = new SkillName[0];
            StartingAbilities = new string[0];
            SkillBonuses = new SkillBonus[0];
            LevelAbilities = new LevelAbility[0];
        }
    }

    /// <summary>
    /// Skill bonus for Vystia class
    /// </summary>
    public class SkillBonus
    {
        public SkillName Skill { get; set; }
        public double Bonus { get; set; }
    }

    /// <summary>
    /// Level ability for Vystia class
    /// </summary>
    public class LevelAbility
    {
        public int Level { get; set; }
        public string Ability { get; set; }
    }

    /// <summary>
    /// Vystia class player data
    /// </summary>
    public class VystiaClassPlayerData
    {
        public Mobile Player { get; set; }
        public VystiaClass Class { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public DateTime LastLevelUp { get; set; }
        public HashSet<string> UnlockedAbilities { get; set; }

        public VystiaClassPlayerData(Mobile player)
        {
            Player = player;
            Class = VystiaClass.None;
            Level = 1;
            Experience = 0;
            LastLevelUp = DateTime.MinValue;
            UnlockedAbilities = new HashSet<string>();
        }
    }

    /// <summary>
    /// Vystia class statistics
    /// </summary>
    public class VystiaClassStatistics
    {
        public int TotalPlayers { get; set; }
        public Dictionary<VystiaClass, int> ClassDistribution { get; set; }
        public Dictionary<int, int> LevelDistribution { get; set; }
        public double AverageLevel { get; set; }
        public DateTime LastUpdate { get; set; }

        public VystiaClassStatistics()
        {
            ClassDistribution = new Dictionary<VystiaClass, int>();
            LevelDistribution = new Dictionary<int, int>();
        }
    }
}
