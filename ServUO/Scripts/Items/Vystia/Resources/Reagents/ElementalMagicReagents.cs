using System;

namespace Server.Items
{
    // ============================================
    // ELEMENTAL MAGIC REAGENTS
    // ============================================
    // Used by Elemental  spells
    // Total: 8 reagents
    // ============================================

    #region AshPetal (Elemental Magic (Circles 1-3))
    /// <summary>
    /// AshPetal - Flower petal from volcanic ash
    /// Found in: Emberlands volcanoes
    /// Used in: Elemental Magic (Circles 1-3)
    /// </summary>
    public class AshPetal : BaseVystiaReagent
    {
        [Constructable]
        public AshPetal() : this(1) { }

        [Constructable]
        public AshPetal(int amount)
            : base(amount, 0x0F86, 0, "Emberlands volcanoes", "Elemental Magic (Circles 1-3)")
        {
            Weight = 0.1;
        }

        public AshPetal(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "ash petal"; } }

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
    #region LavaGlass (Elemental Magic (Circles 2-4))
    /// <summary>
    /// LavaGlass - Glass formed from lava
    /// Found in: Emberlands volcanoes
    /// Used in: Elemental Magic (Circles 2-4)
    /// </summary>
    public class LavaGlass : BaseVystiaReagent
    {
        [Constructable]
        public LavaGlass() : this(1) { }

        [Constructable]
        public LavaGlass(int amount)
            : base(amount, 0x0F8E, 0, "Emberlands volcanoes", "Elemental Magic (Circles 2-4)")
        {
            Weight = 0.1;
        }

        public LavaGlass(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "lava glass"; } }

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
    #region Flameweed (Elemental Magic (Circles 3-5))
    /// <summary>
    /// Flameweed - Weed that grows in flames
    /// Found in: Emberlands volcanoes
    /// Used in: Elemental Magic (Circles 3-5)
    /// </summary>
    public class Flameweed : BaseVystiaReagent
    {
        [Constructable]
        public Flameweed() : this(1) { }

        [Constructable]
        public Flameweed(int amount)
            : base(amount, 0x1A9C, 0, "Emberlands volcanoes", "Elemental Magic (Circles 3-5)")
        {
            Weight = 0.1;
        }

        public Flameweed(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "flameweed"; } }

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
    #region MagmaEssence (Elemental Magic (Circles 4-6))
    /// <summary>
    /// MagmaEssence - Essence of pure magma
    /// Found in: Emberlands volcanoes
    /// Used in: Elemental Magic (Circles 4-6)
    /// </summary>
    public class MagmaEssence : BaseVystiaReagent
    {
        [Constructable]
        public MagmaEssence() : this(1) { }

        [Constructable]
        public MagmaEssence(int amount)
            : base(amount, 0x1C18, 0, "Emberlands volcanoes", "Elemental Magic (Circles 4-6)")
        {
            Weight = 0.1;
        }

        public MagmaEssence(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "magma essence"; } }

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
    #region PhoenixFeather (Elemental Magic (Circles 5-7))
    /// <summary>
    /// PhoenixFeather - Feather from phoenix
    /// Found in: Emberlands volcanoes
    /// Used in: Elemental Magic (Circles 5-7)
    /// </summary>
    public class PhoenixFeather : BaseVystiaReagent
    {
        [Constructable]
        public PhoenixFeather() : this(1) { }

        [Constructable]
        public PhoenixFeather(int amount)
            : base(amount, 0x0F78, 0, "Emberlands volcanoes", "Elemental Magic (Circles 5-7)")
        {
            Weight = 0.1;
        }

        public PhoenixFeather(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "phoenix feather"; } }

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
    #region DragonHeart (Elemental Magic (Circles 6-8))
    /// <summary>
    /// DragonHeart - Heart of fire dragon
    /// Found in: Emberlands volcanoes
    /// Used in: Elemental Magic (Circles 6-8)
    /// </summary>
    public class DragonHeart : BaseVystiaReagent
    {
        [Constructable]
        public DragonHeart() : this(1) { }

        [Constructable]
        public DragonHeart(int amount)
            : base(amount, 0x0F7A, 0, "Emberlands volcanoes", "Elemental Magic (Circles 6-8)")
        {
            Weight = 0.1;
        }

        public DragonHeart(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "dragon heart"; } }

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
    #region PrimordialEmber (Elemental Magic (Circles 7-8))
    /// <summary>
    /// PrimordialEmber - Ember from first flame
    /// Found in: Emberlands volcanoes
    /// Used in: Elemental Magic (Circles 7-8)
    /// </summary>
    public class PrimordialEmber : BaseVystiaReagent
    {
        [Constructable]
        public PrimordialEmber() : this(1) { }

        [Constructable]
        public PrimordialEmber(int amount)
            : base(amount, 0x0F8A, 0, "Emberlands volcanoes", "Elemental Magic (Circles 7-8)")
        {
            Weight = 0.1;
        }

        public PrimordialEmber(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "primordial ember"; } }

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
    #region ElementalCore (Elemental Magic (Circle 8))
    /// <summary>
    /// ElementalCore - Core of elemental power
    /// Found in: Emberlands volcanoes
    /// Used in: Elemental Magic (Circle 8)
    /// </summary>
    public class ElementalCore : BaseVystiaReagent
    {
        [Constructable]
        public ElementalCore() : this(1) { }

        [Constructable]
        public ElementalCore(int amount)
            : base(amount, 0x0F7D, 0, "Emberlands volcanoes", "Elemental Magic (Circle 8)")
        {
            Weight = 0.1;
        }

        public ElementalCore(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "elemental core"; } }

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
