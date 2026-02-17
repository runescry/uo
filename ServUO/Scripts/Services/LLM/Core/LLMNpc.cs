using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Services.LLM;

namespace Server.Mobiles
{
    /// <summary>
    /// An NPC that uses LLM (OpenAI) to generate conversational responses
    /// </summary>
    public class LLMNpc : BaseVendor
    {
        private string m_Personality;
        private NPCPersonalities.PersonalityType m_PersonalityType;
        private NPCPersonalities.SpeechPattern m_SpeechPattern;
        private bool m_UsePersonalitySystem; // Use new personality system vs. freeform
        private int m_HearingRange;
        private int m_MaxChunkLength;
        private int m_DelayBetweenChunks;
        private HashSet<Mobile> m_ProcessingPlayers; // Track which players are being processed
        private Dictionary<Mobile, DateTime> m_ActiveConversations; // Track active conversations with players
        private Type[] m_VendorInventory; // Store vendor inventory types for InitSBInfo
        private static bool m_DebugMode = true; // Enable performance timing
        private static int m_ReInferredCount = 0; // Track personality re-inferences during world load

        // Request throttling/queuing system
        private Queue<SpeechRequest> m_RequestQueue; // Queue of pending speech requests
        private bool m_IsProcessingRequest; // Flag to track if currently processing a request
        private readonly object m_QueueLock = new object(); // Lock for thread-safe queue operations
        private static readonly int MaxQueueSize = 5; // Maximum requests in queue per NPC
        private static readonly TimeSpan QueueTimeout = TimeSpan.FromSeconds(30); // Timeout for queued requests

        // Proactive RAG: Pre-loaded knowledge base (loaded once at spawn, not per query)
        private List<LoreEntry> m_KnowledgeBase;
        private string m_FormattedKnowledge;

        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;

        [CommandProperty(AccessLevel.GameMaster)]
        public string Personality
        {
            get { return m_Personality; }
            set { m_Personality = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public new NPCPersonalities.PersonalityType PersonalityType
        {
            get { return m_PersonalityType; }
            set { m_PersonalityType = value; m_UsePersonalitySystem = true; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public new NPCPersonalities.SpeechPattern SpeechPattern
        {
            get { return m_SpeechPattern; }
            set { m_SpeechPattern = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public static bool DebugMode
        {
            get { return m_DebugMode; }
            set { m_DebugMode = value; }
        }

        /// <summary>
        /// Get count of NPCs that had personality re-inferred during world load
        /// </summary>
        public static int GetReInferredCount()
        {
            return m_ReInferredCount;
        }

        /// <summary>
        /// Reset re-inference counter (called after logging summary)
        /// </summary>
        public static void ResetReInferredCount()
        {
            m_ReInferredCount = 0;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool UsePersonalitySystem
        {
            get { return m_UsePersonalitySystem; }
            set { m_UsePersonalitySystem = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public new int HearingRange
        {
            get { return m_HearingRange; }
            set { m_HearingRange = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxChunkLength
        {
            get { return m_MaxChunkLength; }
            set { m_MaxChunkLength = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int DelayBetweenChunks
        {
            get { return m_DelayBetweenChunks; }
            set { m_DelayBetweenChunks = value; }
        }

        [Constructable]
        public LLMNpc() : this("an AI mystic", "You are a mysterious mystic who speaks in riddles and offers cryptic wisdom.")
        {
        }

        /// <summary>
        /// Constructor using new personality system
        /// </summary>
        [Constructable]
        public LLMNpc(string name, NPCPersonalities.PersonalityType personalityType, NPCPersonalities.SpeechPattern speechPattern) : base(null)
        {
            Name = name;
            m_PersonalityType = personalityType;
            m_SpeechPattern = speechPattern;
            m_UsePersonalitySystem = true;
            m_HearingRange = 10;
            m_MaxChunkLength = 100;
            m_DelayBetweenChunks = 1500;
            m_ProcessingPlayers = new HashSet<Mobile>();
            m_ActiveConversations = new Dictionary<Mobile, DateTime>();
            m_RequestQueue = new Queue<SpeechRequest>();
            m_IsProcessingRequest = false;

            // Auto-suggest personality if not explicitly set
            if (personalityType == NPCPersonalities.PersonalityType.Commoner)
            {
                m_PersonalityType = NPCPersonalities.SuggestPersonality(name);
            }

            // Set up vendor inventory based on personality
            m_VendorInventory = GetVendorInventoryForPersonality(personalityType);
            if (m_VendorInventory != null && m_VendorInventory.Length > 0)
            {
                LoadSBInfo();
            }

            // Default appearance
            Body = Utility.RandomBool() ? 0x190 : 0x191; // Male or female
            Hue = Utility.RandomSkinHue();

            // Add personality-appropriate clothing
            NPCPersonalities.AddPersonalityClothing(this, personalityType);

            CantWalk = true;
            Blessed = true;

            SetStr(100);
            SetDex(100);
            SetInt(100);

            // PROACTIVE RAG: Load knowledge base at spawn time (not per query!)
            // Note: Will reload when placed in world if location is invalid now
            LoadKnowledgeBase();
        }

        /// <summary>
        /// Override MoveToWorld to reload knowledge base when NPC is placed in the world
        /// This ensures location-based knowledge is loaded correctly
        /// </summary>
        public override void MoveToWorld(Point3D loc, Map map)
        {
            Map oldMap = this.Map;
            Point3D oldLoc = this.Location;
            base.MoveToWorld(loc, map);
            
            // Reload knowledge base now that we have a valid location
            // Only reload if we're moving from Internal map (initial spawn) or if knowledge is empty
            if (oldMap == Map.Internal && map != null && map != Map.Internal)
            {
                LoadKnowledgeBase();
            }
            else if ((m_KnowledgeBase == null || m_KnowledgeBase.Count == 0) && map != null && map != Map.Internal)
            {
                LoadKnowledgeBase();
            }

            // Update location database and invalidate cache if NPC moved within the same map
            if (oldMap == map && map != null && map != Map.Internal && oldLoc != loc)
            {
                HandleLocationChange(oldLoc);
            }
        }

        /// <summary>
        /// Override OnLocationChange to handle wandering NPCs
        /// Invalidates cache and updates static database when NPCs move
        /// </summary>
        protected override void OnLocationChange(Point3D oldLocation)
        {
            base.OnLocationChange(oldLocation);
            
            // Only handle if we're in a valid map and actually moved
            if (this.Map != null && this.Map != Map.Internal && oldLocation != this.Location)
            {
                HandleLocationChange(oldLocation);
            }
        }

        /// <summary>
        /// Handle location change for wandering NPCs
        /// Only updates static database if NPC moved significantly (town NPCs don't wander far)
        /// </summary>
        private void HandleLocationChange(Point3D oldLocation)
        {
            try
            {
                // Only update if this NPC is tracked and moved significantly
                // Town NPCs stay within RangeHome (5-10 tiles), so small movements don't need updates
                if (this.LLMConversationEnabled)
                {
                    int dx = this.Location.X - oldLocation.X;
                    int dy = this.Location.Y - oldLocation.Y;
                    double distanceMoved = Math.Sqrt((dx * dx) + (dy * dy));
                    
                    // Only update if moved more than 5 tiles (significant movement)
                    // Small wandering within RangeHome doesn't need frequent updates
                    if (distanceMoved > 5)
                    {
                        NPCKnowledgeSystem.NPCRole role = NPCKnowledgeSystem.InferRoleFromPersonality(this.PersonalityType);
                        string townName = NPCPersonalities.GetLocationName(this);
                        
                        if (!string.IsNullOrEmpty(townName))
                        {
                            // Update location in static database
                            NPCLocationDatabase.UpdateLocation(this.Serial, this.Location);
                            
                            // Invalidate cache for this region (less frequent now due to higher TTL)
                            NPCPersonalities.InvalidateCacheForRegion(townName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LLMNpc] Error handling location change for {this.Name}: {ex.Message}");
            }
        }

        /// <summary>
        /// PROACTIVE RAG: Load NPC's knowledge base at spawn time
        /// This replaces per-query embedding searches with pre-loaded role-based knowledge
        /// </summary>
        private void LoadKnowledgeBase()
        {
            try
            {
                // Infer role from personality
                NPCKnowledgeSystem.NPCRole role = NPCKnowledgeSystem.InferRoleFromPersonality(m_PersonalityType);

                // Get location name for context
                string locationName = NPCPersonalities.GetLocationName(this);

                // Check if we have a valid location (not Internal map)
                if (this.Map == null || this.Map == Map.Internal)
                {
                    // Load only role-based knowledge for now, will reload when placed
                    var allLore = SimpleLoreSystem.GetAllLore();
                    var roleKnowledge = NPCKnowledgeSystem.GetRoleKnowledge(role, allLore);
                    // Also add dungeons and important locations
                    var locationKnowledge = allLore.Where(l =>
                        l.Category == "Dungeon" ||
                        (l.Category == "Location" && l.Importance >= 8)
                    ).ToList();
                    m_KnowledgeBase = new List<LoreEntry>();
                    m_KnowledgeBase.AddRange(roleKnowledge);
                    m_KnowledgeBase.AddRange(locationKnowledge);
                    m_KnowledgeBase = m_KnowledgeBase.GroupBy(l => l.ID).Select(g => g.First()).ToList();
                }
                else
                {
                // Load knowledge base (role + location specific)
                m_KnowledgeBase = NPCKnowledgeSystem.GetNPCKnowledge(role, locationName, this.Location, this.Map);
                }

                // Pre-format for prompts (cache it so we don't rebuild every conversation)
                m_FormattedKnowledge = NPCKnowledgeSystem.FormatKnowledgeForPrompt(m_KnowledgeBase);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LLMNpc] Error loading knowledge base for {Name}: {ex.Message}");
                Console.WriteLine($"[LLMNpc] Stack trace: {ex.StackTrace}");
                m_KnowledgeBase = new List<LoreEntry>();
                m_FormattedKnowledge = "";
            }
        }

        /// <summary>
        /// Get vendor inventory types based on personality
        /// </summary>
        private Type[] GetVendorInventoryForPersonality(NPCPersonalities.PersonalityType personality)
        {
            switch (personality)
            {
                case NPCPersonalities.PersonalityType.Merchant:
                    return new Type[] { typeof(Server.Mobiles.SBBlacksmith) }; // Generic merchant items
                case NPCPersonalities.PersonalityType.Mage:
                    return new Type[] { typeof(Server.Mobiles.SBMage) };
                case NPCPersonalities.PersonalityType.Healer:
                    return new Type[] { typeof(Server.Mobiles.SBHealer) };
                default:
                    // Check if NPC name/title suggests a vendor profession
                    if (this.Name != null)
                    {
                        string lowerName = this.Name.ToLower();
                        if (lowerName.Contains("blacksmith") || lowerName.Contains("smith") ||
                            lowerName.Contains("armorer") || lowerName.Contains("weaponsmith"))
                        {
                            return new Type[] { typeof(Server.Mobiles.SBBlacksmith) };
                        }
                        if (lowerName.Contains("mage") || lowerName.Contains("wizard") || lowerName.Contains("sorcerer"))
                        {
                            return new Type[] { typeof(Server.Mobiles.SBMage) };
                        }
                        if (lowerName.Contains("healer") || lowerName.Contains("physician") || lowerName.Contains("cleric"))
                        {
                            return new Type[] { typeof(Server.Mobiles.SBHealer) };
                        }
                    }
                    return null; // No vendor functionality
            }
        }

        /// <summary>
        /// Add clothing appropriate for the personality
        /// </summary>

        [Constructable]
        public LLMNpc(string name, string personality) : base(null)
        {
            // Set the name directly to avoid BaseVendor's random name generation
            Name = name;
            m_Personality = personality;
            m_HearingRange = 10;
            m_MaxChunkLength = 100; // Characters per chunk
            m_DelayBetweenChunks = 1500; // Milliseconds between chunks
            m_ProcessingPlayers = new HashSet<Mobile>();
            m_ActiveConversations = new Dictionary<Mobile, DateTime>();
            m_RequestQueue = new Queue<SpeechRequest>();
            m_IsProcessingRequest = false;
            m_VendorInventory = null; // No vendor inventory for basic constructor

            // Default appearance - can be customized
            Body = 0x190; // Male body
            Hue = Utility.RandomSkinHue();

            // Give them a distinctive look
            AddItem(new Server.Items.Robe(Utility.RandomBlueHue()));
            AddItem(new Server.Items.WizardsHat(Utility.RandomBlueHue()));
            // Note: BaseVendor already adds shoes, so we don't add sandals to avoid conflict

            // Make them invulnerable and stationary
            CantWalk = true;
            Blessed = true;

            // Set stats
            SetStr(100);
            SetDex(100);
            SetInt(100);
        }

        /// <summary>
        /// Constructor that takes an NPCPersonality to set up vendor inventory
        /// </summary>
        [Constructable]
        public LLMNpc(string name, string personality, Type[] vendorInventory) : base(null)
        {
            Console.WriteLine($"[LLMNpc] Constructor called with vendorInventory: {(vendorInventory != null ? vendorInventory.Length.ToString() : "null")}");

            // Set the name directly to avoid BaseVendor's random name generation
            Name = name;
            m_Personality = personality;
            m_HearingRange = 10;
            m_MaxChunkLength = 100;
            m_DelayBetweenChunks = 1500;
            m_ProcessingPlayers = new HashSet<Mobile>();
            m_ActiveConversations = new Dictionary<Mobile, DateTime>();
            m_RequestQueue = new Queue<SpeechRequest>();
            m_IsProcessingRequest = false;
            m_VendorInventory = vendorInventory; // Store for InitSBInfo

            Console.WriteLine($"[LLMNpc] m_VendorInventory set, count: {(m_VendorInventory != null ? m_VendorInventory.Length.ToString() : "null")}");

            // Reload SBInfo now that m_VendorInventory is set
            // (base constructor already called LoadSBInfo but m_VendorInventory was null at that point)
            if (vendorInventory != null && vendorInventory.Length > 0)
            {
                Console.WriteLine($"[LLMNpc] Calling LoadSBInfo()...");
                LoadSBInfo();
                Console.WriteLine($"[LLMNpc] LoadSBInfo() completed. m_SBInfos count: {m_SBInfos.Count}");
            }
            else
            {
                Console.WriteLine($"[LLMNpc] Skipping LoadSBInfo - no vendor inventory");
            }

            // Default appearance - can be customized
            Body = 0x190;
            Hue = Utility.RandomSkinHue();

            // Give them a distinctive look
            AddItem(new Server.Items.Robe(Utility.RandomBlueHue()));
            AddItem(new Server.Items.WizardsHat(Utility.RandomBlueHue()));

            // Make them invulnerable and stationary
            CantWalk = true;
            Blessed = true;

            // Set stats
            SetStr(100);
            SetDex(100);
            SetInt(100);
        }

        public override void InitSBInfo()
        {
            // Initialize vendor inventory from stored types
            if (m_VendorInventory != null && m_VendorInventory.Length > 0)
            {
                Console.WriteLine($"[LLMNpc] InitSBInfo called for {Name}, adding {m_VendorInventory.Length} SBInfo types");
                foreach (Type sbType in m_VendorInventory)
                {
                    if (sbType != null)
                    {
                        try
                        {
                            SBInfo sbi = null;
                            // Try to create with Mobile parameter first (some SBInfo classes need it)
                            try
                            {
                                sbi = (SBInfo)Activator.CreateInstance(sbType, new object[] { this });
                                Console.WriteLine($"[LLMNpc] Created {sbType.Name} with Mobile parameter");
                            }
                            catch (Exception ex1)
                            {
                                Console.WriteLine($"[LLMNpc] Failed to create {sbType.Name} with Mobile param: {ex1.Message}");
                                // Fall back to parameterless constructor
                                try
                                {
                                    sbi = (SBInfo)Activator.CreateInstance(sbType);
                                    Console.WriteLine($"[LLMNpc] Created {sbType.Name} with parameterless constructor");
                                }
                                catch (Exception ex2)
                                {
                                    Console.WriteLine($"[LLMNpc] Failed to create {sbType.Name} with parameterless: {ex2.Message}");
                                    throw;
                                }
                            }

                            if (sbi != null)
                            {
                                m_SBInfos.Add(sbi);
                                Console.WriteLine($"[LLMNpc] Successfully added SBInfo: {sbType.Name}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[LLMNpc] Error creating SBInfo {sbType.Name}: {ex.Message}");
                            Console.WriteLine($"[LLMNpc] Stack trace: {ex.StackTrace}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Determines if this NPC should listen to speech from a mobile
        /// </summary>
        public override bool HandlesOnSpeech(Mobile from)
        {
            // Only listen to players within range
            if (from is PlayerMobile && from.InRange(this.Location, m_HearingRange) && from.Alive)
            {
                return true;
            }

            return base.HandlesOnSpeech(from);
        }

        /// <summary>
        /// Called when this NPC hears speech
        /// </summary>
        public override void OnSpeech(SpeechEventArgs e)
        {
            Console.WriteLine($"[LLMNpc] OnSpeech called - Handled: {e.Handled}");

            if (e.Handled)
                return;

            // LLMNpc inherits from BaseVendor which implements ILLMConversational
            // Let the base class handle ALL conversation logic via the interface
            base.OnSpeech(e);

            // Base class will handle everything, don't process again
            Console.WriteLine($"[LLMNpc] base.OnSpeech() completed, returning (base handles via ILLMConversational)");
            return;
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
        /// Enqueues a speech request for processing (throttled/queued system)
        /// </summary>
        private void EnqueueSpeechRequest(Mobile player, string message)
        {
            lock (m_QueueLock)
            {
                // Check if player already has a request in queue OR is currently being processed
                if (m_RequestQueue.Any(r => r.Player == player) || m_ProcessingPlayers.Contains(player))
                {
                    Console.WriteLine($"[LLMNpc] Player {player.Name} already has a request in queue or is being processed, skipping duplicate");
                    this.PrivateOverheadMessage(Network.MessageType.Regular, 0x3B2, false, "*is already listening to you*", player.NetState);
                    return;
                }

                // Check queue size limit
                if (m_RequestQueue.Count >= MaxQueueSize)
                {
                    Console.WriteLine($"[LLMNpc] Queue full ({MaxQueueSize} requests), rejecting request from {player.Name}");
                    this.PrivateOverheadMessage(Network.MessageType.Regular, 0x3B2, false, "*is busy with others, please wait*", player.NetState);
                    return;
                }

                // Add to queue
                SpeechRequest request = new SpeechRequest(player, message);
                m_RequestQueue.Enqueue(request);
                Console.WriteLine($"[LLMNpc] Enqueued request from {player.Name}. Queue size: {m_RequestQueue.Count}");

                // Record queue wait time if not processing immediately
                if (m_IsProcessingRequest || m_RequestQueue.Count > 1)
                {
                    // Will be recorded when request is processed
                }

                // Show feedback based on queue position
                if (m_RequestQueue.Count == 1 && !m_IsProcessingRequest)
                {
                    // First in queue, will process immediately
                    this.PrivateOverheadMessage(Network.MessageType.Regular, 0x3B2, false, "*ponders*", player.NetState);
                }
                else
                {
                    // In queue, show position
                    int position = m_RequestQueue.Count;
                    this.PrivateOverheadMessage(Network.MessageType.Regular, 0x3B2, false, $"*acknowledges you ({position} in line)*", player.NetState);
                }
            }

            // Try to process the queue
            ProcessNextRequest();
        }

        /// <summary>
        /// Processes the next request in the queue (if not already processing)
        /// </summary>
        private void ProcessNextRequest()
        {
            lock (m_QueueLock)
            {
                // Don't process if already processing or queue is empty
                if (m_IsProcessingRequest || m_RequestQueue.Count == 0)
                    return;

                // Mark as processing
                m_IsProcessingRequest = true;

                // Clean up expired requests
                CleanupExpiredRequests();

                // Check if queue is still not empty after cleanup
                if (m_RequestQueue.Count == 0)
                {
                    m_IsProcessingRequest = false;
                    return;
                }

                // Get next request
                SpeechRequest request = m_RequestQueue.Dequeue();
                Console.WriteLine($"[LLMNpc] Processing request from {request.Player.Name}. Remaining in queue: {m_RequestQueue.Count}");

                // Process asynchronously
                ProcessSpeechAsync(request.Player, request.Message);
            }
        }

        /// <summary>
        /// Removes expired requests from the queue
        /// </summary>
        private void CleanupExpiredRequests()
        {
            List<SpeechRequest> validRequests = new List<SpeechRequest>();

            while (m_RequestQueue.Count > 0)
            {
                SpeechRequest request = m_RequestQueue.Dequeue();

                // Check if request is still valid
                if (request.Player == null || request.Player.Deleted || !request.Player.Alive)
                {
                    Console.WriteLine($"[LLMNpc] Removing expired request (player deleted/logged out)");
                    continue;
                }

                // Check if player is still in range
                if (!request.Player.InRange(this.Location, m_HearingRange))
                {
                    Console.WriteLine($"[LLMNpc] Removing expired request (player out of range)");
                    continue;
                }

                // Check if request has timed out
                if (DateTime.UtcNow - request.EnqueueTime > QueueTimeout)
                {
                    Console.WriteLine($"[LLMNpc] Removing expired request (timeout)");
                    if (!request.Player.Deleted && request.Player.Alive)
                    {
                        this.PrivateOverheadMessage(Network.MessageType.Regular, 0x3B2, false, "*seems to have lost track of your question*", request.Player.NetState);
                    }
                    continue;
                }

                // Request is still valid
                validRequests.Add(request);
            }

            // Re-add valid requests
            foreach (var request in validRequests)
            {
                m_RequestQueue.Enqueue(request);
            }
        }

        /// <summary>
        /// Processes player speech and generates an LLM response
        /// </summary>
        private async void ProcessSpeechAsync(Mobile player, string message)
        {
            DateTime startTime = DateTime.UtcNow;
            DateTime beforeLLM = DateTime.UtcNow;
            DateTime afterLLM = DateTime.UtcNow;

            try
            {
                if (m_DebugMode)
                    Console.WriteLine($"[LLMNpc] ===============================================");

                Console.WriteLine($"[LLMNpc] ProcessSpeechAsync started for player: {player.Name}, message: '{message}'");

                // Mark player as being processed (prevents duplicate requests)
                m_ProcessingPlayers.Add(player);

                // Check if first conversation BEFORE adding message (critical for performance)
                List<ConversationMessage> historyBefore = ConversationContext.GetHistory(this, player);
                bool isFirstConversation = historyBefore == null || historyBefore.Count == 0;

                // Add player message to conversation history
                ConversationContext.AddPlayerMessage(this, player, message);

                // Get conversation history (now includes the message we just added)
                List<ConversationMessage> history = ConversationContext.GetHistory(this, player);
                Console.WriteLine($"[LLMNpc] Conversation history length: {history.Count}, isFirstConversation: {isFirstConversation}");

                // Call unified LLM service (will route to best provider)
                Console.WriteLine($"[LLMNpc] Calling LLM API...");
                bool isVendor = m_VendorInventory != null && m_VendorInventory.Length > 0;
                
                // Also check if NPC has SBInfos loaded (might be a vendor even if m_VendorInventory is null)
                if (!isVendor && m_SBInfos != null && m_SBInfos.Count > 0)
                {
                    isVendor = true;
                    Console.WriteLine($"[LLMNpc] Detected vendor via SBInfos count: {m_SBInfos.Count}");
                }
                
                Console.WriteLine($"[LLMNpc] isVendor={isVendor}, m_VendorInventory={(m_VendorInventory != null ? m_VendorInventory.Length.ToString() : "null")}, m_SBInfos={m_SBInfos.Count}");

                beforeLLM = DateTime.UtcNow;
                if (m_DebugMode)
                {
                    TimeSpan setupTime = beforeLLM - startTime;
                    Console.WriteLine($"[LLMNpc] [TIMING] Setup time: {setupTime.TotalMilliseconds:F0}ms");
                }

                // Build personality string (use new system or legacy freeform)
                string personalityPrompt = m_Personality;
                if (m_UsePersonalitySystem)
                {
                    personalityPrompt = NPCPersonalities.GetPersonalityPrompt(m_PersonalityType, m_SpeechPattern);
                }

                // Add contextual awareness for ALL NPCs (new and old)
                personalityPrompt += NPCPersonalities.GetContextualInfo(this, player);

                // PROACTIVE RAG: Use pre-loaded knowledge instead of per-query search!
                // If knowledge is empty, try to reload it (might have been created before knowledge loading was fixed)
                if (string.IsNullOrEmpty(m_FormattedKnowledge) && (m_KnowledgeBase == null || m_KnowledgeBase.Count == 0))
                {
                    Console.WriteLine($"[LLMNpc] WARNING: Knowledge base is empty for {Name}, attempting to reload...");
                    LoadKnowledgeBase();
                }
                
                // Load memories and relationship (if memory system is available)
                string memoriesText = "";
                if (LLMMemoryService.IsAvailable() && !isFirstConversation)
                {
                    try
                    {
                        DateTime memoryLoadStart = DateTime.UtcNow;
                        // CRITICAL: Always use this.Serial to ensure memories are NPC-specific
                        // Memories are keyed by (npc_serial, player_name) - each NPC instance has a unique serial
                        var memories = await LLMMemoryService.GetMemoriesAsync(this.Serial, player.Name, limit: 5);
                        var relationship = await LLMMemoryService.GetRelationshipAsync(this.Serial, player.Name);
                        long memoryLoadTime = (long)(DateTime.UtcNow - memoryLoadStart).TotalMilliseconds;
                        
                        // Verify all loaded memories belong to this NPC (defensive check)
                        foreach (var mem in memories)
                        {
                            if (mem.NpcSerial != this.Serial)
                            {
                                Console.WriteLine($"[LLMNpc] ERROR: Memory with serial {mem.NpcSerial} loaded for NPC {this.Serial} - memory belongs to different NPC!");
                            }
                        }
                        
                        // Only load relationship if we have memories - prevents stale relationships from previous NPC instances
                        // A relationship without memories suggests the NPC was respawned/recreated
                        if (memories.Count > 0)
                        {
                            Console.WriteLine($"[LLMNpc] Loaded {memories.Count} memories for NPC {this.Serial} (Name: {this.Name}) and player {player.Name} in {memoryLoadTime}ms");
                            
                            memoriesText = MemoryHelpers.FormatMemoriesForPrompt(memories, maxMemories: 5);
                            
                            // Only include relationship if we have actual memories (prevents stale data from previous NPC instances)
                            // Also verify relationship belongs to this NPC
                            if (relationship != null && relationship.NpcSerial == this.Serial)
                            {
                                memoriesText += MemoryHelpers.FormatRelationshipForPrompt(relationship);
                            }
                            else if (relationship != null && relationship.NpcSerial != this.Serial)
                            {
                                Console.WriteLine($"[LLMNpc] ERROR: Relationship with serial {relationship.NpcSerial} loaded for NPC {this.Serial} - relationship belongs to different NPC!");
                            }
                        }
                        else if (relationship != null)
                        {
                            // Relationship exists but no memories - this is likely a stale relationship from a previous NPC instance
                            // Don't load it for a freshly spawned NPC
                            // Also verify it's not from a different NPC serial
                            if (relationship.NpcSerial != this.Serial)
                            {
                                Console.WriteLine($"[LLMNpc] ERROR: Relationship with serial {relationship.NpcSerial} found for NPC {this.Serial} - ignoring (belongs to different NPC)");
                            }
                            else
                            {
                                Console.WriteLine($"[LLMNpc] Found relationship but no memories for NPC {this.Serial} - ignoring stale relationship (NPC may have been respawned)");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"[LLMNpc] No memories found for NPC {this.Serial} (Name: {this.Name}) and player {player.Name} (load time: {memoryLoadTime}ms)");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[LLMNpc] Error loading memories: {ex.Message}");
                    }
                }
                else if (!LLMMemoryService.IsAvailable())
                {
                    Console.WriteLine($"[LLMNpc] Memory system not available (Redis/PostgreSQL not connected)");
                }
                
                // Append memories to personality prompt if we have any
                if (!string.IsNullOrEmpty(memoriesText))
                {
                    personalityPrompt += memoriesText;
                }
                
                beforeLLM = DateTime.UtcNow;
                
                // Debug: Log knowledge being sent
                if (m_DebugMode)
                {
                    Console.WriteLine($"[LLMNpc] Sending knowledge to LLM: {(string.IsNullOrEmpty(m_FormattedKnowledge) ? "EMPTY" : $"{m_FormattedKnowledge.Length} chars")}");
                    if (!string.IsNullOrEmpty(m_FormattedKnowledge))
                    {
                        Console.WriteLine($"[LLMNpc] Knowledge preview: {m_FormattedKnowledge.Substring(0, Math.Min(500, m_FormattedKnowledge.Length))}...");
                    }
                    if (!string.IsNullOrEmpty(memoriesText))
                    {
                        Console.WriteLine($"[LLMNpc] Memories preview: {memoriesText.Substring(0, Math.Min(200, memoriesText.Length))}...");
                    }
                }
                
                // Pass isFirstConversation flag to optimize LLM call
                string response = await UnifiedLLMService.GetResponseAsync(
                    this.Name,
                    personalityPrompt,
                    history,
                    message,
                    player.Name,
                    m_FormattedKnowledge, // Pre-loaded knowledge (no search needed!)
                    UnifiedLLMService.RequestType.PlayerConversation,
                    UnifiedLLMService.LLMProvider.Auto,
                    isVendor,
                    isFirstConversation // Pass flag to skip embeddings on first conversation
                );
                afterLLM = DateTime.UtcNow;

                if (m_DebugMode)
                {
                    long llmResponseTime = (long)(afterLLM - beforeLLM).TotalMilliseconds;
                    Console.WriteLine($"[LLMNpc] [TIMING] LLM response time: {llmResponseTime}ms");
                }

                Console.WriteLine($"[LLMNpc] Received response: '{response}'");

                // Check for vendor intent commands in the response
                VendorAction vendorAction = DetectVendorIntent(response);

                // Add NPC response to history
                ConversationContext.AddNpcMessage(this, player, response);

                // Extract and save memories from this conversation (async, non-blocking)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        List<ConversationMessage> fullHistory = ConversationContext.GetHistory(this, player);
                        List<Memory> memories = MemoryHelpers.ExtractMemoriesFromConversation(this.Name, player.Name, fullHistory);
                        
                        if (memories.Count > 0)
                        {
                            Console.WriteLine($"[LLMNpc] Extracted {memories.Count} memories from conversation with {player.Name}");
                            foreach (var memory in memories)
                            {
                                await LLMMemoryService.SaveMemoryAsync(this.Serial, this.Name, player.Name, memory);
                                Console.WriteLine($"[LLMNpc] Saved memory: {memory.Content.Substring(0, Math.Min(50, memory.Content.Length))}... (Importance: {memory.Importance})");
                            }
                            
                            // Update relationship based on conversation
                            await LLMMemoryService.UpdateRelationshipAsync(this.Serial, this.Name, player.Name, 1);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[LLMNpc] Error saving memories: {ex.Message}");
                    }
                });

                // Schedule the response to be said on the main server thread
                Timer.DelayCall(TimeSpan.Zero, () =>
                {
                    Console.WriteLine($"[LLMNpc] Sending response to player {player.Name}");

                    // Make sure player is still around and remove from processing list
                    m_ProcessingPlayers.Remove(player);

                    // Mark as no longer processing and trigger next request
                    lock (m_QueueLock)
                    {
                        m_IsProcessingRequest = false;
                    }

                    if (!player.Deleted && player.Alive && player.InRange(this.Location, m_HearingRange))
                    {
                        // Check if this is a SELL action but player has nothing to sell
                        if (vendorAction == VendorAction.Sell && !PlayerHasItemsToSell(player))
                        {
                            Console.WriteLine($"[LLMNpc] Player has nothing to sell, skipping LLM response, letting system message show");
                            // Just execute the vendor action - it will show the system message
                            ExecuteVendorAction(player, vendorAction);
                            return;
                        }

                        // Strip vendor command markers from response before displaying
                        string displayResponse = StripVendorCommands(response);
                        if (displayResponse != response && m_DebugMode)
                        {
                            Console.WriteLine($"[LLMNpc] Stripped meta-commentary. Original length: {response.Length}, Cleaned length: {displayResponse.Length}");
                        }

                        // Break long responses into chunks
                        SendChunkedResponse(player, displayResponse);
                        Console.WriteLine($"[LLMNpc] Response sent successfully");

                        if (m_DebugMode)
                        {
                            TimeSpan totalTime = DateTime.UtcNow - startTime;
                            Console.WriteLine($"[LLMNpc] [TIMING] Total response time: {totalTime.TotalMilliseconds:F0}ms");
                            Console.WriteLine($"[LLMNpc] ===============================================");
                        }

                        // Execute vendor action if detected
                        if (vendorAction != VendorAction.None)
                        {
                            Timer.DelayCall(TimeSpan.FromMilliseconds(500), () =>
                            {
                                ExecuteVendorAction(player, vendorAction);
                            });
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[LLMNpc] Player no longer valid or in range");
                    }

                    // Process next request in queue
                    ProcessNextRequest();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LLMNpc] Error processing speech: {ex.Message}");
                Console.WriteLine($"[LLMNpc] Stack trace: {ex.StackTrace}");

                Timer.DelayCall(TimeSpan.Zero, () =>
                {
                    m_ProcessingPlayers.Remove(player);

                    // Mark as no longer processing
                    lock (m_QueueLock)
                    {
                        m_IsProcessingRequest = false;
                    }

                    if (!player.Deleted && player.Alive && player.InRange(this.Location, m_HearingRange))
                    {
                        this.SayTo(player, "I apologize, but my mind seems clouded at the moment.", 0x3B2);
                    }

                    // Process next request in queue
                    ProcessNextRequest();
                });
            }
        }

        /// <summary>
        /// Vendor action types
        /// </summary>
        private enum VendorAction
        {
            None,
            Buy,
            Sell
        }

        /// <summary>
        /// Detects vendor intent commands in LLM response
        /// </summary>
        private VendorAction DetectVendorIntent(string response)
        {
            if (string.IsNullOrEmpty(response))
                return VendorAction.None;

            string lower = response.ToLower();

            // Check for vendor command markers (both brackets and parentheses)
            if (lower.Contains("[vendor_buy]") || lower.Contains("(vendor_buy)"))
                return VendorAction.Buy;
            if (lower.Contains("[vendor_sell]") || lower.Contains("(vendor_sell)"))
                return VendorAction.Sell;

            return VendorAction.None;
        }

        /// <summary>
        /// Strips vendor command markers and meta-commentary from response text
        /// </summary>
        private string StripVendorCommands(string response)
        {
            if (string.IsNullOrEmpty(response))
                return response;

            // AGGRESSIVE: Strip everything after ANY markdown header (### or ####)
            // NPCs should never use markdown formatting
            int markdownIndex = response.IndexOf("###", StringComparison.OrdinalIgnoreCase);
            if (markdownIndex >= 0)
            {
                response = response.Substring(0, markdownIndex).Trim();
            }

            // Remove meta-commentary sections (EXPLAINER, EXPLANATION, NOTE, SOLUTION, FOLLOW-UP QUESTIONS, etc.)
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
                    lowerResponse = response.ToLower(); // Update for next iteration
                }
            }

            // Also check for line breaks before meta tags and separator lines
            string[] lines = response.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++)
            {
                string trimmedLine = lines[i].Trim();
                
                // Check for separator lines (---, ===, etc.)
                if (trimmedLine.StartsWith("---") && trimmedLine.Length > 3)
                {
                    // Check if it's a separator with meta-content
                    string withoutDashes = trimmedLine.Replace("-", "").Trim();
                    string lowerLine = trimmedLine.ToLower();
                    if (withoutDashes.Length == 0 || // Pure separator
                        lowerLine.Contains("solution") ||
                        lowerLine.Contains("follow-up") ||
                        lowerLine.Contains("question") ||
                        lowerLine.Contains("meta"))
                    {
                        // Take everything before this separator line
                        response = string.Join("\n", lines.Take(i)).Trim();
                        goto done;
                    }
                }
                
                // Check for markdown-style headers (###, ####) that indicate meta-commentary
                // Strip ANY markdown header - NPCs shouldn't use markdown formatting
                if (trimmedLine.StartsWith("###"))
                {
                    response = string.Join("\n", lines.Take(i)).Trim();
                    goto done;
                }
                
                // Also check for lines that START with markdown (even if not trimmed properly)
                if (lines[i].TrimStart().StartsWith("###"))
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
                        // Take everything before this line
                        response = string.Join("\n", lines.Take(i)).Trim();
                        goto done; // Break out of nested loops
                    }
                }
                
                // Remove lines that look like meta-commentary (e.g., "Gareth is aware...")
                string lowerLineForMeta = trimmedLine.ToLower();
                if (lowerLineForMeta.Contains("is aware") ||
                    lowerLineForMeta.Contains("currently discussing") ||
                    lowerLineForMeta.Contains("the llm") ||
                    lowerLineForMeta.Contains("relevant and irrelevant factor") ||
                    lowerLineForMeta.Contains("textbook-level") ||
                    (lowerLineForMeta.Contains("elaborated") && lowerLineForMeta.Contains("solution")))
                {
                    response = string.Join("\n", lines.Take(i)).Trim();
                    goto done;
                }
            }
            done:

            return response
                .Replace("[VENDOR_BUY]", "")
                .Replace("[vendor_buy]", "")
                .Replace("[VENDOR_SELL]", "")
                .Replace("[vendor_sell]", "")
                .Replace("(VENDOR_BUY)", "")  // LLM sometimes uses parentheses
                .Replace("(vendor_buy)", "")
                .Replace("(VENDOR_SELL)", "")
                .Replace("(vendor_sell)", "")
                .Trim();
        }

        /// <summary>
        /// Checks if player has any items the vendor would buy
        /// </summary>
        private bool PlayerHasItemsToSell(Mobile player)
        {
            Container pack = player.Backpack;
            if (pack == null)
                return false;

            var info = GetSellInfo();

            foreach (IShopSellInfo ssi in info)
            {
                var items = pack.FindItemsByType(ssi.Types);

                foreach (Item item in items)
                {
                    if (item is Container && ((Container)item).Items.Count != 0)
                        continue;

                    if (item.IsStandardLoot() && item.Movable && ssi.IsSellable(item))
                        return true; // Found at least one item
                }
            }

            return false; // No items found
        }

        /// <summary>
        /// Executes vendor action (open buy/sell gump)
        /// </summary>
        private void ExecuteVendorAction(Mobile player, VendorAction action)
        {
            if (action == VendorAction.Buy)
            {
                Console.WriteLine($"[LLMNpc] Opening vendor buy for {player.Name}");
                Console.WriteLine($"[LLMNpc] SBInfos count: {m_SBInfos.Count}");
                if (m_SBInfos.Count == 0)
                {
                    Console.WriteLine($"[LLMNpc] WARNING: No SBInfo items! Vendor has no inventory.");
                    this.SayTo(player, "I seem to have misplaced my wares...", 0x3B2);
                    return;
                }
                this.VendorBuy(player);
            }
            else if (action == VendorAction.Sell)
            {
                Console.WriteLine($"[LLMNpc] Opening vendor sell for {player.Name}");
                this.VendorSell(player);
            }
        }

        /// <summary>
        /// Checks if the player has an active conversation with this NPC
        /// Active = spoke to this NPC within the last 2 minutes
        /// </summary>
        private bool IsInActiveConversation(Mobile player)
        {
            if (m_ActiveConversations.TryGetValue(player, out DateTime lastInteraction))
            {
                TimeSpan timeSinceLastMessage = DateTime.UtcNow - lastInteraction;
                if (timeSinceLastMessage.TotalMinutes < 2)
                {
                    Console.WriteLine($"[LLMNpc] {player.Name} is in active conversation with {this.Name}");
                    return true;
                }
                else
                {
                    // Conversation expired, remove from active list
                    m_ActiveConversations.Remove(player);
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if player named a different LLM NPC nearby
        /// </summary>
        private bool PlayerNamedDifferentNPC(Mobile player, string speech)
        {
            if (string.IsNullOrEmpty(speech))
                return false;

            string lowerSpeech = speech.ToLower();

            // Check all nearby LLM NPCs
            IPooledEnumerable eable = this.GetMobilesInRange(m_HearingRange);
            bool namedOther = false;

            foreach (Mobile m in eable)
            {
                if (m is LLMNpc otherNpc && m != this && m.Alive)
                {
                    // Check if other NPC's name is mentioned (with word boundaries)
                    string otherLowerName = otherNpc.Name.ToLower();

                    // Check full name
                    if (ContainsWord(lowerSpeech, otherLowerName))
                    {
                        namedOther = true;
                        break;
                    }

                    // Check first name
                    string[] otherNameParts = otherNpc.Name.Split(' ');
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
        /// Checks if a word exists in text with word boundaries (not as part of another word)
        /// </summary>
        private bool ContainsWord(string text, string word)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(word))
                return false;

            // Use regex for word boundary matching
            string pattern = @"\b" + System.Text.RegularExpressions.Regex.Escape(word) + @"\b";
            return System.Text.RegularExpressions.Regex.IsMatch(text, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Checks if the NPC's name was mentioned in the speech
        /// </summary>
        private bool WasNamed(string speech)
        {
            if (string.IsNullOrEmpty(speech) || string.IsNullOrEmpty(this.Name))
                return false;

            string lowerSpeech = speech.ToLower();
            string lowerName = this.Name.ToLower();

            // Check if full name is mentioned (with word boundaries)
            if (ContainsWord(lowerSpeech, lowerName))
                return true;

            // Check if first name is mentioned (e.g. "Aldric" from "Aldric Ashwood the Blacksmith")
            string[] nameParts = this.Name.Split(' ');
            if (nameParts.Length > 0)
            {
                string firstName = nameParts[0].ToLower();
                // Only match if it's a standalone word, not part of another word
                if (ContainsWord(lowerSpeech, firstName))
                    return true;
            }

            // Check if title is mentioned (e.g. "blacksmith" from "Aldric Ashwood the Blacksmith")
            // BUT: Only if player is alone with this NPC OR if they use the full name+title combo
            if (nameParts.Length > 2)
            {
                // Assume title is after "the"
                for (int i = 0; i < nameParts.Length; i++)
                {
                    if (nameParts[i].ToLower() == "the" && i + 1 < nameParts.Length)
                    {
                        string title = nameParts[i + 1].ToLower();

                        // Only respond to title if:
                        // 1. Player mentioned first name + title together, OR
                        // 2. Player is alone with this NPC (no other NPCs nearby)
                        if (nameParts.Length > 0)
                        {
                            string firstName = nameParts[0].ToLower();
                            // Check if both first name and title are mentioned
                            if (ContainsWord(lowerSpeech, firstName) && ContainsWord(lowerSpeech, title))
                                return true;
                        }
                    }
                }
            }

            // If player is alone with this NPC, respond to anything
            IPooledEnumerable eable = this.GetMobilesInRange(m_HearingRange);
            int npcCount = 0;
            foreach (Mobile m in eable)
            {
                if (m is LLMNpc && m != this && m.Alive)
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

        /// <summary>
        /// Sends a response to the player, breaking it into chunks if it's too long
        /// </summary>
        private void SendChunkedResponse(Mobile player, string response)
        {
            if (string.IsNullOrEmpty(response))
                return;

            // If response is short enough, send it all at once
            if (response.Length <= m_MaxChunkLength)
            {
                this.SayTo(player, response, 0x3B2); // Light blue text
                return;
            }

            // Break into sentences first
            string[] sentences = response.Split(new[] { ". ", "! ", "? " }, StringSplitOptions.None);
            List<string> chunks = new List<string>();
            string currentChunk = "";

            foreach (string sentence in sentences)
            {
                string sentenceWithPunctuation = sentence;

                // Add back the punctuation if it was removed (except for last sentence)
                if (!sentence.EndsWith(".") && !sentence.EndsWith("!") && !sentence.EndsWith("?"))
                {
                    // Check what the original punctuation was
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
                if (currentChunk.Length + sentenceWithPunctuation.Length + 1 > m_MaxChunkLength && currentChunk.Length > 0)
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
                    if (!player.Deleted && player.Alive && player.InRange(this.Location, m_HearingRange))
                    {
                        this.SayTo(player, chunk, 0x3B2); // Light blue text
                    }
                });

                delay += m_DelayBetweenChunks;
            }
        }

        /// <summary>
        /// Double-click interaction - open paperdoll or greet
        /// </summary>
        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this.Location, 3))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            // Check if player is staff or wants to open paperdoll
            if (from.AccessLevel >= AccessLevel.GameMaster)
            {
                // Staff can always open paperdoll
                DisplayPaperdollTo(from);
            }
            else
            {
                // Regular players - just greet them
                this.SayTo(from, "Greetings, traveler. Speak to me if you wish to converse.", 0x3B2);
            }
        }

        /// <summary>
        /// Add context menu options
        /// </summary>
        public override void GetContextMenuEntries(Mobile from, List<Server.ContextMenus.ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from.Alive && from is PlayerMobile)
            {
                list.Add(new ResetConversationEntry(from, this));
                list.Add(new OpenPaperdollEntry(from, this));
            }
        }

        /// <summary>
        /// Context menu entry to reset conversation history
        /// </summary>
        private class ResetConversationEntry : Server.ContextMenus.ContextMenuEntry
        {
            private Mobile m_Player;
            private LLMNpc m_NPC;

            public ResetConversationEntry(Mobile player, LLMNpc npc) : base(6111, 3) // "Reset" with range 3
            {
                m_Player = player;
                m_NPC = npc;
            }

            public override void OnClick()
            {
                ConversationContext.ClearHistory(m_NPC, m_Player);
                m_NPC.SayTo(m_Player, "Let us start our conversation anew.", 0x3B2);
            }
        }

        /// <summary>
        /// Context menu entry to open paperdoll
        /// </summary>
        private class OpenPaperdollEntry : Server.ContextMenus.ContextMenuEntry
        {
            private Mobile m_Player;
            private LLMNpc m_NPC;

            public OpenPaperdollEntry(Mobile player, LLMNpc npc) : base(6123, 3) // "Open Paperdoll" with range 3
            {
                m_Player = player;
                m_NPC = npc;
            }

            public override void OnClick()
            {
                if (m_Player.InRange(m_NPC.Location, 3))
                {
                    m_NPC.DisplayPaperdollTo(m_Player);
                }
            }
        }

        public LLMNpc(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)2); // version

            writer.Write(m_Personality);
            writer.Write(m_HearingRange);
            writer.Write(m_MaxChunkLength);
            writer.Write(m_DelayBetweenChunks);

            // Version 2: Personality system
            writer.Write(m_UsePersonalitySystem);
            writer.Write((int)m_PersonalityType);
            writer.Write((int)m_SpeechPattern);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Personality = reader.ReadString();
            m_HearingRange = reader.ReadInt();

            if (version >= 1)
            {
                m_MaxChunkLength = reader.ReadInt();
                m_DelayBetweenChunks = reader.ReadInt();
            }
            else
            {
                m_MaxChunkLength = 100;
                m_DelayBetweenChunks = 1500;
            }

            if (version >= 2)
            {
                m_UsePersonalitySystem = reader.ReadBool();
                m_PersonalityType = (NPCPersonalities.PersonalityType)reader.ReadInt();
                m_SpeechPattern = (NPCPersonalities.SpeechPattern)reader.ReadInt();
            }
            else
            {
                m_UsePersonalitySystem = false;
                m_PersonalityType = NPCPersonalities.PersonalityType.Commoner;
                m_SpeechPattern = NPCPersonalities.SpeechPattern.Modern;
            }

            m_ProcessingPlayers = new HashSet<Mobile>();
            m_ActiveConversations = new Dictionary<Mobile, DateTime>();
            m_RequestQueue = new Queue<SpeechRequest>();
            m_IsProcessingRequest = false;

            // Re-infer personality from name on load (in case name contains profession or detection was improved)
            if (m_UsePersonalitySystem && !string.IsNullOrEmpty(this.Name))
            {
                NPCPersonalities.PersonalityType suggestedPersonality = NPCPersonalities.SuggestPersonality(this.Name);
                if (suggestedPersonality != NPCPersonalities.PersonalityType.Commoner)
                {
                    NPCPersonalities.PersonalityType oldPersonality = m_PersonalityType;
                    
                    // Only update and log if personality actually changed
                    if (suggestedPersonality != oldPersonality)
                    {
                        m_PersonalityType = suggestedPersonality;
                        System.Threading.Interlocked.Increment(ref m_ReInferredCount);
                        LoadKnowledgeBase();
                    }
                }
            }
        }
    }
}
