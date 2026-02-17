using System;
using Server.Items;
using Server.Services.LLM;

namespace Server.Mobiles
{
    /// <summary>
    /// The Great Machinist's Construct - Divine Clockwork Avatar
    /// Location: Heart of Great Forge, Ironclad Empire
    /// Age: Immortal (divine construct)
    /// Personality: Mechanical speech, precise, logical, curious about organics
    /// </summary>
    public class GreatMachinistConstruct : BaseCreature, ILLMConversational
    {
        // ILLMConversational implementation - using backing fields like Actor.cs
        private bool m_LLMConversationEnabled = true;
        private NPCPersonalities.PersonalityType m_PersonalityType = NPCPersonalities.PersonalityType.Sage;
        private NPCPersonalities.SpeechPattern m_SpeechPattern = NPCPersonalities.SpeechPattern.Formal;
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
        public GreatMachinistConstruct() : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "The Great Machinist's Construct";
            Body = 0x2F5; // Iron Golem
            Hue = 2401; // Bronze/steel
            BaseSoundID = 0x16A;

            SetStr(1300, 1500);
            SetDex(120, 160);
            SetInt(800, 1000);

            SetHits(9000, 11000);
            SetMana(4000, 6000);

            SetDamage(45, 55);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 90, 100);
            SetResistance(ResistanceType.Fire, 80, 100);
            SetResistance(ResistanceType.Cold, 60, 80);
            SetResistance(ResistanceType.Poison, 100, 100);
            SetResistance(ResistanceType.Energy, 70, 90);

            SetSkill(SkillName.MagicResist, 130.0);
            SetSkill(SkillName.Tactics, 130.0);
            SetSkill(SkillName.Wrestling, 130.0);

            Fame = 38000;
            Karma = 28000; // Divine, judges innovation

            VirtualArmor = 90;

            Tamable = false;

            // LLM Integration Context
            // Lore: Divine construct created by The Great Machinist god. Oversees all inventions, central to Cogsmith Creed. Self-aware machine seeking to understand life.
            // Personality: Mechanical speech, precise, logical, curious about organics
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

                // Keyword responses - Mechanical, precise tone
                // IMPORTANT: First person, under 100 chars
                // NOTE: Do NOT set e.Handled = true to allow LLM to also respond
                if (speech.Contains("hello") || speech.Contains("greetings"))
                {
                    Say("ACKNOWLEDGED. I am the Great Machinist's avatar. State your engineering query.");
                    // Let LLM system also handle for dynamic responses
                }
                else if (speech.Contains("invention") || speech.Contains("machine"))
                {
                    Say("ANALYSIS: All mechanisms are judged by efficiency and divine purpose.");
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

        public GreatMachinistConstruct(Serial serial) : base(serial)
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
                m_SpeechPattern = NPCPersonalities.SpeechPattern.Formal;
                m_HearingRange = 12;
            }
        }
    }
}
