# Reports Service

## Overview
The Reports service generates statistical reports about server activity, player behavior, and system performance.

## Era
- **Expansion:** All
- **Availability:** Administrative feature

## Files
| File | Description |
|------|-------------|
| `Reports.cs` | Core reporting logic |
| `Objects/` | Report data objects |
| `Persistence/` | Report data storage |
| `Rendering/` | Report output formatting |

## Functionality
Generates comprehensive reports for server administrators.

### Report Types
| Type | Content |
|------|---------|
| Player Stats | Online time, activities |
| Economy | Gold circulation, trades |
| Combat | PvP/PvM statistics |
| System | Performance metrics |

### Output Formats
- HTML reports
- Text files
- Console output
- Email (if configured)

## How it Works

### Automatic Reports
1. System collects data continuously
2. Reports generated on schedule
3. Output saved to files
4. Admin reviews reports

### Manual Reports
1. Admin requests report
2. System compiles data
3. Report generated
4. Output displayed/saved

## Configuration
```csharp
// Reports.cs
public static class Reports
{
    public static bool Enabled = true;
    public static TimeSpan ReportInterval = TimeSpan.FromDays(1);
    public static string OutputPath = "Reports/";
}
```

## Report Objects
Data containers for report content:
```csharp
// Objects/
public class PlayerReport
{
    public string Name { get; set; }
    public TimeSpan OnlineTime { get; set; }
    public int Actions { get; set; }
}
```

## Rendering
Output formatters:
- HTML renderer (web viewing)
- Text renderer (logs)
- Console renderer (real-time)

## GM Commands
```
[GenerateReport [type]
[ViewReports
```

## Available Reports

### Player Activity
- Login/logout times
- Active play time
- Actions performed
- Location history

### Economy
- Gold in circulation
- Trade volume
- Vendor sales
- Resource gathering

### Combat
- PvP kills/deaths
- PvE kills
- Champion spawns completed
- Boss fights

### System
- Server uptime
- Memory usage
- Network traffic
- Error counts

## FAQ

**Q: How often are reports generated?**
A: Configurable, default daily.

**Q: Where are reports stored?**
A: In the Reports/ directory.

**Q: Can I customize reports?**
A: Yes, through configuration and code.

**Q: Are reports resource-intensive?**
A: Can be; schedule during low activity.

**Q: Can players see reports?**
A: No, admin-only by default.

## Report Schedule
| Report | Default Schedule |
|--------|-----------------|
| Daily | Every 24 hours |
| Weekly | Every Sunday |
| Monthly | First of month |

## Persistence
Report data saved to:
- Binary files
- XML files
- Database (if configured)

## Related Systems
- Remote Admin (`../RemoteAdmin/`)
- Logging systems
- Analytics
