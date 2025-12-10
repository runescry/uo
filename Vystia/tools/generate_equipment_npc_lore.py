"""
Generate Equipment and NPC Lore JSON Files

Reads Vystia equipment and NPC documentation to create comprehensive lore entries.
"""

import json
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

def generate_equipment_domain():
    """Generate equipment_domain.json with all Vystia equipment"""
    entries = []

    # Regional Weapons (abbreviated - would be expanded from EQUIPMENT.md)
    weapon_types = {
        "Frosthold": {
            "Froststeel Longsword": "Longsword forged from Frosthold's Frozen Ore, enchanted with ice magic. Deals cold damage and can freeze enemies. Popular among Frosthold Berserkers.",
            "Glacial Battleaxe": "Heavy battleaxe made from Glacial Ingots. Cleaves through armor while chilling foes. Favored by Barbarians.",
            "Icebound War Mace": "Mace embedded with permafrost shards. Crushes enemies and slows their movement.",
        },
        "Emberlands": {
            "Molten Longsword": "Sword forged in volcanic heat from Molten Ore. Burns with inner fire and deals fire damage. Preferred by Sorcerers.",
            "Emberforged Greatsword": "Massive two-handed blade that glows red-hot. Leaves trails of flame with each swing.",
            "Lava Mace": "Mace crafted from cooled lava rock and Emberforged Ingots. Ignites enemies on impact.",
        },
        "Verdantpeak": {
            "Living Wood Staff": "Staff carved from living trees that continues to grow. Channels nature magic and can entangle enemies.",
            "Thornbound Spear": "Spear wrapped in enchanted thorns. Poisons enemies and can be used by Druids.",
            "Natureforged Bow": "Bow crafted from Livingwood with magical properties. Arrows seek their target.",
        },
        "Ironclad": {
            "Steamwork Longsword": "Mechanical sword powered by steam pressure. Can be charged for explosive strikes.",
            "Clockwork Mace": "Mace with internal gears that increase impact force. Favored by Artificers.",
            "Gear-Linked Warhammer": "Massive hammer with clockwork mechanisms. Used by Fighters and Templars.",
        },
    }

    for region, weapons in weapon_types.items():
        for weapon_name, weapon_desc in weapons.items():
            weapon_id = weapon_name.lower().replace(" ", "_")
            entries.append(create_lore_entry(
                id=weapon_id,
                title=weapon_name,
                category="Crafting",
                content=f"{weapon_desc} Crafted in {region} using regional materials.",
                tags=[weapon_name.lower(), region.lower(), "weapon", "equipment", "crafting"],
                importance=5,
                related_entries=[region.lower().replace(" ", "_")]
            ))

    # Legendary Weapons
    legendary_weapons = [
        {
            "id": "the_eternal_winter",
            "title": "The Eternal Winter",
            "content": "Legendary greatsword forged by the Frost Father himself. This massive blade radiates absolute cold and can freeze entire battlefields. Wielded by the greatest champions of Frosthold. Said to have been used to slay a fire dragon during the ancient wars. Grants immunity to cold and massive frost damage bonuses.",
            "tags": ["eternal winter", "legendary", "weapon", "frosthold", "ice", "greatsword"],
            "importance": 9
        },
        {
            "id": "phoenix_ascension",
            "title": "Phoenix Ascension",
            "content": "Legendary staff carved from the heartwood of the eternal flame tree. This staff channels the power of a captured Phoenix and grants the wielder incredible fire magic. Used by the Archmage Pyrus Ashborn. Can resurrect the wielder once per day. Grants immunity to fire and summons fire elementals.",
            "tags": ["phoenix ascension", "legendary", "weapon", "emberlands", "fire", "staff"],
            "importance": 9
        },
    ]

    for weapon in legendary_weapons:
        entries.append(create_lore_entry(
            id=weapon["id"],
            title=weapon["title"],
            category="Crafting",
            content=weapon["content"],
            tags=weapon["tags"],
            importance=weapon["importance"]
        ))

    # Regional Armor Sets
    armor_info = {
        "Frostforged Plate": "Heavy plate armor forged from Frozen Ore in Frosthold. Provides excellent protection against cold and physical damage. Worn by elite Barbarian warriors. Ice blue coloring with frost patterns.",
        "Emberforged Plate": "Plate armor forged in the volcanic forges of the Emberlands. Glows with inner heat and provides fire resistance. Favored by Emberlands warriors. Orange-red coloring.",
        "Steamwork Plate": "Mechanical plate armor powered by steam. Includes clockwork joints for enhanced mobility. Created by Ironclad artificers. Metallic silver with brass fittings.",
        "Natureforged Leather": "Leather armor created from living bark and enchanted hides. Provides nature magic bonuses and camouflage in forests. Worn by Verdantpeak Rangers and Druids. Forest green coloring.",
    }

    for armor_name, armor_desc in armor_info.items():
        armor_id = armor_name.lower().replace(" ", "_")
        entries.append(create_lore_entry(
            id=armor_id,
            title=armor_name,
            category="Crafting",
            content=armor_desc,
            tags=[armor_name.lower(), "armor", "equipment", "crafting"],
            importance=5
        ))

    # Crafting Materials
    materials = [
        {
            "id": "frozen_ore",
            "title": "Frozen Ore",
            "content": "Rare ore found only in the deepest ice caves of Frosthold. Never melts and radiates cold. Used to forge ice-enchanted weapons and armor. Miners must use special insulated tools to extract it. Key ingredient in Frostforged equipment.",
            "tags": ["frozen ore", "ore", "mining", "frosthold", "material", "crafting"],
            "importance": 6
        },
        {
            "id": "molten_ore",
            "title": "Molten Ore",
            "content": "Ore extracted from volcanic vents in the Emberlands. Remains hot to the touch even after cooling. Used to forge fire-enchanted weapons and armor. Must be cooled in lava to maintain properties. Key ingredient in Emberforged equipment.",
            "tags": ["molten ore", "ore", "mining", "emberlands", "material", "crafting"],
            "importance": 6
        },
        {
            "id": "steamwork_ore",
            "title": "Steamwork Ore",
            "content": "Mechanical ore found in the Ironclad Empire's deep mines. Contains natural clockwork formations. Used to create steam-powered equipment and constructs. Extracted by mining engineers. Key ingredient in Clockwork equipment.",
            "tags": ["steamwork ore", "ore", "mining", "ironclad", "material", "crafting"],
            "importance": 6
        },
    ]

    for material in materials:
        entries.append(create_lore_entry(
            id=material["id"],
            title=material["title"],
            category="Crafting",
            content=material["content"],
            tags=material["tags"],
            importance=material["importance"]
        ))

    # Save to JSON
    output_path = LLM_LORE_DIR / "equipment_domain.json"
    with open(output_path, 'w', encoding='utf-8') as f:
        json.dump(entries, f, indent=2, ensure_ascii=False)

    print(f"[OK] Generated equipment_domain.json ({len(entries)} entries)")
    return len(entries)

def generate_npc_domain():
    """Generate npc_domain.json with important Vystia NPCs"""
    entries = []

    # Major faction leaders
    leaders = [
        {
            "id": "emperor_garrick",
            "title": "Emperor Garrick Steelarm",
            "content": "Emperor of the Ironclad Empire, co-founder of the Ironclad Alliance. Garrick is a visionary leader who signed the historic pact with Warlord Flamefist during the Siege of Ironhold. He believes technology and magic together will bring prosperity to all Vystia. Rules from the Iron Throne in Ironhaven. Known for his strategic genius and pragmatic approach to governance. Follower of the Cogsmith Creed and patron of the Great Machinist.",
            "tags": ["garrick", "emperor", "ironclad", "leader", "alliance", "ironhaven"],
            "importance": 10,
            "role": "Noble"
        },
        {
            "id": "chieftain_bjorn",
            "title": "Chieftain Bjorn Frostbeard",
            "content": "Chieftain of Frosthold and leader of the northern clans. Bjorn rules from the Frost Palace in Frostholm. He is a legendary warrior who has survived countless battles against frost giants and ice dragons. Leads the Polar Alliance defensive pact. Known for his strength, honor, and fierce protection of his people. Follows the Frosthelm Faith and reveres the Frost Father.",
            "tags": ["bjorn", "chieftain", "frosthold", "leader", "barbarian", "frostholm"],
            "importance": 10,
            "role": "Noble"
        },
        {
            "id": "elder_seraphina",
            "title": "Elder Seraphina Leafwhisper",
            "content": "Leader of the Tree Council in Verdantpeak's capital Verdantheart. Seraphina is an ancient elf who has lived for over 500 years. She governs from the Heart Tree and maintains the sacred groves. Leads the Sylvan Concord faction opposing industrial expansion. Master druid who can communicate with all forest creatures. Devoted follower of Lunara's Covenant.",
            "tags": ["seraphina", "elder", "verdantpeak", "leader", "druid", "elf", "verdantheart"],
            "importance": 10,
            "role": "Noble"
        },
        {
            "id": "sultan_azir",
            "title": "Sultan Azir al-Rashid",
            "content": "Sultan of Sunspire and leader of the League of Sands trade confederation. Azir controls the vital trade routes across the desert and maintains neutrality in most conflicts, profiting from trade with all factions. Rules from the Palace of Sun and Sand. Known as the wealthiest merchant prince in Vystia. Shrewd negotiator and master of desert politics. Follows Surya's Sandscript religion.",
            "tags": ["azir", "sultan", "sunspire", "leader", "merchant", "desert", "league"],
            "importance": 9,
            "role": "Merchant"
        },
        {
            "id": "archmage_pyrus",
            "title": "Archmage Pyrus Ashborn",
            "content": "Archmage of the Emberlands and master of elemental fire magic. Pyrus rules from the Magma Citadel in Emberforge. He is the most powerful fire sorcerer in Vystia and wields the legendary staff Phoenix Ascension. Co-founder of the Ironclad Alliance alongside Emperor Garrick. His city's forges produce fire-enchanted weapons of legendary quality. Worships the Forgemaster deity.",
            "tags": ["pyrus", "archmage", "emberlands", "sorcerer", "fire", "emberforge"],
            "importance": 9,
            "role": "Mage"
        },
    ]

    for leader in leaders:
        entries.append(create_lore_entry(
            id=leader["id"],
            title=leader["title"],
            category="NPC",
            content=leader["content"],
            tags=leader["tags"],
            importance=leader["importance"]
        ))

    # Vendor Types
    vendor_types = [
        {
            "id": "vystia_blacksmith",
            "title": "Vystia Blacksmith",
            "content": "Regional blacksmiths in Vystia specialize in crafting equipment from local ores. Frosthold blacksmiths forge ice-enchanted weapons, Emberlands smiths create fire weapons, Ironclad smiths build mechanical armor, and Verdantpeak smiths work with living wood. Each region's blacksmiths have unique techniques passed down through generations.",
            "tags": ["blacksmith", "crafting", "vendor", "weapons", "armor"],
            "importance": 6,
            "role": "Blacksmith"
        },
        {
            "id": "vystia_mage_vendor",
            "title": "Vystia Magic School Vendor",
            "content": "Each magic school in Vystia has specialized vendors who sell reagents, scrolls, and spellbooks. There are 12 magic school vendors: Ice Mage, Druid, Witch, Sorcerer, Warlock, Oracle, Necromancer, Summoner, Shaman, Bard, Enchanter, and Illusionist vendors. Each sells 8 unique reagents, 32 spell scrolls, and one empty spellbook for their school.",
            "tags": ["vendor", "magic", "spells", "reagents", "scrolls", "spellbook"],
            "importance": 7,
            "role": "Mage"
        },
        {
            "id": "vystia_reagent_vendor",
            "title": "Vystia Reagent Vendor",
            "content": "General reagent vendor who sells all 96 Vystia magic reagents from all 12 magic schools. This vendor provides access to the complete set of spell components needed for all magic users in Vystia. Found in major cities like Ironhaven, Frostholm, and Verdantheart. Essential for stocking up on magical supplies.",
            "tags": ["vendor", "reagent", "magic", "supplies"],
            "importance": 7,
            "role": "Merchant"
        },
    ]

    for vendor in vendor_types:
        entries.append(create_lore_entry(
            id=vendor["id"],
            title=vendor["title"],
            category="NPC",
            content=vendor["content"],
            tags=vendor["tags"],
            importance=vendor["importance"]
        ))

    # Save to JSON
    output_path = LLM_LORE_DIR / "npc_domain.json"
    with open(output_path, 'w', encoding='utf-8') as f:
        json.dump(entries, f, indent=2, ensure_ascii=False)

    print(f"[OK] Generated npc_domain.json ({len(entries)} entries)")
    return len(entries)

def main():
    """Generate equipment and NPC lore"""
    print("=" * 60)
    print("VYSTIA EQUIPMENT & NPC LORE GENERATOR")
    print("=" * 60)
    print(f"Output directory: {LLM_LORE_DIR}")
    print()

    total = 0
    total += generate_equipment_domain()
    total += generate_npc_domain()

    print()
    print("=" * 60)
    print(f"[OK] Generated {total} total lore entries")
    print("=" * 60)

if __name__ == "__main__":
    main()
