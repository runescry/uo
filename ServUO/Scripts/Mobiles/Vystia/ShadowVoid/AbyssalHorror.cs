using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an abyssal horror corpse")]
    public class AbyssalHorror : BaseCreature
    {
        [Constructable]
        public AbyssalHorror() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an abyssal horror";
            Body = 0x50;
            Hue = 1109;
            BaseSoundID = 684;

            SetStr(400, 500);
            SetDex(100, 130);
            SetInt(200, 260);

            SetHits(450, 560);
            SetDamage(20, 30);

            SetDamageType(ResistanceType.Physical, 30);
            SetDamageType(ResistanceType.Cold, 35);
            SetDamageType(ResistanceType.Energy, 35);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 65, 75);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 65, 75);

            SetSkill(SkillName.EvalInt, 95.0, 115.0);
            SetSkill(SkillName.Magery, 95.0, 115.0);
            SetSkill(SkillName.MagicResist, 100.0, 120.0);
            SetSkill(SkillName.Tactics, 90.0, 110.0);
            SetSkill(SkillName.Wrestling, 90.0, 110.0);
            SetSkill(SkillName.Meditation, 90.0, 110.0);

            Fame = 18000;
            Karma = -18000;
            VirtualArmor = 60;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.MedScrolls, 2);

            PackItem(new ObsidianOre(Utility.RandomMinMax(8, 15)));
            PackItem(new ShadowforgedIngot(Utility.RandomMinMax(4, 8)));

            if (Utility.RandomDouble() < 0.20)
                PackItem(new VoidDust(Utility.RandomMinMax(1, 3)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new VoidDust(Utility.RandomMinMax(1, 2)));
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.35)
            {
                defender.SendMessage(0x480, "The abyssal horror's tentacles tear at your essence!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1109, 0, EffectLayer.Waist);
                defender.PlaySound(0x1FB);

                int damage = Utility.RandomMinMax(15, 25);
                AOS.Damage(defender, this, damage, 0, 0, 50, 0, 50);

                // Essence drain
                Hits = Math.Min(HitsMax, Hits + damage);

                // Heavy stat drain
                defender.AddStatMod(new StatMod(StatType.Str, "AbyssalDrain_Str", -15, TimeSpan.FromSeconds(15)));
                defender.AddStatMod(new StatMod(StatType.Dex, "AbyssalDrain_Dex", -15, TimeSpan.FromSeconds(15)));
                defender.AddStatMod(new StatMod(StatType.Int, "AbyssalDrain_Int", -15, TimeSpan.FromSeconds(15)));
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Void eruption
            if (Combatant != null && Utility.RandomDouble() < 0.02)
            {
                DoVoidEruption();
            }
        }

        private void DoVoidEruption()
        {
            if (Map == null)
                return;

            Say("*The abyssal horror opens a rift to the void*");
            PlaySound(0x211);

            FixedParticles(0x3709, 10, 30, 5052, 1109, 0, EffectLayer.CenterFeet);

            foreach (Mobile m in GetMobilesInRange(7))
            {
                if (m != this && CanBeHarmful(m))
                {
                    if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                    {
                        DoHarmful(m);
                        m.FixedParticles(0x3709, 10, 30, 5052, 1109, 0, EffectLayer.Waist);
                        m.PlaySound(0x211);

                        int damage = Utility.RandomMinMax(40, 60);
                        AOS.Damage(m, this, damage, 0, 0, 50, 0, 50);

                        // Heavy mana and life drain
                        int drain = Utility.RandomMinMax(40, 60);
                        if (m.Mana >= drain)
                        {
                            m.Mana -= drain;
                            Mana = Math.Min(ManaMax, Mana + drain);
                        }

                        Hits = Math.Min(HitsMax, Hits + damage / 3);

                        // Terrify - freeze and debuff
                        m.Freeze(TimeSpan.FromSeconds(2.0));
                        m.SendMessage(0x480, "Void energy tears at your very being!");
                    }
                }
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 5;

        public AbyssalHorror(Serial serial) : base(serial)
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
