using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a kelp horror corpse")]
    public class KelpHorror : BaseCreature
    {
        [Constructable]
        public KelpHorror() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a kelp horror";
            Body = 0x50;
            Hue = 1365;
            BaseSoundID = 684;

            SetStr(220, 280);
            SetDex(70, 90);
            SetInt(80, 110);

            SetHits(240, 300);
            SetDamage(14, 22);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Cold, 30);
            SetDamageType(ResistanceType.Poison, 20);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 15, 25);
            SetResistance(ResistanceType.Cold, 55, 65);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 35, 45);

            SetSkill(SkillName.MagicResist, 70.0, 90.0);
            SetSkill(SkillName.Tactics, 80.0, 100.0);
            SetSkill(SkillName.Wrestling, 85.0, 105.0);

            Fame = 8000;
            Karma = -8000;
            VirtualArmor = 45;

            CanSwim = true;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);

            PackItem(new ObsidianOre(Utility.RandomMinMax(4, 8)));

            if (Utility.RandomDouble() < 0.12)
                PackItem(new VoidforgedIngot(Utility.RandomMinMax(1, 2)));

            if (Utility.RandomDouble() < 0.08)
                PackItem(new ObsidianOre());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.35)
            {
                defender.SendMessage(0x480, "The kelp horror's tendrils entangle you!");
                defender.FixedParticles(0x376A, 9, 32, 5007, 1365, 0, EffectLayer.Waist);
                defender.PlaySound(0x1E4);

                // Entangle - freeze and DoT
                defender.Freeze(TimeSpan.FromSeconds(2.5));

                // Constriction damage over time
                int ticks = 3;
                for (int i = 0; i < ticks; i++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(0.8 * (i + 1)), () =>
                    {
                        if (defender != null && !defender.Deleted && defender.Alive)
                        {
                            int damage = Utility.RandomMinMax(6, 10);
                            AOS.Damage(defender, this, damage, 100, 0, 0, 0, 0);
                        }
                    });
                }
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Kelp spore cloud
            if (Combatant != null && Utility.RandomDouble() < 0.025)
            {
                DoSporeCloud();
            }
        }

        private void DoSporeCloud()
        {
            if (Map == null)
                return;

            Say("*The kelp horror releases toxic spores*");
            PlaySound(0x026);

            foreach (Mobile m in GetMobilesInRange(4))
            {
                if (m != this && CanBeHarmful(m))
                {
                    if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                    {
                        DoHarmful(m);
                        m.FixedParticles(0x3709, 10, 30, 5052, 1365, 0, EffectLayer.Head);
                        m.PlaySound(0x026);

                        int damage = Utility.RandomMinMax(12, 20);
                        AOS.Damage(m, this, damage, 0, 0, 0, 100, 0);

                        // Apply poison
                        m.ApplyPoison(this, Poison.Greater);
                        m.SendMessage(0x480, "The toxic spores poison you!");
                    }
                }
            }
        }

        public override Poison PoisonImmune => Poison.Greater;
        public override bool AlwaysMurderer => true;

        public KelpHorror(Serial serial) : base(serial)
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
