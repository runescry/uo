using System;
using Server;
using Server.Items;
using Server.Items.VystiaClassItems;

namespace Server.Custom.VystiaClasses
{
    /// <summary>
    /// Barbarian - Frosthold Berserker
    /// High STR, medium CON, low INT
    /// Focus: Melee DPS, rage mechanics, cold resistance
    /// </summary>
    public class BarbarianClass : PlayerClass
    {
        public override PlayerClassType ClassType => PlayerClassType.Barbarian;
        public override string ClassName => "Barbarian";
        public override string ClassDescription => "Frosthold Berserker - Savage warriors who channel primal fury";

        // Starting Stats (total: 80)
        public override int StartStr => 45;  // High strength (Fury melee)
        public override int StartDex => 20;  // Low-medium dexterity
        public override int StartInt => 15;  // Low intelligence

        // Stat Caps - Barbarians favor STR
        public override int StrCap => 150;   // Higher than normal
        public override int DexCap => 110;   // Lower than normal
        public override int IntCap => 90;    // Lower than normal

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Berserking,  // Primary class skill - fury generation
            SkillName.Swords,      // Main weapon skill
            SkillName.Tactics,     // Combat effectiveness
            SkillName.Anatomy,     // Critical hits
            SkillName.Healing,     // Self-sustain
            SkillName.MagicResist, // Resist magic
            SkillName.Parry        // Defense
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0,  // Berserking
            50.0,  // Swords
            50.0,  // Tactics
            40.0,  // Anatomy
            30.0,  // Healing
            40.0,  // Magic Resist
            30.0   // Parry
        };

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Barbarian starting equipment
            // Weapon: Two-handed axe
            BattleAxe axe = new BattleAxe();
            axe.Name = "Berserker's Battleaxe";
            axe.Hue = 1150; // Ice blue
            m.AddItem(axe);

            // Armor: Fur and leather (barbarians don't wear heavy plate)
            LeatherChest chest = new LeatherChest();
            chest.Name = "Frosthold Fur Vest";
            chest.Hue = 1150;
            m.AddItem(chest);

            LeatherLegs legs = new LeatherLegs();
            legs.Name = "Frosthold Fur Leggings";
            legs.Hue = 1150;
            m.AddItem(legs);

            LeatherGloves gloves = new LeatherGloves();
            gloves.Name = "Berserker Wraps";
            gloves.Hue = 1150;
            m.AddItem(gloves);

            LeatherArms arms = new LeatherArms();
            arms.Name = "Frosthold Fur Bracers";
            arms.Hue = 1150;
            m.AddItem(arms);

            // No helmet - barbarians show their face
            // Boots
            ThighBoots boots = new ThighBoots();
            boots.Hue = 1150;
            m.AddItem(boots);

            // Starting supplies
            Bandage bandages = new Bandage(50);
            m.Backpack.DropItem(bandages);

            // Rage totem (special item for rage abilities)
            RageTotem totem = new RageTotem();
            m.Backpack.DropItem(totem);

            // Starting gold
            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            // Barbarians don't use traditional spellbooks
            // Their "abilities" are handled through the RageTotem item
            // and special combat moves (implemented separately)
        }
    }
}
