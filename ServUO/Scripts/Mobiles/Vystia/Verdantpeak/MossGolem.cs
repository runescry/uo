using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a moss golem corpse")]
    public class MossGolem : BaseCreature
    {
        [Constructable]
        public MossGolem() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a moss golem";
            Body = 752;
            Hue = 2010;
            BaseSoundID = 541;

            SetStr(260, 320);
            SetDex(60, 80);
            SetInt(60, 80);

            SetHits(260, 330);
            SetDamage(14, 22);

            SetDamageType(ResistanceType.Physical, 70);
            SetDamageType(ResistanceType.Poison, 30);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 35, 45);
            SetResistance(ResistanceType.Poison, 80, 90);
            SetResistance(ResistanceType.Energy, 30, 40);

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

            PackItem(new LivingOre(Utility.RandomMinMax(4, 8)));
            PackItem(new NatureforgedIngot(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new LivingOre());

            if (Utility.RandomDouble() < 0.05)
                PackItem(new TreantHeart());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x3B2, "The moss golem's mossy fists infect you with spores!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 68, 0, EffectLayer.Waist);
                defender.ApplyPoison(this, Poison.Regular);
            }
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            // Regeneration
            if (Utility.RandomDouble() < 0.15 && Hits < HitsMax)
            {
                int regen = Utility.RandomMinMax(15, 30);
                Hits = Math.Min(Hits + regen, HitsMax);
                FixedParticles(0x376A, 9, 32, 5005, 2010, 0, EffectLayer.Waist);
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 3;

        public MossGolem(Serial serial) : base(serial)
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
