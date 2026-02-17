using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a vystia skeleton corpse")]
    public class VystiaSkeleton : BaseCreature
    {
        [Constructable]
        public VystiaSkeleton() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a vystia skeleton";
            Body = 50;
            BaseSoundID = 0x48D;

            SetStr(80, 110);
            SetDex(70, 90);
            SetInt(30, 50);

            SetHits(70, 100);
            SetDamage(7, 13);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Cold, 40);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 15, 25);
            SetResistance(ResistanceType.Cold, 45, 55);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 45.0, 65.0);
            SetSkill(SkillName.Tactics, 55.0, 75.0);
            SetSkill(SkillName.Wrestling, 55.0, 75.0);

            Fame = 1500;
            Karma = -1500;
            VirtualArmor = 26;

            switch (Utility.Random(3))
            {
                case 0: AddItem(new BoneHarvester()); break;
                case 1: AddItem(new Longsword()); break;
                case 2: break; // unarmed
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Poor);
            AddLoot(LootPack.Meager);

            if (Utility.RandomDouble() < 0.30)
                PackItem(new Bone(Utility.RandomMinMax(1, 3)));
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool AlwaysMurderer => true;

        public VystiaSkeleton(Serial serial) : base(serial)
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
