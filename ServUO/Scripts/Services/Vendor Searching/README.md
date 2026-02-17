# Vendor Searching Service

## Overview
The Vendor Searching system allows players to search for items across all player vendors on the server, making shopping more convenient.

## Era
- **Expansion:** High Seas+ (`Core.HS`)

## Files
| File | Description |
|------|-------------|
| Various vendor search files | Search implementation |

## Functionality
Search across all player-owned vendors for specific items.

### Search Features
- Item name search
- Item type filtering
- Price range filtering
- Property filtering
- Location information

### Results
- Item name and price
- Vendor location
- Navigation assistance
- Comparison shopping

## How it Works for Players

### Accessing Search
1. Use vendor search command/item
2. Search interface opens
3. Enter search criteria
4. View results

### Searching for Items
1. Enter item name or type
2. Optionally set price range
3. Optionally filter properties
4. Execute search

### Viewing Results
1. Results listed by relevance
2. Shows price and vendor
3. Can get directions
4. Navigate to vendor

### Purchasing
1. Travel to vendor location
2. Find the item
3. Purchase directly from vendor
4. Standard vendor purchase

## Configuration
```csharp
// Vendor search configuration
public static bool VendorSearchEnabled = Core.HS;
public static int MaxResults = 100;
public static TimeSpan SearchCooldown = TimeSpan.FromSeconds(30);
```

## GM Commands
```
[VendorSearch          - Open search (if implemented)
```

## Search Criteria
| Criteria | Description |
|----------|-------------|
| Name | Item name text |
| Type | Item category |
| Min Price | Minimum cost |
| Max Price | Maximum cost |
| Properties | Specific attributes |

## Result Information
Each result shows:
- Item name
- Price
- Vendor name
- Vendor location
- Map coordinates

## FAQ

**Q: How do I use vendor search?**
A: Use the search interface or command.

**Q: Is there a search fee?**
A: Server-dependent configuration.

**Q: How accurate is inventory?**
A: Real-time when you search.

**Q: Can I search for specific properties?**
A: Yes, if the search supports it.

**Q: How do I get to the vendor?**
A: Results show location; navigate there.

**Q: Are all vendors included?**
A: Player vendors with public access.

## Search Tips
- Use specific terms for better results
- Price range helps narrow options
- Check multiple vendors for best price
- Recently listed may not appear instantly

## Related Systems
- Player Vendor System
- Housing System
- Economy
