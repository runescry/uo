using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a swamp troll corpse")]
    public class SwampTroll : BaseCreature
    {
        [Constructable]
        public SwampTroll() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a swamp troll";
            Body = 53;
            Hue = 2073;
            BaseSoundID = 461;

            SetStr(220, 270);
            SetDex(70, 90);
            SetInt(40, 60);

            SetHits(180, 230);
            SetDamage(12, 20);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Poison, 40);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 15, 25);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 60.0, 80.0);
            SetSkill(SkillName.Tactics, 70.0, 90.0);
            SetSkill(SkillName.Wrestling, 70.0, 90.0);

            Fame = 6000;
            Karma = -6000;
            VirtualArmor = 45;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average);

            PackItem(new BogIronOre(Utility.RandomMinMax(2, 5)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new ShadowforgedIngot(Utility.RandomMinMax(1, 2)));

            if (Utility.RandomDouble() < 0.05)
                PackItem(new BogIronOre());
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            // Regeneration
            if (Utility.RandomDouble() < 0.20 && Hits < HitsMax)
            {
                int regen = Utility.RandomMinMax(10, 20);
                Hits = Math.Min(Hits + regen, HitsMax);
                FixedParticles(0x376A, 9, 32, 5005, 2073, 0, EffectLayer.Waist);
            }
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x3B2, "The swamp troll's diseased claws infect you!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 68, 0, EffectLayer.Waist);
                defender.ApplyPoison(this, Poison.Regular);
            }
        }

        public override Poison HitPoison => Poison.Regular;
        public override Poison PoisonImmune => Poison.Greater;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 2;

        public SwampTroll(Serial serial) : base(serial)
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
