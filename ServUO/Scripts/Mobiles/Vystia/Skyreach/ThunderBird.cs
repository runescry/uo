using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a thunderbird corpse")]
    public class ThunderBird : BaseCreature
    {
        [Constructable]
        public ThunderBird() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a thunderbird";
            Body = 6;
            Hue = 1281;
            BaseSoundID = 0x2EE;

            SetStr(200, 260);
            SetDex(140, 170);
            SetInt(150, 200);

            SetHits(180, 240);
            SetDamage(14, 20);

            SetDamageType(ResistanceType.Physical, 30);
            SetDamageType(ResistanceType.Energy, 70);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 25, 35);
            SetResistance(ResistanceType.Cold, 45, 55);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 80, 90);

            SetSkill(SkillName.EvalInt, 75.0, 95.0);
            SetSkill(SkillName.Magery, 75.0, 95.0);
            SetSkill(SkillName.MagicResist, 70.0, 90.0);
            SetSkill(SkillName.Tactics, 70.0, 90.0);
            SetSkill(SkillName.Wrestling, 70.0, 90.0);

            Fame = 9000;
            Karma = -9000;
            VirtualArmor = 45;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.MedScrolls);

            PackItem(new CrystalOre(Utility.RandomMinMax(4, 8)));
            PackItem(new CrystallineIngot(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new StormCrystal(Utility.RandomMinMax(1, 2)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new StormCrystal());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x480, "Lightning strikes you from the thunderbird!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1281, 0, EffectLayer.Waist);
                defender.PlaySound(0x1E1);

                int damage = Utility.RandomMinMax(15, 25);
                AOS.Damage(defender, this, damage, 0, 0, 0, 0, 100);
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Lightning storm
            if (Combatant != null && Utility.RandomDouble() < 0.02)
            {
                DoLightningStorm();
            }
        }

        private void DoLightningStorm()
        {
            if (Map == null)
                return;

            Say("*The thunderbird calls down lightning*");
            PlaySound(0x29);

            foreach (Mobile m in GetMobilesInRange(6))
            {
                if (m != this && CanBeHarmful(m))
                {
                    if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                    {
                        DoHarmful(m);
                        m.FixedParticles(0x374A, 10, 15, 5028, 1281, 0, EffectLayer.Head);
                        m.PlaySound(0x1E1);

                        int damage = Utility.RandomMinMax(20, 35);
                        AOS.Damage(m, this, damage, 0, 0, 0, 0, 100);
                        m.SendMessage(0x480, "Lightning strikes you!");
                    }
                }
            }
        }

        public override int Feathers => 50;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 4;

        public ThunderBird(Serial serial) : base(serial)
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
