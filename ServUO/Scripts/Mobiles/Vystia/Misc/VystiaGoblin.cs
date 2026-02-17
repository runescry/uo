using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a vystia goblin corpse")]
    public class VystiaGoblin : BaseCreature
    {
        [Constructable]
        public VystiaGoblin() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a vystia goblin";
            Body = 723;
            BaseSoundID = 0x45A;

            SetStr(80, 110);
            SetDex(100, 130);
            SetInt(40, 60);

            SetHits(70, 100);
            SetDamage(7, 12);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 25, 35);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 25, 35);
            SetResistance(ResistanceType.Energy, 15, 25);

            SetSkill(SkillName.MagicResist, 45.0, 65.0);
            SetSkill(SkillName.Tactics, 55.0, 75.0);
            SetSkill(SkillName.Wrestling, 50.0, 70.0);
            SetSkill(SkillName.Fencing, 55.0, 75.0);

            Fame = 2000;
            Karma = -2000;
            VirtualArmor = 25;

            if (Utility.RandomBool())
                AddItem(new Dagger());
            else
                AddItem(new ShortSpear());
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);

            PackGold(40, 80);

            if (Utility.RandomDouble() < 0.20)
                PackItem(new IronOre(Utility.RandomMinMax(1, 3)));
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            // Sneaky strike - extra damage from behind
            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x22, "The goblin lands a sneaky strike!");
                int damage = Utility.RandomMinMax(5, 10);
                AOS.Damage(defender, this, damage, 100, 0, 0, 0, 0);
            }
        }

        public override bool CanRummageCorpses => true;
        public override bool AlwaysMurderer => true;

        public VystiaGoblin(Serial serial) : base(serial)
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
