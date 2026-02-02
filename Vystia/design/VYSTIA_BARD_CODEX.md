# VYSTIA BARD SYSTEM CODEX

## Overview

You are implementing the Bard class for Vystia, a custom Ultima Online shard running on ServUO. The Bard uses a **Songweaving** skill to cast songs that buff allies, debuff enemies, and control combat. The system has three interlocking resources:

1. **Songweaving Skill (0-120)** - Determines success chance and song access
2. **Concentration (0-100)** - Limits how many songs can be active simultaneously  
3. **Crescendo (0-20)** - Builds during combat, spent on powerful Finales

## Core Constraints

### ServUO Technical Constraints
- Songs use the existing Barding skill check system (Musicianship + specific skill)
- Effects use standard `ResistanceMod`, `StatMod`, `Timer`, and `AOS.Damage`
- No custom client graphics - use existing particle effects and sounds
- Songs are targeted abilities, not channeled states
- All songs share a **7-second global cooldown**

### Balance Constraints
- Bard should be **support-focused**, not a one-man army
- Multiple bards should not stack the same buff infinitely
- Specialization (via Song Mastery) should be rewarded over generalization
- Control abilities (Provoke, Peace) should have meaningful costs

---

## Resource: Concentration

Concentration represents the Bard's mental focus. Songs **reserve** Concentration while active. When a song expires, is cancelled, or is broken, the Concentration returns.

```csharp
// PlayerMobile extension
public int Concentration { get; set; } = 100;
public int MaxConcentration => 100;

public Dictionary<Song, int> ActiveSongConcentration = new Dictionary<Song, int>();

public bool CanCastSong(int cost)
{
    return (Concentration - GetReservedConcentration()) >= cost;
}

public int GetReservedConcentration()
{
    return ActiveSongConcentration.Values.Sum();
}
```

### Concentration Costs by Song

| Song | Base Cost | Per Duration Mastery Level |
|------|-----------|---------------------------|
| Provocation | 35 | -2 per level (min 19 at 8) |
| Peacemaking | 30 | -2 per level (min 14 at 8) |
| Discordance | 30 | -2 per level (min 14 at 8) |
| Courage | 25 | -2 per level (min 9 at 8) |
| Swiftness | 25 | -2 per level (min 9 at 8) |
| Mending | 25 | -2 per level (min 9 at 8) |
| Requiem | 20 | -2 per level (min 4 at 8) |
| Light | 0 | — |
| Fortune | 0 | — |

### Concentration Formula

```csharp
public int GetSongConcentrationCost(SongType song, PlayerMobile bard)
{
    int baseCost = GetBaseCost(song);
    int durationMastery = bard.GetSongMasteryLevel(song, MasteryType.Duration);
    int reduction = durationMastery * 2;
    
    return Math.Max(baseCost - reduction, baseCost / 2); // Floor at 50% base
}
```

### Concentration Recovery

- Concentration is **not** regenerated over time while songs are active
- When a song expires or is cancelled, its reserved Concentration returns immediately
- If all songs end, full Concentration is restored after 10 seconds out of combat

---

## Resource: Crescendo

Crescendo is the Bard's combat momentum. It builds from successful song casts and is spent on Finales.

```csharp
public int Crescendo { get; set; } = 0;
public int MaxCrescendo => 20;
```

### Crescendo Generation

| Action | Crescendo Gained |
|--------|------------------|
| Successful Control song (Provoke, Peace) | +2 |
| Successful Debuff song (Discord, Requiem) | +2 |
| Successful Buff song (Courage, Swiftness) | +1 |
| Successful Heal song (Mending) | +1 |
| Successful Utility song (Light, Fortune) | +1 |
| Song failure | +0 |

### Crescendo Decay

```csharp
// Timer running every 10 seconds
public void CrescendoDecayTick()
{
    if (!InCombat && Crescendo > 0)
    {
        Crescendo = Math.Max(0, Crescendo - 1);
    }
}
```

### Crescendo Thresholds for Finales

| Crescendo | Available Finales |
|-----------|-------------------|
| 5+ | Minor Finales (Coda, Interlude, Sharp Note) |
| 10+ | Standard Finales (Crescendo Burst, Harmonize, Rallying Anthem) |
| 15+ | Major Finales (Fortissimo, Lullaby, Battle Hymn) |
| 20 | Magnum Opus (Symphony of Destruction, Eternal Refrain, etc.) |

---

## Song Implementation Pattern

### Base Song Class

```csharp
public abstract class BaseSong
{
    public abstract SongType Type { get; }
    public abstract int BaseDuration { get; } // seconds
    public abstract int BaseConcentrationCost { get; }
    public abstract int CrescendoGain { get; }
    public abstract int MinSongweaving { get; }
    
    public virtual TimeSpan Cooldown => TimeSpan.FromSeconds(7);
    
    public bool TryCast(PlayerMobile bard, Mobile target)
    {
        // Check skill requirement
        if (bard.Skills[SkillName.Musicianship].Value < MinSongweaving)
        {
            bard.SendMessage("You lack the skill to play this song.");
            return false;
        }
        
        // Check global cooldown
        if (DateTime.UtcNow < bard.NextSongTime)
        {
            bard.SendMessage("You must wait before playing another song.");
            return false;
        }
        
        // Check concentration
        int cost = GetConcentrationCost(bard);
        if (!bard.CanCastSong(cost))
        {
            bard.SendMessage("You cannot maintain focus on another song.");
            return false;
        }
        
        // Musicianship check
        if (!CheckMusicianship(bard))
        {
            bard.SendMessage("You fumble the notes.");
            bard.NextSongTime = DateTime.UtcNow + TimeSpan.FromSeconds(2);
            return false;
        }
        
        // Song-specific skill check (difficulty based on target)
        if (!CheckSongSkill(bard, target))
        {
            bard.SendMessage("Your song fails to take hold.");
            bard.NextSongTime = DateTime.UtcNow + TimeSpan.FromSeconds(2);
            return false;
        }
        
        // Success - apply effect
        ApplyEffect(bard, target);
        
        // Reserve concentration
        bard.ActiveSongConcentration[this.Type] = cost;
        
        // Grant crescendo
        bard.Crescendo = Math.Min(bard.MaxCrescendo, bard.Crescendo + CrescendoGain);
        
        // Set cooldown
        bard.NextSongTime = DateTime.UtcNow + Cooldown;
        
        // Schedule expiration
        int duration = GetDuration(bard);
        Timer.DelayCall(TimeSpan.FromSeconds(duration), () => OnSongExpire(bard, target));
        
        return true;
    }
    
    protected abstract void ApplyEffect(PlayerMobile bard, Mobile target);
    protected abstract void RemoveEffect(PlayerMobile bard, Mobile target);
    
    protected virtual void OnSongExpire(PlayerMobile bard, Mobile target)
    {
        RemoveEffect(bard, target);
        
        // Return concentration
        if (bard.ActiveSongConcentration.ContainsKey(this.Type))
        {
            bard.ActiveSongConcentration.Remove(this.Type);
        }
    }
    
    protected int GetDuration(PlayerMobile bard)
    {
        int durationMastery = bard.GetSongMasteryLevel(Type, MasteryType.Duration);
        int virtuosoBonus = bard.VirtuosoLevel; // 0-8
        
        return BaseDuration + ((durationMastery + virtuosoBonus) * 2);
    }
    
    protected int GetConcentrationCost(PlayerMobile bard)
    {
        int durationMastery = bard.GetSongMasteryLevel(Type, MasteryType.Duration);
        int reduction = durationMastery * 2;
        
        return Math.Max(BaseConcentrationCost - reduction, BaseConcentrationCost / 2);
    }
}
```

### Example: Song of Courage

```csharp
public class SongOfCourage : BaseSong
{
    public override SongType Type => SongType.Courage;
    public override int BaseDuration => 30;
    public override int BaseConcentrationCost => 25;
    public override int CrescendoGain => 1;
    public override int MinSongweaving => 70;
    
    private Dictionary<Mobile, List<StatMod>> m_ActiveMods = new Dictionary<Mobile, List<StatMod>>();
    
    protected override void ApplyEffect(PlayerMobile bard, Mobile target)
    {
        // Calculate potency
        int potencyMastery = bard.GetSongMasteryLevel(Type, MasteryType.Potency);
        int virtuosoBonus = bard.VirtuosoLevel;
        int damageBonus = 5 + potencyMastery + virtuosoBonus; // 5% to 21%
        
        // Apply to party members in range
        List<StatMod> mods = new List<StatMod>();
        
        foreach (Mobile m in GetPartyInRange(bard, 8))
        {
            // Damage bonus implemented as Tactics bonus (affects damage calc)
            StatMod mod = new StatMod(StatType.Str, "SongOfCourage", damageBonus, TimeSpan.Zero);
            m.AddStatMod(mod);
            mods.Add(mod);
            
            m.FixedParticles(0x375A, 10, 15, 5017, EffectLayer.Waist);
            m.SendMessage($"You feel emboldened! (+{damageBonus}% damage)");
        }
        
        m_ActiveMods[bard] = mods;
        
        bard.PlaySound(0x58B); // Lute sound
    }
    
    protected override void RemoveEffect(PlayerMobile bard, Mobile target)
    {
        if (m_ActiveMods.ContainsKey(bard))
        {
            foreach (var mod in m_ActiveMods[bard])
            {
                // Find and remove from affected mobiles
                // (tracking omitted for brevity)
            }
            m_ActiveMods.Remove(bard);
        }
    }
}
```

### Example: Song of Discordance

```csharp
public class SongOfDiscordance : BaseSong
{
    public override SongType Type => SongType.Discordance;
    public override int BaseDuration => 20;
    public override int BaseConcentrationCost => 30;
    public override int CrescendoGain => 2;
    public override int MinSongweaving => 50;
    
    protected override void ApplyEffect(PlayerMobile bard, Mobile target)
    {
        if (!(target is BaseCreature bc))
        {
            bard.SendMessage("You can only discord creatures.");
            return;
        }
        
        // Calculate effect strength
        int potencyMastery = bard.GetSongMasteryLevel(Type, MasteryType.Potency);
        int virtuosoBonus = bard.VirtuosoLevel;
        int effectPercent = 15 + potencyMastery + virtuosoBonus; // 15% to 31%
        
        // Apply debuffs
        // -X% damage dealt
        bc.DamageMin = (int)(bc.DamageMin * (1.0 - effectPercent / 100.0));
        bc.DamageMax = (int)(bc.DamageMax * (1.0 - effectPercent / 100.0));
        
        // +X% damage taken (via resistance reduction)
        for (int i = 0; i < 5; i++)
        {
            bc.AddResistanceMod(new ResistanceMod((ResistanceType)i, -effectPercent));
        }
        
        bc.FixedParticles(0x374A, 10, 15, 5028, EffectLayer.Waist);
        bard.SendMessage($"Your discordant notes weaken the creature! (-{effectPercent}% damage, +{effectPercent}% damage taken)");
        
        bard.PlaySound(0x58C); // Discordant sound
    }
    
    protected override void RemoveEffect(PlayerMobile bard, Mobile target)
    {
        // Restore original values (requires tracking - omitted for brevity)
    }
}
```

---

## Stacking Rules

### Same Song, Multiple Bards

When multiple bards apply the same song type to one target, only the **strongest** takes effect:

```csharp
public bool CanApplySong(Mobile target, SongType type, PlayerMobile newBard)
{
    // Check if target already has this song type
    if (target.HasActiveSong(type))
    {
        var existingSong = target.GetActiveSong(type);
        var existingBard = existingSong.Caster;
        
        // Compare potency mastery
        int existingPotency = existingBard.GetSongMasteryLevel(type, MasteryType.Potency);
        int newPotency = newBard.GetSongMasteryLevel(type, MasteryType.Potency);
        
        if (newPotency > existingPotency)
        {
            // New song is stronger - replace
            existingSong.Cancel();
            return true;
        }
        else if (newPotency == existingPotency)
        {
            // Same potency - compare skill
            if (newBard.Skills[SkillName.Musicianship].Value > existingBard.Skills[SkillName.Musicianship].Value)
            {
                existingSong.Cancel();
                return true;
            }
        }
        
        // Existing song is stronger or equal - reject
        newBard.SendMessage("A stronger song of this type is already in effect.");
        return false;
    }
    
    return true;
}
```

### Different Songs Stack

Different song types always stack. A target can have:
- Courage (damage buff) +
- Swiftness (speed buff) +
- Mending (HoT)

...all active simultaneously from the same or different bards.

### Debuff Stacking on Enemies

- **Discordance:** Does not stack. Highest potency wins.
- **Requiem:** Stacks from multiple bards (encourages focus fire).
- **Provocation:** Cannot provoke an already-provoked creature.
- **Peacemaking:** Can re-peace a creature, refreshing duration.

---

## Finales

Finales are instant, powerful effects that consume Crescendo. They do not reserve Concentration.

### Base Finale Pattern

```csharp
public abstract class BaseFinale
{
    public abstract string Name { get; }
    public abstract int CrescendoCost { get; }
    public abstract TimeSpan Cooldown { get; }
    
    public bool TryCast(PlayerMobile bard, Mobile target)
    {
        if (bard.Crescendo < CrescendoCost)
        {
            bard.SendMessage($"You need {CrescendoCost} Crescendo to perform {Name}.");
            return false;
        }
        
        if (DateTime.UtcNow < bard.NextFinaleTime)
        {
            bard.SendMessage("You cannot perform another finale yet.");
            return false;
        }
        
        // Consume crescendo
        bard.Crescendo -= CrescendoCost;
        
        // Apply effect
        ApplyEffect(bard, target);
        
        // Set cooldown
        bard.NextFinaleTime = DateTime.UtcNow + Cooldown;
        
        return true;
    }
    
    protected abstract void ApplyEffect(PlayerMobile bard, Mobile target);
}
```

### Finale Examples

```csharp
// Minor Finale (5 Crescendo)
public class FinaleCoda : BaseFinale
{
    public override string Name => "Coda";
    public override int CrescendoCost => 5;
    public override TimeSpan Cooldown => TimeSpan.FromSeconds(15);
    
    protected override void ApplyEffect(PlayerMobile bard, Mobile target)
    {
        // Refresh duration of one active song on target
        if (target.HasActiveSongs())
        {
            var song = target.GetStrongestActiveSong();
            song.RefreshDuration();
            bard.SendMessage($"Your Coda extends the duration of {song.Type}!");
        }
    }
}

// Major Finale (15 Crescendo)
public class FinaleLullaby : BaseFinale
{
    public override string Name => "Lullaby";
    public override int CrescendoCost => 15;
    public override TimeSpan Cooldown => TimeSpan.FromSeconds(30);
    
    protected override void ApplyEffect(PlayerMobile bard, Mobile target)
    {
        // AoE Peace - no skill check, 5 second duration
        foreach (Mobile m in bard.GetMobilesInRange(6))
        {
            if (m is BaseCreature bc && bard.CanBeHarmful(bc))
            {
                bc.Pacify(bard, DateTime.UtcNow + TimeSpan.FromSeconds(5));
            }
        }
        
        Effects.PlaySound(bard.Location, bard.Map, 0x58D);
        bard.SendMessage("Your lullaby soothes all nearby creatures!");
    }
}

// Magnum Opus (20 Crescendo)
public class FinaleEternalRefrain : BaseFinale
{
    public override string Name => "Eternal Refrain";
    public override int CrescendoCost => 20;
    public override TimeSpan Cooldown => TimeSpan.FromSeconds(300); // 5 minutes
    
    protected override void ApplyEffect(PlayerMobile bard, Mobile target)
    {
        // All active songs become permanent (until death or rest)
        foreach (var song in bard.ActiveSongs.ToList())
        {
            song.MakePermanent();
        }
        
        // Concentration is fully locked while permanent songs exist
        bard.SendMessage("Your songs echo eternally! All active songs are now permanent.");
        
        Effects.SendLocationParticles(
            EffectItem.Create(bard.Location, bard.Map, EffectItem.DefaultDuration),
            0x376A, 9, 32, 5024);
    }
}
```

---

## Song Mastery Progression

Song Mastery is a permanent progression system. Players earn **Melody Points** from bard activities and spend them on **Duration** or **Potency** levels for individual songs.

### Data Structure

```csharp
public class SongMastery
{
    public int TotalLevel { get; private set; } = 0; // 0-32
    public int MelodyPoints { get; set; } = 0;
    
    // Per-song investments
    public Dictionary<SongType, int> DurationLevels = new Dictionary<SongType, int>();
    public Dictionary<SongType, int> PotencyLevels = new Dictionary<SongType, int>();
    
    public int GetLevel(SongType song, MasteryType type)
    {
        var dict = type == MasteryType.Duration ? DurationLevels : PotencyLevels;
        return dict.ContainsKey(song) ? dict[song] : 0;
    }
    
    public bool TryInvest(SongType song, MasteryType type)
    {
        if (TotalLevel >= 32)
            return false; // Max mastery reached
            
        var dict = type == MasteryType.Duration ? DurationLevels : PotencyLevels;
        int current = dict.ContainsKey(song) ? dict[song] : 0;
        
        if (current >= 8)
            return false; // Max for this song/type
            
        dict[song] = current + 1;
        TotalLevel++;
        return true;
    }
}

public enum MasteryType { Duration, Potency }
```

### Melody Point Gains

| Activity | Points |
|----------|--------|
| Damage to peaced target (by bard) | 50% of damage |
| Damage to peaced target (by party) | 25% of damage |
| Provoked creature damage | 25% of damage dealt |
| Discord bonus damage | 100% of bonus |
| Healing from Mending | 1 per HP |
| Difficult target success | 10 × (Difficulty / 25) |

### Points Required per Level

```csharp
public int GetPointsForLevel(int level)
{
    if (level <= 10)
        return 1000 + (level - 1) * 900; // 1000 to 9100
    else
        return 10000 + (level - 10) * 1000; // 10000 to 31000
}
```

---

## Difficulty System

### Creature Difficulty Ratings

Each creature has a **Barding Difficulty** based on power:

| Difficulty | Examples |
|------------|----------|
| 25-40 | Rabbits, chickens, basic animals |
| 41-60 | Orcs, trolls, basic undead |
| 61-80 | Ogres, ettins, liches |
| 81-100 | Dragons, balrons, ancient wyrms |
| 101-120 | Paragons, mini-bosses |
| 121-150 | Dungeon bosses |
| 151-180 | Regional bosses, raid targets |

### Success Formula

```csharp
public double GetSuccessChance(PlayerMobile bard, BaseCreature target, SongType song)
{
    int difficulty = target.BardingDifficulty;
    
    // Apply bonuses
    int skillBonus = Math.Max(0, (int)bard.Skills[SkillName.Musicianship].Value - 100); // 0-20
    int instrumentBonus = GetInstrumentBonus(bard); // 0-12 for legendary
    int potencyBonus = bard.GetSongMasteryLevel(song, MasteryType.Potency) * 3; // 0-24
    int virtuosoBonus = bard.VirtuosoLevel * 3; // 0-24
    
    int totalBonus = skillBonus + instrumentBonus + potencyBonus + virtuosoBonus;
    int effectiveDifficulty = Math.Max(0, difficulty - totalBonus);
    
    int minSkill = effectiveDifficulty - 25;
    int maxSkill = effectiveDifficulty + 25;
    int skill = (int)bard.Skills[SkillName.Musicianship].Value;
    
    if (skill < minSkill)
        return 0.0;
    if (skill >= maxSkill)
        return 1.0;
        
    return (double)(skill - minSkill) / (maxSkill - minSkill);
}
```

---

## UI: Songbook Gump

The Bard has a Songbook gump displaying status:

```
┌─────────────────────────────────────────────┐
│  ♪ SONGBOOK                    [Mastery: 24]│
├─────────────────────────────────────────────┤
│  CONCENTRATION: ████████████░░░  [62/100]   │
│  CRESCENDO:     ████████░░░░░░░  [8/20]     │
├─────────────────────────────────────────────┤
│  ACTIVE SONGS                               │
│  ├─ Provocation (19 conc) [0:42]            │
│  └─ Discordance (14 conc) [0:38]            │
│  Reserved: 33 | Available: 67               │
├─────────────────────────────────────────────┤
│  SONGS              [D] [P]  Cost   ▶       │
│  ├─ Provocation     [8] [8]  19     Play    │
│  ├─ Peacemaking     [4] [4]  22     Play    │
│  ├─ Discordance     [6] [6]  18     Play    │
│  └─ Courage         [0] [0]  25     Play    │
├─────────────────────────────────────────────┤
│  FINALES AVAILABLE                          │
│  └─ [5+] Coda, Interlude                    │
└─────────────────────────────────────────────┘
```

---

## Summary: Resource Interaction

| Situation | Concentration | Crescendo |
|-----------|---------------|-----------|
| Cast a song | Reserved (locked) | +1 or +2 |
| Song expires | Returned | No change |
| Cancel a song | Returned | No change |
| Cast a Finale | No cost | Consumed |
| Out of combat | Regenerates (if 0 songs) | Decays |
| In combat, no songs | Full (100) | Builds on casts |

### Typical Combat Loop

```
Start: 100 Concentration, 0 Crescendo

1. Cast Provocation (19 cost) → 81 Conc, 2 Cresc
2. Cast Discordance (18 cost) → 63 Conc, 4 Cresc
3. Cast Courage (25 cost) → 38 Conc, 5 Cresc
4. Wait... Crescendo builds
5. Cast Mending (25 cost) → 13 Conc, 6 Cresc
6. At 10 Cresc, use Rallying Anthem (no conc cost) → 0 Cresc
7. Provocation expires → 32 Conc returns → 45 Conc
8. Re-cast Provocation → 26 Conc, 2 Cresc
...
```

The Bard must manage both resources - Concentration limits simultaneous songs, Crescendo enables burst plays.
