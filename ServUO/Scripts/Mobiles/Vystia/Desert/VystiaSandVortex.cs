using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a sand vortex corpse")]
    public class VystiaSandVortex : BaseCreature
    {
        [Constructable]
        public VystiaSandVortex() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a sand vortex";
            Body = 790;
            Hue = 1719;
            BaseSoundID = 263;

            SetStr(200, 260);
            SetDex(150, 180);
            SetInt(60, 80);

            SetHits(220, 300);
            SetDamage(14, 22);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 70.0, 90.0);
            SetSkill(SkillName.Tactics, 80.0, 100.0);
            SetSkill(SkillName.Wrestling, 85.0, 105.0);

            Fame = 5500;
            Karma = -5500;
            VirtualArmor = 40;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);

            PackItem(new SandstoneOre(Utility.RandomMinMax(3, 6)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new SunforgedIngot());
        }

        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 3;

        public VystiaSandVortex(Serial serial) : base(serial)
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
