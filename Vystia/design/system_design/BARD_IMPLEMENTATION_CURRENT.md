# Bard Songweaving Implementation (Current Code Snapshot)

Last Updated: 2026-01-26

This document describes the current Songweaving implementation in code (server + client) as it exists in the repo. It is not a design spec; it is a factual wiring overview.

## 1) Core System Flow (Server)

### Entry points (player-facing)
- Commands:
  - `[song <name>]`, `[songweave]` -> perform a song by registry key
  - `[songbar]` -> open the Songweaving hotbar
  - `[finale <name>]`, `[finales]` -> perform/open finales
  - `[songmastery ...]` and `[songmasteryadd ...]`
- File: `ServUO/Scripts/Custom/VystiaClasses/Commands/SongweavingCommands.cs`

### Spellbook integration (server-side)
- Dedicated spellbook type `VystiaSongweaving` with range **1384–1415**.
- `SongweavingSpellbook` is the main item (hue 0x8A5, item ID 0xEFA).
- A legacy item `SongbookOfWeaving` converts to the new spellbook on use.
- Files:
  - `ServUO/Scripts/Items/Equipment/Spellbooks/Spellbook.cs`
  - `ServUO/Scripts/Items/Equipment/Spellbooks/SongweavingSpellbook.cs`
  - `ServUO/Scripts/Items/Equipment/Spellbooks/SongbookOfWeaving.cs`

### Songweaving system mechanics
- `SongweavingSystem.TryPerformSong`:
  - checks Songweaving skill minimum
  - enforces cooldown
  - checks Musicianship + Songweaving skill rolls
  - grants Crescendo
  - awards Song Mastery XP
- `SongweavingSystem.StartChanneling/StopChanneling` exist but are not used by songs currently.
- File: `ServUO/Scripts/Custom/VystiaClasses/Systems/SongweavingSystem.cs`

### Crescendo (secondary resource)
- Implemented in `CrescendoResource`:
  - Max = 20 (+ class-religion synergy bonus)
  - Decay: -1 every 3s, decays out of combat
  - Channeling flag exists; if `m_IsChanneling` true, generates +1 per tick instead of decay
- File: `ServUO/Scripts/Custom/VystiaClasses/Systems/SecondaryResource.cs`

### Crescendo tracker (UI)
- `CrescendoTrackerGump` shows a small bar with current/max.
- Commands: `[Crescendo]` or `[CR]` toggle.
- Auto-enabled for Bard V2 on class equip and on login.
- Files:
  - `ServUO/Scripts/Custom/VystiaClasses/Gumps/CrescendoTrackerGump.cs`
  - `ServUO/Scripts/Custom/VystiaClasses/Classes/PlayerClassV2.cs`
  - `ServUO/Scripts/Mobiles/PlayerMobile.cs`

## 2) Song Definitions (Server)

### Registry + icons
- `SongweavingRegistry` defines song entries, aliases, display name, icon, and hue.
- Icons are currently scroll item `0x1F2D` with different hues.
- File: `ServUO/Scripts/Custom/VystiaClasses/Systems/SongweavingSongs.cs`

### Songs implemented
All songs are **instant** (not sustained channeling).

Control:
- Song of Provocation -> uses BaseCreature.Provoke (two-target flow)
- Lullaby -> pacify target (player or creature)

Debuffs:
- Discordant Note -> `VystiaBuffType.Vulnerability` (damage taken)
- Dirge of Weakness -> `VystiaBuffType.Corruption` (DoT)

Support:
- Song of Healing -> `VystiaBuffType.Rejuvenation` (HoT)
- Song of Courage -> `VystiaBuffType.AllStatsBuff`
- Song of Swiftness -> `VystiaBuffType.DexterityBuff`

Utility:
- Song of Illumination -> Night Sight (LightCycle + BuffInfo)
- Inspire Accuracy -> Luck bonus via `VystiaLuckBonusSystem`

File: `ServUO/Scripts/Custom/VystiaClasses/Systems/SongweavingSongs.cs`

### Party targeting
- Party range helpers are in `SongweavingParty.GetPartyTargets`.
- By default, `includeSelf` is **true** (self is included).
- File: `ServUO/Scripts/Custom/VystiaClasses/Systems/SongweavingSongs.cs`

## 3) Finales (Server)

Finales are Crescendo-spend effects.
- Sharp Note: single-target damage
- Mesmerise: single-target 4s paralyze
- Cacophony: AoE damage (6 tiles)
- Fortissimo: AoE damage (5 tiles)
- Soothing Chorus: party heal burst
- Symphony of Destruction: AoE damage (8 tiles)

Implementation:
- File: `ServUO/Scripts/Custom/VystiaClasses/Systems/SongweavingFinaleSystem.cs`

## 4) Spellbook “Spells” (Server)

The spellbook casts are thin wrappers that invoke songs/finales:
- `SongweavingSongSpellBase` calls `SongweavingRegistry.GetByKey(...).Begin(...)`
- `SongweavingFinaleSpellBase` calls `SongweavingFinaleSystem.BeginFinale(...)`
- File: `ServUO/Scripts/Custom/VystiaClasses/Spells/Songweaving/SongweavingSpells.cs`

## 5) Class Wiring (Server)

### Bard V2 class
- Secondary resource: Crescendo
- Starting item: Songweaving spellbook
- On class equip and runtime init: enable Crescendo tracker gump
- File: `ServUO/Scripts/Custom/VystiaClasses/Classes/PlayerClassV2.cs`

### Items / consumables
- Crescendo Crystal (resource potion)
- Crescendo Catalyst (craftable reagent)
- File: `ServUO/Scripts/Custom/VystiaClasses/Items/ResourceConsumables.cs`

## 6) Testing / GM Utilities (Server)

- GM testing gumps and spawners include Songweaving spellbook and Crescendo items.
- Files:
  - `ServUO/Scripts/Custom/VystiaClasses/Testing/VystiaTestKitGump.cs`
  - `ServUO/Scripts/Custom/VystiaClasses/Testing/VystiaSkillChestSpawner.cs`

## 7) Client Integration (ClassicUO)

Songweaving spellbook entries are defined in ClassicUO:
- `SpellsVystiaSongweaving.cs` (IDs 1384–1415)
- `SpellbookTypes.cs` includes `VystiaSongweaving` and spell range
- `SpellDefinition.cs` maps the spellbook range to Songweaving spell IDs
- `SpellbookGump.cs` has songbook name, hue mapping, and spellbook entry wiring

Files (ClassicUO):
- `ClassicUO.Client/Game/Data/SpellsVystiaSongweaving.cs`
- `ClassicUO.Client/Game/Data/SpellbookTypes.cs`
- `ClassicUO.Client/Game/Data/SpellDefinition.cs`
- `ClassicUO.Client/Game/UI/Gumps/SpellbookGump.cs`

## 8) Known Limitations in Current Code

- Channeling is not wired into song usage (no sustained songs).
- Crescendo decay is always active out of combat; channeling state is never set by songs.
- Song of Swiftness only applies Dexterity, not movement speed.
- Finale cooldown and song cooldown use the same 7s lockout.
- Spellbook entries are wrappers; actual gameplay logic lives in Songweaving systems.

