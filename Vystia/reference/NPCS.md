# Vystia NPCs Reference

**Total NPCs:** 50+
**Location:** `ServUO/Scripts/Mobiles/Vystia/NPCs/`

---

## NPC Categories

| Category | Count | Location |
|----------|-------|----------|
| Faction Leaders | 20 | `NPCs/FactionLeaders/` |
| Talking Creatures | 12 | `NPCs/TalkingCreatures/` |
| Quest Givers | 3 | `NPCs/QuestGivers/` |
| Regional Vendors | 3 | `NPCs/Vendors/` |
| Class Trainers | 25 | `Trainers/` (separate folder) |
| Quest NPCs | Dynamic | `VystiaClasses/Quests/QuestNPC.cs` |

---

## Faction Leaders (20)

All faction leaders implement `ILLMConversational` for AI-powered dialogue.

### Frosthold / Polar Alliance

| NPC | Class | Title | Body |
|-----|-------|-------|------|
| ChieftainBjornFrostbeard | Chieftain | Polar Alliance Leader | Human |
| KingFrostbeard | King | Frosthold Ruler | Human |
| QueenIceshadow | Queen | Frosthold Co-Ruler | Human |

### Emberlands

| NPC | Class | Title | Body |
|-----|-------|-------|------|
| ArchmagePyrusAshborn | Archmage | Emberlands Leader | Human |
| WarlordEmberonFlamefist | Warlord | Emberlands General | Human |

### Desert / League of Sands

| NPC | Class | Title | Body |
|-----|-------|-------|------|
| SultanAziralRashid | Sultan | League of Sands Leader | Human |
| SheikhTarik | Sheikh | Desert Tribe Chief | Human |

### Verdantpeak / Sylvan Concord

| NPC | Class | Title | Body |
|-----|-------|-------|------|
| ElderSeraphinaLeafwhisper | Elder | Sylvan Concord Voice | Human |
| DruidLordFaelar | Druid Lord | Nature's Champion | Human |
| QueenAmaryllis | Queen | Forest Queen | Human |

### Ironclad / Ironclad Alliance

| NPC | Class | Title | Body |
|-----|-------|-------|------|
| EmperorGarrickSteelarm | Emperor | Ironclad Alliance Leader | Human |
| WarlordKael | Warlord | Ironclad General | Human |

### Crystal Barrens / Arcane Coalition

| NPC | Class | Title | Body |
|-----|-------|-------|------|
| ArchmageLumis | Archmage | Arcane Coalition Leader | Human |
| SageOrin | Sage | Crystal Keeper | Human |

### ShadowVoid

| NPC | Class | Title | Body |
|-----|-------|-------|------|
| SorceressNocturna | Sorceress | Void Master | Human |
| GuardianSylas | Guardian | Void Warden | Human |

### Skyreach / Highland Compact

| NPC | Class | Title | Body |
|-----|-------|-------|------|
| HighGuardianEldurMountainborn | High Guardian | Highland Leader | Human |

### Maritime / Maritime Sovereignty

| NPC | Class | Title | Body |
|-----|-------|-------|------|
| AdmiralMarisHawkseye | Admiral | Maritime Leader | Human |
| LordMarinerTideseeker | Lord Mariner | Fleet Commander | Human |

### Multi-Regional

| NPC | Class | Title | Body |
|-----|-------|-------|------|
| ChiefMaraWildsong | Chief | Nomadic Leader | Human |

---

## Talking Creatures (12)

Ancient, sentient creatures with LLM dialogue support. See also: `ANCIENT_BEINGS_AND_BOSSES_REFERENCE.md`

### Dragons (5)

| NPC | Type | Body ID | Location |
|-----|------|---------|----------|
| FrosthelmEternalWinter | White Dragon | 0xC (12) | Frozen Peak, Frosthold |
| EmberflameAshenTyrant | Red Dragon | 0x3B (59) | Volcano, Emberlands |
| VerdantheartForestGuardian | Green Dragon | 0x3C (60) | Hidden Grove, Verdantpeak |
| CrystalwingPrismaticOracle | Crystal Dragon | 0x3B (59) | Crystal Barrens |
| AbyssusDepthKing | Sea Serpent | 0x96 (150) | Forgotten Depths |

### Treants (2)

| NPC | Type | Body ID | Location |
|-----|------|---------|----------|
| ElderOakbark | Treant | 0x2F (47) | Grand Oak, Verdantpeak |
| IronbarkWarAncient | War Treant | 0x2F (47) | Verdantpeak Border |

### Sphinxes (2)

| NPC | Type | Body ID | Location |
|-----|------|---------|----------|
| SphynxOfEmberlands | Sphinx | 0x5F (95) | Volcanic Wastes |
| TheCrystalSphinx | Sphinx | 0x5F (95) | Crystal Barrens |

### Divine Avatars (3)

| NPC | Type | Body ID | Location |
|-----|------|---------|----------|
| FrostFatherAvatar | Air Elemental | 0xD (13) | Aurora, Frosthold |
| GreatMachinistConstruct | Iron Golem | 0x2F5 (757) | Great Forge, Ironclad |
| LunaraDryadHerald | Female Ethereal | 0x191 (401) | Grove of Spirits |

---

## Quest Givers (3)

| NPC | Type | Function |
|-----|------|----------|
| Chronicler | Human | Lore quests |
| QuartermasterGrimwald | Human | Supply/delivery quests |
| SageTheron | Human | Knowledge quests |

---

## Regional Vendors (3)

Located in `NPCs/Vendors/`:

| NPC | Region | Function |
|-----|--------|----------|
| IronhavenBanker | Ironclad | Banking services |
| IronhavenGuardCaptain | Ironclad | Guard dialogue |
| FrostholmHealer | Frosthold | Healing services |

---

## Dynamic Quest NPCs

**File:** `ServUO/Scripts/Custom/VystiaClasses/Quests/QuestNPC.cs`

Quest NPCs are spawned dynamically and linked to specific quests:
- Implement `ILLMConversational`
- Track player quest progress
- Auto-complete waypoints on interaction
- Configurable personality and speech patterns

### Spawn Command

```
[addquestNPC
[aqn
```

Opens 4-step wizard:
1. Select quest
2. Select waypoint
3. Configure NPC (name, title, personality)
4. Spawn at GM location

---

## LLM Integration

All major NPCs implement `ILLMConversational` interface:

```csharp
public interface ILLMConversational
{
    bool LLMConversationEnabled { get; }
    PersonalityType PersonalityType { get; }
    string SpeechPattern { get; }
    int HearingRange { get; }
    bool ShouldHandleConversation(Mobile from, string text);
    void HandleConversation(Mobile from, string text);
}
```

### Personality Types

- Wise, Gruff, Mysterious, Friendly, Hostile
- Noble, Scholarly, Military, Religious
- Custom per NPC

---

## Spawn Commands

| Command | Description |
|---------|-------------|
| `[SpawnPersonalityNPC` | Spawn LLM NPC with personality |
| `[addquestNPC` / `[aqn` | Quest NPC wizard |
| `[add <NPCClassName>` | Direct spawn |

### Examples

```
[add ChieftainBjornFrostbeard
[add ElderOakbark
[add Chronicler
[SpawnPersonalityNPC
```

---

## File Structure

```
ServUO/Scripts/Mobiles/Vystia/NPCs/
├── FactionLeaders/       # 20 faction leader NPCs
│   ├── ArchmagePyrusAshborn.cs
│   ├── ChieftainBjornFrostbeard.cs
│   └── ... (18 more)
├── TalkingCreatures/     # 12 ancient beings
│   ├── ElderOakbark.cs
│   ├── FrosthelmEternalWinter.cs
│   └── ... (10 more)
├── QuestGivers/          # 3 quest NPCs
│   ├── Chronicler.cs
│   ├── QuartermasterGrimwald.cs
│   └── SageTheron.cs
├── Vendors/              # 3 regional vendors
│   ├── IronhavenBanker.cs
│   ├── IronhavenGuardCaptain.cs
│   └── FrostholmHealer.cs
└── VystiaNPCCommands.cs  # Spawn commands
```

---

*Last Updated: 2026-01-01*
