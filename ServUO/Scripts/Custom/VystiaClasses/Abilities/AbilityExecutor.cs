/*
 * Vystia Class System v2.0
 * Ability Executor
 *
 * Executes abilities based on their data-driven definitions.
 */

using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Commands;
using Server.Custom.VystiaClasses.Systems;
using Server.Custom.VystiaClasses.Skills;
using Server.Targeting;

namespace Server.Custom.VystiaClasses.Abilities
{
    #region Execution Result

    /// <summary>
    /// Result of an ability execution attempt
    /// </summary>
    public class AbilityExecutionResult
    {
        public bool Success { get; set; }
        public string FailureReason { get; set; }
        public int TotalDamage { get; set; }
        public int TotalHealing { get; set; }
        public int TargetsHit { get; set; }
        public bool WasCrit { get; set; }
        public Dictionary<ResourceType, int> ResourcesGenerated { get; set; }
        public Dictionary<ResourceType, int> ResourcesSpent { get; set; }

        public AbilityExecutionResult()
        {
            ResourcesGenerated = new Dictionary<ResourceType, int>();
            ResourcesSpent = new Dictionary<ResourceType, int>();
        }

        public static AbilityExecutionResult Failed(string reason)
        {
            return new AbilityExecutionResult { Success = false, FailureReason = reason };
        }

        public static AbilityExecutionResult Succeeded()
        {
            return new AbilityExecutionResult { Success = true };
        }
    }

    #endregion

    #region Ability Executor

    /// <summary>
    /// Executes abilities from their definitions
    /// </summary>
    public static class AbilityExecutor
    {
        #region Main Execute Method

        /// <summary>
        /// Execute an ability on a target
        /// </summary>
        public static AbilityExecutionResult Execute(Mobile caster, AbilityDefinition ability, Mobile target = null, Point3D? groundTarget = null)
        {
            if (caster == null || caster.Deleted || !caster.Alive)
                return AbilityExecutionResult.Failed("Invalid caster");

            if (ability == null)
                return AbilityExecutionResult.Failed("Invalid ability");

            // Validate requirements
            string requirementError = ValidateRequirements(caster, ability, target);
            if (requirementError != null)
                return AbilityExecutionResult.Failed(requirementError);

            // Pay costs
            string costError = PayCosts(caster, ability);
            if (costError != null)
                return AbilityExecutionResult.Failed(costError);

            // Resolve targets
            List<Mobile> targets = ResolveTargets(caster, ability, target, groundTarget);
            if (targets.Count == 0 && ability.TargetType != AbilityTargetType.Self && ability.TargetType != AbilityTargetType.Passive)
                return AbilityExecutionResult.Failed("No valid targets");

            // Play cast animation/sound
            PlayCastEffects(caster, ability);

            // Apply effects
            AbilityExecutionResult result = AbilityExecutionResult.Succeeded();
            result.TargetsHit = targets.Count;

            foreach (Mobile t in targets)
            {
                ApplyEffects(caster, ability, t, result);
            }

            // If self-targeted, apply to caster
            if (ability.TargetType == AbilityTargetType.Self || targets.Count == 0)
            {
                ApplyEffects(caster, ability, caster, result);
            }

            // Generate resources
            GenerateResources(caster, ability, result);

            // Trigger cooldown
            TriggerCooldown(caster, ability);

            // Break stealth if needed
            if (ability.BreaksStealth && caster.Hidden)
            {
                caster.RevealingAction();
            }

            // Trigger class skill gain check on successful ability execution
            if (result.Success)
            {
                // Difficulty based on spell circle or base 50 for non-spells
                double skillDifficulty = ability.Circle > 0 ? ability.Circle * 12.5 : 50.0;

                // Bonus difficulty from damage dealt
                if (result.TotalDamage > 0)
                {
                    skillDifficulty += result.TotalDamage / 10.0;
                    skillDifficulty = Math.Min(100.0, skillDifficulty);
                }

                VystiaSkillCheck.TriggerGainCheck(caster, skillDifficulty);
            }

            return result;
        }

        #endregion

        #region Validation

        private static string ValidateRequirements(Mobile caster, AbilityDefinition ability, Mobile target)
        {
            // Check mana
            if (ability.ManaCost > 0 && caster.Mana < ability.ManaCost)
                return string.Format("Not enough mana ({0} required)", ability.ManaCost);

            // Check stamina
            if (ability.StaminaCost > 0 && caster.Stam < ability.StaminaCost)
                return string.Format("Not enough stamina ({0} required)", ability.StaminaCost);

            // Check health cost
            if (ability.HealthCost > 0 && caster.Hits <= ability.HealthCost)
                return "Not enough health";

            // Check secondary resources
            if (caster is PlayerMobile pm)
            {
                VystiaResourceManager manager = VystiaResourceManager.GetManager(pm);
                if (manager != null)
                {
                    foreach (var kvp in ability.SecondaryCosts)
                    {
                        ISecondaryResource resource = manager.GetResource(kvp.Key);
                        if (resource == null || !resource.CanSpend(kvp.Value))
                            return string.Format("Not enough {0} ({1} required)", kvp.Key, kvp.Value);
                    }

                    // Check minimum resource stacks
                    if (ability.MinResourceStacks > 0 && ability.RequiredStance.HasValue)
                    {
                        ISecondaryResource resource = manager.GetResource(ability.RequiredStance.Value);
                        if (resource == null || resource.Current < ability.MinResourceStacks)
                            return string.Format("Need {0} {1}", ability.MinResourceStacks, ability.RequiredStance.Value);
                    }
                }
            }

            // Check stealth requirement
            if (ability.RequiresStealth && !caster.Hidden)
                return "Must be stealthed";

            // Check weapon requirement
            if (ability.RequiredWeapon != WeaponRequirement.None)
            {
                if (!HasRequiredWeapon(caster, ability.RequiredWeapon))
                    return string.Format("Requires {0}", ability.RequiredWeapon);
            }

            // Check target requirements
            if (ability.TargetType == AbilityTargetType.SingleTarget && target == null)
                return "Requires a target";

            if (target != null && ability.RequiresBehindTarget)
            {
                if (!IsBehindTarget(caster, target))
                    return "Must be behind target";
            }

            // Check range
            if (target != null && ability.Range > 0)
            {
                int distance = (int)caster.GetDistanceToSqrt(target);
                if (distance > ability.Range)
                    return "Target is too far away";
            }

            // Check line of sight
            if (target != null && ability.RequiresLineOfSight)
            {
                if (!caster.CanSee(target))
                    return "No line of sight to target";
            }

            return null;
        }

        private static bool HasRequiredWeapon(Mobile caster, WeaponRequirement req)
        {
            if (req == WeaponRequirement.None || req == WeaponRequirement.Any)
                return true;

            BaseWeapon weapon = caster.Weapon as BaseWeapon;
            if (weapon == null)
                return req == WeaponRequirement.Fist;

            // Check weapon type
            switch (req)
            {
                case WeaponRequirement.Sword:
                    return weapon is BaseSword;
                case WeaponRequirement.Axe:
                    return weapon is BaseAxe;
                case WeaponRequirement.Mace:
                    return weapon is BaseBashing;
                case WeaponRequirement.Polearm:
                    return weapon is BasePoleArm;
                case WeaponRequirement.Staff:
                    return weapon is BaseStaff;
                case WeaponRequirement.Dagger:
                    return weapon is BaseKnife;
                case WeaponRequirement.Bow:
                    return weapon is Bow || weapon is CompositeBow;
                case WeaponRequirement.Crossbow:
                    return weapon is Crossbow || weapon is HeavyCrossbow;
                case WeaponRequirement.TwoHanded:
                    return weapon.Layer == Layer.TwoHanded;
                case WeaponRequirement.Shield:
                    return caster.FindItemOnLayer(Layer.TwoHanded) is BaseShield;
                default:
                    return true;
            }
        }

        private static bool IsBehindTarget(Mobile attacker, Mobile target)
        {
            if (target == null)
                return false;

            Direction facing = target.Direction & Direction.Mask;
            Direction toAttacker = target.GetDirectionTo(attacker) & Direction.Mask;

            // Behind is opposite direction (with some tolerance)
            int facingVal = (int)facing;
            int toVal = (int)toAttacker;
            int diff = Math.Abs(facingVal - toVal);

            // 4 is directly behind (opposite direction)
            return diff >= 3 && diff <= 5;
        }

        #endregion

        #region Cost Payment

        private static string PayCosts(Mobile caster, AbilityDefinition ability)
        {
            // Pay mana
            if (ability.ManaCost > 0)
            {
                caster.Mana -= ability.ManaCost;
            }

            // Pay stamina
            if (ability.StaminaCost > 0)
            {
                caster.Stam -= ability.StaminaCost;
            }

            // Pay health
            if (ability.HealthCost > 0)
            {
                caster.Hits -= ability.HealthCost;
            }

            // Pay secondary resources
            if (caster is PlayerMobile pm)
            {
                VystiaResourceManager manager = VystiaResourceManager.GetManager(pm);
                if (manager != null)
                {
                    foreach (var kvp in ability.SecondaryCosts)
                    {
                        ISecondaryResource resource = manager.GetResource(kvp.Key);
                        if (resource != null)
                        {
                            // Vystia: Class-religion synergy cost reduction (Monk + Cogsmith Creed: -10% Chi cost)
                            int actualCost = kvp.Value;
                            if (kvp.Key == ResourceType.Chi)
                            {
                                double costReduction = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetClassReligionSynergyCostReduction(pm, "Chi");
                                if (costReduction > 0.0)
                                {
                                    actualCost = (int)(actualCost * (1.0 - costReduction));
                                    if (actualCost < 1)
                                        actualCost = 1; // Minimum cost of 1
                                }
                            }

                            if (resource.Spend(actualCost))
                            {
                                // Trigger skill gain check when spending secondary resources
                                VystiaSkillCheck.OnResourceSpent(pm, kvp.Key, actualCost);
                            }
                        }
                    }
                }
            }

            return null;
        }

        #endregion

        #region Target Resolution

        private static List<Mobile> ResolveTargets(Mobile caster, AbilityDefinition ability, Mobile primaryTarget, Point3D? groundTarget)
        {
            List<Mobile> targets = new List<Mobile>();

            switch (ability.TargetType)
            {
                case AbilityTargetType.Self:
                    targets.Add(caster);
                    break;

                case AbilityTargetType.SingleTarget:
                case AbilityTargetType.SingleFriendly:
                    if (primaryTarget != null)
                        targets.Add(primaryTarget);
                    break;

                case AbilityTargetType.PointBlankAoE:
                    targets = GetMobilesInRange(caster, caster.Location, ability.AoERadius, ability.MaxTargets, true);
                    break;

                case AbilityTargetType.TargetAoE:
                    if (primaryTarget != null)
                    {
                        targets = GetMobilesInRange(caster, primaryTarget.Location, ability.AoERadius, ability.MaxTargets, true);
                    }
                    else if (groundTarget.HasValue)
                    {
                        targets = GetMobilesInRange(caster, groundTarget.Value, ability.AoERadius, ability.MaxTargets, true);
                    }
                    break;

                case AbilityTargetType.Cone:
                    targets = GetMobilesInCone(caster, ability.Range, ability.ConeAngle, ability.MaxTargets);
                    break;

                case AbilityTargetType.Line:
                    if (primaryTarget != null)
                    {
                        targets = GetMobilesInLine(caster, primaryTarget.Location, ability.Range, ability.MaxTargets);
                    }
                    break;

                case AbilityTargetType.ChainTarget:
                    if (primaryTarget != null)
                    {
                        targets = GetChainTargets(caster, primaryTarget, ability.Range / 2, ability.MaxTargets);
                    }
                    break;
            }

            return targets;
        }

        private static List<Mobile> GetMobilesInRange(Mobile caster, Point3D center, int radius, int maxTargets, bool hostileOnly)
        {
            List<Mobile> targets = new List<Mobile>();

            foreach (Mobile m in caster.Map.GetMobilesInRange(center, radius))
            {
                if (m == caster)
                    continue;
                if (m.Deleted || !m.Alive)
                    continue;
                if (hostileOnly && !CanBeAttacked(caster, m))
                    continue;

                targets.Add(m);

                if (targets.Count >= maxTargets)
                    break;
            }

            return targets;
        }

        private static List<Mobile> GetMobilesInCone(Mobile caster, int range, int angle, int maxTargets)
        {
            List<Mobile> targets = new List<Mobile>();
            Direction facing = caster.Direction & Direction.Mask;

            foreach (Mobile m in caster.Map.GetMobilesInRange(caster.Location, range))
            {
                if (m == caster)
                    continue;
                if (m.Deleted || !m.Alive)
                    continue;
                if (!CanBeAttacked(caster, m))
                    continue;

                // Check if in cone
                Direction toTarget = caster.GetDirectionTo(m) & Direction.Mask;
                int diff = Math.Abs((int)facing - (int)toTarget);
                if (diff > 4) diff = 8 - diff;

                int halfAngle = angle / 45; // Convert degrees to direction units
                if (diff <= halfAngle)
                {
                    targets.Add(m);
                    if (targets.Count >= maxTargets)
                        break;
                }
            }

            return targets;
        }

        private static List<Mobile> GetMobilesInLine(Mobile caster, Point3D destination, int range, int maxTargets)
        {
            List<Mobile> targets = new List<Mobile>();

            // Get direction to destination
            int dx = destination.X - caster.X;
            int dy = destination.Y - caster.Y;
            double length = Math.Sqrt(dx * dx + dy * dy);
            if (length == 0) length = 1;

            double ndx = dx / length;
            double ndy = dy / length;

            for (int i = 1; i <= range; i++)
            {
                int x = caster.X + (int)(ndx * i);
                int y = caster.Y + (int)(ndy * i);

                foreach (Mobile m in caster.Map.GetMobilesInRange(new Point3D(x, y, caster.Z), 1))
                {
                    if (m == caster)
                        continue;
                    if (m.Deleted || !m.Alive)
                        continue;
                    if (!CanBeAttacked(caster, m))
                        continue;
                    if (targets.Contains(m))
                        continue;

                    targets.Add(m);
                    if (targets.Count >= maxTargets)
                        return targets;
                }
            }

            return targets;
        }

        private static List<Mobile> GetChainTargets(Mobile caster, Mobile primary, int chainRange, int maxTargets)
        {
            List<Mobile> targets = new List<Mobile> { primary };
            Mobile current = primary;

            while (targets.Count < maxTargets)
            {
                Mobile next = null;
                int closest = int.MaxValue;

                foreach (Mobile m in caster.Map.GetMobilesInRange(current.Location, chainRange))
                {
                    if (targets.Contains(m))
                        continue;
                    if (m == caster)
                        continue;
                    if (m.Deleted || !m.Alive)
                        continue;
                    if (!CanBeAttacked(caster, m))
                        continue;

                    int dist = (int)current.GetDistanceToSqrt(m);
                    if (dist < closest)
                    {
                        closest = dist;
                        next = m;
                    }
                }

                if (next == null)
                    break;

                targets.Add(next);
                current = next;
            }

            return targets;
        }

        private static bool CanBeAttacked(Mobile attacker, Mobile target)
        {
            if (target == null || target.Deleted || !target.Alive)
                return false;

            // Players can attack monsters
            if (target is BaseCreature bc)
            {
                if (bc.Controlled && bc.ControlMaster == attacker)
                    return false;
                return true;
            }

            // PvP checks would go here
            return true;
        }

        #endregion

        #region Effect Application

        private static void ApplyEffects(Mobile caster, AbilityDefinition ability, Mobile target, AbilityExecutionResult result)
        {
            foreach (AbilityEffect effect in ability.Effects)
            {
                ApplyEffect(caster, ability, target, effect, result);
            }

            // Play impact effects
            PlayImpactEffects(target, ability);
        }

        private static void ApplyEffect(Mobile caster, AbilityDefinition ability, Mobile target, AbilityEffect effect, AbilityExecutionResult result)
        {
            // Check condition
            if (!string.IsNullOrEmpty(effect.Condition))
            {
                if (!CheckCondition(caster, target, effect.Condition))
                {
                    // Apply conditional effect if exists
                    if (effect.ConditionalEffect != null)
                    {
                        ApplyEffect(caster, ability, target, effect.ConditionalEffect, result);
                    }
                    return;
                }
            }

            switch (effect.Type)
            {
                case AbilityEffectType.DirectDamage:
                    int damage = CalculateDamage(caster, target, ability, effect, result);
                    result.TotalDamage += damage;
                    break;

                case AbilityEffectType.DamageOverTime:
                    VystiaBuffManager.Instance.ApplyBuff(target, caster, VystiaBuffType.Bleed, effect.Duration, effect.MinValue);
                    break;

                case AbilityEffectType.DirectHeal:
                    int heal = CalculateHeal(caster, target, ability, effect, result);
                    result.TotalHealing += heal;
                    break;

                case AbilityEffectType.HealOverTime:
                    VystiaBuffManager.Instance.ApplyBuff(target, caster, VystiaBuffType.Rejuvenation, effect.Duration, effect.MinValue);
                    break;

                case AbilityEffectType.Absorb:
                    VystiaBuffManager.Instance.ApplyBuff(target, caster, VystiaBuffType.DamageAbsorb, effect.Duration, effect.MinValue);
                    break;

                case AbilityEffectType.ApplyBuff:
                    VystiaBuffManager.Instance.ApplyBuff(target, caster, effect.BuffType, effect.Duration, effect.MinValue);
                    break;

                case AbilityEffectType.ApplyDebuff:
                    VystiaBuffManager.Instance.ApplyBuff(target, caster, effect.BuffType, effect.Duration, effect.MinValue);
                    break;

                case AbilityEffectType.RemoveBuff:
                case AbilityEffectType.RemoveDebuff:
                case AbilityEffectType.DispelMagic:
                    VystiaBuffManager.Instance.RemoveAllBuffs(target,
                        effect.Type == AbilityEffectType.RemoveBuff,
                        effect.Type == AbilityEffectType.RemoveDebuff);
                    break;

                case AbilityEffectType.ApplyCC:
                    CrowdControlManager.Instance.ApplyCC(target, caster, effect.CCType, effect.Duration);
                    break;

                case AbilityEffectType.RemoveCC:
                    CrowdControlManager.Instance.RemoveAllCC(target, true);
                    break;

                case AbilityEffectType.ApplyStack:
                    VystiaTargetTracker.Instance.AddStacks(caster, target, effect.StackType, effect.StackCount, effect.Duration);
                    break;

                case AbilityEffectType.ConsumeStack:
                    int consumed = VystiaTargetTracker.Instance.ConsumeStacks(caster, target, effect.StackType);
                    // Could multiply damage based on consumed stacks
                    break;

                case AbilityEffectType.RestoreMana:
                    target.Mana = Math.Min(target.ManaMax, target.Mana + Utility.RandomMinMax(effect.MinValue, effect.MaxValue));
                    break;

                case AbilityEffectType.RestoreStamina:
                    target.Stam = Math.Min(target.StamMax, target.Stam + Utility.RandomMinMax(effect.MinValue, effect.MaxValue));
                    break;

                case AbilityEffectType.DrainMana:
                    int manaDrain = Utility.RandomMinMax(effect.MinValue, effect.MaxValue);
                    target.Mana = Math.Max(0, target.Mana - manaDrain);
                    caster.Mana = Math.Min(caster.ManaMax, caster.Mana + manaDrain / 2);
                    break;

                case AbilityEffectType.DrainStamina:
                    int stamDrain = Utility.RandomMinMax(effect.MinValue, effect.MaxValue);
                    target.Stam = Math.Max(0, target.Stam - stamDrain);
                    caster.Stam = Math.Min(caster.StamMax, caster.Stam + stamDrain / 2);
                    break;

                case AbilityEffectType.Knockback:
                    KnockbackTarget(caster, target, effect.Distance);
                    break;

                case AbilityEffectType.Pull:
                    PullTarget(caster, target, effect.Distance);
                    break;

                case AbilityEffectType.Teleport:
                    // Teleport caster to target location or specified location
                    if (target != null && target != caster)
                    {
                        Point3D newLoc = target.Location;
                        caster.MoveToWorld(newLoc, caster.Map);
                        Effects.SendLocationParticles(EffectItem.Create(caster.Location, caster.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, 5023);
                    }
                    break;

                case AbilityEffectType.Interrupt:
                    // Interrupt spellcasting by doing 1 damage (triggers spell interrupt)
                    if (target.Spell != null)
                    {
                        AOS.Damage(target, caster, 1, 100, 0, 0, 0, 0);
                        target.SendMessage(0x22, "Your spell has been interrupted!");
                    }
                    break;

                case AbilityEffectType.BreakStealth:
                    target.RevealingAction();
                    break;

                case AbilityEffectType.EnterStealth:
                    caster.Hidden = true;
                    break;

                case AbilityEffectType.ApplyTransform:
                    VystiaBuffManager.Instance.ApplyBuff(target, caster, VystiaBuffType.WolfForm, effect.Duration);
                    break;

                case AbilityEffectType.TriggerAbility:
                    if (effect.TriggerChance >= 1.0 || Utility.RandomDouble() < effect.TriggerChance)
                    {
                        AbilityDefinition triggered = AbilityRegistry.GetAbility(effect.TriggeredAbilityId);
                        if (triggered != null)
                        {
                            Execute(caster, triggered, target);
                        }
                    }
                    break;
            }

            // Play effect-specific visuals/sounds
            if (effect.EffectId > 0)
            {
                target.FixedParticles(effect.EffectId, 10, 30, 5052, effect.EffectHue, 0, EffectLayer.Waist);
            }
            if (effect.SoundId > 0)
            {
                target.PlaySound(effect.SoundId);
            }
        }

        private static bool CheckCondition(Mobile caster, Mobile target, string condition)
        {
            switch (condition.ToLower())
            {
                case "targetfrozen":
                    return VystiaTargetTracker.Instance.IsFrozen(caster, target) ||
                           CrowdControlManager.Instance.HasCC(target, CCType.Freeze);

                case "targetbelow20percent":
                    return ((double)target.Hits / target.HitsMax) < 0.20;

                case "targetbelow35percent":
                    return ((double)target.Hits / target.HitsMax) < 0.35;

                case "targetfullhealth":
                    return target.Hits >= target.HitsMax;

                case "targetstunned":
                    return CrowdControlManager.Instance.HasCC(target, CCType.Stun);

                case "targetbleeding":
                    return VystiaBuffManager.Instance.HasBuff(target, VystiaBuffType.Bleed);

                case "casterfullresource":
                    if (caster is PlayerMobile pm)
                    {
                        VystiaResourceManager manager = VystiaResourceManager.GetManager(pm);
                        // Would check current class's primary resource
                    }
                    return false;

                case "casterhidden":
                    return caster.Hidden;

                default:
                    return true;
            }
        }

        private static int CalculateDamage(Mobile caster, Mobile target, AbilityDefinition ability, AbilityEffect effect, AbilityExecutionResult result)
        {
            DamageContext ctx = new DamageContext
            {
                Source = caster,
                Target = target,
                IsAbility = true,
                CanCrit = ability.CanCrit,
                CritChance = 0.05 + ability.CritChanceBonus,
                CritMultiplier = 1.5 + ability.CritMultiplierBonus,
                IgnoresArmor = ability.IgnoresArmor,
                IgnoresResistance = ability.IgnoresResistance,
                ArmorPenetration = ability.ArmorPenetration
            };

            ctx.SetDamageRange(effect.MinValue, effect.MaxValue);
            ctx.SetDamageType(effect.DamageType);

            // Apply conditional multiplier
            if (effect.ConditionalMultiplier != 1.0 && !string.IsNullOrEmpty(effect.Condition))
            {
                if (CheckCondition(caster, target, effect.Condition))
                {
                    ctx.DamageMultiplier = effect.ConditionalMultiplier;
                }
            }

			// Vystia: Paladin Templar's Verdict scales with Holy Power (Valor stacks)
			if (ability.School == AbilitySchool.Paladin && ability.Id == 2104 && caster is PlayerMobile pm)
			{
				var manager = VystiaResourceManager.GetManager(pm);
				if (manager != null)
				{
					var virtueStacks = manager.GetResource(ResourceType.Virtues) as VirtueStacksResource;
					if (virtueStacks != null)
					{
						int holyPower = virtueStacks.GetStacks(VirtueStacksResource.VirtueType.Valor);
						// Scale damage: base + 20% per Holy Power (max 5)
						double damageMultiplier = 1.0 + (holyPower * 0.20); // +20% per stack, up to +100% at 5
						ctx.DamageMultiplier *= damageMultiplier;
						
						// Consume all Holy Power
						virtueStacks.ConsumeVirtue(VirtueStacksResource.VirtueType.Valor);
					}
				}
			}

			// Vystia: Rogue finisher abilities scale with combo points
			if (ability.IsFinisher && caster is PlayerMobile rogue && target != null)
			{
				double comboMultiplier = VystiaSkillIntegration.GetComboPointFinisherMultiplier(rogue, target);
				if (comboMultiplier > 0.0)
				{
					ctx.DamageMultiplier *= (1.0 + comboMultiplier);
					
					// Consume all combo points after finisher
					var manager = VystiaResourceManager.GetManager(rogue);
					if (manager != null)
					{
						var comboPoints = manager.GetResource(ResourceType.ComboPoints) as ComboPointsResource;
						if (comboPoints != null)
						{
							comboPoints.SetStacks(target, 0); // Reset combo points
						}
					}
				}
			}

			// Vystia: Bounty Hunter + Celestis Arcanum: +10% mark damage
			// Check if target is marked (has Pursuit stacks)
			if (caster is PlayerMobile bh && target != null)
			{
				var manager = VystiaResourceManager.GetManager(bh);
				if (manager != null)
				{
					var pursuit = manager.GetResource(ResourceType.Pursuit) as PursuitResource;
					if (pursuit != null && pursuit.GetStacks(target) > 0)
					{
						double markDamageBonus = VystiaSkillIntegration.GetClassReligionSynergyMarkDamageBonus(bh);
						if (markDamageBonus > 0)
						{
							ctx.DamageMultiplier *= (1.0 + markDamageBonus); // +10% mark damage
						}
					}
				}
			}

            int damage = VystiaDamageCalculator.Calculate(ctx);

            if (ctx.IsCrit)
                result.WasCrit = true;

            return damage;
        }

        private static int CalculateHeal(Mobile caster, Mobile target, AbilityDefinition ability, AbilityEffect effect, AbilityExecutionResult result)
        {
            HealContext ctx = new HealContext
            {
                Source = caster,
                Target = target,
                CanCrit = ability.CanCrit
            };

            ctx.SetHealRange(effect.MinValue, effect.MaxValue);

			// Vystia: Paladin Word of Glory scales with Holy Power (Valor stacks)
			if (ability.School == AbilitySchool.Paladin && ability.Id == 2101 && caster is PlayerMobile pm)
			{
				var manager = VystiaResourceManager.GetManager(pm);
				if (manager != null)
				{
					var virtueStacks = manager.GetResource(ResourceType.Virtues) as VirtueStacksResource;
					if (virtueStacks != null)
					{
						int holyPower = virtueStacks.GetStacks(VirtueStacksResource.VirtueType.Valor);
						// Scale heal: base + 20% per Holy Power (max 5)
						double healMultiplier = 1.0 + (holyPower * 0.20); // +20% per stack, up to +100% at 5
						ctx.HealMultiplier *= healMultiplier;
						
						// Consume all Holy Power
						virtueStacks.ConsumeVirtue(VirtueStacksResource.VirtueType.Valor);
					}
				}
			}

            int heal = VystiaHealingCalculator.Calculate(ctx);

            if (ctx.IsCrit)
                result.WasCrit = true;

            return heal;
        }

        private static void KnockbackTarget(Mobile caster, Mobile target, int distance)
        {
            if (distance <= 0)
                distance = 3;

            Direction pushDir = caster.GetDirectionTo(target);
            PushMobile(target, pushDir, distance);
        }

        private static void PullTarget(Mobile caster, Mobile target, int distance)
        {
            if (distance <= 0)
                distance = 3;

            Direction pullDir = target.GetDirectionTo(caster);
            PushMobile(target, pullDir, distance);
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
                m.PlaySound(0x11C);
            }
        }

        #endregion

        #region Resource Generation

        private static void GenerateResources(Mobile caster, AbilityDefinition ability, AbilityExecutionResult result)
        {
            if (caster is PlayerMobile pm)
            {
                VystiaResourceManager manager = VystiaResourceManager.GetManager(pm);
                if (manager != null)
                {
                    foreach (var kvp in ability.ResourceGeneration)
                    {
                        ISecondaryResource resource = manager.GetResource(kvp.Key);
                        if (resource != null)
                        {
                            resource.Generate(kvp.Value);
                            result.ResourcesGenerated[kvp.Key] = kvp.Value;
                        }
                    }

					// Vystia: Paladin Holy Power builder abilities
					// Crusader Strike (2096) and Judgment (2097) generate 1 Holy Power
					if (ability.School == AbilitySchool.Paladin && 
						(ability.Id == 2096 || ability.Id == 2097)) // Crusader Strike, Judgment
					{
						var virtueStacks = manager.GetResource(ResourceType.Virtues) as VirtueStacksResource;
						if (virtueStacks != null)
						{
							// Holy Power is stored as Valor stacks in VirtueStacksResource (0-5)
							virtueStacks.AddStack(VirtueStacksResource.VirtueType.Valor);
							result.ResourcesGenerated[ResourceType.Virtues] = 1;
						}
					}
                }
            }
        }

        #endregion

        #region Cooldown

        private static Dictionary<Mobile, Dictionary<int, DateTime>> m_Cooldowns = new Dictionary<Mobile, Dictionary<int, DateTime>>();
        private static Dictionary<Mobile, Dictionary<string, DateTime>> m_SharedCooldowns = new Dictionary<Mobile, Dictionary<string, DateTime>>();

        private static void TriggerCooldown(Mobile caster, AbilityDefinition ability)
        {
            if (ability.Cooldown <= TimeSpan.Zero)
                return;

            // Individual ability cooldown
            if (!m_Cooldowns.TryGetValue(caster, out Dictionary<int, DateTime> cooldowns))
            {
                cooldowns = new Dictionary<int, DateTime>();
                m_Cooldowns[caster] = cooldowns;
            }
            cooldowns[ability.Id] = DateTime.UtcNow + ability.Cooldown;

            // Shared cooldown group
            if (!string.IsNullOrEmpty(ability.SharedCooldownGroup))
            {
                if (!m_SharedCooldowns.TryGetValue(caster, out Dictionary<string, DateTime> sharedCooldowns))
                {
                    sharedCooldowns = new Dictionary<string, DateTime>();
                    m_SharedCooldowns[caster] = sharedCooldowns;
                }
                sharedCooldowns[ability.SharedCooldownGroup] = DateTime.UtcNow + ability.Cooldown;
            }
        }

        public static bool IsOnCooldown(Mobile caster, AbilityDefinition ability)
        {
            // Check individual cooldown
            if (m_Cooldowns.TryGetValue(caster, out Dictionary<int, DateTime> cooldowns))
            {
                if (cooldowns.TryGetValue(ability.Id, out DateTime expires))
                {
                    if (DateTime.UtcNow < expires)
                        return true;
                }
            }

            // Check shared cooldown
            if (!string.IsNullOrEmpty(ability.SharedCooldownGroup))
            {
                if (m_SharedCooldowns.TryGetValue(caster, out Dictionary<string, DateTime> sharedCooldowns))
                {
                    if (sharedCooldowns.TryGetValue(ability.SharedCooldownGroup, out DateTime expires))
                    {
                        if (DateTime.UtcNow < expires)
                            return true;
                    }
                }
            }

            return false;
        }

        public static TimeSpan GetRemainingCooldown(Mobile caster, AbilityDefinition ability)
        {
            DateTime latest = DateTime.MinValue;

            if (m_Cooldowns.TryGetValue(caster, out Dictionary<int, DateTime> cooldowns))
            {
                if (cooldowns.TryGetValue(ability.Id, out DateTime expires) && expires > latest)
                    latest = expires;
            }

            if (!string.IsNullOrEmpty(ability.SharedCooldownGroup))
            {
                if (m_SharedCooldowns.TryGetValue(caster, out Dictionary<string, DateTime> sharedCooldowns))
                {
                    if (sharedCooldowns.TryGetValue(ability.SharedCooldownGroup, out DateTime expires) && expires > latest)
                        latest = expires;
                }
            }

            if (latest > DateTime.UtcNow)
                return latest - DateTime.UtcNow;

            return TimeSpan.Zero;
        }

        #endregion

        #region Visual Effects

        private static void PlayCastEffects(Mobile caster, AbilityDefinition ability)
        {
            if (ability.CastAnimation > 0)
            {
                caster.Animate(ability.CastAnimation, 5, 1, true, false, 0);
            }

            if (ability.CastSound > 0)
            {
                caster.PlaySound(ability.CastSound);
            }
        }

        private static void PlayImpactEffects(Mobile target, AbilityDefinition ability)
        {
            if (ability.ImpactEffectId > 0)
            {
                target.FixedParticles(ability.ImpactEffectId, 10, 30, 5052, ability.ImpactEffectHue, 0, EffectLayer.Waist);
            }

            if (ability.ImpactSound > 0)
            {
                target.PlaySound(ability.ImpactSound);
            }
        }

        #endregion
    }

    #endregion

    #region Ability Registry

    /// <summary>
    /// Registry of all ability definitions
    /// </summary>
    public static class AbilityRegistry
    {
        private static Dictionary<int, AbilityDefinition> m_Abilities = new Dictionary<int, AbilityDefinition>();
        private static Dictionary<AbilitySchool, List<AbilityDefinition>> m_AbilitiesBySchool = new Dictionary<AbilitySchool, List<AbilityDefinition>>();

        public static void Initialize()
        {
            // Register GM commands
            CommandSystem.Register("TestAbility", AccessLevel.GameMaster, TestAbility_OnCommand);
            CommandSystem.Register("ListAbilities", AccessLevel.GameMaster, ListAbilities_OnCommand);

            // Register some test abilities
            RegisterTestAbilities();
        }

        public static void RegisterAbility(AbilityDefinition ability)
        {
            string error;
            if (!ability.IsValid(out error))
            {
                Console.WriteLine("Invalid ability {0}: {1}", ability.Name ?? "Unknown", error);
                return;
            }

            m_Abilities[ability.Id] = ability;

            // Index by school
            if (!m_AbilitiesBySchool.TryGetValue(ability.School, out List<AbilityDefinition> schoolList))
            {
                schoolList = new List<AbilityDefinition>();
                m_AbilitiesBySchool[ability.School] = schoolList;
            }
            schoolList.Add(ability);
        }

        public static AbilityDefinition GetAbility(int id)
        {
            if (m_Abilities.TryGetValue(id, out AbilityDefinition ability))
                return ability;
            return null;
        }

        public static List<AbilityDefinition> GetAbilitiesBySchool(AbilitySchool school)
        {
            if (m_AbilitiesBySchool.TryGetValue(school, out List<AbilityDefinition> list))
                return new List<AbilityDefinition>(list);
            return new List<AbilityDefinition>();
        }

        public static IEnumerable<AbilityDefinition> AllAbilities => m_Abilities.Values;

        #region Test Abilities

        private static void RegisterTestAbilities()
        {
            // Ice Bolt - Simple damage spell
            RegisterAbility(AbilityDefinition.CreateDamageSpell(
                10001, "Ice Bolt", AbilitySchool.Ice, 1, 12, 18, VystiaDamageType.Cold, 4)
                .WithStack(StackType.Chill, 1, 10)
                .WithImpactEffect(0x36D4, 0x1E5, 1153));

            // Frostfire Bolt - Damage + DoT
            RegisterAbility(new AbilityDefinition()
                .WithId(10002)
                .WithName("Frostfire Bolt")
                .InSchool(AbilitySchool.Ice)
                .InCircle(3)
                .WithManaCost(9)
                .Targeting(AbilityTargetType.SingleTarget, 12)
                .WithDamage(15, 25, VystiaDamageType.Cold)
                .WithDoT(5, 12, VystiaDamageType.Fire)
                .WithImpactEffect(0x36D4, 0x1E5, 1358));

            // Blizzard - AoE
            RegisterAbility(AbilityDefinition.CreateAoESpell(
                10003, "Blizzard", AbilitySchool.Ice, 6, 25, 40, VystiaDamageType.Cold, 5, 20)
                .WithCooldown(30)
                .WithCC(CCType.Slow, 5));

            // Frost Armor - Buff
            RegisterAbility(AbilityDefinition.CreateBuffSpell(
                10004, "Frost Armor", AbilitySchool.Ice, 2, VystiaBuffType.ColdResist, 20, 120, 6));

            // Sinister Strike - Rogue combo builder
            RegisterAbility(AbilityDefinition.CreateMartialStrike(
                20001, "Sinister Strike", AbilitySchool.Rogue, 20, 35, 15)
                .WithStack(StackType.Combo, 1, 20));

            // Eviscerate - Rogue finisher
            RegisterAbility(AbilityDefinition.CreateFinisher(
                20002, "Eviscerate", AbilitySchool.Rogue, 30, 15, 25)
                .WithCooldown(5));

            // Heroic Strike - Simple warrior ability
            RegisterAbility(AbilityDefinition.CreateMartialStrike(
                20003, "Heroic Strike", AbilitySchool.Fighter, 25, 45, 20)
                .WithCooldown(3));

            // Execute - Warrior execute
            var execute = new AbilityDefinition()
                .WithId(20004)
                .WithName("Execute")
                .InSchool(AbilitySchool.Barbarian)
                .WithStaminaCost(30)
                .Targeting(AbilityTargetType.SingleTarget, 2)
                .AsInstant()
                .WithCooldown(10);

            execute.Effects.Add(new AbilityEffect
            {
                Type = AbilityEffectType.DirectDamage,
                MinValue = 50,
                MaxValue = 75,
                DamageType = VystiaDamageType.Physical,
                Condition = "TargetBelow35Percent",
                ConditionalMultiplier = 2.5
            });
            RegisterAbility(execute);
        }

        #endregion

        #region GM Commands

        [Usage("TestAbility <id>")]
        [Description("Tests an ability on a target")]
        private static void TestAbility_OnCommand(CommandEventArgs e)
        {
            if (e.Arguments.Length < 1)
            {
                e.Mobile.SendMessage("Usage: TestAbility <id>");
                return;
            }

            if (!int.TryParse(e.Arguments[0], out int id))
            {
                e.Mobile.SendMessage("Invalid ability ID");
                return;
            }

            AbilityDefinition ability = GetAbility(id);
            if (ability == null)
            {
                e.Mobile.SendMessage("Ability {0} not found", id);
                return;
            }

            e.Mobile.SendMessage("Target a creature to use {0}:", ability.Name);
            e.Mobile.Target = new InternalTarget((target) =>
            {
                if (target is Mobile m)
                {
                    AbilityExecutionResult result = AbilityExecutor.Execute(e.Mobile, ability, m);

                    if (result.Success)
                    {
                        e.Mobile.SendMessage("=== {0} Result ===", ability.Name);
                        e.Mobile.SendMessage("  Damage: {0}", result.TotalDamage);
                        e.Mobile.SendMessage("  Healing: {0}", result.TotalHealing);
                        e.Mobile.SendMessage("  Targets: {0}", result.TargetsHit);
                        e.Mobile.SendMessage("  Crit: {0}", result.WasCrit ? "YES" : "no");
                    }
                    else
                    {
                        e.Mobile.SendMessage("Failed: {0}", result.FailureReason);
                    }
                }
            });
        }

        [Usage("ListAbilities [school]")]
        [Description("Lists registered abilities")]
        private static void ListAbilities_OnCommand(CommandEventArgs e)
        {
            if (e.Arguments.Length >= 1)
            {
                if (Enum.TryParse(e.Arguments[0], true, out AbilitySchool school))
                {
                    List<AbilityDefinition> list = GetAbilitiesBySchool(school);
                    e.Mobile.SendMessage("=== {0} Abilities ({1}) ===", school, list.Count);
                    foreach (var ability in list)
                    {
                        e.Mobile.SendMessage("  [{0}] {1} (Circle {2})", ability.Id, ability.Name, ability.Circle);
                    }
                    return;
                }
            }

            e.Mobile.SendMessage("=== All Registered Abilities ({0}) ===", m_Abilities.Count);
            foreach (var ability in m_Abilities.Values)
            {
                e.Mobile.SendMessage("  [{0}] {1} ({2})", ability.Id, ability.Name, ability.School);
            }
        }

        private class InternalTarget : Target
        {
            private Action<object> m_Callback;

            public InternalTarget(Action<object> callback) : base(12, false, TargetFlags.None)
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

