using System;

namespace Server.Items
{
    // ============================================
    // DARK MAGIC REAGENTS
    // ============================================
    // Used by Dark  spells
    // Total: 8 reagents
    // ============================================

    #region ShadowMoss (Dark Magic (Circles 1-3))
    /// <summary>
    /// ShadowMoss - Moss from shadow realm
    /// Found in: ShadowVoid
    /// Used in: Dark Magic (Circles 1-3)
    /// </summary>
    public class ShadowMoss : BaseVystiaReagent
    {
        [Constructable]
        public ShadowMoss() : this(1) { }

        [Constructable]
        public ShadowMoss(int amount)
            : base(amount, 0x0F7B, 0, "ShadowVoid", "Dark Magic (Circles 1-3)")
        {
            Weight = 0.1;
        }

        public ShadowMoss(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "shadow moss"; } }

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
    #region VoidCrystal (Dark Magic (Circles 2-4))
    /// <summary>
    /// VoidCrystal - Crystal from the void
    /// Found in: ShadowVoid
    /// Used in: Dark Magic (Circles 2-4)
    /// </summary>
    public class VoidCrystal : BaseVystiaReagent
    {
        [Constructable]
        public VoidCrystal() : this(1) { }

        [Constructable]
        public VoidCrystal(int amount)
            : base(amount, 0x0F8E, 0, "ShadowVoid", "Dark Magic (Circles 2-4)")
        {
            Weight = 0.1;
        }

        public VoidCrystal(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "void crystal"; } }

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
    #region VoidWeed (Dark Magic (Circles 3-5))
    /// <summary>
    /// VoidWeed - Weed from emptiness
    /// Found in: ShadowVoid
    /// Used in: Dark Magic (Circles 3-5)
    /// </summary>
    public class VoidWeed : BaseVystiaReagent
    {
        [Constructable]
        public VoidWeed() : this(1) { }

        [Constructable]
        public VoidWeed(int amount)
            : base(amount, 0x1A9C, 0, "ShadowVoid", "Dark Magic (Circles 3-5)")
        {
            Weight = 0.1;
        }

        public VoidWeed(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "void weed"; } }

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
    #region ShadowPetal (Dark Magic (Circles 4-6))
    /// <summary>
    /// ShadowPetal - Petal from shadow flowers
    /// Found in: ShadowVoid
    /// Used in: Dark Magic (Circles 4-6)
    /// </summary>
    public class ShadowPetal : BaseVystiaReagent
    {
        [Constructable]
        public ShadowPetal() : this(1) { }

        [Constructable]
        public ShadowPetal(int amount)
            : base(amount, 0x0F86, 0, "ShadowVoid", "Dark Magic (Circles 4-6)")
        {
            Weight = 0.1;
        }

        public ShadowPetal(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "shadow petal"; } }

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
    #region VoidDust (Dark Magic (Circles 5-7))
    /// <summary>
    /// VoidDust - Dust of the void
    /// Found in: ShadowVoid
    /// Used in: Dark Magic (Circles 5-7)
    /// </summary>
    public class VoidDust : BaseVystiaReagent
    {
        [Constructable]
        public VoidDust() : this(1) { }

        [Constructable]
        public VoidDust(int amount)
            : base(amount, 0x0F8F, 0, "ShadowVoid", "Dark Magic (Circles 5-7)")
        {
            Weight = 0.1;
        }

        public VoidDust(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "void dust"; } }

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
    #region VoidSilk (Dark Magic (Circles 6-8))
    /// <summary>
    /// VoidSilk - Silk woven from darkness
    /// Found in: ShadowVoid
    /// Used in: Dark Magic (Circles 6-8)
    /// </summary>
    public class VoidSilk : BaseVystiaReagent
    {
        [Constructable]
        public VoidSilk() : this(1) { }

        [Constructable]
        public VoidSilk(int amount)
            : base(amount, 0x0F8D, 0, "ShadowVoid", "Dark Magic (Circles 6-8)")
        {
            Weight = 0.1;
        }

        public VoidSilk(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "void silk"; } }

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
    #region DemonHeart (Dark Magic (Circles 7-8))
    /// <summary>
    /// DemonHeart - Heart of shadow demon
    /// Found in: ShadowVoid
    /// Used in: Dark Magic (Circles 7-8)
    /// </summary>
    public class DemonHeart : BaseVystiaReagent
    {
        [Constructable]
        public DemonHeart() : this(1) { }

        [Constructable]
        public DemonHeart(int amount)
            : base(amount, 0x0F7A, 0, "ShadowVoid", "Dark Magic (Circles 7-8)")
        {
            Weight = 0.1;
        }

        public DemonHeart(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "demon heart"; } }

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
    #region ShadowEssence (Dark Magic (Circle 8))
    /// <summary>
    /// ShadowEssence - Pure essence of darkness
    /// Found in: ShadowVoid
    /// Used in: Dark Magic (Circle 8)
    /// </summary>
    public class ShadowEssence : BaseVystiaReagent
    {
        [Constructable]
        public ShadowEssence() : this(1) { }

        [Constructable]
        public ShadowEssence(int amount)
            : base(amount, 0x0F7D, 0, "ShadowVoid", "Dark Magic (Circle 8)")
        {
            Weight = 0.1;
        }

        public ShadowEssence(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "shadow essence"; } }

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
