using System;
using Server.Items;
using Server.Services.LLM;

namespace Server.Mobiles
{
    /// <summary>
    /// Lunara's Dryad Herald - Divine Nature Spirit
    /// Location: Grove of Spirits, Verdantpeak
    /// Age: Immortal (divine messenger)
    /// Personality: Ethereal, speaks in nature metaphors, protective
    /// </summary>
    public class LunaraDryadHerald : BaseCreature, ILLMConversational
    {
        // ILLMConversational implementation - using backing fields like Actor.cs
        private bool m_LLMConversationEnabled = true;
        private NPCPersonalities.PersonalityType m_PersonalityType = NPCPersonalities.PersonalityType.Sage;
        private NPCPersonalities.SpeechPattern m_SpeechPattern = NPCPersonalities.SpeechPattern.OldEnglish;
        private int m_HearingRange = 12;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool LLMConversationEnabled
        {
            get { return m_LLMConversationEnabled; }
            set { m_LLMConversationEnabled = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public NPCPersonalities.PersonalityType PersonalityType
        {
            get { return m_PersonalityType; }
            set { m_PersonalityType = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public NPCPersonalities.SpeechPattern SpeechPattern
        {
            get { return m_SpeechPattern; }
            set { m_SpeechPattern = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HearingRange
        {
            get { return m_HearingRange; }
            set { m_HearingRange = value; }
        }

        [Constructable]
        public LunaraDryadHerald() : base(AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "Lunara's Dryad Herald";
            Body = 0x191; // Female Ethereal
            Hue = 2010; // Forest green with ethereal glow
            BaseSoundID = 0x16A;

            SetStr(800, 1000);
            SetDex(200, 240);
            SetInt(900, 1100);

            SetHits(7000, 9000);
            SetMana(7000, 9000);

            SetDamage(35, 45);

            SetDamageType(ResistanceType.Physical, 30);
            SetDamageType(ResistanceType.Poison, 40);
            SetDamageType(ResistanceType.Energy, 30);

            SetResistance(ResistanceType.Physical, 65, 85);
            SetResistance(ResistanceType.Fire, 40, 60);
            SetResistance(ResistanceType.Cold, 70, 90);
            SetResistance(ResistanceType.Poison, 90, 100);
            SetResistance(ResistanceType.Energy, 80, 100);

            SetSkill(SkillName.MagicResist, 135.0);
            SetSkill(SkillName.Tactics, 110.0);
            SetSkill(SkillName.Wrestling, 110.0);
            SetSkill(SkillName.Magery, 135.0);
            SetSkill(SkillName.EvalInt, 135.0);

            Fame = 36000;
            Karma = 32000; // Divine messenger

            VirtualArmor = 75;

            // Ethereal appearance
            Hidden = false;
            Tamable = false;

            // LLM Integration Context
            // Lore: Direct messenger of Lunara, Mother of the Grove. Appears to druids with divine messages, shapeshifts between forms, blesses sacred rituals.
            // Personality: Ethereal, speaks in nature metaphors, protective
        }

        public override bool CanTeach => true;
        public override bool PlayerRangeSensitive => false;

        public override void OnSpeech(SpeechEventArgs e)
        {
            base.OnSpeech(e);

            Mobile from = e.Mobile;

            if (!e.Handled && from.InRange(this, HearingRange))
            {
                string speech = e.Speech.ToLower();

                // Keyword responses - Ethereal, nature-focused tone
                // IMPORTANT: First person, under 100 chars
                // NOTE: Do NOT set e.Handled = true to allow LLM to also respond
                if (speech.Contains("hello") || speech.Contains("greetings"))
                {
                    Say("The Mother speaks through me. I am her Herald. What seekest thou of nature's wisdom?");
                    // Let LLM system also handle for dynamic responses
                }
                else if (speech.Contains("lunara") || speech.Contains("mother"))
                {
                    Say("Lunara, Mother of the Grove, watches over all living things. Her will is mine.");
                    // Let LLM system also handle for dynamic responses
                }
            }
        }

        // ILLMConversational interface methods
        public bool ShouldHandleConversation(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;
            return from.InRange(this, HearingRange) && from.Player;
        }

        public void HandleConversation(SpeechEventArgs e)
        {
            LLMConversationHelper.ProcessConversation(this, e.Mobile, e.Speech);
            e.Handled = true;
        }

        public LunaraDryadHerald(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);

            writer.Write(m_LLMConversationEnabled);
            writer.Write((int)m_PersonalityType);
            writer.Write((int)m_SpeechPattern);
            writer.Write(m_HearingRange);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 1)
            {
                m_LLMConversationEnabled = reader.ReadBool();
                m_PersonalityType = (NPCPersonalities.PersonalityType)reader.ReadInt();
                m_SpeechPattern = (NPCPersonalities.SpeechPattern)reader.ReadInt();
                m_HearingRange = reader.ReadInt();
            }
            else
            {
                m_LLMConversationEnabled = true;
                m_PersonalityType = NPCPersonalities.PersonalityType.Sage;
                m_SpeechPattern = NPCPersonalities.SpeechPattern.OldEnglish;
                m_HearingRange = 12;
            }
        }
    }
}
