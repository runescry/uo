using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a shadow bogling corpse")]
    public class VystiaShadowBogling : BaseCreature
    {
        [Constructable]
        public VystiaShadowBogling() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a shadow bogling";
            Body = 779;
            Hue = 2073;
            BaseSoundID = 422;

            SetStr(60, 80);
            SetDex(60, 80);
            SetInt(20, 40);

            SetHits(70, 100);
            SetDamage(5, 10);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 25, 35);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 15, 25);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.MagicResist, 35.0, 55.0);
            SetSkill(SkillName.Tactics, 45.0, 65.0);
            SetSkill(SkillName.Wrestling, 40.0, 60.0);

            Fame = 450;
            Karma = -450;
            VirtualArmor = 18;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);

            PackItem(new BogIronOre(Utility.RandomMinMax(1, 2)));
        }

        public override int Meat => 1;
        public override bool AlwaysMurderer => true;

        public VystiaShadowBogling(Serial serial) : base(serial)
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
