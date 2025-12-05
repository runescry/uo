# Hex Magic - Shadowfen School

**Class:** Witch
**Region:** Shadowfen
**Theme:** Curses, debuffs, poison, life drain, dark rituals, voodoo
**Spellbook:** Grimoire of Hexes (WitchSpellbook.cs)
**Primary Stat:** Intelligence
**Hue:** 0x81D (Murky Green/Purple)

---

## Overview

Hex Magic draws upon the dark swamp energies of Shadowfen, specializing in curses, debuffs, and life-draining abilities. Witches excel at weakening enemies, spreading contagious hexes, and sustaining themselves through life drain.

**Strengths:**
- Powerful debuffs and curses
- Life drain for survivability
- Hexes spread between enemies
- Strong anti-healing capabilities
- Excellent sustained damage via DoTs

**Weaknesses:**
- Low burst damage
- Weak against holy/light magic
- Many spells require setup time
- Less effective against undead (no life drain)

---

## Spellbook Design

### WitchSpellbook (Grimoire of Hexes)
- **Item ID:** 0xFF0 (Necromancer book)
- **Hue:** 0x81D (Murky Green/Purple)
- **Name:** "Grimoire of Shadowfen Hexes"
- **Capacity:** 32 spells
- **Weight:** 3.0 stones
- **Special:** Hexes last 20% longer when equipped

---

## Spell List (32 Spells)

### Circle 1 (4 mana) - Minor Hexes
*Mana: 4 | Cast Time: 0.5-1.0s*

#### 1. Evil Eye
- **Mana:** 4
- **Reagents:** Nightshade
- **Cast Time:** 0.5s
- **Range:** 12 tiles
- **Duration:** 15 seconds
- **Effect:** -5% accuracy
- **Visual:** Purple eye above target (0x3779, purple hue)
- **Sound:** 0x1F8
- **Purpose:** Basic accuracy debuff

#### 2. Weak Curse
- **Mana:** 4
- **Reagents:** Garlic, Nightshade
- **Cast Time:** 0.75s
- **Range:** 10 tiles
- **Duration:** 20 seconds
- **Effect:** -5 to all stats (STR/DEX/INT)
- **Visual:** Dark aura (0x374A, dark hue)
- **Sound:** 0x1FC
- **Purpose:** Basic stat reduction

#### 3. Siphon Life
- **Mana:** 4
- **Reagents:** Ginseng, Nightshade
- **Cast Time:** 1.0s
- **Range:** 8 tiles
- **Effect:** Drains 6-12 HP from target, heals caster for same amount
- **Visual:** Red beam to caster (0x374A)
- **Sound:** 0x1FB
- **Purpose:** Basic life drain

#### 4. Witch Sight
- **Mana:** 4
- **Reagents:** SpidersSilk
- **Cast Time:** 0.75s
- **Duration:** 60 seconds
- **Effect:** Reveals hidden creatures, see in darkness, detect magic auras
- **Visual:** Purple glow in eyes (0x373A)
- **Sound:** 0x1EA
- **Purpose:** Detection and vision

---

### Circle 2 (6 mana) - Lesser Hexes
*Mana: 6 | Cast Time: 1.0-1.5s*

#### 5. Wasting Curse
- **Mana:** 6
- **Reagents:** Nightshade, Bloodmoss
- **Cast Time:** 1.25s
- **Range:** 10 tiles
- **Duration:** 30 seconds
- **Effect:** Target loses 1% max HP every 3 seconds (max 10% loss)
- **Visual:** Sickly green aura (0x372A)
- **Sound:** 0x1FC
- **Purpose:** HP reduction over time

#### 6. Poison Touch
- **Mana:** 6
- **Reagents:** Nightshade, MandrakeRoot
- **Cast Time:** 1.0s
- **Range:** 10 tiles
- **Duration:** 12 seconds
- **Effect:** Applies Greater Poison (5-8 damage/tick)
- **Visual:** Green cloud (0x372A)
- **Sound:** 0x205
- **Purpose:** Strong poison application

#### 7. Enfeeble
- **Mana:** 6
- **Reagents:** Garlic, Nightshade, Bloodmoss
- **Cast Time:** 1.5s
- **Range:** 10 tiles
- **Duration:** 25 seconds
- **Effect:** -15 STR, -10% physical damage output
- **Visual:** Purple weakness effect (0x374A)
- **Sound:** 0x1FC
- **Purpose:** Damage reduction debuff

#### 8. Dark Pact
- **Mana:** 6
- **Reagents:** Bloodmoss, BlackPearl
- **Cast Time:** 1.5s
- **Duration:** 45 seconds
- **Effect:** Sacrifice 15% current HP, gain +20% spell damage for duration
- **Visual:** Dark red aura (0x373A)
- **Sound:** 0x1FB
- **Purpose:** Self-damage for power

---

### Circle 3 (9 mana) - Greater Hexes
*Mana: 9 | Cast Time: 1.5-2.0s*

#### 9. Contagious Hex
- **Mana:** 9
- **Reagents:** Nightshade, Bloodmoss, SpidersSilk
- **Cast Time:** 2.0s
- **Range:** 12 tiles
- **Duration:** 20 seconds
- **Effect:** Cursed target spreads hex to allies within 3 tiles (-10% attack speed, 3-5 damage/tick)
- **Visual:** Purple cloud spreading (0x372A + 0x3779)
- **Sound:** 0x1FC
- **Purpose:** Spreading debuff

#### 10. Life Leech
- **Mana:** 9
- **Reagents:** Ginseng, Nightshade, Bloodmoss
- **Cast Time:** 1.75s
- **Range:** 10 tiles
- **Effect:** Drains 15-25 HP from target, heals caster for 150% of amount drained
- **Visual:** Red beam (0x374A)
- **Sound:** 0x1FB
- **Purpose:** Strong life steal

#### 11. Hex of Frailty
- **Mana:** 9
- **Reagents:** Garlic, Nightshade, MandrakeRoot
- **Cast Time:** 1.5s
- **Range:** 10 tiles
- **Duration:** 30 seconds
- **Effect:** -10 to all resistances
- **Visual:** Purple cracks on target (0x36BD)
- **Sound:** 0x1FC
- **Purpose:** Resistance shredding

#### 12. Voodoo Doll
- **Mana:** 9
- **Reagents:** Bloodmoss, SpidersSilk, MandrakeRoot
- **Cast Time:** 2.0s
- **Range:** 12 tiles
- **Duration:** 25 seconds
- **Effect:** Links to target - 25% of damage caster takes is reflected to linked target
- **Visual:** Shadowy link (0x373A line)
- **Sound:** 0x1FB
- **Purpose:** Damage reflection

---

### Circle 4 (11 mana) - Vile Hexes
*Mana: 11 | Cast Time: 2.0-2.5s*

#### 13. Crippling Curse
- **Mana:** 11
- **Reagents:** Garlic, Nightshade, Bloodmoss, MandrakeRoot
- **Cast Time:** 2.25s
- **Range:** 10 tiles
- **Duration:** 35 seconds
- **Effect:** -20 DEX, -30% movement speed, -20% attack speed
- **Visual:** Dark chains (0x373A)
- **Sound:** 0x1FC
- **Purpose:** Severe mobility reduction

#### 14. Plague Bearer
- **Mana:** 11
- **Reagents:** Nightshade, Bloodmoss, SulfurousAsh
- **Cast Time:** 2.0s
- **Range:** 10 tiles
- **Duration:** 20 seconds
- **Effect:** Target takes 6-10 poison damage/tick and spreads poison to enemies within 2 tiles
- **Visual:** Sickly green aura (0x372A + particles)
- **Sound:** 0x205
- **Purpose:** Spreading poison

#### 15. Drain Essence
- **Mana:** 11
- **Reagents:** Ginseng, Nightshade, Bloodmoss, BlackPearl
- **Cast Time:** 2.5s
- **Range:** 10 tiles
- **Effect:** Drains 25-40 HP AND 10-20 mana, heals caster for HP amount and restores mana
- **Visual:** Purple/red dual beam (0x374A)
- **Sound:** 0x1FB
- **Purpose:** Life AND mana drain

#### 16. Hex of Agony
- **Mana:** 11
- **Reagents:** Nightshade, Bloodmoss, SulfurousAsh, MandrakeRoot
- **Cast Time:** 2.0s
- **Range:** 10 tiles
- **Duration:** 15 seconds
- **Effect:** Whenever target is healed, they take equal damage instead
- **Visual:** Twisted purple aura (0x374A + 0x3779)
- **Sound:** 0x1FC
- **Purpose:** Anti-healing hex

---

### Circle 5 (14 mana) - Cursed Arts
*Mana: 14 | Cast Time: 2.5-3.0s*

#### 17. Mass Hex
- **Mana:** 14
- **Reagents:** Nightshade, Bloodmoss, MandrakeRoot, SpidersSilk
- **Cast Time:** 2.75s
- **Range:** 12 tiles
- **Area:** 5 tile radius
- **Duration:** 25 seconds
- **Effect:** All enemies cursed: -10 all stats, -10% damage, -5 all resists
- **Visual:** Purple explosion (0x36BD)
- **Sound:** 0x1FC
- **Purpose:** AoE mass debuff

#### 18. Soul Siphon
- **Mana:** 14
- **Reagents:** Ginseng, Nightshade, Bloodmoss, BlackPearl
- **Cast Time:** 2.5s
- **Range:** 10 tiles
- **Effect:** Drains 35-55 HP, heals caster for 200% of amount, grants temporary +10% max HP buff (60s)
- **Visual:** Intense red beam (0x374A)
- **Sound:** 0x1FB
- **Purpose:** Massive life steal with buff

#### 19. Hex of Silence
- **Mana:** 14
- **Reagents:** Nightshade, SpidersSilk, MandrakeRoot, Bloodmoss
- **Cast Time:** 2.5s
- **Range:** 10 tiles
- **Duration:** 8 seconds
- **Effect:** Target cannot cast spells (silenced)
- **Visual:** Purple gag effect (0x3779)
- **Sound:** 0x1FC
- **Purpose:** Hard CC (silence)

#### 20. Necrotic Touch
- **Mana:** 14
- **Reagents:** Nightshade, Bloodmoss, SulfurousAsh, MandrakeRoot
- **Cast Time:** 2.5s
- **Range:** 10 tiles
- **Effect:** Deals 25-40 damage, reduces healing received by 50% for 20 seconds, applies Deadly Poison
- **Visual:** Black/green hand (0x36BD)
- **Sound:** 0x205
- **Purpose:** Multi-layered damage debuff

---

### Circle 6 (20 mana) - Master Hexes
*Mana: 20 | Cast Time: 3.0-3.5s*

#### 21. Curse of Mortality
- **Mana:** 20
- **Reagents:** Nightshade, Bloodmoss, MandrakeRoot, BlackPearl, Garlic
- **Cast Time:** 3.25s
- **Range:** 10 tiles
- **Duration:** 40 seconds
- **Effect:** Target loses ALL regeneration (HP/Mana/Stam), -20 all resists, -15% max HP
- **Visual:** Death shroud (0x374A, dark)
- **Sound:** 0x1FC
- **Purpose:** Ultimate anti-sustain

#### 22. Hex Storm
- **Mana:** 20
- **Reagents:** Nightshade, Bloodmoss, MandrakeRoot, SpidersSilk, SulfurousAsh
- **Cast Time:** 3.0s
- **Range:** 12 tiles (ground target)
- **Area:** 6 tile radius
- **Duration:** 15 seconds
- **Effect:** Zone pulses random hexes every 2s: curses, poisons, slows, silences
- **Visual:** Chaotic purple storm (0x3779 + 0x372A)
- **Sound:** 0x1FC (looping)
- **Purpose:** Zone chaos

#### 23. Vampiric Aura
- **Mana:** 20
- **Reagents:** Ginseng, Nightshade, Bloodmoss, BlackPearl, MandrakeRoot
- **Cast Time:** 3.5s
- **Duration:** 30 seconds
- **Effect:** All damage dealt by caster heals caster for 40% of damage, +15% spell damage
- **Visual:** Red pulsing aura (0x373A)
- **Sound:** 0x1FB
- **Purpose:** Sustain ultimate

#### 24. Doomcurse
- **Mana:** 20
- **Reagents:** All 8 reagents
- **Cast Time:** 3.0s
- **Range:** 10 tiles
- **Duration:** 12 seconds
- **Effect:** After 12 seconds, target takes massive damage (100-150) unless dispelled. Cannot be removed by normal means.
- **Visual:** Skull above target, countdown (0x3779 → skull)
- **Sound:** 0x1FC
- **Purpose:** Delayed nuke/pressure

---

### Circle 7 (40 mana) - Ancient Hexes
*Mana: 40 | Cast Time: 3.5-4.0s*

#### 25. Plague of Sorrows
- **Mana:** 40
- **Reagents:** All 8 reagents
- **Cast Time:** 4.0s
- **Range:** 12 tiles
- **Area:** 8 tile radius
- **Duration:** 25 seconds
- **Effect:** Contagious mega-plague: 10-18 damage/tick, spreads to anyone within 5 tiles, reduces healing 75%
- **Visual:** Massive sickly cloud (0x372A + fog)
- **Sound:** 0x205
- **Purpose:** Spreading devastation

#### 26. Soul Harvest
- **Mana:** 40
- **Reagents:** Ginseng, Nightshade, Bloodmoss, BlackPearl, MandrakeRoot, SulfurousAsh
- **Cast Time:** 3.5s
- **Range:** 12 tiles
- **Area:** 6 tile radius
- **Effect:** Drains 40-65 HP from all enemies, heals caster for total amount drained, grants shield equal to healing
- **Visual:** Multiple red beams converging (0x374A)
- **Sound:** 0x1FB
- **Purpose:** AoE massive drain

#### 27. Curse of the Hag
- **Mana:** 40
- **Reagents:** All 8 reagents + SwampLotus (10)
- **Cast Time:** 3.75s
- **Range:** 10 tiles
- **Duration:** 45 seconds
- **Effect:** Target polymorphs into weak form (chicken), -90% damage, -50% movement, cannot cast spells
- **Visual:** Transform into chicken (Body 0x00D0)
- **Sound:** 0x6E (chicken sound)
- **Purpose:** Ultimate debilitation

#### 28. Hexblade Ritual
- **Mana:** 40
- **Reagents:** All 8 reagents
- **Cast Time:** 4.0s
- **Duration:** 60 seconds
- **Effect:** Caster's spells deal bonus physical damage equal to 50% of spell damage, melee attacks apply hexes
- **Visual:** Purple weapon glow (0x3779)
- **Sound:** 0x1FC
- **Purpose:** Hybrid combat mode

---

### Circle 8 (50 mana) - Legendary Hexes
*Mana: 50 | Cast Time: 4.0-5.0s*

#### 29. Curse of Undeath
- **Mana:** 50
- **Reagents:** All 8 reagents + BogIronOre (10)
- **Cast Time:** 5.0s
- **Range:** 10 tiles
- **Duration:** Permanent (until death/dispelled)
- **Effect:** Target becomes cursed undead: heals become damage, holy spells deal double damage, cannot naturally regenerate, +50% poison/curse damage taken
- **Visual:** Undead transformation effect (0x374A + skeleton)
- **Sound:** 0x1FB
- **Purpose:** Ultimate curse

#### 30. Voodoo Mastery
- **Mana:** 50
- **Reagents:** All 8 reagents
- **Cast Time:** 4.5s
- **Range:** 14 tiles
- **Duration:** 20 seconds
- **Effect:** Controls target's actions (charm), can force them to attack allies or cast spells
- **Visual:** Puppet strings (0x373A + lines)
- **Sound:** 0x1FC
- **Purpose:** Mind control

#### 31. Apocalyptic Hex
- **Mana:** 50
- **Reagents:** All 8 reagents + SwampLotus (15)
- **Cast Time:** 4.0s
- **Area:** Entire screen (30 tile radius)
- **Duration:** 30 seconds
- **Effect:** ALL enemies cursed with multiple stacking hexes: -20 all stats, -20 all resists, Deadly Poison, -50% healing, 8-15 damage/tick
- **Visual:** Purple/green apocalypse (0x3779 + 0x372A)
- **Sound:** 0x307
- **Purpose:** Screen-wide mega-curse

#### 32. Witch Queen's Dominion
- **Mana:** 50
- **Reagents:** All 8 reagents + SwampLotus (10) + BogIronOre (10)
- **Cast Time:** 4.5s
- **Duration:** 60 seconds
- **Effect:** Become Witch Queen: All hexes cost 50% less mana, hexes last twice as long, life drain heals 300%, immune to curses/poison, summons 3 hex totems that auto-curse enemies
- **Visual:** Regal dark aura with crown (0x373A + particles)
- **Sound:** 0x1FC
- **Purpose:** Transformation ultimate

---

## Spell Progression

**Early Game (Circles 1-3):** Basic curses, life drain, poison
**Mid Game (Circles 4-5):** Contagious hexes, anti-healing, silences
**Late Game (Circles 6-7):** Mass curses, plague mechanics, transformations
**End Game (Circle 8):** Mind control, permanent curses, screen-wide devastation

---

## Hex Mechanics

**Curse Stacking:**
- Multiple different hexes can stack on same target
- Same hex refreshes duration when reapplied
- Some hexes are contagious (spread to nearby enemies)

**Life Drain:**
- Ineffective against undead (0 damage/healing)
- Doubled against living creatures
- Can overheal and grant temporary HP

**Anti-Healing:**
- Reduces healing from all sources (spells, potions, bandages)
- Can invert healing to damage
- Persists through cleanse attempts

---

## Reagent Usage Summary

**Primary Reagents:**
- Nightshade (28 spells) - Core hex reagent
- Bloodmoss (24 spells) - DoTs and curses
- MandrakeRoot (20 spells) - Offensive hexes
- Ginseng (8 spells) - Life drain spells
- BlackPearl (12 spells) - High-tier hexes
- Garlic (7 spells) - Stat reduction
- SpidersSilk (9 spells) - CC hexes
- SulfurousAsh (8 spells) - Damage amplifiers

**Special Resources:**
- SwampLotus - 3 spells (high-tier hexes)
- BogIronOre - 2 spells (permanent curses)

---

## Mana Cost Distribution

- **Circle 1:** 4 mana × 4 spells = 16 total
- **Circle 2:** 6 mana × 4 spells = 24 total
- **Circle 3:** 9 mana × 4 spells = 36 total
- **Circle 4:** 11 mana × 4 spells = 44 total
- **Circle 5:** 14 mana × 4 spells = 56 total
- **Circle 6:** 20 mana × 4 spells = 80 total
- **Circle 7:** 40 mana × 4 spells = 160 total
- **Circle 8:** 50 mana × 4 spells = 200 total

---

## ServUO Implementation Notes

This section provides detailed guidance on implementing Hex Magic spells in ServUO, categorizing mechanics by complexity.

---

### ✅ Standard Mechanics (Built-in ServUO)

These mechanics are natively supported by ServUO and require minimal custom code:

#### Stat Debuffs
- **Spells:** Weak Curse, Enfeeble, Crippling Curse, Mass Hex
- **ServUO API:** `StatMod(StatType, name, offset, duration)` with negative offset
```csharp
// Weak Curse: -5 all stats
StatMod strDebuff = new StatMod(StatType.Str, "WeakCurse_Str", -5, TimeSpan.FromSeconds(20.0));
StatMod dexDebuff = new StatMod(StatType.Dex, "WeakCurse_Dex", -5, TimeSpan.FromSeconds(20.0));
StatMod intDebuff = new StatMod(StatType.Int, "WeakCurse_Int", -5, TimeSpan.FromSeconds(20.0));
target.AddStatMod(strDebuff);
target.AddStatMod(dexDebuff);
target.AddStatMod(intDebuff);
```

#### Resistance Debuffs
- **Spells:** Hex of Frailty, Mass Hex, Curse of Mortality
- **ServUO API:** `ResistanceMod(ResistanceType, offset)` with negative offset
```csharp
// Hex of Frailty: -10 all resistances
ResistanceMod physMod = new ResistanceMod(ResistanceType.Physical, -10);
ResistanceMod fireMod = new ResistanceMod(ResistanceType.Fire, -10);
ResistanceMod coldMod = new ResistanceMod(ResistanceType.Cold, -10);
ResistanceMod poisMod = new ResistanceMod(ResistanceType.Poison, -10);
ResistanceMod nrgyMod = new ResistanceMod(ResistanceType.Energy, -10);

target.AddResistanceMod(physMod);
target.AddResistanceMod(fireMod);
target.AddResistanceMod(coldMod);
target.AddResistanceMod(poisMod);
target.AddResistanceMod(nrgyMod);

// Remove after duration
Timer.DelayCall(duration, () => {
    target.RemoveResistanceMod(physMod);
    target.RemoveResistanceMod(fireMod);
    target.RemoveResistanceMod(coldMod);
    target.RemoveResistanceMod(poisMod);
    target.RemoveResistanceMod(nrgyMod);
});
```

#### Poison Application
- **Spells:** Poison Touch, Plague Bearer, Necrotic Touch
- **ServUO API:** `target.ApplyPoison(caster, Poison.Greater/Deadly)`
```csharp
// Poison Touch: Greater Poison
target.ApplyPoison(caster, Poison.Greater);

// Necrotic Touch: Deadly Poison
target.ApplyPoison(caster, Poison.Deadly);
```

#### Direct Damage
- **Spells:** Combined with life drain effects
- **ServUO API:** `SpellHelper.Damage()` or `AOS.Damage()`
```csharp
double damage = GetNewAosDamage(15, 1, 5, target);
SpellHelper.Damage(this, target, damage, 0, 0, 0, 100, 0); // 100% poison
```

#### Detection/Reveal
- **Spells:** Witch Sight
- **ServUO API:** `target.Hidden = false`, `target.RevealingAction()`
```csharp
// Reveal hidden creatures in range
IPooledEnumerable eable = caster.Map.GetMobilesInRange(caster.Location, 12);
foreach (Mobile m in eable)
{
    if (m.Hidden)
    {
        m.RevealingAction();
        m.SendMessage("You have been revealed!");
    }
}
eable.Free();
```

---

### 🔧 Custom Mechanics (Require Implementation)

These mechanics need custom classes and systems to be implemented.

#### 1. Life Drain Mechanics
**Spells:** Siphon Life, Life Leech, Drain Essence, Soul Siphon, Soul Harvest

**Challenge:** Drain HP from target and heal caster, with special rules for undead

**Implementation:** Custom life drain system
```csharp
public static class LifeDrainHelper
{
    public static void PerformLifeDrain(Mobile caster, Mobile target, int minDrain, int maxDrain, double healMultiplier = 1.0)
    {
        // Check if target is undead (no life to drain)
        if (target is BaseCreature creature && creature.BloodType == BloodType.None)
        {
            caster.SendMessage("You cannot drain life from the undead!");
            return;
        }

        // Calculate drain amount
        int drain = Utility.RandomMinMax(minDrain, maxDrain);

        // Apply damage to target
        int actualDamage = Math.Min(drain, target.Hits); // Can't drain more than target has
        AOS.Damage(target, caster, actualDamage, 0, 0, 0, 100, 0); // 100% poison type

        // Heal caster
        int healAmount = (int)(actualDamage * healMultiplier);
        caster.Hits = Math.Min(caster.Hits + healAmount, caster.HitsMax);

        // Visual effect - beam from target to caster
        caster.MovingEffect(target, 0x374A, 7, 0, false, false, 0x496, 0);
        caster.PlaySound(0x1FB);

        // Optionally grant temporary HP (overheal)
        if (caster.Hits == caster.HitsMax && healMultiplier > 1.0)
        {
            int overheal = healAmount - (caster.HitsMax - (caster.Hits - healAmount));
            if (overheal > 0)
                BuffInfo.AddBuff(caster, new BuffInfo(BuffIcon.Bless, 1075814, 1075815, TimeSpan.FromSeconds(60.0), caster, $"+{overheal} temporary HP"));
        }
    }

    public static void PerformManaDrain(Mobile caster, Mobile target, int minDrain, int maxDrain)
    {
        int drain = Utility.RandomMinMax(minDrain, maxDrain);
        int actualDrain = Math.Min(drain, target.Mana);

        target.Mana -= actualDrain;
        caster.Mana = Math.Min(caster.Mana + actualDrain, caster.ManaMax);

        target.FixedParticles(0x374A, 1, 15, 9909, 32, 0, EffectLayer.Waist);
    }
}

// Example usage in Siphon Life spell:
public void Target(Mobile target)
{
    if (!Caster.CanBeHarmful(target))
        return;

    Caster.DoHarmful(target);
    LifeDrainHelper.PerformLifeDrain(Caster, target, 6, 12, 1.0);
}

// Example for Soul Siphon (200% healing):
LifeDrainHelper.PerformLifeDrain(Caster, target, 35, 55, 2.0);
```

#### 2. Contagious Hex Spreading
**Spells:** Contagious Hex, Plague Bearer

**Implementation:** Custom hex context that spreads to nearby enemies
```csharp
public class ContagiousHexContext
{
    public Mobile Target { get; private set; }
    public Mobile Caster { get; private set; }
    public int Ticks { get; private set; }
    public Timer HexTimer { get; private set; }
    private int m_SpreadRadius;

    public ContagiousHexContext(Mobile caster, Mobile target, int ticks, int spreadRadius)
    {
        Caster = caster;
        Target = target;
        Ticks = ticks;
        m_SpreadRadius = spreadRadius;

        HexTimer = Timer.DelayCall(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0), ticks, OnTick);
    }

    private void OnTick()
    {
        if (Target == null || Target.Deleted || !Target.Alive)
        {
            HexTimer?.Stop();
            ContagiousHexManager.RemoveHex(Target);
            return;
        }

        // Apply hex damage/debuff
        int damage = Utility.RandomMinMax(3, 5);
        AOS.Damage(Target, Caster, damage, 0, 0, 0, 100, 0);

        // Attack speed debuff
        StatMod slowMod = new StatMod(StatType.Dex, "ContagiousHex_Slow", -10, TimeSpan.FromSeconds(2.5));
        Target.AddStatMod(slowMod);

        Target.FixedParticles(0x374A, 10, 15, 5021, 0x81D, 0, EffectLayer.Waist);

        // Spread to nearby allies
        IPooledEnumerable eable = Target.Map.GetMobilesInRange(Target.Location, m_SpreadRadius);
        foreach (Mobile m in eable)
        {
            if (m != Target && m.Alive && m != Caster && !ContagiousHexManager.IsHexed(m))
            {
                // Check if ally of target (share same team/guild)
                if (AreAllies(Target, m) && Utility.RandomDouble() < 0.4) // 40% spread chance
                {
                    ContagiousHexManager.ApplyHex(Caster, m, Ticks / 2, m_SpreadRadius); // Half duration
                    m.SendMessage("The hex spreads to you!");
                }
            }
        }
        eable.Free();

        if (--Ticks == 0)
        {
            ContagiousHexManager.RemoveHex(Target);
            HexTimer?.Stop();
        }
    }

    private bool AreAllies(Mobile a, Mobile b)
    {
        // Implement ally check logic (guild, party, alignment, etc.)
        if (a is BaseCreature creatureA && b is BaseCreature creatureB)
            return creatureA.Team == creatureB.Team;
        return false;
    }
}

public static class ContagiousHexManager
{
    private static Dictionary<Mobile, ContagiousHexContext> m_Hexed = new Dictionary<Mobile, ContagiousHexContext>();

    public static void ApplyHex(Mobile caster, Mobile target, int ticks, int spreadRadius)
    {
        if (m_Hexed.ContainsKey(target))
            return; // Already hexed

        ContagiousHexContext context = new ContagiousHexContext(caster, target, ticks, spreadRadius);
        m_Hexed[target] = context;
    }

    public static bool IsHexed(Mobile m)
    {
        return m_Hexed.ContainsKey(m);
    }

    public static void RemoveHex(Mobile m)
    {
        if (m_Hexed.ContainsKey(m))
        {
            m_Hexed[m].HexTimer?.Stop();
            m_Hexed.Remove(m);
        }
    }
}
```

#### 3. Voodoo Doll (Damage Reflection)
**Spells:** Voodoo Doll

**Implementation:** Track linked target and reflect damage
```csharp
public class VoodooDollContext
{
    public Mobile Caster { get; private set; }
    public Mobile LinkedTarget { get; private set; }
    public DateTime EndTime { get; private set; }
    public double ReflectionPercent { get; private set; }

    public VoodooDollContext(Mobile caster, Mobile linkedTarget, TimeSpan duration, double reflectionPercent)
    {
        Caster = caster;
        LinkedTarget = linkedTarget;
        EndTime = DateTime.UtcNow + duration;
        ReflectionPercent = reflectionPercent;

        // Hook into damage event
        Timer.DelayCall(duration, () => VoodooDollManager.RemoveLink(caster));
    }

    public void OnCasterDamaged(int damage)
    {
        if (LinkedTarget == null || LinkedTarget.Deleted || !LinkedTarget.Alive)
            return;

        int reflectedDamage = (int)(damage * ReflectionPercent);
        if (reflectedDamage > 0)
        {
            AOS.Damage(LinkedTarget, Caster, reflectedDamage, 100, 0, 0, 0, 0);
            LinkedTarget.FixedParticles(0x374A, 1, 15, 9909, 32, 0, EffectLayer.Waist);
            LinkedTarget.SendMessage("You feel the voodoo doll's curse!");
        }
    }
}

public static class VoodooDollManager
{
    private static Dictionary<Mobile, VoodooDollContext> m_Links = new Dictionary<Mobile, VoodooDollContext>();

    public static void CreateLink(Mobile caster, Mobile target, TimeSpan duration, double reflectionPercent)
    {
        // Remove existing link
        if (m_Links.ContainsKey(caster))
            RemoveLink(caster);

        VoodooDollContext context = new VoodooDollContext(caster, target, duration, reflectionPercent);
        m_Links[caster] = context;
    }

    public static void RemoveLink(Mobile caster)
    {
        if (m_Links.ContainsKey(caster))
            m_Links.Remove(caster);
    }

    public static void OnDamage(Mobile damaged, int amount)
    {
        if (m_Links.ContainsKey(damaged))
            m_Links[damaged].OnCasterDamaged(amount);
    }
}

// In Mobile.cs or PlayerMobile.cs, hook into OnDamaged:
public override void OnDamaged(int amount, Mobile from, bool willKill)
{
    VoodooDollManager.OnDamage(this, amount);
    base.OnDamaged(amount, from, willKill);
}
```

#### 4. Anti-Healing Mechanics
**Spells:** Hex of Agony (healing becomes damage), Necrotic Touch (50% healing reduction)

**Implementation:** Custom buff that modifies healing

**Method 1: Healing Reduction (Necrotic Touch)**
```csharp
public class HealingReductionContext
{
    public Mobile Target { get; private set; }
    public double ReductionPercent { get; private set; }
    public DateTime EndTime { get; private set; }

    public HealingReductionContext(Mobile target, double reductionPercent, TimeSpan duration)
    {
        Target = target;
        ReductionPercent = reductionPercent;
        EndTime = DateTime.UtcNow + duration;

        Timer.DelayCall(duration, () => HealingReductionManager.RemoveReduction(target));
    }

    public int ModifyHealing(int originalHeal)
    {
        int reducedHeal = (int)(originalHeal * (1.0 - ReductionPercent));
        Target.SendMessage($"Your healing is reduced! ({originalHeal} -> {reducedHeal})");
        return reducedHeal;
    }
}

public static class HealingReductionManager
{
    private static Dictionary<Mobile, HealingReductionContext> m_Reductions = new Dictionary<Mobile, HealingReductionContext>();

    public static void ApplyReduction(Mobile target, double reductionPercent, TimeSpan duration)
    {
        if (m_Reductions.ContainsKey(target))
            RemoveReduction(target);

        HealingReductionContext context = new HealingReductionContext(target, reductionPercent, duration);
        m_Reductions[target] = context;
    }

    public static void RemoveReduction(Mobile target)
    {
        if (m_Reductions.ContainsKey(target))
            m_Reductions.Remove(target);
    }

    public static int ProcessHealing(Mobile target, int healAmount)
    {
        if (m_Reductions.ContainsKey(target))
            return m_Reductions[target].ModifyHealing(healAmount);
        return healAmount;
    }
}

// Hook into all healing code:
// Before: target.Hits += healAmount;
// After:
int modifiedHeal = HealingReductionManager.ProcessHealing(target, healAmount);
target.Hits = Math.Min(target.Hits + modifiedHeal, target.HitsMax);
```

**Method 2: Healing Inversion (Hex of Agony)**
```csharp
public static class HexOfAgonyManager
{
    private static HashSet<Mobile> m_Cursed = new HashSet<Mobile>();

    public static void ApplyCurse(Mobile target, TimeSpan duration)
    {
        if (!m_Cursed.Contains(target))
        {
            m_Cursed.Add(target);
            target.SendMessage("You are cursed! Healing will harm you!");

            Timer.DelayCall(duration, () => RemoveCurse(target));
        }
    }

    public static void RemoveCurse(Mobile target)
    {
        if (m_Cursed.Contains(target))
        {
            m_Cursed.Remove(target);
            target.SendMessage("The hex of agony fades.");
        }
    }

    public static bool IsCursed(Mobile m)
    {
        return m_Cursed.Contains(m);
    }

    public static int ProcessHealing(Mobile target, int healAmount)
    {
        if (IsCursed(target))
        {
            // Invert healing to damage
            AOS.Damage(target, null, healAmount, 100, 0, 0, 0, 0);
            target.SendMessage("The healing harms you!");
            target.FixedParticles(0x374A, 1, 15, 9909, 0x81D, 0, EffectLayer.Waist);
            return 0; // No healing applied
        }
        return healAmount;
    }
}

// Integrate into healing code:
int processedHeal = HexOfAgonyManager.ProcessHealing(target, healAmount);
if (processedHeal > 0)
    target.Hits = Math.Min(target.Hits + processedHeal, target.HitsMax);
```

#### 5. Wasting Curse (Max HP Reduction)
**Spells:** Wasting Curse, Curse of Mortality

**Implementation:** Timer-based max HP reduction
```csharp
public class WastingCurseContext
{
    private Mobile m_Target;
    private Mobile m_Caster;
    private int m_Ticks;
    private int m_MaxHPLoss = 0;
    private int m_MaxLossPercent = 10;
    private Timer m_Timer;

    public WastingCurseContext(Mobile caster, Mobile target, int ticks, int maxLossPercent)
    {
        m_Caster = caster;
        m_Target = target;
        m_Ticks = ticks;
        m_MaxLossPercent = maxLossPercent;

        m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(3.0), TimeSpan.FromSeconds(3.0), ticks, OnTick);
    }

    private void OnTick()
    {
        if (m_Target == null || m_Target.Deleted || !m_Target.Alive)
        {
            End();
            return;
        }

        // Reduce max HP by 1%
        int originalMax = m_Target.HitsMax;
        int reduction = Math.Max(1, originalMax / 100); // At least 1 HP

        m_MaxHPLoss += reduction;

        // Cap at max loss percent
        if (m_MaxHPLoss > originalMax * m_MaxLossPercent / 100)
            m_MaxHPLoss = originalMax * m_MaxLossPercent / 100;

        // Apply temporary max HP reduction (use buff or custom property)
        m_Target.SendMessage("Your vitality withers away...");
        m_Target.FixedParticles(0x374A, 1, 15, 9909, 0x81D, 0, EffectLayer.Waist);

        if (--m_Ticks == 0)
            End();
    }

    private void End()
    {
        m_Timer?.Stop();
        // Restore max HP (buff expires)
        WastingCurseManager.RemoveCurse(m_Target);
    }
}

// Note: Actual max HP reduction requires custom property or buff system
// ServUO doesn't natively support temporary max HP changes
```

#### 6. Hex of Silence (Prevent Spellcasting)
**Spells:** Hex of Silence

**Implementation:** Custom flag to prevent casting
```csharp
public static class SilenceManager
{
    private static Dictionary<Mobile, DateTime> m_Silenced = new Dictionary<Mobile, DateTime>();

    public static void ApplySilence(Mobile target, TimeSpan duration)
    {
        DateTime endTime = DateTime.UtcNow + duration;
        m_Silenced[target] = endTime;

        target.SendMessage("You have been silenced! You cannot cast spells!");
        target.FixedParticles(0x3779, 1, 15, 9909, 0x81D, 0, EffectLayer.Head);

        Timer.DelayCall(duration, () => RemoveSilence(target));
    }

    public static void RemoveSilence(Mobile target)
    {
        if (m_Silenced.ContainsKey(target))
        {
            m_Silenced.Remove(target);
            target.SendMessage("You can speak and cast again.");
        }
    }

    public static bool IsSilenced(Mobile m)
    {
        if (m_Silenced.ContainsKey(m))
        {
            if (DateTime.UtcNow < m_Silenced[m])
                return true;
            else
            {
                RemoveSilence(m);
                return false;
            }
        }
        return false;
    }
}

// In Spell.OnCast() or CheckCast():
if (SilenceManager.IsSilenced(Caster))
{
    Caster.SendMessage("You are silenced and cannot cast spells!");
    return false;
}
```

#### 7. Vampiric Aura (Spell Damage as Healing)
**Spells:** Vampiric Aura, Witch Queen's Dominion

**Implementation:** Track spell damage and convert to healing
```csharp
public static class VampiricAuraManager
{
    private static Dictionary<Mobile, VampiricAuraContext> m_Auras = new Dictionary<Mobile, VampiricAuraContext>();

    public static void ApplyAura(Mobile caster, TimeSpan duration, double lifestealPercent, double spellDamageBonus)
    {
        if (m_Auras.ContainsKey(caster))
            RemoveAura(caster);

        VampiricAuraContext context = new VampiricAuraContext(caster, duration, lifestealPercent, spellDamageBonus);
        m_Auras[caster] = context;

        caster.SendMessage("You are surrounded by a vampiric aura!");
        caster.FixedParticles(0x373A, 10, 15, 5021, 0x496, 0, EffectLayer.Waist);

        Timer.DelayCall(duration, () => RemoveAura(caster));
    }

    public static void RemoveAura(Mobile caster)
    {
        if (m_Auras.ContainsKey(caster))
        {
            m_Auras.Remove(caster);
            caster.SendMessage("Your vampiric aura fades.");
        }
    }

    public static void OnSpellDamage(Mobile caster, int damage)
    {
        if (m_Auras.ContainsKey(caster))
        {
            VampiricAuraContext context = m_Auras[caster];
            int healing = (int)(damage * context.LifestealPercent);

            caster.Hits = Math.Min(caster.Hits + healing, caster.HitsMax);
            caster.FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);
        }
    }

    public static double GetSpellDamageBonus(Mobile caster)
    {
        if (m_Auras.ContainsKey(caster))
            return m_Auras[caster].SpellDamageBonus;
        return 0.0;
    }
}

public class VampiricAuraContext
{
    public Mobile Caster { get; private set; }
    public double LifestealPercent { get; private set; }
    public double SpellDamageBonus { get; private set; }

    public VampiricAuraContext(Mobile caster, TimeSpan duration, double lifestealPercent, double spellDamageBonus)
    {
        Caster = caster;
        LifestealPercent = lifestealPercent;
        SpellDamageBonus = spellDamageBonus;
    }
}

// In spell damage code:
double damage = GetNewAosDamage(19, 1, 5, target);
damage *= (1.0 + VampiricAuraManager.GetSpellDamageBonus(Caster));
SpellHelper.Damage(this, target, damage, 0, 0, 0, 100, 0);
VampiricAuraManager.OnSpellDamage(Caster, (int)damage);
```

---

### ⚠️ Advanced Mechanics (Complex Implementation)

These require extensive custom systems and careful balancing.

#### 1. Doomcurse (Delayed Nuke)
**Challenge:** Apply countdown curse that deals massive damage after delay

**Implementation:** Timed curse with visual countdown
```csharp
public class DoomcurseContext
{
    private Mobile m_Target;
    private Mobile m_Caster;
    private int m_CountdownSeconds;
    private Timer m_Timer;
    private int m_Damage;

    public DoomcurseContext(Mobile caster, Mobile target, int countdownSeconds, int damage)
    {
        m_Caster = caster;
        m_Target = target;
        m_CountdownSeconds = countdownSeconds;
        m_Damage = damage;

        m_Target.SendMessage("You have been cursed with DOOM!");
        m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0), countdownSeconds, OnTick);
    }

    private void OnTick()
    {
        if (m_Target == null || m_Target.Deleted || !m_Target.Alive)
        {
            m_Timer?.Stop();
            DoomcurseManager.RemoveCurse(m_Target);
            return;
        }

        m_CountdownSeconds--;

        // Visual countdown
        m_Target.PublicOverheadMessage(MessageType.Regular, 0x21, false, m_CountdownSeconds.ToString());
        m_Target.FixedParticles(0x3779, 1, 15, 9909, 0x21, 0, EffectLayer.Head);
        m_Target.PlaySound(0x1FC);

        if (m_CountdownSeconds == 0)
        {
            // DOOM!
            int damage = Utility.RandomMinMax(100, 150);
            AOS.Damage(m_Target, m_Caster, damage, 0, 0, 0, 100, 0);
            m_Target.FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);
            m_Target.PlaySound(0x307);
            m_Target.SendMessage("THE DOOMCURSE STRIKES!");

            m_Timer?.Stop();
            DoomcurseManager.RemoveCurse(m_Target);
        }
    }

    public void Dispel()
    {
        m_Timer?.Stop();
        m_Target.SendMessage("The doomcurse has been dispelled!");
    }
}

public static class DoomcurseManager
{
    private static Dictionary<Mobile, DoomcurseContext> m_Cursed = new Dictionary<Mobile, DoomcurseContext>();

    public static void ApplyCurse(Mobile caster, Mobile target, int countdownSeconds, int damage)
    {
        if (m_Cursed.ContainsKey(target))
            return; // Already doomed

        DoomcurseContext context = new DoomcurseContext(caster, target, countdownSeconds, damage);
        m_Cursed[target] = context;
    }

    public static void RemoveCurse(Mobile target)
    {
        if (m_Cursed.ContainsKey(target))
            m_Cursed.Remove(target);
    }

    public static bool IsCursed(Mobile m)
    {
        return m_Cursed.ContainsKey(m);
    }

    public static void DispelCurse(Mobile target)
    {
        if (m_Cursed.ContainsKey(target))
        {
            m_Cursed[target].Dispel();
            m_Cursed.Remove(target);
        }
    }
}
```

#### 2. Hex Storm (Random Hex Zone)
**Challenge:** Create zone that applies random hexes periodically

**Implementation:** Custom item with random hex effects
```csharp
public class HexStormItem : Item
{
    private Mobile m_Caster;
    private Timer m_Timer;
    private int m_Ticks = 7; // 15 seconds / 2 second ticks

    private enum HexType { Curse, Poison, Slow, Silence }

    public HexStormItem(Mobile caster, Point3D loc, Map map) : base(0x3779)
    {
        Movable = false;
        Name = "hex storm";
        Hue = 0x81D;
        m_Caster = caster;

        MoveToWorld(loc, map);

        m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0), m_Ticks, PulseHexes);
    }

    private void PulseHexes()
    {
        if (Deleted || Map == null)
            return;

        IPooledEnumerable eable = Map.GetMobilesInRange(Location, 6);
        foreach (Mobile m in eable)
        {
            if (m.Alive && m_Caster.CanBeHarmful(m))
            {
                // Apply random hex
                HexType hex = (HexType)Utility.Random(4);

                switch (hex)
                {
                    case HexType.Curse:
                        StatMod curse = new StatMod(StatType.Str, "HexStorm_Curse", -10, TimeSpan.FromSeconds(5.0));
                        m.AddStatMod(curse);
                        m.SendMessage("The storm curses you!");
                        break;

                    case HexType.Poison:
                        m.ApplyPoison(m_Caster, Poison.Regular);
                        m.SendMessage("The storm poisons you!");
                        break;

                    case HexType.Slow:
                        StatMod slow = new StatMod(StatType.Dex, "HexStorm_Slow", -15, TimeSpan.FromSeconds(5.0));
                        m.AddStatMod(slow);
                        m.SendMessage("The storm slows you!");
                        break;

                    case HexType.Silence:
                        SilenceManager.ApplySilence(m, TimeSpan.FromSeconds(3.0));
                        m.SendMessage("The storm silences you!");
                        break;
                }

                m.FixedParticles(0x374A, 10, 15, 5021, 0x81D, 0, EffectLayer.Waist);
            }
        }
        eable.Free();

        if (--m_Ticks == 0)
            Delete();
    }
}
```

#### 3. Curse of the Hag (Polymorph)
**Challenge:** Transform target into chicken with stat penalties

**Implementation:** Body transformation with extreme debuffs
```csharp
public class CurseOfHagContext
{
    private Mobile m_Target;
    private int m_OriginalBody;
    private Timer m_Timer;

    public CurseOfHagContext(Mobile target, TimeSpan duration)
    {
        m_Target = target;
        m_OriginalBody = target.BodyValue;

        // Transform to chicken
        target.BodyValue = 0x00D0; // Chicken body
        target.PlaySound(0x6E); // Chicken sound

        // Apply massive debuffs
        StatMod strDebuff = new StatMod(StatType.Str, "HagCurse_Str", -(int)(target.RawStr * 0.9), duration);
        StatMod dexDebuff = new StatMod(StatType.Dex, "HagCurse_Dex", -(int)(target.RawDex * 0.5), duration);
        target.AddStatMod(strDebuff);
        target.AddStatMod(dexDebuff);

        // Prevent spellcasting
        SilenceManager.ApplySilence(target, duration);

        target.SendMessage("You have been transformed into a chicken!");

        m_Timer = Timer.DelayCall(duration, Revert);
    }

    private void Revert()
    {
        if (m_Target != null && !m_Target.Deleted)
        {
            m_Target.BodyValue = m_OriginalBody;
            m_Target.SendMessage("You return to your normal form.");
        }
    }
}
```

#### 4. Voodoo Mastery (Mind Control/Charm)
**Challenge:** Control target's actions

**Implementation:** Temporary AI override (complex - requires core modification)
```csharp
// Simplified charm implementation (makes target attack allies)
public static class CharmManager
{
    private static Dictionary<Mobile, CharmContext> m_Charmed = new Dictionary<Mobile, CharmContext>();

    public static void CharmTarget(Mobile caster, Mobile target, TimeSpan duration)
    {
        if (target is PlayerMobile)
        {
            caster.SendMessage("You cannot fully control a player's mind, but you confuse them!");
            // Apply confusion instead (random movement/attacks)
            return;
        }

        if (target is BaseCreature creature)
        {
            creature.Controlled = true;
            creature.ControlMaster = caster;
            creature.ControlOrder = OrderType.Attack;

            Timer.DelayCall(duration, () =>
            {
                creature.Controlled = false;
                creature.ControlMaster = null;
                creature.SendMessage("You break free from the charm!");
            });
        }
    }
}
```

#### 5. Curse of Undeath (Permanent Curse)
**Challenge:** Permanent curse that inverts healing

**Implementation:** Persistent curse flag
```csharp
public static class CurseOfUndeathManager
{
    private static HashSet<Mobile> m_Cursed = new HashSet<Mobile>();

    public static void ApplyCurse(Mobile target)
    {
        if (!m_Cursed.Contains(target))
        {
            m_Cursed.Add(target);
            target.SendMessage("You have been cursed with undeath! This curse is permanent until dispelled!");
            target.FixedParticles(0x374A, 10, 30, 5052, EffectLayer.Waist);

            // Visual: apply undead hue
            target.Hue = 0x4001; // Pale/undead hue
        }
    }

    public static void RemoveCurse(Mobile target)
    {
        if (m_Cursed.Contains(target))
        {
            m_Cursed.Remove(target);
            target.SendMessage("The curse of undeath has been lifted!");
            target.Hue = 0; // Restore normal hue
        }
    }

    public static bool IsCursed(Mobile m)
    {
        return m_Cursed.Contains(m);
    }

    public static int ProcessHealing(Mobile target, int healAmount)
    {
        if (IsCursed(target))
        {
            // Healing becomes damage
            AOS.Damage(target, null, healAmount, 100, 0, 0, 0, 0);
            target.SendMessage("The healing harms you!");
            return 0;
        }
        return healAmount;
    }

    public static double GetDamageModifier(Mobile target, DamageType type)
    {
        if (IsCursed(target))
        {
            if (type == DamageType.Poison || type == DamageType.Curse)
                return 1.5; // +50% poison/curse damage
            if (type == DamageType.Holy)
                return 2.0; // +100% holy damage
        }
        return 1.0;
    }
}

public enum DamageType { Physical, Fire, Cold, Poison, Energy, Holy, Curse }
```

#### 6. Witch Queen's Dominion (Transformation + Hex Totems)
**Challenge:** Transformation with auto-cursing totems

**Implementation:** Buff with summon totems
```csharp
public class HexTotemItem : Item
{
    private Mobile m_Caster;
    private Timer m_Timer;
    private int m_Ticks = 20; // 60 seconds / 3 second ticks

    public HexTotemItem(Mobile caster, Point3D loc, Map map) : base(0x0ED4)
    {
        Movable = false;
        Name = "hex totem";
        Hue = 0x81D;
        m_Caster = caster;

        MoveToWorld(loc, map);

        m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(3.0), TimeSpan.FromSeconds(3.0), m_Ticks, CurseNearby);
    }

    private void CurseNearby()
    {
        if (Deleted || Map == null)
            return;

        IPooledEnumerable eable = Map.GetMobilesInRange(Location, 8);
        foreach (Mobile m in eable)
        {
            if (m.Alive && m != m_Caster && m_Caster.CanBeHarmful(m))
            {
                // Apply random hex
                StatMod hex = new StatMod(StatType.Str, "HexTotem_Curse", -10, TimeSpan.FromSeconds(6.0));
                m.AddStatMod(hex);
                m.FixedParticles(0x374A, 1, 15, 9909, 0x81D, 0, EffectLayer.Waist);
            }
        }
        eable.Free();

        if (--m_Ticks == 0)
            Delete();
    }
}

// In Witch Queen's Dominion spell:
public void SpawnHexTotems()
{
    for (int i = 0; i < 3; i++)
    {
        Point3D loc = GetSpawnLocation(Caster, 5);
        HexTotemItem totem = new HexTotemItem(Caster, loc, Caster.Map);
    }
}
```

---

### 📝 Implementation Checklist per Spell

#### 1. Basic Curse/Debuff (e.g., Weak Curse)
- [ ] Create spell class inheriting from `MagerySpell`
- [ ] Define `SpellInfo` with name, mantra, effect IDs, reagents
- [ ] Set `Circle` property
- [ ] Implement `OnCast()` with target selection
- [ ] Apply stat debuffs with `StatMod` (negative values)
- [ ] Set duration timer
- [ ] Add visual/sound effects

#### 2. Life Drain Spell (e.g., Siphon Life)
- [ ] Create spell class
- [ ] Check if target is undead (no drain)
- [ ] Calculate drain amount
- [ ] Apply damage to target
- [ ] Heal caster for amount drained (or multiplier)
- [ ] Add beam visual from target to caster
- [ ] Optional: grant temporary HP if overhealing

#### 3. Contagious Hex (e.g., Contagious Hex)
- [ ] Create spell class
- [ ] Apply initial hex to target
- [ ] Create timer for periodic damage/debuff
- [ ] Each tick: check for nearby allies to spread
- [ ] Use ContagiousHexManager to track hexed targets
- [ ] Prevent re-hexing already hexed targets
- [ ] Clean up when expired

#### 4. Anti-Healing Hex (e.g., Hex of Agony)
- [ ] Create spell class
- [ ] Add target to anti-heal manager
- [ ] Hook into all healing code to intercept
- [ ] Invert healing to damage
- [ ] Add visual feedback when healing triggers damage
- [ ] Remove from manager after duration

#### 5. Silence Spell (e.g., Hex of Silence)
- [ ] Create spell class
- [ ] Add target to silence manager
- [ ] Hook into Spell.CheckCast() to block casting
- [ ] Display message when attempting to cast
- [ ] Add visual indicator (gag effect)
- [ ] Remove after duration

#### 6. Delayed Nuke (e.g., Doomcurse)
- [ ] Create spell class
- [ ] Apply countdown context
- [ ] Create timer with per-second tick
- [ ] Display countdown overhead
- [ ] Apply massive damage when countdown reaches 0
- [ ] Optional: allow dispel with specific spell

---

### 🎯 Priority Implementation Order

#### Phase 1 - Foundation (Circles 1-2)
1. **Basic Curse:** Weak Curse (stat debuffs)
2. **Life Drain:** Siphon Life (basic drain + heal)
3. **Poison:** Poison Touch (apply Greater Poison)
4. **Detection:** Witch Sight (reveal hidden)
5. **Self-Buff:** Dark Pact (sacrifice HP for power)

**Goal:** Test core hex and life drain mechanics

#### Phase 2 - Core Systems (Circles 3-4)
1. **Life Drain System:** Life Leech, Drain Essence (HP + mana drain)
2. **Contagious Hex:** Contagious Hex (spreading debuff)
3. **Resistance Debuff:** Hex of Frailty (-10 all resists)
4. **Voodoo Doll:** Damage reflection mechanic
5. **Anti-Healing:** Hex of Agony (healing becomes damage)

**Goal:** Establish hex spreading and anti-healing mechanics

#### Phase 3 - Advanced Mechanics (Circles 5-6)
1. **Mass Hex:** AoE debuff zone
2. **Silence:** Hex of Silence (prevent spellcasting)
3. **Ultimate Drain:** Soul Siphon (200% healing + temp HP)
4. **Vampiric Aura:** Spell damage as healing
5. **Doomcurse:** Delayed nuke mechanic

**Goal:** Complete curse variety and control mechanics

#### Phase 4 - Ultimate Spells (Circles 7-8)
1. **Plague of Sorrows:** Spreading mega-plague
2. **Curse of the Hag:** Polymorph debuff
3. **Voodoo Mastery:** Mind control/charm
4. **Curse of Undeath:** Permanent curse
5. **Witch Queen's Dominion:** Transformation + totems

**Goal:** Implement complex ultimate hexes

---

## Balance Considerations

**PvE:**
- Excellent vs. bosses (sustained debuffs)
- Life drain for self-sustain
- Struggles against undead enemies
- Strong group utility with mass hexes

**PvP:**
- High skill cap (hex layering)
- Vulnerable to cleanse/dispel
- Counters healers with anti-heal
- Weak burst but strong sustained pressure

**Synergies:**
- Curse Layering: Weak Curse → Hex of Frailty → Necrotic Touch
- Life Steal: Dark Pact → Vampiric Aura → Soul Harvest
- Plague Strategy: Poison Touch → Plague Bearer → Plague of Sorrows
- Control: Crippling Curse → Hex of Silence → Voodoo Mastery

---

**Last Updated:** 2025-12-05
**Status:** Design Complete - 0/32 Implemented
**Next Steps:** Implement life drain mechanics and Circle 1-2 hexes
