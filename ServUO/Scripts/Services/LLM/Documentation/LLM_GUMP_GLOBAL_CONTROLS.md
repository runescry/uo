# LLM Gump - Global Enable/Disable Controls

## Summary

Added **global enable/disable buttons** to the LLM NPC Gump (`[LLMMenu]`) allowing Game Masters to enable or disable LLM conversations for **all vendor NPCs** with a single click.

## What Was Added

### File Modified
**`D:\UO\ServUO\Scripts\Services\LLM\UI\LLMNpcGump.cs`**

### Changes Made

#### 1. New UI Section (lines 48-55)
Added "All Vendor NPCs" section with two buttons:
- **Enable All** button (green) - Button ID: 201
- **Disable All** button (red) - Button ID: 202

```csharp
// Global NPC Enable/Disable Section
AddHtml(20, 110, 200, 30, "<basefont color=#FFFFFF size=4><b>All Vendor NPCs:</b></basefont>", false, false);

AddButton(220, 110, 2117, 2118, 201, GumpButtonType.Reply, 0);
AddHtml(250, 110, 100, 30, "<basefont color=#00FF00 size=3>Enable All</basefont>", false, false);

AddButton(350, 110, 2117, 2118, 202, GumpButtonType.Reply, 0);
AddHtml(380, 110, 100, 30, "<basefont color=#FF0000 size=3>Disable All</basefont>", false, false);
```

#### 2. Button Handlers (lines 154-162)
Added cases for the two new buttons:

```csharp
case 201: // Enable All Vendor NPCs
    EnableDisableAllVendorNPCs(from, true);
    from.SendGump(new LLMNpcGump());
    break;

case 202: // Disable All Vendor NPCs
    EnableDisableAllVendorNPCs(from, false);
    from.SendGump(new LLMNpcGump());
    break;
```

#### 3. Helper Method (lines 166-186)
Added new method to iterate all vendor NPCs:

```csharp
/// <summary>
/// Enable or disable LLM conversations for all vendor NPCs in the world
/// </summary>
private static void EnableDisableAllVendorNPCs(Mobile from, bool enable)
{
    int count = 0;

    // Get all BaseVendor NPCs that implement ILLMConversational
    foreach (Mobile m in World.Mobiles.Values)
    {
        if (m is BaseVendor vendor && m is ILLMConversational llmNpc)
        {
            llmNpc.LLMConversationEnabled = enable;
            count++;
        }
    }

    string action = enable ? "ENABLED" : "DISABLED";
    from.SendMessage(0x22, $"LLM conversations {action} for {count} vendor NPCs.");
    Console.WriteLine($"[LLMNpcGump] {from.Name} {action} LLM for {count} vendor NPCs");
}
```

#### 4. UI Layout Adjustments
Adjusted Y positions of existing elements to accommodate the new section:
- Categories moved from Y:110 to Y:150 (40px down)
- Quick Spawn buttons moved from Y:400 to Y:430
- Info text moved from Y:485 to Y:505

## How It Works

### Enable All Vendor NPCs

1. GM opens `[LLMMenu]` gump
2. Clicks **"Enable All"** button (green)
3. System iterates through all mobiles in `World.Mobiles.Values`
4. For each BaseVendor that implements ILLMConversational:
   - Sets `LLMConversationEnabled = true`
   - Increments counter
5. Sends message to GM: "LLM conversations ENABLED for X vendor NPCs."
6. Logs to console: `[LLMNpcGump] PlayerName ENABLED LLM for X vendor NPCs`
7. Reopens gump

### Disable All Vendor NPCs

Same process, but sets `LLMConversationEnabled = false`

## UI Layout

```
┌─────────────────────────────────────────────────────┐
│           LLM NPC Spawner                           │
├─────────────────────────────────────────────────────┤
│                                                      │
│ Conversation Plugin:  ENABLED         [Disable]     │
│                                                      │
│ All Vendor NPCs:  [Enable All]  [Disable All]       │
│                                                      │
│ • Town NPCs                                          │
│ • Magic & Mystical                                   │
│ • Adventurers                                        │
│ • Merchants                                          │
│ • Underworld                                         │
│ • Special Characters                                 │
│ • Random NPC                                         │
│                                                      │
│        Quick Spawn Groups                            │
│ [Spawn Town Set] [Spawn Magic Set] [Spawn Advent.]  │
│                                                      │
│  Click a category to see available NPCs              │
└─────────────────────────────────────────────────────┘
```

## Benefits

### For Game Masters:
- **Mass control** - Enable/disable all vendor NPCs instantly
- **Easy testing** - Quickly toggle LLM on/off for all vendors
- **Performance management** - Disable all vendors during server maintenance
- **Debugging** - Isolate issues by turning conversations on/off

### For Server Management:
- **One-click operation** - No need to modify individual NPCs
- **Immediate feedback** - Shows count of affected NPCs
- **Console logging** - Audit trail of who enabled/disabled
- **Persistent changes** - Settings saved with vendor serialization

## Technical Details

### Scope
- **Only affects BaseVendor NPCs** that implement ILLMConversational
- **Does not affect** LLMNpc instances (spawned via gump)
- **Does not affect** BaseCreature NPCs that don't inherit from BaseVendor

### Performance
- **Single iteration** through World.Mobiles.Values
- **Fast type checking** using `is` operator
- **No database queries** - all in-memory operations
- **Immediate effect** - changes take effect on next player speech

### Persistence
- Changes are **saved with NPC serialization**
- **Survives server restarts**
- Each vendor's LLMConversationEnabled property is serialized (BaseVendor version 4)

## Example Usage

### Scenario 1: Testing New LLM Updates

```
GM opens [LLMMenu]
Clicks "Disable All" → "LLM conversations DISABLED for 143 vendor NPCs."
[Makes LLM configuration changes]
Clicks "Enable All" → "LLM conversations ENABLED for 143 vendor NPCs."
[Tests conversations with various vendors]
```

### Scenario 2: Performance Troubleshooting

```
Server experiencing lag
GM opens [LLMMenu]
Clicks "Disable All" to reduce LLM API calls
Monitors server performance
Re-enables when issue resolved
```

### Scenario 3: Selective NPC Management

```
Disable all vendors globally
Use [Props on specific vendors to enable individually
Create curated LLM experience in specific towns
```

## Integration with Existing Features

### Works With:
- ✅ BaseVendor ILLMConversational implementation
- ✅ Individual NPC `[Props` control (can override global setting)
- ✅ LLMConversationPlugin toggle (master switch)
- ✅ BaseAI OnSpeech hook (checks LLMConversationEnabled)
- ✅ Vendor serialization (persists across restarts)

### Hierarchy of Controls:
1. **LLMConversationPlugin.Enabled** (global master switch)
2. **Individual NPC's LLMConversationEnabled** (per-NPC setting)
3. Both must be true for LLM conversations to work

## Commands

### Open Gump:
```
[LLMMenu
[SpawnLLM
```

### Button IDs:
- **200**: Toggle LLMConversationPlugin (master switch)
- **201**: Enable All Vendor NPCs
- **202**: Disable All Vendor NPCs

## Future Enhancements

Possible additions:
- Status display showing "X of Y vendors enabled"
- Filter by facet/region
- Filter by NPC type (Blacksmiths only, Mages only, etc.)
- Batch enable/disable by personality type
- Undo last action

## Status

✅ **Implementation Complete**
✅ **Compiles Successfully**
✅ **No Build Errors**
⏳ **Ready for In-Game Testing**

---

**Quick Reference**: Use `[LLMMenu]` → Click **"Enable All"** or **"Disable All"** to toggle LLM conversations for all vendor NPCs in the world.
