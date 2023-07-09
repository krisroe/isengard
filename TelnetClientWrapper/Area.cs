using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsengardClient
{
    internal class Area
    {
        public Area()
        {
        }
        public Area(Area copied)
        {
            DisplayName = copied.DisplayName;
            TickRoom = copied.TickRoom;
            PawnShop = copied.PawnShop;
            InventorySinkRoomIdentifier = copied.InventorySinkRoomIdentifier;
            InventorySinkRoomObject = copied.InventorySinkRoomObject;
        }
        public int ID { get; set; }
        public string DisplayName { get; set; }
        public HealingRoom? TickRoom { get; set; }
        public PawnShoppe? PawnShop { get; set; }
        /// <summary>
        /// inventory sink room identifier
        /// </summary>
        public string InventorySinkRoomIdentifier { get; set; }
        /// <summary>
        /// inventory sink room object
        /// </summary>
        public Room InventorySinkRoomObject { get; set; }

        public static IEnumerable<Area> GetDefaultAreas()
        {
            Area a;

            a = new Area();
            a.DisplayName = "Bree";
            a.TickRoom = HealingRoom.BreeNortheast;
            a.PawnShop = PawnShoppe.BreeNortheast;
            yield return a;

            a = new Area();
            a.DisplayName = "Imladris";
            a.TickRoom = HealingRoom.Imladris;
            a.PawnShop = PawnShoppe.Imladris;
            yield return a;

            a = new Area();
            a.DisplayName = "Tharbad";
            a.TickRoom = HealingRoom.Tharbad;
            a.PawnShop = PawnShoppe.Tharbad;
            yield return a;

            a = new Area();
            a.DisplayName = "Esgaroth";
            a.TickRoom = HealingRoom.Esgaroth;
            a.PawnShop = PawnShoppe.Esgaroth;
            yield return a;
        }
    }
}
