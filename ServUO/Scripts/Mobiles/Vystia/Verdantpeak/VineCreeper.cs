using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a vine creeper corpse")]
    public class VineCreeper : BaseCreature
    {
        [Constructable]
        public VineCreeper() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a vine creeper";
            Body = 51;
            Hue = 2010;
            BaseSoundID = 456;

            SetStr(150, 180);
            SetDex(90, 120);
            SetInt(50, 70);

            SetHits(120, 160);
            SetDamage(10, 16);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Poison, 50);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.Poisoning, 70.0, 90.0);
            SetSkill(SkillName.MagicResist, 50.0, 70.0);
            SetSkill(SkillName.Tactics, 60.0, 80.0);
            SetSkill(SkillName.Wrestling, 60.0, 80.0);
            SetSkill(SkillName.Hiding, 70.0, 90.0);

            Fame = 4000;
            Karma = -4000;
            VirtualArmor = 32;
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
                defender.SendMessage(0x3B2, "The vine creeper constricts you!");
                defender.FixedParticles(0x376A, 9, 32, 5030, 2010, 0, EffectLayer.Waist);

                // Constrict - freeze and DoT
                defender.Freeze(TimeSpan.FromSeconds(2));
                defender.ApplyPoison(this, Poison.Regular);

                Timer.DelayCall(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5), 4, () =>
                {
                    if (defender != null && !defender.Deleted && defender.Alive)
                    {
                        AOS.Damage(defender, this, Utility.RandomMinMax(4, 8), 50, 0, 0, 50, 0);
                    }
                });
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Ambush from hiding
            if (Combatant != null && !Hidden && Utility.RandomDouble() < 0.02)
            {
                Hidden = true;
                Say("*The vine creeper blends into the foliage*");

                Timer.DelayCall(TimeSpan.FromSeconds(2), () =>
                {
                    if (!Deleted && Combatant != null)
                    {
                        Mobile target = Combatant as Mobile;
                        if (target != null)
                        {
                            Hidden = false;
                            DoHarmful(target);
                            int damage = Utility.RandomMinMax(15, 25);
                            AOS.Damage(target, this, damage, 50, 0, 0, 50, 0);
                            target.SendMessage(0x3B2, "The vine creeper ambushes you!");
                        }
                    }
                });
            }
        }

        public override Poison HitPoison => Poison.Regular;
        public override Poison PoisonImmune => Poison.Greater;

        public VineCreeper(Serial serial) : base(serial)
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
