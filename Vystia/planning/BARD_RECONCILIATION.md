# Bard System End-to-End Reconciliation (Template)

Generated: 2026-01-23
Scope: Bard class, Bardic magic, Songweaving system, related items, trainers, skills, and tests.

## 1) Sources of Truth (SoT)
- Design target: `Vystia/design/system_design/VYSTIA_BARD_SYSTEM.md`
- Deprecation note (legacy spellbook): `Vystia/design/magic/BardicMagic.md`
- Implementation plan: `Vystia/design/system_design/VYSTIA_BARD_IMPLEMENTATION_PLAN.md`
- Current code (Bard class + spellbook):
  - `ServUO/Scripts/Custom/VystiaClasses/Classes/BardClass.cs`
  - `ServUO/Scripts/Custom/VystiaClasses/Classes/PlayerClassV2.cs`
  - `ServUO/Scripts/Custom/VystiaClasses/Spells/Bard/*.cs`
  - `ServUO/Scripts/Items/Equipment/Spellbooks/VystiaSpellbooks.cs`
  - `ServUO/Scripts/Items/Vystia/Scrolls/BardScrolls.cs`
  - `ServUO/Scripts/Items/Vystia/Resources/Reagents/BardicMagicReagents.cs`
  - `ServUO/Scripts/Mobiles/Vystia/Vendors/MagicSchoolVendors.cs`
  - `ServUO/Scripts/Mobiles/Vystia/Trainers/VystiaClassTrainers.cs`

## 2) Current Implementation Snapshot (as-is)
- Bard is implemented as a spellbook-based magic school (BardSpellbook + 32 Bardic spells).
- Bardic spells are present as `.cs` files, but BardicMagic.md reports placeholder STR buffs.
- Bard class uses BardicLore skill (ID 63) in most player/GM references.
- Bard trainers/vendors and scrolls exist per spellbook system.

## 3) Design Intent (target)
- Bard should be Songweaving-first with Song Mastery, Crescendo, Virtuoso, and songbook UI.
- Primary skill: Songweaving (ID 67) in design doc.
- Crescendo cap: 20 (design), plus Finales and mastery progression.

## 4) Reconciliation Table (Design vs Implementation)

| Topic | Design Intent | Current Implementation | Gap | Decision |
|---|---|---|---|---|
| Primary skill | Songweaving (ID 67) | BardicLore (ID 63) | Skill mismatch | Use Songweaving as canonical |
| Core system | Songweaving + Song Mastery + Crescendo | Spellbook-based Bardic spells | System mismatch | Migrate to Songweaving-first |
| Resource cap | Crescendo 0-20 | Docs vary (10/100) | Conflicting docs | Standardize to 0-20 |
| Spell effects | Song effects per design | Placeholder STR buffs | Not aligned | Implement real effects per design |
| UI | Songbook gump w/ mastery | Spellbook gump | Missing UI | Build Songbook gump |
| Religion synergy | Oceana's Covenant | Not represented in implementation | Missing linkage | Add devotion hooks for Bard |

## 5) Canonical Decisions (for this reconciliation)
- Bard will be Songweaving-first (design doc is authoritative).
- Bardic spellbook becomes legacy/deprecated.
- Songweaving skill ID will be established and used consistently.
- Crescendo cap is 20 (design), and all docs will be aligned to it.

## 6) Migration Plan (high-level)
1. **Define skill + resource plumbing**
   - Add/confirm Songweaving skill entry and ID.
   - Implement Crescendo resource as a per-combat value (0-20) on player.
2. **Core song system**
   - Implement Songweaving checks (Musicianship + Songweaving).
   - Implement song effects per design spec (control, debuff, support, utility).
3. **Song mastery + talents**
   - Implement Song Mastery point tracking and investment UI.
   - Add Virtuoso/Repertoire talent benefits.
4. **UI**
   - Build Songbook gump with mastery and active song display.
5. **Deprecate Bardic spellbook**
   - Mark BardSpellbook as legacy; update trainer/vendor offerings.
   - Update tests and guides to new system.

## 6.1) Implementation Task Breakdown

Phase A: Foundations
- Define Songweaving skill entry, ID, and GM command exposure.
- Add Crescendo resource storage + decay hooks to player state.
- Add song effect scaffolding (base song class + common timers).

Phase B: Core Songs
- Implement control songs (Provocation, Peacemaking).
- Implement debuff songs (Discordance, Requiem).
- Implement support songs (Mending, Courage, Swiftness).
- Implement utility songs (Light, Fortune).

Phase C: Finales + Scaling
- Implement Crescendo thresholds + Finale unlocks.
- Implement minor/standard/major finales.
- Implement Magnum Opus finales and cooldowns.

Phase D: Song Mastery + Talents
- Song Mastery point tracking + spend UI.
- Virtuoso talent tree with level caps.
- Repertoire talent and utility unlocks.
- Respec costs and trainer interactions.

Phase E: UI/UX
- Songbook gump (song list, mastery levels, Crescendo bar).
- Active song tracking panel.
- Song targeting indicators (range, AoE).

Phase F: World Integration
- Update Bard trainers/vendors to Songweaving items.
- Update class selection grant items (songbook vs spellbook).
- Add Bard devotion synergy hooks (Oceana’s Covenant).

Phase G: Deprecation & Cleanup
- Mark BardSpellbook as legacy in docs and NPC inventory.
- Remove Bardic spell scrolls from vendors or gate to GM.
- Update GM/player tests and guides to Songweaving system.

Phase H: Verification
- Add/refresh tests in GM and player guides.
- Run targeted in-game verification (song success, Crescendo, finales).
- Log evidence into `Vystia/planning/SYSTEM_AUDIT_MATRIX.md`.

## 7) Tests & Verification
- Update/add test cases in:
  - `Vystia/gm/GM_TESTING_GUIDE.md`
  - `Vystia/gm/CLASS_TESTING_GUIDE.md`
  - `Vystia/TESTING_CHECKLIST.md`
- Minimum verification targets:
  - Songweaving success checks (baseline + bonuses)
  - Crescendo generation and decay
  - Finale unlock thresholds
  - Mastery point accrual and respec
  - Bard trainer/vendor inventory reflects Songweaving

## 8) Doc Updates (SoT alignment)
- Deprecate or rewrite Bardic spellbook sections in:
  - `Vystia/reference/SPELLS.md`
  - `Vystia/player/guides/BARD_GUIDE.md`
  - `Vystia/TESTING_CHECKLIST.md`
  - `Vystia/VYSTIA_MASTER_INVENTORY.md`
- Add explicit Songweaving references and new UI/skill definitions.

## 9) Open Questions
- Final Songweaving skill ID choice (reuse 67 vs new slot)?
- How to transition existing BardSpellbook players in live saves?
- Should Bardic spellbook remain for GMs/dev only?

---

This document is the template for end-to-end reconciliation. Replicate this structure for other systems after Bard is aligned.
