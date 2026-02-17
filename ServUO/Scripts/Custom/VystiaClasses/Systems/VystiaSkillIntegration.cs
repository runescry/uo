/*
 * Vystia Class System v2.0
 * Skill Integration Helper
 *
 * Centralized helper class for calculating skill-based bonuses.
 * Integrates Vystia class skills into ServUO core systems.
 */

using System;
using Server;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Systems;
using Server.Custom.VystiaClasses.Religion;

namespace Server.Custom.VystiaClasses.Systems
{
    /// <summary>
    /// Centralized skill bonus calculations for Vystia class skills
    /// </summary>
    public static class VystiaSkillIntegration
    {
        /// <summary>
        /// Get CombatMastery damage bonus
        /// Formula: +0.2% per 10 skill points (20% at GM)
        /// </summary>
        public static double GetCombatMasteryDamageBonus(Mobile attacker)
        {
            if (attacker == null || attacker.Skills == null)
                return 0.0;

            double combatMastery = attacker.Skills[SkillName.CombatMastery].Value;
            if (combatMastery <= 0)
                return 0.0;

            // +0.2% per 10 skill points = 0.02% per point = 20% at GM
            return combatMastery * 0.002;
        }

        /// <summary>
        /// Get Marksmanship damage bonus for ranged weapons
        /// Formula: +0.3% per 10 skill points (30% at GM)
        /// </summary>
        public static double GetMarksmanshipDamageBonus(Mobile attacker, bool isRanged)
        {
            if (!isRanged || attacker == null || attacker.Skills == null)
                return 0.0;

            double marksmanship = attacker.Skills[SkillName.Marksmanship].Value;
            if (marksmanship <= 0)
                return 0.0;

            // +0.3% per 10 skill points = 0.03% per point = 30% at GM
            return marksmanship * 0.003;
        }

        /// <summary>
        /// Get MartialArts unarmed damage bonus
        /// Formula: +1 damage per 20 skill points (5 damage at GM)
        /// </summary>
        public static int GetMartialArtsUnarmedBonus(Mobile attacker)
        {
            if (attacker == null || attacker.Skills == null)
                return 0;

            double martialArts = attacker.Skills[SkillName.MartialArts].Value;
            if (martialArts <= 0)
                return 0;

            // +1 damage per 20 skill points = 5 damage at GM
            return (int)(martialArts / 20.0);
        }

        /// <summary>
        /// Get Berserking Fury-based damage bonus
        /// Formula: +15% at 50+ Fury, +30% at 80+ Fury
        /// </summary>
        public static double GetBerserkingDamageBonus(PlayerMobile attacker)
        {
            if (attacker == null)
                return 0.0;

            var fury = VystiaResourceManager.GetResource<FuryResource>(attacker);
            if (fury == null)
                return 0.0;

            double currentFury = fury.Current;
            
            if (currentFury >= 80.0)
                return 0.30; // +30% at 80+ Fury
            else if (currentFury >= 50.0)
                return 0.15; // +15% at 50+ Fury
            
            return 0.0;
        }

        /// <summary>
        /// Get CombatMastery block chance bonus for Parry
        /// Formula: +0.1% per skill point (10% at GM)
        /// </summary>
        public static double GetCombatMasteryBlockBonus(Mobile defender)
        {
            if (defender == null || defender.Skills == null)
                return 0.0;

            double combatMastery = defender.Skills[SkillName.CombatMastery].Value;
            if (combatMastery <= 0)
                return 0.0;

            // +0.1% per point = 10% at GM
            return combatMastery * 0.001;
        }

        /// <summary>
        /// Get Subterfuge combo point generation bonus
        /// Formula: +1% generation rate per skill point (100% at GM)
        /// </summary>
        public static double GetSubterfugeComboGenerationBonus(Mobile attacker)
        {
            if (attacker == null || attacker.Skills == null)
                return 0.0;

            double subterfuge = attacker.Skills[SkillName.Subterfuge].Value;
            if (subterfuge <= 0)
                return 0.0;

            // +1% per point = 100% at GM (doubles generation rate)
            return subterfuge * 0.01;
        }

        /// <summary>
        /// Get finisher damage multiplier based on combo points
        /// Formula: +20% damage per combo point (100% at 5 combo points)
        /// </summary>
        public static double GetComboPointFinisherMultiplier(Mobile attacker, Mobile target)
        {
            if (attacker == null || target == null || !(attacker is PlayerMobile pm))
                return 0.0;

            var manager = VystiaResourceManager.GetManager(pm);
            if (manager == null)
                return 0.0;

            var comboPoints = manager.GetResource(ResourceType.ComboPoints) as ComboPointsResource;
            if (comboPoints == null)
                return 0.0;

            int points = comboPoints.GetStacks(target);
            if (points <= 0)
                return 0.0;

            // +20% damage per combo point (100% at 5 points)
            return points * 0.20;
        }

        /// <summary>
        /// Get ArcaneStudies spell power bonus
        /// Formula: +0.25% per 10 skill points (25% at GM)
        /// </summary>
        public static double GetArcaneStudiesSpellPowerBonus(Mobile caster)
        {
            if (caster == null || caster.Skills == null)
                return 0.0;

            double arcaneStudies = caster.Skills[SkillName.ArcaneStudies].Value;
            if (arcaneStudies <= 0)
                return 0.0;

            // +0.25% per 10 skill points = 0.025% per point = 25% at GM
            return arcaneStudies * 0.0025;
        }

        /// <summary>
        /// Get Cryomancy cold damage bonus
        /// Formula: +0.25% per 10 skill points (25% at GM)
        /// </summary>
        public static double GetCryomancyDamageBonus(Mobile caster)
        {
            if (caster == null || caster.Skills == null)
                return 0.0;

            double cryomancy = caster.Skills[SkillName.Cryomancy].Value;
            if (cryomancy <= 0)
                return 0.0;

            // +0.25% per 10 skill points = 0.025% per point = 25% at GM
            return cryomancy * 0.0025;
        }

        /// <summary>
        /// Get Demonology dark/shadow damage bonus
        /// Formula: +0.25% per 10 skill points (25% at GM)
        /// </summary>
        public static double GetDemonologyDamageBonus(Mobile caster)
        {
            if (caster == null || caster.Skills == null)
                return 0.0;

            double demonology = caster.Skills[SkillName.Demonology].Value;
            if (demonology <= 0)
                return 0.0;

            // +0.25% per 10 skill points = 0.025% per point = 25% at GM
            return demonology * 0.0025;
        }

        /// <summary>
        /// Get NecromancyArts necromancy spell damage bonus
        /// Formula: +0.25% per 10 skill points (25% at GM)
        /// </summary>
        public static double GetNecromancyArtsDamageBonus(Mobile caster)
        {
            if (caster == null || caster.Skills == null)
                return 0.0;

            double necromancyArts = caster.Skills[SkillName.NecromancyArts].Value;
            if (necromancyArts <= 0)
                return 0.0;

            // +0.25% per 10 skill points = 0.025% per point = 25% at GM
            return necromancyArts * 0.0025;
        }

        /// <summary>
        /// Get Elementalism elemental damage bonus
        /// Formula: +0.25% per 10 skill points (25% at GM)
        /// </summary>
        public static double GetElementalismDamageBonus(Mobile caster)
        {
            if (caster == null || caster.Skills == null)
                return 0.0;

            double elementalism = caster.Skills[SkillName.Elementalism].Value;
            if (elementalism <= 0)
                return 0.0;

            // +0.25% per 10 skill points = 0.025% per point = 25% at GM
            return elementalism * 0.0025;
        }

        /// <summary>
        /// Get Hexcraft hex/curse damage bonus
        /// Formula: +0.25% per 10 skill points (25% at GM)
        /// </summary>
        public static double GetHexcraftDamageBonus(Mobile caster)
        {
            if (caster == null || caster.Skills == null)
                return 0.0;

            double hexcraft = caster.Skills[SkillName.Hexcraft].Value;
            if (hexcraft <= 0)
                return 0.0;

            // +0.25% per 10 skill points = 0.025% per point = 25% at GM
            return hexcraft * 0.0025;
        }

        /// <summary>
        /// Get SpiritCalling shaman spell damage bonus
        /// Formula: +0.25% per 10 skill points (25% at GM)
        /// </summary>
        public static double GetSpiritCallingDamageBonus(Mobile caster)
        {
            if (caster == null || caster.Skills == null)
                return 0.0;

            double spiritCalling = caster.Skills[SkillName.SpiritCalling].Value;
            if (spiritCalling <= 0)
                return 0.0;

            // +0.25% per 10 skill points = 0.025% per point = 25% at GM
            return spiritCalling * 0.0025;
        }

        /// <summary>
        /// Get Conjuration summon pet damage scaling
        /// Formula: 0.8x at 0 skill to 1.5x at 100 skill
        /// </summary>
        public static double GetConjurationPetDamageBonus(Mobile owner)
        {
            if (owner == null || owner.Skills == null)
                return 1.0;

            double conjuration = owner.Skills[SkillName.Conjuration].Value;
            // Scale from 0.8x at 0 skill to 1.5x at 100 skill
            // Formula: 0.8 + (skillValue / 200.0)
            return 0.8 + (conjuration / 200.0);
        }

        #region Religion Passive Bonuses

        /// <summary>
        /// Get religion passive resistance bonus
        /// </summary>
        public static int GetReligionResistanceBonus(Mobile mobile, ResistanceType type)
        {
            if (mobile == null || !(mobile is PlayerMobile pm))
                return 0;

            var pietyData = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetPiety(pm);
            if (pietyData == null || pietyData.Religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.None)
                return 0;

            var tier = Server.Custom.VystiaClasses.Religion.ReligionData.GetTier(pietyData.Piety);
            
            // Need Initiate (50) or Devoted (200) tier for bonuses
            if (tier < Server.Custom.VystiaClasses.Religion.PietyTier.Initiate)
                return 0;

            switch (pietyData.Religion)
            {
                case Server.Custom.VystiaClasses.Religion.VystiaReligion.FrosthelmFaith:
                    if (type == ResistanceType.Cold && tier >= Server.Custom.VystiaClasses.Religion.PietyTier.Initiate)
                        return 5; // +5% Cold Resistance at Initiate
                    break;

                case Server.Custom.VystiaClasses.Religion.VystiaReligion.SuryasSandscript:
                    if (type == ResistanceType.Fire && tier >= Server.Custom.VystiaClasses.Religion.PietyTier.Initiate)
                        return 5; // +5% Fire Resistance at Initiate
                    break;

                case Server.Custom.VystiaClasses.Religion.VystiaReligion.LunarasCovenant:
                    if (type == ResistanceType.Poison && tier >= Server.Custom.VystiaClasses.Religion.PietyTier.Initiate)
                        return 5; // +5% Poison Resistance at Initiate
                    break;

                case Server.Custom.VystiaClasses.Religion.VystiaReligion.CelestisArcanum:
                    if (type == ResistanceType.Energy && tier >= Server.Custom.VystiaClasses.Religion.PietyTier.Initiate)
                        return 5; // +5% Energy Resistance at Initiate
                    break;

                case Server.Custom.VystiaClasses.Religion.VystiaReligion.OceanasCovenant:
                    if (type == ResistanceType.Physical && tier >= Server.Custom.VystiaClasses.Religion.PietyTier.Initiate)
                        return 3; // +3% Physical Resistance at Initiate
                    break;
            }

            return 0;
        }

        /// <summary>
        /// Get religion skill bonus (e.g., +3 Engineering for Cogsmith Creed at Initiate, +5 at Devoted)
        /// </summary>
        public static double GetReligionSkillBonus(Mobile mobile, SkillName skill)
        {
            if (mobile == null || !(mobile is PlayerMobile pm))
                return 0.0;

            var pietyData = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetPiety(pm);
            if (pietyData == null || pietyData.Religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.None)
                return 0.0;

            var tier = Server.Custom.VystiaClasses.Religion.ReligionData.GetTier(pietyData.Piety);
            
            if (tier < Server.Custom.VystiaClasses.Religion.PietyTier.Initiate)
                return 0.0;

            // Cogsmith Creed: +3 crafting at Initiate, +5 at Devoted
            if (pietyData.Religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.CogsmithCreed)
            {
                // Check if this is a crafting skill
                if (skill == SkillName.Blacksmith || skill == SkillName.Tinkering || 
                    skill == SkillName.Carpentry || skill == SkillName.Tailoring ||
                    skill == SkillName.Alchemy || skill == SkillName.Inscribe ||
                    skill == SkillName.Cooking || skill == SkillName.Cartography ||
                    skill == SkillName.Engineering)
                {
                    if (tier >= Server.Custom.VystiaClasses.Religion.PietyTier.Devoted)
                        return 5.0; // +5 at Devoted
                    else if (tier >= Server.Custom.VystiaClasses.Religion.PietyTier.Initiate)
                        return 3.0; // +3 at Initiate
                }
            }

            return 0.0;
        }

        /// <summary>
        /// Get religion HP bonus (Frosthelm Faith: +5 at Initiate, +10 at Devoted)
        /// Note: Design doc shows Frosthelm Faith, but code has Frosthelm Faith
        /// Using Frosthelm Faith as implemented
        /// </summary>
        public static int GetReligionHPBonus(Mobile mobile)
        {
            if (mobile == null || !(mobile is PlayerMobile pm))
                return 0;

            var pietyData = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetPiety(pm);
            if (pietyData == null || pietyData.Religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.None)
                return 0;

            var tier = Server.Custom.VystiaClasses.Religion.ReligionData.GetTier(pietyData.Piety);
            
            // Note: Design doc mentions Frosthelm Faith, but implementation uses Frosthelm Faith
            // For now, no HP bonus in current implementation - would need to add if Frosthelm Faith is added
            return 0;
        }

        /// <summary>
        /// Get religion mana regen bonus (Celestis Arcanum: +2 at Devoted)
        /// </summary>
        public static int GetReligionManaRegenBonus(Mobile mobile)
        {
            if (mobile == null || !(mobile is PlayerMobile pm))
                return 0;

            var pietyData = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetPiety(pm);
            if (pietyData == null || pietyData.Religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.None)
                return 0;

            var tier = Server.Custom.VystiaClasses.Religion.ReligionData.GetTier(pietyData.Piety);
            
            if (pietyData.Religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.CelestisArcanum)
            {
                if (tier >= Server.Custom.VystiaClasses.Religion.PietyTier.Devoted)
                    return 2; // +2 Mana Regen at Devoted
            }

            return 0;
        }

        /// <summary>
        /// Get religion stealth bonus (Oceana's Covenant: +5 at Devoted)
        /// </summary>
        public static int GetReligionStealthBonus(Mobile mobile)
        {
            if (mobile == null || !(mobile is PlayerMobile pm))
                return 0;

            var pietyData = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetPiety(pm);
            if (pietyData == null || pietyData.Religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.None)
                return 0;

            var tier = Server.Custom.VystiaClasses.Religion.ReligionData.GetTier(pietyData.Piety);
            
            if (pietyData.Religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.OceanasCovenant)
            {
                if (tier >= Server.Custom.VystiaClasses.Religion.PietyTier.Devoted)
                    return 5; // +5 Stealth at Devoted
            }

            return 0;
        }

        /// <summary>
        /// Get religion armor bonus (Cogsmith Creed Initiate: +5 Armor)
        /// </summary>
        public static int GetReligionArmorBonus(Mobile mobile)
        {
            if (mobile == null || !(mobile is PlayerMobile pm))
                return 0;

            var pietyData = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetPiety(pm);
            if (pietyData == null || pietyData.Religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.None)
                return 0;

            var tier = Server.Custom.VystiaClasses.Religion.ReligionData.GetTier(pietyData.Piety);
            
            // Cogsmith Creed Initiate (50): Armor +5
            if (pietyData.Religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.CogsmithCreed &&
                tier >= Server.Custom.VystiaClasses.Religion.PietyTier.Initiate)
            {
                return 5; // +5 Armor
            }

            return 0;
        }

        /// <summary>
        /// Get religion damage bonus by type (Frosthelm Faith/Surya's Sandscript Devoted: +3% damage)
        /// </summary>
        public static double GetReligionDamageBonus(Mobile mobile, VystiaDamageType type)
        {
            if (mobile == null || !(mobile is PlayerMobile pm))
                return 0.0;

            var pietyData = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetPiety(pm);
            if (pietyData == null || pietyData.Religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.None)
                return 0.0;

            var tier = Server.Custom.VystiaClasses.Religion.ReligionData.GetTier(pietyData.Piety);
            
            if (tier < Server.Custom.VystiaClasses.Religion.PietyTier.Devoted)
                return 0.0;

            // Frosthelm Faith Devoted (200): Cold damage +3%
            if (pietyData.Religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.FrosthelmFaith &&
                type == VystiaDamageType.Cold)
            {
                return 0.03; // +3% cold damage
            }

            // Surya's Sandscript Devoted (200): Fire damage +3%
            if (pietyData.Religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.SuryasSandscript &&
                type == VystiaDamageType.Fire)
            {
                return 0.03; // +3% fire damage
            }

            return 0.0;
        }

        /// <summary>
        /// Get religion healing bonus (Lunara's Covenant Devoted: +5% healing)
        /// </summary>
        public static double GetReligionHealingBonus(Mobile mobile)
        {
            if (mobile == null || !(mobile is PlayerMobile pm))
                return 0.0;

            var pietyData = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetPiety(pm);
            if (pietyData == null || pietyData.Religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.None)
                return 0.0;

            var tier = Server.Custom.VystiaClasses.Religion.ReligionData.GetTier(pietyData.Piety);
            
            // Lunara's Covenant Devoted (200): Healing +5%
            if (pietyData.Religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.LunarasCovenant &&
                tier >= Server.Custom.VystiaClasses.Religion.PietyTier.Devoted)
            {
                return 0.05; // +5% healing
            }

            return 0.0;
        }

        #endregion

        #region Class-Religion Synergy Bonuses

        /// <summary>
        /// Get class-religion synergy bonus for resource decay/regen
        /// </summary>
        public static double GetClassReligionSynergyDecayBonus(PlayerMobile pm, string resourceType)
        {
            if (pm == null)
                return 0.0;

            var classType = Server.Custom.VystiaClasses.VystiaClassManager.Instance.GetClassType(pm);
            var religion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(pm);

            // Barbarian + Frosthelm Faith: Fury decay -15%
            if (classType == Server.Custom.VystiaClasses.PlayerClassTypeV2.Barbarian && 
                religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.FrosthelmFaith &&
                resourceType == "Fury")
            {
                return -0.15; // -15% decay (slower decay)
            }

            return 0.0;
        }

        /// <summary>
        /// Get class-religion synergy bonus for resource regen
        /// </summary>
        public static double GetClassReligionSynergyRegenBonus(PlayerMobile pm, string resourceType)
        {
            if (pm == null)
                return 0.0;

            var classType = Server.Custom.VystiaClasses.VystiaClassManager.Instance.GetClassType(pm);
            var religion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(pm);

            // Artificer + Cogsmith Creed: Steam regen +15%
            if (classType == Server.Custom.VystiaClasses.PlayerClassTypeV2.Artificer && 
                religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.CogsmithCreed &&
                resourceType == "Steam")
            {
                return 0.15; // +15% regen
            }

            return 0.0;
        }

        /// <summary>
        /// Get class-religion synergy bonus for resource maximum
        /// </summary>
        public static int GetClassReligionSynergyMaxBonus(PlayerMobile pm, string resourceType)
        {
            if (pm == null)
                return 0;

            var classType = Server.Custom.VystiaClasses.VystiaClassManager.Instance.GetClassType(pm);
            var religion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(pm);

            // Ranger + Celestis Arcanum: +10 maximum Focus
            // Note: Design doc shows Surya's Sandscript, but implementation uses Celestis Arcanum
            // Using Celestis Arcanum as closest match
            if (classType == Server.Custom.VystiaClasses.PlayerClassTypeV2.Ranger && 
                religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.CelestisArcanum &&
                resourceType == "Focus")
            {
                return 10; // +10 max Focus
            }

            // Bard + Celestis Arcanum: +3 max Crescendo
            if (classType == Server.Custom.VystiaClasses.PlayerClassTypeV2.Bard && 
                religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.CelestisArcanum &&
                resourceType == "Crescendo")
            {
                return 3; // +3 max Crescendo
            }

            // Necromancer + Celestis Arcanum: +15 max LifeForce
            if (classType == Server.Custom.VystiaClasses.PlayerClassTypeV2.Necromancer && 
                religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.CelestisArcanum &&
                resourceType == "LifeForce")
            {
                return 15; // +15 max LifeForce
            }

            // Knight + Frosthelm Faith: +2 max Fortitude
            if (classType == Server.Custom.VystiaClasses.PlayerClassTypeV2.Knight && 
                religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.FrosthelmFaith &&
                resourceType == "Fortitude")
            {
                return 2; // +2 max Fortitude
            }

            // Bounty Hunter + Celestis Arcanum: +1 max Pursuit
            if (classType == Server.Custom.VystiaClasses.PlayerClassTypeV2.BountyHunter && 
                religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.CelestisArcanum &&
                resourceType == "Pursuit")
            {
                return 1; // +1 max Pursuit
            }

            return 0;
        }

        /// <summary>
        /// Get class-religion synergy bonus for buff duration
        /// </summary>
        public static double GetClassReligionSynergyDurationBonus(PlayerMobile pm, string buffType)
        {
            if (pm == null)
                return 0.0;

            var classType = Server.Custom.VystiaClasses.VystiaClassManager.Instance.GetClassType(pm);
            var religion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(pm);

            // Druid + Lunara's Covenant: Shapeshift duration +25%
            if (classType == Server.Custom.VystiaClasses.PlayerClassTypeV2.Druid && 
                religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.LunarasCovenant &&
                (buffType == "Shapeshift" || buffType.Contains("Form")))
            {
                return 0.25; // +25% duration
            }

            // Witch + Lunara's Covenant: Hex duration +15%
            if (classType == Server.Custom.VystiaClasses.PlayerClassTypeV2.Witch && 
                religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.LunarasCovenant &&
                buffType == "Hex")
            {
                return 0.15; // +15% duration
            }

            // Shaman + Lunara's Covenant: Totem duration +25%
            if (classType == Server.Custom.VystiaClasses.PlayerClassTypeV2.Shaman && 
                religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.LunarasCovenant &&
                buffType == "Totem")
            {
                return 0.25; // +25% duration
            }

            // Ice Mage + Frosthelm Faith: Freeze duration +10%
            if (classType == Server.Custom.VystiaClasses.PlayerClassTypeV2.IceMage && 
                religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.FrosthelmFaith &&
                buffType == "Freeze")
            {
                return 0.10; // +10% duration
            }

            // Illusionist + Celestis Arcanum: Illusion duration +15%
            if (classType == Server.Custom.VystiaClasses.PlayerClassTypeV2.Illusionist && 
                religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.CelestisArcanum &&
                (buffType == "Illusion" || buffType.Contains("Illusion")))
            {
                return 0.15; // +15% duration
            }

            return 0.0;
        }

        /// <summary>
        /// Get class-religion synergy bonus for resource generation
        /// </summary>
        public static double GetClassReligionSynergyGenerationBonus(PlayerMobile pm, string resourceType)
        {
            if (pm == null)
                return 0.0;

            var classType = Server.Custom.VystiaClasses.VystiaClassManager.Instance.GetClassType(pm);
            var religion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(pm);

            // Templar + Cogsmith Creed: Zeal generation +15%
            if (classType == Server.Custom.VystiaClasses.PlayerClassTypeV2.Templar && 
                religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.CogsmithCreed &&
                resourceType == "Zeal")
            {
                return 0.15; // +15% generation
            }

            // Warlock + Celestis Arcanum: SoulShard gen +5%
            if (classType == Server.Custom.VystiaClasses.PlayerClassTypeV2.Warlock && 
                religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.CelestisArcanum &&
                resourceType == "SoulShards")
            {
                return 0.05; // +5% generation
            }

            // Cleric + Any Religion: Faith generation +15%
            if (classType == Server.Custom.VystiaClasses.PlayerClassTypeV2.Cleric && 
                religion != Server.Custom.VystiaClasses.Religion.VystiaReligion.None &&
                resourceType == "Faith")
            {
                return 0.15; // +15% generation
            }

            // Paladin + Celestis Arcanum: Virtue accumulation +15%
            if (classType == Server.Custom.VystiaClasses.PlayerClassTypeV2.Paladin && 
                religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.CelestisArcanum &&
                resourceType == "Virtues")
            {
                return 0.15; // +15% virtue accumulation
            }

            return 0.0;
        }

        /// <summary>
        /// Get class-religion synergy bonus for mana regen
        /// </summary>
        public static int GetClassReligionSynergyManaRegenBonus(PlayerMobile pm)
        {
            if (pm == null)
                return 0;

            var classType = Server.Custom.VystiaClasses.VystiaClassManager.Instance.GetClassType(pm);
            var religion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(pm);

            // Wizard + Celestis Arcanum: Mana regen +10%
            // Note: This is a percentage bonus, not flat +10
            // We'll return 0 here and apply as percentage in PlayerMobile
            // Actually, let's return a percentage multiplier value
            if (classType == Server.Custom.VystiaClasses.PlayerClassTypeV2.Wizard && 
                religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.CelestisArcanum)
            {
                return 10; // +10% mana regen (will be applied as percentage)
            }

            return 0;
        }

        /// <summary>
        /// Get class-religion synergy bonus for ability cost reduction
        /// </summary>
        public static double GetClassReligionSynergyCostReduction(PlayerMobile pm, string resourceType)
        {
            if (pm == null)
                return 0.0;

            var classType = Server.Custom.VystiaClasses.VystiaClassManager.Instance.GetClassType(pm);
            var religion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(pm);

            // Monk + Cogsmith Creed: Chi ability cost -10%
            if (classType == Server.Custom.VystiaClasses.PlayerClassTypeV2.Monk && 
                religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.CogsmithCreed &&
                resourceType == "Chi")
            {
                return 0.10; // -10% cost
            }

            return 0.0;
        }

        /// <summary>
        /// Get class-religion synergy bonus for pet resistance
        /// </summary>
        public static int GetClassReligionSynergyPetResistBonus(PlayerMobile pm, ResistanceType type)
        {
            if (pm == null)
                return 0;

            var classType = Server.Custom.VystiaClasses.VystiaClassManager.Instance.GetClassType(pm);
            var religion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(pm);

            // Beastmaster + Frosthelm Faith: Pets +10% cold resist
            if (classType == Server.Custom.VystiaClasses.PlayerClassTypeV2.Beastmaster && 
                religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.FrosthelmFaith &&
                type == ResistanceType.Cold)
            {
                return 10; // +10% cold resistance
            }

            return 0;
        }

        /// <summary>
        /// Get class-religion synergy bonus for block chance
        /// </summary>
        public static double GetClassReligionSynergyBlockBonus(PlayerMobile pm)
        {
            if (pm == null)
                return 0.0;

            var classType = Server.Custom.VystiaClasses.VystiaClassManager.Instance.GetClassType(pm);
            var religion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(pm);

            // Knight + Frosthelm Faith: +5% block chance
            if (classType == Server.Custom.VystiaClasses.PlayerClassTypeV2.Knight && 
                religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.FrosthelmFaith)
            {
                return 0.05; // +5% block chance
            }

            return 0.0;
        }

        /// <summary>
        /// Get class-religion synergy bonus for spell damage by type
        /// </summary>
        public static double GetClassReligionSynergySpellDamageBonus(PlayerMobile pm, VystiaDamageType type)
        {
            if (pm == null)
                return 0.0;

            var classType = Server.Custom.VystiaClasses.VystiaClassManager.Instance.GetClassType(pm);
            var religion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(pm);

            // Sorcerer + Cogsmith Creed: Fire spells +5% damage
            if (classType == Server.Custom.VystiaClasses.PlayerClassTypeV2.Sorcerer && 
                religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.CogsmithCreed &&
                type == VystiaDamageType.Fire)
            {
                return 0.05; // +5% fire spell damage
            }

            return 0.0;
        }

        /// <summary>
        /// Get class-religion synergy bonus for repair cost reduction
        /// </summary>
        public static double GetClassReligionSynergyRepairCostReduction(PlayerMobile pm)
        {
            if (pm == null)
                return 0.0;

            var classType = Server.Custom.VystiaClasses.VystiaClassManager.Instance.GetClassType(pm);
            var religion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(pm);

            // Fighter + Cogsmith Creed: Repair costs -15%
            if (classType == Server.Custom.VystiaClasses.PlayerClassTypeV2.Fighter && 
                religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.CogsmithCreed)
            {
                return 0.15; // -15% repair cost
            }

            return 0.0;
        }

        /// <summary>
        /// Get class-religion synergy bonus for potion effectiveness
        /// </summary>
        public static double GetClassReligionSynergyPotionBonus(PlayerMobile pm)
        {
            if (pm == null)
                return 0.0;

            var classType = Server.Custom.VystiaClasses.VystiaClassManager.Instance.GetClassType(pm);
            var religion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(pm);

            // Alchemist + Lunara's Covenant: Potion effectiveness +10%
            if (classType == Server.Custom.VystiaClasses.PlayerClassTypeV2.Alchemist && 
                religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.LunarasCovenant)
            {
                return 0.10; // +10% potion effectiveness
            }

            return 0.0;
        }

        /// <summary>
        /// Get class-religion synergy bonus for skill (e.g., Devotion)
        /// </summary>
        public static double GetClassReligionSynergySkillBonus(PlayerMobile pm, SkillName skill)
        {
            if (pm == null)
                return 0.0;

            var classType = Server.Custom.VystiaClasses.VystiaClassManager.Instance.GetClassType(pm);
            var religion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(pm);

            // Oracle + Celestis Arcanum: +5 Divination skill
            // Note: Plan mentions "Devotion" but Oracle uses Divination skill
            if (classType == Server.Custom.VystiaClasses.PlayerClassTypeV2.Oracle && 
                religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.CelestisArcanum &&
                skill == SkillName.Divination)
            {
                return 5.0; // +5 Divination skill
            }

            return 0.0;
        }

        /// <summary>
        /// Get class-religion synergy bonus for mark damage (Bounty Hunter)
        /// </summary>
        public static double GetClassReligionSynergyMarkDamageBonus(PlayerMobile pm)
        {
            if (pm == null)
                return 0.0;

            var classType = Server.Custom.VystiaClasses.VystiaClassManager.Instance.GetClassType(pm);
            var religion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(pm);

            // Bounty Hunter + Celestis Arcanum: +10% mark damage
            if (classType == Server.Custom.VystiaClasses.PlayerClassTypeV2.BountyHunter && 
                religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.CelestisArcanum)
            {
                return 0.10; // +10% mark damage
            }

            return 0.0;
        }

        /// <summary>
        /// Get class-religion synergy bonus for pet HP (Summoner)
        /// </summary>
        public static double GetClassReligionSynergyPetHPBonus(PlayerMobile pm)
        {
            if (pm == null)
                return 0.0;

            var classType = Server.Custom.VystiaClasses.VystiaClassManager.Instance.GetClassType(pm);
            var religion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(pm);

            // Summoner + Celestis Arcanum: Summon HP +15%
            if (classType == Server.Custom.VystiaClasses.PlayerClassTypeV2.Summoner && 
                religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.CelestisArcanum)
            {
                return 0.15; // +15% summon HP
            }

            return 0.0;
        }

        /// <summary>
        /// Get class-religion synergy bonus for enchant success
        /// </summary>
        public static double GetClassReligionSynergyEnchantBonus(PlayerMobile pm)
        {
            if (pm == null)
                return 0.0;

            var classType = Server.Custom.VystiaClasses.VystiaClassManager.Instance.GetClassType(pm);
            var religion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(pm);

            // Enchanter + Celestis Arcanum: Enchant success +5%
            if (classType == Server.Custom.VystiaClasses.PlayerClassTypeV2.Enchanter && 
                religion == Server.Custom.VystiaClasses.Religion.VystiaReligion.CelestisArcanum)
            {
                return 0.05; // +5% enchant success
            }

            return 0.0;
        }

        /// <summary>
        /// Get religion PvP damage bonus vs opposed religion
        /// Tiers: Initiate +2%, Adherent +4%, Devoted +6%, Zealot +8%, Champion +10%
        /// </summary>
        public static double GetReligionPvPDamageBonus(PlayerMobile attacker, PlayerMobile target)
        {
            if (attacker == null || target == null)
                return 0.0;

            // Check if both are players (PvP)
            if (!(attacker is PlayerMobile) || !(target is PlayerMobile))
                return 0.0;

            // Check if religions are opposed
            var attackerReligion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(attacker);
            var targetReligion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(target);

            if (!Server.Custom.VystiaClasses.Religion.VystiaReligionSystem.AreReligionsOpposed(attackerReligion, targetReligion))
                return 0.0;

            // Get attacker's piety tier
            var pietyData = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetPiety(attacker);
            if (pietyData == null)
                return 0.0;

            var tier = Server.Custom.VystiaClasses.Religion.ReligionData.GetTier(pietyData.Piety);

            // Return bonus based on tier
            // Note: Plan mentions "Zealot" and "Champion" but piety tiers are:
            // Initiate (50), Devoted (200), Faithful (500), Exalted (900)
            // Mapping: Initiate +2%, Devoted +4%, Faithful +6%, Exalted +8%
            // For 1000 piety (max), we could add +10% but Exalted covers 900+
            switch (tier)
            {
                case Server.Custom.VystiaClasses.Religion.PietyTier.Initiate: return 0.02; // +2%
                case Server.Custom.VystiaClasses.Religion.PietyTier.Devoted: return 0.04; // +4%
                case Server.Custom.VystiaClasses.Religion.PietyTier.Faithful: return 0.06; // +6%
                case Server.Custom.VystiaClasses.Religion.PietyTier.Exalted: return 0.08; // +8% (could be +10% at 1000 piety)
                default: return 0.0;
            }
        }

        /// <summary>
        /// Get religion PvP damage reduction (Champion tier: -3% damage taken)
        /// </summary>
        public static double GetReligionPvPDamageReduction(PlayerMobile defender, PlayerMobile attacker)
        {
            if (defender == null || attacker == null)
                return 0.0;

            // Check if both are players (PvP)
            if (!(defender is PlayerMobile) || !(attacker is PlayerMobile))
                return 0.0;

            // Check if religions are opposed
            var defenderReligion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(defender);
            var attackerReligion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(attacker);

            if (!Server.Custom.VystiaClasses.Religion.VystiaReligionSystem.AreReligionsOpposed(defenderReligion, attackerReligion))
                return 0.0;

            // Champion tier (900+ piety, Exalted) only
            var pietyData = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetPiety(defender);
            if (pietyData == null)
                return 0.0;

            var tier = Server.Custom.VystiaClasses.Religion.ReligionData.GetTier(pietyData.Piety);

            // Exalted tier (900+) = Champion
            if (tier >= Server.Custom.VystiaClasses.Religion.PietyTier.Exalted)
                return 0.03; // -3% damage taken

            return 0.0;
        }

        /// <summary>
        /// Get religion healing/buff effectiveness multiplier
        /// Same religion: 100%, Neutral: 100%, Opposed: 50%, Different (non-opposed): 100%
        /// </summary>
        public static double GetReligionHealingEffectiveness(PlayerMobile healer, PlayerMobile target)
        {
            if (healer == null || target == null)
                return 1.0;

            var healerReligion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(healer);
            var targetReligion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(target);

            // Same religion: 100%
            if (healerReligion == targetReligion)
                return 1.0;

            // Neutral (no religion): 100%
            if (healerReligion == Server.Custom.VystiaClasses.Religion.VystiaReligion.None ||
                targetReligion == Server.Custom.VystiaClasses.Religion.VystiaReligion.None)
                return 1.0;

            // Opposed religion: 50%
            if (Server.Custom.VystiaClasses.Religion.VystiaReligionSystem.AreReligionsOpposed(healerReligion, targetReligion))
                return 0.5;

            // Different (non-opposed): 100% (no penalty)
            return 1.0;
        }

        /// <summary>
        /// Get blessed item HP bonus for equipped items
        /// </summary>
        public static int GetBlessedItemHPBonus(PlayerMobile pm)
        {
            if (pm == null)
                return 0;

            int totalBonus = 0;
            var userReligion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(pm);

            // Check all equipped items
            foreach (Item item in pm.Items)
            {
                if (item is IBlessedItem blessedItem && blessedItem.BlessingReligion != Server.Custom.VystiaClasses.Religion.VystiaReligion.None)
                {
                    double effectiveness = Server.Custom.VystiaClasses.Religion.VystiaBlessedItemSystem.GetBlessingEffectiveness(pm, blessedItem.BlessingReligion);
                    if (effectiveness > 0)
                    {
                        // Frosthelm Faith: +5 HP (standard), +10 HP (critical)
                        if (blessedItem.BlessingReligion == Server.Custom.VystiaClasses.Religion.VystiaReligion.FrosthelmFaith)
                        {
                            int bonus = blessedItem.BlessingType == BlessingType.Critical ? 10 : 5;
                            totalBonus += (int)(bonus * effectiveness);
                        }
                    }
                }
            }

            return totalBonus;
        }

        /// <summary>
        /// Get blessed item Mana bonus for equipped items
        /// </summary>
        public static int GetBlessedItemManaBonus(PlayerMobile pm)
        {
            if (pm == null)
                return 0;

            int totalBonus = 0;
            var userReligion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(pm);

            // Check all equipped items
            foreach (Item item in pm.Items)
            {
                if (item is IBlessedItem blessedItem && blessedItem.BlessingReligion != Server.Custom.VystiaClasses.Religion.VystiaReligion.None)
                {
                    double effectiveness = Server.Custom.VystiaClasses.Religion.VystiaBlessedItemSystem.GetBlessingEffectiveness(pm, blessedItem.BlessingReligion);
                    if (effectiveness > 0)
                    {
                        // Surya's Sandscript: +5 Mana (standard), +10 Mana (critical)
                        if (blessedItem.BlessingReligion == Server.Custom.VystiaClasses.Religion.VystiaReligion.SuryasSandscript)
                        {
                            int bonus = blessedItem.BlessingType == BlessingType.Critical ? 10 : 5;
                            totalBonus += (int)(bonus * effectiveness);
                        }
                    }
                }
            }

            return totalBonus;
        }

        /// <summary>
        /// Get blessed item fire damage bonus for equipped weapons
        /// </summary>
        public static double GetBlessedItemFireDamageBonus(PlayerMobile pm, Item weapon)
        {
            if (pm == null || weapon == null)
                return 0.0;

            if (!(weapon is IBlessedItem blessedItem) || blessedItem.BlessingReligion != Server.Custom.VystiaClasses.Religion.VystiaReligion.CogsmithCreed)
                return 0.0;

            double effectiveness = Server.Custom.VystiaClasses.Religion.VystiaBlessedItemSystem.GetBlessingEffectiveness(pm, blessedItem.BlessingReligion);
            if (effectiveness <= 0)
                return 0.0;

            // Cogsmith Creed: +5% Fire Damage (standard), +10% Fire (critical)
            double bonus = blessedItem.BlessingType == BlessingType.Critical ? 0.10 : 0.05;
            return bonus * effectiveness;
        }

        /// <summary>
        /// Get blessed item healing power bonus for equipped items
        /// </summary>
        public static double GetBlessedItemHealingBonus(PlayerMobile pm)
        {
            if (pm == null)
                return 0.0;

            double totalBonus = 0.0;
            var userReligion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(pm);

            // Check all equipped items
            foreach (Item item in pm.Items)
            {
                if (item is IBlessedItem blessedItem && blessedItem.BlessingReligion != Server.Custom.VystiaClasses.Religion.VystiaReligion.None)
                {
                    double effectiveness = Server.Custom.VystiaClasses.Religion.VystiaBlessedItemSystem.GetBlessingEffectiveness(pm, blessedItem.BlessingReligion);
                    if (effectiveness > 0)
                    {
                        // Lunara's Covenant: +5% Healing Power (standard), +10% Healing (critical)
                        if (blessedItem.BlessingReligion == Server.Custom.VystiaClasses.Religion.VystiaReligion.LunarasCovenant)
                        {
                            double bonus = blessedItem.BlessingType == BlessingType.Critical ? 0.10 : 0.05;
                            totalBonus += bonus * effectiveness;
                        }
                    }
                }
            }

            return totalBonus;
        }

        /// <summary>
        /// Get blessed item hit chance bonus for equipped items
        /// </summary>
        public static double GetBlessedItemHitChanceBonus(PlayerMobile pm)
        {
            if (pm == null)
                return 0.0;

            double totalBonus = 0.0;
            var userReligion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(pm);

            // Check all equipped items
            foreach (Item item in pm.Items)
            {
                if (item is IBlessedItem blessedItem && blessedItem.BlessingReligion != Server.Custom.VystiaClasses.Religion.VystiaReligion.None)
                {
                    double effectiveness = Server.Custom.VystiaClasses.Religion.VystiaBlessedItemSystem.GetBlessingEffectiveness(pm, blessedItem.BlessingReligion);
                    if (effectiveness > 0)
                    {
                        // Celestis Arcanum: +5% Hit Chance (standard), +10% Hit Chance (critical)
                        if (blessedItem.BlessingReligion == Server.Custom.VystiaClasses.Religion.VystiaReligion.CelestisArcanum)
                        {
                            double bonus = blessedItem.BlessingType == BlessingType.Critical ? 0.10 : 0.05;
                            totalBonus += bonus * effectiveness;
                        }
                    }
                }
            }

            return totalBonus;
        }

        /// <summary>
        /// Get blessed item spell damage bonus for equipped items
        /// </summary>
        public static double GetBlessedItemSpellDamageBonus(PlayerMobile pm)
        {
            if (pm == null)
                return 0.0;

            double totalBonus = 0.0;
            var userReligion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(pm);

            // Check all equipped items
            foreach (Item item in pm.Items)
            {
                if (item is IBlessedItem blessedItem && blessedItem.BlessingReligion != Server.Custom.VystiaClasses.Religion.VystiaReligion.None)
                {
                    double effectiveness = Server.Custom.VystiaClasses.Religion.VystiaBlessedItemSystem.GetBlessingEffectiveness(pm, blessedItem.BlessingReligion);
                    if (effectiveness > 0)
                    {
                        // Oceana's Covenant: +5% Spell Damage (standard), +10% Spell Damage (critical)
                        if (blessedItem.BlessingReligion == Server.Custom.VystiaClasses.Religion.VystiaReligion.OceanasCovenant)
                        {
                            double bonus = blessedItem.BlessingType == BlessingType.Critical ? 0.10 : 0.05;
                            totalBonus += bonus * effectiveness;
                        }
                    }
                }
            }

            return totalBonus;
        }

        /// <summary>
        /// Get blessed item HP regen bonus for equipped items
        /// </summary>
        public static int GetBlessedItemHPRegenBonus(PlayerMobile pm)
        {
            if (pm == null)
                return 0;

            int totalBonus = 0;
            var userReligion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(pm);

            // Check all equipped items
            foreach (Item item in pm.Items)
            {
                if (item is IBlessedItem blessedItem && blessedItem.BlessingReligion != Server.Custom.VystiaClasses.Religion.VystiaReligion.None)
                {
                    double effectiveness = Server.Custom.VystiaClasses.Religion.VystiaBlessedItemSystem.GetBlessingEffectiveness(pm, blessedItem.BlessingReligion);
                    if (effectiveness > 0)
                    {
                        // Lunara's Covenant Critical: HP Regen 2
                        if (blessedItem.BlessingReligion == Server.Custom.VystiaClasses.Religion.VystiaReligion.LunarasCovenant &&
                            blessedItem.BlessingType == BlessingType.Critical)
                        {
                            totalBonus += (int)(2 * effectiveness);
                        }
                    }
                }
            }

            return totalBonus;
        }

        /// <summary>
        /// Get blessed item self-repair bonus for equipped weapons
        /// </summary>
        public static int GetBlessedItemSelfRepairBonus(PlayerMobile pm, Item weapon)
        {
            if (pm == null || weapon == null)
                return 0;

            if (!(weapon is IBlessedItem blessedItem) || blessedItem.BlessingReligion != Server.Custom.VystiaClasses.Religion.VystiaReligion.CogsmithCreed)
                return 0;

            if (blessedItem.BlessingType != BlessingType.Critical)
                return 0;

            double effectiveness = Server.Custom.VystiaClasses.Religion.VystiaBlessedItemSystem.GetBlessingEffectiveness(pm, blessedItem.BlessingReligion);
            if (effectiveness <= 0)
                return 0;

            // Cogsmith Creed Critical: Self-Repair 2
            return (int)(2 * effectiveness);
        }

        #endregion
    }
}

