using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a swamp drake corpse")]
    public class VystiaSwampDrake : BaseCreature
    {
        [Constructable]
        public VystiaSwampDrake() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {


            Name = "a swamp drake";
            Body = 61;
            Hue = 2073;
            BaseSoundID = 362;

            SetStr(280, 340);
            SetDex(80, 100);
            SetInt(80, 120);

            SetHits(300, 380);
            SetDamage(14, 22);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Poison, 40);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 70, 80);
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

            PackItem(new BogIronOre(Utility.RandomMinMax(5, 10)));
            PackItem(new ShadowforgedIngot(Utility.RandomMinMax(2, 5)));

            if (Utility.RandomDouble() < 0.12)
                PackItem(new VoidDust(Utility.RandomMinMax(1, 2)));
        }

        public override int Meat => 10;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 3;

        public VystiaSwampDrake(Serial serial) : base(serial)
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