/*
 * Runecrafting Items - Crafted by Enchanters
 * Uses Runeweaving skill (ID 68)
 * Materials: Enchanting Magic reagents
 */

using System;
using Server;
using Server.Mobiles;
using Server.Spells;
using Server.Targeting;
using Server.Network;
using Server.Engines.Craft;

namespace Server.Items.Vystia
{
    #region Base Rune Class

    /// <summary>
    /// Base class for consumable runes that apply temporary buffs
    /// </summary>
    public abstract class BaseVystiaRune : Item
    {
        private int m_Charges;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set { m_Charges = value; InvalidateProperties(); }
        }

        public abstract string RuneName { get; }
        public abstract int BuffDuration { get; } // in seconds
        public abstract void ApplyEffect(Mobile target);

        public BaseVystiaRune(int itemID, int hue, int charges) : base(itemID)
        {
            Hue = hue;
            Weight = 1.0;
            m_Charges = charges;
            Stackable = false;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060741, m_Charges.ToString()); // charges: ~1_val~
            list.Add(1070722, $"Duration: {BuffDuration / 60} minutes");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack
                return;
            }

            if (m_Charges <= 0)
            {
                from.SendMessage(0x22, "This rune has no charges remaining.");
                Delete();
                return;
            }

            from.SendMessage(0x3B2, $"Select a target for the {RuneName}...");
            from.Target = new RuneTarget(this);
        }

        public void UseCharge(Mobile from, Mobile target)
        {
            m_Charges--;

            from.PlaySound(0x1F5); // Magic sound
            Effects.SendTargetParticles(target, 0x376A, 9, 32, Hue, 0, 5005, EffectLayer.Waist, 0);

            ApplyEffect(target);

            from.SendMessage(0x3B2, $"You activate the {RuneName}!");

            if (m_Charges <= 0)
            {
                from.SendMessage(0x22, "The rune crumbles to dust.");
                Delete();
            }
        }

        public BaseVystiaRune(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.Write(m_Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Charges = reader.ReadInt();
        }

        private class RuneTarget : Target
        {
            private BaseVystiaRune m_Rune;

            public RuneTarget(BaseVystiaRune rune) : base(12, false, TargetFlags.Beneficial)
            {
                m_Rune = rune;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Rune.Deleted || m_Rune.Charges <= 0)
                    return;

                if (targeted is Mobile target)
                {
                    if (target == from || (target is PlayerMobile && from.CanBeBeneficial(target)))
                    {
                        m_Rune.UseCharge(from, target);
                    }
                    else
                    {
                        from.SendMessage(0x22, "You cannot use this rune on that target.");
                    }
                }
                else
                {
                    from.SendMessage(0x22, "You must target a creature.");
                }
            }
        }
    }

    #endregion

    #region Basic Runes (Skill 0-50)

    /// <summary>
    /// Rune of Strength - +5 STR for 10 minutes
    /// </summary>
    public class RuneOfStrength : BaseVystiaRune
    {
        public override string RuneName => "Rune of Strength";
        public override int BuffDuration => 600; // 10 minutes

        [Constructable]
        public RuneOfStrength() : this(5) { }

        [Constructable]
        public RuneOfStrength(int charges) : base(0x1F14, 1161, charges) // Red hue
        {
            Name = "Rune of Strength";
        }

        public override void ApplyEffect(Mobile target)
        {
            target.AddStatMod(new StatMod(StatType.Str, "RuneStr", 5, TimeSpan.FromSeconds(BuffDuration)));
            target.SendMessage(0x3B2, "You feel a surge of strength!");
        }

        public RuneOfStrength(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Rune of Dexterity - +5 DEX for 10 minutes
    /// </summary>
    public class RuneOfDexterity : BaseVystiaRune
    {
        public override string RuneName => "Rune of Dexterity";
        public override int BuffDuration => 600;

        [Constructable]
        public RuneOfDexterity() : this(5) { }

        [Constructable]
        public RuneOfDexterity(int charges) : base(0x1F14, 1266, charges) // Green hue
        {
            Name = "Rune of Dexterity";
        }

        public override void ApplyEffect(Mobile target)
        {
            target.AddStatMod(new StatMod(StatType.Dex, "RuneDex", 5, TimeSpan.FromSeconds(BuffDuration)));
            target.SendMessage(0x3B2, "You feel more agile!");
        }

        public RuneOfDexterity(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Rune of Intelligence - +5 INT for 10 minutes
    /// </summary>
    public class RuneOfIntelligence : BaseVystiaRune
    {
        public override string RuneName => "Rune of Intelligence";
        public override int BuffDuration => 600;

        [Constructable]
        public RuneOfIntelligence() : this(5) { }

        [Constructable]
        public RuneOfIntelligence(int charges) : base(0x1F14, 1152, charges) // Blue hue
        {
            Name = "Rune of Intelligence";
        }

        public override void ApplyEffect(Mobile target)
        {
            target.AddStatMod(new StatMod(StatType.Int, "RuneInt", 5, TimeSpan.FromSeconds(BuffDuration)));
            target.SendMessage(0x3B2, "Your mind feels sharper!");
        }

        public RuneOfIntelligence(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Intermediate Runes (Skill 40-70)

    /// <summary>
    /// Rune of Fortitude - +15 HP for 15 minutes
    /// </summary>
    public class RuneOfFortitude : BaseVystiaRune
    {
        public override string RuneName => "Rune of Fortitude";
        public override int BuffDuration => 900;

        [Constructable]
        public RuneOfFortitude() : this(5) { }

        [Constructable]
        public RuneOfFortitude(int charges) : base(0x1F14, 1175, charges) // Orange hue
        {
            Name = "Rune of Fortitude";
        }

        public override void ApplyEffect(Mobile target)
        {
            // Fortitude gives +3 STR which translates to HP bonus
            target.AddStatMod(new StatMod(StatType.Str, "RuneFort", 3, TimeSpan.FromSeconds(BuffDuration)));
            target.SendMessage(0x3B2, "You feel more resilient!");
        }

        public RuneOfFortitude(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Rune of Swiftness - +5 DEX (attack speed) for 10 minutes
    /// </summary>
    public class RuneOfSwiftness : BaseVystiaRune
    {
        public override string RuneName => "Rune of Swiftness";
        public override int BuffDuration => 600;

        [Constructable]
        public RuneOfSwiftness() : this(5) { }

        [Constructable]
        public RuneOfSwiftness(int charges) : base(0x1F14, 1367, charges) // Cyan hue
        {
            Name = "Rune of Swiftness";
        }

        public override void ApplyEffect(Mobile target)
        {
            target.AddStatMod(new StatMod(StatType.Dex, "RuneSwift", 10, TimeSpan.FromSeconds(BuffDuration)));
            target.SendMessage(0x3B2, "Your movements quicken!");
        }

        public RuneOfSwiftness(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Rune of Warding - Resist bonus for 10 minutes
    /// </summary>
    public class RuneOfWarding : BaseVystiaRune
    {
        public override string RuneName => "Rune of Warding";
        public override int BuffDuration => 600;

        [Constructable]
        public RuneOfWarding() : this(5) { }

        [Constructable]
        public RuneOfWarding(int charges) : base(0x1F14, 1153, charges) // Purple hue
        {
            Name = "Rune of Warding";
        }

        public override void ApplyEffect(Mobile target)
        {
            // Add temporary magic resist bonus via INT (affects magic defense)
            target.AddStatMod(new StatMod(StatType.Int, "RuneWard", 8, TimeSpan.FromSeconds(BuffDuration)));
            target.SendMessage(0x3B2, "A protective ward surrounds you!");
        }

        public RuneOfWarding(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Advanced Runes (Skill 60-90)

    /// <summary>
    /// Rune of Power - +8 all stats for 10 minutes
    /// </summary>
    public class RuneOfPower : BaseVystiaRune
    {
        public override string RuneName => "Rune of Power";
        public override int BuffDuration => 600;

        [Constructable]
        public RuneOfPower() : this(3) { }

        [Constructable]
        public RuneOfPower(int charges) : base(0x1F14, 1160, charges) // Gold hue
        {
            Name = "Rune of Power";
        }

        public override void ApplyEffect(Mobile target)
        {
            target.AddStatMod(new StatMod(StatType.Str, "RunePowStr", 8, TimeSpan.FromSeconds(BuffDuration)));
            target.AddStatMod(new StatMod(StatType.Dex, "RunePowDex", 8, TimeSpan.FromSeconds(BuffDuration)));
            target.AddStatMod(new StatMod(StatType.Int, "RunePowInt", 8, TimeSpan.FromSeconds(BuffDuration)));
            target.SendMessage(0x3B2, "Raw power surges through you!");
        }

        public RuneOfPower(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Rune of Regeneration - Health regen for 5 minutes
    /// </summary>
    public class RuneOfRegeneration : BaseVystiaRune
    {
        public override string RuneName => "Rune of Regeneration";
        public override int BuffDuration => 300;

        [Constructable]
        public RuneOfRegeneration() : this(3) { }

        [Constructable]
        public RuneOfRegeneration(int charges) : base(0x1F14, 1272, charges) // Bright green
        {
            Name = "Rune of Regeneration";
        }

        public override void ApplyEffect(Mobile target)
        {
            // STR bonus provides HP regen in ServUO
            target.AddStatMod(new StatMod(StatType.Str, "RuneRegen", 10, TimeSpan.FromSeconds(BuffDuration)));
            target.SendMessage(0x3B2, "Your wounds begin to heal rapidly!");
        }

        public RuneOfRegeneration(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Rune of Focus - Mana regen for 5 minutes
    /// </summary>
    public class RuneOfFocus : BaseVystiaRune
    {
        public override string RuneName => "Rune of Focus";
        public override int BuffDuration => 300;

        [Constructable]
        public RuneOfFocus() : this(3) { }

        [Constructable]
        public RuneOfFocus(int charges) : base(0x1F14, 1154, charges) // Light blue
        {
            Name = "Rune of Focus";
        }

        public override void ApplyEffect(Mobile target)
        {
            target.AddStatMod(new StatMod(StatType.Int, "RuneFocus", 15, TimeSpan.FromSeconds(BuffDuration)));
            target.SendMessage(0x3B2, "Your mind becomes clear and focused!");
        }

        public RuneOfFocus(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Greater Runes (Skill 80-100)

    /// <summary>
    /// Greater Rune of Might - +15 STR for 20 minutes
    /// </summary>
    public class GreaterRuneOfMight : BaseVystiaRune
    {
        public override string RuneName => "Greater Rune of Might";
        public override int BuffDuration => 1200;

        [Constructable]
        public GreaterRuneOfMight() : this(3) { }

        [Constructable]
        public GreaterRuneOfMight(int charges) : base(0x1F15, 1161, charges) // Larger rune, red
        {
            Name = "Greater Rune of Might";
        }

        public override void ApplyEffect(Mobile target)
        {
            target.AddStatMod(new StatMod(StatType.Str, "GRuneMight", 15, TimeSpan.FromSeconds(BuffDuration)));
            target.SendMessage(0x3B2, "Tremendous power flows through your muscles!");
        }

        public GreaterRuneOfMight(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Greater Rune of Grace - +15 DEX for 20 minutes
    /// </summary>
    public class GreaterRuneOfGrace : BaseVystiaRune
    {
        public override string RuneName => "Greater Rune of Grace";
        public override int BuffDuration => 1200;

        [Constructable]
        public GreaterRuneOfGrace() : this(3) { }

        [Constructable]
        public GreaterRuneOfGrace(int charges) : base(0x1F15, 1266, charges) // Green
        {
            Name = "Greater Rune of Grace";
        }

        public override void ApplyEffect(Mobile target)
        {
            target.AddStatMod(new StatMod(StatType.Dex, "GRuneGrace", 15, TimeSpan.FromSeconds(BuffDuration)));
            target.SendMessage(0x3B2, "Your reflexes become lightning fast!");
        }

        public GreaterRuneOfGrace(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Greater Rune of Wisdom - +15 INT for 20 minutes
    /// </summary>
    public class GreaterRuneOfWisdom : BaseVystiaRune
    {
        public override string RuneName => "Greater Rune of Wisdom";
        public override int BuffDuration => 1200;

        [Constructable]
        public GreaterRuneOfWisdom() : this(3) { }

        [Constructable]
        public GreaterRuneOfWisdom(int charges) : base(0x1F15, 1152, charges) // Blue
        {
            Name = "Greater Rune of Wisdom";
        }

        public override void ApplyEffect(Mobile target)
        {
            target.AddStatMod(new StatMod(StatType.Int, "GRuneWisdom", 15, TimeSpan.FromSeconds(BuffDuration)));
            target.SendMessage(0x3B2, "Ancient wisdom fills your mind!");
        }

        public GreaterRuneOfWisdom(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Greater Rune of Protection - All resist bonus for 15 minutes
    /// </summary>
    public class GreaterRuneOfProtection : BaseVystiaRune
    {
        public override string RuneName => "Greater Rune of Protection";
        public override int BuffDuration => 900;

        [Constructable]
        public GreaterRuneOfProtection() : this(3) { }

        [Constructable]
        public GreaterRuneOfProtection(int charges) : base(0x1F15, 1153, charges) // Purple
        {
            Name = "Greater Rune of Protection";
        }

        public override void ApplyEffect(Mobile target)
        {
            // All stats boost for comprehensive protection
            target.AddStatMod(new StatMod(StatType.Str, "GRuneProtStr", 5, TimeSpan.FromSeconds(BuffDuration)));
            target.AddStatMod(new StatMod(StatType.Dex, "GRuneProtDex", 5, TimeSpan.FromSeconds(BuffDuration)));
            target.AddStatMod(new StatMod(StatType.Int, "GRuneProtInt", 10, TimeSpan.FromSeconds(BuffDuration)));
            target.SendMessage(0x3B2, "A powerful ward of protection envelops you!");
        }

        public GreaterRuneOfProtection(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Legendary Runes (Skill 95-100)

    /// <summary>
    /// Titan Rune of Ascendancy - +20 all stats for 30 minutes
    /// </summary>
    public class TitanRuneOfAscendancy : BaseVystiaRune
    {
        public override string RuneName => "Titan Rune of Ascendancy";
        public override int BuffDuration => 1800;

        [Constructable]
        public TitanRuneOfAscendancy() : this(1) { }

        [Constructable]
        public TitanRuneOfAscendancy(int charges) : base(0x1F16, 1159, charges) // Legendary graphic, white/gold
        {
            Name = "Titan Rune of Ascendancy";
            LootType = LootType.Blessed;
        }

        public override void ApplyEffect(Mobile target)
        {
            target.AddStatMod(new StatMod(StatType.Str, "TitanStr", 20, TimeSpan.FromSeconds(BuffDuration)));
            target.AddStatMod(new StatMod(StatType.Dex, "TitanDex", 20, TimeSpan.FromSeconds(BuffDuration)));
            target.AddStatMod(new StatMod(StatType.Int, "TitanInt", 20, TimeSpan.FromSeconds(BuffDuration)));
            target.SendMessage(0x3B2, "The power of the Titans courses through your being!");
            target.FixedParticles(0x375A, 9, 20, 5016, EffectLayer.Waist);
        }

        public TitanRuneOfAscendancy(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Enchanted Talismans

    /// <summary>
    /// Protection Talisman - Permanent resist bonus when equipped
    /// </summary>
    public class ProtectionTalisman : BaseTalisman
    {
        [Constructable]
        public ProtectionTalisman() : base(0x2F5B)
        {
            Name = "Protection Talisman";
            Hue = 1153; // Purple
            Weight = 1.0;

            Attributes.DefendChance = 5;
            Attributes.BonusHits = 5;
        }

        public ProtectionTalisman(Serial serial) : base(serial) { }

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

    /// <summary>
    /// Arcane Talisman - Permanent mana bonus when equipped
    /// </summary>
    public class ArcaneTalisman : BaseTalisman
    {
        [Constructable]
        public ArcaneTalisman() : base(0x2F5B)
        {
            Name = "Arcane Talisman";
            Hue = 1152; // Blue
            Weight = 1.0;

            Attributes.BonusMana = 10;
            Attributes.RegenMana = 2;
            Attributes.LowerManaCost = 5;
        }

        public ArcaneTalisman(Serial serial) : base(serial) { }

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

    /// <summary>
    /// Warrior's Talisman - Combat bonuses when equipped
    /// </summary>
    public class WarriorsTalisman : BaseTalisman
    {
        [Constructable]
        public WarriorsTalisman() : base(0x2F5B)
        {
            Name = "Warrior's Talisman";
            Hue = 1161; // Red
            Weight = 1.0;

            Attributes.AttackChance = 5;
            Attributes.WeaponDamage = 10;
            Attributes.BonusStam = 10;
        }

        public WarriorsTalisman(Serial serial) : base(serial) { }

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

    /// <summary>
    /// Runic Talisman - Enhanced runeweaving bonuses
    /// </summary>
    public class RunicTalisman : BaseTalisman
    {
        [Constructable]
        public RunicTalisman() : base(0x2F5B)
        {
            Name = "Runic Talisman";
            Hue = 1159; // White/gold
            Weight = 1.0;

            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 5;
            Attributes.CastRecovery = 1;
        }

        public RunicTalisman(Serial serial) : base(serial) { }

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

    #endregion

    #region Runecrafting Tool

    /// <summary>
    /// Runecrafting Kit - Portable runecrafting station
    /// Uses SkillName.Runeweaving for crafting
    /// </summary>
    public class RunecraftingKit : Item, ITool
    {
        private int m_UsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; InvalidateProperties(); }
        }

        public bool ShowUsesRemaining
        {
            get { return true; }
            set { }
        }

        public CraftSystem CraftSystem => DefRunecrafting.CraftSystem;

        public bool BreakOnDepletion => true;

        [Constructable]
        public RunecraftingKit() : this(50)
        {
        }

        [Constructable]
        public RunecraftingKit(int uses) : base(0x0E34) // Rune bag graphic
        {
            Name = "Runecrafting Kit";
            Hue = 1153; // Purple - arcane hue
            Weight = 3.0;
            m_UsesRemaining = uses;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060584, m_UsesRemaining.ToString()); // uses remaining: ~1_val~
            list.Add(1070722, "Portable runecrafting station");
            list.Add("Uses Runeweaving skill");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack
                return;
            }

            if (m_UsesRemaining <= 0)
            {
                from.SendMessage(0x22, "This kit has been worn out.");
                return;
            }

            from.SendMessage(0x3B2, "You open your runecrafting kit...");
            from.SendGump(new CraftGump(from, CraftSystem, this, null));
        }

        public bool CheckAccessible(Mobile from, ref int num)
        {
            if (RootParent != from)
            {
                num = 1044263; // The tool must be on your person to use.
                return false;
            }

            return true;
        }

        public RunecraftingKit(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.Write(m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_UsesRemaining = reader.ReadInt();
        }
    }

    #endregion
}
