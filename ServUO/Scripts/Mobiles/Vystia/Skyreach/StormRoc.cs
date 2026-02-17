using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a storm roc corpse")]
    public class StormRoc : BaseCreature
    {
        [Constructable]
        public StormRoc() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a storm roc";
            Body = 5;
            Hue = 1281;
            BaseSoundID = 0x2EE;

            SetStr(380, 460);
            SetDex(120, 150);
            SetInt(100, 140);

            SetHits(400, 500);
            SetDamage(18, 28);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 35, 45);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 75, 85);

            SetSkill(SkillName.EvalInt, 70.0, 90.0);
            SetSkill(SkillName.Magery, 70.0, 90.0);
            SetSkill(SkillName.MagicResist, 85.0, 105.0);
            SetSkill(SkillName.Tactics, 90.0, 110.0);
            SetSkill(SkillName.Wrestling, 90.0, 110.0);

            Fame = 15000;
            Karma = -15000;
            VirtualArmor = 58;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);

            PackItem(new CrystalOre(Utility.RandomMinMax(8, 15)));
            PackItem(new CrystallineIngot(Utility.RandomMinMax(4, 8)));

            if (Utility.RandomDouble() < 0.20)
                PackItem(new StormCrystal(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new StormCrystal(Utility.RandomMinMax(1, 2)));
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x480, "The storm roc's talons crackle with lightning!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1281, 0, EffectLayer.Waist);
                defender.PlaySound(0x1E1);

                int damage = Utility.RandomMinMax(15, 25);
                AOS.Damage(defender, this, damage, 0, 0, 0, 0, 100);

                // Bleeding from talons
                if (Utility.RandomDouble() < 0.40)
                {
                    defender.SendMessage(0x22, "The roc's talons tear deep wounds!");
                    DoBleedEffect(defender);
                }
            }
        }

        private void DoBleedEffect(Mobile target)
        {
            if (target == null || target.Deleted || !target.Alive)
                return;

            int bleedDamage = Utility.RandomMinMax(4, 8);
            int ticks = 5;

            for (int i = 0; i < ticks; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(2.0 * (i + 1)), () =>
                {
                    if (target != null && !target.Deleted && target.Alive)
                    {
                        target.Damage(bleedDamage, this);
                        target.FixedParticles(0x377A, 244, 25, 9950, 31, 0, EffectLayer.Waist);
                    }
                });
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Thunderous dive attack
            if (Combatant != null && Utility.RandomDouble() < 0.02)
            {
                DoThunderousDive();
            }
        }

        private void DoThunderousDive()
        {
            if (Map == null || Combatant == null)
                return;

            Mobile target = Combatant as Mobile;
            if (target == null || !CanBeHarmful(target))
                return;

            Say("*The storm roc dives with a thunderous screech*");
            PlaySound(0x29);
            PlaySound(0x2EE);

            DoHarmful(target);
            Effects.SendBoltEffect(target, true, 1281);
            target.FixedParticles(0x3779, 10, 30, 5013, 1281, 0, EffectLayer.Head);

            int damage = Utility.RandomMinMax(40, 60);
            AOS.Damage(target, this, damage, 50, 0, 0, 0, 50);

            // Stun from impact
            target.Freeze(TimeSpan.FromSeconds(2.0));
            target.SendMessage(0x480, "The thunderous dive stuns you!");

            // AoE effect
            foreach (Mobile m in target.GetMobilesInRange(3))
            {
                if (m != this && m != target && CanBeHarmful(m))
                {
                    if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                    {
                        DoHarmful(m);
                        int aoeDamage = Utility.RandomMinMax(15, 25);
                        AOS.Damage(m, this, aoeDamage, 0, 0, 0, 0, 100);
                        m.SendMessage(0x480, "You are caught in the shockwave!");
                    }
                }
            }
        }

        public override int Feathers => 50;
        public override int Meat => 15;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 5;

        public StormRoc(Serial serial) : base(serial)
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
