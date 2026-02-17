using System;
using Server;
using Server.Commands;
using Server.Items;
using Server.Mobiles;

namespace Server.Custom.VystiaClasses.Testing
{
    /// <summary>
    /// Test item to spawn Vystia regional bosses
    /// </summary>
    public class VystiaBossStone : Item
    {
        private BossType m_BossType;

        [CommandProperty(AccessLevel.GameMaster)]
        public BossType Type
        {
            get { return m_BossType; }
            set { m_BossType = value; InvalidateProperties(); }
        }

        public enum BossType
        {
            FrostFather,
            VolcanoWyrm,
            SphinxOfSurya,
            CovenMatriarch,
            AncientTreant,
            CrystalDrakeAlpha,
            ForgeMaster,
            GriffinLord,
            AncientKraken,
            TimewornLich
        }

        [Constructable]
        public VystiaBossStone(BossType bossType) : base(0xED4) // A stone graphic
        {
            Name = $"{bossType} Spawn Stone";
            Hue = 0x22; // Red hue for boss stones
            Movable = true;
            m_BossType = bossType;
        }

        public VystiaBossStone(Serial serial) : base(serial) { }

        public override void OnDoubleClick(Mobile from)
        {
            if (from is PlayerMobile pm && pm.AccessLevel >= AccessLevel.GameMaster)
            {
                BaseCreature boss = CreateBoss(m_BossType);
                if (boss != null)
                {
                    boss.MoveToWorld(pm.Location, pm.Map);
                    pm.SendMessage(0x35, "Spawned {0} at your location.", boss.Name);
                }
                else
                {
                    pm.SendMessage(0x22, "Failed to spawn boss. Boss type may not be implemented yet.");
                }
            }
        }

        private BaseCreature CreateBoss(BossType bossType)
        {
            switch (bossType)
            {
                case BossType.FrostFather:
                    return new FrostFather();
                case BossType.VolcanoWyrm:
                    return new VolcanoWyrm();
                case BossType.SphinxOfSurya:
                    return new SphinxOfSurya();
                case BossType.CovenMatriarch:
                    return new CovenMatriarch();
                case BossType.AncientTreant:
                    return new AncientTreant();
                case BossType.CrystalDrakeAlpha:
                    return new CrystalDrakeAlpha();
                case BossType.ForgeMaster:
                    return new ForgeMaster();
                case BossType.GriffinLord:
                    return new GriffinLord();
                case BossType.AncientKraken:
                    return new AncientKraken();
                case BossType.TimewornLich:
                    return new TimewornLich();
                default:
                    return null;
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Boss Type\t{0}", m_BossType);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.Write((int)m_BossType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_BossType = (BossType)reader.ReadInt();
        }
    }

    /// <summary>
    /// Commands for boss stone management
    /// </summary>
    public class BossStoneCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("BossStones", AccessLevel.GameMaster, new CommandEventHandler(BossStones_OnCommand));
            CommandSystem.Register("BS", AccessLevel.GameMaster, new CommandEventHandler(BossStones_OnCommand));
        }

        [Usage("BossStones")]
        [Description("Spawns one of each Vystia Boss Stone into your backpack.")]
        private static void BossStones_OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            foreach (VystiaBossStone.BossType bossType in Enum.GetValues(typeof(VystiaBossStone.BossType)))
            {
                pm.AddToBackpack(new VystiaBossStone(bossType));
            }

            pm.SendMessage(0x35, "All Vystia Boss Stones have been added to your backpack.");
        }
    }
}
