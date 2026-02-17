using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Systems;

namespace Server.Custom.VystiaClasses.Skills
{
    /// <summary>
    /// Handles skill gain checks for Vystia custom class skills.
    /// Uses UO-style skill gain mechanics with class-specific triggers.
    /// </summary>
    public static class VystiaSkillCheck
    {
        /// <summary>
        /// Maps class types to their primary class skill.
        /// </summary>
        private static readonly Dictionary<PlayerClassTypeV2, SkillName> ClassSkillMap = new Dictionary<PlayerClassTypeV2, SkillName>
        {
            // Magic Classes
            { PlayerClassTypeV2.IceMage, SkillName.Cryomancy },
            { PlayerClassTypeV2.Warlock, SkillName.Demonology },
            { PlayerClassTypeV2.Necromancer, SkillName.NecromancyArts },
            { PlayerClassTypeV2.Druid, SkillName.Druidism },
            { PlayerClassTypeV2.Sorcerer, SkillName.Elementalism },
            { PlayerClassTypeV2.Bard, SkillName.Songweaving },
            { PlayerClassTypeV2.Witch, SkillName.Hexcraft },
            { PlayerClassTypeV2.Oracle, SkillName.Divination },
            { PlayerClassTypeV2.Summoner, SkillName.Conjuration },
            { PlayerClassTypeV2.Shaman, SkillName.SpiritCalling },
            { PlayerClassTypeV2.Enchanter, SkillName.Runeweaving },
            { PlayerClassTypeV2.Illusionist, SkillName.IllusionMagic },

            // Martial Classes
            { PlayerClassTypeV2.Barbarian, SkillName.Berserking },
            { PlayerClassTypeV2.Rogue, SkillName.Subterfuge },
            { PlayerClassTypeV2.Monk, SkillName.MartialArts },
            { PlayerClassTypeV2.Knight, SkillName.ChivalricArts },
            { PlayerClassTypeV2.Paladin, SkillName.HolyDevotion },
            { PlayerClassTypeV2.Ranger, SkillName.Marksmanship },
            { PlayerClassTypeV2.Fighter, SkillName.CombatMastery },
            { PlayerClassTypeV2.Templar, SkillName.Zealotry },
            { PlayerClassTypeV2.BountyHunter, SkillName.Manhunting },
            { PlayerClassTypeV2.Beastmaster, SkillName.BeastBonding },
            { PlayerClassTypeV2.Artificer, SkillName.Engineering },
            { PlayerClassTypeV2.Alchemist, SkillName.Transmutation },
            { PlayerClassTypeV2.Cleric, SkillName.DivineGrace },
            { PlayerClassTypeV2.Wizard, SkillName.ArcaneStudies },
        };

        /// <summary>
        /// Gets the class skill for a player based on their current class.
        /// </summary>
        public static SkillName? GetClassSkill(PlayerMobile pm)
        {
            if (pm == null)
                return null;

            var playerClass = VystiaClassManager.Instance?.GetClass(pm);
            if (playerClass == null)
                return null;

            if (ClassSkillMap.TryGetValue(playerClass.ClassType, out SkillName skill))
                return skill;

            return null;
        }

        /// <summary>
        /// Gets the class skill for a specific class type.
        /// </summary>
        public static SkillName? GetClassSkill(PlayerClassTypeV2 classType)
        {
            if (ClassSkillMap.TryGetValue(classType, out SkillName skill))
                return skill;

            return null;
        }

        /// <summary>
        /// Checks for skill gain when using a class ability.
        /// Uses standard UO skill check mechanics with difficulty scaling.
        /// </summary>
        /// <param name="from">The mobile using the ability</param>
        /// <param name="difficulty">Difficulty of the action (0.0 - 100.0)</param>
        /// <returns>True if the skill check succeeded</returns>
        public static bool CheckClassSkill(Mobile from, double difficulty)
        {
            if (!(from is PlayerMobile pm))
                return false;

            var classSkill = GetClassSkill(pm);
            if (classSkill == null)
                return false;

            // Use standard UO skill check with difficulty range
            double minSkill = Math.Max(0, difficulty - 25.0);
            double maxSkill = Math.Min(100.0, difficulty + 25.0);

            return from.CheckSkill(classSkill.Value, minSkill, maxSkill);
        }

        /// <summary>
        /// Triggers a skill gain check for the player's class skill.
        /// Called when abilities succeed to give a chance at skill gain.
        /// </summary>
        /// <param name="from">The mobile</param>
        /// <param name="difficulty">Difficulty that affects gain chance</param>
        public static void TriggerGainCheck(Mobile from, double difficulty)
        {
            if (!(from is PlayerMobile pm))
                return;

            var classSkill = GetClassSkill(pm);
            if (classSkill == null)
                return;

            Skill skill = from.Skills[classSkill.Value];
            if (skill == null)
                return;

            // Don't try to gain if locked
            if (skill.Lock != SkillLock.Up)
                return;

            // Don't try to gain if at cap
            if (skill.Base >= skill.Cap)
                return;

            // Gain chance decreases as skill approaches cap
            // Base 50% chance at 0 skill, decreasing linearly
            double gc = (skill.Cap - skill.Base) / skill.Cap;
            gc *= 0.5;

            // Difficulty modifier: harder actions = better gains
            gc *= (1.0 + (difficulty / 200.0));

            // Cap gain chance
            if (gc < 0.01)
                gc = 0.01;
            if (gc > 0.5)
                gc = 0.5;

            if (Utility.RandomDouble() < gc)
            {
                skill.Base += 0.1;
                from.SendMessage(0x3B2, "Your {0} skill has increased!", skill.Info.Name);
            }
        }

        /// <summary>
        /// Called when a creature dies to potentially grant skill gains.
        /// Difficulty is based on creature power.
        /// </summary>
        public static void OnCreatureDeath(Mobile killer, Mobile killed)
        {
            if (!(killer is PlayerMobile pm))
                return;

            var classSkill = GetClassSkill(pm);
            if (classSkill == null)
                return;

            // Calculate difficulty based on creature power
            double difficulty = GetCreatureDifficulty(killed);

            // Perform skill check
            CheckClassSkill(pm, difficulty);
        }

        /// <summary>
        /// Calculates difficulty based on creature stats.
        /// Returns a value from 0-100 suitable for skill checks (Vystia: 100 is GM, no power scrolls).
        /// </summary>
        public static double GetCreatureDifficulty(Mobile creature)
        {
            if (creature == null)
                return 0;

            // Base difficulty on HP, fame, and skills
            double power = creature.HitsMax;
            power += creature.Fame / 100.0;
            power += creature.Skills.Total / 100.0;

            // Scale to 0-100 range (Vystia: 100 is GM, no power scrolls)
            double difficulty = Math.Min(100.0, power / 10.0);

            return difficulty;
        }

        /// <summary>
        /// Called when spending a secondary resource (like Soul Shards, Chi, etc.)
        /// to trigger a skill gain check.
        /// </summary>
        /// <param name="from">The mobile spending the resource</param>
        /// <param name="resourceType">The type of resource spent</param>
        /// <param name="amount">Amount of resource spent</param>
        public static void OnResourceSpent(Mobile from, ResourceType resourceType, int amount)
        {
            if (!(from is PlayerMobile pm))
                return;

            var classSkill = GetClassSkill(pm);
            if (classSkill == null)
                return;

            // Difficulty scales with amount spent
            double difficulty = 25.0 + (amount * 5.0);
            difficulty = Math.Min(100.0, difficulty);

            // 25% chance to trigger a gain check on resource spend
            if (Utility.RandomDouble() < 0.25)
            {
                TriggerGainCheck(pm, difficulty);
            }
        }

        /// <summary>
        /// Called when casting a class spell to trigger skill gain.
        /// </summary>
        /// <param name="from">The caster</param>
        /// <param name="spellCircle">The spell circle (1-8)</param>
        public static void OnSpellCast(Mobile from, int spellCircle)
        {
            if (!(from is PlayerMobile pm))
                return;

            // Difficulty based on spell circle (12.5 per circle)
            double difficulty = spellCircle * 12.5;

            // Perform skill check - this uses standard UO mechanics
            CheckClassSkill(pm, difficulty);
        }

        /// <summary>
        /// Called when entering a stance to potentially trigger skill gain.
        /// </summary>
        public static void OnStanceActivated(Mobile from, string stanceName)
        {
            if (!(from is PlayerMobile pm))
                return;

            // Base difficulty for stance activation
            double difficulty = 50.0;

            // 10% chance to trigger gain on stance activation
            if (Utility.RandomDouble() < 0.10)
            {
                TriggerGainCheck(pm, difficulty);
            }
        }

        /// <summary>
        /// Called when applying a buff/debuff to trigger skill gain.
        /// </summary>
        public static void OnBuffApplied(Mobile from, Mobile target, bool isDebuff)
        {
            if (!(from is PlayerMobile pm))
                return;

            // Higher difficulty for debuffs on enemies
            double difficulty = isDebuff ? 60.0 : 40.0;

            // Adjust for target power if it's a creature
            if (target != null && !(target is PlayerMobile))
            {
                difficulty += GetCreatureDifficulty(target) * 0.25;
            }

            // Perform skill check
            CheckClassSkill(pm, Math.Min(100.0, difficulty));
        }

        /// <summary>
        /// Called when dealing damage with a class ability.
        /// </summary>
        public static void OnAbilityDamage(Mobile from, Mobile target, int damage)
        {
            if (!(from is PlayerMobile pm))
                return;

            // Difficulty based on damage dealt and target
            double difficulty = 30.0 + (damage / 10.0);

            if (target != null && !(target is PlayerMobile))
            {
                difficulty += GetCreatureDifficulty(target) * 0.1;
            }

            // 20% chance to trigger gain check on damage
            if (Utility.RandomDouble() < 0.20)
            {
                TriggerGainCheck(pm, Math.Min(100.0, difficulty));
            }
        }

        /// <summary>
        /// Sets a player's class skill to a specific value (GM command).
        /// </summary>
        public static bool SetClassSkill(PlayerMobile pm, double value)
        {
            var classSkill = GetClassSkill(pm);
            if (classSkill == null)
                return false;

            Skill skill = pm.Skills[classSkill.Value];
            if (skill == null)
                return false;

            skill.Base = Math.Max(0, Math.Min(skill.Cap, value));
            return true;
        }

        /// <summary>
        /// Gets the current value of a player's class skill.
        /// </summary>
        public static double GetClassSkillValue(PlayerMobile pm)
        {
            var classSkill = GetClassSkill(pm);
            if (classSkill == null)
                return 0;

            Skill skill = pm.Skills[classSkill.Value];
            if (skill == null)
                return 0;

            return skill.Value;
        }

        /// <summary>
        /// Gets the base value of a player's class skill.
        /// </summary>
        public static double GetClassSkillBase(PlayerMobile pm)
        {
            var classSkill = GetClassSkill(pm);
            if (classSkill == null)
                return 0;

            Skill skill = pm.Skills[classSkill.Value];
            if (skill == null)
                return 0;

            return skill.Base;
        }
    }
}
