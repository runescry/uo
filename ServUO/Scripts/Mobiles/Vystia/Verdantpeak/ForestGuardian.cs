using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a forest guardian corpse")]
    public class ForestGuardian : BaseCreature
    {
        [Constructable]
        public ForestGuardian() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a forest guardian";
            Body = 301;
            Hue = 2010;
            BaseSoundID = 442;

            SetStr(250, 300);
            SetDex(80, 100);
            SetInt(100, 130);

            SetHits(200, 260);
            SetDamage(14, 22);

            SetDamageType(ResistanceType.Physical, 70);
            SetDamageType(ResistanceType.Poison, 30);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 15, 25);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 35, 45);

            SetSkill(SkillName.MagicResist, 70.0, 90.0);
            SetSkill(SkillName.Tactics, 80.0, 100.0);
            SetSkill(SkillName.Wrestling, 80.0, 100.0);

            Fame = 7000;
            Karma = -7000;
            VirtualArmor = 50;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average);

            PackItem(new LivingOre(Utility.RandomMinMax(3, 6)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new NatureforgedIngot(Utility.RandomMinMax(1, 2)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new LivingOre());

            if (Utility.RandomDouble() < 0.05)
                PackItem(new TreantHeart());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x3B2, "The forest guardian's thorny branches tear at you!");
                defender.FixedParticles(0x376A, 9, 32, 5030, 2010, 0, EffectLayer.Waist);
                defender.ApplyPoison(this, Poison.Regular);

                // Bleed DoT
                Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), 3, () =>
                {
                    if (defender != null && !defender.Deleted && defender.Alive)
                    {
                        AOS.Damage(defender, this, Utility.RandomMinMax(4, 8), 100, 0, 0, 0, 0);
                    }
                });
            }
        }

        public override Poison HitPoison => Poison.Regular;
        public override Poison PoisonImmune => Poison.Greater;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 3;

        public ForestGuardian(Serial serial) : base(serial)
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
