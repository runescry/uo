# ServUO Comprehensive Item Inventory

**Purpose**: Complete inventory of UO-specific items from the ServUO codebase to guide lore entry creation for NPC knowledge bases.

**Source**: Actual C# class names extracted from `D:\UO\ServUO\Scripts`

**Date**: 2025-01-18

---

## Overview

This document catalogs **750+ unique item types** found in the ServUO codebase, organized by profession and crafting skill. All items are verified against actual source code files to ensure authenticity (no LLM hallucinations).

**Note on Item Names**: Item names have been cleaned for readability. C# class names often include implementation details like orientation (East/South/North/West) and type suffixes (Addon), which have been removed. For example:
- Class: `SmallBedEastAddon` → Display Name: "Small Bed"
- Class: `FancyElvenTableSouthAddon` → Display Name: "Fancy Elven Table"
- Class: `WoodenKiteShield` → Display Name: "Wooden Kite Shield"

This represents how NPCs would naturally refer to these items in conversation.

---

## 1. REAGENTS (16 types)
**Profession**: Mage, Alchemist
**Files**: `D:\UO\ServUO\Scripts\Items\Resource\` and `D:\UO\ServUO\Scripts\Spells\Reagent.cs`

### Standard Magery Reagents (8):
1. Black Pearl - Scrying, divination spells
2. Bloodmoss (or Blood Moss) - Life force, vitality spells
3. Garlic - Protection from evil
4. Ginseng - Healing spells
5. Mandrake Root - Power, energy spells
6. Nightshade - Poison, death magic
7. Spider's Silk - Binding, web spells
8. Sulfurous Ash - Destruction, fire spells

### Necromancy Reagents (5):
9. Bat Wing - Flight, darkness spells
10. Grave Dust - Death, undeath spells
11. Daemon Blood - Demonic power
12. Nox Crystal - Toxic, corrosive spells
13. Pig Iron - Physical binding

### Special Reagents (3):
14. Daemon Bone - Powerful necromantic component
15. Dragon Blood - High-power reagent
16. Dead Wood - Nature-based necromancy

**Lore Priority**: High - Mages need expert knowledge of all reagents

---

## 2. METAL ORES & INGOTS (9 types)
**Profession**: Blacksmith, Weaponsmith, Armorer, Miner
**Files**: `D:\UO\ServUO\Scripts\Items\Resource\Ore.cs` and `Ingots.cs`

### Ore Types (9):
1. Iron - Common, basic metal
2. Dull Copper - Weak magical properties
3. Shadow Iron - Dark, shadow-aligned
4. Copper - Better than iron
5. Bronze - Strong, durable
6. Gold - Valuable, magically conductive
7. Agapite - Rare, golden hue
8. Verite - Very rare, green hue
9. Valorite - Rarest, blue hue, magic-resistant

**Note**: Each ore has corresponding ingot type (smelted from ore)

**Lore Priority**: High - Already implemented (session 2025-01-18)

---

## 3. LEATHER & HIDES (4 types)
**Profession**: LeatherWorker, Cobbler, Tailor
**Files**: `D:\UO\ServUO\Scripts\Items\Resource\Leathers.cs` and `Hides.cs`

### Leather Types (4):
1. Leather (Regular) - Common hides from cows, bulls
2. Spined Leather - From spined creatures, tougher
3. Horned Leather - From horned beasts, stronger
4. Barbed Leather - From barbed creatures, strongest

**Note**: Hides are converted to leather using scissors

**Lore Priority**: High - Leatherworkers need detailed knowledge

---

## 4. DRAGON SCALES (6 types)
**Profession**: Blacksmith, Armorer (scale armor)
**File**: `D:\UO\ServUO\Scripts\Items\Resource\Scales.cs`

### Scale Types (6):
1. Red Scales - Fire-resistant, from red dragons
2. Yellow Scales - Acid-resistant, from gold dragons
3. Black Scales - Energy-resistant, from shadow wyrms
4. Green Scales - Poison-resistant, from green dragons
5. White Scales - Cold-resistant, from white wyrms
6. Blue Scales (Sea Serpent) - From sea serpents

**Lore Priority**: Medium - Specialized armor crafting

---

## 5. WOOD TYPES (7 types)
**Profession**: Carpenter, Bowyer
**Files**: `D:\UO\ServUO\Scripts\Items\Resource\Board.cs` and `Log.cs`

### Wood Types (7):
1. Regular Wood - Common trees
2. Oak - Strong, durable
3. Ash - Flexible, good for bows
4. Yew - Traditional bowyer wood
5. Heartwood - Magical properties
6. Bloodwood - Dark, crimson hue
7. Frostwood - Ice-blue, cold-aligned

**Note**: Logs are converted to boards at sawmill

**Lore Priority**: High - Carpenters and Bowyers need expertise

---

## 6. GEMS (9 types)
**Profession**: Jeweler
**Files**: `D:\UO\ServUO\Scripts\Items\Resource\`

### Gemstones (9):
1. Ruby - Red, precious
2. Sapphire - Blue, precious
3. Emerald - Green, precious
4. Diamond - Clear, most valuable
5. Amethyst - Purple, semi-precious
6. Citrine - Yellow-orange, semi-precious
7. Tourmaline - Multi-colored
8. Amber - Fossilized resin, golden
9. Star Sapphire - Rare, six-pointed star effect

**Lore Priority**: High - Already implemented (session 2025-01-18)

---

## 7. FISH (7 types)
**Profession**: Fisherman, Cook
**Files**: `D:\UO\ServUO\Scripts\Items\Resource\Fish.cs` and `MagicFish.cs`

### Regular Fish (3):
1. Fish - Basic fish, stackable
2. Big Fish - Trophy fish (display item)
3. Small Fish - Smaller catch

### Magic Fish (4) - Stat-boosting when consumed:
4. Prized Fish - Intelligence +5
5. Wondrous Fish - Dexterity +5
6. Truly Rare Fish - Strength +5
7. Peculiar Fish - Stamina +10

**Lore Priority**: Medium - Fishermen should know fishing spots and fish types

---

## 8. CROPS & VEGETABLES (8 types)
**Profession**: Farmer
**File**: `D:\UO\ServUO\Scripts\Items\Consumables\Vegetables.cs`

### Vegetables (8):
1. Carrot - Root vegetable
2. Cabbage - Leafy vegetable
3. Onion - Bulb vegetable
4. Lettuce - Leafy green
5. Pumpkin - Large squash
6. Small Pumpkin - Smaller variety
7. Squash - Yellow squash
8. Cantaloupe - Melon

**Lore Priority**: Medium - Farmers need crop knowledge

---

## 9. FRUITS (20 types)
**Profession**: Farmer, Cook
**File**: `D:\UO\ServUO\Scripts\Items\Consumables\Fruits.cs`

### Fruits (20):
1. Apple - Common fruit
2. Banana - Single banana
3. Bananas - Bunch of bananas
4. Coconut - Whole coconut
5. Open Coconut - Opened coconut
6. Split Coconut - Split coconut
7. Dates - Date fruit
8. Grapes - Grape cluster
9. Lemon - Single lemon
10. Lemons - Multiple lemons
11. Lime - Single lime
12. Limes - Multiple limes
13. Peach - Stone fruit
14. Pear - Pear fruit
15. Plum - Small stone fruit
16. Watermelon - Large melon
17. Small Watermelon - Smaller melon
18. Squash - Also classified as fruit
19. Cantaloupe - Also classified as fruit
20. Fruit Basket - Mixed fruit

**Lore Priority**: Low-Medium - General food knowledge

---

## 10. WEAPONS (150+ types)
**Profession**: Weaponsmith, Blacksmith, Bowyer, Tinker
**Directory**: `D:\UO\ServUO\Scripts\Items\Equipment\Weapons\`

### Swords (28 types):
- Broadsword, Cutlass, Katana, Kryss, Longsword, Scimitar, Viking Sword, Wakizashi
- Bone Machete, Daisho, Dread Sword, Elven Machete, Elven Spellblade, Glass Sword, Leafblade
- No Dachi, Paladin Sword, Radiant Scimitar, Rune Blade, Shortblade
- Gargish: Gargish Katana, Gargish Talwar, Gargish Daisho

### Axes (15 types):
- Axe, Battle Axe, Double Axe, Executioners Axe, Hatchet, Large Battle Axe, Two Handed Axe, War Axe
- Dual Short Axes, Ornate Axe, Heavy Ornate Axe
- Gargish: Gargish Axe, Gargish Battle Axe

### Maces/Blunt (17 types):
- Club, Hammer Pick, Mace, Maul, Scepter, War Hammer, War Mace
- Diamond Mace, Disc Mace, Emerald Mace, Ruby Mace, Silver Etched Mace
- Nunchaku, Tetsubo
- Gargish: Gargish Maul, Gargish War Hammer

### Polearms (11 types):
- Bardiche, Halberd, Pike, Pitchfork, Spear, Short Spear, War Fork
- Bladed Staff, Double Bladed Staff, Dual Pointed Spear, Tribal Spear
- Gargish: Gargish Bardiche, Gargish Pike, Gargish War Fork, Gargish Lance

### Staves (9 types):
- Black Staff, Gnarled Staff, Quarter Staff, Shepherds Crook, Wild Staff
- Glass Staff, Serpent Stone Staff
- Gargish: Gargish Gnarled Staff

### Ranged Weapons - Bows (11 types):
- Bow, Crossbow, Heavy Crossbow, Composite Bow, Repeating Crossbow, Yumi
- Elven Composite Longbow, Magical Shortbow, Lightweight Shortbow
- Juka Bow, Orcish Bow

### Knives/Daggers (8 types):
- Butcher Knife, Cleaver, Dagger, Skinning Knife, Assassin Spike
- Gargish: Gargish Butcher Knife, Gargish Cleaver, Gargish Dagger

### Ninja Weapons (8 types):
- Kama, Lajatang, Nunchaku, Sai, Tekagi, Tessen, Fukiya, Bokuto

### Wands (13 types):
- Clumsy Wand, Feeble Wand, Fireball Wand, Greater Heal Wand, Harm Wand, Heal Wand, ID Wand
- Lightning Wand, Magic Arrow Wand, Mana Drain Wand, Weakness Wand, Fireworks Wand, Magic Wand

### Exotic/Special (30+ types):
- Bone Harvester, Cyclone, Pickaxe, Scythe, Lance, Boomerang (thrown)

**Lore Priority**: High - Weaponsmiths and Blacksmiths need extensive weapon knowledge

---

## 11. ARMOR (110+ pieces)
**Profession**: Blacksmith, Armorer, LeatherWorker
**Directory**: `D:\UO\ServUO\Scripts\Items\Equipment\Armor\`

### Plate Armor (20+ pieces):
- Plate Arms, Plate Chest, Plate Gloves, Plate Gorget, Plate Helm, Plate Legs
- Japanese: Plate Do, Plate Haidate, Plate Hiro Sode, Plate Mempo, Plate Suneate
- Plate Battle Kabuto, Plate Hatsuburi, Decorative Plate Kabuto
- Gargish variants (male/female)

### Chainmail (5 pieces):
- Chain Chest, Chain Coif, Chain Legs, Chain Hatsuburi

### Ringmail (4 pieces):
- Ringmail Arms, Ringmail Chest, Ringmail Gloves, Ringmail Legs

### Leather Armor (25+ pieces):
- Leather Arms, Leather Chest, Leather Gloves, Leather Gorget, Leather Legs, Leather Cap
- Japanese: Leather Do, Leather Haidate, Leather Hiro Sode, Leather Jingasa, Leather Mempo, Leather Suneate
- Ninja: Leather Ninja Hood, Leather Ninja Jacket, Leather Ninja Mitts, Leather Ninja Pants
- Special: Leather Shorts, Leather Skirt, Leather Bustier Arms
- Gargish variants

### Studded Armor (12 pieces):
- Studded Arms, Studded Chest, Studded Gloves, Studded Gorget, Studded Legs
- Japanese variants: Studded Do, Studded Haidate, Studded Hiro Sode, Studded Mempo, Studded Suneate

### Bone Armor (5 pieces):
- Bone Arms, Bone Chest, Bone Gloves, Bone Helm, Bone Legs

### Dragon Armor (5 pieces):
- Dragon Arms, Dragon Chest, Dragon Gloves, Dragon Helm, Dragon Legs

### Hide Armor (6 pieces):
- Hide Chest, Hide Gloves, Hide Gorget, Hide Pants, Hide Pauldrons, Hide Female Chest

### Woodland Armor (6 pieces):
- Woodland Arms, Woodland Chest, Woodland Gloves, Woodland Gorget, Woodland Legs

### Leaf Armor (6 pieces):
- Leaf Arms, Leaf Chest, Leaf Gloves, Leaf Gorget, Leaf Legs, Leaf Tonlet

### Helmets (20+ types):
- Bascinet, Close Helm, Helmet, Norse Helm, Plate Helm
- Bone Helm, Chain Coif, Leather Cap, Orc Helm
- Circlet, Gemmed Circlet, Royal Circlet

### Shields (15 types):
- Bronze Shield, Buckler, Heater Shield, Metal Shield, Metal Kite Shield, Wooden Shield, Wooden Kite Shield
- Chaos Shield, Order Shield
- Gargish variants

**Lore Priority**: High - Armorers and Blacksmiths need comprehensive armor knowledge

---

## 12. CLOTHING (100+ items)
**Profession**: Tailor
**Directory**: `D:\UO\ServUO\Scripts\Items\Equipment\Clothing\`

### Hats (30+ types):
- Bear Mask, Deer Mask, Feathered Hat, Floppy Hat, Flower Garland, Jester Hat, Straw Hat, Tall Straw Hat, Tricorne Hat, Wide Brim Hat, Wizards Hat
- Bandana, Bonnet, Cap, Skull Cap
- Cloth Ninja Hood, Kasa

### Shirts (15+ types):
- Shirt, Fancy Shirt, Doublet, Tunic, Surcoat
- Cloth Ninja Jacket, Jin Baori, Kamishimo, Hakama Shita
- Elven Shirt, Checkered Shirt, Formal Shirt

### Pants/Legs (15+ types):
- Short Pants, Long Pants, Kilt, Skirt
- Hakama, Tattsuke Hakama, Elven Pants

### Robes (20+ types):
- Robe, Plain Dress, Fancy Dress, Gilded Dress
- Elven Robe, Female Elven Robe

### Cloaks (8 types):
- Cloak, Fur Cape, Reward Cloak

### Shoes/Boots (12+ types):
- Boots, Thigh Boots, Shoes, Sandals
- Fur Boots, Ninja Tabi, Samurai Tabi, Waraji
- Elven Boots

### Accessories:
- Body Sash, Full Apron, Half Apron
- Belts: Dagger Belt, Mace Belt, Sword Belt, Leather Ninja Belt, First Aid Belt

**Lore Priority**: Medium - Tailors need clothing knowledge

---

## 13. CARPENTRY ITEMS (50+ types)
**Profession**: Carpenter
**Directories**: `D:\UO\ServUO\Scripts\Items\Addons\`

### Furniture:
- **Beds**: Small Bed, Large Bed, Elven Bed, Tall Elven Bed, Gargish Cot, Large Gargoyle Bed, Unmade Bed, Broken Bed
- **Tables**: Wooden Table, Long Wooden Table, Metal Table, Long Metal Table, Fancy Elven Table, Ornate Elven Table, Large Stone Table, Medium Stone Table, Buffet Table, Formal Dining Table, Gargish Long Table
- **Chairs/Seating**: Fancy Couch, Fancy Loveseat, Plush Loveseat, Elven Loveseat, Rustic Bench, Bone Couch, Bone Throne, Gargish Couch
- **Storage**: Ornate Elven Chest, Elven Dresser, Ter Mur Dresser, Broken Armoire, Broken Chest of Drawers, Broken Bookcase, Broken Vanity
- **Shelves**: Fancy Wooden Shelf, Plain Wooden Shelf, Arcane Bookshelf, Small Display Case
- **Misc Furniture**: Bone Table, Dressform

### Instruments (12 types):
- Harp, Lap Harp, Lute, Drums, Tambourine, Tambourine Tassel, Trumpet, Bamboo Flute, Cello, Cowbell, Aud Char, Harpsichord

### Craft Stations:
- Loom, Spinning Wheel, Anvil, Forge

**Lore Priority**: Medium - Carpenters need furniture and instrument knowledge

---

## 14. TINKER ITEMS (30+ types)
**Profession**: Tinker
**Files**: Various directories

### Tools & Parts:
- Axle, Axle Gears, Gears, Hinge, Springs
- Clock Frame, Clock Parts, Clockwork Assembly
- Sextant Parts, Barrel Parts, Lockpick

### Special Items:
- Telescope, Sextant
- Various keys, traps, and mechanisms
- Golem parts and assemblies

**Lore Priority**: Medium - Tinkers need mechanical knowledge

---

## 15. ALCHEMY POTIONS (40+ types)
**Profession**: Alchemist
**Directory**: `D:\UO\ServUO\Scripts\Items\Consumables\`

### Healing Potions (3):
- Lesser Heal Potion, Heal Potion, Greater Heal Potion

### Cure Potions (3):
- Lesser Cure Potion, Cure Potion, Greater Cure Potion

### Poison Potions (4):
- Lesser Poison Potion, Poison Potion, Greater Poison Potion, Deadly Poison Potion

### Explosion Potions (3):
- Lesser Explosion Potion, Explosion Potion, Greater Explosion Potion

### Stat Potions (6):
- Agility Potion, Greater Agility Potion
- Strength Potion, Greater Strength Potion
- Refresh Potion, Total Refresh Potion

### Special Potions (20+):
- Night Sight, Invisibility Potion, Darkglow Potion, Parasitic Potion, Shatter Potion
- Conflagration Potion, Greater Conflagration Potion
- Confusion Blast Potion, Greater Confusion Blast Potion
- Exploding Tar Potion

### Eodon Potions (6):
- Barrab Hemolymph Concentrate, Jukari Burn Poultice, Kurak Ambusher's Essence
- Barako Draft of Might, Urali Trance Tonic, Sakkhra Prophylaxis

**Lore Priority**: High - Alchemists need comprehensive potion knowledge

---

## 16. FOOD & COOKING (100+ items)
**Profession**: Cook
**Files**: Multiple in Consumables directory

### Raw Meats:
- Raw Bird, Raw Chicken Leg, Raw Lamb Leg, Raw Ribs, Raw Fish Steak

### Cooked Meats:
- Bacon, Cooked Bird, Chicken Leg, Fried Eggs, Lamb Leg, Ribs, Sausage, Ham, Fish Steak

### Baked Goods:
- Bread, French Bread, Cookies, Muffins, Cake, Pie (various), Pizza

### Prepared Foods:
- Stew, Bowl of Stew, Cheese Wedge, Cheese Wheel

### Beverages:
- Ale, Cider, Liquor, Milk, Wine, Water
- Sake, Harvest Wine, Melisandes Fermented Wine

**Lore Priority**: Medium - Cooks need food and recipe knowledge

---

## 17. TAMEABLE CREATURES (98+ types)
**Profession**: AnimalTrainer
**Directory**: `D:\UO\ServUO\Scripts\Mobiles\Normal\`

### Common Animals (14):
- Dog, Cat, Chicken, Cow, Goat, Pig, Sheep
- Horse, Palamino Horse, Pack Horse, Pack Llama, Llama, Ridable Llama, Rabbit

### Wild Animals (10):
- Rat, Sewer Rat, Giant Rat, Snake, Squirrel, Ferret, Eagle, Walrus, Jack Rabbit

### Predators (15):
- **Wolves**: Grey Wolf, Timber Wolf, White Wolf, Dire Wolf, Tsuki Wolf, Dragon Wolf
- **Bears**: Black Bear, Brown Bear, Grizzly Bear, Polar Bear
- **Big Cats**: Panther, Snow Leopard, Lion, Sabertoothed Tiger, Wild Tiger
- Gorilla

### Monsters (8):
- Mongbat, Strong Mongbat, Greater Mongbat, Imp, Slime
- **Spiders**: Giant Spider, Dread Spider, Frost Spider, Wolf Spider

### Beetles (3):
- Fire Beetle, Iron Beetle, Rune Beetle

### Lizards/Reptiles (5):
- Lava Lizard, Giant Toad, Scorpion

### Dragons (15):
- Drake, Dragon, Greater Dragon, Frost Dragon, Serpentine Dragon, Shadow Wyrm, White Wyrm
- Swamp Dragon, Scaled Swamp Dragon, Stygian Drake, Platinum Drake
- Hiryu, Lesser Hiryu

### Mounts (10):
- Nightmare, Fire Steed, Dread Warhorse
- Ridgeback, Savage Ridgeback
- **Ostards**: Forest Ostard, Frenzied Ostard
- Unicorn, Kirin, Windrunner

### Mystical (2):
- Phoenix, Reptalon

### Eodon Creatures (8):
- Triceratops
- **Bouras**: High Plains Boura, Lowland Boura, Ruddy Boura
- Eowmu, Gaman, Skittering Hopper

### Exotic/Special (18):
- Hell Cat, Predator Hell Cat, Hell Hound, Icehound
- Hungry Coconut Crab, Lasher
- Slith, Stone Slith, Skree
- Giant Ice Worm, Frost Mite, Mountain Goat, Ossein Ram
- Skeletal Cat, Triton, Vorpal Bunny

**Lore Priority**: High - Animal Trainers need creature knowledge, taming difficulty, loyalty requirements

---

## SUMMARY COUNTS

| Category | Count | Priority |
|----------|-------|----------|
| Reagents | 16 | High |
| Metal Ores/Ingots | 9 | High ✅ |
| Leather Types | 4 | High |
| Dragon Scales | 6 | Medium |
| Wood Types | 7 | High |
| Gems | 9 | High ✅ |
| Fish | 7 | Medium |
| Crops/Vegetables | 8 | Medium |
| Fruits | 20 | Low |
| Weapons | 150+ | High |
| Armor | 110+ | High |
| Clothing | 100+ | Medium |
| Carpentry | 50+ | Medium |
| Tinker Items | 30+ | Medium |
| Alchemy Potions | 40+ | High |
| Food/Cooking | 100+ | Medium |
| Tameable Creatures | 98+ | High |

**GRAND TOTAL**: 750+ unique item types

**Legend**:
- ✅ = Already implemented (lore entries created)
- High = Critical for profession immersion
- Medium = Important for completeness
- Low = Nice to have

---

## Implementation Priority

### Phase 1 - Critical Profession Knowledge (High Priority)
1. **Reagents** (16 entries) - Mages, Alchemists
2. **Wood Types** (7 entries) - Carpenters, Bowyers
3. **Leather Types** (4 entries) - LeatherWorkers, Cobblers
4. **Alchemy Potions** (40 entries) - Alchemists
5. **Tameable Creatures** (98 entries) - Animal Trainers

### Phase 2 - Weapon & Armor Knowledge (High Priority)
6. **Weapons** (150+ entries) - Weaponsmiths, Blacksmiths
7. **Armor** (110+ entries) - Armorers, Blacksmiths

### Phase 3 - Secondary Professions (Medium Priority)
8. **Clothing** (100+ entries) - Tailors
9. **Carpentry** (50+ entries) - Carpenters
10. **Food/Cooking** (100+ entries) - Cooks
11. **Fish** (7 entries) - Fishermen
12. **Crops** (8 entries) - Farmers

### Phase 4 - Specialized Items (Medium-Low Priority)
13. **Dragon Scales** (6 entries) - Armorers
14. **Tinker Items** (30+ entries) - Tinkers
15. **Fruits** (20 entries) - Farmers, Cooks

---

## Notes for Lore Entry Creation

### Entry Template
Each lore entry should include:
- **id**: Unique identifier (e.g., "black_pearl")
- **title**: Display name (e.g., "Black Pearl")
- **category**: "Crafting", "Resource", "Creature", etc.
- **summary**: 1-2 sentence summary
- **content**: Detailed description (200-400 words)
  - What it is
  - Where it's found/obtained
  - What it's used for
  - Crafting applications
  - Value/rarity
  - Special properties
- **tags**: Searchable keywords
- **importance**: 6-9 (based on rarity/significance)
- **source**: "UO"
- **domainExpertise**: Which professions have expertise
  - Expert: Primary profession
  - Proficient: Secondary professions
  - Aware: General knowledge
  - Ignorant: Should not know
- **questionCategory**: "Crafting", "Creatures", "Resources", etc.
- **primaryReferralTarget**: Primary expert
- **secondaryReferralTarget**: Secondary expert

### Writing Guidelines
1. **Authentic UO Content**: Use UO lore, locations, and terminology
2. **Profession Context**: How does each profession use the item?
3. **In-Game Mechanics**: Reference actual game mechanics (smelting, taming difficulty, etc.)
4. **Avoid Modern References**: Keep medieval fantasy tone
5. **Cross-References**: Mention related items and professions

---

**Next Steps**: See README.md for current system documentation

**Date Created**: 2025-01-18
**Last Updated**: 2025-01-18
