using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
    /// <summary>
    /// Coven Matriarch - Boss of Shadowfen
    ///
    /// Mechanics:
    /// - Phase 1 (100-66% HP): Poison attacks, curse spells
    /// - Phase 2 (66-33% HP): Summons Bog Hags, creates poison clouds
    /// - Phase 3 (33-0% HP): Mass curse, life drain, shadow step
    ///
    /// Rewards: Shadowfen resources, ShadowSilk, poison equipment
    /// </summary>
    [CorpseName("the corpse of the Coven Matriarch")]
    public class CovenMatriarch : BaseCreature
    {
        private int m_Phase = 1;
        private DateTime m_NextPhaseCheck;
        private DateTime m_NextPoisonCloudTime;
        private DateTime m_NextCurseTime;
        private DateTime m_NextSummonTime;
        private DateTime m_NextLifeDrainTime;

        [Constructable]
        public CovenMatriarch() : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Coven Matriarch";
            Title = "the Swamp Witch";
            Body = 72;          // Hag body
            Hue = 1109;         // Shadow black
            BaseSoundID = 0x482;

            SetStr(400, 500);
            SetDex(120, 150);
            SetInt(400, 500);

            SetHits(1300, 1600);
            SetDamage(25, 35);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Poison, 60);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 90, 100);
            SetResistance(ResistanceType.Energy, 60, 70);

            SetSkill(SkillName.EvalInt, 100.0, 120.0);
            SetSkill(SkillName.Magery, 110.0, 130.0);
            SetSkill(SkillName.Necromancy, 110.0, 130.0);
            SetSkill(SkillName.SpiritSpeak, 100.0, 120.0);
            SetSkill(SkillName.MagicResist, 100.0, 120.0);
            SetSkill(SkillName.Poisoning, 100.0, 120.0);
            SetSkill(SkillName.Tactics, 80.0, 100.0);
            SetSkill(SkillName.Wrestling, 80.0, 100.0);

            Fame = 25000;
            Karma = -25000;
            VirtualArmor = 65;

            Tamable = false;

            m_NextPhaseCheck = DateTime.UtcNow;
            m_NextPoisonCloudTime = DateTime.UtcNow + TimeSpan.FromSeconds(15);
            m_NextCurseTime = DateTime.UtcNow + TimeSpan.FromSeconds(10);
            m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(30);
            m_NextLifeDrainTime = DateTime.UtcNow + TimeSpan.FromSeconds(20);
        }

        public override void GenerateLoot()
        {
            AddLoot(VystiaLootPack.ShadowfenRich);
            AddLoot(LootPack.FilthyRich, 2);

            PackItem(new BogIronOre(Utility.RandomMinMax(10, 20)));
            PackItem(new ShadowforgedIngot(Utility.RandomMinMax(5, 10)));
            PackItem(new SwampLotus(Utility.RandomMinMax(3, 6)));
            PackItem(new VoidDust(Utility.RandomMinMax(3, 6)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new ShadowSilk(Utility.RandomMinMax(2, 4)));

            // 5% chance for Shadowfen equipment
            if (Utility.RandomDouble() < 0.05)
                PackItem(new ShadowFang());
        }

        public override bool Unprovokable => true;
        public override bool BardImmune => true;
        public override bool AlwaysMurderer => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override Poison HitPoison => Poison.Lethal;
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

            if (DateTime.UtcNow >= m_NextCurseTime)
            {
                DoCurse();
                m_NextCurseTime = DateTime.UtcNow + TimeSpan.FromSeconds(m_Phase == 3 ? 8 : 12);
            }

            if (DateTime.UtcNow >= m_NextPoisonCloudTime && m_Phase >= 2)
            {
                DoPoisonCloud();
                m_NextPoisonCloudTime = DateTime.UtcNow + TimeSpan.FromSeconds(15);
            }

            if (m_Phase >= 2 && DateTime.UtcNow >= m_NextSummonTime)
            {
                SummonBogCreatures();
                m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(40);
            }

            if (m_Phase == 3 && DateTime.UtcNow >= m_NextLifeDrainTime)
            {
                DoLifeDrain();
                m_NextLifeDrainTime = DateTime.UtcNow + TimeSpan.FromSeconds(15);
            }
        }

        private void CheckPhaseTransition()
        {
            double hpPercent = (double)Hits / HitsMax;

            if (m_Phase == 1 && hpPercent <= 0.66)
            {
                m_Phase = 2;
                Say("*The Matriarch's form shimmers with dark energy*");
                PublicOverheadMessage(Network.MessageType.Regular, 0x497, false,
                    "The swamp shall claim your bones!");

                FixedParticles(0x374A, 10, 15, 5021, 1109, 0, EffectLayer.Waist);
                PlaySound(0x1FB);

                SummonBogCreatures();
            }
            else if (m_Phase == 2 && hpPercent <= 0.33)
            {
                m_Phase = 3;
                Say("*The Matriarch screams as shadows engulf her*");
                PublicOverheadMessage(Network.MessageType.Regular, 0x497, false,
                    "YOUR SOULS ARE MINE!");

                FixedParticles(0x376A, 9, 32, 5030, 1109, 0, EffectLayer.Head);
                PlaySound(0x482);

                SetDamage(35, 50);
                DoMassCurse();
            }
        }

        private void DoCurse()
        {
            if (Combatant == null)
                return;

            Mobile target = Combatant as Mobile;
            if (target == null || !CanBeHarmful(target))
                return;

            Say("*The Matriarch weaves a dark curse*");

            DoHarmful(target);
            target.FixedParticles(0x374A, 10, 15, 5028, 1109, 0, EffectLayer.Waist);
            target.PlaySound(0x1E1);

            // Apply stat curses
            target.AddStatMod(new StatMod(StatType.Str, "MatriarchCurseStr", -15, TimeSpan.FromSeconds(20)));
            target.AddStatMod(new StatMod(StatType.Dex, "MatriarchCurseDex", -15, TimeSpan.FromSeconds(20)));
            target.AddStatMod(new StatMod(StatType.Int, "MatriarchCurseInt", -15, TimeSpan.FromSeconds(20)));

            target.SendMessage(0x497, "You feel weakened by the Matriarch's curse!");
        }

        private void DoMassCurse()
        {
            Say("*A wave of dark energy spreads from the Matriarch*");

            foreach (Mobile m in GetMobilesInRange(10))
            {
                if (m is PlayerMobile pm && CanBeHarmful(pm))
                {
                    DoHarmful(pm);
                    pm.FixedParticles(0x374A, 10, 15, 5028, 1109, 0, EffectLayer.Waist);

                    pm.AddStatMod(new StatMod(StatType.Str, "MassCurseStr", -20, TimeSpan.FromSeconds(30)));
                    pm.AddStatMod(new StatMod(StatType.Dex, "MassCurseDex", -20, TimeSpan.FromSeconds(30)));
                    pm.AddStatMod(new StatMod(StatType.Int, "MassCurseInt", -20, TimeSpan.FromSeconds(30)));

                    pm.SendMessage(0x497, "A powerful curse saps your strength!");
                }
            }
        }

        private void DoPoisonCloud()
        {
            if (Map == null || Combatant == null)
                return;

            Mobile target = Combatant as Mobile;
            if (target == null)
                return;

            Point3D loc = target.Location;

            Say("*Toxic fumes rise from the swamp*");

            Effects.SendLocationParticles(
                EffectItem.Create(loc, Map, EffectItem.DefaultDuration),
                0x36B0, 10, 30, 1109, 0, 5052, 0);
            Effects.PlaySound(loc, Map, 0x230);

            // Damage over time in the cloud area
            for (int i = 0; i < 5; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(i), () =>
                {
                    if (Map == null)
                        return;

                    foreach (Mobile m in Map.GetMobilesInRange(loc, 3))
                    {
                        if (m != this && CanBeHarmful(m))
                        {
                            DoHarmful(m);
                            m.ApplyPoison(this, Poison.Greater);
                            int damage = Utility.RandomMinMax(10, 20);
                            AOS.Damage(m, this, damage, 0, 0, 0, 100, 0);
                        }
                    }
                });
            }
        }

        private void DoLifeDrain()
        {
            if (Map == null)
                return;

            Say("*The Matriarch drains life from her enemies*");
            PublicOverheadMessage(Network.MessageType.Regular, 0x497, false, "FEED ME!");

            PlaySound(0x231);

            int totalDrained = 0;

            foreach (Mobile m in GetMobilesInRange(8))
            {
                if (m == this || !CanBeHarmful(m))
                    continue;

                if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                {
                    DoHarmful(m);

                    // Life drain effect
                    Effects.SendMovingParticles(m, this, 0x36D4, 7, 0, false, true, 1109, 0, 9502, 1, 0, EffectLayer.Waist, 0x100);

                    int damage = Utility.RandomMinMax(30, 50);
                    AOS.Damage(m, this, damage, 0, 0, 0, 100, 0);
                    totalDrained += damage;

                    m.SendMessage(0x497, "Your life force is being drained!");
                }
            }

            // Heal the Matriarch
            if (totalDrained > 0)
            {
                Hits = Math.Min(Hits + totalDrained / 2, HitsMax);
                FixedParticles(0x376A, 9, 32, 5030, 1109, 0, EffectLayer.Waist);
            }
        }

        private void SummonBogCreatures()
        {
            if (Map == null || Map == Map.Internal)
                return;

            int count = m_Phase == 3 ? 3 : 2;

            Say("*The Matriarch calls forth creatures of the bog*");

            for (int i = 0; i < count; i++)
            {
                BaseCreature minion = new BogThing();
                minion.Name = "a swamp horror";
                minion.Hue = 1109;

                Point3D loc = GetSpawnPosition(4);
                if (loc != Point3D.Zero)
                {
                    minion.MoveToWorld(loc, Map);
                    minion.Combatant = Combatant;

                    Effects.SendLocationParticles(
                        EffectItem.Create(loc, Map, EffectItem.DefaultDuration),
                        0x374A, 10, 15, 1109, 0, 5021, 0);
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
            Say("*The Matriarch dissolves into the swamp*");
            PublicOverheadMessage(Network.MessageType.Regular, 0x497, false,
                "The swamp... is eternal...");

            Effects.SendLocationParticles(
                EffectItem.Create(Location, Map, EffectItem.DefaultDuration),
                0x376A, 9, 32, 1109, 0, 5030, 0);
            Effects.PlaySound(Location, Map, 0x1FB);

            base.OnDeath(c);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.40)
            {
                defender.ApplyPoison(this, m_Phase == 3 ? Poison.Deadly : Poison.Greater);
                defender.SendMessage(0x497, "The Matriarch's touch poisons you!");
            }
        }

        public CovenMatriarch(Serial serial) : base(serial)
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
            m_NextPoisonCloudTime = DateTime.UtcNow + TimeSpan.FromSeconds(15);
            m_NextCurseTime = DateTime.UtcNow + TimeSpan.FromSeconds(10);
            m_NextSummonTime = DateTime.UtcNow + TimeSpan.FromSeconds(30);
            m_NextLifeDrainTime = DateTime.UtcNow + TimeSpan.FromSeconds(20);
        }
    }
}
