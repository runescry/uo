using System;
using Server;
using Server.Items;
using Server.Items.VystiaClassItems;

namespace Server.Custom.VystiaClasses
{
    /// <summary>
    /// Templar - Holy warrior combining heavy armor with divine magic
    /// Region: Ironclad
    /// Role: Tank/DPS
    /// </summary>
    public class TemplarClass : PlayerClass
    {
        public override PlayerClassType ClassType => PlayerClassType.Templar;
        public override string ClassName => "Templar";
        public override string ClassDescription => "Holy warrior combining heavy armor with divine magic";

        // Stats (Total: 80) - Tank/DPS
        public override int StartStr => 40;  // High strength (heavy armor)
        public override int StartDex => 20;  // Medium dexterity
        public override int StartInt => 20;  // Medium intelligence (divine magic)

        // Stat Caps
        public override int StrCap => 130;
        public override int DexCap => 115;
        public override int IntCap => 105;

        // Primary Skills (7 skills)
        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Zealotry, // Primary class skill - zeal generation
            SkillName.Swords, SkillName.Tactics, SkillName.Parry, SkillName.Chivalry, SkillName.MagicResist, SkillName.Healing
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, // Zealotry
            50.0, 50.0, 45.0, 40.0, 35.0, 20.0
        };

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Plate Armor
            m.AddItem(new PlateChest() { Name = "Templar's Plate Chest", Hue = 2305 });
        m.AddItem(new PlateLegs() { Hue = 2305 });
        m.AddItem(new PlateArms() { Hue = 2305 });
        m.AddItem(new PlateGloves() { Hue = 2305 });
        m.AddItem(new PlateGorget() { Hue = 2305 });
        m.AddItem(new PlateHelm() { Hue = 2305 });
        m.AddItem(new MetalKiteShield() { Hue = 2305 });

            // Weapon
            m.AddItem(new Longsword() { Name = "Templar's Longsword", Hue = 2305 });

            // Starting Resources
            m.Backpack.DropItem(new SteamworkOre(20));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Special Class Item
            TemplarCross item = new TemplarCross();
        //         m.Backpack.DropItem(item);
        }
    }
}
