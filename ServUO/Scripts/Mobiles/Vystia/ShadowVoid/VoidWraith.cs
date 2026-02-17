using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a void wraith remains")]
    public class VoidWraith : BaseCreature
    {
        [Constructable]
        public VoidWraith() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a void wraith";
            Body = 26;
            Hue = 1109;
            BaseSoundID = 0x482;

            SetStr(150, 200);
            SetDex(130, 160);
            SetInt(200, 260);

            SetHits(180, 240);
            SetDamage(11, 17);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Cold, 40);
            SetDamageType(ResistanceType.Energy, 40);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 25, 35);
            SetResistance(ResistanceType.Cold, 65, 75);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 65, 75);

            SetSkill(SkillName.EvalInt, 95.0, 115.0);
            SetSkill(SkillName.Magery, 95.0, 115.0);
            SetSkill(SkillName.MagicResist, 90.0, 110.0);
            SetSkill(SkillName.Tactics, 60.0, 80.0);
            SetSkill(SkillName.Wrestling, 55.0, 75.0);
            SetSkill(SkillName.Meditation, 90.0, 110.0);

            Fame = 10000;
            Karma = -10000;
            VirtualArmor = 45;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.MedScrolls, 2);

            PackItem(new ObsidianOre(Utility.RandomMinMax(4, 8)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new ShadowforgedIngot(Utility.RandomMinMax(1, 3)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new VoidDust());

            if (Utility.RandomDouble() < 0.08)
                PackItem(new VoidDust());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.35)
            {
                defender.SendMessage(0x480, "The void wraith's touch tears at your soul!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1109, 0, EffectLayer.Waist);
                defender.PlaySound(0x1FB);

                int damage = Utility.RandomMinMax(10, 16);
                AOS.Damage(defender, this, damage, 0, 0, 50, 0, 50);

                // Life drain
                Hits = Math.Min(HitsMax, Hits + damage);

                // Stat drain
                defender.AddStatMod(new StatMod(StatType.Str, "VoidDrain_Str", -10, TimeSpan.FromSeconds(15)));
                defender.AddStatMod(new StatMod(StatType.Dex, "VoidDrain_Dex", -10, TimeSpan.FromSeconds(15)));
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Void bolt
            if (Combatant != null && Utility.RandomDouble() < 0.03)
            {
                DoVoidBolt();
            }
        }

        private void DoVoidBolt()
        {
            if (Map == null || Combatant == null)
                return;

            Mobile target = Combatant as Mobile;
            if (target == null || !CanBeHarmful(target))
                return;

            Say("*The void wraith channels the void*");
            PlaySound(0x482);

            DoHarmful(target);

            MovingParticles(target, 0x36D4, 7, 0, false, true, 1109, 0, 9502, 1, 0, EffectLayer.Waist, 0x100);

            Timer.DelayCall(TimeSpan.FromSeconds(0.5), () =>
            {
                if (target != null && !target.Deleted && target.Alive)
                {
                    target.FixedParticles(0x374A, 10, 15, 5028, 1109, 0, EffectLayer.Waist);
                    target.PlaySound(0x482);

                    int damage = Utility.RandomMinMax(30, 45);
                    AOS.Damage(target, this, damage, 0, 0, 50, 0, 50);

                    // Heavy mana drain
                    int manaDrain = Utility.RandomMinMax(30, 50);
                    if (target.Mana >= manaDrain)
                    {
                        target.Mana -= manaDrain;
                        Mana = Math.Min(ManaMax, Mana + manaDrain);
                    }

                    target.SendMessage(0x480, "The void bolt sears through you!");
                }
            });
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool AlwaysMurderer => true;

        public VoidWraith(Serial serial) : base(serial)
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
