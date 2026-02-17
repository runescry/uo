using System;
using Server;
using Server.Items;
using Server.Spells;

namespace Server.Custom.VystiaClasses
{
    /// <summary>
    /// Ice Mage - Frostweavers of Winterguard
    /// Low STR, medium DEX, high INT
    /// Focus: Ice magic, AoE damage, crowd control
    /// </summary>
    public class IceMageClass : PlayerClass
    {
        public override PlayerClassType ClassType => PlayerClassType.IceMage;
        public override string ClassName => "Ice Mage";
        public override string ClassDescription => "Frostweavers of Winterguard - Masters of ice and cold magic";

        // Starting Stats (total: 80)
        public override int StartStr => 15;  // Low strength (caster)
        public override int StartDex => 20;  // Medium dexterity
        public override int StartInt => 45;  // High intelligence (ice magic)

        // Stat Caps - Ice Mages favor INT
        public override int StrCap => 90;    // Lower than normal
        public override int DexCap => 110;   // Normal
        public override int IntCap => 150;   // Higher than normal

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Cryomancy,   // Primary class skill - ice magic mastery
            SkillName.Magery,      // Core casting
            SkillName.EvalInt,     // Spell damage
            SkillName.Meditation,  // Mana regen
            SkillName.Wrestling,   // Defensive
            SkillName.MagicResist, // Resist magic
            SkillName.Inscribe     // Spell power bonus
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0,  // Cryomancy
            50.0,  // Magery
            50.0,  // Eval Int
            40.0,  // Meditation
            30.0,  // Wrestling
            40.0,  // Magic Resist
            30.0   // Inscription
        };

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Ice Mage robes
            Robe robe = new Robe();
            robe.Name = "Frostweaver's Robes";
            robe.Hue = 1150; // Ice blue
            m.AddItem(robe);

            // Wizard hat
            WizardsHat hat = new WizardsHat();
            hat.Name = "Winterguard Circlet";
            hat.Hue = 1150;
            m.AddItem(hat);

            // Sandals
            Sandals sandals = new Sandals();
            sandals.Hue = 1150;
            m.AddItem(sandals);

            // Starting weapon - staff
            QuarterStaff staff = new QuarterStaff();
            staff.Name = "Icicle Staff";
            staff.Hue = 1150;
            m.AddItem(staff);

            // Starting reagents
            m.Backpack.DropItem(new BagOfReagents(50));

            // Ice-specific reagents
            m.Backpack.DropItem(new FrozenOre(20)); // Frosthold ore for ice spells
            m.Backpack.DropItem(new FrostforgedIngot(10)); // Frosthold ingots for powerful ice spells

            // Starting gold
            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Give Ice Mage spellbook
            IceMageSpellbook spellbook = new IceMageSpellbook();
            m.Backpack.DropItem(spellbook);

            m.SendMessage(0x3B2, "You have been granted an Ice Mage Spellbook!");
        }
    }
}
