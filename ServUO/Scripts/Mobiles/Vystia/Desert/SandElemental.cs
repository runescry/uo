using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a sand elemental corpse")]
    public class SandElemental : BaseCreature
    {
        [Constructable]
        public SandElemental() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a sand elemental";
            Body = 14;
            Hue = 1719;
            BaseSoundID = 268;

            SetStr(180, 220);
            SetDex(90, 110);
            SetInt(80, 100);

            SetHits(150, 200);
            SetDamage(12, 18);

            SetDamageType(ResistanceType.Physical, 80);
            SetDamageType(ResistanceType.Fire, 20);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.MagicResist, 60.0, 80.0);
            SetSkill(SkillName.Tactics, 70.0, 90.0);
            SetSkill(SkillName.Wrestling, 70.0, 90.0);

            Fame = 5500;
            Karma = -5500;
            VirtualArmor = 40;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Rich);

            PackItem(new SandstoneOre(Utility.RandomMinMax(2, 5)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new SunforgedIngot(Utility.RandomMinMax(1, 2)));

            if (Utility.RandomDouble() < 0.05)
                PackItem(new SandstoneOre());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x5D, "Sand gets in your eyes, blinding you!");
                defender.FixedParticles(0x376A, 9, 32, 5030, 1719, 0, EffectLayer.Head);

                // Blind effect - reduce accuracy
                defender.AddStatMod(new StatMod(StatType.Dex, "SandBlind", -20, TimeSpan.FromSeconds(5)));
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            // Chance to disperse and reform
            if (Utility.RandomDouble() < 0.10)
            {
                Say("*The sand elemental disperses and reforms*");
                FixedParticles(0x376A, 9, 32, 5030, 1719, 0, EffectLayer.Waist);
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Regular;
        public override bool AlwaysMurderer => true;

        public SandElemental(Serial serial) : base(serial)
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
