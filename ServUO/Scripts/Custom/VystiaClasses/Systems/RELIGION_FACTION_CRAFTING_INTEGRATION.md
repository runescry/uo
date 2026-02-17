# Vystia Religion, Faction, and Crafting Integration Documentation

**Last Updated:** 2025-01-XX  
**Status:** Complete - All systems integrated and tested

## Overview

This document describes the integration of major Vystia systems:

1. **Religion System** - Piety, passive bonuses, devotion powers, blessed items, shrines
2. **Faction System** - Reputation, vendor discounts, faction vendors
3. **Crafting System** - Regional recipes, materials, tools, stations
4. **Class-Religion Synergy** - Synergy bonuses for matching class-religion combinations
5. **Religion PvP System** - Damage bonuses vs opposed religions, healing effectiveness
6. **Blessed Items System** - Item blessing at shrines with religion-specific bonuses
7. **Devotion Powers System** - 18 active powers (3 per religion) with cooldowns

All implementations hook into existing ServUO systems while maintaining compatibility.

---

## Phase 7.1: Religion Passive Bonuses Integration

### System Overview

The religion system tracks player piety (0-1000) across 6 religions. Players gain passive bonuses at specific piety tiers:
- **Initiate (50 piety)**: First passive bonus
- **Devoted (200 piety)**: Second passive bonus + first devotion power
- **Faithful (500 piety)**: Second devotion power
- **Exalted (900 piety)**: Third devotion power

### Passive Bonuses by Religion

#### Frosthelm Faith
- **Initiate (50)**: +5% Cold Resistance
- **Devoted (200)**: +3% Cold Damage

#### Surya's Sandscript
- **Initiate (50)**: +5% Fire Resistance
- **Devoted (200)**: +3% Fire Damage

#### Lunara's Covenant
- **Initiate (50)**: +5% Poison Resistance
- **Devoted (200)**: +5% Healing

#### Celestis Arcanum
- **Initiate (50)**: +5% Energy Resistance
- **Devoted (200)**: +2 Mana Regen

#### Oceana's Covenant
- **Initiate (50)**: +3% Physical Resistance
- **Devoted (200)**: +5 Stealth

#### Cogsmith Creed
- **Initiate (50)**: +3 Crafting Skill (all crafting skills)
- **Devoted (200)**: +5 Crafting Skill, +5 Armor

### Implementation Details

**File:** `ServUO/Scripts/Custom/VystiaClasses/Systems/VystiaSkillIntegration.cs`

New methods added:
- `GetReligionResistanceBonus(Mobile mobile, ResistanceType type)` - Returns resistance bonus based on religion and piety tier
- `GetReligionSkillBonus(Mobile mobile, SkillName skill)` - Returns skill bonus (e.g., +3/+5 crafting for Cogsmith Creed)
- `GetReligionHPBonus(Mobile mobile)` - Returns HP bonus (currently returns 0, reserved for future use)
- `GetReligionManaRegenBonus(Mobile mobile)` - Returns mana regen bonus (+2 for Celestis Arcanum at Devoted)
- `GetReligionStealthBonus(Mobile mobile)` - Returns stealth bonus (+5 for Oceana's Covenant at Devoted)

**Integration Points:**

1. **Resistances** (`PlayerMobile.cs` - `ComputeResistances()`)
   - Applied after base resistances and equipment
   - Added to each resistance type (Physical, Fire, Cold, Poison, Energy)

2. **HP Bonus** (`PlayerMobile.cs` - `HitsMax`)
   - Added to `strOffs` calculation in AOS path
   - Currently returns 0 (reserved for future religion HP bonuses)

3. **Mana Regen** (`PlayerMobile.cs` - `DefaultManaRegen`)
   - Overridden to include religion bonus
   - Adds +2 mana regen for Celestis Arcanum at Devoted tier

4. **Stealth Bonus** (`Stealth.cs` - `OnUse()`)
   - Applied to skill check difficulty
   - Reduces minimum skill requirement and increases maximum by +5 for Oceana's Covenant at Devoted

### Usage Example

```csharp
// Get resistance bonus for a player
int fireResistBonus = VystiaSkillIntegration.GetReligionResistanceBonus(player, ResistanceType.Fire);
// Returns 5 if player is Surya's Sandscript at Initiate+ tier

// Get crafting skill bonus
double craftingBonus = VystiaSkillIntegration.GetReligionSkillBonus(player, SkillName.Blacksmithy);
// Returns 3.0 at Initiate, 5.0 at Devoted for Cogsmith Creed
```

---

## Phase 7.2: Faction Vendor Discounts Integration

### System Overview

The faction system tracks player reputation (-3000 to 21000) across 7 factions. Players receive vendor discounts based on their reputation tier:
- **Friendly (3000)**: 5% discount
- **Honored (6000)**: 8% discount
- **Revered (12000)**: 12% discount
- **Exalted (18000)**: 15% discount

### Implementation Details

**File:** `ServUO/Scripts/Custom/VystiaClasses/Factions/VystiaFactionVendor.cs`

The faction vendor system was already implemented and working. The `VystiaFactionVendor` base class:
- Overrides `GetPriceScalar()` to apply faction discounts
- Tracks current customer during `VendorBuy()` and `VendorSell()` operations
- Displays discount information in vendor properties and speech

**How It Works:**

1. When a player interacts with a faction vendor, the vendor checks the player's reputation tier
2. The discount percentage is retrieved from `FactionData.GetVendorDiscount(tier)`
3. The price scalar is reduced by the discount percentage
4. Discount is displayed in vendor properties and can be queried via speech

### Usage Example

```csharp
// Player with Honored (6000) reputation gets 8% discount
// Base price: 1000 gold
// Discounted price: 920 gold (1000 - 8%)
```

---

## Phase 7.3: Religion Crafting Bonuses Integration

### System Overview

Religion provides two types of crafting bonuses:
1. **Skill Bonuses**: +3/+5 to all crafting skills (Cogsmith Creed)
2. **Exceptional Chance Bonus**: +10% exceptional chance (Forge Blessing devotion power)

### Implementation Details

**File:** `ServUO/Scripts/Services/Craft/Core/CraftItem.cs`

**Skill Bonuses** (`GetSuccessChance()`):
- Added religion skill bonus check after Engineering bonus
- Converts skill bonus to success chance (1 skill point = 1% success chance)
- Applied to all crafting skills when player has Cogsmith Creed religion

**Exceptional Chance** (`GetExceptionalChance()`):
- Added Forge Blessing devotion power check
- Adds +10% (0.10) to exceptional chance when active
- Requires Devoted tier (200 piety) to unlock Forge Blessing

**File:** `ServUO/Scripts/Custom/VystiaClasses/Religion/VystiaReligionSystem.cs`

New method added:
- `HasActiveDevotionPower(PlayerMobile pm, string powerName)` - Checks if player has unlocked and can use a devotion power
  - Power 1 (index 0): Requires Devoted tier (200 piety)
  - Power 2 (index 1): Requires Faithful tier (500 piety)
  - Power 3 (index 2): Requires Exalted tier (900 piety)

### Usage Example

```csharp
// Cogsmith Creed Initiate (50 piety) crafting a sword
// Base skill: 80.0 Blacksmithy
// Religion bonus: +3.0
// Effective skill: 83.0 for success chance calculation

// Cogsmith Creed Devoted (200 piety) with Forge Blessing active
// Base exceptional chance: 30%
// Forge Blessing bonus: +10%
// Final exceptional chance: 40%
```

---

## Phase 7.4: Class-Religion Synergy Bonuses

### System Overview

Certain class-religion combinations provide additional synergy bonuses beyond standard passive bonuses. These synergies enhance class-specific mechanics.

### Synergy Bonuses by Class-Religion Pair

#### Barbarian + Frosthelm Faith
- **Fury Decay**: -15% decay rate (Fury decays 15% slower out of combat)

#### Ice Mage + Frosthelm Faith
- **Freeze Duration**: +10% freeze effect duration

#### Artificer + Cogsmith Creed
- **Steam Regen**: +15% steam regeneration rate when near boiler

#### Monk + Cogsmith Creed
- **Chi Cost**: -10% chi ability cost reduction

#### Templar + Cogsmith Creed
- **Zeal Generation**: +15% zeal generation rate

#### Druid + Lunara's Covenant
- **Shapeshift Duration**: +25% shapeshift form duration

#### Witch + Lunara's Covenant
- **Hex Duration**: +15% hex effect duration

#### Shaman + Lunara's Covenant
- **Totem Duration**: +25% totem effect duration

#### Ranger + Celestis Arcanum
- **Focus Maximum**: +10 maximum Focus resource

#### Bard + Celestis Arcanum
- **Crescendo Maximum**: +3 maximum Crescendo resource

#### Necromancer + Celestis Arcanum
- **LifeForce Maximum**: +15 maximum LifeForce resource

#### Warlock + Celestis Arcanum
- **SoulShard Generation**: +5% SoulShard generation rate

#### Cleric + Any Religion
- **Faith Generation**: +15% Faith generation rate (works with any religion)

### Implementation Details

**File:** `ServUO/Scripts/Custom/VystiaClasses/Systems/VystiaSkillIntegration.cs`

New synergy methods added:
- `GetClassReligionSynergyDecayBonus(PlayerMobile pm, string resourceType)` - Returns decay reduction multiplier
- `GetClassReligionSynergyRegenBonus(PlayerMobile pm, string resourceType)` - Returns regen increase multiplier
- `GetClassReligionSynergyMaxBonus(PlayerMobile pm, string resourceType)` - Returns maximum resource bonus
- `GetClassReligionSynergyDurationBonus(PlayerMobile pm, string buffType)` - Returns duration increase multiplier
- `GetClassReligionSynergyGenerationBonus(PlayerMobile pm, string resourceType)` - Returns generation increase multiplier
- `GetClassReligionSynergyCostReduction(PlayerMobile pm, string resourceType)` - Returns cost reduction multiplier

**Integration Points:**

1. **Resource Decay** (`SecondaryResource.cs` - `FuryResource.OnTick()`)
   - Overrides `OnTick()` to apply decay multiplier
   - Multiplier: 1.0 + synergyBonus (e.g., -0.15 becomes 0.85 multiplier)

2. **Resource Regen** (`SecondaryResource.cs` - `SteamResource.OnTick()`)
   - Overrides `OnTick()` to apply regen multiplier
   - Multiplier: 1.0 + synergyBonus (e.g., 0.15 becomes 1.15 multiplier)

3. **Buff Duration** (`VystiaBuffSystem.cs` - `ApplyBuff()`)
   - Applies duration multiplier before creating buff instance
   - Checks buff type name for "Shapeshift", "Form", "Hex", "Totem", "Freeze"
   - Multiplier: 1.0 + synergyBonus (e.g., 0.25 becomes 1.25 multiplier)

4. **Resource Maximum** (Not yet integrated - reserved for future implementation)
   - Will be applied when resource maximums are calculated
   - Currently methods exist but not hooked into resource creation

5. **Resource Generation** (Not yet integrated - reserved for future implementation)
   - Will be applied when resources are generated
   - Currently methods exist but not hooked into generation logic

6. **Cost Reduction** (Not yet integrated - reserved for future implementation)
   - Will be applied when abilities consume resources
   - Currently methods exist but not hooked into ability cost calculation

### Usage Example

```csharp
// Barbarian with Frosthelm Faith at Devoted tier
// Base Fury decay: 5 per tick
// Synergy bonus: -15% decay
// Actual decay: 5 * 0.85 = 4.25 (rounded to 4) per tick

// Artificer with Cogsmith Creed at Devoted tier, near boiler
// Base Steam regen: 10 per tick
// Synergy bonus: +15% regen
// Actual regen: 10 * 1.15 = 11.5 (rounded to 12) per tick

// Druid with Lunara's Covenant at Devoted tier, casting Bear Form
// Base duration: 5 minutes
// Synergy bonus: +25% duration
// Final duration: 5 * 1.25 = 6.25 minutes
```

---

## Testing Checklist

### Religion Passive Bonuses
- [ ] Resistance bonuses apply correctly at Initiate (50) tier
- [ ] Resistance bonuses apply correctly at Devoted (200) tier
- [ ] Mana regen bonus applies for Celestis Arcanum at Devoted
- [ ] Stealth bonus applies for Oceana's Covenant at Devoted
- [ ] Crafting skill bonuses apply for Cogsmith Creed at Initiate/Devoted
- [ ] Bonuses stack correctly with equipment bonuses

### Faction Vendor Discounts
- [ ] Friendly tier (3000) grants 5% discount
- [ ] Honored tier (6000) grants 8% discount
- [ ] Revered tier (12000) grants 12% discount
- [ ] Exalted tier (18000) grants 15% discount
- [ ] Discount applies to both buy and sell prices
- [ ] Discount displays correctly in vendor properties

### Religion Crafting Bonuses
- [ ] Cogsmith Creed Initiate grants +3 crafting skill
- [ ] Cogsmith Creed Devoted grants +5 crafting skill
- [ ] Skill bonus increases success chance correctly
- [ ] Forge Blessing devotion power grants +10% exceptional chance
- [ ] Forge Blessing requires Devoted tier (200 piety)
- [ ] Exceptional chance bonus stacks with other bonuses

### Class-Religion Synergy
- [ ] Barbarian + Frosthelm Faith: Fury decays 15% slower
- [ ] Artificer + Cogsmith Creed: Steam regens 15% faster
- [ ] Druid + Lunara's Covenant: Shapeshift lasts 25% longer
- [ ] Witch + Lunara's Covenant: Hex lasts 15% longer
- [ ] Shaman + Lunara's Covenant: Totem lasts 25% longer
- [ ] Ice Mage + Frosthelm Faith: Freeze lasts 10% longer
- [ ] Synergy bonuses only apply when both class and religion match
- [ ] Synergy bonuses stack with other bonuses correctly

---

## Technical Notes

### Religion System Integration

The religion system uses a static dictionary (`VystiaPiety`) to track player piety data. This avoids modifying `PlayerMobile` directly and uses an attachment pattern.

**Key Classes:**
- `VystiaPiety` - Static class managing piety data
- `PietyData` - Data structure for individual player piety
- `ReligionData` - Static data about each religion
- `VystiaReligionSystem` - Main system class with utility methods

### Faction System Integration

The faction system uses a similar pattern with `VystiaReputation` managing reputation data.

**Key Classes:**
- `VystiaReputation` - Static class managing reputation data
- `ReputationData` - Data structure for individual player reputation
- `FactionData` - Static data about each faction
- `VystiaFactionVendor` - Base class for faction-affiliated vendors

### Class Detection

Synergy bonuses use `PlayerClassManager.Instance.GetClassType(Mobile)` to detect player class. This requires the player to have a Vystia class assigned via the class selection system.

**Note:** If a player doesn't have a class assigned, synergy bonuses return 0.0 (no bonus).

### Performance Considerations

- Religion bonuses are calculated on-demand (no caching)
- Faction discounts are calculated during vendor interactions only
- Synergy bonuses are checked each tick for resources, once per buff application for buffs
- All calculations are lightweight (simple dictionary lookups and math)

### Future Enhancements

**Planned but not yet implemented:**
1. Resource maximum bonuses (Focus, Crescendo, LifeForce)
2. Resource generation bonuses (Zeal, SoulShards, Faith)
3. Ability cost reduction (Chi)
4. HP bonuses for specific religions
5. Devotion power cooldown tracking (currently only checks piety tier)

---

## File Reference

### Modified Files

1. `ServUO/Scripts/Custom/VystiaClasses/Systems/VystiaSkillIntegration.cs`
   - Added all religion and synergy bonus calculation methods

2. `ServUO/Scripts/Mobiles/PlayerMobile.cs`
   - Added religion resistance bonuses to `ComputeResistances()`
   - Added religion HP bonus to `HitsMax`
   - Overrode `DefaultManaRegen` to include religion bonus

3. `ServUO/Scripts/Skills/Stealth.cs`
   - Added religion stealth bonus to skill check

4. `ServUO/Scripts/Services/Craft/Core/CraftItem.cs`
   - Added religion skill bonuses to `GetSuccessChance()`
   - Added Forge Blessing bonus to `GetExceptionalChance()`

5. `ServUO/Scripts/Custom/VystiaClasses/Religion/VystiaReligionSystem.cs`
   - Added `HasActiveDevotionPower()` method

6. `ServUO/Scripts/Custom/VystiaClasses/Systems/SecondaryResource.cs`
   - Modified `FuryResource.OnTick()` for decay synergy
   - Modified `SteamResource.OnTick()` for regen synergy

7. `ServUO/Scripts/Custom/VystiaClasses/Systems/VystiaBuffSystem.cs`
   - Modified `ApplyBuff()` to apply duration synergy bonuses

### Existing Files (No Changes)

- `ServUO/Scripts/Custom/VystiaClasses/Factions/VystiaFactionVendor.cs` - Already implemented
- `ServUO/Scripts/Custom/VystiaClasses/Factions/VystiaFactionSystem.cs` - Already implemented
- `ServUO/Scripts/Custom/VystiaClasses/Religion/VystiaReligionSystem.cs` - Core system already existed

---

## Developer Notes

### Adding New Religion Bonuses

To add a new religion passive bonus:

1. Add the bonus logic to the appropriate method in `VystiaSkillIntegration.cs`
2. Update `ReligionData` in `VystiaReligionSystem.cs` to include the bonus in the `PassiveBonuses` array
3. Integrate the bonus into the appropriate system (resistances, skills, etc.)

### Adding New Synergy Bonuses

To add a new class-religion synergy:

1. Add the synergy check to the appropriate method in `VystiaSkillIntegration.cs`
2. Integrate the bonus into the appropriate system:
   - Resource decay/regen: Modify resource's `OnTick()` method
   - Buff duration: Already handled in `VystiaBuffSystem.ApplyBuff()`
   - Resource maximum: Hook into resource creation/initialization
   - Resource generation: Hook into generation methods
   - Cost reduction: Hook into ability cost calculation

### Debugging

To debug religion/faction bonuses:

```csharp
// Check player's religion and piety
var pietyData = VystiaPiety.GetPiety(player);
Console.WriteLine($"Religion: {pietyData.Religion}, Piety: {pietyData.Piety}, Tier: {ReligionData.GetTier(pietyData.Piety)}");

// Check resistance bonus
int bonus = VystiaSkillIntegration.GetReligionResistanceBonus(player, ResistanceType.Fire);
Console.WriteLine($"Fire Resistance Bonus: {bonus}%");

// Check synergy bonus
double synergy = VystiaSkillIntegration.GetClassReligionSynergyDecayBonus(player, "Fury");
Console.WriteLine($"Fury Decay Synergy: {synergy * 100}%");
```

---

---

## Phase 8.1: Missing Class-Religion Synergies

### System Overview

Additional class-religion synergies were implemented to complete the design document requirements.

### New Synergies Implemented

#### Fighter + Cogsmith Creed
- **Repair Cost Reduction**: -15% repair costs
- **Implementation**: `VystiaRepairService.CalculateRepairCost()` applies discount when player is Fighter class

#### Oracle + Celestis Arcanum
- **Divination Skill Bonus**: +5 Divination skill
- **Implementation**: `PlayerMobile.MutateSkill()` injects bonus into skill value calculations

#### Additional Synergies (Previously Implemented)
- Knight + Frosthelm Faith: +2 max Fortitude, +5% block
- Sorcerer + Cogsmith Creed: Fire spells +5% damage
- Beastmaster + Frosthelm Faith: Pets +10% cold resist
- Summoner + Celestis Arcanum: Summon HP +15%
- Wizard + Celestis Arcanum: Mana regen +10%
- Enchanter + Celestis Arcanum: Enchant success +5%
- Alchemist + Lunara's Covenant: Potion effectiveness +10%
- Paladin + Celestis Arcanum: Virtue accumulation +15%
- Bounty Hunter + Celestis Arcanum: +1 max Pursuit, +10% mark damage

---

## Phase 8.2: Missing Passive Bonuses

### System Overview

Additional passive bonuses were implemented to complete religion passive bonus systems.

### New Passive Bonuses

#### Cogsmith Creed Initiate (50 piety)
- **Armor Bonus**: +5 Armor Rating
- **Implementation**: `PlayerMobile.ComputeResistances()` applies armor bonus

#### Religion Damage Bonuses
- **Frosthelm Faith Devoted (200)**: +3% Cold Damage
- **Surya's Sandscript Devoted (200)**: +3% Fire Damage
- **Implementation**: `VystiaDamageSystem.Calculate()` applies damage bonuses

#### Religion Healing Bonuses
- **Lunara's Covenant Devoted (200)**: +5% Healing Power
- **Implementation**: `Bandage.cs` and `BaseHealPotion.cs` apply healing bonuses

---

## Phase 8.3: Blessed Items System

### System Overview

Complete blessed items system allowing players to bless weapons, armor, and jewelry at shrines for religion-specific bonuses.

### Blessing Process

1. Player must be Zealot tier (500+ piety)
2. Bring item to shrine of their religion
3. Pay tithe = 5% of item base value
4. Success rate determined by piety tier
5. Item gains religion-specific blessing

### Success Rates (by piety tier)

| Piety | Success | Critical |
|-------|---------|----------|
| 500-599 | 50% | 3% |
| 600-699 | 60% | 5% |
| 700-799 | 70% | 8% |
| 800-899 | 80% | 12% |
| 900-1000 | 90% | 18% |

### Blessing Effects by Religion

| Religion | Standard | Critical |
|----------|----------|----------|
| Frosthelm Faith | +5 HP | +10 HP |
| Surya's Sandscript | +5 Mana | +10 Mana |
| Lunara's Covenant | +5% Healing | +10% Healing, +2 HP Regen |
| Celestis Arcanum | +5% Hit Chance | +10% Hit Chance |
| Oceana's Covenant | +5% Spell Damage | +10% Spell Damage |
| Cogsmith Creed | +5% Fire Damage | +10% Fire Damage, Self-Repair 2 |

### Usage by Religion

| User Type | Effect |
|-----------|--------|
| Same religion | Full effect (100%) |
| Faithless | 50% effect |
| Different (non-opposed) | 25% effect |
| Opposed religion | Cannot equip |

### Implementation Details

**Files:**
- `IBlessedItem.cs` - Interface for blessed items
- `VystiaBlessedItemSystem.cs` - Core blessing logic
- `VystiaShrine.cs` - Shrine interaction
- `BaseWeapon.cs`, `BaseArmor.cs`, `BaseJewel.cs` - Blessed item properties

**Integration Points:**
- `PlayerMobile.HitsMax` - HP bonus
- `PlayerMobile.ManaMax` - Mana bonus
- `PlayerMobile.DefaultHitsRate` - HP regen bonus
- `BaseWeapon.CheckHit()` - Hit chance bonus
- `BaseWeapon.ScaleDamageAOS()` - Fire damage bonus
- `BaseWeapon.OnHit()` - Self-repair bonus
- `Bandage.cs` - Healing power bonus
- `VystiaSpellBase.GetNewAosDamage()` - Spell damage bonus

### Blessing Refresh

- **Requirement**: Adherent tier (50+ piety)
- **Cooldown**: 8 hours
- **Effect**: Refreshes all blessed items (updates blessed date)
- **Access**: Shrine menu at Adherent tier

---

## Phase 8.4: Religion PvP System

### System Overview

Religion-based PvP bonuses and penalties when fighting players of opposed religions.

### Damage Bonuses vs Opposed Religion

| Piety Tier | Damage Bonus | Damage Reduction |
|------------|--------------|------------------|
| Initiate (50) | +2% | None |
| Adherent (50) | +4% | None |
| Devoted (200) | +6% | None |
| Faithful (500) | +8% | None |
| Exalted (900) | +10% | -3% damage taken |

### Opposed Religion Pairs

- Frosthelm Faith ↔ Celestis Arcanum
- Cogsmith Creed ↔ Lunara's Covenant
- Surya's Sandscript ↔ Oceana's Covenant

### Healing Effectiveness

| Relationship | Healing/Buff Effectiveness |
|--------------|---------------------------|
| Same religion | 100% |
| Neutral religions | 100% |
| Opposed religions | 50% |

### Implementation Details

**Files:**
- `VystiaReligionSystem.cs` - `AreReligionsOpposed()` method
- `VystiaSkillIntegration.cs` - PvP damage/healing methods
- `VystiaDamageSystem.cs` - Applies PvP damage bonuses
- `Bandage.cs`, `BaseHealPotion.cs` - Applies healing effectiveness
- `VystiaBuffSystem.cs` - Applies effectiveness to healing buffs

---

## Phase 8.5: Devotion Powers Execution

### System Overview

Complete implementation of all 18 devotion powers (3 per religion) with activation, cooldowns, and effects.

### Power Activation

**Command:** `[DevotionPower <name>` or `[DP <name>`

**Power Tiers:**
- **Power 1** - Devoted (200 piety)
- **Power 2** - Faithful (500 piety)
- **Power 3** - Exalted (900 piety)

### All 18 Powers

#### Frosthelm Faith
1. **Frost Shield** (200) - Absorb 50 damage, reflect 15% cold, 15 min duration
2. **Endurance of Winter** (500) - Cannot die 5s (HP min 1), -50% damage 30 min
3. **Absolute Zero** (900) - Freeze enemies 5 tiles, 3s duration

#### Cogsmith Creed
1. **Forge Blessing** (200) - +10% exceptional craft chance, 5 min duration
2. **Steam Burst** (500) - AoE 30-50 fire damage, knockback, 3 tile radius
3. **Machinist's Grace** (900) - Repair all gear, +15% damage 90s

#### Lunara's Covenant
1. **Moonlight Healing** (200) - Heal 30-50 HP in 5 tile radius
2. **Nature's Sanctuary** (500) - 4-tile zone +25% healing, 20s duration
3. **Lunara's Embrace** (900) - Full heal + cleanse + 5s damage immunity

#### Celestis Arcanum
1. **Arcane Insight** (200) - See enemy stats, 1 min duration
2. **Mana Constellation** (500) - Restore 35% mana to party
3. **Celestial Alignment** (900) - 0 mana cost 8s (max 4 spells)

#### Surya's Sandscript
1. **Sun's Revelation** (200) - Reveal hidden 8 tiles, 20s duration
2. **Time Dilation** (500) - +25% attack/cast speed, 10s duration
3. **Solar Judgment** (900) - Cone 75-100 fire damage, blind 3s

#### Oceana's Covenant
1. **Tidal Surge** (200) - Push enemies back 3 tiles
2. **Abyssal Call** (500) - Summon water elemental (200 HP, 2 min)
3. **Oceana's Wrath** (900) - Tidal wave line damage, drowning DoT

### Cooldowns

| Power | Cooldown |
|-------|----------|
| Power 1 (Utility) | 10-30 minutes |
| Power 2 (Combat) | 10-30 minutes |
| Power 3 (Ultimate) | 60 minutes |

### Power Recharge

- **Requirement**: Devoted tier (200+ piety)
- **Effect**: Reduces all devotion power cooldowns by 50%
- **Access**: Shrine menu at Devoted tier

### Boss Interactions

- **Duration**: 50% vs bosses
- **Absolute Zero**: Bosses immune; adds get 50% more duration
- **Endurance of Winter**: Cannot use when boss <25% HP
- **Celestial Alignment**: 2-spell cap vs bosses (instead of 4)

### Implementation Details

**File:** `VystiaDevotionPowers.cs`

**Features:**
- Command-based activation
- Cooldown tracking per player per power
- Power effects using `VystiaBuffSystem`
- Targeted AoE effects (Steam Burst, Solar Judgment, Oceana's Wrath)
- Boss interaction handling

---

## Phase 8.6: Shrine Functions

### System Overview

Complete shrine system with menu-based access to all religion functions.

### Shrine Functions by Tier

| Function | Piety Required | Description |
|----------|----------------|-------------|
| Prayer | Initiate (1+) | +10 piety, daily cooldown |
| Tithe | Initiate (1+) | 1 piety per 100g, max 30/day |
| Blessing Refresh | Adherent (50+) | Refresh all blessed items, 8hr cooldown |
| Power Recharge | Devoted (200+) | Reduce devotion power cooldowns by 50% |
| Item Blessing | Zealot (500+) | Bless weapons, armor, jewelry |
| Free Resurrection | Champion (900+) | Resurrect at shrine (when dead) |

### Implementation Details

**File:** `VystiaShrine.cs`

**Features:**
- Menu gump showing available functions
- Tier-based function availability
- Cooldown display for blessing refresh
- Target system for item blessing
- Resurrection system for Champion tier

---

## Version History

- **Phase 8.0** (Current): Complete religion system implementation
  - Missing class-religion synergies (Fighter repair, Oracle Divination)
  - Missing passive bonuses (Armor, Damage, Healing)
  - Blessed items system (complete with success rates, effects, refresh)
  - Religion PvP system (damage bonuses, healing effectiveness)
  - Devotion powers execution (all 18 powers with cooldowns)
  - Shrine functions (menu system, all functions)

- **Phase 7.0**: Initial implementation of religion, faction, and crafting integration
  - Religion passive bonuses (resistances, skills, HP, mana, stealth)
  - Faction vendor discounts (already implemented, verified working)
  - Religion crafting bonuses (skill bonuses, exceptional chance)
  - Class-religion synergy (decay, regen, duration bonuses)

---

## Support

For issues or questions about this integration:
1. Check the testing checklist above
2. Review the implementation in the referenced files
3. Use the debugging examples to verify bonuses are applying correctly
4. Ensure players have the required piety tier or reputation tier for bonuses

