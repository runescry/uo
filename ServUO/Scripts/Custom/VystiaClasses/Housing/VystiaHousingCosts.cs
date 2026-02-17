/*
 * Vystia Housing Costs System
 *
 * Defines purchase prices and integrates with ServUO housing system.
 * Based on design document specifications:
 * - Small (7x7): 50,000g
 * - Medium (11x11): 150,000g
 * - Large (15x15): 400,000g
 * - Keep (18x18): 1,000,000g
 * - Castle (31x31): 3,000,000g
 */

using System;
using System.Collections.Generic;
using Server;
using Server.Multis;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Commands;

namespace Server.Custom.VystiaClasses.Housing
{
    #region Enums

    /// <summary>
    /// House size categories for Vystia
    /// </summary>
    public enum VystiaHouseSize
    {
        Small,      // 7x7 foundations
        Medium,     // 11x11 foundations
        Large,      // 15x15 foundations
        Keep,       // 18x18 keeps
        Castle      // 31x31 castles
    }

    #endregion

    #region House Cost Data

    /// <summary>
    /// Static data class for Vystia housing costs
    /// </summary>
    public static class VystiaHousingCosts
    {
        // Purchase prices by house size
        private static readonly Dictionary<VystiaHouseSize, int> PurchasePrices = new Dictionary<VystiaHouseSize, int>
        {
            { VystiaHouseSize.Small, 50000 },
            { VystiaHouseSize.Medium, 150000 },
            { VystiaHouseSize.Large, 400000 },
            { VystiaHouseSize.Keep, 1000000 },
            { VystiaHouseSize.Castle, 3000000 }
        };

        // Weekly tax rates by house size
        private static readonly Dictionary<VystiaHouseSize, int> WeeklyTaxRates = new Dictionary<VystiaHouseSize, int>
        {
            { VystiaHouseSize.Small, 500 },
            { VystiaHouseSize.Medium, 1500 },
            { VystiaHouseSize.Large, 4000 },
            { VystiaHouseSize.Keep, 10000 },
            { VystiaHouseSize.Castle, 30000 }
        };

        // Size thresholds (plot area in tiles)
        private static readonly int[] SizeThresholds = { 49, 121, 225, 324, 961 }; // 7x7, 11x11, 15x15, 18x18, 31x31

        /// <summary>
        /// Get the purchase price for a house size category
        /// </summary>
        public static int GetPurchasePrice(VystiaHouseSize size)
        {
            return PurchasePrices.TryGetValue(size, out int price) ? price : 50000;
        }

        /// <summary>
        /// Get the weekly tax rate for a house size category
        /// </summary>
        public static int GetWeeklyTax(VystiaHouseSize size)
        {
            return WeeklyTaxRates.TryGetValue(size, out int tax) ? tax : 500;
        }

        /// <summary>
        /// Determine house size category from plot dimensions
        /// </summary>
        public static VystiaHouseSize GetSizeFromDimensions(int width, int height)
        {
            int area = width * height;

            if (area <= 49)
                return VystiaHouseSize.Small;
            else if (area <= 121)
                return VystiaHouseSize.Medium;
            else if (area <= 225)
                return VystiaHouseSize.Large;
            else if (area <= 324)
                return VystiaHouseSize.Keep;
            else
                return VystiaHouseSize.Castle;
        }

        /// <summary>
        /// Determine house size category from a BaseHouse instance
        /// </summary>
        public static VystiaHouseSize GetSizeFromHouse(BaseHouse house)
        {
            if (house == null)
                return VystiaHouseSize.Small;

            // Check house type first for classic houses
            Type houseType = house.GetType();

            // Castles
            if (houseType == typeof(Castle) || houseType.Name.Contains("Castle"))
                return VystiaHouseSize.Castle;

            // Keeps
            if (houseType == typeof(Keep) || houseType.Name.Contains("Keep"))
                return VystiaHouseSize.Keep;

            // Towers (large)
            if (houseType == typeof(Tower) || houseType.Name.Contains("Tower"))
                return VystiaHouseSize.Large;

            // For foundations and other houses, use component bounds
            MultiComponentList mcl = house.Components;
            if (mcl != null)
            {
                return GetSizeFromDimensions(mcl.Width, mcl.Height);
            }

            // Fallback to small
            return VystiaHouseSize.Small;
        }

        /// <summary>
        /// Get the purchase price for a specific house
        /// </summary>
        public static int GetHousePurchasePrice(BaseHouse house)
        {
            VystiaHouseSize size = GetSizeFromHouse(house);
            return GetPurchasePrice(size);
        }

        /// <summary>
        /// Get the weekly tax for a specific house
        /// </summary>
        public static int GetHouseWeeklyTax(BaseHouse house)
        {
            VystiaHouseSize size = GetSizeFromHouse(house);
            return GetWeeklyTax(size);
        }

        /// <summary>
        /// Get a formatted string describing the house size and costs
        /// </summary>
        public static string GetHouseCostInfo(VystiaHouseSize size)
        {
            int price = GetPurchasePrice(size);
            int tax = GetWeeklyTax(size);
            return string.Format("{0} House: {1:N0}g purchase, {2:N0}g/week tax",
                size, price, tax);
        }

        /// <summary>
        /// Check if player can afford a house of given size
        /// </summary>
        public static bool CanAfford(Mobile from, VystiaHouseSize size)
        {
            if (from == null)
                return false;

            int price = GetPurchasePrice(size);
            return Banker.GetBalance(from) >= price;
        }

        /// <summary>
        /// Attempt to charge a player for house purchase
        /// </summary>
        public static bool ChargeForHouse(Mobile from, VystiaHouseSize size)
        {
            if (from == null)
                return false;

            int price = GetPurchasePrice(size);

            if (Banker.Withdraw(from, price, true))
            {
                from.SendMessage("You have been charged {0:N0} gold for your {1} house.", price, size.ToString().ToLower());
                return true;
            }

            from.SendMessage("You cannot afford a {0} house. Required: {1:N0} gold.", size.ToString().ToLower(), price);
            return false;
        }

        /// <summary>
        /// Apply Vystia pricing to a house placement entry cost
        /// </summary>
        public static int GetVystiaCost(int originalCost, int plotWidth, int plotHeight)
        {
            VystiaHouseSize size = GetSizeFromDimensions(plotWidth, plotHeight);
            return GetPurchasePrice(size);
        }
    }

    #endregion

    #region House Price Override

    /// <summary>
    /// Utility class to override house prices at placement time
    /// </summary>
    public static class VystiaHousePriceOverride
    {
        private static bool m_Enabled = true;

        /// <summary>
        /// Enable or disable Vystia house price overrides
        /// </summary>
        public static bool Enabled
        {
            get { return m_Enabled; }
            set { m_Enabled = value; }
        }

        /// <summary>
        /// Apply Vystia pricing to a newly placed house
        /// Call this after house is constructed but before gold is withdrawn
        /// </summary>
        public static int GetOverridePrice(BaseHouse house, int originalPrice)
        {
            if (!m_Enabled || house == null)
                return originalPrice;

            return VystiaHousingCosts.GetHousePurchasePrice(house);
        }

        /// <summary>
        /// Apply Vystia pricing based on house type name
        /// </summary>
        public static int GetOverridePrice(Type houseType, int originalPrice, int plotWidth = 0, int plotHeight = 0)
        {
            if (!m_Enabled)
                return originalPrice;

            // Check type name for castle/keep
            string typeName = houseType?.Name ?? "";

            if (typeName.Contains("Castle"))
                return VystiaHousingCosts.GetPurchasePrice(VystiaHouseSize.Castle);

            if (typeName.Contains("Keep"))
                return VystiaHousingCosts.GetPurchasePrice(VystiaHouseSize.Keep);

            if (typeName.Contains("Tower"))
                return VystiaHousingCosts.GetPurchasePrice(VystiaHouseSize.Large);

            // Use dimensions if available
            if (plotWidth > 0 && plotHeight > 0)
            {
                VystiaHouseSize size = VystiaHousingCosts.GetSizeFromDimensions(plotWidth, plotHeight);
                return VystiaHousingCosts.GetPurchasePrice(size);
            }

            return originalPrice;
        }
    }

    #endregion

    #region GM Commands

    /// <summary>
    /// GM commands for housing cost management
    /// </summary>
    public static class VystiaHousingCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("HouseCosts", AccessLevel.Player, new CommandEventHandler(HouseCosts_OnCommand));
            CommandSystem.Register("HC", AccessLevel.Player, new CommandEventHandler(HouseCosts_OnCommand));
            CommandSystem.Register("SetHousePrice", AccessLevel.GameMaster, new CommandEventHandler(SetHousePrice_OnCommand));
            CommandSystem.Register("SHP", AccessLevel.GameMaster, new CommandEventHandler(SetHousePrice_OnCommand));
            CommandSystem.Register("HouseInfo", AccessLevel.GameMaster, new CommandEventHandler(HouseInfo_OnCommand));
            CommandSystem.Register("HI", AccessLevel.GameMaster, new CommandEventHandler(HouseInfo_OnCommand));
        }

        [Usage("HouseCosts")]
        [Description("Display Vystia housing costs for all house sizes.")]
        private static void HouseCosts_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            from.SendMessage("=== Vystia Housing Costs ===");
            from.SendMessage("");
            from.SendMessage("{0,-10} {1,12} {2,12}", "Size", "Purchase", "Weekly Tax");
            from.SendMessage("{0,-10} {1,12} {2,12}", "----", "--------", "----------");

            foreach (VystiaHouseSize size in Enum.GetValues(typeof(VystiaHouseSize)))
            {
                int price = VystiaHousingCosts.GetPurchasePrice(size);
                int tax = VystiaHousingCosts.GetWeeklyTax(size);
                from.SendMessage("{0,-10} {1,12:N0}g {2,12:N0}g", size, price, tax);
            }

            from.SendMessage("");
            from.SendMessage("Your bank balance: {0:N0} gold", Banker.GetBalance(from));
        }

        [Usage("SetHousePrice <amount>")]
        [Description("Set the price of a targeted house.")]
        private static void SetHousePrice_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Arguments.Length < 1)
            {
                from.SendMessage("Usage: [SetHousePrice <amount>");
                return;
            }

            int price;
            if (!int.TryParse(e.Arguments[0], out price) || price < 0)
            {
                from.SendMessage("Invalid price amount.");
                return;
            }

            from.SendMessage("Target the house to set price to {0:N0} gold.", price);
            from.Target = new SetHousePriceTarget(price);
        }

        [Usage("HouseInfo")]
        [Description("Display information about a targeted house.")]
        private static void HouseInfo_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            from.SendMessage("Target a house to view its information.");
            from.Target = new HouseInfoTarget();
        }

        private class SetHousePriceTarget : Target
        {
            private readonly int m_Price;

            public SetHousePriceTarget(int price) : base(12, false, TargetFlags.None)
            {
                m_Price = price;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                BaseHouse house = null;

                if (targeted is BaseHouse)
                    house = (BaseHouse)targeted;
                else if (targeted is HouseSign)
                    house = ((HouseSign)targeted).Owner;
                else if (targeted is Item)
                {
                    Item item = (Item)targeted;
                    if (item.RootParent is BaseHouse)
                        house = (BaseHouse)item.RootParent;
                }
                else if (targeted is StaticTarget)
                {
                    // Try to find house at location
                    IPooledEnumerable eable = from.Map.GetItemsInRange(((StaticTarget)targeted).Location, 0);
                    foreach (Item item in eable)
                    {
                        if (item is BaseHouse)
                        {
                            house = (BaseHouse)item;
                            break;
                        }
                    }
                    eable.Free();
                }

                if (house == null)
                {
                    // Check if standing in a house
                    house = BaseHouse.FindHouseAt(from);
                }

                if (house == null)
                {
                    from.SendMessage("That is not a house.");
                    return;
                }

                int oldPrice = house.Price;
                house.Price = m_Price;
                from.SendMessage("House price changed from {0:N0} to {1:N0} gold.", oldPrice, m_Price);
            }
        }

        private class HouseInfoTarget : Target
        {
            public HouseInfoTarget() : base(12, false, TargetFlags.None) { }

            protected override void OnTarget(Mobile from, object targeted)
            {
                BaseHouse house = null;

                if (targeted is BaseHouse)
                    house = (BaseHouse)targeted;
                else if (targeted is HouseSign)
                    house = ((HouseSign)targeted).Owner;
                else if (targeted is Item)
                {
                    Item item = (Item)targeted;
                    if (item.RootParent is BaseHouse)
                        house = (BaseHouse)item.RootParent;
                }

                if (house == null)
                    house = BaseHouse.FindHouseAt(from);

                if (house == null)
                {
                    from.SendMessage("That is not a house.");
                    return;
                }

                VystiaHouseSize size = VystiaHousingCosts.GetSizeFromHouse(house);
                int vystiaPrice = VystiaHousingCosts.GetPurchasePrice(size);
                int vystiaTax = VystiaHousingCosts.GetWeeklyTax(size);

                from.SendMessage("=== House Information ===");
                from.SendMessage("Type: {0}", house.GetType().Name);
                from.SendMessage("Owner: {0}", house.Owner?.Name ?? "None");
                from.SendMessage("Current Price: {0:N0} gold", house.Price);
                from.SendMessage("Vystia Size: {0}", size);
                from.SendMessage("Vystia Price: {0:N0} gold", vystiaPrice);
                from.SendMessage("Vystia Weekly Tax: {0:N0} gold", vystiaTax);

                MultiComponentList mcl = house.Components;
                if (mcl != null)
                {
                    from.SendMessage("Dimensions: {0}x{1} ({2} tiles)", mcl.Width, mcl.Height, mcl.Width * mcl.Height);
                }

                from.SendMessage("Storage: {0}/{1}", house.LockDownCount + house.SecureCount, house.MaxLockDowns + house.MaxSecures);
            }
        }
    }

    #endregion
}
