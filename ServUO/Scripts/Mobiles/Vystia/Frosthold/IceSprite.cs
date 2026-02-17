using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an ice sprite corpse")]
    public class IceSprite : BaseCreature
    {
        [Constructable]
        public IceSprite() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an ice sprite";
            Body = 58;
            Hue = 1150;
            BaseSoundID = 466;

            SetStr(50, 80);
            SetDex(100, 130);
            SetInt(80, 100);

            SetHits(50, 75);
            SetDamage(5, 10);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Cold, 80);

            SetResistance(ResistanceType.Physical, 20, 30);
            SetResistance(ResistanceType.Fire, 5, 10);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.EvalInt, 50.0, 70.0);
            SetSkill(SkillName.Magery, 50.0, 70.0);
            SetSkill(SkillName.MagicResist, 40.0, 60.0);
            SetSkill(SkillName.Tactics, 40.0, 60.0);
            SetSkill(SkillName.Wrestling, 30.0, 50.0);

            Fame = 2000;
            Karma = -2000;
            VirtualArmor = 20;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.LowScrolls);

            if (Utility.RandomDouble() < 0.20)
                PackItem(new FrozenOre(Utility.RandomMinMax(1, 2)));

            if (Utility.RandomDouble() < 0.05)
                PackItem(new FrostEssence());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x480, "The ice sprite's touch chills you!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1150, 0, EffectLayer.Waist);

                int damage = Utility.RandomMinMax(3, 6);
                AOS.Damage(defender, this, damage, 0, 0, 100, 0, 0);
            }
        }

        public IceSprite(Serial serial) : base(serial)
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
