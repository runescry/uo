using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Custom.VystiaClasses.Quests;
using Server.Services.LLM.Core;
using ServUO.Scripts.Services.LLM;

namespace Server.Services.LLM
{
    /// <summary>
    /// Helper class for processing LLM conversations
    /// Extracted from LLMNpc to be reusable for any NPC implementing ILLMConversational
    /// </summary>
    public static class LLMConversationHelper
    {
        // Per-NPC conversation state tracking
        private static readonly Dictionary<Mobile, ConversationState> m_ConversationStates = new Dictionary<Mobile, ConversationState>();
        
        // Per-NPC knowledge base cache (to avoid reloading on every conversation)
        private static readonly Dictionary<Mobile, string> m_KnowledgeBaseCache = new Dictionary<Mobile, string>();
        private static readonly object m_KnowledgeBaseCacheLock = new object();

        private static readonly int MaxQueueSize = 5;
        private static readonly TimeSpan QueueTimeout = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan ActiveConversationTimeout = TimeSpan.FromMinutes(2);

        /// <summary>
        /// Conversation state for an NPC
        /// </summary>
        private class ConversationState
        {
            public Queue<SpeechRequest> RequestQueue { get; set; }
            public bool IsProcessingRequest { get; set; }
            public HashSet<Mobile> ProcessingPlayers { get; set; }
            public Dictionary<Mobile, DateTime> ActiveConversations { get; set; }
            public readonly object QueueLock = new object();

            public ConversationState()
            {
                RequestQueue = new Queue<SpeechRequest>();
                ProcessingPlayers = new HashSet<Mobile>();
                ActiveConversations = new Dictionary<Mobile, DateTime>();
                IsProcessingRequest = false;
            }
        }

        /// <summary>
        /// Represents a queued speech request
        /// </summary>
        private class SpeechRequest
        {
            public Mobile Player { get; set; }
            public string Message { get; set; }
            public DateTime EnqueueTime { get; set; }

            public SpeechRequest(Mobile player, string message)
            {
                Player = player;
                Message = message;
                EnqueueTime = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Gets or creates conversation state for an NPC
        /// </summary>
        private static ConversationState GetOrCreateState(Mobile npc)
        {
            if (!m_ConversationStates.TryGetValue(npc, out ConversationState state))
            {
                state = new ConversationState();
                m_ConversationStates[npc] = state;
            }
            return state;
        }

        /// <summary>
        /// Proactively load knowledge base for an NPC at spawn time (same as LLMNpc does)
        /// This allows all ILLMConversational NPCs to benefit from proactive loading
        /// </summary>
        public static void ProactivelyLoadKnowledgeBase(Mobile npc, ILLMConversational llmNpc)
        {
            if (npc == null || llmNpc == null)
                return;

            lock (m_KnowledgeBaseCacheLock)
            {
                // Skip if already cached
                if (m_KnowledgeBaseCache.ContainsKey(npc))
                    return;

                DateTime knowledgeStart = DateTime.UtcNow;
                string formattedKnowledge = "";

                try
                {
                    // Infer role from personality type
                    NPCKnowledgeSystem.NPCRole role = NPCKnowledgeSystem.InferRoleFromPersonality(llmNpc.PersonalityType);

                    // Get location name for context
                    string locationName = NPCPersonalities.GetLocationName(npc);

                    // Check if we have a valid location (not Internal map)
                    List<LoreEntry> knowledgeBase;
                    if (npc.Map == null || npc.Map == Map.Internal)
                    {
                        // Load only role-based knowledge for now, will reload when placed
                        var allLore = SimpleLoreSystem.GetAllLore();
                        var roleKnowledge = NPCKnowledgeSystem.GetRoleKnowledge(role, allLore);
                        // Also add dungeons and important locations
                        var locationKnowledge = allLore.Where(l =>
                            l.Category == "Dungeon" ||
                            (l.Category == "Location" && l.Importance >= 8)
                        ).ToList();
                        knowledgeBase = new List<LoreEntry>();
                        knowledgeBase.AddRange(roleKnowledge);
                        knowledgeBase.AddRange(locationKnowledge);
                        knowledgeBase = knowledgeBase.GroupBy(l => l.ID).Select(g => g.First()).ToList();
                    }
                    else
                    {
                        // Load knowledge base (role + location specific)
                        knowledgeBase = NPCKnowledgeSystem.GetNPCKnowledge(role, locationName, npc.Location, npc.Map);
                    }

                    // Pre-format for prompts (cache it so we don't rebuild every conversation)
                    formattedKnowledge = NPCKnowledgeSystem.FormatKnowledgeForPrompt(knowledgeBase);

                    // Cache it for this NPC
                    m_KnowledgeBaseCache[npc] = formattedKnowledge;

                    long knowledgeTime = (long)(DateTime.UtcNow - knowledgeStart).TotalMilliseconds;
                    LLMLoggingConfig.LogConversationHelper($"[PROACTIVE] Knowledge base loaded for {npc.Name} ({role}): {knowledgeBase.Count} entries in {knowledgeTime}ms");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LLMConversationHelper] Error in proactive knowledge base loading for {npc.Name}: {ex.Message}");
                    formattedKnowledge = ""; // Use empty to avoid retrying
                    m_KnowledgeBaseCache[npc] = "";
                }
            }
        }

        /// <summary>
        /// Processes a conversation request for an NPC
        /// </summary>
        public static void ProcessConversation(Mobile npc, Mobile player, string message)
        {
            DateTime processStart = DateTime.UtcNow;
            LLMLoggingConfig.LogDebug($"ProcessConversation START - NPC: {npc?.Name ?? "null"}, Player: {player?.Name ?? "null"}, Time: {processStart:HH:mm:ss.fff}");
            
            if (npc == null || player == null || string.IsNullOrWhiteSpace(message))
            {
                Console.WriteLine($"[LLMConversationHelper] ProcessConversation: Invalid parameters - npc={npc?.Name ?? "null"}, player={player?.Name ?? "null"}, message={(string.IsNullOrWhiteSpace(message) ? "empty" : "valid")}");
                return;
            }

            DateTime beforeInterfaceCheck = DateTime.UtcNow;
            var llmNpc = npc as ILLMConversational;
            long interfaceCheckTime = (long)(DateTime.UtcNow - beforeInterfaceCheck).TotalMilliseconds;
            LLMLoggingConfig.LogTiming($"ProcessConversation - Interface check took {interfaceCheckTime}ms");
            
            if (llmNpc == null || !llmNpc.LLMConversationEnabled)
            {
                Console.WriteLine($"[LLMConversationHelper] ProcessConversation: NPC {npc.Name} is not LLM-enabled (is ILLMConversational={llmNpc != null}, enabled={llmNpc?.LLMConversationEnabled ?? false})");
                return;
            }

            LLMLoggingConfig.LogDebug($"ProcessConversation: Processing conversation for {npc.Name} from {player.Name}: '{message}'");

            DateTime beforeGetState = DateTime.UtcNow;
            var state = GetOrCreateState(npc);
            long getStateTime = (long)(DateTime.UtcNow - beforeGetState).TotalMilliseconds;
            LLMLoggingConfig.LogTiming($"ProcessConversation - GetOrCreateState took {getStateTime}ms");

            // Update active conversation timestamp
            state.ActiveConversations[player] = DateTime.UtcNow;

            // Set focus on the speaking player
            if (npc is BaseCreature bc)
            {
                bc.FocusMob = player;
            }

            DateTime beforeEnqueue = DateTime.UtcNow;
            // Enqueue the request
            EnqueueSpeechRequest(npc, llmNpc, state, player, message);
            long enqueueTime = (long)(DateTime.UtcNow - beforeEnqueue).TotalMilliseconds;
            LLMLoggingConfig.LogTiming($"ProcessConversation - EnqueueSpeechRequest took {enqueueTime}ms");
            
            long totalTime = (long)(DateTime.UtcNow - processStart).TotalMilliseconds;
            LLMLoggingConfig.LogTiming($"ProcessConversation END - Total time: {totalTime}ms");
        }

        /// <summary>
        /// Checks if an NPC should handle a conversation
        /// </summary>
        public static bool ShouldHandleConversation(Mobile npc, Mobile player, string speech)
        {
            if (npc == null || player == null || !(player is PlayerMobile) || !player.Alive)
                return false;

            var llmNpc = npc as ILLMConversational;
            if (llmNpc == null || !llmNpc.LLMConversationEnabled)
                return false;

            // Check range
            if (!player.InRange(npc.Location, llmNpc.HearingRange))
                return false;

            if (string.IsNullOrWhiteSpace(speech))
                return false;

            var state = GetOrCreateState(npc);

            // Check if the player is addressing this NPC specifically
            bool wasNamed = WasNamed(npc, speech);
            bool inActiveConversation = IsInActiveConversation(state, player);

            // If player named THIS NPC specifically, respond
            if (wasNamed)
                return true;

            // If player is in active conversation but didn't name anyone, still respond
            if (inActiveConversation)
            {
                // BUT: Check if they named a DIFFERENT NPC nearby
                if (PlayerNamedDifferentNPC(npc, llmNpc, player, speech))
                {
                    state.ActiveConversations.Remove(player); // End this conversation
                    return false;
                }
                return true; // Continue active conversation
            }

            // If player didn't name anyone and not in active conversation:
            // Check if this is the only LLM NPC nearby - if so, respond to any speech
            // OPTIMIZATION: Only check if we're in a town (faster check)
            // If not in town, assume we're the only one (most common case)
            if (npc is BaseCreature bc && bc.IsHumanInTown())
            {
                IPooledEnumerable eable = npc.GetMobilesInRange(llmNpc.HearingRange);
                int otherLLMNpcCount = 0;
                foreach (Mobile m in eable)
                {
                    if (m is ILLMConversational && m != npc && m.Alive)
                    {
                        otherLLMNpcCount++;
                        if (otherLLMNpcCount > 0) break; // Early exit if we find one
                    }
                }
                eable.Free();

                // If this is the only LLM NPC nearby, respond to any speech (makes it easier to start conversations)
                if (otherLLMNpcCount == 0)
                    return true;
            }
            else
            {
                // Not in town, likely the only LLM NPC nearby
                return true;
            }

            // Multiple LLM NPCs nearby, player must name this one specifically
            return false;
        }

        /// <summary>
        /// Enqueues a speech request for processing
        /// </summary>
        private static void EnqueueSpeechRequest(Mobile npc, ILLMConversational llmNpc, ConversationState state, Mobile player, string message)
        {
            DateTime enqueueStart = DateTime.UtcNow;
            LLMLoggingConfig.LogDebug($"EnqueueSpeechRequest START - NPC: {npc?.Name}, Player: {player?.Name}, Time: {enqueueStart:HH:mm:ss.fff}");
            
            DateTime beforeLock = DateTime.UtcNow;
            lock (state.QueueLock)
            {
                long lockWaitTime = (long)(DateTime.UtcNow - beforeLock).TotalMilliseconds;
                if (lockWaitTime > 0)
                {
                    LLMLoggingConfig.LogDebug($"EnqueueSpeechRequest - Lock wait: {lockWaitTime}ms");
                }
                
                DateTime beforeQueueCheck = DateTime.UtcNow;
                // Check if player already has a request in queue OR is currently being processed
                if (state.RequestQueue.Any(r => r.Player == player) || state.ProcessingPlayers.Contains(player))
                {
                    long queueCheckTime = (long)(DateTime.UtcNow - beforeQueueCheck).TotalMilliseconds;
                    LLMLoggingConfig.LogDebug($"EnqueueSpeechRequest - Queue check took {queueCheckTime}ms, player already in queue");
                    npc.PrivateOverheadMessage(MessageType.Regular, 0x3B2, false, "*is already listening to you*", player.NetState);
                    return;
                }

                // Check queue size limit
                if (state.RequestQueue.Count >= MaxQueueSize)
                {
                    npc.PrivateOverheadMessage(MessageType.Regular, 0x3B2, false, "*is busy with others, please wait*", player.NetState);
                    return;
                }

                // Add to queue
                SpeechRequest request = new SpeechRequest(player, message);
                state.RequestQueue.Enqueue(request);

                // Show feedback based on queue position
                if (state.RequestQueue.Count == 1 && !state.IsProcessingRequest)
                {
                    npc.PrivateOverheadMessage(MessageType.Regular, 0x3B2, false, "*ponders*", player.NetState);
                }
                else
                {
                    int position = state.RequestQueue.Count;
                    npc.PrivateOverheadMessage(MessageType.Regular, 0x3B2, false, $"*acknowledges you ({position} in line)*", player.NetState);
                }
            }

            long enqueueTime = (long)(DateTime.UtcNow - enqueueStart).TotalMilliseconds;
            LLMLoggingConfig.LogDebug($"EnqueueSpeechRequest - Enqueue took {enqueueTime}ms");

            // Try to process the queue
            DateTime beforeProcessNext = DateTime.UtcNow;
            ProcessNextRequest(npc, llmNpc, state);
            long processNextTime = (long)(DateTime.UtcNow - beforeProcessNext).TotalMilliseconds;
            LLMLoggingConfig.LogDebug($"EnqueueSpeechRequest - ProcessNextRequest took {processNextTime}ms");
            
            long totalEnqueueTime = (long)(DateTime.UtcNow - enqueueStart).TotalMilliseconds;
            LLMLoggingConfig.LogDebug($"EnqueueSpeechRequest END - Total time: {totalEnqueueTime}ms");
        }

        /// <summary>
        /// Processes the next request in the queue
        /// </summary>
        private static void ProcessNextRequest(Mobile npc, ILLMConversational llmNpc, ConversationState state)
        {
            lock (state.QueueLock)
            {
                // Don't process if already processing or queue is empty
                if (state.IsProcessingRequest || state.RequestQueue.Count == 0)
                    return;

                // Mark as processing
                state.IsProcessingRequest = true;

                // Clean up expired requests
                CleanupExpiredRequests(npc, llmNpc, state);

                // Check if queue is still not empty after cleanup
                if (state.RequestQueue.Count == 0)
                {
                    state.IsProcessingRequest = false;
                    return;
                }

                // Get next request
                SpeechRequest request = state.RequestQueue.Dequeue();

                // Process asynchronously
                ProcessSpeechAsync(npc, llmNpc, state, request.Player, request.Message);
            }
        }

        /// <summary>
        /// Removes expired requests from the queue
        /// </summary>
        private static void CleanupExpiredRequests(Mobile npc, ILLMConversational llmNpc, ConversationState state)
        {
            List<SpeechRequest> validRequests = new List<SpeechRequest>();

            while (state.RequestQueue.Count > 0)
            {
                SpeechRequest request = state.RequestQueue.Dequeue();

                // Check if request is still valid
                if (request.Player == null || request.Player.Deleted || !request.Player.Alive)
                    continue;

                // Check if player is still in range
                if (!request.Player.InRange(npc.Location, llmNpc.HearingRange))
                    continue;

                // Check if request has timed out
                if (DateTime.UtcNow - request.EnqueueTime > QueueTimeout)
                {
                    if (!request.Player.Deleted && request.Player.Alive)
                    {
                        npc.PrivateOverheadMessage(MessageType.Regular, 0x3B2, false, "*seems to have lost track of your question*", request.Player.NetState);
                    }
                    continue;
                }

                // Request is still valid
                validRequests.Add(request);
            }

            // Re-add valid requests
            foreach (var request in validRequests)
            {
                state.RequestQueue.Enqueue(request);
            }
        }

        /// <summary>
        /// Processes player speech and generates an LLM response
        /// </summary>
        private static async void ProcessSpeechAsync(Mobile npc, ILLMConversational llmNpc, ConversationState state, Mobile player, string message)
        {
            DateTime startTime = DateTime.UtcNow;

            try
            {
                // Mark player as being processed
                state.ProcessingPlayers.Add(player);

                // Check if first conversation BEFORE adding message (critical for performance)
                List<ConversationMessage> historyBefore = ConversationContext.GetHistory(npc, player);
                bool isFirstConversation = historyBefore == null || historyBefore.Count == 0;

                // Add player message to conversation history
                ConversationContext.AddPlayerMessage(npc, player, message);

                // Get conversation history (now includes the message we just added)
                List<ConversationMessage> history = ConversationContext.GetHistory(npc, player);
                LLMLoggingConfig.LogDebug($"[LLMConversationHelper] Conversation history length: {history.Count}, isFirstConversation: {isFirstConversation}");

                // Check if NPC is a vendor
                bool isVendor = npc is BaseVendor;

                // Build personality string
                string personalityPrompt = NPCPersonalities.GetPersonalityPrompt(llmNpc.PersonalityType, llmNpc.SpeechPattern);

                // Add contextual awareness
                personalityPrompt += NPCPersonalities.GetContextualInfo(npc, player);

                // Inject quest/waypoint context for QuestNPCs so each waypoint can guide the player properly.
                // This is critical for step-by-step quest narration (Mentor -> Scout -> Captain -> Kill FrostGiant, etc).
                if (npc is QuestNPC questNpc && player is PlayerMobile pm)
                {
                    string questContext = questNpc.BuildQuestAwareContext(pm);
                    if (!string.IsNullOrWhiteSpace(questContext))
                    {
                        // QUEST CONTEXT OVERRIDES PERSONALITY - Make it the PRIMARY prompt
                        personalityPrompt = "=== PRIMARY ROLE: QUEST NPC ===\n" + questContext;
                        personalityPrompt += "\n\n=== CRITICAL INSTRUCTIONS ===";
                        personalityPrompt += "\n1. You are a QUEST NPC. Your primary purpose is to guide the player through the quest.";
                        personalityPrompt += "\n2. The quest context above defines WHO you are, WHAT you should say, and WHERE you should guide the player.";
                        personalityPrompt += "\n3. Use the dialogue instructions in the quest context as your PRIMARY source for what to say.";
                        personalityPrompt += "\n4. Do NOT use generic casual greetings like 'Hey there! What's up?' - instead, speak as the character described in the quest context.";
                        personalityPrompt += "\n5. If the player has not started the quest, offer it. If they are on the quest, guide them to the next step.";
                        personalityPrompt += "\n6. Your personality and speech pattern should match the quest context, not generic NPC behavior.";
                        personalityPrompt += "\n7. MANDATORY LOCATION RULE: When the player asks ANY question about location ('where', 'where exactly', 'how do I find', etc.), you MUST provide the SPECIFIC location information from the quest context (direction, distance, coordinates, landmark). NEVER say 'I can't provide precise directions' - you HAVE the information, PROVIDE IT IMMEDIATELY.";
                        personalityPrompt += "\n8. NEVER give vague poetic directions like 'follow the winding paths' or 'trust your instincts' - players need ACTIONABLE information with direction, distance, and coordinates.";
                        personalityPrompt += "\n\n=== PERSONALITY (SECONDARY - ONLY IF NOT CONFLICTING WITH QUEST CONTEXT) ===";
                        personalityPrompt += "\n" + NPCPersonalities.GetPersonalityPrompt(llmNpc.PersonalityType, llmNpc.SpeechPattern);
                        personalityPrompt += "\n" + NPCPersonalities.GetContextualInfo(npc, player);
                    }
                }
                // Also inject quest context for Chronicler so it can provide quest guidance
                // BUT: If player is requesting a new quest, ignore existing quests
                else if (npc is Chronicler chronicler && player is PlayerMobile pm2)
                {
                    string questContext = chronicler.BuildQuestAwareContext(pm2, message);
                    if (!string.IsNullOrWhiteSpace(questContext))
                    {
                        personalityPrompt += "\n\n=== QUEST CONTEXT (PRIORITY) ===\n" + questContext;
                        personalityPrompt += "\n\nIMPORTANT: When the player asks about where to go or what to do, provide guidance based on their current quest objective. Do NOT give generic advice about healers or other NPCs unless they are part of the quest.";
                    }
                }

                // Load knowledge base for traditional NPCs (same as LLMNpc does)
                // Cache it per NPC to avoid reloading on every conversation
                // Load synchronously but quickly (limited knowledge base size now)
                DateTime knowledgeStart = DateTime.UtcNow;
                string formattedKnowledge = "";
                lock (m_KnowledgeBaseCacheLock)
                {
                    if (!m_KnowledgeBaseCache.TryGetValue(npc, out formattedKnowledge))
                    {
                        // Load knowledge base synchronously but quickly (limited size now)
                        // This is fast enough that it won't cause significant delay
                        try
                        {
                            // Infer role from personality type
                            NPCKnowledgeSystem.NPCRole role = NPCKnowledgeSystem.InferRoleFromPersonality(llmNpc.PersonalityType);

                            // Get location name for context
                            string locationName = NPCPersonalities.GetLocationName(npc);

                            // Load knowledge base (role + location specific) - now limited to top entries
                            List<LoreEntry> knowledgeBase = NPCKnowledgeSystem.GetNPCKnowledge(role, locationName, npc.Location, npc.Map);

                            // Pre-format for prompts (cache it so we don't rebuild every conversation)
                            formattedKnowledge = NPCKnowledgeSystem.FormatKnowledgeForPrompt(knowledgeBase);

                            // Cache it for this NPC
                            m_KnowledgeBaseCache[npc] = formattedKnowledge;

                            long knowledgeTime = (long)(DateTime.UtcNow - knowledgeStart).TotalMilliseconds;
                            LLMLoggingConfig.LogTiming($"Knowledge base loading: {knowledgeTime}ms");
                            LLMLoggingConfig.LogConversationHelper($"{npc.Name} ({role}) knowledge base loaded: {knowledgeBase.Count} entries, formatted: {formattedKnowledge.Length} chars");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[LLMConversationHelper] Error loading knowledge base for {npc.Name}: {ex.Message}");
                            formattedKnowledge = ""; // Use empty to avoid retrying
                            m_KnowledgeBaseCache[npc] = "";
                        }
                    }
                    else
                    {
                        long knowledgeTime = (long)(DateTime.UtcNow - knowledgeStart).TotalMilliseconds;
                        LLMLoggingConfig.LogTiming($"Knowledge base (cached): {knowledgeTime}ms");
                    }
                }

                // Load memories and relationship (if memory system is available)
                string memoriesText = "";
                if (LLMMemoryService.IsAvailable() && !isFirstConversation)
                {
                    try
                    {
                        DateTime memoryLoadStart = DateTime.UtcNow;
                        // CRITICAL: Always use npc.Serial to ensure memories are NPC-specific
                        // Memories are keyed by (npc_serial, player_name) - each NPC instance has a unique serial
                        var memories = await LLMMemoryService.GetMemoriesAsync(npc.Serial, player.Name, limit: 5);
                        var relationship = await LLMMemoryService.GetRelationshipAsync(npc.Serial, player.Name);
                        long memoryLoadTime = (long)(DateTime.UtcNow - memoryLoadStart).TotalMilliseconds;

                        LLMLoggingConfig.LogMemory($"Memory query result: {memories.Count} memories found for NPC {npc.Serial} (Name: {npc.Name}) and player {player.Name} in {memoryLoadTime}ms");

                        // Verify all loaded memories belong to this NPC (defensive check)
                        foreach (var mem in memories)
                        {
                            if (mem.NpcSerial != npc.Serial)
                            {
                                Console.WriteLine($"[LLMConversationHelper] ERROR: Memory with serial {mem.NpcSerial} loaded for NPC {npc.Serial} - memory belongs to different NPC!");
                            }
                        }

                        // Always load relationship if available, even if no memories yet
                        // This allows relationship progression from the start
                        if (relationship != null && relationship.NpcSerial == npc.Serial)
                        {
                            LLMLoggingConfig.LogMemory($"Loading relationship for NPC {npc.Serial} and player {player.Name}");
                            
                            memoriesText = MemoryHelpers.FormatMemoriesForPrompt(memories, maxMemories: 5);
                            memoriesText += MemoryHelpers.FormatRelationshipForPrompt(relationship);
                            
                            // Add relationship-based greeting hint for the LLM
                            if (npc is ILLMConversational conversationalNpc)
                            {
                                string relationshipGreeting = NPCPersonalities.GetRelationshipBasedGreeting(
                                    conversationalNpc.PersonalityType, 
                                    relationship.Type, 
                                    relationship.Score
                                );
                                memoriesText += $"\n## Suggested Greeting Style:\n{relationshipGreeting}";
                            }
                            
                            LLMLoggingConfig.LogMemory($"Loaded {memories.Count} memories + relationship for NPC {npc.Serial} (Name: {npc.Name}) and player {player.Name} in {memoryLoadTime}ms");
                        }
                        else if (relationship != null && relationship.NpcSerial != npc.Serial)
                        {
                            Console.WriteLine($"[LLMConversationHelper] ERROR: Relationship with serial {relationship.NpcSerial} loaded for NPC {npc.Serial} - relationship belongs to different NPC!");
                        }
                        else if (memories.Count > 0)
                        {
                            // Only load memories if we have them but no relationship (edge case)
                            LLMLoggingConfig.LogMemory($"Loaded {memories.Count} memories (no relationship) for NPC {npc.Serial} (Name: {npc.Name}) and player {player.Name} in {memoryLoadTime}ms");
                            memoriesText = MemoryHelpers.FormatMemoriesForPrompt(memories, maxMemories: 5);
                        }
                        else
                        {
                            LLMLoggingConfig.LogMemory($"No memories or relationship found for NPC {npc.Serial} (Name: {npc.Name}) and player {player.Name} (load time: {memoryLoadTime}ms)");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[LLMConversationHelper] Error loading memories: {ex.Message}");
                    }
                }
                else if (!LLMMemoryService.IsAvailable())
                {
                    Console.WriteLine($"[LLMConversationHelper] Memory system not available");
                }

                // Append memories to personality prompt if we have any
                if (!string.IsNullOrEmpty(memoriesText))
                {
                    personalityPrompt += memoriesText;
                }

                // Call unified LLM service (pass isFirstConversation to optimize)
                string response = await UnifiedLLMService.GetResponseAsync(
                    npc.Name,
                    personalityPrompt,
                    history,
                    message,
                    player.Name,
                    formattedKnowledge,
                    UnifiedLLMService.RequestType.PlayerConversation,
                    UnifiedLLMService.LLMProvider.Auto,
                    isVendor,
                    isFirstConversation // Pass flag to skip embeddings on first conversation
                );

                // Check for vendor intent commands in the response using structured parser
                VendorActionResponse vendorActionResponse = VendorActionParser.Parse(response);
                VendorAction vendorAction = vendorActionResponse.Action;

                // Add NPC response to history
                ConversationContext.AddNpcMessage(npc, player, response);

                // Extract and save memories from this conversation (async, non-blocking)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        if (!LLMMemoryService.IsAvailable())
                            return;

                        List<ConversationMessage> fullHistory = ConversationContext.GetHistory(npc, player);
                        List<Memory> memories = MemoryHelpers.ExtractMemoriesFromConversation(npc.Name, player.Name, fullHistory);

                        if (memories.Count > 0)
                        {
                            LLMLoggingConfig.LogMemory($"[LLMConversationHelper] Extracted {memories.Count} memories from conversation with {player.Name}");
                            foreach (var memory in memories)
                            {
                                await LLMMemoryService.SaveMemoryAsync(npc.Serial, npc.Name, player.Name, memory);
                                LLMLoggingConfig.LogMemory($"[LLMConversationHelper] Saved memory: {memory.Content.Substring(0, Math.Min(50, memory.Content.Length))}... (Importance: {memory.Importance})");
                            }

                            // Update relationship based on conversation
                            await LLMMemoryService.UpdateRelationshipAsync(npc.Serial, npc.Name, player.Name, 1);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[LLMConversationHelper] Error saving memories: {ex.Message}");
                    }
                });

                // Schedule the response to be said on the main server thread
                Timer.DelayCall(TimeSpan.Zero, () =>
                {
                    // Make sure player is still around and remove from processing list
                    state.ProcessingPlayers.Remove(player);

                    // Mark as no longer processing
                    lock (state.QueueLock)
                    {
                        state.IsProcessingRequest = false;
                    }

                    if (!player.Deleted && player.Alive && player.InRange(npc.Location, llmNpc.HearingRange))
                    {
                        if (vendorAction == VendorAction.Sell && npc is BaseVendor vendorForSellCheck && !PlayerHasItemsToSell(vendorForSellCheck, player))
                        {
                            ExecuteVendorAction(vendorForSellCheck, player, vendorAction);
                            return;
                        }

                        // Strip vendor command markers from response before displaying
                        string displayResponse = StripVendorCommands(response);

                        // Break long responses into chunks
                        SendChunkedResponse(npc, player, displayResponse, llmNpc);

                        // Execute vendor action if detected
                        if (vendorAction != VendorAction.None && npc is BaseVendor vendor)
                        {
                            Timer.DelayCall(TimeSpan.FromMilliseconds(500), () =>
                            {
                                ExecuteVendorAction(vendor, player, vendorAction);
                            });
                        }
                    }

                    // Process next request in queue
                    ProcessNextRequest(npc, llmNpc, state);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LLMConversationHelper] Error processing speech for {npc.Name}: {ex.Message}");
                Console.WriteLine($"[LLMConversationHelper] Stack trace: {ex.StackTrace}");

                Timer.DelayCall(TimeSpan.Zero, () =>
                {
                    state.ProcessingPlayers.Remove(player);

                    lock (state.QueueLock)
                    {
                        state.IsProcessingRequest = false;
                    }

                    if (!player.Deleted && player.Alive && player.InRange(npc.Location, llmNpc.HearingRange))
                    {
                        npc.SayTo(player, "I apologize, but my mind seems clouded at the moment.", 0x3B2);
                    }

                    // Process next request in queue
                    ProcessNextRequest(npc, llmNpc, state);
                });
            }
        }

        
        /// <summary>
        /// Strips vendor command markers, JSON blocks, and meta-commentary from response text
        /// </summary>
        private static string StripVendorCommands(string response)
        {
            if (string.IsNullOrEmpty(response))
                return response;

            // Strip structured JSON blocks (both fenced and bare)
            string jsonBlock = VendorActionParser.ExtractJsonBlock(response);
            if (jsonBlock != null)
            {
                // Remove the JSON block from the original response
                // Try both fenced and bare patterns
                var fencedMatch = System.Text.RegularExpressions.Regex.Match(
                    response,
                    @"```(?:json)?\s*\{.*?\}\s*```",
                    System.Text.RegularExpressions.RegexOptions.Singleline);
                
                if (fencedMatch.Success)
                {
                    response = response.Replace(fencedMatch.Value, "").Trim();
                }
                else
                {
                    // Remove bare JSON object
                    response = response.Replace(jsonBlock, "").Trim();
                }
            }

            // Strip markdown headers
            int markdownIndex = response.IndexOf("###", StringComparison.OrdinalIgnoreCase);
            if (markdownIndex >= 0)
            {
                response = response.Substring(0, markdownIndex).Trim();
            }

            // Remove meta-commentary sections
            string[] metaTags = new[] { 
                "EXPLAINER:", "EXPLANATION:", "NOTE:", "CONTEXT:", "META:", "SOLUTION:", 
                "--- SOLUTION ---", "---SOLUTION---", "--- THREE FOLLOW-UP QUESTIONS ---",
                "FOLLOW-UP QUESTION", "### Follow-up", "#### Elaborated", "TEXTBOOK-LEVEL"
            };

            string lowerResponse = response.ToLower();
            foreach (string tag in metaTags)
            {
                int tagIndex = lowerResponse.IndexOf(tag.ToLower());
                if (tagIndex >= 0)
                {
                    response = response.Substring(0, tagIndex).Trim();
                    lowerResponse = response.ToLower();
                }
            }

            // Process line by line
            string[] lines = response.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++)
            {
                string trimmedLine = lines[i].Trim();
                
                // Check for separator lines
                if (trimmedLine.StartsWith("---") && trimmedLine.Length > 3)
                {
                    string withoutDashes = trimmedLine.Replace("-", "").Trim();
                    string lowerLine = trimmedLine.ToLower();
                    if (withoutDashes.Length == 0 || 
                        lowerLine.Contains("solution") ||
                        lowerLine.Contains("follow-up") ||
                        lowerLine.Contains("question") ||
                        lowerLine.Contains("meta"))
                    {
                        response = string.Join("\n", lines.Take(i)).Trim();
                        goto done;
                    }
                }
                
                // Check for markdown headers
                if (trimmedLine.StartsWith("###"))
                {
                    response = string.Join("\n", lines.Take(i)).Trim();
                    goto done;
                }
                
                // Check for meta tags
                string lowerLineForTags = trimmedLine.ToLower();
                foreach (string tag in metaTags)
                {
                    if (lowerLineForTags.StartsWith(tag.ToLower()))
                    {
                        response = string.Join("\n", lines.Take(i)).Trim();
                        goto done;
                    }
                }
            }
            done:

            return response
                .Replace("[VENDOR_BUY]", "")
                .Replace("[vendor_buy]", "")
                .Replace("[VENDOR_SELL]", "")
                .Replace("[vendor_sell]", "")
                .Replace("(VENDOR_BUY)", "")
                .Replace("(vendor_buy)", "")
                .Replace("(VENDOR_SELL]", "")
                .Replace("(vendor_sell)", "")
                .Trim();
        }

        /// <summary>
        /// Executes vendor action (open buy/sell gump)
        /// </summary>
        private static void ExecuteVendorAction(BaseVendor vendor, Mobile player, VendorAction action)
        {
            if (action == VendorAction.Buy)
            {
                vendor.VendorBuy(player);
            }
            else if (action == VendorAction.Sell)
            {
                vendor.VendorSell(player);
            }
        }

        private static bool PlayerHasItemsToSell(BaseVendor vendor, Mobile player)
        {
            Container pack = player.Backpack;
            if (pack == null)
                return false;

            var info = vendor.GetSellInfo();

            foreach (IShopSellInfo ssi in info)
            {
                var items = pack.FindItemsByType(ssi.Types);

                foreach (Item item in items)
                {
                    if (item is Container container && container.Items.Count != 0)
                        continue;

                    if (item.IsStandardLoot() && item.Movable && ssi.IsSellable(item))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Sends a response to the player, breaking it into chunks if it's too long
        /// </summary>
        private static void SendChunkedResponse(Mobile npc, Mobile player, string response, ILLMConversational llmNpc)
        {
            if (string.IsNullOrEmpty(response))
                return;

            // Default chunk settings
            int maxChunkLength = 100;
            int delayBetweenChunks = 1500;

            // Try to get from LLMNpc if available
            if (npc is LLMNpc llmNpcInstance)
            {
                // Access via reflection or add properties to interface
                // For now, use defaults
            }

            // If response is short enough, send it all at once
            if (response.Length <= maxChunkLength)
            {
                npc.SayTo(player, response, 0x3B2);
                return;
            }

            // Break into sentences first
            string[] sentences = response.Split(new[] { ". ", "! ", "? " }, StringSplitOptions.None);
            List<string> chunks = new List<string>();
            string currentChunk = "";

            foreach (string sentence in sentences)
            {
                string sentenceWithPunctuation = sentence;

                // Add back the punctuation if it was removed
                if (!sentence.EndsWith(".") && !sentence.EndsWith("!") && !sentence.EndsWith("?"))
                {
                    int originalIndex = response.IndexOf(sentence);
                    if (originalIndex >= 0 && originalIndex + sentence.Length < response.Length)
                    {
                        char nextChar = response[originalIndex + sentence.Length];
                        if (nextChar == '.' || nextChar == '!' || nextChar == '?')
                        {
                            sentenceWithPunctuation = sentence + nextChar;
                        }
                    }
                }

                // If adding this sentence would exceed the limit, start a new chunk
                if (currentChunk.Length + sentenceWithPunctuation.Length + 1 > maxChunkLength && currentChunk.Length > 0)
                {
                    chunks.Add(currentChunk.Trim());
                    currentChunk = sentenceWithPunctuation + " ";
                }
                else
                {
                    currentChunk += sentenceWithPunctuation + " ";
                }
            }

            // Add the last chunk if there's anything left
            if (!string.IsNullOrEmpty(currentChunk.Trim()))
            {
                chunks.Add(currentChunk.Trim());
            }

            // Send chunks with delays
            int delay = 0;
            for (int i = 0; i < chunks.Count; i++)
            {
                string chunk = chunks[i];
                int currentDelay = delay;

                Timer.DelayCall(TimeSpan.FromMilliseconds(currentDelay), () =>
                {
                    if (!player.Deleted && player.Alive && player.InRange(npc.Location, llmNpc.HearingRange))
                    {
                        npc.SayTo(player, chunk, 0x3B2);
                    }
                });

                delay += delayBetweenChunks;
            }
        }

        /// <summary>
        /// Checks if the player has an active conversation with this NPC
        /// </summary>
        private static bool IsInActiveConversation(ConversationState state, Mobile player)
        {
            if (state.ActiveConversations.TryGetValue(player, out DateTime lastInteraction))
            {
                TimeSpan timeSinceLastMessage = DateTime.UtcNow - lastInteraction;
                if (timeSinceLastMessage < ActiveConversationTimeout)
                {
                    return true;
                }
                else
                {
                    // Conversation expired, remove from active list
                    state.ActiveConversations.Remove(player);
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if player named a different LLM NPC nearby
        /// </summary>
        private static bool PlayerNamedDifferentNPC(Mobile npc, ILLMConversational llmNpc, Mobile player, string speech)
        {
            if (string.IsNullOrEmpty(speech))
                return false;

            string lowerSpeech = speech.ToLower();

            // Check all nearby NPCs that implement ILLMConversational
            IPooledEnumerable eable = npc.GetMobilesInRange(llmNpc.HearingRange);
            bool namedOther = false;

            foreach (Mobile m in eable)
            {
                if (m is ILLMConversational otherNpc && m != npc && m.Alive)
                {
                    // Check if other NPC's name is mentioned
                    string otherLowerName = m.Name.ToLower();

                    // Check full name
                    if (ContainsWord(lowerSpeech, otherLowerName))
                    {
                        namedOther = true;
                        break;
                    }

                    // Check first name
                    string[] otherNameParts = m.Name.Split(' ');
                    if (otherNameParts.Length > 0)
                    {
                        string otherFirstName = otherNameParts[0].ToLower();
                        if (ContainsWord(lowerSpeech, otherFirstName))
                        {
                            namedOther = true;
                            break;
                        }
                    }
                }
            }

            eable.Free();
            return namedOther;
        }

        /// <summary>
        /// Checks if a word exists in text with word boundaries
        /// </summary>
        private static bool ContainsWord(string text, string word)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(word))
                return false;

            string pattern = @"\b" + Regex.Escape(word) + @"\b";
            return Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Checks if the NPC's name was mentioned in the speech
        /// </summary>
        private static bool WasNamed(Mobile npc, string speech)
        {
            if (string.IsNullOrEmpty(speech) || string.IsNullOrEmpty(npc.Name))
                return false;

            string lowerSpeech = speech.ToLower();
            string lowerName = npc.Name.ToLower();

            // Check if full name is mentioned
            if (ContainsWord(lowerSpeech, lowerName))
                return true;

            // Check if first name is mentioned
            string[] nameParts = npc.Name.Split(' ');
            if (nameParts.Length > 0)
            {
                string firstName = nameParts[0].ToLower();
                if (ContainsWord(lowerSpeech, firstName))
                    return true;
            }

            // Check if title is mentioned (if player is alone with this NPC)
            IPooledEnumerable eable = npc.GetMobilesInRange(10);
            int npcCount = 0;
            foreach (Mobile m in eable)
            {
                if (m is ILLMConversational && m != npc && m.Alive)
                {
                    npcCount++;
                }
            }
            eable.Free();

            // If no other LLM NPCs nearby, respond to everything
            if (npcCount == 0)
                return true;

            // Multiple NPCs nearby, name must be mentioned
            return false;
        }
    }
}

