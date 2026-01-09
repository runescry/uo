# Artificer - Combat Engineer

**Class:** Artificer
**Ability IDs:** 2160-2175 (16 abilities)
**Archetype:** Gadget DPS
**Theme:** Combat engineer with turrets, constructs, and gadgets
**Primary Resource:** Mana
**Secondary Resource:** Constructs (implied)

---

## Ability Overview

### Distribution by Type
- **Damage (Single Target):** 1 ability (6%)
- **Damage (AoE):** 4 abilities (25%)
- **Buff/Utility:** 7 abilities (44%)
- **Summon/Construct:** 4 abilities (25%)
- **Defense:** 2 abilities (13%)

### Distribution by Circle
- **Circle 1:** 4 abilities (Clockwork Shot, Deploy Turret, Gadget Shield, Tinker)
- **Circle 2:** 4 abilities (Steam Blast, EMP Bomb, Net Launcher, Overclock)
- **Circle 3:** 4 abilities (Mechanical Companion, Gadget Bomb, Emergency Shield, Rocket Boots)
- **Circle 4:** 4 abilities (Deploy Artillery, Clockwork Army, Mech Suit, Overcharge)

---

## Circle 1 Abilities (Mana Cost: 4)

| ID | Name | Type | Range | Damage | Effect | Description |
|----|------|------|-------|--------|--------|-------------|
| 2160 | Clockwork Shot | Damage (Single) | 12 | 12-18 Physical | Effect: 0x36D4, Sound: 0x481 | Ranged gadget attack with physical damage |
| 2161 | Deploy Turret | Summon | 12 | - | Buff: AllStats +10 (30s) | Place gun turret construct at target location |
| 2162 | Gadget Shield | Buff | Self | - | Buff: AllStats +10 (30s) | Create energy barrier around caster |
| 2163 | Tinker | Utility | 12 | - | Buff: AllStats +10 (30s) | Repair construct allies (healing placeholder) |

**Notes:**
- Deploy Turret uses AllStats buff as placeholder for turret summon
- Tinker healing is not yet implemented
- All abilities share the same visual effect (0x36D4)

---

## Circle 2 Abilities (Mana Cost: 7)

| ID | Name | Type | Range | Damage | Effect | Description |
|----|------|------|-------|--------|--------|-------------|
| 2164 | Steam Blast | Damage (AoE) | 4 radius | 14-22 Physical | Effect: 0x36D4, Sound: 0x481 | Cone-shaped steam damage in front of caster |
| 2165 | EMP Bomb | Utility (AoE) | 4 radius | 0 | Effect: 0x36D4, Sound: 0x481 | Disable nearby construct enemies |
| 2166 | Net Launcher | Crowd Control | 12 | - | Buff: AllStats +10 (30s) | Root target in place temporarily |
| 2167 | Overclock | Buff | Self | - | Buff: AllStats +10 (30s) | +50% construct damage for duration |

**Notes:**
- EMP Bomb does no damage (construct disable mechanic not implemented)
- Net Launcher root mechanic uses buff placeholder
- Overclock damage boost not yet functional

---

## Circle 3 Abilities (Mana Cost: 10)

| ID | Name | Type | Range | Damage | Effect | Description |
|----|------|------|-------|--------|--------|-------------|
| 2168 | Mechanical Companion | Summon | 12 | - | Buff: AllStats +10 (30s) | Build clockwork pet companion |
| 2169 | Gadget Bomb | Damage (AoE) | 4 radius | 25-40 Physical | Effect: 0x36D4, Sound: 0x481 | Throw explosive device dealing heavy area damage |
| 2170 | Emergency Shield | Defense | Self | - | Buff: AllStats +10 (60s) | Damage absorb shield (longer duration) |
| 2171 | Rocket Boots | Mobility | 12 | - | Buff: AllStats +10 (30s) | Jet pack dash to target location |

**Notes:**
- Mechanical Companion uses buff placeholder instead of pet summon
- Rocket Boots uses buff placeholder for mobility (no actual dash implemented)
- Emergency Shield has double duration (60s vs 30s)

---

## Circle 4 Abilities (Mana Cost: 13)

| ID | Name | Type | Range | Damage | Effect | Description |
|----|------|------|-------|--------|--------|-------------|
| 2172 | Deploy Artillery | Summon | 12 | - | Buff: AllStats +10 (30s) | Place heavy siege turret construct |
| 2173 | Clockwork Army | Summon | 12 | - | Buff: AllStats +10 (30s) | Summon 5 clockwork bot allies |
| 2174 | Mech Suit | Transform | Self | - | Buff: AllStats +10 (30s) | Enter powered exosuit form |
| 2175 | Overcharge | Damage (AoE) | 4 radius | 50-80 Physical | Effect: 0x36D4, Sound: 0x481 | Massive steam explosion (ultimate) |

**Notes:**
- Deploy Artillery and Clockwork Army use buff placeholders (no summons implemented)
- Mech Suit transform not yet functional (buff placeholder)
- Overcharge is highest damage AoE ability

---

## Ability Details

### Damage Abilities (5 total)

| Ability | Circle | Damage Range | Type | Area | Notes |
|---------|--------|--------------|------|------|-------|
| Clockwork Shot | 1 | 12-18 | Physical | Single (12 range) | Basic ranged attack |
| Steam Blast | 2 | 14-22 | Physical | 4 radius AoE | Cone-shaped damage |
| Gadget Bomb | 3 | 25-40 | Physical | 4 radius AoE | Heavy explosive |
| Overcharge | 4 | 50-80 | Physical | 4 radius AoE | Ultimate explosion |

**Damage Progression:** 12→14→25→50 (min), 18→22→40→80 (max)

### Summon/Construct Abilities (4 total)

| Ability | Circle | Description | Status |
|---------|--------|-------------|--------|
| Deploy Turret | 1 | Gun turret construct | Placeholder (uses AllStats buff) |
| Mechanical Companion | 3 | Clockwork pet | Placeholder (uses AllStats buff) |
| Deploy Artillery | 4 | Heavy siege turret | Placeholder (uses AllStats buff) |
| Clockwork Army | 4 | 5 clockwork bots | Placeholder (uses AllStats buff) |

**Implementation Status:** All summons use temporary AllStats buff (+10 for 30s) as placeholders

### Buff/Utility Abilities (7 total)

| Ability | Circle | Duration | Effect | Type |
|---------|--------|----------|--------|------|
| Gadget Shield | 1 | 30s | +10 AllStats | Defense |
| Tinker | 1 | 30s | +10 AllStats | Healing (placeholder) |
| Net Launcher | 2 | 30s | +10 AllStats | Root (placeholder) |
| Overclock | 2 | 30s | +10 AllStats | +50% construct damage (placeholder) |
| Emergency Shield | 3 | 60s | +10 AllStats | Damage absorb (placeholder) |
| Rocket Boots | 3 | 30s | +10 AllStats | Mobility (placeholder) |
| Mech Suit | 4 | 30s | +10 AllStats | Transform (placeholder) |

**Implementation Status:** All utility abilities use generic AllStats buff placeholders

---

## Combat Strategy

### Rotation Recommendations
1. **Opening:** Clockwork Shot (2160) → Deploy Turret (2161)
2. **AoE Packs:** Steam Blast (2164) → Gadget Bomb (2169)
3. **Defensive:** Emergency Shield (2170) when low HP
4. **Ultimate:** Overcharge (2175) for maximum burst AoE

### Resource Management
- Circle 1: 4 mana per ability
- Circle 2: 7 mana per ability
- Circle 3: 10 mana per ability
- Circle 4: 13 mana per ability

**Total Mana for Full Rotation:** ~60-80 mana (4 abilities)

---

## Known Issues

### Critical (Functionality Missing)
1. **All summon abilities** use AllStats buff placeholders instead of spawning actual constructs
2. **Tinker (2163):** No construct healing implemented
3. **EMP Bomb (2165):** No construct disable mechanic (0 damage, no effect)
4. **Net Launcher (2166):** No root/immobilize implementation
5. **Overclock (2167):** +50% construct damage boost not functional
6. **Rocket Boots (2171):** No dash/mobility implementation
7. **Mech Suit (2174):** No transform/bodymod implementation

### Medium (Incomplete Mechanics)
8. **All abilities share same visual effect** (0x36D4) - no unique effects
9. **Emergency Shield (2170):** Damage absorb not implemented (uses generic buff)
10. **Mechanical Companion (2168):** No clockwork pet summoning
11. **Deploy Artillery (2172):** No heavy turret summon
12. **Clockwork Army (2173):** No multi-summon implementation

### Low (Polish)
13. **No construct resource tracking** (should track active constructs)
14. **No construct health bars** for player-owned constructs
15. **Turret targeting AI** not implemented
16. **Construct expiration/duration** not defined

---

## Future Enhancements

### Phase 1: Core Mechanics (Critical)
1. Implement construct summoning system (BaseCreature with custom AI)
2. Add construct resource tracking (max 3-5 active constructs)
3. Implement EMP Bomb construct disable (stun construct-type enemies)
4. Add Tinker construct healing (target ally construct, restore HP)

### Phase 2: Advanced Abilities (Important)
5. Implement Net Launcher root effect (CrowdControlSystem.Root)
6. Add Overclock damage boost (buff construct damage output)
7. Implement Rocket Boots dash (teleport + particle trail)
8. Add Mech Suit transform (bodymod + stat boost)

### Phase 3: Polish (Enhancement)
9. Unique visual effects per ability (steam, electricity, explosions)
10. Construct control commands (Attack, Defend, Follow, Stop)
11. Emergency Shield damage absorb shield (VystiaBuffType.Shield)
12. Construct expiration timers and UI indicators

---

## File Locations

**Implementation:**
- `ServUO/Scripts/Custom/VystiaClasses/Abilities/Generated/Martial/ArtificerAbilities.cs`

**Class Definition:**
- `ServUO/Scripts/Custom/VystiaClasses/Classes/Martial/ArtificerClass.cs`

**Special Items:**
- `ServUO/Scripts/Custom/VystiaClasses/Items/ClassSpecialItems.cs` (ConstructControlDevice)

**Generation Script:**
- `ServUO/Scripts/Custom/VystiaClasses/Tools/generate_martial_abilities.py`

---

**Last Updated:** 2025-12-28
**Status:** 16/16 abilities implemented with placeholders, core mechanics pending
**Build Status:** ✅ Compiles successfully, ⚠️ Limited functionality
