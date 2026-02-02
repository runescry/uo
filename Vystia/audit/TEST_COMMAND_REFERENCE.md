# Vystia Test Suite - Command Reference

## Quick Start Commands

### 1. Start the ServUO Server
```bash
# Navigate to ServUO directory
cd D:\UO\ServUO

# Build and run (or use your preferred method)
# The server should start and load all scripts
```

### 2. Run Tests (Two Methods)

#### Method A: Server-Side (Recommended - No Client Required)
Run tests directly on the server console without needing to log in:

```bash
# In the server console, type:
[RunServerTests all
```

or

```bash
[RST all
```

**Advantages:**
- No client connection required
- All output logged to files automatically
- Can be automated/scripted
- Faster execution

#### Method B: In-Game (Requires Client)
- Connect to the server
- Log in with a GM-level account
- Ensure you have `AccessLevel.GameMaster` or higher

### 3. Run Automated Tests

#### Run All Tests
```
[TestVystiaSystems all
```
or
```
[TVS all
```

#### Run Specific Test Categories
```
[TestVystiaSystems faction
[TestVystiaSystems religion
[TestVystiaSystems crafting
[TestVystiaSystems reward
[TestVystiaSystems time
```

### 4. Server-Side Test Commands (No Client Required)

#### Run Full Test Suite
```bash
# In server console:
[RunServerTests all
```

#### Run Tests with Setup
```bash
# Run tests with automatic setup:
[RunServerTests all all          # Run all tests with all setups
[RunServerTests faction faction  # Run faction tests with faction setup
[RunServerTests crafting crafting # Run crafting tests with crafting setup
```

#### Available Scopes
- `all` - Run all test categories
- `faction` - Faction system tests only
- `religion` - Religion system tests only
- `crafting` - Crafting system tests only
- `reward` - Reward system tests only
- `time` - Time-based system tests only

#### Available Setups
- `all` - Run all test setups
- `crafting` - Setup crafting test environment
- `potions` - Spawn all potions
- `constructs` - Spawn all construct cores
- `devotion` - Setup devotion power testing
- `faction` - Setup faction reputation testing
- `integration` - Setup multi-system integration testing

**Output:**
- All results logged to `Logs/VystiaTests/TestSuite_*.log`
- Console output for immediate feedback
- No client connection required

### 5. Manual Test Setup Commands (In-Game)

#### Setup Crafting Test Environment
```
[TestCraftingSetup runecrafting
[TestCraftingSetup scrying
[TestCraftingSetup leathercraft
[TestCraftingSetup woodshaping
[TestCraftingSetup clothcraft
[TestCraftingSetup necrocraft
[TestCraftingSetup jewelcraft
```

#### Setup Potion Testing
```
[TestPotionSetup
```
Spawns all 14 resource potions in your backpack.

#### Setup Construct Testing
```
[TestConstructSetup
```
Spawns all 5 engineering construct cores in your backpack.

#### Setup Devotion Power Testing
```
[TestDevotionPowerSetup frostfathercult
[TestDevotionPowerSetup emberheartorder
[TestDevotionPowerSetup greenwardcircle
[TestDevotionPowerSetup crystallineascendancy
[TestDevotionPowerSetup voidwalkerpath
[TestDevotionPowerSetup forgepact
```

#### Setup Faction Testing
```
[TestFactionSetup frostguard exalted
[TestFactionSetup flamelegion friendly
[TestFactionSetup greenward honored
[TestFactionSetup arcaneconclave allied
```

**Faction Names:**
- `frostguard`
- `flamelegion`
- `greenward`
- `arcaneconclave`
- `technoguild`
- `sandwalkers`
- `voidborn`

**Tier Names:**
- `friendly` (3,000 rep)
- `allied` (1,501 rep)
- `honored` (4,501 rep)
- `exalted` (9,001 rep)

#### Setup Integration Testing
```
[TestIntegrationSetup
```
Sets up complex scenario with both faction and religion at Exalted tier.

### 5. Record Manual Test Results

#### Record a Test Result
```
[TestResults PotionCreation pass All 14 potions work correctly
[TestResults ConstructAI fail Repair Drone not healing other constructs
[TestResults DevotionPowerCooldown pass All cooldowns working
```

**Format:** `[TestResults <testname> <pass/fail> [notes]`

#### Generate Test Report
```
[TestReport all
[TestReport faction
[TestReport religion
[TestReport crafting
```

### 6. Check Logs

#### View Latest Test Log
```
# Navigate to log directory
cd D:\UO\ServUO\Logs\VystiaTests

# View latest log file (Windows)
type TestSuite_*.log | more

# Or open in text editor
notepad TestSuite_2025-01-15_14-30-25.log
```

#### Search Logs for Failures
```
# Search for failures (PowerShell)
Select-String -Path "Logs\VystiaTests\*.log" -Pattern "FAIL:"

# Search for exceptions
Select-String -Path "Logs\VystiaTests\*.log" -Pattern "EXCEPTION:"
```

## Complete Test Execution Sequence

### Automated Test Run
```
1. Start server
2. Log in as GM
3. [TestVystiaSystems all
4. Review in-game output
5. Check Logs/VystiaTests/ for detailed logs
```

### Manual Test Workflow
```
1. Start server
2. Log in as GM
3. [TestPotionSetup                    # Setup potions
4. Test each potion manually
5. [TestResults PotionCreation pass     # Record result
6. [TestConstructSetup                 # Setup constructs
7. Test each construct manually
8. [TestResults ConstructAI pass       # Record result
9. [TestReport all                     # Generate report
```

## Expected Output

### Successful Test Run
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
...

All 54 tests passed!
```

### Failed Test Run
```
=== Test Results ===
Passed: 52 | Failed: 2 | Total: 54

--- Reward Tests (7/9) ---
[PASS] BossKillReputationReward
[FAIL] BossKillPietyReward: Piety did not increase: 0 -> 0
[FAIL] DonationReputationReward: Reputation did not increase

2 test(s) failed. Review errors above.
Check Logs/VystiaTests/ for detailed logs.
```

## Troubleshooting Commands

### Check if Test Suite is Loaded
```
# Should see initialization message in server console:
[Vystia Tests] Test suite initialized. Use [TestVystiaSystems to run tests.
```

### Verify Test Commands Registered
```
# Try running the command - if not registered, you'll get an error
[TestVystiaSystems
```

### Check Log Directory
```
# Verify log directory exists
dir D:\UO\ServUO\Logs\VystiaTests
```

## Command Aliases

- `[TVS` = `[TestVystiaSystems`
- `[TR` = `[TestResults`
- Full commands also work

## Tips

1. **Run tests after server restart** to ensure all systems are initialized
2. **Check console output** for real-time test progress
3. **Review log files** for detailed failure information
4. **Use manual test commands** to set up specific scenarios
5. **Record manual results** to track in-game testing progress
