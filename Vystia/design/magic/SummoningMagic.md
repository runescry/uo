# Summoning Magic - Underwater/Multi-Regional School

| Property | Value |
|----------|-------|
| **Class** | Summoner |
| **Region** | Underwater/Various |
| **Theme** | Creature summoning, pet buffs, elemental allies, binding |
| **Spellbook** | Codex of Binding (SummonerSpellbook) |
| **Hue** | 0x555 (Aqua Blue) |
| **Spell IDs** | 1256-1287 |
| **Status** | 100% Complete (32/32 spells) |

---

## Reagents

All Summoning Magic spells use Vystia reagents found in underwater depths.

| Reagent | Item ID | Circles | Description |
|---------|---------|---------|-------------|
| Planar Dust | 0x0F8F | 1-3 | Dust from other planes |
| Ether Shard | 0x0F8A | 2-4 | Shard from ethereal realm |
| Aether Shard | 0x0F7D | 3-5 | Shard from aether plane |
| Summoning Crystal | 0x0F8E | 4-6 | Crystal for summoning |
| Chaos Shard | 0x0DE1 | 5-7 | Shard of chaos realm |
| Binding Rune | 0x0F86 | 6-8 | Rune for binding summoned |
| Dimensional Key | 0x0F7A | 7-8 | Key to other dimensions |
| Summoning Salt | 0x1422 | 8 | Salt for summoning circles |

---

## Circle 1 - Basic Summons (4 Mana)

### 1. Summon Rabbit

| Property | Value |
|----------|-------|
| **File** | SummoningSummonRabbitSpell.cs |
| **Words** | "Summonum Rabbitum" |
| **Reagents** | Planar Dust, Ether Shard |
| **Followers** | 2 slots |
| **Duration** | 10 minutes |

**Effect:**
- Summons harmless scout rabbit
- Fast movement, can detect hidden

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Summon | 0x215 | - | Summon portal effect |
| Creature | Rabbit | - | Standard rabbit mobile |

---

### 2. Arcane Bolt

| Property | Value |
|----------|-------|
| **File** | SummoningArcaneBoltSpell.cs |
| **Words** | "Arcaneus Sagitta" |
| **Reagents** | Planar Dust, Ether Shard |
| **Range** | 10 tiles |
| **Target** | Single enemy |
| **Damage** | 4-10 energy |

**Effect:**
- Basic ranged attack spell
- 100% energy damage

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Energy particle effect |
| Sound | 0x1F2 | - | Magic bolt sound |

---

### 3. Empower Summon

| Property | Value |
|----------|-------|
| **File** | SummoningEmpowerSummonSpell.cs |
| **Words** | "Empowerum Summonum" |
| **Reagents** | Planar Dust, Ether Shard |
| **Range** | 10 tiles |
| **Target** | Single summon |
| **Duration** | 3 minutes |

**Effect:**
- **Placeholder:** +10 STR buff (final: +5 damage)
- Affects one controlled summon

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Power-up particles |
| Sound | 0x1F2 | - | Buff sound |

---

### 4. Summon Wisp

| Property | Value |
|----------|-------|
| **File** | SummoningSummonWispSpell.cs |
| **Words** | "Summonum Wispum" |
| **Reagents** | Planar Dust, Ether Shard |
| **Followers** | 2 slots |
| **Duration** | 10 minutes |

**Effect:**
- Summons wisp (light source)
- Reveals area, detects hidden

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Summon | 0x215 | - | Summon portal effect |
| Creature | Wisp | - | Wisp mobile |

---

## Circle 2 - Combat Pets (6 Mana)

### 5. Summon Wolf

| Property | Value |
|----------|-------|
| **File** | SummoningSummonWolfSpell.cs |
| **Words** | "Summonum Wolfum" |
| **Reagents** | Ether Shard, Aether Shard |
| **Followers** | 2 slots |
| **Duration** | 10 minutes |

**Effect:**
- Summons combat wolf
- 250 HP, fast attacks

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Summon | 0x215 | - | Summon portal effect |
| Creature | GreyWolf | - | Wolf mobile |

---

### 6. Summon Fire Sprite

| Property | Value |
|----------|-------|
| **File** | SummoningSummonFireSpriteSpell.cs |
| **Words** | "Summonum Firespriteum" |
| **Reagents** | Ether Shard, Aether Shard |
| **Followers** | 2 slots |
| **Duration** | 10 minutes |

**Effect:**
- Summons fire sprite elemental
- 200 HP, ranged fire damage

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Summon | 0x215 | - | Summon portal effect |
| Creature | FireElemental | - | Fire sprite mobile |

---

### 7. Mend Summon

| Property | Value |
|----------|-------|
| **File** | SummoningMendSummonSpell.cs |
| **Words** | "Mendem Summonum" |
| **Reagents** | Ether Shard, Aether Shard |
| **Range** | 10 tiles |
| **Target** | Single summon |
| **Healing** | 30-50 HP |

**Effect:**
- **Placeholder:** +10 STR buff (final: heal 30-50 HP)
- Heals one controlled summon

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Healing particles |
| Sound | 0x1F2 | - | Healing sound |

---

### 8. Summon Shield

| Property | Value |
|----------|-------|
| **File** | SummoningSummonShieldSpell.cs |
| **Words** | "Summonum Shieldum" |
| **Reagents** | Ether Shard, Aether Shard |
| **Range** | 10 tiles |
| **Target** | Single summon |
| **Duration** | 3 minutes |

**Effect:**
- **Placeholder:** +10 STR buff (final: 40 damage shield)
- Summon absorbs next 40 damage

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Shield particles |
| Sound | 0x1F2 | - | Shield sound |

---

## Circle 3 - Stronger Pets (9 Mana)

### 9. Summon Bear

| Property | Value |
|----------|-------|
| **File** | SummoningSummonBearSpell.cs |
| **Words** | "Summonum Bearum" |
| **Reagents** | Aether Shard, Summoning Crystal |
| **Followers** | 2 slots |
| **Duration** | 10 minutes |

**Effect:**
- Summons combat bear (tank)
- 500 HP, high damage

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Summon | 0x215 | - | Summon portal effect |
| Creature | GrizzlyBear | - | Bear mobile |

---

### 10. Summon Air Elemental

| Property | Value |
|----------|-------|
| **File** | SummoningSummonAirElementalSpell.cs |
| **Words** | "Summonum Airelementalum" |
| **Reagents** | Aether Shard, Summoning Crystal |
| **Followers** | 2 slots |
| **Duration** | 10 minutes |

**Effect:**
- Summons air elemental
- 350 HP, lightning attacks, flying

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Summon | 0x215 | - | Summon portal effect |
| Creature | AirElemental | - | Air elemental mobile |

---

### 11. Mass Empower

| Property | Value |
|----------|-------|
| **File** | SummoningMassEmpowerSpell.cs |
| **Words** | "Massum Empowerum" |
| **Reagents** | Aether Shard, Summoning Crystal |
| **Range** | 10 tiles |
| **Target** | Single target (should be all summons) |
| **Duration** | 3 minutes |

**Effect:**
- **Placeholder:** +10 STR buff + 0.2 per Conjuration (final: all summons +10 damage for 45s)
- Currently targets single mobile instead of all summons

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Power particles |
| Sound | 0x1F2 | - | Mass buff sound |

---

### 12. Bind Beast

| Property | Value |
|----------|-------|
| **File** | SummoningBindBeastSpell.cs |
| **Words** | "Bindum Beastum" |
| **Reagents** | Aether Shard, Summoning Crystal |
| **Range** | 10 tiles |
| **Target** | Single enemy beast |
| **Duration** | 3 minutes |

**Effect:**
- **Placeholder:** +10 STR buff (final: charm beast enemy to fight for you for 60s)
- Currently just buffs target

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Binding particles |
| Sound | 0x1F2 | - | Charm sound |

---

## Circle 4 - Advanced Summons (11 Mana)

### 13. Summon Drake

| Property | Value |
|----------|-------|
| **File** | SummoningSummonDrakeSpell.cs |
| **Words** | "Summonum Drakeum" |
| **Reagents** | Summoning Crystal, Chaos Shard |
| **Followers** | 2 slots |
| **Duration** | 10 minutes |

**Effect:**
- Summons drake (dragon-kin)
- 600 HP, breath attack

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Summon | 0x215 | - | Summon portal effect |
| Creature | Drake | - | Drake mobile |

---

### 14. Summon Earth Elemental

| Property | Value |
|----------|-------|
| **File** | SummoningSummonEarthElementalSpell.cs |
| **Words** | "Summonum Earthelementalum" |
| **Reagents** | Summoning Crystal, Chaos Shard |
| **Followers** | 2 slots |
| **Duration** | 10 minutes |

**Effect:**
- Summons earth elemental (tank)
- 800 HP, high armor

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Summon | 0x215 | - | Summon portal effect |
| Creature | EarthElemental | - | Earth elemental mobile |

---

### 15. Summon Frenzy

| Property | Value |
|----------|-------|
| **File** | SummoningSummonFrenzySpell.cs |
| **Words** | "Summonum Frenzyum" |
| **Reagents** | Summoning Crystal, Chaos Shard |
| **Range** | 10 tiles |
| **Target** | Single target (should be all summons) |
| **Duration** | 3 minutes |

**Effect:**
- **Placeholder:** +10 STR buff (final: all summons +40% attack speed for 20s)
- Currently targets single mobile

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Frenzy particles |
| Sound | 0x1F2 | - | Attack speed buff |

---

### 16. Unsummon

| Property | Value |
|----------|-------|
| **File** | SummoningUnsummonSpell.cs |
| **Words** | "Unsummonum Summonum" |
| **Reagents** | Summoning Crystal, Chaos Shard |
| **Range** | 10 tiles |
| **Target** | Single summon |

**Effect:**
- **Placeholder:** +10 STR buff (final: dismiss summon, restore 50% mana cost)
- Currently just buffs target

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Dismissal particles |
| Sound | 0x1F2 | - | Unsummon sound |

---

## Circle 5 - Elite Pets (14 Mana)

### 17. Summon Hydra

| Property | Value |
|----------|-------|
| **File** | SummoningSummonHydraSpell.cs |
| **Words** | "Summonum Hydraum" |
| **Reagents** | Chaos Shard, Binding Rune |
| **Followers** | 2 slots |
| **Duration** | 10 minutes |

**Effect:**
- Summons hydra
- 900 HP, multi-headed, triple attack

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Summon | 0x215 | - | Summon portal effect |
| Creature | SeaSerpent | - | Hydra placeholder (serpent) |

---

### 18. Summon Storm Elemental

| Property | Value |
|----------|-------|
| **File** | SummoningSummonStormElementalSpell.cs |
| **Words** | "Summonum Stormelementalum" |
| **Reagents** | Chaos Shard, Binding Rune |
| **Followers** | 2 slots |
| **Duration** | 10 minutes |

**Effect:**
- Summons storm elemental
- 650 HP, AoE lightning

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Summon | 0x215 | - | Summon portal effect |
| Creature | SnowElemental | - | Storm elemental placeholder |

---

### 19. Greater Heal Summon

| Property | Value |
|----------|-------|
| **File** | SummoningGreaterHealSummonSpell.cs |
| **Words** | "Greaterum Healsum Summonum" |
| **Reagents** | Chaos Shard, Binding Rune |
| **Range** | 10 tiles |
| **Target** | Single summon |
| **Healing** | 80-120 HP |

**Effect:**
- **Placeholder:** +10 STR buff (final: heal summon 80-120 HP)
- Heals one controlled summon

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Greater heal particles |
| Sound | 0x1F2 | - | Greater heal sound |

---

### 20. Symbiotic Link

| Property | Value |
|----------|-------|
| **File** | SummoningSymbioticLinkSpell.cs |
| **Words** | "Symbioticum Linkum" |
| **Reagents** | Chaos Shard, Binding Rune |
| **Range** | 10 tiles |
| **Target** | Single summon |
| **Duration** | 3 minutes |

**Effect:**
- **Placeholder:** +10 STR buff (final: link to summon, share HP/mana pools)
- Currently just buffs target

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Link particles |
| Sound | 0x1F2 | - | Link sound |

---

## Circle 6 - Legendary Pets (20 Mana)

### 21. Summon Phoenix

| Property | Value |
|----------|-------|
| **File** | SummoningSummonPhoenixSpell.cs |
| **Words** | "Summonum Phoenixum" |
| **Reagents** | Binding Rune, Dimensional Key |
| **Followers** | 2 slots |
| **Duration** | 10 minutes |

**Effect:**
- Summons phoenix (fire bird)
- 1000 HP, resurrects once when killed (not implemented)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Summon | 0x215 | - | Summon portal effect |
| Creature | Phoenix | - | Phoenix mobile |

---

### 22. Summon Void Elemental

| Property | Value |
|----------|-------|
| **File** | SummoningSummonVoidElementalSpell.cs |
| **Words** | "Summonum Voidelementalum" |
| **Reagents** | Binding Rune, Dimensional Key |
| **Followers** | 2 slots |
| **Duration** | 10 minutes |

**Effect:**
- Summons void elemental
- 850 HP, shadow damage, life drain

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Summon | 0x215 | - | Summon portal effect |
| Creature | PoisonElemental | - | Void elemental placeholder |

---

### 23. Army of Beasts

| Property | Value |
|----------|-------|
| **File** | SummoningArmyofBeastsSpell.cs |
| **Words** | "Armyum Ofum Beastseum" |
| **Reagents** | Binding Rune, Dimensional Key |
| **Followers** | 2 slots (should be 10 total) |
| **Duration** | 10 minutes |

**Effect:**
- **Partial:** Summons 1 wolf (final: 3 wolves + 2 bears)
- Currently only summons single wolf

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Summon | 0x215 | - | Summon portal effect |
| Creature | GreyWolf | - | Wolf mobile (x5 planned) |

---

### 24. Mass Heal Summons

| Property | Value |
|----------|-------|
| **File** | SummoningMassHealSummonsSpell.cs |
| **Words** | "Massum Healum Summonseum" |
| **Reagents** | Binding Rune, Dimensional Key |
| **Range** | 10 tiles |
| **Target** | Single target (should be all summons) |
| **Healing** | 50-80 HP each |

**Effect:**
- **Placeholder:** +10 STR buff (final: heal all summons 50-80 HP each)
- Currently targets single mobile

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Mass heal particles |
| Sound | 0x1F2 | - | Mass heal sound |

---

## Circle 7 - Master Summons (40 Mana)

### 25. Summon Greater Dragon

| Property | Value |
|----------|-------|
| **File** | SummoningSummonGreaterDragonSpell.cs |
| **Words** | "Summonum Greaterdragonum" |
| **Reagents** | Dimensional Key, Summoning Salt |
| **Followers** | 2 slots |
| **Duration** | 10 minutes |

**Effect:**
- Summons greater dragon
- 2000 HP, breath attack, flying

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Summon | 0x215 | - | Summon portal effect |
| Creature | Dragon | - | Greater dragon mobile |

---

### 26. Summon Elemental Lord

| Property | Value |
|----------|-------|
| **File** | SummoningSummonElementalLordSpell.cs |
| **Words** | "Summonum Elementallordum" |
| **Reagents** | Dimensional Key, Summoning Salt |
| **Followers** | 2 slots |
| **Duration** | 10 minutes |

**Effect:**
- Summons elemental lord
- 1800 HP, legendary elemental (element choice not implemented)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Summon | 0x215 | - | Summon portal effect |
| Creature | Balron | - | Elemental lord placeholder |

---

### 27. Sacrifice Summon

| Property | Value |
|----------|-------|
| **File** | SummoningSacrificeSummonSpell.cs |
| **Words** | "Sacrificeum Summonum" |
| **Reagents** | Dimensional Key, Summoning Salt |
| **Range** | 10 tiles |
| **Target** | Single summon |
| **Damage** | 100-180 AoE |

**Effect:**
- **Placeholder:** +10 STR buff (final: kill summon, massive 6-tile radius AoE damage)
- Currently just buffs target

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Explosion particles |
| Sound | 0x1F2 | - | Explosion sound |

---

### 28. Swarm of Creatures

| Property | Value |
|----------|-------|
| **File** | SummoningSwarmofCreaturesSpell.cs |
| **Words** | "Swarmum Ofum Creatureseum" |
| **Reagents** | Dimensional Key, Summoning Salt |
| **Followers** | 2 slots (should be 20 total) |
| **Duration** | 10 minutes |

**Effect:**
- **Partial:** Summons 1 rabbit (final: 10 weak creatures)
- Currently only summons single rabbit

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Summon | 0x215 | - | Summon portal effect |
| Creature | Rabbit | - | Swarm creature (x10 planned) |

---

## Circle 8 - Grandmaster Summons (50 Mana)

### 29. Summon Titan

| Property | Value |
|----------|-------|
| **File** | SummoningSummonTitanSpell.cs |
| **Words** | "Summonum Titanum" |
| **Reagents** | Dimensional Key, Summoning Salt (x2) |
| **Followers** | 2 slots |
| **Duration** | 10 minutes |

**Effect:**
- Summons titan (colossal giant)
- 3500 HP, AoE stomp, knockback

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Summon | 0x215 | - | Summon portal effect |
| Creature | Titan | - | Titan mobile |

---

### 30. Planar Convergence

| Property | Value |
|----------|-------|
| **File** | SummoningPlanarConvergenceSpell.cs |
| **Words** | "Planareum Convergenceum" |
| **Reagents** | Dimensional Key, Summoning Salt (x2) |
| **Followers** | 2 slots (should be 10 total) |
| **Duration** | 10 minutes |

**Effect:**
- **Partial:** Summons 1 elemental (final: one of each element - 5 total)
- Currently only summons single energy elemental

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Summon | 0x215 | - | Summon portal effect |
| Creature | EnergyVortex | - | Elemental placeholder (x5 planned) |

---

### 31. Summoner's Apocalypse

| Property | Value |
|----------|-------|
| **File** | SummoningSummonersApocalypseSpell.cs |
| **Words** | "Summonersum Apocalypseum" |
| **Reagents** | Dimensional Key, Summoning Salt (x2) |
| **Range** | All summons |
| **Damage** | 80-140 each |

**Effect:**
- **Placeholder:** Summons 1 drake (final: all summons explode for 80-140 damage each, then resummon at full HP)
- Currently just summons single drake

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Summon | 0x215 | - | Summon portal effect |
| Creature | Drake | - | Drake placeholder |

---

### 32. Avatar of Summoning

| Property | Value |
|----------|-------|
| **File** | SummoningAvatarofSummoningSpell.cs |
| **Words** | "Avatarum Ofum Summoningum" |
| **Reagents** | Dimensional Key, Summoning Salt (x2) |
| **Followers** | 2 slots (should be unlimited) |
| **Duration** | 10 minutes |

**Effect:**
- **Partial:** Summons 1 wolf (final: infinite summons, instant cast, invulnerable summons)
- Currently only summons single wolf

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Summon | 0x215 | - | Summon portal effect |
| Creature | GreyWolf | - | Avatar form placeholder |

---

## Known Issues

### Placeholder Implementations (21 spells)

The following spells are currently implemented as placeholder STR buffs instead of their intended mechanics:

**Circle 1:**
- Empower Summon (should: +5 damage to summon)

**Circle 2:**
- Mend Summon (should: heal 30-50 HP)
- Summon Shield (should: 40 damage shield)

**Circle 3:**
- Mass Empower (should: all summons +10 damage for 45s, currently: single target buff)
- Bind Beast (should: charm beast enemy for 60s)

**Circle 4:**
- Summon Frenzy (should: all summons +40% attack speed)
- Unsummon (should: dismiss summon, restore 50% mana)

**Circle 5:**
- Greater Heal Summon (should: heal 80-120 HP)
- Symbiotic Link (should: share HP/mana pools)

**Circle 6:**
- Army of Beasts (should: summon 3 wolves + 2 bears, currently: 1 wolf)
- Mass Heal Summons (should: heal all summons 50-80 HP each, currently: single target buff)

**Circle 7:**
- Sacrifice Summon (should: kill summon for 100-180 AoE damage)
- Swarm of Creatures (should: summon 10 weak creatures, currently: 1 rabbit)

**Circle 8:**
- Planar Convergence (should: summon 5 elementals, currently: 1)
- Summoner's Apocalypse (should: explode all summons then resummon, currently: summons 1 drake)
- Avatar of Summoning (should: infinite summons + instant cast + invulnerability, currently: 1 wolf)

### Missing Features

1. **Summon Management System:** No max summon cap enforcement (should be 5)
2. **Phoenix Resurrection:** Phoenix doesn't resurrect on death
3. **Element Selection:** Elemental Lord doesn't allow element choice
4. **Multi-Summon Spells:** Army, Swarm, Planar Convergence, Apocalypse only summon 1 creature
5. **Summon Tracking:** No system to track/manage multiple summons per player
6. **Mana Refund:** Unsummon doesn't return mana
7. **AoE Effects:** Sacrifice Summon missing explosion/damage implementation

### Target Type Issues

Several spells use `TargetFlags.Harmful` when they should be `TargetFlags.Beneficial`:
- Mass Empower (line 88)
- Summon Frenzy (similar issue)
- Mass Heal Summons (similar issue)

---

## File Locations

**Implementation Files:**
- `ServUO/Scripts/Custom/VystiaClasses/Spells/Summoner/` (32 spell files)

**Reagent File:**
- `ServUO/Scripts/Items/Vystia/Resources/Reagents/SummoningMagicReagents.cs`

**Spellbook File:**
- `ServUO/Scripts/Items/Equipment/Spellbooks/VystiaSpellbooks.cs` (SummonerSpellbook)

**Scroll File:**
- `ServUO/Scripts/Items/Vystia/Scrolls/SummoningScrolls.cs` (32 scrolls)

---

## Development Notes

### Summon Categories

**Beasts:**
- Rabbit, Wolf, Bear, Drake, Hydra, Phoenix

**Elementals:**
- Fire Sprite, Air, Earth, Storm, Void, Elemental Lord

**Legendary:**
- Greater Dragon, Titan, Planar beings

### Required Custom Mechanics

**1. Summon Manager System:**
```csharp
public static class SummonerManager
{
    private static Dictionary<Mobile, List<BaseCreature>> m_Summons;
    public static int MAX_SUMMONS = 5;

    public static bool CanSummon(Mobile m);
    public static void AddSummon(Mobile m, BaseCreature summon);
    public static List<BaseCreature> GetSummons(Mobile m);
}
```

**2. Mass Buff System:**
```csharp
List<BaseCreature> summons = SummonerManager.GetSummons(Caster);
foreach (BaseCreature summon in summons)
{
    summon.AddStatMod(new StatMod(StatType.Str, "Empower", 10, TimeSpan.FromSeconds(45.0)));
}
```

**3. Phoenix Resurrection:**
- Override `OnDeath()` in Phoenix mobile
- Track `m_HasResurrected` flag
- Timer.DelayCall(3s) to resurrect at full HP

**4. Sacrifice Explosion:**
- Get summon location before deletion
- IPooledEnumerable for 6-tile radius AoE
- 100-180 damage to all valid targets

---

**Last Updated:** 2025-12-28
**Status:** 100% Implemented (32/32 spells) - 66% Placeholders (21/32 spells need full implementation)
