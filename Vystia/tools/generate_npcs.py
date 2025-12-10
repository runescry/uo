"""
VYSTIA NPC GENERATOR

Generates C# NPC classes from VYSTIA_NPC_DESIGN.md for ServUO.

Generates:
- Vendor NPCs (Bankers, Healers, Guards, Merchants, Trainers)
- Quest Giver NPCs with dialogue
- Talking Creatures (Dragons, Treants, Sphinxes, Avatars)
- Faction Leaders with LLM integration
"""

import json
from pathlib import Path
from typing import List, Dict

# Paths
UO_ROOT = Path(r"C:\DevEnv\GIT\UO")
VYSTIA_ROOT = UO_ROOT / "Vystia"
NPC_OUTPUT_DIR = UO_ROOT / "ServUO" / "Scripts" / "Mobiles" / "Vystia" / "NPCs"

# Ensure output directory exists
NPC_OUTPUT_DIR.mkdir(parents=True, exist_ok=True)

def generate_vendor_template(name: str, body_id: str, hue: int, vendor_type: str,
                             sells_items: List[str], location: str, personality: str) -> str:
    """Generate a vendor NPC with proper ServUO inheritance"""

    class_name = name.replace(" ", "").replace("'", "").replace("-", "")

    return f"""using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{{
    /// <summary>
    /// {name} - {vendor_type} in {location}
    /// Personality: {personality}
    /// </summary>
    public class {class_name} : BaseVendor
    {{
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;

        [Constructable]
        public {class_name}() : base("{name}")
        {{
            // Appearance
            Body = {body_id};
            Hue = {hue};

            // Equipment and appearance setup
            SetupAppearance();
        }}

        private void SetupAppearance()
        {{
            // Add appropriate clothing/equipment based on vendor type
            switch ("{vendor_type}")
            {{
                case "Banker":
                    AddItem(new FancyShirt(Hue));
                    AddItem(new LongPants(Hue));
                    AddItem(new Boots());
                    break;
                case "Healer":
                    AddItem(new Robe(0x47E)); // Blue robe
                    AddItem(new Sandals());
                    break;
                case "Blacksmith":
                    AddItem(new HalfApron(Hue));
                    AddItem(new ShortPants());
                    AddItem(new Boots());
                    AddItem(new SmithHammer());
                    break;
                case "Mage":
                    AddItem(new Robe(Hue));
                    AddItem(new WizardsHat(Hue));
                    AddItem(new Sandals());
                    break;
                case "Guard":
                    AddItem(new PlateChest());
                    AddItem(new PlateLegs());
                    AddItem(new PlateArms());
                    AddItem(new PlateGloves());
                    AddItem(new Longsword());
                    break;
                default:
                    AddItem(new Shirt(Hue));
                    AddItem(new LongPants());
                    AddItem(new Shoes());
                    break;
            }}
        }}

        public override void InitSBInfo()
        {{
            // Initialize vendor buy/sell lists
            // m_SBInfos.Add(new SBVystia{vendor_type}());
        }}

        public {class_name}(Serial serial) : base(serial)
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
}}
"""

def generate_quest_giver_template(name: str, body_id: str, hue: int, location: str,
                                  personality: str, quest_name: str, lore_context: str) -> str:
    """Generate a quest giver NPC with dialogue and LLM integration"""

    class_name = name.replace(" ", "").replace("'", "").replace("-", "")

    return f"""using System;
using Server.Items;
using Server.Engines.Quests;

namespace Server.Mobiles
{{
    /// <summary>
    /// {name} - Quest Giver in {location}
    /// Personality: {personality}
    /// Quest: {quest_name}
    /// </summary>
    public class {class_name} : MondainQuester
    {{
        public override Type[] Quests => new Type[]
        {{
            // typeof({quest_name}Quest)
        }};

        [Constructable]
        public {class_name}() : base("{name}", "Quest Giver")
        {{
            // Appearance
            Body = {body_id};
            Hue = {hue};

            SetupAppearance();
        }}

        private void SetupAppearance()
        {{
            // Quest giver appearance
            AddItem(new Robe({hue}));
            AddItem(new Boots());
            AddItem(new QuarterStaff());
        }}

        public override void InitBody()
        {{
            InitStats(100, 100, 25);

            // LLM Integration Context
            // Lore Context: {lore_context}
        }}

        public {class_name}(Serial serial) : base(serial)
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
}}
"""

def generate_talking_creature_template(name: str, creature_type: str, body_id: str,
                                       hue: int, location: str, personality: str,
                                       age: str, lore_context: str) -> str:
    """Generate a talking creature (Dragon, Treant, Sphinx) with AI dialogue"""

    # Remove articles and special chars, but only " the " as a complete word
    class_name = name.replace(" the ", " ").replace(" ", "").replace("'", "").replace("-", "")

    return f"""using System;
using Server.Items;

namespace Server.Mobiles
{{
    /// <summary>
    /// {name} - Ancient {creature_type}
    /// Location: {location}
    /// Age: {age}
    /// Personality: {personality}
    /// </summary>
    public class {class_name} : BaseCreature
    {{
        [Constructable]
        public {class_name}() : base(AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {{
            Name = "{name}";
            Body = {body_id};
            Hue = {hue};
            BaseSoundID = 0x16A;

            SetStr(800, 1000);
            SetDex(150, 200);
            SetInt(500, 700);

            SetHits(5000, 7000);
            SetMana(3000, 5000);

            SetDamage(25, 35);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Cold, 25);
            SetDamageType(ResistanceType.Energy, 25);

            SetResistance(ResistanceType.Physical, 60, 80);
            SetResistance(ResistanceType.Fire, 50, 70);
            SetResistance(ResistanceType.Cold, 80, 100);
            SetResistance(ResistanceType.Poison, 60, 80);
            SetResistance(ResistanceType.Energy, 70, 90);

            SetSkill(SkillName.MagicResist, 120.0);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Magery, 120.0);
            SetSkill(SkillName.EvalInt, 120.0);

            Fame = 25000;
            Karma = 25000; // Ancient guardian, not hostile

            VirtualArmor = 70;

            Tamable = false;

            // LLM Integration Context
            // Lore: {lore_context}
            // Personality: {personality}
        }}

        public override bool CanTeach => true;
        public override bool PlayerRangeSensitive => false;

        public override void OnSpeech(SpeechEventArgs e)
        {{
            base.OnSpeech(e);

            Mobile from = e.Mobile;

            if (!e.Handled && from.InRange(this, 3))
            {{
                string speech = e.Speech.ToLower();

                // Keyword responses
                if (speech.Contains("hello") || speech.Contains("greetings"))
                {{
                    Say("Greetings, mortal. I am {name}. What brings you to seek my wisdom?");
                    e.Handled = true;
                }}
                else if (speech.Contains("quest"))
                {{
                    Say("I have tasks for those who prove themselves worthy...");
                    e.Handled = true;
                }}
                else if (speech.Contains("lore") || speech.Contains("history"))
                {{
                    Say("{lore_context}");
                    e.Handled = true;
                }}

                // TODO: Integrate with LLM system for dynamic responses
            }}
        }}

        public {class_name}(Serial serial) : base(serial)
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
}}
"""

def generate_faction_leader_template(name: str, title: str, faction: str, body_id: str,
                                     hue: int, location: str, personality: str,
                                     lore_context: str) -> str:
    """Generate a faction leader NPC with full LLM integration"""

    class_name = name.replace(" ", "").replace("'", "").replace("-", "")

    return f"""using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{{
    /// <summary>
    /// {name} - {title}
    /// Faction: {faction}
    /// Location: {location}
    /// Personality: {personality}
    /// </summary>
    public class {class_name} : BaseVendor
    {{
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;

        [Constructable]
        public {class_name}() : base("{title}")
        {{
            Name = "{name}";
            Title = "{title}";
            Body = {body_id};
            Hue = {hue};

            SetupAppearance();

            // High stats for a faction leader
            SetStr(150, 200);
            SetDex(100, 150);
            SetInt(150, 200);

            SetHits(500, 700);
            SetMana(300, 500);

            SetSkill(SkillName.Magery, 100.0);
            SetSkill(SkillName.MagicResist, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.Wrestling, 100.0);

            Fame = 15000;
            Karma = 15000;

            // LLM Integration Context
            // Faction: {faction}
            // Lore: {lore_context}
            // Personality: {personality}
        }}

        private void SetupAppearance()
        {{
            // Leader appearance - regal clothing
            AddItem(new FancyShirt({hue}));
            AddItem(new LongPants({hue}));
            AddItem(new ThighBoots());
            AddItem(new Cloak({hue}));

            // Add appropriate weapon/symbol of office
            // AddItem(new Scepter()); // Custom item
        }}

        public override void InitSBInfo()
        {{
            // Faction leaders don't typically sell items
        }}

        public override bool CanTeach => true;

        public override void OnSpeech(SpeechEventArgs e)
        {{
            base.OnSpeech(e);

            Mobile from = e.Mobile;

            if (!e.Handled && from.InRange(this, 3))
            {{
                string speech = e.Speech.ToLower();

                // Keyword responses - Leader dialogue (regal, authoritative, first-person)
                if (speech.Contains("greetings") || speech.Contains("hail") || speech.Contains("hello"))
                {{
                    Say($"{{GREETING_PLACEHOLDER}}");
                    e.Handled = true;
                }}
                else if (speech.Contains("faction") || speech.Contains("{faction.lower().replace(' ', '')}"))
                {{
                    Say("{{FACTION_PLACEHOLDER}}");
                    e.Handled = true;
                }}
                else if (speech.Contains("{{KEYWORD_PLACEHOLDER}}"))
                {{
                    Say("{{REGIONAL_PLACEHOLDER}}");
                    e.Handled = true;
                }}

                // TODO: Full LLM integration for dynamic dialogue
                // NOTE: Replace placeholder dialogue with proper first-person responses:
                // - Speak AS the leader (first person "I", "my", "we")
                // - Keep responses SHORT (under 100 chars to avoid cutoff)
                // - Use regal/authoritative tone matching personality
                // - Avoid third-person descriptions
            }}
        }}

        public {class_name}(Serial serial) : base(serial)
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
}}
"""

def generate_all_npcs():
    """Generate all NPC classes"""

    print("=" * 70)
    print(" " * 20 + "VYSTIA NPC GENERATOR")
    print("=" * 70)
    print(f"Output directory: {NPC_OUTPUT_DIR}")
    print()

    generated_count = 0

    # Faction Leaders
    print("[1/5] Generating Faction Leaders...")
    faction_leaders = [
        {
            "name": "Emperor Garrick Steelarm",
            "title": "Emperor of the Ironclad Empire",
            "faction": "Ironclad Alliance",
            "body": "0x190",  # Male
            "hue": 2213,
            "location": "Imperial Palace, Ironhaven",
            "personality": "Visionary leader, strategic genius, pragmatic",
            "lore": "Co-founder of Ironclad Alliance who signed pact with Warlord Flamefist during siege of Ironhold. Believes technology and magic together will bring prosperity."
        },
        {
            "name": "Chieftain Bjorn Frostbeard",
            "title": "Chieftain of Frosthold",
            "faction": "Polar Alliance",
            "body": "0x190",
            "hue": 1152,
            "location": "Frost Palace, Frostholm",
            "personality": "Legendary warrior, honorable, fierce protector",
            "lore": "Leader of the northern clans who has survived countless battles against frost giants and ice dragons. Leads the Polar Alliance defensive pact."
        },
        {
            "name": "Elder Seraphina Leafwhisper",
            "title": "Leader of the Tree Council",
            "faction": "Sylvan Concord",
            "body": "0x191",  # Female
            "hue": 2010,
            "location": "Heart Tree, Verdantheart",
            "personality": "Ancient elf, wise, protective of nature",
            "lore": "500-year-old elf who governs from the Heart Tree. Leads the Sylvan Concord faction opposing industrial expansion. Master druid."
        },
        {
            "name": "Sultan Azir al-Rashid",
            "title": "Sultan of Sunspire",
            "faction": "League of Sands",
            "body": "0x190",
            "hue": 1719,
            "location": "Palace of Sun and Sand, Sunspire",
            "personality": "Shrewd merchant prince, neutral diplomat",
            "lore": "Leader of the League of Sands trade confederation. Controls vital desert trade routes. Remains neutral in conflicts, profiting from all sides."
        },
        {
            "name": "Archmage Pyrus Ashborn",
            "title": "Archmage of the Emberlands",
            "faction": "Ironclad Alliance",
            "body": "0x190",
            "hue": 1358,
            "location": "Magma Citadel, Emberforge",
            "personality": "Powerful fire sorcerer, ambitious, innovative",
            "lore": "Most powerful fire sorcerer in Vystia who wields Phoenix Ascension. Co-founder of Ironclad Alliance. His forges produce legendary fire-enchanted weapons."
        },
    ]

    for leader in faction_leaders:
        code = generate_faction_leader_template(
            leader["name"], leader["title"], leader["faction"],
            leader["body"], leader["hue"], leader["location"],
            leader["personality"], leader["lore"]
        )

        filename = leader["name"].replace(" ", "").replace("'", "").replace("-", "") + ".cs"
        output_path = NPC_OUTPUT_DIR / "FactionLeaders" / filename
        output_path.parent.mkdir(parents=True, exist_ok=True)

        with open(output_path, 'w', encoding='utf-8') as f:
            f.write(code)

        generated_count += 1

    print(f"[OK] Generated {len(faction_leaders)} faction leaders")

    # Talking Creatures
    print("[2/5] Generating Talking Creatures...")
    talking_creatures = [
        {
            "name": "Frosthelm the Eternal Winter",
            "type": "White Ancient Dragon",
            "body": "0xC",  # Dragon body
            "hue": 1152,
            "location": "Frozen Peak cave, Frosthold",
            "personality": "Ancient, wise, speaks slowly, protective",
            "age": "3000+ years",
            "lore": "Ancient white dragon who witnessed formation of Frosthold. Guardian of eternal ice secrets. Knows Frost Father personally."
        },
        {
            "name": "Elder Oakbark",
            "type": "Ancient Treant",
            "body": "0x2F", # Treant body (custom)
            "hue": 2010,
            "location": "Deep Verdantpeak Forest",
            "personality": "Patient, wise, protective of forest",
            "age": "2000+ years",
            "lore": "Ancient treant who guards the sacred groves of Verdantpeak. Speaks for the forest itself."
        },
        {
            "name": "Sphinx of Surya",
            "type": "Desert Sphinx",
            "body": "0x5F",  # Sphinx body (custom)
            "hue": 1719,
            "location": "Ancient ruins, Whispering Sands",
            "personality": "Riddler, ancient knowledge keeper",
            "age": "5000+ years",
            "lore": "Ancient sphinx who guards forgotten treasures. Tests worthy adventurers with riddles and ancient wisdom."
        },
    ]

    for creature in talking_creatures:
        code = generate_talking_creature_template(
            creature["name"], creature["type"], creature["body"],
            creature["hue"], creature["location"], creature["personality"],
            creature["age"], creature["lore"]
        )

        # Remove articles and special chars, but only " the " as a complete word
        filename = creature["name"].replace(" the ", " ").replace(" ", "").replace("'", "").replace("-", "") + ".cs"
        output_path = NPC_OUTPUT_DIR / "TalkingCreatures" / filename
        output_path.parent.mkdir(parents=True, exist_ok=True)

        with open(output_path, 'w', encoding='utf-8') as f:
            f.write(code)

        generated_count += 1

    print(f"[OK] Generated {len(talking_creatures)} talking creatures")

    # Essential Vendors (Bankers, Healers, Guards)
    print("[3/5] Generating Essential Vendors...")
    essential_vendors = [
        {
            "name": "Ironhaven Banker",
            "body": "0x190",
            "hue": 2213,
            "type": "Banker",
            "sells": ["banking services"],
            "location": "Ironhaven",
            "personality": "Professional, efficient"
        },
        {
            "name": "Frostholm Healer",
            "body": "0x191",
            "hue": 1152,
            "type": "Healer",
            "sells": ["healing", "cures", "resurrections"],
            "location": "Frostholm",
            "personality": "Compassionate, caring"
        },
        {
            "name": "Ironhaven Guard Captain",
            "body": "0x190",
            "hue": 2213,
            "type": "Guard",
            "sells": [],
            "location": "Ironhaven Gates",
            "personality": "Vigilant, disciplined"
        },
    ]

    for vendor in essential_vendors:
        code = generate_vendor_template(
            vendor["name"], vendor["body"], vendor["hue"],
            vendor["type"], vendor["sells"], vendor["location"],
            vendor["personality"]
        )

        filename = vendor["name"].replace(" ", "") + ".cs"
        output_path = NPC_OUTPUT_DIR / "Vendors" / filename
        output_path.parent.mkdir(parents=True, exist_ok=True)

        with open(output_path, 'w', encoding='utf-8') as f:
            f.write(code)

        generated_count += 1

    print(f"[OK] Generated {len(essential_vendors)} essential vendors")

    # Quest Givers
    print("[4/5] Generating Quest Givers...")
    quest_givers = [
        {
            "name": "Quartermaster Grimwald",
            "body": "0x190",
            "hue": 2213,
            "location": "Ironhaven Barracks",
            "personality": "Gruff military veteran",
            "quest": "SupplyLine",
            "lore": "Veteran quartermaster who oversees supply lines for the Ironclad military."
        },
        {
            "name": "Sage Theron",
            "body": "0x190",
            "hue": 2010,
            "location": "Verdantheart Library",
            "personality": "Scholarly, curious",
            "quest": "AncientTexts",
            "lore": "Elven sage who studies ancient texts and seeks lost knowledge of the old world."
        },
    ]

    for quest_giver in quest_givers:
        code = generate_quest_giver_template(
            quest_giver["name"], quest_giver["body"], quest_giver["hue"],
            quest_giver["location"], quest_giver["personality"],
            quest_giver["quest"], quest_giver["lore"]
        )

        filename = quest_giver["name"].replace(" ", "") + ".cs"
        output_path = NPC_OUTPUT_DIR / "QuestGivers" / filename
        output_path.parent.mkdir(parents=True, exist_ok=True)

        with open(output_path, 'w', encoding='utf-8') as f:
            f.write(code)

        generated_count += 1

    print(f"[OK] Generated {len(quest_givers)} quest givers")

    # Create NPC Spawn Commands file
    print("[5/5] Generating NPC Spawn Commands...")
    generate_spawn_commands(generated_count)

    print()
    print("=" * 70)
    print(f"[OK] GENERATION COMPLETE!")
    print("=" * 70)
    print(f"Generated {generated_count} NPC classes")
    print()
    print(f"Output directory: {NPC_OUTPUT_DIR}")
    print()
    print("Generated categories:")
    print("  - FactionLeaders/ (5 NPCs)")
    print("  - TalkingCreatures/ (3 NPCs)")
    print("  - Vendors/ (3 NPCs)")
    print("  - QuestGivers/ (2 NPCs)")
    print()
    print("Next steps:")
    print("  1. Build ServUO: dotnet build")
    print("  2. Test spawning: [add EmperorGarrickSteelarm")
    print("  3. Expand with more NPCs from VYSTIA_NPC_DESIGN.md")

def generate_spawn_commands(count):
    """Generate GM commands file for easy NPC spawning"""
    commands_code = f"""using Server;
using Server.Commands;

namespace Server.Mobiles
{{
    /// <summary>
    /// GM Commands for spawning Vystia NPCs
    /// Generated: {count} NPCs
    /// </summary>
    public class VystiaNPCCommands
    {{
        public static void Initialize()
        {{
            CommandSystem.Register("SpawnVystiaLeader", AccessLevel.GameMaster, SpawnLeader_OnCommand);
            CommandSystem.Register("SpawnVystiaCreature", AccessLevel.GameMaster, SpawnCreature_OnCommand);
        }}

        [Usage("SpawnVystiaLeader <name>")]
        [Description("Spawns a Vystia faction leader at cursor location")]
        private static void SpawnLeader_OnCommand(CommandEventArgs e)
        {{
            if (e.Length < 1)
            {{
                e.Mobile.SendMessage("Usage: [SpawnVystiaLeader <name>");
                e.Mobile.SendMessage("Available: Garrick, Bjorn, Seraphina, Azir, Pyrus");
                return;
            }}

            string name = e.GetString(0).ToLower();
            Mobile npc = null;

            switch (name)
            {{
                case "garrick":
                    npc = new EmperorGarrickSteelarm();
                    break;
                case "bjorn":
                    npc = new ChieftainBjornFrostbeard();
                    break;
                case "seraphina":
                    npc = new ElderSeraphinaLeafwhisper();
                    break;
                case "azir":
                    npc = new SultanAziralRashid();
                    break;
                case "pyrus":
                    npc = new ArchmagePyrusAshborn();
                    break;
                default:
                    e.Mobile.SendMessage("Unknown leader: {{0}}", name);
                    return;
            }}

            if (npc != null)
            {{
                npc.MoveToWorld(e.Mobile.Location, e.Mobile.Map);
                e.Mobile.SendMessage("Spawned {{0}}", npc.Name);
            }}
        }}

        [Usage("SpawnVystiaCreature <name>")]
        [Description("Spawns a talking creature at cursor location")]
        private static void SpawnCreature_OnCommand(CommandEventArgs e)
        {{
            if (e.Length < 1)
            {{
                e.Mobile.SendMessage("Usage: [SpawnVystiaCreature <name>");
                e.Mobile.SendMessage("Available: Frosthelm, Oakbark, Sphinx");
                return;
            }}

            string name = e.GetString(0).ToLower();
            BaseCreature creature = null;

            switch (name)
            {{
                case "frosthelm":
                    creature = new FrosthelmEternalWinter();
                    break;
                case "oakbark":
                    creature = new ElderOakbark();
                    break;
                case "sphinx":
                    creature = new SphinxofSurya();
                    break;
                default:
                    e.Mobile.SendMessage("Unknown creature: {{0}}", name);
                    return;
            }}

            if (creature != null)
            {{
                creature.MoveToWorld(e.Mobile.Location, e.Mobile.Map);
                e.Mobile.SendMessage("Spawned {{0}}", creature.Name);
            }}
        }}
    }}
}}
"""

    output_path = NPC_OUTPUT_DIR / "VystiaNPCCommands.cs"
    with open(output_path, 'w', encoding='utf-8') as f:
        f.write(commands_code)

    print("[OK] Generated NPC spawn commands")

if __name__ == "__main__":
    generate_all_npcs()
