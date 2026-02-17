using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a shadow demon corpse")]
    public class VystiaShadowDemon : BaseCreature
    {
        [Constructable]
        public VystiaShadowDemon() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a shadow demon";
            Body = 9;
            Hue = 1109;
            BaseSoundID = 357;

            SetStr(280, 340);
            SetDex(100, 130);
            SetInt(200, 260);

            SetHits(300, 380);
            SetDamage(16, 24);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 70, 80);

            SetSkill(SkillName.EvalInt, 90.0, 110.0);
            SetSkill(SkillName.Magery, 90.0, 110.0);
            SetSkill(SkillName.MagicResist, 95.0, 115.0);
            SetSkill(SkillName.Tactics, 85.0, 105.0);
            SetSkill(SkillName.Wrestling, 85.0, 105.0);

            Fame = 12000;
            Karma = -12000;
            VirtualArmor = 55;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.Rich);

            PackItem(new ObsidianOre(Utility.RandomMinMax(5, 10)));
            PackItem(new ShadowforgedIngot(Utility.RandomMinMax(3, 6)));

            if (Utility.RandomDouble() < 0.12)
                PackItem(new VoidDust(Utility.RandomMinMax(1, 2)));
            if (Utility.RandomDouble() < 0.08)
                PackItem(new VoidDust());
        }

        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 4;

        public VystiaShadowDemon(Serial serial) : base(serial)
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
