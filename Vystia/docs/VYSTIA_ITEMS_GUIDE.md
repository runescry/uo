# Vystia Items & Artifacts Guide

**Last Updated:** December 2024

---

## IMPLEMENTATION STATUS

| Category | Designed | Implemented | Status |
|----------|----------|-------------|--------|
| Legendary Artifacts | 4 | 0 | ❌ Not Started |
| Biome Resources | 8+ | 3 | 🟡 ~35% |
| Regional Equipment | 20+ | 1 | 🟡 5% |
| Consumables | 8+ | 0 | ❌ Not Started |
| Quest Items | 4+ | 1 | 🟡 25% |

**Implemented Items:**
- ✅ `FrozenArtifact.cs` - Frosthold crafting material
- ✅ `FrostSeal.cs` - Frozen Halls dungeon key
- ✅ `HeartwoodCoreFragment.cs` - Artifact fragment (5 needed for full artifact)
- ✅ `TheEternalWinter.cs` - Legendary Halberd

**Next Phase:** Phase 2 - Reagents & Special Materials

---

## Overview
This document catalogs all unique items, artifacts, and special equipment mentioned in the Vystia world lore, along with proposed stats, uses, and implementation details for ServUO.

---

## 🏛️ LEGENDARY ARTIFACTS

### Heartwood Core
**Location**: Verdantpeak's Great Sylvan Forest  
**Lore**: The mythical source of the forest's vitality and magic, coveted by the Ironclad Empire  
**Type**: Artifact (Quest Item)  
**Proposed Stats**:
- **Weight**: 5 stones
- **Hue**: 1152 (Forest Green)
- **Special Properties**:
  - Regenerates 1 HP per second when held
  - +20% to all nature-based magic (Druid spells)
  - Can be used to summon a Treant Guardian (1/day)
- **Drop Rate**: 0.1% from Ancient Treants
- **Uses**: 
  - Quest objective for Ironclad vs Sylvan Concord conflict
  - Powers nature-based magical items
  - Required for crafting Living Wood Armor

### Magma Heart
**Location**: Intersection of Deepforge and The Emberlands  
**Lore**: Legendary forge powered by natural lava pool, capable of producing unparalleled metals  
**Type**: Artifact (Crafting Station)  
**Proposed Stats**:
- **Weight**: 50 stones (stationary)
- **Hue**: 1154 (Molten Red)
- **Special Properties**:
  - Allows crafting of Magma Steel weapons/armor
  - +50% success rate for smithing when used
  - Provides infinite fuel for forges
- **Drop Rate**: 0.05% from Magma Elementals
- **Uses**:
  - Central quest objective for Deepforge vs Emberlands conflict
  - Required for crafting legendary weapons
  - Powers industrial machinery

### Luminous Scepter
**Location**: Luminescent Caverns  
**Lore**: Harnesses the sun's power to unprecedented degree, symbol of ultimate authority  
**Type**: Artifact (Weapon)  
**Proposed Stats**:
- **Damage**: 15-25 (Physical + Energy)
- **Speed**: 2.5 seconds
- **Weight**: 8 stones
- **Hue**: 1153 (Bright Gold)
- **Special Properties**:
  - +25% to all magical damage
  - Can cast Greater Heal (1/day)
  - Illuminates area in 20-tile radius
  - +15 Magery, +15 Eval Int
- **Drop Rate**: 0.02% from Light Elementals
- **Uses**:
  - Ultimate weapon for mages
  - Quest reward for Luminescent Caverns exploration
  - Required for Solar Magic research

### Mirror of Truth
**Location**: Shadowfen  
**Lore**: Reveals deepest desires and fears of those who look into it, often at great cost  
**Type**: Artifact (Utility)  
**Proposed Stats**:
- **Weight**: 3 stones
- **Hue**: 1155 (Shadow Purple)
- **Special Properties**:
  - Reveals hidden players/NPCs in 10-tile radius
  - Shows true alignment of target
  - Can detect lies (GM command)
  - -5 HP when used (cost of truth)
- **Drop Rate**: 0.1% from Shadow Wraiths
- **Uses**:
  - Investigation tool for GMs
  - Quest item for truth-seeking missions
  - PvP utility for detecting stealth

---

## 🌿 BIOME-SPECIFIC RESOURCES

### Frosthold Resources

#### Ice Crystals
**Source**: Ice Elementals, Glacier Worms  
**Type**: Reagent  
**Proposed Stats**:
- **Weight**: 0.1 stones each
- **Hue**: 1152 (Ice Blue)
- **Stackable**: Yes (up to 100)
- **Uses**:
  - Crafting Frost Armor (+10 Cold Resistance)
  - Brewing Cold Resistance Potions
  - Enchanting weapons with Ice damage
- **Drop Rate**: 1-3 per Ice Elemental

#### Frozen Artifacts
**Source**: Ancient Ice Dragons, Polar Elementals  
**Type**: Crafting Material  
**Proposed Stats**:
- **Weight**: 2 stones
- **Hue**: 1153 (Deep Ice Blue)
- **Uses**:
  - Required for crafting Ice Dragon Scale Armor
  - Powers Frost Magic items
  - Trade commodity with southern regions
- **Drop Rate**: 0.5% from Ancient Ice Dragons

### Desert Resources

#### Sand Artifacts
**Source**: Desert Mummies, Sand Elementals  
**Type**: Crafting Material  
**Proposed Stats**:
- **Weight**: 1 stone
- **Hue**: 1154 (Desert Sand)
- **Uses**:
  - Crafting Desert Nomad Robes (+10 Fire Resistance)
  - Brewing Heat Resistance Potions
  - Creating Sandstorm scrolls
- **Drop Rate**: 1-2 per Desert Mummy

#### Oasis Gems
**Source**: Oasis Guardians, Desert Phoenix  
**Type**: Gem  
**Proposed Stats**:
- **Weight**: 0.5 stones
- **Hue**: 1155 (Emerald Green)
- **Value**: 500 gold each
- **Uses**:
  - High-value trade item
  - Required for Water Magic items
  - Can be cut into jewelry
- **Drop Rate**: 0.2% from Desert Phoenix

### Forest Resources

#### Living Wood
**Source**: Forest Treants, Ancient Trees  
**Type**: Crafting Material  
**Proposed Stats**:
- **Weight**: 3 stones
- **Hue**: 1152 (Forest Green)
- **Uses**:
  - Crafting Living Wood Armor (regenerates HP)
  - Required for Nature Magic staves
  - Powers Druid spell components
- **Drop Rate**: 1 per Forest Treant

#### Nature Relics
**Source**: Dryads, Forest Spirits  
**Type**: Artifact Component  
**Proposed Stats**:
- **Weight**: 1 stone
- **Hue**: 1153 (Nature Green)
- **Uses**:
  - Required for Heartwood Core quest
  - Powers Nature Magic items
  - Can summon forest creatures
- **Drop Rate**: 0.3% from Dryads

### Underground Resources

#### Crystal Formations
**Source**: Crystal Drakes, Gemstone Golems  
**Type**: Crafting Material  
**Proposed Stats**:
- **Weight**: 2 stones
- **Hue**: 1154 (Crystal Blue)
- **Uses**:
  - Crafting Crystal Armor (+15 Energy Resistance)
  - Required for Light Magic items
  - Powers magical constructs
- **Drop Rate**: 1-2 per Crystal Drake

#### Deep Gems
**Source**: Underground creatures, Cave systems  
**Type**: Gem  
**Proposed Stats**:
- **Weight**: 0.3 stones
- **Hue**: 1155 (Deep Purple)
- **Value**: 1000 gold each
- **Uses**:
  - Premium trade commodity
  - Required for high-level enchantments
  - Powers underground machinery
- **Drop Rate**: 0.1% from Gemstone Golems

---

## ⚔️ REGIONAL WEAPONS & ARMOR

### Frosthold Equipment

#### Ice Dragon Scale Armor
**Materials**: Frozen Artifacts + Dragon Scales  
**Type**: Armor Set  
**Proposed Stats**:
- **Physical Resistance**: +20
- **Cold Resistance**: +30
- **Weight**: 15 stones (full set)
- **Hue**: 1152 (Ice Blue)
- **Special Properties**:
  - Immune to cold damage
  - +5 Cold damage to melee attacks
  - Slows attackers on hit
- **Crafting Requirements**: 100 Smithing, Frozen Artifacts

#### Frost Blade
**Materials**: Ice Crystals + Steel  
**Type**: Weapon  
**Proposed Stats**:
- **Damage**: 12-18 (Physical + Cold)
- **Speed**: 2.0 seconds
- **Weight**: 6 stones
- **Hue**: 1153 (Frost White)
- **Special Properties**:
  - +10 Cold damage
  - Chance to freeze target (5%)
  - +5 Swordsmanship
- **Crafting Requirements**: 90 Smithing, Ice Crystals

### Desert Equipment

#### Desert Nomad Robes
**Materials**: Sand Artifacts + Cloth  
**Type**: Armor  
**Proposed Stats**:
- **Physical Resistance**: +5
- **Fire Resistance**: +25
- **Weight**: 3 stones
- **Hue**: 1154 (Desert Sand)
- **Special Properties**:
  - Immune to heat damage
  - +10% movement speed in desert
  - Camouflage in sand (stealth bonus)
- **Crafting Requirements**: 80 Tailoring, Sand Artifacts

#### Sandstorm Staff
**Materials**: Sand Artifacts + Wood  
**Type**: Weapon  
**Proposed Stats**:
- **Damage**: 8-12 (Physical + Energy)
- **Speed**: 3.0 seconds
- **Weight**: 4 stones
- **Hue**: 1155 (Sandstorm Brown)
- **Special Properties**:
  - Can cast Sandstorm (1/day)
  - +15 Magery
  - Blinds targets on hit (10%)
- **Crafting Requirements**: 85 Carpentry, Sand Artifacts

### Forest Equipment

#### Living Wood Armor
**Materials**: Living Wood + Leather  
**Type**: Armor Set  
**Proposed Stats**:
- **Physical Resistance**: +15
- **Poison Resistance**: +20
- **Weight**: 12 stones (full set)
- **Hue**: 1152 (Forest Green)
- **Special Properties**:
  - Regenerates 1 HP per 5 seconds
  - +10% to Nature Magic
  - Camouflage in forests
- **Crafting Requirements**: 90 Tailoring, Living Wood

#### Nature's Wrath Bow
**Materials**: Living Wood + String  
**Type**: Weapon  
**Proposed Stats**:
- **Damage**: 10-16 (Physical + Poison)
- **Speed**: 2.5 seconds
- **Weight**: 5 stones
- **Hue**: 1153 (Nature Green)
- **Special Properties**:
  - +10 Poison damage
  - Chance to root target (8%)
  - +5 Archery
- **Crafting Requirements**: 85 Carpentry, Living Wood

### Underground Equipment

#### Crystal Armor
**Materials**: Crystal Formations + Metal  
**Type**: Armor Set  
**Proposed Stats**:
- **Physical Resistance**: +18
- **Energy Resistance**: +25
- **Weight**: 16 stones (full set)
- **Hue**: 1154 (Crystal Blue)
- **Special Properties**:
  - Reflects 20% of magical damage
  - +10% to Light Magic
  - Glows in darkness
- **Crafting Requirements**: 95 Smithing, Crystal Formations

#### Lightbringer Sword
**Materials**: Crystal Formations + Steel  
**Type**: Weapon  
**Proposed Stats**:
- **Damage**: 14-20 (Physical + Energy)
- **Speed**: 2.0 seconds
- **Weight**: 7 stones
- **Hue**: 1155 (Light Gold)
- **Special Properties**:
  - +15 Energy damage
  - Illuminates area in 15-tile radius
  - +5 Swordsmanship
- **Crafting Requirements**: 90 Smithing, Crystal Formations

---

## 🧪 CONSUMABLES & POTIONS

### Regional Potions

#### Cold Resistance Potion
**Materials**: Ice Crystals + Reagents  
**Type**: Consumable  
**Proposed Stats**:
- **Duration**: 10 minutes
- **Effect**: +25 Cold Resistance
- **Weight**: 0.5 stones
- **Hue**: 1152 (Ice Blue)
- **Crafting Requirements**: 70 Alchemy, Ice Crystals

#### Heat Resistance Potion
**Materials**: Sand Artifacts + Reagents  
**Type**: Consumable  
**Proposed Stats**:
- **Duration**: 10 minutes
- **Effect**: +25 Fire Resistance
- **Weight**: 0.5 stones
- **Hue**: 1154 (Desert Red)
- **Crafting Requirements**: 70 Alchemy, Sand Artifacts

#### Nature's Blessing Potion
**Materials**: Living Wood + Reagents  
**Type**: Consumable  
**Proposed Stats**:
- **Duration**: 15 minutes
- **Effect**: +20% to Nature Magic, +5 HP regeneration
- **Weight**: 0.5 stones
- **Hue**: 1153 (Forest Green)
- **Crafting Requirements**: 75 Alchemy, Living Wood

#### Crystal Clarity Potion
**Materials**: Crystal Formations + Reagents  
**Type**: Consumable  
**Proposed Stats**:
- **Duration**: 20 minutes
- **Effect**: +15 Magery, +10 Eval Int, See Hidden
- **Weight**: 0.5 stones
- **Hue**: 1154 (Crystal Blue)
- **Crafting Requirements**: 80 Alchemy, Crystal Formations

---

## 🎒 QUEST ITEMS

### Artifact Fragments
**Source**: Various creatures across regions  
**Type**: Quest Item  
**Proposed Stats**:
- **Weight**: 1 stone each
- **Hue**: 1155 (Mystical Purple)
- **Uses**:
  - Required for artifact reconstruction quests
  - Can be traded for regional rewards
  - Powers magical research
- **Drop Rate**: 0.1% from all creatures

### Regional Tokens
**Source**: Completing regional quests  
**Type**: Quest Reward  
**Proposed Stats**:
- **Weight**: 0.5 stones each
- **Hue**: Regional colors
- **Uses**:
  - Required for faction reputation
  - Can be exchanged for regional equipment
  - Shows completion of regional content
- **Acquisition**: Quest completion only

---

## 📊 IMPLEMENTATION PRIORITY

### Phase 1 (High Priority)
1. **Legendary Artifacts** - Core quest items
2. **Basic Regional Resources** - Ice Crystals, Sand Artifacts, Living Wood
3. **Regional Potions** - Resistance and enhancement potions

### Phase 2 (Medium Priority)
1. **Regional Equipment** - Weapons and armor sets
2. **Advanced Resources** - Frozen Artifacts, Oasis Gems, Crystal Formations
3. **Quest Items** - Artifact Fragments, Regional Tokens

### Phase 3 (Low Priority)
1. **Advanced Equipment** - Legendary weapons and armor
2. **Special Consumables** - High-level potions and scrolls
3. **Decorative Items** - Regional decorations and furniture

---

## 🔧 TECHNICAL NOTES

### Drop Rate Guidelines
- **Common Resources**: 1-3 per creature
- **Uncommon Resources**: 0.1-0.5% drop rate
- **Rare Artifacts**: 0.01-0.1% drop rate
- **Legendary Items**: 0.001-0.01% drop rate

### Crafting Requirements
- **Basic Items**: 70-80 skill requirement
- **Advanced Items**: 85-95 skill requirement
- **Legendary Items**: 100 skill + special materials

### Balance Considerations
- All items should be balanced against existing UO:R equipment
- Regional items should have clear advantages in their home regions
- Artifacts should be powerful but rare
- Resources should encourage exploration and trade

---

## 📝 NOTES FOR DEVELOPERS

1. **Lore Integration**: All items should tie into existing Vystia lore
2. **Regional Identity**: Each region should have unique, thematic items
3. **Quest Integration**: Items should support the political conflicts described in lore
4. **Economic Balance**: Items should create meaningful trade opportunities
5. **Player Progression**: Items should provide clear upgrade paths

This comprehensive item system will transform Vystia from a basic loot world into a rich, immersive experience with unique regional identity and meaningful player progression.


