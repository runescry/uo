using System;
using Server;
using Server.Items;

namespace Server.Custom.VystiaClasses
{
    /// <summary>
    /// Rogue - Stealth-based melee DPS with combo points
    /// Region: Multi-Regional
    /// Role: Melee DPS
    /// </summary>
    public class RogueClass : PlayerClass
    {
        public override PlayerClassType ClassType => PlayerClassType.Rogue;
        public override string ClassName => "Rogue";
        public override string ClassDescription => "Shadow operative who relies on stealth, poisons, and precision strikes.";

        // Stats (Total: 85) - High Dex melee
        public override int StartStr => 25;
        public override int StartDex => 50;
        public override int StartInt => 10;

        // Stat Caps
        public override int StrCap => 110;
        public override int DexCap => 145;
        public override int IntCap => 95;

        // Primary Skills (7 skills)
        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Subterfuge, // Primary class skill - combo generation
            SkillName.Fencing, SkillName.Hiding, SkillName.Stealth,
            SkillName.Tactics, SkillName.Anatomy, SkillName.Poisoning
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, // Subterfuge
            50.0, 45.0, 40.0, 40.0, 30.0, 25.0
        };

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Light leather armor
            m.AddItem(new LeatherChest() { Name = "Rogue's Jerkin", Hue = 1109 });
            m.AddItem(new LeatherLegs() { Name = "Rogue's Leggings", Hue = 1109 });
            m.AddItem(new LeatherGloves() { Hue = 1109 });
            m.AddItem(new LeatherArms() { Hue = 1109 });
            m.AddItem(new LeatherCap() { Name = "Rogue's Hood", Hue = 1109 });
            m.AddItem(new Boots() { Hue = 1109 });

            // Weapon
            m.AddItem(new Dagger() { Name = "Rogue's Dagger", Hue = 1109 });

            // Starting supplies
            m.Backpack.DropItem(new Gold(100));
        }
    }
}
