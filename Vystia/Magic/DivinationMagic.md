# Divination Magic - Crystal Barrens School

**Class:** Oracle
**Region:** Crystal Barrens
**Theme:** Foresight, crystal magic, energy, time manipulation, fate
**Spellbook:** Crystal Codex (OracleSpellbook.cs)
**Primary Stat:** Intelligence
**Hue:** 0x482 (Crystal Blue)

**Implementation Status:** ✅ **32/32 spells implemented (100% COMPLETE - ready for testing)**

> **📝 Reagent Note:** Individual spell entries below use placeholder UO reagent names for design reference. The actual ServUO implementation uses custom Vystia reagents as documented in the [Reagents](#reagents) section at the end of this file. See `DivinationMagicReagents.cs` for implementation.

---

## Overview

Divination Magic harnesses crystalline energy and foresight, specializing in buffs, shields, energy damage, and time manipulation. Oracles support allies with powerful buffs while dealing prismatic energy damage.

**Strengths:**
- Powerful buffs and shields
- Can see future (predict attacks)
- Energy damage (ignores most armor)
- Time manipulation (haste, slow)
- Party utility

**Weaknesses:**
- Lower direct damage
- Mana intensive
- Requires positioning
- Vulnerable when channeling

---

## Spell List (32 Spells)

### Circle 1 (4 mana)
1. **Crystal Dart** - 5-11 energy damage
2. **Glimpse Future** - See enemy's next action
3. **Minor Ward** - +10 damage shield
4. **Clarity** - +5 INT for 30s

### Circle 2 (6 mana)
5. **Prismatic Bolt** - 10-17 energy damage, rainbow effect
6. **Foresight** - Next attack against you misses
7. **Crystal Shield** - 30 damage absorb shield
8. **Haste Self** - +20% attack/cast speed for 20s

### Circle 3 (9 mana)
9. **Energy Burst** - 3 tile AoE, 15-26 energy damage
10. **Precognition** - See invisible/hidden, detect traps
11. **Temporal Slow** - Slow enemy 40% for 12s
12. **Mana Crystal** - Creates mana crystal, restore 25 mana when used

### Circle 4 (11 mana)
13. **Prismatic Spray** - Cone, 18-32 damage, random status effects
14. **Time Warp** - Rewind to position 3s ago, restore HP/mana to that state
15. **Barrier of Light** - Party shield, absorbs 50 damage each
16. **Oracle's Sight** - See all enemies within 25 tiles, their HP/mana

### Circle 5 (14 mana)
17. **Crystal Barrage** - 5 tile AoE, 25-42 energy damage
18. **Fate's Thread** - Link two allies, share buffs and damage
19. **Mass Haste** - Party +25% speed for 30s
20. **Temporal Shield** - When hit, rewind damage (immune for 1s after hit)

### Circle 6 (20 mana)
21. **Prismatic Storm** - 8 tile radius, 35-58 energy damage, rainbow explosions
22. **Prophecy of Doom** - Mark enemy, allies deal +30% damage to them
23. **Crystal Fortress** - AoE shield, 200 HP absorb, reflects damage
24. **Time Stop** - Freeze enemy for 5s (complete disable)

### Circle 7 (40 mana)
25. **Cosmic Rift** - Creates energy rift, 12 tiles, pulls enemies, 55-90 damage
26. **Fate Shift** - Swap HP/mana values with target
27. **Mass Foresight** - Party gains "avoid next attack" buff
28. **Chrono Lord** - Control time: rewind cooldowns, double spell speed

### Circle 8 (50 mana)
29. **Prismatic Apocalypse** - Screen-wide, 80-145 energy damage, random status effects
30. **Crystal Maze** - Traps enemies in crystal labyrinth (20 tiles, 45s)
31. **Timeless State** - Party becomes immune to damage for 10s
32. **Oracle Ascendant** - See all futures: auto-dodge attacks, instant cast spells, perfect accuracy

---

## Foresight Mechanics
- Reveals enemy actions 1-3 seconds early
- Allows pre-emptive dodging/countering
- Higher circle spells = longer foresight window

## Crystal Energy
- Energy damage ignores physical armor
- Deals reduced damage to energy-resistant targets
- Can create mana crystals that persist

## Time Manipulation
- Rewind: Restore previous state
- Time Stop: Complete disable
- Haste: Speed buffs for party
- Slow: Reduce enemy speed

## Reagents

Divination Magic uses the following Vystia-specific reagents organized by spell circle:

### Primary Reagents (6 types)
1. **Crystal Dust** (Circles 1-3) - Ground crystal powder from crystal fields
2. **Prism Shard** (Circles 2-4) - Rainbow crystal from light refraction zones
3. **Starlight Crystal** (Circles 1-4) - Glowing crystal from stargazing peaks
4. **Ley Line Shard** (Circles 5-7) - Magic nexus crystal from ley line intersections
5. **Time Sand** (Circles 6-8) - Temporal hourglass sand from time rifts
6. **Crystal Ore** (Circles 3-6) - Raw crystal (existing resource, also used as reagent)

### Rare Reagents (2 types)
7. **Prismatic Shard** (Circles 7-8) - Perfect crystal (existing resource)
8. **Fate Crystal** (Circle 8) - Prophecy crystal, ultimate reagent

**Implementation:** All reagents created in `DivinationMagicReagents.cs`

---

## ServUO Implementation Notes

### ✅ Standard Mechanics
- **Energy Damage:** `SpellHelper.Damage(spell, target, damage, 0, 0, 0, 0, 100)` (100% energy)
- **Stat Buffs:** `StatMod(StatType.Int, "Clarity", bonus, duration)`
- **Damage Shields:** Custom buff tracking absorbed damage
- **Haste:** Increase DEX with `StatMod`, apply swing speed modifier

### 🔧 Custom Mechanics

**1. Foresight (Dodge Next Attack)**
```csharp
public static class ForesightManager
{
    private static HashSet<Mobile> m_Protected = new HashSet<Mobile>();

    public static void ApplyForesight(Mobile m)
    {
        m_Protected.Add(m);
        m.SendMessage("You foresee the next attack!");
    }

    public static bool CheckForesight(Mobile m)
    {
        if (m_Protected.Contains(m))
        {
            m_Protected.Remove(m);
            m.SendMessage("You dodge the attack!");
            return true; // Dodge
        }
        return false;
    }
}

// In OnDamage hook: if (ForesightManager.CheckForesight(defender)) return 0;
```

**2. Crystal Shield (Damage Absorb)**
```csharp
public static class CrystalShieldManager
{
    private static Dictionary<Mobile, int> m_Shields = new Dictionary<Mobile, int>();

    public static void ApplyShield(Mobile m, int amount)
    {
        m_Shields[m] = amount;
    }

    public static int AbsorbDamage(Mobile m, int damage)
    {
        if (m_Shields.ContainsKey(m) && m_Shields[m] > 0)
        {
            int absorbed = Math.Min(damage, m_Shields[m]);
            m_Shields[m] -= absorbed;
            if (m_Shields[m] <= 0)
                m_Shields.Remove(m);
            return damage - absorbed;
        }
        return damage;
    }
}
```

**3. Time Warp (Rewind Position/Stats)**
```csharp
public class TimeWarpSnapshot
{
    public Point3D Location;
    public int HP;
    public int Mana;
}

public static class TimeWarpManager
{
    private static Dictionary<Mobile, TimeWarpSnapshot> m_Snapshots = new Dictionary<Mobile, TimeWarpSnapshot>();

    public static void TakeSnapshot(Mobile m)
    {
        m_Snapshots[m] = new TimeWarpSnapshot
        {
            Location = m.Location,
            HP = m.Hits,
            Mana = m.Mana
        };
    }

    public static void Rewind(Mobile m)
    {
        if (m_Snapshots.ContainsKey(m))
        {
            TimeWarpSnapshot snap = m_Snapshots[m];
            m.MoveToWorld(snap.Location, m.Map);
            m.Hits = snap.HP;
            m.Mana = snap.Mana;
            m.FixedParticles(0x376A, 9, 32, 5008, EffectLayer.Waist);
        }
    }
}

// Time Warp spell:
TimeWarpManager.TakeSnapshot(Caster); // Store state
Timer.DelayCall(TimeSpan.FromSeconds(3.0), () => TimeWarpManager.Rewind(Caster)); // Auto-rewind
```

**4. Time Stop (Complete Disable)**
```csharp
public static class TimeStopManager
{
    public static void FreezeTime(Mobile target, TimeSpan duration)
    {
        target.Paralyzed = true;
        target.Frozen = true;
        target.SendMessage("Time has stopped!");

        // Visual: crystal effect
        target.FixedParticles(0x376A, 1, 15, 9909, 0x482, 0, EffectLayer.Waist);

        Timer.DelayCall(duration, () => {
            target.Paralyzed = false;
            target.Frozen = false;
        });
    }
}
```

**5. Oracle's Sight (Reveal All)**
```csharp
IPooledEnumerable eable = Caster.Map.GetMobilesInRange(Caster.Location, 25);
foreach (Mobile m in eable)
{
    if (m.Hidden)
        m.RevealingAction();

    Caster.SendMessage($"{m.Name}: {m.Hits}/{m.HitsMax} HP, {m.Mana}/{m.ManaMax} Mana");
    // Show floating text overhead
    m.PublicOverheadMessage(MessageType.Regular, 0x482, false, $"HP: {m.Hits}/{m.HitsMax}");
}
eable.Free();
```

**6. Fate's Thread (Link Two Allies)**
```csharp
public class FatesThreadContext
{
    public Mobile Ally1, Ally2;

    public void ShareBuff(Mobile from, StatMod mod)
    {
        Mobile other = (from == Ally1) ? Ally2 : Ally1;
        other.AddStatMod(new StatMod(mod.Type, mod.Name + "_Shared", mod.Offset, mod.Duration));
    }

    public void ShareDamage(Mobile damaged, int amount)
    {
        Mobile other = (damaged == Ally1) ? Ally2 : Ally1;
        int shared = amount / 2;
        AOS.Damage(other, null, shared, 100, 0, 0, 0, 0);
    }
}
```

### ⚠️ Advanced Mechanics
- **Timeless State (Immunity):** Track buff, intercept all damage for 10s
- **Crystal Maze:** Create maze of blocking Items, pathfinding challenges
- **Oracle Ascendant:** Auto-dodge all attacks, instant cast (modify OnCast timing)

### 📝 Implementation Priority
**Phase 1:** Crystal Dart, Clarity, Minor Ward, Foresight
**Phase 2:** Crystal Shield system, Haste, Time Warp
**Phase 3:** Time Stop, Oracle's Sight, Fate's Thread
**Phase 4:** Prismatic Storm, Timeless State, Oracle Ascendant

**Last Updated:** 2025-12-05 | **Status:** Design Complete - 0/32 Implemented
