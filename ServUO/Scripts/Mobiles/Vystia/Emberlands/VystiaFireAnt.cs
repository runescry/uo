using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a fire ant corpse")]
    public class VystiaFireAnt : BaseCreature
    {
        [Constructable]
        public VystiaFireAnt() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a fire ant";
            Body = 738;
            Hue = 1358;
            BaseSoundID = 959;

            SetStr(120, 150);
            SetDex(100, 130);
            SetInt(20, 40);

            SetHits(130, 170);
            SetDamage(10, 16);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Fire, 40);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 80, 90);
            SetResistance(ResistanceType.Cold, 5, 10);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.MagicResist, 50.0, 70.0);
            SetSkill(SkillName.Tactics, 70.0, 90.0);
            SetSkill(SkillName.Wrestling, 70.0, 90.0);

            Fame = 2500;
            Karma = -2500;
            VirtualArmor = 30;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);

            PackItem(new EverburningCoal(Utility.RandomMinMax(1, 3)));
        }

        public override int Meat => 1;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 2;

        public VystiaFireAnt(Serial serial) : base(serial)
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
