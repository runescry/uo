# Nature Magic - Verdantpeak School

**Class:** Druid
**Region:** Verdantpeak
**Theme:** Shapeshifting, healing, poison/earth damage, plant control, animal summons
**Spellbook:** Druid Spellbook (DruidSpellbook.cs)
**Primary Stat:** Intelligence / Strength (shapeshifted)
**Hue:** 0x7D6 (Forest Green)
**Implementation Status:** ✅ **32/32 spells implemented (100% COMPLETE - tested 2025-12-08)**

> **📝 Reagent Note:** Individual spell entries below use placeholder UO reagent names for design reference. The actual ServUO implementation uses custom Vystia reagents as documented in the [Reagents](#reagents) section at the end of this file. See `NatureMagicReagents.cs` for implementation.

---

## Overview

Nature Magic channels the primal forces of Verdantpeak's ancient forests. Druids are versatile spellcasters who can shapeshift into animals, command plants, heal allies, and unleash nature's wrath through poison and earth magic.

**Strengths:**
- Shapeshifting for melee combat
- Strong healing and regeneration
- Versatile (caster or melee via shapeshifting)
- Summons animal companions
- Good sustained damage (poison DoTs)

**Weaknesses:**
- Weaker burst damage than pure mages
- Shapeshifting locks out spellcasting
- Vulnerable to fire damage
- Less effective against undead/constructs

---

## Spellbook Design

### DruidSpellbook
- **Item ID:** 0xEFA (Spellbook graphic)
- **Hue:** 0x7D6 (Forest Green)
- **Name:** "Codex of the Wild"
- **Capacity:** 32 spells
- **Weight:** 3.0 stones
- **Special:** Grants +10% poison damage and +5 HP regen when equipped

---

## Spell List (32 Spells)

### Circle 1 (4 mana) - Seedling Magic
*Mana: 4 | Cast Time: 0.5-1.0s*

#### 1. Nature's Touch
- **Mana:** 4
- **Reagents:** Ginseng, Garlic
- **Cast Time:** 0.5s
- **Range:** 8 tiles
- **Effect:** Heals 8-15 HP
- **Visual:** Green sparkles (0x376A, hue green)
- **Sound:** 0x1F2
- **Purpose:** Basic healing

#### 2. Thorn Dart
- **Mana:** 4
- **Reagents:** MandrakeRoot
- **Cast Time:** 0.75s
- **Range:** 10 tiles
- **Effect:** Deals 4-10 damage (50% poison, 50% physical)
- **Visual:** Flying thorn (0x1BFB)
- **Sound:** 0x5D3
- **Purpose:** Basic ranged attack

#### 3. Barkskin
- **Mana:** 4
- **Reagents:** Garlic, SpidersSilk
- **Cast Time:** 1.0s
- **Duration:** 30 seconds
- **Effect:** +5 Physical Resistance, +5 Poison Resistance
- **Visual:** Brown bark texture on skin (0x32C)
- **Sound:** 0x22F
- **Purpose:** Basic defense

#### 4. Detect Life
- **Mana:** 4
- **Reagents:** Nightshade
- **Cast Time:** 0.75s
- **Duration:** 60 seconds
- **Effect:** Reveals hidden creatures within 12 tiles, shows HP bars
- **Visual:** Green pulses (0x373A)
- **Sound:** 0x1EA
- **Purpose:** Tracking and detection

---

### Circle 2 (6 mana) - Grove Magic
*Mana: 6 | Cast Time: 1.0-1.5s*

#### 5. Entangle
- **Mana:** 6
- **Reagents:** SpidersSilk, Bloodmoss
- **Cast Time:** 1.25s
- **Range:** 10 tiles
- **Duration:** 5 seconds
- **Effect:** Roots target in place (cannot move, can still cast/attack)
- **Visual:** Vines wrap around legs (0xCEB)
- **Sound:** 0x5D1
- **Purpose:** Basic CC (root)

#### 6. Poison Spores
- **Mana:** 6
- **Reagents:** Nightshade, Bloodmoss
- **Cast Time:** 1.0s
- **Range:** 12 tiles
- **Duration:** 12 seconds
- **Effect:** DoT dealing 3-5 poison damage/tick (6 ticks)
- **Visual:** Green cloud around target (0x372A)
- **Sound:** 0x205
- **Purpose:** Poison DoT

#### 7. Rejuvenation
- **Mana:** 6
- **Reagents:** Ginseng, Garlic, MandrakeRoot
- **Cast Time:** 1.5s
- **Range:** 8 tiles
- **Duration:** 30 seconds
- **Effect:** Heals 3-6 HP/tick (10 ticks = 30-60 total)
- **Visual:** Green glow (0x376A)
- **Sound:** 0x1F2
- **Purpose:** HoT (Heal over Time)

#### 8. Animal Aspect: Speed
- **Mana:** 6
- **Reagents:** MandrakeRoot, Bloodmoss
- **Cast Time:** 1.0s
- **Duration:** 60 seconds
- **Effect:** +20 DEX, +15% movement speed
- **Visual:** Ghostly rabbit aura (0x00D7)
- **Sound:** 0x1EA
- **Purpose:** Mobility buff

---

### Circle 3 (9 mana) - Wildwood Magic
*Mana: 9 | Cast Time: 1.5-2.0s*

#### 9. Wild Growth
- **Mana:** 9
- **Reagents:** Bloodmoss, SpidersSilk, MandrakeRoot
- **Cast Time:** 2.0s
- **Range:** 12 tiles (ground target)
- **Area:** 4x4 tiles
- **Duration:** 20 seconds
- **Effect:** Creates thick vegetation that blocks line of sight and slows enemies 30%
- **Visual:** Dense plants (0x0CC1 + particles)
- **Sound:** 0x5D0
- **Purpose:** Area denial and LOS blocking

#### 10. Bear Form (Lesser Shapeshift)
- **Mana:** 9
- **Reagents:** MandrakeRoot, Bloodmoss, Ginseng
- **Cast Time:** 1.5s
- **Duration:** 180 seconds
- **Effect:** Transform into bear: +30 STR, +15 Physical Resist, melee attacks deal bonus damage, CANNOT CAST SPELLS
- **Visual:** Transform into bear (Body 0x00A7)
- **Sound:** 0x22E
- **Purpose:** Melee combat form

#### 11. Thorn Volley
- **Mana:** 9
- **Reagents:** MandrakeRoot, SulfurousAsh
- **Cast Time:** 1.75s
- **Range:** 12 tiles
- **Area:** 3 tile radius
- **Effect:** Deals 12-22 damage to all enemies in area (50% poison, 50% physical)
- **Visual:** Multiple thorns fly out (0x1BFB x6)
- **Sound:** 0x5D3
- **Purpose:** AoE damage

#### 12. Nature's Blessing
- **Mana:** 9
- **Reagents:** Ginseng, Garlic, Bloodmoss
- **Cast Time:** 1.5s
- **Range:** 8 tiles
- **Duration:** 90 seconds
- **Effect:** +10% max HP, +5 HP regen/tick, +10 Poison Resist
- **Visual:** Green aura (0x376A)
- **Sound:** 0x1F2
- **Purpose:** Defensive buff

---

### Circle 4 (11 mana) - Ancient Grove
*Mana: 11 | Cast Time: 2.0-2.5s*

#### 13. Wolf Form (Advanced Shapeshift)
- **Mana:** 11
- **Reagents:** MandrakeRoot, Bloodmoss, BlackPearl
- **Cast Time:** 2.0s
- **Duration:** 180 seconds
- **Effect:** Transform into wolf: +25 DEX, +30% movement speed, +15% attack speed, bleed attacks, CANNOT CAST SPELLS
- **Visual:** Transform into dire wolf (Body 0x00E1)
- **Sound:** 0xE5
- **Purpose:** Fast melee DPS form

#### 14. Strangling Vines
- **Mana:** 11
- **Reagents:** SpidersSilk, Bloodmoss, MandrakeRoot
- **Cast Time:** 2.25s
- **Range:** 10 tiles
- **Duration:** 8 seconds
- **Effect:** Target is rooted and takes 6-10 damage/tick, -20% attack speed
- **Visual:** Thick vines constrict target (0xCEA + particles)
- **Sound:** 0x5D1
- **Purpose:** Strong CC with damage

#### 15. Healing Grove
- **Mana:** 11
- **Reagents:** Ginseng, Garlic, MandrakeRoot, Bloodmoss
- **Cast Time:** 2.5s
- **Range:** 12 tiles (ground target)
- **Area:** 5 tile radius
- **Duration:** 15 seconds
- **Effect:** Allies in area heal 5-8 HP/tick
- **Visual:** Green healing circle (0x376A + grass)
- **Sound:** 0x1F2
- **Purpose:** AoE HoT zone

#### 16. Toxic Bloom
- **Mana:** 11
- **Reagents:** Nightshade, Bloodmoss, SulfurousAsh
- **Cast Time:** 2.0s
- **Range:** 12 tiles
- **Area:** 4 tile radius
- **Effect:** Explodes for 15-25 poison damage, applies poison DoT (5 damage/tick for 10s)
- **Visual:** Poisonous flower explosion (0x36CB)
- **Sound:** 0x205
- **Purpose:** Poison AoE burst

---

### Circle 5 (14 mana) - Primal Magic
*Mana: 14 | Cast Time: 2.5-3.0s*

#### 17. Hawk Form (Aerial Shapeshift)
- **Mana:** 14
- **Reagents:** MandrakeRoot, Bloodmoss, BlackPearl, SpidersSilk
- **Cast Time:** 2.5s
- **Duration:** 120 seconds
- **Effect:** Transform into hawk: Can fly over obstacles, +40 DEX, -50% damage taken from ranged, weak melee, CANNOT CAST SPELLS
- **Visual:** Transform into bird (Body 0x0006)
- **Sound:** 0x1E3
- **Purpose:** Mobility/escape form

#### 18. Earthquake
- **Mana:** 14
- **Reagents:** Bloodmoss, SulfurousAsh, MandrakeRoot
- **Cast Time:** 2.75s
- **Range:** Self (8 tile radius)
- **Effect:** Deals 20-35 physical damage, knocks down enemies (stun 2s), destroys ice walls/barriers
- **Visual:** Ground shaking effect (0x36BD)
- **Sound:** 0x307
- **Purpose:** AoE stun and barrier break

#### 19. Greater Regeneration
- **Mana:** 14
- **Reagents:** Ginseng, Garlic, MandrakeRoot, Bloodmoss
- **Cast Time:** 2.5s
- **Range:** 8 tiles
- **Duration:** 45 seconds
- **Effect:** Heals 8-12 HP/tick (15 ticks = 120-180 total)
- **Visual:** Intense green glow (0x376A)
- **Sound:** 0x1F2
- **Purpose:** Powerful HoT

#### 20. Spore Cloud
- **Mana:** 14
- **Reagents:** Nightshade, Bloodmoss, BlackPearl
- **Cast Time:** 2.5s
- **Range:** 12 tiles (ground target)
- **Area:** 6 tile radius
- **Duration:** 12 seconds
- **Effect:** Poison cloud dealing 5-9 damage/tick, reduces visibility, -25% accuracy
- **Visual:** Thick green fog (0x113A)
- **Sound:** 0x205
- **Purpose:** Zone control with blind

---

### Circle 6 (20 mana) - Elder Grove
*Mana: 20 | Cast Time: 3.0-3.5s*

#### 21. Treant Form (Greater Shapeshift)
- **Mana:** 20
- **Reagents:** MandrakeRoot, Bloodmoss, BlackPearl, Garlic
- **Cast Time:** 3.0s
- **Duration:** 180 seconds
- **Effect:** Transform into treant: +50 STR, +30 Physical Resist, +25% max HP, AoE root on attacks, CANNOT CAST SPELLS
- **Visual:** Transform into treant (Body 0x002F)
- **Sound:** 0x5DC
- **Purpose:** Tank/bruiser form

#### 22. Swarm
- **Mana:** 20
- **Reagents:** Bloodmoss, Nightshade, MandrakeRoot, SpidersSilk
- **Cast Time:** 3.0s
- **Range:** 14 tiles
- **Area:** 5 tile radius
- **Duration:** 10 seconds
- **Effect:** Swarm of insects deals 6-10 damage/tick, -40% accuracy, interrupts spellcasting
- **Visual:** Insect cloud (0x91B + particles)
- **Sound:** 0x5D4
- **Purpose:** Anti-caster AoE

#### 23. Living Fortress
- **Mana:** 20
- **Reagents:** MandrakeRoot, Garlic, Ginseng, SpidersSilk, Bloodmoss
- **Cast Time:** 3.5s
- **Duration:** 60 seconds
- **Effect:** +20 Physical Resist, +20 Poison Resist, +15% max HP, immune to roots/slows, heals 5 HP/tick
- **Visual:** Thick bark covering body (0x32C + glow)
- **Sound:** 0x22F
- **Purpose:** Ultimate defense buff

#### 24. Nature's Wrath
- **Mana:** 20
- **Reagents:** MandrakeRoot, SulfurousAsh, BlackPearl, Bloodmoss
- **Cast Time:** 3.25s
- **Range:** 14 tiles (direction)
- **Area:** 8 tile cone
- **Effect:** Deals 35-55 damage (mix of poison/physical), applies poison DoT, roots all targets
- **Visual:** Massive plant surge (0x0CC1 + thorns)
- **Sound:** 0x5D0
- **Purpose:** Massive cone attack

---

### Circle 7 (40 mana) - Primordial Magic
*Mana: 40 | Cast Time: 3.5-4.0s*

#### 25. Force of Nature
- **Mana:** 40
- **Reagents:** All 8 reagents
- **Cast Time:** 4.0s
- **Duration:** 30 seconds
- **Effect:** Shapeshifts rapidly between all forms (changes every 5s), gains all benefits simultaneously, can still cast spells
- **Visual:** Flickering between all animal forms
- **Sound:** Multiple transform sounds
- **Purpose:** Ultimate hybrid combat

#### 26. Summon Ancient Treant
- **Mana:** 40
- **Reagents:** MandrakeRoot, Bloodmoss, SpidersSilk, BlackPearl, LivingBark (10)
- **Cast Time:** 3.5s
- **Duration:** 240 seconds
- **Effect:** Summons Ancient Treant (1200 HP, powerful melee, AoE root, high resists)
- **Visual:** Tree grows from ground (0x0CCA → Body 0x002F)
- **Sound:** 0x5DC
- **Purpose:** Powerful summon

#### 27. Plague
- **Mana:** 40
- **Reagents:** Nightshade, Bloodmoss, BlackPearl, MandrakeRoot, SulfurousAsh
- **Cast Time:** 3.75s
- **Range:** 12 tiles
- **Area:** 8 tile radius
- **Duration:** 20 seconds
- **Effect:** Devastating poison DoT (10-15 damage/tick), spreads to nearby enemies within 3 tiles
- **Visual:** Sickly green aura (0x372A + fog)
- **Sound:** 0x205
- **Purpose:** Spreading poison ultimate

#### 28. Primordial Restoration
- **Mana:** 40
- **Reagents:** Ginseng, Garlic, MandrakeRoot, Bloodmoss, BlackPearl, TreantHeart (5)
- **Cast Time:** 4.0s
- **Range:** 10 tiles
- **Area:** 8 tile radius
- **Effect:** Massive AoE heal (80-120 HP), removes all poisons/curses, grants immunity to debuffs for 15s
- **Visual:** Massive green explosion (0x376A)
- **Sound:** 0x1F2
- **Purpose:** Emergency group heal

---

### Circle 8 (50 mana) - Legendary Nature
*Mana: 50 | Cast Time: 4.0-5.0s*

#### 29. World Tree's Embrace
- **Mana:** 50
- **Reagents:** All 8 reagents + LivingBark (10) + TreantHeart (5)
- **Cast Time:** 5.0s
- **Range:** Self (12 tile radius)
- **Duration:** 45 seconds
- **Effect:** Massive tree grows from caster: Allies gain +50% HP, 10 HP/tick regen, all resists +25. Enemies take 8-15 damage/tick and are slowed 75%
- **Visual:** Enormous tree (0x0CCA scaled 3x)
- **Sound:** 0x5DC
- **Purpose:** Zone transformation ultimate

#### 30. Hydra Form (Legendary Shapeshift)
- **Mana:** 50
- **Reagents:** All 8 reagents
- **Cast Time:** 4.5s
- **Duration:** 120 seconds
- **Effect:** Transform into multi-headed hydra: +70 STR, +30 DEX, triple attack, regenerates 15 HP/tick, immune to poison, CANNOT CAST SPELLS
- **Visual:** Transform into sea serpent/hydra (Body 0x0096)
- **Sound:** 0x16A
- **Purpose:** Ultimate combat form

#### 31. Thorn Apocalypse
- **Mana:** 50
- **Reagents:** All 8 reagents
- **Cast Time:** 4.0s
- **Area:** Entire screen (30 tile radius)
- **Effect:** Thorns erupt from ground dealing 60-100 damage (50% poison, 50% physical), applies bleed and poison DoTs, roots all enemies for 5s
- **Visual:** Thorns everywhere (0x1BFB + 0xCEB)
- **Sound:** 0x5D3
- **Purpose:** Screen-wide devastation

#### 32. Avatar of the Forest
- **Mana:** 50
- **Reagents:** All 8 reagents + TreantHeart (10)
- **Cast Time:** 4.5s
- **Duration:** 60 seconds
- **Effect:** Become nature incarnate: Can shapeshift instantly and maintain spellcasting, all nature spells cost 50% less mana, immune to poison/fire, +40 all resists, summons 3 treant guardians
- **Visual:** Glowing green aura with forest imagery (0x376A + trees)
- **Sound:** 0x5DC
- **Purpose:** Transformation ultimate

---

## Spell Progression

**Early Game (Circles 1-3):** Basic healing, shapeshifting, poison damage
**Mid Game (Circles 4-5):** Advanced forms, zone control, powerful HoTs
**Late Game (Circles 6-7):** Ultimate forms, summons, plague mechanics
**End Game (Circle 8):** Legendary shapeshifts, screen-wide effects, avatar form

---

## Shapeshift Forms Summary

| Form | Circle | STR | DEX | Special | Duration |
|------|--------|-----|-----|---------|----------|
| Bear | 3 | +30 | - | High damage, tanky | 180s |
| Wolf | 4 | - | +25 | Fast, bleed attacks | 180s |
| Hawk | 5 | - | +40 | Flying, ranged resist | 120s |
| Treant | 6 | +50 | - | Massive tank, AoE roots | 180s |
| Hydra | 8 | +70 | +30 | Triple attack, regen | 120s |

**Note:** All shapeshifts prevent spellcasting except Force of Nature and Avatar of the Forest

---

## Reagent Usage Summary

**Primary Reagents:**
- MandrakeRoot (25 spells) - Core nature reagent
- Bloodmoss (24 spells) - Shapeshifting and DoTs
- Ginseng (13 spells) - Healing spells
- Garlic (11 spells) - Defensive buffs
- Nightshade (9 spells) - Poison spells
- SpidersSilk (13 spells) - Roots and CC
- SulfurousAsh (8 spells) - Offensive spells
- BlackPearl (12 spells) - High-tier spells

**Special Resources:**
- LivingBark - 2 spells (Summon Ancient Treant, World Tree's Embrace)
- TreantHeart - 3 spells (Primordial Restoration, World Tree's Embrace, Avatar of the Forest)

---

## Mana Cost Distribution

- **Circle 1:** 4 mana × 4 spells = 16 total
- **Circle 2:** 6 mana × 4 spells = 24 total
- **Circle 3:** 9 mana × 4 spells = 36 total
- **Circle 4:** 11 mana × 4 spells = 44 total
- **Circle 5:** 14 mana × 4 spells = 56 total
- **Circle 6:** 20 mana × 4 spells = 80 total
- **Circle 7:** 40 mana × 4 spells = 160 total
- **Circle 8:** 50 mana × 4 spells = 200 total

---

## Reagents

Nature Magic uses the following Vystia-specific reagents organized by spell circle:

### Primary Reagents (6 types)
1. **Wild Moss** (Circles 1-3) - Luminescent forest moss from ancient forests
2. **Moonpetal** (Circles 2-4) - Silver forest flower from moonlit groves
3. **Druid Bark** (Circles 1-4) - Sacred tree bark from sacred groves
4. **Treant Sap** (Circles 3-6) - Living tree essence from treant groves
5. **Elderwood Seed** (Circles 5-7) - Ancient tree seed from Verdantpeak heart
6. **Primal Vine** (Circles 6-8) - Magical living vine from primordial forests

### Rare Reagents (2 types)
7. **Treant Heart** (Circles 7-8) - Heart of ancient treant (existing resource)
8. **Living Bark** (Circle 8) - Regenerating bark, ultimate reagent (existing resource)

**Implementation:** All reagents created in `NatureMagicReagents.cs`

---

## ServUO Implementation Notes

This section provides detailed guidance on implementing Nature Magic spells in ServUO, categorizing mechanics by complexity.

---

### ✅ Standard Mechanics (Built-in ServUO)

These mechanics are natively supported by ServUO and require minimal custom code:

#### Direct Damage Spells
- **Spells:** Thorn Dart, Thorn Volley, Thorn Apocalypse
- **ServUO API:** `SpellHelper.Damage(spell, target, damage, phys%, fire%, cold%, pois%, nrgy%)`
- **Example:** `SpellHelper.Damage(this, target, damage, 50, 0, 0, 50, 0)` for 50% physical, 50% poison
- **AoE:** Use `map.GetMobilesInRange(location, radius)`, remember to call `eable.Free()`

#### Healing Spells
- **Spells:** Nature's Touch, Primordial Restoration
- **ServUO API:**
  - `target.Hits += healAmount` (cap at `target.HitsMax`)
  - Check `if (target.Hits + healAmount > target.HitsMax) target.Hits = target.HitsMax`
- **AoE Healing:** Iterate through allies with `GetMobilesInRange()`

#### Heal over Time (HoT)
- **Spells:** Rejuvenation, Greater Regeneration, Healing Grove
- **ServUO API:** Use `Timer.DelayCall()` with repeated healing
```csharp
int ticks = 10;
int healPerTick = 5;
Timer.DelayCall(TimeSpan.FromSeconds(3.0), TimeSpan.FromSeconds(3.0), ticks, () =>
{
    if (target != null && !target.Deleted && target.Alive)
        target.Hits = Math.Min(target.Hits + healPerTick, target.HitsMax);
});
```

#### Poison DoTs
- **Spells:** Poison Spores, Toxic Bloom, Spore Cloud
- **ServUO API:** Built-in `Poison` class with 5 levels
  - `Poison.Lesser, Poison.Regular, Poison.Greater, Poison.Deadly, Poison.Lethal`
  - Apply: `target.ApplyPoison(caster, Poison.Regular)`
  - Check immunity: `target.Poisoned` or `target.Poison != null`
- **Custom DoT:** For custom damage amounts, use Timer-based DoT (see HoT example)

#### Root Effects
- **Spells:** Entangle, Strangling Vines, Nature's Wrath
- **ServUO API:** `target.Frozen = true/false`
- **Duration:** Use `Timer.DelayCall(duration, () => target.Frozen = false)`
- **Note:** Frozen prevents movement but allows casting/attacking

#### Stun/Knockdown
- **Spells:** Earthquake
- **ServUO API:** `target.Paralyzed = true/false`
- **Duration:** Short durations (1-3s) for knockdown
```csharp
target.Paralyzed = true;
Timer.DelayCall(TimeSpan.FromSeconds(2.0), () => target.Paralyzed = false);
```

#### Stat Buffs
- **Spells:** Animal Aspect: Speed, all shapeshift forms
- **ServUO API:** `StatMod(StatType, name, offset, duration)`
```csharp
StatMod dexBuff = new StatMod(StatType.Dex, "AnimalSpeed", 20, TimeSpan.FromSeconds(60.0));
target.AddStatMod(dexBuff);
```

#### Resistance Buffs
- **Spells:** Barkskin, Living Fortress, Nature's Blessing
- **ServUO API:** `ResistanceMod(ResistanceType, offset)`
```csharp
ResistanceMod physMod = new ResistanceMod(ResistanceType.Physical, 10);
ResistanceMod poisMod = new ResistanceMod(ResistanceType.Poison, 20);
target.AddResistanceMod(physMod);
target.AddResistanceMod(poisMod);

// Remove after duration
Timer.DelayCall(duration, () => {
    target.RemoveResistanceMod(physMod);
    target.RemoveResistanceMod(poisMod);
});
```

#### Basic Summons
- **Spells:** Summon Ancient Treant
- **ServUO API:** `BaseCreature` with AI types
  - Create custom creature class inheriting from `BaseCreature`
  - Set `Controlled = true`, `ControlMaster = caster`
  - Set `SummonMaster = caster` for summoned creatures
  - Handle despawn with `Timer.DelayCall(duration, () => summon.Delete())`

---

### 🔧 Custom Mechanics (Require Implementation)

These mechanics need custom classes and systems to be implemented.

#### 1. Shapeshifting System (5 Forms)
**Spells:** Bear Form, Wolf Form, Hawk Form, Treant Form, Hydra Form

**Implementation:** Custom `ShapeshiftContext` class to manage form state

```csharp
public class ShapeshiftContext
{
    public Mobile Mobile { get; private set; }
    public ShapeshiftForm Form { get; private set; }
    public DateTime EndTime { get; private set; }
    public Timer ExpireTimer { get; private set; }

    private int m_OriginalBody;
    private List<StatMod> m_StatMods = new List<StatMod>();
    private List<ResistanceMod> m_ResistMods = new List<ResistanceMod>();

    public ShapeshiftContext(Mobile m, ShapeshiftForm form, TimeSpan duration)
    {
        Mobile = m;
        Form = form;
        EndTime = DateTime.UtcNow + duration;

        m_OriginalBody = m.BodyValue;

        // Apply transformation
        ApplyForm();

        // Set expiration timer
        ExpireTimer = Timer.DelayCall(duration, EndShapeshift);
    }

    private void ApplyForm()
    {
        // Change body
        Mobile.BodyValue = Form.BodyID;

        // Apply stat mods
        if (Form.StrBonus != 0)
        {
            StatMod mod = new StatMod(StatType.Str, "Shapeshift_Str", Form.StrBonus, TimeSpan.Zero);
            Mobile.AddStatMod(mod);
            m_StatMods.Add(mod);
        }
        if (Form.DexBonus != 0)
        {
            StatMod mod = new StatMod(StatType.Dex, "Shapeshift_Dex", Form.DexBonus, TimeSpan.Zero);
            Mobile.AddStatMod(mod);
            m_StatMods.Add(mod);
        }

        // Apply resist mods
        foreach (var resist in Form.ResistBonuses)
        {
            ResistanceMod mod = new ResistanceMod(resist.Key, resist.Value);
            Mobile.AddResistanceMod(mod);
            m_ResistMods.Add(mod);
        }

        // Lock spellcasting (see below)
        Mobile.SetMagicLock(true);
    }

    public void EndShapeshift()
    {
        // Restore body
        Mobile.BodyValue = m_OriginalBody;

        // Remove all mods
        foreach (StatMod mod in m_StatMods)
            Mobile.RemoveStatMod(mod.Name);
        foreach (ResistanceMod mod in m_ResistMods)
            Mobile.RemoveResistanceMod(mod);

        // Unlock spellcasting
        Mobile.SetMagicLock(false);

        ExpireTimer?.Stop();
    }
}

public enum ShapeshiftFormType
{
    None,
    Bear,
    Wolf,
    Hawk,
    Treant,
    Hydra
}

public class ShapeshiftForm
{
    public ShapeshiftFormType Type { get; set; }
    public int BodyID { get; set; }
    public int StrBonus { get; set; }
    public int DexBonus { get; set; }
    public Dictionary<ResistanceType, int> ResistBonuses { get; set; }
    public bool CanFly { get; set; }
    public bool AllowSpellcasting { get; set; }

    // Define forms
    public static ShapeshiftForm Bear = new ShapeshiftForm
    {
        Type = ShapeshiftFormType.Bear,
        BodyID = 0x00A7,
        StrBonus = 30,
        DexBonus = 0,
        ResistBonuses = new Dictionary<ResistanceType, int> { { ResistanceType.Physical, 15 } },
        CanFly = false,
        AllowSpellcasting = false
    };

    // Define other forms similarly...
}
```

**Spell Lockout:** To prevent spellcasting while shapeshifted:
```csharp
// In Mobile class or via PlayerMobile extension
public bool m_MagicLocked = false;

public void SetMagicLock(bool locked)
{
    m_MagicLocked = locked;
}

// In spell OnCast() check:
if (Caster.m_MagicLocked)
{
    Caster.SendLocalizedMessage(500015); // "You cannot cast while shapeshifted."
    return false;
}
```

**Storage:** Use a static dictionary to track active shapeshifts:
```csharp
public static class ShapeshiftManager
{
    private static Dictionary<Mobile, ShapeshiftContext> m_Contexts = new Dictionary<Mobile, ShapeshiftContext>();

    public static void ApplyShapeshift(Mobile m, ShapeshiftForm form, TimeSpan duration)
    {
        // End existing shapeshift
        if (m_Contexts.ContainsKey(m))
            m_Contexts[m].EndShapeshift();

        // Apply new shapeshift
        ShapeshiftContext context = new ShapeshiftContext(m, form, duration);
        m_Contexts[m] = context;
    }

    public static void EndShapeshift(Mobile m)
    {
        if (m_Contexts.ContainsKey(m))
        {
            m_Contexts[m].EndShapeshift();
            m_Contexts.Remove(m);
        }
    }

    public static bool IsShapeshifted(Mobile m)
    {
        return m_Contexts.ContainsKey(m);
    }

    public static ShapeshiftContext GetContext(Mobile m)
    {
        return m_Contexts.ContainsKey(m) ? m_Contexts[m] : null;
    }
}
```

#### 2. Wild Growth Terrain (Blocks LOS + Slows)
**Spells:** Wild Growth

**Implementation Method 1: Custom Item Blocker**
```csharp
public class WildGrowthItem : Item
{
    public WildGrowthItem() : base(0x0CC1) // Dense plant graphic
    {
        Movable = false;
        Name = "wild growth";
    }

    public override bool BlocksFit { get { return true; } } // Blocks line of sight

    public override bool OnMoveOver(Mobile m)
    {
        // Apply slow effect
        if (m != null && !m.Deleted)
        {
            StatMod slowMod = new StatMod(StatType.Dex, "WildGrowth_Slow", -30, TimeSpan.FromSeconds(3.0));
            m.AddStatMod(slowMod);
        }
        return true;
    }
}

// In spell: Create 4x4 grid of items
for (int x = -2; x <= 2; x++)
{
    for (int y = -2; y <= 2; y++)
    {
        Point3D loc = new Point3D(targetLoc.X + x, targetLoc.Y + y, targetLoc.Z);
        WildGrowthItem plant = new WildGrowthItem();
        plant.MoveToWorld(loc, map);

        // Delete after duration
        Timer.DelayCall(TimeSpan.FromSeconds(20.0), () => plant.Delete());
    }
}
```

#### 3. Healing Grove Zone (AoE HoT)
**Spells:** Healing Grove

**Implementation:** Custom Item with area effect
```csharp
public class HealingGroveItem : Item
{
    private Timer m_Timer;
    private Mobile m_Caster;
    private int m_Ticks = 5; // 15 seconds / 3 second ticks

    public HealingGroveItem(Mobile caster, Point3D loc, Map map) : base(0x08E4) // Glowing grass
    {
        Movable = false;
        Name = "healing grove";
        Hue = 0x7D6; // Green
        m_Caster = caster;

        MoveToWorld(loc, map);

        m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(3.0), TimeSpan.FromSeconds(3.0), m_Ticks, OnTick);
    }

    private void OnTick()
    {
        if (Deleted || Map == null)
            return;

        IPooledEnumerable eable = Map.GetMobilesInRange(Location, 5);
        foreach (Mobile m in eable)
        {
            if (m.Alive && (m == m_Caster || m.CanBeBeneficial(m_Caster)))
            {
                int heal = Utility.RandomMinMax(5, 8);
                m.Hits = Math.Min(m.Hits + heal, m.HitsMax);
                m.FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);
            }
        }
        eable.Free();

        if (--m_Ticks == 0)
            Delete();
    }
}
```

#### 4. Plague Spreading Mechanic
**Spells:** Plague

**Implementation:** Custom debuff that spreads to nearby enemies
```csharp
public class PlagueContext
{
    public Mobile Target { get; private set; }
    public Mobile Caster { get; private set; }
    public int Ticks { get; private set; }
    public Timer DoTTimer { get; private set; }

    public PlagueContext(Mobile caster, Mobile target, int ticks)
    {
        Caster = caster;
        Target = target;
        Ticks = ticks;

        DoTTimer = Timer.DelayCall(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0), ticks, OnTick);
    }

    private void OnTick()
    {
        if (Target == null || Target.Deleted || !Target.Alive)
        {
            DoTTimer?.Stop();
            return;
        }

        // Deal damage
        int damage = Utility.RandomMinMax(10, 15);
        AOS.Damage(Target, Caster, damage, 0, 0, 0, 100, 0); // 100% poison

        Target.FixedParticles(0x372A, 10, 15, 5021, 0x7D6, 0, EffectLayer.Waist);

        // Spread to nearby enemies
        IPooledEnumerable eable = Target.Map.GetMobilesInRange(Target.Location, 3);
        foreach (Mobile m in eable)
        {
            if (m != Target && m.Alive && Caster.CanBeHarmful(m))
            {
                if (!PlagueManager.IsInfected(m) && Utility.RandomDouble() < 0.3) // 30% spread chance
                {
                    PlagueManager.Infect(Caster, m, Ticks / 2); // Half duration for spread
                    m.SendMessage("You have been infected by plague!");
                }
            }
        }
        eable.Free();

        if (--Ticks == 0)
        {
            PlagueManager.CureInfection(Target);
            DoTTimer?.Stop();
        }
    }
}

public static class PlagueManager
{
    private static Dictionary<Mobile, PlagueContext> m_Infected = new Dictionary<Mobile, PlagueContext>();

    public static void Infect(Mobile caster, Mobile target, int ticks)
    {
        if (m_Infected.ContainsKey(target))
            return; // Already infected

        PlagueContext context = new PlagueContext(caster, target, ticks);
        m_Infected[target] = context;
    }

    public static bool IsInfected(Mobile m)
    {
        return m_Infected.ContainsKey(m);
    }

    public static void CureInfection(Mobile m)
    {
        if (m_Infected.ContainsKey(m))
        {
            m_Infected[m].DoTTimer?.Stop();
            m_Infected.Remove(m);
        }
    }
}
```

#### 5. Spore Cloud with Visibility Reduction
**Spells:** Spore Cloud

**Implementation:** Custom field effect (similar to poison field)
```csharp
public class SporeCloudItem : Item
{
    private Timer m_Timer;
    private Mobile m_Caster;
    private int m_Ticks = 4; // 12 seconds / 3 second ticks

    public SporeCloudItem(Mobile caster, Point3D loc, Map map) : base(0x113A) // Fog graphic
    {
        Movable = false;
        Name = "spore cloud";
        Hue = 0x7D6;
        m_Caster = caster;

        MoveToWorld(loc, map);

        m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(3.0), TimeSpan.FromSeconds(3.0), m_Ticks, OnTick);
    }

    private void OnTick()
    {
        if (Deleted || Map == null)
            return;

        IPooledEnumerable eable = Map.GetMobilesInRange(Location, 6);
        foreach (Mobile m in eable)
        {
            if (m.Alive && m_Caster.CanBeHarmful(m))
            {
                // Poison damage
                int damage = Utility.RandomMinMax(5, 9);
                AOS.Damage(m, m_Caster, damage, 0, 0, 0, 100, 0);

                // Apply accuracy debuff (simulated via dex reduction)
                StatMod blinded = new StatMod(StatType.Dex, "SporeCloud_Blind", -25, TimeSpan.FromSeconds(3.0));
                m.AddStatMod(blinded);
            }
        }
        eable.Free();

        if (--m_Ticks == 0)
            Delete();
    }
}
```

#### 6. Bleed Effects
**Spells:** Wolf Form attacks

**Implementation:** Similar to poison DoT, applied on melee hits
```csharp
// In WeaponAbility or OnHit handler for Wolf Form
public static void ApplyBleed(Mobile attacker, Mobile defender)
{
    if (defender.HasGump(typeof(BleedGump))) // Already bleeding
        return;

    defender.SendMessage("You are bleeding!");
    defender.SendGump(new BleedGump());

    int ticks = 5;
    Timer.DelayCall(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0), ticks, () =>
    {
        if (defender != null && !defender.Deleted && defender.Alive)
        {
            int damage = Utility.RandomMinMax(3, 7);
            AOS.Damage(defender, attacker, damage, 100, 0, 0, 0, 0); // Physical damage
            defender.FixedParticles(0x377A, 1, 15, 9502, 67, 7, EffectLayer.Waist);
        }
    });
}
```

---

### ⚠️ Advanced Mechanics (Complex Implementation)

These require extensive custom systems and careful balancing.

#### 1. Flying Movement (Hawk Form)
**Challenge:** ServUO pathfinding doesn't natively support "flying over obstacles"

**Implementation Option 1: Ignore Terrain Checks**
```csharp
// Override Mobile.CheckMovement
public override bool CheckMovement(Direction d, out int newZ)
{
    if (ShapeshiftManager.GetContext(this)?.Form.CanFly == true)
    {
        // Allow movement through obstacles
        newZ = Z; // Maintain current Z
        return true;
    }
    return base.CheckMovement(d, out newZ);
}
```

**Implementation Option 2: Increased Z-level**
```csharp
// When entering Hawk Form, increase Z by 10
public void EnterHawkForm()
{
    m_OriginalZ = Z;
    Z += 10; // "Fly" 10 tiles up
    // Apply other form bonuses
}

public void ExitHawkForm()
{
    Z = m_OriginalZ; // Return to ground
}
```

**Note:** Flying over obstacles requires modifying core movement code or using custom map layer.

#### 2. Force of Nature (Multi-Form Hybrid)
**Challenge:** Rapidly cycle through forms every 5 seconds while allowing spellcasting

**Implementation:** Custom state manager with form rotation
```csharp
public class ForceOfNatureContext
{
    private Mobile m_Mobile;
    private int m_CurrentFormIndex = 0;
    private Timer m_SwitchTimer;
    private ShapeshiftForm[] m_Forms = { ShapeshiftForm.Bear, ShapeshiftForm.Wolf, ShapeshiftForm.Hawk, ShapeshiftForm.Treant };

    public ForceOfNatureContext(Mobile m, TimeSpan duration)
    {
        m_Mobile = m;

        // Apply all bonuses (cumulative)
        ApplyAllBonuses();

        // Start form cycling
        m_SwitchTimer = Timer.DelayCall(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(5.0), 6, SwitchForm);

        // End after duration
        Timer.DelayCall(duration, End);
    }

    private void ApplyAllBonuses()
    {
        // Apply cumulative bonuses from all forms
        // STR: Bear +30, Treant +50 = +80 total
        // DEX: Wolf +25, Hawk +40 = +65 total
        // etc.
    }

    private void SwitchForm()
    {
        // Cycle body appearance
        m_Mobile.BodyValue = m_Forms[m_CurrentFormIndex].BodyID;
        m_CurrentFormIndex = (m_CurrentFormIndex + 1) % m_Forms.Length;

        // Visual effect
        m_Mobile.FixedParticles(0x376A, 9, 32, 5008, EffectLayer.Waist);
    }

    private void End()
    {
        m_SwitchTimer?.Stop();
        // Remove all bonuses
        // Restore original body
    }
}
```

#### 3. Avatar of the Forest (Shapeshift + Spellcasting)
**Challenge:** Override shapeshift spell lockout while maintaining form bonuses

**Implementation:** Set special flag to allow casting
```csharp
public void CastAvatarOfTheForest()
{
    // Apply avatar bonuses
    // ...

    // Allow shapeshifting without spell lockout
    Mobile.SetProperty("CanCastWhileShifted", true);

    // Summon 3 treant guardians
    for (int i = 0; i < 3; i++)
    {
        TreantGuardian treant = new TreantGuardian();
        treant.Controlled = true;
        treant.ControlMaster = Caster;
        treant.MoveToWorld(GetSpawnLocation(), Caster.Map);
    }
}

// In spell check:
if (Caster.m_MagicLocked && !Caster.GetProperty("CanCastWhileShifted"))
{
    // Cannot cast
}
```

#### 4. World Tree's Embrace (Zone Transformation)
**Challenge:** Create large zone with dual effects (buff allies, damage enemies)

**Implementation:** Large custom item with dual aura effects
```csharp
public class WorldTreeItem : Item
{
    private Mobile m_Caster;
    private Timer m_AuraTimer;
    private int m_Ticks = 15; // 45 seconds / 3 second ticks

    public WorldTreeItem(Mobile caster, Point3D loc, Map map) : base(0x0CCA)
    {
        Movable = false;
        Name = "World Tree";
        m_Caster = caster;

        // Scale up the visual (if possible via client mods)
        MoveToWorld(loc, map);

        m_AuraTimer = Timer.DelayCall(TimeSpan.FromSeconds(3.0), TimeSpan.FromSeconds(3.0), m_Ticks, ProcessAura);
    }

    private void ProcessAura()
    {
        if (Deleted || Map == null)
            return;

        IPooledEnumerable eable = Map.GetMobilesInRange(Location, 12);
        foreach (Mobile m in eable)
        {
            if (!m.Alive)
                continue;

            if (m == m_Caster || m.CanBeBeneficial(m_Caster))
            {
                // Buff allies
                int heal = Utility.RandomMinMax(8, 12);
                m.Hits = Math.Min(m.Hits + heal, (int)(m.HitsMax * 1.5)); // +50% max HP

                // Apply resistance buffs (if not already applied)
                // ...
            }
            else if (m_Caster.CanBeHarmful(m))
            {
                // Damage enemies
                int damage = Utility.RandomMinMax(8, 15);
                AOS.Damage(m, m_Caster, damage, 0, 0, 0, 100, 0);

                // Apply slow
                StatMod slow = new StatMod(StatType.Dex, "WorldTree_Slow", -50, TimeSpan.FromSeconds(3.0));
                m.AddStatMod(slow);
            }
        }
        eable.Free();

        if (--m_Ticks == 0)
            Delete();
    }
}
```

#### 5. Triple Attack (Hydra Form)
**Challenge:** Make melee attacks hit 3 times

**Implementation:** Custom OnHit handler
```csharp
// In WeaponAbility or Shapeshift context
public override void OnHit(Mobile attacker, Mobile defender, int damage)
{
    if (ShapeshiftManager.GetContext(attacker)?.Form.Type == ShapeshiftFormType.Hydra)
    {
        // First hit (normal)
        base.OnHit(attacker, defender, damage);

        // Second hit (66% damage)
        Timer.DelayCall(TimeSpan.FromSeconds(0.2), () =>
        {
            if (defender != null && !defender.Deleted && defender.Alive)
                AOS.Damage(defender, attacker, (int)(damage * 0.66), 100, 0, 0, 0, 0);
        });

        // Third hit (33% damage)
        Timer.DelayCall(TimeSpan.FromSeconds(0.4), () =>
        {
            if (defender != null && !defender.Deleted && defender.Alive)
                AOS.Damage(defender, attacker, (int)(damage * 0.33), 100, 0, 0, 0, 0);
        });
    }
    else
    {
        base.OnHit(attacker, defender, damage);
    }
}
```

---

### 📝 Implementation Checklist per Spell

#### 1. Basic Damage Spell (e.g., Thorn Dart)
- [ ] Create spell class inheriting from `MagerySpell`
- [ ] Define `SpellInfo` with name, mantra, action/effect IDs, reagents
- [ ] Set `Circle` property
- [ ] Implement `OnCast()` with target selection
- [ ] Calculate damage with `GetNewAosDamage()`
- [ ] Apply damage with `SpellHelper.Damage()` (set poison % for nature spells)
- [ ] Add visual/sound effects

#### 2. Healing Spell (e.g., Nature's Touch)
- [ ] Create spell class
- [ ] Implement heal calculation based on caster skill
- [ ] Cap healing at target's max HP
- [ ] Add beneficial checks (`CanBeBeneficial()`)
- [ ] Apply healing: `target.Hits += amount`
- [ ] Add visual effects (green sparkles)

#### 3. Shapeshift Spell (e.g., Bear Form)
- [ ] Create spell class
- [ ] Check if already shapeshifted (end previous form)
- [ ] Apply body change: `caster.BodyValue = formBodyID`
- [ ] Apply stat mods (STR/DEX bonuses)
- [ ] Apply resistance mods
- [ ] Set spell lockout flag
- [ ] Create timer for duration
- [ ] Add revert function to restore original state

#### 4. DoT Spell (e.g., Poison Spores)
- [ ] Create spell class
- [ ] Apply initial effect (visual/message)
- [ ] Create timer with repeated ticks
- [ ] Each tick: check target alive, deal damage
- [ ] Track total ticks, stop timer when complete
- [ ] Option: Use built-in `Poison` or custom Timer

#### 5. Zone/AoE Field Spell (e.g., Healing Grove)
- [ ] Create custom Item class for zone
- [ ] Place item at target location
- [ ] Create timer for repeated effects
- [ ] Each tick: `GetMobilesInRange()`, apply effect to allies/enemies
- [ ] Remember to call `eable.Free()`
- [ ] Delete item after duration

#### 6. Summon Spell (e.g., Summon Ancient Treant)
- [ ] Create custom `BaseCreature` class
- [ ] Set creature stats, skills, AI type
- [ ] In spell: instantiate creature
- [ ] Set `Controlled = true`, `ControlMaster = caster`
- [ ] Spawn near caster with path checking
- [ ] Create timer to delete after duration

---

### 🎯 Priority Implementation Order

#### Phase 1 - Foundation (Circles 1-2)
1. **Basic Damage:** Thorn Dart (single target)
2. **Basic Healing:** Nature's Touch (direct heal)
3. **First Buff:** Barkskin (simple resist buff)
4. **First Root:** Entangle (frozen = true)
5. **First DoT:** Poison Spores (poison or timer-based)

**Goal:** Test core spell framework and damage types

#### Phase 2 - Core Systems (Circles 3-4)
1. **Shapeshifting System:** Implement `ShapeshiftManager` and `ShapeshiftContext`
2. **First Shapeshift:** Bear Form (melee form with spell lockout)
3. **AoE Damage:** Thorn Volley (GetMobilesInRange)
4. **HoT Spell:** Rejuvenation (timer-based healing)
5. **Zone Creation:** Wild Growth (item-based terrain)

**Goal:** Establish shapeshifting and zone mechanics

#### Phase 3 - Advanced Mechanics (Circles 5-6)
1. **Advanced Forms:** Wolf Form (speed), Hawk Form (flying), Treant Form (tank)
2. **Zone Effects:** Healing Grove (AoE HoT item)
3. **Debuff Zones:** Spore Cloud (damage + accuracy debuff)
4. **Earthquake:** Knockdown + barrier destruction
5. **First Summon:** Summon Ancient Treant (BaseCreature)

**Goal:** Complete shapeshift variety and zone control

#### Phase 4 - Ultimate Spells (Circles 7-8)
1. **Plague:** Spreading DoT mechanic
2. **Force of Nature:** Multi-form hybrid
3. **Avatar of the Forest:** Shapeshift + spellcasting
4. **World Tree's Embrace:** Zone transformation
5. **Hydra Form:** Triple attack mechanic

**Goal:** Implement complex ultimate abilities

---

## Legacy Implementation Notes

### Shapeshifting System:
- Use `BodyMod` and `BodyValue` properties
- Apply `StatMod` for stat bonuses
- Apply `ResistanceMod` for resistances
- Lock spellcasting via flag: `m_Shapeshifted = true`
- Revert on duration end or manual cancel

### Poison Mechanics:
- Use ServUO's `Poison` class (Lesser, Regular, Greater, Deadly, Lethal)
- Apply via `target.ApplyPoison(caster, Poison.Regular)`
- Custom DoT via Timer with damage ticks

### Healing Mechanics:
- Use `target.Hits += amount`
- Cap at `target.HitsMax`
- HoT via Timer.DelayCall with repeated healing

---

## Balance Considerations

**PvE:**
- Versatile: can heal, tank (shapeshifted), or DPS
- Excellent sustained damage via DoTs
- Good group utility with AoE heals
- Strong against natural enemies

**PvP:**
- High skill cap (form management)
- Vulnerable while shapeshifted (no spells)
- Strong anti-melee with roots
- Countered by fire mages and dispels

**Synergies:**
- Hybrid Playstyle: Cast DoTs → Shapeshift → Melee → Revert → Heal
- Zone Control: Wild Growth → Healing Grove → Swarm
- Poison Combo: Poison Spores → Toxic Bloom → Plague
- Tank Mode: Barkskin → Living Fortress → Treant Form

---

**Last Updated:** 2025-12-05
**Status:** Design Complete - 0/32 Implemented
**Next Steps:** Implement shapeshifting system and Circle 1-2 spells
