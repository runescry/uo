# Vystia Magic Schools - Complete Spell Design

This directory contains complete spell designs for all 12 unique magic schools in the Vystia character class system. Each school has a comprehensive 32-spell system organized into 8 circles (4 spells per circle).

**Total Spells Designed:** 384 spells across 12 schools
**Date Created:** 2025-12-05
**Status:** All designs complete, ready for implementation

---

## Magic School Index

### 1. Ice Magic (IceMagic.md)
- **Class:** Ice Mage
- **Region:** Frosthold
- **Theme:** Cold damage, slowing, ice barriers, area denial
- **Status:** 3/32 implemented (Ice Bolt, Frost Armor, Blizzard)
- **Key Features:**
  - Strong AoE damage
  - Slow/freeze mechanics
  - Ice terrain and walls
  - Defensive ice barriers
  - Kiting playstyle

### 2. Nature Magic (NatureMagic.md)
- **Class:** Druid
- **Region:** Verdantpeak
- **Theme:** Shapeshifting, healing, poison, plant control
- **Status:** 0/32 implemented
- **Key Features:**
  - 5 shapeshift forms (Bear, Wolf, Hawk, Treant, Hydra)
  - Strong healing over time
  - Poison damage and DoTs
  - Zone control with plants
  - Hybrid melee/caster

### 3. Hex Magic (HexMagic.md)
- **Class:** Witch
- **Region:** Shadowfen
- **Theme:** Curses, debuffs, life drain, hexes
- **Status:** 0/32 implemented
- **Key Features:**
  - Contagious hexes (spread between enemies)
  - Life drain for sustain
  - Anti-healing mechanics
  - Stat reduction curses
  - Voodoo magic

### 4. Elemental Magic (ElementalMagic.md)
- **Class:** Sorcerer
- **Region:** Emberlands
- **Theme:** Fire/lava, explosive damage, terrain hazards
- **Status:** 0/32 implemented
- **Key Features:**
  - Highest burst damage
  - Lava terrain effects
  - Ignite mechanics
  - AoE devastation
  - Fire elemental transformation

### 5. Dark Magic (DarkMagic.md)
- **Class:** Warlock
- **Region:** Shadowfen
- **Theme:** Demons, shadow damage, fear, chaos
- **Status:** 0/32 implemented
- **Key Features:**
  - 5 demon summon types
  - Soul shard mechanics
  - Chaos spells (RNG effects)
  - Fear-based CC
  - Sacrifice HP for power

### 6. Divination Magic (DivinationMagic.md)
- **Class:** Oracle
- **Region:** Crystal Barrens
- **Theme:** Foresight, crystal energy, time manipulation
- **Status:** 0/32 implemented
- **Key Features:**
  - Foresight (predict attacks)
  - Energy damage (ignores armor)
  - Time manipulation (rewind, slow, haste)
  - Powerful shields and barriers
  - Party support

### 7. Necromancy (Necromancy.md)
- **Class:** Necromancer
- **Region:** ShadowVoid
- **Theme:** Undead, death, decay, corpse magic
- **Status:** 0/32 implemented
- **Key Features:**
  - 6 undead summon types
  - Corpse explosion
  - Permanent summons
  - Life drain
  - Lich transformation

### 8. Summoning Magic (SummoningMagic.md)
- **Class:** Summoner
- **Region:** Underwater
- **Theme:** Creature summoning, elementals, beasts
- **Status:** 0/32 implemented
- **Key Features:**
  - Up to 5 active summons
  - 15+ creature types
  - Summon buffs and heals
  - Sacrifice summons for power
  - Elemental diversity

### 9. Shamanic Magic (ShamanicMagic.md)
- **Class:** Shaman
- **Region:** Wilderlands
- **Theme:** Totems, spirits, chain lightning, elementals
- **Status:** 0/32 implemented
- **Key Features:**
  - Totem placement (up to 4-8)
  - Chain spell mechanics
  - Spirit transformations
  - Hybrid melee/caster
  - Elemental fury

### 10. Bardic Magic (BardicMagic.md)
- **Class:** Bard
- **Region:** Multi-Regional
- **Theme:** Songs, sonic damage, buffs, crowd control
- **Status:** 0/32 implemented
- **Key Features:**
  - Channeled songs (persistent buffs)
  - Sonic damage (bypasses armor)
  - Party-wide buffs
  - AoE crowd control
  - Versatile support

### 11. Enchanting Magic (EnchantingMagic.md)
- **Class:** Enchanter
- **Region:** Multi-Regional
- **Theme:** Equipment buffs, runes, augmentation
- **Status:** 0/32 implemented
- **Key Features:**
  - Weapon enchantments
  - Armor enchantments
  - Rune crafting (consumables)
  - Mass party buffs
  - Disenchant enemies

### 12. Illusion Magic (IllusionMagic.md)
- **Class:** Illusionist
- **Region:** Desert
- **Theme:** Illusions, mind control, invisibility, trickery
- **Status:** 0/32 implemented
- **Key Features:**
  - Invisibility (4 tiers)
  - Illusory copies/decoys
  - Mind control
  - Confusion effects
  - Psychic damage

---

## Spell Circle System

All magic schools use the same 8-circle mana cost structure:

| Circle | Mana Cost | Spell Count | Power Level |
|--------|-----------|-------------|-------------|
| 1 | 4 | 4 spells | Basic |
| 2 | 6 | 4 spells | Lesser |
| 3 | 9 | 4 spells | Greater |
| 4 | 11 | 4 spells | Advanced |
| 5 | 14 | 4 spells | Master |
| 6 | 20 | 4 spells | Expert |
| 7 | 40 | 4 spells | Ancient |
| 8 | 50 | 4 spells | Legendary |

**Total Mana Costs per School:** 616 mana (all 32 spells)

---

## Implementation Priority

### High Priority (Core Gameplay)
1. **Ice Magic** - 3 spells already working, complete Circle 1-2
2. **Nature Magic** - Shapeshifting is unique feature
3. **Hex Magic** - Core witch identity
4. **Necromancy** - Undead summoning is popular

### Medium Priority (Unique Mechanics)
5. **Summoning Magic** - Complex but rewarding
6. **Shamanic Magic** - Totem system
7. **Bardic Magic** - Channeled songs
8. **Divination Magic** - Time manipulation

### Lower Priority (Advanced Systems)
9. **Elemental Magic** - Similar to standard fire magic
10. **Dark Magic** - Complex demon system
11. **Enchanting Magic** - Requires equipment system
12. **Illusion Magic** - Requires advanced AI for confusion

---

## Spell Design Patterns

### Damage Spell Template
```csharp
public class ExampleDamageSpell : MagerySpell
{
    private static readonly SpellInfo m_Info = new SpellInfo(
        "Spell Name", "Mantra Words",
        ActionID, EffectID,
        Reagent.Primary,
        Reagent.Secondary
    );

    public override SpellCircle Circle => SpellCircle.Third;

    public void Target(IDamageable target)
    {
        if (CheckHSequence(target))
        {
            double damage = GetNewAosDamage(19, 1, 5, target);
            SpellHelper.Damage(this, target, damage, 0, 100, 0, 0, 0); // Cold damage
        }
    }
}
```

### Buff Spell Template
```csharp
public class ExampleBuffSpell : MagerySpell
{
    public void Target(Mobile target)
    {
        if (CheckBSequence(target))
        {
            ResistanceMod mod = new ResistanceMod(ResistanceType.Physical, 10);
            target.AddResistanceMod(mod);

            Timer.DelayCall(TimeSpan.FromSeconds(duration), () =>
            {
                if (target != null && !target.Deleted)
                    target.RemoveResistanceMod(mod);
            });
        }
    }
}
```

### Summon Spell Template
```csharp
public class ExampleSummonSpell : MagerySpell
{
    public void Target(IPoint3D p)
    {
        if (CheckSequence())
        {
            BaseCreature summon = new ExampleCreature();
            summon.MoveToWorld(new Point3D(p), Caster.Map);
            summon.Summoned = true;
            summon.SummonMaster = Caster;
            summon.ControlOrder = OrderType.Follow;
        }
    }
}
```

---

## Reagent Summary

### Most Common Reagents (across all schools)
1. **MandrakeRoot** - 185 spells (offensive magic)
2. **Bloodmoss** - 170 spells (DoTs, sacrifices)
3. **Nightshade** - 145 spells (dark/hex/poison)
4. **BlackPearl** - 140 spells (high-tier spells)
5. **SpidersSilk** - 135 spells (CC and control)
6. **SulfurousAsh** - 110 spells (damage amplifiers)
7. **Ginseng** - 100 spells (healing)
8. **Garlic** - 85 spells (defensive)

### Special Vystia Reagents
- **FrozenOre, EternalIce** - Ice Magic
- **LivingBark, TreantHeart** - Nature Magic
- **SwampLotus, BogIronOre** - Hex Magic
- **MoltenOre, EverburningCoal** - Elemental Magic
- **VoidDust, ShadowSilk** - Dark Magic
- **CrystalOre, PrismaticShard** - Divination
- **VoidstoneOre, EchoingShard** - Necromancy
- **SirenScale, AbyssalPearl** - Summoning
- **StormEssence, WindstoneOre** - Shamanic

---

## Balance Philosophy

**PvE Considerations:**
- Early circles (1-3): Efficient farming spells
- Mid circles (4-5): Dungeon/group content
- Late circles (6-7): Boss encounters
- Legendary (8): Epic/raid content

**PvP Considerations:**
- Burst damage vs. sustained damage
- CC duration and diminishing returns
- Mana efficiency
- Counterplay and dispels
- Skill cap requirements

**Class Identity:**
- Each school has unique mechanics
- No two schools play the same
- Clear strengths and weaknesses
- Synergies within each school
- Combo potential

---

## Next Steps

### Phase 1: Core Implementation (Circles 1-3)
1. Implement Circle 1-3 for Ice Magic (already has 3 spells)
2. Implement Circle 1-2 for Nature Magic (shapeshifting)
3. Implement Circle 1-2 for Hex Magic (curses, life drain)
4. Test basic gameplay loops

### Phase 2: Mid-Tier Spells (Circles 4-6)
1. Complete Circle 4-6 for implemented schools
2. Add new mechanics (AoE, summons, transformations)
3. Balance mana costs and cooldowns
4. PvE dungeon testing

### Phase 3: High-Tier Spells (Circles 7-8)
1. Implement legendary spells
2. Add ultimate transformations
3. Boss encounter testing
4. PvP balancing

### Phase 4: Remaining Schools
1. Implement remaining 9 schools
2. Full spell roster completion
3. Cross-school balance
4. Final polish

---

## Documentation Standards

Each spell design document includes:
- ✓ 32 spells organized by circle
- ✓ Mana costs and reagents
- ✓ Visual/sound effect IDs
- ✓ Damage formulas and scaling
- ✓ Unique mechanics explanation
- ✓ Reagent usage summary
- ✓ Balance considerations
- ✓ Implementation notes

---

**Total Effort Estimate:**
- Design: ✅ Complete (12 schools, 384 spells)
- Implementation: ~300-500 hours (full roster)
- Testing: ~100-150 hours
- Balancing: ~50-80 hours

**Recommended Team Size:** 2-3 developers working in parallel

---

*Last Updated: 2025-12-05*
*All spell designs complete and ready for implementation*
*See individual .md files for detailed spell information*
