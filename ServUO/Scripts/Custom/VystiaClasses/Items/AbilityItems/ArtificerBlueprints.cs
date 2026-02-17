using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Items.VystiaClassItems
{
    /// <summary>
    /// Artificer Blueprints - Reference book for construct designs
    /// </summary>
    public class ArtificerBlueprints : Item
    {
        [Constructable]
        public ArtificerBlueprints() : base(0x14F0) // Book item ID
        {
            Name = "Artificer Blueprints";
            Hue = 2305; // Metallic
            Weight = 3.0;
            LootType = LootType.Blessed;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1070722, "Contains clockwork construct designs");
            list.Add("Double-click to view blueprints");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack
                return;
            }

            from.SendMessage(0x3B2, "You study your artificer blueprints...");
            from.SendGump(new ArtificerBlueprintsGump());
        }

        public ArtificerBlueprints(Serial serial) : base(serial)
        {
        }

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

    /// <summary>
    /// Gump displaying construct blueprints
    /// </summary>
    public class ArtificerBlueprintsGump : Gump
    {
        public ArtificerBlueprintsGump() : base(50, 50)
        {
            AddPage(0);
            AddBackground(0, 0, 500, 400, 9200);
            AddLabel(180, 20, 2305, "Artificer Blueprints");
            AddLabel(160, 40, 0, "Clockwork Construct Designs");

            // Page 1 - Clockwork Scout
            AddPage(1);
            AddLabel(50, 70, 2305, "Clockwork Scout");
            AddHtml(50, 95, 400, 120,
                "<BASEFONT COLOR=#FFFFFF>" +
                "<B>Type:</B> Basic Combat Construct<br>" +
                "<B>Body:</B> Golem Frame (752)<br>" +
                "<B>Power Source:</B> Clockwork Gears<br>" +
                "<B>Duration:</B> 10 minutes<br>" +
                "<B>Stats:</B> 80-100 STR, 60-80 DEX, 30-50 INT<br>" +
                "<B>Damage:</B> 8-12 Physical<br>" +
                "<B>Special:</B> Poison Immune<br>" +
                "<B>Control Slots:</B> 2" +
                "</BASEFONT>", false, false);

            AddLabel(50, 230, 0, "Materials Required:");
            AddHtml(50, 250, 400, 50,
                "<BASEFONT COLOR=#FFFFFF>" +
                "- Construct Control Device (1 charge)<br>" +
                "- ClockworkGear (various)<br>" +
                "- ClockworkSpring (various)" +
                "</BASEFONT>", false, false);

            AddButton(420, 350, 4005, 4007, 0, GumpButtonType.Page, 2);
            AddLabel(455, 350, 0, "Next");

            // Page 2 - Mechanical Turret (Future)
            AddPage(2);
            AddLabel(50, 70, 2305, "Mechanical Turret");
            AddHtml(50, 95, 400, 120,
                "<BASEFONT COLOR=#808080>" +
                "<B>Type:</B> Stationary Defense<br>" +
                "<B>Status:</B> BLUEPRINT IN DEVELOPMENT<br><br>" +
                "A stationary turret that automatically fires at enemies.<br>" +
                "Requires advanced clockwork mechanisms and targeting systems.<br><br>" +
                "<I>Check back with the Technoguild for updates.</I>" +
                "</BASEFONT>", false, false);

            AddButton(50, 350, 4014, 4016, 0, GumpButtonType.Page, 1);
            AddLabel(85, 350, 0, "Back");
            AddButton(420, 350, 4005, 4007, 0, GumpButtonType.Page, 3);
            AddLabel(455, 350, 0, "Next");

            // Page 3 - Steam Golem (Future)
            AddPage(3);
            AddLabel(50, 70, 2305, "Steam Golem");
            AddHtml(50, 95, 400, 120,
                "<BASEFONT COLOR=#808080>" +
                "<B>Type:</B> Heavy Combat Construct<br>" +
                "<B>Status:</B> BLUEPRINT IN DEVELOPMENT<br><br>" +
                "A massive steam-powered golem with incredible strength.<br>" +
                "Requires steam core, reinforced plating, and hydraulic systems.<br><br>" +
                "<I>Check back with the Technoguild for updates.</I>" +
                "</BASEFONT>", false, false);

            AddButton(50, 350, 4014, 4016, 0, GumpButtonType.Page, 2);
            AddLabel(85, 350, 0, "Back");
            AddButton(420, 350, 4005, 4007, 0, GumpButtonType.Page, 4);
            AddLabel(455, 350, 0, "Next");

            // Page 4 - Repair Spider (Future)
            AddPage(4);
            AddLabel(50, 70, 2305, "Repair Spider");
            AddHtml(50, 95, 400, 120,
                "<BASEFONT COLOR=#808080>" +
                "<B>Type:</B> Utility Construct<br>" +
                "<B>Status:</B> BLUEPRINT IN DEVELOPMENT<br><br>" +
                "A small spider-like construct that repairs other constructs.<br>" +
                "Equipped with fine manipulators and welding tools.<br><br>" +
                "<I>Check back with the Technoguild for updates.</I>" +
                "</BASEFONT>", false, false);

            AddButton(50, 350, 4014, 4016, 0, GumpButtonType.Page, 3);
            AddLabel(85, 350, 0, "Back");

            // Close button on all pages
            AddButton(250, 350, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddLabel(285, 350, 0, "Close");
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            // Close gump (no action needed)
        }
    }
}
