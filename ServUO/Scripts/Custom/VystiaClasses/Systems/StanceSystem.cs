/*
 * Vystia Class System v2.0
 * Stance & Form System
 *
 * This file implements stances, forms, and transformations for Vystia classes.
 * Stances modify stats, unlock abilities, and can trigger combo effects.
 *
 * Key Classes:
 * - Druid: Bear, Cat, Tree, Moonkin, Travel forms
 * - Sorcerer: Fire, Water, Earth, Air elemental stances
 * - Fighter: Aggressive, Defensive, Balanced, Berserker stances
 * - Barbarian: Normal, Rage transformation
 */

using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Network;
using Server.Commands;

namespace Server.Custom.VystiaClasses.Systems
{
    #region Enums

    /// <summary>
    /// All stance types in the Vystia class system
    /// </summary>
    public enum StanceType
    {
        None = 0,

        // Druid Forms (transform with body change)
        DruidBear,          // Tank form - +STR, +HP, -speed
        DruidCat,           // DPS form - +DEX, +crit, stealth
        DruidTree,          // Healer form - +INT, +healing, rooted
        DruidMoonkin,       // Caster form - +INT, +spell damage
        DruidTravel,        // Movement form - +speed, no combat

        // Sorcerer Elements (stance with visual aura)
        SorcererFire,       // Fire spells enhanced, +fire damage
        SorcererWater,      // Water spells enhanced, +cold damage, healing
        SorcererEarth,      // Earth spells enhanced, +armor, slow but strong
        SorcererAir,        // Air spells enhanced, +speed, +evasion

        // Fighter Stances (combat style)
        FighterAggressive,  // +damage, -defense
        FighterDefensive,   // +defense, -damage
        FighterBalanced,    // Balanced stats
        FighterBerserker,   // High risk/reward, +damage, -armor

        // Barbarian States
        BarbarianNormal,    // Default state
        BarbarianRage,      // Rage transformation - +STR, +damage, -INT

        // Monk Stances
        MonkWindwalker,     // Speed and evasion
        MonkBrewmaster,     // Tank with stagger
        MonkMistweaver,     // Healing stance

        // Rogue Stances
        RogueShadow,        // Stealth and burst damage
        RogueOutlaw,        // Dual wield, sustained damage
        RogueSubtlety,      // Control and DoTs

        // Paladin Auras
        PaladinDevotion,    // Healing aura
        PaladinRetribution, // Damage reflection
        PaladinProtection,  // Damage reduction aura

        // Ranger Aspects
        RangerHawk,         // Ranged accuracy
        RangerWolf,         // Pack tactics, speed
        RangerBear,         // Survival, HP regen
    }

    /// <summary>
    /// Categories of stances for UI and logic grouping
    /// </summary>
    public enum StanceCategory
    {
        None,
        DruidForm,          // Body-changing transformation
        ElementalStance,    // Elemental attunement
        CombatStance,       // Fighting style
        Transformation,     // Full transformation (Rage, etc.)
        MartialStance,      // Martial arts stance
        Aura,               // Passive aura effect
        Aspect,             // Nature aspect
    }

    /// <summary>
    /// Flags for stance behavior
    /// </summary>
    [Flags]
    public enum StanceFlags
    {
        None = 0,
        TransformBody = 1 << 0,      // Changes character body
        PreventsCombat = 1 << 1,     // Cannot attack while in stance
        PreventsSpellcast = 1 << 2,  // Cannot cast spells
        PreventsMelee = 1 << 3,      // Cannot melee attack
        PreventsRanged = 1 << 4,     // Cannot use ranged
        AllowsStealth = 1 << 5,      // Can stealth in this stance
        BreaksOnDamage = 1 << 6,     // Stance breaks when taking damage
        BreaksOnAction = 1 << 7,     // Stance breaks on any action
        Rooted = 1 << 8,             // Cannot move
        HasAura = 1 << 9,            // Displays visual aura
        HasParticles = 1 << 10,      // Continuous particle effects
        Exclusive = 1 << 11,         // Removes other stances when applied
    }

    #endregion

    #region Stat Modifiers

    /// <summary>
    /// Stat modifications applied by a stance
    /// </summary>
    public class StanceStatModifiers
    {
        // Base stats
        public int StrBonus { get; set; }
        public int DexBonus { get; set; }
        public int IntBonus { get; set; }

        // Derived stats
        public int MaxHPBonus { get; set; }
        public int MaxManaBonus { get; set; }
        public int MaxStaminaBonus { get; set; }

        // Combat modifiers (percentage)
        public int DamageModifier { get; set; }      // +/-% damage
        public int HealingModifier { get; set; }     // +/-% healing done
        public int SpellDamageModifier { get; set; } // +/-% spell damage
        public int MeleeDamageModifier { get; set; } // +/-% melee damage
        public int RangedDamageModifier { get; set; }// +/-% ranged damage
        public int CritChanceBonus { get; set; }     // +/-% crit chance
        public int CritDamageBonus { get; set; }     // +/-% crit damage
        public int AttackSpeedModifier { get; set; } // +/-% attack speed
        public int CastSpeedModifier { get; set; }   // +/-% cast speed

        // Defense modifiers
        public int ArmorBonus { get; set; }          // Flat armor bonus
        public int ArmorModifier { get; set; }       // +/-% armor
        public int DamageReduction { get; set; }     // Flat damage reduction
        public int DodgeChanceBonus { get; set; }    // +% dodge
        public int BlockChanceBonus { get; set; }    // +% block
        public int ParryChanceBonus { get; set; }    // +% parry

        // Resistances (flat bonuses)
        public int PhysicalResist { get; set; }
        public int FireResist { get; set; }
        public int ColdResist { get; set; }
        public int PoisonResist { get; set; }
        public int EnergyResist { get; set; }

        // Movement
        public int MovementSpeedModifier { get; set; } // +/-% movement speed

        // Resource modifiers
        public int ResourceGenModifier { get; set; } // +/-% secondary resource gen
        public int ManaRegenModifier { get; set; }   // +/-% mana regen
        public int HPRegenModifier { get; set; }     // +/-% HP regen
        public int StaminaRegenModifier { get; set; }// +/-% stamina regen

        // Threat
        public int ThreatModifier { get; set; }      // +/-% threat generated

        /// <summary>
        /// Create a copy of these modifiers
        /// </summary>
        public StanceStatModifiers Clone()
        {
            return (StanceStatModifiers)this.MemberwiseClone();
        }

        /// <summary>
        /// Combine two modifier sets
        /// </summary>
        public static StanceStatModifiers Combine(StanceStatModifiers a, StanceStatModifiers b)
        {
            if (a == null) return b?.Clone();
            if (b == null) return a.Clone();

            return new StanceStatModifiers
            {
                StrBonus = a.StrBonus + b.StrBonus,
                DexBonus = a.DexBonus + b.DexBonus,
                IntBonus = a.IntBonus + b.IntBonus,
                MaxHPBonus = a.MaxHPBonus + b.MaxHPBonus,
                MaxManaBonus = a.MaxManaBonus + b.MaxManaBonus,
                MaxStaminaBonus = a.MaxStaminaBonus + b.MaxStaminaBonus,
                DamageModifier = a.DamageModifier + b.DamageModifier,
                HealingModifier = a.HealingModifier + b.HealingModifier,
                SpellDamageModifier = a.SpellDamageModifier + b.SpellDamageModifier,
                MeleeDamageModifier = a.MeleeDamageModifier + b.MeleeDamageModifier,
                RangedDamageModifier = a.RangedDamageModifier + b.RangedDamageModifier,
                CritChanceBonus = a.CritChanceBonus + b.CritChanceBonus,
                CritDamageBonus = a.CritDamageBonus + b.CritDamageBonus,
                AttackSpeedModifier = a.AttackSpeedModifier + b.AttackSpeedModifier,
                CastSpeedModifier = a.CastSpeedModifier + b.CastSpeedModifier,
                ArmorBonus = a.ArmorBonus + b.ArmorBonus,
                ArmorModifier = a.ArmorModifier + b.ArmorModifier,
                DamageReduction = a.DamageReduction + b.DamageReduction,
                DodgeChanceBonus = a.DodgeChanceBonus + b.DodgeChanceBonus,
                BlockChanceBonus = a.BlockChanceBonus + b.BlockChanceBonus,
                ParryChanceBonus = a.ParryChanceBonus + b.ParryChanceBonus,
                PhysicalResist = a.PhysicalResist + b.PhysicalResist,
                FireResist = a.FireResist + b.FireResist,
                ColdResist = a.ColdResist + b.ColdResist,
                PoisonResist = a.PoisonResist + b.PoisonResist,
                EnergyResist = a.EnergyResist + b.EnergyResist,
                MovementSpeedModifier = a.MovementSpeedModifier + b.MovementSpeedModifier,
                ResourceGenModifier = a.ResourceGenModifier + b.ResourceGenModifier,
                ManaRegenModifier = a.ManaRegenModifier + b.ManaRegenModifier,
                HPRegenModifier = a.HPRegenModifier + b.HPRegenModifier,
                StaminaRegenModifier = a.StaminaRegenModifier + b.StaminaRegenModifier,
                ThreatModifier = a.ThreatModifier + b.ThreatModifier,
            };
        }
    }

    #endregion

    #region Stance Definition

    /// <summary>
    /// Defines a stance, form, or transformation
    /// </summary>
    public class StanceDefinition
    {
        // Identity
        public StanceType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public StanceCategory Category { get; set; }
        public StanceFlags Flags { get; set; }

        // Visual
        public int BodyValue { get; set; }           // Body to transform into (0 = no change)
        public int OriginalBody { get; set; }        // Stored original body
        public int Hue { get; set; }                 // Hue to apply (0 = no change)
        public int AuraEffect { get; set; }          // Aura particle effect ID
        public int AuraHue { get; set; }             // Aura hue
        public int TransformSound { get; set; }      // Sound on enter
        public int ExitSound { get; set; }           // Sound on exit

        // Timing
        public TimeSpan Duration { get; set; }       // 0 = permanent until switched
        public TimeSpan Cooldown { get; set; }       // Cooldown before can re-enter
        public TimeSpan TransformTime { get; set; }  // Time to transform (can be interrupted)

        // Stats
        public StanceStatModifiers Modifiers { get; set; }

        // Abilities
        public List<int> AllowedAbilities { get; set; }    // Ability IDs allowed in this stance
        public List<int> BlockedAbilities { get; set; }    // Ability IDs blocked in this stance
        public List<int> GrantedAbilities { get; set; }    // Abilities only available in this stance

        // Requirements
        public int MinLevel { get; set; }
        public int ResourceCost { get; set; }              // Mana/stamina to enter
        public ResourceType SecondaryResourceCost { get; set; }
        public int SecondaryResourceAmount { get; set; }

        // Exclusive groups
        public string ExclusiveGroup { get; set; }         // Stances in same group are mutually exclusive

        public StanceDefinition()
        {
            Modifiers = new StanceStatModifiers();
            AllowedAbilities = new List<int>();
            BlockedAbilities = new List<int>();
            GrantedAbilities = new List<int>();
            Duration = TimeSpan.Zero;
            Cooldown = TimeSpan.FromSeconds(1.5);
            TransformTime = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// Check if this stance transforms the body
        /// </summary>
        public bool IsTransformation => (Flags & StanceFlags.TransformBody) != 0;

        /// <summary>
        /// Check if this stance has a duration
        /// </summary>
        public bool HasDuration => Duration > TimeSpan.Zero;

        /// <summary>
        /// Check if stance is exclusive (removes other stances)
        /// </summary>
        public bool IsExclusive => (Flags & StanceFlags.Exclusive) != 0;
    }

    #endregion

    #region Active Stance Instance

    /// <summary>
    /// Represents an active stance on a mobile
    /// </summary>
    public class ActiveStance
    {
        public StanceDefinition Definition { get; private set; }
        public Mobile Owner { get; private set; }
        public DateTime AppliedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public int OriginalBody { get; private set; }
        public int OriginalHue { get; private set; }
        public List<StatMod> AppliedMods { get; private set; }
        public Timer ExpirationTimer { get; set; }
        public Timer AuraTimer { get; set; }

        public ActiveStance(Mobile owner, StanceDefinition definition)
        {
            Owner = owner;
            Definition = definition;
            AppliedAt = DateTime.UtcNow;
            AppliedMods = new List<StatMod>();

            if (definition.HasDuration)
            {
                ExpiresAt = AppliedAt + definition.Duration;
            }
            else
            {
                ExpiresAt = DateTime.MaxValue;
            }
        }

        /// <summary>
        /// Time remaining on this stance
        /// </summary>
        public TimeSpan TimeRemaining
        {
            get
            {
                if (!Definition.HasDuration)
                    return TimeSpan.MaxValue;

                var remaining = ExpiresAt - DateTime.UtcNow;
                return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Check if stance has expired
        /// </summary>
        public bool IsExpired => Definition.HasDuration && DateTime.UtcNow >= ExpiresAt;

        /// <summary>
        /// Store original appearance
        /// </summary>
        public void StoreOriginalAppearance()
        {
            OriginalBody = Owner.Body;
            OriginalHue = Owner.Hue;
        }

        /// <summary>
        /// Restore original appearance
        /// </summary>
        public void RestoreOriginalAppearance()
        {
            if (Definition.IsTransformation && OriginalBody != 0)
            {
                Owner.Body = OriginalBody;
            }

            if (Definition.Hue != 0)
            {
                Owner.Hue = OriginalHue;
            }
        }

        /// <summary>
        /// Extend the duration of this stance
        /// </summary>
        public void ExtendDuration(TimeSpan extension)
        {
            if (!Definition.HasDuration)
                return;

            ExpiresAt = ExpiresAt.Add(extension);

            // Restart timer
            if (ExpirationTimer != null)
            {
                ExpirationTimer.Stop();
                ExpirationTimer = Timer.DelayCall(TimeRemaining, () =>
                {
                    VystiaStanceManager.Instance.RemoveStance(Owner, Definition.Type);
                });
            }
        }
    }

    #endregion

    #region Stance Manager

    /// <summary>
    /// Singleton manager for all stance operations
    /// </summary>
    public class VystiaStanceManager
    {
        private static VystiaStanceManager m_Instance;
        public static VystiaStanceManager Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new VystiaStanceManager();
                return m_Instance;
            }
        }

        // Registered stance definitions
        private Dictionary<StanceType, StanceDefinition> m_Definitions;

        // Active stances per mobile
        private Dictionary<Mobile, Dictionary<StanceType, ActiveStance>> m_ActiveStances;

        // Cooldowns per mobile
        private Dictionary<Mobile, Dictionary<StanceType, DateTime>> m_Cooldowns;

        private VystiaStanceManager()
        {
            m_Definitions = new Dictionary<StanceType, StanceDefinition>();
            m_ActiveStances = new Dictionary<Mobile, Dictionary<StanceType, ActiveStance>>();
            m_Cooldowns = new Dictionary<Mobile, Dictionary<StanceType, DateTime>>();

            // Register default stances
            RegisterDefaultStances();
        }

        #region Registration

        /// <summary>
        /// Register a stance definition
        /// </summary>
        public void RegisterStance(StanceDefinition definition)
        {
            if (definition == null || definition.Type == StanceType.None)
                return;

            m_Definitions[definition.Type] = definition;
        }

        /// <summary>
        /// Get a stance definition
        /// </summary>
        public StanceDefinition GetDefinition(StanceType type)
        {
            if (m_Definitions.TryGetValue(type, out StanceDefinition def))
                return def;
            return null;
        }

        /// <summary>
        /// Get all registered stances
        /// </summary>
        public IEnumerable<StanceDefinition> GetAllDefinitions()
        {
            return m_Definitions.Values;
        }

        /// <summary>
        /// Get stances by category
        /// </summary>
        public List<StanceDefinition> GetStancesByCategory(StanceCategory category)
        {
            List<StanceDefinition> result = new List<StanceDefinition>();
            foreach (var def in m_Definitions.Values)
            {
                if (def.Category == category)
                    result.Add(def);
            }
            return result;
        }

        #endregion

        #region Stance Application

        /// <summary>
        /// Apply a stance to a mobile
        /// </summary>
        public bool ApplyStance(Mobile mobile, StanceType type, bool force = false)
        {
            if (mobile == null || mobile.Deleted)
                return false;

            StanceDefinition definition = GetDefinition(type);
            if (definition == null)
            {
                mobile.SendMessage(0x22, "Unknown stance type.");
                return false;
            }

            // Check cooldown
            if (!force && IsOnCooldown(mobile, type))
            {
                TimeSpan remaining = GetCooldownRemaining(mobile, type);
                mobile.SendMessage(0x22, $"That stance is on cooldown for {remaining.TotalSeconds:F1} seconds.");
                return false;
            }

            // Check requirements
            if (!force && !MeetsRequirements(mobile, definition))
            {
                return false;
            }

            // Pay costs
            if (!force && !PayCosts(mobile, definition))
            {
                mobile.SendMessage(0x22, "You don't have enough resources for that stance.");
                return false;
            }

            // Remove exclusive stances
            if (definition.IsExclusive || !string.IsNullOrEmpty(definition.ExclusiveGroup))
            {
                RemoveExclusiveStances(mobile, definition);
            }

            // Create active stance
            ActiveStance activeStance = new ActiveStance(mobile, definition);

            // Store original appearance before transform
            if (definition.IsTransformation)
            {
                activeStance.StoreOriginalAppearance();
            }

            // Apply transformation
            if (definition.IsTransformation && definition.BodyValue > 0)
            {
                mobile.Body = definition.BodyValue;
            }

            if (definition.Hue != 0)
            {
                mobile.Hue = definition.Hue;
            }

            // Apply stat modifiers
            ApplyStatModifiers(mobile, activeStance);

            // Play effects
            PlayEnterEffects(mobile, definition);

            // Store active stance
            if (!m_ActiveStances.ContainsKey(mobile))
                m_ActiveStances[mobile] = new Dictionary<StanceType, ActiveStance>();

            m_ActiveStances[mobile][type] = activeStance;

            // Start expiration timer if has duration
            if (definition.HasDuration)
            {
                activeStance.ExpirationTimer = Timer.DelayCall(definition.Duration, () =>
                {
                    RemoveStance(mobile, type);
                });
            }

            // Start aura timer if has aura
            if ((definition.Flags & StanceFlags.HasAura) != 0 || (definition.Flags & StanceFlags.HasParticles) != 0)
            {
                StartAuraEffects(activeStance);
            }

            // Notify
            mobile.SendMessage(0x3B2, $"You enter {definition.Name}.");

            // Trigger combo check
            VystiaStanceComboSystem.Instance.OnStanceEntered(mobile, type);

            return true;
        }

        /// <summary>
        /// Remove a stance from a mobile
        /// </summary>
        public bool RemoveStance(Mobile mobile, StanceType type)
        {
            if (mobile == null || mobile.Deleted)
                return false;

            if (!m_ActiveStances.ContainsKey(mobile))
                return false;

            if (!m_ActiveStances[mobile].TryGetValue(type, out ActiveStance activeStance))
                return false;

            // Stop timers
            if (activeStance.ExpirationTimer != null)
            {
                activeStance.ExpirationTimer.Stop();
                activeStance.ExpirationTimer = null;
            }

            if (activeStance.AuraTimer != null)
            {
                activeStance.AuraTimer.Stop();
                activeStance.AuraTimer = null;
            }

            // Remove stat modifiers
            RemoveStatModifiers(mobile, activeStance);

            // Restore appearance
            activeStance.RestoreOriginalAppearance();

            // Play exit effects
            PlayExitEffects(mobile, activeStance.Definition);

            // Remove from active stances
            m_ActiveStances[mobile].Remove(type);

            // Set cooldown
            SetCooldown(mobile, type, activeStance.Definition.Cooldown);

            // Notify
            mobile.SendMessage(0x3B2, $"You leave {activeStance.Definition.Name}.");

            // Trigger combo check
            VystiaStanceComboSystem.Instance.OnStanceExited(mobile, type);

            return true;
        }

        /// <summary>
        /// Remove all stances from a mobile
        /// </summary>
        public void RemoveAllStances(Mobile mobile)
        {
            if (mobile == null || !m_ActiveStances.ContainsKey(mobile))
                return;

            List<StanceType> toRemove = new List<StanceType>(m_ActiveStances[mobile].Keys);
            foreach (var type in toRemove)
            {
                RemoveStance(mobile, type);
            }
        }

        /// <summary>
        /// Toggle a stance (remove if active, apply if not)
        /// </summary>
        public bool ToggleStance(Mobile mobile, StanceType type)
        {
            if (HasStance(mobile, type))
                return RemoveStance(mobile, type);
            else
                return ApplyStance(mobile, type);
        }

        #endregion

        #region Queries

        /// <summary>
        /// Check if mobile has a specific stance
        /// </summary>
        public bool HasStance(Mobile mobile, StanceType type)
        {
            if (mobile == null || !m_ActiveStances.ContainsKey(mobile))
                return false;

            return m_ActiveStances[mobile].ContainsKey(type);
        }

        /// <summary>
        /// Get active stance of a specific type
        /// </summary>
        public ActiveStance GetActiveStance(Mobile mobile, StanceType type)
        {
            if (mobile == null || !m_ActiveStances.ContainsKey(mobile))
                return null;

            if (m_ActiveStances[mobile].TryGetValue(type, out ActiveStance stance))
                return stance;

            return null;
        }

        /// <summary>
        /// Get all active stances for a mobile
        /// </summary>
        public List<ActiveStance> GetAllActiveStances(Mobile mobile)
        {
            if (mobile == null || !m_ActiveStances.ContainsKey(mobile))
                return new List<ActiveStance>();

            return new List<ActiveStance>(m_ActiveStances[mobile].Values);
        }

        /// <summary>
        /// Check if mobile has any stance in a category
        /// </summary>
        public bool HasStanceInCategory(Mobile mobile, StanceCategory category)
        {
            if (mobile == null || !m_ActiveStances.ContainsKey(mobile))
                return false;

            foreach (var stance in m_ActiveStances[mobile].Values)
            {
                if (stance.Definition.Category == category)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Get current stance in a category
        /// </summary>
        public ActiveStance GetStanceInCategory(Mobile mobile, StanceCategory category)
        {
            if (mobile == null || !m_ActiveStances.ContainsKey(mobile))
                return null;

            foreach (var stance in m_ActiveStances[mobile].Values)
            {
                if (stance.Definition.Category == category)
                    return stance;
            }

            return null;
        }

        /// <summary>
        /// Get combined stat modifiers from all active stances
        /// </summary>
        public StanceStatModifiers GetCombinedModifiers(Mobile mobile)
        {
            StanceStatModifiers result = new StanceStatModifiers();

            if (mobile == null || !m_ActiveStances.ContainsKey(mobile))
                return result;

            foreach (var stance in m_ActiveStances[mobile].Values)
            {
                result = StanceStatModifiers.Combine(result, stance.Definition.Modifiers);
            }

            return result;
        }

        #endregion

        #region Cooldowns

        /// <summary>
        /// Check if a stance is on cooldown
        /// </summary>
        public bool IsOnCooldown(Mobile mobile, StanceType type)
        {
            if (mobile == null || !m_Cooldowns.ContainsKey(mobile))
                return false;

            if (!m_Cooldowns[mobile].TryGetValue(type, out DateTime expiry))
                return false;

            return DateTime.UtcNow < expiry;
        }

        /// <summary>
        /// Get remaining cooldown time
        /// </summary>
        public TimeSpan GetCooldownRemaining(Mobile mobile, StanceType type)
        {
            if (mobile == null || !m_Cooldowns.ContainsKey(mobile))
                return TimeSpan.Zero;

            if (!m_Cooldowns[mobile].TryGetValue(type, out DateTime expiry))
                return TimeSpan.Zero;

            TimeSpan remaining = expiry - DateTime.UtcNow;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }

        /// <summary>
        /// Set cooldown for a stance
        /// </summary>
        private void SetCooldown(Mobile mobile, StanceType type, TimeSpan duration)
        {
            if (mobile == null || duration <= TimeSpan.Zero)
                return;

            if (!m_Cooldowns.ContainsKey(mobile))
                m_Cooldowns[mobile] = new Dictionary<StanceType, DateTime>();

            m_Cooldowns[mobile][type] = DateTime.UtcNow + duration;
        }

        /// <summary>
        /// Reset all cooldowns for a mobile
        /// </summary>
        public void ResetCooldowns(Mobile mobile)
        {
            if (mobile != null && m_Cooldowns.ContainsKey(mobile))
                m_Cooldowns[mobile].Clear();
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Check if mobile meets stance requirements
        /// </summary>
        private bool MeetsRequirements(Mobile mobile, StanceDefinition definition)
        {
            // Level check
            if (mobile is PlayerMobile pm && definition.MinLevel > 0)
            {
                // Check skill level or other level metric
            }

            return true;
        }

        /// <summary>
        /// Pay resource costs for stance
        /// </summary>
        private bool PayCosts(Mobile mobile, StanceDefinition definition)
        {
            // Mana/Stamina cost
            if (definition.ResourceCost > 0)
            {
                if (mobile.Mana < definition.ResourceCost)
                    return false;

                mobile.Mana -= definition.ResourceCost;
            }

            // Secondary resource cost
            if (definition.SecondaryResourceAmount > 0 && mobile is PlayerMobile pm)
            {
                var manager = VystiaResourceManager.GetManager(pm);
                if (manager != null)
                {
                    var resource = manager.GetResource(definition.SecondaryResourceCost);
                    if (resource == null || !resource.CanSpend(definition.SecondaryResourceAmount))
                        return false;

                    resource.Spend(definition.SecondaryResourceAmount);
                }
            }

            return true;
        }

        /// <summary>
        /// Remove stances that are exclusive with the new stance
        /// </summary>
        private void RemoveExclusiveStances(Mobile mobile, StanceDefinition newStance)
        {
            if (!m_ActiveStances.ContainsKey(mobile))
                return;

            List<StanceType> toRemove = new List<StanceType>();

            foreach (var kvp in m_ActiveStances[mobile])
            {
                var existing = kvp.Value.Definition;

                // Same exclusive group
                if (!string.IsNullOrEmpty(newStance.ExclusiveGroup) &&
                    newStance.ExclusiveGroup == existing.ExclusiveGroup)
                {
                    toRemove.Add(kvp.Key);
                    continue;
                }

                // Same category for exclusive stances
                if (newStance.IsExclusive && existing.Category == newStance.Category)
                {
                    toRemove.Add(kvp.Key);
                }
            }

            foreach (var type in toRemove)
            {
                RemoveStance(mobile, type);
            }
        }

        /// <summary>
        /// Apply stat modifiers from a stance
        /// </summary>
        private void ApplyStatModifiers(Mobile mobile, ActiveStance stance)
        {
            StanceStatModifiers mods = stance.Definition.Modifiers;
            if (mods == null)
                return;

            string prefix = $"Stance_{stance.Definition.Type}";

            // STR/DEX/INT bonuses
            if (mods.StrBonus != 0)
            {
                StatMod mod = new StatMod(StatType.Str, $"{prefix}_Str", mods.StrBonus, TimeSpan.Zero);
                mobile.AddStatMod(mod);
                stance.AppliedMods.Add(mod);
            }

            if (mods.DexBonus != 0)
            {
                StatMod mod = new StatMod(StatType.Dex, $"{prefix}_Dex", mods.DexBonus, TimeSpan.Zero);
                mobile.AddStatMod(mod);
                stance.AppliedMods.Add(mod);
            }

            if (mods.IntBonus != 0)
            {
                StatMod mod = new StatMod(StatType.Int, $"{prefix}_Int", mods.IntBonus, TimeSpan.Zero);
                mobile.AddStatMod(mod);
                stance.AppliedMods.Add(mod);
            }

            // Other modifiers are checked during combat calculations
            // via GetCombinedModifiers()
        }

        /// <summary>
        /// Remove stat modifiers from a stance
        /// </summary>
        private void RemoveStatModifiers(Mobile mobile, ActiveStance stance)
        {
            foreach (var mod in stance.AppliedMods)
            {
                mobile.RemoveStatMod(mod.Name);
            }

            stance.AppliedMods.Clear();
        }

        /// <summary>
        /// Play visual/sound effects when entering stance
        /// </summary>
        private void PlayEnterEffects(Mobile mobile, StanceDefinition definition)
        {
            if (definition.TransformSound > 0)
                mobile.PlaySound(definition.TransformSound);

            // Transformation effect
            if (definition.IsTransformation)
            {
                mobile.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
            }

            // Aura effect
            if (definition.AuraEffect > 0)
            {
                Effects.SendLocationParticles(
                    EffectItem.Create(mobile.Location, mobile.Map, EffectItem.DefaultDuration),
                    definition.AuraEffect, 10, 30, definition.AuraHue, 0, 0, 0);
            }
        }

        /// <summary>
        /// Play visual/sound effects when exiting stance
        /// </summary>
        private void PlayExitEffects(Mobile mobile, StanceDefinition definition)
        {
            if (definition.ExitSound > 0)
                mobile.PlaySound(definition.ExitSound);

            if (definition.IsTransformation)
            {
                mobile.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
            }
        }

        /// <summary>
        /// Start continuous aura effects
        /// </summary>
        private void StartAuraEffects(ActiveStance stance)
        {
            if (stance.Definition.AuraEffect <= 0)
                return;

            stance.AuraTimer = Timer.DelayCall(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), () =>
            {
                if (stance.Owner == null || stance.Owner.Deleted)
                {
                    stance.AuraTimer?.Stop();
                    return;
                }

                // Subtle aura particle
                stance.Owner.FixedParticles(stance.Definition.AuraEffect, 1, 15, 9502,
                    stance.Definition.AuraHue, 0, EffectLayer.Waist);
            });
        }

        #endregion

        #region Default Stances

        /// <summary>
        /// Register all default stance definitions
        /// </summary>
        private void RegisterDefaultStances()
        {
            // Druid Forms
            RegisterDruidForms();

            // Sorcerer Elements
            RegisterSorcererElements();

            // Fighter Stances
            RegisterFighterStances();

            // Barbarian Rage
            RegisterBarbarianStances();

            // Monk Stances
            RegisterMonkStances();

            // Rogue Stances
            RegisterRogueStances();

            // Paladin Auras
            RegisterPaladinAuras();

            // Ranger Aspects
            RegisterRangerAspects();
        }

        private void RegisterDruidForms()
        {
            // Bear Form - Tank
            RegisterStance(new StanceDefinition
            {
                Type = StanceType.DruidBear,
                Name = "Bear Form",
                Description = "Transform into a mighty bear. Increased health and armor, but slower movement.",
                Category = StanceCategory.DruidForm,
                Flags = StanceFlags.TransformBody | StanceFlags.Exclusive | StanceFlags.PreventsMelee,
                BodyValue = 211,  // Bear body
                Hue = 0,
                TransformSound = 0x1CA,
                ExitSound = 0x1CA,
                ExclusiveGroup = "DruidForm",
                Modifiers = new StanceStatModifiers
                {
                    StrBonus = 20,
                    DexBonus = -10,
                    MaxHPBonus = 50,
                    ArmorModifier = 50,
                    DamageModifier = -20,
                    MovementSpeedModifier = -15,
                    ThreatModifier = 50,
                    PhysicalResist = 10,
                }
            });

            // Cat Form - DPS
            RegisterStance(new StanceDefinition
            {
                Type = StanceType.DruidCat,
                Name = "Cat Form",
                Description = "Transform into a swift cat. Increased speed and critical strikes. Can stealth.",
                Category = StanceCategory.DruidForm,
                Flags = StanceFlags.TransformBody | StanceFlags.Exclusive | StanceFlags.AllowsStealth,
                BodyValue = 201,  // Cat body
                Hue = 0,
                TransformSound = 0x69,
                ExitSound = 0x69,
                ExclusiveGroup = "DruidForm",
                Modifiers = new StanceStatModifiers
                {
                    DexBonus = 15,
                    StrBonus = 5,
                    CritChanceBonus = 15,
                    CritDamageBonus = 25,
                    AttackSpeedModifier = 20,
                    MovementSpeedModifier = 30,
                    DodgeChanceBonus = 10,
                }
            });

            // Tree Form - Healer
            RegisterStance(new StanceDefinition
            {
                Type = StanceType.DruidTree,
                Name = "Tree of Life",
                Description = "Transform into a tree of life. Powerful healing but rooted in place.",
                Category = StanceCategory.DruidForm,
                Flags = StanceFlags.TransformBody | StanceFlags.Exclusive | StanceFlags.Rooted | StanceFlags.PreventsMelee,
                BodyValue = 301,  // Treefellow body
                Hue = 2010,
                TransformSound = 0x1,
                ExclusiveGroup = "DruidForm",
                Modifiers = new StanceStatModifiers
                {
                    IntBonus = 20,
                    HealingModifier = 50,
                    ManaRegenModifier = 30,
                    ArmorModifier = 30,
                    MovementSpeedModifier = -100,  // Cannot move
                }
            });

            // Moonkin Form - Caster DPS
            RegisterStance(new StanceDefinition
            {
                Type = StanceType.DruidMoonkin,
                Name = "Moonkin Form",
                Description = "Transform into a moonkin. Enhanced spell damage and armor.",
                Category = StanceCategory.DruidForm,
                Flags = StanceFlags.TransformBody | StanceFlags.Exclusive | StanceFlags.PreventsMelee,
                BodyValue = 750,  // Ettin-like body for moonkin
                Hue = 1154,
                TransformSound = 0x1CA,
                ExclusiveGroup = "DruidForm",
                Modifiers = new StanceStatModifiers
                {
                    IntBonus = 15,
                    SpellDamageModifier = 30,
                    ArmorModifier = 25,
                    CritChanceBonus = 10,
                    EnergyResist = 15,
                }
            });

            // Travel Form - Movement
            RegisterStance(new StanceDefinition
            {
                Type = StanceType.DruidTravel,
                Name = "Travel Form",
                Description = "Transform into a swift stag. Cannot fight but moves very fast.",
                Category = StanceCategory.DruidForm,
                Flags = StanceFlags.TransformBody | StanceFlags.Exclusive | StanceFlags.PreventsCombat,
                BodyValue = 234,  // Great Hart body
                Hue = 2010,
                TransformSound = 0x85,
                ExclusiveGroup = "DruidForm",
                Modifiers = new StanceStatModifiers
                {
                    MovementSpeedModifier = 60,
                    DexBonus = 10,
                }
            });
        }

        private void RegisterSorcererElements()
        {
            // Fire Element
            RegisterStance(new StanceDefinition
            {
                Type = StanceType.SorcererFire,
                Name = "Fire Attunement",
                Description = "Attune to fire magic. Increased fire damage and spell power.",
                Category = StanceCategory.ElementalStance,
                Flags = StanceFlags.Exclusive | StanceFlags.HasAura,
                Hue = 1358,
                AuraEffect = 0x3709,
                AuraHue = 1358,
                TransformSound = 0x227,
                ExclusiveGroup = "SorcererElement",
                Modifiers = new StanceStatModifiers
                {
                    SpellDamageModifier = 15,
                    FireResist = 20,
                    ColdResist = -10,
                    CritChanceBonus = 5,
                }
            });

            // Water Element
            RegisterStance(new StanceDefinition
            {
                Type = StanceType.SorcererWater,
                Name = "Water Attunement",
                Description = "Attune to water magic. Enhanced cold spells and healing.",
                Category = StanceCategory.ElementalStance,
                Flags = StanceFlags.Exclusive | StanceFlags.HasAura,
                Hue = 1154,
                AuraEffect = 0x375A,
                AuraHue = 1154,
                TransformSound = 0x026,
                ExclusiveGroup = "SorcererElement",
                Modifiers = new StanceStatModifiers
                {
                    HealingModifier = 20,
                    ColdResist = 20,
                    FireResist = -10,
                    ManaRegenModifier = 15,
                }
            });

            // Earth Element
            RegisterStance(new StanceDefinition
            {
                Type = StanceType.SorcererEarth,
                Name = "Earth Attunement",
                Description = "Attune to earth magic. Increased armor and poison resistance.",
                Category = StanceCategory.ElementalStance,
                Flags = StanceFlags.Exclusive | StanceFlags.HasAura,
                Hue = 2305,
                AuraEffect = 0x36B0,
                AuraHue = 2305,
                TransformSound = 0x21F,
                ExclusiveGroup = "SorcererElement",
                Modifiers = new StanceStatModifiers
                {
                    ArmorModifier = 30,
                    PhysicalResist = 15,
                    PoisonResist = 20,
                    MovementSpeedModifier = -10,
                    CastSpeedModifier = -10,
                }
            });

            // Air Element
            RegisterStance(new StanceDefinition
            {
                Type = StanceType.SorcererAir,
                Name = "Air Attunement",
                Description = "Attune to air magic. Increased speed, evasion, and energy damage.",
                Category = StanceCategory.ElementalStance,
                Flags = StanceFlags.Exclusive | StanceFlags.HasAura,
                Hue = 1281,
                AuraEffect = 0x3728,
                AuraHue = 1281,
                TransformSound = 0x15,
                ExclusiveGroup = "SorcererElement",
                Modifiers = new StanceStatModifiers
                {
                    CastSpeedModifier = 20,
                    MovementSpeedModifier = 15,
                    DodgeChanceBonus = 15,
                    EnergyResist = 20,
                }
            });
        }

        private void RegisterFighterStances()
        {
            // Aggressive Stance
            RegisterStance(new StanceDefinition
            {
                Type = StanceType.FighterAggressive,
                Name = "Aggressive Stance",
                Description = "Focus on offense. Increased damage but reduced defense.",
                Category = StanceCategory.CombatStance,
                Flags = StanceFlags.Exclusive,
                TransformSound = 0x51B,
                ExclusiveGroup = "FighterStance",
                Modifiers = new StanceStatModifiers
                {
                    DamageModifier = 20,
                    MeleeDamageModifier = 15,
                    CritChanceBonus = 10,
                    ArmorModifier = -20,
                    BlockChanceBonus = -15,
                }
            });

            // Defensive Stance
            RegisterStance(new StanceDefinition
            {
                Type = StanceType.FighterDefensive,
                Name = "Defensive Stance",
                Description = "Focus on defense. Increased armor and block but reduced damage.",
                Category = StanceCategory.CombatStance,
                Flags = StanceFlags.Exclusive,
                TransformSound = 0x51B,
                ExclusiveGroup = "FighterStance",
                Modifiers = new StanceStatModifiers
                {
                    ArmorModifier = 25,
                    BlockChanceBonus = 20,
                    ParryChanceBonus = 15,
                    DamageReduction = 5,
                    DamageModifier = -15,
                    ThreatModifier = 30,
                }
            });

            // Balanced Stance
            RegisterStance(new StanceDefinition
            {
                Type = StanceType.FighterBalanced,
                Name = "Balanced Stance",
                Description = "A balanced approach to combat. No bonuses or penalties.",
                Category = StanceCategory.CombatStance,
                Flags = StanceFlags.Exclusive,
                TransformSound = 0x51B,
                ExclusiveGroup = "FighterStance",
                Modifiers = new StanceStatModifiers
                {
                    // No modifiers - baseline
                }
            });

            // Berserker Stance
            RegisterStance(new StanceDefinition
            {
                Type = StanceType.FighterBerserker,
                Name = "Berserker Stance",
                Description = "Reckless fighting. High damage but very vulnerable.",
                Category = StanceCategory.CombatStance,
                Flags = StanceFlags.Exclusive,
                TransformSound = 0x51B,
                ExclusiveGroup = "FighterStance",
                Modifiers = new StanceStatModifiers
                {
                    DamageModifier = 35,
                    CritChanceBonus = 20,
                    AttackSpeedModifier = 15,
                    ArmorModifier = -40,
                    DodgeChanceBonus = -20,
                    BlockChanceBonus = -20,
                }
            });
        }

        private void RegisterBarbarianStances()
        {
            // Normal State
            RegisterStance(new StanceDefinition
            {
                Type = StanceType.BarbarianNormal,
                Name = "Normal",
                Description = "Your normal, unraged state.",
                Category = StanceCategory.Transformation,
                Flags = StanceFlags.None,
                ExclusiveGroup = "BarbarianRage",
                Modifiers = new StanceStatModifiers { }
            });

            // Rage Transformation
            RegisterStance(new StanceDefinition
            {
                Type = StanceType.BarbarianRage,
                Name = "Rage",
                Description = "Enter a furious rage! Greatly increased strength and damage, but reduced intelligence.",
                Category = StanceCategory.Transformation,
                Flags = StanceFlags.Exclusive | StanceFlags.HasParticles,
                Hue = 1157,
                AuraEffect = 0x3709,
                AuraHue = 1157,
                TransformSound = 0x16A,
                ExitSound = 0x039,
                Duration = TimeSpan.FromSeconds(30),
                Cooldown = TimeSpan.FromMinutes(2),
                ExclusiveGroup = "BarbarianRage",
                SecondaryResourceCost = ResourceType.Fury,
                SecondaryResourceAmount = 50,
                Modifiers = new StanceStatModifiers
                {
                    StrBonus = 30,
                    IntBonus = -20,
                    DamageModifier = 40,
                    MeleeDamageModifier = 25,
                    CritDamageBonus = 30,
                    AttackSpeedModifier = 20,
                    MovementSpeedModifier = 15,
                    PhysicalResist = -10,
                    DamageReduction = -5,
                }
            });
        }

        private void RegisterMonkStances()
        {
            // Windwalker - Speed/DPS
            RegisterStance(new StanceDefinition
            {
                Type = StanceType.MonkWindwalker,
                Name = "Windwalker Stance",
                Description = "Swift as the wind. Enhanced speed and evasion.",
                Category = StanceCategory.MartialStance,
                Flags = StanceFlags.Exclusive,
                TransformSound = 0x15,
                ExclusiveGroup = "MonkStance",
                Modifiers = new StanceStatModifiers
                {
                    DexBonus = 10,
                    AttackSpeedModifier = 25,
                    MovementSpeedModifier = 20,
                    DodgeChanceBonus = 20,
                    CritChanceBonus = 10,
                    ResourceGenModifier = 20,
                }
            });

            // Brewmaster - Tank
            RegisterStance(new StanceDefinition
            {
                Type = StanceType.MonkBrewmaster,
                Name = "Brewmaster Stance",
                Description = "Sturdy as an oak. Enhanced defense and stagger damage.",
                Category = StanceCategory.MartialStance,
                Flags = StanceFlags.Exclusive,
                TransformSound = 0x240,
                ExclusiveGroup = "MonkStance",
                Modifiers = new StanceStatModifiers
                {
                    StrBonus = 10,
                    ArmorModifier = 30,
                    DamageReduction = 10,
                    DodgeChanceBonus = 15,
                    ThreatModifier = 40,
                    HPRegenModifier = 20,
                }
            });

            // Mistweaver - Healer
            RegisterStance(new StanceDefinition
            {
                Type = StanceType.MonkMistweaver,
                Name = "Mistweaver Stance",
                Description = "Channel healing mists. Enhanced healing and mana regen.",
                Category = StanceCategory.MartialStance,
                Flags = StanceFlags.Exclusive,
                TransformSound = 0x026,
                ExclusiveGroup = "MonkStance",
                Modifiers = new StanceStatModifiers
                {
                    IntBonus = 10,
                    HealingModifier = 35,
                    ManaRegenModifier = 25,
                    CastSpeedModifier = 15,
                }
            });
        }

        private void RegisterRogueStances()
        {
            // Shadow
            RegisterStance(new StanceDefinition
            {
                Type = StanceType.RogueShadow,
                Name = "Shadow Dance",
                Description = "Embrace the shadows. Enhanced stealth and burst damage.",
                Category = StanceCategory.MartialStance,
                Flags = StanceFlags.Exclusive | StanceFlags.AllowsStealth,
                ExclusiveGroup = "RogueStance",
                Modifiers = new StanceStatModifiers
                {
                    DexBonus = 10,
                    CritChanceBonus = 20,
                    CritDamageBonus = 30,
                    DodgeChanceBonus = 15,
                }
            });

            // Outlaw
            RegisterStance(new StanceDefinition
            {
                Type = StanceType.RogueOutlaw,
                Name = "Outlaw Stance",
                Description = "Fight with reckless abandon. Enhanced dual-wield damage.",
                Category = StanceCategory.MartialStance,
                Flags = StanceFlags.Exclusive,
                ExclusiveGroup = "RogueStance",
                Modifiers = new StanceStatModifiers
                {
                    StrBonus = 5,
                    DexBonus = 5,
                    AttackSpeedModifier = 30,
                    MeleeDamageModifier = 20,
                    DodgeChanceBonus = 10,
                }
            });

            // Subtlety
            RegisterStance(new StanceDefinition
            {
                Type = StanceType.RogueSubtlety,
                Name = "Subtlety",
                Description = "Patient and precise. Enhanced control and damage over time.",
                Category = StanceCategory.MartialStance,
                Flags = StanceFlags.Exclusive | StanceFlags.AllowsStealth,
                ExclusiveGroup = "RogueStance",
                Modifiers = new StanceStatModifiers
                {
                    DexBonus = 15,
                    CritChanceBonus = 10,
                    ResourceGenModifier = 25,
                }
            });
        }

        private void RegisterPaladinAuras()
        {
            // Devotion Aura
            RegisterStance(new StanceDefinition
            {
                Type = StanceType.PaladinDevotion,
                Name = "Devotion Aura",
                Description = "Radiate healing energy. Increased healing done and received.",
                Category = StanceCategory.Aura,
                Flags = StanceFlags.Exclusive | StanceFlags.HasAura,
                AuraEffect = 0x376A,
                AuraHue = 1153,
                TransformSound = 0x1F2,
                ExclusiveGroup = "PaladinAura",
                Modifiers = new StanceStatModifiers
                {
                    HealingModifier = 25,
                    HPRegenModifier = 30,
                    PhysicalResist = 5,
                }
            });

            // Retribution Aura
            RegisterStance(new StanceDefinition
            {
                Type = StanceType.PaladinRetribution,
                Name = "Retribution Aura",
                Description = "Holy wrath surrounds you. Reflect damage back to attackers.",
                Category = StanceCategory.Aura,
                Flags = StanceFlags.Exclusive | StanceFlags.HasAura,
                AuraEffect = 0x3789,
                AuraHue = 1161,
                TransformSound = 0x1F2,
                ExclusiveGroup = "PaladinAura",
                Modifiers = new StanceStatModifiers
                {
                    DamageModifier = 15,
                    MeleeDamageModifier = 10,
                    // Reflect effect handled separately
                }
            });

            // Protection Aura
            RegisterStance(new StanceDefinition
            {
                Type = StanceType.PaladinProtection,
                Name = "Protection Aura",
                Description = "Divine protection shields you. Reduced damage taken.",
                Category = StanceCategory.Aura,
                Flags = StanceFlags.Exclusive | StanceFlags.HasAura,
                AuraEffect = 0x375A,
                AuraHue = 1153,
                TransformSound = 0x1F2,
                ExclusiveGroup = "PaladinAura",
                Modifiers = new StanceStatModifiers
                {
                    ArmorModifier = 20,
                    DamageReduction = 10,
                    PhysicalResist = 10,
                    ThreatModifier = 30,
                }
            });
        }

        private void RegisterRangerAspects()
        {
            // Hawk Aspect
            RegisterStance(new StanceDefinition
            {
                Type = StanceType.RangerHawk,
                Name = "Aspect of the Hawk",
                Description = "Channel the hawk's precision. Enhanced ranged accuracy.",
                Category = StanceCategory.Aspect,
                Flags = StanceFlags.Exclusive,
                TransformSound = 0x2EE,
                ExclusiveGroup = "RangerAspect",
                Modifiers = new StanceStatModifiers
                {
                    DexBonus = 10,
                    RangedDamageModifier = 25,
                    CritChanceBonus = 15,
                }
            });

            // Wolf Aspect
            RegisterStance(new StanceDefinition
            {
                Type = StanceType.RangerWolf,
                Name = "Aspect of the Wolf",
                Description = "Channel the wolf's speed. Enhanced movement and pack tactics.",
                Category = StanceCategory.Aspect,
                Flags = StanceFlags.Exclusive,
                TransformSound = 0xE5,
                ExclusiveGroup = "RangerAspect",
                Modifiers = new StanceStatModifiers
                {
                    MovementSpeedModifier = 30,
                    AttackSpeedModifier = 15,
                    DexBonus = 5,
                }
            });

            // Bear Aspect
            RegisterStance(new StanceDefinition
            {
                Type = StanceType.RangerBear,
                Name = "Aspect of the Bear",
                Description = "Channel the bear's endurance. Enhanced survival.",
                Category = StanceCategory.Aspect,
                Flags = StanceFlags.Exclusive,
                TransformSound = 0x1CA,
                ExclusiveGroup = "RangerAspect",
                Modifiers = new StanceStatModifiers
                {
                    StrBonus = 10,
                    MaxHPBonus = 30,
                    HPRegenModifier = 40,
                    ArmorModifier = 15,
                }
            });
        }

        #endregion
    }

    #endregion

    #region Stance Combo System

    /// <summary>
    /// Handles stance transition combos - effects that trigger when switching between stances
    /// </summary>
    public class VystiaStanceComboSystem
    {
        private static VystiaStanceComboSystem m_Instance;
        public static VystiaStanceComboSystem Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new VystiaStanceComboSystem();
                return m_Instance;
            }
        }

        /// <summary>
        /// Combo definition
        /// </summary>
        public class StanceCombo
        {
            public StanceType FromStance { get; set; }
            public StanceType ToStance { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public Action<Mobile> Effect { get; set; }
            public TimeSpan Window { get; set; }  // Time window to trigger combo after leaving first stance
        }

        private List<StanceCombo> m_Combos;
        private Dictionary<Mobile, Tuple<StanceType, DateTime>> m_RecentExits;

        private VystiaStanceComboSystem()
        {
            m_Combos = new List<StanceCombo>();
            m_RecentExits = new Dictionary<Mobile, Tuple<StanceType, DateTime>>();

            RegisterDefaultCombos();
        }

        /// <summary>
        /// Register a combo
        /// </summary>
        public void RegisterCombo(StanceCombo combo)
        {
            if (combo != null)
                m_Combos.Add(combo);
        }

        /// <summary>
        /// Called when a mobile enters a stance
        /// </summary>
        public void OnStanceEntered(Mobile mobile, StanceType newStance)
        {
            if (mobile == null)
                return;

            // Check for combo
            if (m_RecentExits.TryGetValue(mobile, out Tuple<StanceType, DateTime> recent))
            {
                StanceType fromStance = recent.Item1;
                DateTime exitTime = recent.Item2;

                foreach (var combo in m_Combos)
                {
                    if (combo.FromStance == fromStance && combo.ToStance == newStance)
                    {
                        // Check if within time window
                        if (DateTime.UtcNow - exitTime <= combo.Window)
                        {
                            TriggerCombo(mobile, combo);
                        }
                    }
                }

                m_RecentExits.Remove(mobile);
            }
        }

        /// <summary>
        /// Called when a mobile exits a stance
        /// </summary>
        public void OnStanceExited(Mobile mobile, StanceType oldStance)
        {
            if (mobile == null)
                return;

            // Record the exit for combo detection
            m_RecentExits[mobile] = new Tuple<StanceType, DateTime>(oldStance, DateTime.UtcNow);

            // Clean up old entries
            Timer.DelayCall(TimeSpan.FromSeconds(5), () =>
            {
                if (m_RecentExits.TryGetValue(mobile, out Tuple<StanceType, DateTime> entry))
                {
                    if (entry.Item1 == oldStance)
                        m_RecentExits.Remove(mobile);
                }
            });
        }

        /// <summary>
        /// Trigger a combo effect
        /// </summary>
        private void TriggerCombo(Mobile mobile, StanceCombo combo)
        {
            mobile.SendMessage(0x3B2, $"Combo: {combo.Name}!");
            mobile.FixedParticles(0x375A, 10, 15, 5037, EffectLayer.Head);
            mobile.PlaySound(0x1F7);

            combo.Effect?.Invoke(mobile);
        }

        /// <summary>
        /// Register default combos
        /// </summary>
        private void RegisterDefaultCombos()
        {
            // Druid: Cat to Bear = "Savage Roar" - burst threat + damage
            RegisterCombo(new StanceCombo
            {
                FromStance = StanceType.DruidCat,
                ToStance = StanceType.DruidBear,
                Name = "Savage Roar",
                Description = "Transition from cat to bear form with a ferocious roar, generating massive threat.",
                Window = TimeSpan.FromSeconds(3),
                Effect = (m) =>
                {
                    m.PlaySound(0x16A);
                    // Apply temporary damage/strength buff (represents increased threat)
                    m.AddStatMod(new StatMod(StatType.Str, "SavageRoar", 15, TimeSpan.FromSeconds(10)));
                    m.FixedParticles(0x3709, 10, 30, 5052, 1157, 0, EffectLayer.Head);
                }
            });

            // Druid: Bear to Cat = "Pounce" - gap closer + stun
            RegisterCombo(new StanceCombo
            {
                FromStance = StanceType.DruidBear,
                ToStance = StanceType.DruidCat,
                Name = "Feral Pounce",
                Description = "Leap from bear to cat form, stunning your target.",
                Window = TimeSpan.FromSeconds(3),
                Effect = (m) =>
                {
                    m.PlaySound(0x69);
                    // Apply brief speed boost
                    m.AddStatMod(new StatMod(StatType.Dex, "FeralPounce", 20, TimeSpan.FromSeconds(5)));
                }
            });

            // Sorcerer: Fire to Water = "Steam Burst" - AoE damage
            RegisterCombo(new StanceCombo
            {
                FromStance = StanceType.SorcererFire,
                ToStance = StanceType.SorcererWater,
                Name = "Steam Burst",
                Description = "Transitioning from fire to water creates a burst of steam, damaging nearby enemies.",
                Window = TimeSpan.FromSeconds(2),
                Effect = (m) =>
                {
                    m.PlaySound(0x108);
                    Effects.SendLocationParticles(
                        EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration),
                        0x36B0, 10, 30, 0, 0, 0, 0);

                    // Damage nearby enemies
                    foreach (Mobile target in m.GetMobilesInRange(3))
                    {
                        if (target != m && m.CanBeHarmful(target))
                        {
                            m.DoHarmful(target);
                            int damage = Utility.RandomMinMax(15, 25);
                            AOS.Damage(target, m, damage, 0, 50, 50, 0, 0);
                        }
                    }
                }
            });

            // Sorcerer: Earth to Air = "Dust Devil" - evasion buff
            RegisterCombo(new StanceCombo
            {
                FromStance = StanceType.SorcererEarth,
                ToStance = StanceType.SorcererAir,
                Name = "Dust Devil",
                Description = "Earth gives way to air, creating a dust devil that increases evasion.",
                Window = TimeSpan.FromSeconds(2),
                Effect = (m) =>
                {
                    m.PlaySound(0x15);
                    m.FixedParticles(0x36B0, 1, 14, 0, 2305, 0, EffectLayer.Waist);
                    // Boost evasion temporarily
                    m.AddStatMod(new StatMod(StatType.Dex, "DustDevil", 25, TimeSpan.FromSeconds(8)));
                }
            });

            // Fighter: Defensive to Aggressive = "Counter Strike"
            RegisterCombo(new StanceCombo
            {
                FromStance = StanceType.FighterDefensive,
                ToStance = StanceType.FighterAggressive,
                Name = "Counter Strike",
                Description = "Transition from defense to offense with a powerful counter attack.",
                Window = TimeSpan.FromSeconds(2),
                Effect = (m) =>
                {
                    m.PlaySound(0x51B);
                    // Next attack deals bonus damage
                    if (m is PlayerMobile pm)
                    {
                        VystiaBuffManager.Instance.ApplyBuff(pm, pm, VystiaBuffType.DamageIncrease,
                            TimeSpan.FromSeconds(5), 50);
                    }
                }
            });

            // Barbarian: Normal to Rage = (handled by Rage itself)
            // But exiting Rage gives "Exhaustion" debuff
        }
    }

    #endregion

    #region GM Commands

    /// <summary>
    /// GM commands for testing the stance system
    /// </summary>
    public class StanceSystemCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("SetStance", AccessLevel.GameMaster, new CommandEventHandler(SetStance_OnCommand));
            CommandSystem.Register("RemoveStance", AccessLevel.GameMaster, new CommandEventHandler(RemoveStance_OnCommand));
            CommandSystem.Register("ListStances", AccessLevel.GameMaster, new CommandEventHandler(ListStances_OnCommand));
            CommandSystem.Register("ClearStances", AccessLevel.GameMaster, new CommandEventHandler(ClearStances_OnCommand));
            CommandSystem.Register("StanceInfo", AccessLevel.GameMaster, new CommandEventHandler(StanceInfo_OnCommand));
            CommandSystem.Register("ResetStanceCooldowns", AccessLevel.GameMaster, new CommandEventHandler(ResetStanceCooldowns_OnCommand));
        }

        [Usage("SetStance <type>")]
        [Description("Apply a stance to yourself or your target.")]
        private static void SetStance_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            Mobile target = from;

            if (from.Target != null)
            {
                from.SendMessage("You are already targeting something.");
                return;
            }

            if (e.Length < 1)
            {
                from.SendMessage("Usage: [SetStance <type>");
                from.SendMessage("Available stances:");
                foreach (StanceType type in Enum.GetValues(typeof(StanceType)))
                {
                    if (type != StanceType.None)
                        from.SendMessage($"  {type}");
                }
                return;
            }

            string stanceName = e.GetString(0);
            if (!Enum.TryParse(stanceName, true, out StanceType stanceType))
            {
                from.SendMessage($"Unknown stance type: {stanceName}");
                return;
            }

            if (VystiaStanceManager.Instance.ApplyStance(target, stanceType, true))
            {
                from.SendMessage($"Applied {stanceType} to {target.Name}.");
            }
            else
            {
                from.SendMessage($"Failed to apply {stanceType}.");
            }
        }

        [Usage("RemoveStance <type>")]
        [Description("Remove a stance from yourself or your target.")]
        private static void RemoveStance_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            Mobile target = from;

            if (e.Length < 1)
            {
                from.SendMessage("Usage: [RemoveStance <type>");
                return;
            }

            string stanceName = e.GetString(0);
            if (!Enum.TryParse(stanceName, true, out StanceType stanceType))
            {
                from.SendMessage($"Unknown stance type: {stanceName}");
                return;
            }

            if (VystiaStanceManager.Instance.RemoveStance(target, stanceType))
            {
                from.SendMessage($"Removed {stanceType} from {target.Name}.");
            }
            else
            {
                from.SendMessage($"{target.Name} doesn't have {stanceType}.");
            }
        }

        [Usage("ListStances")]
        [Description("List all active stances on yourself or your target.")]
        private static void ListStances_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            Mobile target = from;

            var stances = VystiaStanceManager.Instance.GetAllActiveStances(target);

            if (stances.Count == 0)
            {
                from.SendMessage($"{target.Name} has no active stances.");
                return;
            }

            from.SendMessage($"Active stances on {target.Name}:");
            foreach (var stance in stances)
            {
                string duration = stance.Definition.HasDuration
                    ? $" ({stance.TimeRemaining.TotalSeconds:F1}s remaining)"
                    : " (permanent)";
                from.SendMessage($"  {stance.Definition.Name}{duration}");
            }
        }

        [Usage("ClearStances")]
        [Description("Remove all stances from yourself or your target.")]
        private static void ClearStances_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            Mobile target = from;

            VystiaStanceManager.Instance.RemoveAllStances(target);
            from.SendMessage($"Cleared all stances from {target.Name}.");
        }

        [Usage("StanceInfo <type>")]
        [Description("Get information about a stance type.")]
        private static void StanceInfo_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Length < 1)
            {
                from.SendMessage("Usage: [StanceInfo <type>");
                return;
            }

            string stanceName = e.GetString(0);
            if (!Enum.TryParse(stanceName, true, out StanceType stanceType))
            {
                from.SendMessage($"Unknown stance type: {stanceName}");
                return;
            }

            StanceDefinition def = VystiaStanceManager.Instance.GetDefinition(stanceType);
            if (def == null)
            {
                from.SendMessage($"No definition found for {stanceType}");
                return;
            }

            from.SendMessage($"=== {def.Name} ===");
            from.SendMessage($"Type: {def.Type}");
            from.SendMessage($"Category: {def.Category}");
            from.SendMessage($"Description: {def.Description}");
            from.SendMessage($"Flags: {def.Flags}");
            from.SendMessage($"Duration: {(def.HasDuration ? def.Duration.TotalSeconds + "s" : "Permanent")}");
            from.SendMessage($"Cooldown: {def.Cooldown.TotalSeconds}s");

            if (def.Modifiers != null)
            {
                from.SendMessage("Modifiers:");
                if (def.Modifiers.StrBonus != 0)
                    from.SendMessage($"  STR: {def.Modifiers.StrBonus:+#;-#;0}");
                if (def.Modifiers.DexBonus != 0)
                    from.SendMessage($"  DEX: {def.Modifiers.DexBonus:+#;-#;0}");
                if (def.Modifiers.IntBonus != 0)
                    from.SendMessage($"  INT: {def.Modifiers.IntBonus:+#;-#;0}");
                if (def.Modifiers.DamageModifier != 0)
                    from.SendMessage($"  Damage: {def.Modifiers.DamageModifier:+#;-#;0}%");
                if (def.Modifiers.ArmorModifier != 0)
                    from.SendMessage($"  Armor: {def.Modifiers.ArmorModifier:+#;-#;0}%");
                if (def.Modifiers.MovementSpeedModifier != 0)
                    from.SendMessage($"  Speed: {def.Modifiers.MovementSpeedModifier:+#;-#;0}%");
            }
        }

        [Usage("ResetStanceCooldowns")]
        [Description("Reset all stance cooldowns for yourself.")]
        private static void ResetStanceCooldowns_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            VystiaStanceManager.Instance.ResetCooldowns(from);
            from.SendMessage("All stance cooldowns have been reset.");
        }
    }

    #endregion
}
