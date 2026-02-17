/*
 * Vystia Crafting Tools
 * Regional variants of standard UO crafting tools
 * Each tool has a regional theme and enhanced uses
 */

using System;
using Server;
using Server.Items;
using Server.Engines.Craft;

namespace Server.Items.Vystia
{
    #region Smith Hammers

    /// <summary>
    /// Frosthold smith hammer - Enhanced durability
    /// </summary>
    public class FrostforgedSmithHammer : SmithHammer
    {
        [Constructable]
        public FrostforgedSmithHammer() : this(500)
        {
        }

        [Constructable]
        public FrostforgedSmithHammer(int uses) : base(uses)
        {
            Name = "Frostforged Smith Hammer";
            Hue = 0x480; // Ice blue
        }

        public FrostforgedSmithHammer(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    /// <summary>
    /// Emberlands smith hammer - Enhanced durability
    /// </summary>
    public class EmberforgedSmithHammer : SmithHammer
    {
        [Constructable]
        public EmberforgedSmithHammer() : this(500)
        {
        }

        [Constructable]
        public EmberforgedSmithHammer(int uses) : base(uses)
        {
            Name = "Emberforged Smith Hammer";
            Hue = 0x489; // Fire orange
        }

        public EmberforgedSmithHammer(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    /// <summary>
    /// Ironclad smith hammer - Steamwork enhanced
    /// </summary>
    public class SteamforgedSmithHammer : SmithHammer
    {
        [Constructable]
        public SteamforgedSmithHammer() : this(500)
        {
        }

        [Constructable]
        public SteamforgedSmithHammer(int uses) : base(uses)
        {
            Name = "Steamforged Smith Hammer";
            Hue = 0x3B2; // Bronze/brass
        }

        public SteamforgedSmithHammer(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Tinker Tools

    /// <summary>
    /// Frosthold tinker tools
    /// </summary>
    public class FrostforgedTinkerTools : TinkerTools
    {
        [Constructable]
        public FrostforgedTinkerTools() : this(500)
        {
        }

        [Constructable]
        public FrostforgedTinkerTools(int uses) : base(uses)
        {
            Name = "Frostforged Tinker Tools";
            Hue = 0x480; // Ice blue
        }

        public FrostforgedTinkerTools(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    /// <summary>
    /// Ironclad tinker tools - Clockwork themed
    /// </summary>
    public class ClockworkTinkerTools : TinkerTools
    {
        [Constructable]
        public ClockworkTinkerTools() : this(500)
        {
        }

        [Constructable]
        public ClockworkTinkerTools(int uses) : base(uses)
        {
            Name = "Clockwork Tinker Tools";
            Hue = 0x3B2; // Bronze/brass
        }

        public ClockworkTinkerTools(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Sewing Kits

    /// <summary>
    /// Verdantpeak sewing kit - Nature themed
    /// </summary>
    public class LivingVineSewingKit : SewingKit
    {
        [Constructable]
        public LivingVineSewingKit() : this(500)
        {
        }

        [Constructable]
        public LivingVineSewingKit(int uses) : base(uses)
        {
            Name = "Living Vine Sewing Kit";
            Hue = 0x59B; // Forest green
        }

        public LivingVineSewingKit(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    /// <summary>
    /// Desert sewing kit - Sandwalker themed
    /// </summary>
    public class SandsilkSewingKit : SewingKit
    {
        [Constructable]
        public SandsilkSewingKit() : this(500)
        {
        }

        [Constructable]
        public SandsilkSewingKit(int uses) : base(uses)
        {
            Name = "Sandsilk Sewing Kit";
            Hue = 0x719; // Sand/tan
        }

        public SandsilkSewingKit(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Carpenter Tools

    /// <summary>
    /// Verdantpeak carpenter saw - Living wood themed
    /// </summary>
    public class LivingwoodSaw : DovetailSaw
    {
        [Constructable]
        public LivingwoodSaw() : this(500)
        {
        }

        [Constructable]
        public LivingwoodSaw(int uses) : base(uses)
        {
            Name = "Livingwood Saw";
            Hue = 0x59B; // Forest green
        }

        public LivingwoodSaw(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    /// <summary>
    /// Frosthold carpenter saw
    /// </summary>
    public class FrostwillowSaw : DovetailSaw
    {
        [Constructable]
        public FrostwillowSaw() : this(500)
        {
        }

        [Constructable]
        public FrostwillowSaw(int uses) : base(uses)
        {
            Name = "Frostwillow Saw";
            Hue = 0x480; // Ice blue
        }

        public FrostwillowSaw(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Alchemy Tools

    /// <summary>
    /// Crystal Barrens mortar and pestle - Arcane themed
    /// </summary>
    public class CrystalMortarPestle : MortarPestle
    {
        [Constructable]
        public CrystalMortarPestle() : this(500)
        {
        }

        [Constructable]
        public CrystalMortarPestle(int uses) : base(uses)
        {
            Name = "Crystal Mortar & Pestle";
            Hue = 0x47E; // Crystal purple
        }

        public CrystalMortarPestle(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    /// <summary>
    /// Shadowfen mortar and pestle - Witch themed
    /// </summary>
    public class HexedMortarPestle : MortarPestle
    {
        [Constructable]
        public HexedMortarPestle() : this(500)
        {
        }

        [Constructable]
        public HexedMortarPestle(int uses) : base(uses)
        {
            Name = "Hexed Mortar & Pestle";
            Hue = 0x455; // Dark purple
        }

        public HexedMortarPestle(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Inscription Tools

    /// <summary>
    /// Oracle scribe pen - Divination themed
    /// </summary>
    public class OracleScribePen : ScribesPen
    {
        [Constructable]
        public OracleScribePen() : this(500)
        {
        }

        [Constructable]
        public OracleScribePen(int uses) : base(uses)
        {
            Name = "Oracle's Scribe Pen";
            Hue = 0x47E; // Crystal purple
        }

        public OracleScribePen(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    /// <summary>
    /// Enchanter scribe pen
    /// </summary>
    public class EnchantedScribePen : ScribesPen
    {
        [Constructable]
        public EnchantedScribePen() : this(500)
        {
        }

        [Constructable]
        public EnchantedScribePen(int uses) : base(uses)
        {
            Name = "Enchanted Scribe Pen";
            Hue = 0x501; // Gold
        }

        public EnchantedScribePen(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Cooking Tools

    /// <summary>
    /// Emberlands skillet - Fire themed
    /// </summary>
    public class EmberforgedSkillet : Skillet
    {
        [Constructable]
        public EmberforgedSkillet() : base()
        {
            Name = "Emberforged Skillet";
            Hue = 0x489; // Fire orange
        }

        public EmberforgedSkillet(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    /// <summary>
    /// Frosthold skillet
    /// </summary>
    public class FrostforgedSkillet : Skillet
    {
        [Constructable]
        public FrostforgedSkillet() : base()
        {
            Name = "Frostforged Skillet";
            Hue = 0x480; // Ice blue
        }

        public FrostforgedSkillet(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Tailoring Scissors

    /// <summary>
    /// Crystal scissors - Crystal Barrens themed
    /// </summary>
    public class CrystalScissors : Scissors
    {
        [Constructable]
        public CrystalScissors() : base()
        {
            Name = "Crystal Scissors";
            Hue = 0x47E; // Crystal purple
        }

        public CrystalScissors(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    /// <summary>
    /// Voidtouched scissors - ShadowVoid themed
    /// </summary>
    public class VoidtouchedScissors : Scissors
    {
        [Constructable]
        public VoidtouchedScissors() : base()
        {
            Name = "Voidtouched Scissors";
            Hue = 0x455; // Dark purple
        }

        public VoidtouchedScissors(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Artificer Tools

    /// <summary>
    /// Artificer's Precision Hammer - Gives Engineering skill bonus
    /// Used by Artificer class for mechanical crafting and combat
    /// </summary>
    [FlipableAttribute(0x13E3, 0x13E4)]
    public class ArtificerPrecisionHammer : BaseBashing, ITool
    {
        private int m_Bonus;
        private SkillMod m_SkillMod;

        [Constructable]
        public ArtificerPrecisionHammer(int bonus)
            : base(0x13E3)
        {
            m_Bonus = bonus;
            Weight = 8.0;
            ShowUsesRemaining = true;
            Hue = 2305; // Ironclad metallic hue
            Name = "Artificer's Precision Hammer";
        }

        public ArtificerPrecisionHammer(Serial serial)
            : base(serial)
        {
        }

        #region ITool Members
        public CraftSystem CraftSystem { get { return DefBlacksmithy.CraftSystem; } }
        public bool BreakOnDepletion { get { return true; } }

        public bool CheckAccessible(Mobile m, ref int num)
        {
            if (!IsChildOf(m) && Parent != m)
            {
                num = 1044263;
                return false;
            }

            return true;
        }
        #endregion

        [CommandProperty(AccessLevel.GameMaster)]
        public int Bonus
        {
            get
            {
                return m_Bonus;
            }
            set
            {
                m_Bonus = value;
                InvalidateProperties();

                if (m_Bonus == 0)
                {
                    if (m_SkillMod != null)
                        m_SkillMod.Remove();

                    m_SkillMod = null;
                }
                else if (m_SkillMod == null && Parent is Mobile)
                {
                    m_SkillMod = new DefaultSkillMod(SkillName.Engineering, true, m_Bonus);
                    ((Mobile)Parent).AddSkillMod(m_SkillMod);
                }
                else if (m_SkillMod != null)
                {
                    m_SkillMod.Value = m_Bonus;
                }
            }
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.CrushingBlow;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.ParalyzingBlow;
            }
        }
        public override int AosStrengthReq
        {
            get
            {
                return 5;
            }
        }
        public override int AosMinDamage
        {
            get
            {
                return 13;
            }
        }
        public override int AosMaxDamage
        {
            get
            {
                return 17;
            }
        }
        public override int AosSpeed
        {
            get
            {
                return 40;
            }
        }
        public override float MlSpeed
        {
            get
            {
                return 3.25f;
            }
        }
        public override int OldStrengthReq
        {
            get
            {
                return 5;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 31;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 70;
            }
        }

        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            if (m_Bonus != 0 && parent is Mobile)
            {
                if (m_SkillMod != null)
                    m_SkillMod.Remove();

                m_SkillMod = new DefaultSkillMod(SkillName.Engineering, true, m_Bonus);
                ((Mobile)parent).AddSkillMod(m_SkillMod);
            }
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            if (m_SkillMod != null)
                m_SkillMod.Remove();

            m_SkillMod = null;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Bonus != 0)
                list.Add(1060451, "#1042354\t{0}", m_Bonus.ToString()); // ~1_skillname~ +~2_val~
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.CraftSystem != null && (IsChildOf(from.Backpack) || Parent == from))
            {
                int num = this.CraftSystem.CanCraft(from, this, null);

                if (num > 0 && (num != 1044267 || !Core.SE))
                {
                    from.SendLocalizedMessage(num);
                }
                else
                {
                    CraftContext context = this.CraftSystem.GetContext(from);

                    from.SendGump(new CraftGump(from, this.CraftSystem, this, null));
                }
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((int)m_Bonus);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                case 0:
                    {
                        m_Bonus = reader.ReadInt();
                        break;
                    }
            }

            if (m_Bonus != 0 && Parent is Mobile)
            {
                if (m_SkillMod != null)
                    m_SkillMod.Remove();

                m_SkillMod = new DefaultSkillMod(SkillName.Engineering, true, m_Bonus);
                ((Mobile)Parent).AddSkillMod(m_SkillMod);
            }

            if (version == 0 && Hue == 0)
                Hue = 2305;
        }
    }

    #endregion
}
