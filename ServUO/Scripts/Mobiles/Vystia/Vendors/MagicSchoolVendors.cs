using System;
using System.Collections.Generic;
using Server.Items;
using Server.Items.VystiaScrolls;

namespace Server.Mobiles
{
    /// <summary>
    /// Magic School Vendors - One vendor per school
    /// Each sells: reagents (8), scrolls (32), and empty spellbook for their school
    /// Auto-generated with 96-reagent system - 12 vendors total
    /// </summary>

    /// <summary>
    /// Ice Magic Vendor - Sells reagents, scrolls, and spellbooks
    /// </summary>
    public class IceMageVendor : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public IceMageVendor() : base("the ice mage vendor")
        {
            Hue = 0x481;
            SetSkill(SkillName.Magery, 100.0);
            SetSkill(SkillName.EvalInt, 100.0);
            SetSkill(SkillName.Inscribe, 100.0);
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBIceMage());
        }

        public IceMageVendor(Serial serial) : base(serial) { }

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

    public class SBIceMage : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new List<GenericBuyInfo>();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBIceMage()
        {
            // Spellbook (empty)
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spellbook", typeof(IceMageSpellbook), 150, 20, 0x1F2D, 0x481));

            // Reagents (8 total)
            m_BuyInfo.Add(new GenericBuyInfo("Frostbloom", typeof(Frostbloom), 5, 999, 0x18e9, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Glacier Crystal", typeof(GlacierCrystal), 5, 999, 0x1f19, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Winterleaf", typeof(Winterleaf), 5, 999, 0x18e1, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Permafrost Essence", typeof(PermafrostEssence), 5, 999, 0x1f1c, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Arctic Pearl", typeof(ArcticPearl), 5, 999, 0x1f47, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Frozen Soul", typeof(FrozenSoul), 5, 999, 0x1f13, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Frost Essence", typeof(FrostEssence), 5, 999, 0x1f9d, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Heart Of Winter", typeof(HeartOfWinter), 5, 999, 0x1f19, 0x481));

            // Spell Scrolls (32 total)
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell1_1 Scroll", typeof(IceMageSpell1_1Scroll), 10, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell1_2 Scroll", typeof(IceMageSpell1_2Scroll), 10, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell1_3 Scroll", typeof(IceMageSpell1_3Scroll), 10, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell1_4 Scroll", typeof(IceMageSpell1_4Scroll), 10, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell2_1 Scroll", typeof(IceMageSpell2_1Scroll), 15, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell2_2 Scroll", typeof(IceMageSpell2_2Scroll), 15, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell2_3 Scroll", typeof(IceMageSpell2_3Scroll), 15, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell2_4 Scroll", typeof(IceMageSpell2_4Scroll), 15, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell3_1 Scroll", typeof(IceMageSpell3_1Scroll), 20, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell3_2 Scroll", typeof(IceMageSpell3_2Scroll), 20, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell3_3 Scroll", typeof(IceMageSpell3_3Scroll), 20, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell3_4 Scroll", typeof(IceMageSpell3_4Scroll), 20, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell4_1 Scroll", typeof(IceMageSpell4_1Scroll), 25, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell4_2 Scroll", typeof(IceMageSpell4_2Scroll), 25, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell4_3 Scroll", typeof(IceMageSpell4_3Scroll), 25, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell4_4 Scroll", typeof(IceMageSpell4_4Scroll), 25, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell5_1 Scroll", typeof(IceMageSpell5_1Scroll), 30, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell5_2 Scroll", typeof(IceMageSpell5_2Scroll), 30, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell5_3 Scroll", typeof(IceMageSpell5_3Scroll), 30, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell5_4 Scroll", typeof(IceMageSpell5_4Scroll), 30, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell6_1 Scroll", typeof(IceMageSpell6_1Scroll), 35, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell6_2 Scroll", typeof(IceMageSpell6_2Scroll), 35, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell6_3 Scroll", typeof(IceMageSpell6_3Scroll), 35, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell6_4 Scroll", typeof(IceMageSpell6_4Scroll), 35, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell7_1 Scroll", typeof(IceMageSpell7_1Scroll), 40, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell7_2 Scroll", typeof(IceMageSpell7_2Scroll), 40, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell7_3 Scroll", typeof(IceMageSpell7_3Scroll), 40, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell7_4 Scroll", typeof(IceMageSpell7_4Scroll), 40, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell8_1 Scroll", typeof(IceMageSpell8_1Scroll), 45, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell8_2 Scroll", typeof(IceMageSpell8_2Scroll), 45, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell8_3 Scroll", typeof(IceMageSpell8_3Scroll), 45, 999, 0x1F2D, 0x481));
            m_BuyInfo.Add(new GenericBuyInfo("Ice Magic Spell8_4 Scroll", typeof(IceMageSpell8_4Scroll), 45, 999, 0x1F2D, 0x481));
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
            }
        }
    }

    /// <summary>
    /// Nature Magic Vendor - Sells reagents, scrolls, and spellbooks
    /// </summary>
    public class DruidVendor : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public DruidVendor() : base("the druid vendor")
        {
            Hue = 0x7D6;
            SetSkill(SkillName.Magery, 100.0);
            SetSkill(SkillName.EvalInt, 100.0);
            SetSkill(SkillName.Inscribe, 100.0);
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBDruid());
        }

        public DruidVendor(Serial serial) : base(serial) { }

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

    public class SBDruid : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new List<GenericBuyInfo>();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBDruid()
        {
            // Spellbook (empty)
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spellbook", typeof(DruidSpellbook), 150, 20, 0x1F2D, 0x7D6));

            // Reagents (8 total)
            m_BuyInfo.Add(new GenericBuyInfo("Wild Moss", typeof(WildMoss), 5, 999, 0x1aa2, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Moonpetal", typeof(Moonpetal), 5, 999, 0x18e9, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Druid Bark", typeof(DruidBark), 5, 999, 0x1bd7, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Treant Sap", typeof(TreantSap), 5, 999, 0x1f9d, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Elderwood Seed", typeof(ElderwoodSeed), 5, 999, 0x1f2c, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Primal Vine", typeof(PrimalVine), 5, 999, 0x1aa3, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Living Bark", typeof(LivingBark), 5, 999, 0x1bd7, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Ancient Root", typeof(AncientRoot), 5, 999, 0x1f1c, 0x7D6));

            // Spell Scrolls (32 total)
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell1_1 Scroll", typeof(DruidSpell1_1Scroll), 10, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell1_2 Scroll", typeof(DruidSpell1_2Scroll), 10, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell1_3 Scroll", typeof(DruidSpell1_3Scroll), 10, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell1_4 Scroll", typeof(DruidSpell1_4Scroll), 10, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell2_1 Scroll", typeof(DruidSpell2_1Scroll), 15, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell2_2 Scroll", typeof(DruidSpell2_2Scroll), 15, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell2_3 Scroll", typeof(DruidSpell2_3Scroll), 15, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell2_4 Scroll", typeof(DruidSpell2_4Scroll), 15, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell3_1 Scroll", typeof(DruidSpell3_1Scroll), 20, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell3_2 Scroll", typeof(DruidSpell3_2Scroll), 20, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell3_3 Scroll", typeof(DruidSpell3_3Scroll), 20, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell3_4 Scroll", typeof(DruidSpell3_4Scroll), 20, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell4_1 Scroll", typeof(DruidSpell4_1Scroll), 25, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell4_2 Scroll", typeof(DruidSpell4_2Scroll), 25, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell4_3 Scroll", typeof(DruidSpell4_3Scroll), 25, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell4_4 Scroll", typeof(DruidSpell4_4Scroll), 25, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell5_1 Scroll", typeof(DruidSpell5_1Scroll), 30, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell5_2 Scroll", typeof(DruidSpell5_2Scroll), 30, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell5_3 Scroll", typeof(DruidSpell5_3Scroll), 30, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell5_4 Scroll", typeof(DruidSpell5_4Scroll), 30, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell6_1 Scroll", typeof(DruidSpell6_1Scroll), 35, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell6_2 Scroll", typeof(DruidSpell6_2Scroll), 35, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell6_3 Scroll", typeof(DruidSpell6_3Scroll), 35, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell6_4 Scroll", typeof(DruidSpell6_4Scroll), 35, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell7_1 Scroll", typeof(DruidSpell7_1Scroll), 40, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell7_2 Scroll", typeof(DruidSpell7_2Scroll), 40, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell7_3 Scroll", typeof(DruidSpell7_3Scroll), 40, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell7_4 Scroll", typeof(DruidSpell7_4Scroll), 40, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell8_1 Scroll", typeof(DruidSpell8_1Scroll), 45, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell8_2 Scroll", typeof(DruidSpell8_2Scroll), 45, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell8_3 Scroll", typeof(DruidSpell8_3Scroll), 45, 999, 0x1F2D, 0x7D6));
            m_BuyInfo.Add(new GenericBuyInfo("Nature Magic Spell8_4 Scroll", typeof(DruidSpell8_4Scroll), 45, 999, 0x1F2D, 0x7D6));
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
            }
        }
    }

    /// <summary>
    /// Hex Magic Vendor - Sells reagents, scrolls, and spellbooks
    /// </summary>
    public class WitchVendor : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public WitchVendor() : base("the witch vendor")
        {
            Hue = 0x81D;
            SetSkill(SkillName.Magery, 100.0);
            SetSkill(SkillName.EvalInt, 100.0);
            SetSkill(SkillName.Inscribe, 100.0);
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBWitch());
        }

        public WitchVendor(Serial serial) : base(serial) { }

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

    public class SBWitch : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new List<GenericBuyInfo>();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBWitch()
        {
            // Spellbook (empty)
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spellbook", typeof(WitchSpellbook), 150, 20, 0x1F2D, 0x81D));

            // Reagents (8 total)
            m_BuyInfo.Add(new GenericBuyInfo("Bog Moss", typeof(BogMoss), 5, 999, 0x1aa2, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Viper Fang", typeof(ViperFang), 5, 999, 0x1f26, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Witchweed", typeof(Witchweed), 5, 999, 0x18e1, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Toads Eye", typeof(ToadsEye), 5, 999, 0x1f2f, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Swamp Lotus", typeof(SwampLotus), 5, 999, 0x18e9, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hags Hair", typeof(HagsHair), 5, 999, 0x1aa4, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Cursed Pearl", typeof(CursedPearl), 5, 999, 0x1f47, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Cursed Salt", typeof(CursedSalt), 5, 999, 0x11ea, 0x81D));

            // Spell Scrolls (32 total)
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell1_1 Scroll", typeof(WitchSpell1_1Scroll), 10, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell1_2 Scroll", typeof(WitchSpell1_2Scroll), 10, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell1_3 Scroll", typeof(WitchSpell1_3Scroll), 10, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell1_4 Scroll", typeof(WitchSpell1_4Scroll), 10, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell2_1 Scroll", typeof(WitchSpell2_1Scroll), 15, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell2_2 Scroll", typeof(WitchSpell2_2Scroll), 15, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell2_3 Scroll", typeof(WitchSpell2_3Scroll), 15, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell2_4 Scroll", typeof(WitchSpell2_4Scroll), 15, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell3_1 Scroll", typeof(WitchSpell3_1Scroll), 20, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell3_2 Scroll", typeof(WitchSpell3_2Scroll), 20, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell3_3 Scroll", typeof(WitchSpell3_3Scroll), 20, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell3_4 Scroll", typeof(WitchSpell3_4Scroll), 20, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell4_1 Scroll", typeof(WitchSpell4_1Scroll), 25, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell4_2 Scroll", typeof(WitchSpell4_2Scroll), 25, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell4_3 Scroll", typeof(WitchSpell4_3Scroll), 25, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell4_4 Scroll", typeof(WitchSpell4_4Scroll), 25, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell5_1 Scroll", typeof(WitchSpell5_1Scroll), 30, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell5_2 Scroll", typeof(WitchSpell5_2Scroll), 30, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell5_3 Scroll", typeof(WitchSpell5_3Scroll), 30, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell5_4 Scroll", typeof(WitchSpell5_4Scroll), 30, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell6_1 Scroll", typeof(WitchSpell6_1Scroll), 35, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell6_2 Scroll", typeof(WitchSpell6_2Scroll), 35, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell6_3 Scroll", typeof(WitchSpell6_3Scroll), 35, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell6_4 Scroll", typeof(WitchSpell6_4Scroll), 35, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell7_1 Scroll", typeof(WitchSpell7_1Scroll), 40, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell7_2 Scroll", typeof(WitchSpell7_2Scroll), 40, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell7_3 Scroll", typeof(WitchSpell7_3Scroll), 40, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell7_4 Scroll", typeof(WitchSpell7_4Scroll), 40, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell8_1 Scroll", typeof(WitchSpell8_1Scroll), 45, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell8_2 Scroll", typeof(WitchSpell8_2Scroll), 45, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell8_3 Scroll", typeof(WitchSpell8_3Scroll), 45, 999, 0x1F2D, 0x81D));
            m_BuyInfo.Add(new GenericBuyInfo("Hex Magic Spell8_4 Scroll", typeof(WitchSpell8_4Scroll), 45, 999, 0x1F2D, 0x81D));
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
            }
        }
    }

    /// <summary>
    /// Elemental Magic Vendor - Sells reagents, scrolls, and spellbooks
    /// </summary>
    public class SorcererVendor : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public SorcererVendor() : base("the sorcerer vendor")
        {
            Hue = 0x54E;
            SetSkill(SkillName.Magery, 100.0);
            SetSkill(SkillName.EvalInt, 100.0);
            SetSkill(SkillName.Inscribe, 100.0);
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBSorcerer());
        }

        public SorcererVendor(Serial serial) : base(serial) { }

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

    public class SBSorcerer : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new List<GenericBuyInfo>();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBSorcerer()
        {
            // Spellbook (empty)
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spellbook", typeof(SorcererSpellbook), 150, 20, 0x1F2D, 0x54E));

            // Reagents (8 total)
            m_BuyInfo.Add(new GenericBuyInfo("Ash Petal", typeof(AshPetal), 5, 999, 0x18e9, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Lava Glass", typeof(LavaGlass), 5, 999, 0x1f19, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Flameweed", typeof(Flameweed), 5, 999, 0x18e1, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Magma Essence", typeof(MagmaEssence), 5, 999, 0x1f9d, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Phoenix Feather", typeof(PhoenixFeather), 5, 999, 0x1cff, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Dragon Heart", typeof(DragonHeart), 5, 999, 0x1f13, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Primordial Ember", typeof(PrimordialEmber), 5, 999, 0x19ac, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Core", typeof(ElementalCore), 5, 999, 0x1f13, 0x54E));

            // Spell Scrolls (32 total)
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell1_1 Scroll", typeof(SorcererSpell1_1Scroll), 10, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell1_2 Scroll", typeof(SorcererSpell1_2Scroll), 10, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell1_3 Scroll", typeof(SorcererSpell1_3Scroll), 10, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell1_4 Scroll", typeof(SorcererSpell1_4Scroll), 10, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell2_1 Scroll", typeof(SorcererSpell2_1Scroll), 15, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell2_2 Scroll", typeof(SorcererSpell2_2Scroll), 15, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell2_3 Scroll", typeof(SorcererSpell2_3Scroll), 15, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell2_4 Scroll", typeof(SorcererSpell2_4Scroll), 15, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell3_1 Scroll", typeof(SorcererSpell3_1Scroll), 20, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell3_2 Scroll", typeof(SorcererSpell3_2Scroll), 20, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell3_3 Scroll", typeof(SorcererSpell3_3Scroll), 20, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell3_4 Scroll", typeof(SorcererSpell3_4Scroll), 20, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell4_1 Scroll", typeof(SorcererSpell4_1Scroll), 25, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell4_2 Scroll", typeof(SorcererSpell4_2Scroll), 25, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell4_3 Scroll", typeof(SorcererSpell4_3Scroll), 25, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell4_4 Scroll", typeof(SorcererSpell4_4Scroll), 25, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell5_1 Scroll", typeof(SorcererSpell5_1Scroll), 30, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell5_2 Scroll", typeof(SorcererSpell5_2Scroll), 30, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell5_3 Scroll", typeof(SorcererSpell5_3Scroll), 30, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell5_4 Scroll", typeof(SorcererSpell5_4Scroll), 30, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell6_1 Scroll", typeof(SorcererSpell6_1Scroll), 35, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell6_2 Scroll", typeof(SorcererSpell6_2Scroll), 35, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell6_3 Scroll", typeof(SorcererSpell6_3Scroll), 35, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell6_4 Scroll", typeof(SorcererSpell6_4Scroll), 35, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell7_1 Scroll", typeof(SorcererSpell7_1Scroll), 40, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell7_2 Scroll", typeof(SorcererSpell7_2Scroll), 40, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell7_3 Scroll", typeof(SorcererSpell7_3Scroll), 40, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell7_4 Scroll", typeof(SorcererSpell7_4Scroll), 40, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell8_1 Scroll", typeof(SorcererSpell8_1Scroll), 45, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell8_2 Scroll", typeof(SorcererSpell8_2Scroll), 45, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell8_3 Scroll", typeof(SorcererSpell8_3Scroll), 45, 999, 0x1F2D, 0x54E));
            m_BuyInfo.Add(new GenericBuyInfo("Elemental Magic Spell8_4 Scroll", typeof(SorcererSpell8_4Scroll), 45, 999, 0x1F2D, 0x54E));
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
            }
        }
    }

    /// <summary>
    /// Dark Magic Vendor - Sells reagents, scrolls, and spellbooks
    /// </summary>
    public class WarlockVendor : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public WarlockVendor() : base("the warlock vendor")
        {
            Hue = 0x455;
            SetSkill(SkillName.Magery, 100.0);
            SetSkill(SkillName.EvalInt, 100.0);
            SetSkill(SkillName.Inscribe, 100.0);
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBWarlock());
        }

        public WarlockVendor(Serial serial) : base(serial) { }

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

    public class SBWarlock : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new List<GenericBuyInfo>();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBWarlock()
        {
            // Spellbook (empty)
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spellbook", typeof(WarlockSpellbook), 150, 20, 0x1F2D, 0x455));

            // Reagents (8 total)
            m_BuyInfo.Add(new GenericBuyInfo("Shadow Moss", typeof(ShadowMoss), 5, 999, 0x1aa2, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Void Crystal", typeof(VoidCrystal), 5, 999, 0x1f19, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Void Weed", typeof(VoidWeed), 5, 999, 0x18e1, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Shadow Petal", typeof(ShadowPetal), 5, 999, 0x18e9, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Void Dust", typeof(VoidDust), 5, 999, 0x26b8, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Void Silk", typeof(VoidSilk), 5, 999, 0x1aa4, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Demon Heart", typeof(DemonHeart), 5, 999, 0x1f13, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Shadow Essence", typeof(ShadowEssence), 5, 999, 0x1f9d, 0x455));

            // Spell Scrolls (32 total)
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell1_1 Scroll", typeof(WarlockSpell1_1Scroll), 10, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell1_2 Scroll", typeof(WarlockSpell1_2Scroll), 10, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell1_3 Scroll", typeof(WarlockSpell1_3Scroll), 10, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell1_4 Scroll", typeof(WarlockSpell1_4Scroll), 10, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell2_1 Scroll", typeof(WarlockSpell2_1Scroll), 15, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell2_2 Scroll", typeof(WarlockSpell2_2Scroll), 15, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell2_3 Scroll", typeof(WarlockSpell2_3Scroll), 15, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell2_4 Scroll", typeof(WarlockSpell2_4Scroll), 15, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell3_1 Scroll", typeof(WarlockSpell3_1Scroll), 20, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell3_2 Scroll", typeof(WarlockSpell3_2Scroll), 20, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell3_3 Scroll", typeof(WarlockSpell3_3Scroll), 20, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell3_4 Scroll", typeof(WarlockSpell3_4Scroll), 20, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell4_1 Scroll", typeof(WarlockSpell4_1Scroll), 25, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell4_2 Scroll", typeof(WarlockSpell4_2Scroll), 25, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell4_3 Scroll", typeof(WarlockSpell4_3Scroll), 25, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell4_4 Scroll", typeof(WarlockSpell4_4Scroll), 25, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell5_1 Scroll", typeof(WarlockSpell5_1Scroll), 30, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell5_2 Scroll", typeof(WarlockSpell5_2Scroll), 30, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell5_3 Scroll", typeof(WarlockSpell5_3Scroll), 30, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell5_4 Scroll", typeof(WarlockSpell5_4Scroll), 30, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell6_1 Scroll", typeof(WarlockSpell6_1Scroll), 35, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell6_2 Scroll", typeof(WarlockSpell6_2Scroll), 35, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell6_3 Scroll", typeof(WarlockSpell6_3Scroll), 35, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell6_4 Scroll", typeof(WarlockSpell6_4Scroll), 35, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell7_1 Scroll", typeof(WarlockSpell7_1Scroll), 40, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell7_2 Scroll", typeof(WarlockSpell7_2Scroll), 40, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell7_3 Scroll", typeof(WarlockSpell7_3Scroll), 40, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell7_4 Scroll", typeof(WarlockSpell7_4Scroll), 40, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell8_1 Scroll", typeof(WarlockSpell8_1Scroll), 45, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell8_2 Scroll", typeof(WarlockSpell8_2Scroll), 45, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell8_3 Scroll", typeof(WarlockSpell8_3Scroll), 45, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Dark Magic Spell8_4 Scroll", typeof(WarlockSpell8_4Scroll), 45, 999, 0x1F2D, 0x455));
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
            }
        }
    }

    /// <summary>
    /// Divination Magic Vendor - Sells reagents, scrolls, and spellbooks
    /// </summary>
    public class OracleVendor : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public OracleVendor() : base("the oracle vendor")
        {
            Hue = 0x482;
            SetSkill(SkillName.Magery, 100.0);
            SetSkill(SkillName.EvalInt, 100.0);
            SetSkill(SkillName.Inscribe, 100.0);
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBOracle());
        }

        public OracleVendor(Serial serial) : base(serial) { }

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

    public class SBOracle : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new List<GenericBuyInfo>();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBOracle()
        {
            // Spellbook (empty)
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spellbook", typeof(OracleSpellbook), 150, 20, 0x1F2D, 0x482));

            // Reagents (8 total)
            m_BuyInfo.Add(new GenericBuyInfo("Time Sand", typeof(TimeSand), 5, 999, 0x11ea, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Time Dust", typeof(TimeDust), 5, 999, 0x26b8, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Dust", typeof(DivinationDust), 5, 999, 0x26b8, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Fate Crystal", typeof(FateCrystal), 5, 999, 0x1f19, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Starlight Crystal", typeof(StarlightCrystal), 5, 999, 0x1f19, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Prophetic Leaf", typeof(PropheticLeaf), 5, 999, 0x18e1, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Seeing Stone", typeof(SeeingStone), 5, 999, 0x1f19, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Fate Thread", typeof(FateThread), 5, 999, 0x1aa4, 0x482));

            // Spell Scrolls (32 total)
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell1_1 Scroll", typeof(OracleSpell1_1Scroll), 10, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell1_2 Scroll", typeof(OracleSpell1_2Scroll), 10, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell1_3 Scroll", typeof(OracleSpell1_3Scroll), 10, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell1_4 Scroll", typeof(OracleSpell1_4Scroll), 10, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell2_1 Scroll", typeof(OracleSpell2_1Scroll), 15, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell2_2 Scroll", typeof(OracleSpell2_2Scroll), 15, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell2_3 Scroll", typeof(OracleSpell2_3Scroll), 15, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell2_4 Scroll", typeof(OracleSpell2_4Scroll), 15, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell3_1 Scroll", typeof(OracleSpell3_1Scroll), 20, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell3_2 Scroll", typeof(OracleSpell3_2Scroll), 20, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell3_3 Scroll", typeof(OracleSpell3_3Scroll), 20, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell3_4 Scroll", typeof(OracleSpell3_4Scroll), 20, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell4_1 Scroll", typeof(OracleSpell4_1Scroll), 25, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell4_2 Scroll", typeof(OracleSpell4_2Scroll), 25, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell4_3 Scroll", typeof(OracleSpell4_3Scroll), 25, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell4_4 Scroll", typeof(OracleSpell4_4Scroll), 25, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell5_1 Scroll", typeof(OracleSpell5_1Scroll), 30, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell5_2 Scroll", typeof(OracleSpell5_2Scroll), 30, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell5_3 Scroll", typeof(OracleSpell5_3Scroll), 30, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell5_4 Scroll", typeof(OracleSpell5_4Scroll), 30, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell6_1 Scroll", typeof(OracleSpell6_1Scroll), 35, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell6_2 Scroll", typeof(OracleSpell6_2Scroll), 35, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell6_3 Scroll", typeof(OracleSpell6_3Scroll), 35, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell6_4 Scroll", typeof(OracleSpell6_4Scroll), 35, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell7_1 Scroll", typeof(OracleSpell7_1Scroll), 40, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell7_2 Scroll", typeof(OracleSpell7_2Scroll), 40, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell7_3 Scroll", typeof(OracleSpell7_3Scroll), 40, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell7_4 Scroll", typeof(OracleSpell7_4Scroll), 40, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell8_1 Scroll", typeof(OracleSpell8_1Scroll), 45, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell8_2 Scroll", typeof(OracleSpell8_2Scroll), 45, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell8_3 Scroll", typeof(OracleSpell8_3Scroll), 45, 999, 0x1F2D, 0x482));
            m_BuyInfo.Add(new GenericBuyInfo("Divination Magic Spell8_4 Scroll", typeof(OracleSpell8_4Scroll), 45, 999, 0x1F2D, 0x482));
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
            }
        }
    }

    /// <summary>
    /// Necromancy Vendor - Sells reagents, scrolls, and spellbooks
    /// </summary>
    public class NecromancerVendor : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public NecromancerVendor() : base("the necromancer vendor")
        {
            Hue = 0x455;
            SetSkill(SkillName.Magery, 100.0);
            SetSkill(SkillName.EvalInt, 100.0);
            SetSkill(SkillName.Inscribe, 100.0);
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBVystiaNecromancer());
        }

        public NecromancerVendor(Serial serial) : base(serial) { }

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

    public class SBVystiaNecromancer : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new List<GenericBuyInfo>();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBVystiaNecromancer()
        {
            // Spellbook (empty)
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spellbook", typeof(VystiaNecromancerSpellbook), 150, 20, 0x1F2D, 0x455));

            // Reagents (8 total)
            m_BuyInfo.Add(new GenericBuyInfo("Grave Moss", typeof(GraveMoss), 5, 999, 0x1aa2, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Bone Dust", typeof(BoneDust), 5, 999, 0x26b8, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Corpse Ash", typeof(CorpseAsh), 5, 999, 0x26b8, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Soul Fragment", typeof(SoulFragment), 5, 999, 0x1f1c, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necrotic Shroud", typeof(NecroticShroud), 5, 999, 0x1aa4, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Lich Dust", typeof(LichDust), 5, 999, 0x26b8, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Phylactery Shard", typeof(PhylacteryShard), 5, 999, 0x1f1c, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Reaper Essence", typeof(ReaperEssence), 5, 999, 0x1f9d, 0x455));

            // Spell Scrolls (32 total)
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell1_1 Scroll", typeof(NecromancerSpell1_1Scroll), 10, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell1_2 Scroll", typeof(NecromancerSpell1_2Scroll), 10, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell1_3 Scroll", typeof(NecromancerSpell1_3Scroll), 10, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell1_4 Scroll", typeof(NecromancerSpell1_4Scroll), 10, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell2_1 Scroll", typeof(NecromancerSpell2_1Scroll), 15, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell2_2 Scroll", typeof(NecromancerSpell2_2Scroll), 15, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell2_3 Scroll", typeof(NecromancerSpell2_3Scroll), 15, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell2_4 Scroll", typeof(NecromancerSpell2_4Scroll), 15, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell3_1 Scroll", typeof(NecromancerSpell3_1Scroll), 20, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell3_2 Scroll", typeof(NecromancerSpell3_2Scroll), 20, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell3_3 Scroll", typeof(NecromancerSpell3_3Scroll), 20, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell3_4 Scroll", typeof(NecromancerSpell3_4Scroll), 20, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell4_1 Scroll", typeof(NecromancerSpell4_1Scroll), 25, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell4_2 Scroll", typeof(NecromancerSpell4_2Scroll), 25, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell4_3 Scroll", typeof(NecromancerSpell4_3Scroll), 25, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell4_4 Scroll", typeof(NecromancerSpell4_4Scroll), 25, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell5_1 Scroll", typeof(NecromancerSpell5_1Scroll), 30, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell5_2 Scroll", typeof(NecromancerSpell5_2Scroll), 30, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell5_3 Scroll", typeof(NecromancerSpell5_3Scroll), 30, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell5_4 Scroll", typeof(NecromancerSpell5_4Scroll), 30, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell6_1 Scroll", typeof(NecromancerSpell6_1Scroll), 35, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell6_2 Scroll", typeof(NecromancerSpell6_2Scroll), 35, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell6_3 Scroll", typeof(NecromancerSpell6_3Scroll), 35, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell6_4 Scroll", typeof(NecromancerSpell6_4Scroll), 35, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell7_1 Scroll", typeof(NecromancerSpell7_1Scroll), 40, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell7_2 Scroll", typeof(NecromancerSpell7_2Scroll), 40, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell7_3 Scroll", typeof(NecromancerSpell7_3Scroll), 40, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell7_4 Scroll", typeof(NecromancerSpell7_4Scroll), 40, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell8_1 Scroll", typeof(NecromancerSpell8_1Scroll), 45, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell8_2 Scroll", typeof(NecromancerSpell8_2Scroll), 45, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell8_3 Scroll", typeof(NecromancerSpell8_3Scroll), 45, 999, 0x1F2D, 0x455));
            m_BuyInfo.Add(new GenericBuyInfo("Necromancy Spell8_4 Scroll", typeof(NecromancerSpell8_4Scroll), 45, 999, 0x1F2D, 0x455));
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
            }
        }
    }

    /// <summary>
    /// Summoning Magic Vendor - Sells reagents, scrolls, and spellbooks
    /// </summary>
    public class SummonerVendor : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public SummonerVendor() : base("the summoner vendor")
        {
            Hue = 0x555;
            SetSkill(SkillName.Magery, 100.0);
            SetSkill(SkillName.EvalInt, 100.0);
            SetSkill(SkillName.Inscribe, 100.0);
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBSummoner());
        }

        public SummonerVendor(Serial serial) : base(serial) { }

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

    public class SBSummoner : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new List<GenericBuyInfo>();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBSummoner()
        {
            // Spellbook (empty)
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spellbook", typeof(SummonerSpellbook), 150, 20, 0x1F2D, 0x555));

            // Reagents (8 total)
            m_BuyInfo.Add(new GenericBuyInfo("Planar Dust", typeof(PlanarDust), 5, 999, 0x26b8, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Ether Shard", typeof(EtherShard), 5, 999, 0x1f1c, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Aether Shard", typeof(AetherShard), 5, 999, 0x1f1c, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Crystal", typeof(SummoningCrystal), 5, 999, 0x1f19, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Chaos Shard", typeof(ChaosShard), 5, 999, 0x1f1c, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Binding Rune", typeof(BindingRune), 5, 999, 0x1f14, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Dimensional Key", typeof(DimensionalKey), 5, 999, 0x1f47, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Salt", typeof(SummoningSalt), 5, 999, 0x11ea, 0x555));

            // Spell Scrolls (32 total)
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell1_1 Scroll", typeof(SummonerSpell1_1Scroll), 10, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell1_2 Scroll", typeof(SummonerSpell1_2Scroll), 10, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell1_3 Scroll", typeof(SummonerSpell1_3Scroll), 10, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell1_4 Scroll", typeof(SummonerSpell1_4Scroll), 10, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell2_1 Scroll", typeof(SummonerSpell2_1Scroll), 15, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell2_2 Scroll", typeof(SummonerSpell2_2Scroll), 15, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell2_3 Scroll", typeof(SummonerSpell2_3Scroll), 15, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell2_4 Scroll", typeof(SummonerSpell2_4Scroll), 15, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell3_1 Scroll", typeof(SummonerSpell3_1Scroll), 20, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell3_2 Scroll", typeof(SummonerSpell3_2Scroll), 20, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell3_3 Scroll", typeof(SummonerSpell3_3Scroll), 20, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell3_4 Scroll", typeof(SummonerSpell3_4Scroll), 20, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell4_1 Scroll", typeof(SummonerSpell4_1Scroll), 25, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell4_2 Scroll", typeof(SummonerSpell4_2Scroll), 25, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell4_3 Scroll", typeof(SummonerSpell4_3Scroll), 25, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell4_4 Scroll", typeof(SummonerSpell4_4Scroll), 25, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell5_1 Scroll", typeof(SummonerSpell5_1Scroll), 30, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell5_2 Scroll", typeof(SummonerSpell5_2Scroll), 30, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell5_3 Scroll", typeof(SummonerSpell5_3Scroll), 30, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell5_4 Scroll", typeof(SummonerSpell5_4Scroll), 30, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell6_1 Scroll", typeof(SummonerSpell6_1Scroll), 35, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell6_2 Scroll", typeof(SummonerSpell6_2Scroll), 35, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell6_3 Scroll", typeof(SummonerSpell6_3Scroll), 35, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell6_4 Scroll", typeof(SummonerSpell6_4Scroll), 35, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell7_1 Scroll", typeof(SummonerSpell7_1Scroll), 40, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell7_2 Scroll", typeof(SummonerSpell7_2Scroll), 40, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell7_3 Scroll", typeof(SummonerSpell7_3Scroll), 40, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell7_4 Scroll", typeof(SummonerSpell7_4Scroll), 40, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell8_1 Scroll", typeof(SummonerSpell8_1Scroll), 45, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell8_2 Scroll", typeof(SummonerSpell8_2Scroll), 45, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell8_3 Scroll", typeof(SummonerSpell8_3Scroll), 45, 999, 0x1F2D, 0x555));
            m_BuyInfo.Add(new GenericBuyInfo("Summoning Magic Spell8_4 Scroll", typeof(SummonerSpell8_4Scroll), 45, 999, 0x1F2D, 0x555));
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
            }
        }
    }

    /// <summary>
    /// Shamanic Magic Vendor - Sells reagents, scrolls, and spellbooks
    /// </summary>
    public class ShamanVendor : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public ShamanVendor() : base("the shaman vendor")
        {
            Hue = 0x501;
            SetSkill(SkillName.Magery, 100.0);
            SetSkill(SkillName.EvalInt, 100.0);
            SetSkill(SkillName.Inscribe, 100.0);
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBShaman());
        }

        public ShamanVendor(Serial serial) : base(serial) { }

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

    public class SBShaman : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new List<GenericBuyInfo>();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBShaman()
        {
            // Spellbook (empty)
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spellbook", typeof(ShamanSpellbook), 150, 20, 0x1F2D, 0x501));

            // Reagents (8 total)
            m_BuyInfo.Add(new GenericBuyInfo("Lightning Root", typeof(LightningRoot), 5, 999, 0x18e1, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Thunder Moss", typeof(ThunderMoss), 5, 999, 0x1aa2, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Storm Crystal", typeof(StormCrystal), 5, 999, 0x1f19, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Storm Essence", typeof(StormEssence), 5, 999, 0x1f9d, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Spirit Feather", typeof(SpiritFeather), 5, 999, 0x1cff, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Primal Thunder", typeof(PrimalThunder), 5, 999, 0x1f13, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Totem Carving", typeof(TotemCarving), 5, 999, 0x1bd7, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Wind Essence", typeof(WindEssence), 5, 999, 0x1f9d, 0x501));

            // Spell Scrolls (32 total)
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell1_1 Scroll", typeof(ShamanSpell1_1Scroll), 10, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell1_2 Scroll", typeof(ShamanSpell1_2Scroll), 10, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell1_3 Scroll", typeof(ShamanSpell1_3Scroll), 10, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell1_4 Scroll", typeof(ShamanSpell1_4Scroll), 10, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell2_1 Scroll", typeof(ShamanSpell2_1Scroll), 15, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell2_2 Scroll", typeof(ShamanSpell2_2Scroll), 15, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell2_3 Scroll", typeof(ShamanSpell2_3Scroll), 15, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell2_4 Scroll", typeof(ShamanSpell2_4Scroll), 15, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell3_1 Scroll", typeof(ShamanSpell3_1Scroll), 20, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell3_2 Scroll", typeof(ShamanSpell3_2Scroll), 20, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell3_3 Scroll", typeof(ShamanSpell3_3Scroll), 20, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell3_4 Scroll", typeof(ShamanSpell3_4Scroll), 20, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell4_1 Scroll", typeof(ShamanSpell4_1Scroll), 25, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell4_2 Scroll", typeof(ShamanSpell4_2Scroll), 25, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell4_3 Scroll", typeof(ShamanSpell4_3Scroll), 25, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell4_4 Scroll", typeof(ShamanSpell4_4Scroll), 25, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell5_1 Scroll", typeof(ShamanSpell5_1Scroll), 30, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell5_2 Scroll", typeof(ShamanSpell5_2Scroll), 30, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell5_3 Scroll", typeof(ShamanSpell5_3Scroll), 30, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell5_4 Scroll", typeof(ShamanSpell5_4Scroll), 30, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell6_1 Scroll", typeof(ShamanSpell6_1Scroll), 35, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell6_2 Scroll", typeof(ShamanSpell6_2Scroll), 35, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell6_3 Scroll", typeof(ShamanSpell6_3Scroll), 35, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell6_4 Scroll", typeof(ShamanSpell6_4Scroll), 35, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell7_1 Scroll", typeof(ShamanSpell7_1Scroll), 40, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell7_2 Scroll", typeof(ShamanSpell7_2Scroll), 40, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell7_3 Scroll", typeof(ShamanSpell7_3Scroll), 40, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell7_4 Scroll", typeof(ShamanSpell7_4Scroll), 40, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell8_1 Scroll", typeof(ShamanSpell8_1Scroll), 45, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell8_2 Scroll", typeof(ShamanSpell8_2Scroll), 45, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell8_3 Scroll", typeof(ShamanSpell8_3Scroll), 45, 999, 0x1F2D, 0x501));
            m_BuyInfo.Add(new GenericBuyInfo("Shamanic Magic Spell8_4 Scroll", typeof(ShamanSpell8_4Scroll), 45, 999, 0x1F2D, 0x501));
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
            }
        }
    }

    /// <summary>
    /// Bardic Magic Vendor - Sells reagents, scrolls, and spellbooks
    /// </summary>
    public class BardVendor : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public BardVendor() : base("the bard vendor")
        {
            Hue = 0x8A5;
            SetSkill(SkillName.Magery, 100.0);
            SetSkill(SkillName.EvalInt, 100.0);
            SetSkill(SkillName.Inscribe, 100.0);
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBVystiaBard());
        }

        public BardVendor(Serial serial) : base(serial) { }

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

    public class SBVystiaBard : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new List<GenericBuyInfo>();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBVystiaBard()
        {
            // Spellbook (empty)
            m_BuyInfo.Add(new GenericBuyInfo("Songbook of Weaving", typeof(SongweavingSpellbook), 150, 20, 0x1F2D, 0x8A5));

            // Reagents (8 total)
            m_BuyInfo.Add(new GenericBuyInfo("Song Petal", typeof(SongPetal), 5, 999, 0x18e9, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Echo Dust", typeof(EchoDust), 5, 999, 0x26b8, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Voice Crystal", typeof(VoiceCrystal), 5, 999, 0x1f19, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Muse Essence", typeof(MuseEssence), 5, 999, 0x1f9d, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Harmony Gem", typeof(HarmonyGem), 5, 999, 0x1f19, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Eternal Note", typeof(EternalNote), 5, 999, 0x1aa4, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Golden String", typeof(GoldenString), 5, 999, 0x1aa4, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Dragon Scale", typeof(DragonScale), 5, 999, 0x1f26, 0x8A5));

            // Spell Scrolls (32 total)
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell1_1 Scroll", typeof(BardSpell1_1Scroll), 10, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell1_2 Scroll", typeof(BardSpell1_2Scroll), 10, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell1_3 Scroll", typeof(BardSpell1_3Scroll), 10, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell1_4 Scroll", typeof(BardSpell1_4Scroll), 10, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell2_1 Scroll", typeof(BardSpell2_1Scroll), 15, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell2_2 Scroll", typeof(BardSpell2_2Scroll), 15, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell2_3 Scroll", typeof(BardSpell2_3Scroll), 15, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell2_4 Scroll", typeof(BardSpell2_4Scroll), 15, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell3_1 Scroll", typeof(BardSpell3_1Scroll), 20, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell3_2 Scroll", typeof(BardSpell3_2Scroll), 20, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell3_3 Scroll", typeof(BardSpell3_3Scroll), 20, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell3_4 Scroll", typeof(BardSpell3_4Scroll), 20, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell4_1 Scroll", typeof(BardSpell4_1Scroll), 25, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell4_2 Scroll", typeof(BardSpell4_2Scroll), 25, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell4_3 Scroll", typeof(BardSpell4_3Scroll), 25, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell4_4 Scroll", typeof(BardSpell4_4Scroll), 25, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell5_1 Scroll", typeof(BardSpell5_1Scroll), 30, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell5_2 Scroll", typeof(BardSpell5_2Scroll), 30, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell5_3 Scroll", typeof(BardSpell5_3Scroll), 30, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell5_4 Scroll", typeof(BardSpell5_4Scroll), 30, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell6_1 Scroll", typeof(BardSpell6_1Scroll), 35, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell6_2 Scroll", typeof(BardSpell6_2Scroll), 35, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell6_3 Scroll", typeof(BardSpell6_3Scroll), 35, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell6_4 Scroll", typeof(BardSpell6_4Scroll), 35, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell7_1 Scroll", typeof(BardSpell7_1Scroll), 40, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell7_2 Scroll", typeof(BardSpell7_2Scroll), 40, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell7_3 Scroll", typeof(BardSpell7_3Scroll), 40, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell7_4 Scroll", typeof(BardSpell7_4Scroll), 40, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell8_1 Scroll", typeof(BardSpell8_1Scroll), 45, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell8_2 Scroll", typeof(BardSpell8_2Scroll), 45, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell8_3 Scroll", typeof(BardSpell8_3Scroll), 45, 999, 0x1F2D, 0x8A5));
            m_BuyInfo.Add(new GenericBuyInfo("Bardic Magic Spell8_4 Scroll", typeof(BardSpell8_4Scroll), 45, 999, 0x1F2D, 0x8A5));
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
            }
        }
    }

    /// <summary>
    /// Enchanting Magic Vendor - Sells reagents, scrolls, and spellbooks
    /// </summary>
    public class EnchanterVendor : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public EnchanterVendor() : base("the enchanter vendor")
        {
            Hue = 0x8FD;
            SetSkill(SkillName.Magery, 100.0);
            SetSkill(SkillName.EvalInt, 100.0);
            SetSkill(SkillName.Inscribe, 100.0);
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBEnchanter());
        }

        public EnchanterVendor(Serial serial) : base(serial) { }

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

    public class SBEnchanter : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new List<GenericBuyInfo>();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBEnchanter()
        {
            // Spellbook (empty)
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spellbook", typeof(EnchanterSpellbook), 150, 20, 0x1F2D, 0x8FD));

            // Reagents (8 total)
            m_BuyInfo.Add(new GenericBuyInfo("Arcane Dust", typeof(ArcaneDust), 5, 999, 0x26b8, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Essence Of Magic", typeof(EssenceOfMagic), 5, 999, 0x1f9d, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Mana Crystal", typeof(ManaCrystal), 5, 999, 0x1f19, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Ley Line Essence", typeof(LeyLineEssence), 5, 999, 0x1f9d, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Ley Line Shard", typeof(LeyLineShard), 5, 999, 0x1f1c, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Rune Fragment", typeof(RuneFragment), 5, 999, 0x1f14, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Runic Powder", typeof(RunicPowder), 5, 999, 0x26b8, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Titan Rune", typeof(TitanRune), 5, 999, 0x1f14, 0x8FD));

            // Spell Scrolls (32 total)
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell1_1 Scroll", typeof(EnchanterSpell1_1Scroll), 10, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell1_2 Scroll", typeof(EnchanterSpell1_2Scroll), 10, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell1_3 Scroll", typeof(EnchanterSpell1_3Scroll), 10, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell1_4 Scroll", typeof(EnchanterSpell1_4Scroll), 10, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell2_1 Scroll", typeof(EnchanterSpell2_1Scroll), 15, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell2_2 Scroll", typeof(EnchanterSpell2_2Scroll), 15, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell2_3 Scroll", typeof(EnchanterSpell2_3Scroll), 15, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell2_4 Scroll", typeof(EnchanterSpell2_4Scroll), 15, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell3_1 Scroll", typeof(EnchanterSpell3_1Scroll), 20, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell3_2 Scroll", typeof(EnchanterSpell3_2Scroll), 20, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell3_3 Scroll", typeof(EnchanterSpell3_3Scroll), 20, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell3_4 Scroll", typeof(EnchanterSpell3_4Scroll), 20, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell4_1 Scroll", typeof(EnchanterSpell4_1Scroll), 25, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell4_2 Scroll", typeof(EnchanterSpell4_2Scroll), 25, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell4_3 Scroll", typeof(EnchanterSpell4_3Scroll), 25, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell4_4 Scroll", typeof(EnchanterSpell4_4Scroll), 25, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell5_1 Scroll", typeof(EnchanterSpell5_1Scroll), 30, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell5_2 Scroll", typeof(EnchanterSpell5_2Scroll), 30, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell5_3 Scroll", typeof(EnchanterSpell5_3Scroll), 30, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell5_4 Scroll", typeof(EnchanterSpell5_4Scroll), 30, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell6_1 Scroll", typeof(EnchanterSpell6_1Scroll), 35, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell6_2 Scroll", typeof(EnchanterSpell6_2Scroll), 35, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell6_3 Scroll", typeof(EnchanterSpell6_3Scroll), 35, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell6_4 Scroll", typeof(EnchanterSpell6_4Scroll), 35, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell7_1 Scroll", typeof(EnchanterSpell7_1Scroll), 40, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell7_2 Scroll", typeof(EnchanterSpell7_2Scroll), 40, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell7_3 Scroll", typeof(EnchanterSpell7_3Scroll), 40, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell7_4 Scroll", typeof(EnchanterSpell7_4Scroll), 40, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell8_1 Scroll", typeof(EnchanterSpell8_1Scroll), 45, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell8_2 Scroll", typeof(EnchanterSpell8_2Scroll), 45, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell8_3 Scroll", typeof(EnchanterSpell8_3Scroll), 45, 999, 0x1F2D, 0x8FD));
            m_BuyInfo.Add(new GenericBuyInfo("Enchanting Magic Spell8_4 Scroll", typeof(EnchanterSpell8_4Scroll), 45, 999, 0x1F2D, 0x8FD));
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
            }
        }
    }

    /// <summary>
    /// Illusion Magic Vendor - Sells reagents, scrolls, and spellbooks
    /// </summary>
    public class IllusionistVendor : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public IllusionistVendor() : base("the illusionist vendor")
        {
            Hue = 0x47E;
            SetSkill(SkillName.Magery, 100.0);
            SetSkill(SkillName.EvalInt, 100.0);
            SetSkill(SkillName.Inscribe, 100.0);
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBIllusionist());
        }

        public IllusionistVendor(Serial serial) : base(serial) { }

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

    public class SBIllusionist : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new List<GenericBuyInfo>();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBIllusionist()
        {
            // Spellbook (empty)
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spellbook", typeof(IllusionistSpellbook), 150, 20, 0x1F2D, 0x47E));

            // Reagents (8 total)
            m_BuyInfo.Add(new GenericBuyInfo("Mirror Dust", typeof(MirrorDust), 5, 999, 0x26b8, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Phantom Silk", typeof(PhantomSilk), 5, 999, 0x1aa4, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Mirage Essence", typeof(MirageEssence), 5, 999, 0x1f9d, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Dream Crystal", typeof(DreamCrystal), 5, 999, 0x1f19, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Reality Splinter", typeof(RealitySplinter), 5, 999, 0x1f1c, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Void Mirror", typeof(VoidMirror), 5, 999, 0x1f47, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Chaos Prism", typeof(ChaosPrism), 5, 999, 0x1f19, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Phantom Petal", typeof(PhantomPetal), 5, 999, 0x18e9, 0x47E));

            // Spell Scrolls (32 total)
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell1_1 Scroll", typeof(IllusionistSpell1_1Scroll), 10, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell1_2 Scroll", typeof(IllusionistSpell1_2Scroll), 10, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell1_3 Scroll", typeof(IllusionistSpell1_3Scroll), 10, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell1_4 Scroll", typeof(IllusionistSpell1_4Scroll), 10, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell2_1 Scroll", typeof(IllusionistSpell2_1Scroll), 15, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell2_2 Scroll", typeof(IllusionistSpell2_2Scroll), 15, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell2_3 Scroll", typeof(IllusionistSpell2_3Scroll), 15, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell2_4 Scroll", typeof(IllusionistSpell2_4Scroll), 15, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell3_1 Scroll", typeof(IllusionistSpell3_1Scroll), 20, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell3_2 Scroll", typeof(IllusionistSpell3_2Scroll), 20, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell3_3 Scroll", typeof(IllusionistSpell3_3Scroll), 20, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell3_4 Scroll", typeof(IllusionistSpell3_4Scroll), 20, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell4_1 Scroll", typeof(IllusionistSpell4_1Scroll), 25, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell4_2 Scroll", typeof(IllusionistSpell4_2Scroll), 25, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell4_3 Scroll", typeof(IllusionistSpell4_3Scroll), 25, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell4_4 Scroll", typeof(IllusionistSpell4_4Scroll), 25, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell5_1 Scroll", typeof(IllusionistSpell5_1Scroll), 30, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell5_2 Scroll", typeof(IllusionistSpell5_2Scroll), 30, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell5_3 Scroll", typeof(IllusionistSpell5_3Scroll), 30, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell5_4 Scroll", typeof(IllusionistSpell5_4Scroll), 30, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell6_1 Scroll", typeof(IllusionistSpell6_1Scroll), 35, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell6_2 Scroll", typeof(IllusionistSpell6_2Scroll), 35, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell6_3 Scroll", typeof(IllusionistSpell6_3Scroll), 35, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell6_4 Scroll", typeof(IllusionistSpell6_4Scroll), 35, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell7_1 Scroll", typeof(IllusionistSpell7_1Scroll), 40, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell7_2 Scroll", typeof(IllusionistSpell7_2Scroll), 40, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell7_3 Scroll", typeof(IllusionistSpell7_3Scroll), 40, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell7_4 Scroll", typeof(IllusionistSpell7_4Scroll), 40, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell8_1 Scroll", typeof(IllusionistSpell8_1Scroll), 45, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell8_2 Scroll", typeof(IllusionistSpell8_2Scroll), 45, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell8_3 Scroll", typeof(IllusionistSpell8_3Scroll), 45, 999, 0x1F2D, 0x47E));
            m_BuyInfo.Add(new GenericBuyInfo("Illusion Magic Spell8_4 Scroll", typeof(IllusionistSpell8_4Scroll), 45, 999, 0x1F2D, 0x47E));
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
            }
        }
    }

}
