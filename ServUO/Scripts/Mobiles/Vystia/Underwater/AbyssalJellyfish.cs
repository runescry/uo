using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an abyssal jellyfish remains")]
    public class AbyssalJellyfish : BaseCreature
    {
        [Constructable]
        public AbyssalJellyfish() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an abyssal jellyfish";
            Body = 264;
            Hue = 1365;
            BaseSoundID = 0x388;

            SetStr(80, 110);
            SetDex(100, 130);
            SetInt(120, 160);

            SetHits(80, 110);
            SetDamage(8, 14);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Cold, 40);
            SetDamageType(ResistanceType.Energy, 40);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 15, 25);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 55, 65);

            SetSkill(SkillName.EvalInt, 70.0, 90.0);
            SetSkill(SkillName.Magery, 70.0, 90.0);
            SetSkill(SkillName.MagicResist, 65.0, 85.0);
            SetSkill(SkillName.Tactics, 50.0, 70.0);
            SetSkill(SkillName.Wrestling, 45.0, 65.0);

            Fame = 4000;
            Karma = -4000;
            VirtualArmor = 28;

            CanSwim = true;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.LowScrolls);

            PackItem(new ObsidianOre(Utility.RandomMinMax(1, 3)));

            if (Utility.RandomDouble() < 0.08)
                PackItem(new ObsidianOre());

            if (Utility.RandomDouble() < 0.05)
                PackItem(new VoidforgedIngot());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.40)
            {
                defender.SendMessage(0x480, "The jellyfish's stinging tentacles paralyze you!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1365, 0, EffectLayer.Waist);
                defender.PlaySound(0x1E1);

                int damage = Utility.RandomMinMax(6, 12);
                AOS.Damage(defender, this, damage, 0, 0, 50, 0, 50);

                // Paralysis
                defender.Freeze(TimeSpan.FromSeconds(1.5));

                // Apply poison from sting
                defender.ApplyPoison(this, Poison.Regular);
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            // Electric discharge when damaged
            if (from != null && Utility.RandomDouble() < 0.25)
            {
                from.SendMessage(0x480, "The jellyfish discharges electricity!");
                from.FixedParticles(0x374A, 10, 15, 5028, 1365, 0, EffectLayer.Waist);
                from.PlaySound(0x1E1);

                int damage = Utility.RandomMinMax(8, 14);
                AOS.Damage(from, this, damage, 0, 0, 0, 0, 100);
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override Poison HitPoison => Poison.Regular;

        public AbyssalJellyfish(Serial serial) : base(serial)
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
