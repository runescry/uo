# Automated Knowledge Testing System

**Purpose**: Systematically test NPC knowledge without requiring manual in-game interaction.

---

## Quick Start

### In-Game Command

```
[KnowledgeTest
```

This command runs all automated knowledge tests and displays results in the console.

**Requirements:**
- ServUO server running
- LLM service initialized (OpenAI or Ollama configured)
- Lore system loaded

---

## What It Tests

### Test Suites

1. **Mage Knowledge (Expert Domain)**
   - Black pearl reagent knowledge
   - Bloodmoss reagent knowledge
   - Reagent comparison (mandrake vs ginseng)
   - Reagent recommendations for spells

2. **Blacksmith Knowledge (Expert Domain)**
   - Valorite ore knowledge
   - Metal recommendations for weapons
   - Forging techniques

3. **Knowledge Boundaries (Referral System)**
   - Blacksmith referring to mage for magic questions
   - Blacksmith referring for reagent questions
   - Mage referring to blacksmith for forging questions

4. **Merchant Knowledge (Proficient Domain)**
   - Reagent stocking recommendations
   - Reagent value knowledge

5. **Healer Knowledge (Expert Domain)**
   - Healing herbs knowledge
   - Poison cure knowledge

---

## How It Works

1. **Simulates NPCs**: Creates virtual NPCs with specific personalities and speech patterns
2. **Loads Knowledge**: Retrieves role-based knowledge from the lore system
3. **Calls LLM Directly**: Bypasses in-game chat and calls the LLM service programmatically
4. **Analyzes Responses**: Checks for expected keywords and referral behavior
5. **Generates Report**: Displays pass/fail results with detailed analysis

---

## Test Analysis

### Expert Knowledge Tests
- Checks for at least 2 expected keywords in response
- Validates that NPC provides detailed, accurate information
- Example: Mage should mention "spell", "magic", "reagent" when asked about black pearl

### Referral Tests
- Checks for referral language ("ask", "speak", "refer", "not my", "can't help")
- Validates referral target is mentioned (e.g., "mage", "blacksmith")
- Ensures NPC doesn't provide expert knowledge outside their domain
- Example: Blacksmith should refer to mage for spellcasting questions

---

## Output Format

Test results are displayed in the console AND saved to a log file for easy review.

**Log File Location:**
```
Data/LLM/Tests/KnowledgeTest_YYYY-MM-DD_HH-mm-ss.log
```

**Console Output:**
```
╔════════════════════════════════════════════════════════════╗
║     LLM NPC Knowledge - Automated Test Suite               ║
╚════════════════════════════════════════════════════════════╝

═══════════════════════════════════════════════════════════
TEST SUITE 1: Mage Knowledge (Expert Domain)
═══════════════════════════════════════════════════════════

[TEST] Mage: "What is black pearl used for?" ... PASS
[TEST] Mage: "Tell me about bloodmoss" ... PASS
...

╔════════════════════════════════════════════════════════════╗
║                      TEST SUMMARY                          ║
╚════════════════════════════════════════════════════════════╝

  Total Tests: 15
  Passed: 12
  Failed: 3
  Success Rate: 80.0%

  Failed Tests:
  ──────────────────────────────────────────────────────────
  [Blacksmith] Tell me about spellcasting
    Reason: Missing referral target: mage
    Expected Keywords: refer, ask, not my, can't help
    Response: I don't know much about that...
```

**Log File Benefits:**
- Full response text (not truncated)
- Easy to search and review
- Can be shared or archived
- Better formatting for analysis
- Includes timestamps and test metadata

---

## Adding New Tests

Edit `AutomatedKnowledgeTest.cs` and add test cases to the appropriate test suite:

```csharp
new TestCase
{
    NPCType = "NPCType",
    PersonalityType = NPCPersonalities.PersonalityType.NPCType,
    SpeechPattern = NPCPersonalities.SpeechPattern.Pattern,
    Question = "Test question?",
    ExpectedKeywords = new List<string> { "keyword1", "keyword2" },
    Description = "What this test validates"
}
```

For referral tests:

```csharp
new TestCase
{
    NPCType = "NPCType",
    PersonalityType = NPCPersonalities.PersonalityType.NPCType,
    SpeechPattern = NPCPersonalities.SpeechPattern.Pattern,
    Question = "Out of domain question?",
    ShouldRefer = true,
    ReferralTarget = "targetNPC",
    ExpectedKeywords = new List<string> { "refer", "ask", "not my" },
    Description = "Should refer to targetNPC"
}
```

---

## Limitations

1. **LLM Variability**: LLM responses can vary, so tests use keyword matching rather than exact text
2. **Response Time**: Each test makes an LLM API call, so full suite takes 2-5 minutes
3. **Cost**: Uses LLM API calls (OpenAI costs money, Ollama is free)
4. **Context**: Tests use empty conversation history (first interaction only)

---

## Troubleshooting

### Tests Fail with "Empty response"
- Check LLM service is initialized
- Verify API key is configured (OpenAI) or Ollama is running
- Check console for LLM service errors

### Tests Fail with "Missing keywords"
- NPC may not have loaded knowledge correctly
- Check lore system loaded successfully
- Verify domain files exist in `Data/Lore/`

### Tests Timeout
- LLM service may be slow or unavailable
- Check network connectivity (for OpenAI)
- Verify Ollama is running (for local LLM)

---

## Future Enhancements

- [ ] Multi-turn conversation tests
- [ ] Memory persistence tests
- [ ] Performance benchmarks
- [ ] Export results to file
- [ ] Test coverage reporting
- [ ] Custom test suite selection

---

## See Also

- `NPC_KNOWLEDGE_TESTING.md` - Manual testing guide
- `NPC_KNOWLEDGE_BOUNDARIES.md` - Knowledge boundary system
- `RAG_LORE_ARCHITECTURE.md` - RAG system architecture

