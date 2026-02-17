using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an ember phoenix corpse")]
    public class VystiaEmberPhoenix : BaseCreature
    {
        [Constructable]
        public VystiaEmberPhoenix() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an ember phoenix";
            Body = 5;
            Hue = 1358;
            BaseSoundID = 143;

            SetStr(400, 480);
            SetDex(150, 180);
            SetInt(200, 260);

            SetHits(450, 550);
            SetDamage(18, 26);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 80);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 95, 100);
            SetResistance(ResistanceType.Cold, 0, 5);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.EvalInt, 90.0, 110.0);
            SetSkill(SkillName.Magery, 90.0, 110.0);
            SetSkill(SkillName.MagicResist, 100.0, 120.0);
            SetSkill(SkillName.Tactics, 85.0, 105.0);
            SetSkill(SkillName.Wrestling, 80.0, 100.0);

            Fame = 15000;
            Karma = -15000;
            VirtualArmor = 55;

            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.Rich);

            PackItem(new MoltenOre(Utility.RandomMinMax(8, 15)));
            PackItem(new EmberforgedIngot(Utility.RandomMinMax(4, 8)));

            // REMOVED OLD REAGENT:
            // if (Utility.RandomDouble() < 0.15)
            //     PackItem(new EmberBloom(Utility.RandomMinMax(1, 2)));
            if (Utility.RandomDouble() < 0.20)
                PackItem(new EverburningCoal(Utility.RandomMinMax(1, 3)));
        }

        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 4;

        public VystiaEmberPhoenix(Serial serial) : base(serial)
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
