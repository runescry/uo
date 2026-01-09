#!/usr/bin/env python3
"""
Generate Martial Abilities for Vystia Class System v2.0

This script generates ability registration files for the 14 martial class schools.
Each class gets 16 abilities (4 per "circle" equivalent, 4 circles).

Usage: python generate_martial_abilities.py
Output: ServUO/Scripts/Custom/VystiaClasses/Abilities/Generated/Martial/*.cs
"""

import os

# Output directory
OUTPUT_DIR = r"D:\UO\ServUO\Scripts\Custom\VystiaClasses\Abilities\Generated\Martial"

# Ability ID ranges for martial classes (starting after magic spells 1000-1383)
# 2000-2015 = Fighter, 2016-2031 = Barbarian, etc.
MARTIAL_ABILITY_START = 2000

# Define martial classes with their abilities based on CLASSES.md design
MARTIAL_CLASSES = {
    "Fighter": {
        "school": "Fighter",
        "resource": "Stamina",
        "damage_type": "Physical",
        "abilities": [
            # Circle 1 (basic)
            ("Power Strike", "damage", 15, 25, "High-damage single target attack"),
            ("Shield Bash", "damage_cc", 8, 12, "Stun target for 2s"),
            ("Battle Shout", "buff", 0, 0, "Buff allies with +10 STR"),
            ("Defensive Stance", "buff_self", 0, 0, "+20 all resists for 30s"),
            # Circle 2
            ("Cleave", "aoe", 12, 20, "Hit all enemies in front"),
            ("Shield Wall", "buff_self", 0, 0, "Block 50% damage for 10s"),
            ("Disarm", "cc", 0, 0, "Disarm target for 5s"),
            ("Charge", "damage_gap", 20, 30, "Rush to target and stun"),
            # Circle 3
            ("Weapon Mastery", "buff_self", 0, 0, "+25% damage for 20s"),
            ("Ironclad Stance", "buff_self", 0, 0, "Reduce damage taken 30%"),
            ("Execute", "damage_cond", 40, 60, "Bonus damage if target <30% HP"),
            ("Rallying Cry", "buff_aoe", 0, 0, "Heal and buff nearby allies"),
            # Circle 4 (ultimate)
            ("Bladestorm", "aoe", 25, 40, "Spin dealing damage to all nearby"),
            ("Avatar of War", "transform", 0, 0, "Transform into war avatar"),
            ("Mortal Strike", "damage_debuff", 35, 50, "Reduce healing received 50%"),
            ("Last Stand", "buff_self", 0, 0, "Cannot die for 8s"),
        ]
    },
    "Barbarian": {
        "school": "Barbarian",
        "resource": "Fury",
        "damage_type": "Physical",
        "abilities": [
            # Circle 1
            ("Reckless Strike", "damage", 18, 28, "High damage, costs HP"),
            ("Frost Fury", "damage", 15, 22, "Cold damage melee attack"),
            ("War Cry", "debuff_aoe", 0, 0, "Fear nearby enemies"),
            ("Thick Skin", "buff_self", 0, 0, "+15 cold resist"),
            # Circle 2
            ("Whirlwind", "aoe", 14, 22, "Spin attack all nearby"),
            ("Berserker Rage", "buff_self", 0, 0, "+50% damage, -20% defense"),
            ("Ground Slam", "aoe_cc", 10, 18, "Knockdown nearby enemies"),
            ("Winter's Endurance", "buff_self", 0, 0, "HP regen in cold areas"),
            # Circle 3
            ("Avalanche Strike", "aoe", 20, 35, "Ice shards from ground slam"),
            ("Blood Rage", "buff_self", 0, 0, "Lifesteal on attacks"),
            ("Intimidating Roar", "debuff_aoe", 0, 0, "Reduce enemy damage 25%"),
            ("Frenzy", "buff_self", 0, 0, "+50% attack speed"),
            # Circle 4
            ("Wrath of the North", "aoe", 30, 50, "Massive cold AoE"),
            ("Deathwish", "buff_self", 0, 0, "More damage when low HP"),
            ("Rampage", "damage", 40, 60, "Chain of 5 attacks"),
            ("Primal Avatar", "transform", 0, 0, "Transform into frost giant"),
        ]
    },
    "Monk": {
        "school": "Monk",
        "resource": "Chi",
        "damage_type": "Physical",
        "abilities": [
            # Circle 1
            ("Jab", "damage", 8, 12, "Quick strike, generates Chi"),
            ("Tiger Palm", "damage", 12, 18, "Palm strike with knockback"),
            ("Roll", "mobility", 0, 0, "Dodge roll, remove roots"),
            ("Inner Peace", "buff_self", 0, 0, "Mana regen boost"),
            # Circle 2
            ("Flurry of Blows", "damage", 20, 30, "Rapid 5-hit combo"),
            ("Clockwork Reflexes", "buff_self", 0, 0, "+30% dodge chance"),
            ("Stunning Fist", "damage_cc", 10, 15, "Stun for 3s"),
            ("Healing Mist", "heal_aoe", 15, 25, "Heal nearby allies"),
            # Circle 3
            ("Rising Sun Kick", "damage", 25, 40, "Powerful kick with knockup"),
            ("Iron Fist", "buff_self", 0, 0, "Unarmed = magic damage"),
            ("Perfect Balance", "buff_self", 0, 0, "Immune to knockdown"),
            ("Chi Wave", "damage", 18, 28, "Ranged chi projectile"),
            # Circle 4
            ("Quivering Palm", "damage_cond", 50, 80, "Delayed massive damage"),
            ("Serenity", "buff_self", 0, 0, "Immune to CC for 10s"),
            ("Touch of Death", "damage_cond", 100, 150, "Execute below 10% HP"),
            ("Transcendence", "transform", 0, 0, "Ethereal form"),
        ]
    },
    "Rogue": {
        "school": "Rogue",
        "resource": "ComboPoints",
        "damage_type": "Physical",
        "abilities": [
            # Circle 1
            ("Sinister Strike", "damage", 12, 18, "Build combo point"),
            ("Backstab", "damage_cond", 20, 30, "Bonus from behind"),
            ("Stealth", "buff_self", 0, 0, "Enter stealth mode"),
            ("Pick Pocket", "utility", 0, 0, "Steal gold from target"),
            # Circle 2
            ("Eviscerate", "finisher", 8, 12, "Per combo point damage"),
            ("Kidney Shot", "cc", 0, 0, "Stun per combo point"),
            ("Gouge", "cc", 0, 0, "Incapacitate for 4s"),
            ("Sprint", "buff_self", 0, 0, "+50% move speed"),
            # Circle 3
            ("Ambush", "damage_cond", 35, 50, "From stealth bonus"),
            ("Blade Flurry", "buff_self", 0, 0, "Attacks hit 2 targets"),
            ("Cheap Shot", "cc", 0, 0, "Stun from stealth"),
            ("Rupture", "dot", 5, 8, "Bleed per combo point"),
            # Circle 4
            ("Shadow Dance", "buff_self", 0, 0, "Use stealth abilities"),
            ("Vendetta", "debuff", 0, 0, "+20% damage to target"),
            ("Death from Above", "damage", 45, 70, "Leap strike finisher"),
            ("Vanish", "buff_self", 0, 0, "Instant stealth, drop threat"),
        ]
    },
    "Ranger": {
        "school": "Ranger",
        "resource": "Focus",
        "damage_type": "Physical",
        "abilities": [
            # Circle 1
            ("Steady Shot", "damage", 12, 18, "Basic ranged attack"),
            ("Serpent Sting", "dot", 4, 6, "Poison DoT"),
            ("Aspect of Hawk", "buff_self", 0, 0, "+15% ranged damage"),
            ("Track", "utility", 0, 0, "See hidden enemies"),
            # Circle 2
            ("Aimed Shot", "damage", 22, 35, "High damage, cast time"),
            ("Multi-Shot", "aoe", 8, 14, "Hit up to 3 targets"),
            ("Disengage", "mobility", 0, 0, "Leap backwards"),
            ("Freezing Trap", "cc", 0, 0, "Freeze first enemy"),
            # Circle 3
            ("Sandstorm Arrow", "aoe_debuff", 15, 22, "Blind enemies in area"),
            ("Desert Camouflage", "buff_self", 0, 0, "Stealth in sand"),
            ("Sunburst Shot", "damage", 28, 42, "Fire damage arrow"),
            ("Explosive Trap", "aoe", 20, 30, "Fire AoE trap"),
            # Circle 4
            ("Kill Shot", "damage_cond", 50, 80, "Execute below 20% HP"),
            ("Aspect of Wolf", "buff_self", 0, 0, "+30% move speed"),
            ("Barrage", "aoe", 35, 55, "Volley of arrows"),
            ("Bestial Wrath", "buff_self", 0, 0, "Pet goes berserk"),
        ]
    },
    "Knight": {
        "school": "Knight",
        "resource": "Fortitude",
        "damage_type": "Physical",
        "abilities": [
            # Circle 1
            ("Shield Slam", "damage", 10, 16, "Shield attack"),
            ("Charge", "damage_gap", 15, 22, "Rush and knockdown"),
            ("Noble Shield", "buff_ally", 0, 0, "Take damage for ally"),
            ("Rally", "buff_aoe", 0, 0, "Morale boost allies"),
            # Circle 2
            ("Challenge", "taunt", 0, 0, "Force enemy to attack you"),
            ("Consecration", "aoe_dot", 6, 10, "Holy ground damage"),
            ("Lance Strike", "damage", 20, 32, "Mounted charge damage"),
            ("Divine Protection", "buff_self", 0, 0, "Reduce damage 50%"),
            # Circle 3
            ("Judgment", "damage_debuff", 25, 38, "Holy damage + slow"),
            ("Aura of Devotion", "buff_aoe", 0, 0, "+10 all resists allies"),
            ("Righteous Fury", "buff_self", 0, 0, "Generate threat on heals"),
            ("Shield of Faith", "buff_ally", 0, 0, "Damage absorb shield"),
            # Circle 4
            ("Guardian of Light", "buff_self", 0, 0, "Reflect damage taken"),
            ("Crusader Strike", "damage", 40, 60, "Holy damage combo"),
            ("Last Defender", "buff_self", 0, 0, "Immune while allies alive"),
            ("Azure Tide", "aoe", 45, 70, "Massive water + holy AoE"),
        ]
    },
    "Paladin": {
        "school": "Paladin",
        "resource": "HolyPower",
        "damage_type": "Holy",
        "abilities": [
            # Circle 1
            ("Crusader Strike", "damage", 14, 20, "Build holy power"),
            ("Judgment", "damage", 12, 18, "Ranged holy damage"),
            ("Lay on Hands", "heal", 50, 80, "Large instant heal"),
            ("Blessing of Might", "buff_ally", 0, 0, "+15% damage"),
            # Circle 2
            ("Divine Storm", "aoe", 10, 16, "Holy AoE damage"),
            ("Word of Glory", "heal", 20, 30, "Heal per holy power"),
            ("Divine Shield", "buff_self", 0, 0, "Immune for 8s"),
            ("Hand of Protection", "buff_ally", 0, 0, "Ally immune 10s"),
            # Circle 3
            ("Templar's Verdict", "finisher", 12, 18, "Per holy power damage"),
            ("Aura of Protection", "buff_aoe", 0, 0, "+15 all resists"),
            ("Cleanse", "dispel", 0, 0, "Remove debuffs"),
            ("Turn Evil", "cc", 0, 0, "Fear undead/demons"),
            # Circle 4
            ("Smite Evil", "damage_cond", 55, 85, "Bonus vs evil targets"),
            ("Divine Weapon", "buff_self", 0, 0, "Weapon deals holy"),
            ("Guardian Angel", "buff_ally", 0, 0, "Resurrect if ally dies"),
            ("Wrath of the Righteous", "aoe", 50, 75, "Holy AoE judgment"),
        ]
    },
    "Templar": {
        "school": "Templar",
        "resource": "Zeal",
        "damage_type": "Holy",
        "abilities": [
            # Circle 1
            ("Judgment Strike", "damage", 16, 24, "Holy damage + mark"),
            ("Zealot's Fervor", "buff_self", 0, 0, "Build zeal on hit"),
            ("Aura of Justice", "buff_aoe", 0, 0, "+10% damage allies"),
            ("Interrogate", "debuff", 0, 0, "Weaken marked target"),
            # Circle 2
            ("Divine Shield", "buff_ally", 0, 0, "Protect with barrier"),
            ("Smiting Blow", "damage", 22, 34, "Zeal-empowered strike"),
            ("Condemn", "damage_debuff", 18, 28, "Mark + damage taken increase"),
            ("Absolution", "dispel", 0, 0, "Cleanse debuffs"),
            # Circle 3
            ("Execution", "damage_cond", 60, 100, "Execute marked below 20%"),
            ("Zealous Inspiration", "buff_aoe", 0, 0, "Buff party attack speed"),
            ("Seal of Justice", "buff_self", 0, 0, "Attacks slow enemies"),
            ("Inquisitor's Eye", "utility", 0, 0, "See through stealth"),
            # Circle 4
            ("Final Judgment", "damage", 50, 80, "Consume all zeal damage"),
            ("Wrath of the Justicar", "aoe", 40, 65, "Holy AoE judgment"),
            ("Divine Retribution", "buff_self", 0, 0, "Return damage when hit"),
            ("Iron Justicar", "transform", 0, 0, "Become invulnerable judge"),
        ]
    },
    "BountyHunter": {
        "school": "BountyHunter",
        "resource": "Pursuit",
        "damage_type": "Physical",
        "abilities": [
            # Circle 1
            ("Mark Target", "debuff", 0, 0, "Mark for bonus damage"),
            ("Quick Draw", "damage", 14, 20, "Fast ranged attack"),
            ("Tracking", "utility", 0, 0, "Track marked target"),
            ("Caltrops", "aoe_cc", 0, 0, "Slow enemies in area"),
            # Circle 2
            ("Bola", "cc", 0, 0, "Root target for 4s"),
            ("Silver Strike", "damage_type", 20, 30, "Ignore undead resist"),
            ("Net Trap", "cc", 0, 0, "Snare first enemy"),
            ("Pursuit", "buff_self", 0, 0, "+25% speed vs marked"),
            # Circle 3
            ("Hunter's Quarry", "buff_self", 0, 0, "Track anywhere"),
            ("Explosive Trap", "aoe", 22, 35, "Fire trap damage"),
            ("Venomous Blade", "damage_dot", 18, 28, "+poison DoT"),
            ("Interrogation", "debuff", 0, 0, "Mark reveals weaknesses"),
            # Circle 4
            ("Execution Contract", "damage_cond", 70, 110, "Massive marked damage"),
            ("Dead or Alive", "buff_self", 0, 0, "+100% damage vs marked"),
            ("Monster Slayer", "buff_self", 0, 0, "+50% vs creatures"),
            ("Final Tally", "damage", 55, 85, "Consume pursuit stacks"),
        ]
    },
    "Beastmaster": {
        "school": "Beastmaster",
        "resource": "PackBond",
        "damage_type": "Physical",
        "abilities": [
            # Circle 1
            ("Call Pet", "summon", 0, 0, "Summon bonded beast"),
            ("Growl", "taunt", 0, 0, "Pet taunts enemy"),
            ("Pack Tactics", "buff_self", 0, 0, "Bonus with pet nearby"),
            ("Mend Pet", "heal", 25, 40, "Heal pet"),
            # Circle 2
            ("Kill Command", "damage", 18, 28, "Pet attacks target"),
            ("Feral Bond", "buff_self", 0, 0, "Share senses with pet"),
            ("Alpha's Command", "buff_pet", 0, 0, "Empower pet"),
            ("Bestial Call", "summon", 0, 0, "Summon temporary beast"),
            # Circle 3
            ("Call of the Wild", "summon_aoe", 0, 0, "Summon wolf pack"),
            ("Primal Rage", "buff_pet", 0, 0, "Pet enrages"),
            ("Spirit Bond", "buff_self", 0, 0, "Share HP with pet"),
            ("Savage Rend", "damage", 28, 42, "Pet combo attack"),
            # Circle 4
            ("Beast Mastery", "buff_pet", 0, 0, "Pet +100% damage"),
            ("Stampede", "aoe", 35, 55, "All pets charge"),
            ("Alpha Predator", "transform", 0, 0, "Merge with beast"),
            ("Winter Pack", "summon", 0, 0, "Summon 3 arctic wolves"),
        ]
    },
    "Artificer": {
        "school": "Artificer",
        "resource": "Steam",
        "damage_type": "Physical",
        "abilities": [
            # Circle 1
            ("Clockwork Shot", "damage", 12, 18, "Ranged gadget attack"),
            ("Deploy Turret", "summon", 0, 0, "Place gun turret"),
            ("Gadget Shield", "buff_self", 0, 0, "Energy barrier"),
            ("Tinker", "utility", 0, 0, "Repair constructs"),
            # Circle 2
            ("Steam Blast", "aoe", 14, 22, "Steam cone damage"),
            ("EMP Bomb", "aoe_cc", 0, 0, "Disable constructs"),
            ("Net Launcher", "cc", 0, 0, "Root target"),
            ("Overclock", "buff_self", 0, 0, "+50% construct damage"),
            # Circle 3
            ("Mechanical Companion", "summon", 0, 0, "Build clockwork pet"),
            ("Gadget Bomb", "aoe", 25, 40, "Explosive device"),
            ("Emergency Shield", "buff_self", 0, 0, "Damage absorb"),
            ("Rocket Boots", "mobility", 0, 0, "Jet pack dash"),
            # Circle 4
            ("Deploy Artillery", "summon", 0, 0, "Heavy turret"),
            ("Clockwork Army", "summon_aoe", 0, 0, "Summon 5 bots"),
            ("Mech Suit", "transform", 0, 0, "Enter exosuit"),
            ("Overcharge", "aoe", 50, 80, "Massive steam explosion"),
        ]
    },
    "Alchemist": {
        "school": "Alchemist",
        "resource": "ReagentStock",
        "damage_type": "Physical",
        "abilities": [
            # Circle 1
            ("Fire Bomb", "aoe", 12, 18, "Fire flask AoE"),
            ("Ice Bomb", "aoe_cc", 10, 15, "Cold flask + slow"),
            ("Healing Draught", "heal", 20, 30, "Quick healing potion"),
            ("Reagent Gather", "utility", 0, 0, "Collect nearby reagents"),
            # Circle 2
            ("Acid Flask", "damage_dot", 15, 22, "+acid DoT"),
            ("Smoke Bomb", "aoe_cc", 0, 0, "Blind in area"),
            ("Mutagen: Strength", "buff_self", 0, 0, "+20 STR"),
            ("Mutagen: Speed", "buff_self", 0, 0, "+30% attack speed"),
            # Circle 3
            ("Alchemical Bomb", "aoe", 28, 42, "Choose element type"),
            ("Transmutation", "utility", 0, 0, "Alter object properties"),
            ("Mutagen: Resistance", "buff_self", 0, 0, "+20 all resists"),
            ("Instant Brew", "utility", 0, 0, "Quick craft potion"),
            # Circle 4
            ("Philosopher's Bomb", "aoe", 45, 70, "Massive alchemy explosion"),
            ("Elixir of Life", "heal", 80, 120, "Full party heal"),
            ("Mutagen: Transcendence", "transform", 0, 0, "Mutate into hybrid form"),
            ("Volatile Mixture", "aoe", 55, 85, "Chain reaction explosions"),
        ]
    },
    "Cleric": {
        "school": "Cleric",
        "resource": "Faith",
        "damage_type": "Holy",
        "abilities": [
            # Circle 1
            ("Heal", "heal", 18, 28, "Direct healing spell"),
            ("Smite", "damage", 12, 18, "Holy damage attack"),
            ("Bless", "buff_ally", 0, 0, "+10 all stats"),
            ("Light", "utility", 0, 0, "Illuminate area"),
            # Circle 2
            ("Prayer of Healing", "heal_aoe", 12, 18, "Group heal"),
            ("Turn Undead", "cc", 0, 0, "Fear undead"),
            ("Shield of Faith", "buff_ally", 0, 0, "Damage absorb"),
            ("Purify", "dispel", 0, 0, "Remove poison/disease"),
            # Circle 3
            ("Greater Heal", "heal", 35, 55, "Large heal"),
            ("Divine Smite", "damage", 28, 42, "Empowered holy damage"),
            ("Sanctuary", "aoe_cc", 0, 0, "Pacify enemies in area"),
            ("Mass Blessing", "buff_aoe", 0, 0, "Buff all allies"),
            # Circle 4
            ("Divine Intervention", "heal", 100, 150, "Emergency massive heal"),
            ("Holy Nova", "aoe", 40, 60, "Damage enemies, heal allies"),
            ("Resurrection", "revive", 0, 0, "Revive dead ally"),
            ("Avatar of Light", "transform", 0, 0, "Become divine being"),
        ]
    },
    "Wizard": {
        "school": "Wizard",
        "resource": "Mana",
        "damage_type": "Arcane",
        "abilities": [
            # Circle 1
            ("Arcane Bolt", "damage", 10, 16, "Basic arcane damage"),
            ("Magic Shield", "buff_self", 0, 0, "Mana absorbs damage"),
            ("Detect Magic", "utility", 0, 0, "See magical auras"),
            ("Counterspell", "dispel", 0, 0, "Interrupt spellcast"),
            # Circle 2
            ("Arcane Missiles", "damage", 18, 28, "Multi-hit arcane"),
            ("Blink", "mobility", 0, 0, "Short teleport"),
            ("Polymorph", "cc", 0, 0, "Turn into sheep"),
            ("Spellsteal", "dispel", 0, 0, "Steal enemy buff"),
            # Circle 3
            ("Arcane Explosion", "aoe", 22, 35, "Arcane AoE burst"),
            ("Metamagic: Extend", "buff_self", 0, 0, "+50% spell duration"),
            ("Ritual Casting", "utility", 0, 0, "Cast without combat"),
            ("Arcane Power", "buff_self", 0, 0, "+30% spell damage"),
            # Circle 4
            ("Arcane Barrage", "damage", 45, 70, "Consume charges damage"),
            ("Time Warp", "buff_aoe", 0, 0, "+30% haste party"),
            ("Meteor", "aoe", 55, 85, "Massive fire AoE"),
            ("Arcane Ascendancy", "transform", 0, 0, "Become arcane being"),
        ]
    },
}

def get_ability_type_code(ability_type):
    """Return the appropriate AbilityDefinition builder code for each type."""
    type_mappings = {
        "damage": "CreateDamageSpell",
        "damage_cc": "CreateDamageSpell",  # with CC effect added
        "damage_gap": "CreateDamageSpell",  # gap closer
        "damage_cond": "CreateDamageSpell",  # conditional
        "damage_dot": "CreateDamageSpell",
        "damage_debuff": "CreateDamageSpell",
        "damage_type": "CreateDamageSpell",
        "aoe": "CreateAoESpell",
        "aoe_cc": "CreateAoESpell",
        "aoe_dot": "CreateAoESpell",
        "aoe_debuff": "CreateAoESpell",
        "heal": "CreateHealSpell",
        "heal_aoe": "CreateHealSpell",
        "buff_self": "CreateBuffSpell",
        "buff_ally": "CreateBuffSpell",
        "buff_aoe": "CreateBuffSpell",
        "buff_pet": "CreateBuffSpell",
        "debuff": "builder",
        "debuff_aoe": "builder",
        "cc": "builder",
        "dot": "CreateDoTSpell",
        "finisher": "CreateFinisher",
        "transform": "builder",
        "summon": "builder",
        "summon_aoe": "builder",
        "mobility": "builder",
        "utility": "builder",
        "taunt": "builder",
        "dispel": "builder",
        "revive": "builder",
    }
    return type_mappings.get(ability_type, "builder")


def get_damage_type_enum(damage_type):
    """Map damage type string to enum."""
    mappings = {
        "Physical": "VystiaDamageType.Physical",
        "Fire": "VystiaDamageType.Fire",
        "Cold": "VystiaDamageType.Cold",
        "Poison": "VystiaDamageType.Poison",
        "Energy": "VystiaDamageType.Energy",
        "Holy": "VystiaDamageType.Holy",
        "Shadow": "VystiaDamageType.Shadow",
        "Arcane": "VystiaDamageType.Arcane",
        "Nature": "VystiaDamageType.Nature",
    }
    return mappings.get(damage_type, "VystiaDamageType.Physical")


def generate_ability_code(ability_id, name, school, circle, ability_type, min_val, max_val, desc, damage_type):
    """Generate C# code for a single ability."""
    mana_cost = 4 + (circle - 1) * 3  # 4, 7, 10, 13
    damage_enum = get_damage_type_enum(damage_type)

    code_type = get_ability_type_code(ability_type)

    if code_type == "CreateDamageSpell":
        return f'''            // {name} (Circle {circle})
            AbilityRegistry.RegisterAbility(AbilityDefinition.CreateDamageSpell(
                {ability_id}, "{name}", AbilitySchool.{school}, {circle}, {min_val}, {max_val}, {damage_enum}, {mana_cost})
                .WithDescription("{desc}")
                .WithImpactEffect(0x36D4, 0x1E5, 0x481));
'''
    elif code_type == "CreateAoESpell":
        return f'''            // {name} (Circle {circle})
            AbilityRegistry.RegisterAbility(AbilityDefinition.CreateAoESpell(
                {ability_id}, "{name}", AbilitySchool.{school}, {circle}, {min_val}, {max_val}, {damage_enum}, 4, {mana_cost})
                .WithDescription("{desc}")
                .WithImpactEffect(0x36D4, 0x1E5, 0x481));
'''
    elif code_type == "CreateHealSpell":
        return f'''            // {name} (Circle {circle})
            AbilityRegistry.RegisterAbility(AbilityDefinition.CreateHealSpell(
                {ability_id}, "{name}", AbilitySchool.{school}, {circle}, {min_val}, {max_val}, {mana_cost})
                .WithDescription("{desc}")
                .WithImpactEffect(0x376A, 0x1F2, 0x481));
'''
    elif code_type == "CreateBuffSpell":
        buff_type = "VystiaBuffType.AllStatsBuff"  # Default buff type
        duration = 30 if circle < 3 else 60
        return f'''            // {name} (Circle {circle})
            AbilityRegistry.RegisterAbility(AbilityDefinition.CreateBuffSpell(
                {ability_id}, "{name}", AbilitySchool.{school}, {circle}, {buff_type}, 10, {duration}, {mana_cost})
                .WithDescription("{desc}"));
'''
    elif code_type == "CreateDoTSpell":
        # CreateDoTSpell(id, name, school, circle, damagePerTick, durationSeconds, dmgType, manaCost)
        damage_per_tick = (min_val + max_val) // 2
        return f'''            // {name} (Circle {circle})
            AbilityRegistry.RegisterAbility(AbilityDefinition.CreateDoTSpell(
                {ability_id}, "{name}", AbilitySchool.{school}, {circle}, {damage_per_tick}, 15.0, {damage_enum}, {mana_cost})
                .WithDescription("{desc}"));
'''
    elif code_type == "CreateFinisher":
        return f'''            // {name} (Circle {circle})
            AbilityRegistry.RegisterAbility(AbilityDefinition.CreateFinisher(
                {ability_id}, "{name}", AbilitySchool.{school}, {min_val}, {max_val}, {mana_cost})
                .WithDescription("{desc}"));
'''
    else:  # builder pattern for complex abilities
        return f'''            // {name} (Circle {circle})
            AbilityRegistry.RegisterAbility(new AbilityDefinition()
                .WithId({ability_id})
                .WithName("{name}")
                .InSchool(AbilitySchool.{school})
                .InCircle({circle})
                .WithManaCost({mana_cost})
                .WithDescription("{desc}")
                .Targeting(AbilityTargetType.SingleTarget, 12)
                .WithBuff(VystiaBuffType.AllStatsBuff, 10, 30)
                .WithImpactEffect(0x36D4, 0x1E5, 0x481));
'''


def generate_class_file(class_name, class_data, start_id):
    """Generate a complete ability file for a martial class."""
    school = class_data["school"]
    damage_type = class_data["damage_type"]
    abilities = class_data["abilities"]

    code = f'''// Auto-generated by generate_martial_abilities.py
// Martial abilities for {class_name} class
// Do not edit manually - regenerate using the script

using System;
using Server;
using Server.Custom.VystiaClasses.Systems;

namespace Server.Custom.VystiaClasses.Abilities
{{
    public static class {class_name}Abilities
    {{
        public static void RegisterAll()
        {{
'''

    ability_id = start_id
    for i, (name, ability_type, min_val, max_val, desc) in enumerate(abilities):
        circle = (i // 4) + 1  # 4 abilities per circle
        code += generate_ability_code(ability_id, name, school, circle, ability_type, min_val, max_val, desc, damage_type)
        ability_id += 1

    code += f'''        }}
    }}
}}
'''
    return code


def generate_initializer(class_names):
    """Generate the martial ability initializer file."""
    code = '''// Auto-generated by generate_martial_abilities.py
// Initializer for all martial class abilities
// Do not edit manually - regenerate using the script

using System;
using Server;

namespace Server.Custom.VystiaClasses.Abilities
{
    public static class MartialAbilityInitializer
    {
        public static void RegisterAllMartialAbilities()
        {
            Console.WriteLine("[Vystia] Registering martial class abilities...");

'''

    for class_name in class_names:
        code += f'            {class_name}Abilities.RegisterAll();\n'

    code += f'''
            Console.WriteLine("[Vystia] Registered abilities from {len(class_names)} martial classes");
        }}
    }}
}}
'''
    return code


def main():
    """Generate all martial ability files."""
    # Create output directory
    os.makedirs(OUTPUT_DIR, exist_ok=True)

    # Track ability IDs
    current_id = MARTIAL_ABILITY_START
    class_names = []

    print("Generating martial abilities...")

    for class_name, class_data in MARTIAL_CLASSES.items():
        class_names.append(class_name)
        num_abilities = len(class_data["abilities"])

        # Generate class file
        code = generate_class_file(class_name, class_data, current_id)

        # Write file
        filename = os.path.join(OUTPUT_DIR, f"{class_name}Abilities.cs")
        with open(filename, "w", encoding="utf-8") as f:
            f.write(code)

        print(f"  Generated {class_name}Abilities.cs ({num_abilities} abilities, IDs {current_id}-{current_id + num_abilities - 1})")

        current_id += num_abilities

    # Generate initializer
    init_code = generate_initializer(class_names)
    init_filename = os.path.join(OUTPUT_DIR, "MartialAbilityInitializer.cs")
    with open(init_filename, "w", encoding="utf-8") as f:
        f.write(init_code)
    print(f"  Generated MartialAbilityInitializer.cs")

    total_abilities = current_id - MARTIAL_ABILITY_START
    print(f"\nComplete! Generated {total_abilities} martial abilities across {len(class_names)} classes")
    print(f"Ability ID range: {MARTIAL_ABILITY_START} - {current_id - 1}")
    print(f"\nOutput directory: {OUTPUT_DIR}")
    print("\nNext steps:")
    print("1. Update GeneratedAbilityInitializer.cs to call MartialAbilityInitializer.RegisterAllMartialAbilities()")
    print("2. Run 'dotnet build' to verify compilation")


if __name__ == "__main__":
    main()
