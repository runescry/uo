using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a winter wolf corpse")]
    public class WinterWolf : BaseCreature
    {
        [Constructable]
        public WinterWolf() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a winter wolf";
            Body = 277;
            Hue = 1150;
            BaseSoundID = 0xE5;

            SetStr(120, 150);
            SetDex(100, 130);
            SetInt(30, 50);

            SetHits(85, 100);
            SetDamage(10, 16);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Cold, 40);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 5, 15);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.MagicResist, 50.0, 70.0);
            SetSkill(SkillName.Tactics, 60.0, 80.0);
            SetSkill(SkillName.Wrestling, 70.0, 90.0);

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
                PackItem(new FrozenOre(Utility.RandomMinMax(1, 3)));

            if (Utility.RandomDouble() < 0.05)
                PackItem(new FrostEssence());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.20)
            {
                defender.SendMessage(0x480, "The winter wolf's bite freezes you!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1150, 0, EffectLayer.Waist);
                defender.PlaySound(0x1E5);

                int damage = Utility.RandomMinMax(5, 10);
                AOS.Damage(defender, this, damage, 0, 0, 100, 0, 0);
            }
        }

        public override int Meat => 1;
        public override int Hides => 6;
        public override FoodType FavoriteFood => FoodType.Meat;
        public override PackInstinct PackInstinct => PackInstinct.Canine;

        public WinterWolf(Serial serial) : base(serial)
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
