using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a deep sea serpent corpse")]
    public class VystiaDeepSeaSerpent : BaseCreature
    {
        [Constructable]
        public VystiaDeepSeaSerpent() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a deep sea serpent";
            Body = 150;
            Hue = 1365;
            BaseSoundID = 447;

            SetStr(350, 420);
            SetDex(100, 130);
            SetInt(80, 100);

            SetHits(400, 500);
            SetDamage(18, 28);

            SetDamageType(ResistanceType.Physical, 70);
            SetDamageType(ResistanceType.Cold, 30);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 80.0, 100.0);
            SetSkill(SkillName.Tactics, 90.0, 110.0);
            SetSkill(SkillName.Wrestling, 90.0, 110.0);

            Fame = 11000;
            Karma = -11000;
            VirtualArmor = 50;

            CanSwim = true;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);

            PackItem(new ObsidianOre(Utility.RandomMinMax(5, 10)));
            PackItem(new VoidforgedIngot(Utility.RandomMinMax(3, 6)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new ObsidianOre(Utility.RandomMinMax(1, 2)));
            if (Utility.RandomDouble() < 0.08)
                PackItem(new VoidforgedIngot());
        }

        public override int Meat => 10;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 4;

        public VystiaDeepSeaSerpent(Serial serial) : base(serial)
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
