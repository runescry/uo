using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an oil slime corpse")]
    public class OilSlime : BaseCreature
    {
        [Constructable]
        public OilSlime() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an oil slime";
            Body = 51;
            Hue = 2305;
            BaseSoundID = 456;

            SetStr(80, 100);
            SetDex(60, 80);
            SetInt(30, 50);

            SetHits(70, 100);
            SetDamage(6, 12);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Fire, 50);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 35.0, 55.0);
            SetSkill(SkillName.Tactics, 45.0, 65.0);
            SetSkill(SkillName.Wrestling, 45.0, 65.0);

            Fame = 2500;
            Karma = -2500;
            VirtualArmor = 25;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);

            PackItem(new SteamworkOre(Utility.RandomMinMax(1, 3)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new ClockworkSpring());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x25, "The oil slime covers you in flammable oil!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 2305, 0, EffectLayer.Waist);

                // Oil debuff - increases fire damage taken
                defender.AddStatMod(new StatMod(StatType.Dex, "OilSlimeSlow", -15, TimeSpan.FromSeconds(10)));
                defender.SendMessage(0x22, "The oil makes you more vulnerable to fire!");
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            // Fire explosion when damaged by fire
            if (Utility.RandomDouble() < 0.20 && from != null)
            {
                Say("*The oil slime ignites!*");

                foreach (Mobile m in GetMobilesInRange(2))
                {
                    if (m != this && CanBeHarmful(m))
                    {
                        DoHarmful(m);
                        int damage = Utility.RandomMinMax(8, 15);
                        AOS.Damage(m, this, damage, 0, 100, 0, 0, 0);
                        m.FixedParticles(0x3709, 10, 30, 5052, 2305, 0, EffectLayer.Waist);
                    }
                }
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;

        public OilSlime(Serial serial) : base(serial)
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
