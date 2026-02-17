using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a desert harpy corpse")]
    public class DesertHarpy : BaseCreature
    {
        [Constructable]
        public DesertHarpy() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a desert harpy";
            Body = 30;
            Hue = 1719;
            BaseSoundID = 402;

            SetStr(100, 130);
            SetDex(120, 150);
            SetInt(50, 70);

            SetHits(80, 110);
            SetDamage(8, 14);

            SetDamageType(ResistanceType.Physical, 80);
            SetDamageType(ResistanceType.Fire, 20);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 35, 45);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 40.0, 60.0);
            SetSkill(SkillName.Tactics, 60.0, 80.0);
            SetSkill(SkillName.Wrestling, 60.0, 80.0);

            Fame = 3500;
            Karma = -3500;
            VirtualArmor = 30;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Meager);

            if (Utility.RandomDouble() < 0.10)
                PackItem(new SandstoneOre(Utility.RandomMinMax(1, 2)));

            if (Utility.RandomDouble() < 0.05)
                PackItem(new SandstoneOre());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x5D, "The desert harpy's talons tear into you!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1719, 0, EffectLayer.Waist);

                // Bleeding effect
                Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), 3, () =>
                {
                    if (defender != null && !defender.Deleted && defender.Alive)
                    {
                        AOS.Damage(defender, this, Utility.RandomMinMax(3, 6), 100, 0, 0, 0, 0);
                    }
                });
            }
        }

        public override int Feathers => 25;
        public override bool AlwaysMurderer => true;

        public DesertHarpy(Serial serial) : base(serial)
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
