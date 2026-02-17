// Vystia Boss Kill Reward System
// Grants reputation and piety rewards when players kill regional bosses

using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Religion;

namespace Server.Custom.VystiaClasses.Factions
{
    #region Boss Reward Configuration

    /// <summary>
    /// Configuration for boss kill rewards
    /// </summary>
    public class BossRewardConfig
    {
        public Type BossType { get; set; }
        public string BossName { get; set; }

        // Faction rewards
        public VystiaFaction PrimaryFaction { get; set; }
        public int FactionReputation { get; set; }

        // Religion rewards
        public VystiaReligion AssociatedReligion { get; set; }
        public int PietyReward { get; set; }

        // Token rewards
        public int TokenReward { get; set; }

        // Gold/XP multipliers
        public double GoldMultiplier { get; set; }
        public double XPMultiplier { get; set; }

        public BossRewardConfig()
        {
            FactionReputation = 100;
            PietyReward = 25;
            TokenReward = 5;
            GoldMultiplier = 1.0;
            XPMultiplier = 1.0;
        }
    }

    #endregion

    #region Boss Reward System

    /// <summary>
    /// System for granting rewards when bosses are killed
    /// </summary>
    public static class VystiaBossRewards
    {
        private static readonly Dictionary<Type, BossRewardConfig> s_BossConfigs = new Dictionary<Type, BossRewardConfig>();

        /// <summary>
        /// Initialize the boss reward system
        /// </summary>
        public static void Initialize()
        {
            // Hook into creature death event
            EventSink.CreatureDeath += OnCreatureDeath;

            Console.WriteLine("[Vystia] Initializing Boss Reward System...");

            // ============================================
            // REGIONAL BOSSES
            // ============================================

            // Frosthold Bosses
            RegisterBoss("VystiaFrostLord", VystiaFaction.Frostguard, VystiaReligion.FrosthelmFaith, 150, 50, 10);
            RegisterBoss("FrozenTitan", VystiaFaction.Frostguard, VystiaReligion.FrosthelmFaith, 200, 75, 15);

            // Emberlands Bosses
            RegisterBoss("VystiaFireLord", VystiaFaction.FlameLegion, VystiaReligion.SuryasSandscript, 150, 50, 10);
            RegisterBoss("MoltenBehemoth", VystiaFaction.FlameLegion, VystiaReligion.SuryasSandscript, 200, 75, 15);

            // Verdantpeak Bosses
            RegisterBoss("VystiaForestGuardian", VystiaFaction.Greenward, VystiaReligion.LunarasCovenant, 150, 50, 10);
            RegisterBoss("AncientTreant", VystiaFaction.Greenward, VystiaReligion.LunarasCovenant, 200, 75, 15);

            // Crystal Barrens Bosses
            RegisterBoss("VystiaCrystalGolem", VystiaFaction.ArcaneConclave, VystiaReligion.CelestisArcanum, 150, 50, 10);
            RegisterBoss("PrismaticEntity", VystiaFaction.ArcaneConclave, VystiaReligion.CelestisArcanum, 200, 75, 15);

            // Ironclad Bosses
            RegisterBoss("VystiaSteamGolem", VystiaFaction.Technoguild, VystiaReligion.CogsmithCreed, 150, 50, 10);
            RegisterBoss("MechanicalTitan", VystiaFaction.Technoguild, VystiaReligion.CogsmithCreed, 200, 75, 15);

            // Desert Bosses
            RegisterBoss("VystiaDesertWyrm", VystiaFaction.Sandwalkers, VystiaReligion.None, 150, 0, 10);
            RegisterBoss("SandstormDjinn", VystiaFaction.Sandwalkers, VystiaReligion.None, 200, 0, 15);

            // ShadowVoid Bosses
            RegisterBoss("VystiaVoidLord", VystiaFaction.Voidborn, VystiaReligion.OceanasCovenant, 150, 50, 10);
            RegisterBoss("AbyssalHorror", VystiaFaction.Voidborn, VystiaReligion.OceanasCovenant, 200, 75, 15);

            // World Bosses (higher rewards)
            RegisterBoss("VystiaWorldBoss", VystiaFaction.None, VystiaReligion.None, 500, 100, 50);

            Console.WriteLine("[Vystia] Boss Reward System initialized with {0} boss configurations.", s_BossConfigs.Count);
        }

        /// <summary>
        /// Register a boss type with rewards
        /// </summary>
        private static void RegisterBoss(string bossTypeName, VystiaFaction faction, VystiaReligion religion, int reputation, int piety, int tokens)
        {
            // Try to find the type by name in the Vystia mobiles namespace
            Type bossType = ScriptCompiler.FindTypeByName(bossTypeName);

            if (bossType == null)
            {
                // Try with full namespace
                bossType = ScriptCompiler.FindTypeByName("Server.Mobiles.Vystia." + bossTypeName);
            }

            if (bossType != null)
            {
                s_BossConfigs[bossType] = new BossRewardConfig
                {
                    BossType = bossType,
                    BossName = bossTypeName,
                    PrimaryFaction = faction,
                    AssociatedReligion = religion,
                    FactionReputation = reputation,
                    PietyReward = piety,
                    TokenReward = tokens
                };
            }
        }

        /// <summary>
        /// Register a boss by type directly
        /// </summary>
        public static void RegisterBoss(Type bossType, VystiaFaction faction, VystiaReligion religion, int reputation, int piety, int tokens)
        {
            if (bossType == null)
                return;

            s_BossConfigs[bossType] = new BossRewardConfig
            {
                BossType = bossType,
                BossName = bossType.Name,
                PrimaryFaction = faction,
                AssociatedReligion = religion,
                FactionReputation = reputation,
                PietyReward = piety,
                TokenReward = tokens
            };
        }

        /// <summary>
        /// Handle creature death event
        /// </summary>
        private static void OnCreatureDeath(CreatureDeathEventArgs e)
        {
            if (e.Creature == null || e.Killer == null)
                return;

            // Check if this is a registered boss
            Type creatureType = e.Creature.GetType();

            if (!s_BossConfigs.TryGetValue(creatureType, out var config))
            {
                // Check if it's a mini-boss by name pattern
                if (e.Creature.Name != null && (e.Creature.Name.Contains("Boss") || e.Creature.Name.Contains("Lord") || e.Creature.Name.Contains("Champion")))
                {
                    GrantMiniBossRewards(e.Killer, e.Creature);
                }
                return;
            }

            // Grant rewards to killer and party
            GrantBossRewards(e.Killer, e.Creature, config);
        }

        /// <summary>
        /// Grant boss kill rewards to killer and party members
        /// </summary>
        private static void GrantBossRewards(Mobile killer, Mobile boss, BossRewardConfig config)
        {
            List<PlayerMobile> recipients = new List<PlayerMobile>();

            // Add killer
            if (killer is PlayerMobile pm)
            {
                recipients.Add(pm);
            }
            else if (killer is BaseCreature bc && bc.ControlMaster is PlayerMobile master)
            {
                recipients.Add(master);
            }

            // Add party members in range
            if (recipients.Count > 0)
            {
                var primaryPlayer = recipients[0];
                foreach (Mobile m in boss.GetMobilesInRange(20))
                {
                    if (m is PlayerMobile partyMember && partyMember != primaryPlayer && partyMember.Alive)
                    {
                        // Check if in same party or guild
                        if (primaryPlayer.Party != null && primaryPlayer.Party == partyMember.Party)
                        {
                            recipients.Add(partyMember);
                        }
                        else if (primaryPlayer.Guild != null && primaryPlayer.Guild == partyMember.Guild)
                        {
                            recipients.Add(partyMember);
                        }
                    }
                }
            }

            // Grant rewards to all recipients
            foreach (var recipient in recipients)
            {
                GrantRewardsToPlayer(recipient, config, recipients.Count > 1);
            }
        }

        /// <summary>
        /// Grant rewards to a single player
        /// </summary>
        private static void GrantRewardsToPlayer(PlayerMobile pm, BossRewardConfig config, bool isGroupKill)
        {
            // Calculate rewards (reduced if group kill)
            double multiplier = isGroupKill ? 0.75 : 1.0;

            int reputation = (int)(config.FactionReputation * multiplier);
            int piety = (int)(config.PietyReward * multiplier);
            int tokens = (int)(config.TokenReward * multiplier);

            // Grant faction reputation
            if (config.PrimaryFaction != VystiaFaction.None && reputation > 0)
            {
                VystiaReputation.AddReputation(pm, config.PrimaryFaction, reputation, "boss kill");

                // Grant tokens
                if (tokens > 0)
                {
                    Server.Items.Vystia.FactionTokenSystem.GiveTokens(pm, config.PrimaryFaction, tokens);
                }
            }

            // Grant piety
            if (config.AssociatedReligion != VystiaReligion.None && piety > 0)
            {
                var pietyData = VystiaPiety.GetPiety(pm);
                if (pietyData != null && pietyData.Religion == config.AssociatedReligion)
                {
                    VystiaPiety.AddPiety(pm, piety, "boss kill");
                }
            }

            // Visual effect
            pm.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
            pm.PlaySound(0x1F2);
        }

        /// <summary>
        /// Grant mini-boss rewards (lower than full boss)
        /// </summary>
        private static void GrantMiniBossRewards(Mobile killer, Mobile boss)
        {
            PlayerMobile pm = null;

            if (killer is PlayerMobile player)
            {
                pm = player;
            }
            else if (killer is BaseCreature bc && bc.ControlMaster is PlayerMobile master)
            {
                pm = master;
            }

            if (pm == null)
                return;

            // Determine faction based on boss location/type
            VystiaFaction faction = DetermineFactionFromLocation(boss);

            if (faction != VystiaFaction.None)
            {
                VystiaReputation.AddReputation(pm, faction, ReputationRewards.MiniBossReward, "mini-boss kill");
                Server.Items.Vystia.FactionTokenSystem.GiveTokens(pm, faction, 2);
            }
        }

        /// <summary>
        /// Determine faction based on creature's location
        /// </summary>
        private static VystiaFaction DetermineFactionFromLocation(Mobile creature)
        {
            if (creature.Map == null || creature.Region == null)
                return VystiaFaction.None;

            string regionName = creature.Region.Name?.ToLower() ?? "";

            if (regionName.Contains("frost") || regionName.Contains("ice"))
                return VystiaFaction.Frostguard;
            if (regionName.Contains("ember") || regionName.Contains("fire") || regionName.Contains("volcano"))
                return VystiaFaction.FlameLegion;
            if (regionName.Contains("forest") || regionName.Contains("verdant") || regionName.Contains("grove"))
                return VystiaFaction.Greenward;
            if (regionName.Contains("crystal") || regionName.Contains("arcane"))
                return VystiaFaction.ArcaneConclave;
            if (regionName.Contains("iron") || regionName.Contains("tech") || regionName.Contains("gear"))
                return VystiaFaction.Technoguild;
            if (regionName.Contains("desert") || regionName.Contains("sand"))
                return VystiaFaction.Sandwalkers;
            if (regionName.Contains("void") || regionName.Contains("shadow"))
                return VystiaFaction.Voidborn;

            return VystiaFaction.None;
        }

        /// <summary>
        /// Check if a creature type is a registered boss
        /// </summary>
        public static bool IsBoss(Type creatureType)
        {
            return creatureType != null && s_BossConfigs.ContainsKey(creatureType);
        }

        /// <summary>
        /// Get boss reward config for a type
        /// </summary>
        public static BossRewardConfig GetBossConfig(Type creatureType)
        {
            if (creatureType != null && s_BossConfigs.TryGetValue(creatureType, out var config))
                return config;
            return null;
        }
    }

    #endregion
}

