using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a will o' wisp remains")]
    public class SwampWisp : BaseCreature
    {
        [Constructable]
        public SwampWisp() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a will o' wisp";
            Body = 58;
            Hue = 2073;
            BaseSoundID = 466;

            SetStr(60, 80);
            SetDex(120, 150);
            SetInt(100, 130);

            SetHits(50, 70);
            SetDamage(6, 12);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Energy, 50);
            SetDamageType(ResistanceType.Poison, 30);

            SetResistance(ResistanceType.Physical, 20, 30);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 60, 70);

            SetSkill(SkillName.EvalInt, 60.0, 80.0);
            SetSkill(SkillName.Magery, 60.0, 80.0);
            SetSkill(SkillName.MagicResist, 50.0, 70.0);
            SetSkill(SkillName.Tactics, 40.0, 60.0);
            SetSkill(SkillName.Wrestling, 40.0, 60.0);

            Fame = 3000;
            Karma = -3000;
            VirtualArmor = 25;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.LowScrolls);

            if (Utility.RandomDouble() < 0.20)
                PackItem(new VoidDust());

            if (Utility.RandomDouble() < 0.10)
                PackItem(new BogIronOre());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x3B2, "The will o' wisp's touch drains your energy!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 2073, 0, EffectLayer.Waist);

                // Mana drain
                if (defender is PlayerMobile pm)
                {
                    int manaDrain = Utility.RandomMinMax(10, 20);
                    pm.Mana = Math.Max(0, pm.Mana - manaDrain);
                }

                // Energy damage
                int damage = Utility.RandomMinMax(5, 10);
                AOS.Damage(defender, this, damage, 0, 0, 0, 0, 100);
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Confusing lights - disorient
            if (Combatant != null && Utility.RandomDouble() < 0.02)
            {
                Mobile target = Combatant as Mobile;
                if (target != null && target is PlayerMobile pm)
                {
                    pm.SendMessage(0x3B2, "The wisp's lights disorient you!");
                    pm.FixedParticles(0x376A, 9, 32, 5030, 2073, 0, EffectLayer.Waist);
                    pm.AddStatMod(new StatMod(StatType.Dex, "WispConfusion", -20, TimeSpan.FromSeconds(8)));
                }
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Regular;

        public SwampWisp(Serial serial) : base(serial)
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
