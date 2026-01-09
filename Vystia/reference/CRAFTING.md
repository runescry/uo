# Vystia Crafting Reference

**Total Systems:** 2 (Engineering, Transmutation)
**Total Recipes:** ~40
**Location:** `ServUO/Scripts/Custom/VystiaClasses/Crafting/`

---

## Crafting Systems Overview

| System | Skill | ID | Class | Tool |
|--------|-------|-----|-------|------|
| Engineering | Engineering | 80 | Artificer | Engineering Tool Kit |
| Transmutation | Transmutation | 81 | Alchemist | Mortar and Pestle |

---

## 1. Engineering (Artificer)

**Skill:** Engineering (ID 80)
**File:** `DefEngineering.cs`
**Tool:** Engineering Tool Kit (craftable)
**Materials:** Clockwork Ingots, Clockwork Gears, Clockwork Springs, Steam Cores

### Basic Components

| Recipe | Skill Range | Materials | Output |
|--------|-------------|-----------|--------|
| Clockwork Gear | 0-50 | 1 Clockwork Ingot | Component |
| Clockwork Spring | 15-65 | 2 Clockwork Ingot | Component |
| Steam Core | 55-105 | 5 Clockwork Ingot, 3 Gears | Power source |

### Gadgets

| Recipe | Skill Range | Materials | Output |
|--------|-------------|-----------|--------|
| Smoke Grenade | 45-95 | 2 Gears, 1 Spring | Escape item |
| Small Explosive | 25-75 | 1 Gear, 1 Bottle | Throwable |
| Medium Explosive | 50-100 | 2 Gears, 1 Spring, 1 Bottle | Throwable |
| Large Explosive | 80-130 | 1 Steam Core, 3 Gears, 1 Bottle | Throwable |

### Tools

| Recipe | Skill Range | Materials | Output |
|--------|-------------|-----------|--------|
| Engineering Tool Kit | 35-85 | 3 Clockwork Ingot, 2 Gears | Crafting station (50 uses) |

### Clockwork Items

| Recipe | Skill Range | Materials | Output |
|--------|-------------|-----------|--------|
| Construct Control Device | 70-120 | 2 Steam Cores, 5 Gears, 3 Springs | Artificer class item |

### Clockwork Equipment

| Recipe | Skill Range | Materials | Output |
|--------|-------------|-----------|--------|
| Clockwork Plate Gorget | 50-100 | 3 Ingot, 1 Gear | Armor |
| Clockwork Plate Helm | 55-105 | 5 Ingot, 1 Gear | Armor |
| Clockwork Plate Gloves | 55-105 | 4 Ingot, 2 Gears | Armor |
| Clockwork Plate Arms | 60-110 | 6 Ingot, 2 Gears | Armor |
| Clockwork Shield | 60-110 | 8 Ingot, 2 Gears | Shield |
| Clockwork Sword | 65-115 | 10 Ingot, 3 Gears | Weapon |
| Clockwork Plate Legs | 65-115 | 10 Ingot, 2 Gears | Armor |
| Clockwork Plate Chest | 75-125 | 15 Ingot, 3 Gears | Armor |

---

## 2. Transmutation (Alchemist)

**Skill:** Transmutation (ID 81)
**File:** `DefTransmutation.cs`
**Tool:** Mortar and Pestle (standard alchemy tool)
**Materials:** Nature Reagents (Verdantpeak), Hex Reagents (Shadowfen)

### Healing Potions (Nature Reagents)

| Recipe | Skill Range | Materials | Effect |
|--------|-------------|-----------|--------|
| Lesser Nature's Healing | 0-50 | 1 Wild Moss, Bottle | Lesser heal |
| Nature's Healing | 15-65 | 2 Wild Moss, 1 Moonpetal, Bottle | Heal |
| Greater Nature's Healing | 55-105 | 2 Druid Bark, 2 Treant Sap, Bottle | Greater heal |

### Cure Potions (Hex Reagents)

| Recipe | Skill Range | Materials | Effect |
|--------|-------------|-----------|--------|
| Lesser Antivenom | 0-40 | 1 Bog Moss, Bottle | Lesser cure |
| Antivenom | 25-75 | 2 Viper Fang, Bottle | Cure |
| Greater Antivenom | 65-115 | 2 Swamp Lotus, 2 Viper Fang, Bottle | Greater cure |

### Refresh Potions (Nature Reagents)

| Recipe | Skill Range | Materials | Effect |
|--------|-------------|-----------|--------|
| Stamina Tonic | 0-25 | 1 Moonpetal, Bottle | Refresh |
| Greater Stamina Tonic | 25-75 | 2 Primal Vine, 2 Moonpetal, Bottle | Total refresh |

### Enhancement Potions (Mixed)

| Recipe | Skill Range | Materials | Effect |
|--------|-------------|-----------|--------|
| Night Vision | 0-25 | 1 Toad's Eye, Bottle | Night sight |
| Agility Elixir | 15-65 | 1 Wild Moss, Bottle | +DEX |
| Strength Elixir | 25-75 | 2 Druid Bark, Bottle | +STR |
| Greater Agility | 35-85 | 2 Elderwood Seed, 2 Wild Moss, Bottle | +DEX (greater) |
| Greater Strength | 45-95 | 2 Living Bark, 2 Treant Sap, Bottle | +STR (greater) |

### Poison Potions (Hex Reagents)

| Recipe | Skill Range | Materials | Effect |
|--------|-------------|-----------|--------|
| Lesser Venom | 0-45 | 1 Bog Moss, Bottle | Lesser poison |
| Venom | 15-65 | 1 Witchweed, 1 Viper Fang, Bottle | Poison |
| Greater Venom | 55-105 | 2 Hag's Hair, 2 Swamp Lotus, Bottle | Greater poison |
| Deadly Venom | 90-140 | 3 Cursed Salt, 2 Cursed Pearl, Bottle | Deadly poison |

### Explosive Potions (Mixed)

| Recipe | Skill Range | Materials | Effect |
|--------|-------------|-----------|--------|
| Lesser Explosive Flask | 5-55 | 2 Wild Moss, 1 Bog Moss, Bottle | Small explosion |
| Explosive Flask | 35-85 | 2 Druid Bark, 2 Witchweed, Bottle | Medium explosion |
| Greater Explosive Flask | 65-115 | 3 Living Bark, 3 Hag's Hair, Bottle | Large explosion |

### Special Items

| Recipe | Skill Range | Materials | Effect |
|--------|-------------|-----------|--------|
| Smoke Bomb | 90-120 | 1 Ancient Root, 1 Cursed Salt | Escape item |

---

## Reagent Sources

### Nature Reagents (Verdantpeak)
- Wild Moss, Moonpetal, Druid Bark, Treant Sap
- Elderwood Seed, Primal Vine, Living Bark, Ancient Root

### Hex Reagents (Shadowfen)
- Bog Moss, Viper Fang, Witchweed, Toad's Eye
- Hag's Hair, Cursed Pearl, Cursed Salt, Swamp Lotus

### Mechanical Components (Ironclad)
- Clockwork Ingot, Clockwork Gear, Clockwork Spring, Steam Core

---

## GM Commands

| Command | Description |
|---------|-------------|
| `[add EngineeringToolKit` | Spawn engineering tool |
| `[add ClockworkIngot 100` | Spawn crafting materials |
| `[add WildMoss 100` | Spawn nature reagents |
| `[add BogMoss 100` | Spawn hex reagents |

---

*Last Updated: 2026-01-01*
