# VYSTIA ARTIFICER SYSTEM
## Engineering, Constructs & Gadgets

**Version:** 1.0  
**Class:** Artificer  
**Primary Skill:** Engineering (ID 80)  
**Secondary Resources:** Steam (0-100), Charges (item-based)  
**Progression System:** Blueprint Mastery (Permanent)

---

# PART I: SYSTEM OVERVIEW

## 1.1 ServUO Technical Foundation

The Artificer system is built on proven ServUO mechanics:

| Vystia Feature | ServUO Foundation | Implementation |
|----------------|-------------------|----------------|
| Constructs | BaseCreature (Summoned/Controlled) | Timed summons with ControlSlots |
| Gadgets | Item with OnDoubleClick + Timer | Charged items with durability |
| Steam | PlayerMobile property | Integer 0-100, decay timer |
| Blueprints | Recipe system | PlayerMobile.AcquiredRecipes |
| Traps | Existing trap system | Custom BaseTrap extensions |
| Turrets | Stationary BaseCreature | FightMode.Closest, immobile |

## 1.2 Dual Progression Design

| System | Type | Purpose | Timeframe |
|--------|------|---------|-----------|
| **Engineering Skill** | 0-100 | Base craft success, gadget power | Permanent (standard skill) |
| **Steam** | 0-100 | In-combat resource for constructs | Per-session (decay OOC) |
| **Charges** | Per-item | Gadget activations | Consumed on use |
| **Blueprint Mastery** | Levels 1-32 | Unlock advanced schematics | Permanent (earned) |

---

## 1.3 Core Loop

```
COMBAT FLOW:
1. Deploy Construct (costs Steam + ControlSlots)
   ↓
2. Construct fights, generates Steam on kills
   ↓
3. Use Gadgets (costs Charges, amplified by Steam)
   ↓
4. Maintain constructs with Repair Kits
   ↓
5. At high Steam, deploy powerful constructs or gadgets
```

```
PROGRESSION FLOW:
1. Craft items, deploy constructs
   ↓
2. Earn Schematic Points from engineering activities
   ↓
3. Accumulate points toward Blueprint Mastery level
   ↓
4. Level up → Unlock new construct/gadget recipes
   ↓
5. Craft more advanced machines
```

---

# PART II: ENGINEERING SKILL

## 2.1 Skill Progression

| Engineering | Capability |
|-------------|------------|
| 0-29 | Basic repairs, simple gadgets |
| 30-49 | Clockwork Spider, basic traps |
| 50-69 | Repair Drone, Steam Turret, intermediate gadgets |
| 70-84 | Iron Golem, advanced traps, Steam Boiler |
| 85-99 | Siege Engine, master gadgets |
| 100 | **Master Engineer:** All constructs, -20% Steam costs |
| 110+ | Each point above 100: +1% construct HP/damage (max +20%) |

### Craft Success Formula
```
Success Chance = Engineering + (Tinkering / 2) + Tool Bonus
Exceptional Chance = (Engineering - 50) / 2 %
```

---

## 2.2 Integration with Tinkering

Engineering extends Tinkering for construct-specific items:

| Action | Primary Skill | Secondary Skill |
|--------|---------------|-----------------|
| Basic traps | Tinkering | — |
| Gadget crafting | Engineering | Tinkering |
| Construct assembly | Engineering | Blacksmithy |
| Construct repair | Engineering | Tinkering |
| Steam Core crafting | Engineering | Alchemy |

---

# PART III: STEAM SYSTEM

## 3.1 Resource Mechanics

**Steam** is the Artificer's in-combat secondary resource.

| Property | Value |
|----------|-------|
| Range | 0-100 |
| Base Generation | +5/sec when near Steam Boiler |
| Kill Generation | +10 per construct kill |
| Decay | -2/sec when >50 tiles from boiler, -5/sec OOC |
| Full Reset | 5 minutes out of combat |

### Steam Sources

| Source | Steam Gained | Notes |
|--------|--------------|-------|
| Steam Boiler (deployed) | +5/sec | Must be within 10 tiles |
| Construct kill | +10 | Any construct under your control |
| Gadget overclock | -15 to -30 | Consumes Steam for power |
| Steam Potion | +25 instant | Crafted, 60 sec cooldown |
| Steam Vent (dungeon) | +3/sec | Environmental, some dungeons |

### Steam Thresholds

| Steam | Unlocks |
|-------|---------|
| 20 | Basic construct deployment |
| 40 | Standard construct deployment |
| 60 | Advanced construct deployment, gadget overclock |
| 80 | Master construct deployment |
| 100 | **Overdrive:** All constructs gain +25% stats for 30 sec |

---

## 3.2 Steam Boiler (Deployable)

The Steam Boiler is the Artificer's key strategic item.

| Property | Value |
|----------|-------|
| Craft Requirement | 70 Engineering |
| Materials | 20 Iron Ingots, 5 Gears, 3 Steam Cores |
| Deploy Time | 3 seconds (interruptible) |
| HP | 200 |
| Duration | 10 minutes OR until destroyed |
| Cooldown | 5 minutes after destruction |
| Effect Radius | 10 tiles |

**Boiler Effects (within 10 tiles):**
- +5 Steam/sec to Artificer
- +10% damage to all friendly constructs
- Constructs heal 1% HP/sec
- Enables "Overdrive" at 100 Steam

**Tactical Notes:**
- Only one boiler active at a time
- Enemies can target and destroy the boiler
- Boiler is immobile once placed
- Picking up boiler ends its effects

---

# PART IV: CONSTRUCTS

## 4.1 The Body ID Problem

**Critical Constraint:** The UO client has a fixed set of ~1500 body IDs with pre-made animations. You cannot invent new graphics without custom client patches. Vystia constructs must use EXISTING body IDs.

### Available Construct-Like Body IDs in ServUO

| Body ID | Name | Animation Type | Suitable For |
|---------|------|----------------|--------------|
| **717** | Clockwork Scorpion | Monster | ✓ Already mechanical-themed |
| **714** | Iron Beetle | Monster | ✓ Mechanical beetle |
| **304** | Flesh Golem | Monster | Reskin as "Iron Golem" |
| **752** | Golem | Monster | ✓ Official golem |
| **574** | Blade Spirits | Monster | Turret (stationary) |
| **58** | Wisp | Monster | Repair Drone (floating) |
| **725** | Homunculus | Monster | Small construct |
| **28** | Giant Spider | Animal | Clockwork Spider (hue change) |
| **48** | Scorpion | Animal | Alternative scorpion |
| **60/61** | Drake | Monster | Clockwork Dragon |
| **14** | Earth Elemental | Monster | Heavy golem variant |

### The Hue Solution

Since we can't create new graphics, we use **hues** to distinguish constructs:

```csharp
public class ClockworkSpider : BaseCreature
{
    public ClockworkSpider() : base(AIType.AI_Melee, ...)
    {
        Body = 28;        // Giant Spider body
        Hue = 2500;       // Metallic bronze hue
        Name = "a clockwork spider";
    }
}
```

**Recommended Construct Hues:**
| Hue | Appearance | Use For |
|-----|------------|---------|
| 2500 | Bronze/Copper | Basic constructs |
| 2501 | Iron Gray | Iron Golem |
| 2515 | Brass | Mid-tier constructs |
| 2425 | Valorite Blue | Advanced constructs |
| 1161 | Steam White | Titan |

---

## 4.2 Construct Implementation

### BaseCreature Summon Pattern

All constructs use ServUO's existing summon system:

```csharp
public class ClockworkScorpion : BaseCreature
{
    [Constructable]
    public ClockworkScorpion() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
    {
        // Use EXISTING Body ID 717 (official Clockwork Scorpion)
        Body = 717;
        Name = "a clockwork scorpion";
        
        // No hue needed - already looks mechanical
        
        SetStr(80, 100);
        SetDex(60, 80);
        SetInt(20, 30);
        
        SetHits(120);
        SetDamage(18, 24);
        
        // Construct properties
        Tamable = false;
        ControlSlots = 2;
    }
    
    // Constructs are dispellable
    public override bool IsDispellable => true;
    
    // Custom: Cannot be healed by magic
    public override bool CanBeHealed => false;
    
    // Poison attack
    public override Poison HitPoison => Poison.Greater;
}
```

### Summoning Constructs

```csharp
// In player's Engineering skill or item
public static void DeployConstruct(Mobile caster, Type constructType, int steamCost)
{
    PlayerMobile pm = caster as PlayerMobile;
    
    // Check Steam resource
    if (pm.Steam < steamCost)
    {
        pm.SendMessage("You don't have enough steam.");
        return;
    }
    
    // Check control slots
    BaseCreature construct = (BaseCreature)Activator.CreateInstance(constructType);
    if (pm.Followers + construct.ControlSlots > pm.FollowersMax)
    {
        pm.SendMessage("You have too many followers.");
        construct.Delete();
        return;
    }
    
    // Deduct Steam
    pm.Steam -= steamCost;
    
    // Summon with duration
    TimeSpan duration = TimeSpan.FromMinutes(10);
    BaseCreature.Summon(construct, true, caster, caster.Location, 0x216, duration);
    
    pm.SendMessage("Your construct whirs to life!");
}
```

---

## 4.3 Construct Roster (With Real Body IDs)

### Tier 1: Scout Constructs (Engineering 30+)

#### Clockwork Spider
| Property | Value |
|----------|-------|
| **Body ID** | **28** (Giant Spider) |
| **Hue** | **2500** (Bronze) |
| Engineering | 30 |
| Steam Cost | 15 |
| Control Slots | 1 |
| Duration | 10 minutes |
| HP | 50 |
| Damage | 8-12 |
| Speed | Fast (0.175, 0.35 delay) |
| Special | Detects hidden, web attack |

**Animation Notes:** Uses spider walk/attack anims. Web is existing `Animate(5, 5, 1, true, false, 0)`.

---

#### Clockwork Beetle
| Property | Value |
|----------|-------|
| **Body ID** | **714** (Iron Beetle) |
| **Hue** | **0** (Default - already mechanical) |
| Engineering | 40 |
| Steam Cost | 20 |
| Control Slots | 1 |
| Duration | 10 minutes |
| HP | 80 |
| Damage | 10-15 |
| Special | Has `Container Backpack` for 50 stones |

**Animation Notes:** Official SA expansion beetle. Already has metallic appearance.

---

### Tier 2: Combat Constructs (Engineering 50+)

#### Repair Drone
| Property | Value |
|----------|-------|
| **Body ID** | **58** (Wisp) |
| **Hue** | **2515** (Brass) |
| Engineering | 50 |
| Steam Cost | 25 |
| Control Slots | 1 |
| Duration | 8 minutes |
| HP | 40 |
| Damage | 0 |
| Special | Heals constructs 5 HP/sec via Timer |

**Implementation:**
```csharp
public override void OnThink()
{
    base.OnThink();
    
    // Find lowest HP construct owned by same master
    if (DateTime.UtcNow >= m_NextHealTime)
    {
        BaseCreature target = FindLowestHPConstruct();
        if (target != null && target.Hits < target.HitsMax)
        {
            target.Hits = Math.Min(target.Hits + 5, target.HitsMax);
            target.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
        }
        m_NextHealTime = DateTime.UtcNow + TimeSpan.FromSeconds(1);
    }
}
```

---

#### Steam Turret
| Property | Value |
|----------|-------|
| **Body ID** | **574** (Blade Spirits) |
| **Hue** | **2501** (Iron Gray) |
| Engineering | 55 |
| Steam Cost | 30 |
| Control Slots | 2 |
| Duration | 5 minutes |
| HP | 100 |
| Damage | 15-25 |
| Range | 8 tiles |
| Special | Immobile, ranged attack |

**Implementation:**
```csharp
public class SteamTurret : BaseCreature
{
    public SteamTurret() : base(AIType.AI_Archer, FightMode.Closest, 10, 8, 0.2, 0.4)
    {
        Body = 574;
        Hue = 2501;
        Name = "a steam turret";
        
        // IMMOBILE
        CantWalk = true;
        Frozen = true;
    }
    
    // Custom ranged attack using MovingParticles
    public override void OnGaveMeleeAttack(Mobile defender)
    {
        // Steam blast visual
        this.MovingParticles(defender, 0x36D4, 7, 0, false, true, 2501, 0, 9502, 1, 0, 0);
        // Sound
        this.PlaySound(0x15E);
    }
}
```

---

#### Clockwork Scorpion
| Property | Value |
|----------|-------|
| **Body ID** | **717** (Clockwork Scorpion) |
| **Hue** | **0** (Default) |
| Engineering | 60 |
| Steam Cost | 35 |
| Control Slots | 2 |
| Duration | 8 minutes |
| HP | 120 |
| Damage | 18-24 |
| Special | Poison (Greater), armor pierce |

**Animation Notes:** This is an OFFICIAL UO body - the Clockwork Scorpion from SA expansion. No modifications needed!

---

### Tier 3: Heavy Constructs (Engineering 70+)

#### Iron Golem
| Property | Value |
|----------|-------|
| **Body ID** | **752** (Golem) or **304** (Flesh Golem) |
| **Hue** | **2501** (Iron Gray) |
| Engineering | 70 |
| Steam Cost | 50 |
| Control Slots | 3 |
| Duration | 10 minutes |
| HP | 400 |
| Damage | 30-45 |
| Speed | Slow (0.4, 0.6 delay) |
| Special | 50% physical resist, Taunt ability |

**Taunt Implementation:**
```csharp
private DateTime m_NextTaunt;

public override void OnThink()
{
    base.OnThink();
    
    if (DateTime.UtcNow >= m_NextTaunt && Combatant != null)
    {
        // AoE taunt
        foreach (Mobile m in GetMobilesInRange(5))
        {
            if (m is BaseCreature bc && bc.Combatant != this && !bc.Controlled)
            {
                bc.Combatant = this;
                bc.FocusMob = this;
            }
        }
        
        this.PublicOverheadMessage(MessageType.Emote, 0x3B2, false, "*clanks menacingly*");
        this.PlaySound(0x2F3); // Metal clang
        
        m_NextTaunt = DateTime.UtcNow + TimeSpan.FromSeconds(15);
    }
}
```

---

#### Clockwork Dragon
| Property | Value |
|----------|-------|
| **Body ID** | **60** (Drake) |
| **Hue** | **2425** (Valorite Blue) |
| Engineering | 80 |
| Steam Cost | 60 |
| Control Slots | 4 |
| Duration | 8 minutes |
| HP | 350 |
| Damage | 25-40 melee |
| Special | Fire breath (existing `HasBreath = true`) |

**Implementation:**
```csharp
public class ClockworkDragon : BaseCreature
{
    public ClockworkDragon() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
    {
        Body = 60;  // Drake
        Hue = 2425; // Valorite
        Name = "a clockwork dragon";
        
        // Enable breath attack (built into BaseCreature!)
        SetDamage(25, 40);
    }
    
    public override bool HasBreath => true;
    public override int BreathFireDamage => 100;
    public override double BreathDamageScalar => 0.5;
}
```

---

### Tier 4: Siege Constructs (Engineering 85+)

#### Siege Engine
| Property | Value |
|----------|-------|
| **Body ID** | **14** (Earth Elemental) |
| **Hue** | **2501** (Iron Gray) |
| Engineering | 85 |
| Steam Cost | 70 |
| Control Slots | 4 |
| Duration | 5 minutes |
| HP | 250 |
| Damage | 80-120 |
| Range | 12 tiles |
| Special | Immobile, siege damage to structures |

**Animation Notes:** Earth Elemental body works for "massive siege machine" because of its bulky, slow appearance. CantWalk = true makes it stationary.

---

#### Titan Construct
| Property | Value |
|----------|-------|
| **Body ID** | **752** (Golem) |
| **Hue** | **1161** (Steam White) |
| Engineering | 100 |
| Steam Cost | 80 |
| Control Slots | 5 |
| Duration | 10 minutes |
| HP | 800 |
| Damage | 50-70 |
| Special | Earthquake stomp (AoE stun) |

**Earthquake Implementation:**
```csharp
private DateTime m_NextStomp;

public void EarthquakeStomp()
{
    if (DateTime.UtcNow < m_NextStomp)
        return;
        
    this.Animate(4, 5, 1, true, false, 0); // Stomp animation
    this.PlaySound(0x2F3);
    
    // AoE damage + stun
    foreach (Mobile m in GetMobilesInRange(4))
    {
        if (m != this && m != ControlMaster && CanBeHarmful(m))
        {
            DoHarmful(m);
            AOS.Damage(m, this, Utility.RandomMinMax(20, 30), 100, 0, 0, 0, 0);
            m.Freeze(TimeSpan.FromSeconds(2));
            m.SendMessage("You are stunned by the earthquake!");
        }
    }
    
    // Ground effect
    Effects.SendLocationParticles(
        EffectItem.Create(Location, Map, EffectItem.DefaultDuration),
        0x3728, 10, 10, 2023);
    
    m_NextStomp = DateTime.UtcNow + TimeSpan.FromSeconds(30);
}
```

---

## 4.4 Construct Summary Table (With Body IDs)

| Construct | Body ID | Hue | Eng | Steam | Slots | HP | Role |
|-----------|---------|-----|-----|-------|-------|-----|------|
| Clockwork Spider | 28 (Spider) | 2500 | 30 | 15 | 1 | 50 | Scout |
| Clockwork Beetle | 714 (Iron Beetle) | 0 | 40 | 20 | 1 | 80 | Utility |
| Repair Drone | 58 (Wisp) | 2515 | 50 | 25 | 1 | 40 | Healer |
| Steam Turret | 574 (Blade Spirits) | 2501 | 55 | 30 | 2 | 100 | Ranged |
| Clockwork Scorpion | 717 (Official) | 0 | 60 | 35 | 2 | 120 | Poison |
| Iron Golem | 752 (Golem) | 2501 | 70 | 50 | 3 | 400 | Tank |
| Clockwork Dragon | 60 (Drake) | 2425 | 80 | 60 | 4 | 350 | Breath |
| Siege Engine | 14 (Earth Elem) | 2501 | 85 | 70 | 4 | 250 | Siege |
| Titan Construct | 752 (Golem) | 1161 | 100 | 80 | 5 | 800 | Ultimate |

---

## 4.4 Construct Repair

Constructs cannot be healed by magic or bandages. They require Engineering repair.

| Method | Heal Amount | Requirement |
|--------|-------------|-------------|
| Repair Kit (small) | 25 HP | Tinkering 30 |
| Repair Kit (large) | 75 HP | Tinkering 60 |
| Master Repair Kit | Full heal | Tinkering 100 |
| Repair Drone | 5 HP/sec | Engineering 50 |
| Steam Boiler proximity | 1% HP/sec | Engineering 70 |

**Repair Kit Materials:**
- Small: 5 Gears, 3 Iron Ingots
- Large: 10 Gears, 5 Bronze Ingots
- Master: 20 Gears, 10 Valorite Ingots, 1 Steam Core

---

# PART V: GADGETS

## 5.1 Gadget System (Item Implementation)

Gadgets are charged items using existing ServUO patterns.

### Item ID Selection

UO items have fixed graphics. Gadgets use existing item IDs that "look right":

| Item ID (Hex) | Appearance | Use For |
|---------------|------------|---------|
| 0x1053-0x1054 | Clock parts | Generic gadgets |
| 0x1EBC | Trap kit | Grenades |
| 0x0E27 | Potion flask | Canisters |
| 0x14F0 | Deed/Scroll | Schematics |
| 0x1BEF | Gears | Components |
| 0x0DF8 | Lantern | Light gadgets |
| 0x0E41 | Toolbox | Workbench |
| 0x14FC | Crystal | Tesla devices |

### Base Gadget Pattern

```csharp
public abstract class BaseGadget : Item
{
    private int m_Charges;
    private DateTime m_NextUse;
    private int m_EngineeringRequired;
    
    [CommandProperty(AccessLevel.GameMaster)]
    public int Charges
    {
        get { return m_Charges; }
        set { m_Charges = value; InvalidateProperties(); }
    }
    
    public BaseGadget(int itemID, int charges, int engRequired) : base(itemID)
    {
        m_Charges = charges;
        m_EngineeringRequired = engRequired;
        Weight = 1.0;
        LootType = LootType.Regular;
    }
    
    public override void OnDoubleClick(Mobile from)
    {
        if (!IsChildOf(from.Backpack))
        {
            from.SendLocalizedMessage(1042001); // Must be in pack
            return;
        }
        
        if (from.Skills[SkillName.Tinkering].Value < m_EngineeringRequired)
        {
            from.SendMessage("You lack the engineering skill to use this.");
            return;
        }
        
        if (DateTime.UtcNow < m_NextUse)
        {
            from.SendMessage("The gadget is still cooling down.");
            return;
        }
        
        if (m_Charges <= 0)
        {
            from.SendMessage("This gadget has no charges remaining.");
            return;
        }
        
        // Subclass implements targeting/effect
        OnActivate(from);
    }
    
    protected abstract void OnActivate(Mobile from);
    
    protected void ConsumeCharge(Mobile from, TimeSpan cooldown)
    {
        m_Charges--;
        m_NextUse = DateTime.UtcNow + cooldown;
        
        if (m_Charges <= 0)
            Delete();
    }
    
    public override void GetProperties(ObjectPropertyList list)
    {
        base.GetProperties(list);
        list.Add(1060741, m_Charges.ToString()); // charges: ~1_val~
    }
}
```

---

## 5.2 Gadget Roster (With Implementation)

### Shock Grenade

```csharp
public class ShockGrenade : BaseGadget
{
    public ShockGrenade() : base(0x1EBC, 5, 30) // Trap kit graphic
    {
        Name = "shock grenade";
        Hue = 1154; // Electric blue
    }
    
    protected override void OnActivate(Mobile from)
    {
        from.Target = new InternalTarget(this);
        from.SendMessage("Target the area to throw the grenade.");
    }
    
    private class InternalTarget : Target
    {
        private ShockGrenade m_Grenade;
        
        public InternalTarget(ShockGrenade grenade) : base(10, true, TargetFlags.None)
        {
            m_Grenade = grenade;
        }
        
        protected override void OnTarget(Mobile from, object targeted)
        {
            IPoint3D p = targeted as IPoint3D;
            if (p == null) return;
            
            Point3D loc = new Point3D(p);
            
            // Throwing animation
            from.Animate(11, 5, 1, true, false, 0);
            from.PlaySound(0x5D2);
            
            // Projectile effect
            Effects.SendMovingParticles(
                from, 
                new Entity(Serial.Zero, loc, from.Map),
                0x36D4, 7, 0, false, false, 1154, 0, 9502, 1, 0, EffectLayer.Waist, 0);
            
            // Delayed explosion
            Timer.DelayCall(TimeSpan.FromSeconds(0.5), () =>
            {
                // Explosion effect
                Effects.SendLocationParticles(
                    EffectItem.Create(loc, from.Map, EffectItem.DefaultDuration),
                    0x36BD, 20, 10, 1154, 0, 5044, 0);
                Effects.PlaySound(loc, from.Map, 0x307);
                
                // AoE damage
                foreach (Mobile m in from.Map.GetMobilesInRange(loc, 3))
                {
                    if (m != from && from.CanBeHarmful(m))
                    {
                        from.DoHarmful(m);
                        int damage = Utility.RandomMinMax(20, 30);
                        AOS.Damage(m, from, damage, 0, 0, 0, 0, 100); // Energy damage
                    }
                }
            });
            
            m_Grenade.ConsumeCharge(from, TimeSpan.FromSeconds(10));
        }
    }
}
```

| Property | Value |
|----------|-------|
| **Item ID** | **0x1EBC** (Trap Kit) |
| **Hue** | **1154** (Electric Blue) |
| Engineering | 30 |
| Charges | 5 |
| Cooldown | 10 seconds |
| Effect | 20-30 energy damage, 3 tile radius |
| Overclock (15 Steam) | +50% damage, stun 1 sec |

**Materials:** 3 Gears, 2 Sulfurous Ash, 1 Black Pearl

---

### Flame Canister

| Property | Value |
|----------|-------|
| **Item ID** | **0x0E27** (Potion Flask) |
| **Hue** | **1161** (Fire Orange) |
| Engineering | 45 |
| Charges | 3 |
| Cooldown | 15 seconds |
| Effect | Fire field - 15 damage/sec for 5 sec |

**Implementation Notes:** Uses `FireFieldSpell.InternalItem` pattern - creates ground effect that damages.

```csharp
// Creates fire field at target location
Timer.DelayCall(TimeSpan.FromSeconds(0.5), () =>
{
    new FireFieldItem(loc, from, from.Map, TimeSpan.FromSeconds(5), 15);
});
```

---

### Tesla Coil (Thrown)

| Property | Value |
|----------|-------|
| **Item ID** | **0x14FC** (Crystal) |
| **Hue** | **1152** (Lightning White) |
| Engineering | 60 |
| Charges | 3 |
| Cooldown | 20 seconds |
| Effect | Chain lightning: 40 damage, jumps to 3 targets |

**Chain Lightning Implementation:**
```csharp
private void ChainLightning(Mobile from, Mobile firstTarget, int jumps)
{
    List<Mobile> hit = new List<Mobile> { firstTarget };
    Mobile current = firstTarget;
    
    for (int i = 0; i < jumps; i++)
    {
        Mobile next = FindNextTarget(current, hit, 4);
        if (next == null) break;
        
        // Lightning effect between targets
        Effects.SendBoltEffect(current, true, 1154);
        AOS.Damage(current, from, 40, 0, 0, 0, 0, 100);
        
        hit.Add(next);
        current = next;
    }
    
    // Hit final target
    Effects.SendBoltEffect(current, true, 1154);
    AOS.Damage(current, from, 40, 0, 0, 0, 0, 100);
}
```

---

### Smoke Bomb

| Property | Value |
|----------|-------|
| **Item ID** | **0x0F0D** (Smoke Bomb - exists in UO) |
| **Hue** | **0** (Default gray) |
| Engineering | 35 |
| Charges | 5 |
| Cooldown | 30 seconds |
| Effect | Creates `InternalSmoke` items that debuff hit chance |

**Implementation Notes:** Smoke is done with multiple `Static` items (ID 0x3728) that have a timer and check combat nearby.

---

### Force Shield Generator

| Property | Value |
|----------|-------|
| **Item ID** | **0x1053** (Clock parts) |
| **Hue** | **2425** (Valorite blue) |
| Engineering | 55 |
| Charges | 3 |
| Cooldown | 45 seconds |
| Effect | `ResistanceMod` buff + damage absorption |

```csharp
protected override void OnActivate(Mobile from)
{
    // Visual bubble effect
    from.FixedParticles(0x375A, 10, 15, 5037, EffectLayer.Waist);
    from.PlaySound(0x1E9);
    
    // Apply resistance buff
    ResistanceMod mod = new ResistanceMod(ResistanceType.Physical, 25);
    from.AddResistanceMod(mod);
    
    // Track absorbed damage via custom property
    from.SendMessage("A shimmering shield surrounds you!");
    
    // Remove after duration
    Timer.DelayCall(TimeSpan.FromSeconds(10), () =>
    {
        from.RemoveResistanceMod(mod);
        from.SendMessage("Your shield fades.");
    });
    
    ConsumeCharge(from, TimeSpan.FromSeconds(45));
}
```

---

### Grappling Hook

| Property | Value |
|----------|-------|
| **Item ID** | **0x14F8** (Rope) |
| **Hue** | **2500** (Bronze) |
| Engineering | 40 |
| Charges | 10 |
| Cooldown | 5 seconds |
| Effect | `Mobile.MoveToWorld()` to target location |

```csharp
protected override void OnTarget(Mobile from, object targeted)
{
    IPoint3D p = targeted as IPoint3D;
    if (p == null) return;
    
    Point3D loc = new Point3D(p);
    
    // Check LOS and distance
    if (!from.InLOS(loc) || from.GetDistanceToSqrt(loc) > 10)
    {
        from.SendMessage("You can't reach that location.");
        return;
    }
    
    // Rope visual
    Effects.SendMovingParticles(
        from,
        new Entity(Serial.Zero, loc, from.Map),
        0x14F8, 10, 0, false, false, 0, 0, 9502, 1, 0, EffectLayer.Head, 0);
    
    from.PlaySound(0x4A);
    
    // Pull player to location
    Timer.DelayCall(TimeSpan.FromSeconds(0.3), () =>
    {
        from.MoveToWorld(loc, from.Map);
        from.PlaySound(0x51);
    });
    
    m_Hook.ConsumeCharge(from, TimeSpan.FromSeconds(5));
}
```

---

### Emergency Teleporter

| Property | Value |
|----------|-------|
| **Item ID** | **0x0E29** (Recall rune appearance) |
| **Hue** | **1152** (Bright white) |
| Engineering | 75 |
| Charges | 1 |
| Cooldown | 5 minutes |
| Effect | `Recall` effect to marked location |

**Implementation Notes:** Store `Point3D` and `Map` on item, uses `BaseCreature.TeleportTo()` pattern.

---

### Siege Bomb

| Property | Value |
|----------|-------|
| **Item ID** | **0x0F0F** (Powder keg appearance) |
| **Hue** | **1157** (Deep red) |
| Engineering | 80 |
| Charges | 1 |
| Cooldown | 60 seconds |
| Effect | Massive AoE + structure damage |

```csharp
// Explosion effect
Effects.SendLocationParticles(
    EffectItem.Create(loc, from.Map, EffectItem.DefaultDuration),
    0x36BD, 20, 10, 5044);
Effects.PlaySound(loc, from.Map, 0x307);

// Damage all in range
foreach (Mobile m in from.Map.GetMobilesInRange(loc, 5))
{
    if (from.CanBeHarmful(m))
    {
        int damage = Utility.RandomMinMax(100, 150);
        AOS.Damage(m, from, damage, 100, 0, 0, 0, 0); // Physical
    }
}

// Damage items (for siege)
foreach (Item item in from.Map.GetItemsInRange(loc, 5))
{
    if (item is IDamageableItem di)
    {
        di.Damage(200, from);
    }
}
```

---

## 5.3 Gadget Summary Table (With Item IDs)

| Gadget | Item ID | Hue | Eng | Charges | Type |
|--------|---------|-----|-----|---------|------|
| Shock Grenade | 0x1EBC | 1154 | 30 | 5 | Offense |
| Smoke Bomb | 0x0F0D | 0 | 35 | 5 | Defense |
| Grappling Hook | 0x14F8 | 2500 | 40 | 10 | Utility |
| Flame Canister | 0x0E27 | 1161 | 45 | 3 | Offense |
| Force Shield | 0x1053 | 2425 | 55 | 3 | Defense |
| Tesla Coil | 0x14FC | 1152 | 60 | 3 | Offense |
| Decoy Projector | 0x1053 | 0 | 65 | 2 | Defense |
| Emergency Teleporter | 0x0E29 | 1152 | 75 | 1 | Utility |
| Siege Bomb | 0x0F0F | 1157 | 80 | 1 | Offense |

---

# PART VI: TRAPS

## 6.1 Trap System (BaseTrap Extension)

Traps use ServUO's existing trap mechanics with Engineering enhancements.

| Property | Trap Behavior |
|----------|---------------|
| Placement | Ground-targeted, 4 sec deploy time |
| Detection | Detecting Hidden skill reveals traps |
| Trigger | Enemy walks within 1 tile |
| Duration | 5 minutes or until triggered |
| Limit | 1 trap active per Artificer |

---

## 6.2 Trap Roster

| Trap | Eng | Damage | Effect | Materials |
|------|-----|--------|--------|-----------|
| Spike Trap | 40 | 25-35 | None | 5 Iron, 3 Gears |
| Bear Trap | 50 | 20-30 | Root 3 sec | 8 Iron, 5 Gears |
| Poison Dart Trap | 55 | 15-20 | Level 4 Poison | 5 Iron, 3 Nightshade |
| Explosive Trap | 65 | 50-70 | 3 tile AoE | 10 Iron, 5 Black Powder |
| Tesla Trap | 75 | 40-60 energy | Chain to 2 targets | 10 Copper, 5 Gears, 1 Crystal |
| Void Trap | 90 | 80-100 | Silence 5 sec | 15 Voidforged, 10 Gears |
| Legendary Trap | 100 | 120-160 | Knockback + stun 2 sec | 20 Valorite, 10 Gears, 1 Elemental Core |

### Trap Damage Scaling
```
Trap Damage = Base × (1 + Engineering/200)
```
GM Engineering = +50% trap damage.

---

# PART VII: BLUEPRINT MASTERY

## 7.1 Overview

**Blueprint Mastery** is a permanent progression system that unlocks advanced schematics.

| Property | Value |
|----------|-------|
| Maximum Level | 32 |
| Points per Level | 1 |
| Unlock Type | Recipe acquisition |

### What Blueprint Mastery Unlocks

Unlike the Bard's Song Mastery (which improves existing songs), Blueprint Mastery unlocks **new recipes** for constructs and gadgets.

| Level | Unlock |
|-------|--------|
| 1 | Clockwork Spider schematic |
| 3 | Shock Grenade schematic |
| 5 | Smoke Bomb schematic |
| 7 | Clockwork Beetle schematic |
| 9 | Grappling Hook schematic |
| 11 | Repair Drone schematic |
| 13 | Flame Canister schematic |
| 15 | Steam Turret schematic |
| 17 | Force Shield Generator schematic |
| 19 | Clockwork Scorpion schematic |
| 21 | Tesla Coil schematic |
| 23 | Decoy Projector schematic |
| 25 | Iron Golem schematic |
| 27 | Emergency Teleporter schematic |
| 28 | Clockwork Dragon schematic |
| 29 | Siege Engine schematic |
| 30 | Siege Bomb schematic |
| 31 | Advanced Titan Frame schematic |
| 32 | Titan Construct schematic |

---

## 7.2 Schematic Points

**Schematic Points** are earned through Engineering activities.

### Earning Schematic Points

| Activity | Points Earned |
|----------|---------------|
| Construct kill | 50% of creature difficulty |
| Successful trap trigger | 25 per victim |
| Gadget use in combat | 10 per activation |
| Exceptional craft (Engineering) | 20 per item |
| Repair construct (significant) | 5 per 50 HP repaired |
| Deploy Steam Boiler | 15 per deployment |

### Schematic Points Required

| Level | Total Points | Level | Total Points |
|-------|--------------|-------|--------------|
| 1 | 500 | 17 | 12,000 |
| 2 | 1,000 | 18 | 13,000 |
| 3 | 1,600 | 19 | 14,000 |
| 4 | 2,200 | 20 | 15,000 |
| 5 | 2,900 | 21 | 16,500 |
| 6 | 3,600 | 22 | 18,000 |
| 7 | 4,400 | 23 | 19,500 |
| 8 | 5,200 | 24 | 21,000 |
| 9 | 6,100 | 25 | 23,000 |
| 10 | 7,000 | 26 | 25,000 |
| 11 | 8,000 | 27 | 27,000 |
| 12 | 9,000 | 28 | 29,000 |
| 13 | 10,000 | 29 | 31,000 |
| 14 | 10,500 | 30 | 33,000 |
| 15 | 11,000 | 31 | 35,000 |
| 16 | 11,500 | 32 | 37,500 |

**Estimated Time to Max:**
- Casual play (300 points/hour): ~125 hours
- Active Engineering focus (800 points/hour): ~47 hours

---

## 7.3 Schematic Acquisition

When you level Blueprint Mastery, you automatically learn the schematic for that level. Schematics can also be:

| Source | Schematic Type |
|--------|----------------|
| Blueprint Mastery levels | Standard constructs/gadgets |
| Boss drops | Regional variants (Frost Golem, Lava Turret, etc.) |
| Faction vendors (Honored+) | Faction-specific gadgets |
| Ancient Beings quests | Legendary schematics |
| Treasure maps | Random schematic scrolls |

---

# PART VIII: TALENTS

## 8.1 Overclock Mastery

**Overclock Mastery** enhances gadget overclock effects.

| Property | Value |
|----------|-------|
| Unlock Requirement | 70 Engineering |
| Maximum Levels | 8 |
| Cost per Level | 1,500 Schematic Points |

### Benefits per Level

| Benefit | Per Level | At Max (8) |
|---------|-----------|------------|
| Overclock Steam cost | -5% | -40% |
| Overclock effect bonus | +5% | +40% |
| Gadget charge capacity | +1 per 2 levels | +4 max charges |

---

## 8.2 Construct Commander

**Construct Commander** enhances construct performance.

| Property | Value |
|----------|-------|
| Unlock Requirement | 80 Engineering |
| Maximum Levels | 8 |
| Cost per Level | 2,000 Schematic Points |

### Benefits per Level

| Benefit | Per Level | At Max (8) |
|---------|-----------|------------|
| Construct HP | +5% | +40% |
| Construct damage | +3% | +24% |
| Construct duration | +1 minute | +8 minutes |
| Steam cost reduction | -2% | -16% |

---

## 8.3 Demolition Expert

**Demolition Expert** enhances traps and explosives.

| Property | Value |
|----------|-------|
| Unlock Requirement | 60 Engineering |
| Maximum Levels | 8 |
| Cost per Level | 1,000 Schematic Points |

### Benefits per Level

| Benefit | Per Level | At Max (8) |
|---------|-----------|------------|
| Trap damage | +8% | +64% |
| Trap duration | +1 minute | +8 minutes |
| Max active traps | +1 per 4 levels | +2 (3 total) |
| Explosive gadget radius | +0.5 tiles per 2 levels | +2 tiles |

---

# PART IX: BUILD EXAMPLES

## 9.1 The Swarm Master

**Philosophy:** Maximize construct quantity, overwhelm with numbers

| Investment | Value |
|------------|-------|
| Focus | Low-cost constructs |
| Preferred Constructs | Spiders, Beetles, Scorpions |
| Talent Priority | Construct Commander |

**Typical Deployment (5 ControlSlots):**
- 2× Clockwork Spider (2 slots)
- 1× Clockwork Scorpion (2 slots)
- 1× Repair Drone (1 slot)

**Gameplay:** Flood the field with cheap constructs, use Repair Drone to sustain.

---

## 9.2 The War Machine

**Philosophy:** Single powerful construct + gadget support

| Investment | Value |
|------------|-------|
| Focus | High-tier constructs |
| Preferred Construct | Iron Golem or Titan |
| Talent Priority | Construct Commander, Overclock Mastery |

**Typical Deployment (5 ControlSlots):**
- 1× Iron Golem (3 slots) or Titan (5 slots)
- 1× Repair Drone (1 slot) if Golem
- Gadgets: Force Shield, Tesla Coil

**Gameplay:** Tank with Golem, buff with gadgets, overclock for burst.

---

## 9.3 The Trapper

**Philosophy:** Area denial, trap-focused PvP

| Investment | Value |
|------------|-------|
| Focus | Traps and defensive gadgets |
| Preferred Constructs | Steam Turret (stationary) |
| Talent Priority | Demolition Expert |

**Typical Deployment:**
- 3× Traps (with Demolition Expert 8)
- 1× Steam Turret
- Gadgets: Smoke Bomb, Decoy Projector

**Gameplay:** Control chokepoints, lure enemies into trap fields.

---

## 9.4 The Field Engineer

**Philosophy:** Utility and group support

| Investment | Value |
|------------|-------|
| Focus | Utility gadgets, group buffs |
| Preferred Constructs | Repair Drone, Beetle (pack mule) |
| Talent Priority | Overclock Mastery |

**Typical Deployment:**
- 1× Repair Drone
- 1× Clockwork Beetle
- Gadgets: Portable Workbench, Grappling Hook, Emergency Teleporter

**Gameplay:** Support role—repairs, crafts in field, extraction for group.

---

# PART X: RELIGION SYNERGY

## 10.1 Artificer + Cogsmith Creed

The Artificer's natural religion is **Cogsmith Creed**.

| Piety Tier | Synergy Bonus |
|------------|---------------|
| Adherent (50) | Steam Boiler regen +1/sec (6/sec total) |
| Devoted (200) | Construct HP +10% |
| Zealot (500) | Overclock costs -20% |
| Champion (900) | Titan Construct duration doubled |

---

## 10.2 Devotion Powers for Artificers

| Power | Tier | Effect |
|-------|------|--------|
| **Forge Blessing** | Devoted | +10% exceptional craft chance, perfect for gadgets |
| **Steam Burst** | Zealot | AoE damage that synergizes with construct swarm |
| **Machinist's Grace** | Champion | Full gear repair + construct full heal |

---

# PART XI: CRAFTING REFERENCE

## 11.1 Component Crafting

| Component | Tinkering | Materials | Use |
|-----------|-----------|-----------|-----|
| Basic Gears | 30 | 3 Iron Ingots | All constructs |
| Precision Gears | 60 | 5 Bronze Ingots | Advanced constructs |
| Masterwork Gears | 90 | 8 Valorite Ingots | Titan, Siege |
| Steam Core | 50 Eng | 10 Gears, 5 Sulfur, 3 Water | Power source |
| Elemental Core | 80 Eng | 1 Fire/Water/Earth/Air Core each | Legendary items |

---

## 11.2 Steam Core Crafting

Steam Cores are the universal power source for constructs.

| Property | Value |
|----------|-------|
| Engineering | 50 |
| Materials | 10 Gears, 5 Sulfurous Ash, 3 Bottles of Water |
| Craft Time | 10 seconds |
| Success Rate | Engineering + Alchemy / 2 |
| Exceptional | +20% construct duration when used |

---

# APPENDIX A: QUICK REFERENCE

## Construct Quick Reference

| Construct | Eng | Steam | Slots | HP | Role |
|-----------|-----|-------|-------|-----|------|
| Clockwork Spider | 30 | 15 | 1 | 50 | Scout |
| Clockwork Beetle | 40 | 20 | 1 | 80 | Utility |
| Repair Drone | 50 | 25 | 1 | 40 | Healer |
| Steam Turret | 55 | 30 | 2 | 100 | Ranged |
| Clockwork Scorpion | 60 | 35 | 2 | 120 | Poison DPS |
| Iron Golem | 70 | 50 | 3 | 400 | Tank |
| Clockwork Dragon | 80 | 60 | 4 | 350 | Breath |
| Siege Engine | 85 | 70 | 4 | 250 | Siege |
| Titan Construct | 100 | 80 | 5 | 800 | Ultimate |

## Gadget Quick Reference

| Gadget | Eng | Charges | Type |
|--------|-----|---------|------|
| Shock Grenade | 30 | 5 | Offense |
| Smoke Bomb | 35 | 5 | Defense |
| Grappling Hook | 40 | 10 | Utility |
| Flame Canister | 45 | 3 | Offense |
| Force Shield | 55 | 3 | Defense |
| Tesla Coil | 60 | 3 | Offense |
| Decoy Projector | 65 | 2 | Defense |
| Emergency Teleporter | 75 | 1 | Utility |
| Siege Bomb | 80 | 1 | Offense |

---

**Document Version:** 1.0  
**System:** Artificer / Engineering  
**Date:** January 2026
