using System;
using Server.Items;
using Server.Services.LLM;

namespace Server.Mobiles
{
    /// <summary>
    /// Frosthelm the Eternal Winter - Ancient White Ancient Dragon
    /// Location: Frozen Peak cave, Frosthold
    /// Age: 3000+ years
    /// Personality: Ancient, wise, speaks slowly, protective
    /// </summary>
    public class FrosthelmEternalWinter : BaseCreature, ILLMConversational
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
        public FrosthelmEternalWinter() : base(AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "Frosthelm the Eternal Winter";
            Body = 0xC;
            Hue = 1152;
            BaseSoundID = 0x16A;

            SetStr(800, 1000);
            SetDex(150, 200);
            SetInt(500, 700);

            SetHits(5000, 7000);
            SetMana(3000, 5000);

            SetDamage(25, 35);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Cold, 25);
            SetDamageType(ResistanceType.Energy, 25);

            SetResistance(ResistanceType.Physical, 60, 80);
            SetResistance(ResistanceType.Fire, 50, 70);
            SetResistance(ResistanceType.Cold, 80, 100);
            SetResistance(ResistanceType.Poison, 60, 80);
            SetResistance(ResistanceType.Energy, 70, 90);

            SetSkill(SkillName.MagicResist, 120.0);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Magery, 120.0);
            SetSkill(SkillName.EvalInt, 120.0);

            Fame = 25000;
            Karma = 25000; // Ancient guardian, not hostile

            VirtualArmor = 70;

            Tamable = false;

            // LLM Integration Context
            // Lore: Ancient white dragon who witnessed formation of Frosthold. Guardian of eternal ice secrets. Knows Frost Father personally.
            // Personality: Ancient, wise, speaks slowly, protective
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

                // Keyword responses - Ancient, wise tone
                // IMPORTANT: First person, under 100 chars
                // NOTE: Do NOT set e.Handled = true to allow LLM to also respond
                if (speech.Contains("hello") || speech.Contains("greetings"))
                {
                    Say("Greetings, mortal. I am Frosthelm the Eternal Winter. What brings you to seek my wisdom?");
                    // Let LLM system also handle for dynamic responses
                }
                else if (speech.Contains("age") || speech.Contains("old"))
                {
                    Say("I have lived for 3000+ years. I have seen much.");
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

        public FrosthelmEternalWinter(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version

            // Version 1: LLM properties - using backing fields
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
                // Default values for old saves
                m_LLMConversationEnabled = true;
                m_PersonalityType = NPCPersonalities.PersonalityType.Sage;
                m_SpeechPattern = NPCPersonalities.SpeechPattern.OldEnglish;
                m_HearingRange = 10;
            }
        }
    }
}
