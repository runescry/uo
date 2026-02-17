using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an abyssal squid corpse")]
    public class AbyssalSquid : BaseCreature
    {
        [Constructable]
        public AbyssalSquid() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an abyssal squid";
            Body = 77;
            Hue = 1365;
            BaseSoundID = 353;

            SetStr(350, 420);
            SetDex(100, 130);
            SetInt(90, 120);

            SetHits(380, 460);
            SetDamage(18, 26);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Poison, 20);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 55, 65);
            SetResistance(ResistanceType.Energy, 35, 45);

            SetSkill(SkillName.MagicResist, 80.0, 100.0);
            SetSkill(SkillName.Tactics, 90.0, 110.0);
            SetSkill(SkillName.Wrestling, 95.0, 115.0);

            Fame = 12000;
            Karma = -12000;
            VirtualArmor = 52;

            CanSwim = true;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.Rich);

            PackItem(new ObsidianOre(Utility.RandomMinMax(6, 12)));
            PackItem(new VoidforgedIngot(Utility.RandomMinMax(3, 6)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new ObsidianOre(Utility.RandomMinMax(1, 3)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new VoidforgedIngot());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.35)
            {
                defender.SendMessage(0x480, "The abyssal squid's tentacles constrict you!");
                defender.FixedParticles(0x376A, 9, 32, 5007, 1365, 0, EffectLayer.Waist);
                defender.PlaySound(0x1E4);

                int damage = Utility.RandomMinMax(12, 20);
                AOS.Damage(defender, this, damage, 100, 0, 0, 0, 0);

                // Constrict and hold
                defender.Freeze(TimeSpan.FromSeconds(2.5));
                defender.SendMessage(0x480, "You are held by the squid's tentacles!");

                // Continued damage
                Timer.DelayCall(TimeSpan.FromSeconds(1.0), () =>
                {
                    if (defender != null && !defender.Deleted && defender.Alive)
                    {
                        defender.Damage(Utility.RandomMinMax(6, 10), this);
                    }
                });

                Timer.DelayCall(TimeSpan.FromSeconds(2.0), () =>
                {
                    if (defender != null && !defender.Deleted && defender.Alive)
                    {
                        defender.Damage(Utility.RandomMinMax(6, 10), this);
                    }
                });
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Ink cloud ability
            if (Combatant != null && Utility.RandomDouble() < 0.02)
            {
                DoInkCloud();
            }
        }

        private void DoInkCloud()
        {
            if (Map == null)
                return;

            Say("*The abyssal squid releases an ink cloud*");
            PlaySound(0x026);

            foreach (Mobile m in GetMobilesInRange(5))
            {
                if (m != this && CanBeHarmful(m))
                {
                    if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                    {
                        DoHarmful(m);
                        m.FixedParticles(0x3709, 10, 30, 5052, 1109, 0, EffectLayer.Head);
                        m.PlaySound(0x026);

                        int damage = Utility.RandomMinMax(15, 25);
                        AOS.Damage(m, this, damage, 0, 0, 0, 100, 0);

                        // Blind - reduce DEX (simulating difficulty seeing)
                        m.AddStatMod(new StatMod(StatType.Dex, "InkBlind", -30, TimeSpan.FromSeconds(12)));
                        m.SendMessage(0x480, "The ink blinds you!");
                    }
                }
            }
        }

        public override int Meat => 8;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 4;

        public AbyssalSquid(Serial serial) : base(serial)
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
