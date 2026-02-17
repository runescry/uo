using System;

namespace Server.Items
{
    // ============================================
    // ILLUSION MAGIC REAGENTS
    // ============================================
    // Used by Illusion  spells
    // Total: 8 reagents
    // ============================================

    #region MirrorDust (Illusion Magic (Circles 1-3))
    /// <summary>
    /// MirrorDust - Dust from mirrors
    /// Found in: Desert mirages
    /// Used in: Illusion Magic (Circles 1-3)
    /// </summary>
    public class MirrorDust : BaseVystiaReagent
    {
        [Constructable]
        public MirrorDust() : this(1) { }

        [Constructable]
        public MirrorDust(int amount)
            : base(amount, 0x0F8F, 0, "Desert mirages", "Illusion Magic (Circles 1-3)")
        {
            Weight = 0.1;
        }

        public MirrorDust(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "mirror dust"; } }

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
    #region PhantomSilk (Illusion Magic (Circles 2-4))
    /// <summary>
    /// PhantomSilk - Silk from phantoms
    /// Found in: Desert mirages
    /// Used in: Illusion Magic (Circles 2-4)
    /// </summary>
    public class PhantomSilk : BaseVystiaReagent
    {
        [Constructable]
        public PhantomSilk() : this(1) { }

        [Constructable]
        public PhantomSilk(int amount)
            : base(amount, 0x0F8D, 0, "Desert mirages", "Illusion Magic (Circles 2-4)")
        {
            Weight = 0.1;
        }

        public PhantomSilk(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "phantom silk"; } }

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
    #region MirageEssence (Illusion Magic (Circles 3-5))
    /// <summary>
    /// MirageEssence - Essence of mirages
    /// Found in: Desert mirages
    /// Used in: Illusion Magic (Circles 3-5)
    /// </summary>
    public class MirageEssence : BaseVystiaReagent
    {
        [Constructable]
        public MirageEssence() : this(1) { }

        [Constructable]
        public MirageEssence(int amount)
            : base(amount, 0x0F0E, 0, "Desert mirages", "Illusion Magic (Circles 3-5)")
        {
            Weight = 0.1;
        }

        public MirageEssence(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "mirage essence"; } }

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
    #region DreamCrystal (Illusion Magic (Circles 4-6))
    /// <summary>
    /// DreamCrystal - Crystal from dreams
    /// Found in: Desert mirages
    /// Used in: Illusion Magic (Circles 4-6)
    /// </summary>
    public class DreamCrystal : BaseVystiaReagent
    {
        [Constructable]
        public DreamCrystal() : this(1) { }

        [Constructable]
        public DreamCrystal(int amount)
            : base(amount, 0x0F8E, 0, "Desert mirages", "Illusion Magic (Circles 4-6)")
        {
            Weight = 0.1;
        }

        public DreamCrystal(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "dream crystal"; } }

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
    #region RealitySplinter (Illusion Magic (Circles 5-7))
    /// <summary>
    /// RealitySplinter - Splinter of reality
    /// Found in: Desert mirages
    /// Used in: Illusion Magic (Circles 5-7)
    /// </summary>
    public class RealitySplinter : BaseVystiaReagent
    {
        [Constructable]
        public RealitySplinter() : this(1) { }

        [Constructable]
        public RealitySplinter(int amount)
            : base(amount, 0x0F8A, 0, "Desert mirages", "Illusion Magic (Circles 5-7)")
        {
            Weight = 0.1;
        }

        public RealitySplinter(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "reality splinter"; } }

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
    #region VoidMirror (Illusion Magic (Circles 6-8))
    /// <summary>
    /// VoidMirror - Mirror showing void
    /// Found in: Desert mirages
    /// Used in: Illusion Magic (Circles 6-8)
    /// </summary>
    public class VoidMirror : BaseVystiaReagent
    {
        [Constructable]
        public VoidMirror() : this(1) { }

        [Constructable]
        public VoidMirror(int amount)
            : base(amount, 0x0F7A, 0, "Desert mirages", "Illusion Magic (Circles 6-8)")
        {
            Weight = 0.1;
        }

        public VoidMirror(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "void mirror"; } }

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
    #region ChaosPrism (Illusion Magic (Circles 7-8))
    /// <summary>
    /// ChaosPrism - Prism of chaos
    /// Found in: Desert mirages
    /// Used in: Illusion Magic (Circles 7-8)
    /// </summary>
    public class ChaosPrism : BaseVystiaReagent
    {
        [Constructable]
        public ChaosPrism() : this(1) { }

        [Constructable]
        public ChaosPrism(int amount)
            : base(amount, 0x1C18, 0, "Desert mirages", "Illusion Magic (Circles 7-8)")
        {
            Weight = 0.1;
        }

        public ChaosPrism(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "chaos prism"; } }

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
    #region PhantomPetal (Illusion Magic (Circle 8))
    /// <summary>
    /// PhantomPetal - Petal from illusion realm
    /// Found in: Desert mirages
    /// Used in: Illusion Magic (Circle 8)
    /// </summary>
    public class PhantomPetal : BaseVystiaReagent
    {
        [Constructable]
        public PhantomPetal() : this(1) { }

        [Constructable]
        public PhantomPetal(int amount)
            : base(amount, 0x0F86, 0, "Desert mirages", "Illusion Magic (Circle 8)")
        {
            Weight = 0.1;
        }

        public PhantomPetal(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "phantom petal"; } }

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
