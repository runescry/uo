# 🧠 Vystia LLM NPC Integration

Dynamic, AI-powered NPC dialogue system for the Vystia Ultima Online shard.

## Overview

This system enables NPCs to have natural, context-aware conversations with players using large language models (LLMs). NPCs can respond dynamically to player input while maintaining lore-consistent personalities.

## Features

- ✅ **Natural Dialogue**: LLM-powered responses for immersive conversations
- ✅ **Personality Profiles**: Each NPC type has distinct characteristics
- ✅ **Region-Aware**: Context-aware dialogue based on location
- ✅ **Quest Integration**: Hooks for quest-aware dialogue
- ✅ **Graceful Fallback**: Automatic fallback if LLM service unavailable
- ✅ **Logging**: Complete conversation logging for debugging

## Architecture

```
Player → ServUO → FastAPI Service → LLM Provider (OpenAI/GPT-4)
         (C#)      (Python)          (Remote)
```

## Installation

### 1. Python Service Setup

```bash
cd D:\UO\Vystia\services

# Install dependencies
pip install -r requirements.txt

# Set your OpenAI API key
export OPENAI_API_KEY="your-api-key-here"

# Start the service
uvicorn llm_server:app --host 0.0.0.0 --port 8000
```

### 2. ServUO Integration

1. Copy the C# scripts to your ServUO Scripts directory:
   - `Scripts/AI/AIChatHandler.cs`
   - `Scripts/Mobiles/NPCs/BlacksmithAI.cs`

2. Recompile your ServUO scripts:
   ```bash
   cd ServUO/Scripts
   csc /target:library *.cs
   ```

3. Test the system:
   - Spawn an LLM-powered NPC in-game
   - Use commands like: `[Add LLMBlacksmith`
   - Speak to the NPC by name

## Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `LLM_MODEL` | OpenAI model name | `gpt-4o-mini` |
| `MAX_TOKENS` | Response length limit | `100` |
| `TEMPERATURE` | Creativity (0-1) | `0.7` |
| `OPENAI_API_KEY` | Your API key | (required) |

### NPC Profiles

Edit `data/npc_profiles.json` to customize NPC personalities:

```json
{
  "blacksmith": {
    "tone": "gruff but helpful",
    "region": "Ironclad Empire",
    "context": "forges weapons and armour, wary of outsiders"
  }
}
```

## Usage

### Basic NPC Dialogue

Players can talk to LLM-powered NPCs by saying their name:

```
Player: Hello Borin
Borin: Aye, traveler. What brings you to my forge?

Player: Do you have any steel?
Borin: Steel? Aye, I've some fine Ironclad steel, if your coin's good.
```

### Customizing NPCs

Create new NPC types by:

1. Adding a profile to `npc_profiles.json`
2. Creating a new C# class inheriting from `BaseVendor`
3. Overriding `OnSpeech()` to call `AIChatHandler.QueryLLM()`

Example:
```csharp
public class LLMInnkeeper : BaseVendor
{
    public override void OnSpeech(SpeechEventArgs e)
    {
        if (e.Mobile.InRange(this, 4))
        {
            ProcessLLMDialogueAsync(e.Mobile, e.Speech);
        }
        base.OnSpeech(e);
    }
}
```

## Performance

- **Response Time**: < 4 seconds (remote API)
- **Fallback**: Automatic if API unavailable
- **Caching**: Profile data cached on startup
- **Throttling**: Built-in rate limiting per NPC

## Troubleshooting

### Service Not Responding

```bash
# Check if service is running
curl http://localhost:8000/

# Check logs
tail -f logs/npc_ai_chat.log
```

### API Key Issues

Ensure your `OPENAI_API_KEY` is set:
```bash
echo $OPENAI_API_KEY
```

### NPCs Not Responding

1. Verify NPC scripts compiled successfully
2. Check ServUO console for errors
3. Verify service is accessible from ServUO
4. Check `logs/npc_ai_chat.log` for API errors

## Development

### Reload Profiles Without Restart

```bash
curl -X POST http://localhost:8000/reload_profiles
```

### Test API Endpoint

```bash
curl -X POST http://localhost:8000/llm \
  -H "Content-Type: application/json" \
  -d '{
    "npc_name": "Borin",
    "npc_role": "blacksmith",
    "player_input": "Hello"
  }'
```

### Local LLM (Ollama)

If you prefer to run a local model:

1. Install Ollama: https://ollama.ai
2. Download a model: `ollama pull llama2`
3. Modify `llm_server.py` to use Ollama instead of OpenAI

## License

See main repository license.

## Credits

- **Framework**: FastAPI, ServUO
- **LLM**: OpenAI GPT-4 / Ollama
- **Author**: Marcus Bowles

---

For more information, see: `roadmap/LLM_NPC_FunctionalSpec.md`

