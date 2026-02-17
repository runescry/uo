using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Systems;

namespace Server.Mobiles
{
    /// <summary>
    /// Divine Blessing System
    /// Manages 24-hour blessings granted by ancient beings to Champion tier players
    /// </summary>
    public static class VystiaDivineBlessingSystem
    {
        private static Dictionary<PlayerMobile, DivineBlessingData> s_Blessings = new Dictionary<PlayerMobile, DivineBlessingData>();

        /// <summary>
        /// Grant a divine blessing to a player
        /// </summary>
        public static void GrantBlessing(PlayerMobile pm, BaseAncientBeing ancientBeing)
        {
            if (pm == null || ancientBeing == null)
                return;

            var blessingType = GetBlessingTypeForAncientBeing(ancientBeing);
            if (blessingType == DivineBlessingType.None)
                return;

            // Check if player already has this blessing
            var existingBlessing = GetBlessing(pm);
            if (existingBlessing != null && existingBlessing.Type == blessingType)
            {
                pm.SendMessage("You already have this blessing active.");
                return;
            }

            // Grant the blessing
            var blessing = new DivineBlessingData
            {
                Type = blessingType,
                GrantedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };

            s_Blessings[pm] = blessing;
            ApplyBlessingEffects(pm, blessingType);

            pm.SendMessage(0x35, "You have received the blessing: {0}!", GetBlessingName(blessingType));
            pm.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
            pm.PlaySound(0x1F2);
        }

        /// <summary>
        /// Get the blessing type for an ancient being
        /// </summary>
        private static DivineBlessingType GetBlessingTypeForAncientBeing(BaseAncientBeing ancientBeing)
        {
            string beingName = ancientBeing.GetType().Name.ToLower();
            
            if (beingName.Contains("machinist") || beingName.Contains("construct"))
                return DivineBlessingType.MachinistsPerfection;
            if (beingName.Contains("lunara") || beingName.Contains("dryad") || beingName.Contains("herald"))
                return DivineBlessingType.LunarRadiance;
            if (beingName.Contains("sphynx") || beingName.Contains("surya"))
                return DivineBlessingType.SolarClarity;
            if (beingName.Contains("abyssus") || beingName.Contains("depth"))
                return DivineBlessingType.AbyssalFavor;
            if (beingName.Contains("crystal") || beingName.Contains("crystalwing"))
                return DivineBlessingType.Starlight;
            if (beingName.Contains("frost") || beingName.Contains("father") || beingName.Contains("avatar"))
                return DivineBlessingType.FrostFathersEndurance;

            return DivineBlessingType.None;
        }

        /// <summary>
        /// Get current blessing for a player
        /// </summary>
        public static DivineBlessingData GetBlessing(PlayerMobile pm)
        {
            if (pm == null)
                return null;

            if (s_Blessings.TryGetValue(pm, out var blessing))
            {
                // Check if expired
                if (DateTime.UtcNow >= blessing.ExpiresAt)
                {
                    RemoveBlessing(pm);
                    return null;
                }
                return blessing;
            }

            return null;
        }

        /// <summary>
        /// Remove a blessing from a player
        /// </summary>
        public static void RemoveBlessing(PlayerMobile pm)
        {
            if (pm == null)
                return;

            if (s_Blessings.TryGetValue(pm, out var blessing))
            {
                RemoveBlessingEffects(pm, blessing.Type);
                s_Blessings.Remove(pm);
            }
        }

        /// <summary>
        /// Apply blessing effects to a player
        /// </summary>
        private static void ApplyBlessingEffects(PlayerMobile pm, DivineBlessingType type)
        {
            switch (type)
            {
                case DivineBlessingType.MachinistsPerfection:
                    // +15% exceptional craft chance - handled in crafting system
                    break;
                case DivineBlessingType.LunarRadiance:
                    // +15% healing done/received - handled in healing system
                    break;
                case DivineBlessingType.SolarClarity:
                    // Immune to illusions/blind - handled via buff system
                    // Note: Custom immunity flags would need to be added to buff system
                    VystiaBuffManager.Instance.ApplyBuff(pm, pm, VystiaBuffType.AllResist, TimeSpan.FromHours(24), 0);
                    break;
                case DivineBlessingType.AbyssalFavor:
                    // Water breathing, +35% swim - handled via buff system
                    // Note: Custom water breathing would need to be added to buff system
                    VystiaBuffManager.Instance.ApplyBuff(pm, pm, VystiaBuffType.MovementSpeedBuff, TimeSpan.FromHours(24), 35);
                    break;
                case DivineBlessingType.Starlight:
                    // +15% spell power - handled in spell damage system
                    break;
                case DivineBlessingType.FrostFathersEndurance:
                    // +50 HP, immune to slow - handled via buff system
                    VystiaBuffManager.Instance.ApplyBuff(pm, pm, VystiaBuffType.AllStatsBuff, TimeSpan.FromHours(24), 50);
                    break;
            }
        }

        /// <summary>
        /// Remove blessing effects from a player
        /// </summary>
        private static void RemoveBlessingEffects(PlayerMobile pm, DivineBlessingType type)
        {
            switch (type)
            {
                case DivineBlessingType.SolarClarity:
                    VystiaBuffManager.Instance.RemoveBuff(pm, VystiaBuffType.AllResist);
                    break;
                case DivineBlessingType.AbyssalFavor:
                    VystiaBuffManager.Instance.RemoveBuff(pm, VystiaBuffType.MovementSpeedBuff);
                    break;
                case DivineBlessingType.FrostFathersEndurance:
                    VystiaBuffManager.Instance.RemoveBuff(pm, VystiaBuffType.AllStatsBuff);
                    break;
            }
        }

        /// <summary>
        /// Get blessing name
        /// </summary>
        private static string GetBlessingName(DivineBlessingType type)
        {
            switch (type)
            {
                case DivineBlessingType.MachinistsPerfection: return "Machinist's Perfection";
                case DivineBlessingType.LunarRadiance: return "Lunar Radiance";
                case DivineBlessingType.SolarClarity: return "Solar Clarity";
                case DivineBlessingType.AbyssalFavor: return "Abyssal Favor";
                case DivineBlessingType.Starlight: return "Starlight";
                case DivineBlessingType.FrostFathersEndurance: return "Frost Father's Endurance";
                default: return "Unknown Blessing";
            }
        }

        /// <summary>
        /// Check and expire old blessings (called periodically)
        /// </summary>
        public static void CheckExpiredBlessings()
        {
            var expired = new List<PlayerMobile>();
            foreach (var kvp in s_Blessings)
            {
                if (DateTime.UtcNow >= kvp.Value.ExpiresAt)
                {
                    expired.Add(kvp.Key);
                }
            }

            foreach (var pm in expired)
            {
                var blessing = s_Blessings[pm];
                pm.SendMessage(0x22, "Your blessing {0} has expired.", GetBlessingName(blessing.Type));
                RemoveBlessing(pm);
            }
        }
    }

    /// <summary>
    /// Divine blessing types
    /// </summary>
    public enum DivineBlessingType
    {
        None = 0,
        MachinistsPerfection,      // +15% exceptional craft
        LunarRadiance,             // +15% healing
        SolarClarity,              // Immune to illusions/blind
        AbyssalFavor,              // Water breathing, +35% swim
        Starlight,                 // +15% spell power
        FrostFathersEndurance      // +50 HP, immune to slow
    }

    /// <summary>
    /// Divine blessing data
    /// </summary>
    public class DivineBlessingData
    {
        public DivineBlessingType Type { get; set; }
        public DateTime GrantedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
