# VYSTIA BARD SYSTEM
## Songweaving & Song Mastery

**Version:** 1.0  
**Class:** Bard  
**Primary Skill:** Songweaving (ID 67)  
**Secondary Resource:** Crescendo (0-20)  
**Progression System:** Song Mastery (Permanent)

---

# PART I: SYSTEM OVERVIEW

## 1.1 Dual Progression Design

The Vystia Bard uses two interlocking progression systems:

| System | Type | Purpose | Timeframe |
|--------|------|---------|-----------|
| **Songweaving Skill** | 0-100 | Base success chance, song access | Permanent (standard skill) |
| **Crescendo** | 0-20 | In-combat resource for finishers | Per-fight (resets OOC) |
| **Song Mastery** | Levels 1-32 | Customize song power/duration | Permanent (earned) |

This creates three axes of growth:
1. **Vertical:** Songweaving skill unlocks new songs and improves base success
2. **Horizontal:** Song Mastery points let you specialize in specific songs
3. **Tactical:** Crescendo management determines moment-to-moment effectiveness

---

## 1.2 Core Loop

```
COMBAT FLOW:
1. Play a Song (Songweaving check)
   ↓
2. Success → Effect applies, Crescendo builds (+1-3)
   ↓
3. Crescendo hits threshold → Unlock Finale options
   ↓
4. Spend Crescendo on powerful Finale
   ↓
5. 7-second cooldown before next Song
   ↓
6. Repeat, building toward next Finale
```

```
PROGRESSION FLOW:
1. Play Songs in combat
   ↓
2. Earn Melody Points from song effects
   ↓
3. Accumulate points toward next Song Mastery level
   ↓
4. Level up → Spend 1 point on Duration OR Potency of any song
   ↓
5. Repeat until Song Mastery 32 (max)
```

---

# PART II: SONGWEAVING SKILL

## 2.1 Skill Progression

| Songweaving | Capability |
|-------------|------------|
| 0 | Cannot play songs |
| 30 | Basic songs (Provocation, Peacemaking), 8-tile range |
| 50 | Intermediate songs (Discordance, Mending), 10-tile range |
| 70 | Advanced songs (Fortissimo, Requiem), 11-tile range |
| 90 | Master songs (Symphony, Cacophony), 12-tile range |
| 100 | **Virtuoso:** All songs, 13-tile range, -20 difficulty to all songs |
| 110+ | Each point above 100 reduces difficulty by 1 (max 120 = -20 additional) |

### Range Formula
```
Song Range = 8 + (Songweaving / 20)
```

| Songweaving | Range |
|-------------|-------|
| 30 | 9 tiles |
| 50 | 10 tiles |
| 70 | 11 tiles |
| 90 | 12 tiles |
| 100 | 13 tiles |
| 120 | 14 tiles |

---

## 2.2 Skill Checks

Every song requires a **dual skill check**:

1. **Musicianship Check** (instrument proficiency)
2. **Songweaving Check** (song-specific success)

Both must succeed for the song to take effect.

### Success Formula
```
Difficulty = Target Difficulty - Bonuses
MinSkill = Difficulty - 25
MaxSkill = Difficulty + 25
Success Chance = (Skill - MinSkill) / (MaxSkill - MinSkill)
```

### Difficulty Bonuses

| Source | Bonus |
|--------|-------|
| Songweaving above 100 | -1 per point (max -20) |
| Exceptional instrument | -5 |
| Magical instrument bonus | -1 per +1 skill |
| Song Potency investment | -3 per level (max -24) |
| Virtuoso Talent | -3 per level (max -24) |
| **Maximum Possible** | **-98** |

---

# PART III: CRESCENDO SYSTEM

## 3.1 Resource Mechanics

**Crescendo** is the Bard's in-combat secondary resource.

| Property | Value |
|----------|-------|
| Range | 0-20 |
| Generation | +1-3 per successful song |
| Decay | -1 per 10 seconds out of combat |
| Full Reset | 60 seconds out of combat |

### Crescendo Generation by Song

| Song Type | Crescendo Generated |
|-----------|---------------------|
| Utility (Light, Fortune) | +1 |
| Control (Peacemaking, Provocation) | +2 |
| Damage (Discordance, Cacophony) | +2 |
| Healing (Mending, Symphony) | +1 |
| Finales | Consumes Crescendo |

### Crescendo Thresholds

| Crescendo | Unlocks |
|-----------|---------|
| 5 | Minor Finales available |
| 10 | Standard Finales available |
| 15 | Major Finales available |
| 20 | **Magnum Opus** available (ultimate) |

---

## 3.2 Finales

Finales are powerful abilities that consume Crescendo. They do not require skill checks - if you have the Crescendo, they succeed.

### Minor Finales (5 Crescendo)

| Finale | Effect |
|--------|--------|
| **Coda** | Instantly refresh one song's duration on target |
| **Interlude** | Party gains +10% movement speed for 15 seconds |
| **Sharp Note** | Single target 30-50 sonic damage |

### Standard Finales (10 Crescendo)

| Finale | Effect |
|--------|--------|
| **Crescendo Burst** | All active songs gain +50% remaining duration |
| **Harmonize** | Copy your strongest active song to a second target |
| **Dissonant Strike** | Single target 60-100 sonic damage + 3 second silence |
| **Rallying Anthem** | Party gains +15% damage for 20 seconds |

### Major Finales (15 Crescendo)

| Finale | Effect |
|--------|--------|
| **Fortissimo** | AoE 80-120 sonic damage in 5 tiles |
| **Lullaby** | AoE Peacemaking (6 tiles, 5 seconds, no check) |
| **Battle Hymn** | Party gains +20% damage, +10% attack speed for 30 seconds |
| **Soothing Chorus** | Party heals 50-80 HP over 10 seconds |

### Magnum Opus (20 Crescendo)

| Finale | Songweaving | Effect |
|--------|-------------|--------|
| **Symphony of Destruction** | 90 | All enemies in 8 tiles take 150-200 sonic damage over 5 seconds, -25% damage dealt |
| **Hymn of Restoration** | 90 | All party members in 10 tiles fully healed, cleansed, +50 HP buffer for 30 seconds |
| **Cacophony of Madness** | 100 | All enemies in 6 tiles provoked against random targets for 10 seconds |
| **Eternal Refrain** | 100 | All your active songs become permanent until you die or rest |

---

# PART IV: THE SONGS

## 4.1 Song Categories

| Category | Songs | Purpose |
|----------|-------|---------|
| **Control** | Provocation, Peacemaking | Manipulate enemy behavior |
| **Debuff** | Discordance, Requiem | Weaken enemies |
| **Support** | Mending, Courage, Swiftness | Buff and heal allies |
| **Utility** | Light, Fortune | Non-combat benefits |

---

## 4.2 Control Songs

### Song of Provocation

| Property | Value |
|----------|-------|
| **Effect** | Provokes target creature to attack another creature |
| **Songweaving Required** | 30 |
| **Cooldown** | 7 seconds |
| **Target** | Two creatures |
| **Base Duration** | 20 seconds |
| **Duration per Level** | +2 seconds (max +32 sec at 16 levels) |
| **Potency Effect** | Reduces difficulty (-3 per level) |
| **Crescendo** | +2 on success |

**Duration Formula:**
```
Duration = 20 + (Duration Levels + Virtuoso Levels) × 2
Maximum = 52 seconds
```

**Notes:**
- Provoked creatures fight until one dies OR duration expires
- Higher difficulty creatures resist more effectively
- Cannot provoke creatures already in combat with players

---

### Song of Peacemaking

| Property | Single Target | AoE (Self-Targeted) |
|----------|---------------|---------------------|
| **Effect** | Calms one creature | Calms all creatures in radius |
| **Songweaving Required** | 30 | 30 |
| **Cooldown** | 7 seconds | 7 seconds |
| **Radius** | N/A | 6 tiles |
| **Base Duration** | 20 seconds | 1 second |
| **Duration per Level** | +2 seconds | +0.1 seconds |
| **Maximum Duration** | 52 seconds | 2.6 seconds |
| **Potency Effect** | Reduces difficulty | None |
| **Crescendo** | +2 on success | +2 on success |

**Tune Shattering:**
Peacemaking can break early when the target takes damage:
```
Shatter Chance = (MaxHP - CurrentHP) × 0.1%
```
A creature at 50% HP has a 50% chance to break peace when damaged.

**PvP Notes:**
- Single-target mode interrupts spellcasting
- Forces target out of combat mode
- Does not prevent movement

---

## 4.3 Debuff Songs

### Song of Discordance

| Property | Value |
|----------|-------|
| **Effect** | Target deals less damage and takes more damage |
| **Songweaving Required** | 50 |
| **Cooldown** | 7 seconds |
| **Target** | Single creature |
| **Base Duration** | 20 seconds |
| **Base Effect** | -15% damage dealt, +15% damage taken |
| **Duration per Level** | +2 seconds |
| **Potency per Level** | +1% to both effects |
| **Maximum Effect** | -31% dealt, +31% taken |
| **Crescendo** | +2 on success |

**Potency Formula:**
```
Effect = 15% + Potency Levels + Virtuoso Levels
Maximum = 31% (at 8 Potency + 8 Virtuoso)
```

**Stacking:** Discordance does not stack. Reapplying refreshes duration.

---

### Song of Requiem

| Property | Value |
|----------|-------|
| **Effect** | DoT that intensifies as target loses health |
| **Songweaving Required** | 70 |
| **Cooldown** | 7 seconds |
| **Target** | Single creature |
| **Base Duration** | 15 seconds |
| **Base Damage** | 5 per tick (3 second ticks) |
| **Duration per Level** | +1.5 seconds |
| **Potency per Level** | +2 damage per tick, +5% scaling |
| **Crescendo** | +2 on success |

**Scaling Formula:**
```
Damage per Tick = (Base + Potency Bonus) × (1 + MissingHP% × Scaling)
```

Example at Potency 8:
- Target at 100% HP: 21 damage per tick
- Target at 50% HP: 21 × 1.4 = 29 damage per tick
- Target at 25% HP: 21 × 1.55 = 33 damage per tick

---

## 4.4 Support Songs

### Song of Mending

| Property | Value |
|----------|-------|
| **Effect** | AoE heal over time for party members |
| **Songweaving Required** | 50 |
| **Cooldown** | 7 seconds |
| **Radius** | 6 tiles (centered on bard) |
| **Base Ticks** | 2 |
| **Tick Interval** | 4 seconds |
| **Base Healing** | 5 HP per tick |
| **Duration per Level** | +1 tick |
| **Potency per Level** | +2 HP per tick |
| **Maximum** | 10 ticks, 21 HP per tick (210 total) |
| **Crescendo** | +1 on success |

**Difficulty:** Fixed at 115 (does not scale with target)

---

### Song of Courage

| Property | Value |
|----------|-------|
| **Effect** | Party members gain damage bonus |
| **Songweaving Required** | 70 |
| **Cooldown** | 7 seconds |
| **Radius** | 8 tiles |
| **Base Duration** | 30 seconds |
| **Base Effect** | +5% damage |
| **Duration per Level** | +3 seconds |
| **Potency per Level** | +1% damage |
| **Maximum** | 78 seconds, +21% damage |
| **Crescendo** | +1 on success |

---

### Song of Swiftness

| Property | Value |
|----------|-------|
| **Effect** | Party members gain movement and attack speed |
| **Songweaving Required** | 70 |
| **Cooldown** | 7 seconds |
| **Radius** | 8 tiles |
| **Base Duration** | 30 seconds |
| **Base Effect** | +5% movement, +3% attack speed |
| **Duration per Level** | +3 seconds |
| **Potency per Level** | +1% movement, +0.5% attack speed |
| **Maximum** | 78 seconds, +21% move, +11% attack |
| **Crescendo** | +1 on success |

---

## 4.5 Utility Songs

Utility songs only require a Musicianship check, not Songweaving.

### Song of Light

| Property | Value |
|----------|-------|
| **Effect** | Night sight for bard and party |
| **Songweaving Required** | 30 |
| **Cooldown** | 7 seconds |
| **Base Duration** | 10 minutes |
| **Per Repertoire Level** | +2 minutes |
| **Maximum** | 26 minutes |
| **Crescendo** | +1 on success |

---

### Song of Fortune

| Property | Value |
|----------|-------|
| **Effect** | Luck bonus for bard and party |
| **Songweaving Required** | 50 |
| **Cooldown** | 7 seconds |
| **Base Duration** | 10 minutes |
| **Base Effect** | +10 luck |
| **Per Repertoire Level** | +2 minutes, +2 luck |
| **Maximum** | 26 minutes, +26 luck |
| **Party Effect** | 50% of bard's luck bonus |
| **Crescendo** | +1 on success |

---

# PART V: SONG MASTERY

## 5.1 Overview

**Song Mastery** is a permanent progression system separate from Songweaving skill. It represents the bard's deep expertise with specific songs.

| Property | Value |
|----------|-------|
| Maximum Level | 32 |
| Points per Level | 1 |
| Total Points | 32 |
| Spent On | Duration OR Potency of individual songs |
| Max per Song Aspect | 8 levels |

### What 32 Points Buys

With 32 points, a bard can fully master **two songs** (8 Duration + 8 Potency each) OR spread investment across more songs with less specialization.

**Example Builds:**

| Build | Investment | Identity |
|-------|------------|----------|
| **Provoker** | Provocation 16, Discordance 16 | Dungeon farmer, pet class support |
| **Controller** | Peacemaking 16, Provocation 16 | CC specialist, group utility |
| **War Bard** | Discordance 16, Courage 16 | Raid DPS support |
| **Healer Bard** | Mending 16, Courage 8, Swiftness 8 | Off-healer, buffer |
| **Generalist** | 4 songs at 8 each | Flexibility over power |

---

## 5.2 Melody Points

**Melody Points** are earned through effective bard play and accumulate toward Song Mastery levels.

### Earning Melody Points

| Activity | Points Earned |
|----------|---------------|
| Damage to peaced target (by bard) | 50% of damage |
| Damage to peaced target (by others) | 25% of damage |
| Damage from provoked creatures | 25% of damage dealt |
| Bonus damage from Discordance | 100% of bonus damage |
| Healing from Song of Mending | 1 point per HP healed |
| Successful song on difficult target | 10 × (Difficulty / 25) |

### Melody Points Required

| Level | Total Points | Level | Total Points |
|-------|--------------|-------|--------------|
| 1 | 1,000 | 17 | 16,000 |
| 2 | 1,900 | 18 | 17,000 |
| 3 | 2,900 | 19 | 18,000 |
| 4 | 3,800 | 20 | 19,000 |
| 5 | 4,800 | 21 | 20,000 |
| 6 | 5,800 | 22 | 21,000 |
| 7 | 6,700 | 23 | 22,000 |
| 8 | 7,700 | 24 | 23,000 |
| 9 | 8,700 | 25 | 24,000 |
| 10 | 9,600 | 26 | 25,000 |
| 11 | 10,600 | 27 | 26,000 |
| 12 | 11,500 | 28 | 27,000 |
| 13 | 12,500 | 29 | 28,000 |
| 14 | 13,400 | 30 | 29,000 |
| 15 | 14,400 | 31 | 30,000 |
| 16 | 15,400 | 32 | 31,000 |

**Estimated Time to Max:**
- Casual play (500 points/hour): ~62 hours to level 32
- Active dungeon farming (1,500 points/hour): ~21 hours to level 32

---

## 5.3 Spending Song Mastery Points

When you gain a Song Mastery level, you receive **1 point** to spend.

### Investable Songs

| Song | Duration Effect | Potency Effect |
|------|-----------------|----------------|
| Provocation | +2 sec/level | -3 difficulty/level |
| Peacemaking | +2 sec/level (single), +0.1 sec/level (AoE) | -3 difficulty/level |
| Discordance | +2 sec/level | -3 difficulty/level, +1% effect/level |
| Requiem | +1.5 sec/level | -3 difficulty/level, +2 damage/level |
| Mending | +1 tick/level | +2 HP per tick/level |
| Courage | +3 sec/level | +1% damage/level |
| Swiftness | +3 sec/level | +1% move, +0.5% attack/level |

### Maximum Investment

| Aspect | Maximum Levels | From Mastery | From Virtuoso Talent |
|--------|----------------|--------------|----------------------|
| Duration | 16 | 8 | +8 |
| Potency | 16 | 8 | +8 |

The **Virtuoso Talent** (see Part VI) adds up to 8 additional effective levels to both Duration and Potency for the three core songs (Provocation, Peacemaking, Discordance).

---

## 5.4 Respeccing

Song Mastery points can be reallocated:

| Method | Cost | Availability |
|--------|------|--------------|
| Full Respec | 5,000 gold | Any major city bard trainer |
| Single Point Move | 500 gold | Any bard trainer |
| Free Respec | Free | Once per character (newbie grace) |

---

# PART VI: VIRTUOSO TALENT

## 6.1 Overview

**Virtuoso** is a talent tree available to bards who reach GM Songweaving (100). It provides additional power scaling beyond Song Mastery.

| Property | Value |
|----------|-------|
| Unlock Requirement | 100 Songweaving |
| Maximum Levels | 8 |
| Cost per Level | 2,000 Melody Points |
| Total Cost | 16,000 Melody Points |

### Virtuoso Benefits (Per Level)

| Benefit | Per Level | At Max (8) |
|---------|-----------|------------|
| Provocation Duration | +2 seconds | +16 seconds |
| Peacemaking Duration | +2 seconds | +16 seconds |
| Discordance Duration | +2 seconds | +16 seconds |
| Core Song Potency | +3 difficulty reduction | -24 difficulty |
| Discordance Effect | +1% | +8% |
| Crescendo Generation | +0.1 per song | +0.8 per song |

**Note:** Virtuoso bonuses stack with Song Mastery investments, allowing the three core songs to reach 16/16 in both Duration and Potency.

---

## 6.2 Repertoire Talent

**Repertoire** is a secondary talent that enhances utility songs.

| Property | Value |
|----------|-------|
| Unlock Requirement | 70 Songweaving |
| Maximum Levels | 8 |
| Cost per Level | 1,000 Melody Points |
| Total Cost | 8,000 Melody Points |

### Repertoire Benefits (Per Level)

| Benefit | Per Level | At Max (8) |
|---------|-----------|------------|
| Song of Light Duration | +2 minutes | +16 minutes |
| Song of Fortune Duration | +2 minutes | +16 minutes |
| Song of Fortune Luck | +2 | +16 |
| Unlocks New Utility Songs | At levels 2, 4, 6, 8 | 4 new songs |

### Repertoire Unlocked Songs

| Level | Song | Effect |
|-------|------|--------|
| 2 | **Song of Insight** | Party gains +10% skill gain rate for 10 min |
| 4 | **Song of Warding** | Party gains +10% magic resist for 5 min |
| 6 | **Song of the Traveler** | Party gains +25% mount speed for 10 min |
| 8 | **Song of Sanctuary** | Create safe rest area (no spawns 10 tiles) for 5 min |

---

# PART VII: INSTRUMENTS

## 7.1 Instrument Quality

| Quality | Difficulty Modifier | Availability |
|---------|---------------------|--------------|
| Standard | None | Vendor, loot |
| Quality | -2 | Crafted |
| Exceptional | -5 | Crafted (high skill) |
| Masterwork | -8 | GM crafter + runic tool |
| Legendary | -12 | Boss drop, special craft |

## 7.2 Instrument Types

| Instrument | Stat Bonus | Best For |
|------------|------------|----------|
| **Lute** | +5% song duration | Sustained buffs |
| **Harp** | +10% healing songs | Mending builds |
| **Drum** | +5% Crescendo generation | Finale-focused |
| **Flute** | +2 tile range | Ranged support |
| **Horn** | +10% AoE radius | Group support |
| **Fiddle** | +5% debuff potency | Discordance builds |

## 7.3 Magical Instruments

Magical instruments can have:

| Property | Range | Effect |
|----------|-------|--------|
| Skill Bonus | +1 to +25 | Adds to Songweaving for checks |
| Slayer | Specific creature type | +100% effect vs that type |
| Mana Reduction | -5 to -20 | Reduces mana cost (if any) |
| Durability | +10 to +50 | Increased uses before repair |

### Named Legendary Instruments

| Instrument | Source | Special Effect |
|------------|--------|----------------|
| **Orpheus's Lyre** | Ancient Treant | Songs affect undead (normally immune) |
| **The Siren's Call** | Ancient Kraken | Provocation works on sea creatures |
| **Thunderhorn** | Griffin Lord | Finales deal +50% damage |
| **The Eternal Chord** | Timeworn Lich | Songs persist 10 seconds after death |
| **Moonsilver Harp** | Lunara's Herald | Mending heals +50%, affects pets |

---

# PART VIII: TARGET DIFFICULTY

## 8.1 Creature Difficulty Ratings

Creatures have a **Barding Difficulty** rating based on their overall power.

| Difficulty | Creature Examples |
|------------|-------------------|
| 25-40 | Rabbits, chickens, basic animals |
| 41-60 | Orcs, trolls, basic undead |
| 61-80 | Ogres, ettins, liches |
| 81-100 | Dragons, balrons, ancient wyrms |
| 101-120 | Paragons, mini-bosses |
| 121-150 | Dungeon bosses |
| 151-180 | Regional bosses, raid targets |

### Boss Difficulty Reference

| Boss | Difficulty |
|------|------------|
| Frost Father | 160 |
| Volcano Wyrm | 165 |
| Sphinx of Surya | 155 |
| Coven Matriarch | 150 |
| Ancient Treant | 170 |
| Crystal Drake Alpha | 160 |
| Forge Master | 175 |
| Griffin Lord | 155 |
| Ancient Kraken | 165 |
| Timeworn Lich | 180 |

---

## 8.2 Success Rate Calculations

### Example: GM Bard vs Timeworn Lich (Difficulty 180)

**Bonuses:**
- Songweaving 120: -20
- Exceptional instrument: -5
- Potency 8: -24
- Virtuoso 8: -24
- **Total: -73**

**Calculation:**
```
Effective Difficulty = 180 - 73 = 107
MinSkill = 107 - 25 = 82
MaxSkill = 107 + 25 = 132
Success = (120 - 82) / (132 - 82) = 38/50 = 76%
```

**With +15 Magical Instrument:**
```
Success = (135 - 82) / (147 - 82) = 53/65 = 82%
```

### Example: Mid-Game Bard vs Lich (Difficulty 100)

**Character:** 100 Songweaving, Potency 6, Virtuoso 4, Exceptional instrument

**Bonuses:**
- Songweaving 100: -0
- Exceptional instrument: -5
- Potency 6: -18
- Virtuoso 4: -12
- **Total: -35**

**Calculation:**
```
Effective Difficulty = 100 - 35 = 65
MinSkill = 65 - 25 = 40
MaxSkill = 65 + 25 = 90
Success = (100 - 40) / (90 - 40) = 60/50 = 100% (capped)
```

---

# PART IX: SONGBOOK UI

## 9.1 Interface Design

The Bard has a **Songbook** gump that displays:

```
┌─────────────────────────────────────────────┐
│  ♪ SONGBOOK                    [Mastery: 24]│
├─────────────────────────────────────────────┤
│  CRESCENDO: ████████████░░░░░░░░  [12/20]   │
├─────────────────────────────────────────────┤
│  CONTROL SONGS                              │
│  ├─ Provocation    [D:8] [P:8]  ▶ Play      │
│  └─ Peacemaking    [D:4] [P:4]  ▶ Play      │
│                                             │
│  DEBUFF SONGS                               │
│  ├─ Discordance    [D:6] [P:6]  ▶ Play      │
│  └─ Requiem        [D:0] [P:0]  ▶ Play      │
│                                             │
│  SUPPORT SONGS                              │
│  ├─ Mending        [D:2] [P:2]  ▶ Play      │
│  ├─ Courage        [D:0] [P:0]  ▶ Play      │
│  └─ Swiftness      [D:0] [P:0]  ▶ Play      │
│                                             │
│  UTILITY SONGS (Repertoire: 5)              │
│  ├─ Song of Light              ▶ Play       │
│  ├─ Song of Fortune            ▶ Play       │
│  ├─ Song of Insight            ▶ Play       │
│  └─ Song of Warding            ▶ Play       │
│                                             │
│  FINALES AVAILABLE                          │
│  ├─ [5+]  Coda, Interlude, Sharp Note       │
│  └─ [10+] Crescendo Burst, Harmonize        │
│                                             │
│  [Melody Points: 18,432 / 19,000]           │
│  [Virtuoso: 6/8]  [Repertoire: 5/8]         │
└─────────────────────────────────────────────┘
```

---

## 9.2 Active Song Tracking

The bard can see active songs on targets:

```
┌─────────────────────────────┐
│  ACTIVE SONGS               │
│  Orc Captain:               │
│  ├─ Discordance [0:42]      │
│  └─ Provoked → Orc Mage     │
│  Orc Mage:                  │
│  └─ Provoked → Orc Captain  │
│  Party:                     │
│  └─ Courage [1:15]          │
└─────────────────────────────┘
```

---

# PART X: RELIGION SYNERGY

## 10.1 Bard + Oceana's Covenant

The Bard's natural religion is **Oceana's Covenant**.

| Piety Tier | Synergy Bonus |
|------------|---------------|
| Adherent (50) | +3 max Crescendo (23 max) |
| Devoted (200) | Song of Mending heals +10% |
| Zealot (500) | Finales cost -2 Crescendo |
| Champion (900) | Magnum Opus cooldown -50% |

---

## 10.2 Devotion Powers for Bards

| Power | Tier | Effect |
|-------|------|--------|
| **Tidal Surge** | Devoted | AoE knockback synergizes with Peacemaking |
| **Abyssal Call** | Zealot | Summon + Provocation = easy kills |
| **Oceana's Wrath** | Champion | Pairs with Discordance for massive damage |

---

# PART XI: BUILD EXAMPLES

## 11.1 The Provoker (Dungeon Farmer)

**Philosophy:** Maximize provocation uptime, let monsters kill each other

| Investment | Levels |
|------------|--------|
| Provocation Duration | 8 |
| Provocation Potency | 8 |
| Discordance Duration | 8 |
| Discordance Potency | 8 |
| Virtuoso | 8 |
| **Total** | 32 Mastery + 8 Virtuoso |

**Stats:**
- Provocation: 52 second duration, 76%+ success on bosses
- Discordance: 52 second duration, 31% effect

**Gameplay:** Provoke, Discord the winner, collect loot.

---

## 11.2 The War Bard (Raid Support)

**Philosophy:** Maximize party damage output

| Investment | Levels |
|------------|--------|
| Discordance Duration | 8 |
| Discordance Potency | 8 |
| Courage Duration | 8 |
| Courage Potency | 8 |
| Virtuoso | 8 |
| **Total** | 32 Mastery + 8 Virtuoso |

**Stats:**
- Discordance: 52 sec, 31% effect
- Courage: 78 sec, +21% party damage
- Combined: +52% effective damage increase

**Gameplay:** Discord boss, Courage party, spam Battle Hymn finales.

---

## 11.3 The Pacifist (CC Specialist)

**Philosophy:** Nothing hits anyone, ever

| Investment | Levels |
|------------|--------|
| Peacemaking Duration | 8 |
| Peacemaking Potency | 8 |
| Provocation Duration | 8 |
| Provocation Potency | 8 |
| Virtuoso | 8 |
| Repertoire | 8 |
| **Total** | 32 Mastery + 8 Virtuoso + 8 Repertoire |

**Stats:**
- Peacemaking: 52 sec single, 2.6 sec AoE
- Provocation: 52 sec
- Lullaby Finale: 5 sec AoE peace (no check)

**Gameplay:** Chain CC, never let anything reach the party.

---

## 11.4 The Traveling Minstrel (Utility Focus)

**Philosophy:** Make the party's life easier in every way

| Investment | Levels |
|------------|--------|
| Mending Duration | 8 |
| Mending Potency | 4 |
| Courage Duration | 4 |
| Courage Potency | 4 |
| Swiftness Duration | 4 |
| Swiftness Potency | 4 |
| Fortune via Repertoire | 8 |
| **Total** | 28 Mastery + 8 Repertoire |

**Stats:**
- Mending: 10 ticks, 13 HP/tick (130 total party heal)
- Courage: +9% damage
- Swiftness: +9% move, +5% attack
- Fortune: +26 luck

**Gameplay:** Keep all buffs rolling, off-heal, never stop moving.

---

# APPENDIX A: QUICK REFERENCE

## Song Summary

| Song | Req | CD | Base Dur | Max Dur | Effect |
|------|-----|-----|----------|---------|--------|
| Provocation | 30 | 7s | 20s | 52s | Creature vs creature |
| Peacemaking | 30 | 7s | 20s/1s | 52s/2.6s | Calm target/AoE |
| Discordance | 50 | 7s | 20s | 52s | -/+15-31% damage |
| Requiem | 70 | 7s | 15s | 27s | Scaling DoT |
| Mending | 50 | 7s | 2 ticks | 10 ticks | AoE HoT |
| Courage | 70 | 7s | 30s | 78s | +5-21% damage |
| Swiftness | 70 | 7s | 30s | 78s | +5-21% move |
| Light | 30 | 7s | 10m | 26m | Night sight |
| Fortune | 50 | 7s | 10m | 26m | +10-26 luck |

## Finale Summary

| Finale | Cost | Effect |
|--------|------|--------|
| Coda | 5 | Refresh song duration |
| Interlude | 5 | +10% party move speed |
| Sharp Note | 5 | 30-50 sonic damage |
| Crescendo Burst | 10 | +50% song durations |
| Harmonize | 10 | Copy song to second target |
| Dissonant Strike | 10 | 60-100 damage + silence |
| Rallying Anthem | 10 | +15% party damage 20s |
| Fortissimo | 15 | AoE 80-120 damage |
| Lullaby | 15 | AoE peace 5s (no check) |
| Battle Hymn | 15 | +20% damage, +10% speed 30s |
| Soothing Chorus | 15 | Party heal 50-80 |
| Symphony of Destruction | 20 | AoE 150-200 + debuff |
| Hymn of Restoration | 20 | Full party heal + buffer |
| Cacophony of Madness | 20 | Mass provocation |
| Eternal Refrain | 20 | Songs become permanent |

---

**Document Version:** 1.0  
**System:** Bard / Songweaving  
**Date:** January 2026
