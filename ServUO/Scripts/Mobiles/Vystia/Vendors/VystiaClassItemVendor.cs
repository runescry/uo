using System;
using System.Collections.Generic;
using Server.Items;
using Server.Items.VystiaClassItems;

namespace Server.Mobiles
{
    /// <summary>
    /// Vystia Class Item Vendor - Sells class focus items, resource potions, and combat potions
    /// </summary>
    public class VystiaClassItemVendor : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public VystiaClassItemVendor() : base("the class equipment dealer")
        {
            Name = NameList.RandomName("male") + " the Class Equipment Dealer";
            Hue = 0;
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBVystiaClassItems());
        }

        public VystiaClassItemVendor(Serial serial) : base(serial)
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
    /// Sell/Buy info for Vystia Class Items
    /// </summary>
    public class SBVystiaClassItems : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new List<GenericBuyInfo>();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBVystiaClassItems()
        {
            // ============================================
            // CLASS FOCUS ITEMS (26 items - one per class)
            // Price based on rarity/power: 500-2000 gold
            // ============================================

            // Martial Classes
            m_BuyInfo.Add(new GenericBuyInfo("Barbarian Focus - Fury Idol", typeof(FuryIdol), 800, 20, 0x1F14, 1157));
            m_BuyInfo.Add(new GenericBuyInfo("Fighter Focus - War Banner", typeof(WarBanner), 750, 20, 0x1F14, 2305));
            m_BuyInfo.Add(new GenericBuyInfo("Knight Focus - Combat Manual", typeof(CombatManual), 900, 20, 0x1F14, 2305));
            m_BuyInfo.Add(new GenericBuyInfo("Monk Focus - Chi Beads", typeof(ChiBeads), 800, 20, 0x1F14, 1161));
            m_BuyInfo.Add(new GenericBuyInfo("Paladin Focus - Virtuous Relic", typeof(VirtuousRelic), 1000, 20, 0x1F14, 1153));
            m_BuyInfo.Add(new GenericBuyInfo("Ranger Focus - Hunter's Mark Totem", typeof(HuntersMarkTotem), 850, 20, 0x1F14, 2010));
            m_BuyInfo.Add(new GenericBuyInfo("Rogue Focus - Shadow Veil", typeof(ShadowVeil), 900, 20, 0x1F14, 1109));
            m_BuyInfo.Add(new GenericBuyInfo("Templar Focus - Zealous Icon", typeof(ZealousIcon), 950, 20, 0x1F14, 1153));
            m_BuyInfo.Add(new GenericBuyInfo("Bounty Hunter Focus - Tracking Stone", typeof(TrackingStone), 850, 20, 0x1F14, 1719));
            m_BuyInfo.Add(new GenericBuyInfo("Beastmaster Focus - Beast Bond", typeof(BeastBond), 900, 20, 0x1F14, 1150));

            // Magic Classes
            m_BuyInfo.Add(new GenericBuyInfo("Ice Mage Focus - Frost Crystal", typeof(FrostCrystal), 1000, 20, 0x1F19, 1152));
            m_BuyInfo.Add(new GenericBuyInfo("Sorcerer Focus - Elemental Orb", typeof(ElementalOrb), 1200, 20, 0x1F19, 1358));
            m_BuyInfo.Add(new GenericBuyInfo("Warlock Focus - Soul Gem", typeof(SoulGem), 1500, 20, 0x1F19, 1109));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancer Focus - Death's Hourglass", typeof(DeathsHourglass), 1500, 20, 0x1F19, 1175));
            m_BuyInfo.Add(new GenericBuyInfo("Druid Focus - Primal Totem", typeof(PrimalTotem), 1000, 20, 0x1F14, 2010));
            m_BuyInfo.Add(new GenericBuyInfo("Wizard Focus - Arcane Conduit", typeof(ArcaneConduit), 1100, 20, 0x1F19, 1154));
            m_BuyInfo.Add(new GenericBuyInfo("Oracle Focus - Seer's Crystal", typeof(SeersCrystal), 1200, 20, 0x1F19, 1154));
            m_BuyInfo.Add(new GenericBuyInfo("Witch Focus - Hex Doll", typeof(HexDoll), 1100, 20, 0x1F14, 2073));
            m_BuyInfo.Add(new GenericBuyInfo("Illusionist Focus - Mirror Shard", typeof(MirrorShard), 1000, 20, 0x1F19, 1719));
            m_BuyInfo.Add(new GenericBuyInfo("Summoner Focus - Summoner's Sigil", typeof(SummonersSigil), 1300, 20, 0x1F14, 1365));
            m_BuyInfo.Add(new GenericBuyInfo("Shaman Focus - Spirit Feather", typeof(Server.Items.VystiaClassItems.SpiritFeather), 1000, 20, 0x1F14, 2073));

            // Hybrid/Support Classes
            m_BuyInfo.Add(new GenericBuyInfo("Cleric Focus - Sacred Censer", typeof(SacredCenser), 1000, 20, 0x1F14, 1153));
            m_BuyInfo.Add(new GenericBuyInfo("Bard Focus - Dragon Lute", typeof(DragonLute), 1200, 20, 0x0EB3, 1161));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanter Focus - Rune Stone", typeof(RuneStone), 1100, 20, 0x1F14, 1154));
            m_BuyInfo.Add(new GenericBuyInfo("Alchemist Focus - Philosopher's Stone", typeof(PhilosophersStone), 2000, 20, 0x1F19, 1161));
            m_BuyInfo.Add(new GenericBuyInfo("Artificer Focus - Steam Core", typeof(Server.Items.VystiaClassItems.SteamCore), 1500, 20, 0x1053, 2305));

            // ============================================
            // RESOURCE POTIONS (restore secondary resources)
            // Price based on resource value: 50-200 gold
            // ============================================
            m_BuyInfo.Add(new GenericBuyInfo("Soul Essence Vial (Warlock)", typeof(SoulEssenceVial), 150, 50, 0x0F0E, 1109));
            m_BuyInfo.Add(new GenericBuyInfo("Fury Tonic (Barbarian)", typeof(FuryTonic), 75, 50, 0x0F0E, 1157));
            m_BuyInfo.Add(new GenericBuyInfo("Chi Tea (Monk)", typeof(ChiTea), 100, 50, 0x0F0E, 1161));
            m_BuyInfo.Add(new GenericBuyInfo("Focus Elixir (Ranger)", typeof(FocusElixir), 75, 50, 0x0F0E, 2010));
            m_BuyInfo.Add(new GenericBuyInfo("Fortitude Draught (Knight)", typeof(FortitudeDraught), 100, 50, 0x0F0E, 2305));
            m_BuyInfo.Add(new GenericBuyInfo("Faith Incense (Cleric)", typeof(FaithIncense), 80, 50, 0x0F0E, 1153));
            m_BuyInfo.Add(new GenericBuyInfo("Crescendo Crystal (Bard)", typeof(CrescendoCrystal), 120, 50, 0x1F19, 1161));
            m_BuyInfo.Add(new GenericBuyInfo("Life Force Vial (Necromancer)", typeof(LifeForceVial), 100, 50, 0x0F0E, 1175));
            m_BuyInfo.Add(new GenericBuyInfo("Zeal Elixir (Templar)", typeof(ZealElixir), 100, 50, 0x0F0E, 1153));
            m_BuyInfo.Add(new GenericBuyInfo("Steam Canister (Artificer)", typeof(SteamCanister), 90, 50, 0x1F19, 2305));
            m_BuyInfo.Add(new GenericBuyInfo("Pursuit Tracker (Bounty Hunter)", typeof(PursuitTracker), 110, 50, 0x1F14, 1719));

            // ============================================
            // COMBAT POTIONS (temporary buffs)
            // Price based on power: 100-300 gold
            // ============================================
            m_BuyInfo.Add(new GenericBuyInfo("Burst Potion (+100% damage)", typeof(BurstPotion), 200, 50, 0x0F0E, 1157));
            m_BuyInfo.Add(new GenericBuyInfo("Haste Potion (+50% speed)", typeof(HastePotion), 250, 50, 0x0F0E, 1281));
            m_BuyInfo.Add(new GenericBuyInfo("Resist Potion (+30 resists)", typeof(ResistPotion), 150, 50, 0x0F0E, 1153));
            m_BuyInfo.Add(new GenericBuyInfo("Cleanse Potion (remove debuffs)", typeof(CleansePotion), 175, 50, 0x0F0E, 1150));
            m_BuyInfo.Add(new GenericBuyInfo("Second Wind Potion (50% HP/Mana/Stam)", typeof(SecondWindPotion), 300, 50, 0x0F0E, 1161));
            m_BuyInfo.Add(new GenericBuyInfo("Invisibility Potion (10s)", typeof(VystiaInvisibilityPotion), 200, 50, 0x0F0E, 1150));
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                // Vendors buy items back at reduced prices
            }
        }
    }
}
