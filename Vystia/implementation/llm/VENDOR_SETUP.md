# Vendor Integration Guide

LLM NPCs can function as vendors with natural language buy/sell commands, creating a more immersive shopping experience.

**Note**: All vendor NPCs (BaseVendor) automatically support LLM conversations. See "Technical Integration" section below for details.

---

## Overview

Traditional UO vendors require specific keywords:
- "vendor buy" → opens buy gump
- "vendor sell" → opens sell gump

LLM vendors understand natural language:
- "I'd like to buy a sword"
- "Can I sell some ore?"
- "Show me your wares"
- "What do you have for sale?"

---

## How It Works

### 1. Natural Language Detection

When a player talks to an LLM vendor:

```
Player: "I'd like to buy a sword"
    │
    ▼
LLM processes request with vendor context
    │
    ▼
LLM response: "Of course! I have fine blades for sale. [VENDOR_BUY]"
    │
    ▼
System detects [VENDOR_BUY] marker
    │
    ▼
Opens vendor buy gump automatically
```

### 2. Intent Markers

The LLM is instructed to include markers in its response:

**Buy Intent**:
```
Player wants to buy → LLM includes [VENDOR_BUY] or (vendor_buy)
```

**Sell Intent**:
```
Player wants to sell → LLM includes [VENDOR_SELL] or (vendor_sell)
```

**Markers are stripped** from the displayed text, so players only see the natural dialogue.

---

## Creating Vendor NPCs

### Method 1: Personality-Based

Some personalities automatically have vendor inventory:

```csharp
// Blacksmith personality → SBBlacksmith inventory
[SpawnPersonalityNPC Blacksmith Archaic

// Mage personality → SBMage inventory
[SpawnPersonalityNPC Mage Formal

// Healer personality → SBHealer inventory
[SpawnPersonalityNPC Healer Casual
```

**Auto-Assigned Inventories**:
- **Blacksmith** → SBBlacksmith (weapons, armor, metal items)
- **Weaponsmith** → SBBlacksmith (weapons focus)
- **Armorer** → SBBlacksmith (armor focus)
- **Mage** → SBMage (reagents, scrolls, spellbooks)
- **Healer** → SBHealer (bandages, potions, reagents)
- **Herbalist** → SBHealer (herbs, potions)

### Method 2: Manual Inventory

Create NPC with custom SBInfo:

```csharp
Type[] vendorInventory = new Type[] { typeof(SBBlacksmith) };
LLMNpc npc = new LLMNpc(
    "Gareth the Smith",
    "You are a skilled blacksmith...",
    vendorInventory
);
```

### Method 3: UI Gump

```
[spawnllmnpc
→ Select "Town NPCs"
→ Click "Blacksmith"
→ Place NPC (auto-has SBBlacksmith inventory)
```

---

## Vendor Prompts

### System Instruction

When an NPC is a vendor, the LLM receives this instruction:

```
You are a vendor who sells items. When a player wants to:
- Buy items: Include [VENDOR_BUY] in your response
- Sell items: Include [VENDOR_SELL] in your response

The marker will be invisible to the player, so write your response naturally,
then add the marker at the end.

Example:
Player: "I need a sword"
You: "Of course! I have excellent blades in stock. [VENDOR_BUY]"
```

### Context Awareness

Vendors know what they sell:

```
You are Gareth the Blacksmith.
You sell: weapons, armor, shields, metal items
You buy: ore, ingots, old weapons, scrap metal

When players ask what you sell, mention your specialty.
```

---

## Example Conversations

### Buying Items

```
Player: "Hello Gareth!"
Gareth: "Greetings, traveler! Welcome to my forge. I craft the finest weapons
         and armor in all of Britannia!"

Player: "I need a new sword"
Gareth: "Ah, looking for a blade! I have several excellent swords in stock -
         from simple longswords to mighty two-handed greatswords. Let me show
         ye what I've got!"
*Opens vendor buy gump*

Player: "Show me"
*Buy gump displays inventory*
```

### Selling Items

```
Player: "Can I sell you some ore?"
Gareth: "Ore? Aye, I'm always in need of good ore for the forge! Let's see
         what ye have."
*Opens vendor sell gump*

Player: "I have iron ingots"
*Sell gump shows player's eligible items*
```

### No Inventory to Sell

```
Player: "I'd like to sell some items"
Gareth: "I'd be happy to buy from ye, but it seems ye don't have anything I need
         at the moment. I buy ore, ingots, and old weapons if ye find any!"
*No gump opens - system message displays instead*
```

---

## Response Processing

### Meta-Commentary Stripping

The system removes LLM meta-commentary:

**Raw LLM Response**:
```
"Let me show you my wares! [VENDOR_BUY]

### EXPLANATION:
The player wants to buy, so I'm opening the vendor interface.
This is a buy transaction."
```

**Displayed to Player**:
```
"Let me show you my wares!"
*Opens vendor buy gump*
```

**Stripped Content**:
- `[VENDOR_BUY]` marker
- Everything after `###` (markdown headers)
- Lines starting with "EXPLANATION:", "NOTE:", "META:", etc.
- Separator lines (`---`, `===`)

### Code Reference

**File**: `Core/LLMNpc.cs` → `StripVendorCommands()`

```csharp
private string StripVendorCommands(string response)
{
    // Remove [VENDOR_BUY], [VENDOR_SELL] markers
    response = response.Replace("[VENDOR_BUY]", "");
    response = response.Replace("[vendor_buy]", "");
    response = response.Replace("[VENDOR_SELL]", "");
    response = response.Replace("[vendor_sell]", "");

    // Remove markdown headers and meta-commentary
    int markdownIndex = response.IndexOf("###");
    if (markdownIndex >= 0)
        response = response.Substring(0, markdownIndex);

    // Remove meta tags
    string[] metaTags = { "EXPLANATION:", "NOTE:", "META:", ... };
    // ... (see code for full implementation)

    return response.Trim();
}
```

---

## Vendor Inventory Setup

### Using SBInfo Classes

ServUO vendors use `SBInfo` classes to define inventory. LLM vendors work the same way:

**Blacksmith Example**:
```csharp
public override void InitSBInfo()
{
    m_SBInfos.Add(new SBBlacksmith(this));
    // Automatically sells: weapons, armor, repair deeds
    // Automatically buys: ore, ingots, old weapons
}
```

**Available SBInfo Classes** (ServUO standard):
- `SBBlacksmith` - Weapons, armor, shields
- `SBMage` - Reagents, scrolls, spellbooks
- `SBHealer` - Bandages, potions, cure/heal items
- `SBAlchemist` - Potions, alchemy supplies
- `SBTinker` - Tools, lockpicks, traps
- `SBCarpenter` - Furniture, wood items
- `SBTailor` - Cloth, clothing, dyes
- `SBProvisioner` - Food, drink, supplies
- And many more...

### Custom Inventory

Create custom SBInfo for unique vendors:

```csharp
public class SBMagicWeapons : SBInfo
{
    public override IShopSellInfo SellInfo { get; } = new InternalSellInfo();
    public override List<GenericBuyInfo> BuyInfo { get; } = new InternalBuyInfo();

    public class InternalBuyInfo : List<GenericBuyInfo>
    {
        public InternalBuyInfo()
        {
            Add(new GenericBuyInfo("Katana of Vanquishing", typeof(Katana), 5000, 20));
            Add(new GenericBuyInfo("Magic Bow", typeof(Bow), 3000, 20));
            // ... more magic weapons
        }
    }

    public class InternalSellInfo : GenericSellInfo
    {
        public InternalSellInfo()
        {
            Add(typeof(Katana), 250);
            Add(typeof(Bow), 150);
            // ... what vendor buys
        }
    }
}
```

Then use:
```csharp
Type[] inventory = new Type[] { typeof(SBMagicWeapons) };
LLMNpc vendor = new LLMNpc("Mystara the Enchantress", personality, inventory);
```

---

## Personality-to-Inventory Mapping

**File**: `Core/LLMNpc.cs` → `GetVendorInventoryForPersonality()`

```csharp
private Type[] GetVendorInventoryForPersonality(PersonalityType personality)
{
    switch (personality)
    {
        case PersonalityType.Merchant:
            return new Type[] { typeof(SBBlacksmith) };  // Generic goods

        case PersonalityType.Mage:
            return new Type[] { typeof(SBMage) };

        case PersonalityType.Healer:
            return new Type[] { typeof(SBHealer) };

        case PersonalityType.Blacksmith:
            return new Type[] { typeof(SBBlacksmith) };

        // Name-based inference for non-profession personalities
        default:
            if (this.Name.ToLower().Contains("blacksmith"))
                return new Type[] { typeof(SBBlacksmith) };

            if (this.Name.ToLower().Contains("mage"))
                return new Type[] { typeof(SBMage) };

            return null;  // Not a vendor
    }
}
```

---

## Testing Vendor Functionality

### Test Vendor Creation

```
1. Spawn vendor NPC:
[SpawnPersonalityNPC Blacksmith Archaic

2. Check console:
[LLMNpc] InitSBInfo called for Gareth, adding 1 SBInfo types
[LLMNpc] Created SBBlacksmith with Mobile parameter
[LLMNpc] Successfully added SBInfo: SBBlacksmith

3. Verify with props:
[props <npc>
Check: m_SBInfos count > 0
```

### Test Buy Command

```
1. Talk to vendor:
Player: "I'd like to buy a sword"

2. Watch console:
[LLMNpc] Vendor action detected: Buy
[LLMNpc] Opening vendor buy for PlayerName
[LLMNpc] SBInfos count: 1

3. Verify gump opens with inventory
```

### Test Sell Command

```
1. Talk to vendor:
Player: "Can I sell some items?"

2. Watch console:
[LLMNpc] Vendor action detected: Sell
[LLMNpc] Opening vendor sell for PlayerName

3. Verify sell gump opens
```

### Test No Inventory

```
1. Empty backpack
2. Talk to vendor:
Player: "I want to sell something"

3. Expect:
- Console: "Player has nothing to sell, skipping LLM response"
- Game: System message "You have nothing I would be interested in."
- No LLM response (prevents confusing dialogue)
```

---

## Advanced Features

### Conditional Pricing

Vendors can reference relationship for pricing:

```csharp
// In custom SBInfo (future enhancement):
public override int GetBuyPrice(Item item, Mobile buyer)
{
    int basePrice = base.GetBuyPrice(item, buyer);

    // Check relationship (if memory system enabled)
    var relationship = LLMMemoryService.GetRelationshipAsync(npcName, buyer.Name).Result;

    if (relationship != null && relationship.Score > 75)
        return (int)(basePrice * 0.8);  // 20% discount for allies

    if (relationship != null && relationship.Score < -50)
        return (int)(basePrice * 1.5);  // 50% markup for enemies

    return basePrice;
}
```

### Stock Awareness

Vendors know their inventory:

```
Gareth (checking stock):
"I have 5 longswords, 3 katanas, and 2 greatswords in stock right now.
 My specialty is two-handed weapons - I forge the best in the region!"
```

This requires custom stock tracking (future enhancement).

---

## Troubleshooting

### Vendor Commands Not Working

**Symptom**: Player says "buy" but gump doesn't open

**Check**:
1. Is NPC actually a vendor?
```
[props <npc>
Check: m_SBInfos count > 0
```

2. Check console for LLM response:
```
[LLMNpc] Received response: '<text>'
```
Does it contain `[VENDOR_BUY]` or `[VENDOR_SELL]`?

3. Verify LLM knows it's a vendor:
```
Console: [LLMNpc] isVendor=true
```

**Solutions**:
- Ensure personality or manual inventory set
- Check SBInfo loaded: Console should show "InitSBInfo called"
- Test with explicit phrase: "vendor buy"

### Vendor Gump Empty

**Symptom**: Gump opens but has no items

**Check**:
1. Verify SBInfo has buy info:
```
[props <npc>
Navigate to: m_SBInfos[0]
Check: BuyInfo count > 0
```

2. Check gold on vendor:
```
Vendors need gold to buy from players
```

**Solutions**:
- Verify correct SBInfo type for personality
- Check SBInfo implementation has items
- Give vendor gold if buying from players

### Wrong Inventory

**Symptom**: Blacksmith sells magic items instead of weapons

**Check**:
```
[props <npc>
m_VendorInventory: <Type[]>
```

**Solutions**:
- Verify personality type matches intended inventory
- Check `GetVendorInventoryForPersonality()` logic
- Manually set correct inventory type

---

## Best Practices

1. **Match Personality to Inventory**: Blacksmith sells weapons, Mage sells magic
2. **Test Vendor Commands**: Verify buy/sell work after spawning
3. **Clear Instructions**: Tell players vendors understand natural language
4. **Fallback to Keywords**: Traditional "vendor buy" still works
5. **Monitor Console**: Watch for intent detection issues
6. **Stock Management**: Restock vendors periodically (standard UO mechanics)

---

## Future Enhancements

### Planned Features

**Haggling System**:
```
Player: "That's too expensive!"
Vendor: "Well... for you, I could lower the price to 450 gold."
```

**Stock Commentary**:
```
Player: "Do you have any katanas?"
Vendor: "I have 3 katanas left - just sold 2 today! They're popular!"
```

**Trade Recommendations**:
```
Player: "What should I buy for fighting orcs?"
Vendor: "For orcs? I'd recommend a good mace - they're weak to crushing weapons.
         I have a fine waraxe that would serve you well!"
```

**Repair Integration**:
```
Player: "Can you repair my sword?"
Vendor: *examines blade* "Aye, I can fix this. It'll cost 50 gold and take a moment."
```

---

## Technical Integration

### BaseVendor Integration

All vendor NPCs in ServUO automatically support LLM conversations through the `ILLMConversational` interface integrated into `BaseVendor.cs`.

**Key Features:**
- Automatic personality inference from vendor class name or title
- GM-configurable properties: `LLMConversationEnabled`, `PersonalityType`, `SpeechPattern`, `HearingRange`
- Backwards compatible serialization (version 4)
- No code changes needed for individual vendor types

**Personality Inference:**
The system automatically maps vendor types to appropriate personalities:
- `Blacksmith` class → Blacksmith personality
- `Mage` class → Mage personality
- Generic vendor with title "the bard" → Bard personality

See [NPC_TITLES_REFERENCE.md](NPC_TITLES_REFERENCE.md) for complete title-to-personality mappings.

**Configuration:**
Vendors can be configured via properties:
- `[props` on vendor → Set `LLMConversationEnabled` to true/false
- `[props` → Set `PersonalityType` to override auto-detection
- `[props` → Set `SpeechPattern` (Modern, Formal, Archaic, etc.)

**Files Modified:**
- `Scripts/Mobiles/NPCs/BaseVendor.cs` - Added `ILLMConversational` interface implementation

**Impact:**
- ✅ 78+ vendor NPCs now have LLM conversations
- ✅ Automatic personality assignment
- ✅ Knowledge boundaries implemented
- ✅ Referral system active
- ✅ GM controls available
- ✅ Backwards compatible serialization

---

**Related Documentation:**
- [README.md](README.md) - Main documentation
- [PERSONALITY_GUIDE.md](PERSONALITY_GUIDE.md) - Personality system
- [NPC_TITLES_REFERENCE.md](NPC_TITLES_REFERENCE.md) - Title mappings
- [00_INDEX.md](00_INDEX.md) - Documentation index
