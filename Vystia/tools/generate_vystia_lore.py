"""
Generate Vystia Lore JSON Files for LLM RAG System

This script reads WORLD_LORE.md, VYSTIA_MASTER_INVENTORY.md, and VYSTIA_NPC_DESIGN.md
to create comprehensive lore JSON files for the ServUO LLM system.

Output: Multiple domain-specific JSON files in ServUO/Scripts/Services/LLM/Data/Lore/
"""

import json
import os
import sys
from pathlib import Path

# Base paths
UO_ROOT = Path(r"C:\DevEnv\GIT\UO")
VYSTIA_ROOT = UO_ROOT / "Vystia"
LLM_LORE_DIR = UO_ROOT / "ServUO" / "Data" / "Lore"  # Correct: Server looks in ServUO/Data/Lore/

# Ensure output directory exists
LLM_LORE_DIR.mkdir(parents=True, exist_ok=True)

def create_lore_entry(id, title, category, content, tags, importance, related_entries=None, source=None):
    """Create a properly formatted lore entry"""
    entry = {
        "id": id,
        "title": title,
        "category": category,
        "content": content,
        "tags": tags,
        "importance": importance
    }
    if related_entries:
        entry["relatedEntries"] = related_entries
    if source:
        entry["source"] = source
    return entry

def generate_vystia_general():
    """Generate vystia_general.json with regions, cities, history, factions"""
    entries = []

    # Major Regions (21 total)
    regions = [
        {
            "id": "ironclad_empire",
            "title": "Ironclad Empire",
            "content": "The Ironclad Empire is a beacon of human ingenuity and arcane mastery, featuring sprawling cities where magic powers everything from humble homes to impressive forges. The constant clanging of metal and the soft hum of enchantments create an ever-present symphony. Stone streets wind through bustling marketplaces and towering keeps, where spellsmiths and craftsmen work side by side, blending tradition with technological and magical progress.",
            "tags": ["ironclad", "empire", "steampunk", "technology", "humans", "dwarves", "gnomes", "innovation"],
            "importance": 10,
            "related": ["ironhaven", "artificer", "fighter", "templar", "cogsmith_creed"]
        },
        {
            "id": "frosthold",
            "title": "Frosthold",
            "content": "In the northernmost reaches of Vystia, Frosthold is a realm of ice and snow where only the hardiest souls dare to live. Orcs and Barbarians, clad in furs and wielding ice magic, thrive here, respecting the land's deadly beauty and embracing its brutal challenges. Glaciers and frozen lakes dot the landscape, shimmering under aurora-filled skies.",
            "tags": ["frosthold", "ice", "snow", "tundra", "barbarian", "beastmaster", "ice mage", "orcs", "frost"],
            "importance": 10,
            "related": ["frostholm", "barbarian", "beastmaster", "ice_mage", "frosthelm_faith"]
        },
        {
            "id": "verdantpeak",
            "title": "Verdantpeak",
            "content": "Verdantpeak features lush, magical forests protected by ancient earth magic. Ancient towering trees, hidden groves, mystical rivers, and abundant wildlife characterize this realm. Elves and forest dwellers live in harmony with nature, emphasizing balance and the cycles of life. The forest is home to treants, dryads, and ancient spirits.",
            "tags": ["verdantpeak", "forest", "nature", "elves", "druid", "alchemist", "trees", "magic"],
            "importance": 10,
            "related": ["verdantheart", "druid", "alchemist", "lunara_covenant"]
        },
        {
            "id": "emberlands",
            "title": "The Emberlands",
            "content": "The Emberlands are a volcanic wasteland of molten lava, ash plains, and geothermal vents. Fire elementals and heat-resistant creatures thrive in this harsh environment. Sorcerers channel the raw elemental chaos of their volcanic home, wielding fire magic with devastating power. The landscape is dotted with obsidian formations and rivers of lava.",
            "tags": ["emberlands", "fire", "volcano", "lava", "sorcerer", "elemental", "heat", "ash"],
            "importance": 10,
            "related": ["emberforge", "sorcerer", "cogsmith_creed"]
        },
        {
            "id": "whispering_sands",
            "title": "The Whispering Sands (Desert of Surya)",
            "content": "The Whispering Sands is an ancient desert filled with ruins, sand dunes, and hidden oases. Rangers known as Dune Striders are expert survivalists who navigate the challenging desert landscape with ease. Illusionists called Mirage Weavers create illusions to protect caravans from intruders. Ancient sphinxes and sand dragons guard forgotten treasures.",
            "tags": ["desert", "sand", "ruins", "ranger", "illusionist", "oasis", "sphinx", "ancient"],
            "importance": 10,
            "related": ["sunspire", "ranger", "illusionist", "surya_sandscript"]
        },
        {
            "id": "shadowfen",
            "title": "Shadowfen",
            "content": "Shadowfen is a toxic swamp shrouded in perpetual mist and darkness. Witches form secretive covens that delve into the darker aspects of magic, using herbal magic and curses. The swamp is home to poisonous creatures, bog spirits, and ancient hags. Will-o'-wisps lead travelers astray into dangerous areas.",
            "tags": ["shadowfen", "swamp", "poison", "witch", "hex", "fog", "dark", "bog"],
            "importance": 9,
            "related": ["witch", "lunara_covenant"]
        },
        {
            "id": "crystal_barrens",
            "title": "Crystal Barrens",
            "content": "The Crystal Barrens are expansive crystal fields where massive crystalline formations rise from the ground, refracting light into dazzling displays. Oracles known as Seers utilize the mystical energies to foresee potential futures. The crystals hum with arcane power and are sought after for magical crafting. Crystal elementals patrol the barrens.",
            "tags": ["crystal", "barrens", "oracle", "divination", "energy", "future", "vision", "magic"],
            "importance": 9,
            "related": ["oracle", "celestis_arcanum"]
        },
        {
            "id": "shadowvoid",
            "title": "ShadowVoid (Eternal Twilight)",
            "content": "The ShadowVoid is a planar realm of perpetual twilight where darkness and shadow reign. Warlocks of the Covenant of the Hidden Eye gain powers through pacts with otherworldly beings. Necromancers called Shadowbinders manipulate the energies of life and death. The realm exists between dimensions, touched by void energies.",
            "tags": ["shadowvoid", "void", "darkness", "warlock", "necromancer", "twilight", "planar", "shadow"],
            "importance": 9,
            "related": ["warlock", "necromancer", "celestis_arcanum"]
        },
        {
            "id": "deepforge",
            "title": "Deepforge",
            "content": "Beneath the earth lies the expansive network of tunnels and forges known as Deepforge. Illuminated by the fiery glow of forges and molten metal, this underground kingdom is a marvel of dwarven craftsmanship and engineering. They harness fire magic to shape the finest weapons and armor.",
            "tags": ["deepforge", "underground", "dwarves", "forge", "crafting", "mining", "metal", "caves"],
            "importance": 9,
            "related": ["ironclad_empire", "artificer", "dogoth_forge"]
        },
        {
            "id": "skyreach_mountains",
            "title": "Skyreach Mountains",
            "content": "The Skyreach Mountains are towering peaks that pierce the clouds, with floating sky islands suspended by ancient magic. Monks of the Gearwheel Monastery integrate spiritual practice with mechanical innovations. The mountains are home to griffons, wyverns, and air elementals. Strong winds and lightning storms are common.",
            "tags": ["skyreach", "mountains", "sky", "peaks", "monk", "clouds", "flying", "wind"],
            "importance": 8,
            "related": ["monk", "gearwheel_monastery"]
        },
        {
            "id": "sunken_isles",
            "title": "Sunken Isles",
            "content": "The Sunken Isles are a chain of partially submerged islands where ruins of an ancient civilization lie beneath the waves. Bounty Hunters called Crypt Trackers specialize in recovering sunken treasures. The waters are filled with sea serpents, krakens, and undead mariners. Ancient temples peek above the waterline.",
            "tags": ["sunken", "isles", "underwater", "ocean", "bounty hunter", "ruins", "sea", "treasure"],
            "importance": 8,
            "related": ["bounty_hunter", "oceana_covenant"]
        },
        {
            "id": "glimmering_archipelago",
            "title": "Glimmering Archipelago",
            "content": "The Glimmering Archipelago is a collection of shimmering islands with crystal-clear waters and white sand beaches. Knights called Guardians protect the islands from pirates and sea monsters. The waters glow with bioluminescent plankton at night. Coral reefs hide underwater caves filled with treasure.",
            "tags": ["archipelago", "islands", "ocean", "knight", "crystal", "beaches", "pirates", "sea"],
            "importance": 8,
            "related": ["knight", "oceana_covenant"]
        },
        {
            "id": "forgotten_depths",
            "title": "The Forgotten Depths",
            "content": "The Forgotten Depths are an underwater civilization in a massive oceanic trench. Summoners called Arcane Summoners explore esoteric magic by calling forth entities from other dimensions. Ancient leviathans and abyssal creatures lurk in the darkness. Bio-luminescent coral cities dot the trench walls.",
            "tags": ["depths", "underwater", "ocean", "summoner", "trench", "dark", "abyssal", "deep"],
            "importance": 8,
            "related": ["summoner", "oceana_covenant"]
        },
        {
            "id": "wilderlands",
            "title": "The Wilderlands",
            "content": "The Wilderlands are untamed frontier territories filled with primal forests, grasslands, and dangerous wildlife. Shamans called Spiritcallers serve as bridges between the physical world and spiritual realm. Ancient tribal settlements dot the landscape. Dire wolves, owlbears, and prehistoric beasts roam freely.",
            "tags": ["wilderlands", "wilderness", "frontier", "shaman", "tribal", "spirits", "primal", "nature"],
            "importance": 8,
            "related": ["shaman"]
        },
        {
            "id": "mystic_canyons",
            "title": "The Mystic Canyons",
            "content": "The Mystic Canyons are deep ravines carved by ancient magic, with walls covered in glowing runes and sigils. Enchanters called Sigilmasters imbue objects with magical properties using arcane symbols. Echo elementals and rune guardians protect the canyon's secrets. Ancient libraries are carved into cliff faces.",
            "tags": ["canyons", "ravines", "enchanter", "runes", "sigils", "magic", "ancient", "arcane"],
            "importance": 7,
            "related": ["enchanter"]
        },
        {
            "id": "radiant_plains",
            "title": "The Radiant Plains",
            "content": "The Radiant Plains are vast grasslands bathed in eternal golden sunlight. Paladins serve as holy warriors dedicated to justice and righteousness. The plains are home to herds of wild horses, giant eagles, and celestial beings. Ancient standing stones mark places of power.",
            "tags": ["plains", "grasslands", "paladin", "holy", "light", "justice", "horses", "radiant"],
            "importance": 7,
            "related": ["paladin"]
        },
        {
            "id": "hollow_forests",
            "title": "The Hollow Forests",
            "content": "The Hollow Forests are ancient woodlands where massive trees have hollow interiors large enough to house entire communities. Druids and clerics protect the balance between light and shadow within the forest. Fey creatures, dryads, and tree spirits dwell in the hollows.",
            "tags": ["hollow", "forest", "trees", "ancient", "fey", "dryads", "spirits", "woods"],
            "importance": 7,
            "related": ["druid", "cleric", "lunara_covenant"]
        },
        {
            "id": "blazing_frontier",
            "title": "The Blazing Frontier",
            "content": "The Blazing Frontier is a harsh badlands of extreme heat, rock formations, and sparse vegetation. Rogues and outlaws make their homes in hidden canyons. Fire snakes, rock elementals, and desert raiders patrol the territory. Ancient fire temples dot the landscape.",
            "tags": ["frontier", "badlands", "heat", "desert", "rogue", "outlaws", "fire", "harsh"],
            "importance": 6,
            "related": ["rogue"]
        },
        {
            "id": "golden_steppe",
            "title": "Golden Steppe",
            "content": "The Golden Steppe is an endless expanse of golden grass and rolling hills. Nomadic tribes travel the steppe with herds of livestock. The region is known for its spectacular sunsets and fierce windstorms. Horse-mounted warriors patrol the open plains.",
            "tags": ["steppe", "grasslands", "nomadic", "horses", "tribes", "plains", "golden", "wind"],
            "importance": 6,
            "related": []
        },
        {
            "id": "verdant_isles",
            "title": "The Verdant Isles",
            "content": "The Verdant Isles are a lush archipelago covered in tropical rainforests and exotic flora. Alchemists called Brewmasters specialize in concocting potent elixirs from rare herbs. The islands are home to colorful birds, giant insects, and botanical wonders. Ancient alchemical laboratories are hidden in jungle ruins.",
            "tags": ["isles", "tropical", "jungle", "alchemist", "herbs", "rainforest", "exotic", "lush"],
            "importance": 7,
            "related": ["alchemist"]
        },
        {
            "id": "winterguard",
            "title": "Winterguard",
            "content": "Winterguard is a fortified region on the edge of Frosthold where Ice Mages called Frostweavers defend against threats from the frozen wastes. Massive ice walls protect settlements from blizzards and frost giants. The region is known for its ice magic academies and frozen fortresses.",
            "tags": ["winterguard", "ice", "fortress", "ice mage", "defense", "frozen", "walls", "academy"],
            "importance": 8,
            "related": ["frosthold", "ice_mage"]
        }
    ]

    for region in regions:
        entries.append(create_lore_entry(
            id=region["id"],
            title=region["title"],
            category="Location",
            content=region["content"],
            tags=region["tags"],
            importance=region["importance"],
            related_entries=region.get("related", [])
        ))

    # Major Cities (10 capitals)
    cities = [
        {
            "id": "ironhaven",
            "title": "Ironhaven",
            "content": "Ironhaven is the capital of the Ironclad Empire, a massive metropolis of steam-powered machinery, clockwork automatons, and towering factories. The Imperial Palace dominates the skyline, while the Great Forge district produces the finest mechanical wonders in Vystia. Emperor Garrick Steelarm rules from the Iron Throne.",
            "tags": ["ironhaven", "capital", "city", "ironclad", "steampunk", "emperor", "metropolis"],
            "importance": 10,
            "related": ["ironclad_empire", "emperor_garrick"]
        },
        {
            "id": "frostholm",
            "title": "Frostholm",
            "content": "Frostholm is the capital of Frosthold, built entirely from ice and frozen stone. The Frost Palace is home to Chieftain Bjorn Frostbeard, leader of the northern clans. Ice sculptors create magnificent frozen art throughout the city. The Frostweaver Academy trains Ice Mages in the mastery of cold magic.",
            "tags": ["frostholm", "capital", "city", "frosthold", "ice", "palace", "chieftain"],
            "importance": 10,
            "related": ["frosthold", "chieftain_bjorn"]
        },
        {
            "id": "verdantheart",
            "title": "Verdantheart",
            "content": "Verdantheart is the capital of Verdantpeak, a city built within and around massive ancient trees. Wooden platforms connected by rope bridges create a multi-layered canopy city. The Tree Council, led by Elder Seraphina Leafwhisper, governs from the Heart Tree. Druids maintain the sacred groves surrounding the city.",
            "tags": ["verdantheart", "capital", "city", "verdantpeak", "forest", "trees", "elves", "druid"],
            "importance": 10,
            "related": ["verdantpeak", "elder_seraphina"]
        },
        {
            "id": "emberforge",
            "title": "Emberforge",
            "content": "Emberforge is the capital of the Emberlands, built inside a massive dormant volcano caldera. Obsidian buildings line streets of cooled lava. The Magma Citadel houses Archmage Pyrus Ashborn, master of elemental fire magic. The city's forges produce fire-enchanted weapons and armor of legendary quality.",
            "tags": ["emberforge", "capital", "city", "emberlands", "volcano", "fire", "obsidian", "forge"],
            "importance": 10,
            "related": ["emberlands", "archmage_pyrus"]
        },
        {
            "id": "sunspire",
            "title": "Sunspire",
            "content": "Sunspire is the capital of the Whispering Sands, a city of golden sandstone towers and dome structures. The Grand Bazaar is the largest marketplace in Vystia. Sultan Azir al-Rashid rules from the Palace of Sun and Sand. The Mirage Academy teaches Illusionists their mystical arts.",
            "tags": ["sunspire", "capital", "city", "desert", "sand", "sultan", "bazaar", "palace"],
            "importance": 10,
            "related": ["whispering_sands", "sultan_azir"]
        },
        {
            "id": "shadowmire",
            "title": "Shadowmire",
            "content": "Shadowmire is a dark city built on stilts above the toxic swamps of Shadowfen. Covered walkways and bridges connect buildings shrouded in perpetual fog. The Witch Council meets in the Tower of Thorns. Alchemists brew powerful potions from rare swamp ingredients.",
            "tags": ["shadowmire", "capital", "city", "shadowfen", "swamp", "witch", "fog", "stilts"],
            "importance": 8,
            "related": ["shadowfen", "witch_council"]
        },
        {
            "id": "crystalspire",
            "title": "Crystalspire",
            "content": "Crystalspire is the capital of the Crystal Barrens, a city built entirely from crystalline structures that glow with inner light. The Oracle Tower is where Seers gather to meditate and receive visions. High Oracle Lysandra Stargazer leads the council of diviners. The Crystal Market sells enchanted gems and focusing crystals.",
            "tags": ["crystalspire", "capital", "city", "crystal", "barrens", "oracle", "divination", "tower"],
            "importance": 9,
            "related": ["crystal_barrens", "high_oracle_lysandra"]
        },
        {
            "id": "twilight_citadel",
            "title": "Twilight Citadel",
            "content": "The Twilight Citadel is the capital of ShadowVoid, a fortress city that exists partially between dimensions. Obsidian spires pierce the purple-grey sky. The Necropolis houses the Academy of Death where Necromancers study. Lord Malachar the Eternal rules as the immortal king of shadows.",
            "tags": ["twilight", "citadel", "capital", "city", "shadowvoid", "necromancer", "void", "fortress"],
            "importance": 9,
            "related": ["shadowvoid", "lord_malachar"]
        },
        {
            "id": "deepholm",
            "title": "Deepholm",
            "content": "Deepholm is the capital of Deepforge, a massive underground city carved from living rock. Lava channels provide light and heat throughout the caverns. The Grand Forge is where legendary weapons are crafted. King Thorin Ironhammer rules from the Stone Throne, maintaining ancient dwarven traditions.",
            "tags": ["deepholm", "capital", "city", "deepforge", "underground", "dwarves", "forge", "king"],
            "importance": 9,
            "related": ["deepforge", "king_thorin"]
        },
        {
            "id": "skyhold",
            "title": "Skyhold",
            "content": "Skyhold is the capital of Skyreach Mountains, built on a massive floating sky island. Monasteries cling to cliff faces connected by suspension bridges. The Gearwheel Monastery houses Monks who blend martial arts with mechanical precision. Grandmaster Kai Stormfist leads the monastic order.",
            "tags": ["skyhold", "capital", "city", "skyreach", "floating", "monk", "monastery", "sky"],
            "importance": 8,
            "related": ["skyreach_mountains", "grandmaster_kai"]
        }
    ]

    for city in cities:
        entries.append(create_lore_entry(
            id=city["id"],
            title=city["title"],
            category="Location",
            content=city["content"],
            tags=city["tags"],
            importance=city["importance"],
            related_entries=city.get("related", [])
        ))

    # Major Factions (7 factions)
    factions = [
        {
            "id": "ironclad_alliance",
            "title": "The Ironclad Alliance",
            "content": "The Ironclad Alliance is a coalition formed between the Ironclad Empire and the Emberlands to promote technological advancement and industrial power. Emperor Garrick Steelarm and Warlord Flamefist signed the pact during the siege of Ironhold. The alliance values innovation, progress, and the fusion of magic with technology. They oppose Verdantpeak's nature-first philosophy.",
            "tags": ["ironclad", "alliance", "faction", "technology", "industry", "empire", "emberlands"],
            "importance": 10,
            "related": ["ironclad_empire", "emberlands", "emperor_garrick"]
        },
        {
            "id": "sylvan_concord",
            "title": "The Sylvan Concord",
            "content": "The Sylvan Concord is an alliance of nature-focused regions including Verdantpeak, Shadowfen, and the Hollow Forests. They believe in living in harmony with nature and protecting ancient forests from industrial expansion. The concord is led by a council of druids and witches. They are in direct opposition to the Ironclad Alliance.",
            "tags": ["sylvan", "concord", "faction", "nature", "forest", "druids", "verdantpeak"],
            "importance": 10,
            "related": ["verdantpeak", "shadowfen", "hollow_forests", "lunara_covenant"]
        },
        {
            "id": "league_of_sands",
            "title": "The League of Sands",
            "content": "The League of Sands is a trade confederation of desert cities including Sunspire and other settlements in the Whispering Sands and Blazing Frontier. Sultan Azir al-Rashid leads the league which controls vital trade routes across the desert. They remain neutral in most conflicts, profiting from trade with all factions.",
            "tags": ["league", "sands", "faction", "desert", "trade", "merchants", "neutral"],
            "importance": 9,
            "related": ["whispering_sands", "blazing_frontier", "sultan_azir"]
        },
        {
            "id": "maritime_sovereignty",
            "title": "The Maritime Sovereignty",
            "content": "The Maritime Sovereignty is a naval alliance between the Sunken Isles, Glimmering Archipelago, and Forgotten Depths. They control the seas and protect shipping lanes from pirates. The faction is led by a council of admirals and island governors. They maintain dominance over all oceanic trade and exploration.",
            "tags": ["maritime", "sovereignty", "faction", "ocean", "naval", "islands", "sea"],
            "importance": 9,
            "related": ["sunken_isles", "glimmering_archipelago", "forgotten_depths", "oceana_covenant"]
        },
        {
            "id": "highland_compact",
            "title": "The Highland Compact",
            "content": "The Highland Compact is an alliance of mountainous and elevated regions including Frosthold, Skyreach Mountains, and Winterguard. They value strength, honor, and martial prowess. The compact is led by warrior-chiefs and defends against threats from both land and sky. They remain independent but trade with most factions.",
            "tags": ["highland", "compact", "faction", "mountains", "warriors", "frosthold", "skyreach"],
            "importance": 8,
            "related": ["frosthold", "skyreach_mountains", "winterguard"]
        },
        {
            "id": "arcane_coalition",
            "title": "The Arcane Coalition",
            "content": "The Arcane Coalition is a league of magic-focused regions including the Mystic Canyons, Crystal Barrens, and Eternal Twilight (ShadowVoid). They pursue magical knowledge above all else and share arcane research. The coalition is governed by a council of archmages. They maintain neutrality but possess devastating magical power.",
            "tags": ["arcane", "coalition", "faction", "magic", "wizards", "research", "knowledge"],
            "importance": 9,
            "related": ["mystic_canyons", "crystal_barrens", "shadowvoid", "celestis_arcanum"]
        },
        {
            "id": "polar_alliance",
            "title": "The Polar Alliance",
            "content": "The Polar Alliance is a defensive pact between Frosthold and Winterguard against threats from the frozen north. Ice Mages and Barbarians work together to defend the realm from frost giants, ice dragons, and other arctic threats. Chieftain Bjorn Frostbeard leads the alliance.",
            "tags": ["polar", "alliance", "faction", "ice", "frosthold", "defense", "north"],
            "importance": 8,
            "related": ["frosthold", "winterguard", "chieftain_bjorn"]
        }
    ]

    for faction in factions:
        entries.append(create_lore_entry(
            id=faction["id"],
            title=faction["title"],
            category="History",
            content=faction["content"],
            tags=faction["tags"],
            importance=faction["importance"],
            related_entries=faction.get("related", [])
        ))

    # Historical Events
    history = [
        {
            "id": "siege_of_ironhold",
            "title": "The Siege of Ironhold",
            "content": "The Siege of Ironhold was a pivotal moment when Emberlands forces attacked the Ironclad Empire's fortress. Emperor Garrick Steelarm and Warlord Flamefist realized their mutual strength and signed the Ironclad Alliance pact during the battle, turning enemies into allies. This alliance has shaped Vystia's politics for generations.",
            "tags": ["siege", "ironhold", "history", "war", "alliance", "ironclad", "emberlands"],
            "importance": 10,
            "related": ["ironclad_alliance", "emperor_garrick", "ironclad_empire"]
        },
        {
            "id": "verdantpeak_ironclad_rivalry",
            "title": "Verdantpeak vs. Ironclad Empire Rivalry",
            "content": "The ancient rivalry between Verdantpeak and the Ironclad Empire stems from fundamental philosophical differences. Verdantpeak believes in living with nature, while the Ironclad Empire seeks to control and industrialize it. This conflict has led to several skirmishes and continues to shape regional politics. The Forest War of 437 resulted in a stalemate and uneasy truce.",
            "tags": ["verdantpeak", "ironclad", "rivalry", "conflict", "nature", "industry", "war"],
            "importance": 9,
            "related": ["verdantpeak", "ironclad_empire", "sylvan_concord"]
        },
        {
            "id": "great_cataclysm",
            "title": "The Great Cataclysm",
            "content": "The Great Cataclysm was a magical disaster 800 years ago that reshaped Vystia's geography. Massive earthquakes, volcanic eruptions, and arcane storms created new regions and destroyed ancient civilizations. The Sunken Isles were created when an entire continent sank beneath the waves. This event is why so many ancient ruins exist throughout Vystia.",
            "tags": ["cataclysm", "disaster", "history", "ancient", "magic", "catastrophe", "ruins"],
            "importance": 10,
            "related": ["sunken_isles"]
        }
    ]

    for event in history:
        entries.append(create_lore_entry(
            id=event["id"],
            title=event["title"],
            category="History",
            content=event["content"],
            tags=event["tags"],
            importance=event["importance"],
            related_entries=event.get("related", [])
        ))

    # Save to JSON
    output_path = LLM_LORE_DIR / "vystia_general.json"
    with open(output_path, 'w', encoding='utf-8') as f:
        json.dump(entries, f, indent=2, ensure_ascii=False)

    print(f"[OK] Generated vystia_general.json ({len(entries)} entries)")
    return len(entries)

def generate_religion_domain():
    """Generate religion_domain.json with all Vystia religions and deities"""
    entries = []

    religions = [
        {
            "id": "cogsmith_creed",
            "title": "Cogsmith Creed",
            "content": "The Cogsmith Creed is the dominant religion in the Ironclad Empire and Emberlands. Followers believe in a clockwork god called The Great Machinist who maintains cosmic order through precision and innovation. The religion emphasizes technological progress, craftsmanship, and the belief that the universe operates like a vast machine. Temples feature intricate clockwork mechanisms and steam-powered organs.",
            "tags": ["cogsmith", "creed", "religion", "technology", "ironclad", "emberlands", "innovation"],
            "importance": 9,
            "deities": ["great_machinist", "forgemaster"]
        },
        {
            "id": "lunara_covenant",
            "title": "Lunara's Covenant",
            "content": "Lunara's Covenant is practiced in Verdantpeak, Shadowfen, and the Hollow Forests. Followers worship Lunara, the Mother of the Grove, who embodies the forest's life force. The religion emphasizes the cycles of growth, death, and renewal, living in harmony with nature. Druids serve as priests, conducting rituals in sacred groves under moonlight.",
            "tags": ["lunara", "covenant", "religion", "nature", "forest", "verdantpeak", "cycles"],
            "importance": 9,
            "deities": ["lunara", "teron"]
        },
        {
            "id": "surya_sandscript",
            "title": "Surya's Sandscript",
            "content": "Surya's Sandscript is the ancient religion of the Whispering Sands and Blazing Frontier. Followers worship the sun god Surya and believe truth is written in the shifting sands. The religion emphasizes endurance, survival, and finding wisdom in harsh conditions. Temples are built to align with solar events, and priests interpret patterns in sand.",
            "tags": ["surya", "sandscript", "religion", "sun", "desert", "sand", "ancient"],
            "importance": 8,
            "deities": ["surya", "mirage_mother"]
        },
        {
            "id": "frosthelm_faith",
            "title": "Frosthelm Faith",
            "content": "The Frosthelm Faith reveres the elemental spirits of ice and snow in Frosthold and Winterguard. The Frost Father is the ruler of winter's harshness, while the Snow Maiden represents snow's beauty. Followers believe the frozen wastes test one's strength and that surviving winter proves worthiness. Shamans conduct ice rituals and ice sculptors create sacred art.",
            "tags": ["frosthelm", "faith", "religion", "ice", "frost", "frosthold", "winter"],
            "importance": 8,
            "deities": ["frost_father", "snow_maiden"]
        },
        {
            "id": "oceana_covenant",
            "title": "Oceana's Covenant",
            "content": "Oceana's Covenant is practiced in the Sunken Isles, Glimmering Archipelago, and Forgotten Depths. Followers worship Oceana, goddess of the seas, and her court of ocean spirits. The religion emphasizes respecting the ocean's power, protecting marine life, and honoring drowned ancestors. Temples are built on shorelines and underwater grottos.",
            "tags": ["oceana", "covenant", "religion", "ocean", "sea", "water", "islands"],
            "importance": 8,
            "deities": ["oceana", "tidecaller", "abyss_keeper"]
        },
        {
            "id": "celestis_arcanum",
            "title": "Celestis Arcanum",
            "content": "Celestis Arcanum is the mystical faith of the Mystic Canyons and Eternal Twilight. Followers believe magic is a divine force flowing from celestial sources. The religion emphasizes magical study as spiritual practice and cosmic alignment. Archmages serve as priests, conducting rituals involving star patterns and arcane geometry.",
            "tags": ["celestis", "arcanum", "religion", "magic", "arcane", "stars", "celestial"],
            "importance": 8,
            "deities": ["star_weaver", "void_sage"]
        },
        {
            "id": "dogoth_forge",
            "title": "Dogoth's Forge",
            "content": "Dogoth's Forge is the dwarven religion of Deepforge. Followers believe their patron god Dogoth was forged from the fires of the earth by the Wielder of the Hammer. The religion emphasizes craftsmanship, metalworking, and honoring the stone. Every dwarf must craft a masterwork item as a religious rite of passage. Temples are built around sacred forges.",
            "tags": ["dogoth", "forge", "religion", "dwarves", "crafting", "deepforge", "metal"],
            "importance": 8,
            "deities": ["dogoth", "selara"]
        }
    ]

    for religion in religions:
        entries.append(create_lore_entry(
            id=religion["id"],
            title=religion["title"],
            category="History",
            content=religion["content"],
            tags=religion["tags"],
            importance=religion["importance"],
            related_entries=religion.get("deities", [])
        ))

    # Major Deities
    deities = [
        {
            "id": "great_machinist",
            "title": "The Great Machinist",
            "content": "The Great Machinist is the supreme deity of the Cogsmith Creed, patron god of engineers, inventors, and artificers. Depicted as a massive clockwork titan, he is believed to maintain the universe's cosmic machinery. His holy symbol is a golden gear. Prayers are offered before beginning new inventions or repairs.",
            "tags": ["great machinist", "deity", "god", "cogsmith", "technology", "invention"],
            "importance": 8
        },
        {
            "id": "forgemaster",
            "title": "The Forgemaster",
            "content": "The Forgemaster is the god of smiths and metallurgists in the Cogsmith Creed. He teaches the sacred art of metalworking and blesses those who create with fire and forge. His temples house sacred anvils where legendary weapons are crafted. Blacksmiths pray to him before major projects.",
            "tags": ["forgemaster", "deity", "god", "smith", "forge", "metal", "fire"],
            "importance": 7
        },
        {
            "id": "lunara",
            "title": "Lunara, Mother of the Grove",
            "content": "Lunara is the primary goddess of Lunara's Covenant, embodying the forest's life force and the cycles of nature. She appears as a beautiful elf maiden crowned with flowers or as a ancient dryad. Druids are her priests, and sacred groves are her temples. She teaches balance, growth, and renewal.",
            "tags": ["lunara", "deity", "goddess", "nature", "forest", "druid", "growth"],
            "importance": 8
        },
        {
            "id": "teron",
            "title": "Teron, The Stag Protector",
            "content": "Teron is the male consort of Lunara, depicted as a massive white stag or a muscular elf hunter. He is the protector of wild animals and guardian of the deep forest. Rangers and hunters pray to him for success and to honor their prey. He teaches respect for nature's predators.",
            "tags": ["teron", "deity", "god", "stag", "hunter", "protector", "animals"],
            "importance": 7
        },
        {
            "id": "frost_father",
            "title": "The Frost Father",
            "content": "The Frost Father is the harsh god of winter and ice in the Frosthelm Faith. He appears as an ancient orc covered in ice, or as a massive frost giant. He tests mortals with brutal cold to prove their strength. Barbarians and Ice Mages both revere him. His blessing grants resistance to cold.",
            "tags": ["frost father", "deity", "god", "ice", "winter", "cold", "frosthold"],
            "importance": 8
        },
        {
            "id": "snow_maiden",
            "title": "The Snow Maiden",
            "content": "The Snow Maiden is the gentler goddess of snow beauty and winter's quiet moments. She appears as a beautiful woman made of snow and ice, leaving no footprints. She teaches that winter can be beautiful as well as deadly. Artists and poets pray to her for inspiration. Her blessing grants clarity of thought.",
            "tags": ["snow maiden", "deity", "goddess", "snow", "beauty", "winter", "ice"],
            "importance": 7
        },
        {
            "id": "oceana",
            "title": "Oceana, Goddess of the Seas",
            "content": "Oceana is the supreme goddess of all oceans, seas, and waters in Oceana's Covenant. She appears as a beautiful mermaid or as a massive sea serpent. She controls tides, storms, and marine life. Sailors pray to her for safe passage, and summoners call upon her for water magic. She is both nurturing and vengeful.",
            "tags": ["oceana", "deity", "goddess", "ocean", "sea", "water", "storms"],
            "importance": 8
        },
        {
            "id": "surya",
            "title": "Surya, God of the Sun",
            "content": "Surya is the solar deity of the desert regions, representing truth, endurance, and the harsh light that reveals all. He appears as a figure of pure sunlight or as a golden lion. He teaches that truth is found through surviving hardship. His temples align with solar events, and his priests read omens in sand patterns.",
            "tags": ["surya", "deity", "god", "sun", "desert", "light", "truth"],
            "importance": 8
        },
        {
            "id": "dogoth",
            "title": "Dogoth, The Forge God",
            "content": "Dogoth is the dwarven god of the forge, patron of all dwarven craftsmen. He was forged from the fires of creation and teaches that every dwarf must create to find purpose. His sacred forges never go cold. Master crafters receive visions from him showing perfect designs. His blessing grants skill in crafting.",
            "tags": ["dogoth", "deity", "god", "dwarf", "forge", "crafting", "creation"],
            "importance": 8
        }
    ]

    for deity in deities:
        entries.append(create_lore_entry(
            id=deity["id"],
            title=deity["title"],
            category="NPC",
            content=deity["content"],
            tags=deity["tags"],
            importance=deity["importance"]
        ))

    # Save to JSON
    output_path = LLM_LORE_DIR / "religion_domain.json"
    with open(output_path, 'w', encoding='utf-8') as f:
        json.dump(entries, f, indent=2, ensure_ascii=False)

    print(f"[OK] Generated religion_domain.json ({len(entries)} entries)")
    return len(entries)

def generate_class_domain():
    """Generate class_domain.json with all 25 Vystia classes"""
    entries = []

    classes = [
        {
            "id": "barbarian",
            "title": "Barbarian - Frosthold Berserkers",
            "content": "Barbarians from Frosthold are known as Frosthold Berserkers who channel the icy ferocity of their frozen homeland. They are formidable melee warriors who embrace the relentless survival spirit of the tundra. Using ice-enhanced attacks and incredible physical strength, they dominate in close combat. Starting equipment includes fur armor and a greataxe.",
            "tags": ["barbarian", "frosthold", "berserker", "melee", "ice", "warrior", "dps"],
            "stats": "STR:45 DEX:20 INT:15 (Total:80)",
            "importance": 7
        },
        {
            "id": "beastmaster",
            "title": "Beastmaster - Pack Leaders of Frosthold",
            "content": "Beastmasters from Frosthold are Pack Leaders who form bonds with ice wolves and arctic creatures. They command animal companions in battle and use nature magic to enhance their pets. Their special item is the Beast Whistle which summons animal companions. They excel at pet-based combat and tracking.",
            "tags": ["beastmaster", "frosthold", "pets", "animals", "ranger", "companion"],
            "stats": "STR:25 DEX:35 INT:20 (Total:80)",
            "importance": 7
        },
        {
            "id": "ice_mage",
            "title": "Ice Mage - Frostweavers of Winterguard",
            "content": "Ice Mages from Winterguard are called Frostweavers who specialize in ice and snow magic. They control the battlefield with freezing spells, ice barriers, and devastating cold damage. Their spellbook contains 32 ice spells across 8 circles. They can slow, freeze, and shatter enemies with their mastery of cold magic.",
            "tags": ["ice mage", "winterguard", "frost", "caster", "ice", "magic", "cold"],
            "stats": "STR:15 DEX:20 INT:45 (Total:80)",
            "importance": 7
        },
        {
            "id": "sorcerer",
            "title": "Sorcerer - Elemental Savants of the Emberlands",
            "content": "Sorcerers from the Emberlands are Elemental Savants who channel raw elemental chaos, especially fire magic. They wield devastating elemental spells with little need for study, drawing power directly from their volcanic homeland. Their spellbook contains 32 elemental spells focusing on fire, lightning, and pure energy.",
            "tags": ["sorcerer", "emberlands", "fire", "elemental", "caster", "chaos", "dps"],
            "stats": "STR:15 DEX:20 INT:45 (Total:80)",
            "importance": 7
        },
        {
            "id": "ranger",
            "title": "Ranger - Dune Striders of the Whispering Sands",
            "content": "Rangers from the Whispering Sands are Dune Striders, expert survivalists and trackers who navigate the desert with ease. They excel at ranged combat with bows, trap-setting, and animal companionship. They can camouflage in desert terrain and track enemies across vast distances. They protect desert caravans from raiders.",
            "tags": ["ranger", "desert", "whispering sands", "ranged", "bow", "tracker", "survival"],
            "stats": "STR:25 DEX:45 INT:10 (Total:80)",
            "importance": 7
        },
        {
            "id": "illusionist",
            "title": "Illusionist - Mirage Weavers of the Whispering Sands",
            "content": "Illusionists from the Whispering Sands are Mirage Weavers who create illusions to protect settlements and confuse enemies. They can create mirror images, turn invisible, alter terrain appearance, and craft terrifying phantasms. Their spellbook contains 32 illusion spells focused on deception and misdirection. They make indispensable scouts.",
            "tags": ["illusionist", "desert", "whispering sands", "illusion", "magic", "deception", "mirages"],
            "stats": "STR:10 DEX:25 INT:45 (Total:80)",
            "importance": 7
        },
        {
            "id": "witch",
            "title": "Witch - Covens of Shadowfen",
            "content": "Witches from Shadowfen form secretive covens that delve into dark magic, curses, and herbal potions. They use the swamp's flora and fauna to craft powerful hexes and protective charms. Their spellbook contains 32 hex spells for debuffing and cursing enemies. They can brew potions, summon familiars, and control poison.",
            "tags": ["witch", "shadowfen", "hex", "curse", "swamp", "poison", "dark"],
            "stats": "STR:15 DEX:20 INT:45 (Total:80)",
            "importance": 7
        },
        {
            "id": "warlock",
            "title": "Warlock - Covenant of the Hidden Eye",
            "content": "Warlocks are members of the Covenant of the Hidden Eye who gain powers through pacts with otherworldly beings. They wield dark magic carefully, walking the line between control and corruption. Their spellbook contains 32 dark magic spells including soul magic, void energy, and demonic summoning. They can drain life and curse enemies.",
            "tags": ["warlock", "shadowvoid", "dark magic", "pact", "void", "demon", "curse"],
            "stats": "STR:15 DEX:20 INT:45 (Total:80)",
            "importance": 7
        },
        {
            "id": "druid",
            "title": "Druid - Guardians of Verdantpeak",
            "content": "Druids from Verdantpeak are revered Guardians who wield nature's power to protect the forest. They can shapeshift into animals, control plants, summon nature spirits, and heal allies. Their spellbook contains 32 nature spells including entangle, wildshape, and earthquake. They maintain balance between civilization and wilderness.",
            "tags": ["druid", "verdantpeak", "nature", "shapeshift", "forest", "healing", "balance"],
            "stats": "STR:20 DEX:25 INT:35 (Total:80)",
            "importance": 7
        },
        {
            "id": "alchemist",
            "title": "Alchemist - Brewmasters of the Verdant Isles",
            "content": "Alchemists from the Verdant Isles are Brewmasters who concoct potent elixirs from rare tropical herbs. They can throw healing or damage potions, create explosive concoctions, deploy gas clouds, and transmute materials. Their Alchemist's Kit serves as a portable crafting station. They create potions of legendary quality.",
            "tags": ["alchemist", "verdant isles", "potions", "brewing", "crafting", "support", "herbs"],
            "stats": "STR:20 DEX:30 INT:30 (Total:80)",
            "importance": 7
        },
        {
            "id": "oracle",
            "title": "Oracle - Seers of the Crystal Barrens",
            "content": "Oracles from the Crystal Barrens are Seers who utilize crystal energies to foresee futures and guide their people. Their spellbook contains 32 divination spells for revealing hidden things, predicting attacks, and manipulating fate. They can detect intentions, recall used abilities, and even reverse time. Their visions are crucial for navigation.",
            "tags": ["oracle", "crystal barrens", "divination", "seer", "future", "vision", "fate"],
            "stats": "STR:10 DEX:20 INT:50 (Total:80)",
            "importance": 7
        },
        {
            "id": "artificer",
            "title": "Artificer - Technoguild Artisans",
            "content": "Artificers from the Ironclad Empire are Technoguild Artisans who craft magical gadgets and mechanical contraptions. They can summon clockwork constructs, deploy turrets, enhance weapons, create shields, and throw bombs. Their Construct Control Device summons scouts (rat, sprite, spider). They blend engineering with magic.",
            "tags": ["artificer", "ironclad", "technology", "constructs", "gadgets", "mechanical", "invention"],
            "stats": "STR:25 DEX:30 INT:25 (Total:80)",
            "importance": 7
        },
        {
            "id": "fighter",
            "title": "Fighter - Ironclad Legionnaires",
            "content": "Fighters from the Ironclad Empire are Ironclad Legionnaires, the backbone of the imperial military. Equipped with heavy armor and advanced weapons, they excel at melee combat, defense, and battlefield tactics. They can form shield walls, execute power strikes, rally allies, and lead charges. They are disciplined warriors.",
            "tags": ["fighter", "ironclad", "warrior", "tank", "melee", "soldier", "legionnaire"],
            "stats": "STR:40 DEX:25 INT:15 (Total:80)",
            "importance": 7
        },
        {
            "id": "monk",
            "title": "Monk - Gearwheel Monastery's Monks",
            "content": "Monks from the Gearwheel Monastery integrate spiritual practice with mechanical precision. They focus on unarmed combat, incredible agility, and chi manipulation. They can deliver flurries of blows, enhance accuracy, increase movement speed, and heal themselves. Monk's Prayer Beads aid their meditation. They are martial artists.",
            "tags": ["monk", "skyreach", "martial arts", "chi", "monastery", "unarmed", "agility"],
            "stats": "STR:30 DEX:35 INT:15 (Total:80)",
            "importance": 7
        },
        {
            "id": "templar",
            "title": "Templar - Justicars of the Ironclad Empire",
            "content": "Templars from the Ironclad Empire are Justicars who blend devotion to law with mechanical prowess. They are holy warriors serving as judge and executioner. They can deliver divine justice, create holy shields, sanctify ground, blind enemies, and revive fallen allies. The Templar's Cross is their holy symbol.",
            "tags": ["templar", "ironclad", "holy", "justice", "paladin", "divine", "law"],
            "stats": "STR:35 DEX:25 INT:20 (Total:80)",
            "importance": 7
        },
        {
            "id": "necromancer",
            "title": "Necromancer - Shadowbinders of Eternal Twilight",
            "content": "Necromancers from Eternal Twilight are Shadowbinders who manipulate life and death energies. Their spellbook contains 32 necromancy spells for raising undead, draining life, causing fear, and instant death. They can create skeletal armies, curse enemies with weakness, and communicate with spirits. They walk the line between life and death.",
            "tags": ["necromancer", "shadowvoid", "undead", "death", "necromancy", "dark", "spirits"],
            "stats": "STR:15 DEX:15 INT:50 (Total:80)",
            "importance": 7
        },
        {
            "id": "summoner",
            "title": "Summoner - Arcane Summoners of the Forgotten Depths",
            "content": "Summoners from the Forgotten Depths explore esoteric magic by calling forth entities from other dimensions. Their spellbook contains 32 summoning spells for conjuring elementals, opening portals, binding creatures, and fusing summons. Their Summoning Circle serves as a focus. They can banish enemies and empower familiars.",
            "tags": ["summoner", "forgotten depths", "summoning", "portal", "elemental", "binding", "dimensional"],
            "stats": "STR:15 DEX:20 INT:45 (Total:80)",
            "importance": 7
        },
        {
            "id": "bounty_hunter",
            "title": "Bounty Hunter - Crypt Trackers of the Sunken Isles",
            "content": "Bounty Hunters from the Sunken Isles are Crypt Trackers who hunt treasures and fugitives. They combine sea magic with tracking skills, using nets, harpoons, and grappling hooks. They can mark targets for double damage, track invisibly, and execute low-health enemies. The Bounty Ledger tracks their targets.",
            "tags": ["bounty hunter", "sunken isles", "tracker", "ranged", "hunter", "sea", "mercenary"],
            "stats": "STR:30 DEX:35 INT:15 (Total:80)",
            "importance": 7
        },
        {
            "id": "knight",
            "title": "Knight - Guardians of the Glimmering Archipelago",
            "content": "Knights from the Glimmering Archipelago are Guardians who protect islands from pirates and sea monsters. They are trained in naval combat and knightly virtues, wielding sword and sorcery. They can charge enemies, block attacks, rally allies, and fight with honor. The Knight's Banner represents their code.",
            "tags": ["knight", "glimmering archipelago", "guardian", "tank", "naval", "honor", "warrior"],
            "stats": "STR:38 DEX:27 INT:15 (Total:80)",
            "importance": 7
        },
        {
            "id": "shaman",
            "title": "Shaman - Spiritcallers of the Wilderlands",
            "content": "Shamans from the Wilderlands are Spiritcallers who bridge the physical and spiritual realms. Their spellbook contains 32 shamanic spells for summoning totems, linking with spirits, revealing paths, and calling elemental bursts. They can heal with totems, see through illusions, and call lightning. The Spirit Totem channels spirit magic.",
            "tags": ["shaman", "wilderlands", "spirits", "totems", "primal", "elemental", "tribal"],
            "stats": "STR:20 DEX:20 INT:40 (Total:80)",
            "importance": 7
        },
        {
            "id": "wizard",
            "title": "Wizard - Scholars of the Arcane Academy",
            "content": "Wizards from the Arcane Academy dedicate themselves to mastering all forms of magic through study. They can cast magic missiles, shields, invisibility, teleportation, and area spells. They are versatile spellcasters with extensive magical knowledge. They can detect magic, summon familiars, and manipulate the fabric of reality.",
            "tags": ["wizard", "arcane academy", "magic", "scholar", "versatile", "study", "spells"],
            "stats": "STR:10 DEX:20 INT:50 (Total:80)",
            "importance": 7
        },
        {
            "id": "cleric",
            "title": "Cleric - Priests of the Elemental Balance",
            "content": "Clerics in Vystia are Priests of the Elemental Balance who uphold spiritual practices worshiping natural forces. They are healers and protectors using divine magic. They can heal wounds, bless allies, smite evil, prevent damage, and resurrect the fallen. The Holy Symbol channels their divine power. They maintain harmony.",
            "tags": ["cleric", "priest", "divine", "healing", "balance", "holy", "protection"],
            "stats": "STR:20 DEX:20 INT:40 (Total:80)",
            "importance": 7
        },
        {
            "id": "paladin",
            "title": "Paladin - Knights of the Forge Pact",
            "content": "Paladins are Knights of the Forge Pact, holy warriors dedicated to justice and righteousness. They wield faith and weapons to defend against evil. They can smite evil creatures, heal allies, protect with auras, charge into battle, and lay on hands. Their Shield of Faith absorbs damage. They are righteous defenders.",
            "tags": ["paladin", "holy", "knight", "divine", "justice", "tank", "healer"],
            "stats": "STR:35 DEX:20 INT:25 (Total:80)",
            "importance": 7
        },
        {
            "id": "bard",
            "title": "Bard - Steam Harmonic Virtuosos",
            "content": "Bards from the Ironclad Empire are Steam Harmonic Virtuosos who blend traditional music with steampunk sounds. Their spellbook contains 32 bardic spells for sonic damage, buffs, confusion, healing, and control. They can stun with cacophony, speed allies, put enemies to sleep, and reveal secrets. The Magic Lute is their instrument.",
            "tags": ["bard", "ironclad", "music", "support", "sonic", "buffs", "performance"],
            "stats": "STR:15 DEX:35 INT:30 (Total:80)",
            "importance": 7
        },
        {
            "id": "enchanter",
            "title": "Enchanter - Sigilmasters of the Mystic Canyons",
            "content": "Enchanters from the Mystic Canyons are Sigilmasters who imbue objects with magic using arcane symbols. Their spellbook contains 32 enchanting spells for creating wards, enhancing weapons/armor, silencing casters, binding enemies, and marking targets. The Enchanting Crystal is their focus. They create magical items.",
            "tags": ["enchanter", "mystic canyons", "enchantment", "runes", "sigils", "buff", "enhancement"],
            "stats": "STR:15 DEX:25 INT:40 (Total:80)",
            "importance": 7
        }
    ]

    for cls in classes:
        entries.append(create_lore_entry(
            id=cls["id"],
            title=cls["title"],
            category="NPC",
            content=cls["content"],
            tags=cls["tags"],
            importance=cls["importance"],
            source=cls.get("stats")
        ))

    # Save to JSON
    output_path = LLM_LORE_DIR / "class_domain.json"
    with open(output_path, 'w', encoding='utf-8') as f:
        json.dump(entries, f, indent=2, ensure_ascii=False)

    print(f"[OK] Generated class_domain.json ({len(entries)} entries)")
    return len(entries)

def generate_magic_domain():
    """Generate magic_domain.json with 12 magic schools and reagents"""
    entries = []

    schools = [
        {
            "id": "ice_magic",
            "title": "Ice Magic School",
            "content": "Ice Magic is the school of frost and cold practiced by Ice Mages in Winterguard. This school focuses on slowing, freezing, and shattering enemies with devastating cold damage. Ice Mages can create ice barriers, summon blizzards, and freeze enemies solid. The school has 32 spells across 8 circles and uses 8 ice-themed reagents including Frozen Heartstone and Glacial Essence.",
            "tags": ["ice", "magic", "cold", "frost", "freeze", "winterguard", "ice mage"],
            "importance": 8
        },
        {
            "id": "nature_magic",
            "title": "Nature Magic School",
            "content": "Nature Magic is the school of plants, animals, and natural forces practiced by Druids in Verdantpeak. This school focuses on healing, shapeshifting, and controlling plants. Druids can entangle enemies in vines, transform into animals, summon nature spirits, and cause earthquakes. The school has 32 spells and uses 8 nature reagents including Living Bark and Moonpetal.",
            "tags": ["nature", "magic", "druid", "plants", "animals", "verdantpeak", "healing"],
            "importance": 8
        },
        {
            "id": "hex_magic",
            "title": "Hex Magic School",
            "content": "Hex Magic is the school of curses, hexes, and dark enchantments practiced by Witches in Shadowfen. This school focuses on debuffing enemies, brewing potions, and inflicting status effects. Witches can curse targets, create voodoo dolls, summon familiars, and create swamp hazards. The school has 32 spells and uses 8 hex reagents including Nightshade Essence and Bog Lotus.",
            "tags": ["hex", "magic", "curse", "witch", "shadowfen", "debuff", "poison"],
            "importance": 8
        },
        {
            "id": "elemental_magic",
            "title": "Elemental Magic School",
            "content": "Elemental Magic is the school of fire, lightning, and raw elemental forces practiced by Sorcerers in the Emberlands. This school focuses on devastating damage through fireballs, lightning bolts, and elemental summons. Sorcerers channel chaotic elemental energy with incredible power. The school has 32 spells and uses 8 elemental reagents including Embercore Crystal and Stormglass Shard.",
            "tags": ["elemental", "magic", "fire", "lightning", "sorcerer", "emberlands", "chaos"],
            "importance": 8
        },
        {
            "id": "dark_magic",
            "title": "Dark Magic School",
            "content": "Dark Magic is the school of shadow, void, and demonic powers practiced by Warlocks in the ShadowVoid. This school focuses on life drain, curses, void shields, and summoning fiends. Warlocks gain power through pacts with otherworldly beings. The school has 32 spells and uses 8 dark reagents including Voidstone Dust and Shadow Wisp Essence.",
            "tags": ["dark", "magic", "shadow", "void", "warlock", "shadowvoid", "demon"],
            "importance": 8
        },
        {
            "id": "divination_magic",
            "title": "Divination Magic School",
            "content": "Divination Magic is the school of prophecy, revelation, and fate practiced by Oracles in the Crystal Barrens. This school focuses on revealing hidden things, predicting the future, and manipulating probability. Oracles can detect enemies, foresee attacks, and even reverse time. The school has 32 spells and uses 8 divination reagents including Crystal Shard and Starlight Dust.",
            "tags": ["divination", "magic", "prophecy", "oracle", "crystal barrens", "future", "vision"],
            "importance": 8
        },
        {
            "id": "necromancy",
            "title": "Necromancy School",
            "content": "Necromancy is the school of death, undead, and life force manipulation practiced by Necromancers in Eternal Twilight. This school focuses on raising undead, draining life, causing fear, and instant death spells. Necromancers can command skeletal armies and communicate with spirits. The school has 32 spells and uses 8 necromancy reagents including Bone Dust and Spirit Essence.",
            "tags": ["necromancy", "magic", "death", "undead", "necromancer", "shadowvoid", "spirits"],
            "importance": 8
        },
        {
            "id": "summoning_magic",
            "title": "Summoning Magic School",
            "content": "Summoning Magic is the school of conjuration, binding, and dimensional travel practiced by Summoners in the Forgotten Depths. This school focuses on summoning creatures from other planes, binding entities, and opening portals. Summoners can call elementals, fuse summons, and banish enemies. The school has 32 spells and uses 8 summoning reagents including Portal Stone and Binding Rune.",
            "tags": ["summoning", "magic", "conjuration", "portal", "summoner", "forgotten depths", "elemental"],
            "importance": 8
        },
        {
            "id": "shamanic_magic",
            "title": "Shamanic Magic School",
            "content": "Shamanic Magic is the school of spirits, totems, and primal forces practiced by Shamans in the Wilderlands. This school focuses on spirit communion, totem summoning, ancestral guidance, and elemental bursts. Shamans bridge the physical and spiritual realms. The school has 32 spells and uses 8 shamanic reagents including Spirit Stone and Totem Wood.",
            "tags": ["shamanic", "magic", "spirits", "totems", "shaman", "wilderlands", "primal"],
            "importance": 8
        },
        {
            "id": "bardic_magic",
            "title": "Bardic Magic School",
            "content": "Bardic Magic is the school of sound, music, and sonic manipulation practiced by Bards in the Ironclad Empire. This school focuses on buffs, crowd control, healing, and sonic damage through magical music. Bards can inspire allies, confuse enemies, and reveal secrets. The school has 32 spells and uses 8 bardic reagents including Harmonic Crystal and Echo Stone.",
            "tags": ["bardic", "magic", "music", "sonic", "bard", "ironclad", "support"],
            "importance": 8
        },
        {
            "id": "enchanting_magic",
            "title": "Enchanting Magic School",
            "content": "Enchanting Magic is the school of sigils, wards, and magical enhancement practiced by Enchanters in the Mystic Canyons. This school focuses on imbuing items with magic, creating protective wards, enhancing weapons/armor, and binding enemies. Enchanters use arcane symbols and runes. The school has 32 spells and uses 8 enchanting reagents including Rune Dust and Sigil Ink.",
            "tags": ["enchanting", "magic", "runes", "sigils", "enchanter", "mystic canyons", "enhancement"],
            "importance": 8
        },
        {
            "id": "illusion_magic",
            "title": "Illusion Magic School",
            "content": "Illusion Magic is the school of deception, invisibility, and phantasms practiced by Illusionists in the Whispering Sands. This school focuses on creating false images, turning invisible, altering terrain appearance, and crafting terrifying illusions. Illusionists can confuse and mislead enemies. The school has 32 spells and uses 8 illusion reagents including Mirage Dust and Phantom Veil.",
            "tags": ["illusion", "magic", "deception", "invisibility", "illusionist", "whispering sands", "mirages"],
            "importance": 8
        }
    ]

    for school in schools:
        entries.append(create_lore_entry(
            id=school["id"],
            title=school["title"],
            category="Magic",
            content=school["content"],
            tags=school["tags"],
            importance=school["importance"]
        ))

    # Magic Theory Entry
    entries.append(create_lore_entry(
        id="vystia_magic_theory",
        title="Vystia Magic Theory",
        category="Magic",
        content="Magic in Vystia is divided into 12 distinct schools, each tied to regional culture and philosophy. Unlike traditional magic systems, Vystia's magic requires specialized reagents unique to each school. Every magic school has 32 spells organized into 8 circles of increasing power. Spellbooks can hold all 32 spells of a school. Magic is taught in regional academies and by specialized trainers.",
        tags=["magic", "theory", "spells", "schools", "reagents", "spellbooks"],
        importance=10
    ))

    # Save to JSON
    output_path = LLM_LORE_DIR / "magic_domain.json"
    with open(output_path, 'w', encoding='utf-8') as f:
        json.dump(entries, f, indent=2, ensure_ascii=False)

    print(f"[OK] Generated magic_domain.json ({len(entries)} entries)")
    return len(entries)

def main():
    """Generate all Vystia lore JSON files"""
    print("=" * 60)
    print("VYSTIA LORE GENERATOR")
    print("=" * 60)
    print(f"Output directory: {LLM_LORE_DIR}")
    print()

    total_entries = 0

    # Generate all domain files
    total_entries += generate_vystia_general()
    total_entries += generate_religion_domain()
    total_entries += generate_class_domain()
    total_entries += generate_magic_domain()

    print()
    print("=" * 60)
    print(f"[OK] COMPLETE! Generated {total_entries} total lore entries")
    print("=" * 60)
    print()
    print("Files created in:")
    print(f"  {LLM_LORE_DIR}")
    print()
    print("Generated files:")
    print("  - vystia_general.json (regions, cities, factions, history)")
    print("  - religion_domain.json (religions and deities)")
    print("  - class_domain.json (all 25 classes)")
    print("  - magic_domain.json (12 magic schools)")
    print()
    print("Next steps:")
    print("  1. Run additional generators for creatures, equipment, NPCs")
    print("  2. Test with [LoreStats] command in-game")
    print("  3. Test with [LoreSearch <query>] command")

if __name__ == "__main__":
    main()
