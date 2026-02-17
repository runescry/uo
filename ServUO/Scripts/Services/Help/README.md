# Help Service

## Overview
The Help system provides player support features including help menus, stuck character rescue, GM page queuing, and speech logging for customer service.

## Era
- **Expansion:** All
- **Availability:** Always enabled

## Files
| File | Description |
|------|-------------|
| `HelpGump.cs` | Main help menu UI |
| `HelpPersistence.cs` | Help data persistence |
| `PagePrompt.cs` | GM page text input |
| `PagePromptGump.cs` | Page input UI |
| `PageQueue.cs` | Queue management for GM pages |
| `PageQueueGump.cs` | Staff page queue UI |
| `PageResponseGump.cs` | GM response interface |
| `SpeechLog.cs` | Player speech logging |
| `SpeechLogGump.cs` | Speech log viewer |
| `StuckMenu.cs` | Stuck character teleport menu |

## Functionality
Provides multiple avenues for player assistance and staff support tools.

### Help Menu
Main menu accessed via Help button or command:
- General help information
- Stuck character option
- Report harassment
- Contact GM option
- FAQ and resources

### Stuck Menu
Emergency teleport for stuck characters:
- Teleport to nearest inn
- Teleport to starting city
- Cooldown between uses
- Prevents abuse

### GM Pages
Player-to-staff communication:
- Submit issue reports
- Queue system for staff
- Priority handling
- Response tracking

### Speech Logging
Records player conversations for:
- Harassment reports
- Dispute resolution
- Staff investigations

## How it Works for Players

### Accessing Help
1. Press the Help button (client)
2. Or use `[help` command
3. Help gump opens
4. Select appropriate option

### Using Stuck Option
1. Open help menu
2. Select "I'm stuck"
3. Choose destination
4. Teleported after confirmation

### Contacting Staff
1. Open help menu
2. Select "Contact Staff"
3. Enter description of issue
4. Page submitted to queue
5. Wait for staff response

## Configuration
```csharp
// StuckMenu.cs
public static TimeSpan Cooldown = TimeSpan.FromMinutes(10);

// PageQueue.cs
public class PageQueue
{
    public static List<PageEntry> Pages { get; }
    public static void Add(PageEntry entry)
    public static void Remove(PageEntry entry)
}
```

## GM Commands
```
[pages              - View page queue
[pageresponse       - Respond to page
[speechlog [player] - View player speech log
[stuck              - Force stuck teleport
```

## Staff Page Queue
Staff see pages with:
- Player name
- Page type (stuck, harassment, etc.)
- Submission time
- Issue description
- Location information

### Page Handling
1. Staff opens `[pages`
2. Select page to handle
3. View player info and issue
4. Respond or teleport to player
5. Mark page resolved

## Speech Log System
Captures:
- Local speech
- Emotes
- Whispers
- Party chat (configurable)

### Log Retention
- Configurable duration
- Auto-purge old logs
- Staff access only

## FAQ

**Q: How do I get help?**
A: Press Help button or type `[help` in chat.

**Q: I'm stuck in terrain, what do I do?**
A: Use the stuck option in help menu to teleport out.

**Q: How long until staff responds?**
A: Depends on staff availability and queue length.

**Q: Is my speech recorded?**
A: Recent speech may be logged for support purposes.

**Q: Can I cancel a page?**
A: Contact staff to cancel a pending page.

**Q: What qualifies as harassment?**
A: Repeated unwanted contact, threats, or abuse.

## Stuck Destinations
| Destination | Description |
|-------------|-------------|
| Nearest Inn | Closest inn to current location |
| Britain | Britain West Bank |
| Moonglow | Moonglow Bank |
| (Others) | Starting city banks |

## Page Types
| Type | Description |
|------|-------------|
| General | General assistance |
| Stuck | Character stuck in terrain |
| Harassment | Report player harassment |
| Bug | Report game bug |
| Other | Miscellaneous issues |

## Related Systems
- Chat (`../Chat/`) - Communication system
- Remote Admin (`../RemoteAdmin/`) - Server administration
