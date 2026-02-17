using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an air sprite corpse")]
    public class AirSprite : BaseCreature
    {
        [Constructable]
        public AirSprite() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an air sprite";
            Body = 58;
            Hue = 1281;
            BaseSoundID = 466;

            SetStr(50, 70);
            SetDex(130, 160);
            SetInt(90, 120);

            SetHits(45, 65);
            SetDamage(5, 10);

            SetDamageType(ResistanceType.Physical, 30);
            SetDamageType(ResistanceType.Energy, 70);

            SetResistance(ResistanceType.Physical, 25, 35);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 35, 45);
            SetResistance(ResistanceType.Poison, 25, 35);
            SetResistance(ResistanceType.Energy, 60, 70);

            SetSkill(SkillName.EvalInt, 55.0, 75.0);
            SetSkill(SkillName.Magery, 55.0, 75.0);
            SetSkill(SkillName.MagicResist, 45.0, 65.0);
            SetSkill(SkillName.Tactics, 40.0, 60.0);
            SetSkill(SkillName.Wrestling, 35.0, 55.0);

            Fame = 2500;
            Karma = -2500;
            VirtualArmor = 22;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.LowScrolls);

            if (Utility.RandomDouble() < 0.15)
                PackItem(new CrystalOre());

            if (Utility.RandomDouble() < 0.05)
                PackItem(new StormCrystal());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x480, "The air sprite's touch shocks you!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1281, 0, EffectLayer.Waist);

                int damage = Utility.RandomMinMax(4, 8);
                AOS.Damage(defender, this, damage, 0, 0, 0, 0, 100);
            }
        }

        public override bool BleedImmune => true;

        public AirSprite(Serial serial) : base(serial)
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
