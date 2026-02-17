using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a gale elemental corpse")]
    public class VystiaGaleElemental : BaseCreature
    {
        [Constructable]
        public VystiaGaleElemental() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a gale elemental";
            Body = 13;
            Hue = 1281;
            BaseSoundID = 655;

            SetStr(180, 220);
            SetDex(150, 180);
            SetInt(120, 160);

            SetHits(200, 260);
            SetDamage(10, 16);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 70, 80);

            SetSkill(SkillName.EvalInt, 70.0, 90.0);
            SetSkill(SkillName.Magery, 70.0, 90.0);
            SetSkill(SkillName.MagicResist, 80.0, 100.0);
            SetSkill(SkillName.Tactics, 70.0, 90.0);
            SetSkill(SkillName.Wrestling, 60.0, 80.0);

            Fame = 6000;
            Karma = -6000;
            VirtualArmor = 35;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);

            PackItem(new CrystalOre(Utility.RandomMinMax(3, 6)));
            PackItem(new CrystallineIngot(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new StormCrystal(Utility.RandomMinMax(1, 2)));
            if (Utility.RandomDouble() < 0.08)
                PackItem(new StormCrystal());
        }

        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 3;

        public VystiaGaleElemental(Serial serial) : base(serial)
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
