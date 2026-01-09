# Vystia Magic Schools

Detailed spell designs for all 12 magic schools.

**Quick Reference:** See `../reference/SPELLS.md` for spell ID lookup table.

---

## Status

| Item | Count | Status |
|------|-------|--------|
| Spells | 384/384 | ✅ 100% Complete |
| Spellbooks | 12/12 | ✅ All functional |
| Reagents | 96/96 | ✅ All implemented |
| Vendors | 14/14 | ✅ All implemented |

**Last Updated:** 2025-12-11

---

## Magic School Index

### Schools Overview

| # | School | Class | Theme | Spell IDs |
|---|--------|-------|-------|-----------|
| 1 | [Ice Magic](IceMagic.md) | Ice Mage | Cold, slowing, barriers | 1000-1031 |
| 2 | [Nature Magic](NatureMagic.md) | Druid | Shapeshifting, healing, plants | 1032-1063 |
| 3 | [Hex Magic](HexMagic.md) | Witch | Curses, debuffs, life drain | 1064-1095 |
| 4 | [Elemental Magic](ElementalMagic.md) | Sorcerer | Fire, explosive damage | 1096-1127 |
| 5 | [Dark Magic](DarkMagic.md) | Warlock | Demons, shadow, fear | 1128-1159 |
| 6 | [Divination Magic](DivinationMagic.md) | Oracle | Foresight, time, crystal | 1160-1191 |
| 7 | [Necromancy](Necromancy.md) | Necromancer | Undead, death, decay | 1192-1223 |
| 8 | [Summoning Magic](SummoningMagic.md) | Summoner | Creature summoning | 1224-1255 |
| 9 | [Shamanic Magic](ShamanicMagic.md) | Shaman | Totems, spirits | 1256-1287 |
| 10 | [Bardic Magic](BardicMagic.md) | Bard | Songs, sonic damage | 1288-1319 |
| 11 | [Enchanting Magic](EnchantingMagic.md) | Enchanter | Buffs, runes | 1320-1351 |
| 12 | [Illusion Magic](IllusionMagic.md) | Illusionist | Illusions, mind control | 1352-1383 |

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

## Other Documentation

- **VYSTIA_MAGIC_REAGENTS.md** - Complete reagent list
- **VYSTIA_SPELLBOOKS_IMPLEMENTATION.md** - Spellbook technical details
- **REAGENT_IMPLEMENTATION_SUMMARY.md** - Reagent implementation notes

---

*See individual school .md files for detailed spell designs.*
*Last Updated: 2025-12-11*
