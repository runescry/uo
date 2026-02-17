using System;

namespace Server.Items
{
    // ============================================
    // NECROMANCY REAGENTS
    // ============================================
    // Used by Necromancy spells
    // Total: 8 reagents
    // ============================================

    #region GraveMoss (Necromancy (Circles 1-3))
    /// <summary>
    /// GraveMoss - Moss from graves
    /// Found in: ShadowVoid crypts
    /// Used in: Necromancy (Circles 1-3)
    /// </summary>
    public class GraveMoss : BaseVystiaReagent
    {
        [Constructable]
        public GraveMoss() : this(1) { }

        [Constructable]
        public GraveMoss(int amount)
            : base(amount, 0x0F7B, 0, "ShadowVoid crypts", "Necromancy (Circles 1-3)")
        {
            Weight = 0.1;
        }

        public GraveMoss(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "grave moss"; } }

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
    #region BoneDust (Necromancy (Circles 2-4))
    /// <summary>
    /// BoneDust - Ground bone powder
    /// Found in: ShadowVoid crypts
    /// Used in: Necromancy (Circles 2-4)
    /// </summary>
    public class BoneDust : BaseVystiaReagent
    {
        [Constructable]
        public BoneDust() : this(1) { }

        [Constructable]
        public BoneDust(int amount)
            : base(amount, 0x0F8F, 0, "ShadowVoid crypts", "Necromancy (Circles 2-4)")
        {
            Weight = 0.1;
        }

        public BoneDust(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "bone dust"; } }

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
    #region CorpseAsh (Necromancy (Circles 3-5))
    /// <summary>
    /// CorpseAsh - Ash from cremated dead
    /// Found in: ShadowVoid crypts
    /// Used in: Necromancy (Circles 3-5)
    /// </summary>
    public class CorpseAsh : BaseVystiaReagent
    {
        [Constructable]
        public CorpseAsh() : this(1) { }

        [Constructable]
        public CorpseAsh(int amount)
            : base(amount, 0x0DE1, 0, "ShadowVoid crypts", "Necromancy (Circles 3-5)")
        {
            Weight = 0.1;
        }

        public CorpseAsh(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "corpse ash"; } }

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
    #region SoulFragment (Necromancy (Circles 4-6))
    /// <summary>
    /// SoulFragment - Fragment of captured soul
    /// Found in: ShadowVoid crypts
    /// Used in: Necromancy (Circles 4-6)
    /// </summary>
    public class SoulFragment : BaseVystiaReagent
    {
        [Constructable]
        public SoulFragment() : this(1) { }

        [Constructable]
        public SoulFragment(int amount)
            : base(amount, 0x0F0E, 0, "ShadowVoid crypts", "Necromancy (Circles 4-6)")
        {
            Weight = 0.1;
        }

        public SoulFragment(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "soul fragment"; } }

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
    #region NecroticShroud (Necromancy (Circles 5-7))
    /// <summary>
    /// NecroticShroud - Cloth from the dead
    /// Found in: ShadowVoid crypts
    /// Used in: Necromancy (Circles 5-7)
    /// </summary>
    public class NecroticShroud : BaseVystiaReagent
    {
        [Constructable]
        public NecroticShroud() : this(1) { }

        [Constructable]
        public NecroticShroud(int amount)
            : base(amount, 0x1422, 0, "ShadowVoid crypts", "Necromancy (Circles 5-7)")
        {
            Weight = 0.1;
        }

        public NecroticShroud(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "necrotic shroud"; } }

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
    #region LichDust (Necromancy (Circles 6-8))
    /// <summary>
    /// LichDust - Dust from lich phylactery
    /// Found in: ShadowVoid crypts
    /// Used in: Necromancy (Circles 6-8)
    /// </summary>
    public class LichDust : BaseVystiaReagent
    {
        [Constructable]
        public LichDust() : this(1) { }

        [Constructable]
        public LichDust(int amount)
            : base(amount, 0x0F86, 0, "ShadowVoid crypts", "Necromancy (Circles 6-8)")
        {
            Weight = 0.1;
        }

        public LichDust(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "lich dust"; } }

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
    #region PhylacteryShard (Necromancy (Circles 7-8))
    /// <summary>
    /// PhylacteryShard - Shard of phylactery
    /// Found in: ShadowVoid crypts
    /// Used in: Necromancy (Circles 7-8)
    /// </summary>
    public class PhylacteryShard : BaseVystiaReagent
    {
        [Constructable]
        public PhylacteryShard() : this(1) { }

        [Constructable]
        public PhylacteryShard(int amount)
            : base(amount, 0x0F8A, 0, "ShadowVoid crypts", "Necromancy (Circles 7-8)")
        {
            Weight = 0.1;
        }

        public PhylacteryShard(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "phylactery shard"; } }

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
    #region ReaperEssence (Necromancy (Circle 8))
    /// <summary>
    /// ReaperEssence - Essence of death itself
    /// Found in: ShadowVoid crypts
    /// Used in: Necromancy (Circle 8)
    /// </summary>
    public class ReaperEssence : BaseVystiaReagent
    {
        [Constructable]
        public ReaperEssence() : this(1) { }

        [Constructable]
        public ReaperEssence(int amount)
            : base(amount, 0x0F7D, 0, "ShadowVoid crypts", "Necromancy (Circle 8)")
        {
            Weight = 0.1;
        }

        public ReaperEssence(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "reaper essence"; } }

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
