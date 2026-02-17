using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a sunlit serpent corpse")]
    public class SunlitSerpent : BaseCreature
    {
        [Constructable]
        public SunlitSerpent() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a sunlit serpent";
            Body = 52;
            Hue = 1719;
            BaseSoundID = 219;

            SetStr(120, 150);
            SetDex(120, 150);
            SetInt(40, 60);

            SetHits(90, 120);
            SetDamage(8, 14);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Fire, 30);
            SetDamageType(ResistanceType.Poison, 20);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 15, 25);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.Poisoning, 90.0, 110.0);
            SetSkill(SkillName.MagicResist, 50.0, 70.0);
            SetSkill(SkillName.Tactics, 60.0, 80.0);
            SetSkill(SkillName.Wrestling, 60.0, 80.0);

            Fame = 3500;
            Karma = -3500;
            VirtualArmor = 30;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 75.0;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);

            if (Utility.RandomDouble() < 0.15)
                PackItem(new SandstoneOre(Utility.RandomMinMax(1, 2)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new SandstoneOre());

            if (Utility.RandomDouble() < 0.05)
                PackItem(new SandstoneOre());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.35)
            {
                defender.SendMessage(0x5D, "The sunlit serpent's venom burns like fire!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1719, 0, EffectLayer.Waist);
                defender.ApplyPoison(this, Poison.Greater);

                // Fire damage
                int damage = Utility.RandomMinMax(5, 10);
                AOS.Damage(defender, this, damage, 0, 100, 0, 0, 0);
            }
        }

        public override Poison HitPoison => Poison.Greater;
        public override Poison PoisonImmune => Poison.Lethal;
        public override int Meat => 1;
        public override FoodType FavoriteFood => FoodType.Eggs | FoodType.Meat;

        public SunlitSerpent(Serial serial) : base(serial)
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
