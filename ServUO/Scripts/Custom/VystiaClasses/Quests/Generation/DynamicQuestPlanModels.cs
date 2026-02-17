using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Server.Custom.VystiaClasses.Quests.Generation
{
    // NOTE: These models are the contract between the LLM planner and the server compiler.
    // Keep them modular, versioned, and validated before spawning anything.

    public class DynamicQuestPlan
    {
        [JsonProperty("schemaVersion")]
        public int SchemaVersion { get; set; } = 1;

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("tier")]
        public string Tier { get; set; } // maps to QuestTier enum by name

        [JsonProperty("expiresMinutes")]
        public int ExpiresMinutes { get; set; } = 120;

        [JsonProperty("waypoints")]
        public List<DynamicQuestWaypointPlan> Waypoints { get; set; } = new List<DynamicQuestWaypointPlan>();
    }

    public class DynamicQuestWaypointPlan
    {
        [JsonProperty("type")]
        public string Type { get; set; } // Origin|Waypoint|BossCompletion|NPCCompletion

        [JsonProperty("condition")]
        public string Condition { get; set; } // TalkToNPC|ReachLocation|DefeatBoss|CollectItems|RecruitSidekick|Custom

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("poiId")]
        public string PoiId { get; set; }

        [JsonProperty("npcTemplateId")]
        public string NpcTemplateId { get; set; }

        [JsonProperty("bossTypeName")]
        public string BossTypeName { get; set; }

        [JsonProperty("requiredAmount")]
        public int RequiredAmount { get; set; } = 1;

        [JsonProperty("radius")]
        public int Radius { get; set; } = 10;

        [JsonProperty("npcDialogueContext")]
        public string NpcDialogueContext { get; set; }

        [JsonProperty("playerLocationHint")]
        public string PlayerLocationHint { get; set; }
    }

    public static class DynamicQuestPlanJson
    {
        public static DynamicQuestPlan Deserialize(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                Console.WriteLine($"[DynamicQuestPlan] ERROR: JSON is null or empty");
                return null;
            }

            // Log the JSON for debugging
            Console.WriteLine($"[DynamicQuestPlan] Attempting to deserialize JSON (length: {json.Length})");
            Console.WriteLine($"[DynamicQuestPlan] JSON preview: {json.Substring(0, Math.Min(500, json.Length))}...");

            try
            {
                var settings = new Newtonsoft.Json.JsonSerializerSettings
                {
                    MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore,
                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                    Error = (sender, args) =>
                    {
                        // Log but don't throw for missing properties
                        Console.WriteLine($"[DynamicQuestPlan] JSON Error: {args.ErrorContext.Error.Message}");
                        args.ErrorContext.Handled = true;
                    }
                };

                var result = JsonConvert.DeserializeObject<DynamicQuestPlan>(json, settings);
                
                if (result == null)
                {
                    Console.WriteLine($"[DynamicQuestPlan] ERROR: Deserialization returned null. JSON may be invalid.");
                    Console.WriteLine($"[DynamicQuestPlan] Full JSON was: {json}");
                }
                else
                {
                    Console.WriteLine($"[DynamicQuestPlan] Successfully deserialized. Title: {result.Title}, Waypoints: {result.Waypoints?.Count ?? 0}");
                }
                
                return result;
            }
            catch (Newtonsoft.Json.JsonException ex)
            {
                Console.WriteLine($"[DynamicQuestPlan] JSON Deserialization Error: {ex.Message}");
                // JsonException doesn't have Path, LineNumber, LinePosition in all versions
                // Try to get path if available via reflection or just use message
                if (ex.Data != null && ex.Data.Contains("Path"))
                {
                    Console.WriteLine($"[DynamicQuestPlan] Error at Path: {ex.Data["Path"]}");
                }
                Console.WriteLine($"[DynamicQuestPlan] JSON was: {json.Substring(0, Math.Min(1000, json.Length))}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DynamicQuestPlan] Unexpected error during deserialization: {ex.Message}");
                Console.WriteLine($"[DynamicQuestPlan] Exception type: {ex.GetType().Name}");
                Console.WriteLine($"[DynamicQuestPlan] Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public static string Serialize(DynamicQuestPlan plan)
        {
            if (plan == null)
                return null;

            return JsonConvert.SerializeObject(plan, Formatting.Indented);
        }
    }
}


