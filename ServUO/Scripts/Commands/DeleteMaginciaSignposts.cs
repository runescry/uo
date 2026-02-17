using System;
using Server;
using Server.Commands;
using Server.Items;

namespace Server.Commands
{
    public class DeleteMaginciaSignposts
    {
        public static void Initialize()
        {
            CommandSystem.Register("DeleteMaginciaSignposts", AccessLevel.Administrator, new CommandEventHandler(DeleteSignposts_OnCommand));
        }

        [Usage("DeleteMaginciaSignposts")]
        [Description("Deletes all signposts from Magincia area.")]
        private static void DeleteSignposts_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            int deleted = 0;

            // Magincia area bounds (approximate)
            int x1 = 3600, y1 = 2000;
            int x2 = 3900, y2 = 2400;

            from.SendMessage("Searching for signposts in Magincia area...");

            // Signpost Item IDs
            int[] signpostIDs = { 0x0BA0, 0x0B9D, 0x0BA2, 2978, 2977, 2971, 2970 };

            foreach (int itemID in signpostIDs)
            {
                foreach (Item item in World.Items.Values)
                {
                    if (item.ItemID == itemID && 
                        item.Map == Map.Trammel &&
                        item.X >= x1 && item.X <= x2 &&
                        item.Y >= y1 && item.Y <= y2)
                    {
                        item.Delete();
                        deleted++;
                    }
                }
            }

            from.SendMessage("Deleted {0} signposts from Magincia area.", deleted);
        }
    }
}
