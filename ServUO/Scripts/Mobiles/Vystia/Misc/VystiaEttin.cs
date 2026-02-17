using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a vystia ettin corpse")]
    public class VystiaEttin : BaseCreature
    {
        [Constructable]
        public VystiaEttin() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a vystia ettin";
            Body = 18;
            BaseSoundID = 367;

            SetStr(180, 230);
            SetDex(60, 80);
            SetInt(40, 60);

            SetHits(180, 240);
            SetDamage(14, 22);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 25, 35);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.MagicResist, 55.0, 75.0);
            SetSkill(SkillName.Tactics, 70.0, 90.0);
            SetSkill(SkillName.Wrestling, 75.0, 95.0);

            Fame = 5000;
            Karma = -5000;
            VirtualArmor = 40;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Rich);

            PackGold(100, 200);

            if (Utility.RandomDouble() < 0.30)
                PackItem(new Club());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            // Double hit (two heads!)
            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x22, "Both heads of the ettin strike you!");
                int damage = Utility.RandomMinMax(8, 14);
                AOS.Damage(defender, this, damage, 100, 0, 0, 0, 0);
            }
        }

        public override bool CanRummageCorpses => true;
        public override int Meat => 4;
        public override bool AlwaysMurderer => true;

        public VystiaEttin(Serial serial) : base(serial)
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
