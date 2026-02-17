using System;

namespace Server.Items
{
    // ============================================
    // HEX MAGIC REAGENTS
    // ============================================
    // Used by Hex  spells
    // Total: 8 reagents
    // ============================================

    #region BogMoss (Hex Magic (Circles 1-3))
    /// <summary>
    /// BogMoss - Moss from toxic bogs
    /// Found in: Shadowfen swamps
    /// Used in: Hex Magic (Circles 1-3)
    /// </summary>
    public class BogMoss : BaseVystiaReagent
    {
        [Constructable]
        public BogMoss() : this(1) { }

        [Constructable]
        public BogMoss(int amount)
            : base(amount, 0x0F7B, 0, "Shadowfen swamps", "Hex Magic (Circles 1-3)")
        {
            Weight = 0.1;
        }

        public BogMoss(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "bog moss"; } }

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
    #region ViperFang (Hex Magic (Circles 2-4))
    /// <summary>
    /// ViperFang - Fang of swamp viper
    /// Found in: Shadowfen swamps
    /// Used in: Hex Magic (Circles 2-4)
    /// </summary>
    public class ViperFang : BaseVystiaReagent
    {
        [Constructable]
        public ViperFang() : this(1) { }

        [Constructable]
        public ViperFang(int amount)
            : base(amount, 0x0F78, 0, "Shadowfen swamps", "Hex Magic (Circles 2-4)")
        {
            Weight = 0.1;
        }

        public ViperFang(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "viper fang"; } }

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
    #region Witchweed (Hex Magic (Circles 3-5))
    /// <summary>
    /// Witchweed - Weed used in witch rituals
    /// Found in: Shadowfen swamps
    /// Used in: Hex Magic (Circles 3-5)
    /// </summary>
    public class Witchweed : BaseVystiaReagent
    {
        [Constructable]
        public Witchweed() : this(1) { }

        [Constructable]
        public Witchweed(int amount)
            : base(amount, 0x1A9C, 0, "Shadowfen swamps", "Hex Magic (Circles 3-5)")
        {
            Weight = 0.1;
        }

        public Witchweed(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "witchweed"; } }

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
    #region ToadsEye (Hex Magic (Circles 4-6))
    /// <summary>
    /// ToadsEye - Eye of giant toad
    /// Found in: Shadowfen swamps
    /// Used in: Hex Magic (Circles 4-6)
    /// </summary>
    public class ToadsEye : BaseVystiaReagent
    {
        [Constructable]
        public ToadsEye() : this(1) { }

        [Constructable]
        public ToadsEye(int amount)
            : base(amount, 0x0F7A, 0, "Shadowfen swamps", "Hex Magic (Circles 4-6)")
        {
            Weight = 0.1;
        }

        public ToadsEye(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "toads eye"; } }

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
    #region SwampLotus (Hex Magic (Circles 5-7))
    /// <summary>
    /// SwampLotus - Rare lotus from swamps
    /// Found in: Shadowfen swamps
    /// Used in: Hex Magic (Circles 5-7)
    /// </summary>
    public class SwampLotus : BaseVystiaReagent
    {
        [Constructable]
        public SwampLotus() : this(1) { }

        [Constructable]
        public SwampLotus(int amount)
            : base(amount, 0x0F86, 0, "Shadowfen swamps", "Hex Magic (Circles 5-7)")
        {
            Weight = 0.1;
        }

        public SwampLotus(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "swamp lotus"; } }

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
    #region HagsHair (Hex Magic (Circles 6-8))
    /// <summary>
    /// HagsHair - Hair from swamp hag
    /// Found in: Shadowfen swamps
    /// Used in: Hex Magic (Circles 6-8)
    /// </summary>
    public class HagsHair : BaseVystiaReagent
    {
        [Constructable]
        public HagsHair() : this(1) { }

        [Constructable]
        public HagsHair(int amount)
            : base(amount, 0x1422, 0, "Shadowfen swamps", "Hex Magic (Circles 6-8)")
        {
            Weight = 0.1;
        }

        public HagsHair(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "hags hair"; } }

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
    #region CursedPearl (Hex Magic (Circles 7-8))
    /// <summary>
    /// CursedPearl - Pearl cursed by hex magic
    /// Found in: Shadowfen swamps
    /// Used in: Hex Magic (Circles 7-8)
    /// </summary>
    public class CursedPearl : BaseVystiaReagent
    {
        [Constructable]
        public CursedPearl() : this(1) { }

        [Constructable]
        public CursedPearl(int amount)
            : base(amount, 0x0F8E, 0, "Shadowfen swamps", "Hex Magic (Circles 7-8)")
        {
            Weight = 0.1;
        }

        public CursedPearl(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "cursed pearl"; } }

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
    #region CursedSalt (Hex Magic (Circle 8))
    /// <summary>
    /// CursedSalt - Salt used in dark curses
    /// Found in: Shadowfen swamps
    /// Used in: Hex Magic (Circle 8)
    /// </summary>
    public class CursedSalt : BaseVystiaReagent
    {
        [Constructable]
        public CursedSalt() : this(1) { }

        [Constructable]
        public CursedSalt(int amount)
            : base(amount, 0x0F8F, 0, "Shadowfen swamps", "Hex Magic (Circle 8)")
        {
            Weight = 0.1;
        }

        public CursedSalt(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "cursed salt"; } }

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
}
