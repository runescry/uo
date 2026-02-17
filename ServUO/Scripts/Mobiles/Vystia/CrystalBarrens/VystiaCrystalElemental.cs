using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a crystal elemental corpse")]
    public class VystiaCrystalElemental : BaseCreature
    {
        [Constructable]
        public VystiaCrystalElemental() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a crystal elemental";
            Body = 13;
            Hue = 1154;
            BaseSoundID = 263;

            SetStr(180, 220);
            SetDex(70, 90);
            SetInt(120, 160);

            SetHits(200, 260);
            SetDamage(12, 18);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 70, 80);

            SetSkill(SkillName.EvalInt, 70.0, 90.0);
            SetSkill(SkillName.Magery, 70.0, 90.0);
            SetSkill(SkillName.MagicResist, 80.0, 100.0);
            SetSkill(SkillName.Tactics, 70.0, 90.0);
            SetSkill(SkillName.Wrestling, 70.0, 90.0);

            Fame = 6500;
            Karma = -6500;
            VirtualArmor = 40;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);

            PackItem(new CrystalOre(Utility.RandomMinMax(3, 6)));
            PackItem(new CrystallineIngot(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new PrismaticShard(Utility.RandomMinMax(1, 2)));
        }

        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 3;

        public VystiaCrystalElemental(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
