# Shamanic Magic - Wilderlands School

| Property | Value |
|----------|-------|
| **Class** | Shaman |
| **Region** | Wilderlands / Multi-Regional |
| **Theme** | Totems, spirits, elementals, chain lightning, hybrid combat |
| **Spellbook** | Tome of Spirits (ShamanSpellbook) |
| **Hue** | 0x6E0 (Earthy Brown/Blue) |
| **Spell IDs** | 1256-1287 |
| **Status** | 100% Complete (32/32 spells implemented) |

---

## Reagents

All Shamanic Magic spells use Vystia reagents found in Skyreach peaks.

| Reagent | Item ID | Circles | Description |
|---------|---------|---------|-------------|
| LightningRoot | 0x0F86 | 1-3 | Root struck by lightning |
| ThunderMoss | 0x0F7B | 2-4 | Moss from storm clouds |
| StormCrystal | 0x0F8E | 3-5 | Crystal from storm |
| StormEssence | 0x1C18 | 4-6 | Essence of tempest |
| SpiritFeather | 0x0F78 | 5-7 | Feather from spirit animal |
| PrimalThunder | 0x0F8A | 6-8 | Primal thunder essence |
| TotemCarving | 0x0DE1 | 7-8 | Sacred totem wood |
| WindEssence | 0x0F0E | 8 | Essence of wind itself |

---

## Circle 1 - Basic Shamanic (4 Mana)

### 1. Lightning Bolt (ID: 1256)

| Property | Value |
|----------|-------|
| **File** | ShamanicLightningBoltSpell.cs |
| **Words** | "Lightningum Sagitta" |
| **Reagents** | LightningRoot, ThunderMoss |
| **Range** | 10 tiles |
| **Target** | Single enemy |
| **Damage** | 5-10 energy + (Conjuration * 0.5) |

**Effect:**
- Basic ranged lightning damage
- Scales with Conjuration skill

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Lightning strike |
| Sound | 0x1F2 | - | Thunder crack |

---

### 2. Strength Totem (ID: 1257)

| Property | Value |
|----------|-------|
| **File** | ShamanicStrengthTotemSpell.cs |
| **Words** | "Strengthum Totemum" |
| **Reagents** | LightningRoot, ThunderMoss |
| **Range** | 10 tiles |
| **Target** | Ground (totem placement) |
| **Duration** | 1 minute |

**Effect:**
- Applies +5 STR buff (scaled by Conjuration * 0.2) to target
- Currently implemented as direct single-target buff
- **Note:** Placeholder - should place totem item with AoE buff

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Blue shimmer |
| Sound | 0x1F2 | - | Totem placement |

---

### 3. Ghost Wolf Form (ID: 1258)

| Property | Value |
|----------|-------|
| **File** | ShamanicGhostWolfFormSpell.cs |
| **Words** | "Ghostum Wolfum" |
| **Reagents** | LightningRoot, ThunderMoss |
| **Range** | Self |
| **Target** | Caster |
| **Duration** | Variable |

**Effect:**
- Applies STR buff (placeholder)
- **Note:** Should transform into wolf form (+20 DEX, +25% speed)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Transformation effect |
| Sound | 0x1F2 | - | Spirit howl |

---

### 4. Healing Stream (ID: 1259)

| Property | Value |
|----------|-------|
| **File** | ShamanicHealingStreamSpell.cs |
| **Words** | "Healingum Streamum" |
| **Reagents** | LightningRoot, ThunderMoss |
| **Range** | 10 tiles |
| **Target** | Ground (totem placement) |

**Effect:**
- Applies STR buff (placeholder)
- **Note:** Should summon totem that heals allies 3-5 HP/tick

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Blue shimmer |
| Sound | 0x1F2 | - | Healing stream |

---

## Circle 2 - Elemental Arts (6 Mana)

### 5. Chain Lightning (ID: 1260)

| Property | Value |
|----------|-------|
| **File** | ShamanicChainLightningSpell.cs |
| **Words** | "Chainum Lightningum" |
| **Reagents** | ThunderMoss, StormCrystal |
| **Range** | 10 tiles |
| **Target** | Single enemy |
| **Damage** | 12-20 energy |

**Effect:**
- Direct damage to single target
- **Note:** Should bounce to 2 additional targets

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Lightning strike |
| Sound | 0x1F2 | - | Thunder crack |

---

### 6. Fire Totem (ID: 1261)

| Property | Value |
|----------|-------|
| **File** | ShamanicFireTotemSpell.cs |
| **Words** | "Fireum Totemum" |
| **Reagents** | ThunderMoss, StormCrystal |
| **Range** | 10 tiles |
| **Target** | Ground |

**Effect:**
- STR buff (placeholder)
- **Note:** Should place totem that auto-attacks enemies for 5-10 fire damage

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Totem placement |
| Sound | 0x1F2 | - | Fire crackle |

---

### 7. Spirit Strike (ID: 1262)

| Property | Value |
|----------|-------|
| **File** | ShamanicSpiritStrikeSpell.cs |
| **Words** | "Spiritum Strikeum" |
| **Reagents** | ThunderMoss, StormCrystal |
| **Range** | 10 tiles |
| **Target** | Single enemy |
| **Damage** | 10-18 energy |

**Effect:**
- Direct damage
- **Note:** Should be melee attack with knockback

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Spirit impact |
| Sound | 0x1F2 | - | Strike sound |

---

### 8. Purification (ID: 1263)

| Property | Value |
|----------|-------|
| **File** | ShamanicPurificationSpell.cs |
| **Words** | "Purificationum" |
| **Reagents** | ThunderMoss, StormCrystal |
| **Range** | 10 tiles |
| **Target** | Self or ally |

**Effect:**
- STR buff (placeholder)
- **Note:** Should remove poison, disease, curses

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Cleansing light |
| Sound | 0x1F2 | - | Purification chime |

---

## Circle 3 - Totem Mastery (9 Mana)

### 9. Lightning Storm (ID: 1264)

| Property | Value |
|----------|-------|
| **File** | ShamanicLightningStormSpell.cs |
| **Words** | "Lightningum Stormum" |
| **Reagents** | StormCrystal, StormEssence |
| **Range** | 10 tiles |
| **Target** | Ground (AoE center) |
| **Damage** | 15-28 energy |

**Effect:**
- Direct damage to single target
- **Note:** Should be 4 tile radius AoE

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Lightning strikes |
| Sound | 0x1F2 | - | Thunder roar |

---

### 10. Earth Shield (ID: 1265)

| Property | Value |
|----------|-------|
| **File** | ShamanicEarthShieldSpell.cs |
| **Words** | "Earthum Shieldum" |
| **Reagents** | StormCrystal, StormEssence |
| **Range** | 10 tiles |
| **Target** | Self or ally |

**Effect:**
- STR buff (placeholder)
- **Note:** Should create shield that absorbs 60 damage, regenerates 10/tick

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Earth barrier |
| Sound | 0x1F2 | - | Stone rumble |

---

### 11. Totemic Recall (ID: 1266)

| Property | Value |
|----------|-------|
| **File** | ShamanicTotemicRecallSpell.cs |
| **Words** | "Totemicum Recallum" |
| **Reagents** | StormCrystal, StormEssence |
| **Range** | Self |
| **Target** | Caster's totems |

**Effect:**
- STR buff (placeholder)
- **Note:** Should destroy all totems and restore 50% mana

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Spirit return |
| Sound | 0x1F2 | - | Totem destruction |

---

### 12. Summon Spirit Wolf (ID: 1267)

| Property | Value |
|----------|-------|
| **File** | ShamanicSummonSpiritWolfSpell.cs |
| **Words** | "Summonum Spiritum Wolfum" |
| **Reagents** | StormCrystal, StormEssence |
| **Range** | Self |
| **Target** | Ground near caster |
| **Duration** | 10 minutes |

**Effect:**
- Summons GreyWolf creature
- Uses 2 follower slots
- Requires follower space

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Summon | 0x1EA | - | Summoning effect |
| Sound | - | - | Wolf howl |

---

## Circle 4 - Spirit Healing (11 Mana)

### 13. Chain Heal (ID: 1268)

| Property | Value |
|----------|-------|
| **File** | ShamanicChainHealSpell.cs |
| **Words** | "Chainum Sanitas" |
| **Reagents** | StormEssence, SpiritFeather |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Healing** | 25-40 HP |

**Effect:**
- Heals single target
- **Note:** Should bounce to 3 allies

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Healing sparkles |
| Sound | 0x1F2 | - | Healing chime |

---

### 14. Mana Spring Totem (ID: 1269)

| Property | Value |
|----------|-------|
| **File** | ShamanicManaSpringTotemSpell.cs |
| **Words** | "Mana Springum Totemum" |
| **Reagents** | StormEssence, SpiritFeather |
| **Range** | 10 tiles |
| **Target** | Ground |

**Effect:**
- STR buff (placeholder)
- **Note:** Should place totem that restores 5 mana/tick to allies

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Mana spring |
| Sound | 0x1F2 | - | Water trickle |

---

### 15. Lava Burst (ID: 1270)

| Property | Value |
|----------|-------|
| **File** | ShamanicLavaBurstSpell.cs |
| **Words** | "Lavanum Burstum" |
| **Reagents** | StormEssence, SpiritFeather |
| **Range** | 10 tiles |
| **Target** | Single enemy |
| **Damage** | 30-50 fire |

**Effect:**
- Direct damage
- **Note:** Should be instant cast if Flame Shock is active on target

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Lava explosion |
| Sound | 0x1F2 | - | Lava burst |

---

### 16. Flame Shock (ID: 1271)

| Property | Value |
|----------|-------|
| **File** | ShamanicFlameShockSpell.cs |
| **Words** | "Flameum Shockum" |
| **Reagents** | StormEssence, SpiritFeather |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- STR buff (placeholder)
- **Note:** Should apply DoT (6-10 fire/tick), enables Lava Burst instant cast

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Flame ignition |
| Sound | 0x1F2 | - | Fire crackle |

---

## Circle 5 - Ancestral Power (14 Mana)

### 17. Thunderstorm Totem (ID: 1272)

| Property | Value |
|----------|-------|
| **File** | ShamanicThunderstormTotemSpell.cs |
| **Words** | "Thunderstormum Totemum" |
| **Reagents** | SpiritFeather, PrimalThunder |
| **Range** | 10 tiles |
| **Target** | Ground |

**Effect:**
- STR buff (placeholder)
- **Note:** Should place totem that casts chain lightning every 3s (8 tile range)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Totem placement |
| Sound | 0x1F2 | - | Thunder rumble |

---

### 18. Ancestral Spirit (ID: 1273)

| Property | Value |
|----------|-------|
| **File** | ShamanicAncestralSpiritSpell.cs |
| **Words** | "Ancestralum Spiritum" |
| **Reagents** | SpiritFeather, PrimalThunder |
| **Range** | Self |
| **Target** | Caster |
| **Duration** | 30 seconds |

**Effect:**
- STR buff (placeholder)
- **Note:** Should transform into spirit (+30% dodge, phase through walls)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Spirit transformation |
| Sound | 0x1F2 | - | Spirit whisper |

---

### 19. Earth Elemental (ID: 1274)

| Property | Value |
|----------|-------|
| **File** | ShamanicEarthElementalSpell.cs |
| **Words** | "Earthum Elementalum" |
| **Reagents** | SpiritFeather, PrimalThunder |
| **Range** | Self |
| **Target** | Ground near caster |

**Effect:**
- STR buff (placeholder)
- **Note:** Should summon Earth Elemental tank (1000 HP)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Earth rising |
| Sound | 0x1F2 | - | Stone rumble |

---

### 20. Maelstrom (ID: 1275)

| Property | Value |
|----------|-------|
| **File** | ShamanicMaelstromSpell.cs |
| **Words** | "Maelstromum" |
| **Reagents** | SpiritFeather, PrimalThunder |
| **Range** | 10 tiles |
| **Target** | Ground (AoE center) |
| **Damage** | 10-18 energy/tick |

**Effect:**
- Direct damage to single target
- **Note:** Should be ground AoE that pulls enemies in, deals damage per tick

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Swirling vortex |
| Sound | 0x1F2 | - | Wind vortex |

---

## Circle 6 - Elemental Fury (20 Mana)

### 21. Mega Chain Lightning (ID: 1276)

| Property | Value |
|----------|-------|
| **File** | ShamanicMegaChainLightningSpell.cs |
| **Words** | "Meganum Chainum Lightningum" |
| **Reagents** | PrimalThunder, TotemCarving |
| **Range** | 10 tiles |
| **Target** | Single enemy |
| **Damage** | 35-60 energy |

**Effect:**
- Direct damage
- **Note:** Should bounce to 5 targets

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Massive lightning |
| Sound | 0x1F2 | - | Thunder explosion |

---

### 22. Totem of Wrath (ID: 1277)

| Property | Value |
|----------|-------|
| **File** | ShamanicTotemofWrathSpell.cs |
| **Words** | "Totemum Wrathum" |
| **Reagents** | PrimalThunder, TotemCarving |
| **Range** | 10 tiles |
| **Target** | Ground |

**Effect:**
- STR buff (placeholder)
- **Note:** Should place totem that grants +20% spell damage to all allies

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Totem placement |
| Sound | 0x1F2 | - | War drums |

---

### 23. Spirit Link Totem (ID: 1278)

| Property | Value |
|----------|-------|
| **File** | ShamanicSpiritLinkTotemSpell.cs |
| **Words** | "Spiritum Linkum Totemum" |
| **Reagents** | PrimalThunder, TotemCarving |
| **Range** | 10 tiles |
| **Target** | Ground |

**Effect:**
- STR buff (placeholder)
- **Note:** Should place totem that splits damage among allies (damage reduction)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Totem placement |
| Sound | 0x1F2 | - | Spirit link |

---

### 24. Elemental Fury (ID: 1279)

| Property | Value |
|----------|-------|
| **File** | ShamanicElementalFurySpell.cs |
| **Words** | "Elementalum Furyum" |
| **Reagents** | PrimalThunder, TotemCarving |
| **Range** | Self |
| **Target** | Caster |
| **Duration** | 45 seconds |

**Effect:**
- STR buff (placeholder)
- **Note:** Should transform - allows casting spells while meleeing

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Elemental transformation |
| Sound | 0x1F2 | - | Power surge |

---

## Circle 7 - Legendary Totems (40 Mana)

### 25. Summon Greater Earth Elemental (ID: 1280)

| Property | Value |
|----------|-------|
| **File** | ShamanicSummonGreaterEarthElementalSpell.cs |
| **Words** | "Summonum Greaterum Earthum Elementalum" |
| **Reagents** | TotemCarving, WindEssence |
| **Range** | Self |
| **Target** | Ground near caster |

**Effect:**
- STR buff (placeholder)
- **Note:** Should summon elite elemental (2200 HP, AoE slam)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Earth eruption |
| Sound | 0x1F2 | - | Stone crash |

---

### 26. Ancestor's Blessing (ID: 1281)

| Property | Value |
|----------|-------|
| **File** | ShamanicAncestorsBlessingSpell.cs |
| **Words** | "Ancestorum Blessingum" |
| **Reagents** | TotemCarving, WindEssence |
| **Range** | 10 tiles |
| **Target** | Fallen ally |

**Effect:**
- STR buff (placeholder)
- **Note:** Should resurrect fallen ally at full HP/mana

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Ancestral light |
| Sound | 0x1F2 | - | Resurrection chime |

---

### 27. Four Totems (ID: 1282)

| Property | Value |
|----------|-------|
| **File** | ShamanicFourTotemsSpell.cs |
| **Words** | "Fourum Totemsium" |
| **Reagents** | TotemCarving, WindEssence |
| **Range** | Self |
| **Target** | Ground near caster |

**Effect:**
- STR buff (placeholder)
- **Note:** Should instantly place 4 totems (Fire, Earth, Water, Air)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Multiple totems appear |
| Sound | 0x1F2 | - | Quad placement |

---

### 28. Ascendance (ID: 1283)

| Property | Value |
|----------|-------|
| **File** | ShamanicAscendanceSpell.cs |
| **Words** | "Ascendanceum" |
| **Reagents** | TotemCarving, WindEssence |
| **Range** | Self |
| **Target** | Caster |
| **Duration** | Variable |

**Effect:**
- STR buff (placeholder)
- **Note:** Should transform into air (flying, +50% lightning damage, immune to melee)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Air transformation |
| Sound | 0x1F2 | - | Wind roar |

---

## Circle 8 - Shaman Lord (50 Mana)

### 29. Apocalyptic Chain Lightning (ID: 1284)

| Property | Value |
|----------|-------|
| **File** | ShamanicApocalypticChainLightningSpell.cs |
| **Words** | "Apocalypticanum Chainum Lightningum" |
| **Reagents** | TotemCarving, WindEssence (×2) |
| **Range** | 10 tiles |
| **Target** | Initial enemy |
| **Damage** | 80-140 energy |

**Effect:**
- Direct damage
- **Note:** Should bounce to ALL enemies in range

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Apocalyptic lightning |
| Sound | 0x1F2 | - | Thunder apocalypse |

---

### 30. Spirit of the Wild (ID: 1285)

| Property | Value |
|----------|-------|
| **File** | ShamanicSpiritoftheWildSpell.cs |
| **Words** | "Spiritum Wildus" |
| **Reagents** | TotemCarving, WindEssence (×2) |
| **Range** | Self |
| **Target** | Ground near caster |

**Effect:**
- STR buff (placeholder)
- **Note:** Should summon pack of 5 spirit wolves

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Spirit pack appears |
| Sound | 0x1F2 | - | Pack howl |

---

### 31. Totem Army (ID: 1286)

| Property | Value |
|----------|-------|
| **File** | ShamanicTotemArmySpell.cs |
| **Words** | "Totemum Armyum" |
| **Reagents** | TotemCarving, WindEssence (×2) |
| **Range** | Self |
| **Target** | Ground near caster |

**Effect:**
- STR buff (placeholder)
- **Note:** Should place 8 totems simultaneously, max totems increased to 8

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Army of totems |
| Sound | 0x1F2 | - | Mass placement |

---

### 32. Shaman Lord (ID: 1287)

| Property | Value |
|----------|-------|
| **File** | ShamanicShamanLordSpell.cs |
| **Words** | "Shamanum Lordum" |
| **Reagents** | TotemCarving, WindEssence (×2) |
| **Range** | 10 tiles |
| **Target** | Self or ally |
| **Duration** | 15 minutes |

**Effect:**
- Applies +25 STR buff (scaled by Conjuration * 0.2)
- **Note:** Should make totems indestructible, instant totem placement, elemental mastery

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Shaman Lord transformation |
| Sound | 0x1F2 | - | Ultimate power surge |

---

## Known Issues

### Placeholder Implementations
Most spells currently apply placeholder STR buffs instead of their designed effects:

1. **Totem System Not Implemented**
   - Totems should be placeable items with AoE effects
   - Current implementation: Direct single-target buffs
   - Affected: Strength Totem, Fire Totem, Healing Stream, Mana Spring Totem, Thunderstorm Totem, Totem of Wrath, Spirit Link Totem

2. **Chain Mechanics Not Implemented**
   - Chain Lightning should bounce to 2 additional targets
   - Chain Heal should bounce to 3 allies
   - Mega Chain Lightning should bounce to 5 targets
   - Apocalyptic Chain Lightning should bounce to ALL enemies

3. **Transformation Spells Not Implemented**
   - Ghost Wolf Form should change body, increase DEX/speed
   - Ancestral Spirit should grant dodge/phasing
   - Elemental Fury should enable hybrid combat
   - Ascendance should grant flight/immunity

4. **Summon Spells Partially Implemented**
   - Summon Spirit Wolf works (summons GreyWolf)
   - Earth Elemental not implemented (STR buff placeholder)
   - Greater Earth Elemental not implemented
   - Spirit of the Wild not implemented (5 wolves)

5. **Unique Mechanics Missing**
   - Flame Shock DoT not implemented
   - Lava Burst instant cast combo missing
   - Totemic Recall mana refund missing
   - Maelstrom pull effect missing
   - Four Totems multi-spawn missing
   - Totem Army 8-totem system missing

### Targeting Issues
- Many totem spells use harmful targeting instead of ground targeting
- Some spells should target ground but target entities

### Animation Placeholders
- All spells use same effect (0x376A) and sound (0x1F2)
- Need unique effects per spell type
- Totem placement needs visual item sprites

---

## File Locations

**Spell Files:** `ServUO/Scripts/Custom/VystiaClasses/Spells/Shaman/`

All 32 spell files follow naming convention: `Shamanic[SpellName]Spell.cs`

**Reagent File:** `ServUO/Scripts/Items/Vystia/Resources/Reagents/ShamanicMagicReagents.cs`

**Spellbook:** `ServUO/Scripts/Items/Equipment/Spellbooks/VystiaSpellbooks.cs` (ShamanSpellbook class)

**Spell Registration:** `ServUO/Scripts/Custom/VystiaClasses/Spells/VystiaSpellInitializer.cs` (Lines 351-382)

---

## Implementation Notes

### Totem System Design (Not Yet Implemented)

The totem system should use the following architecture:

```csharp
public abstract class TotemItem : Item
{
    protected Mobile m_Caster;
    protected Timer m_EffectTimer;
    protected int m_Duration = 60; // seconds

    public TotemItem(Mobile caster, int itemID) : base(itemID)
    {
        Movable = false;
        m_Caster = caster;

        m_EffectTimer = Timer.DelayCall(
            TimeSpan.FromSeconds(3.0),
            TimeSpan.FromSeconds(3.0),
            m_Duration / 3,
            OnTick
        );
        Timer.DelayCall(TimeSpan.FromSeconds(m_Duration), () => Delete());
    }

    protected abstract void OnTick();
}
```

### Chain Mechanic Design (Not Yet Implemented)

Chain spells should implement bounce logic:

```csharp
public void CastChainLightning(Mobile initialTarget, int bounces, int baseDamage)
{
    List<Mobile> hitTargets = new List<Mobile>();
    Mobile currentTarget = initialTarget;

    for (int i = 0; i < bounces && currentTarget != null; i++)
    {
        int damage = (int)(baseDamage * Math.Pow(0.9, i)); // -10% per bounce
        AOS.Damage(currentTarget, Caster, damage, 0, 0, 0, 0, 100);

        hitTargets.Add(currentTarget);
        currentTarget = FindNearestEnemy(currentTarget.Location, 5, hitTargets);
    }
}
```

---

**Last Updated:** 2025-12-28
**Status:** All spells implemented with placeholder effects - totem system, chain mechanics, transformations, and unique features need full implementation
