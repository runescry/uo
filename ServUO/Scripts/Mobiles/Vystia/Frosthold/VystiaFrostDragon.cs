using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a frost dragon corpse")]
    public class VystiaFrostDragon : BaseCreature
    {
        [Constructable]
        public VystiaFrostDragon() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {


            Name = "a frost dragon";
            Body = 12;
            Hue = 1152;
            BaseSoundID = 362;

            SetStr(550, 650);
            SetDex(100, 130);
            SetInt(200, 260);

            SetHits(500, 600);
            SetDamage(20, 28);

            SetDamageType(ResistanceType.Physical, 30);
            SetDamageType(ResistanceType.Cold, 70);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 80, 90);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 45, 55);

            SetSkill(SkillName.EvalInt, 90.0, 110.0);
            SetSkill(SkillName.Magery, 90.0, 110.0);
            SetSkill(SkillName.MagicResist, 95.0, 115.0);
            SetSkill(SkillName.Tactics, 95.0, 115.0);
            SetSkill(SkillName.Wrestling, 95.0, 115.0);

            Fame = 18000;
            Karma = -18000;
            VirtualArmor = 60;

            Tamable = true;
            ControlSlots = 4;
            MinTameSkill = 100.0;

            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.Rich);

            PackItem(new FrozenOre(Utility.RandomMinMax(10, 18)));
            PackItem(new FrostforgedIngot(Utility.RandomMinMax(5, 9)));

            if (Utility.RandomDouble() < 0.20)
                PackItem(new IceCrystal(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new EternalIce(Utility.RandomMinMax(1, 2)));
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x480, "The frost dragon's icy claws freeze you!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1152, 0, EffectLayer.Waist);
                defender.PlaySound(0x1FB);

                int damage = Utility.RandomMinMax(15, 25);
                AOS.Damage(defender, this, damage, 0, 0, 100, 0, 0);

                defender.AddStatMod(new StatMod(StatType.Dex, "FrostDragonSlow", -20, TimeSpan.FromSeconds(10)));
            }
        }

        public override int Meat => 15;
        public override int Hides => 25;
        public override int Scales => 8;
        public override ScaleType ScaleType => ScaleType.White;
        public override FoodType FavoriteFood => FoodType.Meat;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 5;

        public VystiaFrostDragon(Serial serial) : base(serial)
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