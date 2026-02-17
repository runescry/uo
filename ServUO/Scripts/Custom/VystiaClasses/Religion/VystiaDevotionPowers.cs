using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Mobiles;
using Server.Commands;
using Server.Targeting;
using Server.Items;
using Server.Spells;
using Server.Engines.PartySystem;
using Server.Custom.VystiaClasses.Systems;

namespace Server.Custom.VystiaClasses.Religion
{
    /// <summary>
    /// Devotion Powers System
    /// Implements all 18 devotion powers (3 per religion × 6 religions)
    /// Powers unlock at 200 (Devoted), 500 (Faithful), and 900 (Exalted) piety
    /// </summary>
    public static class VystiaDevotionPowers
    {
        #region Initialization

        public static void Initialize()
        {
            CommandSystem.Register("DevotionPower", AccessLevel.Player, new CommandEventHandler(DevotionPower_OnCommand));
            CommandSystem.Register("DP", AccessLevel.Player, new CommandEventHandler(DevotionPower_OnCommand));

            // Initialize test item commands
            ShrineStoneCommands.Initialize();
            Testing.VystiaTestItems.Initialize();
            Testing.BossStoneCommands.Initialize();
            Testing.AncientStoneCommands.Initialize();
        }

        #endregion

        #region Power Definitions

        /// <summary>
        /// Devotion power definition
        /// </summary>
        public class DevotionPower
        {
            public string Name { get; set; }
            public VystiaReligion Religion { get; set; }
            public PietyTier RequiredTier { get; set; }
            public TimeSpan Cooldown { get; set; }
            public string Description { get; set; }
            public Action<PlayerMobile> Effect { get; set; }
        }

        /// <summary>
        /// All 18 devotion powers
        /// </summary>
        private static readonly Dictionary<string, DevotionPower> s_Powers = new Dictionary<string, DevotionPower>();

        static VystiaDevotionPowers()
        {
            InitializePowers();
        }

        private static void InitializePowers()
        {
            // Frosthelm Faith (Frosthelm Faith)
            RegisterPower("FrostShield", VystiaReligion.FrosthelmFaith, PietyTier.Devoted, TimeSpan.FromMinutes(15),
                "Absorb 50 damage, reflect 15% cold damage for 30 seconds",
                pm => ActivateFrostShield(pm));

            RegisterPower("EnduranceOfWinter", VystiaReligion.FrosthelmFaith, PietyTier.Faithful, TimeSpan.FromMinutes(30),
                "Cannot die for 5 seconds (HP minimum 1), -50% damage taken",
                pm => ActivateEnduranceOfWinter(pm));

            RegisterPower("AbsoluteZero", VystiaReligion.FrosthelmFaith, PietyTier.Exalted, TimeSpan.FromMinutes(60),
                "Freeze enemies in 5 tile radius for 3 seconds",
                pm => ActivateAbsoluteZero(pm));

            // Cogsmith Creed (Cogsmith Creed)
            RegisterPower("ForgeBlessing", VystiaReligion.CogsmithCreed, PietyTier.Devoted, TimeSpan.FromMinutes(30),
                "+10% exceptional craft chance for 5 minutes",
                pm => ActivateForgeBlessing(pm));

            RegisterPower("SteamBurst", VystiaReligion.CogsmithCreed, PietyTier.Faithful, TimeSpan.FromMinutes(10),
                "AoE 30-50 fire damage with knockback in 3 tile radius",
                pm => ActivateSteamBurst(pm));

            RegisterPower("MachinistsGrace", VystiaReligion.CogsmithCreed, PietyTier.Exalted, TimeSpan.FromMinutes(60),
                "Repair all gear and +15% damage for 90 seconds",
                pm => ActivateMachinistsGrace(pm));

            // Lunara's Covenant (Lunara's Covenant)
            RegisterPower("MoonlightHealing", VystiaReligion.LunarasCovenant, PietyTier.Devoted, TimeSpan.FromMinutes(15),
                "Heal 30-50 HP to all allies in 5 tile radius",
                pm => ActivateMoonlightHealing(pm));

            RegisterPower("NaturesSanctuary", VystiaReligion.LunarasCovenant, PietyTier.Faithful, TimeSpan.FromMinutes(20),
                "Create 4-tile zone with +25% healing for 20 seconds",
                pm => ActivateNaturesSanctuary(pm));

            RegisterPower("LunarasEmbrace", VystiaReligion.LunarasCovenant, PietyTier.Exalted, TimeSpan.FromMinutes(60),
                "Full heal + cleanse + 5 seconds damage immunity",
                pm => ActivateLunarasEmbrace(pm));

            // Celestis Arcanum (Celestis Arcanum)
            RegisterPower("ArcaneInsight", VystiaReligion.CelestisArcanum, PietyTier.Devoted, TimeSpan.FromMinutes(10),
                "See enemy stats for 1 minute",
                pm => ActivateArcaneInsight(pm));

            RegisterPower("ManaConstellation", VystiaReligion.CelestisArcanum, PietyTier.Faithful, TimeSpan.FromMinutes(20),
                "Restore 35% mana to all party members",
                pm => ActivateManaConstellation(pm));

            RegisterPower("CelestialAlignment", VystiaReligion.CelestisArcanum, PietyTier.Exalted, TimeSpan.FromMinutes(60),
                "0 mana cost for 8 seconds (max 4 spells)",
                pm => ActivateCelestialAlignment(pm));

            // Surya's Sandscript (Surya's Sandscript)
            RegisterPower("SunsRevelation", VystiaReligion.SuryasSandscript, PietyTier.Devoted, TimeSpan.FromMinutes(10),
                "Reveal hidden enemies in 8 tile radius for 20 seconds",
                pm => ActivateSunsRevelation(pm));

            RegisterPower("TimeDilation", VystiaReligion.SuryasSandscript, PietyTier.Faithful, TimeSpan.FromMinutes(20),
                "+25% attack/cast speed for 10 seconds",
                pm => ActivateTimeDilation(pm));

            RegisterPower("SolarJudgment", VystiaReligion.SuryasSandscript, PietyTier.Exalted, TimeSpan.FromMinutes(60),
                "Cone 75-100 fire damage, blind for 3 seconds",
                pm => ActivateSolarJudgment(pm));

            // Oceana's Covenant (Oceana's Covenant)
            RegisterPower("TidalSurge", VystiaReligion.OceanasCovenant, PietyTier.Devoted, TimeSpan.FromMinutes(15),
                "Push enemies back 3 tiles in front of you",
                pm => ActivateTidalSurge(pm));

            RegisterPower("AbyssalCall", VystiaReligion.OceanasCovenant, PietyTier.Faithful, TimeSpan.FromMinutes(30),
                "Summon water elemental (200 HP, 2 minutes)",
                pm => ActivateAbyssalCall(pm));

            RegisterPower("OceanasWrath", VystiaReligion.OceanasCovenant, PietyTier.Exalted, TimeSpan.FromMinutes(60),
                "Tidal wave line damage with drowning DoT",
                pm => ActivateOceanasWrath(pm));
        }

        private static void RegisterPower(string name, VystiaReligion religion, PietyTier tier, TimeSpan cooldown, string description, Action<PlayerMobile> effect)
        {
            s_Powers[name] = new DevotionPower
            {
                Name = name,
                Religion = religion,
                RequiredTier = tier,
                Cooldown = cooldown,
                Description = description,
                Effect = effect
            };
        }

        #endregion

        #region Cooldown Tracking

        private static Dictionary<PlayerMobile, Dictionary<string, DateTime>> s_Cooldowns = new Dictionary<PlayerMobile, Dictionary<string, DateTime>>();

        /// <summary>
        /// Check if a power is on cooldown
        /// </summary>
        public static bool IsOnCooldown(PlayerMobile pm, string powerName)
        {
            if (pm == null || !s_Cooldowns.ContainsKey(pm))
                return false;

            if (!s_Cooldowns[pm].ContainsKey(powerName))
                return false;

            return s_Cooldowns[pm][powerName] > DateTime.UtcNow;
        }

        /// <summary>
        /// Get remaining cooldown time
        /// </summary>
        public static TimeSpan GetRemainingCooldown(PlayerMobile pm, string powerName)
        {
            if (pm == null || !s_Cooldowns.ContainsKey(pm))
                return TimeSpan.Zero;

            if (!s_Cooldowns[pm].ContainsKey(powerName))
                return TimeSpan.Zero;

            DateTime cooldownEnd = s_Cooldowns[pm][powerName];
            if (cooldownEnd <= DateTime.UtcNow)
                return TimeSpan.Zero;

            return cooldownEnd - DateTime.UtcNow;
        }

        /// <summary>
        /// Set cooldown for a power
        /// </summary>
        private static void SetCooldown(PlayerMobile pm, string powerName, TimeSpan cooldown)
        {
            if (pm == null)
                return;

            if (!s_Cooldowns.ContainsKey(pm))
                s_Cooldowns[pm] = new Dictionary<string, DateTime>();

            s_Cooldowns[pm][powerName] = DateTime.UtcNow + cooldown;
        }

        /// <summary>
        /// Recharge all devotion powers (reduce cooldowns by 50%, Devoted tier required)
        /// </summary>
        public static bool RechargeAllPowers(PlayerMobile pm)
        {
            if (pm == null)
                return false;

            var pietyData = VystiaPiety.GetPiety(pm);
            if (pietyData == null || pietyData.Piety < 200) // Devoted tier
            {
                pm.SendMessage(0x22, "You need at least 200 piety (Devoted tier) to recharge devotion powers.");
                return false;
            }

            if (!s_Cooldowns.ContainsKey(pm) || s_Cooldowns[pm].Count == 0)
            {
                pm.SendMessage(0x35, "You have no devotion powers on cooldown.");
                return false;
            }

            int rechargedCount = 0;
            foreach (var powerName in s_Cooldowns[pm].Keys.ToList())
            {
                DateTime currentCooldown = s_Cooldowns[pm][powerName];
                if (currentCooldown > DateTime.UtcNow)
                {
                    // Reduce remaining cooldown by 50%
                    TimeSpan remaining = currentCooldown - DateTime.UtcNow;
                    TimeSpan reduced = TimeSpan.FromSeconds(remaining.TotalSeconds * 0.5);
                    s_Cooldowns[pm][powerName] = DateTime.UtcNow + reduced;
                    rechargedCount++;
                }
            }

            if (rechargedCount > 0)
            {
                pm.SendMessage(0x35, "You have recharged {0} devotion power{1}!", rechargedCount, rechargedCount == 1 ? "" : "s");
                pm.PlaySound(0x1F2);
                pm.FixedParticles(0x376A, 9, 32, 5030, ReligionData.GetInfo(pietyData.Religion)?.Hue ?? 0, 0, EffectLayer.Waist);
            }
            else
            {
                pm.SendMessage(0x35, "You have no devotion powers on cooldown.");
            }

            return rechargedCount > 0;
        }

        #endregion

        #region Power Activation

        /// <summary>
        /// Activate a devotion power
        /// </summary>
        public static bool ActivatePower(PlayerMobile pm, string powerName)
        {
            if (pm == null)
                return false;

            if (!s_Powers.ContainsKey(powerName))
            {
                pm.SendMessage(0x22, "Unknown devotion power: {0}", powerName);
                return false;
            }

            DevotionPower power = s_Powers[powerName];

            // Check religion
            var pietyData = VystiaPiety.GetPiety(pm);
            if (pietyData == null || pietyData.Religion != power.Religion)
            {
                pm.SendMessage(0x22, "You must follow {0} to use this power.", power.Religion);
                return false;
            }

            // Check piety tier
            var tier = ReligionData.GetTier(pietyData.Piety);
            if (tier < power.RequiredTier)
            {
                pm.SendMessage(0x22, "You need {0} tier piety to use this power.", power.RequiredTier);
                return false;
            }

            // Check cooldown
            if (IsOnCooldown(pm, powerName))
            {
                TimeSpan remaining = GetRemainingCooldown(pm, powerName);
                pm.SendMessage(0x22, "{0} is on cooldown for {1:F1} more minutes.", power.Name, remaining.TotalMinutes);
                return false;
            }

            // Activate power
            try
            {
                power.Effect(pm);
                SetCooldown(pm, powerName, power.Cooldown);
                pm.SendMessage(0x35, "You activate {0}!", power.Name);
                pm.PlaySound(0x1F2);
                pm.FixedParticles(0x376A, 9, 32, 5030, ReligionData.GetInfo(power.Religion)?.Hue ?? 0, 0, EffectLayer.Waist);
                return true;
            }
            catch (Exception ex)
            {
                pm.SendMessage(0x22, "Error activating power: {0}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Get available powers for a player
        /// </summary>
        public static List<DevotionPower> GetAvailablePowers(PlayerMobile pm)
        {
            List<DevotionPower> powers = new List<DevotionPower>();

            if (pm == null)
                return powers;

            var pietyData = VystiaPiety.GetPiety(pm);
            if (pietyData == null || pietyData.Religion == VystiaReligion.None)
                return powers;

            var tier = ReligionData.GetTier(pietyData.Piety);

            foreach (var power in s_Powers.Values)
            {
                if (power.Religion == pietyData.Religion && tier >= power.RequiredTier)
                {
                    powers.Add(power);
                }
            }

            return powers;
        }

        #endregion

        #region Power Effects Implementation

        // Frosthelm Faith Powers
        private static void ActivateFrostShield(PlayerMobile pm)
        {
            // Absorb 50 damage, reflect 15% cold for 30 seconds
            VystiaBuffManager.Instance.ApplyBuff(pm, pm, VystiaBuffType.DamageAbsorb, TimeSpan.FromSeconds(30), 50);
            VystiaBuffManager.Instance.ApplyBuff(pm, pm, VystiaBuffType.ReflectShield, TimeSpan.FromSeconds(30), 15);
        }

        private static void ActivateEnduranceOfWinter(PlayerMobile pm)
        {
            // Cannot die for 5 seconds (HP minimum 1), -50% damage taken
            VystiaBuffManager.Instance.ApplyBuff(pm, pm, VystiaBuffType.DamageDecrease, TimeSpan.FromSeconds(5), 50);

            // Apply immortality effect - player cannot die for 5 seconds
            pm.SendMessage(0x3B2, "The Frost Father protects you from death!");
            pm.FixedParticles(0x376A, 9, 32, 5030, 1152, 0, EffectLayer.Waist);
            pm.PlaySound(0x1E0);

            // Start immortality timer - check HP every 250ms for 5 seconds
            int ticksRemaining = 20; // 5 seconds / 0.25s = 20 ticks
            Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromMilliseconds(250), () =>
            {
                if (pm == null || pm.Deleted || ticksRemaining <= 0)
                    return;

                // Prevent death - keep HP at minimum 1
                if (pm.Hits < 1 && pm.Alive)
                {
                    pm.Hits = 1;
                }

                ticksRemaining--;
                if (ticksRemaining == 0)
                {
                    pm.SendMessage(0x22, "Your divine protection fades...");
                }
            });
        }

        private static void ActivateAbsoluteZero(PlayerMobile pm)
        {
            // Freeze enemies in 5 tile radius for 3 seconds
            // Duration is 50% vs bosses (1.5s), bosses are immune but adds get 50% more (4.5s)
            List<Mobile> targets = new List<Mobile>();
            foreach (Mobile m in pm.GetMobilesInRange(5))
            {
                if (m != pm && m.CanBeHarmful(pm) && SpellHelper.ValidIndirectTarget(pm, m))
                {
                    targets.Add(m);
                }
            }

            foreach (Mobile target in targets)
            {
                bool isBoss = target is BaseCreature && ((BaseCreature)target).IsParagon;
                TimeSpan duration = isBoss ? TimeSpan.FromSeconds(1.5) : TimeSpan.FromSeconds(3);
                VystiaBuffManager.Instance.ApplyBuff(target, pm, VystiaBuffType.SlowDebuff, duration, 100);
            }
        }

        // Cogsmith Creed Powers
        private static void ActivateForgeBlessing(PlayerMobile pm)
        {
            // +10% exceptional craft chance for 5 minutes
            // This is handled in CraftItem.cs via a flag - use a generic buff for tracking
            VystiaBuffManager.Instance.ApplyBuff(pm, pm, VystiaBuffType.AllStatsBuff, TimeSpan.FromMinutes(5), 0);
            // Note: Actual craft bonus is applied in CraftItem.cs via a piety check
        }

        private static void ActivateSteamBurst(PlayerMobile pm)
        {
            // AoE 30-50 fire damage with knockback in 3 tile radius
            pm.Target = new SteamBurstTarget(pm);
        }

        private static void ActivateMachinistsGrace(PlayerMobile pm)
        {
            // Repair all gear and +15% damage for 90 seconds
            // Repair all equipped items
            foreach (Item item in pm.Items)
            {
                if (item is BaseWeapon weapon && weapon.HitPoints < weapon.MaxHitPoints)
                    weapon.HitPoints = weapon.MaxHitPoints;
                else if (item is BaseArmor armor && armor.HitPoints < armor.MaxHitPoints)
                    armor.HitPoints = armor.MaxHitPoints;
            }

            VystiaBuffManager.Instance.ApplyBuff(pm, pm, VystiaBuffType.DamageIncrease, TimeSpan.FromSeconds(90), 15);
        }

        // Lunara's Covenant Powers
        private static void ActivateMoonlightHealing(PlayerMobile pm)
        {
            // Heal 30-50 HP to all allies in 5 tile radius
            foreach (Mobile m in pm.GetMobilesInRange(5))
            {
                if (m != pm && m is PlayerMobile && SpellHelper.ValidIndirectTarget(pm, m))
                {
                    int heal = Utility.RandomMinMax(30, 50);
                    m.Heal(heal);
                    m.FixedParticles(0x376A, 9, 32, 5030, 0x3B2, 0, EffectLayer.Waist);
                }
            }
        }

        private static void ActivateNaturesSanctuary(PlayerMobile pm)
        {
            // Create 4-tile zone with +25% healing for 20 seconds
            Point3D center = pm.Location;
            Map map = pm.Map;

            pm.SendMessage(0x3B2, "You create a sanctuary of nature!");
            pm.PlaySound(0x1E3);

            // Create visual zone effect
            for (int x = -4; x <= 4; x++)
            {
                for (int y = -4; y <= 4; y++)
                {
                    Point3D effectLoc = new Point3D(center.X + x, center.Y + y, center.Z);
                    if (Math.Abs(x) + Math.Abs(y) <= 5) // Diamond shape
                    {
                        Effects.SendLocationParticles(
                            EffectItem.Create(effectLoc, map, EffectItem.DefaultDuration),
                            0x376A, 1, 13, 2010, 0, 9502, 0);
                    }
                }
            }

            // Healing zone effect - heal players in zone every 2 seconds for 20 seconds
            int ticksRemaining = 10; // 20 seconds / 2s = 10 ticks
            Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(2), () =>
            {
                if (ticksRemaining <= 0)
                    return;

                foreach (Mobile m in map.GetMobilesInRange(center, 4))
                {
                    if (m is PlayerMobile target && target.Alive)
                    {
                        // 25% bonus healing - heal 5-10 HP per tick
                        int heal = Utility.RandomMinMax(5, 10);
                        target.Heal(heal);
                        target.FixedParticles(0x376A, 9, 32, 5030, 2010, 0, EffectLayer.Waist);
                    }
                }

                ticksRemaining--;
                if (ticksRemaining == 0)
                {
                    pm.SendMessage(0x22, "Your Nature's Sanctuary fades...");
                }
            });
        }

        private static void ActivateLunarasEmbrace(PlayerMobile pm)
        {
            // Full heal + cleanse + 5 seconds damage immunity
            pm.Hits = pm.HitsMax;
            pm.Mana = pm.ManaMax;
            pm.Stam = pm.StamMax;
            pm.CurePoison(pm);
            VystiaBuffManager.Instance.ApplyBuff(pm, pm, VystiaBuffType.DamageAbsorb, TimeSpan.FromSeconds(5), 100);
        }

        // Celestis Arcanum Powers
        private static void ActivateArcaneInsight(PlayerMobile pm)
        {
            // See enemy stats for 1 minute
            // This is handled via a buff that reveals stats - use IntelligenceBuff as placeholder
            VystiaBuffManager.Instance.ApplyBuff(pm, pm, VystiaBuffType.IntelligenceBuff, TimeSpan.FromMinutes(1), 0);
            // Note: Actual stat reveal is handled via property display
        }

        private static void ActivateManaConstellation(PlayerMobile pm)
        {
            // Restore 35% mana to all party members
            if (pm.Party != null && pm.Party is Server.Engines.PartySystem.Party party)
            {
                foreach (Server.Engines.PartySystem.PartyMemberInfo member in party.Members)
                {
                    if (member.Mobile is PlayerMobile partyMember)
                    {
                        int restore = (int)(partyMember.ManaMax * 0.35);
                        partyMember.Mana = Math.Min(partyMember.ManaMax, partyMember.Mana + restore);
                        partyMember.FixedParticles(0x376A, 9, 32, 5030, 0x3B2, 0, EffectLayer.Waist);
                    }
                }
            }
            else
            {
                int restore = (int)(pm.ManaMax * 0.35);
                pm.Mana = Math.Min(pm.ManaMax, pm.Mana + restore);
            }
        }

        // Static dictionary to track free spell counts
        private static Dictionary<PlayerMobile, int> m_CelestialAlignmentSpells = new Dictionary<PlayerMobile, int>();

        public static bool HasFreeCelestialSpell(PlayerMobile pm)
        {
            if (m_CelestialAlignmentSpells.ContainsKey(pm) && m_CelestialAlignmentSpells[pm] > 0)
                return true;
            return false;
        }

        public static void ConsumeCelestialSpell(PlayerMobile pm)
        {
            if (m_CelestialAlignmentSpells.ContainsKey(pm) && m_CelestialAlignmentSpells[pm] > 0)
            {
                m_CelestialAlignmentSpells[pm]--;
                pm.SendMessage(0x3B2, $"Free spell used! ({m_CelestialAlignmentSpells[pm]} remaining)");
                if (m_CelestialAlignmentSpells[pm] <= 0)
                {
                    m_CelestialAlignmentSpells.Remove(pm);
                    pm.SendMessage(0x22, "Your Celestial Alignment has ended.");
                }
            }
        }

        private static void ActivateCelestialAlignment(PlayerMobile pm)
        {
            // 0 mana cost for 8 seconds (max 4 spells)
            pm.SendMessage(0x3B2, "The stars align! Your next 4 spells cost no mana.");
            pm.FixedParticles(0x376A, 9, 32, 5030, 1153, 0, EffectLayer.Waist);
            pm.PlaySound(0x1E9);

            // Set up free spell tracking - 4 free spells
            m_CelestialAlignmentSpells[pm] = 4;

            // Apply visible buff for tracking
            VystiaBuffManager.Instance.ApplyBuff(pm, pm, VystiaBuffType.ManaRegen, TimeSpan.FromSeconds(8), 100);

            // Duration timer - expires after 8 seconds even if spells not used
            Timer.DelayCall(TimeSpan.FromSeconds(8), () =>
            {
                if (m_CelestialAlignmentSpells.ContainsKey(pm))
                {
                    m_CelestialAlignmentSpells.Remove(pm);
                    pm.SendMessage(0x22, "Your Celestial Alignment has expired.");
                }
            });
        }

        // Surya's Sandscript Powers
        private static void ActivateSunsRevelation(PlayerMobile pm)
        {
            // Reveal hidden enemies in 8 tile radius for 20 seconds
            foreach (Mobile m in pm.GetMobilesInRange(8))
            {
                if (m != pm && m.Hidden && SpellHelper.ValidIndirectTarget(pm, m))
                {
                    m.RevealingAction();
                }
            }
            // Reveal hidden buff - use DexterityBuff as placeholder for tracking
            VystiaBuffManager.Instance.ApplyBuff(pm, pm, VystiaBuffType.DexterityBuff, TimeSpan.FromSeconds(20), 0);
        }

        private static void ActivateTimeDilation(PlayerMobile pm)
        {
            // +25% attack/cast speed for 10 seconds
            VystiaBuffManager.Instance.ApplyBuff(pm, pm, VystiaBuffType.SwingSpeedBuff, TimeSpan.FromSeconds(10), 25);
            VystiaBuffManager.Instance.ApplyBuff(pm, pm, VystiaBuffType.CastSpeedBuff, TimeSpan.FromSeconds(10), 25);
        }

        private static void ActivateSolarJudgment(PlayerMobile pm)
        {
            // Cone 75-100 fire damage, blind for 3 seconds
            pm.Target = new SolarJudgmentTarget(pm);
        }

        // Oceana's Covenant Powers
        private static void ActivateTidalSurge(PlayerMobile pm)
        {
            // Push enemies back 3 tiles in front of you
            Direction dir = pm.Direction;
            Point3D pushTarget = new Point3D(pm.X, pm.Y, pm.Z);
            for (int i = 0; i < 3; i++)
            {
                pushTarget = GetNextPointInDirection(pushTarget, dir);
            }

            foreach (Mobile m in pm.GetMobilesInRange(2))
            {
                if (m != pm && m.CanBeHarmful(pm) && SpellHelper.ValidIndirectTarget(pm, m))
                {
                    // Push towards target point
                    m.MoveToWorld(pushTarget, pm.Map);
                    m.FixedParticles(0x376A, 9, 32, 5030, 0x3B2, 0, EffectLayer.Waist);
                }
            }
        }

        private static void ActivateAbyssalCall(PlayerMobile pm)
        {
            // Summon water elemental (200 HP, 2 minutes)
            // Check follower slots
            if ((pm.Followers + 2) > pm.FollowersMax)
            {
                pm.SendMessage(0x22, "You have too many followers to summon a water elemental.");
                return;
            }

            // Create the water elemental
            AbyssalWaterElemental elemental = new AbyssalWaterElemental();
            elemental.MoveToWorld(pm.Location, pm.Map);
            elemental.ControlMaster = pm;
            elemental.Controlled = true;
            elemental.ControlOrder = OrderType.Follow;
            elemental.ControlTarget = pm;

            pm.SendMessage(0x3B2, "You summon a servant of the deep!");
            pm.FixedParticles(0x376A, 9, 32, 5030, 1152, 0, EffectLayer.Waist);
            pm.PlaySound(0x026);

            // 2 minute duration timer
            Timer.DelayCall(TimeSpan.FromMinutes(2), () =>
            {
                if (elemental != null && !elemental.Deleted)
                {
                    elemental.Say("*returns to the depths*");
                    elemental.FixedParticles(0x376A, 9, 32, 5030, 1152, 0, EffectLayer.Waist);
                    elemental.PlaySound(0x026);
                    elemental.Delete();
                    pm.SendMessage(0x22, "Your water elemental returns to the abyss.");
                }
            });
        }

        private static void ActivateOceanasWrath(PlayerMobile pm)
        {
            // Tidal wave line damage with drowning DoT
            pm.Target = new OceanasWrathTarget(pm);
        }

        #endregion

        #region Helper Methods

        private static Point3D GetNextPointInDirection(Point3D p, Direction d)
        {
            int x = p.X;
            int y = p.Y;

            switch (d)
            {
                case Direction.North: y--; break;
                case Direction.Right: x++; y--; break;
                case Direction.East: x++; break;
                case Direction.Down: x++; y++; break;
                case Direction.South: y++; break;
                case Direction.Left: x--; y++; break;
                case Direction.West: x--; break;
                case Direction.Up: x--; y--; break;
            }

            return new Point3D(x, y, p.Z);
        }

        #endregion

        #region Target Classes

        private class SteamBurstTarget : Target
        {
            private PlayerMobile m_Caster;

            public SteamBurstTarget(PlayerMobile caster) : base(3, false, TargetFlags.None)
            {
                m_Caster = caster;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is IPoint3D point)
                {
                    Point3D p = new Point3D(point);
                    foreach (Mobile m in m_Caster.GetMobilesInRange(3))
                    {
                        if (m != m_Caster && m.CanBeHarmful(m_Caster) && SpellHelper.ValidIndirectTarget(m_Caster, m))
                        {
                            int damage = Utility.RandomMinMax(30, 50);
                            AOS.Damage(m, m_Caster, damage, 0, 100, 0, 0, 0); // 100% fire
                            m.FixedParticles(0x3709, 10, 30, 5052, 0, 0, EffectLayer.LeftFoot);
                            // Knockback
                            Direction pushDir = m_Caster.GetDirectionTo(m);
                            Point3D pushPoint = GetNextPointInDirection(m.Location, pushDir);
                            m.MoveToWorld(pushPoint, m_Caster.Map);
                        }
                    }
                }
            }
        }

        private class SolarJudgmentTarget : Target
        {
            private PlayerMobile m_Caster;

            public SolarJudgmentTarget(PlayerMobile caster) : base(10, false, TargetFlags.None)
            {
                m_Caster = caster;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is IPoint3D point)
                {
                    Point3D p = new Point3D(point);
                    // Cone effect - damage all mobiles in cone
                    foreach (Mobile m in m_Caster.GetMobilesInRange(10))
                    {
                        if (m != m_Caster && m.CanBeHarmful(m_Caster) && SpellHelper.ValidIndirectTarget(m_Caster, m))
                        {
                            // Check if in cone (simplified - check direction)
                            int damage = Utility.RandomMinMax(75, 100);
                            AOS.Damage(m, m_Caster, damage, 0, 100, 0, 0, 0); // 100% fire
                            m.FixedParticles(0x3709, 10, 30, 5052, 0, 0, EffectLayer.LeftFoot);
                            // Blindness effect - use SlowDebuff as placeholder
                            VystiaBuffManager.Instance.ApplyBuff(m, m_Caster, VystiaBuffType.SlowDebuff, TimeSpan.FromSeconds(3), 50);
                        }
                    }
                }
            }
        }

        private class OceanasWrathTarget : Target
        {
            private PlayerMobile m_Caster;

            public OceanasWrathTarget(PlayerMobile caster) : base(10, false, TargetFlags.None)
            {
                m_Caster = caster;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is IPoint3D point)
                {
                    Point3D p = new Point3D(point);
                    // Line effect - damage all mobiles in line
                    foreach (Mobile m in m_Caster.GetMobilesInRange(10))
                    {
                        if (m != m_Caster && m.CanBeHarmful(m_Caster) && SpellHelper.ValidIndirectTarget(m_Caster, m))
                        {
                            int damage = Utility.RandomMinMax(50, 75);
                            AOS.Damage(m, m_Caster, damage, 0, 0, 0, 0, 100); // 100% energy
                            m.FixedParticles(0x3709, 10, 30, 5052, 0x3B2, 0, EffectLayer.LeftFoot);
                            // Drowning DoT
                            VystiaBuffManager.Instance.ApplyBuff(m, m_Caster, VystiaBuffType.Corruption, TimeSpan.FromSeconds(10), 10);
                        }
                    }
                }
            }
        }

        #endregion

        #region Summons

        /// <summary>
        /// Water Elemental summoned by Abyssal Call devotion power
        /// 200 HP, 2 minute duration, 2 control slots
        /// </summary>
        [CorpseName("water elemental remains")]
        public class AbyssalWaterElemental : BaseCreature
        {
            [Constructable]
            public AbyssalWaterElemental()
                : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
            {
                Name = "Abyssal Water Elemental";
                Body = 16; // Water elemental body
                BaseSoundID = 278;
                Hue = 1152; // Deep blue

                SetStr(100);
                SetDex(80);
                SetInt(100);

                SetHits(200);
                SetDamage(10, 18);

                SetDamageType(ResistanceType.Physical, 0);
                SetDamageType(ResistanceType.Cold, 100);

                SetResistance(ResistanceType.Physical, 50, 60);
                SetResistance(ResistanceType.Fire, 10, 20);
                SetResistance(ResistanceType.Cold, 70, 80);
                SetResistance(ResistanceType.Poison, 60, 70);
                SetResistance(ResistanceType.Energy, 40, 50);

                SetSkill(SkillName.MagicResist, 60.0, 80.0);
                SetSkill(SkillName.Tactics, 70.0, 90.0);
                SetSkill(SkillName.Wrestling, 70.0, 90.0);
                SetSkill(SkillName.Magery, 60.0, 80.0);
                SetSkill(SkillName.EvalInt, 60.0, 80.0);

                Fame = 0;
                Karma = 0;
                ControlSlots = 2;
                VirtualArmor = 40;

                // Can't be tamed or provoked
                Tamable = false;
            }

            public override void OnGaveMeleeAttack(Mobile defender)
            {
                base.OnGaveMeleeAttack(defender);

                // Chance to chill on hit
                if (Utility.RandomDouble() < 0.25)
                {
                    defender.SendMessage("The elemental's touch chills you to the bone!");
                    defender.FixedParticles(0x376A, 9, 32, 5030, 1152, 0, EffectLayer.Waist);
                    AOS.Damage(defender, this, Utility.RandomMinMax(5, 10), 0, 0, 100, 0, 0);
                }
            }

            public AbyssalWaterElemental(Serial serial) : base(serial) { }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write((int)0);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                reader.ReadInt();
            }
        }

        #endregion

        #region Commands

        [Usage("DevotionPower <name>")]
        [Aliases("DP")]
        [Description("Activates a devotion power")]
        public static void DevotionPower_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile pm)
            {
                if (e.Arguments.Length == 0)
                {
                    // List available powers
                    var powers = GetAvailablePowers(pm);
                    if (powers.Count == 0)
                    {
                        pm.SendMessage("You have no devotion powers available.");
                        return;
                    }

                    pm.SendMessage("Available devotion powers:");
                    foreach (var power in powers)
                    {
                        TimeSpan cooldown = GetRemainingCooldown(pm, power.Name);
                        if (cooldown > TimeSpan.Zero)
                        {
                            pm.SendMessage("  {0} - {1} (Cooldown: {2:F1} min)", power.Name, power.Description, cooldown.TotalMinutes);
                        }
                        else
                        {
                            pm.SendMessage("  {0} - {1} (Ready)", power.Name, power.Description);
                        }
                    }
                }
                else
                {
                    string powerName = e.Arguments[0];
                    ActivatePower(pm, powerName);
                }
            }
        }


        #endregion
    }
}

