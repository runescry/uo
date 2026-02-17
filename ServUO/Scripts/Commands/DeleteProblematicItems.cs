using System;
using Server;
using Server.Items;
using Server.Commands;

namespace Server.Commands
{
    public class DeleteProblematicItems
    {
        public static void Initialize()
        {
            CommandSystem.Register("DeleteBadItems", AccessLevel.Administrator, 
                new CommandEventHandler(DeleteBadItems_OnCommand));
        }

        [Usage("DeleteBadItems [type]")]
        [Description("Deletes items with serialization issues. Use 'all' to delete all problematic types.")]
        public static void DeleteBadItems_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            string typeArg = e.Arguments.Length > 0 ? e.Arguments[0].ToLower() : "all";
            
            int deleted = 0;
            int checked_count = 0;
            
            from.SendMessage("Searching for problematic items...");
            
            // Problematic item types that may have serialization issues
            string[] problematicTypes = new string[]
            {
                "ChainChest", "LeatherGloves", "Necklace", "PlateArms",
                "Ring", "Bracelet", "Earrings", "BaseArmor", "BaseJewel", "BaseWeapon"
            };
            
            foreach (Item item in World.Items.Values)
            {
                if (item == null || item.Deleted)
                    continue;
                
                checked_count++;
                
                string itemType = item.GetType().Name;
                
                // Check if this is a problematic type
                bool isProblematic = false;
                if (typeArg == "all")
                {
                    foreach (string probType in problematicTypes)
                    {
                        if (itemType == probType || itemType.Contains(probType))
                        {
                            isProblematic = true;
                            break;
                        }
                    }
                }
                else
                {
                    isProblematic = itemType.ToLower().Contains(typeArg);
                }
                
                if (isProblematic)
                {
                    try
                    {
                        // Try to access properties that might fail on corrupted items
                        var test = item.Serial;
                        test = item.ItemID;
                        
                        // If we get here, item seems OK, skip it
                        continue;
                    }
                    catch
                    {
                        // Item is corrupted, delete it
                        from.SendMessage($"Deleting corrupted item: {itemType} (Serial: 0x{item.Serial.Value:X8})");
                        item.Delete();
                        deleted++;
                    }
                }
            }
            
            from.SendMessage($"Checked {checked_count} items. Deleted {deleted} problematic items.");
            
            if (deleted > 0)
            {
                from.SendMessage("Server will need to save for changes to take effect.");
            }
        }
        
        [Usage("DeleteBadItemsBySerial <serial>")]
        [Description("Deletes a specific item by serial number (hex format: 0x400000B1)")]
        public static void DeleteBadItemsBySerial_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            
            if (e.Arguments.Length == 0)
            {
                from.SendMessage("Usage: [DeleteBadItemsBySerial <serial>");
                from.SendMessage("Example: [DeleteBadItemsBySerial 0x400000B1");
                return;
            }
            
            string serialStr = e.Arguments[0];
            Serial serial;
            
            if (serialStr.StartsWith("0x") || serialStr.StartsWith("0X"))
            {
                serial = (Serial)Convert.ToInt32(serialStr.Substring(2), 16);
            }
            else
            {
                serial = (Serial)Convert.ToInt32(serialStr);
            }
            
            Item item = World.FindItem(serial);
            
            if (item == null)
            {
                from.SendMessage($"Item with serial {serialStr} not found.");
                return;
            }
            
            from.SendMessage($"Deleting item: {item.GetType().Name} (Serial: {serialStr})");
            item.Delete();
            from.SendMessage("Item deleted. Server will need to save for changes to take effect.");
        }
    }
}
