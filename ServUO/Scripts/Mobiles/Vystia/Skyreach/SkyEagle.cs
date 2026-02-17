using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a sky eagle corpse")]
    public class SkyEagle : BaseCreature
    {
        [Constructable]
        public SkyEagle() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a sky eagle";
            Body = 5;
            Hue = 1281;
            BaseSoundID = 0x2EE;

            SetStr(130, 160);
            SetDex(150, 180);
            SetInt(40, 60);

            SetHits(100, 140);
            SetDamage(10, 16);

            SetDamageType(ResistanceType.Physical, 80);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 25, 35);
            SetResistance(ResistanceType.Energy, 55, 65);

            SetSkill(SkillName.MagicResist, 50.0, 70.0);
            SetSkill(SkillName.Tactics, 70.0, 90.0);
            SetSkill(SkillName.Wrestling, 70.0, 90.0);

            Fame = 4500;
            Karma = -4500;
            VirtualArmor = 32;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 85.0;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);

            PackItem(new CrystalOre(Utility.RandomMinMax(1, 3)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new StormCrystal());

            if (Utility.RandomDouble() < 0.05)
                PackItem(new CrystallineIngot());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            // Dive attack
            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x480, "The sky eagle dives and rakes you with its talons!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1281, 0, EffectLayer.Waist);

                int damage = Utility.RandomMinMax(8, 15);
                AOS.Damage(defender, this, damage, 100, 0, 0, 0, 0);

                // Bleed
                Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), 3, () =>
                {
                    if (defender != null && !defender.Deleted && defender.Alive)
                    {
                        AOS.Damage(defender, this, Utility.RandomMinMax(3, 6), 100, 0, 0, 0, 0);
                    }
                });
            }
        }

        public override int Feathers => 40;
        public override int Meat => 1;
        public override FoodType FavoriteFood => FoodType.Meat | FoodType.Fish;

        public SkyEagle(Serial serial) : base(serial)
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
