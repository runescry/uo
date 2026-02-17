using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a lava elemental corpse")]
    public class VystiaLavaElemental : BaseCreature
    {
        [Constructable]
        public VystiaLavaElemental() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a lava elemental";
            Body = 15;
            Hue = 1358;
            BaseSoundID = 838;

            SetStr(280, 340);
            SetDex(80, 100);
            SetInt(150, 200);

            SetHits(300, 380);
            SetDamage(16, 24);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Fire, 60);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 90, 100);
            SetResistance(ResistanceType.Cold, 0, 5);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.EvalInt, 80.0, 100.0);
            SetSkill(SkillName.Magery, 80.0, 100.0);
            SetSkill(SkillName.MagicResist, 90.0, 110.0);
            SetSkill(SkillName.Tactics, 80.0, 100.0);
            SetSkill(SkillName.Wrestling, 80.0, 100.0);

            Fame = 10000;
            Karma = -10000;
            VirtualArmor = 50;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);

            PackItem(new MoltenOre(Utility.RandomMinMax(6, 12)));
            PackItem(new EmberforgedIngot(Utility.RandomMinMax(3, 6)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new EverburningCoal(Utility.RandomMinMax(1, 2)));
            if (Utility.RandomDouble() < 0.08)
                PackItem(new LavaPearl());
        }

        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 4;

        public VystiaLavaElemental(Serial serial) : base(serial)
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
