using System;

namespace Server.Items
{
    // ============================================
    // DIVINATION MAGIC REAGENTS
    // ============================================
    // Used by Divination  spells
    // Total: 8 reagents
    // ============================================

    #region TimeSand (Divination Magic (Circles 1-3))
    /// <summary>
    /// TimeSand - Sand that flows through time
    /// Found in: Crystal Barrens
    /// Used in: Divination Magic (Circles 1-3)
    /// </summary>
    public class TimeSand : BaseVystiaReagent
    {
        [Constructable]
        public TimeSand() : this(1) { }

        [Constructable]
        public TimeSand(int amount)
            : base(amount, 0x0F8F, 0, "Crystal Barrens", "Divination Magic (Circles 1-3)")
        {
            Weight = 0.1;
        }

        public TimeSand(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "time sand"; } }

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
    #region TimeDust (Divination Magic (Circles 2-4))
    /// <summary>
    /// TimeDust - Dust of past and future
    /// Found in: Crystal Barrens
    /// Used in: Divination Magic (Circles 2-4)
    /// </summary>
    public class TimeDust : BaseVystiaReagent
    {
        [Constructable]
        public TimeDust() : this(1) { }

        [Constructable]
        public TimeDust(int amount)
            : base(amount, 0x0F7D, 0, "Crystal Barrens", "Divination Magic (Circles 2-4)")
        {
            Weight = 0.1;
        }

        public TimeDust(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "time dust"; } }

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
    #region DivinationDust (Divination Magic (Circles 3-5))
    /// <summary>
    /// DivinationDust - Dust for seeing truth
    /// Found in: Crystal Barrens
    /// Used in: Divination Magic (Circles 3-5)
    /// </summary>
    public class DivinationDust : BaseVystiaReagent
    {
        [Constructable]
        public DivinationDust() : this(1) { }

        [Constructable]
        public DivinationDust(int amount)
            : base(amount, 0x0DE1, 0, "Crystal Barrens", "Divination Magic (Circles 3-5)")
        {
            Weight = 0.1;
        }

        public DivinationDust(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "divination dust"; } }

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
    #region FateCrystal (Divination Magic (Circles 4-6))
    /// <summary>
    /// FateCrystal - Crystal showing fate
    /// Found in: Crystal Barrens
    /// Used in: Divination Magic (Circles 4-6)
    /// </summary>
    public class FateCrystal : BaseVystiaReagent
    {
        [Constructable]
        public FateCrystal() : this(1) { }

        [Constructable]
        public FateCrystal(int amount)
            : base(amount, 0x0F8E, 0, "Crystal Barrens", "Divination Magic (Circles 4-6)")
        {
            Weight = 0.1;
        }

        public FateCrystal(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "fate crystal"; } }

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
    #region StarlightCrystal (Divination Magic (Circles 5-7))
    /// <summary>
    /// StarlightCrystal - Crystal of starlight
    /// Found in: Crystal Barrens
    /// Used in: Divination Magic (Circles 5-7)
    /// </summary>
    public class StarlightCrystal : BaseVystiaReagent
    {
        [Constructable]
        public StarlightCrystal() : this(1) { }

        [Constructable]
        public StarlightCrystal(int amount)
            : base(amount, 0x0F0E, 0, "Crystal Barrens", "Divination Magic (Circles 5-7)")
        {
            Weight = 0.1;
        }

        public StarlightCrystal(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "starlight crystal"; } }

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
    #region PropheticLeaf (Divination Magic (Circles 6-8))
    /// <summary>
    /// PropheticLeaf - Leaf showing prophecies
    /// Found in: Crystal Barrens
    /// Used in: Divination Magic (Circles 6-8)
    /// </summary>
    public class PropheticLeaf : BaseVystiaReagent
    {
        [Constructable]
        public PropheticLeaf() : this(1) { }

        [Constructable]
        public PropheticLeaf(int amount)
            : base(amount, 0x1A9C, 0, "Crystal Barrens", "Divination Magic (Circles 6-8)")
        {
            Weight = 0.1;
        }

        public PropheticLeaf(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "prophetic leaf"; } }

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
    #region SeeingStone (Divination Magic (Circles 7-8))
    /// <summary>
    /// SeeingStone - Stone for scrying
    /// Found in: Crystal Barrens
    /// Used in: Divination Magic (Circles 7-8)
    /// </summary>
    public class SeeingStone : BaseVystiaReagent
    {
        [Constructable]
        public SeeingStone() : this(1) { }

        [Constructable]
        public SeeingStone(int amount)
            : base(amount, 0x0F7A, 0, "Crystal Barrens", "Divination Magic (Circles 7-8)")
        {
            Weight = 0.1;
        }

        public SeeingStone(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "seeing stone"; } }

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
    #region FateThread (Divination Magic (Circle 8))
    /// <summary>
    /// FateThread - Thread of destiny
    /// Found in: Crystal Barrens
    /// Used in: Divination Magic (Circle 8)
    /// </summary>
    public class FateThread : BaseVystiaReagent
    {
        [Constructable]
        public FateThread() : this(1) { }

        [Constructable]
        public FateThread(int amount)
            : base(amount, 0x0F8D, 0, "Crystal Barrens", "Divination Magic (Circle 8)")
        {
            Weight = 0.1;
        }

        public FateThread(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "fate thread"; } }

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
