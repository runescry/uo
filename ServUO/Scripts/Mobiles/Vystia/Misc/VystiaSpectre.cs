using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a vystia spectre remains")]
    public class VystiaSpectre : BaseCreature
    {
        [Constructable]
        public VystiaSpectre() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a vystia spectre";
            Body = 26;
            Hue = 0x4001;
            BaseSoundID = 0x482;

            SetStr(100, 140);
            SetDex(110, 140);
            SetInt(140, 180);

            SetHits(120, 160);
            SetDamage(9, 15);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Cold, 60);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 45, 55);

            SetSkill(SkillName.EvalInt, 75.0, 95.0);
            SetSkill(SkillName.Magery, 75.0, 95.0);
            SetSkill(SkillName.MagicResist, 70.0, 90.0);
            SetSkill(SkillName.Tactics, 55.0, 75.0);
            SetSkill(SkillName.Wrestling, 50.0, 70.0);
            SetSkill(SkillName.Meditation, 65.0, 85.0);

            Fame = 5000;
            Karma = -5000;
            VirtualArmor = 38;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.MedScrolls);

            if (Utility.RandomDouble() < 0.15)
                PackItem(new Bone(Utility.RandomMinMax(2, 4)));
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            // Chilling touch
            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x480, "The spectre's icy touch chills your soul!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1153, 0, EffectLayer.Waist);
                defender.PlaySound(0x1FB);

                int damage = Utility.RandomMinMax(8, 14);
                AOS.Damage(defender, this, damage, 0, 0, 100, 0, 0);

                // Mana drain
                int manaDrain = Utility.RandomMinMax(10, 20);
                if (defender.Mana >= manaDrain)
                {
                    defender.Mana -= manaDrain;
                    Mana = Math.Min(ManaMax, Mana + manaDrain);
                }
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Spectral wail
            if (Combatant != null && Utility.RandomDouble() < 0.02)
            {
                DoSpectralWail();
            }
        }

        private void DoSpectralWail()
        {
            if (Map == null)
                return;

            Say("*The spectre lets out an unearthly wail*");
            PlaySound(0x482);

            foreach (Mobile m in GetMobilesInRange(5))
            {
                if (m != this && CanBeHarmful(m))
                {
                    if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                    {
                        DoHarmful(m);
                        m.FixedParticles(0x374A, 10, 15, 5028, 1153, 0, EffectLayer.Head);

                        int damage = Utility.RandomMinMax(12, 20);
                        AOS.Damage(m, this, damage, 0, 0, 100, 0, 0);

                        // Fear - stat debuff
                        m.AddStatMod(new StatMod(StatType.Str, "SpectralFear_Str", -10, TimeSpan.FromSeconds(10)));
                        m.AddStatMod(new StatMod(StatType.Dex, "SpectralFear_Dex", -10, TimeSpan.FromSeconds(10)));
                        m.SendMessage(0x480, "The wail fills you with dread!");
                    }
                }
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool AlwaysMurderer => true;

        public VystiaSpectre(Serial serial) : base(serial)
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
