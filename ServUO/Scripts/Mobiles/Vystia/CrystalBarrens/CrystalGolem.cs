using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a crystal golem corpse")]
    public class CrystalGolem : BaseCreature
    {
        [Constructable]
        public CrystalGolem() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a crystal golem";
            Body = 752;
            Hue = 1154;
            BaseSoundID = 541;

            SetStr(280, 350);
            SetDex(70, 90);
            SetInt(60, 80);

            SetHits(280, 360);
            SetDamage(16, 24);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 35, 45);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 85, 95);

            SetSkill(SkillName.MagicResist, 90.0, 110.0);
            SetSkill(SkillName.Tactics, 80.0, 100.0);
            SetSkill(SkillName.Wrestling, 80.0, 100.0);

            Fame = 9000;
            Karma = -9000;
            VirtualArmor = 55;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);

            PackItem(new CrystalOre(Utility.RandomMinMax(5, 10)));
            PackItem(new CrystallineIngot(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new PrismaticShard(Utility.RandomMinMax(1, 2)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new PrismaticShard());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x480, "The crystal golem's crystalline fists shock you!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1154, 0, EffectLayer.Waist);
                defender.PlaySound(0x1E2);

                int damage = Utility.RandomMinMax(12, 20);
                AOS.Damage(defender, this, damage, 0, 0, 0, 0, 100);
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            // Crystal shard explosion
            if (Utility.RandomDouble() < 0.12 && from != null)
            {
                Say("*Crystal shards explode outward*");

                foreach (Mobile m in GetMobilesInRange(3))
                {
                    if (m != this && CanBeHarmful(m))
                    {
                        DoHarmful(m);
                        int damage = Utility.RandomMinMax(8, 15);
                        AOS.Damage(m, this, damage, 0, 0, 0, 0, 100);
                        m.FixedParticles(0x374A, 10, 15, 5028, 1154, 0, EffectLayer.Waist);
                    }
                }
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 4;

        public CrystalGolem(Serial serial) : base(serial)
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
