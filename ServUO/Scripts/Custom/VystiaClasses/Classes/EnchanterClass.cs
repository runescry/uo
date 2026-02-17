using System;
using Server;
using Server.Items;
using Server.Items.VystiaClassItems;

namespace Server.Custom.VystiaClasses
{
    /// <summary>
    /// Enchanter - Magical crafter who enhances items and allies
    /// Region: Multi-Regional
    /// Role: Utility/Buff
    /// </summary>
    public class EnchanterClass : PlayerClass
    {
        public override PlayerClassType ClassType => PlayerClassType.Enchanter;
        public override string ClassName => "Enchanter";
        public override string ClassDescription => "Magical crafter who enhances items and allies";

        // Stats (Total: 80) - Utility/Buff Caster
        public override int StartStr => 15;  // Low strength
        public override int StartDex => 25;  // Medium dexterity
        public override int StartInt => 40;  // High intelligence (enchanting)

        // Stat Caps
        public override int StrCap => 100;
        public override int DexCap => 115;
        public override int IntCap => 130;

        // Primary Skills (7 skills)
        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Runeweaving, // Primary class skill - enchantment power
            SkillName.Magery, SkillName.Inscribe, SkillName.ItemID, SkillName.Meditation, SkillName.EvalInt, SkillName.MagicResist
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, // Runeweaving
            50.0, 50.0, 45.0, 40.0, 35.0, 20.0
        };

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Cloth Armor
            m.AddItem(new Robe() { Name = "Enchanter's Robe", Hue = 1153 });
        m.AddItem(new Sandals() { Hue = 1153 });
        m.AddItem(new WizardsHat() { Hue = 1153 });

            // Weapon
            m.AddItem(new QuarterStaff() { Name = "Enchanter's QuarterStaff", Hue = 1153 });

            // Starting Resources
            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Special Class Item
            EnchantingCrystal item = new EnchantingCrystal();
        //         m.Backpack.DropItem(item);
        // Give class spellbook
        EnchanterSpellbook spellbook = new EnchanterSpellbook();
        m.Backpack.DropItem(spellbook);
        }
    }
}
