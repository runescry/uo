using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a magma troll corpse")]
    public class MagmaTroll : BaseCreature
    {
        [Constructable]
        public MagmaTroll() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a magma troll";
            Body = 53;
            Hue = 1358;
            BaseSoundID = 461;

            SetStr(250, 300);
            SetDex(80, 100);
            SetInt(50, 70);

            SetHits(200, 250);
            SetDamage(14, 22);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Fire, 60);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 75, 85);
            SetResistance(ResistanceType.Cold, 5, 15);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 60.0, 80.0);
            SetSkill(SkillName.Tactics, 80.0, 100.0);
            SetSkill(SkillName.Wrestling, 80.0, 100.0);

            Fame = 7000;
            Karma = -7000;
            VirtualArmor = 45;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average);

            PackItem(new MoltenOre(Utility.RandomMinMax(3, 6)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new EmberforgedIngot(Utility.RandomMinMax(1, 2)));

            if (Utility.RandomDouble() < 0.05)
                PackItem(new EverburningCoal());
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            // Fire regeneration
            if (Utility.RandomDouble() < 0.15 && Hits < HitsMax)
            {
                int regen = Utility.RandomMinMax(10, 20);
                Hits = Math.Min(Hits + regen, HitsMax);
                FixedParticles(0x3709, 10, 30, 5052, 1358, 0, EffectLayer.Waist);
            }

            // Burn attacker
            if (attacker != null && Utility.RandomDouble() < 0.20)
            {
                attacker.SendMessage(0x22, "The magma troll's molten skin burns you!");
                int damage = Utility.RandomMinMax(5, 12);
                AOS.Damage(attacker, this, damage, 0, 100, 0, 0, 0);
            }
        }

        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 3;

        public MagmaTroll(Serial serial) : base(serial)
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
