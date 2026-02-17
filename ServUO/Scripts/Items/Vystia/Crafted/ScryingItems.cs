/*
 * Scrying Items - Crafted by Oracles
 * Uses Divination skill (ID 63)
 * Materials: Divination Magic reagents
 */

using System;
using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;
using Server.Engines.Craft;

namespace Server.Items.Vystia
{
    #region Base Prophecy Scroll Class

    /// <summary>
    /// Base class for consumable prophecy scrolls that reveal information or grant foresight
    /// </summary>
    public abstract class BaseProphecyScroll : Item
    {
        private int m_Charges;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set { m_Charges = value; InvalidateProperties(); }
        }

        public abstract string ScrollName { get; }
        public abstract void RevealProphecy(Mobile user, Mobile target);

        public BaseProphecyScroll(int itemID, int hue, int charges) : base(itemID)
        {
            Hue = hue;
            Weight = 1.0;
            m_Charges = charges;
            Stackable = false;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060741, m_Charges.ToString()); // charges: ~1_val~
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack
                return;
            }

            if (m_Charges <= 0)
            {
                from.SendMessage(0x22, "This scroll has no charges remaining.");
                Delete();
                return;
            }

            from.SendMessage(0x3B2, $"Select a target for {ScrollName}...");
            from.Target = new ProphecyTarget(this);
        }

        public void UseCharge(Mobile from, Mobile target)
        {
            m_Charges--;

            from.PlaySound(0x1F4); // Mystical sound
            Effects.SendTargetParticles(from, 0x375A, 9, 32, Hue, 0, 5005, EffectLayer.Waist, 0);

            RevealProphecy(from, target);

            if (m_Charges <= 0)
            {
                from.SendMessage(0x22, "The scroll crumbles to dust.");
                Delete();
            }
        }

        public BaseProphecyScroll(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Charges = reader.ReadInt();
        }

        private class ProphecyTarget : Target
        {
            private BaseProphecyScroll m_Scroll;

            public ProphecyTarget(BaseProphecyScroll scroll) : base(12, false, TargetFlags.None)
            {
                m_Scroll = scroll;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Scroll.Deleted || m_Scroll.Charges <= 0)
                    return;

                if (targeted is Mobile target)
                {
                    m_Scroll.UseCharge(from, target);
                }
                else
                {
                    from.SendMessage(0x22, "You must target a creature.");
                }
            }
        }
    }

    #endregion

    #region Foresight Scrolls (Reveal Stats/Info)

    /// <summary>
    /// Scroll of Insight - Reveals target's stats
    /// </summary>
    public class ScrollOfInsight : BaseProphecyScroll
    {
        public override string ScrollName => "Scroll of Insight";

        [Constructable]
        public ScrollOfInsight() : this(5) { }

        [Constructable]
        public ScrollOfInsight(int charges) : base(0x14EE, 1152, charges) // Blue scroll
        {
            Name = "Scroll of Insight";
        }

        public override void RevealProphecy(Mobile user, Mobile target)
        {
            user.SendMessage(0x3B2, "=== Prophetic Insight ===");
            user.SendMessage(0x3B2, $"Name: {target.Name}");
            user.SendMessage(0x3B2, $"Health: {target.Hits}/{target.HitsMax}");
            user.SendMessage(0x3B2, $"Mana: {target.Mana}/{target.ManaMax}");
            user.SendMessage(0x3B2, $"Stamina: {target.Stam}/{target.StamMax}");
            user.SendMessage(0x3B2, $"STR: {target.Str} | DEX: {target.Dex} | INT: {target.Int}");
        }

        public ScrollOfInsight(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Scroll of Weakness - Reveals target's resistances
    /// </summary>
    public class ScrollOfWeakness : BaseProphecyScroll
    {
        public override string ScrollName => "Scroll of Weakness";

        [Constructable]
        public ScrollOfWeakness() : this(5) { }

        [Constructable]
        public ScrollOfWeakness(int charges) : base(0x14EE, 1161, charges) // Red scroll
        {
            Name = "Scroll of Weakness";
        }

        public override void RevealProphecy(Mobile user, Mobile target)
        {
            user.SendMessage(0x3B2, "=== Revealed Weaknesses ===");
            user.SendMessage(0x3B2, $"Physical Resist: {target.PhysicalResistance}%");
            user.SendMessage(0x3B2, $"Fire Resist: {target.FireResistance}%");
            user.SendMessage(0x3B2, $"Cold Resist: {target.ColdResistance}%");
            user.SendMessage(0x3B2, $"Poison Resist: {target.PoisonResistance}%");
            user.SendMessage(0x3B2, $"Energy Resist: {target.EnergyResistance}%");

            // Find lowest resistance
            int lowest = Math.Min(Math.Min(Math.Min(Math.Min(
                target.PhysicalResistance, target.FireResistance),
                target.ColdResistance), target.PoisonResistance), target.EnergyResistance);

            string weakness = "Physical";
            if (lowest == target.FireResistance) weakness = "Fire";
            else if (lowest == target.ColdResistance) weakness = "Cold";
            else if (lowest == target.PoisonResistance) weakness = "Poison";
            else if (lowest == target.EnergyResistance) weakness = "Energy";

            user.SendMessage(0x22, $"Greatest Weakness: {weakness} ({lowest}%)");
        }

        public ScrollOfWeakness(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Scroll of Fate - Reveals target's combat capabilities
    /// </summary>
    public class ScrollOfFate : BaseProphecyScroll
    {
        public override string ScrollName => "Scroll of Fate";

        [Constructable]
        public ScrollOfFate() : this(3) { }

        [Constructable]
        public ScrollOfFate(int charges) : base(0x14EE, 1159, charges) // Gold scroll
        {
            Name = "Scroll of Fate";
        }

        public override void RevealProphecy(Mobile user, Mobile target)
        {
            user.SendMessage(0x3B2, "=== Fate Revealed ===");

            if (target is BaseCreature bc)
            {
                user.SendMessage(0x3B2, $"Creature Type: {bc.GetType().Name}");
                user.SendMessage(0x3B2, $"Fame: {bc.Fame} | Karma: {bc.Karma}");
                user.SendMessage(0x3B2, $"Damage Range: {bc.DamageMin}-{bc.DamageMax}");
                user.SendMessage(0x3B2, $"Virtual Armor: {bc.VirtualArmor}");

                if (bc.Tamable)
                    user.SendMessage(0x3B2, $"Tamable: Yes (Min Skill: {bc.MinTameSkill})");
                else
                    user.SendMessage(0x22, "Tamable: No");
            }
            else if (target is PlayerMobile pm)
            {
                user.SendMessage(0x3B2, $"Player: {pm.Name}");
                user.SendMessage(0x3B2, $"Kills: {pm.Kills} | ShortTermMurders: {pm.ShortTermMurders}");
            }
        }

        public ScrollOfFate(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Blessing Scrolls (Apply Buffs)

    /// <summary>
    /// Scroll of Foresight - Grants defense bonus
    /// </summary>
    public class ScrollOfForesight : BaseProphecyScroll
    {
        public override string ScrollName => "Scroll of Foresight";

        [Constructable]
        public ScrollOfForesight() : this(3) { }

        [Constructable]
        public ScrollOfForesight(int charges) : base(0x14EF, 1153, charges) // Purple scroll
        {
            Name = "Scroll of Foresight";
        }

        public override void RevealProphecy(Mobile user, Mobile target)
        {
            // +10 DEX for 10 minutes (improves defense chance)
            target.AddStatMod(new StatMod(StatType.Dex, "ForesightDex", 10, TimeSpan.FromMinutes(10)));
            target.SendMessage(0x3B2, "Visions of the future enhance your reflexes!");
            user.SendMessage(0x3B2, $"You grant {target.Name} prophetic foresight!");
        }

        public ScrollOfForesight(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Scroll of Clarity - Grants INT bonus for spellcasting
    /// </summary>
    public class ScrollOfClarity : BaseProphecyScroll
    {
        public override string ScrollName => "Scroll of Clarity";

        [Constructable]
        public ScrollOfClarity() : this(3) { }

        [Constructable]
        public ScrollOfClarity(int charges) : base(0x14EF, 1152, charges) // Blue scroll
        {
            Name = "Scroll of Clarity";
        }

        public override void RevealProphecy(Mobile user, Mobile target)
        {
            // +12 INT for 10 minutes
            target.AddStatMod(new StatMod(StatType.Int, "ClarityInt", 12, TimeSpan.FromMinutes(10)));
            target.SendMessage(0x3B2, "Your mind becomes crystal clear!");
            user.SendMessage(0x3B2, $"You grant {target.Name} mental clarity!");
        }

        public ScrollOfClarity(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Scroll of Destiny - Grants all stats bonus
    /// </summary>
    public class ScrollOfDestiny : BaseProphecyScroll
    {
        public override string ScrollName => "Scroll of Destiny";

        [Constructable]
        public ScrollOfDestiny() : this(1) { }

        [Constructable]
        public ScrollOfDestiny(int charges) : base(0x14F0, 1159, charges) // Gold fancy scroll
        {
            Name = "Scroll of Destiny";
            LootType = LootType.Blessed;
        }

        public override void RevealProphecy(Mobile user, Mobile target)
        {
            // +15 all stats for 15 minutes
            target.AddStatMod(new StatMod(StatType.Str, "DestinyStr", 15, TimeSpan.FromMinutes(15)));
            target.AddStatMod(new StatMod(StatType.Dex, "DestinyDex", 15, TimeSpan.FromMinutes(15)));
            target.AddStatMod(new StatMod(StatType.Int, "DestinyInt", 15, TimeSpan.FromMinutes(15)));
            target.SendMessage(0x3B2, "Your destiny is revealed - power flows through you!");
            target.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
            user.SendMessage(0x3B2, $"You reveal {target.Name}'s true destiny!");
        }

        public ScrollOfDestiny(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Scrying Tools

    /// <summary>
    /// Crystal Ball - Shows nearby creatures
    /// </summary>
    public class ScryingCrystalBall : Item
    {
        private int m_Charges;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set { m_Charges = value; InvalidateProperties(); }
        }

        [Constructable]
        public ScryingCrystalBall() : this(10)
        {
        }

        [Constructable]
        public ScryingCrystalBall(int charges) : base(0x0E2E) // Crystal ball graphic
        {
            Name = "Scrying Crystal Ball";
            Hue = 1154; // Light blue
            Weight = 5.0;
            m_Charges = charges;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060741, m_Charges.ToString());
            list.Add(1070722, "Reveals nearby creatures");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
                return;
            }

            if (m_Charges <= 0)
            {
                from.SendMessage(0x22, "The crystal ball has lost its power.");
                return;
            }

            m_Charges--;
            from.PlaySound(0x1F4);
            Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, 1154, 0, 5005, 0);

            from.SendMessage(0x3B2, "=== Scrying Vision ===");
            int count = 0;
            foreach (Mobile m in from.GetMobilesInRange(20))
            {
                if (m != from && m.Alive)
                {
                    string direction = GetDirection(from, m);
                    from.SendMessage(0x3B2, $"{m.Name} - {direction} ({(int)from.GetDistanceToSqrt(m)} tiles)");
                    count++;

                    if (count >= 10)
                    {
                        from.SendMessage(0x3B2, "... and more");
                        break;
                    }
                }
            }

            if (count == 0)
                from.SendMessage(0x3B2, "No creatures detected nearby.");

            InvalidateProperties();
        }

        private string GetDirection(Mobile from, Mobile to)
        {
            int dx = to.X - from.X;
            int dy = to.Y - from.Y;

            if (Math.Abs(dx) > Math.Abs(dy) * 2) return dx > 0 ? "East" : "West";
            if (Math.Abs(dy) > Math.Abs(dx) * 2) return dy > 0 ? "South" : "North";
            if (dx > 0 && dy > 0) return "Southeast";
            if (dx > 0 && dy < 0) return "Northeast";
            if (dx < 0 && dy > 0) return "Southwest";
            return "Northwest";
        }

        public ScryingCrystalBall(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Charges = reader.ReadInt();
        }
    }

    /// <summary>
    /// Oracle's Compass - Points to nearest creature type
    /// </summary>
    public class OraclesCompass : Item
    {
        [Constructable]
        public OraclesCompass() : base(0x1051) // Compass graphic
        {
            Name = "Oracle's Compass";
            Hue = 1159; // Gold
            Weight = 1.0;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
                return;
            }

            from.SendMessage(0x3B2, "Target a creature to track its kind...");
            from.Target = new CompassTarget(this);
        }

        private class CompassTarget : Target
        {
            private OraclesCompass m_Compass;

            public CompassTarget(OraclesCompass compass) : base(12, false, TargetFlags.None)
            {
                m_Compass = compass;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is BaseCreature bc)
                {
                    Type creatureType = bc.GetType();
                    Mobile nearest = null;
                    double nearestDist = double.MaxValue;

                    foreach (Mobile m in from.GetMobilesInRange(50))
                    {
                        if (m != bc && m.GetType() == creatureType && m.Alive)
                        {
                            double dist = from.GetDistanceToSqrt(m);
                            if (dist < nearestDist)
                            {
                                nearestDist = dist;
                                nearest = m;
                            }
                        }
                    }

                    if (nearest != null)
                    {
                        from.SendMessage(0x3B2, $"The compass points to another {bc.Name} to the {GetDirection(from, nearest)}!");
                    }
                    else
                    {
                        from.SendMessage(0x22, $"No other {bc.Name} detected within range.");
                    }
                }
                else
                {
                    from.SendMessage(0x22, "You must target a creature.");
                }
            }

            private string GetDirection(Mobile from, Mobile to)
            {
                int dx = to.X - from.X;
                int dy = to.Y - from.Y;

                if (Math.Abs(dx) > Math.Abs(dy) * 2) return dx > 0 ? "East" : "West";
                if (Math.Abs(dy) > Math.Abs(dx) * 2) return dy > 0 ? "South" : "North";
                if (dx > 0 && dy > 0) return "Southeast";
                if (dx > 0 && dy < 0) return "Northeast";
                if (dx < 0 && dy > 0) return "Southwest";
                return "Northwest";
            }
        }

        public OraclesCompass(Serial serial) : base(serial) { }

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

    #region Talismans

    /// <summary>
    /// Oracle's Eye Talisman - Permanent scrying bonuses
    /// </summary>
    public class OraclesEyeTalisman : BaseTalisman
    {
        [Constructable]
        public OraclesEyeTalisman() : base(0x2F5B)
        {
            Name = "Oracle's Eye Talisman";
            Hue = 1154; // Light blue
            Weight = 1.0;

            Attributes.BonusInt = 8;
            Attributes.RegenMana = 3;
            Attributes.SpellDamage = 5;
        }

        public OraclesEyeTalisman(Serial serial) : base(serial) { }

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

    /// <summary>
    /// Seer's Talisman - Enhanced foresight
    /// </summary>
    public class SeersTalisman : BaseTalisman
    {
        [Constructable]
        public SeersTalisman() : base(0x2F5B)
        {
            Name = "Seer's Talisman";
            Hue = 1159; // Gold
            Weight = 1.0;

            Attributes.BonusInt = 5;
            Attributes.DefendChance = 10;
            Attributes.CastRecovery = 2;
        }

        public SeersTalisman(Serial serial) : base(serial) { }

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

    #region Scrying Tool

    /// <summary>
    /// Scrying Kit - Portable scrying station for crafting
    /// </summary>
    public class ScryingKit : Item, ITool
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
            get { return true; }
            set { }
        }

        public CraftSystem CraftSystem => DefScrying.CraftSystem;

        public bool BreakOnDepletion => true;

        [Constructable]
        public ScryingKit() : this(50)
        {
        }

        [Constructable]
        public ScryingKit(int uses) : base(0x1C11) // Scroll case graphic
        {
            Name = "Scrying Kit";
            Hue = 1154; // Light blue
            Weight = 3.0;
            m_UsesRemaining = uses;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060584, m_UsesRemaining.ToString());
            list.Add(1070722, "Portable scrying station");
            list.Add("Uses Divination skill");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
                return;
            }

            if (m_UsesRemaining <= 0)
            {
                from.SendMessage(0x22, "This kit has been worn out.");
                return;
            }

            from.SendMessage(0x3B2, "You open your scrying kit...");
            from.SendGump(new CraftGump(from, CraftSystem, this, null));
        }

        public bool CheckAccessible(Mobile from, ref int num)
        {
            if (RootParent != from)
            {
                num = 1044263;
                return false;
            }

            return true;
        }

        public ScryingKit(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_UsesRemaining = reader.ReadInt();
        }
    }

    #endregion
}
