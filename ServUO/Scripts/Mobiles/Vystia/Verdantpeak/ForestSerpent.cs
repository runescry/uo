using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a forest serpent corpse")]
    public class ForestSerpent : BaseCreature
    {
        [Constructable]
        public ForestSerpent() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a forest serpent";
            Body = 52;
            Hue = 2010;
            BaseSoundID = 219;

            SetStr(100, 130);
            SetDex(100, 130);
            SetInt(30, 50);

            SetHits(80, 110);
            SetDamage(8, 14);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Poison, 50);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 15, 25);
            SetResistance(ResistanceType.Cold, 25, 35);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.Poisoning, 80.0, 100.0);
            SetSkill(SkillName.MagicResist, 45.0, 65.0);
            SetSkill(SkillName.Tactics, 55.0, 75.0);
            SetSkill(SkillName.Wrestling, 55.0, 75.0);

            Fame = 3000;
            Karma = -3000;
            VirtualArmor = 28;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 70.0;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);

            if (Utility.RandomDouble() < 0.10)
                PackItem(new LivingOre(Utility.RandomMinMax(1, 2)));

            if (Utility.RandomDouble() < 0.05)
                PackItem(new NatureforgedIngot());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.35)
            {
                defender.SendMessage(0x3B2, "The forest serpent's venom courses through your veins!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 68, 0, EffectLayer.Waist);
                defender.ApplyPoison(this, Poison.Greater);
            }
        }

        public override Poison HitPoison => Poison.Greater;
        public override Poison PoisonImmune => Poison.Lethal;
        public override int Meat => 1;
        public override FoodType FavoriteFood => FoodType.Eggs | FoodType.Meat;

        public ForestSerpent(Serial serial) : base(serial)
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
