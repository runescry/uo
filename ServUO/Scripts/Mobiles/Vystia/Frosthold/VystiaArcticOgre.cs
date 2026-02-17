using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an arctic ogre corpse")]
    public class VystiaArcticOgre : BaseCreature
    {
        [Constructable]
        public VystiaArcticOgre() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an arctic ogre";
            Body = 1;
            Hue = 1152;
            BaseSoundID = 427;

            SetStr(350, 420);
            SetDex(70, 90);
            SetInt(60, 80);

            SetHits(380, 460);
            SetDamage(18, 28);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Cold, 40);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 15, 25);
            SetResistance(ResistanceType.Cold, 70, 80);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 70.0, 90.0);
            SetSkill(SkillName.Tactics, 90.0, 110.0);
            SetSkill(SkillName.Wrestling, 95.0, 115.0);

            Fame = 12000;
            Karma = -12000;
            VirtualArmor = 55;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.Rich);

            PackItem(new FrozenOre(Utility.RandomMinMax(6, 12)));
            PackItem(new FrostforgedIngot(Utility.RandomMinMax(3, 6)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new IceCrystal(Utility.RandomMinMax(1, 2)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new EternalIce());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x480, "The arctic ogre's freezing blow chills you to the bone!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1152, 0, EffectLayer.Waist);
                defender.PlaySound(0x1FB);

                int damage = Utility.RandomMinMax(12, 20);
                AOS.Damage(defender, this, damage, 0, 0, 100, 0, 0);

                defender.AddStatMod(new StatMod(StatType.Dex, "ArcticOgreSlow", -25, TimeSpan.FromSeconds(12)));
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant != null && Utility.RandomDouble() < 0.02)
            {
                DoGroundSlam();
            }
        }

        private void DoGroundSlam()
        {
            if (Map == null)
                return;

            Say("*The arctic ogre slams the frozen ground*");
            PlaySound(0x2F3);

            foreach (Mobile m in GetMobilesInRange(5))
            {
                if (m != this && CanBeHarmful(m))
                {
                    if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                    {
                        DoHarmful(m);
                        m.FixedParticles(0x36BD, 20, 10, 5044, 1152, 0, EffectLayer.Head);

                        int damage = Utility.RandomMinMax(25, 40);
                        AOS.Damage(m, this, damage, 50, 0, 50, 0, 0);

                        m.Freeze(TimeSpan.FromSeconds(2.0));
                        m.SendMessage(0x480, "The frozen shockwave knocks you down!");
                    }
                }
            }
        }

        public override bool CanRummageCorpses => true;
        public override int Meat => 6;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 4;

        public VystiaArcticOgre(Serial serial) : base(serial)
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
