# Sosaria Lore Database

## Overview
This directory contains the lore database for the LLM NPC system. NPCs can reference this lore when conversing with players to provide accurate, immersive information about the world of Britannia/Sosaria.

## Files
- **sosaria_lore.json** - Main lore database (47 entries)

## Lore Entry Structure
```json
{
  "id": "unique_identifier",
  "category": "Location|History|NPC|Faction|Monster|Magic|Virtue",
  "title": "Display Title",
  "content": "2-5 sentences of rich lore content",
  "tags": ["searchable", "keywords", "for", "retrieval"],
  "importance": 1-10,
  "source": "Official Lore|Server Event|Custom"
}
```

## Current Coverage (47 Entries)

### Cities & Locations (15 entries)
- Britain (Capital, Compassion)
- Trinsic (Honor, Paladins)
- Vesper (Mining, Trade)
- Moonglow (Magic, Lycaeum)
- Yew (Justice, Forest)
- Minoc (Sacrifice, Mining)
- Skara Brae (Spirituality, Island)
- Magincia (Humility, Rebuilt)
- Jhelom (Valor, Warriors)
- Cove (Coastal Village)
- Serpent's Hold (Fortress)
- Nujel'm (Desert, Exotic)
- Buccaneer's Den (Pirates, Lawless)
- Wind (Rangers)
- Britannia (The Realm)

### Dungeons (8 entries)
- Shame (Humility)
- Despise (Prison)
- Destard (Dragons)
- Deceit (Undead, Illusions)
- Covetous (Greed, Harpies)
- Wrong (Criminals)
- Hythloth (Daemons, Abyss)

### The Eight Virtues (8 entries)
- Compassion (Britain)
- Honor (Trinsic)
- Valor (Jhelom)
- Justice (Yew)
- Sacrifice (Minoc)
- Honesty (Moonglow)
- Spirituality (Skara Brae)
- Humility (Magincia)

### Major NPCs (5 entries)
- Lord British (Ruler of Britannia)
- Lord Blackthorn (Dark Lord)
- Mondain (Evil Wizard)
- Minax (Enchantress)
- Exodus (Creation of Evil)

### Magic & World Lore (5 entries)
- Moongates (Magical Portals)
- Reagents (Magic Components)
- Eight Circles of Magic
- Trammel & Felucca (The Two Moons)

### Factions (4 entries)
- Gargoyles (Winged Race)
- Paladins (Warriors of Honor)
- Thieves Guild (Shadows)
- Rangers (Wilderness Guardians)

### Monsters (5 entries)
- Dragons (Ancient Terror)
- Liches (Undead Mages)
- Daemons (Hellish Entities)
- Orcs (Savage Warriors)

## Usage with LLM NPCs

When a player asks about something in the world, the RAG (Retrieval-Augmented Generation) system will:

1. Search the lore database using keywords from the player's question
2. Retrieve the top 3-5 most relevant lore entries
3. Include them in the NPC's context
4. The LLM generates a response using this lore knowledge

### Example:
**Player:** "Tell me about Britain"

**Retrieved Lore:**
- britain_001 (Britain - Capital of Britannia)
- virtue_compassion_001 (Virtue of Compassion)
- lord_british_001 (Lord British - Ruler)

**NPC Response:**
*"Ah, Britain! The jewel of Britannia, founded by Lord British himself. The city represents the virtue of Compassion and serves as our realm's capital. Castle British stands at its heart, where our great ruler holds court..."*

## Adding New Lore

To add new lore entries:

1. Open `sosaria_lore.json`
2. Add new entry following the structure above
3. Use unique ID (category_name_###)
4. Include comprehensive tags for searchability
5. Set appropriate importance (10 = critical, 1 = minor)
6. Validate JSON syntax

## Future Expansion

Potential additions:
- Server-specific events and lore
- Player-created history
- Guild histories
- Legendary items and artifacts
- Historical events and wars
- Minor NPCs and personalities
- Regional details and sub-locations
- Quests and storylines
- Custom content for your shard

## Integration Status

- ✅ Lore database created (47 entries)
- ⏳ RAG retrieval system (pending)
- ⏳ NPC context integration (pending)
- ⏳ In-game testing (pending)
