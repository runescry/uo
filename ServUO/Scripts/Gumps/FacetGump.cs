using System;
using System.Collections.Generic;
using Server.Network;

namespace Server.Gumps
{
    public class FacetGump : Gump
    {
        private Mobile m_Owner;
        private List<Map> m_AvailableMaps;
        
        public FacetGump(Mobile owner)
            : base(10, 10)
        {
            owner.CloseGump(typeof(FacetGump));

            this.m_Owner = owner;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            // Get all available maps (excluding Internal map)
            m_AvailableMaps = new List<Map>();
            foreach (Map map in Map.AllMaps)
            {
                if (map != null && map != Map.Internal)
                {
                    m_AvailableMaps.Add(map);
                }
            }

            // Calculate gump size based on number of maps
            int gumpHeight = 40 + (m_AvailableMaps.Count * 20);
            int gumpWidth = 200;

            this.AddPage(0);

            this.AddBackground(0, 0, gumpWidth, gumpHeight, 0xA3C);

            // Add title
            this.AddLabel(10, 10, 965, "Select Map:");

            // Add map buttons and labels dynamically
            int buttonY = 30;
            int buttonID = 1;

            foreach (Map map in m_AvailableMaps)
            {
                // Button
                this.AddButton(10, buttonY, 0xA9A, 0xA9A, buttonID, GumpButtonType.Reply, 0);
                
                // Label
                this.AddLabel(30, buttonY, 965, map.Name);
                
                buttonY += 20;
                buttonID++;
            }
        }

        public Mobile Owner
        {
            get
            {
                return this.m_Owner;
            }
            set
            {
                this.m_Owner = value;
            }
        }
        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            int buttonID = info.ButtonID;
            
            if (buttonID > 0 && buttonID <= m_AvailableMaps.Count)
            {
                Map targetMap = m_AvailableMaps[buttonID - 1];
                Point3D location;
                CityInfo city;

                // Define default locations for each map
                switch (targetMap.Name.ToLower())
                {
                    case "felucca":
                        city = new CityInfo("Britain", "Town Center", 1475, 1645, 20);
                        location = city.Location;
                        break;
                    case "trammel":
                        city = new CityInfo("Britain", "Town Center", 1475, 1645, 20);
                        location = city.Location;
                        break;
                    case "ilshenar":
                        city = new CityInfo("Lakeshire", "Town Center", 1203, 1124, -25);
                        location = city.Location;
                        break;
                    case "malas":
                        city = new CityInfo("Luna", "Town Center", 989, 519, -50);
                        location = city.Location;
                        break;
                    case "tokuno":
                        city = new CityInfo("Zento", "Town Center", 735, 1257, 30);
                        location = city.Location;
                        break;
                    case "termur":
                        city = new CityInfo("Test", "Test", 852, 3526, -43);
                        location = city.Location;
                        break;
                    case "vystia":
                        // Default location for Vystia - adjust coordinates as needed
                        location = new Point3D(3584, 2048, 0); // Center of 7168x4096 map
                        break;
                    default:
                        // Default to center of map for unknown maps
                        location = new Point3D(targetMap.Width / 2, targetMap.Height / 2, 0);
                        break;
                }

                from.MoveToWorld(location, targetMap);
            }
        }
    }
}