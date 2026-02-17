using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a tidal elemental corpse")]
    public class TidalElemental : BaseCreature
    {
        [Constructable]
        public TidalElemental() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a tidal elemental";
            Body = 16;
            Hue = 1365;
            BaseSoundID = 278;

            SetStr(200, 260);
            SetDex(90, 120);
            SetInt(120, 160);

            SetHits(180, 240);
            SetDamage(12, 18);

            SetDamageType(ResistanceType.Physical, 30);
            SetDamageType(ResistanceType.Cold, 70);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 15, 25);
            SetResistance(ResistanceType.Cold, 70, 80);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.EvalInt, 75.0, 95.0);
            SetSkill(SkillName.Magery, 75.0, 95.0);
            SetSkill(SkillName.MagicResist, 70.0, 90.0);
            SetSkill(SkillName.Tactics, 70.0, 90.0);
            SetSkill(SkillName.Wrestling, 65.0, 85.0);

            Fame = 7500;
            Karma = -7500;
            VirtualArmor = 42;

            CanSwim = true;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.MedScrolls);

            PackItem(new ObsidianOre(Utility.RandomMinMax(4, 8)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new VoidforgedIngot(Utility.RandomMinMax(1, 3)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new VoidforgedIngot());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x480, "The tidal elemental's watery touch chills you!");
                defender.FixedParticles(0x3818, 10, 15, 5052, 1365, 0, EffectLayer.Waist);
                defender.PlaySound(0x026);

                int damage = Utility.RandomMinMax(8, 14);
                AOS.Damage(defender, this, damage, 0, 0, 100, 0, 0);

                // Slow from cold water
                defender.AddStatMod(new StatMod(StatType.Dex, "TidalChill", -15, TimeSpan.FromSeconds(8)));
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Water spout attack
            if (Combatant != null && Utility.RandomDouble() < 0.03)
            {
                DoWaterSpout();
            }
        }

        private void DoWaterSpout()
        {
            if (Map == null || Combatant == null)
                return;

            Mobile target = Combatant as Mobile;
            if (target == null || !CanBeHarmful(target))
                return;

            Say("*The tidal elemental creates a water spout*");
            PlaySound(0x026);

            DoHarmful(target);
            target.FixedParticles(0x3818, 10, 30, 5052, 1365, 0, EffectLayer.Head);

            int damage = Utility.RandomMinMax(20, 35);
            AOS.Damage(target, this, damage, 0, 0, 100, 0, 0);

            // Lift and disorient
            target.Freeze(TimeSpan.FromSeconds(1.5));
            target.SendMessage(0x480, "The water spout lifts you into the air!");
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Regular;

        public TidalElemental(Serial serial) : base(serial)
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
