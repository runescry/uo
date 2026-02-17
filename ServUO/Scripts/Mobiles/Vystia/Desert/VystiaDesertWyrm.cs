using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a desert wyrm corpse")]
    public class VystiaDesertWyrm : BaseCreature
    {
        [Constructable]
        public VystiaDesertWyrm() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {


            Name = "a desert wyrm";
            Body = 12;
            Hue = 1719;
            BaseSoundID = 362;

            SetStr(450, 550);
            SetDex(100, 130);
            SetInt(180, 240);

            SetHits(500, 620);
            SetDamage(20, 30);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Fire, 50);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 80, 90);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.EvalInt, 85.0, 105.0);
            SetSkill(SkillName.Magery, 85.0, 105.0);
            SetSkill(SkillName.MagicResist, 95.0, 115.0);
            SetSkill(SkillName.Tactics, 95.0, 115.0);
            SetSkill(SkillName.Wrestling, 95.0, 115.0);

            Fame = 16000;
            Karma = -16000;
            VirtualArmor = 60;

            Tamable = true;
            ControlSlots = 4;
            MinTameSkill = 100.0;

            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.Rich);

            PackItem(new SandstoneOre(Utility.RandomMinMax(8, 15)));
            PackItem(new SunforgedIngot(Utility.RandomMinMax(4, 8)));

            if (Utility.RandomDouble() < 0.18)
                PackItem(new SunforgedIngot(Utility.RandomMinMax(1, 2)));
        }

        public override int Meat => 15;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 4;

        public VystiaDesertWyrm(Serial serial) : base(serial)
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