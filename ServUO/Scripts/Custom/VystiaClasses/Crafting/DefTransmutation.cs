using System;
using Server.Items;

namespace Server.Engines.Craft
{
    /// <summary>
    /// Transmutation Crafting System - Uses SkillName.Transmutation (ID 81)
    /// For Alchemist class - uses Nature and Hex reagents from Verdantpeak/Shadowfen
    /// </summary>
    public class DefTransmutation : CraftSystem
    {
        public override SkillName MainSkill => SkillName.Transmutation;

        public override string GumpTitleString => "<CENTER>TRANSMUTATION MENU</CENTER>";

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefTransmutation();

                return m_CraftSystem;
            }
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.0; // 0% at minimum skill
        }

        private DefTransmutation()
            : base(1, 1, 1.25) // min/max craft effect, delay multiplier
        {
        }

        public override int CanCraft(Mobile from, ITool tool, Type itemType)
        {
            if (tool == null || tool.Deleted || tool.UsesRemaining <= 0)
                return 1044038; // You have worn out your tool!

            int num = 0;
            if (!tool.CheckAccessible(from, ref num))
                return num; // The tool must be on your person to use.

            return 0;
        }

        public override void PlayCraftEffect(Mobile from)
        {
            from.PlaySound(0x242); // Bubbling sound
        }

        public override int PlayEndingEffect(Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item)
        {
            if (toolBroken)
                from.SendLocalizedMessage(1044038); // You have worn out your tool

            if (failed)
            {
                from.AddToBackpack(new Bottle());
                return 500287; // You fail to create a useful potion.
            }
            else
            {
                from.PlaySound(0x240); // Filling bottle sound
                return 1044154; // You create the item.
            }
        }

        public override void InitCraftList()
        {
            int index = -1;

            // ============================================
            // HEALING POTIONS (Nature reagents)
            // ============================================

            // Lesser Nature's Healing - Skill 0-50
            index = AddCraft(typeof(LesserHealPotion), "Healing Potions", "Lesser Nature's Healing", -25.0, 25.0, typeof(WildMoss), "Wild Moss", 1, "You need wild moss to make this potion.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // Nature's Healing - Skill 15-65
            index = AddCraft(typeof(HealPotion), "Healing Potions", "Nature's Healing", 15.0, 65.0, typeof(WildMoss), "Wild Moss", 2, "You need wild moss to make this potion.");
            AddRes(index, typeof(Moonpetal), "Moonpetal", 1, "You need moonpetal.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // Greater Nature's Healing - Skill 55-105
            index = AddCraft(typeof(GreaterHealPotion), "Healing Potions", "Greater Nature's Healing", 55.0, 105.0, typeof(DruidBark), "Druid Bark", 2, "You need druid bark to make this potion.");
            AddRes(index, typeof(TreantSap), "Treant Sap", 2, "You need treant sap.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // ============================================
            // CURE POTIONS (Hex reagents - antidotes)
            // ============================================

            // Lesser Antivenom - Skill 0-40
            index = AddCraft(typeof(LesserCurePotion), "Cure Potions", "Lesser Antivenom", -10.0, 40.0, typeof(BogMoss), "Bog Moss", 1, "You need bog moss to make this potion.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // Antivenom - Skill 25-75
            index = AddCraft(typeof(CurePotion), "Cure Potions", "Antivenom", 25.0, 75.0, typeof(ViperFang), "Viper Fang", 2, "You need viper fang to make this potion.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // Greater Antivenom - Skill 65-115
            index = AddCraft(typeof(GreaterCurePotion), "Cure Potions", "Greater Antivenom", 65.0, 115.0, typeof(SwampLotus), "Swamp Lotus", 2, "You need swamp lotus to make this potion.");
            AddRes(index, typeof(ViperFang), "Viper Fang", 2, "You need viper fang.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // ============================================
            // REFRESH POTIONS (Nature reagents)
            // ============================================

            // Stamina Tonic - Skill 0-25
            index = AddCraft(typeof(RefreshPotion), "Refresh Potions", "Stamina Tonic", -25.0, 25.0, typeof(Moonpetal), "Moonpetal", 1, "You need moonpetal to make this potion.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // Greater Stamina Tonic - Skill 25-75
            index = AddCraft(typeof(TotalRefreshPotion), "Refresh Potions", "Greater Stamina Tonic", 25.0, 75.0, typeof(PrimalVine), "Primal Vine", 2, "You need primal vine to make this potion.");
            AddRes(index, typeof(Moonpetal), "Moonpetal", 2, "You need moonpetal.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // ============================================
            // ENHANCEMENT POTIONS (Mixed reagents)
            // ============================================

            // Agility Elixir - Skill 15-65
            index = AddCraft(typeof(AgilityPotion), "Enhancement Potions", "Agility Elixir", 15.0, 65.0, typeof(WildMoss), "Wild Moss", 1, "You need wild moss.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // Greater Agility Elixir - Skill 35-85
            index = AddCraft(typeof(GreaterAgilityPotion), "Enhancement Potions", "Greater Agility Elixir", 35.0, 85.0, typeof(ElderwoodSeed), "Elderwood Seed", 2, "You need elderwood seeds.");
            AddRes(index, typeof(WildMoss), "Wild Moss", 2, "You need wild moss.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // Strength Elixir - Skill 25-75
            index = AddCraft(typeof(StrengthPotion), "Enhancement Potions", "Strength Elixir", 25.0, 75.0, typeof(DruidBark), "Druid Bark", 2, "You need druid bark.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // Greater Strength Elixir - Skill 45-95
            index = AddCraft(typeof(GreaterStrengthPotion), "Enhancement Potions", "Greater Strength Elixir", 45.0, 95.0, typeof(LivingBark), "Living Bark", 2, "You need living bark.");
            AddRes(index, typeof(TreantSap), "Treant Sap", 2, "You need treant sap.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // Night Vision Potion - Skill 0-25
            index = AddCraft(typeof(NightSightPotion), "Enhancement Potions", "Night Vision Potion", -25.0, 25.0, typeof(ToadsEye), "Toad's Eye", 1, "You need toad's eye.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // ============================================
            // POISON POTIONS (Hex reagents)
            // ============================================

            // Lesser Venom - Skill 0-45
            index = AddCraft(typeof(LesserPoisonPotion), "Poison Potions", "Lesser Venom", -5.0, 45.0, typeof(BogMoss), "Bog Moss", 1, "You need bog moss.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // Venom - Skill 15-65
            index = AddCraft(typeof(PoisonPotion), "Poison Potions", "Venom", 15.0, 65.0, typeof(Witchweed), "Witchweed", 1, "You need witchweed.");
            AddRes(index, typeof(ViperFang), "Viper Fang", 1, "You need viper fang.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // Greater Venom - Skill 55-105
            index = AddCraft(typeof(GreaterPoisonPotion), "Poison Potions", "Greater Venom", 55.0, 105.0, typeof(HagsHair), "Hag's Hair", 2, "You need hag's hair.");
            AddRes(index, typeof(SwampLotus), "Swamp Lotus", 2, "You need swamp lotus.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // Deadly Venom - Skill 90-140
            index = AddCraft(typeof(DeadlyPoisonPotion), "Poison Potions", "Deadly Venom", 90.0, 140.0, typeof(CursedSalt), "Cursed Salt", 3, "You need cursed salt.");
            AddRes(index, typeof(CursedPearl), "Cursed Pearl", 2, "You need cursed pearl.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // ============================================
            // EXPLOSIVE POTIONS (Mixed reagents)
            // ============================================

            // Lesser Explosive Flask - Skill 5-55
            index = AddCraft(typeof(VystiaLesserTransmutationExplosive), "Explosive Potions", "Lesser Explosive Flask", 5.0, 55.0, typeof(WildMoss), "Wild Moss", 2, "You need wild moss.");
            AddRes(index, typeof(BogMoss), "Bog Moss", 1, "You need bog moss.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // Explosive Flask - Skill 35-85
            index = AddCraft(typeof(VystiaTransmutationExplosive), "Explosive Potions", "Explosive Flask", 35.0, 85.0, typeof(DruidBark), "Druid Bark", 2, "You need druid bark.");
            AddRes(index, typeof(Witchweed), "Witchweed", 2, "You need witchweed.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // Greater Explosive Flask - Skill 65-115
            index = AddCraft(typeof(VystiaGreaterTransmutationExplosive), "Explosive Potions", "Greater Explosive Flask", 65.0, 115.0, typeof(LivingBark), "Living Bark", 3, "You need living bark.");
            AddRes(index, typeof(HagsHair), "Hag's Hair", 3, "You need hag's hair.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // ============================================
            // SMOKE BOMBS (High skill)
            // ============================================

            // Smoke Bomb - Skill 90-100 (Vystia: 100 is GM)
            index = AddCraft(typeof(SmokeBomb), "Special Items", "Smoke Bomb", 90.0, 100.0, typeof(AncientRoot), "Ancient Root", 1, "You need ancient root.");
            AddRes(index, typeof(CursedSalt), "Cursed Salt", 1, "You need cursed salt.");

            // ============================================
            // RESOURCE ENHANCEMENT POTIONS (High skill)
            // ============================================

            // Fury Draught (Barbarian) - Skill 60-110
            index = AddCraft(typeof(Server.Items.Vystia.FuryDraught), "Resource Potions", "Fury Draught", 60.0, 110.0, typeof(DruidBark), "Druid Bark", 3, "You need druid bark.");
            AddRes(index, typeof(WildMoss), "Wild Moss", 2, "You need wild moss.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // Berserker's Blood (Barbarian) - Skill 70-120
            index = AddCraft(typeof(Server.Items.Vystia.BerserkersBlood), "Resource Potions", "Berserker's Blood", 70.0, 120.0, typeof(LivingBark), "Living Bark", 2, "You need living bark.");
            AddRes(index, typeof(HagsHair), "Hag's Hair", 2, "You need hag's hair.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // Chi Elixir (Monk) - Skill 65-115
            index = AddCraft(typeof(Server.Items.Vystia.ChiElixir), "Resource Potions", "Chi Elixir", 65.0, 115.0, typeof(TreantSap), "Treant Sap", 3, "You need treant sap.");
            AddRes(index, typeof(Moonpetal), "Moonpetal", 2, "You need moonpetal.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // Focused Serum (Ranger) - Skill 55-105
            index = AddCraft(typeof(Server.Items.Vystia.FocusedSerum), "Resource Potions", "Focused Serum", 55.0, 105.0, typeof(ElderwoodSeed), "Elderwood Seed", 2, "You need elderwood seeds.");
            AddRes(index, typeof(PrimalVine), "Primal Vine", 2, "You need primal vine.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // Zealot's Tonic (Templar) - Skill 60-110
            index = AddCraft(typeof(Server.Items.Vystia.ZealotsTonic), "Resource Potions", "Zealot's Tonic", 60.0, 110.0, typeof(DruidBark), "Druid Bark", 2, "You need druid bark.");
            AddRes(index, typeof(CursedSalt), "Cursed Salt", 1, "You need cursed salt.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // Knight's Fortifier (Knight) - Skill 65-115
            index = AddCraft(typeof(Server.Items.Vystia.KnightsFortifier), "Resource Potions", "Knight's Fortifier", 65.0, 115.0, typeof(LivingBark), "Living Bark", 3, "You need living bark.");
            AddRes(index, typeof(AncientRoot), "Ancient Root", 1, "You need ancient root.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // Hunter's Mark Oil (Bounty Hunter) - Skill 70-120
            index = AddCraft(typeof(Server.Items.Vystia.HuntersMarkOil), "Resource Potions", "Hunter's Mark Oil", 70.0, 120.0, typeof(BogMoss), "Bog Moss", 3, "You need bog moss.");
            AddRes(index, typeof(Witchweed), "Witchweed", 2, "You need witchweed.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // Shard Catalyst (Warlock) - Skill 75-125
            index = AddCraft(typeof(Server.Items.Vystia.ShardCatalyst), "Resource Potions", "Shard Catalyst", 75.0, 125.0, typeof(CursedPearl), "Cursed Pearl", 2, "You need cursed pearl.");
            AddRes(index, typeof(HagsHair), "Hag's Hair", 2, "You need hag's hair.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // Chill Enhancer (Ice Mage) - Skill 60-110
            index = AddCraft(typeof(Server.Items.Vystia.ChillEnhancer), "Resource Potions", "Chill Enhancer", 60.0, 110.0, typeof(WildMoss), "Wild Moss", 3, "You need wild moss.");
            AddRes(index, typeof(Moonpetal), "Moonpetal", 2, "You need moonpetal.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // Crescendo Catalyst (Bard) - Skill 65-115
            index = AddCraft(typeof(Server.Items.Vystia.CrescendoCatalyst), "Resource Potions", "Crescendo Catalyst", 65.0, 115.0, typeof(ElderwoodSeed), "Elderwood Seed", 3, "You need elderwood seeds.");
            AddRes(index, typeof(TreantSap), "Treant Sap", 2, "You need treant sap.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // Virtue Essence (Paladin) - Skill 80-130
            index = AddCraft(typeof(Server.Items.Vystia.VirtueEssence), "Resource Potions", "Virtue Essence", 80.0, 130.0, typeof(AncientRoot), "Ancient Root", 2, "You need ancient root.");
            AddRes(index, typeof(LivingBark), "Living Bark", 2, "You need living bark.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // ============================================
            // RESOURCE STORAGE FLASKS (Very high skill)
            // ============================================

            // LifeForce Flask (Necromancer) - Skill 85-135
            index = AddCraft(typeof(Server.Items.Vystia.LifeForceFlask), "Resource Flasks", "LifeForce Flask", 85.0, 135.0, typeof(CursedPearl), "Cursed Pearl", 3, "You need cursed pearl.");
            AddRes(index, typeof(CursedSalt), "Cursed Salt", 3, "You need cursed salt.");
            AddRes(index, typeof(Bottle), "Bottle", 3, "You need bottles.");

            // Faith Vessel (Cleric) - Skill 85-135
            index = AddCraft(typeof(Server.Items.Vystia.FaithVessel), "Resource Flasks", "Faith Vessel", 85.0, 135.0, typeof(AncientRoot), "Ancient Root", 3, "You need ancient root.");
            AddRes(index, typeof(TreantSap), "Treant Sap", 3, "You need treant sap.");
            AddRes(index, typeof(Bottle), "Bottle", 3, "You need bottles.");

            // Steam Concentrate (Artificer) - Skill 85-135
            index = AddCraft(typeof(Server.Items.Vystia.SteamConcentrate), "Resource Flasks", "Steam Concentrate", 85.0, 135.0, typeof(LivingBark), "Living Bark", 4, "You need living bark.");
            AddRes(index, typeof(DruidBark), "Druid Bark", 4, "You need druid bark.");
            AddRes(index, typeof(Bottle), "Bottle", 3, "You need bottles.");

            // ============================================
            // TRANSMUTATION TOOL
            // ============================================

            // Transmutation Kit - Skill 25-75
            index = AddCraft(typeof(Server.Items.Vystia.TransmutationKit), "Tools", "Transmutation Kit", 25.0, 75.0, typeof(WildMoss), "Wild Moss", 5, "You need wild moss.");
            AddRes(index, typeof(BogMoss), "Bog Moss", 3, "You need bog moss.");

            Console.WriteLine("[Vystia] Transmutation system initialized with 33 recipes.");
        }
    }
}
