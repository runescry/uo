# Enchanting Magic - Multi-Regional School

**Class:** Enchanter
**Region:** Multi-Regional
**Theme:** Weapon/armor enchantments, augmentation, runes, item empowerment
**Spellbook:** Codex of Enhancement (EnchanterSpellbook.cs)
**Primary Stat:** Intelligence
**Hue:** 0x8FD (Arcane Purple)

---

## Overview

Enchanting Magic focuses on empowering weapons, armor, and allies with magical augmentations. Enchanters buff equipment, create runes, and manipulate magical properties.

**Strengths:**
- Powerful equipment buffs
- Long-duration enchantments
- Can buff entire party's equipment
- Rune creation for instant effects
- Disenchant enemy buffs

**Weaknesses:**
- Weak direct damage
- Buffs can be dispelled
- Requires items to enchant
- Long cast times
- Expensive mana costs

---

## Spell List (32 Spells)

### Circle 1 (4 mana)
1. **Magic Weapon** - +5 weapon damage for 60s
2. **Arcane Shield** - +5 armor for 60s
3. **Rune of Power** - Create rune: Instant +10% damage (30s)
4. **Detect Magic** - See magical auras, enchanted items

### Circle 2 (6 mana)
5. **Flaming Weapon** - Weapon deals +8 fire damage for 90s
6. **Fortify Armor** - +10 Physical Resist for 90s
7. **Disenchant** - Remove one buff from enemy
8. **Rune of Protection** - Create rune: Instant 50 damage shield

### Circle 3 (9 mana)
9. **Lightning Weapon** - Weapon has 25% chance to chain lightning (120s)
10. **Elemental Barrier** - +10 to all elemental resists (90s)
11. **Sharpen** - Weapon ignores 15 armor (120s)
12. **Rune of Healing** - Create rune: Instant heal 40-60 HP

### Circle 4 (11 mana)
13. **Vampiric Weapon** - Weapon drains 10% damage as HP (120s)
14. **Spell Reflection** - Armor reflects next spell (60s)
15. **Enchant Arrows** - 100 arrows deal +10 damage each
16. **Mass Disenchant** - Remove all buffs from target

### Circle 5 (14 mana)
17. **Holy Weapon** - Weapon deals double damage to undead/demons (180s)
18. **Aegis of Warding** - Absorbs next 100 damage (120s)
19. **Runic Empowerment** - All stats +15 (90s)
20. **Enchant Party Weapons** - All party weapons +10 damage (120s)

### Circle 6 (20 mana)
21. **Legendary Weapon** - Weapon +25 damage, +20% speed, +10% crit (180s)
22. **Invulnerability** - Armor grants 50% damage reduction (60s)
23. **Mass Enchant Weapons** - All allies' weapons gain element damage (120s)
24. **Rune of Resurrection** - Create rune: Instant self-resurrection

### Circle 7 (40 mana)
25. **Godly Weapon** - Weapon +50 damage, triple attack speed, life drain (240s)
26. **Prismatic Barrier** - +40 all resists, reflect damage (180s)
27. **Enchant Army** - All allies gain +20% damage, +15 armor (180s)
28. **Greater Disenchant** - Remove ALL buffs from all enemies (10 tile AoE)

### Circle 8 (50 mana)
29. **Artifact Empowerment** - Weapon becomes legendary: +100 damage, special effects (300s)
30. **Invincible Armor** - Armor grants immunity to physical damage (30s)
31. **Rune of Apocalypse** - Create rune: Instant AoE 150-250 damage
32. **Archmage's Blessing** - All enchantments last forever, can stack multiple (60s)

---

## Enchantment System

**Weapon Enchantments:**
- Only one weapon enchant active at a time
- Can be reapplied to refresh
- Works on melee and ranged weapons
- Types: Damage, Element, Effect (drain, chain, etc.)

**Armor Enchantments:**
- Only one armor enchant active at a time
- Stacks with resistance buffs
- Types: Resist, Absorb, Reflect

**Rune System:**
- Create consumable runes (stored in pack)
- Instant use (no cast time)
- Max 10 runes of each type
- Tradeable to other players

## Enchantment Types

**Damage Types:**
- Magic Weapon: Raw damage
- Flaming: Fire damage
- Lightning: Chain effect
- Holy: Anti-undead/demon
- Vampiric: Life drain

**Defense Types:**
- Arcane Shield: Armor boost
- Fortify Armor: Resistance boost
- Elemental Barrier: Elemental protection
- Spell Reflection: Reflect magic
- Invulnerability: Damage reduction

**Utility:**
- Sharpen: Armor penetration
- Enchant Arrows: Ammunition buff
- Runic Empowerment: Stat boost
- Mass enchants: Party-wide

## Reagents

Enchanting Magic uses the following Vystia-specific reagents organized by spell circle:

### Primary Reagents (6 types)
1. **Arcane Dust** (Circles 1-3) - Powdered magic essence from enchanter workshops
2. **Rune Fragment** (Circles 2-4) - Broken rune pieces from ancient rune circles
3. **Mana Crystal** (Circles 1-4) - Crystallized mana from mana wells
4. **Runic Powder** (Circles 3-6) - Ground rune essence from runeforges
5. **Enchanter's Ink** (Circles 5-7) - Magical inscription fluid from master libraries
6. **Aether Shard** (Circles 6-8) - Pure magic crystal from aetheric rifts

### Rare Reagents (2 types)
7. **Titan Rune** (Circles 7-8) - Ancient power rune from titan ruins
8. **Essence of Magic** (Circle 8) - Pure arcane energy, ultimate reagent

**Implementation:** All reagents created in `EnchantingMagicReagents.cs`

## Rune Crafting
**Circle 1:** Rune of Power
**Circle 2:** Rune of Protection
**Circle 3:** Rune of Healing
**Circle 6:** Rune of Resurrection
**Circle 8:** Rune of Apocalypse

Runes are:
- Created via spells
- Stored as items in backpack
- Instant use (double-click)
- Consume on use
- Can be given to allies

---

## ServUO Implementation Notes

### ✅ Standard Mechanics
- **Weapon/Armor Enchants:** Apply properties to equipped items via custom attributes
- **Stat Buffs:** `StatMod` for temporary stat bonuses
- **Resistance Buffs:** `ResistanceMod` for elemental protection

### 🔧 Custom Mechanics

**1. Weapon Enchantment System**
```csharp
public static class WeaponEnchantManager
{
    private static Dictionary<Mobile, WeaponEnchant> m_Enchants = new Dictionary<Mobile, WeaponEnchant>();

    public static void ApplyEnchant(Mobile m, WeaponEnchant enchant, TimeSpan duration)
    {
        // Remove existing enchant
        if (m_Enchants.ContainsKey(m))
            RemoveEnchant(m);

        m_Enchants[m] = enchant;
        m.SendMessage($"Your weapon is enchanted with {enchant.Name}!");

        Timer.DelayCall(duration, () => RemoveEnchant(m));
    }

    public static void RemoveEnchant(Mobile m)
    {
        if (m_Enchants.ContainsKey(m))
        {
            m_Enchants.Remove(m);
            m.SendMessage("Your weapon enchantment fades.");
        }
    }

    public static WeaponEnchant GetEnchant(Mobile m)
    {
        return m_Enchants.ContainsKey(m) ? m_Enchants[m] : null;
    }
}

public class WeaponEnchant
{
    public string Name;
    public int BonusDamage;
    public int FireDamage;
    public int LightningDamage;
    public double LifeDrainPercent;
    public bool ChainLightning;
}

// In weapon OnHit:
WeaponEnchant enchant = WeaponEnchantManager.GetEnchant(attacker);
if (enchant != null)
{
    if (enchant.BonusDamage > 0)
        damage += enchant.BonusDamage;

    if (enchant.FireDamage > 0)
        AOS.Damage(defender, attacker, enchant.FireDamage, 0, 100, 0, 0, 0);

    if (enchant.LifeDrainPercent > 0)
    {
        int drain = (int)(damage * enchant.LifeDrainPercent);
        attacker.Hits = Math.Min(attacker.Hits + drain, attacker.HitsMax);
    }
}
```

**2. Rune Creation (Consumable Items)**
```csharp
public class RuneOfPower : Item
{
    public RuneOfPower() : base(0x1F14) // Rune graphic
    {
        Name = "Rune of Power";
        Hue = 0x8FD;
        Weight = 0.1;
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (!IsChildOf(from.Backpack))
        {
            from.SendMessage("The rune must be in your backpack.");
            return;
        }

        // Apply instant +10% damage buff
        StatMod buff = new StatMod(StatType.Str, "RuneOfPower", 10, TimeSpan.FromSeconds(30.0));
        from.AddStatMod(buff);
        from.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
        from.SendMessage("You activate the Rune of Power!");

        Delete(); // Consume rune
    }
}

public class RuneOfProtection : Item
{
    public RuneOfProtection() : base(0x1F14)
    {
        Name = "Rune of Protection";
        Hue = 0x8FD;
        Weight = 0.1;
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (!IsChildOf(from.Backpack))
        {
            from.SendMessage("The rune must be in your backpack.");
            return;
        }

        // Apply 50 damage shield
        CrystalShieldManager.ApplyShield(from, 50);
        from.FixedParticles(0x376A, 9, 32, 5008, EffectLayer.Waist);
        from.SendMessage("You activate the Rune of Protection!");

        Delete();
    }
}
```

**3. Flaming Weapon (Fire Damage)**
```csharp
public void EnchantFlamingWeapon(Mobile target)
{
    WeaponEnchant enchant = new WeaponEnchant
    {
        Name = "Flaming Weapon",
        FireDamage = 8
    };

    WeaponEnchantManager.ApplyEnchant(target, enchant, TimeSpan.FromSeconds(90.0));
    target.FixedParticles(0x3709, 10, 30, 5052, EffectLayer.RightHand);
}
```

**4. Spell Reflection**
```csharp
public static class SpellReflectionManager
{
    private static HashSet<Mobile> m_Protected = new HashSet<Mobile>();

    public static void ApplyReflection(Mobile m, TimeSpan duration)
    {
        m_Protected.Add(m);
        m.SendMessage("The next spell cast on you will be reflected!");

        Timer.DelayCall(duration, () =>
        {
            if (m_Protected.Contains(m))
            {
                m_Protected.Remove(m);
                m.SendMessage("Your spell reflection fades.");
            }
        });
    }

    public static bool CheckReflection(Mobile defender, Mobile attacker, Spell spell)
    {
        if (m_Protected.Contains(defender))
        {
            m_Protected.Remove(defender);
            defender.SendMessage("You reflect the spell!");
            attacker.SendMessage("Your spell was reflected!");

            // Cast spell back at attacker
            // (Complex - requires spell redirection)
            return true;
        }
        return false;
    }
}
```

**5. Mass Disenchant (Remove All Buffs)**
```csharp
public void MassDisenchant(Mobile target)
{
    // Remove all stat mods
    List<StatMod> toRemove = new List<StatMod>();
    foreach (StatMod mod in target.StatMods)
        toRemove.Add(mod);

    foreach (StatMod mod in toRemove)
        target.RemoveStatMod(mod.Name);

    // Remove all resistance mods
    List<ResistanceMod> resistToRemove = new List<ResistanceMod>();
    foreach (ResistanceMod mod in target.ResistanceMods)
        resistToRemove.Add(mod);

    foreach (ResistanceMod mod in resistToRemove)
        target.RemoveResistanceMod(mod);

    target.FixedParticles(0x374A, 10, 15, 5013, EffectLayer.Waist);
    target.SendMessage("All your enchantments are stripped away!");
}
```

**6. Legendary Weapon (Multiple Buffs)**
```csharp
public void EnchantLegendaryWeapon(Mobile target)
{
    WeaponEnchant enchant = new WeaponEnchant
    {
        Name = "Legendary Weapon",
        BonusDamage = 25
        // Note: Attack speed and crit require custom weapon handling
    };

    WeaponEnchantManager.ApplyEnchant(target, enchant, TimeSpan.FromSeconds(180.0));

    // Apply attack speed buff via DEX
    StatMod speedBuff = new StatMod(StatType.Dex, "LegendarySpeed", 20, TimeSpan.FromSeconds(180.0));
    target.AddStatMod(speedBuff);
}
```

### ⚠️ Advanced Mechanics
- **Invincible Armor (Immunity):** Hook into damage, return 0 for 30s
- **Artifact Empowerment:** Massive weapon buffs, special effects (requires custom properties)
- **Archmage's Blessing:** Enchantments last forever - remove timer expiration

### 📝 Implementation Priority
**Phase 1:** Magic Weapon, Arcane Shield, Detect Magic, Rune of Power
**Phase 2:** Flaming Weapon, Disenchant, Rune system
**Phase 3:** Spell Reflection, Mass Disenchant, Legendary Weapon
**Phase 4:** Artifact Empowerment, Invincible Armor, Archmage's Blessing

**Last Updated:** 2025-12-05 | **Status:** Design Complete - 0/32 Implemented
