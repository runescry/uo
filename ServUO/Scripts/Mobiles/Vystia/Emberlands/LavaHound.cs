using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a lava hound corpse")]
    public class LavaHound : BaseCreature
    {
        [Constructable]
        public LavaHound() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a lava hound";
            Body = 98;
            Hue = 1358;
            BaseSoundID = 229;

            SetStr(150, 180);
            SetDex(100, 130);
            SetInt(40, 60);

            SetHits(100, 130);
            SetDamage(10, 16);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Fire, 60);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 70, 80);
            SetResistance(ResistanceType.Cold, 5, 15);
            SetResistance(ResistanceType.Poison, 25, 35);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 50.0, 70.0);
            SetSkill(SkillName.Tactics, 70.0, 90.0);
            SetSkill(SkillName.Wrestling, 70.0, 90.0);

            Fame = 4500;
            Karma = -4500;
            VirtualArmor = 35;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 80.0;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);

            if (Utility.RandomDouble() < 0.15)
                PackItem(new MoltenOre(Utility.RandomMinMax(1, 3)));

            if (Utility.RandomDouble() < 0.05)
                PackItem(new EverburningCoal());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x22, "The lava hound's bite burns you!");
                defender.FixedParticles(0x3709, 10, 30, 5052, 1358, 0, EffectLayer.Waist);

                int damage = Utility.RandomMinMax(5, 10);
                AOS.Damage(defender, this, damage, 0, 100, 0, 0, 0);

                // Fire DoT
                Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), 3, () =>
                {
                    if (defender != null && !defender.Deleted && defender.Alive)
                    {
                        AOS.Damage(defender, this, Utility.RandomMinMax(2, 5), 0, 100, 0, 0, 0);
                    }
                });
            }
        }

        public override int Meat => 1;
        public override FoodType FavoriteFood => FoodType.Meat;
        public override PackInstinct PackInstinct => PackInstinct.Canine;

        public LavaHound(Serial serial) : base(serial)
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
