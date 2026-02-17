// Vystia Engineering Constructs - Craftable summoning items for Artificer
// Creates mechanical pets when activated

using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Custom.VystiaClasses.Pets;
using Server.Targeting;

namespace Server.Items.Vystia
{
    #region Base Construct Item

    /// <summary>
    /// Base class for craftable construct items that summon mechanical pets
    /// </summary>
    public abstract class BaseConstructItem : Item
    {
        public abstract string ConstructName { get; }
        public abstract int ControlSlots { get; }
        public abstract int Charges { get; }
        public abstract int SummonDuration { get; } // seconds, 0 = permanent until death
        public abstract double RequiredEngineering { get; }

        private int m_ChargesRemaining;
        private BaseCreature m_ActiveConstruct;

        [CommandProperty(AccessLevel.GameMaster)]
        public int ChargesRemaining
        {
            get { return m_ChargesRemaining; }
            set { m_ChargesRemaining = value; InvalidateProperties(); }
        }

        public BaseConstructItem(int itemID) : base(itemID)
        {
            Weight = 5.0;
            m_ChargesRemaining = Charges;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1070722, $"Summons: {ConstructName}");
            list.Add(1060584, m_ChargesRemaining.ToString()); // uses remaining: ~1_val~
            list.Add(1060662, $"Control Slots\t{ControlSlots}");
            if (SummonDuration > 0)
                list.Add(1042971, $"Duration: {SummonDuration / 60}min");
            else
                list.Add(1042971, "Permanent until destroyed");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // Must be in backpack
                return;
            }

            if (!(from is PlayerMobile pm))
            {
                from.SendMessage("Only players can use this.");
                return;
            }

            // Check Engineering skill
            if (from.Skills[SkillName.Engineering].Base < RequiredEngineering)
            {
                from.SendMessage($"You need at least {RequiredEngineering} Engineering skill to activate this construct.");
                return;
            }

            // Check charges
            if (m_ChargesRemaining <= 0)
            {
                from.SendMessage("This construct core is depleted.");
                return;
            }

            // Check follower slots
            if ((from.Followers + ControlSlots) > from.FollowersMax)
            {
                from.SendMessage($"You have insufficient control slots. This construct requires {ControlSlots} slots.");
                return;
            }

            // Check if we already have an active construct from this item
            if (m_ActiveConstruct != null && !m_ActiveConstruct.Deleted)
            {
                from.SendMessage("You already have this construct active. Dismiss it first.");
                return;
            }

            // Summon the construct
            BaseCreature construct = CreateConstruct(pm);
            if (construct != null)
            {
                construct.MoveToWorld(from.Location, from.Map);
                construct.ControlMaster = pm;
                construct.Controlled = true;
                construct.ControlOrder = OrderType.Follow;
                construct.ControlTarget = pm;

                m_ActiveConstruct = construct;
                m_ChargesRemaining--;

                // Set up duration timer if applicable
                if (SummonDuration > 0)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(SummonDuration), () =>
                    {
                        if (construct != null && !construct.Deleted)
                        {
                            construct.Say("*systems shutting down*");
                            construct.PlaySound(0x1F8);
                            construct.Delete();
                        }
                    });
                }

                from.PlaySound(0x22F); // Mechanical whirring
                from.FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);
                from.SendMessage($"You activate the {ConstructName}!");

                InvalidateProperties();
            }
        }

        protected abstract BaseCreature CreateConstruct(PlayerMobile owner);

        public BaseConstructItem(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_ChargesRemaining);
            writer.Write(m_ActiveConstruct);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
            m_ChargesRemaining = reader.ReadInt();
            m_ActiveConstruct = reader.ReadMobile() as BaseCreature;
        }
    }

    #endregion

    #region Construct Items

    /// <summary>
    /// Clockwork Spider Core - Summons a fast scouting spider (50 HP, 1 slot)
    /// </summary>
    public class ClockworkSpiderCore : BaseConstructItem
    {
        public override string ConstructName => "Clockwork Spider";
        public override int ControlSlots => 1;
        public override int Charges => 5;
        public override int SummonDuration => 1800; // 30 minutes
        public override double RequiredEngineering => 40.0;

        [Constructable]
        public ClockworkSpiderCore() : base(0x1EB9) // Clockwork assembly
        {
            Name = "Clockwork Spider Core";
            Hue = 2500; // Bronze
        }

        protected override BaseCreature CreateConstruct(PlayerMobile owner)
        {
            return new SummonedClockworkSpider(VystiaPetTier.Standard);
        }

        public ClockworkSpiderCore(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Repair Drone Core - Summons a construct that heals other constructs (1 slot)
    /// </summary>
    public class RepairDroneCore : BaseConstructItem
    {
        public override string ConstructName => "Repair Drone";
        public override int ControlSlots => 1;
        public override int Charges => 3;
        public override int SummonDuration => 1200; // 20 minutes
        public override double RequiredEngineering => 55.0;

        [Constructable]
        public RepairDroneCore() : base(0x1EB9)
        {
            Name = "Repair Drone Core";
            Hue = 1153; // White/silver
        }

        protected override BaseCreature CreateConstruct(PlayerMobile owner)
        {
            return new SummonedRepairDrone();
        }

        public RepairDroneCore(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Steam Turret Core - Summons a stationary ranged turret (100 HP, 2 slots)
    /// </summary>
    public class SteamTurretCore : BaseConstructItem
    {
        public override string ConstructName => "Steam Turret";
        public override int ControlSlots => 2;
        public override int Charges => 3;
        public override int SummonDuration => 900; // 15 minutes
        public override double RequiredEngineering => 65.0;

        [Constructable]
        public SteamTurretCore() : base(0x1EB9)
        {
            Name = "Steam Turret Core";
            Hue = 2305; // Ironclad metallic
        }

        protected override BaseCreature CreateConstruct(PlayerMobile owner)
        {
            return new SummonedSteamTurret();
        }

        public SteamTurretCore(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Iron Golem Core - Summons a massive tank construct (500 HP, 3 slots)
    /// </summary>
    public class IronGolemCore : BaseConstructItem
    {
        public override string ConstructName => "Iron Golem";
        public override int ControlSlots => 3;
        public override int Charges => 2;
        public override int SummonDuration => 0; // Permanent until death
        public override double RequiredEngineering => 80.0;

        [Constructable]
        public IronGolemCore() : base(0x1EB9)
        {
            Name = "Iron Golem Core";
            Hue = 2401; // Dark iron
        }

        protected override BaseCreature CreateConstruct(PlayerMobile owner)
        {
            return new SummonedIronGolem();
        }

        public IronGolemCore(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Siege Engine Core - Summons a massive siege construct (Territory warfare, 5 slots)
    /// </summary>
    public class SiegeEngineCore : BaseConstructItem
    {
        public override string ConstructName => "Siege Engine";
        public override int ControlSlots => 5;
        public override int Charges => 1;
        public override int SummonDuration => 0; // Permanent until death
        public override double RequiredEngineering => 95.0;

        [Constructable]
        public SiegeEngineCore() : base(0x1EB9)
        {
            Name = "Siege Engine Core";
            Hue = 1175; // Dark steel
        }

        protected override BaseCreature CreateConstruct(PlayerMobile owner)
        {
            return new SummonedSiegeEngine();
        }

        public SiegeEngineCore(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Construct Creatures

    /// <summary>
    /// Repair Drone - Heals other mechanical constructs
    /// </summary>
    [CorpseName("a repair drone wreck")]
    public class SummonedRepairDrone : BaseCreature
    {
        private DateTime m_NextHeal;

        [Constructable]
        public SummonedRepairDrone()
            : base(AIType.AI_Healer, FightMode.None, 10, 1, 0.2, 0.4)
        {
            Name = "Repair Drone";
            Body = 723; // Small mechanical body
            BaseSoundID = 0x387;
            Hue = 1153; // White/silver

            SetStr(50);
            SetDex(80);
            SetInt(100);

            SetHits(75);
            SetDamage(1, 3);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.Healing, 80.0);
            SetSkill(SkillName.MagicResist, 50.0);

            Fame = 0;
            Karma = 0;
            ControlSlots = 1;
            VirtualArmor = 20;
        }

        public override void OnThink()
        {
            base.OnThink();

            if (DateTime.UtcNow > m_NextHeal && ControlMaster != null)
            {
                // Look for damaged constructs to heal
                foreach (Mobile m in GetMobilesInRange(8))
                {
                    if (m is BaseCreature bc && bc.Controlled && bc.ControlMaster == ControlMaster)
                    {
                        if (bc.Hits < bc.HitsMax * 0.8 && bc != this)
                        {
                            int healAmount = Utility.RandomMinMax(15, 25);
                            bc.Hits = Math.Min(bc.Hits + healAmount, bc.HitsMax);
                            bc.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
                            bc.PlaySound(0x1F2);
                            this.Say("*repairs damage*");
                            m_NextHeal = DateTime.UtcNow + TimeSpan.FromSeconds(5);
                            break;
                        }
                    }
                }
            }
        }

        public SummonedRepairDrone(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    /// <summary>
    /// Steam Turret - Stationary ranged construct
    /// </summary>
    [CorpseName("a steam turret wreck")]
    public class SummonedSteamTurret : BaseCreature
    {
        private DateTime m_NextShot;

        [Constructable]
        public SummonedSteamTurret()
            : base(AIType.AI_Archer, FightMode.Closest, 12, 8, 0.2, 0.4)
        {
            Name = "Steam Turret";
            Body = 0xED6; // Faction guildstone body
            BaseSoundID = 541;
            Hue = 2305; // Ironclad

            SetStr(80);
            SetDex(40);
            SetInt(50);

            SetHits(100);
            SetDamage(12, 18);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Fire, 50);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.MagicResist, 60.0);
            SetSkill(SkillName.Tactics, 70.0);
            SetSkill(SkillName.Archery, 80.0);

            Fame = 0;
            Karma = 0;
            ControlSlots = 2;
            VirtualArmor = 40;

            // Turrets don't move
            CantWalk = true;
        }

        public override void OnThink()
        {
            base.OnThink();

            if (DateTime.UtcNow > m_NextShot && Combatant != null && Combatant is Mobile target)
            {
                if (InRange(target, 12) && CanSee(target))
                {
                    // Steam cannon attack
                    MovingParticles(target, 0x36D4, 7, 0, false, true, 2305, 0, 9502, 4019, 0x160, 0);
                    PlaySound(0x15E);

                    Timer.DelayCall(TimeSpan.FromSeconds(0.5), () =>
                    {
                        if (target != null && !target.Deleted && !Deleted)
                        {
                            AOS.Damage(target, this, Utility.RandomMinMax(15, 25), 50, 50, 0, 0, 0);
                        }
                    });

                    m_NextShot = DateTime.UtcNow + TimeSpan.FromSeconds(3);
                }
            }
        }

        public SummonedSteamTurret(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    /// <summary>
    /// Iron Golem - Massive tank construct
    /// </summary>
    [CorpseName("an iron golem wreck")]
    public class SummonedIronGolem : BaseCreature
    {
        [Constructable]
        public SummonedIronGolem()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.3, 0.5)
        {
            Name = "Iron Golem";
            Body = 752; // Golem body
            BaseSoundID = 541;
            Hue = 2401; // Dark iron

            SetStr(200);
            SetDex(60);
            SetInt(30);

            SetHits(500);
            SetDamage(18, 28);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 70, 80);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 80.0);
            SetSkill(SkillName.Tactics, 90.0);
            SetSkill(SkillName.Wrestling, 90.0);

            Fame = 0;
            Karma = 0;
            ControlSlots = 3;
            VirtualArmor = 60;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            // Chance to stun
            if (Utility.RandomDouble() < 0.15)
            {
                defender.Freeze(TimeSpan.FromSeconds(2));
                defender.SendMessage("The iron golem's massive strike stuns you!");
                defender.PlaySound(0x22C);
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            // Defensive shockwave when critically damaged
            if (Hits < HitsMax * 0.25 && Utility.RandomDouble() < 0.20)
            {
                PublicOverheadMessage(Network.MessageType.Emote, 0, false, "*releases defensive shockwave*");
                PlaySound(0x2F3);

                foreach (Mobile m in GetMobilesInRange(3))
                {
                    if (m != this && m != ControlMaster && m is Mobile target)
                    {
                        if (target.Alive && !target.IsDeadBondedPet)
                        {
                            target.FixedParticles(0x3709, 10, 15, 5021, EffectLayer.Waist);
                            AOS.Damage(target, this, Utility.RandomMinMax(10, 20), 100, 0, 0, 0, 0);
                        }
                    }
                }
            }
        }

        public SummonedIronGolem(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    /// <summary>
    /// Siege Engine - Massive siege construct for territory warfare
    /// </summary>
    [CorpseName("a siege engine wreck")]
    public class SummonedSiegeEngine : BaseCreature
    {
        private DateTime m_NextSiegeShot;

        [Constructable]
        public SummonedSiegeEngine()
            : base(AIType.AI_Melee, FightMode.Closest, 15, 10, 0.4, 0.6)
        {
            Name = "Siege Engine";
            Body = 0x2198; // Cannon body ID
            BaseSoundID = 541;
            Hue = 1175; // Dark steel

            SetStr(300);
            SetDex(30);
            SetInt(50);

            SetHits(800);
            SetDamage(25, 40);

            SetDamageType(ResistanceType.Physical, 70);
            SetDamageType(ResistanceType.Fire, 30);

            SetResistance(ResistanceType.Physical, 75, 85);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.MagicResist, 70.0);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.Wrestling, 100.0);

            Fame = 0;
            Karma = 0;
            ControlSlots = 5;
            VirtualArmor = 80;

            // Very slow movement
            ActiveSpeed = 0.6;
            PassiveSpeed = 0.8;
        }

        public override void OnThink()
        {
            base.OnThink();

            // Siege cannon - long range AOE attack
            if (DateTime.UtcNow > m_NextSiegeShot && Combatant != null && Combatant is Mobile target)
            {
                if (InRange(target, 15))
                {
                    PublicOverheadMessage(Network.MessageType.Emote, 0, false, "*SIEGE CANNON FIRING*");
                    PlaySound(0x2F);

                    // Create explosion at target location
                    Point3D loc = target.Location;
                    Effects.SendLocationEffect(loc, Map, 0x36B0, 20, 10, 2305, 0);
                    Effects.PlaySound(loc, Map, 0x307);

                    // Damage all in area
                    foreach (Mobile m in Map.GetMobilesInRange(loc, 3))
                    {
                        if (m != this && m != ControlMaster && m.Alive)
                        {
                            if (m is BaseCreature bc && bc.Controlled && bc.ControlMaster == ControlMaster)
                                continue; // Don't hit friendly constructs

                            AOS.Damage(m, this, Utility.RandomMinMax(30, 50), 70, 30, 0, 0, 0);
                        }
                    }

                    m_NextSiegeShot = DateTime.UtcNow + TimeSpan.FromSeconds(8);
                }
            }
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            // Crushing blow
            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage("The siege engine's crushing blow deals massive damage!");
                AOS.Damage(defender, this, Utility.RandomMinMax(20, 35), 100, 0, 0, 0, 0);
                defender.PlaySound(0x22C);
            }
        }

        public SummonedSiegeEngine(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    #endregion
}
