# Illusion Magic - Desert School

**Class:** Illusionist
**Region:** Desert (Sunbaked Expanse)
**Theme:** Illusions, mind control, invisibility, trickery, confusion
**Spellbook:** Tome of Deception (IllusionistSpellbook.cs)
**Primary Stat:** Intelligence
**Hue:** 0x8B0 (Shimmering Gold)

**Implementation Status:** ✅ **32/32 spells implemented (100% COMPLETE - ready for testing)**

> **📝 Reagent Note:** Individual spell entries below use placeholder UO reagent names for design reference. The actual ServUO implementation uses custom Vystia reagents as documented in the [Reagents](#reagents) section at the end of this file. See `IllusionMagicReagents.cs` for implementation.

---

## Overview

Illusion Magic manipulates perception and mind, specializing in creating illusions, going invisible, confusing enemies, and mind control. Illusionists excel at trickery and misdirection.

**Strengths:**
- Invisibility (complete stealth)
- Create illusory copies (decoys)
- Mind control enemies
- Confusion and disorientation
- Escape and misdirection

**Weaknesses:**
- Low direct damage
- Illusions can be detected (True Sight)
- Vulnerable when visible
- Weak vs. mindless enemies (undead, constructs)
- Relies on deception (skill-based)

---

## Spell List (32 Spells)

### Circle 1 (4 mana)
1. **Mind Spike** - 5-11 psychic damage
2. **Blur** - +10% dodge for 30s
3. **Minor Illusion** - Create small illusory object (cosmetic)
4. **Detect Thoughts** - Read surface thoughts, see intentions

### Circle 2 (6 mana)
5. **Phantom Bolt** - 10-18 psychic damage
6. **Invisibility** - Become invisible for 30s (breaks on attack/spell)
7. **Illusory Double** - Create 1 weak copy (100 HP, 20% damage)
8. **Confuse** - Enemy attacks random target for 8s

### Circle 3 (9 mana)
9. **Psychic Scream** - 4 tile AoE, 15-26 psychic damage, fear 3s
10. **Greater Invisibility** - Invisible for 45s (breaks only on damage dealt)
11. **Mirror Image** - Create 2 illusory copies (200 HP each)
12. **Charm Beast** - Control beast enemy for 30s

### Circle 4 (11 mana)
13. **Mind Blast** - 25-42 psychic damage, silences 4s
14. **Illusory Terrain** - Create fake walls/obstacles (8x8 area)
15. **Phantasmal Killer** - Summons nightmare, fears and damages (300 HP)
16. **Mass Confusion** - 5 tile AoE, all enemies confused for 10s

### Circle 5 (14 mana)
17. **Mind Control** - Control humanoid enemy for 20s
18. **Perfect Invisibility** - Invisible for 60s, doesn't break
19. **Illusory Army** - Create 5 illusory warriors (250 HP each, 30% damage)
20. **Psychic Storm** - Ground AoE, 8-14 psychic damage/tick, mass confusion

### Circle 6 (20 mana)
21. **Dominate Mind** - Control any non-boss enemy for 40s
22. **Phase Shift** - Become intangible (immune to physical, 20s)
23. **Legion of Mirrors** - Create 10 illusory copies (300 HP each)
24. **Mass Charm** - 8 tile AoE, charm all enemies for 15s

### Circle 7 (40 mana)
25. **Mind Shatter** - 50-85 psychic damage, permanent confusion until dispelled
26. **True Invisibility** - Invisible permanently (until you choose to reveal)
27. **Phantasmal Dragon** - Summon illusory dragon (2000 HP, fear aura, breath)
28. **Reality Warp** - Swap positions of all enemies/allies (20 tiles)

### Circle 8 (50 mana)
29. **Apocalyptic Nightmare** - Screen-wide, 80-140 psychic damage, mass fear/confusion
30. **Master of Puppets** - Control up to 5 enemies simultaneously (30s)
31. **Illusory Apocalypse** - Creates massive illusion of destruction, all enemies flee in terror (45s)
32. **Perfect Illusion** - Ultimate: All abilities cost no mana, infinite illusions, unkillable copies

---

## Illusion Mechanics

**Illusory Copies:**
- Created copies look identical to caster
- Enemies may target copies instead of caster
- Copies deal reduced damage (20-50% of caster)
- Copies have HP and can be killed
- Disappear after duration or when killed

**Invisibility Levels:**
1. **Invisibility:** Breaks on any hostile action
2. **Greater Invisibility:** Breaks only when dealing damage
3. **Perfect Invisibility:** Doesn't break (timed duration)
4. **True Invisibility:** Permanent until canceled

**Mind Control:**
- Control enemy's actions
- Can force them to attack allies
- Can force them to use abilities
- Broken by damage to controlled target
- Immune: Undead, constructs, demons (some)

## Confusion Mechanics
- Confused enemies attack random targets (including allies)
- 50% chance to attack ally vs. enemy
- Can attack self
- Lasts until duration expires or dispelled
- Stacks: Mass confusion lasts longer

## Psychic Damage
- Ignores physical armor
- Effective against heavily armored targets
- Reduced vs. strong-willed enemies
- Can cause mental status effects (fear, confusion, silence)

## Illusory Terrain
- Creates fake obstacles
- Blocks line of sight
- Enemies try to path around them
- Deals no damage
- Duration-based (30-60s)
- Can trap enemies in "fake walls"

## Reagents

Illusion Magic uses the following Vystia-specific reagents organized by spell circle:

### Primary Reagents (6 types)
1. **Shadow Petal** (Circles 1-3) - Illusion flower from twilight gardens
2. **Mirror Dust** (Circles 2-4) - Crushed mirror glass from mirror mazes
3. **Phantom Silk** (Circles 1-4) - Ghostly fabric from ethereal planes
4. **Mirage Essence** (Circles 3-6) - Bottled illusion from desert mirages
5. **Dream Crystal** (Circles 5-7) - Sleep-caught dream from dream realms
6. **Reality Splinter** (Circles 6-8) - Broken reality fragment from reality rifts

### Rare Reagents (2 types)
7. **Void Mirror** (Circles 7-8) - Portal glass from dimensional portals
8. **Chaos Prism** (Circle 8) - Reality-warping crystal, ultimate reagent

**Implementation:** All reagents created in `IllusionMagicReagents.cs`

## Playstyle Strategies

**Stealth Assassin:**
1. True Invisibility
2. Approach undetected
3. Mind Control key target
4. Psychic Storm AoE
5. Re-stealth and escape

**Illusion Army:**
1. Create Mirror Images / Legion of Mirrors
2. Summon Phantasmal Killer/Dragon
3. Confuse enemies (attack wrong targets)
4. Stay safe while illusions fight

**Mind Controller:**
1. Dominate Mind on strongest enemy
2. Force them to attack allies
3. Mass Charm additional enemies
4. Watch chaos unfold

**Escape Artist:**
1. Phase Shift (intangible)
2. Perfect Invisibility
3. Reality Warp (teleport)
4. Illusory doubles to distract

---

## ServUO Implementation Notes

### ✅ Standard Mechanics
- **Psychic Damage:** Energy damage `SpellHelper.Damage(spell, target, damage, 0, 0, 0, 0, 100)`
- **Invisibility:** `target.Hidden = true`, breaks on action via `RevealingAction()`
- **Fear:** `target.Frozen = true` + direction manipulation for fleeing
- **Confusion:** Random target selection, modify AI

### 🔧 Custom Mechanics

**1. Invisibility Tiers**
```csharp
public static class InvisibilityManager
{
    private static Dictionary<Mobile, InvisibilityLevel> m_Invisible = new Dictionary<Mobile, InvisibilityLevel>();

    public enum InvisibilityLevel
    {
        None,
        Basic,      // Breaks on any action
        Greater,    // Breaks only on damage dealt
        Perfect,    // Timed, doesn't break
        True        // Permanent until canceled
    }

    public static void ApplyInvisibility(Mobile m, InvisibilityLevel level, TimeSpan duration)
    {
        m.Hidden = true;
        m_Invisible[m] = level;

        if (level == InvisibilityLevel.Perfect || level == InvisibilityLevel.True)
        {
            if (duration != TimeSpan.Zero)
                Timer.DelayCall(duration, () => RemoveInvisibility(m));
        }
    }

    public static void OnAction(Mobile m, ActionType action)
    {
        if (!m_Invisible.ContainsKey(m))
            return;

        InvisibilityLevel level = m_Invisible[m];

        if (level == InvisibilityLevel.Basic)
        {
            // Breaks on any hostile action
            if (action == ActionType.Spell || action == ActionType.Attack || action == ActionType.Move)
                RemoveInvisibility(m);
        }
        else if (level == InvisibilityLevel.Greater)
        {
            // Only breaks on damage dealt
            if (action == ActionType.DamageDealt)
                RemoveInvisibility(m);
        }
        // Perfect and True don't break
    }

    public static void RemoveInvisibility(Mobile m)
    {
        if (m_Invisible.ContainsKey(m))
        {
            m.Hidden = false;
            m.RevealingAction();
            m_Invisible.Remove(m);
        }
    }
}

public enum ActionType { Spell, Attack, Move, DamageDealt }
```

**2. Illusory Copies (Mirror Image)**
```csharp
public class IllusoryDouble : BaseCreature
{
    private Mobile m_Master;

    public IllusoryDouble(Mobile master, int hpPercent, int damagePercent) : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
    {
        m_Master = master;

        // Copy appearance
        Body = master.Body;
        Hue = master.Hue;
        Name = master.Name;

        // Reduced stats
        SetStr((int)(master.Str * (damagePercent / 100.0)));
        SetDex(master.Dex);
        SetInt(master.Int);

        SetHits((int)(master.HitsMax * (hpPercent / 100.0)));

        // Copy equipment visuals
        foreach (Item item in master.Items)
        {
            if (item.Layer != Layer.Bank && item.Layer != Layer.Backpack)
            {
                Item copy = new Item(item.ItemID);
                copy.Hue = item.Hue;
                copy.Layer = item.Layer;
                AddItem(copy);
            }
        }
    }

    public override void OnDeath(Container c)
    {
        Effects.SendLocationEffect(Location, Map, 0x3728, 10, 10);
        PlaySound(0x1F2);
        // Illusory - just vanish, no loot
        base.OnDeath(c);
    }
}

// Legion of Mirrors: Create 10 copies
for (int i = 0; i < 10; i++)
{
    IllusoryDouble copy = new IllusoryDouble(Caster, 30, 30); // 30% HP, 30% damage
    copy.MoveToWorld(GetSpawnLocation(Caster, 3), Caster.Map);
    Timer.DelayCall(TimeSpan.FromSeconds(60.0), () => copy.Delete());
}
```

**3. Mind Control**
```csharp
public static class MindControlManager
{
    private static Dictionary<Mobile, MindControlContext> m_Controlled = new Dictionary<Mobile, MindControlContext>();

    public static void ControlMind(Mobile caster, Mobile target, TimeSpan duration)
    {
        // Check immunities
        if (target is PlayerMobile)
        {
            target.SendMessage("Your mind is too strong to control!");
            return;
        }

        if (target is BaseCreature creature)
        {
            // Store original state
            Mobile originalMaster = creature.ControlMaster;

            creature.Controlled = true;
            creature.ControlMaster = caster;
            creature.ControlOrder = OrderType.Follow;

            MindControlContext context = new MindControlContext(caster, creature, originalMaster, duration);
            m_Controlled[creature] = context;

            Timer.DelayCall(duration, () => EndControl(creature));
        }
    }

    public static void EndControl(Mobile controlled)
    {
        if (m_Controlled.ContainsKey(controlled) && controlled is BaseCreature creature)
        {
            MindControlContext context = m_Controlled[controlled];

            creature.Controlled = false;
            creature.ControlMaster = null;
            creature.SendMessage("You break free from mind control!");

            m_Controlled.Remove(controlled);
        }
    }
}

public class MindControlContext
{
    public Mobile Caster;
    public BaseCreature Target;
    public Mobile OriginalMaster;
    public DateTime EndTime;

    public MindControlContext(Mobile caster, BaseCreature target, Mobile originalMaster, TimeSpan duration)
    {
        Caster = caster;
        Target = target;
        OriginalMaster = originalMaster;
        EndTime = DateTime.UtcNow + duration;
    }
}
```

**4. Confusion Mechanics**
```csharp
public static class ConfusionManager
{
    private static HashSet<Mobile> m_Confused = new HashSet<Mobile>();

    public static void ApplyConfusion(Mobile target, TimeSpan duration)
    {
        if (!m_Confused.Contains(target))
        {
            m_Confused.Add(target);
            target.SendMessage("You are confused!");

            Timer.DelayCall(duration, () => RemoveConfusion(target));
        }
    }

    public static void RemoveConfusion(Mobile target)
    {
        if (m_Confused.Contains(target))
        {
            m_Confused.Remove(target);
            target.SendMessage("Your confusion fades.");
        }
    }

    public static bool IsConfused(Mobile m)
    {
        return m_Confused.Contains(m);
    }

    // In BaseCreature OnThink or attack selection:
    public static Mobile GetConfusedTarget(Mobile confused)
    {
        if (!IsConfused(confused))
            return null;

        // 50% chance to attack random nearby target (including allies)
        if (Utility.RandomDouble() < 0.5)
        {
            IPooledEnumerable eable = confused.Map.GetMobilesInRange(confused.Location, 10);
            List<Mobile> targets = new List<Mobile>();
            foreach (Mobile m in eable)
            {
                if (m != confused && m.Alive)
                    targets.Add(m);
            }
            eable.Free();

            if (targets.Count > 0)
                return targets[Utility.Random(targets.Count)];
        }

        return null;
    }
}
```

**5. Phase Shift (Intangible)**
```csharp
public static class PhaseShiftManager
{
    private static HashSet<Mobile> m_Phased = new HashSet<Mobile>();

    public static void ApplyPhaseShift(Mobile m, TimeSpan duration)
    {
        m_Phased.Add(m);
        m.SendMessage("You become intangible!");

        // Immunity to physical damage
        ResistanceMod physImmu = new ResistanceMod(ResistanceType.Physical, 100);
        m.AddResistanceMod(physImmu);

        // Visual: ethereal appearance
        m.Hue = 0x8B0;

        Timer.DelayCall(duration, () =>
        {
            m_Phased.Remove(m);
            m.RemoveResistanceMod(physImmu);
            m.Hue = 0; // Restore
            m.SendMessage("You return to physical form.");
        });
    }

    public static bool IsPhased(Mobile m)
    {
        return m_Phased.Contains(m);
    }
}
```

**6. Reality Warp (Swap Positions)**
```csharp
public void RealityWarp()
{
    List<Mobile> allies = new List<Mobile>();
    List<Mobile> enemies = new List<Mobile>();

    IPooledEnumerable eable = Caster.Map.GetMobilesInRange(Caster.Location, 20);
    foreach (Mobile m in eable)
    {
        if (m == Caster)
            continue;

        if (m.Alive)
        {
            if (Caster.CanBeBeneficial(m))
                allies.Add(m);
            else if (Caster.CanBeHarmful(m))
                enemies.Add(m);
        }
    }
    eable.Free();

    // Swap positions randomly
    List<Point3D> positions = new List<Point3D>();
    foreach (Mobile m in allies)
        positions.Add(m.Location);
    foreach (Mobile m in enemies)
        positions.Add(m.Location);

    // Shuffle positions
    for (int i = 0; i < positions.Count; i++)
    {
        int j = Utility.Random(positions.Count);
        Point3D temp = positions[i];
        positions[i] = positions[j];
        positions[j] = temp;
    }

    // Apply new positions
    int index = 0;
    foreach (Mobile m in allies)
    {
        m.MoveToWorld(positions[index], m.Map);
        Effects.SendLocationEffect(positions[index], m.Map, 0x3728, 10, 10);
        index++;
    }
    foreach (Mobile m in enemies)
    {
        m.MoveToWorld(positions[index], m.Map);
        Effects.SendLocationEffect(positions[index], m.Map, 0x3728, 10, 10);
        index++;
    }
}
```

**7. Illusory Terrain**
```csharp
public class IllusoryWallItem : Item
{
    public IllusoryWallItem() : base(0x0080) // Wall graphic
    {
        Movable = false;
        Name = "illusory wall";
        Hue = 0x8B0; // Shimmering
    }

    public override bool BlocksFit { get { return true; } } // Blocks line of sight
}

// Create 8x8 maze of illusory walls
for (int x = -4; x <= 4; x++)
{
    for (int y = -4; y <= 4; y++)
    {
        if (Utility.RandomDouble() < 0.3) // 30% chance per tile
        {
            Point3D loc = new Point3D(targetLoc.X + x, targetLoc.Y + y, targetLoc.Z);
            IllusoryWallItem wall = new IllusoryWallItem();
            wall.MoveToWorld(loc, Caster.Map);

            Timer.DelayCall(TimeSpan.FromSeconds(60.0), () => wall.Delete());
        }
    }
}
```

### ⚠️ Advanced Mechanics
- **Master of Puppets:** Control 5 enemies simultaneously - track multiple control contexts
- **Perfect Illusion:** Infinite illusions, unkillable copies - remove death handling
- **Illusory Apocalypse:** Mass fear - all enemies flee for 45s (screen-wide)

### 📝 Implementation Priority
**Phase 1:** Mind Spike, Blur, Invisibility, Minor Illusion
**Phase 2:** Illusory Double (1 copy), Confuse, Greater Invisibility
**Phase 3:** Mirror Image, Mind Control, Phase Shift
**Phase 4:** Legion of Mirrors, Reality Warp, Perfect Illusion

**Last Updated:** 2025-12-05 | **Status:** Design Complete - 0/32 Implemented
