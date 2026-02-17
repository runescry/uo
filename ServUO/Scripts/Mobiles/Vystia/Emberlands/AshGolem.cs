using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an ash golem corpse")]
    public class AshGolem : BaseCreature
    {
        [Constructable]
        public AshGolem() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an ash golem";
            Body = 752;
            Hue = 1358;
            BaseSoundID = 541;

            SetStr(280, 350);
            SetDex(60, 80);
            SetInt(60, 80);

            SetHits(280, 350);
            SetDamage(16, 24);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Fire, 60);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 85, 95);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 80.0, 100.0);
            SetSkill(SkillName.Tactics, 80.0, 100.0);
            SetSkill(SkillName.Wrestling, 80.0, 100.0);

            Fame = 8000;
            Karma = -8000;
            VirtualArmor = 55;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);

            PackItem(new MoltenOre(Utility.RandomMinMax(4, 8)));
            PackItem(new EmberforgedIngot(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new EverburningCoal());

            if (Utility.RandomDouble() < 0.05)
                PackItem(new LavaPearl());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x22, "The ash golem's smoldering fists burn you!");
                defender.FixedParticles(0x3709, 10, 30, 5052, 1358, 0, EffectLayer.Waist);
                defender.PlaySound(0x208);

                int damage = Utility.RandomMinMax(10, 18);
                AOS.Damage(defender, this, damage, 0, 100, 0, 0, 0);
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            // Ash cloud explosion
            if (Utility.RandomDouble() < 0.10 && from != null)
            {
                Say("*Ash explodes from the golem*");

                foreach (Mobile m in GetMobilesInRange(3))
                {
                    if (m != this && CanBeHarmful(m))
                    {
                        DoHarmful(m);
                        int damage = Utility.RandomMinMax(8, 15);
                        AOS.Damage(m, this, damage, 0, 100, 0, 0, 0);
                        m.FixedParticles(0x3709, 10, 15, 5052, 1358, 0, EffectLayer.Waist);
                    }
                }
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 3;

        public AshGolem(Serial serial) : base(serial)
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
