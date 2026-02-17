using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a nimbus wraith remains")]
    public class NimbusWraith : BaseCreature
    {
        [Constructable]
        public NimbusWraith() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a nimbus wraith";
            Body = 26;
            Hue = 1281;
            BaseSoundID = 0x482;

            SetStr(120, 150);
            SetDex(130, 160);
            SetInt(150, 200);

            SetHits(140, 180);
            SetDamage(10, 16);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Cold, 40);
            SetDamageType(ResistanceType.Energy, 40);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 70, 80);

            SetSkill(SkillName.EvalInt, 85.0, 105.0);
            SetSkill(SkillName.Magery, 85.0, 105.0);
            SetSkill(SkillName.MagicResist, 80.0, 100.0);
            SetSkill(SkillName.Tactics, 60.0, 80.0);
            SetSkill(SkillName.Wrestling, 55.0, 75.0);
            SetSkill(SkillName.Meditation, 70.0, 90.0);

            Fame = 8000;
            Karma = -8000;
            VirtualArmor = 38;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.MedScrolls, 2);

            PackItem(new CrystalOre(Utility.RandomMinMax(3, 6)));

            if (Utility.RandomDouble() < 0.18)
                PackItem(new StormCrystal());

            if (Utility.RandomDouble() < 0.12)
                PackItem(new StormCrystal());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x480, "The nimbus wraith's icy touch chills your soul!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1281, 0, EffectLayer.Waist);
                defender.PlaySound(0x1FB);

                int damage = Utility.RandomMinMax(8, 14);
                AOS.Damage(defender, this, damage, 0, 0, 50, 0, 50);

                // Mana drain
                int manaDrain = Utility.RandomMinMax(15, 25);
                if (defender.Mana >= manaDrain)
                {
                    defender.Mana -= manaDrain;
                    Mana = Math.Min(ManaMax, Mana + manaDrain);
                    defender.SendMessage(0x480, "Your mana is drained by the wraith!");
                }
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Chilling mist attack
            if (Combatant != null && Utility.RandomDouble() < 0.025)
            {
                DoChillingMist();
            }
        }

        private void DoChillingMist()
        {
            if (Map == null)
                return;

            Say("*The nimbus wraith releases a chilling mist*");
            PlaySound(0x10B);

            foreach (Mobile m in GetMobilesInRange(6))
            {
                if (m != this && CanBeHarmful(m))
                {
                    if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                    {
                        DoHarmful(m);
                        m.FixedParticles(0x376A, 9, 32, 5007, 1281, 0, EffectLayer.Waist);
                        m.PlaySound(0x1FB);

                        int damage = Utility.RandomMinMax(15, 25);
                        AOS.Damage(m, this, damage, 0, 0, 70, 0, 30);

                        // Slow effect
                        m.AddStatMod(new StatMod(StatType.Dex, "NimbusMist", -20, TimeSpan.FromSeconds(10)));
                        m.SendMessage(0x480, "The chilling mist slows your movements!");
                    }
                }
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool AlwaysMurderer => true;

        public NimbusWraith(Serial serial) : base(serial)
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
