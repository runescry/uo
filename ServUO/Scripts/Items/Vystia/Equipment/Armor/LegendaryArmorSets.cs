using System;
using Server;

namespace Server.Items.Vystia
{
    // ============================================================================
    // LEGENDARY ARMOR SETS
    // Complete sets with powerful bonuses
    // ============================================================================

    // ============================================================================
    // GLACIAL AEGIS
    // Type: Plate Armor
    // Hue: 1152
    // ============================================================================

    public class GlacialAegisHelm : PlateHelm
    {
        [Constructable]
        public GlacialAegisHelm()
        {
            Name = "Glacial Aegis Helm";
            Hue = 1152;

            // Legendary stats
            SkillBonuses.SetValues(0, SkillName.Parry, 5.0);

            Attributes.BonusStr = 3;
            Attributes.DefendChance = 10;
            Attributes.ReflectPhysical = 5;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Forged from eternal ice by the Frost Father");
            list.Add("Set: Glacial Aegis (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public GlacialAegisHelm(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GlacialAegisGorget : PlateGorget
    {
        [Constructable]
        public GlacialAegisGorget()
        {
            Name = "Glacial Aegis Gorget";
            Hue = 1152;

            // Legendary stats
            SkillBonuses.SetValues(0, SkillName.Parry, 5.0);

            Attributes.BonusStr = 3;
            Attributes.DefendChance = 10;
            Attributes.ReflectPhysical = 5;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Forged from eternal ice by the Frost Father");
            list.Add("Set: Glacial Aegis (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public GlacialAegisGorget(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GlacialAegisChest : PlateChest
    {
        [Constructable]
        public GlacialAegisChest()
        {
            Name = "Glacial Aegis Chest";
            Hue = 1152;

            // Legendary stats
            SkillBonuses.SetValues(0, SkillName.Parry, 5.0);

            Attributes.BonusStr = 3;
            Attributes.DefendChance = 10;
            Attributes.ReflectPhysical = 5;

            BaseArmorRating = 54;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Forged from eternal ice by the Frost Father");
            list.Add("Set: Glacial Aegis (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public GlacialAegisChest(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GlacialAegisArms : PlateArms
    {
        [Constructable]
        public GlacialAegisArms()
        {
            Name = "Glacial Aegis Arms";
            Hue = 1152;

            // Legendary stats
            SkillBonuses.SetValues(0, SkillName.Parry, 5.0);

            Attributes.BonusStr = 3;
            Attributes.DefendChance = 10;
            Attributes.ReflectPhysical = 5;

            BaseArmorRating = 56;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Forged from eternal ice by the Frost Father");
            list.Add("Set: Glacial Aegis (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public GlacialAegisArms(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GlacialAegisGloves : PlateGloves
    {
        [Constructable]
        public GlacialAegisGloves()
        {
            Name = "Glacial Aegis Gloves";
            Hue = 1152;

            // Legendary stats
            SkillBonuses.SetValues(0, SkillName.Parry, 5.0);

            Attributes.BonusStr = 3;
            Attributes.DefendChance = 10;
            Attributes.ReflectPhysical = 5;

            BaseArmorRating = 58;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Forged from eternal ice by the Frost Father");
            list.Add("Set: Glacial Aegis (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public GlacialAegisGloves(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GlacialAegisLegs : PlateLegs
    {
        [Constructable]
        public GlacialAegisLegs()
        {
            Name = "Glacial Aegis Legs";
            Hue = 1152;

            // Legendary stats
            SkillBonuses.SetValues(0, SkillName.Parry, 5.0);

            Attributes.BonusStr = 3;
            Attributes.DefendChance = 10;
            Attributes.ReflectPhysical = 5;

            BaseArmorRating = 60;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Forged from eternal ice by the Frost Father");
            list.Add("Set: Glacial Aegis (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public GlacialAegisLegs(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    // ============================================================================
    // STEAMWORK EXOSUIT
    // Type: Plate Armor
    // Hue: 2401
    // ============================================================================

    public class SteamworkExosuitHelm : PlateHelm
    {
        [Constructable]
        public SteamworkExosuitHelm()
        {
            Name = "Steamwork Exosuit Helm";
            Hue = 2401;

            // Legendary stats
            SkillBonuses.SetValues(0, SkillName.Blacksmith, 5.0);

            Attributes.BonusStr = 5;
            Attributes.BonusDex = -2;
            Attributes.WeaponSpeed = 10;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Mechanized armor of the Clockwork Titan");
            list.Add("Set: Steamwork Exosuit (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public SteamworkExosuitHelm(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class SteamworkExosuitGorget : PlateGorget
    {
        [Constructable]
        public SteamworkExosuitGorget()
        {
            Name = "Steamwork Exosuit Gorget";
            Hue = 2401;

            // Legendary stats
            SkillBonuses.SetValues(0, SkillName.Blacksmith, 5.0);

            Attributes.BonusStr = 5;
            Attributes.BonusDex = -2;
            Attributes.WeaponSpeed = 10;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Mechanized armor of the Clockwork Titan");
            list.Add("Set: Steamwork Exosuit (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public SteamworkExosuitGorget(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class SteamworkExosuitChest : PlateChest
    {
        [Constructable]
        public SteamworkExosuitChest()
        {
            Name = "Steamwork Exosuit Chest";
            Hue = 2401;

            // Legendary stats
            SkillBonuses.SetValues(0, SkillName.Blacksmith, 5.0);

            Attributes.BonusStr = 5;
            Attributes.BonusDex = -2;
            Attributes.WeaponSpeed = 10;

            BaseArmorRating = 54;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Mechanized armor of the Clockwork Titan");
            list.Add("Set: Steamwork Exosuit (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public SteamworkExosuitChest(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class SteamworkExosuitArms : PlateArms
    {
        [Constructable]
        public SteamworkExosuitArms()
        {
            Name = "Steamwork Exosuit Arms";
            Hue = 2401;

            // Legendary stats
            SkillBonuses.SetValues(0, SkillName.Blacksmith, 5.0);

            Attributes.BonusStr = 5;
            Attributes.BonusDex = -2;
            Attributes.WeaponSpeed = 10;

            BaseArmorRating = 56;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Mechanized armor of the Clockwork Titan");
            list.Add("Set: Steamwork Exosuit (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public SteamworkExosuitArms(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class SteamworkExosuitGloves : PlateGloves
    {
        [Constructable]
        public SteamworkExosuitGloves()
        {
            Name = "Steamwork Exosuit Gloves";
            Hue = 2401;

            // Legendary stats
            SkillBonuses.SetValues(0, SkillName.Blacksmith, 5.0);

            Attributes.BonusStr = 5;
            Attributes.BonusDex = -2;
            Attributes.WeaponSpeed = 10;

            BaseArmorRating = 58;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Mechanized armor of the Clockwork Titan");
            list.Add("Set: Steamwork Exosuit (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public SteamworkExosuitGloves(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class SteamworkExosuitLegs : PlateLegs
    {
        [Constructable]
        public SteamworkExosuitLegs()
        {
            Name = "Steamwork Exosuit Legs";
            Hue = 2401;

            // Legendary stats
            SkillBonuses.SetValues(0, SkillName.Blacksmith, 5.0);

            Attributes.BonusStr = 5;
            Attributes.BonusDex = -2;
            Attributes.WeaponSpeed = 10;

            BaseArmorRating = 60;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Mechanized armor of the Clockwork Titan");
            list.Add("Set: Steamwork Exosuit (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public SteamworkExosuitLegs(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    // ============================================================================
    // SHADOW SHROUD
    // Type: Leather Armor
    // Hue: 1109
    // ============================================================================

    public class ShadowShroudCap : LeatherCap
    {
        [Constructable]
        public ShadowShroudCap()
        {
            Name = "Shadow Shroud Cap";
            Hue = 1109;

            // Legendary stats
            SkillBonuses.SetValues(0, SkillName.Stealth, 5.0);
            SkillBonuses.SetValues(1, SkillName.Hiding, 5.0);

            Attributes.BonusDex = 5;
            Attributes.NightSight = 1;
            Attributes.RegenStam = 2;

            BaseArmorRating = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Shadowweave armor of the Shadow Master");
            list.Add("Set: Shadow Shroud (1/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ShadowShroudCap(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class ShadowShroudGorget : LeatherGorget
    {
        [Constructable]
        public ShadowShroudGorget()
        {
            Name = "Shadow Shroud Gorget";
            Hue = 1109;

            // Legendary stats
            SkillBonuses.SetValues(0, SkillName.Stealth, 5.0);
            SkillBonuses.SetValues(1, SkillName.Hiding, 5.0);

            Attributes.BonusDex = 5;
            Attributes.NightSight = 1;
            Attributes.RegenStam = 2;

            BaseArmorRating = 52;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Shadowweave armor of the Shadow Master");
            list.Add("Set: Shadow Shroud (2/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ShadowShroudGorget(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class ShadowShroudTunic : LeatherChest
    {
        [Constructable]
        public ShadowShroudTunic()
        {
            Name = "Shadow Shroud Tunic";
            Hue = 1109;

            // Legendary stats
            SkillBonuses.SetValues(0, SkillName.Stealth, 5.0);
            SkillBonuses.SetValues(1, SkillName.Hiding, 5.0);

            Attributes.BonusDex = 5;
            Attributes.NightSight = 1;
            Attributes.RegenStam = 2;

            BaseArmorRating = 54;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Shadowweave armor of the Shadow Master");
            list.Add("Set: Shadow Shroud (3/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ShadowShroudTunic(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class ShadowShroudSleeves : LeatherArms
    {
        [Constructable]
        public ShadowShroudSleeves()
        {
            Name = "Shadow Shroud Sleeves";
            Hue = 1109;

            // Legendary stats
            SkillBonuses.SetValues(0, SkillName.Stealth, 5.0);
            SkillBonuses.SetValues(1, SkillName.Hiding, 5.0);

            Attributes.BonusDex = 5;
            Attributes.NightSight = 1;
            Attributes.RegenStam = 2;

            BaseArmorRating = 56;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Shadowweave armor of the Shadow Master");
            list.Add("Set: Shadow Shroud (4/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ShadowShroudSleeves(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class ShadowShroudGloves : LeatherGloves
    {
        [Constructable]
        public ShadowShroudGloves()
        {
            Name = "Shadow Shroud Gloves";
            Hue = 1109;

            // Legendary stats
            SkillBonuses.SetValues(0, SkillName.Stealth, 5.0);
            SkillBonuses.SetValues(1, SkillName.Hiding, 5.0);

            Attributes.BonusDex = 5;
            Attributes.NightSight = 1;
            Attributes.RegenStam = 2;

            BaseArmorRating = 58;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Shadowweave armor of the Shadow Master");
            list.Add("Set: Shadow Shroud (5/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ShadowShroudGloves(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class ShadowShroudLegs : LeatherLegs
    {
        [Constructable]
        public ShadowShroudLegs()
        {
            Name = "Shadow Shroud Legs";
            Hue = 1109;

            // Legendary stats
            SkillBonuses.SetValues(0, SkillName.Stealth, 5.0);
            SkillBonuses.SetValues(1, SkillName.Hiding, 5.0);

            Attributes.BonusDex = 5;
            Attributes.NightSight = 1;
            Attributes.RegenStam = 2;

            BaseArmorRating = 60;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Shadowweave armor of the Shadow Master");
            list.Add("Set: Shadow Shroud (6/6)");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ShadowShroudLegs(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

}
