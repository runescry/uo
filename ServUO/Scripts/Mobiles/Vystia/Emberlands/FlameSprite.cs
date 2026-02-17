using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a flame sprite corpse")]
    public class FlameSprite : BaseCreature
    {
        [Constructable]
        public FlameSprite() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a flame sprite";
            Body = 58;
            Hue = 1358;
            BaseSoundID = 466;

            SetStr(60, 80);
            SetDex(120, 150);
            SetInt(90, 110);

            SetHits(50, 70);
            SetDamage(5, 10);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 80);

            SetResistance(ResistanceType.Physical, 20, 30);
            SetResistance(ResistanceType.Fire, 70, 80);
            SetResistance(ResistanceType.Cold, 5, 15);
            SetResistance(ResistanceType.Poison, 25, 35);
            SetResistance(ResistanceType.Energy, 35, 45);

            SetSkill(SkillName.EvalInt, 50.0, 70.0);
            SetSkill(SkillName.Magery, 50.0, 70.0);
            SetSkill(SkillName.MagicResist, 40.0, 60.0);
            SetSkill(SkillName.Tactics, 40.0, 60.0);
            SetSkill(SkillName.Wrestling, 30.0, 50.0);

            Fame = 2500;
            Karma = -2500;
            VirtualArmor = 20;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.LowScrolls);

            if (Utility.RandomDouble() < 0.15)
                PackItem(new MoltenOre(Utility.RandomMinMax(1, 2)));

            if (Utility.RandomDouble() < 0.05)
                PackItem(new EverburningCoal());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x22, "The flame sprite's touch burns you!");
                defender.FixedParticles(0x3709, 10, 15, 5052, 1358, 0, EffectLayer.Waist);

                int damage = Utility.RandomMinMax(4, 8);
                AOS.Damage(defender, this, damage, 0, 100, 0, 0, 0);
            }
        }

        public FlameSprite(Serial serial) : base(serial)
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
