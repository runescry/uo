using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Custom.VystiaClasses;

namespace Server.Custom.VystiaClasses.Quests.ClassQuests
{
    /// <summary>
    /// Ice Mage class quests
    /// Tier 1: The Frozen Path
    /// </summary>

    #region Tier 1 Quests

    /// <summary>
    /// Ice Mage Initiation Quest - First quest for new Ice Mages
    /// </summary>
    public class IceMageInitiationQuest : VystiaQuest
    {
        public override string Title { get { return "Whispers of Winter"; } }
        public override string Description
        {
            get
            {
                return "The ice calls to you. Seek out the Ice Mage Archon in Frosthold " +
                       "to begin your training in the ways of Cryomancy. The frozen north " +
                       "holds many secrets for those who can endure its chill.";
            }
        }
        public override PlayerClassTypeV2 RequiredClass { get { return PlayerClassTypeV2.IceMage; } }
        public override QuestTier Tier { get { return QuestTier.Initiation; } }

        public IceMageInitiationQuest()
        {
            Objectives["talk_to_trainer"] = 1;
        }

        public override void GiveRewards(PlayerMobile pm)
        {
            // Give Ice Mage focus item
            var spellbook = new IceMageSpellbook();
            spellbook.Content = 0xF; // First 4 spells
            pm.Backpack?.DropItem(spellbook);

            // Give starter reagents
            pm.Backpack?.DropItem(new Frostbloom(10));
            pm.Backpack?.DropItem(new GlacierCrystal(10));

            // Skill boost
            pm.Skills[SkillName.Magery].Base += 5.0;

            pm.SendMessage("You have been granted a spellbook and starter reagents.");
        }
    }

    /// <summary>
    /// Ice Mage Gathering Quest - Collect Frosthold reagents
    /// </summary>
    public class IceMageGatheringQuest : VystiaQuest
    {
        public override string Title { get { return "Gathering Frost"; } }
        public override string Description
        {
            get
            {
                return "To truly master ice magic, you must understand its components. " +
                       "Gather 25 Frostbloom and 25 Glacier Crystals from the frozen lands. " +
                       "These reagents hold the essence of eternal winter.";
            }
        }
        public override PlayerClassTypeV2 RequiredClass { get { return PlayerClassTypeV2.IceMage; } }
        public override int PrerequisiteQuestID { get { return 1; } } // Requires Initiation
        public override QuestTier Tier { get { return QuestTier.Initiation; } }

        public IceMageGatheringQuest()
        {
            Objectives["collect_frostbloom"] = 25;
            Objectives["collect_glacier_crystal"] = 25;
        }

        public override void GiveRewards(PlayerMobile pm)
        {
            // Give more advanced reagents
            pm.Backpack?.DropItem(new PermafrostEssence(20));
            pm.Backpack?.DropItem(new ArcticPearl(20));

            // Gold reward
            pm.Backpack?.DropItem(new Gold(500));

            pm.SendMessage("Your dedication to the frozen arts is noted. Take these rare reagents.");
        }
    }

    /// <summary>
    /// Ice Mage Skill Quest - Use ice spells
    /// </summary>
    public class IceMageSkillQuest : VystiaQuest
    {
        public override string Title { get { return "Chill of Knowledge"; } }
        public override string Description
        {
            get
            {
                return "Knowledge without practice is worthless. Cast 10 ice spells successfully " +
                       "to prove your understanding of Cryomancy. Feel the cold flow through you " +
                       "and let it become an extension of your will.";
            }
        }
        public override PlayerClassTypeV2 RequiredClass { get { return PlayerClassTypeV2.IceMage; } }
        public override int PrerequisiteQuestID { get { return 2; } } // Requires Gathering
        public override QuestTier Tier { get { return QuestTier.Initiation; } }

        public IceMageSkillQuest()
        {
            Objectives["cast_ice_spell"] = 10;
        }

        public override void GiveRewards(PlayerMobile pm)
        {
            // Unlock more spells in spellbook
            foreach (Item item in pm.Backpack.Items)
            {
                if (item is IceMageSpellbook book)
                {
                    book.Content = 0xFF; // First 8 spells (Circle 1 & 2)
                    break;
                }
            }

            // Skill boost
            pm.Skills[SkillName.Magery].Base += 10.0;

            // Title
            pm.Title = "Ice Mage Initiate";

            pm.SendMessage("You have earned the title of Ice Mage Initiate!");
        }
    }

    #endregion

    #region Tier 2 Quests

    /// <summary>
    /// Ice Mage Artifact Quest - Find the Heart of Winter
    /// </summary>
    public class IceMageArtifactQuest : VystiaQuest
    {
        public override string Title { get { return "The Frozen Heart"; } }
        public override string Description
        {
            get
            {
                return "Deep within the Glacier Caves lies an artifact of immense power - " +
                       "the Heart of Winter. This crystallized essence of eternal cold will " +
                       "greatly enhance your mastery over ice magic. Seek it out and claim it.";
            }
        }
        public override PlayerClassTypeV2 RequiredClass { get { return PlayerClassTypeV2.IceMage; } }
        public override int PrerequisiteQuestID { get { return 3; } } // Requires Skill Quest
        public override QuestTier Tier { get { return QuestTier.Apprentice; } }

        public IceMageArtifactQuest()
        {
            Objectives["find_heart_of_winter"] = 1;
        }

        public override void GiveRewards(PlayerMobile pm)
        {
            // Give Heart of Winter reagent item
            pm.Backpack?.DropItem(new Server.Items.HeartOfWinter(5));

            // Give a special frost ring
            var ring = new GoldRing();
            ring.Name = "Ring of Frost";
            ring.Hue = 0x481;
            ring.Attributes.CastSpeed = 1;
            ring.Attributes.LowerManaCost = 5;
            ring.Attributes.RegenMana = 2;
            pm.Backpack?.DropItem(ring);

            pm.SendMessage("The Heart of Winter's power flows through you!");
        }
    }

    #endregion

    /// <summary>
    /// Registers all Ice Mage quests on server startup
    /// </summary>
    public static class IceMageQuestInitializer
    {
        public static void Initialize()
        {
            // Register Tier 1 quests
            VystiaQuestSystem.RegisterQuest(new IceMageInitiationQuest()); // ID 1
            VystiaQuestSystem.RegisterQuest(new IceMageGatheringQuest()); // ID 2
            VystiaQuestSystem.RegisterQuest(new IceMageSkillQuest()); // ID 3

            // Register Tier 2 quests
            VystiaQuestSystem.RegisterQuest(new IceMageArtifactQuest()); // ID 4

            Console.WriteLine("VystiaQuests: Registered 4 Ice Mage quests");
        }
    }
}
