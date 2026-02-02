# Bard Songweaving Channeling + Limited Stacking Proposal

## Goals
- Add a clear "channeling" state for sustained songs.
- Prevent runaway buff stacking while keeping bards fun to play.
- Make Crescendo meaningful for both sustain and finales.

## Core Rules (Baseline)
1. **One sustained song per bard**  
   A bard can channel only one active song at a time.
2. **One bard-song buff per target**  
   A target can only receive one sustained bard buff at once. A new song replaces the old one based on priority.
3. **Finales are separate**  
   Finales are instant effects that can stack with sustained songs but are gated by cooldown and Crescendo.

## Channeling Mechanics
- **Start Channeling:** Casting a song applies its effect and enters a channeling state.
- **Sustain Cost:** Drain mana or stamina per tick (e.g., 2 mana/sec).
- **Interrupts:** Movement or damage can break channeling unless a skill check succeeds.
- **Retune Time:** Switching songs requires a short retune (0.5–1.0s) instead of a full cast.

## Crescendo Rules
- **Build Rate:** +1 every 3s while channeling.
- **Decay:** -1 every 3s when not channeling.
- **Finale Cost:** Consumes 25/50/75 Crescendo based on tier.
- **Finale Cooldown:** 15–20s (prevents spam).

## Concentration System

**Concentration** is the Bard's attention/focus resource.

| Property | Value |
|----------|-------|
| Maximum | 100 |
| Recovery | Returns when song expires or is cancelled |
| Regen | None while songs active; full restore after 10s with no songs |

### Concentration Costs

Songs **reserve** Concentration while active:

| Song | Base Cost | Per Duration Level |
|------|-----------|-------------------|
| Provocation | 35 | -2 |
| Peacemaking | 30 | -2 |
| Discordance | 30 | -2 |
| Courage | 25 | -2 |
| Swiftness | 25 | -2 |
| Mending | 25 | -2 |
| Requiem | 20 | -2 |
| Light/Fortune | 0 | 0 |

### Example

Bard with Provocation Duration 8, Discordance Duration 6:
- Provocation costs: 35 - (8 × 2) = 19
- Discordance costs: 30 - (6 × 2) = 18
- Can maintain both (37) plus Courage base (25) = 62 total
- Has 38 Concentration buffer for emergency Peace or Mending

## Intensity Tiers
Each song has 3 tiers (I/II/III) tied to skill or Crescendo:
- Tier I: 0–25 Crescendo
- Tier II: 26–50 Crescendo
- Tier III: 51–100 Crescendo

## Example Songs (Sustained)
- **Song of Courage:** +5/+10/+15 all stats
- **Song of Swiftness:** +5/+10/+15 dex and +5/+10/+15 movement speed
- **Song of Healing:** party HoT pulses while channeling
- **Song of Provocation:** enemy accuracy/damage reduction in radius
- **Song of Illumination:** night sight + stealth detection radius
- **Cacophony:** periodic AoE damage + stamina drain

## Finales (Instant)
- High impact effects triggered by Crescendo spend.
- Ends channeling when used.
- Applies a short global cooldown to prevent chaining.

## Priority / Replacement Rules
- If a target already has a bard song, the incoming song replaces it only if:
  - higher tier, or
  - higher bard skill, or
  - same tier but newer cast (optional rule).

## Optional Looser Variant (If Too Restrictive)
- **Primary + Motif:** Allow a second “motif” song at reduced power (50%) with higher drain.
- **Solo Baseline:** Allow self buffs even when no party is present (default recommended).

## Open Decisions
- Final decay rule if not channeling.
- Allow self-buff when solo (recommended).
- Retune time and interrupt thresholds.
