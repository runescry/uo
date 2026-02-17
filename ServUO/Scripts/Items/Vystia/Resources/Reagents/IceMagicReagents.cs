using System;

namespace Server.Items
{
    // ============================================
    // ICE MAGIC REAGENTS
    // ============================================
    // Used by Ice  spells
    // Total: 8 reagents
    // ============================================

    #region Frostbloom (Ice Magic (Circles 1-3))
    /// <summary>
    /// Frostbloom - Magical frozen flower
    /// Found in: Frosthold tundra
    /// Used in: Ice Magic (Circles 1-3)
    /// </summary>
    public class Frostbloom : BaseVystiaReagent
    {
        [Constructable]
        public Frostbloom() : this(1) { }

        [Constructable]
        public Frostbloom(int amount)
            : base(amount, 0x0F86, 0, "Frosthold tundra", "Ice Magic (Circles 1-3)")
        {
            Weight = 0.1;
        }

        public Frostbloom(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "frostbloom"; } }

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
    #region GlacierCrystal (Ice Magic (Circles 2-4))
    /// <summary>
    /// GlacierCrystal - Crystal formed from glacial ice
    /// Found in: Frosthold tundra
    /// Used in: Ice Magic (Circles 2-4)
    /// </summary>
    public class GlacierCrystal : BaseVystiaReagent
    {
        [Constructable]
        public GlacierCrystal() : this(1) { }

        [Constructable]
        public GlacierCrystal(int amount)
            : base(amount, 0x0F8E, 0, "Frosthold tundra", "Ice Magic (Circles 2-4)")
        {
            Weight = 0.1;
        }

        public GlacierCrystal(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "glacier crystal"; } }

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
    #region Winterleaf (Ice Magic (Circles 3-5))
    /// <summary>
    /// Winterleaf - Leaf from an eternal winter tree
    /// Found in: Frosthold tundra
    /// Used in: Ice Magic (Circles 3-5)
    /// </summary>
    public class Winterleaf : BaseVystiaReagent
    {
        [Constructable]
        public Winterleaf() : this(1) { }

        [Constructable]
        public Winterleaf(int amount)
            : base(amount, 0x1A9C, 0, "Frosthold tundra", "Ice Magic (Circles 3-5)")
        {
            Weight = 0.1;
        }

        public Winterleaf(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "winterleaf"; } }

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
    #region PermafrostEssence (Ice Magic (Circles 4-6))
    /// <summary>
    /// PermafrostEssence - Essence of永久冻土
    /// Found in: Frosthold tundra
    /// Used in: Ice Magic (Circles 4-6)
    /// </summary>
    public class PermafrostEssence : BaseVystiaReagent
    {
        [Constructable]
        public PermafrostEssence() : this(1) { }

        [Constructable]
        public PermafrostEssence(int amount)
            : base(amount, 0x0F0E, 0, "Frosthold tundra", "Ice Magic (Circles 4-6)")
        {
            Weight = 0.1;
        }

        public PermafrostEssence(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "permafrost essence"; } }

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
    #region ArcticPearl (Ice Magic (Circles 5-7))
    /// <summary>
    /// ArcticPearl - Pearl from frozen seas
    /// Found in: Frosthold tundra
    /// Used in: Ice Magic (Circles 5-7)
    /// </summary>
    public class ArcticPearl : BaseVystiaReagent
    {
        [Constructable]
        public ArcticPearl() : this(1) { }

        [Constructable]
        public ArcticPearl(int amount)
            : base(amount, 0x0F7A, 0, "Frosthold tundra", "Ice Magic (Circles 5-7)")
        {
            Weight = 0.1;
        }

        public ArcticPearl(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "arctic pearl"; } }

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
    #region FrozenSoul (Ice Magic (Circles 6-8))
    /// <summary>
    /// FrozenSoul - Captured soul of ice elemental
    /// Found in: Frosthold tundra
    /// Used in: Ice Magic (Circles 6-8)
    /// </summary>
    public class FrozenSoul : BaseVystiaReagent
    {
        [Constructable]
        public FrozenSoul() : this(1) { }

        [Constructable]
        public FrozenSoul(int amount)
            : base(amount, 0x0F7D, 0, "Frosthold tundra", "Ice Magic (Circles 6-8)")
        {
            Weight = 0.1;
        }

        public FrozenSoul(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "frozen soul"; } }

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
    #region FrostEssence (Ice Magic (Circles 1-8))
    /// <summary>
    /// FrostEssence - Pure essence of frost magic
    /// Found in: Frosthold tundra
    /// Used in: Ice Magic (Circles 1-8) - Used by 14 spells
    /// </summary>
    public class FrostEssence : BaseVystiaReagent
    {
        [Constructable]
        public FrostEssence() : this(1) { }

        [Constructable]
        public FrostEssence(int amount)
            : base(amount, 0x1C18, 0, "Frosthold tundra", "Ice Magic (Circles 1-8)")
        {
            Weight = 0.1;
        }

        public FrostEssence(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "frost essence"; } }

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
    #region HeartOfWinter (Ice Magic (Circle 8))
    /// <summary>
    /// HeartOfWinter - Heart of the winter itself
    /// Found in: Frosthold tundra
    /// Used in: Ice Magic (Circle 8)
    /// </summary>
    public class HeartOfWinter : BaseVystiaReagent
    {
        [Constructable]
        public HeartOfWinter() : this(1) { }

        [Constructable]
        public HeartOfWinter(int amount)
            : base(amount, 0x0F7B, 0, "Frosthold tundra", "Ice Magic (Circle 8)")
        {
            Weight = 0.1;
        }

        public HeartOfWinter(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "heart of winter"; } }

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
