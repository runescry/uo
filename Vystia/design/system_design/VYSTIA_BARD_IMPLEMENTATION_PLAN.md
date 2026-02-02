# Vystia Bard Implementation Plan (Songweaving-First)

**Status:** Planned
**Date:** 2026-01-23
**Goal:** Replace the current Bardic spellbook implementation with the Songweaving system defined in `VYSTIA_BARD_SYSTEM.md`.

---

## 1) Objectives
- Make Songweaving the primary Bard experience (combat loop, scaling, identity).
- Deprecate the current Bardic spellbook (legacy/placeholder spells).
- Implement Song Mastery + Melody Points progression.
- Implement Crescendo generation/spend and Finale abilities.
- Provide in-game UI (Songbook + Active Songs tracking).

---

## 2) Guiding Principles
- **Design is source of truth:** `VYSTIA_BARD_SYSTEM.md` defines numbers and mechanics.
- **Data-driven songs:** Song definitions should be table/data-based to avoid 30+ bespoke classes.
- **Clear migration path:** Keep legacy Bardic spells disabled or flagged as deprecated; do not delete immediately.

---

## 3) Systems to Build

### 3.1 Songweaving Skill (Primary)
- Add custom skill ID 67 if not already registered.
- Success checks: dual Musicianship + Songweaving as specified.
- Range formula and difficulty bonuses as per design doc.

### 3.2 Song Definitions (Core Data)
Create a `BardSongDefinition` structure containing:
- Song Id, Name, Category (Control/Debuff/Support/Utility)
- Required Songweaving, Base Duration, Potency/Duration scaling
- Crescendo generation on success
- Targeting type (single/AoE/self/party)
- Effects (buffs/debuffs/heal/damage)

### 3.3 Song Runtime System
- **SongManager**: applies/removes songs, refresh, stacking rules, duration tracking.
- **Active Song Tracking**: for targets and party, with timers.
- **Cooldown**: 7s global song cooldown.

### 3.4 Crescendo System
- Replace channel-based generation with per-song generation (+1-3).
- Decay out of combat and full reset timings per design.
- Finale thresholds and available options.

### 3.5 Finales System
- Finale definitions (cost + effects) with no skill checks.
- Implement Magnum Opus tier (20 Crescendo).

### 3.6 Song Mastery + Melody Points
- Track Melody Points by activity (damage/healing/discord bonus, etc.).
- Level thresholds to 32.
- Point spend on Duration or Potency per song (8 cap each).
- Respec rules (gold cost).

### 3.7 Virtuoso + Repertoire Talents
- Virtuoso (GM Songweaving gate) with level 1–8 bonuses.
- Repertoire (utility song expansion) with unlocks at 2/4/6/8.

### 3.8 Songbook UI
- Songbook gump with:
  - Crescendo bar
  - Song lists + mastery investment display
  - Finales list per threshold
  - Melody points and talent status
- Active songs tracker gump (targets/party)

---

## 4) Deprecation Plan for Bardic Spellbook

### Phase A (Immediate)
- Mark Bardic spellbook as **Legacy** in docs and vendor descriptions.
- Disable Bardic spell casting for players who have Songweaving enabled (config flag).

### Phase B (Migration)
- Map each Bardic spell to the closest Songweaving song (or Finale).
- Provide a one-time conversion for players: Bardic spellbook -> Songbook.

### Phase C (Removal)
- Remove Bardic spell vendors and scroll drops.
- Keep code for reference for one patch cycle, then archive.

---

## 5) Technical Breakdown (Implementation Order)

1. **Skill & Data**
   - Register Songweaving skill (ID 67)
   - Create `BardSongDefinition` registry

2. **Core Runtime**
   - SongManager + active song tracking
   - Songweaving dual-check logic

3. **Crescendo + Finales**
   - Per-song generation
   - Finale spend + effects

4. **Progression**
   - Melody Points
   - Song Mastery points
   - Virtuoso + Repertoire talents

5. **UI**
   - Songbook + Active Songs gumps

6. **Deprecation Switch**
   - Legacy Bardic spellbook disable + conversion

---

## 6) Data/Config Files (Proposed)
- `ServUO/Scripts/Custom/VystiaClasses/Bard/Songs/BardSongRegistry.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Bard/Songs/BardSongDefinition.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Bard/Songs/BardSongManager.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Bard/Progression/SongMastery.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Bard/Progression/MelodyPoints.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Bard/UI/SongbookGump.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Bard/UI/ActiveSongsGump.cs`

---

## 7) Test Plan (Minimum)
- Songweaving success checks at 30/50/70/90/100
- Crescendo generation per song category
- Finale spend thresholds and effect application
- Mastery point spend/respec
- Virtuoso/Repertoire bonuses applied
- UI correctness and cooldown enforcement

---

## 8) Open Questions
- Should Bardic spellbook be fully removed or kept for GM/admin testing only?
- Should Bard songs use mana or purely Songweaving checks?
- How should Songweaving interact with existing Bardic Magic reagent economy?

---

**Primary spec:** `Vystia/design/system_design/VYSTIA_BARD_SYSTEM.md`
