using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Custom.VystiaClasses.Economy
{
    /// <summary>
    /// Vystia Repair Service - Gold sink through NPC repairs
    ///
    /// Repair Costs (per durability point):
    /// - Standard/Iron items: 2g
    /// - Regional Tier 1 materials: 35g
    /// - Regional Tier 2 materials: 50g
    /// - Legendary/Artifact items: 75g
    ///
    /// Players can self-repair for free using crafting tools,
    /// but NPC vendors charge gold for convenient repairs.
    /// </summary>
    public static class VystiaRepairService
    {
        // Cost per durability point by tier
        public const int StandardCostPerDurability = 2;
        public const int RegionalT1CostPerDurability = 35;
        public const int RegionalT2CostPerDurability = 50;
        public const int LegendaryCostPerDurability = 75;

        /// <summary>
        /// Get the repair tier for an item based on its properties
        /// </summary>
        public static RepairTier GetRepairTier(Item item)
        {
            // Check for legendary/artifact status
            if (item is BaseWeapon weapon)
            {
                if (weapon.ArtifactRarity > 0 || weapon.Quality == ItemQuality.Exceptional)
                {
                    if (weapon.ArtifactRarity >= 10)
                        return RepairTier.Legendary;
                    if (weapon.ArtifactRarity >= 5)
                        return RepairTier.RegionalT2;
                }

                // Check for regional materials by resource type
                var resource = weapon.Resource;
                if (IsRegionalT2Resource(resource))
                    return RepairTier.RegionalT2;
                if (IsRegionalT1Resource(resource))
                    return RepairTier.RegionalT1;
            }
            else if (item is BaseArmor armor)
            {
                if (armor.ArtifactRarity > 0 || armor.Quality == ItemQuality.Exceptional)
                {
                    if (armor.ArtifactRarity >= 10)
                        return RepairTier.Legendary;
                    if (armor.ArtifactRarity >= 5)
                        return RepairTier.RegionalT2;
                }

                var resource = armor.Resource;
                if (IsRegionalT2Resource(resource))
                    return RepairTier.RegionalT2;
                if (IsRegionalT1Resource(resource))
                    return RepairTier.RegionalT1;
            }

            return RepairTier.Standard;
        }

        private static bool IsRegionalT1Resource(CraftResource resource)
        {
            // Vystia regional materials - Tier 1
            switch (resource)
            {
                case CraftResource.Agapite:
                case CraftResource.Verite:
                case CraftResource.Valorite:
                case CraftResource.SpinedLeather:
                case CraftResource.HornedLeather:
                case CraftResource.BarbedLeather:
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsRegionalT2Resource(CraftResource resource)
        {
            // Vystia regional materials - Tier 2 (highest tier standard resources)
            switch (resource)
            {
                case CraftResource.Valorite:
                case CraftResource.BarbedLeather:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Calculate repair cost for an item
        /// </summary>
        public static int CalculateRepairCost(Item item, Mobile from = null)
        {
            int damage = 0;
            int maxDurability = 0;

            if (item is BaseWeapon weapon)
            {
                damage = weapon.MaxHitPoints - weapon.HitPoints;
                maxDurability = weapon.MaxHitPoints;
            }
            else if (item is BaseArmor armor)
            {
                damage = armor.MaxHitPoints - armor.HitPoints;
                maxDurability = armor.MaxHitPoints;
            }
            else if (item is BaseClothing clothing)
            {
                damage = clothing.MaxHitPoints - clothing.HitPoints;
                maxDurability = clothing.MaxHitPoints;
            }
            else if (item is BaseJewel jewel)
            {
                damage = jewel.MaxHitPoints - jewel.HitPoints;
                maxDurability = jewel.MaxHitPoints;
            }

            if (damage <= 0)
                return 0;

            RepairTier tier = GetRepairTier(item);
            int costPerPoint = GetCostPerDurability(tier);

            // Minimum cost of 10g for any repair
            int baseCost = Math.Max(10, damage * costPerPoint);

            // Vystia: Fighter + Cogsmith Creed: Repair costs -15%
            if (from is PlayerMobile pm)
            {
                double costReduction = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetClassReligionSynergyRepairCostReduction(pm);
                if (costReduction > 0)
                {
                    baseCost = (int)(baseCost * (1.0 - costReduction)); // costReduction is 0.15, so multiply by 0.85
                }
            }

            return baseCost;
        }

        public static int GetCostPerDurability(RepairTier tier)
        {
            switch (tier)
            {
                case RepairTier.Standard: return StandardCostPerDurability;
                case RepairTier.RegionalT1: return RegionalT1CostPerDurability;
                case RepairTier.RegionalT2: return RegionalT2CostPerDurability;
                case RepairTier.Legendary: return LegendaryCostPerDurability;
                default: return StandardCostPerDurability;
            }
        }

        public static string GetTierName(RepairTier tier)
        {
            switch (tier)
            {
                case RepairTier.Standard: return "Standard";
                case RepairTier.RegionalT1: return "Regional";
                case RepairTier.RegionalT2: return "Superior";
                case RepairTier.Legendary: return "Legendary";
                default: return "Standard";
            }
        }

        /// <summary>
        /// Perform repair and deduct gold
        /// </summary>
        public static bool DoRepair(Mobile from, Item item, Mobile vendor, int cost)
        {
            if (item == null || from == null)
                return false;

            // Verify they can afford it
            int playerGold = Banker.GetBalance(from) + GetBackpackGold(from);
            if (playerGold < cost)
            {
                from.SendMessage(0x22, "You cannot afford the repair cost of {0:N0} gold.", cost);
                return false;
            }

            // Deduct gold
            if (!DeductGold(from, cost))
            {
                from.SendMessage(0x22, "Failed to deduct gold for repair.");
                return false;
            }

            // Perform the repair
            if (item is BaseWeapon weapon)
            {
                weapon.HitPoints = weapon.MaxHitPoints;
            }
            else if (item is BaseArmor armor)
            {
                armor.HitPoints = armor.MaxHitPoints;
            }
            else if (item is BaseClothing clothing)
            {
                clothing.HitPoints = clothing.MaxHitPoints;
            }
            else if (item is BaseJewel jewel)
            {
                jewel.HitPoints = jewel.MaxHitPoints;
            }

            // Effects
            from.PlaySound(0x2A); // Repair sound
            from.SendMessage(0x35, "Your item has been repaired for {0:N0} gold.", cost);

            if (vendor != null)
                vendor.Say("There you go, good as new!");

            return true;
        }

        private static int GetBackpackGold(Mobile m)
        {
            if (m.Backpack == null)
                return 0;

            Item[] gold = m.Backpack.FindItemsByType(typeof(Gold));
            int total = 0;
            foreach (Item g in gold)
                total += g.Amount;
            return total;
        }

        private static bool DeductGold(Mobile m, int amount)
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

            // Remainder from bank
            return Banker.Withdraw(m, amount);
        }
    }

    public enum RepairTier
    {
        Standard,
        RegionalT1,
        RegionalT2,
        Legendary
    }

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
            m_RepairableItems = GetRepairableItems(player);

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            BuildGump();
        }

        private List<Item> GetRepairableItems(PlayerMobile player)
        {
            var items = new List<Item>();

            if (player.Backpack == null)
                return items;

            // Find all damaged equipment in backpack
            foreach (Item item in player.Backpack.Items)
            {
                if (IsDamagedRepairable(item))
                    items.Add(item);
            }

            // Also check equipped items
            foreach (Item item in player.Items)
            {
                if (IsDamagedRepairable(item))
                    items.Add(item);
            }

            return items;
        }

        private bool IsDamagedRepairable(Item item)
        {
            if (item is BaseWeapon weapon)
                return weapon.HitPoints < weapon.MaxHitPoints && weapon.MaxHitPoints > 0;
            if (item is BaseArmor armor)
                return armor.HitPoints < armor.MaxHitPoints && armor.MaxHitPoints > 0;
            if (item is BaseClothing clothing)
                return clothing.HitPoints < clothing.MaxHitPoints && clothing.MaxHitPoints > 0;
            if (item is BaseJewel jewel)
                return jewel.HitPoints < jewel.MaxHitPoints && jewel.MaxHitPoints > 0;
            return false;
        }

        private void BuildGump()
        {
            AddPage(0);

            int width = 500;
            int height = Math.Min(450, 150 + (m_RepairableItems.Count * 28));

            // Background
            AddBackground(0, 0, width, height, 9270);
            AddAlphaRegion(10, 10, width - 20, height - 20);

            // Title
            AddHtml(15, 15, width - 30, 25, "<CENTER><BASEFONT COLOR=#FFD700><BIG>Repair Service</BIG></BASEFONT></CENTER>", false, false);

            string vendorName = m_Vendor != null ? m_Vendor.Name : "the smith";
            AddHtml(15, 40, width - 30, 20, String.Format("<CENTER><BASEFONT COLOR=#AAAAAA>{0}</BASEFONT></CENTER>", vendorName), false, false);

            // Gold display
            int playerGold = Banker.GetBalance(m_Player) + GetBackpackGold(m_Player);
            AddHtml(15, 60, width - 30, 20, String.Format("<CENTER><BASEFONT COLOR=#FFFF00>Your Gold: {0:N0}</BASEFONT></CENTER>", playerGold), false, false);

            if (m_RepairableItems.Count == 0)
            {
                AddHtml(15, 95, width - 30, 40, "<CENTER><BASEFONT COLOR=#AAFFAA>All your equipment is in perfect condition!</BASEFONT></CENTER>", false, false);
            }
            else
            {
                // Header
                AddHtml(20, 90, 200, 20, "<BASEFONT COLOR=#FFFFFF>Item</BASEFONT>", false, false);
                AddHtml(220, 90, 60, 20, "<BASEFONT COLOR=#FFFFFF>Damage</BASEFONT>", false, false);
                AddHtml(280, 90, 60, 20, "<BASEFONT COLOR=#FFFFFF>Tier</BASEFONT>", false, false);
                AddHtml(350, 90, 80, 20, "<BASEFONT COLOR=#FFFFFF>Cost</BASEFONT>", false, false);
                AddHtml(430, 90, 50, 20, "<BASEFONT COLOR=#FFFFFF>Repair</BASEFONT>", false, false);

                AddImageTiled(15, 110, width - 30, 2, 2624);

                int y = 118;
                for (int i = 0; i < m_RepairableItems.Count && i < 10; i++)
                {
                    Item item = m_RepairableItems[i];
                    int damage = GetDamage(item);
                    int cost = VystiaRepairService.CalculateRepairCost(item, m_Player);
                    RepairTier tier = VystiaRepairService.GetRepairTier(item);
                    string tierName = VystiaRepairService.GetTierName(tier);
                    string tierColor = GetTierColor(tier);

                    bool canAfford = playerGold >= cost;

                    AddHtml(20, y, 195, 20, String.Format("<BASEFONT COLOR=#FFFFFF>{0}</BASEFONT>", TruncateName(item.Name ?? item.GetType().Name, 28)), false, false);
                    AddHtml(220, y, 55, 20, String.Format("<BASEFONT COLOR=#FF6666>-{0}</BASEFONT>", damage), false, false);
                    AddHtml(280, y, 65, 20, String.Format("<BASEFONT COLOR={0}>{1}</BASEFONT>", tierColor, tierName), false, false);
                    AddHtml(350, y, 75, 20, String.Format("<BASEFONT COLOR=#FFD700>{0:N0}g</BASEFONT>", cost), false, false);

                    if (canAfford)
                    {
                        AddButton(440, y, 4005, 4007, 100 + i, GumpButtonType.Reply, 0);
                    }
                    else
                    {
                        AddImage(440, y, 4006); // Disabled button
                    }

                    y += 28;
                }

                // Repair All button
                int totalCost = 0;
                foreach (Item item in m_RepairableItems)
                    totalCost += VystiaRepairService.CalculateRepairCost(item, m_Player);

                y += 10;
                AddImageTiled(15, y, width - 30, 2, 2624);
                y += 8;

                bool canAffordAll = playerGold >= totalCost;
                AddHtml(20, y, 300, 20, String.Format("<BASEFONT COLOR=#FFFF00>Repair All ({0} items): {1:N0} gold</BASEFONT>", m_RepairableItems.Count, totalCost), false, false);
                if (canAffordAll && m_RepairableItems.Count > 0)
                {
                    AddButton(350, y, 4005, 4007, 1, GumpButtonType.Reply, 0);
                    AddHtml(385, y, 100, 20, "<BASEFONT COLOR=#AAFFAA>Repair All</BASEFONT>", false, false);
                }
            }

            // Close button
            AddButton(width - 70, height - 35, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddHtml(width - 110, height - 32, 40, 20, "<BASEFONT COLOR=#FFFFFF>Close</BASEFONT>", false, false);
        }

        private string TruncateName(string name, int maxLength)
        {
            if (name.Length <= maxLength)
                return name;
            return name.Substring(0, maxLength - 3) + "...";
        }

        private int GetDamage(Item item)
        {
            if (item is BaseWeapon weapon)
                return weapon.MaxHitPoints - weapon.HitPoints;
            if (item is BaseArmor armor)
                return armor.MaxHitPoints - armor.HitPoints;
            if (item is BaseClothing clothing)
                return clothing.MaxHitPoints - clothing.HitPoints;
            if (item is BaseJewel jewel)
                return jewel.MaxHitPoints - jewel.HitPoints;
            return 0;
        }

        private string GetTierColor(RepairTier tier)
        {
            switch (tier)
            {
                case RepairTier.Standard: return "#AAFFAA";
                case RepairTier.RegionalT1: return "#AAAAFF";
                case RepairTier.RegionalT2: return "#FFAA44";
                case RepairTier.Legendary: return "#FF5555";
                default: return "#FFFFFF";
            }
        }

        private int GetBackpackGold(Mobile m)
        {
            if (m.Backpack == null)
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

            if (info.ButtonID == 1) // Repair All
            {
                int totalRepaired = 0;
                int totalCost = 0;

                foreach (Item item in m_RepairableItems)
                {
                    int cost = VystiaRepairService.CalculateRepairCost(item, m_Player);
                    if (VystiaRepairService.DoRepair(m_Player, item, null, cost))
                    {
                        totalRepaired++;
                        totalCost += cost;
                    }
                    else
                    {
                        break; // Stop if we can't afford more
                    }
                }

                if (totalRepaired > 0)
                {
                    m_Player.SendMessage(0x35, "Repaired {0} items for a total of {1:N0} gold.", totalRepaired, totalCost);
                    if (m_Vendor != null)
                        m_Vendor.Say("All done! Your equipment is ready for battle.");
                }
            }
            else if (info.ButtonID >= 100)
            {
                int index = info.ButtonID - 100;
                if (index >= 0 && index < m_RepairableItems.Count)
                {
                    Item item = m_RepairableItems[index];
                    int cost = VystiaRepairService.CalculateRepairCost(item, m_Player);
                    VystiaRepairService.DoRepair(m_Player, item, m_Vendor, cost);
                }

                // Reopen gump
                m_Player.SendGump(new VystiaRepairGump(m_Vendor, m_Player));
            }
        }
    }

    #endregion

    #region Repair Service Vendor

    /// <summary>
    /// Vystia Blacksmith with repair service
    /// </summary>
    [CorpseNameAttribute("corpse of a blacksmith")]
    public class VystiaBlacksmith : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public VystiaBlacksmith() : base("the blacksmith")
        {
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the Blacksmith";

            SetSkill(SkillName.Blacksmith, 85.0, 100.0);
            SetSkill(SkillName.ArmsLore, 65.0, 88.0);
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBBlacksmith());
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;

            if (!e.Handled && from.InRange(this.Location, 3))
            {
                string speech = e.Speech.ToLower();

                if (speech.Contains("repair") || speech.Contains("fix"))
                {
                    PlayerMobile pm = from as PlayerMobile;
                    if (pm != null)
                    {
                        SayTo(from, "I can repair your equipment for a reasonable fee. Let me see what you have.");
                        pm.SendGump(new VystiaRepairGump(this, pm));
                        e.Handled = true;
                    }
                }
            }

            base.OnSpeech(e);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.Location, 4))
            {
                PlayerMobile pm = from as PlayerMobile;
                if (pm != null)
                {
                    SayTo(from, "Looking to buy something, or do you need repairs? Just say 'repair' if you need equipment fixed.");
                }
            }

            base.OnDoubleClick(from);
        }

        public VystiaBlacksmith(Serial serial) : base(serial) { }

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
}

