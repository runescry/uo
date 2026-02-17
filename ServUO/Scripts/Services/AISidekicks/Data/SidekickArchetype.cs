using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Services.LLM;

namespace Server.Services.AISidekicks
{
    /// <summary>
    /// Defines the available sidekick archetypes
    /// </summary>
    public enum SidekickArchetypeType
    {
        Warrior,
        Mage,
        Archer,
        Healer,
        Paladin,
        Ranger,
        Thief,
        Necromancer,
        Battlemage,
        Cleric,
        Druid,
        Tamer
    }

    /// <summary>
    /// Combat style for sidekicks
    /// </summary>
    public enum CombatStyle
    {
        Melee,      // Close combat
        Archer,     // Ranged combat
        Mage,       // Spell casting
        Hybrid      // Combination
    }

    /// <summary>
    /// Starting skill definition
    /// </summary>
    public struct StartingSkill
    {
        public SkillName SkillName;
        public double Value;

        public StartingSkill(SkillName skillName, double value)
        {
            SkillName = skillName;
            Value = value;
        }
    }

    /// <summary>
    /// Stat locks for growth direction
    /// </summary>
    public struct StatLocks
    {
        public StatLockType StrLock;
        public StatLockType DexLock;
        public StatLockType IntLock;

        public StatLocks(StatLockType strLock, StatLockType dexLock, StatLockType intLock)
        {
            StrLock = strLock;
            DexLock = dexLock;
            IntLock = intLock;
        }
    }

    /// <summary>
    /// Complete archetype definition with starting stats, skills, personality, and combat style
    /// </summary>
    public class SidekickArchetype
    {
        public SidekickArchetypeType Type { get; set; }
        public string Name { get; set; }
        public int StartingStr { get; set; }
        public int StartingDex { get; set; }
        public int StartingInt { get; set; }
        public List<StartingSkill> StartingSkills { get; set; }
        public StatLocks StatLocks { get; set; }
        public NPCPersonalities.PersonalityType PersonalityType { get; set; }
        public NPCPersonalities.SpeechPattern SpeechPattern { get; set; }
        public CombatStyle CombatStyle { get; set; }
        public AIType BaseAIType { get; set; }
        public int OptimalCombatRange { get; set; }

        public SidekickArchetype()
        {
            StartingSkills = new List<StartingSkill>();
        }

        /// <summary>
        /// Get archetype definition by type
        /// </summary>
        public static SidekickArchetype GetArchetype(SidekickArchetypeType type)
        {
            switch (type)
            {
                case SidekickArchetypeType.Warrior:
                    return CreateWarrior();
                case SidekickArchetypeType.Mage:
                    return CreateMage();
                case SidekickArchetypeType.Archer:
                    return CreateArcher();
                case SidekickArchetypeType.Healer:
                    return CreateHealer();
                case SidekickArchetypeType.Paladin:
                    return CreatePaladin();
                case SidekickArchetypeType.Ranger:
                    return CreateRanger();
                case SidekickArchetypeType.Thief:
                    return CreateThief();
                case SidekickArchetypeType.Necromancer:
                    return CreateNecromancer();
                case SidekickArchetypeType.Battlemage:
                    return CreateBattlemage();
                case SidekickArchetypeType.Cleric:
                    return CreateCleric();
                case SidekickArchetypeType.Druid:
                    return CreateDruid();
                case SidekickArchetypeType.Tamer:
                    return CreateTamer();
                default:
                    return CreateWarrior(); // Default fallback
            }
        }

        /// <summary>
        /// Get archetype by name (case-insensitive)
        /// </summary>
        public static SidekickArchetype GetArchetypeByName(string name)
        {
            if (Enum.TryParse<SidekickArchetypeType>(name, true, out SidekickArchetypeType type))
            {
                return GetArchetype(type);
            }
            return null;
        }

        #region Archetype Definitions

        private static SidekickArchetype CreateWarrior()
        {
            // T2A Lumberjack Build - 700 skill points, 225 stats
            return new SidekickArchetype
            {
                Type = SidekickArchetypeType.Warrior,
                Name = "Warrior",
                StartingStr = 100,  // Max for melee damage
                StartingDex = 100,  // Max for swing speed
                StartingInt = 25,   // Minimal for light magic
                StartingSkills = new List<StartingSkill>
                {
                    // Combat core (Lumberjacking bonus with axes!)
                    new StartingSkill(SkillName.Lumberjacking, 100.0), // +damage with axes
                    new StartingSkill(SkillName.Tactics, 100.0),       // +damage
                    new StartingSkill(SkillName.Anatomy, 100.0),       // +damage + heal bonus
                    new StartingSkill(SkillName.Swords, 100.0),        // Axes use Swords skill
                    // Defense & utility
                    new StartingSkill(SkillName.Healing, 100.0),       // Bandage healing
                    new StartingSkill(SkillName.MagicResist, 100.0),   // Magic defense
                    // Light magic for utility
                    new StartingSkill(SkillName.Magery, 75.0),         // Emergency spells
                    new StartingSkill(SkillName.Meditation, 25.0)      // Mana regen
                },
                StatLocks = new StatLocks(StatLockType.Up, StatLockType.Up, StatLockType.Down),
                PersonalityType = NPCPersonalities.PersonalityType.Warrior,
                SpeechPattern = NPCPersonalities.SpeechPattern.Archaic,
                CombatStyle = CombatStyle.Melee,
                BaseAIType = AIType.AI_Melee,
                OptimalCombatRange = 1
            };
        }

        private static SidekickArchetype CreateMage()
        {
            // T2A Pure Mage Build - 700 skill points, 225 stats
            return new SidekickArchetype
            {
                Type = SidekickArchetypeType.Mage,
                Name = "Mage",
                StartingStr = 80,   // HP pool (Str = HP)
                StartingDex = 45,   // Casting speed
                StartingInt = 100,  // Max mana pool
                StartingSkills = new List<StartingSkill>
                {
                    // Magic core
                    new StartingSkill(SkillName.Magery, 100.0),        // Spell casting
                    new StartingSkill(SkillName.EvalInt, 100.0),       // Spell damage
                    new StartingSkill(SkillName.Meditation, 100.0),    // Mana regen
                    new StartingSkill(SkillName.MagicResist, 100.0),   // Magic defense
                    // Defense & utility
                    new StartingSkill(SkillName.Wrestling, 100.0),     // Melee defense
                    new StartingSkill(SkillName.Anatomy, 100.0),       // Heal bonus
                    new StartingSkill(SkillName.Healing, 100.0)        // Bandage healing
                },
                StatLocks = new StatLocks(StatLockType.Locked, StatLockType.Locked, StatLockType.Up),
                PersonalityType = NPCPersonalities.PersonalityType.Mage,
                SpeechPattern = NPCPersonalities.SpeechPattern.Formal,
                CombatStyle = CombatStyle.Mage,
                BaseAIType = AIType.AI_Mage,
                OptimalCombatRange = 10
            };
        }

        private static SidekickArchetype CreateArcher()
        {
            // T2A Archer Build - 700 skill points, 225 stats
            return new SidekickArchetype
            {
                Type = SidekickArchetypeType.Archer,
                Name = "Archer",
                StartingStr = 80,   // Bow damage
                StartingDex = 100,  // Max archery speed
                StartingInt = 45,   // Minimal
                StartingSkills = new List<StartingSkill>
                {
                    // Combat core
                    new StartingSkill(SkillName.Archery, 100.0),       // Bow/Crossbow
                    new StartingSkill(SkillName.Tactics, 100.0),       // +damage
                    new StartingSkill(SkillName.Anatomy, 100.0),       // +damage + heal bonus
                    // Defense & utility
                    new StartingSkill(SkillName.Healing, 100.0),       // Bandage healing
                    new StartingSkill(SkillName.MagicResist, 100.0),   // Magic defense
                    new StartingSkill(SkillName.Hiding, 100.0),        // Stealth ambush
                    new StartingSkill(SkillName.Tracking, 100.0)       // Find targets
                },
                StatLocks = new StatLocks(StatLockType.Locked, StatLockType.Up, StatLockType.Locked),
                PersonalityType = NPCPersonalities.PersonalityType.Ranger,
                SpeechPattern = NPCPersonalities.SpeechPattern.Casual,
                CombatStyle = CombatStyle.Archer,
                BaseAIType = AIType.AI_Archer,
                OptimalCombatRange = 5
            };
        }

        private static SidekickArchetype CreateHealer()
        {
            // T2A Healer/Support Mage Build - 700 skill points, 225 stats
            return new SidekickArchetype
            {
                Type = SidekickArchetypeType.Healer,
                Name = "Healer",
                StartingStr = 80,   // HP pool (Str = HP)
                StartingDex = 45,   // Casting speed
                StartingInt = 100,  // Max mana for heals
                StartingSkills = new List<StartingSkill>
                {
                    // Healing core
                    new StartingSkill(SkillName.Magery, 100.0),        // Heal spells
                    new StartingSkill(SkillName.Veterinary, 100.0),    // Heal pets/creatures
                    new StartingSkill(SkillName.Healing, 100.0),       // Bandage healing (players)
                    new StartingSkill(SkillName.Anatomy, 100.0),       // +heal bonus (players)
                    // Defense & utility
                    new StartingSkill(SkillName.MagicResist, 100.0),   // Magic defense
                    new StartingSkill(SkillName.Meditation, 100.0),    // Mana regen
                    new StartingSkill(SkillName.AnimalLore, 100.0)     // +pet heal bonus
                },
                StatLocks = new StatLocks(StatLockType.Locked, StatLockType.Locked, StatLockType.Up),
                PersonalityType = NPCPersonalities.PersonalityType.Healer,
                SpeechPattern = NPCPersonalities.SpeechPattern.Casual,
                CombatStyle = CombatStyle.Mage,
                BaseAIType = AIType.AI_Healer,
                OptimalCombatRange = 8
            };
        }

        private static SidekickArchetype CreatePaladin()
        {
            // T2A Paladin Build - 700 skill points, 225 stats
            return new SidekickArchetype
            {
                Type = SidekickArchetypeType.Paladin,
                Name = "Paladin",
                StartingStr = 100,  // Max melee damage
                StartingDex = 100,  // Swing speed
                StartingInt = 25,   // Chivalry mana
                StartingSkills = new List<StartingSkill>
                {
                    // Combat core
                    new StartingSkill(SkillName.Swords, 100.0),        // Primary weapon
                    new StartingSkill(SkillName.Tactics, 100.0),       // +damage
                    new StartingSkill(SkillName.Anatomy, 100.0),       // +damage + heal bonus
                    new StartingSkill(SkillName.Parry, 100.0),      // Shield blocking
                    // Defense & utility
                    new StartingSkill(SkillName.MagicResist, 100.0),   // Magic defense
                    new StartingSkill(SkillName.Healing, 100.0),       // Bandage healing
                    new StartingSkill(SkillName.Chivalry, 100.0)       // Paladin abilities
                },
                StatLocks = new StatLocks(StatLockType.Up, StatLockType.Locked, StatLockType.Up),
                PersonalityType = NPCPersonalities.PersonalityType.Paladin,
                SpeechPattern = NPCPersonalities.SpeechPattern.Formal,
                CombatStyle = CombatStyle.Melee,
                BaseAIType = AIType.AI_Paladin,
                OptimalCombatRange = 1
            };
        }

        private static SidekickArchetype CreateRanger()
        {
            // T2A Ranger Build - 700 skill points, 225 stats
            return new SidekickArchetype
            {
                Type = SidekickArchetypeType.Ranger,
                Name = "Ranger",
                StartingStr = 80,   // Bow damage
                StartingDex = 100,  // Max archery speed
                StartingInt = 45,   // Minimal
                StartingSkills = new List<StartingSkill>
                {
                    // Combat core
                    new StartingSkill(SkillName.Archery, 100.0),       // Ranged weapon
                    new StartingSkill(SkillName.Tactics, 100.0),       // +damage
                    new StartingSkill(SkillName.Anatomy, 100.0),       // +damage + heal bonus
                    // Defense & utility
                    new StartingSkill(SkillName.Healing, 100.0),       // Bandage healing
                    new StartingSkill(SkillName.MagicResist, 100.0),   // Magic defense
                    new StartingSkill(SkillName.Tracking, 100.0),      // Find targets
                    new StartingSkill(SkillName.AnimalLore, 100.0)     // Beast knowledge
                },
                StatLocks = new StatLocks(StatLockType.Locked, StatLockType.Up, StatLockType.Locked),
                PersonalityType = NPCPersonalities.PersonalityType.Ranger,
                SpeechPattern = NPCPersonalities.SpeechPattern.Casual,
                CombatStyle = CombatStyle.Archer,
                BaseAIType = AIType.AI_Archer,
                OptimalCombatRange = 5
            };
        }

        private static SidekickArchetype CreateThief()
        {
            // T2A Thief/Assassin Build - 700 skill points, 225 stats
            return new SidekickArchetype
            {
                Type = SidekickArchetypeType.Thief,
                Name = "Thief",
                StartingStr = 75,   // Backstab damage
                StartingDex = 100,  // Max stealth/speed
                StartingInt = 50,   // Utility
                StartingSkills = new List<StartingSkill>
                {
                    // Combat core
                    new StartingSkill(SkillName.Fencing, 100.0),       // Dagger weapon
                    new StartingSkill(SkillName.Tactics, 100.0),       // +damage
                    new StartingSkill(SkillName.Anatomy, 100.0),       // +damage + heal bonus
                    // Stealth skills
                    new StartingSkill(SkillName.Hiding, 100.0),        // Go invisible
                    new StartingSkill(SkillName.Stealth, 100.0),       // Move while hidden
                    // Defense & utility
                    new StartingSkill(SkillName.MagicResist, 100.0),   // Magic defense
                    new StartingSkill(SkillName.Healing, 100.0)        // Bandage healing
                },
                StatLocks = new StatLocks(StatLockType.Down, StatLockType.Up, StatLockType.Locked),
                PersonalityType = NPCPersonalities.PersonalityType.Thief,
                SpeechPattern = NPCPersonalities.SpeechPattern.Casual,
                CombatStyle = CombatStyle.Melee,
                BaseAIType = AIType.AI_Thief,
                OptimalCombatRange = 1
            };
        }

        private static SidekickArchetype CreateNecromancer()
        {
            // T2A Necromancer Build - 700 skill points, 225 stats
            return new SidekickArchetype
            {
                Type = SidekickArchetypeType.Necromancer,
                Name = "Necromancer",
                StartingStr = 80,   // HP pool (Str = HP)
                StartingDex = 45,   // Casting speed
                StartingInt = 100,  // Max mana pool
                StartingSkills = new List<StartingSkill>
                {
                    // Necromancy core
                    new StartingSkill(SkillName.Necromancy, 100.0),    // Death magic
                    new StartingSkill(SkillName.SpiritSpeak, 100.0),   // Spirit communication
                    new StartingSkill(SkillName.Magery, 100.0),        // Standard magic
                    new StartingSkill(SkillName.EvalInt, 100.0),       // Spell damage
                    // Defense & utility
                    new StartingSkill(SkillName.Meditation, 100.0),    // Mana regen
                    new StartingSkill(SkillName.MagicResist, 100.0),   // Magic defense
                    new StartingSkill(SkillName.Wrestling, 100.0)      // Melee defense
                },
                StatLocks = new StatLocks(StatLockType.Locked, StatLockType.Locked, StatLockType.Up),
                PersonalityType = NPCPersonalities.PersonalityType.Mage,
                SpeechPattern = NPCPersonalities.SpeechPattern.Cryptic,
                CombatStyle = CombatStyle.Mage,
                BaseAIType = AIType.AI_NecroMage,
                OptimalCombatRange = 10
            };
        }

        private static SidekickArchetype CreateBattlemage()
        {
            // T2A Battlemage/Scribe Build - 700 skill points, 225 stats
            return new SidekickArchetype
            {
                Type = SidekickArchetypeType.Battlemage,
                Name = "Battlemage",
                StartingStr = 80,   // Melee damage
                StartingDex = 70,   // Balanced
                StartingInt = 75,   // Spell mana
                StartingSkills = new List<StartingSkill>
                {
                    // Combat core
                    new StartingSkill(SkillName.Swords, 100.0),        // Melee weapon
                    new StartingSkill(SkillName.Tactics, 100.0),       // +damage
                    new StartingSkill(SkillName.Anatomy, 100.0),       // +damage + heal bonus
                    // Magic
                    new StartingSkill(SkillName.Magery, 100.0),        // Spell casting
                    new StartingSkill(SkillName.EvalInt, 100.0),       // Spell damage
                    // Defense & utility
                    new StartingSkill(SkillName.MagicResist, 100.0),   // Magic defense
                    new StartingSkill(SkillName.Meditation, 100.0)     // Mana regen
                },
                StatLocks = new StatLocks(StatLockType.Up, StatLockType.Locked, StatLockType.Up),
                PersonalityType = NPCPersonalities.PersonalityType.Warrior,
                SpeechPattern = NPCPersonalities.SpeechPattern.Formal,
                CombatStyle = CombatStyle.Hybrid,
                BaseAIType = AIType.AI_Mage,
                OptimalCombatRange = 5
            };
        }

        private static SidekickArchetype CreateCleric()
        {
            // T2A Cleric/War Mage Build - 700 skill points, 225 stats
            return new SidekickArchetype
            {
                Type = SidekickArchetypeType.Cleric,
                Name = "Cleric",
                StartingStr = 80,   // Mace damage
                StartingDex = 70,   // Balanced
                StartingInt = 75,   // Heal mana
                StartingSkills = new List<StartingSkill>
                {
                    // Combat core
                    new StartingSkill(SkillName.Macing, 100.0),        // Maces (classic cleric weapon)
                    new StartingSkill(SkillName.Tactics, 100.0),       // +damage
                    // Healing & magic
                    new StartingSkill(SkillName.Magery, 100.0),        // Heal spells
                    new StartingSkill(SkillName.Healing, 100.0),       // Bandage healing
                    new StartingSkill(SkillName.Anatomy, 100.0),       // +heal bonus
                    // Defense & utility
                    new StartingSkill(SkillName.MagicResist, 100.0),   // Magic defense
                    new StartingSkill(SkillName.Parry, 100.0)       // Shield blocking
                },
                StatLocks = new StatLocks(StatLockType.Locked, StatLockType.Locked, StatLockType.Up),
                PersonalityType = NPCPersonalities.PersonalityType.Healer,
                SpeechPattern = NPCPersonalities.SpeechPattern.Formal,
                CombatStyle = CombatStyle.Hybrid,
                BaseAIType = AIType.AI_Healer,
                OptimalCombatRange = 5
            };
        }

        private static SidekickArchetype CreateDruid()
        {
            // T2A Druid/Nature Mage Build - 700 skill points, 225 stats
            return new SidekickArchetype
            {
                Type = SidekickArchetypeType.Druid,
                Name = "Druid",
                StartingStr = 80,   // HP pool (Str = HP)
                StartingDex = 45,   // Casting speed
                StartingInt = 100,  // Max mana pool
                StartingSkills = new List<StartingSkill>
                {
                    // Nature magic
                    new StartingSkill(SkillName.Magery, 100.0),        // Spell casting
                    new StartingSkill(SkillName.EvalInt, 100.0),       // Spell damage
                    new StartingSkill(SkillName.Meditation, 100.0),    // Mana regen
                    // Animal skills
                    new StartingSkill(SkillName.AnimalLore, 100.0),    // Beast knowledge
                    new StartingSkill(SkillName.Veterinary, 100.0),    // Pet healing
                    // Defense & utility
                    new StartingSkill(SkillName.MagicResist, 100.0),   // Magic defense
                    new StartingSkill(SkillName.Wrestling, 100.0)      // Melee defense
                },
                StatLocks = new StatLocks(StatLockType.Locked, StatLockType.Locked, StatLockType.Up),
                PersonalityType = NPCPersonalities.PersonalityType.Healer,
                SpeechPattern = NPCPersonalities.SpeechPattern.Casual,
                CombatStyle = CombatStyle.Hybrid,
                BaseAIType = AIType.AI_Mage,
                OptimalCombatRange = 8
            };
        }

        private static SidekickArchetype CreateTamer()
        {
            // T2A Tamer Build - 700 skill points, 225 stats
            return new SidekickArchetype
            {
                Type = SidekickArchetypeType.Tamer,
                Name = "Tamer",
                StartingStr = 80,   // HP pool (Str = HP) - survive while taming
                StartingDex = 45,   // Casting/defense
                StartingInt = 100,  // Max mana for taming
                StartingSkills = new List<StartingSkill>
                {
                    // Taming core
                    new StartingSkill(SkillName.AnimalTaming, 100.0),  // Tame creatures
                    new StartingSkill(SkillName.AnimalLore, 100.0),    // Beast knowledge
                    new StartingSkill(SkillName.Veterinary, 100.0),    // Pet healing
                    // Magic for utility
                    new StartingSkill(SkillName.Magery, 100.0),        // Spell casting
                    new StartingSkill(SkillName.Meditation, 100.0),    // Mana regen
                    // Defense
                    new StartingSkill(SkillName.MagicResist, 100.0),   // Magic defense
                    new StartingSkill(SkillName.Wrestling, 100.0)      // Melee defense
                },
                StatLocks = new StatLocks(StatLockType.Locked, StatLockType.Locked, StatLockType.Up),
                PersonalityType = NPCPersonalities.PersonalityType.Ranger,
                SpeechPattern = NPCPersonalities.SpeechPattern.Casual,
                CombatStyle = CombatStyle.Hybrid,
                BaseAIType = AIType.AI_Melee,
                OptimalCombatRange = 3
            };
        }

        #endregion
    }
}

