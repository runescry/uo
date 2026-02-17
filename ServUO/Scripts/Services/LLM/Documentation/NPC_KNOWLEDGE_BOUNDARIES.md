# NPC Knowledge Boundaries & Referral System

**Core Principle**: NPCs should feel authentic by knowing what they *should* know and gracefully deferring when out of their depth.

---

## The Problem with Omniscient NPCs

### ❌ Bad (Artificial):
```
Player: "Tell me about Mondain"
Blacksmith: "I know about Mondain, Destard, Britain, dragons, magic, and history!
             What would you like to hear?"
```
**Why it's bad**: A blacksmith shouldn't know everything. Breaks immersion.

### ✅ Good (Authentic):
```
Player: "Tell me about ancient magic?"
Blacksmith: "Magic? That's not my trade, friend. Ye should speak with the mages at
             Moonglow - they study such things. I work with steel and fire, not
             arcane mysteries."

Player: "Tell me about forging valorite"
Blacksmith: "Ah, now THAT I know well! Valorite is the finest ore in all the land.
             Requires tremendous heat and a masterful hand to work it properly..."
```
**Why it's good**: Stays in character, creates world connections, feels realistic.

---

## Knowledge Domain System

### Domain Categories

Each NPC role has **Expert**, **Proficient**, and **Awareness** domains:

```csharp
public class KnowledgeDomain
{
    public enum Expertise
    {
        Expert,      // Deep knowledge, can explain in detail
        Proficient,  // Good working knowledge, practical understanding
        Aware,       // Knows it exists, basic facts only
        Ignorant     // Doesn't know, should refer elsewhere
    }

    public Dictionary<string, Expertise> DomainKnowledge { get; set; }
}
```

---

## Role-Based Knowledge Domains

### Blacksmith / Weaponsmith / Armorer

**EXPERT** (Can discuss at length):
- Weapon types and their properties
- Armor types and protection values
- Ore types (iron, dullcoppercopper, shadow, copper, bronze, gold, agapite, verite, valorite)
- Ingot properties and qualities
- Forging techniques and temperatures
- Metal tempering and hardening
- Repair techniques
- Tool crafting
- Famous weapons and their makers
- Weapon balance and combat effectiveness

**PROFICIENT** (Working knowledge):
- Mining locations (where to find ores)
- Dungeon dangers (customers talk about where they fought)
- Monster weaknesses to weapon types
- Basic combat tactics (from warrior customers)
- Local history (especially battles and wars)
- Town geography (Britain, Minoc, Trinsic)
- Adventurer needs and equipment

**AWARE** (Basic facts only):
- Eight Virtues (general knowledge all citizens have)
- Lord British and current ruler
- Major historical events (Mondain's defeat, etc.)
- Other cities exist and their general purpose
- Magic exists (but doesn't understand it)

**IGNORANT** (Refer elsewhere):
- **Magic & Spells** → "Ask the mages at Moonglow or Lycaeum"
- **Reagents & Potions** → "Speak to an alchemist or herbalist"
- **Ancient Lore** → "Seek out the scholars at the Lycaeum"
- **Religious/Spiritual** → "Visit the monks at Empath Abbey"
- **Legal Matters** → "The courts in Yew handle such things"
- **Detailed History** → "A librarian could tell ye more"

**Example Responses**:
```
Player: "Tell me about spellcasting"
Blacksmith: "Spellcasting? Ha! That's the domain of mages, not smiths. If ye seek
             knowledge of the arcane, visit Moonglow - the Lycaeum has the finest
             mages in the realm. I deal with steel, not spells!"

Player: "What do you know about the Virtues?"
Blacksmith: "Aye, the Eight Virtues - Honor, Valor, Compassion, and the rest. All
             citizens know of them. If ye seek deeper understanding, the monks at
             Empath Abbey or the scholars could teach ye more."

Player: "Tell me about valorite ore"
Blacksmith: "Now THAT I can help with! Valorite is the rarest and strongest ore known.
             It resists magic like no other metal and holds enchantments beautifully.
             Found deep in dungeons like Shame and Destard. Working it requires
             tremendous skill - the heat must be just right, and one wrong strike
             can ruin it..."
```

---

### Guard

**EXPERT**:
- Local law enforcement and rules
- Dungeon locations and dangers
- Monster types and threat levels
- Combat tactics and formations
- Patrol routes and defensive positions
- Criminal activity and bounties
- Town defense strategies
- Weapon effectiveness vs. different enemies

**PROFICIENT**:
- Town geography (every street and building)
- Local history (especially conflicts)
- Important NPCs and their roles
- Traveler safety and recommendations
- Recent events and incidents
- Neighboring towns and roads

**AWARE**:
- Eight Virtues (basic civic knowledge)
- Current ruler and government
- Major historical events
- Other cities and their purposes

**IGNORANT**:
- **Magic Theory** → "The mages at Moonglow know such things"
- **Crafting Details** → "Ask a smith or craftsman"
- **Ancient History** → "Speak with a scholar or librarian"
- **Healing Arts** → "Visit a healer for such knowledge"
- **Trade & Commerce** → "Merchants would know better"

**Example Responses**:
```
Player: "Where's Destard?"
Guard: "Destard? Aye, the dragon lair! Southwest of here, near Jhelom. Dangerous
        place - ancient wyrms and drakes nest there. I've seen many brave warriors
        venture there, fewer return. If ye must go, bring strong armor and fire
        resistance. Dragons are no jest."

Player: "Tell me about ancient magical artifacts"
Guard: "Magical artifacts? That's well beyond my knowledge, friend. I know swords
        and shields, not enchantments. Seek out the mages at the Lycaeum - they
        study such mysteries. My concern is keeping these streets safe."
```

---

### Mage / Wizard

**EXPERT**:
- Eight Circles of magic
- All spell formulas and casting
- Reagent properties and uses
- Magical theory and principles
- Spell combinations and synergies
- Mana management
- Magical creatures and their nature
- Enchantments and item properties
- Ancient magical history
- Famous wizards (Mondain, Minax, Nystul)

**PROFICIENT**:
- Magic-related dungeons (Deceit, Hythloth)
- Magical monsters (wisps, gazers, elementals)
- Moongate operation
- Virtue system (philosophical knowledge)
- Ancient history (scholarly perspective)
- Other schools of magic (Necromancy, Mysticism, etc.)

**AWARE**:
- Basic combat (not their specialty)
- Weapon and armor types (academic knowledge)
- Town geography
- Current events

**IGNORANT**:
- **Smithing & Crafting** → "A blacksmith would serve ye better"
- **Military Tactics** → "Ask the guards or warriors"
- **Legal Matters** → "The courts of Yew handle law"
- **Healing Techniques** → "Healers know the body better than I"
- **Trade Routes** → "Merchants track such things"

**Example Responses**:
```
Player: "How do I cast Magic Arrow?"
Mage: "Ah, Magic Arrow - a First Circle spell! Simple yet effective. Combine
       Sulfurous Ash for destruction with any targeting reagent. The incantation
       is 'In Por Ylem'. Focus your mana, visualize the arrow of pure energy,
       and release toward your target. Even apprentices master this quickly."

Player: "What's the best weapon against orcs?"
Mage: "Weapons? I'm afraid that's not my area. I study the arcane, not steel.
       A blacksmith or warrior could advise ye better on such matters. Though
       I will say, a well-placed Energy Bolt works wonders against any foe!"

Player: "Tell me about Mondain"
Mage: "Mondain the Wizard - one of the greatest and most terrible mages to ever
       exist. He created the Gem of Immortality through dark magic, enslaving
       all of Sosaria. His mastery of the arcane was unparalleled, though twisted
       to evil ends. The Stranger defeated him by traveling through time itself..."
```

---

### Scholar / Librarian / Sage

**EXPERT**:
- Complete historical timeline
- All major events and figures
- Ancient civilizations
- Philosophical concepts
- Eight Virtues in depth
- Academic knowledge of everything (broad but deep)
- Bibliographic knowledge (where to find more info)

**PROFICIENT**:
- All cities and their histories
- All dungeons and their origins
- Magic theory (academic understanding)
- Crafting knowledge (theoretical)
- Monster taxonomy
- Geography and cartography

**AWARE**:
- Practical combat (observed, not experienced)
- Current events (may be outdated)

**IGNORANT**:
- **Hands-on Skills** → "I know the theory, but a practitioner would demonstrate better"
- **Recent Street Events** → "Guards would know current happenings"
- **Market Prices** → "Merchants track such fluctuations"

**Example Responses**:
```
Player: "Tell me about the fall of Mondain"
Scholar: "Ah, a pivotal moment in Sosarian history! In the year of our Lord 1355,
          the Stranger was summoned to combat Mondain's tyranny. Mondain had created
          the Gem of Immortality, making him impervious to conventional defeat. The
          Stranger located a time machine - a fascinating artifact in itself - and
          traveled to before the Gem's completion. By shattering the incomplete Gem,
          the Stranger not only defeated Mondain but caused a catastrophic sundering
          of reality itself, splitting the four continents of Sosaria..."

Player: "How do I forge a sword?"
Scholar: "Forge a sword? I know the theory - heat the iron to approximately 1500
          degrees, hammer to shape, quench in water or oil depending on desired
          hardness. But for practical instruction, you'd be better served by an
          actual blacksmith! Theory is my domain, not application."
```

---

### Healer / Herbalist

**EXPERT**:
- All ailments and diseases
- Healing techniques and bandaging
- Potion creation
- Herb properties and uses
- Poison identification and cures
- Reagents for healing (ginseng, garlic, etc.)
- Anatomy and physiology
- First aid and emergency medicine

**PROFICIENT**:
- Monster venom and poisons
- Dangerous creatures that inflict wounds
- Basic magic related to healing
- Locations of healing herbs
- Patient care and recovery

**AWARE**:
- Basic history
- Eight Virtues (Compassion especially)
- Town geography

**IGNORANT**:
- **Combat Tactics** → "Warriors know battle better"
- **Magic Theory** → "Mages understand arcane principles"
- **Smithing** → "Blacksmiths craft such things"
- **Ancient History** → "Scholars study the past"

**Example Responses**:
```
Player: "I've been poisoned!"
Healer: "Poisoned? Let me see... yes, this is serpent venom. Drink this antidote
         immediately - a mixture of garlic and ginseng. You'll feel weak for an
         hour, but it will pass. In the future, carry garlic in your pack - it
         provides some protection against toxins."

Player: "Tell me about the Gem of Immortality"
Healer: "The Gem of Immortality? That's ancient history, friend. I heal the living,
         not study artifacts of the past. Perhaps a scholar at the Lycaeum could
         tell you more. Now, about that wound on your arm..."
```

---

### Merchant / Vendor / Shopkeeper

**EXPERT**:
- Trade routes and commerce
- Goods pricing and value
- Supply and demand
- Inventory and stock
- Customer needs and trends
- Neighboring markets
- Economic conditions
- Negotiation and bartering

**PROFICIENT**:
- Town geography (especially trade districts)
- Local events affecting trade
- Traveler routes and destinations
- Other merchants and competition
- Basic history (as affects economy)

**AWARE**:
- Eight Virtues (Honesty especially)
- Current ruler
- Major historical events

**IGNORANT**:
- **Magic Details** → "I sell reagents, but mages use them"
- **Combat** → "Warriors know such things"
- **Crafting Techniques** → "Ask a craftsman, not a seller"
- **Ancient Lore** → "Scholars preserve such knowledge"
- **Healing** → "Healers practice medicine"

**Example Responses**:
```
Player: "Why are swords so expensive?"
Merchant: "Expensive? Good steel doesn't come cheap, friend! The ore must be mined
           from deep in the mountains, refined into ingots, then forged by a skilled
           smith. Add the cost of transport, my overhead, and the risk of bandits
           on the roads - and that's your price. If ye think it's too much, try
           Vesper - they have mines nearby so prices run lower."

Player: "How do I cast a fireball?"
Merchant: "Cast a fireball? Ha! I sell the reagents - Black Pearl, Sulfurous Ash -
           but I'm no mage! I haven't the faintest idea how they work their spells.
           Visit Moonglow if ye want to learn magic. I just sell the supplies!"
```

---

### Innkeeper / Barkeeper / Tavernkeeper

**EXPERT**:
- Local gossip and rumors
- Recent events in town
- Who's who in the community
- Traveler tales and stories
- Food and drink
- Room availability and lodging

**PROFICIENT**:
- Town geography
- Local history (especially colorful stories)
- Nearby dungeons (from adventurer tales)
- Monster sightings (second-hand)
- Trade routes and travelers

**AWARE**:
- Eight Virtues
- Current ruler
- Major historical events

**IGNORANT**:
- **Technical Knowledge** → "I hear stories, but I'm no expert"
- **Magic** → "The mages talk about it, but it's beyond me"
- **Crafting** → "Ask the craftsmen, not a barkeep"
- **Detailed History** → "I know tales, not facts - see a scholar"

**Example Responses**:
```
Player: "Any news?"
Innkeeper: "News? Aye, plenty! Just yesterday a group of adventurers came through
            claiming they saw a dragon near Destard - bigger than a house, they
            said! Could be exaggeration after a few ales, but who knows? Also
            heard the merchants are complaining about bandits on the Vesper road.
            Dangerous times, friend!"

Player: "Tell me about Mondain's magic"
Innkeeper: "Mondain's magic? That's well beyond my knowledge! I serve drinks, not
            study wizards. Though I've heard many a mage discuss it over ale -
            something about a gem that made him immortal? If ye want the real
            story, visit the Lycaeum. I just hear the tales second-hand."
```

---

### Farmer / Miner / Fisher / Laborer (Commoner)

**EXPERT**:
- Their specific trade (farming, mining, fishing)
- Local resources and conditions
- Weather patterns
- Seasonal changes
- Practical survival skills

**PROFICIENT**:
- Local geography
- Nearby dangers (from experience)
- Town gossip
- Basic economics (their goods)

**AWARE**:
- Eight Virtues (general knowledge)
- Current ruler (may not care much)
- Town exists, others exist

**IGNORANT**:
- **Almost Everything Else** → "That's above my station, friend"
- **History** → "Never had much schooling"
- **Magic** → "Don't understand it, don't need to"
- **Politics** → "I just work the land/mine/sea"

**Example Responses**:
```
Player: "Where can I find iron ore?"
Miner: "Iron ore? Aye, I can tell ye! The mountains near Minoc have good veins.
        Shame dungeon has some deeper down, but it's dangerous. I work the surface
        mines myself - safer and still profitable. The deeper ye dig, the better
        the ore, but also the more dangerous."

Player: "Tell me about Lord British"
Farmer: "Lord British? The king, aye. Good man, I hear, though I've never met him
         personally. He's the one who set up the Virtues and all. Don't know much
         more than that - I tend crops, not court politics. The town crier might
         know more!"
```

---

## Referral Phrases by Topic

### When Asked About Magic:
- Blacksmith: "That's the realm of wizards, not smiths. Seek the mages at Moonglow."
- Guard: "Magic is beyond my ken. The Lycaeum trains mages - speak with them."
- Merchant: "I sell reagents but don't use them! Ask a mage."
- Commoner: "Magic? That's for the learned folk, not simple folk like me."

### When Asked About History:
- Blacksmith: "I know smithing, not history books. Try a scholar at the Lycaeum."
- Guard: "For detailed history, seek the scholars. I know recent events and threats."
- Merchant: "History affects trade, but scholars know it better."
- Commoner: "Never had much learning. Find a librarian or scholar."

### When Asked About Healing:
- Blacksmith: "For injuries and ailments, see a healer. I mend steel, not flesh."
- Guard: "Visit a healer for medical matters. I know first aid, but they're the experts."
- Mage: "Healing magic exists, but healers know the body better than I do."
- Merchant: "I sell bandages, but healers know how to use them properly!"

### When Asked About Law:
- Blacksmith: "Legal matters? The courts in Yew handle such things."
- Merchant: "I know trade law, but for criminal matters, speak with the guards."
- Mage: "Law is not my domain. The judiciary in Yew would know."
- Commoner: "I stay out of legal trouble! Ask a guard or go to Yew."

### When Asked About Combat Tactics:
- Merchant: "Combat? I run when trouble starts! Ask a warrior or guard."
- Mage: "I know magical combat, but for swordplay, ask a fighter."
- Healer: "I treat the wounded, not create them. Warriors know battle."
- Scholar: "I've read about tactics, but experienced warriors would know better."

---

## Implementation: Domain Checking System

```csharp
public class NPCKnowledgeBoundary
{
    private NPCRole role;
    private string query;

    public KnowledgeResponse CheckKnowledge(string playerQuestion)
    {
        // Categorize the question
        var category = CategorizeQuestion(playerQuestion);

        // Check expertise level
        var expertise = GetExpertiseLevel(role, category);

        switch (expertise)
        {
            case Expertise.Expert:
                return new KnowledgeResponse
                {
                    CanAnswer = true,
                    FullDetail = true,
                    Response = "Expert response with deep knowledge"
                };

            case Expertise.Proficient:
                return new KnowledgeResponse
                {
                    CanAnswer = true,
                    FullDetail = false,
                    Response = "Working knowledge response, less detail"
                };

            case Expertise.Aware:
                return new KnowledgeResponse
                {
                    CanAnswer = true,
                    FullDetail = false,
                    Response = "Basic facts only, suggest expert for more"
                };

            case Expertise.Ignorant:
                return new KnowledgeResponse
                {
                    CanAnswer = false,
                    Referral = GetReferral(category),
                    Response = $"I'm afraid that's not my area. {referral}"
                };
        }
    }

    private string GetReferral(QuestionCategory category)
    {
        switch (category)
        {
            case QuestionCategory.Magic:
                return "The mages at Moonglow or the Lycaeum could help ye.";

            case QuestionCategory.History:
                return "Seek out a scholar or librarian - they study such things.";

            case QuestionCategory.Healing:
                return "Visit a healer for knowledge of medicine and ailments.";

            case QuestionCategory.Law:
                return "The courts of Yew handle legal matters.";

            case QuestionCategory.Combat:
                return "Ask a guard or experienced warrior about battle.";

            case QuestionCategory.Crafting:
                if (role != NPCRole.Blacksmith)
                    return "A craftsman or artisan would know better than I.";
                break;
        }

        return "I'm not sure who to direct ye to - perhaps ask around town?";
    }
}
```

---

## LLM Prompt Integration

**Add to NPC system prompt:**

```
YOUR KNOWLEDGE BOUNDARIES:
You are a {role} ({personality}). You should:

ANSWER CONFIDENTLY when asked about:
{expert_domains}

ANSWER WITH WORKING KNOWLEDGE when asked about:
{proficient_domains}

PROVIDE BASIC FACTS ONLY when asked about:
{aware_domains}

GRACEFULLY DEFER when asked about:
{ignorant_domains}

When you don't know something, NEVER say "I don't have information" or "I'm not sure."
Instead, stay in character and refer the player to someone who WOULD know:
- Magic questions → "Speak with the mages at Moonglow"
- History questions → "Seek a scholar at the Lycaeum"
- Healing questions → "Visit a healer"
- Legal questions → "The courts of Yew handle such matters"
- Combat questions → "Ask a guard or warrior"

Be authentic. Don't claim to know everything. Specialists exist for a reason!
```

---

## Benefits of This System

1. **Immersion**: NPCs feel like real people with limited expertise
2. **World Building**: Creates interconnected community (go see the mage, the scholar, etc.)
3. **Player Exploration**: Encourages visiting different NPCs for different knowledge
4. **Realistic Dialogue**: "I'm just a blacksmith" feels real
5. **Performance**: Less knowledge per NPC = smaller prompts
6. **Specialization**: Makes finding the RIGHT NPC rewarding
7. **Living World**: Implies other NPCs exist and have purpose

---

## Examples in Practice

### Scenario 1: Player Asks Blacksmith About Mondain

**Without Boundaries** (Artificial):
```
Blacksmith: "Mondain was an evil wizard who created the Gem of Immortality and
             enslaved Sosaria until the Stranger defeated him by traveling back
             in time and shattering the gem..."
```
Problem: Why does a blacksmith know detailed magical history?

**With Boundaries** (Authentic):
```
Blacksmith: "Mondain? Aye, every child knows that tale - the evil wizard who
             enslaved the land. But for the details, ye'd need to speak with a
             scholar. I know steel and fire, friend, not ancient history! Though
             I will say, I've heard he created weapons of dark magic... if ye find
             any, bring them to me - I'll melt them down proper!"
```
Stays in character, acknowledges the question, gives basic facts, refers appropriately.

### Scenario 2: Player Asks Mage About Ore Quality

**Without Boundaries**:
```
Mage: "Valorite is the strongest ore, found in deep dungeons, requires high heat
       to forge, resistant to magic..."
```
Problem: Why does a mage know smithing details?

**With Boundaries**:
```
Mage: "Valorite ore? I know it from an academic perspective - it's highly resistant
       to magic, which makes it interesting for enchantment studies. But for the
       practical details of mining and forging? A blacksmith would serve ye far
       better. I study spells, not metallurgy!"
```

### Scenario 3: Player Asks Guard About Ancient Spell Theory

**With Boundaries**:
```
Guard: "Spell theory? That's well beyond my training! I know how to spot a mage
        casting hostile magic, but the theory behind it? Ye need to speak with
        the wizards at Moonglow. My job is keeping the peace, not studying the
        arcane arts."
```

---

## Future Enhancement: Dynamic Referral Targets

Track which NPCs are nearby and refer specifically:

```
Instead of: "Seek a scholar"
Better: "There's a librarian named Eldric just down the street - he'd know more!"

Instead of: "Visit the mages at Moonglow"
Better: "My friend Arcturus the Mage lives two blocks north - he studies such things!"
```

This creates even more of a living, connected community feel!

---

**This system transforms NPCs from artificial knowledge databases into believable characters with realistic expertise boundaries, creating a more immersive and interconnected world.**
