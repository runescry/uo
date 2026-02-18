using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Quests.Generation;

namespace Server.Custom.VystiaClasses.Quests.Generation
{
    /// <summary>
    /// Pre-written quest templates for fallback when LLM generation fails
    /// These provide engaging, functional quests when external services are unavailable
    /// </summary>
    public static class QuestTemplateLibrary
    {
        private static readonly List<QuestTemplate> s_Templates = new List<QuestTemplate>();

        static QuestTemplateLibrary()
        {
            InitializeTemplates();
        }

        /// <summary>
        /// Get a random quest template appropriate for the player's context
        /// </summary>
        public static QuestTemplate GetRandomTemplate(PlayerMobile player, string theme = null)
        {
            if (player == null)
                return GetDefaultTemplate();

            // Filter templates by theme if specified
            var availableTemplates = s_Templates;
            if (!string.IsNullOrWhiteSpace(theme))
            {
                availableTemplates = s_Templates.Where(t => 
                    t.Theme.Equals(theme, StringComparison.OrdinalIgnoreCase) ||
                    t.Tags.Any(tag => tag.Equals(theme, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            // If no theme-specific templates found, use all templates
            if (availableTemplates.Count == 0)
                availableTemplates = s_Templates;

            // Filter by player level/class appropriateness
            var appropriateTemplates = availableTemplates.Where(t => 
                t.IsAppropriateForPlayer(player)
            ).ToList();

            // If no appropriate templates, use all templates as fallback
            if (appropriateTemplates.Count == 0)
                appropriateTemplates = availableTemplates;

            // Return random template
            return appropriateTemplates[Utility.Random(appropriateTemplates.Count)];
        }

        /// <summary>
        /// Get a default template when no specific template matches
        /// </summary>
        public static QuestTemplate GetDefaultTemplate()
        {
            return s_Templates.FirstOrDefault(t => t.IsDefault) ?? s_Templates.First();
        }

        /// <summary>
        /// Initialize all quest templates
        /// </summary>
        private static void InitializeTemplates()
        {
            // Combat-focused templates
            AddTemplate(new QuestTemplate
            {
                Title = "The Bandit Menace",
                Description = "Local merchants are being terrorized by bandits. Help clear the roads and restore safety to the region.",
                Tier = "Initiation",
                Theme = "Combat",
                Tags = new[] { "combat", "bandits", "merchants" },
                Waypoints = new[]
                {
                    new WaypointTemplate
                    {
                        Type = "Origin",
                        Name = "Start Your Journey",
                        Description = "Speak with the local merchant who needs help.",
                        Condition = "TalkToNPC"
                    },
                    new WaypointTemplate
                    {
                        Type = "Waypoint",
                        Name = "Clear the Bandit Camp",
                        Description = "Find and defeat the bandits terrorizing the merchants.",
                        Condition = "DefeatBoss",
                        PoiId = "BANDIT_CAMP"
                    },
                    new WaypointTemplate
                    {
                        Type = "BossCompletion",
                        Name = "Report Your Success",
                        Description = "Return to the merchant with news of your victory.",
                        Condition = "TalkToNPC"
                    }
                },
                Rewards = new[] { "Gold", "Reputation", "Experience" },
                IsDefault = true
            });

            AddTemplate(new QuestTemplate
            {
                Title = "The Lost Artifact",
                Description = "A precious artifact has been stolen from the local museum. Recover it before it's lost forever.",
                Tier = "Apprentice",
                Theme = "Investigation",
                Tags = new[] { "investigation", "artifact", "museum" },
                Waypoints = new[]
                {
                    new WaypointTemplate
                    {
                        Type = "Origin",
                        Name = "Museum Curator",
                        Description = "Speak with the museum curator about the stolen artifact.",
                        Condition = "TalkToNPC"
                    },
                    new WaypointTemplate
                    {
                        Type = "Waypoint",
                        Name = "Follow the Trail",
                        Description = "Investigate the theft scene and follow the clues.",
                        Condition = "ReachLocation",
                        PoiId = "THEFT_SCENE"
                    },
                    new WaypointTemplate
                    {
                        Type = "Waypoint",
                        Name = "Confront the Thief",
                        Description = "Find the thief and recover the stolen artifact.",
                        Condition = "DefeatBoss",
                        PoiId = "THIEF_HIDEOUT"
                    },
                    new WaypointTemplate
                    {
                        Type = "NPCCompletion",
                        Name = "Return the Artifact",
                        Description = "Bring the artifact back to the museum curator.",
                        Condition = "TalkToNPC"
                    }
                },
                Rewards = new[] { "Gold", "Reputation", "UniqueItem" }
            });

            // Collection templates
            AddTemplate(new QuestTemplate
            {
                Title = "Rare Herb Gathering",
                Description = "The local alchemist needs rare herbs for an important potion. Gather them from the dangerous wilds.",
                Tier = "Initiation",
                Theme = "Collection",
                Tags = new[] { "collection", "herbs", "alchemy" },
                Waypoints = new[]
                {
                    new WaypointTemplate
                    {
                        Type = "Origin",
                        Name = "Alchemist's Request",
                        Description = "Speak with the alchemist who needs rare herbs.",
                        Condition = "TalkToNPC"
                    },
                    new WaypointTemplate
                    {
                        Type = "Waypoint",
                        Name = "Gather Bloodmoss",
                        Description = "Collect rare bloodmoss from the swamp regions.",
                        Condition = "CollectItems",
                        RequiredItems = new[] { "Bloodmoss" },
                        RequiredAmount = 10
                    },
                    new WaypointTemplate
                    {
                        Type = "Waypoint",
                        Name = "Find Mandrake Root",
                        Description = "Locate and gather mandrake root from the forest.",
                        Condition = "CollectItems",
                        RequiredItems = new[] { "MandrakeRoot" },
                        RequiredAmount = 5
                    },
                    new WaypointTemplate
                    {
                        Type = "NPCCompletion",
                        Name = "Deliver the Herbs",
                        Description = "Bring the collected herbs to the alchemist.",
                        Condition = "TalkToNPC"
                    }
                },
                Rewards = new[] { "Gold", "Potions", "Experience" }
            });

            // Exploration templates
            AddTemplate(new QuestTemplate
            {
                Title = "The Ancient Ruins",
                Description = "Mysterious ruins have been discovered nearby. Explore them and uncover their secrets.",
                Tier = "Journeyman",
                Theme = "Exploration",
                Tags = new[] { "exploration", "ruins", "mystery" },
                Waypoints = new[]
                {
                    new WaypointTemplate
                    {
                        Type = "Origin",
                        Name = "Historian's Tale",
                        Description = "Speak with the historian who knows about the ancient ruins.",
                        Condition = "TalkToNPC"
                    },
                    new WaypointTemplate
                    {
                        Type = "Waypoint",
                        Name = "Find the Ruins",
                        Description = "Locate the ancient ruins in the wilderness.",
                        Condition = "ReachLocation",
                        PoiId = "ANCIENT_RUINS"
                    },
                    new WaypointTemplate
                    {
                        Type = "Waypoint",
                        Name = "Explore the Interior",
                        Description = "Enter the ruins and search for valuable artifacts or clues.",
                        Condition = "ReachLocation",
                        PoiId = "RUINS_INTERIOR"
                    },
                    new WaypointTemplate
                    {
                        Type = "BossCompletion",
                        Name = "Defeat the Guardian",
                        Description = "Face the ancient guardian protecting the ruins' greatest secret.",
                        Condition = "DefeatBoss",
                        PoiId = "RUINS_GUARDIAN"
                    },
                    new WaypointTemplate
                    {
                        Type = "NPCCompletion",
                        Name = "Share Your Discovery",
                        Description = "Report your findings to the historian.",
                        Condition = "TalkToNPC"
                    }
                },
                Rewards = new[] { "Gold", "Reputation", "Knowledge", "UniqueItem" }
            });

            // Social templates
            AddTemplate(new QuestTemplate
            {
                Title = "The Missing Child",
                Description = "A child has gone missing from the village. Help find them before it's too late.",
                Tier = "Apprentice",
                Theme = "Social",
                Tags = new[] { "social", "missing", "rescue" },
                Waypoints = new[]
                {
                    new WaypointTemplate
                    {
                        Type = "Origin",
                        Name = "Worried Parent",
                        Description = "Speak with the worried parent about their missing child.",
                        Condition = "TalkToNPC"
                    },
                    new WaypointTemplate
                    {
                        Type = "Waypoint",
                        Name = "Gather Information",
                        Description = "Talk to villagers and gather clues about the child's whereabouts.",
                        Condition = "TalkToNPC"
                    },
                    new WaypointTemplate
                    {
                        Type = "Waypoint",
                        Name = "Follow the Trail",
                        Description = "Follow the clues to find where the child might be.",
                        Condition = "ReachLocation",
                        PoiId = "CHILD_LOCATION"
                    },
                    new WaypointTemplate
                    {
                        Type = "NPCCompletion",
                        Name = "Rescue the Child",
                        Description = "Find and rescue the missing child.",
                        Condition = "TalkToNPC"
                    },
                    new WaypointTemplate
                    {
                        Type = "NPCCompletion",
                        Name = "Return Home",
                        Description = "Bring the child safely back to their parent.",
                        Condition = "TalkToNPC"
                    }
                },
                Rewards = new[] { "Gold", "Reputation", "Blessing" }
            });
        }

        private static void AddTemplate(QuestTemplate template)
        {
            s_Templates.Add(template);
        }

        /// <summary>
        /// Get all available templates for debugging
        /// </summary>
        public static List<QuestTemplate> GetAllTemplates()
        {
            return new List<QuestTemplate>(s_Templates);
        }
    }

    /// <summary>
    /// Represents a pre-written quest template
    /// </summary>
    public class QuestTemplate
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tier { get; set; }
        public string Theme { get; set; }
        public string[] Tags { get; set; }
        public WaypointTemplate[] Waypoints { get; set; }
        public string[] Rewards { get; set; }
        public bool IsDefault { get; set; }

        /// <summary>
        /// Check if this template is appropriate for the player
        /// </summary>
        public bool IsAppropriateForPlayer(PlayerMobile player)
        {
            // Could implement level/class checks here
            // For now, all templates are appropriate
            return true;
        }

        /// <summary>
        /// Convert this template to a DynamicQuestPlan JSON
        /// </summary>
        public string ToJson()
        {
            var json = $@"{{
  ""schemaVersion"": 1,
  ""title"": ""{Title}"",
  ""description"": ""{Description}"",
  ""tier"": ""{Tier}"",
  ""expiresMinutes"": 120,
  ""waypoints"": [
";

            for (int i = 0; i < Waypoints.Length; i++)
            {
                var waypoint = Waypoints[i];
                json += $@"
    {{
      ""type"": ""{waypoint.Type}"",
      ""condition"": ""{waypoint.Condition}"",
      ""name"": ""{waypoint.Name}"",
      ""description"": ""{waypoint.Description}""";

                if (!string.IsNullOrEmpty(waypoint.PoiId))
                    json += $@",
      ""poiId"": ""{waypoint.PoiId}""";

                if (waypoint.RequiredItems?.Length > 0)
                    json += $@",
      ""requiredItems"": [{string.Join(", ", waypoint.RequiredItems.Select(item => $@"""{item}"""))}]";

                if (waypoint.RequiredAmount > 0)
                    json += $@",
      ""requiredAmount"": {waypoint.RequiredAmount}";

                json += @"
    }";

                if (i < Waypoints.Length - 1)
                    json += ",";
            }

            json += @"
  ]
}";

            return json;
        }
    }

    /// <summary>
    /// Represents a single waypoint in a quest template
    /// </summary>
    public class WaypointTemplate
    {
        public string Type { get; set; }
        public string Condition { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PoiId { get; set; }
        public string[] RequiredItems { get; set; }
        public int RequiredAmount { get; set; }
    }
}
