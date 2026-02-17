using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a steam elemental corpse")]
    public class SteamElemental : BaseCreature
    {
        [Constructable]
        public SteamElemental() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a steam elemental";
            Body = 161;
            Hue = 2305;
            BaseSoundID = 268;

            SetStr(180, 220);
            SetDex(100, 130);
            SetInt(130, 170);

            SetHits(160, 210);
            SetDamage(12, 18);

            SetDamageType(ResistanceType.Physical, 30);
            SetDamageType(ResistanceType.Fire, 70);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 70, 80);
            SetResistance(ResistanceType.Cold, 25, 35);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 45, 55);

            SetSkill(SkillName.EvalInt, 70.0, 90.0);
            SetSkill(SkillName.Magery, 70.0, 90.0);
            SetSkill(SkillName.MagicResist, 70.0, 90.0);
            SetSkill(SkillName.Tactics, 60.0, 80.0);
            SetSkill(SkillName.Wrestling, 60.0, 80.0);

            Fame = 7500;
            Karma = -7500;
            VirtualArmor = 42;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.MedScrolls);

            PackItem(new SteamworkOre(Utility.RandomMinMax(3, 6)));
            PackItem(new ClockworkIngot(Utility.RandomMinMax(1, 3)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new ClockworkSpring());

            if (Utility.RandomDouble() < 0.05)
                PackItem(new SteamCore());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.35)
            {
                defender.SendMessage(0x25, "The steam elemental scalds you!");
                defender.FixedParticles(0x3709, 10, 30, 5052, 2305, 0, EffectLayer.Waist);
                defender.PlaySound(0x108);

                int damage = Utility.RandomMinMax(10, 18);
                AOS.Damage(defender, this, damage, 0, 100, 0, 0, 0);
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Steam vent AoE
            if (Combatant != null && Utility.RandomDouble() < 0.02)
            {
                DoSteamVent();
            }
        }

        private void DoSteamVent()
        {
            if (Map == null)
                return;

            Say("*Pressurized steam erupts*");
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

                        // Blind effect
                        m.AddStatMod(new StatMod(StatType.Dex, "SteamBlind", -15, TimeSpan.FromSeconds(5)));
                    }
                }
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 3;

        public SteamElemental(Serial serial) : base(serial)
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
