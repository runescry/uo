using System;
using Server.Items;
using Server.Services.LLM;

namespace Server.Mobiles
{
    /// <summary>
    /// Emberflame the Ashen Tyrant - Ancient Red Dragon
    /// Location: Volcano caldera, Emberlands
    /// Age: 2500+ years
    /// Personality: Arrogant, temperamental, respects strength, hoards treasures
    /// </summary>
    public class EmberflameAshenTyrant : BaseCreature, ILLMConversational
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
        public EmberflameAshenTyrant() : base(AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "Emberflame the Ashen Tyrant";
            Body = 0x3B; // Red Dragon
            Hue = 1358; // Emberlands fiery orange/red
            BaseSoundID = 0x16A;

            SetStr(1000, 1200);
            SetDex(150, 200);
            SetInt(600, 800);

            SetHits(6000, 8000);
            SetMana(4000, 6000);

            SetDamage(30, 40);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Fire, 50);

            SetResistance(ResistanceType.Physical, 65, 85);
            SetResistance(ResistanceType.Fire, 90, 100);
            SetResistance(ResistanceType.Cold, 40, 60);
            SetResistance(ResistanceType.Poison, 65, 85);
            SetResistance(ResistanceType.Energy, 70, 90);

            SetSkill(SkillName.MagicResist, 120.0);
            SetSkill(SkillName.Tactics, 110.0);
            SetSkill(SkillName.Wrestling, 110.0);
            SetSkill(SkillName.Magery, 120.0);
            SetSkill(SkillName.EvalInt, 120.0);

            Fame = 30000;
            Karma = -15000; // Arrogant tyrant

            VirtualArmor = 75;

            Tamable = false;

            // LLM Integration Context
            // Lore: Ancient red dragon who claims ownership of all volcanic regions, follower of Flame Wielder. Respects strength, collects emberforged weapons.
            // Personality: Arrogant, temperamental, respects strength, hoards treasures
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

                // Keyword responses - Arrogant, powerful tone
                // IMPORTANT: First person, under 100 chars
                // NOTE: Do NOT set e.Handled = true to allow LLM to also respond
                if (speech.Contains("hello") || speech.Contains("greetings"))
                {
                    Say("Mortal. You dare address Emberflame? State your business or be ash.");
                    // Let LLM system also handle for dynamic responses
                }
                else if (speech.Contains("treasure") || speech.Contains("hoard"))
                {
                    Say("My hoard is beyond your comprehension. Earned through 2500 years of conquest.");
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

        public EmberflameAshenTyrant(Serial serial) : base(serial)
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
