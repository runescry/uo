"""
MASTER LORE GENERATOR

Generates all Vystia lore JSON files for the LLM RAG system.
Calls individual generators and creates remaining domain files.
"""

import json
import subprocess
import sys
from pathlib import Path

# Base paths
UO_ROOT = Path(r"C:\DevEnv\GIT\UO")
LLM_LORE_DIR = UO_ROOT / "ServUO" / "Data" / "Lore"  # Correct: Server looks in ServUO/Data/Lore/

def create_lore_entry(id, title, category, content, tags, importance, related_entries=None):
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
    return entry

def generate_standard_domains():
    """Generate standard domain files that can reuse UO knowledge with Vystia context"""

    # Crafting Domain (supplement to equipment_domain.json)
    crafting_entries = [
        create_lore_entry(
            id="vystia_blacksmithing",
            title="Vystia Blacksmithing",
            content="Blacksmithing in Vystia uses regional ores to create specialized equipment. Frosthold uses Frozen Ore for ice weapons, Emberlands uses Molten Ore for fire weapons, Ironclad uses Steamwork Ore for mechanical armor, and Verdantpeak uses Livingwood for nature-themed equipment. Each region requires different forges and techniques.",
            tags=["blacksmithing", "crafting", "smithing", "forging"],
            category="Crafting",
            importance=7
        ),
        create_lore_entry(
            id="vystia_alchemy",
            title="Vystia Alchemy",
            content="Alchemy in Vystia involves brewing potions from regional ingredients. Verdant Isles Alchemists are the most skilled, using rare tropical herbs. Shadowfen Witches brew powerful hexing potions. Each region has unique alchemical ingredients and recipes.",
            tags=["alchemy", "potions", "brewing", "crafting"],
            category="Crafting",
            importance=6
        ),
        create_lore_entry(
            id="vystia_carpentry",
            title="Vystia Carpentry",
            content="Carpentry in Vystia uses regional woods including Frostwillow from Frosthold, Flamewood from Emberlands, and Livingwood from Verdantpeak. Each wood type has unique properties and is used for different purposes from building to crafting magical staves.",
            tags=["carpentry", "woodworking", "crafting", "lumber"],
            category="Crafting",
            importance=5
        ),
    ]

    output_path = LLM_LORE_DIR / "crafting_domain.json"
    with open(output_path, 'w', encoding='utf-8') as f:
        json.dump(crafting_entries, f, indent=2, ensure_ascii=False)
    print(f"[OK] Generated crafting_domain.json ({len(crafting_entries)} entries)")

    # Combat Domain
    combat_entries = [
        create_lore_entry(
            id="vystia_combat_basics",
            title="Vystia Combat Basics",
            content="Combat in Vystia follows traditional Ultima Online mechanics with Vystia enhancements. Each of the 25 classes has unique combat styles. Barbarians excel at melee DPS, Ice Mages control with freezing spells, Rangers use ranged attacks, and Artificers deploy mechanical constructs. Regional equipment provides bonuses suited to each combat style.",
            tags=["combat", "fighting", "battle", "tactics"],
            category="Combat",
            importance=8
        ),
        create_lore_entry(
            id="vystia_magic_combat",
            title="Vystia Magic Combat",
            content="Magical combat in Vystia uses 12 distinct spell schools. Each school has 32 spells across 8 circles of increasing power. Ice Magic slows and freezes, Fire Magic burns, Nature Magic heals and entangles, Dark Magic drains life, and Necromancy raises undead. Proper reagent management is essential as each school requires 8 unique reagents.",
            tags=["magic", "combat", "spells", "casting"],
            category="Combat",
            importance=8
        ),
    ]

    output_path = LLM_LORE_DIR / "combat_domain.json"
    with open(output_path, 'w', encoding='utf-8') as f:
        json.dump(combat_entries, f, indent=2, ensure_ascii=False)
    print(f"[OK] Generated combat_domain.json ({len(combat_entries)} entries)")

    # Healing Domain
    healing_entries = [
        create_lore_entry(
            id="vystia_healing",
            title="Vystia Healing Arts",
            content="Healing in Vystia is performed by Clerics, Druids, Paladins, and Shamans. Clerics use divine magic to restore health and cure ailments. Druids channel nature's healing power. Paladins lay hands on wounded allies. Shamans summon healing totems. Alchemists brew healing potions from regional herbs.",
            tags=["healing", "medicine", "restoration", "health"],
            category="Healing",
            importance=7
        ),
        create_lore_entry(
            id="vystia_bandages",
            title="Vystia Bandaging",
            content="Bandaging works the same as traditional methods but Vystia healers often infuse bandages with regional ingredients. Frosthold bandages contain frost moss to numb pain. Verdantpeak bandages use healing leaves. Shadowfen bandages include antivenoms for poison treatment.",
            tags=["bandages", "healing", "first aid"],
            category="Healing",
            importance=5
        ),
    ]

    output_path = LLM_LORE_DIR / "healing_domain.json"
    with open(output_path, 'w', encoding='utf-8') as f:
        json.dump(healing_entries, f, indent=2, ensure_ascii=False)
    print(f"[OK] Generated healing_domain.json ({len(healing_entries)} entries)")

    # Trade Domain
    trade_entries = [
        create_lore_entry(
            id="vystia_trade",
            title="Vystia Trade Networks",
            content="Trade in Vystia is dominated by the League of Sands led by Sultan Azir al-Rashid. The desert trade routes connect all regions. The Maritime Sovereignty controls oceanic trade. Each region specializes in different goods: Ironclad exports machinery, Frosthold exports ice-enchanted weapons, Emberlands exports fire weapons, and Verdantpeak exports herbs.",
            tags=["trade", "commerce", "merchants", "economy"],
            category="Trade",
            importance=7
        ),
        create_lore_entry(
            id="vystia_currency",
            title="Vystia Currency",
            content="Vystia uses standard gold pieces but regional currencies exist. The Ironclad Empire mints Steam Crowns featuring clockwork designs. Frosthold uses Frost Marks with ice crystal inlays. The League of Sands uses Sun Discs. All currencies are exchangeable at banks in major cities.",
            tags=["currency", "money", "gold", "coins"],
            category="Trade",
            importance=5
        ),
    ]

    output_path = LLM_LORE_DIR / "trade_domain.json"
    with open(output_path, 'w', encoding='utf-8') as f:
        json.dump(trade_entries, f, indent=2, ensure_ascii=False)
    print(f"[OK] Generated trade_domain.json ({len(trade_entries)} entries)")

    # Hospitality Domain
    hospitality_entries = [
        create_lore_entry(
            id="vystia_inns",
            title="Vystia Inns and Taverns",
            content="Inns and taverns in Vystia reflect regional culture. Frosthold's Ice Hall serves hot mead and roasted meats. Ironhaven's Steam Pub features clockwork entertainment. Verdantheart's Grove Inn offers vegetarian fare in tree-house rooms. Sunspire's Desert Rose has belly dancers and spiced wines. Each provides lodging, food, and local gossip.",
            tags=["inn", "tavern", "lodging", "hospitality", "food"],
            category="Hospitality",
            importance=6
        ),
    ]

    output_path = LLM_LORE_DIR / "hospitality_domain.json"
    with open(output_path, 'w', encoding='utf-8') as f:
        json.dump(hospitality_entries, f, indent=2, ensure_ascii=False)
    print(f"[OK] Generated hospitality_domain.json ({len(hospitality_entries)} entries)")

    # Finance Domain
    finance_entries = [
        create_lore_entry(
            id="vystia_banking",
            title="Vystia Banking System",
            content="Banks in Vystia operate in all major cities. The Iron Bank of Ironhaven is the largest, using clockwork vaults. Frostholm's Ice Vault stores wealth in frozen chambers. Banks provide deposits, withdrawals, and currency exchange. Bankers are trusted members of society and often have knowledge of local economies and trade.",
            tags=["bank", "banking", "finance", "vault"],
            category="Finance",
            importance=6
        ),
    ]

    output_path = LLM_LORE_DIR / "finance_domain.json"
    with open(output_path, 'w', encoding='utf-8') as f:
        json.dump(finance_entries, f, indent=2, ensure_ascii=False)
    print(f"[OK] Generated finance_domain.json ({len(finance_entries)} entries)")

    # Animal Domain
    animal_entries = [
        create_lore_entry(
            id="vystia_animal_training",
            title="Vystia Animal Training",
            content="Animal training in Vystia involves taming regional creatures. Beastmasters from Frosthold specialize in ice wolves and winter bears. Verdantpeak Rangers tame dire wolves and unicorns. Desert Rangers train sand drakes. The Maritime Sovereignty trains sea creatures. Each region has unique tameable creatures with special abilities.",
            tags=["animals", "taming", "pets", "training", "beastmaster"],
            category="Animal",
            importance=6
        ),
    ]

    output_path = LLM_LORE_DIR / "animal_domain.json"
    with open(output_path, 'w', encoding='utf-8') as f:
        json.dump(animal_entries, f, indent=2, ensure_ascii=False)
    print(f"[OK] Generated animal_domain.json ({len(animal_entries)} entries)")

    # Food Domain
    food_entries = [
        create_lore_entry(
            id="vystia_cuisine",
            title="Vystia Regional Cuisine",
            content="Vystia cuisine varies dramatically by region. Frosthold serves preserved meats, ice fish, and hot stews. Emberlands offers spicy fire-roasted dishes and volcanic peppers. Verdantpeak has vegetarian feasts with forest fruits and nuts. Whispering Sands features spiced meats, dates, and flatbreads. Ironclad has mechanically-prepared meals in automated cafeterias.",
            tags=["food", "cooking", "cuisine", "meals"],
            category="Food",
            importance=5
        )
    ]

    output_path = LLM_LORE_DIR / "food_domain.json"
    with open(output_path, 'w', encoding='utf-8') as f:
        json.dump(food_entries, f, indent=2, ensure_ascii=False)
    print(f"[OK] Generated food_domain.json ({len(food_entries)} entries)")

    # Resource Domain
    resource_entries = [
        create_lore_entry(
            id="vystia_mining",
            title="Vystia Mining",
            content="Mining in Vystia yields regional ores with magical properties. Frosthold has Frozen Ore that never melts. Emberlands has Molten Ore that stays hot. Ironclad has Steamwork Ore with clockwork formations. Crystal Barrens has Crystal Ore that glows. Each ore type is used for specific crafting purposes and requires specialized mining techniques.",
            tags=["mining", "ore", "resources", "minerals"],
            category="Resource",
            importance=7
        ),
        create_lore_entry(
            id="vystia_lumberjacking",
            title="Vystia Lumberjacking",
            content="Lumberjacking in Vystia harvests magical woods. Verdantpeak has Livingwood that continues growing after harvest. Frosthold has Frostwillow that resists fire. Emberlands has Flamewood that burns eternally. Each wood type has unique properties for crafting, construction, and magic item creation. Verdantpeak Druids carefully regulate logging to maintain forest balance.",
            tags=["lumberjacking", "wood", "lumber", "resources", "trees"],
            category="Resource",
            importance=6
        ),
    ]

    output_path = LLM_LORE_DIR / "resource_domain.json"
    with open(output_path, 'w', encoding='utf-8') as f:
        json.dump(resource_entries, f, indent=2, ensure_ascii=False)
    print(f"[OK] Generated resource_domain.json ({len(resource_entries)} entries)")

    total = len(crafting_entries) + len(combat_entries) + len(healing_entries) + len(trade_entries) + len(hospitality_entries) + len(finance_entries) + len(animal_entries) + len(food_entries) + len(resource_entries)
    return total

def run_generator(script_name):
    """Run a Python generator script"""
    try:
        result = subprocess.run(
            [sys.executable, script_name],
            cwd=Path(__file__).parent,
            capture_output=True,
            text=True,
            check=True
        )
        print(result.stdout)
        return True
    except subprocess.CalledProcessError as e:
        print(f"[ERROR] Failed to run {script_name}")
        print(e.stdout)
        print(e.stderr)
        return False

def main():
    """Generate all lore files"""
    print("=" * 70)
    print(" " * 15 + "VYSTIA MASTER LORE GENERATOR")
    print("=" * 70)
    print(f"Output directory: {LLM_LORE_DIR}")
    print()
    print("This script generates ALL lore JSON files for the LLM RAG system")
    print()

    total_entries = 0
    successful_files = 0
    total_files = 0

    # Step 1: Run main lore generator
    print("[1/4] Generating core lore (regions, religions, classes, magic)...")
    if run_generator("generate_vystia_lore.py"):
        successful_files += 4
        total_entries += 95  # From first script
    total_files += 4

    # Step 2: Run creatures generator
    print("[2/4] Generating creatures lore...")
    if run_generator("generate_creatures_lore.py"):
        successful_files += 1
        total_entries += 56
    total_files += 1

    # Step 3: Run equipment/NPC generator
    print("[3/4] Generating equipment and NPC lore...")
    if run_generator("generate_equipment_npc_lore.py"):
        successful_files += 2
        total_entries += 29
    total_files += 2

    # Step 4: Generate standard domains
    print("[4/4] Generating standard domain files...")
    standard_count = generate_standard_domains()
    successful_files += 8
    total_files += 8
    total_entries += standard_count

    print()
    print("=" * 70)
    print(f"[OK] GENERATION COMPLETE!")
    print("=" * 70)
    print(f"Files generated: {successful_files}/{total_files}")
    print(f"Total lore entries: {total_entries}")
    print()
    print("Files created in:")
    print(f"  {LLM_LORE_DIR}")
    print()
    print("Generated files:")
    print("  1. vystia_general.json - Regions, cities, factions, history")
    print("  2. religion_domain.json - Religions and deities")
    print("  3. class_domain.json - All 25 character classes")
    print("  4. magic_domain.json - 12 magic schools")
    print("  5. creatures_domain.json - Creatures and monsters")
    print("  6. equipment_domain.json - Weapons, armor, materials")
    print("  7. npc_domain.json - Important NPCs and vendors")
    print("  8. crafting_domain.json - Crafting skills")
    print("  9. combat_domain.json - Combat mechanics")
    print(" 10. healing_domain.json - Healing arts")
    print(" 11. trade_domain.json - Trade and economy")
    print(" 12. hospitality_domain.json - Inns and taverns")
    print(" 13. finance_domain.json - Banking system")
    print(" 14. animal_domain.json - Animal training")
    print(" 15. food_domain.json - Regional cuisine")
    print(" 16. resource_domain.json - Mining and lumberjacking")
    print()
    print("Next steps:")
    print("  1. Build ServUO: dotnet build")
    print("  2. Start server: dotnet run")
    print("  3. Test with [LoreStats] command")
    print("  4. Test with [LoreSearch <query>] command")
    print("  5. Example: [LoreSearch frosthold")
    print("  6. Example: [LoreSearch ice mage")
    print("  7. Example: [LoreSearch frozen ore")

if __name__ == "__main__":
    main()
