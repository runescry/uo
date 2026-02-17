using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
    /// <summary>
    /// Forge Master - Boss of Ironclad Foundry
    ///
    /// Mechanics:
    /// - Phase 1 (100-66% HP): Hammer strikes, molten metal splash
    /// - Phase 2 (66-33% HP): Summons Clockwork creatures, steam vents
    /// - Phase 3 (33-0% HP): Forge explosion, mechanical overdrive
    ///
    /// Rewards: Ironclad resources, GearCore, mechanical equipment, TheCogmaster (legendary)
    /// </summary>
    [CorpseName("the broken remains of the Forge Master")]
    public class ForgeMaster : BaseCreature
    {
        private int m_Phase = 1;
        private DateTime m_NextPhaseCheck;
        private DateTime m_NextHammerTime;
        private DateTime m_NextMoltenTime;
        private DateTime m_NextSummonTime;
        private DateTime m_NextSteamTime;
        private DateTime m_NextOverdriveTime;

        [Constructable]
        public ForgeMaster() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Forge Master";
            Title = "the Iron Tyrant";
            Body = 752;         // Golem body
            Hue = 2401;         // Steel gray
            BaseSoundID = 541;

            SetStr(700, 800);
            SetDex(80, 100);
            SetInt(200, 250);

            SetHits(1800, 2100);
            SetDamage(40, 52);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Fire, 30);
            SetDamageType(ResistanceType.Energy, 10);

            SetResistance(ResistanceType.Physical, 75, 85);
            SetResistance(ResistanceType.Fire, 60, 70);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 90, 100);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.MagicResist, 110.0, 130.0);
            SetSkill(SkillName.Tactics, 120.0, 140.0);
            SetSkill(SkillName.Wrestling, 120.0, 140.0);
            SetSkill(SkillName.Anatomy, 90.0, 110.0);

            Fame = 30000;
            Karma = -30000;
            VirtualArmor = 80;

            Tamable = false;

            m_NextPhaseCheck = DateTime.UtcNow;
            m_NextHammerTime = DateTime.UtcNow + TimeSpan.FromSeconds(8);
            m_NextMoltenTime = DateTime.UtcNow + TimeSpan.FromSeconds(12);
            m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(35);
            m_NextSteamTime = DateTime.UtcNow + TimeSpan.FromSeconds(18);
            m_NextOverdriveTime = DateTime.UtcNow + TimeSpan.FromSeconds(25);
        }

        public override void GenerateLoot()
        {
            AddLoot(VystiaLootPack.IroncladRich);
            AddLoot(LootPack.FilthyRich, 2);

            PackItem(new SteamworkOre(Utility.RandomMinMax(15, 25)));
            PackItem(new ClockworkIngot(Utility.RandomMinMax(5, 10)));
            PackItem(new ClockworkGear(Utility.RandomMinMax(5, 10)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new SteamCore(Utility.RandomMinMax(1, 2)));

            // 2% chance for TheCogmaster legendary weapon
            if (Utility.RandomDouble() < 0.02)
                PackItem(new TheCogmaster());
        }

        public override bool Unprovokable => true;
        public override bool BardImmune => true;
        public override bool AlwaysMurderer => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool BleedImmune => true;
        public override int TreasureMapLevel => 5;

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant == null || Deleted || !Alive)
                return;

            if (DateTime.UtcNow >= m_NextPhaseCheck)
            {
                m_NextPhaseCheck = DateTime.UtcNow + TimeSpan.FromSeconds(2);
                CheckPhaseTransition();
            }

            if (DateTime.UtcNow >= m_NextHammerTime)
            {
                DoHammerStrike();
                m_NextHammerTime = DateTime.UtcNow + TimeSpan.FromSeconds(m_Phase == 3 ? 5 : 8);
            }

            if (DateTime.UtcNow >= m_NextMoltenTime)
            {
                DoMoltenSplash();
                m_NextMoltenTime = DateTime.UtcNow + TimeSpan.FromSeconds(15);
            }

            if (m_Phase >= 2 && DateTime.UtcNow >= m_NextSteamTime)
            {
                DoSteamVent();
                m_NextSteamTime = DateTime.UtcNow + TimeSpan.FromSeconds(12);
            }

            if (m_Phase >= 2 && DateTime.UtcNow >= m_NextSummonTime)
            {
                SummonClockworks();
                m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(45);
            }

            if (m_Phase == 3 && DateTime.UtcNow >= m_NextOverdriveTime)
            {
                DoMechanicalOverdrive();
                m_NextOverdriveTime = DateTime.UtcNow + TimeSpan.FromSeconds(20);
            }
        }

        private void CheckPhaseTransition()
        {
            double hpPercent = (double)Hits / HitsMax;

            if (m_Phase == 1 && hpPercent <= 0.66)
            {
                m_Phase = 2;
                Say("*Steam hisses from the Forge Master's joints*");
                PublicOverheadMessage(Network.MessageType.Regular, 0x44E, false,
                    "ACTIVATING SECONDARY SYSTEMS!");

                FixedParticles(0x376A, 9, 32, 5030, 2401, 0, EffectLayer.Waist);
                PlaySound(0x2F3);

                SummonClockworks();
            }
            else if (m_Phase == 2 && hpPercent <= 0.33)
            {
                m_Phase = 3;
                Say("*The Forge Master's core glows with unstable energy*");
                PublicOverheadMessage(Network.MessageType.Regular, 0x44E, false,
                    "MAXIMUM POWER! FORGE PROTOCOL ENGAGED!");

                FixedParticles(0x3709, 10, 30, 5052, 2401, 0, EffectLayer.Head);
                PlaySound(0x349);

                SetDamage(52, 68);
                DoForgeExplosion();
            }
        }

        private void DoHammerStrike()
        {
            if (Combatant == null)
                return;

            Mobile target = Combatant as Mobile;
            if (target == null || !CanBeHarmful(target))
                return;

            Say("*The Forge Master brings down its mighty hammer*");

            DoHarmful(target);
            target.FixedParticles(0x36BD, 20, 10, 5044, 2401, 0, EffectLayer.Head);
            target.PlaySound(0x2F3);

            int damage = Utility.RandomMinMax(40, 60);
            AOS.Damage(target, this, damage, 100, 0, 0, 0, 0);
            target.SendMessage(0x44E, "The massive hammer crushes you!");

            // Armor reduction effect
            target.AddStatMod(new StatMod(StatType.Dex, "HammerStrike", -15, TimeSpan.FromSeconds(10)));

            // Ground shake - affects nearby
            foreach (Mobile m in GetMobilesInRange(3))
            {
                if (m != this && m != target && CanBeHarmful(m))
                {
                    DoHarmful(m);
                    AOS.Damage(m, this, Utility.RandomMinMax(15, 25), 100, 0, 0, 0, 0);
                    m.SendMessage(0x44E, "The ground shakes from the impact!");
                }
            }
        }

        private void DoMoltenSplash()
        {
            if (Map == null || Combatant == null)
                return;

            Mobile target = Combatant as Mobile;
            if (target == null)
                return;

            Point3D loc = target.Location;

            Say("*Molten metal sprays from the Forge Master*");

            Effects.SendLocationParticles(
                EffectItem.Create(loc, Map, EffectItem.DefaultDuration),
                0x3709, 10, 30, 1358, 0, 5052, 0);
            Effects.PlaySound(loc, Map, 0x208);

            foreach (Mobile m in Map.GetMobilesInRange(loc, 3))
            {
                if (m != this && CanBeHarmful(m))
                {
                    DoHarmful(m);
                    int damage = Utility.RandomMinMax(25, 40);
                    AOS.Damage(m, this, damage, 0, 100, 0, 0, 0);
                    m.SendMessage(0x44E, "Molten metal burns your flesh!");

                    // Burn DoT
                    Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), 3, () =>
                    {
                        if (m != null && !m.Deleted && m.Alive)
                        {
                            AOS.Damage(m, this, Utility.RandomMinMax(8, 15), 0, 100, 0, 0, 0);
                        }
                    });
                }
            }
        }

        private void DoSteamVent()
        {
            if (Map == null)
                return;

            Say("*Superheated steam erupts from vents*");
            PlaySound(0x108);

            foreach (Mobile m in GetMobilesInRange(6))
            {
                if (m == this || !CanBeHarmful(m))
                    continue;

                if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                {
                    DoHarmful(m);

                    m.FixedParticles(0x36B0, 10, 25, 5052, 2401, 0, EffectLayer.Head);

                    int damage = Utility.RandomMinMax(20, 35);
                    AOS.Damage(m, this, damage, 0, 100, 0, 0, 0);
                    m.SendMessage(0x44E, "Scalding steam burns you!");

                    // Blinding steam effect
                    m.AddStatMod(new StatMod(StatType.Dex, "SteamBlind", -20, TimeSpan.FromSeconds(6)));
                }
            }
        }

        private void DoMechanicalOverdrive()
        {
            if (Map == null)
                return;

            Say("*The Forge Master enters mechanical overdrive*");
            PublicOverheadMessage(Network.MessageType.Regular, 0x44E, false, "OVERDRIVE!");

            PlaySound(0x2F3);
            FixedParticles(0x3709, 10, 30, 5052, 2401, 0, EffectLayer.Waist);

            // Temporarily boost stats
            SetStr(Str + 100);
            SetDex(Dex + 50);

            Timer.DelayCall(TimeSpan.FromSeconds(15), () =>
            {
                if (!Deleted && Alive)
                {
                    SetStr(Str - 100);
                    SetDex(Dex - 50);
                    Say("*Systems cooling down*");
                }
            });

            // Rapid attacks on all nearby targets
            foreach (Mobile m in GetMobilesInRange(8))
            {
                if (m == this || !CanBeHarmful(m))
                    continue;

                if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                {
                    DoHarmful(m);

                    m.FixedParticles(0x36BD, 20, 10, 5044, 2401, 0, EffectLayer.Head);

                    int damage = Utility.RandomMinMax(45, 65);
                    AOS.Damage(m, this, damage, 80, 20, 0, 0, 0);
                    m.SendMessage(0x44E, "The Forge Master's attacks become a blur!");
                }
            }
        }

        private void DoForgeExplosion()
        {
            if (Map == null)
                return;

            Say("*The forge core releases a massive explosion*");

            PlaySound(0x349);

            foreach (Mobile m in GetMobilesInRange(10))
            {
                if (m == this || !CanBeHarmful(m))
                    continue;

                if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                {
                    DoHarmful(m);

                    m.FixedParticles(0x3709, 10, 30, 5052, 1358, 0, EffectLayer.Head);

                    int damage = Utility.RandomMinMax(70, 100);
                    AOS.Damage(m, this, damage, 50, 50, 0, 0, 0);
                    m.SendMessage(0x44E, "A massive explosion engulfs you!");

                    m.Freeze(TimeSpan.FromSeconds(2));
                }
            }
        }

        private void SummonClockworks()
        {
            if (Map == null || Map == Map.Internal)
                return;

            int count = m_Phase == 3 ? 3 : 2;

            Say("*The Forge Master activates its clockwork minions*");

            for (int i = 0; i < count; i++)
            {
                BaseCreature minion = new Golem();
                minion.Name = "a clockwork sentinel";
                minion.Hue = 2401;

                Point3D loc = GetSpawnPosition(4);
                if (loc != Point3D.Zero)
                {
                    minion.MoveToWorld(loc, Map);
                    minion.Combatant = Combatant;

                    Effects.SendLocationParticles(
                        EffectItem.Create(loc, Map, EffectItem.DefaultDuration),
                        0x376A, 9, 32, 2401, 0, 5030, 0);
                    Effects.PlaySound(loc, Map, 0x2F3);
                }
                else
                {
                    minion.Delete();
                }
            }
        }

        private new Point3D GetSpawnPosition(int range)
        {
            for (int i = 0; i < 10; i++)
            {
                int x = X + Utility.RandomMinMax(-range, range);
                int y = Y + Utility.RandomMinMax(-range, range);
                int z = Map.GetAverageZ(x, y);

                Point3D p = new Point3D(x, y, z);

                if (Map.CanSpawnMobile(p))
                    return p;
            }

            return Point3D.Zero;
        }

        public override void OnDeath(Container c)
        {
            Say("*The Forge Master's systems fail*");
            PublicOverheadMessage(Network.MessageType.Regular, 0x44E, false,
                "SYSTEM... FAILURE...");

            Effects.SendLocationParticles(
                EffectItem.Create(Location, Map, EffectItem.DefaultDuration),
                0x3709, 10, 30, 2401, 0, 5052, 0);
            Effects.PlaySound(Location, Map, 0x349);

            base.OnDeath(c);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x44E, "The impact jars your bones!");

                // Stun chance
                if (Utility.RandomDouble() < 0.30)
                {
                    defender.Freeze(TimeSpan.FromSeconds(1));
                }
            }
        }

        public ForgeMaster(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
            writer.Write(m_Phase);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 1)
                m_Phase = reader.ReadInt();

            m_NextPhaseCheck = DateTime.UtcNow;
            m_NextHammerTime = DateTime.UtcNow + TimeSpan.FromSeconds(8);
            m_NextMoltenTime = DateTime.UtcNow + TimeSpan.FromSeconds(12);
            m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(35);
            m_NextSteamTime = DateTime.UtcNow + TimeSpan.FromSeconds(18);
            m_NextOverdriveTime = DateTime.UtcNow + TimeSpan.FromSeconds(25);
        }
    }
}
