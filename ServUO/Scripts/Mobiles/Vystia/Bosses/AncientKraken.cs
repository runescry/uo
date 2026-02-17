using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
    /// <summary>
    /// Ancient Kraken - Boss of Underwater/Abyssal Depths
    ///
    /// Mechanics:
    /// - Phase 1 (100-66% HP): Tentacle attacks, ink cloud
    /// - Phase 2 (66-33% HP): Summons Sea Serpents, whirlpool
    /// - Phase 3 (33-0% HP): Crushing grasp, tidal wave
    ///
            // REMOVED OLD REAGENT: /// Rewards: Underwater resources, KrakenInk, abyssal equipment
    /// </summary>
    [CorpseName("the corpse of the Ancient Kraken")]
    public class AncientKraken : BaseCreature
    {
        private int m_Phase = 1;
        private DateTime m_NextPhaseCheck;
        private DateTime m_NextTentacleTime;
        private DateTime m_NextInkCloudTime;
        private DateTime m_NextSummonTime;
        private DateTime m_NextWhirlpoolTime;
        private DateTime m_NextCrushTime;

        [Constructable]
        public AncientKraken() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Ancient Kraken";
            Title = "Terror of the Deep";
            Body = 77;          // Kraken body
            Hue = 1266;         // Deep ocean blue
            BaseSoundID = 353;

            SetStr(650, 750);
            SetDex(100, 130);
            SetInt(300, 400);

            SetHits(1700, 2000);
            SetDamage(35, 48);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Cold, 40);
            SetDamageType(ResistanceType.Poison, 20);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 80, 90);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.EvalInt, 100.0, 120.0);
            SetSkill(SkillName.Magery, 100.0, 120.0);
            SetSkill(SkillName.MagicResist, 110.0, 130.0);
            SetSkill(SkillName.Tactics, 100.0, 120.0);
            SetSkill(SkillName.Wrestling, 120.0, 140.0);

            Fame = 28000;
            Karma = -28000;
            VirtualArmor = 75;

            Tamable = false;
            CanSwim = true;

            m_NextPhaseCheck = DateTime.UtcNow;
            m_NextTentacleTime = DateTime.UtcNow + TimeSpan.FromSeconds(8);
            m_NextInkCloudTime = DateTime.UtcNow + TimeSpan.FromSeconds(15);
            m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(35);
            m_NextWhirlpoolTime = DateTime.UtcNow + TimeSpan.FromSeconds(25);
            m_NextCrushTime = DateTime.UtcNow + TimeSpan.FromSeconds(20);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.Rich);

            // Use existing shadow/void resources for deep sea theme
            PackItem(new BogIronOre(Utility.RandomMinMax(10, 20)));
            PackItem(new ShadowforgedIngot(Utility.RandomMinMax(5, 10)));
            PackItem(new SwampLotus(Utility.RandomMinMax(3, 6)));

            // REMOVED OLD REAGENT:
            // if (Utility.RandomDouble() < 0.15)
            //     PackItem(new KrakenInk(Utility.RandomMinMax(2, 4)));

            // 5% chance for equipment
            if (Utility.RandomDouble() < 0.05)
                PackItem(new Spear());
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

            if (DateTime.UtcNow >= m_NextTentacleTime)
            {
                DoTentacleSlam();
                m_NextTentacleTime = DateTime.UtcNow + TimeSpan.FromSeconds(m_Phase == 3 ? 6 : 10);
            }

            if (DateTime.UtcNow >= m_NextInkCloudTime)
            {
                DoInkCloud();
                m_NextInkCloudTime = DateTime.UtcNow + TimeSpan.FromSeconds(18);
            }

            if (m_Phase >= 2 && DateTime.UtcNow >= m_NextWhirlpoolTime)
            {
                DoWhirlpool();
                m_NextWhirlpoolTime = DateTime.UtcNow + TimeSpan.FromSeconds(20);
            }

            if (m_Phase >= 2 && DateTime.UtcNow >= m_NextSummonTime)
            {
                SummonSeaCreatures();
                m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(45);
            }

            if (m_Phase == 3 && DateTime.UtcNow >= m_NextCrushTime)
            {
                DoCrushingGrasp();
                m_NextCrushTime = DateTime.UtcNow + TimeSpan.FromSeconds(18);
            }
        }

        private void CheckPhaseTransition()
        {
            double hpPercent = (double)Hits / HitsMax;

            if (m_Phase == 1 && hpPercent <= 0.66)
            {
                m_Phase = 2;
                Say("*The Ancient Kraken's eyes glow with fury*");
                PublicOverheadMessage(Network.MessageType.Regular, 0x25, false,
                    "The depths shall swallow you whole!");

                FixedParticles(0x376A, 9, 32, 5030, 1266, 0, EffectLayer.Waist);
                PlaySound(0x026);

                SummonSeaCreatures();
            }
            else if (m_Phase == 2 && hpPercent <= 0.33)
            {
                m_Phase = 3;
                Say("*The Ancient Kraken unleashes its true power*");
                PublicOverheadMessage(Network.MessageType.Regular, 0x25, false,
                    "DROWN IN THE ABYSS!");

                FixedParticles(0x375A, 10, 15, 5037, 1266, 0, EffectLayer.Head);
                PlaySound(0x026);

                SetDamage(45, 62);
                DoTidalWave();
            }
        }

        private void DoTentacleSlam()
        {
            if (Combatant == null)
                return;

            Mobile target = Combatant as Mobile;
            if (target == null || !CanBeHarmful(target))
                return;

            Say("*Massive tentacles strike from the depths*");

            DoHarmful(target);
            target.FixedParticles(0x374A, 10, 15, 5028, 1266, 0, EffectLayer.Waist);
            target.PlaySound(0x026);

            int damage = Utility.RandomMinMax(30, 45);
            AOS.Damage(target, this, damage, 100, 0, 0, 0, 0);
            target.SendMessage(0x25, "A massive tentacle slams into you!");

            // Chance to grapple
            if (Utility.RandomDouble() < 0.35)
            {
                target.Freeze(TimeSpan.FromSeconds(2));
                target.SendMessage(0x25, "A tentacle wraps around you!");
            }
        }

        private void DoInkCloud()
        {
            if (Map == null)
                return;

            Say("*The Kraken releases a cloud of blinding ink*");
            PlaySound(0x026);

            foreach (Mobile m in GetMobilesInRange(8))
            {
                if (m == this || !CanBeHarmful(m))
                    continue;

                if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                {
                    DoHarmful(m);

                    m.FixedParticles(0x374A, 10, 30, 5028, 1109, 0, EffectLayer.Head);
                    m.SendMessage(0x25, "You are blinded by the ink cloud!");

                    // Blind effect - reduce dex and hit chance
                    m.AddStatMod(new StatMod(StatType.Dex, "InkBlind", -30, TimeSpan.FromSeconds(10)));
                }
            }
        }

        private void DoWhirlpool()
        {
            if (Map == null || Combatant == null)
                return;

            Mobile target = Combatant as Mobile;
            if (target == null)
                return;

            Point3D loc = target.Location;

            Say("*A massive whirlpool forms*");
            PublicOverheadMessage(Network.MessageType.Regular, 0x25, false, "WHIRLPOOL!");

            Effects.SendLocationParticles(
                EffectItem.Create(loc, Map, EffectItem.DefaultDuration),
                0x37CC, 10, 30, 1266, 0, 5052, 0);
            PlaySound(0x026);

            // Pull and damage in area
            for (int i = 0; i < 5; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(i), () =>
                {
                    if (Map == null)
                        return;

                    foreach (Mobile m in Map.GetMobilesInRange(loc, 4))
                    {
                        if (m != this && CanBeHarmful(m))
                        {
                            DoHarmful(m);
                            int damage = Utility.RandomMinMax(12, 20);
                            AOS.Damage(m, this, damage, 0, 0, 50, 0, 50);
                            m.SendMessage(0x25, "The whirlpool pulls at you!");
                        }
                    }
                });
            }
        }

        private void DoCrushingGrasp()
        {
            if (Combatant == null)
                return;

            Mobile target = Combatant as Mobile;
            if (target == null || !CanBeHarmful(target))
                return;

            Say("*The Kraken's tentacles wrap around its prey*");
            PublicOverheadMessage(Network.MessageType.Regular, 0x25, false, "CRUSHING GRASP!");

            DoHarmful(target);

            target.Freeze(TimeSpan.FromSeconds(4));
            target.FixedParticles(0x374A, 10, 15, 5028, 1266, 0, EffectLayer.Waist);
            target.SendMessage(0x25, "The Kraken's tentacles crush you!");

            // Damage over time while grappled
            for (int i = 0; i < 4; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(i), () =>
                {
                    if (target != null && !target.Deleted && target.Alive)
                    {
                        int damage = Utility.RandomMinMax(20, 35);
                        AOS.Damage(target, this, damage, 100, 0, 0, 0, 0);
                    }
                });
            }
        }

        private void DoTidalWave()
        {
            if (Map == null)
                return;

            Say("*A massive tidal wave crashes outward*");

            PlaySound(0x026);

            foreach (Mobile m in GetMobilesInRange(10))
            {
                if (m == this || !CanBeHarmful(m))
                    continue;

                if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                {
                    DoHarmful(m);

                    m.FixedParticles(0x37CC, 10, 30, 5052, 1266, 0, EffectLayer.Head);

                    int damage = Utility.RandomMinMax(60, 90);
                    AOS.Damage(m, this, damage, 0, 0, 100, 0, 0);
                    m.SendMessage(0x25, "A massive tidal wave crashes into you!");

                    // Knockback and slow
                    m.Freeze(TimeSpan.FromSeconds(2));
                    m.AddStatMod(new StatMod(StatType.Dex, "TidalWaveSlow", -25, TimeSpan.FromSeconds(8)));
                }
            }
        }

        private void SummonSeaCreatures()
        {
            if (Map == null || Map == Map.Internal)
                return;

            int count = m_Phase == 3 ? 3 : 2;

            Say("*The Ancient Kraken summons creatures of the deep*");

            for (int i = 0; i < count; i++)
            {
                BaseCreature minion = new SeaSerpent();
                minion.Name = "an abyssal serpent";
                minion.Hue = 1266;

                Point3D loc = GetSpawnPosition(4);
                if (loc != Point3D.Zero)
                {
                    minion.MoveToWorld(loc, Map);
                    minion.Combatant = Combatant;

                    Effects.SendLocationParticles(
                        EffectItem.Create(loc, Map, EffectItem.DefaultDuration),
                        0x376A, 9, 32, 1266, 0, 5030, 0);
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
            Say("*The Ancient Kraken sinks back into the depths*");
            PublicOverheadMessage(Network.MessageType.Regular, 0x25, false,
                "The abyss... awaits...");

            Effects.SendLocationParticles(
                EffectItem.Create(Location, Map, EffectItem.DefaultDuration),
                0x376A, 9, 32, 1266, 0, 5030, 0);
            Effects.PlaySound(Location, Map, 0x026);

            base.OnDeath(c);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.ApplyPoison(this, Poison.Greater);
                defender.SendMessage(0x25, "The Kraken's venomous touch poisons you!");
            }
        }

        public AncientKraken(Serial serial) : base(serial)
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
            m_NextTentacleTime = DateTime.UtcNow + TimeSpan.FromSeconds(8);
            m_NextInkCloudTime = DateTime.UtcNow + TimeSpan.FromSeconds(15);
            m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(35);
            m_NextWhirlpoolTime = DateTime.UtcNow + TimeSpan.FromSeconds(25);
            m_NextCrushTime = DateTime.UtcNow + TimeSpan.FromSeconds(20);
        }
    }
}
