using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an abyssal crab corpse")]
    public class AbyssalCrab : BaseCreature
    {
        [Constructable]
        public AbyssalCrab() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an abyssal crab";
            Body = 721;
            Hue = 1365;
            BaseSoundID = 0x4F2;

            SetStr(180, 220);
            SetDex(80, 100);
            SetInt(30, 50);

            SetHits(180, 220);
            SetDamage(14, 20);

            SetDamageType(ResistanceType.Physical, 80);
            SetDamageType(ResistanceType.Cold, 20);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 25, 35);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 45, 55);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 60.0, 80.0);
            SetSkill(SkillName.Tactics, 75.0, 95.0);
            SetSkill(SkillName.Wrestling, 80.0, 100.0);

            Fame = 5000;
            Karma = -5000;
            VirtualArmor = 50;

            CanSwim = true;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);

            PackItem(new ObsidianOre(Utility.RandomMinMax(2, 5)));

            if (Utility.RandomDouble() < 0.08)
                PackItem(new VoidforgedIngot());

            if (Utility.RandomDouble() < 0.04)
                PackItem(new VoidforgedIngot());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.35)
            {
                defender.SendMessage(0x480, "The abyssal crab's claws crush you!");
                defender.FixedParticles(0x376A, 9, 32, 5007, 1365, 0, EffectLayer.Waist);
                defender.PlaySound(0x1E4);

                int damage = Utility.RandomMinMax(10, 16);
                AOS.Damage(defender, this, damage, 100, 0, 0, 0, 0);

                // Armor crush - reduce physical resistance
                defender.AddStatMod(new StatMod(StatType.Str, "CrabCrush", -10, TimeSpan.FromSeconds(10)));
                defender.SendMessage(0x480, "Your armor buckles under the crushing blow!");
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            // Defensive pinch when damaged
            if (from != null && Utility.RandomDouble() < 0.20 && InRange(from, 1))
            {
                from.SendMessage(0x480, "The crab retaliates with a defensive pinch!");
                from.FixedParticles(0x377A, 244, 25, 9950, 1365, 0, EffectLayer.Waist);

                int damage = Utility.RandomMinMax(6, 10);
                AOS.Damage(from, this, damage, 100, 0, 0, 0, 0);
            }
        }

        public override int Meat => 5;
        public override bool AlwaysMurderer => true;

        public AbyssalCrab(Serial serial) : base(serial)
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
