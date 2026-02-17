using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a vystia spider corpse")]
    public class VystiaSpider : BaseCreature
    {
        [Constructable]
        public VystiaSpider() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a vystia spider";
            Body = 28;
            BaseSoundID = 0x388;

            SetStr(100, 130);
            SetDex(110, 140);
            SetInt(30, 50);

            SetHits(90, 120);
            SetDamage(9, 15);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Poison, 40);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 25, 35);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.MagicResist, 50.0, 70.0);
            SetSkill(SkillName.Tactics, 65.0, 85.0);
            SetSkill(SkillName.Wrestling, 70.0, 90.0);
            SetSkill(SkillName.Poisoning, 70.0, 90.0);

            Fame = 2800;
            Karma = -2800;
            VirtualArmor = 30;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 75.0;

            PackItem(new SpidersSilk(Utility.RandomMinMax(3, 6)));
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.Average);

            if (Utility.RandomDouble() < 0.20)
                PackItem(new SpidersSilk(Utility.RandomMinMax(5, 10)));
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            // Web slow
            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x22, "The spider's webbing slows you!");
                defender.AddStatMod(new StatMod(StatType.Dex, "SpiderWeb", -20, TimeSpan.FromSeconds(8)));
            }
        }

        public override Poison PoisonImmune => Poison.Regular;
        public override Poison HitPoison => Poison.Regular;
        public override FoodType FavoriteFood => FoodType.Meat;
        public override bool AlwaysMurderer => true;

        public VystiaSpider(Serial serial) : base(serial)
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
