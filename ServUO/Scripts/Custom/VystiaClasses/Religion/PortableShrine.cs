using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Custom.VystiaClasses.Religion
{
    /// <summary>
    /// Portable shrine item - allows players to place a temporary shrine
    /// One per religion, craftable with regional materials
    /// </summary>
    public class PortableShrine : Item
    {
        private VystiaReligion m_Religion;
        private DateTime m_PlacedTime;
        private bool m_IsPlaced = false;

        [CommandProperty(AccessLevel.GameMaster)]
        public VystiaReligion Religion
        {
            get { return m_Religion; }
            set { m_Religion = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsPlaced => m_IsPlaced;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime PlacedTime => m_PlacedTime;

        [Constructable]
        public PortableShrine(VystiaReligion religion) : base(0x1EA7) // Shrine graphic
        {
            m_Religion = religion;
            Name = GetShrineName(religion) + " (Portable)";
            Hue = ReligionData.GetInfo(religion)?.Hue ?? 0;
            Weight = 10.0;
            LootType = LootType.Blessed;
        }

        public PortableShrine(Serial serial) : base(serial) { }

        private string GetShrineName(VystiaReligion religion)
        {
            switch (religion)
            {
                case VystiaReligion.FrosthelmFaith:
                case VystiaReligion.SuryasSandscript:
                case VystiaReligion.LunarasCovenant:
                case VystiaReligion.CelestisArcanum:
                case VystiaReligion.OceanasCovenant:
                case VystiaReligion.CogsmithCreed:
                    return string.Concat("Portable Shrine of ", ReligionData.GetLoreName(religion));
                default:
                    return "Portable Shrine";
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from == null || !(from is PlayerMobile))
                return;

            PlayerMobile pm = from as PlayerMobile;

            if (!IsAccessibleTo(pm))
                return;

            if (m_IsPlaced)
            {
                // If placed, act as a shrine (show menu)
                var pietyData = VystiaPiety.GetPiety(pm);
                if (pietyData != null && pietyData.Religion == m_Religion)
                {
                    // Create a temporary shrine to use its menu system
                    var tempShrine = new VystiaShrine(m_Religion);
                    tempShrine.OnDoubleClick(from);
                    return;
                }

                // Pick up the shrine
                if (!from.InRange(GetWorldLocation(), 2))
                {
                    from.SendMessage(0x22, "You are too far away to pick up the shrine.");
                    return;
                }

                // Check if shrine has been placed for at least 1 minute (prevents abuse)
                if (DateTime.UtcNow - m_PlacedTime < TimeSpan.FromMinutes(1))
                {
                    from.SendMessage(0x22, "You must wait at least 1 minute before picking up the shrine.");
                    return;
                }

                m_IsPlaced = false;
                Movable = true;
                Weight = 10.0;
                from.SendMessage(0x35, "You pick up the portable shrine.");
            }
            else
            {
                // Place the shrine
                if (!from.InRange(from.Location, 1))
                {
                    from.SendMessage(0x22, "You must be standing still to place the shrine.");
                    return;
                }

                // Check if player follows this religion
                var pietyData = VystiaPiety.GetPiety(pm);
                if (pietyData == null || pietyData.Religion != m_Religion)
                {
                    pm.SendMessage(0x22, "You must follow {0} to place this shrine.", ReligionData.GetDisplayName(m_Religion));
                    return;
                }

                // Check if there's already a shrine nearby (within 10 tiles)
                foreach (Item item in from.Map.GetItemsInRange(from.Location, 10))
                {
                    if (item is VystiaShrine || (item is PortableShrine ps && ps.IsPlaced))
                    {
                        from.SendMessage(0x22, "There is already a shrine nearby.");
                        return;
                    }
                }

                m_IsPlaced = true;
                m_PlacedTime = DateTime.UtcNow;
                Movable = false;
                Weight = 0;
                from.SendMessage(0x35, "You place the portable shrine. You can pick it up again after 1 minute.");
            }
        }

        public override bool OnMoveOver(Mobile m)
        {
            // When placed, act as a shrine - players can double-click to use it
            return base.OnMoveOver(m);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Religion\t{0}", m_Religion);
            if (m_IsPlaced)
                list.Add(1060659, "Status\tPlaced");
            else
                list.Add(1060659, "Status\tPortable");
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
            writer.Write((int)m_Religion);
            writer.Write(m_IsPlaced);
            writer.Write(m_PlacedTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Religion = (VystiaReligion)reader.ReadInt();
            m_IsPlaced = reader.ReadBool();
            m_PlacedTime = reader.ReadDateTime();
        }
    }
}

