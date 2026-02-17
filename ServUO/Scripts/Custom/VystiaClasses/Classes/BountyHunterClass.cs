using System;
using Server;
using Server.Items;
using Server.Items.VystiaClassItems;

namespace Server.Custom.VystiaClasses
{
    /// <summary>
    /// BountyHunter - Tracker and assassin who hunts down targets
    /// Region: Multi-Regional
    /// Role: Ranged/Melee
    /// </summary>
    public class BountyHunterClass : PlayerClass
    {
        public override PlayerClassType ClassType => PlayerClassType.BountyHunter;
        public override string ClassName => "BountyHunter";
        public override string ClassDescription => "Tracker and assassin who hunts down targets";

        // Stats (Total: 80) - Ranged/Melee DPS
        public override int StartStr => 30;  // Medium strength (melee backup)
        public override int StartDex => 35;  // High dexterity (archery/stealth)
        public override int StartInt => 15;  // Low intelligence

        // Stat Caps
        public override int StrCap => 120;
        public override int DexCap => 130;
        public override int IntCap => 100;

        // Primary Skills (7 skills)
        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Manhunting, // Primary class skill - pursuit, tracking
            SkillName.Archery, SkillName.Tactics, SkillName.Tracking, SkillName.Stealth, SkillName.Hiding, SkillName.DetectHidden
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, // Manhunting
            50.0, 50.0, 50.0, 40.0, 35.0, 15.0
        };

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Leather Armor
            m.AddItem(new LeatherChest() { Name = "BountyHunter's Jerkin", Hue = 1153 });
        m.AddItem(new LeatherLegs() { Hue = 1153 });
        m.AddItem(new LeatherArms() { Hue = 1153 });
        m.AddItem(new LeatherGloves() { Hue = 1153 });
        m.AddItem(new LeatherGorget() { Hue = 1153 });
        m.AddItem(new LeatherCap() { Hue = 1153 });

            // Weapon
            m.AddItem(new Crossbow() { Name = "BountyHunter's Crossbow", Hue = 1153 });

            // Starting Resources
            m.Backpack.DropItem(new Gold(200));
        m.Backpack.DropItem(new Bolt(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Special Class Item
            BountyLedger item = new BountyLedger();
        //         m.Backpack.DropItem(item);
        }
    }
}
