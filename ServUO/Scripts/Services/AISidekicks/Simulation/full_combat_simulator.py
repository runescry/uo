#!/usr/bin/env python3
"""
Full Combat Simulator

Simulates complete mage vs enemy combat with:
- Real map terrain
- Spell casting and damage
- Healing (bandages, potions, spells)
- Poison and curing
- Movement and kiting
- All timing mechanics
"""

import random
import json
from dataclasses import dataclass, field, asdict
from typing import Optional, List, Dict, Tuple
from enum import IntEnum, auto

from combat_mechanics import (
    CombatStats, CombatCalculator, CombatState,
    SpellInfo, SPELLS, PoisonLevel, POISONS
)
from map_data_extractor import MapDataExtractor
from walkability_check import WalkabilityChecker
from movement_check import MovementChecker, Direction
from fast_astar import FastAStar
from sidekick_ai_movement import SidekickMovement, MoveResult


DEFAULT_UO_PATH = r"C:\Program Files (x86)\Electronic Arts\Ultima Online Classic"


class CombatAction(IntEnum):
    """Possible combat actions"""
    None_ = 0
    CastSpell = auto()
    Bandage = auto()
    DrinkPotion = auto()
    Move = auto()
    Wait = auto()


class EnemyType(IntEnum):
    """Types of enemies for simulation"""
    OgreLord = 0       # High HP melee tank - tests kiting
    Lich = 1           # Mage enemy - tests mage vs mage


def create_ogre_lord_stats() -> CombatStats:
    """
    Create Ogre Lord stats from ServUO:
    - HP: 476-552
    - Damage: 20-25 physical
    - High magic resist (125-140)
    - Slow (dex 66-75)
    - AI: Melee rusher
    """
    return CombatStats(
        hits=random.randint(476, 552),
        hits_max=random.randint(476, 552),
        mana=0,
        mana_max=0,
        strength=random.randint(767, 945),
        dexterity=random.randint(66, 75),
        intelligence=random.randint(46, 70),
        resist=random.uniform(125.1, 140.0),  # Magic resist skill
        resist_physical=random.randint(45, 55),
        resist_fire=random.randint(30, 40),
        resist_cold=random.randint(30, 40),
        resist_poison=random.randint(40, 50),
        resist_energy=random.randint(40, 50),
    )


def create_lich_stats() -> CombatStats:
    """
    Create Lich stats from ServUO:
    - HP: 103-120
    - Damage: 24-26 (10% phys, 40% cold, 50% energy)
    - Magery 70-80, Eval Int 100
    - Necromancy 89-99
    - AI: NecroMage (casts spells, kites)
    """
    return CombatStats(
        hits=random.randint(103, 120),
        hits_max=random.randint(103, 120),
        mana=random.randint(276, 305),  # Based on Int
        mana_max=random.randint(276, 305),
        strength=random.randint(171, 200),
        dexterity=random.randint(126, 145),
        intelligence=random.randint(276, 305),
        magery=random.uniform(70.1, 80.0),
        eval_int=100.0,
        meditation=random.uniform(85.1, 95.0),
        resist=random.uniform(80.1, 100.0),
        resist_physical=random.randint(40, 60),
        resist_fire=random.randint(20, 30),
        resist_cold=random.randint(50, 60),
        resist_poison=random.randint(55, 65),
        resist_energy=random.randint(40, 50),
    )


def create_sidekick_mage_stats() -> CombatStats:
    """
    Create a typical Sidekick mage stats.
    GM skills, decent gear.
    """
    return CombatStats(
        hits=100,
        hits_max=100,
        mana=100,
        mana_max=100,
        stam=100,
        stam_max=100,
        strength=100,
        dexterity=100,
        intelligence=100,
        magery=100.0,
        eval_int=100.0,
        resist=100.0,
        anatomy=100.0,
        healing=100.0,
        meditation=100.0,
        inscribe=0.0,
        sdi=0,  # No spell damage increase
        fc=2,   # Faster casting 2
        fcr=2,  # Faster cast recovery 2
        lmc=40, # Lower mana cost cap
        resist_physical=50,
        resist_fire=50,
        resist_cold=50,
        resist_poison=50,
        resist_energy=50,
        bandages=50,
        heal_potions=5,
        greater_heal_potions=5,
        cure_potions=5,
    )


@dataclass
class SimulationConfig:
    """Configuration for combat simulation"""
    # Mage AI parameters (these are what we want to optimize)
    min_retreat_distance: int = 4
    casting_range: int = 12
    health_heal_threshold: float = 0.62
    critical_health_threshold: float = 0.29
    mana_meditation_threshold: int = 27
    bandage_cooldown: float = 10.0
    prefer_bandage_over_spell: bool = True

    # Enemy settings
    enemy_type: EnemyType = EnemyType.OgreLord

    # Simulation settings
    max_ticks: int = 1000
    tick_duration: float = 0.25  # 250ms per tick (UO standard)

    # Map settings
    use_real_map: bool = True
    map_region: Tuple[int, int, int, int] = (5379, 4, 5499, 125)  # Dungeon


@dataclass
class CombatResult:
    """Result of a combat simulation"""
    winner: str  # "mage" or "enemy"
    ticks: int
    duration_seconds: float

    mage_hp_remaining: int
    mage_mana_remaining: int
    mage_damage_dealt: int
    mage_damage_taken: int
    mage_spells_cast: int
    mage_heals: int
    mage_times_cornered: int

    enemy_hp_remaining: int
    enemy_damage_dealt: int

    # Movement stats
    mage_tiles_moved: int
    path_calculations: int
    path_failures: int


class FullCombatSimulator:
    """
    Complete combat simulator with all mechanics.
    """

    def __init__(self, config: SimulationConfig, uo_path: str = DEFAULT_UO_PATH):
        self.config = config

        # Initialize map systems if using real map
        if config.use_real_map:
            self.extractor = MapDataExtractor(uo_path)
            self.walk_checker = WalkabilityChecker(self.extractor)
            self.move_checker = MovementChecker(self.extractor, self.walk_checker)
            self.pathfinder = FastAStar(self.move_checker)
        else:
            self.extractor = None
            self.walk_checker = None
            self.move_checker = None
            self.pathfinder = None

        # Combat state
        self.mage: Optional[CombatState] = None
        self.enemy: Optional[CombatState] = None
        self.mage_movement: Optional[SidekickMovement] = None

        # Timing
        self.current_tick: int = 0
        self.current_time: float = 0.0

        # Tracking
        self.mage_tiles_moved: int = 0
        self.mage_times_cornered: int = 0
        self.path_calculations: int = 0
        self.path_failures: int = 0

        # Delayed damage queue: (tick_to_apply, damage, target)
        self.damage_queue: List[Tuple[int, int, str]] = []

    def setup_combatants(self, mage_stats: CombatStats, enemy_stats: CombatStats,
                          mage_pos: Tuple[int, int], enemy_pos: Tuple[int, int]):
        """Initialize combatants"""
        self.mage = CombatState(mage_stats)
        self.enemy = CombatState(enemy_stats)

        # Get Z levels from map
        if self.walk_checker:
            mage_z_info = self.walk_checker.get_average_z(mage_pos[0], mage_pos[1])
            enemy_z_info = self.walk_checker.get_average_z(enemy_pos[0], enemy_pos[1])
            self.mage.set_position(mage_pos[0], mage_pos[1], mage_z_info.avg_z)
            self.enemy.set_position(enemy_pos[0], enemy_pos[1], enemy_z_info.avg_z)
        else:
            self.mage.set_position(mage_pos[0], mage_pos[1], 0)
            self.enemy.set_position(enemy_pos[0], enemy_pos[1], 0)

        # Setup movement AI
        if self.pathfinder and self.move_checker:
            self.mage_movement = SidekickMovement(self.pathfinder, self.move_checker)
            self.mage_movement.set_position(*self.mage.position)

        # Reset tracking
        self.current_tick = 0
        self.current_time = 0.0
        self.mage_tiles_moved = 0
        self.mage_times_cornered = 0
        self.path_calculations = 0
        self.path_failures = 0
        self.damage_queue = []

    def simulate(self) -> CombatResult:
        """Run the combat simulation"""
        if not self.mage or not self.enemy:
            raise ValueError("Must call setup_combatants first")

        while self.current_tick < self.config.max_ticks:
            # Check for combat end
            if self.mage.stats.hits <= 0:
                return self._create_result("enemy")
            if self.enemy.stats.hits <= 0:
                return self._create_result("mage")

            # Process delayed damage
            self._process_damage_queue()

            # Process poison ticks
            self._process_poison()

            # Mage turn
            self._mage_turn()

            # Enemy turn
            self._enemy_turn()

            # Advance time
            self.current_tick += 1
            self.current_time += self.config.tick_duration

        # Timeout - determine winner by HP percentage
        mage_hp_pct = self.mage.stats.hp_percent
        enemy_hp_pct = self.enemy.stats.hp_percent
        winner = "mage" if mage_hp_pct >= enemy_hp_pct else "enemy"

        return self._create_result(winner)

    def _mage_turn(self):
        """Execute mage AI turn"""
        distance = self.mage.distance_to(self.enemy)
        hp_pct = self.mage.stats.hp_percent
        mana = self.mage.stats.mana

        # Priority 1: Critical health - emergency heal
        if hp_pct < self.config.critical_health_threshold:
            if self._try_emergency_heal():
                return

        # Priority 2: Cure poison
        if self.mage.stats.poisoned:
            if self._try_cure_poison():
                return

        # Priority 3: Regular healing
        if hp_pct < self.config.health_heal_threshold:
            if self._try_heal():
                return

        # Priority 4: Too close - retreat
        if distance < self.config.min_retreat_distance:
            self._mage_retreat()
            return

        # Priority 5: In casting range with mana - attack
        if distance <= self.config.casting_range and mana >= 20:
            if not self.mage.is_casting:
                self._start_spell_cast()
                return

        # Priority 6: Too far - approach
        if distance > self.config.casting_range:
            self._mage_approach()
            return

        # Priority 7: Already casting - wait
        if self.mage.is_casting:
            if self.current_time >= self.mage.cast_end_time:
                self._finish_spell_cast()
            return

        # Default: maintain position
        pass

    def _enemy_turn(self):
        """Enemy AI based on enemy type"""
        if self.config.enemy_type == EnemyType.OgreLord:
            self._ogre_lord_turn()
        elif self.config.enemy_type == EnemyType.Lich:
            self._lich_turn()

    def _ogre_lord_turn(self):
        """Ogre Lord AI - pure melee rusher, slow but hard hitting"""
        distance = self.mage.distance_to(self.enemy)

        # Ogre Lord is slow (dex 66-75), moves ~0.7 tiles per tick when running
        # Move toward mage
        if distance > 1.5:  # Not in melee range
            self._enemy_move_toward_mage(speed=0.7)  # Slower than player
        else:
            # In melee - attack with heavy damage (20-25)
            self._enemy_melee_attack(min_damage=20, max_damage=25)

    def _lich_turn(self):
        """Lich AI - NecroMage, casts spells and tries to maintain distance"""
        distance = self.mage.distance_to(self.enemy)
        mana = self.enemy.stats.mana

        # Lich tries to stay at range and cast spells
        # If too close, retreat
        if distance < 4 and not self.enemy.is_casting:
            self._enemy_retreat_from_mage()
            return

        # If in casting range and has mana, cast
        if distance <= 12 and mana >= 20 and not self.enemy.is_casting:
            self._lich_start_cast()
            return

        # If casting, check if done
        if self.enemy.is_casting:
            if self.current_time >= self.enemy.cast_end_time:
                self._lich_finish_cast()
            return

        # If too far, approach to casting range
        if distance > 14:
            self._enemy_move_toward_mage(speed=1.0)  # Lich is faster (high dex)
            return

        # Otherwise maintain position

    def _lich_start_cast(self):
        """Lich starts casting a spell"""
        mana = self.enemy.stats.mana

        # Choose spell based on mana
        if mana >= 20:
            spell_name = "energy_bolt"
        elif mana >= 11:
            spell_name = "lightning"
        else:
            spell_name = "magic_arrow"

        spell = SPELLS.get(spell_name)
        if not spell:
            return

        # Lich has lower magery (70-80), so longer cast times
        cast_time = CombatCalculator.calculate_cast_time(spell, self.enemy.stats)
        self.enemy.casting_spell = spell_name
        self.enemy.cast_end_time = self.current_time + cast_time

    def _lich_finish_cast(self):
        """Lich finishes casting and deals damage"""
        if not self.enemy.casting_spell:
            return

        spell = SPELLS.get(self.enemy.casting_spell)
        if not spell:
            self.enemy.casting_spell = None
            return

        # Consume mana
        mana_cost = CombatCalculator.calculate_mana_cost(spell, self.enemy.stats)
        self.enemy.stats.mana -= mana_cost
        self.enemy.spells_cast += 1

        # Calculate damage (lich has 100 eval int but lower magery)
        damage = CombatCalculator.calculate_spell_damage(spell, self.enemy.stats, self.mage.stats)

        # Apply damage to mage
        if spell.delayed_damage:
            delay_ticks = int(spell.delay_seconds / self.config.tick_duration)
            self.damage_queue.append((self.current_tick + delay_ticks, damage, "mage"))
        else:
            self.mage.stats.hits = max(0, self.mage.stats.hits - damage)
            self.mage.damage_taken += damage

        self.enemy.casting_spell = None
        self.enemy.damage_dealt += damage

    def _enemy_retreat_from_mage(self):
        """Enemy moves away from mage"""
        dx = self.enemy.x - self.mage.x
        dy = self.enemy.y - self.mage.y

        # Move away
        move_x = 0 if dx == 0 else (1 if dx > 0 else -1)
        move_y = 0 if dy == 0 else (1 if dy > 0 else -1)

        new_x = self.enemy.x + move_x
        new_y = self.enemy.y + move_y

        if self.walk_checker:
            if self.walk_checker.can_fit(new_x, new_y, self.enemy.z):
                self.enemy.set_position(new_x, new_y, self.enemy.z)
        else:
            self.enemy.set_position(new_x, new_y, self.enemy.z)

    def _try_emergency_heal(self) -> bool:
        """Emergency heal attempt"""
        # Try greater heal potion first
        if self.mage.stats.greater_heal_potions > 0 and self.current_time >= self.mage.potion_cooldown_time:
            heal = CombatCalculator.calculate_potion_heal("greater", self.mage.stats)
            self.mage.stats.hits = min(self.mage.stats.hits_max, self.mage.stats.hits + heal)
            self.mage.stats.greater_heal_potions -= 1
            self.mage.potion_cooldown_time = self.current_time + 10.0
            self.mage.heals_done += 1
            return True

        # Try Greater Heal spell
        if self.mage.stats.mana >= 11:
            heal = CombatCalculator.calculate_spell_heal("greater_heal", self.mage.stats)
            self.mage.stats.hits = min(self.mage.stats.hits_max, self.mage.stats.hits + heal)
            self.mage.stats.mana -= 11
            self.mage.heals_done += 1
            return True

        return False

    def _try_cure_poison(self) -> bool:
        """Try to cure poison"""
        # Try cure potion
        if self.mage.stats.cure_potions > 0:
            # Simplified: assume cure always works for lesser/regular
            if self.mage.stats.poison_level in (PoisonLevel.Lesser, PoisonLevel.Regular):
                self.mage.stats.poisoned = False
                self.mage.stats.poison_level = None
                self.mage.stats.cure_potions -= 1
                return True

        # Try Cure spell
        if self.mage.stats.mana >= 6 and self.mage.stats.poison_level is not None:
            cure_chance = CombatCalculator.calculate_cure_chance(
                self.mage.stats, self.mage.stats.poison_level
            )
            if random.random() < cure_chance:
                self.mage.stats.poisoned = False
                self.mage.stats.poison_level = None
            self.mage.stats.mana -= 6
            return True

        return False

    def _try_heal(self) -> bool:
        """Try to heal"""
        # Prefer bandage if available and not on cooldown
        if (self.config.prefer_bandage_over_spell and
            self.mage.stats.bandages > 0 and
            not self.mage.is_bandaging and
            self.current_time >= self.mage.bandage_end_time):

            heal_amount, heal_time = CombatCalculator.calculate_bandage_heal(self.mage.stats)
            self.mage.bandage_end_time = self.current_time + heal_time
            self.mage.stats.bandages -= 1
            # Actual heal will be applied when bandage completes
            return True

        # Check if bandage just completed
        if self.mage.is_bandaging and self.current_time >= self.mage.bandage_end_time:
            heal_amount, _ = CombatCalculator.calculate_bandage_heal(self.mage.stats)
            self.mage.stats.hits = min(self.mage.stats.hits_max, self.mage.stats.hits + heal_amount)
            self.mage.bandage_end_time = self.current_time + self.config.bandage_cooldown
            self.mage.heals_done += 1
            return True

        # Try heal potion
        if self.mage.stats.heal_potions > 0 and self.current_time >= self.mage.potion_cooldown_time:
            heal = CombatCalculator.calculate_potion_heal("regular", self.mage.stats)
            self.mage.stats.hits = min(self.mage.stats.hits_max, self.mage.stats.hits + heal)
            self.mage.stats.heal_potions -= 1
            self.mage.potion_cooldown_time = self.current_time + 8.0
            self.mage.heals_done += 1
            return True

        return False

    def _start_spell_cast(self):
        """Start casting a spell"""
        # Choose spell based on mana
        mana = self.mage.stats.mana

        if mana >= 40:
            # Use explosion combo
            spell_name = "explosion"
        elif mana >= 20:
            spell_name = "energy_bolt"
        elif mana >= 11:
            spell_name = "lightning"
        else:
            spell_name = "magic_arrow"

        spell = SPELLS.get(spell_name)
        if not spell:
            return

        cast_time = CombatCalculator.calculate_cast_time(spell, self.mage.stats)
        mana_cost = CombatCalculator.calculate_mana_cost(spell, self.mage.stats)

        if mana_cost > self.mage.stats.mana:
            return

        self.mage.casting_spell = spell_name
        self.mage.cast_end_time = self.current_time + cast_time

    def _finish_spell_cast(self):
        """Complete spell cast and deal damage"""
        if not self.mage.casting_spell:
            return

        spell = SPELLS.get(self.mage.casting_spell)
        if not spell:
            self.mage.casting_spell = None
            return

        mana_cost = CombatCalculator.calculate_mana_cost(spell, self.mage.stats)

        # Consume mana
        self.mage.stats.mana -= mana_cost
        self.mage.spells_cast += 1

        # Calculate damage
        damage = CombatCalculator.calculate_spell_damage(spell, self.mage.stats, self.enemy.stats)

        # Apply or queue damage
        if spell.delayed_damage:
            delay_ticks = int(spell.delay_seconds / self.config.tick_duration)
            self.damage_queue.append((self.current_tick + delay_ticks, damage, "enemy"))
        else:
            self._apply_damage_to_enemy(damage)

        self.mage.casting_spell = None
        self.mage.damage_dealt += damage

    def _apply_damage_to_enemy(self, damage: int):
        """Apply damage to enemy"""
        self.enemy.stats.hits = max(0, self.enemy.stats.hits - damage)
        self.mage.damage_dealt += damage

    def _mage_retreat(self):
        """Mage retreats from enemy"""
        if not self.mage_movement:
            return

        self.path_calculations += 1
        result = self.mage_movement.run_from(self.enemy.x, self.enemy.y)

        if result == MoveResult.Success:
            pos = self.mage_movement.get_position()
            self.mage.set_position(*pos)
            self.mage_tiles_moved += 1
        elif result == MoveResult.Blocked:
            self.path_failures += 1
            if self.mage_movement.is_cornered(self.enemy.x, self.enemy.y):
                self.mage_times_cornered += 1

    def _mage_approach(self):
        """Mage approaches enemy"""
        if not self.mage_movement:
            return

        self.path_calculations += 1
        result = self.mage_movement.move_to_target(
            self.enemy.x, self.enemy.y, self.enemy.z,
            casting_range=self.config.casting_range
        )

        if result == MoveResult.Success:
            pos = self.mage_movement.get_position()
            self.mage.set_position(*pos)
            self.mage_tiles_moved += 1
        elif result == MoveResult.PathNotFound:
            self.path_failures += 1

    def _enemy_move_toward_mage(self, speed: float = 1.0):
        """Enemy moves toward mage (simple direct movement)

        Args:
            speed: Movement speed multiplier (0.7 for ogre lord, 1.0 for lich)
        """
        # Speed check - slower enemies don't always move each tick
        if speed < 1.0 and random.random() > speed:
            return  # Skip this tick's movement

        dx = self.mage.x - self.enemy.x
        dy = self.mage.y - self.enemy.y

        # Normalize to single tile movement
        move_x = 0 if dx == 0 else (1 if dx > 0 else -1)
        move_y = 0 if dy == 0 else (1 if dy > 0 else -1)

        new_x = self.enemy.x + move_x
        new_y = self.enemy.y + move_y

        # Check walkability if we have map data
        if self.move_checker:
            # Simplified check - just verify destination is walkable
            if self.walk_checker.can_fit(new_x, new_y, self.enemy.z):
                self.enemy.set_position(new_x, new_y, self.enemy.z)
        else:
            self.enemy.set_position(new_x, new_y, self.enemy.z)

    def _enemy_melee_attack(self, min_damage: int = 10, max_damage: int = 20):
        """Enemy performs melee attack

        Args:
            min_damage: Minimum damage (20 for ogre lord)
            max_damage: Maximum damage (25 for ogre lord)
        """
        damage = random.randint(min_damage, max_damage)
        self.mage.stats.hits = max(0, self.mage.stats.hits - damage)
        self.mage.damage_taken += damage
        self.enemy.damage_dealt += damage

    def _process_damage_queue(self):
        """Process delayed damage"""
        remaining = []
        for tick, damage, target in self.damage_queue:
            if tick <= self.current_tick:
                if target == "enemy":
                    self._apply_damage_to_enemy(damage)
                else:
                    self.mage.stats.hits = max(0, self.mage.stats.hits - damage)
                    self.mage.damage_taken += damage
            else:
                remaining.append((tick, damage, target))
        self.damage_queue = remaining

    def _process_poison(self):
        """Process poison damage"""
        if self.mage.stats.poisoned and self.mage.stats.poison_ticks_remaining > 0:
            poison_info = POISONS.get(self.mage.stats.poison_level)
            if poison_info:
                # Check if it's time for a poison tick
                tick_interval = int(poison_info.interval / self.config.tick_duration)
                if self.current_tick % tick_interval == 0:
                    damage = CombatCalculator.calculate_poison_damage(self.mage.stats)
                    self.mage.stats.hits = max(0, self.mage.stats.hits - damage)
                    self.mage.damage_taken += damage
                    self.mage.stats.poison_ticks_remaining -= 1

                    if self.mage.stats.poison_ticks_remaining <= 0:
                        self.mage.stats.poisoned = False
                        self.mage.stats.poison_level = None

    def _create_result(self, winner: str) -> CombatResult:
        """Create combat result"""
        return CombatResult(
            winner=winner,
            ticks=self.current_tick,
            duration_seconds=self.current_time,
            mage_hp_remaining=self.mage.stats.hits,
            mage_mana_remaining=self.mage.stats.mana,
            mage_damage_dealt=self.mage.damage_dealt,
            mage_damage_taken=self.mage.damage_taken,
            mage_spells_cast=self.mage.spells_cast,
            mage_heals=self.mage.heals_done,
            mage_times_cornered=self.mage_times_cornered,
            enemy_hp_remaining=self.enemy.stats.hits,
            enemy_damage_dealt=self.enemy.damage_dealt,
            mage_tiles_moved=self.mage_tiles_moved,
            path_calculations=self.path_calculations,
            path_failures=self.path_failures,
        )


def run_batch_simulation(config: SimulationConfig, num_simulations: int = 100,
                          verbose: bool = False) -> Dict:
    """Run batch simulations and collect statistics"""
    results = []

    # Create simulator once (expensive due to map loading)
    sim = FullCombatSimulator(config)

    # Define spawn region
    min_x, min_y, max_x, max_y = config.map_region

    for i in range(num_simulations):
        # Random spawn positions
        mage_x = random.randint(min_x + 10, max_x - 10)
        mage_y = random.randint(min_y + 10, max_y - 10)
        enemy_x = mage_x + random.randint(-15, 15)
        enemy_y = mage_y + random.randint(-15, 15)

        # Ensure enemy is in bounds
        enemy_x = max(min_x, min(max_x, enemy_x))
        enemy_y = max(min_y, min(max_y, enemy_y))

        # Create combatants based on enemy type
        mage_stats = create_sidekick_mage_stats()

        if config.enemy_type == EnemyType.OgreLord:
            enemy_stats = create_ogre_lord_stats()
        elif config.enemy_type == EnemyType.Lich:
            enemy_stats = create_lich_stats()
        else:
            enemy_stats = create_ogre_lord_stats()  # Default

        sim.setup_combatants(
            mage_stats, enemy_stats,
            (mage_x, mage_y), (enemy_x, enemy_y)
        )

        result = sim.simulate()
        results.append(result)

        if verbose and (i + 1) % 10 == 0:
            print(f"Completed {i + 1}/{num_simulations} simulations")

    # Calculate statistics
    wins = sum(1 for r in results if r.winner == "mage")
    avg_duration = sum(r.duration_seconds for r in results) / len(results)
    avg_damage_dealt = sum(r.mage_damage_dealt for r in results) / len(results)
    avg_damage_taken = sum(r.mage_damage_taken for r in results) / len(results)
    avg_spells = sum(r.mage_spells_cast for r in results) / len(results)
    avg_heals = sum(r.mage_heals for r in results) / len(results)
    avg_cornered = sum(r.mage_times_cornered for r in results) / len(results)
    total_path_failures = sum(r.path_failures for r in results)

    return {
        "num_simulations": num_simulations,
        "config": asdict(config),
        "win_rate": wins / len(results),
        "avg_duration_seconds": avg_duration,
        "avg_damage_dealt": avg_damage_dealt,
        "avg_damage_taken": avg_damage_taken,
        "avg_spells_cast": avg_spells,
        "avg_heals": avg_heals,
        "avg_times_cornered": avg_cornered,
        "total_path_failures": total_path_failures,
    }


def main():
    """Run example simulation with both enemy types"""
    print("=" * 70)
    print("FULL COMBAT SIMULATOR")
    print("=" * 70)

    # Test vs Ogre Lord (melee tank)
    print("\n" + "=" * 70)
    print("TEST 1: Sidekick Mage vs OGRE LORD (Melee Tank)")
    print("=" * 70)
    print("Ogre Lord: 476-552 HP, 20-25 damage, slow (dex 66-75)")

    ogre_config = SimulationConfig(
        min_retreat_distance=4,
        casting_range=12,
        health_heal_threshold=0.62,
        critical_health_threshold=0.29,
        max_ticks=1000,
        enemy_type=EnemyType.OgreLord,
    )

    ogre_results = run_batch_simulation(ogre_config, num_simulations=20, verbose=True)

    print(f"\n--- vs Ogre Lord Results (20 fights) ---")
    print(f"Win rate: {ogre_results['win_rate']*100:.1f}%")
    print(f"Avg duration: {ogre_results['avg_duration_seconds']:.1f}s")
    print(f"Avg damage dealt: {ogre_results['avg_damage_dealt']:.0f}")
    print(f"Avg damage taken: {ogre_results['avg_damage_taken']:.0f}")
    print(f"Avg heals: {ogre_results['avg_heals']:.1f}")
    print(f"Avg times cornered: {ogre_results['avg_times_cornered']:.1f}")

    # Test vs Lich (mage)
    print("\n" + "=" * 70)
    print("TEST 2: Sidekick Mage vs LICH (Enemy Mage)")
    print("=" * 70)
    print("Lich: 103-120 HP, casts Energy Bolt/Lightning, kites")

    lich_config = SimulationConfig(
        min_retreat_distance=4,
        casting_range=12,
        health_heal_threshold=0.62,
        critical_health_threshold=0.29,
        max_ticks=1000,
        enemy_type=EnemyType.Lich,
    )

    lich_results = run_batch_simulation(lich_config, num_simulations=20, verbose=True)

    print(f"\n--- vs Lich Results (20 fights) ---")
    print(f"Win rate: {lich_results['win_rate']*100:.1f}%")
    print(f"Avg duration: {lich_results['avg_duration_seconds']:.1f}s")
    print(f"Avg damage dealt: {lich_results['avg_damage_dealt']:.0f}")
    print(f"Avg damage taken: {lich_results['avg_damage_taken']:.0f}")
    print(f"Avg heals: {lich_results['avg_heals']:.1f}")
    print(f"Avg times cornered: {lich_results['avg_times_cornered']:.1f}")

    # Summary
    print("\n" + "=" * 70)
    print("SUMMARY")
    print("=" * 70)
    print(f"{'Enemy':<15} {'Win Rate':<12} {'Avg Duration':<15} {'Dmg Dealt':<12} {'Dmg Taken':<12}")
    print("-" * 66)
    print(f"{'Ogre Lord':<15} {ogre_results['win_rate']*100:.1f}%{'':<7} "
          f"{ogre_results['avg_duration_seconds']:.1f}s{'':<10} "
          f"{ogre_results['avg_damage_dealt']:.0f}{'':<8} "
          f"{ogre_results['avg_damage_taken']:.0f}")
    print(f"{'Lich':<15} {lich_results['win_rate']*100:.1f}%{'':<7} "
          f"{lich_results['avg_duration_seconds']:.1f}s{'':<10} "
          f"{lich_results['avg_damage_dealt']:.0f}{'':<8} "
          f"{lich_results['avg_damage_taken']:.0f}")


if __name__ == "__main__":
    main()
