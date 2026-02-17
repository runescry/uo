using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a sky golem corpse")]
    public class SkyGolem : BaseCreature
    {
        [Constructable]
        public SkyGolem() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a sky golem";
            Body = 752;
            Hue = 1281;
            BaseSoundID = 541;

            SetStr(280, 350);
            SetDex(80, 100);
            SetInt(60, 80);

            SetHits(280, 350);
            SetDamage(16, 24);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Energy, 40);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 45, 55);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 70, 80);

            SetSkill(SkillName.MagicResist, 80.0, 100.0);
            SetSkill(SkillName.Tactics, 85.0, 105.0);
            SetSkill(SkillName.Wrestling, 85.0, 105.0);

            Fame = 9000;
            Karma = -9000;
            VirtualArmor = 55;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);

            PackItem(new CrystalOre(Utility.RandomMinMax(5, 10)));
            PackItem(new CrystallineIngot(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new StormCrystal());

            if (Utility.RandomDouble() < 0.08)
                PackItem(new StormCrystal());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x480, "The sky golem discharges static electricity!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1281, 0, EffectLayer.Waist);
                defender.PlaySound(0x1E1);

                int damage = Utility.RandomMinMax(12, 20);
                AOS.Damage(defender, this, damage, 0, 0, 0, 0, 100);
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 4;

        public SkyGolem(Serial serial) : base(serial)
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
