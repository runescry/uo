using System;

namespace Server.Items
{
    // ============================================
    // SUMMONING MAGIC REAGENTS
    // ============================================
    // Used by Summoning  spells
    // Total: 8 reagents
    // ============================================

    #region PlanarDust (Summoning Magic (Circles 1-3))
    /// <summary>
    /// PlanarDust - Dust from other planes
    /// Found in: Underwater depths
    /// Used in: Summoning Magic (Circles 1-3)
    /// </summary>
    public class PlanarDust : BaseVystiaReagent
    {
        [Constructable]
        public PlanarDust() : this(1) { }

        [Constructable]
        public PlanarDust(int amount)
            : base(amount, 0x0F8F, 0, "Underwater depths", "Summoning Magic (Circles 1-3)")
        {
            Weight = 0.1;
        }

        public PlanarDust(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "planar dust"; } }

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
    #region EtherShard (Summoning Magic (Circles 2-4))
    /// <summary>
    /// EtherShard - Shard from ethereal realm
    /// Found in: Underwater depths
    /// Used in: Summoning Magic (Circles 2-4)
    /// </summary>
    public class EtherShard : BaseVystiaReagent
    {
        [Constructable]
        public EtherShard() : this(1) { }

        [Constructable]
        public EtherShard(int amount)
            : base(amount, 0x0F8A, 0, "Underwater depths", "Summoning Magic (Circles 2-4)")
        {
            Weight = 0.1;
        }

        public EtherShard(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "ether shard"; } }

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
    #region AetherShard (Summoning Magic (Circles 3-5))
    /// <summary>
    /// AetherShard - Shard from aether plane
    /// Found in: Underwater depths
    /// Used in: Summoning Magic (Circles 3-5)
    /// </summary>
    public class AetherShard : BaseVystiaReagent
    {
        [Constructable]
        public AetherShard() : this(1) { }

        [Constructable]
        public AetherShard(int amount)
            : base(amount, 0x0F7D, 0, "Underwater depths", "Summoning Magic (Circles 3-5)")
        {
            Weight = 0.1;
        }

        public AetherShard(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "aether shard"; } }

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
    #region SummoningCrystal (Summoning Magic (Circles 4-6))
    /// <summary>
    /// SummoningCrystal - Crystal for summoning
    /// Found in: Underwater depths
    /// Used in: Summoning Magic (Circles 4-6)
    /// </summary>
    public class SummoningCrystal : BaseVystiaReagent
    {
        [Constructable]
        public SummoningCrystal() : this(1) { }

        [Constructable]
        public SummoningCrystal(int amount)
            : base(amount, 0x0F8E, 0, "Underwater depths", "Summoning Magic (Circles 4-6)")
        {
            Weight = 0.1;
        }

        public SummoningCrystal(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "summoning crystal"; } }

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
    #region ChaosShard (Summoning Magic (Circles 5-7))
    /// <summary>
    /// ChaosShard - Shard of chaos realm
    /// Found in: Underwater depths
    /// Used in: Summoning Magic (Circles 5-7)
    /// </summary>
    public class ChaosShard : BaseVystiaReagent
    {
        [Constructable]
        public ChaosShard() : this(1) { }

        [Constructable]
        public ChaosShard(int amount)
            : base(amount, 0x0DE1, 0, "Underwater depths", "Summoning Magic (Circles 5-7)")
        {
            Weight = 0.1;
        }

        public ChaosShard(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "chaos shard"; } }

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
    #region BindingRune (Summoning Magic (Circles 6-8))
    /// <summary>
    /// BindingRune - Rune for binding summoned
    /// Found in: Underwater depths
    /// Used in: Summoning Magic (Circles 6-8)
    /// </summary>
    public class BindingRune : BaseVystiaReagent
    {
        [Constructable]
        public BindingRune() : this(1) { }

        [Constructable]
        public BindingRune(int amount)
            : base(amount, 0x0F86, 0, "Underwater depths", "Summoning Magic (Circles 6-8)")
        {
            Weight = 0.1;
        }

        public BindingRune(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "binding rune"; } }

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
    #region DimensionalKey (Summoning Magic (Circles 7-8))
    /// <summary>
    /// DimensionalKey - Key to other dimensions
    /// Found in: Underwater depths
    /// Used in: Summoning Magic (Circles 7-8)
    /// </summary>
    public class DimensionalKey : BaseVystiaReagent
    {
        [Constructable]
        public DimensionalKey() : this(1) { }

        [Constructable]
        public DimensionalKey(int amount)
            : base(amount, 0x0F7A, 0, "Underwater depths", "Summoning Magic (Circles 7-8)")
        {
            Weight = 0.1;
        }

        public DimensionalKey(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "dimensional key"; } }

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
    #region SummoningSalt (Summoning Magic (Circle 8))
    /// <summary>
    /// SummoningSalt - Salt for summoning circles
    /// Found in: Underwater depths
    /// Used in: Summoning Magic (Circle 8)
    /// </summary>
    public class SummoningSalt : BaseVystiaReagent
    {
        [Constructable]
        public SummoningSalt() : this(1) { }

        [Constructable]
        public SummoningSalt(int amount)
            : base(amount, 0x1422, 0, "Underwater depths", "Summoning Magic (Circle 8)")
        {
            Weight = 0.1;
        }

        public SummoningSalt(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "summoning salt"; } }

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
