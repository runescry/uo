using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
    /// <summary>
    /// Griffin Lord - Boss of Skyreach
    ///
    /// Mechanics:
    /// - Phase 1 (100-66% HP): Dive attacks, wind gusts
    /// - Phase 2 (66-33% HP): Summons Wind Elementals, lightning strikes
    /// - Phase 3 (33-0% HP): Tornado attack, aerial supremacy
    ///
    /// Rewards: Skyreach resources, StormFeather, aerial equipment
    /// </summary>
    [CorpseName("the corpse of the Griffin Lord")]
    public class GriffinLord : BaseCreature
    {
        private int m_Phase = 1;
        private DateTime m_NextPhaseCheck;
        private DateTime m_NextDiveTime;
        private DateTime m_NextWindGustTime;
        private DateTime m_NextSummonTime;
        private DateTime m_NextLightningTime;
        private DateTime m_NextTornadoTime;

        [Constructable]
        public GriffinLord() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Griffin Lord";
            Title = "Master of the Skies";
            Body = 73;          // Griffin body
            Hue = 2498;         // Sky blue
            BaseSoundID = 0x2EE;

            SetStr(550, 650);
            SetDex(180, 220);
            SetInt(250, 350);

            SetHits(1350, 1650);
            SetDamage(30, 42);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 80, 90);

            SetSkill(SkillName.EvalInt, 100.0, 120.0);
            SetSkill(SkillName.Magery, 100.0, 120.0);
            SetSkill(SkillName.MagicResist, 110.0, 130.0);
            SetSkill(SkillName.Tactics, 110.0, 130.0);
            SetSkill(SkillName.Wrestling, 100.0, 120.0);

            Fame = 25000;
            Karma = -25000;
            VirtualArmor = 65;

            Tamable = false;

            m_NextPhaseCheck = DateTime.UtcNow;
            m_NextDiveTime = DateTime.UtcNow + TimeSpan.FromSeconds(12);
            m_NextWindGustTime = DateTime.UtcNow + TimeSpan.FromSeconds(8);
            m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(35);
            m_NextLightningTime = DateTime.UtcNow + TimeSpan.FromSeconds(15);
            m_NextTornadoTime = DateTime.UtcNow + TimeSpan.FromSeconds(30);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.Rich);

            // Use existing crystal resources as sky-themed alternatives
            PackItem(new CrystalOre(Utility.RandomMinMax(10, 20)));
            PackItem(new CrystallineIngot(Utility.RandomMinMax(5, 10)));
            PackItem(new StormCrystal(Utility.RandomMinMax(3, 6)));

            // REMOVED OLD REAGENT:
            // if (Utility.RandomDouble() < 0.15)
            //     PackItem(new EmberBloom(Utility.RandomMinMax(2, 4)));

            // 5% chance for equipment
            if (Utility.RandomDouble() < 0.05)
                PackItem(new WarHammer());
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

            if (DateTime.UtcNow >= m_NextWindGustTime)
            {
                DoWindGust();
                m_NextWindGustTime = DateTime.UtcNow + TimeSpan.FromSeconds(m_Phase == 3 ? 6 : 10);
            }

            if (DateTime.UtcNow >= m_NextDiveTime)
            {
                DoDiveAttack();
                m_NextDiveTime = DateTime.UtcNow + TimeSpan.FromSeconds(15);
            }

            if (m_Phase >= 2 && DateTime.UtcNow >= m_NextLightningTime)
            {
                DoLightningStrike();
                m_NextLightningTime = DateTime.UtcNow + TimeSpan.FromSeconds(12);
            }

            if (m_Phase >= 2 && DateTime.UtcNow >= m_NextSummonTime)
            {
                SummonWindElementals();
                m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(45);
            }

            if (m_Phase == 3 && DateTime.UtcNow >= m_NextTornadoTime)
            {
                DoTornado();
                m_NextTornadoTime = DateTime.UtcNow + TimeSpan.FromSeconds(25);
            }
        }

        private void CheckPhaseTransition()
        {
            double hpPercent = (double)Hits / HitsMax;

            if (m_Phase == 1 && hpPercent <= 0.66)
            {
                m_Phase = 2;
                Say("*The Griffin Lord's feathers crackle with lightning*");
                PublicOverheadMessage(Network.MessageType.Regular, 0x5D, false,
                    "The storms answer my call!");

                FixedParticles(0x376A, 9, 32, 5030, 2498, 0, EffectLayer.Waist);
                PlaySound(0x29);

                SummonWindElementals();
            }
            else if (m_Phase == 2 && hpPercent <= 0.33)
            {
                m_Phase = 3;
                Say("*The Griffin Lord screams as winds spiral around it*");
                PublicOverheadMessage(Network.MessageType.Regular, 0x5D, false,
                    "WITNESS THE FURY OF THE SKIES!");

                FixedParticles(0x375A, 10, 15, 5037, 2498, 0, EffectLayer.Head);
                PlaySound(0x2F);

                SetDamage(40, 55);
            }
        }

        private void DoWindGust()
        {
            if (Map == null)
                return;

            Say("*A powerful gust of wind erupts*");
            PlaySound(0x14D);

            foreach (Mobile m in GetMobilesInRange(6))
            {
                if (m == this || !CanBeHarmful(m))
                    continue;

                if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                {
                    DoHarmful(m);

                    m.FixedParticles(0x376A, 9, 32, 5030, 2498, 0, EffectLayer.Waist);

                    int damage = Utility.RandomMinMax(15, 25);
                    AOS.Damage(m, this, damage, 100, 0, 0, 0, 0);
                    m.SendMessage(0x5D, "The wind gust throws you off balance!");

                    // Knockback effect
                    m.AddStatMod(new StatMod(StatType.Dex, "WindGust", -20, TimeSpan.FromSeconds(5)));
                }
            }
        }

        private void DoDiveAttack()
        {
            if (Combatant == null)
                return;

            Mobile target = Combatant as Mobile;
            if (target == null || !CanBeHarmful(target))
                return;

            Say("*The Griffin Lord dives from above*");
            PublicOverheadMessage(Network.MessageType.Regular, 0x5D, false, "DIVE!");

            PlaySound(0x2EE);

            DoHarmful(target);

            // Teleport to target
            Point3D oldLoc = Location;
            Point3D newLoc = target.Location;

            Effects.SendLocationParticles(
                EffectItem.Create(oldLoc, Map, EffectItem.DefaultDuration),
                0x376A, 9, 32, 2498, 0, 5030, 0);

            Location = newLoc;

            Effects.SendLocationParticles(
                EffectItem.Create(newLoc, Map, EffectItem.DefaultDuration),
                0x376A, 9, 32, 2498, 0, 5030, 0);

            int damage = Utility.RandomMinMax(45, 65);
            AOS.Damage(target, this, damage, 100, 0, 0, 0, 0);
            target.SendMessage(0x5D, "The Griffin Lord's diving attack strikes you!");

            // Stun effect
            target.Freeze(TimeSpan.FromSeconds(2));
        }

        private void DoLightningStrike()
        {
            if (Combatant == null || Map == null)
                return;

            Mobile target = Combatant as Mobile;
            if (target == null)
                return;

            Say("*Lightning crackles from the Griffin Lord's wings*");

            DoHarmful(target);

            // Lightning effect
            target.BoltEffect(0);
            PlaySound(0x29);

            int damage = Utility.RandomMinMax(40, 60);
            AOS.Damage(target, this, damage, 0, 0, 0, 0, 100);
            target.SendMessage(0x5D, "Lightning strikes you!");

            // Chain lightning to nearby targets
            if (m_Phase == 3)
            {
                foreach (Mobile m in target.GetMobilesInRange(3))
                {
                    if (m != this && m != target && CanBeHarmful(m))
                    {
                        DoHarmful(m);
                        m.BoltEffect(0);
                        int chainDamage = Utility.RandomMinMax(20, 35);
                        AOS.Damage(m, this, chainDamage, 0, 0, 0, 0, 100);
                    }
                }
            }
        }

        private void DoTornado()
        {
            if (Map == null)
                return;

            Say("*A massive tornado forms around the Griffin Lord*");
            PublicOverheadMessage(Network.MessageType.Regular, 0x5D, false, "TORNADO!");

            PlaySound(0x14D);

            // Create visual tornado effect
            FixedParticles(0x376A, 20, 40, 5030, 2498, 0, EffectLayer.Waist);

            foreach (Mobile m in GetMobilesInRange(8))
            {
                if (m == this || !CanBeHarmful(m))
                    continue;

                if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                {
                    DoHarmful(m);

                    m.FixedParticles(0x376A, 9, 32, 5030, 2498, 0, EffectLayer.Head);

                    int damage = Utility.RandomMinMax(50, 75);
                    AOS.Damage(m, this, damage, 50, 0, 0, 0, 50);
                    m.SendMessage(0x5D, "The tornado tears at you!");

                    // Disorientation effect
                    m.AddStatMod(new StatMod(StatType.Dex, "TornadoDisorient", -30, TimeSpan.FromSeconds(8)));
                    m.AddStatMod(new StatMod(StatType.Int, "TornadoConfuse", -20, TimeSpan.FromSeconds(8)));
                }
            }
        }

        private void SummonWindElementals()
        {
            if (Map == null || Map == Map.Internal)
                return;

            int count = m_Phase == 3 ? 3 : 2;

            Say("*The Griffin Lord summons creatures of wind*");

            for (int i = 0; i < count; i++)
            {
                BaseCreature minion = new AirElemental();
                minion.Name = "a storm spirit";
                minion.Hue = 2498;

                Point3D loc = GetSpawnPosition(4);
                if (loc != Point3D.Zero)
                {
                    minion.MoveToWorld(loc, Map);
                    minion.Combatant = Combatant;

                    Effects.SendLocationParticles(
                        EffectItem.Create(loc, Map, EffectItem.DefaultDuration),
                        0x376A, 9, 32, 2498, 0, 5030, 0);
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
            Say("*The Griffin Lord falls from the sky*");
            PublicOverheadMessage(Network.MessageType.Regular, 0x5D, false,
                "The winds... grow still...");

            Effects.SendLocationParticles(
                EffectItem.Create(Location, Map, EffectItem.DefaultDuration),
                0x376A, 9, 32, 2498, 0, 5030, 0);
            Effects.PlaySound(Location, Map, 0x2F);

            base.OnDeath(c);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x5D, "The Griffin Lord's talons leave deep gashes!");

                // Bleed effect
                Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), 3, () =>
                {
                    if (defender != null && !defender.Deleted && defender.Alive)
                    {
                        AOS.Damage(defender, this, Utility.RandomMinMax(5, 10), 100, 0, 0, 0, 0);
                    }
                });
            }
        }

        public GriffinLord(Serial serial) : base(serial)
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
            m_NextDiveTime = DateTime.UtcNow + TimeSpan.FromSeconds(12);
            m_NextWindGustTime = DateTime.UtcNow + TimeSpan.FromSeconds(8);
            m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(35);
            m_NextLightningTime = DateTime.UtcNow + TimeSpan.FromSeconds(15);
            m_NextTornadoTime = DateTime.UtcNow + TimeSpan.FromSeconds(30);
        }
    }
}
