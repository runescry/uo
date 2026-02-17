using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a zephyr remains")]
    public class Zephyr : BaseCreature
    {
        [Constructable]
        public Zephyr() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a zephyr";
            Body = 13;
            Hue = 1281;
            BaseSoundID = 655;

            SetStr(100, 130);
            SetDex(150, 180);
            SetInt(100, 130);

            SetHits(90, 120);
            SetDamage(9, 15);

            SetDamageType(ResistanceType.Physical, 30);
            SetDamageType(ResistanceType.Cold, 35);
            SetDamageType(ResistanceType.Energy, 35);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 65, 75);

            SetSkill(SkillName.EvalInt, 70.0, 90.0);
            SetSkill(SkillName.Magery, 70.0, 90.0);
            SetSkill(SkillName.MagicResist, 65.0, 85.0);
            SetSkill(SkillName.Tactics, 55.0, 75.0);
            SetSkill(SkillName.Wrestling, 55.0, 75.0);

            Fame = 5000;
            Karma = -5000;
            VirtualArmor = 32;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.MedScrolls);

            PackItem(new CrystalOre(Utility.RandomMinMax(3, 6)));

            if (Utility.RandomDouble() < 0.12)
                PackItem(new StormCrystal());

            if (Utility.RandomDouble() < 0.08)
                PackItem(new StormCrystal());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x480, "The zephyr's chilling winds cut through you!");
                defender.FixedParticles(0x376A, 9, 32, 5007, 1281, 0, EffectLayer.Waist);
                defender.PlaySound(0x10B);

                int damage = Utility.RandomMinMax(6, 12);
                AOS.Damage(defender, this, damage, 0, 0, 50, 0, 50);

                // Speed reduction
                defender.SendMessage(0x480, "The icy wind slows your movements!");
                defender.AddStatMod(new StatMod(StatType.Dex, "ZephyrSlow", -15, TimeSpan.FromSeconds(8)));
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Gust attack
            if (Combatant != null && Utility.RandomDouble() < 0.03)
            {
                DoGustAttack();
            }
        }

        private void DoGustAttack()
        {
            if (Map == null || Combatant == null)
                return;

            Mobile target = Combatant as Mobile;
            if (target == null || !CanBeHarmful(target))
                return;

            Say("*The zephyr unleashes a powerful gust*");
            PlaySound(0x10B);

            DoHarmful(target);
            target.FixedParticles(0x376A, 9, 32, 5007, 1281, 0, EffectLayer.Waist);

            int damage = Utility.RandomMinMax(15, 25);
            AOS.Damage(target, this, damage, 0, 0, 50, 0, 50);

            // Knockback effect
            target.Freeze(TimeSpan.FromSeconds(1.0));
            target.SendMessage(0x480, "The gust knocks you off balance!");
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Regular;

        public Zephyr(Serial serial) : base(serial)
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
