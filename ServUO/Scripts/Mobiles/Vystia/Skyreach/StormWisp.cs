using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a storm wisp remains")]
    public class StormWisp : BaseCreature
    {
        [Constructable]
        public StormWisp() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a storm wisp";
            Body = 165;
            Hue = 1281;
            BaseSoundID = 466;

            SetStr(80, 100);
            SetDex(120, 150);
            SetInt(120, 150);

            SetHits(70, 100);
            SetDamage(8, 14);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Energy, 80);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 25, 35);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 35, 45);
            SetResistance(ResistanceType.Energy, 75, 85);

            SetSkill(SkillName.EvalInt, 65.0, 85.0);
            SetSkill(SkillName.Magery, 65.0, 85.0);
            SetSkill(SkillName.MagicResist, 60.0, 80.0);
            SetSkill(SkillName.Tactics, 50.0, 70.0);
            SetSkill(SkillName.Wrestling, 50.0, 70.0);

            Fame = 4000;
            Karma = -4000;
            VirtualArmor = 28;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.MedScrolls);

            PackItem(new CrystalOre(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new StormCrystal());

            if (Utility.RandomDouble() < 0.10)
                PackItem(new StormCrystal());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.35)
            {
                defender.SendMessage(0x480, "The storm wisp shocks you!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1281, 0, EffectLayer.Waist);
                defender.PlaySound(0x1E1);

                int damage = Utility.RandomMinMax(8, 15);
                AOS.Damage(defender, this, damage, 0, 0, 0, 0, 100);
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Regular;

        public StormWisp(Serial serial) : base(serial)
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
