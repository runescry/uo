using System;
using Server;
using Server.Mobiles;
using Server.Engines.Craft;
using Server.Gumps;

namespace Server.Items.VystiaClassItems
{
    #region BeastWhistle
    /// <summary>
    /// Beastmaster's Beast Whistle - Summons a temporary animal companion
    /// Companion type depends on BeastBonding skill level
    /// </summary>
    public class BeastWhistle : Item
    {
        private int m_Charges;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set { m_Charges = value; InvalidateProperties(); }
        }

        [Constructable]
        public BeastWhistle() : base(0x1F1C) // Flute item ID
        {
            Name = "Beast Whistle";
            Hue = 1150; // Frosthold hue
            Weight = 0.5;
            LootType = LootType.Blessed;
            m_Charges = 10;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060741, m_Charges.ToString()); // charges remaining
            list.Add(1070722, "Call animal companion");
            list.Add("Companion scales with BeastBonding skill");
            list.Add("Duration: 10 minutes");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // must be in pack
                return;
            }

            if (m_Charges <= 0)
            {
                from.SendMessage(0x22, "The whistle has no charges remaining.");
                return;
            }

            // Check follower slots
            if (from.Followers + 2 > from.FollowersMax)
            {
                from.SendMessage(0x22, "You have too many followers to summon a companion!");
                return;
            }

            // Get BeastBonding skill for scaling
            double beastBonding = from.Skills[SkillName.BeastBonding].Value;

            BaseCreature companion = GetCompanionForSkill(beastBonding);
            if (companion == null)
            {
                from.SendMessage(0x22, "The whistle fails to call any creature.");
                return;
            }

            from.SendMessage(0x3B2, "You blow the whistle, calling for an animal companion...");
            from.FixedParticles(0x376A, 10, 20, 5038, 1150, 0, EffectLayer.Waist);
            from.PlaySound(0x4CF);

            // Setup the companion
            companion.Controlled = true;
            companion.ControlMaster = from;
            companion.ControlOrder = OrderType.Follow;

            // Find spawn location
            Point3D loc = from.Location;
            Map map = from.Map;

            if (map != null)
            {
                for (int i = 0; i < 10; i++)
                {
                    int x = loc.X + Utility.RandomMinMax(-2, 2);
                    int y = loc.Y + Utility.RandomMinMax(-2, 2);
                    int z = map.GetAverageZ(x, y);

                    Point3D p = new Point3D(x, y, z);

                    if (map.CanSpawnMobile(p))
                    {
                        companion.MoveToWorld(p, map);

                        Effects.SendLocationParticles(
                            EffectItem.Create(p, map, EffectItem.DefaultDuration),
                            0x376A, 10, 15, 1150, 0, 5038, 0);

                        m_Charges--;
                        InvalidateProperties();

                        from.SendMessage(0x3B2, $"A {companion.Name} answers your call!");

                        // Schedule despawn after 10 minutes
                        Timer.DelayCall(TimeSpan.FromMinutes(10), () =>
                        {
                            if (companion != null && !companion.Deleted)
                            {
                                if (companion.ControlMaster != null && !companion.ControlMaster.Deleted)
                                {
                                    companion.ControlMaster.SendMessage(0x3B2, $"Your {companion.Name} returns to the wild.");
                                }
                                companion.Delete();
                            }
                        });

                        return;
                    }
                }
            }

            // Failed to find spawn location
            companion.Delete();
            from.SendMessage(0x22, "There is no room to summon your companion!");
        }

        private BaseCreature GetCompanionForSkill(double skill)
        {
            // Skill tiers determine companion type
            if (skill >= 90) // Grandmaster
            {
                switch (Utility.Random(3))
                {
                    case 0: return new WhiteWolf() { Name = "bonded white wolf" };
                    case 1: return new GrizzlyBear() { Name = "bonded grizzly bear" };
                    default: return new GreatHart() { Name = "bonded great hart" };
                }
            }
            else if (skill >= 60) // Expert
            {
                switch (Utility.Random(3))
                {
                    case 0: return new TimberWolf() { Name = "bonded timber wolf" };
                    case 1: return new BlackBear() { Name = "bonded black bear" };
                    default: return new Panther() { Name = "bonded panther" };
                }
            }
            else if (skill >= 30) // Journeyman
            {
                switch (Utility.Random(3))
                {
                    case 0: return new DireWolf() { Name = "bonded dire wolf" };
                    case 1: return new BrownBear() { Name = "bonded brown bear" };
                    default: return new Cougar() { Name = "bonded cougar" };
                }
            }
            else // Apprentice
            {
                switch (Utility.Random(3))
                {
                    case 0: return new GreyWolf() { Name = "bonded grey wolf" };
                    case 1: return new Dog() { Name = "bonded hound" };
                    default: return new Cat() { Name = "bonded wildcat" };
                }
            }
        }

        public BeastWhistle(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.Write(m_Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Charges = reader.ReadInt();
        }
    }
    #endregion

    #region AlchemistKit
    /// <summary>
    /// Alchemist's Kit - Portable transmutation station
    /// Uses SkillName.Transmutation for crafting
    /// Blessed item with unlimited uses
    /// </summary>
    public class AlchemistKit : Item, ITool
    {
        private int m_UsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; InvalidateProperties(); }
        }

        public bool ShowUsesRemaining
        {
            get { return false; } // Blessed, unlimited uses
            set { }
        }

        public CraftSystem CraftSystem => DefTransmutation.CraftSystem;

        public bool BreakOnDepletion => false; // Never breaks

        [Constructable]
        public AlchemistKit() : base(0x182D) // Mortar and Pestle
        {
            Name = "Alchemist's Kit";
            Hue = 2010; // Verdantpeak hue
            Weight = 2.0;
            LootType = LootType.Blessed;
            m_UsesRemaining = int.MaxValue; // Unlimited
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1070722, "Portable transmutation station");
            list.Add("Uses Transmutation skill");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack
                return;
            }

            from.SendMessage(0x3B2, "You open your alchemist's kit...");
            from.SendGump(new CraftGump(from, CraftSystem, this, null));
        }

        public bool CheckAccessible(Mobile from, ref int num)
        {
            if (RootParent != from)
            {
                num = 1044263; // The tool must be on your person to use.
                return false;
            }

            return true;
        }

        public AlchemistKit(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
            writer.Write(m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 1)
                m_UsesRemaining = reader.ReadInt();
            else
                m_UsesRemaining = int.MaxValue;
        }
    }
    #endregion

    #region CrystalOrb
    /// <summary>
    /// Oracle's Crystal Orb - Divination focus
    /// Reveals information about nearby creatures and items
    /// </summary>
    public class CrystalOrb : Item
    {
        private DateTime m_NextUse;
        private TimeSpan m_Cooldown = TimeSpan.FromSeconds(30);

        [Constructable]
        public CrystalOrb() : base(0xE2E) // Crystal Ball
        {
            Name = "Oracle's Crystal Orb";
            Hue = 1154; // Crystal Barrens hue
            Weight = 1.0;
            LootType = LootType.Blessed;
            m_NextUse = DateTime.UtcNow;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1070722, "Focus for divination magic");
            list.Add("Reveals creature information");
            list.Add("Cooldown: 30 seconds");

            TimeSpan remaining = m_NextUse - DateTime.UtcNow;
            if (remaining > TimeSpan.Zero)
                list.Add($"Ready in: {remaining.Seconds}s");
            else
                list.Add(1153, "Ready to use");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // must be in pack
                return;
            }

            if (DateTime.UtcNow < m_NextUse)
            {
                TimeSpan remaining = m_NextUse - DateTime.UtcNow;
                from.SendMessage(0x22, $"The orb needs time to recharge. ({remaining.Seconds} seconds remaining)");
                return;
            }

            from.SendMessage(0x3B2, "You gaze into the crystal orb...");
            from.FixedParticles(0x375A, 10, 15, 5038, 1154, 0, EffectLayer.Waist);
            from.PlaySound(0x1F5);

            // Divination effect - reveal info about nearby creatures
            int revealed = 0;
            foreach (Mobile m in from.GetMobilesInRange(8))
            {
                if (m != from && m is BaseCreature bc)
                {
                    from.SendMessage(0x3B2, $"[{bc.Name}] HP: {bc.Hits}/{bc.HitsMax}, STR: {bc.Str}, DEX: {bc.Dex}, INT: {bc.Int}");
                    revealed++;
                    if (revealed >= 5) break;
                }
            }

            if (revealed == 0)
                from.SendMessage(0x3B2, "The orb reveals no creatures nearby.");
            else
                from.SendMessage(0x3B2, $"The orb revealed {revealed} creature(s).");

            m_NextUse = DateTime.UtcNow + m_Cooldown;
            InvalidateProperties();
        }

        public CrystalOrb(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
            writer.Write(m_NextUse);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 1)
                m_NextUse = reader.ReadDateTime();
        }
    }
    #endregion

    #region MonkBeads
    /// <summary>
    /// Monk's Prayer Beads - Meditation and focus tool
    /// Grants instant mana and stamina regeneration on use
    /// </summary>
    public class MonkBeads : Item
    {
        private DateTime m_NextUse;
        private TimeSpan m_Cooldown = TimeSpan.FromSeconds(60);

        [Constructable]
        public MonkBeads() : base(0x1F06) // Beads
        {
            Name = "Monk's Prayer Beads";
            Hue = 2305; // Ironclad hue
            Weight = 0.1;
            LootType = LootType.Blessed;
            m_NextUse = DateTime.UtcNow;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1070722, "Aids meditation and focus");
            list.Add("Restores Mana and Stamina");
            list.Add("Cooldown: 60 seconds");

            TimeSpan remaining = m_NextUse - DateTime.UtcNow;
            if (remaining > TimeSpan.Zero)
                list.Add($"Ready in: {remaining.Seconds}s");
            else
                list.Add(1153, "Ready to use");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // must be in pack
                return;
            }

            if (DateTime.UtcNow < m_NextUse)
            {
                TimeSpan remaining = m_NextUse - DateTime.UtcNow;
                from.SendMessage(0x22, $"You must wait ({remaining.Seconds} seconds) before meditating again.");
                return;
            }

            from.SendMessage(0x3B2, "You focus your mind through the prayer beads...");
            from.FixedParticles(0x376A, 10, 15, 5005, 2305, 0, EffectLayer.Waist);
            from.PlaySound(0x1F8);

            // Restore mana and stamina based on MartialArts skill
            double martialArts = from.Skills[SkillName.MartialArts].Value;
            int regenAmount = 20 + (int)(martialArts * 0.3); // 20-50 based on skill

            from.Mana = Math.Min(from.Mana + regenAmount, from.ManaMax);
            from.Stam = Math.Min(from.Stam + regenAmount, from.StamMax);

            from.SendMessage(0x3B2, $"Your meditation restores {regenAmount} mana and stamina!");

            m_NextUse = DateTime.UtcNow + m_Cooldown;
            InvalidateProperties();
        }

        public MonkBeads(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
            writer.Write(m_NextUse);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 1)
                m_NextUse = reader.ReadDateTime();
        }
    }
    #endregion

    #region TemplarCross
    /// <summary>
    /// Templar's Holy Cross - Symbol of divine power
    /// Grants +10 all resists for 30 seconds
    /// </summary>
    public class TemplarCross : Item
    {
        private DateTime m_NextUse;
        private TimeSpan m_Cooldown = TimeSpan.FromSeconds(90);

        [Constructable]
        public TemplarCross() : base(0x1F09) // Holy Symbol
        {
            Name = "Templar's Cross";
            Hue = 2305; // Ironclad hue
            Weight = 0.5;
            LootType = LootType.Blessed;
            m_NextUse = DateTime.UtcNow;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1070722, "Symbol of divine faith");
            list.Add("Grants +10 All Resists");
            list.Add("Duration: 30 seconds");
            list.Add("Cooldown: 90 seconds");

            TimeSpan remaining = m_NextUse - DateTime.UtcNow;
            if (remaining > TimeSpan.Zero)
                list.Add($"Ready in: {remaining.Seconds}s");
            else
                list.Add(1153, "Ready to use");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // must be in pack
                return;
            }

            if (DateTime.UtcNow < m_NextUse)
            {
                TimeSpan remaining = m_NextUse - DateTime.UtcNow;
                from.SendMessage(0x22, $"The cross needs time to recharge. ({remaining.Seconds} seconds remaining)");
                return;
            }

            from.SendMessage(0x3B2, "You raise the holy cross and invoke divine protection!");
            from.FixedParticles(0x373A, 10, 15, 5018, 1153, 0, EffectLayer.Waist);
            from.PlaySound(0x1F2);

            // Apply resist buffs
            from.AddResistanceMod(new ResistanceMod(ResistanceType.Physical, 10));
            from.AddResistanceMod(new ResistanceMod(ResistanceType.Fire, 10));
            from.AddResistanceMod(new ResistanceMod(ResistanceType.Cold, 10));
            from.AddResistanceMod(new ResistanceMod(ResistanceType.Poison, 10));
            from.AddResistanceMod(new ResistanceMod(ResistanceType.Energy, 10));

            from.SendMessage(0x3B2, "Divine protection surrounds you! (+10 All Resists for 30 seconds)");

            // Schedule buff removal
            Timer.DelayCall(TimeSpan.FromSeconds(30), () =>
            {
                if (from != null && !from.Deleted)
                {
                    from.RemoveResistanceMod(new ResistanceMod(ResistanceType.Physical, 10));
                    from.RemoveResistanceMod(new ResistanceMod(ResistanceType.Fire, 10));
                    from.RemoveResistanceMod(new ResistanceMod(ResistanceType.Cold, 10));
                    from.RemoveResistanceMod(new ResistanceMod(ResistanceType.Poison, 10));
                    from.RemoveResistanceMod(new ResistanceMod(ResistanceType.Energy, 10));
                    from.SendMessage(0x3B2, "The divine protection fades...");
                }
            });

            m_NextUse = DateTime.UtcNow + m_Cooldown;
            InvalidateProperties();
        }

        public TemplarCross(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
            writer.Write(m_NextUse);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 1)
                m_NextUse = reader.ReadDateTime();
        }
    }
    #endregion

    #region SummoningCircle
    /// <summary>
    /// Summoner's Circle - Focus for binding creatures
    /// On use: Empowers your summoned creatures with +25% HP for 60 seconds
    /// Cooldown: 120 seconds
    /// </summary>
    public class SummoningCircle : Item
    {
        private DateTime m_NextUse;
        private TimeSpan m_Cooldown = TimeSpan.FromSeconds(120);

        [Constructable]
        public SummoningCircle() : base(0x1F13) // Pentagram
        {
            Name = "Summoning Circle";
            Hue = 1365; // Underwater hue
            Weight = 1.0;
            LootType = LootType.Blessed;
            m_NextUse = DateTime.UtcNow;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1070722, "Focus for summoning magic");
            list.Add("Empowers summoned creatures (+25% HP)");
            list.Add("Duration: 60 seconds");
            list.Add("Cooldown: 120 seconds");

            TimeSpan remaining = m_NextUse - DateTime.UtcNow;
            if (remaining > TimeSpan.Zero)
                list.Add($"Ready in: {remaining.Seconds}s");
            else
                list.Add(1153, "Ready to use");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // must be in pack
                return;
            }

            if (DateTime.UtcNow < m_NextUse)
            {
                TimeSpan remaining = m_NextUse - DateTime.UtcNow;
                from.SendMessage(0x22, $"The circle needs time to recharge. ({remaining.Seconds} seconds remaining)");
                return;
            }

            from.SendMessage(0x3B2, "You invoke the power of the summoning circle...");
            from.FixedParticles(0x3779, 10, 25, 5052, 1365, 0, EffectLayer.Waist);
            from.PlaySound(0x1E9);

            // Find all controlled summons and empower them
            int empowered = 0;
            foreach (Mobile m in World.Mobiles.Values)
            {
                if (m is BaseCreature bc && bc.Controlled && bc.ControlMaster == from && bc.Summoned)
                {
                    int bonusHits = bc.HitsMax / 4; // +25% HP
                    int originalMax = bc.HitsMax;
                    bc.SetHits(bc.HitsMax + bonusHits, bc.HitsMax + bonusHits);
                    bc.Hits = bc.HitsMax;

                    bc.FixedParticles(0x376A, 10, 15, 5038, 1365, 0, EffectLayer.Waist);
                    bc.PlaySound(0x1E9);

                    empowered++;

                    // Schedule buff removal
                    Timer.DelayCall(TimeSpan.FromSeconds(60), () =>
                    {
                        if (bc != null && !bc.Deleted)
                        {
                            bc.SetHits(originalMax, originalMax);
                            bc.Hits = Math.Min(bc.Hits, bc.HitsMax);
                        }
                    });
                }
            }

            if (empowered == 0)
                from.SendMessage(0x22, "You have no summoned creatures to empower!");
            else
            {
                from.SendMessage(0x3B2, $"Empowered {empowered} summoned creature(s) with +25% HP for 60 seconds!");
                m_NextUse = DateTime.UtcNow + m_Cooldown;
                InvalidateProperties();
            }
        }

        public SummoningCircle(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
            writer.Write(m_NextUse);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 1)
                m_NextUse = reader.ReadDateTime();
        }
    }
    #endregion

    #region BountyLedger
    /// <summary>
    /// Bounty Hunter's Ledger - Tracks targets and bounties
    /// On use: Marks a target for tracking, gaining +10% damage against them
    /// One target at a time, mark lasts 5 minutes
    /// </summary>
    public class BountyLedger : Item
    {
        private Mobile m_MarkedTarget;
        private DateTime m_MarkExpires;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile MarkedTarget
        {
            get { return m_MarkedTarget; }
            set { m_MarkedTarget = value; InvalidateProperties(); }
        }

        [Constructable]
        public BountyLedger() : base(0xFF0) // Book
        {
            Name = "Bounty Ledger";
            Hue = 1153; // Multi-Regional hue
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1070722, "Tracks bounties and targets");
            list.Add("Mark a target for +10% damage");
            list.Add("Duration: 5 minutes");

            if (m_MarkedTarget != null && !m_MarkedTarget.Deleted && DateTime.UtcNow < m_MarkExpires)
            {
                TimeSpan remaining = m_MarkExpires - DateTime.UtcNow;
                list.Add($"Marked: {m_MarkedTarget.Name} ({remaining.Minutes}m {remaining.Seconds}s)");
            }
            else
                list.Add("No target marked");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // must be in pack
                return;
            }

            from.SendMessage(0x3B2, "Select a target to mark in your bounty ledger...");
            from.Target = new BountyMarkTarget(this);
        }

        public void MarkTarget(Mobile from, Mobile target)
        {
            if (target == null || target.Deleted)
            {
                from.SendMessage(0x22, "Invalid target!");
                return;
            }

            if (target == from)
            {
                from.SendMessage(0x22, "You cannot mark yourself!");
                return;
            }

            // Clear previous mark
            m_MarkedTarget = target;
            m_MarkExpires = DateTime.UtcNow + TimeSpan.FromMinutes(5);

            from.SendMessage(0x3B2, $"You mark {target.Name} in your bounty ledger. You deal +10% damage to this target!");
            from.FixedParticles(0x373A, 10, 15, 5036, 1153, 0, EffectLayer.Head);
            from.PlaySound(0x1F5);

            // Visual effect on target
            target.FixedParticles(0x376A, 10, 15, 5013, 1153, 0, EffectLayer.Head);

            InvalidateProperties();

            // Schedule mark removal
            Timer.DelayCall(TimeSpan.FromMinutes(5), () =>
            {
                if (m_MarkedTarget == target)
                {
                    m_MarkedTarget = null;
                    if (from != null && !from.Deleted)
                        from.SendMessage(0x3B2, "Your bounty mark has expired.");
                    InvalidateProperties();
                }
            });
        }

        public bool IsMarked(Mobile target)
        {
            return m_MarkedTarget == target && DateTime.UtcNow < m_MarkExpires && !target.Deleted;
        }

        private class BountyMarkTarget : Server.Targeting.Target
        {
            private BountyLedger m_Ledger;

            public BountyMarkTarget(BountyLedger ledger) : base(12, false, Server.Targeting.TargetFlags.None)
            {
                m_Ledger = ledger;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Mobile target)
                {
                    m_Ledger.MarkTarget(from, target);
                }
                else
                {
                    from.SendMessage(0x22, "You can only mark creatures or players!");
                }
            }
        }

        public BountyLedger(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
            writer.Write(m_MarkedTarget);
            writer.Write(m_MarkExpires);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 1)
            {
                m_MarkedTarget = reader.ReadMobile();
                m_MarkExpires = reader.ReadDateTime();
            }
        }
    }
    #endregion

    #region KnightBanner
    /// <summary>
    /// Knight's Banner - Symbol of honor and courage
    /// On use: Buffs all party members within 8 tiles with +5 STR/DEX for 60 seconds
    /// Cooldown: 180 seconds
    /// </summary>
    public class KnightBanner : Item
    {
        private DateTime m_NextUse;
        private TimeSpan m_Cooldown = TimeSpan.FromSeconds(180);

        [Constructable]
        public KnightBanner() : base(0x1614) // Banner
        {
            Name = "Knight's Banner";
            Hue = 1153; // Multi-Regional hue
            Weight = 2.0;
            LootType = LootType.Blessed;
            m_NextUse = DateTime.UtcNow;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1070722, "Symbol of knightly honor");
            list.Add("Rally allies: +5 STR/DEX");
            list.Add("Range: 8 tiles, Duration: 60s");
            list.Add("Cooldown: 180 seconds");

            TimeSpan remaining = m_NextUse - DateTime.UtcNow;
            if (remaining > TimeSpan.Zero)
                list.Add($"Ready in: {remaining.Seconds}s");
            else
                list.Add(1153, "Ready to use");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // must be in pack
                return;
            }

            if (DateTime.UtcNow < m_NextUse)
            {
                TimeSpan remaining = m_NextUse - DateTime.UtcNow;
                from.SendMessage(0x22, $"The banner's power is recharging. ({remaining.Seconds} seconds remaining)");
                return;
            }

            from.SendMessage(0x3B2, "You raise the banner high, rallying your allies!");
            from.FixedParticles(0x373A, 10, 30, 5052, 1153, 0, EffectLayer.Waist);
            from.PlaySound(0x1E3);

            // Buff all party members and self within 8 tiles
            int buffed = 0;

            // Always buff self
            ApplyBannerBuff(from);
            buffed++;

            // Check party members
            if (from is PlayerMobile pm && pm.Party is Server.Engines.PartySystem.Party party)
            {
                foreach (Server.Engines.PartySystem.PartyMemberInfo pmi in party.Members)
                {
                    if (pmi.Mobile != from && pmi.Mobile.InRange(from.Location, 8) && pmi.Mobile.Map == from.Map)
                    {
                        ApplyBannerBuff(pmi.Mobile);
                        buffed++;
                    }
                }
            }

            from.SendMessage(0x3B2, $"Your banner rallies {buffed} warrior(s)! (+5 STR/DEX for 60 seconds)");

            m_NextUse = DateTime.UtcNow + m_Cooldown;
            InvalidateProperties();
        }

        private void ApplyBannerBuff(Mobile m)
        {
            m.AddStatMod(new StatMod(StatType.Str, "KnightBanner_Str", 5, TimeSpan.FromSeconds(60)));
            m.AddStatMod(new StatMod(StatType.Dex, "KnightBanner_Dex", 5, TimeSpan.FromSeconds(60)));

            m.FixedParticles(0x376A, 10, 15, 5005, 1153, 0, EffectLayer.Waist);
            m.SendMessage(0x3B2, "You are inspired by the knight's banner! (+5 STR/DEX)");
        }

        public KnightBanner(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
            writer.Write(m_NextUse);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 1)
                m_NextUse = reader.ReadDateTime();
        }
    }
    #endregion

    #region SpiritTotem
    /// <summary>
    /// Shaman's Spirit Totem - Channel for spiritual energy
    /// On use: Channels ancestral spirits for instant heal based on Shamanism skill
    /// Also grants temporary +5 HP regen for 30 seconds
    /// Cooldown: 90 seconds
    /// </summary>
    public class SpiritTotem : Item
    {
        private DateTime m_NextUse;
        private TimeSpan m_Cooldown = TimeSpan.FromSeconds(90);

        [Constructable]
        public SpiritTotem() : base(0x1F18) // Totem
        {
            Name = "Spirit Totem";
            Hue = 1153; // Multi-Regional hue
            Weight = 1.5;
            LootType = LootType.Blessed;
            m_NextUse = DateTime.UtcNow;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1070722, "Channel for spirit magic");
            list.Add("Channels spirits to heal you");
            list.Add("+5 HP Regen for 30s");
            list.Add("Cooldown: 90 seconds");

            TimeSpan remaining = m_NextUse - DateTime.UtcNow;
            if (remaining > TimeSpan.Zero)
                list.Add($"Ready in: {remaining.Seconds}s");
            else
                list.Add(1153, "Ready to use");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // must be in pack
                return;
            }

            if (DateTime.UtcNow < m_NextUse)
            {
                TimeSpan remaining = m_NextUse - DateTime.UtcNow;
                from.SendMessage(0x22, $"The spirits need time to recover. ({remaining.Seconds} seconds remaining)");
                return;
            }

            from.SendMessage(0x3B2, "You channel the ancestral spirits through the totem...");
            from.FixedParticles(0x375A, 10, 30, 5052, 1109, 0, EffectLayer.Waist);
            from.PlaySound(0x1F2);

            // Heal based on SpiritCalling skill
            double spiritCalling = from.Skills[SkillName.SpiritCalling].Value;
            int healAmount = 30 + (int)(spiritCalling * 0.5); // 30-80 based on skill

            from.Heal(healAmount);
            from.SendMessage(0x3B2, $"The spirits restore {healAmount} health!");

            // Add HP regeneration buff
            from.AddStatMod(new StatMod(StatType.All, "SpiritTotem_Regen", 0, TimeSpan.FromSeconds(30)));

            // Create a delayed effect for HP regen visual cue
            for (int i = 0; i < 6; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(i * 5), () =>
                {
                    if (from != null && !from.Deleted && from.Alive)
                    {
                        from.Heal(5);
                        from.FixedParticles(0x376A, 5, 10, 5005, 1109, 0, EffectLayer.Waist);
                    }
                });
            }

            from.SendMessage(0x3B2, "Ancestral spirits continue healing you for 30 seconds!");

            m_NextUse = DateTime.UtcNow + m_Cooldown;
            InvalidateProperties();
        }

        public SpiritTotem(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
            writer.Write(m_NextUse);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 1)
                m_NextUse = reader.ReadDateTime();
        }
    }
    #endregion

    #region MagicLute
    /// <summary>
    /// Bard's Magic Lute - Enchanted musical instrument
    /// On use: Plays an inspiring song granting +10 to all skills for 45 seconds
    /// Range: 6 tiles (self and party members)
    /// Cooldown: 120 seconds
    /// </summary>
    public class MagicLute : Item
    {
        private DateTime m_NextUse;
        private TimeSpan m_Cooldown = TimeSpan.FromSeconds(120);

        [Constructable]
        public MagicLute() : base(0xEB3) // Lute
        {
            Name = "Magic Lute";
            Hue = 1153; // Multi-Regional hue
            Weight = 5.0;
            LootType = LootType.Blessed;
            m_NextUse = DateTime.UtcNow;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1070722, "Enchanted musical instrument");
            list.Add("Song of Inspiration: +10 All Skills");
            list.Add("Range: 6 tiles, Duration: 45s");
            list.Add("Cooldown: 120 seconds");

            TimeSpan remaining = m_NextUse - DateTime.UtcNow;
            if (remaining > TimeSpan.Zero)
                list.Add($"Ready in: {remaining.Seconds}s");
            else
                list.Add(1153, "Ready to use");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // must be in pack
                return;
            }

            if (DateTime.UtcNow < m_NextUse)
            {
                TimeSpan remaining = m_NextUse - DateTime.UtcNow;
                from.SendMessage(0x22, $"You need time to prepare another song. ({remaining.Seconds} seconds remaining)");
                return;
            }

            from.SendMessage(0x3B2, "You play an inspiring melody on the magic lute!");
            from.FixedParticles(0x376A, 10, 30, 5052, 1109, 0, EffectLayer.Waist);
            from.PlaySound(0x38); // Lute sound

            // Buff all allies in range
            int inspired = 0;

            // Buff self
            ApplyInspiration(from);
            inspired++;

            // Check party members
            if (from is PlayerMobile pm && pm.Party is Server.Engines.PartySystem.Party party)
            {
                foreach (Server.Engines.PartySystem.PartyMemberInfo pmi in party.Members)
                {
                    if (pmi.Mobile != from && pmi.Mobile.InRange(from.Location, 6) && pmi.Mobile.Map == from.Map)
                    {
                        ApplyInspiration(pmi.Mobile);
                        inspired++;
                    }
                }
            }

            from.SendMessage(0x3B2, $"Your song inspires {inspired} listener(s)! (+10 All Skills for 45 seconds)");

            m_NextUse = DateTime.UtcNow + m_Cooldown;
            InvalidateProperties();
        }

        private void ApplyInspiration(Mobile m)
        {
            // Add skill bonus to all skills (using DefaultSkillMod)
            foreach (Skill skill in m.Skills)
            {
                m.AddSkillMod(new TimedSkillMod(skill.SkillName, true, 10.0, TimeSpan.FromSeconds(45)));
            }

            m.FixedParticles(0x375A, 10, 15, 5005, 1153, 0, EffectLayer.Waist);
            m.SendMessage(0x3B2, "You are inspired by the bard's song! (+10 All Skills)");
        }

        public MagicLute(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
            writer.Write(m_NextUse);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 1)
                m_NextUse = reader.ReadDateTime();
        }
    }
    #endregion

    #region EnchantingCrystal
    /// <summary>
    /// Enchanter's Crystal - Focus for enchantment magic
    /// On use: Temporarily enchants your equipped weapon with +10 damage and elemental damage
    /// Duration: 60 seconds
    /// Cooldown: 180 seconds
    /// </summary>
    public class EnchantingCrystal : Item
    {
        private DateTime m_NextUse;
        private TimeSpan m_Cooldown = TimeSpan.FromSeconds(180);

        [Constructable]
        public EnchantingCrystal() : base(0x1F19) // Crystal
        {
            Name = "Enchanting Crystal";
            Hue = 1153; // Multi-Regional hue
            Weight = 0.5;
            LootType = LootType.Blessed;
            m_NextUse = DateTime.UtcNow;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1070722, "Focus for enchanting magic");
            list.Add("Enchant Weapon: +10 Damage");
            list.Add("Duration: 60s, Cooldown: 180s");

            TimeSpan remaining = m_NextUse - DateTime.UtcNow;
            if (remaining > TimeSpan.Zero)
                list.Add($"Ready in: {remaining.Seconds}s");
            else
                list.Add(1153, "Ready to use");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // must be in pack
                return;
            }

            if (DateTime.UtcNow < m_NextUse)
            {
                TimeSpan remaining = m_NextUse - DateTime.UtcNow;
                from.SendMessage(0x22, $"The crystal needs time to recharge. ({remaining.Seconds} seconds remaining)");
                return;
            }

            // Find equipped weapon
            Item weapon = from.FindItemOnLayer(Layer.OneHanded);
            if (weapon == null)
                weapon = from.FindItemOnLayer(Layer.TwoHanded);

            if (weapon == null || !(weapon is BaseWeapon bw))
            {
                from.SendMessage(0x22, "You must have a weapon equipped to enchant!");
                return;
            }

            from.SendMessage(0x3B2, "You channel magical energy through the crystal into your weapon!");
            from.FixedParticles(0x375A, 10, 30, 5052, 1154, 0, EffectLayer.Waist);
            from.PlaySound(0x1E9);

            // Visual effect on weapon
            int originalHue = weapon.Hue;
            weapon.Hue = 1154; // Crystal hue

            // Apply damage bonus using attribute mod
            bw.Attributes.WeaponDamage += 10;

            from.SendMessage(0x3B2, $"Your {weapon.Name ?? "weapon"} glows with magical power! (+10 Damage for 60 seconds)");

            // Schedule enchantment removal
            Timer.DelayCall(TimeSpan.FromSeconds(60), () =>
            {
                if (bw != null && !bw.Deleted)
                {
                    bw.Attributes.WeaponDamage -= 10;
                    bw.Hue = originalHue;

                    if (from != null && !from.Deleted)
                        from.SendMessage(0x3B2, "The enchantment on your weapon fades...");
                }
            });

            m_NextUse = DateTime.UtcNow + m_Cooldown;
            InvalidateProperties();
        }

        public EnchantingCrystal(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
            writer.Write(m_NextUse);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 1)
                m_NextUse = reader.ReadDateTime();
        }
    }
    #endregion
}
