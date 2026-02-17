using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a forest sprite corpse")]
    public class ForestSprite : BaseCreature
    {
        [Constructable]
        public ForestSprite() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a forest sprite";
            Body = 58;
            Hue = 2010;
            BaseSoundID = 466;

            SetStr(50, 70);
            SetDex(100, 130);
            SetInt(90, 120);

            SetHits(45, 65);
            SetDamage(5, 10);

            SetDamageType(ResistanceType.Physical, 30);
            SetDamageType(ResistanceType.Poison, 70);

            SetResistance(ResistanceType.Physical, 25, 35);
            SetResistance(ResistanceType.Fire, 15, 25);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 35, 45);

            SetSkill(SkillName.EvalInt, 50.0, 70.0);
            SetSkill(SkillName.Magery, 50.0, 70.0);
            SetSkill(SkillName.MagicResist, 40.0, 60.0);
            SetSkill(SkillName.Tactics, 40.0, 60.0);
            SetSkill(SkillName.Wrestling, 30.0, 50.0);

            Fame = 2000;
            Karma = -2000;
            VirtualArmor = 22;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.LowScrolls);

            if (Utility.RandomDouble() < 0.15)
                PackItem(new LivingOre());

            if (Utility.RandomDouble() < 0.05)
                PackItem(new NatureforgedIngot());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x3B2, "The forest sprite's touch stings with nature's fury!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 2010, 0, EffectLayer.Waist);
                defender.ApplyPoison(this, Poison.Lesser);
            }
        }

        public override Poison HitPoison => Poison.Lesser;
        public override Poison PoisonImmune => Poison.Regular;

        public ForestSprite(Serial serial) : base(serial)
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
