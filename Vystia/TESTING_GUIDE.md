# Vystia Production Testing Guide

*Focus: Testing actual game mechanics with exact values from code*

---

## Quick Setup

```
[svs 100                    # Max all 26 Vystia skills
[skillcap 7200              # Ensure skill cap
[SetClassV2 Barbarian       # Start with a class
[CD Passive                 # Spawn test target
```

---

# PHASE 1: CLASS SYSTEM (25 Classes)

## TEST 1.1: Class Stats Apply Correctly

### Barbarian (Frosthold)
| | |
|---|---|
| **Command** | `[SetClassV2 Barbarian` |
| **Expected Stats** | STR 45 (cap 140), DEX 25 (cap 110), INT 10 (cap 100) |
| **Expected Resource** | Fury (max 100, decay 5/sec OOC after 3s delay) |
| **Expected Equipment** | FrostforgedBattleaxe, FrostforgedChainChest/Legs/Helm (Hue 1150) |
| **Expected Skills** | Melee, Tactics, Anatomy at 50.0 |
| **Failure Indicates** | PlayerClassV2.SetStartingStats() or EquipStartingGear() broken |

### Ice Mage (Frosthold)
| | |
|---|---|
| **Command** | `[SetClassV2 IceMage` |
| **Expected Stats** | STR 10 (cap 85), DEX 20 (cap 110), INT 50 (cap 155) |
| **Expected Resource** | Chill Stacks (per-target, max 5, duration 30s) |
| **Expected Equipment** | Wizard Robes, Arcane Hat, Wizard Staff, IceMageSpellbook (Hue 1154) |
| **Expected Skills** | IceMagic at 50.0 |
| **Failure Indicates** | Per-target resource system not initializing |

### Fighter (Ironclad)
| | |
|---|---|
| **Command** | `[SetClassV2 Fighter` |
| **Expected Stats** | STR 40 (cap 140), DEX 25 (cap 110), INT 15 (cap 100) |
| **Expected Resource** | Stances (4 types: Aggressive, Defensive, Balanced, Berserker) |
| **Expected Equipment** | Legionnaire's Cuirass/Greaves/Blade/Shield (Hue 2305) |
| **Failure Indicates** | Stance system not initializing |

### Monk (Ironclad)
| | |
|---|---|
| **Command** | `[SetClassV2 Monk` |
| **Expected Stats** | STR 25 (cap 115), DEX 35 (cap 130), INT 20 (cap 105) |
| **Expected Resource** | Chi (max 5, NO decay, persists OOC) |
| **Expected Skills** | Wrestling, Focus at 50.0 |
| **Failure Indicates** | Non-decay resource not persisting |

### Rogue (Multi-Regional)
| | |
|---|---|
| **Command** | `[SetClassV2 Rogue` |
| **Expected Stats** | STR 20 (cap 105), DEX 45 (cap 135), INT 15 (cap 105) |
| **Expected Resource** | Combo Points (per-target, max 5, duration 30s) |
| **Failure Indicates** | Per-target combo tracking broken |

---

## TEST 1.2: Secondary Resource Generation

### Fury Generation (Barbarian)
| | |
|---|---|
| **Setup** | `[SetClassV2 Barbarian`, `[ResetResources` |
| **Action** | Attack target 5 times, take 2 hits, get 1 kill |
| **Expected Generation** | +5 per hit dealt, +10 per hit taken, +20 per kill |
| **Expected Total** | 5×5 + 2×10 + 1×20 = 65 Fury |
| **Verify** | `[GetResources` shows Fury = 65/100 |
| **Failure Indicates** | VystiaResourceManager.OnDamageDealt/OnDamageTaken hooks not firing |

### Fury Decay (Barbarian)
| | |
|---|---|
| **Setup** | `[SetResource Fury 100` |
| **Action** | Exit combat, wait 3 seconds (delay), then 10 more seconds |
| **Expected Decay** | 5 per second after 3s delay = 50 Fury lost |
| **Expected Final** | 100 - 50 = 50 Fury |
| **Failure Indicates** | Combat timeout (10s) or decay timer not running |

### Zeal Generation (Templar)
| | |
|---|---|
| **Setup** | `[SetClassV2 Templar`, `[ResetResources` |
| **Expected Resource** | Zeal (max 10) |
| **Expected Generation** | +1 normal hit, +2 crit, +3 kill |
| **Expected Decay** | 1 per second OOC (starts after 5s delay) |
| **Failure Indicates** | Zeal-specific generation hooks broken |

### Focus Behavior (Ranger)
| | |
|---|---|
| **Setup** | `[SetClassV2 Ranger`, `[ResetResources` |
| **Expected Resource** | Focus (max 100, +10 if Crystalline Ascendancy synergy) |
| **Stationary Regen** | +10 per second while not moving |
| **Moving Decay** | -5 per second while moving |
| **Test** | Stand still 5 seconds (+50), walk 5 seconds (-25), final = 25 |
| **Failure Indicates** | Movement-based regen/decay broken |

### Per-Target Resources (Ice Mage)
| | |
|---|---|
| **Setup** | `[SetClassV2 IceMage` |
| **Action** | Cast Frostbolt on Enemy A 4 times, switch to Enemy B, cast 2 times |
| **Expected** | Chill stacks: A=4, B=2 (tracked separately) |
| **Action** | Return to A, verify stacks still = 4 |
| **Action** | Wait 30+ seconds, check A stacks = 0 (expired) |
| **At 5 stacks** | Target becomes "Frozen" = 50% bonus cold damage + root |
| **Failure Indicates** | TargetTracker per-target state broken |

---

# PHASE 2: BUFF/DEBUFF SYSTEM (25+ Types)

## TEST 2.1: Stat Buff Application

### StrengthBuff
| | |
|---|---|
| **Command** | `[ApplyBuff StrengthBuff 60 20` (60s duration, +20 STR) |
| **Expected** | STR increases by 20 via StatMod |
| **Verify** | Paperdoll shows increased STR, `[ListBuffs` shows active |
| **After Duration** | STR returns to base, buff removed from list |
| **Failure Indicates** | VystiaBuffSystem.OnApply() not applying StatMod |

### Stack Behavior: Refresh
| | |
|---|---|
| **Setup** | Apply StrengthBuff (+10, Refresh behavior) |
| **Action** | Reapply same buff within duration |
| **Expected** | Duration resets, still +10 (NOT +20) |
| **Failure Indicates** | StackBehavior.Refresh not working |

### Stack Behavior: Stack
| | |
|---|---|
| **Setup** | Apply stackable buff (MaxStacks=5) |
| **Action** | Apply 5 times |
| **Expected** | 5 stacks active, 5× effect applied |
| **Action** | Apply 6th time |
| **Expected** | Still 5 stacks (capped), duration refreshed |
| **Failure Indicates** | StackBehavior.Stack or MaxStacks broken |

## TEST 2.2: Damage Over Time

### Bleed DoT
| | |
|---|---|
| **Command** | Apply Bleed (30 damage, 10s duration, 2s tick) |
| **Expected Ticks** | 5 ticks × 6 damage = 30 total |
| **Timing** | Tick at 2s, 4s, 6s, 8s, 10s |
| **Verify** | Damage numbers appear at correct intervals |
| **Failure Indicates** | TargetTracker DoT processing broken |

### Multiple DoT Stacking
| | |
|---|---|
| **Action** | Apply Bleed + Burn + Poison simultaneously |
| **Expected** | All 3 DoTs tick independently |
| **Verify** | 3 damage numbers per tick interval |
| **Failure Indicates** | DoT stacking logic broken |

## TEST 2.3: Heal Over Time

### Rejuvenation HoT
| | |
|---|---|
| **Setup** | Damage self to 50% HP |
| **Command** | Apply Rejuvenation (50 heal, 10s, 2s tick) |
| **Expected** | +10 HP every 2 seconds for 10 seconds |
| **Verify** | HP increases in steps, NOT instant full heal |
| **Failure Indicates** | HoT processing broken |

## TEST 2.4: Shield Buffs

### DamageAbsorb Shield
| | |
|---|---|
| **Command** | Apply DamageAbsorb (absorb 50 damage) |
| **Action** | Take 30 damage hit |
| **Expected** | 0 damage taken, shield reduced to 20 remaining |
| **Action** | Take 40 damage hit |
| **Expected** | 20 absorbed, 20 damage taken, shield removed |
| **Failure Indicates** | AbsorbRemaining calculation broken |

---

# PHASE 3: CROWD CONTROL & DIMINISHING RETURNS

## TEST 3.1: CC Application

### Stun
| | |
|---|---|
| **Command** | `[ApplyCC Stun 5` (5 second stun) |
| **Expected** | Target cannot move, attack, or cast for 5 seconds |
| **Verify** | Aggressive dummy stands still, no attacks during stun |
| **After 5s** | Target immediately resumes actions |
| **Failure Indicates** | CC effect not freezing AI |

### Sleep (Breaks on Damage)
| | |
|---|---|
| **Command** | `[ApplyCC Sleep 10` (10 second sleep) |
| **Action** | Deal ANY damage to target |
| **Expected** | Sleep breaks immediately |
| **Failure Indicates** | Sleep damage-break check missing |

### Fear (Flee Behavior)
| | |
|---|---|
| **Command** | `[ApplyCC Fear 5` |
| **Expected** | Target runs away via BeginFlee() |
| **Verify** | Target moves directly away from caster |
| **Failure Indicates** | Fear flee AI broken |

## TEST 3.2: Diminishing Returns

### DR Calculation
| | |
|---|---|
| **DR Level 0 (1st CC)** | 100% duration (5.0s from 5s) |
| **DR Level 1 (2nd CC)** | 50% duration (2.5s from 5s) |
| **DR Level 2 (3rd CC)** | 25% duration (1.25s from 5s) |
| **DR Level 3 (4th+ CC)** | IMMUNE for 15 seconds |
| **After 15s no CC** | DR resets to Level 0 |

### DR Test Sequence
| | |
|---|---|
| **Action 1** | `[ApplyCC Stun 5` → verify 5.0s duration |
| **Action 2** | Wait for stun to end, immediately `[ApplyCC Stun 5` |
| **Expected** | 2.5s duration (50%) |
| **Action 3** | Immediately `[ApplyCC Stun 5` again |
| **Expected** | 1.25s duration (25%) |
| **Action 4** | Immediately `[ApplyCC Stun 5` again |
| **Expected** | Message "Target is immune", no stun applied |
| **Wait 15s** | `[ApplyCC Stun 5` → 5.0s duration (DR reset) |
| **Failure Indicates** | CrowdControlSystem DR tracking broken |

### DR Check Command
| | |
|---|---|
| **Command** | `[CheckDR Stun` |
| **Expected** | Shows current DR level and time until reset |

### DR Categories
| Category | CC Types | Shared DR |
|----------|----------|-----------|
| **Stun** | Stun, Knockdown | Yes |
| **Incapacitate** | Fear, Sleep, Charm, Confuse | Yes |
| **Root** | Root, Freeze | Yes |
| **Silence** | Silence, Pacify, Disarm | Yes |
| **None (No DR)** | Slow, Blind, Knockback, Polymorph | No DR |

---

# PHASE 4: STANCE SYSTEM (28 Stances)

## TEST 4.1: Stance Activation

### Fighter Stances
| | |
|---|---|
| **Command** | `[SetStance Aggressive` |
| **Expected Bonuses** | +damage, -defense |
| **Verify** | `[StanceInfo` shows bonuses |
| **Action** | `[SetStance Defensive` |
| **Expected** | Previous stance removed, new stance active |
| **Verify** | Only Defensive bonuses applied |
| **Failure Indicates** | Stance exclusive groups not working |

### Druid Forms (Transform)
| | |
|---|---|
| **Command** | `[SetStance Bear` |
| **Expected** | Body transforms, BodyValue changes |
| **Expected Bonuses** | +HP, +armor, melee focus |
| **Transform Time** | 0.5 seconds (interruptible) |
| **Action** | Take damage during transform |
| **Expected** | Transform interrupted if damage exceeds threshold |
| **Failure Indicates** | TransformBody flag or interrupt check broken |

### Stance Properties
| Property | Value |
|----------|-------|
| **Cooldown Minimum** | 1.5 seconds |
| **Transform Time** | 0.5 seconds |
| **Duration** | 0 = permanent (until switched) |

## TEST 4.2: Stance Combat Effects

### Berserker Stance Damage
| | |
|---|---|
| **Setup** | Note base weapon damage (e.g., 50-60) |
| **Action** | `[SetStance Berserker` (+20% damage, -10% defense) |
| **Action** | Attack target multiple times |
| **Expected** | Damage ~20% higher (60-72) |
| **Failure Indicates** | DamageModifier not integrated into damage formula |

---

# PHASE 5: MAGIC SYSTEM (12 Schools, 384 Spells)

## TEST 5.1: Spell Damage Scaling

| | |
|---|---|
| **Setup** | `[setskill IceMagic 50`, `[sb ice` |
| **Action** | Cast Frost Bolt, note damage (e.g., 20-25) |
| **Action** | `[setskill IceMagic 100` |
| **Action** | Cast Frost Bolt again |
| **Expected** | Damage ~50-100% higher (30-50) |
| **Failure Indicates** | Spell damage formula not using skill level |

## TEST 5.2: Reagent Consumption

| | |
|---|---|
| **Setup** | Get Ice spellbook `[sb ice`, remove ALL reagents |
| **Action** | Try to cast Frost Bolt |
| **Expected** | "You lack the reagents" - cast fails |
| **Action** | Add FrozenTear + IceDust (Vystia reagents) |
| **Action** | Cast Frost Bolt |
| **Expected** | Cast succeeds, reagents consumed |
| **Test** | Add BlackPearl (UO reagent), remove Vystia reagents |
| **Expected** | Cast fails - UO reagents do NOT work |
| **Failure Indicates** | Spell using wrong reagent definitions |

## TEST 5.3: Mana Cost by Circle

| Circle | Expected Mana | Example Spell |
|--------|---------------|---------------|
| 1 | 4-8 mana | Frost Bolt |
| 2 | 6-11 mana | Ice Armor |
| 3 | 9-14 mana | Cone of Cold |
| 4 | 11-18 mana | Ice Wall |
| 5 | 14-24 mana | Frost Nova |
| 6 | 20-40 mana | Blizzard |
| 7 | 40-60 mana | Glacial Spike |
| 8 | 50-80 mana | Absolute Zero |

**Verify:** Cast Circle 1 and Circle 8, compare mana consumed.

## TEST 5.4: AoE Target Count

| | |
|---|---|
| **Setup** | Spawn 5 dummies in 3-tile radius |
| **Action** | Cast Blizzard (Circle 6 AoE) centered on group |
| **Expected** | All 5 take damage (damage numbers over each) |
| **Failure Indicates** | AoE targeting not hitting all valid targets |

## TEST 5.5: Spell Effects Apply

| Effect | Test Spell | Verification |
|--------|------------|--------------|
| **Slow** | Frost Bolt | Target move speed reduced |
| **Root** | Freeze | Target cannot move, can attack |
| **Silence** | Silencing hex | Target cannot cast spells |
| **DoT (Cold)** | Frostbite | Damage ticks every 2-3s |

---

# PHASE 6: CREATURE & BOSS MECHANICS

## TEST 6.1: Elemental Resistances

### Ice Golem (High Cold, Low Fire)
| | |
|---|---|
| **Setup** | Spawn Ice Golem, note resists (80% cold, 0% fire) |
| **Action** | Cast Ice spell (cold damage) |
| **Expected** | ~80% reduced damage |
| **Action** | Cast Fire spell (fire damage) |
| **Expected** | Full damage or bonus |
| **If Equal** | Resistances not being checked in damage calc |

## TEST 6.2: Regional Loot Drops

| Region | Expected Drops |
|--------|----------------|
| **Frosthold** | FrozenOre, GlacialShard, EternalIce, FrostwillowLog |
| **Emberlands** | MoltenOre, EmberShard, EverburningCoal, FlamewoodLog |
| **Verdantpeak** | LivingOre, TreantHeart, LivingBark |
| **Crystal Barrens** | CrystalOre, PrismaticShard, CrystalPollen |
| **Ironclad** | SteamworkOre, ClockworkGear, ClockworkSpring, SteamCore |
| **ShadowVoid** | ObsidianOre, VoidEssence, ShadowCloth |

**Test:** Kill 5 creatures per region, verify loot tables include regional materials.

## TEST 6.3: Boss Special Abilities

### FrostFather (Frosthold Boss)
| | |
|---|---|
| **Expected HP** | ~5000+ |
| **Expected Abilities** | Blizzard AoE, Freeze targets, Ice Armor buff |
| **Verify** | Boss uses abilities during combat, not just melee |
| **Failure Indicates** | Boss AI or ability triggers not implemented |

### All 10 Regional Bosses
| Boss | Region | Unique Ability |
|------|--------|----------------|
| FrostFather | Frosthold | Blizzard, Freeze |
| VolcanoWyrm | Emberlands | Lava Breath, Fire AoE |
| SphinxOfSurya | Desert | Riddle Debuff, Sandstorm |
| CovenMatriarch | Shadowfen | Hex Curse, Summon Witches |
| AncientTreant | Verdantpeak | Root, Nature's Wrath |
| CrystalDrakeAlpha | Crystal Barrens | Prismatic Breath, Reflect |
| ForgeMaster | Ironclad | Steam Blast, Summon Constructs |
| GriffinLord | Skyreach | Dive Attack, Wind Gust |
| AncientKraken | Underwater | Tentacle Grab, Ink Cloud |
| TimewornLich | ShadowVoid | Soul Drain, Summon Undead |

---

# PHASE 7: FACTION SYSTEM (7 Factions)

## TEST 7.1: Reputation Tiers

| Tier | Rep Range | Vendor Discount |
|------|-----------|-----------------|
| **Hostile** | -3000 to -1001 | 0% |
| **Unfriendly** | -1000 to -1 | 0% |
| **Neutral** | 0 to 2999 | 0% |
| **Friendly** | 3000 to 5999 | 5% |
| **Honored** | 6000 to 11999 | 8% |
| **Revered** | 12000 to 14999 | 12% |
| **Exalted** | 15000+ | 15% |

### Tier Progression Test
| | |
|---|---|
| **Action** | `[SetReputation Frostguard 0` |
| **Verify** | `[Factions` shows Neutral |
| **Action** | `[SetReputation Frostguard 3000` |
| **Verify** | Shows Friendly |
| **Action** | `[SetReputation Frostguard 15000` |
| **Verify** | Shows Exalted |

## TEST 7.2: Vendor Discount Application

| | |
|---|---|
| **Setup** | Find Frostguard Vendor, note base price (e.g., 1000g) |
| **Action** | `[SetReputation Frostguard 0` (Neutral) |
| **Expected** | Price = 1000g (0% discount) |
| **Action** | `[SetReputation Frostguard 3000` (Friendly) |
| **Expected** | Price = 950g (5% discount) |
| **Action** | `[SetReputation Frostguard 15000` (Exalted) |
| **Expected** | Price = 850g (15% discount) |
| **If Same** | VystiaFactionVendor discount calc broken |

## TEST 7.3: Enemy Faction Penalty

| | |
|---|---|
| **Setup** | Frostguard and Flame Legion are enemies |
| **Action** | `[SetReputation Frostguard 0`, `[SetReputation FlameLegion 0` |
| **Action** | `[AddReputation Frostguard 500` |
| **Expected** | Frostguard = 500, Flame Legion = -250 (50% penalty) |
| **Failure Indicates** | Enemy faction penalty not applying |

## TEST 7.4: Reputation Changes

| Action | Rep Change |
|--------|------------|
| Small Quest | +50 |
| Medium Quest | +150 |
| Large Quest | +350 |
| Epic Quest | +500 |
| World Boss | +250 |
| Donation (per 1000g) | +50 |

---

# PHASE 8: RELIGION SYSTEM (6 Religions)

## TEST 8.1: Piety Tiers

| Tier | Piety Range | Unlocks |
|------|-------------|---------|
| **None** | 0-49 | Nothing |
| **Initiate** | 50-199 | First passive bonus |
| **Devoted** | 200-499 | Devotion Power 1 |
| **Faithful** | 500-899 | Devotion Power 2 |
| **Exalted** | 900-1000 | Devotion Power 3 |

### Tier Progression Test
| | |
|---|---|
| **Action** | `[SetReligion FrostfatherCult`, `[SetPiety 0` |
| **Verify** | `[Religion` shows Tier: None |
| **Action** | `[SetPiety 200` |
| **Verify** | Shows Devoted, Power 1 unlocked |
| **Action** | `[SetPiety 900` |
| **Verify** | Shows Exalted, all 3 powers unlocked |

## TEST 8.2: Prayer System

| | |
|---|---|
| **Command** | `[Pray` |
| **Expected** | +10 piety, animation/sound |
| **Cooldown** | Once per 24 hours |
| **Verify** | Second prayer same day shows error |

## TEST 8.3: Tithe System

| | |
|---|---|
| **Command** | `[Tithe 1000` |
| **Expected** | +10 piety, -1000g |
| **Rate** | +1 piety per 100g |
| **Daily Cap** | 30 piety (3000g max) |
| **Minimum** | 100g per tithe |

## TEST 8.4: Devotion Powers

### Frostfather Cult Powers
| Tier | Power | Effect |
|------|-------|--------|
| Devoted (200) | Frost Shield | Absorb 50 damage, reflect 15% cold, 15 min |
| Faithful (500) | Endurance of Winter | Cannot die 5s (HP min 1), -50% damage, 30 min CD |
| Exalted (900) | Absolute Zero | Freeze enemies 5 tiles, 3s duration, 60 min CD |

### Power Activation Test
| | |
|---|---|
| **Setup** | `[SetPiety 200` (Devoted tier) |
| **Action** | Activate Frost Shield at shrine |
| **Expected** | Buff applied, 15 min duration |
| **Action** | Take 30 damage |
| **Expected** | 0 damage taken, shield reduced to 20 |

---

# PHASE 9: LLM NPC QUEST SYSTEM

## TEST 9.1: Auto-Greet Trigger

| Property | Value |
|----------|-------|
| **Proximity Check** | Every 2 seconds |
| **Auto-Speak Range** | 10 tiles |
| **Hearing Range** | 6 tiles (for OnSpeech) |

| | |
|---|---|
| **Setup** | `[aqn` to spawn QuestNPC at Origin waypoint |
| **Action** | Walk to within 10 tiles of NPC |
| **Expected** | NPC auto-greets within 2-3 seconds |
| **Greeting** | "Greetings, [Player]. I have a task..." |
| **Failure Indicates** | AutoSpeakTimer not triggering |

## TEST 9.2: Quest Keywords

| Keyword | Action |
|---------|--------|
| "accept", "yes", "begin" | Start quest (if at Origin) |
| "complete", "done", "finished" | Turn in quest (if at NPCCompletion) |
| "quest", "task", "job" | Show current status |
| "hello", "hail", "greetings" | Complete TalkToNPC waypoint |

| | |
|---|---|
| **Action** | Say "quest" to NPC |
| **Expected** | NPC shows quest offer or current objective |
| **Action** | Say "accept" |
| **Expected** | Quest added to log, Origin waypoint completes |

## TEST 9.3: Location Hints

### Distance Tiers
| Distance | Description |
|----------|-------------|
| <= 60 tiles | "a short walk" |
| <= 200 tiles | "a fair walk" |
| <= 500 tiles | "a moderate journey" |
| > 500 tiles | "a long journey" |

| | |
|---|---|
| **Setup** | NPC at (100, 100), waypoint at (150, 140) |
| **Ask NPC** | "Where do I go?" |
| **Expected Format** | "Head [southeast] of here, a short walk (~50 tiles). Coordinates: (150, 140)." |
| **Required** | Concrete direction, distance, coordinates |
| **Prohibited** | "I can't provide directions", "Trust your instincts" |

## TEST 9.4: Waypoint Types & Conditions

| Type | Condition | Trigger |
|------|-----------|---------|
| Origin | TalkToNPC | Accept quest keyword |
| Waypoint | ReachLocation | Enter radius (default 5 tiles) |
| Waypoint | TalkToNPC | Greet NPC |
| Waypoint | DefeatBoss | Kill specific creature type |
| Waypoint | CollectItems | Acquire required items |
| NPCCompletion | TalkToNPC | Complete quest keyword |

### Location Waypoint Auto-Complete
| | |
|---|---|
| **Setup** | Quest with ReachLocation waypoint (radius 5) |
| **Action** | Walk to within 5 tiles of location |
| **Expected** | "Location reached: [Name]" - auto-completes |
| **Check Interval** | Every 2 seconds |
| **Failure Indicates** | QuestWaypointDetector movement hook broken |

### Boss Kill Auto-Complete
| | |
|---|---|
| **Setup** | Quest with DefeatBoss waypoint, TargetTypeName = "Lich" |
| **Action** | Kill a Lich while quest active |
| **Expected** | "Creature defeated: Lich. Waypoint complete." |
| **If Final Waypoint** | Quest auto-completes |
| **Failure Indicates** | OnKilledBy hook not checking quest |

## TEST 9.5: Quest Chain Continuity

| | |
|---|---|
| **Setup** | 3-NPC quest chain: Giver → Scout → Captain |
| **At Scout** | NPC says "I heard from [Giver] you'd be coming..." |
| **At Captain** | NPC says "The Scout sent word ahead..." |
| **Failure Indicates** | Previous NPC context not in BuildQuestAwareContext() |

---

# PHASE 10: ZONE SYSTEM (4 Types)

## Zone Rules Matrix

| Zone | PvP | Consent | Guards | Death Penalty | Loot Drop | XP Bonus |
|------|-----|---------|--------|---------------|-----------|----------|
| **Sanctuary** | No | N/A | Yes | 0% skill loss | 0% | 0% |
| **Contested** | Yes | Required | Yes | 25% skill loss | 10% | +25% |
| **Lawless** | Yes | None | No | 50% skill loss | 50% | +50% |
| **Extreme** | Yes | None | No | 100% skill loss | 100% | +100% |

## TEST 10.1: Sanctuary PvP Block

| | |
|---|---|
| **Setup** | `[SetZone Sanctuary` |
| **Action** | Player A attacks Player B |
| **Expected** | Attack blocked, "Cannot attack in Sanctuary" |
| **Failure Indicates** | Zone PvP check not in damage system |

## TEST 10.2: Lawless PvP Enable

| | |
|---|---|
| **Setup** | `[SetZone Lawless` |
| **Action** | Player A attacks Player B |
| **Expected** | Full damage dealt, no consent needed |
| **Failure Indicates** | Zone type not enabling PvP |

## TEST 10.3: Death Penalty

| | |
|---|---|
| **Setup** | Note skill value (e.g., Swordsmanship 100.0) |
| **Action** | `[SetZone Extreme`, die |
| **Expected** | Skill loses 100% (drops significantly) |
| **Setup** | `[SetZone Sanctuary`, die |
| **Expected** | Skill unchanged (0% penalty) |

---

# PHASE 11: AI SIDEKICK MECHANICS

## TEST 11.1: Mage Sidekick Kiting

| | |
|---|---|
| **Spawn** | `[SpawnSidekick Mage` |
| **Expected Range** | 6-8 tiles from enemy |
| **Action** | Spawn melee enemy, watch positioning |
| **Expected** | Mage maintains distance, casts spells |
| **If Enemy Closes** | Mage moves away to restore range |
| **Failure Indicates** | Kiting AI logic broken |

## TEST 11.2: Warrior Sidekick Tank

| | |
|---|---|
| **Spawn** | `[SpawnSidekick Warrior` |
| **Expected Range** | 1 tile (melee) |
| **Action** | Spawn enemy, watch engagement |
| **Expected** | Warrior closes to melee, continuous attacks |
| **Expected** | Higher HP than Mage |
| **Failure Indicates** | Melee engagement AI broken |

## TEST 11.3: Self-Heal Trigger

| | |
|---|---|
| **Setup** | Spawn sidekick, damage to 30% HP |
| **Expected** | Sidekick casts heal at ~40% HP threshold |
| **Verify** | HP recovers, heal animation plays |
| **Failure Indicates** | Self-preservation AI not triggering |

## TEST 11.4: Follow Behavior

| | |
|---|---|
| **Setup** | Spawn sidekick |
| **Action** | Walk 20 tiles away |
| **Expected** | Sidekick follows, maintains ~3 tile distance |
| **Pathfinding** | Uses A* around obstacles |
| **In Combat** | Stops following, engages enemies |

---

# PHASE 12: CRAFTING SYSTEM

## TEST 12.1: Regional Recipe Requirements

| Region | Ingot | Example Weapon |
|--------|-------|----------------|
| Frosthold | FrostforgedIngot | Icicle Blade |
| Emberlands | EmberforgedIngot | Flame Tongue |
| Ironclad | ClockworkIngot | Gear Blade |
| ShadowVoid | VoidforgedIngot | Shadow Fang |

| | |
|---|---|
| **Setup** | Open blacksmith crafting |
| **Action** | Select FrostforgedBattleaxe recipe |
| **Expected** | Required: FrostforgedIngot (NOT standard Iron) |
| **Action** | Have only IronIngot, try to craft |
| **Expected** | "You lack the materials" |
| **Failure Indicates** | VystiaCraftingRecipes using wrong resources |

## TEST 12.2: Crafted Item Properties

| | |
|---|---|
| **Action** | Craft FrostforgedBattleaxe |
| **Expected Hue** | 1150 (ice blue) |
| **Expected Damage** | Cold damage bonus |
| **Expected Stats** | Regional bonuses (cold resist, etc.) |
| **Failure Indicates** | Equipment templates not applying regional props |

---

# PHASE 13: ANCIENT BEING ENCOUNTERS

## TEST 13.1: All 12 Ancient Beings

| Ancient | HP | Abilities |
|---------|-----|----------|
| FrosthelmEternalWinter | 10000+ | Multi-phase, ice abilities |
| EmberflameAshenTyrant | 10000+ | Fire AoE, enrage |
| VerdantheartForestGuardian | 10000+ | Root, nature damage |
| CrystalwingPrismaticOracle | 10000+ | Prismatic breath, reflect |
| AbyssusDepthKing | 10000+ | Drowning, tentacles |
| ElderOakbark | 10000+ | Nature's wrath, summons |
| SphynxOfEmberlands | 10000+ | Riddles, sandstorm |
| FrostFathersAvatar | 10000+ | Avatar of boss |
| GreatMachinistsConstruct | 10000+ | Mechanical, steam |
| LunarasDryadHerald | 10000+ | Nature, healing |
| TheCrystalSphinx | 10000+ | Crystal, energy |
| IronbarkWarAncient | 10000+ | Nature, melee |

| | |
|---|---|
| **Command** | `[AncientStones` to get all 12 spawn stones |
| **Action** | Double-click each stone |
| **Expected** | Ancient spawns with raid-tier stats |

## TEST 13.2: Boss LLM Dialogue

| | |
|---|---|
| **Setup** | Create quest with BossCompletion waypoint |
| **Configure** | Boss as QuestNPC with menacing personality |
| **Expected Dialogue** | "You dare challenge me?", "Mortals shall fall!" |
| **Context Includes** | Boss taunt instructions for LLM |

---

# MASTER CHECKLIST

## Class System
- [ ] All 25 classes assign correct stats
- [ ] Secondary resources generate on damage dealt (+5 Fury per hit)
- [ ] Secondary resources generate on damage taken (+10 Fury per hit)
- [ ] Secondary resources generate on kill (+20 Fury per kill)
- [ ] Resources decay out of combat (5/sec after 3s delay)
- [ ] Per-target resources track separately (Chill, Combo)
- [ ] Non-decay resources persist (Chi, Soul Shards)

## Buff/Debuff
- [ ] Stat buffs apply via StatMod
- [ ] Buff duration expires correctly
- [ ] Stack behavior: Refresh resets duration
- [ ] Stack behavior: Stack accumulates
- [ ] DoT ticks every 2-3 seconds
- [ ] HoT heals over time, not instant
- [ ] Shield absorbs damage correctly

## Crowd Control
- [ ] Stun prevents all actions
- [ ] Sleep breaks on damage
- [ ] Fear causes flee behavior
- [ ] DR Level 1 = 50% duration
- [ ] DR Level 2 = 25% duration
- [ ] DR Level 3 = immune 15 seconds
- [ ] DR resets after 15s no CC

## Stance System
- [ ] 28 stances activate correctly
- [ ] Stance bonuses affect combat
- [ ] Transform stances change body
- [ ] Only one stance active at time

## Magic System
- [ ] Spell damage scales with skill
- [ ] Vystia reagents consumed
- [ ] UO reagents do NOT work
- [ ] Mana cost increases by circle
- [ ] AoE hits multiple targets
- [ ] Spell effects apply (slow, root, DoT)

## Factions
- [ ] 7 tiers from Hostile to Exalted
- [ ] Friendly = 5% discount
- [ ] Honored = 8% discount
- [ ] Revered = 12% discount
- [ ] Exalted = 15% discount
- [ ] Enemy faction penalty applies

## Religion
- [ ] 6 religions with 5 piety tiers
- [ ] Prayer grants +10 piety (daily)
- [ ] Tithe: +1 piety per 100g (30 cap)
- [ ] Devotion powers unlock at tiers

## LLM NPCs
- [ ] Auto-greet within 10 tiles
- [ ] Hearing range 6 tiles
- [ ] Location hints include coordinates
- [ ] Quest chain continuity works

## Zones
- [ ] Sanctuary blocks PvP
- [ ] Lawless enables PvP
- [ ] Death penalty varies by zone

## AI Sidekicks
- [ ] Mage kites at 6-8 tiles
- [ ] Warrior engages melee
- [ ] Self-heal at 40% HP
- [ ] Follow behavior works

---

*Last Updated: 2026-01-06*
