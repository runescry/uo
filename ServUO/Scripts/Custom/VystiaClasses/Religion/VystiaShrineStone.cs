using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Commands;

namespace Server.Custom.VystiaClasses.Religion
{
    /// <summary>
    /// Test item: Shrine Stone for each religion
    /// Spawns a shrine of the specified religion when double-clicked
    /// </summary>
    public class VystiaShrineStone : Item
    {
        private VystiaReligion m_Religion;

        [CommandProperty(AccessLevel.GameMaster)]
        public VystiaReligion Religion
        {
            get { return m_Religion; }
            set
            {
                m_Religion = value;
                InvalidateProperties();
            }
        }

        [Constructable]
        public VystiaShrineStone(VystiaReligion religion)
            : base(0x1EA7) // Shrine graphic
        {
            m_Religion = religion;
            Name = GetStoneName(religion);
            Hue = ReligionData.GetInfo(religion)?.Hue ?? 0;
            Weight = 1.0;
            Movable = true;
        }

        public VystiaShrineStone(Serial serial)
            : base(serial)
        {
        }

        private string GetStoneName(VystiaReligion religion)
        {
            switch (religion)
            {
                case VystiaReligion.FrosthelmFaith:
                case VystiaReligion.SuryasSandscript:
                case VystiaReligion.LunarasCovenant:
                case VystiaReligion.CelestisArcanum:
                case VystiaReligion.OceanasCovenant:
                case VystiaReligion.CogsmithCreed:
                    return string.Concat(ReligionData.GetLoreName(religion), " Shrine Stone");
                default:
                    return "Shrine Stone";
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from == null || !(from is PlayerMobile))
                return;

            if (!IsChildOf(from.Backpack) && !from.InRange(GetWorldLocation(), 2))
            {
                from.SendMessage(0x22, "You must be closer to use the shrine stone.");
                return;
            }

            // Create shrine at the player's location (in front of them)
            Point3D spawnLoc = from.Location;
            Map spawnMap = from.Map;

            // Try to place it 1 tile in front of the player
            int offsetX = 0, offsetY = 0;
            switch (from.Direction & Direction.Mask)
            {
                case Direction.North: offsetY = -1; break;
                case Direction.South: offsetY = 1; break;
                case Direction.East: offsetX = 1; break;
                case Direction.West: offsetX = -1; break;
                case Direction.Up: offsetX = -1; offsetY = -1; break;
                case Direction.Down: offsetX = 1; offsetY = 1; break;
                case Direction.Left: offsetX = -1; offsetY = 1; break;
                case Direction.Right: offsetX = 1; offsetY = -1; break;
            }

            Point3D frontLoc = new Point3D(spawnLoc.X + offsetX, spawnLoc.Y + offsetY, spawnLoc.Z);
            if (spawnMap.CanFit(frontLoc, 16, false, false))
                spawnLoc = frontLoc;

            VystiaShrine shrine = new VystiaShrine(m_Religion);
            shrine.MoveToWorld(spawnLoc, spawnMap);

            from.SendMessage(0x35, "You have created a {0}!", GetStoneName(m_Religion));
            from.PlaySound(0x1F2);

            // Delete the stone
            Delete();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Religion\t{0}", ReligionData.GetDisplayName(m_Religion));
            list.Add(1060659, "Usage\tDouble-click to create shrine");
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
            writer.Write((int)m_Religion);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Religion = (VystiaReligion)reader.ReadInt();
        }
    }

    /// <summary>
    /// Command to spawn all shrine stones for testing
    /// </summary>
    public class ShrineStoneCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("ShrineStones", AccessLevel.GameMaster, ShrineStones_OnCommand);
            CommandSystem.Register("SS", AccessLevel.GameMaster, ShrineStones_OnCommand); // Short alias
        }

        [Usage("ShrineStones")]
        [Description("Spawns all religion shrine stones in your backpack")]
        private static void ShrineStones_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            if (from == null || from.Backpack == null)
                return;

            // Create shrine stones for all 6 religions
            VystiaShrineStone[] stones = new VystiaShrineStone[]
            {
                new VystiaShrineStone(VystiaReligion.FrosthelmFaith),
                new VystiaShrineStone(VystiaReligion.SuryasSandscript),
                new VystiaShrineStone(VystiaReligion.LunarasCovenant),
                new VystiaShrineStone(VystiaReligion.CelestisArcanum),
                new VystiaShrineStone(VystiaReligion.OceanasCovenant),
                new VystiaShrineStone(VystiaReligion.CogsmithCreed)
            };

            foreach (var stone in stones)
            {
                from.Backpack.DropItem(stone);
            }

            from.SendMessage(0x35, "You have received {0} shrine stones!", stones.Length);
        }
    }
}

