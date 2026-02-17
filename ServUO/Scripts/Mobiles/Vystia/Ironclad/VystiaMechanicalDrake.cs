using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a mechanical drake corpse")]
    public class VystiaMechanicalDrake : BaseCreature
    {
        [Constructable]
        public VystiaMechanicalDrake() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {


            Name = "a mechanical drake";
            Body = 61;
            Hue = 2305;
            BaseSoundID = 362;

            SetStr(350, 420);
            SetDex(80, 100);
            SetInt(100, 140);

            SetHits(380, 460);
            SetDamage(16, 24);

            SetDamageType(ResistanceType.Physical, 70);
            SetDamageType(ResistanceType.Fire, 30);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.MagicResist, 80.0, 100.0);
            SetSkill(SkillName.Tactics, 90.0, 110.0);
            SetSkill(SkillName.Wrestling, 90.0, 110.0);

            Fame = 10000;
            Karma = -10000;
            VirtualArmor = 55;

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 95.0;

            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);

            PackItem(new SteamworkOre(Utility.RandomMinMax(6, 12)));
            PackItem(new ClockworkIngot(Utility.RandomMinMax(3, 6)));

            if (Utility.RandomDouble() < 0.20)
                PackItem(new ClockworkSpring(Utility.RandomMinMax(1, 3)));
            if (Utility.RandomDouble() < 0.08)
                PackItem(new SteamCore());
        }

        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 4;

        public VystiaMechanicalDrake(Serial serial) : base(serial)
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