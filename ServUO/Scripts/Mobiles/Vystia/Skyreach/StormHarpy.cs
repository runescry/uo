using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a storm harpy corpse")]
    public class StormHarpy : BaseCreature
    {
        [Constructable]
        public StormHarpy() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a storm harpy";
            Body = 30;
            Hue = 1281;
            BaseSoundID = 402;

            SetStr(120, 150);
            SetDex(130, 160);
            SetInt(100, 130);

            SetHits(100, 140);
            SetDamage(10, 16);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Energy, 60);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 25, 35);
            SetResistance(ResistanceType.Energy, 70, 80);

            SetSkill(SkillName.EvalInt, 60.0, 80.0);
            SetSkill(SkillName.Magery, 60.0, 80.0);
            SetSkill(SkillName.MagicResist, 55.0, 75.0);
            SetSkill(SkillName.Tactics, 65.0, 85.0);
            SetSkill(SkillName.Wrestling, 65.0, 85.0);

            Fame = 5000;
            Karma = -5000;
            VirtualArmor = 35;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Rich);

            PackItem(new CrystalOre(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new CrystallineIngot());

            if (Utility.RandomDouble() < 0.05)
                PackItem(new StormCrystal());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x480, "The storm harpy shocks you with lightning!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1281, 0, EffectLayer.Waist);
                defender.PlaySound(0x1E1);

                int damage = Utility.RandomMinMax(8, 15);
                AOS.Damage(defender, this, damage, 0, 0, 0, 0, 100);
            }
        }

        public override int Feathers => 30;
        public override bool AlwaysMurderer => true;

        public StormHarpy(Serial serial) : base(serial)
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
