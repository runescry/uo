using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a desert scorpion corpse")]
    public class VystiaDesertScorpion : BaseCreature
    {
        [Constructable]
        public VystiaDesertScorpion() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a desert scorpion";
            Body = 48;
            Hue = 1719;
            BaseSoundID = 397;

            SetStr(120, 150);
            SetDex(80, 100);
            SetInt(20, 40);

            SetHits(130, 170);
            SetDamage(10, 18);

            SetDamageType(ResistanceType.Physical, 80);
            SetDamageType(ResistanceType.Poison, 20);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.MagicResist, 50.0, 70.0);
            SetSkill(SkillName.Tactics, 70.0, 90.0);
            SetSkill(SkillName.Wrestling, 70.0, 90.0);
            SetSkill(SkillName.Poisoning, 80.0, 100.0);

            Fame = 2800;
            Karma = -2800;
            VirtualArmor = 30;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 55.0;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);

            PackItem(new SandstoneOre(Utility.RandomMinMax(1, 2)));
        }

        public override Poison PoisonImmune => Poison.Greater;
        public override Poison HitPoison => Poison.Regular;

        public override int Meat => 1;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 2;

        public VystiaDesertScorpion(Serial serial) : base(serial)
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
