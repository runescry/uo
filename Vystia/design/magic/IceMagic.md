# Ice Magic - Frosthold School

| Property | Value |
|----------|-------|
| **Class** | Ice Mage |
| **Region** | Frosthold |
| **Theme** | Cold damage, slows, freezes, area denial |
| **Spellbook** | Ice Mage Spellbook |
| **Hue** | 0x481 (Ice Blue) |
| **Spell IDs** | 1000-1031 |
| **Status** | 100% Complete (32/32 spells) |

---

## Reagents

All Ice Magic spells use Vystia reagents found in Frosthold tundra.

| Reagent | Item ID | Circles | Description |
|---------|---------|---------|-------------|
| Frostbloom | 0x0F86 | 1-3 | Magical frozen flower |
| Glacier Crystal | 0x0F8E | 2-4 | Crystal formed from glacial ice |
| Winterleaf | 0x1A9C | 3-5 | Leaf from an eternal winter tree |
| Permafrost Essence | 0x0F0E | 4-6 | Essence of eternal frost |
| Arctic Pearl | 0x0F7A | 5-7 | Pearl from frozen seas |
| Frozen Soul | 0x0F7D | 6-8 | Captured soul of ice elemental |
| Frost Essence | 0x1C18 | 1-8 | Pure essence of frost magic |
| Heart of Winter | 0x0F7B | 8 | Heart of the winter itself |

---

## Circle 1 - Basic Ice (4 Mana)

### 1. Frost Touch

| Property | Value |
|----------|-------|
| **File** | FrostTouchSpell.cs |
| **Words** | "Kal Frio" |
| **Reagents** | Frostbloom |
| **Range** | 1 tile (melee) |
| **Target** | Single enemy |

**Effect:**
- 2 second paralyze (target cannot move or act)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Caster | 0x373A | 0x481 | Blue frost on hands |
| Target | 0x376A | 0x481 | Paralyze field effect |
| Sound | 0x204 | - | Paralyze sound |

---

### 2. Ice Shard

| Property | Value |
|----------|-------|
| **File** | IceShardSpell.cs |
| **Words** | "Glacius Sagitta" |
| **Reagents** | Frostbloom |
| **Range** | 10 tiles |
| **Target** | Single enemy |
| **Damage** | 5-10 cold |

**Effect:**
- Basic ranged cold damage
- Supports spell reflection

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Projectile | 0x36E4 | 0x481 | Ice projectile |
| Sound | 0x64F | - | Cracking ice |

---

### 3. Frost Ward

| Property | Value |
|----------|-------|
| **File** | FrostWardSpell.cs |
| **Words** | "Sanct Frio" |
| **Reagents** | Winterleaf, Frostbloom |
| **Range** | 12 tiles |
| **Target** | Self or ally |
| **Duration** | 30 seconds |

**Effect:**
- +5 Cold Resistance

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x375A | 0x481 | Blue shimmer |
| Sound | 0x1E9 | - | Wind chime |

---

### 4. Avalanche

| Property | Value |
|----------|-------|
| **File** | AvalancheSpell.cs |
| **Words** | "Kal Vas Frio Multi" |
| **Reagents** | Frostbloom |
| **Range** | 14 tiles |
| **Area** | 8 tile cone (45 degree angle) |
| **Damage** | 30-50 cold |

**Effect:**
- Cone AoE damage
- Knockback 3 tiles (away from caster)
- -20 DEX slow for 5 seconds

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x36E4 | 0x481 | Ice particles |
| Target | 0x3709 | 0x481 | Snow cascade |
| Sound | 0x664 | - | Avalanche rumble |

---

## Circle 2 - Freezing Arts (6 Mana)

### 5. Freezing Grasp

| Property | Value |
|----------|-------|
| **File** | FreezingGraspSpell.cs |
| **Words** | "Kal Frio Mani" |
| **Reagents** | Frostbloom, Winterleaf |
| **Range** | 8 tiles |
| **Target** | Single enemy |
| **Damage** | 8-14 cold |

**Effect:**
- Guaranteed slow: -20 DEX for 5 seconds (100% chance)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Paralyze field recolored |
| Target | 0x374A | 0x481 | Frost at waist |
| Sound | 0x204 | - | Paralyze sound |

---

### 6. Frost Slick

| Property | Value |
|----------|-------|
| **File** | FrostSlickSpell.cs |
| **Words** | "Frio Terra" |
| **Reagents** | Frostbloom, Winterleaf |
| **Range** | 12 tiles (ground) |
| **Area** | 7x7 tiles |
| **Duration** | 15 seconds |

**Effect:**
- Creates visible ice terrain (Static 0x122A tiles)
- Enemies on slick: -20 DEX for 5 seconds
- Enemies on slick: Stamina drained to 0
- Stamina restored when leaving

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Terrain | 0x122A | 0x481 | Snow tiles (49 tiles created) |
| Light | Circle150 | - | Tiles emit light |
| Sound | 0x028 | - | Ice crackle |

---

### 7. Glacial Mend

| Property | Value |
|----------|-------|
| **File** | GlacialMendSpell.cs |
| **Words** | "In Mani Frio" |
| **Reagents** | Winterleaf, Frostbloom |
| **Range** | 8 tiles |
| **Target** | Self or ally |
| **Healing** | 15-25 HP |

**Effect:**
- Heals target
- +5 Cold Resistance for 30 seconds

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Blue healing sparkles |
| Sound | 0x1F2 | - | Healing sound |

---

### 8. Ice Shield

| Property | Value |
|----------|-------|
| **File** | IceShieldSpell.cs |
| **Words** | "Sanct Glacius" |
| **Reagents** | Winterleaf, Glacier Crystal |
| **Range** | 12 tiles |
| **Target** | Self or ally |
| **Duration** | 60 seconds |

**Effect:**
- +10 Physical Resistance
- +15 Cold Resistance
- 15% chance to reflect physical damage as cold damage

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x3779 | 0x481 | Orbiting ice shards |
| Sound | 0x64F | - | Ice crystal |
| Reflect | 0x374A | 0x481 | Frost burst on attacker |

---

## Circle 3 - Offensive Ice (9 Mana)

### 9. Ice Bolt

| Property | Value |
|----------|-------|
| **File** | IceBoltSpell.cs |
| **Words** | "Corp Frio" |
| **Reagents** | Frostbloom, Winterleaf, Glacier Crystal |
| **Range** | 12 tiles |
| **Target** | Single enemy |
| **Damage** | 19-34 cold (scaled with Magery/EvalInt) |

**Effect:**
- Primary single-target nuke
- 25% chance: -15 DEX slow for 5 seconds
- Supports spell reflection

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Projectile | 0x379F | 0x481 | Large ice bolt |
| Sound | 0x20A | - | Energy bolt sound |

---

### 10. Frozen Ground

| Property | Value |
|----------|-------|
| **File** | FrozenGroundSpell.cs |
| **Words** | "Vas Frio Terra" |
| **Reagents** | Frostbloom, Winterleaf, Glacier Crystal |
| **Range** | 12 tiles (ground) |
| **Area** | 3 tile radius (circular) |
| **Duration** | 15 seconds |

**Effect:**
- DoT: 2-4 cold damage per second (30-60 total)
- -20 DEX slow for 4 seconds (refreshes every 3 ticks)
- Creates visible snow tiles

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Terrain | 0x122A | 0x481 | Snow tiles in circle |
| Particles | 0x374A | 0x481 | Frost on enemies |
| Sound | 0x64F | - | Storm sound (every 5s) |

---

### 11. Ice Spear

| Property | Value |
|----------|-------|
| **File** | IceSpearSpell.cs |
| **Words** | "Vas Corp Frio" |
| **Reagents** | Frostbloom, Winterleaf, Glacier Crystal |
| **Range** | 14 tiles |
| **Target** | Line (up to 3 enemies) |
| **Damage** | 25-40 cold per target |

**Effect:**
- Pierces through up to 3 enemies in a line
- Each enemy takes full damage

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Projectile | 0x36E4 | 0x481 | Large ice spear |
| Impact | 0x36CB | 0x481 | Ice shatter on each hit |

---

### 12. Frostbite

| Property | Value |
|----------|-------|
| **File** | FrostbiteSpell.cs |
| **Words** | "An Frio Mani" |
| **Reagents** | Winterleaf, Glacier Crystal |
| **Range** | 10 tiles |
| **Target** | Single enemy |
| **Duration** | 12 seconds |

**Effect:**
- DoT: 4-7 cold damage every 2 seconds (6 ticks, 24-42 total)
- Note: Healing reduction mentioned in messages but not implemented

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x374A | 0x481 | Frost at waist |
| Sound | 0x1ED | - | Debuff sound |

---

## Circle 4 - Ice Barriers (11 Mana)

### 13. Frost Armor

| Property | Value |
|----------|-------|
| **File** | FrostArmorSpell.cs |
| **Words** | "Sanct Ort Frio" |
| **Reagents** | Winterleaf, Glacier Crystal, Permafrost Essence |
| **Range** | 12 tiles |
| **Target** | Self or ally |
| **Duration** | 120-240 seconds (scales with Magery) |

**Effect:**
- +10 Physical Resistance
- +20 Cold Resistance
- Visually equips ice-hued plate armor (chest, legs, arms, gloves, helm, gorget)
- Original armor re-equipped when spell ends

**Duration Formula:** `120 + (Magery x 1.2)` seconds, capped at 240s

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x375A | 0x481 | Frost aura |
| Equipment | PlateArmor | 0x481 | Ice-blue plate pieces |
| Sound | 0x1E9 | - | Ice formation |

---

### 14. Ice Wall

| Property | Value |
|----------|-------|
| **File** | IceWallSpell.cs |
| **Words** | "Kal Frio Murus" |
| **Reagents** | Glacier Crystal, Permafrost Essence |
| **Range** | 12 tiles (ground) |
| **Area** | 5x5 square (25 wall segments) |
| **Duration** | 30 seconds |

**Effect:**
- Creates physical barrier blocking movement
- Each segment: 100 HP, can be destroyed
- Orientation varies by position (horizontal 0x0080 or vertical 0x007E)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Wall | 0x0080/0x007E | 0x481 | Ice wall segments |
| Particles | 0x376A | 0x481 | Sparkles on each segment |
| Sound | 0x1F8 | - | Ice formation |
| Shatter | 0x3735 | 0x481 | Wall break effect |

---

### 15. Icicle Barrage

| Property | Value |
|----------|-------|
| **File** | IcicleBarrageSpell.cs |
| **Words** | "Vas Frio Multi" |
| **Reagents** | Glacier Crystal, Permafrost Essence |
| **Range** | 12 tiles |
| **Area** | 3 tile radius around target |
| **Damage** | 15-25 cold per enemy |

**Effect:**
- AoE burst damage centered on target
- 5 staggered icicles (200ms apart) for visual effect
- All damage instant

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Icicles | 0x36E4 | 0x481 | 5 falling icicles |
| Impact | 0x36CB | 0x481 | Ice impact on enemies |
| Sound | 0x64F | - | Ice storm |

---

### 16. Permafrost

| Property | Value |
|----------|-------|
| **File** | PermafrostSpell.cs |
| **Words** | "An Sanct Frio" |
| **Reagents** | Winterleaf, Glacier Crystal, Permafrost Essence |
| **Range** | 10 tiles |
| **Target** | Single enemy |
| **Duration** | 8 seconds |

**Effect:**
- Root: Target.Frozen = true (cannot move, can still cast/attack)
- DoT: 5-10 cold damage per second (40-80 total)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Ice at feet |
| Sound | 0x1F8 | - | Freezing sound |

---

## Circle 5 - Greater Ice (14 Mana)

### 17. Glacial Strike

| Property | Value |
|----------|-------|
| **File** | GlacialStrikeSpell.cs |
| **Words** | "Vas Corp An Frio" |
| **Reagents** | Permafrost Essence, Arctic Pearl |
| **Range** | 14 tiles |
| **Target** | Single enemy |
| **Damage** | 35-55 cold |

**Effect:**
- High damage nuke
- 50% chance: 2 second freeze (Frozen = true, cannot move or act)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Projectile | 0x36E4 | 0x481 | Massive ice bolt |
| Freeze | 0x376A | 0x481 | Complete ice encasement |
| Sound | 0x1FB | - | Heavy impact |

---

### 18. Frozen Tomb

| Property | Value |
|----------|-------|
| **File** | FrozenTombSpell.cs |
| **Words** | "An Ex Frio" |
| **Reagents** | Permafrost Essence, Arctic Pearl |
| **Range** | 10 tiles |
| **Target** | Self, ally, or enemy |
| **Duration** | 6 seconds |

**Effect:**
- Target becomes: Frozen + Paralyzed + Blessed (immune to damage)
- Double-edged: Saves allies from death OR disables enemies temporarily

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x3779 | 0x481 | Ice sphere |
| Sound | 0x1F8 | - | Freezing |
| Shatter | 0x3735 | 0x481 | Tomb break |

---

### 19. Shatter

| Property | Value |
|----------|-------|
| **File** | ShatterSpell.cs |
| **Words** | "Corp An Frio" |
| **Reagents** | Permafrost Essence, Glacier Crystal |
| **Range** | 12 tiles |
| **Target** | Single enemy |
| **Damage** | 20-35 cold (30-52 if target slowed/frozen) |

**Effect:**
- Combo finisher: +50% damage vs slowed or frozen targets
- Checks for any Ice Magic slow debuff or Frozen state

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Projectile | 0x36BD | 0x481 | Shattering ice |
| Bonus | 0x3735 | 0x481 | Extra shatter particles |
| Sound | 0x1FB | - | Impact |

---

### 20. Hypothermia

| Property | Value |
|----------|-------|
| **File** | HypothermiaSpell.cs |
| **Words** | "An Sanct Corp Frio" |
| **Reagents** | Permafrost Essence, Arctic Pearl |
| **Range** | 10 tiles |
| **Target** | Single enemy |
| **Duration** | 20 seconds |

**Effect:**
- Instant damage: 10% of target's max HP
- Debuff: -40 DEX for 20 seconds

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x374A | 0x481 | Blue debuff aura |
| Sound | 0x1ED | - | Debuff sound |

---

## Circle 6 - Master Ice (20 Mana)

### 21. Chill Aura

| Property | Value |
|----------|-------|
| **File** | ChillAuraSpell.cs |
| **Words** | "Kal An Frio" |
| **Reagents** | Winterleaf, Glacier Crystal, Permafrost Essence |
| **Range** | Self (2 tile radius) |
| **Duration** | 20 seconds |

**Effect:**
- Defensive aura around caster
- First enemy contact: 2 second paralyze
- While enemy in range: DEX drained to 0
- DEX restored when enemy leaves

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Caster | 0x3709 | 0x481 | Cold mist at feet |
| Enemy | 0x376A | 0x481 | Freeze effect |
| Sound | 0x64F | - | Cold wind |

---

### 22. Blizzard

| Property | Value |
|----------|-------|
| **File** | BlizzardSpell.cs |
| **Words** | "Tempestas Glacialis" |
| **Reagents** | Permafrost Essence, Arctic Pearl, Frozen Soul |
| **Range** | 12 tiles (ground) |
| **Area** | 5 tile radius |
| **Duration** | 10 seconds |

**Effect:**
- DoT: 3-8 + (Magery/20) cold damage per second
- Total: 30-80+ damage over 10 seconds

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Area | 0x3709 | 1150 | Swirling ice storm |
| Target | 0x374A | 1150 | Frost on enemies |
| Sound | 0x64F | - | Storm (every 3s) |

---

### 23. Deep Freeze

| Property | Value |
|----------|-------|
| **File** | DeepFreezeSpell.cs |
| **Words** | "An Ort Frio" |
| **Reagents** | Permafrost Essence, Arctic Pearl, Frozen Soul |
| **Range** | 12 tiles |
| **Target** | Single enemy |
| **Duration** | 10 seconds |

**Effect:**
- Freeze: Target cannot move or act
- Vulnerability: -25 to ALL resistances (Physical, Fire, Cold, Poison, Energy)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Complete ice encasement |
| Sound | 0x1F8 | - | Heavy freezing |
| Shatter | 0x3735 | 0x481 | Ice break |

---

### 24. Glacial Fortress

| Property | Value |
|----------|-------|
| **File** | GlacialFortressSpell.cs |
| **Words** | "Vas Sanct Frio" |
| **Reagents** | Permafrost Essence, Arctic Pearl, Frozen Soul |
| **Range** | Self only |
| **Duration** | 60 seconds |

**Effect:**
- +25 to ALL resistances:
  - Physical +25
  - Fire +25
  - Cold +25
  - Poison +25
  - Energy +25

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Caster | 0x3779 | 0x481 | Ice dome |
| Sound | 0x1F8 | - | Fortress formation |
| End | 0x3735 | 0x481 | Fortress crumble |

---

## Circle 7 - Ancient Ice (40 Mana)

### 25. Absolute Zero

| Property | Value |
|----------|-------|
| **File** | AbsoluteZeroSpell.cs |
| **Words** | "Kal Vas An Frio" |
| **Reagents** | Arctic Pearl, Frozen Soul, Frost Essence |
| **Range** | 12 tiles (ground) |
| **Area** | 6 tile radius |
| **Damage** | 50-80 cold |

**Effect:**
- Massive AoE instant damage
- All enemies frozen for 3 seconds
- Creates Frozen Ground DoT zone at impact

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Impact | 0x36BD | 0x481 | Massive blue explosion |
| Freeze | 0x376A | 0x481 | All enemies frozen |
| Sound | 0x307 | - | Explosion |

---

### 26. Glacier Summon

| Property | Value |
|----------|-------|
| **File** | GlacierSummonSpell.cs |
| **Words** | "Kal Xen Frio" |
| **Reagents** | Arctic Pearl, Frozen Soul, Permafrost Essence |
| **Range** | 12 tiles (ground) |
| **Duration** | 180 seconds (3 minutes) |

**Effect:**
- Summons Ice Elemental:
  - 700 HP
  - 20-30 damage per hit
  - Immune to cold damage
  - Follows and fights for caster

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Summon | 0x3779 | 0x481 | Ice forms and swirls |
| Sound | 0x1F8 | - | Summoning |

---

### 27. Eternal Winter

| Property | Value |
|----------|-------|
| **File** | EternalWinterSpell.cs |
| **Words** | "Vas Kal An Frio" |
| **Reagents** | Arctic Pearl, Frozen Soul, Frost Essence |
| **Range** | 12 tiles (ground) |
| **Area** | 8 tile radius (17x17 tiles) |
| **Duration** | 30 seconds |

**Effect:**
- DoT: 5-10 cold damage per second (150-300 total)
- -50 DEX slow (refreshes each tick)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Area | 0x3709 | 0x481 | Constant blizzard |
| Sound | 0x64F | - | Storm (every 5s) |

---

### 28. Fimbulwinter's Wrath

| Property | Value |
|----------|-------|
| **File** | FimbulwintersWrathSpell.cs |
| **Words** | "Kal Vas Corp Frio" |
| **Reagents** | Arctic Pearl, Frozen Soul, Frost Essence, Heart of Winter |
| **Range** | Self |
| **Duration** | 15 seconds |

**Effect:**
- Transform into avatar of winter:
  - 100% Cold Resistance (immune)
  - 100% Fire Resistance (immune)
  - Damage aura: 10 cold damage/sec to enemies within 15 tiles

**Note:** +50% cold damage and 50% mana reduction documented in code TODO but not implemented.

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Caster | 0x375A | 0x481 | Ice aura |
| Caster | 0x3779 | 0x481 | Frost particles |
| Aura | 0x374A | 0x481 | Damage on enemies |
| Sound | 0x307 | - | Transformation |

---

## Circle 8 - Legendary Ice (50 Mana)

### 29. Frost Meteor

| Property | Value |
|----------|-------|
| **File** | FrostMeteorSpell.cs |
| **Words** | "Kal Vas An Corp Frio" |
| **Reagents** | Frozen Soul, Frost Essence, Heart of Winter |
| **Range** | 20 tiles (ground) |
| **Area** | 10 tile radius |
| **Damage** | 80-120 cold |

**Effect:**
- 2 second cast delay (telegraphed)
- Massive AoE damage
- -75 DEX slow for 10 seconds
- Creates Frozen Ground DoT zone at impact

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Impact | 0x36D4 | 0x481 | Meteor crash |
| Freeze | 0x376A | 0x481 | Ice on all enemies |
| Sound | 0x664 | - | Earthquake |

---

### 30. Ice Age

| Property | Value |
|----------|-------|
| **File** | IceAgeSpell.cs |
| **Words** | "Kal Vas An Xen Frio" |
| **Reagents** | Frozen Soul, Frost Essence, Heart of Winter, Frozen Ore (5) |
| **Range** | Self (no targeting) |
| **Area** | 30 tile radius (entire screen) |
| **Duration** | 20 seconds |

**Effect:**
- Screen-wide devastation
- DoT: 8-15 cold damage per second (160-300 total)
- -90 DEX slow
- 20% chance per tick to freeze enemy for 2 seconds

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Area | 0x3709 | 0x481 | Ice/snow everywhere |
| Freeze | 0x376A | 0x481 | Random enemy freezes |
| Sound | 0x307 | - | Constant storm |

---

### 31. Rime Reaper

| Property | Value |
|----------|-------|
| **File** | RimeReaperSpell.cs |
| **Words** | "Corp An Frio Mort" |
| **Reagents** | Frozen Soul, Frost Essence, Heart of Winter |
| **Range** | 15 tiles (direction) |
| **Area** | 90 degree arc sweep |
| **Damage** | 100-150 cold |

**Effect:**
- Execute mechanic: Enemies at 20% max HP or below die instantly
- Wide arc damage in front of caster
- Can execute multiple enemies at once

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Sweep | 0x36BD | 0x481 | Giant ice scythe slash |
| Execute | 0x3735 | 0x481 | Target shatters into ice |
| Sound | 0x232 | - | Scythe sound |

---

### 32. Cocytus Prison

| Property | Value |
|----------|-------|
| **File** | CocytusPrisonSpell.cs |
| **Words** | "Kal Xen An Frio" |
| **Reagents** | Frozen Soul, Frost Essence (3), Heart of Winter |
| **Range** | 12 tiles |
| **Target** | Single enemy |
| **Duration** | Up to 60 seconds (channeled) |

**Effect:**
- Target becomes: Frozen + Paralyzed + Blessed (immune to damage)
- Caster must channel: Cannot move or cast during duration
- Ends early if caster releases

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Prison | 0x3779 | 0x481 | Massive ice prison (2x scale) |
| Pulse | 0x374A | 0x481 | Ice particles (every 2s) |
| Sound | 0x1F8 | - | Freezing |
| Shatter | 0x3735 | 0x481 | Massive particle burst |

---

## Testing Commands

```
[spellbook ice              - Spawn Ice Mage Spellbook
[add Frostbloom 100         - Circle 1-3 reagent
[add GlacierCrystal 100     - Circle 2-4 reagent
[add Winterleaf 100         - Circle 3-5 reagent
[add PermafrostEssence 100  - Circle 4-6 reagent
[add ArcticPearl 100        - Circle 5-7 reagent
[add FrozenSoul 100         - Circle 6-8 reagent
[add FrostEssence 100       - Circle 1-8 reagent
[add HeartOfWinter 100      - Circle 8 reagent
[add FrozenOre 100          - Ice Age spell
```

---

## Known Issues

| Issue | Status |
|-------|--------|
| Frostbite healing reduction not implemented | Messages display but no mechanic |
| Fimbulwinter's +50% cold damage not implemented | Code TODO |
| Fimbulwinter's 50% mana reduction not implemented | Code TODO |

---

## File Locations

| Type | Path |
|------|------|
| Spell Files | `ServUO/Scripts/Custom/VystiaClasses/Spells/IceMage/` |
| Spellbook | `ServUO/Scripts/Items/Equipment/Spellbooks/VystiaSpellbooks.cs` |
| Reagents | `ServUO/Scripts/Items/Vystia/Resources/Reagents/IceMagicReagents.cs` |
| Commands | `ServUO/Scripts/Custom/Commands/VystiaSpellbookCommands.cs` |

---

*Last Updated: 2025-12-28*
*Documentation synced with ServUO implementation*
