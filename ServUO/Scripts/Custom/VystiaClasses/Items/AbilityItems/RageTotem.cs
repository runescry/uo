using System;
using Server;
using Server.Mobiles;

namespace Server.Items.VystiaClassItems
{
    /// <summary>
    /// Rage Totem - Barbarian ability item
    /// Grants +20 STR for 30 seconds when activated
    /// </summary>
    public class RageTotem : Item
    {
        private int m_Charges;
        private int m_MaxCharges = 10;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set { m_Charges = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxCharges
        {
            get { return m_MaxCharges; }
            set { m_MaxCharges = value; InvalidateProperties(); }
        }

        [Constructable]
        public RageTotem() : this(10)
        {
        }

        [Constructable]
        public RageTotem(int charges) : base(0x1F1C) // Totem item ID
        {
            Name = "Rage Totem";
            Hue = 1150; // Ice blue
            Weight = 1.0;
            LootType = LootType.Blessed;
            m_Charges = charges;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060741, m_Charges.ToString()); // ~1_val~ charges remaining
            list.Add(1070722, "Channel primal fury for powerful attacks");
            list.Add("Duration: 30 seconds");
            list.Add("Effect: +20 Strength");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                return;
            }

            if (m_Charges <= 0)
            {
                from.SendMessage(0x21, "Your rage totem is depleted. Rest to recharge it.");
                return;
            }

            // Check if already has rage buff active
            StatMod existingMod = from.GetStatMod("RageBuff_Str");
            if (existingMod != null)
            {
                from.SendMessage(0x21, "You are already enraged!");
                return;
            }

            // Activate rage mode
            from.SendMessage(0x21, "You channel primal fury through the totem!");
            from.FixedParticles(0x3709, 10, 30, 5052, 1150, 0, EffectLayer.Waist);
            from.PlaySound(0x208);

            // Consume charge
            m_Charges--;
            InvalidateProperties();

            // Apply rage buff
            from.AddStatMod(new StatMod(StatType.Str, "RageBuff_Str", 20, TimeSpan.FromSeconds(30)));

            // Message when buff starts
            from.SendMessage(0x21, "Your strength surges with primal fury! (+20 STR for 30 seconds)");

            // Visual effect
            Effects.SendLocationParticles(
                EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration),
                0x3709, 10, 30, 1150, 0, 5052, 0);

            // Schedule buff end message
            Timer.DelayCall(TimeSpan.FromSeconds(30), () =>
            {
                if (from != null && !from.Deleted)
                {
                    from.SendMessage(0x21, "Your rage subsides...");
                }
            });
        }

        public RageTotem(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.Write(m_Charges);
            writer.Write(m_MaxCharges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Charges = reader.ReadInt();
            m_MaxCharges = reader.ReadInt();
        }
    }
}
