# Documentation Audit Plan - Systems vs Design vs Implementation

Last Updated: 2026-01-23
Owner: TBD

## Purpose
Establish a repeatable, end-to-end process to reconcile Vystia documentation with actual ServUO implementation and current design roadmap. Reduce document sprawl, prevent contradictions, and create a single source of truth per system.

## Goals
- Build a complete inventory of documentation by system, status, and accuracy.
- Map each system to: design spec, implementation location, and current status.
- Identify conflicts (design vs implementation), stale claims, and duplicate/archived docs.
- Produce an actionable remediation plan with ownership and deadlines.
- Establish ongoing governance to prevent drift.

## Scope
In scope:
- Vystia/ (design, implementation, reference, admin, gm, planning, root docs)
- ServUO/Scripts/ (implementation source of truth)
- ServUO/Data/ (runtime configs, quest XML, lore data)

Out of scope (for initial pass):
- Archive/ historical records (unless referenced by active docs)
- External tooling repos (unless referenced as source of truth)

## Definitions
- Source of Truth (SoT): The single authoritative document or code location for a system.
- Design Spec: Desired behavior, rules, and balance.
- Implementation: Actual code and data that run on the server.
- Roadmap: Planned future work with explicit status labels.

## Deliverables
1. System Audit Matrix (CSV/MD):
   - System name
   - Design doc(s)
   - Implementation location(s)
   - Current status (implemented/partial/planned)
   - Known gaps
   - SoT designation
   - Doc owner

2. Doc Inventory Report:
   - File list with tags: active/archive, system tags, last updated date
   - Duplication map (docs that overlap same system)

3. Remediation Backlog:
   - Each issue includes severity, owner, and target date

4. Governance Rules:
   - Update policies, deprecation rules, review cadence

## Audit Phases

### Phase 0 - Inventory (1-2 days)
- Generate a full list of docs using `rg --files` in Vystia/.
- Tag each doc by:
  - System(s)
  - Status (active, reference, planning, archive)
  - Date (last updated line if present)
- Output: `Vystia/planning/DOC_INVENTORY.md`

### Phase 1 - System Mapping (2-4 days)
- For each system, collect:
  - Design docs (design/)
  - Implementation files (ServUO/Scripts/)
  - Runtime data (ServUO/Data/)
  - Tests (Vystia/gm, Vystia/TESTING_*)
- Output: `Vystia/planning/SYSTEM_AUDIT_MATRIX.md`

### Phase 2 - Consistency Review (3-5 days)
- Compare design vs implementation for each system.
- Identify discrepancies:
  - Missing features claimed as implemented
  - Outdated counts
  - Conflicting mechanics descriptions
- Output: `Vystia/planning/DOC_GAP_REPORT.md`

### Phase 3 - Remediation (ongoing)
- Update active docs to match SoT.
- Deprecate or archive stale docs.
- Add notes to design docs where implementation diverges.
- Output: Updated docs + backlog in `Vystia/planning/DOC_REMEDIATION_BACKLOG.md`

### Phase 4 - Governance (ongoing)
- Add a SoT banner to key system docs.
- Require a "Last Updated" line in active docs.
- Monthly audit: check top 10 systems for drift.
- Output: `Vystia/planning/DOC_GOVERNANCE.md`

## System Coverage Checklist (Initial)
- Classes (v2 system)
- Magic schools/spells
- Religions
- Factions
- Quest systems (Dynamic vs Mondain/BaseQuest)
- NPC systems
- Economy
- Housing
- Pets
- Zones/PvP rules
- Sea/Underwater
- Crafting tiers
- LLM lore/memory

## Tooling
- `rg` for fast content search
- `git` for change tracking
- Optional: small script to extract "Last Updated" metadata

## Rules for Reducing Doc Sprawl
1. Each system must have one SoT doc in Vystia/ (design or implementation reference).
2. Any other doc must link to the SoT and state its own scope (guide, tutorial, test, roadmap).
3. Archive docs must be clearly marked as historical and must not be referenced as SoT.
4. New docs require:
   - Purpose
   - SoT link
   - Last Updated date

## Immediate Next Steps
1. Create `DOC_INVENTORY.md` with system tags.
2. Build `SYSTEM_AUDIT_MATRIX.md` for top 10 systems.
3. Triage the top 20 conflicts into `DOC_GAP_REPORT.md`.
