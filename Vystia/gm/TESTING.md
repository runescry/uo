# GM Testing Guide

How to test Vystia systems in-game.

**See also:** [SYSTEM_TEST_GUIDE.md](SYSTEM_TEST_GUIDE.md) - Comprehensive 19-section testing checklist

---

## Quick Start

1. Login as GM
2. `[va` - Open VystiaAdmin Gump
3. Use gump to assign class, set skills, test abilities

---

## Testing a Class

### Setup
```
[SetClassV2 IceMage       # Assign class
[svs 100                   # Set all Vystia skills to 100
[skillcap 84000            # Ensure skill cap is high enough
[sb ice                    # Give Ice Mage spellbook
[srb                       # Give all reagent bags
```

### Test Magic Class
1. Open spellbook
2. Cast spells from each circle (1-8)
3. Verify:
   - Spell names display correctly
   - Mana consumption
   - Reagent consumption
   - Spell effects
   - Skill gains

### Test Martial Class
```
[SetClassV2 Barbarian
[svs 100
[SetResource Fury 100
[TestDummy                 # Spawn target
[TestAbility 2016          # Test first Barbarian ability
```

---

## Testing Combat Systems

### Secondary Resources
```
[GetResources              # View current resources
[SetResource Fury 50       # Set specific resource
[ResetResources            # Clear all
```

### Buffs
```
[ApplyBuff StrengthBoost 60 10    # +10 STR for 60 seconds
[ListBuffs                         # View active buffs
[ClearBuffs                        # Remove all
```

### Crowd Control
```
[TestDummy                 # Spawn target
[ApplyCC Stun 5            # 5 second stun
[CheckDR Stun              # Check diminishing returns
[ResetDR                   # Reset DR for testing
```

### Stances
```
[ListStances               # See all 28 stances
[SetStance Berserker       # Activate stance
[StanceInfo                # View current effects
[RemoveStance              # Deactivate
```

---

## Testing Spawning

### Creatures
```
[spawnvystia               # Open spawn gump
                           # Page 1: Creatures by region
                           # Click creature name to spawn
[clearvystia 20            # Clear within 20 tiles
```

### Trainers
```
[sat                       # Spawn all 25 trainers in circle
[str IceMage               # Spawn specific trainer
```

### Vendors
```
[spawnvystia               # Open gump, go to Vendors page
[vreag                     # Spawn reagent vendor directly
[vres                      # Spawn resource vendor directly
```

---

## Testing Spellbooks

### Give Spellbook
```
[sb ice        # Ice Mage
[sb druid      # Druid
[sb witch      # Witch
[sb sorcerer   # Sorcerer
[sb warlock    # Warlock
[sb oracle     # Oracle
[sb necro      # Necromancer
[sb summon     # Summoner
[sb shaman     # Shaman
[sb songweaving # Bard (Songweaving)
[sb enchant    # Enchanter
[sb illusion   # Illusionist
```

### Verify
1. Open spellbook - should show custom spell names
2. Spells organized by circles 1-8
3. Double-click spell to cast
4. Check reagent consumption

---

## Testing Equipment

### Via Admin Gump
1. `[va` - Open VystiaAdmin
2. Navigate to Equipment tab
3. Select item to spawn

### Via Spawn Gump
1. `[spawnvystia`
2. Go to Magic Items page
3. Spawn equipment

---

## Testing Quests

### Setup
```
[sat                       # Spawn all trainers
```

### Test Quest Flow
1. Talk to trainer
2. Accept quest
3. Complete objectives
4. Return to trainer
5. Verify rewards

---

## Common Issues

### Spellbook shows wrong spells
- Ensure using modified ClassicUO client
- Check spellbook hue matches school

### Skills not gaining
- Check skill cap: `[skillcap`
- Skills have max of 120.0

### Abilities not working
- Verify class assignment: `[ClassInfoV2`
- Check resource: `[GetResources`
- Verify cooldowns

### Creatures not spawning
- Check map validity
- Use `[spawnvystia` gump
- Verify creature type exists

---

## Test Checklist

### Per Class
- [ ] Class assignment works
- [ ] Starting equipment received
- [ ] Custom skill appears
- [ ] Focus item functional
- [ ] All abilities/spells work
- [ ] Resource generates correctly
- [ ] Stances activate properly

### Per Magic School
- [ ] Spellbook displays correctly
- [ ] All 32 spells castable
- [ ] Reagents consumed
- [ ] Circle progression works
- [ ] Skill gains occur

### Per Region
- [ ] Creatures spawn
- [ ] Regional drops work
- [ ] Boss encounters functional
- [ ] Equipment has correct hues

---

## Economy & World Systems (NEW - 2026-01-02)

### Repair Service
```
[RepairCost                  # Show costs for all damaged items
[SpawnBlacksmith             # Spawn blacksmith at your location
```

**Testing:**
1. Damage some equipment (use combat or `[Durability` command)
2. `[RepairCost` to see repair estimates
3. `[SpawnBlacksmith` and say "repair" or double-click
4. Select items to repair, verify gold deduction
5. Check item durability restored

**Repair Cost Tiers:**
- Standard (iron): 2g per durability
- Regional T1: 35g per durability
- Regional T2: 50g per durability
- Legendary: 75g per durability

### Service NPCs
```
[SpawnHealer                 # Spawn healer NPC
[SpawnMoongateAttendant      # Spawn travel NPC
[ServiceFees                 # Display all fee rates
```

**Testing Resurrection:**
1. `[SpawnHealer` at your location
2. Kill your character (or use `[Kill`)
3. As ghost, say "resurrect" near healer
4. Accept gump, verify gold deducted
5. Cost: 50g base + 10g per fame level

**Testing Travel Fees:**
1. `[SpawnMoongateAttendant` near moongate
2. Say "travel" to see rates
3. Short: 100g, Medium: 175g, Long: 250g

### Training Costs (Implemented)
Training costs are built into VystiaClassTrainers:
- 0-20 skill: 500g
- 20-40 skill: 2,000g
- 40-60 skill: 5,000g
- 60-80 skill: 10,000g
- 80-100 skill: 25,000g

### Religion System (Implemented)
```
[Religion                    # Show your religion info
[SetReligion <name>          # Set religion (GM)
[SetPiety <value>            # Set piety value (GM)
[AddPiety <value>            # Add piety (GM)
[Pray                        # Pray for piety (+5 to +15)
[Tithe <amount>              # Donate gold (+1 per 100g)
```

**Religions:** Solarius, Lunara, Terranis, Ignisara, Aquarion, Voidus

**Piety Thresholds:**
- 50: Minor passive bonus
- 200: Moderate passive bonus
- 500: Major bonus + blessed crafting
- 900: Divine champion status

### Pet System
```
[SummonPet WaterElemental    # Summon pet
[DismissPets                 # Dismiss all pets
[PetInfo                     # Show pet info
[PetList Summoner            # List available pets
```

### Housing System
```
[HouseCosts                  # Display all housing prices
[TaxInfo                     # Show your tax status
[PayTax                      # Pay outstanding taxes
[HouseInfo                   # View targeted house info (GM)
[ForceTaxCollection          # Force tax collection (GM)
```

### Zone Control
```
[ZoneInfo                    # Show current zone rules
[TogglePvP                   # Toggle PvP consent
[SetZone Lawless             # Set zone type (GM)
[ZoneList                    # List all zones (GM)
```

### Faction System
```
[Factions                    # Show all faction standings
[SetReputation Ironclad 5000 # Set faction rep (GM)
[DonateFaction Frosthold 1000 # Donate gold for rep
```

### Economy Test Checklist
- [ ] Pet summoning and limits work
- [ ] Housing costs display correctly
- [ ] Property taxes collect weekly
- [ ] Zone PvP rules enforced
- [ ] Zone death penalties apply
- [ ] Faction reputation tracking works
- [ ] Vendor discounts apply based on rep

---

*Last Updated: 2026-01-02*
