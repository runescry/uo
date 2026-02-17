using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an iron elemental corpse")]
    public class IronElemental : BaseCreature
    {
        [Constructable]
        public IronElemental() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an iron elemental";
            Body = 14;
            Hue = 2305;
            BaseSoundID = 268;

            SetStr(250, 300);
            SetDex(70, 90);
            SetInt(70, 90);

            SetHits(220, 280);
            SetDamage(14, 22);

            SetDamageType(ResistanceType.Physical, 90);
            SetDamageType(ResistanceType.Fire, 10);

            SetResistance(ResistanceType.Physical, 65, 75);
            SetResistance(ResistanceType.Fire, 45, 55);
            SetResistance(ResistanceType.Cold, 35, 45);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.MagicResist, 70.0, 90.0);
            SetSkill(SkillName.Tactics, 80.0, 100.0);
            SetSkill(SkillName.Wrestling, 80.0, 100.0);

            Fame = 8000;
            Karma = -8000;
            VirtualArmor = 55;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average);

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

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x25, "The iron elemental's heavy blow staggers you!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 2305, 0, EffectLayer.Waist);
                defender.PlaySound(0x2A1);

                int damage = Utility.RandomMinMax(10, 18);
                AOS.Damage(defender, this, damage, 100, 0, 0, 0, 0);

                // Stagger effect
                defender.Freeze(TimeSpan.FromSeconds(1));
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            // Iron shrapnel
            if (Utility.RandomDouble() < 0.10 && from != null)
            {
                Say("*Iron shrapnel flies outward*");

                foreach (Mobile m in GetMobilesInRange(3))
                {
                    if (m != this && CanBeHarmful(m))
                    {
                        DoHarmful(m);
                        int damage = Utility.RandomMinMax(6, 12);
                        AOS.Damage(m, this, damage, 100, 0, 0, 0, 0);
                        m.FixedParticles(0x374A, 10, 15, 5028, 2305, 0, EffectLayer.Waist);
                    }
                }
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 3;

        public IronElemental(Serial serial) : base(serial)
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
