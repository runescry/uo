using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    public class SongweavingSpellbook : Spellbook
    {
        public const int SongweavingBookOffset = 1384;
        public const int SongweavingBookCount = 32;

        [Constructable]
        public SongweavingSpellbook() : this(0xFFFFFFFF) // Fill with all 32 slots
        {
        }

        [Constructable]
        public SongweavingSpellbook(ulong content) : base(content, 0xEFA)
        {
            Name = "Songbook of Weaving";
            Hue = 0x8A5;
            Weight = 3.0;
            Layer = Layer.OneHanded;
        }

        public SongweavingSpellbook(Serial serial) : base(serial)
        {
        }

        public override SpellbookType SpellbookType => SpellbookType.VystiaSongweaving;
        public override int BookOffset => SongweavingBookOffset;
        public override int BookCount => SongweavingBookCount;

        public override bool AllowEquipedCast(Mobile from) => true;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            _ = reader.ReadInt();
        }

    }
}
