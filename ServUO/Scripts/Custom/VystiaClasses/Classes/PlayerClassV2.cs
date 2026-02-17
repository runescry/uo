/*
 * Vystia Class System v2.0
 * Enhanced Player Class Framework
 *
 * This file defines the enhanced base class for all Vystia player classes.
 * It integrates with the new resource, stance, buff, and ability systems.
 *
 * Key Features:
 * - Secondary resource integration
 * - Stance system support
 * - Passive hooks (OnKill, OnCrit, OnBlock, OnHeal, OnDamage)
 * - Ability school binding
 * - Class-specific mechanics
 */

using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Commands;
using Server.Engines.Craft;
using Server.Custom.VystiaClasses.Systems;
using Server.Custom.VystiaClasses.Abilities;

namespace Server.Custom.VystiaClasses
{
    #region Enums

    /// <summary>
    /// All player class types in Vystia v2.0
    /// </summary>
    public enum PlayerClassTypeV2
    {
        None = 0,

        // Magic Classes (6 Core)
        IceMage,        // Frosthold - Chill stacking, frozen shatter
        Warlock,        // ShadowVoid - Soul Shards, demons
        Necromancer,    // ShadowVoid - Life Force, undead army
        Druid,          // Verdantpeak - Forms, nature magic
        Sorcerer,       // Emberlands - Elemental stances
        Bard,           // Multi-regional - Songs, Crescendo

        // Martial Classes (6 Core)
        Barbarian,      // Frosthold - Fury, Rage transformation
        Rogue,          // Multi-regional - Combo points, stealth
        Monk,           // Ironclad - Chi, martial stances
        Knight,         // Ironclad - Fortitude, tank
        Paladin,        // Multi-regional - Tithing, Virtues
        Ranger,         // Desert - Focus, terrain mastery

        // Extended Magic Classes (6 More)
        Witch,          // Shadowfen - Hex magic, curses
        Oracle,         // Crystal Barrens - Divination, time
        Summoner,       // Underwater - Summoning, bonds
        Shaman,         // Multi-regional - Totems, spirits
        Enchanter,      // Multi-regional - Enchanting, buffs
        Illusionist,    // Desert - Illusion, trickery

        // Extended Martial Classes (8 More)
        Fighter,        // Ironclad - Combat stances
        Templar,        // Ironclad - Zeal, holy warrior
        BountyHunter,   // Multi-regional - Pursuit, marks
        Beastmaster,    // Frosthold - Pet synergy
        Artificer,      // Ironclad - Steam, Charges, gadgets
        Alchemist,      // Verdantpeak - Mutagens, potions
        Cleric,         // Multi-regional - Faith, healing
        Wizard,         // Crystal Barrens - Standard magery
    }

    /// <summary>
    /// Class role for group composition
    /// </summary>
    public enum ClassRole
    {
        Tank,           // High threat, damage mitigation
        Healer,         // Healing and support
        MeleeDPS,       // Close-range damage
        RangedDPS,      // Long-range damage
        CasterDPS,      // Spell-based damage
        Support,        // Buffs, debuffs, utility
        Hybrid,         // Multiple roles
    }

    /// <summary>
    /// Primary resource type
    /// </summary>
    public enum PrimaryResourceType
    {
        Mana,
        Stamina,
        Both,
    }

    #endregion

    #region Player Class Definition

    /// <summary>
    /// Enhanced base class for all Vystia player classes
    /// </summary>
    public abstract class PlayerClassV2
    {
        #region Identity

        /// <summary>
        /// Class type enum value
        /// </summary>
        public abstract PlayerClassTypeV2 ClassType { get; }

        /// <summary>
        /// Display name of the class
        /// </summary>
        public abstract string ClassName { get; }

        /// <summary>
        /// Short description of the class
        /// </summary>
        public abstract string ClassDescription { get; }

        /// <summary>
        /// Class role for group composition
        /// </summary>
        public abstract ClassRole Role { get; }

        /// <summary>
        /// Home region of this class
        /// </summary>
        public virtual string HomeRegion => "Multi-Regional";

        /// <summary>
        /// Hue for class-themed items
        /// </summary>
        public virtual int ClassHue => 0;

        #endregion

        #region Stats

        /// <summary>
        /// Starting strength
        /// </summary>
        public abstract int StartStr { get; }

        /// <summary>
        /// Starting dexterity
        /// </summary>
        public abstract int StartDex { get; }

        /// <summary>
        /// Starting intelligence
        /// </summary>
        public abstract int StartInt { get; }

        /// <summary>
        /// Strength cap
        /// </summary>
        public virtual int StrCap => 125;

        /// <summary>
        /// Dexterity cap
        /// </summary>
        public virtual int DexCap => 125;

        /// <summary>
        /// Intelligence cap
        /// </summary>
        public virtual int IntCap => 125;

        /// <summary>
        /// Total stat cap
        /// </summary>
        public virtual int TotalStatCap => 225;

        #endregion

        #region Skills

        /// <summary>
        /// Primary skills for this class
        /// </summary>
        public abstract SkillName[] PrimarySkills { get; }

        /// <summary>
        /// Starting values for primary skills
        /// </summary>
        public abstract double[] StartingSkillValues { get; }

        /// <summary>
        /// Skill cap modifier (default 100.0)
        /// </summary>
        public virtual double SkillCapModifier => 100.0;

        /// <summary>
        /// Primary class skill unique to this class (from Vystia custom skills 58-83).
        /// Used for ability scaling and skill gain checks.
        /// </summary>
        public abstract SkillName ClassSkill { get; }

        #endregion

        #region Resources

        /// <summary>
        /// Primary resource type (Mana or Stamina)
        /// </summary>
        public abstract PrimaryResourceType PrimaryResource { get; }

        /// <summary>
        /// Secondary resource type (null if none)
        /// </summary>
        public virtual ResourceType? SecondaryResource => null;

        /// <summary>
        /// Maximum value for secondary resource
        /// </summary>
        public virtual int SecondaryResourceMax => 100;

        #endregion

        #region Stances

        /// <summary>
        /// Available stances for this class (null if none)
        /// </summary>
        public virtual StanceType[] AvailableStances => null;

        /// <summary>
        /// Default stance when entering combat (None = no stance)
        /// </summary>
        public virtual StanceType DefaultStance => StanceType.None;

        #endregion

        #region Abilities

        /// <summary>
        /// Ability school for this class
        /// </summary>
        public abstract AbilitySchool AbilitySchool { get; }

        /// <summary>
        /// Ability IDs this class has access to
        /// </summary>
        public virtual List<int> ClassAbilities => new List<int>();

        #endregion

        #region Class Initialization

        /// <summary>
        /// Initialize a mobile with this class
        /// </summary>
        public virtual void InitializeClass(Mobile m)
        {
            if (m == null)
                return;

            // Set stats
            m.Str = StartStr;
            m.Dex = StartDex;
            m.Int = StartInt;

            // Set stat caps
            if (m is PlayerMobile pm)
            {
                // StatCaps are typically set via StatCapAttribute or items
                // For now we just ensure the values fit
            }

            // Set skills
            SetStartingSkills(m);

            // Initialize secondary resource
            InitializeSecondaryResource(m);

            // Apply passives
            ApplyPassives(m);

            // Equipment
            EquipStartingGear(m);

            // Abilities
            GiveStartingAbilities(m);
        }

        /// <summary>
        /// Initialize runtime-only state for an already-created character.
        /// Does not modify stats, skills, or gear.
        /// </summary>
        public virtual void InitializeRuntimeState(Mobile m)
        {
            if (m == null)
                return;

            InitializeSecondaryResource(m);
            ApplyPassives(m);
        }

        /// <summary>
        /// Set starting skill values
        /// </summary>
        protected virtual void SetStartingSkills(Mobile m)
        {
            if (m == null || PrimarySkills == null || StartingSkillValues == null)
                return;

            for (int i = 0; i < PrimarySkills.Length && i < StartingSkillValues.Length; i++)
            {
                m.Skills[PrimarySkills[i]].Base = StartingSkillValues[i];
            }
        }

        /// <summary>
        /// Initialize secondary resource for this class
        /// </summary>
        protected virtual void InitializeSecondaryResource(Mobile m)
        {
            if (m is PlayerMobile pm && SecondaryResource.HasValue)
            {
                var manager = VystiaResourceManager.GetManager(pm);
                if (manager != null)
                {
                    manager.SetClassResources(ResourceType.None, SecondaryResource.Value);
                }

                VystiaResourceManager.SendBardStatus(pm);
            }
        }

        /// <summary>
        /// Equip starting gear (override in derived classes)
        /// </summary>
        public abstract void EquipStartingGear(Mobile m);

        /// <summary>
        /// Give starting abilities (override in derived classes)
        /// </summary>
        public abstract void GiveStartingAbilities(Mobile m);

        #endregion

        #region Passives

        /// <summary>
        /// Apply passive bonuses to a mobile
        /// </summary>
        public virtual void ApplyPassives(Mobile m)
        {
            // Override in derived classes
        }

        /// <summary>
        /// Remove passive bonuses from a mobile
        /// </summary>
        public virtual void RemovePassives(Mobile m)
        {
            // Override in derived classes
        }

        #endregion

        #region Combat Hooks

        /// <summary>
        /// Called when this class kills an enemy
        /// </summary>
        public virtual void OnKill(Mobile killer, Mobile victim)
        {
            // Override in derived classes
            // Example: Warlock generates Soul Shards
        }

        /// <summary>
        /// Called when this class lands a critical hit
        /// </summary>
        public virtual void OnCrit(Mobile attacker, Mobile target, int damage)
        {
            // Override in derived classes
            // Example: Warlock has chance for Soul Shard
        }

        /// <summary>
        /// Called when this class deals damage
        /// </summary>
        public virtual void OnDamageDealt(Mobile attacker, Mobile target, int damage)
        {
            // Override in derived classes
            // Example: Barbarian generates Fury
        }

        /// <summary>
        /// Called when this class takes damage
        /// </summary>
        public virtual void OnDamageTaken(Mobile victim, Mobile attacker, int damage)
        {
            // Override in derived classes
            // Example: Barbarian generates Fury when hit
        }

        /// <summary>
        /// Called when this class blocks an attack
        /// </summary>
        public virtual void OnBlock(Mobile blocker, Mobile attacker)
        {
            // Override in derived classes
            // Example: Knight generates Fortitude
        }

        /// <summary>
        /// Called when this class heals
        /// </summary>
        public virtual void OnHeal(Mobile healer, Mobile target, int amount)
        {
            // Override in derived classes
            // Example: Cleric generates Faith
        }

        /// <summary>
        /// Called when this class dodges
        /// </summary>
        public virtual void OnDodge(Mobile dodger, Mobile attacker)
        {
            // Override in derived classes
        }

        /// <summary>
        /// Called when combat starts
        /// </summary>
        public virtual void OnCombatStart(Mobile m)
        {
            // Override in derived classes
            // Apply default stance if any
            if (DefaultStance != StanceType.None)
            {
                VystiaStanceManager.Instance.ApplyStance(m, DefaultStance);
            }
        }

        /// <summary>
        /// Called when combat ends
        /// </summary>
        public virtual void OnCombatEnd(Mobile m)
        {
            // Override in derived classes
        }

        #endregion

        #region Utility

        /// <summary>
        /// Check if this class can use an ability
        /// </summary>
        public virtual bool CanUseAbility(Mobile m, int abilityId)
        {
            // Check if ability is in class list or is a general ability
            return ClassAbilities.Contains(abilityId);
        }

        /// <summary>
        /// Get bonus damage modifier for this class
        /// </summary>
        public virtual double GetBonusDamageModifier(Mobile m, VystiaDamageType damageType)
        {
            return 0.0;
        }

        /// <summary>
        /// Get bonus healing modifier for this class
        /// </summary>
        public virtual double GetBonusHealingModifier(Mobile m)
        {
            return 0.0;
        }

        #endregion

        #region Serialization

        /// <summary>
        /// Serialize class-specific data
        /// </summary>
        public virtual void Serialize(GenericWriter writer)
        {
            writer.Write((int)0); // version
        }

        /// <summary>
        /// Deserialize class-specific data
        /// </summary>
        public virtual void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();
        }

        #endregion

        #region Factory

        private static Dictionary<PlayerClassTypeV2, Type> m_ClassTypes;

        /// <summary>
        /// Register a class implementation
        /// </summary>
        public static void RegisterClass(PlayerClassTypeV2 classType, Type implementationType)
        {
            if (m_ClassTypes == null)
                m_ClassTypes = new Dictionary<PlayerClassTypeV2, Type>();

            m_ClassTypes[classType] = implementationType;
        }

        /// <summary>
        /// Get an instance of a class by type
        /// </summary>
        public static PlayerClassV2 GetClass(PlayerClassTypeV2 classType)
        {
            if (m_ClassTypes == null || !m_ClassTypes.TryGetValue(classType, out Type type))
                return null;

            return Activator.CreateInstance(type) as PlayerClassV2;
        }

        /// <summary>
        /// Get all registered class types
        /// </summary>
        public static IEnumerable<PlayerClassTypeV2> GetAllClassTypes()
        {
            if (m_ClassTypes == null)
                yield break;

            foreach (var type in m_ClassTypes.Keys)
                yield return type;
        }

        #endregion
    }

    #endregion

    #region Core Magic Classes

    /// <summary>
    /// Ice Mage - Chill stacking specialist
    /// </summary>
    public class IceMageClassV2 : PlayerClassV2
    {
        public override PlayerClassTypeV2 ClassType => PlayerClassTypeV2.IceMage;
        public override string ClassName => "Ice Mage";
        public override string ClassDescription => "Master of frost magic who stacks chill on enemies to freeze and shatter them.";
        public override ClassRole Role => ClassRole.CasterDPS;
        public override string HomeRegion => "Frosthold";
        public override int ClassHue => 1152;

        public override int StartStr => 20;
        public override int StartDex => 25;
        public override int StartInt => 55;
        public override int IntCap => 150;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Magery,
            SkillName.EvalInt,
            SkillName.Meditation,
            SkillName.MagicResist,
            SkillName.Inscribe,
            SkillName.Wrestling
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, 50.0, 40.0, 30.0, 20.0, 10.0
        };

        public override SkillName ClassSkill => SkillName.Cryomancy;

        public override PrimaryResourceType PrimaryResource => PrimaryResourceType.Mana;
        public override ResourceType? SecondaryResource => ResourceType.ChillStacks;
        public override int SecondaryResourceMax => 5;

        public override AbilitySchool AbilitySchool => AbilitySchool.Ice;

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new Robe() { Name = "Frostweave Robe", Hue = ClassHue });
            m.AddItem(new FloppyHat() { Hue = ClassHue });
            m.AddItem(new Sandals() { Hue = ClassHue });
            m.AddItem(new GnarledStaff() { Name = "Staff of Winter", Hue = ClassHue });

            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            // Ice Mage spellbook would go here
        }

        public override void OnDamageDealt(Mobile attacker, Mobile target, int damage)
        {
            // Cold damage adds chill stacks automatically via damage system
        }
    }

    /// <summary>
    /// Warlock - Soul Shard resource user
    /// </summary>
    public class WarlockClassV2 : PlayerClassV2
    {
        public override PlayerClassTypeV2 ClassType => PlayerClassTypeV2.Warlock;
        public override string ClassName => "Warlock";
        public override string ClassDescription => "Dark spellcaster who harvests Soul Shards from defeated enemies to fuel powerful abilities.";
        public override ClassRole Role => ClassRole.CasterDPS;
        public override string HomeRegion => "ShadowVoid";
        public override int ClassHue => 1109;

        public override int StartStr => 20;
        public override int StartDex => 25;
        public override int StartInt => 55;
        public override int IntCap => 145;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Magery,
            SkillName.EvalInt,
            SkillName.SpiritSpeak,
            SkillName.Necromancy,
            SkillName.Meditation,
            SkillName.MagicResist
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, 45.0, 40.0, 35.0, 30.0, 20.0
        };

        public override SkillName ClassSkill => SkillName.Demonology;

        public override PrimaryResourceType PrimaryResource => PrimaryResourceType.Mana;
        public override ResourceType? SecondaryResource => ResourceType.SoulShards;
        public override int SecondaryResourceMax => 3;

        public override AbilitySchool AbilitySchool => AbilitySchool.Dark;

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new Server.Items.Vystia.VystiaShadowclothRobe());
            m.AddItem(new Sandals() { Hue = ClassHue });
            m.AddItem(new BlackStaff() { Name = "Staff of Shadows", Hue = ClassHue });

            // Warlock spellbook: same item ID as necromancer (0x2253) but dark purple hue
            var spellbook = new Server.Items.WarlockSpellbook();
            spellbook.Hue = ClassHue; // Dark purple (0x455)
            m.Backpack.DropItem(spellbook);

            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            // Warlock spellbook would go here
        }

        public override void OnKill(Mobile killer, Mobile victim)
        {
            // Generate Soul Shard on kill
            if (killer is PlayerMobile pm)
            {
                var manager = VystiaResourceManager.GetManager(pm);
                if (manager != null)
                {
                    var resource = manager.GetResource(ResourceType.SoulShards);
                    if (resource != null)
                    {
                        resource.Generate(1);
                        pm.SendMessage(0x3B2, "You harvest a Soul Shard from your fallen enemy.");
                    }
                }
            }
        }

        public override void OnCrit(Mobile attacker, Mobile target, int damage)
        {
            // 25% chance for Soul Shard on crit
            if (Utility.RandomDouble() < 0.25 && attacker is PlayerMobile pm)
            {
                var manager = VystiaResourceManager.GetManager(pm);
                if (manager != null)
                {
                    var resource = manager.GetResource(ResourceType.SoulShards);
                    if (resource != null && resource.Current < resource.Maximum)
                    {
                        resource.Generate(1);
                        pm.SendMessage(0x3B2, "Your critical strike yields a Soul Shard!");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Necromancer - Life Force and undead army
    /// </summary>
    public class NecromancerClassV2 : PlayerClassV2
    {
        public override PlayerClassTypeV2 ClassType => PlayerClassTypeV2.Necromancer;
        public override string ClassName => "Necromancer";
        public override string ClassDescription => "Master of death who commands undead minions and drains life force from the dying.";
        public override ClassRole Role => ClassRole.CasterDPS;
        public override string HomeRegion => "ShadowVoid";
        public override int ClassHue => 1109;

        public override int StartStr => 20;
        public override int StartDex => 20;
        public override int StartInt => 60;
        public override int IntCap => 150;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Necromancy,
            SkillName.SpiritSpeak,
            SkillName.Magery,
            SkillName.EvalInt,
            SkillName.Meditation,
            SkillName.MagicResist
        };

        public override double[] StartingSkillValues => new double[]
        {
            55.0, 50.0, 35.0, 30.0, 30.0, 20.0
        };

        public override SkillName ClassSkill => SkillName.NecromancyArts;

        public override PrimaryResourceType PrimaryResource => PrimaryResourceType.Mana;
        public override ResourceType? SecondaryResource => ResourceType.LifeForce;
        public override int SecondaryResourceMax => 100;

        public override AbilitySchool AbilitySchool => AbilitySchool.Necromancy;

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new Server.Items.Vystia.VystiaDeathshroudRobe());
            m.AddItem(new Server.Items.Vystia.VystiaDeathboneHelm());
            m.AddItem(new Sandals() { Hue = ClassHue });
            m.AddItem(new Server.Items.Vystia.VystiaScythe());

            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            // Necromancer spellbook would go here
        }

        public override void OnKill(Mobile killer, Mobile victim)
        {
            // Generate Life Force on nearby deaths
            if (killer is PlayerMobile pm)
            {
                var manager = VystiaResourceManager.GetManager(pm);
                if (manager != null)
                {
                    var resource = manager.GetResource(ResourceType.LifeForce);
                    if (resource != null)
                    {
                        int gain = Math.Max(10, victim.HitsMax / 10);
                        resource.Generate(gain);
                        pm.SendMessage(0x3B2, $"You absorb {gain} Life Force from the death.");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Druid - Form-shifting nature caster
    /// </summary>
    public class DruidClassV2 : PlayerClassV2
    {
        public override PlayerClassTypeV2 ClassType => PlayerClassTypeV2.Druid;
        public override string ClassName => "Druid";
        public override string ClassDescription => "Shapeshifter who channels nature magic and transforms into powerful animal forms.";
        public override ClassRole Role => ClassRole.Hybrid;
        public override string HomeRegion => "Verdantpeak";
        public override int ClassHue => 2010;

        public override int StartStr => 30;
        public override int StartDex => 25;
        public override int StartInt => 45;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Magery,
            SkillName.AnimalLore,
            SkillName.AnimalTaming,
            SkillName.Veterinary,
            SkillName.Meditation,
            SkillName.MagicResist
        };

        public override double[] StartingSkillValues => new double[]
        {
            45.0, 45.0, 40.0, 35.0, 30.0, 25.0
        };

        public override SkillName ClassSkill => SkillName.Druidism;

        public override PrimaryResourceType PrimaryResource => PrimaryResourceType.Both;

        public override StanceType[] AvailableStances => new StanceType[]
        {
            StanceType.DruidBear,
            StanceType.DruidCat,
            StanceType.DruidTree,
            StanceType.DruidMoonkin,
            StanceType.DruidTravel
        };

        public override AbilitySchool AbilitySchool => AbilitySchool.Nature;

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new LeatherChest() { Name = "Wildweave Tunic", Hue = ClassHue });
            m.AddItem(new LeatherLegs() { Hue = ClassHue });
            m.AddItem(new LeatherGloves() { Hue = ClassHue });
            m.AddItem(new LeatherArms() { Hue = ClassHue });
            m.AddItem(new Sandals() { Hue = ClassHue });
            m.AddItem(new Server.Items.Vystia.VystiaWildStaff());

            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            // Druid spellbook would go here
        }
    }

    /// <summary>
    /// Sorcerer - Elemental stance master
    /// </summary>
    public class SorcererClassV2 : PlayerClassV2
    {
        public override PlayerClassTypeV2 ClassType => PlayerClassTypeV2.Sorcerer;
        public override string ClassName => "Sorcerer";
        public override string ClassDescription => "Elemental mage who attunes to fire, water, earth, or air to enhance their spells.";
        public override ClassRole Role => ClassRole.CasterDPS;
        public override string HomeRegion => "Emberlands";
        public override int ClassHue => 0x455; // Dark Purple

        public override int StartStr => 20;
        public override int StartDex => 25;
        public override int StartInt => 55;
        public override int IntCap => 150;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Magery,
            SkillName.EvalInt,
            SkillName.Meditation,
            SkillName.MagicResist,
            SkillName.Inscribe,
            SkillName.Alchemy
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, 50.0, 40.0, 30.0, 25.0, 25.0
        };

        public override SkillName ClassSkill => SkillName.Elementalism;

        public override PrimaryResourceType PrimaryResource => PrimaryResourceType.Mana;

        public override StanceType[] AvailableStances => new StanceType[]
        {
            StanceType.SorcererFire,
            StanceType.SorcererWater,
            StanceType.SorcererEarth,
            StanceType.SorcererAir
        };

        public override StanceType DefaultStance => StanceType.SorcererFire;

        public override AbilitySchool AbilitySchool => AbilitySchool.Elemental;

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new Robe() { Name = "Elementalist Robe", Hue = ClassHue });
            m.AddItem(new FloppyHat() { Name = "Elementalist Hood", Hue = ClassHue });
            m.AddItem(new Sandals() { Hue = ClassHue });
            m.AddItem(new QuarterStaff() { Name = "Staff of Elements", Hue = ClassHue });

            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            // Sorcerer spellbook would go here
        }
    }

    /// <summary>
    /// Bard - Song-based support with Crescendo
    /// </summary>
    public class BardClassV2 : PlayerClassV2
    {
        public override PlayerClassTypeV2 ClassType => PlayerClassTypeV2.Bard;
        public override string ClassName => "Bard";
        public override string ClassDescription => "Musical performer who channels songs to buff allies and debuff enemies.";
        public override ClassRole Role => ClassRole.Support;
        public override string HomeRegion => "Multi-Regional";
        public override int ClassHue => 1161;

        public override int StartStr => 25;
        public override int StartDex => 35;
        public override int StartInt => 40;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Musicianship,
            SkillName.Provocation,
            SkillName.Discordance,
            SkillName.Peacemaking,
            SkillName.Magery,
            SkillName.EvalInt
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, 45.0, 40.0, 35.0, 30.0, 20.0
        };

        public override SkillName ClassSkill =>
#if VYSTIA_SONGWEAVING
            SkillName.Songweaving;
#else
            SkillName.Musicianship;
#endif

        public override PrimaryResourceType PrimaryResource => PrimaryResourceType.Mana;
        public override ResourceType? SecondaryResource =>
#if VYSTIA_CRESCENDO
            ResourceType.Crescendo;
#else
            null;
#endif
        public override int SecondaryResourceMax =>
#if VYSTIA_CRESCENDO
            100;
#else
            0;
#endif

        public override AbilitySchool AbilitySchool => AbilitySchool.Bardic;

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new FancyShirt() { Name = "Performer's Shirt", Hue = ClassHue });
            m.AddItem(new LongPants() { Hue = ClassHue });
            m.AddItem(new Cloak() { Name = "Bardic Cloak", Hue = ClassHue });
            m.AddItem(new FeatheredHat() { Hue = ClassHue });
            m.AddItem(new Boots() { Hue = ClassHue });
            
            // Bard spawns with blessed lute in backpack
            var lute = new Lute() { Name = "Enchanted Lute", Hue = ClassHue };
            lute.LootType = LootType.Blessed;
            m.Backpack.DropItem(lute);

            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

#if VYSTIA_SONGWEAVING
            m.Backpack.DropItem(new SongweavingSpellbook());
#endif

#if VYSTIA_CRESCENDO
            if (m is PlayerMobile pm)
            {
                Server.Custom.VystiaClasses.Gumps.CrescendoTrackerGump.Enable(pm);
            }
#endif
        }

        public override void InitializeRuntimeState(Mobile m)
        {
            base.InitializeRuntimeState(m);

#if VYSTIA_CRESCENDO
            if (m is PlayerMobile pm)
            {
                Server.Custom.VystiaClasses.Gumps.CrescendoTrackerGump.Enable(pm);
            }
#endif
        }
    }

    #endregion

    #region Core Martial Classes

    /// <summary>
    /// Barbarian - Fury resource and Rage transformation
    /// </summary>
    public class BarbarianClassV2 : PlayerClassV2
    {
        public override PlayerClassTypeV2 ClassType => PlayerClassTypeV2.Barbarian;
        public override string ClassName => "Barbarian";
        public override string ClassDescription => "Savage warrior who builds Fury in combat and unleashes devastating Rage.";
        public override ClassRole Role => ClassRole.MeleeDPS;
        public override string HomeRegion => "Frosthold";
        public override int ClassHue => 1150;

        public override int StartStr => 55;
        public override int StartDex => 25;
        public override int StartInt => 10;
        public override int StrCap => 150;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Swords,
            SkillName.Macing,
            SkillName.Tactics,
            SkillName.Anatomy,
            SkillName.Healing,
            SkillName.MagicResist
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, 40.0, 45.0, 40.0, 30.0, 15.0
        };

        public override SkillName ClassSkill => SkillName.Berserking;

        public override PrimaryResourceType PrimaryResource => PrimaryResourceType.Stamina;
        public override ResourceType? SecondaryResource => ResourceType.Fury;
        public override int SecondaryResourceMax => 100;

        public override StanceType[] AvailableStances => new StanceType[]
        {
            StanceType.BarbarianNormal,
            StanceType.BarbarianRage
        };

        public override AbilitySchool AbilitySchool => AbilitySchool.Barbarian;

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new BoneChest() { Name = "Frostbone Harness", Hue = 0x26 }); // Blood red
            m.AddItem(new BoneLegs() { Hue = 0x26 }); // Blood red
            m.AddItem(new BoneArms() { Hue = 0x26 }); // Blood red
            m.AddItem(new BoneGloves() { Hue = 0x26 }); // Blood red
            m.AddItem(new BoneHelm() { Name = "Skull Helmet", Hue = 0x26 }); // Blood red
            m.AddItem(new Boots() { Hue = 0x26 }); // Blood red
            m.AddItem(new DoubleAxe() { Name = "Frostborn Axe", Hue = ClassHue });

            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            // Barbarian abilities would go here
        }

        public override void OnDamageDealt(Mobile attacker, Mobile target, int damage)
        {
            // Generate Fury on damage dealt
            if (attacker is PlayerMobile pm)
            {
                var manager = VystiaResourceManager.GetManager(pm);
                if (manager != null)
                {
                    var resource = manager.GetResource(ResourceType.Fury);
                    if (resource != null)
                    {
                        int gain = Math.Max(1, damage / 10);
                        resource.Generate(gain);
                    }
                }
            }
        }

        public override void OnDamageTaken(Mobile victim, Mobile attacker, int damage)
        {
            // Generate Fury on damage taken
            if (victim is PlayerMobile pm)
            {
                var manager = VystiaResourceManager.GetManager(pm);
                if (manager != null)
                {
                    var resource = manager.GetResource(ResourceType.Fury);
                    if (resource != null)
                    {
                        int gain = Math.Max(1, damage / 5);
                        resource.Generate(gain);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Rogue - Combo points and stealth
    /// </summary>
    public class RogueClassV2 : PlayerClassV2
    {
        public override PlayerClassTypeV2 ClassType => PlayerClassTypeV2.Rogue;
        public override string ClassName => "Rogue";
        public override string ClassDescription => "Stealthy assassin who builds Combo Points on targets for devastating finishers.";
        public override ClassRole Role => ClassRole.MeleeDPS;
        public override string HomeRegion => "Multi-Regional";
        public override int ClassHue => 1109;

        public override int StartStr => 30;
        public override int StartDex => 50;
        public override int StartInt => 10;
        public override int DexCap => 150;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Fencing,
            SkillName.Hiding,
            SkillName.Stealth,
            SkillName.Tactics,
            SkillName.Anatomy,
            SkillName.Poisoning
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, 50.0, 45.0, 40.0, 30.0, 25.0
        };

        public override SkillName ClassSkill => SkillName.Subterfuge;

        public override PrimaryResourceType PrimaryResource => PrimaryResourceType.Stamina;
        public override ResourceType? SecondaryResource => ResourceType.ComboPoints;
        public override int SecondaryResourceMax => 5;

        public override StanceType[] AvailableStances => new StanceType[]
        {
            StanceType.RogueShadow,
            StanceType.RogueOutlaw,
            StanceType.RogueSubtlety
        };

        public override StanceType DefaultStance => StanceType.RogueShadow;

        public override AbilitySchool AbilitySchool => AbilitySchool.Rogue;

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new LeatherChest() { Name = "Shadowleather Tunic", Hue = ClassHue });
            m.AddItem(new LeatherLegs() { Hue = ClassHue });
            m.AddItem(new LeatherGloves() { Hue = ClassHue });
            m.AddItem(new LeatherArms() { Hue = ClassHue });
            m.AddItem(new LeatherCap() { Hue = ClassHue });
            m.AddItem(new Boots() { Hue = ClassHue });
            m.AddItem(new AssassinSpike() { Name = "Shadow Blade", Hue = ClassHue });

            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            // Rogue abilities would go here
        }
    }

    /// <summary>
    /// Monk - Chi resource and martial stances
    /// </summary>
    public class MonkClassV2 : PlayerClassV2
    {
        public override PlayerClassTypeV2 ClassType => PlayerClassTypeV2.Monk;
        public override string ClassName => "Monk";
        public override string ClassDescription => "Disciplined martial artist who builds Chi through combat to unleash powerful techniques.";
        public override ClassRole Role => ClassRole.Hybrid;
        public override string HomeRegion => "Ironclad";
        public override int ClassHue => 2305;

        public override int StartStr => 35;
        public override int StartDex => 40;
        public override int StartInt => 25;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Wrestling,
            SkillName.Tactics,
            SkillName.Anatomy,
            SkillName.Focus,
            SkillName.Meditation,
            SkillName.Healing
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, 45.0, 40.0, 40.0, 30.0, 25.0
        };

        public override SkillName ClassSkill => SkillName.MartialArts;

        public override PrimaryResourceType PrimaryResource => PrimaryResourceType.Stamina;
        public override ResourceType? SecondaryResource => ResourceType.Chi;
        public override int SecondaryResourceMax => 5;

        public override StanceType[] AvailableStances => new StanceType[]
        {
            StanceType.MonkWindwalker,
            StanceType.MonkBrewmaster,
            StanceType.MonkMistweaver
        };

        public override StanceType DefaultStance => StanceType.MonkWindwalker;

        public override AbilitySchool AbilitySchool => AbilitySchool.Monk;

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new MonkRobe() { Name = "Monk's Robes", Hue = ClassHue });
            m.AddItem(new Sandals() { Hue = ClassHue });

            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            // Monk abilities would go here
        }

        public override void OnDamageDealt(Mobile attacker, Mobile target, int damage)
        {
            // 30% chance to generate Chi on hit
            if (Utility.RandomDouble() < 0.30 && attacker is PlayerMobile pm)
            {
                var manager = VystiaResourceManager.GetManager(pm);
                if (manager != null)
                {
                    var resource = manager.GetResource(ResourceType.Chi);
                    if (resource != null && resource.Current < resource.Maximum)
                    {
                        resource.Generate(1);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Knight - Fortitude tank
    /// </summary>
    public class KnightClassV2 : PlayerClassV2
    {
        public override PlayerClassTypeV2 ClassType => PlayerClassTypeV2.Knight;
        public override string ClassName => "Knight";
        public override string ClassDescription => "Stalwart defender who builds Fortitude through blocking to protect allies.";
        public override ClassRole Role => ClassRole.Tank;
        public override string HomeRegion => "Ironclad";
        public override int ClassHue => 2305;

        public override int StartStr => 50;
        public override int StartDex => 20;
        public override int StartInt => 10;
        public override int StrCap => 145;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Swords,
            SkillName.Parry,
            SkillName.Tactics,
            SkillName.Anatomy,
            SkillName.Healing,
            SkillName.MagicResist
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, 50.0, 45.0, 35.0, 30.0, 30.0
        };

        public override SkillName ClassSkill => SkillName.ChivalricArts;

        public override PrimaryResourceType PrimaryResource => PrimaryResourceType.Stamina;
        public override ResourceType? SecondaryResource => ResourceType.Fortitude;
        public override int SecondaryResourceMax => 10;

        public override AbilitySchool AbilitySchool => AbilitySchool.Knight;

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new PlateChest() { Name = "Knight's Cuirass", Hue = ClassHue });
            m.AddItem(new PlateLegs() { Hue = ClassHue });
            m.AddItem(new PlateArms() { Hue = ClassHue });
            m.AddItem(new PlateGloves() { Hue = ClassHue });
            m.AddItem(new PlateHelm() { Hue = ClassHue });
            m.AddItem(new Boots() { Hue = ClassHue });
            m.AddItem(new VikingSword() { Name = "Knight's Blade", Hue = ClassHue });
            m.AddItem(new HeaterShield() { Name = "Knight's Shield", Hue = ClassHue });

            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            // Knight abilities would go here
        }

        public override void OnBlock(Mobile blocker, Mobile attacker)
        {
            // Generate Fortitude on block
            if (blocker is PlayerMobile pm)
            {
                var manager = VystiaResourceManager.GetManager(pm);
                if (manager != null)
                {
                    var resource = manager.GetResource(ResourceType.Fortitude);
                    if (resource != null)
                    {
                        resource.Generate(1);
                        pm.SendMessage(0x3B2, "You gain Fortitude from blocking!");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Paladin - Tithing and Virtues
    /// </summary>
    public class PaladinClassV2 : PlayerClassV2
    {
        public override PlayerClassTypeV2 ClassType => PlayerClassTypeV2.Paladin;
        public override string ClassName => "Paladin";
        public override string ClassDescription => "Holy warrior who tithes gold for power and embodies the virtues.";
        public override ClassRole Role => ClassRole.Hybrid;
        public override string HomeRegion => "Multi-Regional";
        public override int ClassHue => 1150; // Really light grey

        public override int StartStr => 45;
        public override int StartDex => 20;
        public override int StartInt => 35;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Swords,
            SkillName.Chivalry,
            SkillName.Tactics,
            SkillName.Anatomy,
            SkillName.Focus,
            SkillName.MagicResist
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, 50.0, 40.0, 35.0, 30.0, 25.0
        };

        public override SkillName ClassSkill => SkillName.HolyDevotion;

        public override PrimaryResourceType PrimaryResource => PrimaryResourceType.Both;
        public override ResourceType? SecondaryResource => ResourceType.Virtues;
        public override int SecondaryResourceMax => 4;

        public override StanceType[] AvailableStances => new StanceType[]
        {
            StanceType.PaladinDevotion,
            StanceType.PaladinRetribution,
            StanceType.PaladinProtection
        };

        public override StanceType DefaultStance => StanceType.PaladinDevotion;

        public override AbilitySchool AbilitySchool => AbilitySchool.Paladin;

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new PlateChest() { Name = "Paladin's Breastplate", Hue = ClassHue });
            m.AddItem(new PlateLegs() { Hue = ClassHue });
            m.AddItem(new PlateArms() { Hue = ClassHue });
            m.AddItem(new PlateGloves() { Hue = ClassHue });
            m.AddItem(new PlateHelm() { Hue = ClassHue });
            m.AddItem(new Boots() { Hue = ClassHue });
            m.AddItem(new Server.Items.Vystia.VystiaLance());
            m.AddItem(new OrderShield() { Name = "Virtuous Shield", Hue = ClassHue });

            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            // Paladin abilities would go here
        }
    }

    /// <summary>
    /// Ranger - Focus resource and terrain mastery
    /// </summary>
    public class RangerClassV2 : PlayerClassV2
    {
        public override PlayerClassTypeV2 ClassType => PlayerClassTypeV2.Ranger;
        public override string ClassName => "Ranger";
        public override string ClassDescription => "Expert archer and tracker who builds Focus through patience and precision.";
        public override ClassRole Role => ClassRole.RangedDPS;
        public override string HomeRegion => "Desert";
        public override int ClassHue => 1719;

        public override int StartStr => 30;
        public override int StartDex => 45;
        public override int StartInt => 25;
        public override int DexCap => 145;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Archery,
            SkillName.Tracking,
            SkillName.Tactics,
            SkillName.Anatomy,
            SkillName.Hiding,
            SkillName.AnimalLore
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, 50.0, 40.0, 35.0, 30.0, 25.0
        };

        public override SkillName ClassSkill => SkillName.Marksmanship;

        public override PrimaryResourceType PrimaryResource => PrimaryResourceType.Stamina;
        public override ResourceType? SecondaryResource => ResourceType.Focus;
        public override int SecondaryResourceMax => 100;

        public override StanceType[] AvailableStances => new StanceType[]
        {
            StanceType.RangerHawk,
            StanceType.RangerWolf,
            StanceType.RangerBear
        };

        public override StanceType DefaultStance => StanceType.RangerHawk;

        public override AbilitySchool AbilitySchool => AbilitySchool.Ranger;

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new LeatherChest() { Name = "Ranger's Vest", Hue = ClassHue });
            m.AddItem(new LeatherLegs() { Hue = ClassHue });
            m.AddItem(new LeatherGloves() { Hue = ClassHue });
            m.AddItem(new LeatherArms() { Hue = ClassHue });
            m.AddItem(new LeatherCap() { Hue = ClassHue });
            m.AddItem(new Boots() { Hue = ClassHue });
            m.AddItem(new Server.Items.Vystia.VystiaYumi());
            m.Backpack.DropItem(new Arrow(100));

            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            // Ranger abilities would go here
        }
    }

    #endregion

    #region Extended Magic Classes

    /// <summary>
    /// Witch - Hex magic and curses
    /// </summary>
    public class WitchClassV2 : PlayerClassV2
    {
        public override PlayerClassTypeV2 ClassType => PlayerClassTypeV2.Witch;
        public override string ClassName => "Witch";
        public override string ClassDescription => "Dark practitioner of hex magic who curses enemies and drains their life force.";
        public override ClassRole Role => ClassRole.CasterDPS;
        public override string HomeRegion => "Shadowfen";
        public override int ClassHue => 0x3E8; // Dark Grey

        public override int StartStr => 20;
        public override int StartDex => 25;
        public override int StartInt => 55;
        public override int IntCap => 145;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Magery,
            SkillName.EvalInt,
            SkillName.Poisoning,
            SkillName.SpiritSpeak,
            SkillName.Meditation,
            SkillName.MagicResist
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, 45.0, 40.0, 35.0, 30.0, 20.0
        };

        public override SkillName ClassSkill => SkillName.Hexcraft;

        public override PrimaryResourceType PrimaryResource => PrimaryResourceType.Mana;
        public override AbilitySchool AbilitySchool => AbilitySchool.Hex;

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new Robe() { Name = "Witch's Robes", Hue = ClassHue });
            m.AddItem(new Cloak() { Name = "Tattered Cloak", Hue = ClassHue });
            m.AddItem(new FloppyHat() { Name = "Witch's Hat", Hue = ClassHue });
            m.AddItem(new Sandals() { Hue = ClassHue });
            m.AddItem(new ShepherdsCrook() { Name = "Hexwood Staff", Hue = ClassHue });

            // Witch is female-only
            if (m is PlayerMobile pm && pm.Female == false)
            {
                pm.Female = true;
            }

            // Give necromancer spellbook (same as necromancer)
            var spellbook = new Server.Items.VystiaNecromancerSpellbook();
            spellbook.Hue = ClassHue;
            m.Backpack.DropItem(spellbook);

            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            // Witch spellbook would go here
        }
    }

    /// <summary>
    /// Oracle - Divination and time manipulation
    /// </summary>
    public class OracleClassV2 : PlayerClassV2
    {
        public override PlayerClassTypeV2 ClassType => PlayerClassTypeV2.Oracle;
        public override string ClassName => "Oracle";
        public override string ClassDescription => "Seer who manipulates time and fate through divination magic.";
        public override ClassRole Role => ClassRole.Support;
        public override string HomeRegion => "Crystal Barrens";
        public override int ClassHue => 1154;

        public override int StartStr => 20;
        public override int StartDex => 25;
        public override int StartInt => 55;
        public override int IntCap => 150;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Magery,
            SkillName.EvalInt,
            SkillName.Meditation,
            SkillName.SpiritSpeak,
            SkillName.ItemID,
            SkillName.MagicResist
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, 50.0, 40.0, 35.0, 25.0, 20.0
        };

        public override SkillName ClassSkill => SkillName.Divination;

        public override PrimaryResourceType PrimaryResource => PrimaryResourceType.Mana;
        public override AbilitySchool AbilitySchool => AbilitySchool.Divination;

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new Robe() { Name = "Seer's Robe", Hue = ClassHue });
            m.AddItem(new Cloak() { Name = "Oracle's Mantle", Hue = ClassHue });
            m.AddItem(new WideBrimHat() { Hue = ClassHue });
            m.AddItem(new Sandals() { Hue = ClassHue });

            // Oracle spawns with spellbook in hand (no staff)
            var spellbook = new Server.Items.OracleSpellbook();
            spellbook.Hue = ClassHue;
            m.EquipItem(spellbook);

            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            // Oracle spellbook would go here
        }
    }

    /// <summary>
    /// Summoner - Creature summoning and bonds
    /// </summary>
    public class SummonerClassV2 : PlayerClassV2
    {
        public override PlayerClassTypeV2 ClassType => PlayerClassTypeV2.Summoner;
        public override string ClassName => "Summoner";
        public override string ClassDescription => "Master of conjuration who calls forth powerful creatures to fight.";
        public override ClassRole Role => ClassRole.CasterDPS;
        public override string HomeRegion => "Underwater";
        public override int ClassHue => 1365;

        public override int StartStr => 20;
        public override int StartDex => 20;
        public override int StartInt => 60;
        public override int IntCap => 150;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Magery,
            SkillName.EvalInt,
            SkillName.SpiritSpeak,
            SkillName.AnimalLore,
            SkillName.Meditation,
            SkillName.MagicResist
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, 45.0, 45.0, 35.0, 30.0, 25.0
        };

        public override SkillName ClassSkill => SkillName.Conjuration;

        public override PrimaryResourceType PrimaryResource => PrimaryResourceType.Mana;
        public override AbilitySchool AbilitySchool => AbilitySchool.Summoning;

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new Robe() { Name = "Conjurer's Robe", Hue = ClassHue });
            m.AddItem(new Cloak() { Name = "Tide Cloak", Hue = ClassHue });
            m.AddItem(new FloppyHat() { Hue = ClassHue });
            m.AddItem(new Sandals() { Hue = ClassHue });
            m.AddItem(new GnarledStaff() { Name = "Staff of Summoning", Hue = ClassHue });

            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            // Summoner spellbook would go here
        }
    }

    /// <summary>
    /// Shaman - Totems and spirits
    /// </summary>
    public class ShamanClassV2 : PlayerClassV2
    {
        public override PlayerClassTypeV2 ClassType => PlayerClassTypeV2.Shaman;
        public override string ClassName => "Shaman";
        public override string ClassDescription => "Spirit caller who places totems and communes with ancestral spirits.";
        public override ClassRole Role => ClassRole.Hybrid;
        public override string HomeRegion => "Multi-Regional";
        public override int ClassHue => 0x7D6; // Dark Green

        public override int StartStr => 30;
        public override int StartDex => 25;
        public override int StartInt => 45;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Magery,
            SkillName.SpiritSpeak,
            SkillName.Meditation,
            SkillName.Healing,
            SkillName.AnimalLore,
            SkillName.MagicResist
        };

        public override double[] StartingSkillValues => new double[]
        {
            45.0, 50.0, 40.0, 35.0, 30.0, 20.0
        };

        public override SkillName ClassSkill => SkillName.SpiritCalling;

        public override PrimaryResourceType PrimaryResource => PrimaryResourceType.Mana;
        public override AbilitySchool AbilitySchool => AbilitySchool.Shamanic;

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            // Shaman has same loadout as druid but with green leathers
            m.AddItem(new LeatherChest() { Name = "Spirit Vest", Hue = ClassHue });
            m.AddItem(new LeatherLegs() { Hue = ClassHue });
            m.AddItem(new LeatherGloves() { Hue = ClassHue });
            m.AddItem(new LeatherArms() { Hue = ClassHue });
            m.AddItem(new Sandals() { Hue = ClassHue });
            m.AddItem(new ShepherdsCrook() { Name = "Spirit Staff", Hue = ClassHue });

            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            // Shaman spellbook would go here
        }
    }

    /// <summary>
    /// Enchanter - Item enhancement and buffs
    /// </summary>
    public class EnchanterClassV2 : PlayerClassV2
    {
        public override PlayerClassTypeV2 ClassType => PlayerClassTypeV2.Enchanter;
        public override string ClassName => "Enchanter";
        public override string ClassDescription => "Master of magical augmentation who enhances weapons and armor.";
        public override ClassRole Role => ClassRole.Support;
        public override string HomeRegion => "Multi-Regional";
        public override int ClassHue => 1154;

        public override int StartStr => 25;
        public override int StartDex => 25;
        public override int StartInt => 50;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Magery,
            SkillName.EvalInt,
            SkillName.Inscribe,
            SkillName.ItemID,
            SkillName.Meditation,
            SkillName.MagicResist
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, 45.0, 45.0, 35.0, 30.0, 25.0
        };

        public override SkillName ClassSkill => SkillName.Runeweaving;

        public override PrimaryResourceType PrimaryResource => PrimaryResourceType.Mana;
        public override AbilitySchool AbilitySchool => AbilitySchool.Enchanting;

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new Robe() { Name = "Enchanter's Robe", Hue = ClassHue });
            m.AddItem(new Cloak() { Name = "Runic Cloak", Hue = ClassHue });
            m.AddItem(new WizardsHat() { Hue = ClassHue });
            m.AddItem(new Sandals() { Hue = ClassHue });
            m.AddItem(new Scepter() { Name = "Runestave", Hue = ClassHue });

            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            // Enchanter spellbook would go here
        }
    }

    /// <summary>
    /// Illusionist - Trickery and deception
    /// </summary>
    public class IllusionistClassV2 : PlayerClassV2
    {
        public override PlayerClassTypeV2 ClassType => PlayerClassTypeV2.Illusionist;
        public override string ClassName => "Illusionist";
        public override string ClassDescription => "Master of deception who creates illusions to confuse and misdirect enemies.";
        public override ClassRole Role => ClassRole.CasterDPS;
        public override string HomeRegion => "Desert";
        public override int ClassHue => 1719;

        public override int StartStr => 20;
        public override int StartDex => 30;
        public override int StartInt => 50;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Magery,
            SkillName.EvalInt,
            SkillName.Hiding,
            SkillName.Stealth,
            SkillName.Meditation,
            SkillName.MagicResist
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, 45.0, 40.0, 35.0, 30.0, 20.0
        };

        public override SkillName ClassSkill => SkillName.IllusionMagic;

        public override PrimaryResourceType PrimaryResource => PrimaryResourceType.Mana;
        public override AbilitySchool AbilitySchool => AbilitySchool.Illusion;

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new Robe() { Name = "Mirage Robe", Hue = ClassHue });
            m.AddItem(new Cloak() { Name = "Shimmer Cloak", Hue = ClassHue });
            m.AddItem(new FloppyHat() { Hue = ClassHue });
            m.AddItem(new Sandals() { Hue = ClassHue });
            m.AddItem(new BlackStaff() { Name = "Staff of Illusions", Hue = ClassHue });

            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            // Illusionist spellbook would go here
        }
    }

    #endregion

    #region Extended Martial Classes

    /// <summary>
    /// Fighter - Combat stance specialist
    /// </summary>
    public class FighterClassV2 : PlayerClassV2
    {
        public override PlayerClassTypeV2 ClassType => PlayerClassTypeV2.Fighter;
        public override string ClassName => "Fighter";
        public override string ClassDescription => "Versatile warrior who masters multiple combat stances.";
        public override ClassRole Role => ClassRole.MeleeDPS;
        public override string HomeRegion => "Ironclad";
        public override int ClassHue => 2305;

        public override int StartStr => 50;
        public override int StartDex => 25;
        public override int StartInt => 10;
        public override int StrCap => 145;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Swords,
            SkillName.Tactics,
            SkillName.Anatomy,
            SkillName.Parry,
            SkillName.Healing,
            SkillName.MagicResist
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, 50.0, 40.0, 35.0, 30.0, 25.0
        };

        public override SkillName ClassSkill => SkillName.CombatMastery;

        public override PrimaryResourceType PrimaryResource => PrimaryResourceType.Stamina;

        public override StanceType[] AvailableStances => new StanceType[]
        {
            StanceType.FighterAggressive,
            StanceType.FighterDefensive,
            StanceType.FighterBalanced,
            StanceType.FighterBerserker
        };

        public override StanceType DefaultStance => StanceType.FighterBalanced;

        public override AbilitySchool AbilitySchool => AbilitySchool.Fighter;

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new ChainChest() { Name = "Fighter's Hauberk", Hue = ClassHue });
            m.AddItem(new ChainLegs() { Hue = ClassHue });
            m.AddItem(new RingmailArms() { Hue = ClassHue });
            m.AddItem(new RingmailGloves() { Hue = ClassHue });
            m.AddItem(new Boots() { Hue = ClassHue });
            m.AddItem(new VikingSword() { Name = "Fighter's Blade", Hue = ClassHue });
            m.AddItem(new MetalShield() { Name = "Combat Shield", Hue = ClassHue });

            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            // Fighter abilities would go here
        }
    }

    /// <summary>
    /// Templar - Holy warrior with Zeal
    /// </summary>
    public class TemplarClassV2 : PlayerClassV2
    {
        public override PlayerClassTypeV2 ClassType => PlayerClassTypeV2.Templar;
        public override string ClassName => "Templar";
        public override string ClassDescription => "Zealous holy warrior who channels divine fury into devastating attacks.";
        public override ClassRole Role => ClassRole.MeleeDPS;
        public override string HomeRegion => "Ironclad";
        public override int ClassHue => 1150; // Really light grey

        public override int StartStr => 45;
        public override int StartDex => 20;
        public override int StartInt => 35;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Swords,
            SkillName.Chivalry,
            SkillName.Tactics,
            SkillName.Anatomy,
            SkillName.Focus,
            SkillName.MagicResist
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, 45.0, 45.0, 35.0, 30.0, 25.0
        };

        public override SkillName ClassSkill => SkillName.Zealotry;

        public override PrimaryResourceType PrimaryResource => PrimaryResourceType.Both;
        public override ResourceType? SecondaryResource => ResourceType.Zeal;
        public override int SecondaryResourceMax => 10;

        public override AbilitySchool AbilitySchool => AbilitySchool.Templar;

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new PlateChest() { Name = "Templar's Cuirass", Hue = ClassHue });
            m.AddItem(new PlateLegs() { Hue = ClassHue });
            m.AddItem(new PlateArms() { Hue = ClassHue });
            m.AddItem(new PlateGloves() { Hue = ClassHue });
            m.AddItem(new PlateHelm() { Hue = ClassHue });
            m.AddItem(new Boots() { Hue = ClassHue });
            m.AddItem(new Longsword() { Name = "Zealot's Blade", Hue = ClassHue });
            m.AddItem(new OrderShield() { Name = "Templar Shield" }); // Normal color, no hue

            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            // Templar abilities would go here
        }

        public override void OnDamageDealt(Mobile attacker, Mobile target, int damage)
        {
            // Generate Zeal on damage dealt
            if (attacker is PlayerMobile pm)
            {
                var manager = VystiaResourceManager.GetManager(pm);
                if (manager != null)
                {
                    var resource = manager.GetResource(ResourceType.Zeal);
                    if (resource != null && resource.Current < resource.Maximum)
                    {
                        if (Utility.RandomDouble() < 0.20) // 20% chance per hit
                        {
                            resource.Generate(1);
                            pm.SendMessage(0x3B2, "Your righteous fury builds!");
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Bounty Hunter - Mark and pursuit specialist
    /// </summary>
    public class BountyHunterClassV2 : PlayerClassV2
    {
        public override PlayerClassTypeV2 ClassType => PlayerClassTypeV2.BountyHunter;
        public override string ClassName => "Bounty Hunter";
        public override string ClassDescription => "Expert tracker who marks targets and builds Pursuit stacks for deadly strikes.";
        public override ClassRole Role => ClassRole.MeleeDPS;
        public override string HomeRegion => "Multi-Regional";
        public override int ClassHue => 1719;

        public override int StartStr => 35;
        public override int StartDex => 40;
        public override int StartInt => 15;
        public override int DexCap => 140;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Fencing,
            SkillName.Tracking,
            SkillName.Tactics,
            SkillName.Anatomy,
            SkillName.Hiding,
            SkillName.DetectHidden
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, 50.0, 40.0, 35.0, 30.0, 25.0
        };

        public override SkillName ClassSkill => SkillName.Manhunting;

        public override PrimaryResourceType PrimaryResource => PrimaryResourceType.Stamina;
        public override ResourceType? SecondaryResource => ResourceType.Pursuit;
        public override int SecondaryResourceMax => 10;

        public override AbilitySchool AbilitySchool => AbilitySchool.BountyHunter;

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new LeatherChest() { Name = "Hunter's Vest", Hue = ClassHue });
            m.AddItem(new LeatherLegs() { Hue = ClassHue });
            m.AddItem(new LeatherGloves() { Hue = ClassHue });
            m.AddItem(new LeatherArms() { Hue = ClassHue });
            m.AddItem(new LeatherCap() { Hue = ClassHue });
            m.AddItem(new Boots() { Hue = ClassHue });
            m.AddItem(new Kryss() { Name = "Hunter's Blade", Hue = ClassHue });
            m.AddItem(new Crossbow() { Name = "Tracker's Crossbow", Hue = ClassHue });
            m.Backpack.DropItem(new Bolt(50));

            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            // Bounty Hunter abilities would go here
        }
    }

    /// <summary>
    /// Beastmaster - Pet synergy specialist
    /// </summary>
    public class BeastmasterClassV2 : PlayerClassV2
    {
        public override PlayerClassTypeV2 ClassType => PlayerClassTypeV2.Beastmaster;
        public override string ClassName => "Beastmaster";
        public override string ClassDescription => "Expert animal handler who fights alongside powerful beast companions.";
        public override ClassRole Role => ClassRole.Hybrid;
        public override string HomeRegion => "Frosthold";
        public override int ClassHue => 0x7D6; // Dark green

        public override int StartStr => 35;
        public override int StartDex => 30;
        public override int StartInt => 35;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.AnimalTaming,
            SkillName.AnimalLore,
            SkillName.Veterinary,
            SkillName.Archery,
            SkillName.Tactics,
            SkillName.Tracking
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, 50.0, 40.0, 35.0, 30.0, 25.0
        };

        public override SkillName ClassSkill => SkillName.BeastBonding;

        public override PrimaryResourceType PrimaryResource => PrimaryResourceType.Stamina;

        public override AbilitySchool AbilitySchool => AbilitySchool.Beastmaster;

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new LeatherChest() { Name = "Beastmaster's Jerkin", Hue = ClassHue });
            m.AddItem(new LeatherLegs() { Hue = ClassHue });
            m.AddItem(new LeatherGloves() { Hue = ClassHue });
            m.AddItem(new LeatherArms() { Hue = ClassHue });
            m.AddItem(new LeatherCap() { Hue = ClassHue });
            m.AddItem(new Boots() { Hue = ClassHue });
            m.AddItem(new ShepherdsCrook() { Name = "Beastmaster's Crook", Hue = ClassHue });
            m.Backpack.DropItem(new Arrow(100));

            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            // Beastmaster abilities would go here
        }
    }

    /// <summary>
    /// Artificer - Steam and mechanical gadgets
    /// </summary>
    public class ArtificerClassV2 : PlayerClassV2
    {
        public override PlayerClassTypeV2 ClassType => PlayerClassTypeV2.Artificer;
        public override string ClassName => "Artificer";
        public override string ClassDescription => "Mechanical genius who deploys steam-powered gadgets and constructs.";
        public override ClassRole Role => ClassRole.Hybrid;
        public override string HomeRegion => "Ironclad";
        public override int ClassHue => 2305;

        public override int StartStr => 30;
        public override int StartDex => 30;
        public override int StartInt => 40;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Tinkering,
            SkillName.Blacksmith,
            SkillName.Mining,
            SkillName.ItemID,
            SkillName.Tactics,
            SkillName.MagicResist
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, 45.0, 40.0, 35.0, 30.0, 20.0
        };

        public override SkillName ClassSkill => SkillName.Engineering;

        public override PrimaryResourceType PrimaryResource => PrimaryResourceType.Both;
        public override ResourceType? SecondaryResource => ResourceType.Steam;
        public override int SecondaryResourceMax => 100;

        public override AbilitySchool AbilitySchool => AbilitySchool.Artificer;

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new LeatherChest() { Name = "Engineer's Vest", Hue = ClassHue });
            m.AddItem(new LeatherLegs() { Hue = ClassHue });
            m.AddItem(new LeatherGloves() { Name = "Work Gloves", Hue = ClassHue });
            m.AddItem(new LeatherArms() { Hue = ClassHue });
            m.AddItem(new LeatherCap() { Name = "Goggles", Hue = ClassHue });
            m.AddItem(new Boots() { Hue = ClassHue });
            m.AddItem(new Server.Items.Vystia.ArtificerPrecisionHammer(15) { Name = "Artificer's Precision Hammer", Hue = ClassHue });

            m.Backpack.DropItem(new EngineeringToolKit());
            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            // Artificer abilities would go here
        }
    }

    /// <summary>
    /// Alchemist - Potions and transmutation
    /// </summary>
    public class AlchemistClassV2 : PlayerClassV2
    {
        public override PlayerClassTypeV2 ClassType => PlayerClassTypeV2.Alchemist;
        public override string ClassName => "Alchemist";
        public override string ClassDescription => "Master of potions and transmutation who creates powerful elixirs.";
        public override ClassRole Role => ClassRole.Support;
        public override string HomeRegion => "Verdantpeak";
        public override int ClassHue => 2010;

        public override int StartStr => 25;
        public override int StartDex => 25;
        public override int StartInt => 50;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Alchemy,
            SkillName.TasteID,
            SkillName.Magery,
            SkillName.ItemID,
            SkillName.Poisoning,
            SkillName.Healing
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, 45.0, 40.0, 35.0, 30.0, 30.0
        };

        public override SkillName ClassSkill => SkillName.Transmutation;

        public override PrimaryResourceType PrimaryResource => PrimaryResourceType.Mana;

        public override AbilitySchool AbilitySchool => AbilitySchool.Alchemist;

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new Robe() { Name = "Alchemist's Robe", Hue = ClassHue });
            m.AddItem(new Cloak() { Name = "Lab Coat", Hue = ClassHue });
            m.AddItem(new HalfApron() { Name = "Alchemist's Apron", Hue = ClassHue });
            m.AddItem(new Sandals() { Hue = ClassHue });
            m.AddItem(new GnarledStaff() { Name = "Mixing Rod", Hue = ClassHue });

            m.Backpack.DropItem(new MortarPestle());
            m.Backpack.DropItem(new Bottle(20));
            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            // Alchemist abilities would go here
        }
    }

    /// <summary>
    /// Cleric - Faith healing and holy magic
    /// </summary>
    public class ClericClassV2 : PlayerClassV2
    {
        public override PlayerClassTypeV2 ClassType => PlayerClassTypeV2.Cleric;
        public override string ClassName => "Cleric";
        public override string ClassDescription => "Divine healer who channels Faith to mend wounds and smite evil.";
        public override ClassRole Role => ClassRole.Healer;
        public override string HomeRegion => "Multi-Regional";
        public override int ClassHue => 1153;

        public override int StartStr => 25;
        public override int StartDex => 20;
        public override int StartInt => 55;
        public override int IntCap => 145;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Magery,
            SkillName.EvalInt,
            SkillName.Meditation,
            SkillName.Healing,
            SkillName.SpiritSpeak,
            SkillName.MagicResist
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, 45.0, 40.0, 40.0, 30.0, 25.0
        };

        public override SkillName ClassSkill => SkillName.DivineGrace;

        public override PrimaryResourceType PrimaryResource => PrimaryResourceType.Mana;
        public override ResourceType? SecondaryResource => ResourceType.Faith;
        public override int SecondaryResourceMax => 100;

        public override AbilitySchool AbilitySchool => AbilitySchool.Cleric;

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new Robe() { Name = "Cleric's Vestments", Hue = ClassHue });
            m.AddItem(new Cloak() { Name = "Holy Mantle", Hue = ClassHue });
            m.AddItem(new WideBrimHat() { Hue = ClassHue });
            m.AddItem(new Sandals() { Hue = ClassHue });
            m.AddItem(new Scepter() { Name = "Staff of Healing", Hue = ClassHue });

            m.Backpack.DropItem(new Bandage(50));
            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            // Cleric spellbook would go here
        }

        public override void OnHeal(Mobile healer, Mobile target, int amount)
        {
            // Generate Faith on heals
            if (healer is PlayerMobile pm)
            {
                var manager = VystiaResourceManager.GetManager(pm);
                if (manager != null)
                {
                    var resource = manager.GetResource(ResourceType.Faith);
                    if (resource != null)
                    {
                        int gain = Math.Max(1, amount / 5);
                        resource.Generate(gain);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Wizard - Standard magery specialist
    /// </summary>
    public class WizardClassV2 : PlayerClassV2
    {
        public override PlayerClassTypeV2 ClassType => PlayerClassTypeV2.Wizard;
        public override string ClassName => "Wizard";
        public override string ClassDescription => "Traditional arcane practitioner who masters the fundamental arts of magic.";
        public override ClassRole Role => ClassRole.CasterDPS;
        public override string HomeRegion => "Crystal Barrens";
        public override int ClassHue => 1154;

        public override int StartStr => 20;
        public override int StartDex => 20;
        public override int StartInt => 60;
        public override int IntCap => 150;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Magery,
            SkillName.EvalInt,
            SkillName.Meditation,
            SkillName.Inscribe,
            SkillName.MagicResist,
            SkillName.Wrestling
        };

        public override double[] StartingSkillValues => new double[]
        {
            55.0, 50.0, 45.0, 30.0, 25.0, 15.0
        };

        public override SkillName ClassSkill => SkillName.ArcaneStudies;

        public override PrimaryResourceType PrimaryResource => PrimaryResourceType.Mana;

        public override AbilitySchool AbilitySchool => AbilitySchool.Wizard;

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new Robe() { Name = "Arcane Robe", Hue = ClassHue });
            m.AddItem(new WizardsHat() { Hue = ClassHue });
            m.AddItem(new Cloak() { Name = "Mage's Cloak", Hue = ClassHue });
            m.AddItem(new Sandals() { Hue = ClassHue });
            m.AddItem(new QuarterStaff() { Name = "Arcane Staff", Hue = ClassHue });

            m.Backpack.DropItem(new Spellbook());
            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            // Standard spellbook would go here
        }
    }

    #endregion

    #region Class Manager

    /// <summary>
    /// Manages player class assignments
    /// </summary>
    public class VystiaClassManager
    {
        private static VystiaClassManager m_Instance;
        public static VystiaClassManager Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new VystiaClassManager();
                return m_Instance;
            }
        }

        // Player class assignments
        private Dictionary<Mobile, PlayerClassV2> m_ClassAssignments;

        private VystiaClassManager()
        {
            m_ClassAssignments = new Dictionary<Mobile, PlayerClassV2>();

            // Register all class implementations
            RegisterClasses();
        }

        /// <summary>
        /// Register all class implementations
        /// </summary>
        private void RegisterClasses()
        {
            // Core Magic Classes (6)
            PlayerClassV2.RegisterClass(PlayerClassTypeV2.IceMage, typeof(IceMageClassV2));
            PlayerClassV2.RegisterClass(PlayerClassTypeV2.Warlock, typeof(WarlockClassV2));
            PlayerClassV2.RegisterClass(PlayerClassTypeV2.Necromancer, typeof(NecromancerClassV2));
            PlayerClassV2.RegisterClass(PlayerClassTypeV2.Druid, typeof(DruidClassV2));
            PlayerClassV2.RegisterClass(PlayerClassTypeV2.Sorcerer, typeof(SorcererClassV2));
            // Bard is always registered; behavior still gated by VYSTIA_SONGWEAVING inside the class
            PlayerClassV2.RegisterClass(PlayerClassTypeV2.Bard, typeof(BardClassV2));

            // Core Martial Classes (6)
            PlayerClassV2.RegisterClass(PlayerClassTypeV2.Barbarian, typeof(BarbarianClassV2));
            PlayerClassV2.RegisterClass(PlayerClassTypeV2.Rogue, typeof(RogueClassV2));
            PlayerClassV2.RegisterClass(PlayerClassTypeV2.Monk, typeof(MonkClassV2));
            PlayerClassV2.RegisterClass(PlayerClassTypeV2.Knight, typeof(KnightClassV2));
            PlayerClassV2.RegisterClass(PlayerClassTypeV2.Paladin, typeof(PaladinClassV2));
            PlayerClassV2.RegisterClass(PlayerClassTypeV2.Ranger, typeof(RangerClassV2));

            // Extended Magic Classes (6)
            PlayerClassV2.RegisterClass(PlayerClassTypeV2.Witch, typeof(WitchClassV2));
            PlayerClassV2.RegisterClass(PlayerClassTypeV2.Oracle, typeof(OracleClassV2));
            PlayerClassV2.RegisterClass(PlayerClassTypeV2.Summoner, typeof(SummonerClassV2));
            PlayerClassV2.RegisterClass(PlayerClassTypeV2.Shaman, typeof(ShamanClassV2));
            PlayerClassV2.RegisterClass(PlayerClassTypeV2.Enchanter, typeof(EnchanterClassV2));
            PlayerClassV2.RegisterClass(PlayerClassTypeV2.Illusionist, typeof(IllusionistClassV2));

            // Extended Martial Classes (8)
            PlayerClassV2.RegisterClass(PlayerClassTypeV2.Fighter, typeof(FighterClassV2));
            PlayerClassV2.RegisterClass(PlayerClassTypeV2.Templar, typeof(TemplarClassV2));
            PlayerClassV2.RegisterClass(PlayerClassTypeV2.BountyHunter, typeof(BountyHunterClassV2));
            PlayerClassV2.RegisterClass(PlayerClassTypeV2.Beastmaster, typeof(BeastmasterClassV2));
            PlayerClassV2.RegisterClass(PlayerClassTypeV2.Artificer, typeof(ArtificerClassV2));
            PlayerClassV2.RegisterClass(PlayerClassTypeV2.Alchemist, typeof(AlchemistClassV2));
            PlayerClassV2.RegisterClass(PlayerClassTypeV2.Cleric, typeof(ClericClassV2));
            PlayerClassV2.RegisterClass(PlayerClassTypeV2.Wizard, typeof(WizardClassV2));
        }

        /// <summary>
        /// Assign a class to a mobile
        /// </summary>
        public bool AssignClass(Mobile mobile, PlayerClassTypeV2 classType)
        {
            if (mobile == null)
                return false;

            // Get class instance
            PlayerClassV2 playerClass = PlayerClassV2.GetClass(classType);
            if (playerClass == null)
            {
                mobile.SendMessage(0x22, "That class is not yet implemented.");
                return false;
            }

            // Remove existing class if any
            if (m_ClassAssignments.ContainsKey(mobile))
            {
                RemoveClass(mobile);
            }

            StripEquipmentForClass(mobile);

            // Initialize the class
            playerClass.InitializeClass(mobile);

            // Store assignment
            m_ClassAssignments[mobile] = playerClass;

            if (mobile is PlayerMobile pm)
            {
                pm.VystiaClassV2 = classType;
            }

            mobile.SendMessage(0x3B2, $"You are now a {playerClass.ClassName}!");
            return true;
        }

        private static void StripEquipmentForClass(Mobile mobile)
        {
            if (mobile == null || mobile.Backpack == null)
                return;

            List<Item> toMove = new List<Item>();

            foreach (Item item in mobile.Items)
            {
                if (item == null || item.Deleted || item.Parent != mobile)
                    continue;

                switch (item.Layer)
                {
                    case Layer.Backpack:
                    case Layer.Hair:
                    case Layer.FacialHair:
                    case Layer.Mount:
                    case Layer.ShopBuy:
                    case Layer.ShopResale:
                    case Layer.ShopSell:
                    case Layer.Bank:
                    case Layer.SecureTrade:
                        continue;
                }

                toMove.Add(item);
            }

            foreach (Item item in toMove)
            {
                if (item == null || item.Deleted)
                    continue;

                if (!mobile.Backpack.TryDropItem(mobile, item, false))
                {
                    mobile.Backpack.DropItem(item);
                }
            }
        }

        /// <summary>
        /// Remove class from a mobile
        /// </summary>
        public bool RemoveClass(Mobile mobile)
        {
            if (mobile == null || !m_ClassAssignments.ContainsKey(mobile))
                return false;

            PlayerClassV2 oldClass = m_ClassAssignments[mobile];
            oldClass.RemovePassives(mobile);

            // Remove all stances
            VystiaStanceManager.Instance.RemoveAllStances(mobile);

            m_ClassAssignments.Remove(mobile);

            if (mobile is PlayerMobile pm)
            {
                pm.VystiaClassV2 = PlayerClassTypeV2.None;
            }

            mobile.SendMessage(0x3B2, $"You are no longer a {oldClass.ClassName}.");
            return true;
        }

        /// <summary>
        /// Restore a player's class assignment from saved state without reinitializing stats/gear.
        /// </summary>
        public void RestoreClass(PlayerMobile pm)
        {
            if (pm == null || pm.VystiaClassV2 == PlayerClassTypeV2.None)
                return;

            if (m_ClassAssignments.ContainsKey(pm))
                return;

            var playerClass = PlayerClassV2.GetClass(pm.VystiaClassV2);
            if (playerClass == null)
                return;

            m_ClassAssignments[pm] = playerClass;
            playerClass.InitializeRuntimeState(pm);
        }

        /// <summary>
        /// Get the class assigned to a mobile
        /// </summary>
        public PlayerClassV2 GetClass(Mobile mobile)
        {
            if (mobile == null || !m_ClassAssignments.ContainsKey(mobile))
                return null;

            return m_ClassAssignments[mobile];
        }

        /// <summary>
        /// Check if mobile has a class
        /// </summary>
        public bool HasClass(Mobile mobile)
        {
            return mobile != null && m_ClassAssignments.ContainsKey(mobile);
        }

        /// <summary>
        /// Get class type for a mobile
        /// </summary>
        public PlayerClassTypeV2 GetClassType(Mobile mobile)
        {
            var playerClass = GetClass(mobile);
            return playerClass?.ClassType ?? PlayerClassTypeV2.None;
        }

        #region Combat Hook Forwarding

        /// <summary>
        /// Forward OnKill event to class
        /// </summary>
        public void OnKill(Mobile killer, Mobile victim)
        {
            var playerClass = GetClass(killer);
            playerClass?.OnKill(killer, victim);
        }

        /// <summary>
        /// Forward OnCrit event to class
        /// </summary>
        public void OnCrit(Mobile attacker, Mobile target, int damage)
        {
            var playerClass = GetClass(attacker);
            playerClass?.OnCrit(attacker, target, damage);
        }

        /// <summary>
        /// Forward OnDamageDealt event to class
        /// </summary>
        public void OnDamageDealt(Mobile attacker, Mobile target, int damage)
        {
            var playerClass = GetClass(attacker);
            playerClass?.OnDamageDealt(attacker, target, damage);
        }

        /// <summary>
        /// Forward OnDamageTaken event to class
        /// </summary>
        public void OnDamageTaken(Mobile victim, Mobile attacker, int damage)
        {
            var playerClass = GetClass(victim);
            playerClass?.OnDamageTaken(victim, attacker, damage);
        }

        /// <summary>
        /// Forward OnBlock event to class
        /// </summary>
        public void OnBlock(Mobile blocker, Mobile attacker)
        {
            var playerClass = GetClass(blocker);
            playerClass?.OnBlock(blocker, attacker);
        }

        /// <summary>
        /// Forward OnHeal event to class
        /// </summary>
        public void OnHeal(Mobile healer, Mobile target, int amount)
        {
            var playerClass = GetClass(healer);
            playerClass?.OnHeal(healer, target, amount);
        }

        #endregion
    }

    #endregion

    #region GM Commands

    /// <summary>
    /// GM commands for the class system
    /// </summary>
    public class ClassSystemCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("SetClassV2", AccessLevel.GameMaster, new CommandEventHandler(SetClass_OnCommand));
            CommandSystem.Register("RemoveClassV2", AccessLevel.GameMaster, new CommandEventHandler(RemoveClass_OnCommand));
            CommandSystem.Register("ClassInfoV2", AccessLevel.GameMaster, new CommandEventHandler(ClassInfo_OnCommand));
            CommandSystem.Register("ListClassesV2", AccessLevel.GameMaster, new CommandEventHandler(ListClasses_OnCommand));
        }

        [Usage("SetClassV2 <classtype>")]
        [Description("Assign a v2.0 class to yourself.")]
        private static void SetClass_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Length < 1)
            {
                from.SendMessage("Usage: [SetClassV2 <classtype>");
                from.SendMessage("Available classes:");
                foreach (PlayerClassTypeV2 type in Enum.GetValues(typeof(PlayerClassTypeV2)))
                {
                    if (type != PlayerClassTypeV2.None)
                        from.SendMessage($"  {type}");
                }
                return;
            }

            string className = e.GetString(0);
            if (!Enum.TryParse(className, true, out PlayerClassTypeV2 classType))
            {
                from.SendMessage($"Unknown class type: {className}");
                return;
            }

            if (VystiaClassManager.Instance.AssignClass(from, classType))
            {
                from.SendMessage($"Successfully assigned {classType} class.");
            }
        }

        [Usage("RemoveClassV2")]
        [Description("Remove your v2.0 class.")]
        private static void RemoveClass_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (VystiaClassManager.Instance.RemoveClass(from))
            {
                from.SendMessage("Class removed.");
            }
            else
            {
                from.SendMessage("You don't have a class assigned.");
            }
        }

        [Usage("ClassInfoV2")]
        [Description("Get information about your current class.")]
        private static void ClassInfo_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            PlayerClassV2 playerClass = VystiaClassManager.Instance.GetClass(from);
            if (playerClass == null)
            {
                from.SendMessage("You don't have a class assigned.");
                return;
            }

            from.SendMessage($"=== {playerClass.ClassName} ===");
            from.SendMessage($"Type: {playerClass.ClassType}");
            from.SendMessage($"Role: {playerClass.Role}");
            from.SendMessage($"Region: {playerClass.HomeRegion}");
            from.SendMessage($"Description: {playerClass.ClassDescription}");
            from.SendMessage($"Primary Resource: {playerClass.PrimaryResource}");

            if (playerClass.SecondaryResource.HasValue)
            {
                from.SendMessage($"Secondary Resource: {playerClass.SecondaryResource.Value} (max {playerClass.SecondaryResourceMax})");
            }

            if (playerClass.AvailableStances != null && playerClass.AvailableStances.Length > 0)
            {
                from.SendMessage($"Stances: {string.Join(", ", playerClass.AvailableStances)}");
            }

            from.SendMessage($"Ability School: {playerClass.AbilitySchool}");
        }

        [Usage("ListClassesV2")]
        [Description("List all available v2.0 classes.")]
        private static void ListClasses_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            from.SendMessage("=== Available Classes (26 Total) ===");
            from.SendMessage("");
            from.SendMessage("Magic Classes (12):");
            from.SendMessage("  Core: IceMage, Warlock, Necromancer, Druid, Sorcerer, Bard");
            from.SendMessage("  Extended: Witch, Oracle, Summoner, Shaman, Enchanter, Illusionist");
            from.SendMessage("");
            from.SendMessage("Martial Classes (14):");
            from.SendMessage("  Core: Barbarian, Rogue, Monk, Knight, Paladin, Ranger");
            from.SendMessage("  Extended: Fighter, Templar, BountyHunter, Beastmaster, Artificer, Alchemist, Cleric, Wizard");
            from.SendMessage("");
            from.SendMessage("Use [SetClassV2 <classname> to assign a class.");
        }
    }

    #endregion
}
