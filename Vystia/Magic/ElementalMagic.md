# Elemental Magic - Emberlands School

**Class:** Sorcerer
**Region:** Emberlands
**Theme:** Fire/lava, earth/magma, combined elemental fury
**Spellbook:** Tome of Elemental Fury (SorcererSpellbook.cs)
**Primary Stat:** Intelligence
**Hue:** 0x54E (Fiery Orange)

---

## Overview

Elemental Magic harnesses the volcanic fury of the Emberlands, specializing in fire damage, lava terrain, and explosive combinations. Sorcerers are pure damage dealers who excel at burning groups and creating hazardous terrain.

**Strengths:**
- Highest burst damage potential
- Excellent AoE capabilities
- Terrain hazards (lava, magma)
- Strong vs. ice/nature
- Ignites enemies for bonus damage

**Weaknesses:**
- Fragile (low defenses)
- Vulnerable to water/ice
- High mana costs
- Limited CC options

---

## Spell List (32 Spells)

### Circle 1 (4 mana)
1. **Flame Bolt** - 5-12 fire damage
2. **Molten Touch** - Melee fire attack, ignites
3. **Heat Shield** - +5 Fire Resist for 30s
4. **Smoke Screen** - Creates obscuring smoke, -20% accuracy

### Circle 2 (6 mana)
5. **Fireball** - 10-18 fire damage, small AoE (2 tiles)
6. **Lava Puddle** - Ground effect, 4-7 damage/tick to walkers
7. **Ember Burst** - Short range cone, 8-15 damage
8. **Flame Ward** - Reflects 10 fire damage on melee hits

### Circle 3 (9 mana)
9. **Incinerate** - 20-35 fire damage, high single target
10. **Volcanic Rock** - Hurl boulder, 15-25 damage, stuns 1s
11. **Ring of Fire** - Circle around caster, 3-6 damage/tick, blocks movement
12. **Ignite** - Applies burning DoT (5-10/tick, 10s), spreads to nearby

### Circle 4 (11 mana)
13. **Flame Pillar** - Ground target, 18-30 damage, knockup
14. **Magma Armor** - +15 Physical Resist, returns fire damage on hits
15. **Pyroclasm** - 4 tile radius AoE, 20-35 damage
16. **Meteor Strike** - Falling meteor, 25-40 damage, leaves lava

### Circle 5 (14 mana)
17. **Inferno** - 6 tile radius, 15-28 damage/tick for 8s
18. **Lava Flow** - Creates flowing lava stream (8 tiles long)
19. **Combustion** - Detonates burning targets, 30-50 damage
20. **Phoenix Shield** - When taking fatal damage, revive at 30% HP (once)

### Circle 6 (20 mana)
21. **Hellfire Nova** - 8 tile radius, 35-60 damage, ignites all
22. **Molten Titan Form** - Transform: +40 STR, +30 Fire Resist, immune to ignite
23. **Volcanic Eruption** - Ground target, massive AoE (10 tiles), 40-70 damage
24. **Flame Tempest** - Tornado of fire, moves randomly, 8-15 damage/tick

### Circle 7 (40 mana)
25. **Cataclysm** - Screen-wide AoE, 50-85 damage, creates lava everywhere
26. **Summon Fire Elemental Lord** - Powerful summon (1500 HP, immune to fire)
27. **Pyroclastic Flow** - Massive cone (15 tiles), 60-100 damage, lethal ignite
28. **Avatar of Flame** - Ultimate transformation: +100% fire damage, immune to fire, mana costs halved

### Circle 8 (50 mana)
29. **Apocalypse** - Entire screen, 80-140 damage, burns for 20-35/tick
30. **Magma Core** - Creates permanent lava zone (20 tiles), 15-25 damage/tick
31. **Solar Flare** - Blinds all enemies, 100-160 damage, disarms for 10s
32. **Primordial Inferno** - Ultimate DoT: 25-40/tick for 30s, cannot be removed

---

## Reagent Usage
- **Primary:** SulfurousAsh (28), MandrakeRoot (18), BlackPearl (15)
- **Secondary:** Bloodmoss (12), Nightshade (8)
- **Special:** MoltenOre (3 spells), EverburningCoal (2 spells)

---

## Key Mechanics
- **Ignite Status:** Burning DoT that spreads to nearby enemies
- **Lava Terrain:** Persists on ground, damages over time
- **Combustion Combos:** Ignite enemies → Detonate for massive damage
- **Heat Stack:** Multiple fire spells on same target increase damage

---

## Reagents

Elemental Magic uses the following Vystia-specific reagents organized by spell circle:

### Primary Reagents (6 types)
1. **Ash Petal** (Circles 1-3) - Volcanic flower from volcanic slopes
2. **Lava Glass** (Circles 2-4) - Cooled obsidian shard from lava flows
3. **Flameweed** (Circles 1-4) - Fire-resistant plant from volcanic hot springs
4. **Magma Essence** (Circles 3-6) - Bottled lava energy from volcanic chambers
5. **Phoenix Feather** (Circles 6-8) - Rare fire bird feather from phoenix nests
6. **Molten Ore** (Circles 5-7) - Superheated metal (existing resource)

### Rare Reagents (2 types)
7. **Everburning Coal** (Circles 7-8) - Eternal flame coal (existing resource)
8. **Primordial Ember** (Circle 8) - First fire spark, ultimate reagent

**Implementation:** All reagents created in `ElementalMagicReagents.cs`

---

## ServUO Implementation Notes

### ✅ Standard Mechanics
- **Direct Fire Damage:** Use `SpellHelper.Damage(spell, target, damage, 0, 100, 0, 0, 0)` (100% fire)
- **AoE Fire:** `map.GetMobilesInRange()` with fire damage distribution
- **Fire Resistance Buffs:** `ResistanceMod(ResistanceType.Fire, bonus)`
- **Stuns:** `target.Paralyzed = true` with timed removal

### 🔧 Custom Mechanics

**1. Ignite Status (Spreading DoT)**
```csharp
public static class IgniteManager
{
    private static Dictionary<Mobile, IgniteContext> m_Ignited = new Dictionary<Mobile, IgniteContext>();

    public static void Ignite(Mobile caster, Mobile target, int baseDamage, TimeSpan duration, bool spreads = false)
    {
        if (m_Ignited.ContainsKey(target))
            m_Ignited[target].Refresh(baseDamage); // Stack/refresh
        else
            m_Ignited[target] = new IgniteContext(caster, target, baseDamage, duration, spreads);
    }
}

public class IgniteContext
{
    private void OnTick()
    {
        // Deal DoT damage
        AOS.Damage(Target, Caster, Utility.RandomMinMax(5, 10), 0, 100, 0, 0, 0);

        // Spread to nearby if enabled
        if (m_Spreads)
        {
            IPooledEnumerable eable = Target.Map.GetMobilesInRange(Target.Location, 3);
            foreach (Mobile m in eable)
            {
                if (m != Target && !IgniteManager.IsIgnited(m) && Utility.RandomDouble() < 0.3)
                    IgniteManager.Ignite(Caster, m, m_BaseDamage / 2, TimeSpan.FromSeconds(5.0), false);
            }
            eable.Free();
        }
    }
}
```

**2. Lava Terrain (Ground Hazard)**
```csharp
public class LavaPuddleItem : Item
{
    private Mobile m_Caster;
    private Timer m_Timer;

    public LavaPuddleItem(Mobile caster, Point3D loc, Map map, TimeSpan duration) : base(0x122A) // Lava graphic
    {
        Movable = false;
        Hue = 0x54E;
        m_Caster = caster;
        MoveToWorld(loc, map);

        m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0), (int)duration.TotalSeconds, OnTick);
    }

    private void OnTick()
    {
        IPooledEnumerable eable = Map.GetMobilesInRange(Location, 1);
        foreach (Mobile m in eable)
        {
            if (m.Alive && m_Caster.CanBeHarmful(m))
                AOS.Damage(m, m_Caster, Utility.RandomMinMax(4, 7), 0, 100, 0, 0, 0);
        }
        eable.Free();
    }
}
```

**3. Combustion (Detonate Ignited Targets)**
```csharp
public void CastCombustion(Mobile target)
{
    if (IgniteManager.IsIgnited(target))
    {
        // Bonus damage for ignited targets
        int damage = Utility.RandomMinMax(30, 50);
        int ignitedBonus = IgniteManager.GetStackCount(target) * 10; // +10 per stack

        AOS.Damage(target, Caster, damage + ignitedBonus, 0, 100, 0, 0, 0);
        IgniteManager.RemoveIgnite(target); // Consume ignite

        // AoE explosion
        Effects.SendLocationEffect(target.Location, target.Map, 0x36BD, 20, 10);
    }
}
```

**4. Molten Titan Form (Transformation)**
```csharp
// Apply body change, stat bonuses, fire immunity
target.BodyValue = 0x0010; // Fire elemental body
StatMod strBonus = new StatMod(StatType.Str, "MoltenTitan_Str", 40, duration);
ResistanceMod fireMod = new ResistanceMod(ResistanceType.Fire, 30);
target.AddStatMod(strBonus);
target.AddResistanceMod(fireMod);

// Immunity to ignite
IgniteManager.SetImmune(target, true);
```

**5. Lava Flow (Directional Stream)**
```csharp
public void CreateLavaFlow(Point3D start, Direction direction, int length)
{
    for (int i = 0; i < length; i++)
    {
        Point3D loc = GetPointInDirection(start, direction, i);
        LavaPuddleItem lava = new LavaPuddleItem(Caster, loc, Caster.Map, TimeSpan.FromSeconds(20.0));
    }
}
```

### ⚠️ Advanced Mechanics
- **Flame Tempest (Moving AoE):** Custom Item that changes location every 2s
- **Phoenix Shield (Revive on Death):** Hook into `OnDamaged`, check for fatal damage, revive at 30% HP
- **Avatar of Flame:** Track transformation state, halve all fire spell mana costs, apply damage multiplier
- **Magma Core (Permanent Zone):** Long-duration lava zone (600s+), use persistent Item

### 📝 Implementation Priority
**Phase 1:** Flame Bolt, Fireball, Heat Shield
**Phase 2:** Ignite system, Lava Puddle, Incinerate
**Phase 3:** Ring of Fire, Meteor Strike, Pyroclasm
**Phase 4:** Inferno, Cataclysm, Avatar of Flame

**Last Updated:** 2025-12-05 | **Status:** Design Complete - 0/32 Implemented
