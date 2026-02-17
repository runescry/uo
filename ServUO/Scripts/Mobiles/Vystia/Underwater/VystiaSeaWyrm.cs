using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a sea wyrm corpse")]
    public class VystiaSeaWyrm : BaseCreature
    {
        [Constructable]
        public VystiaSeaWyrm() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {


            Name = "a sea wyrm";
            Body = 150;
            Hue = 1365;
            BaseSoundID = 447;

            SetStr(450, 550);
            SetDex(100, 130);
            SetInt(180, 240);

            SetHits(500, 620);
            SetDamage(20, 30);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Cold, 50);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 80, 90);
            SetResistance(ResistanceType.Poison, 45, 55);
            SetResistance(ResistanceType.Energy, 45, 55);

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

            CanSwim = true;

            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.Rich);

            PackItem(new ObsidianOre(Utility.RandomMinMax(8, 15)));
            PackItem(new VoidforgedIngot(Utility.RandomMinMax(4, 8)));

            if (Utility.RandomDouble() < 0.18)
                PackItem(new ObsidianOre(Utility.RandomMinMax(1, 3)));
            if (Utility.RandomDouble() < 0.12)
                PackItem(new VoidforgedIngot(Utility.RandomMinMax(1, 2)));
        }

        public override int Meat => 15;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 4;

        public VystiaSeaWyrm(Serial serial) : base(serial)
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