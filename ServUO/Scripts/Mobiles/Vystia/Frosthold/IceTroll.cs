using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an ice troll corpse")]
    public class IceTroll : BaseCreature
    {
        [Constructable]
        public IceTroll() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an ice troll";
            Body = 53;
            Hue = 1153;
            BaseSoundID = 461;

            SetStr(200, 250);
            SetDex(80, 100);
            SetInt(40, 60);

            SetHits(150, 200);
            SetDamage(12, 20);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Cold, 50);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 5, 15);
            SetResistance(ResistanceType.Cold, 65, 75);
            SetResistance(ResistanceType.Poison, 25, 35);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 60.0, 80.0);
            SetSkill(SkillName.Tactics, 70.0, 90.0);
            SetSkill(SkillName.Wrestling, 70.0, 90.0);

            Fame = 5000;
            Karma = -5000;
            VirtualArmor = 40;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average);

            PackItem(new FrozenOre(Utility.RandomMinMax(2, 5)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new FrostforgedIngot(Utility.RandomMinMax(1, 2)));

            if (Utility.RandomDouble() < 0.05)
                PackItem(new IceCrystal());
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            // Regeneration
            if (Utility.RandomDouble() < 0.15 && Hits < HitsMax)
            {
                int regen = Utility.RandomMinMax(5, 15);
                Hits = Math.Min(Hits + regen, HitsMax);
                FixedParticles(0x376A, 9, 32, 5005, 1153, 0, EffectLayer.Waist);
            }
        }

        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 2;

        public IceTroll(Serial serial) : base(serial)
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
