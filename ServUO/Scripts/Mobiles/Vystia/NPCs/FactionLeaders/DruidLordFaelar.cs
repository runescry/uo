using System;
using System.Collections.Generic;
using Server.Items;
using Server.Services.LLM;

namespace Server.Mobiles
{
    /// <summary>
    /// Druid Lord Faelar - Druid Lord of Shadowfen
    /// Faction: Sylvan Concord
    /// Location: Shadowfen
    /// Personality: Elder, formal, nature-focused
    /// </summary>
    public class DruidLordFaelar : BaseVendor, ILLMConversational
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;

        public new bool LLMConversationEnabled { get; set; } = true;
        public new NPCPersonalities.PersonalityType PersonalityType { get; set; } = NPCPersonalities.PersonalityType.Elder;
        public new NPCPersonalities.SpeechPattern SpeechPattern { get; set; } = NPCPersonalities.SpeechPattern.Formal;
        public new int HearingRange { get; set; } = 8;

        [Constructable]
        public DruidLordFaelar() : base("Druid Lord of Shadowfen")
        {
            Name = "Druid Lord Faelar";
            Title = "Druid Lord of Shadowfen";
            Body = 0x190;
            Hue = 0x83F; // Swamp green

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
            AddItem(new FancyShirt(0x83F));
            AddItem(new LongPants(0x83F));
            AddItem(new ThighBoots());
            AddItem(new Cloak(0x83F));
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
                    Say($"Hail, {from.Name}. I am Druid Lord Faelar. What brings you before me?");
                    e.Handled = true;
                }
                else if (speech.Contains("faction") || speech.Contains("alliance"))
                {
                    Say("I lead the Sylvan Concord. We protect nature and the wilds.");
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

        public DruidLordFaelar(Serial serial) : base(serial)
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
                PersonalityType = NPCPersonalities.PersonalityType.Elder;
                SpeechPattern = NPCPersonalities.SpeechPattern.Formal;
                HearingRange = 8;
            }
        }
    }
}

