# Vystia Mechanics Testing Checklist

*Production testing for all Vystia systems. Values from source code.*

**Enhanced Detail Level:** This checklist now includes specific spell names, ability IDs, expected behaviors, and visual indicators for comprehensive testing. Each section provides:
- Specific spells/abilities to test
- Expected values and durations
- Visual/audio feedback indicators
- Step-by-step test procedures

---

## Phase 1: Environment Setup
- [ ] **Access Test Kit**: Use Vystia Test Kit item (if available) or test through normal gameplay
- [ ] **Test Area**: Travel to Green Acres (5445, 1153) or designated test area
- [ ] **Skill Training**: Train skills through normal gameplay (use skills, complete quests, train with NPCs)
- [ ] **Skill Cap**: Verify skill cap is set correctly (check skill gump or status)

### Test Dummies Setup and Usage

**Test Dummy Types:**

1. **Practice Target (PracticeDummy)**
   - **Purpose**: Invulnerable target for testing spells, abilities, damage, buffs, debuffs, DoTs, HoTs, and effects
   - **Properties**: 
     - 100 million HP (won't die)
     - 0% resistances (shows true damage values)
     - Frozen/immobile (won't move or fight back)
     - Reports all effects via messages
   - **How to Spawn**: 
     - Use `[PracticeTarget` or `[PT` command
     - OR: Use spawn gump (Misc category) → Select "Practice Target"
   - **How to Use**:
     - Spawn near test area
     - Attack with weapons → See damage messages: "[Dummy] DAMAGE: X (HP: X/X)"
     - Cast spells → See effect messages: "[Dummy] HEAL: +X HP", "[Dummy] BUFF: StrengthBoost", etc.
     - Apply DoTs → See tick damage messages
     - Apply HoTs → See heal tick messages
     - Apply CC → See CC effect messages
     - Apply buffs/debuffs → See status messages
   - **Best For**: Testing damage numbers, spell effects, buff/debuff application, DoT/HoT ticks, CC effects

2. **Combat Dummy (VystiaCombatDummy)**
   - **Purpose**: Fighting target that can fight back, test faction/religion interactions, test aggro mechanics
   - **Modes**:
     - **Passive**: Takes damage, doesn't fight back (like Practice Target but can die)
     - **Melee**: Fights back with melee attacks only
     - **Caster**: Casts spells only (uses regional magic schools)
     - **Hybrid**: Both melee and spells (default)
   - **Properties**:
     - 500-750 HP (can be killed)
     - Can be assigned faction and religion
     - Can be assigned class type (uses class skills)
     - Tracks damage taken and hit count
   - **How to Spawn**:
     - Use `[CreateSidekick` or `[Sidekick` command → Opens sidekick creation menu → May include dummy options
     - OR: Find combat dummy spawner item/stone in test area
     - OR: Talk to test area NPC that spawns dummies
   - **How to Use**:
     - Spawn in desired mode (Passive/Melee/Caster/Hybrid)
     - Assign faction/religion if testing faction/religion mechanics
     - Attack dummy → Test damage, resource generation, ability effects
     - Let dummy attack you → Test defensive abilities, damage taken, resource generation from taking damage
     - Test faction interactions → Attack enemy faction dummy → Verify aggro/reputation changes
     - Test religion interactions → Attack opposing religion dummy → Verify religion mechanics
   - **Best For**: Testing combat mechanics, resource generation, faction/religion systems, defensive abilities, taking damage

**Test Dummy Workflow:**
1. **Setup Phase**: Spawn appropriate dummy type for your test
   - Use Practice Target for damage/effect testing
   - Use Combat Dummy for combat/resource testing
2. **Positioning**: Place dummy in accessible location (near crafting tables if testing crafting-related combat)
3. **Testing**: 
   - Practice Target: Attack/cast → Read damage/effect messages → Verify values match expected
   - Combat Dummy: Engage in combat → Test abilities → Watch resource generation → Test defensive mechanics
4. **Cleanup**: Dummies persist until killed (Combat Dummy) or manually removed (Practice Target)

## Phase 2: Class System (25 Classes)

### Class Assignment
**Player Method:**
- [ ] Talk to class trainer NPC → Select class from trainer menu
- [ ] OR: Choose class during character creation
- [ ] Character receives class assignment → Stats set to class defaults
- [ ] Check stats → Verify STR/DEX/INT match class design
- [ ] Check resource → Secondary resource available (e.g., Fury 0/100 for Barbarian)
- [ ] Receive starting equipment: Weapon, armor, class focus item, spellbook (if magic class)
- [ ] Receive starting skills: Primary skills set to class starting values

### Resource Generation (Melee Combat)

#### Fury (Barbarian/Knight) - Max 100
**Generation:**
- [ ] +5 Fury per hit dealt (melee attack)
- [ ] +10 Fury per hit taken (damage received)
- [ ] +20 Fury per kill
- [ ] Visual: Resource bar shows current/max (e.g., "45/100 Fury")

**Threshold Testing:**
- [ ] 0-25 Fury: No visual/audio feedback
- [ ] 25-50 Fury: Resource bar visible, no special effects
- [ ] 50-75 Fury: Character shows slight aggression (optional visual)
- [ ] 75-99 Fury: Resource bar near full, anticipation builds
- [ ] 100 Fury: "RAGE READY" message, can activate Rage transformation
- [ ] Rage activation: Character transforms, +STR bonus, drains 5 Fury/sec

**Test Procedure:**
1. **Setup:** Select Barbarian class from trainer or character creation
2. **Combat Test:** Attack training dummy or enemy creature
3. **Generate Fury:** Attack enemy 20 times with melee weapon → Should see ~100 Fury (20 × 5)
   - Watch resource bar: Fury increases with each hit
   - Message may appear: "Fury: X/100" or resource bar updates
4. **Take Damage:** Let enemy attack you → Should see +10 Fury per hit taken (watch resource bar)
5. **Kill Bonus:** Kill enemy → Should see +20 Fury bonus (watch resource bar jump)
6. **Decay Test:** Wait 3 seconds out of combat → Fury should decay at 5/sec
   - Watch resource bar decrease
   - Decay stops when entering combat again (attack or get attacked)

#### Chi (Monk) - Max 5
- [ ] +3 Chi per hit dealt
- [ ] +15 Chi on combo finisher ability
- [ ] Visual: Shows "Chi: 3/5" or similar
- [ ] At 5 Chi: Ultimate ability available

#### Combo Points (Fighter/Rogue) - Max 5 per target
- [ ] +1 Combo Point per hit (per-target tracked)
- [ ] Max 5 stacks per target
- [ ] Expires after 30 seconds of no hits
- [ ] At 5 stacks: Finisher abilities available

#### Energy (Artificer) - Max 100
- [ ] +10 Energy per kill
- [ ] Passive regen: +5/sec (always active)
- [ ] Used for construct abilities

#### Focus (Ranger/BountyHunter) - Max 100
- [ ] +2 Focus per hit
- [ ] +25 Focus on critical hit
- [ ] Stationary: +10/sec regen
- [ ] Moving: -5/sec decay

### Resource Thresholds & Activation Effects

**Important: Activation Methods by Class Type**

- **Magic Classes** (Ice Mage, Sorcerer, Warlock, Druid, Witch, Oracle, Summoner, Shaman, Bard (Songweaving), Enchanter, Illusionist):
  - **Use Spellbooks**: Open spellbook → Double-click spell to cast
  - **Get Spellbook**: Receive spellbook when selecting class, or obtain from trainer/vendor
  - **Spell IDs**: 1000-1383 (32 spells per school)
  - **Spells in Spellbook**: Organized by 8 circles (4 spells per circle)
  - **Casting**: Double-click spell in spellbook → Select target (if required) → Spell casts

- **Martial Classes** (Barbarian, Fighter, Monk, Rogue, Ranger, Knight, Paladin, Templar, Bounty Hunter, Beastmaster, Artificer, Alchemist, Cleric, Wizard):
  - **Use Ability System**: Access abilities via ability list/gump/hotkeys (NO spellbook)
  - **Ability IDs**: 2000-2223 (16 abilities per class)
  - **Ability Access**: Abilities available through ability system UI (varies by client - hotkeys, gump, or class-specific interface)
  - **Activation**: Select ability from list/gump or use hotkey → Ability executes or prompts for target

---

#### Fury (Barbarian/Knight) - Threshold: 100
**At 100 Fury:**
- [ ] **Rage Activation Available**: Message "RAGE READY" appears
- [ ] **Activate Rage**: 
  - **Player Method**: Use Rage ability from ability system (hotkey/gump) or RageTotem item
  - **Activation Method**: Ability is self-targeted, instant cast
  - **Ability Access**: Open ability list/gump → Find "Berserker Rage" or "Rage" ability → Activate
- [ ] **Rage Effects** (while active):
  - [ ] +30 STR bonus
  - [ ] +25% damage dealt
  - [ ] +25% attack speed
  - [ ] Immune to crowd control (CC)
  - [ ] Cannot use non-Rage abilities (only Rage abilities available)
  - [ ] Character visual transformation (red aura, aggressive stance)
- [ ] **Rage Duration**: Drains 5 Fury/sec until depleted
- [ ] **Rage Deactivation**: When Fury reaches 0, Rage ends automatically
- [ ] **Message**: "RAGE ACTIVATED!" on activation, "Rage ends" on deactivation

**Test Procedure:**
1. Build Fury to 100 through combat (attack enemies, take damage, get kills)
2. **Activate Rage**: 
   - Open ability list/gump → Find "Berserker Rage" or "Rage" ability
   - Activate ability (click/hotkey) → Rage transformation begins
   - OR: Use RageTotem item if available
3. Verify transformation and stat bonuses (+30 STR, +25% damage, +25% attack speed)
4. Use Rage abilities → Verify only Rage abilities available (normal abilities disabled)
5. Watch Fury drain → Verify 5/sec drain rate (watch resource bar decrease)
6. When Fury reaches 0 → Verify Rage deactivates and normal abilities return

#### Chi (Monk) - Threshold: 5 (Max)
**At 5 Chi (Maximum):**
- [ ] **Finisher Abilities Unlocked**: High-damage finisher abilities become available
- [ ] **Finisher Damage**: Scales with Chi amount (5 Chi = maximum damage)
- [ ] **Available Finishers**:
  - [ ] **Quivering Palm** (ID ~2038): Delayed massive damage (50-80 base, scales with Chi)
  - [ ] **Touch of Death** (ID ~2040): Execute ability (100-150 damage if target <10% HP)
  - [ ] **Rising Sun Kick** (ID ~2033): Powerful kick with knockup (25-40 base, +Chi multiplier)
- [ ] **Chi Consumption**: Finishers consume all Chi (5 → 0)
- [ ] **Visual**: Resource bar shows "Chi: 5/5" with special indicator

**Activation Instructions:**
- **Monk Class**: Access abilities via ability system (hotkeys, ability gump, or class-specific UI)
- **No Spellbook**: Martial classes do NOT have spellbooks - abilities are accessed differently
- **Ability Access**: Open ability list/gump → Check for available finishers when Chi = 5
- **Finisher Names**: Look for "Quivering Palm", "Touch of Death", "Rising Sun Kick" in ability list

**Test Procedure:**
1. Build Chi to 5 through combat (use Jab ability, basic attacks)
2. **Verify finisher availability**: Open ability list/gump → Finishers should be enabled/visible
3. **Activate Finisher**: 
   - Select finisher ability from ability list (e.g., "Quivering Palm" or "Touch of Death")
   - Activate ability → Select target (if required)
4. Cast finisher → Verify all Chi consumed (5 → 0, watch resource bar)
5. Verify finisher damage scales with Chi amount (test with 1 Chi vs 5 Chi - rebuild and compare damage)
6. Rebuild Chi and test different finishers

#### Combo Points (Fighter/Rogue) - Threshold: 5 (Max per target)
**At 5 Combo Points (Maximum):**
- [ ] **Finisher Abilities Unlocked**: High-damage finisher abilities become available
- [ ] **Finisher Damage**: Scales with Combo Points (5 = maximum damage)
- [ ] **Available Finishers**:
  - [ ] **Eviscerate** (Rogue, ID 20002): Massive damage (base + combo multiplier)
  - [ ] **Execute** (Fighter/Barbarian, ID 20004): High damage, bonus if target <30% HP
  - [ ] **Finishing Move** (varies by class): Class-specific finisher
- [ ] **Combo Consumption**: Finishers consume all Combo Points (5 → 0)
- [ ] **Per-Target Tracking**: Combo Points tracked separately per target
- [ ] **Expiration**: Combo Points expire after 30 seconds of no hits on that target

**Activation Instructions:**
- **Martial Classes**: Access finisher abilities via ability system (no spellbook)
- **Ability Access**: Open ability list/gump → Finishers appear when Combo Points = 5 on target
- **Finisher Names**: Look for "Eviscerate" (Rogue), "Execute" (Fighter/Barbarian), or class-specific finisher names
- **Target Required**: Finishers require the target with Combo Points (must select same target you built Combo Points on)

**Test Procedure:**
1. Attack target 5 times → Build 5 Combo Points (watch resource bar show "Combo: 5/5")
2. **Verify finisher availability**: Open ability list/gump → Finishers should be enabled/visible
3. **Activate Finisher**: 
   - Select finisher ability from ability list (e.g., "Eviscerate" or "Execute")
   - Target the same enemy you built Combo Points on → Activate ability
4. Cast finisher → Verify all Combo Points consumed (5 → 0, watch resource bar reset)
5. Attack different target → Verify separate Combo Point tracking (new target starts at 0, old target's points remain)
6. Wait 30 seconds on first target → Verify Combo Points expire (watch timer or resource bar)

#### Fortitude (Knight) - Threshold: 10 (Max)
**At 10 Fortitude (Maximum):**
- [ ] **Ultimate Abilities Unlocked**: High-cost defensive abilities become available
- [ ] **Available Abilities** (Knight ability IDs ~2080-2095):
  - [ ] **Last Stand**: Cannot die for 8s, costs 5 Fortitude
  - [ ] **Unbreakable**: Cannot drop below 1 HP for 6s, costs 5 Fortitude
  - [ ] **Concussive Blow**: Stun 4s, costs 2 Fortitude
  - [ ] **Iron Skin**: -30% all damage taken 12s, costs 3 Fortitude
- [ ] **Fortitude Generation**: +1 per block, +bonus on big hits taken
- [ ] **Fortitude Decay**: -1/sec out of combat

**Activation Instructions:**
- **Knight Class**: Access abilities via ability system (no spellbook for martial classes)
- **Ability Access**: Open ability list/gump → Ultimate abilities appear when Fortitude = 10
- **Ability Names**: Look for "Last Stand", "Unbreakable", "Concussive Blow", "Iron Skin" in ability list
- **Self-Targeted**: Most Fortitude abilities are self-targeted (defensive) - activate on self

**Test Procedure:**
1. Block attacks to build Fortitude to 10 (equip shield, block incoming attacks from enemy)
2. **Verify ability availability**: Open ability list/gump → Ultimate abilities should be enabled/visible
3. **Activate Last Stand**: 
   - Select "Last Stand" ability from ability list
   - Activate on self (self-targeted ability)
4. Verify cannot die for 8s, costs 5 Fortitude (Fortitude: 10 → 5, watch resource bar)
5. **Activate Concussive Blow**: 
   - Select "Concussive Blow" ability from ability list
   - Target enemy → Activate ability
   - Verify stun works, costs 2 Fortitude (Fortitude: 5 → 3)
6. Wait out of combat → Verify Fortitude decays at 1/sec (watch resource bar decrease)

#### Zeal (Templar) - Threshold: 10 (Max)
**At 10 Zeal (Maximum):**
- [ ] **Judgment Abilities Unlocked**: High-damage judgment abilities available
- [ ] **Available Abilities** (Templar ability IDs ~2112-2127):
  - [ ] **Judgment**: Massive damage, scales with Zeal amount
  - [ ] **Divine Wrath**: AoE damage, costs 5 Zeal
  - [ ] **Redemption**: Self-buff, costs 3 Zeal
- [ ] **Zeal Generation**: +1 per hit, +2 per crit, +3 per kill
- [ ] **Zeal Decay**: -1/sec out of combat

**Activation Instructions:**
- **Templar Class**: Access abilities via ability system (no spellbook)
- **Ability Access**: Open ability list/gump → Judgment abilities appear when Zeal = 10
- **Ability Names**: Look for "Judgment", "Divine Wrath", "Redemption" in ability list

**Test Procedure:**
1. Build Zeal to 10 through combat (+1 per hit, +2 per crit, +3 per kill)
2. **Verify ability availability**: Open ability list/gump → Judgment abilities should be enabled/visible
3. **Activate Judgment**: 
   - Select "Judgment" ability from ability list
   - Target enemy → Activate ability
4. Cast Judgment → Verify high damage, Zeal consumed (10 → 0 or partial based on cost, watch resource bar)
5. Test different Zeal costs → Rebuild Zeal and test "Divine Wrath" (costs 5) vs "Redemption" (costs 3) → Verify abilities scale with Zeal amount

#### Chill Stacks (Ice Mage) - Threshold: 5 (Max per target)
**At 5 Chill Stacks (Maximum):**
- [ ] **Auto-Freeze Triggered**: Target automatically becomes Frozen
- [ ] **Frozen Effect**: Target rooted, cannot move (but can still cast/attack)
- [ ] **Visual**: Ice particles, frozen appearance
- [ ] **Duration**: Frozen lasts until Chill stacks expire or are removed
- [ ] **Stack Generation**: Each ice spell adds 1 Chill stack per target
- [ ] **Stack Expiration**: Chill stacks expire after duration (varies by spell)

**Activation Instructions:**
- **Ice Mage Class**: Uses **IceMageSpellbook** (spellbook system)
- **Spell Access**: Open spellbook → Double-click ice spells to cast
- **Spell IDs**: Ice Bolt (ID 1008), Frostbite (ID 1009), etc. (IDs 1000-1031)
- **No Manual Activation**: Freeze triggers automatically at 5 Chill stacks (no ability to activate)

**Test Procedure:**
1. **Get Spellbook**: Receive IceMageSpellbook when selecting Ice Mage class, or obtain from trainer/vendor
2. **Open Spellbook**: Double-click IceMageSpellbook item → Spellbook opens showing 8 circles
3. **Cast Ice Bolt**: Double-click Ice Bolt (Circle 3) → Target enemy → Spell casts
4. Repeat 5 times on same target → Build 5 Chill stacks (watch stack counter or target's status)
5. **Verify Auto-Freeze**: At 5 stacks, target automatically becomes Frozen (no manual activation needed)
6. Verify target cannot move but can still cast spells
7. Verify visual effects (ice particles, frozen appearance)
8. Wait for stacks to expire → Verify Frozen effect ends

#### Soul Shards (Warlock) - Threshold: 3 (Max)
**At 3 Soul Shards (Maximum):**
- [ ] **Ultimate Abilities Unlocked**: High-power dark magic abilities available
- [ ] **Available Abilities** (Dark Magic spells, IDs 1128-1159):
  - [ ] **Soul Shatter** (Circle 5, ID ~1133): Massive damage + mana burn, costs 1 Shard
  - [ ] **Chaos Storm** (Circle 6, ID ~1150): Random element AoE, costs 1 Shard
  - [ ] **Apocalyptic Chaos** (Circle 7, ID ~1155): AoE stun + debuff, costs 2 Shards
- [ ] **Shard Generation**: 25% chance on kill or crit
- [ ] **Shard Persistence**: Soul Shards do not decay (persist until used)

**Activation Instructions:**
- **Warlock Class**: Uses **WarlockSpellbook** (spellbook system)
- **Get Spellbook**: Receive WarlockSpellbook when selecting Warlock class, or obtain from trainer/vendor
- **Spell Access**: Open spellbook → Double-click dark magic spells to cast
- **Spell IDs**: Dark Magic spells (IDs 1128-1159)
- **Shard-Cost Spells**: Ultimate spells require Soul Shards and are only available when you have enough Shards

**Test Procedure:**
1. **Get Spellbook**: Receive WarlockSpellbook when selecting Warlock class, or obtain from trainer/vendor
2. **Open Spellbook**: Double-click WarlockSpellbook → View 8 circles of spells
3. Kill enemies to build Soul Shards (25% chance per kill/crit, watch resource bar)
4. Build to 3 Soul Shards maximum (check resource bar shows "Soul Shards: 3/3")
5. **Verify spell availability**: Open spellbook → Ultimate spells (Circle 5-7) should be enabled/visible
6. **Cast Soul Shatter**: Double-click Soul Shatter spell (Circle 5) → Target enemy → Verify high damage, 1 Shard consumed (3 → 2, watch resource bar)
7. **Cast Apocalyptic Chaos**: Double-click Apocalyptic Chaos spell (Circle 7) → Target ground/AoE → Verify AoE effect, 2 Shards consumed (2 → 0, watch resource bar)

#### Focus (Ranger/Bounty Hunter) - Threshold: 100 (Max)
**At 100 Focus (Maximum):**
- [ ] **Aimed Shot Enhanced**: Maximum damage bonus from Focus
- [ ] **Precision Abilities**: High-accuracy abilities available
- [ ] **Focus Scaling**: Ranged damage scales with Focus amount
- [ ] **Stationary Regen**: +10/sec while stationary
- [ ] **Moving Decay**: -5/sec while moving

**Activation Instructions:**
- **Ranger/Bounty Hunter**: Martial classes - use ability system (no spellbook)
- **Ability Access**: Open ability list/gump → Ranged abilities available
- **Ability Names**: Look for "Aimed Shot", "Multishot", "Precise Shot" in ability list
- **Passive Scaling**: Focus enhances abilities automatically (damage scales with Focus amount)
- **No Manual Activation**: Focus is passive - abilities automatically use current Focus amount

**Test Procedure:**
1. Stand still to build Focus to 100 (10/sec regen, watch resource bar increase)
2. **Use Aimed Shot**: 
   - Open ability list/gump → Select "Aimed Shot" ability
   - Target enemy → Activate ability
3. Verify Aimed Shot shows maximum damage (damage scales with Focus: 0 Focus = base, 100 Focus = +bonus)
4. Move around → Verify Focus decays at 5/sec (watch resource bar decrease)
5. Stand still again → Verify Focus regens at 10/sec (watch resource bar increase)
6. Test ranged abilities at different Focus levels → Use Aimed Shot at 0 Focus, then rebuild to 100 and compare damage

#### Crescendo (Bard) - Threshold: 20 (Max)
**At 20 Crescendo (Maximum):**
- [ ] **Crescendo Cap**: Resource caps at 20 (+synergy bonus if applicable)
- [ ] **Crescendo Generation**: Increases on successful Songweaving songs
- [ ] **Decay**: Begins after 3s out of combat, -1 per tick

**Activation Instructions:**
- **Bard Class**: Uses **Songbook of Weaving** + **Songweaving Hotbar**
- **Get Songbook**: Receive Songbook when selecting Bard class, or obtain from trainer/vendor
- **Song Access**: Use songbook, hotbar, or `[song <name>]` command
- **Core Songs**: Provocation, Peacemaking, Discordance, Requiem, Mending, Courage, Swiftness, Light, Fortune

**Test Procedure:**
1. **Get Songbook**: Receive Songbook when selecting Bard class, or obtain from trainer/vendor
2. **Open Songbook**: Double-click Songbook ? View song list
3. **Cast Songs**: Use songbook/hotbar to cast Mending or Courage ? Verify Crescendo increases on success
4. **Cooldown**: Attempt to spam a song ? Verify cooldown blocks rapid re-cast (hotbar shows timer)
5. **Decay**: Exit combat and wait 3 seconds ? Verify Crescendo decays over time

#### Faith (Cleric) - Threshold: 100 (Max)
**At 100 Faith (Maximum):**
- [ ] **Divine Abilities Unlocked**: Most powerful healing/divine abilities available
- [ ] **Available Abilities** (Cleric abilities, IDs ~2192-2207):
  - [ ] **Resurrection**: Revive dead player, costs 20 Faith
  - [ ] **Divine Intervention**: Massive heal + buff, costs 10 Faith
  - [ ] **Blessing of Light**: Party-wide buff, costs 5 Faith
- [ ] **Faith Generation**: +5 per heal, +10 per crit heal, +20 per resurrection
- [ ] **Faith Persistence**: Faith does not decay (persists until used)

**Activation Instructions:**
- **Cleric Class**: Martial class - uses ability system (no spellbook)
- **Ability Access**: Open ability list/gump → Divine abilities appear when Faith = 100
- **Ability Names**: Look for "Resurrection", "Divine Intervention", "Blessing of Light" in ability list

**Test Procedure:**
1. Heal allies to build Faith to 100 (+5 per heal, +10 per crit heal, +20 per resurrection, watch resource bar)
2. **Verify ability availability**: Open ability list/gump → Divine abilities should be enabled/visible
3. **Activate Resurrection**: 
   - Select "Resurrection" ability from ability list
   - Target dead player/creature → Activate ability
   - Verify revive works, 20 Faith consumed (100 → 80, watch resource bar)
4. **Activate Divine Intervention**: 
   - Select "Divine Intervention" ability from ability list
   - Target damaged ally → Activate ability
   - Verify massive heal, 10 Faith consumed (80 → 70, watch resource bar)
5. Continue healing → Verify Faith regenerates (+5 per heal, watch resource bar increase)

### Resource Decay
- [ ] Fury decays at 5/sec after 3 seconds out of combat
- [ ] Chi does NOT decay (persists until used)
- [ ] Combo Points expire after 30 seconds per target
- [ ] Energy regenerates passively (5/sec)
- [ ] Focus: +10/sec stationary, -5/sec moving
- [ ] Fortitude: -1/sec out of combat
- [ ] Zeal: -1/sec out of combat
- [ ] Soul Shards: No decay (persist until used)
- [ ] Faith: No decay (persists until used)
- [ ] Crescendo: Increases on successful songs, decays out of combat after 3s

### All 25 Classes Test Matrix
| Class | Resource | Primary Stat | Verified |
|-------|----------|--------------|----------|
| Barbarian | Fury | STR 100 | [ ] |
| Beastmaster | Nature | DEX 85 | [ ] |
| IceMage | Chill | INT 100 | [ ] |
| Sorcerer | Heat | INT 100 | [ ] |
| Ranger | Focus | DEX 100 | [ ] |
| Illusionist | Shadow | INT 90 | [ ] |
| Witch | Shadow | INT 95 | [ ] |
| Warlock | Soul | INT 100 | [ ] |
| Necromancer | Soul | INT 100 | [ ] |
| Druid | Nature | INT 90 | [ ] |
| Alchemist | Essence | INT 85 | [ ] |
| Wizard | Essence | INT 100 | [ ] |
| Oracle | Spirit | INT 95 | [ ] |
| Artificer | Energy | INT 80 | [ ] |
| Fighter | Combo | STR 95 | [ ] |
| Monk | Chi | DEX 90 | [ ] |
| Templar | Spirit | STR 85 | [ ] |
| Summoner | Essence | INT 100 | [ ] |
| BountyHunter | Focus | DEX 95 | [ ] |
| Knight | Fury | STR 90 | [ ] |
| Shaman | Spirit | INT 85 | [ ] |
| Cleric | Spirit | INT 90 | [ ] |
| Paladin | Spirit | STR 85 | [ ] |
| Bard (Songweaving) | Crescendo | DEX 80 | [ ] |
| Enchanter | Essence | INT 95 | [ ] |

## Phase 3: Buff/Debuff System

### Stat Buffs

**Test with Nature Magic (Druid):**
- [ ] Cast **Nature's Blessing** (Circle 2, ID 1037) on target → +10 to all stats (STR, DEX, INT)
- [ ] Buff icon appears in buff bar
- [ ] Check character sheet → Stats increased by 10
- [ ] Buff expires after 60 seconds → Stats revert to normal
- [ ] Visual: Green glow/particles on target

**Test with Enchanting Magic (Enchanter):**
- [ ] Cast **Enhance Strength** (Circle 2, ID 1325) → +15 STR buff
- [ ] Cast **Enhance Dexterity** (Circle 2, ID 1326) → +15 DEX buff
- [ ] Multiple stat buffs stack (STR + DEX both active)
- [ ] Buffs expire independently based on duration

### Stack Behaviors

#### Refresh Behavior
**Test Spells/Abilities:**
- [ ] **Nature's Blessing** (Nature Magic Circle 2): +10 all stats, 60s duration
  - Cast twice on same target → Duration resets to 60s, power stays +10
  - Buff icon shows refreshed timer
- [ ] **Barkskin** (Nature Magic Circle 1): +15 AR, 60s duration
  - Recast before expiry → Duration resets, AR bonus unchanged
- [ ] **Frost Armor** (Ice Magic Circle 4): +Cold Resist, 60s duration
  - Recast → Duration refreshes, resist bonus same

#### Stack Behavior
**Test Spells/Abilities:**
- [ ] **Chill Stacks** (Ice Mage): Each ice spell adds 1 stack, max 5
  - Cast Ice Bolt (1008) 5 times → Should see "Chill: 5/5"
  - At 5 stacks: Target becomes Frozen (rooted)
- [ ] **Combo Points** (Fighter/Rogue): +1 per hit, max 5
  - Attack same target 5 times → Should see "Combo: 5/5"
  - Finisher abilities consume all stacks
- [ ] **Bleed Stacks** (Martial abilities): Some abilities stack bleed
  - Apply multiple bleed effects → Each adds stack, damage increases

#### Replace Behavior
**Test Spells/Abilities:**
- [ ] **Shield** spells: Higher circle replaces lower
  - Cast **Ice Shield** (Ice Magic Circle 2, ID 1005) → 200 HP absorb
  - Cast **Glacial Fortress** (Ice Magic Circle 6, ID 1021) → 500 HP absorb replaces old
  - Verify: Old shield removed, new shield active, only one shield at a time
- [ ] **Armor Buffs**: Stronger replaces weaker
  - Cast **Barkskin** (Nature Magic Circle 1, ID 1035) → +15 AR
  - Cast **Living Fortress** (Nature Magic Circle 6, ID 1054) → +50 AR replaces Barkskin
- [ ] **Enhancement Spells** (Enchanter): Higher circle replaces lower
  - Cast **Enhance Strength** (Circle 2) → +15 STR
  - Cast **Greater Enhance Strength** (Circle 5) → +30 STR replaces +15

#### Ignore Behavior
**Test Spells/Abilities:**
- [ ] **Bear Form** (Druid): Transform spell
  - Cast Bear Form → Transform active
  - Cast Bear Form again → No effect, message "You are already in Bear Form"
- [ ] **Stealth** (Rogue abilities): Hide ability
  - Activate stealth → Hidden
  - Activate stealth again → No effect, already hidden

### DoT Damage (Damage Over Time)

#### Basic DoT Testing
**Test with Elemental Magic (Sorcerer):**
- [ ] Cast **Ignite** (Circle 1, ID 1097) on target → Fire DoT applied
- [ ] DoT ticks every 2-3 seconds (~4 ticks over 8s duration)
- [ ] Total damage over duration: ~8-12 total (2 damage per tick × 4 ticks)
- [ ] DoT removed when duration expires
- [ ] Visual: Target shows fire particles on each tick
- [ ] Combat log shows "Target takes 2 fire damage from Ignite" per tick

#### DoT Spells by Magic School

**Ice Magic (Ice Mage):**
- [ ] **Frostbite** (Circle 3, ID 1009): Cold DoT, 3-5/tick, 10s duration
- [ ] **Hypothermia** (Circle 5, ID 1019): Cold DoT, 5-8/tick, 15s duration
- [ ] **Permafrost** (Circle 4, ID 1015): Cold DoT zone, 2-4/tick, 12s duration

**Nature Magic (Druid):**
- [ ] **Poison Spores** (Circle 2, ID 1037): Poison DoT, 3-5/tick, 8s duration
- [ ] **Strangling Vines** (Circle 3, ID 1041): Physical DoT + Root, 4-6/tick, 10s duration
- [ ] **Toxic Bloom** (Circle 6, ID 1057): Poison DoT AoE, 5-8/tick, 12s duration
- [ ] **Plague** (Circle 4, ID 1049): Poison DoT that spreads, 6-10/tick, 15s duration

**Hex Magic (Witch):**
- [ ] **Hex of Pain** (Circle 3, ID 1073): Shadow DoT, 3-5/tick, 10s duration
- [ ] **Hex of Agony** (Circle 4, ID 1077): Poison DoT + Anti-Heal, 3-5/tick, 8-12s duration
  - **CRITICAL:** Healing converts to damage while active
- [ ] **Torment** (Circle 5, ID 1081): Shadow DoT, 5-8/tick, 15s duration
- [ ] **Corruption** (Dark Magic Circle 1, ID 1129): Shadow DoT, 3-5/tick, 10s duration

**Elemental Magic (Sorcerer):**
- [ ] **Ignite** (Circle 1, ID 1097): Fire DoT, 2/tick, 8s duration
- [ ] **Flame Pillar** (Circle 3, ID 1105): Fire DoT zone, 4-6/tick, 10s duration
- [ ] **Lava Flow** (Circle 4, ID 1113): Fire DoT zone, 5-8/tick, 10s duration
- [ ] **Inferno** (Circle 4, ID 1111): Fire DoT AoE, 6-10/tick, 12s duration

**Dark Magic (Warlock):**
- [ ] **Corruption** (Circle 1, ID 1129): Shadow DoT, 3-5/tick, 10s duration
- [ ] **Rain of Fire** (Circle 4, ID 1141): Fire DoT zone, 6-9/tick, 12s duration
- [ ] **Void Zone** (Circle 7, ID 1155): Shadow DoT zone, 8-12/tick, 15s duration

**Necromancy (Necromancer):**
- [ ] **Soul Drain** (Circle 3, ID 1197): Shadow DoT + Mana drain, 4-6/tick, 12s duration
- [ ] **Decay** (Circle 4, ID 1201): Physical DoT, 5-8/tick, 15s duration

#### DoT Stacking
- [ ] Multiple DoTs of different types stack (Burn + Poison + Corruption)
- [ ] Same DoT type from different sources may stack (check per-spell design)
- [ ] DoT damage shows in combat log with source name
- [ ] DoT particles visible on target during each tick

### HoT Healing (Heal Over Time)

#### Basic HoT Testing
**Test with Nature Magic (Druid):**
- [ ] Cast **Rejuvenation** (Circle 3, ID 1036) on damaged target → HoT applied
- [ ] Heal ticks every 2-3 seconds (~4 ticks over 10s duration)
- [ ] Total healing: ~20 HP (5 HP per tick × 4 ticks)
- [ ] HoT removed when duration expires
- [ ] Visual: Target shows green particles on each tick
- [ ] Combat log shows "Target heals 5 HP from Rejuvenation" per tick
- [ ] HoT stops at max HP (does not overheal)

#### HoT Spells by Magic School

**Nature Magic (Druid):**
- [ ] **Rejuvenation** (Circle 3, ID 1036): HoT, 5 HP/tick, 10s duration (50 total)
- [ ] **Greater Regen** (Circle 4, ID 1044): HoT, 8 HP/tick, 20s duration (160 total)
- [ ] **Healing Grove** (Circle 5, ID 1052): Ground-target HoT zone, 10 HP/tick, 15s duration
  - All allies in 5-tile radius heal per tick
- [ ] **Lifebloom** (Druid ability): HoT, 6-10/tick, blooms for 25-40 at end

**Shamanic Magic (Shaman):**
- [ ] **Healing Stream** (Circle 2, ID 1261): HoT, 4 HP/tick, 12s duration
- [ ] **Regeneration Totem** (Circle 4, ID 1269): Totem provides HoT to nearby allies

**Songweaving (Bard):**
- [ ] **Mending**: Party HoT while song is active

**Cleric Abilities:**
- [ ] **Regeneration** (Cleric ability): HoT, 8-12/tick, 15s duration

#### HoT Mechanics
- [ ] HoT does not overheal (stops at max HP)
- [ ] Multiple HoTs stack (Rejuvenation + Greater Regen = both active)
- [ ] HoT shows green particles/healing effect on each tick
- [ ] HoT healing visible in combat log
- [ ] HoT continues through combat (not removed by damage)

### Shields (Damage Absorption)

#### Basic Shield Testing
**Test with Ice Magic (Ice Mage):**
- [ ] Cast **Ice Shield** (Circle 2, ID 1005) on self → 200 HP absorb shield created
- [ ] Shield shows visual effect (ice aura, particles)
- [ ] Take damage from enemy → Shield absorbs damage before HP
- [ ] Message: "Shield absorbs X damage! (Y remaining)"
- [ ] Shield breaks when depleted (message: "Shield broken!")
- [ ] Partial damage bleeds through when shield < damage
  - Example: 100 shield remaining, 300 damage → 100 absorbed, 200 to HP
- [ ] Shield remaining shown in buff tooltip/bar
- [ ] Shield expires after 60 seconds if not broken

#### Shield Spells by Magic School

**Ice Magic (Ice Mage):**
- [ ] **Ice Shield** (Circle 2, ID 1005): 200 HP absorb, 60s duration, +Cold Resist
- [ ] **Frost Armor** (Circle 4, ID 1012): 300 HP absorb, 60s duration, +Cold Resist
- [ ] **Glacial Fortress** (Circle 6, ID 1021): 500 HP absorb, 90s duration, +Cold Resist, slows attackers

**Elemental Magic (Sorcerer):**
- [ ] **Heat Shield** (Circle 1, ID 1096): 150 HP absorb, 60s duration, +Fire Resist
- [ ] **Flame Ward** (Circle 2, ID 1102): 250 HP absorb, 60s duration, reflects fire damage
- [ ] **Ring of Fire** (Circle 4, ID 1114): 300 HP absorb zone, 30s duration, damages melee attackers

**Dark Magic (Warlock):**
- [ ] **Demonic Armor** (Circle 2, ID 1134): 200 HP absorb, 60s duration, dark aura
- [ ] **Demon Skin** (Circle 4, ID 1142): 400 HP absorb, 60s duration, +40 AR

**Summoning Magic (Summoner):**
- [ ] **Summon Shield** (Circle 3, ID 1232): Summon provides shield to caster

**Enchanting Magic (Enchanter):**
- [ ] **Runic Shield** (Circle 3, ID 1328): 250 HP absorb, 60s duration, +Energy Resist

**Divination Magic (Oracle):**
- [ ] **Temporal Shield** (Circle 5, ID 1177): 400 HP absorb, remembers HP, restores after 4s

#### Shield Mechanics
- [ ] Shield absorbs all damage types (physical, fire, cold, etc.)
- [ ] Shield does not prevent status effects (DoT, CC still apply)
- [ ] Multiple shields do NOT stack (newer replaces older)
- [ ] Shield visual effect visible on character
- [ ] Shield remaining HP shown in buff bar tooltip
- [ ] Message on shield break: "Shield absorbs X damage! (Y remaining)" or "Shield broken!"

## Phase 4: Crowd Control & Diminishing Returns

### CC Application

#### Basic CC Testing
**Test with Nature Magic (Druid):**
- [ ] Cast **Earthquake** (Circle 5, ID 1053) on target → Stun 2s applied
- [ ] Stunned target cannot move or attack
- [ ] Target shows paralyzed animation
- [ ] Target resumes actions after stun ends
- [ ] CC visual effect visible (particles, animation)

**Test with Ice Magic (Ice Mage):**
- [ ] Cast **Freezing Grasp** (Circle 2, ID 1004) on target → Freeze (root) 3s
- [ ] Target cannot move but can still cast spells/attack
- [ ] Visual: Ice particles, frozen appearance

#### CC Spells by Magic School

**Ice Magic (Ice Mage):**
- [ ] **Freezing Grasp** (Circle 2, ID 1004): Freeze (root) 3s, can still cast
- [ ] **Frozen Tomb** (Circle 5, ID 1017): Freeze 5s, high cold damage
- [ ] **Deep Freeze** (Circle 6, ID 1022): Freeze 6s, AoE 3 tiles
- [ ] **Chill 5 Stacks** → Auto-Freeze: Rooted, cannot move

**Nature Magic (Druid):**
- [ ] **Entangle** (Circle 2, ID 1037): Root 4s, cannot move
- [ ] **Strangling Vines** (Circle 3, ID 1041): Root + DoT, 6s duration
- [ ] **Earthquake** (Circle 5, ID 1053): Stun 2s + AoE damage

**Hex Magic (Witch):**
- [ ] **Evil Eye** (Circle 1, ID 1066): Daze (accuracy debuff), 20s duration
- [ ] **Crippling Curse** (Circle 3, ID 1079): Slow -30 DEX, 30s duration

**Dark Magic (Warlock):**
- [ ] **Fear** (Circle 1, ID 1130): Fear 3s, target flees
- [ ] **Terror** (Circle 4, ID 1144): Mass Fear 4s, AoE 5 tiles
- [ ] **Mass Fear** (Circle 6, ID 1150): Fear all enemies 5s, AoE 6 tiles
- [ ] **Nightmare** (Circle 2, ID 1134): Sleep + DoT, breaks on damage

**Divination Magic (Oracle):**
- [ ] **Time Freeze** (Circle 3, ID 1169): Stun 3s, single target
- [ ] **Time Stop** (Circle 6, ID 1185): Freeze 3s, AoE 5 tiles

**Illusion Magic (Illusionist):**
- [ ] **Mesmerize** (Circle 2, ID 1357): Incapacitate 4s, target confused
- [ ] **Confusion** (Circle 3, ID 1361): Random actions, 6s duration
- [ ] **Polymorph** (Circle 5, ID 1373): Transform target, 10s duration

**Songweaving (Bard):**
- [ ] **Peacemaking**: Pacify target briefly (breaks on damage)

#### CC from Martial Abilities
- [ ] **Shield Bash** (Fighter/Knight): Stun 2s, melee range
- [ ] **Charge** (Fighter): Stun 2s, gap closer
- [ ] **Concussive Blow** (Knight): Stun 4s, costs 2 Fortitude
- [ ] **Pounce** (Druid Cat Form): Stun 3s, from stealth

### Diminishing Returns (Same CC Type)
| Application | Duration | DR Level | Verified |
|-------------|----------|----------|----------|
| 1st Stun | 5.0s (100%) | 0 | [ ] |
| 2nd Stun | 2.5s (50%) | 1 | [ ] |
| 3rd Stun | 1.25s (25%) | 2 | [ ] |
| 4th Stun | 0s (immune) | 3 | [ ] |
| After 15s | Full duration | Reset | [ ] |

### DR Categories
- [ ] Stun category: Stun, Bash, KnockOut share DR
- [ ] Incapacitate category: Mesmerize, Daze share DR
- [ ] Root category: Root, Entangle, Pin share DR
- [ ] Silence category: Silence, Mute share DR
- [ ] None category: Slow, Blind have NO DR

### CC Commands
- [ ] `[CheckDR Stun` shows current DR level and reset timer
- [ ] `[RemoveCC Stun` removes CC from target
- [ ] `[ResetDR` clears all DR on target

## Phase 5: Stance System (28 Stances)

### Stance Activation

**Test with Fighter Class:**
- [ ] Use **Aggressive Stance** ability → Activates Aggressive stance
- [ ] Stance icon/visual appears on character
- [ ] Check character sheet → +20% damage, -10% defense visible
- [ ] Only one stance active at a time (cannot have Aggressive + Defensive)

**Test with Druid Class:**
- [ ] Cast **Bear Form** spell (Nature Magic Circle 4, ID 1048) → Transforms to Bear
- [ ] Body changes to bear appearance
- [ ] Check stats → +30 STR, +20% max HP, +15 Physical Resist
- [ ] Cannot cast other forms while in Bear Form

### Stance Switching

**Test with Fighter Class:**
- [ ] Activate **Aggressive Stance** → Active
- [ ] Activate **Defensive Stance** → Replaces Aggressive
- [ ] 0.5 second transform time during switch (character animation)
- [ ] Minimum 1.5 second cooldown before can switch again
- [ ] Cannot activate same stance while already active

**Test with Druid Class:**
- [ ] Cast **Bear Form** → Bear form active
- [ ] Cast **Cat Form** → Transforms from Bear to Cat
- [ ] Transform animation plays during switch
- [ ] Cooldown prevents rapid form switching

### Stance Bonuses

#### Fighter Stances
- [ ] **Aggressive Stance**: +20% damage, -10% defense, -15% block chance
- [ ] **Defensive Stance**: +25% armor, +20% block chance, +15% parry, +5% damage reduction, -15% damage dealt
- [ ] **Balanced Stance**: No bonuses or penalties (baseline)
- [ ] **Berserker Stance**: +30% damage, -20% armor, high risk/reward

#### Druid Forms
- [ ] **Bear Form**: +30 STR, +20% max HP, +15 Physical Resist, -10% speed (tank form)
- [ ] **Cat Form**: +20 DEX, +15% crit chance, stealth available (DPS form)
- [ ] **Tree Form**: +25 INT, +20% healing power, rooted (healer form)
- [ ] **Moonkin Form**: +30 INT, +15% spell damage (caster form)
- [ ] **Travel Form**: +100% speed, no combat allowed (movement only)

#### Sorcerer Elemental Stances
- [ ] **Fire Stance**: +20% fire damage, +10% spell damage, fire aura visible
- [ ] **Water Stance**: +20% cold damage, +10% healing power, water aura
- [ ] **Earth Stance**: +30% armor, +15 Physical Resist, -10% speed, slow but strong
- [ ] **Air Stance**: +15% speed, +20% evasion, +10% cast speed

#### Barbarian States
- [ ] **Normal State**: Baseline stats
- [ ] **Rage** (requires 100 Fury): +30 STR, +25% damage, -15 INT, drains 5 Fury/sec
  - Visual: Character transforms, red aura
  - Cannot cast spells while in Rage
  - Auto-deactivates when Fury reaches 0

#### Monk Stances
- [ ] **Windwalker Stance**: +20% speed, +15% evasion, +10% crit chance
- [ ] **Iron Fist Stance**: +15% damage, +10% armor, +5% block chance

#### Rogue Stances
- [ ] **Shadow Stance**: Stealth available, +20% crit damage, +10% evasion
- [ ] **Poison Stance**: Poison attacks enhanced, +15% poison damage
- [ ] **Evasion Stance**: +25% evasion, +15% dodge chance
- [ ] **Precision Stance**: +20% accuracy, +15% crit chance

#### Visual Indicators
- [ ] Stance icon appears in buff bar
- [ ] Body transformation visible for forms (Bear, Cat, Tree, etc.)
- [ ] Aura particles for elemental stances (Fire, Water, etc.)
- [ ] Stat changes visible in character sheet
- [ ] `[StanceInfo` command shows all active bonuses

### Stance Categories
- [ ] Warrior stances: Berserker, Defensive, Balanced, Aggressive
- [ ] Mage stances: Fire, Ice, Lightning, Arcane
- [ ] Rogue stances: Shadow, Poison, Evasion, Precision
- [ ] Tank stances: Iron, Stone, Guardian, Fortress

## Phase 6: Magic System (12 Schools, 384 Spells)

### Spellbook Acquisition
| School | Spellbook Name | How to Get | Spell IDs | Verified |
|--------|----------------|------------|-----------|----------|
| Ice Magic | IceMageSpellbook | Receive when selecting Ice Mage class, or buy from trainer/vendor | 1000-1031 | [ ] |
| Nature Magic | DruidSpellbook | Receive when selecting Druid class, or buy from trainer/vendor | 1032-1063 | [ ] |
| Hex Magic | WitchSpellbook | Receive when selecting Witch class, or buy from trainer/vendor | 1064-1095 | [ ] |
| Elemental Magic | SorcererSpellbook | Receive when selecting Sorcerer class, or buy from trainer/vendor | 1096-1127 | [ ] |
| Dark Magic | WarlockSpellbook | Receive when selecting Warlock class, or buy from trainer/vendor | 1128-1159 | [ ] |
| Divination | OracleSpellbook | Receive when selecting Oracle class, or buy from trainer/vendor | 1160-1191 | [ ] |
| Necromancy | NecromancerSpellbook | Receive when selecting Necromancer class, or buy from trainer/vendor | 1192-1223 | [ ] |
| Summoning | SummonerSpellbook | Receive when selecting Summoner class, or buy from trainer/vendor | 1224-1255 | [ ] |
| Shamanic | ShamanSpellbook | Receive when selecting Shaman class, or buy from trainer/vendor | 1256-1287 | [ ] |
| Songweaving | Songbook of Weaving | Receive when selecting Bard class, or buy from trainer/vendor | songs list | [ ] |
| Enchanting | EnchanterSpellbook | Receive when selecting Enchanter class, or buy from trainer/vendor | 1320-1351 | [ ] |
| Illusion | IllusionistSpellbook | Receive when selecting Illusionist class, or buy from trainer/vendor | 1352-1383 | [ ] |

### Spell Casting
- [ ] Spellbook opens and shows 8 circles (4 spells each)
- [ ] Circle 1 spell costs ~4-8 mana
- [ ] Circle 8 spell costs ~50-80 mana
- [ ] Casting animation plays during cast time
- [ ] Spell effect applies on completion

### Reagent Consumption
- [ ] Ice spells consume FrozenTear, GlacialShard, IceDust, etc.
- [ ] Standard UO reagents (BlackPearl, Ginseng) do NOT work
- [ ] "You lack the reagents" message when missing

### Skill Scaling
- [ ] 50 skill vs 100 skill = ~50% damage difference
- [ ] IceMagic skill affects Ice spells
- [ ] NatureMagic skill affects Nature spells
- [ ] Correct skill governs each school

## Phase 7: Creature & Boss Mechanics

### Elemental Resistances
- [ ] Cold spell vs Ice Golem = ~80% reduced damage
- [ ] Fire spell vs Ice Golem = full/bonus damage
- [ ] Fire spell vs Magma Troll = ~90% reduced damage
- [ ] Cold spell vs Magma Troll = full/bonus damage

### Regional Loot Drops
| Region | Creature | Expected Drops | Verified |
|--------|----------|----------------|----------|
| Frosthold | Ice Golem | FrozenOre, GlacialShard | [ ] |
| Emberlands | Magma Troll | MoltenOre, EmberShard | [ ] |
| Verdantpeak | Treant | LivingBark, TreantHeart | [ ] |
| Crystal Barrens | CrystalGolem | CrystalOre, PrismaticShard | [ ] |
| Ironclad | ClockworkSpider | ClockworkGear, SteamCore | [ ] |

### Creature AI
- [ ] Caster creatures maintain range (8+ tiles)
- [ ] Melee creatures close to melee range (1-2 tiles)
- [ ] Creatures flee when low HP (some types)
- [ ] Tameable creatures show "This creature looks tameable"

### Boss Mechanics
- [ ] Bosses have 5000+ HP
- [ ] Bosses use special abilities (AoE, freeze, summons)
- [ ] Boss name in red/special color
- [ ] Boss loot includes rare drops

## Phase 8: Faction System (7 Factions)

### Reputation Display
- [ ] Use `[Factions` command → Shows all 7 faction reputations with current tier
- [ ] Each faction shows: Name, Reputation value, Tier (Neutral, Friendly, etc.)
- [ ] Color coding: Green (positive), Red (negative), Yellow (neutral)

### Reputation Gain Methods

**Gold Donation:**
- [ ] Talk to faction donation NPC (e.g., Frostguard representative)
- [ ] Select "Donate Gold" option from NPC menu
- [ ] Donate 1000 gold → Gain +50 reputation (1 piety per 20g = 50 rep per 1000g)
- [ ] Message: "You have donated 1000 gold to Frostguard and gained 50 reputation!"
- [ ] Reputation increases, tier may change (check faction status)

**Quest Completion:**
- [ ] Complete faction quest from Frostguard NPC
- [ ] Gain +100 to +500 reputation based on quest tier
- [ ] Message shows reputation gain and new tier if applicable
- [ ] Enemy faction (Flame Legion) loses 25% of gained rep

**Killing Faction Creatures:**
- [ ] Kill Frostguard enemy creature → Gain +10 to +50 rep
- [ ] Kill Frostguard allied creature → Lose -25 to -100 rep
- [ ] Message shows reputation change

### Reputation Tiers
| Tier | Rep Range | Vendor Discount | Verified |
|------|-----------|-----------------|----------|
| Hostile | -6000 to -3001 | +15% penalty | [ ] |
| Unfriendly | -3000 to -1 | +5% penalty | [ ] |
| Neutral | 0 to 2999 | 0% | [ ] |
| Friendly | 3000 to 5999 | -5% | [ ] |
| Honored | 6000 to 8999 | -8% | [ ] |
| Revered | 9000 to 11999 | -12% | [ ] |
| Exalted | 12000+ | -15% | [ ] |

### Vendor Discount Test
**Test Procedure:**
1. Find Frostguard faction vendor (or spawn with `[SpawnFactionVendor Frostguard`)
2. Check item price at Neutral tier (0 rep) → Note price (e.g., 1000g)
3. Gain reputation to Exalted (15000 rep) via donations or quests
4. Check same item again → Should be 15% cheaper (850g)
5. Verify discount applies automatically based on tier
6. Test at different tiers:
   - Friendly (3000 rep): -5% discount (950g)
   - Honored (6000 rep): -8% discount (920g)
   - Revered (12000 rep): -12% discount (880g)
   - Exalted (15000 rep): -15% discount (850g)

### Reputation Changes

**Killing Faction Creatures:**
- [ ] Kill Frostguard enemy (Flame Legion creature) → +10 to +50 rep with Frostguard
- [ ] Kill Frostguard ally (Frostguard creature) → -25 to -100 rep with Frostguard
- [ ] Message shows: "You gain/lose X reputation with [Faction]"

**Completing Faction Quests:**
- [ ] Accept quest from Frostguard NPC
- [ ] Complete quest objectives
- [ ] Turn in quest → +100 to +500 rep based on quest tier
- [ ] Tier change message if crossing threshold

**Faction NPC Interactions:**
- [ ] At Neutral: Standard dialogue, basic services
- [ ] At Friendly: "Welcome, friend of the Frostguard!"
- [ ] At Exalted: "Welcome, champion of the Frostguard!"
- [ ] At Hostile: NPCs may attack on sight

## Phase 9: Religion System (6 Religions)

### Religion Display
- [ ] Use `[Religion` command → Shows current religion and piety value
- [ ] Shows current tier (None, Initiate, Devoted, Faithful, Exalted)
- [ ] Shows available abilities for current tier

### Converting to a Religion

**Shrine Interaction:**
- [ ] Find or spawn Frosthelm Faith shrine (use `[ShrineStones` to spawn all 6)
- [ ] Double-click shrine → Shows conversion menu
- [ ] Select "Convert to Frosthelm Faith" → Joins religion
- [ ] Message: "You have converted to the Frosthelm Faith"
- [ ] Piety resets to 0 (if converting from another religion)
- [ ] Can now use shrine functions

### Piety Tiers
| Tier | Piety Range | Abilities | Verified |
|------|-------------|-----------|----------|
| None | 0-49 | Pray only | [ ] |
| Initiate | 50-199 | Tithe | [ ] |
| Devoted | 200-499 | Recharge, Pilgrimage | [ ] |
| Faithful | 500-899 | Bless Items | [ ] |
| Exalted | 900-1000 | Resurrection, All | [ ] |

### Shrine Functions

**Prayer (Daily):**
- [ ] Double-click Frosthelm Faith shrine → Select "Pray"
- [ ] Character performs bow animation
- [ ] Gain +10 piety
- [ ] Message: "You have prayed at the shrine. (+10 piety)"
- [ ] Can only pray once per real day (24 hours)
- [ ] Attempting to pray again: "You have already prayed today. Return tomorrow."

**Tithing (Gold Donation):**
- [ ] Double-click shrine → Select "Tithe"
- [ ] Enter gold amount (e.g., 1000 gold)
- [ ] Gain +10 piety (1 piety per 100g)
- [ ] Daily cap: 30 piety from tithes (3000g max per day)
- [ ] Message: "You have tithed X gold. (+Y piety)"

**Pilgrimage (Weekly):**
- [ ] Travel to Frosthelm Faith shrine
- [ ] Double-click shrine → Select "Perform Pilgrimage"
- [ ] Gain +75 piety
- [ ] Can only perform once per week (7 days)
- [ ] Visual: Particles, bow animation

**Bless Items (Faithful+ tier):**
- [ ] Reach Faithful tier (500 piety)
- [ ] Double-click shrine → "Bless Items" option available
- [ ] Select item to bless → Item gains blessed property
- [ ] Gain +25 piety for blessing item

**Resurrection (Exalted only):**
- [ ] Reach Exalted tier (900 piety)
- [ ] Double-click shrine → "Resurrection" option available
- [ ] Can resurrect dead player (if implemented)

### 6 Religions
- [ ] FrosthelmFaith shrine spawns and functions
- [ ] SuryasSandscript shrine spawns and functions
- [ ] LunarasCovenant shrine spawns and functions
- [ ] CelestisArcanum shrine spawns and functions
- [ ] OceanasCovenant shrine spawns and functions
- [ ] CogsmithCreed shrine spawns and functions

## Phase 10: LLM NPC Quest System

**Note:** This phase tests **Vystia Dynamic Quests** (QuestNPC/Chronicler).  
The **Mondain/BaseQuest** system is separate and uses classic quest givers.

### Auto-Greet
- [ ] Quest NPC greets player within 10 tiles
- [ ] Greeting occurs within 2-3 seconds of approach
- [ ] Greeting reflects NPC personality type
- [ ] Quest offer mentioned if Origin waypoint

### Context-Aware Dialogue
- [ ] Before accepting: "I have a task for you..."
- [ ] After accepting: "You've accepted my task..."
- [ ] During progress: References completed/remaining objectives
- [ ] On completion: "You've done it!"

### Quest Chain Continuity
- [ ] NPC B references NPC A by name
- [ ] Context preserved across chain NPCs
- [ ] Previous waypoint info available to next NPC

### Waypoint Types
- [ ] TalkToNPC: Complete by greeting target NPC
- [ ] ReachLocation: Auto-complete when entering area
- [ ] DefeatBoss: Complete when boss killed
- [ ] CollectItems: Complete when items obtained

### Location Hints
- [ ] NPC provides direction (north, southwest, etc.)
- [ ] Distance tier: "very close" (<50), "nearby" (50-200), "some distance" (200-500), "far" (500+)
- [ ] Example: "Head northeast about 150 tiles"

## Phase 11: Zone System

### Zone Types
| Zone | PvP | Death Penalty | Loot Drop | Verified |
|------|-----|---------------|-----------|----------|
| Sanctuary | No | 0% | 0% | [ ] |
| Contested | Consent | 25% | 10% | [ ] |
| Lawless | Yes | 50% | 50% | [ ] |
| Extreme | Yes | 100% | 100% | [ ] |

### Zone Detection
- [ ] Enter different zones → Verify zone type shown in status/UI
- [ ] Check zone boundaries → Verify zone changes when crossing boundaries
- [ ] Test PvP rules → Verify different rules apply in different zones

### PvP Rules
- [ ] Sanctuary: All PvP damage blocked
- [ ] Contested: Requires `[TogglePvP` consent
- [ ] Lawless: Open PvP, no consent needed
- [ ] Extreme: Full loot, max penalties

## Phase 12: AI Sidekick Mechanics

### Sidekick Spawning
- [ ] Use `[CreateSidekick Mage` or `[Sidekick` command → Opens sidekick creation menu
- [ ] Select sidekick archetype from menu (Mage, Warrior, Tamer, etc.)
- [ ] Sidekick spawns near player
- [ ] Sidekick is assigned to player and follows commands

### Combat Behavior
| Archetype | Range | Behavior | Verified |
|-----------|-------|----------|----------|
| Mage | 6-8 tiles | Kites, casts from range | [ ] |
| Warrior | 1 tile | Tanks, engages melee | [ ] |
| Tamer | 3-5 tiles | Supports pets, heals | [ ] |

### AI Features
- [ ] Sidekicks follow player out of combat (~3 tiles)
- [ ] Sidekicks attack player's combat target
- [ ] Sidekicks self-heal at ~40% HP
- [ ] Mage kites (moves away) if enemy closes

### Spawn Test Target
- [ ] `[ol` spawns Arctic Ogre Lord for combat test
- [ ] Sidekick engages automatically
- [ ] Verify damage/healing behavior

## Phase 13: Class-Specific Testing

### Magic Classes - Spell Testing

**Ice Mage (Cryomancy):**
- [ ] All 32 Ice Magic spells (IDs 1000-1031) accessible via spellbook
- [ ] Chill stack system: Each ice spell adds 1 stack, max 5
- [ ] At 5 Chill stacks: Target becomes Frozen (rooted)
- [ ] Class-specific buff: Frost Armor provides cold resistance
- [ ] Resource: Chill stacks tracked per-target

**Sorcerer (Elementalism):**
- [ ] All 32 Elemental Magic spells (IDs 1096-1127) accessible
- [ ] Fire DoT spells stack properly (Ignite, Flame Pillar, Lava Flow)
- [ ] Heat Shield and Flame Ward provide fire resistance
- [ ] Resource: Heat generation from fire spells

**Warlock (Demonology):**
- [ ] All 32 Dark Magic spells (IDs 1128-1159) accessible
- [ ] Corruption DoT applies shadow damage over time
- [ ] Soul Shard resource: Generated on kill/crit (25% chance)
- [ ] Demonic Armor provides shadow protection
- [ ] Fear and Terror CC effects work correctly

**Necromancer (NecromancyArts):**
- [ ] All 32 Necromancy spells (IDs 1192-1223) accessible
- [ ] Soul Drain DoT + mana drain works
- [ ] Life Force resource: +10-25 per nearby death
- [ ] Undead summoning abilities function
- [ ] Decay DoT applies physical damage over time

**Druid (Druidism):**
- [ ] All 32 Nature Magic spells (IDs 1032-1063) accessible
- [ ] Shapeshift forms: Bear, Cat, Tree, Moonkin, Travel
- [ ] HoT spells: Rejuvenation, Greater Regen, Healing Grove
- [ ] DoT spells: Poison Spores, Strangling Vines, Toxic Bloom
- [ ] Nature resource for shapeshifting

**Witch (Hexcraft):**
- [ ] All 32 Hex Magic spells (IDs 1064-1095) accessible
- [ ] Hex of Agony: Anti-heal (healing converts to damage)
- [ ] Hex of Pain DoT applies shadow damage
- [ ] Curses stack properly (Weakness, Frailty, Doom)
- [ ] Life drain spells heal caster

**Oracle (Divination):**
- [ ] All 32 Divination spells (IDs 1160-1191) accessible
- [ ] Time manipulation: Time Freeze, Time Stop
- [ ] Prismatic spells: Random element damage
- [ ] Foresight buffs provide dodge/evasion
- [ ] Spirit resource for divination powers

**Summoner (Conjuration):**
- [ ] All 32 Summoning spells (IDs 1224-1255) accessible
- [ ] Creature summoning works correctly
- [ ] Summon Shield provides protection
- [ ] Essence resource for summoning
- [ ] Summoned creatures follow commands

**Shaman (SpiritCalling):**
- [ ] All 32 Shamanic spells (IDs 1256-1287) accessible
- [ ] Totem placement and effects work
- [ ] Healing Stream HoT applies correctly
- [ ] Regeneration Totem provides area HoT
- [ ] Spirit resource for totem powers

**Bard (Songweaving):**
- [ ] Core songs accessible (Provocation, Peacemaking, Discordance, Requiem, Mending, Courage, Swiftness, Light, Fortune)
- [ ] Crescendo increases on successful songs and decays out of combat
- [ ] Hotbar shows cooldown and blocks rapid re-cast
- [ ] Mending applies party HoT in range
- [ ] Fortune applies luck bonus

**Enchanter (Runeweaving):**
- [ ] All 32 Enchanting spells (IDs 1320-1351) accessible
- [ ] Enhance Strength/Dexterity buffs stack properly
- [ ] Runic Shield provides damage absorption
- [ ] Item enhancement spells work
- [ ] Essence resource for enchanting

**Illusionist (IllusionMagic):**
- [ ] All 32 Illusion spells (IDs 1352-1383) accessible
- [ ] Mesmerize: Incapacitate CC works
- [ ] Confusion: Random actions apply
- [ ] Polymorph: Transform target
- [ ] Shadow resource for illusions

### Martial Classes - Ability Testing

**Barbarian (Berserking):**
- [ ] Fury resource: +5 per hit dealt, +10 per hit taken, +20 per kill
- [ ] Rage transformation: Activates at 100 Fury
- [ ] Rage drains 5 Fury/sec while active
- [ ] Reckless Strike: High damage, costs HP
- [ ] Frost Fury: Cold damage melee attack
- [ ] Whirlwind: AoE damage ability
- [ ] Battle Cry: Buff ability
- [ ] All 16 abilities (IDs 2016-2031) accessible

**Fighter (CombatMastery):**
- [ ] Combo Points: +1 per hit, max 5 per target
- [ ] Finisher abilities consume combo points
- [ ] Shield Bash: Stun 2s CC
- [ ] Charge: Gap closer + stun
- [ ] Stance abilities: Aggressive, Defensive, Balanced, Berserker
- [ ] All 16 abilities (IDs 2000-2015) accessible

**Monk (MartialArts):**
- [ ] Chi resource: +3 per hit, +15 on combo finisher
- [ ] Chi max: 5 stacks
- [ ] Jab: Chi builder ability
- [ ] Palm Strike: Chi spender
- [ ] Flying Kick: Mobility + damage
- [ ] Zen Focus: Self-buff
- [ ] All 16 abilities (IDs 2032-2047) accessible

**Rogue:**
- [ ] Combo Points: +1 per hit, max 5 per target
- [ ] Stealth: Hide ability works
- [ ] Backstab: High damage from stealth
- [ ] Eviscerate: Finisher consumes combo points
- [ ] Vanish: Escape ability
- [ ] All 16 abilities accessible

**Ranger (Marksmanship):**
- [ ] Focus resource: +2 per hit, +25 on crit
- [ ] Stationary: +10/sec regen
- [ ] Moving: -5/sec decay
- [ ] Aimed Shot: High damage ranged
- [ ] Multishot: AoE ranged
- [ ] Track: Utility ability
- [ ] All 16 abilities (IDs 2064-2079) accessible

**Knight (ChivalricArts):**
- [ ] Fury resource: +5 per hit, +10 taken, +20 kill
- [ ] Fortitude resource: +1 per block, max 10
- [ ] Shield Bash: Stun CC
- [ ] Concussive Blow: Stun 4s, costs 2 Fortitude
- [ ] Shield Wall: Defensive buff
- [ ] Rally: Party buff
- [ ] All 16 abilities (IDs 2080-2095) accessible

**Paladin (HolyDevotion):**
- [ ] Spirit resource for holy powers
- [ ] Holy Strike: Damage ability
- [ ] Lay on Hands: Direct heal
- [ ] Divine Shield: Protection buff
- [ ] Consecrate: Area effect
- [ ] All 16 abilities (IDs 2096-2111) accessible

**Templar (Zealotry):**
- [ ] Zeal resource: +1 per hit, +2 per crit, +3 per kill
- [ ] Zeal max: 10 stacks
- [ ] Taunt: Threat generation
- [ ] Holy Shield: Defensive buff
- [ ] Judgment: Zeal spender
- [ ] All 16 abilities (IDs 2112-2127) accessible

**Bounty Hunter (Manhunting):**
- [ ] Focus resource: +2 per hit, +25 on crit
- [ ] Pursuit stacks: Per marked target
- [ ] Mark Target: Debuff ability
- [ ] Track: Utility ability
- [ ] Execute: Finisher ability
- [ ] All 16 abilities (IDs 2128-2143) accessible

**Beastmaster (BeastBonding):**
- [ ] Nature resource for pet powers
- [ ] Beast Whistle: Summon companion
- [ ] Pack Tactics: Pet buff
- [ ] Command Pet: Control ability
- [ ] All 16 abilities (IDs 2144-2159) accessible

**Artificer (Engineering):**
- [ ] Energy resource: +10 per kill, +5/sec passive
- [ ] Steam resource: +10/sec near boiler
- [ ] Construct Control: Summon construct
- [ ] Turret Placement: Deploy turret
- [ ] All 16 abilities (IDs 2160-2175) accessible

**Alchemist (Transmutation):**
- [ ] Essence resource for alchemy
- [ ] Potion Creation: Craft potions
- [ ] Mutagen Application: Self-buff
- [ ] Bomb Throw: AoE damage
- [ ] All 16 abilities (IDs 2176-2191) accessible

**Cleric (DivineGrace):**
- [ ] Spirit resource for divine powers
- [ ] Faith resource: +5 per heal, +10 per crit, +20 per rez
- [ ] Direct Heal: Single target healing
- [ ] Regeneration: HoT ability
- [ ] Prayer: Resource generation
- [ ] All 16 abilities (IDs 2192-2207) accessible

### Class-Specific Buffs & Stackables

**Barbarian:**
- [ ] Rage buff: +30 STR, +25% damage, -15 INT (when Fury = 100)
- [ ] Battle Cry: Party buff (+STR)
- [ ] Frost Resistance: Passive cold resist

**Druid:**
- [ ] Bear Form buff: +30 STR, +20% max HP, +15 Physical Resist
- [ ] Cat Form buff: +20 DEX, +15% crit chance
- [ ] Tree Form buff: +25 INT, +20% healing power
- [ ] Moonkin Form buff: +30 INT, +15% spell damage

**Monk:**
- [ ] Chi stacks: Max 5, consumed by finishers
- [ ] Zen Focus: Self-buff ability
- [ ] Meditation: Resource generation buff

**Rogue:**
- [ ] Combo Points: Max 5 per target, consumed by finishers
- [ ] Stealth buff: Hidden state
- [ ] Shadow Cloak: Evasion buff

**Ranger:**
- [ ] Focus resource: Max 100, affects ranged damage
- [ ] Track buff: Reveals hidden targets
- [ ] Nature's Blessing: Self-buff

**Knight:**
- [ ] Fortitude stacks: Max 10, from blocking
- [ ] Shield Wall: Defensive buff
- [ ] Rally: Party buff (+armor)

**Paladin:**
- [ ] Divine Shield: Protection buff
- [ ] Consecration: Area buff
- [ ] Blessing: Party buff

**Templar:**
- [ ] Zeal stacks: Max 10, affects judgment damage
- [ ] Holy Shield: Defensive buff
- [ ] Redemption: Self-buff

### Class-Specific Crafting

**Alchemist:**
- [ ] Can craft potions with Transmutation skill
- [ ] Mutagens: Self-buff potions work
- [ ] Bombs: Throwable AoE items craftable
- [ ] Regional reagents affect potion power

**Artificer:**
- [ ] Can craft constructs with Engineering skill
- [ ] Turrets: Deployable items craftable
- [ ] Clockwork items: Regional variants available
- [ ] Steam-powered items function correctly

**Enchanter:**
- [ ] Can enhance items with Runeweaving skill
- [ ] Rune application: Adds properties to items
- [ ] Enhancement spells: Temporary item buffs
- [ ] Essence resource used for enchanting

**Beastmaster:**
- [ ] Can craft pet items with BeastBonding skill
- [ ] Pet equipment: Enhances companion stats
- [ ] Beast Whistle: Craftable summon item

**Cleric:**
- [ ] Can bless items with DivineGrace skill
- [ ] Blessed items: Gain holy properties
- [ ] Faith resource used for blessing

## Phase 14: Crafting System

### How to Perform Crafting Tests

**Crafting Setup:**
1. **Obtain Crafting Tools**:
   - **Standard Crafting**: Obtain tools from vendors or craft them
     - Blacksmithy: Tongs + Forge + Anvil
     - Tailoring: Sewing Kit
     - Carpentry: Saw + Dovetail Saw
     - Tinkering: Tinker's Tools
     - Fletching: Fletching Kit
     - Alchemy: Mortar and Pestle
     - Inscription: Blank Scroll
   - **Vystia-Specific Crafting**:
     - Engineering (Artificer): Engineering Tool Kit (craftable, requires 35-85 Engineering skill)
     - Transmutation (Alchemist): Mortar and Pestle (standard alchemy tool)
2. **Obtain Crafting Tables/Stations**:
   - **Standard Tables**: Find crafting tables in towns or test area
     - Forge (for Blacksmithy)
     - Anvil (for Blacksmithy)
     - Spinning Wheel (for Tailoring)
     - Loom (for Tailoring)
     - Carpenter's Bench (for Carpentry)
     - Tinker's Bench (for Tinkering)
     - Alchemy Table (for Alchemy)
   - **Test Area Tables**: Use "Spawn ALL Tables" option in test area (creates 12 tables at Green Acres)
3. **Gather Materials**:
   - **Standard Materials**: Mine ore, cut logs, gather cloth, etc.
   - **Regional Materials**: 
     - Mine in specific regions for regional ores (e.g., FrozenOre in Frosthold)
     - Smelt regional ores into regional ingots (e.g., FrozenOre → FrostforgedIngot)
     - Gather regional resources (plants, gems, etc.)
   - **Vystia-Specific Materials**:
     - Engineering: Clockwork Ingots, Clockwork Gears, Clockwork Springs, Steam Cores
     - Transmutation: Essence resource, potion ingredients

**Crafting Process:**
1. **Open Crafting Menu**:
   - **Standard Crafting**: Double-click crafting tool → Select crafting table/station → Crafting menu opens
   - **Engineering**: Double-click Engineering Tool Kit → Engineering menu opens (no table needed)
   - **Transmutation**: Double-click Mortar and Pestle → Transmutation menu opens (no table needed)
2. **Select Recipe**:
   - Browse recipe categories in crafting menu
   - Select desired item to craft
   - Menu shows: Required skill, required materials, material amounts
3. **Verify Materials**:
   - Check backpack has all required materials
   - Check material types match (e.g., FrostforgedIngot vs EmberforgedIngot)
   - Check material amounts are sufficient
4. **Craft Item**:
   - Click "Make" or select quantity
   - System checks skill level → Success chance calculated
   - System checks materials → Consumes materials on success
   - Item created and placed in backpack
   - Skill gains possible (if skill below cap)
5. **Verify Results**:
   - Check item properties match expected
   - Check regional properties (hue, damage bonuses, etc.)
   - Check exceptional quality (if skill high enough)
   - Check material consumption (materials removed from backpack)

**Crafting Test Checklist:**

### Regional Recipes
- [ ] Frostforged weapons require FrostforgedIngot
- [ ] EmberforgedIngot cannot substitute for FrostforgedIngot
- [ ] Wrong material type shows "You don't have the resources"
- [ ] All regional recipes visible in blacksmith menu
- [ ] **Test Procedure**:
  1. Mine FrozenOre in Frosthold region
  2. Smelt FrozenOre at Forge → Get FrostforgedIngot
  3. Open Blacksmithy menu (Tongs + Forge + Anvil)
  4. Select Frostforged weapon recipe
  5. Verify FrostforgedIngot is required material
  6. Try to use EmberforgedIngot → Verify error message
  7. Use FrostforgedIngot → Verify weapon crafts successfully

### Crafted Item Properties
- [ ] Frostforged items have cold damage bonus
- [ ] Emberforged items have fire damage bonus
- [ ] Regional hue matches region theme
- [ ] Correct stats on finished item
- [ ] **Test Procedure**:
  1. Craft Frostforged weapon (see Regional Recipes test)
  2. Check item properties → Verify cold damage bonus listed
  3. Check item hue → Verify matches Frosthold theme (blue/white)
  4. Equip weapon → Attack test dummy
  5. Verify cold damage applies to attacks
  6. Repeat with Emberforged weapon → Verify fire damage bonus

### Resource Chain
- [ ] Mining in Frosthold yields FrozenOre
- [ ] FrozenOre smelts to FrostforgedIngot
- [ ] FrostforgedIngot crafts Frostforged weapon
- [ ] Complete chain verified
- [ ] **Test Procedure**:
  1. Travel to Frosthold region
  2. Mine ore vein → Verify FrozenOre obtained
  3. Travel to Forge (or use portable forge)
  4. Smelt FrozenOre → Verify FrostforgedIngot created
  5. Open Blacksmithy menu → Select Frostforged weapon recipe
  6. Craft weapon using FrostforgedIngot → Verify weapon created
  7. Check weapon properties → Verify all regional properties present

### Class-Specific Crafting Tests

**Artificer (Engineering):**
- [ ] **Test Procedure**:
  1. Select Artificer class → Receive Engineering skill
  2. Craft Engineering Tool Kit (if not received):
     - Need: 3 Clockwork Ingot, 2 Clockwork Gears
     - Skill: 35-85 Engineering
  3. Double-click Engineering Tool Kit → Engineering menu opens
  4. Craft basic components (Clockwork Gear, Clockwork Spring)
  5. Craft gadgets (Smoke Grenade, Explosives)
  6. Craft constructs (Construct Control Device)
  7. Verify all recipes visible and craftable

**Alchemist (Transmutation):**
- [ ] **Test Procedure**:
  1. Select Alchemist class → Receive Transmutation skill
  2. Obtain Mortar and Pestle (vendor or craft)
  3. Gather Essence resource (class-specific resource)
  4. Double-click Mortar and Pestle → Transmutation menu opens
  5. Craft potions using Essence
  6. Craft bombs (throwable AoE items)
  7. Verify all recipes visible and craftable
  8. Verify Essence resource consumed correctly

## Phase 14: Ancient Being Encounters (12 Ancients)

### Ancient Being Stats
- [ ] 10,000+ HP (raid-tier)
- [ ] Multiple phases/abilities
- [ ] Unique mechanics per ancient
- [ ] Extreme loot drops

### LLM Dialogue
- [ ] Ancient beings have menacing dialogue
- [ ] Boss taunts during combat
- [ ] Death dialogue on defeat

### Quest Integration
- [ ] Ancient beings work as quest bosses
- [ ] DefeatBoss waypoint completes on kill
- [ ] Quest rewards granted

### All 12 Ancients
- [ ] FrosthelmEternalWinter spawns and functions
- [ ] EmberflameAshenTyrant spawns and functions
- [ ] VerdantheartForestGuardian spawns and functions
- [ ] CrystalwingPrismaticOracle spawns and functions
- [ ] AbyssusDepthKing spawns and functions
- [ ] ElderOakbark spawns and functions
- [ ] SphynxOfEmberlands spawns and functions
- [ ] FrostFathersAvatar spawns and functions
- [ ] GreatMachinistsConstruct spawns and functions
- [ ] LunarasDryadHerald spawns and functions
- [ ] TheCrystalSphinx spawns and functions
- [ ] IronbarkWarAncient spawns and functions

## Phase 15: Religion Quest Integration

### Religion-Gated Quests
- [ ] Non-follower blocked from religion quest
- [ ] Message: "You must follow [religion] to accept"
- [ ] After joining religion, quest available

### Piety Rewards
- [ ] Quest completion grants piety
- [ ] Piety amount matches quest reward setting
- [ ] NPC dialogue mentions faith/piety

### Shrine Quest Chains
- [ ] Quest directs player to shrine
- [ ] Shrine grants buff based on piety tier
- [ ] Location waypoint at shrine completes

## Phase 16: Faction Quest Integration

### Reputation-Gated Quests
- [ ] Hostile rep (-2000) blocks quest acceptance
- [ ] Message: "The [faction] will not trust you"
- [ ] Improving rep allows quest acceptance

### Reputation Rewards
- [ ] Quest completion grants reputation
- [ ] Rep tier may change (Neutral -> Friendly)
- [ ] Vendor prices update after rep change

### Faction NPC Dialogue
- [ ] At Neutral: Standard responses
- [ ] At Exalted: "Welcome, champion of the [faction]!"
- [ ] Dialogue changes reflect reputation tier

---

## Master Summary

| Phase | System | Tests | Passed | Failed |
|-------|--------|-------|--------|--------|
| 1 | Environment Setup | 5 | | |
| 2 | Class System | 60+ | | |
| 3 | Buff/Debuff | 80+ | | |
| 4 | Crowd Control/DR | 40+ | | |
| 5 | Stance System | 35+ | | |
| 6 | Magic System | 20 | | |
| 7 | Creatures/Bosses | 15 | | |
| 8 | Faction System | 15 | | |
| 9 | Religion System | 15 | | |
| 10 | LLM NPC Quests | 15 | | |
| 11 | Zone System | 12 | | |
| 12 | AI Sidekicks | 12 | | |
| 13 | Crafting | 10 | | |
| 14 | Ancient Beings | 20 | | |
| 15 | Religion Integration | 8 | | |
| 16 | Faction Integration | 10 | | |
| | **TOTAL** | **350+** | | |

**Note:** Test counts increased due to detailed spell/ability testing requirements.

---

## Code Changes Required

Before testing secondary resource generation from melee combat:

### BaseWeapon.cs Hook (Line ~2892)
- [x] Added `pmResourceGen` variable for resource generation
- [x] Calls `VystiaResourceManager.GetManager(pmResourceGen)`
- [x] Calls `manager?.OnDamageDealt(defender, damageGiven, isCrit)`

### PlayerMobile.cs Hook (Line ~3720)
- [x] Added before `base.OnDamage(amount, from, willKill)`
- [x] Calls `VystiaResourceManager.GetManager(this)`
- [x] Calls `manager?.OnDamageTaken(from, amount)`

---

*Last Updated: 2026-01-06*
*Enhanced: Added detailed spell lists, resource thresholds, stack behaviors, DoT/HoT/Shield/CC examples, and stance bonuses*

