using System;
using System.Collections.Generic;
using Server.Items;
using Server.Services.LLM;

namespace Server.Mobiles
{
    /// <summary>
    /// Warlord Emberon Flamefist - Warlord of the Emberlands
    /// Faction: Ironclad Alliance
    /// Location: The Emberlands
    /// Personality: Warrior, gruff, fire-focused
    /// </summary>
    public class WarlordEmberonFlamefist : BaseVendor, ILLMConversational
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;

        public new bool LLMConversationEnabled { get; set; } = true;
        public new NPCPersonalities.PersonalityType PersonalityType { get; set; } = NPCPersonalities.PersonalityType.Warrior;
        public new NPCPersonalities.SpeechPattern SpeechPattern { get; set; } = NPCPersonalities.SpeechPattern.Casual;
        public new int HearingRange { get; set; } = 8;

        [Constructable]
        public WarlordEmberonFlamefist() : base("Warlord of the Emberlands")
        {
            Name = "Warlord Emberon Flamefist";
            Title = "Warlord of the Emberlands";
            Body = 0x190;
            Hue = 0x4EC; // Fire red

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
            AddItem(new FancyShirt(0x4EC));
            AddItem(new LongPants(0x4EC));
            AddItem(new ThighBoots());
            AddItem(new Cloak(0x4EC));
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
                    Say($"Hail, {from.Name}. I am Warlord Emberon Flamefist. What brings you before me?");
                    e.Handled = true;
                }
                else if (speech.Contains("faction") || speech.Contains("alliance"))
                {
                    Say("I lead the Ironclad Alliance. We stand united for our people's future.");
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

        public WarlordEmberonFlamefist(Serial serial) : base(serial)
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
                PersonalityType = NPCPersonalities.PersonalityType.Warrior;
                SpeechPattern = NPCPersonalities.SpeechPattern.Casual;
                HearingRange = 8;
            }
        }
    }
}

