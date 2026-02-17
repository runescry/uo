using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a mistwalker corpse")]
    public class Mistwalker : BaseCreature
    {
        [Constructable]
        public Mistwalker() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a mistwalker";
            Body = 154;
            Hue = 2073;
            BaseSoundID = 471;

            SetStr(150, 180);
            SetDex(100, 130);
            SetInt(120, 150);

            SetHits(130, 170);
            SetDamage(10, 16);

            SetDamageType(ResistanceType.Physical, 30);
            SetDamageType(ResistanceType.Cold, 40);
            SetDamageType(ResistanceType.Poison, 30);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 15, 25);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 35, 45);

            SetSkill(SkillName.EvalInt, 60.0, 80.0);
            SetSkill(SkillName.Magery, 60.0, 80.0);
            SetSkill(SkillName.MagicResist, 60.0, 80.0);
            SetSkill(SkillName.Tactics, 60.0, 80.0);
            SetSkill(SkillName.Wrestling, 60.0, 80.0);

            Fame = 5500;
            Karma = -5500;
            VirtualArmor = 35;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.MedScrolls);

            PackItem(new BogIronOre(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new ShadowforgedIngot());

            if (Utility.RandomDouble() < 0.05)
                PackItem(new VoidDust());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x3B2, "The mistwalker's touch chills your soul!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 2073, 0, EffectLayer.Waist);

                // Mana drain
                if (defender is PlayerMobile pm)
                {
                    int manaDrain = Utility.RandomMinMax(15, 30);
                    pm.Mana = Math.Max(0, pm.Mana - manaDrain);
                    pm.SendMessage(0x3B2, "Your mana is drained!");
                }

                // Cold damage
                int damage = Utility.RandomMinMax(8, 15);
                AOS.Damage(defender, this, damage, 0, 0, 100, 0, 0);
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Mist form - brief invisibility
            if (Combatant != null && Utility.RandomDouble() < 0.02 && !Hidden)
            {
                Say("*The mistwalker fades into the fog*");
                Hidden = true;
                FixedParticles(0x376A, 9, 32, 5030, 2073, 0, EffectLayer.Waist);

                Timer.DelayCall(TimeSpan.FromSeconds(3), () =>
                {
                    if (!Deleted)
                    {
                        Hidden = false;
                        FixedParticles(0x376A, 9, 32, 5030, 2073, 0, EffectLayer.Waist);
                    }
                });
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Greater;
        public override bool AlwaysMurderer => true;

        public Mistwalker(Serial serial) : base(serial)
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
