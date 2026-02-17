using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a nightmare hound corpse")]
    public class NightmareHound : BaseCreature
    {
        [Constructable]
        public NightmareHound() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a nightmare hound";
            Body = 98;
            Hue = 1109;
            BaseSoundID = 229;

            SetStr(200, 260);
            SetDex(130, 160);
            SetInt(80, 110);

            SetHits(220, 280);
            SetDamage(16, 24);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Cold, 25);
            SetDamageType(ResistanceType.Energy, 25);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 35, 45);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 45, 55);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.MagicResist, 75.0, 95.0);
            SetSkill(SkillName.Tactics, 85.0, 105.0);
            SetSkill(SkillName.Wrestling, 90.0, 110.0);

            Fame = 8000;
            Karma = -8000;
            VirtualArmor = 45;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 96.0;

            PackItem(new SulfurousAsh(5));
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);

            PackItem(new ObsidianOre(Utility.RandomMinMax(3, 6)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new ShadowforgedIngot());

            if (Utility.RandomDouble() < 0.06)
                PackItem(new VoidDust());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x480, "The nightmare hound's bite burns with shadow fire!");
                defender.FixedParticles(0x3709, 10, 15, 5052, 1109, 0, EffectLayer.Waist);
                defender.PlaySound(0x208);

                int damage = Utility.RandomMinMax(10, 16);
                AOS.Damage(defender, this, damage, 0, 0, 50, 0, 50);

                // Fear effect - brief freeze
                if (Utility.RandomDouble() < 0.25)
                {
                    defender.Freeze(TimeSpan.FromSeconds(1.5));
                    defender.SendMessage(0x480, "Fear grips your heart!");
                }
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Howl of terror
            if (Combatant != null && Utility.RandomDouble() < 0.02)
            {
                DoHowlOfTerror();
            }
        }

        private void DoHowlOfTerror()
        {
            if (Map == null)
                return;

            Say("*The nightmare hound lets loose a terrifying howl*");
            PlaySound(0x229);

            foreach (Mobile m in GetMobilesInRange(6))
            {
                if (m != this && CanBeHarmful(m))
                {
                    if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                    {
                        DoHarmful(m);
                        m.FixedParticles(0x374A, 10, 15, 5028, 1109, 0, EffectLayer.Head);

                        // Fear - stat debuff and brief stun
                        m.AddStatMod(new StatMod(StatType.Str, "NightmareFear_Str", -15, TimeSpan.FromSeconds(12)));
                        m.AddStatMod(new StatMod(StatType.Dex, "NightmareFear_Dex", -15, TimeSpan.FromSeconds(12)));
                        m.Freeze(TimeSpan.FromSeconds(1.0));
                        m.SendMessage(0x480, "The howl fills you with terror!");
                    }
                }
            }
        }

        public override int Meat => 3;
        public override int Hides => 8;
        public override FoodType FavoriteFood => FoodType.Meat;
        public override PackInstinct PackInstinct => PackInstinct.Canine;
        public override bool AlwaysMurderer => true;

        public NightmareHound(Serial serial) : base(serial)
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
