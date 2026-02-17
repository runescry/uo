using System;
using Server;
using Server.Items;
using Server.Items.VystiaClassItems;

namespace Server.Custom.VystiaClasses
{
    /// <summary>
    /// Artificer - Technoguild Artisan
    /// Medium all stats
    /// Focus: Constructs, gadgets, steam-powered technology
    /// </summary>
    public class ArtificerClass : PlayerClass
    {
        public override PlayerClassType ClassType => PlayerClassType.Artificer;
        public override string ClassName => "Artificer";
        public override string ClassDescription => "Technoguild Artisan - Master of clockwork constructs and steam technology";

        // Starting Stats (total: 80) - balanced crafter
        public override int StartStr => 25;  // Medium strength (crafting)
        public override int StartDex => 30;  // Medium dexterity (tinkering)
        public override int StartInt => 25;  // Medium intelligence (engineering)

        // Stat Caps - Artificers are balanced
        public override int StrCap => 110;
        public override int DexCap => 125;
        public override int IntCap => 115;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Engineering, // Primary class skill - construct power
            SkillName.Tinkering,   // Core crafting
            SkillName.Blacksmith,  // Metalworking
            SkillName.Inscribe,    // Enchanting
            SkillName.Archery,     // Ranged combat
            SkillName.Tactics,     // Combat effectiveness
            SkillName.AnimalTaming // Control constructs (repurposed)
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0,  // Engineering
            50.0,  // Tinkering
            40.0,  // Blacksmith
            40.0,  // Inscription
            40.0,  // Archery
            40.0,  // Tactics
            40.0   // Animal Taming (for construct control)
        };

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Artificer equipment
            LeatherChest chest = new LeatherChest();
            chest.Name = "Technoguild Engineer's Vest";
            chest.Hue = 2305; // Metallic
            m.AddItem(chest);

            LeatherLegs legs = new LeatherLegs();
            legs.Name = "Technoguild Engineer's Leggings";
            legs.Hue = 2305;
            m.AddItem(legs);

            LeatherGloves gloves = new LeatherGloves();
            gloves.Name = "Tinker's Gloves";
            gloves.Hue = 2305;
            m.AddItem(gloves);

            LeatherArms arms = new LeatherArms();
            arms.Name = "Technoguild Bracers";
            arms.Hue = 2305;
            m.AddItem(arms);

            // Goggles (using circlet)
            Circlet goggles = new Circlet();
            goggles.Name = "Artificer's Goggles";
            goggles.Hue = 2305;
            m.AddItem(goggles);

            Boots boots = new Boots();
            boots.Hue = 2305;
            m.AddItem(boots);

            // Starting weapon - crossbow
            Crossbow crossbow = new Crossbow();
            crossbow.Name = "Steam-Powered Crossbow";
            crossbow.Hue = 2305;
            m.AddItem(crossbow);

            // Bolts
            Bolt bolts = new Bolt(100);
            m.Backpack.DropItem(bolts);

            // Tinker tools
            TinkerTools tools = new TinkerTools();
            m.Backpack.DropItem(tools);

            // Starting resources
            m.Backpack.DropItem(new IronIngot(50));
            m.Backpack.DropItem(new SteamworkOre(10)); // Ironclad ore
            m.Backpack.DropItem(new ClockworkGear(10)); // Mechanical component
            m.Backpack.DropItem(new ClockworkSpring(10)); // Mechanical component

            // Construct Control Device
            ConstructControlDevice device = new ConstructControlDevice();
            m.Backpack.DropItem(device);

            // Starting gold
            m.Backpack.DropItem(new Gold(150));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            // Artificers get blueprints instead of spellbooks
            ArtificerBlueprints blueprints = new ArtificerBlueprints();
            m.Backpack.DropItem(blueprints);

            m.SendMessage(0x3B2, "You have been granted Artificer Blueprints!");
        }
    }
}
