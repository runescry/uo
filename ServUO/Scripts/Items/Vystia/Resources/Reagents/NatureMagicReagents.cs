using System;

namespace Server.Items
{
    // ============================================
    // NATURE MAGIC REAGENTS
    // ============================================
    // Used by Nature  spells
    // Total: 8 reagents
    // ============================================

    #region WildMoss (Nature Magic (Circles 1-3))
    /// <summary>
    /// WildMoss - Moss from ancient forests
    /// Found in: Verdantpeak forests
    /// Used in: Nature Magic (Circles 1-3)
    /// </summary>
    public class WildMoss : BaseVystiaReagent
    {
        [Constructable]
        public WildMoss() : this(1) { }

        [Constructable]
        public WildMoss(int amount)
            : base(amount, 0x0F7B, 0, "Verdantpeak forests", "Nature Magic (Circles 1-3)")
        {
            Weight = 0.1;
        }

        public WildMoss(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "wild moss"; } }

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
    #region Moonpetal (Nature Magic (Circles 2-4))
    /// <summary>
    /// Moonpetal - Flower that blooms under moonlight
    /// Found in: Verdantpeak forests
    /// Used in: Nature Magic (Circles 2-4)
    /// </summary>
    public class Moonpetal : BaseVystiaReagent
    {
        [Constructable]
        public Moonpetal() : this(1) { }

        [Constructable]
        public Moonpetal(int amount)
            : base(amount, 0x0F86, 0, "Verdantpeak forests", "Nature Magic (Circles 2-4)")
        {
            Weight = 0.1;
        }

        public Moonpetal(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "moonpetal"; } }

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
    #region DruidBark (Nature Magic (Circles 3-5))
    /// <summary>
    /// DruidBark - Bark from sacred druid groves
    /// Found in: Verdantpeak forests
    /// Used in: Nature Magic (Circles 3-5)
    /// </summary>
    public class DruidBark : BaseVystiaReagent
    {
        [Constructable]
        public DruidBark() : this(1) { }

        [Constructable]
        public DruidBark(int amount)
            : base(amount, 0x0DE1, 0, "Verdantpeak forests", "Nature Magic (Circles 3-5)")
        {
            Weight = 0.1;
        }

        public DruidBark(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "druid bark"; } }

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
    #region TreantSap (Nature Magic (Circles 4-6))
    /// <summary>
    /// TreantSap - Sap from living treants
    /// Found in: Verdantpeak forests
    /// Used in: Nature Magic (Circles 4-6)
    /// </summary>
    public class TreantSap : BaseVystiaReagent
    {
        [Constructable]
        public TreantSap() : this(1) { }

        [Constructable]
        public TreantSap(int amount)
            : base(amount, 0x1C18, 0, "Verdantpeak forests", "Nature Magic (Circles 4-6)")
        {
            Weight = 0.1;
        }

        public TreantSap(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "treant sap"; } }

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
    #region ElderwoodSeed (Nature Magic (Circles 5-7))
    /// <summary>
    /// ElderwoodSeed - Seed from eldest trees
    /// Found in: Verdantpeak forests
    /// Used in: Nature Magic (Circles 5-7)
    /// </summary>
    public class ElderwoodSeed : BaseVystiaReagent
    {
        [Constructable]
        public ElderwoodSeed() : this(1) { }

        [Constructable]
        public ElderwoodSeed(int amount)
            : base(amount, 0x1422, 0, "Verdantpeak forests", "Nature Magic (Circles 5-7)")
        {
            Weight = 0.1;
        }

        public ElderwoodSeed(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "elderwood seed"; } }

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
    #region PrimalVine (Nature Magic (Circles 6-8))
    /// <summary>
    /// PrimalVine - Vine with primal life force
    /// Found in: Verdantpeak forests
    /// Used in: Nature Magic (Circles 6-8)
    /// </summary>
    public class PrimalVine : BaseVystiaReagent
    {
        [Constructable]
        public PrimalVine() : this(1) { }

        [Constructable]
        public PrimalVine(int amount)
            : base(amount, 0x1A9C, 0, "Verdantpeak forests", "Nature Magic (Circles 6-8)")
        {
            Weight = 0.1;
        }

        public PrimalVine(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "primal vine"; } }

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
    #region LivingBark (Nature Magic (Circles 7-8))
    /// <summary>
    /// LivingBark - Bark that lives without tree
    /// Found in: Verdantpeak forests
    /// Used in: Nature Magic (Circles 7-8)
    /// </summary>
    public class LivingBark : BaseVystiaReagent
    {
        [Constructable]
        public LivingBark() : this(1) { }

        [Constructable]
        public LivingBark(int amount)
            : base(amount, 0x0F78, 0, "Verdantpeak forests", "Nature Magic (Circles 7-8)")
        {
            Weight = 0.1;
        }

        public LivingBark(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "living bark"; } }

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
    #region AncientRoot (Nature Magic (Circle 8))
    /// <summary>
    /// AncientRoot - Root from eldest tree
    /// Found in: Verdantpeak forests
    /// Used in: Nature Magic (Circle 8)
    /// </summary>
    public class AncientRoot : BaseVystiaReagent
    {
        [Constructable]
        public AncientRoot() : this(1) { }

        [Constructable]
        public AncientRoot(int amount)
            : base(amount, 0x0F7A, 0, "Verdantpeak forests", "Nature Magic (Circle 8)")
        {
            Weight = 0.1;
        }

        public AncientRoot(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "ancient root"; } }

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
