using System;
using Server.Items;
using Server.Services.LLM;

namespace Server.Mobiles
{
    /// <summary>
    /// Ironbark the War-Ancient - War Treant
    /// Location: Verdantpeak border patrol
    /// Age: 1000+ years
    /// Personality: Aggressive, protective, militant, hates industry
    /// </summary>
    public class IronbarkWarAncient : BaseCreature, ILLMConversational
    {
        // ILLMConversational implementation - using backing fields like Actor.cs
        private bool m_LLMConversationEnabled = true;
        private NPCPersonalities.PersonalityType m_PersonalityType = NPCPersonalities.PersonalityType.Warrior;
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
        public IronbarkWarAncient() : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "Ironbark the War-Ancient";
            Body = 0x2F; // Treant
            Hue = 2010; // Darker forest tone
            BaseSoundID = 0x16A;

            SetStr(900, 1100);
            SetDex(80, 120);
            SetInt(400, 600);

            SetHits(6000, 8000);
            SetMana(2000, 4000);

            SetDamage(30, 40);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 75, 95);
            SetResistance(ResistanceType.Fire, 20, 40);
            SetResistance(ResistanceType.Cold, 60, 80);
            SetResistance(ResistanceType.Poison, 80, 100);
            SetResistance(ResistanceType.Energy, 50, 70);

            SetSkill(SkillName.MagicResist, 110.0);
            SetSkill(SkillName.Tactics, 120.0);
            SetSkill(SkillName.Wrestling, 120.0);

            Fame = 26000;
            Karma = 15000; // Protector, but militant

            VirtualArmor = 80;

            Tamable = false;

            // LLM Integration Context
            // Lore: War leader of treants, organizes defense against Ironclad deforestation. Scarred by axes, commands treant army, would start war if not restrained.
            // Personality: Aggressive, protective, militant, hates industry
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

                // Keyword responses - Militant, protective tone
                // IMPORTANT: First person, under 100 chars
                // NOTE: Do NOT set e.Handled = true to allow LLM to also respond
                if (speech.Contains("hello") || speech.Contains("greetings"))
                {
                    Say("Ironbark stands guard. State thy purpose, or leave this forest immediately.");
                    // Let LLM system also handle for dynamic responses
                }
                else if (speech.Contains("ironclad") || speech.Contains("industry"))
                {
                    Say("Those metal-worshippers! They scar the forest with their axes. I shall not permit it!");
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

        public IronbarkWarAncient(Serial serial) : base(serial)
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
                m_PersonalityType = NPCPersonalities.PersonalityType.Warrior;
                m_SpeechPattern = NPCPersonalities.SpeechPattern.OldEnglish;
                m_HearingRange = 10;
            }
        }
    }
}
