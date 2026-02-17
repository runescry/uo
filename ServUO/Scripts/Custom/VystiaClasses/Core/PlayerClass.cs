using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Spells;

namespace Server.Custom.VystiaClasses
{
    /// <summary>
    /// Base enum for all Vystia character classes
    /// </summary>
    public enum PlayerClassType
    {
        None = 0,

        // Frosthold
        Barbarian = 1,
        Beastmaster = 2,

        // Emberlands
        Sorcerer = 3,

        // Desert
        Ranger = 4,
        Illusionist = 5,

        // Shadowfen
        Witch = 6,
        Warlock = 7,

        // Verdantpeak
        Druid = 8,
        Alchemist = 9,

        // Crystal Barrens
        Oracle = 10,

        // Ironclad
        Artificer = 11,
        Fighter = 12,
        Monk = 13,
        Templar = 14,

        // Skyreach
        IceMage = 15,

        // ShadowVoid
        Necromancer = 16,

        // Underwater
        Summoner = 17,

        // Sunken Isles
        BountyHunter = 18,

        // Glimmering Archipelago
        Knight = 19,

        // Wilderlands
        Shaman = 20,

        // Multi-Regional
        Wizard = 21,
        Cleric = 22,
        Paladin = 23,
        Bard = 24,
        Enchanter = 25,
        Rogue = 26
    }

    /// <summary>
    /// Base class defining stats, skills, and equipment for each player class
    /// </summary>
    public abstract class PlayerClass
    {
        public abstract PlayerClassType ClassType { get; }
        public abstract string ClassName { get; }
        public abstract string ClassDescription { get; }

        // Starting Stats
        public abstract int StartStr { get; }
        public abstract int StartDex { get; }
        public abstract int StartInt { get; }

        // Stat Caps (default UO is 125 for players)
        public virtual int StrCap => 125;
        public virtual int DexCap => 125;
        public virtual int IntCap => 125;

        // Skill Caps (default UO is 100.0 for individual skills, 700.0 total)
        public virtual double SkillCap => 100.0;
        public virtual double TotalSkillCap => 700.0;

        /// <summary>
        /// Get primary skills for this class (skills that start above 0)
        /// </summary>
        public abstract SkillName[] PrimarySkills { get; }

        /// <summary>
        /// Get starting skill values for primary skills
        /// </summary>
        public abstract double[] StartingSkillValues { get; }

        /// <summary>
        /// Equip starting gear for this class
        /// </summary>
        public abstract void EquipStartingGear(Mobile m);

        /// <summary>
        /// Give starting spellbook/abilities for this class (if applicable)
        /// </summary>
        public virtual void GiveStartingAbilities(Mobile m)
        {
            // Override in subclasses that have spellbooks
        }

        /// <summary>
        /// Initialize class-specific stats and skills on player
        /// </summary>
        public virtual void InitializeClass(Mobile m)
        {
            if (m == null)
                return;

            // Set starting stats
            m.RawStr = StartStr;
            m.RawDex = StartDex;
            m.RawInt = StartInt;

            // Set stat caps
            m.StrCap = StrCap;
            m.DexCap = DexCap;
            m.IntCap = IntCap;

            // Set skills
            if (PrimarySkills != null && StartingSkillValues != null)
            {
                int count = Math.Min(PrimarySkills.Length, StartingSkillValues.Length);
                for (int i = 0; i < count; i++)
                {
                    Skill skill = m.Skills[PrimarySkills[i]];
                    if (skill != null)
                    {
                        skill.Base = StartingSkillValues[i];
                        skill.Cap = SkillCap;
                    }
                }
            }

            // Equip gear
            EquipStartingGear(m);

            // Give abilities
            GiveStartingAbilities(m);
        }

        /// <summary>
        /// Get the PlayerClass instance for a given class type
        /// </summary>
        public static PlayerClass GetClass(PlayerClassType classType)
        {
            switch (classType)
            {
                // Fully Implemented Classes (10)
                case PlayerClassType.Barbarian: return new BarbarianClass();
                case PlayerClassType.IceMage: return new IceMageClass();
                case PlayerClassType.Artificer: return new ArtificerClass();
                case PlayerClassType.Druid: return new DruidClass();
                case PlayerClassType.Ranger: return new RangerClass();
                case PlayerClassType.Witch: return new WitchClass();
                case PlayerClassType.Fighter: return new FighterClass();
                case PlayerClassType.Wizard: return new WizardClass();
                case PlayerClassType.Cleric: return new ClericClass();
                case PlayerClassType.Paladin: return new PaladinClass();

                // Pending Classes (15) - Stub implementations
                case PlayerClassType.Beastmaster: return new BeastmasterClass();
                case PlayerClassType.Sorcerer: return new SorcererClass();
                case PlayerClassType.Illusionist: return new IllusionistClass();
                case PlayerClassType.Warlock: return new WarlockClass();
                case PlayerClassType.Alchemist: return new AlchemistClass();
                case PlayerClassType.Oracle: return new OracleClass();
                case PlayerClassType.Monk: return new MonkClass();
                case PlayerClassType.Templar: return new TemplarClass();
                case PlayerClassType.Necromancer: return new NecromancerClass();
                case PlayerClassType.Summoner: return new SummonerClass();
                case PlayerClassType.BountyHunter: return new BountyHunterClass();
                case PlayerClassType.Knight: return new KnightClass();
                case PlayerClassType.Shaman: return new ShamanClass();
                case PlayerClassType.Bard: return new BardClass();
                case PlayerClassType.Enchanter: return new EnchanterClass();
                case PlayerClassType.Rogue: return new RogueClass();

                // None or invalid
                default: return null;
            }
        }
    }
}
