using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a frost wraith corpse")]
    public class FrostWraith : BaseCreature
    {
        [Constructable]
        public FrostWraith() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a frost wraith";
            Body = 26;
            Hue = 1152;
            BaseSoundID = 0x482;

            SetStr(100, 120);
            SetDex(90, 110);
            SetInt(100, 130);

            SetHits(80, 100);
            SetDamage(8, 14);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Cold, 80);

            SetResistance(ResistanceType.Physical, 25, 35);
            SetResistance(ResistanceType.Fire, 5, 15);
            SetResistance(ResistanceType.Cold, 70, 80);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.EvalInt, 60.0, 80.0);
            SetSkill(SkillName.Magery, 60.0, 80.0);
            SetSkill(SkillName.MagicResist, 60.0, 80.0);
            SetSkill(SkillName.Tactics, 50.0, 70.0);
            SetSkill(SkillName.Wrestling, 50.0, 70.0);

            Fame = 4000;
            Karma = -4000;
            VirtualArmor = 28;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.MedScrolls);

            if (Utility.RandomDouble() < 0.15)
                PackItem(new FrozenOre(Utility.RandomMinMax(1, 3)));

            if (Utility.RandomDouble() < 0.08)
                PackItem(new IceCrystal());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x480, "The frost wraith's touch drains your warmth!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1152, 0, EffectLayer.Waist);

                // Mana drain
                if (defender is PlayerMobile pm)
                {
                    int manaDrain = Utility.RandomMinMax(10, 20);
                    pm.Mana = Math.Max(0, pm.Mana - manaDrain);
                }

                // Cold damage
                int damage = Utility.RandomMinMax(5, 10);
                AOS.Damage(defender, this, damage, 0, 0, 100, 0, 0);
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Regular;
        public override bool AlwaysMurderer => true;

        public FrostWraith(Serial serial) : base(serial)
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
