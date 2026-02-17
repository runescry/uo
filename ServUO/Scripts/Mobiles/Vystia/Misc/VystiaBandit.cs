using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a vystia bandit corpse")]
    public class VystiaBandit : BaseCreature
    {
        [Constructable]
        public VystiaBandit() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Title = "the bandit";

            if (Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");
                AddItem(new LeatherBustierArms());
                AddItem(new LeatherSkirt());
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
                AddItem(new LeatherChest());
                AddItem(new LeatherLegs());
            }

            SetStr(100, 140);
            SetDex(110, 140);
            SetInt(50, 70);

            SetHits(100, 150);
            SetDamage(10, 16);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 25, 35);
            SetResistance(ResistanceType.Energy, 15, 25);

            SetSkill(SkillName.MagicResist, 50.0, 70.0);
            SetSkill(SkillName.Tactics, 70.0, 90.0);
            SetSkill(SkillName.Wrestling, 60.0, 80.0);
            SetSkill(SkillName.Swords, 70.0, 90.0);
            SetSkill(SkillName.Fencing, 70.0, 90.0);

            Fame = 3000;
            Karma = -3000;
            VirtualArmor = 30;

            // Random weapon
            switch (Utility.Random(4))
            {
                case 0: AddItem(new Longsword()); break;
                case 1: AddItem(new Cutlass()); break;
                case 2: AddItem(new Kryss()); break;
                case 3: AddItem(new WarFork()); break;
            }

            AddItem(new Boots());
            AddItem(new Bandana());

            Utility.AssignRandomHair(this);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);

            PackGold(100, 200);

            if (Utility.RandomDouble() < 0.25)
                PackItem(new Bandage(Utility.RandomMinMax(5, 10)));
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            // Pickpocket attempt - steal gold
            if (Utility.RandomDouble() < 0.15)
            {
                Container pack = defender.Backpack;
                if (pack != null)
                {
                    Item gold = pack.FindItemByType(typeof(Gold));
                    if (gold != null && gold.Amount > 0)
                    {
                        int stolen = Math.Min(gold.Amount, Utility.RandomMinMax(10, 50));
                        gold.Amount -= stolen;
                        if (gold.Amount <= 0)
                            gold.Delete();

                        defender.SendMessage(0x22, "The bandit steals {0} gold from you!", stolen);
                        PackItem(new Gold(stolen));
                    }
                }
            }
        }

        public override bool AlwaysMurderer => true;

        public VystiaBandit(Serial serial) : base(serial)
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
