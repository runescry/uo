using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    /// <summary>
    /// Vystia Reagent Vendor - Sells all 82 Vystia magic reagents
    /// Auto-generated from actual implemented reagent classes
    /// </summary>
    public class VystiaReagentVendor : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public VystiaReagentVendor() : base("the reagent merchant")
        {
            SetSkill(SkillName.Alchemy, 90.0, 100.0);
            SetSkill(SkillName.Magery, 90.0, 100.0);
            SetSkill(SkillName.Inscribe, 80.0, 100.0);
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBVystiaReagents());
        }

        public VystiaReagentVendor(Serial serial) : base(serial)
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

    /// <summary>
    /// Sell/Buy info for all 82 Vystia Reagents
    /// </summary>
    public class SBVystiaReagents : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new List<GenericBuyInfo>();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBVystiaReagents()
        {

            // ============================================
            // ICE MAGIC (7 reagents)
            // ============================================
            m_BuyInfo.Add(new GenericBuyInfo("Frostbloom", typeof(Frostbloom), 5, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Glacier Crystal", typeof(GlacierCrystal), 5, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Winterleaf", typeof(Winterleaf), 12, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Permafrost Essence", typeof(PermafrostEssence), 14, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Arctic Pearl", typeof(ArcticPearl), 35, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Frozen Soul", typeof(FrozenSoul), 40, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Heart Of Winter", typeof(HeartOfWinter), 60, 999, 0x1F2D, 0x481));

            // ============================================
            // NATURE MAGIC (6 reagents)
            // ============================================
            m_BuyInfo.Add(new GenericBuyInfo("Wild Moss", typeof(WildMoss), 5, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Moonpetal", typeof(Moonpetal), 5, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Druid Bark", typeof(DruidBark), 12, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Treant Sap", typeof(TreantSap), 14, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Elderwood Seed", typeof(ElderwoodSeed), 35, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Primal Vine", typeof(PrimalVine), 40, 999, 0x1F2D, 0x7D6));

            // ============================================
            // HEX MAGIC (6 reagents)
            // ============================================
            m_BuyInfo.Add(new GenericBuyInfo("Bog Moss", typeof(BogMoss), 5, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Viper Fang", typeof(ViperFang), 5, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Witchweed", typeof(Witchweed), 12, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Toads Eye", typeof(ToadsEye), 14, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hags Hair", typeof(HagsHair), 35, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Cursed Pearl", typeof(CursedPearl), 40, 999, 0x1F2D, 0x81D));

            // ============================================
            // ELEMENTAL MAGIC (6 reagents)
            // ============================================
            m_BuyInfo.Add(new GenericBuyInfo("Ash Petal", typeof(AshPetal), 5, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Lava Glass", typeof(LavaGlass), 5, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Flameweed", typeof(Flameweed), 12, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Magma Essence", typeof(MagmaEssence), 14, 999, 0x1F2D, 0x54E));
            // REMOVED OLD REAGENT: m_BuyInfo.Add(new GenericBuyInfo("Ember Feather", typeof(EmberFeather), 35, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Primordial Ember", typeof(PrimordialEmber), 40, 999, 0x1F2D, 0x54E));

            // ============================================
            // DARK MAGIC (8 reagents)
            // ============================================
            m_BuyInfo.Add(new GenericBuyInfo("Shadow Moss", typeof(ShadowMoss), 5, 999, 0x1F2D, 0x455));
            // REMOVED OLD REAGENT: m_BuyInfo.Add(new GenericBuyInfo("Demon Scale", typeof(DemonScale), 5, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Void Weed", typeof(VoidWeed), 12, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Chaos Shard", typeof(ChaosShard), 14, 999, 0x1F2D, 0x455));
            // REMOVED OLD REAGENT: m_BuyInfo.Add(new GenericBuyInfo("Dark Void Dust", typeof(DarkVoidDust), 35, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Void Silk", typeof(VoidSilk), 40, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Demon Heart", typeof(DemonHeart), 60, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Void Crystal", typeof(VoidCrystal), 80, 999, 0x1F2D, 0x455));

            // ============================================
            // DIVINATION MAGIC (6 reagents)
            // ============================================
            m_BuyInfo.Add(new GenericBuyInfo("Divination Dust", typeof(DivinationDust), 5, 999, 0x1F2D, 0x482));
            // REMOVED OLD REAGENT: m_BuyInfo.Add(new GenericBuyInfo("Rainbow Crystal", typeof(RainbowCrystal), 5, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Starlight Crystal", typeof(StarlightCrystal), 12, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Ley Line Shard", typeof(LeyLineShard), 14, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Time Sand", typeof(TimeSand), 35, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Fate Crystal", typeof(FateCrystal), 40, 999, 0x1F2D, 0x482));

            // ============================================
            // NECROMANCY (6 reagents)
            // ============================================
            m_BuyInfo.Add(new GenericBuyInfo("Grave Moss", typeof(GraveMoss), 5, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Bone Dust", typeof(BoneDust), 5, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necrotic Shroud", typeof(NecroticShroud), 12, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Soul Fragment", typeof(SoulFragment), 14, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Corpse Ash", typeof(CorpseAsh), 35, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Lich Dust", typeof(LichDust), 40, 999, 0x1F2D, 0x455));

            // ============================================
            // SUMMONING MAGIC (5 reagents)
            // ============================================
            // REMOVED OLD REAGENT: m_BuyInfo.Add(new GenericBuyInfo("Kelp Strand", typeof(KelpStrand), 5, 999, 0x1F2D, 0x555));
            // REMOVED OLD REAGENT: m_BuyInfo.Add(new GenericBuyInfo("Coral Fragment", typeof(CoralFragment), 5, 999, 0x1F2D, 0x555));
            // REMOVED OLD REAGENT: m_BuyInfo.Add(new GenericBuyInfo("Sea Glass", typeof(SeaGlass), 12, 999, 0x1F2D, 0x555));
            // REMOVED OLD REAGENT: m_BuyInfo.Add(new GenericBuyInfo("Leviathan Tooth", typeof(LeviathanTooth), 14, 999, 0x1F2D, 0x555));
            // REMOVED OLD REAGENT: m_BuyInfo.Add(new GenericBuyInfo("Abyssal Ink", typeof(AbyssalInk), 35, 999, 0x1F2D, 0x555));

            // ============================================
            // SHAMANIC MAGIC (8 reagents)
            // ============================================
            m_BuyInfo.Add(new GenericBuyInfo("Thunder Moss", typeof(ThunderMoss), 5, 999, 0x1F2D, 0x501));
            // REMOVED OLD REAGENT: m_BuyInfo.Add(new GenericBuyInfo("Wind Crystal", typeof(WindCrystal), 5, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Spirit Feather", typeof(SpiritFeather), 12, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Lightning Root", typeof(LightningRoot), 14, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Totem Carving", typeof(TotemCarving), 35, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Storm Essence", typeof(StormEssence), 40, 999, 0x1F2D, 0x501));
            // REMOVED OLD REAGENT: m_BuyInfo.Add(new GenericBuyInfo("Windstone Ore", typeof(WindstoneOre), 60, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Primal Thunder", typeof(PrimalThunder), 80, 999, 0x1F2D, 0x501));

            // ============================================
            // BARDIC MAGIC (8 reagents)
            // ============================================
            m_BuyInfo.Add(new GenericBuyInfo("Song Petal", typeof(SongPetal), 5, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Echo Dust", typeof(EchoDust), 5, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Voice Crystal", typeof(VoiceCrystal), 12, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Golden String", typeof(GoldenString), 14, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Harmony Gem", typeof(HarmonyGem), 35, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Muse Essence", typeof(MuseEssence), 40, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Dragon Scale", typeof(DragonScale), 60, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Eternal Note", typeof(EternalNote), 80, 999, 0x1F2D, 0x8A5));

            // ============================================
            // ENCHANTING MAGIC (8 reagents)
            // ============================================
            m_BuyInfo.Add(new GenericBuyInfo("Arcane Dust", typeof(ArcaneDust), 5, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Rune Fragment", typeof(RuneFragment), 5, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Mana Crystal", typeof(ManaCrystal), 12, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Runic Powder", typeof(RunicPowder), 14, 999, 0x1F2D, 0x8FD));
            // REMOVED OLD REAGENT: m_BuyInfo.Add(new GenericBuyInfo("Enchanters Ink", typeof(EnchantersInk), 35, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Aether Shard", typeof(AetherShard), 40, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Titan Rune", typeof(TitanRune), 60, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Essence Of Magic", typeof(EssenceOfMagic), 80, 999, 0x1F2D, 0x8FD));

            // ============================================
            // ILLUSION MAGIC (8 reagents)
            // ============================================
            m_BuyInfo.Add(new GenericBuyInfo("Shadow Petal", typeof(ShadowPetal), 5, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Mirror Dust", typeof(MirrorDust), 5, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Phantom Silk", typeof(PhantomSilk), 12, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Mirage Essence", typeof(MirageEssence), 14, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Dream Crystal", typeof(DreamCrystal), 35, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Reality Splinter", typeof(RealitySplinter), 40, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Void Mirror", typeof(VoidMirror), 60, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Chaos Prism", typeof(ChaosPrism), 80, 999, 0x1F2D, 0x47E));
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                // Vendors buy reagents back at 50% price
            }
        }
    }
}
