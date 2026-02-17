using System;
using System.Collections.Generic;
using Server;

namespace Server.Custom.VystiaClasses.Systems
{
    public static class VystiaLuckBonusSystem
    {
        private class LuckBonus
        {
            public int Value;
            public DateTime ExpiresAt;
        }

        private static readonly Dictionary<Mobile, LuckBonus> Bonuses = new Dictionary<Mobile, LuckBonus>();

        public static void ApplyLuckBonus(Mobile target, int value, TimeSpan duration)
        {
            if (target == null || value == 0)
                return;

            Bonuses[target] = new LuckBonus
            {
                Value = value,
                ExpiresAt = DateTime.UtcNow + duration
            };

            target.SendMessage(0x3B2, $"Fortune favors you (+{value} Luck).");
            if (BuffInfo.Enabled)
            {
                BuffInfo.AddBuff(target, new BuffInfo(BuffIcon.PotionGloriousFortune, BuffInfo.Blank, BuffInfo.Blank, duration, target, $"Inspire Accuracy (+{value} Luck)"));
            }
            target.Delta(MobileDelta.Armor);
        }

        public static int GetLuckBonus(Mobile target)
        {
            if (target == null)
                return 0;

            if (!Bonuses.TryGetValue(target, out LuckBonus bonus))
                return 0;

            if (DateTime.UtcNow >= bonus.ExpiresAt)
            {
                Bonuses.Remove(target);
                target.SendMessage(0x22, "Your luck bonus fades.");
                if (BuffInfo.Enabled)
                {
                    BuffInfo.RemoveBuff(target, BuffIcon.PotionGloriousFortune);
                }
                target.Delta(MobileDelta.Armor);
                return 0;
            }

            return bonus.Value;
        }
    }
}
