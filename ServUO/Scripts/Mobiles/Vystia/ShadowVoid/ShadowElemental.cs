using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a shadow elemental corpse")]
    public class ShadowElemental : BaseCreature
    {
        [Constructable]
        public ShadowElemental() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a shadow elemental";
            Body = 13;
            Hue = 1109;
            BaseSoundID = 655;

            SetStr(180, 230);
            SetDex(120, 150);
            SetInt(150, 200);

            SetHits(200, 260);
            SetDamage(12, 18);

            SetDamageType(ResistanceType.Physical, 30);
            SetDamageType(ResistanceType.Cold, 35);
            SetDamageType(ResistanceType.Energy, 35);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 25, 35);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 55, 65);

            SetSkill(SkillName.EvalInt, 80.0, 100.0);
            SetSkill(SkillName.Magery, 80.0, 100.0);
            SetSkill(SkillName.MagicResist, 75.0, 95.0);
            SetSkill(SkillName.Tactics, 65.0, 85.0);
            SetSkill(SkillName.Wrestling, 60.0, 80.0);
            SetSkill(SkillName.Meditation, 70.0, 90.0);

            Fame = 8000;
            Karma = -8000;
            VirtualArmor = 42;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.MedScrolls);

            PackItem(new ObsidianOre(Utility.RandomMinMax(3, 7)));

            if (Utility.RandomDouble() < 0.12)
                PackItem(new ShadowforgedIngot(Utility.RandomMinMax(1, 3)));

            if (Utility.RandomDouble() < 0.08)
                PackItem(new VoidDust());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x480, "The shadow elemental's touch chills your soul!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1109, 0, EffectLayer.Waist);
                defender.PlaySound(0x1FB);

                int damage = Utility.RandomMinMax(8, 14);
                AOS.Damage(defender, this, damage, 0, 0, 50, 0, 50);

                // Mana drain
                int manaDrain = Utility.RandomMinMax(15, 25);
                if (defender.Mana >= manaDrain)
                {
                    defender.Mana -= manaDrain;
                    Mana = Math.Min(ManaMax, Mana + manaDrain);
                    defender.SendMessage(0x480, "Your magical energy is drained!");
                }
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Darkness shroud
            if (Combatant != null && Utility.RandomDouble() < 0.025)
            {
                DoDarknessShroud();
            }
        }

        private void DoDarknessShroud()
        {
            if (Map == null)
                return;

            Say("*The shadow elemental summons darkness*");
            PlaySound(0x482);

            foreach (Mobile m in GetMobilesInRange(6))
            {
                if (m != this && CanBeHarmful(m))
                {
                    if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                    {
                        DoHarmful(m);
                        m.FixedParticles(0x3709, 10, 30, 5052, 1109, 0, EffectLayer.Head);
                        m.PlaySound(0x482);

                        int damage = Utility.RandomMinMax(15, 25);
                        AOS.Damage(m, this, damage, 0, 0, 50, 0, 50);

                        // Blind - DEX reduction
                        m.AddStatMod(new StatMod(StatType.Dex, "ShadowBlind", -25, TimeSpan.FromSeconds(10)));
                        m.SendMessage(0x480, "Darkness engulfs you!");
                    }
                }
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Regular;

        public ShadowElemental(Serial serial) : base(serial)
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
