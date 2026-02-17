using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a siren wraith remains")]
    public class SirenWraith : BaseCreature
    {
        [Constructable]
        public SirenWraith() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a siren wraith";
            Body = 26;
            Hue = 1365;
            BaseSoundID = 0x482;

            SetStr(100, 130);
            SetDex(120, 150);
            SetInt(180, 230);

            SetHits(130, 170);
            SetDamage(9, 15);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Cold, 50);
            SetDamageType(ResistanceType.Energy, 30);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 15, 25);
            SetResistance(ResistanceType.Cold, 65, 75);
            SetResistance(ResistanceType.Poison, 55, 65);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.EvalInt, 95.0, 115.0);
            SetSkill(SkillName.Magery, 95.0, 115.0);
            SetSkill(SkillName.MagicResist, 90.0, 110.0);
            SetSkill(SkillName.Tactics, 55.0, 75.0);
            SetSkill(SkillName.Wrestling, 50.0, 70.0);
            SetSkill(SkillName.Meditation, 85.0, 105.0);

            Fame = 10000;
            Karma = -10000;
            VirtualArmor = 40;

            CanSwim = true;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.MedScrolls, 2);

            PackItem(new ObsidianOre(Utility.RandomMinMax(3, 6)));

            if (Utility.RandomDouble() < 0.20)
                PackItem(new ObsidianOre(Utility.RandomMinMax(1, 2)));

            if (Utility.RandomDouble() < 0.12)
                PackItem(new VoidforgedIngot());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x480, "The siren wraith's touch drains your life!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1365, 0, EffectLayer.Waist);
                defender.PlaySound(0x1FB);

                int damage = Utility.RandomMinMax(8, 14);
                AOS.Damage(defender, this, damage, 0, 0, 50, 0, 50);

                // Life drain
                Hits = Math.Min(HitsMax, Hits + damage);
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Siren's song - mesmerize
            if (Combatant != null && Utility.RandomDouble() < 0.025)
            {
                DoSirenSong();
            }
        }

        private void DoSirenSong()
        {
            if (Map == null)
                return;

            Say("*The siren wraith sings a haunting melody*");
            PlaySound(0x5C9);

            foreach (Mobile m in GetMobilesInRange(8))
            {
                if (m != this && CanBeHarmful(m))
                {
                    if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                    {
                        DoHarmful(m);
                        m.FixedParticles(0x376A, 9, 32, 5007, 1365, 0, EffectLayer.Head);
                        m.PlaySound(0x5C9);

                        // Mesmerize - freeze and mana drain
                        m.Freeze(TimeSpan.FromSeconds(3.0));
                        m.SendMessage(0x480, "The siren's song mesmerizes you!");

                        int manaDrain = Utility.RandomMinMax(30, 50);
                        if (m.Mana >= manaDrain)
                        {
                            m.Mana -= manaDrain;
                            Mana = Math.Min(ManaMax, Mana + manaDrain);
                        }
                    }
                }
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool AlwaysMurderer => true;

        public SirenWraith(Serial serial) : base(serial)
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
