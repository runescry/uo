using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
    /// <summary>
    /// Timeworn Lich - Boss of Shadow Void / Obsidian Wastes
    ///
    /// Mechanics:
    /// - Phase 1 (100-66% HP): Necromantic bolts, soul drain
    /// - Phase 2 (66-33% HP): Summons Spectres, void storms
    /// - Phase 3 (33-0% HP): Time stop, mass soul harvest, void collapse
    ///
    /// Rewards: Obsidian resources, VoidEssence, shadow equipment, Voidcaller (legendary)
    /// </summary>
    [CorpseName("the ancient remains of the Timeworn Lich")]
    public class TimewornLich : BaseCreature
    {
        private int m_Phase = 1;
        private DateTime m_NextPhaseCheck;
        private DateTime m_NextBoltTime;
        private DateTime m_NextDrainTime;
        private DateTime m_NextSummonTime;
        private DateTime m_NextVoidStormTime;
        private DateTime m_NextTimeStopTime;

        [Constructable]
        public TimewornLich() : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Timeworn Lich";
            Title = "the Eternal Shadow";
            Body = 24;          // Lich body
            Hue = 1109;         // Void black
            BaseSoundID = 0x482;

            SetStr(350, 450);
            SetDex(100, 130);
            SetInt(550, 650);

            SetHits(1500, 1800);
            SetDamage(28, 38);

            SetDamageType(ResistanceType.Physical, 10);
            SetDamageType(ResistanceType.Cold, 40);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 80, 90);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 70, 80);

            SetSkill(SkillName.EvalInt, 120.0, 140.0);
            SetSkill(SkillName.Magery, 120.0, 140.0);
            SetSkill(SkillName.Necromancy, 120.0, 140.0);
            SetSkill(SkillName.SpiritSpeak, 110.0, 130.0);
            SetSkill(SkillName.MagicResist, 120.0, 140.0);
            SetSkill(SkillName.Meditation, 100.0, 120.0);

            Fame = 32000;
            Karma = -32000;
            VirtualArmor = 70;

            Tamable = false;

            m_NextPhaseCheck = DateTime.UtcNow;
            m_NextBoltTime = DateTime.UtcNow + TimeSpan.FromSeconds(6);
            m_NextDrainTime = DateTime.UtcNow + TimeSpan.FromSeconds(12);
            m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(35);
            m_NextVoidStormTime = DateTime.UtcNow + TimeSpan.FromSeconds(20);
            m_NextTimeStopTime = DateTime.UtcNow + TimeSpan.FromSeconds(30);
        }

        public override void GenerateLoot()
        {
            AddLoot(VystiaLootPack.ObsidianRich);
            AddLoot(LootPack.FilthyRich, 2);

            PackItem(new ObsidianOre(Utility.RandomMinMax(15, 25)));
            PackItem(new VoidforgedIngot(Utility.RandomMinMax(5, 10)));
            PackItem(new Nightshade(Utility.RandomMinMax(3, 6)));
            PackItem(new VoidDust(Utility.RandomMinMax(3, 6)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new StormCrystal(Utility.RandomMinMax(1, 2)));

            // 2% chance for Voidcaller legendary weapon
            if (Utility.RandomDouble() < 0.02)
                PackItem(new Voidcaller());

            // 1% chance for Mirror of Truth legendary artifact
            if (Utility.RandomDouble() < 0.01)
                PackItem(new MirrorOfTruth());
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

            if (DateTime.UtcNow >= m_NextBoltTime)
            {
                DoNecroBolt();
                m_NextBoltTime = DateTime.UtcNow + TimeSpan.FromSeconds(m_Phase == 3 ? 4 : 7);
            }

            if (DateTime.UtcNow >= m_NextDrainTime)
            {
                DoSoulDrain();
                m_NextDrainTime = DateTime.UtcNow + TimeSpan.FromSeconds(15);
            }

            if (m_Phase >= 2 && DateTime.UtcNow >= m_NextVoidStormTime)
            {
                DoVoidStorm();
                m_NextVoidStormTime = DateTime.UtcNow + TimeSpan.FromSeconds(18);
            }

            if (m_Phase >= 2 && DateTime.UtcNow >= m_NextSummonTime)
            {
                SummonSpectres();
                m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(45);
            }

            if (m_Phase == 3 && DateTime.UtcNow >= m_NextTimeStopTime)
            {
                DoTimeStop();
                m_NextTimeStopTime = DateTime.UtcNow + TimeSpan.FromSeconds(25);
            }
        }

        private void CheckPhaseTransition()
        {
            double hpPercent = (double)Hits / HitsMax;

            if (m_Phase == 1 && hpPercent <= 0.66)
            {
                m_Phase = 2;
                Say("*The Lich's form shimmers between dimensions*");
                PublicOverheadMessage(Network.MessageType.Regular, 0x455, false,
                    "The void answers my call!");

                FixedParticles(0x376A, 9, 32, 5030, 1109, 0, EffectLayer.Waist);
                PlaySound(0x1FB);

                SummonSpectres();
            }
            else if (m_Phase == 2 && hpPercent <= 0.33)
            {
                m_Phase = 3;
                Say("*Reality tears asunder around the Lich*");
                PublicOverheadMessage(Network.MessageType.Regular, 0x455, false,
                    "TIME ITSELF BENDS TO MY WILL!");

                FixedParticles(0x375A, 10, 15, 5037, 1109, 0, EffectLayer.Head);
                PlaySound(0x1F4);

                SetDamage(38, 52);
                DoMassSoulHarvest();
            }
        }

        private void DoNecroBolt()
        {
            if (Combatant == null)
                return;

            Mobile target = Combatant as Mobile;
            if (target == null || !CanBeHarmful(target))
                return;

            Say("*Dark energy crackles*");

            DoHarmful(target);

            // Dark bolt effect
            Effects.SendMovingParticles(this, target, 0x36E4, 7, 0, false, true, 1109, 0, 9502, 1, 0, EffectLayer.Head, 0x100);
            PlaySound(0x1E5);

            Timer.DelayCall(TimeSpan.FromSeconds(0.3), () =>
            {
                if (target != null && !target.Deleted && target.Alive)
                {
                    int damage = Utility.RandomMinMax(35, 55);
                    AOS.Damage(target, this, damage, 0, 0, 50, 0, 50);
                    target.SendMessage(0x455, "Necromantic energy tears at your soul!");

                    target.FixedParticles(0x374A, 10, 15, 5028, 1109, 0, EffectLayer.Waist);
                }
            });
        }

        private void DoSoulDrain()
        {
            if (Combatant == null)
                return;

            Mobile target = Combatant as Mobile;
            if (target == null || !CanBeHarmful(target))
                return;

            Say("*The Lich drains your essence*");

            DoHarmful(target);

            // Soul drain visual
            Effects.SendMovingParticles(target, this, 0x36D4, 7, 0, false, true, 1109, 0, 9502, 1, 0, EffectLayer.Waist, 0x100);
            target.PlaySound(0x1FB);

            int damage = Utility.RandomMinMax(30, 45);
            AOS.Damage(target, this, damage, 0, 0, 50, 0, 50);

            // Heal the Lich
            Hits = Math.Min(Hits + damage, HitsMax);

            // Mana drain
            if (target is PlayerMobile pm)
            {
                int manaDrain = Utility.RandomMinMax(30, 50);
                pm.Mana = Math.Max(0, pm.Mana - manaDrain);
                pm.SendMessage(0x455, "Your life force and mana are drained!");
            }

            target.FixedParticles(0x374A, 10, 15, 5028, 1109, 0, EffectLayer.Head);
        }

        private void DoVoidStorm()
        {
            if (Map == null)
                return;

            Say("*A storm of void energy erupts*");
            PublicOverheadMessage(Network.MessageType.Regular, 0x455, false, "VOID STORM!");

            PlaySound(0x1F4);

            foreach (Mobile m in GetMobilesInRange(8))
            {
                if (m == this || !CanBeHarmful(m))
                    continue;

                if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                {
                    DoHarmful(m);

                    m.FixedParticles(0x376A, 9, 32, 5030, 1109, 0, EffectLayer.Head);

                    int damage = Utility.RandomMinMax(35, 55);
                    AOS.Damage(m, this, damage, 0, 0, 50, 0, 50);
                    m.SendMessage(0x455, "The void storm tears at your being!");

                    // Random stat drain
                    StatType stat = (StatType)Utility.Random(3);
                    m.AddStatMod(new StatMod(stat, "VoidStormDrain", -20, TimeSpan.FromSeconds(15)));
                }
            }
        }

        private void DoTimeStop()
        {
            if (Map == null)
                return;

            Say("*Time freezes around the Lich*");
            PublicOverheadMessage(Network.MessageType.Regular, 0x455, false, "TIME STOP!");

            PlaySound(0x1F4);
            FixedParticles(0x375A, 10, 15, 5037, 1109, 0, EffectLayer.Head);

            foreach (Mobile m in GetMobilesInRange(10))
            {
                if (m == this || !CanBeHarmful(m))
                    continue;

                if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                {
                    DoHarmful(m);

                    m.FixedParticles(0x376A, 9, 32, 5030, 1109, 0, EffectLayer.Waist);

                    // Freeze all targets
                    m.Freeze(TimeSpan.FromSeconds(4));
                    m.SendMessage(0x455, "Time itself stops around you!");

                    // Apply all stat penalties
                    m.AddStatMod(new StatMod(StatType.Str, "TimeStopStr", -25, TimeSpan.FromSeconds(10)));
                    m.AddStatMod(new StatMod(StatType.Dex, "TimeStopDex", -25, TimeSpan.FromSeconds(10)));
                    m.AddStatMod(new StatMod(StatType.Int, "TimeStopInt", -25, TimeSpan.FromSeconds(10)));
                }
            }

            // Use the freeze time to attack
            Timer.DelayCall(TimeSpan.FromSeconds(2), () =>
            {
                if (!Deleted && Alive && Combatant != null)
                {
                    Mobile target = Combatant as Mobile;
                    if (target != null && !target.Deleted && target.Alive)
                    {
                        int damage = Utility.RandomMinMax(50, 75);
                        AOS.Damage(target, this, damage, 0, 0, 50, 0, 50);
                        target.SendMessage(0x455, "The Lich strikes while you are frozen!");
                    }
                }
            });
        }

        private void DoMassSoulHarvest()
        {
            if (Map == null)
                return;

            Say("*The Lich harvests the souls of all nearby*");

            PlaySound(0x1FB);

            int totalHarvested = 0;

            foreach (Mobile m in GetMobilesInRange(10))
            {
                if (m == this || !CanBeHarmful(m))
                    continue;

                if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                {
                    DoHarmful(m);

                    Effects.SendMovingParticles(m, this, 0x36D4, 7, 0, false, true, 1109, 0, 9502, 1, 0, EffectLayer.Waist, 0x100);

                    int damage = Utility.RandomMinMax(60, 90);
                    AOS.Damage(m, this, damage, 0, 0, 50, 0, 50);
                    totalHarvested += damage;

                    m.SendMessage(0x455, "Your very soul is being harvested!");
                }
            }

            // Heal from harvested souls
            if (totalHarvested > 0)
            {
                Hits = Math.Min(Hits + totalHarvested / 2, HitsMax);
                FixedParticles(0x376A, 9, 32, 5030, 1109, 0, EffectLayer.Waist);
            }
        }

        private void SummonSpectres()
        {
            if (Map == null || Map == Map.Internal)
                return;

            int count = m_Phase == 3 ? 4 : 2;

            Say("*The Lich summons spirits from the void*");

            for (int i = 0; i < count; i++)
            {
                BaseCreature minion = new Spectre();
                minion.Name = "a void spectre";
                minion.Hue = 1109;

                Point3D loc = GetSpawnPosition(4);
                if (loc != Point3D.Zero)
                {
                    minion.MoveToWorld(loc, Map);
                    minion.Combatant = Combatant;

                    Effects.SendLocationParticles(
                        EffectItem.Create(loc, Map, EffectItem.DefaultDuration),
                        0x376A, 9, 32, 1109, 0, 5030, 0);
                    Effects.PlaySound(loc, Map, 0x1FB);
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
            Say("*The Lich's form unravels into the void*");
            PublicOverheadMessage(Network.MessageType.Regular, 0x455, false,
                "Death... is only... the beginning...");

            Effects.SendLocationParticles(
                EffectItem.Create(Location, Map, EffectItem.DefaultDuration),
                0x376A, 9, 32, 1109, 0, 5030, 0);
            Effects.PlaySound(Location, Map, 0x1F4);

            base.OnDeath(c);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.35)
            {
                defender.SendMessage(0x455, "The Lich's icy touch chills your soul!");

                // Cold damage and slow
                int coldDamage = Utility.RandomMinMax(10, 20);
                AOS.Damage(defender, this, coldDamage, 0, 0, 100, 0, 0);
                defender.AddStatMod(new StatMod(StatType.Dex, "LichTouch", -15, TimeSpan.FromSeconds(8)));
            }
        }

        public TimewornLich(Serial serial) : base(serial)
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
            m_NextBoltTime = DateTime.UtcNow + TimeSpan.FromSeconds(6);
            m_NextDrainTime = DateTime.UtcNow + TimeSpan.FromSeconds(12);
            m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(35);
            m_NextVoidStormTime = DateTime.UtcNow + TimeSpan.FromSeconds(20);
            m_NextTimeStopTime = DateTime.UtcNow + TimeSpan.FromSeconds(30);
        }
    }
}
