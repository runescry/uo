# BODY ART IMPLEMENTATION GUIDE FOR SERVUO

## Overview
This guide explains how to view creature art and implement new body types into ServUO.

---

## 1. VIEWING CREATURE ART

### Method 1: Using UO:Renaissance Client Files
The UO:Renaissance client contains the art files you need:

**Key Files:**
- `art.mul` - Contains the actual art data
- `artidx.mul` - Index file for art.mul
- `Arttable.cfg` - Maps art IDs to body IDs
- `mobtypes.txt` - Defines animation types and flags

**Art File Structure:**
```
Arttable.cfg format:
BodyID { ArtID } Flags
Example: 1 { 3 } -1
```

### Method 2: Using Art Viewing Tools
**Recommended Tools:**
1. **UOArt** - Standalone art viewer
2. **UOFiddler** - Comprehensive UO file editor
3. **ClassicUO** - Modern client with art viewing capabilities

**Installation:**
1. Download UOFiddler from GitHub
2. Point it to your UO:Renaissance folder
3. Open `art.mul` to browse creature art

### Method 3: Using ServUO's Built-in Art System
ServUO has an art system in `Ultima/Art.cs`:

```csharp
// Get art for body ID
Bitmap art = Ultima.Art.GetStatic(bodyID);

// Check if art exists
if (art != null)
{
    // Art exists for this body ID
}
```

---

## 2. BODY TYPE IMPLEMENTATION

### Step 1: Update bodyTable.cfg
Add new body types to `ServUO/Data/bodyTable.cfg`:

```
# Format: BodyID <tab> BodyType
# Types: Empty, Monster, Sea, Animal, Human, Equipment

# Example new entries:
1000	Monster	# Custom Dragon
1001	Animal	# Custom Wolf
1002	Sea	# Custom Sea Serpent
```

### Step 2: Create Creature Classes
Create new creature classes in `Scripts/Mobiles/Normal/`:

```csharp
using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a custom dragon corpse")]
    public class CustomDragon : BaseCreature
    {
        [Constructable]
        public CustomDragon() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a custom dragon";
            Body = 1000; // Your custom body ID
            BaseSoundID = 362; // Dragon sound
            
            SetStr(796, 825);
            SetDex(86, 105);
            SetInt(436, 475);
            
            SetHits(478, 495);
            SetDamage(16, 22);
            
            SetDamageType(ResistanceType.Physical, 100);
            
            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Poison, 25, 35);
            SetResistance(ResistanceType.Energy, 35, 45);
            
            SetSkill(SkillName.MagicResist, 25.1, 40.0);
            SetSkill(SkillName.Tactics, 35.1, 50.0);
            SetSkill(SkillName.Wrestling, 40.1, 60.0);
            
            Fame = 15000;
            Karma = -15000;
            
            VirtualArmor = 60;
            
            Tamable = false;
            ControlSlots = 3;
            MinTameSkill = 93.9;
        }
        
        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.Gems, 8);
        }
        
        public CustomDragon(Serial serial) : base(serial)
        {
        }
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }
        
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
```

### Step 3: Add Art Files (Optional)
If you have custom art files:

1. **Extract art from UO:Renaissance:**
   ```bash
   # Use UOFiddler to export art
   # Export body ID 1000 as CustomDragon.bmp
   ```

2. **Add to ServUO:**
   - Place art files in `Data/Art/`
   - Update `Ultima/Art.cs` to load custom art

### Step 4: Test Implementation
1. **Compile ServUO:**
   ```bash
   cd ServUO
   msbuild ServUO.sln
   ```

2. **Test in-game:**
   ```csharp
   // Spawn test creature
   [CommandProperty(AccessLevel.GameMaster)]
   public static void SpawnCustomDragon()
   {
       CustomDragon dragon = new CustomDragon();
       dragon.MoveToWorld(new Point3D(100, 100, 0), Map.Felucca);
   }
   ```

---

## 3. BODY TYPE COMPATIBILITY

### UO:Renaissance Compatible Body IDs
Based on your body types list, these are safe to use:

**Classic Body IDs (1-200):**
- 1-100: Most classic creatures
- 101-200: Classic animals and monsters

**Expansion Body IDs (200+):**
- 300-500: Third Dawn creatures
- 600-800: Samurai Empire creatures
- 1000+: High Seas creatures
- 1400+: Time of Legends creatures

### ServUO Current Usage
ServUO currently uses approximately **30-50 body IDs** from your list of **487 available**.

**Most Used Body IDs in ServUO:**
- 1 (Ogre), 4 (Gargoyle), 5 (Eagle), 6 (Bird)
- 12 (Dragon), 13 (Air Elemental), 14 (Earth Elemental)
- 23 (Wolf), 24 (Lich), 26 (Wraith), 30 (Harpy)
- 39 (Mongbat), 47 (Reaper), 48 (Scorpion)
- 53 (Troll), 57 (Skeletal Knight), 58 (Wisp)
- 76 (Titan), 87 (Ophidian Matriarch), 98 (Hell Hound)
- 150 (Sea Serpent), 154 (Mummy), 225 (Wolf Brown)

---

## 4. IMPLEMENTATION EXAMPLES

### Example 1: Adding a New Dragon Type
```csharp
// In Scripts/Mobiles/Normal/CustomDragon.cs
public class CustomDragon : BaseCreature
{
    public CustomDragon() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
    {
        Name = "a custom dragon";
        Body = 1000; // New body ID
        Hue = 1152; // Ice blue hue
        BaseSoundID = 362;
        
        // Set stats, resistances, skills, etc.
    }
}
```

### Example 2: Adding a New Animal
```csharp
// In Scripts/Mobiles/Normal/CustomWolf.cs
public class CustomWolf : BaseCreature
{
    public CustomWolf() : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
    {
        Name = "a custom wolf";
        Body = 1001; // New body ID
        Hue = 1109; // Shadow black hue
        BaseSoundID = 0xE5;
        
        // Set stats, resistances, skills, etc.
        Tamable = true;
        ControlSlots = 1;
        MinTameSkill = 83.1;
    }
}
```

### Example 3: Adding a New Sea Creature
```csharp
// In Scripts/Mobiles/Normal/CustomSeaSerpent.cs
public class CustomSeaSerpent : BaseCreature
{
    public CustomSeaSerpent() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
    {
        Name = "a custom sea serpent";
        Body = 1002; // New body ID
        Hue = 1367; // Deep blue hue
        BaseSoundID = 447;
        
        // Set stats, resistances, skills, etc.
        CanSwim = true;
        CantWalk = true;
    }
}
```

---

## 5. TROUBLESHOOTING

### Common Issues:

**1. Art Not Displaying:**
- Check if body ID exists in `Arttable.cfg`
- Verify art files are in correct location
- Ensure client has the art files

**2. Animation Issues:**
- Check `mobtypes.txt` for correct animation type
- Verify body type in `bodyTable.cfg`

**3. Compilation Errors:**
- Check for duplicate class names
- Verify all required using statements
- Ensure proper inheritance from BaseCreature

**4. Runtime Errors:**
- Check if body ID is within valid range
- Verify all required properties are set
- Test with simple creature first

---

## 6. BEST PRACTICES

### Art Selection:
1. **Use existing art when possible** - Reduces file size
2. **Test compatibility** - Ensure art works with UO:Renaissance
3. **Consistent theming** - Match art style to creature concept

### Code Organization:
1. **Group by region** - Organize creatures by biome/area
2. **Consistent naming** - Use clear, descriptive names
3. **Proper documentation** - Comment complex creatures

### Performance:
1. **Limit custom art** - Too many custom files slow loading
2. **Reuse body types** - Use different hues for variants
3. **Optimize stats** - Balance creature difficulty

---

## 7. RESOURCES

### Tools:
- **UOFiddler** - Art viewing and editing
- **ClassicUO** - Modern UO client
- **ServUO Forums** - Community support

### Documentation:
- **ServUO Wiki** - Official documentation
- **UO:Renaissance Forums** - Client-specific help
- **Body Types List** - Complete reference

### Files:
- `ServUO/Data/bodyTable.cfg` - Body type definitions
- `ServUO/Ultima/Art.cs` - Art loading system
- `UO:Renaissance/art.mul` - Art data file
- `UO:Renaissance/Arttable.cfg` - Art mapping

---

This guide provides everything needed to view creature art and implement new body types into ServUO. Start with simple creatures and gradually add more complex ones as you become familiar with the system.
