using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a steam golem corpse")]
    public class SteamGolem : BaseCreature
    {
        [Constructable]
        public SteamGolem() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a steam golem";
            Body = 752;
            Hue = 2305;
            BaseSoundID = 541;

            SetStr(300, 380);
            SetDex(70, 90);
            SetInt(60, 80);

            SetHits(300, 380);
            SetDamage(16, 24);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Fire, 40);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.MagicResist, 80.0, 100.0);
            SetSkill(SkillName.Tactics, 85.0, 105.0);
            SetSkill(SkillName.Wrestling, 85.0, 105.0);

            Fame = 9000;
            Karma = -9000;
            VirtualArmor = 55;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);

            PackItem(new SteamworkOre(Utility.RandomMinMax(5, 10)));
            PackItem(new ClockworkIngot(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new ClockworkSpring(Utility.RandomMinMax(1, 2)));

            if (Utility.RandomDouble() < 0.08)
                PackItem(new SteamCore());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x25, "The steam golem vents scalding steam!");
                defender.FixedParticles(0x3709, 10, 30, 5052, 2305, 0, EffectLayer.Waist);
                defender.PlaySound(0x108);

                int damage = Utility.RandomMinMax(12, 20);
                AOS.Damage(defender, this, damage, 0, 100, 0, 0, 0);
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Steam blast AoE
            if (Combatant != null && Utility.RandomDouble() < 0.02)
            {
                DoSteamBlast();
            }
        }

        private void DoSteamBlast()
        {
            if (Map == null)
                return;

            Say("*The steam golem releases a massive steam blast*");
            PlaySound(0x108);

            foreach (Mobile m in GetMobilesInRange(4))
            {
                if (m != this && CanBeHarmful(m))
                {
                    if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                    {
                        DoHarmful(m);
                        m.FixedParticles(0x3709, 10, 30, 5052, 2305, 0, EffectLayer.Waist);

                        int damage = Utility.RandomMinMax(15, 25);
                        AOS.Damage(m, this, damage, 0, 100, 0, 0, 0);
                        m.SendMessage(0x25, "The scalding steam burns you!");
                    }
                }
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 4;

        public SteamGolem(Serial serial) : base(serial)
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
