using System;
using Server;
using Server.Items;
using Server.Items.VystiaClassItems;

namespace Server.Custom.VystiaClasses
{
    /// <summary>
    /// Knight - Honorable armored warrior trained in chivalry
    /// Region: Multi-Regional
    /// Role: Tank/Melee
    /// </summary>
    public class KnightClass : PlayerClass
    {
        public override PlayerClassType ClassType => PlayerClassType.Knight;
        public override string ClassName => "Knight";
        public override string ClassDescription => "Honorable armored warrior trained in chivalry";

        // Stats (Total: 80) - Tank
        public override int StartStr => 42;  // High strength (tank)
        public override int StartDex => 23;  // Medium dexterity
        public override int StartInt => 15;  // Low intelligence

        // Stat Caps
        public override int StrCap => 135;
        public override int DexCap => 115;
        public override int IntCap => 100;

        // Primary Skills (7 skills)
        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.ChivalricArts, // Primary class skill - fortitude, defense
            SkillName.Swords, SkillName.Tactics, SkillName.Parry, SkillName.Chivalry, SkillName.Anatomy, SkillName.MagicResist
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, // ChivalricArts
            55.0, 50.0, 50.0, 40.0, 30.0, 15.0
        };

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Plate Armor
            m.AddItem(new PlateChest() { Name = "Knight's Plate Chest", Hue = 1153 });
        m.AddItem(new PlateLegs() { Hue = 1153 });
        m.AddItem(new PlateArms() { Hue = 1153 });
        m.AddItem(new PlateGloves() { Hue = 1153 });
        m.AddItem(new PlateGorget() { Hue = 1153 });
        m.AddItem(new PlateHelm() { Hue = 1153 });
        m.AddItem(new MetalKiteShield() { Hue = 1153 });

            // Weapon
            m.AddItem(new Broadsword() { Name = "Knight's Broadsword", Hue = 1153 });

            // Starting Resources
            m.Backpack.DropItem(new Gold(150));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Special Class Item
            KnightBanner item = new KnightBanner();
        //         m.Backpack.DropItem(item);
        }
    }
}
