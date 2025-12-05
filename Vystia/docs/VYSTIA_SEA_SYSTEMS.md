# Vystia Sea & Underwater Systems Guide

## Overview
Rules, gear, treasure, and encounters for sea travel and underwater content.

---

## Movement & Environment
- Underwater movement speed reduced by 30%
- Jumping disabled; pathing prefers seabed
- Visibility reduced; light sources increase radius

## Breath & Pressure
- Breath meter: 60 seconds base; drowning damage after expiry
- Pressure zones in deep areas reduce breath by +50% drain
- Safe pockets near air vents to recover breath

## Diving Gear
- Basic Diving Mask: +60s breath, light radius +3
- Reinforced Helm: Immune to pressure zones, -10% movement
- Aqualung: +180s breath, reduces drowning damage by 50%
- Crafted from Maritime tokens + Deep Gems

## Underwater Combat
- Ranged accuracy -20% unless using harpoons/crossbows
- Fire spells weakened; cold and energy unaffected
- Certain creatures resist knockback

## Sunken Treasure
- Sunken Chests: lockpicking + salvage kit required
- Loot Tables: pearls, Deep Gems, maritime contracts, artifact fragments (rare)
- Salvage Mini-Game: timed gump to align tumblers; failures spawn eels

## Encounters & Bounties
- Crypt Tracker Bounties: rotating targets in Sunken Isles
- Patrol Events: Maritime Sovereignty escorts vs pirates
- Abyssal Anomalies: random rifts spawning sea wraiths

## Technical Notes
- Use region checks to toggle underwater rules
- Attach breath timer to player via `XmlAttachment`
- Gear effects applied via item modifiers when equipped
- Chests: custom item with lockpicking + gump flow
