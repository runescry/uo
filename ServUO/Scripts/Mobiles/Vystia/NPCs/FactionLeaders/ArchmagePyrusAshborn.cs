using System;
using System.Collections.Generic;
using Server.Items;
using Server.Services.LLM;

namespace Server.Mobiles
{
    /// <summary>
    /// Archmage Pyrus Ashborn - Archmage of the Emberlands
    /// Faction: Ironclad Alliance
    /// Location: Magma Citadel, Emberforge
    /// Personality: Powerful fire sorcerer, ambitious, innovative
    /// </summary>
    public class ArchmagePyrusAshborn : BaseVendor, ILLMConversational
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;

        // ILLMConversational implementation
        public new bool LLMConversationEnabled { get; set; } = true;
        public new NPCPersonalities.PersonalityType PersonalityType { get; set; } = NPCPersonalities.PersonalityType.Sage;
        public new NPCPersonalities.SpeechPattern SpeechPattern { get; set; } = NPCPersonalities.SpeechPattern.Formal;
        public new int HearingRange { get; set; } = 8;

        [Constructable]
        public ArchmagePyrusAshborn() : base("Archmage of the Emberlands")
        {
            Name = "Archmage Pyrus Ashborn";
            Title = "Archmage of the Emberlands";
            Body = 0x190;
            Hue = 1358;

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
            // Lore: Most powerful fire sorcerer in Vystia who wields Phoenix Ascension. Co-founder of Ironclad Alliance. His forges produce legendary fire-enchanted weapons.
            // Personality: Powerful fire sorcerer, ambitious, innovative
        }

        private void SetupAppearance()
        {
            // Leader appearance - regal clothing
            AddItem(new FancyShirt(1358));
            AddItem(new LongPants(1358));
            AddItem(new ThighBoots());
            AddItem(new Cloak(1358));

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
                    Say($"Hail, {from.Name}. I am Archmage Pyrus Ashborn. What brings you before me?");
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

        public ArchmagePyrusAshborn(Serial serial) : base(serial)
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
                PersonalityType = NPCPersonalities.PersonalityType.Sage;
                SpeechPattern = NPCPersonalities.SpeechPattern.Formal;
                HearingRange = 8;
            }
        }
    }
}
