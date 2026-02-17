"""
Vystia Mage Combat Simulator
Simulates 1v1 mage battles to test spell balance
"""

import random
from dataclasses import dataclass, field
from typing import List, Dict, Optional
from mage_spell_data import Spell, MagicSchool

@dataclass
class Mage:
    """Represents a mage in combat"""
    name: str
    school: MagicSchool
    max_hp: int = 150
    hp: int = 150
    max_mana: int = 200
    mana: int = 200
    
    def __post_init__(self):
        self.hp = self.max_hp
        self.mana = self.max_mana

print("Mage combat simulator loaded")
