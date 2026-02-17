using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a wild horse corpse")]
    public class WildHorse : BaseCreature
    {
        [Constructable]
        public WildHorse() : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a wild horse";
            Body = 0xCC;
            Hue = Utility.RandomList(0, 1109, 1150, 1175);
            BaseSoundID = 0xA8;

            SetStr(80, 110);
            SetDex(100, 130);
            SetInt(30, 50);

            SetHits(70, 100);
            SetDamage(6, 12);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 25, 35);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 15, 25);
            SetResistance(ResistanceType.Poison, 15, 25);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.MagicResist, 35.0, 55.0);
            SetSkill(SkillName.Tactics, 45.0, 65.0);
            SetSkill(SkillName.Wrestling, 45.0, 65.0);

            Fame = 1000;
            Karma = 0;
            VirtualArmor = 22;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 55.0;
        }

        public override void GenerateLoot()
        {
            // Wild horses don't drop loot
        }

        public override int Meat => 3;
        public override int Hides => 10;
        public override FoodType FavoriteFood => FoodType.FruitsAndVegies | FoodType.GrainsAndHay;

        public WildHorse(Serial serial) : base(serial)
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
