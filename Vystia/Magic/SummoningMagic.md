# Summoning Magic - Underwater/Multi-Regional School

**Class:** Summoner
**Region:** Underwater/Various
**Theme:** Creature summoning, pet buffs, elemental allies, binding
**Spellbook:** Codex of Binding (SummonerSpellbook.cs)
**Primary Stat:** Intelligence
**Hue:** 0x555 (Aqua Blue)

**Implementation Status:** ✅ **32/32 spells implemented (100% COMPLETE - ready for testing)**

> **📝 Reagent Note:** Individual spell entries below use placeholder UO reagent names for design reference. The actual ServUO implementation uses custom Vystia reagents as documented in the [Reagents](#reagents) section at the end of this file. See `SummoningMagicReagents.cs` for implementation.

---

## Overview

Summoning Magic binds creatures from all planes to serve the caster, specializing in summoning diverse creatures, buffing allies, and commanding multiple summons simultaneously.

**Strengths:**
- Multiple active summons (up to 5)
- Diverse creature types
- Buff summons for power
- Can summon elementals, beasts, demons
- Action economy (summons attack while you cast)

**Weaknesses:**
- Weak personal damage
- Summons can be killed/dispelled
- Mana intensive
- Requires positioning

---

## Spell List (32 Spells)

### Circle 1 (4 mana)
1. **Summon Rabbit** - Harmless scout (50 HP, fast, detects hidden)
2. **Arcane Bolt** - 4-10 damage (basic attack)
3. **Empower Summon** - +5 damage to one summon for 30s
4. **Summon Wisp** - Light source (100 HP, reveals area)

### Circle 2 (6 mana)
5. **Summon Wolf** - Combat pet (250 HP, fast attacks)
6. **Summon Fire Sprite** - Fire elemental (200 HP, ranged fire)
7. **Mend Summon** - Heal summon 30-50 HP
8. **Summon Shield** - Summon absorbs next 40 damage

### Circle 3 (9 mana)
9. **Summon Bear** - Tank pet (500 HP, high damage)
10. **Summon Air Elemental** - Flying elemental (350 HP, lightning)
11. **Mass Empower** - All summons +10 damage for 45s
12. **Bind Beast** - Charm beast enemy to fight for you (60s)

### Circle 4 (11 mana)
13. **Summon Drake** - Dragon-kin (600 HP, breath attack)
14. **Summon Earth Elemental** - Tank elemental (800 HP, high armor)
15. **Summon Frenzy** - All summons +40% attack speed for 20s
16. **Unsummon** - Dismiss summon, restore 50% mana cost

### Circle 5 (14 mana)
17. **Summon Hydra** - Multi-headed beast (900 HP, triple attack)
18. **Summon Storm Elemental** - AoE lightning elemental (650 HP)
19. **Greater Heal Summon** - Heal summon 80-120 HP
20. **Symbiotic Link** - Link to summon, share HP/mana pools

### Circle 6 (20 mana)
21. **Summon Phoenix** - Fire bird (1000 HP, resurrects once when killed)
22. **Summon Void Elemental** - Shadow elemental (850 HP, life drain)
23. **Army of Beasts** - Summon 3 wolves, 2 bears simultaneously
24. **Mass Heal Summons** - Heal all summons 50-80 HP each

### Circle 7 (40 mana)
25. **Summon Greater Dragon** - Powerful dragon (2000 HP, breath, flying)
26. **Summon Elemental Lord** - Choose element, legendary (1800 HP)
27. **Sacrifice Summon** - Kill summon, massive AoE damage (100-180)
28. **Swarm of Creatures** - Summon 10 weak creatures that swarm enemies

### Circle 8 (50 mana)
29. **Summon Titan** - Colossal giant (3500 HP, AoE stomp, knockback)
30. **Planar Convergence** - Summon one creature of each element (5 total)
31. **Summoner's Apocalypse** - All summons explode for 80-140 damage each, then resummon at full HP
32. **Avatar of Summoning** - Ultimate form: Infinite summons, instant cast, summons are invulnerable

---

## Summon Categories

**Beasts:**
- Rabbit, Wolf, Bear, Drake, Hydra, Phoenix

**Elementals:**
- Fire Sprite, Air, Earth, Storm, Void, Elemental Lord

**Legendary:**
- Greater Dragon, Titan, Planar beings

## Summon Management
- Max 5 summons active (increased by talents)
- Summons persist until dismissed/killed
- Can buff, heal, and empower summons
- Sacrifice summons for explosive damage

## Reagents

Summoning Magic uses the following Vystia-specific reagents organized by spell circle:

### Primary Reagents (6 types)
1. **Kelp Strand** (Circles 1-3) - Deep sea kelp from kelp forests
2. **Coral Fragment** (Circles 2-4) - Magical coral from coral reefs
3. **Sea Glass** (Circles 1-4) - Polished ocean glass from ocean floors
4. **Leviathan Tooth** (Circles 6-8) - Sea monster tooth from deep trenches
5. **Siren Scale** (Circles 3-6) - Siren fish scale (existing resource)
6. **Abyssal Pearl** (Circles 5-7) - Deep ocean pearl (existing resource)

### Rare Reagents (2 types)
7. **Deepwater Ore** (Circles 7-8) - Oceanic mineral (existing resource)
8. **Kraken Ink** (Circle 8) - Legendary squid ink, ultimate reagent

**Implementation:** All reagents created in `SummoningMagicReagents.cs`

---

## ServUO Implementation Notes

### ✅ Standard Mechanics
- **Summons:** `BaseCreature` with `Controlled = true`, `ControlMaster = caster`
- **Summon Buffs:** Apply `StatMod` or `ResistanceMod` to controlled creatures
- **Heal Summon:** `summon.Hits += healAmount` (cap at HitsMax)

### 🔧 Custom Mechanics

**1. Multiple Summon Management (Max 5)**
```csharp
public static class SummonerManager
{
    private static Dictionary<Mobile, List<BaseCreature>> m_Summons = new Dictionary<Mobile, List<BaseCreature>>();
    public static int MAX_SUMMONS = 5;

    public static bool CanSummon(Mobile m)
    {
        if (!m_Summons.ContainsKey(m))
            m_Summons[m] = new List<BaseCreature>();

        m_Summons[m].RemoveAll(s => s == null || s.Deleted);
        return m_Summons[m].Count < MAX_SUMMONS;
    }

    public static void AddSummon(Mobile m, BaseCreature summon)
    {
        if (!m_Summons.ContainsKey(m))
            m_Summons[m] = new List<BaseCreature>();

        m_Summons[m].Add(summon);
    }

    public static List<BaseCreature> GetSummons(Mobile m)
    {
        if (!m_Summons.ContainsKey(m))
            return new List<BaseCreature>();

        m_Summons[m].RemoveAll(s => s == null || s.Deleted);
        return m_Summons[m];
    }
}
```

**2. Mass Empower (Buff All Summons)**
```csharp
public void EmpowerAllSummons()
{
    List<BaseCreature> summons = SummonerManager.GetSummons(Caster);
    foreach (BaseCreature summon in summons)
    {
        StatMod damageBuff = new StatMod(StatType.Str, "Empower", 10, TimeSpan.FromSeconds(45.0));
        summon.AddStatMod(damageBuff);
        summon.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
    }
}
```

**3. Unsummon (Dismiss for Mana Return)**
```csharp
public void Unsummon(BaseCreature summon)
{
    if (summon.ControlMaster == Caster)
    {
        // Return 50% mana
        int manaReturn = 10; // Depends on summon circle
        Caster.Mana = Math.Min(Caster.Mana + manaReturn, Caster.ManaMax);

        summon.Delete();
        SummonerManager.GetSummons(Caster).Remove(summon);
    }
}
```

**4. Symbiotic Link (Share HP/Mana)**
```csharp
public class SymbioticLinkContext
{
    public Mobile Summoner;
    public BaseCreature Summon;

    public void ShareDamage(Mobile damaged, int amount)
    {
        Mobile other = (damaged == Summoner) ? (Mobile)Summon : Summoner;
        int shared = amount / 2;

        // Share damage
        if (other != null && !other.Deleted && other.Alive)
        {
            other.Hits = Math.Max(1, other.Hits - shared);
        }
    }

    public void ShareHealing(Mobile healed, int amount)
    {
        Mobile other = (healed == Summoner) ? (Mobile)Summon : Summoner;
        if (other != null && !other.Deleted)
        {
            other.Hits = Math.Min(other.Hits + amount, other.HitsMax);
        }
    }
}
```

**5. Phoenix Summon (Resurrects Once)**
```csharp
public class PhoenixSummon : BaseCreature
{
    private bool m_HasResurrected = false;

    public override void OnDeath(Container c)
    {
        if (!m_HasResurrected)
        {
            m_HasResurrected = true;

            Timer.DelayCall(TimeSpan.FromSeconds(3.0), () =>
            {
                if (!Deleted)
                {
                    Resurrect();
                    Hits = HitsMax;
                    FixedParticles(0x3709, 10, 30, 5052, EffectLayer.LeftFoot);
                    SendMessage("The Phoenix rises from the ashes!");
                }
            });
        }
        else
        {
            base.OnDeath(c);
        }
    }
}
```

**6. Sacrifice Summon (Explosive Damage)**
```csharp
public void SacrificeSummon(BaseCreature summon)
{
    Point3D loc = summon.Location;
    Map map = summon.Map;

    // Massive explosion
    Effects.SendLocationEffect(loc, map, 0x36BD, 30, 10);

    IPooledEnumerable eable = map.GetMobilesInRange(loc, 6);
    foreach (Mobile m in eable)
    {
        if (m.Alive && Caster.CanBeHarmful(m))
        {
            int damage = Utility.RandomMinMax(100, 180);
            AOS.Damage(m, Caster, damage, 50, 50, 0, 0, 0);
        }
    }
    eable.Free();

    summon.Delete();
}
```

### ⚠️ Advanced Mechanics
- **Army of Beasts:** Summon 5 creatures simultaneously, manage summon cap
- **Planar Convergence:** Summon one of each element (5 total), different AIs
- **Avatar of Summoning:** Infinite summons - remove cap check, instant cast, invulnerability

### 📝 Implementation Priority
**Phase 1:** Arcane Bolt, Summon Wolf, Empower Summon, Mend Summon
**Phase 2:** Summon Limit system, Mass Empower, Bear/Drake summons
**Phase 3:** Symbiotic Link, Phoenix summon, Hydra
**Phase 4:** Army of Beasts, Sacrifice Summon, Avatar of Summoning

**Last Updated:** 2025-12-05 | **Status:** Design Complete - 0/32 Implemented
