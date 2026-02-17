/*
 * Artificer Class Pets
 * Mechanical constructs for the Artificer class
 *
 * Summon Types:
 * - Clockwork Spider (scout, fast)
 * - Clockwork Golem (tank, slow)
 * - Steam Turret (ranged, stationary)
 * - Clockwork Knight (balanced melee)
 * - Siege Automaton (heavy damage)
 */

using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Custom.VystiaClasses.Pets
{
    #region Clockwork Spider

    /// <summary>
    /// Summoned Clockwork Spider - Fast scout construct
    /// </summary>
    [CorpseName("a clockwork spider wreck")]
    public class SummonedClockworkSpider : VystiaSummonedPet
    {
        [Constructable]
        public SummonedClockworkSpider() : this(VystiaPetTier.Standard)
        {
        }

        public SummonedClockworkSpider(VystiaPetTier tier)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.1, 0.2)
        {
            Name = GetTierName(tier) + " Clockwork Spider";
            Body = 28; // Spider body
            BaseSoundID = 0x388;
            Hue = 2500; // Bronze/copper metallic

            PetType = VystiaPetType.MechanicalConstruct;
            PetTier = tier;

            // Spiders are fast but fragile
            int hp = (int)(VystiaPetStats.GetBaseHP(tier) * 0.7);
            var (minDam, maxDam) = VystiaPetStats.GetBaseDamage(tier);

            SetStr(80 + (int)tier * 15);
            SetDex(150 + (int)tier * 20);
            SetInt(50 + (int)tier * 10);

            SetHits(hp);
            SetDamage(minDam, maxDam);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 20, 35);
            SetResistance(ResistanceType.Cold, 20, 35);
            SetResistance(ResistanceType.Poison, 100); // Immune - mechanical
            SetResistance(ResistanceType.Energy, 40, 60);

            SetSkill(SkillName.MagicResist, 50.0 + (int)tier * 10);
            SetSkill(SkillName.Tactics, 70.0 + (int)tier * 10);
            SetSkill(SkillName.Wrestling, 70.0 + (int)tier * 10);

            Fame = 0;
            Karma = 0;

            ControlSlots = Math.Max(1, VystiaPetStats.GetControlSlots(tier) - 1); // Cheaper

            VirtualArmor = 25 + (int)tier * 5;
        }

        private string GetTierName(VystiaPetTier tier)
        {
            switch (tier)
            {
                case VystiaPetTier.Lesser: return "Small";
                case VystiaPetTier.Standard: return "";
                case VystiaPetTier.Greater: return "Large";
                case VystiaPetTier.Superior: return "Giant";
                case VystiaPetTier.Legendary: return "Titanic";
                default: return "";
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Mechanical clicking sound
            if (Utility.RandomDouble() < 0.02)
            {
                PlaySound(0x387);
            }
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            // Chance to inject oil (fire vulnerability)
            if (Utility.RandomDouble() < 0.15 + ((int)PetTier * 0.03))
            {
                defender.SendMessage("Hot oil sprays from the construct's fangs!");
                defender.FixedParticles(0x3709, 10, 15, 5021, 1161, 0, EffectLayer.Waist);
                // Apply minor fire damage
                AOS.Damage(defender, this, Utility.RandomMinMax(3, 8 + (int)PetTier * 2), 0, 100, 0, 0, 0);
            }
        }

        public SummonedClockworkSpider(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Clockwork Golem

    /// <summary>
    /// Summoned Clockwork Golem - Tank construct
    /// </summary>
    [CorpseName("a clockwork golem wreck")]
    public class SummonedClockwork : VystiaSummonedPet
    {
        [Constructable]
        public SummonedClockwork() : this(VystiaPetTier.Standard)
        {
        }

        public SummonedClockwork(VystiaPetTier tier)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.3, 0.5)
        {
            Name = GetTierName(tier) + " Clockwork Golem";
            Body = 752; // Golem body
            BaseSoundID = 541;
            Hue = 2419; // Steel gray

            PetType = VystiaPetType.MechanicalConstruct;
            PetTier = tier;

            // Golems are tanky but slow
            int hp = (int)(VystiaPetStats.GetBaseHP(tier) * 1.5);
            var (minDam, maxDam) = VystiaPetStats.GetBaseDamage(tier);

            SetStr(250 + (int)tier * 35);
            SetDex(50 + (int)tier * 5);
            SetInt(30 + (int)tier * 10);

            SetHits(hp);
            SetDamage(minDam, maxDam);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 50, 65);
            SetResistance(ResistanceType.Fire, 30, 45);
            SetResistance(ResistanceType.Cold, 30, 45);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 40, 55);

            SetSkill(SkillName.MagicResist, 60.0 + (int)tier * 10);
            SetSkill(SkillName.Tactics, 70.0 + (int)tier * 10);
            SetSkill(SkillName.Wrestling, 80.0 + (int)tier * 10);

            Fame = 0;
            Karma = 0;

            ControlSlots = VystiaPetStats.GetControlSlots(tier);

            VirtualArmor = 50 + (int)tier * 10;
        }

        private string GetTierName(VystiaPetTier tier)
        {
            switch (tier)
            {
                case VystiaPetTier.Lesser: return "Small";
                case VystiaPetTier.Standard: return "";
                case VystiaPetTier.Greater: return "Heavy";
                case VystiaPetTier.Superior: return "Siege";
                case VystiaPetTier.Legendary: return "Titan";
                default: return "";
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Steam venting
            if (Combatant != null && Utility.RandomDouble() < 0.03)
            {
                FixedParticles(0x3728, 1, 13, 5052, 2500, 0, EffectLayer.Waist);
                PlaySound(0x231);
            }
        }

        public override void OnDamagedBySpell(Mobile caster)
        {
            base.OnDamagedBySpell(caster);

            // Magic resistance - reflect some damage
            if (Utility.RandomDouble() < 0.12 + ((int)PetTier * 0.03))
            {
                int reflect = Utility.RandomMinMax(8, 20 + (int)PetTier * 5);
                AOS.Damage(caster, this, reflect, 0, 0, 0, 0, 100);
                caster.SendMessage("Energy arcs back from the construct's plating!");
                PlaySound(0x1F8);
            }
        }

        public SummonedClockwork(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Steam Turret

    /// <summary>
    /// Summoned Steam Turret - Stationary ranged attacker
    /// </summary>
    [CorpseName("a steam turret wreck")]
    public class SummonedSteamTurret : VystiaSummonedPet
    {
        private DateTime m_NextShot;

        [Constructable]
        public SummonedSteamTurret() : this(VystiaPetTier.Standard)
        {
        }

        public SummonedSteamTurret(VystiaPetTier tier)
            : base(AIType.AI_Archer, FightMode.Closest, 12, 8, 0.2, 0.4)
        {
            Name = GetTierName(tier) + " Steam Turret";
            Body = 0xED6; // Faction guildstone body
            BaseSoundID = 541;
            Hue = 2411; // Brass

            PetType = VystiaPetType.MechanicalConstruct;
            PetTier = tier;

            int hp = VystiaPetStats.GetBaseHP(tier);
            var (minDam, maxDam) = VystiaPetStats.GetBaseDamage(tier);
            // Turrets do bonus damage
            minDam = (int)(minDam * 1.3);
            maxDam = (int)(maxDam * 1.3);

            SetStr(100 + (int)tier * 15);
            SetDex(50 + (int)tier * 5);
            SetInt(50 + (int)tier * 10);

            SetHits(hp);
            SetDamage(minDam, maxDam);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Fire, 50);

            SetResistance(ResistanceType.Physical, 40, 55);
            SetResistance(ResistanceType.Fire, 50, 70);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 35, 50);

            SetSkill(SkillName.MagicResist, 50.0 + (int)tier * 10);
            SetSkill(SkillName.Tactics, 60.0 + (int)tier * 10);
            SetSkill(SkillName.Archery, 80.0 + (int)tier * 10);

            Fame = 0;
            Karma = 0;

            ControlSlots = VystiaPetStats.GetControlSlots(tier);

            VirtualArmor = 35 + (int)tier * 8;

            // Turrets don't move much
            CantWalk = false; // They can reposition but prefer not to

            m_NextShot = DateTime.UtcNow;
        }

        private string GetTierName(VystiaPetTier tier)
        {
            switch (tier)
            {
                case VystiaPetTier.Lesser: return "Light";
                case VystiaPetTier.Standard: return "";
                case VystiaPetTier.Greater: return "Heavy";
                case VystiaPetTier.Superior: return "Siege";
                case VystiaPetTier.Legendary: return "Artillery";
                default: return "";
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Ranged steam cannon attack
            if (Combatant != null && DateTime.UtcNow >= m_NextShot && InRange(Combatant, 10))
            {
                DoSteamShot();
                m_NextShot = DateTime.UtcNow + TimeSpan.FromSeconds(3.0 - ((int)PetTier * 0.3));
            }
        }

        private void DoSteamShot()
        {
            if (Combatant == null)
                return;

            Mobile target = Combatant as Mobile;
            if (target == null || !CanBeHarmful(target))
                return;

            DoHarmful(target);

            // Steam bolt visual
            MovingParticles(target, 0x36D4, 7, 0, false, true, 2500, 0, 9502, 0x100, 0, (EffectLayer)255, 0x100);
            PlaySound(0x11D);

            Timer.DelayCall(TimeSpan.FromSeconds(0.5), () =>
            {
                if (target != null && !target.Deleted)
                {
                    int damage = Utility.RandomMinMax(DamageMin, DamageMax);
                    AOS.Damage(target, this, damage, 50, 50, 0, 0, 0);
                    target.FixedParticles(0x3709, 10, 30, 5052, 2500, 0, EffectLayer.LeftFoot);
                }
            });
        }

        public SummonedSteamTurret(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_NextShot = DateTime.UtcNow;
        }
    }

    #endregion

    #region Clockwork Knight

    /// <summary>
    /// Summoned Clockwork Knight - Balanced melee construct
    /// </summary>
    [CorpseName("a clockwork knight wreck")]
    public class SummonedClockworkKnight : VystiaSummonedPet
    {
        [Constructable]
        public SummonedClockworkKnight() : this(VystiaPetTier.Standard)
        {
        }

        public SummonedClockworkKnight(VystiaPetTier tier)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = GetTierName(tier) + " Clockwork Knight";
            Body = 185; // Knight body
            BaseSoundID = 0x423;
            Hue = 2500; // Bronze

            PetType = VystiaPetType.ClockworkServant;
            PetTier = tier;

            int hp = VystiaPetStats.GetBaseHP(tier);
            var (minDam, maxDam) = VystiaPetStats.GetBaseDamage(tier);

            SetStr(180 + (int)tier * 25);
            SetDex(100 + (int)tier * 15);
            SetInt(50 + (int)tier * 10);

            SetHits(hp);
            SetDamage(minDam, maxDam);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 55);
            SetResistance(ResistanceType.Fire, 30, 45);
            SetResistance(ResistanceType.Cold, 30, 45);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 35, 50);

            SetSkill(SkillName.MagicResist, 60.0 + (int)tier * 10);
            SetSkill(SkillName.Tactics, 80.0 + (int)tier * 10);
            SetSkill(SkillName.Wrestling, 60.0 + (int)tier * 5);
            SetSkill(SkillName.Swords, 80.0 + (int)tier * 10);
            SetSkill(SkillName.Anatomy, 60.0 + (int)tier * 10);

            Fame = 0;
            Karma = 0;

            ControlSlots = VystiaPetStats.GetControlSlots(tier);

            VirtualArmor = 40 + (int)tier * 8;

            // Equip based on tier
            EquipKnight(tier);
        }

        private void EquipKnight(VystiaPetTier tier)
        {
            // All items should be non-movable and have bronze hue
            Item weapon;
            switch (tier)
            {
                case VystiaPetTier.Lesser:
                    weapon = new Longsword();
                    break;
                case VystiaPetTier.Standard:
                    weapon = new VikingSword();
                    AddItem(new MetalShield { Hue = 2500 });
                    break;
                case VystiaPetTier.Greater:
                    weapon = new Halberd();
                    break;
                case VystiaPetTier.Superior:
                    weapon = new Bardiche();
                    AddItem(new PlateChest { Hue = 2500 });
                    break;
                case VystiaPetTier.Legendary:
                default:
                    weapon = new Bardiche();
                    AddItem(new PlateChest { Hue = 2500 });
                    AddItem(new PlateLegs { Hue = 2500 });
                    AddItem(new PlateArms { Hue = 2500 });
                    AddItem(new PlateGloves { Hue = 2500 });
                    AddItem(new PlateHelm { Hue = 2500 });
                    break;
            }

            weapon.Hue = 2500;
            AddItem(weapon);
        }

        private string GetTierName(VystiaPetTier tier)
        {
            switch (tier)
            {
                case VystiaPetTier.Lesser: return "Apprentice";
                case VystiaPetTier.Standard: return "";
                case VystiaPetTier.Greater: return "Veteran";
                case VystiaPetTier.Superior: return "Champion";
                case VystiaPetTier.Legendary: return "Warforged";
                default: return "";
            }
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            // Precision strike - chance to ignore armor
            if (Utility.RandomDouble() < 0.10 + ((int)PetTier * 0.03))
            {
                int bonus = Utility.RandomMinMax(5, 10 + (int)PetTier * 3);
                AOS.Damage(defender, this, bonus, 100, 0, 0, 0, 0);
                defender.SendMessage("The construct's precision strike finds a gap in your defenses!");
                PlaySound(0x1F5);
            }
        }

        public SummonedClockworkKnight(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Siege Automaton

    /// <summary>
    /// Summoned Siege Automaton - Heavy damage construct
    /// </summary>
    [CorpseName("a siege automaton wreck")]
    public class SummonedSiegeAutomaton : VystiaSummonedPet
    {
        private DateTime m_NextSiegeCannon;

        [Constructable]
        public SummonedSiegeAutomaton() : this(VystiaPetTier.Greater)
        {
        }

        public SummonedSiegeAutomaton(VystiaPetTier tier)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.35, 0.5)
        {
            // Minimum tier is Greater
            if (tier < VystiaPetTier.Greater)
                tier = VystiaPetTier.Greater;

            Name = GetTierName(tier) + " Siege Automaton";
            Body = 0x2198; // Cannon body ID
            BaseSoundID = 541;
            Hue = 2406; // Dark iron

            PetType = VystiaPetType.MechanicalConstruct;
            PetTier = tier;

            // Siege Automatons are heavy hitters
            int hp = (int)(VystiaPetStats.GetBaseHP(tier) * 1.3);
            var (minDam, maxDam) = VystiaPetStats.GetBaseDamage(tier);
            minDam = (int)(minDam * 1.5);
            maxDam = (int)(maxDam * 1.5);

            SetStr(300 + (int)tier * 40);
            SetDex(40 + (int)tier * 5);
            SetInt(80 + (int)tier * 15);

            SetHits(hp);
            SetDamage(minDam, maxDam);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Fire, 40);

            SetResistance(ResistanceType.Physical, 55, 70);
            SetResistance(ResistanceType.Fire, 40, 60);
            SetResistance(ResistanceType.Cold, 25, 40);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 40, 55);

            SetSkill(SkillName.MagicResist, 70.0 + (int)tier * 10);
            SetSkill(SkillName.Tactics, 90.0 + (int)tier * 10);
            SetSkill(SkillName.Wrestling, 90.0 + (int)tier * 10);

            Fame = 0;
            Karma = 0;

            ControlSlots = VystiaPetStats.GetControlSlots(tier);

            VirtualArmor = 60 + (int)tier * 12;

            m_NextSiegeCannon = DateTime.UtcNow + TimeSpan.FromSeconds(5);
        }

        private string GetTierName(VystiaPetTier tier)
        {
            switch (tier)
            {
                case VystiaPetTier.Greater: return "";
                case VystiaPetTier.Superior: return "Heavy";
                case VystiaPetTier.Legendary: return "Dreadnought";
                default: return "";
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Siege Cannon ability
            if (Combatant != null && DateTime.UtcNow >= m_NextSiegeCannon)
            {
                DoSiegeCannon();
                m_NextSiegeCannon = DateTime.UtcNow + TimeSpan.FromSeconds(20 - (int)PetTier * 2);
            }
        }

        private void DoSiegeCannon()
        {
            if (Combatant == null)
                return;

            Mobile target = Combatant as Mobile;
            if (target == null || !CanBeHarmful(target))
                return;

            Emote("*the siege automaton's cannon charges*");
            PlaySound(0x2F4);
            FixedParticles(0x376A, 1, 14, 5045, 2406, 0, EffectLayer.Waist);

            Timer.DelayCall(TimeSpan.FromSeconds(1.5), () =>
            {
                if (target == null || target.Deleted || !CanBeHarmful(target))
                    return;

                DoHarmful(target);

                // AoE cannon blast
                int range = 2 + (int)PetTier / 2;
                int damage = 25 + (int)PetTier * 15;

                Effects.SendLocationEffect(target.Location, target.Map, 0x36BD, 20, 10, 2406, 0);
                target.PlaySound(0x307);

                foreach (Mobile m in target.GetMobilesInRange(range))
                {
                    if (m != this && m != ControlMaster && m != SummonMaster && CanBeHarmful(m))
                    {
                        DoHarmful(m);
                        int actualDamage = m == target ? damage : damage / 2;
                        AOS.Damage(m, this, actualDamage, 60, 40, 0, 0, 0);
                        m.FixedParticles(0x3709, 10, 30, 5052, 2406, 0, EffectLayer.LeftFoot);
                    }
                }
            });
        }

        public SummonedSiegeAutomaton(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_NextSiegeCannon = DateTime.UtcNow + TimeSpan.FromSeconds(5);
        }
    }

    #endregion
}
