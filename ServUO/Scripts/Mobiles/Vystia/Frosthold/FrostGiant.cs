using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a frost giant corpse")]
    public class FrostGiant : BaseCreature
    {
        [Constructable]
        public FrostGiant() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a frost giant";
            Body = 76;
            Hue = 1152;
            BaseSoundID = 609;

            SetStr(400, 500);
            SetDex(80, 100);
            SetInt(60, 80);

            SetHits(800, 1000);
            SetDamage(18, 28);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Cold, 50);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 80, 90);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 80.0, 100.0);
            SetSkill(SkillName.Tactics, 90.0, 110.0);
            SetSkill(SkillName.Wrestling, 90.0, 110.0);
            SetSkill(SkillName.Anatomy, 70.0, 90.0);

            Fame = 15000;
            Karma = -15000;
            VirtualArmor = 50;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.Rich);

            PackItem(new FrozenOre(Utility.RandomMinMax(5, 10)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new FrostforgedIngot(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.05)
                PackItem(new FrostEssence(Utility.RandomMinMax(1, 2)));
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x480, "The frost giant's blow chills you to the bone!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1152, 0, EffectLayer.Waist);

                // Slow effect
                defender.AddStatMod(new StatMod(StatType.Dex, "FrostGiantChill", -20, TimeSpan.FromSeconds(10)));
            }
        }

        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 4;

        public FrostGiant(Serial serial) : base(serial)
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
