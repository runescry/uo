using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a copper sentinel corpse")]
    public class CopperSentinel : BaseCreature
    {
        [Constructable]
        public CopperSentinel() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a copper sentinel";
            Body = 752;
            Hue = 2305;
            BaseSoundID = 541;

            SetStr(220, 280);
            SetDex(80, 100);
            SetInt(60, 80);

            SetHits(200, 260);
            SetDamage(14, 20);

            SetDamageType(ResistanceType.Physical, 70);
            SetDamageType(ResistanceType.Energy, 30);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 55, 65);

            SetSkill(SkillName.MagicResist, 70.0, 90.0);
            SetSkill(SkillName.Tactics, 75.0, 95.0);
            SetSkill(SkillName.Wrestling, 75.0, 95.0);

            Fame = 7000;
            Karma = -7000;
            VirtualArmor = 50;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);

            PackItem(new SteamworkOre(Utility.RandomMinMax(4, 8)));
            PackItem(new ClockworkIngot(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new ClockworkSpring());

            if (Utility.RandomDouble() < 0.05)
                PackItem(new SteamCore());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x25, "The copper sentinel discharges electricity!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 2305, 0, EffectLayer.Waist);
                defender.PlaySound(0x1E1);

                int damage = Utility.RandomMinMax(10, 18);
                AOS.Damage(defender, this, damage, 0, 0, 0, 0, 100);
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Electric field AoE
            if (Combatant != null && Utility.RandomDouble() < 0.02)
            {
                DoElectricField();
            }
        }

        private void DoElectricField()
        {
            if (Map == null)
                return;

            Say("*The copper sentinel generates an electric field*");
            PlaySound(0x1E1);

            foreach (Mobile m in GetMobilesInRange(3))
            {
                if (m != this && CanBeHarmful(m))
                {
                    if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                    {
                        DoHarmful(m);
                        m.FixedParticles(0x374A, 10, 15, 5028, 2305, 0, EffectLayer.Waist);

                        int damage = Utility.RandomMinMax(12, 22);
                        AOS.Damage(m, this, damage, 0, 0, 0, 0, 100);
                        m.SendMessage(0x25, "Electricity courses through your body!");
                    }
                }
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 3;

        public CopperSentinel(Serial serial) : base(serial)
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
