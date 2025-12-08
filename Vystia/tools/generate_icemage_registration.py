#!/usr/bin/env python3
"""
Generate Ice Mage spell registration code
"""

# Ice Mage spells in order (based on spell scrolls 1000-1031)
ICE_MAGE_SPELLS = [
    # Circle 1 (IDs 1000-1003)
    ("FrostTouchSpell", 1000),
    ("IceShardSpell", 1001),
    ("FrostWardSpell", 1002),
    ("ChillAuraSpell", 1003),

    # Circle 2 (IDs 1004-1007)
    ("FreezingGraspSpell", 1004),
    ("IceShieldSpell", 1005),
    ("FrostSlickSpell", 1006),
    ("GlacialMendSpell", 1007),

    # Circle 3 (IDs 1008-1011)
    ("IceBoltSpell", 1008),
    ("FrostbiteSpell", 1009),
    ("FrozenGroundSpell", 1010),
    ("IceWallSpell", 1011),

    # Circle 4 (IDs 1012-1015)
    ("FrostArmorSpell", 1012),
    ("IceSpearSpell", 1013),
    ("GlacialStrikeSpell", 1014),
    ("HypothermiaSpell", 1015),

    # Circle 5 (IDs 1016-1019)
    ("IcicleBarrageSpell", 1016),
    ("DeepFreezeSpell", 1017),
    ("FrozenTombSpell", 1018),
    ("PermafrostSpell", 1019),

    # Circle 6 (IDs 1020-1023)
    ("BlizzardSpell", 1020),
    ("GlacialFortressSpell", 1021),
    ("FrostMeteorSpell", 1022),
    ("ShatterSpell", 1023),

    # Circle 7 (IDs 1024-1027)
    ("AvalancheSpell", 1024),
    ("GlacierSummonSpell", 1025),
    ("IceAgeSpell", 1026),
    ("CocytusPrisonSpell", 1027),

    # Circle 8 (IDs 1028-1031)
    ("AbsoluteZeroSpell", 1028),
    ("EternalWinterSpell", 1029),
    ("RimeReaperSpell", 1030),
    ("FimbulwintersWrathSpell", 1031),
]

def generate_registration():
    """Generate the RegisterIceMageSpells() method"""

    lines = []
    lines.append("        private static void RegisterIceMageSpells()")
    lines.append("        {")
    lines.append("            // Ice Magic Spells (IDs 1000-1031)")

    for spell_name, spell_id in ICE_MAGE_SPELLS:
        lines.append(f"            Register({spell_id}, typeof(IceMage{spell_name}));")

    lines.append("        }")

    return "\n".join(lines)

if __name__ == "__main__":
    print(generate_registration())
