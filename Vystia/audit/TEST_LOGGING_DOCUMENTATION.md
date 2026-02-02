# Vystia Test Suite Logging Documentation

## Overview

The Vystia Test Suite includes comprehensive server-side logging to capture test execution, failures, and exceptions for debugging and analysis.

## Logging Features

### 1. Test Execution Logging

**Location:** `Logs/VystiaTests/TestSuite_YYYY-MM-DD_HH-mm-ss.log`

**What's Logged:**
- Test suite execution start/end with timestamps
- Test scope and total test count
- Individual test results (pass/fail)
- Test execution duration
- Category summaries

**Example Log Entry:**
```
[2025-01-15 14:30:25.123] ========================================
[2025-01-15 14:30:25.123] TEST SUITE EXECUTION STARTED
[2025-01-15 14:30:25.123] Scope: all
[2025-01-15 14:30:25.123] Total Tests: 54
[2025-01-15 14:30:25.123] Time: 2025-01-15 14:30:25
[2025-01-15 14:30:25.123] ========================================
[2025-01-15 14:30:25.234] PASS: FactionEnumValues (Faction)
[2025-01-15 14:30:25.345] FAIL: BossKillReputationReward (Reward) - Reputation did not increase: 0 -> 0
```

### 2. Exception Logging

**What's Logged:**
- Exception message
- Full stack trace
- Inner exceptions (if any)
- Context where exception occurred

**Example Log Entry:**
```
[2025-01-15 14:30:26.456] EXCEPTION in TestBossKillReputationReward: Object reference not set to an instance of an object.
[2025-01-15 14:30:26.456] Stack Trace: at Server.Custom.VystiaClasses.Testing.TestHelpers.SimulateBossDeath...
[2025-01-15 14:30:26.456] Inner Exception: NullReferenceException
```

### 3. Console Output

All log entries are also written to the server console with the prefix `[Vystia Tests]` for real-time monitoring.

### 4. Test Result Tracking

Each test result (pass/fail) is automatically logged when created via `TestResult.Pass()` or `TestResult.Fail()`.

## Log File Location

**Directory:** `Logs/VystiaTests/`

**File Format:** `TestSuite_YYYY-MM-DD_HH-mm-ss.log`

**Example:** `TestSuite_2025-01-15_14-30-25.log`

## Logging Methods

### TestLogging Class

Located in: `ServUO/Scripts/Custom/VystiaClasses/Testing/TestLogging.cs`

**Key Methods:**
- `Initialize()` - Initialize logging system
- `WriteLine(string)` - Write a log entry
- `LogTestStart()` - Log test execution start
- `LogTestResult()` - Log test result (pass/fail)
- `LogTestSuiteStart()` - Log test suite execution start
- `LogTestSuiteEnd()` - Log test suite execution completion
- `LogException()` - Log exception with stack trace
- `LogCategorySummary()` - Log category test summary

## Automatic Logging

The following are automatically logged:

1. **Test Suite Execution**
   - Start time, scope, total tests
   - End time, results summary, duration

2. **Individual Tests**
   - All test results via `TestResult.Pass()` and `TestResult.Fail()`
   - Test name, category, pass/fail status
   - Error messages for failures

3. **Exceptions**
   - All exceptions caught in test methods
   - Exceptions in helper methods (SimulateBossDeath, etc.)
   - Full stack traces

4. **Category Summaries**
   - Pass/fail counts per category
   - Total tests per category

## Manual Logging

You can add custom logging in test methods:

```csharp
TestLogging.WriteLine("Custom log message: {0}", someValue);
TestLogging.LogException("MyTestMethod", ex);
```

## Log Analysis

### Finding Failed Tests

Search log files for `FAIL:` entries:
```
[2025-01-15 14:30:25.345] FAIL: BossKillReputationReward (Reward) - Reputation did not increase
```

### Finding Exceptions

Search log files for `EXCEPTION:` entries:
```
[2025-01-15 14:30:26.456] EXCEPTION in TestBossKillReputationReward: ...
```

### Performance Analysis

Check test suite execution duration:
```
[2025-01-15 14:30:30.789] Duration: 5.66 seconds
```

## Log Retention

Log files are created per test suite execution. Consider:
- Archiving old log files periodically
- Implementing log rotation if disk space is a concern
- Reviewing logs after each test run

## Integration with Test Results Tracker

The logging system works alongside `TestResultsTracker`:
- **TestLogging**: Server-side execution logs (technical details)
- **TestResultsTracker**: Manual test result tracking (user-reported results)

Both systems complement each other for comprehensive test coverage tracking.

## Troubleshooting

### Logs Not Being Created

1. Check that `TestLogging.Initialize()` is called in `TestVystiaSystemsCommand.Initialize()`
2. Verify `Logs/VystiaTests/` directory exists and is writable
3. Check server console for initialization errors

### Missing Log Entries

1. Ensure tests are using `TestResult.Pass()` and `TestResult.Fail()` (not direct constructor)
2. Check that exceptions are being caught and logged
3. Verify log file is not locked by another process

### Log File Size

Log files grow with each test execution. Consider:
- Implementing log rotation
- Archiving old logs
- Cleaning up test logs periodically

## Best Practices

1. **Review logs after test runs** - Check for patterns in failures
2. **Archive important logs** - Keep logs from significant test runs
3. **Monitor console output** - Real-time feedback during test execution
4. **Use log analysis tools** - Parse logs for trends and patterns
5. **Include context in custom logs** - Add relevant information when logging manually

## Example Log Analysis Workflow

1. Run test suite: `[TestVystiaSystems all`
2. Check console for immediate feedback
3. Review log file: `Logs/VystiaTests/TestSuite_*.log`
4. Search for `FAIL:` entries to identify failures
5. Search for `EXCEPTION:` entries to find errors
6. Analyze patterns and fix issues
7. Re-run tests and compare logs
