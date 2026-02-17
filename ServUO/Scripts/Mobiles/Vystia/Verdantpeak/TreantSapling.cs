using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a treant sapling remains")]
    public class TreantSapling : BaseCreature
    {
        [Constructable]
        public TreantSapling() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a treant sapling";
            Body = 301;
            Hue = 2010;
            BaseSoundID = 442;

            SetStr(100, 130);
            SetDex(60, 80);
            SetInt(50, 70);

            SetHits(80, 110);
            SetDamage(8, 14);

            SetDamageType(ResistanceType.Physical, 70);
            SetDamageType(ResistanceType.Poison, 30);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 55, 65);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 40.0, 60.0);
            SetSkill(SkillName.Tactics, 50.0, 70.0);
            SetSkill(SkillName.Wrestling, 50.0, 70.0);

            Fame = 3000;
            Karma = -3000;
            VirtualArmor = 28;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);

            PackItem(new LivingOre(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new LivingOre());

            if (Utility.RandomDouble() < 0.05)
                PackItem(new NatureforgedIngot());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x3B2, "The treant sapling's branches scrape you!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 2010, 0, EffectLayer.Waist);

                int damage = Utility.RandomMinMax(3, 6);
                AOS.Damage(defender, this, damage, 0, 0, 0, 100, 0);
            }
        }

        public override Poison PoisonImmune => Poison.Regular;

        public TreantSapling(Serial serial) : base(serial)
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
