using System;
using Server;
using Server.Items;
using Server.Items.VystiaClassItems;

namespace Server.Custom.VystiaClasses
{
    /// <summary>
    /// Monk - Unarmed martial artist with spiritual abilities
    /// Region: Ironclad
    /// Role: Melee/Hybrid
    /// </summary>
    public class MonkClass : PlayerClass
    {
        public override PlayerClassType ClassType => PlayerClassType.Monk;
        public override string ClassName => "Monk";
        public override string ClassDescription => "Unarmed martial artist with spiritual abilities";

        // Stats (Total: 80) - DEX Melee
        public override int StartStr => 30;  // Medium strength
        public override int StartDex => 35;  // High dexterity (martial arts)
        public override int StartInt => 15;  // Low intelligence

        // Stat Caps
        public override int StrCap => 120;
        public override int DexCap => 130;
        public override int IntCap => 100;

        // Primary Skills (7 skills)
        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.MartialArts, // Primary class skill - chi generation
            SkillName.Wrestling, SkillName.Tactics, SkillName.Anatomy, SkillName.Healing, SkillName.Focus, SkillName.Meditation
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, // MartialArts
            55.0, 50.0, 45.0, 40.0, 35.0, 15.0
        };

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Cloth Armor
            m.AddItem(new Robe() { Name = "Monk's Robe", Hue = 2305 });
        m.AddItem(new Sandals() { Hue = 2305 });
        m.AddItem(new WizardsHat() { Hue = 2305 });

            // Weapon
                    // Unarmed combat - no weapon

            // Starting Resources
            m.Backpack.DropItem(new SteamworkOre(20));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Special Class Item
            MonkBeads item = new MonkBeads();
        //         m.Backpack.DropItem(item);
        }
    }
}
