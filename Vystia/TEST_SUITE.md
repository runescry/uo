# Vystia Automated Test Suite

This document summarizes the automated tests that ship with the Vystia shard, explains how they are grouped into a single suite, and lists the commands you can use to run or extend them.

## Test Suite Structure

- **Main suite:** `ServUO/Scripts/Custom/VystiaClasses/Testing/VystiaSystemsTestSuite.cs` defines 54 automated checks grouped by domain (Faction, Religion, Crafting, Reward, Time) and exposes helpers such as `RunAllTests()`, `RunFactionTests()`, etc. Every test reports its result via `TestResult.Pass/Fail` into the shared logging system.
- **Execution command:** `ServUO/Scripts/Custom/VystiaClasses/Testing/TestVystiaSystemsCommand.cs` registers the `[TestVystiaSystems` (alias `[TVS`) command and the `[RunServerTests`/`[RST` entry point. The command initializes logging (`TestLogging`), test scripts, tracker, and runner, then executes the chosen scope and prints per-category summaries.
- **Server automation:** `ServerTestRunner.cs` allows running the same suites from the server console (optionally invoking predefined setups such as `crafting`, `potions`, `constructs`, `faction`, or `devotion`).
- **Utilities:** `TestHelpers.cs`, `TimeTestHelpers.cs`, `VystiaTestScripts.cs`, `TestResultsTracker.cs`, and `TestLogging.cs` provide player/scene setup, time manipulation, manual command scaffolding, result tracking, and log rotation. The logs land under `Logs/VystiaTests/` and are referenced by the console summaries emitted by the commands.

## Running the Suite

1. Start the ServUO server and confirm the console logs include `"[Vystia Tests] Test suite initialized..."`.
2. Log in as a GM and run the GM command:
   ```
   [TestVystiaSystems all
   ```
   or limit to one category:
   ```
   [TestVystiaSystems faction
   [TestVystiaSystems religion
   [TestVystiaSystems crafting
   [TestVystiaSystems reward
   [TestVystiaSystems time
   ```
3. For unattended or scheduled runs, execute the server command:
   ```
   RunServerTests all
   ```
   Add a second argument to trigger a setup routine (e.g., `RunServerTests crafting potions`). The console output mirrors the GM command and writes detailed logs for review.

## Test Categories & Coverage

- **Faction (11 checks):** Enum values, reputation tiers, vendor discounts, caps, enemy relationships, data integrity, token/title systems, tier-gated recipes, exalted perks.
- **Religion (11 checks):** Enum values, piety thresholds, opposed religions, devotion powers, cooldowns, data integrity, shrine/temple references.
- **Crafting (13 checks):** Ore/ingot definitions, smelting ratios, materials, disciplines, recipes, potion/construct/shrine existence.
- **Reward (9 checks):** Reputation/piety from boss kills, donations, PvP, tier gated items, potion/construct/shrine crafting.
- **Time (6 checks):** Pilgrimage/prayer cooldowns, tithe cap, PvP cooldown, fury decay, devotion power cooldown.

Each test is self-contained, centered on verifying data definitions or helper methods, and cleans up any spawned players/items before reporting success.

## Manual Support & Reporting

- Use `VystiaTestScripts.cs` to spawn fixtures before running tests: `[TestCraftingSetup`, `[TestPotionSetup`, `[TestConstructSetup`, `[TestDevotionPowerSetup`, `[TestFactionSetup`, `[TestIntegrationSetup`.
- Record manual observations with `[TestResults <name> <pass|fail> [notes]` and generate overviews with `[TestReport`.
- Logs from automated runs are written to `Logs/VystiaTests/` by `TestLogging.cs`, which also provides `LogTestSuiteStart/End` markers and per-category summaries. Review these files when in-game output is insufficient.

## Next Steps

1. Run the full suite (`[TestVystiaSystems all` or `RunServerTests all`) after any major system change (classes, quests, or economy tweaks).
2. Use the GM setup commands before executing dependent tests so they have known starting states.
3. Track manual outcomes via `[TestResults` to capture edge cases that automated checks currently skip.

