"""
VYSTIA QUEST GENERATOR

Generates ServUO quest classes for Vystia NPCs.
Creates properly structured quests with objectives, rewards, and dialogue.
"""

import json
from pathlib import Path

# Base paths
UO_ROOT = Path(r"C:\DevEnv\GIT\UO")
QUEST_OUTPUT_DIR = UO_ROOT / "ServUO" / "Scripts" / "Quests" / "Vystia"

def generate_slay_quest_template(class_name: str, title: str, description: str,
                                  target_type: str, target_count: int,
                                  reward_gold: int, reward_item: str = None) -> str:
    """Generate a slay quest (kill X creatures)"""

    reward_code = f'AddReward(new BaseReward(typeof({reward_item}), "{reward_item}"));' if reward_item else f'AddReward(new BaseReward({reward_gold})); // {reward_gold} gold'

    return f"""using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{{
    public class {class_name} : BaseQuest
    {{
        public {class_name}()
            : base()
        {{
            AddObjective(new SlayObjective(typeof({target_type}), "{target_type}", {target_count}));

            {reward_code}
        }}

        public override object Title => "{title}";

        public override object Description =>
            "{description}";

        public override object Refuse =>
            "I understand. These creatures are dangerous. Return if you change your mind.";

        public override object Uncomplete =>
            "You haven't completed your task yet. Return when you've slain all {target_count} {target_type}s.";

        public override object Complete =>
            "Well done! You've proven your strength. Here is your reward.";

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

def generate_deliver_quest_template(class_name: str, title: str, description: str,
                                     item_type: str, item_name: str, item_count: int,
                                     npc_type: str, npc_name: str,
                                     reward_gold: int, reward_item: str = None) -> str:
    """Generate a delivery quest (bring X items to NPC)"""

    reward_code = f'AddReward(new BaseReward(typeof({reward_item}), "{reward_item}"));' if reward_item else f'AddReward(new BaseReward({reward_gold})); // {reward_gold} gold'

    return f"""using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{{
    public class {class_name} : BaseQuest
    {{
        public {class_name}()
            : base()
        {{
            AddObjective(new DeliverObjective(typeof({item_type}), "{item_name}", {item_count}, typeof({npc_type}), "{npc_name}"));

            {reward_code}
        }}

        public override object Title => "{title}";

        public override object Description =>
            "{description}";

        public override object Refuse =>
            "Very well. The supplies can wait, I suppose.";

        public override object Uncomplete =>
            "I still need {item_count} {item_name}. Please deliver them to {npc_name}.";

        public override object Complete =>
            "Excellent! The supplies have arrived safely. Thank you for your help.";

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

def generate_obtain_quest_template(class_name: str, title: str, description: str,
                                    item_type: str, item_name: str, item_count: int,
                                    reward_gold: int, reward_item: str = None) -> str:
    """Generate an obtain quest (collect X items)"""

    reward_code = f'AddReward(new BaseReward(typeof({reward_item}), "{reward_item}"));' if reward_item else f'AddReward(new BaseReward({reward_gold})); // {reward_gold} gold'

    return f"""using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{{
    public class {class_name} : BaseQuest
    {{
        public {class_name}()
            : base()
        {{
            AddObjective(new ObtainObjective(typeof({item_type}), "{item_name}", {item_count}));

            {reward_code}
        }}

        public override object Title => "{title}";

        public override object Description =>
            "{description}";

        public override object Refuse =>
            "I see. Perhaps another time then.";

        public override object Uncomplete =>
            "I still need {item_count} {item_name}. Keep searching!";

        public override object Complete =>
            "Perfect! You've gathered everything I needed. Here's your reward.";

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

def main():
    """Generate initial Vystia quests"""
    print("=" * 70)
    print(" " * 20 + "VYSTIA QUEST GENERATOR")
    print("=" * 70)
    print(f"Output directory: {QUEST_OUTPUT_DIR}")
    print()

    # Create output directory
    QUEST_OUTPUT_DIR.mkdir(parents=True, exist_ok=True)

    generated_count = 0

    # Quest 1: Supply Line Quest (for Quartermaster Grimwald)
    quest1 = generate_deliver_quest_template(
        class_name="SupplyLineQuest",
        title="Supply Line",
        description="The Ironclad military needs supplies urgently. I need you to deliver 10 Iron Ingots to Captain Steelhart at the northern outpost. Our supply lines have been disrupted by raiders, and the garrison is running low on equipment repairs.",
        item_type="IronIngot",
        item_name="iron ingot",
        item_count=10,
        npc_type="IronhavenGuardCaptain",
        npc_name="Captain Steelhart",
        reward_gold=1000
    )

    output_path = QUEST_OUTPUT_DIR / "SupplyLineQuest.cs"
    with open(output_path, 'w', encoding='utf-8') as f:
        f.write(quest1)

    generated_count += 1
    print(f"[OK] Generated SupplyLineQuest.cs")

    # Quest 2: Ancient Texts Quest (for Sage Theron)
    quest2 = generate_obtain_quest_template(
        class_name="AncientTextsQuest",
        title="Ancient Texts",
        description="I'm researching the ancient history of Verdantpeak and I need scrolls from the old library ruins. The library was abandoned centuries ago and is now overrun with creatures. Please retrieve 5 Ancient Scrolls for my research. They should be found on the Guardians protecting the ruins.",
        item_type="BlankScroll",
        item_name="ancient scroll",
        item_count=5,
        reward_gold=750
    )

    output_path = QUEST_OUTPUT_DIR / "AncientTextsQuest.cs"
    with open(output_path, 'w', encoding='utf-8') as f:
        f.write(quest2)

    generated_count += 1
    print(f"[OK] Generated AncientTextsQuest.cs")

    # Quest 3: Frost Wolf Hunt (Frosthold regional quest)
    quest3 = generate_slay_quest_template(
        class_name="FrostWolfHuntQuest",
        title="Frost Wolf Hunt",
        description="The frost wolves have been attacking our supply caravans traveling between Frostholm and the southern settlements. Chieftain Bjorn has authorized a bounty on these beasts. Slay 8 Frost Wolves and I'll pay you well for your service to Frosthold.",
        target_type="DireWolf",
        target_count=8,
        reward_gold=1500
    )

    output_path = QUEST_OUTPUT_DIR / "FrostWolfHuntQuest.cs"
    with open(output_path, 'w', encoding='utf-8') as f:
        f.write(quest3)

    generated_count += 1
    print(f"[OK] Generated FrostWolfHuntQuest.cs")

    # Quest 4: Emberlands Fire Elemental Threat
    quest4 = generate_slay_quest_template(
        class_name="FireElementalThreatQuest",
        title="Fire Elemental Threat",
        description="Fire elementals have been spawning near the volcanic vents and threatening our miners. Archmage Pyrus requests that you eliminate 10 Fire Elementals to secure the mining operations. The forges of Emberforge depend on those ores!",
        target_type="FireElemental",
        target_count=10,
        reward_gold=2000
    )

    output_path = QUEST_OUTPUT_DIR / "FireElementalThreatQuest.cs"
    with open(output_path, 'w', encoding='utf-8') as f:
        f.write(quest4)

    generated_count += 1
    print(f"[OK] Generated FireElementalThreatQuest.cs")

    # Quest 5: Verdantpeak Herb Gathering
    quest5 = generate_obtain_quest_template(
        class_name="HerbGatheringQuest",
        title="Sacred Herb Gathering",
        description="The Tree Council needs rare herbs for a healing ritual. I need you to gather 20 Ginseng from the deepest parts of Verdantpeak Forest. Be careful - ancient guardians protect those groves, and they don't take kindly to outsiders.",
        item_type="Ginseng",
        item_name="ginseng",
        item_count=20,
        reward_gold=500
    )

    output_path = QUEST_OUTPUT_DIR / "HerbGatheringQuest.cs"
    with open(output_path, 'w', encoding='utf-8') as f:
        f.write(quest5)

    generated_count += 1
    print(f"[OK] Generated HerbGatheringQuest.cs")

    # Quest 6: Crystal Shard Collection (Crystal Barrens)
    quest6 = generate_obtain_quest_template(
        class_name="CrystalShardQuest",
        title="Crystal Shard Collection",
        description="The energy crystals of the Crystal Barrens hold immense magical power. I'm conducting experiments and need 15 pristine Crystal Shards. You'll likely find them on the Crystal Elementals that inhabit the region. Handle them carefully - they're quite volatile!",
        item_type="EnergyVortex",
        item_name="crystal shard",
        item_count=15,
        reward_gold=1800
    )

    output_path = QUEST_OUTPUT_DIR / "CrystalShardQuest.cs"
    with open(output_path, 'w', encoding='utf-8') as f:
        f.write(quest6)

    generated_count += 1
    print(f"[OK] Generated CrystalShardQuest.cs")

    print()
    print("=" * 70)
    print(f"[OK] GENERATION COMPLETE!")
    print("=" * 70)
    print(f"Generated {generated_count} quest classes")
    print()
    print("Output directory:")
    print(f"  {QUEST_OUTPUT_DIR}")
    print()
    print("Generated quests:")
    print("  1. SupplyLineQuest - Deliver 10 iron ingots (Quartermaster Grimwald)")
    print("  2. AncientTextsQuest - Collect 5 ancient scrolls (Sage Theron)")
    print("  3. FrostWolfHuntQuest - Slay 8 dire wolves (Frosthold)")
    print("  4. FireElementalThreatQuest - Slay 10 fire elementals (Emberlands)")
    print("  5. HerbGatheringQuest - Gather 20 ginseng (Verdantpeak)")
    print("  6. CrystalShardQuest - Collect 15 crystal shards (Crystal Barrens)")
    print()
    print("Next steps:")
    print("  1. Build ServUO: dotnet build")
    print("  2. Uncomment quest types in quest giver NPCs:")
    print("     - QuartermasterGrimwald: typeof(SupplyLineQuest)")
    print("     - SageTheron: typeof(AncientTextsQuest)")
    print("  3. Test quests in-game by talking to quest givers")
    print("  4. Expand with more region-specific quests")

if __name__ == "__main__":
    main()
