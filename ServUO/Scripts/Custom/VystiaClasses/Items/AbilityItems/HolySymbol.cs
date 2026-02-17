using System;
using Server;
using Server.Mobiles;

namespace Server.Items.VystiaClassItems
{
    /// <summary>
    /// Holy Symbol - Cleric ability item
    /// AoE healing ability with cooldown
    /// </summary>
    public class HolySymbol : Item
    {
        private DateTime m_NextUse;
        private TimeSpan m_Cooldown = TimeSpan.FromSeconds(60);

        [Constructable]
        public HolySymbol() : base(0x1F14) // Holy symbol item ID
        {
            Name = "Holy Symbol";
            Hue = 1153; // Holy white/gold
            Weight = 0.5;
            LootType = LootType.Blessed;
            m_NextUse = DateTime.UtcNow;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1070722, "Channel divine energy for area healing");
            list.Add("Range: 5 tiles");
            list.Add("Healing: 10-20 HP");
            list.Add("Cooldown: 60 seconds");

            TimeSpan remaining = m_NextUse - DateTime.UtcNow;
            if (remaining > TimeSpan.Zero)
            {
                list.Add($"Ready in: {remaining.Seconds}s");
            }
            else
            {
                list.Add(1153, "Ready to use");
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack
                return;
            }

            if (DateTime.UtcNow < m_NextUse)
            {
                TimeSpan remaining = m_NextUse - DateTime.UtcNow;
                from.SendMessage(0x3B2, $"The holy symbol needs time to recharge. ({remaining.Seconds} seconds remaining)");
                return;
            }

            // Activate divine healing
            from.SendMessage(0x3B2, "You channel divine energy through the holy symbol!");
            from.FixedParticles(0x373A, 10, 15, 5018, 1153, 0, EffectLayer.Waist);
            from.PlaySound(0x1F2);

            // Heal nearby allies
            int healCount = 0;
            foreach (Mobile m in from.GetMobilesInRange(5))
            {
                if (m != null && m.Alive && !from.CanBeHarmful(m, false))
                {
                    // Heal amount
                    int healAmount = Utility.RandomMinMax(10, 20);
                    m.Hits += healAmount;

                    // Visual effect
                    m.FixedParticles(0x376A, 9, 32, 5005, 1153, 0, EffectLayer.Waist);
                    m.SendMessage(0x3B2, $"You are healed by divine light! (+{healAmount} HP)");

                    healCount++;
                }
            }

            if (healCount > 0)
            {
                from.SendMessage(0x3B2, $"You have healed {healCount} allies with divine energy!");
            }
            else
            {
                from.SendMessage(0x3B2, "There are no allies nearby to heal.");
            }

            // Set cooldown
            m_NextUse = DateTime.UtcNow + m_Cooldown;
            InvalidateProperties();

            // Visual area effect
            Effects.SendLocationParticles(
                EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration),
                0x373A, 10, 30, 1153, 0, 5036, 0);
        }

        public HolySymbol(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.Write(m_NextUse);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_NextUse = reader.ReadDateTime();
        }
    }
}
