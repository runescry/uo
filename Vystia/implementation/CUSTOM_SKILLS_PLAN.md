# Vystia Custom Skills Implementation Plan

## Overview

This document outlines the plan to add custom class-specific skills to the Vystia shard. These skills will replace or supplement standard UO skills for the 26 Vystia classes.

---

## Proposed Custom Skills (26 total)

### Magic Class Skills (12)
| Skill ID | Skill Name | Class | Description |
|----------|------------|-------|-------------|
| 58 | Cryomancy | Ice Mage | Ice magic mastery, chill stack effectiveness |
| 59 | Demonology | Warlock | Dark pact magic, soul shard generation |
| 60 | Necromantic Arts | Necromancer | Undead control, life force manipulation |
| 61 | Druidism | Druid | Nature magic, shapeshifting effectiveness |
| 62 | Elementalism | Sorcerer | Elemental attunement, stance power |
| 63 | Bardic Lore | Bard | Song power, crescendo generation |
| 64 | Hexcraft | Witch | Curse potency, hex duration |
| 65 | Divination | Oracle | Foresight, time manipulation |
| 66 | Conjuration | Summoner | Summon power, bond strength |
| 67 | Spirit Calling | Shaman | Totem power, ancestor connection |
| 68 | Runeweaving | Enchanter | Enchantment power, rune crafting |
| 69 | Illusion | Illusionist | Illusion duration, misdirection |

### Martial Class Skills (14)
| Skill ID | Skill Name | Class | Description |
|----------|------------|-------|-------------|
| 70 | Berserking | Barbarian | Fury generation, rage duration |
| 71 | Subterfuge | Rogue | Combo point generation, stealth power |
| 72 | Martial Arts | Monk | Chi generation, stance effectiveness |
| 73 | Chivalric Arts | Knight | Fortitude generation, defensive power |
| 74 | Holy Devotion | Paladin | Virtue power, tithing efficiency |
| 75 | Marksmanship | Ranger | Focus generation, aspect power |
| 76 | Combat Mastery | Fighter | Stance effectiveness, weapon mastery |
| 77 | Zealotry | Templar | Zeal generation, holy strike power |
| 78 | Manhunting | Bounty Hunter | Pursuit generation, tracking power |
| 79 | Beast Bonding | Beastmaster | Pet power, beast control |
| 80 | Engineering | Artificer | Steam/charge generation, construct power |
| 81 | Transmutation | Alchemist | Potion power, mutagen effectiveness |
| 82 | Divine Grace | Cleric | Faith generation, healing power |
| 83 | Arcane Studies | Wizard | Spell mastery, arcane power |

---

## Implementation Phases

### Phase 1: Server-Side Skill Registration

#### 1.1 Extend SkillName Enum
**File:** `ServUO/Server/Skills.cs`

```csharp
public enum SkillName
{
    // ... existing skills 0-57 ...
    Throwing = 57,

    // Vystia Custom Skills (58-83)
    Cryomancy = 58,
    Demonology = 59,
    NecromancyArts = 60,  // Note: Different from existing Necromancy
    Druidism = 61,
    Elementalism = 62,
    BardicLore = 63,
    Hexcraft = 64,
    Divination = 65,
    Conjuration = 66,
    SpiritCalling = 67,
    Runeweaving = 68,
    IllusionMagic = 69,
    Berserking = 70,
    Subterfuge = 71,
    MartialArts = 72,
    ChivalricArts = 73,
    HolyDevotion = 74,
    Marksmanship = 75,
    CombatMastery = 76,
    Zealotry = 77,
    Manhunting = 78,
    BeastBonding = 79,
    Engineering = 80,
    Transmutation = 81,
    DivineGrace = 82,
    ArcaneStudies = 83
}
```

#### 1.2 Add Skill Info Definitions
**File:** `ServUO/Scripts/Misc/SkillInfo.cs` (or new file)

```csharp
// For each new skill, define:
new SkillInfo(58, "Cryomancy", 0.0, 0.0, 0.0, "Cryomancer", null, 0.0, 0.0, 0.0, 1.0),
// Parameters: id, name, strScale, dexScale, intScale, title, callback, strGain, dexGain, intGain, gainFactor
```

#### 1.3 Create Skill Use Handlers
**File:** `ServUO/Scripts/Custom/VystiaClasses/Skills/VystiaSkillHandlers.cs`

Define what happens when each skill is actively used (if applicable).

---

### Phase 2: Client-Side Support

#### 2.1 Skills Data File
**File:** `skills.mul` / `skills.idx`

Add entries for skills 58-83 with:
- Skill name strings
- Skill category assignment
- Button/icon references

#### 2.2 ClassicUO Modifications
**File:** `ClassicUO/src/ClassicUO.Client/Game/Data/SkillsLoader.cs`

Ensure ClassicUO recognizes skill IDs beyond 57:
- Extend skill count constant
- Add skill name mappings
- Handle skill gump display

#### 2.3 Skill Window UI
**File:** ClassicUO skill gump files

- Add Vystia skills to appropriate categories
- Or create new "Vystia" skill category
- Display custom skill icons if available

---

### Phase 3: Integration with Class System

#### 3.1 Update PlayerClassV2
**File:** `ServUO/Scripts/Custom/VystiaClasses/Classes/PlayerClassV2.cs`

```csharp
public abstract class PlayerClassV2
{
    // Add new property
    public abstract SkillName ClassSkill { get; }  // The primary custom skill

    // Modify existing
    public abstract SkillName[] PrimarySkills { get; }  // Include custom skill
}
```

#### 3.2 Update Class Definitions
Each class file updated to include their custom skill:

```csharp
public class IceMageClass : PlayerClassV2
{
    public override SkillName ClassSkill => SkillName.Cryomancy;

    public override SkillName[] PrimarySkills => new SkillName[]
    {
        SkillName.Cryomancy,  // Custom class skill
        SkillName.Magery,
        SkillName.EvalInt,
        SkillName.Meditation,
        SkillName.MagicResist,
        SkillName.Inscribe
    };
}
```

#### 3.3 Skill-Based Ability Scaling
**File:** `ServUO/Scripts/Custom/VystiaClasses/Abilities/AbilityExecutor.cs`

Abilities scale with class skill:
- Damage/healing values
- Duration of effects
- Resource generation rates
- Cooldown reduction

---

### Phase 4: Skill Gain System (UO-Style)

The standard UO skill gain system works as follows:
1. Player uses skill against a difficulty check (minSkill to maxSkill range)
2. Success chance = (currentSkill - minSkill) / (maxSkill - minSkill)
3. Gain chance based on: skill cap proximity + total skill cap proximity + gainFactor
4. Anti-macro code prevents spam training on same target/location

#### 4.1 Gain Triggers for Class Skills
**File:** `ServUO/Scripts/Custom/VystiaClasses/Skills/VystiaSkillGain.cs`

Each class skill gains through specific activities:

| Skill | Gain Triggers | Difficulty Scaling |
|-------|---------------|-------------------|
| **Cryomancy** | Cast ice spells, apply chill stacks, shatter frozen targets | Spell circle determines difficulty |
| **Demonology** | Cast dark spells, generate soul shards, summon demons | Enemy strength determines difficulty |
| **Necromantic Arts** | Raise undead, drain life, control corpses | Corpse power determines difficulty |
| **Druidism** | Shapeshift, cast nature spells, heal with nature | Form duration determines difficulty |
| **Elementalism** | Switch elements, cast elemental spells, trigger combos | Combo complexity determines difficulty |
| **Bardic Lore** | Play songs, build crescendo, affect enemies/allies | Number affected determines difficulty |
| **Hexcraft** | Apply curses, drain life, hex enemies | Curse potency determines difficulty |
| **Divination** | Cast divination spells, manipulate time | Spell complexity determines difficulty |
| **Conjuration** | Summon creatures, maintain bonds | Summon strength determines difficulty |
| **Spirit Calling** | Place totems, channel spirits | Spirit power determines difficulty |
| **Runeweaving** | Enchant items, inscribe runes | Item quality determines difficulty |
| **Illusion** | Create illusions, misdirect enemies | Illusion complexity determines difficulty |
| **Berserking** | Build fury, enter rage, deal rage damage | Fury spent determines difficulty |
| **Subterfuge** | Build combo points, execute finishers, stealth | Combo points spent determines difficulty |
| **Martial Arts** | Build chi, execute techniques, switch stances | Chi spent determines difficulty |
| **Chivalric Arts** | Block attacks, build fortitude, protect allies | Damage mitigated determines difficulty |
| **Holy Devotion** | Tithe gold, use virtues, smite evil | Gold tithed determines difficulty |
| **Marksmanship** | Hit ranged attacks, build focus, track prey | Range distance determines difficulty |
| **Combat Mastery** | Switch stances, execute stance combos | Stance mastery determines difficulty |
| **Zealotry** | Build zeal, execute holy strikes | Zeal spent determines difficulty |
| **Manhunting** | Track targets, build pursuit, execute bounties | Target difficulty determines difficulty |
| **Beast Bonding** | Command pets, heal pets, bond with beasts | Pet strength determines difficulty |
| **Engineering** | Build constructs, generate steam/charges | Construct complexity determines difficulty |
| **Transmutation** | Brew potions, create mutagens, transmute | Recipe difficulty determines difficulty |
| **Divine Grace** | Heal allies, build faith, channel divinity | Healing amount determines difficulty |
| **Arcane Studies** | Cast arcane spells, counter magic | Spell complexity determines difficulty |

#### 4.2 Skill Check Integration
**File:** `ServUO/Scripts/Custom/VystiaClasses/Skills/VystiaSkillCheck.cs`

```csharp
public static class VystiaSkillCheck
{
    /// <summary>
    /// Called when using a class ability - checks for skill gain
    /// </summary>
    public static bool CheckClassSkill(Mobile from, double difficulty)
    {
        if (!(from is PlayerMobile pm))
            return false;

        var playerClass = VystiaClassManager.Instance.GetClass(pm);
        if (playerClass == null)
            return false;

        SkillName classSkill = playerClass.ClassSkill;

        // Use standard UO skill check
        // minSkill = difficulty - 25, maxSkill = difficulty + 25
        double minSkill = Math.Max(0, difficulty - 25.0);
        double maxSkill = Math.Min(100.0, difficulty + 25.0);

        return from.CheckSkill(classSkill, minSkill, maxSkill);
    }

    /// <summary>
    /// Called when ability succeeds - guaranteed gain attempt
    /// </summary>
    public static void TriggerGainCheck(Mobile from, double difficulty)
    {
        if (!(from is PlayerMobile pm))
            return;

        var playerClass = VystiaClassManager.Instance.GetClass(pm);
        if (playerClass == null)
            return;

        SkillName classSkill = playerClass.ClassSkill;
        Skill skill = from.Skills[classSkill];

        if (skill == null)
            return;

        // Gain chance decreases as skill approaches cap
        double gc = (skill.Cap - skill.Base) / skill.Cap;
        gc *= 0.5; // Base 50% chance at 0 skill, decreasing

        if (Utility.RandomDouble() < gc)
        {
            skill.Base += 0.1;
            from.SendMessage(0x3B2, $"Your {skill.Info.Name} skill has increased!");
        }
    }
}
```

#### 4.3 Ability Integration Example
**File:** Ability execution code

```csharp
// In AbilityExecutor.Execute() or spell OnCast()
public void Execute(Mobile caster, AbilityDefinition ability, Mobile target)
{
    // ... ability logic ...

    // Calculate difficulty based on ability circle/level
    double difficulty = ability.Circle * 12.5; // Circle 1 = 12.5, Circle 8 = 100

    // Check for skill gain (UO-style)
    if (caster is PlayerMobile pm)
    {
        VystiaSkillCheck.CheckClassSkill(pm, difficulty);
    }

    // ... rest of ability ...
}
```

#### 4.4 Combat Gain Triggers
**File:** `ServUO/Scripts/Custom/VystiaClasses/Skills/VystiaCombatGain.cs`

```csharp
public static class VystiaCombatGain
{
    public static void Initialize()
    {
        // Hook into combat events
        EventSink.CreatureDeath += OnCreatureDeath;
    }

    private static void OnCreatureDeath(CreatureDeathEventArgs e)
    {
        Mobile killer = e.Killer;
        Mobile killed = e.Creature;

        if (!(killer is PlayerMobile pm))
            return;

        var playerClass = VystiaClassManager.Instance.GetClass(pm);
        if (playerClass == null)
            return;

        // Difficulty based on creature fame/karma/hits
        double difficulty = GetCreatureDifficulty(killed);

        // Gain check on kill
        VystiaSkillCheck.CheckClassSkill(pm, difficulty);
    }

    private static double GetCreatureDifficulty(Mobile creature)
    {
        // Scale 0-100 based on creature power
        double power = creature.HitsMax + (creature.Fame / 100.0);
        return Math.Min(100.0, power / 10.0);
    }
}
```

#### 4.5 Secondary Resource Gain Triggers

| Resource Action | Skill Gain Trigger |
|----------------|-------------------|
| Generate Soul Shard | Demonology check |
| Spend Soul Shard | Demonology check |
| Build Fury | Berserking check |
| Enter Rage | Berserking check |
| Build Chi | Martial Arts check |
| Spend Chi | Martial Arts check |
| Build Combo Points | Subterfuge check |
| Execute Finisher | Subterfuge check |
| Generate Faith | Divine Grace check |
| Apply Chill Stack | Cryomancy check |
| Shatter Frozen Target | Cryomancy check (bonus) |

#### 4.6 Skill Info Configuration
**File:** `ServUO/Scripts/Misc/SkillInfo.cs`

```csharp
// Each skill needs a SkillInfo entry defining:
// - GainFactor: How fast this skill gains (1.0 = normal, 0.5 = slower, 2.0 = faster)
// - Stat scaling: Which stats affect this skill
// - Title: What title players get at GM level

new SkillInfo(
    58,                    // Skill ID
    "Cryomancy",          // Name
    0.0,                   // STR scaling
    0.0,                   // DEX scaling
    1.0,                   // INT scaling (primary stat)
    "Cryomancer",         // Title at GM
    null,                  // Use callback
    0.0,                   // STR gain chance
    0.0,                   // DEX gain chance
    0.5,                   // INT gain chance
    1.0                    // Gain factor (1.0 = normal speed)
),
```

#### 4.7 Anti-Macro Protection
```csharp
// Add to UseAntiMacro array in SkillCheck.cs
// Custom skills should use anti-macro to prevent spam training
true, // Cryomancy = 58
true, // Demonology = 59
// ... etc for all 26 skills
```

#### 4.8 Skill Caps
- **Standard cap:** 100.0 (same as UO)
- **Power Scroll support:** 105.0, 110.0, 115.0, 120.0
- **Class skill only trainable:** While assigned to that class
- **Switching classes:** Skill remains but cannot gain further
- **Total skill cap:** 720.0 (standard) or higher for Vystia

#### 4.9 Training Methods Summary

| Method | Description | Difficulty |
|--------|-------------|------------|
| **Combat Use** | Cast spells, use abilities in combat | Based on ability circle |
| **Resource Generation** | Generate class secondary resource | Based on source difficulty |
| **Resource Spending** | Spend secondary resource on abilities | Based on ability cost |
| **Kills** | Defeat enemies appropriate to class | Based on enemy power |
| **Crafting** | Create class-specific items | Based on item difficulty |
| **Training Dummies** | Practice on training targets | Low difficulty, slower gains |
| **NPCs** | Pay NPC trainers | Instant gain to 30-50 skill |

---

### Phase 5: GM Tools

#### 5.1 Admin Commands
```
[SetSkill <skillname> <value>  - Set any skill including custom
[SkillInfo <skillname>         - Show skill info
[ListVystiaSkills              - List all custom skills
```

#### 5.2 VystiaAdminGump Integration
Add Skills tab to admin gump:
- Display all Vystia skills
- Set skill values
- Reset class skill
- View skill gain history

---

## File Changes Summary

### ServUO Core (Server/)
| File | Change |
|------|--------|
| `Server/Skills.cs` | Add 26 new SkillName enum values |

### ServUO Scripts
| File | Change |
|------|--------|
| `Scripts/Misc/SkillInfo.cs` | Add 26 skill info definitions |
| `Scripts/Custom/VystiaClasses/Skills/VystiaSkillHandlers.cs` | NEW - Skill use handlers |
| `Scripts/Custom/VystiaClasses/Skills/VystiaSkillGain.cs` | NEW - Skill gain logic |
| `Scripts/Custom/VystiaClasses/Classes/PlayerClassV2.cs` | Add ClassSkill property |
| `Scripts/Custom/VystiaClasses/Classes/*.cs` | Update all 26 class files |
| `Scripts/Custom/VystiaClasses/Abilities/AbilityExecutor.cs` | Add skill scaling |
| `Scripts/Custom/VystiaClasses/Gumps/VystiaAdminGump.cs` | Add Skills tab |

### ClassicUO
| File | Change |
|------|--------|
| `Game/Data/SkillsLoader.cs` | Extend skill ID range |
| `Game/UI/Gumps/SkillsGump.cs` | Display custom skills |

### Client Data Files
| File | Change |
|------|--------|
| `skills.mul` | Add 26 skill entries |
| `skills.idx` | Update index |

---

## Alternative: Pseudo-Skill System

If modifying core ServUO and client is too invasive, implement a **pseudo-skill system**:

### Concept
- Store "class proficiency" separately from UO skills
- Track in `VystiaClassManager` or player attachment
- Display in custom Vystia gumps (not native skill window)
- Use for ability scaling without touching SkillName enum

### Advantages
- No ServUO core modifications
- No client modifications needed
- Fully self-contained in VystiaClasses folder
- Easier to maintain and update

### Disadvantages
- Not visible in standard skill window
- Doesn't integrate with UO skill system
- Requires custom UI for everything

### Implementation
```csharp
public class VystiaProficiency
{
    public PlayerClassTypeV2 ClassType { get; set; }
    public double Value { get; set; }  // 0.0 - 100.0
    public double Cap { get; set; }    // Default 100.0

    public void CheckGain(Mobile m, double chance) { ... }
}

public class VystiaProficiencyManager
{
    private Dictionary<PlayerMobile, Dictionary<PlayerClassTypeV2, VystiaProficiency>> m_Proficiencies;

    public double GetProficiency(PlayerMobile pm, PlayerClassTypeV2 classType) { ... }
    public void SetProficiency(PlayerMobile pm, PlayerClassTypeV2 classType, double value) { ... }
    public void TriggerGainCheck(PlayerMobile pm) { ... }
}
```

---

## Recommendation

**Start with Pseudo-Skill System (Alternative):**
1. Faster to implement
2. No core/client modifications
3. Can be upgraded to real skills later
4. Fully functional for class gameplay

**Upgrade to Real Skills Later:**
1. When pseudo-system is proven
2. When client modification workflow is established
3. When ready for "release quality" integration

---

## Dependencies

- [ ] PlayerClassV2 system complete
- [ ] VystiaClassManager functional
- [ ] Ability system in place
- [ ] Admin gump framework ready

---

## Estimated Effort

| Phase | Effort | Dependencies |
|-------|--------|--------------|
| Phase 1 (Server skills) | Medium | None |
| Phase 2 (Client support) | High | ClassicUO build process |
| Phase 3 (Class integration) | Medium | Phase 1 |
| Phase 4 (Gain system) | Medium | Phase 3 |
| Phase 5 (GM tools) | Low | Phase 3 |
| **Alternative (Pseudo-skills)** | **Low-Medium** | **None** |

---

## Decision Required

Before implementation:
1. **Real skills** vs **Pseudo-skills** approach?
2. If real skills, are client modifications acceptable?
3. Skill cap values (100 vs 120)?
4. Cross-class skill policy?

---

*Document Created: 2025-12-10*
*Status: Planning*
