using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
    /// <summary>
    /// Volcano Wyrm - Boss of the Emberlands
    ///
    /// Mechanics:
    /// - Phase 1 (100-66% HP): Fire breath and magma pools
    /// - Phase 2 (66-33% HP): Summons Fire Elementals, creates lava zones
    /// - Phase 3 (33-0% HP): Eruption attack, massive AoE damage
    ///
    /// Rewards: Phoenix Ascension (legendary katana), Emberlands resources
    /// </summary>
    [CorpseName("the corpse of the Volcano Wyrm")]
    public class VolcanoWyrm : BaseCreature, IAuraCreature
    {
        private int m_Phase = 1;
        private DateTime m_NextPhaseCheck;
        private DateTime m_NextEruptionTime;
        private DateTime m_NextSummonTime;
        private DateTime m_NextMagmaPoolTime;

        [Constructable]
        public VolcanoWyrm() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {


            Name = "Volcano Wyrm";
            Title = "the Molten Terror";
            Body = 46;          // Dragon
            Hue = 1358;         // Fire orange
            BaseSoundID = 362;

            SetStr(700, 800);
            SetDex(80, 100);
            SetInt(150, 200);

            SetHits(1500, 1800);
            SetDamage(40, 50);

            SetDamageType(ResistanceType.Physical, 30);
            SetDamageType(ResistanceType.Fire, 70);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 90, 100);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.EvalInt, 90.0, 110.0);
            SetSkill(SkillName.Magery, 100.0, 120.0);
            SetSkill(SkillName.MagicResist, 100.0, 120.0);
            SetSkill(SkillName.Meditation, 60.0, 80.0);
            SetSkill(SkillName.Tactics, 100.0, 120.0);
            SetSkill(SkillName.Wrestling, 100.0, 120.0);

            Fame = 28000;
            Karma = -28000;
            VirtualArmor = 75;

            Tamable = false;

            SetSpecialAbility(SpecialAbility.DragonBreath);
            SetAreaEffect(AreaEffect.AuraDamage);

            m_NextPhaseCheck = DateTime.UtcNow;
            m_NextEruptionTime = DateTime.UtcNow + TimeSpan.FromSeconds(40);
            m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(30);
            m_NextMagmaPoolTime = DateTime.UtcNow + TimeSpan.FromSeconds(15);
        }

        public override void GenerateLoot()
        {
            AddLoot(VystiaLootPack.EmberlandsRich);
            AddLoot(LootPack.FilthyRich, 2);

            PackItem(new MoltenOre(Utility.RandomMinMax(10, 20)));
            PackItem(new EmberforgedIngot(Utility.RandomMinMax(5, 10)));
            PackItem(new EverburningCoal(Utility.RandomMinMax(2, 5)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new Server.Items.LavaPearl(Utility.RandomMinMax(1, 3)));

            // 2% chance for Phoenix Ascension legendary weapon
            if (Utility.RandomDouble() < 0.02)
                PackItem(new PhoenixAscension());

            // 1% chance for Magma Heart legendary artifact
            if (Utility.RandomDouble() < 0.01)
                PackItem(new MagmaHeart());
        }

        public override bool Unprovokable => true;
        public override bool BardImmune => true;
        public override bool AlwaysMurderer => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override int TreasureMapLevel => 5;

        public void AuraEffect(Mobile m)
        {
            if (m == null || m.Deleted || !m.Alive)
                return;

            m.FixedParticles(0x3709, 10, 30, 5052, Hue, 0, EffectLayer.Waist);
            m.PlaySound(0x208);

            int damage = m_Phase == 3 ? Utility.RandomMinMax(15, 25) : Utility.RandomMinMax(10, 18);
            AOS.Damage(m, this, damage, 0, 100, 0, 0, 0);
            m.SendMessage(0x22, "The intense heat from the Volcano Wyrm burns you!");
        }

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

            if (DateTime.UtcNow >= m_NextMagmaPoolTime)
            {
                CreateMagmaPool();
                m_NextMagmaPoolTime = DateTime.UtcNow + TimeSpan.FromSeconds(m_Phase == 3 ? 8 : 12);
            }

            if (m_Phase >= 2 && DateTime.UtcNow >= m_NextSummonTime)
            {
                SummonFireElementals();
                m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(40);
            }

            if (m_Phase == 3 && DateTime.UtcNow >= m_NextEruptionTime)
            {
                DoEruption();
                m_NextEruptionTime = DateTime.UtcNow + TimeSpan.FromSeconds(25);
            }
        }

        private void CheckPhaseTransition()
        {
            double hpPercent = (double)Hits / HitsMax;

            if (m_Phase == 1 && hpPercent <= 0.66)
            {
                m_Phase = 2;
                Say("*The Volcano Wyrm's scales begin to glow with molten fury*");
                PublicOverheadMessage(Network.MessageType.Regular, 0x22, false,
                    "The earth shall consume you!");

                FixedParticles(0x3709, 10, 30, 5052, 1358, 0, EffectLayer.Waist);
                PlaySound(0x208);

                SummonFireElementals();
            }
            else if (m_Phase == 2 && hpPercent <= 0.33)
            {
                m_Phase = 3;
                Say("*The Volcano Wyrm roars as magma erupts from its body*");
                PublicOverheadMessage(Network.MessageType.Regular, 0x22, false,
                    "BURN IN THE FIRES OF THE EARTH!");

                FixedParticles(0x36B0, 20, 10, 5044, 1358, 0, EffectLayer.Head);
                PlaySound(0x349);

                SetDamage(50, 65);
            }
        }

        private void CreateMagmaPool()
        {
            if (Map == null || Combatant == null)
                return;

            Mobile target = Combatant as Mobile;
            if (target == null)
                return;

            Point3D loc = target.Location;

            Say("*Magma erupts from the ground*");

            Effects.SendLocationParticles(
                EffectItem.Create(loc, Map, EffectItem.DefaultDuration),
                0x3709, 10, 30, 1358, 0, 5052, 0);
            Effects.PlaySound(loc, Map, 0x208);

            Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
            {
                foreach (Mobile m in Map.GetMobilesInRange(loc, 2))
                {
                    if (m != this && CanBeHarmful(m))
                    {
                        DoHarmful(m);
                        int damage = Utility.RandomMinMax(30, 50);
                        AOS.Damage(m, this, damage, 0, 100, 0, 0, 0);
                        m.SendMessage(0x22, "You are burned by the magma pool!");
                    }
                }
            });
        }

        private void SummonFireElementals()
        {
            if (Map == null || Map == Map.Internal)
                return;

            int count = m_Phase == 3 ? 3 : 2;

            Say("*The Volcano Wyrm calls forth servants of flame*");

            for (int i = 0; i < count; i++)
            {
                BaseCreature minion = new FireElemental();
                minion.Name = "a volcanic elemental";
                minion.Hue = 1358;

                Point3D loc = GetSpawnPosition(4);
                if (loc != Point3D.Zero)
                {
                    minion.MoveToWorld(loc, Map);
                    minion.Combatant = Combatant;

                    Effects.SendLocationParticles(
                        EffectItem.Create(loc, Map, EffectItem.DefaultDuration),
                        0x3709, 10, 30, 1358, 0, 5052, 0);
                }
                else
                {
                    minion.Delete();
                }
            }
        }

        private void DoEruption()
        {
            if (Map == null)
                return;

            Say("*The Volcano Wyrm causes a massive eruption*");
            PublicOverheadMessage(Network.MessageType.Regular, 0x22, false, "ERUPTION!");

            PlaySound(0x349);

            foreach (Mobile m in GetMobilesInRange(10))
            {
                if (m == this || !CanBeHarmful(m))
                    continue;

                if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                {
                    DoHarmful(m);

                    m.FixedParticles(0x36B0, 20, 10, 5044, 1358, 0, EffectLayer.Head);

                    int damage = Utility.RandomMinMax(60, 90);
                    AOS.Damage(m, this, damage, 0, 100, 0, 0, 0);
                    m.SendMessage(0x22, "You are engulfed in the volcanic eruption!");
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
            Say("*The Volcano Wyrm lets out a final roar as its flames die*");
            PublicOverheadMessage(Network.MessageType.Regular, 0x22, false,
                "The fire... returns... to the earth...");

            Effects.SendLocationParticles(
                EffectItem.Create(Location, Map, EffectItem.DefaultDuration),
                0x36B0, 20, 10, 1358, 0, 5044, 0);
            Effects.PlaySound(Location, Map, 0x349);

            base.OnDeath(c);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x22, "The Volcano Wyrm's attack sets you ablaze!");
                defender.FixedParticles(0x3709, 10, 30, 5052, 1358, 0, EffectLayer.LeftFoot);

                Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), 5, () =>
                {
                    if (defender != null && !defender.Deleted && defender.Alive)
                    {
                        AOS.Damage(defender, this, Utility.RandomMinMax(5, 10), 0, 100, 0, 0, 0);
                    }
                });
            }
        }

        public VolcanoWyrm(Serial serial) : base(serial)
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
            m_NextEruptionTime = DateTime.UtcNow + TimeSpan.FromSeconds(40);
            m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(30);
            m_NextMagmaPoolTime = DateTime.UtcNow + TimeSpan.FromSeconds(15);
        }
    }
}