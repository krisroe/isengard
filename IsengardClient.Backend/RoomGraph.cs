using System.Collections.Generic;
using System.Drawing;
namespace IsengardClient.Backend
{
    public class RoomGraph
    {
        public RoomGraph(MapType mapType, string Name)
        {
            MapType = mapType;
            Rooms = new Dictionary<Room, PointF>();
            this.Name = Name;
        }
        public override string ToString()
        {
            return Name;
        }
        public MapType MapType { get; set; }
        public Dictionary<Room, PointF> Rooms { get; set; }
        public string Name { get; set; }
        public int ScalingFactor { get; set; }
    }
}
