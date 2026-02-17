using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a thorn vine remains")]
    public class ThornVine : BaseCreature
    {
        [Constructable]
        public ThornVine() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a thorn vine";
            Body = 8;
            Hue = 2010;
            BaseSoundID = 352;

            SetStr(120, 150);
            SetDex(70, 90);
            SetInt(40, 60);

            SetHits(100, 140);
            SetDamage(10, 16);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Poison, 40);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 65, 75);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 40.0, 60.0);
            SetSkill(SkillName.Tactics, 60.0, 80.0);
            SetSkill(SkillName.Wrestling, 60.0, 80.0);

            Fame = 3500;
            Karma = -3500;
            VirtualArmor = 30;
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

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x3B2, "The thorn vine's thorns inject poison!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 68, 0, EffectLayer.Waist);
                defender.ApplyPoison(this, Poison.Regular);

                // Bleed effect
                Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), 3, () =>
                {
                    if (defender != null && !defender.Deleted && defender.Alive)
                    {
                        AOS.Damage(defender, this, Utility.RandomMinMax(3, 6), 100, 0, 0, 0, 0);
                    }
                });
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            // Entangle attacker
            if (Utility.RandomDouble() < 0.15 && from != null)
            {
                from.SendMessage(0x3B2, "The thorn vine entangles you!");
                from.Freeze(TimeSpan.FromSeconds(1.5));
                from.FixedParticles(0x376A, 9, 32, 5030, 2010, 0, EffectLayer.Waist);
            }
        }

        public override Poison HitPoison => Poison.Regular;
        public override Poison PoisonImmune => Poison.Greater;

        public ThornVine(Serial serial) : base(serial)
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
