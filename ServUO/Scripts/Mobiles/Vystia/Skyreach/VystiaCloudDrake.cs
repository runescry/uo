using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a cloud drake corpse")]
    public class VystiaCloudDrake : BaseCreature
    {
        [Constructable]
        public VystiaCloudDrake() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {


            Name = "a cloud drake";
            Body = 61;
            Hue = 1281;
            BaseSoundID = 362;

            SetStr(300, 360);
            SetDex(100, 130);
            SetInt(100, 140);

            SetHits(320, 400);
            SetDamage(15, 23);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 60, 70);

            SetSkill(SkillName.MagicResist, 75.0, 95.0);
            SetSkill(SkillName.Tactics, 85.0, 105.0);
            SetSkill(SkillName.Wrestling, 85.0, 105.0);

            Fame = 9000;
            Karma = -9000;
            VirtualArmor = 48;

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 90.0;

            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average);

            PackItem(new CrystalOre(Utility.RandomMinMax(5, 10)));
            PackItem(new CrystallineIngot(Utility.RandomMinMax(2, 5)));

            if (Utility.RandomDouble() < 0.18)
                PackItem(new StormCrystal(Utility.RandomMinMax(1, 3)));
            if (Utility.RandomDouble() < 0.10)
                PackItem(new StormCrystal());
        }

        public override int Meat => 10;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 3;

        public VystiaCloudDrake(Serial serial) : base(serial)
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