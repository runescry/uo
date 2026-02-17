using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a frost elemental corpse")]
    public class FrostElemental : BaseCreature
    {
        [Constructable]
        public FrostElemental() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a frost elemental";
            Body = 161;
            Hue = 1152;
            BaseSoundID = 268;

            SetStr(180, 220);
            SetDex(90, 110);
            SetInt(120, 150);

            SetHits(150, 200);
            SetDamage(12, 18);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Cold, 80);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 5, 15);
            SetResistance(ResistanceType.Cold, 75, 85);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.EvalInt, 70.0, 90.0);
            SetSkill(SkillName.Magery, 70.0, 90.0);
            SetSkill(SkillName.MagicResist, 70.0, 90.0);
            SetSkill(SkillName.Tactics, 60.0, 80.0);
            SetSkill(SkillName.Wrestling, 60.0, 80.0);

            Fame = 6500;
            Karma = -6500;
            VirtualArmor = 40;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.MedScrolls);

            PackItem(new FrozenOre(Utility.RandomMinMax(3, 6)));
            PackItem(new FrostforgedIngot(Utility.RandomMinMax(1, 3)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new IceCrystal());

            if (Utility.RandomDouble() < 0.05)
                PackItem(new EternalIce());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x480, "The frost elemental's icy touch freezes you!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1152, 0, EffectLayer.Waist);
                defender.PlaySound(0x204);

                // Cold damage
                int damage = Utility.RandomMinMax(8, 15);
                AOS.Damage(defender, this, damage, 0, 0, 100, 0, 0);

                // Brief freeze
                if (Utility.RandomDouble() < 0.25)
                {
                    defender.Freeze(TimeSpan.FromSeconds(1.5));
                    defender.SendMessage(0x480, "You are frozen in place!");
                }
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Regular;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 2;

        public FrostElemental(Serial serial) : base(serial)
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
