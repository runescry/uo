using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a forest bear corpse")]
    public class ForestBear : BaseCreature
    {
        [Constructable]
        public ForestBear() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a forest bear";
            Body = 211;
            Hue = 2010;
            BaseSoundID = 0xA3;

            SetStr(150, 190);
            SetDex(70, 90);
            SetInt(30, 50);

            SetHits(130, 170);
            SetDamage(10, 16);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 35, 45);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 40.0, 60.0);
            SetSkill(SkillName.Tactics, 60.0, 80.0);
            SetSkill(SkillName.Wrestling, 70.0, 90.0);

            Fame = 3500;
            Karma = -3500;
            VirtualArmor = 35;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 75.0;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Meager);

            if (Utility.RandomDouble() < 0.10)
                PackItem(new LivingOre(Utility.RandomMinMax(1, 2)));
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x3B2, "The forest bear mauls you with its claws!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 2010, 0, EffectLayer.Waist);

                // Bleed
                Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), 3, () =>
                {
                    if (defender != null && !defender.Deleted && defender.Alive)
                    {
                        AOS.Damage(defender, this, Utility.RandomMinMax(3, 6), 100, 0, 0, 0, 0);
                    }
                });
            }
        }

        public override int Meat => 2;
        public override int Hides => 14;
        public override FoodType FavoriteFood => FoodType.Fish | FoodType.Meat | FoodType.FruitsAndVegies;

        public ForestBear(Serial serial) : base(serial)
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
