# Vystia Manual Test Scripts

Step-by-step guides for manual in-game testing of Vystia systems.

## Overview

This document provides detailed test scripts for manual testing of all Vystia systems. Use the GM commands from `VystiaTestScripts.cs` to set up test scenarios, then follow these scripts to verify functionality.

## Test Scripts

### 1. Crafting Discipline Test Script

**Setup Command:** `[TestCraftingSetup <discipline>`

**Disciplines to Test:**
- Runecrafting (Enchanter)
- Scrying (Oracle)
- Leathercraft (Ranger)
- Woodshaping (Druid)
- Clothcraft (Bard)
- Necrocraft (Necromancer)
- Jewelcraft (Sorcerer)

**Prerequisites:**
- GM access
- Appropriate class character
- Required materials

**Step-by-Step Instructions:**

1. **Setup Test Environment**
   - Use `[TestCraftingSetup <discipline>` command
   - Verify materials are spawned in backpack
   - Verify workstation is accessible

2. **Test Recipe Access**
   - Open crafting menu for the discipline
   - Verify all recipes are visible
   - Check tier-gated recipes (if applicable)

3. **Test Material Consumption**
   - Select a recipe
   - Verify materials are consumed correctly
   - Verify material amounts match recipe requirements

4. **Test Item Creation**
   - Complete crafting attempt
   - Verify item is created
   - Verify item properties (name, hue, stats)

5. **Test Skill Requirements**
   - Test with insufficient skill
   - Verify failure message
   - Test with sufficient skill
   - Verify success

**Expected Results:**
- All recipes accessible
- Materials consumed correctly
- Items created with correct properties
- Skill requirements enforced

**Verification Checklist:**
- [ ] All recipes visible
- [ ] Materials consumed correctly
- [ ] Items created successfully
- [ ] Skill requirements work
- [ ] Quality system works (if applicable)

**Common Issues and Solutions:**
- **Issue:** Materials not spawning
  - **Solution:** Check backpack space, use `[TestCraftingSetup` again
- **Issue:** Recipes not visible
  - **Solution:** Check skill level, verify discipline is initialized

---

### 2. Resource Potion Test Script

**Setup Command:** `[TestPotionSetup`

**Potions to Test:**
1. Fury Draught (+15 max Fury)
2. Berserker's Blood (Fury decay -35%)
3. Chi Elixir (+1 max Chi)
4. Focused Serum (Focus decay -35%)
5. Zealot's Tonic (+1 Zeal per kill)
6. Knight's Fortifier (+2 max Fortitude)
7. Hunter's Mark Oil (+35% Mark duration)
8. Shard Catalyst (+5% Soul Shard gen)
9. LifeForce Flask (Store 35 LifeForce)
10. Chill Enhancer (+5% freeze duration)
11. Crescendo Catalyst (+35% Crescendo gen)
12. Faith Vessel (Store 35 Faith)
13. Steam Concentrate (Portable Steam, 35 charges)
14. Virtue Essence (+1 to one Virtue)

**Prerequisites:**
- GM access
- Character for each class (Barbarian, Monk, Paladin, etc.)

**Step-by-Step Instructions:**

1. **Setup Test Environment**
   - Use `[TestPotionSetup` command
   - Verify all 14 potions spawned in backpack

2. **Test Potion Properties**
   - Check each potion's name
   - Verify hue matches expected color
   - Check stackability

3. **Test Potion Effects**
   - Use each potion
   - Verify effect description matches
   - Verify duration is correct
   - Verify cooldown is enforced

4. **Test Resource Enhancement**
   - Use resource enhancement potions
   - Verify resource maximum increased
   - Verify resource decay reduced (if applicable)

5. **Test Resource Storage**
   - Use storage potions (LifeForce Flask, Faith Vessel)
   - Verify resources can be stored
   - Verify resources can be retrieved

**Expected Results:**
- All potions spawn correctly
- Effects match descriptions
- Durations and cooldowns work
- Resource enhancements work

**Verification Checklist:**
- [ ] All 14 potions spawned
- [ ] Potion properties correct
- [ ] Effects work as described
- [ ] Durations correct
- [ ] Cooldowns enforced
- [ ] Resource enhancements work

**Common Issues and Solutions:**
- **Issue:** Potion effect not applying
  - **Solution:** Check class requirements, verify resource system initialized
- **Issue:** Cooldown not working
  - **Solution:** Check time system, verify cooldown tracking

---

### 3. Engineering Construct Test Script

**Setup Command:** `[TestConstructSetup`

**Constructs to Test:**
1. Clockwork Spider (50 HP, Scout)
2. Repair Drone (Heals constructs)
3. Steam Turret (100 HP, Ranged)
4. Iron Golem (500 HP, Tank)
5. Siege Engine (Territory warfare)

**Prerequisites:**
- GM access
- Artificer class character
- Engineering skill (40+ for basic, 55+ for advanced)

**Step-by-Step Instructions:**

1. **Setup Test Environment**
   - Use `[TestConstructSetup` command
   - Verify all 5 construct cores spawned

2. **Test Construct Summoning**
   - Use each construct core
   - Verify construct is summoned
   - Verify construct properties (HP, control slots)

3. **Test Construct AI**
   - Test Clockwork Spider scouting behavior
   - Test Repair Drone healing behavior
   - Test Steam Turret ranged attacks
   - Test Iron Golem tanking behavior
   - Test Siege Engine territory warfare

4. **Test Construct Durability**
   - Damage constructs
   - Verify HP decreases correctly
   - Test Repair Drone healing other constructs

5. **Test Control Slots**
   - Summon multiple constructs
   - Verify control slot limits
   - Verify excess constructs cannot be summoned

**Expected Results:**
- All constructs summon correctly
- AI behavior works as expected
- Control slots enforced
- Repair Drone heals other constructs

**Verification Checklist:**
- [ ] All 5 constructs spawn
- [ ] Construct properties correct
- [ ] AI behavior works
- [ ] Control slots enforced
- [ ] Repair Drone heals constructs

**Common Issues and Solutions:**
- **Issue:** Construct not summoning
  - **Solution:** Check Engineering skill, verify control slots available
- **Issue:** AI not working
  - **Solution:** Check construct AI initialization, verify target selection

---

### 4. Devotion Power Test Script

**Setup Command:** `[TestDevotionPowerSetup <religion>`

**Religions to Test:**
- Frosthelm Faith
- Surya's Sandscript
- Lunara's Covenant
- Celestis Arcanum
- VoidWalker Path
- Cogsmith Creed

**Prerequisites:**
- GM access
- Character with religion
- Piety at required tier (200, 500, or 900)

**Step-by-Step Instructions:**

1. **Setup Test Environment**
   - Use `[TestDevotionPowerSetup <religion>` command
   - Verify piety set to Exalted tier (900+)

2. **Test Power Access**
   - Use `[DevotionPower` or `[DP` command
   - Verify all 3 powers for religion are visible
   - Check power requirements (piety tier)

3. **Test Power Activation**
   - Select a power
   - Verify power activates
   - Verify power effects work correctly

4. **Test Power Cooldowns**
   - Activate a power
   - Verify cooldown is set
   - Try to activate again immediately
   - Verify cooldown prevents activation

5. **Test Power Effects**
   - Test each power's specific effect
   - Verify duration is correct
   - Verify visual effects appear

**Expected Results:**
- All powers accessible at correct tiers
- Powers activate correctly
- Cooldowns work
- Effects match descriptions

**Verification Checklist:**
- [ ] All 3 powers visible per religion
- [ ] Powers activate correctly
- [ ] Cooldowns enforced
- [ ] Effects work as described
- [ ] Visual effects appear

**Common Issues and Solutions:**
- **Issue:** Power not activating
  - **Solution:** Check piety tier, verify cooldown expired
- **Issue:** Effect not working
  - **Solution:** Check power implementation, verify target selection

---

### 5. Faction System Test Script

**Setup Command:** `[TestFactionSetup <faction> <tier>`

**Factions to Test:**
- Frostguard
- Flame Legion
- Greenward
- Arcane Conclave
- Technoguild
- Sandwalkers
- Voidborn

**Tiers to Test:**
- Friendly (3,000 rep)
- Honored (6,000 rep)
- Revered (12,000 rep)
- Exalted (15,000 rep)

**Prerequisites:**
- GM access
- Character with faction reputation

**Step-by-Step Instructions:**

1. **Setup Test Environment**
   - Use `[TestFactionSetup <faction> <tier>` command
   - Verify reputation set to specified tier

2. **Test Vendor Discounts**
   - Find faction vendor
   - Check vendor discount percentage
   - Verify discount matches tier

3. **Test Tier-Gated Recipes**
   - Check crafting recipes
   - Verify recipes unlock at correct tiers
   - Test recipe access at different tiers

4. **Test Faction Titles**
   - Check title at Exalted tier
   - Verify title displays correctly
   - Test title persistence

5. **Test Exalted Items**
   - Check for Exalted-tier items
   - Verify items are accessible
   - Test item properties

**Expected Results:**
- Vendor discounts match tiers
- Recipes unlock at correct tiers
- Titles awarded at Exalted
- Exalted items accessible

**Verification Checklist:**
- [ ] Vendor discounts correct
- [ ] Tier-gated recipes work
- [ ] Titles awarded correctly
- [ ] Exalted items accessible
- [ ] Reputation thresholds correct

**Common Issues and Solutions:**
- **Issue:** Vendor discount not applying
  - **Solution:** Check reputation tier, verify vendor faction
- **Issue:** Recipes not unlocking
  - **Solution:** Check tier-gated recipe system, verify reputation

---

### 6. Integration Test Script

**Setup Command:** `[TestIntegrationSetup`

**Prerequisites:**
- GM access
- Character with faction and religion

**Step-by-Step Instructions:**

1. **Setup Test Environment**
   - Use `[TestIntegrationSetup` command
   - Verify faction set to Exalted
   - Verify religion set to Exalted

2. **Test Multi-System Interactions**
   - Test faction + religion bonuses
   - Test crafting with faction recipes
   - Test devotion powers with faction bonuses

3. **Test Reward Integration**
   - Kill a boss
   - Verify both reputation and piety rewards
   - Check token rewards

4. **Test System Synergies**
   - Test class + religion + faction combinations
   - Verify bonuses stack correctly
   - Test edge cases

**Expected Results:**
- Systems work together correctly
- Bonuses stack appropriately
- Rewards integrate properly

**Verification Checklist:**
- [ ] Multi-system interactions work
- [ ] Bonuses stack correctly
- [ ] Rewards integrate properly
- [ ] No conflicts between systems

**Common Issues and Solutions:**
- **Issue:** Bonuses not stacking
  - **Solution:** Check bonus calculation, verify system integration
- **Issue:** Rewards not integrating
  - **Solution:** Check reward system initialization, verify event handlers

---

## Test Results Tracking

Use `[TestResults <testname> <pass/fail>` command to track manual test results.

## Notes

- All tests should be performed in a test environment
- Document any issues found during testing
- Update test scripts as systems evolve
- Report bugs to development team

