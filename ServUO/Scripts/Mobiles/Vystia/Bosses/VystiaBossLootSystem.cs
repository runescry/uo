using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
    /// <summary>
    /// Centralized boss loot system
    /// Handles gold rewards, guaranteed materials, and rare drops for all Vystia bosses
    /// </summary>
    public static class VystiaBossLootSystem
    {
        #region Gold Rewards

        /// <summary>
        /// Get gold reward range for a boss
        /// </summary>
        public static (int min, int max) GetGoldRewardRange(string bossName)
        {
            switch (bossName.ToLower())
            {
                case "frost father":
                    return (3000, 4000);
                case "volcano wyrm":
                    return (3500, 4500);
                case "sphinx of surya":
                    return (3000, 4000);
                case "coven matriarch":
                    return (2800, 3800);
                case "ancient treant":
                    return (3200, 4200);
                case "crystal drake alpha":
                    return (3500, 4500);
                case "forge master":
                    return (4000, 5000);
                case "griffin lord":
                    return (3000, 4000);
                case "ancient kraken":
                    return (3500, 4500);
                case "timeworn lich":
                    return (4500, 6000);
                default:
                    return (3000, 4000);
            }
        }

        /// <summary>
        /// Generate gold reward for a boss
        /// </summary>
        public static int GenerateGoldReward(string bossName)
        {
            var (min, max) = GetGoldRewardRange(bossName);
            return Utility.RandomMinMax(min, max);
        }

        #endregion

        #region Material Rewards

        /// <summary>
        /// Get guaranteed materials for a boss
        /// Returns: (ingot type, ingot count, special material type, special material count)
        /// </summary>
        public static (Type ingotType, int ingotCount, Type specialMaterialType, int specialMaterialCount) GetGuaranteedMaterials(string bossName)
        {
            // This would need to reference actual item types
            // For now, return nulls - will be implemented when material items exist
            return (null, 5, null, 3);
        }

        /// <summary>
        /// Add guaranteed materials to a boss's corpse
        /// </summary>
        public static void AddGuaranteedMaterials(BaseVystiaBoss boss, Container corpse)
        {
            if (corpse == null)
                return;

            var materials = GetGuaranteedMaterials(boss.Name);
            // TODO: Add materials when material item classes exist
            // if (materials.ingotType != null)
            //     corpse.DropItem(Activator.CreateInstance(materials.ingotType, materials.ingotCount) as Item);
        }

        #endregion

        #region Rare Drops

        /// <summary>
        /// Get rare drop item for a boss (15% chance)
        /// </summary>
        public static Type GetRareDrop(string bossName)
        {
            switch (bossName.ToLower())
            {
                case "frost father":
                    return typeof(HeartOfWinter); // TODO: Create this item
                case "volcano wyrm":
                    return typeof(LavaPearl); // TODO: Create this item
                case "sphinx of surya":
                    return typeof(SphinxEye); // TODO: Create this item
                case "coven matriarch":
                    return typeof(HagsHeart); // TODO: Create this item
                case "ancient treant":
                    return typeof(LivingHeartwood); // TODO: Create this item
                case "crystal drake alpha":
                    return typeof(PrismCore); // TODO: Create this item
                case "forge master":
                    return typeof(ForgeHeart); // TODO: Create this item
                case "griffin lord":
                    return typeof(GriffinHeart); // TODO: Create this item
                case "ancient kraken":
                    return typeof(KrakenInk); // TODO: Create this item
                case "timeworn lich":
                    return typeof(PhylacteryFragment); // TODO: Create this item
                default:
                    return null;
            }
        }

        /// <summary>
        /// Check if boss should drop rare item (15% chance)
        /// </summary>
        public static bool ShouldDropRare()
        {
            return Utility.RandomDouble() < 0.15;
        }

        /// <summary>
        /// Add rare drop to boss's corpse
        /// </summary>
        public static void AddRareDrop(BaseVystiaBoss boss, Container corpse)
        {
            if (corpse == null || !ShouldDropRare())
                return;

            Type rareType = GetRareDrop(boss.Name);
            if (rareType != null)
            {
                try
                {
                    Item rareItem = Activator.CreateInstance(rareType) as Item;
                    if (rareItem != null)
                    {
                        corpse.DropItem(rareItem);
                    }
                }
                catch
                {
                    // Item type doesn't exist yet - skip
                }
            }
        }

        #endregion

        #region Generate Loot

        /// <summary>
        /// Generate all loot for a boss and add it to the corpse
        /// </summary>
        public static void GenerateBossLoot(BaseVystiaBoss boss, Container corpse)
        {
            if (corpse == null)
                return;

            // Gold reward
            int gold = GenerateGoldReward(boss.Name);
            corpse.DropItem(new Gold(gold));

            // Guaranteed materials
            AddGuaranteedMaterials(boss, corpse);

            // Rare drop (15% chance)
            AddRareDrop(boss, corpse);
        }

        #endregion
    }

    // Boss loot items - rare drops from Vystia bosses
    public class HeartOfWinter : Item
    {
        [Constructable]
        public HeartOfWinter() : base(0x1F1C) { Name = "Heart of Winter"; Hue = 1152; }
        public HeartOfWinter(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class SphinxEye : Item
    {
        [Constructable]
        public SphinxEye() : base(0x1F1C) { Name = "Sphinx Eye"; Hue = 1153; }
        public SphinxEye(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class HagsHeart : Item
    {
        [Constructable]
        public HagsHeart() : base(0x1F1C) { Name = "Hag's Heart"; Hue = 1372; }
        public HagsHeart(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class LivingHeartwood : Item
    {
        [Constructable]
        public LivingHeartwood() : base(0x1F1C) { Name = "Living Heartwood"; Hue = 2010; }
        public LivingHeartwood(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class PrismCore : Item
    {
        [Constructable]
        public PrismCore() : base(0x1F1C) { Name = "Prism Core"; Hue = 1154; }
        public PrismCore(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class ForgeHeart : Item
    {
        [Constructable]
        public ForgeHeart() : base(0x1F1C) { Name = "Forge Heart"; Hue = 2305; }
        public ForgeHeart(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class GriffinHeart : Item
    {
        [Constructable]
        public GriffinHeart() : base(0x1F1C) { Name = "Griffin Heart"; Hue = 1281; }
        public GriffinHeart(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class KrakenInk : Item
    {
        [Constructable]
        public KrakenInk() : base(0x1F1C) { Name = "Kraken Ink"; Hue = 1266; }
        public KrakenInk(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class PhylacteryFragment : Item
    {
        [Constructable]
        public PhylacteryFragment() : base(0x1F1C) { Name = "Phylactery Fragment"; Hue = 1109; }
        public PhylacteryFragment(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
