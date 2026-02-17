using System;
using Server;
using Server.Items;
using Server.Items.VystiaClassItems;

namespace Server.Custom.VystiaClasses
{
    /// <summary>
    /// Alchemist - Potion master and chemical warfare specialist
    /// Region: Verdantpeak
    /// Role: Support
    /// </summary>
    public class AlchemistClass : PlayerClass
    {
        public override PlayerClassType ClassType => PlayerClassType.Alchemist;
        public override string ClassName => "Alchemist";
        public override string ClassDescription => "Potion master and chemical warfare specialist";

        // Stats (Total: 80) - Support (Balanced)
        public override int StartStr => 20;  // Low-medium strength
        public override int StartDex => 30;  // Medium dexterity (potion throwing)
        public override int StartInt => 30;  // Medium-high intelligence

        // Stat Caps
        public override int StrCap => 105;
        public override int DexCap => 120;
        public override int IntCap => 115;

        // Primary Skills (7 skills)
        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Transmutation, // Primary class skill - potion power
            SkillName.Alchemy, SkillName.TasteID, SkillName.Magery, SkillName.Poisoning, SkillName.Cooking, SkillName.Meditation
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, // Transmutation
            60.0, 50.0, 35.0, 40.0, 35.0, 20.0
        };

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Leather Armor
            m.AddItem(new LeatherChest() { Name = "Alchemist's Jerkin", Hue = 2010 });
        m.AddItem(new LeatherLegs() { Hue = 2010 });
        m.AddItem(new LeatherArms() { Hue = 2010 });
        m.AddItem(new LeatherGloves() { Hue = 2010 });
        m.AddItem(new LeatherGorget() { Hue = 2010 });
        m.AddItem(new LeatherCap() { Hue = 2010 });

            // Weapon
            m.AddItem(new Dagger() { Name = "Alchemist's Dagger", Hue = 2010 });

            // Starting Resources
            m.Backpack.DropItem(new LivingBark(20));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Special Class Item
            AlchemistKit item = new AlchemistKit();
            m.Backpack.DropItem(item);
        }
    }
}
