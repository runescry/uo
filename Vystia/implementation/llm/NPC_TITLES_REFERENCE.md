# NPC Titles Reference

Complete list of all NPC titles used in ServUO vendor and NPC systems.

## Crafting & Trade Professions

| Title | Class Name | Personality Type |
|-------|------------|------------------|
| the alchemist | Alchemist, Glassblower | Alchemist |
| the animal trainer | AnimalTrainer | AnimalTrainer |
| the armourer / the armorer | Armorer | Armorer |
| the architect | Architect | Architect |
| the baker | Baker | Cook |
| the barkeeper | Barkeeper, PlayerBarkeeper | Barkeeper |
| the bard | Bard | Bard |
| the banker | Banker | Banker |
| the blacksmith | Blacksmith | Blacksmith |
| the bowyer | Bowyer | Bowyer |
| the butcher | Butcher | Cook |
| the carpenter | Carpenter | Carpenter |
| the cobbler | Cobbler | Cobbler |
| the cook | Cook | Cook |
| the farmer | Farmer, GrandpaCharley | Farmer |
| the fisher / the fisherman | Fisherman | Fisherman |
| the furtrader | Furtrader | Furtrader |
| the gardener | Gardener | Gardener |
| the hairstylist / the hair stylist | HairStylist, CustomHairstylist | HairStylist |
| the herbalist | Herbalist | Herbalist |
| the innkeeper | InnKeeper | InnKeeper |
| the iron worker | IronWorker | Blacksmith |
| the jeweler | Jeweler | Jeweler |
| the leather worker | LeatherWorker | LeatherWorker |
| the mapmaker | Mapmaker | Mapmaker |
| the mage | Mage, EscortableMage, NewHavenMage | Mage |
| the miller | Miller | Miller |
| the miner | Miner | Miner |
| the mystic | Mystic | Mystic |
| the provisioner | Provisioner | Provisioner |
| the rancher | Rancher | Farmer |
| the ranger | Ranger | Ranger |
| the real estate broker | RealEstateBroker | RealEstateBroker |
| the scribe | Scribe | Scribe |
| the shipwright | Shipwright | Shipwright |
| the stone crafter | StoneCrafter | StoneCrafter |
| the tailor | Tailor | Tailor |
| the tanner | Tanner | LeatherWorker |
| the tavern keeper | TavernKeeper | InnKeeper |
| the thief | Thief | Thief |
| the tinker | Tinker | Tinker |
| the variety dealer | VarietyDealer | Merchant |
| the vet / the veterinarian | Veterinarian | Veterinarian |
| the waiter | Waiter | Waiter |
| the weaponsmith | Weaponsmith | Weaponsmith |
| the weaver | Weaver | Weaver |

## Artisan & Specialized Crafts

| Title | Class Name | Personality Type |
|-------|------------|------------------|
| the beekeeper | Beekeeper | Beekeeper |
| the golem crafter | GolemCrafter | GolemCrafter |
| the bark weaver | Yellienir | Weaver |
| the famed toymaker | TomasONeerlan | Tinker |
| the impresario | Impresario | Impresario |
| the well-known collector | ElwoodMcCarrin | Merchant |
| the respected painter | AlbertaGiacco | Artist |

## Magical & Mystical Professions

| Title | Class Name | Personality Type |
|-------|------------|------------------|
| the arcanist | Bolaevin | Mage |
| the thaumaturgist | Olaeni | Mage |
| the Sorceress | Victoria | Mage |
| the High Mage | Schmendrick | Mage |
| the Holy Mage | HolyMage | Mage |
| the Ancient Necromancer | Mardoth | Necromancer |
| the Conjurer | Uzeraan | Mage |
| the Exalted Artificer | Vrulkax | Mage |

## Nature & Wilderness

| Title | Class Name | Personality Type |
|-------|------------|------------------|
| the Naturalist | Naturalist | Naturalist |
| the aborist | AlelleTheAborist, Daelas | Herbalist |
| the Dryad | Dryad | Dryad |

## Combat & Authority

| Title | Class Name | Personality Type |
|-------|------------|------------------|
| the guard | ArcherGuard, BaseShieldGuard | Guard |
| the Mansion Guard | MansionGuard | Guard |
| the Guardsman of Daimyo Haochi | HaochisGuardsman | Guard |
| the Militia Cannoneer | MilitiaCanoneer | Guard |

## Entertainment & Arts

| Title | Class Name | Personality Type |
|-------|------------|------------------|
| the actor / the actress | Actor | Actor |
| the artist | Artist | Artist |
| the renowned minstrel | GabrielPiete | Bard |

## Social Roles

| Title | Class Name | Personality Type |
|-------|------------|------------------|
| the bride / the groom | BrideGroom, NewHavenBrideGroom | Commoner |
| the gypsy / the gypsy maiden | GypsyMaiden, Relnia | Gypsy |
| the merchant | NewHavenMerchant, Escortables | Merchant |
| the messenger | NewHavenMessenger | Messenger |
| the noble | NewHavenNoble | Noble |
| the peasant | NewHavenPeasant | Commoner |
| the seeker of adventure | NewHavenSeekerOfAdventure | Adventurer |
| the vagabond | Vagabond | Vagabond |

## Scholarly & Wise

| Title | Class Name | Personality Type |
|-------|------------|------------------|
| the wise | Abbein, Alethanian, Jothan, Mallew, Taellia, Vicaie | Scholar |
| the student | Nythalia | Scholar |
| the expeditionist | Athialon, Tyleelor | Adventurer |
| the keeper of tradition | Aneen | Scholar |

## Unique Named Titles

| Title | Class Name | Description |
|-------|------------|-------------|
| the Keeper of Chivalry | KeeperOfChivalry | Teaches chivalry |
| the Guardian | Horus | Guardian NPC |
| the Ferryman | Chyloth | Boat transport |
| the Silent | JedahEntille | Silent/mysterious NPC |
| the Hag | Grizelda | Witch/hag character |
| the Notorious | Emino | Infamous character |
| the Drunken Pirate | Blackheart | Pirate NPC |
| the Masterful Tactician | Zoel | Strategic combat NPC |
| the Honorable Samurai Legend | Haochi | Legendary samurai |
| the Lucky Dealer | AttendantLuckyDealer | Casino dealer |
| the Herald | AttendantHerald | Herald/announcer |
| the Guide | AttendantGuide | Guide NPC |
| the Fortune Teller | AttendantFortuneTeller | Fortune teller |
| the healer | AluniolTheHealer, Rebinil | Healer |
| the Monk | Monk | Monk |

## Title-to-Personality Mapping for InferPersonality()

When implementing title-based personality detection, the following mappings should be used:

### Exact Title Matches
- "the alchemist" → Alchemist
- "the animal trainer" → AnimalTrainer
- "the armorer" / "the armourer" → Armorer
- "the bard" → Bard
- "the banker" → Banker
- "the blacksmith" / "the iron worker" → Blacksmith
- "the bowyer" → Bowyer
- "the carpenter" → Carpenter
- "the cook" / "the baker" / "the butcher" → Cook
- "the farmer" / "the rancher" → Farmer
- "the fisher" / "the fisherman" → Fisherman
- "the hairstylist" / "the hair stylist" → HairStylist
- "the herbalist" / "the aborist" → Herbalist
- "the innkeeper" / "the tavern keeper" → InnKeeper
- "the barkeeper" → Barkeeper
- "the jeweler" → Jeweler
- "the leather worker" / "the tanner" → LeatherWorker
- "the mapmaker" → Mapmaker
- "the miner" → Miner
- "the provisioner" → Provisioner
- "the real estate broker" → RealEstateBroker
- "the scribe" → Scribe
- "the shipwright" → Shipwright
- "the tailor" → Tailor
- "the tinker" → Tinker
- "the veterinarian" / "the vet" → Veterinarian
- "the weaponsmith" → Weaponsmith

### Keyword-Based Title Matches
If title contains:
- "mage" / "arcanist" / "thaumaturgist" / "sorceress" / "conjurer" → Mage
- "bard" / "minstrel" → Bard
- "guard" / "guardsman" → Guard
- "healer" → Healer
- "actor" / "actress" → Actor
- "artist" / "painter" → Artist
- "gypsy" → Gypsy
- "merchant" → Merchant
- "naturalist" → Naturalist
- "wise" / "scholar" / "student" → Scholar
- "ranger" → Ranger
- "monk" → Monk
- "necromancer" → Necromancer

## Statistics

- **Total Unique Titles**: 95+
- **Crafting Professions**: 35
- **Magical Professions**: 8
- **Combat Roles**: 4
- **Social Roles**: 9
- **Named/Unique**: 39+

## Usage Notes

1. **Title Priority**: When implementing personality detection:
   - First check class name (e.g., `Blacksmith` class)
   - Then check title (e.g., "the blacksmith")
   - Default to Merchant if no match

2. **Case Sensitivity**: Title checks should be case-insensitive

3. **Partial Matches**: Use `Contains()` for keyword matching in titles

4. **Multiple Mappings**: Some titles map to the same personality:
   - Baker, Cook, Butcher → Cook
   - Tanner, Leather Worker → LeatherWorker
   - Innkeeper, Tavern Keeper → InnKeeper
   - All mage variants → Mage

## Example: Denton the Bard

If an NPC is named "Denton" with title "the bard":
- Class name check: "LLMNpc" or "BaseVendor" (no match)
- Title check: Contains("bard") → **Bard personality** ✓

## See Also

- [BaseVendor LLM Integration](BASEVENDOR_LLM_INTEGRATION.md)
- [Personality System](../Core/NPCPersonalities.cs)
- [Knowledge Boundaries](KNOWLEDGE_BOUNDARIES_IMPLEMENTATION.md)
