# Vystia Factions Guide

## Overview
Faction overview, reputation, tokens, vendors, and rewards aligned to Vystia's alliances and conflicts.

---

## Factions

### Ironclad Alliance
- Theme: Industrial, smithing, engineering
- Tokens: `IroncladToken`
- Vendors: `IroncladBlacksmith`, `Steamwright`
- Reputation Tiers: Neutral, Allied, Honored, Exalted
- Rewards: Magma Steel recipes, industrial deco, siege tools

### Sylvan Concord
- Theme: Nature, druidry, forest guardians
- Tokens: `SylvanToken`
- Vendors: `SylvanHerbalist`, `DruidicWarden`
- Reputation Tiers: Neutral, Allied, Honored, Exalted
- Rewards: Living Wood Armors, druid staves, forest deco

### League of Sands
- Theme: Desert survival, solar rites, caravans
- Tokens: `SandsToken`
- Vendors: `NomadOutfitter`, `SunPriest`
- Reputation Tiers: Neutral, Allied, Honored, Exalted
- Rewards: Desert robes, sandstorm staffs, caravan contracts

### Maritime Sovereignty
- Theme: Naval power, pearl trade, sea patrols
- Tokens: `MaritimeToken`
- Vendors: `Shipwright`, `PearlTrader`
- Reputation Tiers: Neutral, Allied, Honored, Exalted
- Rewards: Diving gear, sea charts, ship deco

### Highland Compact
- Theme: Mountain tribes, rangers, guardians
- Tokens: `HighlandToken`
- Vendors: `RangerQuartermaster`, `StoneCarver`
- Reputation Tiers: Neutral, Allied, Honored, Exalted
- Rewards: Ranger bows, mountain deco, patrol contracts

### Arcane Coalition
- Theme: Magical regulation, research, scrying
- Tokens: `ArcaneToken`
- Vendors: `Archivist`, `ScryingAdept`
- Reputation Tiers: Neutral, Allied, Honored, Exalted
- Rewards: Light/Nature/Shadow augments, scrying mirrors, arcane deco

### Polar Alliance
- Theme: Frost survival, ice magic, trade routes
- Tokens: `PolarToken`
- Vendors: `IceSmith`, `FrostBinder`
- Reputation Tiers: Neutral, Allied, Honored, Exalted
- Rewards: Frost armor recipes, cold potions, polar deco

---

## Reputation & Tokens
- Earned via quests, dungeon bosses, regional events
- Token turn-in at faction vendors; reputation unlocks inventory tiers
- Weekly decay off by default; configurable

---

## Vendor Inventories (Examples)
- Allied: consumables, basic recipes
- Honored: advanced gear, deco
- Exalted: artifacts, legendary recipes

---

## Technical Notes
- Store rep via `XmlAttachment` on player
- Token items: stackable, region-colored hues
- Vendor gumps show required rep tier
