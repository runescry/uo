using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a bog beast corpse")]
    public class BogBeast : BaseCreature
    {
        [Constructable]
        public BogBeast() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a bog beast";
            Body = 72;
            Hue = 2073;
            BaseSoundID = 655;

            SetStr(180, 220);
            SetDex(60, 80);
            SetInt(50, 70);

            SetHits(160, 200);
            SetDamage(12, 18);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Poison, 40);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 75, 85);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 50.0, 70.0);
            SetSkill(SkillName.Tactics, 70.0, 90.0);
            SetSkill(SkillName.Wrestling, 70.0, 90.0);

            Fame = 5000;
            Karma = -5000;
            VirtualArmor = 40;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Rich);

            PackItem(new BogIronOre(Utility.RandomMinMax(2, 5)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new ShadowforgedIngot(Utility.RandomMinMax(1, 2)));

            if (Utility.RandomDouble() < 0.05)
                PackItem(new BogIronOre());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x3B2, "The bog beast's fetid grasp poisons you!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 2073, 0, EffectLayer.Waist);
                defender.ApplyPoison(this, Poison.Greater);

                // Slow effect
                defender.AddStatMod(new StatMod(StatType.Dex, "BogBeastSlow", -15, TimeSpan.FromSeconds(8)));
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            // Poison cloud when damaged
            if (Utility.RandomDouble() < 0.10 && from != null)
            {
                Say("*Toxic spores burst from the bog beast*");

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
        public override bool AlwaysMurderer => true;

        public BogBeast(Serial serial) : base(serial)
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
