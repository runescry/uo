# Content Creation Guide

How to create quests, NPCs, and custom content for Vystia.

---

## Quest Creation

### Primary Method: Quest Wizard (Step-by-Step)

**Commands:** `[QuestEditor`, `[QE`, `[QuestWizard`, or `[QW`

The Quest Wizard is a step-by-step interface that allows GMs to create multi-waypoint quests without any coding:

1. **Open wizard:** `[QE` or `[QuestEditor`
2. **Select or Create Quest:** Choose existing quest to edit or create new
3. **Quest Details:** Set title, description, class requirement, tier
4. **Add Waypoints:** Create quest chain with sequential waypoints:
   - **Origin** - Starting point (where player accepts quest)
   - **Waypoint** - Intermediate steps (talk to NPC, reach location, defeat boss, etc.)
   - **BossCompletion** - Final boss fight
   - **NPCCompletion** - Final NPC dialogue
5. **Configure Each Waypoint:**
   - Set condition: `TalkToNPC`, `ReachLocation`, `DefeatBoss`, `CollectItems`, `CastSpell`, `RecruitSidekick`, or `Custom`
   - Set location (coordinates or POI ID)
   - Link NPCs (spawn QuestNPCs inline)
   - Add dialogue context for LLM NPCs
6. **Add Rewards:** Gold, skill points, items, titles
7. **Give Quest:** Assign quest to target player

**Features:**
- Multi-chain quests with sequential waypoints
- Inline NPC spawning (no need to leave wizard)
- LLM-powered quest NPCs with context-aware dialogue
- Auto-detection for `ReachLocation` and `DefeatBoss` waypoints
- XML persistence - all quests save to `Data/VystiaQuests.xml`

**Waypoint Conditions:**
- `TalkToNPC` - Player must speak to assigned NPC
- `ReachLocation` - Player must enter location/region (auto-detected)
- `DefeatBoss` - Player must kill specific boss (auto-detected)
- `CollectItems` - Player must gather items
- `CastSpell` - Player must cast specific spell
- `RecruitSidekick` - Player must recruit AI sidekick
- `Custom` - Custom objective key

---

### Quest Tiers

Vystia uses a 4-tier quest system:

| Tier | Level | Quests Per Class |
|------|-------|------------------|
| Initiation | 1-30 | 3 |
| Apprentice | 30-60 | 3 |
| Journeyman | 60-90 | 3 |
| Master | 90+ | 3 |

---

### Advanced: Code-Based Quests (Legacy)

For quests requiring custom logic that can't be handled by the wizard, you can create code-based quests:

**File:** `ServUO/Scripts/Custom/VystiaClasses/Quests/ClassQuests/<ClassName>Quests.cs`

```csharp
public class ExampleQuest : VystiaQuest
{
    public override string Title => "Quest Title";
    public override string Description => "Quest description text.";
    public override PlayerClassType RequiredClass => PlayerClassType.IceMage;
    public override QuestTier Tier => QuestTier.Initiation;
    public override int PrerequisiteQuestID => 0; // 0 = no prereq

    public ExampleQuest()
    {
        // Define objectives using dictionary keys (legacy system)
        Objectives["kill_creature"] = 10;
        Objectives["collect_item"] = 25;
        Objectives["talk_to_npc"] = 1;
    }

    public override void GiveRewards(PlayerMobile pm)
    {
        pm.Backpack?.DropItem(new Gold(500));
        pm.Skills[SkillName.Magery].Base += 5.0;
        pm.SendMessage("Quest complete!");
    }
}
```

**Note:** The Quest Wizard creates `DynamicQuest` objects which extend `VystiaQuest`. Code-based quests are for advanced use cases only. Most quests should be created via the wizard.

### Registering Code-Based Quests

```csharp
public static class ExampleQuestInitializer
{
    public static void Initialize()
    {
        VystiaQuestSystem.RegisterQuest(new ExampleQuest());
    }
}
```

---

## NPC Creation

### Class Trainer Template

**File:** `ServUO/Scripts/Mobiles/Vystia/Trainers/VystiaClassTrainers.cs`

All 25 class trainers are defined in a single file. To add a new trainer:

```csharp
public class ExampleTrainer : BaseCreature, IVystiaClassTrainer
{
    [Constructable]
    public ExampleTrainer() : base(AIType.AI_Vendor, FightMode.None, 10, 1, 0.2, 0.4)
    {
        Name = "Example Trainer";
        Title = "the Class Master";
        Body = 400; // Human male
        Hue = 1150; // Regional hue

        SetStr(100);
        SetDex(100);
        SetInt(100);

        AddItem(new Robe(1150));
    }

    public PlayerClassType ClassType => PlayerClassType.ExampleClass;

    public override void OnDoubleClick(Mobile from)
    {
        if (from is PlayerMobile pm)
        {
            // Open class selection or quest gump
            pm.SendGump(new VystiaClassSelectionGump(pm, this));
        }
    }
}
```

**Note:** Trainers can be spawned using `[spawntrainer <class>` or `[sat` for all trainers.

### Vendor Template

```csharp
public class ExampleVendor : BaseVendor
{
    private List<SBInfo> m_SBInfos = new List<SBInfo>();
    protected override List<SBInfo> SBInfos => m_SBInfos;

    [Constructable]
    public ExampleVendor() : base("the vendor")
    {
        Name = "Example Vendor";
        Hue = 1150;
    }

    public override void InitSBInfo()
    {
        m_SBInfos.Add(new SBExampleVendor());
    }
}

public class SBExampleVendor : SBInfo
{
    public override IShellEntry[] ShellEntries => new[]
    {
        new GenericBuyInfo(typeof(Frostbloom), 10, 100, 0x18E0, 1152),
        new GenericBuyInfo(typeof(GlacierCrystal), 15, 100, 0x1779, 1152),
    };
}
```

---

## Creature Creation

### Basic Creature

**File:** `ServUO/Scripts/Mobiles/Vystia/Creatures/<Region>/<CreatureName>.cs`

```csharp
public class ExampleCreature : BaseCreature
{
    [Constructable]
    public ExampleCreature() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
    {
        Name = "Example Creature";
        Body = 0x190; // Body ID
        Hue = 1150; // Regional hue
        BaseSoundID = 0x165;

        SetStr(150, 200);
        SetDex(100, 150);
        SetInt(50, 75);

        SetHits(150, 200);
        SetMana(0);

        SetDamage(10, 15);

        SetDamageType(ResistanceType.Physical, 50);
        SetDamageType(ResistanceType.Cold, 50);

        SetResistance(ResistanceType.Physical, 30, 40);
        SetResistance(ResistanceType.Cold, 50, 60);

        SetSkill(SkillName.MagicResist, 50.0);
        SetSkill(SkillName.Tactics, 60.0);
        SetSkill(SkillName.Wrestling, 60.0);

        Fame = 1000;
        Karma = -1000;
    }

    public override void GenerateLoot()
    {
        AddLoot(LootPack.Average);
        // Regional drops
        PackItem(new FrozenOre(Utility.RandomMinMax(1, 3)));
    }
}
```

### Boss Creature

Add special abilities and mechanics:

```csharp
public override void OnThink()
{
    base.OnThink();

    // Special attack at 50% health
    if (Hits < HitsMax / 2 && !m_HasUsedSpecial)
    {
        DoSpecialAttack();
        m_HasUsedSpecial = true;
    }
}
```

---

## Spell Creation

### Basic Spell

**File:** `ServUO/Scripts/Custom/VystiaClasses/Spells/<School>/<SpellName>.cs`

```csharp
public class ExampleSpell : VystiaSpellBase
{
    public override SkillName CastSkill => SkillName.Cryomancy;
    public override int RequiredMana => 15;
    public override SpellCircle Circle => SpellCircle.Third;

    public override void OnCast()
    {
        if (CheckFizzle()) // Handles skill check + gains
        {
            Caster.Target = new InternalTarget(this);
        }
        else
        {
            FinishSequence();
        }
    }

    public void Target(Mobile target)
    {
        if (CheckHSequence(target))
        {
            SpellHelper.Turn(Caster, target);

            // Visual effects
            target.FixedParticles(0x374A, 10, 15, 5013, 1152, 0, EffectLayer.Waist);
            target.PlaySound(0x1F4);

            // Damage
            int damage = Utility.RandomMinMax(20, 30);
            SpellHelper.Damage(this, target, damage, 0, 0, 100, 0, 0);
        }

        FinishSequence();
    }
}
```

---

## Item Creation

### Reagent

```csharp
public class ExampleReagent : Item
{
    [Constructable]
    public ExampleReagent() : this(1) { }

    [Constructable]
    public ExampleReagent(int amount) : base(0x0F88) // Stackable graphic
    {
        Name = "Example Reagent";
        Hue = 1152;
        Stackable = true;
        Amount = amount;
    }
}
```

### Equipment

```csharp
public class ExampleSword : Longsword
{
    [Constructable]
    public ExampleSword()
    {
        Name = "Frostbrand";
        Hue = 1152;

        Attributes.WeaponDamage = 25;
        WeaponAttributes.HitColdArea = 30;

        AosElementDamages[AosElementAttribute.Cold] = 50;
    }
}
```

---

## File Naming Conventions

| Type | Path | Example |
|------|------|---------|
| Class | `VystiaClasses/Classes/` | `IceMageClass.cs` |
| Trainer | `Mobiles/Vystia/Trainers/` | `VystiaClassTrainers.cs` (all in one file) |
| Quest (Code) | `VystiaClasses/Quests/ClassQuests/` | `IceMageQuests.cs` |
| Quest (Dynamic) | Created via `[QE` wizard | Saved to `Data/VystiaQuests.xml` |
| Quest NPC | Spawned via wizard | `QuestNPC.cs` (runtime spawned) |
| Creature | `Mobiles/Vystia/Creatures/<Region>/` | `FrostWolf.cs` |
| Spell | `VystiaClasses/Spells/<School>/` | `FrostBolt.cs` |
| Reagent | `Items/Vystia/Resources/Reagents/` | `IceMagicReagents.cs` |
| Equipment | `Items/Vystia/Equipment/` | `RegionalWeapons.cs` |

---

## Testing New Content

After creating content:

1. **Build ServUO:** `dotnet build`
2. **Check for errors** - Fix any compilation errors
3. **Login as GM**
4. **Spawn content:**
   - Creatures/Items: `[add <TypeName>`
   - Quest NPCs: Use `[aqn` or Quest Wizard
   - Quests: Use `[QE` to create and test
5. **Test functionality** - Verify all features work
6. **Iterate** - Make adjustments and rebuild

### Quest Testing Commands

| Command | Description |
|---------|-------------|
| `[QE` / `[QuestEditor` | Open Quest Wizard to create/edit quests |
| `[aqn` / `[addquestNPC` | Add quest NPC (opens wizard at NPC linking step) |
| `[FindQuestNPC` / `[FQNPC` | Find quest NPCs for your active quests |
| `[FindQuestNPC respawn` | Respawn missing quest NPCs |
| `[ClearQuests [playerName]` | Clear all quests for player |
| `[GenLLMQuest [poiId]` | Generate quest using LLM |
| `[DeleteQuest <questId>` | Delete a quest by ID |

### Additional Resources

- **Quest System Documentation:** `Vystia/implementation/QUEST_SYSTEM_COMPLETE.md`
- **Quest Quick Reference:** `Vystia/reference/QUEST_GENERATION_QUICK_REF.md`
- **GM Commands:** `Vystia/reference/COMMANDS.md`
