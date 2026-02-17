using System;
using Server;
using Server.Items;
using Server.Items.VystiaClassItems;

namespace Server.Custom.VystiaClasses
{
    /// <summary>
    /// Warlock - Dark magic wielder who channels demonic power
    /// Region: ShadowVoid
    /// Role: Caster DPS
    /// </summary>
    public class WarlockClass : PlayerClass
    {
        public override PlayerClassType ClassType => PlayerClassType.Warlock;
        public override string ClassName => "Warlock";
        public override string ClassDescription => "Dark magic wielder who channels demonic power";

        // Stats (Total: 80) - Caster DPS
        public override int StartStr => 18;  // Low strength
        public override int StartDex => 17;  // Low dexterity
        public override int StartInt => 45;  // High intelligence (dark magic)

        // Stat Caps
        public override int StrCap => 100;
        public override int DexCap => 110;
        public override int IntCap => 140;

        // Primary Skills (7 skills)
        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Demonology, // Primary class skill - dark pact magic
            SkillName.Magery, SkillName.EvalInt, SkillName.Meditation, SkillName.SpiritSpeak, SkillName.MagicResist, SkillName.Necromancy
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, // Demonology
            50.0, 50.0, 40.0, 35.0, 40.0, 35.0
        };

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Cloth Armor
            m.AddItem(new Robe() { Name = "Warlock's Robe", Hue = 1109 });
        m.AddItem(new Sandals() { Hue = 1109 });
        m.AddItem(new WizardsHat() { Hue = 1109 });

            // Weapon
            m.AddItem(new GnarledStaff() { Name = "Warlock's GnarledStaff", Hue = 1109 });

            // Starting Resources
            m.Backpack.DropItem(new ObsidianOre(20));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Special Class Item
                    // No special class item
        // Give class spellbook
        WarlockSpellbook spellbook = new WarlockSpellbook();
        m.Backpack.DropItem(spellbook);
        }
    }
}
