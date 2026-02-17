# Local LLM Setup Guide (Ollama)

This guide covers setting up Ollama for free, local LLM-powered NPCs with no API costs.

---

## Why Use Local LLMs?

### Benefits
- **Free**: No API costs, unlimited conversations
- **Fast**: 1-2 second responses with phi3:mini
- **Private**: All data stays on your server
- **Offline**: Works without internet connection
- **No Rate Limits**: No API quotas or throttling

### Drawbacks
- **RAM Required**: 8GB+ for 8B models, 16GB+ for 70B models
- **CPU/GPU Usage**: Ongoing compute load
- **Initial Setup**: More complex than OpenAI API key
- **Quality**: Slightly lower than GPT-4o for complex tasks

---

## Installation

### Step 1: Install Ollama

**Windows:**
1. Download from https://ollama.com/download/windows
2. Run the installer
3. Ollama will install to `C:\Users\<username>\AppData\Local\Programs\Ollama\`
4. Service starts automatically

**Linux:**
```bash
curl -fsSL https://ollama.com/install.sh | sh
```

**macOS:**
```bash
brew install ollama
```

### Step 2: Verify Installation

Open terminal/command prompt:
```bash
ollama --version
```

You should see version information like:
```
ollama version is 0.1.x
```

### Step 3: Start Ollama Service

**Windows**: Service starts automatically

**Linux/macOS**:
```bash
ollama serve
```

Leave this running in the background.

---

## Model Selection

### Recommended Models

| Model | Size | RAM | Speed | Quality | Best For |
|-------|------|-----|-------|---------|----------|
| **phi3:mini** | 3.8B | 8GB | 1-2s | Very Good | General conversations (RECOMMENDED) |
| llama3.1:8b | 8B | 16GB | 2-3s | Excellent | High quality conversations |
| mistral:7b | 7B | 16GB | 2-4s | Excellent | Complex reasoning |
| llama3.1:70b | 70B | 48GB+ | 5-10s | Outstanding | Best quality (server-grade) |

### Download Models

**Download phi3:mini (Recommended):**
```bash
ollama pull phi3:mini
```

**Download llama3.1:8b:**
```bash
ollama pull llama3.1:8b
```

**Download mistral:7b:**
```bash
ollama pull mistral:7b
```

**List Downloaded Models:**
```bash
ollama list
```

Output:
```
NAME            SIZE    MODIFIED
phi3:mini       2.3GB   2 days ago
llama3.1:8b     4.7GB   3 days ago
```

---

## ServUO Configuration

### Step 1: Start ServUO Once

This creates the config file at:
```
Config/LocalLLM.cfg
```

### Step 2: Edit Configuration

**Config/LocalLLM.cfg:**
```
# Local LLM Configuration (Ollama)
Endpoint=http://localhost:11434
Model=phi3:mini

# Available models (run 'ollama pull <model>' first):
# - phi3:mini (fastest, 1-2 sec, 3.8B params, RECOMMENDED)
# - llama3.1:8b (balanced, 2-3 sec, 8B params)
# - llama3.1:70b (best quality, 5-10 sec, 70B params, requires 48GB RAM)
# - mistral:7b (quality, 2-4 sec, 7B params)
```

### Step 3: Configure Provider Preference

In-game command:
```
[LLMConfig provider local
```

Or for automatic selection:
```
[LLMConfig provider auto
[LLMConfig preferlocal true
```

### Step 4: Restart ServUO

Restart the server to apply configuration changes.

---

## Testing

### Test Connection

In-game:
```
[LLMConfig test local
```

Expected output:
```
Testing local provider...
Test successful! Response: <greeting from NPC>
```

### Test NPC

1. Spawn a test NPC:
```
[SpawnPersonalityNPC Blacksmith Archaic
```

2. Talk to the NPC:
```
Player: "Hello!"
NPC: *ponders*
NPC: "Greetings, traveler! Welcome to my forge..."
```

Watch console for timing:
```
[LocalLLM] Response time: 1523ms
```

---

## Performance Tuning

### Model Options

You can customize model behavior in `LocalLLMService.cs`:

```csharp
var options = new
{
    temperature = 0.7,      // Creativity (0.0-1.0)
    top_k = 40,             // Token sampling
    top_p = 0.9,            // Nucleus sampling
    num_predict = 150       // Max tokens
};
```

### Hardware Recommendations

**Minimum (phi3:mini):**
- CPU: 4 cores, 2.0 GHz+
- RAM: 8GB
- Disk: 5GB free (for models)

**Recommended (llama3.1:8b):**
- CPU: 8 cores, 3.0 GHz+ OR GPU
- RAM: 16GB
- Disk: 10GB free

**Optimal (llama3.1:70b):**
- CPU: 16+ cores OR GPU with 48GB VRAM
- RAM: 64GB
- Disk: 50GB free

### GPU Acceleration

Ollama automatically uses GPU if available:

**NVIDIA (CUDA):**
- Install CUDA toolkit
- Ollama detects automatically
- 10-20x faster responses

**AMD (ROCm):**
- Install ROCm drivers
- Set environment: `HSA_OVERRIDE_GFX_VERSION=10.3.0`

**Apple Silicon (Metal):**
- Native support on M1/M2/M3
- Excellent performance

Check GPU usage:
```bash
ollama ps
```

---

## Advanced Configuration

### Custom Endpoint

If running Ollama on different machine:

**Config/LocalLLM.cfg:**
```
Endpoint=http://192.168.1.100:11434
Model=phi3:mini
```

### Multiple Models

Switch models per request type (edit `UnifiedLLMService.cs`):

```csharp
// Use phi3:mini for simple conversations
if (requestType == RequestType.SimpleGreeting)
{
    model = "phi3:mini";
}
// Use llama3.1:8b for complex dialogue
else if (requestType == RequestType.QuestDialogue)
{
    model = "llama3.1:8b";
}
```

### Timeout Configuration

For slower models, increase timeout in `LocalLLMService.cs`:

```csharp
private static readonly int DefaultTimeout = 120000; // 2 minutes
```

---

## Model Comparison

### Response Quality Test

**Prompt**: "Tell me about the art of blacksmithing"

**phi3:mini (1.5s)**:
```
"Ah, the craft of forging! I've worked metal for twenty years. Each blade has a soul,
shaped by fire and hammer. The heat reveals the true quality of the steel."
```
*Quality: Very Good, Natural, Appropriate*

**llama3.1:8b (2.8s)**:
```
"Greetings, traveler! Blacksmithing is the ancient art of shaping metal through heat
and force. I've studied the properties of different ores - iron for durability, steel
for strength, valorite for magic resistance. Each piece requires understanding the
metal's nature and coaxing it into form with precise hammer strikes."
```
*Quality: Excellent, Detailed, Immersive*

**mistral:7b (3.2s)**:
```
"The forge is where raw ore becomes art and function intertwined. I've mastered
the temperatures needed for each metal - iron glows orange at 1500 degrees, steel
must reach yellow-white heat for proper forging. The rhythm of the hammer is like
a heartbeat, each strike purposeful and measured. Would you like to know more
about specific techniques?"
```
*Quality: Excellent, Technical, Engaging*

### Cost Comparison

| Provider | Model | Cost/1K Convs | Response Time |
|----------|-------|---------------|---------------|
| OpenAI | gpt-4o-mini | $1.00 | 1-3s |
| Ollama | phi3:mini | $0.00 | 1-2s |
| Ollama | llama3.1:8b | $0.00 | 2-3s |
| Ollama | mistral:7b | $0.00 | 2-4s |

**Electricity Cost Estimate:**
- Idle: ~50W
- Active (CPU): ~150-200W
- Active (GPU): ~250-400W
- Cost: ~$0.02-0.05 per hour (depending on electricity rates)

---

## Troubleshooting

### Ollama Not Running

**Symptom**: "Connection refused" or "Cannot connect to Ollama"

**Solution**:
```bash
# Check if Ollama is running
ollama ps

# If not running, start it:
ollama serve
```

**Windows**: Check Task Manager for Ollama process

### Model Not Found

**Symptom**: "Model not found: phi3:mini"

**Solution**:
```bash
# List models
ollama list

# Pull missing model
ollama pull phi3:mini
```

### Slow Responses

**Symptom**: Responses taking 10+ seconds

**Possible Causes:**
1. **Wrong model**: Check you're using phi3:mini, not llama3.1:70b
2. **No GPU**: CPU inference is slower
3. **Low RAM**: System swapping to disk
4. **Other processes**: CPU/RAM used by other apps

**Check model size**:
```bash
ollama list
```

**Monitor resources**:
- Windows: Task Manager
- Linux: `htop` or `top`
- macOS: Activity Monitor

### Out of Memory

**Symptom**: Ollama crashes or "out of memory" errors

**Solutions**:
1. **Use smaller model**:
```bash
ollama pull phi3:mini  # Instead of llama3.1:70b
```

2. **Close other applications**

3. **Increase system RAM**

4. **Use GPU** (if available)

### Port Already in Use

**Symptom**: "Address already in use: 11434"

**Solution**:
```bash
# Find process using port 11434
# Windows:
netstat -ano | findstr :11434

# Linux/macOS:
lsof -i :11434

# Kill the process or change port in Config/LocalLLM.cfg
Endpoint=http://localhost:11435
```

Then restart Ollama with custom port:
```bash
OLLAMA_HOST=0.0.0.0:11435 ollama serve
```

---

## Embedding Model (for Semantic Search)

### Install Embedding Model

For vector-based lore search:
```bash
ollama pull nomic-embed-text
```

This enables semantic lore search in the RAG system.

**Size**: ~274MB
**Speed**: ~100ms per embedding
**Quality**: Excellent for semantic matching

### Test Embedding

In-game:
```
[LoreStats
```

Look for:
```
Vector embeddings: AVAILABLE
Cached embeddings: 47/47
```

---

## Best Practices

### Model Selection Strategy

**For Most Servers:**
- Use **phi3:mini** as default (fast, free, good quality)
- Enable OpenAI fallback for complex queries
- Cost: $0/month

**For High-Quality Servers:**
- Use **llama3.1:8b** for conversations (excellent quality)
- Use **phi3:mini** for simple greetings
- Cost: $0/month + electricity

**For Budget Servers:**
- Use **OpenAI gpt-4o-mini** only
- No local compute overhead
- Cost: ~$10-50/month depending on traffic

### Hybrid Approach (Recommended)

Configure smart routing:
```
[LLMConfig provider auto
[LLMConfig preferlocal true
```

This uses:
- **Local LLM**: Player conversations, vendor commands, simple greetings
- **OpenAI**: Complex quest dialogue, important story moments
- **Fallback**: Switches to OpenAI if local fails

### Resource Management

**Monitor Usage:**
```bash
# Check Ollama process
ollama ps

# View resource usage
# Windows: Task Manager
# Linux: htop
```

**Optimize:**
1. Use phi3:mini during peak hours
2. Switch to llama3.1:8b during off-peak
3. Monitor RAM usage
4. Close unused models: Ollama keeps last-used model in RAM

---

## Updating

### Update Ollama

**Windows**: Download new installer from https://ollama.com

**Linux/macOS**:
```bash
curl -fsSL https://ollama.com/install.sh | sh
```

### Update Models

Models improve over time. Update with:
```bash
ollama pull phi3:mini
ollama pull llama3.1:8b
```

Old versions are automatically replaced.

---

## Uninstalling

### Remove Ollama

**Windows**:
1. Programs → Uninstall Ollama
2. Delete: `C:\Users\<username>\.ollama`

**Linux**:
```bash
sudo systemctl stop ollama
sudo systemctl disable ollama
sudo rm /usr/local/bin/ollama
sudo rm -rf /usr/share/ollama
sudo rm /etc/systemd/system/ollama.service
```

**macOS**:
```bash
brew uninstall ollama
rm -rf ~/.ollama
```

### Remove Models

```bash
# List models
ollama list

# Remove specific model
ollama rm phi3:mini
```

---

## FAQ

**Q: Can I use both OpenAI and Ollama?**
A: Yes! Use `[LLMConfig provider auto` for smart routing.

**Q: Which model is best?**
A: phi3:mini for speed, llama3.1:8b for quality, mistral:7b for balance.

**Q: How much RAM do I need?**
A: 8GB minimum for phi3:mini, 16GB recommended for llama3.1:8b.

**Q: Can I run this on a VPS?**
A: Yes, but you need adequate RAM. Most VPS plans are too small.

**Q: Does this work offline?**
A: Yes! Once models are downloaded, no internet needed.

**Q: Can I use custom models?**
A: Yes, Ollama supports custom GGUF models via `ollama create`.

**Q: What about fine-tuning?**
A: Ollama doesn't support fine-tuning directly. Use base models only.

**Q: GPU required?**
A: No, but highly recommended for llama3.1:8b and larger.

---

## Additional Resources

- **Ollama Documentation**: https://github.com/ollama/ollama
- **Model Library**: https://ollama.com/library
- **Community Discord**: https://discord.gg/ollama
- **Performance Tips**: https://github.com/ollama/ollama/blob/main/docs/faq.md

---

**Related Documentation:**
- [README.md](README.md) - Main documentation
- [00_INDEX.md](00_INDEX.md) - Documentation index
- [QUICKSTART.txt](QUICKSTART.txt) - Quick start guide
