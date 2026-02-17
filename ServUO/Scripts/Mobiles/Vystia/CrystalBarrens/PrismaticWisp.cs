using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a prismatic wisp remains")]
    public class PrismaticWisp : BaseCreature
    {
        [Constructable]
        public PrismaticWisp() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a prismatic wisp";
            Body = 58;
            Hue = 1154;
            BaseSoundID = 466;

            SetStr(60, 80);
            SetDex(120, 150);
            SetInt(120, 150);

            SetHits(55, 75);
            SetDamage(6, 12);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Energy, 80);

            SetResistance(ResistanceType.Physical, 25, 35);
            SetResistance(ResistanceType.Fire, 35, 45);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 75, 85);

            SetSkill(SkillName.EvalInt, 65.0, 85.0);
            SetSkill(SkillName.Magery, 65.0, 85.0);
            SetSkill(SkillName.MagicResist, 60.0, 80.0);
            SetSkill(SkillName.Tactics, 45.0, 65.0);
            SetSkill(SkillName.Wrestling, 40.0, 60.0);

            Fame = 3500;
            Karma = -3500;
            VirtualArmor = 25;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.LowScrolls);

            PackItem(new CrystalOre(Utility.RandomMinMax(1, 3)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new PrismaticShard());

            if (Utility.RandomDouble() < 0.08)
                PackItem(new PrismaticShard());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x480, "The prismatic wisp's touch drains your energy!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1154, 0, EffectLayer.Waist);

                // Heavy mana drain
                if (defender is PlayerMobile pm)
                {
                    int manaDrain = Utility.RandomMinMax(15, 25);
                    pm.Mana = Math.Max(0, pm.Mana - manaDrain);
                }

                int damage = Utility.RandomMinMax(5, 10);
                AOS.Damage(defender, this, damage, 0, 0, 0, 0, 100);
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Prismatic flash - disorients
            if (Combatant != null && Utility.RandomDouble() < 0.02)
            {
                Say("*The prismatic wisp flashes with blinding light*");
                PlaySound(0x1E1);

                foreach (Mobile m in GetMobilesInRange(4))
                {
                    if (m != this && CanBeHarmful(m))
                    {
                        if (m is PlayerMobile pm)
                        {
                            pm.SendMessage(0x480, "The blinding flash disorients you!");
                            pm.FixedParticles(0x376A, 9, 32, 5030, 1154, 0, EffectLayer.Waist);
                            pm.AddStatMod(new StatMod(StatType.Dex, "PrismaticFlash", -20, TimeSpan.FromSeconds(6)));
                        }
                    }
                }
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Regular;

        public PrismaticWisp(Serial serial) : base(serial)
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
