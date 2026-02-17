using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a vystia ghoul corpse")]
    public class VystiaGhoul : BaseCreature
    {
        [Constructable]
        public VystiaGhoul() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a vystia ghoul";
            Body = 153;
            BaseSoundID = 0x482;

            SetStr(120, 160);
            SetDex(90, 120);
            SetInt(60, 80);

            SetHits(120, 160);
            SetDamage(10, 16);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Poison, 20);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 15, 25);
            SetResistance(ResistanceType.Cold, 45, 55);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 55.0, 75.0);
            SetSkill(SkillName.Tactics, 65.0, 85.0);
            SetSkill(SkillName.Wrestling, 70.0, 90.0);

            Fame = 3500;
            Karma = -3500;
            VirtualArmor = 32;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.Average);

            if (Utility.RandomDouble() < 0.20)
                PackItem(new Bone(Utility.RandomMinMax(1, 3)));
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            // Paralytic touch
            if (Utility.RandomDouble() < 0.20)
            {
                defender.SendMessage(0x22, "The ghoul's touch paralyzes you!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1109, 0, EffectLayer.Waist);
                defender.PlaySound(0x1FB);

                defender.Freeze(TimeSpan.FromSeconds(2.0));
            }

            // Life drain
            if (Utility.RandomDouble() < 0.25)
            {
                int drain = Utility.RandomMinMax(6, 12);
                AOS.Damage(defender, this, drain, 100, 0, 0, 0, 0);
                Hits = Math.Min(HitsMax, Hits + drain);
                defender.SendMessage(0x22, "The ghoul drains your life force!");
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Greater;
        public override bool AlwaysMurderer => true;

        public VystiaGhoul(Serial serial) : base(serial)
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
