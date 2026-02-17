using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Services.LLM;
using Server.Custom.VystiaClasses.Quests.Generation;

namespace Server.Custom.VystiaClasses.Quests
{
    /// <summary>
    /// NPC that is linked to a quest waypoint and provides LLM-powered dialogue
    /// </summary>
    public class QuestNPC : BaseCreature, ILLMConversational
    {
        private int m_QuestID;
        private int m_WaypointID;
        private string m_QuestContext;
        private string m_NPCTitle;
        private string m_NpcTemplateId; // Store template ID to check for faction leader status

        // ILLMConversational properties
        private bool m_LLMConversationEnabled = true;
        private NPCPersonalities.PersonalityType m_PersonalityType = NPCPersonalities.PersonalityType.Sage;
        private NPCPersonalities.SpeechPattern m_SpeechPattern = NPCPersonalities.SpeechPattern.Formal;
        private int m_HearingRange = 6;

        #region Properties

        [CommandProperty(AccessLevel.GameMaster)]
        public int QuestID
        {
            get => m_QuestID;
            set { m_QuestID = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int WaypointID
        {
            get => m_WaypointID;
            set { m_WaypointID = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string QuestContext
        {
            get => m_QuestContext;
            set => m_QuestContext = value;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string NPCTitle
        {
            get => m_NPCTitle;
            set { m_NPCTitle = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string NpcTemplateId
        {
            get => m_NpcTemplateId;
            set => m_NpcTemplateId = value;
        }

        // ILLMConversational interface properties
        [CommandProperty(AccessLevel.GameMaster)]
        public bool LLMConversationEnabled
        {
            get => m_LLMConversationEnabled;
            set => m_LLMConversationEnabled = value;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public NPCPersonalities.PersonalityType PersonalityType
        {
            get => m_PersonalityType;
            set => m_PersonalityType = value;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public NPCPersonalities.SpeechPattern SpeechPattern
        {
            get => m_SpeechPattern;
            set => m_SpeechPattern = value;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HearingRange
        {
            get => m_HearingRange;
            set => m_HearingRange = value;
        }

        #endregion

        [Constructable]
        public QuestNPC() : this("Quest Giver", "the Quest NPC")
        {
        }

        [Constructable]
        public QuestNPC(string name, string title) : base(AIType.AI_Vendor, FightMode.None, 10, 1, 0.2, 0.4)
        {
            Name = name;
            m_NPCTitle = title;
            Title = title;

            Body = Utility.RandomBool() ? 0x190 : 0x191; // Human male or female
            Hue = Utility.RandomSkinHue();

            SetStr(50);
            SetDex(50);
            SetInt(100);

            SetHits(100);
            SetMana(0);

            Fame = 0;
            Karma = 0;

            CantWalk = true;

            SetupDefaultAppearance();
        }

        private void SetupDefaultAppearance()
        {
            // Basic clothing
            AddItem(new Robe(Utility.RandomNeutralHue()));
            AddItem(new Sandals());

            // Random hair
            HairItemID = Utility.RandomList(0x203B, 0x203C, 0x203D);
            HairHue = Utility.RandomHairHue();
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            if (m_QuestID > 0)
            {
                var quest = GetLinkedQuest();
                if (quest != null)
                {
                    list.Add(1060658, $"Quest\t{quest.Title}"); // ~1_val~: ~2_val~
                }
            }

            if (m_WaypointID > 0)
            {
                var waypoint = GetLinkedWaypoint();
                if (waypoint != null)
                {
                    list.Add(1060659, $"Role\t{waypoint.Type}"); // ~1_val~: ~2_val~
                }
            }
        }

        #region Quest Integration

        public DynamicQuest GetLinkedQuest()
        {
            if (m_QuestID <= 0)
                return null;

            return DynamicQuestManager.GetQuest(m_QuestID);
        }

        public QuestWaypoint GetLinkedWaypoint()
        {
            var quest = GetLinkedQuest();
            if (quest == null || m_WaypointID <= 0)
                return null;

            return quest.GetWaypoint(m_WaypointID);
        }

        /// <summary>
        /// Link this NPC to a specific quest waypoint
        /// </summary>
        public void LinkToWaypoint(int questId, int waypointId)
        {
            m_QuestID = questId;
            m_WaypointID = waypointId;

            var waypoint = GetLinkedWaypoint();
            if (waypoint != null)
            {
                waypoint.AssignedNPCSerial = this.Serial;
                waypoint.Location = this.Location;
                waypoint.Map = this.Map;

                // Set dialogue context from waypoint if available
                if (!string.IsNullOrEmpty(waypoint.NPCDialogueContext))
                    m_QuestContext = waypoint.NPCDialogueContext;

                // Set personality from waypoint if available
                if (!string.IsNullOrEmpty(waypoint.LLMPersonality))
                {
                    if (Enum.TryParse<NPCPersonalities.PersonalityType>(waypoint.LLMPersonality, true, out var personality))
                        m_PersonalityType = personality;
                }

                if (!string.IsNullOrEmpty(waypoint.LLMSpeechPattern))
                {
                    if (Enum.TryParse<NPCPersonalities.SpeechPattern>(waypoint.LLMSpeechPattern, true, out var pattern))
                        m_SpeechPattern = pattern;
                }
            }

            InvalidateProperties();
        }

        /// <summary>
        /// Build context string for LLM based on quest and player progress
        /// </summary>
        public string BuildQuestAwareContext(PlayerMobile pm)
        {
            var quest = GetLinkedQuest();
            var waypoint = GetLinkedWaypoint();

            if (quest == null)
                return m_QuestContext ?? "";

            string context = $"You are {Name}";
            if (!string.IsNullOrEmpty(m_NPCTitle))
                context += $", {m_NPCTitle}";
            context += ".\n";

            // Check if this NPC is a faction leader
            bool isFactionLeader = !string.IsNullOrEmpty(m_NpcTemplateId) && 
                                   m_NpcTemplateId.StartsWith("FACTION_LEADER_", StringComparison.OrdinalIgnoreCase);
            
            if (isFactionLeader)
            {
                context += "IMPORTANT: You are a FACTION LEADER - a powerful and influential figure in Vystia.\n";
                context += "You speak with authority, gravitas, and the weight of leadership.\n";
                context += "Your words carry the weight of your position and the responsibility you bear for your people.\n";
                context += "You are not a common NPC - you are a leader who commands respect.\n";
                
                // Add faction-specific context if we can get it from the template
                if (VystiaNpcTemplateRegistry.TryGet(m_NpcTemplateId, out var template))
                {
                    if (!string.IsNullOrEmpty(template.Title))
                        context += $"Your official title is: {template.Title}\n";
                    if (template.KnowledgeTags != null && template.KnowledgeTags.Count > 0)
                    {
                        context += $"You are associated with: {string.Join(", ", template.KnowledgeTags)}\n";
                    }
                }
            }

            context += $"You are assigned to the quest \"{quest.Title}\".\n";

            if (waypoint != null)
            {
                context += $"This is a {waypoint.Type} waypoint";
                if (!string.IsNullOrEmpty(waypoint.Name))
                    context += $" named \"{waypoint.Name}\"";
                context += ".\n";

                if (!string.IsNullOrEmpty(waypoint.Description))
                    context += $"Waypoint purpose: {waypoint.Description}\n";

                // Per-waypoint LLM guidance (this is the "what should I say at THIS step" field in the wizard).
                if (!string.IsNullOrEmpty(waypoint.NPCDialogueContext))
                {
                    context += "\n=== YOUR PRIMARY DIALOGUE (MOST IMPORTANT) ===\n";
                    context += "The following dialogue context tells you EXACTLY what to say. Extract the actual spoken words and say them naturally:\n";
                    context += waypoint.NPCDialogueContext + "\n";
                    context += "\nCRITICAL RULES:\n";
                    context += "1. The dialogue context above contains the ACTUAL WORDS you should speak (paraphrased naturally, not verbatim).\n";
                    context += "2. If it says 'Offer the quest and say: [text]', then you should offer the quest and say something like that text.\n";
                    context += "3. If it says 'When approached, say: [text]', then say something like that text when the player approaches.\n";
                    context += "4. NEVER repeat instruction phrases like 'You are [name]' or 'When the player approaches, say:' - these are instructions, not dialogue.\n";
                    context += "5. Extract the actual dialogue content (the quoted text or the text after 'say:') and speak it naturally as your character.\n";
                    context += "6. Your response should match the tone and content of the dialogue context above - this is your PRIMARY source of what to say.\n";
                }
            }

            // Player quest status
            if (pm != null)
            {
                bool hasStarted = VystiaQuestSystem.HasActiveQuest(pm, m_QuestID);
                bool hasCompleted = VystiaQuestSystem.HasCompletedQuest(pm, m_QuestID);

                if (hasCompleted)
                {
                    context += "The player has COMPLETED this quest.\n";
                }
                else if (hasStarted)
                {
                    context += "The player is currently ON this quest.\n";

                    // Show current waypoint progress
                    var tracker = VystiaQuestTracker.GetTracker(pm);
                    if (tracker != null)
                    {
                        var progress = tracker.GetProgress(m_QuestID);
                        var currentWp = quest.GetCurrentWaypoint(progress);
                        if (currentWp != null)
                        {
                            context += $"Current objective: {currentWp.Name}\n";

                            // Provide two forms of location guidance for the CURRENT objective:
                            //  1) In-character direction + distance (LLM style: "east of here, a short walk")
                            //  2) GM-authored player hint (freeform), e.g. "near the entrance of Destard"
                            context += BuildLocationHintsForWaypoint(currentWp);
                            context += "\n=== MANDATORY: PROVIDE CONCRETE DIRECTIONS ===\n";
                            context += "CRITICAL RULE: When the player asks ANY location question ('where', 'where exactly', 'how do I find', etc.), you MUST:\n";
                            context += "1. IMMEDIATELY use the location information provided above\n";
                            context += "2. NEVER say 'I can't provide precise directions' - you HAVE the information, PROVIDE IT\n";
                            context += "3. NEVER give vague descriptions - ALWAYS include direction + distance + coordinates/landmark\n";
                            context += "4. If the location hints show coordinates, you MUST include them in your response\n";
                            context += "5. Format: '[NPC] can be found [direction] of here, approximately [distance] tiles away, [landmark]. Coordinates: ([X], [Y]).'\n";
                            context += "\n";
                            context += "EXAMPLE (use the actual values from the location hints above):\n";
                            if (currentWp.Location != Point3D.Zero && currentWp.Map != null && Map == currentWp.Map)
                            {
                                int dx = currentWp.Location.X - Location.X;
                                int dy = currentWp.Location.Y - Location.Y;
                                int dist = Math.Max(Math.Abs(dx), Math.Abs(dy));
                                string dir = GetCompassDirection(dx, dy);
                                string landmark = !string.IsNullOrWhiteSpace(currentWp.PlayerLocationHint) ? currentWp.PlayerLocationHint : "in that area";
                                context += $"  '{currentWp.Name} is located {dir} of here, approximately {dist} tiles away, {landmark}. The exact coordinates are ({currentWp.Location.X}, {currentWp.Location.Y}).'\n";
                            }
                            context += "\n";
                            context += "DO NOT USE vague phrases like:\n";
                            context += "  - 'I'm afraid I can't provide precise directions'\n";
                            context += "  - 'Follow the winding paths'\n";
                            context += "  - 'Trust your instincts'\n";
                            context += "  - 'The trees will guide you'\n";
                            context += "These are NOT helpful - players need SPECIFIC, ACTIONABLE information.\n";
                        }
                    }
                }
                else
                {
                    context += "The player has NOT started this quest.\n";

                    if (waypoint?.Type == WaypointType.Origin)
                    {
                        context += "You should offer the quest to them.\n";
                        
                        // Provide location hints for the first waypoint (next step after accepting)
                        if (quest.Waypoints.Count > 1)
                        {
                            var nextWaypoint = quest.Waypoints.OrderBy(w => w.OrderIndex).Skip(1).FirstOrDefault();
                            if (nextWaypoint != null)
                            {
                                context += BuildLocationHintsForWaypoint(nextWaypoint);
                                context += "\nWhen offering the quest, naturally guide the player to where they should go next using the location hints above.\n";
                            }
                        }
                    }
                }
            }

            // Add information about other NPCs in the quest chain for interlinked dialogue
            context += BuildQuestChainContext(quest, waypoint);

            // Add custom context if any
            if (!string.IsNullOrEmpty(m_QuestContext))
                context += $"\nAdditional context: {m_QuestContext}\n";

            // Quest description
            context += $"\nQuest description: {quest.Description}\n";

            return context;
        }

        /// <summary>
        /// Builds context about other NPCs in the quest chain for interlinked dialogue
        /// </summary>
        private string BuildQuestChainContext(DynamicQuest quest, QuestWaypoint currentWaypoint)
        {
            if (quest == null || currentWaypoint == null)
                return "";

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("\n=== QUEST CHAIN CONTEXT (FOR INTERLINKED DIALOGUE) ===");
            
            // Get all waypoints in order
            var allWaypoints = quest.Waypoints.OrderBy(w => w.OrderIndex).ToList();
            int currentIndex = allWaypoints.IndexOf(currentWaypoint);
            
            if (currentIndex < 0)
                return "";

            // Find previous NPCs (who might have sent the player here)
            var previousNPCs = new List<QuestNPCInfo>();
            for (int i = 0; i < currentIndex; i++)
            {
                var prevWp = allWaypoints[i];
                if (prevWp.Condition == WaypointCondition.TalkToNPC || 
                    prevWp.Type == WaypointType.Origin || 
                    prevWp.Type == WaypointType.NPCCompletion)
                {
                    var npc = GetNPCFromWaypoint(prevWp);
                    if (npc != null)
                    {
                        previousNPCs.Add(new QuestNPCInfo
                        {
                            Name = npc.Name,
                            Title = npc.NPCTitle ?? prevWp.Name,
                            WaypointName = prevWp.Name,
                            WaypointDescription = prevWp.Description,
                            DialogueContext = prevWp.NPCDialogueContext
                        });
                    }
                }
            }

            // Find next NPCs (who this NPC should direct the player to)
            var nextNPCs = new List<QuestNPCInfo>();
            for (int i = currentIndex + 1; i < allWaypoints.Count; i++)
            {
                var nextWp = allWaypoints[i];
                if (nextWp.Condition == WaypointCondition.TalkToNPC || 
                    nextWp.Type == WaypointType.NPCCompletion)
                {
                    var npc = GetNPCFromWaypoint(nextWp);
                    if (npc != null)
                    {
                        nextNPCs.Add(new QuestNPCInfo
                        {
                            Name = npc.Name,
                            Title = npc.NPCTitle ?? nextWp.Name,
                            WaypointName = nextWp.Name,
                            WaypointDescription = nextWp.Description,
                            DialogueContext = nextWp.NPCDialogueContext
                        });
                    }
                }
            }

            // Find boss/final enemy
            QuestWaypoint bossWaypoint = null;
            foreach (var wp in allWaypoints)
            {
                if (wp.Condition == WaypointCondition.DefeatBoss && wp.OrderIndex > currentIndex)
                {
                    bossWaypoint = wp;
                    break;
                }
            }

            // Build context for previous NPCs
            if (previousNPCs.Count > 0)
            {
                sb.AppendLine("\nPREVIOUS NPCs IN THE QUEST CHAIN (who may have sent the player to you):");
                foreach (var npcInfo in previousNPCs)
                {
                    sb.AppendLine($"- {npcInfo.Name} ({npcInfo.Title}): {npcInfo.WaypointName}");
                    if (!string.IsNullOrEmpty(npcInfo.DialogueContext))
                        sb.AppendLine($"  Context: {npcInfo.DialogueContext}");
                }
                sb.AppendLine("You may reference these NPCs naturally, e.g., 'I had heard that [NPC1] might be sending you my way...'");
            }

            // Build context for next NPCs
            if (nextNPCs.Count > 0)
            {
                sb.AppendLine("\nNEXT NPCs IN THE QUEST CHAIN (who you should direct the player to):");
                foreach (var npcInfo in nextNPCs)
                {
                    sb.AppendLine($"- {npcInfo.Name} ({npcInfo.Title}): {npcInfo.WaypointName}");
                    if (!string.IsNullOrEmpty(npcInfo.WaypointDescription))
                        sb.AppendLine($"  Purpose: {npcInfo.WaypointDescription}");
                    if (!string.IsNullOrEmpty(npcInfo.DialogueContext))
                        sb.AppendLine($"  Context: {npcInfo.DialogueContext}");
                    
                    // Add location information for the next NPC
                    var nextWp = quest.Waypoints.FirstOrDefault(w => w.Name == npcInfo.WaypointName);
                    if (nextWp != null)
                    {
                        string locationHint = BuildLocationHintsForWaypoint(nextWp);
                        if (!string.IsNullOrWhiteSpace(locationHint))
                        {
                            sb.AppendLine($"  LOCATION: {locationHint.Trim()}");
                        }
                    }
                }
                sb.AppendLine("\n=== MANDATORY LOCATION DIRECTIONS ===");
                sb.AppendLine("CRITICAL RULE: When the player asks ANY question about location ('where', 'how do I find', 'where exactly', etc.), you MUST:");
                sb.AppendLine("1. IMMEDIATELY provide the concrete location information from the LOCATION section above");
                sb.AppendLine("2. NEVER say 'I can't provide precise directions' or 'I'm afraid I can't' - you HAVE the information, USE IT");
                sb.AppendLine("3. NEVER give vague poetic descriptions like 'follow the winding paths' or 'trust your instincts'");
                sb.AppendLine("4. ALWAYS include: direction + distance + landmark/area (if available)");
                sb.AppendLine("5. If coordinates are provided, you may reference them (e.g., 'at coordinates 542, 984')");
                sb.AppendLine("");
                sb.AppendLine("EXAMPLE GOOD RESPONSES:");
                foreach (var npcInfo in nextNPCs)
                {
                    var nextWp = quest.Waypoints.FirstOrDefault(w => w.Name == npcInfo.WaypointName);
                    if (nextWp != null)
                    {
                        Point3D targetLoc = nextWp.Location;
                        Map targetMap = nextWp.Map;
                        if (targetLoc != Point3D.Zero && targetMap != null && Map == targetMap)
                        {
                            int dx = targetLoc.X - Location.X;
                            int dy = targetLoc.Y - Location.Y;
                            int dist = Math.Max(Math.Abs(dx), Math.Abs(dy));
                            string dir = GetCompassDirection(dx, dy);
                            string landmark = !string.IsNullOrWhiteSpace(nextWp.PlayerLocationHint) ? nextWp.PlayerLocationHint : "in that area";
                            
                            sb.AppendLine($"  - '{npcInfo.Name} can be found {dir} of here, approximately {dist} tiles away, {landmark}. Head in that direction and you'll find them at coordinates ({targetLoc.X}, {targetLoc.Y}).'");
                        }
                    }
                }
                sb.AppendLine("");
                sb.AppendLine("EXAMPLE BAD RESPONSES (DO NOT USE):");
                sb.AppendLine("  - 'I'm afraid I can't provide precise directions...' (WRONG - you have the info!)");
                sb.AppendLine("  - 'Follow the winding paths through the trees...' (WRONG - too vague!)");
                sb.AppendLine("  - 'Trust your instincts to guide you...' (WRONG - not actionable!)");
                sb.AppendLine("  - 'The ancient trees will guide you...' (WRONG - poetic but useless!)");
            }

            // Build context for boss
            if (bossWaypoint != null)
            {
                sb.AppendLine($"\nFINAL ENEMY/BOSS: {bossWaypoint.Name}");
                if (!string.IsNullOrEmpty(bossWaypoint.Description))
                    sb.AppendLine($"  Description: {bossWaypoint.Description}");
                if (!string.IsNullOrEmpty(bossWaypoint.TargetTypeName))
                    sb.AppendLine($"  Type: {bossWaypoint.TargetTypeName}");
                sb.AppendLine("If you are the boss or a villain, you may taunt the player, e.g., 'You are to defeat me, hah! I will carve your soul and store it in my lifestone forever!'");
            }

            sb.AppendLine("\nDIALOGUE STYLE:");
            sb.AppendLine("- Reference other NPCs naturally in your dialogue to create a cohesive narrative.");
            sb.AppendLine("- If previous NPCs exist, acknowledge that they may have sent the player.");
            sb.AppendLine("- If next NPCs exist, guide the player toward them with purpose.");
            sb.AppendLine("- If you are a boss/villain, be menacing and confident.");

            return sb.ToString();
        }

        private QuestNPC GetNPCFromWaypoint(QuestWaypoint wp)
        {
            if (wp == null || wp.AssignedNPCSerial.Value == -1)
                return null;

            var mobile = World.FindMobile(wp.AssignedNPCSerial);
            return mobile as QuestNPC;
        }

        private class QuestNPCInfo
        {
            public string Name { get; set; }
            public string Title { get; set; }
            public string WaypointName { get; set; }
            public string WaypointDescription { get; set; }
            public string DialogueContext { get; set; }
        }

        private string BuildLocationHintsForWaypoint(QuestWaypoint wp)
        {
            if (wp == null)
                return "";

            Point3D targetLoc = wp.Location;
            Map targetMap = wp.Map;

            if ((targetLoc == Point3D.Zero || targetMap == null) && wp.AssignedNPCSerial.Value != -1)
            {
                Mobile m = World.FindMobile(wp.AssignedNPCSerial);
                if (m != null && !m.Deleted)
                {
                    targetLoc = m.Location;
                    targetMap = m.Map;
                }
            }

            bool hasPlayerHint = !string.IsNullOrWhiteSpace(wp.PlayerLocationHint);
            bool canComputeDirectional = !(targetLoc == Point3D.Zero || targetMap == null || Map != targetMap);

            if (!hasPlayerHint && !canComputeDirectional)
                return "";

            string hint = "\n=== CONCRETE LOCATION INFORMATION ===\n";

            if (canComputeDirectional)
            {
                int dx = targetLoc.X - Location.X;
                int dy = targetLoc.Y - Location.Y;
                int dist = Math.Max(Math.Abs(dx), Math.Abs(dy));

                string dir = GetCompassDirection(dx, dy);

                // Provide specific, actionable directions
                if (dist <= 60)
                {
                    hint += $"DIRECTION: {dir} of here, a short walk (~{dist} tiles).\n";
                }
                else if (dist <= 200)
                {
                    hint += $"DIRECTION: {dir} of here, a fair walk (~{dist} tiles).\n";
                }
                else if (dist <= 500)
                {
                    hint += $"DIRECTION: {dir} of here, a moderate journey (~{dist} tiles).\n";
                }
                else
                {
                    hint += $"DIRECTION: {dir} of here, a long journey (~{dist} tiles).\n";
                }
                
                // Add coordinates for very specific guidance
                hint += $"COORDINATES: The target is at approximately ({targetLoc.X}, {targetLoc.Y}) on {targetMap?.Name ?? "this map"}.\n";
            }

            if (hasPlayerHint)
            {
                hint += $"LANDMARK/AREA: {wp.PlayerLocationHint}\n";
            }
            
            hint += "\nUSE THIS INFORMATION: When giving directions, combine the direction, distance, and landmark into a clear, actionable instruction.\n";
            return hint;
        }

        private static string GetCompassDirection(int dx, int dy)
        {
            if (dx == 0 && dy == 0)
                return "right here";

            // UO coordinate system: Y increases to the south.
            bool east = dx > 0;
            bool west = dx < 0;
            bool south = dy > 0;
            bool north = dy < 0;

            int ax = Math.Abs(dx);
            int ay = Math.Abs(dy);

            // Strong axis bias -> cardinal direction
            if (ax >= (ay * 2))
                return east ? "east" : "west";

            if (ay >= (ax * 2))
                return south ? "south" : "north";

            // Diagonal
            if (north && east) return "northeast";
            if (north && west) return "northwest";
            if (south && east) return "southeast";
            if (south && west) return "southwest";

            // Fallback
            return east ? "east" : west ? "west" : south ? "south" : "north";
        }

        #endregion

        #region Speech Handling

        public override void OnSpeech(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;
            if (!(from is PlayerMobile pm))
                return;

            if (!from.InRange(this, HearingRange))
                return;

            string speech = e.Speech.ToLower();

            // If this NPC is the CURRENT TalkToNPC waypoint, any interaction should advance it (not only "hello").
            // This makes waypoints like "Report to Captain Doran" reliably complete.
            if (VystiaQuestSystem.HasActiveQuest(pm, m_QuestID))
            {
                var linkedWp = GetLinkedWaypoint();
                if (linkedWp != null && linkedWp.Condition == WaypointCondition.TalkToNPC && linkedWp.Type == WaypointType.Waypoint)
                {
                    var current = VystiaQuestSystem.GetCurrentWaypoint(pm, m_QuestID);
                    if (current != null && current.WaypointID == linkedWp.WaypointID)
                    {
                        VystiaQuestSystem.CompleteWaypoint(pm, m_QuestID, linkedWp.WaypointID);
                        // Do NOT return here — let the LLM respond using the updated quest context
                        // so the NPC can tell the player the next steps in-character.
                    }
                }
            }

            // Handle quest-specific keywords FIRST (before BaseCreature/LLM can claim the speech)
            if (HandleQuestKeywords(pm, speech))
            {
                e.Handled = true;
                return;
            }

            base.OnSpeech(e);

            if (e.Handled)
                return;

            // Let LLM handle if enabled
            if (m_LLMConversationEnabled && ShouldHandleConversation(e))
            {
                HandleConversation(e);
            }
        }

        private bool HandleQuestKeywords(PlayerMobile pm, string speech)
        {
            var quest = GetLinkedQuest();
            var waypoint = GetLinkedWaypoint();

            if (quest == null)
                return false;

            bool hasStarted = VystiaQuestSystem.HasActiveQuest(pm, m_QuestID);
            bool hasCompleted = VystiaQuestSystem.HasCompletedQuest(pm, m_QuestID);

            // Accept quest keywords
            if (speech.Contains("accept") || speech.Contains("yes") || speech.Contains("begin"))
            {
                if (waypoint?.Type == WaypointType.Origin && !hasStarted && !hasCompleted)
                {
                    // Start quest and show a clear UI confirmation
                    bool started = VystiaQuestSystem.StartQuest(pm, m_QuestID);
                    if (started)
                    {
                        Say($"Excellent! You have begun: {quest.Title}");
                        CompleteWaypointIfTalkCondition(pm);
                        pm.SendGump(new Server.Custom.VystiaClasses.Gumps.VystiaQuestOfferGump(pm, m_QuestID, true));
                        return true;
                    }

                    // If it failed (class/prereq/etc.), still show the offer gump so the player sees requirements/errors.
                    pm.SendGump(new Server.Custom.VystiaClasses.Gumps.VystiaQuestOfferGump(pm, m_QuestID, false));
                    return true;
                }
            }

            // Complete quest keywords
            if (speech.Contains("complete") || speech.Contains("done") || speech.Contains("finished"))
            {
                if (waypoint?.Type == WaypointType.NPCCompletion && hasStarted)
                {
                    if (VystiaQuestSystem.CompleteQuest(pm, m_QuestID))
                    {
                        Say("Well done! You have completed the quest. Here is your reward.");
                        return true;
                    }
                    else
                    {
                        Say("You have not yet completed all objectives. Return when you are ready.");
                        return true;
                    }
                }
            }

            // Quest status keywords
            if (speech.Contains("quest") || speech.Contains("task") || speech.Contains("job"))
            {
                if (hasCompleted)
                {
                    Say("You have already completed this quest. Thank you for your service.");
                    return true;
                }
                else if (hasStarted)
                {
                    var tracker = VystiaQuestTracker.GetTracker(pm);
                    if (tracker != null)
                    {
                        var progress = tracker.GetProgress(m_QuestID);
                        var currentWp = quest.GetCurrentWaypoint(progress);
                        if (currentWp != null)
                        {
                            Say($"Your current objective is: {currentWp.Name}");
                            if (!string.IsNullOrEmpty(currentWp.Description))
                                Say(currentWp.Description);
                        }
                    }
                    return true;
                }
                else if (waypoint?.Type == WaypointType.Origin)
                {
                    // Offer quest via gump (more reliable than relying on LLM text)
                    pm.SendGump(new Server.Custom.VystiaClasses.Gumps.VystiaQuestOfferGump(pm, m_QuestID, false));
                    return true;
                }
            }

            // Greetings - complete waypoint if appropriate
            if (speech.Contains("hello") || speech.Contains("hail") || speech.Contains("greetings"))
            {
                if (hasStarted && waypoint != null)
                {
                    CompleteWaypointIfTalkCondition(pm);
                }
            }

            return false;
        }

        /// <summary>
        /// Complete this waypoint if the condition is TalkToNPC
        /// </summary>
        private void CompleteWaypointIfTalkCondition(PlayerMobile pm)
        {
            var waypoint = GetLinkedWaypoint();
            if (waypoint == null || waypoint.Condition != WaypointCondition.TalkToNPC)
                return;

            if (!VystiaQuestSystem.HasActiveQuest(pm, m_QuestID))
                return;

            VystiaQuestSystem.CompleteWaypoint(pm, m_QuestID, waypoint.WaypointID);
        }

        #endregion

        #region ILLMConversational Implementation

        public bool ShouldHandleConversation(SpeechEventArgs e)
        {
            if (!m_LLMConversationEnabled)
                return false;

            Mobile from = e.Mobile;
            return from.InRange(this, HearingRange) && from.Player;
        }

        public void HandleConversation(SpeechEventArgs e)
        {
            if (!(e.Mobile is PlayerMobile pm))
                return;

            // Build quest-aware context and pass to LLM helper
            string context = BuildQuestAwareContext(pm);

            // TODO: Integrate with LLMConversationHelper when it supports custom context
            // For now, use the helper directly
            LLMConversationHelper.ProcessConversation(this, e.Mobile, e.Speech);

            e.Handled = true;
        }

        #endregion

        #region Proximity Auto-Speak

        private DateTime m_LastProximityCheck = DateTime.MinValue;
        private HashSet<Serial> m_PlayersInRange = new HashSet<Serial>();

        public override void OnThink()
        {
            base.OnThink();

            // Check proximity every 2 seconds
            if (DateTime.UtcNow - m_LastProximityCheck < TimeSpan.FromSeconds(2))
                return;

            m_LastProximityCheck = DateTime.UtcNow;

            // Check for players within 10 tiles
            var nearbyPlayers = new List<PlayerMobile>();
            foreach (var m in GetMobilesInRange(10))
            {
                if (m is PlayerMobile pm && !pm.Deleted && pm.Alive)
                {
                    nearbyPlayers.Add(pm);
                }
            }

            // Auto-speak to players who just entered range
            foreach (var pm in nearbyPlayers)
            {
                if (!m_PlayersInRange.Contains(pm.Serial.Value))
                {
                    // Player just entered range - trigger auto-speak
                    m_PlayersInRange.Add(pm.Serial.Value);
                    TriggerAutoSpeak(pm);
                }
            }

            // Remove players who left range
            m_PlayersInRange.RemoveWhere(serial =>
            {
                var m = World.FindMobile(serial);
                return m == null || m.Deleted || !m.InRange(this, 10);
            });
        }

        private void TriggerAutoSpeak(PlayerMobile pm)
        {
            if (pm == null || pm.Deleted || !pm.InRange(this, 10))
                return;

            // Only auto-speak if player has the quest active or is at the origin waypoint
            var quest = GetLinkedQuest();
            var waypoint = GetLinkedWaypoint();
            
            if (quest == null || waypoint == null)
                return;

            bool hasQuest = VystiaQuestSystem.HasActiveQuest(pm, m_QuestID);
            bool isOrigin = waypoint.Type == WaypointType.Origin && !hasQuest;

            if (hasQuest || isOrigin)
            {
                // Build a greeting message based on quest context
                string greeting = BuildAutoSpeakGreeting(pm, hasQuest);
                if (!string.IsNullOrEmpty(greeting))
                {
                    Say(greeting);
                }
            }
        }

        private string BuildAutoSpeakGreeting(PlayerMobile pm, bool hasQuest)
        {
            var waypoint = GetLinkedWaypoint();
            if (waypoint == null)
                return null;

            if (!hasQuest && waypoint.Type == WaypointType.Origin)
            {
                // Offer quest
                return $"Greetings, {pm.Name}. I have a task that may interest you.";
            }
            else if (hasQuest)
            {
                // Provide guidance based on waypoint context
                if (!string.IsNullOrEmpty(waypoint.NPCDialogueContext))
                {
                    // Use a short version of the dialogue context
                    return waypoint.NPCDialogueContext.Length > 100 
                        ? waypoint.NPCDialogueContext.Substring(0, 100) + "..." 
                        : waypoint.NPCDialogueContext;
                }
                else
                {
                    return $"Ah, {pm.Name}, you're on the right path. Continue your quest.";
                }
            }

            return null;
        }

        #endregion

        #region Serialization

        public QuestNPC(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)2); // version

            // Version 0
            writer.Write(m_QuestID);
            writer.Write(m_WaypointID);
            writer.Write(m_QuestContext ?? "");
            writer.Write(m_NPCTitle ?? "");

            // Version 1 - LLM properties
            writer.Write(m_LLMConversationEnabled);
            writer.Write((int)m_PersonalityType);
            writer.Write((int)m_SpeechPattern);
            writer.Write(m_HearingRange);
            
            // Version 2 - NPC Template ID
            writer.Write(m_NpcTemplateId ?? "");
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_QuestID = reader.ReadInt();
            m_WaypointID = reader.ReadInt();
            m_QuestContext = reader.ReadString();
            m_NPCTitle = reader.ReadString();

            if (version >= 1)
            {
                m_LLMConversationEnabled = reader.ReadBool();
                m_PersonalityType = (NPCPersonalities.PersonalityType)reader.ReadInt();
                m_SpeechPattern = (NPCPersonalities.SpeechPattern)reader.ReadInt();
                m_HearingRange = reader.ReadInt();
            }
            else
            {
                // Defaults for old saves
                m_LLMConversationEnabled = true;
                m_PersonalityType = NPCPersonalities.PersonalityType.Sage;
                m_SpeechPattern = NPCPersonalities.SpeechPattern.Formal;
                m_HearingRange = 6;
            }
            
            if (version >= 2)
            {
                m_NpcTemplateId = reader.ReadString();
            }
        }

        #endregion
    }
}
