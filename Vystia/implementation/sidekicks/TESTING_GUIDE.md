# AI Sidekick Testing Guide

## How to Interact with Your Sidekick

### 1. Talking to Your Sidekick (LLM Conversation)

**Method:** Just type in chat (no command needed)

Simply speak normally near your sidekick (within 10 tiles by default). The sidekick will hear you and respond using LLM.

**Examples:**
```
Hello!
What's your name?
How are you doing?
Follow me
Stay here
Attack that orc
```

**Note:** Make sure LLM service is enabled and configured. The sidekick uses the same LLM system as other LLM NPCs.

---

### 2. Pet Commands (Standard UO Pet System)

Since sidekicks use the pet control system, you can use standard pet commands:

**Commands:**
- `[name] follow` - Makes sidekick follow you
- `[name] stay` - Makes sidekick stay in place
- `[name] guard` - Makes sidekick guard you
- `[name] attack [target]` - Makes sidekick attack a target
- `[name] kill [target]` - Same as attack
- `[name] come` - Summons sidekick to your location

**Example:**
```
Aric follow
Aric stay
Aric attack that orc
```

---

### 3. Testing Following Behavior

1. **Walk around** - Your sidekick should automatically follow you
2. **Run away** - Sidekick should run to catch up
3. **Teleport far away** - Sidekick should teleport to you if >30 tiles away
4. **Say "stay"** - Sidekick should stop following
5. **Say "follow"** - Sidekick should resume following

---

### 4. Testing Combat

1. **Attack a creature** - Your sidekick should automatically engage
2. **Get attacked** - Sidekick should defend you
3. **Watch positioning** - Sidekick should maintain optimal range for their combat style:
   - Warriors: Close (1 tile)
   - Archers: Medium (5 tiles)
   - Mages: Far (10 tiles)

**Combat Commands:**
```
Attack that orc
Kill the skeleton
```

---

### 5. Testing Autonomous Behavior

The sidekick makes autonomous decisions every 5 seconds:

**What to Test:**
- **Idle behavior** - When not in combat, sidekick should follow or explore
- **Combat engagement** - When you're attacked, sidekick should help
- **Healing** - If you're low on health, sidekick should try to assist
- **Loot collection** - After combat, sidekick may collect nearby items (if implemented)

---

### 6. Testing LLM Conversation

**Basic Conversation:**
```
Hello [sidekick name]
What's your name?
How are you?
Tell me about yourself
```

**Commands via Speech:**
```
Follow me
Stay here
Attack that enemy
Guard me
How are you doing?
What's your status?
```

**Combat Commentary:**
- During combat, sidekick may make comments (if combat commentary is enabled)
- After combat, sidekick may comment on the fight

---

### 7. Testing Death & Resurrection

1. **Let sidekick die** - Sidekick should become a ghost (like a player)
2. **Resurrect** - Use `[Resurrect` command or cast Resurrection spell
3. **Sidekick should respond** - "Thank you for bringing me back, master!"

---

### 8. Testing Growth System

**Skills & Stats:**
- Sidekick gains skills through use (automatic)
- Sidekick gains stats through skill usage (automatic)
- Check stats with `[Props` command on sidekick
- Watch skills increase as sidekick fights/uses abilities

**To Check Stats:**
```
[Props [sidekick name]
```

---

### 9. Removing Sidekicks

**Method 1: Release Command (Recommended)**
```
[RemoveSidekick
[RemoveSidekick [name]
[ReleaseSidekick
[ReleaseSidekick [name]
```

**Method 2: Pet Release Command**
```
[name] release
```

**Method 3: Admin Delete**
```
[Delete [sidekick name]
[Remove [sidekick name]
```

**What Happens:**
- Sidekick says farewell message
- Sidekick is released from your control
- Sidekick is deleted after 1-2 seconds (DeleteOnRelease = true)

**Note:** Sidekicks are permanently deleted when released. They cannot be recovered.

---

### 10. Admin Commands for Testing

**Check Sidekick Properties:**
```
[Props [sidekick name]
```

**Force Commands:**
```
[Control [sidekick name]
```

**Check AI State:**
```
[Props [sidekick name]
```
Look for:
- `CurrentGoal` - Current autonomous goal
- `IsFollowing` - Whether following
- `CombatStyle` - Combat style
- `ArchetypeType` - Archetype

---

### 11. Common Issues & Solutions

**Sidekick not responding to speech:**
- Check LLM service is enabled
- Check sidekick is within hearing range (10 tiles default)
- Check `LLMConversationEnabled` property is true

**Sidekick not following:**
- Check `Owner` property is set to you
- Check `ControlMaster` is set
- Try `[name] follow` command

**Sidekick not fighting:**
- Check sidekick has a weapon (if melee/archer)
- Check sidekick has reagents (if mage)
- Check sidekick's AI is active

**Sidekick not using LLM:**
- Verify LLM service is initialized
- Check server console for LLM errors
- Verify API keys are configured (if using OpenAI)

---

## Quick Test Checklist

- [ ] Sidekick spawns successfully
- [ ] Sidekick follows when you walk
- [ ] Sidekick responds to speech (LLM)
- [ ] Sidekick engages in combat
- [ ] Sidekick uses appropriate combat style
- [ ] Sidekick can be commanded ("follow", "stay", "attack")
- [ ] Sidekick makes autonomous decisions
- [ ] Sidekick can die and be resurrected
- [ ] Sidekick stats/skills are visible with [Props
- [ ] Sidekick can be removed with [RemoveSidekick

---

## Example Test Session

```
1. Spawn sidekick: [CreateSidekick
2. Select "Warrior" archetype
3. Name: "Aric"
4. Click "Create Sidekick"

5. Walk around - sidekick should follow
6. Type: "Hello Aric" - should get LLM response
7. Type: "Follow me" - sidekick confirms
8. Attack a creature - sidekick should engage
9. Watch sidekick fight - should use melee combat
10. After combat, type: "Good job!" - should get response
```

---

## Debugging

**Check Console Output:**
- Look for `[AutonomousSidekick]` messages
- Look for `[LLMConversationHelper]` messages
- Look for `[SidekickAI]` debug messages

**Check Properties:**
```
[Props [sidekick name]
```

Key properties to check:
- `Owner` - Should be your player
- `ControlMaster` - Should be your player
- `ControlOrder` - Should be "Follow" or "Stay"
- `CurrentGoal` - Current autonomous goal
- `LLMConversationEnabled` - Should be true
- `HearingRange` - Default 10 tiles

