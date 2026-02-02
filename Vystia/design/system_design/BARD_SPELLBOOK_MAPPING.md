# Bard Spellbook + Songbar Mapping (Current vs Intended)

**Last Updated:** 2026-01-26  
**Scope:** Songweaving spellbook entries (Songweaving range 1384-1415) and Songbar entries.  
**Sources:**  
- `ServUO/Scripts/Custom/VystiaClasses/Spells/VystiaSpellInitializer.cs`  
- `ServUO/Scripts/Custom/VystiaClasses/Spells/Songweaving/SongweavingSpells.cs`  
- `ServUO/Scripts/Custom/VystiaClasses/Systems/SongweavingSongs.cs`  
- `Vystia/design/system_design/VYSTIA_BARD_SYSTEM.md`

---

## A) Spellbook Entries (IDs 1288–1319)

**Legend:**  
- **Current Invoked** = what the server triggers right now.  
- **Intended Invoked** = what the entry should invoke per design intent (Bardic name → Songweaving target).  

| Spell ID | Client Spell Name (Bardic) | Current Invoked (Code) | Intended Invoked (Design) | Notes |
|---|---|---|---|---|
| 1384 | Discordant Note | Discordance (Song) | Discordance | OK |
| 1385 | Song of Courage | Courage (Song) | Courage | OK |
| 1386 | Lullaby | Peacemaking (Song) | Peacemaking | OK |
| 1387 | Inspire Accuracy | Fortune (Song) | Fortune | OK (luck/accuracy theme) |
| 1388 | Sonic Burst | Sharp Note (Finale) | Sharp Note (Finale) | OK |
| 1389 | Song of Healing | Mending (Song) | Song of Healing | OK |
| 1390 | Dirge of Weakness | Requiem (Song) | Requiem | OK |
| 1391 | Song of Illumination | Light (Song) | Song of Illumination | OK |
| 1392 | Thunderous Chord | Fortissimo (Finale) | Fortissimo (Finale) | OK |
| 1393 | Song of Provocation | Provocation (Song) | Song of Provocation | OK |
| 1394 | Mesmerise | Mesmerise (Finale) | Mesmerise (Paralyze) | OK |
| 1395 | Song of Swiftness | Swiftness (Song) | Swiftness | OK |
| 1396 | Sonic Wave | Symphony of Destruction (Finale) | Symphony of Destruction (Finale) | OK |
| 1397 | Ballad of Health | Soothing Chorus (Finale) | Soothing Chorus (Finale) | OK |
| 1398 | Cacophony | Cacophony (Finale) | Cacophony (AoE damage) | OK |
| 1303–1319 | (Unused) | None | None | Reserved slots if needed |

---

## B) Songbar Entries (SongweavingRegistry Order)

**Legend:**  
- **Key** = command key used by `[song <name>]` (aliases allowed).  
- **Current Invoked** = Songweaving class invoked by the songbar.  
- **Intended Invoked** = desired design mapping if different.

| Order | Songbar Label (Bardic) | Key | Current Invoked (Code) | Intended Invoked (Design) | Notes |
|---|---|---|---|---|---|
| 1 | Song of Provocation | `provocation` | SongProvocation | Song of Provocation | OK |
| 2 | Lullaby | `peacemaking` | SongPeacemaking | Peacemaking | OK |
| 3 | Discordant Note | `discordance` | SongDiscordance | Discordance | OK |
| 4 | Dirge of Weakness | `requiem` | SongRequiem | Requiem | OK |
| 5 | Song of Healing | `mending` | SongMending | Song of Healing | OK |
| 6 | Song of Courage | `courage` | SongCourage | Courage | OK |
| 7 | Song of Swiftness | `swiftness` | SongSwiftness | Swiftness | OK |
| 8 | Song of Illumination | `light` | SongLight | Song of Illumination | OK |
| 9 | Inspire Accuracy | `fortune` | SongFortune | Fortune | OK |

---

## C) Finales (Gump + Spellbook Buttons)

| Finale | Current Invoked | Intended Invoked | Notes |
|---|---|---|---|
| Sharp Note | Single-target sonic strike | Sharp Note | OK |
| Mesmerise | Paralyze target (4s) | Mesmerise | OK |
| Cacophony | AoE sonic damage (6 tiles) | Cacophony | OK |
| Fortissimo | AoE sonic burst (5 tiles) | Fortissimo | OK |
| Soothing Chorus | Party heal burst | Soothing Chorus | OK |
| Symphony of Destruction | AoE sonic damage (8 tiles) | Symphony of Destruction | OK |

---

## D) Approved Fixes (Applied)

1) **Song of Healing** = Mending (party heal-over-time).  
2) **Song of Illumination** = Light (night sight).  
3) **Mesmerise** = Single-target paralyze (finale).  
4) **Cacophony** = AoE damage (finale).  
5) **Song of Provocation** = Provocation.