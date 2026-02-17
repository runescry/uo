using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.ContextMenus;
using Server.Targeting;

namespace Server.Services.Economy
{
    /// <summary>
    /// Vystia Repair Service - Gold Sink for Item Repair
    ///
    /// NPCs can repair items for gold based on:
    /// - Material type (standard vs regional)
    /// - Item quality (regular, exceptional, artifact)
    /// - Current durability loss
    ///
    /// Repair Costs per durability point:
    /// - Standard Iron: 2g
    /// - Regional Tier 1 (FrostforgedIngot, etc.): 35g
    /// - Regional Tier 2 (EternalIce, etc.): 50g
    /// - Legendary/Artifact: 75g
    /// </summary>

    #region Repair Cost Calculator

    public static class VystiaRepairCosts
    {
        // Cost per durability point restored
        public const int StandardCost = 2;
        public const int RegionalTier1Cost = 35;
        public const int RegionalTier2Cost = 50;
        public const int LegendaryCost = 75;
        public const int ArtifactCost = 100;

        // Minimum repair cost (to prevent micro-transactions)
        public const int MinimumRepairCost = 50;

        /// <summary>
        /// Get the repair cost per durability point for an item
        /// </summary>
        public static int GetCostPerDurability(Item item)
        {
            if (item == null)
                return StandardCost;

            // Check for artifacts
            if (item is BaseArmor armor && armor.IsArtifact)
                return ArtifactCost;
            if (item is BaseWeapon weapon && weapon.IsArtifact)
                return ArtifactCost;

            // Check item name for legendary/regional indicators
            string name = item.GetType().Name.ToLower();

            // Legendary items
            if (name.Contains("legendary") || name.Contains("eternal") || name.Contains("prismatic") ||
                name.Contains("phoenix") || name.Contains("cogmaster") || name.Contains("voidcaller"))
                return LegendaryCost;

            // Regional Tier 2 materials
            if (name.Contains("eternal") || name.Contains("prismatic") || name.Contains("treant") ||
                name.Contains("phoenix") || name.Contains("kraken") || name.Contains("abyssal"))
                return RegionalTier2Cost;

            // Regional Tier 1 materials (check for regional resource names)
            if (name.Contains("frost") || name.Contains("ember") || name.Contains("crystal") ||
                name.Contains("steam") || name.Contains("bog") || name.Contains("sand") ||
                name.Contains("shadow") || name.Contains("deepwater") || name.Contains("storm"))
                return RegionalTier1Cost;

            // Check resource type for weapons/armor
            if (item is BaseWeapon wep)
            {
                if (wep.Resource != CraftResource.Iron && wep.Resource != CraftResource.None)
                    return RegionalTier1Cost;
            }
            else if (item is BaseArmor arm)
            {
                if (arm.Resource != CraftResource.Iron && arm.Resource != CraftResource.None)
                    return RegionalTier1Cost;
            }

            return StandardCost;
        }

        /// <summary>
        /// Calculate total repair cost for an item
        /// </summary>
        public static int CalculateRepairCost(Item item)
        {
            if (item == null)
                return 0;

            int durabilityLost = 0;

            if (item is BaseWeapon weapon)
            {
                durabilityLost = weapon.MaxHitPoints - weapon.HitPoints;
            }
            else if (item is BaseArmor armor)
            {
                durabilityLost = armor.MaxHitPoints - armor.HitPoints;
            }
            else if (item is BaseClothing clothing)
            {
                durabilityLost = clothing.MaxHitPoints - clothing.HitPoints;
            }

            if (durabilityLost <= 0)
                return 0;

            int costPerPoint = GetCostPerDurability(item);
            int totalCost = durabilityLost * costPerPoint;

            return Math.Max(totalCost, MinimumRepairCost);
        }

        /// <summary>
        /// Check if an item can be repaired
        /// </summary>
        public static bool CanBeRepaired(Item item)
        {
            if (item == null || item.Deleted)
                return false;

            if (item is BaseWeapon weapon)
                return weapon.HitPoints < weapon.MaxHitPoints;

            if (item is BaseArmor armor)
                return armor.HitPoints < armor.MaxHitPoints;

            if (item is BaseClothing clothing)
                return clothing.HitPoints < clothing.MaxHitPoints;

            return false;
        }

        /// <summary>
        /// Perform the repair on an item
        /// </summary>
        public static bool DoRepair(Item item)
        {
            if (item == null)
                return false;

            if (item is BaseWeapon weapon)
            {
                weapon.HitPoints = weapon.MaxHitPoints;
                return true;
            }

            if (item is BaseArmor armor)
            {
                armor.HitPoints = armor.MaxHitPoints;
                return true;
            }

            if (item is BaseClothing clothing)
            {
                clothing.HitPoints = clothing.MaxHitPoints;
                return true;
            }

            return false;
        }
    }

    #endregion

    #region Repair Context Menu Entry

    public class VystiaRepairContextMenuEntry : ContextMenuEntry
    {
        private Mobile m_Vendor;
        private Mobile m_From;

        public VystiaRepairContextMenuEntry(Mobile vendor, Mobile from) : base(6269) // "Repair" cliloc
        {
            m_Vendor = vendor;
            m_From = from;
        }

        public override void OnClick()
        {
            if (m_From == null || m_Vendor == null || !m_From.InRange(m_Vendor.Location, 4))
            {
                m_From?.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            // Open repair gump
            m_From.CloseGump(typeof(VystiaRepairGump));
            m_From.SendGump(new VystiaRepairGump(m_Vendor, m_From as PlayerMobile));
        }
    }

    #endregion

    #region Repair Gump

    public class VystiaRepairGump : Gump
    {
        private Mobile m_Vendor;
        private PlayerMobile m_Player;
        private List<Item> m_RepairableItems;

        public VystiaRepairGump(Mobile vendor, PlayerMobile player) : base(50, 50)
        {
            m_Vendor = vendor;
            m_Player = player;
            m_RepairableItems = new List<Item>();

            // Find all repairable items in player's backpack
            if (player.Backpack != null)
            {
                foreach (Item item in player.Backpack.FindItemsByType(typeof(Item), true))
                {
                    if (VystiaRepairCosts.CanBeRepaired(item))
                        m_RepairableItems.Add(item);
                }
            }

            // Also check equipped items
            for (int i = 0; i < player.Items.Count; i++)
            {
                Item item = player.Items[i];
                if (item.Layer != Layer.Backpack && item.Layer != Layer.Bank && VystiaRepairCosts.CanBeRepaired(item))
                    m_RepairableItems.Add(item);
            }

            Closable = true;
            Disposable = true;
            Dragable = true;

            BuildGump();
        }

        private void BuildGump()
        {
            AddPage(0);

            int height = Math.Min(400, 140 + m_RepairableItems.Count * 30);

            AddBackground(0, 0, 450, height, 9270);
            AddAlphaRegion(10, 10, 430, height - 20);

            // Title
            AddHtml(15, 15, 420, 25, "<CENTER><BASEFONT COLOR=#FFD700><BIG>Repair Service</BIG></BASEFONT></CENTER>", false, false);

            string vendorName = m_Vendor.Name ?? "the vendor";
            AddHtml(15, 40, 420, 20, String.Format("<CENTER><BASEFONT COLOR=#AAAAAA>{0}</BASEFONT></CENTER>", vendorName), false, false);

            // Player gold
            int playerGold = Banker.GetBalance(m_Player) + GetBackpackGold(m_Player);
            AddHtml(15, 60, 420, 20, String.Format("<CENTER><BASEFONT COLOR=#FFFF00>Your Gold: {0:N0}</BASEFONT></CENTER>", playerGold), false, false);

            if (m_RepairableItems.Count == 0)
            {
                AddHtml(20, 100, 410, 40, "<CENTER><BASEFONT COLOR=#88FF88>All your equipment is in perfect condition!</BASEFONT></CENTER>", false, false);
            }
            else
            {
                // Header
                AddHtml(20, 90, 180, 20, "<BASEFONT COLOR=#FFFFFF>Item</BASEFONT>", false, false);
                AddHtml(200, 90, 80, 20, "<BASEFONT COLOR=#FFFFFF>Durability</BASEFONT>", false, false);
                AddHtml(290, 90, 80, 20, "<BASEFONT COLOR=#FFFFFF>Cost</BASEFONT>", false, false);
                AddHtml(375, 90, 60, 20, "<BASEFONT COLOR=#FFFFFF>Repair</BASEFONT>", false, false);

                AddImageTiled(15, 110, 420, 2, 2624);

                int y = 118;
                int totalCost = 0;

                for (int i = 0; i < m_RepairableItems.Count && i < 8; i++)
                {
                    Item item = m_RepairableItems[i];
                    int cost = VystiaRepairCosts.CalculateRepairCost(item);
                    totalCost += cost;

                    string itemName = GetItemName(item);
                    string durability = GetDurabilityString(item);
                    bool canAfford = playerGold >= cost;

                    AddHtml(20, y, 175, 20, String.Format("<BASEFONT COLOR=#FFFFFF>{0}</BASEFONT>", itemName), false, false);
                    AddHtml(200, y, 85, 20, String.Format("<BASEFONT COLOR=#FF8888>{0}</BASEFONT>", durability), false, false);
                    AddHtml(290, y, 80, 20, String.Format("<BASEFONT COLOR={0}>{1:N0}g</BASEFONT>",
                        canAfford ? "#FFD700" : "#FF4444", cost), false, false);

                    AddButton(385, y, canAfford ? 4005 : 4006, 4007, 100 + i, GumpButtonType.Reply, 0);

                    y += 28;
                }

                // Repair All button
                y += 10;
                AddImageTiled(15, y, 420, 2, 2624);
                y += 10;

                bool canRepairAll = playerGold >= totalCost;
                AddHtml(20, y, 200, 20, String.Format("<BASEFONT COLOR=#FFFFFF>Total Repair Cost:</BASEFONT>"), false, false);
                AddHtml(220, y, 100, 20, String.Format("<BASEFONT COLOR={0}>{1:N0} gold</BASEFONT>",
                    canRepairAll ? "#FFD700" : "#FF4444", totalCost), false, false);

                if (m_RepairableItems.Count > 1)
                {
                    AddButton(340, y, canRepairAll ? 4005 : 4006, 4007, 1, GumpButtonType.Reply, 0);
                    AddHtml(375, y, 60, 20, "<BASEFONT COLOR=#FFFFFF>All</BASEFONT>", false, false);
                }
            }

            // Close button
            AddButton(380, height - 35, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddHtml(340, height - 32, 40, 20, "<BASEFONT COLOR=#FFFFFF>Close</BASEFONT>", false, false);
        }

        private string GetItemName(Item item)
        {
            if (item == null)
                return "Unknown";

            if (!String.IsNullOrEmpty(item.Name))
                return item.Name.Length > 22 ? item.Name.Substring(0, 19) + "..." : item.Name;

            string typeName = item.GetType().Name;
            // Insert spaces before capitals
            var sb = new System.Text.StringBuilder();
            foreach (char c in typeName)
            {
                if (char.IsUpper(c) && sb.Length > 0)
                    sb.Append(' ');
                sb.Append(c);
            }
            string result = sb.ToString();
            return result.Length > 22 ? result.Substring(0, 19) + "..." : result;
        }

        private string GetDurabilityString(Item item)
        {
            if (item is BaseWeapon weapon)
                return String.Format("{0}/{1}", weapon.HitPoints, weapon.MaxHitPoints);
            if (item is BaseArmor armor)
                return String.Format("{0}/{1}", armor.HitPoints, armor.MaxHitPoints);
            if (item is BaseClothing clothing)
                return String.Format("{0}/{1}", clothing.HitPoints, clothing.MaxHitPoints);
            return "???";
        }

        private int GetBackpackGold(Mobile m)
        {
            if (m?.Backpack == null)
                return 0;

            Item[] gold = m.Backpack.FindItemsByType(typeof(Gold));
            int total = 0;
            foreach (Item g in gold)
                total += g.Amount;
            return total;
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 0)
                return;

            int playerGold = Banker.GetBalance(m_Player) + GetBackpackGold(m_Player);

            if (info.ButtonID == 1) // Repair All
            {
                int totalCost = 0;
                foreach (Item item in m_RepairableItems)
                    totalCost += VystiaRepairCosts.CalculateRepairCost(item);

                if (playerGold < totalCost)
                {
                    m_Player.SendMessage(0x22, "You don't have enough gold. You need {0:N0} gold.", totalCost);
                    m_Player.SendGump(new VystiaRepairGump(m_Vendor, m_Player));
                    return;
                }

                if (!DeductGold(m_Player, totalCost))
                {
                    m_Player.SendMessage(0x22, "Failed to deduct gold.");
                    return;
                }

                int repairedCount = 0;
                foreach (Item item in m_RepairableItems)
                {
                    if (VystiaRepairCosts.DoRepair(item))
                        repairedCount++;
                }

                m_Vendor.Say("There you go! {0} items repaired to perfect condition.", repairedCount);
                m_Player.SendMessage(0x35, "You paid {0:N0} gold to repair {1} items.", totalCost, repairedCount);
                m_Player.PlaySound(0x2A);
            }
            else if (info.ButtonID >= 100 && info.ButtonID < 100 + m_RepairableItems.Count)
            {
                int index = info.ButtonID - 100;
                Item item = m_RepairableItems[index];

                if (item == null || item.Deleted || !VystiaRepairCosts.CanBeRepaired(item))
                {
                    m_Player.SendMessage(0x22, "That item no longer needs repair.");
                    m_Player.SendGump(new VystiaRepairGump(m_Vendor, m_Player));
                    return;
                }

                int cost = VystiaRepairCosts.CalculateRepairCost(item);

                if (playerGold < cost)
                {
                    m_Player.SendMessage(0x22, "You don't have enough gold. You need {0:N0} gold.", cost);
                    m_Player.SendGump(new VystiaRepairGump(m_Vendor, m_Player));
                    return;
                }

                if (!DeductGold(m_Player, cost))
                {
                    m_Player.SendMessage(0x22, "Failed to deduct gold.");
                    return;
                }

                VystiaRepairCosts.DoRepair(item);
                string itemName = GetItemName(item);
                m_Vendor.Say("Your {0} is now as good as new!", itemName);
                m_Player.SendMessage(0x35, "You paid {0:N0} gold to repair your {1}.", cost, itemName);
                m_Player.PlaySound(0x2A);

                // Reopen gump
                m_Player.SendGump(new VystiaRepairGump(m_Vendor, m_Player));
            }
        }

        private bool DeductGold(Mobile m, int amount)
        {
            // Try backpack first
            if (m.Backpack != null)
            {
                Item[] gold = m.Backpack.FindItemsByType(typeof(Gold));
                int backpackGold = 0;
                foreach (Item g in gold)
                    backpackGold += g.Amount;

                if (backpackGold >= amount)
                {
                    m.Backpack.ConsumeTotal(typeof(Gold), amount);
                    return true;
                }
                else if (backpackGold > 0)
                {
                    m.Backpack.ConsumeTotal(typeof(Gold), backpackGold);
                    amount -= backpackGold;
                }
            }

            return Banker.Withdraw(m, amount);
        }
    }

    #endregion

    #region Repair Service Mixin

    /// <summary>
    /// Interface for NPCs that can repair items for gold
    /// Add to any vendor that should offer repair services
    /// </summary>
    public interface IVystiaRepairService
    {
        bool OffersRepairService { get; }
    }

    #endregion
}
