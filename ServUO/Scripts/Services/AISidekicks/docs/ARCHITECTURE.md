# AI Sidekick Architecture Analysis

**Date:** 2025-11-23  
**Status:** Converting from Mobile-based to BaseCreature-based architecture

## Current Architecture Problems

### 1. Death Crash Issue

**Problem:** Server crashes when sidekick dies.

**Root Cause:**
- `BaseSidekick` inherits from `Mobile`, not `BaseCreature`
- `Mobile.OnDeath()` checks `m_Player` - if false, it calls `Delete()`
- `BaseCreature.OnDeath()` checks `IsBonded` - if true, it prevents deletion and sets `IsDeadPet = true`
- `BaseSidekick` doesn't have `IsBonded` property, so it can't use the bonded pet death handling
- When sidekick is deleted, the `SidekickCreatureWrapper` (which is a `BaseCreature`) may still be referenced by AI, causing null reference exceptions

**Evidence:**
```csharp
// Mobile.OnDeath (line 4229)
if (!m_Player)
{
    Delete();  // Sidekick gets deleted if Player=false
}

// BaseCreature.OnDeath (line 6271)
if (IsBonded)
{
    IsDeadPet = true;  // Prevents deletion, allows resurrection
    // ... handles death properly
}
```

### 2. Command Flow Issues

**Problem:** Pet commands don't work reliably.

**Root Cause:**
- `BaseAI` expects `m_Mobile` to be a `BaseCreature`
- `BaseAI.OnSpeech()` checks `m_Mobile.Controlled` and `m_Mobile.Commandable` (BaseCreature properties)
- `BaseAI.EndPickTarget()` sets `m_Mobile.ControlTarget` and `m_Mobile.ControlOrder` (BaseCreature properties)
- We use `SidekickCreatureWrapper` (inherits `BaseCreature`) to bridge the gap
- Synchronization between `AutonomousSidekick` (Mobile) and `SidekickCreatureWrapper` (BaseCreature) is fragile:
  - Changes to sidekick properties must sync to wrapper
  - Changes to wrapper properties (from BaseAI) must sync back to sidekick
  - Multiple sync points create race conditions
  - Reflection is used to access private fields, which is error-prone

**Evidence:**
```csharp
// BaseAI.OnSpeech (line 650)
if (m_Mobile.Controlled && m_Mobile.Commandable)
{
    // Processes commands
    m_Mobile.ControlOrder = OrderType.Follow;  // Sets on wrapper
}

// SidekickCreatureWrapper uses reflection to sync
s_ControlledField.SetValue(this, sidekickControlled);  // Fragile
```

### 3. Architecture Complexity

**Problems:**
1. **Dual State Management:** Two objects (sidekick + wrapper) must stay in sync
2. **Reflection Overhead:** Using reflection to access private fields is slow and error-prone
3. **Type Mismatch:** BaseAI expects BaseCreature, but we're using Mobile
4. **Lifecycle Issues:** Wrapper and sidekick have different lifecycles
5. **Map State Issues:** Wrapper is created on Map.Internal, sidekick may be on world map

## Solution: Convert to BaseCreature

### Why BaseCreature?

1. **Native Pet Support:** BaseCreature has built-in pet command handling
2. **Death Handling:** IsBonded prevents deletion and enables resurrection
3. **AI Compatibility:** BaseAI works directly with BaseCreature
4. **No Wrapper Needed:** Eliminates synchronization issues
5. **Proven Architecture:** All pets in ServUO use BaseCreature

### What We Lose

1. **Player-like Behavior:** Some Mobile-specific features may need workarounds
2. **Resurrection:** Need to ensure IsBonded=true and proper death handling
3. **Custom Properties:** May need to override more BaseCreature methods

### What We Gain

1. **Stability:** No crashes from death handling
2. **Reliability:** Commands work natively
3. **Simplicity:** No wrapper, no synchronization
4. **Performance:** No reflection overhead
5. **Maintainability:** Standard ServUO architecture

## Conversion Plan

1. Change `BaseSidekick` to inherit from `BaseCreature` instead of `Mobile`
2. Remove `SidekickCreatureWrapper` class entirely
3. Update `SidekickAI` to work directly with `BaseCreature`
4. Set `IsBonded = true` in constructor to enable proper death handling
5. Override `OnDeath` to use BaseCreature's bonded pet handling
6. Ensure `Commandable = true` for pet commands
7. Test all pet commands (follow, attack, stay, guard, etc.)
8. Test death and resurrection

## Implementation Notes

- `BaseCreature` already has all pet control properties (Controlled, ControlMaster, ControlOrder, etc.)
- `BaseCreature` already has `HandlesOnSpeech` and `OnSpeech` for command processing
- `BaseCreature` already has `IsBonded` for death handling
- We just need to ensure proper initialization and override specific behaviors

