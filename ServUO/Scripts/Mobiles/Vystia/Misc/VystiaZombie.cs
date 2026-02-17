using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a vystia zombie corpse")]
    public class VystiaZombie : BaseCreature
    {
        [Constructable]
        public VystiaZombie() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.6)
        {
            Name = "a vystia zombie";
            Body = 3;
            BaseSoundID = 471;

            SetStr(90, 120);
            SetDex(50, 70);
            SetInt(20, 40);

            SetHits(80, 110);
            SetDamage(8, 14);

            SetDamageType(ResistanceType.Physical, 70);
            SetDamageType(ResistanceType.Poison, 30);

            SetResistance(ResistanceType.Physical, 25, 35);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.MagicResist, 40.0, 60.0);
            SetSkill(SkillName.Tactics, 50.0, 70.0);
            SetSkill(SkillName.Wrestling, 55.0, 75.0);

            Fame = 1800;
            Karma = -1800;
            VirtualArmor = 24;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Poor);
            AddLoot(LootPack.Meager);

            if (Utility.RandomDouble() < 0.20)
                PackItem(new Bone(Utility.RandomMinMax(1, 2)));
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            // Disease from rotting flesh
            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x22, "The zombie's rotting touch infects you!");
                defender.ApplyPoison(this, Poison.Lesser);
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool AlwaysMurderer => true;

        public VystiaZombie(Serial serial) : base(serial)
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
