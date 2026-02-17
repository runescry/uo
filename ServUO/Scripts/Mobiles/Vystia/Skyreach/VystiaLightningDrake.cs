using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a lightning drake corpse")]
    public class VystiaLightningDrake : BaseCreature
    {
        [Constructable]
        public VystiaLightningDrake() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {


            Name = "a lightning drake";
            Body = 61;
            Hue = 1281;
            BaseSoundID = 362;

            SetStr(320, 380);
            SetDex(120, 150);
            SetInt(120, 160);

            SetHits(340, 420);
            SetDamage(16, 24);

            SetDamageType(ResistanceType.Physical, 30);
            SetDamageType(ResistanceType.Energy, 70);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 25, 35);
            SetResistance(ResistanceType.Cold, 35, 45);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 75, 85);

            SetSkill(SkillName.EvalInt, 75.0, 95.0);
            SetSkill(SkillName.Magery, 75.0, 95.0);
            SetSkill(SkillName.MagicResist, 85.0, 105.0);
            SetSkill(SkillName.Tactics, 85.0, 105.0);
            SetSkill(SkillName.Wrestling, 85.0, 105.0);

            Fame = 10000;
            Karma = -10000;
            VirtualArmor = 50;

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 93.0;

            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);

            PackItem(new CrystalOre(Utility.RandomMinMax(6, 12)));
            PackItem(new CrystallineIngot(Utility.RandomMinMax(3, 6)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new StormCrystal(Utility.RandomMinMax(1, 2)));
        }

        public override int Meat => 10;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 4;

        public VystiaLightningDrake(Serial serial) : base(serial)
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