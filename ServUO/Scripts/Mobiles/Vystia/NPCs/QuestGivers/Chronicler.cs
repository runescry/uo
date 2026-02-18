using System;
using System.Linq;
using System.Threading.Tasks;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Services.LLM;
using Server.Custom.VystiaClasses.Quests;
using Server.Custom.VystiaClasses.Quests.Generation;

namespace Server.Mobiles
{
    /// <summary>
    /// The Chronicler - Player-accessible NPC that generates dynamic quests via LLM
    /// Location: Can be placed in major cities
    /// Personality: Sage, wise, story-focused
    /// </summary>
    public class Chronicler : BaseVendor, ILLMConversational
    {
        private System.Collections.Generic.List<SBInfo> m_SBInfos = new System.Collections.Generic.List<SBInfo>();
        protected override System.Collections.Generic.List<SBInfo> SBInfos => m_SBInfos;

        // ILLMConversational implementation
        public new bool LLMConversationEnabled { get; set; } = true;
        public new NPCPersonalities.PersonalityType PersonalityType { get; set; } = NPCPersonalities.PersonalityType.Sage;
        public new NPCPersonalities.SpeechPattern SpeechPattern { get; set; } = NPCPersonalities.SpeechPattern.Formal;
        public new int HearingRange { get; set; } = 8;

        [Constructable]
        public Chronicler() : base("the Chronicler")
        {
            Name = "Chronicler";
            Title = "the Chronicler";
            Body = 0x190; // Human male
            Hue = Utility.RandomSkinHue();

            SetupAppearance();

            SetStr(100, 150);
            SetDex(100, 150);
            SetInt(150, 200);

            SetHits(300, 400);
            SetMana(500, 700);

            SetSkill(SkillName.Magery, 100.0);
            SetSkill(SkillName.MagicResist, 100.0);
            SetSkill(SkillName.EvalInt, 100.0);

            Fame = 10000;
            Karma = 10000;
        }

        private void SetupAppearance()
        {
            AddItem(new Robe(0x4B9)); // Sage robe
            AddItem(new Sandals());
            AddItem(new WizardsHat(0x4B9));
        }

        public override void InitSBInfo()
        {
            // Chronicler doesn't sell items
        }

        public override bool CanTeach => false;

        public override void OnSpeech(SpeechEventArgs e)
        {
            base.OnSpeech(e);

            Mobile from = e.Mobile;

            if (!e.Handled && from.InRange(this, 3))
            {
                string speech = e.Speech.ToLower();

                // Keyword: "quest", "adventure", "story", "tale"
                if (speech.Contains("quest") || speech.Contains("adventure") || 
                    speech.Contains("story") || speech.Contains("tale") || 
                    speech.Contains("generate") || speech.Contains("create"))
                {
                    if (from is PlayerMobile pm)
                    {
                        OfferQuestGeneration(pm);
                        e.Handled = true;
                    }
                }
                else if (speech.Contains("greetings") || speech.Contains("hail") || speech.Contains("hello"))
                {
                    Say($"Greetings, {from.Name}. I am the Chronicler. Speak of 'quest' or 'adventure' if you seek a tale tailored to your journey.");
                    e.Handled = true;
                }
            }
        }

        private void OfferQuestGeneration(PlayerMobile pm)
        {
            Say($"Ah, {pm.Name}, you seek a new adventure! I shall weave a tale unique to your path. This will take a moment...");

            // Generate quest asynchronously with better error handling
            Task.Run(async () =>
            {
                try
                {
                    pm.SendMessage(68, "[Chronicler] Weaving your tale...");

                    string json = await LLMQuestGenerationService.GeneratePlanJsonAsync(pm);
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        // This should not happen with the new fallback system, but handle it gracefully
                        string friendlyMessage = ErrorClassifier.GetUserFriendlyMessage(ErrorClassifier.ErrorType.UnknownError);
                        pm.SendMessage(38, $"[Chronicler] {friendlyMessage}");
                        Say("The threads of fate are unclear at the moment. Let me try a different approach...");
                        
                        // Try again with a simpler request
                        await Task.Delay(1000);
                        json = await LLMQuestGenerationService.GeneratePlanJsonAsync(pm, "simple");
                    }

                    if (!string.IsNullOrWhiteSpace(json) && 
                        LLMQuestGenerationService.CreateFromPlanJson(pm, json, out int questId, out string error))
                    {
                        pm.SendMessage(68, $"[Chronicler] Your tale is ready! Check your quest log.");
                        Say("Your tale has been woven. May it guide you well, adventurer.");
                    }
                    else
                    {
                        // Provide user-friendly error message
                        string friendlyMessage = ErrorClassifier.GetUserFriendlyMessage(ErrorClassifier.ErrorType.ValidationError);
                        pm.SendMessage(38, $"[Chronicler] {friendlyMessage}");
                        Say("I apologize, but the tale could not be completed. Perhaps try again later.");
                    }
                }
                catch (Exception ex)
                {
                    // Categorize the error and provide appropriate feedback
                    var errorType = ErrorClassifier.ClassifyError(ex);
                    string friendlyMessage = ErrorClassifier.GetUserFriendlyMessage(errorType);
                    
                    pm.SendMessage(38, $"[Chronicler] {friendlyMessage}");
                    Console.WriteLine($"[Chronicler] Error generating quest: {ex.Message}");
                    
                    // Provide context-specific response
                    if (errorType == ErrorClassifier.ErrorType.NetworkError || errorType == ErrorClassifier.ErrorType.APIError)
                    {
                        Say("The great archives seem to be experiencing difficulties. Let me share a classic tale with you instead.");
                    }
                    else if (errorType == ErrorClassifier.ErrorType.RateLimitError)
                    {
                        Say("So many adventurers seek tales at once! Let me craft you a story from my memory.");
                    }
                    else
                    {
                        Say("I apologize, but the threads of fate are tangled. Perhaps another time.");
                    }
                }
            });
        }

        /// <summary>
        /// Build quest-aware context for the Chronicler to provide quest guidance
        /// </summary>
        public string BuildQuestAwareContext(PlayerMobile pm, string playerMessage = null)
        {
            if (pm == null)
                return "";

            // If player is requesting a new quest, ignore existing quests
            if (!string.IsNullOrWhiteSpace(playerMessage))
            {
                string msg = playerMessage.ToLower();
                if (msg.Contains("quest") || msg.Contains("adventure") || msg.Contains("story") || 
                    msg.Contains("tale") || msg.Contains("generate") || msg.Contains("create") ||
                    msg.Contains("new quest") || msg.Contains("new adventure"))
                {
                    // Player is requesting a new quest - don't mention existing quests
                    return "";
                }
            }

            var tracker = VystiaQuestTracker.GetTracker(pm);
            if (tracker == null)
                return "";

            var activeQuests = tracker.GetActiveQuests();
            if (activeQuests == null || activeQuests.Count == 0)
                return "";

            // Get the most recent active quest (or first one)
            int questID = activeQuests.LastOrDefault();
            if (questID == 0)
                questID = activeQuests.FirstOrDefault();

            var quest = DynamicQuestManager.GetQuest(questID);
            if (quest == null)
                return "";

            var progress = tracker.GetProgress(questID);
            var currentWaypoint = quest.GetCurrentWaypoint(progress);

            if (currentWaypoint == null)
                return "";

            string context = $"The player is currently on the quest \"{quest.Title}\".\n";
            context += $"Current objective: {currentWaypoint.Name ?? "Unknown"}\n";
            
            if (!string.IsNullOrEmpty(currentWaypoint.Description))
                context += $"Objective description: {currentWaypoint.Description}\n";

            // Add location hints
            if (!string.IsNullOrEmpty(currentWaypoint.PlayerLocationHint))
            {
                context += $"Location hint: {currentWaypoint.PlayerLocationHint}\n";
            }

            // Add NPC dialogue context if available
            if (!string.IsNullOrEmpty(currentWaypoint.NPCDialogueContext))
            {
                context += $"Context for this step: {currentWaypoint.NPCDialogueContext}\n";
            }

            // Add location info if available
            if (currentWaypoint.Location != Point3D.Zero && currentWaypoint.Map != null)
            {
                int dx = currentWaypoint.Location.X - pm.Location.X;
                int dy = currentWaypoint.Location.Y - pm.Location.Y;
                int dist = (int)Math.Sqrt(dx * dx + dy * dy);

                if (currentWaypoint.Map == pm.Map && dist > 0)
                {
                    string dir = "";
                    if (Math.Abs(dx) > Math.Abs(dy))
                        dir = dx > 0 ? "east" : "west";
                    else
                        dir = dy > 0 ? "south" : "north";

                    string rangePhrase;
                    if (dist <= 60)
                        rangePhrase = "a short walk";
                    else if (dist <= 200)
                        rangePhrase = "a fair walk";
                    else
                        rangePhrase = "far away from this place";

                    context += $"Directional hint: {dir} of here, {rangePhrase}.\n";
                }
            }

            context += "\nIMPORTANT: When the player asks 'where do i go?' or similar questions, provide guidance based on the current quest objective and location hints above. Do NOT give generic advice about healers or other NPCs unless they are part of the quest.";

            return context;
        }

        // ILLMConversational interface methods
        public new bool ShouldHandleConversation(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;
            return from.InRange(this, HearingRange) && from.Player;
        }

        public new void HandleConversation(SpeechEventArgs e)
        {
            LLMConversationHelper.ProcessConversation(this, e.Mobile, e.Speech);
            e.Handled = true;
        }

        public Chronicler(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version

            writer.Write(LLMConversationEnabled);
            writer.Write((int)PersonalityType);
            writer.Write((int)SpeechPattern);
            writer.Write(HearingRange);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 1)
            {
                LLMConversationEnabled = reader.ReadBool();
                PersonalityType = (NPCPersonalities.PersonalityType)reader.ReadInt();
                SpeechPattern = (NPCPersonalities.SpeechPattern)reader.ReadInt();
                HearingRange = reader.ReadInt();
            }
            else
            {
                LLMConversationEnabled = true;
                PersonalityType = NPCPersonalities.PersonalityType.Sage;
                SpeechPattern = NPCPersonalities.SpeechPattern.Formal;
                HearingRange = 8;
            }
        }
    }
}

