using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Commands;

namespace Server.Custom.VystiaClasses.Systems
{
    #region Enums

    /// <summary>
    /// Extended damage types for Vystia system
    /// </summary>
    public enum VystiaDamageType
    {
        Physical,
        Fire,
        Cold,
        Poison,
        Energy,
        // Extended types
        Shadow,     // Void/dark damage
        Holy,       // Divine/light damage
        Arcane,     // Pure magic damage
        Nature,     // Druid/nature damage
        Bleed       // Physical DoT that ignores armor
    }

    /// <summary>
    /// Types of on-hit effects
    /// </summary>
    public enum OnHitEffect
    {
        None,
        ApplyChill,
        ApplyBleed,
        ApplyBurn,
        ApplyCorruption,
        ApplyPoison,
        ApplySlow,
        ApplyWeaken,
        LifeSteal,
        ManaSteal,
        StaminaDrain,
        Knockback,
        Stun,
        SoulShardGenerate,
        FuryGenerate,
        ComboPointGenerate,
        ChiGenerate
    }

    #endregion

    #region Damage Context

    /// <summary>
    /// Contains all context for a damage calculation
    /// </summary>
    public class DamageContext
    {
        // Source and target
        public Mobile Source { get; set; }
        public Mobile Target { get; set; }

        // Base damage
        public int MinDamage { get; set; }
        public int MaxDamage { get; set; }

        // Damage type distribution (must total 100)
        public int PhysicalPercent { get; set; }
        public int FirePercent { get; set; }
        public int ColdPercent { get; set; }
        public int PoisonPercent { get; set; }
        public int EnergyPercent { get; set; }

        // Extended damage types (overlay on Energy for now)
        public VystiaDamageType PrimaryType { get; set; }

        // Critical hit
        public bool CanCrit { get; set; }
        public double CritChance { get; set; }      // 0.0 - 1.0
        public double CritMultiplier { get; set; }  // Default 1.5 = 150% damage
        public bool IsCrit { get; private set; }

        // Modifiers
        public double DamageMultiplier { get; set; }        // Stacks multiplicatively
        public int FlatDamageBonus { get; set; }            // Adds after scaling
        public int ArmorPenetration { get; set; }           // Reduces target armor
        public int ResistancePenetration { get; set; }      // Reduces target resists

        // Flags
        public bool CanBeBlocked { get; set; }
        public bool CanBeDodged { get; set; }
        public bool CanBeMissed { get; set; }
        public bool IgnoresArmor { get; set; }
        public bool IgnoresResistance { get; set; }
        public bool IsSpell { get; set; }
        public bool IsAbility { get; set; }
        public bool IsAutoAttack { get; set; }

        // On-hit effects
        public List<OnHitEffect> OnHitEffects { get; set; }
        public Dictionary<OnHitEffect, int> OnHitValues { get; set; }

        // Results (filled after calculation)
        public int RawDamage { get; set; }
        public int FinalDamage { get; set; }
        public bool WasBlocked { get; set; }
        public bool WasDodged { get; set; }
        public bool WasMissed { get; set; }
        public bool WasAbsorbed { get; set; }
        public int AbsorbedAmount { get; set; }
        public int ReflectedAmount { get; set; }

        // Resource generation
        public Dictionary<ResourceType, int> ResourceGenerated { get; set; }

        public DamageContext()
        {
            // Defaults
            PhysicalPercent = 100;
            FirePercent = 0;
            ColdPercent = 0;
            PoisonPercent = 0;
            EnergyPercent = 0;
            PrimaryType = VystiaDamageType.Physical;

            CanCrit = true;
            CritChance = 0.05;      // 5% base crit
            CritMultiplier = 1.5;   // 150% crit damage

            DamageMultiplier = 1.0;
            FlatDamageBonus = 0;
            ArmorPenetration = 0;
            ResistancePenetration = 0;

            CanBeBlocked = true;
            CanBeDodged = true;
            CanBeMissed = true;
            IgnoresArmor = false;
            IgnoresResistance = false;
            IsSpell = false;
            IsAbility = false;
            IsAutoAttack = false;

            OnHitEffects = new List<OnHitEffect>();
            OnHitValues = new Dictionary<OnHitEffect, int>();
            ResourceGenerated = new Dictionary<ResourceType, int>();
        }

        public void SetDamageRange(int min, int max)
        {
            MinDamage = min;
            MaxDamage = max;
        }

        public void SetDamageType(VystiaDamageType type)
        {
            PrimaryType = type;

            // Reset percentages
            PhysicalPercent = 0;
            FirePercent = 0;
            ColdPercent = 0;
            PoisonPercent = 0;
            EnergyPercent = 0;

            switch (type)
            {
                case VystiaDamageType.Physical:
                case VystiaDamageType.Bleed:
                    PhysicalPercent = 100;
                    break;
                case VystiaDamageType.Fire:
                    FirePercent = 100;
                    break;
                case VystiaDamageType.Cold:
                    ColdPercent = 100;
                    break;
                case VystiaDamageType.Poison:
                    PoisonPercent = 100;
                    break;
                case VystiaDamageType.Energy:
                case VystiaDamageType.Arcane:
                    EnergyPercent = 100;
                    break;
                case VystiaDamageType.Shadow:
                    EnergyPercent = 50;
                    PoisonPercent = 50;
                    break;
                case VystiaDamageType.Holy:
                    EnergyPercent = 50;
                    FirePercent = 50;
                    break;
                case VystiaDamageType.Nature:
                    EnergyPercent = 50;
                    PoisonPercent = 50;
                    break;
            }
        }

        public void AddOnHitEffect(OnHitEffect effect, int value = 0)
        {
            if (!OnHitEffects.Contains(effect))
            {
                OnHitEffects.Add(effect);
            }
            OnHitValues[effect] = value;
        }

        public void RollCrit()
        {
            if (CanCrit && Utility.RandomDouble() < CritChance)
            {
                IsCrit = true;
            }
        }
    }

    #endregion

    #region Heal Context

    /// <summary>
    /// Contains all context for a healing calculation
    /// </summary>
    public class HealContext
    {
        public Mobile Source { get; set; }
        public Mobile Target { get; set; }

        public int MinHeal { get; set; }
        public int MaxHeal { get; set; }

        public bool IsHoT { get; set; }
        public bool IsAbsorb { get; set; }
        public bool CanCrit { get; set; }
        public double CritChance { get; set; }
        public double CritMultiplier { get; set; }
        public bool IsCrit { get; private set; }

        public double HealMultiplier { get; set; }
        public int FlatHealBonus { get; set; }

        // Results
        public int RawHeal { get; set; }
        public int FinalHeal { get; set; }
        public int Overheal { get; set; }

        // Resource generation
        public Dictionary<ResourceType, int> ResourceGenerated { get; set; }

        public HealContext()
        {
            CanCrit = false;
            CritChance = 0.10;
            CritMultiplier = 1.5;
            HealMultiplier = 1.0;
            FlatHealBonus = 0;
            ResourceGenerated = new Dictionary<ResourceType, int>();
        }

        public void SetHealRange(int min, int max)
        {
            MinHeal = min;
            MaxHeal = max;
        }

        public void RollCrit()
        {
            if (CanCrit && Utility.RandomDouble() < CritChance)
            {
                IsCrit = true;
            }
        }
    }

    #endregion

    #region Damage Calculator

    /// <summary>
    /// Handles all damage calculations for Vystia system
    /// </summary>
    public static class VystiaDamageCalculator
    {
        /// <summary>
        /// Calculate and apply damage using the Vystia system
        /// </summary>
        public static int Calculate(DamageContext ctx)
        {
            if (ctx.Source == null || ctx.Target == null || ctx.Target.Deleted || !ctx.Target.Alive)
                return 0;

            // Step 1: Roll base damage
            ctx.RawDamage = Utility.RandomMinMax(ctx.MinDamage, ctx.MaxDamage);

            // Step 2: Roll critical hit
            ctx.RollCrit();
            if (ctx.IsCrit)
            {
                ctx.RawDamage = (int)(ctx.RawDamage * ctx.CritMultiplier);

                // Notify
                ctx.Source.SendMessage(0x3B2, "Critical hit!");
                ctx.Target.SendMessage(0x22, "Critical hit!");

                // Visual effect
                ctx.Target.FixedParticles(0x37B9, 1, 5, 9910, 0, 0, EffectLayer.Head);
            }

            // Step 3: Apply damage multiplier
            int scaledDamage = (int)(ctx.RawDamage * ctx.DamageMultiplier);

            // Vystia: Religion damage bonuses (Frosthelm Faith/Surya's Sandscript Devoted: +3% damage)
            if (ctx.Source is PlayerMobile pm)
            {
                double religionDamageBonus = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetReligionDamageBonus(pm, ctx.PrimaryType);
                if (religionDamageBonus > 0)
                {
                    scaledDamage = (int)(scaledDamage * (1.0 + religionDamageBonus)); // +3% damage
                }

                // Vystia: Religion PvP damage bonus vs opposed religion
                if (ctx.Target is PlayerMobile targetPM)
                {
                    double pvpBonus = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetReligionPvPDamageBonus(pm, targetPM);
                    if (pvpBonus > 0)
                    {
                        scaledDamage = (int)(scaledDamage * (1.0 + pvpBonus)); // +2% to +8% based on tier
                    }
                }
            }

            // Vystia: Religion PvP damage reduction (Champion tier: -3% damage taken)
            if (ctx.Target is PlayerMobile defenderPM && ctx.Source is PlayerMobile attackerPM)
            {
                double pvpReduction = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetReligionPvPDamageReduction(defenderPM, attackerPM);
                if (pvpReduction > 0)
                {
                    scaledDamage = (int)(scaledDamage * (1.0 - pvpReduction)); // -3% damage taken
                }
            }

            // Step 4: Apply flat bonus
            scaledDamage += ctx.FlatDamageBonus;

            // Step 5: Apply source buffs (from VystiaBuffSystem)
            scaledDamage = VystiaBuffManager.Instance.ProcessOutgoingDamage(ctx.Source, ctx.Target, scaledDamage);

            // Step 6: Calculate effective resists (with penetration)
            int physRes = ctx.Target.PhysicalResistance;
            int fireRes = ctx.Target.FireResistance;
            int coldRes = ctx.Target.ColdResistance;
            int poisRes = ctx.Target.PoisonResistance;
            int energyRes = ctx.Target.EnergyResistance;

            if (!ctx.IgnoresResistance)
            {
                // Apply resistance penetration
                physRes = Math.Max(0, physRes - ctx.ResistancePenetration);
                fireRes = Math.Max(0, fireRes - ctx.ResistancePenetration);
                coldRes = Math.Max(0, coldRes - ctx.ResistancePenetration);
                poisRes = Math.Max(0, poisRes - ctx.ResistancePenetration);
                energyRes = Math.Max(0, energyRes - ctx.ResistancePenetration);
            }
            else
            {
                physRes = fireRes = coldRes = poisRes = energyRes = 0;
            }

            // Step 7: Apply resistances (UO style)
            int finalDamage = scaledDamage;
            if (!ctx.IgnoresResistance)
            {
                // Calculate damage after resistances
                int physDmg = (scaledDamage * ctx.PhysicalPercent / 100) * (100 - physRes) / 100;
                int fireDmg = (scaledDamage * ctx.FirePercent / 100) * (100 - fireRes) / 100;
                int coldDmg = (scaledDamage * ctx.ColdPercent / 100) * (100 - coldRes) / 100;
                int poisDmg = (scaledDamage * ctx.PoisonPercent / 100) * (100 - poisRes) / 100;
                int energyDmg = (scaledDamage * ctx.EnergyPercent / 100) * (100 - energyRes) / 100;

                finalDamage = physDmg + fireDmg + coldDmg + poisDmg + energyDmg;
            }

            // Step 8: Apply target debuffs (shields, absorbs)
            finalDamage = VystiaBuffManager.Instance.ProcessIncomingDamage(ctx.Target, ctx.Source, finalDamage);

            // Step 9: Ensure minimum damage
            finalDamage = Math.Max(1, finalDamage);

            ctx.FinalDamage = finalDamage;

            // Step 10: Apply the actual damage
            AOS.Damage(ctx.Target, ctx.Source, finalDamage,
                ctx.PhysicalPercent, ctx.FirePercent, ctx.ColdPercent, ctx.PoisonPercent, ctx.EnergyPercent);

            // Step 11: Process on-hit effects
            ProcessOnHitEffects(ctx);

            // Step 12: Generate resources
            GenerateResources(ctx);

            // Step 13: Record hit in target tracker
            VystiaTargetTracker.Instance.RecordHit(ctx.Source, ctx.Target, finalDamage);

            return finalDamage;
        }

        private static void ProcessOnHitEffects(DamageContext ctx)
        {
            foreach (OnHitEffect effect in ctx.OnHitEffects)
            {
                int value = ctx.OnHitValues.ContainsKey(effect) ? ctx.OnHitValues[effect] : 0;

                switch (effect)
                {
                    case OnHitEffect.ApplyChill:
                        VystiaTargetTracker.Instance.AddStacks(ctx.Source, ctx.Target, StackType.Chill, 1, TimeSpan.FromSeconds(10));
                        break;

                    case OnHitEffect.ApplyBleed:
                        VystiaBuffManager.Instance.ApplyBuff(ctx.Target, ctx.Source, VystiaBuffType.Bleed, TimeSpan.FromSeconds(12), value > 0 ? value : 5);
                        break;

                    case OnHitEffect.ApplyBurn:
                        VystiaBuffManager.Instance.ApplyBuff(ctx.Target, ctx.Source, VystiaBuffType.Burn, TimeSpan.FromSeconds(8), value > 0 ? value : 8);
                        break;

                    case OnHitEffect.ApplyCorruption:
                        VystiaBuffManager.Instance.ApplyBuff(ctx.Target, ctx.Source, VystiaBuffType.Corruption, TimeSpan.FromSeconds(15), value > 0 ? value : 6);
                        break;

                    case OnHitEffect.ApplyPoison:
                        VystiaBuffManager.Instance.ApplyBuff(ctx.Target, ctx.Source, VystiaBuffType.Poison, TimeSpan.FromSeconds(10), value > 0 ? value : 4);
                        break;

                    case OnHitEffect.ApplySlow:
                        VystiaBuffManager.Instance.ApplyBuff(ctx.Target, ctx.Source, VystiaBuffType.SlowDebuff, TimeSpan.FromSeconds(5), 25);
                        break;

                    case OnHitEffect.ApplyWeaken:
                        VystiaBuffManager.Instance.ApplyBuff(ctx.Target, ctx.Source, VystiaBuffType.StrengthDebuff, TimeSpan.FromSeconds(10), 15);
                        break;

                    case OnHitEffect.LifeSteal:
                        int lifeSteal = Math.Max(1, (ctx.FinalDamage * (value > 0 ? value : 15)) / 100);
                        ctx.Source.Heal(lifeSteal, ctx.Source, false);
                        ctx.Source.FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);
                        break;

                    case OnHitEffect.ManaSteal:
                        int manaSteal = Math.Max(1, (ctx.FinalDamage * (value > 0 ? value : 10)) / 100);
                        ctx.Source.Mana = Math.Min(ctx.Source.ManaMax, ctx.Source.Mana + manaSteal);
                        ctx.Source.FixedParticles(0x374A, 9, 32, 5007, 1154, 0, EffectLayer.Waist);
                        break;

                    case OnHitEffect.StaminaDrain:
                        int stamDrain = Math.Max(1, value > 0 ? value : 10);
                        ctx.Target.Stam = Math.Max(0, ctx.Target.Stam - stamDrain);
                        break;

                    case OnHitEffect.Knockback:
                        // Calculate direction and push target
                        Direction dir = ctx.Source.GetDirectionTo(ctx.Target);
                        int tiles = value > 0 ? value : 2;
                        PushMobile(ctx.Target, dir, tiles);
                        break;

                    case OnHitEffect.Stun:
                        int stunDuration = value > 0 ? value : 2;
                        ctx.Target.Freeze(TimeSpan.FromSeconds(stunDuration));
                        ctx.Target.SendMessage(0x22, "You are stunned!");
                        break;
                }
            }
        }

        private static void PushMobile(Mobile m, Direction dir, int tiles)
        {
            if (m == null || m.Map == null)
                return;

            int xOffset = 0, yOffset = 0;

            switch (dir & Direction.Mask)
            {
                case Direction.North: yOffset = -1; break;
                case Direction.South: yOffset = 1; break;
                case Direction.West: xOffset = -1; break;
                case Direction.East: xOffset = 1; break;
                case Direction.Up: xOffset = -1; yOffset = -1; break;
                case Direction.Down: xOffset = 1; yOffset = 1; break;
                case Direction.Left: xOffset = -1; yOffset = 1; break;
                case Direction.Right: xOffset = 1; yOffset = -1; break;
            }

            Point3D newLoc = m.Location;
            for (int i = 0; i < tiles; i++)
            {
                Point3D testLoc = new Point3D(newLoc.X + xOffset, newLoc.Y + yOffset, newLoc.Z);
                if (m.Map.CanFit(testLoc.X, testLoc.Y, testLoc.Z, 16, false, false))
                {
                    newLoc = testLoc;
                }
                else
                {
                    break;
                }
            }

            if (newLoc != m.Location)
            {
                m.MoveToWorld(newLoc, m.Map);
            }
        }

        private static void GenerateResources(DamageContext ctx)
        {
            if (ctx.Source is PlayerMobile pm)
            {
                VystiaResourceManager manager = VystiaResourceManager.GetManager(pm);
                if (manager == null)
                    return;

                // Class-specific resource generation
                // Soul Shards on kill (handled by OnKill)
                // Soul Shards on crit
                if (ctx.IsCrit && manager.HasResource(ResourceType.SoulShards))
                {
                    ISecondaryResource shards = manager.GetResource(ResourceType.SoulShards);
                    if (shards != null && Utility.RandomDouble() < 0.25) // 25% chance on crit
                    {
                        shards.Generate(1);
                        pm.SendMessage(0x3B2, "Soul Shard generated! ({0}/{1})", shards.Current, shards.Maximum);
                        ctx.ResourceGenerated[ResourceType.SoulShards] = 1;
                    }
                }

                // Fury on damage dealt
                if (manager.HasResource(ResourceType.Fury))
                {
                    ISecondaryResource fury = manager.GetResource(ResourceType.Fury);
                    if (fury != null)
                    {
                        int furyGain = Math.Max(1, ctx.FinalDamage / 10); // 1 fury per 10 damage
                        fury.Generate(furyGain);
                        ctx.ResourceGenerated[ResourceType.Fury] = furyGain;
                    }
                }

                // Chi on hit (Monk)
                if (ctx.IsAutoAttack && manager.HasResource(ResourceType.Chi))
                {
                    ISecondaryResource chi = manager.GetResource(ResourceType.Chi);
                    if (chi != null && Utility.RandomDouble() < 0.30) // 30% chance
                    {
                        chi.Generate(1);
                        pm.SendMessage(0x3B2, "Chi generated! ({0}/{1})", chi.Current, chi.Maximum);
                        ctx.ResourceGenerated[ResourceType.Chi] = 1;
                    }
                }

                // Combo Points on hit (Rogue - per-target tracked via TargetTracker)
                if (ctx.IsAutoAttack && manager.HasResource(ResourceType.ComboPoints))
                {
                    // Vystia: Subterfuge skill increases combo point generation
                    int baseComboPoints = 1;
                    double subterfugeBonus = VystiaSkillIntegration.GetSubterfugeComboGenerationBonus(pm);
                    
                    // Apply bonus: chance to generate extra combo point based on skill
                    // At GM (100 skill), 100% bonus = double generation rate
                    int comboPointsToAdd = baseComboPoints;
                    if (subterfugeBonus > 0.0 && Utility.RandomDouble() < subterfugeBonus)
                    {
                        comboPointsToAdd += 1; // Extra combo point
                    }
                    
                    // Use TargetTracker for per-target combo points
                    VystiaTargetTracker.Instance.AddStacks(pm, ctx.Target, StackType.Combo, comboPointsToAdd, TimeSpan.FromSeconds(20));
                    int comboStacks = VystiaTargetTracker.Instance.GetStacks(pm, ctx.Target, StackType.Combo);
                    pm.SendMessage(0x3B2, "Combo Point! ({0}/5)", comboStacks);
                    ctx.ResourceGenerated[ResourceType.ComboPoints] = comboPointsToAdd;
                }

                // Pursuit on marked target (Bounty Hunter)
                if (manager.HasResource(ResourceType.Pursuit))
                {
                    // Check if target is marked
                    TargetState state = VystiaTargetTracker.Instance.GetState(pm, ctx.Target);
                    if (state != null && state.CurrentMark == MarkType.BountyMark)
                    {
                        ISecondaryResource pursuit = manager.GetResource(ResourceType.Pursuit);
                        if (pursuit != null)
                        {
                            pursuit.Generate(1);
                            ctx.ResourceGenerated[ResourceType.Pursuit] = 1;
                        }
                    }
                }

                // Fortitude on block (Knight) - handled separately
            }
        }
    }

    #endregion

    #region Healing Calculator

    /// <summary>
    /// Handles all healing calculations for Vystia system
    /// </summary>
    public static class VystiaHealingCalculator
    {
        /// <summary>
        /// Calculate and apply healing using the Vystia system
        /// </summary>
        public static int Calculate(HealContext ctx)
        {
            if (ctx.Target == null || ctx.Target.Deleted || !ctx.Target.Alive)
                return 0;

            // Step 1: Roll base heal
            ctx.RawHeal = Utility.RandomMinMax(ctx.MinHeal, ctx.MaxHeal);

            // Step 2: Roll critical heal
            ctx.RollCrit();
            if (ctx.IsCrit)
            {
                ctx.RawHeal = (int)(ctx.RawHeal * ctx.CritMultiplier);

                // Notify
                if (ctx.Source != null)
                {
                    ctx.Source.SendMessage(0x3B2, "Critical heal!");
                }
                ctx.Target.SendMessage(0x3B2, "Critical heal!");

                // Visual effect
                ctx.Target.FixedParticles(0x376A, 9, 32, 5005, 2010, 0, EffectLayer.Waist);
            }

            // Step 3: Apply heal multiplier
            int scaledHeal = (int)(ctx.RawHeal * ctx.HealMultiplier);

            // Step 4: Apply flat bonus
            scaledHeal += ctx.FlatHealBonus;

            // Step 5: Apply healing reduction debuffs
            scaledHeal = VystiaBuffManager.Instance.ProcessHealing(ctx.Target, scaledHeal);

            // Step 6: Calculate actual heal amount (accounting for overheal)
            int missingHP = ctx.Target.HitsMax - ctx.Target.Hits;
            int actualHeal = Math.Min(scaledHeal, missingHP);
            ctx.Overheal = scaledHeal - actualHeal;

            ctx.FinalHeal = actualHeal;

            // Step 7: Apply the heal
            if (actualHeal > 0)
            {
                ctx.Target.Heal(actualHeal, ctx.Source ?? ctx.Target, false);
            }

            // Step 8: Visual effect
            if (!ctx.IsHoT) // HoTs have their own effects
            {
                ctx.Target.FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);
                ctx.Target.PlaySound(0x1F2);
            }

            // Step 9: Generate Faith resource (Cleric)
            GenerateHealingResources(ctx);

            return actualHeal;
        }

        private static void GenerateHealingResources(HealContext ctx)
        {
            if (ctx.Source is PlayerMobile pm)
            {
                VystiaResourceManager manager = VystiaResourceManager.GetManager(pm);
                if (manager == null)
                    return;

                // Faith on healing (Cleric)
                if (manager.HasResource(ResourceType.Faith))
                {
                    ISecondaryResource faith = manager.GetResource(ResourceType.Faith);
                    if (faith != null)
                    {
                        int faithGain = Math.Max(1, ctx.FinalHeal / 5); // 1 faith per 5 healing
                        faith.Generate(faithGain);
                        ctx.ResourceGenerated[ResourceType.Faith] = faithGain;
                    }
                }
            }
        }
    }

    #endregion

    #region GM Commands

    public static class VystiaDamageCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("TestDamage", AccessLevel.GameMaster, TestDamage_OnCommand);
            CommandSystem.Register("TestHeal", AccessLevel.GameMaster, TestHeal_OnCommand);
            CommandSystem.Register("TestCrit", AccessLevel.GameMaster, TestCrit_OnCommand);
        }

        [Usage("TestDamage <min> <max> [type]")]
        [Description("Tests the Vystia damage system on a target")]
        private static void TestDamage_OnCommand(CommandEventArgs e)
        {
            if (e.Arguments.Length < 2)
            {
                e.Mobile.SendMessage("Usage: TestDamage <min> <max> [type]");
                e.Mobile.SendMessage("Types: Physical, Fire, Cold, Poison, Energy, Shadow, Holy, Arcane, Nature");
                return;
            }

            if (!int.TryParse(e.Arguments[0], out int min) || !int.TryParse(e.Arguments[1], out int max))
            {
                e.Mobile.SendMessage("Invalid damage values");
                return;
            }

            VystiaDamageType dmgType = VystiaDamageType.Physical;
            if (e.Arguments.Length >= 3)
            {
                Enum.TryParse(e.Arguments[2], true, out dmgType);
            }

            e.Mobile.SendMessage("Target a creature to deal {0}-{1} {2} damage:", min, max, dmgType);
            e.Mobile.Target = new InternalTarget((target) =>
            {
                if (target is Mobile m)
                {
                    DamageContext ctx = new DamageContext
                    {
                        Source = e.Mobile,
                        Target = m,
                        IsAbility = true
                    };
                    ctx.SetDamageRange(min, max);
                    ctx.SetDamageType(dmgType);

                    int damage = VystiaDamageCalculator.Calculate(ctx);

                    e.Mobile.SendMessage("Dealt {0} {1} damage to {2} (raw: {3}, crit: {4})",
                        damage, dmgType, m.Name, ctx.RawDamage, ctx.IsCrit ? "YES" : "no");
                }
            });
        }

        [Usage("TestHeal <min> <max>")]
        [Description("Tests the Vystia healing system on a target")]
        private static void TestHeal_OnCommand(CommandEventArgs e)
        {
            if (e.Arguments.Length < 2)
            {
                e.Mobile.SendMessage("Usage: TestHeal <min> <max>");
                return;
            }

            if (!int.TryParse(e.Arguments[0], out int min) || !int.TryParse(e.Arguments[1], out int max))
            {
                e.Mobile.SendMessage("Invalid heal values");
                return;
            }

            e.Mobile.SendMessage("Target a creature to heal {0}-{1}:", min, max);
            e.Mobile.Target = new InternalTarget((target) =>
            {
                if (target is Mobile m)
                {
                    HealContext ctx = new HealContext
                    {
                        Source = e.Mobile,
                        Target = m,
                        CanCrit = true
                    };
                    ctx.SetHealRange(min, max);

                    int heal = VystiaHealingCalculator.Calculate(ctx);

                    e.Mobile.SendMessage("Healed {0} for {1} (raw: {2}, crit: {3}, overheal: {4})",
                        m.Name, heal, ctx.RawHeal, ctx.IsCrit ? "YES" : "no", ctx.Overheal);
                }
            });
        }

        [Usage("TestCrit <critChance>")]
        [Description("Tests critical hit with specified chance (0-100)")]
        private static void TestCrit_OnCommand(CommandEventArgs e)
        {
            if (e.Arguments.Length < 1)
            {
                e.Mobile.SendMessage("Usage: TestCrit <critChance>");
                return;
            }

            if (!int.TryParse(e.Arguments[0], out int critChance))
            {
                e.Mobile.SendMessage("Invalid crit chance");
                return;
            }

            e.Mobile.SendMessage("Target a creature to deal 50-100 damage with {0}% crit:", critChance);
            e.Mobile.Target = new InternalTarget((target) =>
            {
                if (target is Mobile m)
                {
                    DamageContext ctx = new DamageContext
                    {
                        Source = e.Mobile,
                        Target = m,
                        CanCrit = true,
                        CritChance = critChance / 100.0,
                        CritMultiplier = 2.0,
                        IsAbility = true
                    };
                    ctx.SetDamageRange(50, 100);
                    ctx.SetDamageType(VystiaDamageType.Physical);

                    int damage = VystiaDamageCalculator.Calculate(ctx);

                    e.Mobile.SendMessage("Dealt {0} damage (crit: {1}, multiplier: 2.0x)",
                        damage, ctx.IsCrit ? "YES!" : "no");
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
    }

    #endregion
}

