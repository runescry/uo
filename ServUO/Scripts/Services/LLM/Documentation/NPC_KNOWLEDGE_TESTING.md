# LLM NPC Knowledge Testing Guide

This guide provides comprehensive test cases for validating the LLM NPC knowledge system implementation.

## Prerequisites

- ServUO server running with LLM service enabled
- OpenAI API configured OR Ollama running with required models
- All domain JSON files loaded (20 domain files, ~230+ lore entries)
- Test NPCs spawned with appropriate personalities

## Quick Spawn Commands

```
[SpawnPersonalityNPC Mage Archaic
[SpawnPersonalityNPC Merchant Formal
[SpawnPersonalityNPC Guard Formal
[SpawnPersonalityNPC Healer OldEnglish
[SpawnPersonalityNPC Sage OldEnglish
[SpawnPersonalityNPC Commoner Casual
[SpawnPersonalityNPC Blacksmith Archaic
[SpawnPersonalityNPC InnKeeper Casual
[SpawnPersonalityNPC Banker Formal
[SpawnPersonalityNPC AnimalTrainer Casual
[SpawnPersonalityNPC Ranger Casual
[SpawnPersonalityNPC Cook Casual
[SpawnPersonalityNPC Farmer Casual
[SpawnPersonalityNPC Fisherman Casual
[SpawnPersonalityNPC Miner Casual
[SpawnPersonalityNPC Tinker Casual
[SpawnPersonalityNPC Provisioner Formal
[SpawnPersonalityNPC Scribe Formal
[SpawnPersonalityNPC Bard Casual
[SpawnPersonalityNPC Mapmaker Formal
[SpawnPersonalityNPC RealEstateBroker Formal
[SpawnPersonalityNPC Scholar OldEnglish
[SpawnPersonalityNPC Noble Formal
[SpawnPersonalityNPC Peasant Casual
[SpawnPersonalityNPC TownCrier Casual

[SpawnTownNPCs
[SpawnMagicNPCs
[SpawnAdventurerNPCs
```

## Domain Knowledge System Overview

The LLM NPC system now includes **20 domain-specific JSON files** covering all player-interactive NPC types:

### Domain Files
1. `britannia_general.json` - General world knowledge (History, NPC, Location, Dungeon, Virtue)
2. `crafting_domain.json` - All crafting professions (Blacksmith, Alchemy, Bowyer, Tailor, Jeweler, Carpenter, LeatherWorker)
3. `magic_domain.json` - Magic theory, reagents, spell circles
4. `combat_domain.json` - Combat skills, mechanics, tactics
5. `creatures_domain.json` - Creatures and monsters
6. `healing_domain.json` - Healing, herbs, potions, diseases
7. `trade_domain.json` - Trade, currency, appraisal
8. `hospitality_domain.json` - InnKeeper, Barkeeper
9. `finance_domain.json` - Banker
10. `animal_domain.json` - AnimalTrainer
11. `wilderness_domain.json` - Ranger
12. `food_domain.json` - Cook, Farmer, Fisherman
13. `resource_domain.json` - Miner
14. `technology_domain.json` - Tinker
15. `supply_domain.json` - Provisioner
16. `knowledge_domain.json` - Scribe, Scholar
17. `entertainment_domain.json` - Bard, Actor, Artist
18. `geography_domain.json` - Mapmaker, RealEstateBroker
19. `social_domain.json` - Commoner, Peasant, Noble, TownCrier, Gypsy, HairStylist, Shipwright
20. `spells_generated.json` - Individual spells

**Total:** ~230+ lore entries across all domains

---

## Phase 1 Knowledge Testing (51 Entries - COMPLETED)

### Reagent Knowledge (16 Entries)

#### Test: Mage NPC - Reagent Expertise

**NPC Type:** Mage (Expert), Alchemist (Expert)
**Expected Knowledge Level:** Expert (can discuss in detail)

**Test Questions:**
1. "What is black pearl used for?"
   - ✅ Expected: Detailed explanation of black pearl in magery, specific spell categories
   - ✅ Should mention: 3rd-8th circle spells, connection to water/sea

2. "Tell me about bloodmoss"
   - ✅ Expected: Necromancy reagent, specific uses
   - ✅ Should mention: Dark magic, necromantic spells

3. "What's the difference between mandrake root and ginseng?"
   - ✅ Expected: Compare their uses, spell circles, properties
   - ✅ Mandrake: powerful offensive spells, high-circle magic
   - ✅ Ginseng: healing spells, restorative magic

4. "I need reagents for a summoning spell, what should I get?"
   - ✅ Expected: Recommend appropriate reagents based on spell type
   - ✅ Should mention: Blood moss, mandrake root, spider silk

5. "Where can I find sulfurous ash?"
   - ✅ Expected: Locations, how to obtain, uses in fire magic

#### Test: Merchant NPC - Reagent Trade Knowledge

**NPC Type:** Merchant (Proficient)
**Expected Knowledge Level:** Proficient (knows what to stock and sell)

**Test Questions:**
1. "What reagents should I stock in my shop?"
   - ✅ Expected: Recommend common reagents, mention demand
   - ✅ Should prioritize: Black pearl, blood moss, garlic, ginseng, mandrake root

2. "Which reagents are most valuable?"
   - ✅ Expected: Discuss rarity and demand
   - ✅ Should mention: Blood moss (necromancy), mandrake root (high circles)

#### Test: Commoner NPC - Limited Reagent Knowledge

**NPC Type:** Commoner (Aware)
**Expected Knowledge Level:** Aware (basic knowledge, should refer to expert)

**Test Questions:**
1. "What is garlic used for?"
   - ✅ Expected: Basic knowledge that it's a magic reagent, protection uses
   - ⚠️ Should mention: "A mage could tell you more" or similar referral

2. "How do I use nightshade?"
   - ❌ Expected: Should refer to Mage or Alchemist
   - ✅ Might mention: Knows it's dangerous/poisonous

### Wood Types Knowledge (7 Entries)

#### Test: Carpenter NPC - Wood Expertise

**NPC Type:** Merchant (should be Carpenter - note for future profession expansion)
**Expected Knowledge Level:** Expert

**Test Questions:**
1. "What's the best wood for crafting a strong bow?"
   - ✅ Expected: Recommend yew, frostwood, or heartwood
   - ✅ Should explain: Wood properties, durability bonuses

2. "Tell me about frostwood"
   - ✅ Expected: Detailed properties, where to harvest, uses
   - ✅ Should mention: Cold resistance, rare, specific locations

3. "What's the difference between oak and ash?"
   - ✅ Expected: Compare properties, uses, availability

4. "I need wood that's easy to work with for basic furniture"
   - ✅ Expected: Recommend regular/plain wood or oak
   - ✅ Should explain: Easier to work, readily available

#### Test: Bowyer NPC - Wood for Weapons

**NPC Type:** Merchant (should be Bowyer)
**Expected Knowledge Level:** Expert for weapon-grade wood

**Test Questions:**
1. "What wood makes the best crossbow?"
   - ✅ Expected: Recommend yew or better exotic woods
   - ✅ Should mention: Damage bonuses, durability

2. "Is heartwood good for bows?"
   - ✅ Expected: Detailed answer about heartwood properties for ranged weapons

### Leather Types Knowledge (4 Entries)

#### Test: Leatherworker NPC - Leather Expertise

**NPC Type:** Merchant (should be Leatherworker/Tailor)
**Expected Knowledge Level:** Expert

**Test Questions:**
1. "What leather should I use for studded armor?"
   - ✅ Expected: Recommend based on desired properties
   - ✅ Should mention: Spined (luck), horned (physical resist), barbed (all resists)

2. "Tell me about barbed leather"
   - ✅ Expected: Detailed explanation of properties, sources, uses
   - ✅ Should mention: High-end armor, resistance bonuses, harder to obtain

3. "What's the difference between spined and horned leather?"
   - ✅ Expected: Compare bonuses and uses
   - ✅ Spined: Luck bonus
   - ✅ Horned: Physical resistance

4. "I need leather for basic armor, what do you recommend?"
   - ✅ Expected: Regular leather for beginners, explain upgrade path

### Alchemy Potion Knowledge (9 Entries)

#### Test: Alchemist NPC - Potion Expertise

**NPC Type:** Merchant with Alchemist specialty
**Expected Knowledge Level:** Expert

**Test Questions:**
1. "How do I make a greater healing potion?"
   - ✅ Expected: Recipe, reagents (ginseng), skill requirements
   - ✅ Should explain: Difference from lesser/normal healing

2. "What potions should I bring for dungeon crawling?"
   - ✅ Expected: Recommend healing, cure, refresh, possibly night sight
   - ✅ Should explain: Why each is useful

3. "Tell me about explosion potions"
   - ✅ Expected: Damage, uses, ingredients, safety warnings
   - ✅ Should mention: PvP uses, alchemy skill affects damage

4. "What's the cure for deadly poison?"
   - ✅ Expected: Greater cure potion required
   - ✅ Should explain: Potion strength must match poison level

5. "Can I make invisibility potions?"
   - ✅ Expected: Yes, explain ingredients and uses
   - ✅ Should mention: Breaks on action, reagent requirements

#### Test: Healer NPC - Healing Potion Knowledge

**NPC Type:** Healer
**Expected Knowledge Level:** Expert on healing potions, Proficient on cure

**Test Questions:**
1. "Which healing potion should I use for this wound?"
   - ✅ Expected: Assess severity, recommend appropriate strength

2. "What's the difference between cure and heal potions?"
   - ✅ Expected: Clear explanation of different purposes

### Tameable Creature Knowledge (15 Category Entries)

#### Test: Animal Trainer - Creature Expertise

**NPC Type:** Commoner (should be AnimalTrainer - note for future)
**Expected Knowledge Level:** Expert

**Test Questions:**
1. "What's the best creature for a new tamer?"
   - ✅ Expected: Recommend starting creatures (farm animals, horses, maybe wolves)
   - ✅ Should explain: Skill requirements, ease of control

2. "Tell me about dragons"
   - ✅ Expected: Detailed explanation of dragon types, power, taming difficulty
   - ✅ Should mention: Drake → Dragon → Greater Dragon → Wyrm progression

3. "Can I tame a nightmare?"
   - ✅ Expected: Yes, explain requirements, difficulty, benefits
   - ✅ Should mention: High skill needed, fire resistance, combat power

4. "What creatures are good for riding?"
   - ✅ Expected: List horses, ostards, ridgebacks, swamp dragons
   - ✅ Should compare: Speed, stamina, combat capability

5. "I want a strong combat pet, what do you suggest?"
   - ✅ Expected: Based on skill level, recommend appropriate tier
   - ✅ Low skill: Bears, great cats
   - ✅ Medium skill: White wyrm, nightmare
   - ✅ High skill: Dragons, cu sidhe, greater dragons

6. "What's the difference between a cu sidhe and a regular dog?"
   - ✅ Expected: Explain cu sidhe as legendary/magical, much more powerful
   - ✅ Should mention: Special abilities, healing, difficulty to obtain

7. "Are spiders good pets?"
   - ✅ Expected: Discuss giant spider types, poison abilities
   - ✅ Should mention: Useful for poison damage, moderate taming skill

#### Test: Scholar NPC - Creature Lore

**NPC Type:** Sage
**Expected Knowledge Level:** Proficient (knows lore, not practical taming)

**Test Questions:**
1. "What can you tell me about unicorns?"
   - ✅ Expected: Lore, magical properties, purity requirements
   - ✅ Should mention: Only tameable by pure of heart

2. "Are dragons intelligent?"
   - ✅ Expected: Lore about dragon intelligence, ancient nature

3. "Tell me the legend of the cu sidhe"
   - ✅ Expected: Mythological background, special nature

## Cross-Domain Knowledge Testing

### Test: Knowledge Boundaries and Referrals

#### Test: Healer Asked About Reagents

**NPC Type:** Healer (Expert in healing, Proficient in reagents)

**Test Questions:**
1. "What reagents heal wounds?"
   - ✅ Expected: Mention ginseng, garlic
   - ✅ Should say: "A mage or alchemist could tell you more about reagent preparation"

2. "How do I make a heal spell?"
   - ✅ Expected: Should refer to Mage for spell instruction
   - ✅ Might mention: "I know ginseng is used, but you'd need a mage's training"

#### Test: Guard Asked About Magic

**NPC Type:** Guard (Aware of basic magic)

**Test Questions:**
1. "What magic reagents are dangerous?"
   - ✅ Expected: Basic awareness (nightshade, blood moss)
   - ⚠️ Should refer: "The court mage knows more about such things"

2. "How do I defend against poison spells?"
   - ✅ Expected: Practical defense (cure potions, avoid clouds)
   - ⚠️ Should refer: Alchemist for potions, Mage for counter-spells

#### Test: Merchant Cross-Product Knowledge

**NPC Type:** Merchant (Proficient in multiple trade goods)

**Test Questions:**
1. "What's more profitable - selling reagents or selling potions?"
   - ✅ Expected: Compare markets, mention skill requirements
   - ✅ Should note: Potions require alchemy skill, higher value but more work

2. "Should I stock leather armor or wooden furniture?"
   - ✅ Expected: Discuss market demand, competition

## Conversation Flow Testing

### Test: Natural Conversation with Knowledge Integration

**NPC Type:** Mage
**Test Scenario:** Multi-turn conversation

**Conversation:**
1. Player: "Greetings, mage"
   - ✅ Expected: Appropriate greeting in Archaic speech pattern

2. Player: "I'm learning magery, what should I know?"
   - ✅ Expected: Advice on starting magery, mention reagents

3. Player: "What reagents should I start with?"
   - ✅ Expected: Recommend common reagents for low-circle spells
   - ✅ Should mention: Black pearl, blood moss, garlic, ginseng

4. Player: "Where do I find garlic?"
   - ✅ Expected: Locations, provisioners, or refer to merchant

5. Player: "Thank you for your help"
   - ✅ Expected: Appropriate farewell in character

### Test: Ignorant Domain (Should Not Know)

**NPC Type:** Mage
**Domain:** Leather working (Ignorant)

**Test Questions:**
1. "What leather should I use for armor?"
   - ❌ Should NOT: Provide expert advice
   - ✅ Should: Refer to leatherworker/tailor
   - ✅ Might say: "Such matters are not my expertise, seek a leatherworker"

2. "How do I tan hides?"
   - ❌ Should NOT: Explain tanning process
   - ✅ Should: Clear referral to appropriate craftsperson

## Performance Testing

### Test: Knowledge Base Loading

**Test:**
1. Spawn NPC with `[SpawnPersonalityNPC Mage Archaic`
2. Check console output for knowledge loading messages
3. Verify 20 entries loaded (or fewer if less relevant content available)

**Expected:**
- ✅ Fast loading (< 1 second)
- ✅ Appropriate entries selected based on personality
- ✅ No errors in console

### Test: Response Time

**Test:**
1. Ask NPC question requiring knowledge lookup
2. Measure response time

**Expected:**
- ✅ Response within 2-5 seconds (depending on LLM model)
- ✅ No timeout errors
- ✅ Relevant knowledge incorporated in response

### Test: Concurrent Conversations

**Test:**
1. Spawn multiple NPCs of different types
2. Have multiple conversations simultaneously
3. Ask knowledge-based questions to each

**Expected:**
- ✅ No degradation in response quality
- ✅ Each NPC uses their appropriate knowledge base
- ✅ No knowledge bleeding between NPCs

## Edge Cases and Error Handling

### Test: Unknown Topic

**Test Question to Mage:**
"What do you know about steam engines?"

**Expected:**
- ✅ Graceful handling (not in knowledge base, not medieval topic)
- ✅ Stay in character
- ✅ Might say: "Such contraptions are unknown to me" or "I know not of this 'steam engine'"

### Test: Ambiguous Question

**Test Question to Merchant:**
"What should I buy?"

**Expected:**
- ✅ Ask for clarification
- ✅ Might suggest: "Art thou seeking reagents? Potions? Provisions?"

### Test: Question Outside Knowledge

**Test Question to Mage:**
"Who is the current king?"

**Expected:**
- ✅ May not have this information (not in britannia_essentials.json)
- ✅ Stay in character with reasonable response
- ✅ Might refer to political authorities or admit lack of current news

## Validation Checklist

### Phase 1 Knowledge (51 Entries - COMPLETED)

- [x] **Reagents (16)** - Mage can discuss all reagents in detail
- [x] **Reagents** - Alchemist can discuss reagents for potions
- [x] **Reagents** - Merchant knows trade value
- [x] **Reagents** - Commoner has basic awareness, refers to experts
- [x] **Wood (7)** - Carpenter/Bowyer can discuss wood types expertly
- [x] **Leather (4)** - Leatherworker can discuss leather types expertly
- [x] **Potions (9)** - Alchemist can explain all potion types
- [x] **Potions** - Healer knows healing/cure potions well
- [x] **Potions** - Merchant knows what to stock
- [x] **Creatures (15)** - Animal trainer can discuss all tameable creatures
- [x] **Creatures** - Scholar knows creature lore
- [x] **Creatures** - Mage has basic awareness

### Phase 7: Complete Domain Knowledge (25+ NPC Types - NEW)

#### High Priority NPCs (10 Types)
- [ ] **InnKeeper/Barkeeper** - Hospitality services, rooms, food, gossip
- [ ] **Banker** - Gold storage, security, investments
- [ ] **AnimalTrainer** - Taming, training, creature knowledge
- [ ] **Ranger** - Tracking, wilderness survival, creatures
- [ ] **Cook** - Cooking skill, recipes, food preparation
- [ ] **Farmer** - Crops, agriculture, weather patterns
- [ ] **Fisherman** - Fishing skill, fish types, locations
- [ ] **Miner** - Mining skill, ore types, locations
- [ ] **Tinker** - Tinkering skill, gadgets, repairs
- [ ] **Provisioner** - Travel gear, adventuring supplies

#### Medium Priority NPCs (5 Types)
- [ ] **Scribe** - Writing, documentation, knowledge preservation
- [ ] **Bard** - Music, performance, stories, ballads
- [ ] **Mapmaker** - Cartography, maps, locations, geography
- [ ] **RealEstateBroker** - Properties, housing, real estate
- [ ] **Scholar** - Lore, history, research, academic knowledge

#### Lower Priority NPCs (10+ Types)
- [ ] **Actor** - Theatrical performance, dramatic arts
- [ ] **Artist** - Visual art, artistic expression
- [ ] **Gypsy** - Fortune telling, mystical arts
- [ ] **HairStylist** - Hair styling, appearance customization
- [ ] **Shipwright** - Ship building, maritime knowledge
- [ ] **TownCrier** - News, announcements, public communication
- [ ] **Commoner** - Daily life, local customs, practical knowledge
- [ ] **Peasant** - Rural life, agriculture, simple living
- [ ] **Noble** - Social hierarchy, court etiquette, political knowledge

### Personality and Speech Patterns

- [ ] Mage uses Archaic speech pattern correctly
- [ ] Merchant uses Formal speech pattern correctly
- [ ] Guard uses Formal speech pattern correctly
- [ ] Commoner uses Casual speech pattern correctly
- [ ] Sage uses OldEnglish speech pattern correctly
- [ ] Responses match personality traits (helpful, gruff, wise, etc.)

### Knowledge Boundaries

- [ ] Expert knowledge shows deep understanding
- [ ] Proficient knowledge shows working familiarity
- [ ] Aware knowledge shows basic familiarity
- [ ] Ignorant domain triggers appropriate referrals
- [ ] Referrals mention correct NPC type (primaryReferralTarget)

### System Integration

- [ ] Knowledge loads at spawn time (not per query)
- [ ] All 20 domain files load correctly
- [ ] Domain knowledge summaries appear in personality prompts
- [ ] Prioritization by profession works correctly
- [ ] No duplicate entries in knowledge base
- [ ] Console logging shows knowledge loading from all domain files
- [ ] SimpleLoreSystem loads all domain files without errors

## Testing Tips

1. **Use Console Logging:** Watch server console for knowledge loading and RAG debugging
2. **Test Systematically:** Cover each domain with appropriate NPC types
3. **Test Boundaries:** Verify NPCs don't claim expertise they shouldn't have
4. **Test Referrals:** Confirm NPCs refer to correct profession types
5. **Multi-Turn Conversations:** Test knowledge persistence across conversation
6. **Performance:** Monitor response times and server load
7. **Speech Patterns:** Verify personality + speech pattern combinations
8. **Edge Cases:** Test unusual or out-of-scope questions

## Reporting Issues

When reporting knowledge system issues, include:

1. **NPC Type:** Personality and speech pattern
2. **Question Asked:** Exact player input
3. **Expected Response:** What should have happened
4. **Actual Response:** What actually happened
5. **Knowledge Base:** Which lore entries should have been used
6. **Console Output:** Any errors or debug messages

## Phase 7: Complete Domain Knowledge Testing (NEW - 25+ NPC Types)

### High Priority NPCs (10 Types)

#### Test: InnKeeper/Barkeeper - Hospitality Services

**NPC Type:** InnKeeper, Barkeeper
**Expected Knowledge Level:** Expert

**Test Questions:**
1. "Do you have any rooms available?"
   - ✅ Expected: Room availability, pricing, quality options
   - ✅ Should mention: Room types, rates, amenities

2. "What food do you serve?"
   - ✅ Expected: Menu items, local specialties, meal options
   - ✅ Should mention: Food quality, prices, preparation

3. "Any interesting news or gossip?"
   - ✅ Expected: Local gossip, traveler tales, recent events
   - ✅ Should mention: Community happenings, rumors, stories

4. "Where can travelers rest safely?"
   - ✅ Expected: Room security, inn safety, traveler services

#### Test: Banker - Financial Services

**NPC Type:** Banker
**Expected Knowledge Level:** Expert

**Test Questions:**
1. "How do I store my gold safely?"
   - ✅ Expected: Banking procedures, account management, security
   - ✅ Should mention: Deposits, withdrawals, account access

2. "What investment opportunities are available?"
   - ✅ Expected: Financial advice, economic trends, wealth management
   - ✅ Should mention: Investment strategies, market conditions

3. "Is my gold safe in the bank?"
   - ✅ Expected: Security measures, vault protection, account safety

#### Test: AnimalTrainer - Creature Taming

**NPC Type:** AnimalTrainer
**Expected Knowledge Level:** Expert

**Test Questions:**
1. "What creatures can I tame?"
   - ✅ Expected: List of tameable creatures, skill requirements
   - ✅ Should mention: Basic to legendary creatures, taming difficulty

2. "How do I bond with my pet?"
   - ✅ Expected: Pet bonding mechanics, care requirements
   - ✅ Should mention: Feeding, loyalty, bonding process

3. "What's the best combat pet?"
   - ✅ Expected: Creature recommendations based on skill level
   - ✅ Should mention: Dragons, nightmares, combat abilities

4. "How do I care for my pet?"
   - ✅ Expected: Feeding, healing, maintenance requirements
   - ✅ Should mention: Creature health, loyalty, care tips

#### Test: Ranger - Wilderness Survival

**NPC Type:** Ranger
**Expected Knowledge Level:** Expert

**Test Questions:**
1. "Can you track that creature?"
   - ✅ Expected: Tracking skill explanation, wilderness navigation
   - ✅ Should mention: Tracking techniques, creature signs

2. "How do I survive in the wilderness?"
   - ✅ Expected: Survival techniques, camping, foraging
   - ✅ Should mention: Food, water, shelter, navigation

3. "What creatures live in these woods?"
   - ✅ Expected: Wilderness creature knowledge, behavior patterns
   - ✅ Should mention: Predators, prey, habitats

#### Test: Cook - Food Preparation

**NPC Type:** Cook
**Expected Knowledge Level:** Expert

**Test Questions:**
1. "How do I cook better meals?"
   - ✅ Expected: Cooking skill requirements, recipes, techniques
   - ✅ Should mention: Skill levels (0-100), recipe quality

2. "What ingredients do I need for bread?"
   - ✅ Expected: Recipe ingredients, preparation methods
   - ✅ Should mention: Flour, cooking process, quality factors

3. "Can you teach me to cook?"
   - ✅ Expected: Cooking instruction, skill progression advice

#### Test: Farmer - Agriculture

**NPC Type:** Farmer
**Expected Knowledge Level:** Expert

**Test Questions:**
1. "What crops grow best here?"
   - ✅ Expected: Crop types, growing conditions, seasonal cycles
   - ✅ Should mention: Wheat, corn, vegetables, weather effects

2. "When should I plant?"
   - ✅ Expected: Seasonal timing, weather patterns, planting seasons
   - ✅ Should mention: Spring planting, harvest times

3. "How do I maximize my crop yield?"
   - ✅ Expected: Farming techniques, soil quality, pest control

#### Test: Fisherman - Fishing

**NPC Type:** Fisherman
**Expected Knowledge Level:** Expert

**Test Questions:**
1. "Where are the best fishing spots?"
   - ✅ Expected: Fishing locations, fish types by location
   - ✅ Should mention: Rivers, lakes, oceans, different catches

2. "What fish can I catch here?"
   - ✅ Expected: Fish types, rarity, values
   - ✅ Should mention: Bass, salmon, trout, exotic species

3. "How do I improve my fishing?"
   - ✅ Expected: Fishing skill requirements, techniques, bait selection

#### Test: Miner - Mining

**NPC Type:** Miner
**Expected Knowledge Level:** Expert

**Test Questions:**
1. "Where can I find valorite ore?"
   - ✅ Expected: Mining locations, ore rarity, extraction techniques
   - ✅ Should mention: Deep dungeons, dangerous locations

2. "What's the best way to mine?"
   - ✅ Expected: Mining techniques, skill requirements, safety
   - ✅ Should mention: Mining skill (0-100), vein identification

3. "How do I smelt ore into ingots?"
   - ✅ Expected: Smelting process, forge requirements, techniques

#### Test: Tinker - Mechanical Items

**NPC Type:** Tinker
**Expected Knowledge Level:** Expert

**Test Questions:**
1. "Can you repair my weapon?"
   - ✅ Expected: Repair services, item maintenance, durability
   - ✅ Should mention: Repair skill, costs, item condition

2. "What gadgets can you make?"
   - ✅ Expected: Mechanical items, tools, devices
   - ✅ Should mention: Locks, keys, traps, clockwork devices

3. "How do I learn tinkering?"
   - ✅ Expected: Tinkering skill requirements, learning progression

#### Test: Provisioner - Travel Supplies

**NPC Type:** Provisioner
**Expected Knowledge Level:** Expert

**Test Questions:**
1. "What do I need for a long journey?"
   - ✅ Expected: Travel gear recommendations, essential supplies
   - ✅ Should mention: Backpacks, bedrolls, camping equipment

2. "Do you have adventuring supplies?"
   - ✅ Expected: Equipment inventory, travel essentials
   - ✅ Should mention: Containers, tools, travel gear

### Medium Priority NPCs (5 Types)

#### Test: Scribe - Writing and Documentation

**NPC Type:** Scribe
**Expected Knowledge Level:** Expert

**Test Questions:**
1. "Can you write a book for me?"
   - ✅ Expected: Writing services, documentation, book creation
   - ✅ Should mention: Writing techniques, knowledge preservation

2. "How do I preserve important information?"
   - ✅ Expected: Documentation methods, record keeping

#### Test: Bard - Music and Performance

**NPC Type:** Bard
**Expected Knowledge Level:** Expert

**Test Questions:**
1. "Can you play a song?"
   - ✅ Expected: Musical performance, instruments, songs
   - ✅ Should mention: Lutes, harps, ballads, entertainment

2. "Tell me a story"
   - ✅ Expected: Storytelling, ballads, epic tales
   - ✅ Should mention: Historical ballads, narrative arts

#### Test: Mapmaker - Cartography

**NPC Type:** Mapmaker
**Expected Knowledge Level:** Expert

**Test Questions:**
1. "Can you make me a map?"
   - ✅ Expected: Map creation, cartography services
   - ✅ Should mention: Location knowledge, geographic features

2. "Where is the nearest dungeon?"
   - ✅ Expected: Location knowledge, geography, navigation
   - ✅ Should mention: Dungeon locations, travel routes

#### Test: RealEstateBroker - Properties

**NPC Type:** RealEstateBroker
**Expected Knowledge Level:** Expert

**Test Questions:**
1. "What properties are available?"
   - ✅ Expected: Property listings, locations, values
   - ✅ Should mention: Houses, shops, property characteristics

2. "How do I buy a house?"
   - ✅ Expected: Property transactions, ownership, requirements

#### Test: Scholar - Lore and History

**NPC Type:** Scholar
**Expected Knowledge Level:** Expert

**Test Questions:**
1. "Tell me about Lord British"
   - ✅ Expected: Historical knowledge, lore, research
   - ✅ Should mention: Historical figures, events, cultural knowledge

2. "What can you teach me about Britannia's history?"
   - ✅ Expected: Comprehensive historical knowledge, research methods

### Lower Priority NPCs (10+ Types)

#### Test: Actor - Theatrical Performance

**NPC Type:** Actor
**Test Questions:**
1. "Can you perform a play?"
   - ✅ Expected: Theatrical performance, dramatic arts, acting

#### Test: Artist - Visual Arts

**NPC Type:** Artist
**Test Questions:**
1. "Can you create artwork?"
   - ✅ Expected: Artistic expression, visual arts, creation techniques

#### Test: Gypsy - Fortune Telling

**NPC Type:** Gypsy
**Test Questions:**
1. "Can you tell my fortune?"
   - ✅ Expected: Fortune telling, mystical arts, divination

#### Test: HairStylist - Appearance

**NPC Type:** HairStylist
**Test Questions:**
1. "Can you style my hair?"
   - ✅ Expected: Hair styling, appearance customization, fashion

#### Test: Shipwright - Ship Building

**NPC Type:** Shipwright
**Test Questions:**
1. "Can you build me a ship?"
   - ✅ Expected: Ship construction, maritime knowledge, sailing

#### Test: TownCrier - News

**NPC Type:** TownCrier
**Test Questions:**
1. "What's the news?"
   - ✅ Expected: Current events, announcements, community information

#### Test: Commoner - Daily Life

**NPC Type:** Commoner
**Test Questions:**
1. "What's life like here?"
   - ✅ Expected: Daily life, local customs, practical knowledge

#### Test: Peasant - Rural Life

**NPC Type:** Peasant
**Test Questions:**
1. "How do you farm?"
   - ✅ Expected: Agriculture, rural life, simple living

#### Test: Noble - Social Hierarchy

**NPC Type:** Noble
**Test Questions:**
1. "Tell me about the court"
   - ✅ Expected: Social hierarchy, court etiquette, political knowledge

---

## Next Steps After Complete Testing

Once all domain knowledge is validated:

1. **Performance Optimization:** Monitor response times with expanded knowledge base
2. **Personality Refinement:** Polish dialogue quality for all NPC types
3. **Knowledge Boundaries:** Test referral system across all professions
4. **Additional NPC Classes:** Extend to non-BaseVendor NPCs (Guards, Quest Givers)
5. **Advanced Features:** Dynamic quests, faction awareness, time-based dialogue
6. **Knowledge Updates:** System for updating lore entries without restart

## Success Criteria

### Phase 1 Knowledge System (COMPLETED)
- ✅ 90%+ of domain-appropriate questions get accurate, in-character responses
- ✅ NPCs correctly refer players to appropriate experts for out-of-domain questions
- ✅ Response times are acceptable (< 5 seconds average)
- ✅ No system errors or crashes during normal operation
- ✅ Knowledge enhances immersion and player experience
- ✅ Players can learn about Britannia's crafting systems through NPC conversations

### Phase 7: Complete Domain Knowledge System (NEW)
- [ ] All 25+ NPC types provide domain-specific knowledge in conversations
- [ ] Domain knowledge summaries appear correctly in personality prompts
- [ ] All 20 domain files load without errors
- [ ] NPCs demonstrate expertise in their profession domains
- [ ] Cross-domain referrals work correctly (e.g., Blacksmith refers gem questions to Jeweler)
- [ ] Response times remain acceptable (< 2.5 seconds target) with expanded knowledge base
- [ ] No performance degradation with 20 domain files vs 8 original files
- [ ] Players can learn about all major game systems through appropriate NPC conversations
