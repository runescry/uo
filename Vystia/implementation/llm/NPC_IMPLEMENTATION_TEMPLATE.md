# NPC LLM Implementation Template

**Created:** 2025-12-08
**Purpose:** Complete guide for implementing custom NPCs with full LLM integration in Vystia
**Reference:** Emperor Garrick Steelarm implementation (fully tested and working)

---

## Overview

This template documents the complete process for creating NPCs with LLM (AI conversation) support in the Vystia shard. Follow this pattern for consistent, working implementations.

**What This Achieves:**
- ✅ NPC responds intelligently using OpenAI GPT
- ✅ NPC has appropriate personality and knowledge
- ✅ NPC speaks in character about Vystia (not Britannia)
- ✅ NPC role determines expertise level (Emperor vs Merchant vs Commoner)
- ✅ Scripted keyword responses for quick interactions
- ✅ Dynamic LLM responses for complex questions

---

## Required Files to Modify

### Core System Files (3 files - modify ONCE per new personality type)

1. **`ServUO/Scripts/Services/LLM/Data/NPCPersonalities.cs`**
   - Add PersonalityType enum entry
   - Add domain knowledge description
   - Add base personality description

2. **`ServUO/Scripts/Services/LLM/Data/NPCKnowledgeSystem.cs`**
   - Add NPCRole enum entry
   - Add PersonalityType → NPCRole mapping
   - Add knowledge domain initialization

3. **`ServUO/Scripts/Services/LLM/Data/KnowledgeDomain.cs`**
   - Add NPCRole case in switch statement
   - Add domain initialization method (if not using existing)

### NPC Class File (1 file per NPC)

4. **`ServUO/Scripts/Mobiles/Vystia/NPCs/[Category]/[NPCName].cs`**
   - Implement ILLMConversational interface
   - Set PersonalityType and SpeechPattern
   - Add scripted keyword responses
   - Add HandleConversation implementation
   - Add serialization for LLM properties

---

## Step-by-Step Implementation Guide

### STEP 1: Define PersonalityType (NPCPersonalities.cs)

**File:** `ServUO/Scripts/Services/LLM/Data/NPCPersonalities.cs`

**1.1: Add enum entry (line ~230)**

```csharp
public enum PersonalityType
{
    // ... existing entries ...
    Beggar,         // Desperate, pleading, talks about hardship and charity

    // Vystia faction leaders
    Emperor,        // Imperial ruler, strategic visionary, commands with authority
    Chieftain,      // Warrior leader, gruff and honorable, protects his people
    Elder,          // Ancient wise leader, protective of nature and traditions
    Sultan,         // Diplomatic merchant prince, shrewd and neutral
    FactionLeader   // General faction leader (any other leader type)
}
```

**1.2: Add domain knowledge description (line ~593)**

**CRITICAL:** This defines what the NPC KNOWS. Must mention "Vystia" explicitly!

```csharp
private static string GetDomainKnowledgeSummary(PersonalityType personality)
{
    switch (personality)
    {
        // ... existing cases ...

        // Vystia Faction Leaders
        case PersonalityType.Emperor:
            return "DOMAIN KNOWLEDGE: You are Emperor of the Ironclad Empire in Vystia, a realm where technology and magic unite. You know the history, politics, and geography of Vystia's regions. You understand statecraft, military strategy, economic policy, and diplomatic relations between the factions. You are a visionary leader who believes in progress through the fusion of steam technology and arcane magic.";

        case PersonalityType.Chieftain:
            return "DOMAIN KNOWLEDGE: You are Chieftain of Frosthold in Vystia, leader of the northern clans. You know the frozen territories, their dangers, and their treasures. You understand warrior culture, honor codes, and the harsh realities of surviving in the frozen wastes. You are a legendary fighter who has faced frost giants and ice dragons, protecting your people with strength and honor.";

        // ... add more as needed ...

        default:
            return ""; // No domain knowledge summary for other types
    }
}
```

**1.3: Add base personality traits (line ~784)**

**CRITICAL:** This defines HOW the NPC speaks. Must mention "Vystia" and specific character name!

```csharp
private static string GetBasePersonality(PersonalityType personality)
{
    switch (personality)
    {
        // ... existing cases ...

        // Vystia Faction Leaders
        case PersonalityType.Emperor:
            return "You are Emperor Garrick Steelarm, ruler of the Ironclad Empire in Vystia. Speak with imperial authority and strategic vision. You are pragmatic, decisive, and believe in progress through the fusion of technology and magic. Discuss governance, military strategy, and the future of Vystia. Use commanding but not arrogant language. You lead with wisdom earned through difficult decisions.";

        case PersonalityType.Chieftain:
            return "You are a warrior chieftain of the frozen north in Vystia. Speak with gruff directness and martial honor. You value strength, courage, and protecting your people. Discuss battles, honor codes, and the harsh beauty of the frozen wastes. You are straightforward, honorable, and fear nothing. You've earned respect through countless battles.";

        // ... add more as needed ...

        default:
            return "You are a resident of Vystia with your own unique personality.";
    }
}
```

---

### STEP 2: Define NPCRole (NPCKnowledgeSystem.cs)

**File:** `ServUO/Scripts/Services/LLM/Data/NPCKnowledgeSystem.cs`

**2.1: Add NPCRole enum entry (line ~37)**

```csharp
public enum NPCRole
{
    Blacksmith,
    Weaponsmith,
    Armorer,
    Guard,
    Mage,
    Scholar,
    Vendor,
    Innkeeper,
    Merchant,
    Healer,
    Ranger,
    Sailor,
    Miner,
    Farmer,
    QuestGiver,
    Commoner,
    Jeweler,
    // Vystia faction leaders
    FactionLeader,
    Emperor,
    Chieftain,
    Elder,
    Sultan,
    Archmage
}
```

**2.2: Map PersonalityType to NPCRole (line ~454)**

```csharp
public static NPCRole InferRoleFromPersonality(NPCPersonalities.PersonalityType personality)
{
    switch (personality)
    {
        // ... existing cases ...

        case NPCPersonalities.PersonalityType.Ranger:
            return NPCRole.Ranger;

        // Vystia faction leaders
        case NPCPersonalities.PersonalityType.Emperor:
            return NPCRole.Emperor;
        case NPCPersonalities.PersonalityType.Chieftain:
            return NPCRole.Chieftain;
        case NPCPersonalities.PersonalityType.Elder:
            return NPCRole.Elder;
        case NPCPersonalities.PersonalityType.Sultan:
            return NPCRole.Sultan;
        case NPCPersonalities.PersonalityType.FactionLeader:
            return NPCRole.FactionLeader;

        case NPCPersonalities.PersonalityType.Commoner:
        default:
            return NPCRole.Commoner;
    }
}
```

---

### STEP 3: Define Knowledge Domain (KnowledgeDomain.cs)

**File:** `ServUO/Scripts/Services/LLM/Data/KnowledgeDomain.cs`

**3.1: Add switch cases (line ~117)**

```csharp
private void InitializeDomains()
{
    switch (Role)
    {
        // ... existing cases ...

        case NPCKnowledgeSystem.NPCRole.Ranger:
            InitializeRangerDomain();
            break;

        // Vystia faction leaders - all use same domain (political/historical experts)
        case NPCKnowledgeSystem.NPCRole.FactionLeader:
        case NPCKnowledgeSystem.NPCRole.Emperor:
        case NPCKnowledgeSystem.NPCRole.Chieftain:
        case NPCKnowledgeSystem.NPCRole.Elder:
        case NPCKnowledgeSystem.NPCRole.Sultan:
        case NPCKnowledgeSystem.NPCRole.Archmage:
            InitializeFactionLeaderDomain();
            break;

        case NPCKnowledgeSystem.NPCRole.Commoner:
        case NPCKnowledgeSystem.NPCRole.Farmer:
        case NPCKnowledgeSystem.NPCRole.Miner:
        default:
            InitializeCommonerDomain();
            break;
    }
}
```

**3.2: Add domain initialization method (line ~347, before InitializeCommonerDomain)**

```csharp
private void InitializeFactionLeaderDomain()
{
    // EXPERT: Political, historical, and geographical knowledge
    DomainExpertise[QuestionCategory.History] = KnowledgeExpertise.Expert; // They make history
    DomainExpertise[QuestionCategory.Geography] = KnowledgeExpertise.Expert; // Their territories
    DomainExpertise[QuestionCategory.Law] = KnowledgeExpertise.Expert; // They enforce/create it
    DomainExpertise[QuestionCategory.Trade] = KnowledgeExpertise.Expert; // Economic policy

    // PROFICIENT: Military and magical knowledge
    DomainExpertise[QuestionCategory.Combat] = KnowledgeExpertise.Proficient; // Military strategy
    DomainExpertise[QuestionCategory.Magic] = KnowledgeExpertise.Proficient; // Educated in magic
    DomainExpertise[QuestionCategory.Monsters] = KnowledgeExpertise.Proficient; // Threats to realm
    DomainExpertise[QuestionCategory.Dungeons] = KnowledgeExpertise.Proficient; // Realm knowledge

    // AWARE: General educated knowledge
    DomainExpertise[QuestionCategory.Religion] = KnowledgeExpertise.Aware; // Cultural knowledge
    DomainExpertise[QuestionCategory.Healing] = KnowledgeExpertise.Aware; // General knowledge
    DomainExpertise[QuestionCategory.Crafting] = KnowledgeExpertise.Aware; // Economic oversight
    DomainExpertise[QuestionCategory.Nature] = KnowledgeExpertise.Aware; // Environmental awareness

    // Leaders don't refer people - they ARE the authority
    // Leave referral phrases empty (they know everything or can find out)
}
```

**Knowledge Expertise Levels:**
- **Expert:** Core domain - answers confidently, provides detailed information
- **Proficient:** Related knowledge - understands well, can discuss competently
- **Aware:** General knowledge - knows basics, can reference but not deep dive
- **Ignorant:** Outside domain - should refer to specialists

---

### STEP 4: Create NPC Class File

**File:** `ServUO/Scripts/Mobiles/Vystia/NPCs/[Category]/[NPCName].cs`

**4.1: Class declaration with ILLMConversational**

```csharp
using System;
using System.Collections.Generic;
using Server.Items;
using Server.Services.LLM;  // REQUIRED for ILLMConversational

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

        // ILLMConversational implementation - REQUIRED PROPERTIES
        public bool LLMConversationEnabled { get; set; } = true;
        public NPCPersonalities.PersonalityType PersonalityType { get; set; } = NPCPersonalities.PersonalityType.Emperor;
        public NPCPersonalities.SpeechPattern SpeechPattern { get; set; } = NPCPersonalities.SpeechPattern.Formal;
        public int HearingRange { get; set; } = 8;
```

**Speech Patterns:**
- `Modern` - Contemporary English
- `Formal` - Elevated, proper English (recommended for leaders)
- `OldEnglish` - Thee, thou, dost (avoid unless intentional)
- `Cryptic` - Mysterious, riddles
- `Casual` - Relaxed, contractions, slang
- `Archaic` - Medieval fantasy speech

**4.2: Constructor**

```csharp
        [Constructable]
        public EmperorGarrickSteelarm() : base("Emperor of the Ironclad Empire")
        {
            Name = "Emperor Garrick Steelarm";
            Title = "Emperor of the Ironclad Empire";
            Body = 0x190;  // Male body
            Hue = 2213;    // Custom hue

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
        }
```

**4.3: Scripted keyword responses (OnSpeech)**

**IMPORTANT:** Keep responses SHORT (under 100 chars) to prevent cutoff!

```csharp
        public override void OnSpeech(SpeechEventArgs e)
        {
            base.OnSpeech(e);

            Mobile from = e.Mobile;

            if (!e.Handled && from.InRange(this, 3))
            {
                string speech = e.Speech.ToLower();

                // Keyword responses - Imperial, authoritative tone
                if (speech.Contains("greetings") || speech.Contains("hail") || speech.Contains("hello"))
                {
                    Say($"Hail, {from.Name}. I am Emperor Garrick Steelarm. What brings you before the Iron Throne?");
                    e.Handled = true;
                }
                else if (speech.Contains("faction") || speech.Contains("alliance"))
                {
                    Say("The Ironclad Alliance unites technology and magic. Together, we shall bring prosperity to all Vystia.");
                    e.Handled = true;
                }
                else if (speech.Contains("ironclad") || speech.Contains("empire"))
                {
                    Say("My empire leads Vystia into a new age. Steam and steel shall forge our destiny.");
                    e.Handled = true;
                }

                // LLM handles non-keyword questions automatically via BaseAI
            }
        }
```

**Guidelines for Scripted Responses:**
- Use 2-4 keyword triggers per response
- Keep responses under 100 characters
- Use first person ("I am", "my empire")
- Match personality tone (imperial, gruff, wise, etc.)
- Mention Vystia or specific locations
- Don't use archaic English unless PersonalityType requires it

**4.4: ILLMConversational interface methods**

```csharp
        // ILLMConversational interface methods - REQUIRED
        public bool ShouldHandleConversation(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;
            return from.InRange(this, HearingRange) && from.Player;
        }

        public void HandleConversation(SpeechEventArgs e)
        {
            // CRITICAL: This calls the LLM system
            LLMConversationHelper.ProcessConversation(this, e.Mobile, e.Speech);
            e.Handled = true;
        }
```

**4.5: Serialization (REQUIRED for persistence)**

```csharp
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
```

---

## Build and Test Procedure

### Build Steps

```bash
cd D:\UO\ServUO
dotnet build
```

**Expected:** 0 errors (warnings about unreachable code are harmless)

### Testing Steps

**1. Server Restart (REQUIRED)**
```
Stop server (Ctrl+C)
Start server (dotnet run)
```

**2. Spawn NPC**
```
[add EmperorGarrickSteelarm
```

**3. Check Server Logs**

Look for these log entries:

```
[NPCKnowledge] Building knowledge base for Emperor at Wind
[LLMConversationHelper] Emperor Garrick Steelarm (Emperor) knowledge base loaded: X entries
```

**CORRECT:** Says "Emperor" not "Merchant"!

**4. Test Scripted Responses**

Say keywords to trigger scripted responses:

```
Say: "hello"
Expected: "Hail, {YourName}. I am Emperor Garrick Steelarm. What brings you before the Iron Throne?"

Say: "faction"
Expected: "The Ironclad Alliance unites technology and magic. Together, we shall bring prosperity to all Vystia."
```

**Verify:**
- ✅ Short response (no cutoff)
- ✅ First person ("I am")
- ✅ Appropriate tone
- ✅ Mentions Vystia

**5. Test LLM Integration**

Say non-keyword questions to trigger LLM:

```
Say: "Who are you?"
Say: "Tell me about your home"
Say: "What do you know about history?"
```

**Watch Server Logs for:**
```
[UnifiedLLM] Using default provider: OpenAI (preferLocal=False)
[LLMService] Building request for NPC: Emperor Garrick Steelarm
[LLMService] Sending request to OpenAI API...
[LLMService] Response status: OK
[LLMService] Extracted NPC response: '[response text]'
```

**Verify LLM Response:**
- ✅ Says "Vystia" (NOT "Britannia"!)
- ✅ Identifies correctly ("I am Emperor...")
- ✅ Appropriate personality (imperial, strategic)
- ✅ First person speech
- ✅ NO archaic English unless PersonalityType requires it
- ✅ Brief (1-2 sentences per system prompt)
- ✅ Uses knowledge from domain (history, politics, etc.)

---

## Common Issues and Solutions

### Issue 1: NPC Still Says "Britannia"

**Symptoms:**
- LLM response mentions "Britannia" instead of "Vystia"
- Generic UO references instead of Vystia lore

**Causes:**
1. PersonalityType not set in NPC class
2. Personality description doesn't mention "Vystia"
3. Domain knowledge doesn't mention "Vystia"
4. Old NPC instance (spawned before fixes)

**Solutions:**
1. Verify PersonalityType property is set in NPC constructor
2. Check GetBasePersonality() mentions "Vystia" explicitly
3. Check GetDomainKnowledgeSummary() mentions "Vystia"
4. Delete old NPC and spawn fresh one
5. Restart server to clear caches

### Issue 2: NPC Detected as Wrong Role

**Symptoms:**
- Server logs show "Merchant" instead of "Emperor"
- Wrong knowledge level (limited instead of comprehensive)

**Causes:**
1. PersonalityType enum not defined
2. InferRoleFromPersonality missing mapping
3. NPC class doesn't set PersonalityType

**Solutions:**
1. Add PersonalityType to enum (NPCPersonalities.cs)
2. Add case in InferRoleFromPersonality (NPCKnowledgeSystem.cs)
3. Set PersonalityType property in NPC class

### Issue 3: NPC Not Responding

**Symptoms:**
- HandleConversation takes 0ms
- No LLM response, no scripted response

**Causes:**
1. HandleConversation method is empty stub
2. ILLMConversational not implemented
3. LLMConversationEnabled = false

**Solutions:**
1. Call LLMConversationHelper.ProcessConversation in HandleConversation
2. Implement ILLMConversational interface fully
3. Set LLMConversationEnabled = true

### Issue 4: Responses Getting Cut Off

**Symptoms:**
- Scripted responses end mid-sentence
- Text appears truncated

**Causes:**
1. Response string too long (>100 chars)
2. Multiple concatenated responses

**Solutions:**
1. Keep all scripted responses under 100 characters
2. Use concise, direct language
3. Break long responses into multiple shorter ones

### Issue 5: Wrong Personality/Tone

**Symptoms:**
- Merchant dialogue instead of leader dialogue
- Wrong speech style (archaic when shouldn't be)

**Causes:**
1. SpeechPattern set incorrectly
2. Personality description doesn't match character
3. Old cached personality prompt

**Solutions:**
1. Use SpeechPattern.Formal for leaders (not OldEnglish)
2. Review GetBasePersonality description
3. Restart server to clear caches

---

## Quick Reference Checklist

### Before Building:

- [ ] PersonalityType enum added (NPCPersonalities.cs line ~230)
- [ ] Domain knowledge added (NPCPersonalities.cs line ~593)
- [ ] Base personality added (NPCPersonalities.cs line ~784)
- [ ] NPCRole enum added (NPCKnowledgeSystem.cs line ~37)
- [ ] Role mapping added (NPCKnowledgeSystem.cs line ~454)
- [ ] Knowledge domain case added (KnowledgeDomain.cs line ~117)
- [ ] Domain init method added if needed (KnowledgeDomain.cs line ~347)
- [ ] NPC class implements ILLMConversational
- [ ] PersonalityType and SpeechPattern set
- [ ] HandleConversation calls LLMConversationHelper.ProcessConversation
- [ ] Serialization includes LLM properties
- [ ] Scripted responses mention "Vystia"
- [ ] All descriptions mention "Vystia" not "Britannia"

### After Building:

- [ ] 0 build errors
- [ ] Server restarted
- [ ] Old NPC removed
- [ ] New NPC spawned
- [ ] Server logs show correct role (not Merchant)
- [ ] Scripted keywords work (hello, faction, etc.)
- [ ] LLM responses mention "Vystia"
- [ ] NPC speaks in first person
- [ ] Appropriate personality tone
- [ ] No archaic English (unless intended)

---

## Template Files for Copy/Paste

### Minimal NPC Class Template

```csharp
using System;
using System.Collections.Generic;
using Server.Items;
using Server.Services.LLM;

namespace Server.Mobiles
{
    public class [NPCName] : BaseVendor, ILLMConversational
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;

        // ILLMConversational implementation
        public bool LLMConversationEnabled { get; set; } = true;
        public NPCPersonalities.PersonalityType PersonalityType { get; set; } = NPCPersonalities.PersonalityType.[YourType];
        public NPCPersonalities.SpeechPattern SpeechPattern { get; set; } = NPCPersonalities.SpeechPattern.Formal;
        public int HearingRange { get; set; } = 8;

        [Constructable]
        public [NPCName]() : base("[Title]")
        {
            Name = "[NPC Name]";
            Title = "[Title]";
            Body = 0x190; // or 0x191 for female
            Hue = [hue];

            // Stats, skills, etc.
        }

        public override void InitSBInfo()
        {
            // Vendor items if needed
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            base.OnSpeech(e);
            Mobile from = e.Mobile;

            if (!e.Handled && from.InRange(this, 3))
            {
                string speech = e.Speech.ToLower();

                if (speech.Contains("hello"))
                {
                    Say($"[Greeting response mentioning Vystia]");
                    e.Handled = true;
                }
                // Add more keyword responses
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

        public [NPCName](Serial serial) : base(serial) { }

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
                PersonalityType = NPCPersonalities.PersonalityType.[YourType];
                SpeechPattern = NPCPersonalities.SpeechPattern.Formal;
                HearingRange = 8;
            }
        }
    }
}
```

---

## Next Steps

### Remaining Faction Leaders to Implement

Using this template, implement:

1. **Chieftain Bjorn Frostbeard** (Frosthold)
   - PersonalityType: Chieftain
   - SpeechPattern: Casual or Formal
   - Tone: Gruff warrior

2. **Elder Seraphina Leafwhisper** (Verdantpeak)
   - PersonalityType: Elder
   - SpeechPattern: Formal
   - Tone: Ancient, wise

3. **Sultan Azir al-Rashid** (Desert of Surya)
   - PersonalityType: Sultan
   - SpeechPattern: Formal
   - Tone: Diplomatic, shrewd

4. **Archmage Pyrus Ashborn** (Emberlands)
   - PersonalityType: Mage (or create Archmage)
   - SpeechPattern: Formal
   - Tone: Powerful, ambitious

All personality types and role mappings are already in place - just need to create the NPC class files!

---

## Reference: Working Example

**Fully implemented and tested:** Emperor Garrick Steelarm
**Location:** `ServUO/Scripts/Mobiles/Vystia/NPCs/FactionLeaders/EmperorGarrickSteelarm.cs`
**Status:** ✅ All tests passing, speaks correctly about Vystia, proper Emperor role and knowledge

Use this as the reference implementation for all future LLM-enabled NPCs.

---

*Template created 2025-12-08 based on successful Emperor Garrick Steelarm implementation*
*All code tested and verified working with OpenAI GPT-4o-mini*
