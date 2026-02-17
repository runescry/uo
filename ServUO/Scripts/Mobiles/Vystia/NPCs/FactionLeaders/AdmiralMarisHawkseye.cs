using System;
using System.Collections.Generic;
using Server.Items;
using Server.Services.LLM;

namespace Server.Mobiles
{
    /// <summary>
    /// Admiral Maris Hawkseye - Admiral of The Sunken Isles
    /// Faction: Maritime Sovereignty
    /// Location: The Sunken Isles
    /// Personality: Authoritative naval commander, strategic, maritime-focused
    /// </summary>
    public class AdmiralMarisHawkseye : BaseVendor, ILLMConversational
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;

        // ILLMConversational implementation
        public new bool LLMConversationEnabled { get; set; } = true;
        public new NPCPersonalities.PersonalityType PersonalityType { get; set; } = NPCPersonalities.PersonalityType.FactionLeader;
        public new NPCPersonalities.SpeechPattern SpeechPattern { get; set; } = NPCPersonalities.SpeechPattern.Formal;
        public new int HearingRange { get; set; } = 8;

        [Constructable]
        public AdmiralMarisHawkseye() : base("Admiral of The Sunken Isles")
        {
            Name = "Admiral Maris Hawkseye";
            Title = "Admiral of The Sunken Isles";
            Body = 0x190;
            Hue = 0x83EA; // Ocean blue

            SetupAppearance();

            // High stats for a faction leader
            SetStr(150, 200);
            SetDex(100, 150);
            SetInt(150, 200);

            SetHits(500, 700);
            SetMana(300, 500);

            SetSkill(SkillName.Magery, 100.0);
            SetSkill(SkillName.MagicResist, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.Wrestling, 100.0);

            Fame = 15000;
            Karma = 15000;

            // LLM Integration Context
            // Faction: Maritime Sovereignty
            // Lore: Naval commander who leads the maritime factions, expert in naval warfare and ocean navigation
            // Personality: Authoritative naval commander, strategic, maritime-focused
        }

        private void SetupAppearance()
        {
            AddItem(new FancyShirt(0x83EA));
            AddItem(new LongPants(0x83EA));
            AddItem(new ThighBoots());
            AddItem(new Cloak(0x83EA));
        }

        public override void InitSBInfo()
        {
            // Faction leaders don't typically sell items
        }

        public override bool CanTeach => true;

        public override void OnSpeech(SpeechEventArgs e)
        {
            base.OnSpeech(e);

            Mobile from = e.Mobile;

            if (!e.Handled && from.InRange(this, 3))
            {
                string speech = e.Speech.ToLower();

                if (speech.Contains("greetings") || speech.Contains("hail") || speech.Contains("hello"))
                {
                    Say($"Hail, {from.Name}. I am Admiral Maris Hawkseye. What brings you before me?");
                    e.Handled = true;
                }
                else if (speech.Contains("faction") || speech.Contains("alliance"))
                {
                    Say("I lead the Maritime Sovereignty. We rule the waves and protect our shores.");
                    e.Handled = true;
                }
            }
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

        public AdmiralMarisHawkseye(Serial serial) : base(serial)
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
                PersonalityType = NPCPersonalities.PersonalityType.FactionLeader;
                SpeechPattern = NPCPersonalities.SpeechPattern.Formal;
                HearingRange = 8;
            }
        }
    }
}

