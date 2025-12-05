# Bardic Magic - Multi-Regional School

**Class:** Bard
**Region:** Multi-Regional
**Theme:** Songs, sonic damage, buffs, crowd control, inspiration
**Spellbook:** Songbook of Legends (BardSpellbook.cs)
**Primary Stat:** Intelligence / Charisma
**Hue:** 0x8A5 (Golden/Musical)

---

## Overview

Bardic Magic uses music and song to inspire allies, damage enemies, and control the battlefield. Bards are versatile support casters who excel at party buffs and sonic AoE damage.

**Strengths:**
- Powerful party-wide buffs
- Sonic damage (bypasses physical armor)
- AoE crowd control
- Songs persist (channeled effects)
- Versatile (damage, heal, buff, CC)

**Weaknesses:**
- Must channel songs (can't move/cast while singing)
- Vulnerable while performing
- Lower single-target damage
- Mana intensive for songs

---

## Spell List (32 Spells)

### Circle 1 (4 mana)
1. **Discordant Note** - 5-11 sonic damage
2. **Song of Courage** - Allies +5 to all stats (30s)
3. **Lullaby** - Enemy becomes drowsy, -20% attack speed (20s)
4. **Inspire Accuracy** - Ally +10% accuracy (30s)

### Circle 2 (6 mana)
5. **Sonic Burst** - 10-18 sonic damage, 2 tile AoE
6. **Song of Resilience** - Allies +10 Physical Resist (45s)
7. **Dirge of Weakness** - Enemy -10 STR (25s)
8. **Rejuvenating Melody** - Ally heals 3-5 HP/tick for 30s

### Circle 3 (9 mana)
9. **Thunderous Chord** - 18-30 sonic damage, stuns 2s
10. **War Song** - CHANNELED: Allies +15% damage while singing
11. **Mesmerize** - Enemy charmed for 8s (attacks allies)
12. **Song of Swiftness** - Allies +25% movement speed (45s)

### Circle 4 (11 mana)
13. **Sonic Wave** - Cone, 20-35 sonic damage, pushes enemies back
14. **Ballad of Health** - CHANNELED: Heals 6-10 HP/tick to allies
15. **Cacophony** - AoE confusion, enemies attack randomly (10s)
16. **Inspire Heroism** - Target ally +30% damage for 30s

### Circle 5 (14 mana)
17. **Shattering Scream** - 28-48 sonic damage, destroys barriers/shields
18. **Song of Sanctuary** - CHANNELED: Allies take 30% less damage
19. **Requiem** - DoT, 8-14 sonic damage/tick, silences (12s)
20. **Mass Inspire** - All allies +15% attack speed (45s)

### Circle 6 (20 mana)
21. **Destructive Resonance** - 8 tile AoE, 35-60 sonic damage, shatters equipment
22. **Epic Ballad** - CHANNELED: Allies +25% all stats, +20 all resists
23. **Song of Fear** - AoE, all enemies flee in terror (8s)
24. **Crescendo** - Channeled AoE damage, ramps up: 5→10→15→20→25/tick

### Circle 7 (40 mana)
25. **Symphony of Destruction** - 12 tile AoE, 50-85 sonic damage, AoE stun 4s
26. **Song of the Ancients** - CHANNELED: Massive party buff (+40% damage, +30 all resists, +5 HP regen)
27. **Sonic Apocalypse** - Screen-wide, 45-75 sonic damage, mass confusion
28. **Legendary Performance** - Transform: All songs can be cast while moving/casting (60s)

### Circle 8 (50 mana)
29. **Ode to Devastation** - Screen-wide, 80-140 sonic damage, destroys all barriers/summons
30. **Eternal Symphony** - CHANNELED: Ultimate buff (party unkillable while channeling, max 15s)
31. **Voice of the Dragon** - Massive cone (20 tiles), 100-170 sonic damage, permanent deafen
32. **Maestro Ascendant** - Ultimate form: All songs cost no mana, can channel 3 songs simultaneously

---

## Song Channeling System

**Channeled Songs:**
- Must maintain channel (can't move or cast other spells)
- Lasts as long as you channel (or until mana runs out)
- Costs mana per tick (5 mana/tick)
- If interrupted, song ends
- Channeled songs: War Song, Ballad of Health, Song of Sanctuary, Epic Ballad, Song of the Ancients, Eternal Symphony

**Instant Songs:**
- Cast instantly, duration-based
- Can stack multiple instant songs
- Do not prevent movement/casting

## Sonic Damage
- Bypasses physical armor
- Effective against heavily armored enemies
- Can shatter barriers, shields, equipment
- Deaf status prevents some effects

## Performance Buffs
**Song Types:**
1. **Battle Songs:** Damage buffs (War Song, Inspire Heroism)
2. **Protective Songs:** Defense buffs (Resilience, Sanctuary)
3. **Healing Songs:** HoT effects (Rejuvenating Melody, Ballad of Health)
4. **Debuff Songs:** Enemy weakening (Dirge, Requiem)
5. **Epic Songs:** Ultimate buffs (Epic Ballad, Song of the Ancients)

## Reagents

Bardic Magic uses the following Vystia-specific reagents organized by spell circle:

### Primary Reagents (6 types)
1. **Song Petal** (Circles 1-3) - Musical flower from performance halls
2. **Echo Dust** (Circles 2-4) - Resonance powder from echo chambers
3. **Voice Crystal** (Circles 1-4) - Sound-storing crystal from crystal caves
4. **Golden String** (Circles 3-6) - Enchanted instrument string from master luthiers
5. **Harmony Gem** (Circles 5-7) - Perfectly tuned gemstone from tuning forks
6. **Muse Essence** (Circles 6-8) - Bottled inspiration from legendary performances

### Rare Reagents (2 types)
7. **Dragon Scale** (Circles 7-8) - Legendary voice scale from dragons
8. **Eternal Note** (Circle 8) - Perfect musical tone, ultimate reagent

**Implementation:** All reagents created in `BardicMagicReagents.cs`

---

## ServUO Implementation Notes

### ✅ Standard Mechanics
- **Sonic Damage:** Energy damage `SpellHelper.Damage(spell, target, damage, 0, 0, 0, 0, 100)`
- **Party Buffs:** Loop through party members, apply `StatMod` or `ResistanceMod`
- **Fear/Charm:** Use `target.Frozen = true` + direction manipulation for fear, `Controlled = true` for charm

### 🔧 Custom Mechanics

**1. Channeled Songs System**
```csharp
public class ChanneledSongContext
{
    public Mobile Bard;
    public string SongName;
    public Timer ManaTimer;
    public Timer EffectTimer;
    public int ManaPerTick = 5;

    public ChanneledSongContext(Mobile bard, string songName, Action effectTick)
    {
        Bard = bard;
        SongName = songName;

        // Mana drain every 2 seconds
        ManaTimer = Timer.DelayCall(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0), () =>
        {
            if (Bard.Mana >= ManaPerTick)
                Bard.Mana -= ManaPerTick;
            else
                EndSong(); // Out of mana
        });

        // Effect tick every 2 seconds
        EffectTimer = Timer.DelayCall(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0), () => effectTick());
    }

    public void EndSong()
    {
        ManaTimer?.Stop();
        EffectTimer?.Stop();
        ChanneledSongManager.RemoveSong(Bard);
        Bard.SendMessage($"You stop singing {SongName}.");
    }
}

public static class ChanneledSongManager
{
    private static Dictionary<Mobile, ChanneledSongContext> m_Songs = new Dictionary<Mobile, ChanneledSongContext>();

    public static void StartSong(Mobile bard, string songName, Action effectTick)
    {
        // End current song if any
        if (m_Songs.ContainsKey(bard))
            m_Songs[bard].EndSong();

        ChanneledSongContext song = new ChanneledSongContext(bard, songName, effectTick);
        m_Songs[bard] = song;
    }

    public static void RemoveSong(Mobile bard)
    {
        if (m_Songs.ContainsKey(bard))
            m_Songs.Remove(bard);
    }

    public static bool IsChanneling(Mobile bard)
    {
        return m_Songs.ContainsKey(bard);
    }
}

// Prevent movement/casting while channeling
// In spell CheckCast():
if (ChanneledSongManager.IsChanneling(Caster))
{
    Caster.SendMessage("You cannot cast while channeling a song!");
    return false;
}
```

**2. War Song (Channeled Damage Buff)**
```csharp
public void CastWarSong()
{
    ChanneledSongManager.StartSong(Caster, "War Song", () =>
    {
        // Every tick, buff nearby allies
        IPooledEnumerable eable = Caster.Map.GetMobilesInRange(Caster.Location, 8);
        foreach (Mobile m in eable)
        {
            if (m.Alive && (m == Caster || m.CanBeBeneficial(Caster)))
            {
                // Apply 15% damage buff (tracked separately)
                WarSongManager.ApplyBuff(m, 0.15, TimeSpan.FromSeconds(3.0));
                m.FixedParticles(0x373A, 1, 15, 9909, 0x8A5, 0, EffectLayer.Waist);
            }
        }
        eable.Free();
    });
}
```

**3. Sonic Damage with Equipment Shattering**
```csharp
public void CastDestructiveResonance(Mobile target)
{
    int damage = Utility.RandomMinMax(35, 60);
    AOS.Damage(target, Caster, damage, 0, 0, 0, 0, 100); // Sonic damage

    // Chance to destroy equipment
    if (Utility.RandomDouble() < 0.2) // 20% chance
    {
        Item equipped = GetRandomEquippedItem(target);
        if (equipped != null && equipped.LootType != LootType.Blessed)
        {
            target.SendMessage($"Your {equipped.Name} shatters from the sonic resonance!");
            equipped.Delete();
        }
    }
}

private Item GetRandomEquippedItem(Mobile m)
{
    List<Item> items = new List<Item>();
    for (int i = 0; i < m.Items.Count; i++)
    {
        Item item = m.Items[i];
        if (item.Layer != Layer.Bank && item.Layer != Layer.Backpack)
            items.Add(item);
    }
    return items.Count > 0 ? items[Utility.Random(items.Count)] : null;
}
```

**4. Crescendo (Ramping Damage)**
```csharp
public class CrescendoContext
{
    private Mobile m_Caster;
    private int m_CurrentTick = 0;
    private int[] m_DamagePerTick = { 5, 10, 15, 20, 25 };
    private Timer m_Timer;

    public CrescendoContext(Mobile caster)
    {
        m_Caster = caster;
        m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0), m_DamagePerTick.Length, OnTick);
    }

    private void OnTick()
    {
        int damage = m_DamagePerTick[m_CurrentTick];

        IPooledEnumerable eable = m_Caster.Map.GetMobilesInRange(m_Caster.Location, 8);
        foreach (Mobile m in eable)
        {
            if (m.Alive && m_Caster.CanBeHarmful(m))
            {
                AOS.Damage(m, m_Caster, damage, 0, 0, 0, 0, 100);
                m.PublicOverheadMessage(MessageType.Regular, 0x8A5, false, damage.ToString());
            }
        }
        eable.Free();

        m_CurrentTick++;
    }
}
```

**5. Mesmerize/Charm**
```csharp
public void Mesmerize(Mobile target, TimeSpan duration)
{
    if (target is PlayerMobile)
    {
        // Players: confuse to attack randomly
        target.SendMessage("You are mesmerized and confused!");
        // Apply confusion status
    }
    else if (target is BaseCreature creature)
    {
        // NPCs: control temporarily
        Mobile originalMaster = creature.ControlMaster;
        creature.Controlled = true;
        creature.ControlMaster = Caster;
        creature.ControlOrder = OrderType.Attack;

        Timer.DelayCall(duration, () =>
        {
            creature.Controlled = false;
            creature.ControlMaster = null;
            creature.SendMessage("You break free from the charm!");
        });
    }
}
```

### ⚠️ Advanced Mechanics
- **Eternal Symphony (Unkillable):** Hook into damage, set all party HP to 1 minimum
- **Legendary Performance:** Allow movement/casting while channeling - remove checks
- **Maestro Ascendant:** Multiple simultaneous songs - track 3 separate ChanneledSongContexts

### 📝 Implementation Priority
**Phase 1:** Discordant Note, Song of Courage, Sonic Burst
**Phase 2:** War Song (channeled system), Rejuvenating Melody
**Phase 3:** Crescendo, Mesmerize, Destructive Resonance
**Phase 4:** Epic Ballad, Eternal Symphony, Maestro Ascendant

**Last Updated:** 2025-12-05 | **Status:** Design Complete - 0/32 Implemented
