/*
 * Vystia Religion System
 *
 * 6 religions with piety tiers (0-1000)
 * - Passive bonuses at 50/200 piety
 * - Devotion Powers at 200/500/900 piety
 * - Daily prayer (+10 piety), tithes (+1 per 100g), pilgrimages (+75)
 */

using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Custom.VystiaClasses.Religion
{
    #region Enums

    /// <summary>
    /// The six religions of Vystia
    /// </summary>
    public enum VystiaReligion
    {
        None = 0,

        /// <summary>
        /// Frosthold religion - Lore: Frosthelm Faith (ice/snow spirits)
        /// </summary>
        FrosthelmFaith = 1,

        /// <summary>
        /// Lore: Surya's Sandscript (sun/sand faith of the desert)
        /// </summary>
        SuryasSandscript = 2,

        /// <summary>
        /// Verdantpeak religion - Lore: Lunara's Covenant (nature's balance)
        /// </summary>
        LunarasCovenant = 3,

        /// <summary>
        /// Lore: Celestis Arcanum (cosmic knowledge, magic/time)
        /// </summary>
        CelestisArcanum = 4,

        /// <summary>
        /// Lore: Oceana's Covenant (oceanic deities and tides)
        /// </summary>
        OceanasCovenant = 5,

        /// <summary>
        /// Lore: Cogsmith Creed (clockwork god of innovation/industry)
        /// </summary>
        CogsmithCreed = 6
    }

    /// <summary>
    /// Piety tier thresholds and bonuses
    /// </summary>
    public enum PietyTier
    {
        None = 0,           // 0-49 piety
        Initiate = 1,       // 50-199 piety (first passive bonus)
        Devoted = 2,        // 200-499 piety (first devotion power)
        Faithful = 3,       // 500-899 piety (second devotion power)
        Exalted = 4         // 900-1000 piety (third devotion power)
    }

    #endregion

    #region Religion Data

    /// <summary>
    /// Static data about each religion
    /// </summary>
    public static class ReligionData
    {
        private static readonly Dictionary<VystiaReligion, ReligionInfo> s_ReligionInfo = new Dictionary<VystiaReligion, ReligionInfo>
        {
            { VystiaReligion.FrosthelmFaith, new ReligionInfo(
                "Frosthelm Faith",
                "Frosthelm Faith",
                "Reveres the spirits of ice and snow as protectors and challengers",
                1150, // Ice blue hue
                new[] { "Cold Resistance +5%", "Cold damage +3%" },
                new[] { "Frost Shield", "Winter's Embrace", "Avatar of Ice" }
            )},
            { VystiaReligion.SuryasSandscript, new ReligionInfo(
                "Surya's Sandscript",
                "Surya's Sandscript",
                "Sun-guided desert faith honoring Surya and sacred winds",
                1358, // Fire red hue
                new[] { "Fire Resistance +5%", "Fire damage +3%" },
                new[] { "Flame Shield", "Phoenix Blessing", "Avatar of Fire" }
            )},
            { VystiaReligion.LunarasCovenant, new ReligionInfo(
                "Lunara's Covenant",
                "Lunara's Covenant",
                "Guardians of the forest's life force and cycles of renewal",
                2010, // Forest green hue
                new[] { "Poison Resistance +5%", "Healing +5%" },
                new[] { "Nature's Grace", "Regeneration", "Avatar of Nature" }
            )},
            { VystiaReligion.CelestisArcanum, new ReligionInfo(
                "Celestis Arcanum",
                "Celestis Arcanum",
                "Seekers of cosmic knowledge and the forces shaping magic and time",
                1154, // Crystal blue hue
                new[] { "Energy Resistance +5%", "Mana Regen +2" },
                new[] { "Arcane Insight", "Mana Shield", "Avatar of Crystal" }
            )},
            { VystiaReligion.OceanasCovenant, new ReligionInfo(
                "Oceana's Covenant",
                "Oceana's Covenant",
                "Oceanic faithful who revere tides, protection, and bounty",
                1109, // Dark purple hue
                new[] { "Physical Resistance +3%", "Stealth +5" },
                new[] { "Shadow Cloak", "Void Step", "Avatar of Void" }
            )},
            { VystiaReligion.CogsmithCreed, new ReligionInfo(
                "Cogsmith Creed",
                "Cogsmith Creed",
                "Followers of a clockwork god who blesses innovation and industry",
                2305, // Metallic hue
                new[] { "Armor +5", "Crafting +5" },
                new[] { "Iron Skin", "Mechanical Blessing", "Avatar of Steel" }
            )}
        };

        public static ReligionInfo GetInfo(VystiaReligion religion)
        {
            if (s_ReligionInfo.TryGetValue(religion, out var info))
                return info;
            return null;
        }

        public static string GetSystemName(VystiaReligion religion)
        {
            return GetInfo(religion)?.Name ?? religion.ToString();
        }

        public static string GetLoreName(VystiaReligion religion)
        {
            return GetInfo(religion)?.LoreName ?? GetSystemName(religion);
        }

        public static string GetDisplayName(VystiaReligion religion)
        {
            var info = GetInfo(religion);
            if (info == null)
                return religion.ToString();

            if (string.IsNullOrWhiteSpace(info.LoreName) || info.LoreName == info.Name)
                return info.Name;

            return string.Concat(info.LoreName, " (", info.Name, ")");
        }

        /// <summary>
        /// Get the piety tier for a given piety value
        /// </summary>
        public static PietyTier GetTier(int piety)
        {
            if (piety >= 900) return PietyTier.Exalted;
            if (piety >= 500) return PietyTier.Faithful;
            if (piety >= 200) return PietyTier.Devoted;
            if (piety >= 50) return PietyTier.Initiate;
            return PietyTier.None;
        }

        /// <summary>
        /// Get the piety required for a tier
        /// </summary>
        public static int GetTierThreshold(PietyTier tier)
        {
            switch (tier)
            {
                case PietyTier.Initiate: return 50;
                case PietyTier.Devoted: return 200;
                case PietyTier.Faithful: return 500;
                case PietyTier.Exalted: return 900;
                default: return 0;
            }
        }
    }

    /// <summary>
    /// Information about a specific religion
    /// </summary>
    public class ReligionInfo
    {
        public string Name { get; }
        public string LoreName { get; }
        public string Description { get; }
        public int Hue { get; }
        public string[] PassiveBonuses { get; }  // At Initiate (50) and Devoted (200)
        public string[] DevotionPowers { get; }   // At Devoted (200), Faithful (500), Exalted (900)

        public ReligionInfo(string name, string loreName, string description, int hue, string[] passiveBonuses, string[] devotionPowers)
        {
            Name = name;
            LoreName = loreName;
            Description = description;
            Hue = hue;
            PassiveBonuses = passiveBonuses;
            DevotionPowers = devotionPowers;
        }
    }

    #endregion

    #region Player Piety Storage

    /// <summary>
    /// Manages piety data for players
    /// Uses attachment pattern to avoid modifying PlayerMobile directly
    /// </summary>
    public static class VystiaPiety
    {
        private static readonly Dictionary<PlayerMobile, PietyData> s_PietyData = new Dictionary<PlayerMobile, PietyData>();

        /// <summary>
        /// Get or create piety data for a player
        /// </summary>
        public static PietyData GetPiety(PlayerMobile pm)
        {
            if (pm == null)
                return null;

            if (!s_PietyData.TryGetValue(pm, out var data))
            {
                data = new PietyData();
                s_PietyData[pm] = data;
            }

            return data;
        }

        /// <summary>
        /// Set piety data for a player
        /// </summary>
        public static void SetPiety(PlayerMobile pm, PietyData data)
        {
            if (pm == null)
                return;

            s_PietyData[pm] = data;
        }

        /// <summary>
        /// Remove piety data for a player (on logout/delete)
        /// </summary>
        public static void RemovePiety(PlayerMobile pm)
        {
            if (pm != null)
                s_PietyData.Remove(pm);
        }

        /// <summary>
        /// Get player's current religion
        /// </summary>
        public static VystiaReligion GetReligion(PlayerMobile pm)
        {
            return GetPiety(pm)?.Religion ?? VystiaReligion.None;
        }

        /// <summary>
        /// Get player's current piety value
        /// </summary>
        public static int GetPietyValue(PlayerMobile pm)
        {
            return GetPiety(pm)?.Piety ?? 0;
        }

        /// <summary>
        /// Get player's piety tier
        /// </summary>
        public static PietyTier GetPietyTier(PlayerMobile pm)
        {
            return ReligionData.GetTier(GetPietyValue(pm));
        }

        /// <summary>
        /// Add piety to a player (capped at 1000)
        /// </summary>
        public static void AddPiety(PlayerMobile pm, int amount, string source)
        {
            var data = GetPiety(pm);
            if (data == null || data.Religion == VystiaReligion.None)
                return;

            int oldPiety = data.Piety;
            data.Piety = Math.Min(1000, Math.Max(0, data.Piety + amount));

            if (data.Piety != oldPiety)
            {
                var oldTier = ReligionData.GetTier(oldPiety);
                var newTier = ReligionData.GetTier(data.Piety);

                if (amount > 0)
                    pm.SendMessage(0x35, "You gain {0} piety from {1}. (Total: {2})", amount, source, data.Piety);
                else
                    pm.SendMessage(0x22, "You lose {0} piety. (Total: {1})", -amount, data.Piety);

                // Notify on tier change
                if (newTier > oldTier)
                {
                    pm.SendMessage(0x35, "You have achieved the rank of {0}!", newTier);
                    pm.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
                    pm.PlaySound(0x1F2);
                }
                else if (newTier < oldTier)
                {
                    pm.SendMessage(0x22, "You have fallen to the rank of {0}.", newTier);
                }
            }
        }

        /// <summary>
        /// Convert a player to a religion
        /// </summary>
        public static bool ConvertToReligion(PlayerMobile pm, VystiaReligion religion)
        {
            var data = GetPiety(pm);
            if (data == null)
                return false;

            if (data.Religion == religion)
            {
                pm.SendMessage("You are already a follower of this religion.");
                return false;
            }

            // Penalty for converting from another religion
            if (data.Religion != VystiaReligion.None)
            {
                pm.SendMessage(0x22, "You abandon {0} and lose all your accumulated piety.",
                    ReligionData.GetInfo(data.Religion)?.Name ?? "your old faith");
                data.Piety = 0;
            }

            data.Religion = religion;
            var info = ReligionData.GetInfo(religion);

            if (info != null)
            {
                pm.SendMessage(0x35, "You have converted to the {0}!", info.Name);
                pm.FixedParticles(0x376A, 9, 32, 5030, info.Hue, 0, EffectLayer.Waist);
                pm.PlaySound(0x1F2);
            }

            return true;
        }
    }

    /// <summary>
    /// Piety data for a single player
    /// </summary>
    public class PietyData
    {
        public VystiaReligion Religion { get; set; }
        public int Piety { get; set; }
        public DateTime LastPrayer { get; set; }
        public DateTime LastTithe { get; set; }
        public DateTime LastPilgrimage { get; set; }

        public PietyData()
        {
            Religion = VystiaReligion.None;
            Piety = 0;
            LastPrayer = DateTime.MinValue;
            LastTithe = DateTime.MinValue;
            LastPilgrimage = DateTime.MinValue;
        }

        /// <summary>
        /// Check if player can pray (once per day)
        /// </summary>
        public bool CanPray()
        {
            return DateTime.UtcNow - LastPrayer >= TimeSpan.FromHours(24);
        }

        /// <summary>
        /// Daily tithe cap check (3000g = 30 piety max per day)
        /// </summary>
        public bool CanTithe()
        {
            // Reset daily tithe if 24 hours passed
            if (DateTime.UtcNow - LastTithe >= TimeSpan.FromHours(24))
                return true;
            return false;
        }

        /// <summary>
        /// Check if player can perform pilgrimage (once per week)
        /// </summary>
        public bool CanPilgrimage()
        {
            return DateTime.UtcNow - LastPilgrimage >= TimeSpan.FromDays(7);
        }
    }

    #endregion

    #region Piety Actions

    /// <summary>
    /// Actions that generate piety
    /// </summary>
    public static class PietyActions
    {
        public const int PrayerPiety = 10;           // +10 per daily prayer
        public const int TithePietyPer100Gold = 1;   // +1 per 100g donated
        public const int TitheDailyCap = 30;          // Max 30 piety from tithes per day (3000g)
        public const int PilgrimagePiety = 75;        // +75 per shrine pilgrimage
        public const int BlessedItemPiety = 25;       // +25 for crafting blessed item

        /// <summary>
        /// Player prays at a shrine
        /// </summary>
        public static void Pray(PlayerMobile pm)
        {
            var data = VystiaPiety.GetPiety(pm);
            if (data == null)
            {
                pm.SendMessage("You have not chosen a religion.");
                return;
            }

            if (data.Religion == VystiaReligion.None)
            {
                pm.SendMessage("You must first convert to a religion before praying.");
                return;
            }

            if (!data.CanPray())
            {
                pm.SendMessage("You have already prayed today. Return tomorrow.");
                return;
            }

            data.LastPrayer = DateTime.UtcNow;
            VystiaPiety.AddPiety(pm, PrayerPiety, "prayer");

            pm.Animate(32, 5, 1, true, false, 0); // Bow animation
            pm.PlaySound(0x1F2);
        }

        /// <summary>
        /// Player performs a pilgrimage to a shrine (+75 piety, weekly cooldown)
        /// </summary>
        public static void PerformPilgrimage(PlayerMobile pm, VystiaReligion shrineReligion)
        {
            var data = VystiaPiety.GetPiety(pm);
            if (data == null)
            {
                pm.SendMessage("You have not chosen a religion.");
                return;
            }

            if (data.Religion == VystiaReligion.None)
            {
                pm.SendMessage("You must first convert to a religion before performing a pilgrimage.");
                return;
            }

            if (data.Religion != shrineReligion)
            {
                pm.SendMessage("You can only perform pilgrimages to shrines of your own religion.");
                return;
            }

            if (!data.CanPilgrimage())
            {
                var timeUntil = TimeSpan.FromDays(7) - (DateTime.UtcNow - data.LastPilgrimage);
                pm.SendMessage("You have already performed a pilgrimage this week. Return in {0} days.", 
                    Math.Ceiling(timeUntil.TotalDays));
                return;
            }

            data.LastPilgrimage = DateTime.UtcNow;
            VystiaPiety.AddPiety(pm, PilgrimagePiety, "pilgrimage");

            pm.Animate(32, 5, 1, true, false, 0); // Bow animation
            pm.PlaySound(0x1F2);
            pm.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
            pm.SendMessage(0x35, "You have completed a pilgrimage to the shrine! (+{0} piety)", PilgrimagePiety);
        }

        /// <summary>
        /// Player donates gold as tithe
        /// </summary>
        public static void Tithe(PlayerMobile pm, int goldAmount)
        {
            var data = VystiaPiety.GetPiety(pm);
            if (data == null || data.Religion == VystiaReligion.None)
            {
                pm.SendMessage("You must first convert to a religion before tithing.");
                return;
            }

            if (goldAmount < 100)
            {
                pm.SendMessage("Tithes must be at least 100 gold.");
                return;
            }

            // Check if player has the gold
            if (!Banker.Withdraw(pm, goldAmount))
            {
                pm.SendMessage("You don't have enough gold to tithe.");
                return;
            }

            int pietyGained = goldAmount / 100;

            // Apply daily cap
            // (For simplicity, we apply full amount - could track daily tithe amount)
            VystiaPiety.AddPiety(pm, pietyGained, "tithing");

            pm.PlaySound(0x2E6); // Coin sound
        }
    }

    #endregion

    #region Religion Opposition

    /// <summary>
    /// Main religion system utilities
    /// </summary>
    public static class VystiaReligionSystem
    {
        /// <summary>
        /// Check if two religions are opposed to each other
        /// Opposed pairs:
        /// - Frosthelm Faith ↔ Surya's Sandscript (Cold vs Fire)
        /// - Lunara's Covenant ↔ Oceana's Covenant (Nature vs Void)
        /// - Cogsmith Creed ↔ Celestis Arcanum (Technology vs Magic)
        /// </summary>
        public static bool AreReligionsOpposed(VystiaReligion r1, VystiaReligion r2)
        {
            if (r1 == VystiaReligion.None || r2 == VystiaReligion.None)
                return false;

            if (r1 == r2)
                return false;

            // Frosthelm Faith ↔ Surya's Sandscript
            if ((r1 == VystiaReligion.FrosthelmFaith && r2 == VystiaReligion.SuryasSandscript) ||
                (r1 == VystiaReligion.SuryasSandscript && r2 == VystiaReligion.FrosthelmFaith))
                return true;

            // Lunara's Covenant ↔ Oceana's Covenant
            if ((r1 == VystiaReligion.LunarasCovenant && r2 == VystiaReligion.OceanasCovenant) ||
                (r1 == VystiaReligion.OceanasCovenant && r2 == VystiaReligion.LunarasCovenant))
                return true;

            // Cogsmith Creed ↔ Celestis Arcanum
            if ((r1 == VystiaReligion.CogsmithCreed && r2 == VystiaReligion.CelestisArcanum) ||
                (r1 == VystiaReligion.CelestisArcanum && r2 == VystiaReligion.CogsmithCreed))
                return true;

            return false;
        }

        #endregion

        #region Devotion Powers

        /// <summary>
        /// Check if player has an active devotion power
        /// Note: This is a simplified check - in a full implementation, devotion powers would be tracked
        /// with cooldowns and active states. For now, we check if player has the required piety tier.
        /// </summary>
        public static bool HasActiveDevotionPower(PlayerMobile pm, string powerName)
        {
            if (pm == null)
                return false;

            var pietyData = VystiaPiety.GetPiety(pm);
            if (pietyData == null || pietyData.Religion == VystiaReligion.None)
                return false;

            var info = ReligionData.GetInfo(pietyData.Religion);
            if (info == null)
                return false;

            var tier = ReligionData.GetTier(pietyData.Piety);

            // Check if the power exists for this religion and if player has required tier
            for (int i = 0; i < info.DevotionPowers.Length; i++)
            {
                if (info.DevotionPowers[i] == powerName)
                {
                    // Power 1 (index 0): Devoted (200 piety)
                    // Power 2 (index 1): Faithful (500 piety)
                    // Power 3 (index 2): Exalted (900 piety)
                    if (i == 0 && tier >= PietyTier.Devoted)
                        return true;
                    if (i == 1 && tier >= PietyTier.Faithful)
                        return true;
                    if (i == 2 && tier >= PietyTier.Exalted)
                        return true;
                }
            }

            return false;
        }

        #endregion
    }
}

