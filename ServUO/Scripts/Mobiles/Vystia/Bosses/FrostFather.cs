using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
    /// <summary>
    /// Frost Father - Boss of the Frozen Halls dungeon (Frosthold)
    ///
    /// Mechanics:
    /// - Phase 1 (100-66% HP): Normal combat with cold aura
    /// - Phase 2 (66-33% HP): Frost Shield activates, summons Frost Wraiths
    /// - Phase 3 (33-0% HP): Cone Freeze attack, increased damage
    ///
    /// Key: Frost Seal (drops from Frost Wraiths)
    /// Rewards: Frozen Artifacts, Heartwood Core fragment (rare)
    /// </summary>
    [CorpseName("the corpse of Frost Father")]
    public class FrostFather : BaseCreature, IAuraCreature
    {
        private int m_Phase = 1;
        private DateTime m_NextPhaseCheck;
        private DateTime m_NextConeFreezeTime;
        private DateTime m_NextSummonTime;
        private bool m_FrostShieldActive = false;

        [Constructable]
        public FrostFather() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Frost Father";
            Title = "Guardian of the Frozen Halls";
            Body = 76;          // Titan
            Hue = 1152;         // Ice blue
            BaseSoundID = 609;

            SetStr(600, 700);
            SetDex(60, 80);
            SetInt(100, 150);

            SetHits(1200, 1500);
            SetDamage(35, 45);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Cold, 50);

            SetResistance(ResistanceType.Physical, 65, 75);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 90, 100);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.EvalInt, 80.0, 100.0);
            SetSkill(SkillName.Magery, 90.0, 110.0);
            SetSkill(SkillName.MagicResist, 100.0, 120.0);
            SetSkill(SkillName.Meditation, 50.0, 70.0);
            SetSkill(SkillName.Tactics, 100.0, 120.0);
            SetSkill(SkillName.Wrestling, 100.0, 120.0);

            Fame = 25000;
            Karma = -25000;
            VirtualArmor = 70;

            Tamable = false;

            // Enable cold breath
            SetSpecialAbility(SpecialAbility.DragonBreath);
            SetAreaEffect(AreaEffect.AuraDamage);

            m_NextPhaseCheck = DateTime.UtcNow;
            m_NextConeFreezeTime = DateTime.UtcNow + TimeSpan.FromSeconds(30);
            m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(45);
        }

        public override void GenerateLoot()
        {
            // Use Vystia Frosthold boss loot pack for standard drops
            AddLoot(VystiaLootPack.FrostholdBoss);
            AddLoot(LootPack.FilthyRich, 2);

            // Guaranteed Frozen Artifacts (1-3) - boss always drops some
            PackItem(new FrozenArtifact(Utility.RandomMinMax(1, 3)));

            // 10% chance for additional Frozen Artifacts
            if (Utility.RandomDouble() < 0.10)
            {
                PackItem(new FrozenArtifact(Utility.RandomMinMax(2, 4)));
            }

            // 1% chance for Heartwood Core Fragment (ultra-rare)
            if (Utility.RandomDouble() < 0.01)
            {
                PackItem(new HeartwoodCoreFragment());
            }

            // 2% chance for The Eternal Winter legendary weapon
            if (Utility.RandomDouble() < 0.02)
            {
                PackItem(new TheEternalWinter());
            }
        }

        public override bool Unprovokable => true;
        public override bool BardImmune => true;
        public override bool AlwaysMurderer => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override int TreasureMapLevel => 5;

        /// <summary>
        /// Cold aura effect - damages nearby enemies
        /// </summary>
        public void AuraEffect(Mobile m)
        {
            if (m == null || m.Deleted || !m.Alive)
                return;

            m.FixedParticles(0x374A, 10, 30, 5052, Hue, 0, EffectLayer.Waist);
            m.PlaySound(0x5C6);

            int damage = Utility.RandomMinMax(8, 15);

            // Phase 3: Increased aura damage
            if (m_Phase == 3)
                damage = Utility.RandomMinMax(12, 20);

            AOS.Damage(m, this, damage, 0, 0, 100, 0, 0);
            m.SendMessage(0x480, "The intense cold from Frost Father is damaging you!");
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant == null || Deleted || !Alive)
                return;

            // Phase check every 2 seconds
            if (DateTime.UtcNow >= m_NextPhaseCheck)
            {
                m_NextPhaseCheck = DateTime.UtcNow + TimeSpan.FromSeconds(2);
                CheckPhaseTransition();
            }

            // Phase 2+: Summon Frost Wraiths
            if (m_Phase >= 2 && DateTime.UtcNow >= m_NextSummonTime)
            {
                SummonFrostWraiths();
                m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(45);
            }

            // Phase 3: Cone Freeze attack
            if (m_Phase == 3 && DateTime.UtcNow >= m_NextConeFreezeTime)
            {
                DoConeFreeze();
                m_NextConeFreezeTime = DateTime.UtcNow + TimeSpan.FromSeconds(20);
            }
        }

        private void CheckPhaseTransition()
        {
            double hpPercent = (double)Hits / HitsMax;

            if (m_Phase == 1 && hpPercent <= 0.66)
            {
                // Transition to Phase 2
                m_Phase = 2;
                m_FrostShieldActive = true;

                Say("*An icy shield forms around Frost Father*");
                PublicOverheadMessage(Network.MessageType.Regular, 0x480, false,
                    "You cannot defeat the cold!");

                // Visual effect
                FixedParticles(0x376A, 9, 32, 5030, 1152, 0, EffectLayer.Waist);
                PlaySound(0x1FB);

                // Boost resistances during frost shield
                SetResistance(ResistanceType.Physical, 75, 85);
                SetResistance(ResistanceType.Fire, 40, 50);

                // Summon initial wraiths
                SummonFrostWraiths();
            }
            else if (m_Phase == 2 && hpPercent <= 0.33)
            {
                // Transition to Phase 3
                m_Phase = 3;
                m_FrostShieldActive = false;

                Say("*The frost shield shatters as Frost Father enters a rage*");
                PublicOverheadMessage(Network.MessageType.Regular, 0x480, false,
                    "FEEL THE WRATH OF ETERNAL WINTER!");

                // Visual effect
                FixedParticles(0x36BD, 20, 10, 5044, 1152, 0, EffectLayer.Head);
                PlaySound(0x307);

                // Increase damage in final phase
                SetDamage(40, 55);

                // Reset resistances
                SetResistance(ResistanceType.Physical, 65, 75);
                SetResistance(ResistanceType.Fire, 20, 30);
            }
        }

        private void SummonFrostWraiths()
        {
            if (Map == null || Map == Map.Internal)
                return;

            int count = m_Phase == 2 ? 2 : 3;

            Say("*Frost Father summons servants from the frozen depths*");
            PublicOverheadMessage(Network.MessageType.Regular, 0x480, false,
                "Rise, my children!");

            for (int i = 0; i < count; i++)
            {
                // Use Wraith as base until FrostWraith is implemented
                BaseCreature minion = new Wraith();
                minion.Name = "a frost wraith";
                minion.Hue = 1152;
                minion.SetDamageType(ResistanceType.Physical, 50);
                minion.SetDamageType(ResistanceType.Cold, 50);

                Point3D loc = GetSpawnPosition(3);
                if (loc != Point3D.Zero)
                {
                    minion.MoveToWorld(loc, Map);
                    minion.Combatant = Combatant;

                    // Effect at spawn location
                    Effects.SendLocationParticles(
                        EffectItem.Create(loc, Map, EffectItem.DefaultDuration),
                        0x376A, 9, 32, 1152, 0, 5030, 0);
                }
                else
                {
                    minion.Delete();
                }
            }
        }

        private void DoConeFreeze()
        {
            if (Combatant == null || Map == null)
                return;

            Say("*Frost Father unleashes a cone of freezing cold*");
            PublicOverheadMessage(Network.MessageType.Regular, 0x480, false,
                "FREEZE!");

            PlaySound(0x64F);

            List<Mobile> targets = new List<Mobile>();

            foreach (Mobile m in GetMobilesInRange(8))
            {
                if (m == this || !CanBeHarmful(m))
                    continue;

                if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                {
                    // Check if target is in front (cone attack)
                    Direction d = GetDirectionTo(m);
                    if (InCone(d))
                    {
                        targets.Add(m);
                    }
                }
            }

            foreach (Mobile target in targets)
            {
                DoHarmful(target);

                // Freeze effect
                target.Frozen = true;
                target.FixedParticles(0x376A, 9, 32, 5030, 1152, 0, EffectLayer.Waist);
                target.PlaySound(0x1FB);

                int damage = Utility.RandomMinMax(50, 70);
                AOS.Damage(target, this, damage, 0, 0, 100, 0, 0);

                target.SendMessage(0x480, "You are frozen solid by the cone of cold!");

                // Unfreeze after 3 seconds
                Timer.DelayCall(TimeSpan.FromSeconds(3), () =>
                {
                    if (target != null && !target.Deleted)
                    {
                        target.Frozen = false;
                        target.SendMessage("You break free from the ice!");
                    }
                });
            }
        }

        private bool InCone(Direction d)
        {
            // Check if direction is within a 90-degree cone in front
            Direction facing = Direction;
            int diff = Math.Abs((int)facing - (int)d);
            if (diff > 4) diff = 8 - diff;
            return diff <= 2;
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
            Say("*Frost Father crumbles into ice and snow*");
            PublicOverheadMessage(Network.MessageType.Regular, 0x480, false,
                "The cold... never... dies...");

            // Death effect
            Effects.SendLocationParticles(
                EffectItem.Create(Location, Map, EffectItem.DefaultDuration),
                0x36BD, 20, 10, 1152, 0, 5044, 0);
            Effects.PlaySound(Location, Map, 0x307);

            base.OnDeath(c);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            // Chance to apply frostbite (slow effect)
            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x480, "Frost Father's attack chills you to the bone!");

                // Reduce dex temporarily
                if (defender is PlayerMobile pm)
                {
                    pm.AddStatMod(new StatMod(StatType.Dex, "FrostFatherChill", -20, TimeSpan.FromSeconds(10)));
                }
            }
        }

        public FrostFather(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version

            writer.Write(m_Phase);
            writer.Write(m_FrostShieldActive);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 1)
            {
                m_Phase = reader.ReadInt();
                m_FrostShieldActive = reader.ReadBool();
            }

            m_NextPhaseCheck = DateTime.UtcNow;
            m_NextConeFreezeTime = DateTime.UtcNow + TimeSpan.FromSeconds(30);
            m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(45);
        }
    }
}
