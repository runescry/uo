# Shamanic Magic - Wilderlands/Multi-Regional School

**Class:** Shaman
**Region:** Wilderlands
**Theme:** Totems, spirits, elementals, ancestral magic, chain lightning
**Spellbook:** Tome of Spirits (ShamanSpellbook.cs)
**Primary Stat:** Intelligence / Wisdom
**Hue:** 0x6E0 (Earthy Brown/Blue)

**Implementation Status:** ✅ **32/32 spells implemented (100% COMPLETE - ready for testing)**

> **📝 Reagent Note:** Individual spell entries below use placeholder UO reagent names for design reference. The actual ServUO implementation uses custom Vystia reagents as documented in the [Reagents](#reagents) section at the end of this file. See `ShamanicMagicReagents.cs` for implementation.

---

## Overview

Shamanic Magic calls upon ancestral spirits and elemental totems, specializing in totem placement, chain spells, spirit wolves, and hybrid melee/caster combat.

**Strengths:**
- Totem placement for persistent effects
- Chain lightning spells
- Spirit summons
- Hybrid combat (can melee)
- Party buffs via totems

**Weaknesses:**
- Totems can be destroyed
- Limited to 4 active totems
- Positional (must stay near totems)
- Split scaling (INT and STR)

---

## Spell List (32 Spells)

### Circle 1 (4 mana)
1. **Lightning Bolt** - 6-12 lightning damage
2. **Strength Totem** - Place totem: Allies +10 STR in 8 tiles
3. **Ghost Wolf Form** - Transform: +20 DEX, +25% speed (60s)
4. **Healing Stream** - Summon: Heals allies 3-5 HP/tick

### Circle 2 (6 mana)
5. **Chain Lightning** - 12-20 damage, bounces to 2 additional targets
6. **Fire Totem** - Totem: Auto-attacks enemies for 5-10 fire damage
7. **Spirit Strike** - Melee attack: 10-18 damage + knockback
8. **Purification** - Remove poison, disease, curses from ally

### Circle 3 (9 mana)
9. **Lightning Storm** - 4 tile AoE, 15-28 lightning damage
10. **Earth Shield** - Shield absorbs 60 damage, regenerates 10/tick
11. **Totemic Recall** - Destroy all totems, restore 50% mana
12. **Summon Spirit Wolf** - Spirit companion (400 HP, fast, phases)

### Circle 4 (11 mana)
13. **Chain Heal** - Heal 25-40 HP, bounces to 3 allies
14. **Mana Spring Totem** - Totem: Restores 5 mana/tick to allies
15. **Lava Burst** - 30-50 fire damage, instant cast if Flame Shock active
16. **Flame Shock** - DoT 6-10 fire/tick, enables Lava Burst

### Circle 5 (14 mana)
17. **Thunderstorm Totem** - Totem: Chain lightning every 3s (8 tiles)
18. **Ancestral Spirit** - Become spirit: +30% dodge, phase through walls (30s)
19. **Earth Elemental** - Summon earth elemental tank (1000 HP)
20. **Maelstrom** - Ground AoE: Pulls enemies in, 10-18 damage/tick

### Circle 6 (20 mana)
21. **Mega Chain Lightning** - 35-60 damage, bounces to 5 targets
22. **Totem of Wrath** - Totem: +20% spell damage to all allies
23. **Spirit Link Totem** - Totem: Splits damage among allies (damage reduction)
24. **Elemental Fury** - Transform: Cast spells while meleeing (45s)

### Circle 7 (40 mana)
25. **Summon Greater Earth Elemental** - Elite elemental (2200 HP, AoE slam)
26. **Ancestor's Blessing** - Resurrect fallen ally at full HP/mana
27. **Four Totems** - Instantly place 4 totems (Fire, Earth, Water, Air)
28. **Ascendance** - Become air: Flying, +50% lightning damage, immune to melee

### Circle 8 (50 mana)
29. **Apocalyptic Chain Lightning** - 80-140 damage, bounces to ALL enemies
30. **Spirit of the Wild** - Summon pack of 5 spirit wolves
31. **Totem Army** - Place 8 totems simultaneously, max totems increased to 8
32. **Shaman Lord** - Ultimate: Totems are indestructible, instant totem placement, elemental mastery

---

## Totem System
- Max 4 totems active (increased by talents)
- Each totem lasts 60 seconds
- Totems are targetable (100 HP each)
- Types: Strength, Fire, Healing Stream, Mana Spring, Thunderstorm, Wrath, Spirit Link

## Chain Spell Mechanics
- Bounces to nearby enemies (5 tile range per bounce)
- Each bounce reduces damage by 10%
- Can't bounce to same target twice
- Affects: Lightning Bolt, Chain Lightning, Chain Heal

## Spirit Forms
- **Ghost Wolf:** Fast travel form
- **Ancestral Spirit:** Ghost form, dodge
- **Elemental Fury:** Hybrid combat
- **Ascendance:** Air elemental form

## Reagents

Shamanic Magic uses the following Vystia-specific reagents organized by spell circle:

### Primary Reagents (6 types)
1. **Thunder Moss** (Circles 1-3) - Storm-touched moss from highlands
2. **Wind Crystal** (Circles 2-4) - Solidified air from mountain peaks
3. **Spirit Feather** (Circles 1-4) - Ghost bird feather from totemic sites
4. **Lightning Root** (Circles 3-6) - Lightning-struck tree root
5. **Storm Essence** (Circles 5-7) - Bottled storm from storm vortexes
6. **Totem Carving** (Circles 6-8) - Enchanted wood carving from sacred totems

### Rare Reagents (2 types)
7. **Windstone Ore** (Circles 7-8) - Wind-carved stone from sky peaks
8. **Primal Thunder** (Circle 8) - Captured lightning essence, ultimate reagent

**Implementation:** All reagents created in `ShamanicMagicReagents.cs`

---

## ServUO Implementation Notes

### ✅ Standard Mechanics
- **Lightning Damage:** `SpellHelper.Damage(spell, target, damage, 0, 0, 0, 0, 100)` (100% energy)
- **Stat Totems:** Place Item that applies `StatMod` to allies in range
- **Chain Heals:** Loop through nearby allies, apply healing with bounce mechanic

### 🔧 Custom Mechanics

**1. Totem System (Max 4 Totems)**
```csharp
public abstract class TotemItem : Item
{
    protected Mobile m_Caster;
    protected Timer m_EffectTimer;
    protected int m_Duration = 60; // seconds

    public TotemItem(Mobile caster, int itemID) : base(itemID)
    {
        Movable = false;
        m_Caster = caster;
        Name = "totem";

        // Totems are targetable
        LootType = LootType.Blessed;

        m_EffectTimer = Timer.DelayCall(TimeSpan.FromSeconds(3.0), TimeSpan.FromSeconds(3.0), m_Duration / 3, OnTick);
        Timer.DelayCall(TimeSpan.FromSeconds(m_Duration), () => Delete());
    }

    protected abstract void OnTick();

    public override void OnAfterDelete()
    {
        m_EffectTimer?.Stop();
        TotemManager.RemoveTotem(m_Caster, this);
        base.OnAfterDelete();
    }
}

public static class TotemManager
{
    private static Dictionary<Mobile, List<TotemItem>> m_Totems = new Dictionary<Mobile, List<TotemItem>>();
    public static int MAX_TOTEMS = 4;

    public static bool CanPlaceTotem(Mobile m)
    {
        if (!m_Totems.ContainsKey(m))
            m_Totems[m] = new List<TotemItem>();

        m_Totems[m].RemoveAll(t => t == null || t.Deleted);
        return m_Totems[m].Count < MAX_TOTEMS;
    }

    public static void AddTotem(Mobile m, TotemItem totem)
    {
        if (!m_Totems.ContainsKey(m))
            m_Totems[m] = new List<TotemItem>();

        m_Totems[m].Add(totem);
    }
}
```

**2. Example Totems**
```csharp
public class StrengthTotem : TotemItem
{
    public StrengthTotem(Mobile caster) : base(caster, 0x0ED4)
    {
        Hue = 0x6E0;
        Name = "Strength Totem";
    }

    protected override void OnTick()
    {
        IPooledEnumerable eable = Map.GetMobilesInRange(Location, 8);
        foreach (Mobile m in eable)
        {
            if (m.Alive && (m == m_Caster || m.CanBeBeneficial(m_Caster)))
            {
                StatMod buff = new StatMod(StatType.Str, "StrengthTotem", 10, TimeSpan.FromSeconds(4.0));
                m.AddStatMod(buff);
            }
        }
        eable.Free();
    }
}

public class FireTotem : TotemItem
{
    public FireTotem(Mobile caster) : base(caster, 0x0ED4)
    {
        Hue = 0x54E;
        Name = "Fire Totem";
    }

    protected override void OnTick()
    {
        // Find nearest enemy
        Mobile target = FindNearestEnemy(Location, Map, 8);
        if (target != null)
        {
            int damage = Utility.RandomMinMax(5, 10);
            AOS.Damage(target, m_Caster, damage, 0, 100, 0, 0, 0);
            target.FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);
        }
    }
}
```

**3. Chain Lightning (Bounce Mechanic)**
```csharp
public void CastChainLightning(Mobile initialTarget, int bounces, int baseDamage)
{
    List<Mobile> hitTargets = new List<Mobile>();
    Mobile currentTarget = initialTarget;

    for (int i = 0; i < bounces && currentTarget != null; i++)
    {
        hitTargets.Add(currentTarget);

        // Deal damage (reduced per bounce)
        int damage = (int)(baseDamage * Math.Pow(0.9, i)); // -10% per bounce
        AOS.Damage(currentTarget, Caster, damage, 0, 0, 0, 0, 100);

        // Visual effect
        if (i > 0)
            currentTarget.MovingEffect(hitTargets[i - 1], 0x379F, 7, 0, false, false);

        // Find next target
        currentTarget = FindNearestEnemy(currentTarget.Location, currentTarget.Map, 5, hitTargets);
    }
}

private Mobile FindNearestEnemy(Point3D from, Map map, int range, List<Mobile> exclude)
{
    Mobile closest = null;
    double closestDist = 999;

    IPooledEnumerable eable = map.GetMobilesInRange(from, range);
    foreach (Mobile m in eable)
    {
        if (m.Alive && Caster.CanBeHarmful(m) && !exclude.Contains(m))
        {
            double dist = m.GetDistanceToSqrt(from);
            if (dist < closestDist)
            {
                closest = m;
                closestDist = dist;
            }
        }
    }
    eable.Free();

    return closest;
}
```

**4. Ghost Wolf Form**
```csharp
Caster.BodyValue = 0x00E1; // Wolf body
StatMod dexBuff = new StatMod(StatType.Dex, "GhostWolf", 20, duration);
Caster.AddStatMod(dexBuff);

// Speed buff (increase movement speed)
// Note: Movement speed modification requires custom code or DEX increase

Timer.DelayCall(duration, () =>
{
    Caster.BodyValue = Caster.Race.Body; // Restore
});
```

**5. Spirit Wolf (Phasing)**
```csharp
public class SpiritWolf : BaseCreature
{
    public SpiritWolf() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
    {
        Body = 0x00E1;
        Name = "Spirit Wolf";
        Hue = 0x8B0; // Ethereal hue

        SetStr(100, 120);
        SetDex(120, 140);
        SetHits(400);

        // Can phase through walls (requires custom pathfinding)
    }

    public override bool CanMoveThrough { get { return true; } } // Phase through obstacles
}
```

### ⚠️ Advanced Mechanics
- **Four Totems:** Instantly place 4 different totems, bypass placement time
- **Maelstrom:** Pull enemies toward center (gradual location changes each tick)
- **Shaman Lord:** Totems are indestructible (set HP to 999999), instant placement

### 📝 Implementation Priority
**Phase 1:** Lightning Bolt, Strength Totem, Ghost Wolf, Healing Stream
**Phase 2:** Chain Lightning, Fire Totem, Spirit Strike, Chain Heal
**Phase 3:** Totem system expansion, Mana Spring, Thunderstorm Totem
**Phase 4:** Four Totems, Mega Chain Lightning, Shaman Lord

**Last Updated:** 2025-12-05 | **Status:** Design Complete - 0/32 Implemented
