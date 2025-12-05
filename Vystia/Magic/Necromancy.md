# Necromancy - ShadowVoid School

**Class:** Necromancer
**Region:** ShadowVoid
**Theme:** Death, undead, bone, decay, soul manipulation
**Spellbook:** Necronomicon (NecromancerSpellbook.cs)
**Primary Stat:** Intelligence
**Hue:** 0x455 (Deathly Black/Green)

---

## Overview

Necromancy controls death and undead, specializing in raising skeletons, bone magic, decay, and soul manipulation. Necromancers command undead armies and drain life force.

**Strengths:**
- Undead summons (permanent until killed)
- Life drain and death magic
- Corpse explosion combos
- Immune to poison/disease
- Army of minions

**Weaknesses:**
- Weak vs. holy magic
- Summons can be dispelled
- Requires corpses for some spells
- Fragile (low HP)

---

## Spell List (32 Spells)

### Circle 1 (4 mana)
1. **Death Bolt** - 5-12 decay damage
2. **Animate Bone** - Raise skeleton from corpse (150 HP)
3. **Life Siphon** - Drain 8-15 HP, heal self
4. **Deathsight** - See in darkness, detect undead

### Circle 2 (6 mana)
5. **Bone Shard** - 10-18 damage, pierces armor
6. **Decay** - DoT 4-8 decay damage/tick, reduces max HP
7. **Raise Zombie** - Raise zombie from corpse (300 HP, slow, tanky)
8. **Soul Shield** - Absorb 25 damage, heals when broken

### Circle 3 (9 mana)
9. **Death Coil** - 20-32 damage to living OR heal undead ally
10. **Bone Armor** - +12 Physical Resist, bone spikes reflect damage
11. **Mass Raise** - Raise 3 skeletons from nearby corpses
12. **Soul Harvest** - AoE drain, 10-18 HP from each enemy

### Circle 4 (11 mana)
13. **Skeletal Mage** - Raise skeletal mage from corpse (400 HP, casts death bolts)
14. **Corpse Explosion** - Detonate corpse, 25-45 AoE damage
15. **Death Grip** - Pull enemy to you, deal 15-25 damage
16. **Vampiric Touch** - Drain 20-35 HP, heal for 150%

### Circle 5 (14 mana)
17. **Bone Wall** - Create wall of bones (blocks movement, 150 HP)
18. **Death and Decay** - Ground AoE, 8-14 decay damage/tick for 12s
19. **Raise Bone Golem** - Summon bone construct (900 HP, high damage, taunts)
20. **Soul Link** - Link souls with ally, share damage/healing

### Circle 6 (20 mana)
21. **Death Knights** - Summon 2 death knights (700 HP each, mounted, powerful)
22. **Plague Cloud** - 8 tile AoE, decay + poison, spreads
23. **Unholy Frenzy** - Undead allies +50% attack speed, take damage over time
24. **Lich Form** - Transform: immune to poison, +30% necro damage, undead

### Circle 7 (40 mana)
25. **Army of the Dead** - Summon 10 undead warriors (varied types)
26. **Death Wave** - Screen-wide cone, 50-85 decay damage, raises corpses
27. **Bone Prison** - Trap enemy in bones, 10s disable, 300 HP to break
28. **Demi-Lich Transformation** - Ultimate lich: flying, +50% power, immune to physical

### Circle 8 (50 mana)
29. **Apocalypse of Death** - Screen-wide, 80-140 damage, raises all corpses
30. **Summon Undead Dragon** - Legendary summon (3000 HP, breath attack, flying)
31. **Death's Door** - All enemies reduced to 1 HP (resisted by high stats)
32. **Archlich Ascension** - Become archlich: phylactery (revive on death), control all undead, life drain aura

---

## Undead Summon Types
1. **Skeleton** - Basic melee (Circle 1)
2. **Zombie** - Tank, slow (Circle 2)
3. **Skeletal Mage** - Caster (Circle 4)
4. **Bone Golem** - Elite tank (Circle 5)
5. **Death Knight** - Elite cavalry (Circle 6)
6. **Undead Dragon** - Legendary (Circle 8)

## Corpse Mechanics
- Many spells require corpses nearby
- Corpses can be exploded for AoE damage
- Limit: 5 active summons at once
- Permanent summons (until killed)

## Reagents

Necromancy uses the following Vystia-specific reagents organized by spell circle:

### Primary Reagents (6 types)
1. **Grave Moss** (Circles 1-3) - Graveyard fungus from crypts
2. **Bone Dust** (Circles 2-4) - Ground bones from ossuaries
3. **Death Shroud** (Circles 1-4) - Burial cloth from tombs
4. **Soul Fragment** (Circles 3-6) - Captured soul essence from death temples
5. **Corpse Ash** (Circles 6-8) - Cremated remains from crematoriums
6. **Voidstone Ore** (Circles 5-7) - Death-touched stone (existing resource)

### Rare Reagents (2 types)
7. **Echoing Shard** (Circles 7-8) - Captured death echo (existing resource)
8. **Lich Dust** (Circle 8) - Ancient lich remains, ultimate reagent

**Implementation:** All reagents created in `NecromancyReagents.cs`

---

## ServUO Implementation Notes

### ✅ Standard Mechanics
- **Decay Damage:** Mixed poison/physical `SpellHelper.Damage(spell, target, damage, 50, 0, 0, 50, 0)`
- **Life Drain:** See Hex Magic LifeDrainHelper implementation
- **Summons:** `BaseCreature` with permanent control (until killed)

### 🔧 Custom Mechanics

**1. Corpse-Based Summons**
```csharp
public void RaiseSkeleton()
{
    Corpse nearbyCorpse = FindNearestCorpse(Caster, 5);
    if (nearbyCorpse != null)
    {
        SkeletonSummon skeleton = new SkeletonSummon();
        skeleton.Controlled = true;
        skeleton.ControlMaster = Caster;
        skeleton.MoveToWorld(nearbyCorpse.Location, nearbyCorpse.Map);

        nearbyCorpse.Delete(); // Consume corpse
        NecromancerSummonManager.AddSummon(Caster, skeleton);
    }
}

private Corpse FindNearestCorpse(Mobile from, int range)
{
    IPooledEnumerable eable = from.Map.GetItemsInRange(from.Location, range);
    Corpse closest = null;
    foreach (Item item in eable)
    {
        if (item is Corpse corpse && corpse.Owner != null)
        {
            closest = corpse;
            break;
        }
    }
    eable.Free();
    return closest;
}
```

**2. Corpse Explosion**
```csharp
public void CorpseExplosion(Corpse corpse)
{
    Point3D loc = corpse.Location;
    Map map = corpse.Map;

    // Delete corpse
    corpse.Delete();

    // AoE explosion
    Effects.SendLocationEffect(loc, map, 0x36BD, 20, 10);

    IPooledEnumerable eable = map.GetMobilesInRange(loc, 4);
    foreach (Mobile m in eable)
    {
        if (m.Alive && Caster.CanBeHarmful(m))
        {
            int damage = Utility.RandomMinMax(25, 45);
            AOS.Damage(m, Caster, damage, 50, 0, 0, 50, 0);
        }
    }
    eable.Free();
}
```

**3. Summon Limit Management**
```csharp
public static class NecromancerSummonManager
{
    private static Dictionary<Mobile, List<BaseCreature>> m_Summons = new Dictionary<Mobile, List<BaseCreature>>();
    private const int MAX_SUMMONS = 5;

    public static bool CanSummon(Mobile necro)
    {
        if (!m_Summons.ContainsKey(necro))
            m_Summons[necro] = new List<BaseCreature>();

        // Clean up dead summons
        m_Summons[necro].RemoveAll(s => s == null || s.Deleted);

        return m_Summons[necro].Count < MAX_SUMMONS;
    }

    public static void AddSummon(Mobile necro, BaseCreature summon)
    {
        if (!m_Summons.ContainsKey(necro))
            m_Summons[necro] = new List<BaseCreature>();

        m_Summons[necro].Add(summon);
    }
}
```

**4. Lich Form (Transformation)**
```csharp
Caster.BodyValue = 0x0018; // Lich body
Caster.Hue = 0x455;

// Immunity to poison
ResistanceMod poisonImmu = new ResistanceMod(ResistanceType.Poison, 100);
Caster.AddResistanceMod(poisonImmu);

// Bonus necro damage
NecromancerBuffManager.ApplyLichForm(Caster, duration, 0.30); // +30%
```

**5. Death and Decay (Ground DoT)**
```csharp
public class DeathAndDecayItem : Item
{
    private Mobile m_Caster;
    private Timer m_Timer;

    public DeathAndDecayItem(Mobile caster, Point3D loc, Map map) : base(0x3915) // Decay graphic
    {
        Movable = false;
        Hue = 0x455;
        m_Caster = caster;
        MoveToWorld(loc, map);

        m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0), 6, OnTick);
    }

    private void OnTick()
    {
        IPooledEnumerable eable = Map.GetMobilesInRange(Location, 5);
        foreach (Mobile m in eable)
        {
            if (m.Alive && m_Caster.CanBeHarmful(m))
            {
                int damage = Utility.RandomMinMax(8, 14);
                AOS.Damage(m, m_Caster, damage, 50, 0, 0, 50, 0);
            }
        }
        eable.Free();
    }
}
```

**6. Army of the Dead**
```csharp
// Summon 10 varied undead
int[] undeadTypes = { 0, 1, 2, 0, 1, 0, 2, 0, 1, 0 }; // Mix of skeleton, zombie, mage
for (int i = 0; i < 10; i++)
{
    BaseCreature undead = CreateUndeadByType(undeadTypes[i]);
    undead.Controlled = true;
    undead.ControlMaster = Caster;
    undead.MoveToWorld(GetSpawnLocation(Caster, 5), Caster.Map);
}
```

### ⚠️ Advanced Mechanics
- **Bone Prison:** Create bone wall Items around target (8 tiles), targetable with HP
- **Death's Door:** Reduce all enemies to 1 HP (high resist check)
- **Archlich Phylactery:** Create persistent Item, respawn on death from phylactery location

### 📝 Implementation Priority
**Phase 1:** Death Bolt, Animate Bone, Life Siphon
**Phase 2:** Corpse Explosion, Summon Limit system, Raise Zombie
**Phase 3:** Death and Decay, Lich Form, Mass Raise
**Phase 4:** Army of the Dead, Bone Prison, Archlich Ascension

**Last Updated:** 2025-12-05 | **Status:** Design Complete - 0/32 Implemented
