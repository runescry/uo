using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
    /// <summary>
    /// Ancient Treant - Boss of Verdantpeak
    ///
    /// Mechanics:
    /// - Phase 1 (100-66% HP): Root attacks, entangling vines
    /// - Phase 2 (66-33% HP): Summons Reapers, nature's wrath AoE
    /// - Phase 3 (33-0% HP): Regeneration aura, devastating slam attacks
    ///
    /// Rewards: Verdantpeak resources, TreantHeart, nature equipment
    /// </summary>
    [CorpseName("the remains of the Ancient Treant")]
    public class AncientTreant : BaseCreature
    {
        private int m_Phase = 1;
        private DateTime m_NextPhaseCheck;
        private DateTime m_NextRootTime;
        private DateTime m_NextVineTime;
        private DateTime m_NextSummonTime;
        private DateTime m_NextSlamTime;
        private DateTime m_NextRegenTime;

        [Constructable]
        public AncientTreant() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.3, 0.5)
        {
            Name = "Ancient Treant";
            Title = "Guardian of the Grove";
            Body = 301;         // Reaper body
            Hue = 2010;         // Forest green
            BaseSoundID = 442;

            SetStr(600, 700);
            SetDex(60, 80);
            SetInt(200, 250);

            SetHits(1600, 1900);
            SetDamage(35, 45);

            SetDamageType(ResistanceType.Physical, 70);
            SetDamageType(ResistanceType.Poison, 30);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 80, 90);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.MagicResist, 100.0, 120.0);
            SetSkill(SkillName.Tactics, 110.0, 130.0);
            SetSkill(SkillName.Wrestling, 110.0, 130.0);
            SetSkill(SkillName.Anatomy, 80.0, 100.0);

            Fame = 27000;
            Karma = -27000;
            VirtualArmor = 70;

            Tamable = false;

            m_NextPhaseCheck = DateTime.UtcNow;
            m_NextRootTime = DateTime.UtcNow + TimeSpan.FromSeconds(10);
            m_NextVineTime = DateTime.UtcNow + TimeSpan.FromSeconds(15);
            m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(35);
            m_NextSlamTime = DateTime.UtcNow + TimeSpan.FromSeconds(20);
            m_NextRegenTime = DateTime.UtcNow + TimeSpan.FromSeconds(5);
        }

        public override void GenerateLoot()
        {
            AddLoot(VystiaLootPack.VerdantpeakRich);
            AddLoot(LootPack.FilthyRich, 2);

            PackItem(new LivingOre(Utility.RandomMinMax(15, 25)));
            PackItem(new NatureforgedIngot(Utility.RandomMinMax(5, 10)));
            PackItem(new LivingOre(Utility.RandomMinMax(3, 6)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new TreantHeart(Utility.RandomMinMax(1, 2)));

            // 1% chance for Heartwood Core legendary artifact
            if (Utility.RandomDouble() < 0.01)
                PackItem(new HeartwoodCore());

            // 5% chance for Verdantpeak equipment
            if (Utility.RandomDouble() < 0.05)
                PackItem(new Bow());
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

            if (DateTime.UtcNow >= m_NextRootTime)
            {
                DoRootAttack();
                m_NextRootTime = DateTime.UtcNow + TimeSpan.FromSeconds(m_Phase == 3 ? 8 : 12);
            }

            if (DateTime.UtcNow >= m_NextVineTime)
            {
                DoVineEntangle();
                m_NextVineTime = DateTime.UtcNow + TimeSpan.FromSeconds(15);
            }

            if (m_Phase >= 2 && DateTime.UtcNow >= m_NextSummonTime)
            {
                SummonReapers();
                m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(45);
            }

            if (m_Phase == 3 && DateTime.UtcNow >= m_NextSlamTime)
            {
                DoDevastatingSlam();
                m_NextSlamTime = DateTime.UtcNow + TimeSpan.FromSeconds(18);
            }

            if (m_Phase == 3 && DateTime.UtcNow >= m_NextRegenTime)
            {
                DoRegeneration();
                m_NextRegenTime = DateTime.UtcNow + TimeSpan.FromSeconds(3);
            }
        }

        private void CheckPhaseTransition()
        {
            double hpPercent = (double)Hits / HitsMax;

            if (m_Phase == 1 && hpPercent <= 0.66)
            {
                m_Phase = 2;
                Say("*The Ancient Treant's bark cracks with ancient power*");
                PublicOverheadMessage(Network.MessageType.Regular, 0x3B2, false,
                    "The forest awakens to destroy you!");

                FixedParticles(0x376A, 9, 32, 5030, 2010, 0, EffectLayer.Waist);
                PlaySound(0x1F7);

                SummonReapers();
            }
            else if (m_Phase == 2 && hpPercent <= 0.33)
            {
                m_Phase = 3;
                Say("*The Ancient Treant draws power from the earth itself*");
                PublicOverheadMessage(Network.MessageType.Regular, 0x3B2, false,
                    "NATURE'S WRATH SHALL CONSUME YOU!");

                FixedParticles(0x375A, 10, 15, 5037, 2010, 0, EffectLayer.Head);
                PlaySound(0x1F8);

                SetDamage(45, 60);
            }
        }

        private void DoRootAttack()
        {
            if (Combatant == null)
                return;

            Mobile target = Combatant as Mobile;
            if (target == null || !CanBeHarmful(target))
                return;

            Say("*Roots erupt from the ground*");

            DoHarmful(target);
            target.FixedParticles(0x376A, 9, 32, 5005, 2010, 0, EffectLayer.Waist);
            target.PlaySound(0x1F8);

            // Root effect - paralyze
            target.Freeze(TimeSpan.FromSeconds(3));
            target.SendMessage(0x3B2, "Roots entangle your legs!");

            int damage = Utility.RandomMinMax(20, 35);
            AOS.Damage(target, this, damage, 50, 0, 0, 50, 0);
        }

        private void DoVineEntangle()
        {
            if (Map == null)
                return;

            Say("*Vines lash out at all nearby foes*");
            PlaySound(0x1F7);

            foreach (Mobile m in GetMobilesInRange(8))
            {
                if (m == this || !CanBeHarmful(m))
                    continue;

                if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                {
                    DoHarmful(m);

                    m.FixedParticles(0x374A, 10, 15, 5028, 2010, 0, EffectLayer.Waist);

                    // Slow effect
                    m.AddStatMod(new StatMod(StatType.Dex, "VineEntangle", -25, TimeSpan.FromSeconds(8)));
                    m.SendMessage(0x3B2, "Vines slow your movement!");

                    int damage = Utility.RandomMinMax(15, 25);
                    AOS.Damage(m, this, damage, 0, 0, 0, 100, 0);
                }
            }
        }

        private void DoDevastatingSlam()
        {
            if (Map == null)
                return;

            Say("*The Ancient Treant slams the ground with tremendous force*");
            PublicOverheadMessage(Network.MessageType.Regular, 0x3B2, false, "GROUND SLAM!");

            PlaySound(0x2A1);

            foreach (Mobile m in GetMobilesInRange(6))
            {
                if (m == this || !CanBeHarmful(m))
                    continue;

                if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                {
                    DoHarmful(m);

                    m.FixedParticles(0x36BD, 20, 10, 5044, 2010, 0, EffectLayer.Head);

                    int damage = Utility.RandomMinMax(50, 75);
                    AOS.Damage(m, this, damage, 100, 0, 0, 0, 0);
                    m.SendMessage(0x3B2, "The ground slam throws you off balance!");

                    // Knockback effect (stun)
                    m.Freeze(TimeSpan.FromSeconds(1.5));
                }
            }
        }

        private void DoRegeneration()
        {
            if (Hits < HitsMax)
            {
                int regen = Utility.RandomMinMax(20, 40);
                Hits = Math.Min(Hits + regen, HitsMax);

                if (Utility.RandomDouble() < 0.3)
                {
                    FixedParticles(0x376A, 9, 32, 5005, 2010, 0, EffectLayer.Waist);
                    Say("*The Treant draws strength from nature*");
                }
            }
        }

        private void SummonReapers()
        {
            if (Map == null || Map == Map.Internal)
                return;

            int count = m_Phase == 3 ? 3 : 2;

            Say("*The Ancient Treant calls forth its brethren*");

            for (int i = 0; i < count; i++)
            {
                BaseCreature minion = new Reaper();
                minion.Name = "a forest guardian";
                minion.Hue = 2010;

                Point3D loc = GetSpawnPosition(4);
                if (loc != Point3D.Zero)
                {
                    minion.MoveToWorld(loc, Map);
                    minion.Combatant = Combatant;

                    Effects.SendLocationParticles(
                        EffectItem.Create(loc, Map, EffectItem.DefaultDuration),
                        0x376A, 9, 32, 2010, 0, 5030, 0);
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
            Say("*The Ancient Treant collapses, returning to the earth*");
            PublicOverheadMessage(Network.MessageType.Regular, 0x3B2, false,
                "The forest... endures...");

            Effects.SendLocationParticles(
                EffectItem.Create(Location, Map, EffectItem.DefaultDuration),
                0x376A, 9, 32, 2010, 0, 5030, 0);
            Effects.PlaySound(Location, Map, 0x1F8);

            base.OnDeath(c);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.25)
            {
                defender.ApplyPoison(this, Poison.Greater);
                defender.SendMessage(0x3B2, "The Treant's thorns poison you!");
            }
        }

        public AncientTreant(Serial serial) : base(serial)
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
            m_NextRootTime = DateTime.UtcNow + TimeSpan.FromSeconds(10);
            m_NextVineTime = DateTime.UtcNow + TimeSpan.FromSeconds(15);
            m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(35);
            m_NextSlamTime = DateTime.UtcNow + TimeSpan.FromSeconds(20);
            m_NextRegenTime = DateTime.UtcNow + TimeSpan.FromSeconds(5);
        }
    }
}
