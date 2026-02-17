using System;
using Server.Spells;

namespace Server.Items
{
    // ============================================
    // VYSTIA CUSTOM SPELLBOOKS (12 Magic Schools)
    // ============================================
    // Each magic school has a unique spellbook with:
    // - Custom item ID and hue
    // - 32 spell capacity (8 circles × 4 spells)
    // - Special bonuses when equipped
    // ============================================

    #region Ice Magic - Ice Mage Spellbook
    /// <summary>
    /// Tome of Frozen Arts - Ice Mage Spellbook (Frosthold)
    /// Grants +5% cold damage when equipped
    /// </summary>
    public class IceMageSpellbook : Spellbook
    {
        [Constructable]
        public IceMageSpellbook() : this(0xFFFFFFFF) // Fill with all 32 spells
        {
        }

        [Constructable]
        public IceMageSpellbook(ulong content) : base(content, 0x2252) // Chivalry book graphic (exists in client)
        {
            Name = "Tome of Frozen Arts";
            Hue = 0x481; // Ice Blue (makes it visually distinct from Chivalry)
            Weight = 3.0;
            Layer = Layer.OneHanded;
        }

        public IceMageSpellbook(Serial serial) : base(serial)
        {
        }

        public override SpellbookType SpellbookType => SpellbookType.VystiaIceMage;
        public override int BookOffset => 1000; // Spell IDs 1000-1031 (Fixed: was 999-1030)
        public override int BookCount => 32; // 8 circles × 4 spells

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
    #endregion

    #region Nature Magic - Druid Spellbook
    /// <summary>
    /// Codex of the Wild - Druid Spellbook (Verdantpeak)
    /// Grants +10% poison damage and +5 HP regen when equipped
    /// </summary>
    public class DruidSpellbook : Spellbook
    {
        [Constructable]
        public DruidSpellbook() : this(0xFFFFFFFF) // Fill with all 32 spells
        {
        }

        [Constructable]
        public DruidSpellbook(ulong content) : base(content, 0xEFA)
        {
            Name = "Codex of the Wild";
            Hue = 0x7D6; // Forest Green
            Weight = 3.0;
            Layer = Layer.OneHanded;
        }

        public DruidSpellbook(Serial serial) : base(serial)
        {
        }

        public override SpellbookType SpellbookType => SpellbookType.VystiaDruid;
        public override int BookOffset => 1032; // Spell IDs 1032-1063 (Fixed: was 1031-1062)
        public override int BookCount => 32;

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

    #region Hex Magic - Witch Spellbook
    /// <summary>
    /// Grimoire of Shadowfen Hexes - Witch Spellbook (Shadowfen)
    /// Hexes last 20% longer when equipped
    /// </summary>
    public class WitchSpellbook : Spellbook
    {
        [Constructable]
        public WitchSpellbook() : this(0xFFFFFFFF) // Fill with all 32 spells
        {
        }

        [Constructable]
        public WitchSpellbook(ulong content) : base(content, 0xFF0) // Necromancer book graphic
        {
            Name = "Grimoire of Shadowfen Hexes";
            Hue = 0x81D; // Murky Green/Purple
            Weight = 3.0;
            Layer = Layer.OneHanded;
        }

        public WitchSpellbook(Serial serial) : base(serial)
        {
        }

        public override SpellbookType SpellbookType => SpellbookType.VystiaWitch;
        public override int BookOffset => 1064; // Spell IDs 1064-1095 (Fixed: was 1063-1094)
        public override int BookCount => 32;

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

    #region Elemental Magic - Sorcerer Spellbook
    /// <summary>
    /// Tome of Elemental Fury - Sorcerer Spellbook (Emberlands)
    /// Grants +8% fire damage when equipped
    /// </summary>
    public class SorcererSpellbook : Spellbook
    {
        [Constructable]
        public SorcererSpellbook() : this(0xFFFFFFFF) // Fill with all 32 spells
        {
        }

        [Constructable]
        public SorcererSpellbook(ulong content) : base(content, 0xEFA)
        {
            Name = "Tome of Elemental Fury";
            Hue = 0x54E; // Fiery Orange
            Weight = 3.0;
            Layer = Layer.OneHanded;
        }

        public SorcererSpellbook(Serial serial) : base(serial)
        {
        }

        public override SpellbookType SpellbookType => SpellbookType.VystiaSorcerer;
        public override int BookOffset => 1096; // Spell IDs 1096-1127 (Fixed: was 1095-1126)
        public override int BookCount => 32;

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

    #region Dark Magic - Warlock Spellbook
    /// <summary>
    /// Codex of Shadows - Warlock Spellbook (ShadowVoid)
    /// Soul shard gain +25% when equipped
    /// </summary>
    public class WarlockSpellbook : Spellbook
    {
        [Constructable]
        public WarlockSpellbook() : this(0xFFFFFFFF) // Fill with all 32 spells
        {
        }

        [Constructable]
        public WarlockSpellbook(ulong content) : base(content, 0x2253) // Same item ID as necromancer
        {
            Name = "Codex of Shadows";
            Hue = 0x455; // Dark Purple (same as robe)
            Weight = 3.0;
            Layer = Layer.OneHanded;
        }

        public WarlockSpellbook(Serial serial) : base(serial)
        {
        }

        public override SpellbookType SpellbookType => SpellbookType.VystiaWarlock;
        public override int BookOffset => 1128; // Spell IDs 1128-1159 (Fixed: was 1127-1158)
        public override int BookCount => 32;

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

    #region Divination Magic - Oracle Spellbook
    /// <summary>
    /// Crystal Codex - Oracle Spellbook (Crystal Barrens)
    /// Grants +6% energy damage when equipped
    /// </summary>
    public class OracleSpellbook : Spellbook
    {
        [Constructable]
        public OracleSpellbook() : this(0xFFFFFFFF) // Fill with all 32 spells
        {
        }

        [Constructable]
        public OracleSpellbook(ulong content) : base(content, 0xEFA)
        {
            Name = "Crystal Codex";
            Hue = 0x482; // Crystal Blue
            Weight = 3.0;
            Layer = Layer.OneHanded;
        }

        public OracleSpellbook(Serial serial) : base(serial)
        {
        }

        public override SpellbookType SpellbookType => SpellbookType.VystiaOracle;
        public override int BookOffset => 1160; // Spell IDs 1160-1191 (Fixed: was 1159-1190)
        public override int BookCount => 32;

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

    #region Necromancy - Necromancer Spellbook
    /// <summary>
    /// Necronomicon - Necromancer Spellbook (ShadowVoid)
    /// Undead summons last 25% longer when equipped
    /// </summary>
    public class VystiaNecromancerSpellbook : Spellbook
    {
        [Constructable]
        public VystiaNecromancerSpellbook() : this(0xFFFFFFFF) // Fill with all 32 spells
        {
        }

        [Constructable]
        public VystiaNecromancerSpellbook(ulong content) : base(content, 0x2253) // Necro book graphic
        {
            Name = "Necronomicon";
            Hue = 0x455; // Void Black
            Weight = 3.0;
            Layer = Layer.OneHanded;
        }

        public VystiaNecromancerSpellbook(Serial serial) : base(serial)
        {
        }

        public override SpellbookType SpellbookType => SpellbookType.VystiaNecromancer;
        public override int BookOffset => 1192; // Spell IDs 1192-1223 (Fixed: was 1191-1222)
        public override int BookCount => 32;

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

    #region Summoning Magic - Summoner Spellbook
    /// <summary>
    /// Codex of Binding - Summoner Spellbook (Underwater)
    /// Summons have +10% HP when equipped
    /// </summary>
    public class SummonerSpellbook : Spellbook
    {
        [Constructable]
        public SummonerSpellbook() : this(0xFFFFFFFF) // Fill with all 32 spells
        {
        }

        [Constructable]
        public SummonerSpellbook(ulong content) : base(content, 0xEFA)
        {
            Name = "Codex of Binding";
            Hue = 0x555; // Deep Blue
            Weight = 3.0;
            Layer = Layer.OneHanded;
        }

        public SummonerSpellbook(Serial serial) : base(serial)
        {
        }

        public override SpellbookType SpellbookType => SpellbookType.VystiaSummoner;
        public override int BookOffset => 1224; // Spell IDs 1224-1255 (Fixed: was 1223-1254)
        public override int BookCount => 32;

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

    #region Shamanic Magic - Shaman Spellbook
    /// <summary>
    /// Tome of Spirits - Shaman Spellbook (Skyreach/Wilderlands)
    /// Grants +7% energy damage when equipped
    /// </summary>
    public class ShamanSpellbook : Spellbook
    {
        [Constructable]
        public ShamanSpellbook() : this(0xFFFFFFFF) // Fill with all 32 spells
        {
        }

        [Constructable]
        public ShamanSpellbook(ulong content) : base(content, 0xEFA)
        {
            Name = "Tome of Spirits";
            Hue = 0x501; // Storm Blue
            Weight = 3.0;
            Layer = Layer.OneHanded;
        }

        public ShamanSpellbook(Serial serial) : base(serial)
        {
        }

        public override SpellbookType SpellbookType => SpellbookType.VystiaShaman;
        public override int BookOffset => 1256; // Spell IDs 1256-1287 (Fixed: was 1255-1286)
        public override int BookCount => 32;

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

    #region Bardic Magic - Bard Spellbook
    /// <summary>
    /// Songbook of Legends - Bard Spellbook (Multi-regional)
    /// Songs last 15% longer when equipped
    /// </summary>
    public class BardSpellbook : Spellbook
    {
        [Constructable]
        public BardSpellbook() : this(0xFFFFFFFF) // Fill with all 32 spells
        {
        }

        [Constructable]
        public BardSpellbook(ulong content) : base(content, 0xEFA)
        {
            Name = "Songbook of Legends";
            Hue = 0x8A5; // Golden
            Weight = 3.0;
            Layer = Layer.OneHanded;
        }

        public BardSpellbook(Serial serial) : base(serial)
        {
        }

        public override SpellbookType SpellbookType => SpellbookType.VystiaBard;
        public override int BookOffset => 1288; // Spell IDs 1288-1319 (Fixed: was 1287-1318)
        public override int BookCount => 32;

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

    #region Enchanting Magic - Enchanter Spellbook
    /// <summary>
    /// Codex of Enhancement - Enchanter Spellbook (Multi-regional)
    /// Enchantments last 20% longer when equipped
    /// </summary>
    public class EnchanterSpellbook : Spellbook
    {
        [Constructable]
        public EnchanterSpellbook() : this(0xFFFFFFFF) // Fill with all 32 spells
        {
        }

        [Constructable]
        public EnchanterSpellbook(ulong content) : base(content, 0xEFA)
        {
            Name = "Codex of Enhancement";
            Hue = 0x8FD; // Arcane Purple
            Weight = 3.0;
            Layer = Layer.OneHanded;
        }

        public EnchanterSpellbook(Serial serial) : base(serial)
        {
        }

        public override SpellbookType SpellbookType => SpellbookType.VystiaEnchanter;
        public override int BookOffset => 1320; // Spell IDs 1320-1351 (Fixed: was 1319-1350)
        public override int BookCount => 32;

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

    #region Illusion Magic - Illusionist Spellbook
    /// <summary>
    /// Tome of Deception - Illusionist Spellbook (Multi-regional)
    /// Illusions last 25% longer when equipped
    /// </summary>
    public class IllusionistSpellbook : Spellbook
    {
        [Constructable]
        public IllusionistSpellbook() : this(0xFFFFFFFF) // Fill with all 32 spells
        {
        }

        [Constructable]
        public IllusionistSpellbook(ulong content) : base(content, 0xEFA)
        {
            Name = "Tome of Deception";
            Hue = 0x47E; // Silvery
            Weight = 3.0;
            Layer = Layer.OneHanded;
        }

        public IllusionistSpellbook(Serial serial) : base(serial)
        {
        }

        public override SpellbookType SpellbookType => SpellbookType.VystiaIllusionist;
        public override int BookOffset => 1352; // Spell IDs 1352-1383 (Fixed: was 1351-1382)
        public override int BookCount => 32;

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
}
