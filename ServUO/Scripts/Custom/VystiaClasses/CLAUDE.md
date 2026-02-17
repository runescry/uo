# Vystia Class System - Development Patterns

This document covers patterns for implementing classes and abilities. For project overview, see `/CLAUDE.md`.

**Quick Reference:** See `Vystia/reference/` for class, spell, skill, and command tables.

---

## Directory Structure

```
Scripts/Custom/VystiaClasses/
├── Core/                      # Base class system
│   └── PlayerClass.cs         # Abstract base class
├── Classes/                   # Class implementations (25 classes)
├── Spells/                    # 384 spells across 12 magic schools
│   ├── VystiaSpellBase.cs     # Base spell with CheckFizzle
│   └── [SchoolName]/          # 32 spells per school
├── Items/                     # Custom ability items
│   ├── AbilityItems/          # RageTotem, ShapeshiftTotem, etc.
│   └── ClassSpecialItems.cs   # 11 consolidated items
├── README.md                  # Implementation guide
├── IMPLEMENTATION_SUMMARY.md  # Detailed status
└── KNOWN_ISSUES.md            # Bug tracking
```

---

## Pattern 1: Creating a New Class

```csharp
public class ExampleClass : PlayerClass
{
    public override PlayerClassType ClassType => PlayerClassType.Example;
    public override string ClassName => "Example";
    public override string ClassDescription => "...";

    // Stats (total ~80 points)
    public override int StartStr => 30;
    public override int StartDex => 25;
    public override int StartInt => 25;

    // Stat caps (customize per role)
    public override int StrCap => 125;
    public override int DexCap => 130;
    public override int IntCap => 120;

    // Skills (6 skills, ~240 total points)
    public override SkillName[] PrimarySkills => new SkillName[]
    {
        SkillName.AnimalTaming,
        SkillName.AnimalLore,
        SkillName.Veterinary,
        SkillName.Archery,
        SkillName.Tactics,
        SkillName.Tracking
    };

    public override double[] StartingSkillValues => new double[]
    {
        50.0, 50.0, 40.0, 35.0, 35.0, 30.0
    };

    // Equipment with regional hue
    public override void EquipStartingGear(Mobile m)
    {
        m.AddItem(new LeatherChest() { Name = "Example Jerkin", Hue = 1150 });
        // ... more equipment
        m.Backpack.DropItem(new FrozenOre(20)); // Use existing resources!
    }

    public override void GiveStartingAbilities(Mobile m)
    {
        // Give custom items if any
    }
}
```

**Key Guidelines:**
- Stats total ~80 points
- Skills total ~240 points across 6 skills
- Use regional hues (see Regional Hue table below)
- Use existing Vystia resources (see RESOURCE_CORRECTIONS.md)

---

## Pattern 2: Creating a Custom Ability Item

```csharp
// File: Items/AbilityItems/ExampleItem.cs
namespace Server.Items.VystiaClassItems
{
    public class ExampleItem : Item
    {
        private int m_Charges;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set { m_Charges = value; InvalidateProperties(); }
        }

        [Constructable]
        public ExampleItem() : base(0x1F14) // Item graphic ID
        {
            Name = "Example Item";
            Hue = 1150; // Regional hue
            Weight = 0.5;
            LootType = LootType.Blessed;
            m_Charges = 5;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
                return;
            }

            if (m_Charges <= 0)
            {
                from.SendMessage("No charges remaining.");
                return;
            }

            // Visual/sound effects
            from.FixedParticles(0x3709, 10, 30, 5052, 1150, 0, EffectLayer.Waist);
            from.PlaySound(0x22F);

            // Ability logic here
            m_Charges--;
            InvalidateProperties();
        }

        // REQUIRED: Serialization
        public ExampleItem(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Charges = reader.ReadInt();
        }
    }
}
```

**Key Guidelines:**
- Namespace: `Server.Items.VystiaClassItems`
- Always include `[CommandProperty]` for GM access
- Always include full serialization
- Use visual/sound effects for feedback

---

## Pattern 3: Vytia Spell with Skill Gains

```csharp
// File: Spells/[School]/ExampleSpell.cs
namespace Server.Spells.VystiaSpells.IceMage
{
    public class ExampleSpell : VystiaSpellBase
    {
        public override SkillName CastSkill => SkillName.Cryomancy; // Custom skill
        public override int RequiredMana => 9;
        public override SpellCircle Circle => SpellCircle.Third;

        // Damage types use cold, fire, energy, poison, or physical
        public override int DamageType => 100; // 100% cold

        public override void OnCast()
        {
            if (CheckFizzle()) // This handles skill gains!
            {
                // Spell succeeds - do effect
            }

            FinishSequence();
        }
    }
}
```

**Skill Gain via CheckFizzle:**
| Circle | Min Skill | Max Skill | Notes |
|--------|-----------|-----------|-------|
| 1 | 0 | 30 | Anyone can attempt |
| 2 | 0 | 40 | Gains up to 40 |
| 3 | 10 | 50 | Requires 10 to attempt |
| 4 | 20 | 60 | Requires 20 to attempt |
| 5 | 35 | 75 | Mid-level spells |
| 6 | 50 | 90 | Advanced spells |
| 7 | 65 | 100 | Near mastery |
| 8 | 80 | 120 | Grandmaster |

---

## Regional Hue Reference

| Region | Hue | Classes |
|--------|-----|---------|
| Frosthold | 1150-1153 | Barbarian, Ice Mage, Beastmaster |
| Emberlands | 1358 | Sorcerer |
| Desert | 1719 | Ranger, Illusionist |
| Shadowfen | 2073 | Witch |
| ShadowVoid | 1109 | Warlock, Necromancer |
| Verdantpeak | 2010 | Druid, Alchemist |
| Crystal Barrens | 1154 | Wizard, Oracle |
| Ironclad | 2305 | Artificer, Fighter, Monk, Templar |
| Underwater | 1365 | Summoner |
| Multi-Regional | 1153 | Cleric, Paladin, others |

---

## Common Resources (Use These!)

### Frosthold
FrozenOre, FrostforgedIngot, EternalIce, IceCrystal

### Ironclad
SteamworkOre, ClockworkIngot, ClockworkGear, ClockworkSpring, SteamCore

### Verdantpeak
LivingOre, NatureforgedIngot, LivingBark, TreantHeart

### Shadowfen
BogIronOre, MireforgedIngot, SwampLotus, VoidDust

**Full list:** See `RESOURCE_CORRECTIONS.md`

---

## Common Mistakes to Avoid

1. **Stats not totaling ~80**
   - Bad: 50+40+30=120
   - Good: 40+25+15=80

2. **Embedding items in class files**
   - Bad: Define RageTotem class inside BarbarianClass.cs
   - Good: Separate file in Items/AbilityItems/RageTotem.cs

3. **Wrong resource names**
   - Bad: `new FrostOre(20)` (doesn't exist)
   - Good: `new FrozenOre(20)` (correct)

4. **Missing serialization**
   - Always include Serialize/Deserialize for custom items

---

## Namespaces

- Classes: `Server.Custom.VystiaClasses`
- Custom items: `Server.Items.VystiaClassItems`
- Spells: `Server.Spells.VystiaSpells.[School]`

---

*Development patterns for class system. See `/CLAUDE.md` for project overview.*
*Last Updated: 2025-12-11*
