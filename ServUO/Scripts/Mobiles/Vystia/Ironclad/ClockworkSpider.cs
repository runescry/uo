using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a clockwork spider corpse")]
    public class ClockworkSpider : BaseCreature
    {
        [Constructable]
        public ClockworkSpider() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a clockwork spider";
            Body = 28;
            Hue = 2305;
            BaseSoundID = 1170;

            SetStr(100, 130);
            SetDex(110, 140);
            SetInt(40, 60);

            SetHits(80, 110);
            SetDamage(8, 14);

            SetDamageType(ResistanceType.Physical, 80);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 25, 35);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 45, 55);

            SetSkill(SkillName.MagicResist, 50.0, 70.0);
            SetSkill(SkillName.Tactics, 60.0, 80.0);
            SetSkill(SkillName.Wrestling, 60.0, 80.0);

            Fame = 4000;
            Karma = -4000;
            VirtualArmor = 35;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);

            PackItem(new SteamworkOre(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new ClockworkIngot());

            if (Utility.RandomDouble() < 0.10)
                PackItem(new ClockworkSpring());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x25, "The clockwork spider's mechanical legs cut into you!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 2305, 0, EffectLayer.Waist);

                // Bleed effect
                Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), 3, () =>
                {
                    if (defender != null && !defender.Deleted && defender.Alive)
                    {
                        AOS.Damage(defender, this, Utility.RandomMinMax(3, 6), 100, 0, 0, 0, 0);
                    }
                });
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;

        public ClockworkSpider(Serial serial) : base(serial)
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
