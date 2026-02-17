using System;
using System.Collections.Generic;
using Server.Items;
using Server.Services.LLM;

namespace Server.Mobiles
{
    /// <summary>
    /// Emperor Garrick Steelarm - Emperor of the Ironclad Empire
    /// Faction: Ironclad Alliance
    /// Location: Imperial Palace, Ironhaven
    /// Personality: Visionary leader, strategic genius, pragmatic
    /// </summary>
    public class EmperorGarrickSteelarm : BaseVendor, ILLMConversational
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;

        // ILLMConversational implementation
        public new bool LLMConversationEnabled { get; set; } = true;
        public new NPCPersonalities.PersonalityType PersonalityType { get; set; } = NPCPersonalities.PersonalityType.Emperor;
        public new NPCPersonalities.SpeechPattern SpeechPattern { get; set; } = NPCPersonalities.SpeechPattern.Formal;
        public new int HearingRange { get; set; } = 8;

        [Constructable]
        public EmperorGarrickSteelarm() : base("Emperor of the Ironclad Empire")
        {
            Name = "Emperor Garrick Steelarm";
            Title = "Emperor of the Ironclad Empire";
            Body = 0x190;
            Hue = 2213;

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
            // Faction: Ironclad Alliance
            // Lore: Co-founder of Ironclad Alliance who signed pact with Warlord Flamefist during siege of Ironhold. Believes technology and magic together will bring prosperity.
            // Personality: Visionary leader, strategic genius, pragmatic
        }

        private void SetupAppearance()
        {
            // Leader appearance - regal clothing
            AddItem(new FancyShirt(2213));
            AddItem(new LongPants(2213));
            AddItem(new ThighBoots());
            AddItem(new Cloak(2213));

            // Add appropriate weapon/symbol of office
            // AddItem(new Scepter()); // Custom item
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

                // Keyword responses - Imperial, authoritative tone
                // IMPORTANT: Keep under 100 chars to prevent cutoff!
                // Speak in FIRST PERSON (I, my, we) not third person
                if (speech.Contains("greetings") || speech.Contains("hail") || speech.Contains("hello"))
                {
                    Say($"Hail, {from.Name}. I am Emperor Garrick Steelarm. What brings you before me?");
                    e.Handled = true;
                }
                else if (speech.Contains("faction") || speech.Contains("alliance"))
                {
                    Say("I lead the Ironclad Alliance. We stand united for our people's future.");
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

        public EmperorGarrickSteelarm(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version

            // Version 1: LLM properties
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
                // Default values for old saves
                LLMConversationEnabled = true;
                PersonalityType = NPCPersonalities.PersonalityType.Emperor;
                SpeechPattern = NPCPersonalities.SpeechPattern.Formal;
                HearingRange = 8;
            }
        }
    }
}
