using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a glacial bear corpse")]
    public class GlacialBear : BaseCreature
    {
        [Constructable]
        public GlacialBear() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a glacial bear";
            Body = 211;
            Hue = 1153;
            BaseSoundID = 0xA3;

            SetStr(200, 250);
            SetDex(80, 100);
            SetInt(30, 50);

            SetHits(180, 220);
            SetDamage(12, 18);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Cold, 40);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 5, 15);
            SetResistance(ResistanceType.Cold, 65, 75);
            SetResistance(ResistanceType.Poison, 25, 35);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 50.0, 70.0);
            SetSkill(SkillName.Tactics, 70.0, 90.0);
            SetSkill(SkillName.Wrestling, 80.0, 100.0);

            Fame = 4500;
            Karma = -4500;
            VirtualArmor = 35;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 85.0;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Meager);

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
                defender.SendMessage(0x480, "The glacial bear's claws tear into you with icy fury!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1153, 0, EffectLayer.Waist);

                // Bleeding + cold damage
                int coldDamage = Utility.RandomMinMax(5, 10);
                AOS.Damage(defender, this, coldDamage, 0, 0, 100, 0, 0);

                // Bleed DoT
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
        public override int Hides => 16;
        public override FoodType FavoriteFood => FoodType.Fish | FoodType.Meat;

        public GlacialBear(Serial serial) : base(serial)
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
