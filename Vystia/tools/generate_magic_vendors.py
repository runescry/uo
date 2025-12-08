#!/usr/bin/env python3
"""
Generate 12 Magic School Vendors (one per school)
Each vendor sells: reagents for that school + all 32 scrolls + empty spellbook
"""

import os
import re
from pathlib import Path

# Configuration for each magic school
MAGIC_SCHOOLS = {
    "IceMage": {
        "name": "Ice Magic",
        "vendor_title": "ice mage vendor",
        "hue": "0x481",
        "spellbook": "IceMageSpellbook",
        "reagent_file": "IceMagicReagents.cs",
        "scroll_file": "IceMageScrolls.cs"
    },
    "Druid": {
        "name": "Nature Magic",
        "vendor_title": "druid vendor",
        "hue": "0x7D6",
        "spellbook": "DruidSpellbook",
        "reagent_file": "NatureMagicReagents.cs",
        "scroll_file": "DruidScrolls.cs"
    },
    "Witch": {
        "name": "Hex Magic",
        "vendor_title": "witch vendor",
        "hue": "0x81D",
        "spellbook": "WitchSpellbook",
        "reagent_file": "HexMagicReagents.cs",
        "scroll_file": "WitchScrolls.cs"
    },
    "Sorcerer": {
        "name": "Elemental Magic",
        "vendor_title": "sorcerer vendor",
        "hue": "0x54E",
        "spellbook": "SorcererSpellbook",
        "reagent_file": "ElementalMagicReagents.cs",
        "scroll_file": "SorcererScrolls.cs"
    },
    "Warlock": {
        "name": "Dark Magic",
        "vendor_title": "warlock vendor",
        "hue": "0x455",
        "spellbook": "WarlockSpellbook",
        "reagent_file": "DarkMagicReagents.cs",
        "scroll_file": "WarlockScrolls.cs"
    },
    "Oracle": {
        "name": "Divination Magic",
        "vendor_title": "oracle vendor",
        "hue": "0x482",
        "spellbook": "OracleSpellbook",
        "reagent_file": "DivinationMagicReagents.cs",
        "scroll_file": "OracleScrolls.cs"
    },
    "Necromancer": {
        "name": "Necromancy",
        "vendor_title": "necromancer vendor",
        "hue": "0x455",
        "spellbook": "VystiaNecromancerSpellbook",
        "reagent_file": "NecromancyReagents.cs",
        "scroll_file": "NecromancerScrolls.cs"
    },
    "Summoner": {
        "name": "Summoning Magic",
        "vendor_title": "summoner vendor",
        "hue": "0x555",
        "spellbook": "SummonerSpellbook",
        "reagent_file": "SummoningMagicReagents.cs",
        "scroll_file": "SummonerScrolls.cs"
    },
    "Shaman": {
        "name": "Shamanic Magic",
        "vendor_title": "shaman vendor",
        "hue": "0x501",
        "spellbook": "ShamanSpellbook",
        "reagent_file": "ShamanicMagicReagents.cs",
        "scroll_file": "ShamanScrolls.cs"
    },
    "Bard": {
        "name": "Bardic Magic",
        "vendor_title": "bard vendor",
        "hue": "0x8A5",
        "spellbook": "BardSpellbook",
        "reagent_file": "BardicMagicReagents.cs",
        "scroll_file": "BardScrolls.cs"
    },
    "Enchanter": {
        "name": "Enchanting Magic",
        "vendor_title": "enchanter vendor",
        "hue": "0x8FD",
        "spellbook": "EnchanterSpellbook",
        "reagent_file": "EnchantingMagicReagents.cs",
        "scroll_file": "EnchanterScrolls.cs"
    },
    "Illusionist": {
        "name": "Illusion Magic",
        "vendor_title": "illusionist vendor",
        "hue": "0x47E",
        "spellbook": "IllusionistSpellbook",
        "reagent_file": "IllusionMagicReagents.cs",
        "scroll_file": "IllusionistScrolls.cs"
    }
}

REAGENT_DIR = Path("C:/DevEnv/GIT/UO/ServUO/Scripts/Items/Vystia/Resources/Reagents")
SCROLL_DIR = Path("C:/DevEnv/GIT/UO/ServUO/Scripts/Items/Vystia/Scrolls")
OUTPUT_FILE = Path("C:/DevEnv/GIT/UO/ServUO/Scripts/Mobiles/Vystia/Vendors/MagicSchoolVendors.cs")

def extract_reagents(filepath):
    """Extract reagent class names from file"""
    reagents = []
    try:
        with open(filepath, 'r', encoding='utf-8') as f:
            content = f.read()
            pattern = r'public class (\w+)\s*:\s*BaseVystiaReagent'
            reagents = re.findall(pattern, content)
    except Exception as e:
        print(f"[WARN] Could not read {filepath}: {e}")
    return reagents

def extract_scrolls(filepath):
    """Extract scroll class names from file"""
    scrolls = []
    try:
        with open(filepath, 'r', encoding='utf-8') as f:
            content = f.read()
            pattern = r'public class (\w+)\s*:\s*SpellScroll'
            scrolls = re.findall(pattern, content)
    except Exception as e:
        print(f"[WARN] Could not read {filepath}: {e}")
    return scrolls

def format_name(class_name):
    """Convert CamelCase to Title Case with spaces"""
    return re.sub(r'([a-z])([A-Z])', r'\1 \2', class_name)

def get_reagent_price(index):
    """Calculate price based on position (rarity)"""
    if index < 2:
        return 5
    elif index < 4:
        return 8 + (index * 2)
    elif index < 6:
        return 15 + (index * 5)
    else:
        return 40 + ((index - 5) * 20)

def generate_vendor_class(school_key, school_data):
    """Generate a single vendor class for a magic school"""

    # Get reagents
    reagent_file = REAGENT_DIR / school_data['reagent_file']
    reagents = extract_reagents(reagent_file)

    # Get scrolls
    scroll_file = SCROLL_DIR / school_data['scroll_file']
    scrolls = extract_scrolls(scroll_file)

    hue = school_data['hue']

    # Build reagent buy info
    reagent_lines = []
    for i, reagent in enumerate(reagents):
        name = format_name(reagent)
        price = get_reagent_price(i)
        reagent_lines.append(f'            m_BuyInfo.Add(new GenericBuyInfo("{name}", typeof({reagent}), {price}, 999, 0x1F2D, {hue}));')

    # Build scroll buy info
    scroll_lines = []
    for i, scroll in enumerate(scrolls):
        # Extract spell name from scroll class (e.g., "IceBoltScroll" -> "Ice Bolt")
        spell_name = scroll.replace('Scroll', '')
        spell_name = format_name(spell_name)
        # Scrolls priced by circle (4 per circle, 8 circles)
        circle = (i // 4) + 1
        price = 5 + (circle * 5)
        scroll_lines.append(f'            m_BuyInfo.Add(new GenericBuyInfo("{spell_name} Scroll", typeof({scroll}), {price}, 999, 0x1F2D, {hue}));')

    vendor_code = f"""    /// <summary>
    /// {school_data['name']} Vendor - Sells reagents, scrolls, and spellbooks
    /// </summary>
    public class {school_key}Vendor : BaseVendor
    {{
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos {{ get {{ return m_SBInfos; }} }}

        [Constructable]
        public {school_key}Vendor() : base("the {school_data['vendor_title']}")
        {{
            Hue = {hue};
            SetSkill(SkillName.Magery, 100.0);
            SetSkill(SkillName.EvalInt, 100.0);
            SetSkill(SkillName.Inscribe, 100.0);
        }}

        public override void InitSBInfo()
        {{
            m_SBInfos.Add(new SB{school_key}());
        }}

        public {school_key}Vendor(Serial serial) : base(serial) {{ }}

        public override void Serialize(GenericWriter writer)
        {{
            base.Serialize(writer);
            writer.Write((int)0);
        }}

        public override void Deserialize(GenericReader reader)
        {{
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }}
    }}

    public class SB{school_key} : SBInfo
    {{
        private List<GenericBuyInfo> m_BuyInfo = new List<GenericBuyInfo>();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SB{school_key}()
        {{
            // Spellbook (empty)
            m_BuyInfo.Add(new GenericBuyInfo("{school_data['name']} Spellbook", typeof({school_data['spellbook']}), 150, 20, 0x1F2D, {hue}));

            // Reagents ({len(reagents)} total)
{chr(10).join(reagent_lines)}

            // Spell Scrolls ({len(scrolls)} total)
{chr(10).join(scroll_lines)}
        }}

        public override IShopSellInfo SellInfo {{ get {{ return m_SellInfo; }} }}
        public override List<GenericBuyInfo> BuyInfo {{ get {{ return m_BuyInfo; }} }}

        public class InternalSellInfo : GenericSellInfo
        {{
            public InternalSellInfo() {{ }}
        }}
    }}
"""

    return vendor_code, len(reagents), len(scrolls)

def generate_all_vendors():
    """Generate all 12 magic school vendors"""
    print("="*80)
    print("GENERATING MAGIC SCHOOL VENDORS")
    print("="*80)
    print()

    vendor_classes = []
    total_reagents = 0
    total_scrolls = 0

    for school_key, school_data in MAGIC_SCHOOLS.items():
        print(f"[INFO] Generating {school_key}Vendor...")
        code, reagent_count, scroll_count = generate_vendor_class(school_key, school_data)
        vendor_classes.append(code)
        total_reagents += reagent_count
        total_scrolls += scroll_count
        print(f"       - {reagent_count} reagents, {scroll_count} scrolls, 1 spellbook")

    # Generate complete file
    file_content = f"""using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{{
    /// <summary>
    /// Magic School Vendors - One vendor per school
    /// Each sells: reagents, scrolls (32), and empty spellbook for their school
    /// Auto-generated - {len(MAGIC_SCHOOLS)} vendors total
    /// </summary>

{chr(10).join(vendor_classes)}
}}
"""

    # Write file
    print(f"\n[INFO] Writing to: {OUTPUT_FILE}")
    with open(OUTPUT_FILE, 'w', encoding='utf-8') as f:
        f.write(file_content)

    print(f"\n[SUCCESS] Generated {len(MAGIC_SCHOOLS)} magic school vendors!")
    print(f"           Total: {total_reagents} reagents, {total_scrolls} scrolls, {len(MAGIC_SCHOOLS)} spellbooks")
    print("="*80)

    return MAGIC_SCHOOLS.keys()

if __name__ == "__main__":
    vendor_names = generate_all_vendors()
    print("\n[INFO] Vendor classes created:")
    for name in vendor_names:
        print(f"       - {name}Vendor")
    print(f"\n[NEXT] Add these vendors to VystiaCreatureSpawnGump.cs")
