/*
 * Vystia Crafting Discipline Items
 * Items for: Leathercraft, Woodshaping, Clothcraft, Necrocraft, Jewelcraft
 */

using System;
using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;
using Server.Engines.Craft;

namespace Server.Items.Vystia
{
    // ============================================
    // LEATHERCRAFT ITEMS (Ranger)
    // ============================================

    #region Leathercraft Tool

    public class LeathercraftKit : Item, ITool
    {
        private int m_UsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; InvalidateProperties(); }
        }

        public bool ShowUsesRemaining { get { return true; } set { } }
        public CraftSystem CraftSystem => DefLeathercraft.CraftSystem;
        public bool BreakOnDepletion => true;

        [Constructable]
        public LeathercraftKit() : this(50) { }

        [Constructable]
        public LeathercraftKit(int uses) : base(0x0F9D) // Sewing kit graphic
        {
            Name = "Leathercraft Kit";
            Hue = 1126; // Brown leather hue
            Weight = 2.0;
            m_UsesRemaining = uses;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060584, m_UsesRemaining.ToString());
            list.Add("Uses Tailoring skill");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack)) { from.SendLocalizedMessage(1042001); return; }
            if (m_UsesRemaining <= 0) { from.SendMessage(0x22, "This kit is worn out."); return; }
            from.SendGump(new CraftGump(from, CraftSystem, this, null));
        }

        public bool CheckAccessible(Mobile from, ref int num)
        {
            if (RootParent != from) { num = 1044263; return false; }
            return true;
        }

        public LeathercraftKit(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); writer.Write(m_UsesRemaining); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); m_UsesRemaining = reader.ReadInt(); }
    }

    #endregion

    #region Ranger Quivers

    public class RangerQuiver : BaseQuiver
    {
        [Constructable]
        public RangerQuiver() : base(0x2FB7)
        {
            Name = "Ranger's Quiver";
            Hue = 1126;
            WeightReduction = 30;
            DamageIncrease = 5;
        }

        public RangerQuiver(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class EliteRangerQuiver : BaseQuiver
    {
        [Constructable]
        public EliteRangerQuiver() : base(0x2FB7)
        {
            Name = "Elite Ranger's Quiver";
            Hue = 1266; // Forest green
            WeightReduction = 50;
            DamageIncrease = 10;
            LowerAmmoCost = 20;
        }

        public EliteRangerQuiver(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Ranger Leather Armor

    public class RangerLeatherCap : LeatherCap
    {
        [Constructable]
        public RangerLeatherCap()
        {
            Name = "Ranger's Leather Cap";
            Hue = 1126;
            Attributes.BonusDex = 3;
            Attributes.DefendChance = 5;
        }

        public RangerLeatherCap(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class RangerLeatherChest : LeatherChest
    {
        [Constructable]
        public RangerLeatherChest()
        {
            Name = "Ranger's Leather Tunic";
            Hue = 1126;
            Attributes.BonusDex = 5;
            Attributes.BonusStam = 10;
        }

        public RangerLeatherChest(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class RangerLeatherLegs : LeatherLegs
    {
        [Constructable]
        public RangerLeatherLegs()
        {
            Name = "Ranger's Leather Leggings";
            Hue = 1126;
            Attributes.BonusDex = 3;
            Attributes.RegenStam = 2;
        }

        public RangerLeatherLegs(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class RangerLeatherGloves : LeatherGloves
    {
        [Constructable]
        public RangerLeatherGloves()
        {
            Name = "Ranger's Leather Gloves";
            Hue = 1126;
            Attributes.BonusDex = 2;
            Attributes.WeaponSpeed = 5;
        }

        public RangerLeatherGloves(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    // ============================================
    // WOODSHAPING ITEMS (Druid)
    // ============================================

    #region Woodshaping Tool

    public class WoodshapingKit : Item, ITool
    {
        private int m_UsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; InvalidateProperties(); }
        }

        public bool ShowUsesRemaining { get { return true; } set { } }
        public CraftSystem CraftSystem => DefWoodshaping.CraftSystem;
        public bool BreakOnDepletion => true;

        [Constructable]
        public WoodshapingKit() : this(50) { }

        [Constructable]
        public WoodshapingKit(int uses) : base(0x1028) // Carpentry tools
        {
            Name = "Woodshaping Kit";
            Hue = 1266; // Forest green
            Weight = 3.0;
            m_UsesRemaining = uses;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060584, m_UsesRemaining.ToString());
            list.Add("Uses Carpentry skill");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack)) { from.SendLocalizedMessage(1042001); return; }
            if (m_UsesRemaining <= 0) { from.SendMessage(0x22, "This kit is worn out."); return; }
            from.SendGump(new CraftGump(from, CraftSystem, this, null));
        }

        public bool CheckAccessible(Mobile from, ref int num)
        {
            if (RootParent != from) { num = 1044263; return false; }
            return true;
        }

        public WoodshapingKit(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); writer.Write(m_UsesRemaining); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); m_UsesRemaining = reader.ReadInt(); }
    }

    #endregion

    #region Druid Staves and Totems

    public class DruidStaff : BlackStaff
    {
        [Constructable]
        public DruidStaff()
        {
            Name = "Druid's Staff";
            Hue = 1266;
            Attributes.BonusInt = 5;
            Attributes.RegenMana = 2;
            Attributes.SpellDamage = 5;
        }

        public DruidStaff(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class NatureTotem : Item
    {
        [Constructable]
        public NatureTotem() : base(0x12CA) // Totem graphic
        {
            Name = "Nature Totem";
            Hue = 1266;
            Weight = 5.0;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack)) { from.SendLocalizedMessage(1042001); return; }

            from.PlaySound(0x1F7);
            Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, 1266, 0, 5005, 0);
            from.SendMessage(0x3B2, "The nature totem pulses with life energy!");

            // Heal nearby friendly creatures
            foreach (Mobile m in from.GetMobilesInRange(5))
            {
                if (m != from && from.CanBeBeneficial(m))
                {
                    m.Heal(Utility.RandomMinMax(10, 25));
                    m.SendMessage(0x3B2, "You feel nature's embrace!");
                }
            }
        }

        public NatureTotem(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class GreaterNatureTotem : Item
    {
        private int m_Charges;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges { get { return m_Charges; } set { m_Charges = value; InvalidateProperties(); } }

        [Constructable]
        public GreaterNatureTotem() : base(0x12CB) // Larger totem
        {
            Name = "Greater Nature Totem";
            Hue = 1272; // Bright green
            Weight = 8.0;
            m_Charges = 10;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060741, m_Charges.ToString());
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack)) { from.SendLocalizedMessage(1042001); return; }
            if (m_Charges <= 0) { from.SendMessage(0x22, "The totem has lost its power."); return; }

            m_Charges--;
            from.PlaySound(0x1F7);
            Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, 1272, 0, 5005, 0);

            // Powerful heal + regen buff
            foreach (Mobile m in from.GetMobilesInRange(8))
            {
                if (from.CanBeBeneficial(m))
                {
                    m.Heal(Utility.RandomMinMax(30, 50));
                    m.AddStatMod(new StatMod(StatType.Str, "NatureTotemRegen", 5, TimeSpan.FromMinutes(5)));
                    m.SendMessage(0x3B2, "Nature's power flows through you!");
                }
            }

            InvalidateProperties();
        }

        public GreaterNatureTotem(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); writer.Write(m_Charges); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); m_Charges = reader.ReadInt(); }
    }

    #endregion

    // ============================================
    // CLOTHCRAFT ITEMS (Bard)
    // ============================================

    #region Clothcraft Tool

    public class ClothcraftKit : Item, ITool
    {
        private int m_UsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; InvalidateProperties(); }
        }

        public bool ShowUsesRemaining { get { return true; } set { } }
        public CraftSystem CraftSystem => DefClothcraft.CraftSystem;
        public bool BreakOnDepletion => true;

        [Constructable]
        public ClothcraftKit() : this(50) { }

        [Constructable]
        public ClothcraftKit(int uses) : base(0x0F9D) // Sewing kit
        {
            Name = "Clothcraft Kit";
            Hue = 1153; // Purple - bardic
            Weight = 2.0;
            m_UsesRemaining = uses;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060584, m_UsesRemaining.ToString());
            list.Add("Uses Tailoring skill");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack)) { from.SendLocalizedMessage(1042001); return; }
            if (m_UsesRemaining <= 0) { from.SendMessage(0x22, "This kit is worn out."); return; }
            from.SendGump(new CraftGump(from, CraftSystem, this, null));
        }

        public bool CheckAccessible(Mobile from, ref int num)
        {
            if (RootParent != from) { num = 1044263; return false; }
            return true;
        }

        public ClothcraftKit(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); writer.Write(m_UsesRemaining); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); m_UsesRemaining = reader.ReadInt(); }
    }

    #endregion

    #region Bard Clothing

    public class BardicRobe : Robe
    {
        [Constructable]
        public BardicRobe()
        {
            Name = "Bardic Robe";
            Hue = 1153; // Purple
            Attributes.BonusInt = 5;
            Attributes.RegenMana = 2;
        }

        public BardicRobe(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class BardicCloak : Cloak
    {
        [Constructable]
        public BardicCloak()
        {
            Name = "Bardic Cloak";
            Hue = 1153;
            Attributes.BonusDex = 3;
            Attributes.DefendChance = 5;
        }

        public BardicCloak(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class MaestroRobe : Robe
    {
        [Constructable]
        public MaestroRobe()
        {
            Name = "Maestro's Robe";
            Hue = 1159; // Gold
            Attributes.BonusInt = 8;
            Attributes.RegenMana = 3;
            Attributes.CastSpeed = 1;
            LootType = LootType.Blessed;
        }

        public MaestroRobe(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class BardicCap : FeatheredHat
    {
        [Constructable]
        public BardicCap()
        {
            Name = "Bardic Cap";
            Hue = 1153;
            Attributes.BonusInt = 3;
        }

        public BardicCap(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    // ============================================
    // NECROCRAFT ITEMS (Necromancer)
    // ============================================

    #region Necrocraft Tool

    public class NecrocraftKit : Item, ITool
    {
        private int m_UsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; InvalidateProperties(); }
        }

        public bool ShowUsesRemaining { get { return true; } set { } }
        public CraftSystem CraftSystem => DefNecrocraft.CraftSystem;
        public bool BreakOnDepletion => true;

        [Constructable]
        public NecrocraftKit() : this(50) { }

        [Constructable]
        public NecrocraftKit(int uses) : base(0x0F64) // Skull graphic
        {
            Name = "Necrocraft Kit";
            Hue = 1109; // Dark gray/bone
            Weight = 3.0;
            m_UsesRemaining = uses;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060584, m_UsesRemaining.ToString());
            list.Add("Uses Necromancy skill");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack)) { from.SendLocalizedMessage(1042001); return; }
            if (m_UsesRemaining <= 0) { from.SendMessage(0x22, "This kit is worn out."); return; }
            from.SendGump(new CraftGump(from, CraftSystem, this, null));
        }

        public bool CheckAccessible(Mobile from, ref int num)
        {
            if (RootParent != from) { num = 1044263; return false; }
            return true;
        }

        public NecrocraftKit(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); writer.Write(m_UsesRemaining); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); m_UsesRemaining = reader.ReadInt(); }
    }

    #endregion

    #region Necromancer Items

    public class BoneWand : Item
    {
        [Constructable]
        public BoneWand() : base(0x0DF2) // Wand graphic
        {
            Name = "Bone Wand";
            Hue = 1109;
            Weight = 1.0;
            Layer = Layer.OneHanded;
        }

        public BoneWand(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class SoulVessel : Item
    {
        private int m_Souls;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Souls { get { return m_Souls; } set { m_Souls = value; InvalidateProperties(); } }

        [Constructable]
        public SoulVessel() : base(0x0E26) // Jar graphic
        {
            Name = "Soul Vessel";
            Hue = 1175; // Dark purple
            Weight = 2.0;
            m_Souls = 0;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add($"Souls Captured: {m_Souls}");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack)) { from.SendLocalizedMessage(1042001); return; }

            if (m_Souls >= 10)
            {
                // Release souls for mana
                from.Mana = Math.Min(from.Mana + 50, from.ManaMax);
                m_Souls = 0;
                from.PlaySound(0x1F6);
                from.SendMessage(0x3B2, "You release the captured souls, gaining their energy!");
                InvalidateProperties();
            }
            else
            {
                from.SendMessage(0x3B2, $"The vessel contains {m_Souls} souls. Collect 10 to release their energy.");
            }
        }

        public SoulVessel(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); writer.Write(m_Souls); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); m_Souls = reader.ReadInt(); }
    }

    public class BoneArmor : BoneChest
    {
        [Constructable]
        public BoneArmor()
        {
            Name = "Necromancer's Bone Armor";
            Hue = 1109;
            Attributes.BonusInt = 5;
            Attributes.RegenMana = 2;
        }

        public BoneArmor(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class SkullStaff : GnarledStaff
    {
        [Constructable]
        public SkullStaff()
        {
            Name = "Skull Staff";
            Hue = 1109;
            Attributes.BonusInt = 8;
            Attributes.SpellDamage = 10;
            WeaponAttributes.HitLeechMana = 25;
        }

        public SkullStaff(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    // ============================================
    // JEWELCRAFT ITEMS (Sorcerer)
    // ============================================

    #region Jewelcraft Tool

    public class JewelcraftKit : Item, ITool
    {
        private int m_UsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; InvalidateProperties(); }
        }

        public bool ShowUsesRemaining { get { return true; } set { } }
        public CraftSystem CraftSystem => DefJewelcraft.CraftSystem;
        public bool BreakOnDepletion => true;

        [Constructable]
        public JewelcraftKit() : this(50) { }

        [Constructable]
        public JewelcraftKit(int uses) : base(0x1EBC) // Gem graphic
        {
            Name = "Jewelcraft Kit";
            Hue = 1175; // Fire orange - ember sorcerer
            Weight = 2.0;
            m_UsesRemaining = uses;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060584, m_UsesRemaining.ToString());
            list.Add("Uses Tinkering skill");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack)) { from.SendLocalizedMessage(1042001); return; }
            if (m_UsesRemaining <= 0) { from.SendMessage(0x22, "This kit is worn out."); return; }
            from.SendGump(new CraftGump(from, CraftSystem, this, null));
        }

        public bool CheckAccessible(Mobile from, ref int num)
        {
            if (RootParent != from) { num = 1044263; return false; }
            return true;
        }

        public JewelcraftKit(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); writer.Write(m_UsesRemaining); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); m_UsesRemaining = reader.ReadInt(); }
    }

    #endregion

    #region Sorcerer Jewelry

    public class ElementalRing : GoldRing
    {
        [Constructable]
        public ElementalRing()
        {
            Name = "Elemental Ring";
            Hue = 1175;
            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 5;
            Resistances.Fire = 5;
        }

        public ElementalRing(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class SorcerersBracelet : GoldBracelet
    {
        [Constructable]
        public SorcerersBracelet()
        {
            Name = "Sorcerer's Bracelet";
            Hue = 1175;
            Attributes.BonusInt = 5;
            Attributes.RegenMana = 2;
            Attributes.LowerManaCost = 5;
        }

        public SorcerersBracelet(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class GreaterElementalRing : GoldRing
    {
        [Constructable]
        public GreaterElementalRing()
        {
            Name = "Greater Elemental Ring";
            Hue = 1161; // Bright red
            Attributes.BonusInt = 8;
            Attributes.SpellDamage = 10;
            Resistances.Fire = 10;
            Resistances.Cold = 10;
            Resistances.Energy = 10;
            LootType = LootType.Blessed;
        }

        public GreaterElementalRing(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class MasterSorcerersBracelet : GoldBracelet
    {
        [Constructable]
        public MasterSorcerersBracelet()
        {
            Name = "Master Sorcerer's Bracelet";
            Hue = 1159; // Gold
            Attributes.BonusInt = 10;
            Attributes.RegenMana = 4;
            Attributes.LowerManaCost = 10;
            Attributes.CastSpeed = 1;
            LootType = LootType.Blessed;
        }

        public MasterSorcerersBracelet(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class SpellFocus : Item
    {
        [Constructable]
        public SpellFocus() : base(0x0E2E) // Crystal ball
        {
            Name = "Spell Focus";
            Hue = 1175;
            Weight = 2.0;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack)) { from.SendLocalizedMessage(1042001); return; }

            from.PlaySound(0x1F5);
            from.AddStatMod(new StatMod(StatType.Int, "SpellFocus", 10, TimeSpan.FromMinutes(5)));
            from.SendMessage(0x3B2, "The focus crystal enhances your magical power!");
        }

        public SpellFocus(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion
}
