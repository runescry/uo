# Vystia Test Suite Execution Guide

## Overview

The Vystia Test Suite has been expanded from 22 tests to ~54 tests, increasing automation coverage from ~25% to ~70%. This guide explains how to execute and use the test suite.

## Quick Start - Command Sequence

### Step-by-Step Execution

1. **Start the ServUO Server**
   ```bash
   # Build and start the server
   # Wait for all scripts to compile and load
   ```

2. **Verify Test Suite Initialized**
   - Check server console for: `[Vystia Tests] Test suite initialized. Use [TestVystiaSystems to run tests.`

3. **Log In as GM Character**
   - Connect to server
   - Use account with `AccessLevel.GameMaster` or higher

4. **Run Automated Tests**
   ```
   [TestVystiaSystems all
   ```
   Or test specific categories:
   ```
   [TestVystiaSystems faction
   [TestVystiaSystems religion
   [TestVystiaSystems crafting
   [TestVystiaSystems reward
   [TestVystiaSystems time
   ```

5. **Review Results**
   - Check in-game output for pass/fail summary
   - Review detailed logs: `Logs/VystiaTests/TestSuite_*.log`

### Test Categories

- **Faction Tests** (11 tests): Enum values, tier thresholds, vendor discounts, reputation caps, enemy relationships, data integrity, tokens, tier-gated recipes, titles, exalted items
- **Religion Tests** (11 tests): Enum values, tier thresholds, piety calculation, opposed religions, devotion powers, piety caps, cooldowns, data integrity, power registration, portable shrines, major temples
- **Crafting Tests** (13 tests): Ore/ingot types, smelting ratios, material properties, discipline existence, recipe existence, potion classes, construct classes, portable shrines, major temples
- **Reward Tests** (9 tests): Boss kill rewards (reputation & piety), donation rewards, PvP kill rewards, tier-gated access, title awarding, item creation
- **Time Tests** (6 tests): Pilgrimage cooldown, prayer cooldown, tithe daily cap, PvP kill cooldown, fury decay, devotion power cooldown

## Manual Test Setup Commands

### Crafting Test Setup
```
[TestCraftingSetup <discipline>
```
Spawns test materials and workstation for a specific crafting discipline.

### Potion Test Setup
```
[TestPotionSetup
```
Spawns all 14 resource potions in your backpack.

### Construct Test Setup
```
[TestConstructSetup
```
Spawns all 5 engineering construct cores in your backpack.

### Devotion Power Test Setup
```
[TestDevotionPowerSetup <religion>
```
Sets your piety to Exalted tier (900+) for testing devotion powers.

### Faction Test Setup
```
[TestFactionSetup <faction> <tier>
```
Sets your faction reputation to a specific tier (friendly, honored, revered, exalted).

### Integration Test Setup
```
[TestIntegrationSetup
```
Sets up a complex scenario with both faction and religion at Exalted tier.

## Recording Manual Test Results

### Record a Test Result
```
[TestResults <testname> <pass/fail> [notes]
```
Example:
```
[TestResults PotionCreation pass All 14 potions work correctly
[TestResults ConstructAI fail Repair Drone not healing other constructs
```

### Generate Test Report
```
[TestReport [category]
```
Generates a report of all recorded test results. Use `[TestReport all` for all results, or specify a category.

## Test Files Created

### Automated Test Files
- `VystiaSystemsTestSuite.cs` - Main test suite (expanded from 1,105 to ~2,230 lines)
- `TestHelpers.cs` - Test player creation and event simulation utilities
- `TimeTestHelpers.cs` - Time-based testing utilities

### Manual Test Files
- `VystiaTestScripts.cs` - GM commands for manual test setup
- `TestResultsTracker.cs` - Manual test result tracking and reporting
- `MANUAL_TEST_SCRIPTS.md` - Step-by-step guides for manual testing

## Test Coverage Summary

### Phase 1: File/Class Existence Tests ✅
- 6 new crafting tests
- 3 new religion tests
- 4 new faction tests
- **Total: +13 tests**

### Phase 2: Reward Simulation Tests ✅
- 9 new reward system tests
- Test helper infrastructure
- **Total: +9 tests**

### Phase 3: Time-Based Tests ✅
- 6 new time-based tests
- Time manipulation utilities
- **Total: +6 tests**

### Phase 4: Manual Test Scripts ✅
- 6 GM setup commands
- Test result tracking system
- Comprehensive documentation

## Expected Test Results

When running `[TestVystiaSystems all`, you should see:

```
=== Vystia Systems Test Suite ===
Running all tests...

=== Test Results ===
Passed: 54 | Failed: 0 | Total: 54

--- Faction Tests (11/11) ---
[PASS] FactionEnumValues
[PASS] ReputationTierThresholds
...

--- Religion Tests (11/11) ---
[PASS] ReligionEnumValues
[PASS] PietyTierThresholds
...

--- Crafting Tests (13/13) ---
[PASS] OreTypesExist
[PASS] IngotTypesExist
...

--- Reward Tests (9/9) ---
[PASS] BossKillReputationReward
[PASS] BossKillPietyReward
...

--- Time Tests (6/6) ---
[PASS] PilgrimageCooldown
[PASS] PrayerCooldown
...

All 54 tests passed!
```

## Troubleshooting

### Tests Fail to Run
- Ensure you're logged in as a GM
- Check that all Vystia systems are initialized
- Verify server is running correctly

### Test Helpers Not Found
- Ensure `TestHelpers.cs` is compiled
- Check namespace: `Server.Custom.VystiaClasses.Testing`

### Manual Test Commands Not Working
- Verify `VystiaTestScripts.Initialize()` is called
- Check that `TestVystiaSystemsCommand.Initialize()` includes test script initialization

## Next Steps

1. **Run the automated test suite** to verify all systems
2. **Use manual test setup commands** for in-game verification
3. **Record manual test results** using `[TestResults`
4. **Generate reports** using `[TestReport`
5. **Review test coverage** and add additional tests as needed

## Notes

- All tests are designed to be idempotent (can run multiple times)
- Tests clean up after themselves (items, players, etc.)
- Test failures provide clear error messages
- Tests do not modify production data
- Test execution should be fast (< 1 minute for all automated tests)
