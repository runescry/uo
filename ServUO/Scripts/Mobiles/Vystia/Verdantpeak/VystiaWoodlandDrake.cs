using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a woodland drake corpse")]
    public class VystiaWoodlandDrake : BaseCreature
    {
        [Constructable]
        public VystiaWoodlandDrake() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {


            Name = "a woodland drake";
            Body = 61;
            Hue = 2010;
            BaseSoundID = 362;

            SetStr(280, 340);
            SetDex(90, 120);
            SetInt(80, 120);

            SetHits(300, 380);
            SetDamage(14, 22);

            SetDamageType(ResistanceType.Physical, 70);
            SetDamageType(ResistanceType.Poison, 30);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 25, 35);
            SetResistance(ResistanceType.Cold, 35, 45);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 70.0, 90.0);
            SetSkill(SkillName.Tactics, 80.0, 100.0);
            SetSkill(SkillName.Wrestling, 80.0, 100.0);

            Fame = 8000;
            Karma = -8000;
            VirtualArmor = 45;

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 85.0;

            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);

            PackItem(new LivingOre(Utility.RandomMinMax(5, 10)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new LivingOre(Utility.RandomMinMax(1, 2)));
            if (Utility.RandomDouble() < 0.05)
                PackItem(new TreantHeart());
        }

        public override int Meat => 10;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 3;

        public VystiaWoodlandDrake(Serial serial) : base(serial)
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