using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a dark wisp corpse")]
    public class VystiaDarkWisp : BaseCreature
    {
        [Constructable]
        public VystiaDarkWisp() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a dark wisp";
            Body = 58;
            Hue = 1109;
            BaseSoundID = 466;

            SetStr(80, 100);
            SetDex(150, 180);
            SetInt(150, 200);

            SetHits(80, 120);
            SetDamage(8, 14);

            SetDamageType(ResistanceType.Energy, 100);

            SetResistance(ResistanceType.Physical, 25, 35);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 80, 90);

            SetSkill(SkillName.EvalInt, 80.0, 100.0);
            SetSkill(SkillName.Magery, 80.0, 100.0);
            SetSkill(SkillName.MagicResist, 90.0, 110.0);
            SetSkill(SkillName.Tactics, 50.0, 70.0);
            SetSkill(SkillName.Wrestling, 40.0, 60.0);

            Fame = 4000;
            Karma = -4000;
            VirtualArmor = 25;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);

            PackItem(new ObsidianOre(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.08)
                PackItem(new VoidDust());
        }

        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 2;

        public VystiaDarkWisp(Serial serial) : base(serial)
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
