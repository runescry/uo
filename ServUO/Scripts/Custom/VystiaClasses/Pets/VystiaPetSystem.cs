/*
 * Vystia Pet System
 * Base classes and utilities for class-specific pets
 *
 * Pet Classes:
 * - Beastmaster: Tamed beasts (permanent, bondable)
 * - Summoner: Summoned elementals/creatures (timed, controllable)
 * - Necromancer: Undead minions (timed, controllable)
 * - Artificer: Mechanical constructs (timed, controllable)
 */

using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Network;

namespace Server.Custom.VystiaClasses.Pets
{
    #region Enums

    /// <summary>
    /// Types of Vystia pets by class
    /// </summary>
    public enum VystiaPetType
    {
        None,
        // Beastmaster - Tamed beasts
        TamedBeast,
        // Summoner - Elemental summons
        SummonedElemental,
        SummonedCreature,
        // Necromancer - Undead
        UndeadMinion,
        UndeadServant,
        // Artificer - Constructs
        MechanicalConstruct,
        ClockworkServant
    }

    /// <summary>
    /// Pet power tiers
    /// </summary>
    public enum VystiaPetTier
    {
        Lesser = 1,      // Circle 1-2 summons, basic tames
        Standard = 2,    // Circle 3-4 summons, medium tames
        Greater = 3,     // Circle 5-6 summons, advanced tames
        Superior = 4,    // Circle 7 summons, rare tames
        Legendary = 5    // Circle 8 summons, legendary tames
    }

    #endregion

    #region Pet Stats Calculator

    /// <summary>
    /// Calculates pet stats based on tier and owner skill
    /// </summary>
    public static class VystiaPetStats
    {
        /// <summary>
        /// Get base HP for pet tier
        /// </summary>
        public static int GetBaseHP(VystiaPetTier tier)
        {
            switch (tier)
            {
                case VystiaPetTier.Lesser: return 50;
                case VystiaPetTier.Standard: return 100;
                case VystiaPetTier.Greater: return 175;
                case VystiaPetTier.Superior: return 275;
                case VystiaPetTier.Legendary: return 400;
                default: return 50;
            }
        }

        /// <summary>
        /// Get base damage range for pet tier
        /// </summary>
        public static (int min, int max) GetBaseDamage(VystiaPetTier tier)
        {
            switch (tier)
            {
                case VystiaPetTier.Lesser: return (5, 10);
                case VystiaPetTier.Standard: return (10, 18);
                case VystiaPetTier.Greater: return (15, 25);
                case VystiaPetTier.Superior: return (20, 32);
                case VystiaPetTier.Legendary: return (28, 42);
                default: return (5, 10);
            }
        }

        /// <summary>
        /// Get control slots for pet tier
        /// </summary>
        public static int GetControlSlots(VystiaPetTier tier)
        {
            switch (tier)
            {
                case VystiaPetTier.Lesser: return 1;
                case VystiaPetTier.Standard: return 2;
                case VystiaPetTier.Greater: return 3;
                case VystiaPetTier.Superior: return 4;
                case VystiaPetTier.Legendary: return 5;
                default: return 1;
            }
        }

        /// <summary>
        /// Get summon duration for pet tier (in minutes)
        /// </summary>
        public static double GetBaseDuration(VystiaPetTier tier)
        {
            switch (tier)
            {
                case VystiaPetTier.Lesser: return 5.0;
                case VystiaPetTier.Standard: return 8.0;
                case VystiaPetTier.Greater: return 12.0;
                case VystiaPetTier.Superior: return 15.0;
                case VystiaPetTier.Legendary: return 20.0;
                default: return 5.0;
            }
        }

        /// <summary>
        /// Calculate scaled stats based on owner skill
        /// </summary>
        public static double GetSkillScalar(Mobile owner, SkillName skill)
        {
            if (owner == null)
                return 1.0;

            double skillValue = owner.Skills[skill].Value;
            // Scale from 0.8x at 0 skill to 1.5x at 100 skill (Vystia: 100 is GM)
            return 0.8 + (skillValue * 0.007);
        }

        /// <summary>
        /// Calculate final HP with skill scaling
        /// </summary>
        public static int GetScaledHP(VystiaPetTier tier, Mobile owner, SkillName skill)
        {
            int baseHP = GetBaseHP(tier);
            double scalar = GetSkillScalar(owner, skill);
            return (int)(baseHP * scalar);
        }

        /// <summary>
        /// Calculate final damage with skill scaling
        /// </summary>
        public static (int min, int max) GetScaledDamage(VystiaPetTier tier, Mobile owner, SkillName skill)
        {
            var (baseMin, baseMax) = GetBaseDamage(tier);
            double scalar = GetSkillScalar(owner, skill);
            return ((int)(baseMin * scalar), (int)(baseMax * scalar));
        }

        /// <summary>
        /// Calculate final duration with skill scaling (in minutes)
        /// </summary>
        public static TimeSpan GetScaledDuration(VystiaPetTier tier, Mobile owner, SkillName skill)
        {
            double baseDuration = GetBaseDuration(tier);
            double scalar = GetSkillScalar(owner, skill);
            return TimeSpan.FromMinutes(baseDuration * scalar);
        }
    }

    #endregion

    #region Pet Summoning Helper

    /// <summary>
    /// Helper class for summoning Vystia pets
    /// </summary>
    public static class VystiaPetSummoner
    {
        /// <summary>
        /// Summon a controllable pet for a player
        /// </summary>
        public static bool SummonPet(Mobile caster, BaseCreature pet, VystiaPetTier tier,
            SkillName scalingSkill, int soundEffect = 0x215)
        {
            if (caster == null || pet == null)
                return false;

            int slots = VystiaPetStats.GetControlSlots(tier);

            // Check follower slots
            if (caster.Followers + slots > caster.FollowersMax)
            {
                caster.SendMessage("You have too many followers to summon this creature.");
                pet.Delete();
                return false;
            }

            // Calculate duration
            TimeSpan duration = VystiaPetStats.GetScaledDuration(tier, caster, scalingSkill);

            // Scale pet stats
            ScalePetStats(pet, tier, caster, scalingSkill);

            // Set control slots before summoning
            pet.ControlSlots = slots;

            // Summon the pet (controlled = true so it responds to commands)
            BaseCreature.Summon(pet, true, caster, caster.Location, soundEffect, duration);

            // Visual effect
            Effects.SendLocationParticles(
                EffectItem.Create(pet.Location, pet.Map, EffectItem.DefaultDuration),
                0x3728, 10, 10, 2023);

            caster.SendMessage($"You have summoned a {pet.Name}.");

            return true;
        }

        /// <summary>
        /// Scale pet stats based on tier and owner skill
        /// </summary>
        private static void ScalePetStats(BaseCreature pet, VystiaPetTier tier,
            Mobile owner, SkillName skill)
        {
            int hp = VystiaPetStats.GetScaledHP(tier, owner, skill);
            var (minDam, maxDam) = VystiaPetStats.GetScaledDamage(tier, owner, skill);

            pet.SetHits(hp);
            pet.SetDamage(minDam, maxDam);

            // Scale stats based on tier
            int statBase = 20 + ((int)(int)tier * 15);
            double scalar = VystiaPetStats.GetSkillScalar(owner, skill);

            pet.SetStr((int)(statBase * scalar));
            pet.SetDex((int)(statBase * scalar * 0.8));
            pet.SetInt((int)(statBase * scalar * 0.6));
        }

        /// <summary>
        /// Get count of active pets of a specific type for a player
        /// </summary>
        public static int GetActivePetCount(Mobile owner, VystiaPetType petType)
        {
            if (owner == null)
                return 0;

            int count = 0;

            if (owner is PlayerMobile pm)
            {
                foreach (var follower in pm.AllFollowers)
                {
                    if (follower is VystiaSummonedPet vsp && vsp.PetType == petType)
                        count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Dismiss all pets of a specific type
        /// </summary>
        public static void DismissAllPets(Mobile owner, VystiaPetType petType)
        {
            if (owner == null)
                return;

            List<BaseCreature> toRemove = new List<BaseCreature>();

            if (owner is PlayerMobile pm)
            {
                foreach (var follower in pm.AllFollowers)
                {
                    if (follower is VystiaSummonedPet vsp && vsp.PetType == petType)
                        toRemove.Add(vsp);
                }
            }

            foreach (var pet in toRemove)
            {
                Effects.SendLocationParticles(
                    EffectItem.Create(pet.Location, pet.Map, EffectItem.DefaultDuration),
                    0x3728, 10, 10, 2023);
                pet.Delete();
            }

            if (toRemove.Count > 0)
                owner.SendMessage($"You have dismissed {toRemove.Count} pet(s).");
        }
    }

    #endregion

    #region Base Summoned Pet Class

    /// <summary>
    /// Base class for all Vystia summoned pets (Summoner, Necromancer, Artificer)
    /// </summary>
    public abstract class VystiaSummonedPet : BaseCreature
    {
        private VystiaPetType m_PetType;
        private VystiaPetTier m_PetTier;

        [CommandProperty(AccessLevel.GameMaster)]
        public VystiaPetType PetType
        {
            get { return m_PetType; }
            set { m_PetType = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public VystiaPetTier PetTier
        {
            get { return m_PetTier; }
            set { m_PetTier = value; }
        }

        public VystiaSummonedPet(AIType ai, FightMode mode, int iRangePerception, int iRangeFight,
            double dActiveSpeed, double dPassiveSpeed)
            : base(ai, mode, iRangePerception, iRangeFight, dActiveSpeed, dPassiveSpeed)
        {
            m_PetType = VystiaPetType.None;
            m_PetTier = VystiaPetTier.Lesser;
        }

        public VystiaSummonedPet(Serial serial) : base(serial)
        {
        }

        /// <summary>
        /// Summoned pets don't drop loot
        /// </summary>
        public override bool AutoDispel { get { return false; } }
        public override bool BardImmune { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        /// <summary>
        /// Summoned pets cannot be tamed
        /// </summary>
        public new bool Tamable { get { return false; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write((int)m_PetType);
            writer.Write((int)m_PetTier);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_PetType = (VystiaPetType)reader.ReadInt();
            m_PetTier = (VystiaPetTier)reader.ReadInt();
        }
    }

    #endregion

    #region Pet Commands

    /// <summary>
    /// GM Commands for pet system testing
    /// </summary>
    public static class VystiaPetCommands
    {
        public static void Initialize()
        {
            Server.Commands.CommandSystem.Register("SummonPet", AccessLevel.GameMaster,
                new Server.Commands.CommandEventHandler(SummonPet_OnCommand));
            Server.Commands.CommandSystem.Register("DismissPets", AccessLevel.GameMaster,
                new Server.Commands.CommandEventHandler(DismissPets_OnCommand));
            Server.Commands.CommandSystem.Register("PetInfo", AccessLevel.GameMaster,
                new Server.Commands.CommandEventHandler(PetInfo_OnCommand));
        }

        [Usage("SummonPet <type> [tier]")]
        [Description("Summon a test Vystia pet. Types: elemental, undead, construct")]
        private static void SummonPet_OnCommand(Server.Commands.CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Arguments.Length < 1)
            {
                from.SendMessage("Usage: [SummonPet <elemental|undead|construct> [tier 1-5]");
                return;
            }

            string type = e.Arguments[0].ToLower();
            int tierNum = e.Arguments.Length > 1 ? Utility.ToInt32(e.Arguments[1]) : 2;
            VystiaPetTier tier = (VystiaPetTier)Math.Max(1, Math.Min(5, tierNum));

            BaseCreature pet = null;
            SkillName skill = SkillName.Magery;

            switch (type)
            {
                case "elemental":
                case "fire":
                    pet = new SummonedFireElemental(tier);
                    skill = SkillName.Magery;
                    break;
                case "ice":
                case "water":
                    pet = new SummonedIceElemental(tier);
                    skill = SkillName.Magery;
                    break;
                case "undead":
                case "skeleton":
                    pet = new SummonedSkeleton(tier);
                    skill = SkillName.Necromancy;
                    break;
                case "zombie":
                    pet = new SummonedZombie(tier);
                    skill = SkillName.Necromancy;
                    break;
                case "construct":
                case "clockwork":
                    pet = new SummonedClockwork(tier);
                    skill = SkillName.Tinkering;
                    break;
                default:
                    from.SendMessage("Unknown pet type. Use: elemental, ice, undead, zombie, construct");
                    return;
            }

            if (pet != null)
            {
                VystiaPetSummoner.SummonPet(from, pet, tier, skill);
            }
        }

        [Usage("DismissPets [type]")]
        [Description("Dismiss all Vystia pets or a specific type")]
        private static void DismissPets_OnCommand(Server.Commands.CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Arguments.Length > 0)
            {
                string type = e.Arguments[0].ToLower();
                VystiaPetType petType = VystiaPetType.None;

                switch (type)
                {
                    case "elemental":
                        petType = VystiaPetType.SummonedElemental;
                        break;
                    case "undead":
                        petType = VystiaPetType.UndeadMinion;
                        break;
                    case "construct":
                        petType = VystiaPetType.MechanicalConstruct;
                        break;
                }

                if (petType != VystiaPetType.None)
                {
                    VystiaPetSummoner.DismissAllPets(from, petType);
                    return;
                }
            }

            // Dismiss all
            if (from is PlayerMobile pm)
            {
                List<BaseCreature> toRemove = new List<BaseCreature>();
                foreach (var m in pm.AllFollowers)
                {
                    if (m is BaseCreature bc)
                        toRemove.Add(bc);
                }
                foreach (var pet in toRemove)
                {
                    if (pet is VystiaSummonedPet)
                    {
                        Effects.SendLocationParticles(
                            EffectItem.Create(pet.Location, pet.Map, EffectItem.DefaultDuration),
                            0x3728, 10, 10, 2023);
                        pet.Delete();
                    }
                }
                from.SendMessage("All Vystia pets dismissed.");
            }
        }

        [Usage("PetInfo")]
        [Description("Show info about your current pets")]
        private static void PetInfo_OnCommand(Server.Commands.CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            from.SendMessage("=== Your Pets ===");
            from.SendMessage($"Followers: {from.Followers}/{from.FollowersMax}");

            if (from is PlayerMobile pm)
            {
                foreach (var follower in pm.AllFollowers)
                {
                    if (follower is VystiaSummonedPet vsp)
                    {
                        from.SendMessage($"  - {vsp.Name} ({vsp.PetType}, Tier {(int)vsp.PetTier}) HP:{vsp.Hits}/{vsp.HitsMax} Slots:{vsp.ControlSlots}");
                    }
                    else if (follower is BaseCreature bc)
                    {
                        from.SendMessage($"  - {bc.Name} HP:{bc.Hits}/{bc.HitsMax} Slots:{bc.ControlSlots}");
                    }
                }
            }
        }
    }

    #endregion
}
