# Vystia Magic Schools - Complete Spell Design

This directory contains complete spell designs for all 12 unique magic schools in the Vystia character class system. Each school has a comprehensive 32-spell system organized into 8 circles (4 spells per circle).

**Total Spells Designed:** 384 spells across 12 schools
**Total Spells Implemented:** 32/384 (8.3%)
**Spellbooks Implemented:** ✅ 12/12 functional spellbooks
**Reagent System:** ✅ 96 unique reagents implemented
**Date Created:** 2025-12-05
**Last Updated:** 2025-12-06
**Status:** Ice Magic 100% complete (32/32 spells), 11 schools pending

---

## Magic School Index

### 1. Ice Magic (IceMagic.md)
- **Class:** Ice Mage
- **Region:** Frosthold
- **Theme:** Cold damage, slowing, ice barriers, area denial
- **Status:** ✅ 32/32 implemented (100% COMPLETE)
- **Implementation Date:** 2025-12-06
- **Key Features:**
  - Strong AoE damage (Blizzard, Absolute Zero, Ice Age)
  - Slow/freeze mechanics (multiple slow spells, freezes, roots)
  - Ice terrain and walls (Ice Wall, Frozen Ground, Eternal Winter)
  - Defensive ice barriers (Frost Armor, Ice Shield, Glacial Fortress)
  - Kiting playstyle (ranged attacks with slows)
  - Execute mechanics (Rime Reaper instant kill below 20% HP)
  - Summons (Ice Elemental)
  - Transformation (Fimbulwinter's Wrath)

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

### ✅ Completed
1. **Ice Magic** - ✅ 32/32 spells complete (100%)

### High Priority (Core Gameplay)
2. **Nature Magic** - Shapeshifting is unique feature (0/32)
3. **Hex Magic** - Core witch identity (0/32)
4. **Necromancy** - Undead summoning is popular (0/32)

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

### ✅ Vystia Magic Reagent System - FULLY IMPLEMENTED

**Status:** All 12 magic schools now use unique Vystia-specific reagents instead of standard UO reagents.

**Total Reagents:** 96 unique reagents (8 per school × 12 schools)
**Implementation:** All created in `ServUO/Scripts/Items/Vystia/Resources/Reagents/`
**Documentation:** See `VYSTIA_MAGIC_REAGENTS.md` for complete details

### Reagents by Magic School

1. **Ice Magic** - IceMagicReagents.cs (8 items)
   - Frostbloom, Glacier Crystal, Winterleaf, Permafrost Essence, Arctic Pearl, Frozen Soul, Eternal Ice, Heart of Winter

2. **Nature Magic** - NatureMagicReagents.cs (8 items)
   - Wild Moss, Moonpetal, Druid Bark, Treant Sap, Elderwood Seed, Primal Vine, Treant Heart, Living Bark

3. **Hex Magic** - HexMagicReagents.cs (8 items)
   - Bog Moss, Viper Fang, Witchweed, Toad's Eye, Hag's Hair, Swamp Lotus, Bog Iron Ore, Cursed Pearl

4. **Elemental Magic** - ElementalMagicReagents.cs (8 items)
   - Ash Petal, Lava Glass, Flameweed, Magma Essence, Phoenix Feather, Molten Ore, Everburning Coal, Primordial Ember

5. **Dark Magic** - DarkMagicReagents.cs (8 items)
   - Shadow Moss, Demon Scale, Void Weed, Chaos Shard, Void Dust, Shadow Silk, Demon Heart, Void Crystal

6. **Divination Magic** - DivinationMagicReagents.cs (8 items)
   - Crystal Dust, Prism Shard, Starlight Crystal, Ley Line Shard, Time Sand, Crystal Ore, Prismatic Shard, Fate Crystal

7. **Necromancy** - NecromancyReagents.cs (8 items)
   - Grave Moss, Bone Dust, Death Shroud, Soul Fragment, Corpse Ash, Voidstone Ore, Echoing Shard, Lich Dust

8. **Summoning Magic** - SummoningMagicReagents.cs (8 items)
   - Kelp Strand, Coral Fragment, Sea Glass, Leviathan Tooth, Siren Scale, Abyssal Pearl, Deepwater Ore, Kraken Ink

9. **Shamanic Magic** - ShamanicMagicReagents.cs (8 items)
   - Thunder Moss, Wind Crystal, Spirit Feather, Lightning Root, Storm Essence, Totem Carving, Windstone Ore, Primal Thunder

10. **Bardic Magic** - BardicMagicReagents.cs (8 items)
    - Song Petal, Echo Dust, Voice Crystal, Golden String, Harmony Gem, Muse Essence, Dragon Scale, Eternal Note

11. **Enchanting Magic** - EnchantingMagicReagents.cs (8 items)
    - Arcane Dust, Rune Fragment, Mana Crystal, Runic Powder, Enchanter's Ink, Aether Shard, Titan Rune, Essence of Magic

12. **Illusion Magic** - IllusionMagicReagents.cs (8 items)
    - Shadow Petal, Mirror Dust, Phantom Silk, Mirage Essence, Dream Crystal, Reality Splinter, Void Mirror, Chaos Prism

### Reagent Acquisition
- **Common (Circles 1-3):** Drop from common regional creatures
- **Uncommon (Circles 4-6):** Drop from elite regional mobs
- **Rare (Circles 7-8):** Drop from regional bosses only

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

### Phase 1: Core Implementation (Circles 1-3) ✅ COMPLETE FOR ICE MAGIC
1. ✅ Ice Magic Circle 1-8 complete (32/32 spells)
2. ⏳ Implement Circle 1-2 for Nature Magic (shapeshifting)
3. ⏳ Implement Circle 1-2 for Hex Magic (curses, life drain)
4. ⏳ Test basic gameplay loops

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
- ✅ 32 spells organized by circle
- ✅ Mana costs and Vystia-specific reagents (UPDATED!)
- ✅ Visual/sound effect IDs
- ✅ Damage formulas and scaling
- ✅ Unique mechanics explanation
- ✅ Reagent usage summary (ALL UPDATED with Vystia reagents!)
- ✅ Balance considerations
- ✅ Implementation notes

**Recent Updates:**
- **2025-12-06:**
  - ✅ **ICE MAGIC 100% COMPLETE** - All 32 spells implemented and functional
  - ✅ 29 new spell files created in `ServUO/Scripts/Custom/VystiaClasses/Spells/IceMage/`
  - ✅ All Ice Magic spells use correct ServUO API (MagerySpell inheritance)
  - ✅ All Ice Magic spells use Vystia reagents (Frostbloom, Winterleaf, etc.)
  - ✅ Updated IceMagic.md with actual implementation details and gameplay descriptions

- **2025-12-05:**
  - ✅ All 12 magic schools updated with Vystia-specific reagent sections
  - ✅ Standard UO reagents (Ginseng, MandrakeRoot, etc.) completely removed
  - ✅ 96 unique Vystia reagents created and documented
  - ✅ Fixed "BagsOfReagents" error in Necromancy.md
  - ✅ **Spellbooks Implemented:** All 12 functional spellbooks created
    - File: `ServUO/Scripts/Items/Equipment/Spellbooks/VystiaSpellbooks.cs`
    - GM Commands: [spellbook <type>] or [sb <type>]
    - See: `VYSTIA_SPELLBOOKS_IMPLEMENTATION.md` for details

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
