using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a storm giant corpse")]
    public class StormGiant : BaseCreature
    {
        [Constructable]
        public StormGiant() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a storm giant";
            Body = 76;
            Hue = 1281;
            BaseSoundID = 609;

            SetStr(450, 550);
            SetDex(90, 110);
            SetInt(150, 200);

            SetHits(600, 750);
            SetDamage(20, 30);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Energy, 60);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 35, 45);
            SetResistance(ResistanceType.Energy, 85, 95);

            SetSkill(SkillName.EvalInt, 80.0, 100.0);
            SetSkill(SkillName.Magery, 80.0, 100.0);
            SetSkill(SkillName.MagicResist, 90.0, 110.0);
            SetSkill(SkillName.Tactics, 95.0, 115.0);
            SetSkill(SkillName.Wrestling, 95.0, 115.0);

            Fame = 18000;
            Karma = -18000;
            VirtualArmor = 60;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.Rich);

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
                defender.SendMessage(0x480, "The storm giant's blow crackles with lightning!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1281, 0, EffectLayer.Waist);
                defender.PlaySound(0x1E1);

                int damage = Utility.RandomMinMax(15, 25);
                AOS.Damage(defender, this, damage, 0, 0, 0, 0, 100);

                // Stun
                defender.Freeze(TimeSpan.FromSeconds(1.5));
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Call lightning AoE
            if (Combatant != null && Utility.RandomDouble() < 0.02)
            {
                DoCallLightning();
            }
        }

        private void DoCallLightning()
        {
            if (Map == null)
                return;

            Say("*The storm giant calls lightning from the heavens*");
            PlaySound(0x29);

            foreach (Mobile m in GetMobilesInRange(8))
            {
                if (m != this && CanBeHarmful(m))
                {
                    if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                    {
                        DoHarmful(m);
                        m.FixedParticles(0x374A, 10, 15, 5028, 1281, 0, EffectLayer.Head);
                        m.PlaySound(0x1E1);

                        int damage = Utility.RandomMinMax(30, 50);
                        AOS.Damage(m, this, damage, 0, 0, 0, 0, 100);
                        m.SendMessage(0x480, "Lightning strikes you from above!");
                    }
                }
            }
        }

        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 5;

        public StormGiant(Serial serial) : base(serial)
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
