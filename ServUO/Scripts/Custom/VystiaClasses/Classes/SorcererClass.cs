using System;
using Server;
using Server.Items;
using Server.Items.VystiaClassItems;

namespace Server.Custom.VystiaClasses
{
    /// <summary>
    /// Sorcerer - Master of elemental fire magic with devastating AoE damage
    /// Region: Emberlands
    /// Role: Caster DPS
    /// </summary>
    public class SorcererClass : PlayerClass
    {
        public override PlayerClassType ClassType => PlayerClassType.Sorcerer;
        public override string ClassName => "Sorcerer";
        public override string ClassDescription => "Master of elemental fire magic with devastating AoE damage";

        // Stats (Total: 80) - Caster DPS
        public override int StartStr => 15;  // Low strength
        public override int StartDex => 20;  // Low dexterity
        public override int StartInt => 45;  // High intelligence (fire magic)

        // Stat Caps
        public override int StrCap => 100;
        public override int DexCap => 110;
        public override int IntCap => 140;

        // Primary Skills (7 skills)
        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Elementalism, // Primary class skill - elemental attunement
            SkillName.Magery, SkillName.EvalInt, SkillName.Meditation, SkillName.Wrestling, SkillName.MagicResist, SkillName.Tactics
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, // Elementalism
            50.0, 50.0, 40.0, 30.0, 40.0, 30.0
        };

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Cloth Armor
            m.AddItem(new Robe() { Name = "Sorcerer's Robe", Hue = 1358 });
        m.AddItem(new Sandals() { Hue = 1358 });
        m.AddItem(new WizardsHat() { Hue = 1358 });

            // Weapon
            m.AddItem(new QuarterStaff() { Name = "Sorcerer's QuarterStaff", Hue = 1358 });

            // Starting Resources
            m.Backpack.DropItem(new MoltenOre(20));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Special Class Item
                    // No special class item
        // Give class spellbook
        SorcererSpellbook spellbook = new SorcererSpellbook();
        m.Backpack.DropItem(spellbook);
        }
    }
}
