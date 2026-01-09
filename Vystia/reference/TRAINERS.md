# Vystia Class Trainers Reference

**Total Trainers:** 25 (one per class)
**Location:** `ServUO/Scripts/Mobiles/Vystia/Trainers/VystiaClassTrainers.cs`

---

## Trainer List

| # | Class | Trainer Type | Region | Skills Trained |
|---|-------|--------------|--------|----------------|
| 1 | Barbarian | BarbarianTrainer | Frosthold | Berserking (70) |
| 2 | Beastmaster | BeastmasterTrainer | Frosthold | BeastBonding (79) |
| 3 | Ice Mage | IceMageTrainer | Frosthold | Cryomancy (58) |
| 4 | Sorcerer | SorcererTrainer | Emberlands | Elementalism (62) |
| 5 | Ranger | RangerTrainer | Desert | Marksmanship (75) |
| 6 | Illusionist | IllusionistTrainer | Desert | IllusionMagic (69) |
| 7 | Witch | WitchTrainer | Shadowfen | Hexcraft (64) |
| 8 | Warlock | WarlockTrainer | ShadowVoid | Demonology (59) |
| 9 | Necromancer | NecromancerTrainer | ShadowVoid | NecromancyArts (60) |
| 10 | Druid | DruidTrainer | Verdantpeak | Druidism (61) |
| 11 | Alchemist | AlchemistTrainer | Verdantpeak | Transmutation (81) |
| 12 | Oracle | OracleTrainer | Crystal Barrens | Divination (65) |
| 13 | Wizard | WizardTrainer | Crystal Barrens | ArcaneStudies (83) |
| 14 | Artificer | ArtificerTrainer | Ironclad | Engineering (80) |
| 15 | Fighter | FighterTrainer | Ironclad | CombatMastery (76) |
| 16 | Monk | MonkTrainer | Ironclad | MartialArts (72) |
| 17 | Templar | TemplarTrainer | Ironclad | Zealotry (77) |
| 18 | Summoner | SummonerTrainer | Underwater | Conjuration (66) |
| 19 | Bounty Hunter | BountyHunterTrainer | Multi-Regional | Manhunting (78) |
| 20 | Knight | KnightTrainer | Multi-Regional | ChivalricArts (73) |
| 21 | Shaman | ShamanTrainer | Multi-Regional | SpiritCalling (67) |
| 22 | Cleric | ClericTrainer | Multi-Regional | DivineGrace (82) |
| 23 | Paladin | PaladinTrainer | Multi-Regional | HolyDevotion (74) |
| 24 | Bard | BardTrainer | Multi-Regional | BardicLore (63) |
| 25 | Enchanter | EnchanterTrainer | Multi-Regional | Runeweaving (68) |

---

## Trainer Features

### Class Selection
- Double-click trainer or say "join" to learn about class
- Trainers show class description and confirmation gump
- Only players without a class can join
- Class assignment is permanent (GM can reset)

### Speech Commands

| Keyword | Action |
|---------|--------|
| `join` / `train` / `class` | Open class selection dialog |
| `skill` / `learn` | Open skill training (same class only) |
| `hello` / `greet` | Trainer greeting message |

### Trainer Behavior
- Non-vendor (don't sell items)
- Teach class-specific skills
- Check if player already has a class
- Regional appearance and hues

---

## Spawn Commands

| Command | Description |
|---------|-------------|
| `[spawntrainer <class>` | Spawn specific trainer |
| `[str <class>` | Shortcut for above |
| `[spawnalltrainers` | Spawn all 25 trainers in circle |
| `[sat` | Shortcut for above |

### Examples

```
[spawntrainer IceMage
[str Barbarian
[spawnalltrainers
[sat
```

### Direct Add Commands

```
[add BarbarianTrainer
[add IceMageTrainer
[add DruidTrainer
```

---

## Trainers by Region

### Frosthold (3)
- BarbarianTrainer
- BeastmasterTrainer
- IceMageTrainer

### Emberlands (1)
- SorcererTrainer

### Desert of Surya (2)
- RangerTrainer
- IllusionistTrainer

### Shadowfen (1)
- WitchTrainer

### ShadowVoid (2)
- WarlockTrainer
- NecromancerTrainer

### Verdantpeak (2)
- DruidTrainer
- AlchemistTrainer

### Crystal Barrens (2)
- OracleTrainer
- WizardTrainer

### Ironclad Wastes (4)
- ArtificerTrainer
- FighterTrainer
- MonkTrainer
- TemplarTrainer

### Underwater (1)
- SummonerTrainer

### Multi-Regional (7)
- BountyHunterTrainer
- KnightTrainer
- ShamanTrainer
- ClericTrainer
- PaladinTrainer
- BardTrainer
- EnchanterTrainer

---

## Implementation Details

### Base Class
All trainers inherit from `VystiaClassTrainer` which extends `BaseVendor`:

```csharp
public abstract class VystiaClassTrainer : BaseVendor
{
    public abstract PlayerClassType TrainerClass { get; }
    public abstract string TrainerGreeting { get; }
    public abstract string ClassDescription { get; }
    public abstract SkillName[] TrainableSkills { get; }
}
```

### Class Assignment Flow
1. Player double-clicks trainer
2. Trainer checks if player has no class
3. Shows class description
4. Opens `VystiaClassConfirmationGump`
5. Player confirms choice
6. Class assigned via `PlayerMobile.VystiaClass`

---

*Last Updated: 2026-01-01*
