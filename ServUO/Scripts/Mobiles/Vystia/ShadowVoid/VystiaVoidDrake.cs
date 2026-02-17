using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a void drake corpse")]
    public class VystiaVoidDrake : BaseCreature
    {
        [Constructable]
        public VystiaVoidDrake() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {


            Name = "a void drake";
            Body = 61;
            Hue = 1109;
            BaseSoundID = 362;

            SetStr(340, 400);
            SetDex(100, 130);
            SetInt(140, 180);

            SetHits(360, 440);
            SetDamage(16, 24);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Energy, 60);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 35, 45);
            SetResistance(ResistanceType.Cold, 45, 55);
            SetResistance(ResistanceType.Poison, 45, 55);
            SetResistance(ResistanceType.Energy, 70, 80);

            SetSkill(SkillName.EvalInt, 80.0, 100.0);
            SetSkill(SkillName.Magery, 80.0, 100.0);
            SetSkill(SkillName.MagicResist, 90.0, 110.0);
            SetSkill(SkillName.Tactics, 85.0, 105.0);
            SetSkill(SkillName.Wrestling, 85.0, 105.0);

            Fame = 11000;
            Karma = -11000;
            VirtualArmor = 52;

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 95.0;

            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);

            PackItem(new ObsidianOre(Utility.RandomMinMax(6, 12)));
            PackItem(new ShadowforgedIngot(Utility.RandomMinMax(3, 6)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new VoidDust(Utility.RandomMinMax(1, 2)));
            if (Utility.RandomDouble() < 0.10)
                PackItem(new VoidDust());
        }

        public override int Meat => 10;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 4;

        public VystiaVoidDrake(Serial serial) : base(serial)
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