# Wandering NPC & Creature Tracking Roadmap

## Overview
This roadmap outlines the plan for tracking fully wandering NPCs and creatures that move beyond their spawn radius. 

**Current Status (v1.6.0)**: The system is optimized for town NPCs that stay within their `RangeHome` (5-10 tiles) of spawn points. The static location database works perfectly for these NPCs since they don't wander far. This roadmap covers future enhancements for NPCs that do wander extensively.

## Current State

### What Works Now (v1.6.0)
- **Static Database**: Tracks NPC spawn locations by town/region (auto-populated on server start)
- **Caching System**: 30-second TTL cache for nearby NPC lookups (optimized for town NPCs)
- **Movement Tracking**: Updates database when NPCs move >5 tiles (for significant movement)
- **Region Scanning**: Fallback to dynamic region scanning when static DB is insufficient
- **Parent Region Fallback**: Handles sub-regions (buildings within cities)
- **Cross-Town Referrals**: Suggests other towns when local NPCs unavailable
- **Distance Descriptions**: "nearby", "a short walk", "across town"
- **Multiple NPC Options**: Can mention 2-3 nearby NPCs of same type
- **NPC Name Mentions**: Includes NPC names in directions when available

### Limitations
- Optimized for town NPCs that don't wander far
- Static database assumes NPCs stay near spawn point
- No tracking for NPCs without spawners (manually placed)
- No tracking for creatures that wander extensively
- No periodic refresh for NPCs that move beyond their home range

## Phase 1: Enhanced Static Database (Foundation)

### 1.1 Periodic Location Refresh
**Goal**: Update static database periodically for NPCs that may have moved

**Implementation**:
- Add background timer (every 60 seconds)
- Scan all tracked NPCs in static database
- Update locations if NPC has moved >10 tiles from last known position
- Only update if NPC is still in same region/town

**Files to Modify**:
- `NPCLocationDatabase.cs`: Add `RefreshLocations()` method
- `LLMInitializer.cs`: Register background timer

**Performance Considerations**:
- Batch updates (process 10-20 NPCs per tick)
- Skip NPCs that haven't moved
- Use async/background processing

### 1.2 Spawn Point Tracking
**Goal**: Track original spawn point separately from current location

**Implementation**:
```csharp
public struct LocationInfo
{
    public Point3D SpawnLocation;  // Original spawn point
    public Point3D CurrentLocation; // Current position
    public DateTime LastVerified;
    public int Serial;
    public bool IsWandering; // True if moved >RangeHome from spawn
}
```

**Benefits**:
- Know if NPC has wandered from spawn
- Can provide "near spawn point" vs "current location" directions
- Better accuracy for NPCs that return to spawn

### 1.3 RangeHome Integration
**Goal**: Use NPC's `RangeHome` property to determine if they've wandered

**Implementation**:
- Check `BaseCreature.RangeHome` when updating location
- Mark NPC as "wandering" if distance from spawn > RangeHome
- Prioritize current location for wandering NPCs, spawn location for stationary

## Phase 2: Dynamic Tracking for Wandering NPCs

### 2.1 Movement-Based Updates
**Goal**: More frequent updates for NPCs that are actively wandering

**Implementation**:
- Track movement frequency per NPC
- NPCs that move frequently get more frequent updates
- NPCs that stay put get less frequent updates

**Algorithm**:
```csharp
if (movementFrequency > threshold)
    updateInterval = 15 seconds; // Active wanderer
else if (distanceFromSpawn > RangeHome)
    updateInterval = 30 seconds; // Wandering but slow
else
    updateInterval = 60 seconds; // Stationary
```

### 2.2 Region Boundary Detection
**Goal**: Detect when NPCs cross region boundaries

**Implementation**:
- Track region changes in `OnLocationChange`
- Update static database when NPC changes regions
- Remove from old region, add to new region
- Invalidate caches for both regions

**Files to Modify**:
- `LLMNpc.cs`: Enhanced `HandleLocationChange()`
- `NPCLocationDatabase.cs`: `MoveNPCToRegion()` method

### 2.3 Waypoint Tracking
**Goal**: Track NPCs following waypoints or nav points

**Implementation**:
- Detect `CurrentWayPoint` or `NavPoints`
- Predict likely path based on waypoint sequence
- Update location more frequently when following waypoints
- Store waypoint information in database

**Benefits**:
- More accurate directions for NPCs on patrol routes
- Can predict where NPC will be

## Phase 3: Creature Tracking (Non-Conversational)

### 3.1 Creature Database
**Goal**: Track non-conversational creatures for combat/quest purposes

**Implementation**:
- Separate database for creatures (not just conversational NPCs)
- Track by creature type/name rather than role
- Lower update frequency (creatures move more)
- Optional feature (can be disabled for performance)

**Use Cases**:
- "Where can I find orcs?"
- "Is there a dragon nearby?"
- Quest-related creature location queries

### 3.2 Spawner Integration
**Goal**: Track creatures spawned by spawners

**Implementation**:
- Hook into spawner spawn events
- Track spawner location and creature type
- Update when creatures are spawned/despawned
- Handle spawner-based respawns

**Files to Modify**:
- Create `CreatureLocationDatabase.cs`
- Hook into `Spawner.Spawn()` events

## Phase 4: Advanced Features

### 4.1 Predictive Location
**Goal**: Predict where NPCs will be based on movement patterns

**Implementation**:
- Track movement history (last 5-10 positions)
- Calculate average movement direction/speed
- Predict likely location in next 30 seconds
- Use for "where will they be" queries

**Complexity**: High
**Performance Impact**: Medium

### 4.2 Movement Pattern Learning
**Goal**: Learn NPC movement patterns over time

**Implementation**:
- Track time-based movement patterns
- Learn "NPC X is usually at location Y at time Z"
- Use patterns to improve accuracy
- Store patterns in database

**Complexity**: Very High
**Performance Impact**: Low (background processing)

### 4.3 Multi-Map Tracking
**Goal**: Track NPCs across different maps (Felucca/Trammel)

**Implementation**:
- Store map information in location database
- Handle map transitions
- Provide map-specific directions
- Track NPCs on multiple maps separately

## Phase 5: Performance Optimizations

### 5.1 Lazy Loading
**Goal**: Only track NPCs that are actually queried

**Implementation**:
- Don't pre-populate all NPCs
- Add to database on first query
- Remove NPCs that haven't been queried in X hours

### 5.2 Spatial Indexing
**Goal**: Fast location-based queries

**Implementation**:
- Use spatial data structure (quadtree, R-tree)
- Fast "find NPCs within X tiles" queries
- Efficient region-based lookups

### 5.3 Batch Processing
**Goal**: Process multiple updates efficiently

**Implementation**:
- Queue location updates
- Process in batches every N seconds
- Reduce database lock contention
- Parallel processing where safe

## Implementation Priority

### High Priority (Phase 1)
1. Periodic location refresh (1.1)
2. Spawn point tracking (1.2)
3. RangeHome integration (1.3)

**Reasoning**: Foundation for all other features, relatively simple to implement

### Medium Priority (Phase 2)
1. Movement-based updates (2.1)
2. Region boundary detection (2.2)

**Reasoning**: Improves accuracy for wandering NPCs without major performance impact

### Low Priority (Phase 3-5)
- Creature tracking (Phase 3)
- Advanced features (Phase 4)
- Performance optimizations (Phase 5)

**Reasoning**: Nice-to-have features, can be added as needed

## Testing Strategy

### Unit Tests
- Test location updates
- Test region boundary crossing
- Test spawn point tracking
- Test cache invalidation

### Integration Tests
- Test with actual wandering NPCs
- Test with waypoint-following NPCs
- Test with region-crossing NPCs
- Performance testing with 100+ NPCs

### Performance Benchmarks
- Measure update overhead
- Measure query performance
- Measure memory usage
- Compare before/after optimizations

## Success Metrics

### Accuracy
- Location accuracy within 10 tiles: >95%
- Direction accuracy: >90%
- Region detection accuracy: >99%

### Performance
- Update overhead: <5% CPU
- Query time: <10ms average
- Memory usage: <50MB for 1000 NPCs

### User Experience
- Players find NPCs on first try: >90%
- Directions are helpful: >85%
- No noticeable lag from tracking: Yes

## Future Considerations

### Distributed Tracking
- Track NPCs across multiple servers
- Share location data between shards
- Cross-server referrals

### Machine Learning
- Use ML to predict NPC locations
- Learn optimal update frequencies
- Predict movement patterns

### Real-Time Updates
- WebSocket-based real-time location updates
- Live tracking for admins
- Player-visible NPC tracking (optional)

## Notes

- Current system is optimized for town NPCs (good enough for now)
- Wandering NPC tracking is a "nice to have" feature
- Performance is critical - don't add overhead unless needed
- Start with Phase 1, measure impact, then decide on Phase 2+
- Consider making advanced features optional/configurable

