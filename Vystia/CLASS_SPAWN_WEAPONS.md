# Class Spawn Weapons - All 25 Classes

This document lists all class-specific starting weapons that spawn when a player is assigned to a class in Vystia v2.0.

## Complete Weapon List

| # | Class | Weapon Name | Item ID | Base Item | C# Class Name | Notes |
|---|-------|-------------|---------|-----------|---------------|-------|
| 1 | **Ice Mage** | Staff of Winter | `0x905` | Glass Staff | `GlassStaff` | Flipable: `0x4070` |
| 2 | **Warlock** | Staff of Shadows | `0xDF0` | Black Staff | `BlackStaff` | Flipable: `0xDF1` |
| 3 | **Necromancer** | Soul Reaper | `0x26BA` | Scythe | `VystiaScythe` | Flipable: `0x26C4` |
| 4 | **Druid** | Verdant Staff | `0x2D25` | Wild Staff | `VystiaWildStaff` | Flipable: `0x2D31` |
| 5 | **Sorcerer** | Staff of Elements | `0xE89` | Quarter Staff | `QuarterStaff` | Flipable: `0xE8A` |
| 6 | **Bard** | Enchanted Lute | `0xEB3` | Lute | `Lute` | Instrument |
| 7 | **Barbarian** | Frostborn Axe | `0x1443` | Two Handed Axe | `TwoHandedAxe` | Flipable: `0x1442` |
| 8 | **Rogue** | Shadow Blade | `0xF52` | Dagger | `Dagger` | Flipable: `0xF51`, spawns x2 |
| 9 | **Monk** | *(None - Fists)* | N/A | Fists | N/A | Unarmed combat |
| 10 | **Knight** | Knight's Blade | `0x13B9` | Viking Sword | `VikingSword` | Flipable: `0x13BA` |
| 11 | **Paladin** | Holy Lance | `0x26C0` | Lance | `VystiaLance` | Flipable: `0x26CA` |
| 12 | **Ranger** | Desert Longbow | `0x27A5` | Yumi | `VystiaYumi` | Flipable: `0x27F0` |
| 13 | **Witch** | Hexwood Staff | `0xE81` | Shepherd's Crook | `ShepherdsCrook` | Flipable: `0xE82` |
| 14 | **Oracle** | Crystal Staff | `0x905` | Glass Staff | `GlassStaff` | Flipable: `0x4070` |
| 15 | **Summoner** | Staff of Summoning | `0x906` | Serpent Stone Staff | `SerpentStoneStaff` | Flipable: `0x406F` |
| 16 | **Shaman** | Spirit Staff | `0xE81` | Shepherd's Crook | `ShepherdsCrook` | Flipable: `0xE82` |
| 17 | **Enchanter** | Runestave | `0x26BC` | Scepter | `Scepter` | Flipable: `0x26C6` |
| 18 | **Illusionist** | Staff of Illusions | `0xDF0` | Black Staff | `BlackStaff` | Flipable: `0xDF1` |
| 19 | **Fighter** | Fighter's Blade | `0x13B9` | Viking Sword | `VikingSword` | Flipable: `0x13BA` |
| 20 | **Templar** | Zealot's Blade | `0xF61` | Longsword | `Longsword` | Flipable: `0xF60` |
| 21 | **Bounty Hunter** | Hunter's Blade | `0x1401` | Kryss | `Kryss` | Flipable: `0x1400` |
| 21b | **Bounty Hunter** | Tracker's Crossbow | `0xF50` | Crossbow | `Crossbow` | Flipable: `0xF4F`, spawns with Kryss |
| 22 | **Beastmaster** | Hunter's Bow | `0x13B2` | Bow | `Bow` | Flipable: `0x13B1` |
| 23 | **Artificer** | Engineer's Wrench | `0xF5C` | Mace | `Mace` | Flipable: `0xF5D` |
| 24 | **Alchemist** | Mixing Rod | `0xF5C` | Mace | `Mace` | Flipable: `0xF5D` |
| 25 | **Cleric** | Staff of Healing | `0x26BC` | Scepter | `Scepter` | Flipable: `0x26C6` |
| 26 | **Wizard** | Arcane Staff | `0xE89` | Quarter Staff | `QuarterStaff` | Flipable: `0xE8A` |

## Class-Specific Vystia Weapons

These are custom Vystia weapon classes that extend base UO weapons:

| Class | Weapon Name | Base Item | C# Class | Location |
|-------|-------------|-----------|----------|----------|
| **Paladin** | Holy Lance | Lance | `Server.Items.Vystia.VystiaLance` | `RegionalWeapons.cs` |
| **Necromancer** | Soul Reaper | Scythe | `Server.Items.Vystia.VystiaScythe` | `RegionalWeapons.cs` |
| **Ranger** | Desert Longbow | Yumi | `Server.Items.Vystia.VystiaYumi` | `RegionalWeapons.cs` |
| **Druid** | Verdant Staff | Wild Staff | `Server.Items.Vystia.VystiaWildStaff` | `RegionalWeapons.cs` |

## Weapon Details by Class

### Magic Classes (6 Core)

1. **Ice Mage** - Staff of Winter
   - Base: Glass Staff (`0x905`)
   - Hue: 1152 (Ice Mage hue)
   - Hat: Floppy Hat

2. **Warlock** - Staff of Shadows
   - Base: Black Staff (`0xDF0`)
   - Hue: 1109 (Warlock hue)
   - Hat: Wide Brim Hat

3. **Necromancer** - Soul Reaper
   - Base: Scythe (`0x26BA`) - **Vystia Custom**
   - Hue: 1109 (Necromancer hue)
   - Stats: +10% Weapon Damage, +5% Hit Lower Defend

4. **Druid** - Verdant Staff
   - Base: Wild Staff (`0x2D25`) - **Vystia Custom**
   - Hue: 2010 (Druid hue)
   - Stats: +10% Weapon Damage, +5% Hit Lightning

5. **Sorcerer** - Staff of Elements
   - Base: Quarter Staff (`0xE89`)
   - Hue: 1358 (Sorcerer hue)
   - Hat: Floppy Hat

6. **Bard** - Enchanted Lute
   - Base: Lute (`0xEB3`)
   - Hue: 1153 (Bard hue)
   - Note: This is an instrument, not a weapon

### Martial Classes (6 Core)

7. **Barbarian** - Frostborn Axe
   - Base: Two Handed Axe (`0x1443`)
   - Hue: 1150 (Barbarian hue)

8. **Rogue** - Shadow Blade
   - Base: Dagger (`0xF52`)
   - Hue: 1109 (Rogue hue)
   - Note: Spawns **two** daggers

9. **Monk** - *(No Weapon)*
   - Uses unarmed combat (fists)
   - No weapon spawned

10. **Knight** - Knight's Blade
    - Base: Viking Sword (`0x13B9`)
    - Hue: 2305 (Knight hue)

11. **Paladin** - Holy Lance
    - Base: Lance (`0x26C0`) - **Vystia Custom**
    - Hue: 1153 (Paladin hue)
    - Stats: +10% Weapon Damage, +5% Hit Lightning

12. **Ranger** - Desert Longbow
    - Base: Yumi (`0x27A5`) - **Vystia Custom**
    - Hue: 1719 (Ranger hue)
    - Stats: +10% Weapon Damage, +5% Hit Lightning

### Extended Magic Classes (6 More)

13. **Witch** - Hexwood Staff
    - Base: Shepherd's Crook (`0xE81`)
    - Hue: 1109 (Witch hue)
    - Hat: Floppy Hat

14. **Oracle** - Crystal Staff
    - Base: Glass Staff (`0x905`)
    - Hue: 1154 (Oracle hue)
    - Hat: Wide Brim Hat

15. **Summoner** - Staff of Summoning
    - Base: Serpent Stone Staff (`0x906`)
    - Hue: 1152 (Summoner hue)
    - Hat: Floppy Hat

16. **Shaman** - Spirit Staff
    - Base: Shepherd's Crook (`0xE81`)
    - Hue: 2010 (Shaman hue)
    - Accessory: Body Sash

17. **Enchanter** - Runestave
    - Base: Scepter (`0x26BC`)
    - Hue: 1153 (Enchanter hue)
    - Hat: Wizards Hat

18. **Illusionist** - Staff of Illusions
    - Base: Black Staff (`0xDF0`)
    - Hue: 1719 (Illusionist hue)
    - Hat: Floppy Hat

### Extended Martial Classes (8 More)

19. **Fighter** - Fighter's Blade
    - Base: Viking Sword (`0x13B9`)
    - Hue: 2305 (Fighter hue)

20. **Templar** - Zealot's Blade
    - Base: Longsword (`0xF61`)
    - Hue: 1153 (Templar hue)

21. **Bounty Hunter** - Hunter's Blade & Tracker's Crossbow
    - Primary: Kryss (`0x1401`)
    - Secondary: Crossbow (`0xF50`)
    - Hue: 1719 (Bounty Hunter hue)
    - Note: Spawns **both** weapons

22. **Beastmaster** - Hunter's Bow
    - Base: Bow (`0x13B2`)
    - Hue: 1150 (Beastmaster hue)

23. **Artificer** - Engineer's Wrench
    - Base: Mace (`0xF5C`)
    - Hue: 2305 (Artificer hue)

24. **Alchemist** - Mixing Rod
    - Base: Mace (`0xF5C`)
    - Hue: 2010 (Alchemist hue)
    - Accessory: Half Apron

25. **Cleric** - Staff of Healing
    - Base: Scepter (`0x26BC`)
    - Hue: 1153 (Cleric hue)
    - Hat: Wide Brim Hat

26. **Wizard** - Arcane Staff
    - Base: Quarter Staff (`0xE89`)
    - Hue: 1154 (Wizard hue)
    - Hat: Wizards Hat

## Implementation

### File Locations

- **Class Definitions**: `ServUO/Scripts/Custom/VystiaClasses/Classes/PlayerClassV2.cs`
- **Vystia Custom Weapons**: `ServUO/Scripts/Items/Vystia/Equipment/Weapons/RegionalWeapons.cs`
- **Base Weapon Classes**: `ServUO/Scripts/Items/Equipment/Weapons/`

### Weapon Assignment

Weapons are assigned in the `EquipStartingGear()` method of each class. The method is called during class initialization via `InitializeClass()`.

### Notes

- **Monk** is the only class that spawns without a weapon (uses unarmed combat)
- **Rogue** spawns with **two** daggers (dual-wield)
- **Bounty Hunter** spawns with **two** weapons (Kryss and Crossbow)
- Most magic classes use **Gnarled Staff** (`0x13F8`) with different names and hues
- Only **4 classes** use custom Vystia weapon classes: Paladin, Necromancer, Ranger, and Druid
- **Bard** uses a Lute, which is an instrument rather than a traditional weapon
- All flipable weapons have two item IDs (male/female variants)

## Weapon Statistics

### Vystia Custom Weapons

All Vystia custom weapons include:
- +10% Weapon Damage
- Minor weapon attribute bonuses (varies by weapon)
- Class-appropriate hue
- Property indicating "Class Starting Weapon"

### Base Weapon Stats

Standard UO weapons use their default stats as defined in their base classes.
