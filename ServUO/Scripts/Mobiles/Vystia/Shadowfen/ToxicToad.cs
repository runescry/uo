using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a toxic toad corpse")]
    public class ToxicToad : BaseCreature
    {
        [Constructable]
        public ToxicToad() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a toxic toad";
            Body = 80;
            Hue = 2073;
            BaseSoundID = 0x26B;

            SetStr(100, 130);
            SetDex(80, 100);
            SetInt(30, 50);

            SetHits(80, 110);
            SetDamage(8, 14);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Poison, 60);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 25, 35);
            SetResistance(ResistanceType.Poison, 80, 90);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.Poisoning, 80.0, 100.0);
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

            if (Utility.RandomDouble() < 0.15)
                PackItem(new BogIronOre(Utility.RandomMinMax(1, 2)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new BogIronOre());

            if (Utility.RandomDouble() < 0.05)
                PackItem(new VoidDust());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.35)
            {
                defender.SendMessage(0x3B2, "The toxic toad secretes poison onto you!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 68, 0, EffectLayer.Waist);
                defender.ApplyPoison(this, Poison.Greater);
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            // Poison spray when hit
            if (Utility.RandomDouble() < 0.15 && from != null)
            {
                Say("*The toxic toad sprays venom*");

                foreach (Mobile m in GetMobilesInRange(2))
                {
                    if (m != this && CanBeHarmful(m))
                    {
                        DoHarmful(m);
                        m.ApplyPoison(this, Poison.Regular);
                        m.FixedParticles(0x374A, 10, 15, 5028, 68, 0, EffectLayer.Waist);
                    }
                }
            }
        }

        public override Poison HitPoison => Poison.Greater;
        public override Poison PoisonImmune => Poison.Lethal;

        public ToxicToad(Serial serial) : base(serial)
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
