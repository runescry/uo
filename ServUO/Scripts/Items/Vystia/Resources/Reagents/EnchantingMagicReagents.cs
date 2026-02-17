using System;

namespace Server.Items
{
    // ============================================
    // ENCHANTING MAGIC REAGENTS
    // ============================================
    // Used by Enchanting  spells
    // Total: 8 reagents
    // ============================================

    #region ArcaneDust (Enchanting Magic (Circles 1-3))
    /// <summary>
    /// ArcaneDust - Dust of arcane power
    /// Found in: Multi-regional
    /// Used in: Enchanting Magic (Circles 1-3)
    /// </summary>
    public class ArcaneDust : BaseVystiaReagent
    {
        [Constructable]
        public ArcaneDust() : this(1) { }

        [Constructable]
        public ArcaneDust(int amount)
            : base(amount, 0x0F8F, 0, "Multi-regional", "Enchanting Magic (Circles 1-3)")
        {
            Weight = 0.1;
        }

        public ArcaneDust(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "arcane dust"; } }

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
    #region EssenceOfMagic (Enchanting Magic (Circles 2-4))
    /// <summary>
    /// EssenceOfMagic - Pure magical essence
    /// Found in: Multi-regional
    /// Used in: Enchanting Magic (Circles 2-4)
    /// </summary>
    public class EssenceOfMagic : BaseVystiaReagent
    {
        [Constructable]
        public EssenceOfMagic() : this(1) { }

        [Constructable]
        public EssenceOfMagic(int amount)
            : base(amount, 0x1C18, 0, "Multi-regional", "Enchanting Magic (Circles 2-4)")
        {
            Weight = 0.1;
        }

        public EssenceOfMagic(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "essence of magic"; } }

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
    #region ManaCrystal (Enchanting Magic (Circles 3-5))
    /// <summary>
    /// ManaCrystal - Crystal of mana
    /// Found in: Multi-regional
    /// Used in: Enchanting Magic (Circles 3-5)
    /// </summary>
    public class ManaCrystal : BaseVystiaReagent
    {
        [Constructable]
        public ManaCrystal() : this(1) { }

        [Constructable]
        public ManaCrystal(int amount)
            : base(amount, 0x0F8E, 0, "Multi-regional", "Enchanting Magic (Circles 3-5)")
        {
            Weight = 0.1;
        }

        public ManaCrystal(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "mana crystal"; } }

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
    #region LeyLineEssence (Enchanting Magic (Circles 4-6))
    /// <summary>
    /// LeyLineEssence - Essence from ley lines
    /// Found in: Multi-regional
    /// Used in: Enchanting Magic (Circles 4-6)
    /// </summary>
    public class LeyLineEssence : BaseVystiaReagent
    {
        [Constructable]
        public LeyLineEssence() : this(1) { }

        [Constructable]
        public LeyLineEssence(int amount)
            : base(amount, 0x0F7D, 0, "Multi-regional", "Enchanting Magic (Circles 4-6)")
        {
            Weight = 0.1;
        }

        public LeyLineEssence(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "ley line essence"; } }

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
    #region LeyLineShard (Enchanting Magic (Circles 5-7))
    /// <summary>
    /// LeyLineShard - Shard from ley nexus
    /// Found in: Multi-regional
    /// Used in: Enchanting Magic (Circles 5-7)
    /// </summary>
    public class LeyLineShard : BaseVystiaReagent
    {
        [Constructable]
        public LeyLineShard() : this(1) { }

        [Constructable]
        public LeyLineShard(int amount)
            : base(amount, 0x0F8A, 0, "Multi-regional", "Enchanting Magic (Circles 5-7)")
        {
            Weight = 0.1;
        }

        public LeyLineShard(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "ley line shard"; } }

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
    #region RuneFragment (Enchanting Magic (Circles 6-8))
    /// <summary>
    /// RuneFragment - Fragment of power rune
    /// Found in: Multi-regional
    /// Used in: Enchanting Magic (Circles 6-8)
    /// </summary>
    public class RuneFragment : BaseVystiaReagent
    {
        [Constructable]
        public RuneFragment() : this(1) { }

        [Constructable]
        public RuneFragment(int amount)
            : base(amount, 0x0DE1, 0, "Multi-regional", "Enchanting Magic (Circles 6-8)")
        {
            Weight = 0.1;
        }

        public RuneFragment(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "rune fragment"; } }

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
    #region RunicPowder (Enchanting Magic (Circles 7-8))
    /// <summary>
    /// RunicPowder - Powder of runes
    /// Found in: Multi-regional
    /// Used in: Enchanting Magic (Circles 7-8)
    /// </summary>
    public class RunicPowder : BaseVystiaReagent
    {
        [Constructable]
        public RunicPowder() : this(1) { }

        [Constructable]
        public RunicPowder(int amount)
            : base(amount, 0x0F86, 0, "Multi-regional", "Enchanting Magic (Circles 7-8)")
        {
            Weight = 0.1;
        }

        public RunicPowder(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "runic powder"; } }

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
    #region TitanRune (Enchanting Magic (Circle 8))
    /// <summary>
    /// TitanRune - Rune of titan power
    /// Found in: Multi-regional
    /// Used in: Enchanting Magic (Circle 8)
    /// </summary>
    public class TitanRune : BaseVystiaReagent
    {
        [Constructable]
        public TitanRune() : this(1) { }

        [Constructable]
        public TitanRune(int amount)
            : base(amount, 0x0F7A, 0, "Multi-regional", "Enchanting Magic (Circle 8)")
        {
            Weight = 0.1;
        }

        public TitanRune(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "titan rune"; } }

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
