using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a mire elemental corpse")]
    public class VystiaMireElemental : BaseCreature
    {
        [Constructable]
        public VystiaMireElemental() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a mire elemental";
            Body = 14;
            Hue = 2073;
            BaseSoundID = 268;

            SetStr(200, 260);
            SetDex(60, 80);
            SetInt(80, 100);

            SetHits(220, 300);
            SetDamage(12, 20);

            SetDamageType(ResistanceType.Physical, 70);
            SetDamageType(ResistanceType.Poison, 30);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 80, 90);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 60.0, 80.0);
            SetSkill(SkillName.Tactics, 70.0, 90.0);
            SetSkill(SkillName.Wrestling, 75.0, 95.0);

            Fame = 5000;
            Karma = -5000;
            VirtualArmor = 35;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);

            PackItem(new BogIronOre(Utility.RandomMinMax(4, 8)));
            PackItem(new ShadowforgedIngot(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.20)
                PackItem(new BogIronOre(Utility.RandomMinMax(1, 3)));
        }

        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 3;

        public VystiaMireElemental(Serial serial) : base(serial)
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
