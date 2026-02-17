using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server;
using Server.Engines.Quests;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Services.LLM
{
    /// <summary>
    /// LLM-powered quest NPC that integrates with ServUO's quest system
    /// Uses LLM for natural dialogue while leveraging traditional quest mechanics
    /// </summary>
    public class LLMQuester : BaseQuester
    {
        private string m_Personality;
        private NPCPersonalities.PersonalityType m_PersonalityType;
        private NPCPersonalities.SpeechPattern m_SpeechPattern;
        private Type m_QuestType; // The quest this NPC offers
        private Dictionary<Mobile, QuestConversationContext> m_Conversations;

        [Constructable]
        public LLMQuester(string name, Type questType, NPCPersonalities.PersonalityType personalityType = NPCPersonalities.PersonalityType.Commoner, NPCPersonalities.SpeechPattern speechPattern = NPCPersonalities.SpeechPattern.Archaic)
            : base(null)
        {
            Name = name;
            m_QuestType = questType;
            m_PersonalityType = personalityType;
            m_SpeechPattern = speechPattern;
            m_Conversations = new Dictionary<Mobile, QuestConversationContext>();

            // Generate personality prompt
            m_Personality = NPCPersonalities.GetPersonalityPrompt(personalityType, speechPattern);

            // Initialize appearance based on personality
            InitBody();
            InitOutfit();
        }

        public LLMQuester(Serial serial) : base(serial)
        {
            m_Conversations = new Dictionary<Mobile, QuestConversationContext>();
        }

        #region Properties

        [CommandProperty(AccessLevel.GameMaster)]
        public new NPCPersonalities.PersonalityType PersonalityType
        {
            get { return m_PersonalityType; }
            set { m_PersonalityType = value; m_Personality = NPCPersonalities.GetPersonalityPrompt(value, m_SpeechPattern); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public new NPCPersonalities.SpeechPattern SpeechPattern
        {
            get { return m_SpeechPattern; }
            set { m_SpeechPattern = value; m_Personality = NPCPersonalities.GetPersonalityPrompt(m_PersonalityType, value); }
        }

        public override int GetAutoTalkRange(PlayerMobile pm)
        {
            return 3; // Auto-talk within 3 tiles
        }

        #endregion

        #region Quest Integration

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
            if (m_QuestType == null)
            {
                // No quest assigned, just have a conversation
                player.SendMessage("I have nothing for you at this time.");
                return;
            }

            // Check if player already has an active quest
            if (player.Quest != null)
            {
                if (player.Quest.GetType() == m_QuestType)
                {
                    // Player is on our quest - have LLM-driven conversation about quest state
                    ProcessQuestConversationAsync(player);
                }
                else
                {
                    // Player has a different quest
                    SayRandom(new string[]
                    {
                        "You seem busy with another task, friend. Return when you are free.",
                        "I see you have other matters to attend to. Come back later.",
                        "Your hands are full with another quest. Finish that first."
                    });
                }
                return;
            }

            // Check if player can receive the quest
            if (!CanOfferQuest(player))
            {
                SayRandom(new string[]
                {
                    "I have nothing for you right now.",
                    "You are not ready for what I have to offer.",
                    "Perhaps another time, friend."
                });
                return;
            }

            // Offer the quest via LLM dialogue
            OfferQuestWithLLMAsync(player);
        }

        /// <summary>
        /// Check if the player can receive this quest
        /// Override this for custom quest eligibility logic
        /// </summary>
        public virtual bool CanOfferQuest(PlayerMobile player)
        {
            return QuestSystem.CanOfferQuest(player, m_QuestType);
        }

        /// <summary>
        /// Offer quest using LLM-generated dialogue
        /// </summary>
        private async void OfferQuestWithLLMAsync(PlayerMobile player)
        {
            try
            {
                // Build context for quest offering
                StringBuilder context = new StringBuilder();
                context.AppendLine($"You are {Name}, and you are about to offer a quest to {player.Name}.");
                context.AppendLine($"Your personality: {m_Personality}");
                context.AppendLine();
                context.AppendLine("QUEST CONTEXT:");
                context.AppendLine($"Quest Name: {GetQuestName()}");
                context.AppendLine($"Quest Description: {GetQuestDescription()}");
                context.AppendLine();
                context.AppendLine("You should introduce yourself and hint at the quest naturally.");
                context.AppendLine("Make the player curious about what you need help with.");
                context.AppendLine("Keep your response to 2-4 sentences.");
                context.AppendLine();
                context.AppendLine("If the player seems interested, they will respond and you can reveal more details.");

                // Get LLM response
                string response = await GetLLMResponseAsync(context.ToString(), $"Hello, {Name}!", player.Name);

                Say(response);

                // Store context that quest is being offered
                GetOrCreateConversation(player).QuestOffered = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LLMQuester] Error offering quest: {ex.Message}");
                SayRandom(new string[]
                {
                    "I have a task for you. Would you be interested?",
                    "I need assistance with something. Can you help?"
                });
            }
        }

        /// <summary>
        /// Handle ongoing quest conversation with LLM awareness
        /// </summary>
        private async void ProcessQuestConversationAsync(PlayerMobile player)
        {
            QuestSystem quest = player.Quest;

            try
            {
                // Build quest state context
                StringBuilder context = new StringBuilder();
                context.AppendLine($"You are {Name}, and {player.Name} is working on your quest: {GetQuestName()}");
                context.AppendLine($"Your personality: {m_Personality}");
                context.AppendLine();
                context.AppendLine("CURRENT QUEST STATE:");

                // Add objective status
                if (quest.Objectives != null && quest.Objectives.Count > 0)
                {
                    for (int i = 0; i < quest.Objectives.Count; i++)
                    {
                        QuestObjective obj = (QuestObjective)quest.Objectives[i];
                        if (obj != null)
                        {
                            string status = obj.Completed ? "COMPLETED" : "IN PROGRESS";
                            context.AppendLine($"Objective {i + 1}: {obj.Message} [{status}]");
                            if (!obj.Completed && obj.MaxProgress > 0)
                            {
                                context.AppendLine($"  Progress: {obj.CurProgress}/{obj.MaxProgress}");
                            }
                        }
                    }
                }

                context.AppendLine();
                context.AppendLine("Respond naturally based on their progress.");
                context.AppendLine("If they've made progress, acknowledge it.");
                context.AppendLine("If they're stuck, give helpful hints.");
                context.AppendLine("Keep responses to 2-3 sentences.");

                // Get player's last message from conversation history
                QuestConversationContext conv = GetOrCreateConversation(player);
                string playerMessage = conv.LastPlayerMessage ?? "How's my progress?";

                string response = await GetLLMResponseAsync(context.ToString(), playerMessage, player.Name);

                Say(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LLMQuester] Error processing quest conversation: {ex.Message}");
                Say("Let me check on your progress...");
            }
        }

        #endregion

        #region Speech Handling

        public override void OnSpeech(SpeechEventArgs e)
        {
            base.OnSpeech(e);

            Mobile from = e.Mobile;
            if (from is PlayerMobile player && player.InRange(this, 10))
            {
                string speech = e.Speech.ToLower();

                // Store the player's message
                QuestConversationContext conv = GetOrCreateConversation(player);
                conv.LastPlayerMessage = e.Speech;
                conv.LastInteraction = DateTime.UtcNow;

                // Check for quest acceptance keywords
                if (conv.QuestOffered && (speech.Contains("yes") || speech.Contains("accept") || speech.Contains("help") || speech.Contains("sure")))
                {
                    // Player wants to accept the quest
                    TryOfferQuestGump(player);
                    conv.QuestOffered = false;
                    return;
                }

                // Check for quest decline keywords
                if (conv.QuestOffered && (speech.Contains("no") || speech.Contains("decline") || speech.Contains("not interested") || speech.Contains("maybe later")))
                {
                    SayRandom(new string[]
                    {
                        "Very well. If you change your mind, seek me out.",
                        "I understand. Return if you reconsider.",
                        "Perhaps another time then."
                    });
                    conv.QuestOffered = false;
                    return;
                }

                // Handle quest-related speech
                if (player.Quest != null && player.Quest.GetType() == m_QuestType)
                {
                    ProcessQuestConversationAsync(player);
                }
                else if (conv.QuestOffered)
                {
                    // Continue quest offering conversation
                    OfferQuestWithLLMAsync(player);
                }
            }
        }

        /// <summary>
        /// Show the traditional quest offer gump
        /// </summary>
        private void TryOfferQuestGump(PlayerMobile player)
        {
            try
            {
                QuestSystem quest = (QuestSystem)Activator.CreateInstance(m_QuestType, new object[] { player });
                if (quest != null)
                {
                    quest.SendOffer();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LLMQuester] Error creating quest: {ex.Message}");
                Say("I apologize, but I seem to have forgotten what I needed help with!");
            }
        }

        #endregion

        #region LLM Integration

        private async Task<string> GetLLMResponseAsync(string systemPrompt, string userMessage, string playerName)
        {
            // Use UnifiedLLM system to get response
            string fullPrompt = systemPrompt;

            // Add contextual information
            fullPrompt += NPCPersonalities.GetContextualInfo(this, null);

            try
            {
                // Determine which LLM service to use
                if (await LocalLLMService.IsAvailableAsync())
                {
                    return await LocalLLMService.GetResponseAsync(fullPrompt, userMessage, 150);
                }
                else
                {
                    // Fallback to OpenAI
                    List<ConversationMessage> history = new List<ConversationMessage>();
                    return await LLMService.GetResponseAsync(Name, fullPrompt, history, userMessage, playerName, false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LLMQuester] LLM Error: {ex.Message}");
                return "I seem to have lost my train of thought...";
            }
        }

        private QuestConversationContext GetOrCreateConversation(Mobile player)
        {
            if (!m_Conversations.ContainsKey(player))
            {
                m_Conversations[player] = new QuestConversationContext();
            }
            return m_Conversations[player];
        }

        #endregion

        #region Helper Methods

        private string GetQuestName()
        {
            try
            {
                QuestSystem tempQuest = (QuestSystem)Activator.CreateInstance(m_QuestType);
                return tempQuest.Name?.ToString() ?? m_QuestType.Name;
            }
            catch
            {
                return m_QuestType.Name;
            }
        }

        private string GetQuestDescription()
        {
            try
            {
                QuestSystem tempQuest = (QuestSystem)Activator.CreateInstance(m_QuestType);
                return tempQuest.OfferMessage?.ToString() ?? "A quest of great importance.";
            }
            catch
            {
                return "A quest of great importance.";
            }
        }

        private void SayRandom(string[] messages)
        {
            if (messages != null && messages.Length > 0)
            {
                Say(messages[Utility.Random(messages.Length)]);
            }
        }

        #endregion

        #region Appearance

        public override void InitBody()
        {
            InitStats(100, 100, 25);
            Female = Utility.RandomBool();
            Body = Female ? 0x191 : 0x190;
            Hue = Utility.RandomSkinHue();
            HairItemID = Utility.RandomList(0x203B, 0x203C, 0x203D, 0x2044, 0x2045, 0x204A);
            HairHue = Utility.RandomHairHue();

            if (!Female && Utility.RandomBool())
            {
                FacialHairItemID = Utility.RandomList(0x203E, 0x203F, 0x2040, 0x2041, 0x204B, 0x204C, 0x204D);
                FacialHairHue = HairHue;
            }
        }

        public override void InitOutfit()
        {
            // Add personality-appropriate clothing
            NPCPersonalities.AddPersonalityClothing(this, m_PersonalityType);
        }

        #endregion

        #region Serialization

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write(m_Personality);
            writer.Write((int)m_PersonalityType);
            writer.Write((int)m_SpeechPattern);
            writer.Write(m_QuestType?.FullName ?? string.Empty);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Personality = reader.ReadString();
            m_PersonalityType = (NPCPersonalities.PersonalityType)reader.ReadInt();
            m_SpeechPattern = (NPCPersonalities.SpeechPattern)reader.ReadInt();

            string questTypeName = reader.ReadString();
            if (!string.IsNullOrEmpty(questTypeName))
            {
                m_QuestType = Type.GetType(questTypeName);
            }

            m_Conversations = new Dictionary<Mobile, QuestConversationContext>();
        }

        #endregion
    }

    /// <summary>
    /// Tracks conversation state for quest offering
    /// </summary>
    public class QuestConversationContext
    {
        public bool QuestOffered { get; set; }
        public string LastPlayerMessage { get; set; }
        public DateTime LastInteraction { get; set; }

        public QuestConversationContext()
        {
            LastInteraction = DateTime.UtcNow;
        }
    }
}
