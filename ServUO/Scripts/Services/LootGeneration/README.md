# Loot Generation Service

## Overview
The Loot Generation system handles random item property generation, imbuing, and runic reforging - the systems that create magically enhanced equipment.

## Era
- **Expansion:** Varies by subsystem
  - Random Properties: AOS+
  - Imbuing: SA+ (`Core.SA`)
  - Runic Reforging: TOL+ (`Core.TOL`)

## Files
| File | Description |
|------|-------------|
| `RandomItemGenerator.cs` | Core random property generation |
| `ItemPropertyInfo.cs` | Property definitions and weights |
| `ItemPropertiesGump.cs` | Item property display UI |
| `Imbuing/` | Imbuing system implementation |
| `RunicReforging/` | Runic reforging implementation |

## Functionality
Generates magical properties on items through loot drops, imbuing, and reforging.

### Random Item Generation
When creatures die, loot may include:
- Weapons with random properties
- Armor with random properties
- Jewelry with random properties

### Property Budget System
Items have an "intensity" budget:
- Total property weight limited
- Better properties cost more budget
- Higher tier monsters = higher budgets

## Imbuing (`Imbuing/`)

### What is Imbuing?
Player-controlled property addition:
- Choose specific properties
- Use special ingredients
- Craft customized items
- Soul Forge required

### Imbuing Process
1. Use Soul Forge
2. Select item to imbue
3. Choose property to add
4. Provide required ingredients
5. Success adds property

### Imbuing Ingredients
| Property Type | Ingredient Examples |
|---------------|---------------------|
| Hit Effects | Special gems |
| Resistances | Magical reagents |
| Stat Bonuses | Rare essences |
| Skill Bonuses | Special resources |

### Imbuing Limits
- Maximum properties per item
- Property intensity caps
- Cannot exceed item weight limit

## Runic Reforging (`RunicReforging/`)

### What is Reforging?
Using runic tools to add random powerful properties:
- Consumes runic tool charges
- Adds multiple properties at once
- Options for property types

### Reforging Options
| Option | Effect |
|--------|--------|
| Powerful | Standard random properties |
| Structural | Defense-focused properties |
| Fundamental | Core stat properties |
| Inspired | Specific property themes |

### Runic Tool Tiers
Higher tier tools = better results:
- Dull Copper → Valorite
- Each tier increases potential

## How it Works for Players

### Getting Random Loot
1. Kill monsters (higher tier = better loot)
2. Check corpse for magic items
3. Identify properties

### Using Imbuing
1. Train Imbuing skill
2. Obtain Soul Forge access
3. Gather imbuing ingredients
4. Select property and imbue

### Using Reforging
1. Obtain runic tool (BOD reward)
2. Use tool on item
3. Select reforging options
4. Tool adds random properties

## Configuration
```csharp
// RandomItemGenerator.cs
public static class RandomItemGenerator
{
    public static void GenerateRandomItem(Item item, int intensity)
    {
        // Add random properties based on intensity
    }
}

// ItemPropertyInfo.cs
public class ItemPropertyInfo
{
    public int Weight { get; set; }
    public int MinIntensity { get; set; }
    public int MaxIntensity { get; set; }
}
```

## GM Commands
```
[RandomItem [intensity]  - Generate random item
[Imbue                   - Force imbue
[props                   - View item properties
```

## Property Categories
| Category | Examples |
|----------|----------|
| Damage Increase | 10-25% |
| Hit Chance Increase | 1-15% |
| Defense Chance Increase | 1-15% |
| Resistances | 1-15 per type |
| Hit Effects | Lightning, Fireball, etc. |
| Stat Bonuses | Strength, Dexterity, Intelligence |
| Skill Bonuses | +1 to +15 specific skill |

## Loot Tiers
| Monster Tier | Loot Quality |
|--------------|--------------|
| Weak | Minor properties |
| Medium | Moderate properties |
| Strong | Good properties |
| Boss | Excellent properties |
| Peerless | Best properties + artifacts |

## FAQ

**Q: How do I get better loot?**
A: Kill stronger monsters. Boss monsters drop best random loot.

**Q: Can I customize loot properties?**
A: Yes, use imbuing (SA+) to add specific properties.

**Q: What's the max properties on an item?**
A: Varies by item type, typically 5-8 properties.

**Q: How do I get runic tools?**
A: Bulk Order Deed rewards from crafting vendors.

**Q: Is imbuing better than random loot?**
A: Imbuing gives control; random loot can exceed imbuing limits.

**Q: What skill is needed for imbuing?**
A: Imbuing skill (SA+), trained at Soul Forge.

## Related Systems
- Craft (`../Craft/`) - Item creation
- Bulk Orders (`../BulkOrders/`) - Runic tool acquisition
- Armor Refinement (`../Armor Refinement/`) - Additional enhancement
