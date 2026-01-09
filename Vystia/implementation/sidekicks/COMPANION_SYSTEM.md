# Sidekick Companion System - Design Document

**Purpose:** Define how sidekicks function as true companions that grow and develop with the player.

**Date:** 2025-01-XX

---

## Core Concept

Sidekicks are **true companions** that:
- Start as new characters with the same stats/skills as player archetypes
- Gain skills and stats through use, just like players
- Can die and be resurrected
- Persist across sessions
- Grow and develop alongside their master

---

## Starting Stats & Skills

Sidekicks use the **exact same starting stats and skills** as player character archetypes, defined in `CharacterCreation.cs`:

### Warrior (Profession 1)
- **Stats:** STR=45, DEX=35, INT=10 (total=90)
- **Skills:** Anatomy=30, Healing=30, Swords=30, Tactics=30 (total=120)

### Magician (Profession 2)
- **Stats:** STR=25, DEX=20, INT=45 (total=90)
- **Skills:** EvalInt=30, Wrestling=30, Magery=30, Meditation=30 (total=120)

### Necromancer (Profession 4)
- **Stats:** STR=25, DEX=20, INT=45 (total=90)
- **Skills:** Necromancy=30, SpiritSpeak=30, Swords=30, Meditation=20 (total=110)

### Paladin (Profession 5)
- **Stats:** STR=45, DEX=20, INT=25 (total=90)
- **Skills:** Chivalry=30, Swords=30, Focus=30, Tactics=30 (total=120)

### Samurai (Profession 6)
- **Stats:** STR=40, DEX=30, INT=20 (total=90)
- **Skills:** Bushido=30, Swords=30, Anatomy=30, Healing=30 (total=120)

### Ninja (Profession 7)
- **Stats:** STR=40, DEX=30, INT=20 (total=90)
- **Skills:** Ninjitsu=30, Hiding=30, Fencing=30, Stealth=30 (total=120)

---

## Growth System

### Skill Gains

Sidekicks gain skills through the standard `SkillCheck.cs` system:
- Skills gain when used (just like players)
- Uses same skill gain mechanics as players
- No special handling needed - works automatically

**Code Reference:**
- `ServUO/Scripts/Misc/SkillCheck.cs` - Handles skill gains
- `SkillCheck.CheckSkill()` - Called when skills are used
- Skills gain based on difficulty and success

### Stat Gains

Sidekicks gain stats through the standard stat gain system:
- Stats gain when using skills (just like players)
- Uses `TryStatGain()` method from `SkillCheck.cs`
- Controlled creatures use `PetChanceToGainStats` (5% default)

**Code Reference:**
- `ServUO/Scripts/Misc/SkillCheck.cs` - `TryStatGain()` method
- `ServUO/Config/PlayerCaps.cfg` - `PetChanceToGainStats=5.0`
- Stats gain based on skill usage and stat locks

**Stat Locks:**
- Sidekicks should have stat locks set appropriately:
  - Warrior: STR=Up, DEX=Up, INT=Down
  - Mage: STR=Down, DEX=Down, INT=Up
  - etc.

---

## Death & Resurrection

### Death Mechanics

Sidekicks can die just like players:
- When killed, they become ghosts (if `Player = true`)
- Death penalties apply (stat/skill loss if configured)
- Corpse is created with equipment
- Can be resurrected

**Implementation:**
```csharp
public override void OnDeath(Container c)
{
    // Mark as player-like to prevent deletion
    if (!m_Player)
        m_Player = true;
    
    base.OnDeath(c);
    
    // Sidekick becomes ghost, can be resurrected
}
```

### Resurrection

Sidekicks can be resurrected:
- By owner using `Resurrect()` method
- By other players (if configured)
- Using standard resurrection mechanics

**Code Reference:**
- `ServUO/Server/Mobile.cs` - `Resurrect()` method (line 3646)
- `CheckResurrect()` - Override to control who can resurrect
- `OnBeforeResurrect()` / `OnAfterResurrect()` - Hooks for custom logic

**Implementation:**
```csharp
public override bool CheckResurrect()
{
    // Allow resurrection if owner is nearby
    if (ControlMaster != null && ControlMaster.InRange(this, 12))
        return true;
    
    // Or allow owner to resurrect from anywhere
    return ControlMaster != null;
}

public override void OnAfterResurrect()
{
    base.OnAfterResurrect();
    
    // Restore to full health/mana/stam
    Hits = HitsMax;
    Stam = StamMax;
    Mana = ManaMax;
    
    // LLM commentary
    if (_llmCore != null)
        _llmCore.Say("Thank you for bringing me back, master!");
}
```

---

## Persistence

### Serialization

Sidekicks must serialize all state:
- Stats and skills (automatically handled by `Mobile` base class)
- Equipment (automatically handled)
- LLM memory and personality (handle in `LLMSidekick` class)
- Relationship with owner (via `ControlMaster`)

**Implementation:**
```csharp
public override void Serialize(GenericWriter writer)
{
    base.Serialize(writer);
    writer.Write((int)0); // Version
    
    // Serialize LLM-specific data
    _llmCore?.Serialize(writer);
}

public override void Deserialize(GenericReader reader)
{
    base.Deserialize(reader);
    int version = reader.ReadInt();
    
    // Deserialize LLM-specific data
    if (version >= 0)
        _llmCore = new LLMNpc(reader);
}
```

---

## Implementation Details

### Character Creation

Use `CharacterCreation.cs` methods to initialize sidekicks:

```csharp
public LLMSidekick(ArchetypeType archetype) : base(AIType.AI_Use_Default, ...)
{
    // Set as player-like for resurrection
    Player = true;
    
    // Initialize stats based on archetype
    switch (archetype)
    {
        case ArchetypeType.Warrior:
            InitStats(45, 35, 10);
            SetSkill(SkillName.Anatomy, 30.0);
            SetSkill(SkillName.Healing, 30.0);
            SetSkill(SkillName.Swordsmanship, 30.0);
            SetSkill(SkillName.Tactics, 30.0);
            break;
        
        case ArchetypeType.Mage:
            InitStats(25, 20, 45);
            SetSkill(SkillName.EvalInt, 30.0);
            SetSkill(SkillName.Wrestling, 30.0);
            SetSkill(SkillName.Magery, 30.0);
            SetSkill(SkillName.Meditation, 30.0);
            break;
        
        // ... other archetypes
    }
    
    // Set stat locks for growth
    SetStatLocks(archetype);
    
    // Initialize LLM
    _llmCore = new LLMNpc(Name, GetPersonalityType(archetype), GetSpeechPattern(archetype));
}
```

### Stat Locks

Set stat locks to guide growth:

```csharp
private void SetStatLocks(ArchetypeType archetype)
{
    switch (archetype)
    {
        case ArchetypeType.Warrior:
            StrLock = StatLockType.Up;
            DexLock = StatLockType.Up;
            IntLock = StatLockType.Down;
            break;
        
        case ArchetypeType.Mage:
            StrLock = StatLockType.Down;
            DexLock = StatLockType.Down;
            IntLock = StatLockType.Up;
            break;
        
        // ... other archetypes
    }
}
```

---

## Code References

### Character Creation
- `ServUO/Scripts/Misc/CharacterCreation.cs` lines 389-451 - `SetStats()` method
- `ServUO/Scripts/Misc/CharacterCreation.cs` lines 453-533 - `SetSkills()` method

### Skill & Stat Gains
- `ServUO/Scripts/Misc/SkillCheck.cs` - Skill gain system
- `ServUO/Scripts/Misc/SkillCheck.cs` line 499 - `TryStatGain()` method
- `ServUO/Config/PlayerCaps.cfg` - Stat gain configuration

### Death & Resurrection
- `ServUO/Server/Mobile.cs` line 3646 - `Resurrect()` method
- `ServUO/Server/Mobile.cs` line 4196 - `OnDeath()` method
- `ServUO/Server/Mobile.cs` line 3627 - `CheckResurrect()` method

---

## Summary

Sidekicks are **true companions** that:
1. ✅ Start with same stats/skills as player archetypes
2. ✅ Gain skills and stats through use (automatic)
3. ✅ Can die and be resurrected
4. ✅ Persist across sessions
5. ✅ Grow and develop alongside their master

**No special handling needed** - the existing ServUO systems handle skill/stat gains, death, and resurrection automatically. We just need to:
- Initialize sidekicks with correct starting stats/skills
- Set `Player = true` to enable resurrection
- Set stat locks to guide growth
- Handle LLM-specific serialization

