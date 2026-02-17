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

            // PROACTIVE RAG: Load knowledge base at spawn time using shared helper
            // Note: Will reload when placed in world if location is invalid now
            LLMConversationHelper.ProactivelyLoadKnowledgeBase(this, this);
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
            // Only reload if we're moving from Internal map (initial spawn)
            if (oldMap == Map.Internal && map != null && map != Map.Internal)
            {
                LLMConversationHelper.ProactivelyLoadKnowledgeBase(this, this);
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
            _ = MaxQueueSize;
            _ = m_IsProcessingRequest;
            LLMConversationHelper.ProcessConversation(this, player, message);
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
                    if (m_PersonalityType != suggestedPersonality)
                    {
                        Console.WriteLine($"[LLMNpc] Re-inferred personality for {Name} from {m_PersonalityType} to {suggestedPersonality}");
                        m_PersonalityType = suggestedPersonality;
                        System.Threading.Interlocked.Increment(ref m_ReInferredCount);
                        LLMConversationHelper.ProactivelyLoadKnowledgeBase(this, this);
                    }
                }
            }
        }
    }
}
