using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a frozen horror corpse")]
    public class FrozenHorror : BaseCreature
    {
        [Constructable]
        public FrozenHorror() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a frozen horror";
            Body = 312;
            Hue = 1151;
            BaseSoundID = 0x165;

            SetStr(250, 300);
            SetDex(70, 90);
            SetInt(80, 100);

            SetHits(200, 250);
            SetDamage(15, 22);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Cold, 60);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 5, 10);
            SetResistance(ResistanceType.Cold, 75, 85);
            SetResistance(ResistanceType.Poison, 35, 45);
            SetResistance(ResistanceType.Energy, 35, 45);

            SetSkill(SkillName.MagicResist, 70.0, 90.0);
            SetSkill(SkillName.Tactics, 80.0, 100.0);
            SetSkill(SkillName.Wrestling, 80.0, 100.0);

            Fame = 8000;
            Karma = -8000;
            VirtualArmor = 45;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Gems, 2);

            PackItem(new FrozenOre(Utility.RandomMinMax(3, 6)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new FrostforgedIngot(Utility.RandomMinMax(1, 3)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new IceCrystal(Utility.RandomMinMax(1, 2)));

            if (Utility.RandomDouble() < 0.05)
                PackItem(new EternalIce());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x480, "The frozen horror's grasp freezes you in place!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1151, 0, EffectLayer.Waist);
                defender.PlaySound(0x204);

                // Freeze effect
                defender.Freeze(TimeSpan.FromSeconds(2));

                // Cold DoT
                Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), 3, () =>
                {
                    if (defender != null && !defender.Deleted && defender.Alive)
                    {
                        AOS.Damage(defender, this, Utility.RandomMinMax(5, 10), 0, 0, 100, 0, 0);
                    }
                });
            }
        }

        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 3;

        public FrozenHorror(Serial serial) : base(serial)
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
