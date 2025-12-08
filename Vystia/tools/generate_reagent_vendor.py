#!/usr/bin/env python3
"""
Generate complete VystiaReagentVendor with all actual implemented reagents
"""

import os
import re
from pathlib import Path

# Paths
REAGENT_DIR = Path("C:/DevEnv/GIT/UO/ServUO/Scripts/Items/Vystia/Resources/Reagents")
OUTPUT_FILE = Path("C:/DevEnv/GIT/UO/ServUO/Scripts/Mobiles/Vystia/Vendors/VystiaReagentVendor.cs")

# School configuration (name -> hue)
SCHOOL_CONFIG = {
    "IceMagic": ("Ice Magic", "0x481"),
    "NatureMagic": ("Nature Magic", "0x7D6"),
    "HexMagic": ("Hex Magic", "0x81D"),
    "ElementalMagic": ("Elemental Magic", "0x54E"),
    "DarkMagic": ("Dark Magic", "0x455"),
    "DivinationMagic": ("Divination Magic", "0x482"),
    "Necromancy": ("Necromancy", "0x455"),
    "SummoningMagic": ("Summoning Magic", "0x555"),
    "ShamanicMagic": ("Shamanic Magic", "0x501"),
    "BardicMagic": ("Bardic Magic", "0x8A5"),
    "EnchantingMagic": ("Enchanting Magic", "0x8FD"),
    "IllusionMagic": ("Illusion Magic", "0x47E")
}

def extract_reagents_from_file(filepath):
    """Extract reagent class names from a file"""
    reagents = []
    try:
        with open(filepath, 'r', encoding='utf-8') as f:
            content = f.read()
            pattern = r'public class (\w+)\s*:\s*BaseVystiaReagent'
            reagents = re.findall(pattern, content)
    except Exception as e:
        print(f"[ERROR] Reading {filepath}: {e}")
    return reagents

def format_reagent_name(class_name):
    """Convert CamelCase to Title Case with spaces"""
    # Insert space before uppercase letters
    result = re.sub(r'([a-z])([A-Z])', r'\1 \2', class_name)
    return result

def get_reagent_price(index, total):
    """Calculate reagent price based on rarity (position in list)"""
    # Common reagents (first 2-3): 5-8 gp
    # Uncommon (middle): 15-25 gp
    # Rare (last 2-3): 40-100 gp
    if index < 2:
        return 5
    elif index < 4:
        return 8 + (index * 2)
    elif index < 6:
        return 15 + (index * 5)
    else:
        return 40 + ((index - 5) * 20)

def generate_vendor_code():
    """Generate complete vendor code with all reagents"""
    print("[INFO] Scanning reagent files...")

    all_buy_info = []
    total_reagents = 0

    for school_file, (school_name, hue) in SCHOOL_CONFIG.items():
        filepath = REAGENT_DIR / f"{school_file}Reagents.cs"

        if not filepath.exists():
            print(f"[WARN] File not found: {filepath}")
            continue

        reagents = extract_reagents_from_file(filepath)
        print(f"[INFO] {school_name:20} - {len(reagents)} reagents")

        if not reagents:
            continue

        # Generate buy info for this school
        school_section = f"""
            // ============================================
            // {school_name.upper()} ({len(reagents)} reagents)
            // ============================================"""

        all_buy_info.append(school_section)

        for i, reagent in enumerate(reagents):
            formatted_name = format_reagent_name(reagent)
            price = get_reagent_price(i, len(reagents))
            buy_line = f'            m_BuyInfo.Add(new GenericBuyInfo("{formatted_name}", typeof({reagent}), {price}, 999, 0x1F2D, {hue}));'
            all_buy_info.append(buy_line)

        total_reagents += len(reagents)

    print(f"\n[INFO] Total reagents: {total_reagents}")

    # Generate complete vendor file
    vendor_code = f"""using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{{
    /// <summary>
    /// Vystia Reagent Vendor - Sells all {total_reagents} Vystia magic reagents
    /// Auto-generated from actual implemented reagent classes
    /// </summary>
    public class VystiaReagentVendor : BaseVendor
    {{
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos {{ get {{ return m_SBInfos; }} }}

        [Constructable]
        public VystiaReagentVendor() : base("the reagent merchant")
        {{
            SetSkill(SkillName.Alchemy, 90.0, 100.0);
            SetSkill(SkillName.Magery, 90.0, 100.0);
            SetSkill(SkillName.Inscribe, 80.0, 100.0);
        }}

        public override void InitSBInfo()
        {{
            m_SBInfos.Add(new SBVystiaReagents());
        }}

        public VystiaReagentVendor(Serial serial) : base(serial)
        {{
        }}

        public override void Serialize(GenericWriter writer)
        {{
            base.Serialize(writer);
            writer.Write((int)0); // version
        }}

        public override void Deserialize(GenericReader reader)
        {{
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }}
    }}

    /// <summary>
    /// Sell/Buy info for all {total_reagents} Vystia Reagents
    /// </summary>
    public class SBVystiaReagents : SBInfo
    {{
        private List<GenericBuyInfo> m_BuyInfo = new List<GenericBuyInfo>();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBVystiaReagents()
        {{
{chr(10).join(all_buy_info)}
        }}

        public override IShopSellInfo SellInfo {{ get {{ return m_SellInfo; }} }}
        public override List<GenericBuyInfo> BuyInfo {{ get {{ return m_BuyInfo; }} }}

        public class InternalSellInfo : GenericSellInfo
        {{
            public InternalSellInfo()
            {{
                // Vendors buy reagents back at 50% price
            }}
        }}
    }}
}}
"""

    return vendor_code, total_reagents

def update_vendor_file():
    """Generate and write the vendor file"""
    print("="*80)
    print("GENERATING VYSTIA REAGENT VENDOR")
    print("="*80)
    print()

    code, total = generate_vendor_code()

    # Backup existing file if it exists
    if OUTPUT_FILE.exists():
        backup = OUTPUT_FILE.with_suffix('.cs.backup')
        print(f"\n[INFO] Backing up existing vendor to: {backup}")
        with open(OUTPUT_FILE, 'r', encoding='utf-8') as f:
            with open(backup, 'w', encoding='utf-8') as fb:
                fb.write(f.read())

    # Write new file
    print(f"[INFO] Writing vendor to: {OUTPUT_FILE}")
    with open(OUTPUT_FILE, 'w', encoding='utf-8') as f:
        f.write(code)

    print(f"\n[SUCCESS] Generated VystiaReagentVendor with {total} reagents!")
    print("="*80)

    return total

if __name__ == "__main__":
    total = update_vendor_file()
    print(f"\n[DONE] Vendor sells {total} Vystia magic reagents")
    print("[INFO] Spawn with: [vreag] or [VystiaReagents]")
