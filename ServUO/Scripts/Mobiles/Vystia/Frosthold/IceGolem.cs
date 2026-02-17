using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an ice golem corpse")]
    public class IceGolem : BaseCreature
    {
        [Constructable]
        public IceGolem() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an ice golem";
            Body = 752;
            Hue = 1152;
            BaseSoundID = 541;

            SetStr(251, 350);
            SetDex(76, 100);
            SetInt(50, 80);

            SetHits(250, 350);
            SetDamage(14, 20);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Cold, 60);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 5, 15);
            SetResistance(ResistanceType.Cold, 80, 90);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 80.0, 100.0);
            SetSkill(SkillName.Tactics, 80.0, 100.0);
            SetSkill(SkillName.Wrestling, 80.0, 100.0);

            Fame = 7000;
            Karma = -7000;
            VirtualArmor = 50;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);

            PackItem(new FrozenOre(Utility.RandomMinMax(4, 8)));
            PackItem(new FrostforgedIngot(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new IceCrystal());

            if (Utility.RandomDouble() < 0.05)
                PackItem(new EternalIce());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x480, "The ice golem's frozen fist chills you!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1152, 0, EffectLayer.Waist);
                defender.PlaySound(0x204);

                // Slow effect
                defender.AddStatMod(new StatMod(StatType.Dex, "IceGolemChill", -15, TimeSpan.FromSeconds(6)));

                int damage = Utility.RandomMinMax(8, 12);
                AOS.Damage(defender, this, damage, 0, 0, 100, 0, 0);
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            // Chance to create ice shard explosion when damaged
            if (Utility.RandomDouble() < 0.10 && from != null)
            {
                Say("*Ice shards explode outward*");

                foreach (Mobile m in GetMobilesInRange(3))
                {
                    if (m != this && CanBeHarmful(m))
                    {
                        DoHarmful(m);
                        int damage = Utility.RandomMinMax(5, 10);
                        AOS.Damage(m, this, damage, 0, 0, 100, 0, 0);
                        m.FixedParticles(0x374A, 10, 15, 5028, 1152, 0, EffectLayer.Waist);
                    }
                }
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 3;

        public IceGolem(Serial serial) : base(serial)
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
