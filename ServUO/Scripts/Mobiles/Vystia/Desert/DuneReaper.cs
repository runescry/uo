using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a dune reaper corpse")]
    public class DuneReaper : BaseCreature
    {
        [Constructable]
        public DuneReaper() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a dune reaper";
            Body = 305;
            Hue = 1719;
            BaseSoundID = 397;

            SetStr(200, 250);
            SetDex(100, 130);
            SetInt(50, 70);

            SetHits(180, 230);
            SetDamage(14, 20);

            SetDamageType(ResistanceType.Physical, 70);
            SetDamageType(ResistanceType.Poison, 30);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.Poisoning, 80.0, 100.0);
            SetSkill(SkillName.MagicResist, 60.0, 80.0);
            SetSkill(SkillName.Tactics, 80.0, 100.0);
            SetSkill(SkillName.Wrestling, 80.0, 100.0);

            Fame = 6500;
            Karma = -6500;
            VirtualArmor = 45;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average);

            PackItem(new SandstoneOre(Utility.RandomMinMax(2, 5)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new SunforgedIngot(Utility.RandomMinMax(1, 2)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new SandstoneOre());

            if (Utility.RandomDouble() < 0.05)
                PackItem(new SandstoneOre());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x5D, "The dune reaper's venomous stinger strikes!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 68, 0, EffectLayer.Waist);
                defender.ApplyPoison(this, Poison.Deadly);

                // Additional poison damage over time
                Timer.DelayCall(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), 4, () =>
                {
                    if (defender != null && !defender.Deleted && defender.Alive)
                    {
                        AOS.Damage(defender, this, Utility.RandomMinMax(5, 10), 0, 0, 0, 100, 0);
                    }
                });
            }
        }

        public override Poison HitPoison => Poison.Deadly;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 3;

        public DuneReaper(Serial serial) : base(serial)
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
