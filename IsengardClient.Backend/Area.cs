using System.Collections.Generic;
namespace IsengardClient.Backend
{
    public class Area
    {
        public bool IsValid { get; set; }
        public int ID { get; set; }
        public int ParentID { get; set; }
        public Area Parent { get; set; }
        public List<Area> Children { get; set; }
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

        public override string ToString()
        {
            return DisplayName;
        }

        public Area(Area Parent)
        {
            this.Parent = Parent;
            if (Parent != null) this.ParentID = Parent.ID;
        }
        public Area(Area copied, Area parent, Dictionary<Area, Area> AreaMapping) : this(parent)
        {
            ID = copied.ID;
            if (copied.Children != null)
            {
                Children = new List<Area>();
                foreach (var next in copied.Children)
                {
                    Children.Add(new Area(next, this, AreaMapping));
                }
            }
            DisplayName = copied.DisplayName;
            AreaMapping[copied] = this;
            TickRoom = copied.TickRoom;
            PawnShop = copied.PawnShop;
            InventorySinkRoomIdentifier = copied.InventorySinkRoomIdentifier;
            InventorySinkRoomObject = copied.InventorySinkRoomObject;
        }
        public static Area GetDefaultHomeArea()
        {
            Area aHome = new Area(null);
            aHome.DisplayName = "Bree";
            aHome.TickRoom = HealingRoom.BreeNortheast;
            aHome.PawnShop = PawnShoppe.BreeNortheast;
            aHome.Children = new List<Area>();

            Area aImladris = new Area(aHome);
            aImladris.DisplayName = "Imladris";
            aImladris.TickRoom = HealingRoom.Imladris;
            aImladris.PawnShop = PawnShoppe.Imladris;
            aHome.Children.Add(aImladris);
            aImladris.Children = new List<Area>();

            Area a = new Area(aImladris);
            a.DisplayName = "Tharbad";
            a.TickRoom = HealingRoom.Tharbad;
            a.PawnShop = PawnShoppe.Tharbad;
            aImladris.Children.Add(a);

            a = new Area(aImladris);
            a.DisplayName = "Esgaroth";
            a.TickRoom = HealingRoom.Esgaroth;
            a.PawnShop = PawnShoppe.Esgaroth;
            aImladris.Children.Add(a);

            return aHome;
        }

        public Area DetermineCommonParentArea(Area other)
        {
            //determine all area parents of the second area
            HashSet<Area> otherAreas = new HashSet<Area>();
            Area aTemp = other;
            while (aTemp != null)
            {
                otherAreas.Add(aTemp);
                aTemp = aTemp.Parent;
            }

            //traverse backward from the current area until a common parent is found
            aTemp = this;
            while (aTemp != null)
            {
                if (otherAreas.Contains(aTemp)) return aTemp;
                aTemp = aTemp.Parent;
            }

            //we should never get here, since all areas should have the home area as a common parent
            return null;
        }

        public List<Area> GetAreaPathBackToHome()
        {
            List<Area> ret = new List<Area>();
            Area aTemp = this;
            while (aTemp != null)
            {
                ret.Add(aTemp);
                aTemp = aTemp.Parent;
            }
            return ret;
        }
    }
}
