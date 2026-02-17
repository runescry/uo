using System;
using Server;
using Server.Items;
using Server.Items.VystiaClassItems;

namespace Server.Custom.VystiaClasses
{
    /// <summary>
    /// Oracle - Diviner who manipulates fate and foresees the future
    /// Region: Crystal Barrens
    /// Role: Utility
    /// </summary>
    public class OracleClass : PlayerClass
    {
        public override PlayerClassType ClassType => PlayerClassType.Oracle;
        public override string ClassName => "Oracle";
        public override string ClassDescription => "Diviner who manipulates fate and foresees the future";

        // Stats (Total: 80) - Utility Caster
        public override int StartStr => 15;  // Low strength
        public override int StartDex => 22;  // Low-medium dexterity
        public override int StartInt => 43;  // High intelligence (divination)

        // Stat Caps
        public override int StrCap => 95;
        public override int DexCap => 110;
        public override int IntCap => 145;

        // Primary Skills (7 skills)
        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Divination, // Primary class skill - foresight, time manipulation
            SkillName.Magery, SkillName.EvalInt, SkillName.Meditation, SkillName.ItemID, SkillName.MagicResist, SkillName.DetectHidden
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, // Divination
            55.0, 50.0, 45.0, 35.0, 40.0, 15.0
        };

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Cloth Armor
            m.AddItem(new Robe() { Name = "Oracle's Robe", Hue = 1154 });
        m.AddItem(new Sandals() { Hue = 1154 });
        m.AddItem(new WizardsHat() { Hue = 1154 });

            // Weapon
            m.AddItem(new QuarterStaff() { Name = "Oracle's QuarterStaff", Hue = 1154 });

            // Starting Resources
            m.Backpack.DropItem(new CrystalOre(20));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Special Class Item
            CrystalOrb item = new CrystalOrb();
        //         m.Backpack.DropItem(item);
        // Give class spellbook
        OracleSpellbook spellbook = new OracleSpellbook();
        m.Backpack.DropItem(spellbook);
        }
    }
}
