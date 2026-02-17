using System;
using Server.Items;
using Server.Services.LLM;

namespace Server.Mobiles
{
    /// <summary>
    /// The Crystal Sphinx - Crystalline Riddle Master
    /// Location: Crystal Barrens main cave
    /// Age: 1000+ years
    /// Personality: Logical, mathematical riddles, precise
    /// </summary>
    public class TheCrystalSphinx : BaseCreature, ILLMConversational
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
        public TheCrystalSphinx() : base(AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "The Crystal Sphinx";
            Body = 0x5F; // Sphinx
            Hue = 1154; // Crystal prismatic
            BaseSoundID = 0x16A;

            SetStr(750, 950);
            SetDex(160, 200);
            SetInt(700, 900);

            SetHits(4500, 6500);
            SetMana(5000, 7000);

            SetDamage(22, 32);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Energy, 60);

            SetResistance(ResistanceType.Physical, 55, 75);
            SetResistance(ResistanceType.Fire, 55, 75);
            SetResistance(ResistanceType.Cold, 55, 75);
            SetResistance(ResistanceType.Poison, 65, 85);
            SetResistance(ResistanceType.Energy, 85, 100);

            SetSkill(SkillName.MagicResist, 125.0);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Magery, 125.0);
            SetSkill(SkillName.EvalInt, 125.0);

            Fame = 24000;
            Karma = 22000; // Guardian of knowledge

            VirtualArmor = 65;

            Tamable = false;

            // LLM Integration Context
            // Lore: Crystalline sphinx that poses mathematical and magical puzzles, guards greatest crystal formations. Studies arcane mathematics, follower of Luminous Architect.
            // Personality: Logical, mathematical riddles, precise
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

                // Keyword responses - Mathematical, logical tone
                // IMPORTANT: First person, under 100 chars
                // NOTE: Do NOT set e.Handled = true to allow LLM to also respond
                if (speech.Contains("hello") || speech.Contains("greetings"))
                {
                    Say("I am the Crystal Sphinx. Solve my riddles to pass. Logic is key.");
                    // Let LLM system also handle for dynamic responses
                }
                else if (speech.Contains("riddle") || speech.Contains("puzzle"))
                {
                    Say("Answer correctly and gain passage. Fail, and you shall not proceed.");
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

        public TheCrystalSphinx(Serial serial) : base(serial)
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
