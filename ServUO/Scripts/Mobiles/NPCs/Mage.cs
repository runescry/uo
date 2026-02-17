using System;
using System.Collections.Generic;
using Server.Services.LLM;

namespace Server.Mobiles
{
    public class Mage : BaseVendor, ILLMConversational
    {
        private bool m_LLMConversationEnabled = true;
        private NPCPersonalities.PersonalityType m_PersonalityType = NPCPersonalities.PersonalityType.Mage;
        private NPCPersonalities.SpeechPattern m_SpeechPattern = NPCPersonalities.SpeechPattern.Archaic;
        private int m_HearingRange = 10;

        [CommandProperty(AccessLevel.GameMaster)]
        public new bool LLMConversationEnabled
        {
            get { return m_LLMConversationEnabled; }
            set { m_LLMConversationEnabled = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public new NPCPersonalities.PersonalityType PersonalityType
        {
            get { return m_PersonalityType; }
            set { m_PersonalityType = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public new NPCPersonalities.SpeechPattern SpeechPattern
        {
            get { return m_SpeechPattern; }
            set { m_SpeechPattern = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public new int HearingRange
        {
            get { return m_HearingRange; }
            set { m_HearingRange = value; }
        }

        public new bool ShouldHandleConversation(SpeechEventArgs e)
        {
            return LLMConversationHelper.ShouldHandleConversation(this, e.Mobile, e.Speech);
        }

        public new void HandleConversation(SpeechEventArgs e)
        {
            LLMConversationHelper.ProcessConversation(this, e.Mobile, e.Speech);
            e.Handled = true;
        }
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Mage()
            : base("the mage")
        {
            this.SetSkill(SkillName.EvalInt, 65.0, 88.0);
            this.SetSkill(SkillName.Inscribe, 60.0, 83.0);
            this.SetSkill(SkillName.Magery, 64.0, 100.0);
            this.SetSkill(SkillName.Meditation, 60.0, 83.0);
            this.SetSkill(SkillName.MagicResist, 65.0, 88.0);
            this.SetSkill(SkillName.Wrestling, 36.0, 68.0);
        }

        public Mage(Serial serial)
            : base(serial)
        {
        }

        public override NpcGuild NpcGuild
        {
            get
            {
                return NpcGuild.MagesGuild;
            }
        }
        public override VendorShoeType ShoeType
        {
            get
            {
                return Utility.RandomBool() ? VendorShoeType.Shoes : VendorShoeType.Sandals;
            }
        }
        protected override List<SBInfo> SBInfos
        {
            get
            {
                return this.m_SBInfos;
            }
        }
        public override void InitSBInfo()
        {
            this.m_SBInfos.Add(new SBMage());
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            this.AddItem(new Server.Items.Robe(Utility.RandomBlueHue()));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            // Version 1: LLM conversation support
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
                // Default values for existing mages
                m_LLMConversationEnabled = true;
                m_PersonalityType = NPCPersonalities.PersonalityType.Mage;
                m_SpeechPattern = NPCPersonalities.SpeechPattern.Archaic;
                m_HearingRange = 10;
            }
        }
    }
}