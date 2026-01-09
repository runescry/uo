#!/usr/bin/env python3
"""
Generate gameplay guides for all 26 Vystia classes
Based on Ice Mage template
"""

import os

OUTPUT_PATH = r'D:\UO\Vystia\guides'

# Class data from reference documents
CLASSES = {
    'Barbarian': {
        'region': 'Frosthold',
        'role': 'Melee DPS',
        'difficulty': '★★☆☆☆ (Easy)',
        'stats': {'STR': 45, 'DEX': 20, 'INT': 15},
        'skill': 'Berserking',
        'skill_id': 70,
        'spellbook': None,
        'theme': 'Rage, fury, powerful melee attacks, self-buffs',
        'special_item': 'RageTotem',
        'playstyle': 'Aggressive melee combat with rage mechanics',
        'strengths': ['High burst damage', 'Self-healing through combat', 'Rage generation', 'Strong vs single targets'],
        'weaknesses': ['No ranged attacks', 'Vulnerable to kiting', 'Limited crowd control', 'Rage depletes out of combat'],
        'stances': ['Berserker Stance (+damage, -defense)', 'Defensive Stance (+defense, -damage)', 'Battle Stance (balanced)'],
        'abilities': [
            ('Enrage', 'Instantly gain 50 rage', '60s cooldown'),
            ('Whirlwind', 'AoE spin attack', 'Costs 30 rage'),
            ('Execute', 'High damage vs low HP targets', 'Costs 20 rage'),
        ],
        'rotation': '1. Charge to engage\n2. Build rage with basic attacks\n3. Enrage when needed\n4. Whirlwind for groups\n5. Execute at <20% HP',
    },
    'Beastmaster': {
        'region': 'Frosthold',
        'role': 'Pet / Ranged',
        'difficulty': '★★★☆☆ (Intermediate)',
        'stats': {'STR': 25, 'DEX': 35, 'INT': 20},
        'skill': 'BeastBonding',
        'skill_id': 79,
        'spellbook': None,
        'theme': 'Animal companions, nature bond, ranged attacks with pet support',
        'special_item': 'BeastWhistle',
        'playstyle': 'Command pets while dealing ranged damage',
        'strengths': ['Pet tanking', 'Versatile pet types', 'Good solo potential', 'Ranged safety'],
        'weaknesses': ['Pet management complexity', 'Weak without pet', 'Pet can die', 'Split attention'],
        'stances': ['Pack Leader Stance (+pet damage)', 'Survival Stance (+pet defense)', 'Hunter Stance (+ranged damage)'],
        'abilities': [
            ('Call of the Wild', 'Summon temporary beast', '120s cooldown'),
            ('Beast Bond', 'Heal your pet significantly', '30s cooldown'),
            ('Feral Rage', 'Pet deals double damage for 10s', '60s cooldown'),
        ],
        'rotation': '1. Summon/command pet to attack\n2. Ranged attacks from safety\n3. Heal pet as needed\n4. Use Feral Rage for burst\n5. Call of Wild for tough fights',
    },
    'Sorcerer': {
        'region': 'Emberlands',
        'role': 'Caster DPS',
        'difficulty': '★★★☆☆ (Intermediate)',
        'stats': {'STR': 15, 'DEX': 20, 'INT': 45},
        'skill': 'Elementalism',
        'skill_id': 62,
        'spellbook': 'SorcererSpellbook',
        'theme': 'Fire, earth, air, water elemental magic',
        'special_item': 'SorcererSpellbook',
        'playstyle': 'Versatile elemental caster with multiple damage types',
        'strengths': ['Multiple damage types', 'Strong AoE', 'Elemental shields', 'Bypass specific resists'],
        'weaknesses': ['Mana intensive', 'Squishy', 'Long cast times', 'Element matching needed'],
        'stances': ['Fire Stance (+fire damage)', 'Earth Stance (+defense)', 'Storm Stance (+lightning)', 'Water Stance (+healing)'],
        'abilities': [
            ('Elemental Attunement', 'Switch active element instantly', '10s cooldown'),
            ('Elemental Surge', 'Empower next spell 50%', '45s cooldown'),
            ('Primordial Form', 'Transform into elemental for 20s', '180s cooldown'),
        ],
        'rotation': '1. Attune to target weakness\n2. Shield up before combat\n3. Open with empowered spell\n4. AoE for groups\n5. Switch elements as needed',
    },
    'Ranger': {
        'region': 'Desert',
        'role': 'Ranged DPS',
        'difficulty': '★★☆☆☆ (Easy)',
        'stats': {'STR': 25, 'DEX': 45, 'INT': 10},
        'skill': 'Marksmanship',
        'skill_id': 75,
        'spellbook': None,
        'theme': 'Archery, traps, tracking, survival',
        'special_item': None,
        'playstyle': 'Mobile ranged attacker with trap utility',
        'strengths': ['High sustained ranged damage', 'Traps for control', 'Good mobility', 'Tracking abilities'],
        'weaknesses': ['Limited magic', 'Weak in melee', 'Trap setup time', 'Arrow management'],
        'stances': ['Sniper Stance (+damage, -speed)', 'Rapid Fire Stance (+speed, -damage)', 'Survival Stance (+defense, mobility)'],
        'abilities': [
            ('Aimed Shot', 'Powerful charged shot', '15s cooldown'),
            ('Multi-Shot', 'Hit multiple targets', '20s cooldown'),
            ('Bear Trap', 'Root enemy for 4s', '30s cooldown'),
        ],
        'rotation': '1. Set traps at chokepoints\n2. Open with Aimed Shot\n3. Kite with normal shots\n4. Multi-Shot for groups\n5. Trap pursuers',
    },
    'Illusionist': {
        'region': 'Desert',
        'role': 'Caster CC',
        'difficulty': '★★★★☆ (Hard)',
        'stats': {'STR': 10, 'DEX': 25, 'INT': 45},
        'skill': 'IllusionMagic',
        'skill_id': 69,
        'spellbook': 'IllusionistSpellbook',
        'theme': 'Illusions, misdirection, mind control, phantasms',
        'special_item': 'IllusionistSpellbook',
        'playstyle': 'Control and confuse enemies with illusions',
        'strengths': ['Unmatched crowd control', 'Escape abilities', 'Confusion effects', 'Mirror images'],
        'weaknesses': ['Low direct damage', 'Complex mechanics', 'True sight counters', 'Mana hungry'],
        'stances': ['Phantom Stance (+illusion duration)', 'Mind Stance (+control effects)', 'Mirage Stance (+evasion)'],
        'abilities': [
            ('Mirror Image', 'Create 3 copies of yourself', '60s cooldown'),
            ('Mass Confusion', 'Enemies attack each other', '90s cooldown'),
            ('Vanish', 'Become invisible for 10s', '45s cooldown'),
        ],
        'rotation': '1. Mirror Image for safety\n2. Confusion on groups\n3. Phantasm for distraction\n4. Control priority targets\n5. Vanish if in danger',
    },
    'Witch': {
        'region': 'Shadowfen',
        'role': 'Debuffer',
        'difficulty': '★★★☆☆ (Intermediate)',
        'stats': {'STR': 15, 'DEX': 20, 'INT': 45},
        'skill': 'Hexcraft',
        'skill_id': 64,
        'spellbook': 'WitchSpellbook',
        'theme': 'Curses, hexes, debuffs, life drain',
        'special_item': 'WitchSpellbook',
        'playstyle': 'Weaken enemies while draining their life',
        'strengths': ['Powerful debuffs', 'Life drain sustain', 'DoT damage', 'Enemy crippling'],
        'weaknesses': ['Slow kill speed', 'Cleanse removes curses', 'Setup time', 'Less burst'],
        'stances': ['Curse Stance (+curse power)', 'Drain Stance (+life steal)', 'Hex Stance (+debuff duration)'],
        'abilities': [
            ('Mass Curse', 'Curse all nearby enemies', '60s cooldown'),
            ('Life Tap', 'Convert HP to mana', '30s cooldown'),
            ('Doom', 'Target dies in 10s unless healed', '120s cooldown'),
        ],
        'rotation': '1. Apply curses first\n2. Stack debuffs\n3. Life drain for sustain\n4. DoTs on all targets\n5. Doom on priority target',
    },
    'Warlock': {
        'region': 'ShadowVoid',
        'role': 'Caster DPS',
        'difficulty': '★★★★☆ (Hard)',
        'stats': {'STR': 15, 'DEX': 20, 'INT': 45},
        'skill': 'Demonology',
        'skill_id': 59,
        'spellbook': 'WarlockSpellbook',
        'theme': 'Demons, shadow magic, chaos, dark pacts',
        'special_item': 'WarlockSpellbook',
        'playstyle': 'Summon demons and unleash shadow destruction',
        'strengths': ['Demon pets', 'High shadow damage', 'Fear effects', 'Chaos magic'],
        'weaknesses': ['Self-damage from pacts', 'Complex pet management', 'Long cooldowns', 'Weak to holy'],
        'stances': ['Destruction Stance (+damage)', 'Demonology Stance (+pet power)', 'Affliction Stance (+DoTs)'],
        'abilities': [
            ('Summon Demon', 'Call forth a demon minion', '180s cooldown'),
            ('Dark Pact', 'Sacrifice HP for massive damage', '60s cooldown'),
            ('Demonic Form', 'Transform into demon', '300s cooldown'),
        ],
        'rotation': '1. Summon demon first\n2. Apply corruption DoTs\n3. Shadow bolts for damage\n4. Dark Pact for burst\n5. Fear fleeing enemies',
    },
    'Druid': {
        'region': 'Verdantpeak',
        'role': 'Healer / Hybrid',
        'difficulty': '★★★★☆ (Hard)',
        'stats': {'STR': 20, 'DEX': 25, 'INT': 35},
        'skill': 'Druidism',
        'skill_id': 61,
        'spellbook': 'DruidSpellbook',
        'theme': 'Nature magic, shapeshifting, healing, plants',
        'special_item': 'DruidSpellbook, ShapeshiftTotem',
        'playstyle': 'Versatile shapeshifter who can heal, tank, or DPS',
        'strengths': ['Shapeshifting versatility', 'Strong healing', 'Nature damage', 'HoTs'],
        'weaknesses': ['Jack of all trades', 'Form-locked abilities', 'Complex management', 'Moderate at everything'],
        'stances': ['Caster Form (spells)', 'Bear Form (tank)', 'Wolf Form (melee DPS)', 'Hawk Form (mobility)'],
        'abilities': [
            ('Shapeshift', 'Change into animal form', '10s cooldown'),
            ('Rejuvenation', 'Powerful HoT on target', '15s cooldown'),
            ('Natures Grasp', 'Root attackers automatically', '60s cooldown'),
        ],
        'rotation': 'Caster: HoTs → Direct heals → Nature damage\nBear: Roar → Maul → Swipe\nWolf: Pounce → Bite → Claw',
    },
    'Alchemist': {
        'region': 'Verdantpeak',
        'role': 'Support',
        'difficulty': '★★★☆☆ (Intermediate)',
        'stats': {'STR': 20, 'DEX': 30, 'INT': 30},
        'skill': 'Transmutation',
        'skill_id': 81,
        'spellbook': None,
        'theme': 'Potions, transmutation, bombs, elixirs',
        'special_item': 'AlchemistKit',
        'playstyle': 'Support allies with potions and debuff enemies with bombs',
        'strengths': ['Strong buffs', 'Versatile utility', 'Potion crafting', 'AoE bombs'],
        'weaknesses': ['Consumable dependent', 'Setup time', 'Limited direct combat', 'Resource management'],
        'stances': ['Brewer Stance (+potion power)', 'Bomber Stance (+explosive damage)', 'Transmuter Stance (+buff duration)'],
        'abilities': [
            ('Quick Brew', 'Instantly create a potion', '30s cooldown'),
            ('Volatile Mixture', 'Throw AoE damage bomb', '20s cooldown'),
            ('Elixir of Power', 'Major buff to target', '60s cooldown'),
        ],
        'rotation': '1. Pre-buff party with elixirs\n2. Volatile Mixture opener\n3. Support with potions\n4. Bombs for AoE\n5. Transmute as needed',
    },
    'Oracle': {
        'region': 'Crystal Barrens',
        'role': 'Utility',
        'difficulty': '★★★★★ (Expert)',
        'stats': {'STR': 10, 'DEX': 20, 'INT': 50},
        'skill': 'Divination',
        'skill_id': 65,
        'spellbook': 'OracleSpellbook',
        'theme': 'Prophecy, time manipulation, fate, foresight',
        'special_item': 'OracleSpellbook, CrystalOrb',
        'playstyle': 'Manipulate time and fate to control battle outcomes',
        'strengths': ['Time manipulation', 'Fate changing', 'Precognition', 'Unique utility'],
        'weaknesses': ['Very complex', 'Low damage', 'Support focused', 'High skill cap'],
        'stances': ['Seer Stance (+divination)', 'Chrono Stance (+time effects)', 'Fate Stance (+luck manipulation)'],
        'abilities': [
            ('Foresight', 'See next 3 enemy actions', '60s cooldown'),
            ('Time Rewind', 'Undo last 5 seconds', '180s cooldown'),
            ('Alter Fate', 'Change a failed roll to success', '120s cooldown'),
        ],
        'rotation': '1. Foresight to plan\n2. Buff allies with fortune\n3. Debuff enemies with ill fate\n4. Time Rewind emergencies\n5. Alter Fate critical moments',
    },
    'Artificer': {
        'region': 'Ironclad',
        'role': 'Pet / Ranged',
        'difficulty': '★★★★☆ (Hard)',
        'stats': {'STR': 25, 'DEX': 30, 'INT': 25},
        'skill': 'Engineering',
        'skill_id': 80,
        'spellbook': None,
        'theme': 'Clockwork constructs, gadgets, turrets, steam power',
        'special_item': 'ConstructControlDevice, ArtificerBlueprints',
        'playstyle': 'Deploy constructs and gadgets to control the battlefield',
        'strengths': ['Multiple pets/turrets', 'Gadget utility', 'Zone control', 'Mechanical allies'],
        'weaknesses': ['Setup time', 'Constructs can be destroyed', 'Resource intensive', 'Complex management'],
        'stances': ['Constructor Stance (+pet power)', 'Gadgeteer Stance (+gadget effects)', 'Engineer Stance (+turret damage)'],
        'abilities': [
            ('Deploy Turret', 'Place an auto-attacking turret', '45s cooldown'),
            ('Clockwork Companion', 'Summon combat construct', '120s cooldown'),
            ('Overcharge', 'Empower all constructs for 15s', '90s cooldown'),
        ],
        'rotation': '1. Deploy turrets at chokepoints\n2. Summon construct\n3. Gadgets for utility\n4. Overcharge for burst\n5. Repair constructs',
    },
    'Fighter': {
        'region': 'Ironclad',
        'role': 'Tank',
        'difficulty': '★☆☆☆☆ (Very Easy)',
        'stats': {'STR': 40, 'DEX': 25, 'INT': 15},
        'skill': 'CombatMastery',
        'skill_id': 76,
        'spellbook': None,
        'theme': 'Weapons mastery, defensive skills, taunts, armor',
        'special_item': None,
        'playstyle': 'Straightforward melee tank with high survivability',
        'strengths': ['High defense', 'Simple mechanics', 'Reliable tanking', 'Weapon mastery'],
        'weaknesses': ['Low damage', 'No magic', 'Limited range', 'Basic gameplay'],
        'stances': ['Shield Stance (+defense)', 'Sword Stance (+damage)', 'Balance Stance (even stats)'],
        'abilities': [
            ('Taunt', 'Force enemy to attack you', '10s cooldown'),
            ('Shield Wall', '+50% defense for 10s', '60s cooldown'),
            ('Charge', 'Rush to enemy and stun', '20s cooldown'),
        ],
        'rotation': '1. Charge to engage\n2. Taunt priority targets\n3. Shield Wall when low\n4. Block heavy attacks\n5. Maintain aggro',
    },
    'Monk': {
        'region': 'Ironclad',
        'role': 'Melee / Hybrid',
        'difficulty': '★★★☆☆ (Intermediate)',
        'stats': {'STR': 30, 'DEX': 35, 'INT': 15},
        'skill': 'MartialArts',
        'skill_id': 72,
        'spellbook': None,
        'theme': 'Martial arts, chi, meditation, unarmed combat',
        'special_item': 'MonkBeads',
        'playstyle': 'Fast unarmed striker with chi-powered abilities',
        'strengths': ['Fast attacks', 'Chi resource', 'Evasion', 'Self-healing through chi'],
        'weaknesses': ['Unarmed only', 'Squishy if hit', 'Chi management', 'Close range'],
        'stances': ['Tiger Stance (+damage)', 'Crane Stance (+evasion)', 'Dragon Stance (+chi regen)'],
        'abilities': [
            ('Flurry of Blows', 'Rapid 5-hit combo', '15s cooldown'),
            ('Chi Heal', 'Convert chi to health', '20s cooldown'),
            ('Flying Kick', 'Gap closer with stun', '25s cooldown'),
        ],
        'rotation': '1. Flying Kick to engage\n2. Build chi with basics\n3. Flurry of Blows burst\n4. Chi Heal when needed\n5. Evade heavy attacks',
    },
    'Templar': {
        'region': 'Ironclad',
        'role': 'Tank / DPS',
        'difficulty': '★★★☆☆ (Intermediate)',
        'stats': {'STR': 35, 'DEX': 25, 'INT': 20},
        'skill': 'Zealotry',
        'skill_id': 77,
        'spellbook': None,
        'theme': 'Holy warrior, divine punishment, righteous fury',
        'special_item': 'TemplarCross',
        'playstyle': 'Holy warrior dealing divine damage while tanking',
        'strengths': ['Holy damage', 'Self-buffs', 'Anti-undead', 'Hybrid tank/DPS'],
        'weaknesses': ['Split role', 'Zealotry resource', 'Moderate at both roles', 'Limited healing'],
        'stances': ['Crusader Stance (+holy damage)', 'Guardian Stance (+defense)', 'Zealot Stance (+zealotry gain)'],
        'abilities': [
            ('Divine Strike', 'Holy damage attack', '10s cooldown'),
            ('Holy Shield', 'Block + reflect holy', '30s cooldown'),
            ('Smite', 'High damage vs undead/demons', '20s cooldown'),
        ],
        'rotation': '1. Charge with Divine Strike\n2. Holy Shield for defense\n3. Smite undead/demons\n4. Build zealotry\n5. Execute with holy burst',
    },
    'Necromancer': {
        'region': 'ShadowVoid',
        'role': 'Caster / Pet',
        'difficulty': '★★★★☆ (Hard)',
        'stats': {'STR': 15, 'DEX': 15, 'INT': 50},
        'skill': 'NecromancyArts',
        'skill_id': 60,
        'spellbook': 'NecromancerSpellbook',
        'theme': 'Undead summoning, death magic, life drain, decay',
        'special_item': 'NecromancerSpellbook',
        'playstyle': 'Command undead armies while dealing death magic',
        'strengths': ['Multiple pets', 'Life drain sustain', 'Corpse explosion', 'Fear effects'],
        'weaknesses': ['Needs corpses', 'Weak to holy', 'Pet management', 'Ethical concerns'],
        'stances': ['Unholy Stance (+pet power)', 'Death Stance (+damage)', 'Blood Stance (+life drain)'],
        'abilities': [
            ('Raise Dead', 'Create skeleton from corpse', '30s cooldown'),
            ('Army of Dead', 'Raise multiple skeletons', '120s cooldown'),
            ('Death Coil', 'Damage enemy, heal you', '15s cooldown'),
        ],
        'rotation': '1. Kill weak enemy for corpse\n2. Raise Dead army\n3. Death Coil for sustain\n4. DoTs on targets\n5. Corpse Explosion finish',
    },
    'Summoner': {
        'region': 'Underwater',
        'role': 'Pet / Caster',
        'difficulty': '★★★★☆ (Hard)',
        'stats': {'STR': 15, 'DEX': 20, 'INT': 45},
        'skill': 'Conjuration',
        'skill_id': 66,
        'spellbook': 'SummonerSpellbook',
        'theme': 'Creature summoning, planar bonds, elemental conjuration',
        'special_item': 'SummonerSpellbook, SummoningCircle',
        'playstyle': 'Summon and command powerful elemental creatures',
        'strengths': ['Powerful summons', 'Elemental variety', 'Strong pets', 'Planar magic'],
        'weaknesses': ['Summon limits', 'Cast times', 'Mana intensive', 'Pet deaths hurt'],
        'stances': ['Conjurer Stance (+summon duration)', 'Binder Stance (+summon power)', 'Elemental Stance (+elemental damage)'],
        'abilities': [
            ('Greater Summon', 'Call powerful creature', '180s cooldown'),
            ('Planar Binding', 'Strengthen summon greatly', '60s cooldown'),
            ('Dismiss', 'Explode summon for AoE', '30s cooldown'),
        ],
        'rotation': '1. Summon before combat\n2. Planar Binding buff\n3. Support summon with spells\n4. Dismiss for AoE burst\n5. Re-summon as needed',
    },
    'Bounty Hunter': {
        'region': 'Multi-Regional',
        'role': 'Ranged / Melee',
        'difficulty': '★★★☆☆ (Intermediate)',
        'stats': {'STR': 30, 'DEX': 35, 'INT': 15},
        'skill': 'Manhunting',
        'skill_id': 78,
        'spellbook': None,
        'theme': 'Tracking, traps, bounties, assassination',
        'special_item': 'BountyLedger',
        'playstyle': 'Hunt and eliminate marked targets with precision',
        'strengths': ['Target marking', 'Tracking', 'Burst damage on marked', 'Versatile combat'],
        'weaknesses': ['Setup required', 'Mark cooldowns', 'Less group utility', 'Single target focus'],
        'stances': ['Hunter Stance (+tracking)', 'Assassin Stance (+burst)', 'Survivor Stance (+defense)'],
        'abilities': [
            ('Mark Target', 'Mark for bonus damage', '30s cooldown'),
            ('Execute Contract', 'Massive damage to marked', '60s cooldown'),
            ('Track Prey', 'Reveal marked target location', '45s cooldown'),
        ],
        'rotation': '1. Track and find target\n2. Mark Target for bonus\n3. Engage with traps\n4. Burst damage combo\n5. Execute Contract finish',
    },
    'Knight': {
        'region': 'Multi-Regional',
        'role': 'Tank / Melee',
        'difficulty': '★★☆☆☆ (Easy)',
        'stats': {'STR': 38, 'DEX': 27, 'INT': 15},
        'skill': 'ChivalricArts',
        'skill_id': 73,
        'spellbook': None,
        'theme': 'Chivalry, honor, mounted combat, leadership',
        'special_item': 'KnightBanner',
        'playstyle': 'Honorable warrior with party leadership abilities',
        'strengths': ['Party buffs', 'Mounted combat', 'Honor mechanics', 'Reliable tank'],
        'weaknesses': ['Honor restrictions', 'Mount dependency', 'Limited magic', 'Code of conduct'],
        'stances': ['Cavalry Stance (+mounted damage)', 'Defender Stance (+party defense)', 'Champion Stance (+honor gain)'],
        'abilities': [
            ('Rally', 'Buff all nearby allies', '60s cooldown'),
            ('Charge', 'Mounted charge attack', '20s cooldown'),
            ('Challenge', 'Force 1v1 with enemy', '45s cooldown'),
        ],
        'rotation': '1. Rally party before fight\n2. Mounted Charge engage\n3. Challenge priority target\n4. Defend allies\n5. Honorable combat',
    },
    'Shaman': {
        'region': 'Multi-Regional',
        'role': 'Healer / Hybrid',
        'difficulty': '★★★☆☆ (Intermediate)',
        'stats': {'STR': 20, 'DEX': 20, 'INT': 40},
        'skill': 'SpiritCalling',
        'skill_id': 67,
        'spellbook': 'ShamanSpellbook',
        'theme': 'Spirits, totems, lightning, ancestral magic',
        'special_item': 'ShamanSpellbook, SpiritTotem',
        'playstyle': 'Call upon spirits and totems to heal and damage',
        'strengths': ['Totem utility', 'Spirit healing', 'Lightning damage', 'Versatile support'],
        'weaknesses': ['Totem positioning', 'Spirit management', 'Mobile enemies avoid totems', 'Split focus'],
        'stances': ['Spirit Stance (+healing)', 'Storm Stance (+lightning)', 'Totem Stance (+totem power)'],
        'abilities': [
            ('Spirit Link', 'Share damage among party', '60s cooldown'),
            ('Lightning Storm', 'AoE lightning damage', '30s cooldown'),
            ('Ancestral Spirit', 'Summon healing spirit', '90s cooldown'),
        ],
        'rotation': '1. Place totems strategically\n2. Spirit Link on tank\n3. Heal with spirit spells\n4. Lightning Storm damage\n5. Ancestral Spirit emergency',
    },
    'Wizard': {
        'region': 'Crystal Barrens',
        'role': 'Utility',
        'difficulty': '★★★★☆ (Hard)',
        'stats': {'STR': 10, 'DEX': 20, 'INT': 50},
        'skill': 'ArcaneStudies',
        'skill_id': 83,
        'spellbook': None,
        'theme': 'Arcane research, spell modification, magical theory',
        'special_item': None,
        'playstyle': 'Modify and enhance spells through arcane knowledge',
        'strengths': ['Spell modification', 'Arcane utility', 'Research bonuses', 'Flexible magic'],
        'weaknesses': ['Study requirements', 'Complex mechanics', 'Less direct power', 'Knowledge gated'],
        'stances': ['Research Stance (+learning)', 'Combat Stance (+damage)', 'Utility Stance (+spell effects)'],
        'abilities': [
            ('Arcane Insight', 'Analyze enemy weaknesses', '30s cooldown'),
            ('Spell Modification', 'Enhance next spell', '20s cooldown'),
            ('Arcane Explosion', 'Raw arcane damage', '45s cooldown'),
        ],
        'rotation': '1. Arcane Insight to analyze\n2. Modify spells for weakness\n3. Arcane Explosion burst\n4. Utility spells support\n5. Adapt to situation',
    },
    'Cleric': {
        'region': 'Multi-Regional',
        'role': 'Healer',
        'difficulty': '★★☆☆☆ (Easy)',
        'stats': {'STR': 20, 'DEX': 20, 'INT': 40},
        'skill': 'DivineGrace',
        'skill_id': 82,
        'spellbook': None,
        'theme': 'Divine healing, holy magic, protection, blessings',
        'special_item': 'HolySymbol',
        'playstyle': 'Dedicated healer with powerful divine magic',
        'strengths': ['Best single target heals', 'Divine buffs', 'Resurrection', 'Anti-undead'],
        'weaknesses': ['Low damage', 'Heal focused', 'Target of enemies', 'Mana management'],
        'stances': ['Holy Stance (+healing)', 'Protection Stance (+shields)', 'Smite Stance (+vs undead)'],
        'abilities': [
            ('Divine Heal', 'Powerful single heal', '10s cooldown'),
            ('Resurrection', 'Revive dead ally', '300s cooldown'),
            ('Holy Nova', 'AoE heal + damage undead', '30s cooldown'),
        ],
        'rotation': '1. Bless party before fight\n2. Shield tank\n3. Heal priority targets\n4. Holy Nova for AoE heal\n5. Resurrect fallen',
    },
    'Paladin': {
        'region': 'Multi-Regional',
        'role': 'Tank / Healer',
        'difficulty': '★★★☆☆ (Intermediate)',
        'stats': {'STR': 35, 'DEX': 20, 'INT': 25},
        'skill': 'HolyDevotion',
        'skill_id': 74,
        'spellbook': None,
        'theme': 'Holy knight, protection, healing, divine justice',
        'special_item': None,
        'playstyle': 'Holy tank who can heal and deal divine damage',
        'strengths': ['Self-healing tank', 'Divine damage', 'Auras', 'Anti-evil'],
        'weaknesses': ['Split stats', 'Moderate at both roles', 'Holy restrictions', 'Mana limits'],
        'stances': ['Devotion Stance (+healing aura)', 'Retribution Stance (+damage)', 'Protection Stance (+defense)'],
        'abilities': [
            ('Lay on Hands', 'Full heal on target', '600s cooldown'),
            ('Divine Shield', 'Invulnerable for 8s', '300s cooldown'),
            ('Consecration', 'Holy ground damage', '15s cooldown'),
        ],
        'rotation': '1. Aura active always\n2. Consecration on pull\n3. Self-heal as needed\n4. Divine Shield emergency\n5. Lay on Hands saves',
    },
    'Bard': {
        'region': 'Multi-Regional',
        'role': 'Support / CC',
        'difficulty': '★★★☆☆ (Intermediate)',
        'stats': {'STR': 15, 'DEX': 35, 'INT': 30},
        'skill': 'BardicLore',
        'skill_id': 63,
        'spellbook': 'BardSpellbook',
        'theme': 'Music, songs, inspiration, crowd control',
        'special_item': 'BardSpellbook, MagicLute',
        'playstyle': 'Inspire allies and debilitate enemies with music',
        'strengths': ['Party buffs', 'Enemy debuffs', 'Crowd control', 'Versatile support'],
        'weaknesses': ['Song management', 'Interrupt vulnerable', 'Low solo damage', 'Support dependent'],
        'stances': ['War Song Stance (+party damage)', 'Ballad Stance (+party defense)', 'Lullaby Stance (+CC)'],
        'abilities': [
            ('Inspiring Song', 'Buff all party stats', '60s cooldown'),
            ('Discordant Note', 'AoE confusion', '30s cooldown'),
            ('Battle Hymn', 'Major party damage buff', '120s cooldown'),
        ],
        'rotation': '1. Inspiring Song pre-fight\n2. Battle Hymn on pull\n3. Maintain buff songs\n4. Discordant Note CC\n5. Debuff priority targets',
    },
    'Enchanter': {
        'region': 'Multi-Regional',
        'role': 'Utility / Buff',
        'difficulty': '★★★★☆ (Hard)',
        'stats': {'STR': 15, 'DEX': 25, 'INT': 40},
        'skill': 'Runeweaving',
        'skill_id': 68,
        'spellbook': 'EnchanterSpellbook',
        'theme': 'Enchantments, runes, item enhancement, magical buffs',
        'special_item': 'EnchanterSpellbook, EnchantingCrystal',
        'playstyle': 'Enhance allies and items with powerful enchantments',
        'strengths': ['Item enhancement', 'Powerful buffs', 'Rune magic', 'Party empowerment'],
        'weaknesses': ['Setup time', 'Buff management', 'Low direct damage', 'Resource intensive'],
        'stances': ['Enchant Stance (+buff power)', 'Rune Stance (+rune effects)', 'Empower Stance (+item enhancement)'],
        'abilities': [
            ('Greater Enchant', 'Major weapon buff', '60s cooldown'),
            ('Rune of Power', 'Place empowering rune', '45s cooldown'),
            ('Mass Enchant', 'Buff entire party', '120s cooldown'),
        ],
        'rotation': '1. Enchant party weapons\n2. Place power runes\n3. Maintain buffs\n4. Rune damage/utility\n5. Re-enchant as needed',
    },
}

def generate_guide(class_name, data):
    """Generate a complete gameplay guide for a class"""

    skill_line = f"**{data['skill']} (ID: {data['skill_id']})** - Governs all {class_name} abilities"

    spellbook_section = ""
    if data['spellbook']:
        spellbook_section = f"""### Spellbook
**{data['spellbook']}**
- 32 spells across 8 circles
- Spawn: `[spellbook {class_name.lower()}]` or `[sb {class_name.lower()}]`

"""

    special_item_section = ""
    if data['special_item']:
        special_item_section = f"**Special Item:** {data['special_item']}\n"

    strengths = '\n'.join([f"- {s}" for s in data['strengths']])
    weaknesses = '\n'.join([f"- {w}" for w in data['weaknesses']])
    stances = '\n'.join([f"- **{s.split('(')[0].strip()}** {s[s.find('('):] if '(' in s else ''}" for s in data['stances']])

    abilities_section = ""
    for name, desc, cooldown in data['abilities']:
        abilities_section += f"""#### {name}
- **Effect:** {desc}
- **Cooldown:** {cooldown}

"""

    guide = f"""# {class_name} - Complete Gameplay Guide

**Class:** {class_name}
**Region:** {data['region']}
**Role:** {data['role']}
**Difficulty:** {data['difficulty']}

---

## Overview

{data['theme'].capitalize()}. {data['playstyle']}.

### Base Stats
| Stat | Value |
|------|-------|
| STR | {data['stats']['STR']} |
| DEX | {data['stats']['DEX']} |
| INT | {data['stats']['INT']} |

### Primary Skill
{skill_line}

{spellbook_section}{special_item_section}
---

## Playstyle Philosophy

### Core Concept
{data['playstyle']}

### Strengths
{strengths}

### Weaknesses
{weaknesses}

---

## Stance System

{class_name}s have access to the following stances:

{stances}

**Stance Commands:**
```
[stance {data['stances'][0].split()[0].lower()}]
[stance {data['stances'][1].split()[0].lower()}]
[stance {data['stances'][2].split()[0].lower()}]
```

---

## Abilities

{abilities_section}
---

## Combat Rotation

```
{data['rotation']}
```

---

## Tips & Tricks

### General Tips
1. **Know your role** - {data['role']} means focusing on that aspect
2. **Use stances** - Switch stances based on situation
3. **Manage resources** - Don't spam abilities unnecessarily
4. **Position well** - {'Stay at range' if 'Ranged' in data['role'] or 'Caster' in data['role'] else 'Stay in melee range'}

### Advanced Techniques
- Combine abilities for maximum effect
- Pre-buff before engaging tough enemies
- Know when to retreat and reset

---

## Leveling Path

### Levels 1-20
- Learn basic abilities
- Practice {data['role'].split('/')[0].lower()} fundamentals
- Understand resource management

### Levels 21-40
- Add more abilities to rotation
- Start using stances effectively
- Group content practice

### Levels 41-60
- Master full rotation
- Optimize for your role
- Advanced stance switching

### Levels 61+
- Full mastery
- Min-max for endgame
- Raid/PvP optimization

---

## GM Commands for Testing

```
[class {class_name.lower().replace(' ', '')}]  - Apply class
[svs 100]                                       - Set skills to 100
[stance {data['stances'][0].split()[0].lower()}] - Set stance
```

---

*Guide Version: 1.0*
*Last Updated: 2025-12-11*
*For Vystia Shard - {class_name} Class*
"""
    return guide

def main():
    # Create output directory
    os.makedirs(OUTPUT_PATH, exist_ok=True)

    print("="*70)
    print("  GENERATING CLASS GAMEPLAY GUIDES")
    print("="*70)
    print()

    generated = 0

    for class_name, data in CLASSES.items():
        filename = f"{class_name.upper().replace(' ', '_')}_GUIDE.md"
        filepath = os.path.join(OUTPUT_PATH, filename)

        # Skip Ice Mage (already created manually with full detail)
        if class_name == 'Ice Mage':
            print(f"Skipping {class_name} (manual guide exists)")
            continue

        guide_content = generate_guide(class_name, data)

        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(guide_content)

        print(f"Generated: {filename}")
        generated += 1

    print()
    print("="*70)
    print(f"  COMPLETE: {generated} guides generated")
    print("="*70)
    print()
    print(f"Location: {OUTPUT_PATH}")
    print()

if __name__ == "__main__":
    main()
