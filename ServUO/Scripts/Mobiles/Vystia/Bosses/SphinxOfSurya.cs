using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
    /// <summary>
    /// Sphinx of Surya - Boss of the Desert (Whispering Sands)
    ///
    /// Mechanics:
    /// - Phase 1 (100-66% HP): Riddles that debuff players, sand storms
    /// - Phase 2 (66-33% HP): Solar beam attacks, summons Sand Elementals
    /// - Phase 3 (33-0% HP): Time manipulation (slow), blinding light
    ///
    /// Rewards: Desert resources, TimeDust, rare artifacts
    /// </summary>
    [CorpseName("the corpse of the Sphinx of Surya")]
    public class SphinxOfSurya : BaseCreature
    {
        private int m_Phase = 1;
        private DateTime m_NextPhaseCheck;
        private DateTime m_NextSandstormTime;
        private DateTime m_NextSolarBeamTime;
        private DateTime m_NextSummonTime;
        private DateTime m_NextRiddleTime;

        [Constructable]
        public SphinxOfSurya() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Sphinx of Surya";
            Title = "Guardian of the Sands";
            Body = 788;         // Sphinx body
            Hue = 2305;         // Desert gold
            BaseSoundID = 0x289;

            SetStr(550, 650);
            SetDex(100, 120);
            SetInt(300, 400);

            SetHits(1400, 1700);
            SetDamage(30, 40);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Fire, 30);
            SetDamageType(ResistanceType.Energy, 30);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 70, 80);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 70, 80);

            SetSkill(SkillName.EvalInt, 100.0, 120.0);
            SetSkill(SkillName.Magery, 110.0, 130.0);
            SetSkill(SkillName.MagicResist, 110.0, 130.0);
            SetSkill(SkillName.Meditation, 80.0, 100.0);
            SetSkill(SkillName.Tactics, 90.0, 110.0);
            SetSkill(SkillName.Wrestling, 90.0, 110.0);

            Fame = 26000;
            Karma = -26000;
            VirtualArmor = 70;

            Tamable = false;

            m_NextPhaseCheck = DateTime.UtcNow;
            m_NextSandstormTime = DateTime.UtcNow + TimeSpan.FromSeconds(20);
            m_NextSolarBeamTime = DateTime.UtcNow + TimeSpan.FromSeconds(15);
            m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(35);
            m_NextRiddleTime = DateTime.UtcNow + TimeSpan.FromSeconds(25);
        }

        public override void GenerateLoot()
        {
            AddLoot(VystiaLootPack.DesertRich);
            AddLoot(LootPack.FilthyRich, 2);

            PackItem(new SandstoneOre(Utility.RandomMinMax(10, 20)));
            PackItem(new SunforgedIngot(Utility.RandomMinMax(5, 10)));
            // REMOVED OLD REAGENT: PackItem(new DesertRose(Utility.RandomMinMax(3, 6)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new TimeDust(Utility.RandomMinMax(2, 4)));

            // 5% chance for Sunforged equipment
            if (Utility.RandomDouble() < 0.05)
                PackItem(new SunBlade());
        }

        public override bool Unprovokable => true;
        public override bool BardImmune => true;
        public override bool AlwaysMurderer => true;
        public override Poison PoisonImmune => Poison.Lethal;
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

            if (DateTime.UtcNow >= m_NextSandstormTime)
            {
                DoSandstorm();
                m_NextSandstormTime = DateTime.UtcNow + TimeSpan.FromSeconds(m_Phase == 3 ? 15 : 20);
            }

            if (DateTime.UtcNow >= m_NextRiddleTime && m_Phase >= 1)
            {
                DoRiddle();
                m_NextRiddleTime = DateTime.UtcNow + TimeSpan.FromSeconds(30);
            }

            if (m_Phase >= 2 && DateTime.UtcNow >= m_NextSolarBeamTime)
            {
                DoSolarBeam();
                m_NextSolarBeamTime = DateTime.UtcNow + TimeSpan.FromSeconds(12);
            }

            if (m_Phase >= 2 && DateTime.UtcNow >= m_NextSummonTime)
            {
                SummonSandElementals();
                m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(45);
            }
        }

        private void CheckPhaseTransition()
        {
            double hpPercent = (double)Hits / HitsMax;

            if (m_Phase == 1 && hpPercent <= 0.66)
            {
                m_Phase = 2;
                Say("*The Sphinx's eyes begin to glow with solar energy*");
                PublicOverheadMessage(Network.MessageType.Regular, 0x35, false,
                    "The sun's wrath shall be your end!");

                FixedParticles(0x376A, 9, 32, 5030, 2305, 0, EffectLayer.Waist);
                PlaySound(0x1E3);

                SetResistance(ResistanceType.Fire, 80, 90);
            }
            else if (m_Phase == 2 && hpPercent <= 0.33)
            {
                m_Phase = 3;
                Say("*Time itself bends to the Sphinx's will*");
                PublicOverheadMessage(Network.MessageType.Regular, 0x35, false,
                    "GAZE UPON ETERNITY!");

                FixedParticles(0x375A, 10, 15, 5037, 2305, 0, EffectLayer.Head);
                PlaySound(0x1F4);

                SetDamage(40, 55);
                DoTimeWarp();
            }
        }

        private void DoRiddle()
        {
            if (Combatant == null)
                return;

            string[] riddles = new string[]
            {
                "What walks on four legs, then two, then three?",
                "I am not alive, yet I grow. What am I?",
                "The more you take, the more you leave behind.",
                "What has keys but no locks?",
                "I speak without a mouth and hear without ears."
            };

            string riddle = riddles[Utility.Random(riddles.Length)];
            PublicOverheadMessage(Network.MessageType.Regular, 0x35, false, riddle);

            // Apply debuff for "failing" the riddle
            foreach (Mobile m in GetMobilesInRange(8))
            {
                if (m is PlayerMobile pm && CanBeHarmful(pm))
                {
                    pm.SendMessage(0x35, "The riddle confuses your mind!");
                    pm.AddStatMod(new StatMod(StatType.Int, "SphinxRiddle", -15, TimeSpan.FromSeconds(15)));
                }
            }
        }

        private void DoSandstorm()
        {
            if (Map == null)
                return;

            Say("*A fierce sandstorm engulfs the area*");
            PlaySound(0x14D);

            foreach (Mobile m in GetMobilesInRange(8))
            {
                if (m == this || !CanBeHarmful(m))
                    continue;

                if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                {
                    DoHarmful(m);

                    m.FixedParticles(0x36B0, 10, 25, 5052, 2305, 0, EffectLayer.Head);

                    int damage = Utility.RandomMinMax(20, 35);
                    AOS.Damage(m, this, damage, 100, 0, 0, 0, 0);
                    m.SendMessage(0x35, "The sandstorm tears at your flesh!");

                    // Blind effect
                    m.AddStatMod(new StatMod(StatType.Dex, "SandstormBlind", -10, TimeSpan.FromSeconds(5)));
                }
            }
        }

        private void DoSolarBeam()
        {
            if (Combatant == null || Map == null)
                return;

            Mobile target = Combatant as Mobile;
            if (target == null)
                return;

            Say("*The Sphinx channels the power of the sun*");
            PublicOverheadMessage(Network.MessageType.Regular, 0x35, false, "SOLAR BEAM!");

            // Beam effect
            Effects.SendMovingParticles(this, target, 0x36D4, 7, 0, false, true, 2305, 0, 9502, 1, 0, EffectLayer.Head, 0x100);
            PlaySound(0x211);

            Timer.DelayCall(TimeSpan.FromSeconds(0.5), () =>
            {
                if (target != null && !target.Deleted && target.Alive)
                {
                    DoHarmful(target);
                    int damage = Utility.RandomMinMax(50, 70);
                    AOS.Damage(target, this, damage, 0, 50, 0, 0, 50);
                    target.SendMessage(0x35, "The solar beam sears your flesh!");

                    target.FixedParticles(0x36BD, 20, 10, 5044, 2305, 0, EffectLayer.Head);
                }
            });
        }

        private void DoTimeWarp()
        {
            Say("*Time slows around the Sphinx*");

            foreach (Mobile m in GetMobilesInRange(10))
            {
                if (m is PlayerMobile pm && CanBeHarmful(pm))
                {
                    pm.SendMessage(0x35, "Time itself slows around you!");
                    pm.AddStatMod(new StatMod(StatType.Dex, "TimeWarp", -30, TimeSpan.FromSeconds(10)));
                    pm.FixedParticles(0x375A, 10, 15, 5037, 2305, 0, EffectLayer.Waist);
                }
            }
        }

        private void SummonSandElementals()
        {
            if (Map == null || Map == Map.Internal)
                return;

            int count = m_Phase == 3 ? 3 : 2;

            Say("*The Sphinx calls forth guardians of sand*");

            for (int i = 0; i < count; i++)
            {
                BaseCreature minion = new EarthElemental();
                minion.Name = "a sand elemental";
                minion.Hue = 2305;
                minion.SetDamageType(ResistanceType.Physical, 80);
                minion.SetDamageType(ResistanceType.Fire, 20);

                Point3D loc = GetSpawnPosition(4);
                if (loc != Point3D.Zero)
                {
                    minion.MoveToWorld(loc, Map);
                    minion.Combatant = Combatant;

                    Effects.SendLocationParticles(
                        EffectItem.Create(loc, Map, EffectItem.DefaultDuration),
                        0x376A, 9, 32, 2305, 0, 5030, 0);
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
            Say("*The Sphinx crumbles to dust*");
            PublicOverheadMessage(Network.MessageType.Regular, 0x35, false,
                "The sands... reclaim... all...");

            Effects.SendLocationParticles(
                EffectItem.Create(Location, Map, EffectItem.DefaultDuration),
                0x376A, 9, 32, 2305, 0, 5030, 0);
            Effects.PlaySound(Location, Map, 0x1F4);

            base.OnDeath(c);
        }

        public SphinxOfSurya(Serial serial) : base(serial)
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
            m_NextSandstormTime = DateTime.UtcNow + TimeSpan.FromSeconds(20);
            m_NextSolarBeamTime = DateTime.UtcNow + TimeSpan.FromSeconds(15);
            m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(35);
            m_NextRiddleTime = DateTime.UtcNow + TimeSpan.FromSeconds(25);
        }
    }
}
