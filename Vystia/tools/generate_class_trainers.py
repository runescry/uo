#!/usr/bin/env python3
"""
Generate Vystia Class Trainer NPCs

Creates 25 trainer NPCs - one for each class.
Trainers:
- Teach class-specific skills
- Explain class mechanics
- Offer class lore and roleplay dialogue
- Can reset players to their class
- Regional appearance and hues
"""

# Skill name mapping - some custom Vystia skills need special handling
# Standard UO skills work as-is, Vystia skills are also in the SkillName enum
# All skill names here must be valid SkillName enum values

# Class definitions with trainer data
CLASSES = {
    # Frosthold Classes (3)
    "Barbarian": {
        "region": "Frosthold",
        "hue": 1150,
        "body": "HUMAN",
        "title": "Barbarian Battlemaster",
        "skills": ["Swords", "Tactics", "Anatomy", "Healing", "Parrying", "MagicResist"],
        "description": "Savage warriors who channel primal rage into devastating melee attacks.",
        "greeting": "The frozen north breeds the strongest warriors. Are you ready to embrace the rage?",
    },
    "Beastmaster": {
        "region": "Frosthold",
        "hue": 1150,
        "body": "HUMAN",
        "title": "Beastmaster Elder",
        "skills": ["AnimalTaming", "AnimalLore", "Veterinary", "Archery", "Tracking", "Camping"],
        "description": "Masters of the wild who form bonds with fierce creatures of the north.",
        "greeting": "The beasts of Frosthold are loyal companions. Let me teach you their ways.",
    },
    "IceMage": {
        "region": "Frosthold",
        "hue": 1150,
        "body": "HUMAN",
        "title": "Ice Mage Archon",
        "skills": ["Magery", "EvalInt", "Meditation", "MagicResist", "Inscribe", "Focus"],
        "description": "Wielders of frost magic who command the power of eternal winter.",
        "greeting": "Ice is patient. Ice is relentless. Let me show you its secrets.",
    },

    # Emberlands (1)
    "Sorcerer": {
        "region": "Emberlands",
        "hue": 1358,
        "body": "HUMAN",
        "title": "Sorcerer Flamecaller",
        "skills": ["Magery", "EvalInt", "Meditation", "MagicResist", "Inscribe", "Focus"],
        "description": "Masters of elemental fury who command fire, water, earth, and air.",
        "greeting": "The elements rage within us all. I can teach you to command them.",
    },

    # Desert (2)
    "Ranger": {
        "region": "Desert",
        "hue": 1719,
        "body": "HUMAN",
        "title": "Ranger Sandstrider",
        "skills": ["Archery", "Tracking", "Hiding", "Stealth", "Tactics", "Anatomy"],
        "description": "Expert trackers and marksmen who thrive in the harsh desert.",
        "greeting": "The desert reveals all secrets to those who know where to look.",
    },
    "Illusionist": {
        "region": "Desert",
        "hue": 1719,
        "body": "HUMAN",
        "title": "Illusionist Mirageweaver",
        "skills": ["Magery", "EvalInt", "Meditation", "Hiding", "Stealth", "Inscribe"],
        "description": "Masters of deception who bend reality with their spells.",
        "greeting": "What you see is rarely what is. Let me show you the truth behind illusion.",
    },

    # Shadowfen (1)
    "Witch": {
        "region": "Shadowfen",
        "hue": 2073,
        "body": "HUMAN",
        "title": "Witch Hexmother",
        "skills": ["Magery", "EvalInt", "Poisoning", "Alchemy", "Meditation", "SpiritSpeak"],
        "description": "Dark practitioners who specialize in curses, hexes, and poisons.",
        "greeting": "The swamp holds dark knowledge. Are you prepared to learn its secrets?",
    },

    # ShadowVoid (2)
    "Warlock": {
        "region": "ShadowVoid",
        "hue": 1109,
        "body": "HUMAN",
        "title": "Warlock Voidbinder",
        "skills": ["Magery", "EvalInt", "SpiritSpeak", "Meditation", "MagicResist", "Necromancy"],
        "description": "Dark mages who forge pacts with demons for forbidden power.",
        "greeting": "Power has a price. I can show you what lies beyond the veil.",
    },
    "Necromancer": {
        "region": "ShadowVoid",
        "hue": 1109,
        "body": "HUMAN",
        "title": "Necromancer Deathspeaker",
        "skills": ["Magery", "EvalInt", "SpiritSpeak", "Meditation", "Necromancy", "MagicResist"],
        "description": "Masters of death magic who command the undead.",
        "greeting": "Death is not an ending, but a beginning. I can teach you to speak with the departed.",
    },

    # Verdantpeak (2)
    "Druid": {
        "region": "Verdantpeak",
        "hue": 2010,
        "body": "HUMAN",
        "title": "Druid Forestkeeper",
        "skills": ["Magery", "AnimalLore", "Healing", "Meditation", "Veterinary", "Herding"],
        "description": "Guardians of nature who shapeshift and command natural forces.",
        "greeting": "The forest speaks to those who listen. Let me teach you its language.",
    },
    "Alchemist": {
        "region": "Verdantpeak",
        "hue": 2010,
        "body": "HUMAN",
        "title": "Alchemist Transmuter",
        "skills": ["Alchemy", "Magery", "TasteID", "ItemID", "Cooking", "Healing"],
        "description": "Masters of potions and transmutation who unlock nature's secrets.",
        "greeting": "All matter can be transformed. I will show you the art of transmutation.",
    },

    # Crystal Barrens (2)
    "Oracle": {
        "region": "Crystal Barrens",
        "hue": 1154,
        "body": "HUMAN",
        "title": "Oracle Seer",
        "skills": ["Magery", "EvalInt", "Meditation", "SpiritSpeak", "Inscribe", "Focus"],
        "description": "Seers who peer into the future and manipulate time itself.",
        "greeting": "Time flows like water. I can teach you to see its currents.",
    },
    "Wizard": {
        "region": "Crystal Barrens",
        "hue": 1154,
        "body": "HUMAN",
        "title": "Wizard Archmage",
        "skills": ["Magery", "EvalInt", "Meditation", "Inscribe", "MagicResist", "Focus"],
        "description": "Scholars of the arcane who master all schools of magic.",
        "greeting": "Knowledge is the greatest power. Let me share my wisdom with you.",
    },

    # Ironclad (4)
    "Artificer": {
        "region": "Ironclad",
        "hue": 2305,
        "body": "HUMAN",
        "title": "Artificer Gearmaster",
        "skills": ["Tinkering", "Blacksmith", "Mining", "ArmsLore", "ItemID", "Carpentry"],
        "description": "Masters of clockwork who build mechanical companions and gadgets.",
        "greeting": "Steel and steam can accomplish anything. Let me show you the art of engineering.",
    },
    "Fighter": {
        "region": "Ironclad",
        "hue": 2305,
        "body": "HUMAN",
        "title": "Fighter Champion",
        "skills": ["Swords", "Tactics", "Anatomy", "Parrying", "Healing", "MagicResist"],
        "description": "Elite warriors who have mastered all forms of combat.",
        "greeting": "A true warrior masters every weapon. Are you ready to train?",
    },
    "Monk": {
        "region": "Ironclad",
        "hue": 2305,
        "body": "HUMAN",
        "title": "Monk Grandmaster",
        "skills": ["Wrestling", "Tactics", "Anatomy", "Focus", "Meditation", "Healing"],
        "description": "Masters of unarmed combat who channel inner strength.",
        "greeting": "The body is the ultimate weapon. Let me help you unlock its potential.",
    },
    "Templar": {
        "region": "Ironclad",
        "hue": 2305,
        "body": "HUMAN",
        "title": "Templar Crusader",
        "skills": ["Swords", "Tactics", "Anatomy", "Chivalry", "MagicResist", "Parrying"],
        "description": "Holy warriors who serve the forges with unwavering devotion.",
        "greeting": "Faith and steel make us strong. I can teach you the ways of the Templar.",
    },

    # Underwater (1)
    "Summoner": {
        "region": "Underwater",
        "hue": 1365,
        "body": "HUMAN",
        "title": "Summoner Planeswalker",
        "skills": ["Magery", "EvalInt", "SpiritSpeak", "Meditation", "MagicResist", "Focus"],
        "description": "Masters of conjuration who summon creatures from other planes.",
        "greeting": "Other realms teem with creatures waiting to serve. I will teach you to call them.",
    },

    # Multi-Regional (7)
    "BountyHunter": {
        "region": "Multi-Regional",
        "hue": 1719,
        "body": "HUMAN",
        "title": "Bounty Hunter Tracker",
        "skills": ["Tracking", "Forensics", "DetectHidden", "Archery", "Hiding", "Stealth"],
        "description": "Relentless trackers who hunt down targets for gold and justice.",
        "greeting": "Every target leaves a trail. I can teach you how to follow it.",
    },
    "Knight": {
        "region": "Multi-Regional",
        "hue": 1153,
        "body": "HUMAN",
        "title": "Knight Commander",
        "skills": ["Swords", "Tactics", "Anatomy", "Chivalry", "Parrying", "Healing"],
        "description": "Noble warriors who uphold honor and protect the realm.",
        "greeting": "Honor above all. Let me teach you the ways of true knighthood.",
    },
    "Shaman": {
        "region": "Multi-Regional",
        "hue": 1281,
        "body": "HUMAN",
        "title": "Shaman Spiritwalker",
        "skills": ["Magery", "SpiritSpeak", "Meditation", "Healing", "AnimalLore", "Herding"],
        "description": "Spirit guides who commune with ancestors and elemental totems.",
        "greeting": "The spirits are always watching. I can teach you to hear their voices.",
    },
    "Cleric": {
        "region": "Multi-Regional",
        "hue": 1153,
        "body": "HUMAN",
        "title": "Cleric High Priest",
        "skills": ["Magery", "Healing", "Anatomy", "Meditation", "MagicResist", "Focus"],
        "description": "Holy healers who channel divine power to aid allies.",
        "greeting": "Divine light shines upon the faithful. Let me guide you in its ways.",
    },
    "Paladin": {
        "region": "Multi-Regional",
        "hue": 1153,
        "body": "HUMAN",
        "title": "Paladin Justicar",
        "skills": ["Swords", "Tactics", "Chivalry", "Healing", "Anatomy", "MagicResist"],
        "description": "Holy warriors who combine martial prowess with divine healing.",
        "greeting": "Righteousness guides our swords. Join me in the path of the Paladin.",
    },
    "Bard": {
        "region": "Multi-Regional",
        "hue": 2011,
        "body": "HUMAN",
        "title": "Bard Songmaster",
        "skills": ["Musicianship", "Peacemaking", "Provocation", "Discordance", "Magery", "EvalInt"],
        "description": "Masters of song who inspire allies and confound enemies.",
        "greeting": "Music is the language of the soul. Let me teach you its power.",
    },
    "Enchanter": {
        "region": "Multi-Regional",
        "hue": 1154,
        "body": "HUMAN",
        "title": "Enchanter Runesmith",
        "skills": ["Magery", "Inscribe", "EvalInt", "ItemID", "Meditation", "Focus"],
        "description": "Arcane crafters who imbue items with magical properties.",
        "greeting": "Every object has potential. I can teach you to unlock it.",
    },
}


def generate_trainer_file():
    """Generate the VystiaClassTrainers.cs file"""

    output = '''using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Custom.VystiaClasses;
using Server.Custom.VystiaClasses.Gumps;

namespace Server.Mobiles
{
    /// <summary>
    /// Vystia Class Trainer NPCs
    /// One trainer per class (25 total)
    /// Trainers can:
    /// - Explain class mechanics and lore
    /// - Teach class-specific skills
    /// - Open class selection gump for new players
    /// - Reset class for GM characters
    /// </summary>

    #region Base Trainer

    public abstract class VystiaClassTrainer : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        public abstract PlayerClassType TrainerClass { get; }
        public abstract string TrainerGreeting { get; }
        public abstract string ClassDescription { get; }
        public abstract SkillName[] TrainableSkills { get; }

        public override bool IsActiveVendor { get { return false; } }

        public VystiaClassTrainer(string title) : base(title)
        {
        }

        public override void InitSBInfo()
        {
            // Trainers don't sell items - they teach skills
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;

            if (!e.Handled && from.InRange(this.Location, 3))
            {
                string speech = e.Speech.ToLower();

                if (speech.Contains("join") || speech.Contains("train") || speech.Contains("class"))
                {
                    HandleClassInquiry(from);
                    e.Handled = true;
                }
                else if (speech.Contains("skill") || speech.Contains("learn"))
                {
                    HandleSkillInquiry(from);
                    e.Handled = true;
                }
                else if (speech.Contains("hello") || speech.Contains("greet"))
                {
                    SayTo(from, TrainerGreeting);
                    e.Handled = true;
                }
            }

            base.OnSpeech(e);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.Location, 4))
            {
                // Show greeting and options
                SayTo(from, TrainerGreeting);

                PlayerMobile pm = from as PlayerMobile;
                if (pm != null)
                {
                    if (pm.VystiaClass == PlayerClassType.None)
                    {
                        SayTo(from, "Say 'join' if you wish to learn the ways of my class.");
                    }
                    else if (pm.VystiaClass == TrainerClass)
                    {
                        SayTo(from, "Welcome, fellow " + TrainerClass.ToString() + "! Say 'skill' to train.");
                    }
                    else
                    {
                        SayTo(from, "You have already chosen a different path. I cannot help you.");
                    }
                }
            }
            else
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
        }

        private void HandleClassInquiry(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;
            if (pm == null)
                return;

            if (pm.VystiaClass == PlayerClassType.None)
            {
                SayTo(from, ClassDescription);
                SayTo(from, "If you wish to join our ranks, confirm your choice.");
                pm.SendGump(new VystiaClassConfirmationGump(pm, TrainerClass));
            }
            else if (pm.VystiaClass == TrainerClass)
            {
                SayTo(from, "You are already one of us. Well met!");
            }
            else
            {
                SayTo(from, "You have already chosen a different path.");
            }
        }

        private void HandleSkillInquiry(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;
            if (pm == null)
                return;

            if (pm.VystiaClass != TrainerClass)
            {
                SayTo(from, "I only train those who have chosen my path.");
                return;
            }

            // Standard vendor skill training
            SayTo(from, "I can help you improve your skills. What would you like to learn?");
            // This would typically open a skill training gump
        }

        public VystiaClassTrainer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

'''

    # Generate each trainer class
    for class_name, data in CLASSES.items():
        trainer_class = f"{class_name}Trainer"

        # Format skills array
        skills = data["skills"]
        skills_array = "new SkillName[] { " + ", ".join([f"SkillName.{s}" for s in skills]) + " }"

        output += f'''    #region {class_name} Trainer

    [CorpseNameAttribute("corpse of a {data["title"].lower()}")]
    public class {trainer_class} : VystiaClassTrainer
    {{
        public override PlayerClassType TrainerClass {{ get {{ return PlayerClassType.{class_name}; }} }}
        public override string TrainerGreeting {{ get {{ return "{data["greeting"]}"; }} }}
        public override string ClassDescription {{ get {{ return "{data["description"]}"; }} }}
        public override SkillName[] TrainableSkills {{ get {{ return {skills_array}; }} }}

        [Constructable]
        public {trainer_class}() : base("{data["title"]}")
        {{
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the {data["title"]}";
            Hue = {data["hue"]};

            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
        }}

        public {trainer_class}(Serial serial) : base(serial) {{ }}

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

    #endregion

'''

    output += "}\n"
    return output


def main():
    output = generate_trainer_file()

    # Write the file
    output_path = r'D:\UO\ServUO\Scripts\Mobiles\Vystia\Trainers\VystiaClassTrainers.cs'

    import os
    os.makedirs(os.path.dirname(output_path), exist_ok=True)

    with open(output_path, 'w', encoding='utf-8') as f:
        f.write(output)

    print(f"Generated {len(CLASSES)} class trainers")
    print(f"Output: {output_path}")


if __name__ == "__main__":
    main()
