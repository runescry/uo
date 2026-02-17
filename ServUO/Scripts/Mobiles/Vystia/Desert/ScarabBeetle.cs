using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a scarab beetle corpse")]
    public class ScarabBeetle : BaseCreature
    {
        [Constructable]
        public ScarabBeetle() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a scarab beetle";
            Body = 244;
            Hue = 1719;
            BaseSoundID = 1006;

            SetStr(80, 100);
            SetDex(80, 100);
            SetInt(20, 40);

            SetHits(60, 80);
            SetDamage(6, 12);

            SetDamageType(ResistanceType.Physical, 70);
            SetDamageType(ResistanceType.Poison, 30);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 25, 35);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 30.0, 50.0);
            SetSkill(SkillName.Tactics, 50.0, 70.0);
            SetSkill(SkillName.Wrestling, 50.0, 70.0);

            Fame = 2000;
            Karma = -2000;
            VirtualArmor = 30;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);

            if (Utility.RandomDouble() < 0.20)
                PackItem(new SandstoneOre());

            if (Utility.RandomDouble() < 0.10)
                PackItem(new SandstoneOre());

            if (Utility.RandomDouble() < 0.03)
                PackItem(new SunforgedIngot());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.20)
            {
                defender.SendMessage(0x5D, "The scarab beetle's mandibles inject venom!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 68, 0, EffectLayer.Waist);
                defender.ApplyPoison(this, Poison.Regular);
            }
        }

        public override Poison HitPoison => Poison.Regular;
        public override Poison PoisonImmune => Poison.Regular;

        public ScarabBeetle(Serial serial) : base(serial)
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
