using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
    /// <summary>
    /// Crystal Drake Alpha - Boss of Crystal Barrens
    ///
    /// Mechanics:
    /// - Phase 1 (100-66% HP): Crystal shard attacks, reflective shield
    /// - Phase 2 (66-33% HP): Summons Crystal Elementals, prismatic beams
    /// - Phase 3 (33-0% HP): Crystal explosion, shatter AoE, mana drain
    ///
    /// Rewards: Crystal Barrens resources, PrismCore, crystal equipment
    /// </summary>
    [CorpseName("the shattered remains of the Crystal Drake Alpha")]
    public class CrystalDrakeAlpha : BaseCreature
    {
        private int m_Phase = 1;
        private DateTime m_NextPhaseCheck;
        private DateTime m_NextShardTime;
        private DateTime m_NextPrismaticTime;
        private DateTime m_NextSummonTime;
        private DateTime m_NextShatterTime;
        private bool m_ShieldActive;

        [Constructable]
        public CrystalDrakeAlpha() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {


            Name = "Crystal Drake Alpha";
            Title = "the Prismatic Terror";
            Body = 104;         // Drake body
            Hue = 1154;         // Crystal blue
            BaseSoundID = 362;

            SetStr(500, 600);
            SetDex(120, 150);
            SetInt(350, 450);

            SetHits(1400, 1700);
            SetDamage(30, 40);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Energy, 80);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 85, 95);

            SetSkill(SkillName.EvalInt, 110.0, 130.0);
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
            m_NextShardTime = DateTime.UtcNow + TimeSpan.FromSeconds(8);
            m_NextPrismaticTime = DateTime.UtcNow + TimeSpan.FromSeconds(20);
            m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(35);
            m_NextShatterTime = DateTime.UtcNow + TimeSpan.FromSeconds(25);
            m_ShieldActive = false;
        }

        public override void GenerateLoot()
        {
            AddLoot(VystiaLootPack.CrystalRich);
            AddLoot(LootPack.FilthyRich, 2);

            PackItem(new CrystalOre(Utility.RandomMinMax(10, 20)));
            PackItem(new CrystallineIngot(Utility.RandomMinMax(5, 10)));
            PackItem(new PrismaticShard(Utility.RandomMinMax(3, 6)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new PrismaticShard(Utility.RandomMinMax(1, 2)));

            // 1% chance for Luminous Scepter legendary artifact
            if (Utility.RandomDouble() < 0.01)
                PackItem(new LuminousScepter());

            // 5% chance for Crystal equipment
            if (Utility.RandomDouble() < 0.05)
                PackItem(new QuarterStaff());
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

            if (DateTime.UtcNow >= m_NextShardTime)
            {
                DoCrystalShards();
                m_NextShardTime = DateTime.UtcNow + TimeSpan.FromSeconds(m_Phase == 3 ? 6 : 10);
            }

            if (m_Phase >= 2 && DateTime.UtcNow >= m_NextPrismaticTime)
            {
                DoPrismaticBeam();
                m_NextPrismaticTime = DateTime.UtcNow + TimeSpan.FromSeconds(15);
            }

            if (m_Phase >= 2 && DateTime.UtcNow >= m_NextSummonTime)
            {
                SummonCrystalElementals();
                m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(45);
            }

            if (m_Phase == 3 && DateTime.UtcNow >= m_NextShatterTime)
            {
                DoCrystalShatter();
                m_NextShatterTime = DateTime.UtcNow + TimeSpan.FromSeconds(20);
            }
        }

        private void CheckPhaseTransition()
        {
            double hpPercent = (double)Hits / HitsMax;

            if (m_Phase == 1 && hpPercent <= 0.66)
            {
                m_Phase = 2;
                Say("*The Crystal Drake's scales shimmer with prismatic energy*");
                PublicOverheadMessage(Network.MessageType.Regular, 0x480, false,
                    "Light itself bends to my will!");

                FixedParticles(0x376A, 9, 32, 5030, 1154, 0, EffectLayer.Waist);
                PlaySound(0x1E1);

                ActivateReflectiveShield();
            }
            else if (m_Phase == 2 && hpPercent <= 0.33)
            {
                m_Phase = 3;
                Say("*The Crystal Drake's body crackles with unstable energy*");
                PublicOverheadMessage(Network.MessageType.Regular, 0x480, false,
                    "SHATTER BEFORE THE CRYSTAL'S MIGHT!");

                FixedParticles(0x375A, 10, 15, 5037, 1154, 0, EffectLayer.Head);
                PlaySound(0x1F4);

                SetDamage(40, 55);
            }
        }

        private void ActivateReflectiveShield()
        {
            m_ShieldActive = true;
            Say("*A crystalline barrier forms around the Drake*");
            FixedParticles(0x375A, 10, 15, 5037, 1154, 0, EffectLayer.Waist);

            Timer.DelayCall(TimeSpan.FromSeconds(15), () =>
            {
                m_ShieldActive = false;
            });
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            // Reflect damage when shield is active
            if (m_ShieldActive && from != null && from != this && Utility.RandomDouble() < 0.35)
            {
                int reflectedDamage = amount / 3;
                if (reflectedDamage > 0)
                {
                    from.Damage(reflectedDamage, this);
                    from.SendMessage(0x480, "The crystalline shield reflects part of your attack!");
                    from.FixedParticles(0x374A, 10, 15, 5028, 1154, 0, EffectLayer.Waist);
                }
            }
        }

        private void DoCrystalShards()
        {
            if (Combatant == null)
                return;

            Mobile target = Combatant as Mobile;
            if (target == null || !CanBeHarmful(target))
                return;

            Say("*Crystal shards fly through the air*");

            DoHarmful(target);

            // Projectile effect
            Effects.SendMovingParticles(this, target, 0x36D4, 7, 0, false, true, 1154, 0, 9502, 1, 0, EffectLayer.Head, 0x100);
            PlaySound(0x1E2);

            Timer.DelayCall(TimeSpan.FromSeconds(0.3), () =>
            {
                if (target != null && !target.Deleted && target.Alive)
                {
                    int damage = Utility.RandomMinMax(25, 40);
                    AOS.Damage(target, this, damage, 0, 0, 0, 0, 100);
                    target.SendMessage(0x480, "Crystal shards pierce your flesh!");

                    target.FixedParticles(0x374A, 10, 15, 5028, 1154, 0, EffectLayer.Waist);
                }
            });
        }

        private void DoPrismaticBeam()
        {
            if (Map == null)
                return;

            Say("*The Crystal Drake unleashes a prismatic beam*");
            PublicOverheadMessage(Network.MessageType.Regular, 0x480, false, "PRISMATIC BEAM!");

            PlaySound(0x211);

            foreach (Mobile m in GetMobilesInRange(10))
            {
                if (m == this || !CanBeHarmful(m))
                    continue;

                if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                {
                    DoHarmful(m);

                    Effects.SendMovingParticles(this, m, 0x36D4, 7, 0, false, true, 1154, 0, 9502, 1, 0, EffectLayer.Head, 0x100);

                    Timer.DelayCall(TimeSpan.FromSeconds(0.5), () =>
                    {
                        if (m != null && !m.Deleted && m.Alive)
                        {
                            int damage = Utility.RandomMinMax(35, 55);
                            AOS.Damage(m, this, damage, 0, 0, 0, 0, 100);

                            // Mana drain effect
                            if (m is PlayerMobile pm)
                            {
                                int manaDrain = Utility.RandomMinMax(20, 40);
                                pm.Mana = Math.Max(0, pm.Mana - manaDrain);
                                pm.SendMessage(0x480, "The prismatic energy drains your mana!");
                            }

                            m.FixedParticles(0x36BD, 20, 10, 5044, 1154, 0, EffectLayer.Head);
                        }
                    });
                }
            }
        }

        private void DoCrystalShatter()
        {
            if (Map == null)
                return;

            Say("*The Crystal Drake causes a massive crystal explosion*");
            PublicOverheadMessage(Network.MessageType.Regular, 0x480, false, "SHATTER!");

            PlaySound(0x1F4);
            FixedParticles(0x36BD, 20, 10, 5044, 1154, 0, EffectLayer.Head);

            foreach (Mobile m in GetMobilesInRange(8))
            {
                if (m == this || !CanBeHarmful(m))
                    continue;

                if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                {
                    DoHarmful(m);

                    m.FixedParticles(0x374A, 10, 15, 5028, 1154, 0, EffectLayer.Waist);

                    int damage = Utility.RandomMinMax(55, 80);
                    AOS.Damage(m, this, damage, 0, 0, 0, 0, 100);
                    m.SendMessage(0x480, "Crystal shards explode around you!");

                    // Bleeding effect (DoT)
                    Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), 4, () =>
                    {
                        if (m != null && !m.Deleted && m.Alive)
                        {
                            AOS.Damage(m, this, Utility.RandomMinMax(8, 15), 100, 0, 0, 0, 0);
                        }
                    });
                }
            }
        }

        private void SummonCrystalElementals()
        {
            if (Map == null || Map == Map.Internal)
                return;

            int count = m_Phase == 3 ? 3 : 2;

            Say("*The Crystal Drake calls forth crystal guardians*");

            for (int i = 0; i < count; i++)
            {
                BaseCreature minion = new EnergyVortex();
                minion.Name = "a crystal elemental";
                minion.Hue = 1154;

                Point3D loc = GetSpawnPosition(4);
                if (loc != Point3D.Zero)
                {
                    minion.MoveToWorld(loc, Map);
                    minion.Combatant = Combatant;

                    Effects.SendLocationParticles(
                        EffectItem.Create(loc, Map, EffectItem.DefaultDuration),
                        0x376A, 9, 32, 1154, 0, 5030, 0);
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
            Say("*The Crystal Drake shatters into a thousand pieces*");
            PublicOverheadMessage(Network.MessageType.Regular, 0x480, false,
                "The light... fades...");

            Effects.SendLocationParticles(
                EffectItem.Create(Location, Map, EffectItem.DefaultDuration),
                0x36BD, 20, 10, 1154, 0, 5044, 0);
            Effects.PlaySound(Location, Map, 0x1F4);

            base.OnDeath(c);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x480, "Crystal energy courses through your body!");
                int manaDrain = Utility.RandomMinMax(10, 25);
                if (defender is PlayerMobile pm)
                {
                    pm.Mana = Math.Max(0, pm.Mana - manaDrain);
                }
            }
        }

        public CrystalDrakeAlpha(Serial serial) : base(serial)
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
            m_NextShardTime = DateTime.UtcNow + TimeSpan.FromSeconds(8);
            m_NextPrismaticTime = DateTime.UtcNow + TimeSpan.FromSeconds(20);
            m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(35);
            m_NextShatterTime = DateTime.UtcNow + TimeSpan.FromSeconds(25);
            m_ShieldActive = false;
        }
    }
}