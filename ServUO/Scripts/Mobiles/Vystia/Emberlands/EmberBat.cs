using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an ember bat corpse")]
    public class EmberBat : BaseCreature
    {
        [Constructable]
        public EmberBat() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an ember bat";
            Body = 317;
            Hue = 1358;
            BaseSoundID = 0x270;

            SetStr(60, 80);
            SetDex(100, 130);
            SetInt(30, 50);

            SetHits(40, 60);
            SetDamage(6, 10);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Fire, 60);

            SetResistance(ResistanceType.Physical, 20, 30);
            SetResistance(ResistanceType.Fire, 60, 70);
            SetResistance(ResistanceType.Cold, 5, 15);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.MagicResist, 30.0, 50.0);
            SetSkill(SkillName.Tactics, 50.0, 70.0);
            SetSkill(SkillName.Wrestling, 50.0, 70.0);

            Fame = 1500;
            Karma = -1500;
            VirtualArmor = 20;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);

            if (Utility.RandomDouble() < 0.10)
                PackItem(new MoltenOre());

            if (Utility.RandomDouble() < 0.03)
                PackItem(new EverburningCoal());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.20)
            {
                defender.SendMessage(0x22, "The ember bat's fiery bite singes you!");
                defender.FixedParticles(0x3709, 10, 15, 5052, 1358, 0, EffectLayer.Waist);

                int damage = Utility.RandomMinMax(3, 6);
                AOS.Damage(defender, this, damage, 0, 100, 0, 0, 0);
            }
        }

        public EmberBat(Serial serial) : base(serial)
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
