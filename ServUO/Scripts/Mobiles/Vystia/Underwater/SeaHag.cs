using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a sea hag corpse")]
    public class SeaHag : BaseCreature
    {
        [Constructable]
        public SeaHag() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a sea hag";
            Body = 4;
            Hue = 1365;
            BaseSoundID = 0x4B0;

            SetStr(140, 180);
            SetDex(100, 130);
            SetInt(180, 230);

            SetHits(160, 200);
            SetDamage(10, 16);

            SetDamageType(ResistanceType.Physical, 30);
            SetDamageType(ResistanceType.Cold, 40);
            SetDamageType(ResistanceType.Poison, 30);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 65, 75);
            SetResistance(ResistanceType.Energy, 35, 45);

            SetSkill(SkillName.EvalInt, 90.0, 110.0);
            SetSkill(SkillName.Magery, 90.0, 110.0);
            SetSkill(SkillName.MagicResist, 85.0, 105.0);
            SetSkill(SkillName.Tactics, 60.0, 80.0);
            SetSkill(SkillName.Wrestling, 55.0, 75.0);
            SetSkill(SkillName.Meditation, 80.0, 100.0);

            Fame = 9000;
            Karma = -9000;
            VirtualArmor = 40;

            CanSwim = true;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.MedScrolls, 2);
            AddLoot(LootPack.Potions);

            PackItem(new ObsidianOre(Utility.RandomMinMax(3, 6)));

            if (Utility.RandomDouble() < 0.12)
                PackItem(new ObsidianOre());

            if (Utility.RandomDouble() < 0.08)
                PackItem(new VoidforgedIngot());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x480, "The sea hag's cursed touch weakens you!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1365, 0, EffectLayer.Waist);
                defender.PlaySound(0x1FB);

                // Curse effect
                defender.AddStatMod(new StatMod(StatType.Str, "SeaHagCurse_Str", -15, TimeSpan.FromSeconds(15)));
                defender.AddStatMod(new StatMod(StatType.Int, "SeaHagCurse_Int", -15, TimeSpan.FromSeconds(15)));
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Drowning curse
            if (Combatant != null && Utility.RandomDouble() < 0.025)
            {
                DoDrowningCurse();
            }
        }

        private void DoDrowningCurse()
        {
            if (Map == null || Combatant == null)
                return;

            Mobile target = Combatant as Mobile;
            if (target == null || !CanBeHarmful(target))
                return;

            Say("*The sea hag invokes a drowning curse*");
            PlaySound(0x1FB);

            DoHarmful(target);
            target.FixedParticles(0x3818, 10, 30, 5052, 1365, 0, EffectLayer.Head);
            target.PlaySound(0x026);

            target.SendMessage(0x480, "You feel as if you're drowning!");

            // DoT effect - "drowning"
            int ticks = 5;
            for (int i = 0; i < ticks; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(1.5 * (i + 1)), () =>
                {
                    if (target != null && !target.Deleted && target.Alive)
                    {
                        int damage = Utility.RandomMinMax(10, 15);
                        AOS.Damage(target, this, damage, 0, 0, 50, 50, 0);
                        target.FixedParticles(0x3818, 5, 10, 5052, 1365, 0, EffectLayer.Head);
                    }
                });
            }
        }

        public override Poison PoisonImmune => Poison.Greater;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 3;

        public SeaHag(Serial serial) : base(serial)
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
