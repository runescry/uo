using System;
using Server;
using Server.Items;
using Server.Items.VystiaClassItems;

namespace Server.Custom.VystiaClasses
{
    /// <summary>
    /// Illusionist - Master of illusions and mind control magic
    /// Region: Desert
    /// Role: Caster CC
    /// </summary>
    public class IllusionistClass : PlayerClass
    {
        public override PlayerClassType ClassType => PlayerClassType.Illusionist;
        public override string ClassName => "Illusionist";
        public override string ClassDescription => "Master of illusions and mind control magic";

        // Stats (Total: 80) - Caster CC
        public override int StartStr => 15;  // Low strength
        public override int StartDex => 23;  // Medium dexterity (stealth)
        public override int StartInt => 42;  // High intelligence (illusion magic)

        // Stat Caps
        public override int StrCap => 95;
        public override int DexCap => 115;
        public override int IntCap => 140;

        // Primary Skills (7 skills)
        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.IllusionMagic, // Primary class skill - illusion duration
            SkillName.Magery, SkillName.EvalInt, SkillName.Meditation, SkillName.Wrestling, SkillName.MagicResist, SkillName.Stealth
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, // IllusionMagic
            50.0, 50.0, 40.0, 30.0, 40.0, 30.0
        };

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Cloth Armor
            m.AddItem(new Robe() { Name = "Illusionist's Robe", Hue = 1719 });
        m.AddItem(new Sandals() { Hue = 1719 });
        m.AddItem(new WizardsHat() { Hue = 1719 });

            // Weapon
            m.AddItem(new QuarterStaff() { Name = "Illusionist's QuarterStaff", Hue = 1719 });

            // Starting Resources
            m.Backpack.DropItem(new SandstoneOre(20));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Special Class Item
                    // No special class item
        // Give class spellbook
        IllusionistSpellbook spellbook = new IllusionistSpellbook();
        m.Backpack.DropItem(spellbook);
        }
    }
}
