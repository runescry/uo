using System;
using Server.Items;
using Server.Services.LLM;

namespace Server.Mobiles 
{ 
    public class Actor : BaseCreature, ILLMConversational
    {
        private bool m_LLMConversationEnabled = true;
        private NPCPersonalities.PersonalityType m_PersonalityType = NPCPersonalities.PersonalityType.Actor;
        private NPCPersonalities.SpeechPattern m_SpeechPattern = NPCPersonalities.SpeechPattern.Casual;
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

        public bool ShouldHandleConversation(SpeechEventArgs e)
        {
            return LLMConversationHelper.ShouldHandleConversation(this, e.Mobile, e.Speech);
        }

        public void HandleConversation(SpeechEventArgs e)
        {
            LLMConversationHelper.ProcessConversation(this, e.Mobile, e.Speech);
            e.Handled = true;
        } 
        [Constructable] 
        public Actor()
            : base(AIType.AI_Animal, FightMode.None, 10, 1, 0.2, 0.4)
        { 
            this.InitStats(31, 41, 51); 

            this.SpeechHue = Utility.RandomDyedHue(); 

            this.Hue = Utility.RandomSkinHue(); 

            if (this.Female = Utility.RandomBool()) 
            { 
                this.Body = 0x191; 
                this.Name = NameList.RandomName("female");
                this.AddItem(new FancyDress(Utility.RandomDyedHue())); 
                this.Title = "the actress"; 
            }
            else 
            { 
                this.Body = 0x190; 
                this.Name = NameList.RandomName("male");
                this.AddItem(new LongPants(Utility.RandomNeutralHue())); 
                this.AddItem(new FancyShirt(Utility.RandomDyedHue()));
                this.Title = "the actor";
            }

            this.AddItem(new Boots(Utility.RandomNeutralHue()));

            Utility.AssignRandomHair(this);

            Container pack = new Backpack(); 

            pack.DropItem(new Gold(250, 300)); 

            pack.Movable = false; 

            this.AddItem(pack); 
        }

        public Actor(Serial serial)
            : base(serial)
        { 
        }

        public override bool ClickTitle
        {
            get
            {
                return false;
            }
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
                // Default values for existing actors
                m_LLMConversationEnabled = true;
                m_PersonalityType = NPCPersonalities.PersonalityType.Actor;
                m_SpeechPattern = NPCPersonalities.SpeechPattern.Casual;
                m_HearingRange = 10;
            }
        }
    }
}