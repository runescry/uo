# Creating Custom Sized Races and Creatures for Ultima Online

This guide documents how to create custom-sized races (like dwarves at 75% human size) for ServUO private shards with ClassicUO client.

## Overview

UO stores animations in `.mul` files with an index (`.idx`) file. Modern clients also use `.uop` files which take priority over `.mul` for certain body IDs. The key insight is that `mobtypes.txt` controls which file format the client reads from based on flags.

## Prerequisites

- Python 3.x with Pillow (`pip install Pillow`)
- ServUO server
- ClassicUO client
- UO Classic client files (anim.mul, anim.idx, mobtypes.txt)

## File Structure

```
Vystia/tools/
├── dwarf_sprite_creator.py   # Extracts & resizes sprites
├── dwarf_sprite_writer.py    # Writes sprites back to anim.mul
├── dwarf_sprites/            # Extracted PNG frames
│   ├── body_1600/           # Male dwarf frames
│   └── body_1601/           # Female dwarf frames
├── patched_client/          # Output patched files
└── backups/                 # Timestamped backups
```

## Step 1: Choose Target Body IDs

Find body IDs that:
1. Already exist in `anim.mul` (have animation data)
2. Have **low flags** in `mobtypes.txt` (not 10000+ which means UOP)
3. Are marked as `HUMAN` type for equipment support

Check mobtypes.txt:
```
987	HUMAN	10    # Good - flag 10 means MUL file
988	HUMAN	10    # Good - flag 10 means MUL file
400	HUMAN	20000 # Bad - flag 20000 means UOP file
```

We used body 987/988 which have flag `10` (reads from anim.mul, not UOP).

## Step 2: Extract and Resize Sprites

### dwarf_sprite_creator.py

Key configuration:
```python
# Source bodies (full-size humans)
SOURCE_MALE_BODY = 400
SOURCE_FEMALE_BODY = 401

# Output folder naming (intermediate storage)
TARGET_MALE_BODY = 1600
TARGET_FEMALE_BODY = 1601

# Scale factor
SCALE_FACTOR = 0.75  # 75% size for dwarves
```

### Critical: Alpha Thresholding

The original human sprites have semi-transparent edge pixels (anti-aliasing). These cause a "halo" artifact when converted to UO's format. The fix is to threshold alpha:

```python
def threshold_alpha(img, threshold=128):
    """Force pixels to fully transparent or fully opaque"""
    pixels = img.load()
    for y in range(img.height):
        for x in range(img.width):
            r, g, b, a = pixels[x, y]
            if a < threshold:
                pixels[x, y] = (0, 0, 0, 0)      # Fully transparent
            else:
                pixels[x, y] = (r, g, b, 255)   # Fully opaque
    return img
```

### Critical: Use NEAREST Resampling

For pixel art, use NEAREST neighbor (not LANCZOS) to preserve hard edges:

```python
resized = img.resize((new_width, new_height), Image.Resampling.NEAREST)
```

Run the creator:
```bash
python dwarf_sprite_creator.py
```

## Step 3: Write Sprites to anim.mul

### dwarf_sprite_writer.py

Key configuration:
```python
# Where to write in anim.mul
TARGET_MALE_BODY = 987
TARGET_FEMALE_BODY = 988

# Where to read PNGs from
SOURCE_MALE_BODY = 1600
SOURCE_FEMALE_BODY = 1601
```

### Critical: Write IN-PLACE

The client reads from the **original offset** stored in `anim.idx`. Appending new data to the end of `anim.mul` doesn't work - you must **overwrite in-place** at the original offset:

```python
def write_to_mul_files(client_path, body_id, all_frames):
    # Get original offset from idx
    orig_offset, orig_length = get_original_offset_and_length(idx_path, body_id, action, direction)

    # Write at original offset (not end of file)
    mul_file.seek(orig_offset)
    mul_file.write(anim_data)

    # Pad remaining space
    padding = orig_length - len(anim_data)
    if padding > 0:
        mul_file.write(b'\x00' * padding)
```

Run the writer:
```bash
python dwarf_sprite_writer.py
```

## Step 4: Create Scaled Equipment (Optional)

If your custom race needs equipment that matches their scale, you need to:
1. Extract equipment animations at the same scale
2. Write them to unused body IDs
3. Use equipconv.def to map equipment when worn by your custom body

### dwarf_equipment_creator.py

Extract and resize equipment sprites:
```python
SOURCE_EQUIPMENT = {
    # Male plate armor
    'plate_chest': 527,        # Platemail chest animation ID
    'plate_arms': 528,         # Platemail arms
    'plate_legs': 529,         # Platemail legs
    'plate_gloves': 530,       # Platemail gloves
    'warhammer': 646,          # Warhammer
    # Female leather armor
    'leather_tunic': 542,      # Leather tunic
    'leather_leggings': 543,   # Leather leggings
    'leather_sleeves': 544,    # Leather sleeves
    'leather_gloves': 545,     # Leather gloves
    'leather_gorget': 546,     # Leather gorget
}
SCALE_FACTOR = 0.75
```

### dwarf_equipment_writer.py

Write equipment to target body IDs:
```python
EQUIPMENT_MAPPING = {
    # Male plate armor (body IDs 909-913)
    'plate_chest_body_527': 909,       # Dwarf plate chest
    'plate_arms_body_528': 910,        # Dwarf plate arms
    'plate_legs_body_529': 911,        # Dwarf plate legs
    'plate_gloves_body_530': 912,      # Dwarf plate gloves
    'warhammer_body_646': 913,         # Dwarf warhammer
    # Female leather armor (body IDs 914-918)
    'leather_tunic_body_542': 914,     # Dwarf leather tunic
    'leather_leggings_body_543': 915,  # Dwarf leather leggings
    'leather_sleeves_body_544': 916,   # Dwarf leather sleeves
    'leather_gloves_body_545': 917,    # Dwarf leather gloves
    'leather_gorget_body_546': 918,    # Dwarf leather gorget
}
```

### equipconv.def (Client File)

Add entries to convert equipment for your body type:
```
# Format: bodyType  equipmentID  convertToID  GumpIDToUse  hue

# Dwarf Male (body 987) - Plate armor
987	527	909	0	0	# Plate Chest -> Dwarf Plate Chest
987	528	910	0	0	# Plate Arms -> Dwarf Plate Arms
987	529	911	0	0	# Plate Legs -> Dwarf Plate Legs
987	530	912	0	0	# Plate Gloves -> Dwarf Plate Gloves
987	646	913	0	0	# WarHammer -> Dwarf WarHammer

# Dwarf Female (body 988) - Leather armor
988	542	914	0	0	# Leather Tunic -> Dwarf Leather Tunic
988	543	915	0	0	# Leather Leggings -> Dwarf Leather Leggings
988	544	916	0	0	# Leather Sleeves -> Dwarf Leather Sleeves
988	545	917	0	0	# Leather Gloves -> Dwarf Leather Gloves
988	546	918	0	0	# Leather Gorget -> Dwarf Leather Gorget
```

The client reads equipconv.def and automatically substitutes equipment animations when worn by the specified body type.

**Note:** Male and female dwarves use different armor types because UO equipment animations are gender-specific. Plate armor animations only exist for males, while leather works for both.

## Step 5: Update Server Files

### bodyTable.cfg
Add entries for your body IDs:
```
987	Human	# Dwarf Male
988	Human	# Dwarf Female
```

### Create NPC Class (Dwarf.cs)

Located at: `Scripts/Mobiles/Vystia/Races/Dwarf.cs`

```csharp
[CorpseName("a dwarf corpse")]
public class Dwarf : BaseCreature
{
    [Constructable]
    public Dwarf(bool female)
        : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
    {
        Female = female;
        Name = GetRandomName(female);
        Title = "the dwarf";

        // Custom dwarf body - 75% scaled human sprites
        Body = female ? 988 : 987;
        Hue = Utility.RandomSkinHue();

        // Hair and beards (long beards for males)
        HairItemID = Utility.RandomList(0x203B, 0x203C, 0x203D);
        HairHue = Utility.RandomHairHue();

        if (!female)
        {
            FacialHairItemID = Utility.RandomList(0x203E, 0x204B, 0x204D); // Long beards
            FacialHairHue = HairHue;
        }

        // Gender-specific equipment (must match equipconv.def mappings)
        if (female)
        {
            AddItem(new LeatherChest());
            AddItem(new LeatherLegs());
            AddItem(new LeatherArms());
            AddItem(new LeatherGloves());
            AddItem(new LeatherGorget());
            AddItem(new HammerPick());
        }
        else
        {
            AddItem(new PlateChest());
            AddItem(new PlateLegs());
            AddItem(new PlateArms());
            AddItem(new PlateGloves());
            AddItem(new WarHammer());
        }
    }
}
```

### GM Spawn Commands

Add commands to spawn dwarves:
```csharp
CommandSystem.Register("sd", AccessLevel.GameMaster, SpawnDwarf_OnCommand);   // Random gender
CommandSystem.Register("sdm", AccessLevel.GameMaster, SpawnDwarfMale_OnCommand);  // Male
CommandSystem.Register("sdf", AccessLevel.GameMaster, SpawnDwarfFemale_OnCommand); // Female
```

## Step 5: Deploy to Client

Copy patched files to client folder:
```bash
copy "patched_client\anim.mul" "C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\anim.mul"
copy "patched_client\anim.idx" "C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\anim.idx"
```

## UO Animation Format Reference

### Index Entry (12 bytes)
```
Offset: uint32  - Position in anim.mul
Length: uint32  - Size of animation data
Extra:  uint32  - Usually 0
```

### Index Calculation
```python
def get_index_offset(body, action, direction):
    if body < 200:
        base_index = body * 110           # High detail monsters
    elif body < 400:
        base_index = 22000 + ((body - 200) * 65)  # Low detail
    else:
        base_index = 35000 + ((body - 400) * 175) # Humans/equipment

    index = base_index + (action * 5) + direction
    return index
```

### Animation Data Structure
```
Palette:     512 bytes (256 colors × 2 bytes, ARGB1555 XOR 0x8000)
Frame Count: 4 bytes (uint32)
Lookups:     frame_count × 4 bytes (offsets from after palette)
Frames:      Variable length frame data
```

### Frame Structure
```
Center X:  2 bytes (int16)
Center Y:  2 bytes (int16)
Width:     2 bytes (uint16)
Height:    2 bytes (uint16)
Runs:      Variable (run-length encoded pixel data)
End:       4 bytes (0x7FFF7FFF marker)
```

### Run Header (4 bytes, XOR with DOUBLE_XOR)
```
Bits 0-11:  Run length
Bits 12-21: Y offset
Bits 22-31: X offset
```

## mobtypes.txt Flags

| Flag | Meaning |
|------|---------|
| 0-99 | Read from anim.mul |
| 10000+ | Read from AnimationFrame*.uop |
| 20000 | Human body type (UOP) |

## Troubleshooting

### NPC invisible (only nametag shows)
- Check mobtypes.txt flag - must be < 10000 for MUL bodies
- Verify animation data was written at correct offset
- Check bodyTable.cfg has entry for body ID

### Halo/pixelation around sprite
- Apply alpha thresholding before resize
- Use NEAREST resampling, not LANCZOS
- Threshold alpha again after resize

### Full size instead of scaled
- Client reading from wrong offset
- Must overwrite in-place, not append to file
- Verify idx entry points to correct offset

### Newer mounts/creatures invisible
- AnimationFrame*.uop files contain expansion content
- Don't disable UOP files if you need expansion creatures
- Use body IDs with low mobtypes.txt flags for custom races

### Equipment not rendering on custom body (ClassicUO)
- ClassicUO has hardcoded `IsHuman` check in `Mobile.cs`
- Only bodies in this list get equipment rendering
- **Fix:** Add your body ID to the IsHuman property in ClassicUO source
- Example: Add `|| Graphic == 0x03DC` for body 988

### bodyconv.def redirecting to wrong file
- Default bodyconv.def may redirect your equipment body IDs to anim5.mul
- **Fix:** Comment out the redirect lines for your body IDs (e.g., 914-919)

## Distribution

Players need these modified files:
- `anim.mul` - Patched animation data (contains body + equipment sprites)
- `anim.idx` - Patched index file
- `equipconv.def` - Equipment animation conversion rules (for scaled equipment)
- `bodyconv.def` - Modified to disable redirects for custom equipment IDs
- **ClassicUO** - Custom build with body IDs added to IsHuman check

Server needs:
- `bodyTable.cfg` - Body type definitions
- NPC class files (e.g., `Dwarf.cs`)

## Credits

Developed for Vystia Shard, December 2025.
