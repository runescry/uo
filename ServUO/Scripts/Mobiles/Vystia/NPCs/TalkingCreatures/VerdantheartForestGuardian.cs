using System;
using Server.Items;
using Server.Services.LLM;

namespace Server.Mobiles
{
    /// <summary>
    /// Verdantheart the Forest Guardian - Ancient Green Dragon
    /// Location: Hidden grove, Verdantpeak
    /// Age: 4000+ years (oldest dragon in Vystia)
    /// Personality: Wise, patient, protective of nature, cryptic speaker
    /// </summary>
    public class VerdantheartForestGuardian : BaseCreature, ILLMConversational
    {
        // ILLMConversational implementation - using backing fields like Actor.cs
        private bool m_LLMConversationEnabled = true;
        private NPCPersonalities.PersonalityType m_PersonalityType = NPCPersonalities.PersonalityType.Sage;
        private NPCPersonalities.SpeechPattern m_SpeechPattern = NPCPersonalities.SpeechPattern.OldEnglish;
        private int m_HearingRange = 10;

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
        public VerdantheartForestGuardian() : base(AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "Verdantheart the Forest Guardian";
            Body = 0x3C; // Green Dragon
            Hue = 2010; // Verdantpeak forest green
            BaseSoundID = 0x16A;

            SetStr(1100, 1300);
            SetDex(150, 200);
            SetInt(700, 900);

            SetHits(7000, 9000);
            SetMana(5000, 7000);

            SetDamage(35, 45);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Poison, 50);

            SetResistance(ResistanceType.Physical, 70, 90);
            SetResistance(ResistanceType.Fire, 50, 70);
            SetResistance(ResistanceType.Cold, 65, 85);
            SetResistance(ResistanceType.Poison, 90, 100);
            SetResistance(ResistanceType.Energy, 75, 95);

            SetSkill(SkillName.MagicResist, 125.0);
            SetSkill(SkillName.Tactics, 115.0);
            SetSkill(SkillName.Wrestling, 115.0);
            SetSkill(SkillName.Magery, 125.0);
            SetSkill(SkillName.EvalInt, 125.0);

            Fame = 35000;
            Karma = 30000; // Protector of nature

            VirtualArmor = 80;

            Tamable = false;

            // LLM Integration Context
            // Lore: Oldest dragon in Vystia, protected The Grand Oak for millennia, avatar of Lunara's will. Despises Ironclad deforestation, allies with Druids.
            // Personality: Wise, patient, protective of nature, cryptic speaker
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

                // Keyword responses - Wise, ancient tone
                // IMPORTANT: First person, under 100 chars
                // NOTE: Do NOT set e.Handled = true to allow LLM to also respond
                if (speech.Contains("hello") || speech.Contains("greetings"))
                {
                    Say("The forest speaks through me. I am Verdantheart. Why dost thou seek the ancient ways?");
                    // Let LLM system also handle for dynamic responses
                }
                else if (speech.Contains("forest") || speech.Contains("nature"))
                {
                    Say("I have guarded these woods for 4000 years. Every tree is known to me.");
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

        public VerdantheartForestGuardian(Serial serial) : base(serial)
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
                m_HearingRange = 10;
            }
        }
    }
}
