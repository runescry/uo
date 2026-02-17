using System;
using Server;
using Server.Mobiles;

namespace Server.Items.VystiaClassItems
{
    /// <summary>
    /// Construct Control Device - Artificer ability item
    /// Summons clockwork constructs to fight for the artificer
    /// </summary>
    public class ConstructControlDevice : Item
    {
        private int m_Charges;
        private int m_MaxCharges = 5;

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
        public ConstructControlDevice() : this(5)
        {
        }

        [Constructable]
        public ConstructControlDevice(int charges) : base(0x1EB8) // Clock item ID
        {
            Name = "Construct Control Device";
            Hue = 2305; // Metallic
            Weight = 2.0;
            LootType = LootType.Blessed;
            m_Charges = charges;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060741, m_Charges.ToString()); // ~1_val~ charges remaining
            list.Add(1070722, "Summon and control clockwork constructs");
            list.Add("Duration: 10 minutes");
            list.Add("Construct: Clockwork Scout");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack
                return;
            }

            if (m_Charges <= 0)
            {
                from.SendMessage(0x3B2, "Your control device needs recharging!");
                return;
            }

            // Check follower slots
            if (from.Followers + 2 > from.FollowersMax)
            {
                from.SendMessage(0x3B2, "You have too many followers to summon a construct!");
                return;
            }

            // Summon clockwork construct
            from.SendMessage(0x3B2, "You activate your construct control device!");
            from.FixedParticles(0x3709, 10, 30, 5052, 2305, 0, EffectLayer.Waist);
            from.PlaySound(0x22F);

            // Create construct
            ClockworkScout construct = new ClockworkScout();
            construct.Controlled = true;
            construct.ControlMaster = from;
            construct.ControlOrder = OrderType.Follow;
            construct.ControlTarget = from; // Set target so it follows immediately

            Point3D loc = from.Location;
            Map map = from.Map;

            if (map != null)
            {
                // Try to find valid spawn location
                for (int i = 0; i < 10; i++)
                {
                    int x = loc.X + Utility.RandomMinMax(-2, 2);
                    int y = loc.Y + Utility.RandomMinMax(-2, 2);
                    int z = map.GetAverageZ(x, y);

                    Point3D p = new Point3D(x, y, z);

                    if (map.CanSpawnMobile(p))
                    {
                        construct.MoveToWorld(p, map);

                        // Visual effect
                        Effects.SendLocationParticles(
                            EffectItem.Create(p, map, EffectItem.DefaultDuration),
                            0x3728, 10, 10, 2305, 0, 5052, 0);

                        // Consume charge
                        m_Charges--;
                        InvalidateProperties();

                        from.SendMessage(0x3B2, "Your clockwork construct materializes!");
                        return;
                    }
                }
            }

            // Failed to find spawn location
            construct.Delete();
            from.SendMessage(0x3B2, "There is no room to summon your construct!");
        }

        public ConstructControlDevice(Serial serial) : base(serial)
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
