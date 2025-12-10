# Extracted Mob Bodies Reference

## All 184 Extracted Body IDs (0-399 range)

### Classic Monsters (1-87)
1, 2, 3, 4, 5, 6, 7, 8, 9, 12, 13, 14, 15, 16, 17, 18, 21, 22, 24, 26, 28, 29, 30, 31, 32, 33, 34, 35, 36, 39, 41, 42, 44, 45, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87

### Special Creatures (101-122)
10, 11, 19, 20, 23, 25, 27, 37, 38, 40, 43, 46, 101, 102, 117, 118, 119, 120, 121, 122

### Rare Creatures (150-151)
150, 151

### Extended Set (195-238)
195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 237, 238

### Special Mobs (290-292)
290, 291, 292

### Advanced Set (303-314)
303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 314

### Modern Expansion (369-399)
369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399

---

## Usage in ServUO

### Example Creature Template

```csharp
using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a creature corpse")]
    public class UOAdventuresCreature : BaseCreature
    {
        [Constructable]
        public UOAdventuresCreature() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a creature";
            Body = 77;  // Replace with desired body ID
            BaseSoundID = 0x165;  // Optional: creature sounds

            SetStr(100, 150);
            SetDex(80, 100);
            SetInt(50, 75);

            SetHits(200, 300);

            SetDamage(10, 15);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 20, 30);

            SetSkill(SkillName.MagicResist, 60.0, 80.0);
            SetSkill(SkillName.Tactics, 70.0, 90.0);
            SetSkill(SkillName.Wrestling, 60.0, 80.0);

            Fame = 2500;
            Karma = -2500;

            VirtualArmor = 30;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
        }

        public UOAdventuresCreature(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
```

---

## ClassicUO Configuration

### Option 1: Point to UO Adventures Client

Edit ClassicUO `settings.json`:
```json
{
  "ultimaonlinedirectory": "C:\\DevEnv\\GIT\\UO\\UO Adventures\\Client\\Data Files",
  "clientversion": "7.0.90.0",
  ...
}
```

### Option 2: Copy Specific Anim Files

Copy desired anim files from UO Adventures to your main client:
- `anim.mul` / `anim.idx`
- `anim2.mul` / `anim2.idx` (if you want bodies from this file)
- etc.

---

## Notes

- **Body IDs 0-399** are creatures/monsters
- **Body IDs 400+** are equipment and human animations (not extracted)
- All extracted animations include:
  - Multiple actions (walk, attack, die, etc.)
  - 5 directional views
  - Frame-by-frame PNG data with center points

## Extraction Location

All extracted animations: `C:\DevEnv\GIT\UO\Vystia\tools\creature_animations\`

Each body has a `body_XX` directory with subdirectories for each action/direction combination.
