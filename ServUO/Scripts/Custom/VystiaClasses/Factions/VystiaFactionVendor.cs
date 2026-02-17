/*
 * Vystia Faction Vendor System
 *
 * Base class for faction-affiliated vendors that provide reputation-based discounts.
 * Discounts: Friendly=5%, Honored=8%, Revered=12%, Exalted=15%
 */

using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Items.Vystia;
using Server.Mobiles;
using Server.Network;
using Server.Gumps;

namespace Server.Custom.VystiaClasses.Factions
{
    /// <summary>
    /// Base class for Vystia faction-affiliated vendors.
    /// Provides automatic price discounts based on player reputation with the vendor's faction.
    /// </summary>
    public abstract class VystiaFactionVendor : BaseVendor
    {
        private VystiaFaction m_Faction;

        /// <summary>
        /// The faction this vendor belongs to
        /// </summary>
        [CommandProperty(AccessLevel.GameMaster)]
        public VystiaFaction Faction
        {
            get { return m_Faction; }
            set { m_Faction = value; InvalidateProperties(); }
        }

        // Track the current customer for price calculation
        private Mobile m_CurrentCustomer;

        public VystiaFactionVendor(string title, VystiaFaction faction)
            : base(title)
        {
            m_Faction = faction;
        }

        public VystiaFactionVendor(Serial serial)
            : base(serial)
        {
        }

        #region Faction Discount System

        /// <summary>
        /// Override GetPriceScalar to apply faction reputation discount
        /// </summary>
        public override int GetPriceScalar()
        {
            int baseScalar = base.GetPriceScalar();

            // Apply faction discount if we have a current customer
            if (m_CurrentCustomer is PlayerMobile pm)
            {
                ReputationTier tier = VystiaReputation.GetFactionTier(pm, m_Faction);
                int discount = FactionData.GetVendorDiscount(tier);

                if (discount > 0)
                {
                    // Reduce price by discount percentage
                    return baseScalar - (baseScalar * discount / 100);
                }
            }

            return baseScalar;
        }

        /// <summary>
        /// Override VendorBuy to track the current customer
        /// </summary>
        public override void VendorBuy(Mobile from)
        {
            m_CurrentCustomer = from;
            try
            {
                base.VendorBuy(from);
            }
            finally
            {
                m_CurrentCustomer = null;
            }
        }

        /// <summary>
        /// Override VendorSell to track the current customer
        /// </summary>
        public override void VendorSell(Mobile from)
        {
            m_CurrentCustomer = from;
            try
            {
                base.VendorSell(from);
            }
            finally
            {
                m_CurrentCustomer = null;
            }
        }

        #endregion

        #region Faction Display

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            FactionInfo info = FactionData.GetInfo(m_Faction);
            if (info != null)
            {
                list.Add("<BASEFONT COLOR=#FFD700>{0}</BASEFONT>", info.Name);
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (from is PlayerMobile pm)
            {
                ReputationTier tier = VystiaReputation.GetFactionTier(pm, m_Faction);
                int discount = FactionData.GetVendorDiscount(tier);

                if (discount > 0)
                {
                    PrivateOverheadMessage(MessageType.Regular, 0x3B2, false,
                        string.Format("[{0}% Faction Discount]", discount), from.NetState);
                }
            }
        }

        /// <summary>
        /// Show faction greeting with discount info on speech
        /// </summary>
        public override void OnSpeech(SpeechEventArgs e)
        {
            base.OnSpeech(e);

            if (!e.Handled && e.Mobile is PlayerMobile pm && InRange(pm, 4))
            {
                string speech = e.Speech.ToLower();

                if (speech.Contains("discount") || speech.Contains("price") || speech.Contains("reputation"))
                {
                    ShowDiscountInfo(pm);
                    e.Handled = true;
                }
            }
        }

        private void ShowDiscountInfo(PlayerMobile pm)
        {
            ReputationTier tier = VystiaReputation.GetFactionTier(pm, m_Faction);
            int discount = FactionData.GetVendorDiscount(tier);
            FactionInfo info = FactionData.GetInfo(m_Faction);
            string factionName = info?.Name ?? m_Faction.ToString();

            if (discount > 0)
            {
                SayTo(pm, string.Format("Your {0} standing with {1} grants you a {2}% discount on all purchases.",
                    tier, factionName, discount));
            }
            else if (tier == ReputationTier.Neutral)
            {
                SayTo(pm, string.Format("Improve your standing with {0} to earn discounts. Friendly status grants 5% off.",
                    factionName));
            }
            else if (tier == ReputationTier.Unfriendly || tier == ReputationTier.Hostile)
            {
                SayTo(pm, string.Format("Your reputation with {0} is too low for any discount. Improve your standing first.",
                    factionName));
            }
        }

        #endregion

        #region Serialization

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write((int)m_Faction);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Faction = (VystiaFaction)reader.ReadInt();
        }

        #endregion
    }

    #region Faction-Specific Vendor Classes

    /// <summary>
    /// Frostguard faction vendor (Frosthold region)
    /// </summary>
    public class FrostguardVendor : VystiaFactionVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public FrostguardVendor() : base("the Frostguard merchant", VystiaFaction.Frostguard)
        {
            Name = NameList.RandomName("male");
            Hue = 1150; // Ice blue
            SetSkill(SkillName.ItemID, 80.0, 100.0);
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBFrostguardGoods());
        }

        public FrostguardVendor(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    /// <summary>
    /// Flame Legion faction vendor (Emberlands region)
    /// </summary>
    public class FlameLegionVendor : VystiaFactionVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public FlameLegionVendor() : base("the Flame Legion merchant", VystiaFaction.FlameLegion)
        {
            Name = NameList.RandomName("male");
            Hue = 1358; // Fire red
            SetSkill(SkillName.ItemID, 80.0, 100.0);
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBFlameLegionGoods());
        }

        public FlameLegionVendor(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    /// <summary>
    /// Greenward faction vendor (Verdantpeak region)
    /// </summary>
    public class GreenwardVendor : VystiaFactionVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public GreenwardVendor() : base("the Greenward merchant", VystiaFaction.Greenward)
        {
            Name = NameList.RandomName("female");
            Hue = 2010; // Forest green
            SetSkill(SkillName.ItemID, 80.0, 100.0);
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBGreenwardGoods());
        }

        public GreenwardVendor(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    /// <summary>
    /// Arcane Conclave faction vendor (Crystal Barrens region)
    /// </summary>
    public class ArcaneConclaveVendor : VystiaFactionVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public ArcaneConclaveVendor() : base("the Arcane Conclave merchant", VystiaFaction.ArcaneConclave)
        {
            Name = NameList.RandomName("male");
            Hue = 1154; // Crystal blue
            SetSkill(SkillName.ItemID, 80.0, 100.0);
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBArcaneConclaveGoods());
        }

        public ArcaneConclaveVendor(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    /// <summary>
    /// Technoguild faction vendor (Ironclad region)
    /// </summary>
    public class TechnoguildVendor : VystiaFactionVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public TechnoguildVendor() : base("the Technoguild merchant", VystiaFaction.Technoguild)
        {
            Name = NameList.RandomName("male");
            Hue = 2305; // Metallic
            SetSkill(SkillName.ItemID, 80.0, 100.0);
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBTechnoguildGoods());
        }

        public TechnoguildVendor(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    /// <summary>
    /// Sandwalker faction vendor (Desert region)
    /// </summary>
    public class SandwalkersVendor : VystiaFactionVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public SandwalkersVendor() : base("the Sandwalker merchant", VystiaFaction.Sandwalkers)
        {
            Name = NameList.RandomName("male");
            Hue = 1719; // Sand/tan
            SetSkill(SkillName.ItemID, 80.0, 100.0);
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBSandwalkersGoods());
        }

        public SandwalkersVendor(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    /// <summary>
    /// Voidborn faction vendor (ShadowVoid region)
    /// </summary>
    public class VoidbornVendor : VystiaFactionVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public VoidbornVendor() : base("the Voidborn merchant", VystiaFaction.Voidborn)
        {
            Name = NameList.RandomName("male");
            Hue = 1109; // Dark purple
            SetSkill(SkillName.ItemID, 80.0, 100.0);
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBVoidbornGoods());
        }

        public VoidbornVendor(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Faction Shop Buy Info

    /// <summary>
    /// Frostguard faction goods - Ice weapons, frost resources
    /// </summary>
    public class SBFrostguardGoods : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new List<GenericBuyInfo>();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBFrostguardGoods()
        {
            // Frosthold resources
            m_BuyInfo.Add(new GenericBuyInfo("Frozen Ore", typeof(FrozenOre), 20, 100, 0x19B9, 1152));
            m_BuyInfo.Add(new GenericBuyInfo("Frostforged Ingot", typeof(FrostforgedIngot), 35, 50, 0x1BF2, 1152));
            m_BuyInfo.Add(new GenericBuyInfo("Eternal Ice", typeof(EternalIce), 150, 20, 0x1F1D, 1152));

            // Frosthold potions
            m_BuyInfo.Add(new GenericBuyInfo("Cold Resistance Potion", typeof(ColdResistancePotion), 200, 20, 0xF0E, 1152));
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(FrozenOre), 8);
                Add(typeof(FrostforgedIngot), 15);
            }
        }
    }

    /// <summary>
    /// Flame Legion faction goods - Fire weapons, ember resources
    /// </summary>
    public class SBFlameLegionGoods : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new List<GenericBuyInfo>();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBFlameLegionGoods()
        {
            // Emberlands resources
            m_BuyInfo.Add(new GenericBuyInfo("Molten Ore", typeof(MoltenOre), 25, 100, 0x19B9, 1358));
            m_BuyInfo.Add(new GenericBuyInfo("Emberforged Ingot", typeof(EmberforgedIngot), 40, 50, 0x1BF2, 1358));
            m_BuyInfo.Add(new GenericBuyInfo("Everburning Coal", typeof(EverburningCoal), 150, 20, 0x19AC, 1358));

            // Emberlands potions
            m_BuyInfo.Add(new GenericBuyInfo("Heat Resistance Potion", typeof(HeatResistancePotion), 200, 20, 0xF0E, 1358));
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(MoltenOre), 10);
                Add(typeof(EmberforgedIngot), 18);
            }
        }
    }

    /// <summary>
    /// Greenward faction goods - Nature items, living resources
    /// </summary>
    public class SBGreenwardGoods : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new List<GenericBuyInfo>();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBGreenwardGoods()
        {
            // Verdantpeak resources
            m_BuyInfo.Add(new GenericBuyInfo("Living Ore", typeof(LivingOre), 28, 100, 0x19B9, 2010));
            m_BuyInfo.Add(new GenericBuyInfo("Natureforged Ingot", typeof(NatureforgedIngot), 45, 50, 0x1BF2, 2010));
            m_BuyInfo.Add(new GenericBuyInfo("Treant Heart", typeof(TreantHeart), 300, 10, 0x1CED, 2010));
            m_BuyInfo.Add(new GenericBuyInfo("Living Bark", typeof(LivingBark), 100, 30, 0x1BD7, 2010));

            // Verdantpeak potions
            m_BuyInfo.Add(new GenericBuyInfo("Nature's Blessing Potion", typeof(NaturesBlessingPotion), 250, 20, 0xF0E, 2010));
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(LivingOre), 12);
                Add(typeof(NatureforgedIngot), 20);
            }
        }
    }

    /// <summary>
    /// Arcane Conclave faction goods - Crystal items, magical resources
    /// </summary>
    public class SBArcaneConclaveGoods : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new List<GenericBuyInfo>();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBArcaneConclaveGoods()
        {
            // Crystal Barrens resources
            m_BuyInfo.Add(new GenericBuyInfo("Crystal Ore", typeof(CrystalOre), 30, 100, 0x19B9, 1154));
            m_BuyInfo.Add(new GenericBuyInfo("Crystalline Ingot", typeof(CrystallineIngot), 50, 50, 0x1BF2, 1154));
            m_BuyInfo.Add(new GenericBuyInfo("Prismatic Shard", typeof(PrismaticShard), 200, 15, 0x1F1C, 1154));

            // Crystal potions
            m_BuyInfo.Add(new GenericBuyInfo("Crystal Clarity Potion", typeof(CrystalClarityPotion), 300, 20, 0xF0E, 1154));
            m_BuyInfo.Add(new GenericBuyInfo("Energy Resistance Potion", typeof(EnergyResistancePotion), 200, 20, 0xF0E, 1154));
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(CrystalOre), 13);
                Add(typeof(CrystallineIngot), 22);
            }
        }
    }

    /// <summary>
    /// Technoguild faction goods - Mechanical components, clockwork items
    /// </summary>
    public class SBTechnoguildGoods : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new List<GenericBuyInfo>();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBTechnoguildGoods()
        {
            // Ironclad resources
            m_BuyInfo.Add(new GenericBuyInfo("Steamwork Ore", typeof(SteamworkOre), 30, 100, 0x19B9, 2305));
            m_BuyInfo.Add(new GenericBuyInfo("Clockwork Ingot", typeof(ClockworkIngot), 55, 50, 0x1BF2, 2305));
            m_BuyInfo.Add(new GenericBuyInfo("Clockwork Gear", typeof(ClockworkGear), 15, 100, 0x1053, 0));
            m_BuyInfo.Add(new GenericBuyInfo("Clockwork Spring", typeof(ClockworkSpring), 12, 100, 0x105B, 0));
            m_BuyInfo.Add(new GenericBuyInfo("Steam Core", typeof(SteamCore), 75, 20, 0x1879, 2305));

            // Ironclad potions
            m_BuyInfo.Add(new GenericBuyInfo("Ironclad Fortitude Potion", typeof(IroncladFortitudePotion), 220, 20, 0xF0E, 2401));
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(SteamworkOre), 13);
                Add(typeof(ClockworkIngot), 25);
                Add(typeof(ClockworkGear), 5);
                Add(typeof(ClockworkSpring), 4);
            }
        }
    }

    /// <summary>
    /// Sandwalkers faction goods - Desert items, sand resources
    /// </summary>
    public class SBSandwalkersGoods : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new List<GenericBuyInfo>();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBSandwalkersGoods()
        {
            // Desert resources
            m_BuyInfo.Add(new GenericBuyInfo("Sandstone Ore", typeof(SandstoneOre), 22, 100, 0x19B9, 1719));
            m_BuyInfo.Add(new GenericBuyInfo("Sunforged Ingot", typeof(SunforgedIngot), 38, 50, 0x1BF2, 1719));

            // Desert potions
            m_BuyInfo.Add(new GenericBuyInfo("Desert Swiftness Potion", typeof(DesertSwiftnessPotion), 220, 20, 0xF0E, 2305));
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(SandstoneOre), 9);
                Add(typeof(SunforgedIngot), 17);
            }
        }
    }

    /// <summary>
    /// Voidborn faction goods - Shadow items, void resources
    /// </summary>
    public class SBVoidbornGoods : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new List<GenericBuyInfo>();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBVoidbornGoods()
        {
            // ShadowVoid resources
            m_BuyInfo.Add(new GenericBuyInfo("Obsidian Ore", typeof(ObsidianOre), 35, 100, 0x19B9, 1109));
            m_BuyInfo.Add(new GenericBuyInfo("Voidforged Ingot", typeof(VoidforgedIngot), 60, 50, 0x1BF2, 1109));
            m_BuyInfo.Add(new GenericBuyInfo("Bog Iron Ore", typeof(BogIronOre), 20, 100, 0x19B9, 2073));
            m_BuyInfo.Add(new GenericBuyInfo("Shadowforged Ingot", typeof(ShadowforgedIngot), 35, 50, 0x1BF2, 2073));

            // Shadow potions
            m_BuyInfo.Add(new GenericBuyInfo("Poison Resistance Potion", typeof(PoisonResistancePotion), 200, 20, 0xF0E, 2212));
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(ObsidianOre), 15);
                Add(typeof(VoidforgedIngot), 28);
                Add(typeof(BogIronOre), 8);
                Add(typeof(ShadowforgedIngot), 15);
            }
        }
    }

    #endregion
}
