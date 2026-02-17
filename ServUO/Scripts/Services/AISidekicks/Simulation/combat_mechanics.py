#!/usr/bin/env python3
"""
Combat Mechanics Module

Contains all the UO combat formulas extracted from ServUO for accurate simulation.
This includes spell damage, healing, poison, and timing calculations.
"""

import random
from enum import IntEnum, auto
from dataclasses import dataclass, field
from typing import Optional, Tuple, List


class SpellCircle(IntEnum):
    """Magery spell circles"""
    First = 0
    Second = 1
    Third = 2
    Fourth = 3
    Fifth = 4
    Sixth = 5
    Seventh = 6
    Eighth = 7


class PoisonLevel(IntEnum):
    """Poison severity levels"""
    Lesser = 0
    Regular = 1
    Greater = 2
    Deadly = 3
    Lethal = 4


class DamageType(IntEnum):
    """Damage element types"""
    Physical = 0
    Fire = 1
    Cold = 2
    Poison = 3
    Energy = 4


@dataclass
class SpellInfo:
    """Information about a spell"""
    name: str
    circle: SpellCircle
    mana_cost: int
    base_damage: int
    dice_count: int
    dice_sides: int
    damage_type: DamageType
    delayed_damage: bool
    delay_seconds: float
    range: int = 10

    @property
    def cast_time_aos(self) -> float:
        """AOS cast time in seconds"""
        return (4 + int(self.circle)) * 0.25

    @property
    def cast_time_pre_aos(self) -> float:
        """Pre-AOS cast time in seconds"""
        return 0.5 + (0.25 * int(self.circle))


# Spell database
SPELLS = {
    "magic_arrow": SpellInfo(
        name="Magic Arrow",
        circle=SpellCircle.First,
        mana_cost=4,
        base_damage=10,
        dice_count=1,
        dice_sides=4,
        damage_type=DamageType.Fire,
        delayed_damage=True,
        delay_seconds=1.0,
    ),
    "fireball": SpellInfo(
        name="Fireball",
        circle=SpellCircle.Third,
        mana_cost=9,
        base_damage=19,
        dice_count=1,
        dice_sides=5,
        damage_type=DamageType.Fire,
        delayed_damage=True,
        delay_seconds=1.0,
    ),
    "lightning": SpellInfo(
        name="Lightning",
        circle=SpellCircle.Fourth,
        mana_cost=11,
        base_damage=23,
        dice_count=1,
        dice_sides=4,
        damage_type=DamageType.Energy,
        delayed_damage=False,
        delay_seconds=0.0,
    ),
    "energy_bolt": SpellInfo(
        name="Energy Bolt",
        circle=SpellCircle.Sixth,
        mana_cost=20,
        base_damage=40,
        dice_count=1,
        dice_sides=5,
        damage_type=DamageType.Energy,
        delayed_damage=True,
        delay_seconds=1.0,
    ),
    "explosion": SpellInfo(
        name="Explosion",
        circle=SpellCircle.Sixth,
        mana_cost=20,
        base_damage=40,
        dice_count=1,
        dice_sides=5,
        damage_type=DamageType.Fire,
        delayed_damage=True,
        delay_seconds=3.0,  # Long delay!
    ),
    "flamestrike": SpellInfo(
        name="Flamestrike",
        circle=SpellCircle.Seventh,
        mana_cost=40,
        base_damage=48,
        dice_count=1,
        dice_sides=5,
        damage_type=DamageType.Fire,
        delayed_damage=False,
        delay_seconds=0.0,
    ),
    "heal": SpellInfo(
        name="Heal",
        circle=SpellCircle.First,
        mana_cost=4,
        base_damage=0,  # Healing spell
        dice_count=0,
        dice_sides=0,
        damage_type=DamageType.Physical,
        delayed_damage=False,
        delay_seconds=0.0,
    ),
    "greater_heal": SpellInfo(
        name="Greater Heal",
        circle=SpellCircle.Fourth,
        mana_cost=11,
        base_damage=0,  # Healing spell
        dice_count=0,
        dice_sides=0,
        damage_type=DamageType.Physical,
        delayed_damage=False,
        delay_seconds=0.0,
    ),
    "cure": SpellInfo(
        name="Cure",
        circle=SpellCircle.Second,
        mana_cost=6,
        base_damage=0,
        dice_count=0,
        dice_sides=0,
        damage_type=DamageType.Physical,
        delayed_damage=False,
        delay_seconds=0.0,
    ),
}


@dataclass
class PoisonInfo:
    """Poison tick information"""
    level: PoisonLevel
    min_damage: int
    max_damage: int
    hp_scalar: float  # Percentage of max HP
    interval: float  # Seconds between ticks
    tick_count: int


# Poison database (AOS values)
POISONS = {
    PoisonLevel.Lesser: PoisonInfo(PoisonLevel.Lesser, 4, 16, 0.075, 2.25, 10),
    PoisonLevel.Regular: PoisonInfo(PoisonLevel.Regular, 8, 18, 0.10, 3.25, 10),
    PoisonLevel.Greater: PoisonInfo(PoisonLevel.Greater, 12, 20, 0.15, 4.25, 10),
    PoisonLevel.Deadly: PoisonInfo(PoisonLevel.Deadly, 16, 30, 0.30, 5.25, 15),
    PoisonLevel.Lethal: PoisonInfo(PoisonLevel.Lethal, 20, 50, 0.35, 5.25, 20),
}


@dataclass
class CombatStats:
    """Character combat statistics"""
    # Base stats
    hits: int = 100
    hits_max: int = 100
    mana: int = 100
    mana_max: int = 100
    stam: int = 100
    stam_max: int = 100

    # Attributes
    strength: int = 100
    dexterity: int = 100
    intelligence: int = 100

    # Skills (0-1200 fixed point, divide by 10 for display)
    magery: float = 100.0
    eval_int: float = 100.0
    resist: float = 100.0
    anatomy: float = 100.0
    healing: float = 100.0
    meditation: float = 100.0
    inscribe: float = 0.0

    # Equipment bonuses
    sdi: int = 0  # Spell Damage Increase
    fc: int = 0   # Faster Casting (cap 2 for magery)
    fcr: int = 0  # Faster Cast Recovery
    lmc: int = 0  # Lower Mana Cost (cap 40%)
    lrc: int = 0  # Lower Reagent Cost

    # Resistances
    resist_physical: int = 0
    resist_fire: int = 0
    resist_cold: int = 0
    resist_poison: int = 0
    resist_energy: int = 0

    # State
    poisoned: bool = False
    poison_level: Optional[PoisonLevel] = None
    poison_ticks_remaining: int = 0

    # Consumables
    bandages: int = 50
    heal_potions: int = 5
    greater_heal_potions: int = 5
    cure_potions: int = 5

    @property
    def hp_percent(self) -> float:
        return self.hits / self.hits_max if self.hits_max > 0 else 0

    @property
    def mana_percent(self) -> float:
        return self.mana / self.mana_max if self.mana_max > 0 else 0


class CombatCalculator:
    """Handles all combat calculations based on ServUO formulas"""

    @staticmethod
    def calculate_spell_damage(spell: SpellInfo, caster: CombatStats,
                                target: CombatStats, pvp: bool = True) -> int:
        """Calculate AOS spell damage"""
        # Base damage: bonus + dice
        damage = spell.base_damage * 100
        if spell.dice_count > 0:
            for _ in range(spell.dice_count):
                damage += random.randint(1, spell.dice_sides) * 100

        # Inscribe bonus (max 10% at GM)
        inscribe_fixed = int(caster.inscribe * 10)
        scribe_bonus = 10 if inscribe_fixed >= 1000 else inscribe_fixed // 200

        # Total damage bonus
        sdi_bonus = caster.sdi
        if pvp:
            sdi_bonus = min(sdi_bonus, 15)  # PvP cap

        damage_bonus = scribe_bonus + (caster.intelligence // 10) + sdi_bonus

        # Eval Int scaling: 30% base + 0.9% per skill point
        eval_fixed = int(caster.eval_int * 10)
        eval_scale = 30 + ((9 * eval_fixed) // 100)

        # Apply scaling
        damage = (damage * eval_scale) // 100
        damage = (damage * (100 + damage_bonus)) // 100

        # Apply resistance
        resist_value = CombatCalculator._get_resistance(target, spell.damage_type)
        damage = (damage * (100 - resist_value)) // 100

        return damage // 100

    @staticmethod
    def _get_resistance(target: CombatStats, damage_type: DamageType) -> int:
        """Get target's resistance to a damage type"""
        if damage_type == DamageType.Physical:
            return target.resist_physical
        elif damage_type == DamageType.Fire:
            return target.resist_fire
        elif damage_type == DamageType.Cold:
            return target.resist_cold
        elif damage_type == DamageType.Poison:
            return target.resist_poison
        elif damage_type == DamageType.Energy:
            return target.resist_energy
        return 0

    @staticmethod
    def calculate_resist_chance(spell: SpellInfo, caster: CombatStats,
                                 target: CombatStats) -> float:
        """Calculate chance to resist a spell (reduce damage by 25%)"""
        resist_skill = target.resist
        magery_skill = caster.magery
        circle = int(spell.circle)

        first_percent = resist_skill / 5.0
        second_percent = resist_skill - (((magery_skill - 20.0) / 5.0) + (1 + circle) * 5.0)

        return max(first_percent, second_percent) / 200.0  # Half of calculated value

    @staticmethod
    def calculate_cast_time(spell: SpellInfo, caster: CombatStats) -> float:
        """Calculate actual cast time with FC"""
        base_time = spell.cast_time_aos
        fc = min(caster.fc, 2)  # Cap at 2 for magery
        reduction = fc * 0.25
        return max(0.25, base_time - reduction)  # Minimum 0.25s

    @staticmethod
    def calculate_mana_cost(spell: SpellInfo, caster: CombatStats) -> int:
        """Calculate actual mana cost with LMC"""
        lmc = min(caster.lmc, 40)  # Cap at 40%
        return int(spell.mana_cost * (1.0 - lmc / 100.0))

    @staticmethod
    def calculate_bandage_heal(healer: CombatStats, self_heal: bool = True) -> Tuple[int, float]:
        """Calculate bandage heal amount and time"""
        anatomy = healer.anatomy
        healing = healer.healing
        dex = healer.dexterity

        # Heal amount (AOS)
        min_heal = (anatomy / 8.0) + (healing / 5.0) + 4.0
        max_heal = (anatomy / 6.0) + (healing / 2.5) + 4.0
        heal_amount = int(min_heal + random.random() * (max_heal - min_heal))

        # Heal time (AOS self-healing)
        if self_heal:
            seconds = max(4, 11.0 - dex / 20.0)
        else:
            seconds = max(2, 4 - dex / 60.0)

        return heal_amount, seconds

    @staticmethod
    def calculate_potion_heal(potion_type: str, caster: CombatStats) -> int:
        """Calculate potion heal amount"""
        # Base amounts
        amounts = {
            "lesser": (6, 8),
            "regular": (13, 16),
            "greater": (20, 25),
        }

        min_hp, max_hp = amounts.get(potion_type, (13, 16))
        base_heal = random.randint(min_hp, max_hp)

        # Enhancement from alchemy (simplified)
        # Full formula: scalar = 1.0 + (EnhancePotions + Alchemy/33) / 100
        # We'll skip enhance potions for now
        return base_heal

    @staticmethod
    def calculate_spell_heal(spell_name: str, caster: CombatStats,
                             target_is_self: bool = True) -> int:
        """Calculate spell heal amount"""
        magery = caster.magery

        if spell_name == "heal":
            heal = (magery / 12.0) + random.randint(1, 4)
            if not target_is_self:
                heal *= 1.5
            return int(heal)
        elif spell_name == "greater_heal":
            return int((magery * 0.4) + random.randint(1, 10))
        return 0

    @staticmethod
    def calculate_cure_chance(caster: CombatStats, poison_level: PoisonLevel) -> float:
        """Calculate chance to cure poison with Cure spell"""
        magery = caster.magery * 10  # Fixed point
        level = int(poison_level)

        # Formula: (10000 + (Magery * 75) - ((Level + 1) * 3300)) / 10000
        chance = (10000 + (magery * 7.5) - ((level + 1) * 3300)) / 10000
        return max(0.0, min(1.0, chance))

    @staticmethod
    def calculate_poison_damage(target: CombatStats) -> int:
        """Calculate poison tick damage"""
        if not target.poisoned or target.poison_level is None:
            return 0

        poison = POISONS[target.poison_level]

        # Damage: 1 + (HP * scalar)
        damage = 1 + int(target.hits_max * poison.hp_scalar)
        damage = max(poison.min_damage, min(poison.max_damage, damage))

        return damage

    @staticmethod
    def get_potion_cooldowns() -> dict:
        """Return potion cooldown times in seconds"""
        return {
            "lesser_heal": 3.0,
            "heal": 8.0,
            "greater_heal": 10.0,
            "cure": 0.0,  # No cooldown for cure
        }


class CombatState:
    """Tracks combat state for a single combatant"""

    def __init__(self, stats: CombatStats):
        self.stats = stats

        # Timing state
        self.casting_spell: Optional[str] = None
        self.cast_end_time: float = 0.0
        self.spell_recovery_time: float = 0.0
        self.bandage_end_time: float = 0.0
        self.potion_cooldown_time: float = 0.0

        # Position
        self.x: int = 0
        self.y: int = 0
        self.z: int = 0

        # Combat tracking
        self.damage_dealt: int = 0
        self.damage_taken: int = 0
        self.spells_cast: int = 0
        self.heals_done: int = 0

    @property
    def is_casting(self) -> bool:
        return self.casting_spell is not None

    @property
    def is_bandaging(self) -> bool:
        return self.bandage_end_time > 0

    @property
    def position(self) -> Tuple[int, int, int]:
        return (self.x, self.y, self.z)

    def set_position(self, x: int, y: int, z: int):
        self.x = x
        self.y = y
        self.z = z

    def distance_to(self, other: 'CombatState') -> float:
        """Calculate distance to another combatant"""
        dx = self.x - other.x
        dy = self.y - other.y
        return (dx * dx + dy * dy) ** 0.5


if __name__ == "__main__":
    # Quick test of calculations
    caster = CombatStats(
        magery=100.0,
        eval_int=100.0,
        intelligence=100,
        sdi=0,
        fc=2,
    )

    target = CombatStats(
        resist=50.0,
        resist_fire=50,
        resist_energy=50,
    )

    print("=== Combat Mechanics Test ===\n")

    for spell_name, spell in SPELLS.items():
        if spell.base_damage > 0:  # Only damage spells
            damage = CombatCalculator.calculate_spell_damage(spell, caster, target, pvp=True)
            cast_time = CombatCalculator.calculate_cast_time(spell, caster)
            mana_cost = CombatCalculator.calculate_mana_cost(spell, caster)

            print(f"{spell.name}:")
            print(f"  Damage: ~{damage} (vs 50 resist)")
            print(f"  Cast time: {cast_time:.2f}s (with FC 2)")
            print(f"  Mana: {mana_cost}")
            print()

    # Test healing
    print("=== Healing Test ===\n")
    healer = CombatStats(anatomy=100.0, healing=100.0, dexterity=100)
    heal_amount, heal_time = CombatCalculator.calculate_bandage_heal(healer)
    print(f"Bandage self-heal: ~{heal_amount} HP in {heal_time:.1f}s")

    gh_heal = CombatCalculator.calculate_spell_heal("greater_heal", caster)
    print(f"Greater Heal: ~{gh_heal} HP")
