#if VYSTIA_SONGWEAVING
using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Commands;

namespace Server.Custom.VystiaClasses.Systems
{
    public static class SongweavingMasterySystem
    {
        public const int MaxMasteryLevel = 32;
        public const int XPPerPoint = 10;
        public const int MaxPerSong = 5;

        public static void AwardMastery(PlayerMobile caster, int amount)
        {
            if (caster == null || caster.Deleted)
                return;

            if (caster.VystiaClassV2 != PlayerClassTypeV2.Bard)
                return;

            if (caster.SongMasteryLevel >= MaxMasteryLevel)
                return;

            caster.SongMasteryXP += amount;

            while (caster.SongMasteryXP >= XPPerPoint && caster.SongMasteryLevel < MaxMasteryLevel)
            {
                caster.SongMasteryXP -= XPPerPoint;
                caster.SongMasteryLevel++;
                caster.SongMasteryPoints++;
            }
        }

        public static int GetPotencyLevel(PlayerMobile caster, string songKey)
        {
            if (caster == null || songKey == null)
                return 0;

            return caster.SongMasteryPotency.TryGetValue(songKey, out int value) ? value : 0;
        }

        public static int GetDurationLevel(PlayerMobile caster, string songKey)
        {
            if (caster == null || songKey == null)
                return 0;

            return caster.SongMasteryDuration.TryGetValue(songKey, out int value) ? value : 0;
        }

        public static bool TrySpendPoint(PlayerMobile caster, string songKey, bool potency)
        {
            if (caster == null || caster.Deleted || songKey == null)
                return false;

            if (caster.SongMasteryPoints <= 0)
                return false;

            var table = potency ? caster.SongMasteryPotency : caster.SongMasteryDuration;
            int current = table.TryGetValue(songKey, out int value) ? value : 0;
            if (current >= MaxPerSong)
                return false;

            table[songKey] = current + 1;
            caster.SongMasteryPoints--;
            return true;
        }

        public static TimeSpan ApplyDuration(PlayerMobile caster, TimeSpan baseDuration, string songKey)
        {
            int level = GetDurationLevel(caster, songKey);
            if (level <= 0)
                return baseDuration;

            double scalar = 1.0 + (level * 0.1);
            return TimeSpan.FromSeconds(baseDuration.TotalSeconds * scalar);
        }

        public static int ApplyPower(PlayerMobile caster, int basePower, string songKey)
        {
            int level = GetPotencyLevel(caster, songKey);
            if (level <= 0)
                return basePower;

            double scalar = 1.0 + (level * 0.1);
            return (int)Math.Round(basePower * scalar);
        }

        public static void SendStatus(PlayerMobile caster)
        {
            if (caster == null)
                return;

            caster.SendMessage($"Song Mastery: Level {caster.SongMasteryLevel} | XP {caster.SongMasteryXP}/{XPPerPoint} | Points {caster.SongMasteryPoints}");
        }
    }
}
#endif
