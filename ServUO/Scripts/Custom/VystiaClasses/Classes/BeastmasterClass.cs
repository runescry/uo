using System;
using Server;
using Server.Items;
using Server.Items.VystiaClassItems;

namespace Server.Custom.VystiaClasses
{
    /// <summary>
    /// Beastmaster - Master of beasts who commands animals and excels at archery
    /// Region: Frosthold
    /// Role: Pet/Ranged
    /// </summary>
    public class BeastmasterClass : PlayerClass
    {
        public override PlayerClassType ClassType => PlayerClassType.Beastmaster;
        public override string ClassName => "Beastmaster";
        public override string ClassDescription => "Master of beasts who commands animals and excels at archery";

        // Stats (Total: 80) - Pet/Ranged
        public override int StartStr => 25;  // Low-medium strength
        public override int StartDex => 40;  // High dexterity (archery)
        public override int StartInt => 15;  // Low intelligence

        // Stat Caps
        public override int StrCap => 110;
        public override int DexCap => 130;
        public override int IntCap => 100;

        // Primary Skills (7 skills)
        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.BeastBonding, // Primary class skill - pet power
            SkillName.AnimalTaming, SkillName.AnimalLore, SkillName.Veterinary, SkillName.Archery, SkillName.Tactics, SkillName.Tracking
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, // BeastBonding
            50.0, 50.0, 40.0, 35.0, 35.0, 30.0
        };

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Leather Armor
            m.AddItem(new LeatherChest() { Name = "Beastmaster's Jerkin", Hue = 1150 });
        m.AddItem(new LeatherLegs() { Hue = 1150 });
        m.AddItem(new LeatherArms() { Hue = 1150 });
        m.AddItem(new LeatherGloves() { Hue = 1150 });
        m.AddItem(new LeatherGorget() { Hue = 1150 });
        m.AddItem(new LeatherCap() { Hue = 1150 });

            // Weapon
            m.AddItem(new Bow() { Name = "Beastmaster's Bow", Hue = 1150 });

            // Starting Resources
            m.Backpack.DropItem(new FrozenOre(20));
        m.Backpack.DropItem(new Arrow(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Special Class Item
            BeastWhistle item = new BeastWhistle();
        m.Backpack.DropItem(item);
        }
    }
}
