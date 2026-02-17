using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a crystal serpent corpse")]
    public class CrystalSerpent : BaseCreature
    {
        [Constructable]
        public CrystalSerpent() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a crystal serpent";
            Body = 52;
            Hue = 1154;
            BaseSoundID = 219;

            SetStr(120, 150);
            SetDex(110, 140);
            SetInt(60, 80);

            SetHits(100, 130);
            SetDamage(10, 16);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Energy, 60);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 45, 55);
            SetResistance(ResistanceType.Poison, 35, 45);
            SetResistance(ResistanceType.Energy, 70, 80);

            SetSkill(SkillName.MagicResist, 60.0, 80.0);
            SetSkill(SkillName.Tactics, 60.0, 80.0);
            SetSkill(SkillName.Wrestling, 60.0, 80.0);

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
            AddLoot(LootPack.Rich);

            PackItem(new CrystalOre(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new CrystallineIngot());

            if (Utility.RandomDouble() < 0.05)
                PackItem(new PrismaticShard());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x480, "The crystal serpent's bite channels energy through you!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1154, 0, EffectLayer.Waist);

                int damage = Utility.RandomMinMax(8, 15);
                AOS.Damage(defender, this, damage, 0, 0, 0, 0, 100);

                // Mana drain
                if (defender is PlayerMobile pm)
                {
                    int manaDrain = Utility.RandomMinMax(8, 15);
                    pm.Mana = Math.Max(0, pm.Mana - manaDrain);
                    pm.SendMessage(0x480, "Your mana drains away!");
                }
            }
        }

        public override int Meat => 1;
        public override FoodType FavoriteFood => FoodType.Meat;

        public CrystalSerpent(Serial serial) : base(serial)
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
