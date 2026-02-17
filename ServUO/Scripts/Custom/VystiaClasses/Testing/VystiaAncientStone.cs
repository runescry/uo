using System;
using Server;
using Server.Commands;
using Server.Items;
using Server.Mobiles;

namespace Server.Custom.VystiaClasses.Testing
{
    /// <summary>
    /// Test item to spawn Vystia Ancient Beings
    /// </summary>
    public class VystiaAncientStone : Item
    {
        private AncientType m_AncientType;

        [CommandProperty(AccessLevel.GameMaster)]
        public AncientType Type
        {
            get { return m_AncientType; }
            set { m_AncientType = value; InvalidateProperties(); }
        }

        public enum AncientType
        {
            FrosthelmEternalWinter,
            EmberflameAshenTyrant,
            VerdantheartForestGuardian,
            CrystalwingPrismaticOracle,
            AbyssusDepthKing,
            ElderOakbark,
            SphynxOfEmberlands,
            FrostFathersAvatar,
            GreatMachinistsConstruct,
            LunarasDryadHerald,
            TheCrystalSphinx,
            IronbarkWarAncient
        }

        [Constructable]
        public VystiaAncientStone(AncientType ancientType) : base(0xED4) // A stone graphic
        {
            Name = $"{ancientType} Spawn Stone";
            Hue = 0x35; // Gold hue for ancient beings
            Movable = true;
            m_AncientType = ancientType;
        }

        public VystiaAncientStone(Serial serial) : base(serial) { }

        public override void OnDoubleClick(Mobile from)
        {
            if (from is PlayerMobile pm && pm.AccessLevel >= AccessLevel.GameMaster)
            {
                BaseCreature ancient = CreateAncient(m_AncientType);
                if (ancient != null)
                {
                    ancient.MoveToWorld(pm.Location, pm.Map);
                    pm.SendMessage(0x35, "Spawned {0} at your location.", ancient.Name);
                }
                else
                {
                    pm.SendMessage(0x22, "Failed to spawn ancient being. Type may not be implemented yet.");
                }
            }
        }

        private BaseCreature CreateAncient(AncientType ancientType)
        {
            switch (ancientType)
            {
                case AncientType.FrosthelmEternalWinter:
                    return new FrosthelmEternalWinter();
                case AncientType.EmberflameAshenTyrant:
                    return new EmberflameAshenTyrant();
                case AncientType.VerdantheartForestGuardian:
                    return new VerdantheartForestGuardian();
                case AncientType.CrystalwingPrismaticOracle:
                    return new CrystalwingPrismaticOracle();
                case AncientType.AbyssusDepthKing:
                    return new AbyssusDepthKing();
                case AncientType.ElderOakbark:
                    return new ElderOakbark();
                case AncientType.SphynxOfEmberlands:
                    return new SphynxOfEmberlands();
                case AncientType.FrostFathersAvatar:
                    return new FrostFathersAvatar();
                case AncientType.GreatMachinistsConstruct:
                    return new GreatMachinistsConstruct();
                case AncientType.LunarasDryadHerald:
                    return new LunarasDryadHerald();
                case AncientType.TheCrystalSphinx:
                    return new TheCrystalSphinx();
                case AncientType.IronbarkWarAncient:
                    return new IronbarkWarAncient();
                default:
                    return null;
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Ancient Type\t{0}", m_AncientType);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.Write((int)m_AncientType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_AncientType = (AncientType)reader.ReadInt();
        }
    }

    /// <summary>
    /// Commands for ancient stone management
    /// </summary>
    public class AncientStoneCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("AncientStones", AccessLevel.GameMaster, new CommandEventHandler(AncientStones_OnCommand));
            CommandSystem.Register("AS", AccessLevel.GameMaster, new CommandEventHandler(AncientStones_OnCommand));
        }

        [Usage("AncientStones")]
        [Description("Spawns one of each Vystia Ancient Stone into your backpack.")]
        private static void AncientStones_OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            foreach (VystiaAncientStone.AncientType ancientType in Enum.GetValues(typeof(VystiaAncientStone.AncientType)))
            {
                pm.AddToBackpack(new VystiaAncientStone(ancientType));
            }

            pm.SendMessage(0x35, "All Vystia Ancient Stones have been added to your backpack.");
        }
    }
}
