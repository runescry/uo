using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a mire leech corpse")]
    public class MireLeech : BaseCreature
    {
        [Constructable]
        public MireLeech() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a mire leech";
            Body = 293;
            Hue = 2073;
            BaseSoundID = 898;

            SetStr(80, 100);
            SetDex(70, 90);
            SetInt(20, 40);

            SetHits(60, 80);
            SetDamage(6, 12);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Poison, 50);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 5, 15);
            SetResistance(ResistanceType.Cold, 25, 35);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.MagicResist, 30.0, 50.0);
            SetSkill(SkillName.Tactics, 50.0, 70.0);
            SetSkill(SkillName.Wrestling, 50.0, 70.0);

            Fame = 2000;
            Karma = -2000;
            VirtualArmor = 25;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);

            if (Utility.RandomDouble() < 0.10)
                PackItem(new BogIronOre());

            if (Utility.RandomDouble() < 0.05)
                PackItem(new BogIronOre());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.35)
            {
                defender.SendMessage(0x3B2, "The mire leech drains your life force!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 2073, 0, EffectLayer.Waist);

                // Life drain
                int damage = Utility.RandomMinMax(8, 15);
                AOS.Damage(defender, this, damage, 0, 0, 0, 100, 0);

                // Heal self
                Hits = Math.Min(Hits + damage, HitsMax);
                FixedParticles(0x376A, 9, 32, 5005, 2073, 0, EffectLayer.Waist);
            }
        }

        public override Poison HitPoison => Poison.Regular;
        public override Poison PoisonImmune => Poison.Greater;

        public MireLeech(Serial serial) : base(serial)
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
