using System;

namespace Server.Items
{
    // ============================================
    // BARDIC MAGIC REAGENTS
    // ============================================
    // Used by Bardic  spells
    // Total: 8 reagents
    // ============================================

    #region SongPetal (Bardic Magic (Circles 1-3))
    /// <summary>
    /// SongPetal - Petal that sings
    /// Found in: Multi-regional
    /// Used in: Bardic Magic (Circles 1-3)
    /// </summary>
    public class SongPetal : BaseVystiaReagent
    {
        [Constructable]
        public SongPetal() : this(1) { }

        [Constructable]
        public SongPetal(int amount)
            : base(amount, 0x0F86, 0, "Multi-regional", "Bardic Magic (Circles 1-3)")
        {
            Weight = 0.1;
        }

        public SongPetal(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "song petal"; } }

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
    #region EchoDust (Bardic Magic (Circles 2-4))
    /// <summary>
    /// EchoDust - Dust of echoes
    /// Found in: Multi-regional
    /// Used in: Bardic Magic (Circles 2-4)
    /// </summary>
    public class EchoDust : BaseVystiaReagent
    {
        [Constructable]
        public EchoDust() : this(1) { }

        [Constructable]
        public EchoDust(int amount)
            : base(amount, 0x0F8F, 0, "Multi-regional", "Bardic Magic (Circles 2-4)")
        {
            Weight = 0.1;
        }

        public EchoDust(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "echo dust"; } }

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
    #region VoiceCrystal (Bardic Magic (Circles 3-5))
    /// <summary>
    /// VoiceCrystal - Crystal of voice
    /// Found in: Multi-regional
    /// Used in: Bardic Magic (Circles 3-5)
    /// </summary>
    public class VoiceCrystal : BaseVystiaReagent
    {
        [Constructable]
        public VoiceCrystal() : this(1) { }

        [Constructable]
        public VoiceCrystal(int amount)
            : base(amount, 0x0F8E, 0, "Multi-regional", "Bardic Magic (Circles 3-5)")
        {
            Weight = 0.1;
        }

        public VoiceCrystal(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "voice crystal"; } }

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
    #region MuseEssence (Bardic Magic (Circles 4-6))
    /// <summary>
    /// MuseEssence - Essence of muse
    /// Found in: Multi-regional
    /// Used in: Bardic Magic (Circles 4-6)
    /// </summary>
    public class MuseEssence : BaseVystiaReagent
    {
        [Constructable]
        public MuseEssence() : this(1) { }

        [Constructable]
        public MuseEssence(int amount)
            : base(amount, 0x1C18, 0, "Multi-regional", "Bardic Magic (Circles 4-6)")
        {
            Weight = 0.1;
        }

        public MuseEssence(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "muse essence"; } }

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
    #region HarmonyGem (Bardic Magic (Circles 5-7))
    /// <summary>
    /// HarmonyGem - Gem of perfect harmony
    /// Found in: Multi-regional
    /// Used in: Bardic Magic (Circles 5-7)
    /// </summary>
    public class HarmonyGem : BaseVystiaReagent
    {
        [Constructable]
        public HarmonyGem() : this(1) { }

        [Constructable]
        public HarmonyGem(int amount)
            : base(amount, 0x0F7A, 0, "Multi-regional", "Bardic Magic (Circles 5-7)")
        {
            Weight = 0.1;
        }

        public HarmonyGem(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "harmony gem"; } }

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
    #region EternalNote (Bardic Magic (Circles 6-8))
    /// <summary>
    /// EternalNote - Note that never ends
    /// Found in: Multi-regional
    /// Used in: Bardic Magic (Circles 6-8)
    /// </summary>
    public class EternalNote : BaseVystiaReagent
    {
        [Constructable]
        public EternalNote() : this(1) { }

        [Constructable]
        public EternalNote(int amount)
            : base(amount, 0x0F8D, 0, "Multi-regional", "Bardic Magic (Circles 6-8)")
        {
            Weight = 0.1;
        }

        public EternalNote(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "eternal note"; } }

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
    #region GoldenString (Bardic Magic (Circles 7-8))
    /// <summary>
    /// GoldenString - String from golden lyre
    /// Found in: Multi-regional
    /// Used in: Bardic Magic (Circles 7-8)
    /// </summary>
    public class GoldenString : BaseVystiaReagent
    {
        [Constructable]
        public GoldenString() : this(1) { }

        [Constructable]
        public GoldenString(int amount)
            : base(amount, 0x1422, 0, "Multi-regional", "Bardic Magic (Circles 7-8)")
        {
            Weight = 0.1;
        }

        public GoldenString(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "golden string"; } }

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
    #region DragonScale (Bardic Magic (Circle 8))
    /// <summary>
    /// DragonScale - Scale from ancient dragon
    /// Found in: Multi-regional
    /// Used in: Bardic Magic (Circle 8)
    /// </summary>
    public class DragonScale : BaseVystiaReagent
    {
        [Constructable]
        public DragonScale() : this(1) { }

        [Constructable]
        public DragonScale(int amount)
            : base(amount, 0x0F78, 0, "Multi-regional", "Bardic Magic (Circle 8)")
        {
            Weight = 0.1;
        }

        public DragonScale(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "dragon scale"; } }

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
