using System;
using Server.Items;
using Server.Services.LLM;

namespace Server.Mobiles
{
    /// <summary>
    /// Crystalwing the Prismatic Oracle - Ancient Crystal Dragon
    /// Location: Crystal Barrens crystalline cave
    /// Age: 1500+ years
    /// Personality: Ethereal, sees all timelines, speaks in riddles, distant
    /// </summary>
    public class CrystalwingPrismaticOracle : BaseCreature, ILLMConversational
    {
        // ILLMConversational implementation - using backing fields like Actor.cs
        private bool m_LLMConversationEnabled = true;
        private NPCPersonalities.PersonalityType m_PersonalityType = NPCPersonalities.PersonalityType.Sage;
        private NPCPersonalities.SpeechPattern m_SpeechPattern = NPCPersonalities.SpeechPattern.Cryptic;
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
        public CrystalwingPrismaticOracle() : base(AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "Crystalwing the Prismatic Oracle";
            Body = 0x3B; // Dragon body
            Hue = 1154; // Crystal prismatic
            BaseSoundID = 0x16A;

            SetStr(900, 1100);
            SetDex(180, 220);
            SetInt(800, 1000);

            SetHits(5500, 7500);
            SetMana(6000, 8000);

            SetDamage(28, 38);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Energy, 60);

            SetResistance(ResistanceType.Physical, 60, 80);
            SetResistance(ResistanceType.Fire, 60, 80);
            SetResistance(ResistanceType.Cold, 60, 80);
            SetResistance(ResistanceType.Poison, 70, 90);
            SetResistance(ResistanceType.Energy, 90, 100);

            SetSkill(SkillName.MagicResist, 130.0);
            SetSkill(SkillName.Tactics, 105.0);
            SetSkill(SkillName.Wrestling, 105.0);
            SetSkill(SkillName.Magery, 130.0);
            SetSkill(SkillName.EvalInt, 130.0);

            Fame = 32000;
            Karma = 25000; // Oracle, neutral but helpful

            VirtualArmor = 70;

            Tamable = false;

            // LLM Integration Context
            // Lore: Unique crystal dragon formed from pure magical energy, sees past and future simultaneously. Speaks in multiple timelines, follower of Luminous Architect.
            // Personality: Ethereal, sees all timelines, speaks in riddles, distant
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

                // Keyword responses - Cryptic, otherworldly tone
                // IMPORTANT: First person, under 100 chars
                // NOTE: Do NOT set e.Handled = true to allow LLM to also respond
                if (speech.Contains("hello") || speech.Contains("greetings"))
                {
                    Say("I am Crystalwing. Past, present, future... all converge in crystal clarity.");
                    // Let LLM system also handle for dynamic responses
                }
                else if (speech.Contains("future") || speech.Contains("prophecy"))
                {
                    Say("The future is already written in crystalline patterns. Do you wish to see?");
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

        public CrystalwingPrismaticOracle(Serial serial) : base(serial)
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
                m_SpeechPattern = NPCPersonalities.SpeechPattern.Cryptic;
                m_HearingRange = 10;
            }
        }
    }
}
