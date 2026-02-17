using System;

namespace Server.Items
{
    // ============================================
    // SHAMANIC MAGIC REAGENTS
    // ============================================
    // Used by Shamanic  spells
    // Total: 8 reagents
    // ============================================

    #region LightningRoot (Shamanic Magic (Circles 1-3))
    /// <summary>
    /// LightningRoot - Root struck by lightning
    /// Found in: Skyreach peaks
    /// Used in: Shamanic Magic (Circles 1-3)
    /// </summary>
    public class LightningRoot : BaseVystiaReagent
    {
        [Constructable]
        public LightningRoot() : this(1) { }

        [Constructable]
        public LightningRoot(int amount)
            : base(amount, 0x0F86, 0, "Skyreach peaks", "Shamanic Magic (Circles 1-3)")
        {
            Weight = 0.1;
        }

        public LightningRoot(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "lightning root"; } }

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
    #region ThunderMoss (Shamanic Magic (Circles 2-4))
    /// <summary>
    /// ThunderMoss - Moss from storm clouds
    /// Found in: Skyreach peaks
    /// Used in: Shamanic Magic (Circles 2-4)
    /// </summary>
    public class ThunderMoss : BaseVystiaReagent
    {
        [Constructable]
        public ThunderMoss() : this(1) { }

        [Constructable]
        public ThunderMoss(int amount)
            : base(amount, 0x0F7B, 0, "Skyreach peaks", "Shamanic Magic (Circles 2-4)")
        {
            Weight = 0.1;
        }

        public ThunderMoss(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "thunder moss"; } }

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
    #region StormCrystal (Shamanic Magic (Circles 3-5))
    /// <summary>
    /// StormCrystal - Crystal from storm
    /// Found in: Skyreach peaks
    /// Used in: Shamanic Magic (Circles 3-5)
    /// </summary>
    public class StormCrystal : BaseVystiaReagent
    {
        [Constructable]
        public StormCrystal() : this(1) { }

        [Constructable]
        public StormCrystal(int amount)
            : base(amount, 0x0F8E, 0, "Skyreach peaks", "Shamanic Magic (Circles 3-5)")
        {
            Weight = 0.1;
        }

        public StormCrystal(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "storm crystal"; } }

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
    #region StormEssence (Shamanic Magic (Circles 4-6))
    /// <summary>
    /// StormEssence - Essence of tempest
    /// Found in: Skyreach peaks
    /// Used in: Shamanic Magic (Circles 4-6)
    /// </summary>
    public class StormEssence : BaseVystiaReagent
    {
        [Constructable]
        public StormEssence() : this(1) { }

        [Constructable]
        public StormEssence(int amount)
            : base(amount, 0x1C18, 0, "Skyreach peaks", "Shamanic Magic (Circles 4-6)")
        {
            Weight = 0.1;
        }

        public StormEssence(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "storm essence"; } }

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
    #region SpiritFeather (Shamanic Magic (Circles 5-7))
    /// <summary>
    /// SpiritFeather - Feather from spirit animal
    /// Found in: Skyreach peaks
    /// Used in: Shamanic Magic (Circles 5-7)
    /// </summary>
    public class SpiritFeather : BaseVystiaReagent
    {
        [Constructable]
        public SpiritFeather() : this(1) { }

        [Constructable]
        public SpiritFeather(int amount)
            : base(amount, 0x0F78, 0, "Skyreach peaks", "Shamanic Magic (Circles 5-7)")
        {
            Weight = 0.1;
        }

        public SpiritFeather(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "spirit feather"; } }

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
    #region PrimalThunder (Shamanic Magic (Circles 6-8))
    /// <summary>
    /// PrimalThunder - Primal thunder essence
    /// Found in: Skyreach peaks
    /// Used in: Shamanic Magic (Circles 6-8)
    /// </summary>
    public class PrimalThunder : BaseVystiaReagent
    {
        [Constructable]
        public PrimalThunder() : this(1) { }

        [Constructable]
        public PrimalThunder(int amount)
            : base(amount, 0x0F8A, 0, "Skyreach peaks", "Shamanic Magic (Circles 6-8)")
        {
            Weight = 0.1;
        }

        public PrimalThunder(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "primal thunder"; } }

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
    #region TotemCarving (Shamanic Magic (Circles 7-8))
    /// <summary>
    /// TotemCarving - Sacred totem wood
    /// Found in: Skyreach peaks
    /// Used in: Shamanic Magic (Circles 7-8)
    /// </summary>
    public class TotemCarving : BaseVystiaReagent
    {
        [Constructable]
        public TotemCarving() : this(1) { }

        [Constructable]
        public TotemCarving(int amount)
            : base(amount, 0x0DE1, 0, "Skyreach peaks", "Shamanic Magic (Circles 7-8)")
        {
            Weight = 0.1;
        }

        public TotemCarving(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "totem carving"; } }

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
    #region WindEssence (Shamanic Magic (Circle 8))
    /// <summary>
    /// WindEssence - Essence of wind itself
    /// Found in: Skyreach peaks
    /// Used in: Shamanic Magic (Circle 8)
    /// </summary>
    public class WindEssence : BaseVystiaReagent
    {
        [Constructable]
        public WindEssence() : this(1) { }

        [Constructable]
        public WindEssence(int amount)
            : base(amount, 0x0F0E, 0, "Skyreach peaks", "Shamanic Magic (Circle 8)")
        {
            Weight = 0.1;
        }

        public WindEssence(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "wind essence"; } }

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
