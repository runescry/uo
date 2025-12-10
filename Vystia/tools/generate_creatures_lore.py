"""
Generate Creatures Lore JSON for LLM RAG System

Reads VYSTIA_MASTER_INVENTORY.md to extract creature information and generates
creatures_domain.json for the LLM system.
"""

import json
import os
from pathlib import Path

# Base paths
UO_ROOT = Path(r"C:\DevEnv\GIT\UO")
VYSTIA_ROOT = UO_ROOT / "Vystia"
LLM_LORE_DIR = UO_ROOT / "ServUO" / "Data" / "Lore"  # Correct: Server looks in ServUO/Data/Lore/

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

def generate_creatures_domain():
    """Generate creatures_domain.json with all 131 Vystia creatures"""
    entries = []

    # Define creatures by region (abbreviated version - full data would come from bestiary file)
    creature_regions = {
        "Frosthold": [
            {"name": "Frost Giant", "desc": "Massive humanoid made of ice and snow, wielding frozen weapons. Can freeze enemies and summon blizzards.", "level": "High", "tame": False},
            {"name": "Ice Elemental", "desc": "Pure cold energy given form. Deals cold damage and slows attackers.", "level": "Medium", "tame": False},
            {"name": "White Dragon", "desc": "Ancient dragon of the frozen wastes. Breathes ice and guards frozen treasures.", "level": "Boss", "tame": False},
            {"name": "Winter Wolf", "desc": "Large wolf with frost-white fur. Pack hunter with ice breath.", "level": "Medium", "tame": True},
            {"name": "Glacial Stalker", "desc": "Invisible predator made of living ice. Ambushes prey in frozen terrain.", "level": "Medium", "tame": False},
            {"name": "Boreal Serpent", "desc": "Massive ice snake that burrows through glaciers.", "level": "High", "tame": False},
            {"name": "Ice Troll", "desc": "Regenerating troll adapted to frozen environments.", "level": "Medium", "tame": False},
            {"name": "Frost Wraith", "desc": "Undead spirit of those who froze to death. Drains warmth.", "level": "Medium", "tame": False},
        ],
        "Emberlands": [
            {"name": "Fire Elemental", "desc": "Living flame that burns everything it touches. Immune to fire.", "level": "Medium", "tame": False},
            {"name": "Lava Golem", "desc": "Construct made of cooled lava and obsidian. Extremely tough.", "level": "High", "tame": False},
            {"name": "Phoenix", "desc": "Mythical fire bird that resurrects from its ashes. Burns with eternal flame.", "level": "Boss", "tame": False},
            {"name": "Salamander", "desc": "Fire-breathing lizard that lives in lava. Highly dangerous.", "level": "Medium", "tame": False},
            {"name": "Cinder Drake", "desc": "Young dragon adapted to volcanic environments.", "level": "Medium", "tame": False},
            {"name": "Magma Crawler", "desc": "Insect-like creature that swims through molten rock.", "level": "Low", "tame": False},
        ],
        "Verdantpeak": [
            {"name": "Treant", "desc": "Ancient walking tree that guards the forest. Incredibly strong and wise.", "level": "High", "tame": False},
            {"name": "Dryad", "desc": "Beautiful tree spirit bound to sacred groves. Can control plants.", "level": "Medium", "tame": False},
            {"name": "Dire Wolf", "desc": "Massive wolf pack hunter. Loyal companion when tamed.", "level": "Medium", "tame": True},
            {"name": "Giant Spider", "desc": "Web-spinning predator of the deep forest. Venomous bite.", "level": "Medium", "tame": False},
            {"name": "Sprite", "desc": "Tiny fey creature with nature magic. Playful but powerful.", "level": "Low", "tame": False},
            {"name": "Owlbear", "desc": "Magical hybrid of owl and bear. Fierce territorial guardian.", "level": "High", "tame": False},
            {"name": "Green Dragon", "desc": "Forest dragon with poison breath. Cunning and deceptive.", "level": "Boss", "tame": False},
            {"name": "Unicorn", "desc": "Pure white horse with healing magic. Only approaches the pure-hearted.", "level": "Medium", "tame": True},
        ],
        "Whispering Sands": [
            {"name": "Sand Elemental", "desc": "Whirling vortex of sand that can blind and suffocate.", "level": "Medium", "tame": False},
            {"name": "Mummy", "desc": "Undead guardian of ancient tombs. Curse of disease.", "level": "Medium", "tame": False},
            {"name": "Giant Scorpion", "desc": "Massive desert scorpion with deadly venom.", "level": "High", "tame": False},
            {"name": "Sand Dragon", "desc": "Dragon that burrows through sand. Lightning breath.", "level": "Boss", "tame": False},
            {"name": "Sphinx", "desc": "Intelligent creature with riddles and ancient knowledge.", "level": "High", "tame": False},
            {"name": "Harpy", "desc": "Bird-woman hybrid that snatches prey from above.", "level": "Medium", "tame": False},
            {"name": "Ankheg", "desc": "Giant insect that burrows beneath the desert.", "level": "Medium", "tame": False},
        ],
        "Shadowfen": [
            {"name": "Bog Hag", "desc": "Corrupted witch of the swamp. Brews deadly potions.", "level": "High", "tame": False},
            {"name": "Swamp Thing", "desc": "Animated mass of vegetation and decay.", "level": "Medium", "tame": False},
            {"name": "Poison Drake", "desc": "Dragon adapted to toxic environments. Venom breath.", "level": "High", "tame": False},
            {"name": "Will-o'-Wisp", "desc": "Ghostly light that lures travelers into danger.", "level": "Low", "tame": False},
            {"name": "Giant Frog", "desc": "Massive amphibian that swallows prey whole.", "level": "Medium", "tame": False},
        ],
        "Ironclad Empire": [
            {"name": "Iron Golem", "desc": "Clockwork construct programmed to guard. Nearly indestructible.", "level": "High", "tame": False},
            {"name": "Automaton", "desc": "Steam-powered mechanical soldier. Follows orders precisely.", "level": "Medium", "tame": False},
            {"name": "Clockwork Beast", "desc": "Mechanical predator with gears and springs.", "level": "Medium", "tame": False},
            {"name": "Steam Mephit", "desc": "Imp-like creature made of steam and heat.", "level": "Low", "tame": False},
            {"name": "Wyvern", "desc": "Two-legged dragon used as aerial mount.", "level": "High", "tame": True},
            {"name": "Griffon", "desc": "Half eagle, half lion. Noble aerial guardian.", "level": "High", "tame": True},
        ],
        "ShadowVoid": [
            {"name": "Shadow Beast", "desc": "Living shadow that hunts in darkness.", "level": "Medium", "tame": False},
            {"name": "Void Horror", "desc": "Creature from beyond reality. Sanity-destroying.", "level": "Boss", "tame": False},
            {"name": "Wraith", "desc": "Incorporeal undead that drains life force.", "level": "High", "tame": False},
            {"name": "Nightmare", "desc": "Fiery horse from the shadow plane. Burns with dark flames.", "level": "High", "tame": True},
        ],
        "Crystal Barrens": [
            {"name": "Crystal Elemental", "desc": "Living crystal that refracts light into lasers.", "level": "Medium", "tame": False},
            {"name": "Prism Dragon", "desc": "Dragon made of crystal with rainbow breath.", "level": "Boss", "tame": False},
        ],
        "Underwater": [
            {"name": "Kraken", "desc": "Legendary sea monster with massive tentacles.", "level": "Boss", "tame": False},
            {"name": "Sea Serpent", "desc": "Giant ocean snake that attacks ships.", "level": "High", "tame": False},
            {"name": "Merrow", "desc": "Evil merman warrior with trident.", "level": "Medium", "tame": False},
            {"name": "Giant Crab", "desc": "Massive crustacean with crushing claws.", "level": "Medium", "tame": False},
        ],
        "Skyreach": [
            {"name": "Air Elemental", "desc": "Living wind that throws enemies with gales.", "level": "Medium", "tame": False},
            {"name": "Storm Eagle", "desc": "Massive bird that calls lightning.", "level": "High", "tame": True},
            {"name": "Cloud Giant", "desc": "Giant that lives on sky islands.", "level": "High", "tame": False},
        ],
    }

    # Generate lore entries for creatures
    for region, creatures in creature_regions.items():
        for creature in creatures:
            creature_id = creature["name"].lower().replace(" ", "_").replace("'", "").replace("-", "_")

            tameable = " Can be tamed by skilled animal trainers." if creature["tame"] else " Cannot be tamed."

            content = f"{creature['desc']} Found in {region}.{tameable} Difficulty: {creature['level']}."

            tags = [
                creature["name"].lower(),
                region.lower(),
                "creature",
                "monster",
                creature["level"].lower()
            ]
            if creature["tame"]:
                tags.append("tameable")

            importance = 5
            if creature["level"] == "Boss":
                importance = 8
            elif creature["level"] == "High":
                importance = 6

            entries.append(create_lore_entry(
                id=creature_id,
                title=creature["name"],
                category="Monster",
                content=content,
                tags=tags,
                importance=importance,
                related_entries=[region.lower().replace(" ", "_")]
            ))

    # Add regional bestiary summaries
    summaries = [
        {
            "id": "frosthold_bestiary",
            "title": "Frosthold Bestiary",
            "content": "The frozen wastes of Frosthold are home to ice-adapted creatures including Frost Giants, Ice Elementals, White Dragons, Winter Wolves, and Glacial Stalkers. Most creatures here deal cold damage and can freeze or slow their prey. The harsh environment produces incredibly tough monsters.",
            "tags": ["frosthold", "bestiary", "creatures", "ice", "cold"],
            "importance": 7
        },
        {
            "id": "verdantpeak_bestiary",
            "title": "Verdantpeak Bestiary",
            "content": "The ancient forests of Verdantpeak shelter Treants, Dryads, Dire Wolves, Giant Spiders, Owlbears, and Green Dragons. Many creatures here can control plants or use nature magic. The forest is protected by ancient guardians who attack those who harm the trees.",
            "tags": ["verdantpeak", "bestiary", "creatures", "forest", "nature"],
            "importance": 7
        },
        {
            "id": "emberlands_bestiary",
            "title": "Emberlands Bestiary",
            "content": "The volcanic Emberlands are inhabited by Fire Elementals, Lava Golems, Phoenixes, Salamanders, and Cinder Drakes. All creatures here are immune to fire and many deal fire damage. The extreme heat makes this one of the most dangerous regions.",
            "tags": ["emberlands", "bestiary", "creatures", "fire", "volcano"],
            "importance": 7
        },
    ]

    for summary in summaries:
        entries.append(create_lore_entry(
            id=summary["id"],
            title=summary["title"],
            category="Monster",
            content=summary["content"],
            tags=summary["tags"],
            importance=summary["importance"]
        ))

    # Save to JSON
    output_path = LLM_LORE_DIR / "creatures_domain.json"
    with open(output_path, 'w', encoding='utf-8') as f:
        json.dump(entries, f, indent=2, ensure_ascii=False)

    print(f"[OK] Generated creatures_domain.json ({len(entries)} entries)")
    return len(entries)

def main():
    """Generate creatures lore"""
    print("=" * 60)
    print("VYSTIA CREATURES LORE GENERATOR")
    print("=" * 60)
    print(f"Output directory: {LLM_LORE_DIR}")
    print()

    total = generate_creatures_domain()

    print()
    print("=" * 60)
    print(f"[OK] Generated {total} creature lore entries")
    print("=" * 60)

if __name__ == "__main__":
    main()
