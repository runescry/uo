// Vystia Class System v2.0 - Class Focus Items
// Items that enhance class abilities through stat bonuses

using System;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Items.VystiaClassItems
{
    #region Base Class

    public abstract class ClassFocusItem : Item
    {
        private bool m_IsEquipped;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsEquipped
        {
            get { return m_IsEquipped; }
            set { m_IsEquipped = value; }
        }

        public abstract string ClassName { get; }
        public abstract string BonusDescription { get; }

        public ClassFocusItem(int itemID) : base(itemID)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
            Layer = Layer.TwoHanded;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1070722, $"{ClassName} Focus Item");
            list.Add(1042971, BonusDescription);
        }

        public override bool OnEquip(Mobile from)
        {
            if (from is PlayerMobile pm)
            {
                m_IsEquipped = true;
                ApplyBonus(pm);
                from.SendMessage(0x3B2, $"You feel the power of the {Name} enhancing your abilities.");
            }
            return base.OnEquip(from);
        }

        public override void OnRemoved(object parent)
        {
            if (parent is PlayerMobile pm)
            {
                m_IsEquipped = false;
                RemoveBonus(pm);
                pm.SendMessage(0x22, $"The {Name}'s enhancement fades.");
            }
            base.OnRemoved(parent);
        }

        public abstract void ApplyBonus(PlayerMobile pm);
        public abstract void RemoveBonus(PlayerMobile pm);

        public ClassFocusItem(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_IsEquipped);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_IsEquipped = reader.ReadBool();
        }
    }

    #endregion

    #region Magic Class Focus Items

    public class SoulGem : ClassFocusItem
    {
        public override string ClassName => "Warlock";
        public override string BonusDescription => "+5 INT, Enhanced Soul Shard generation";

        [Constructable]
        public SoulGem() : base(0x1F19)
        {
            Name = "Soul Gem";
            Hue = 1109;
        }

        public override void ApplyBonus(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Int, "SoulGem_Int", 5, TimeSpan.Zero));
        }

        public override void RemoveBonus(PlayerMobile pm)
        {
            pm.RemoveStatMod("SoulGem_Int");
        }

        public SoulGem(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class FrostCrystal : ClassFocusItem
    {
        public override string ClassName => "Ice Mage";
        public override string BonusDescription => "+5 INT, Extended Chill duration";

        [Constructable]
        public FrostCrystal() : base(0x1F19)
        {
            Name = "Frost Crystal";
            Hue = 1152;
        }

        public override void ApplyBonus(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Int, "FrostCrystal_Int", 5, TimeSpan.Zero));
        }

        public override void RemoveBonus(PlayerMobile pm)
        {
            pm.RemoveStatMod("FrostCrystal_Int");
        }

        public FrostCrystal(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class DeathsHourglass : ClassFocusItem
    {
        public override string ClassName => "Necromancer";
        public override string BonusDescription => "+5 INT, Enhanced Life Force capacity";

        [Constructable]
        public DeathsHourglass() : base(0x1810)
        {
            Name = "Death's Hourglass";
            Hue = 1175;
        }

        public override void ApplyBonus(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Int, "DeathsHourglass_Int", 5, TimeSpan.Zero));
        }

        public override void RemoveBonus(PlayerMobile pm)
        {
            pm.RemoveStatMod("DeathsHourglass_Int");
        }

        public DeathsHourglass(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class PrimalTotem : ClassFocusItem
    {
        public override string ClassName => "Druid";
        public override string BonusDescription => "+5 DEX, Instant form switching";

        [Constructable]
        public PrimalTotem() : base(0x1F0B)
        {
            Name = "Primal Totem";
            Hue = 2010;
        }

        public override void ApplyBonus(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Dex, "PrimalTotem_Dex", 5, TimeSpan.Zero));
        }

        public override void RemoveBonus(PlayerMobile pm)
        {
            pm.RemoveStatMod("PrimalTotem_Dex");
        }

        public PrimalTotem(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class ElementalOrb : ClassFocusItem
    {
        public override string ClassName => "Sorcerer";
        public override string BonusDescription => "+10 INT, +15% Elemental damage";

        [Constructable]
        public ElementalOrb() : base(0x1F19)
        {
            Name = "Elemental Orb";
            Hue = 1358;
        }

        public override void ApplyBonus(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Int, "ElementalOrb_Int", 10, TimeSpan.Zero));
        }

        public override void RemoveBonus(PlayerMobile pm)
        {
            pm.RemoveStatMod("ElementalOrb_Int");
        }

        public ElementalOrb(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class DragonLute : ClassFocusItem
    {
        public override string ClassName => "Bard";
        public override string BonusDescription => "+8 INT, +2 Crescendo per tick";

        [Constructable]
        public DragonLute() : base(0x0EB3)
        {
            Name = "Dragon Lute";
            Hue = 1161;
        }

        public override void ApplyBonus(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Int, "DragonLute_Int", 8, TimeSpan.Zero));
        }

        public override void RemoveBonus(PlayerMobile pm)
        {
            pm.RemoveStatMod("DragonLute_Int");
        }

        public DragonLute(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class SeersCrystal : ClassFocusItem
    {
        public override string ClassName => "Oracle";
        public override string BonusDescription => "+8 INT, +20% Divination effectiveness";

        [Constructable]
        public SeersCrystal() : base(0x1F19)
        {
            Name = "Seer's Crystal";
            Hue = 1154;
        }

        public override void ApplyBonus(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Int, "SeersCrystal_Int", 8, TimeSpan.Zero));
        }

        public override void RemoveBonus(PlayerMobile pm)
        {
            pm.RemoveStatMod("SeersCrystal_Int");
        }

        public SeersCrystal(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class SummonersSigil : ClassFocusItem
    {
        public override string ClassName => "Summoner";
        public override string BonusDescription => "+10 INT, +1 Summon slot";

        [Constructable]
        public SummonersSigil() : base(0x1F14)
        {
            Name = "Summoner's Sigil";
            Hue = 1365;
        }

        public override void ApplyBonus(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Int, "SummonersSigil_Int", 10, TimeSpan.Zero));
        }

        public override void RemoveBonus(PlayerMobile pm)
        {
            pm.RemoveStatMod("SummonersSigil_Int");
        }

        public SummonersSigil(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class SpiritFeather : ClassFocusItem
    {
        public override string ClassName => "Shaman";
        public override string BonusDescription => "+7 INT, +25% Totem effectiveness";

        [Constructable]
        public SpiritFeather() : base(0x1BD1)
        {
            Name = "Spirit Feather";
            Hue = 1281;
        }

        public override void ApplyBonus(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Int, "SpiritFeather_Int", 7, TimeSpan.Zero));
        }

        public override void RemoveBonus(PlayerMobile pm)
        {
            pm.RemoveStatMod("SpiritFeather_Int");
        }

        public SpiritFeather(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class HexDoll : ClassFocusItem
    {
        public override string ClassName => "Witch";
        public override string BonusDescription => "+6 INT, Curses last +50% longer";

        [Constructable]
        public HexDoll() : base(0x2D8F)
        {
            Name = "Hex Doll";
            Hue = 2073;
        }

        public override void ApplyBonus(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Int, "HexDoll_Int", 6, TimeSpan.Zero));
        }

        public override void RemoveBonus(PlayerMobile pm)
        {
            pm.RemoveStatMod("HexDoll_Int");
        }

        public HexDoll(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class RuneStone : ClassFocusItem
    {
        public override string ClassName => "Enchanter";
        public override string BonusDescription => "+8 INT, +20% Enchantment potency";

        [Constructable]
        public RuneStone() : base(0x1F14)
        {
            Name = "Rune Stone";
            Hue = 1153;
        }

        public override void ApplyBonus(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Int, "RuneStone_Int", 8, TimeSpan.Zero));
        }

        public override void RemoveBonus(PlayerMobile pm)
        {
            pm.RemoveStatMod("RuneStone_Int");
        }

        public RuneStone(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class MirrorShard : ClassFocusItem
    {
        public override string ClassName => "Illusionist";
        public override string BonusDescription => "+7 INT, Illusions last +30% longer";

        [Constructable]
        public MirrorShard() : base(0x1F19)
        {
            Name = "Mirror Shard";
            Hue = 1150;
        }

        public override void ApplyBonus(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Int, "MirrorShard_Int", 7, TimeSpan.Zero));
        }

        public override void RemoveBonus(PlayerMobile pm)
        {
            pm.RemoveStatMod("MirrorShard_Int");
        }

        public MirrorShard(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Martial Class Focus Items

    public class FuryIdol : ClassFocusItem
    {
        public override string ClassName => "Barbarian";
        public override string BonusDescription => "+5 STR, Fury decays 50% slower";

        [Constructable]
        public FuryIdol() : base(0x1F0B)
        {
            Name = "Fury Idol";
            Hue = 1157;
        }

        public override void ApplyBonus(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Str, "FuryIdol_Str", 5, TimeSpan.Zero));
        }

        public override void RemoveBonus(PlayerMobile pm)
        {
            pm.RemoveStatMod("FuryIdol_Str");
        }

        public FuryIdol(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class ShadowVeil : ClassFocusItem
    {
        public override string ClassName => "Rogue";
        public override string BonusDescription => "+8 DEX, +1 Combo Point on crit";

        [Constructable]
        public ShadowVeil() : base(0x1F03)
        {
            Name = "Shadow Veil";
            Hue = 1175;
        }

        public override void ApplyBonus(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Dex, "ShadowVeil_Dex", 8, TimeSpan.Zero));
        }

        public override void RemoveBonus(PlayerMobile pm)
        {
            pm.RemoveStatMod("ShadowVeil_Dex");
        }

        public ShadowVeil(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class ChiBeads : ClassFocusItem
    {
        public override string ClassName => "Monk";
        public override string BonusDescription => "+5 DEX, +5 STR, Enhanced Chi";

        [Constructable]
        public ChiBeads() : base(0x108B)
        {
            Name = "Chi Beads";
            Hue = 1161;
        }

        public override void ApplyBonus(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Dex, "ChiBeads_Dex", 5, TimeSpan.Zero));
            pm.AddStatMod(new StatMod(StatType.Str, "ChiBeads_Str", 5, TimeSpan.Zero));
        }

        public override void RemoveBonus(PlayerMobile pm)
        {
            pm.RemoveStatMod("ChiBeads_Dex");
            pm.RemoveStatMod("ChiBeads_Str");
        }

        public ChiBeads(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class WarBanner : ClassFocusItem
    {
        public override string ClassName => "Knight";
        public override string BonusDescription => "+5 STR, +3 Max Fortitude";

        [Constructable]
        public WarBanner() : base(0x0FF6)
        {
            Name = "War Banner";
            Hue = 2305;
        }

        public override void ApplyBonus(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Str, "WarBanner_Str", 5, TimeSpan.Zero));
        }

        public override void RemoveBonus(PlayerMobile pm)
        {
            pm.RemoveStatMod("WarBanner_Str");
        }

        public WarBanner(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class VirtuousRelic : ClassFocusItem
    {
        public override string ClassName => "Paladin";
        public override string BonusDescription => "+5 STR, +5 INT, -20% Tithing costs";

        [Constructable]
        public VirtuousRelic() : base(0x1F14)
        {
            Name = "Virtuous Relic";
            Hue = 1153;
        }

        public override void ApplyBonus(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Str, "VirtuousRelic_Str", 5, TimeSpan.Zero));
            pm.AddStatMod(new StatMod(StatType.Int, "VirtuousRelic_Int", 5, TimeSpan.Zero));
        }

        public override void RemoveBonus(PlayerMobile pm)
        {
            pm.RemoveStatMod("VirtuousRelic_Str");
            pm.RemoveStatMod("VirtuousRelic_Int");
        }

        public VirtuousRelic(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class HuntersMarkTotem : ClassFocusItem
    {
        public override string ClassName => "Ranger";
        public override string BonusDescription => "+5 DEX, +50% Focus regen";

        [Constructable]
        public HuntersMarkTotem() : base(0x1F0B)
        {
            Name = "Hunter's Mark Totem";
            Hue = 2010;
        }

        public override void ApplyBonus(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Dex, "HuntersMarkTotem_Dex", 5, TimeSpan.Zero));
        }

        public override void RemoveBonus(PlayerMobile pm)
        {
            pm.RemoveStatMod("HuntersMarkTotem_Dex");
        }

        public HuntersMarkTotem(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class TrackingStone : ClassFocusItem
    {
        public override string ClassName => "Bounty Hunter";
        public override string BonusDescription => "+5 DEX, +3 Max Pursuit";

        [Constructable]
        public TrackingStone() : base(0x1F14)
        {
            Name = "Tracking Stone";
            Hue = 1719;
        }

        public override void ApplyBonus(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Dex, "TrackingStone_Dex", 5, TimeSpan.Zero));
        }

        public override void RemoveBonus(PlayerMobile pm)
        {
            pm.RemoveStatMod("TrackingStone_Dex");
        }

        public TrackingStone(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class CombatManual : ClassFocusItem
    {
        public override string ClassName => "Fighter";
        public override string BonusDescription => "+8 STR, +4 DEX, +10% Weapon damage";

        [Constructable]
        public CombatManual() : base(0x0FF2)
        {
            Name = "Combat Manual";
            Hue = 2305;
        }

        public override void ApplyBonus(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Str, "CombatManual_Str", 8, TimeSpan.Zero));
            pm.AddStatMod(new StatMod(StatType.Dex, "CombatManual_Dex", 4, TimeSpan.Zero));
        }

        public override void RemoveBonus(PlayerMobile pm)
        {
            pm.RemoveStatMod("CombatManual_Str");
            pm.RemoveStatMod("CombatManual_Dex");
        }

        public CombatManual(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class ZealousIcon : ClassFocusItem
    {
        public override string ClassName => "Templar";
        public override string BonusDescription => "+5 STR, +3 Max Zeal";

        [Constructable]
        public ZealousIcon() : base(0x1F14)
        {
            Name = "Zealous Icon";
            Hue = 1153;
        }

        public override void ApplyBonus(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Str, "ZealousIcon_Str", 5, TimeSpan.Zero));
        }

        public override void RemoveBonus(PlayerMobile pm)
        {
            pm.RemoveStatMod("ZealousIcon_Str");
        }

        public ZealousIcon(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class BeastBond : ClassFocusItem
    {
        public override string ClassName => "Beastmaster";
        public override string BonusDescription => "+5 DEX, +5 INT, +20% Pet damage";

        [Constructable]
        public BeastBond() : base(0x1F03)
        {
            Name = "Beast Bond";
            Hue = 1150;
        }

        public override void ApplyBonus(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Dex, "BeastBond_Dex", 5, TimeSpan.Zero));
            pm.AddStatMod(new StatMod(StatType.Int, "BeastBond_Int", 5, TimeSpan.Zero));
        }

        public override void RemoveBonus(PlayerMobile pm)
        {
            pm.RemoveStatMod("BeastBond_Dex");
            pm.RemoveStatMod("BeastBond_Int");
        }

        public BeastBond(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class SteamCore : ClassFocusItem
    {
        public override string ClassName => "Artificer";
        public override string BonusDescription => "+5 INT, +25 Max Steam, +2 Max Charges";

        [Constructable]
        public SteamCore() : base(0x1F19)
        {
            Name = "Steam Core";
            Hue = 2305;
        }

        public override void ApplyBonus(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Int, "SteamCore_Int", 5, TimeSpan.Zero));
        }

        public override void RemoveBonus(PlayerMobile pm)
        {
            pm.RemoveStatMod("SteamCore_Int");
        }

        public SteamCore(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class PhilosophersStone : ClassFocusItem
    {
        public override string ClassName => "Alchemist";
        public override string BonusDescription => "+10 INT, +25% Potion effectiveness";

        [Constructable]
        public PhilosophersStone() : base(0x1F19)
        {
            Name = "Philosopher's Stone";
            Hue = 1161;
        }

        public override void ApplyBonus(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Int, "PhilosophersStone_Int", 10, TimeSpan.Zero));
        }

        public override void RemoveBonus(PlayerMobile pm)
        {
            pm.RemoveStatMod("PhilosophersStone_Int");
        }

        public PhilosophersStone(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class SacredCenser : ClassFocusItem
    {
        public override string ClassName => "Cleric";
        public override string BonusDescription => "+8 INT, +25% Faith generation";

        [Constructable]
        public SacredCenser() : base(0x1F19)
        {
            Name = "Sacred Censer";
            Hue = 1153;
        }

        public override void ApplyBonus(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Int, "SacredCenser_Int", 8, TimeSpan.Zero));
        }

        public override void RemoveBonus(PlayerMobile pm)
        {
            pm.RemoveStatMod("SacredCenser_Int");
        }

        public SacredCenser(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class ArcaneConduit : ClassFocusItem
    {
        public override string ClassName => "Wizard";
        public override string BonusDescription => "+12 INT, +15% Spell damage";

        [Constructable]
        public ArcaneConduit() : base(0x1F19)
        {
            Name = "Arcane Conduit";
            Hue = 1154;
        }

        public override void ApplyBonus(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Int, "ArcaneConduit_Int", 12, TimeSpan.Zero));
        }

        public override void RemoveBonus(PlayerMobile pm)
        {
            pm.RemoveStatMod("ArcaneConduit_Int");
        }

        public ArcaneConduit(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion
}
