using System;
using Server;

namespace Server.Items
{
    // ============================================
    // VYSTIA MAGIC REAGENT BAGS
    // ============================================
    // One bag per school containing all 8 reagents
    // Total: 12 bags (one per active magic school; Bardic is legacy)
    // ============================================

    #region IceMagicReagentBag
    /// <summary>
    /// Ice Magic Reagent Bag - Contains all 8 reagents for Ice Magic
    /// </summary>
    public class IceMagicReagentBag : Bag
    {
        [Constructable]
        public IceMagicReagentBag() : this(1)
        {
        }

        [Constructable]
        public IceMagicReagentBag(int amount)
        {
            Name = "Ice Magic Reagent Bag";
            Hue = 0;  // Standard bag (not hued)

            DropItem(new Frostbloom(10));
            DropItem(new GlacierCrystal(10));
            DropItem(new Winterleaf(10));
            DropItem(new PermafrostEssence(10));
            DropItem(new ArcticPearl(10));
            DropItem(new FrozenSoul(10));
            DropItem(new FrostEssence(10));  // Used by 14 spells
            DropItem(new HeartOfWinter(10));
        }

        public IceMagicReagentBag(Serial serial) : base(serial)
        {
        }

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
    #region NatureMagicReagentBag
    /// <summary>
    /// Nature Magic Reagent Bag - Contains all 8 reagents for Nature Magic
    /// </summary>
    public class NatureMagicReagentBag : Bag
    {
        [Constructable]
        public NatureMagicReagentBag() : this(1)
        {
        }

        [Constructable]
        public NatureMagicReagentBag(int amount)
        {
            Name = "Nature Magic Reagent Bag";
            Hue = 0;  // Standard bag (not hued)

            DropItem(new WildMoss(10));
            DropItem(new Moonpetal(10));
            DropItem(new DruidBark(10));
            DropItem(new TreantSap(10));
            DropItem(new ElderwoodSeed(10));
            DropItem(new PrimalVine(10));
            DropItem(new LivingBark(10));
            DropItem(new AncientRoot(10));
        }

        public NatureMagicReagentBag(Serial serial) : base(serial)
        {
        }

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
    #region HexMagicReagentBag
    /// <summary>
    /// Hex Magic Reagent Bag - Contains all 8 reagents for Hex Magic
    /// </summary>
    public class HexMagicReagentBag : Bag
    {
        [Constructable]
        public HexMagicReagentBag() : this(1)
        {
        }

        [Constructable]
        public HexMagicReagentBag(int amount)
        {
            Name = "Hex Magic Reagent Bag";
            Hue = 0;  // Standard bag (not hued)

            DropItem(new BogMoss(10));
            DropItem(new ViperFang(10));
            DropItem(new Witchweed(10));
            DropItem(new ToadsEye(10));
            DropItem(new SwampLotus(10));
            DropItem(new HagsHair(10));
            DropItem(new CursedPearl(10));
            DropItem(new CursedSalt(10));
        }

        public HexMagicReagentBag(Serial serial) : base(serial)
        {
        }

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
    #region ElementalMagicReagentBag
    /// <summary>
    /// Elemental Magic Reagent Bag - Contains all 8 reagents for Elemental Magic
    /// </summary>
    public class ElementalMagicReagentBag : Bag
    {
        [Constructable]
        public ElementalMagicReagentBag() : this(1)
        {
        }

        [Constructable]
        public ElementalMagicReagentBag(int amount)
        {
            Name = "Elemental Magic Reagent Bag";
            Hue = 0;  // Standard bag (not hued)

            DropItem(new AshPetal(10));
            DropItem(new LavaGlass(10));
            DropItem(new Flameweed(10));
            DropItem(new MagmaEssence(10));
            DropItem(new PhoenixFeather(10));
            DropItem(new DragonHeart(10));
            DropItem(new PrimordialEmber(10));
            DropItem(new ElementalCore(10));
        }

        public ElementalMagicReagentBag(Serial serial) : base(serial)
        {
        }

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
    #region DarkMagicReagentBag
    /// <summary>
    /// Dark Magic Reagent Bag - Contains all 8 reagents for Dark Magic
    /// </summary>
    public class DarkMagicReagentBag : Bag
    {
        [Constructable]
        public DarkMagicReagentBag() : this(1)
        {
        }

        [Constructable]
        public DarkMagicReagentBag(int amount)
        {
            Name = "Dark Magic Reagent Bag";
            Hue = 0;  // Standard bag (not hued)

            DropItem(new ShadowMoss(10));
            DropItem(new VoidCrystal(10));
            DropItem(new VoidWeed(10));
            DropItem(new ShadowPetal(10));
            DropItem(new VoidDust(10));
            DropItem(new VoidSilk(10));
            DropItem(new DemonHeart(10));
            DropItem(new ShadowEssence(10));
        }

        public DarkMagicReagentBag(Serial serial) : base(serial)
        {
        }

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
    #region DivinationMagicReagentBag
    /// <summary>
    /// Divination Magic Reagent Bag - Contains all 8 reagents for Divination Magic
    /// </summary>
    public class DivinationMagicReagentBag : Bag
    {
        [Constructable]
        public DivinationMagicReagentBag() : this(1)
        {
        }

        [Constructable]
        public DivinationMagicReagentBag(int amount)
        {
            Name = "Divination Magic Reagent Bag";
            Hue = 0;  // Standard bag (not hued)

            DropItem(new TimeSand(10));
            DropItem(new TimeDust(10));
            DropItem(new DivinationDust(10));
            DropItem(new FateCrystal(10));
            DropItem(new StarlightCrystal(10));
            DropItem(new PropheticLeaf(10));
            DropItem(new SeeingStone(10));
            DropItem(new FateThread(10));
        }

        public DivinationMagicReagentBag(Serial serial) : base(serial)
        {
        }

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
    #region NecromancyReagentBag
    /// <summary>
    /// Necromancy Reagent Bag - Contains all 8 reagents for Necromancy
    /// </summary>
    public class NecromancyReagentBag : Bag
    {
        [Constructable]
        public NecromancyReagentBag() : this(1)
        {
        }

        [Constructable]
        public NecromancyReagentBag(int amount)
        {
            Name = "Necromancy Reagent Bag";
            Hue = 0;  // Standard bag (not hued)

            DropItem(new GraveMoss(10));
            DropItem(new BoneDust(10));
            DropItem(new CorpseAsh(10));
            DropItem(new SoulFragment(10));
            DropItem(new NecroticShroud(10));
            DropItem(new LichDust(10));
            DropItem(new PhylacteryShard(10));
            DropItem(new ReaperEssence(10));
        }

        public NecromancyReagentBag(Serial serial) : base(serial)
        {
        }

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
    #region SummoningMagicReagentBag
    /// <summary>
    /// Summoning Magic Reagent Bag - Contains all 8 reagents for Summoning Magic
    /// </summary>
    public class SummoningMagicReagentBag : Bag
    {
        [Constructable]
        public SummoningMagicReagentBag() : this(1)
        {
        }

        [Constructable]
        public SummoningMagicReagentBag(int amount)
        {
            Name = "Summoning Magic Reagent Bag";
            Hue = 0;  // Standard bag (not hued)

            DropItem(new PlanarDust(10));
            DropItem(new EtherShard(10));
            DropItem(new AetherShard(10));
            DropItem(new SummoningCrystal(10));
            DropItem(new ChaosShard(10));
            DropItem(new BindingRune(10));
            DropItem(new DimensionalKey(10));
            DropItem(new SummoningSalt(10));
        }

        public SummoningMagicReagentBag(Serial serial) : base(serial)
        {
        }

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
    #region ShamanicMagicReagentBag
    /// <summary>
    /// Shamanic Magic Reagent Bag - Contains all 8 reagents for Shamanic Magic
    /// </summary>
    public class ShamanicMagicReagentBag : Bag
    {
        [Constructable]
        public ShamanicMagicReagentBag() : this(1)
        {
        }

        [Constructable]
        public ShamanicMagicReagentBag(int amount)
        {
            Name = "Shamanic Magic Reagent Bag";
            Hue = 0;  // Standard bag (not hued)

            DropItem(new LightningRoot(10));
            DropItem(new ThunderMoss(10));
            DropItem(new StormCrystal(10));
            DropItem(new StormEssence(10));
            DropItem(new SpiritFeather(10));
            DropItem(new PrimalThunder(10));
            DropItem(new TotemCarving(10));
            DropItem(new WindEssence(10));
        }

        public ShamanicMagicReagentBag(Serial serial) : base(serial)
        {
        }

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
    #region SongweavingReagentBag
    /// <summary>
    /// Songweaving Reagent Bag - Contains all 8 reagents for Songweaving
    /// </summary>
    public class SongweavingReagentBag : Bag
    {
        [Constructable]
        public SongweavingReagentBag() : this(1)
        {
        }

        [Constructable]
        public SongweavingReagentBag(int amount)
        {
            Name = "Songweaving Reagent Bag";
            Hue = 0;  // Standard bag (not hued)

            DropItem(new SongPetal(10));
            DropItem(new EchoDust(10));
            DropItem(new VoiceCrystal(10));
            DropItem(new MuseEssence(10));
            DropItem(new HarmonyGem(10));
            DropItem(new EternalNote(10));
            DropItem(new GoldenString(10));
            DropItem(new DragonScale(10));
        }

        public SongweavingReagentBag(Serial serial) : base(serial)
        {
        }

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
    #region BardicMagicReagentBag
    /// <summary>
    /// Bardic Magic Reagent Bag - Contains all 8 reagents for Bardic Magic
    /// </summary>
    public class BardicMagicReagentBag : Bag
    {
        [Constructable]
        public BardicMagicReagentBag() : this(1)
        {
        }

        [Constructable]
        public BardicMagicReagentBag(int amount)
        {
            Name = "Bardic Magic Reagent Bag";
            Hue = 0;  // Standard bag (not hued)

            DropItem(new SongPetal(10));
            DropItem(new EchoDust(10));
            DropItem(new VoiceCrystal(10));
            DropItem(new MuseEssence(10));
            DropItem(new HarmonyGem(10));
            DropItem(new EternalNote(10));
            DropItem(new GoldenString(10));
            DropItem(new DragonScale(10));
        }

        public BardicMagicReagentBag(Serial serial) : base(serial)
        {
        }

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
    #region EnchantingMagicReagentBag
    /// <summary>
    /// Enchanting Magic Reagent Bag - Contains all 8 reagents for Enchanting Magic
    /// </summary>
    public class EnchantingMagicReagentBag : Bag
    {
        [Constructable]
        public EnchantingMagicReagentBag() : this(1)
        {
        }

        [Constructable]
        public EnchantingMagicReagentBag(int amount)
        {
            Name = "Enchanting Magic Reagent Bag";
            Hue = 0;  // Standard bag (not hued)

            DropItem(new ArcaneDust(10));
            DropItem(new EssenceOfMagic(10));
            DropItem(new ManaCrystal(10));
            DropItem(new LeyLineEssence(10));
            DropItem(new LeyLineShard(10));
            DropItem(new RuneFragment(10));
            DropItem(new RunicPowder(10));
            DropItem(new TitanRune(10));
        }

        public EnchantingMagicReagentBag(Serial serial) : base(serial)
        {
        }

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
    #region IllusionMagicReagentBag
    /// <summary>
    /// Illusion Magic Reagent Bag - Contains all 8 reagents for Illusion Magic
    /// </summary>
    public class IllusionMagicReagentBag : Bag
    {
        [Constructable]
        public IllusionMagicReagentBag() : this(1)
        {
        }

        [Constructable]
        public IllusionMagicReagentBag(int amount)
        {
            Name = "Illusion Magic Reagent Bag";
            Hue = 0;  // Standard bag (not hued)

            DropItem(new MirrorDust(10));
            DropItem(new PhantomSilk(10));
            DropItem(new MirageEssence(10));
            DropItem(new DreamCrystal(10));
            DropItem(new RealitySplinter(10));
            DropItem(new VoidMirror(10));
            DropItem(new ChaosPrism(10));
            DropItem(new PhantomPetal(10));
        }

        public IllusionMagicReagentBag(Serial serial) : base(serial)
        {
        }

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
