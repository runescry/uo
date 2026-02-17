using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a wild boar corpse")]
    public class WildBoar : BaseCreature
    {
        [Constructable]
        public WildBoar() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a wild boar";
            Body = 0x122;
            Hue = 2010;
            BaseSoundID = 0xC4;

            SetStr(100, 130);
            SetDex(80, 100);
            SetInt(20, 40);

            SetHits(70, 100);
            SetDamage(8, 14);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 15, 25);
            SetResistance(ResistanceType.Cold, 25, 35);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.MagicResist, 40.0, 60.0);
            SetSkill(SkillName.Tactics, 60.0, 80.0);
            SetSkill(SkillName.Wrestling, 60.0, 80.0);

            Fame = 2500;
            Karma = -2500;
            VirtualArmor = 30;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 65.0;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);

            if (Utility.RandomDouble() < 0.10)
                PackItem(new LivingOre(Utility.RandomMinMax(1, 2)));
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            // Charge attack
            if (Utility.RandomDouble() < 0.20)
            {
                defender.SendMessage(0x3B2, "The wild boar charges into you!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 2010, 0, EffectLayer.Waist);

                int damage = Utility.RandomMinMax(5, 10);
                AOS.Damage(defender, this, damage, 100, 0, 0, 0, 0);
                defender.Freeze(TimeSpan.FromSeconds(1));
            }
        }

        public override int Meat => 2;
        public override int Hides => 8;
        public override FoodType FavoriteFood => FoodType.FruitsAndVegies;

        public WildBoar(Serial serial) : base(serial)
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
