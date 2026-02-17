using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a cloud serpent corpse")]
    public class VystiaCloudSerpent : BaseCreature
    {
        [Constructable]
        public VystiaCloudSerpent() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a cloud serpent";
            Body = 89;
            Hue = 1281;
            BaseSoundID = 219;

            SetStr(350, 420);
            SetDex(150, 180);
            SetInt(150, 200);

            SetHits(380, 460);
            SetDamage(14, 22);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Energy, 60);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 35, 45);
            SetResistance(ResistanceType.Energy, 70, 80);

            SetSkill(SkillName.EvalInt, 80.0, 100.0);
            SetSkill(SkillName.Magery, 80.0, 100.0);
            SetSkill(SkillName.MagicResist, 85.0, 105.0);
            SetSkill(SkillName.Tactics, 80.0, 100.0);
            SetSkill(SkillName.Wrestling, 80.0, 100.0);

            Fame = 10000;
            Karma = -10000;
            VirtualArmor = 50;

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 92.0;

            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);

            PackItem(new CrystalOre(Utility.RandomMinMax(6, 12)));
            PackItem(new CrystallineIngot(Utility.RandomMinMax(3, 6)));

            if (Utility.RandomDouble() < 0.20)
                PackItem(new StormCrystal(Utility.RandomMinMax(1, 3)));
            if (Utility.RandomDouble() < 0.12)
                PackItem(new StormCrystal(Utility.RandomMinMax(1, 2)));
        }

        public override int Meat => 8;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 4;

        public VystiaCloudSerpent(Serial serial) : base(serial)
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
