using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a desert mummy corpse")]
    public class DesertMummy : BaseCreature
    {
        [Constructable]
        public DesertMummy() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a desert mummy";
            Body = 154;
            Hue = 1719;
            BaseSoundID = 471;

            SetStr(200, 250);
            SetDex(60, 80);
            SetInt(60, 80);

            SetHits(150, 200);
            SetDamage(12, 18);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Poison, 20);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 50.0, 70.0);
            SetSkill(SkillName.Tactics, 70.0, 90.0);
            SetSkill(SkillName.Wrestling, 70.0, 90.0);

            Fame = 5500;
            Karma = -5500;
            VirtualArmor = 40;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average);

            PackItem(new SandstoneOre(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new SunforgedIngot(Utility.RandomMinMax(1, 2)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new SandstoneOre());

            if (Utility.RandomDouble() < 0.05)
                PackItem(new Bandage(Utility.RandomMinMax(5, 15)));
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x5D, "The mummy's curse weakens you!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1719, 0, EffectLayer.Waist);

                // Curse effect - weaken stats
                defender.AddStatMod(new StatMod(StatType.Str, "MummyCurseStr", -15, TimeSpan.FromSeconds(20)));
                defender.AddStatMod(new StatMod(StatType.Dex, "MummyCurseDex", -15, TimeSpan.FromSeconds(20)));

                // Apply bandage DoT (mummy rot)
                Timer.DelayCall(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3), 4, () =>
                {
                    if (defender != null && !defender.Deleted && defender.Alive)
                    {
                        AOS.Damage(defender, this, Utility.RandomMinMax(3, 8), 0, 0, 0, 100, 0);
                    }
                });
            }
        }

        public override Poison HitPoison => Poison.Regular;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool BleedImmune => true;
        public override bool AlwaysMurderer => true;

        public DesertMummy(Serial serial) : base(serial)
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
