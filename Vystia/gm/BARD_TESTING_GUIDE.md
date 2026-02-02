# Vystia Bard (Songweaving) Testing Guide

**Purpose:** End-to-end validation of the Bard class and Songweaving system.
**Last Updated:** 2026-01-23
**Source of Truth:** `ServUO/Scripts/Custom/VystiaClasses/Systems/SongweavingSongs.cs` and `ServUO/Scripts/Custom/VystiaClasses/Systems/SongweavingSystem.cs`.

---

## Quick Setup

```
[SetClassV2 Bard
[svs 100
[skillcap 7200
[ResetResources
```

### Required Items
- **Songbook of Weaving (Spellbook)** (received on Bard class select or via vendor; drag icons/hotkeys)
- **Songweaving Hotbar** (open via `[songbar`)

Optional for support tests:
- **MagicLute** (party buff item)
- **CrescendoCrystal** (restore Crescendo)
- **CrescendoCatalyst** (increase Crescendo generation)

---

## Commands

```
[sb songweaving     # Gives Songbook of Weaving
[song <name>        # Perform a song by key
[songbar            # Open Songweaving hotbar
[finale <name>      # Perform a finale (Crescendo spender)
[finales           # Open Finale list (with Crescendo tracker)
[songmastery        # View mastery status / spend points
```

Song keys (Bardic names accepted):
- `discordant note`, `song of courage`, `lullaby`, `inspire accuracy`, `song of healing`, `dirge of weakness`, `song of illumination`, `song of swiftness`, `song of provocation`

Finale keys:
- `sharpnote`, `interlude`, `rally`, `fortissimo`, `soothing`, `symphony`

---

## Core System Checks

### 1) Class Gating
- **Non-Bard** attempts to use songbook or commands ? should be blocked with a message.
- **Bard** can open the Songbook and Hotbar without errors.

### 2) Global Cooldown
- Songs share a global cooldown (default 7s).
- Attempt to cast twice quickly ? blocked and hotbar shows remaining seconds.

### 3) Skill Checks
- Songs require **Songweaving** + **Musicianship** checks.
- With low skill, song fails with performance or requirement messages.

### 4) Crescendo Resource
- Successful songs generate Crescendo (amount varies by song).
- Crescendo **decays out of combat** after 3s at -1 per tick.
- Verify resource display updates via `[GetResources`.
- Open `[finales` and confirm Crescendo tracker shows current/max and costs reflect availability.

---

## Song-by-Song Tests

### Song of Provocation
**Type:** Control (single target)
- **Required Songweaving:** 30.0
- **Crescendo Gain:** 2
- **Test:**
  1. Target a creature to provoke.
  2. Target a second creature.
  3. **Expected:** Creature A attacks creature B.

### Lullaby
**Type:** Control (single target)
- **Required Songweaving:** 30.0
- **Duration:** 20s
- **Crescendo Gain:** 2
- **Test:**
  1. Target a creature or player.
  2. **Expected (creature):** Pacified for 20s.
  3. **Expected (player):** Warmode disabled; message shown.

### Discordant Note
**Type:** Debuff (single target)
- **Required Songweaving:** 50.0
- **Duration:** 20s
- **Effect:** Vulnerability debuff (power 15)
- **Crescendo Gain:** 2
- **Test:**
  1. Target enemy.
  2. **Expected:** Vulnerability buff applied; target message.

### Dirge of Weakness
**Type:** Damage-over-time (single target)
- **Required Songweaving:** 70.0
- **Duration:** 15s
- **Effect:** Corruption DoT (power 5)
- **Crescendo Gain:** 2
- **Test:**
  1. Target enemy.
  2. **Expected:** Corruption DoT applied; target message.

### Song of Healing
**Type:** Party support (AoE)
- **Required Songweaving:** 50.0
- **Range:** 6 tiles
- **Duration:** 10s
- **Effect:** Rejuvenation HoT (power 5)
- **Crescendo Gain:** 1
- **Test:**
  1. Form a party, keep members within 6 tiles.
  2. Cast Mending.
  3. **Expected:** Party members receive Rejuvenation buff and message.

### Song of Courage
**Type:** Party support (AoE)
- **Required Songweaving:** 70.0
- **Range:** 8 tiles
- **Duration:** 30s
- **Effect:** All-stats buff (power 5)
- **Crescendo Gain:** 1
- **Test:**
  1. Party in range.
  2. Cast Courage.
  3. **Expected:** All party members receive AllStatsBuff.

### Song of Swiftness
**Type:** Party support (AoE)
- **Required Songweaving:** 70.0
- **Range:** 8 tiles
- **Duration:** 30s
- **Effect:** Dexterity buff (power 5)
- **Crescendo Gain:** 1
- **Test:**
  1. Party in range.
  2. Cast Swiftness.
  3. **Expected:** DexterityBuff applied to party members.

### Song of Illumination
**Type:** Utility (AoE)
- **Required Songweaving:** 30.0
- **Range:** 6 tiles
- **Effect:** Night sight (BuffIcon.NightSight)
- **Crescendo Gain:** 1
- **Test:**
  1. Party in range.
  2. Cast Light.
  3. **Expected:** Night sight active on party; light particles and sound.

### Inspire Accuracy
**Type:** Utility (AoE)
- **Required Songweaving:** 50.0
- **Range:** 6 tiles
- **Duration:** 10 minutes
- **Effect:** Luck bonus +20
- **Crescendo Gain:** 1
- **Test:**
  1. Party in range.
  2. Cast Fortune.
  3. **Expected:** Luck bonus applied; message shown.

---

## Failure & Edge Cases

- **Immune target:** Provocation on unprovokable creature ? immune message.
- **Invalid target:** Non-mobile target for single-target songs ? error message.
- **Out of range:** Target too far ? no effect, standard target failure.
- **Cooldown:** Attempt during cooldown ? You must wait... and no effect.

---

## Regression Checklist

- [ ] Songbook only usable by Bard class.
- [ ] Hotbar opens from Songbook and via `[songbar`.
- [ ] Global cooldown visible on Songbook and Hotbar.
- [ ] Crescendo increases on successful songs and decays out of combat.
- [ ] Party range checks (6/8 tiles) respected.
- [ ] Buffs apply with correct durations and power.
- [ ] Fortune applies luck bonus for 10 minutes.
- [ ] Messages appear on caster and targets.

---

## Related Docs

- `Vystia/gm/GM_TESTING_GUIDE.md`
- `Vystia/TESTING_GUIDE.md`
- `Vystia/TESTING_CHECKLIST.md`

---

## Crescendo Finales Tests

### Setup
1. Build Crescendo to 20 via songs.
2. Use `[GetResources` to confirm Crescendo value.

### Finale Execution
- **Sharp Note (5):** Target a creature, confirm 30-50 damage and effects.
- **Mesmerise (5):** Target a creature, confirm paralyze (~4s) and effects.
- **Cacophony (10):** Target ground, confirm AoE damage in 6 tiles.
- **Fortissimo (15):** Target ground, confirm AoE damage in 5 tiles.
- **Soothing Chorus (15):** Party heal 50-80 with effects.
- **Symphony of Destruction (20):** Target ground, confirm AoE damage in 8 tiles.

### Expected
- Crescendo cost is deducted immediately.
- Global cooldown prevents immediate re-cast.

---

## Song Mastery Tests

### Gain Points
1. Cast songs repeatedly.
2. Every 10 successful songs ? +1 Song Mastery point.
3. Verify `[songmastery` shows level, XP, and points.

### Spend Points
```
[songmastery mending potency
[songmastery courage duration
```

### Validate Effects
- **Potency:** Buff magnitude increases by +10% per point (max 5).
- **Duration:** Buff duration increases by +10% per point (max 5).

---
