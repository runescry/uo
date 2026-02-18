using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Gumps;
using Server.Services.LLM;
using Server.Custom.VystiaClasses;
using Server.Custom.VystiaClasses.Quests;
using Server.Engines.PartySystem;

namespace Server.Custom.VystiaClasses.Quests.Generation
{
    /// <summary>
    /// Orchestrates: (plan json) -> validate -> compile/spawn -> attach instance -> show offer.
    /// This is intentionally modular so we can swap planners and expand schemas.
    /// </summary>
    public static class LLMQuestGenerationService
    {
        public static IQuestPlanValidator Validator { get; set; } = new QuestPlanValidator();
        public static IQuestPlanCompiler Compiler { get; set; } = new QuestPlanCompilerAndSpawner();

        /// <summary>
        /// MVP hook: accept a JSON plan (from an LLM planner) and turn it into a playable quest instance.
        /// </summary>
        public static bool CreateFromPlanJson(PlayerMobile owner, string planJson, out int questId, out string error)
        {
            questId = 0;
            error = null;

            if (owner == null || owner.Deleted)
            {
                error = "Invalid owner.";
                return false;
            }

            VystiaPoiRegistry.EnsureLoaded();

            // Validate JSON before deserialization
            if (string.IsNullOrWhiteSpace(planJson))
            {
                error = "Plan JSON is empty or null.";
                return false;
            }

            // Log the JSON for debugging (truncated if too long)
            string jsonPreview = planJson.Length > 500 ? planJson.Substring(0, 500) + "..." : planJson;
            Console.WriteLine($"[LLMQuestGeneration] Attempting to deserialize JSON (length: {planJson.Length}):\n{jsonPreview}");

            DynamicQuestPlan plan;
            try
            {
                plan = DynamicQuestPlanJson.Deserialize(planJson);
                if (plan == null)
                {
                    error = "Deserialization returned null. JSON may be invalid.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                error = $"Failed to parse plan JSON: {ex.Message}";
                Console.WriteLine($"[LLMQuestGeneration] JSON Parse Error: {ex.Message}\nJSON was: {jsonPreview}");
                return false;
            }

            var result = Validator.Validate(plan);
            if (!result.Success)
            {
                error = "Plan validation failed:\n- " + string.Join("\n- ", result.Errors);
                return false;
            }

            var compiled = Compiler.CompileAndSpawn(owner, plan);
            if (compiled == null || compiled.QuestId <= 0)
            {
                error = "Failed to compile quest plan.";
                return false;
            }

            questId = compiled.QuestId;

            // Track instance for cleanup (temporary spawns)
            var attachment = GeneratedQuestInstanceAttachment.GetOrCreate(owner);
            attachment?.AddInstance(compiled);

            // Show offer to start quest
            owner.SendGump(new VystiaQuestOfferGump(owner, questId, false));
            return true;
        }

        /// <summary>
        /// Generate a quest plan using LLM with fallback to templates when service fails
        /// </summary>
        public static async Task<string> GeneratePlanJsonAsync(PlayerMobile owner, string theme = null)
        {
            if (owner == null || owner.Deleted)
                return null;

            VystiaPoiRegistry.EnsureLoaded();
            VystiaNpcTemplateRegistry.EnsureLoaded();

            // Build context for LLM
            var context = BuildPlayerContext(owner, theme);

            // Try LLM generation with fallback
            return await CircuitBreaker.ExecuteAsync(
                async () => await RetryPolicy.ExecuteAsync(
                    async () => await GenerateLLMPlanAsync(owner, context),
                    () => GenerateFallbackPlan(owner, theme)
                ),
                () => GenerateFallbackPlan(owner, theme)
            );
        }

        /// <summary>
        /// Generate quest plan using LLM service
        /// </summary>
        private static async Task<string> GenerateLLMPlanAsync(PlayerMobile owner, string context)
        {
            // Build LLM prompt
            var prompt = BuildLLMPrompt(context);

            // Call UnifiedLLMService to generate JSON plan
            var conversationHistory = new List<ConversationMessage>();
            var response = await UnifiedLLMService.GetResponseAsync(
                npcName: "Quest Planner",
                npcPersonality: "Analytical",
                conversationHistory: conversationHistory,
                playerMessage: prompt,
                playerName: owner.Name,
                preloadedKnowledge: context,
                requestType: UnifiedLLMService.RequestType.QuestDialogue,
                providerOverride: UnifiedLLMService.LLMProvider.OpenAI
            );

            // Extract JSON from response (may be wrapped in markdown code blocks)
            Console.WriteLine($"[LLMQuestGeneration] Raw LLM response (length: {response?.Length ?? 0}): {response?.Substring(0, Math.Min(500, response?.Length ?? 0)) ?? "null"}...");
            
            string json = ExtractJsonFromResponse(response);
            
            if (string.IsNullOrWhiteSpace(json))
            {
                Console.WriteLine($"[LLMQuestGeneration] ERROR: ExtractJsonFromResponse returned null or empty. Raw response was: {response?.Substring(0, Math.Min(1000, response?.Length ?? 0)) ?? "null"}");
                throw new InvalidOperationException("Failed to extract JSON from LLM response");
            }
            
            Console.WriteLine($"[LLMQuestGeneration] Extracted JSON (length: {json.Length}): {json.Substring(0, Math.Min(500, json.Length))}...");
            
            // Validate JSON structure before returning
            if (!json.TrimStart().StartsWith("{"))
            {
                Console.WriteLine($"[LLMQuestGeneration] ERROR: Extracted JSON does not start with '{{'. First 200 chars: {json.Substring(0, Math.Min(200, json.Length))}");
                throw new InvalidOperationException("Invalid JSON structure from LLM");
            }
            
            return json;
        }

        /// <summary>
        /// Generate fallback quest plan from templates when LLM fails
        /// </summary>
        private static string GenerateFallbackPlan(PlayerMobile owner, string theme)
        {
            Console.WriteLine($"[LLMQuestGeneration] Using fallback quest generation for {owner.Name}");
            
            try
            {
                var template = QuestTemplateLibrary.GetRandomTemplate(owner, theme);
                string json = template.ToJson();
                
                Console.WriteLine($"[LLMQuestGeneration] Generated fallback quest: {template.Title}");
                Console.WriteLine($"[LLMQuestGeneration] Fallback JSON (length: {json.Length}): {json.Substring(0, Math.Min(300, json.Length))}...");
                
                return json;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LLMQuestGeneration] Error in fallback generation: {ex.Message}");
                
                // Ultimate fallback - return a very simple quest
                return GetUltimateFallbackQuest();
            }
        }

        /// <summary>
        /// Ultimate fallback when even template system fails
        /// </summary>
        private static string GetUltimateFallbackQuest()
        {
            Console.WriteLine("[LLMQuestGeneration] Using ultimate fallback quest");
            
            return @"{
  ""schemaVersion"": 1,
  ""title"": ""A Simple Errand"",
  ""description"": ""The local merchant needs help with a simple task. Assist them to earn some gold and experience."",
  ""tier"": ""Initiation"",
  ""expiresMinutes"": 60,
  ""waypoints"": [
    {
      ""type"": ""Origin"",
      ""condition"": ""TalkToNPC"",
      ""name"": ""Merchant's Request"",
      ""description"": ""Speak with the local merchant who needs assistance.""
    },
    {
      ""type"": ""Waypoint"",
      ""condition"": ""ReachLocation"",
      ""name"": ""Complete the Task"",
      ""description"": ""Help the merchant with their request."",
      ""poiId"": ""TOWN_CENTER""
    },
    {
      ""type"": ""NPCCompletion"",
      ""condition"": ""TalkToNPC"",
      ""name"": ""Report Completion"",
      ""description"": ""Return to the merchant and report your success.""
    }
  ]
}";
        }

        private static string BuildPlayerContext(PlayerMobile owner, string theme)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== PLAYER CONTEXT ===");
            sb.AppendLine($"Player: {owner.Name}");
            sb.AppendLine($"Class: {owner.VystiaClassV2}");
            sb.AppendLine($"Location: {owner.Map?.Name ?? "Unknown"} at ({owner.Location.X}, {owner.Location.Y})");
            
            // Party info
            var party = owner.Party as Server.Engines.PartySystem.Party;
            if (party != null && party.Members != null)
            {
                sb.AppendLine($"Party Size: {party.Members.Count}");
                var memberNames = party.Members.Where(m => m != null && m.Mobile != null && !m.Mobile.Deleted).Select(m => m.Mobile.Name).ToList();
                if (memberNames.Count > 0)
                    sb.AppendLine($"Party Members: {string.Join(", ", memberNames)}");
            }
            else
            {
                sb.AppendLine("Party: None (solo)");
            }

            if (!string.IsNullOrWhiteSpace(theme))
            {
                sb.AppendLine($"Theme Request: {theme}");
            }

            // Add active quests to context so LLM avoids duplicates
            var tracker = VystiaQuestTracker.GetTracker(owner);
            if (tracker != null)
            {
                var activeQuests = tracker.GetActiveQuests();
                if (activeQuests.Count > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine("=== PLAYER'S ACTIVE QUESTS (DO NOT DUPLICATE) ===");
                    foreach (int questId in activeQuests)
                    {
                        var quest = DynamicQuestManager.GetQuest(questId);
                        if (quest != null)
                        {
                            sb.AppendLine($"- ACTIVE: \"{quest.Title}\" (ID: {questId}) - {quest.Description}");
                        }
                        else
                        {
                            // Try VystiaQuestSystem for non-dynamic quests
                            var vq = VystiaQuestSystem.GetQuest(questId);
                            if (vq != null)
                            {
                                sb.AppendLine($"- ACTIVE: \"{vq.Title}\" (ID: {questId})");
                            }
                        }
                    }
                }
            }

            // Add recently generated quests (even if not active) to avoid similar titles/themes
            var genTracker = GeneratedQuestInstanceAttachment.Get(owner);
            if (genTracker != null && genTracker.Instances != null && genTracker.Instances.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("=== RECENTLY GENERATED QUESTS (AVOID SIMILAR THEMES/TITLES) ===");
                var recentQuests = genTracker.Instances
                    .OrderByDescending(i => i.ExpiresAtUtc)
                    .Take(5) // Only show last 5 to avoid prompt bloat
                    .ToList();
                
                foreach (var instance in recentQuests)
                {
                    var quest = DynamicQuestManager.GetQuest(instance.QuestId);
                    if (quest != null)
                    {
                        sb.AppendLine($"- RECENT: \"{quest.Title}\" (ID: {instance.QuestId}) - {quest.Description}");
                    }
                }
            }

            sb.AppendLine();
            sb.AppendLine("=== AVAILABLE POIs ===");
            foreach (var poi in VystiaPoiRegistry.GetAll())
            {
                sb.AppendLine($"- {poi.PoiId}: {poi.PoiName} ({poi.MapName} at {poi.X}, {poi.Y})");
            }

            sb.AppendLine();
            sb.AppendLine("=== AVAILABLE NPC TEMPLATES ===");
            sb.AppendLine("Faction Leaders:");
            foreach (var npc in VystiaNpcTemplateRegistry.GetByCategory("FACTION_LEADER"))
            {
                sb.AppendLine($"  - {npc.TemplateId}: {npc.DisplayName} ({npc.MobileTypeName})");
            }
            sb.AppendLine("Ancient Beings:");
            foreach (var npc in VystiaNpcTemplateRegistry.GetByCategory("ANCIENT"))
            {
                sb.AppendLine($"  - {npc.TemplateId}: {npc.DisplayName} ({npc.MobileTypeName})");
            }
            sb.AppendLine("Bosses:");
            foreach (var npc in VystiaNpcTemplateRegistry.GetByCategory("BOSS"))
            {
                sb.AppendLine($"  - {npc.TemplateId}: {npc.DisplayName} ({npc.MobileTypeName})");
            }

            return sb.ToString();
        }

        private static string BuildLLMPrompt(string context)
        {
            var sb = new StringBuilder();
            sb.AppendLine("You are a quest planner for the Vystia shard. Generate a DynamicQuestPlan JSON for a player.");
            sb.AppendLine();
            sb.AppendLine(context);
            sb.AppendLine();
            sb.AppendLine("=== CRITICAL REQUIREMENTS ===");
            sb.AppendLine("1. MUST include exactly 1 Origin waypoint (type: \"Origin\")");
            sb.AppendLine("2. MUST include exactly 1 Completion waypoint (type: \"BossCompletion\" OR \"NPCCompletion\")");
            sb.AppendLine("3. Every waypoint MUST have: type, condition, name, description, poiId");
            sb.AppendLine("4. Valid waypoint types: Origin, Waypoint, BossCompletion, NPCCompletion");
            sb.AppendLine("5. Valid conditions: TalkToNPC, ReachLocation, DefeatBoss, CollectItems, RecruitSidekick, Custom");
            sb.AppendLine("6. If condition is \"DefeatBoss\", MUST include bossTypeName (one of: FrostFather, ForgeMaster, CovenMatriarch, VolcanoWyrm, TimewornLich, AncientTreant, CrystalDrakeAlpha, SphinxOfSurya, AncientKraken, GriffinLord)");
            sb.AppendLine("7. Every waypoint MUST have a valid poiId from the available POIs list");
            sb.AppendLine("8. expiresMinutes must be between 10 and 1440");
            sb.AppendLine("9. CRITICAL: Generate a UNIQUE quest title and theme. Do NOT reuse titles or themes from the player's ACTIVE or RECENT quests listed above.");
            sb.AppendLine("10. Vary the quest locations, NPCs, and bosses. Do not generate multiple quests with the same boss or similar locations.");
            sb.AppendLine("11. CRITICAL: The 'description' field must be a rich, engaging storyline (2-4 sentences) that sets the scene, explains the quest's purpose, and creates atmosphere. Do NOT use generic descriptions like 'A quest to do something.' Instead, write compelling narrative text that draws the player into the world.");
            sb.AppendLine();
            sb.AppendLine("=== EXAMPLE VALID JSON ===");
            sb.AppendLine("{");
            sb.AppendLine("  \"title\": \"A Whisper in the Snow\",");
            sb.AppendLine("  \"description\": \"A rumor spreads of a threat stirring beyond the last warm lights of civilization. Ancient powers awaken, and only the brave dare venture into the frozen wastes where legends speak of a being that commands the very essence of winter itself.\",");
            sb.AppendLine("  \"tier\": \"Initiation\",");
            sb.AppendLine("  \"expiresMinutes\": 120,");
            sb.AppendLine("  \"waypoints\": [");
            sb.AppendLine("    {");
            sb.AppendLine("      \"type\": \"Origin\",");
            sb.AppendLine("      \"condition\": \"TalkToNPC\",");
            sb.AppendLine("      \"name\": \"Mentor Arlen\",");
            sb.AppendLine("      \"description\": \"Speak with the one who set this task in motion.\",");
            sb.AppendLine("      \"poiId\": \"IRONCLAD_BRITAIN_CENTER\",");
            sb.AppendLine("      \"npcDialogueContext\": \"You are Mentor Arlen, a wise guide. Offer the quest and tell the player: 'You must seek the counsel of Scout Lyra in Yew. She has been tracking strange disturbances in the northern forests. She will guide you on your way and reveal what she has discovered.'\",");
            sb.AppendLine("      \"playerLocationHint\": \"The ranger can be typically found in yew\"");
            sb.AppendLine("    },");
            sb.AppendLine("    {");
            sb.AppendLine("      \"type\": \"Waypoint\",");
            sb.AppendLine("      \"condition\": \"TalkToNPC\",");
            sb.AppendLine("      \"name\": \"Scout Lyra\",");
            sb.AppendLine("      \"description\": \"Find the scout who knows more.\",");
            sb.AppendLine("      \"poiId\": \"IRONCLAD_YEW_CENTER\",");
            sb.AppendLine("      \"npcDialogueContext\": \"You are Scout Lyra, a ranger. When the player approaches, say: 'I had heard that Mentor Arlen might be sending you my way. It is a dark time indeed. The frost has been spreading unnaturally, and I fear something ancient stirs. You must report to Captain Doran at the castle - he has been coordinating our defenses and will know what to do next.'\",");
            sb.AppendLine("      \"playerLocationHint\": \"near the throne of our Lord Britain\"");
            sb.AppendLine("    },");
            sb.AppendLine("    {");
            sb.AppendLine("      \"type\": \"Waypoint\",");
            sb.AppendLine("      \"condition\": \"TalkToNPC\",");
            sb.AppendLine("      \"name\": \"Captain Doran\",");
            sb.AppendLine("      \"description\": \"Report to the captain for final instructions.\",");
            sb.AppendLine("      \"poiId\": \"IRONCLAD_BRITAIN_CENTER\",");
            sb.AppendLine("      \"npcDialogueContext\": \"You are Captain Doran, a military leader. Say: 'Scout Lyra's reports have been troubling. The evil FrostFather has awakened on the island of ice. You must journey there and defeat this ancient terror. Beware, it will not be easy - the FrostFather is a being of immense power. May the light guide your blade.'\",");
            sb.AppendLine("      \"playerLocationHint\": \"on the island of ice, beware, it will not be easy\"");
            sb.AppendLine("    },");
            sb.AppendLine("    {");
            sb.AppendLine("      \"type\": \"BossCompletion\",");
            sb.AppendLine("      \"condition\": \"DefeatBoss\",");
            sb.AppendLine("      \"name\": \"End the threat\",");
            sb.AppendLine("      \"description\": \"Defeat the entity behind the winter terror.\",");
            sb.AppendLine("      \"poiId\": \"FROSTHOLD_ICELAND\",");
            sb.AppendLine("      \"bossTypeName\": \"FrostFather\",");
            sb.AppendLine("      \"npcDialogueContext\": \"\",");
            sb.AppendLine("      \"playerLocationHint\": \"on the island of ice, beware, it will not be easy\"");
            sb.AppendLine("    }");
            sb.AppendLine("  ]");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("=== INTERLINKED DIALOGUE REQUIREMENTS ===");
            sb.AppendLine("CRITICAL: NPCs must reference each other in their dialogue to create a cohesive narrative:");
            sb.AppendLine("1. Origin NPC should mention the NEXT NPC by name and guide the player to them.");
            sb.AppendLine("2. Middle NPCs should acknowledge PREVIOUS NPCs (e.g., 'I had heard that [PreviousNPC] might be sending you my way...') and guide to NEXT NPCs.");
            sb.AppendLine("3. Final NPC (before boss) should acknowledge the quest chain and prepare the player for the final confrontation.");
            sb.AppendLine("4. Boss/villain NPCs should be menacing and confident (e.g., 'You are to defeat me, hah! I will carve your soul...').");
            sb.AppendLine("5. Use the npcDialogueContext field to write dialogue that naturally references other NPCs in the quest chain.");
            sb.AppendLine();
            sb.AppendLine("=== OUTPUT FORMAT ===");
            sb.AppendLine("- Return ONLY the JSON object");
            sb.AppendLine("- NO markdown code blocks (no ```json or ```)");
            sb.AppendLine("- NO explanation text before or after");
            sb.AppendLine("- NO comments in the JSON");
            sb.AppendLine("- Ensure all brackets and braces are properly closed");
            sb.AppendLine("- Every waypoint must have ALL required fields");
            sb.AppendLine();
            sb.AppendLine("Now generate a quest plan following the example above:");
            
            return sb.ToString();
        }

        private static string ExtractJsonFromResponse(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                return null;

            string json = response.Trim();

            // Remove markdown code blocks if present
            if (json.StartsWith("```json"))
            {
                int start = json.IndexOf('\n') + 1;
                int end = json.LastIndexOf("```");
                if (end > start)
                    json = json.Substring(start, end - start).Trim();
            }
            else if (json.StartsWith("```"))
            {
                int start = json.IndexOf('\n') + 1;
                int end = json.LastIndexOf("```");
                if (end > start)
                    json = json.Substring(start, end - start).Trim();
            }

            // If the response was extracted from quotes by ExtractResponseContent,
            // it might be truncated. Look for the actual JSON start.
            // The JSON might be embedded in the string, so we need to find the first {
            int firstBrace = json.IndexOf('{');
            if (firstBrace < 0)
            {
                // No JSON found - maybe it's in a different format
                Console.WriteLine($"[LLMQuestGeneration] No JSON object found in response. Response preview: {json.Substring(0, Math.Min(200, json.Length))}");
                
                // Try to find JSON array start
                int firstBracket = json.IndexOf('[');
                if (firstBracket >= 0)
                {
                    Console.WriteLine($"[LLMQuestGeneration] Found JSON array instead of object, trying to extract...");
                    // For now, return null - we expect an object, not an array
                    return null;
                }
                
                return null;
            }

            // If there's text before the JSON, skip it
            if (firstBrace > 0)
            {
                json = json.Substring(firstBrace);
            }

            // Now find the complete JSON object by matching braces
            // Stop at the first complete JSON object to avoid API metadata
            int braceCount = 0;
            int bracketCount = 0;
            int lastBrace = -1;
            bool inString = false;
            char stringChar = '\0';
            bool escaped = false;

            for (int i = 0; i < json.Length; i++)
            {
                char c = json[i];

                if (escaped)
                {
                    escaped = false;
                    continue;
                }

                if (c == '\\')
                {
                    escaped = true;
                    continue;
                }

                if (!inString)
                {
                    if (c == '"' || c == '\'')
                    {
                        inString = true;
                        stringChar = c;
                    }
                    else if (c == '{')
                    {
                        braceCount++;
                    }
                    else if (c == '}')
                    {
                        braceCount--;
                        if (braceCount == 0 && bracketCount == 0)
                        {
                            // Found the end of the root JSON object
                            lastBrace = i;
                            break;
                        }
                    }
                    else if (c == '[')
                    {
                        bracketCount++;
                    }
                    else if (c == ']')
                    {
                        bracketCount--;
                    }
                }
                else
                {
                    if (c == stringChar)
                    {
                        inString = false;
                    }
                }
            }

            if (lastBrace > 0)
            {
                // Extract only the complete JSON object, ignore anything after
                json = json.Substring(0, lastBrace + 1);
                Console.WriteLine($"[LLMQuestGeneration] Extracted complete JSON object (length: {json.Length})");
            }
            else
            {
                // JSON is incomplete - try to close it
                Console.WriteLine($"[LLMQuestGeneration] Warning: JSON appears incomplete. Attempting to fix...");
                json = TryFixIncompleteJson(json);
            }

            return json;
        }

        private static string TryFixIncompleteJson(string partialJson)
        {
            // Count unclosed brackets/braces
            int openBraces = 0;
            int openBrackets = 0;
            bool inString = false;
            char stringChar = '\0';

            for (int i = 0; i < partialJson.Length; i++)
            {
                char c = partialJson[i];
                
                if (!inString)
                {
                    if (c == '"' || c == '\'')
                    {
                        inString = true;
                        stringChar = c;
                    }
                    else if (c == '{')
                        openBraces++;
                    else if (c == '}')
                        openBraces--;
                    else if (c == '[')
                        openBrackets++;
                    else if (c == ']')
                        openBrackets--;
                }
                else
                {
                    if (c == stringChar && (i == 0 || partialJson[i - 1] != '\\'))
                        inString = false;
                }
            }

            // Close unclosed structures
            var fixedJson = new StringBuilder(partialJson);
            while (openBrackets > 0)
            {
                fixedJson.Append(']');
                openBrackets--;
            }
            while (openBraces > 0)
            {
                fixedJson.Append('}');
                openBraces--;
            }

            return fixedJson.ToString();
        }

        /// <summary>
        /// Placeholder: future planner will call UnifiedLLMService to generate JSON.
        /// For now, this returns a tiny working plan so the pipeline can be tested end-to-end.
        /// </summary>
        public static string BuildDemoPlanJson(string poiId)
        {
            // NOTE: This is intentionally small; real generation will come from the LLM.
            var plan = new DynamicQuestPlan
            {
                Title = "A Whisper in the Snow",
                Description = "A rumor spreads of a threat stirring beyond the last warm lights of civilization.",
                Tier = "Initiation",
                ExpiresMinutes = 60,
            };

            plan.Waypoints.Add(new DynamicQuestWaypointPlan
            {
                Type = "Origin",
                Condition = "TalkToNPC",
                Name = "Mentor",
                Description = "Speak with the one who set this task in motion.",
                PoiId = poiId,
                NpcDialogueContext = "Offer the quest and point the adventurer toward the next contact.",
                PlayerLocationHint = "The ranger can be typically found in yew",
            });

            plan.Waypoints.Add(new DynamicQuestWaypointPlan
            {
                Type = "BossCompletion",
                Condition = "DefeatBoss",
                Name = "End the threat",
                Description = "Defeat the entity behind the winter terror.",
                PoiId = poiId,
                BossTypeName = "FrostFather",
                NpcDialogueContext = "Warn the adventurer of the danger ahead.",
                PlayerLocationHint = "The evil FrostFather is on the island of ice, beware, it will not be easy",
            });

            return DynamicQuestPlanJson.Serialize(plan);
        }
    }
}


