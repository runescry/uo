using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a brass automaton corpse")]
    public class BrassAutomaton : BaseCreature
    {
        [Constructable]
        public BrassAutomaton() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a brass automaton";
            Body = 400;
            Hue = 2305;
            BaseSoundID = 541;

            SetStr(200, 250);
            SetDex(80, 100);
            SetInt(50, 70);

            SetHits(180, 230);
            SetDamage(12, 18);

            SetDamageType(ResistanceType.Physical, 80);
            SetDamageType(ResistanceType.Fire, 20);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 45, 55);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.MagicResist, 65.0, 85.0);
            SetSkill(SkillName.Tactics, 75.0, 95.0);
            SetSkill(SkillName.Wrestling, 75.0, 95.0);

            Fame = 6500;
            Karma = -6500;
            VirtualArmor = 45;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average);

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

            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x25, "The brass automaton's precise strike finds its mark!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 2305, 0, EffectLayer.Waist);

                int damage = Utility.RandomMinMax(8, 15);
                AOS.Damage(defender, this, damage, 100, 0, 0, 0, 0);
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Self repair
            if (Utility.RandomDouble() < 0.02 && Hits < HitsMax)
            {
                int repair = Utility.RandomMinMax(15, 25);
                Hits = Math.Min(Hits + repair, HitsMax);
                FixedParticles(0x376A, 9, 32, 5005, 2305, 0, EffectLayer.Waist);
                Say("*The automaton self-repairs*");
                PlaySound(0x2A);
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool AlwaysMurderer => true;

        public BrassAutomaton(Serial serial) : base(serial)
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
