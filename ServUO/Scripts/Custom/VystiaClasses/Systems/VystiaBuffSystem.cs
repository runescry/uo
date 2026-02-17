using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Commands;
using Server.Network;

namespace Server.Custom.VystiaClasses.Systems
{
    #region Enums

    /// <summary>
    /// Categories of buff effects
    /// </summary>
    public enum BuffCategory
    {
        Beneficial,     // Positive effect (buff)
        Harmful,        // Negative effect (debuff)
        Neutral         // Utility effect
    }

    /// <summary>
    /// Types of buff effects
    /// </summary>
    public enum VystiaBuffType
    {
        // Stat Buffs
        StrengthBuff,
        DexterityBuff,
        IntelligenceBuff,
        AllStatsBuff,

        // Stat Debuffs
        StrengthDebuff,
        DexterityDebuff,
        IntelligenceDebuff,
        AllStatsDebuff,

        // Damage Buffs
        DamageIncrease,
        PhysicalDamageIncrease,
        FireDamageIncrease,
        ColdDamageIncrease,
        PoisonDamageIncrease,
        EnergyDamageIncrease,
        SpellDamageIncrease,

        // Damage Debuffs
        DamageDecrease,
        Vulnerability,      // Takes more damage

        // Resistance Buffs
        PhysicalResist,
        FireResist,
        ColdResist,
        PoisonResist,
        EnergyResist,
        AllResist,

        // Resistance Debuffs
        ArmorBreak,
        ResistanceDebuff,

        // Speed Buffs
        HasteBuff,
        SwingSpeedBuff,
        CastSpeedBuff,
        MovementSpeedBuff,

        // Speed Debuffs
        SlowDebuff,
        AttackSpeedDebuff,
        CastSpeedDebuff,

        // Regen Buffs
        HitPointRegen,
        ManaRegen,
        StaminaRegen,

        // Regen Debuffs
        HealingReduction,   // Mortal Strike style

        // DoT Effects
        Bleed,
        Burn,
        Poison,
        Corruption,
        SoulDrain,

        // HoT Effects
        Rejuvenation,
        LifeBloom,
        Tranquility,

        // Shield Effects
        DamageAbsorb,       // Absorbs X damage
        ManaShield,         // Damage goes to mana
        ReflectShield,      // Reflects % damage

        // Transform Effects
        BearForm,
        CatForm,
        WolfForm,
        TreeForm,
        MoonkinForm,
        RageForm,
        ShadowForm,
        ElementalForm,

        // Control Effects (also see CrowdControlSystem)
        Stealth,
        Invisible,
        Invulnerable,
        Silenced,
        Pacified,           // Can't attack

        // Utility Effects
        WaterBreathing,
        NightVision,
        TrueSeeing,         // See through stealth/invis
        LifeTap,            // Damage returns as health
        ManaTap,            // Damage returns as mana

        // Class-Specific
        SongOfCourage,      // Bard
        SongOfSwiftness,    // Bard
        BattleCry,          // Barbarian
        Blessing,           // Paladin
        Consecration,       // Paladin
        Inspiration,        // Bard
        HexWard,            // Witch
        ShadowCloak,        // Warlock
        ArcaneInfusion,     // Wizard
        DivineShield        // Paladin
    }

    /// <summary>
    /// How the buff behaves when reapplied
    /// </summary>
    public enum StackBehavior
    {
        Refresh,            // Resets duration, same strength
        Stack,              // Adds stacks, each has effect
        Replace,            // Stronger replaces weaker
        Extend,             // Adds to duration
        Ignore              // Does nothing if already present
    }

    #endregion

    #region Buff Definition

    /// <summary>
    /// Defines a buff's properties and behavior
    /// </summary>
    public class VystiaBuffDefinition
    {
        public VystiaBuffType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public BuffCategory Category { get; set; }
        public StackBehavior StackBehavior { get; set; }
        public int MaxStacks { get; set; }
        public TimeSpan DefaultDuration { get; set; }
        public TimeSpan TickInterval { get; set; }      // For DoTs/HoTs
        public bool Dispellable { get; set; }
        public bool PersistsThroughDeath { get; set; }
        public int IconID { get; set; }                 // For client buff icon
        public int Hue { get; set; }                    // Visual hue
        public VystiaBuffType[] Exclusions { get; set; } // Buffs this replaces

        public VystiaBuffDefinition()
        {
            MaxStacks = 1;
            StackBehavior = StackBehavior.Refresh;
            Dispellable = true;
            PersistsThroughDeath = false;
            TickInterval = TimeSpan.Zero;
            Exclusions = new VystiaBuffType[0];
        }
    }

    #endregion

    #region Active Buff Instance

    /// <summary>
    /// Represents an active buff on a target
    /// </summary>
    public class VystiaBuffInstance
    {
        public VystiaBuffDefinition Definition { get; private set; }
        public Mobile Target { get; private set; }
        public Mobile Caster { get; private set; }
        public int Stacks { get; set; }
        public DateTime AppliedAt { get; private set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime LastTick { get; set; }

        // Effect values
        public int StatModifier { get; set; }           // For stat buffs
        public int DamagePerTick { get; set; }          // For DoTs
        public int HealPerTick { get; set; }            // For HoTs
        public int AbsorbRemaining { get; set; }        // For shields
        public int ReflectPercent { get; set; }         // For reflect shields
        public double SpeedModifier { get; set; }       // Multiplier for speed effects
        public int ResistModifier { get; set; }         // For resist buffs

        // Transform data
        public int OriginalBody { get; set; }
        public int TransformBody { get; set; }
        public int OriginalHue { get; set; }
        public int TransformHue { get; set; }

        // Active stat mods (for removal)
        private List<StatMod> m_AppliedStatMods;
        private List<ResistanceMod> m_AppliedResistMods;

        public VystiaBuffInstance(VystiaBuffDefinition def, Mobile target, Mobile caster, TimeSpan duration)
        {
            Definition = def;
            Target = target;
            Caster = caster;
            Stacks = 1;
            AppliedAt = DateTime.UtcNow;
            ExpiresAt = DateTime.UtcNow + duration;
            LastTick = DateTime.UtcNow;
            m_AppliedStatMods = new List<StatMod>();
            m_AppliedResistMods = new List<ResistanceMod>();
        }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public TimeSpan RemainingDuration => IsExpired ? TimeSpan.Zero : ExpiresAt - DateTime.UtcNow;

        public void Refresh(TimeSpan duration)
        {
            ExpiresAt = DateTime.UtcNow + duration;
        }

        public void Extend(TimeSpan duration)
        {
            ExpiresAt = ExpiresAt + duration;
        }

        public void AddStack()
        {
            if (Stacks < Definition.MaxStacks)
                Stacks++;
        }

        #region Stat Mod Management

        public void AddStatMod(StatType stat, int offset)
        {
            string name = string.Format("VystiaBuff_{0}_{1}", Definition.Type, stat);
            StatMod mod = new StatMod(stat, name, offset, TimeSpan.Zero);
            Target.AddStatMod(mod);
            m_AppliedStatMods.Add(mod);
        }

        public void AddResistMod(ResistanceType resist, int offset)
        {
            ResistanceMod mod = new ResistanceMod(resist, offset);
            Target.AddResistanceMod(mod);
            m_AppliedResistMods.Add(mod);
        }

        public void RemoveAllMods()
        {
            foreach (StatMod mod in m_AppliedStatMods)
            {
                Target.RemoveStatMod(mod.Name);
            }
            m_AppliedStatMods.Clear();

            foreach (ResistanceMod mod in m_AppliedResistMods)
            {
                Target.RemoveResistanceMod(mod);
            }
            m_AppliedResistMods.Clear();
        }

        #endregion

        #region Effect Processing

        public void OnApply()
        {
            ApplyStatEffects();
            ApplyVisualEffects();

            if (Definition.Category == BuffCategory.Beneficial)
            {
                Target.SendMessage(0x3B2, "{0} applied!", Definition.Name);
            }
            else if (Definition.Category == BuffCategory.Harmful)
            {
                Target.SendMessage(0x22, "You are afflicted with {0}!", Definition.Name);
            }
        }

        public void OnRemove()
        {
            RemoveAllMods();
            RevertTransform();

            if (Definition.Category == BuffCategory.Beneficial)
            {
                Target.SendMessage(0x3B2, "{0} has faded.", Definition.Name);
            }
            else if (Definition.Category == BuffCategory.Harmful)
            {
                Target.SendMessage(0x3B2, "{0} has worn off.", Definition.Name);
            }
        }

        public void OnTick()
        {
            if (Target == null || Target.Deleted || !Target.Alive)
                return;

            // DoT damage
            if (DamagePerTick > 0)
            {
                int totalDamage = DamagePerTick * Stacks;
                ApplyTickDamage(totalDamage);
            }

            // HoT healing
            if (HealPerTick > 0)
            {
                int totalHeal = HealPerTick * Stacks;
                ApplyTickHeal(totalHeal);
            }

            LastTick = DateTime.UtcNow;
        }

        private void ApplyTickDamage(int damage)
        {
            // Determine damage type
            int phys = 0, fire = 0, cold = 0, pois = 0, energy = 0;

            switch (Definition.Type)
            {
                case VystiaBuffType.Bleed:
                    phys = 100;
                    break;
                case VystiaBuffType.Burn:
                    fire = 100;
                    break;
                case VystiaBuffType.Poison:
                    pois = 100;
                    break;
                case VystiaBuffType.Corruption:
                case VystiaBuffType.SoulDrain:
                    energy = 50;
                    pois = 50;
                    break;
                default:
                    phys = 100;
                    break;
            }

            if (Caster != null && !Caster.Deleted)
            {
                AOS.Damage(Target, Caster, damage, phys, fire, cold, pois, energy);
            }
            else
            {
                AOS.Damage(Target, damage, phys, fire, cold, pois, energy);
            }
        }

        private void ApplyTickHeal(int heal)
        {
            Target.Heal(heal, Caster ?? Target, false);

            // Visual effect
            Target.FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);
        }

        private void ApplyStatEffects()
        {
            switch (Definition.Type)
            {
                case VystiaBuffType.StrengthBuff:
                    AddStatMod(StatType.Str, StatModifier);
                    break;
                case VystiaBuffType.DexterityBuff:
                    AddStatMod(StatType.Dex, StatModifier);
                    break;
                case VystiaBuffType.IntelligenceBuff:
                    AddStatMod(StatType.Int, StatModifier);
                    break;
                case VystiaBuffType.AllStatsBuff:
                    AddStatMod(StatType.Str, StatModifier);
                    AddStatMod(StatType.Dex, StatModifier);
                    AddStatMod(StatType.Int, StatModifier);
                    break;
                case VystiaBuffType.SongOfSwiftness:
                    AddStatMod(StatType.Dex, StatModifier);
                    break;
                case VystiaBuffType.StrengthDebuff:
                    AddStatMod(StatType.Str, -StatModifier);
                    break;
                case VystiaBuffType.DexterityDebuff:
                    AddStatMod(StatType.Dex, -StatModifier);
                    break;
                case VystiaBuffType.IntelligenceDebuff:
                    AddStatMod(StatType.Int, -StatModifier);
                    break;
                case VystiaBuffType.AllStatsDebuff:
                    AddStatMod(StatType.Str, -StatModifier);
                    AddStatMod(StatType.Dex, -StatModifier);
                    AddStatMod(StatType.Int, -StatModifier);
                    break;
                case VystiaBuffType.PhysicalResist:
                    AddResistMod(ResistanceType.Physical, ResistModifier);
                    break;
                case VystiaBuffType.FireResist:
                    AddResistMod(ResistanceType.Fire, ResistModifier);
                    break;
                case VystiaBuffType.ColdResist:
                    AddResistMod(ResistanceType.Cold, ResistModifier);
                    break;
                case VystiaBuffType.PoisonResist:
                    AddResistMod(ResistanceType.Poison, ResistModifier);
                    break;
                case VystiaBuffType.EnergyResist:
                    AddResistMod(ResistanceType.Energy, ResistModifier);
                    break;
                case VystiaBuffType.AllResist:
                    AddResistMod(ResistanceType.Physical, ResistModifier);
                    AddResistMod(ResistanceType.Fire, ResistModifier);
                    AddResistMod(ResistanceType.Cold, ResistModifier);
                    AddResistMod(ResistanceType.Poison, ResistModifier);
                    AddResistMod(ResistanceType.Energy, ResistModifier);
                    break;
            }

            // Transform effects
            ApplyTransform();
        }

        private void ApplyTransform()
        {
            if (TransformBody <= 0)
                return;

            OriginalBody = Target.Body;
            OriginalHue = Target.Hue;

            Target.BodyMod = TransformBody;
            if (TransformHue != 0)
            {
                Target.HueMod = TransformHue;
            }

            // Sound and effect
            Target.PlaySound(0x20F);
            Target.FixedParticles(0x3728, 1, 13, 9912, Definition.Hue, 7, EffectLayer.Head);
        }

        private void RevertTransform()
        {
            if (OriginalBody <= 0)
                return;

            Target.BodyMod = 0;
            Target.HueMod = -1;

            // Sound and effect
            Target.PlaySound(0x20F);
            Target.FixedParticles(0x3728, 1, 13, 9912, 0, 7, EffectLayer.Head);
        }

        private void ApplyVisualEffects()
        {
            if (Definition.Hue != 0)
            {
                Target.FixedParticles(0x376A, 9, 32, 5030, Definition.Hue, 0, EffectLayer.Waist);
            }
        }

        #endregion

        #region Shield Processing

        public int AbsorbDamage(int damage)
        {
            if (AbsorbRemaining <= 0)
                return damage;

            if (damage <= AbsorbRemaining)
            {
                AbsorbRemaining -= damage;
                Target.SendMessage(0x3B2, "Shield absorbs {0} damage! ({1} remaining)", damage, AbsorbRemaining);
                return 0;
            }
            else
            {
                int absorbed = AbsorbRemaining;
                int remaining = damage - AbsorbRemaining;
                AbsorbRemaining = 0;
                Target.SendMessage(0x22, "Shield broken after absorbing {0} damage!", absorbed);
                return remaining;
            }
        }

        public int ReflectDamage(int damage, Mobile attacker)
        {
            if (ReflectPercent <= 0 || attacker == null)
                return damage;

            int reflected = (damage * ReflectPercent) / 100;
            if (reflected > 0)
            {
                AOS.Damage(attacker, Target, reflected, 100, 0, 0, 0, 0);
                attacker.SendMessage(0x22, "{0} damage reflected!", reflected);
            }

            return damage;
        }

        #endregion
    }

    #endregion

    #region Buff Manager

    /// <summary>
    /// Manages all active buffs for the server
    /// </summary>
    public class VystiaBuffManager
    {
        private static VystiaBuffManager m_Instance;
        public static VystiaBuffManager Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new VystiaBuffManager();
                return m_Instance;
            }
        }

        // Target -> List of active buffs
        private Dictionary<Mobile, List<VystiaBuffInstance>> m_ActiveBuffs;

        // Buff definitions registry
        private Dictionary<VystiaBuffType, VystiaBuffDefinition> m_Definitions;

        private Timer m_TickTimer;

        public VystiaBuffManager()
        {
            m_ActiveBuffs = new Dictionary<Mobile, List<VystiaBuffInstance>>();
            m_Definitions = new Dictionary<VystiaBuffType, VystiaBuffDefinition>();

            RegisterDefaultDefinitions();

            // Tick every second for DoTs/HoTs and expiration
            m_TickTimer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), ProcessTicks);
        }

        #region Definition Registry

        public void RegisterDefinition(VystiaBuffDefinition def)
        {
            m_Definitions[def.Type] = def;
        }

        public VystiaBuffDefinition GetDefinition(VystiaBuffType type)
        {
            if (m_Definitions.TryGetValue(type, out VystiaBuffDefinition def))
                return def;
            return null;
        }

        private void RegisterDefaultDefinitions()
        {
            RegisterDefinition(new VystiaBuffDefinition
            {
                Type = VystiaBuffType.MovementSpeedBuff,
                Name = "Swiftness",
                Description = "Increased movement speed",
                Category = BuffCategory.Beneficial,
                StackBehavior = StackBehavior.Replace,
                DefaultDuration = TimeSpan.FromSeconds(30),
                Hue = 0x54E
            });

            RegisterDefinition(new VystiaBuffDefinition
            {
                Type = VystiaBuffType.SongOfSwiftness,
                Name = "Song of Swiftness",
                Description = "Bardic song that increases speed and dexterity",
                Category = BuffCategory.Beneficial,
                StackBehavior = StackBehavior.Replace,
                DefaultDuration = TimeSpan.FromSeconds(30),
                Hue = 0x54E
            });

            // Stat Buffs
            RegisterDefinition(new VystiaBuffDefinition
            {
                Type = VystiaBuffType.StrengthBuff,
                Name = "Strength",
                Description = "Increases Strength",
                Category = BuffCategory.Beneficial,
                DefaultDuration = TimeSpan.FromMinutes(5),
                Hue = 1157
            });

            RegisterDefinition(new VystiaBuffDefinition
            {
                Type = VystiaBuffType.DexterityBuff,
                Name = "Agility",
                Description = "Increases Dexterity",
                Category = BuffCategory.Beneficial,
                DefaultDuration = TimeSpan.FromMinutes(5),
                Hue = 2010
            });

            RegisterDefinition(new VystiaBuffDefinition
            {
                Type = VystiaBuffType.IntelligenceBuff,
                Name = "Cunning",
                Description = "Increases Intelligence",
                Category = BuffCategory.Beneficial,
                DefaultDuration = TimeSpan.FromMinutes(5),
                Hue = 1154
            });

            // DoT Effects
            RegisterDefinition(new VystiaBuffDefinition
            {
                Type = VystiaBuffType.Bleed,
                Name = "Bleeding",
                Description = "Taking physical damage over time",
                Category = BuffCategory.Harmful,
                StackBehavior = StackBehavior.Stack,
                MaxStacks = 5,
                DefaultDuration = TimeSpan.FromSeconds(12),
                TickInterval = TimeSpan.FromSeconds(2),
                Hue = 1157
            });

            RegisterDefinition(new VystiaBuffDefinition
            {
                Type = VystiaBuffType.Burn,
                Name = "Burning",
                Description = "Taking fire damage over time",
                Category = BuffCategory.Harmful,
                StackBehavior = StackBehavior.Refresh,
                MaxStacks = 3,
                DefaultDuration = TimeSpan.FromSeconds(8),
                TickInterval = TimeSpan.FromSeconds(2),
                Hue = 1358
            });

            RegisterDefinition(new VystiaBuffDefinition
            {
                Type = VystiaBuffType.Corruption,
                Name = "Corruption",
                Description = "Shadow energies drain your life",
                Category = BuffCategory.Harmful,
                StackBehavior = StackBehavior.Stack,
                MaxStacks = 3,
                DefaultDuration = TimeSpan.FromSeconds(15),
                TickInterval = TimeSpan.FromSeconds(3),
                Hue = 1109
            });

            // HoT Effects
            RegisterDefinition(new VystiaBuffDefinition
            {
                Type = VystiaBuffType.Rejuvenation,
                Name = "Rejuvenation",
                Description = "Healing over time",
                Category = BuffCategory.Beneficial,
                StackBehavior = StackBehavior.Refresh,
                DefaultDuration = TimeSpan.FromSeconds(15),
                TickInterval = TimeSpan.FromSeconds(3),
                Hue = 2010
            });

            RegisterDefinition(new VystiaBuffDefinition
            {
                Type = VystiaBuffType.LifeBloom,
                Name = "Life Bloom",
                Description = "Nature heals you over time",
                Category = BuffCategory.Beneficial,
                StackBehavior = StackBehavior.Stack,
                MaxStacks = 3,
                DefaultDuration = TimeSpan.FromSeconds(10),
                TickInterval = TimeSpan.FromSeconds(2),
                Hue = 2010
            });

            // Shield Effects
            RegisterDefinition(new VystiaBuffDefinition
            {
                Type = VystiaBuffType.DamageAbsorb,
                Name = "Damage Shield",
                Description = "Absorbs incoming damage",
                Category = BuffCategory.Beneficial,
                StackBehavior = StackBehavior.Replace,
                DefaultDuration = TimeSpan.FromSeconds(30),
                Hue = 1153
            });

            RegisterDefinition(new VystiaBuffDefinition
            {
                Type = VystiaBuffType.ReflectShield,
                Name = "Reflection",
                Description = "Reflects a portion of damage",
                Category = BuffCategory.Beneficial,
                StackBehavior = StackBehavior.Refresh,
                DefaultDuration = TimeSpan.FromSeconds(20),
                Hue = 1154
            });

            // Transform Effects
            RegisterDefinition(new VystiaBuffDefinition
            {
                Type = VystiaBuffType.BearForm,
                Name = "Bear Form",
                Description = "Transformed into a bear",
                Category = BuffCategory.Beneficial,
                StackBehavior = StackBehavior.Ignore,
                DefaultDuration = TimeSpan.Zero, // Permanent until cancelled
                PersistsThroughDeath = false,
                Exclusions = new[] { VystiaBuffType.CatForm, VystiaBuffType.WolfForm, VystiaBuffType.TreeForm, VystiaBuffType.MoonkinForm }
            });

            RegisterDefinition(new VystiaBuffDefinition
            {
                Type = VystiaBuffType.CatForm,
                Name = "Cat Form",
                Description = "Transformed into a cat",
                Category = BuffCategory.Beneficial,
                StackBehavior = StackBehavior.Ignore,
                DefaultDuration = TimeSpan.Zero,
                PersistsThroughDeath = false,
                Exclusions = new[] { VystiaBuffType.BearForm, VystiaBuffType.WolfForm, VystiaBuffType.TreeForm, VystiaBuffType.MoonkinForm }
            });

            RegisterDefinition(new VystiaBuffDefinition
            {
                Type = VystiaBuffType.RageForm,
                Name = "Rage",
                Description = "Consumed by berserker rage",
                Category = BuffCategory.Beneficial,
                StackBehavior = StackBehavior.Refresh,
                DefaultDuration = TimeSpan.FromSeconds(30),
                PersistsThroughDeath = false,
                Hue = 1157
            });

            // Resistance Buffs
            RegisterDefinition(new VystiaBuffDefinition
            {
                Type = VystiaBuffType.AllResist,
                Name = "Protection",
                Description = "Increased resistance to all damage",
                Category = BuffCategory.Beneficial,
                DefaultDuration = TimeSpan.FromMinutes(3),
                Hue = 1153
            });

            // Debuffs
            RegisterDefinition(new VystiaBuffDefinition
            {
                Type = VystiaBuffType.Vulnerability,
                Name = "Vulnerable",
                Description = "Taking increased damage",
                Category = BuffCategory.Harmful,
                DefaultDuration = TimeSpan.FromSeconds(10),
                Hue = 1109
            });

            RegisterDefinition(new VystiaBuffDefinition
            {
                Type = VystiaBuffType.HealingReduction,
                Name = "Mortal Wound",
                Description = "Healing effects reduced",
                Category = BuffCategory.Harmful,
                DefaultDuration = TimeSpan.FromSeconds(8),
                Hue = 1157
            });
        }

        #endregion

        #region Buff Application

        public VystiaBuffInstance ApplyBuff(Mobile target, Mobile caster, VystiaBuffType type, TimeSpan duration, int value = 0)
        {
            if (target == null || target.Deleted)
                return null;

            VystiaBuffDefinition def = GetDefinition(type);
            if (def == null)
            {
                // Create default definition
                def = new VystiaBuffDefinition
                {
                    Type = type,
                    Name = type.ToString(),
                    Description = type.ToString(),
                    Category = BuffCategory.Neutral,
                    DefaultDuration = duration
                };
            }

            // Handle exclusions
            if (def.Exclusions != null && def.Exclusions.Length > 0)
            {
                foreach (VystiaBuffType excluded in def.Exclusions)
                {
                    RemoveBuff(target, excluded);
                }
            }

            // Get or create buff list
            if (!m_ActiveBuffs.TryGetValue(target, out List<VystiaBuffInstance> buffs))
            {
                buffs = new List<VystiaBuffInstance>();
                m_ActiveBuffs[target] = buffs;
            }

            // Vystia: Apply class-religion synergy duration bonuses
            TimeSpan finalDuration = duration;
            int finalValue = value;
            if (caster is PlayerMobile pm)
            {
                string buffType = "";
                // Map buff types to synergy check names
                if (type.ToString().Contains("Shapeshift") || type.ToString().Contains("Form"))
                    buffType = "Shapeshift";
                else if (type.ToString().Contains("Hex"))
                    buffType = "Hex";
                else if (type.ToString().Contains("Totem"))
                    buffType = "Totem";
                else if (type.ToString().Contains("Freeze") || type.ToString().Contains("Frozen"))
                    buffType = "Freeze";

                if (!string.IsNullOrEmpty(buffType))
                {
                    double synergyBonus = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetClassReligionSynergyDurationBonus(pm, buffType);
                    if (synergyBonus > 0.0)
                    {
                        finalDuration = TimeSpan.FromMilliseconds(duration.TotalMilliseconds * (1.0 + synergyBonus));
                    }
                }

                // Vystia: Religion buff effectiveness (opposed religion: 50% effectiveness)
                // Apply effectiveness to buff value for beneficial buffs
                if (target is PlayerMobile targetPM && def.Category == BuffCategory.Beneficial)
                {
                    double effectiveness = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetReligionHealingEffectiveness(pm, targetPM);
                    if (effectiveness < 1.0)
                    {
                        finalValue = (int)(finalValue * effectiveness); // Reduce buff value for opposed religions
                    }
                }
            }

            // Check for existing buff
            VystiaBuffInstance existing = buffs.Find(b => b.Definition.Type == type);

            if (existing != null)
            {
                switch (def.StackBehavior)
                {
                    case StackBehavior.Refresh:
                        existing.Refresh(finalDuration);
                        UpdateBuffBar(existing);
                        return existing;

                    case StackBehavior.Stack:
                        existing.AddStack();
                        existing.Refresh(finalDuration);
                        UpdateBuffBar(existing);
                        return existing;

                    case StackBehavior.Extend:
                        existing.Extend(finalDuration);
                        UpdateBuffBar(existing);
                        return existing;

                    case StackBehavior.Replace:
                        existing.OnRemove();
                        buffs.Remove(existing);
                        break;

                    case StackBehavior.Ignore:
                        return existing;
                }
            }

            // Create new buff instance
            VystiaBuffInstance instance = new VystiaBuffInstance(def, target, caster, finalDuration);

            // Set values based on type (use finalValue which includes effectiveness multiplier)
            SetBuffValues(instance, type, finalValue);

            buffs.Add(instance);
            instance.OnApply();
            SendBuffStatMessages(target, instance);
            UpdateBuffBar(instance);

            return instance;
        }

        private void SetBuffValues(VystiaBuffInstance instance, VystiaBuffType type, int value)
        {
            switch (type)
            {
                case VystiaBuffType.MovementSpeedBuff:
                    instance.SpeedModifier = value > 0 ? 1.0 + (value / 100.0) : 1.1;
                    break;
                case VystiaBuffType.SongOfSwiftness:
                    instance.StatModifier = value > 0 ? value : 5;
                    instance.SpeedModifier = value > 0 ? 1.0 + (value / 100.0) : 1.1;
                    break;

                // Stat buffs/debuffs
                case VystiaBuffType.StrengthBuff:
                case VystiaBuffType.DexterityBuff:
                case VystiaBuffType.IntelligenceBuff:
                case VystiaBuffType.AllStatsBuff:
                case VystiaBuffType.StrengthDebuff:
                case VystiaBuffType.DexterityDebuff:
                case VystiaBuffType.IntelligenceDebuff:
                case VystiaBuffType.AllStatsDebuff:
                    instance.StatModifier = value > 0 ? value : 10;
                    break;

                // Resist buffs
                case VystiaBuffType.PhysicalResist:
                case VystiaBuffType.FireResist:
                case VystiaBuffType.ColdResist:
                case VystiaBuffType.PoisonResist:
                case VystiaBuffType.EnergyResist:
                case VystiaBuffType.AllResist:
                    instance.ResistModifier = value > 0 ? value : 15;
                    break;

                // DoTs
                case VystiaBuffType.Bleed:
                case VystiaBuffType.Burn:
                case VystiaBuffType.Corruption:
                case VystiaBuffType.SoulDrain:
                case VystiaBuffType.Poison:
                    instance.DamagePerTick = value > 0 ? value : 5;
                    break;

                // HoTs
                case VystiaBuffType.Rejuvenation:
                case VystiaBuffType.LifeBloom:
                case VystiaBuffType.Tranquility:
                    instance.HealPerTick = value > 0 ? value : 8;
                    break;

                // Shields
                case VystiaBuffType.DamageAbsorb:
                    instance.AbsorbRemaining = value > 0 ? value : 50;
                    break;

                case VystiaBuffType.ReflectShield:
                    instance.ReflectPercent = value > 0 ? value : 25;
                    break;

                // Transforms
                case VystiaBuffType.BearForm:
                    instance.TransformBody = 212; // Bear body
                    instance.StatModifier = 25;
                    instance.AddStatMod(StatType.Str, 25);
                    instance.AddResistMod(ResistanceType.Physical, 15);
                    break;

                case VystiaBuffType.CatForm:
                    instance.TransformBody = 201; // Cat body
                    instance.StatModifier = 20;
                    instance.AddStatMod(StatType.Dex, 20);
                    instance.SpeedModifier = 1.25;
                    break;

                case VystiaBuffType.WolfForm:
                    instance.TransformBody = 225; // Wolf body
                    instance.StatModifier = 15;
                    instance.AddStatMod(StatType.Str, 10);
                    instance.AddStatMod(StatType.Dex, 15);
                    break;

                case VystiaBuffType.RageForm:
                    instance.TransformHue = 1157;
                    instance.StatModifier = 30;
                    instance.AddStatMod(StatType.Str, 30);
                    break;
            }
        }

        public void RemoveBuff(Mobile target, VystiaBuffType type)
        {
            if (!m_ActiveBuffs.TryGetValue(target, out List<VystiaBuffInstance> buffs))
                return;

            VystiaBuffInstance buff = buffs.Find(b => b.Definition.Type == type);
            if (buff != null)
            {
                buff.OnRemove();
                RemoveBuffBar(buff);
                buffs.Remove(buff);
            }

            if (buffs.Count == 0)
            {
                m_ActiveBuffs.Remove(target);
            }
        }

        public void RemoveAllBuffs(Mobile target, bool beneficialOnly = false, bool harmfulOnly = false)
        {
            if (!m_ActiveBuffs.TryGetValue(target, out List<VystiaBuffInstance> buffs))
                return;

            List<VystiaBuffInstance> toRemove = new List<VystiaBuffInstance>();

            foreach (var buff in buffs)
            {
                if (beneficialOnly && buff.Definition.Category != BuffCategory.Beneficial)
                    continue;
                if (harmfulOnly && buff.Definition.Category != BuffCategory.Harmful)
                    continue;
                if (!buff.Definition.Dispellable)
                    continue;

                toRemove.Add(buff);
            }

            foreach (var buff in toRemove)
            {
                buff.OnRemove();
                RemoveBuffBar(buff);
                buffs.Remove(buff);
            }

            if (buffs.Count == 0)
            {
                m_ActiveBuffs.Remove(target);
            }
        }

        #endregion

        #region Buff Queries

        public bool HasBuff(Mobile target, VystiaBuffType type)
        {
            if (!m_ActiveBuffs.TryGetValue(target, out List<VystiaBuffInstance> buffs))
                return false;

            return buffs.Exists(b => b.Definition.Type == type && !b.IsExpired);
        }

        public VystiaBuffInstance GetBuff(Mobile target, VystiaBuffType type)
        {
            if (!m_ActiveBuffs.TryGetValue(target, out List<VystiaBuffInstance> buffs))
                return null;

            return buffs.Find(b => b.Definition.Type == type && !b.IsExpired);
        }

        public List<VystiaBuffInstance> GetAllBuffs(Mobile target)
        {
            if (!m_ActiveBuffs.TryGetValue(target, out List<VystiaBuffInstance> buffs))
                return new List<VystiaBuffInstance>();

            return new List<VystiaBuffInstance>(buffs);
        }

        public int GetBuffStacks(Mobile target, VystiaBuffType type)
        {
            VystiaBuffInstance buff = GetBuff(target, type);
            return buff?.Stacks ?? 0;
        }

        #endregion

        #region Damage Interception

        public int ProcessIncomingDamage(Mobile target, Mobile attacker, int damage)
        {
            if (!m_ActiveBuffs.TryGetValue(target, out List<VystiaBuffInstance> buffs))
                return damage;

            int remaining = damage;

            foreach (var buff in buffs)
            {
                // Damage absorption
                if (buff.AbsorbRemaining > 0)
                {
                    remaining = buff.AbsorbDamage(remaining);
                    if (remaining <= 0)
                        break;
                }

                // Damage reflection
                if (buff.ReflectPercent > 0 && attacker != null)
                {
                    buff.ReflectDamage(remaining, attacker);
                }
            }

            // Check for vulnerability debuff
            VystiaBuffInstance vuln = buffs.Find(b => b.Definition.Type == VystiaBuffType.Vulnerability);
            if (vuln != null)
            {
                remaining = (int)(remaining * 1.25); // +25% damage taken
            }

            return remaining;
        }

        public int ProcessOutgoingDamage(Mobile attacker, Mobile target, int damage)
        {
            if (!m_ActiveBuffs.TryGetValue(attacker, out List<VystiaBuffInstance> buffs))
                return damage;

            int modified = damage;

            foreach (var buff in buffs)
            {
                switch (buff.Definition.Type)
                {
                    case VystiaBuffType.DamageIncrease:
                        modified = (int)(modified * (1 + buff.StatModifier / 100.0));
                        break;
                    case VystiaBuffType.RageForm:
                        modified = (int)(modified * 1.3); // +30% damage in rage
                        break;
                }
            }

            return modified;
        }

        public int ProcessHealing(Mobile target, int heal)
        {
            if (!m_ActiveBuffs.TryGetValue(target, out List<VystiaBuffInstance> buffs))
                return heal;

            VystiaBuffInstance mortal = buffs.Find(b => b.Definition.Type == VystiaBuffType.HealingReduction);
            if (mortal != null)
            {
                return heal / 2; // 50% healing reduction
            }

            return heal;
        }

        #endregion

        #region Tick Processing

        private void ProcessTicks()
        {
            List<Mobile> emptyTargets = new List<Mobile>();

            foreach (var kvp in m_ActiveBuffs)
            {
                Mobile target = kvp.Key;
                List<VystiaBuffInstance> buffs = kvp.Value;

                if (target == null || target.Deleted)
                {
                    emptyTargets.Add(target);
                    continue;
                }

                List<VystiaBuffInstance> expired = new List<VystiaBuffInstance>();

                foreach (var buff in buffs)
                {
                    // Check expiration (skip permanent buffs with Zero duration)
                    if (buff.Definition.DefaultDuration != TimeSpan.Zero && buff.IsExpired)
                    {
                        expired.Add(buff);
                        continue;
                    }

                    // Shield depleted
                    if (buff.Definition.Type == VystiaBuffType.DamageAbsorb && buff.AbsorbRemaining <= 0)
                    {
                        expired.Add(buff);
                        continue;
                    }

                    // Process tick effects (DoTs/HoTs)
                    if (buff.Definition.TickInterval > TimeSpan.Zero)
                    {
                        if (DateTime.UtcNow - buff.LastTick >= buff.Definition.TickInterval)
                        {
                            buff.OnTick();
                        }
                    }
                }

                // Remove expired buffs
                    foreach (var buff in expired)
                    {
                        buff.OnRemove();
                        RemoveBuffBar(buff);
                        buffs.Remove(buff);
                    }

                if (buffs.Count == 0)
                {
                    emptyTargets.Add(target);
                }
            }

            // Cleanup empty entries
            foreach (Mobile target in emptyTargets)
            {
                m_ActiveBuffs.Remove(target);
            }
        }

        #endregion

        private void SendBuffStatMessages(Mobile target, VystiaBuffInstance instance)
        {
            if (target == null || instance == null)
                return;

            switch (instance.Definition.Type)
            {
                case VystiaBuffType.AllStatsBuff:
                    SendStatChangeMessage(target, "strength", instance.StatModifier, target.Str);
                    SendStatChangeMessage(target, "dexterity", instance.StatModifier, target.Dex);
                    SendStatChangeMessage(target, "intelligence", instance.StatModifier, target.Int);
                    break;
                case VystiaBuffType.SongOfSwiftness:
                    SendStatChangeMessage(target, "dexterity", instance.StatModifier, target.Dex);
                    int speedPercent = (int)Math.Round((instance.SpeedModifier - 1.0) * 100);
                    if (speedPercent > 0)
                        target.SendMessage(0x3B2, $"Your movement speed has increased by {speedPercent}%.");
                    break;
                case VystiaBuffType.Rejuvenation:
                    int heal = instance.HealPerTick;
                    int healInterval = (int)instance.Definition.TickInterval.TotalSeconds;
                    target.SendMessage(0x3B2, $"Song of Healing: {heal} heal every {healInterval}s.");
                    break;
                case VystiaBuffType.Vulnerability:
                    target.SendMessage(0x22, "Discordant Note: +25% damage taken.");
                    break;
                case VystiaBuffType.Corruption:
                    int damage = instance.DamagePerTick;
                    int tickInterval = (int)instance.Definition.TickInterval.TotalSeconds;
                    target.SendMessage(0x22, $"Dirge of Weakness: {damage} damage every {tickInterval}s.");
                    break;
            }
        }

        private void SendStatChangeMessage(Mobile target, string statName, int delta, int newValue)
        {
            if (target == null || delta == 0)
                return;

            target.SendMessage(0x3B2, $"Your {statName} has changed by {delta}. It is now {newValue}.");
        }

        private void UpdateBuffBar(VystiaBuffInstance instance)
        {
            if (instance == null || instance.Target == null || !BuffInfo.Enabled)
                return;

            BuffIcon? icon = GetBuffIcon(instance.Definition.Type);
            if (icon == null)
                return;

            string label = GetBuffLabel(instance);
            TimeSpan length = instance.RemainingDuration;
            BuffInfo.AddBuff(instance.Target, new BuffInfo(icon.Value, BuffInfo.Blank, BuffInfo.Blank, length, instance.Target, label));
        }

        private void RemoveBuffBar(VystiaBuffInstance instance)
        {
            if (instance == null || instance.Target == null || !BuffInfo.Enabled)
                return;

            BuffIcon? icon = GetBuffIcon(instance.Definition.Type);
            if (icon == null)
                return;

            BuffInfo.RemoveBuff(instance.Target, icon.Value);
        }

        private BuffIcon? GetBuffIcon(VystiaBuffType type)
        {
            switch (type)
            {
                case VystiaBuffType.AllStatsBuff:
                    return BuffIcon.Inspire;
                case VystiaBuffType.SongOfSwiftness:
                    return BuffIcon.Invigorate;
                case VystiaBuffType.Rejuvenation:
                    return BuffIcon.Resilience;
                case VystiaBuffType.Vulnerability:
                    return BuffIcon.TribulationTarget;
                case VystiaBuffType.Corruption:
                    return BuffIcon.DespairTarget;
                default:
                    return null;
            }
        }

        private string GetBuffLabel(VystiaBuffInstance instance)
        {
            switch (instance.Definition.Type)
            {
                case VystiaBuffType.AllStatsBuff:
                    return $"Song of Courage (+{instance.StatModifier} all stats)";
                case VystiaBuffType.SongOfSwiftness:
                    {
                        int speedPercent = (int)Math.Round((instance.SpeedModifier - 1.0) * 100);
                        return $"Song of Swiftness (+{instance.StatModifier} Dex, +{speedPercent}% speed)";
                    }
                case VystiaBuffType.Rejuvenation:
                    return $"Song of Healing (+{instance.HealPerTick}/tick)";
                case VystiaBuffType.Vulnerability:
                    return "Discordant Note (+25% damage taken)";
                case VystiaBuffType.Corruption:
                    return $"Dirge of Weakness ({instance.DamagePerTick} dmg/tick)";
                default:
                    return instance.Definition.Name ?? instance.Definition.Type.ToString();
            }
        }

        #region GM Commands

        public static void Initialize()
        {
            CommandSystem.Register("ApplyBuff", AccessLevel.GameMaster, ApplyBuff_OnCommand);
            CommandSystem.Register("RemoveBuff", AccessLevel.GameMaster, RemoveBuff_OnCommand);
            CommandSystem.Register("ListBuffs", AccessLevel.GameMaster, ListBuffs_OnCommand);
            CommandSystem.Register("ClearBuffs", AccessLevel.GameMaster, ClearBuffs_OnCommand);
        }

        [Usage("ApplyBuff <type> [duration] [value]")]
        [Description("Applies a buff to a target")]
        private static void ApplyBuff_OnCommand(CommandEventArgs e)
        {
            if (e.Arguments.Length < 1)
            {
                e.Mobile.SendMessage("Usage: ApplyBuff <type> [duration] [value]");
                e.Mobile.SendMessage("Types: StrengthBuff, Bleed, Burn, Rejuvenation, DamageAbsorb, BearForm, etc.");
                return;
            }

            if (!Enum.TryParse(e.Arguments[0], true, out VystiaBuffType type))
            {
                e.Mobile.SendMessage("Invalid buff type: {0}", e.Arguments[0]);
                return;
            }

            int duration = 30;
            if (e.Arguments.Length >= 2)
            {
                int.TryParse(e.Arguments[1], out duration);
            }

            int value = 0;
            if (e.Arguments.Length >= 3)
            {
                int.TryParse(e.Arguments[2], out value);
            }

            e.Mobile.SendMessage("Target a creature to apply {0}:", type);
            e.Mobile.Target = new InternalTarget((target) =>
            {
                if (target is Mobile m)
                {
                    Instance.ApplyBuff(m, e.Mobile, type, TimeSpan.FromSeconds(duration), value);
                    e.Mobile.SendMessage("Applied {0} to {1} for {2} seconds", type, m.Name, duration);
                }
            });
        }

        [Usage("RemoveBuff <type>")]
        [Description("Removes a specific buff from a target")]
        private static void RemoveBuff_OnCommand(CommandEventArgs e)
        {
            if (e.Arguments.Length < 1)
            {
                e.Mobile.SendMessage("Usage: RemoveBuff <type>");
                return;
            }

            if (!Enum.TryParse(e.Arguments[0], true, out VystiaBuffType type))
            {
                e.Mobile.SendMessage("Invalid buff type: {0}", e.Arguments[0]);
                return;
            }

            e.Mobile.SendMessage("Target a creature to remove {0}:", type);
            e.Mobile.Target = new InternalTarget((target) =>
            {
                if (target is Mobile m)
                {
                    Instance.RemoveBuff(m, type);
                    e.Mobile.SendMessage("Removed {0} from {1}", type, m.Name);
                }
            });
        }

        [Usage("ListBuffs")]
        [Description("Lists all active buffs on a target")]
        private static void ListBuffs_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Target a creature to list buffs:");
            e.Mobile.Target = new InternalTarget((target) =>
            {
                if (target is Mobile m)
                {
                    List<VystiaBuffInstance> buffs = Instance.GetAllBuffs(m);
                    e.Mobile.SendMessage("=== Active Buffs on {0} ({1}) ===", m.Name, buffs.Count);

                    foreach (var buff in buffs)
                    {
                        string duration = buff.Definition.DefaultDuration == TimeSpan.Zero
                            ? "Permanent"
                            : string.Format("{0:F1}s remaining", buff.RemainingDuration.TotalSeconds);

                        e.Mobile.SendMessage("  {0} x{1} ({2})",
                            buff.Definition.Name,
                            buff.Stacks,
                            duration);
                    }
                }
            });
        }

        [Usage("ClearBuffs [all|beneficial|harmful]")]
        [Description("Clears buffs from a target")]
        private static void ClearBuffs_OnCommand(CommandEventArgs e)
        {
            bool beneficialOnly = false;
            bool harmfulOnly = false;

            if (e.Arguments.Length >= 1)
            {
                string arg = e.Arguments[0].ToLower();
                if (arg == "beneficial")
                    beneficialOnly = true;
                else if (arg == "harmful")
                    harmfulOnly = true;
            }

            e.Mobile.SendMessage("Target a creature to clear buffs:");
            e.Mobile.Target = new InternalTarget((target) =>
            {
                if (target is Mobile m)
                {
                    Instance.RemoveAllBuffs(m, beneficialOnly, harmfulOnly);
                    e.Mobile.SendMessage("Cleared buffs from {0}", m.Name);
                }
            });
        }

        private class InternalTarget : Server.Targeting.Target
        {
            private Action<object> m_Callback;

            public InternalTarget(Action<object> callback) : base(12, false, Server.Targeting.TargetFlags.None)
            {
                m_Callback = callback;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                m_Callback?.Invoke(targeted);
            }
        }

        #endregion
    }

    #endregion
}
