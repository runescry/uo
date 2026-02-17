using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Custom.VystiaClasses.Skills;

namespace Server.Spells.VystiaSpells
{
    /// <summary>
    /// Base class for all Vystia magic spells.
    /// Provides skill requirements, fizzle chance, and skill gain mechanics.
    /// </summary>
    public abstract class VystiaSpell : MagerySpell
    {
        // Skill requirements by circle (matches standard UO Magery)
        private static readonly double[] CircleSkillRequirements = new double[]
        {
            0.0,   // Circle 1 - Anyone can attempt
            10.0,  // Circle 2
            20.0,  // Circle 3
            30.0,  // Circle 4
            45.0,  // Circle 5
            60.0,  // Circle 6
            75.0,  // Circle 7
            90.0   // Circle 8
        };

        // Skill range for gain chances (min skill where gain starts to max where gain stops)
        private static readonly double[] CircleGainMinSkill = new double[]
        {
            0.0, 0.0, 10.0, 20.0, 35.0, 50.0, 65.0, 80.0
        };

        private static readonly double[] CircleGainMaxSkill = new double[]
        {
            30.0, 40.0, 50.0, 60.0, 75.0, 90.0, 100.0, 100.0  // Vystia: 100 is GM, no power scrolls
        };

        /// <summary>
        /// The magic school this spell belongs to. Override in derived classes.
        /// </summary>
        public abstract VystiaMagicSchool MagicSchool { get; }

        public VystiaSpell(Mobile caster, Item scroll, SpellInfo info) : base(caster, scroll, info)
        {
        }

        /// <summary>
        /// Gets the SkillName for this spell's magic school
        /// </summary>
        public SkillName GetSchoolSkill()
        {
            return MagicSchoolToSkill(MagicSchool);
        }

        /// <summary>
        /// Maps magic school to its corresponding skill
        /// </summary>
        public static SkillName MagicSchoolToSkill(VystiaMagicSchool school)
        {
            switch (school)
            {
                case VystiaMagicSchool.Ice: return SkillName.Cryomancy;
                case VystiaMagicSchool.Nature: return SkillName.Druidism;
                case VystiaMagicSchool.Hex: return SkillName.Hexcraft;
                case VystiaMagicSchool.Elemental: return SkillName.Elementalism;
                case VystiaMagicSchool.Dark: return SkillName.Demonology;
                case VystiaMagicSchool.Divination: return SkillName.Divination;
                case VystiaMagicSchool.Necromancy: return SkillName.NecromancyArts;
                case VystiaMagicSchool.Summoning: return SkillName.Conjuration;
                case VystiaMagicSchool.Shamanic: return SkillName.SpiritCalling;
                case VystiaMagicSchool.Bardic: return SkillName.Songweaving;
                case VystiaMagicSchool.Enchanting: return SkillName.Runeweaving;
                case VystiaMagicSchool.Illusion: return SkillName.IllusionMagic;
                default: return SkillName.Magery; // Fallback
            }
        }

        /// <summary>
        /// Gets the minimum skill required for this spell's circle
        /// </summary>
        public double GetMinSkillRequired()
        {
            int circleIndex = (int)Circle;
            if (circleIndex >= 0 && circleIndex < CircleSkillRequirements.Length)
                return CircleSkillRequirements[circleIndex];
            return 0.0;
        }

        /// <summary>
        /// Gets the caster's skill in this spell's magic school
        /// </summary>
        public double GetCasterSkill()
        {
            if (Caster == null)
                return 0.0;

            SkillName skill = GetSchoolSkill();
            return Caster.Skills[skill].Value;
        }

        /// <summary>
        /// Calculates fizzle chance based on skill vs circle requirement
        /// </summary>
        public double GetFizzleChance()
        {
            double skill = GetCasterSkill();
            double required = GetMinSkillRequired();

            if (skill >= required + 30)
                return 0.0; // No fizzle if 30+ points above requirement

            if (skill < required)
            {
                // Below requirement: high fizzle chance (50-95%)
                double deficit = required - skill;
                return Math.Min(0.95, 0.50 + (deficit * 0.03));
            }

            // At or above requirement: decreasing fizzle chance
            double surplus = skill - required;
            return Math.Max(0.0, 0.30 - (surplus * 0.01));
        }

        // Body IDs for druid shapeshifts that block spellcasting
        private static readonly int[] ShapeshiftBodies = { 213, 225, 5, 301 }; // Bear, Wolf, Hawk, Treant

        /// <summary>
        /// Checks if the caster is currently in a shapeshift form that blocks spellcasting
        /// </summary>
        protected bool IsInShapeshiftForm()
        {
            if (Caster == null || Caster.BodyMod == 0)
                return false;

            foreach (int bodyId in ShapeshiftBodies)
            {
                if (Caster.BodyMod == bodyId)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the name of the current shapeshift form
        /// </summary>
        protected string GetShapeshiftName()
        {
            switch (Caster.BodyMod)
            {
                case 213: return "bear";
                case 225: return "wolf";
                case 5: return "hawk";
                case 301: return "treant";
                default: return "animal";
            }
        }

        /// <summary>
        /// Checks if this spell is a shapeshift spell (allowed to be cast to toggle off forms)
        /// </summary>
        protected bool IsShapeshiftSpell()
        {
            string typeName = GetType().Name;
            return typeName.Contains("BearForm") ||
                   typeName.Contains("WolfForm") ||
                   typeName.Contains("HawkForm") ||
                   typeName.Contains("TreantForm") ||
                   typeName.Contains("ForceofNature") ||
                   typeName.Contains("Shapeshift");
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            // Check for shapeshift form blocking spellcasting
            // Allow shapeshifting spells to be cast to toggle off the form
            if (IsInShapeshiftForm() && !IsShapeshiftSpell())
            {
                Caster.SendMessage(0x22, "You cannot cast spells while in {0} form! Cast the shapeshift spell again to return to normal.", GetShapeshiftName());
                return false;
            }

            double skill = GetCasterSkill();
            double required = GetMinSkillRequired();
            SkillName skillName = GetSchoolSkill();

            // Allow casting even below requirement, but with high fizzle chance
            // This creates risk/reward for attempting harder spells
            if (skill < required - 20)
            {
                // Way too difficult - can't even attempt
                Caster.SendMessage(0x22, "This spell is far beyond your current {0} skill ({1:F1}). You need at least {2:F1} to attempt it.",
                    skillName, skill, required - 20);
                return false;
            }

            if (skill < required)
            {
                Caster.SendMessage(0x35, "Warning: This spell requires {0:F1} {1}. Your skill is {2:F1}. High fizzle chance!",
                    required, skillName, skill);
            }

            return true;
        }

        /// <summary>
        /// Override CheckFizzle to use Vystia skill instead of Magery.
        /// This follows the standard UO pattern where CheckSkill determines both fizzle AND gain.
        /// </summary>
        public override bool CheckFizzle()
        {
            if (Scroll is BaseWand)
                return true;

            SkillName skill = GetSchoolSkill();
            int circleIndex = (int)Circle;

            // Get skill range for this circle (matches UO pattern: min/max determine both fizzle and gain)
            double minSkill = circleIndex < CircleGainMinSkill.Length ? CircleGainMinSkill[circleIndex] : 0.0;
            double maxSkill = circleIndex < CircleGainMaxSkill.Length ? CircleGainMaxSkill[circleIndex] : 100.0;  // Vystia: 100 is GM

            double currentSkill = Caster.Skills[skill].Value;
            double oldBase = Caster.Skills[skill].Base;

            // Debug output
            Caster.SendMessage(0x3B2, "[Spell Debug] {0} (School: {1})", GetType().Name, MagicSchool);
            Caster.SendMessage(0x3B2, "[Spell Debug] {0}={1:F1}, Circle {2} range: {3:F1}-{4:F1}",
                skill, currentSkill, circleIndex + 1, minSkill, maxSkill);

            // Single CheckSkill call - determines BOTH fizzle AND gain (UO standard pattern)
            // If skill < minSkill: auto-fail (fizzle), no gain possible
            // If skill >= maxSkill: auto-success, no gain possible (too easy)
            // If skill in range: random success/fail, gain possible on attempt
            bool success = Caster.CheckSkill(skill, minSkill, maxSkill);

            double newBase = Caster.Skills[skill].Base;

            if (newBase > oldBase)
            {
                Caster.SendMessage(0x35, "*** SKILL GAIN! {0}: {1:F1} -> {2:F1} ***", skill, oldBase, newBase);
            }

            if (!success)
            {
                Caster.SendMessage(0x3B2, "[Spell Debug] Skill check failed - spell will fizzle");
            }

            // BaseCreature always succeeds (like standard UO)
            return Caster is BaseCreature || success;
        }

        /// <summary>
        /// Gets mana cost for this spell
        /// </summary>
        protected new int GetMana()
        {
            // Standard UO mana costs by circle
            int[] manaCosts = { 4, 6, 9, 11, 14, 20, 40, 50 };
            int circleIndex = (int)Circle;

            if (circleIndex >= 0 && circleIndex < manaCosts.Length)
                return manaCosts[circleIndex];

            return 4;
        }

        /// <summary>
        /// Override GetNewAosDamage to apply Vystia magic school damage bonuses
        /// </summary>
        public override int GetNewAosDamage(int bonus, int dice, int sides, bool playerVsPlayer, double scalar, IDamageable damageable)
        {
            // Call base damage calculation
            int baseDamage = base.GetNewAosDamage(bonus, dice, sides, playerVsPlayer, scalar, damageable);

            // Vystia: Apply magic school damage bonus
            if (Caster is PlayerMobile pm)
            {
                double skillBonus = 0.0;

                switch (MagicSchool)
                {
                    case VystiaMagicSchool.Ice:
                        skillBonus = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetCryomancyDamageBonus(pm);
                        break;
                    case VystiaMagicSchool.Dark:
                        skillBonus = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetDemonologyDamageBonus(pm);
                        break;
                    case VystiaMagicSchool.Necromancy:
                        skillBonus = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetNecromancyArtsDamageBonus(pm);
                        break;
                    case VystiaMagicSchool.Elemental:
                        skillBonus = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetElementalismDamageBonus(pm);
                        break;
                    case VystiaMagicSchool.Hex:
                        skillBonus = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetHexcraftDamageBonus(pm);
                        break;
                    case VystiaMagicSchool.Shamanic:
                        skillBonus = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetSpiritCallingDamageBonus(pm);
                        break;
                }

                if (skillBonus > 0.0)
                {
                    baseDamage = (int)(baseDamage * (1.0 + skillBonus));
                }

                // Vystia: Sorcerer + Cogsmith Creed: Fire spells +5% damage
                // Check if this spell does fire damage (Elemental school with fire element, or Dark school with fire)
                if (MagicSchool == VystiaMagicSchool.Elemental || MagicSchool == VystiaMagicSchool.Dark)
                {
                    // For now, apply to all Elemental and Dark spells (can be refined later to check specific fire spells)
                    double fireSpellBonus = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetClassReligionSynergySpellDamageBonus(pm, Server.Custom.VystiaClasses.Systems.VystiaDamageType.Fire);
                    if (fireSpellBonus > 0)
                    {
                        baseDamage = (int)(baseDamage * (1.0 + fireSpellBonus));
                    }
                }

                // Vystia: Blessed item spell damage bonus (Oceana's Covenant)
                double blessedSpellDamageBonus = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetBlessedItemSpellDamageBonus(pm);
                if (blessedSpellDamageBonus > 0.0)
                {
                    baseDamage = (int)(baseDamage * (1.0 + blessedSpellDamageBonus)); // +5% or +10% spell damage
                }
            }

            return baseDamage;
        }
    }

    /// <summary>
    /// Vystia magic school enumeration
    /// </summary>
    public enum VystiaMagicSchool
    {
        Ice,        // Cryomancy - Ice Mage
        Nature,     // Druidism - Druid
        Hex,        // Hexcraft - Witch
        Elemental,  // Elementalism - Sorcerer
        Dark,       // Demonology - Warlock
        Divination, // Divination - Oracle
        Necromancy, // NecromancyArts - Necromancer
        Summoning,  // Conjuration - Summoner
        Shamanic,   // SpiritCalling - Shaman
        Bardic,     // Songweaving - Bard
        Enchanting, // Runeweaving - Enchanter
        Illusion    // IllusionMagic - Illusionist
    }
}

