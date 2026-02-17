using System;
using Server;
using Server.Items;
using Server.Items.VystiaClassItems;

namespace Server.Custom.VystiaClasses
{
    /// <summary>
    /// Summoner - Conjurer who binds creatures from other planes
    /// Region: Underwater
    /// Role: Pet/Caster
    /// </summary>
    public class SummonerClass : PlayerClass
    {
        public override PlayerClassType ClassType => PlayerClassType.Summoner;
        public override string ClassName => "Summoner";
        public override string ClassDescription => "Conjurer who binds creatures from other planes";

        // Stats (Total: 80) - Pet/Caster
        public override int StartStr => 15;  // Low strength
        public override int StartDex => 20;  // Low dexterity
        public override int StartInt => 45;  // High intelligence (summoning)

        // Stat Caps
        public override int StrCap => 100;
        public override int DexCap => 110;
        public override int IntCap => 140;

        // Primary Skills (7 skills)
        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Conjuration, // Primary class skill - summon power
            SkillName.Magery, SkillName.EvalInt, SkillName.Meditation, SkillName.AnimalLore, SkillName.MagicResist, SkillName.Focus
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, // Conjuration
            50.0, 45.0, 45.0, 40.0, 40.0, 20.0
        };

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Cloth Armor
            m.AddItem(new Robe() { Name = "Summoner's Robe", Hue = 1365 });
        m.AddItem(new Sandals() { Hue = 1365 });
        m.AddItem(new WizardsHat() { Hue = 1365 });

            // Weapon
            m.AddItem(new QuarterStaff() { Name = "Summoner's QuarterStaff", Hue = 1365 });

            // Starting Resources
            m.Backpack.DropItem(new CrystalOre(20));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Special Class Item
            SummoningCircle item = new SummoningCircle();
        //         m.Backpack.DropItem(item);
        // Give class spellbook
        SummonerSpellbook spellbook = new SummonerSpellbook();
        m.Backpack.DropItem(spellbook);
        }
    }
}
