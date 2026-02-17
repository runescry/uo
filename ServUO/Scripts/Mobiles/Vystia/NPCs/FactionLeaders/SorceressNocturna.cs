using System;
using System.Collections.Generic;
using Server.Items;
using Server.Services.LLM;

namespace Server.Mobiles
{
    /// <summary>
    /// Sorceress Nocturna - Sorceress of Eternal Twilight
    /// Faction: Arcane Coalition
    /// Location: Eternal Twilight
    /// Personality: Sage, cryptic, shadow-focused
    /// </summary>
    public class SorceressNocturna : BaseVendor, ILLMConversational
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;

        public new bool LLMConversationEnabled { get; set; } = true;
        public new NPCPersonalities.PersonalityType PersonalityType { get; set; } = NPCPersonalities.PersonalityType.Sage;
        public new NPCPersonalities.SpeechPattern SpeechPattern { get; set; } = NPCPersonalities.SpeechPattern.Cryptic;
        public new int HearingRange { get; set; } = 8;

        [Constructable]
        public SorceressNocturna() : base("Sorceress of Eternal Twilight")
        {
            Name = "Sorceress Nocturna";
            Title = "Sorceress of Eternal Twilight";
            Body = 0x191; // Female
            Hue = 0x3E7; // Dark purple/shadow

            SetupAppearance();

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
        }

        private void SetupAppearance()
        {
            AddItem(new FancyShirt(0x3E7));
            AddItem(new LongPants(0x3E7));
            AddItem(new ThighBoots());
            AddItem(new Cloak(0x3E7));
        }

        public override void InitSBInfo()
        {
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
                    Say($"Hail, {from.Name}. I am Sorceress Nocturna. What brings you before me?");
                    e.Handled = true;
                }
                else if (speech.Contains("faction") || speech.Contains("alliance"))
                {
                    Say("I lead the Arcane Coalition. We study the mysteries of magic.");
                    e.Handled = true;
                }
            }
        }

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

        public SorceressNocturna(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);

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
                PersonalityType = NPCPersonalities.PersonalityType.Sage;
                SpeechPattern = NPCPersonalities.SpeechPattern.Cryptic;
                HearingRange = 8;
            }
        }
    }
}

