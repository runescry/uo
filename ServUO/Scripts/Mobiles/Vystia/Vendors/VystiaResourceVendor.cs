using System;
using System.Collections.Generic;
using Server.Items;
using Server.Items.Vystia;

namespace Server.Mobiles
{
    /// <summary>
    /// Vystia Resource Vendor - Sells all Vystia ores, ingots, and special materials
    /// Temporary vendor until resource gathering systems are implemented
    /// </summary>
    public class VystiaResourceVendor : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public VystiaResourceVendor() : base("the resource merchant")
        {
            SetSkill(SkillName.Mining, 100.0);
            SetSkill(SkillName.Blacksmith, 100.0);
            SetSkill(SkillName.Lumberjacking, 100.0);
            SetSkill(SkillName.Carpentry, 100.0);
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBVystiaResources());
        }

        public VystiaResourceVendor(Serial serial) : base(serial)
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
    /// Sell/Buy info for Vystia Resources (ores, ingots, special materials)
    /// </summary>
    public class SBVystiaResources : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new List<GenericBuyInfo>();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBVystiaResources()
        {
            // ============================================
            // REGIONAL ORES (8 types)
            // ============================================
            m_BuyInfo.Add(new GenericBuyInfo("Frozen Ore", typeof(FrozenOre), 15, 999, 0x19B9, 1152));
            m_BuyInfo.Add(new GenericBuyInfo("Molten Ore", typeof(MoltenOre), 18, 999, 0x19B9, 1358));
            m_BuyInfo.Add(new GenericBuyInfo("Crystal Ore", typeof(CrystalOre), 20, 999, 0x19B9, 1154));
            m_BuyInfo.Add(new GenericBuyInfo("Steamwork Ore", typeof(SteamworkOre), 22, 999, 0x19B9, 2305));
            m_BuyInfo.Add(new GenericBuyInfo("Bog Iron Ore", typeof(BogIronOre), 15, 999, 0x19B9, 2073));
            m_BuyInfo.Add(new GenericBuyInfo("Sandstone Ore", typeof(SandstoneOre), 17, 999, 0x19B9, 1719));
            m_BuyInfo.Add(new GenericBuyInfo("Obsidian Ore", typeof(ObsidianOre), 25, 999, 0x19B9, 1109));
            m_BuyInfo.Add(new GenericBuyInfo("Living Ore", typeof(LivingOre), 20, 999, 0x19B9, 2010));

            // ============================================
            // REGIONAL INGOTS (8 types)
            // ============================================
            m_BuyInfo.Add(new GenericBuyInfo("Frostforged Ingot", typeof(FrostforgedIngot), 25, 999, 0x1BF2, 1152));
            m_BuyInfo.Add(new GenericBuyInfo("Emberforged Ingot", typeof(EmberforgedIngot), 30, 999, 0x1BF2, 1358));
            m_BuyInfo.Add(new GenericBuyInfo("Crystalline Ingot", typeof(CrystallineIngot), 35, 999, 0x1BF2, 1154));
            m_BuyInfo.Add(new GenericBuyInfo("Clockwork Ingot", typeof(ClockworkIngot), 40, 999, 0x1BF2, 2305));
            m_BuyInfo.Add(new GenericBuyInfo("Shadowforged Ingot", typeof(ShadowforgedIngot), 25, 999, 0x1BF2, 2073));
            m_BuyInfo.Add(new GenericBuyInfo("Sunforged Ingot", typeof(SunforgedIngot), 28, 999, 0x1BF2, 1719));
            m_BuyInfo.Add(new GenericBuyInfo("Voidforged Ingot", typeof(VoidforgedIngot), 45, 999, 0x1BF2, 1109));
            m_BuyInfo.Add(new GenericBuyInfo("Natureforged Ingot", typeof(NatureforgedIngot), 35, 999, 0x1BF2, 2010));

            // ============================================
            // MECHANICAL COMPONENTS (Artificer materials)
            // ============================================
            m_BuyInfo.Add(new GenericBuyInfo("Clockwork Gear", typeof(ClockworkGear), 10, 999, 0x1053, 0));
            m_BuyInfo.Add(new GenericBuyInfo("Clockwork Spring", typeof(ClockworkSpring), 8, 999, 0x105B, 0));
            m_BuyInfo.Add(new GenericBuyInfo("Steam Core", typeof(SteamCore), 50, 999, 0x1879, 2305));

            // ============================================
            // SPECIAL RESOURCES
            // ============================================
            m_BuyInfo.Add(new GenericBuyInfo("Eternal Ice", typeof(EternalIce), 100, 999, 0x1F1D, 1152));
            m_BuyInfo.Add(new GenericBuyInfo("Everburning Coal", typeof(EverburningCoal), 100, 999, 0x19AC, 1358));
            m_BuyInfo.Add(new GenericBuyInfo("Prismatic Shard", typeof(PrismaticShard), 150, 999, 0x1F1C, 1154));
            m_BuyInfo.Add(new GenericBuyInfo("Treant Heart", typeof(TreantHeart), 200, 999, 0x1CED, 2010));
            m_BuyInfo.Add(new GenericBuyInfo("Living Bark", typeof(LivingBark), 75, 999, 0x1BD7, 2010));
            m_BuyInfo.Add(new GenericBuyInfo("Swamp Lotus", typeof(SwampLotus), 50, 999, 0x18E9, 2073));
            // REMOVED OLD REAGENT: m_BuyInfo.Add(new GenericBuyInfo("Desert Rose", typeof(DesertRose), 50, 999, 0x18E9, 1719));
            // REMOVED OLD REAGENT: m_BuyInfo.Add(new GenericBuyInfo("Crystal Pollen", typeof(CrystalPollen), 60, 999, 0x26B8, 1154));

            // ============================================
            // REGIONAL POTIONS (8 types)
            // ============================================
            // Resistance Potions
            m_BuyInfo.Add(new GenericBuyInfo("Cold Resistance Potion", typeof(ColdResistancePotion), 150, 20, 0xF0E, 1152));
            m_BuyInfo.Add(new GenericBuyInfo("Heat Resistance Potion", typeof(HeatResistancePotion), 150, 20, 0xF0E, 1358));
            m_BuyInfo.Add(new GenericBuyInfo("Poison Resistance Potion", typeof(PoisonResistancePotion), 150, 20, 0xF0E, 2212));
            m_BuyInfo.Add(new GenericBuyInfo("Energy Resistance Potion", typeof(EnergyResistancePotion), 150, 20, 0xF0E, 1154));
            // Enhancement Potions
            m_BuyInfo.Add(new GenericBuyInfo("Nature's Blessing Potion", typeof(NaturesBlessingPotion), 200, 20, 0xF0E, 2010));
            m_BuyInfo.Add(new GenericBuyInfo("Crystal Clarity Potion", typeof(CrystalClarityPotion), 250, 20, 0xF0E, 1154));
            m_BuyInfo.Add(new GenericBuyInfo("Desert Swiftness Potion", typeof(DesertSwiftnessPotion), 175, 20, 0xF0E, 2305));
            m_BuyInfo.Add(new GenericBuyInfo("Ironclad Fortitude Potion", typeof(IroncladFortitudePotion), 175, 20, 0xF0E, 2401));
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                // Vendors buy resources back at 50% price
            }
        }
    }
}
