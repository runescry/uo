# Vystia Systems Testing Guide

**Last Updated:** 2025-01-XX  
**Status:** Comprehensive testing documentation for all Vystia systems

---

## Overview

This guide provides comprehensive testing instructions for all Vystia systems including:
- Religion System (Piety, Devotion Powers, Blessed Items, Shrines)
- Faction System (Reputation, Vendor Discounts)
- Crafting System (Regional Recipes, Materials, Tools)
- Class Systems (25 Classes, Skills, Resources)
- Combat Systems (Damage Scaling, Pet Scaling, Threat)
- Support Systems (Buffs, Enchantments, Repair)

---

## Quick Start: Test Items

### Getting Test Items

Use the command to spawn all test items:
```
[VystiaTestKit
```
or
```
[VTK
```

This creates a backpack containing:
- All religion shrine stones (6)
- All faction stones (7)
- All crafting materials (ingots and ores)
- All crafting tools
- All crafting station stones
- All vendor stones
- Magic reagents
- Basic materials

---

## Part 1: Religion System Testing

### 1.1 Shrine Stones

**Items:** 6 Shrine Stones (one per religion)

**Testing Steps:**
1. Double-click a shrine stone to create a shrine
2. Verify shrine appears at stone location
3. Test with all 6 religions:
   - Frosthelm Faith
   - Surya's Sandscript
   - Lunara's Covenant
   - Celestis Arcanum
   - Oceana's Covenant
   - Cogsmith Creed

**Expected Results:**
- Shrine appears with correct name and hue
- Shrine is immovable
- Stone is consumed

---

### 1.2 Shrine Functions

**Access:** Double-click a shrine

**Available Functions by Piety Tier:**

#### Initiate (1+ piety)
- **Pray** - +10 piety, daily cooldown
- **Tithe** - 1 piety per 100g, max 30 piety/day

#### Adherent (50+ piety)
- **Blessing Refresh** - Refresh all blessed items (8 hour cooldown)

#### Devoted (200+ piety)
- **Power Recharge** - Reduce devotion power cooldowns by 50%

#### Zealot (500+ piety)
- **Item Blessing** - Bless weapons, armor, jewelry

#### Champion (900+ piety)
- **Free Resurrection** - Resurrect at shrine (when dead)

**Testing Steps:**
1. Convert to a religion (use admin commands or piety system)
2. Gain piety through prayer/tithe
3. Test each function at appropriate tier
4. Verify cooldowns work correctly
5. Test blessing items at Zealot tier
6. Test resurrection at Champion tier

**Expected Results:**
- Menu shows only available functions for current piety tier
- Functions execute correctly
- Cooldowns prevent abuse
- Blessed items gain religion-specific bonuses

---

### 1.3 Blessed Items System

**Requirements:** Zealot tier (500+ piety)

**Success Rates (by piety tier):**
- 500-599: 50% success, 3% critical
- 600-699: 60% success, 5% critical
- 700-799: 70% success, 8% critical
- 800-899: 80% success, 12% critical
- 900-1000: 90% success, 18% critical

**Blessing Effects by Religion:**

| Religion | Standard | Critical |
|----------|----------|----------|
| Frosthelm Faith | +5 HP | +10 HP |
| Surya's Sandscript | +5 Mana | +10 Mana |
| Lunara's Covenant | +5% Healing | +10% Healing, +2 HP Regen |
| Celestis Arcanum | +5% Hit Chance | +10% Hit Chance |
| Oceana's Covenant | +5% Spell Damage | +10% Spell Damage |
| Cogsmith Creed | +5% Fire Damage | +10% Fire Damage, Self-Repair 2 |

**Testing Steps:**
1. Reach Zealot tier (500 piety)
2. Use shrine to bless a weapon
3. Verify success rates match design at each piety tier
4. Test critical blessings
5. Equip blessed item and verify bonuses apply
6. Test effectiveness multipliers:
   - Same religion: 100%
   - Faithless: 50%
   - Different (non-opposed): 25%
   - Opposed religion: Cannot equip

**Expected Results:**
- Success rates match design document
- Critical blessings provide enhanced effects
- Bonuses apply correctly when equipped
- Opposed religion items cannot be equipped
- Effectiveness multipliers work correctly

---

### 1.4 Devotion Powers

**Activation:** `[DevotionPower <name>` or `[DP <name>`

**Power Tiers:**
- **Power 1** - Devoted (200 piety)
- **Power 2** - Faithful (500 piety)
- **Power 3** - Exalted (900 piety)

**All 18 Powers:**

#### Frosthelm Faith
1. Frost Shield (200) - Absorb 50 damage, reflect 15% cold
2. Endurance of Winter (500) - Cannot die 5s, -50% damage
3. Absolute Zero (900) - Freeze enemies 5 tiles, 3s

#### Cogsmith Creed
1. Forge Blessing (200) - +10% exceptional craft chance
2. Steam Burst (500) - AoE 30-50 fire damage, knockback
3. Machinist's Grace (900) - Repair all gear, +15% damage

#### Lunara's Covenant
1. Moonlight Healing (200) - Heal 30-50 HP in 5 tiles
2. Nature's Sanctuary (500) - 4-tile zone +25% healing
3. Lunara's Embrace (900) - Full heal + cleanse + immunity

#### Celestis Arcanum
1. Arcane Insight (200) - See enemy stats 1 min
2. Mana Constellation (500) - Restore 35% mana to party
3. Celestial Alignment (900) - 0 mana cost 8s (max 4 spells)

#### Surya's Sandscript
1. Sun's Revelation (200) - Reveal hidden 8 tiles, 20s
2. Time Dilation (500) - +25% attack/cast speed, 10s
3. Solar Judgment (900) - Cone 75-100 fire, blind 3s

#### Oceana's Covenant
1. Tidal Surge (200) - Push enemies back 3 tiles
2. Abyssal Call (500) - Summon water elemental
3. Oceana's Wrath (900) - Tidal wave line damage, drowning DoT

**Testing Steps:**
1. Reach appropriate piety tier
2. Activate each power using command
3. Verify cooldowns work correctly
4. Test power effects
5. Test power recharge at shrine (Devoted tier)
6. Verify boss interactions (50% duration, special rules)

**Expected Results:**
- Powers activate correctly
- Cooldowns prevent spam
- Effects match design document
- Boss interactions apply correctly

---

## Part 2: Faction System Testing

### 2.1 Faction Stones

**Items:** 7 Faction Stones (one per faction)

**Testing Steps:**
1. Double-click a faction stone
2. Verify faction menu appears
3. Test all 7 factions:
   - Frostguard
   - Flame Legion
   - Greenward
   - Arcane Conclave
   - Technoguild
   - Sandwalkers
   - Voidborn

**Expected Results:**
- Menu shows current reputation and tier
- Can set reputation to any tier
- Quick adjustments work (+100, +500, +1000, -100, -500)

---

### 2.2 Faction Reputation Tiers

**Tiers:**
- Hostile (-3000 to -1000)
- Unfriendly (-1000 to 0)
- Neutral (0 to 3000)
- Friendly (3000 to 6000)
- Honored (6000 to 12000)
- Revered (12000 to 15000)
- Exalted (15000+)

**Testing Steps:**
1. Use faction stone to set reputation to each tier
2. Verify tier name updates correctly
3. Test vendor discounts at each tier:
   - Friendly: 5%
   - Honored: 10%
   - Revered: 15%
   - Exalted: 15%

**Expected Results:**
- Reputation tiers display correctly
- Vendor discounts apply at appropriate tiers
- Color coding matches tier (red for hostile, gold for exalted)

---

### 2.3 Faction Vendors

**Items:** 7 Vendor Stones (one per faction)

**Testing Steps:**
1. Double-click vendor stone to spawn vendor
2. Verify vendor appears
3. Test vendor discounts:
   - Set reputation to Friendly/Honored/Revered/Exalted
   - Buy items from vendor
   - Verify discount applies
4. Test with opposed factions (should not affect discounts)

**Expected Results:**
- Vendors spawn correctly
- Discounts apply based on reputation tier
- Opposed faction status doesn't affect vendor discounts

---

## Part 3: Crafting System Testing

### 3.1 Crafting Materials

**Ingots (8 types):**
- Frostforged Ingot (Frosthold)
- Emberforged Ingot (Emberlands)
- Crystalline Ingot (Crystal Barrens)
- Clockwork Ingot (Ironclad)
- Voidforged Ingot (ShadowVoid)
- Shadowforged Ingot (Shadowfen)
- Sunforged Ingot (Desert)
- Natureforged Ingot (Verdantpeak)

**Ores (8 types):**
- Frozen Ore → Frostforged Ingot
- Molten Ore → Emberforged Ingot
- Crystal Ore → Crystalline Ingot
- Steamwork Ore → Clockwork Ingot
- Obsidian Ore → Voidforged Ingot
- Bog Iron Ore → Shadowforged Ingot
- Sandstone Ore → Sunforged Ingot
- Living Ore → Natureforged Ingot

**Testing Steps:**
1. Verify all ingots and ores exist in test kit
2. Test smelting ores at forge:
   - Double-click ore
   - Target forge
   - Verify 2 ore = 1 ingot
3. Test crafting with ingots:
   - Use forge and anvil
   - Craft regional weapons/armor
   - Verify recipes appear in crafting menu

**Expected Results:**
- All materials available
- Smelting works correctly
- Recipes appear in crafting menu
- Regional properties apply to crafted items

---

### 3.2 Crafting Stations

**Stations:**
- Forge (0xFB1)
- Anvil (0xFAF)
- Carpentry Bench (0x1E7F)
- Alchemy Table (0x1E9D)
- Tailoring Table (0xF9C)
- Tinkering Table (0x1EB9)

**Testing Steps:**
1. Double-click crafting station stone
2. Verify station appears on ground
3. Test crafting at each station
4. Verify stations are immovable

**Expected Results:**
- Stations spawn correctly
- Can craft at stations
- Stations are immovable

---

### 3.3 Crafting Tools

**Tools:**
- Smith Hammer
- Tinker Tools
- Sewing Kit
- Scissors
- Dovetail Saw
- Mortar & Pestle
- Scribe's Pen
- Skillet

**Testing Steps:**
1. Verify all tools exist in test kit
2. Test crafting with each tool
3. Verify tools are required for appropriate crafts

**Expected Results:**
- All tools available
- Tools work for appropriate crafts
- Missing tools prevent crafting

---

### 3.4 Regional Recipes

**Recipe Categories:**
- Vystia Weapons (regional weapons)
- Vystia Armor (regional armor)
- Vystia Shields (regional shields)

**Testing Steps:**
1. Open crafting menu at forge/anvil
2. Navigate to "Vystia Weapons", "Vystia Armor", "Vystia Shields"
3. Verify all regional recipes appear
4. Craft items from each region
5. Verify regional properties apply:
   - Frosthold: Cold damage
   - Emberlands: Fire damage
   - Crystal Barrens: Energy damage
   - Ironclad: Durability bonus
   - ShadowVoid: Mage armor

**Expected Results:**
- All recipes appear in menu
- Crafting succeeds with correct materials
- Regional properties apply to crafted items

---

## Part 4: Class System Testing

### 4.1 Class Selection

**Access:** `[VystiaClass` or `[VC`

**Testing Steps:**
1. Open class selection gump
2. Verify all 25 classes appear
3. Select a class
4. Verify class applies correctly
5. Test class switching

**Expected Results:**
- All 25 classes available
- Class selection works
- Skills and resources apply correctly

---

### 4.2 Secondary Resources

**Resources (25 types):**
- Fury, Focus, Chi, Holy Power, Zeal, Combo Points
- Life Force, Crescendo, Steam, Faith, Fortitude, Pursuit
- Virtues, Soul Shards, and more

**Testing Steps:**
1. Select a class with a unique resource
2. Verify resource appears in resource bar
3. Test resource generation
4. Test resource consumption
5. Test resource maximums and synergies

**Expected Results:**
- Resources display correctly
- Generation works as designed
- Consumption works for abilities
- Synergies apply correctly

---

### 4.3 Class Skills

**Custom Skills (26 skills):**
- IDs 58-83 in SkillName enum

**Testing Steps:**
1. Select a class
2. Verify primary skills are set correctly
3. Test skill bonuses from religion/faction
4. Test skill-based bonuses in combat/crafting

**Expected Results:**
- Skills set correctly on class selection
- Bonuses apply from religion/faction
- Skill-based bonuses work in gameplay

---

## Part 5: Combat System Testing

### 5.1 Damage Scaling

**Systems:**
- Combat Mastery damage bonus
- Berserking damage bonus
- Marksmanship damage bonus
- Bounty Mark damage bonus
- Magic school damage bonuses
- Religion passive damage bonuses
- Blessed item damage bonuses

**Testing Steps:**
1. Equip appropriate class
2. Test damage with different skills
3. Verify damage bonuses apply
4. Test religion damage bonuses
5. Test blessed item damage bonuses

**Expected Results:**
- All damage bonuses apply correctly
- Bonuses stack appropriately
- Damage scales with skill level

---

### 5.2 Pet Scaling

**Pet Types:**
- Beastmaster pets (BeastBonding)
- Summoner pets (Conjuration)
- Artificer constructs (Engineering)

**Testing Steps:**
1. Tame/summon pets
2. Verify pet damage scaling
3. Test pet resistance bonuses
4. Test pet HP bonuses from synergies

**Expected Results:**
- Pet damage scales with skill
- Resistance bonuses apply
- Synergy bonuses work correctly

---

### 5.3 Threat System

**Testing Steps:**
1. Engage in combat with multiple enemies
2. Generate threat through damage/healing
3. Verify enemies prioritize high-threat targets
4. Test threat reduction abilities

**Expected Results:**
- Threat generates correctly
- Enemies prioritize high-threat targets
- Threat reduction works

---

## Part 6: Support System Testing

### 6.1 Buff System

**Testing Steps:**
1. Apply buffs through abilities/spells
2. Verify buffs display correctly
3. Test buff duration
4. Test buff stacking
5. Test buff removal

**Expected Results:**
- Buffs apply correctly
- Durations work as designed
- Stacking rules apply
- Removal works

---

### 6.2 Enchantment System

**Testing Steps:**
1. Enchant items
2. Verify enchantment success rates
3. Test enchantment bonuses
4. Test religion crafting bonuses

**Expected Results:**
- Enchantments apply correctly
- Success rates match design
- Bonuses work correctly

---

### 6.3 Repair System

**Testing Steps:**
1. Damage equipment
2. Use repair service
3. Test Fighter + Cogsmith Creed repair cost reduction
4. Verify repair costs calculate correctly

**Expected Results:**
- Repair works correctly
- Cost reduction applies for Fighter + Cogsmith Creed
- Costs match design

---

## Part 7: Integration Testing

### 7.1 Class-Religion Synergies

**Key Synergies:**
- Fighter + Cogsmith Creed: Repair costs -15%
- Oracle + Celestis Arcanum: +5 Divination skill
- Knight + Frosthelm Faith: +2 max Fortitude, +5% block
- Sorcerer + Cogsmith Creed: Fire spells +5% damage
- And 20+ more...

**Testing Steps:**
1. Select appropriate class
2. Join matching religion
3. Verify synergy bonuses apply
4. Test all synergies

**Expected Results:**
- All synergies work correctly
- Bonuses apply when conditions met
- Bonuses don't apply when conditions not met

---

### 7.2 Religion PvP System

**Testing Steps:**
1. Set players to opposed religions
2. Test damage bonuses vs opposed religion
3. Test damage reduction (Champion tier)
4. Test healing effectiveness reduction

**Expected Results:**
- Damage bonuses apply vs opposed religion
- Damage reduction works at Champion tier
- Healing effectiveness reduced for opposed religions

---

### 7.3 Faction-Crafting Integration

**Testing Steps:**
1. Gain faction reputation
2. Test faction vendor discounts
3. Test faction-specific recipes (if any)

**Expected Results:**
- Vendor discounts apply
- Faction recipes work (if implemented)

---

## Test Checklist

### Religion System
- [ ] Shrine stones create shrines
- [ ] Shrine menu shows correct functions
- [ ] Prayer and tithe work
- [ ] Blessing refresh works (8hr cooldown)
- [ ] Power recharge works
- [ ] Item blessing works (success rates correct)
- [ ] Blessed item effects apply
- [ ] Devotion powers activate correctly
- [ ] Cooldowns work correctly

### Faction System
- [ ] Faction stones show menu
- [ ] Reputation tiers display correctly
- [ ] Reputation adjustments work
- [ ] Vendor discounts apply
- [ ] Vendor stones spawn vendors

### Crafting System
- [ ] All materials available
- [ ] Ore smelting works
- [ ] Crafting stations spawn
- [ ] Recipes appear in menu
- [ ] Regional properties apply
- [ ] Tools work correctly

### Class System
- [ ] All 25 classes selectable
- [ ] Skills apply correctly
- [ ] Resources work correctly
- [ ] Synergies apply correctly

### Combat System
- [ ] Damage scaling works
- [ ] Pet scaling works
- [ ] Threat system works
- [ ] PvP bonuses work

### Support Systems
- [ ] Buffs work correctly
- [ ] Enchantments work
- [ ] Repair system works
- [ ] All integrations work

---

## Common Issues and Solutions

### Issue: Shrine menu doesn't appear
**Solution:** Verify player has selected a religion and has sufficient piety

### Issue: Blessed item effects don't apply
**Solution:** Verify item is equipped and player's religion matches or is compatible

### Issue: Crafting recipes don't appear
**Solution:** Verify VystiaCraftingRecipes.Initialize() is called at server startup

### Issue: Faction vendor discounts don't apply
**Solution:** Verify reputation tier is Friendly or higher

### Issue: Devotion powers don't activate
**Solution:** Verify piety tier is sufficient and power is not on cooldown

---

## Commands Reference

### Test Item Commands
- `[VystiaTestKit` or `[VTK` - Spawn complete test kit
- `[ShrineStones` or `[SS` - Spawn all shrine stones
- `[FactionStones` or `[FS` - Spawn all faction stones

### System Commands
- `[DevotionPower <name>` or `[DP <name>` - Activate devotion power
- `[VystiaClass` or `[VC` - Open class selection
- `[VystiaAdmin` or `[VA` - Open admin panel

---

*Last Updated: 2025-01-XX*  
*For questions or issues, refer to KNOWN_ISSUES.md or documentation files*

