// Vystia Camping System
// Allows players to set up camps for rest, regeneration, and safety

using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Commands;

namespace Server.Custom.VystiaClasses.Systems
{
    #region Camp Types

    /// <summary>
    /// Types of camps
    /// </summary>
    public enum CampType
    {
        Campfire,       // Basic campfire - regen bonus
        SafeCamp,       // Protected camp - monster aggro reduction
        Outpost         // Full outpost - vendor access, storage
    }

    #endregion

    #region Campfire Item

    /// <summary>
    /// Basic campfire - provides regeneration bonuses
    /// </summary>
    public class VystiaCampfire : Item
    {
        private PlayerMobile m_Owner;
        private DateTime m_Created;
        private Timer m_DecayTimer;
        private Timer m_RegenTimer;
        private List<Mobile> m_CampingMobiles;

        private static readonly TimeSpan CampfireDuration = TimeSpan.FromMinutes(30);
        private static readonly int CampfireRange = 5;

        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerMobile Owner => m_Owner;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime Created => m_Created;

        [Constructable]
        public VystiaCampfire() : base(0xDE3) // Campfire graphic
        {
            Name = "a campfire";
            Movable = false;
            Light = LightType.Circle300;
            m_CampingMobiles = new List<Mobile>();
        }

        public void Setup(PlayerMobile owner)
        {
            m_Owner = owner;
            m_Created = DateTime.UtcNow;

            // Start decay timer
            m_DecayTimer = Timer.DelayCall(CampfireDuration, Delete);

            // Start regen timer
            m_RegenTimer = Timer.DelayCall(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), OnRegenTick);

            // Play setup sound
            Effects.PlaySound(Location, Map, 0x4B9);
        }

        private void OnRegenTick()
        {
            if (Deleted || Map == null || Map == Map.Internal)
                return;

            foreach (Mobile m in GetMobilesInRange(CampfireRange))
            {
                if (m is PlayerMobile pm && pm.Alive)
                {
                    // Grant regen bonus
                    int hpRegen = Math.Max(1, pm.HitsMax / 50); // 2% per tick
                    int stamRegen = Math.Max(1, pm.StamMax / 50);
                    int manaRegen = Math.Max(1, pm.ManaMax / 50);

                    if (pm.Hits < pm.HitsMax)
                        pm.Hits = Math.Min(pm.HitsMax, pm.Hits + hpRegen);
                    if (pm.Stam < pm.StamMax)
                        pm.Stam = Math.Min(pm.StamMax, pm.Stam + stamRegen);
                    if (pm.Mana < pm.ManaMax)
                        pm.Mana = Math.Min(pm.ManaMax, pm.Mana + manaRegen);

                    // Check if just entered camp
                    if (!m_CampingMobiles.Contains(pm))
                    {
                        m_CampingMobiles.Add(pm);
                        pm.SendMessage(0x35, "You rest by the campfire and feel your strength returning.");
                    }
                }
            }

            // Clean up mobiles that left
            for (int i = m_CampingMobiles.Count - 1; i >= 0; i--)
            {
                if (!m_CampingMobiles[i].InRange(Location, CampfireRange))
                {
                    m_CampingMobiles[i].SendMessage("You leave the warmth of the campfire.");
                    m_CampingMobiles.RemoveAt(i);
                }
            }

            // Visual effect
            Effects.SendLocationParticles(EffectItem.Create(Location, Map, TimeSpan.FromSeconds(1)), 0x3709, 10, 15, 0, 0, 5029, 0);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from is PlayerMobile pm)
            {
                if (!from.InRange(Location, 2))
                {
                    pm.SendMessage("You are too far away to use the campfire.");
                    return;
                }

                TimeSpan remaining = CampfireDuration - (DateTime.UtcNow - m_Created);
                pm.SendMessage(0x35, "This campfire will burn for {0:F0} more minutes.", remaining.TotalMinutes);
                pm.SendMessage(0x35, "Resting nearby restores 2% HP/Stam/Mana every 5 seconds.");
            }
        }

        public override void OnDelete()
        {
            m_DecayTimer?.Stop();
            m_RegenTimer?.Stop();

            foreach (var m in m_CampingMobiles)
            {
                if (m is PlayerMobile pm)
                    pm.SendMessage("The campfire has burned out.");
            }

            base.OnDelete();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            if (m_Owner != null)
                list.Add(1060658, "Owner\t{0}", m_Owner.Name);

            TimeSpan remaining = CampfireDuration - (DateTime.UtcNow - m_Created);
            list.Add("Burns for {0:F0} more minutes", remaining.TotalMinutes);
            list.Add("Restores HP/Stam/Mana while resting");
        }

        public VystiaCampfire(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write(m_Owner);
            writer.Write(m_Created);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
            m_Owner = reader.ReadMobile() as PlayerMobile;
            m_Created = reader.ReadDateTime();
            m_CampingMobiles = new List<Mobile>();

            // Restart timers
            TimeSpan remaining = CampfireDuration - (DateTime.UtcNow - m_Created);
            if (remaining > TimeSpan.Zero)
            {
                m_DecayTimer = Timer.DelayCall(remaining, Delete);
                m_RegenTimer = Timer.DelayCall(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), OnRegenTick);
            }
            else
            {
                Delete();
            }
        }
    }

    #endregion

    #region Safe Camp Item

    /// <summary>
    /// Safe camp - reduces monster aggro and provides protection
    /// </summary>
    public class VystiaSafeCamp : Item
    {
        private PlayerMobile m_Owner;
        private DateTime m_Created;
        private Timer m_DecayTimer;
        private Timer m_ProtectionTimer;

        private static readonly TimeSpan SafeCampDuration = TimeSpan.FromHours(1);
        private static readonly int SafeCampRange = 8;

        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerMobile Owner => m_Owner;

        [Constructable]
        public VystiaSafeCamp() : base(0xA5A) // Tent graphic
        {
            Name = "a safe camp";
            Movable = false;
        }

        public void Setup(PlayerMobile owner)
        {
            m_Owner = owner;
            m_Created = DateTime.UtcNow;

            // Add bedroll and campfire
            var campfire = new VystiaCampfire();
            campfire.MoveToWorld(new Point3D(X + 1, Y, Z), Map);
            campfire.Setup(owner);

            // Start decay timer
            m_DecayTimer = Timer.DelayCall(SafeCampDuration, Delete);

            // Start protection timer
            m_ProtectionTimer = Timer.DelayCall(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3), OnProtectionTick);

            owner.SendMessage(0x35, "You set up a safe camp. Monsters will be less likely to attack you here.");
        }

        private void OnProtectionTick()
        {
            if (Deleted || Map == null)
                return;

            // Reduce aggro for nearby creatures
            foreach (Mobile m in GetMobilesInRange(SafeCampRange + 5))
            {
                if (m is BaseCreature bc && !bc.Controlled && bc.Combatant != null)
                {
                    // Check if combatant is in camp
                    if (bc.Combatant is PlayerMobile pm && pm.InRange(Location, SafeCampRange))
                    {
                        // 30% chance to lose aggro each tick
                        if (Utility.RandomDouble() < 0.30)
                        {
                            bc.Combatant = null;
                            bc.FocusMob = null;
                        }
                    }
                }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from is PlayerMobile pm)
            {
                if (!from.InRange(Location, 2))
                {
                    pm.SendMessage("You are too far away.");
                    return;
                }

                TimeSpan remaining = SafeCampDuration - (DateTime.UtcNow - m_Created);
                pm.SendMessage(0x35, "This safe camp will last for {0:F0} more minutes.", remaining.TotalMinutes);
                pm.SendMessage(0x35, "Monsters are less likely to attack you here.");
            }
        }

        public override void OnDelete()
        {
            m_DecayTimer?.Stop();
            m_ProtectionTimer?.Stop();
            base.OnDelete();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            if (m_Owner != null)
                list.Add(1060658, "Owner\t{0}", m_Owner.Name);
            list.Add("Reduces monster aggro");
            list.Add("Includes campfire benefits");
        }

        public VystiaSafeCamp(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write(m_Owner);
            writer.Write(m_Created);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
            m_Owner = reader.ReadMobile() as PlayerMobile;
            m_Created = reader.ReadDateTime();

            TimeSpan remaining = SafeCampDuration - (DateTime.UtcNow - m_Created);
            if (remaining > TimeSpan.Zero)
            {
                m_DecayTimer = Timer.DelayCall(remaining, Delete);
                m_ProtectionTimer = Timer.DelayCall(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3), OnProtectionTick);
            }
            else
            {
                Delete();
            }
        }
    }

    #endregion

    #region Outpost Item

    /// <summary>
    /// Outpost - full camp with vendor access and storage
    /// </summary>
    public class VystiaOutpost : Item
    {
        private PlayerMobile m_Owner;
        private DateTime m_Created;
        private Timer m_DecayTimer;
        private Container m_Storage;

        private static readonly TimeSpan OutpostDuration = TimeSpan.FromHours(2);

        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerMobile Owner => m_Owner;

        [CommandProperty(AccessLevel.GameMaster)]
        public Container Storage => m_Storage;

        [Constructable]
        public VystiaOutpost() : base(0x1F13) // Tent/structure graphic
        {
            Name = "an outpost";
            Movable = false;
        }

        public void Setup(PlayerMobile owner)
        {
            m_Owner = owner;
            m_Created = DateTime.UtcNow;

            // Add safe camp
            var safeCamp = new VystiaSafeCamp();
            safeCamp.MoveToWorld(new Point3D(X + 2, Y + 2, Z), Map);
            safeCamp.Setup(owner);

            // Add storage chest
            m_Storage = new WoodenChest();
            m_Storage.Name = "outpost storage";
            m_Storage.Movable = false;
            m_Storage.MoveToWorld(new Point3D(X - 1, Y, Z), Map);

            // Start decay timer
            m_DecayTimer = Timer.DelayCall(OutpostDuration, Delete);

            owner.SendMessage(0x35, "You establish an outpost. You have a storage chest available for 2 hours.");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from is PlayerMobile pm)
            {
                if (!from.InRange(Location, 3))
                {
                    pm.SendMessage("You are too far away.");
                    return;
                }

                TimeSpan remaining = OutpostDuration - (DateTime.UtcNow - m_Created);
                pm.SendMessage(0x35, "This outpost will last for {0:F0} more minutes.", remaining.TotalMinutes);
                pm.SendMessage(0x35, "Storage chest, safe camp, and campfire are available.");

                if (m_Storage != null && !m_Storage.Deleted)
                {
                    pm.SendMessage("Double-click the storage chest to access your supplies.");
                }
            }
        }

        public override void OnDelete()
        {
            m_DecayTimer?.Stop();

            // Move items from storage to owner's bank
            if (m_Storage != null && !m_Storage.Deleted && m_Owner != null)
            {
                Container bank = m_Owner.BankBox;
                if (bank != null)
                {
                    foreach (Item item in new List<Item>(m_Storage.Items))
                    {
                        bank.DropItem(item);
                    }
                    m_Owner.SendMessage("Items from your outpost storage have been moved to your bank.");
                }
                m_Storage.Delete();
            }

            base.OnDelete();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            if (m_Owner != null)
                list.Add(1060658, "Owner\t{0}", m_Owner.Name);
            list.Add("Includes storage, safe camp, and campfire");
        }

        public VystiaOutpost(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write(m_Owner);
            writer.Write(m_Created);
            writer.Write(m_Storage);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
            m_Owner = reader.ReadMobile() as PlayerMobile;
            m_Created = reader.ReadDateTime();
            m_Storage = reader.ReadItem() as Container;

            TimeSpan remaining = OutpostDuration - (DateTime.UtcNow - m_Created);
            if (remaining > TimeSpan.Zero)
            {
                m_DecayTimer = Timer.DelayCall(remaining, Delete);
            }
            else
            {
                Delete();
            }
        }
    }

    #endregion

    #region Camping Kit Item

    /// <summary>
    /// Camping kit - consumable item to set up camps
    /// </summary>
    public class VystiaCampingKit : Item
    {
        private CampType m_CampType;
        private int m_UsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public CampType CampType
        {
            get { return m_CampType; }
            set { m_CampType = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; InvalidateProperties(); }
        }

        [Constructable]
        public VystiaCampingKit() : this(CampType.Campfire, 5) { }

        [Constructable]
        public VystiaCampingKit(CampType type) : this(type, 5) { }

        [Constructable]
        public VystiaCampingKit(CampType type, int uses) : base(0x1C9C) // Bedroll graphic
        {
            m_CampType = type;
            m_UsesRemaining = uses;
            UpdateName();
            Weight = 5.0;
        }

        private void UpdateName()
        {
            switch (m_CampType)
            {
                case CampType.Campfire:
                    Name = "campfire kit";
                    Hue = 0;
                    break;
                case CampType.SafeCamp:
                    Name = "safe camp kit";
                    Hue = 2010;
                    break;
                case CampType.Outpost:
                    Name = "outpost kit";
                    Hue = 2305;
                    break;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!(from is PlayerMobile pm))
                return;

            if (!IsChildOf(pm.Backpack))
            {
                pm.SendLocalizedMessage(1042001); // Must be in pack
                return;
            }

            if (m_UsesRemaining <= 0)
            {
                pm.SendMessage(0x22, "This kit is empty.");
                Delete();
                return;
            }

            // Check camping skill requirement
            double requiredSkill = 0;
            switch (m_CampType)
            {
                case CampType.Campfire:
                    requiredSkill = 0;
                    break;
                case CampType.SafeCamp:
                    requiredSkill = 50;
                    break;
                case CampType.Outpost:
                    requiredSkill = 80;
                    break;
            }

            if (pm.Skills[SkillName.Camping].Value < requiredSkill)
            {
                pm.SendMessage(0x22, "You need {0} camping skill to set up this type of camp.", requiredSkill);
                return;
            }

            // Check for existing camps nearby
            foreach (Item item in pm.GetItemsInRange(15))
            {
                if (item is VystiaCampfire || item is VystiaSafeCamp || item is VystiaOutpost)
                {
                    pm.SendMessage(0x22, "There is already a camp nearby.");
                    return;
                }
            }

            pm.SendMessage("Where do you want to set up camp?");
            pm.Target = new SetupCampTarget(this);
        }

        public void SetupCamp(PlayerMobile pm, Point3D location)
        {
            if (m_UsesRemaining <= 0)
                return;

            // Check if location is valid
            if (!pm.Map.CanFit(location, 16, true, true))
            {
                pm.SendMessage(0x22, "You cannot set up camp there.");
                return;
            }

            Item camp = null;

            switch (m_CampType)
            {
                case CampType.Campfire:
                    var campfire = new VystiaCampfire();
                    campfire.MoveToWorld(location, pm.Map);
                    campfire.Setup(pm);
                    camp = campfire;
                    break;

                case CampType.SafeCamp:
                    var safeCamp = new VystiaSafeCamp();
                    safeCamp.MoveToWorld(location, pm.Map);
                    safeCamp.Setup(pm);
                    camp = safeCamp;
                    break;

                case CampType.Outpost:
                    var outpost = new VystiaOutpost();
                    outpost.MoveToWorld(location, pm.Map);
                    outpost.Setup(pm);
                    camp = outpost;
                    break;
            }

            if (camp != null)
            {
                m_UsesRemaining--;

                // Gain camping skill
                pm.CheckSkill(SkillName.Camping, 0, 100);

                if (m_UsesRemaining <= 0)
                {
                    pm.SendMessage("Your camping kit is now empty.");
                    Delete();
                }
            }
        }

        private class SetupCampTarget : Target
        {
            private VystiaCampingKit m_Kit;

            public SetupCampTarget(VystiaCampingKit kit) : base(10, true, TargetFlags.None)
            {
                m_Kit = kit;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!(from is PlayerMobile pm) || m_Kit == null || m_Kit.Deleted)
                    return;

                IPoint3D p = targeted as IPoint3D;
                if (p == null)
                    return;

                m_Kit.SetupCamp(pm, new Point3D(p));
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Camp Type\t{0}", m_CampType.ToString());
            list.Add(1060659, "Uses\t{0}", m_UsesRemaining);
        }

        public VystiaCampingKit(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write((int)m_CampType);
            writer.Write(m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
            m_CampType = (CampType)reader.ReadInt();
            m_UsesRemaining = reader.ReadInt();
        }
    }

    #endregion

    #region Camping Commands

    /// <summary>
    /// GM commands for camping system
    /// </summary>
    public static class CampingCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("MakeCamp", AccessLevel.Player, new CommandEventHandler(MakeCamp_OnCommand));
            CommandSystem.Register("MC", AccessLevel.Player, new CommandEventHandler(MakeCamp_OnCommand));
            CommandSystem.Register("GiveCampKit", AccessLevel.GameMaster, new CommandEventHandler(GiveCampKit_OnCommand));

            Console.WriteLine("[Vystia] Camping system commands registered.");
        }

        /// <summary>
        /// [MakeCamp - Set up a basic campfire (requires kindling)
        /// </summary>
        [Usage("MakeCamp")]
        [Description("Set up a basic campfire at your location.")]
        private static void MakeCamp_OnCommand(CommandEventArgs e)
        {
            if (!(e.Mobile is PlayerMobile pm))
                return;

            // Check for kindling in backpack
            Item kindling = pm.Backpack?.FindItemByType(typeof(Kindling));
            if (kindling == null)
            {
                pm.SendMessage(0x22, "You need kindling to start a campfire.");
                return;
            }

            // Check for existing camps
            foreach (Item item in pm.GetItemsInRange(15))
            {
                if (item is VystiaCampfire || item is VystiaSafeCamp || item is VystiaOutpost)
                {
                    pm.SendMessage(0x22, "There is already a camp nearby.");
                    return;
                }
            }

            // Consume kindling
            kindling.Consume(1);

            // Create campfire
            var campfire = new VystiaCampfire();
            campfire.MoveToWorld(pm.Location, pm.Map);
            campfire.Setup(pm);

            pm.CheckSkill(SkillName.Camping, 0, 100);
        }

        /// <summary>
        /// [GiveCampKit <type> - Give a camping kit (GM command)
        /// </summary>
        [Usage("GiveCampKit <Campfire|SafeCamp|Outpost>")]
        [Description("Give yourself a camping kit.")]
        private static void GiveCampKit_OnCommand(CommandEventArgs e)
        {
            if (!(e.Mobile is PlayerMobile pm))
                return;

            CampType type = CampType.Campfire;
            if (e.Arguments.Length > 0)
            {
                if (!Enum.TryParse(e.Arguments[0], true, out type))
                {
                    pm.SendMessage("Valid types: Campfire, SafeCamp, Outpost");
                    return;
                }
            }

            var kit = new VystiaCampingKit(type, 5);
            pm.Backpack?.DropItem(kit);
            pm.SendMessage("You receive a {0} kit with 5 uses.", type);
        }
    }

    #endregion
}
