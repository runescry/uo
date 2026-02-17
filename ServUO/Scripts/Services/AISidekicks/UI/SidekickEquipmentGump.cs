using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Services.AISidekicks;

namespace Server.Services.AISidekicks.UI
{
    /// <summary>
    /// Gump for managing sidekick equipment and inventory
    /// </summary>
    public class SidekickEquipmentGump : Gump
    {
        private const int GumpWidth = 500;
        private const int GumpHeight = 400;

        private AutonomousSidekick m_Sidekick;
        private PlayerMobile m_Player;

        public SidekickEquipmentGump(PlayerMobile player, AutonomousSidekick sidekick) : base(50, 50)
        {
            m_Player = player;
            m_Sidekick = sidekick;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            BuildGump();
        }

        private void BuildGump()
        {
            AddPage(0);

            // Background
            AddBackground(0, 0, GumpWidth, GumpHeight, 9200);
            AddImageTiled(10, 10, GumpWidth - 20, GumpHeight - 20, 2624);
            AddAlphaRegion(10, 10, GumpWidth - 20, GumpHeight - 20);

            // Title
            AddHtml(20, 20, GumpWidth - 40, 30, $"<center><basefont color=#FFFFFF size=6><b>{m_Sidekick.Name}'s Equipment</b></basefont></center>", false, false);

            int y = 60;
            int x = 30;

            // Equipment slots
            AddHtml(x, y, 200, 25, "<basefont color=#FFFFFF size=4><b>Equipment Slots:</b></basefont>", false, false);
            y += 30;

            // Display equipment by layer
            Dictionary<Layer, Item> equipment = GetEquipment();
            int slotIndex = 0;

            foreach (var kvp in equipment)
            {
                Layer layer = kvp.Key;
                Item item = kvp.Value;

                string slotName = GetLayerName(layer);
                string itemName = item != null ? item.Name : "Empty";

                AddHtml(x, y, 150, 20, $"<basefont color=#FFFFFF size=3>{slotName}:</basefont>", false, false);
                AddHtml(x + 150, y, 250, 20, $"<basefont color=#FFFF00 size=3>{itemName}</basefont>", false, false);

                if (item != null)
                {
                    AddButton(x + 400, y, 4017, 4019, 100 + slotIndex, GumpButtonType.Reply, 0); // Remove button
                }

                y += 25;
                slotIndex++;
            }

            y += 10;
            AddHtml(x, y, GumpWidth - 60, 25, "<basefont color=#FFFFFF size=4><b>Instructions:</b></basefont>", false, false);
            y += 25;
            AddHtml(x, y, GumpWidth - 60, 60, "<basefont color=#CCCCCC size=3>Drag items from your inventory onto this sidekick to equip them.<br>Click 'Remove' to unequip items.<br>Double-click the sidekick to access their backpack.</basefont>", false, false);

            // Close button
            int buttonY = GumpHeight - 50;
            AddButton(GumpWidth / 2 - 60, buttonY, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddHtml(GumpWidth / 2 - 30, buttonY + 3, 120, 25, "<center><basefont color=#FF0000 size=4><b>Close</b></basefont></center>", false, false);
        }

        private Dictionary<Layer, Item> GetEquipment()
        {
            Dictionary<Layer, Item> equipment = new Dictionary<Layer, Item>();

            // Standard equipment layers
            Layer[] layers = {
                Layer.Cloak, Layer.Bracelet, Layer.Ring, Layer.Shirt, Layer.Pants,
                Layer.InnerLegs, Layer.Shoes, Layer.Arms, Layer.InnerTorso,
                Layer.MiddleTorso, Layer.OuterLegs, Layer.Neck, Layer.Waist,
                Layer.Gloves, Layer.OuterTorso, Layer.OneHanded, Layer.TwoHanded,
                Layer.FacialHair, Layer.Hair, Layer.Helm
            };

            foreach (Layer layer in layers)
            {
                Item item = m_Sidekick.FindItemOnLayer(layer);
                if (item != null || IsEquipmentSlot(layer))
                {
                    equipment[layer] = item;
                }
            }

            return equipment;
        }

        private bool IsEquipmentSlot(Layer layer)
        {
            // Only show actual equipment slots, not decorative layers
            switch (layer)
            {
                case Layer.Cloak:
                case Layer.Bracelet:
                case Layer.Ring:
                case Layer.Shirt:
                case Layer.Pants:
                case Layer.InnerLegs:
                case Layer.Shoes:
                case Layer.Arms:
                case Layer.InnerTorso:
                case Layer.MiddleTorso:
                case Layer.OuterLegs:
                case Layer.Neck:
                case Layer.Waist:
                case Layer.Gloves:
                case Layer.OuterTorso:
                case Layer.OneHanded:
                case Layer.TwoHanded:
                case Layer.Helm:
                    return true;
                default:
                    return false;
            }
        }

        private string GetLayerName(Layer layer)
        {
            switch (layer)
            {
                case Layer.Cloak: return "Cloak";
                case Layer.Bracelet: return "Bracelet";
                case Layer.Ring: return "Ring";
                case Layer.Shirt: return "Shirt";
                case Layer.Pants: return "Pants";
                case Layer.InnerLegs: return "Legs";
                case Layer.Shoes: return "Shoes";
                case Layer.Arms: return "Arms";
                case Layer.InnerTorso: return "Chest";
                case Layer.MiddleTorso: return "Middle";
                case Layer.OuterLegs: return "Outer Legs";
                case Layer.Neck: return "Neck";
                case Layer.Waist: return "Waist";
                case Layer.Gloves: return "Gloves";
                case Layer.OuterTorso: return "Outer Torso";
                case Layer.OneHanded: return "Right Hand";
                case Layer.TwoHanded: return "Two Handed";
                case Layer.Helm: return "Helm";
                default: return layer.ToString();
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (from == null || from != m_Player)
                return;

            if (info.ButtonID == 0)
            {
                // Close
                return;
            }

            // Remove equipment (buttons 100+)
            if (info.ButtonID >= 100)
            {
                int slotIndex = info.ButtonID - 100;
                Dictionary<Layer, Item> equipment = GetEquipment();
                List<Layer> layers = new List<Layer>(equipment.Keys);

                if (slotIndex >= 0 && slotIndex < layers.Count)
                {
                    Layer layer = layers[slotIndex];
                    Item item = equipment[layer];

                    if (item != null)
                    {
                        m_Sidekick.RemoveItem(item);
                        m_Player.AddToBackpack(item);
                        m_Player.SendMessage($"You removed {item.Name} from {m_Sidekick.Name}.");
                        
                        // Refresh gump
                        from.SendGump(new SidekickEquipmentGump(m_Player, m_Sidekick));
                    }
                }
            }
        }
    }
}

