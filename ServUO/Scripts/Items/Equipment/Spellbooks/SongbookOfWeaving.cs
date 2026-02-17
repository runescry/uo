#if VYSTIA_SONGWEAVING
using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Gumps;

namespace Server.Items
{
    public class SongbookOfWeaving : Item
    {
        [Constructable]
        public SongbookOfWeaving() : base(0x1F2D)
        {
            Name = "Songbook of Weaving (Legacy)";
            Hue = 0x8A5;
            Weight = 1.0;
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;
            if (pm == null)
                return;

            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // must be in pack
                return;
            }

            if (pm.VystiaClassV2 != Server.Custom.VystiaClasses.PlayerClassTypeV2.Bard)
            {
                pm.SendMessage("Only bards can use this songbook.");
                return;
            }

            pm.SendMessage("This legacy songbook has been replaced by a spellbook. Converting...");

            SongweavingSpellbook newBook = new SongweavingSpellbook();

            Container pack = pm.Backpack;
            if (pack != null)
            {
                pack.DropItem(newBook);
            }
            else
            {
                newBook.MoveToWorld(pm.Location, pm.Map);
            }

            Delete();

            pm.CloseGump(typeof(SongweavingFinaleGump));
            pm.SendGump(new SongweavingFinaleGump(pm));
            newBook.DisplayTo(pm);
        }

        public SongbookOfWeaving(Serial serial) : base(serial) { }

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
#endif
