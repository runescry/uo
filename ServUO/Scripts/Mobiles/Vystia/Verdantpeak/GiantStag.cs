using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a giant stag corpse")]
    public class GiantStag : BaseCreature
    {
        [Constructable]
        public GiantStag() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a giant stag";
            Body = 0xEA;
            Hue = 2010;
            BaseSoundID = 0x82;

            SetStr(180, 220);
            SetDex(100, 130);
            SetInt(40, 60);

            SetHits(150, 200);
            SetDamage(12, 18);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 35, 45);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 50.0, 70.0);
            SetSkill(SkillName.Tactics, 70.0, 90.0);
            SetSkill(SkillName.Wrestling, 70.0, 90.0);

            Fame = 5000;
            Karma = -5000;
            VirtualArmor = 40;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 80.0;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Rich);

            PackItem(new LivingOre(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new NatureforgedIngot());

            if (Utility.RandomDouble() < 0.05)
                PackItem(new LivingOre());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            // Antler charge attack
            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x3B2, "The giant stag gores you with its antlers!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 2010, 0, EffectLayer.Waist);

                int damage = Utility.RandomMinMax(8, 15);
                AOS.Damage(defender, this, damage, 100, 0, 0, 0, 0);

                // Knockback
                defender.Freeze(TimeSpan.FromSeconds(1));
            }
        }

        public override int Meat => 4;
        public override int Hides => 12;
        public override FoodType FavoriteFood => FoodType.FruitsAndVegies;

        public GiantStag(Serial serial) : base(serial)
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
