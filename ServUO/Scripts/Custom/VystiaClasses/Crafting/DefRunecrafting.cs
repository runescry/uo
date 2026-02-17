using System;
using Server.Items;
using Server.Items.Vystia;

namespace Server.Engines.Craft
{
    /// <summary>
    /// Runecrafting Crafting System - Uses SkillName.Runeweaving (ID 68)
    /// For Enchanter class - uses runes and magical components
    /// </summary>
    public class DefRunecrafting : CraftSystem
    {
        public override SkillName MainSkill => SkillName.Runeweaving;

        public override string GumpTitleString => "<CENTER>RUNECRAFTING MENU</CENTER>";

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefRunecrafting();

                return m_CraftSystem;
            }
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.0; // 0% at minimum skill
        }

        private DefRunecrafting()
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
            from.PlaySound(0x1F5); // Magic sound
        }

        public override int PlayEndingEffect(Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item)
        {
            if (toolBroken)
                from.SendLocalizedMessage(1044038); // You have worn out your tool

            if (failed)
            {
                from.PlaySound(0x1F2); // Fizzle sound
                return 1044043; // You failed to create the item, and some of your materials are lost.
            }
            else
            {
                from.PlaySound(0x1F5); // Magic sound
                return 1044154; // You create the item.
            }
        }

        public override void InitCraftList()
        {
            int index = -1;

            // ============================================
            // TOOLS (Skill 25-75)
            // ============================================

            // Runecrafting Kit - Skill 25-75
            index = AddCraft(typeof(RunecraftingKit), "Tools", "Runecrafting Kit", 25.0, 75.0, typeof(ArcaneDust), "Arcane Dust", 5, "You need arcane dust.");
            AddRes(index, typeof(EssenceOfMagic), "Essence of Magic", 3, "You need essence of magic.");

            // ============================================
            // BASIC RUNES (Skill 0-50)
            // ============================================

            // Rune of Strength - Skill 0-50
            index = AddCraft(typeof(RuneOfStrength), "Basic Runes", "Rune of Strength", 0.0, 50.0, typeof(ArcaneDust), "Arcane Dust", 3, "You need arcane dust.");

            // Rune of Dexterity - Skill 5-55
            index = AddCraft(typeof(RuneOfDexterity), "Basic Runes", "Rune of Dexterity", 5.0, 55.0, typeof(ArcaneDust), "Arcane Dust", 3, "You need arcane dust.");

            // Rune of Intelligence - Skill 10-60
            index = AddCraft(typeof(RuneOfIntelligence), "Basic Runes", "Rune of Intelligence", 10.0, 60.0, typeof(ArcaneDust), "Arcane Dust", 3, "You need arcane dust.");
            AddRes(index, typeof(EssenceOfMagic), "Essence of Magic", 1, "You need essence of magic.");

            // ============================================
            // INTERMEDIATE RUNES (Skill 40-80)
            // ============================================

            // Rune of Fortitude - Skill 40-90
            index = AddCraft(typeof(RuneOfFortitude), "Intermediate Runes", "Rune of Fortitude", 40.0, 90.0, typeof(EssenceOfMagic), "Essence of Magic", 3, "You need essence of magic.");
            AddRes(index, typeof(ManaCrystal), "Mana Crystal", 2, "You need mana crystals.");

            // Rune of Swiftness - Skill 45-95
            index = AddCraft(typeof(RuneOfSwiftness), "Intermediate Runes", "Rune of Swiftness", 45.0, 95.0, typeof(EssenceOfMagic), "Essence of Magic", 3, "You need essence of magic.");
            AddRes(index, typeof(ManaCrystal), "Mana Crystal", 2, "You need mana crystals.");

            // Rune of Warding - Skill 50-100
            index = AddCraft(typeof(RuneOfWarding), "Intermediate Runes", "Rune of Warding", 50.0, 100.0, typeof(ManaCrystal), "Mana Crystal", 3, "You need mana crystals.");
            AddRes(index, typeof(LeyLineEssence), "Ley Line Essence", 2, "You need ley line essence.");

            // ============================================
            // ADVANCED RUNES (Skill 60-100)
            // ============================================

            // Rune of Power - Skill 60-110
            index = AddCraft(typeof(RuneOfPower), "Advanced Runes", "Rune of Power", 60.0, 110.0, typeof(ManaCrystal), "Mana Crystal", 4, "You need mana crystals.");
            AddRes(index, typeof(LeyLineEssence), "Ley Line Essence", 3, "You need ley line essence.");
            AddRes(index, typeof(LeyLineShard), "Ley Line Shard", 1, "You need a ley line shard.");

            // Rune of Regeneration - Skill 65-115
            index = AddCraft(typeof(RuneOfRegeneration), "Advanced Runes", "Rune of Regeneration", 65.0, 115.0, typeof(LeyLineEssence), "Ley Line Essence", 4, "You need ley line essence.");
            AddRes(index, typeof(LeyLineShard), "Ley Line Shard", 2, "You need ley line shards.");

            // Rune of Focus - Skill 70-120
            index = AddCraft(typeof(RuneOfFocus), "Advanced Runes", "Rune of Focus", 70.0, 120.0, typeof(LeyLineEssence), "Ley Line Essence", 4, "You need ley line essence.");
            AddRes(index, typeof(LeyLineShard), "Ley Line Shard", 2, "You need ley line shards.");
            AddRes(index, typeof(RuneFragment), "Rune Fragment", 1, "You need a rune fragment.");

            // ============================================
            // GREATER RUNES (Skill 80-120)
            // ============================================

            // Greater Rune of Might - Skill 80-130
            index = AddCraft(typeof(GreaterRuneOfMight), "Greater Runes", "Greater Rune of Might", 80.0, 130.0, typeof(LeyLineShard), "Ley Line Shard", 4, "You need ley line shards.");
            AddRes(index, typeof(RuneFragment), "Rune Fragment", 3, "You need rune fragments.");
            AddRes(index, typeof(RunicPowder), "Runic Powder", 2, "You need runic powder.");

            // Greater Rune of Grace - Skill 80-130
            index = AddCraft(typeof(GreaterRuneOfGrace), "Greater Runes", "Greater Rune of Grace", 80.0, 130.0, typeof(LeyLineShard), "Ley Line Shard", 4, "You need ley line shards.");
            AddRes(index, typeof(RuneFragment), "Rune Fragment", 3, "You need rune fragments.");
            AddRes(index, typeof(RunicPowder), "Runic Powder", 2, "You need runic powder.");

            // Greater Rune of Wisdom - Skill 82-132
            index = AddCraft(typeof(GreaterRuneOfWisdom), "Greater Runes", "Greater Rune of Wisdom", 82.0, 132.0, typeof(LeyLineShard), "Ley Line Shard", 4, "You need ley line shards.");
            AddRes(index, typeof(RuneFragment), "Rune Fragment", 3, "You need rune fragments.");
            AddRes(index, typeof(RunicPowder), "Runic Powder", 2, "You need runic powder.");

            // Greater Rune of Protection - Skill 85-135
            index = AddCraft(typeof(GreaterRuneOfProtection), "Greater Runes", "Greater Rune of Protection", 85.0, 135.0, typeof(RuneFragment), "Rune Fragment", 5, "You need rune fragments.");
            AddRes(index, typeof(RunicPowder), "Runic Powder", 4, "You need runic powder.");

            // ============================================
            // LEGENDARY RUNES (Skill 95-145)
            // ============================================

            // Titan Rune of Ascendancy - Skill 95-145
            index = AddCraft(typeof(TitanRuneOfAscendancy), "Legendary Runes", "Titan Rune of Ascendancy", 95.0, 145.0, typeof(RunicPowder), "Runic Powder", 5, "You need runic powder.");
            AddRes(index, typeof(TitanRune), "Titan Rune", 3, "You need titan runes.");
            AddRes(index, typeof(RuneFragment), "Rune Fragment", 5, "You need rune fragments.");

            // ============================================
            // TALISMANS (Skill 50-100)
            // ============================================

            // Protection Talisman - Skill 50-100
            index = AddCraft(typeof(ProtectionTalisman), "Talismans", "Protection Talisman", 50.0, 100.0, typeof(ManaCrystal), "Mana Crystal", 5, "You need mana crystals.");
            AddRes(index, typeof(LeyLineEssence), "Ley Line Essence", 3, "You need ley line essence.");

            // Arcane Talisman - Skill 60-110
            index = AddCraft(typeof(ArcaneTalisman), "Talismans", "Arcane Talisman", 60.0, 110.0, typeof(LeyLineEssence), "Ley Line Essence", 5, "You need ley line essence.");
            AddRes(index, typeof(LeyLineShard), "Ley Line Shard", 3, "You need ley line shards.");

            // Warrior's Talisman - Skill 65-115
            index = AddCraft(typeof(WarriorsTalisman), "Talismans", "Warrior's Talisman", 65.0, 115.0, typeof(LeyLineEssence), "Ley Line Essence", 5, "You need ley line essence.");
            AddRes(index, typeof(RuneFragment), "Rune Fragment", 3, "You need rune fragments.");

            // Runic Talisman - Skill 75-125
            index = AddCraft(typeof(RunicTalisman), "Talismans", "Runic Talisman", 75.0, 125.0, typeof(RuneFragment), "Rune Fragment", 5, "You need rune fragments.");
            AddRes(index, typeof(RunicPowder), "Runic Powder", 3, "You need runic powder.");

            Console.WriteLine("[Vystia] Runecrafting system initialized with 18 recipes.");
        }
    }
}
