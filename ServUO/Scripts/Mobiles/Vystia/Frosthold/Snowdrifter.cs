using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a snowdrifter corpse")]
    public class Snowdrifter : BaseCreature
    {
        [Constructable]
        public Snowdrifter() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a snowdrifter";
            Body = 13;
            Hue = 1150;
            BaseSoundID = 655;

            SetStr(150, 180);
            SetDex(120, 150);
            SetInt(100, 130);

            SetHits(120, 150);
            SetDamage(10, 16);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Cold, 80);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 5, 15);
            SetResistance(ResistanceType.Cold, 70, 80);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.EvalInt, 60.0, 80.0);
            SetSkill(SkillName.Magery, 60.0, 80.0);
            SetSkill(SkillName.MagicResist, 60.0, 80.0);
            SetSkill(SkillName.Tactics, 60.0, 80.0);
            SetSkill(SkillName.Wrestling, 60.0, 80.0);

            Fame = 5500;
            Karma = -5500;
            VirtualArmor = 35;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.MedScrolls);

            PackItem(new FrozenOre(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new FrostforgedIngot(Utility.RandomMinMax(1, 2)));

            if (Utility.RandomDouble() < 0.05)
                PackItem(new IceCrystal());
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant != null && Utility.RandomDouble() < 0.02)
            {
                DoBlizzardAttack();
            }
        }

        private void DoBlizzardAttack()
        {
            if (Map == null)
                return;

            Say("*The snowdrifter summons a localized blizzard*");
            PlaySound(0x10B);

            foreach (Mobile m in GetMobilesInRange(5))
            {
                if (m != this && CanBeHarmful(m))
                {
                    if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                    {
                        DoHarmful(m);
                        m.FixedParticles(0x374A, 10, 15, 5028, 1150, 0, EffectLayer.Waist);

                        int damage = Utility.RandomMinMax(10, 20);
                        AOS.Damage(m, this, damage, 0, 0, 100, 0, 0);
                        m.SendMessage(0x480, "The blizzard tears at you!");

                        // Slow effect
                        m.AddStatMod(new StatMod(StatType.Dex, "BlizzardSlow", -10, TimeSpan.FromSeconds(5)));
                    }
                }
            }
        }

        public override bool AlwaysMurderer => true;

        public Snowdrifter(Serial serial) : base(serial)
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
